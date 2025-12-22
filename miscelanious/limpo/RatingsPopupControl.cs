using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RatingsPopupControl : MonoBehaviour
{
  public bool WaitForUserToStart;
  public string m_startPressedEvent = "USER_START_PRESSED";
  public string m_inputModeClickEvent = "INPUT_MODE_CLICK";
  public string m_inputModeTouchEvent = "INPUT_MODE_TOUCH";
  private Widget m_widget;

  public event Action OnUserStartPressed;

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnEventMessage));
    this.m_widget.RegisterReadyListener(new Action<object>(this.OnWidgetReady), (object) null, true);
  }

  private void OnEventMessage(string eventName)
  {
    if (!eventName.Equals(this.m_startPressedEvent))
      return;
    Action userStartPressed = this.OnUserStartPressed;
    if (userStartPressed == null)
      return;
    userStartPressed();
  }

  private void OnWidgetReady(object obj)
  {
    switch (PlatformSettings.Input)
    {
      case InputCategory.Mouse:
        this.m_widget.TriggerEvent(this.m_inputModeClickEvent);
        break;
      case InputCategory.Touch:
        this.m_widget.TriggerEvent(this.m_inputModeTouchEvent);
        break;
    }
  }
}
