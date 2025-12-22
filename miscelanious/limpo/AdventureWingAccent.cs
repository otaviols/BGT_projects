using System.Collections.Generic;
using UnityEngine;

public class AdventureWingAccent : MonoBehaviour
{
  [SerializeField]
  public AdventureWing AssociatedWing;
  [SerializeField]
  public List<WingAccentMapping> WingAccentMappingList;

  private void Start()
  {
    if ((Object) this.AssociatedWing == (Object) null)
      return;
    GameObject objectFromWingId = this.GetAccentObjectFromWingId(this.AssociatedWing.GetWingId());
    if ((Object) objectFromWingId == (Object) null)
      return;
    objectFromWingId.SetActive(true);
  }

  private GameObject GetAccentObjectFromWingId(WingDbId wingId)
  {
    foreach (WingAccentMapping wingAccentMapping in this.WingAccentMappingList)
    {
      if (wingAccentMapping.WingId == wingId)
        return wingAccentMapping.AccentObject;
    }
    return (GameObject) null;
  }
}
