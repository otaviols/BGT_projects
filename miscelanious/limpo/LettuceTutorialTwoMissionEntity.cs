using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LettuceTutorialTwoMissionEntity : LettuceMissionEntity
{
  private static readonly Map<GameEntityOption, bool> s_booleanOptions = new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      false
    }
  };
  private static readonly Map<GameEntityOption, string> s_stringOptions = new Map<GameEntityOption, string>();
  private Notification endTurnNotifier;
  private LettuceTutorialTwoMissionEntity.TutorialStep m_currentTutorialStep;
  private static readonly AssetReference VO_DRG_082_Male_Kobold_Attack_02 = new AssetReference("VO_DRG_082_Male_Kobold_Attack_02.prefab:1941508e37e04de4fb8ae327e9e155a5");
  private static readonly AssetReference VO_DAL_416_Female_Kobold_Play_01 = new AssetReference("VO_DAL_416_Female_Kobold_Play_01.prefab:d3b15a1c1362c734da6573caa0976203");
  private Notification.SpeechBubbleDirection enemyMinionSpeakingDirection = Notification.SpeechBubbleDirection.BottomLeft;
  private TooltipPanel speedHelpPanel;
  private PlatformDependentValue<Vector3> m_speedTooltipPositionLeftmostAbility = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.35f, 0.0f, -1.8f),
    Phone = new Vector3(1.5f, 0.0f, -2.2f)
  };
  private PlatformDependentValue<Vector3> m_speedTooltipPositionMiddleAbility = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(-1.55f, 0.0f, -1.8f),
    Phone = new Vector3(-0.35f, 0.0f, -2.2f)
  };
  private PlatformDependentValue<float> m_gemScale = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 1.75f,
    Phone = 1.5f
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LettuceTutorialTwoMissionEntity.VO_DRG_082_Male_Kobold_Attack_02,
      (string) LettuceTutorialTwoMissionEntity.VO_DAL_416_Female_Kobold_Play_01
    })
      this.PreloadSound(soundPath);
  }

  public LettuceTutorialTwoMissionEntity()
    : base()
  {
    this.m_gameOptions.AddOptions(LettuceTutorialTwoMissionEntity.s_booleanOptions, LettuceTutorialTwoMissionEntity.s_stringOptions);
    this.m_abilityOrderSpeechBubblesEnabled = true;
    this.m_enemyAbilityOrderSpeechBubblesEnabled = false;
  }

  protected override void OnLettuceMissionEntityReconnect(int currentTurn)
  {
    if (currentTurn != 1 || this.IsAnyFriendlyAbilitySelected())
      return;
    this.SetTutorialStep(LettuceTutorialTwoMissionEntity.TutorialStep.SPEED_TUTORIAL);
  }

  private void SetTutorialStep(LettuceTutorialTwoMissionEntity.TutorialStep step)
  {
    this.m_currentTutorialStep = step;
    GameEntity.Coroutines.StartCoroutine(this.TransitionTutorialStepCoroutine(step));
  }

  private IEnumerator TransitionTutorialStepCoroutine(
    LettuceTutorialTwoMissionEntity.TutorialStep step)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LettuceTutorialTwoMissionEntity twoMissionEntity = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (GameMgr.Get().IsSpectator() || step != LettuceTutorialTwoMissionEntity.TutorialStep.SPEED_TUTORIAL)
      return false;
    GameState.Get().SetBusy(true);
    twoMissionEntity.CreateTutorialDialog(LettuceTutorialResources.LettuceTutorialPopupSpeedPrefab, "GAMEPLAY_LETTUCE_SPEED_TITLE_TUTORIAL", "GAMEPLAY_LETTUCE_SPEED_BODY_TUTORIAL", "GAMEPLAY_LETTUCE_SPEED_BUTTON_TUTORIAL", new UIEvent.Handler(twoMissionEntity.UserPressedSpeedTutorial), Vector2.zero);
    return false;
  }

  private void UserPressedSpeedTutorial(UIEvent e)
  {
    GameState.Get().SetBusy(false);
    GameEntity.Coroutines.StartCoroutine(this.PlayVOOnUserPressedSpeedTutorialSpeed());
  }

  protected IEnumerator PlayVOOnUserPressedSpeedTutorialSpeed()
  {
    LettuceTutorialTwoMissionEntity twoMissionEntity = this;
    float seconds = 0.5f;
    string m_SpeakerActor = "LETL_810_01";
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(seconds);
    GameState.Get().SetBusy(false);
    Actor playByDesignCode = twoMissionEntity.FindEnemyActorInPlayByDesignCode(m_SpeakerActor);
    yield return (object) twoMissionEntity.PlayLineAlways(playByDesignCode, (string) LettuceTutorialTwoMissionEntity.VO_DAL_416_Female_Kobold_Play_01, twoMissionEntity.enemyMinionSpeakingDirection);
  }

  protected IEnumerator PlayLineAlways(
    Actor speaker,
    string line,
    Notification.SpeechBubbleDirection direction,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LettuceTutorialTwoMissionEntity twoMissionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(twoMissionEntity.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public override void NotifyOfStartOfTurnEventsFinished()
  {
    if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.TURN) != 1)
      return;
    this.SetTutorialStep(LettuceTutorialTwoMissionEntity.TutorialStep.SPEED_TUTORIAL);
  }

  public override bool NotifyOfCardTooltipDisplayShow(Card card)
  {
    if (GameState.Get().IsGameOver())
      return false;
    if (card.GetEntity().IsLettuceAbility() && (Object) this.speedHelpPanel == (Object) null)
      this.ShowSpeedTooltip(card);
    return true;
  }

  private void ShowSpeedTooltip(Card card)
  {
    LayerUtils.SetLayer(card.GetActor().GetCostTextObject().gameObject, GameLayer.Tooltip);
    Actor actor = card.GetActor();
    Vector3 position = actor.transform.position;
    Vector3 speedTooltipOffset = this.GetSpeedTooltipOffset(card);
    Vector3 vector3 = new Vector3(position.x + speedTooltipOffset.x, position.y + speedTooltipOffset.y, position.z + speedTooltipOffset.z);
    this.speedHelpPanel = TooltipPanelManager.Get().CreateKeywordPanel(0);
    this.speedHelpPanel.Reset();
    this.speedHelpPanel.Initialize(GameStrings.Get("GAMEPLAY_LETTUCE_SPEED_LABEL_TUTORIAL"), GameStrings.Get("GAMEPLAY_LETTUCE_SPEED_TOOLTIP_TUTORIAL"));
    this.speedHelpPanel.SetScale((float) TooltipPanel.GAMEPLAY_SCALE);
    this.speedHelpPanel.transform.position = vector3;
    RenderUtils.SetAlpha(this.speedHelpPanel.gameObject, 0.0f);
    iTween.FadeTo(this.speedHelpPanel.gameObject, iTween.Hash((object) "alpha", (object) 1, (object) "time", (object) 0.25f));
    this.EnlargeGameObject(actor.GetCostTextObject(), (float) this.m_gemScale);
    MercenaryRoleGemObject componentInChildren = actor.gameObject.GetComponentInChildren<MercenaryRoleGemObject>();
    if (!((Object) componentInChildren != (Object) null))
      return;
    this.EnlargeGameObject(componentInChildren.gameObject, (float) this.m_gemScale);
  }

  private Vector3 GetSpeedTooltipOffset(Card card)
  {
    if ((Object) card == (Object) null)
      return Vector3.zero;
    switch (ZoneMgr.Get().GetLettuceZoneController().GetAbilityTray().GetTrayPositionOfAbility(card))
    {
      case 0:
        return (Vector3) this.m_speedTooltipPositionLeftmostAbility;
      case 1:
        return (Vector3) this.m_speedTooltipPositionMiddleAbility;
      default:
        return Vector3.zero;
    }
  }

  public override void NotifyOfCardTooltipDisplayHide(Card card)
  {
    if (!((Object) this.speedHelpPanel != (Object) null))
      return;
    if ((Object) card != (Object) null)
    {
      Actor actor = card.GetActor();
      GameObject costTextObject = actor.GetCostTextObject();
      LayerUtils.SetLayer(costTextObject, GameLayer.Default);
      this.ShrinkGameObject(costTextObject);
      MercenaryRoleGemObject componentInChildren = actor.gameObject.GetComponentInChildren<MercenaryRoleGemObject>();
      if ((Object) componentInChildren != (Object) null)
        this.ShrinkGameObject(componentInChildren.gameObject);
    }
    Object.Destroy((Object) this.speedHelpPanel.gameObject);
  }

  public override List<TooltipPanelManager.TooltipPanelData> GetOverwriteKeywordHelpPanelDisplay(
    Entity ent)
  {
    if (ent == null)
      return (List<TooltipPanelManager.TooltipPanelData>) null;
    return ent.IsLettuceAbility() ? new List<TooltipPanelManager.TooltipPanelData>() : base.GetOverwriteKeywordHelpPanelDisplay(ent);
  }

  private void EnlargeGameObject(GameObject gameObject, float scaleFactor)
  {
    iTween.Stop(gameObject);
    iTween.ScaleTo(gameObject, iTween.Hash((object) "scale", (object) new Vector3(scaleFactor, scaleFactor, scaleFactor), (object) "time", (object) 2f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
  }

  private void ShrinkGameObject(GameObject gameObject) => iTween.ScaleTo(gameObject, new Vector3(1f, 1f, 1f), 0.5f);

  public override bool NotifyOfEndTurnButtonPushed()
  {
    if (this.GetTag(GAME_TAG.TURN) < 1)
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

  private enum TutorialStep
  {
    Invalid,
    SPEED_TUTORIAL,
  }
}
