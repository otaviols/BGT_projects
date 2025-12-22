using Hearthstone.UI;
using UnityEngine;

public class LettuceTaskCollectionListRowPreloader : MonoBehaviour
{
  private Widget m_widget;

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
  }

  private void HandleEvent(string eventName)
  {
    if (eventName == "POST_SHOW_TASK_ITEM")
    {
      this.m_widget.TriggerEvent("SHOW_TASK_ITEM");
    }
    else
    {
      if (!(eventName == "POST_HIDE_TASK_ITEM"))
        return;
      this.m_widget.TriggerEvent("HIDE_TASK_ITEM");
    }
  }
}
