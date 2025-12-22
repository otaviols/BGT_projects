using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class LuckyDrawFinisherDetails : MonoBehaviour
{
  [SerializeField]
  private AsyncReference m_baconFinisherCollectionDetailsReference;
  private BaconFinisherCollectionDetails m_baconFinisherCollectionDetails;

  private void Start() => this.m_baconFinisherCollectionDetailsReference.RegisterReadyListener<BaconFinisherCollectionDetails>(new Action<BaconFinisherCollectionDetails>(this.OnFinisherDetailsDisplayReady));

  private void OnFinisherDetailsDisplayReady(BaconFinisherCollectionDetails collectionDetails) => this.m_baconFinisherCollectionDetails = collectionDetails;

  private void AssignDataModel(LuckyDrawRewardDataModel finisherData)
  {
    if (finisherData == null)
      Debug.LogError((object) "Error [LuckyDrawFinisherDetails] AssignDataModel finisherData was null!");
    else if (finisherData.RewardList == null)
      Debug.LogError((object) "Error [LuckyDrawFinisherDetails] AssignDataModel rewardList was null!");
    else if (finisherData.RewardList.Items == null || finisherData.RewardList.Items.Count <= 0)
      Debug.LogError((object) "Error [LuckyDrawFinisherDetails] AssignDataModel rewardList items were null or empty!");
    else if (finisherData.RewardList.Items[0].BGFinisher == null)
      Debug.LogError((object) "Error [LuckyDrawFinisherDetails] AssignDataModel rewardList Item was not a finisher!");
    else
      this.m_baconFinisherCollectionDetails.AssignDataModels((IDataModel) finisherData.RewardList.Items[0].BGFinisher, (IDataModel) null);
  }

  public bool ShowingRewardGrantVFX { get; set; }

  public void OnShow(IDataModel dataModel)
  {
    this.AssignDataModel(dataModel as LuckyDrawRewardDataModel);
    this.m_baconFinisherCollectionDetails.Show();
    if (this.ShowingRewardGrantVFX)
      EventFunctions.TriggerEvent(this.m_baconFinisherCollectionDetails.transform.parent, "LUCKY_DRAW_SHOW_REWARD");
    else
      EventFunctions.TriggerEvent(this.m_baconFinisherCollectionDetails.transform.parent, "LUCKY_DRAW_SHOW");
  }

  public void OnHide()
  {
    if (!this.m_baconFinisherCollectionDetails.CanHide())
      return;
    this.m_baconFinisherCollectionDetails.Hide();
  }
}
