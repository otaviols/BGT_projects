using UnityEngine;

public class CardCrafting_WepPartSetParent : MonoBehaviour
{
  public GameObject m_Parent;
  public GameObject m_WepParts;

  private void Start()
  {
    if ((bool) (Object) this.m_Parent)
      return;
    Debug.LogError((object) "Animation Event Set Parent is null!");
    this.enabled = false;
  }

  public void SetParentWepParts()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_WepParts.transform.parent = this.m_Parent.transform;
  }
}
