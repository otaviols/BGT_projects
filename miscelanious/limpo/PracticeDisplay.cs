using UnityEngine;

public class PracticeDisplay : MonoBehaviour
{
  public GameObject m_deckPickerTrayContainer;
  public GameObject m_practicePickerTrayContainer;
  public GameObject_MobileOverride m_practicePickerTrayPrefab;
  public Vector3_MobileOverride m_practicePickerTrayHideOffset;
  private static PracticeDisplay s_instance;
  private PracticePickerTrayDisplay m_practicePickerTray;
  private Vector3 m_practicePickerTrayShowPos;
  private DeckPickerTrayDisplay m_deckPickerTray;

  private void Awake()
  {
    PracticeDisplay.s_instance = this;
    this.m_practicePickerTray = ((GameObject) GameUtils.Instantiate((GameObject) (MobileOverrideValue<GameObject>) this.m_practicePickerTrayPrefab, this.m_practicePickerTrayContainer)).GetComponent<PracticePickerTrayDisplay>();
    if ((bool) UniversalInputManager.UsePhoneUI)
      LayerUtils.SetLayer((Component) this.m_practicePickerTray, GameLayer.IgnoreFullScreenEffects);
    AssetLoader.Get().InstantiatePrefab((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "DeckPickerTray_phone.prefab:a30124f640b5b92459bf820a4e3b1ca7" : "DeckPickerTray.prefab:3e13b59cdca14074bbce2b7d903ed895"), (PrefabCallback<GameObject>) ((name, go, data) =>
    {
      if ((Object) go == (Object) null)
      {
        Debug.LogError((object) "Unable to load DeckPickerTray.");
      }
      else
      {
        this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
        if ((Object) this.m_deckPickerTray == (Object) null)
        {
          Debug.LogError((object) "DeckPickerTrayDisplay component not found in DeckPickerTray object.");
        }
        else
        {
          if ((Object) this.m_deckPickerTrayContainer != (Object) null)
            GameUtils.SetParent((Component) this.m_deckPickerTray, this.m_deckPickerTrayContainer);
          AdventureSubScene component = this.GetComponent<AdventureSubScene>();
          if ((Object) component != (Object) null)
          {
            this.m_practicePickerTray.AddTrayLoadedListener((PracticePickerTrayDisplay.TrayLoaded) (() =>
            {
              this.OnTrayPartLoaded();
              this.m_practicePickerTray.gameObject.SetActive(false);
            }));
            this.m_deckPickerTray.AddDeckTrayLoadedListener(new AbsDeckPickerTrayDisplay.DeckTrayLoaded(this.OnTrayPartLoaded));
            if (this.m_practicePickerTray.IsLoaded() && this.m_deckPickerTray.IsLoaded())
              component.SetIsLoaded(true);
          }
          this.InitializeTrays();
          CheatMgr.Get().RegisterCheatHandler("replaymissions", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions));
          CheatMgr.Get().RegisterCheatHandler("replaymission", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions));
          NetCache.Get().RegisterScreenPractice(new NetCache.NetCacheCallback(this.OnNetCacheReady));
        }
      }
    }), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnTrayPartLoaded()
  {
    AdventureSubScene component = this.GetComponent<AdventureSubScene>();
    if (!((Object) component != (Object) null))
      return;
    component.SetIsLoaded(this.IsLoaded());
  }

  private void OnDestroy()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if (CheatMgr.Get() != null)
    {
      CheatMgr.Get().UnregisterCheatHandler("replaymissions", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions));
      CheatMgr.Get().UnregisterCheatHandler("replaymission", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions));
    }
    PracticeDisplay.s_instance = (PracticeDisplay) null;
  }

  public static PracticeDisplay Get() => PracticeDisplay.s_instance;

  public bool IsLoaded() => this.m_practicePickerTray.IsLoaded() && this.m_deckPickerTray.IsLoaded();

  private bool OnProcessCheat_replaymissions(string func, string[] args, string rawArgs)
  {
    AssetLoader.Get().InstantiatePrefab((AssetReference) "ReplayTutorialDebug.prefab:895d5f9524722b24582e50484279bba1");
    return true;
  }

  public Vector3 GetPracticePickerShowPosition() => this.m_practicePickerTrayShowPos;

  public Vector3 GetPracticePickerHidePosition() => this.m_practicePickerTrayShowPos + (Vector3) (MobileOverrideValue<Vector3>) this.m_practicePickerTrayHideOffset;

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if (NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.Practice || SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
      return;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_PRACTICE");
  }

  private void InitializeTrays()
  {
    this.m_deckPickerTray.SetHeaderText((string) GameUtils.GetAdventureDataRecord((int) AdventureConfig.Get().GetSelectedAdventure(), (int) AdventureConfig.Get().GetSelectedMode()).Name);
    this.m_deckPickerTray.InitAssets();
    this.m_practicePickerTray.Init();
    this.m_practicePickerTrayShowPos = this.m_practicePickerTray.transform.localPosition;
    this.m_practicePickerTray.transform.localPosition = this.GetPracticePickerHidePosition();
  }
}
