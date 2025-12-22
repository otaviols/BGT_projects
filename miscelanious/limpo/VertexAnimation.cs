using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof (Renderer))]
public class VertexAnimation : MonoBehaviour
{
  private const string ANIMATION_TIME = "_AnimTime";
  private const string VERTEX_COUNT_KEY = "_VertCount";
  private const string FRAME_COUNT_KEY = "_FrameCount";
  private const string ANIMATION_TEXTURE_KEY = "_MorphTex";
  private const string TIME_OFFSET = "_TimeOffset";
  [Tooltip("The animation's default speed. Even if the current speed is modified via Timeline or PlayMaker, this variable will not change.")]
  [Min(0.0001f)]
  public float AnimationSpeed;
  public int VertexCount;
  public int RecordedFrameCount;
  public int OriginalFrameCount;
  public int OriginalFPS;
  public Texture2D AnimationTexture;
  public VertexAnimation.AnimationClipInfo[] AnimationData;
  private float m_currentAnimationSpeed;
  private MaterialPropertyBlock m_properties;
  private Renderer m_renderer;
  private bool m_animationIsActive;
  private string m_animationName = string.Empty;

  private void Awake()
  {
    this.m_properties = new MaterialPropertyBlock();
    this.m_renderer = this.GetComponent<Renderer>();
    this.UpdateProperties();
  }

  private void OnValidate()
  {
    this.ValidateAnimationData();
    this.UpdateProperties();
  }

  private void ValidateAnimationData()
  {
    if (this.AnimationData == null)
      return;
    foreach (VertexAnimation.AnimationClipInfo animationClipInfo in this.AnimationData)
    {
      this.OriginalFPS = Mathf.Max(this.OriginalFPS, 1);
      this.AnimationSpeed = Mathf.Max(this.AnimationSpeed, 0.01f);
      this.OriginalFrameCount = Mathf.Max(this.OriginalFrameCount, 2);
      animationClipInfo.frameRange.x = Mathf.Clamp(animationClipInfo.frameRange.x, 0, this.OriginalFrameCount - 2);
      animationClipInfo.frameRange.y = Mathf.Clamp(animationClipInfo.frameRange.y, 1, this.OriginalFrameCount - 1);
      if (animationClipInfo.frameRange.y <= animationClipInfo.frameRange.x)
        animationClipInfo.frameRange.y = animationClipInfo.frameRange.x + 1;
    }
  }

  private void UpdateProperties()
  {
    if (!(bool) (UnityEngine.Object) this.m_renderer)
      return;
    this.m_currentAnimationSpeed = this.AnimationSpeed;
    this.m_renderer.GetPropertyBlock(this.m_properties);
    this.m_properties.SetFloat("_AnimTime", this.CalculateAnimationTime());
    this.m_properties.SetFloat("_VertCount", (float) this.VertexCount);
    this.m_properties.SetFloat("_FrameCount", (float) this.RecordedFrameCount);
    this.m_properties.SetTexture("_MorphTex", (Texture) this.AnimationTexture);
    this.m_renderer.SetPropertyBlock(this.m_properties);
  }

  public void StartAnimation(string animationName)
  {
    if (!(bool) (UnityEngine.Object) this.m_renderer)
      return;
    this.m_renderer.GetPropertyBlock(this.m_properties);
    if (!Mathf.Approximately(this.m_properties.GetFloat("_VertCount"), (float) this.VertexCount))
      this.UpdateProperties();
    this.m_animationIsActive = true;
    this.m_animationName = animationName;
    this.m_properties.SetFloat("_TimeOffset", Time.timeSinceLevelLoad - this.CalculateStartFrameTime(this.m_animationName));
    this.m_renderer.SetPropertyBlock(this.m_properties);
  }

  public void SetAnimationCompletionPercent(string animationName, float completionPercent)
  {
    if (!(bool) (UnityEngine.Object) this.m_renderer)
      this.m_renderer = this.GetComponent<Renderer>();
    if (this.m_properties == null)
      this.m_properties = new MaterialPropertyBlock();
    this.m_renderer.GetPropertyBlock(this.m_properties);
    this.m_animationIsActive = true;
    this.m_animationName = animationName;
    if (!Mathf.Approximately(this.m_properties.GetFloat("_VertCount"), (float) this.VertexCount))
      this.UpdateProperties();
    completionPercent = Mathf.Clamp01(completionPercent);
    float animationLength = this.GetAnimationLength(this.m_animationName);
    this.m_properties.SetFloat("_TimeOffset", (float) ((double) Time.timeSinceLevelLoad - (double) this.CalculateStartFrameTime(this.m_animationName) - (double) completionPercent * (double) animationLength));
    this.m_renderer.SetPropertyBlock(this.m_properties);
  }

  [Obsolete("Use SetCurrentAnimationCompletionPercent().")]
  public void SetAnimationCompletionPercent(float completionPercent) => this.SetCurrentAnimationCompletionPercent(completionPercent);

  public void SetCurrentAnimationCompletionPercent(float completionPercent) => this.SetAnimationCompletionPercent(this.m_animationName, completionPercent);

  private void SetAnimationSpeed(float animationSpeed)
  {
    if ((double) animationSpeed <= 0.0 || (double) animationSpeed == (double) this.m_currentAnimationSpeed)
      return;
    this.m_renderer.GetPropertyBlock(this.m_properties);
    this.m_currentAnimationSpeed = animationSpeed;
    this.m_properties.SetFloat("_AnimTime", this.CalculateAnimationTime());
    this.m_renderer.SetPropertyBlock(this.m_properties);
  }

  public float CurrentAnimationCompletionPercent
  {
    get
    {
      if (!this.m_animationIsActive)
        return 0.0f;
      if (!(bool) (UnityEngine.Object) this.m_renderer)
        this.m_renderer = this.GetComponent<Renderer>();
      if (this.m_properties == null)
        this.m_properties = new MaterialPropertyBlock();
      this.m_renderer.GetPropertyBlock(this.m_properties);
      if (!Mathf.Approximately(this.m_properties.GetFloat("_VertCount"), (float) this.VertexCount))
        this.UpdateProperties();
      return (float) -((double) this.m_properties.GetFloat("_TimeOffset") - (double) Time.timeSinceLevelLoad + (double) this.CalculateStartFrameTime(this.m_animationName)) / this.GetAnimationLength(this.m_animationName);
    }
  }

  public void OverwriteAnimationSpeed(float animationSpeed)
  {
    float completionPercent = 1f;
    if (this.m_animationIsActive)
      completionPercent = this.CurrentAnimationCompletionPercent;
    this.SetAnimationSpeed(animationSpeed);
    if (!this.m_animationIsActive)
      return;
    this.SetAnimationCompletionPercent(this.m_animationName, completionPercent);
  }

  public void UseDefaultAnimationSpeed() => this.SetAnimationSpeed(this.AnimationSpeed);

  public float GetAnimationLengthUnscaled(string animationName)
  {
    VertexAnimation.AnimationClipInfo animationInfo = this.GetAnimationInfo(animationName);
    return animationInfo != null ? (float) (animationInfo.frameRange.y - animationInfo.frameRange.x) / (float) this.OriginalFPS : 0.0f;
  }

  public float GetAnimationLength(string animationName) => this.GetAnimationLengthUnscaled(animationName) / this.CurrentAnimationSpeed;

  private float CalculateStartFrameTime(string animationName)
  {
    VertexAnimation.AnimationClipInfo animationInfo = this.GetAnimationInfo(animationName);
    return animationInfo != null ? (float) animationInfo.frameRange.x / (float) this.OriginalFPS / this.m_currentAnimationSpeed : 0.0f;
  }

  private VertexAnimation.AnimationClipInfo GetAnimationInfo(string animationName)
  {
    foreach (VertexAnimation.AnimationClipInfo animationInfo in this.AnimationData)
    {
      if (animationInfo.name == animationName)
        return animationInfo;
    }
    return (VertexAnimation.AnimationClipInfo) null;
  }

  private float CalculateAnimationTime() => (float) this.OriginalFrameCount / (float) this.OriginalFPS / this.m_currentAnimationSpeed;

  public float CurrentAnimationSpeed => this.m_currentAnimationSpeed;

  [Serializable]
  public class AnimationClipInfo
  {
    public string name;
    public Vector2Int frameRange;
  }
}
