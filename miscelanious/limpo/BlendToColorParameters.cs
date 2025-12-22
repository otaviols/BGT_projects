using UnityEngine;

public struct BlendToColorParameters
{
  public static BlendToColorParameters Default = new BlendToColorParameters();
  public static BlendToColorParameters None = new BlendToColorParameters(Color.white, 0.0f);
  public Color BlendColor;
  public float Amount;

  public BlendToColorParameters(Color blendColor, float amount)
  {
    this.BlendColor = blendColor;
    this.Amount = amount;
  }
}
