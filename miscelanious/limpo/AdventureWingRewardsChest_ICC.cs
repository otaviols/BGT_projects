using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureWingRewardsChest_ICC : MonoBehaviour
{
  [CustomEditField(Sections = "ICC")]
  public List<GameObject> m_chests = new List<GameObject>();

  public void SetBigChestColliderEnabled(bool isEnabled) => this.GetComponent<Collider>().enabled = isEnabled;

  public bool ActivateChest(int index)
  {
    if (index >= this.m_chests.Count)
      return false;
    for (int index1 = 0; index1 < this.m_chests.Count; ++index1)
      this.m_chests[index1].SetActive(index1 == index);
    return true;
  }
}
