using Blizzard.T5.Core;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TournamentDisplay : MonoBehaviour
{
  public TextMesh m_modeName;
  public Vector3_MobileOverride m_deckPickerPosition;
  public Vector3 m_SetRotationOnscreenPosition = new Vector3(27.051f, 1.7f, -22.4f);
  public Vector3 m_SetRotationOffscreenPosition = new Vector3(-60f, 1.7f, -22.4f);
  public Vector3 m_SetRotationOffscreenDuringTransition = new Vector3(-260f, 1.7f, -22.4f);
  public float m_SetRotationSideInTime = 1f;
  private static TournamentDisplay s_instance;
  private bool m_allInitialized;
  private bool m_netCacheReturned;
  private bool m_deckPickerTrayLoaded;
  private DeckPickerTrayDisplay m_deckPickerTray;
  private GameObject m_deckPickerTrayGO;
  private NetCache.NetCacheMedalInfo m_currentMedalInfo;
  private List<TournamentDisplay.DelMedalChanged> m_medalChangedListeners = new List<TournamentDisplay.DelMedalChanged>();

  private void Awake()
  {
    AssetLoader.Get().InstantiatePrefab((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "DeckPickerTray_phone.prefab:a30124f640b5b92459bf820a4e3b1ca7" : "DeckPickerTray.prefab:3e13b59cdca14074bbce2b7d903ed895"), new PrefabCallback<GameObject>(this.DeckPickerTrayLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    TournamentDisplay.s_instance = this;
  }

  private void OnDestroy()
  {
    TournamentDisplay.s_instance = (TournamentDisplay) null;
    UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
    this.UnregisterListeners();
  }

  private void Start()
  {
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Tournament);
    NetCache.Get().RegisterScreenTourneys(new NetCache.NetCacheCallback(this.UpdateTourneyPage), new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
  }

  private void Update()
  {
    if (this.m_allInitialized || !this.m_netCacheReturned || !this.m_deckPickerTrayLoaded)
      return;
    Log.PlayModeInvestigation.PrintInfo(string.Format("TournamentDisplay.Update() called. m_allInitialized={0}, m_netCacheReturned={1}, m_deckPickerTrayLoaded={2}", (object) this.m_allInitialized, (object) this.m_netCacheReturned, (object) this.m_deckPickerTrayLoaded));
    if (SetRotationManager.Get().ShouldShowSetRotationIntro())
    {
      Log.PlayModeInvestigation.PrintInfo("TournamentDisplay.Update() -- ShouldShowSetRotationIntro() = true");
      this.m_deckPickerTrayGO.transform.localPosition = this.m_SetRotationOffscreenDuringTransition;
      this.SetupSetRotation();
      Log.PlayModeInvestigation.PrintInfo("TournamentDisplay.Update() -- SetupSetRotation() is complete");
    }
    this.m_deckPickerTray.InitAssets();
    this.m_allInitialized = true;
  }

  public void UpdateHeaderText()
  {
    if ((UnityEngine.Object) this.m_deckPickerTray == (UnityEngine.Object) null)
      return;
    if (!Options.GetInRankedPlayMode())
    {
      this.m_deckPickerTray.SetHeaderText(GameStrings.Get("GLUE_PLAY_CASUAL"));
    }
    else
    {
      Map<FormatType, string> map = new Map<FormatType, string>();
      map.Add(FormatType.FT_WILD, "GLUE_PLAY_WILD");
      map.Add(FormatType.FT_STANDARD, "GLUE_PLAY_STANDARD");
      map.Add(FormatType.FT_CLASSIC, "GLUE_PLAY_CLASSIC");
      FormatType formatType = Options.GetFormatType();
      string key;
      string text;
      if (!map.TryGetValue(formatType, out key))
      {
        Debug.LogError((object) ("TournamentDisplay.UpdateHeaderText called in unsupported format type: " + formatType.ToString()));
        text = "UNSUPPORTED HEADER TEXT " + formatType.ToString();
      }
      else
        text = GameStrings.Get(key);
      this.m_deckPickerTray.SetHeaderText(text);
    }
  }

  private void DeckPickerTrayLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_deckPickerTrayGO = go;
    this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
    this.m_deckPickerTray.transform.parent = this.transform;
    this.m_deckPickerTray.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_deckPickerPosition;
    this.m_deckPickerTrayLoaded = true;
    this.UpdateHeaderText();
  }

  public bool SlidingInForSetRotation { get; private set; }

  public void SetRotationSlideIn()
  {
    this.SlidingInForSetRotation = true;
    this.m_deckPickerTrayGO.transform.localPosition = this.m_SetRotationOffscreenPosition;
    iTween.MoveTo(this.m_deckPickerTrayGO, iTween.Hash((object) "position", (object) this.m_SetRotationOnscreenPosition, (object) "delay", (object) 0.0f, (object) "time", (object) this.m_SetRotationSideInTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce, (object) "oncomplete", (object) (Action<object>) (o => this.SlidingInForSetRotation = false)));
  }

  private void UpdateTourneyPage()
  {
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.Tournament)
    {
      if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        return;
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_PLAY");
    }
    else
    {
      NetCache.NetCacheMedalInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>();
      bool flag = false;
      if (this.m_currentMedalInfo != null)
      {
        foreach (FormatType formatType in Enum.GetValues(typeof (FormatType)))
        {
          if (formatType != FormatType.FT_UNKNOWN)
          {
            MedalInfoData medalInfoData1 = netObject.GetMedalInfoData(formatType);
            MedalInfoData medalInfoData2 = this.m_currentMedalInfo.GetMedalInfoData(formatType);
            if (medalInfoData1 == null || medalInfoData2 == null || medalInfoData1.LeagueId != medalInfoData2.LeagueId || medalInfoData1.StarLevel != medalInfoData2.StarLevel || medalInfoData1.Stars != medalInfoData2.Stars || medalInfoData1.StarsPerWin != medalInfoData2.StarsPerWin)
            {
              flag = true;
              break;
            }
          }
        }
      }
      this.m_currentMedalInfo = netObject;
      if (flag)
      {
        foreach (TournamentDisplay.DelMedalChanged delMedalChanged in this.m_medalChangedListeners.ToArray())
          delMedalChanged(this.m_currentMedalInfo);
      }
      this.m_netCacheReturned = true;
    }
  }

  private void UnregisterListeners()
  {
    if (NetCache.Get() == null)
      return;
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.UpdateTourneyPage));
  }

  public static TournamentDisplay Get() => TournamentDisplay.s_instance;

  public void SceneUnload() => this.UnregisterListeners();

  public NetCache.NetCacheMedalInfo GetCurrentMedalInfo() => this.m_currentMedalInfo;

  public void RegisterMedalChangedListener(TournamentDisplay.DelMedalChanged listener)
  {
    if (this.m_medalChangedListeners.Contains(listener))
      return;
    this.m_medalChangedListeners.Add(listener);
  }

  public void RemoveMedalChangedListener(TournamentDisplay.DelMedalChanged listener) => this.m_medalChangedListeners.Remove(listener);

  private void SetupSetRotation() => AssetLoader.Get().InstantiatePrefab((AssetReference) "TheBox_TheClock.prefab:d922114c10efb5e4d8ab76d57913eff3");

  public delegate void DelMedalChanged(NetCache.NetCacheMedalInfo medalInfo);
}
