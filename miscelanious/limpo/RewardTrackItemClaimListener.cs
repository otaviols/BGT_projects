using Assets;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RewardTrackItemClaimListener : MonoBehaviour
{
  public const string CLAIM_INDIVIDUAL_REWARD = "CODE_CLAIM_INDIVIDUAL_REWARD";
  public const string CLAIM_CHOOSE_ONE_REWARD = "CODE_CLAIM_CHOOSE_ONE_REWARD";
  private WidgetTemplate m_widget;

  private void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "CODE_CLAIM_INDIVIDUAL_REWARD"))
      {
        if (!(eventName == "CODE_CLAIM_CHOOSE_ONE_REWARD"))
          return;
        this.ClaimReward(true);
        this.m_widget.TriggerEvent("CLEANUP_POPUP_AFTER_CONFIRM", new Widget.TriggerEventParameters());
      }
      else
        this.ClaimReward(false);
    }));
  }

  private void ClaimReward(bool chooseOne)
  {
    if (!Network.IsLoggedIn())
    {
      ProgressUtils.ShowOfflinePopup();
    }
    else
    {
      RewardTrackNodeRewardsDataModel dataModel = this.m_widget.GetDataModel<RewardTrackNodeRewardsDataModel>();
      if (dataModel == null)
      {
        Debug.LogError((object) "RewardTrackItemClaimListener: Failed to get reward track node rewards data model!");
      }
      else
      {
        int chooseOneRewardItemId = 0;
        if (chooseOne)
        {
          if (!(this.m_widget.GetDataModel<EventDataModel>()?.Payload is RewardItemDataModel payload))
          {
            Debug.LogError((object) "RewardTrackItemClaimListener: failed to get reward item data model from event payload!");
            return;
          }
          chooseOneRewardItemId = payload.AssetId;
        }
        Global.RewardTrackType rewardTrackType = (Global.RewardTrackType) dataModel.RewardTrackType;
        RewardTrackManager.Get().GetRewardTrack(rewardTrackType)?.ClaimReward(dataModel.RewardTrackId, dataModel.Level, dataModel.IsPremium, chooseOneRewardItemId);
      }
    }
  }
}
