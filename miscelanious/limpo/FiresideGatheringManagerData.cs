using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class FiresideGatheringManagerData : ScriptableObject
{
  public Vector3_MobileOverride m_nearbyFiresidePopupOffset;
  public Vector3_MobileOverride m_nearbyFiresidePopupScale;
  public Vector3_MobileOverride m_nearbyFiresidePopupRotation;
  public Vector3_MobileOverride m_returnToFsgFriendListPopupOffset;
  public Vector3_MobileOverride m_signPosition;
  public Vector3_MobileOverride m_signScale;
  public List<FiresideGatheringManagerData.SignTypeMapping> m_signTypeMapping = new List<FiresideGatheringManagerData.SignTypeMapping>();
  public bool m_hasSeenReturnToFSGSceneTooltip;

  [Serializable]
  public class SignTypeMapping
  {
    public TavernSignType m_type;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string m_prefabName;
  }
}
