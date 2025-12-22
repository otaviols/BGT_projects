using UnityEngine;

public class AnimationEventSetParent : MonoBehaviour
{
  public GameObject m_Parent;

  private void Start()
  {
    if ((bool) (Object) this.m_Parent)
      return;
    Debug.LogError((object) "Animation Event Set Parent is null!");
    this.enabled = false;
  }

  public void SetParent()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.transform.parent = this.m_Parent.transform;
  }
}
