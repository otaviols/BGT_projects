using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_03 : TutorialEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = Tutorial_03.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = Tutorial_03.InitStringOptions();
  private int numTauntGorillasPlayed;
  private bool enemyPlayedBigBrother;
  private bool needATaunterVOPlayed;
  private bool monkeyLinePlayedOnce;
  private bool defenselessVoPlayed;
  private bool seenTheBrother;
  private bool warnedAgainstAttackingGorilla;
  private bool victory;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.MOUSEOVER_DELAY_OVERRIDDEN,
      false
    },
    {
      GameEntityOption.KEYWORD_HELP_DELAY_OVERRIDDEN,
      true
    },
    {
      GameEntityOption.SHOW_CRAZY_KEYWORD_TOOLTIP,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public Tutorial_03() => this.m_gameOptions.AddOptions(Tutorial_03.s_booleanOptions, Tutorial_03.s_stringOptions);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_TUTORIAL_03_JAINA_17_33.prefab:b96f78fff7ab94a42894930d51bd45bd");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_18_34.prefab:b9a2a99d30893804790829b3ceabc9b8");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_01_24.prefab:b9515cf173f876a458202c6092055709");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_05_25.prefab:38e2d64610e757245877b8f8e2f68584");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_07_26.prefab:e93d67263c3d99740aaa4acc4b7d87a4");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_12_28.prefab:d30f0c732643aa74aba9ec4cf2c2e6dd");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_13_29.prefab:efca9c5305a101e4d968d08e58061cda");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_16_32.prefab:b05bea699e2f897478c81a485a7d1a1a");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_14_30.prefab:0787881bd0a25a342ba06f566f16051b");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_15_31.prefab:4e0f1eaa19e283a4cac77219e1f10fe3");
    this.PreloadSound("VO_TUTORIAL_03_JAINA_20_36.prefab:79671f155307aa24a89b0581e4c5c4b2");
    this.PreloadSound("VO_TUTORIAL_03_MUKLA_01_01.prefab:3f6638f7f0d96da4ca422a290035c97a");
    this.PreloadSound("VO_TUTORIAL_03_MUKLA_03_03.prefab:5018131495f68c247bac073424fab700");
    this.PreloadSound("VO_TUTORIAL_03_MUKLA_04_04.prefab:0e4a4c87ac994c845b06230a34b168f9");
    this.PreloadSound("VO_TUTORIAL_03_MUKLA_05_05.prefab:8c12c75976cdfe044ad8ff3dd14ae5b8");
    this.PreloadSound("VO_TUTORIAL_03_MUKLA_06_06.prefab:8ed0c9ff5d18314469821d5be3d62dc7");
    this.PreloadSound("VO_TUTORIAL_03_MUKLA_07_07.prefab:c7b7dc3589c10c94bb3b9c0c6c08e3f6");
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON)
      this.victory = true;
    base.NotifyOfGameOver(gameResult);
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      GameUtils.SetTutorialProgress(TutorialProgress.MUKLA_COMPLETE);
      this.PlaySound("VO_TUTORIAL_03_MUKLA_07_07.prefab:c7b7dc3589c10c94bb3b9c0c6c08e3f6");
    }
    else if (gameResult == TAG_PLAYSTATE.TIED)
    {
      this.PlaySound("VO_TUTORIAL_03_MUKLA_07_07.prefab:c7b7dc3589c10c94bb3b9c0c6c08e3f6");
    }
    else
    {
      if (gameResult != TAG_PLAYSTATE.LOST)
        return;
      this.SetTutorialLostProgress(TutorialProgress.MUKLA_COMPLETE);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    Tutorial_03 tutorial03 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (tutorial03.enemyPlayedBigBrother)
    {
      if (GameState.Get().IsFriendlySidePlayerTurn())
      {
        if (GameState.Get().GetNumEnemyMinionsInPlay(false) > 0)
        {
          if (!tutorial03.needATaunterVOPlayed)
          {
            if (GameState.Get().GetFriendlySidePlayer().HasATauntMinion())
            {
              yield break;
            }
            else
            {
              tutorial03.needATaunterVOPlayed = true;
              Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_17_33.prefab:b96f78fff7ab94a42894930d51bd45bd", "TUTORIAL03_JAINA_17", Notification.SpeechBubbleDirection.BottomLeft, actor));
              yield break;
            }
          }
          else if (!tutorial03.defenselessVoPlayed)
          {
            bool flag = true;
            foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards())
            {
              if (card.GetEntity().HasTaunt())
                flag = false;
            }
            if (flag)
            {
              tutorial03.defenselessVoPlayed = true;
              Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_18_34.prefab:b9a2a99d30893804790829b3ceabc9b8", "TUTORIAL03_JAINA_18", Notification.SpeechBubbleDirection.BottomLeft, actor));
            }
          }
        }
      }
      else if (!tutorial03.seenTheBrother)
        Gameplay.Get().StartCoroutine(tutorial03.GetReadyForBro());
    }
    switch (turn)
    {
      case 1:
        if (tutorial03.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
          break;
        Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_01_24.prefab:b9515cf173f876a458202c6092055709", "TUTORIAL03_JAINA_01", Notification.SpeechBubbleDirection.BottomLeft, actor));
        break;
      case 5:
        if (tutorial03.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
          break;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_05_25.prefab:38e2d64610e757245877b8f8e2f68584", "TUTORIAL03_JAINA_05", Notification.SpeechBubbleDirection.BottomLeft, actor));
        Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_03_03.prefab:5018131495f68c247bac073424fab700", "TUTORIAL03_MUKLA_03", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 6:
        if (tutorial03.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_04_04.prefab:0e4a4c87ac994c845b06230a34b168f9", "TUTORIAL03_MUKLA_04", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 9:
        Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_07_26.prefab:e93d67263c3d99740aaa4acc4b7d87a4", "TUTORIAL03_JAINA_07", Notification.SpeechBubbleDirection.BottomLeft, actor));
        break;
      case 14:
        Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_05_05.prefab:8c12c75976cdfe044ad8ff3dd14ae5b8", "TUTORIAL03_MUKLA_05", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
    }
  }

  private IEnumerator GetReadyForBro()
  {
    Tutorial_03 tutorial03 = this;
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    tutorial03.seenTheBrother = true;
    GameState.Get().SetBusy(true);
    yield return (object) Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_12_28.prefab:d30f0c732643aa74aba9ec4cf2c2e6dd", "TUTORIAL03_JAINA_12", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
    GameState.Get().SetBusy(false);
    yield return (object) new WaitForSeconds(3.2f);
    Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_13_29.prefab:efca9c5305a101e4d968d08e58061cda", "TUTORIAL03_JAINA_13", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    Tutorial_03 tutorial03 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        tutorial03.HandleGameStartEvent();
        AssetLoader.Get().InstantiatePrefab((AssetReference) "TutorialKeywordManager.prefab:c1276fda3e1df594990295731f80c9c2", AssetLoadingOptions.IgnorePrefabPosition);
        break;
      case 4:
        ++tutorial03.numTauntGorillasPlayed;
        if (tutorial03.numTauntGorillasPlayed == 1)
        {
          Gameplay.Get().StartCoroutine(tutorial03.ShowTauntPopup());
          break;
        }
        if (tutorial03.numTauntGorillasPlayed != 2 || tutorial03.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_06_06.prefab:8ed0c9ff5d18314469821d5be3d62dc7", "TUTORIAL03_MUKLA_06", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 10:
        tutorial03.enemyPlayedBigBrother = true;
        Gameplay.Get().StartCoroutine(tutorial03.AdjustBigBrotherTransform());
        if (GameState.Get().IsFriendlySidePlayerTurn())
          break;
        Gameplay.Get().StartCoroutine(tutorial03.GetReadyForBro());
        break;
      case 11:
        Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_16_32.prefab:b05bea699e2f897478c81a485a7d1a1a", "TUTORIAL03_JAINA_16", Notification.SpeechBubbleDirection.BottomLeft, actor));
        break;
      case 12:
        if (!tutorial03.monkeyLinePlayedOnce)
        {
          tutorial03.monkeyLinePlayedOnce = true;
          Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_14_30.prefab:0787881bd0a25a342ba06f566f16051b", "TUTORIAL03_JAINA_14", Notification.SpeechBubbleDirection.BottomLeft, actor));
          break;
        }
        if (tutorial03.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
          break;
        Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_15_31.prefab:4e0f1eaa19e283a4cac77219e1f10fe3", "TUTORIAL03_JAINA_15", Notification.SpeechBubbleDirection.BottomLeft, actor));
        break;
      case 54:
        yield return (object) new WaitForSeconds(2f);
        string bodyTextGameString = "TUTORIAL03_HELP_03";
        if (UniversalInputManager.Get().IsTouchMode())
          bodyTextGameString = "TUTORIAL03_HELP_03_TOUCH";
        tutorial03.m_preTutorialNotification = tutorial03.ShowTutorialDialog("TUTORIAL03_HELP_02", bodyTextGameString, "TUTORIAL01_HELP_16", new Vector2(0.5f, 0.5f));
        break;
      case 55:
        tutorial03.FadeInHeroActor(enemyActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial03.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_01_01.prefab:3f6638f7f0d96da4ca422a290035c97a", "TUTORIAL03_MUKLA_01", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        MulliganManager.Get().BeginMulligan();
        tutorial03.FadeOutHeroActor(enemyActor);
        break;
    }
  }

  private IEnumerator ShowTauntPopup()
  {
    Card gorillaCard = (Card) null;
    while ((Object) gorillaCard == (Object) null)
    {
      gorillaCard = this.FindCardInEnemyBattlefield("TU5_CS2_127");
      if (!((Object) gorillaCard != (Object) null))
        yield return (object) null;
      else
        break;
    }
    while (!gorillaCard.IsActorReady())
      yield return (object) null;
    Vector3 position = gorillaCard.transform.position - new Vector3(3f, 0.0f, 0.0f);
    Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL03_HELP_01"));
    popupText.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
    NotificationManager.Get().DestroyNotification(popupText, 6f);
  }

  private IEnumerator AdjustBigBrotherTransform()
  {
    ZonePlay enemyBattlefield = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone();
    Vector3 prevBattlefieldScale = enemyBattlefield.transform.localScale;
    enemyBattlefield.transform.localScale = 1.6f * enemyBattlefield.transform.localScale;
    Vector3 position = enemyBattlefield.transform.position;
    enemyBattlefield.transform.position = new Vector3(position.x + 2.393164f, position.y, position.z + 0.7f);
    Card bigBrotherCard = (Card) null;
    while ((Object) bigBrotherCard == (Object) null)
    {
      bigBrotherCard = this.FindCardInEnemyBattlefield("TU4c_007");
      if (!((Object) bigBrotherCard != (Object) null))
        yield return (object) null;
      else
        break;
    }
    while (!bigBrotherCard.IsActorReady())
      yield return (object) null;
    Actor actor = bigBrotherCard.GetActor();
    Transform parent = actor.transform.parent;
    Vector3 localScale = actor.transform.localScale;
    actor.transform.parent = (Transform) null;
    bigBrotherCard.transform.localScale = prevBattlefieldScale;
    actor.transform.parent = new GameObject()
    {
      transform = {
        parent = parent,
        localPosition = new Vector3(0.0f, 0.0f, 0.0f),
        localScale = new Vector3(1.6f, 1.6f, 1.6f)
      }
    }.transform;
    actor.transform.localScale = localScale;
    enemyBattlefield.transform.localScale = prevBattlefieldScale;
  }

  private Card FindCardInEnemyBattlefield(string cardId)
  {
    ZonePlay battlefieldZone = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone();
    for (int index = 0; index < battlefieldZone.GetCardCount(); ++index)
    {
      Card cardAtIndex = battlefieldZone.GetCardAtIndex(index);
      if (!(cardAtIndex.GetEntity().GetCardId() != cardId))
        return cardAtIndex;
    }
    return (Card) null;
  }

  public override void NotifyOfCardMousedOff(Entity mousedOffEntity) => this.GetGameOptions().SetBooleanOption(GameEntityOption.MOUSEOVER_DELAY_OVERRIDDEN, false);

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    if (!mousedOverEntity.HasTaunt())
      return;
    this.GetGameOptions().SetBooleanOption(GameEntityOption.MOUSEOVER_DELAY_OVERRIDDEN, true);
  }

  public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
  {
    if (!base.NotifyOfBattlefieldCardClicked(clickedEntity, wasInTargetMode))
      return false;
    if (!wasInTargetMode || !(clickedEntity.GetCardId() == "TU4c_007") || this.warnedAgainstAttackingGorilla)
      return true;
    this.warnedAgainstAttackingGorilla = true;
    this.HandleMissionEvent(11);
    return false;
  }

  public override void NotifyOfDefeatCoinAnimation()
  {
    if (!this.victory)
      return;
    this.PlaySound("VO_TUTORIAL_03_JAINA_20_36.prefab:79671f155307aa24a89b0581e4c5c4b2");
  }

  public override List<RewardData> GetCustomRewards()
  {
    if (!this.victory)
      return (List<RewardData>) null;
    List<RewardData> customRewards = new List<RewardData>();
    CardRewardData cardRewardData = new CardRewardData("CS2_022", TAG_PREMIUM.NORMAL, 2);
    cardRewardData.MarkAsDummyReward();
    customRewards.Add((RewardData) cardRewardData);
    return customRewards;
  }
}
