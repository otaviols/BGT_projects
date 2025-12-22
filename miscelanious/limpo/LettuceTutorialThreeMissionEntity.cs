using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LettuceTutorialThreeMissionEntity : LettuceMissionEntity
{
  private static readonly Map<GameEntityOption, bool> s_booleanOptions = new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      false
    }
  };
  private static readonly Map<GameEntityOption, string> s_stringOptions = new Map<GameEntityOption, string>();
  protected List<GameObject> m_weaknessLabels = new List<GameObject>();
  private LettuceTutorialThreeMissionEntity.TutorialStep m_currentTutorialStep;
  private static readonly AssetReference VO_MurkEye_Male_Murloc_Bark_02 = new AssetReference("VO_MurkEye_Male_Murloc_Bark_02.prefab:1dab01cf2e464c13bca196c5933dce05");
  private Notification.SpeechBubbleDirection enemyMinionSpeakingDirection = Notification.SpeechBubbleDirection.BottomLeft;
  private Notification endTurnNotifier;

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LettuceTutorialThreeMissionEntity.VO_MurkEye_Male_Murloc_Bark_02
    })
      this.PreloadSound(soundPath);
  }

  public LettuceTutorialThreeMissionEntity()
    : base()
  {
    this.m_gameOptions.AddOptions(LettuceTutorialThreeMissionEntity.s_booleanOptions, LettuceTutorialThreeMissionEntity.s_stringOptions);
    this.m_abilityOrderSpeechBubblesEnabled = true;
    this.m_enemyAbilityOrderSpeechBubblesEnabled = false;
  }

  protected override void OnLettuceMissionEntityReconnect(int currentTurn)
  {
    if (currentTurn != 1 || this.IsAnyFriendlyAbilitySelected())
      return;
    this.SetTutorialStep(LettuceTutorialThreeMissionEntity.TutorialStep.WEAKNESS_TUTORIAL);
  }

  public override void OnDecommissionGame()
  {
    this.DestroyAllTutorialPopUps();
    base.OnDecommissionGame();
  }

  private void SetTutorialStep(
    LettuceTutorialThreeMissionEntity.TutorialStep step)
  {
    this.m_currentTutorialStep = step;
    GameEntity.Coroutines.StartCoroutine(this.TransitionTutorialStepCoroutine(step));
  }

  private IEnumerator TransitionTutorialStepCoroutine(
    LettuceTutorialThreeMissionEntity.TutorialStep step)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LettuceTutorialThreeMissionEntity threeMissionEntity = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (GameMgr.Get().IsSpectator())
      return false;
    switch (step)
    {
      case LettuceTutorialThreeMissionEntity.TutorialStep.WEAKNESS_TUTORIAL:
        threeMissionEntity.CreateTutorialDialog(LettuceTutorialResources.LettuceTutorialPopupBonusDamagePrefab, "GAMEPLAY_LETTUCE_WEAKNESS_TITLE_TUTORIAL", "GAMEPLAY_LETTUCE_WEAKNESS_BODY_TUTORIAL", "GAMEPLAY_LETTUCE_WEAKNESS_BUTTON_TUTORIAL", new UIEvent.Handler(threeMissionEntity.UserPressedWeaknessTutorial), Vector2.zero);
        break;
    }
    return false;
  }

  protected void UserPressedWeaknessTutorial(UIEvent e)
  {
    this.SetTutorialStep(LettuceTutorialThreeMissionEntity.TutorialStep.WEAKNESS_REMINDER_TUTORIAL);
    GameEntity.Coroutines.StartCoroutine(this.PlayVOOnUserPressedWeaknessTutorial());
  }

  protected IEnumerator PlayVOOnUserPressedWeaknessTutorial()
  {
    LettuceTutorialThreeMissionEntity threeMissionEntity = this;
    float seconds = 0.5f;
    string m_SpeakerActor = "LETL_026H_01";
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(seconds);
    GameState.Get().SetBusy(false);
    Actor playByDesignCode = threeMissionEntity.FindEnemyActorInPlayByDesignCode(m_SpeakerActor);
    yield return (object) threeMissionEntity.PlayLineAlways(playByDesignCode, (string) LettuceTutorialThreeMissionEntity.VO_MurkEye_Male_Murloc_Bark_02, threeMissionEntity.enemyMinionSpeakingDirection);
  }

  protected IEnumerator PlayLineAlways(
    Actor speaker,
    string line,
    Notification.SpeechBubbleDirection direction,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LettuceTutorialThreeMissionEntity threeMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(threeMissionEntity.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public override void NotifyOfStartOfTurnEventsFinished()
  {
    if (this.GetTag(GAME_TAG.TURN) != 1)
      return;
    this.SetTutorialStep(LettuceTutorialThreeMissionEntity.TutorialStep.WEAKNESS_TUTORIAL);
  }

  private void WeaknessLabelLoadedCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    GameObject healthTextObject = ((Card) callbackData).GetActor().GetHealthTextObject();
    if ((Object) healthTextObject == (Object) null)
    {
      Object.Destroy((Object) go);
    }
    else
    {
      Vector3 vector3_1;
      Vector3 vector3_2;
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        vector3_1 = new Vector3(-0.35f, -0.5f, 0.4f);
        vector3_2 = new Vector3(1.5f, 1.5f, 1.5f);
      }
      else
      {
        vector3_1 = new Vector3(-0.5f, -0.65f, 0.4f);
        vector3_2 = new Vector3(2.25f, 2.25f, 2.25f);
      }
      this.m_weaknessLabels.Add(go);
      go.transform.parent = healthTextObject.transform;
      go.transform.localScale = Vector3.zero;
      go.transform.localPosition = vector3_1;
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      iTween.ScaleTo(go, iTween.Hash((object) "scale", (object) vector3_2, (object) "time", (object) 2f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
      go.GetComponent<UberText>().Text = GameStrings.Get("GAMEPLAY_LETTUCE_WEAKNESS_LABEL");
    }
  }

  public override bool NotifyOfEndTurnButtonPushed()
  {
    this.DestroyAllTutorialPopUps();
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

  protected void DestroyAllTutorialPopUps()
  {
    NotificationManager.Get().DestroyAllArrows();
    NotificationManager.Get().DestroyAllPopUps();
    NotificationManager.Get().DestroyAllNotificationsNowWithNoAnim();
    foreach (Object weaknessLabel in this.m_weaknessLabels)
      Object.Destroy(weaknessLabel);
    this.m_weaknessLabels.Clear();
  }

  public override void OnAbilityTrayShown(Entity entity)
  {
    foreach (Object weaknessLabel in this.m_weaknessLabels)
      Object.Destroy(weaknessLabel);
    this.m_weaknessLabels.Clear();
    if (!entity.IsMinion() || !entity.IsControlledByFriendlySidePlayer())
      return;
    foreach (Card card in ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.OPPOSING).GetCards())
    {
      if (entity.IsMyLettuceRoleStrongAgainst(card.GetEntity()))
        AssetLoader.Get().InstantiatePrefab((AssetReference) "NumberLabel.prefab:597544d5ed24b994f95fe56e28584992", new PrefabCallback<GameObject>(this.WeaknessLabelLoadedCallback), (object) card, AssetLoadingOptions.IgnorePrefabPosition);
    }
  }

  public override void OnAbilityTrayDismissed() => this.DestroyAllTutorialPopUps();

  private enum TutorialStep
  {
    Invalid,
    WEAKNESS_TUTORIAL,
    WEAKNESS_REMINDER_TUTORIAL,
  }
}
