using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class UberShaderController : MonoBehaviour
{
  private const int GUI_PROPERTY_LABEL_WIDTH = 130;
  [SerializeField]
  private UberShaderAnimation m_UberShaderAnimation;
  public int m_MaterialIndex = -1;
  private bool m_firstFrame;
  private float m_time;
  private float m_deltaTime;
  private Renderer m_renderer;
  private float m_lastTime;
  private float m_randomOffset;
  private string m_copyBuffer;
  private UberShaderAnimation.PropertyType m_copyBufferType;
  private string m_copyBufferLayer;
  private int m_copyBufferLayerCount;
  private DateTime? m_lastSaveTime;
  private float m_maxTime = (float) ushort.MaxValue;
  private List<Material> m_sharedAnimationMaterials = new List<Material>();
  private static bool s_autoSave = false;
  private static float s_autoSaveInterval = 30f;

  public UberShaderAnimation UberShaderAnimation
  {
    get => this.m_UberShaderAnimation;
    set
    {
      if ((UnityEngine.Object) this.m_UberShaderAnimation != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_UberShaderAnimation);
      this.m_UberShaderAnimation = value;
      this.UpdateShaderIDs();
    }
  }

  public DateTime? LastSaveTime => this.m_lastSaveTime;

  public static bool GetAutoSaveEnabled() => UberShaderController.s_autoSave;

  public static float GetAutoSaveInterval() => UberShaderController.s_autoSaveInterval;

  private void Awake()
  {
    if ((UnityEngine.Object) this.m_UberShaderAnimation == (UnityEngine.Object) null)
      this.m_UberShaderAnimation = ScriptableObject.CreateInstance<UberShaderAnimation>();
    this.m_firstFrame = true;
    this.m_randomOffset = UnityEngine.Random.Range(0.0f, 10f);
    this.m_time += this.m_randomOffset;
    this.m_renderer = this.GetComponent<Renderer>();
  }

  private void OnEnable() => this.LoadUberShaderAnimation();

  private void Update() => this.UpdateAnimation();

  [ContextMenu("Reload Animation File")]
  private void LoadUberShaderAnimation()
  {
    this.m_firstFrame = true;
    if ((UnityEngine.Object) this.m_UberShaderAnimation == (UnityEngine.Object) null)
      this.m_UberShaderAnimation = ScriptableObject.CreateInstance<UberShaderAnimation>();
    this.UpdateShaderIDs();
  }

  private void UpdateTime()
  {
    this.m_deltaTime = Time.deltaTime;
    this.m_time += this.m_deltaTime;
    if ((double) this.m_time <= (double) this.m_maxTime)
      return;
    this.m_time = 0.0001f;
  }

  private void UpdateEditorTime()
  {
    float num = Time.realtimeSinceStartup + this.m_randomOffset;
    this.m_deltaTime = num - this.m_lastTime;
    this.m_lastTime = num;
    this.m_time += this.m_deltaTime;
    if ((double) this.m_time <= (double) this.m_maxTime)
      return;
    this.m_time = 0.0001f;
  }

  private void UpdateAnimation()
  {
    this.UpdateTime();
    if ((UnityEngine.Object) this.m_renderer == (UnityEngine.Object) null)
      return;
    this.m_sharedAnimationMaterials.Clear();
    this.m_renderer.GetSharedMaterials(this.m_sharedAnimationMaterials);
    if (this.m_sharedAnimationMaterials.Count < 1 || this.m_UberShaderAnimation == null || this.m_UberShaderAnimation.animations == null)
      return;
    for (int index1 = 0; index1 < this.m_UberShaderAnimation.animations.Count; ++index1)
    {
      UberShaderAnimation.UberAnimation animation = this.m_UberShaderAnimation.animations[index1];
      int materialPropertyId = this.m_UberShaderAnimation.materialPropertyIDs[index1];
      int materialIndex = animation.materialIndex;
      if (this.m_MaterialIndex > -1 && this.m_MaterialIndex < this.m_sharedAnimationMaterials.Count)
        materialIndex = this.m_MaterialIndex;
      Material animationMaterial = this.m_sharedAnimationMaterials[materialIndex];
      if (!((UnityEngine.Object) animationMaterial == (UnityEngine.Object) null))
      {
        if (animation.propertyType == UberShaderAnimation.PropertyType.Color)
        {
          UberShaderAnimation.UberAnimationElement animationElement = animation.animationElement[0];
          if (animationElement != null)
          {
            UberShaderAnimation.UberAnimationColor colorAnimation = animationElement.colorAnimation;
            if (colorAnimation != null && !colorAnimation.enabled)
              continue;
          }
          else
            continue;
        }
        if (animationMaterial.HasProperty(materialPropertyId))
        {
          Vector4 vector4_1 = Vector4.zero;
          if (animation.propertyType == UberShaderAnimation.PropertyType.Vector)
            vector4_1 = animationMaterial.GetVector(materialPropertyId);
          else if (animation.propertyType == UberShaderAnimation.PropertyType.Float)
            vector4_1[0] = animationMaterial.GetFloat(materialPropertyId);
          Vector4 vector4_2 = vector4_1;
          for (int index2 = 0; index2 < animation.animationElement.Count; ++index2)
          {
            UberShaderAnimation.UberAnimationElement animationElement = animation.animationElement[index2];
            UberShaderAnimation.UberAnimationCurve animationCurve = animationElement.animationCurve;
            UberShaderAnimation.UberAnimationRandom randomAnimation = animationElement.randomAnimation;
            int element = animationElement.element;
            float num1 = 0.0f;
            if (!animationElement.incrementingValue)
            {
              switch (element)
              {
                case 0:
                  num1 = vector4_1.x;
                  break;
                case 1:
                  num1 = vector4_1.y;
                  break;
                case 2:
                  num1 = vector4_1.z;
                  break;
                case 3:
                  num1 = vector4_1.w;
                  break;
              }
            }
            if (animationCurve.animationCurve != null && animationCurve.enabled)
              num1 = (animationCurve.animationCurve.Evaluate(this.m_time * animationCurve.speed) + animationCurve.offset) * animationCurve.scale;
            if (randomAnimation != null && randomAnimation.enabled)
            {
              if (animationCurve.animationCurve == null || !animationCurve.enabled)
                num1 = 0.0f;
              float num2 = 1f;
              if (randomAnimation.intensityCurve != null)
                num2 = randomAnimation.intensityCurve.Evaluate(this.m_time * randomAnimation.intensitySpeed);
              num1 += Mathf.Lerp(randomAnimation.minValue, randomAnimation.maxValue, (float) (((double) UberMath.SimplexNoise(this.m_time * randomAnimation.speed + randomAnimation.seed, 0.5f) + 1.0) * 0.5) * num2) * randomAnimation.scale;
            }
            if (animationElement.incrementingValue)
            {
              if (this.m_firstFrame)
                animationElement.incrementingLastValue = 0.0f;
              if ((double) animationElement.incrementingLastValue > (double) this.m_maxTime)
                animationElement.incrementingLastValue = 0.0001f;
              float num3 = this.m_deltaTime * (num1 + animationElement.incrementingSpeed);
              float num4 = animationElement.incrementingLastValue + num3;
              num1 = num4;
              animationElement.incrementingLastValue = num4;
            }
            switch (element)
            {
              case 0:
                vector4_2.x = num1;
                break;
              case 1:
                vector4_2.y = num1;
                break;
              case 2:
                vector4_2.z = num1;
                break;
              case 3:
                vector4_2.w = num1;
                break;
            }
          }
          if (animation.propertyType == UberShaderAnimation.PropertyType.Color)
          {
            Color color = animation.animationElement[0].colorAnimation.gradient.Evaluate(vector4_2.x);
            animationMaterial.SetColor(materialPropertyId, color);
          }
          else
            animationMaterial.SetVector(materialPropertyId, vector4_2);
        }
      }
    }
    this.m_firstFrame = false;
  }

  private void UpdateShaderIDs()
  {
    if ((UnityEngine.Object) this.m_renderer == (UnityEngine.Object) null)
      return;
    List<Material> sharedMaterials = this.m_renderer.GetSharedMaterials();
    if (sharedMaterials == null || sharedMaterials.Count < 1 || this.m_UberShaderAnimation == null || this.m_UberShaderAnimation.animations == null)
      return;
    this.m_UberShaderAnimation.materialPropertyIDs = new List<int>(this.m_UberShaderAnimation.animations.Count);
    for (int index = 0; index < this.m_UberShaderAnimation.animations.Count; ++index)
      this.UberShaderAnimation.materialPropertyIDs.Add(Shader.PropertyToID(this.m_UberShaderAnimation.animations[index].materialPropertyName));
  }
}
