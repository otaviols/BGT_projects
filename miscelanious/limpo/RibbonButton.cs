using UnityEngine;

public class RibbonButton : PegUIElement
{
  public GameObject m_highlight;

  public void Start()
  {
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
  }

  public void OnButtonOver(UIEvent e)
  {
    if (!((Object) this.m_highlight != (Object) null))
      return;
    this.m_highlight.SetActive(true);
  }

  public void OnButtonOut(UIEvent e)
  {
    if (!((Object) this.m_highlight != (Object) null))
      return;
    this.m_highlight.SetActive(false);
  }
}
