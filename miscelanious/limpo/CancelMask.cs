using UnityEngine;

public class CancelMask : MonoBehaviour
{
  public GameObject m_root;

  public void Trigger()
  {
    this.m_root.gameObject.SetActive(false);
    Object.Destroy((Object) this.gameObject);
  }
}
