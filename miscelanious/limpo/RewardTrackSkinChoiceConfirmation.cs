using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RewardTrackSkinChoiceConfirmation : MonoBehaviour
{
  public const string CLAIM_CLICKED = "CODE_CLAIM_CLICKED";
  private WidgetTemplate m_widget;

  private void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "CODE_CLAIM_CLICKED"))
        return;
      this.ClaimClicked();
    }));
  }

  public void ClaimClicked()
  {
    if (!(this.m_widget.GetDataModel<EventDataModel>()?.Payload is RewardItemDataModel payload))
    {
      Debug.LogError((object) "RewardTrackSkinChoiceConfirmation: failed to get reward item data model from event payload!");
    }
    else
    {
      RewardItemDbfRecord record = GameDbf.RewardItem.GetRecord(payload.AssetId);
      string className = GameStrings.GetClassName(GameUtils.GetTagClassFromCardDbId(record.Card));
      this.m_widget.TriggerEvent("HIDE_POPUP_FOR_CONFIRM", new Widget.TriggerEventParameters());
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_PROGRESSION_REWARD_TRACK_POPUP_SKIN_CHOICE_CONFIRMATION_HEADER"),
        m_text = GameStrings.Format("GLUE_PROGRESSION_REWARD_TRACK_POPUP_SKIN_CHOICE_CONFIRMATION_TEXT", (object) className, (object) record.CardRecord.Name.GetString()),
        m_showAlertIcon = false,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response == AlertPopup.Response.CONFIRM)
            this.m_widget.TriggerEvent("CLAIM_CHOOSE_ONE_REWARD", new Widget.TriggerEventParameters());
          else
            this.m_widget.TriggerEvent("SHOW_POPUP_AFTER_CONFIRM", new Widget.TriggerEventParameters());
        })
      };
      DialogManager.Get().ShowPopup(info);
    }
  }
}
