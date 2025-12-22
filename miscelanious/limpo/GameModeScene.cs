using Assets;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class GameModeScene : PegasusScene
{
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_screenPrefab;
  private bool m_screenPrefabLoaded;
  private bool m_gameSaveDataReceived;
  private GameModeDisplay m_gameModeDisplay;
  private GameObject m_gameModeDisplayRoot;

  private void Start()
  {
    GameSaveDataManager.Get().Request(GameSaveKeyId.GAME_MODE_SCENE, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnGameSaveDataReceived));
    this.StartCoroutine(this.NotifySceneLoadedWhenReady());
  }

  private void Update() => Network.Get().ProcessNetwork();

  public override bool IsUnloading() => false;

  public override void Unload()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      BnetBar bnetBar = BnetBar.Get();
      if ((UnityEngine.Object) bnetBar != (UnityEngine.Object) null)
        bnetBar.ToggleActive(true);
    }
    if (!((UnityEngine.Object) this.m_gameModeDisplayRoot != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_gameModeDisplayRoot.gameObject != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_gameModeDisplayRoot.gameObject);
  }

  public override void ExecuteSceneDrivenTransition(Action onTransitionCompleteCallback) => this.m_gameModeDisplay.ShowSlidingTrayAfterSceneLoad(onTransitionCompleteCallback);

  private void OnScreenPrefabLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_screenPrefabLoaded = true;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("GameModeScene.OnScreenLoaded() - failed to load screen {0}", (object) assetRef));
    else
      this.m_gameModeDisplayRoot = go;
  }

  private void OnGameSaveDataReceived(bool success) => this.m_gameSaveDataReceived = true;

  private IEnumerator NotifySceneLoadedWhenReady()
  {
    GameModeScene gameModeScene = this;
    while (!gameModeScene.m_gameSaveDataReceived)
      yield return (object) null;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.HUB);
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) gameModeScene.m_screenPrefab, new PrefabCallback<GameObject>(gameModeScene.OnScreenPrefabLoaded));
    while (!gameModeScene.m_screenPrefabLoaded)
      yield return (object) null;
    while ((UnityEngine.Object) gameModeScene.m_gameModeDisplayRoot == (UnityEngine.Object) null)
      yield return (object) null;
    while ((UnityEngine.Object) gameModeScene.m_gameModeDisplayRoot.GetComponentInChildren<GameModeDisplay>() == (UnityEngine.Object) null)
      yield return (object) null;
    gameModeScene.m_gameModeDisplay = gameModeScene.m_gameModeDisplayRoot.GetComponentInChildren<GameModeDisplay>();
    while (!gameModeScene.m_gameModeDisplay.IsFinishedLoading)
      yield return (object) null;
    if (GameModeUtils.ShouldSeeSoloAdventuresMovedPopup())
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.FTUE_SHOULD_SEE_SOLO_ADVENTURES_MOVED_POPUP, new long[1]));
    SceneMgr.Get().NotifySceneLoaded();
  }
}
