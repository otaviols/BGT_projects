using System.Collections;
using UnityEngine;

public class HeroLabel : MonoBehaviour
{
  public UberText m_nameText;
  public UberText m_classText;

  public void UpdateText(string nameText, string classText)
  {
    this.m_nameText.Text = nameText;
    this.m_classText.Text = classText;
  }

  public void SetFade(float fade)
  {
    this.m_nameText.TextAlpha = fade;
    this.m_classText.TextAlpha = fade;
  }

  public void SetColor(Color color)
  {
    this.m_nameText.TextColor = color;
    this.m_classText.TextColor = color;
  }

  public void FadeIn()
  {
    if ((Object) this.m_nameText == (Object) null || (Object) this.m_classText == (Object) null)
      return;
    iTween.Stop(this.m_nameText.gameObject);
    iTween.Stop(this.m_classText.gameObject);
    iTween.FadeTo(this.m_nameText.gameObject, 1f, 0.5f);
    iTween.FadeTo(this.m_classText.gameObject, 1f, 0.5f);
  }

  public void FadeOut()
  {
    if ((Object) this.m_nameText == (Object) null || (Object) this.m_classText == (Object) null)
      return;
    iTween.Stop(this.m_nameText.gameObject);
    iTween.Stop(this.m_classText.gameObject);
    iTween.FadeTo(this.m_nameText.gameObject, 0.0f, 0.5f);
    iTween.FadeTo(this.m_classText.gameObject, 0.0f, 0.5f);
    this.StartCoroutine(this.FinishFade());
  }

  private IEnumerator FinishFade()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    HeroLabel heroLabel = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Object.Destroy((Object) heroLabel.gameObject);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(1f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
