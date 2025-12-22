using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LettuceTutorialOneMissionEntity : LettuceMissionEntity
{
  private static readonly Map<GameEntityOption, bool> s_booleanOptions = new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      false
    },
    {
      GameEntityOption.DISABLE_MANUAL_DISMISSAL_OF_MERC_ABILITY_TRAY,
      true
    }
  };
  private static readonly Map<GameEntityOption, string> s_stringOptions = new Map<GameEntityOption, string>();
  private Notification m_clickChampionNotification;
  private Notification m_queueAbilityNotification;
  private Notification endTurnNotifier;
  private LettuceTutorialOneMissionEntity.TutorialStep m_currentTutorialStep;
  private readonly List<Card> m_clickBlockedCards = new List<Card>();
  private static readonly AssetReference GiantRat_LOOTA_BOSS_18h_EmoteResponse = new AssetReference("GiantRat_LOOTA_BOSS_18h_EmoteResponse.prefab:323ab0c47034e8043b688bb368fa912c");
  private TooltipPanel attackHelpPanel;
  private TooltipPanel healthHelpPanel;
  private PlatformDependentValue<Vector3> m_attackTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(-2.35f, 0.0f, -0.62f),
    Phone = new Vector3(-3.5f, 0.0f, -0.62f)
  };
  private PlatformDependentValue<Vector3> m_healthTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(2.25f, 0.0f, -0.62f),
    Phone = new Vector3(3.25f, 0.0f, -0.62f)
  };
  private PlatformDependentValue<float> m_gemScale = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 1.75f,
    Phone = 1.2f
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LettuceTutorialOneMissionEntity.GiantRat_LOOTA_BOSS_18h_EmoteResponse
    })
      this.PreloadSound(soundPath);
  }

  public LettuceTutorialOneMissionEntity()
    : base()
  {
    this.m_gameOptions.AddOptions(LettuceTutorialOneMissionEntity.s_booleanOptions, LettuceTutorialOneMissionEntity.s_stringOptions);
    this.m_abilityOrderSpeechBubblesEnabled = true;
    this.m_enemyAbilityOrderSpeechBubblesEnabled = false;
  }

  protected override void OnLettuceMissionEntityReconnect(int currentTurn)
  {
    switch (currentTurn)
    {
      case 1:
        Card minionInFriendlyPlay1 = this.GetLeftMostMinionInFriendlyPlay();
        int? lettuceAbilityId;
        int num1;
        if (minionInFriendlyPlay1 == null)
        {
          num1 = 0;
        }
        else
        {
          lettuceAbilityId = minionInFriendlyPlay1.GetEntity()?.GetSelectedLettuceAbilityID();
          int num2 = 0;
          num1 = lettuceAbilityId.GetValueOrDefault() == num2 & lettuceAbilityId.HasValue ? 1 : 0;
        }
        if (num1 != 0)
        {
          this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.CLICK_FIRST_MERCENARY);
          break;
        }
        Card minionInFriendlyPlay2 = this.GetRightMostMinionInFriendlyPlay();
        int num3;
        if (minionInFriendlyPlay2 == null)
        {
          num3 = 0;
        }
        else
        {
          lettuceAbilityId = minionInFriendlyPlay2.GetEntity()?.GetSelectedLettuceAbilityID();
          int num4 = 0;
          num3 = lettuceAbilityId.GetValueOrDefault() == num4 & lettuceAbilityId.HasValue ? 1 : 0;
        }
        if (num3 != 0)
        {
          this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.CLICK_SECOND_MERCENARY);
          break;
        }
        this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.FIRST_TURN_END_READY);
        break;
      case 2:
        if (this.IsAnyFriendlyAbilitySelected())
          break;
        this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.EXPLAIN_ATTACK_TYPE);
        break;
    }
  }

  public override void OnDecommissionGame()
  {
    this.DestroyAllTutorialPopUps();
    base.OnDecommissionGame();
  }

  private void SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep step)
  {
    this.m_currentTutorialStep = step;
    GameEntity.Coroutines.StartCoroutine(this.TransitionTutorialStepCoroutine(step));
  }

  private IEnumerator TransitionTutorialStepCoroutine(
    LettuceTutorialOneMissionEntity.TutorialStep step)
  {
    LettuceTutorialOneMissionEntity oneMissionEntity = this;
    if (!GameMgr.Get().IsSpectator())
    {
      switch (step)
      {
        case LettuceTutorialOneMissionEntity.TutorialStep.CLICK_FIRST_MERCENARY:
          LettuceTutorialOneMissionEntity.SetEndTurnEnableAndBlocker(false);
          oneMissionEntity.AddClickBlockerForFriendlyMinions();
          Card minionInFriendlyPlay1 = oneMissionEntity.GetLeftMostMinionInFriendlyPlay();
          if (!((Object) minionInFriendlyPlay1 != (Object) null))
            break;
          oneMissionEntity.m_clickBlockedCards.Remove(minionInFriendlyPlay1);
          oneMissionEntity.ShowClickChampionTutorial(minionInFriendlyPlay1);
          break;
        case LettuceTutorialOneMissionEntity.TutorialStep.SELECT_FIRST_ABILITY:
          oneMissionEntity.AddInputBlockerFriendlyAbilityZone();
          yield return (object) oneMissionEntity.WaitForAbilityToLoad();
          oneMissionEntity.AddClickBlockerForFriendlyMinions();
          oneMissionEntity.RemoveInputBlockerFriendlyAbilityZone();
          yield return (object) new WaitForSeconds(0.5f);
          Card abilityButtonBySlot1 = oneMissionEntity.GetAbilityButtonBySlot(0);
          if ((Object) abilityButtonBySlot1 != (Object) null)
            oneMissionEntity.ShowQueueAbilityTutorial(abilityButtonBySlot1);
          yield return (object) oneMissionEntity.WaitForMinionAbilitySelection(oneMissionEntity.GetLeftMostMinionInFriendlyPlay(), LettuceTutorialOneMissionEntity.TutorialStep.CLICK_SECOND_MERCENARY);
          oneMissionEntity.DestroyNotification(oneMissionEntity.m_queueAbilityNotification);
          break;
        case LettuceTutorialOneMissionEntity.TutorialStep.CLICK_SECOND_MERCENARY:
          Card minionInFriendlyPlay2 = oneMissionEntity.GetRightMostMinionInFriendlyPlay();
          if (!((Object) minionInFriendlyPlay2 != (Object) null))
            break;
          oneMissionEntity.m_clickBlockedCards.Remove(minionInFriendlyPlay2);
          oneMissionEntity.ShowClickChampionTutorial(minionInFriendlyPlay2);
          break;
        case LettuceTutorialOneMissionEntity.TutorialStep.SELECT_SECOND_ABILITY:
          GameState.Get().SetBusy(true);
          yield return (object) oneMissionEntity.WaitForAbilityToLoad();
          oneMissionEntity.AddClickBlockerForFriendlyMinions();
          GameState.Get().SetBusy(false);
          yield return (object) new WaitForSeconds(0.5f);
          Card abilityButtonBySlot2 = oneMissionEntity.GetAbilityButtonBySlot(0);
          if ((Object) abilityButtonBySlot2 != (Object) null)
            oneMissionEntity.ShowQueueAbilityTutorial(abilityButtonBySlot2);
          yield return (object) oneMissionEntity.WaitForMinionAbilitySelection(oneMissionEntity.GetRightMostMinionInFriendlyPlay(), LettuceTutorialOneMissionEntity.TutorialStep.FIRST_TURN_END_READY);
          oneMissionEntity.DestroyNotification(oneMissionEntity.m_queueAbilityNotification);
          break;
        case LettuceTutorialOneMissionEntity.TutorialStep.FIRST_TURN_END_READY:
          oneMissionEntity.DestroyAllTutorialPopUps();
          oneMissionEntity.AddClickBlockerForFriendlyMinions();
          oneMissionEntity.AddInputBlockerFriendlyAbilityZone();
          LettuceTutorialOneMissionEntity.SetEndTurnEnableAndBlocker(true);
          yield return (object) new WaitForSeconds(1f);
          oneMissionEntity.ShowEndTurnBouncingArrow();
          break;
        case LettuceTutorialOneMissionEntity.TutorialStep.FIRST_COMBAT_START:
          GameState.Get().SetBusy(true);
          oneMissionEntity.RemoveClickBlockerForFriendlyMinions();
          oneMissionEntity.RemoveInputBlockerFriendlyAbilityZone();
          oneMissionEntity.CreateTutorialDialog(LettuceTutorialResources.LettuceTutorialPopupCombatFlowPrefab, "GAMEPLAY_LETTUCE_COMBAT_TITLE_TUTORIAL", "GAMEPLAY_LETTUCE_COMBAT_BODY_TUTORIAL", "GAMEPLAY_LETTUCE_COMBAT_BUTTON_TUTORIAL", new UIEvent.Handler(oneMissionEntity.UserPressedCombatTutorial), Vector2.zero);
          break;
        case LettuceTutorialOneMissionEntity.TutorialStep.EXPLAIN_ATTACK_TYPE:
          GameState.Get().SetBusy(true);
          oneMissionEntity.CreateTutorialDialog(LettuceTutorialResources.LettuceTutorialPopupAbilitiesPrefab, "GAMEPLAY_LETTUCE_ATTACK_TYPE_TITLE_TUTORIAL", "GAMEPLAY_LETTUCE_ATTACK_TYPE_BODY_TUTORIAL", "GAMEPLAY_LETTUCE_ATTACK_TYPE_BUTTON_TUTORIAL", new UIEvent.Handler(oneMissionEntity.UserPressedAttackTutorial), Vector2.zero);
          break;
      }
    }
  }

  private void ShowClickChampionTutorial(Card card, string textID = "GAMEPLAY_CHAMPION_CLICK_TUTORIAL", bool hideImmediately = false)
  {
    if ((Object) card == (Object) null)
      return;
    Vector3 position1 = card.transform.position;
    Vector3 position2;
    Notification.PopUpArrowDirection direction;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      position2 = new Vector3(position1.x + 0.05f, position1.y, position1.z + 2.9f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    else
    {
      position2 = new Vector3(position1.x, position1.y, position1.z + 2.5f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    this.m_clickChampionNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(textID));
    this.m_clickChampionNotification.ShowPopUpArrow(direction);
    this.m_clickChampionNotification.PulseReminderEveryXSeconds(2f);
  }

  private void ShowQueueAbilityTutorial(Card abilityCard = null, string textID = "GAMEPLAY_QUEUE_ABILITY_TUTORIAL", bool hideImmediately = false)
  {
    if ((Object) abilityCard == (Object) null)
      return;
    Vector3 position1 = abilityCard.GetActor().transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z + 2.25f) : new Vector3(position1.x, position1.y, position1.z + 2.5f);
    this.m_queueAbilityNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(textID));
    this.m_queueAbilityNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
    this.m_queueAbilityNotification.PulseReminderEveryXSeconds(2f);
  }

  public override void NotifyOfStartOfTurnEventsFinished()
  {
    switch (this.GetTag(GAME_TAG.TURN))
    {
      case 1:
        this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.CLICK_FIRST_MERCENARY);
        break;
      case 2:
        this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.EXPLAIN_ATTACK_TYPE);
        break;
    }
  }

  private void ShowEndTurnBouncingArrow()
  {
    if (EndTurnButton.Get().IsInWaitingState())
      return;
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 2f, position1.y, position1.z);
    NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, -90f, 0.0f));
  }

  public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
  {
    if (this.m_clickBlockedCards.Contains(clickedEntity.GetCard()) || clickedEntity.IsControlledByOpposingSidePlayer() && !GameState.Get().IsInTargetMode())
      return false;
    this.DestroyNotification(this.m_clickChampionNotification);
    if (clickedEntity.IsLettuceAbility())
      this.DestroyNotification(this.m_queueAbilityNotification);
    switch (this.m_currentTutorialStep)
    {
      case LettuceTutorialOneMissionEntity.TutorialStep.CLICK_FIRST_MERCENARY:
        this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.SELECT_FIRST_ABILITY);
        break;
      case LettuceTutorialOneMissionEntity.TutorialStep.CLICK_SECOND_MERCENARY:
        this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.SELECT_SECOND_ABILITY);
        break;
    }
    return true;
  }

  public override bool NotifyOfEndTurnButtonPushed()
  {
    this.DestroyAllTutorialPopUps();
    int tag = this.GetTag(GAME_TAG.TURN);
    if (tag == 1)
    {
      this.SetTutorialStep(LettuceTutorialOneMissionEntity.TutorialStep.FIRST_COMBAT_START);
      return true;
    }
    if (tag < 2)
      return false;
    if (EndTurnButton.Get().HasNoMorePlays())
      return true;
    if ((Object) this.endTurnNotifier != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
    string key = "GAMEPLAY_LETTUCE_NO_ENDTURN";
    this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
    return false;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    yield return (object) base.HandleGameOverWithTiming(gameResult);
    this.DestroyAllTutorialPopUps();
    yield return (object) null;
  }

  private IEnumerator WaitForMinionAbilitySelection(
    Card minionCard,
    LettuceTutorialOneMissionEntity.TutorialStep nextTutorialStep)
  {
    Entity minionEnt = minionCard.GetEntity();
    int currentSelectedAbilityID = minionEnt.GetSelectedLettuceAbilityID();
    while (minionEnt.GetSelectedLettuceAbilityID() == currentSelectedAbilityID)
      yield return (object) null;
    this.SetTutorialStep(nextTutorialStep);
  }

  private IEnumerator WaitForAbilityToLoad()
  {
    while (!ZoneMgr.Get().IsMercenariesAbilityTrayVisible())
      yield return (object) null;
  }

  private void AddClickBlockerForFriendlyMinions()
  {
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards())
    {
      if (!this.m_clickBlockedCards.Contains(card))
        this.m_clickBlockedCards.Add(card);
    }
  }

  private void RemoveClickBlockerForFriendlyMinions()
  {
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards())
    {
      if (this.m_clickBlockedCards.Contains(card))
        this.m_clickBlockedCards.Remove(card);
    }
  }

  private void DestroyAllTutorialPopUps()
  {
    NotificationManager.Get().DestroyAllArrows();
    NotificationManager.Get().DestroyAllPopUps();
    NotificationManager.Get().DestroyAllNotificationsNowWithNoAnim();
  }

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    base.NotifyOfCardMousedOver(mousedOverEntity);
    if (!mousedOverEntity.IsLettuceAbility())
      return;
    this.DestroyNotification(this.m_queueAbilityNotification);
  }

  private void UserPressedCombatTutorial(UIEvent e) => GameState.Get().SetBusy(false);

  private void UserPressedAttackTutorial(UIEvent e) => GameState.Get().SetBusy(false);

  private static void SetEndTurnEnableAndBlocker(bool isEnabled)
  {
    if (isEnabled)
    {
      EndTurnButton.Get().RemoveInputBlocker();
      EndTurnButton.Get().SetDisabled(false);
      EndTurnButton.Get().Reset();
    }
    else
    {
      EndTurnButton.Get().AddInputBlocker();
      EndTurnButton.Get().SetDisabled(true);
    }
  }

  public override void NotifyOfCardTooltipBigCardActorShow()
  {
    Card card = BigCard.Get()?.GetCard();
    if ((Object) card == (Object) null)
      return;
    Actor actor = (Actor) null;
    if (card.IsLettuceAbility())
      actor = BigCard.Get()?.GetBigCardActor();
    else if (card.GetEntity().IsMinion())
      actor = BigCard.Get()?.GetExtraBigCardActor();
    if (!((Object) actor != (Object) null))
      return;
    actor.GetCostTextObject().SetActive(false);
    actor.GetCostText().Text = (string) null;
  }

  public override bool NotifyOfCardTooltipDisplayShow(Card card)
  {
    if (GameState.Get().IsGameOver())
      return false;
    if (!card.GetEntity().IsMinion())
      return true;
    if ((Object) this.attackHelpPanel == (Object) null)
    {
      this.ShowAttackTooltip(card);
      Gameplay.Get().StartCoroutine(this.ShowHealthTooltipAfterWait(card));
    }
    return false;
  }

  private void ShowAttackTooltip(Card card)
  {
    LayerUtils.SetLayer(card.GetActor().GetAttackObject().gameObject, GameLayer.Tooltip);
    Vector3 position = card.transform.position;
    Vector3 attackTooltipPosition = (Vector3) this.m_attackTooltipPosition;
    Vector3 vector3 = new Vector3(position.x + attackTooltipPosition.x, position.y + attackTooltipPosition.y, position.z + attackTooltipPosition.z);
    if ((Object) this.attackHelpPanel != (Object) null)
      Object.Destroy((Object) this.attackHelpPanel.gameObject);
    this.attackHelpPanel = TooltipPanelManager.Get().CreateKeywordPanel(0);
    this.attackHelpPanel.Reset();
    this.attackHelpPanel.Initialize(GameStrings.Get("GLOBAL_ATTACK"), GameStrings.Get("TUTORIAL01_HELP_12"));
    this.attackHelpPanel.SetScale((float) TooltipPanel.GAMEPLAY_SCALE);
    this.attackHelpPanel.transform.position = vector3;
    RenderUtils.SetAlpha(this.attackHelpPanel.gameObject, 0.0f);
    iTween.FadeTo(this.attackHelpPanel.gameObject, iTween.Hash((object) "alpha", (object) 1, (object) "time", (object) 0.25f));
    card.GetActor().GetAttackObject().Enlarge((float) this.m_gemScale);
    card.GetActor().GetComponentInChildren<LettuceMinionInPlayFrame>()?.EnlargeAttackBauble((float) this.m_gemScale);
  }

  private IEnumerator ShowHealthTooltipAfterWait(Card card)
  {
    yield return (object) new WaitForSeconds(0.05f);
    if (!((Object) InputManager.Get().GetMousedOverCard() != (Object) card))
      this.ShowHealthTooltip(card);
  }

  private void ShowHealthTooltip(Card card)
  {
    LayerUtils.SetLayer(card.GetActor().GetHealthObject().gameObject, GameLayer.Tooltip);
    Vector3 position = card.transform.position;
    Vector3 healthTooltipPosition = (Vector3) this.m_healthTooltipPosition;
    Vector3 vector3 = new Vector3(position.x + healthTooltipPosition.x, position.y + healthTooltipPosition.y, position.z + healthTooltipPosition.z);
    if ((Object) this.healthHelpPanel != (Object) null)
      Object.Destroy((Object) this.healthHelpPanel.gameObject);
    this.healthHelpPanel = TooltipPanelManager.Get().CreateKeywordPanel(0);
    this.healthHelpPanel.Reset();
    this.healthHelpPanel.Initialize(GameStrings.Get("GLOBAL_HEALTH"), GameStrings.Get("TUTORIAL01_HELP_13"));
    this.healthHelpPanel.SetScale((float) TooltipPanel.GAMEPLAY_SCALE);
    this.healthHelpPanel.transform.position = vector3;
    RenderUtils.SetAlpha(this.healthHelpPanel.gameObject, 0.0f);
    iTween.FadeTo(this.healthHelpPanel.gameObject, iTween.Hash((object) "alpha", (object) 1, (object) "time", (object) 0.25f));
    card.GetActor().GetHealthObject().Enlarge((float) this.m_gemScale);
    card.GetActor().GetComponentInChildren<LettuceMinionInPlayFrame>()?.EnlargeHealthBauble((float) this.m_gemScale);
  }

  public override void NotifyOfCardTooltipDisplayHide(Card card)
  {
    if ((Object) card == (Object) null)
      return;
    Actor actor = card.GetActor();
    if ((Object) actor == (Object) null)
      return;
    LettuceMinionInPlayFrame componentInChildren = actor.GetComponentInChildren<LettuceMinionInPlayFrame>();
    if ((Object) this.attackHelpPanel != (Object) null)
    {
      GemObject attackObject = actor.GetAttackObject();
      if ((Object) attackObject != (Object) null)
      {
        LayerUtils.SetLayer(attackObject.gameObject, GameLayer.Default);
        attackObject.Shrink();
      }
      if ((Object) componentInChildren != (Object) null)
        componentInChildren.ShrinkAttackBauble();
      Object.Destroy((Object) this.attackHelpPanel.gameObject);
    }
    if (!((Object) this.healthHelpPanel != (Object) null))
      return;
    GemObject healthObject = actor.GetHealthObject();
    if ((Object) healthObject != (Object) null)
    {
      LayerUtils.SetLayer(healthObject.gameObject, GameLayer.Default);
      healthObject.Shrink();
    }
    if ((Object) componentInChildren != (Object) null)
      componentInChildren.ShrinkHealthBauble();
    Object.Destroy((Object) this.healthHelpPanel.gameObject);
  }

  private enum TutorialStep
  {
    Invalid,
    INTRO_VO,
    CLICK_FIRST_MERCENARY,
    SELECT_FIRST_ABILITY,
    CLICK_SECOND_MERCENARY,
    SELECT_SECOND_ABILITY,
    FIRST_TURN_END_READY,
    FIRST_COMBAT_START,
    EXPLAIN_ATTACK_TYPE,
  }
}
