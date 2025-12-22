using UnityEngine;

public class CardCrafting_WepSetParent : MonoBehaviour
{
  public GameObject m_Parent;
  public Transform m_OrgParent;
  public GameObject m_ManaGem;
  public GameObject m_Portrait;
  public GameObject m_NameBanner;
  public GameObject m_RarityGem;
  public GameObject m_Discription;
  public GameObject m_Swords;
  public GameObject m_Shield;

  private void Start()
  {
    if ((bool) (Object) this.m_Parent)
      return;
    Debug.LogError((object) "Animation Event Set Parent is null!");
    this.enabled = false;
  }

  public void SetParentManaGem()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_ManaGem.transform.parent = this.m_Parent.transform;
  }

  public void SetParentPortrait()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_Portrait.transform.parent = this.m_Parent.transform;
  }

  public void SetParentNameBanner()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_NameBanner.transform.parent = this.m_Parent.transform;
  }

  public void SetParentRarityGem()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_RarityGem.transform.parent = this.m_Parent.transform;
  }

  public void SetParentDiscription()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_Discription.transform.parent = this.m_Parent.transform;
  }

  public void SetParentSwords()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_Swords.transform.parent = this.m_Parent.transform;
  }

  public void SetParentShield()
  {
    if (!(bool) (Object) this.m_Parent)
      return;
    this.m_Shield.transform.parent = this.m_Parent.transform;
  }

  public void SetBackToOrgParent()
  {
    if ((bool) (Object) this.m_OrgParent)
      this.m_ManaGem.transform.parent = this.m_OrgParent;
    this.m_Portrait.transform.parent = this.m_OrgParent;
    this.m_NameBanner.transform.parent = this.m_OrgParent;
    this.m_RarityGem.transform.parent = this.m_OrgParent;
    this.m_Discription.transform.parent = this.m_OrgParent;
    this.m_Swords.transform.parent = this.m_OrgParent;
    this.m_Shield.transform.parent = this.m_OrgParent;
  }
}
