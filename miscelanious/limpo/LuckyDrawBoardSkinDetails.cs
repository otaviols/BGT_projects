using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class LuckyDrawBoardSkinDetails : MonoBehaviour
{
  [SerializeField]
  private AsyncReference m_baconBoardSkinCollectionDetailsReference;
  private BaconBoardCollectionDetails m_baconBoardSkinCollectionDetails;

  private void Start() => this.m_baconBoardSkinCollectionDetailsReference.RegisterReadyListener<BaconBoardCollectionDetails>(new Action<BaconBoardCollectionDetails>(this.OnBoardDetailsReady));

  private void OnBoardDetailsReady(BaconBoardCollectionDetails collectionDetails) => this.m_baconBoardSkinCollectionDetails = collectionDetails;

  private void AssignDataModel(LuckyDrawRewardDataModel boardSkinData)
  {
    if (boardSkinData == null)
      Debug.LogError((object) "Error [LuckyDrawBoardSkinDetails] AssignDataModel boardSkindData was null!");
    else if (boardSkinData.RewardList == null)
      Debug.LogError((object) "Error [LuckyDrawBoardSkinDetails] AssignDataModel rewardList was null!");
    else if (boardSkinData.RewardList.Items == null || boardSkinData.RewardList.Items.Count <= 0)
      Debug.LogError((object) "Error [LuckyDrawBoardSkinDetails] AssignDataModel rewardList items were null or empty!");
    else if (boardSkinData.RewardList.Items[0].BGBoardSkin == null)
    {
      Debug.LogError((object) "Error [LuckyDrawBoardSkinDetails] AssignDataModel rewardList Item was not a boardSkin!");
    }
    else
    {
      if (boardSkinData.RewardList.Items.Count > 1)
        Debug.LogWarning((object) "Warning [LuckyDrawBoardSkinDetails] Only 1 reward item is expected, multiple reward items will not be displayed!");
      this.m_baconBoardSkinCollectionDetails.AssignDataModels((IDataModel) boardSkinData.RewardList.Items[0].BGBoardSkin, (IDataModel) null);
    }
  }

  public bool ShowingRewardGrantVFX { get; set; }

  public void OnShow(IDataModel dataModel)
  {
    this.AssignDataModel(dataModel as LuckyDrawRewardDataModel);
    this.m_baconBoardSkinCollectionDetails.Show();
    if (this.ShowingRewardGrantVFX)
      EventFunctions.TriggerEvent(this.m_baconBoardSkinCollectionDetails.transform.parent, "LUCKY_DRAW_SHOW_REWARD");
    else
      EventFunctions.TriggerEvent(this.m_baconBoardSkinCollectionDetails.transform.parent, "LUCKY_DRAW_SHOW");
  }

  public void OnHide()
  {
    if (!this.m_baconBoardSkinCollectionDetails.CanHide())
      return;
    this.m_baconBoardSkinCollectionDetails.Hide();
  }
}
