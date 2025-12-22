using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class PremiumMaterialSwitcher : MonoBehaviour
{
  public Material[] m_PremiumMaterials;
  private List<Material> OrgMaterials;
  private Renderer m_renderer;

  private void Start() => this.m_renderer = this.GetComponent<Renderer>();

  public void SetToPremium(int premium)
  {
    if (premium < 1)
    {
      List<Material> materials = this.m_renderer.GetMaterials();
      if (materials == null || this.OrgMaterials == null)
        return;
      for (int index = 0; index < this.m_PremiumMaterials.Length && index < materials.Count; ++index)
      {
        if (!((Object) this.m_PremiumMaterials[index] == (Object) null))
          materials[index] = this.OrgMaterials[index];
      }
      this.m_renderer.SetMaterials(materials);
      this.OrgMaterials = (List<Material>) null;
    }
    else
    {
      if (this.m_PremiumMaterials.Length < 1)
        return;
      if (this.OrgMaterials == null)
        this.OrgMaterials = this.m_renderer.GetMaterials();
      List<Material> materials = this.m_renderer.GetMaterials();
      for (int index = 0; index < this.m_PremiumMaterials.Length && index < materials.Count; ++index)
      {
        if (!((Object) this.m_PremiumMaterials[index] == (Object) null))
          materials[index] = this.m_PremiumMaterials[index];
      }
      this.m_renderer.SetMaterials(materials);
    }
  }
}
