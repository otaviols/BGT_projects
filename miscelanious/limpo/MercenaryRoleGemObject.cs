using System;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryRoleGemObject : GemObject
{
  public List<MercenaryRoleGemObject.RoleGemObjectMapping> m_roleGemObjects;

  public void SetRole(TAG_ROLE role)
  {
    foreach (MercenaryRoleGemObject.RoleGemObjectMapping roleGemObject in this.m_roleGemObjects)
      roleGemObject.m_roleGemObject.SetActive(roleGemObject.m_role == role);
  }

  [Serializable]
  public class RoleGemObjectMapping
  {
    [SerializeField]
    public TAG_ROLE m_role;
    [SerializeField]
    public GameObject m_roleGemObject;
  }
}
