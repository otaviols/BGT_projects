using UnityEngine;

public class ThreeSliceTextElement : MonoBehaviour
{
  public UberText m_text;
  public ThreeSliceElement m_threeSlice;

  public void SetText(string text)
  {
    this.m_text.Text = text;
    this.m_text.UpdateNow();
    this.Resize();
  }

  public void Resize() => this.m_threeSlice.SetMiddleWidth(this.GetTextWidth());

  private float GetTextWidth() => this.m_text.GetTextBounds().size.x;
}
