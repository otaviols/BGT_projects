using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RewardChooseOneItemClaimListener : MonoBehaviour
{
  public const string CLAIM_CHOOSE_ONE_REWARD = "CODE_CLAIM_CHOOSE_ONE_REWARD";
  private WidgetTemplate m_widget;

  private void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "CODE_CLAIM_CHOOSE_ONE_REWARD"))
        return;
      this.OnClaimChooseOneReward();
    }));
  }

  private void OnClaimChooseOneReward()
  {
    if (!Network.IsLoggedIn())
      ProgressUtils.ShowOfflinePopup();
    else if (!(this.m_widget.GetDataModel<EventDataModel>()?.Payload is RewardItemDataModel payload))
    {
      Debug.LogError((object) "RewardTrackItemClaimListener: failed to get reward item data model from event payload.");
    }
    else
    {
      AchievementDataModel dataModel = this.m_widget.GetDataModel<AchievementDataModel>();
      if (dataModel == null)
      {
        Debug.LogError((object) "RewardTrackItemClaimListener: failed to get achievement data model from widget.");
      }
      else
      {
        AchievementManager.Get().ClaimAchievementReward(dataModel.ID, payload.AssetId);
        this.m_widget.TriggerEvent("CLEANUP_POPUP_AFTER_CONFIRM", new Widget.TriggerEventParameters());
      }
    }
  }
}
