using Assets;
using Hearthstone;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditClass]
public class BasicScene : PegasusScene
{
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_displayPrefab;
  public GameSaveKeyId RequiredGameSaveDataKey;
  public Global.PresenceStatus m_PresenceStatus = Global.PresenceStatus.UNKNOWN;
  protected const float IS_FINISHED_LOADING_TIMEOUT_SECONDS_NETWORK = 15f;
  protected const float IS_FINISHED_LOADING_TIMEOUT_SECONDS = 30f;
  protected bool m_displayPrefabLoaded;
  private bool m_gameSaveDataReceived;
  protected GameObject m_displayRoot;
  protected AbsSceneDisplay m_sceneDisplay;
  protected float m_isFinishedLoadingTimer;

  protected virtual void Start()
  {
    if (this.RequiredGameSaveDataKey != GameSaveKeyId.INVALID)
      GameSaveDataManager.Get().Request(this.RequiredGameSaveDataKey, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnGameSaveDataReceived));
    else
      this.m_gameSaveDataReceived = true;
    this.StartCoroutine(this.NotifySceneLoadedWhenReady());
  }

  private void Update() => Network.Get().ProcessNetwork();

  public override void Unload()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      BnetBar bnetBar = BnetBar.Get();
      if ((UnityEngine.Object) bnetBar != (UnityEngine.Object) null)
        bnetBar.ToggleActive(true);
    }
    if ((UnityEngine.Object) this.m_displayRoot != (UnityEngine.Object) null && (UnityEngine.Object) this.m_displayRoot.gameObject != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_displayRoot.gameObject);
    if (string.IsNullOrWhiteSpace(this.m_sceneName))
      return;
    SceneManager.GetSceneByName(this.m_sceneName);
    if (!SceneManager.GetSceneByName(this.m_sceneName).isLoaded)
      return;
    SceneManager.UnloadSceneAsync(this.m_sceneName);
  }

  public override void ExecuteSceneDrivenTransition(Action onTransitionCompleteCallback)
  {
    if ((UnityEngine.Object) this.m_sceneDisplay == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "BasicScene.ExecuteSceneDrivenTransition() - Null Scene Display.");
      if (onTransitionCompleteCallback == null)
        return;
      onTransitionCompleteCallback();
    }
    else
      this.m_sceneDisplay.ShowSlidingTrayAfterSceneLoad(onTransitionCompleteCallback);
  }

  public override bool IsBlockingPopupDisplayManager() => (UnityEngine.Object) this.m_sceneDisplay != (UnityEngine.Object) null && this.m_sceneDisplay.IsBlockingPopupDisplayManager();

  private void OnGameSaveDataReceived(bool success) => this.m_gameSaveDataReceived = true;

  private void OnDisplayPrefabLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_displayPrefabLoaded = true;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("BasicScene.OnScreenLoaded() - failed to load screen {0}", (object) assetRef));
    else
      this.m_displayRoot = go;
  }

  protected virtual IEnumerator NotifySceneLoadedWhenReady()
  {
    BasicScene basicScene = this;
    while (!basicScene.m_gameSaveDataReceived)
    {
      basicScene.m_isFinishedLoadingTimer += Time.unscaledDeltaTime;
      if ((double) basicScene.m_isFinishedLoadingTimer > 15.0)
      {
        Error.AddFatal(FatalErrorReason.LOAD_SCENE_NETWORK_TIMEOUT, "GLOBAL_ERROR_NETWORK_DISCONNECT");
        yield break;
      }
      else
        yield return (object) null;
    }
    if (basicScene.m_PresenceStatus != Global.PresenceStatus.UNKNOWN)
      PresenceMgr.Get().SetStatus((Enum) basicScene.m_PresenceStatus);
    AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) basicScene.m_displayPrefab, new PrefabCallback<GameObject>(basicScene.OnDisplayPrefabLoaded));
    while (!basicScene.m_displayPrefabLoaded)
    {
      basicScene.m_isFinishedLoadingTimer += Time.unscaledDeltaTime;
      if ((double) basicScene.m_isFinishedLoadingTimer > 30.0)
      {
        basicScene.DisplayFailedToLoadDialog("Display prefab never instantiated.");
        yield break;
      }
      else
        yield return (object) null;
    }
    while ((UnityEngine.Object) basicScene.m_sceneDisplay == (UnityEngine.Object) null)
    {
      basicScene.m_sceneDisplay = basicScene.m_displayRoot.GetComponentInChildren<AbsSceneDisplay>();
      basicScene.m_isFinishedLoadingTimer += Time.unscaledDeltaTime;
      if ((double) basicScene.m_isFinishedLoadingTimer > 30.0)
      {
        basicScene.DisplayFailedToLoadDialog("SceneDisplay script was not found on the display prefab.");
        yield break;
      }
      else
        yield return (object) null;
    }
    basicScene.PassSceneTransitionPayloadToSceneDisplay();
    string failureMessage = string.Empty;
    while (!basicScene.m_sceneDisplay.IsFinishedLoading(out failureMessage) || !basicScene.m_sceneDisplay.IsRootWidgetDoneChangingStates())
    {
      basicScene.m_isFinishedLoadingTimer += Time.unscaledDeltaTime;
      if ((double) basicScene.m_isFinishedLoadingTimer > 30.0)
      {
        basicScene.DisplayFailedToLoadDialog(failureMessage);
        yield break;
      }
      else
        yield return (object) null;
    }
    SceneMgr.Get().NotifySceneLoaded();
  }

  protected void DisplayFailedToLoadDialog(string devFailureMessage)
  {
    if (HearthstoneApplication.IsPublic())
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_SCENE_LOAD_ERROR_TITLE"),
        m_text = GameStrings.Get("GLUE_SCENE_LOAD_ERROR_BODY"),
        m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
        m_showAlertIcon = true,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    else
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_SCENE_LOAD_ERROR_TITLE"),
        m_text = string.Format("{0}\n<color=red>{1}</color>", (object) GameStrings.Get("GLUE_SCENE_LOAD_ERROR_BODY"), (object) devFailureMessage),
        m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
        m_showAlertIcon = true,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
  }

  private void PassSceneTransitionPayloadToSceneDisplay() => this.m_sceneDisplay.SetSceneTransitionPayload(this.m_sceneTransitionPayload);
}
