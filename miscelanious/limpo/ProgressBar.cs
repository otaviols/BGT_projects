using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
  public TextMesh m_label;
  public UberText m_uberLabel;
  public float m_increaseAnimTime = 2f;
  public float m_decreaseAnimTime = 1f;
  public float m_coolDownAnimTime = 1f;
  public float m_barIntensity = 1.2f;
  public float m_barIntensityIncreaseMax = 3f;
  public float m_audioFadeInOut = 0.2f;
  public float m_increasePitchStart = 1f;
  public float m_increasePitchEnd = 1.2f;
  public float m_decreasePitchStart = 1f;
  public float m_decreasePitchEnd = 0.8f;
  private Material m_barMaterial;
  private float m_prevVal;
  private float m_currVal;
  private float m_factor;
  private float m_maxIntensity;
  private float m_Uadd;
  private float m_animationTime;
  private float m_progress;
  private float m_animationValueLastFrame;

  [Overridable]
  public float Progress
  {
    get => this.m_progress;
    set => this.SetProgressBar(value);
  }

  public event Action OnProgressBarFilled;

  public void Awake() => this.m_barMaterial = this.GetComponent<Renderer>().GetMaterial();

  public void OnDestroy() => UnityEngine.Object.Destroy((UnityEngine.Object) this.m_barMaterial);

  public void SetMaterial(Material material) => this.m_barMaterial = material;

  public void AnimateProgress(float prevVal, float currVal, iTween.EaseType easeType = iTween.EaseType.easeOutQuad)
  {
    this.m_prevVal = prevVal;
    this.m_currVal = currVal;
    this.m_factor = (double) this.m_currVal <= (double) this.m_prevVal ? this.m_prevVal - this.m_currVal : this.m_currVal - this.m_prevVal;
    this.m_factor = Mathf.Abs(this.m_factor);
    this.m_animationValueLastFrame = prevVal;
    if ((double) this.m_currVal > (double) this.m_prevVal)
      this.IncreaseProgress(this.m_currVal, this.m_prevVal, easeType);
    else
      this.DecreaseProgress(this.m_currVal, this.m_prevVal);
  }

  public void SetProgressBar(float progress)
  {
    this.m_progress = Mathf.Repeat(progress, 1f);
    if ((double) progress % 1.0 == 0.0 && (double) progress != 0.0)
      this.m_progress = 1f;
    if ((UnityEngine.Object) this.m_barMaterial == (UnityEngine.Object) null)
      this.m_barMaterial = this.GetComponent<Renderer>().GetMaterial();
    this.m_barMaterial.SetFloat("_Intensity", this.m_barIntensity);
    this.m_barMaterial.SetFloat("_Percent", this.m_progress);
  }

  public float GetAnimationTime() => this.m_animationTime;

  public void SetLabel(string text)
  {
    if ((UnityEngine.Object) this.m_uberLabel != (UnityEngine.Object) null)
      this.m_uberLabel.Text = text;
    if (!((UnityEngine.Object) this.m_label != (UnityEngine.Object) null))
      return;
    this.m_label.text = text;
  }

  public void SetBarTexture(Texture texture)
  {
    if ((UnityEngine.Object) this.m_barMaterial == (UnityEngine.Object) null)
      this.m_barMaterial = this.GetComponent<Renderer>().GetMaterial();
    this.m_barMaterial.SetTexture("_NoiseTex", texture);
  }

  private void IncreaseProgress(float currProgress, float prevProgress, iTween.EaseType easeType)
  {
    float num = this.m_increaseAnimTime * this.m_factor;
    this.m_animationTime = num;
    Hashtable args1 = iTween.Hash((object) "from", (object) prevProgress, (object) "to", (object) currProgress, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "Progress_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) nameof (IncreaseProgress));
    iTween.StopByName(this.gameObject, nameof (IncreaseProgress));
    iTween.ValueTo(this.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 0.005f, (object) "time", (object) num, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "onupdate", (object) "ScrollSpeed_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "UVSpeed");
    iTween.StopByName(this.gameObject, "UVSpeed");
    iTween.ValueTo(this.gameObject, args2);
    this.m_maxIntensity = this.m_barIntensity + (this.m_barIntensityIncreaseMax - this.m_barIntensity) * Mathf.Clamp01(this.m_factor);
    Hashtable args3 = iTween.Hash((object) "from", (object) this.m_barIntensity, (object) "to", (object) this.m_maxIntensity, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "Intensity_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "Intensity", (object) "oncomplete", (object) "Intensity_OnComplete", (object) "oncompletetarget", (object) this.gameObject);
    iTween.StopByName(this.gameObject, "Intensity");
    iTween.ValueTo(this.gameObject, args3);
    AudioSource component;
    if (this.TryGetComponent<AudioSource>(out component))
    {
      SoundManager.Get().SetVolume(component, 0.0f);
      SoundManager.Get().SetPitch(component, this.m_increasePitchStart);
      SoundManager.Get().Play(component);
    }
    Hashtable args4 = iTween.Hash((object) "from", (object) 0, (object) "to", (object) 1, (object) "time", (object) (float) ((double) num * (double) this.m_audioFadeInOut), (object) "delay", (object) 0, (object) "easetype", (object) easeType, (object) "onupdate", (object) "AudioVolume_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "barVolumeStart");
    iTween.StopByName(this.gameObject, "barVolumeStart");
    iTween.ValueTo(this.gameObject, args4);
    Hashtable args5 = iTween.Hash((object) "from", (object) 1, (object) "to", (object) 0, (object) "time", (object) (float) ((double) num * (double) this.m_audioFadeInOut), (object) "delay", (object) (float) ((double) num * (1.0 - (double) this.m_audioFadeInOut)), (object) "easetype", (object) easeType, (object) "onupdate", (object) "AudioVolume_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "oncomplete", (object) "AudioVolume_OnComplete", (object) "name", (object) "barVolumeEnd");
    iTween.StopByName(this.gameObject, "barVolumeEnd");
    iTween.ValueTo(this.gameObject, args5);
    Hashtable args6 = iTween.Hash((object) "from", (object) this.m_increasePitchStart, (object) "to", (object) this.m_increasePitchEnd, (object) "time", (object) num, (object) "delay", (object) 0, (object) "easetype", (object) easeType, (object) "onupdate", (object) "AudioPitch_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "barPitch");
    iTween.StopByName(this.gameObject, "barPitch");
    iTween.ValueTo(this.gameObject, args6);
  }

  private void Progress_OnUpdate(float val)
  {
    if ((UnityEngine.Object) this.m_barMaterial == (UnityEngine.Object) null)
      this.m_barMaterial = this.GetComponent<Renderer>().GetMaterial();
    float num = Mathf.Repeat(val, 1f);
    if ((double) val % 1.0 == 0.0 && (double) val != 0.0)
      num = 1f;
    this.m_barMaterial.SetFloat("_Percent", num);
    if ((double) this.m_animationValueLastFrame > (double) num && (double) this.m_animationValueLastFrame != 1.0 || (double) num == 1.0)
    {
      Action progressBarFilled = this.OnProgressBarFilled;
      if (progressBarFilled != null)
        progressBarFilled();
    }
    this.m_animationValueLastFrame = num;
  }

  private void Intensity_OnComplete()
  {
    iTween.StopByName(this.gameObject, "Increase");
    iTween.StopByName(this.gameObject, "Intensity");
    iTween.StopByName(this.gameObject, "UVSpeed");
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) this.m_maxIntensity, (object) "to", (object) this.m_barIntensity, (object) "time", (object) this.m_coolDownAnimTime, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "onupdate", (object) "Intensity_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "Intensity"));
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) 0.005f, (object) "to", (object) 0.0f, (object) "time", (object) this.m_coolDownAnimTime, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "onupdate", (object) "ScrollSpeed_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "UVSpeed"));
  }

  private void Intensity_OnUpdate(float val)
  {
    if ((UnityEngine.Object) this.m_barMaterial == (UnityEngine.Object) null)
      this.m_barMaterial = this.GetComponent<Renderer>().GetMaterial();
    this.m_barMaterial.SetFloat("_Intensity", val);
  }

  private void ScrollSpeed_OnUpdate(float val)
  {
    if ((UnityEngine.Object) this.m_barMaterial == (UnityEngine.Object) null)
      this.m_barMaterial = this.GetComponent<Renderer>().GetMaterial();
    this.m_Uadd += val;
    this.m_barMaterial.SetFloat("_Uadd", this.m_Uadd);
  }

  private void AudioVolume_OnUpdate(float val)
  {
    AudioSource component;
    if (!this.TryGetComponent<AudioSource>(out component))
      return;
    SoundManager.Get().SetVolume(component, val);
  }

  private void AudioVolume_OnComplete()
  {
    AudioSource component;
    if (!this.TryGetComponent<AudioSource>(out component))
      return;
    SoundManager.Get().Stop(component);
  }

  private void AudioPitch_OnUpdate(float val)
  {
    AudioSource component;
    if (!this.TryGetComponent<AudioSource>(out component))
      return;
    SoundManager.Get().SetPitch(component, val);
  }

  private void DecreaseProgress(float currProgress, float prevProgress)
  {
    float num = this.m_decreaseAnimTime * this.m_factor;
    this.m_animationTime = num;
    iTween.EaseType easeType = iTween.EaseType.easeInOutCubic;
    Hashtable args1 = iTween.Hash((object) "from", (object) prevProgress, (object) "to", (object) currProgress, (object) "time", (object) num, (object) "easetype", (object) easeType, (object) "onupdate", (object) "Progress_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "Decrease");
    iTween.StopByName(this.gameObject, "Decrease");
    iTween.ValueTo(this.gameObject, args1);
    AudioSource component;
    if (this.TryGetComponent<AudioSource>(out component))
    {
      SoundManager.Get().SetVolume(component, 0.0f);
      SoundManager.Get().SetPitch(component, this.m_decreasePitchStart);
      SoundManager.Get().Play(component);
    }
    Hashtable args2 = iTween.Hash((object) "from", (object) 0, (object) "to", (object) 1, (object) "time", (object) (float) ((double) num * (double) this.m_audioFadeInOut), (object) "delay", (object) 0, (object) "easetype", (object) easeType, (object) "onupdate", (object) "AudioVolume_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "barVolumeStart");
    iTween.StopByName(this.gameObject, "barVolumeStart");
    iTween.ValueTo(this.gameObject, args2);
    Hashtable args3 = iTween.Hash((object) "from", (object) 1, (object) "to", (object) 0, (object) "time", (object) (float) ((double) num * (double) this.m_audioFadeInOut), (object) "delay", (object) (float) ((double) num * (1.0 - (double) this.m_audioFadeInOut)), (object) "easetype", (object) easeType, (object) "onupdate", (object) "AudioVolume_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "oncomplete", (object) "AudioVolume_OnComplete", (object) "name", (object) "barVolumeEnd");
    iTween.StopByName(this.gameObject, "barVolumeEnd");
    iTween.ValueTo(this.gameObject, args3);
    Hashtable args4 = iTween.Hash((object) "from", (object) this.m_decreasePitchStart, (object) "to", (object) this.m_decreasePitchEnd, (object) "time", (object) num, (object) "delay", (object) 0, (object) "easetype", (object) easeType, (object) "onupdate", (object) "AudioPitch_OnUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "barPitch");
    iTween.StopByName(this.gameObject, "barPitch");
    iTween.ValueTo(this.gameObject, args4);
  }
}
