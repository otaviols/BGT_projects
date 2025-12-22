using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Magic Effect Material", menuName = "ScriptableObjects/Legendary Hero/Magic Effect Material")]
public class LegendaryHeroMagicEffectMaterial : ScriptableObject
{
  [Header("Mesh")]
  public Mesh Mesh;
  public LegendaryHeroMagicEffectMaterial.NoiseData Noise;
  [Header("Shader")]
  public Shader Shader;
  [Header("Effects")]
  public LegendaryHeroMagicEffectMaterial.StreamData Stream;
  public LegendaryHeroMagicEffectMaterial.SoulsData Souls;
  private static readonly int s_stream1TextureID = Shader.PropertyToID("_Stream1Tex");
  private static readonly int s_stream2TextureID = Shader.PropertyToID("_Stream2Tex");
  private static readonly int s_stream1ColorLowID = Shader.PropertyToID("_Stream1ColorLow");
  private static readonly int s_stream2ColorLowID = Shader.PropertyToID("_Stream2ColorLow");
  private static readonly int s_stream1ColorHighID = Shader.PropertyToID("_Stream1ColorHigh");
  private static readonly int s_stream2ColorHighID = Shader.PropertyToID("_Stream2ColorHigh");
  private static readonly int s_soulsTextureID = Shader.PropertyToID("_SoulsTex");
  private static readonly int s_soulsColorLowID = Shader.PropertyToID("_SoulsColorLow");
  private static readonly int s_soulsColorHighID = Shader.PropertyToID("_SoulsColorHigh");
  private static readonly int s_mipMapRangeID = Shader.PropertyToID("_MipMapRange");
  private static readonly int s_softEdgeControlID = Shader.PropertyToID("_SoftEdgeControl");
  private static readonly int s_verticalNoiseFrequencyID = Shader.PropertyToID("_VerticalNoiseFrequency");
  private static readonly int s_verticalNoiseOffsetID = Shader.PropertyToID("_VerticalNoiseOffset");
  private static readonly int s_verticalNoiseAmplitudeID = Shader.PropertyToID("_VerticalNoiseAmplitude");
  private static readonly int s_radialNoiseFrequencyID = Shader.PropertyToID("_RadialNoiseFrequency");
  private static readonly int s_radialNoiseOffsetID = Shader.PropertyToID("_RadialNoiseOffset");
  private static readonly int s_radialNoiseAmplitudeID = Shader.PropertyToID("_RadialNoiseAmplitude");
  private static readonly int s_stream1UVScaleAndOffsetID = Shader.PropertyToID("_Stream1UVScaleAndOffset");
  private static readonly int s_stream2UVScaleAndOffsetID = Shader.PropertyToID("_Stream2UVScaleAndOffset");
  private static readonly int s_mipMapControlID = Shader.PropertyToID("_MipMapControl");
  private static readonly int s_soulsHorizontalSpaceID = Shader.PropertyToID("_SoulsHorizontalSpacing");
  private static readonly int s_soul1UVScaleAndOffsetID = Shader.PropertyToID("_Soul1UVScaleAndOffset");
  private static readonly int s_soul2UVScaleAndOffsetID = Shader.PropertyToID("_Soul2UVScaleAndOffset");
  private static readonly int s_soul3UVScaleAndOffsetID = Shader.PropertyToID("_Soul3UVScaleAndOffset");
  private static readonly int s_soul4UVScaleAndOffsetID = Shader.PropertyToID("_Soul4UVScaleAndOffset");

  public LegendaryHeroMagicEffectState UpdateState(
    float deltaTime,
    in LegendaryHeroMagicEffectState oldState)
  {
    Vector4 vector4_1 = (bool) (UnityEngine.Object) this.Noise.VerticalNoiseFunction ? this.Noise.VerticalNoiseFunction.OffsetRate : Vector4.zero;
    Vector4 vector4_2 = oldState.RotationState + deltaTime * vector4_1;
    vector4_2.x = Mathf.Repeat(vector4_2.x, 6.283185f);
    vector4_2.y = Mathf.Repeat(vector4_2.y, 6.283185f);
    vector4_2.z = Mathf.Repeat(vector4_2.z, 6.283185f);
    vector4_2.w = Mathf.Repeat(vector4_2.w, 6.283185f);
    Vector4 vector4_3 = (bool) (UnityEngine.Object) this.Noise.RadialNoiseFunction ? this.Noise.RadialNoiseFunction.OffsetRate : Vector4.zero;
    Vector4 vector4_4 = oldState.RadialState + deltaTime * vector4_3;
    vector4_4.x = Mathf.Repeat(vector4_4.x, 6.283185f);
    vector4_4.y = Mathf.Repeat(vector4_4.y, 6.283185f);
    vector4_4.z = Mathf.Repeat(vector4_4.z, 6.283185f);
    vector4_4.w = Mathf.Repeat(vector4_4.w, 6.283185f);
    Vector2 vector2_1 = oldState.UV1State + deltaTime * this.Stream.Stream1.ScrollRate;
    vector2_1.x = Mathf.Repeat(vector2_1.x, 1f);
    vector2_1.y = Mathf.Repeat(vector2_1.y, 1f);
    Vector2 vector2_2 = oldState.UV2State + deltaTime * this.Stream.Stream2.ScrollRate;
    vector2_2.x = Mathf.Repeat(vector2_2.x, 1f);
    vector2_2.y = Mathf.Repeat(vector2_2.y, 1f);
    float num1 = Mathf.Repeat(oldState.MipMapState + deltaTime * this.Souls.MipMapRate, 1f);
    float num2 = Mathf.Repeat(oldState.SoulsState + deltaTime * this.Souls.Speed, 1f);
    return new LegendaryHeroMagicEffectState()
    {
      RotationState = vector4_2,
      RadialState = vector4_4,
      UV1State = vector2_1,
      UV2State = vector2_2,
      MipMapState = num1,
      SoulsState = num2
    };
  }

  public void InitialiseMaterial(Material material)
  {
    Vector4 vector4_1 = new Vector4(this.Souls.ColorLow.r, this.Souls.ColorLow.g, this.Souls.ColorLow.b, 1f) * this.Souls.ColorLow.a;
    Vector4 vector4_2 = new Vector4(this.Souls.ColorHigh.r, this.Souls.ColorHigh.g, this.Souls.ColorHigh.b, 1f) * this.Souls.ColorHigh.a;
    Vector4 vector4_3 = new Vector4(this.Stream.Stream1.Intensity, this.Stream.Stream1.Intensity, this.Stream.Stream1.Intensity, 1f);
    Vector4 vector4_4 = (Vector4) (this.Stream.Stream1.ColorLow * (Color) vector4_3);
    Vector4 vector4_5 = (Vector4) (this.Stream.Stream1.ColorHigh * (Color) vector4_3);
    Vector4 vector4_6 = new Vector4(this.Stream.Stream2.Intensity, this.Stream.Stream2.Intensity, this.Stream.Stream2.Intensity, 1f);
    Vector4 vector4_7 = (Vector4) (this.Stream.Stream2.ColorLow * (Color) vector4_6);
    Vector4 vector4_8 = (Vector4) (this.Stream.Stream2.ColorHigh * (Color) vector4_6);
    material.SetTexture(LegendaryHeroMagicEffectMaterial.s_stream1TextureID, this.Stream.Stream1.Texture);
    material.SetTexture(LegendaryHeroMagicEffectMaterial.s_stream2TextureID, this.Stream.Stream2.Texture);
    material.SetTexture(LegendaryHeroMagicEffectMaterial.s_soulsTextureID, this.Souls.Texture);
    material.SetColor(LegendaryHeroMagicEffectMaterial.s_stream1ColorLowID, (Color) vector4_4);
    material.SetColor(LegendaryHeroMagicEffectMaterial.s_stream2ColorLowID, (Color) vector4_7);
    material.SetColor(LegendaryHeroMagicEffectMaterial.s_stream1ColorHighID, (Color) vector4_5);
    material.SetColor(LegendaryHeroMagicEffectMaterial.s_stream2ColorHighID, (Color) vector4_8);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_soulsColorLowID, vector4_1);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_soulsColorHighID, vector4_2);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_mipMapRangeID, (Vector4) new Vector2(this.Souls.MipMapMin, this.Souls.MipMapMax));
    material.SetFloat(LegendaryHeroMagicEffectMaterial.s_softEdgeControlID, 2f / Mathf.Max(Mathf.Epsilon, this.Stream.SoftEdgeControl));
    SinNoiseFunction verticalNoiseFunction = this.Noise.VerticalNoiseFunction;
    if ((UnityEngine.Object) verticalNoiseFunction != (UnityEngine.Object) null)
    {
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_verticalNoiseFrequencyID, verticalNoiseFunction.Frequency * 6.283185f * this.Noise.FrequencyScale);
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_verticalNoiseAmplitudeID, verticalNoiseFunction.GetAmplitude(this.Noise.Magnitude));
    }
    else
    {
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_verticalNoiseFrequencyID, Vector4.zero);
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_verticalNoiseAmplitudeID, Vector4.zero);
    }
    SinNoiseFunction radialNoiseFunction = this.Noise.RadialNoiseFunction;
    if ((UnityEngine.Object) radialNoiseFunction != (UnityEngine.Object) null)
    {
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_radialNoiseFrequencyID, radialNoiseFunction.Frequency * 6.283185f * this.Noise.FrequencyScale);
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_radialNoiseAmplitudeID, radialNoiseFunction.GetAmplitude(this.Noise.Magnitude));
    }
    else
    {
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_radialNoiseFrequencyID, Vector4.zero);
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_radialNoiseAmplitudeID, Vector4.zero);
    }
  }

  public void UpdateMaterialState(Material material, in LegendaryHeroMagicEffectState state)
  {
    if ((UnityEngine.Object) this.Noise.VerticalNoiseFunction != (UnityEngine.Object) null)
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_verticalNoiseOffsetID, state.RotationState);
    else
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_verticalNoiseOffsetID, Vector4.zero);
    if ((UnityEngine.Object) this.Noise.RadialNoiseFunction != (UnityEngine.Object) null)
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_radialNoiseOffsetID, state.RotationState);
    else
      material.SetVector(LegendaryHeroMagicEffectMaterial.s_radialNoiseOffsetID, Vector4.zero);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_stream1UVScaleAndOffsetID, new Vector4(this.Stream.Stream1.Scale, 1f, state.UV1State.x, state.UV1State.y));
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_stream2UVScaleAndOffsetID, new Vector4(this.Stream.Stream2.Scale, 1f, state.UV2State.x, state.UV2State.y));
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_mipMapControlID, (Vector4) new Vector3(this.Souls.MipMapHash.x, this.Souls.MipMapHash.y, this.Souls.MipMapHash.z * Mathf.Cos(state.MipMapState * 6.283185f)));
    float num1 = (float) (0.5 * (1.0 + (double) this.Souls.VerticalSpread));
    float num2 = 1f - num1;
    float num3 = (float) (((double) num1 + 2.0 * (double) num2) / 3.0);
    float num4 = (float) ((2.0 * (double) num1 + (double) num2) / 3.0);
    float y = 1f / this.Souls.VerticalScale;
    float num5 = 1f + this.Souls.HorizontalSpacing;
    Vector4 vector4_1 = new Vector4(this.Souls.HorizontalScale, y, num5 * state.SoulsState, (float) (0.5 - (double) num1 * (double) y));
    Vector4 vector4_2 = new Vector4(this.Souls.HorizontalScale, y, num5 * (state.SoulsState + this.Souls.Offset1), (float) (0.5 - (double) num4 * (double) y));
    Vector4 vector4_3 = new Vector4(this.Souls.HorizontalScale, y, num5 * (state.SoulsState + this.Souls.Offset2), (float) (0.5 - (double) num3 * (double) y));
    Vector4 vector4_4 = new Vector4(this.Souls.HorizontalScale, y, num5 * (state.SoulsState + this.Souls.Offset3), (float) (0.5 - (double) num2 * (double) y));
    material.SetFloat(LegendaryHeroMagicEffectMaterial.s_soulsHorizontalSpaceID, num5);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_soul1UVScaleAndOffsetID, vector4_1);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_soul2UVScaleAndOffsetID, vector4_2);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_soul3UVScaleAndOffsetID, vector4_3);
    material.SetVector(LegendaryHeroMagicEffectMaterial.s_soul4UVScaleAndOffsetID, vector4_4);
  }

  [Serializable]
  public struct StreamSubData
  {
    [Header("Texture")]
    public Texture Texture;
    [Header("Color")]
    public Color ColorHigh;
    public Color ColorLow;
    [Range(0.0f, 4f)]
    public float Intensity;
    [Header("Animation")]
    public float Scale;
    public Vector2 ScrollRate;
  }

  [Serializable]
  public struct StreamData
  {
    [Range(0.0f, 1f)]
    public float SoftEdgeControl;
    public LegendaryHeroMagicEffectMaterial.StreamSubData Stream1;
    public LegendaryHeroMagicEffectMaterial.StreamSubData Stream2;
  }

  [Serializable]
  public struct NoiseData
  {
    [Range(0.0f, 1f)]
    public float Magnitude;
    public float FrequencyScale;
    public SinNoiseFunction RadialNoiseFunction;
    public SinNoiseFunction VerticalNoiseFunction;
  }

  [Serializable]
  public struct SoulsData
  {
    [Header("Texture")]
    public Texture Texture;
    [Header("Color")]
    public Color ColorHigh;
    public Color ColorLow;
    [Header("Blurring (Texture Mip Map)")]
    public float MipMapRate;
    [Range(0.0f, 10f)]
    public float MipMapMin;
    [Range(0.0f, 10f)]
    public float MipMapMax;
    public Vector3 MipMapHash;
    [Header("Position and Size")]
    [Range(0.0f, 1f)]
    public float VerticalSpread;
    [Range(0.0f, 1f)]
    public float VerticalScale;
    public float HorizontalScale;
    [Min(0.0f)]
    public float HorizontalSpacing;
    [Range(0.0f, 1f)]
    [Header("Offsets From Primary")]
    public float Offset1;
    [Range(0.0f, 1f)]
    public float Offset2;
    [Range(0.0f, 1f)]
    public float Offset3;
    [Header("Animation")]
    public float Speed;
  }
}
