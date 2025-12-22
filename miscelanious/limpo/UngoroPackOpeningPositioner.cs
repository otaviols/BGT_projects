using System;
using System.Collections.Generic;
using UnityEngine;

public class UngoroPackOpeningPositioner : MonoBehaviour
{
  public List<UngoroPackOpeningPositioner.PositioningBoneSet> m_PositioningBoneSets;
  public List<UngoroPackOpeningPositioner.PositioningBoneSet> m_PositioningBoneSetsMobile;
  public Transform m_PackSpawningBone;

  public List<Transform> GetPositioningBonesForCardCount(int cardCount)
  {
    if (cardCount <= 0)
      return (List<Transform>) null;
    if (cardCount - 1 >= this.m_PositioningBoneSets.Count)
      return (List<Transform>) null;
    return (bool) UniversalInputManager.UsePhoneUI ? this.m_PositioningBoneSetsMobile[cardCount - 1].m_Bones : this.m_PositioningBoneSets[cardCount - 1].m_Bones;
  }

  [Serializable]
  public class PositioningBoneSet
  {
    public List<Transform> m_Bones;
  }
}
