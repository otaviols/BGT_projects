using UnityEngine;

public class QuestLogButton : PegUIElement
{
  public HighlightState m_highlight;

  private void Start()
  {
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
    if (SoundManager.Get() == null)
      return;
    SoundManager.Get().Load((AssetReference) "quest_log_button_mouse_over.prefab:e5102eddbc19ec84297aa49eecb66397");
  }

  private void OnButtonOver(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "quest_log_button_mouse_over.prefab:e5102eddbc19ec84297aa49eecb66397", this.gameObject);
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    TooltipZone component = this.GetComponent<TooltipZone>();
    if ((Object) component == (Object) null)
      return;
    component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_QUESTLOG_HEADLINE"), GameStrings.Get("GLUE_TOOLTIP_BUTTON_QUESTLOG_DESC"));
  }

  private void OnButtonOut(UIEvent e)
  {
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    TooltipZone component = this.GetComponent<TooltipZone>();
    if (!((Object) component != (Object) null))
      return;
    component.HideTooltip();
  }
}
