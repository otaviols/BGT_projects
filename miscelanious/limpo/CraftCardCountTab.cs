using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CraftCardCountTab : MonoBehaviour
{
  public UberText m_count;
  public UberText m_x;
  public UberText m_plus;
  public GameObject m_shadow;
  public Color m_normalColor;
  public Color m_goldenColor;
  public Material m_normalMaterial;
  public Material m_goldenMaterial;
  public MeshRenderer m_countTab;

  public void UpdateText(int numCopies, TAG_PREMIUM premium)
  {
    if (premium == TAG_PREMIUM.DIAMOND)
      this.gameObject.SetActive(false);
    if (numCopies > 9)
    {
      this.m_count.Text = "9";
      this.m_plus.gameObject.SetActive(true);
    }
    else
    {
      if (numCopies >= 2)
      {
        this.m_shadow.SetActive(true);
        this.m_shadow.GetComponent<Animation>().Play("Crafting2ndCardShadow");
      }
      else
        this.m_shadow.SetActive(false);
      this.m_count.TextColor = this.m_normalColor;
      this.m_plus.TextColor = this.m_normalColor;
      this.m_x.TextColor = this.m_normalColor;
      this.m_countTab.SetMaterial(this.m_normalMaterial);
      this.m_count.Text = numCopies.ToString();
      this.m_plus.gameObject.SetActive(false);
    }
  }
}
