using Hearthstone.UI;
using UnityEngine;

public class DeepLinkEventController : MonoBehaviour
{
  public string m_deepLink = "hearthstone://ranked";
  private Widget m_widget;
  private const string GoToDeepLink = "GoToDeepLink";

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    if (!((Object) this.m_widget != (Object) null))
      return;
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnEvent));
  }

  private void OnEvent(string eventName)
  {
    if (!(eventName == "GoToDeepLink") || this.m_deepLink == null || this.m_deepLink.Length <= 0)
      return;
    DeepLinkManager.ExecuteDeepLink(this.m_deepLink.Substring("hearthstone://".Length).Split('/'), DeepLinkManager.DeepLinkSource.IN_GAME_MESSAGE, false);
  }
}
