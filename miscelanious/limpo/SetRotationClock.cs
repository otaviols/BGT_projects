using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using UnityEngine;

public class SetRotationClock : MonoBehaviour
{
  public Texture2D m_PreviousIcon;
  public Texture2D m_PreviousIconBlur;
  public Texture2D m_NewIcon;
  public Texture2D m_NewIconBlur;
  public Renderer m_GlassPanel;
  public float m_AnimationWaitTime = 5.5f;
  public GameObject m_CenterPanel;
  public float m_CenterPanelFlipTime = 1f;
  public GameObject m_SetRotationButton;
  public GameObject m_SetRotationButtonMesh;
  public GameObject m_SetRotationIconWidget;
  public float m_SetRotationButtonDelay = 0.75f;
  public float m_SetRotationButtonWobbleTime = 0.5f;
  public float m_ButtonRotationHoldTime = 1.5f;
  public GameObject m_ButtonRiseBone;
  public GameObject m_ButtonBanner;
  public UberText m_ButtonBannerStandard;
  public UberText m_ButtonBannerClassic;
  public Color m_ButtonBannerTextColor = Color.white;
  public float m_ButtonRiseTime = 1.75f;
  public float m_BlurScreenDelay = 0.5f;
  public float m_BlurScreenTime = 1f;
  public float m_MoveButtonUpZ = -0.1f;
  public float m_MoveButtonUpZphone = -0.3f;
  public float m_MoveButtonUpTime = 1f;
  public float m_ButtonFlipTime = 0.5f;
  public float m_ButtonToTrayAnimTime = 0.5f;
  public float m_EndBlurScreenDelay = 0.5f;
  public float m_EndBlurScreenTime = 1f;
  public float m_MoveButtonToTrayDelay = 1.5f;
  public float m_TextDelayTime = 1f;
  public float m_VeteranGhostedIconDelayTime = 3f;
  public ClockOverlayText m_overlayText;
  public GameObject m_ButtonGlowPlaneYellow;
  public GameObject m_ButtonGlowPlaneGreen;
  public ParticleSystem m_ImpactParticles;
  public AnimationCurve m_ButtonGlowAnimation;
  public PegUIElement m_clickCatcher;
  public AudioSource m_TheClockAmbientSound;
  public float m_TheClockAmbientSoundVolume = 1f;
  public float m_TheClockAmbientSoundFadeInTime = 2f;
  public float m_TheClockAmbientSoundFadeOutTime = 1f;
  public AudioSource m_ClickSound;
  public AudioSource m_Stage1Sound;
  public AudioSource m_Stage1Sound_Veteran;
  public AudioSource m_Stage2Sound;
  public AudioSource m_Stage2Sound_Veteran;
  public AudioSource m_Stage3Sound;
  public AudioSource m_Stage4Sound;
  public AudioSource m_Stage5Sound;
  public AudioSource m_Stage5Sound_Veteran;
  private bool m_clickCaptured;
  private Vector3 m_buttonBannerScale;
  private AudioSource m_ambientSound;
  private const float BUTTON_MESH_Z_ROTATION_FOR_CLASSIC = 0.0f;
  private const float BUTTON_MESH_Z_ROTATION_FOR_STANDARD = 180f;
  private static SetRotationClock s_instance;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    SetRotationClock.s_instance = this;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.transform.position = new Vector3(-60.7f, -18.939f, -43f);
      this.transform.localScale = new Vector3(9.043651f, 9.043651f, 9.043651f);
    }
    else
    {
      this.transform.position = new Vector3(-47.234f, -18.939f, -31.837f);
      this.transform.localScale = new Vector3(6.970411f, 6.970411f, 6.970411f);
    }
    this.m_overlayText.HideImmediate();
    this.m_clickCatcher.gameObject.SetActive(false);
    this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
    this.m_buttonBannerScale = this.m_ButtonBanner.transform.localScale;
    this.m_ButtonBannerStandard.TextColor = this.m_ButtonBannerTextColor;
    this.m_ButtonBannerClassic.TextColor = this.m_ButtonBannerTextColor;
    this.m_ButtonBanner.SetActive(false);
    this.m_ButtonBannerStandard.gameObject.SetActive(false);
    this.m_ButtonBannerClassic.gameObject.SetActive(false);
    Material sharedMaterial = this.m_GlassPanel.GetSharedMaterial();
    sharedMaterial.SetTexture("_BlendImage1", (Texture) this.m_PreviousIcon);
    sharedMaterial.SetTexture("_BlendImage2", (Texture) this.m_PreviousIconBlur);
    sharedMaterial.SetFloat("_BlendTransparency", 1f);
    sharedMaterial.SetFloat("_DistortionAmountX", 0.0f);
    sharedMaterial.SetFloat("_DistortionAmountY", 0.0f);
    sharedMaterial.SetFloat("_BlendImageSizeX", 6.5f);
    sharedMaterial.SetFloat("_BlendImageSizeY", 6.5f);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  public static SetRotationClock Get() => SetRotationClock.s_instance;

  public void StartTheClock()
  {
    this.m_SetRotationButton.SetActive(true);
    this.StartCoroutine(this.ClockAnimation());
  }

  public void ShakeCamera() => CameraShakeMgr.Shake(Camera.main, new Vector3(0.1f, 0.1f, 0.1f), 0.4f);

  public void SwapSetIcons()
  {
    Material sharedMaterial = this.m_GlassPanel.GetSharedMaterial();
    sharedMaterial.SetTexture("_BlendImage1", (Texture) this.m_NewIcon);
    sharedMaterial.SetTexture("_BlendImage2", (Texture) this.m_NewIconBlur);
  }

  public IEnumerator ClockAnimation()
  {
    bool veteranFlow = SetRotationManager.HasSeenStandardModeTutorial();
    AudioSource clickSound = (AudioSource) null;
    if ((UnityEngine.Object) this.m_ClickSound != (UnityEngine.Object) null)
      clickSound = UnityEngine.Object.Instantiate<AudioSource>(this.m_ClickSound);
    while (!DeckPickerTrayDisplay.Get().IsLoaded())
      yield return (object) null;
    DeckPickerTrayDisplay.Get().InitSetRotationTutorial(veteranFlow);
    if (!veteranFlow)
    {
      this.PlayClockAnimation();
      if ((UnityEngine.Object) this.m_Stage1Sound != (UnityEngine.Object) null)
        SoundManager.Get().Play(UnityEngine.Object.Instantiate<AudioSource>(this.m_Stage1Sound));
      if ((UnityEngine.Object) this.m_TheClockAmbientSound != (UnityEngine.Object) null)
        this.FadeInAmbientSound();
      yield return (object) new WaitForSeconds(this.m_AnimationWaitTime);
      this.VignetteBackground(0.5f);
      this.m_clickCatcher.gameObject.SetActive(true);
      this.m_clickCaptured = false;
      this.m_overlayText.UpdateText(0);
      this.m_overlayText.Show();
      yield return (object) new WaitForSeconds(this.m_TextDelayTime);
      while (!this.m_clickCaptured)
        yield return (object) null;
      if ((UnityEngine.Object) this.m_Stage2Sound != (UnityEngine.Object) null)
        SoundManager.Get().Play(UnityEngine.Object.Instantiate<AudioSource>(this.m_Stage2Sound));
      if ((UnityEngine.Object) clickSound != (UnityEngine.Object) null)
        SoundManager.Get().Play(clickSound);
      this.StopVignetteBackground(0.5f);
      this.m_clickCatcher.gameObject.SetActive(false);
      this.m_overlayText.Hide();
      yield return (object) new WaitForSeconds(this.m_TextDelayTime);
    }
    else
    {
      if ((UnityEngine.Object) this.m_TheClockAmbientSound != (UnityEngine.Object) null)
        this.FadeInAmbientSound();
      yield return (object) new WaitForSeconds(this.m_VeteranGhostedIconDelayTime);
      if ((UnityEngine.Object) this.m_Stage2Sound_Veteran != (UnityEngine.Object) null)
        SoundManager.Get().Play(UnityEngine.Object.Instantiate<AudioSource>(this.m_Stage2Sound_Veteran));
    }
    this.FlipCenterPanelButton();
    yield return (object) new WaitForSeconds(this.m_ButtonRotationHoldTime);
    this.RaiseButton();
    yield return (object) new WaitForSeconds(this.m_BlurScreenDelay);
    this.BlurBackground(this.m_BlurScreenTime);
    yield return (object) new WaitForSeconds(this.m_BlurScreenTime);
    this.m_clickCatcher.gameObject.SetActive(true);
    this.m_clickCaptured = false;
    this.m_overlayText.UpdateText(1);
    this.m_overlayText.Show();
    yield return (object) new WaitForSeconds(this.m_TextDelayTime);
    while (!this.m_clickCaptured)
      yield return (object) null;
    if ((UnityEngine.Object) clickSound != (UnityEngine.Object) null)
      SoundManager.Get().Play(clickSound);
    this.m_clickCatcher.gameObject.SetActive(false);
    this.m_overlayText.Hide();
    if ((UnityEngine.Object) this.m_Stage3Sound != (UnityEngine.Object) null)
      SoundManager.Get().Play(UnityEngine.Object.Instantiate<AudioSource>(this.m_Stage3Sound));
    this.MoveButtonUp();
    yield return (object) new WaitForSeconds(this.m_TextDelayTime);
    this.m_clickCatcher.gameObject.SetActive(true);
    this.m_clickCaptured = false;
    this.ShowButtonBanner();
    this.ShowButtonYellowGlow();
    TournamentDisplay.Get().SetRotationSlideIn();
    this.FadeOutAmbientSound();
    while (!this.m_clickCaptured)
      yield return (object) null;
    if ((UnityEngine.Object) clickSound != (UnityEngine.Object) null)
      SoundManager.Get().Play(clickSound);
    this.m_clickCatcher.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_Stage4Sound != (UnityEngine.Object) null)
      SoundManager.Get().Play(UnityEngine.Object.Instantiate<AudioSource>(this.m_Stage4Sound));
    while (TournamentDisplay.Get().SlidingInForSetRotation)
      yield return (object) null;
    if ((UnityEngine.Object) clickSound != (UnityEngine.Object) null)
      SoundManager.Get().Play(clickSound);
    this.m_clickCatcher.gameObject.SetActive(false);
    this.HideButtonBanner();
    this.StopBlurBackground(this.m_EndBlurScreenTime);
    this.StopButtonDrift();
    this.EndClockStartTutorial();
  }

  private void FadeInAmbientSound()
  {
    if ((UnityEngine.Object) this.m_TheClockAmbientSound == (UnityEngine.Object) null)
      return;
    this.m_ambientSound = UnityEngine.Object.Instantiate<AudioSource>(this.m_TheClockAmbientSound);
    SoundManager.Get().SetVolume(this.m_ambientSound, 0.01f);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "name", (object) "TheClockAmbientSound", (object) "from", (object) 0.01f, (object) "to", (object) this.m_TheClockAmbientSoundVolume, (object) "time", (object) this.m_TheClockAmbientSoundFadeInTime, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (amount => SoundManager.Get().SetVolume(this.m_ambientSound, (float) amount)), (object) "onupdatetarget", (object) this.gameObject));
    SoundManager.Get().Play(this.m_ambientSound);
  }

  private void FadeOutAmbientSound()
  {
    if ((UnityEngine.Object) this.m_ambientSound == (UnityEngine.Object) null)
      return;
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "name", (object) "TheClockAmbientSound", (object) "from", (object) this.m_TheClockAmbientSoundVolume, (object) "to", (object) 0.0f, (object) "time", (object) this.m_TheClockAmbientSoundFadeOutTime, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (amount => SoundManager.Get().SetVolume(this.m_ambientSound, (float) amount)), (object) "onupdatetarget", (object) this.gameObject, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "StopAmbientSound"));
  }

  private void StopAmbientSound()
  {
    if ((UnityEngine.Object) this.m_ambientSound == (UnityEngine.Object) null)
      return;
    SoundManager.Get().Stop(this.m_ambientSound);
  }

  private void PlayClockAnimation()
  {
    Animator component = this.GetComponent<Animator>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.SetTrigger("StartClock");
  }

  private void AnimateButtonToTournamentTray() => TournamentDisplay.Get().SetRotationSlideIn();

  private void FlipCenterPanelButton()
  {
    iTween.RotateTo(this.m_CenterPanel, iTween.Hash((object) "z", (object) 180f, (object) "time", (object) this.m_CenterPanelFlipTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce));
    this.m_SetRotationButton.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -10f);
    iTween.RotateTo(this.m_SetRotationButton, iTween.Hash((object) "z", (object) 0.0f, (object) "delay", (object) this.m_SetRotationButtonDelay, (object) "time", (object) this.m_SetRotationButtonWobbleTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce));
  }

  private void RaiseButton()
  {
    this.GetComponent<Animator>().SetTrigger(nameof (RaiseButton));
    LayerUtils.SetLayer(this.m_SetRotationButton, GameLayer.IgnoreFullScreenEffects);
    iTween.MoveTo(this.m_SetRotationButton, iTween.Hash((object) "position", (object) this.m_ButtonRiseBone.transform.position, (object) "delay", (object) 0.0f, (object) "time", (object) this.m_ButtonRiseTime, (object) "islocal", (object) false, (object) "easetype", (object) iTween.EaseType.easeInOutQuint, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "RaiseButtonComplete"));
  }

  private void RaiseButtonComplete()
  {
    TokyoDrift componentInChildren = this.m_SetRotationButton.GetComponentInChildren<TokyoDrift>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    componentInChildren.enabled = true;
  }

  private void StopButtonDrift()
  {
    this.m_ButtonBanner.SetActive(false);
    this.m_ButtonBannerStandard.gameObject.SetActive(false);
    this.m_ButtonBannerClassic.gameObject.SetActive(false);
    TokyoDrift componentInChildren = this.m_SetRotationButton.GetComponentInChildren<TokyoDrift>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    componentInChildren.enabled = false;
  }

  private void ShowButtonBanner()
  {
    this.m_ButtonBanner.SetActive(true);
    this.m_ButtonBannerStandard.gameObject.SetActive(true);
    this.m_ButtonBanner.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash((object) "scale", (object) this.m_buttonBannerScale, (object) "time", (object) 0.15f, (object) "easetype", (object) iTween.EaseType.easeOutQuad));
  }

  private void ShowButtonYellowGlow() => iTween.ValueTo(this.gameObject, iTween.Hash((object) "islocal", (object) true, (object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) 0.3f, (object) "easeType", (object) iTween.EaseType.easeOutExpo, (object) "onupdate", (object) (Action<object>) (value => this.m_ButtonGlowPlaneYellow.GetComponent<Renderer>().GetMaterial().SetFloat("_Intensity", (float) value)), (object) "onupdatetarget", (object) this.gameObject));

  private void CrossFadeToGreenGlow()
  {
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "islocal", (object) true, (object) "from", (object) 1f, (object) "to", (object) 0.0f, (object) "time", (object) 0.3f, (object) "easeType", (object) iTween.EaseType.easeOutExpo, (object) "onupdate", (object) (Action<object>) (value => this.m_ButtonGlowPlaneYellow.GetComponent<Renderer>().GetMaterial().SetFloat("_Intensity", (float) value)), (object) "onupdatetarget", (object) this.m_ButtonGlowPlaneYellow));
    iTween.ValueTo(this.m_ButtonGlowPlaneGreen, iTween.Hash((object) "islocal", (object) true, (object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) 0.3f, (object) "easeType", (object) iTween.EaseType.easeOutExpo, (object) "onupdate", (object) (Action<object>) (value => this.m_ButtonGlowPlaneGreen.GetComponent<Renderer>().GetMaterial().SetFloat("_Intensity", (float) value)), (object) "onupdatetarget", (object) this.gameObject));
  }

  private void ButtonBannerCrossFadeText()
  {
    this.m_ButtonBannerStandard.gameObject.SetActive(true);
    this.m_ButtonBannerClassic.gameObject.SetActive(true);
    this.m_ButtonBannerClassic.TextColor = this.m_ButtonBannerClassic.TextColor with
    {
      a = 0.0f
    };
    iTween.FadeTo(this.m_ButtonBannerStandard.gameObject, 0.0f, this.m_ButtonFlipTime * 0.1f);
    iTween.FadeTo(this.m_ButtonBannerClassic.gameObject, 1f, this.m_ButtonFlipTime * 0.1f);
  }

  private void ButtonBannerPunch()
  {
    Vector3 localScale = this.m_ButtonBanner.transform.localScale;
    iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash((object) "scale", (object) (localScale * 1.5f), (object) "time", (object) 0.075f, (object) "delay", (object) (float) ((double) this.m_ButtonFlipTime * 0.25), (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "onupdatetarget", (object) this.gameObject));
    iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.25f, (object) "delay", (object) (float) ((double) this.m_ButtonFlipTime * 0.25 + 0.0750000029802322), (object) "easetype", (object) iTween.EaseType.easeInOutQuad, (object) "onupdatetarget", (object) this.gameObject));
  }

  private void HideButtonBanner() => iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "time", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeInQuad, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "HideButtonBannerComplete"));

  private void HideButtonBannerComplete() => this.m_ButtonBanner.SetActive(false);

  private void FlipButton() => iTween.RotateTo(this.m_SetRotationButtonMesh, iTween.Hash((object) "z", (object) 0.0f, (object) "time", (object) this.m_ButtonFlipTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic));

  private void MoveButtonUp()
  {
    float num = this.m_MoveButtonUpZ;
    if ((bool) UniversalInputManager.UsePhoneUI)
      num = this.m_MoveButtonUpZphone;
    iTween.MoveTo(this.m_SetRotationButton, iTween.Hash((object) "z", (object) num, (object) "delay", (object) 0.0f, (object) "time", (object) this.m_MoveButtonUpTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
  }

  private void VignetteBackground(float time)
  {
    ScreenEffectParameters vignettePerspective = ScreenEffectParameters.VignettePerspective;
    vignettePerspective.Vignette.Amount = 0.99f;
    vignettePerspective.EaseType = iTween.EaseType.easeOutCubic;
    this.m_screenEffectsHandle.StartEffect(vignettePerspective);
  }

  private void StopVignetteBackground(float time) => this.m_screenEffectsHandle.StopEffect(time, iTween.EaseType.easeInCubic);

  private void BlurBackground(float time) => this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.VignettePerspective with
  {
    Time = time
  });

  private void StopBlurBackground(float time) => this.m_screenEffectsHandle.StopEffect(time);

  private void MoveButtonToDeckPickerTray(bool socketAsClassic)
  {
    this.StopButtonDrift();
    Vector3 b = Vector3.zero;
    Vector3 vector3_1 = Vector3.one;
    GameObject theClockButtonBone = DeckPickerTrayDisplay.Get().m_TheClockButtonBone;
    if ((UnityEngine.Object) theClockButtonBone != (UnityEngine.Object) null)
    {
      b = theClockButtonBone.transform.position;
      vector3_1 = theClockButtonBone.transform.localScale;
    }
    Vector3 vector3_2 = Vector3.Lerp(this.m_SetRotationButton.transform.position, b, 0.75f);
    vector3_2 = new Vector3(vector3_2.x + 7f, vector3_2.y, vector3_2.z);
    Vector3[] vector3Array = new Vector3[3]
    {
      this.m_SetRotationButton.transform.position,
      vector3_2,
      b
    };
    this.GetComponent<Animator>().SetTrigger("SocketButton");
    iTween.MoveTo(this.m_SetRotationButton, iTween.Hash((object) "path", (object) vector3Array, (object) "delay", (object) 0.0f, (object) "time", (object) this.m_ButtonToTrayAnimTime, (object) "islocal", (object) false, (object) "easetype", (object) iTween.EaseType.easeInOutQuint, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "ButtonImpactAndShutdownTheClock"));
    iTween.RotateTo(this.m_SetRotationButtonMesh, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, socketAsClassic ? 0.0f : 180f), (object) "time", (object) this.m_ButtonToTrayAnimTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
    iTween.RotateTo(this.m_SetRotationButton, iTween.Hash((object) "rotation", (object) Vector3.zero, (object) "time", (object) this.m_ButtonToTrayAnimTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
    iTween.ScaleTo(this.m_SetRotationButton, iTween.Hash((object) "scale", (object) vector3_1, (object) "delay", (object) 0.0f, (object) "time", (object) this.m_ButtonToTrayAnimTime, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
  }

  private void ButtonImpactAndShutdownTheClock()
  {
    this.ShakeCamera();
    this.m_ImpactParticles.Play();
    this.StartCoroutine(this.FinalGlowAndDisableTheClock());
  }

  private IEnumerator FinalGlowAndDisableTheClock()
  {
    SetRotationClock setRotationClock = this;
    setRotationClock.EndClockStartTutorial();
    Material glowMat = (SetRotationManager.HasSeenStandardModeTutorial() ? setRotationClock.m_ButtonGlowPlaneYellow.GetComponent<Renderer>() : setRotationClock.m_ButtonGlowPlaneGreen.GetComponent<Renderer>()).GetMaterial();
    float animLength = setRotationClock.m_ButtonGlowAnimation[setRotationClock.m_ButtonGlowAnimation.length - 1].time;
    float animTime = 0.0f;
    while ((double) animTime < (double) animLength)
    {
      animTime += Time.deltaTime;
      glowMat.SetFloat("_Intensity", setRotationClock.m_ButtonGlowAnimation.Evaluate(animTime));
      yield return (object) null;
    }
    yield return (object) new WaitForSeconds(3f);
    setRotationClock.gameObject.SetActive(false);
  }

  private void DisableTheClock() => this.gameObject.SetActive(false);

  private void EndClockStartTutorial()
  {
    SetRotationClock.DisableTheClockCallback callback = new SetRotationClock.DisableTheClockCallback(this.DisableTheClock);
    this.GetComponent<Animator>().StopPlayback();
    DeckPickerTrayDisplay.Get().StartSetRotationTutorial(callback);
  }

  private void OnClick(UIEvent e) => this.m_clickCaptured = true;

  public delegate void DisableTheClockCallback();
}
