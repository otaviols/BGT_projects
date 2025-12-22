using Hearthstone.UI;
using UnityEngine;

public class VillageButton : PegUIElement
{
  private PlayMakerFSM m_fsm;
  private bool m_isStarted;
  private VisualController m_visualController;
  private const string PLAY_BUTTON_ENABLED_STATE = "ENABLED";
  private const string PLAY_BUTTON_DISABLED_STATE = "DISABLED";
  private const string PLAY_BUTTON_PRESSED_STATE = "PRESSED";
  private const string PLAY_BUTTON_RELEASED_STATE = "RELEASED";

  protected override void Awake()
  {
    base.Awake();
    this.SetOriginalLocalPosition();
  }

  protected void Start()
  {
    this.m_isStarted = true;
    this.m_fsm = this.GetComponent<PlayMakerFSM>();
    this.m_visualController = this.GetComponent<VisualController>();
    if (this.IsEnabled())
      this.Enable();
    else
      this.Disable();
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("RELEASED");
  }

  public void Disable(bool keepLabelTextVisible = false)
  {
    this.SetEnabled(false);
    if (!this.m_isStarted)
      return;
    if ((Object) this.m_fsm != (Object) null && !keepLabelTextVisible)
      this.m_fsm.SendEvent("Cancel");
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("DISABLED");
  }

  public void Enable()
  {
    this.SetEnabled(true);
    if (!this.m_isStarted)
      return;
    if ((Object) this.m_fsm != (Object) null)
      this.m_fsm.SendEvent("Birth");
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("ENABLED");
  }

  protected override void OnPress()
  {
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("PRESSED");
  }

  protected override void OnRelease()
  {
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("RELEASED");
  }
}
