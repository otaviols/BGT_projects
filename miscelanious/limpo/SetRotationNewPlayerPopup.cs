using Hearthstone.UI;
using System;
using UnityEngine;

public class SetRotationNewPlayerPopup : BasicPopup
{
  private const string HIDE_FINISHED_EVENT_NAME = "CODE_HIDE_FINISHED";
  private WidgetTemplate m_widget;

  protected override void Awake()
  {
    base.Awake();
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (eventName == "Button_Framed_Clicked")
        this.Hide();
      if (!(eventName == "CODE_HIDE_FINISHED"))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }));
    this.m_widget.RegisterReadyListener((Action<object>) (_ => this.OnWidgetReady()), (object) null, true);
  }

  protected override void OnDestroy()
  {
    GameObject gameObject = this.transform.parent.gameObject;
    if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null && (UnityEngine.Object) gameObject.GetComponent<WidgetInstance>() != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.transform.parent.gameObject);
    base.OnDestroy();
  }

  private void OnWidgetReady()
  {
    if (!((UnityEngine.Object) this.m_headerText != (UnityEngine.Object) null))
      return;
    this.m_headerText.Text = GameStrings.Format("GLUE_NEW_PLAYER_SET_ROTATION_POPUP_HEADER", (object) SetRotationManager.Get().GetActiveSetRotationYearLocalizedString());
  }
}
