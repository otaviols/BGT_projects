using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_05 : TutorialEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = Tutorial_05.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = Tutorial_05.InitStringOptions();
  private int weaponsPlayed;
  private int numTimesRemindedAboutGoal;
  private bool heroPowerHasNotBeenUsed = true;
  private bool victory;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.KEYWORD_HELP_DELAY_OVERRIDDEN,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public Tutorial_05() => this.m_gameOptions.AddOptions(Tutorial_05.s_booleanOptions, Tutorial_05.s_stringOptions);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_12_12.prefab:dacd7715ffe4d38458679bd5cac593d1");
    this.PreloadSound("VO_TUTORIAL_04_JAINA_03_39.prefab:ef84060011610064abeee5d2d526bf85");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_11_11.prefab:8cd68956e13f8ee43bb816a92c56ab7e");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_02_03.prefab:00cdf773e524ae548a31d82db5bb35c2");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_04_05.prefab:eb68b53ffa7195841a18d4c50516ce35");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_08_08.prefab:32281bee676aa6d4e9c590dfb9e03cb6");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_03_04.prefab:38739c8e8bb7eba42a94afe8bce981f3");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_05_06.prefab:30bf89624d8c3df4b9f776218c7300ad");
    this.PreloadSound("VO_TUTORIAL_05_JAINA_02_46.prefab:4daa9f9fc9fc730429c198b9a7212521");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_06_07.prefab:f8e57e165a11de047a2fcaa95e22457b");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_09_09.prefab:1ca9806eebcb0e841be971e486199833");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_10_10.prefab:30c0266100fcd714e804006040c241ad");
    this.PreloadSound("VO_TUTORIAL_05_JAINA_05_47.prefab:8caf0051fc3c91c48852eed53e886e4b");
    this.PreloadSound("VO_TUTORIAL_05_JAINA_06_48.prefab:fbfba7282a8cb334ba40699cab0524fd");
    this.PreloadSound("VO_TUTORIAL_05_ILLIDAN_01_02.prefab:d4b65ea6366e7a64d8833321001590f1");
    this.PreloadSound("VO_TUTORIAL_05_JAINA_01_45.prefab:d5b645ea8a95c0e44a90838ab77b2564");
    this.PreloadSound("VO_INNKEEPER_TUT_COMPLETE_05.prefab:c8d19a552e18c7c429946f62102c9460");
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON)
      this.victory = true;
    base.NotifyOfGameOver(gameResult);
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      GameUtils.SetTutorialProgress(TutorialProgress.ILLIDAN_COMPLETE);
      if (Network.ShouldBeConnectedToAurora() && Network.IsLoggedIn() && !GameMgr.Get().IsSpectator())
        BnetPresenceMgr.Get().SetGameField(15U, 1);
      this.ResetTutorialLostProgress();
      this.PlaySound("VO_TUTORIAL_05_ILLIDAN_12_12.prefab:dacd7715ffe4d38458679bd5cac593d1");
    }
    else if (gameResult == TAG_PLAYSTATE.TIED)
    {
      this.PlaySound("VO_TUTORIAL_05_ILLIDAN_12_12.prefab:dacd7715ffe4d38458679bd5cac593d1");
    }
    else
    {
      if (gameResult != TAG_PLAYSTATE.LOST)
        return;
      this.SetTutorialLostProgress(TutorialProgress.ILLIDAN_COMPLETE);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    Tutorial_05 tutorial05 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (GameState.Get().GetOpposingSidePlayer().HasWeapon())
      GameState.Get().GetOpposingSidePlayer().GetWeaponCard().GetActorSpell(SpellType.DEATH).m_BlockServerEvents = true;
    if (turn == 2)
    {
      yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_03_39.prefab:ef84060011610064abeee5d2d526bf85", "TUTORIAL04_JAINA_03", Notification.SpeechBubbleDirection.BottomLeft, actor));
      if (!tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
      {
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_11_11.prefab:8cd68956e13f8ee43bb816a92c56ab7e", "TUTORIAL05_ILLIDAN_11", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
      }
      if (tutorial05.GetTag(GAME_TAG.TURN) == 2 && EndTurnButton.Get().IsInNMPState())
        tutorial05.ShowEndTurnBouncingArrow();
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    Tutorial_05 tutorial05 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        tutorial05.HandleGameStartEvent();
        break;
      case 2:
        if (tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        ++tutorial05.weaponsPlayed;
        if (tutorial05.weaponsPlayed == 1)
          yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_02_03.prefab:00cdf773e524ae548a31d82db5bb35c2", "TUTORIAL05_ILLIDAN_02", Notification.SpeechBubbleDirection.TopLeft, enemyActor));
        else if (tutorial05.weaponsPlayed == 2)
          yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_04_05.prefab:eb68b53ffa7195841a18d4c50516ce35", "TUTORIAL05_ILLIDAN_04", Notification.SpeechBubbleDirection.TopLeft, enemyActor));
        else
          yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_08_08.prefab:32281bee676aa6d4e9c590dfb9e03cb6", "TUTORIAL05_ILLIDAN_08", Notification.SpeechBubbleDirection.TopLeft, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 3:
        if (tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
          break;
        Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_03_04.prefab:38739c8e8bb7eba42a94afe8bce981f3", "TUTORIAL05_ILLIDAN_03", Notification.SpeechBubbleDirection.TopLeft, enemyActor));
        break;
      case 4:
        if (tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_05_06.prefab:30bf89624d8c3df4b9f776218c7300ad", "TUTORIAL05_ILLIDAN_05", Notification.SpeechBubbleDirection.TopLeft, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 5:
        if (!tutorial05.heroPowerHasNotBeenUsed)
          break;
        tutorial05.heroPowerHasNotBeenUsed = false;
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_02_46.prefab:4daa9f9fc9fc730429c198b9a7212521", "TUTORIAL05_JAINA_02", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_06_07.prefab:f8e57e165a11de047a2fcaa95e22457b", "TUTORIAL05_ILLIDAN_06", Notification.SpeechBubbleDirection.TopLeft, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 8:
        if (tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
          break;
        Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_09_09.prefab:1ca9806eebcb0e841be971e486199833", "TUTORIAL05_ILLIDAN_09", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 9:
        if (tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
          break;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_10_10.prefab:30c0266100fcd714e804006040c241ad", "TUTORIAL05_ILLIDAN_10", Notification.SpeechBubbleDirection.TopLeft, enemyActor));
        break;
      case 10:
        if (tutorial05.numTimesRemindedAboutGoal == 0)
          yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_05_47.prefab:8caf0051fc3c91c48852eed53e886e4b", "TUTORIAL05_JAINA_05", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        else if (tutorial05.numTimesRemindedAboutGoal == 1)
          yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_06_48.prefab:fbfba7282a8cb334ba40699cab0524fd", "TUTORIAL05_JAINA_06", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        ++tutorial05.numTimesRemindedAboutGoal;
        break;
      case 12:
        GameState.Get().SetBusy(true);
        Vector3 position1 = GameState.Get().GetOpposingSidePlayer().GetHeroCard().transform.position;
        Vector3 position2 = new Vector3(position1.x - 1.55f, position1.y, position1.z - 2.721f);
        Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL05_HELP_01"));
        popupText.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
        NotificationManager.Get().DestroyNotification(popupText, 5f);
        yield return (object) new WaitForSeconds(5.5f);
        GameState.Get().SetBusy(false);
        break;
      case 54:
        yield return (object) new WaitForSeconds(2f);
        string bodyTextGameString = !tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE) ? "TUTORIAL05_HELP_03" : "TUTORIAL05_HELP_04";
        tutorial05.m_preTutorialNotification = tutorial05.ShowTutorialDialog("TUTORIAL05_HELP_02", bodyTextGameString, "TUTORIAL01_HELP_16", new Vector2(0.5f, 0.0f), true);
        break;
      case 55:
        if (!tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
        {
          tutorial05.FadeInHeroActor(enemyActor);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_01_02.prefab:d4b65ea6366e7a64d8833321001590f1", "TUTORIAL05_ILLIDAN_01", Notification.SpeechBubbleDirection.TopRight, enemyActor));
          tutorial05.FadeOutHeroActor(enemyActor);
          yield return (object) new WaitForSeconds(0.5f);
          tutorial05.FadeInHeroActor(jainaActor);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial05.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_01_45.prefab:d5b645ea8a95c0e44a90838ab77b2564", "TUTORIAL05_JAINA_01", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        }
        MulliganManager.Get().BeginMulligan();
        if (tutorial05.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
          break;
        yield return (object) new WaitForSeconds(2.3f);
        tutorial05.FadeOutHeroActor(jainaActor);
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

  public override bool NotifyOfEndTurnButtonPushed()
  {
    NotificationManager.Get().DestroyAllArrows();
    return true;
  }

  public override bool NotifyOfTooltipDisplay(TooltipZone specificZone) => false;

  public override string[] NotifyOfKeywordHelpPanelDisplay(Entity entity)
  {
    if (!(entity.GetCardId() == "TU4e_004") && !(entity.GetCardId() == "TU4e_007"))
      return (string[]) null;
    return new string[2]
    {
      GameStrings.Get("TUTORIAL05_WEAPON_HEADLINE"),
      GameStrings.Get("TUTORIAL05_WEAPON_DESC")
    };
  }

  public override List<RewardData> GetCustomRewards()
  {
    if (!this.victory)
      return (List<RewardData>) null;
    List<RewardData> customRewards = new List<RewardData>();
    CardRewardData cardRewardData = new CardRewardData("EX1_277", TAG_PREMIUM.NORMAL, 2);
    cardRewardData.MarkAsDummyReward();
    customRewards.Add((RewardData) cardRewardData);
    return customRewards;
  }
}
