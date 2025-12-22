using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_04 : TutorialEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = Tutorial_04.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = Tutorial_04.InitStringOptions();
  private Notification endTurnNotifier;
  private Notification handBounceArrow;
  private Notification sheepTheBog;
  private Notification noSheepPopup;
  private int numBeastsPlayed;
  private GameObject m_heroPowerCostLabel;
  private Notification heroPowerHelp;
  private bool victory;
  private bool m_hemetSpeaking;
  private int numComplaintsMade;
  private bool m_shouldSignalPolymorph;
  private bool m_isPolymorphGrabbed;
  private bool m_isBogSheeped;
  private bool m_playOneHealthCommentNextTurn;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.KEYWORD_HELP_DELAY_OVERRIDDEN,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public Tutorial_04() => this.m_gameOptions.AddOptions(Tutorial_04.s_booleanOptions, Tutorial_04.s_stringOptions);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_TUTORIAL04_HEMET_23_21.prefab:86859f5cb798f304395a63f446fe9d00");
    this.PreloadSound("VO_TUTORIAL04_HEMET_15_13.prefab:c0da1267215708947a954e9c0ea1b061");
    this.PreloadSound("VO_TUTORIAL04_HEMET_20_18.prefab:5d49a0bac4c03b94e9e13945624a581b");
    this.PreloadSound("VO_TUTORIAL04_HEMET_16_14.prefab:df368c7075e4a2649803729f7b86601e");
    this.PreloadSound("VO_TUTORIAL04_HEMET_13_12.prefab:fe14ab273aa4b7e4491f30310a7d0eca");
    this.PreloadSound("VO_TUTORIAL04_HEMET_19_17.prefab:b9d5bd30659aae84b8a1380cbdba0398");
    this.PreloadSound("VO_TUTORIAL_04_JAINA_09_43.prefab:1ee05d74948aba04ebd7065e44813921");
    this.PreloadSound("VO_TUTORIAL_04_JAINA_10_44.prefab:6f5921db1071ead4585c8cc9689d22ea");
    this.PreloadSound("VO_TUTORIAL04_HEMET_06_05.prefab:2527939914db3e543941a13266e88a01");
    this.PreloadSound("VO_TUTORIAL04_HEMET_07_06_ALT.prefab:c19475ec3c3b0e648a97f423e0e86143");
    this.PreloadSound("VO_TUTORIAL_04_JAINA_04_40.prefab:5bfc80c6184279140878a51eb1fa3469");
    this.PreloadSound("VO_TUTORIAL04_HEMET_08_07.prefab:68207d2681a60c84d840d37c4b90740f");
    this.PreloadSound("VO_TUTORIAL04_HEMET_09_08.prefab:2994b6b35f2e5f54782b6100ea92f40e");
    this.PreloadSound("VO_TUTORIAL04_HEMET_10_09.prefab:3282099b41c7ab94aa99e84c20dd7db7");
    this.PreloadSound("VO_TUTORIAL04_HEMET_11_10.prefab:db8c8cea0db51d14fbd5d4c782b8b160");
    this.PreloadSound("VO_TUTORIAL04_HEMET_12_11.prefab:b0ea652d6f1ec6845845226680ade252");
    this.PreloadSound("VO_TUTORIAL04_HEMET_12_11_ALT.prefab:b59f55e14876bee43a0d9ab4b7317f84");
    this.PreloadSound("VO_TUTORIAL_04_JAINA_08_42.prefab:36763b11766e2b64198719d44b0fa8bf");
    this.PreloadSound("VO_TUTORIAL04_HEMET_01_01.prefab:89be0839b16c1244a9221b373fd8fb61");
    this.PreloadSound("VO_TUTORIAL_04_JAINA_01_37.prefab:c7fc40d1595ca3c49b524b9929264477");
    this.PreloadSound("VO_TUTORIAL04_HEMET_02_02.prefab:c3ca1337cb01efe4194899d42918f80c");
    this.PreloadSound("VO_TUTORIAL04_HEMET_03_03.prefab:b014c684c85f1c440bed5560c6b6dbf5");
    this.PreloadSound("VO_TUTORIAL_04_JAINA_02_38.prefab:83b64d5eeb884db43b9fa5f20316da2c");
    this.PreloadSound("VO_TUTORIAL04_HEMET_04_04_ALT.prefab:bb3fadd78adce274993862115f3c5137");
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON)
      this.victory = true;
    base.NotifyOfGameOver(gameResult);
    if ((Object) this.m_heroPowerCostLabel != (Object) null)
      Object.Destroy((Object) this.m_heroPowerCostLabel);
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        GameUtils.SetTutorialProgress(TutorialProgress.NESINGWARY_COMPLETE);
        this.PlaySound("VO_TUTORIAL04_HEMET_23_21.prefab:86859f5cb798f304395a63f446fe9d00");
        break;
      case TAG_PLAYSTATE.LOST:
        this.SetTutorialLostProgress(TutorialProgress.NESINGWARY_COMPLETE);
        break;
      case TAG_PLAYSTATE.TIED:
        this.PlaySound("VO_TUTORIAL04_HEMET_23_21.prefab:86859f5cb798f304395a63f446fe9d00");
        break;
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    Tutorial_04 tutorial04 = this;
    tutorial04.m_shouldSignalPolymorph = false;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (tutorial04.m_playOneHealthCommentNextTurn)
    {
      tutorial04.m_playOneHealthCommentNextTurn = false;
      GameState.Get().SetBusy(true);
      yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_08_42.prefab:36763b11766e2b64198719d44b0fa8bf", "TUTORIAL04_JAINA_08", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
      GameState.Get().SetBusy(false);
    }
    switch (turn)
    {
      case 1:
        if (tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_15_13.prefab:c0da1267215708947a954e9c0ea1b061", "TUTORIAL04_HEMET_15", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 4:
        yield return (object) new WaitForSeconds(1f);
        Vector3 position1 = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard().transform.position;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          Vector3 position2 = new Vector3(position1.x, position1.y, position1.z + 2.3f);
          tutorial04.heroPowerHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL04_HELP_01"));
          tutorial04.heroPowerHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
          break;
        }
        Vector3 position3 = new Vector3(position1.x + 3f, position1.y, position1.z);
        tutorial04.heroPowerHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position3, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL04_HELP_01"));
        tutorial04.heroPowerHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        AssetLoader.Get().InstantiatePrefab((AssetReference) "NumberLabel.prefab:597544d5ed24b994f95fe56e28584992", new PrefabCallback<GameObject>(tutorial04.ManaLabelLoadedCallback), (object) GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard(), AssetLoadingOptions.IgnorePrefabPosition);
        break;
      case 5:
        NotificationManager.Get().DestroyNotification(tutorial04.heroPowerHelp, 0.0f);
        Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_20_18.prefab:5d49a0bac4c03b94e9e13945624a581b", "TUTORIAL04_HEMET_20", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 7:
        if (tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_16_14.prefab:df368c7075e4a2649803729f7b86601e", "TUTORIAL04_HEMET_16", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 9:
        if (tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_13_12.prefab:fe14ab273aa4b7e4491f30310a7d0eca", "TUTORIAL04_HEMET_13", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 11:
        GameState.Get().SetBusy(true);
        Gameplay.Get().SetGameStateBusy(false, 3f);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_19_17.prefab:b9d5bd30659aae84b8a1380cbdba0398", "TUTORIAL04_HEMET_19", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        yield return (object) new WaitForSeconds(0.7f);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_09_43.prefab:1ee05d74948aba04ebd7065e44813921", "TUTORIAL04_JAINA_09", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        break;
      case 12:
        if (!tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
          break;
        tutorial04.m_shouldSignalPolymorph = true;
        List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
        if (!((Object) InputManager.Get().GetHeldCard() == (Object) null))
          break;
        Card card1 = (Card) null;
        foreach (Card card2 in cards)
        {
          if (card2.GetEntity().GetCardId() == "TU5_CS2_022")
            card1 = card2;
        }
        if ((Object) card1 == (Object) null || card1.IsMousedOver())
          break;
        Gameplay.Get().StartCoroutine(tutorial04.ShowArrowInSeconds(0.0f));
        break;
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    Tutorial_04 tutorial04 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        tutorial04.HandleGameStartEvent();
        break;
      case 2:
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_06_05.prefab:2527939914db3e543941a13266e88a01", "TUTORIAL04_HEMET_06", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_07_06_ALT.prefab:c19475ec3c3b0e648a97f423e0e86143", "TUTORIAL04_HEMET_07", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.Wait(1f));
        GameState.Get().SetBusy(false);
        break;
      case 3:
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.Wait(2f));
        GameState.Get().SetBusy(false);
        break;
      case 4:
        if ((bool) UniversalInputManager.UsePhoneUI)
          InputManager.Get().GetFriendlyHand().SetHandEnlarged(false);
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_04_40.prefab:5bfc80c6184279140878a51eb1fa3469", "TUTORIAL04_JAINA_04", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        GameState.Get().SetBusy(false);
        break;
      case 5:
        if (tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
          break;
        switch (tutorial04.numBeastsPlayed)
        {
          case 0:
            Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_08_07.prefab:68207d2681a60c84d840d37c4b90740f", "TUTORIAL04_HEMET_08", Notification.SpeechBubbleDirection.TopRight, enemyActor));
            break;
          case 1:
            Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_09_08.prefab:2994b6b35f2e5f54782b6100ea92f40e", "TUTORIAL04_HEMET_09", Notification.SpeechBubbleDirection.TopRight, enemyActor));
            break;
          case 2:
            Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_10_09.prefab:3282099b41c7ab94aa99e84c20dd7db7", "TUTORIAL04_HEMET_10", Notification.SpeechBubbleDirection.TopRight, enemyActor));
            break;
          case 3:
            Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_11_10.prefab:db8c8cea0db51d14fbd5d4c782b8b160", "TUTORIAL04_HEMET_11", Notification.SpeechBubbleDirection.TopRight, enemyActor));
            break;
        }
        ++tutorial04.numBeastsPlayed;
        break;
      case 6:
        if (tutorial04.numComplaintsMade == 0)
        {
          Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_12_11.prefab:b0ea652d6f1ec6845845226680ade252", "TUTORIAL04_HEMET_12a", Notification.SpeechBubbleDirection.TopRight, enemyActor));
          ++tutorial04.numComplaintsMade;
          break;
        }
        Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_12_11_ALT.prefab:b59f55e14876bee43a0d9ab4b7317f84", "TUTORIAL04_HEMET_12b", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 7:
        tutorial04.m_playOneHealthCommentNextTurn = true;
        break;
      case 54:
        yield return (object) new WaitForSeconds(2f);
        string bodyTextGameString = !tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE) ? "TUTORIAL04_HELP_15" : "TUTORIAL04_HELP_16";
        tutorial04.m_preTutorialNotification = tutorial04.ShowTutorialDialog("TUTORIAL04_HELP_14", bodyTextGameString, "TUTORIAL01_HELP_16", Vector2.zero, true);
        break;
      case 55:
        if (!tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
        {
          tutorial04.FadeInHeroActor(enemyActor);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_01_01.prefab:89be0839b16c1244a9221b373fd8fb61", "TUTORIAL04_HEMET_01", Notification.SpeechBubbleDirection.TopRight, enemyActor));
          tutorial04.FadeOutHeroActor(enemyActor);
          tutorial04.FadeInHeroActor(jainaActor);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_01_37.prefab:c7fc40d1595ca3c49b524b9929264477", "TUTORIAL04_JAINA_01", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
          tutorial04.FadeOutHeroActor(jainaActor);
          yield return (object) new WaitForSeconds(0.5f);
        }
        MulliganManager.Get().BeginMulligan();
        if (tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
          break;
        tutorial04.m_hemetSpeaking = true;
        tutorial04.FadeInHeroActor(enemyActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_02_02.prefab:c3ca1337cb01efe4194899d42918f80c", "TUTORIAL04_HEMET_02", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        tutorial04.FadeOutHeroActor(enemyActor);
        tutorial04.m_hemetSpeaking = false;
        break;
    }
  }

  public override void NotifyOfCoinFlipResult() => Gameplay.Get().StartCoroutine(this.HandleCoinFlip());

  private IEnumerator HandleCoinFlip()
  {
    Tutorial_04 tutorial04 = this;
    GameState.Get().SetBusy(true);
    yield return (object) Gameplay.Get().StartCoroutine(tutorial04.Wait(1f));
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    while (tutorial04.m_hemetSpeaking)
      yield return (object) null;
    tutorial04.FadeInHeroActor(enemyActor);
    yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_03_03.prefab:b014c684c85f1c440bed5560c6b6dbf5", "TUTORIAL04_HEMET_03", Notification.SpeechBubbleDirection.TopRight, enemyActor));
    tutorial04.FadeOutHeroActor(enemyActor);
    yield return (object) new WaitForSeconds(0.3f);
    tutorial04.FadeInHeroActor(jainaActor);
    yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_02_38.prefab:83b64d5eeb884db43b9fa5f20316da2c", "TUTORIAL04_JAINA_02", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
    tutorial04.FadeOutHeroActor(jainaActor);
    yield return (object) new WaitForSeconds(0.25f);
    if (!tutorial04.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
    {
      tutorial04.FadeInHeroActor(enemyActor);
      yield return (object) Gameplay.Get().StartCoroutine(tutorial04.PlaySoundAndWait("VO_TUTORIAL04_HEMET_04_04_ALT.prefab:bb3fadd78adce274993862115f3c5137", "TUTORIAL04_HEMET_04", Notification.SpeechBubbleDirection.TopRight, enemyActor));
      tutorial04.FadeOutHeroActor(enemyActor);
      yield return (object) new WaitForSeconds(0.4f);
    }
    GameState.Get().SetBusy(false);
  }

  private IEnumerator Wait(float seconds)
  {
    yield return (object) new WaitForSeconds(seconds);
  }

  private bool AllowEndTurn() => !this.m_shouldSignalPolymorph || this.m_shouldSignalPolymorph && this.m_isBogSheeped;

  public override bool NotifyOfEndTurnButtonPushed()
  {
    if (this.GetTag(GAME_TAG.TURN) != 4 && this.AllowEndTurn())
    {
      NotificationManager.Get().DestroyAllArrows();
      return true;
    }
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket != null && !optionsPacket.HasValidOption())
    {
      NotificationManager.Get().DestroyAllArrows();
      return true;
    }
    if ((Object) this.endTurnNotifier != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
    string key = "TUTORIAL_NO_ENDTURN_HP";
    if (GameState.Get().GetFriendlySidePlayer().HasReadyAttackers())
      key = "TUTORIAL_NO_ENDTURN_ATK";
    if (this.m_shouldSignalPolymorph && !this.m_isBogSheeped)
      key = "TUTORIAL_NO_ENDTURN";
    this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
    return false;
  }

  public override void NotifyOfTargetModeCancelled()
  {
    if ((Object) this.sheepTheBog == (Object) null)
      return;
    NotificationManager.Get().DestroyAllPopUps();
  }

  public override void NotifyOfCardGrabbed(Entity entity)
  {
    if (!this.m_shouldSignalPolymorph)
      return;
    if (entity.GetCardId() == "TU5_CS2_022")
    {
      this.m_isPolymorphGrabbed = true;
      if ((Object) this.sheepTheBog != (Object) null)
        NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
      if ((Object) this.handBounceArrow != (Object) null)
        NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.handBounceArrow);
      NotificationManager.Get().DestroyAllPopUps();
      Vector3 position1 = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard().transform.position;
      Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
      this.sheepTheBog = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL04_HELP_02"));
      this.sheepTheBog.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
    }
    else
    {
      if ((Object) this.sheepTheBog != (Object) null)
        NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
      NotificationManager.Get().DestroyAllPopUps();
      if ((bool) UniversalInputManager.UsePhoneUI)
        InputManager.Get().ReturnHeldCardToHand();
      else
        InputManager.Get().DropHeldCard();
    }
  }

  public override void NotifyOfCardDropped(Entity entity)
  {
    this.m_isPolymorphGrabbed = false;
    if (!this.m_shouldSignalPolymorph)
      return;
    if ((Object) this.sheepTheBog != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
    NotificationManager.Get().DestroyAllPopUps();
    if (!this.ShouldShowArrowOnCardInHand(entity))
      return;
    Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0.5f));
  }

  public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
  {
    if (this.m_shouldSignalPolymorph)
    {
      if (clickedEntity.GetCardId() == "TU5_CS1_069" & wasInTargetMode)
      {
        if ((Object) this.sheepTheBog != (Object) null)
          NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
        NotificationManager.Get().DestroyAllPopUps();
        this.m_shouldSignalPolymorph = false;
        this.m_isBogSheeped = true;
      }
      else
      {
        if (!(this.m_isPolymorphGrabbed & wasInTargetMode))
          return false;
        if ((Object) this.noSheepPopup != (Object) null)
          NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.noSheepPopup);
        Vector3 position1 = clickedEntity.GetCard().transform.position;
        Vector3 position2 = new Vector3(position1.x + 2.5f, position1.y, position1.z);
        this.noSheepPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL04_HELP_03"));
        NotificationManager.Get().DestroyNotification(this.noSheepPopup, 3f);
        return false;
      }
    }
    return true;
  }

  public override bool ShouldAllowCardGrab(Entity entity) => !this.m_shouldSignalPolymorph || !(entity.GetCardId() != "TU5_CS2_022");

  private void ManaLabelLoadedCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    GameObject costTextObject = ((Card) callbackData).GetActor().GetCostTextObject();
    if ((Object) costTextObject == (Object) null)
    {
      Object.Destroy((Object) go);
    }
    else
    {
      this.m_heroPowerCostLabel = go;
      LayerUtils.SetLayer(go, GameLayer.Default);
      go.transform.parent = costTextObject.transform;
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.transform.localPosition = new Vector3(-0.02f, 0.38f, 0.0f);
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.x, go.transform.localScale.x);
      go.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_COST");
    }
  }

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    if (!this.ShouldShowArrowOnCardInHand(mousedOverEntity))
      return;
    NotificationManager.Get().DestroyAllArrows();
  }

  public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
  {
    if (!this.ShouldShowArrowOnCardInHand(mousedOffEntity))
      return;
    Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0.5f));
  }

  private bool ShouldShowArrowOnCardInHand(Entity entity) => entity.GetZone() == TAG_ZONE.HAND && this.m_shouldSignalPolymorph && entity.GetCardId() == "TU5_CS2_022";

  private IEnumerator ShowArrowInSeconds(float seconds)
  {
    yield return (object) new WaitForSeconds(seconds);
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count != 0 && !this.m_isPolymorphGrabbed)
    {
      Card polyMorph = (Card) null;
      foreach (Card card in cards)
      {
        if (card.GetEntity().GetCardId() == "TU5_CS2_022")
          polyMorph = card;
      }
      if (!((Object) polyMorph == (Object) null))
      {
        while (iTween.Count(polyMorph.gameObject) > 0)
          yield return (object) null;
        if (!polyMorph.IsMousedOver() && !((Object) InputManager.Get().GetHeldCard() == (Object) polyMorph))
          this.ShowHandBouncingArrow();
      }
    }
  }

  private void ShowHandBouncingArrow()
  {
    if ((Object) this.handBounceArrow != (Object) null)
      return;
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count == 0)
      return;
    Card card1 = (Card) null;
    foreach (Card card2 in cards)
    {
      if (card2.GetEntity().GetCardId() == "TU5_CS2_022")
        card1 = card2;
    }
    if ((Object) card1 == (Object) null || this.m_isPolymorphGrabbed)
      return;
    Vector3 position1 = card1.transform.position;
    Vector3 position2 = new Vector3(position1.x, position1.y, position1.z + 2f);
    this.handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, 0.0f, 0.0f));
    this.handBounceArrow.transform.parent = card1.transform;
  }

  public override void NotifyOfDefeatCoinAnimation()
  {
    if (!this.victory)
      return;
    this.PlaySound("VO_TUTORIAL_04_JAINA_10_44.prefab:6f5921db1071ead4585c8cc9689d22ea");
  }

  public override List<RewardData> GetCustomRewards()
  {
    if (!this.victory)
      return (List<RewardData>) null;
    List<RewardData> customRewards = new List<RewardData>();
    CardRewardData cardRewardData = new CardRewardData("CS2_213", TAG_PREMIUM.NORMAL, 2);
    cardRewardData.MarkAsDummyReward();
    customRewards.Add((RewardData) cardRewardData);
    return customRewards;
  }
}
