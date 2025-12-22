using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CollectionSetFilterDropdownToggle : PegUIElement
{
  public MeshRenderer m_currentIconQuad;
  public MeshRenderer m_buttonMesh;
  public MeshRenderer m_buttonMeshBackground;
  public Material m_normalBackgroundMaterial;
  public Material m_tavernBrawlBackgroundMaterial;
  public Material m_duelsBackgroundMaterial;

  public void SetToggleIcon(Texture texture, Vector2 materialOffset)
  {
    Material material = this.m_currentIconQuad.GetMaterial();
    material.SetTexture("_MainTex", texture);
    material.SetTextureOffset("_MainTex", materialOffset);
  }

  public void SetEnabledVisual(bool enabled)
  {
    if ((Object) this.m_buttonMesh == (Object) null)
      return;
    this.m_buttonMesh.GetMaterial().SetFloat("_Desaturate", enabled ? 0.0f : 1f);
  }

  public void SetButtonBackgroundMaterial()
  {
    if (SceneMgr.Get().IsInTavernBrawlMode())
      this.m_buttonMeshBackground.SetMaterial(this.m_tavernBrawlBackgroundMaterial);
    else if (SceneMgr.Get().IsInDuelsMode())
      this.m_buttonMeshBackground.SetMaterial(this.m_duelsBackgroundMaterial);
    else
      this.m_buttonMeshBackground.SetMaterial(this.m_normalBackgroundMaterial);
  }
}
