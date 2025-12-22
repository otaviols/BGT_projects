using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStorePhoneCover : MonoBehaviour
{
  [CustomEditField(Sections = "General UI")]
  public GeneralStore m_parentStore;
  [CustomEditField(Sections = "General UI")]
  public PegUIElement m_backToCoverButton;
  [CustomEditField(Sections = "Animation")]
  public Animator m_animationController;
  [CustomEditField(Sections = "Animation")]
  public string m_buttonEnterAnimation = "";
  [CustomEditField(Sections = "Animation")]
  public List<GeneralStorePhoneCover.ModeAnimation> m_buttonExitAnimations = new List<GeneralStorePhoneCover.ModeAnimation>();
  [CustomEditField(Sections = "UI Blockers")]
  public GameObject m_coverClickArea;
  [CustomEditField(Sections = "UI Blockers")]
  public GameObject m_animationClickBlocker;
  [CustomEditField(Sections = "Aspect Ratio Scaling")]
  public float m_scale3to2_XZ = 0.39f;
  [CustomEditField(Sections = "Aspect Ratio Scaling")]
  public float m_scale16to9_XZ = 0.37f;
  [CustomEditField(Sections = "Aspect Ratio Scaling")]
  public float m_scaleExtraWide_XZ = 0.79f;
  [CustomEditField(Sections = "Aspect Ratio Scaling")]
  public float m_scaleY = 0.35f;
  private static GeneralStorePhoneCover s_instance;
  private const string s_coverAnimationCoroutine = "PlayAndWaitForAnimation";

  private void Awake()
  {
    GeneralStorePhoneCover.s_instance = this;
    this.m_parentStore.RegisterModeChangedListener(new GeneralStore.ModeChanged(this.OnGeneralStoreModeChanged));
    this.m_backToCoverButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => Navigation.GoBack()));
  }

  private void OnDestroy() => GeneralStorePhoneCover.s_instance = (GeneralStorePhoneCover) null;

  private void Start() => this.ShowCover();

  public void ShowCover()
  {
    this.UpdateCoverScale();
    this.StopCoroutine("PlayAndWaitForAnimation");
    this.StartCoroutine("PlayAndWaitForAnimation", (object) this.m_buttonEnterAnimation);
    this.m_coverClickArea.SetActive(true);
  }

  public void HideCover(GeneralStoreMode selectedMode)
  {
    this.StartCoroutine(this.PushBackMethodWhenShown());
    GeneralStorePhoneCover.ModeAnimation modeAnimation = this.m_buttonExitAnimations.Find((Predicate<GeneralStorePhoneCover.ModeAnimation>) (o => o.m_mode == selectedMode));
    if (modeAnimation == null)
      Debug.LogError((object) string.Format("Unable to find animation for {0} mode.", (object) selectedMode));
    else if (string.IsNullOrEmpty(modeAnimation.m_playAnimationName))
    {
      Debug.LogError((object) string.Format("Animation name not defined for {0} mode.", (object) selectedMode));
    }
    else
    {
      this.StopCoroutine("PlayAndWaitForAnimation");
      this.StartCoroutine("PlayAndWaitForAnimation", (object) modeAnimation.m_playAnimationName);
      this.m_coverClickArea.SetActive(false);
    }
  }

  private IEnumerator PushBackMethodWhenShown()
  {
    while (!this.m_parentStore.IsShown())
      yield return (object) null;
    Navigation.Push(new Navigation.NavigateBackHandler(GeneralStorePhoneCover.OnNavigateBack));
  }

  private void OnGeneralStoreModeChanged(GeneralStoreMode oldMode, GeneralStoreMode newMode)
  {
    if (newMode != GeneralStoreMode.NONE)
      this.HideCover(newMode);
    else
      this.ShowCover();
  }

  private IEnumerator PlayAndWaitForAnimation(string animationName)
  {
    this.m_animationController.enabled = true;
    this.m_animationController.StopPlayback();
    this.m_animationClickBlocker.SetActive(true);
    yield return (object) new WaitForEndOfFrame();
    this.m_animationController.Play(animationName);
    while ((double) this.m_animationController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0)
      yield return (object) null;
    this.m_animationClickBlocker.SetActive(false);
  }

  private void UpdateCoverScale()
  {
    float num = !TransformUtil.IsExtraWideAspectRatio() ? TransformUtil.GetAspectRatioDependentValue(this.m_scale3to2_XZ, this.m_scale16to9_XZ, this.m_scale16to9_XZ) : this.m_scaleExtraWide_XZ;
    this.transform.localScale = new Vector3(num, this.m_scaleY, num);
  }

  public static bool OnNavigateBack()
  {
    if ((UnityEngine.Object) GeneralStorePhoneCover.s_instance == (UnityEngine.Object) null)
      return false;
    GeneralStorePhoneCover.s_instance.m_parentStore.SetMode(GeneralStoreMode.NONE);
    return true;
  }

  public delegate void AnimationCallback();

  [Serializable]
  public class ModeAnimation
  {
    public GeneralStoreMode m_mode;
    public string m_playAnimationName;
  }
}
