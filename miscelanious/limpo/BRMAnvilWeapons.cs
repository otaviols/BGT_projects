using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class BRMAnvilWeapons : MonoBehaviour
{
  public List<BRMAnvilWeapons.AnvilWeapon> m_Weapons;
  private int m_LastWeaponIndex;

  public void RandomWeaponEvent()
  {
    List<int> intList = new List<int>();
    for (int index = 0; index < this.m_Weapons.Count; ++index)
    {
      if (index != this.m_LastWeaponIndex)
        intList.Add(index);
    }
    if (this.m_Weapons.Count <= 0 || intList.Count <= 0)
      return;
    int index1 = UnityEngine.Random.Range(0, intList.Count);
    BRMAnvilWeapons.AnvilWeapon weapon = this.m_Weapons[intList[index1]];
    this.m_LastWeaponIndex = intList[index1];
    weapon.m_FSM.SendEvent(weapon.m_Events[this.RandomSubWeapon(weapon)]);
  }

  public int RandomSubWeapon(BRMAnvilWeapons.AnvilWeapon weapon)
  {
    List<int> intList = new List<int>();
    for (int index = 0; index < weapon.m_Events.Count; ++index)
    {
      if (index != weapon.m_CurrentWeaponIndex)
        intList.Add(index);
    }
    int index1 = UnityEngine.Random.Range(0, intList.Count);
    weapon.m_CurrentWeaponIndex = intList[index1];
    return intList[index1];
  }

  [Serializable]
  public class AnvilWeapon
  {
    public PlayMakerFSM m_FSM;
    public List<string> m_Events;
    [HideInInspector]
    public int m_CurrentWeaponIndex;
  }
}
