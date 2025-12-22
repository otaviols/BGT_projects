using Blizzard.T5.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BannerManager
{
  private static BannerManager s_instance;
  private bool m_bannerWasAcknowledged;
  private List<int> m_seenBanners = new List<int>();
  private bool m_isShowing;
  private VarKey m_bannerIdOverride = new VarKey("Events.BannerIdOverride");

  public static BannerManager Get()
  {
    if (BannerManager.s_instance == null)
      BannerManager.s_instance = new BannerManager();
    return BannerManager.s_instance;
  }

  public bool IsShowing => this.m_isShowing;

  private int GetOutstandingDisplayBannerId()
  {
    int outstandingDisplayBannerId = this.m_bannerIdOverride.GetInt(0);
    if (outstandingDisplayBannerId != 0)
      return outstandingDisplayBannerId;
    NetCache.NetCacheDisplayBanner netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDisplayBanner>();
    return netObject != null ? netObject.Id : 0;
  }

  private bool AcknowledgeBanner(int banner)
  {
    this.m_seenBanners.Add(banner);
    if (banner != this.GetOutstandingDisplayBannerId() || this.m_bannerWasAcknowledged)
      return false;
    this.m_bannerWasAcknowledged = true;
    NetCache.NetCacheDisplayBanner netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDisplayBanner>();
    if (netObject != null)
    {
      netObject.Id = banner;
      NetCache.Get().NetCacheChanged<NetCache.NetCacheDisplayBanner>();
    }
    Network.Get().AcknowledgeBanner(banner);
    return true;
  }

  public void AutoAcknowledgeOutstandingBanner()
  {
    int outstandingDisplayBannerId = this.GetOutstandingDisplayBannerId();
    if (outstandingDisplayBannerId == 0)
      return;
    this.AcknowledgeBanner(outstandingDisplayBannerId);
  }

  public bool ShowOutstandingBannerEvent(BannerManager.DelOnCloseBanner callback = null)
  {
    int outstandingDisplayBannerId = this.GetOutstandingDisplayBannerId();
    if (outstandingDisplayBannerId == 0 || !Options.Get().GetBool(Option.HAS_SEEN_HUB, false) || this.m_seenBanners.Contains(outstandingDisplayBannerId))
      return false;
    if (ReturningPlayerMgr.Get().IsInReturningPlayerMode)
    {
      this.AcknowledgeBanner(outstandingDisplayBannerId);
      return false;
    }
    if (!this.ShowBanner(outstandingDisplayBannerId, callback))
      return false;
    this.AcknowledgeBanner(outstandingDisplayBannerId);
    return true;
  }

  public bool ShowBanner(
    string prefabAssetPath,
    string headerText,
    string text,
    BannerManager.DelOnCloseBanner callback = null,
    Action<BannerPopup> onCreateCallback = null)
  {
    BannerPopup bannerPopup = GameUtils.LoadGameObjectWithComponent<BannerPopup>(prefabAssetPath);
    if ((UnityEngine.Object) bannerPopup == (UnityEngine.Object) null)
      return false;
    if (onCreateCallback != null)
      onCreateCallback(bannerPopup);
    this.m_isShowing = true;
    bannerPopup.Show(headerText, text, (BannerManager.DelOnCloseBanner) (() =>
    {
      this.OnBannerClose();
      if (callback == null)
        return;
      callback();
    }));
    return true;
  }

  public bool ShowBanner(int bannerID, BannerManager.DelOnCloseBanner callback = null)
  {
    if (bannerID == 0)
      return false;
    BannerDbfRecord record = GameDbf.Banner.GetRecord(bannerID);
    string prefabAssetPath = record == null ? (string) null : record.Prefab;
    if (record != null && prefabAssetPath != null)
      return this.ShowBanner(prefabAssetPath, (string) record.HeaderText, (string) record.Text, callback);
    Debug.LogWarning((object) string.Format("No banner defined for bannerID={0}", (object) bannerID));
    return false;
  }

  public void Cheat_ClearSeenBannersNewerThan(int bannerId) => this.m_seenBanners.RemoveAll((Predicate<int>) (i => i >= bannerId));

  public void Cheat_ClearSeenBanners() => this.m_seenBanners.Clear();

  private BannerManager()
  {
  }

  private void OnBannerClose() => this.m_isShowing = false;

  public delegate void DelOnCloseBanner();
}
