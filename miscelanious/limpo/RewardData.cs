using System.Collections.Generic;
using UnityEngine;

public abstract class RewardData
{
  private Reward.Type m_type;
  private NetCache.ProfileNotice.NoticeOrigin m_origin = NetCache.ProfileNotice.NoticeOrigin.UNKNOWN;
  private long m_originData;
  protected List<long> m_noticeIDs = new List<long>();
  private bool m_showQuestToast;
  private bool m_isDummyReward;

  public Reward.Type RewardType => this.m_type;

  public NetCache.ProfileNotice.NoticeOrigin Origin => this.m_origin;

  public long OriginData => this.m_originData;

  public bool ShowQuestToast => this.m_showQuestToast;

  public bool IsDummyReward => this.m_isDummyReward;

  public string NameOverride { get; set; }

  public string DescriptionOverride { get; set; }

  public int? RewardChestAssetId { get; set; }

  public int? RewardChestBagNum { get; set; }

  public void LoadRewardObject(Reward.DelOnRewardLoaded callback) => this.LoadRewardObject(callback, (object) null);

  public void LoadRewardObject(Reward.DelOnRewardLoaded callback, object callbackData)
  {
    string assetPath = this.GetAssetPath();
    if (string.IsNullOrEmpty(assetPath))
    {
      Debug.LogError((object) string.Format("Reward.LoadRewardObject(): Do not know how to load reward object for {0}.", (object) this));
    }
    else
    {
      Reward.LoadRewardCallbackData callbackData1 = new Reward.LoadRewardCallbackData()
      {
        m_callback = callback,
        m_callbackData = callbackData
      };
      AssetLoader.Get().InstantiatePrefab((AssetReference) assetPath, new PrefabCallback<GameObject>(this.OnRewardObjectLoaded), (object) callbackData1);
    }
  }

  public void SetOrigin(NetCache.ProfileNotice.NoticeOrigin origin, long originData)
  {
    this.m_origin = origin;
    this.m_originData = originData;
  }

  public void AddNoticeID(long noticeID)
  {
    if (this.m_noticeIDs.Contains(noticeID))
      return;
    this.m_noticeIDs.Add(noticeID);
  }

  public List<long> GetNoticeIDs() => this.m_noticeIDs;

  public bool HasNotices() => this.m_noticeIDs.Count > 0;

  public void AcknowledgeNotices()
  {
    long[] array = this.m_noticeIDs.ToArray();
    this.m_noticeIDs.Clear();
    foreach (long id in array)
      Network.Get().AckNotice(id);
  }

  public void MarkAsDummyReward() => this.m_isDummyReward = true;

  protected RewardData(Reward.Type type, bool showQuestToast = false)
  {
    this.m_type = type;
    this.m_showQuestToast = showQuestToast;
  }

  protected abstract string GetAssetPath();

  private void OnRewardObjectLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      Debug.LogWarning((object) string.Format("Reward.OnRewardObjectLoaded() - game object is null assetRef={0}", (object) assetRef));
    }
    else
    {
      Reward component = go.GetComponent<Reward>();
      if ((Object) component == (Object) null)
      {
        Debug.LogErrorFormat("Reward.OnRewardObjectLoaded() - loaded game object has no reward component assetRef={0}", (object) assetRef);
      }
      else
      {
        go.transform.parent = SceneMgr.Get().SceneObject.transform;
        component.SetData(this, true);
        Reward.LoadRewardCallbackData loadRewardCallbackData = callbackData as Reward.LoadRewardCallbackData;
        component.NotifyLoadedWhenReady(loadRewardCallbackData);
      }
    }
  }
}
