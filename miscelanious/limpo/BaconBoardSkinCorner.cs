using System.Collections.Generic;
using UnityEngine;

public class BaconBoardSkinCorner : MonoBehaviour
{
  public GameObject m_TopContainer;
  public GameObject m_BackContainer;
  private List<GameObject> m_CopiedBackside = new List<GameObject>();

  public void CopyToBackside(GameObject source)
  {
    foreach (Object @object in this.m_CopiedBackside)
      Object.Destroy(@object);
    this.m_CopiedBackside.Clear();
    if ((Object) source == (Object) null)
      return;
    foreach (Component component in source.transform)
      this.m_CopiedBackside.Add(Object.Instantiate<GameObject>(component.gameObject, this.m_BackContainer.transform));
  }

  public void OnDestroy()
  {
    foreach (Object @object in this.m_CopiedBackside)
      Object.Destroy(@object);
    this.m_CopiedBackside.Clear();
  }
}
