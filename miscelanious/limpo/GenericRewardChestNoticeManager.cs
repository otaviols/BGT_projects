using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

public class GenericRewardChestNoticeManager : IService
{
  private Dictionary<int, GenericRewardChestNoticeManager.GenericRewardChestAssetStatus> m_mapOfRewardChestAssetIdToNoticeIds = new Dictionary<int, GenericRewardChestNoticeManager.GenericRewardChestAssetStatus>();
  private List<GenericRewardChestNoticeManager.GenericRewardChestUpdatedListener> m_genericRewardUpdatedListeners = new List<GenericRewardChestNoticeManager.GenericRewardChestUpdatedListener>();

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    GenericRewardChestNoticeManager chestNoticeManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    serviceLocator.Get<NetCache>().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(chestNoticeManager.OnNewNotices));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (NetCache)
  };

  public void Shutdown()
  {
  }

  public static GenericRewardChestNoticeManager Get() => ServiceManager.Get<GenericRewardChestNoticeManager>();

  public HashSet<long> GetReadyGenericRewardChestNotices()
  {
    HashSet<long> rewardChestNotices = new HashSet<long>();
    foreach (GenericRewardChestNoticeManager.GenericRewardChestAssetStatus chestAssetStatus in this.m_mapOfRewardChestAssetIdToNoticeIds.Values)
    {
      if (chestAssetStatus.m_isReady)
        rewardChestNotices.UnionWith((IEnumerable<long>) chestAssetStatus.m_noticeIds);
    }
    return rewardChestNotices;
  }

  public bool RegisterRewardsUpdatedListener(
    GenericRewardChestNoticeManager.GenericRewardUpdatedCallback callback,
    object userData = null)
  {
    if (callback == null)
      return false;
    GenericRewardChestNoticeManager.GenericRewardChestUpdatedListener chestUpdatedListener = new GenericRewardChestNoticeManager.GenericRewardChestUpdatedListener();
    chestUpdatedListener.SetCallback(callback);
    chestUpdatedListener.SetUserData(userData);
    if (this.m_genericRewardUpdatedListeners.Contains(chestUpdatedListener))
      return false;
    this.m_genericRewardUpdatedListeners.Add(chestUpdatedListener);
    return true;
  }

  public bool RemoveRewardsUpdatedListener(
    GenericRewardChestNoticeManager.GenericRewardUpdatedCallback callback)
  {
    return this.RemoveRewardsUpdatedListener(callback, (object) null);
  }

  public bool RemoveRewardsUpdatedListener(
    GenericRewardChestNoticeManager.GenericRewardUpdatedCallback callback,
    object userData)
  {
    if (callback == null)
      return false;
    GenericRewardChestNoticeManager.GenericRewardChestUpdatedListener chestUpdatedListener = new GenericRewardChestNoticeManager.GenericRewardChestUpdatedListener();
    chestUpdatedListener.SetCallback(callback);
    chestUpdatedListener.SetUserData(userData);
    if (!this.m_genericRewardUpdatedListeners.Contains(chestUpdatedListener))
      return false;
    this.m_genericRewardUpdatedListeners.Remove(chestUpdatedListener);
    return true;
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    if (NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() == null)
      return;
    bool flag = false;
    foreach (NetCache.ProfileNotice newNotice in newNotices)
    {
      if (NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST == newNotice.Type)
      {
        if (!(newNotice is NetCache.ProfileNoticeGenericRewardChest genericRewardChest) || genericRewardChest.RewardChestHash == null || genericRewardChest.RewardChestByteSize == 0U)
        {
          Debug.LogError((object) string.Format("ProfileNoticeGenericRewardChest with asset id [{0}] with no hash or a byte size of 0. Unable to request reward chest record information.", (object) genericRewardChest.RewardChestAssetId));
          if (GameDbf.RewardChest.HasRecord(genericRewardChest.RewardChestAssetId))
          {
            Debug.LogWarning((object) string.Format("Local RewardChest record found for asset id {0}. Using cached value.", (object) genericRewardChest.RewardChestAssetId));
            this.InformListenersThatNoticeIsReady(newNotice.NoticeID);
          }
        }
        else
        {
          AssetRecordInfo assetRecordInfo = new AssetRecordInfo()
          {
            Asset = new AssetKey()
          };
          assetRecordInfo.Asset.Type = AssetType.ASSET_TYPE_REWARD_CHEST;
          assetRecordInfo.Asset.AssetId = genericRewardChest.RewardChestAssetId;
          assetRecordInfo.RecordByteSize = genericRewardChest.RewardChestByteSize;
          assetRecordInfo.RecordHash = genericRewardChest.RewardChestHash;
          if (!this.m_mapOfRewardChestAssetIdToNoticeIds.ContainsKey(genericRewardChest.RewardChestAssetId))
            this.m_mapOfRewardChestAssetIdToNoticeIds[genericRewardChest.RewardChestAssetId] = new GenericRewardChestNoticeManager.GenericRewardChestAssetStatus();
          this.m_mapOfRewardChestAssetIdToNoticeIds[genericRewardChest.RewardChestAssetId].m_noticeIds.Add(newNotice.NoticeID);
          if (!flag)
            flag = DownloadableDbfCache.Get().IsAssetRequestInProgress(genericRewardChest.RewardChestAssetId, AssetType.ASSET_TYPE_REWARD_CHEST);
          DownloadableDbfCache.Get().LoadCachedAssets((!flag ? 1 : 0) != 0, new DownloadableDbfCache.LoadCachedAssetCallback(this.OnRewardChestDownloadableDbfAssetsLoaded), assetRecordInfo);
        }
      }
    }
  }

  private void OnRewardChestDownloadableDbfAssetsLoaded(
    AssetKey requestedKey,
    PegasusShared.ErrorCode code,
    byte[] assetBytes)
  {
    if (code != PegasusShared.ErrorCode.ERROR_OK)
    {
      Debug.LogError((object) string.Format("Unable to get reward chest asset information for Reward Chest ID: {0}, ErrorCode: {1}", (object) requestedKey.AssetId, (object) code));
    }
    else
    {
      GenericRewardChestNoticeManager.GenericRewardChestAssetStatus assetIdToNoticeId = this.m_mapOfRewardChestAssetIdToNoticeIds[requestedKey.AssetId];
      assetIdToNoticeId.m_isReady = true;
      foreach (long noticeId in assetIdToNoticeId.m_noticeIds)
        this.InformListenersThatNoticeIsReady(noticeId);
    }
  }

  private void InformListenersThatNoticeIsReady(long noticeId)
  {
    foreach (GenericRewardChestNoticeManager.GenericRewardChestUpdatedListener chestUpdatedListener in this.m_genericRewardUpdatedListeners.ToArray())
      chestUpdatedListener.Fire(noticeId);
  }

  private class GenericRewardChestAssetStatus
  {
    public bool m_isReady;
    public HashSet<long> m_noticeIds = new HashSet<long>();
  }

  public delegate void GenericRewardUpdatedCallback(long receivedRewardNoticeIds, object userData);

  private class GenericRewardChestUpdatedListener : 
    EventListener<GenericRewardChestNoticeManager.GenericRewardUpdatedCallback>
  {
    public void Fire(long receivedRewardNoticeIds) => this.m_callback(receivedRewardNoticeIds, this.m_userData);
  }
}
