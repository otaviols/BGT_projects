using Hearthstone.UI;
using UnityEngine;

public class PlayButton : PegUIElement
{
  public Vector3 m_pressMovement = new Vector3(0.0f, -0.9f, 0.0f);
  public UberText m_newPlayButtonText;
  public UberText m_playButtonSecondaryText;
  protected HighlightState m_playButtonHighlightState;
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
    if (SoundManager.Get() != null)
      SoundManager.Get().Load((AssetReference) "play_button_mouseover.prefab:359a8482de643b141bb9afb5a351fe33");
    this.m_playButtonHighlightState = this.gameObject.GetComponentInChildren<HighlightState>();
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

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "play_button_mouseover.prefab:359a8482de643b141bb9afb5a351fe33", this.gameObject);
    if (!((Object) this.m_playButtonHighlightState != (Object) null))
      return;
    this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOriginalLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.25f));
    if ((Object) this.m_playButtonHighlightState != (Object) null)
      this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("RELEASED");
  }

  public void ChangeHighlightState(ActorStateType stateType)
  {
    if ((Object) this.m_playButtonHighlightState == (Object) null)
      return;
    this.m_playButtonHighlightState.ChangeState(stateType);
  }

  public void Disable(bool keepLabelTextVisible = false)
  {
    this.SetEnabled(false);
    if (!this.m_isStarted)
      return;
    if ((Object) this.m_fsm != (Object) null && !keepLabelTextVisible)
      this.m_fsm.SendEvent("Cancel");
    if ((Object) this.m_playButtonHighlightState != (Object) null)
      this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("DISABLED");
  }

  public void Enable()
  {
    this.SetEnabled(true);
    this.m_newPlayButtonText.UpdateNow();
    if (!this.m_isStarted)
      return;
    if ((Object) this.m_newPlayButtonText != (Object) null)
      this.m_newPlayButtonText.TextAlpha = 1f;
    if ((Object) this.m_fsm != (Object) null)
      this.m_fsm.SendEvent("Birth");
    if ((Object) this.m_playButtonHighlightState != (Object) null)
      this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("ENABLED");
  }

  protected override void OnPress()
  {
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.GetOriginalLocalPosition() + this.m_pressMovement), (object) "isLocal", (object) true, (object) "time", (object) 0.25f));
    this.ChangeHighlightState(ActorStateType.HIGHLIGHT_OFF);
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_select_hero.prefab:248ea6ef307bf88468af342d2c2bd2e7");
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("PRESSED");
  }

  protected override void OnRelease()
  {
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOriginalLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.25f));
    if (!((Object) this.m_visualController != (Object) null))
      return;
    this.m_visualController.SetState("RELEASED");
  }

  public void SetText(string newText)
  {
    if (!((Object) this.m_newPlayButtonText != (Object) null))
      return;
    this.m_newPlayButtonText.Text = newText;
  }

  public void SetSecondaryText(string newText)
  {
    if (!((Object) this.m_playButtonSecondaryText != (Object) null))
      return;
    this.m_playButtonSecondaryText.Text = newText;
  }
}
