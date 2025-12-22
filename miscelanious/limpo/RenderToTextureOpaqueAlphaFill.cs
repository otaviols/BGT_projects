using Blizzard.T5.AssetManager;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderToTextureOpaqueAlphaFill : RenderToTexturePostProcess
{
  private AssetHandle<Material> m_materialHandle;
  private Material m_alphaFillMaterial;
  private CommandBuffer m_commandBuffer;
  private Color m_clearColor = Color.clear;
  private RenderTexture m_sourceTexture;

  public void Init(int outputTextureWidth, int outputTextureHeight)
  {
    this.CreateAlphaFillMaterial();
    this.CreateCommandBuffer(outputTextureWidth, outputTextureHeight);
  }

  public void Init(
    int outputTextureWidth,
    int outputTextureHeight,
    Color clearColor,
    RenderTexture source)
  {
    this.m_clearColor = clearColor;
    this.m_sourceTexture = source;
    this.Init(outputTextureWidth, outputTextureHeight);
  }

  public void End() => this.m_materialHandle.Dispose();

  public bool IsUsedBy(DiamondRenderToTexture r2t) => r2t.m_OpaqueObjectAlphaFill;

  public void AddCommandBuffers() => Graphics.ExecuteCommandBuffer(this.m_commandBuffer);

  private void CreateAlphaFillMaterial()
  {
    this.m_materialHandle = AssetLoader.Get().LoadAsset<Material>((AssetReference) "ARTT_AlphaOpaqueFill.mat:0ff23894e37f8374a9dda7e852f9bcd3");
    this.m_alphaFillMaterial = this.m_materialHandle.Asset;
  }

  private void CreateCommandBuffer(int outputTextureWidth, int outputTextureHeight)
  {
    this.m_commandBuffer = new CommandBuffer();
    this.m_commandBuffer.name = "AlphaOpaqueBlend";
    int id = Shader.PropertyToID("_TempAlphaTex");
    this.m_commandBuffer.GetTemporaryRT(id, outputTextureWidth, outputTextureHeight, 0, FilterMode.Bilinear);
    this.m_commandBuffer.SetGlobalColor("_ClearColor", this.m_clearColor);
    this.m_commandBuffer.Blit((Texture) this.m_sourceTexture, (RenderTargetIdentifier) id, this.m_alphaFillMaterial);
    this.m_commandBuffer.Blit((RenderTargetIdentifier) id, (RenderTargetIdentifier) (Texture) this.m_sourceTexture);
    this.m_commandBuffer.ReleaseTemporaryRT(id);
  }
}
