using Assets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditClass]
public class FiresideGatheringDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "Main Tray Fields")]
  public GameObject m_FiresideGatheringDisplayTray;
  [CustomEditField(Sections = "Main Tray Fields")]
  public GameObject m_TrayContainer;
  [CustomEditField(Sections = "Main Tray Fields")]
  public GameObject m_TavernSignContainer;
  [CustomEditField(Sections = "Main Tray Fields")]
  public Vector3 m_TrayHideOffset;
  [CustomEditField(Sections = "Main Tray Fields")]
  public FiresideGatheringAccordionMenuTray m_AccordionMenuTray;
  [CustomEditField(Sections = "Main Tray Fields", T = EditType.SOUND_PREFAB)]
  public string m_LobbyArriveAudio = "VO_Innkeeper_Male_Dwarf_FSG_LobbyArrive_01.prefab:5071efa83205af742a82b7456b7a6060";
  [CustomEditField(Sections = "Sub Tray Fields")]
  public Vector3_MobileOverride m_TavernBrawlTrayPosition;
  [CustomEditField(Sections = "Sub Tray Fields")]
  public Vector3_MobileOverride m_CollectionManagerTrayPosition;
  [CustomEditField(Sections = "Sub Tray Fields")]
  public Vector3_MobileOverride m_DeckPickerTrayPosition;
  [CustomEditField(Sections = "Main Tray Fields", T = EditType.GAME_OBJECT)]
  public string m_TavernBrawlDisplayPrefab;
  [CustomEditField(Sections = "Opponent Picker Fields")]
  public GameObject m_OpponentPickerTrayContainer;
  [CustomEditField(Sections = "Opponent Picker Fields")]
  public GameObject_MobileOverride m_OpponentPickerTrayPrefab;
  [CustomEditField(Sections = "Opponent Picker Fields")]
  public Vector3 m_OpponentPickerTrayHideOffset;
  [CustomEditField(Sections = "Animation Settings")]
  public float m_trayAnimationTime = 0.5f;
  [CustomEditField(Sections = "Animation Settings")]
  public iTween.EaseType m_trayInEaseType = iTween.EaseType.easeOutBounce;
  [CustomEditField(Sections = "Animation Settings")]
  public iTween.EaseType m_trayOutEaseType = iTween.EaseType.easeOutCubic;
  private static FiresideGatheringDisplay s_instance;
  private FiresideGatheringOpponentPickerTrayDisplay m_opponentPickerTray;
  private Vector3 m_opponentPickerTrayShowPos;
  private DeckPickerTrayDisplay m_deckPickerTray;
  private Vector3 m_trayShowPos;
  private readonly Vector3 m_firesideGatheringDisplayTrayHidePosition = new Vector3(0.0f, 0.0f, -5000f);

  private void Awake()
  {
    FiresideGatheringDisplay.s_instance = this;
    if (FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.NONE || FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.MAIN_SCREEN)
    {
      FiresideGatheringManager.Get().CurrentFiresideGatheringMode = FiresideGatheringManager.FiresideGatheringMode.MAIN_SCREEN;
    }
    else
    {
      PegasusShared.FormatType formatType = Options.GetFormatType();
      this.m_AccordionMenuTray.GoToSpecifiedModeAutomatically(FiresideGatheringManager.Get().CurrentFiresideGatheringMode, formatType);
    }
    this.m_opponentPickerTray = ((GameObject) GameUtils.Instantiate((GameObject) (MobileOverrideValue<GameObject>) this.m_OpponentPickerTrayPrefab, this.m_OpponentPickerTrayContainer)).GetComponent<FiresideGatheringOpponentPickerTrayDisplay>();
    if ((bool) UniversalInputManager.UsePhoneUI)
      LayerUtils.SetLayer((Component) this.m_opponentPickerTray, GameLayer.IgnoreFullScreenEffects);
    this.m_opponentPickerTray.Init();
    this.m_opponentPickerTray.gameObject.SetActive(false);
    this.m_opponentPickerTrayShowPos = this.m_opponentPickerTray.transform.localPosition;
    this.m_opponentPickerTray.transform.localPosition = this.GetOpponentPickerHidePosition();
    this.m_trayShowPos = Vector3.zero;
    this.m_TrayContainer.transform.localPosition = this.GetTrayHidePosition();
    int num = FiresideGatheringManager.Get().ShowSmallSignIfNeeded(this.m_TavernSignContainer.transform) ? 1 : 0;
    Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    if (num == 0)
      FiresideGatheringManager.Get().OnSignClosed += new FiresideGatheringManager.FSGSignClosedCallback(this.OnTavernSignAnimationComplete);
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_TavernBrawl);
    this.SetFiresideGatheringPresenceStatus();
  }

  private void OnDestroy()
  {
    FiresideGatheringDisplay.s_instance = (FiresideGatheringDisplay) null;
    if (FiresideGatheringManager.Get() != null)
      FiresideGatheringManager.Get().OnSignClosed -= new FiresideGatheringManager.FSGSignClosedCallback(this.OnTavernSignAnimationComplete);
    if (!((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null))
      return;
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
  }

  public static FiresideGatheringDisplay Get() => FiresideGatheringDisplay.s_instance;

  public Vector3 GetOpponentPickerShowPosition() => this.m_opponentPickerTrayShowPos;

  public Vector3 GetOpponentPickerHidePosition() => this.m_opponentPickerTrayShowPos + this.m_OpponentPickerTrayHideOffset;

  public Vector3 GetTrayShowPosition() => this.m_trayShowPos;

  public Vector3 GetTrayHidePosition() => this.m_trayShowPos + this.m_TrayHideOffset;

  public void ShowDeckPickerTray() => AssetLoader.Get().InstantiatePrefab((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "DeckPickerTray_phone.prefab:a30124f640b5b92459bf820a4e3b1ca7" : "DeckPickerTray.prefab:3e13b59cdca14074bbce2b7d903ed895"), (PrefabCallback<GameObject>) ((name, go, data) =>
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to load DeckPickerTray.");
    }
    else
    {
      this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
      if ((UnityEngine.Object) this.m_TrayContainer != (UnityEngine.Object) null)
        GameUtils.SetParent((Component) this.m_deckPickerTray, this.m_TrayContainer);
      this.m_deckPickerTray.InitAssets();
      this.m_deckPickerTray.SetHeaderText(GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_TITLE"));
      this.m_deckPickerTray.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_DeckPickerTrayPosition;
      Navigation.RemoveHandler(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack));
      this.StartCoroutine(this.ShowDeckPickerTrayWhenReady());
    }
  }), options: AssetLoadingOptions.IgnorePrefabPosition);

  public void HideDeckPickerTray() => this.HideTray((Action) (() =>
  {
    if (!((UnityEngine.Object) this.m_deckPickerTray != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_deckPickerTray.gameObject != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_deckPickerTray.gameObject);
  }));

  public void ShowTavernBrawlTray()
  {
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnTavernBrawlSceneLoaded));
    SceneManager.LoadSceneAsync("TavernBrawl", LoadSceneMode.Additive);
  }

  public void HideTavernBrawlTray() => this.HideTray((Action) (() =>
  {
    if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() != (UnityEngine.Object) null)
    {
      CollectionManager.Get().GetCollectibleDisplay().Unload();
      UnityEngine.Object.Destroy((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay().gameObject);
    }
    if (!((UnityEngine.Object) TavernBrawlDisplay.Get() != (UnityEngine.Object) null))
      return;
    TavernBrawlDisplay.Get().Unload();
    UnityEngine.Object.Destroy((UnityEngine.Object) TavernBrawlDisplay.Get().gameObject);
  }));

  public void ShowOpponentPickerTray(Action onTrayHiddenListener)
  {
    FiresideGatheringOpponentPickerTrayDisplay.Get().RegisterTrayHiddenListener(onTrayHiddenListener);
    FiresideGatheringOpponentPickerTrayDisplay.Get().Show();
  }

  private void ShowTray()
  {
    iTween.Stop(this.m_TrayContainer);
    this.m_TrayContainer.SetActive(true);
    iTween.MoveTo(this.m_TrayContainer, iTween.Hash((object) "position", (object) this.GetTrayShowPosition(), (object) "isLocal", (object) true, (object) "time", (object) this.m_trayAnimationTime, (object) "easetype", (object) this.m_trayInEaseType, (object) "oncomplete", (object) (Action<object>) (e => this.HideFiresideGatheringDisplayTray(true)), (object) "delay", (object) (1f / 1000f)));
    SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_on.prefab:66491d3d01ed663429ab80daf6a5e880");
  }

  public void HideTray(Action onComplete)
  {
    this.HideFiresideGatheringDisplayTray(false);
    iTween.Stop(this.m_TrayContainer);
    iTween.MoveTo(this.m_TrayContainer, iTween.Hash((object) "position", (object) this.GetTrayHidePosition(), (object) "isLocal", (object) true, (object) "time", (object) this.m_trayAnimationTime, (object) "easetype", (object) this.m_trayOutEaseType, (object) "oncomplete", (object) (Action<object>) (e => onComplete()), (object) "delay", (object) (1f / 1000f)));
    SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_off.prefab:3139d09eb94899d41b9bf612649f47bf");
    this.SetFiresideGatheringPresenceStatus();
  }

  private void HideFiresideGatheringDisplayTray(bool hidden) => this.m_FiresideGatheringDisplayTray.transform.localPosition = hidden ? this.m_firesideGatheringDisplayTrayHidePosition : Vector3.zero;

  private IEnumerator ShowDeckPickerTrayWhenReady()
  {
    while (!NetCache.Get().IsNetObjectAvailable<NetCache.NetCacheDecks>())
      yield return (object) null;
    while (!CollectionManager.Get().AreAllDeckContentsReady())
      yield return (object) null;
    this.ShowTray();
  }

  private void OnTavernBrawlSceneLoaded(
    SceneMgr.Mode mode,
    PegasusScene scene,
    object callbackData)
  {
    if (((object) scene).GetType() != typeof (TavernBrawlScene))
      return;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnTavernBrawlSceneLoaded));
    this.StartCoroutine(this.WaitThenShowTavernBrawlTray());
  }

  private IEnumerator WaitThenShowTavernBrawlTray()
  {
    while ((UnityEngine.Object) TavernBrawlDisplay.Get() == (UnityEngine.Object) null)
      yield return (object) null;
    if ((UnityEngine.Object) this.m_TrayContainer != (UnityEngine.Object) null)
      GameUtils.SetParent((Component) TavernBrawlDisplay.Get(), this.m_TrayContainer);
    TavernBrawlDisplay.Get().transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_TavernBrawlTrayPosition;
    if (TavernBrawlManager.Get().CurrentMission() != null && TavernBrawlManager.Get().CurrentMission().canEditDeck)
    {
      while ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() == (UnityEngine.Object) null)
        yield return (object) null;
      if ((UnityEngine.Object) this.m_TrayContainer != (UnityEngine.Object) null)
        GameUtils.SetParent((Component) CollectionManager.Get().GetCollectibleDisplay(), this.m_TrayContainer);
      CollectionManager.Get().GetCollectibleDisplay().transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_CollectionManagerTrayPosition;
    }
    this.ShowTray();
  }

  private void OnTavernSignAnimationComplete()
  {
    if (FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.NONE)
      return;
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("GLUE_FIRESIDE_GATHERING_INNKEEPER_LOBBY_ARRIVE"), this.m_LobbyArriveAudio);
  }

  private void SetFiresideGatheringPresenceStatus() => PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.HUB);

  private void OnBoxTransitionFinished(object userdata) => FiresideGatheringManager.Get().EnableTransitionInputBlocker(false);
}
