using Unity.Profiling;
using UnityEngine;

[RequireComponent(typeof (Camera))]
public class FullScreenEffects : MonoBehaviour
{
  private const int NO_WORK_FRAMES_BEFORE_DEACTIVATE = 2;
  public Texture2D m_VignettingMask;
  private int m_DeactivateFrameCount;
  private Camera m_Camera;
  private ScreenEffectParameters m_toParameters;
  private ScreenEffectParameters m_startParameters;
  private ScreenEffectParameters m_currentParameters;
  private float m_effectTime;
  private ProfilerMarker m_updateProfilerMarker;
  private bool m_invokedOnFinishCallbacks;
  private bool m_overridingBlendToColor;

  public FullScreenFXMgr.ScreenEffectsInstance ActiveEffectsInstance { get; private set; }

  protected void Awake()
  {
    this.m_Camera = this.GetComponent<Camera>();
    this.m_updateProfilerMarker = new ProfilerMarker("FullScreenEffects.DoUpdate");
    this.m_currentParameters = ScreenEffectParameters.None;
  }

  private void Update()
  {
    if (this.IsActive)
      return;
    if (this.m_DeactivateFrameCount > 2)
    {
      this.m_DeactivateFrameCount = 0;
      this.Disable();
    }
    else
      ++this.m_DeactivateFrameCount;
  }

  private void LateUpdate()
  {
    if (!this.IsActive)
      return;
    this.DoToUpdate();
  }

  public Camera Camera => this.m_Camera;

  public bool BlurEnabled
  {
    get => (this.m_currentParameters.Type & ScreenEffectType.BLUR) != 0;
    set
    {
      if (value)
      {
        this.m_currentParameters.Type |= ScreenEffectType.BLUR;
        this.enabled = true;
      }
      else
        this.m_currentParameters.Type &= ~ScreenEffectType.BLUR;
    }
  }

  public float BlurBlend
  {
    get => this.m_currentParameters.Blur.Blend;
    set
    {
      this.BlurEnabled = true;
      this.m_currentParameters.Blur.Blend = value;
    }
  }

  public bool VignettingEnable
  {
    get => (this.m_currentParameters.Type & ScreenEffectType.VIGNETTE) != 0;
    set
    {
      if (value)
      {
        this.m_currentParameters.Type |= ScreenEffectType.VIGNETTE;
        this.enabled = true;
      }
      else
        this.m_currentParameters.Type &= ~ScreenEffectType.VIGNETTE;
    }
  }

  public float VignettingIntensity
  {
    get => this.m_currentParameters.Vignette.Amount;
    set
    {
      this.VignettingEnable = true;
      this.m_currentParameters.Type |= ScreenEffectType.VIGNETTE;
      this.m_currentParameters.Vignette.Amount = value;
    }
  }

  public bool BlendToColorEnable
  {
    get => (this.m_currentParameters.Type & ScreenEffectType.BLENDTOCOLOR) != 0;
    set
    {
      if (value)
      {
        this.m_currentParameters.Type |= ScreenEffectType.BLENDTOCOLOR;
        this.enabled = true;
      }
      else
        this.m_currentParameters.Type &= ~ScreenEffectType.BLENDTOCOLOR;
    }
  }

  public Color BlendColor
  {
    get => this.m_currentParameters.BlendToColor.BlendColor;
    set
    {
      this.BlendToColorEnable = true;
      this.m_currentParameters.Type |= ScreenEffectType.BLENDTOCOLOR;
      this.m_currentParameters.BlendToColor.BlendColor = value;
    }
  }

  public float BlendToColorAmount
  {
    get => this.m_currentParameters.BlendToColor.Amount;
    set
    {
      this.BlendToColorEnable = true;
      this.m_currentParameters.Type |= ScreenEffectType.BLENDTOCOLOR;
      this.m_currentParameters.BlendToColor.Amount = value;
    }
  }

  public bool DesaturationEnabled
  {
    get => (this.m_currentParameters.Type & ScreenEffectType.DESATURATE) != 0;
    set
    {
      if (value)
      {
        this.m_currentParameters.Type |= ScreenEffectType.DESATURATE;
        this.enabled = true;
      }
      else
        this.m_currentParameters.Type &= ~ScreenEffectType.DESATURATE;
    }
  }

  public float Desaturation
  {
    get => this.m_currentParameters.Desaturate.Amount;
    set
    {
      this.DesaturationEnabled = true;
      this.m_currentParameters.Type |= ScreenEffectType.DESATURATE;
      this.m_currentParameters.Desaturate.Amount = value;
    }
  }

  public bool IsActive => this.gameObject.activeInHierarchy && this.enabled && this.HasActiveEffects;

  public bool HasActiveEffects => this.m_currentParameters.Type != 0;

  public void SetBlendToColorOverride(float amount, Color color)
  {
    this.m_currentParameters.Type |= ScreenEffectType.BLENDTOCOLOR;
    this.m_currentParameters.BlendToColor = new BlendToColorParameters(color, amount);
    this.m_toParameters.Type |= ScreenEffectType.BLENDTOCOLOR;
    this.m_toParameters.BlendToColor = this.m_currentParameters.BlendToColor;
    this.m_overridingBlendToColor = true;
    this.enabled = true;
  }

  public void DisableBlendToColorOverride()
  {
    this.m_currentParameters.Type &= ~ScreenEffectType.BLENDTOCOLOR;
    this.m_toParameters.Type &= ~ScreenEffectType.BLENDTOCOLOR;
    this.m_overridingBlendToColor = false;
  }

  private void SetDefaults()
  {
    this.m_currentParameters = ScreenEffectParameters.None;
    this.m_toParameters = ScreenEffectParameters.None;
    this.ActiveEffectsInstance = (FullScreenFXMgr.ScreenEffectsInstance) null;
  }

  public void Disable()
  {
    if (this.ActiveEffectsInstance != null)
      FullScreenFXMgr.Get().OnFinishedEffect(this.ActiveEffectsInstance);
    this.enabled = false;
    this.SetDefaults();
  }

  public void StartEffect(
    FullScreenFXMgr.ScreenEffectsInstance screenEffectsInstance)
  {
    ScreenEffectParameters parameters = screenEffectsInstance.Parameters;
    this.m_currentParameters.Type = parameters.Type;
    this.m_currentParameters.PassLocation = parameters.PassLocation;
    this.m_startParameters = this.m_currentParameters;
    this.m_toParameters = parameters;
    this.m_effectTime = Time.time;
    if (!this.m_invokedOnFinishCallbacks && this.ActiveEffectsInstance != null && this.ActiveEffectsInstance.OnFinishedCallback != null)
      this.ActiveEffectsInstance.OnFinishedCallback();
    this.ActiveEffectsInstance = screenEffectsInstance;
    this.m_invokedOnFinishCallbacks = false;
    this.enabled = true;
  }

  public void CleanupEffects(float time)
  {
    this.m_toParameters = ScreenEffectParameters.None;
    this.m_toParameters.Time = time;
    this.m_invokedOnFinishCallbacks = false;
    this.enabled = true;
  }

  private void DoToUpdate()
  {
    using (this.m_updateProfilerMarker.Auto())
    {
      float num1 = Time.time - this.m_effectTime;
      if ((double) num1 > (double) this.m_toParameters.Time)
      {
        this.FinishEffect();
      }
      else
      {
        iTween.EasingFunction easingFunction = iTween.GetEasingFunction(this.m_toParameters.EaseType);
        float num2 = num1 / this.m_toParameters.Time;
        this.m_currentParameters.Blur.Blend = easingFunction(this.m_startParameters.Blur.Blend, this.m_toParameters.Blur.Blend, num2);
        this.m_currentParameters.Vignette.Amount = easingFunction(this.m_startParameters.Vignette.Amount, this.m_toParameters.Vignette.Amount, num2);
        this.m_currentParameters.Desaturate.Amount = easingFunction(this.m_startParameters.Desaturate.Amount, this.m_toParameters.Desaturate.Amount, num2);
        if (this.m_overridingBlendToColor)
          return;
        this.m_currentParameters.BlendToColor.Amount = easingFunction(this.m_startParameters.BlendToColor.Amount, this.m_toParameters.BlendToColor.Amount, num2);
      }
    }
  }

  private void FinishEffect()
  {
    if (this.m_invokedOnFinishCallbacks)
      return;
    if (this.ActiveEffectsInstance == null)
    {
      this.m_currentParameters = this.m_toParameters;
      this.m_invokedOnFinishCallbacks = true;
    }
    else
    {
      if (this.ActiveEffectsInstance.OnFinishedCallback != null)
        this.ActiveEffectsInstance.OnFinishedCallback();
      if (this.ActiveEffectsInstance != null && this.ActiveEffectsInstance.Released)
      {
        this.m_toParameters = ScreenEffectParameters.None;
        this.m_currentParameters = ScreenEffectParameters.None;
        FullScreenFXMgr.Get().OnFinishedEffect(this.ActiveEffectsInstance);
        this.ActiveEffectsInstance = (FullScreenFXMgr.ScreenEffectsInstance) null;
      }
      else
        this.m_currentParameters = this.m_toParameters;
      this.m_invokedOnFinishCallbacks = true;
    }
  }
}
