using UnityEngine;

[CustomEditClass]
[ExecuteAlways]
public class UIBFollowObject : MonoBehaviour
{
  public GameObject m_rootObject;
  public GameObject m_objectToFollow;
  public Vector3 m_offset;
  public bool m_useWorldOffset;

  public void UpdateFollowPosition()
  {
    if ((Object) this.m_rootObject == (Object) null || (Object) this.m_objectToFollow == (Object) null)
      return;
    Vector3 position = this.m_objectToFollow.transform.position;
    if ((double) this.m_offset.sqrMagnitude > 0.0)
      position += (Vector3) (this.m_objectToFollow.transform.localToWorldMatrix * (Vector4) this.m_offset);
    this.m_rootObject.transform.position = position;
  }
}
