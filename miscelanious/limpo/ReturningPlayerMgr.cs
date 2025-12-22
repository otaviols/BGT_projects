using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using PegasusShared;
using PegasusUtil;
using System.Collections.Generic;
using UnityEngine;

public class ReturningPlayerMgr : IService
{
  private ReturningPlayerStatus m_returningPlayerProgress;
  private uint m_abTestGroup;
  private long m_notificationSuppressionTimeSeconds;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (Network)
  };

  public void Shutdown()
  {
  }

  public static ReturningPlayerMgr Get() => ServiceManager.Get<ReturningPlayerMgr>();

  public void SetReturningPlayerInfo(ReturningPlayerInfo info)
  {
    if (info == null)
    {
      Debug.LogError((object) "SetReturningPlayerInfo called with no ReturningPlayerInfo!");
    }
    else
    {
      this.m_returningPlayerProgress = info.Status;
      if (info.HasAbTestGroup)
        this.m_abTestGroup = info.AbTestGroup;
      this.m_notificationSuppressionTimeSeconds = info.NotificationSuppressionTimeDays;
    }
  }

  public bool IsInReturningPlayerMode => this.m_returningPlayerProgress == ReturningPlayerStatus.RPS_ACTIVE || this.m_returningPlayerProgress == ReturningPlayerStatus.RPS_ACTIVE_WITH_MANY_LOSSES;

  public bool SuppressOldPopups => this.IsInReturningPlayerMode;

  public bool ShowReturningPlayerWelcomeBannerIfNeeded(
    ReturningPlayerMgr.WelcomeBannerCloseCallback callback)
  {
    bool flag = false;
    if (this.ShouldShowReturningPlayerWelcomeBanner())
    {
      this.ShowBanner(callback);
      flag = true;
    }
    return flag;
  }

  public bool ShouldShowReturningPlayerWelcomeBanner() => SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN && !this.HasSeenReturningPlayerWelcomeBanner() && this.IsInReturningPlayerMode;

  public bool HasSeenReturningPlayerWelcomeBanner() => GameSaveDataManager.Get() != null && GameUtils.IsGSDFlagSet(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.RETURNING_PLAYER_SEEN_BANNER);

  public void SetSeenReturningPlayerWelcomeBanner()
  {
    if (GameSaveDataManager.Get() == null)
      return;
    GameUtils.SetGSDFlag(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.RETURNING_PLAYER_SEEN_BANNER, true);
  }

  public bool PlayReturningPlayerInnkeeperGreetingIfNecessary()
  {
    if (!this.IsInReturningPlayerMode || Options.Get().GetBool(Option.HAS_HEARD_RETURNING_PLAYER_WELCOME_BACK_VO))
      return false;
    SoundManager.Get().LoadAndPlay((AssetReference) "VO_Innkeeper_Male_Dwarf_ReturningPlayers_01.prefab:cd3f8a594d06834408cb5a119aa33a21");
    Options.Get().SetBool(Option.HAS_HEARD_RETURNING_PLAYER_WELCOME_BACK_VO, true);
    return true;
  }

  public void Cheat_SetReturningPlayerProgress(int progress) => this.m_returningPlayerProgress = (ReturningPlayerStatus) progress;

  public void Cheat_ResetReturningPlayer()
  {
    if (GameSaveDataManager.Get() == null)
      return;
    GameUtils.SetGSDFlag(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.RETURNING_PLAYER_SEEN_BANNER, false);
  }

  private void ShowBanner(
    ReturningPlayerMgr.WelcomeBannerCloseCallback callback)
  {
    BannerManager.Get().ShowBanner("WoodenSign_Paint_Welcome_Back.prefab:4cb64d2b8c67feb45b4e17042d58f1ba", (string) null, GameStrings.Get("GLUE_RETURNING_PLAYER_WELCOME_DESC"), (BannerManager.DelOnCloseBanner) (() => callback()));
    this.SetSeenReturningPlayerWelcomeBanner();
  }

  public delegate void WelcomeBannerCloseCallback();
}
