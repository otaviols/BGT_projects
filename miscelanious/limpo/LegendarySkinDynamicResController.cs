using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LegendarySkinDynamicResController : MonoBehaviour
{
  private LegendarySkin m_skin;
  private Vector2 m_originalScale = new Vector2(1f, 1f);
  private Vector2 m_originalOffset = new Vector2(0.0f, 0.0f);
  [NonSerialized]
  public Renderer Renderer;
  [NonSerialized]
  public int MaterialIdx;
  [SerializeField]
  public bool ForceFullResolution;

  public LegendarySkin Skin
  {
    get => this.m_skin;
    set
    {
      if ((UnityEngine.Object) this.m_skin != (UnityEngine.Object) null)
        this.m_skin.RemoveDynamicResController(this);
      this.m_skin = value;
      if (!((UnityEngine.Object) this.m_skin != (UnityEngine.Object) null) || !this.isActiveAndEnabled)
        return;
      this.m_skin.AddDynamicResController(this);
    }
  }

  public void CacheMaterialProperties(Material material)
  {
    this.m_originalScale = material.mainTextureScale;
    this.m_originalOffset = material.mainTextureOffset;
  }

  private void OnEnable()
  {
    if (!((UnityEngine.Object) this.m_skin != (UnityEngine.Object) null))
      return;
    this.m_skin.AddDynamicResController(this);
  }

  private void OnDisable()
  {
    if (!((UnityEngine.Object) this.m_skin != (UnityEngine.Object) null))
      return;
    this.m_skin.RemoveDynamicResController(this);
  }

  public LegendarySkinDynamicResController.SizeResult GetSize(
    IEnumerable<Camera> cameras,
    out float size)
  {
    if (this.ForceFullResolution)
    {
      size = float.MaxValue;
      return LegendarySkinDynamicResController.SizeResult.MaxSize;
    }
    if ((UnityEngine.Object) this.Renderer == (UnityEngine.Object) null)
    {
      size = 0.0f;
      return LegendarySkinDynamicResController.SizeResult.Invalid;
    }
    float num1 = 0.0f;
    Bounds bounds = this.Renderer.bounds;
    foreach (Camera camera in cameras)
    {
      if (camera.isActiveAndEnabled)
      {
        Vector2 screenPoint1 = (Vector2) camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
        Vector2 lhs1 = screenPoint1;
        Vector3 screenPoint2 = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
        Vector2 lhs2 = Vector2.Min(screenPoint1, (Vector2) screenPoint2);
        Vector2 rhs1 = (Vector2) screenPoint2;
        Vector2 lhs3 = Vector2.Max(lhs1, rhs1);
        Vector3 screenPoint3 = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
        Vector2 lhs4 = Vector2.Min(lhs2, (Vector2) screenPoint3);
        Vector2 rhs2 = (Vector2) screenPoint3;
        Vector2 lhs5 = Vector2.Max(lhs3, rhs2);
        Vector3 screenPoint4 = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
        Vector2 lhs6 = Vector2.Min(lhs4, (Vector2) screenPoint4);
        Vector2 rhs3 = (Vector2) screenPoint4;
        Vector2 lhs7 = Vector2.Max(lhs5, rhs3);
        Vector3 screenPoint5 = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
        Vector2 lhs8 = Vector2.Min(lhs6, (Vector2) screenPoint5);
        Vector2 rhs4 = (Vector2) screenPoint5;
        Vector2 lhs9 = Vector2.Max(lhs7, rhs4);
        Vector3 screenPoint6 = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
        Vector2 lhs10 = Vector2.Min(lhs8, (Vector2) screenPoint6);
        Vector2 rhs5 = (Vector2) screenPoint6;
        Vector2 lhs11 = Vector2.Max(lhs9, rhs5);
        Vector3 screenPoint7 = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
        Vector2 lhs12 = Vector2.Min(lhs10, (Vector2) screenPoint7);
        Vector2 rhs6 = (Vector2) screenPoint7;
        Vector2 lhs13 = Vector2.Max(lhs11, rhs6);
        Vector3 screenPoint8 = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));
        Vector2 vector2_1 = Vector2.Min(lhs12, (Vector2) screenPoint8);
        Vector2 rhs7 = (Vector2) screenPoint8;
        Vector2 vector2_2 = Vector2.Max(lhs13, rhs7) - vector2_1;
        float num2 = Mathf.Abs(vector2_2.x) / this.m_originalScale.x;
        float num3 = Mathf.Abs(vector2_2.y) / this.m_originalScale.y;
        num1 = Mathf.Max(num1, num2, num3);
      }
    }
    size = num1;
    return LegendarySkinDynamicResController.SizeResult.Bounded;
  }

  public void UpdateMaterial(float dynamicResolution)
  {
    if (!((UnityEngine.Object) this.Renderer != (UnityEngine.Object) null))
      return;
    Material sharedMaterial = this.Renderer.GetSharedMaterial(this.MaterialIdx);
    if (!((UnityEngine.Object) sharedMaterial != (UnityEngine.Object) null))
      return;
    sharedMaterial.mainTextureScale = this.m_originalScale * dynamicResolution;
    sharedMaterial.mainTextureOffset = this.m_originalOffset * dynamicResolution;
  }

  public enum SizeResult
  {
    Invalid,
    Bounded,
    MaxSize,
  }
}
