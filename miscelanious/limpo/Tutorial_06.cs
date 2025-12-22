using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_06 : TutorialEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = Tutorial_06.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = Tutorial_06.InitStringOptions();
  private Notification m_endTurnNotifier;
  private bool m_victory;
  private bool m_choSpeaking;
  private Spell m_choFloatSpell;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.KEYWORD_HELP_DELAY_OVERRIDDEN,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public Tutorial_06() => this.m_gameOptions.AddOptions(Tutorial_06.s_booleanOptions, Tutorial_06.s_stringOptions);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_TUTORIAL_06_CHO_15_15.prefab:5f0d0a2d3c6884a47aeadcf29b3d802d");
    this.PreloadSound("VO_TUTORIAL_06_CHO_09_13.prefab:99a983ceaa6615848a8bea922e428a2d");
    this.PreloadSound("VO_TUTORIAL_06_CHO_17_16.prefab:d337628cbe1422e4ca21dbe174ddef2e");
    this.PreloadSound("VO_TUTORIAL_06_CHO_05_09.prefab:ef06af76837b9ff4c8ac27ee18516291");
    this.PreloadSound("VO_TUTORIAL_06_JAINA_03_51.prefab:06bd40a237dd0674e8d377240de40e65");
    this.PreloadSound("VO_TUTORIAL_06_CHO_06_10.prefab:cd28a9685f46936409d5300001540558");
    this.PreloadSound("VO_TUTORIAL_06_CHO_21_18.prefab:48c935e7ec96a104ab04d185382898a4");
    this.PreloadSound("VO_TUTORIAL_06_CHO_20_17.prefab:dfc795a107caddb42b3d131d6a627fd8");
    this.PreloadSound("VO_TUTORIAL_06_CHO_07_11.prefab:b691c4acfee6c5342a727189de686b6d");
    this.PreloadSound("VO_TUTORIAL_06_JAINA_04_52.prefab:5d75f42502bc99b4c84704bedf553ba5");
    this.PreloadSound("VO_TUTORIAL_06_CHO_04_08.prefab:8164c968ccb1be44d9dfc01c1668b014");
    this.PreloadSound("VO_TUTORIAL_06_CHO_12_14.prefab:13ee98fef9d3e6746a69c385c889dc3a");
    this.PreloadSound("VO_TUTORIAL_06_CHO_01_05.prefab:10097a4886a24384d8e8f6dd668bb1c7");
    this.PreloadSound("VO_TUTORIAL_06_JAINA_01_49.prefab:b9513645100911741b9bda379bc27a75");
    this.PreloadSound("VO_TUTORIAL_06_CHO_02_06.prefab:a9c29883676f21d4e932dccc0f92feca");
    this.PreloadSound("VO_TUTORIAL_06_JAINA_02_50.prefab:b97fe840305cae04f8486ac1770b126f");
    this.PreloadSound("VO_TUTORIAL_06_CHO_03_07.prefab:c71aaff381cdbd346a9bcf54fa5d7db9");
    this.PreloadSound("VO_TUTORIAL_06_CHO_22_19.prefab:8c70f69b5da1f9c43accca95c1854ddf");
    this.PreloadSound("VO_TUTORIAL_06_JAINA_05_53.prefab:6fb71de610db1234887f6d6c948f5174");
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    this.CancelChoFloating();
    if (gameResult == TAG_PLAYSTATE.WON)
      this.m_victory = true;
    base.NotifyOfGameOver(gameResult);
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      GameUtils.SetTutorialProgress(TutorialProgress.CHO_COMPLETE);
      this.PlaySound("VO_TUTORIAL_06_CHO_22_19.prefab:8c70f69b5da1f9c43accca95c1854ddf");
    }
    else if (gameResult == TAG_PLAYSTATE.TIED)
    {
      this.PlaySound("VO_TUTORIAL_06_CHO_22_19.prefab:8c70f69b5da1f9c43accca95c1854ddf");
    }
    else
    {
      if (gameResult != TAG_PLAYSTATE.LOST)
        return;
      this.SetTutorialLostProgress(TutorialProgress.CHO_COMPLETE);
    }
  }

  protected override Spell BlowUpHero(Card card, SpellType spellType)
  {
    if (card.GetEntity().GetCardId() != "TU4f_001")
      return base.BlowUpHero(card, spellType);
    Spell spell = card.ActivateActorSpell(SpellType.CHODEATH);
    Gameplay.Get().StartCoroutine(this.HideOtherElements(card));
    return spell;
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    Tutorial_06 tutorial06 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 2:
        if (tutorial06.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_15_15.prefab:5f0d0a2d3c6884a47aeadcf29b3d802d", "TUTORIAL06_CHO_15", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 4:
        if (tutorial06.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_09_13.prefab:99a983ceaa6615848a8bea922e428a2d", "TUTORIAL06_CHO_09", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 14:
        if (tutorial06.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
          break;
        while (tutorial06.m_choSpeaking)
          yield return (object) null;
        tutorial06.m_choSpeaking = true;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_21_18.prefab:48c935e7ec96a104ab04d185382898a4", "TUTORIAL06_CHO_21", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        tutorial06.m_choSpeaking = false;
        break;
      case 15:
        if (tutorial06.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
          break;
        while (tutorial06.m_choSpeaking)
          yield return (object) null;
        yield return (object) new WaitForSeconds(0.5f);
        tutorial06.m_choSpeaking = true;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_05_09.prefab:ef06af76837b9ff4c8ac27ee18516291", "TUTORIAL06_CHO_05", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        tutorial06.m_choSpeaking = false;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_03_51.prefab:06bd40a237dd0674e8d377240de40e65", "TUTORIAL06_JAINA_03", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        tutorial06.m_choSpeaking = true;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_06_10.prefab:cd28a9685f46936409d5300001540558", "TUTORIAL06_CHO_06", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        tutorial06.m_choSpeaking = false;
        break;
      case 16:
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_20_17.prefab:dfc795a107caddb42b3d131d6a627fd8", "TUTORIAL06_CHO_20", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    Tutorial_06 tutorial06 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        tutorial06.HandleGameStartEvent();
        break;
      case 2:
        GameState.Get().SetBusy(true);
        while (tutorial06.m_choSpeaking)
          yield return (object) null;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_17_16.prefab:d337628cbe1422e4ca21dbe174ddef2e", "TUTORIAL06_CHO_17", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 6:
        GameState.Get().SetBusy(true);
        Card card = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard();
        tutorial06.m_choFloatSpell = card.GetActorSpell(SpellType.CHOFLOAT);
        tutorial06.m_choFloatSpell.ActivateState(SpellStateType.BIRTH);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_07_11.prefab:b691c4acfee6c5342a727189de686b6d", "TUTORIAL06_CHO_07", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_04_52.prefab:5d75f42502bc99b4c84704bedf553ba5", "TUTORIAL06_JAINA_04", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        break;
      case 8:
        Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_04_08.prefab:8164c968ccb1be44d9dfc01c1668b014", "TUTORIAL06_CHO_04", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 9:
        tutorial06.CancelChoFloating();
        tutorial06.m_choSpeaking = true;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_12_14.prefab:13ee98fef9d3e6746a69c385c889dc3a", "TUTORIAL06_CHO_12", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        tutorial06.m_choSpeaking = false;
        break;
      case 10:
        Card doctorInOpposingSide = tutorial06.FindVoodooDoctorInOpposingSide();
        if ((Object) doctorInOpposingSide == (Object) null)
          break;
        GameState.Get().SetBusy(true);
        Vector3 position = doctorInOpposingSide.transform.position;
        Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, new Vector3(position.x + 3f, position.y, position.z), TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL06_HELP_03"));
        popupText.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        NotificationManager.Get().DestroyNotification(popupText, 5f);
        yield return (object) new WaitForSeconds(5f);
        GameState.Get().SetBusy(false);
        break;
      case 54:
        yield return (object) new WaitForSeconds(2f);
        string bodyTextGameString = !tutorial06.DidLoseTutorial(TutorialProgress.CHO_COMPLETE) ? "TUTORIAL06_HELP_02" : "TUTORIAL06_HELP_04";
        tutorial06.m_preTutorialNotification = tutorial06.ShowTutorialDialog("TUTORIAL06_HELP_01", bodyTextGameString, "TUTORIAL01_HELP_16", new Vector2(0.0f, 0.5f));
        break;
      case 55:
        MulliganManager.Get().BeginMulligan();
        tutorial06.FadeInHeroActor(enemyActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_01_05.prefab:10097a4886a24384d8e8f6dd668bb1c7", "TUTORIAL06_CHO_01", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        tutorial06.FadeOutHeroActor(enemyActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.Wait(0.5f));
        tutorial06.FadeInHeroActor(jainaActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_01_49.prefab:b9513645100911741b9bda379bc27a75", "TUTORIAL06_JAINA_01", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        tutorial06.FadeOutHeroActor(jainaActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.Wait(0.5f));
        tutorial06.FadeInHeroActor(enemyActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_02_06.prefab:a9c29883676f21d4e932dccc0f92feca", "TUTORIAL06_CHO_02", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        tutorial06.FadeOutHeroActor(enemyActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.Wait(0.25f));
        tutorial06.FadeInHeroActor(jainaActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_02_50.prefab:b97fe840305cae04f8486ac1770b126f", "TUTORIAL06_JAINA_02", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        tutorial06.FadeOutHeroActor(jainaActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial06.Wait(0.25f));
        Gameplay.Get().StartCoroutine(tutorial06.PlaySoundAndWait("VO_TUTORIAL_06_CHO_03_07.prefab:c71aaff381cdbd346a9bcf54fa5d7db9", "TUTORIAL06_CHO_03", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
    }
  }

  private void CancelChoFloating()
  {
    if ((Object) this.m_choFloatSpell == (Object) null || this.m_choFloatSpell.GetActiveState() == SpellStateType.NONE)
      return;
    this.m_choFloatSpell.ActivateState(SpellStateType.CANCEL);
  }

  private Card FindVoodooDoctorInOpposingSide()
  {
    foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
    {
      if (card.GetEntity().GetCardId() == "EX1_011")
        return card;
    }
    return (Card) null;
  }

  private IEnumerator Wait(float seconds)
  {
    yield return (object) new WaitForSeconds(seconds);
  }

  public override float GetAdditionalTimeToWaitForSpells() => 1.5f;

  public override bool NotifyOfEndTurnButtonPushed()
  {
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket != null && !optionsPacket.HasValidOption())
    {
      NotificationManager.Get().DestroyAllArrows();
      return true;
    }
    for (int index = 0; index < optionsPacket.List.Count; ++index)
    {
      Network.Options.Option option = optionsPacket.List[index];
      if (option.Main.PlayErrorInfo.IsValid() && option.Type == Network.Options.Option.OptionType.POWER && GameState.Get().GetEntity(option.Main.ID).GetZone() == TAG_ZONE.PLAY)
      {
        if ((Object) this.m_endTurnNotifier != (Object) null)
          NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_endTurnNotifier);
        Vector3 position1 = EndTurnButton.Get().transform.position;
        Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
        this.m_endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL_NO_ENDTURN_ATK"));
        NotificationManager.Get().DestroyNotification(this.m_endTurnNotifier, 2.5f);
        return false;
      }
    }
    return true;
  }

  public override void NotifyOfDefeatCoinAnimation()
  {
    if (!this.m_victory)
      return;
    this.PlaySound("VO_TUTORIAL_06_JAINA_05_53.prefab:6fb71de610db1234887f6d6c948f5174");
  }

  public override List<RewardData> GetCustomRewards()
  {
    if (!this.m_victory)
      return (List<RewardData>) null;
    List<RewardData> customRewards = new List<RewardData>();
    CardRewardData cardRewardData = new CardRewardData("CS2_124", TAG_PREMIUM.NORMAL, 2);
    cardRewardData.MarkAsDummyReward();
    customRewards.Add((RewardData) cardRewardData);
    return customRewards;
  }
}
