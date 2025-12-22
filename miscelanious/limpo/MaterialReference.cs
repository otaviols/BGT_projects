using System;
using UnityEngine;

[Serializable]
public class MaterialReference
{
  [SerializeField]
  private string m_materialRef;
  [SerializeField]
  private string m_mainTextureRef;

  public string MaterialRef => this.m_materialRef;

  public Material GetMaterial()
  {
    if (string.IsNullOrWhiteSpace(this.m_materialRef))
    {
      Debug.LogWarning((object) string.Format("Material Reference used with no value"));
      return (Material) null;
    }
    if (AssetLoader.Get() == null)
      return (Material) null;
    Material material = AssetLoader.Get().LoadMaterial((AssetReference) this.m_materialRef);
    if ((UnityEngine.Object) material == (UnityEngine.Object) null)
      return (Material) null;
    if (!string.IsNullOrWhiteSpace(this.m_mainTextureRef))
    {
      if ((UnityEngine.Object) material.mainTexture == (UnityEngine.Object) null)
        material.mainTexture = AssetLoader.Get().LoadTexture((AssetReference) this.m_mainTextureRef);
      if ((UnityEngine.Object) material.mainTexture == (UnityEngine.Object) null)
        Debug.LogWarning((object) string.Format("Material Reference attempted to load texture and failed: \"{0}\"", (object) this.m_mainTextureRef));
    }
    return material;
  }
}
