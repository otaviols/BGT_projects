using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class CardNerfGlows : MonoBehaviour
{
  [SerializeField]
  private Material m_buffMaterial;
  [SerializeField]
  private Material m_nerfMaterial;
  [SerializeField]
  private GameObject m_attack;
  [SerializeField]
  private GameObject m_health;
  [SerializeField]
  private GameObject m_manaCost;
  [SerializeField]
  private GameObject m_rarityGem;
  [SerializeField]
  private GameObject m_art;
  [SerializeField]
  private GameObject m_cardText;
  [SerializeField]
  private GameObject m_cardName;
  [SerializeField]
  private GameObject m_race;
  [SerializeField]
  private GameObject m_armor;

  private void Awake() => this.HideAll();

  public void SetGlowsForCard(List<CardChangeDbfRecord> cardChanges)
  {
    this.HideAll();
    if (cardChanges == null)
      return;
    foreach (CardChangeDbfRecord cardChange in cardChanges)
    {
      if (cardChange.ChangeType == Assets.CardChange.ChangeType.BUFF || cardChange.ChangeType == Assets.CardChange.ChangeType.NERF)
      {
        Material material = cardChange.ChangeType == Assets.CardChange.ChangeType.BUFF ? this.m_buffMaterial : this.m_nerfMaterial;
        switch (cardChange.TagId)
        {
          case 45:
            this.m_health.GetComponent<Renderer>().SetMaterial(material);
            this.m_health.SetActive(true);
            continue;
          case 47:
            this.m_attack.GetComponent<Renderer>().SetMaterial(material);
            this.m_attack.SetActive(true);
            continue;
          case 48:
            this.m_manaCost.GetComponent<Renderer>().SetMaterial(material);
            this.m_manaCost.SetActive(true);
            continue;
          case 184:
            this.m_cardText.GetComponent<Renderer>().SetMaterial(material);
            this.m_cardText.SetActive(true);
            continue;
          case 292:
            this.m_armor.GetComponent<Renderer>().SetMaterial(material);
            this.m_armor.SetActive(true);
            continue;
          default:
            continue;
        }
      }
    }
  }

  private void HideAll()
  {
    foreach (Component component in this.transform)
      component.gameObject.SetActive(false);
  }
}
