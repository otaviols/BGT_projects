using Assets;
using Blizzard.T5.Core.Utils;
using Hearthstone;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureScene : PegasusScene
{
  private static AdventureScene s_instance;
  [CustomEditField(Sections = "Transition Blocker")]
  public GameObject m_transitionClickBlocker;
  [CustomEditField(Sections = "Transition Motions")]
  public Vector3 m_SubScenePosition = Vector3.zero;
  [CustomEditField(Sections = "Transition Motions")]
  public float m_DefaultTransitionAnimationTime = 1f;
  [CustomEditField(Sections = "Transition Motions")]
  public iTween.EaseType m_TransitionEaseType = iTween.EaseType.easeInOutSine;
  [CustomEditField(Sections = "Transition Motions")]
  public AdventureScene.TransitionDirection m_TransitionDirection;
  [CustomEditField(Sections = "Transition Sounds", T = EditType.SOUND_PREFAB)]
  public string m_SlideInSound;
  [CustomEditField(Sections = "Transition Sounds", T = EditType.SOUND_PREFAB)]
  public string m_SlideOutSound;
  [CustomEditField(Sections = "Adventure Subscene Prefabs")]
  public List<AdventureScene.AdventureSubSceneDef> m_SubSceneDefs = new List<AdventureScene.AdventureSubSceneDef>();
  [CustomEditField(Sections = "Music Settings")]
  public List<AdventureScene.AdventureModeMusic> m_AdventureModeMusic = new List<AdventureScene.AdventureModeMusic>();
  private GameObject m_TransitionOutSubSceneParent;
  private GameObject m_CurrentSubSceneParent;
  private GameObject m_TransitionOutSubScene;
  private GameObject m_CurrentSubScene;
  private bool m_transitionIsGoingBack;
  private int m_StartupAssetLoads;
  private int m_SubScenesLoaded;
  private bool m_MusicStopped;
  private bool m_Unloading;
  private AdventureScene.TransitionDirection m_CurrentTransitionDirection;
  private bool m_isTransitioning;
  private bool m_isLoading;
  private Coroutine m_waitForSubSceneToLoadCoroutine;
  private const AdventureData.Adventuresubscene s_StartMode = AdventureData.Adventuresubscene.CHOOSER;
  private List<AdventureDbId> m_adventuresThatRequestedGameSaveData = new List<AdventureDbId>();
  private AdventureDefCache m_adventureDefCache;
  private AdventureWingDefCache m_adventureWingDefCache;

  public bool IsDevMode { get; set; }

  public int DevModeSetting { get; set; }

  protected override void Awake()
  {
    base.Awake();
    AdventureScene.s_instance = this;
    this.m_CurrentSubScene = (GameObject) null;
    this.m_TransitionOutSubScene = (GameObject) null;
    this.m_CurrentTransitionDirection = this.m_TransitionDirection;
    AdventureConfig adventureConfig = AdventureConfig.Get();
    adventureConfig.OnAdventureSceneAwake();
    adventureConfig.AddSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubSceneChange));
    adventureConfig.AddSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChanged));
    adventureConfig.AddAdventureModeChangeListener(new AdventureConfig.AdventureModeChange(this.OnAdventureModeChanged));
    adventureConfig.AddAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnAdventureMissionChanged));
    ++this.m_StartupAssetLoads;
    this.SetCurrentTransitionDirection();
    if (HearthstoneApplication.IsInternal())
    {
      CheatMgr.Get().RegisterCategory("adventure");
      CheatMgr.Get().RegisterCheatHandler("advdev", new CheatMgr.ProcessCheatCallback(this.OnDevCheat));
      CheatMgr.Get().DefaultCategory();
    }
    this.m_adventureDefCache = new AdventureDefCache(true);
    this.m_adventureWingDefCache = new AdventureWingDefCache(true);
    this.NotifyAchieveManagerOfAdventureSceneLoaded();
    this.LoadSubScene(adventureConfig.CurrentSubScene, new GameObjectCallback(this.OnFirstSubSceneLoaded), (object) new Action(this.OnStartupAssetLoaded));
  }

  private void Start() => AdventureConfig.Get().UpdatePresence();

  private void OnDestroy() => AdventureScene.s_instance = (AdventureScene) null;

  private void Update() => Network.Get().ProcessNetwork();

  public static AdventureScene Get() => AdventureScene.s_instance;

  public override bool IsUnloading() => this.m_Unloading;

  public override void Unload()
  {
    this.m_Unloading = true;
    AdventureConfig adventureConfig = AdventureConfig.Get();
    adventureConfig.ClearBossDefs();
    DeckPickerTray.Get().Unload();
    adventureConfig.RemoveAdventureModeChangeListener(new AdventureConfig.AdventureModeChange(this.OnAdventureModeChanged));
    adventureConfig.RemoveSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChanged));
    adventureConfig.RemoveSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubSceneChange));
    adventureConfig.OnAdventureSceneUnload();
    CheatMgr.Get().UnregisterCheatHandler("advdev", new CheatMgr.ProcessCheatCallback(this.OnDevCheat));
    if ((UnityEngine.Object) this.m_CurrentSubScene != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_CurrentSubScene);
    if ((UnityEngine.Object) this.m_transitionClickBlocker != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_transitionClickBlocker);
    this.m_Unloading = false;
  }

  public override bool IsTransitioning() => this.m_isTransitioning;

  public bool IsInitialScreen() => this.m_SubScenesLoaded <= 1;

  public AdventureDef GetAdventureDef(AdventureDbId advId) => this.m_adventureDefCache.GetDef(advId);

  public List<AdventureDef> GetSortedAdventureDefs()
  {
    List<AdventureDef> sortedAdventureDefs = new List<AdventureDef>(this.m_adventureDefCache.Values);
    sortedAdventureDefs.Sort((Comparison<AdventureDef>) ((l, r) => r.GetSortOrder() - l.GetSortOrder()));
    return sortedAdventureDefs;
  }

  public AdventureWingDef GetWingDef(WingDbId wingId) => this.m_adventureWingDefCache.GetDef(wingId);

  private void UpdateAdventureModeMusic()
  {
    AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
    AdventureData.Adventuresubscene currentSubScene = AdventureConfig.Get().CurrentSubScene;
    MusicPlaylistType? nullable = new MusicPlaylistType?();
    foreach (AdventureScene.AdventureModeMusic music in this.m_AdventureModeMusic)
    {
      if (music.m_subsceneId == currentSubScene && music.m_adventureId == selectedAdventure)
      {
        MusicPlaylistType? playlist;
        nullable = !AdventureScene.GetAdventureModeMusicWingOverride(music, out playlist) ? new MusicPlaylistType?(music.m_playlist) : playlist;
        break;
      }
      if (music.m_subsceneId == currentSubScene && music.m_adventureId == AdventureDbId.INVALID)
        nullable = new MusicPlaylistType?(music.m_playlist);
    }
    if (!nullable.HasValue)
      return;
    MusicManager.Get().StartPlaylist(nullable.Value);
  }

  private static bool GetAdventureModeMusicWingOverride(
    AdventureScene.AdventureModeMusic music,
    out MusicPlaylistType? playlist)
  {
    playlist = new MusicPlaylistType?();
    if (music == null || music.m_wingOverrides.Count == 0)
      return false;
    ScenarioDbId lastSelectedMission = AdventureConfig.Get().GetLastSelectedMission();
    if (lastSelectedMission == ScenarioDbId.INVALID)
      return false;
    WingDbfRecord recordFromMissionId = GameUtils.GetWingRecordFromMissionId((int) lastSelectedMission);
    WingDbId wingDbId = recordFromMissionId != null ? (WingDbId) recordFromMissionId.ID : WingDbId.INVALID;
    if (wingDbId == WingDbId.INVALID)
      return false;
    foreach (AdventureScene.AdventureModeMusicWingOverride wingOverride in music.m_wingOverrides)
    {
      if (wingOverride.m_wingId == wingDbId)
      {
        playlist = new MusicPlaylistType?(wingOverride.m_playlist);
        return true;
      }
    }
    return false;
  }

  private void OnStartupAssetLoaded()
  {
    --this.m_StartupAssetLoads;
    if (this.m_StartupAssetLoads > 0)
      return;
    this.UpdateAdventureModeMusic();
    SceneMgr.Get().NotifySceneLoaded();
  }

  private void LoadSubScene(AdventureData.Adventuresubscene subscene) => this.LoadSubScene(subscene, new GameObjectCallback(this.OnSubSceneLoaded));

  private void LoadSubScene(
    AdventureData.Adventuresubscene subscene,
    GameObjectCallback callback,
    object callbackData = null)
  {
    AdventureScene.AdventureSubSceneDef subSceneDef = this.m_SubSceneDefs.Find((Predicate<AdventureScene.AdventureSubSceneDef>) (item => item.m_SubScene == subscene));
    if (subSceneDef == null)
    {
      Debug.LogErrorFormat("Subscene {0} prefab not defined in m_SubSceneDefs", (object) subscene);
    }
    else
    {
      if (this.m_isLoading)
        Debug.LogErrorFormat("Attempting to load subscene {0}, but another subscene is already loading! This is a bad idea!", (object) subscene);
      this.m_isTransitioning = true;
      this.m_isLoading = true;
      this.EnableTransitionBlocker(true);
      if (this.m_waitForSubSceneToLoadCoroutine != null)
        this.StopCoroutine(this.m_waitForSubSceneToLoadCoroutine);
      GameObjectCallback runCallback = callback;
      if (subSceneDef.isWidget)
      {
        WidgetInstance widgetInstance = WidgetInstance.Create((string) (MobileOverrideValue<string>) subSceneDef.m_Prefab);
        widgetInstance.RegisterReadyListener((Action<object>) (_ =>
        {
          this.SetUpSubSceneParent(widgetInstance.gameObject);
          if (runCallback != null)
            runCallback((AssetReference) (string) (MobileOverrideValue<string>) subSceneDef.m_Prefab, widgetInstance.Widget.gameObject, callbackData);
          this.UpdateAdventureModeMusic();
          this.m_isLoading = false;
        }), (object) null, true);
      }
      else
        AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) subSceneDef.m_Prefab, (PrefabCallback<GameObject>) ((assetRef, go, data) =>
        {
          this.SetUpSubSceneParent(go);
          if (runCallback != null)
            runCallback(assetRef, go, data);
          this.UpdateAdventureModeMusic();
          this.m_isLoading = false;
        }), callbackData, AssetLoadingOptions.IgnorePrefabPosition);
    }
  }

  private void OnSubSceneChange(AdventureData.Adventuresubscene newscene, bool forward)
  {
    this.m_transitionIsGoingBack = !forward;
    this.LoadSubScene(newscene);
  }

  private Vector3 GetMoveDirection()
  {
    float num = 1f;
    if (this.m_CurrentTransitionDirection >= AdventureScene.TransitionDirection.NX)
      num *= -1f;
    Vector3 zero = Vector3.zero;
    zero[(int) this.m_CurrentTransitionDirection % 3] = num;
    return zero;
  }

  private void OnFirstSubSceneLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.ShowExpertAIUnlockTip();
    this.OnSubSceneLoaded(assetRef, go, callbackData);
  }

  private void SetUpSubSceneParent(GameObject parent)
  {
    this.m_TransitionOutSubSceneParent = this.m_CurrentSubSceneParent;
    this.m_CurrentSubSceneParent = parent;
    GameUtils.SetParent(this.m_CurrentSubSceneParent, (Component) this.transform);
    this.m_CurrentSubSceneParent.transform.position = new Vector3(-500f, 0.0f, 0.0f);
  }

  private void OnSubSceneLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_TransitionOutSubScene = this.m_CurrentSubScene;
    this.m_CurrentSubScene = go;
    ++this.m_SubScenesLoaded;
    AdventureSubScene component = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
    Action callback = (Action) callbackData;
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      this.DoSubSceneTransition(component);
      if (callback == null)
        return;
      callback();
    }
    else
      this.m_waitForSubSceneToLoadCoroutine = this.StartCoroutine(this.WaitForSubSceneToLoad(callback));
  }

  private void DoSubSceneTransition(AdventureSubScene subscene)
  {
    this.m_CurrentSubSceneParent.transform.localPosition = this.m_SubScenePosition;
    if ((UnityEngine.Object) this.m_TransitionOutSubSceneParent == (UnityEngine.Object) null)
    {
      this.CompleteTransition();
    }
    else
    {
      float num1 = (UnityEngine.Object) subscene == (UnityEngine.Object) null ? this.m_DefaultTransitionAnimationTime : subscene.m_TransitionAnimationTime;
      Vector3 moveDirection = this.GetMoveDirection();
      GameObject delobj = this.m_TransitionOutSubSceneParent;
      AdventureSubScene component1 = this.m_TransitionOutSubScene.GetComponent<AdventureSubScene>();
      bool flag1 = this.m_transitionIsGoingBack;
      int num2 = !((UnityEngine.Object) component1 != (UnityEngine.Object) null) || !component1.m_reverseTransitionAfterThisSubscene ? 0 : (!this.m_transitionIsGoingBack ? 1 : 0);
      bool flag2 = (UnityEngine.Object) subscene != (UnityEngine.Object) null && subscene.m_reverseTransitionAfterThisSubscene && this.m_transitionIsGoingBack;
      bool flag3 = (UnityEngine.Object) subscene != (UnityEngine.Object) null && subscene.m_reverseTransitionBeforeThisSubscene && !this.m_transitionIsGoingBack;
      bool flag4 = (UnityEngine.Object) component1 != (UnityEngine.Object) null && component1.m_reverseTransitionBeforeThisSubscene && this.m_transitionIsGoingBack;
      int num3 = flag2 ? 1 : 0;
      if ((num2 | num3 | (flag3 ? 1 : 0) | (flag4 ? 1 : 0)) != 0)
        flag1 = !flag1;
      if (flag1)
      {
        AdventureSubScene adventureSubScene = component1;
        Vector3 vector3 = (UnityEngine.Object) adventureSubScene == (UnityEngine.Object) null ? TransformUtil.GetBoundsOfChildren(this.m_TransitionOutSubScene).size : (Vector3) (MobileOverrideValue<Vector3>) adventureSubScene.m_SubSceneBounds;
        Vector3 localPosition = this.m_TransitionOutSubSceneParent.transform.localPosition;
        localPosition.x -= vector3.x * moveDirection.x;
        localPosition.y -= vector3.y * moveDirection.y;
        localPosition.z -= vector3.z * moveDirection.z;
        iTween.MoveTo(this.m_TransitionOutSubScene, iTween.Hash((object) "islocal", (object) true, (object) "position", (object) localPosition, (object) "time", (object) num1, (object) "easeType", (object) this.m_TransitionEaseType, (object) "oncomplete", (object) (Action<object>) (e =>
        {
          this.DestroyTransitioningSubScene(delobj);
          this.CompleteTransition();
        }), (object) "oncompletetarget", (object) this.gameObject));
        if (!string.IsNullOrEmpty(this.m_SlideOutSound))
          SoundManager.Get().LoadAndPlay((AssetReference) this.m_SlideOutSound);
      }
      else
      {
        AdventureSubScene component2 = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
        Vector3 vector3 = (UnityEngine.Object) component2 == (UnityEngine.Object) null ? TransformUtil.GetBoundsOfChildren(this.m_CurrentSubScene).size : (Vector3) (MobileOverrideValue<Vector3>) component2.m_SubSceneBounds;
        Vector3 localPosition1 = this.m_CurrentSubSceneParent.transform.localPosition;
        Vector3 localPosition2 = this.m_CurrentSubSceneParent.transform.localPosition;
        localPosition2.x -= vector3.x * moveDirection.x;
        localPosition2.y -= vector3.y * moveDirection.y;
        localPosition2.z -= vector3.z * moveDirection.z;
        this.m_CurrentSubScene.transform.localPosition = localPosition2;
        iTween.MoveTo(this.m_CurrentSubScene, iTween.Hash((object) "islocal", (object) true, (object) "position", (object) localPosition1, (object) "time", (object) num1, (object) "easeType", (object) this.m_TransitionEaseType, (object) "oncomplete", (object) (Action<object>) (e =>
        {
          this.DestroyTransitioningSubScene(delobj);
          this.CompleteTransition();
        }), (object) "oncompletetarget", (object) this.gameObject));
        if (!string.IsNullOrEmpty(this.m_SlideInSound))
          SoundManager.Get().LoadAndPlay((AssetReference) this.m_SlideInSound);
      }
      this.m_TransitionOutSubScene = (GameObject) null;
    }
  }

  private void DestroyTransitioningSubScene(GameObject destroysubscene)
  {
    if (!((UnityEngine.Object) destroysubscene != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) destroysubscene);
  }

  private void CompleteTransition()
  {
    this.m_isTransitioning = false;
    AdventureSubScene component = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      component.NotifyTransitionComplete();
      this.UpdateAdventureModeMusic();
    }
    this.EnableTransitionBlocker(false);
  }

  private IEnumerator WaitForSubSceneToLoad(Action callback = null)
  {
    AdventureSubScene subscene = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
    while (!subscene.IsLoaded())
      yield return (object) null;
    this.DoSubSceneTransition(subscene);
    if (callback != null)
      callback();
  }

  private void OnSelectedModeChanged(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    this.UpdateAdventureModeMusic();
    if (!AdventureConfig.CanPlayMode(adventureId, modeId))
      return;
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventureId, (int) modeId);
    this.SetCurrentTransitionDirection();
    GameSaveKeyId saveDataClientKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey;
    if (saveDataClientKey == ~GameSaveKeyId.INVALID)
      return;
    bool flag = GameSaveDataManager.Get().IsDataReady(saveDataClientKey);
    if (!flag && !this.m_adventuresThatRequestedGameSaveData.Contains(adventureId))
    {
      this.m_adventuresThatRequestedGameSaveData.Add(adventureId);
      GameSaveDataManager.Get().Request(saveDataClientKey, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnRequestGameSaveDataClientResponse_CreateIntroConversation));
    }
    else
    {
      if (!flag)
        return;
      this.OnRequestGameSaveDataClientResponse_CreateIntroConversation(true);
    }
  }

  private void OnRequestGameSaveDataClientResponse_CreateIntroConversation(bool success)
  {
    AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
    AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
    if (!success)
      Log.Adventures.PrintWarning(string.Format("Unable to request game save data key for adventure: {0}.", (object) selectedAdventure));
    AdventureDef adventureDef = this.GetAdventureDef(selectedAdventure);
    if ((UnityEngine.Object) adventureDef == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError(string.Format("Unable to get adventure def for adventure: {0}.", (object) selectedAdventure));
    }
    else
    {
      List<AdventureDef.IntroConversationLine> conversationLines = adventureDef.m_IntroConversationLines;
      bool introOnFirstSeen = adventureDef.m_ShouldOnlyPlayIntroOnFirstSeen;
      AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) selectedAdventure, (int) selectedMode);
      if (adventureDataRecord == null)
      {
        Log.Adventures.PrintError(string.Format("Unable to get adventure data record for adventure = {0}, mode = {1}.", (object) selectedAdventure, (object) selectedMode));
      }
      else
      {
        GameSaveKeyId saveDataClientKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey;
        long num = 0;
        if (saveDataClientKey != GameSaveKeyId.INVALID)
          GameSaveDataManager.Get().GetSubkeyValue(saveDataClientKey, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE, out num);
        if (introOnFirstSeen && num != 0L && !this.IsDevMode)
          return;
        this.OnSelectedModeChanged_CreateIntroConversation(0, conversationLines, saveDataClientKey);
      }
    }
  }

  private void OnSelectedModeChanged_CreateIntroConversation(
    int index,
    List<AdventureDef.IntroConversationLine> convoLines,
    GameSaveKeyId gameSaveClientKey)
  {
    Action<int> finishCallback = (Action<int>) null;
    if (index < convoLines.Count - 1)
      finishCallback = (Action<int>) (groupId =>
      {
        if (SceneMgr.Get() == null || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
          return;
        this.OnSelectedModeChanged_CreateIntroConversation(index + 1, convoLines, gameSaveClientKey);
      });
    bool flag = (UnityEngine.Object) AdventureScene.Get() != (UnityEngine.Object) null && AdventureScene.Get().IsDevMode;
    if (index >= convoLines.Count - 1 && !flag && gameSaveClientKey != ~GameSaveKeyId.INVALID)
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(gameSaveClientKey, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE, new long[1]
      {
        1L
      }));
    if (index >= convoLines.Count)
      return;
    string text = GameStrings.Get(new AssetReference(convoLines[index].VoLinePrefab).GetLegacyAssetName());
    bool allowRepeatDuringSession = flag;
    NotificationManager.Get().CreateCharacterQuote(convoLines[index].CharacterPrefab, NotificationManager.DEFAULT_CHARACTER_POS, text, convoLines[index].VoLinePrefab, allowRepeatDuringSession, finishCallback: finishCallback);
  }

  private void OnAdventureModeChanged(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    if (GameUtils.IsModeHeroic(modeId))
      this.ShowHeroicWarning();
    if (adventureId == AdventureDbId.NAXXRAMAS && !Options.Get().GetBool(Option.HAS_ENTERED_NAXX))
    {
      NotificationManager.Get().CreateKTQuote("VO_KT_INTRO2_40", "VO_KT_INTRO2_40.prefab:5615c7daf91a7ea4e8a4127b70a09682");
      Options.Get().SetBool(Option.HAS_ENTERED_NAXX, true);
    }
    this.UpdateAdventureModeMusic();
  }

  private void OnAdventureMissionChanged(ScenarioDbId mission, bool showDetails) => this.UpdateAdventureModeMusic();

  private void ShowHeroicWarning()
  {
    if (Options.Get().GetBool(Option.HAS_SEEN_HEROIC_WARNING))
      return;
    Options.Get().SetBool(Option.HAS_SEEN_HEROIC_WARNING, true);
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_HEROIC_WARNING_TITLE"),
      m_text = GameStrings.Get("GLUE_HEROIC_WARNING"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  private void ShowExpertAIUnlockTip()
  {
    if (!AchieveManager.Get().HasUnlockedDefaultHeroes() || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && !Options.Get().GetBool(Option.HAS_SEEN_UNLOCK_ALL_HEROES_TRANSITION) || ReturningPlayerMgr.Get().IsInReturningPlayerMode || Options.Get().GetBool(Option.HAS_SEEN_EXPERT_AI_UNLOCK, false) || !UserAttentionManager.CanShowAttentionGrabber("AdventureScene.ShowExpertAIUnlockTip"))
      return;
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_EXPERT_AI_10"), "VO_INNKEEPER_EXPERT_AI_10.prefab:7979b1ca6d60f7b448686a248246542d");
    Options.Get().SetBool(Option.HAS_SEEN_EXPERT_AI_UNLOCK, true);
  }

  private bool OnDevCheat(string func, string[] args, string rawArgs)
  {
    if (!HearthstoneApplication.IsInternal())
      return true;
    this.IsDevMode = true;
    if (args.Length != 0)
    {
      int result = 1;
      if (int.TryParse(args[0], out result))
      {
        if (result > 0)
        {
          this.IsDevMode = true;
          this.DevModeSetting = result;
        }
        else
        {
          this.IsDevMode = false;
          this.DevModeSetting = 0;
        }
      }
    }
    if ((UnityEngine.Object) UIStatus.Get() != (UnityEngine.Object) null)
      UIStatus.Get().AddInfo(string.Format("{0}: IsDevMode={1} DevModeSetting={2}", (object) func, (object) this.IsDevMode, (object) this.DevModeSetting));
    return true;
  }

  private void EnableTransitionBlocker(bool block)
  {
    if (!((UnityEngine.Object) this.m_transitionClickBlocker != (UnityEngine.Object) null))
      return;
    this.m_transitionClickBlocker.SetActive(block);
  }

  private void NotifyAchieveManagerOfAdventureSceneLoaded() => AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_ADVENTURE);

  private void SetCurrentTransitionDirection()
  {
    AdventureDataDbfRecord adventureDataRecord = AdventureConfig.Get().GetSelectedAdventureDataRecord();
    if (adventureDataRecord == null)
    {
      this.m_CurrentTransitionDirection = this.m_TransitionDirection;
    }
    else
    {
      AdventureScene.TransitionDirection transitionDirection = EnumUtils.SafeParse<AdventureScene.TransitionDirection>(adventureDataRecord.SubsceneTransitionDirection, AdventureScene.TransitionDirection.INVALID, true);
      if (transitionDirection != AdventureScene.TransitionDirection.INVALID)
        this.m_CurrentTransitionDirection = transitionDirection;
      else
        this.m_CurrentTransitionDirection = this.m_TransitionDirection;
    }
  }

  public enum TransitionDirection
  {
    INVALID = -1, // 0xFFFFFFFF
    X = 0,
    Y = 1,
    Z = 2,
    NX = 3,
    NY = 4,
    NZ = 5,
  }

  [Serializable]
  public class AdventureModeMusicWingOverride
  {
    public WingDbId m_wingId;
    public MusicPlaylistType m_playlist;
  }

  [Serializable]
  public class AdventureModeMusic
  {
    public AdventureData.Adventuresubscene m_subsceneId;
    public AdventureDbId m_adventureId;
    public MusicPlaylistType m_playlist;
    [CustomEditField(ListSortable = true)]
    public List<AdventureScene.AdventureModeMusicWingOverride> m_wingOverrides;
  }

  [Serializable]
  public class AdventureSubSceneDef
  {
    [CustomEditField(ListSortable = true)]
    public AdventureData.Adventuresubscene m_SubScene;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public String_MobileOverride m_Prefab;
    public bool isWidget;
  }
}
