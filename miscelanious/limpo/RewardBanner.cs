using UnityEngine;

public class RewardBanner : MonoBehaviour
{
  public UberText m_headlineText;
  public UberText m_detailsText;
  public UberText m_sourceText;
  public GameObject m_headlineCenterBone;
  private float m_headlineHeight;

  private void Awake()
  {
    if ((bool) UniversalInputManager.UsePhoneUI && (Object) this.m_sourceText != (Object) null)
      this.m_sourceText.gameObject.SetActive(false);
    this.m_headlineHeight = this.m_headlineText.Height;
  }

  public string HeadlineText => this.m_headlineText.Text;

  public string DetailsText => this.m_detailsText.Text;

  public string SourceText => this.m_sourceText.Text;

  public void SetText(string headline, string details, string source)
  {
    this.m_headlineText.Text = headline;
    this.m_detailsText.Text = details;
    this.m_sourceText.Text = source;
    if (!(details == ""))
      return;
    this.AlignHeadlineToCenterBone();
    this.m_headlineText.Height = this.m_headlineHeight * 1.5f;
  }

  public void AlignHeadlineToCenterBone()
  {
    if (!((Object) this.m_headlineCenterBone != (Object) null))
      return;
    this.m_headlineText.transform.localPosition = this.m_headlineCenterBone.transform.localPosition;
  }
}
