using UnityEngine;

public class BoxDrawer : MonoBehaviour
{
  private Box m_parent;
  private BoxDrawerStateInfo m_info;
  private BoxDrawer.State m_state;

  public Box GetParent() => this.m_parent;

  public void SetParent(Box parent) => this.m_parent = parent;

  public BoxDrawerStateInfo GetInfo() => this.m_info;

  public void SetInfo(BoxDrawerStateInfo info) => this.m_info = info;

  public bool ChangeState(BoxDrawer.State state)
  {
    if (DemoMgr.Get().GetMode() == DemoMode.PAX_EAST_2013 || DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2013)
      return true;
    if (this.m_state == state)
      return false;
    BoxDrawer.State state1 = this.m_state;
    this.m_state = state;
    if (this.IsInactiveState(state1) && this.IsInactiveState(this.m_state))
      return true;
    this.gameObject.SetActive(true);
    switch (state)
    {
      case BoxDrawer.State.CLOSED:
        this.m_parent.OnAnimStarted();
        iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_info.m_ClosedBone.transform.position, (object) "delay", (object) this.m_info.m_ClosedDelaySec, (object) "time", (object) this.m_info.m_ClosedMoveSec, (object) "easeType", (object) this.m_info.m_ClosedMoveEaseType, (object) "oncomplete", (object) "OnClosedAnimFinished", (object) "oncompletetarget", (object) this.gameObject));
        this.m_parent.GetEventSpell(BoxEventType.DRAWER_CLOSE).Activate();
        break;
      case BoxDrawer.State.CLOSED_BOX_OPENED:
        this.m_parent.OnAnimStarted();
        iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_info.m_ClosedBoxOpenedBone.transform.position, (object) "delay", (object) this.m_info.m_ClosedBoxOpenedDelaySec, (object) "time", (object) this.m_info.m_ClosedBoxOpenedMoveSec, (object) "easeType", (object) this.m_info.m_ClosedBoxOpenedMoveEaseType, (object) "oncomplete", (object) "OnClosedBoxOpenedAnimFinished", (object) "oncompletetarget", (object) this.gameObject));
        this.m_parent.GetEventSpell(BoxEventType.DRAWER_CLOSE).Activate();
        break;
      case BoxDrawer.State.OPENED:
        this.m_parent.OnAnimStarted();
        iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_info.m_OpenedBone.transform.position, (object) "delay", (object) this.m_info.m_OpenedDelaySec, (object) "time", (object) this.m_info.m_OpenedMoveSec, (object) "easeType", (object) this.m_info.m_OpenedMoveEaseType, (object) "oncomplete", (object) "OnOpenedAnimFinished", (object) "oncompletetarget", (object) this.gameObject));
        this.m_parent.GetEventSpell(BoxEventType.DRAWER_OPEN).Activate();
        break;
    }
    return true;
  }

  public void UpdateState(BoxDrawer.State state)
  {
    this.m_state = state;
    switch (state)
    {
      case BoxDrawer.State.CLOSED:
        this.transform.position = this.m_info.m_ClosedBone.transform.position;
        this.gameObject.SetActive(false);
        break;
      case BoxDrawer.State.CLOSED_BOX_OPENED:
        this.transform.position = this.m_info.m_ClosedBoxOpenedBone.transform.position;
        this.gameObject.SetActive(false);
        break;
      case BoxDrawer.State.OPENED:
        this.transform.position = this.m_info.m_OpenedBone.transform.position;
        this.gameObject.SetActive(true);
        break;
    }
  }

  private bool IsInactiveState(BoxDrawer.State state) => state == BoxDrawer.State.CLOSED || state == BoxDrawer.State.CLOSED_BOX_OPENED;

  private void OnClosedAnimFinished()
  {
    this.gameObject.SetActive(false);
    this.m_parent.OnAnimFinished();
  }

  private void OnClosedBoxOpenedAnimFinished()
  {
    this.gameObject.SetActive(false);
    this.m_parent.OnAnimFinished();
  }

  private void OnOpenedAnimFinished()
  {
    this.gameObject.SetActive(true);
    this.m_parent.OnAnimFinished();
  }

  public enum State
  {
    CLOSED,
    CLOSED_BOX_OPENED,
    OPENED,
  }
}
