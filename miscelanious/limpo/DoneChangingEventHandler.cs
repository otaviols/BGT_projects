using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class DoneChangingEventHandler : MonoBehaviour
{
  public const string WAIT_FOR_DONE_CHANGING = "CODE_WAIT_FOR_DONE_CHANGING";
  public const string DISMISS_FROM_FOREGROUND = "CODE_DISMISS_FROM_FOREGROUND";
  public const string DONE_CHANGING = "CODE_DONE_CHANGING";
  [Tooltip("Hide this widget visually until it is done changing states.")]
  [SerializeField]
  private bool m_isInvisibleWhileChanging = true;
  [SerializeField]
  [Tooltip("Blur the background behind this widget.")]
  private bool m_useBackgroundBlur = true;
  private WidgetTemplate m_widget;
  private bool m_isWaiting;

  private void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "CODE_WAIT_FOR_DONE_CHANGING"))
      {
        if (!(eventName == "CODE_DISMISS_FROM_FOREGROUND"))
          return;
        this.DismissFromForeground();
      }
      else
        this.ShowWhenDone();
    }));
  }

  private void ShowWhenDone()
  {
    if (this.m_isWaiting)
      return;
    this.m_isWaiting = true;
    if (this.m_isInvisibleWhileChanging)
      this.m_widget.Hide();
    object payload = this.m_widget.GetDataModel<EventDataModel>().Payload;
    this.m_widget.RegisterDoneChangingStatesListener((Action<object>) (listener =>
    {
      this.m_widget.TriggerEvent("CODE_DONE_CHANGING", new Widget.TriggerEventParameters()
      {
        NoDownwardPropagation = true,
        Payload = payload
      });
      if (this.m_isInvisibleWhileChanging)
        this.m_widget.Show();
      if (this.m_useBackgroundBlur)
        UIContext.GetRoot().ShowPopup(this.gameObject);
      this.m_isWaiting = false;
    }), (object) null, true, true);
  }

  private void DismissFromForeground()
  {
    if (!this.m_useBackgroundBlur)
      return;
    UIContext.GetRoot().DismissPopup(this.gameObject);
  }
}
