using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthyGamingMgr : IService, IHasUpdate
{
  private bool m_BattleNetReady;
  private string m_AccountCountry = string.Empty;
  private Lockouts m_Restrictions;
  private bool m_NetworkDataReady;
  private float m_NextCheckTime;
  private float m_NextMessageDisplayTime;
  private int m_PlayedMinutes;
  private int m_RestedMinutes;
  private ulong m_SessionStartTime;
  private bool m_DebugMode;
  private bool m_HealthyGamingArenaEnabled = true;
  private bool m_UpdateEnabled;

  private float DEBUG_TIMESCALE => Vars.Key("CAIS.DebugTimescale").GetFloat(1f);

  private string DEBUG_TIMESCALE_LOGMSG => (double) this.DEBUG_TIMESCALE != 1.0 ? string.Format("[CAIS.DebugTimescale={0}]", (object) this.DEBUG_TIMESCALE) : string.Empty;

  private float PLAYTIME_CHECK_FREQUENCY_SECONDS => Mathf.Max(1f, this.DEBUG_TIMESCALE * Vars.Key("CAIS.PlayTimeCheckFrequencySeconds").GetFloat(300f));

  private float INITIAL_PLAYTIME_CHECK_FREQUENCY_SECONDS => Mathf.Max(1f, this.DEBUG_TIMESCALE * Vars.Key("CAIS.InitialPlayTimeCheckFrequencySeconds").GetFloat(45f));

  private float CHINA_CAIS_ACTIVE_DISPLAY_TIME_SECONDS => Mathf.Max(1f, this.DEBUG_TIMESCALE * Vars.Key("CAIS.ChinaCAISActiveDisplayTimeSeconds").GetFloat(60f));

  private int CHINA_FEATURES_LOCKOUT_THRESHOLD_MINUTES => Mathf.RoundToInt(this.DEBUG_TIMESCALE * (float) Vars.Key("CAIS.ChinaFeaturesLockoutThresholdMinutes").GetInt(180));

  private int CHINA_FIRST_MESSAGE_THRESHOLD_MINUTES => Mathf.RoundToInt(this.DEBUG_TIMESCALE * (float) Vars.Key("CAIS.ChinaFirstMessageThresholdMinutes").GetInt(60));

  private float CHINA_FIRST_MESSAGE_FREQUENCY_MINUTES => this.DEBUG_TIMESCALE * Vars.Key("CAIS.ChinaFirstMessageFrequencyMinutes").GetFloat(60f);

  public float CHINA_FIRST_MESSAGE_DISPLAY_TIME_SECONDS => Mathf.Clamp(this.DEBUG_TIMESCALE * Vars.Key("CAIS.ChinaFirstMessageDisplayTimeSeconds").GetFloat(60f), 1f, this.CHINA_FIRST_MESSAGE_FREQUENCY_MINUTES * 60f);

  private int CHINA_SECOND_MESSAGE_THRESHOLD_MINUTES => Mathf.RoundToInt(this.DEBUG_TIMESCALE * (float) Vars.Key("CAIS.ChinaSecondMessageThresholdMinutes").GetInt(180));

  private float CHINA_SECOND_MESSAGE_FREQUENCY_MINUTES => this.DEBUG_TIMESCALE * Vars.Key("CAIS.ChinaSecondMessageFrequencyMinutes").GetFloat(30f);

  public float CHINA_SECOND_MESSAGE_DISPLAY_TIME_SECONDS => Mathf.Clamp(this.DEBUG_TIMESCALE * Vars.Key("CAIS.ChinaSecondMessageDisplayTimeSeconds").GetFloat(60f), 1f, this.CHINA_SECOND_MESSAGE_FREQUENCY_MINUTES * 60f);

  private int CHINA_THIRD_MESSAGE_THRESHOLD_MINUTES => Mathf.RoundToInt(this.DEBUG_TIMESCALE * (float) Vars.Key("CAIS.ChinaThirdMessageThresholdMinutes").GetInt(300));

  private float CHINA_THIRD_MESSAGE_FREQUENCY_MINUTES => this.DEBUG_TIMESCALE * Vars.Key("CAIS.ChinaThirdMessageFrequencyMinutes").GetFloat(15f);

  public float CHINA_THIRD_MESSAGE_DISPLAY_TIME_SECONDS => Mathf.Clamp(this.DEBUG_TIMESCALE * Vars.Key("CAIS.ChinaThirdMessageDisplayTimeSeconds").GetFloat(60f), 1f, this.CHINA_THIRD_MESSAGE_FREQUENCY_MINUTES * 60f);

  private float KOREA_MESSAGE_FREQUENCY_MINUTES => this.DEBUG_TIMESCALE * Vars.Key("CAIS.KoreaMessageFrequencyMinutes").GetFloat(60f);

  private float KOREA_MESSAGE_DISPLAY_TIME_SECONDS => Mathf.Clamp(this.DEBUG_TIMESCALE * Vars.Key("CAIS.KoreaMessageDisplayTimeSeconds").GetFloat(60f), 1f, this.KOREA_MESSAGE_FREQUENCY_MINUTES * 60f);

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    HealthyGamingMgr objectRef = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (Options.Get().GetBool(Option.HEALTHY_GAMING_DEBUG, false))
      objectRef.m_DebugMode = true;
    objectRef.m_NextCheckTime = Time.realtimeSinceStartup + objectRef.INITIAL_PLAYTIME_CHECK_FREQUENCY_SECONDS;
    HearthstoneApplication.Get().WillReset += new System.Action(objectRef.WillReset);
    HearthstoneApplication.Get().Resetting += new System.Action(objectRef.OnReset);
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(objectRef.OnFatalError));
    Processor.RunCoroutine(objectRef.InitNetworkData(), (object) objectRef);
    objectRef.m_UpdateEnabled = true;
    return false;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
    HearthstoneApplication.Get().WillReset -= new System.Action(this.WillReset);
    HearthstoneApplication.Get().Resetting -= new System.Action(this.OnReset);
    FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
  }

  public void Update()
  {
    if (!this.m_UpdateEnabled || !this.m_NetworkDataReady || (double) Time.realtimeSinceStartup < (double) this.m_NextCheckTime)
      return;
    this.m_NextCheckTime = Time.realtimeSinceStartup + this.PLAYTIME_CHECK_FREQUENCY_SECONDS;
    string accountCountry = this.m_AccountCountry;
    if (!(accountCountry == "CHN"))
    {
      if (accountCountry == "KOR")
        this.KoreaRestrictions();
      else
        this.m_UpdateEnabled = false;
    }
    else
      this.ChinaRestrictions();
  }

  public static HealthyGamingMgr Get() => ServiceManager.Get<HealthyGamingMgr>();

  public void OnLoggedIn() => this.m_BattleNetReady = true;

  public bool isArenaEnabled() => this.m_HealthyGamingArenaEnabled;

  public ulong GetSessionStartTime() => this.m_SessionStartTime;

  private void WillReset() => this.StopCoroutinesAndResetState();

  private void OnReset() => Processor.RunCoroutine(this.InitNetworkData(), (object) this);

  private void OnFatalError(FatalErrorMessage message, object userData) => this.StopCoroutinesAndResetState();

  private void StopCoroutinesAndResetState()
  {
    this.m_BattleNetReady = false;
    this.m_NetworkDataReady = false;
    Processor.StopAllCoroutinesWithObjectRef((object) this);
  }

  private bool IsInitializationReady => this.m_BattleNetReady && NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>() != null;

  private IEnumerator InitNetworkData()
  {
    if (Network.ShouldBeConnectedToAurora())
    {
      while (!this.IsInitializationReady)
        yield return (object) null;
      this.m_AccountCountry = BattleNet.GetAccountCountry();
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      bool caisEnabled = false;
      if (this.m_AccountCountry == "CHN" || this.m_AccountCountry == "KOR")
      {
        if (PlatformSettings.IsMobile())
        {
          if (this.m_AccountCountry == "CHN")
            caisEnabled = netObject.CaisEnabledMobileChina;
          else if (this.m_AccountCountry == "KOR")
            caisEnabled = netObject.CaisEnabledMobileSouthKorea;
        }
        else
          caisEnabled = netObject.CaisEnabledNonMobile;
      }
      this.m_UpdateEnabled = caisEnabled;
      this.m_Restrictions = new Lockouts();
      BattleNet.GetPlayRestrictions(ref this.m_Restrictions, true);
      while (!this.m_Restrictions.loaded)
      {
        BattleNet.GetPlayRestrictions(ref this.m_Restrictions, false);
        yield return (object) null;
      }
      this.m_SessionStartTime = this.m_Restrictions.sessionStartTime;
      this.m_PlayedMinutes = this.m_Restrictions.CAISplayed;
      this.m_RestedMinutes = this.m_Restrictions.CAISrested;
      if (this.m_DebugMode)
      {
        Debug.LogFormat("[HealthyGaming] Healthy Gaming Debug Logging ON");
        Debug.LogFormat("[HealthyGaming] Account Region: " + BattleNet.GetAccountRegion().ToString());
        Debug.LogFormat("[HealthyGaming] Current Region: " + BattleNet.GetCurrentRegion().ToString());
      }
      Debug.LogFormat("[HealthyGaming] CAIS ServerEnabledForPlatform={0} AccountCAISForChina={1} Country={2} TimeSec={3} PlayedTimeMin={4} RestedTimeMin={5} SessionStartTime={6}", (object) caisEnabled, (object) this.m_Restrictions.CAISactive, (object) this.m_AccountCountry, (object) Time.realtimeSinceStartup, (object) this.m_PlayedMinutes, (object) this.m_RestedMinutes, (object) this.m_SessionStartTime);
      if (!this.m_Restrictions.CAISactive && this.m_AccountCountry == "CHN")
      {
        Debug.LogFormat("[HealthyGaming] Healthy Gaming Deactivated: account not set for CAIS.");
        this.m_UpdateEnabled = false;
      }
      else
      {
        if (caisEnabled)
          Debug.LogFormat("[HealthyGaming] Healthy Gaming Active!");
        if (this.m_AccountCountry == "KOR")
          this.m_NextMessageDisplayTime = Time.realtimeSinceStartup + this.KOREA_MESSAGE_FREQUENCY_MINUTES * 60f;
        if (this.m_AccountCountry == "CHN")
        {
          string key = "GLOBAL_HEALTHY_GAMING_CHINA_CAIS_ACTIVE";
          if ((bool) UniversalInputManager.UsePhoneUI && GameStrings.HasKey(key + "_PHONE"))
            key += "_PHONE";
          string textArg = GameStrings.Get(key);
          SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, textArg, SocialToastMgr.TOAST_TYPE.DEFAULT, this.CHINA_CAIS_ACTIVE_DISPLAY_TIME_SECONDS);
          this.m_NextMessageDisplayTime = -2f;
        }
        this.m_NetworkDataReady = true;
      }
    }
  }

  private void KoreaRestrictions()
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    if (this.m_DebugMode)
      Debug.LogFormat("[HealthyGaming] Minutes Played: " + (object) (float) ((double) realtimeSinceStartup / 60.0));
    if ((double) realtimeSinceStartup < (double) this.m_NextMessageDisplayTime)
      return;
    this.m_NextMessageDisplayTime += this.KOREA_MESSAGE_FREQUENCY_MINUTES * 60f;
    int num = (int) ((double) realtimeSinceStartup / 60.0) / 60;
    SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, GameStrings.Format("GLOBAL_HEALTHY_GAMING_TOAST", (object) num), SocialToastMgr.TOAST_TYPE.DEFAULT, this.KOREA_MESSAGE_DISPLAY_TIME_SECONDS);
  }

  private void ChinaRestrictions()
  {
    BattleNet.GetPlayRestrictions(ref this.m_Restrictions, true);
    Processor.RunCoroutine(this.ChinaRestrictionsUpdate(), (object) this);
  }

  private IEnumerator ChinaRestrictionsUpdate()
  {
    while (!this.m_Restrictions.loaded)
    {
      BattleNet.GetPlayRestrictions(ref this.m_Restrictions, false);
      yield return (object) null;
    }
    this.m_PlayedMinutes = this.m_Restrictions.CAISplayed;
    this.m_RestedMinutes = this.m_Restrictions.CAISrested;
    int minutesPlayed = this.m_PlayedMinutes;
    if (this.m_DebugMode)
    {
      Debug.LogFormat("[HealthyGaming] CAIS Time Played: {0} min    Rested: {1} min", (object) this.m_PlayedMinutes.ToString(), (object) this.m_RestedMinutes.ToString());
      Debug.LogFormat("[HealthyGaming] CAIS Minutes Played: {0} min", (object) minutesPlayed);
    }
    if ((double) this.m_NextMessageDisplayTime == -2.0)
    {
      yield return (object) new WaitForSeconds(this.CHINA_CAIS_ACTIVE_DISPLAY_TIME_SECONDS);
      this.m_NextMessageDisplayTime = -1f;
    }
    if ((double) minutesPlayed >= (double) this.m_NextMessageDisplayTime || (double) this.m_NextMessageDisplayTime <= 0.0)
    {
      if (minutesPlayed >= this.CHINA_FEATURES_LOCKOUT_THRESHOLD_MINUTES)
        this.ChinaRestrictions_LockoutFeatures(minutesPlayed);
      if (minutesPlayed >= this.CHINA_FIRST_MESSAGE_THRESHOLD_MINUTES && minutesPlayed < this.CHINA_SECOND_MESSAGE_THRESHOLD_MINUTES)
        this.ChinaRestrictions_LessThan3Hours(minutesPlayed);
      if (minutesPlayed >= this.CHINA_SECOND_MESSAGE_THRESHOLD_MINUTES && minutesPlayed <= this.CHINA_THIRD_MESSAGE_THRESHOLD_MINUTES)
        this.ChinaRestrictions_3to5Hours(minutesPlayed);
      if (minutesPlayed > this.CHINA_THIRD_MESSAGE_THRESHOLD_MINUTES)
        this.ChinaRestrictions_MoreThan5Hours(minutesPlayed);
    }
  }

  private void ChinaRestrictions_LessThan3Hours(int minutesPlayed)
  {
    this.m_NextMessageDisplayTime = (double) this.m_NextMessageDisplayTime >= 0.0 ? (float) this.m_PlayedMinutes + this.CHINA_FIRST_MESSAGE_FREQUENCY_MINUTES : (float) this.m_PlayedMinutes + (this.CHINA_FIRST_MESSAGE_FREQUENCY_MINUTES - (float) minutesPlayed % this.CHINA_FIRST_MESSAGE_FREQUENCY_MINUTES);
    string key = "GLOBAL_HEALTHY_GAMING_CHINA_LESS_THAN_THREE_HOURS";
    if ((bool) UniversalInputManager.UsePhoneUI && GameStrings.HasKey(key + "_PHONE"))
      key += "_PHONE";
    int num = minutesPlayed / 60;
    string textArg = GameStrings.Format(key, (object) num);
    SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, textArg, SocialToastMgr.TOAST_TYPE.DEFAULT, this.CHINA_FIRST_MESSAGE_DISPLAY_TIME_SECONDS);
    if (this.m_DebugMode)
    {
      Debug.LogFormat("[HealthyGaming] GLOBAL_HEALTHY_GAMING_CHINA_LESS_THAN_THREE_HOURS: {0} minutes {1}", (object) minutesPlayed, (object) this.DEBUG_TIMESCALE_LOGMSG);
      Debug.LogFormat("[HealthyGaming] First message: {0}", (object) GameStrings.Format("GLOBAL_HEALTHY_GAMING_CHINA_LESS_THAN_THREE_HOURS", (object) num));
      Debug.LogFormat("[HealthyGaming] NextMessageDisplayTime: " + this.m_NextMessageDisplayTime.ToString());
    }
    else
      Debug.LogFormat("[HealthyGaming] Time: {0} sec,  Played: {1} min,  First message: {2}", (object) Time.realtimeSinceStartup, (object) minutesPlayed, (object) textArg);
  }

  private void ChinaRestrictions_3to5Hours(int minutesPlayed)
  {
    this.m_NextMessageDisplayTime = (double) this.m_NextMessageDisplayTime >= 0.0 ? (float) this.m_PlayedMinutes + this.CHINA_SECOND_MESSAGE_FREQUENCY_MINUTES : (float) this.m_PlayedMinutes + (this.CHINA_SECOND_MESSAGE_FREQUENCY_MINUTES - (float) minutesPlayed % this.CHINA_SECOND_MESSAGE_FREQUENCY_MINUTES);
    string key = "GLOBAL_HEALTHY_GAMING_CHINA_THREE_TO_FIVE_HOURS";
    if ((bool) UniversalInputManager.UsePhoneUI && GameStrings.HasKey(key + "_PHONE"))
      key += "_PHONE";
    string textArg = GameStrings.Get(key);
    SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, textArg, SocialToastMgr.TOAST_TYPE.DEFAULT, this.CHINA_SECOND_MESSAGE_DISPLAY_TIME_SECONDS);
    if (this.m_DebugMode)
    {
      Debug.LogFormat("[HealthyGaming] GLOBAL_HEALTHY_GAMING_CHINA_THREE_TO_FIVE_HOURS: {0} minutes {1}", (object) minutesPlayed, (object) this.DEBUG_TIMESCALE_LOGMSG);
      Debug.LogFormat("[HealthyGaming] Second message: {0}", (object) GameStrings.Get("GLOBAL_HEALTHY_GAMING_CHINA_THREE_TO_FIVE_HOURS"));
      Debug.LogFormat("[HealthyGaming] NextMessageDisplayTime: " + this.m_NextMessageDisplayTime.ToString());
    }
    else
      Debug.LogFormat("[HealthyGaming] Time: {0} sec,  Played: {1} min,  Second message: {2}", (object) Time.realtimeSinceStartup, (object) minutesPlayed, (object) textArg);
  }

  private void ChinaRestrictions_MoreThan5Hours(int minutesPlayed)
  {
    this.m_NextMessageDisplayTime = (double) this.m_NextMessageDisplayTime >= 0.0 ? (float) this.m_PlayedMinutes + this.CHINA_THIRD_MESSAGE_FREQUENCY_MINUTES : (float) this.m_PlayedMinutes + (this.CHINA_THIRD_MESSAGE_FREQUENCY_MINUTES - (float) minutesPlayed % this.CHINA_THIRD_MESSAGE_FREQUENCY_MINUTES);
    string key = "GLOBAL_HEALTHY_GAMING_CHINA_MORE_THAN_FIVE_HOURS";
    if ((bool) UniversalInputManager.UsePhoneUI && GameStrings.HasKey(key + "_PHONE"))
      key += "_PHONE";
    string textArg = GameStrings.Get(key);
    SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, textArg, SocialToastMgr.TOAST_TYPE.DEFAULT, this.CHINA_THIRD_MESSAGE_DISPLAY_TIME_SECONDS);
    if (this.m_DebugMode)
    {
      Debug.LogFormat("[HealthyGaming] GLOBAL_HEALTHY_GAMING_CHINA_MORE_THAN_FIVE_HOURS: {0} minutes {1}", (object) minutesPlayed, (object) this.DEBUG_TIMESCALE_LOGMSG);
      Debug.LogFormat("[HealthyGaming] Third message: {0}", (object) GameStrings.Get("GLOBAL_HEALTHY_GAMING_CHINA_MORE_THAN_FIVE_HOURS"));
      Debug.LogFormat("[HealthyGaming] NextMessageDisplayTime: " + this.m_NextMessageDisplayTime.ToString());
    }
    else
      Debug.LogFormat("[HealthyGaming] Time: {0} sec,  Played: {1} min,  Third message: {2}", (object) Time.realtimeSinceStartup, (object) minutesPlayed, (object) textArg);
  }

  private void ChinaRestrictions_LockoutFeatures(int minutesPlayed)
  {
    this.m_HealthyGamingArenaEnabled = false;
    Box box = Box.Get();
    if (!((UnityEngine.Object) box != (UnityEngine.Object) null))
      return;
    box.UpdateUI();
  }
}
