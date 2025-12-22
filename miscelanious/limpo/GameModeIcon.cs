using System.Collections.Generic;
using UnityEngine;

public class GameModeIcon : PegUIElement
{
  public UberText m_text;
  public List<GameObject> m_Xmarks = new List<GameObject>();
  public GameObject m_friendlyChallengeBanner;
  public GameObject m_wildVines;

  public void Show(bool show) => this.gameObject.SetActive(show);

  public void SetText(string text)
  {
    if ((Object) this.m_text == (Object) null)
      return;
    this.m_text.Text = text;
  }

  public void ShowXMarks(uint numberOfMarks)
  {
    if (this.m_Xmarks.Count == 0)
      return;
    for (int index = 0; (long) index < (long) numberOfMarks; ++index)
      this.m_Xmarks[index].SetActive(true);
  }

  public void ShowFriendlyChallengeBanner(bool showBanner)
  {
    if ((Object) this.m_friendlyChallengeBanner == (Object) null)
      return;
    this.m_friendlyChallengeBanner.SetActive(showBanner);
  }

  public void ShowWildVines(bool showVines)
  {
    if ((Object) this.m_wildVines == (Object) null)
      return;
    this.m_wildVines.SetActive(showVines);
  }
}
