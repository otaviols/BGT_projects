using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenEffectsPass : ScriptableRenderPass
{
  private string m_ProfilerTag;
  private FullScreenEffectsFeature.Settings m_settings;
  private FullScreenEffects m_fullScreenEffects;
  private int m_blur1 = Shader.PropertyToID("_Blur1");
  private RenderTargetIdentifier m_blur1Id;
  private int m_blur2 = Shader.PropertyToID("_Blur2");
  private RenderTargetIdentifier m_blur2Id;
  private int m_desaturationTexture = Shader.PropertyToID("_DesaturationTexture");
  private RenderTargetIdentifier m_desaturationTextureID;
  private int m_amountID = Shader.PropertyToID("_Amount");
  private int m_brightnessID = Shader.PropertyToID("_Brightness");
  private int m_desaturationID = Shader.PropertyToID("_Desaturation");
  private int m_maskTexID = Shader.PropertyToID("_MaskTex");
  private int m_colorID = Shader.PropertyToID("_Color");
  private int m_blurOffsetID = Shader.PropertyToID("_BlurOffset");
  private int m_blendTexID = Shader.PropertyToID("_BlendTex");
  private const int BLUR_BUFFER_SIZE = 512;
  private const float BLUR_SECOND_PASS_REDUCTION = 0.5f;
  private const float BLUR_PASS_1_OFFSET = 1f;
  private const float BLUR_PASS_2_OFFSET = 0.4f;
  private const float BLUR_PASS_3_OFFSET = -0.2f;

  public void Setup(
    string profilerTag,
    FullScreenEffectsFeature.Settings settings,
    FullScreenEffects fullScreenEffects)
  {
    this.m_ProfilerTag = profilerTag;
    this.m_settings = settings;
    this.m_fullScreenEffects = fullScreenEffects;
  }

  private void CalcTextureSize(
    int currentWidth,
    int currentHeight,
    out float outWidth,
    out float outHeight)
  {
    float num1 = (float) currentWidth;
    float num2 = (float) currentHeight;
    if ((double) num1 > (double) num2)
    {
      outWidth = 512f;
      outHeight = (float) (512.0 * ((double) num2 / (double) num1));
    }
    else
    {
      outWidth = (float) (512.0 * ((double) num1 / (double) num2));
      outHeight = 512f;
    }
  }

  public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
  {
    if (this.m_fullScreenEffects.BlurEnabled)
    {
      float outWidth = (float) cameraTextureDescriptor.width;
      float outHeight = (float) cameraTextureDescriptor.height;
      this.CalcTextureSize(cameraTextureDescriptor.width, cameraTextureDescriptor.height, out outWidth, out outHeight);
      cmd.GetTemporaryRT(this.m_blur1, (int) outWidth, (int) outHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
      this.m_blur1Id = new RenderTargetIdentifier(this.m_blur1);
      cmd.GetTemporaryRT(this.m_blur2, (int) ((double) outWidth * 0.5), (int) ((double) outHeight * 0.5), 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
      this.m_blur2Id = new RenderTargetIdentifier(this.m_blur2);
      this.m_settings.m_blurMaterial.SetFloat(this.m_brightnessID, 1f);
    }
    if (this.m_fullScreenEffects.DesaturationEnabled)
    {
      if (this.m_fullScreenEffects.BlurEnabled)
      {
        float num = 1f - this.m_fullScreenEffects.Desaturation;
        this.m_settings.m_desaturationMaterial.SetFloat(this.m_desaturationID, num * num * num);
        this.m_desaturationTextureID = new RenderTargetIdentifier(this.m_blur1);
      }
      else
      {
        this.m_settings.m_desaturationMaterial.SetFloat(this.m_desaturationID, 1f - this.m_fullScreenEffects.Desaturation);
        cmd.GetTemporaryRT(this.m_desaturationTexture, cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
        this.m_desaturationTextureID = new RenderTargetIdentifier(this.m_desaturationTexture);
      }
    }
    if (this.m_fullScreenEffects.VignettingEnable)
    {
      float num1 = this.m_fullScreenEffects.VignettingIntensity;
      if (this.m_fullScreenEffects.BlurEnabled)
      {
        float num2 = 1f - num1;
        num1 = 1f - num2 * num2 * num2;
      }
      this.m_settings.m_vignettingMaterial.SetFloat(this.m_amountID, num1);
    }
    if (this.m_fullScreenEffects.BlendToColorEnable)
    {
      this.m_settings.m_blendToColorMaterial.SetFloat(this.m_amountID, this.m_fullScreenEffects.BlendToColorAmount);
      this.m_settings.m_blendToColorMaterial.SetColor(this.m_colorID, this.m_fullScreenEffects.BlendColor);
    }
    this.ConfigureTarget(this.m_settings.cameraColorTarget, this.m_settings.cameraDepthTarget);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
  {
    CommandBuffer commandBuffer = CommandBufferPool.Get(this.m_ProfilerTag);
    commandBuffer.ClearRenderTarget(true, false, Color.black);
    context.ExecuteCommandBuffer(commandBuffer);
    commandBuffer.Clear();
    if (this.m_fullScreenEffects.DesaturationEnabled)
    {
      this.Blit(commandBuffer, this.m_settings.cameraColorTarget, this.m_desaturationTextureID, this.m_settings.m_desaturationMaterial);
      this.Blit(commandBuffer, this.m_desaturationTextureID, this.m_settings.cameraColorTarget, this.m_settings.m_desaturationMaterial, 1);
    }
    if (this.m_fullScreenEffects.VignettingEnable)
      this.Blit(commandBuffer, (RenderTargetIdentifier) (Texture) this.m_fullScreenEffects.m_VignettingMask, this.m_settings.cameraColorTarget, this.m_settings.m_vignettingMaterial);
    if (this.m_fullScreenEffects.BlurEnabled)
    {
      commandBuffer.SetGlobalFloat(this.m_blurOffsetID, 1f);
      this.Blit(commandBuffer, this.m_settings.cameraColorTarget, this.m_blur1Id, this.m_settings.m_blurMaterial);
      commandBuffer.SetGlobalFloat(this.m_blurOffsetID, 0.4f);
      this.Blit(commandBuffer, this.m_blur1Id, this.m_blur2Id, this.m_settings.m_blurMaterial);
      commandBuffer.SetGlobalFloat(this.m_blurOffsetID, -0.2f);
      if ((double) this.m_fullScreenEffects.BlurBlend >= 1.0)
      {
        this.Blit(commandBuffer, this.m_blur2Id, this.m_settings.cameraColorTarget, this.m_settings.m_blurMaterial);
      }
      else
      {
        this.Blit(commandBuffer, this.m_blur2Id, this.m_blur1Id, this.m_settings.m_blurMaterial);
        commandBuffer.SetGlobalFloat(this.m_amountID, this.m_fullScreenEffects.BlurBlend);
        this.Blit(commandBuffer, this.m_blur1Id, this.m_settings.cameraColorTarget, this.m_settings.m_blurBlendMaterial);
      }
    }
    if (this.m_fullScreenEffects.BlendToColorEnable)
      this.Blit(commandBuffer, (RenderTargetIdentifier) (Texture) this.m_fullScreenEffects.m_VignettingMask, this.m_settings.cameraColorTarget, this.m_settings.m_blendToColorMaterial);
    context.ExecuteCommandBuffer(commandBuffer);
    CommandBufferPool.Release(commandBuffer);
  }

  public override void FrameCleanup(CommandBuffer cmd)
  {
    if (this.m_fullScreenEffects.BlurEnabled)
    {
      cmd.ReleaseTemporaryRT(this.m_blur1);
      cmd.ReleaseTemporaryRT(this.m_blur2);
    }
    else
    {
      if (!this.m_fullScreenEffects.DesaturationEnabled)
        return;
      cmd.ReleaseTemporaryRT(this.m_desaturationTexture);
    }
  }
}
