using System.Runtime.CompilerServices;
using UnityEngine;

public class MobileChatLogMessageFrame : MonoBehaviour, ITouchListItem
{
  public UberText text;
  public GameObject m_Background;

  public string Message
  {
    get => this.text.Text;
    set
    {
      this.text.Text = value;
      this.text.UpdateNow();
      this.UpdateLocalBounds();
    }
  }

  public bool IsHeader => false;

  public bool Visible
  {
    get => true;
    set
    {
    }
  }

  public Color Color
  {
    get => this.text.TextColor;
    set => this.text.TextColor = value;
  }

  public virtual float Width
  {
    get => this.text.Width;
    set
    {
      this.text.Width = value;
      if (!((Object) this.m_Background != (Object) null))
        return;
      float x = this.m_Background.GetComponent<MeshFilter>().mesh.bounds.size.x;
      this.m_Background.transform.localScale = new Vector3(value / x, this.m_Background.transform.localScale.y, 1f);
      this.m_Background.transform.localPosition = new Vector3((float) (-(double) value / (0.5 * (double) x)), 0.0f, 0.0f);
    }
  }

  public Bounds LocalBounds { get; protected set; }

  public new T GetComponent<T>() where T : Component => base.GetComponent<T>();

  public virtual void RebuildUberText() => this.text.UpdateNow(true);

  public void OnScrollOutOfView()
  {
  }

  public virtual void OnPositionUpdate()
  {
  }

  public virtual void UpdateLocalBounds()
  {
    this.RebuildUberText();
    Bounds textBounds = this.text.GetTextBounds();
    Vector3 size = textBounds.size;
    Bounds bounds = new Bounds();
    bounds.center = this.transform.InverseTransformPoint(textBounds.center) + 10f * Vector3.up;
    Vector3 lossyScale = this.transform.lossyScale;
    bounds.size = new Vector3(size.x / lossyScale.x, size.y / lossyScale.y, size.z / lossyScale.z);
    this.LocalBounds = bounds;
  }

  [SpecialName]
  GameObject ITouchListItem.get_gameObject() => this.gameObject;

  [SpecialName]
  Transform ITouchListItem.get_transform() => this.transform;
}
