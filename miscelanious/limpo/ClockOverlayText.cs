using System;
using UnityEngine;

public class ClockOverlayText : MonoBehaviour
{
  [SerializeField]
  private GameObject m_bannerStandard;
  [SerializeField]
  private GameObject m_bannerYear;
  [SerializeField]
  private UberText m_detailsText;
  [SerializeField]
  private UberText m_detailsTextStandard;
  [SerializeField]
  private UberText m_bannerHeadlineText;
  [SerializeField]
  private Vector3 m_maxScale;
  [SerializeField]
  private Vector3 m_maxScale_phone;
  private Vector3 m_minScale = new Vector3(0.01f, 0.01f, 0.01f);

  public void Show()
  {
    Vector3 vector3 = this.m_maxScale;
    if ((bool) UniversalInputManager.UsePhoneUI)
      vector3 = this.m_maxScale_phone;
    iTween.Stop(this.gameObject);
    this.gameObject.SetActive(true);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) vector3, (object) "time", (object) 0.4f, (object) "easetype", (object) iTween.EaseType.easeOutQuad));
    if ((UnityEngine.Object) this.m_bannerHeadlineText == (UnityEngine.Object) null)
      Debug.LogError((object) "ClockOverlayText: Banner Headline Text is not set!");
    else
      this.m_bannerHeadlineText.Text = SetRotationManager.Get().GetActiveSetRotationYearLocalizedString();
  }

  public void Hide()
  {
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_minScale, (object) "time", (object) 0.15f, (object) "easetype", (object) iTween.EaseType.easeInQuad, (object) "oncomplete", (object) (Action<object>) (o => this.gameObject.SetActive(false))));
  }

  public void HideImmediate()
  {
    this.gameObject.SetActive(false);
    this.transform.localScale = this.m_minScale;
  }

  public void UpdateText(int step)
  {
    if (step == 0)
    {
      this.m_bannerYear.SetActive(false);
      this.m_detailsText.gameObject.SetActive(false);
      this.m_bannerStandard.SetActive(true);
      this.m_detailsTextStandard.gameObject.SetActive(true);
    }
    else
    {
      this.m_bannerStandard.SetActive(false);
      this.m_detailsTextStandard.gameObject.SetActive(false);
      this.m_bannerYear.SetActive(true);
      this.m_detailsText.gameObject.SetActive(true);
    }
  }
}
