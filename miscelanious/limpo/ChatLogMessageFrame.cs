using UnityEngine;

public class ChatLogMessageFrame : MonoBehaviour
{
  public GameObject m_Background;
  public UberText m_Text;
  private float m_initialPadding;
  private float m_initialBackgroundHeight;
  private float m_initialBackgroundLocalScaleY;

  private void Awake()
  {
    Bounds bounds = this.m_Background.GetComponent<Collider>().bounds;
    Bounds worldSpaceBounds = this.m_Text.GetTextWorldSpaceBounds();
    this.m_initialPadding = bounds.size.y - worldSpaceBounds.size.y;
    this.m_initialBackgroundHeight = bounds.size.y;
    this.m_initialBackgroundLocalScaleY = this.m_Background.transform.localScale.y;
  }

  public string GetMessage() => this.m_Text.Text;

  public void SetMessage(string message)
  {
    this.m_Text.Text = message;
    this.UpdateText();
    this.UpdateBackground();
  }

  public Color GetColor() => this.m_Text.TextColor;

  public void SetColor(Color color) => this.m_Text.TextColor = color;

  private void UpdateText() => this.m_Text.UpdateNow();

  private void UpdateBackground()
  {
    float num = this.m_Text.GetTextWorldSpaceBounds().size.y + this.m_initialPadding;
    float backgroundLocalScaleY = this.m_initialBackgroundLocalScaleY;
    if ((double) num > (double) this.m_initialBackgroundHeight)
      backgroundLocalScaleY *= num / this.m_initialBackgroundHeight;
    TransformUtil.SetLocalScaleY(this.m_Background, backgroundLocalScaleY);
  }
}
