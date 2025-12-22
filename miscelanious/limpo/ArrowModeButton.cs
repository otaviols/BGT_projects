using UnityEngine;

public class ArrowModeButton : PegUIElement
{
  public HighlightState m_highlight;
  private int m_numFlips;
  private bool m_isHighlighted;

  protected override void Awake()
  {
    base.Awake();
    SoundManager.Get().Load((AssetReference) "Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9");
    SoundManager.Get().Load((AssetReference) "deck_select_button_press.prefab:46d62875e039445439070cc9f7480d48");
  }

  public void Activate(bool activate)
  {
    if (activate == this.IsEnabled())
      return;
    this.SetEnabled(activate);
    if (!activate && (Object) this.m_highlight != (Object) null)
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    ++this.m_numFlips;
    if (iTweenManager.Get().GetTweenForObject(this.gameObject) != null)
      return;
    this.Flip();
  }

  public void ActivateHighlight(bool highlightOn)
  {
    if ((Object) this.m_highlight == (Object) null)
      return;
    this.m_isHighlighted = highlightOn;
    this.m_highlight.ChangeState(this.m_isHighlighted ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.HIGHLIGHT_OFF);
  }

  protected override void OnRelease()
  {
    if ((Object) this.m_highlight == (Object) null)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "deck_select_button_press.prefab:46d62875e039445439070cc9f7480d48");
    this.m_isHighlighted = false;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9");
    if ((Object) this.m_highlight == (Object) null)
      return;
    this.m_highlight.ChangeState(this.m_isHighlighted ? ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER : ActorStateType.HIGHLIGHT_MOUSE_OVER);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if ((Object) this.m_highlight == (Object) null)
      return;
    this.m_highlight.ChangeState(this.m_isHighlighted ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.HIGHLIGHT_OFF);
  }

  private void Flip() => iTween.RotateAdd(this.gameObject, iTween.Hash((object) "amount", (object) new Vector3(180f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self, (object) "oncomplete", (object) "OnFlipComplete", (object) "oncompletetarget", (object) this.gameObject));

  private void OnFlipComplete()
  {
    --this.m_numFlips;
    if (this.m_numFlips <= 0)
      return;
    this.Flip();
  }
}
