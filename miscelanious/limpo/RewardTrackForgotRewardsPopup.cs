using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RewardTrackForgotRewardsPopup : MonoBehaviour
{
  public UberText m_headerText;
  public UberText m_bodyText;
  private Widget m_widget;
  private readonly string CODE_HIDE = nameof (CODE_HIDE);

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == this.CODE_HIDE))
        return;
      this.m_widget.Hide();
    }));
  }

  public void Show()
  {
    RewardTrackDataModel dataModel = this.m_widget.GetDataModel<RewardTrackDataModel>();
    if (dataModel == null)
    {
      Debug.LogWarning((object) "Unexpected state: no bound RewardTrackDataModel");
    }
    else
    {
      this.m_headerText.Text = GameStrings.FormatPlurals("GLUE_PROGRESSION_REWARD_TRACK_POPUP_FORGOT_REWARDS_TITLE", GameStrings.MakePlurals(dataModel.Unclaimed));
      this.m_bodyText.Text = GameStrings.Format(ProgressUtils.IsEventRewardTrackType(dataModel.RewardTrackType) ? "GLUE_PROGRESSION_EVENT_TAB_POPUP_FORGOT_REWARDS_BODY" : "GLUE_PROGRESSION_REWARD_TRACK_POPUP_FORGOT_REWARDS_BODY", (object) dataModel.Unclaimed, (object) dataModel.Name);
      this.m_widget.Show();
    }
  }
}
