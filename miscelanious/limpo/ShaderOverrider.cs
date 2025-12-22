using Blizzard.T5.MaterialService;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShaderOverrider : MonoBehaviour
{
  [SerializeField]
  private bool m_override;
  [SerializeField]
  private GameObject m_target;
  [SerializeField]
  protected Shader m_shaderOverride;
  [SerializeField]
  private List<ShaderOverrider.ShaderTweak> m_tweaks = new List<ShaderOverrider.ShaderTweak>();
  private Dictionary<Renderer, Material> m_rendererMapping = new Dictionary<Renderer, Material>();
  private Dictionary<Material, Material> m_materialOverrides = new Dictionary<Material, Material>();

  private void OnValidate()
  {
    if ((UnityEngine.Object) this.m_target == (UnityEngine.Object) null)
      this.m_target = this.gameObject;
    this.Apply(this.m_override);
  }

  private void OnDestroy() => this.DestroyInstancedMaterials();

  [Overridable]
  public bool Override
  {
    get => this.m_override;
    set
    {
      this.m_override = value;
      this.Apply(this.m_override);
    }
  }

  private void Apply(bool applied)
  {
    if (applied)
    {
      this.InstantiateMaterials();
      this.ApplyShaderOverrides();
    }
    else
      this.RestoreOriginalMaterials();
  }

  private void InstantiateMaterials()
  {
    if ((UnityEngine.Object) this.m_target == (UnityEngine.Object) null)
      return;
    IMaterialService materialService = ServiceManager.Get<IMaterialService>();
    foreach (Renderer componentsInChild in this.m_target.GetComponentsInChildren<Renderer>(true))
    {
      Material sharedMaterial;
      if (!this.m_rendererMapping.TryGetValue(componentsInChild, out sharedMaterial))
      {
        sharedMaterial = componentsInChild.GetSharedMaterial();
        if (!((UnityEngine.Object) sharedMaterial == (UnityEngine.Object) null))
        {
          this.m_rendererMapping[componentsInChild] = sharedMaterial;
          Material material;
          if (!this.m_materialOverrides.TryGetValue(sharedMaterial, out material))
          {
            material = UnityEngine.Object.Instantiate<Material>(sharedMaterial);
            materialService.IgnoreMaterial(material);
            this.m_materialOverrides[sharedMaterial] = material;
          }
          componentsInChild.SetSharedMaterial(material);
        }
      }
    }
  }

  private void ApplyShaderOverrides()
  {
    foreach (KeyValuePair<Material, Material> materialOverride in this.m_materialOverrides)
    {
      Material material = materialOverride.Value;
      if ((UnityEngine.Object) material.shader != (UnityEngine.Object) this.m_shaderOverride && (UnityEngine.Object) this.m_shaderOverride != (UnityEngine.Object) null)
        material.shader = this.m_shaderOverride;
      foreach (ShaderOverrider.ShaderTweak tweak in this.m_tweaks)
      {
        if (!material.HasProperty(tweak.parameter))
          Debug.LogWarningFormat("Property '{0}' does not exist on shader '{1}'", (object) tweak.parameter, (object) material.shader.name);
        else
          material.SetFloat(tweak.parameter, tweak.value);
      }
    }
  }

  private void RestoreOriginalMaterials()
  {
    foreach (KeyValuePair<Renderer, Material> keyValuePair in this.m_rendererMapping)
      keyValuePair.Key.SetSharedMaterial(keyValuePair.Value);
    this.m_rendererMapping.Clear();
  }

  private void DestroyInstancedMaterials()
  {
    foreach (KeyValuePair<Material, Material> materialOverride in this.m_materialOverrides)
      UnityEngine.Object.Destroy((UnityEngine.Object) materialOverride.Value);
    this.m_materialOverrides.Clear();
  }

  [Serializable]
  private class ShaderTweak
  {
    public string parameter;
    public float value;
  }
}
