using UnityEngine;

public class ResizableTooltipPanel : TooltipPanel
{
  protected float m_heightPadding = 1f;
  protected float m_bodyTextHeight;
  protected float m_bodyPadding = 0.1f;

  public override void Initialize(string keywordName, string keywordText)
  {
    NewThreeSliceElement component = this.m_background.GetComponent<NewThreeSliceElement>();
    if ((Object) component == (Object) null)
      Error.AddDevFatal("Prefab expecting m_background to have a NewThreeSliceElement!");
    base.Initialize(keywordName, keywordText);
    this.m_bodyTextHeight = this.m_body.GetTextBounds().size.y;
    if (keywordText == "")
      this.m_bodyTextHeight = 0.0f;
    if ((double) this.m_initialBackgroundHeight == 0.0 || this.m_initialBackgroundScale == Vector3.zero)
    {
      this.m_initialBackgroundHeight = component.m_middle.GetComponent<Renderer>().bounds.size.z;
      this.m_initialBackgroundScale = component.m_middle.transform.localScale;
    }
    this.SetBackgroundSize(!string.IsNullOrEmpty(keywordName) ? (this.m_name.Height + this.m_bodyTextHeight) * this.m_heightPadding : (this.m_bodyTextHeight + this.m_bodyPadding) * this.m_heightPadding);
  }

  protected void SetBackgroundSize(float totalHeight) => this.m_background.GetComponent<NewThreeSliceElement>().SetSize(new Vector3(this.m_initialBackgroundScale.x, this.m_initialBackgroundScale.y * totalHeight / this.m_initialBackgroundHeight, this.m_initialBackgroundScale.z));

  public override float GetHeight()
  {
    NewThreeSliceElement component = this.m_background.GetComponent<NewThreeSliceElement>();
    Bounds bounds = component.m_leftOrTop.GetComponent<Renderer>().bounds;
    double z1 = (double) bounds.size.z;
    bounds = component.m_middle.GetComponent<Renderer>().bounds;
    double z2 = (double) bounds.size.z;
    double num = z1 + z2;
    bounds = component.m_rightOrBottom.GetComponent<Renderer>().bounds;
    double z3 = (double) bounds.size.z;
    return (float) (num + z3);
  }

  public override float GetWidth() => this.m_background.GetComponent<NewThreeSliceElement>().m_leftOrTop.GetComponent<Renderer>().bounds.size.x;
}
