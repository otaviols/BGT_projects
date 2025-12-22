using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class EventEndedPopup : MonoBehaviour
{
  [SerializeField]
  private UberText m_messageBody;
  public static readonly AssetReference EVENT_ENDED_POPUP_PREFAB = new AssetReference("EventEndedPopup.prefab:2e21ebc3432a3044294370e100cbf81a");
  private const string CODE_DISMISS = "CODE_DISMISS";
  private Widget m_widget;
  private GameObject m_owner;
  private Action m_callback;
  private EventDetailsDataModel m_eventDetails;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "CODE_DISMISS"))
        return;
      this.Hide();
    }));
    this.m_owner = this.gameObject;
    if (!((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null) || !((UnityEngine.Object) this.transform.parent.GetComponent<WidgetInstance>() != (UnityEngine.Object) null))
      return;
    this.m_owner = this.transform.parent.gameObject;
  }

  private void OnDestroy()
  {
    UIContext.GetRoot().DismissPopup(this.m_owner);
    Action callback = this.m_callback;
    if (callback == null)
      return;
    callback();
  }

  public void Initialize(Action callback, EventDetailsDataModel eventDetails)
  {
    this.m_callback = callback;
    if (eventDetails == null)
    {
      Debug.LogError((object) "EventEndedPopup initialized without an Event.");
    }
    else
    {
      this.m_eventDetails = eventDetails;
      this.m_widget.BindDataModel((IDataModel) this.m_eventDetails);
    }
  }

  public void Show()
  {
    if ((UnityEngine.Object) this.m_messageBody != (UnityEngine.Object) null && this.m_eventDetails != null)
      this.m_messageBody.Text = GameStrings.Format("GLUE_PROGRESSION_EVENT_TAB_POPUP_EXPIRED_BODY", (object) this.m_eventDetails.Name);
    OverlayUI.Get().AddGameObject(this.transform.parent.gameObject);
    this.m_widget.RegisterDoneChangingStatesListener((Action<object>) (_ => UIContext.GetRoot().ShowPopup(this.gameObject)), (object) null, true, true);
  }

  public void Hide()
  {
    this.m_widget.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_owner);
  }
}
