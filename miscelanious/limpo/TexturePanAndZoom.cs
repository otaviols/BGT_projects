using Hearthstone.UI.Core;
using UnityEngine;

public class TexturePanAndZoom : MonoBehaviour
{
  private Vector2 m_startScale;
  private Vector2 m_targetScale;
  private Vector2 m_startOffset;
  private Vector2 m_targetOffset;
  private float m_startTime = -1f;
  public float m_transitionTime;
  public AnimationCurve m_transitionEase = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
  public Material m_targetMaterial;

  [Overridable]
  public float TargetScaleX
  {
    set
    {
      this.m_targetScale = new Vector2(value, this.m_targetScale.y);
      this.m_startScale = this.m_targetMaterial.mainTextureScale;
      this.m_startTime = Time.time;
    }
    get => this.m_targetScale.x;
  }

  [Overridable]
  public float TargetScaleY
  {
    set
    {
      this.m_targetScale = new Vector2(this.m_targetScale.x, value);
      this.m_startScale = this.m_targetMaterial.mainTextureScale;
      this.m_startTime = Time.time;
    }
    get => this.m_targetScale.y;
  }

  [Overridable]
  public float TargetOffsetX
  {
    set
    {
      this.m_targetOffset = new Vector2(value, this.m_targetOffset.y);
      this.m_startOffset = this.m_targetMaterial.mainTextureOffset;
      this.m_startTime = Time.time;
    }
    get => this.m_targetOffset.x;
  }

  [Overridable]
  public float TargetOffsetY
  {
    set
    {
      this.m_targetOffset = new Vector2(this.m_targetOffset.x, value);
      this.m_startOffset = this.m_targetMaterial.mainTextureOffset;
      this.m_startTime = Time.time;
    }
    get => this.m_targetOffset.y;
  }

  private bool IsPanAndZooming() => (double) this.m_startTime >= 0.0;

  private void Update()
  {
    if (!this.IsPanAndZooming())
      return;
    float num = Time.time - this.m_startTime;
    float time = (double) this.m_transitionTime <= (double) Mathf.Epsilon ? 1f : num / this.m_transitionTime;
    if ((double) time >= 1.0)
    {
      time = 1f;
      this.m_startTime = -1f;
    }
    float t = this.m_transitionEase.Evaluate(time);
    this.m_targetMaterial.mainTextureScale = Vector2.LerpUnclamped(this.m_startScale, this.m_targetScale, t);
    this.m_targetMaterial.mainTextureOffset = Vector2.LerpUnclamped(this.m_startOffset, this.m_targetOffset, t);
  }
}
