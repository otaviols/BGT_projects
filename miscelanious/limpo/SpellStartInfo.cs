using System;
using UnityEngine;

[Serializable]
public class SpellStartInfo
{
  public bool m_Enabled = true;
  public Spell m_Prefab;
  public bool m_UseSuperSpellLocation = true;
  public bool m_DeathAfterAllMissilesFire = true;
  public bool m_AdjustRotation;
  public Vector3 m_StartRotationAdjustment = Vector3.zero;
}
