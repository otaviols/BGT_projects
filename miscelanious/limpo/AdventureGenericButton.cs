using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

[CustomEditClass]
public class AdventureGenericButton : PegUIElement
{
  private const string s_DefaultPortraitMaterialtextureAssetPath = "_MainTex";
  private const int s_DefaultPortraitMaterialIndex = 1;
  [CustomEditField(Sections = "Portrait Settings")]
  public MeshRenderer m_PortraitRenderer;
  [CustomEditField(Sections = "Portrait Settings")]
  public AdventureGenericButton.MaterialProperties m_MaterialProperties = new AdventureGenericButton.MaterialProperties();
  [CustomEditField(Sections = "Border Settings")]
  public MeshRenderer m_BorderRenderer;
  [CustomEditField(Sections = "Border Settings")]
  public AdventureGenericButton.MaterialProperties m_BorderMaterialProperties = new AdventureGenericButton.MaterialProperties();
  [CustomEditField(Sections = "Text Settings")]
  public UberText m_ButtonTextObject;
  [CustomEditField(Sections = "Text Settings")]
  public Color m_NormalTextColor;
  public Color m_DisabledTextColor;
  private bool m_PortraitLoaded = true;
  private AssetHandle<Texture> m_portraitTexture;

  protected override void OnDestroy()
  {
    AssetHandle.SafeDispose<Texture>(ref this.m_portraitTexture);
    base.OnDestroy();
  }

  public bool IsPortraitLoaded() => this.m_PortraitLoaded;

  public void SetDesaturate(bool desaturate)
  {
    if (!this.CheckValidMaterialProperties(this.m_MaterialProperties) || !this.CheckValidMaterialProperties(this.m_BorderMaterialProperties))
      return;
    RendererExtension.GetMaterial((Renderer) this.m_PortraitRenderer, this.m_MaterialProperties.m_MaterialIndex).SetFloat("_Desaturate", desaturate ? 1f : 0.0f);
    RendererExtension.GetMaterial((Renderer) this.m_BorderRenderer, this.m_BorderMaterialProperties.m_MaterialIndex).SetFloat("_Desaturate", desaturate ? 1f : 0.0f);
    this.m_ButtonTextObject.TextColor = desaturate ? this.m_DisabledTextColor : this.m_NormalTextColor;
  }

  public void SetContrast(float contrast)
  {
    if (!this.CheckValidMaterialProperties(this.m_MaterialProperties) || !this.CheckValidMaterialProperties(this.m_BorderMaterialProperties))
      return;
    RendererExtension.GetMaterial((Renderer) this.m_PortraitRenderer, this.m_MaterialProperties.m_MaterialIndex).SetFloat("_Contrast", contrast);
    RendererExtension.GetMaterial((Renderer) this.m_BorderRenderer, this.m_BorderMaterialProperties.m_MaterialIndex).SetFloat("_Contrast", contrast);
  }

  public void SetButtonText(string str)
  {
    if ((UnityEngine.Object) this.m_ButtonTextObject == (UnityEngine.Object) null)
      return;
    this.m_ButtonTextObject.Text = str;
  }

  public void SetPortraitTexture(string textureAssetPath) => this.SetPortraitTexture(textureAssetPath, this.m_MaterialProperties.m_MaterialIndex, this.m_MaterialProperties.m_MaterialPropertyName);

  public void SetPortraitTexture(string textureAssetPath, int index, string mattexprop)
  {
    switch (textureAssetPath)
    {
      case "":
        break;
      case null:
        break;
      default:
        AdventureGenericButton.MaterialProperties materialProperties = new AdventureGenericButton.MaterialProperties()
        {
          m_MaterialIndex = index,
          m_MaterialPropertyName = mattexprop
        };
        if (!this.CheckValidMaterialProperties(materialProperties))
          break;
        this.m_PortraitLoaded = false;
        AssetLoader.Get().LoadAsset<Texture>((AssetReference) textureAssetPath, new AssetHandleCallback<Texture>(this.ApplyPortraitTexture), (object) materialProperties);
        break;
    }
  }

  public void SetPortraitTiling(Vector2 tiling) => this.SetPortraitTiling(tiling, this.m_MaterialProperties.m_MaterialIndex, this.m_MaterialProperties.m_MaterialPropertyName);

  public void SetPortraitTiling(Vector2 tiling, int index, string mattexprop)
  {
    AdventureGenericButton.MaterialProperties matprop = new AdventureGenericButton.MaterialProperties()
    {
      m_MaterialIndex = index,
      m_MaterialPropertyName = mattexprop
    };
    if (!this.CheckValidMaterialProperties(matprop))
      return;
    RendererExtension.GetMaterial((Renderer) this.m_PortraitRenderer, matprop.m_MaterialIndex).SetTextureScale(matprop.m_MaterialPropertyName, tiling);
  }

  public void SetPortraitOffset(Vector2 offset) => this.SetPortraitOffset(offset, this.m_MaterialProperties.m_MaterialIndex, this.m_MaterialProperties.m_MaterialPropertyName);

  public void SetPortraitOffset(Vector2 offset, int index, string mattexprop)
  {
    AdventureGenericButton.MaterialProperties matprop = new AdventureGenericButton.MaterialProperties()
    {
      m_MaterialIndex = index,
      m_MaterialPropertyName = mattexprop
    };
    if (!this.CheckValidMaterialProperties(matprop))
      return;
    RendererExtension.GetMaterial((Renderer) this.m_PortraitRenderer, matprop.m_MaterialIndex).SetTextureOffset(matprop.m_MaterialPropertyName, offset);
  }

  private void ApplyPortraitTexture(
    AssetReference assetRef,
    AssetHandle<Texture> loadedTexture,
    object userdata)
  {
    using (loadedTexture)
    {
      this.m_PortraitLoaded = true;
      AdventureGenericButton.MaterialProperties materialProperties = userdata as AdventureGenericButton.MaterialProperties;
      if (!(bool) loadedTexture)
      {
        Debug.LogError((object) string.Format("Unable to load portrait texture {0}.", (object) assetRef.ToString()), (UnityEngine.Object) this);
      }
      else
      {
        AssetHandle.Set<Texture>(ref this.m_portraitTexture, loadedTexture);
        RendererExtension.GetMaterial((Renderer) this.m_PortraitRenderer, materialProperties.m_MaterialIndex).SetTexture(materialProperties.m_MaterialPropertyName, (Texture) this.m_portraitTexture);
      }
    }
  }

  private bool CheckValidMaterialProperties(AdventureGenericButton.MaterialProperties matprop)
  {
    if ((UnityEngine.Object) this.m_PortraitRenderer == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "No portrait mesh renderer set.");
      return false;
    }
    if (matprop.m_MaterialIndex < RendererExtension.GetMaterials((Renderer) this.m_PortraitRenderer).Count)
      return true;
    Debug.LogError((object) string.Format("Unable to find material index {0}", (object) matprop.m_MaterialIndex));
    return false;
  }

  [Serializable]
  public class MaterialProperties
  {
    public int m_MaterialIndex = 1;
    public string m_MaterialPropertyName = "_MainTex";
  }
}
