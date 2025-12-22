using Blizzard.T5.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_02 : TutorialEntity
{
  private Notification endTurnNotifier;
  private Notification manaNotifier;
  private Notification manaNotifier2;
  private GameObject costLabel;
  private int numManaThisTurn = 1;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_02_05.prefab:d1334881818e67d4c85216afa56226d6");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_01_04.prefab:5b48a6d28da46464ea99c7b278f63226");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_04_07.prefab:a804332a9a314af49b35d1c6d4a1f306");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_05_08.prefab:946dc71f989978844af5222d4342df4c");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_07_10.prefab:ffdc387467735484390ee8545698c57e");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_11_14.prefab:ada9c4aef7cd8dc418005c0a4c5f578d");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_13_16.prefab:80757414dc5a3b54b9cfc328ce2b7f6c");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_15_17.prefab:973e26c00c354b24595965035e8efba7");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_06_09.prefab:04bd4efe66a93bb438327216a4254560");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_03_06.prefab:c509f7e0eca4fb84dbf9be77a7ed5823");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_17_19.prefab:a7aab1a8c3e6d304a9b6f451187fdb00");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_08_11.prefab:21d83afbda98c8844b0ba771b14833e7");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_09_12.prefab:a050db78c641ba04d88382e2b759dbac");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_10_13.prefab:a22defa2f9b5ec242a1f4e502d9349eb");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_16_18.prefab:2493cb5abcdbf45468d74ab4ab4c10f8");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT.prefab:79394b29df25e894085524bdad538962");
    this.PreloadSound("VO_TUTORIAL_02_JAINA_08_22.prefab:52cd86a7a20daeb4b8d1f3fd2647e9ea");
    this.PreloadSound("VO_TUTORIAL_02_JAINA_03_18.prefab:4942e6b39e0bf0747b0ad09944cf9ad2");
    this.PreloadSound("VO_TUTORIAL02_MILLHOUSE_19_21.prefab:bc8b4236bf74f1244afa49a8195c7f74");
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      GameUtils.SetTutorialProgress(TutorialProgress.MILLHOUSE_COMPLETE);
      this.PlaySound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT.prefab:79394b29df25e894085524bdad538962");
    }
    else
    {
      if (gameResult != TAG_PLAYSTATE.TIED)
        return;
      this.PlaySound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT.prefab:79394b29df25e894085524bdad538962");
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    Tutorial_02 tutorial02 = this;
    if (GameState.Get().IsFriendlySidePlayerTurn())
      ++tutorial02.numManaThisTurn;
    Actor millhouseActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    AudioSource previousLine;
    AudioSource comeOnLine;
    switch (turn)
    {
      case 1:
        Vector3 crystalSpawnPosition1 = ManaCrystalMgr.Get().GetManaCrystalSpawnPosition();
        Vector3 position1;
        Notification.PopUpArrowDirection direction1;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          position1 = new Vector3(crystalSpawnPosition1.x - 0.7f, crystalSpawnPosition1.y + 1.14f, crystalSpawnPosition1.z + 4.33f);
          direction1 = Notification.PopUpArrowDirection.RightDown;
        }
        else
        {
          position1 = new Vector3(crystalSpawnPosition1.x - 0.02f, crystalSpawnPosition1.y + 0.2f, crystalSpawnPosition1.z + 1.8f);
          direction1 = Notification.PopUpArrowDirection.Down;
        }
        tutorial02.manaNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position1, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL02_HELP_01"));
        tutorial02.manaNotifier.ShowPopUpArrow(direction1);
        yield return (object) new WaitForSeconds(4.5f);
        if ((Object) tutorial02.manaNotifier != (Object) null)
        {
          iTween.PunchScale(tutorial02.manaNotifier.gameObject, iTween.Hash((object) "amount", (object) new Vector3(1f, 1f, 1f), (object) "time", (object) 1f));
          yield return (object) new WaitForSeconds(4.5f);
          if ((Object) tutorial02.manaNotifier != (Object) null)
          {
            iTween.PunchScale(tutorial02.manaNotifier.gameObject, iTween.Hash((object) "amount", (object) new Vector3(1f, 1f, 1f), (object) "time", (object) 1f));
            yield return (object) new WaitForSeconds(4.5f);
            if ((Object) tutorial02.manaNotifier != (Object) null)
            {
              NotificationManager.Get().DestroyNotification(tutorial02.manaNotifier, 0.0f);
              break;
            }
            break;
          }
          break;
        }
        break;
      case 2:
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(0.5f);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_04_07.prefab:a804332a9a314af49b35d1c6d4a1f306", "TUTORIAL02_MILLHOUSE_04", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        yield return (object) new WaitForSeconds(0.3f);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_05_08.prefab:946dc71f989978844af5222d4342df4c", "TUTORIAL02_MILLHOUSE_05", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        GameState.Get().SetBusy(false);
        break;
      case 3:
        Vector3 crystalSpawnPosition2 = ManaCrystalMgr.Get().GetManaCrystalSpawnPosition();
        Vector3 position2;
        Notification.PopUpArrowDirection direction2;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          position2 = new Vector3(crystalSpawnPosition2.x - 0.7f, crystalSpawnPosition2.y + 1.14f, crystalSpawnPosition2.z + 4.33f);
          direction2 = Notification.PopUpArrowDirection.RightDown;
        }
        else
        {
          position2 = new Vector3(crystalSpawnPosition2.x - 0.02f, crystalSpawnPosition2.y + 0.2f, crystalSpawnPosition2.z + 1.7f);
          direction2 = Notification.PopUpArrowDirection.Down;
        }
        tutorial02.manaNotifier2 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL02_HELP_03"));
        tutorial02.manaNotifier2.ShowPopUpArrow(direction2);
        yield return (object) new WaitForSeconds(4.5f);
        if ((Object) tutorial02.manaNotifier2 != (Object) null)
        {
          iTween.PunchScale(tutorial02.manaNotifier2.gameObject, iTween.Hash((object) "amount", (object) new Vector3(1f, 1f, 1f), (object) "time", (object) 1f));
          yield return (object) new WaitForSeconds(4.5f);
          if ((Object) tutorial02.manaNotifier2 != (Object) null)
          {
            iTween.PunchScale(tutorial02.manaNotifier2.gameObject, iTween.Hash((object) "amount", (object) new Vector3(1f, 1f, 1f), (object) "time", (object) 1f));
            break;
          }
          break;
        }
        break;
      case 4:
        if ((Object) tutorial02.manaNotifier2 != (Object) null)
          NotificationManager.Get().DestroyNotification(tutorial02.manaNotifier2, 0.0f);
        GameState.Get().SetBusy(true);
        previousLine = tutorial02.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_17_19.prefab:a7aab1a8c3e6d304a9b6f451187fdb00");
        while (SoundManager.Get().IsPlaying(previousLine))
          yield return (object) null;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_07_10.prefab:ffdc387467735484390ee8545698c57e", "TUTORIAL02_MILLHOUSE_07", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        GameState.Get().SetBusy(false);
        break;
      case 6:
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_11_14.prefab:ada9c4aef7cd8dc418005c0a4c5f578d", "TUTORIAL02_MILLHOUSE_11", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        GameState.Get().SetBusy(false);
        break;
      case 8:
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_13_16.prefab:80757414dc5a3b54b9cfc328ce2b7f6c", "TUTORIAL02_MILLHOUSE_13", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        GameState.Get().SetBusy(false);
        break;
      case 9:
        yield return (object) new WaitForSeconds(0.5f);
        Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_15_17.prefab:973e26c00c354b24595965035e8efba7", "TUTORIAL02_MILLHOUSE_15", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        break;
      case 10:
        GameState.Get().SetBusy(true);
        comeOnLine = tutorial02.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_16_18.prefab:2493cb5abcdbf45468d74ab4ab4c10f8");
        while (SoundManager.Get().IsPlaying(comeOnLine))
          yield return (object) null;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_06_09.prefab:04bd4efe66a93bb438327216a4254560", "TUTORIAL02_MILLHOUSE_06", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        GameState.Get().SetBusy(false);
        break;
    }
    previousLine = (AudioSource) null;
    comeOnLine = (AudioSource) null;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    Tutorial_02 tutorial02 = this;
    Actor millhouseActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    AudioSource feelslikeLine;
    AudioSource whatLine;
    AudioSource winngingLine;
    switch (missionEvent)
    {
      case 1:
        tutorial02.HandleGameStartEvent();
        break;
      case 2:
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(1.5f);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_03_06.prefab:c509f7e0eca4fb84dbf9be77a7ed5823", "TUTORIAL02_MILLHOUSE_03", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        GameState.Get().SetBusy(false);
        yield return (object) new WaitForSeconds(4f);
        if (tutorial02.GetTag(GAME_TAG.TURN) == 1 && !EndTurnButton.Get().IsInWaitingState())
        {
          tutorial02.ShowEndTurnBouncingArrow();
          break;
        }
        break;
      case 3:
        Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_17_19.prefab:a7aab1a8c3e6d304a9b6f451187fdb00", "TUTORIAL02_MILLHOUSE_17", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        break;
      case 4:
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_08_11.prefab:21d83afbda98c8844b0ba771b14833e7", "TUTORIAL02_MILLHOUSE_08", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL_02_JAINA_03_18.prefab:4942e6b39e0bf0747b0ad09944cf9ad2", "TUTORIAL02_JAINA_03", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_09_12.prefab:a050db78c641ba04d88382e2b759dbac", "TUTORIAL02_MILLHOUSE_09", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        break;
      case 5:
        GameState.Get().SetBusy(true);
        feelslikeLine = tutorial02.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_08_11.prefab:21d83afbda98c8844b0ba771b14833e7");
        while (SoundManager.Get().IsPlaying(feelslikeLine))
          yield return (object) null;
        whatLine = tutorial02.GetPreloadedSound("VO_TUTORIAL_02_JAINA_03_18.prefab:4942e6b39e0bf0747b0ad09944cf9ad2");
        while (SoundManager.Get().IsPlaying(whatLine))
          yield return (object) null;
        winngingLine = tutorial02.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_09_12.prefab:a050db78c641ba04d88382e2b759dbac");
        while (SoundManager.Get().IsPlaying(winngingLine))
          yield return (object) null;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_10_13.prefab:a22defa2f9b5ec242a1f4e502d9349eb", "TUTORIAL02_MILLHOUSE_10", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        GameState.Get().SetBusy(false);
        break;
      case 6:
        if (EndTurnButton.Get().IsInNMPState())
        {
          Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_16_18.prefab:2493cb5abcdbf45468d74ab4ab4c10f8", "TUTORIAL02_MILLHOUSE_16", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
          break;
        }
        break;
      case 54:
        yield return (object) new WaitForSeconds(2f);
        tutorial02.m_preTutorialNotification = tutorial02.ShowTutorialDialog("TUTORIAL02_HELP_06", "TUTORIAL02_HELP_07", "TUTORIAL01_HELP_16", new Vector2(0.5f, 0.0f));
        break;
      case 55:
        tutorial02.FadeInHeroActor(millhouseActor);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_02_05.prefab:d1334881818e67d4c85216afa56226d6", "TUTORIAL02_MILLHOUSE_02", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
        HistoryManager.Get().DisableHistory();
        MulliganManager.Get().BeginMulligan();
        yield return (object) new WaitForSeconds(1.1f);
        tutorial02.FadeOutHeroActor(millhouseActor);
        break;
    }
    feelslikeLine = (AudioSource) null;
    whatLine = (AudioSource) null;
    winngingLine = (AudioSource) null;
  }

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    if (mousedOverEntity.GetZone() != TAG_ZONE.HAND || this.GetTag(GAME_TAG.TURN) > 7)
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "NumberLabel.prefab:597544d5ed24b994f95fe56e28584992", new PrefabCallback<GameObject>(this.ManaLabelLoadedCallback), (object) mousedOverEntity.GetCard(), AssetLoadingOptions.IgnorePrefabPosition);
  }

  public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
  {
    if (!((Object) this.costLabel != (Object) null))
      return;
    Object.Destroy((Object) this.costLabel);
  }

  public override void NotifyOfCoinFlipResult() => Gameplay.Get().StartCoroutine(this.HandleCoinFlip());

  private IEnumerator HandleCoinFlip()
  {
    Tutorial_02 tutorial02 = this;
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(3.5f);
    Actor millhouseActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    tutorial02.FadeInHeroActor(millhouseActor);
    yield return (object) Gameplay.Get().StartCoroutine(tutorial02.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_01_04.prefab:5b48a6d28da46464ea99c7b278f63226", "TUTORIAL02_MILLHOUSE_01", Notification.SpeechBubbleDirection.TopRight, millhouseActor));
    GameState.Get().SetBusy(false);
    yield return (object) new WaitForSeconds(0.175f);
    tutorial02.FadeOutHeroActor(millhouseActor);
  }

  public override bool NotifyOfEndTurnButtonPushed()
  {
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket != null)
    {
      if (!optionsPacket.HasValidOption())
      {
        NotificationManager.Get().DestroyAllArrows();
        return true;
      }
      bool flag = false;
      for (int index = 0; index < optionsPacket.List.Count; ++index)
      {
        Network.Options.Option option = optionsPacket.List[index];
        if (option.Main.PlayErrorInfo.IsValid() && option.Type == Network.Options.Option.OptionType.POWER && !(GameState.Get().GetEntity(option.Main.ID).GetCardId() == "TU5_CS2_025"))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return true;
    }
    if ((Object) this.endTurnNotifier != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
    string key = "TUTORIAL_NO_ENDTURN";
    if (GameState.Get().GetFriendlySidePlayer().HasReadyAttackers())
      key = "TUTORIAL_NO_ENDTURN_ATK";
    this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
    return false;
  }

  private void ShowEndTurnBouncingArrow()
  {
    if (EndTurnButton.Get().IsInWaitingState())
      return;
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 2f, position1.y, position1.z);
    NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, -90f, 0.0f));
  }

  public override string[] NotifyOfKeywordHelpPanelDisplay(Entity entity)
  {
    if (entity.GetCardId() == "CS2_122")
      return new string[2]
      {
        GameStrings.Get("TUTORIAL_RAID_LEADER_HEADLINE"),
        GameStrings.Get("TUTORIAL_RAID_LEADER_DESCRIPTION")
      };
    if (!(entity.GetCardId() == "TU5_CS2_023"))
      return (string[]) null;
    return new string[2]
    {
      GameStrings.Get("TUTORIAL_ARCANE_INTELLECT_HEADLINE"),
      GameStrings.Get("TUTORIAL_ARCANE_INTELLECT_DESCRIPTION")
    };
  }

  public override void NotifyOfCardGrabbed(Entity entity)
  {
    if (entity.GetCardId() == "TU5_CS2_023" && GameState.Get().GetFriendlySidePlayer().GetNumAvailableResources() >= entity.GetCost())
      BoardTutorial.Get().EnableFullHighlight(true);
    if (!((Object) this.costLabel != (Object) null))
      return;
    Object.Destroy((Object) this.costLabel);
  }

  public override void NotifyOfCardDropped(Entity entity)
  {
    if (!(entity.GetCardId() == "TU5_CS2_023"))
      return;
    BoardTutorial.Get().EnableFullHighlight(false);
  }

  public override void NotifyOfManaCrystalSpawned()
  {
    AssetLoader.Get().InstantiatePrefab((AssetReference) "plus1.prefab:7427d28c07eea8645a3308e04398ee30", new PrefabCallback<GameObject>(this.Plus1ActorLoadedCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    if (this.GetTag(GAME_TAG.TURN) == 3)
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Gameplay.Get().StartCoroutine(this.PlaySoundAndWait("VO_TUTORIAL_02_JAINA_08_22.prefab:52cd86a7a20daeb4b8d1f3fd2647e9ea", "TUTORIAL02_JAINA_08", Notification.SpeechBubbleDirection.BottomLeft, actor));
    }
    this.FadeInManaSpotlight();
  }

  private void FadeInManaSpotlight() => Gameplay.Get().StartCoroutine(this.StartManaSpotFade());

  private IEnumerator StartManaSpotFade()
  {
    Light manaSpot = BoardTutorial.Get().m_ManaSpotlight;
    manaSpot.enabled = true;
    manaSpot.spotAngle = 179f;
    manaSpot.intensity = 0.0f;
    float TARGET_INTENSITY = 0.6f;
    while ((double) manaSpot.intensity < (double) TARGET_INTENSITY * 0.949999988079071)
    {
      manaSpot.intensity = Mathf.Lerp(manaSpot.intensity, TARGET_INTENSITY, Time.deltaTime * 5f);
      manaSpot.spotAngle = Mathf.Lerp(manaSpot.spotAngle, 80f, Time.deltaTime * 5f);
      yield return (object) null;
    }
    yield return (object) new WaitForSeconds(2f);
    while ((double) manaSpot.intensity > 0.0500000007450581)
    {
      manaSpot.intensity = Mathf.Lerp(manaSpot.intensity, 0.0f, Time.deltaTime * 10f);
      yield return (object) null;
    }
    manaSpot.enabled = false;
  }

  private void Plus1ActorLoadedCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    Vector3 position = GameObjectUtils.FindChildBySubstring(Board.Get().gameObject, "ManaCounter_Friendly").transform.position;
    Vector3 vector3 = new Vector3(position.x - 0.02f, position.y + 0.2f, position.z);
    go.transform.position = vector3;
    go.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
    Vector3 localScale = go.transform.localScale;
    go.transform.localScale = new Vector3(1f, 1f, 1f);
    iTween.MoveTo(go, new Vector3(vector3.x, vector3.y, vector3.z + 2f), 3f);
    float num = 2.5f;
    iTween.ScaleTo(go, new Vector3(localScale.x * num, localScale.y * num, localScale.z * num), 3f);
    iTween.RotateTo(go, new Vector3(0.0f, 170f, 0.0f), 3f);
    iTween.FadeTo(go, 0.0f, 2.75f);
  }

  public override void NotifyOfEnemyManaCrystalSpawned() => AssetLoader.Get().InstantiatePrefab((AssetReference) "plus1.prefab:7427d28c07eea8645a3308e04398ee30", new PrefabCallback<GameObject>(this.Plus1ActorLoadedCallbackEnemy), options: AssetLoadingOptions.IgnorePrefabPosition);

  private void Plus1ActorLoadedCallbackEnemy(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    Vector3 position = GameObjectUtils.FindChildBySubstring(Board.Get().gameObject, "ManaCounter_Opposing").transform.position;
    Vector3 vector3 = new Vector3(position.x, position.y + 0.2f, position.z);
    go.transform.position = vector3;
    go.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
    Vector3 localScale = go.transform.localScale;
    go.transform.localScale = new Vector3(1f, 1f, 1f);
    iTween.MoveTo(go, new Vector3(vector3.x, vector3.y, vector3.z - 2f), 3f);
    float num = 2.5f;
    iTween.ScaleTo(go, new Vector3(localScale.x * num, localScale.y * num, localScale.z * num), 3f);
    iTween.RotateTo(go, new Vector3(0.0f, 170f, 0.0f), 3f);
    iTween.FadeTo(go, 0.0f, 2.75f);
  }

  private void ManaLabelLoadedCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    GameObject costTextObject = ((Card) callbackData).GetActor().GetCostTextObject();
    if ((Object) costTextObject == (Object) null)
    {
      Object.Destroy((Object) go);
    }
    else
    {
      if ((Object) this.costLabel != (Object) null)
        Object.Destroy((Object) this.costLabel);
      this.costLabel = go;
      go.transform.parent = costTextObject.transform;
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.transform.localPosition = new Vector3(-0.025f, 0.28f, 0.0f);
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_COST");
    }
  }

  public override void NotifyOfTooltipZoneMouseOver(TooltipZone tooltip)
  {
    if (!((Object) tooltip.targetObject.GetComponent<ManaCrystalMgr>() != (Object) null))
      return;
    if ((Object) this.manaNotifier != (Object) null)
      Object.Destroy((Object) this.manaNotifier.gameObject);
    if (!((Object) this.manaNotifier2 != (Object) null))
      return;
    Object.Destroy((Object) this.manaNotifier2.gameObject);
  }

  public override string GetTurnStartReminderText() => GameStrings.Format("TUTORIAL02_HELP_04", (object) this.numManaThisTurn);

  public override void NotifyOfDefeatCoinAnimation() => Gameplay.Get().StartCoroutine(this.PlayGoingOutSound());

  private IEnumerator PlayGoingOutSound()
  {
    Tutorial_02 tutorial02 = this;
    AudioSource deathLine = tutorial02.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT.prefab:79394b29df25e894085524bdad538962");
    while ((Object) deathLine != (Object) null && deathLine.isPlaying)
      yield return (object) null;
    tutorial02.PlaySound("VO_TUTORIAL02_MILLHOUSE_19_21.prefab:bc8b4236bf74f1244afa49a8195c7f74");
  }

  protected override void NotifyOfManaError()
  {
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.manaNotifier);
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.manaNotifier2);
  }

  public override List<RewardData> GetCustomRewards()
  {
    List<RewardData> customRewards = new List<RewardData>();
    CardRewardData cardRewardData = new CardRewardData("EX1_015", TAG_PREMIUM.NORMAL, 2);
    cardRewardData.MarkAsDummyReward();
    customRewards.Add((RewardData) cardRewardData);
    return customRewards;
  }
}
