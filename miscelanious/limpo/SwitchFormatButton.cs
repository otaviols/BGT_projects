using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class SwitchFormatButton : UIBButton
{
  public MeshRenderer m_buttonRenderer;
  public HighlightState m_highlight;
  public GameObject m_coverObject;
  public UIBHighlight m_uibHighlight;
  public VisualController m_visualController;
  private VisualsFormatType m_visualsFormatType = VisualsFormatType.VFT_STANDARD;
  private bool m_isCovered;
  private const string STANDARD_STATE = "STANDARD";
  private const string CLASSIC_STATE = "CLASSIC";
  private const string WILD_STATE = "WILD";
  private const string CASUAL_STATE = "CASUAL";
  private const string COVERED_STATE = "COVERED";

  private void UpdateIcon()
  {
    if (this.m_isCovered)
    {
      this.m_visualController.SetState("COVERED");
    }
    else
    {
      switch (this.m_visualsFormatType)
      {
        case VisualsFormatType.VFT_WILD:
          this.m_visualController.SetState("WILD");
          break;
        case VisualsFormatType.VFT_STANDARD:
          this.m_visualController.SetState("STANDARD");
          break;
        case VisualsFormatType.VFT_CLASSIC:
          this.m_visualController.SetState("CLASSIC");
          break;
        case VisualsFormatType.VFT_CASUAL:
          this.m_visualController.SetState("CASUAL");
          break;
      }
    }
  }

  public void SetVisualsFormatType(VisualsFormatType newVisualsFormatType)
  {
    if (this.m_visualsFormatType == newVisualsFormatType)
      return;
    this.m_visualsFormatType = newVisualsFormatType;
    this.UpdateIcon();
  }

  public void Disable()
  {
    this.m_uibHighlight.Reset();
    this.SetEnabled(false);
  }

  public void Enable()
  {
    if (!this.gameObject.activeSelf)
      this.gameObject.SetActive(true);
    this.SetEnabled(true);
    this.UpdateIcon();
  }

  public IEnumerator EnableWithDelay(float delay)
  {
    SwitchFormatButton switchFormatButton = this;
    yield return (object) new WaitForSeconds(delay);
    if (!switchFormatButton.gameObject.activeSelf)
      switchFormatButton.gameObject.SetActive(true);
    switchFormatButton.SetEnabled(true);
    switchFormatButton.UpdateIcon();
  }

  public void Cover()
  {
    this.m_isCovered = true;
    this.UpdateIcon();
  }

  public void Uncover()
  {
    this.m_isCovered = false;
    this.UpdateIcon();
  }

  public bool IsCovered() => this.m_isCovered;

  public void EnableHighlight(bool enabled) => this.EnableHighlightImpl(enabled);

  private void EnableHighlightImpl(bool enabled)
  {
    if (enabled)
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
  }
}
