using UnityEngine;

public class BoxDoor : MonoBehaviour
{
  private const float BOX_SLIDE_PERCENTAGE_PHONE = 1.038f;
  private Box m_parent;
  private BoxDoorStateInfo m_info;
  private BoxDoor.State m_state;
  private bool m_main;
  private Vector3 m_startingPosition;

  private void Awake() => this.m_startingPosition = this.gameObject.transform.localPosition;

  public Box GetParent() => this.m_parent;

  public void SetParent(Box parent) => this.m_parent = parent;

  public BoxDoorStateInfo GetInfo() => this.m_info;

  public void SetInfo(BoxDoorStateInfo info) => this.m_info = info;

  public void EnableMain(bool enable) => this.m_main = enable;

  private bool IsMain() => this.m_main;

  public bool ChangeState(BoxDoor.State state)
  {
    if (this.m_state == state)
      return false;
    this.m_state = state;
    switch (state)
    {
      case BoxDoor.State.CLOSED:
        this.m_parent.OnAnimStarted();
        iTween.RotateAdd(this.gameObject, iTween.Hash((object) "amount", (object) (this.m_info.m_ClosedRotation - this.m_info.m_OpenedRotation), (object) "delay", (object) this.m_info.m_ClosedDelaySec, (object) "time", (object) this.m_info.m_ClosedRotateSec, (object) "easeType", (object) this.m_info.m_ClosedRotateEaseType, (object) "space", (object) Space.Self, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompletetarget", (object) this.m_parent.gameObject));
        if ((bool) UniversalInputManager.UsePhoneUI)
          iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_startingPosition, (object) "isLocal", (object) true, (object) "delay", (object) this.m_info.m_ClosedDelaySec, (object) "time", (object) this.m_info.m_ClosedRotateSec, (object) "easeType", (object) this.m_info.m_ClosedRotateEaseType));
        if (this.IsMain())
        {
          this.m_parent.GetEventSpell(BoxEventType.DOORS_CLOSE).Activate();
          this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_IN).ActivateState(SpellStateType.BIRTH);
          break;
        }
        break;
      case BoxDoor.State.OPENED:
        this.m_parent.OnAnimStarted();
        iTween.RotateAdd(this.gameObject, iTween.Hash((object) "amount", (object) (this.m_info.m_OpenedRotation - this.m_info.m_ClosedRotation), (object) "delay", (object) this.m_info.m_OpenedDelaySec, (object) "time", (object) this.m_info.m_OpenedRotateSec, (object) "easeType", (object) this.m_info.m_OpenedRotateEaseType, (object) "space", (object) Space.Self, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompletetarget", (object) this.m_parent.gameObject));
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          Vector3 startingPosition = this.m_startingPosition;
          startingPosition.x *= 1.038f;
          iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) startingPosition, (object) "isLocal", (object) true, (object) "delay", (object) this.m_info.m_ClosedDelaySec, (object) "time", (object) this.m_info.m_ClosedRotateSec, (object) "easeType", (object) this.m_info.m_ClosedRotateEaseType));
        }
        if (this.IsMain())
        {
          this.m_parent.GetEventSpell(BoxEventType.DOORS_OPEN).Activate();
          this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_OUT).ActivateState(SpellStateType.BIRTH);
          break;
        }
        break;
    }
    return true;
  }

  public void UpdateState(BoxDoor.State state)
  {
    this.m_state = state;
    if (state == BoxDoor.State.CLOSED)
    {
      this.transform.localRotation = Quaternion.Euler(this.m_info.m_ClosedRotation);
      this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_IN).ActivateState(SpellStateType.ACTION);
    }
    else
    {
      if (state != BoxDoor.State.OPENED)
        return;
      this.transform.localRotation = Quaternion.Euler(this.m_info.m_OpenedRotation);
      this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_OUT).ActivateState(SpellStateType.ACTION);
    }
  }

  public enum State
  {
    CLOSED,
    OPENED,
  }
}
