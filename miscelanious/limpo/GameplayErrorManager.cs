using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class GameplayErrorManager : IService
{
  private static GameplayErrorCloud s_messageInstance;
  private GUIStyle m_errorDisplayStyle;
  private string m_message;
  private float m_displaySecsLeft;
  private UberText m_uberText;

  private GameplayErrorManagerData Data { get; set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    GameplayErrorManager gameplayErrorManager = this;
    LoadResource loadData;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      gameplayErrorManager.Data = loadData.LoadedAsset as GameplayErrorManagerData;
      serviceLocator.Get<SceneMgr>().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(gameplayErrorManager.OnPreUnload));
      gameplayErrorManager.m_message = "";
      gameplayErrorManager.m_errorDisplayStyle = new GUIStyle();
      gameplayErrorManager.m_errorDisplayStyle.fontSize = 24;
      gameplayErrorManager.m_errorDisplayStyle.fontStyle = FontStyle.Bold;
      gameplayErrorManager.m_errorDisplayStyle.alignment = TextAnchor.UpperCenter;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    loadData = new LoadResource("ServiceData/GameplayErrorManagerData", LoadResourceFlags.FailOnError);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (IAsyncJobResult) loadData;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (SceneMgr)
  };

  public void Shutdown()
  {
  }

  public static GameplayErrorManager Get() => ServiceManager.Get<GameplayErrorManager>();

  private void OnPreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData) => this.HideMessage();

  private void LoadUbertextIfNeeded()
  {
    if (!((UnityEngine.Object) GameplayErrorManager.s_messageInstance == (UnityEngine.Object) null) && !((UnityEngine.Object) this.m_uberText == (UnityEngine.Object) null))
      return;
    GameplayErrorManager.s_messageInstance = UnityEngine.Object.Instantiate<GameplayErrorCloud>(this.Data.m_errorMessagePrefab);
    if ((UnityEngine.Object) GameplayErrorManager.s_messageInstance.GetComponent<HSDontDestroyOnLoad>() == (UnityEngine.Object) null)
      GameplayErrorManager.s_messageInstance.gameObject.AddComponent<HSDontDestroyOnLoad>();
    this.m_uberText = GameplayErrorManager.s_messageInstance.gameObject.GetComponentInChildren<UberText>(true);
  }

  public void DisplayMessage(string message)
  {
    this.LoadUbertextIfNeeded();
    this.m_message = message;
    this.m_displaySecsLeft = (float) message.Length * 0.1f;
    if (CollectionManager.Get().IsInEditMode() || CollectionManager.Get().IsInEditTeamMode())
      GameplayErrorManager.s_messageInstance.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_messagePositionInCollectionManager;
    else
      GameplayErrorManager.s_messageInstance.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_messagePositionInGame;
    this.m_uberText.gameObject.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.Data.m_mobileTextAdjustment;
    GameplayErrorManager.s_messageInstance.ShowMessage(this.m_message, this.m_displaySecsLeft);
    SoundManager.Get().LoadAndPlay((AssetReference) "UI_no_can_do.prefab:7b1a22774f818544387c0f2ca4fea02c");
  }

  private void HideMessage()
  {
    if (!((UnityEngine.Object) GameplayErrorManager.s_messageInstance != (UnityEngine.Object) null))
      return;
    this.LoadUbertextIfNeeded();
    GameplayErrorManager.s_messageInstance.Hide();
  }
}
