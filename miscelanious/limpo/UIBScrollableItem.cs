using Hearthstone.UI;
using UnityEngine;

public class UIBScrollableItem : MonoBehaviour
{
  [Tooltip("Fixed: Use values for Size and Offset defined at edit-time.\n\nWidgetBoundsLocal: Match the size and position of this object's Widget bounds.\n\nWidgetBoundsIncludeChildren: Encapsulate the Widget bounds defined on this object and its children.")]
  public UIBScrollableItem.BoundsMode m_boundsMode;
  public Vector3 m_offset = Vector3.zero;
  public Vector3 m_size = Vector3.one;
  public UIBScrollableItem.ActiveState m_active;
  private UIBScrollableItem.ActiveStateCallback m_activeStateCallback;
  private Vector3[] m_boundsPointTempVector = new Vector3[8];
  private UIBScrollable.IContent m_ScrollableParent;

  public void Awake() => this.UpdateScrollableParent();

  public void OnEnable() => this.UpdateScrollableParent();

  public void UpdateScrollableParent()
  {
    this.m_ScrollableParent = this.GetComponentInParent<UIBScrollable.IContent>();
    if (this.m_ScrollableParent == null)
      return;
    this.m_ScrollableParent.Scrollable.RegisterScrollableItem(this);
  }

  private void OnDestroy()
  {
    if (this.m_ScrollableParent == null)
      return;
    this.m_ScrollableParent.Scrollable.RemoveScrollableItem(this);
  }

  public void SetScrollableParent(UIBScrollable.IContent parent) => this.m_ScrollableParent = parent;

  public bool IsActive()
  {
    if (this.m_activeStateCallback != null)
      return this.m_activeStateCallback();
    if (this.m_active == UIBScrollableItem.ActiveState.Active)
      return true;
    return this.m_active == UIBScrollableItem.ActiveState.UseHierarchy && this.gameObject.activeInHierarchy;
  }

  public void SetCustomActiveState(UIBScrollableItem.ActiveStateCallback callback) => this.m_activeStateCallback = callback;

  public OrientedBounds GetOrientedBounds()
  {
    Transform transform = this.transform;
    this.UpdateBounds(transform);
    Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
    return new OrientedBounds()
    {
      Origin = transform.position + (Vector3) (localToWorldMatrix * (Vector4) this.m_offset),
      Extents = new Vector3[3]
      {
        (Vector3) (localToWorldMatrix * (Vector4) new Vector3(this.m_size.x * 0.5f, 0.0f, 0.0f)),
        (Vector3) (localToWorldMatrix * (Vector4) new Vector3(0.0f, this.m_size.y * 0.5f, 0.0f)),
        (Vector3) (localToWorldMatrix * (Vector4) new Vector3(0.0f, 0.0f, this.m_size.z * 0.5f))
      }
    };
  }

  public void GetWorldBounds(out Vector3 min, out Vector3 max)
  {
    min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    this.UpdateBoundsPoints();
    for (int index = 0; index < 8; ++index)
    {
      Vector3 vector3 = this.m_boundsPointTempVector[index];
      min.x = Mathf.Min(vector3.x, min.x);
      min.y = Mathf.Min(vector3.y, min.y);
      min.z = Mathf.Min(vector3.z, min.z);
      max.x = Mathf.Max(vector3.x, max.x);
      max.y = Mathf.Max(vector3.y, max.y);
      max.z = Mathf.Max(vector3.z, max.z);
    }
  }

  private void UpdateBoundsPoints()
  {
    Transform transform = this.transform;
    this.UpdateBounds(transform);
    Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
    Vector3 vector3_1 = (Vector3) (localToWorldMatrix * (Vector4) new Vector3(this.m_size.x * 0.5f, 0.0f, 0.0f));
    Vector3 vector3_2 = (Vector3) (localToWorldMatrix * (Vector4) new Vector3(0.0f, this.m_size.y * 0.5f, 0.0f));
    Vector3 vector3_3 = (Vector3) (localToWorldMatrix * (Vector4) new Vector3(0.0f, 0.0f, this.m_size.z * 0.5f));
    Vector3 vector3_4 = transform.position + (Vector3) (localToWorldMatrix * (Vector4) this.m_offset);
    Vector3 vector3_5 = vector3_4 + vector3_1;
    Vector3 vector3_6 = vector3_4 - vector3_1;
    Vector3 vector3_7 = vector3_2 + vector3_3;
    Vector3 vector3_8 = vector3_2 - vector3_3;
    this.m_boundsPointTempVector[0] = vector3_5 + vector3_7;
    this.m_boundsPointTempVector[1] = vector3_5 + vector3_8;
    this.m_boundsPointTempVector[2] = vector3_5 - vector3_7;
    this.m_boundsPointTempVector[3] = vector3_5 - vector3_8;
    this.m_boundsPointTempVector[4] = vector3_6 + vector3_7;
    this.m_boundsPointTempVector[5] = vector3_6 + vector3_8;
    this.m_boundsPointTempVector[6] = vector3_6 - vector3_7;
    this.m_boundsPointTempVector[7] = vector3_6 - vector3_8;
  }

  private void UpdateBounds(Transform transform)
  {
    if (this.m_boundsMode == UIBScrollableItem.BoundsMode.WidgetBoundsLocal)
    {
      if (!((Object) this.GetComponent<WidgetTransform>() != (Object) null))
        return;
      Bounds ofWidgetTransform = WidgetTransform.GetLocalBoundsOfWidgetTransform(transform);
      this.m_size = ofWidgetTransform.size;
      this.m_offset = ofWidgetTransform.center;
    }
    else
    {
      if (this.m_boundsMode != UIBScrollableItem.BoundsMode.WidgetBoundsIncludeChildren)
        return;
      Bounds widgetTransforms = WidgetTransform.GetBoundsOfWidgetTransforms(transform, transform.worldToLocalMatrix);
      this.m_size = widgetTransforms.size;
      this.m_offset = widgetTransforms.center;
    }
  }

  public delegate bool ActiveStateCallback();

  public enum ActiveState
  {
    Active,
    Inactive,
    UseHierarchy,
  }

  public enum BoundsMode
  {
    Fixed,
    WidgetBoundsLocal,
    WidgetBoundsIncludeChildren,
  }
}
