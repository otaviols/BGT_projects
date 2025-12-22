using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MobileFriendListItem : MonoBehaviour, ISelectableTouchListItem, ITouchListItem
{
  private Bounds m_localBounds;
  private ITouchListItem m_parent;
  private GameObject m_showObject;

  public event Action OnScrollOutOfViewEvent;

  public MobileFriendListItem.TypeFlags Type { get; set; }

  public Bounds LocalBounds => this.m_localBounds;

  public bool Selectable => this.Type == MobileFriendListItem.TypeFlags.Friend || this.Type == MobileFriendListItem.TypeFlags.NearbyPlayer;

  public void SetParent(ITouchListItem parent) => this.m_parent = parent;

  public void SetShowObject(GameObject showobj) => this.m_showObject = showobj;

  public bool IsHeader => MobileFriendListItem.ItemIsHeader(this.Type);

  public static bool ItemIsHeader(MobileFriendListItem.TypeFlags typeFlags) => (typeFlags & MobileFriendListItem.TypeFlags.Header) != 0;

  public bool Visible
  {
    get => this.m_parent == null || this.m_parent.Visible;
    set
    {
      if ((UnityEngine.Object) this.m_showObject == (UnityEngine.Object) null || value == this.m_showObject.activeSelf)
        return;
      this.m_showObject.SetActive(value);
    }
  }

  private void Awake()
  {
    Transform parent = this.transform.parent;
    TransformProps worldTransformProps = TransformUtil.GetWorldTransformProps((Component) this.transform);
    this.transform.parent = (Transform) null;
    TransformUtil.Identity((Component) this.transform);
    this.m_localBounds = this.ComputeWorldBounds();
    this.transform.parent = parent;
    TransformUtil.CopyWorld((Component) this.transform, worldTransformProps);
  }

  public bool IsSelected()
  {
    FriendListUIElement component = this.GetComponent<FriendListUIElement>();
    return (UnityEngine.Object) component != (UnityEngine.Object) null && component.IsSelected();
  }

  public void Selected()
  {
    FriendListUIElement component = this.GetComponent<FriendListUIElement>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.SetSelected(true);
  }

  public void Unselected()
  {
    FriendListUIElement component = this.GetComponent<FriendListUIElement>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.SetSelected(false);
  }

  public Bounds ComputeWorldBounds() => TransformUtil.ComputeSetPointBounds(this.gameObject);

  public new T GetComponent<T>() where T : Component => base.GetComponent<T>();

  public void OnScrollOutOfView()
  {
    if (this.OnScrollOutOfViewEvent == null)
      return;
    this.OnScrollOutOfViewEvent();
  }

  public void OnPositionUpdate()
  {
  }

  [SpecialName]
  GameObject ITouchListItem.get_gameObject() => this.gameObject;

  [SpecialName]
  Transform ITouchListItem.get_transform() => this.transform;

  [Flags]
  public enum TypeFlags
  {
    FoundFiresideGathering = 512, // 0x00000200
    Request = 256, // 0x00000100
    CurrentFiresideGathering = 128, // 0x00000080
    FiresideGatheringPlayer = 64, // 0x00000040
    FiresideGatheringFooter = 32, // 0x00000020
    RecentPlayer = 16, // 0x00000010
    NearbyPlayer = 8,
    CurrentGame = 4,
    Friend = 2,
    Header = 1,
  }
}
