using System.Runtime.CompilerServices;
using UnityEngine;

public class FriendListItemFooter : PegUIElement, ITouchListItem
{
  public UberText m_Text;

  public Bounds LocalBounds
  {
    get
    {
      Bounds setPointBounds = TransformUtil.ComputeSetPointBounds((Component) this.GetComponent<Collider>());
      return new Bounds(this.transform.InverseTransformPoint(setPointBounds.center), setPointBounds.size);
    }
  }

  public string Text
  {
    get => this.m_Text.Text;
    set => this.m_Text.Text = value;
  }

  public bool IsHeader => false;

  public bool Visible
  {
    get => this.gameObject.activeSelf;
    set => this.gameObject.SetActive(value);
  }

  public new T GetComponent<T>() where T : Component => base.GetComponent<T>();

  protected override void Awake() => base.Awake();

  public void OnScrollOutOfView()
  {
  }

  public void OnPositionUpdate()
  {
  }

  [SpecialName]
  GameObject ITouchListItem.get_gameObject() => this.gameObject;

  [SpecialName]
  Transform ITouchListItem.get_transform() => this.transform;
}
