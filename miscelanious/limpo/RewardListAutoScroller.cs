using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class RewardListAutoScroller : MonoBehaviour
{
  public UIBScrollable m_scrollable;
  public GameObject[] m_sections;
  public float m_positionOffset;
  private Widget m_listWidget;
  private int m_sectionIndex;

  private bool IsReady => (Object) this.m_listWidget != (Object) null && this.m_listWidget.IsReady && !this.m_listWidget.IsChangingStates;

  public void Init(Widget listWidget, int sectionIndex)
  {
    this.m_listWidget = listWidget;
    this.m_sectionIndex = sectionIndex;
  }

  private void OnPlayMakerPopupIntroFinished() => this.StartCoroutine(this.ScrollToSectionWhenReady());

  private IEnumerator ScrollToSectionWhenReady()
  {
    while (!this.IsReady)
      yield return (object) null;
    if (this.m_sectionIndex >= 0 && this.m_sectionIndex < this.m_sections.Length)
      this.m_scrollable.CenterObjectInView(this.m_sections[this.m_sectionIndex], this.m_positionOffset, (UIBScrollable.OnScrollComplete) null, iTween.EaseType.linear, 0.0f);
  }
}
