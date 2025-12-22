using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UberShaderAnimation : ScriptableObject
{
  [SerializeField]
  public List<UberShaderAnimation.UberAnimation> animations = new List<UberShaderAnimation.UberAnimation>();
  public List<int> materialPropertyIDs = new List<int>();
  public int version = 1;

  public enum PropertyType
  {
    Color,
    Vector,
    Float,
  }

  [Serializable]
  public class UberAnimation
  {
    public string materialPropertyName;
    public UberShaderAnimation.PropertyType propertyType;
    public int materialIndex;
    public List<UberShaderAnimation.UberAnimationElement> animationElement = new List<UberShaderAnimation.UberAnimationElement>();
  }

  [Serializable]
  public class UberAnimationElement
  {
    public int element;
    public bool incrementingValue;
    public int incrementingElement;
    public float incrementingLastValue;
    public float incrementingSpeed;
    public UberShaderAnimation.UberAnimationCurve animationCurve;
    public UberShaderAnimation.UberAnimationRandom randomAnimation;
    public UberShaderAnimation.UberAnimationColor colorAnimation;
  }

  [Serializable]
  public class UberAnimationCurve
  {
    public bool enabled;
    public AnimationCurve animationCurve;
    public float speed = 1f;
    public float scale = 1f;
    public float offset;
  }

  [Serializable]
  public class UberAnimationRandom
  {
    public bool enabled;
    public AnimationCurve intensityCurve;
    public float intensitySpeed = 1f;
    public float seed;
    public float minValue = -1f;
    public float maxValue = 1f;
    public float speed = 1f;
    public float scale = 1f;
  }

  [Serializable]
  public class UberAnimationColor
  {
    public bool enabled;
    public Color color = Color.white;
    public Gradient gradient = new Gradient();
  }
}
