using UnityEngine;

public class BoxDisk : MonoBehaviour
{
  private Box m_parent;
  private BoxDiskStateInfo m_info;
  private BoxDisk.State m_state;

  public void SetParent(Box parent) => this.m_parent = parent;

  public Box GetParent() => this.m_parent;

  public BoxDiskStateInfo GetInfo() => this.m_info;

  public void SetInfo(BoxDiskStateInfo info) => this.m_info = info;

  public bool ChangeState(BoxDisk.State state)
  {
    if (this.m_state == state)
      return false;
    this.m_state = state;
    switch (state)
    {
      case BoxDisk.State.LOADING:
        this.m_parent.OnAnimStarted();
        iTween.RotateAdd(this.gameObject, iTween.Hash((object) "amount", (object) (this.m_info.m_LoadingRotation - this.transform.localRotation.eulerAngles), (object) "delay", (object) this.m_info.m_LoadingDelaySec, (object) "time", (object) this.m_info.m_LoadingRotateSec, (object) "easeType", (object) this.m_info.m_LoadingRotateEaseType, (object) "space", (object) Space.Self, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompletetarget", (object) this.m_parent.gameObject));
        this.m_parent.GetEventSpell(BoxEventType.DISK_LOADING).ActivateState(SpellStateType.BIRTH);
        break;
      case BoxDisk.State.MAINMENU:
        this.m_parent.OnAnimStarted();
        iTween.RotateAdd(this.gameObject, iTween.Hash((object) "amount", (object) (this.m_info.m_MainMenuRotation - this.transform.localRotation.eulerAngles), (object) "delay", (object) this.m_info.m_MainMenuDelaySec, (object) "time", (object) this.m_info.m_MainMenuRotateSec, (object) "easeType", (object) this.m_info.m_MainMenuRotateEaseType, (object) "space", (object) Space.Self, (object) "oncomplete", (object) "OnAnimFinished", (object) "oncompletetarget", (object) this.m_parent.gameObject));
        this.m_parent.GetEventSpell(BoxEventType.DISK_MAIN_MENU).ActivateState(SpellStateType.BIRTH);
        break;
    }
    return true;
  }

  public void UpdateState(BoxDisk.State state)
  {
    this.m_state = state;
    if (state == BoxDisk.State.LOADING)
    {
      this.transform.localRotation = Quaternion.Euler(this.m_info.m_LoadingRotation);
      this.m_parent.GetEventSpell(BoxEventType.DISK_LOADING).ActivateState(SpellStateType.ACTION);
    }
    else
    {
      if (state != BoxDisk.State.MAINMENU)
        return;
      this.transform.localRotation = Quaternion.Euler(this.m_info.m_MainMenuRotation);
      this.m_parent.GetEventSpell(BoxEventType.DISK_MAIN_MENU).ActivateState(SpellStateType.ACTION);
    }
  }

  public enum State
  {
    LOADING,
    MAINMENU,
  }
}
