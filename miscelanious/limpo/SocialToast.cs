using UnityEngine;

public class SocialToast : MonoBehaviour
{
  [SerializeField]
  private UberText m_text;

  public void SetText(string text)
  {
    this.m_text.Text = text;
    ThreeSliceElement component = this.GetComponent<ThreeSliceElement>();
    if (!((Object) component != (Object) null))
      return;
    float x = this.m_text.GetTextWorldSpaceBounds().size.x;
    float num = (float) (((double) component.GetLeftSize().x + (double) component.GetRightSize().x) * 0.5);
    component.SetWidth(x + num);
  }
}
