using UnityEngine;

[CustomEditClass]
[RequireComponent(typeof (BoxCollider), typeof (PegUIElement))]
public class UIBScrollableTrack : MonoBehaviour
{
  public UIBScrollable m_parentScrollbar;
  public GameObject m_scrollTrack;
  public Vector3 m_showRotation = Vector3.zero;
  public Vector3 m_hideRotation = new Vector3(0.0f, 0.0f, 180f);
  public float m_rotateAnimationTime = 0.5f;
  private bool m_lastEnabled;

  private void Awake()
  {
    if ((Object) this.m_parentScrollbar == (Object) null)
      Debug.LogError((object) "Parent scroll bar not set!");
    else
      this.m_parentScrollbar.AddEnableScrollListener(new UIBScrollable.EnableScroll(this.OnScrollEnabled));
  }

  private void OnEnable()
  {
    if (!((Object) this.m_scrollTrack != (Object) null))
      return;
    this.m_lastEnabled = this.m_parentScrollbar.IsEnabled();
    this.m_scrollTrack.transform.localEulerAngles = this.m_lastEnabled ? this.m_showRotation : this.m_hideRotation;
  }

  private void OnScrollEnabled(bool enabled)
  {
    if ((Object) this.m_scrollTrack == (Object) null || !this.m_scrollTrack.activeInHierarchy || this.m_lastEnabled == enabled)
      return;
    this.m_lastEnabled = enabled;
    Vector3 vector3_1;
    Vector3 vector3_2;
    if (enabled)
    {
      vector3_1 = this.m_hideRotation;
      vector3_2 = this.m_showRotation;
    }
    else
    {
      vector3_1 = this.m_showRotation;
      vector3_2 = this.m_hideRotation;
    }
    this.m_scrollTrack.transform.localEulerAngles = vector3_1;
    iTween.StopByName(this.m_scrollTrack, "rotate");
    iTween.RotateTo(this.m_scrollTrack, iTween.Hash((object) "rotation", (object) vector3_2, (object) "islocal", (object) true, (object) "time", (object) this.m_rotateAnimationTime, (object) "name", (object) "rotate"));
  }
}
