using System.Collections.Generic;
using UnityEngine;

public class CardRuneBanner : MonoBehaviour
{
  public GameObject m_visualContainer;
  public List<RuneSlotVisual> m_runeSlotVisuals;
  public GameObject m_highlight;
  public GameObject m_runeBannerBackground;
  private RunePattern m_lastShownRunePattern;

  public void Show(RunePattern runePattern)
  {
    foreach (RuneSlotVisual runeSlotVisual in this.m_runeSlotVisuals)
      runeSlotVisual.Hide();
    switch (runePattern.CombinedValue)
    {
      case 1:
        this.m_runeSlotVisuals[0].Show(runePattern);
        break;
      case 2:
        this.m_runeSlotVisuals[1].Show(runePattern);
        break;
      case 3:
        this.m_runeSlotVisuals[2].Show(runePattern);
        break;
      default:
        return;
    }
    if ((Object) this.m_visualContainer != (Object) null)
      this.m_visualContainer.gameObject.SetActive(true);
    if ((Object) this.m_runeBannerBackground != (Object) null && runePattern.CombinedValue > 0)
      this.m_runeBannerBackground.SetActive(true);
    this.m_lastShownRunePattern = runePattern;
  }

  public void Hide()
  {
    if ((Object) this.m_runeBannerBackground != (Object) null)
      this.m_runeBannerBackground.SetActive(false);
    if (!((Object) this.m_visualContainer != (Object) null))
      return;
    this.m_visualContainer.gameObject.SetActive(false);
  }

  public void ShowLastShownRuneBanner() => this.Show(this.m_lastShownRunePattern);

  public void SetHighlighted(bool highlighted) => this.m_highlight.SetActive(highlighted);

  public RunePattern GetCurrentRunePattern() => this.m_lastShownRunePattern;
}
