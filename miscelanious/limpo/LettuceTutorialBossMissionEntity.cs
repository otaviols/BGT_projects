using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LettuceTutorialBossMissionEntity : LettuceMissionEntity
{
  private static readonly Map<GameEntityOption, bool> s_booleanOptions = new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      false
    }
  };
  private static readonly Map<GameEntityOption, string> s_stringOptions = new Map<GameEntityOption, string>();
  private LettuceTutorialBossMissionEntity.TutorialStep m_currentTutorialStep;
  private Notification m_clickChampionNotification;
  private readonly List<Card> m_clickBlockedCards = new List<Card>();
  private static readonly AssetReference Valeera_BrassRing_Quote = new AssetReference("Valeera_BrassRing_Quote.prefab:170a2a9fe4b70f04aa2a058f3a27ba7b");
  private static readonly AssetReference VO_TUTORIAL_01_HOGGER_02_02 = new AssetReference("VO_TUTORIAL_01_HOGGER_02_02.prefab:7f321b26431a4974a82deefc368adf63");
  private static readonly AssetReference VO_TUTORIAL_01_HOGGER_10_10 = new AssetReference("VO_TUTORIAL_01_HOGGER_10_10.prefab:119535c251852324cb0794b4fd536627");

  public LettuceTutorialBossMissionEntity()
    : base()
  {
    this.m_gameOptions.AddOptions(LettuceTutorialBossMissionEntity.s_booleanOptions, LettuceTutorialBossMissionEntity.s_stringOptions);
    this.m_abilityOrderSpeechBubblesEnabled = true;
  }

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LettuceTutorialBossMissionEntity.VO_TUTORIAL_01_HOGGER_02_02,
      (string) LettuceTutorialBossMissionEntity.VO_TUTORIAL_01_HOGGER_10_10
    })
      this.PreloadSound(soundPath);
  }

  public override void OnDecommissionGame()
  {
    NotificationManager.Get().DestroyAllNotificationsNowWithNoAnim();
    base.OnDecommissionGame();
  }

  protected override void OnLettuceMissionEntityReconnect(int currentTurn)
  {
    if (currentTurn != 1 || this.GetTag<ACTION_STEP_TYPE>(GAME_TAG.ACTION_STEP_TYPE) != ACTION_STEP_TYPE.DEFAULT || this.IsAnyFriendlyAbilitySelected())
      return;
    this.SetTutorialStep(LettuceTutorialBossMissionEntity.TutorialStep.MOUSEOVER_INSPECT);
  }

  private void SetTutorialStep(LettuceTutorialBossMissionEntity.TutorialStep step)
  {
    this.m_currentTutorialStep = step;
    GameEntity.Coroutines.StartCoroutine(this.TransitionTutorialStepCoroutine(step));
  }

  private IEnumerator TransitionTutorialStepCoroutine(
    LettuceTutorialBossMissionEntity.TutorialStep step)
  {
    LettuceTutorialBossMissionEntity bossMissionEntity = this;
    if (!GameMgr.Get().IsSpectator())
    {
      switch (step)
      {
        case LettuceTutorialBossMissionEntity.TutorialStep.MOUSEOVER_INSPECT:
          LettuceTutorialBossMissionEntity.SetEndTurnEnableAndBlocker(false);
          bossMissionEntity.AddInputBlockerFriendlyAbilityZone();
          bossMissionEntity.AddClickBlockerForFriendlyMinions();
          yield return (object) new WaitForSeconds(0.5f);
          GameState.Get().SetBusy(true);
          GameState.Get().SetBusy(false);
          Card minionInEnemyPlay = bossMissionEntity.GetRightMostMinionInEnemyPlay();
          if ((Object) minionInEnemyPlay != (Object) null)
            bossMissionEntity.ShowClickChampionTutorial(minionInEnemyPlay, "GAMEPLAY_CHAMPION_MOUSEOVER_TUTORIAL");
          yield return (object) bossMissionEntity.WaitForEnemyMouseOver();
          break;
        case LettuceTutorialBossMissionEntity.TutorialStep.MOUSEOVER_INSPECT_DONE:
          LettuceTutorialBossMissionEntity.SetEndTurnEnableAndBlocker(true);
          bossMissionEntity.RemoveInputBlockerFriendlyAbilityZone();
          bossMissionEntity.RemoveClickBlockerForFriendlyMinions();
          bossMissionEntity.ShowAllMercenaryAbilityOrderBubbles();
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
      position2 = new Vector3(position1.x + 3.2f, position1.y, position1.z + 1f);
      direction = Notification.PopUpArrowDirection.Left;
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

  private Card GetRightMostMinionInEnemyPlay()
  {
    List<Card> cards = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards();
    foreach (Card minionInEnemyPlay in cards)
    {
      if (minionInEnemyPlay.GetEntity().GetTag(GAME_TAG.ZONE_POSITION) == cards.Count)
        return minionInEnemyPlay;
    }
    return (Card) null;
  }

  private IEnumerator WaitForEnemyMouseOver()
  {
    LettuceTutorialBossMissionEntity bossMissionEntity = this;
    while (!(bool) (Object) InputManager.Get().GetMousedOverCard() || (Object) InputManager.Get().GetMousedOverCard() != (Object) bossMissionEntity.GetRightMostMinionInEnemyPlay())
      yield return (object) null;
    bossMissionEntity.DestroyNotification(bossMissionEntity.m_clickChampionNotification);
    yield return (object) new WaitForSeconds(1f);
    while ((bool) (Object) InputManager.Get().GetMousedOverCard())
      yield return (object) null;
    bossMissionEntity.SetTutorialStep(LettuceTutorialBossMissionEntity.TutorialStep.MOUSEOVER_INSPECT_DONE);
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

  public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode) => !this.m_clickBlockedCards.Contains(clickedEntity.GetCard()) && (!clickedEntity.IsControlledByOpposingSidePlayer() || GameState.Get().IsInTargetMode());

  public override void NotifyOfStartOfTurnEventsFinished()
  {
    int tag1 = this.GetTag(GAME_TAG.TURN);
    int tag2 = GameState.Get().GetGameEntity().GetTag(GAME_TAG.ACTION_STEP_TYPE);
    if (tag1 != 1 || tag2 != 0)
      return;
    this.SetTutorialStep(LettuceTutorialBossMissionEntity.TutorialStep.MOUSEOVER_INSPECT);
  }

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

  private enum TutorialStep
  {
    Invalid,
    MOUSEOVER_INSPECT,
    MOUSEOVER_INSPECT_DONE,
    INTRO_TUTORIAL,
    INSPECT_REMINDER_VO,
  }
}
