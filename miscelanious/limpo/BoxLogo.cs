using UnityEngine;

public class BoxLogo : MonoBehaviour
{
  private Box m_parent;
  private BoxLogoStateInfo m_info;
  private BoxLogo.State m_state;

  public Box GetParent() => this.m_parent;

  public void SetParent(Box parent) => this.m_parent = parent;

  public BoxLogoStateInfo GetInfo() => this.m_info;

  public void SetInfo(BoxLogoStateInfo info) => this.m_info = info;

  public bool ChangeState(BoxLogo.State state)
  {
    if (this.m_state == state)
      return false;
    this.m_state = state;
    switch (state)
    {
      case BoxLogo.State.SHOWN:
        this.m_parent.OnAnimStarted();
        iTween.FadeTo(this.gameObject, iTween.Hash((object) "amount", (object) this.m_info.m_ShownAlpha, (object) "delay", (object) this.m_info.m_ShownDelaySec, (object) "time", (object) this.m_info.m_ShownFadeSec, (object) "easeType", (object) this.m_info.m_ShownFadeEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompletetarget", (object) this.m_parent.gameObject));
        break;
      case BoxLogo.State.HIDDEN:
        this.m_parent.OnAnimStarted();
        iTween.FadeTo(this.gameObject, iTween.Hash((object) "amount", (object) this.m_info.m_HiddenAlpha, (object) "delay", (object) this.m_info.m_HiddenDelaySec, (object) "time", (object) this.m_info.m_HiddenFadeSec, (object) "easeType", (object) this.m_info.m_HiddenFadeEaseType, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompletetarget", (object) this.m_parent.gameObject));
        break;
    }
    return true;
  }

  public void UpdateState(BoxLogo.State state)
  {
    this.m_state = state;
    if (state == BoxLogo.State.SHOWN)
    {
      RenderUtils.SetAlpha(this.gameObject, this.m_info.m_ShownAlpha);
    }
    else
    {
      if (state != BoxLogo.State.HIDDEN)
        return;
      RenderUtils.SetAlpha(this.gameObject, this.m_info.m_HiddenAlpha);
    }
  }

  public enum State
  {
    SHOWN,
    HIDDEN,
  }
}
