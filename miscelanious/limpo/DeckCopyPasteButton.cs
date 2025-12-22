using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class DeckCopyPasteButton : PegUIElement
{
  public UberText ButtonText;
  private bool m_clickEnabled;

  public string TooltipMessage { get; set; }

  public string TooltipHeaderString { get; set; }

  private void Start()
  {
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
  }

  public override void SetEnabled(bool enabled, bool isInternal = false)
  {
    this.m_clickEnabled = enabled;
    this.GetComponent<Renderer>().GetMaterial().SetFloat("_Desaturate", enabled ? 0.0f : 1f);
  }

  public bool IsClickEnabled() => this.m_clickEnabled;

  public override void TriggerPress()
  {
    if (!this.m_clickEnabled)
      return;
    base.TriggerPress();
  }

  private void OnButtonOver(UIEvent e)
  {
    TooltipZone component = this.GetComponent<TooltipZone>();
    if ((Object) component == (Object) null || string.IsNullOrEmpty(this.TooltipMessage))
      return;
    component.ShowTooltip(this.TooltipHeaderString, this.TooltipMessage, 4f);
  }

  private void OnButtonOut(UIEvent e)
  {
    TooltipZone component = this.GetComponent<TooltipZone>();
    if (!((Object) component != (Object) null))
      return;
    component.HideTooltip();
  }
}
