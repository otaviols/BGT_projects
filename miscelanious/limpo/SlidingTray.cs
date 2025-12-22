using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone.UI.Core;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class SlidingTray : MonoBehaviour
{
  [CustomEditField(Sections = "Bones")]
  public Transform m_trayHiddenBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_trayShownBone;
  [CustomEditField(Sections = "Parameters")]
  public bool m_inactivateOnHide = true;
  [Tooltip("Useful to use (instead of 'inactivate On Hide') when the SlidingTray has Widgets on it that you want to load before it gets shown.")]
  [CustomEditField(Sections = "Parameters")]
  public bool m_invisibleOnHide;
  [CustomEditField(Sections = "Parameters")]
  public bool m_useNavigationBack;
  [CustomEditField(Sections = "Parameters")]
  public bool m_playAudioOnSlide = true;
  [CustomEditField(Sections = "Parameters")]
  public string m_SlideOnSFXAssetString = "choose_opponent_panel_slide_on.prefab:66491d3d01ed663429ab80daf6a5e880";
  [CustomEditField(Sections = "Parameters")]
  public string m_SlideOffSFXAssetString = "choose_opponent_panel_slide_off.prefab:3139d09eb94899d41b9bf612649f47bf";
  [CustomEditField(Sections = "Parameters")]
  public float m_traySlideDuration = 0.5f;
  [CustomEditField(Sections = "Parameters")]
  public bool m_animateBounce;
  [CustomEditField(Sections = "Parameters")]
  public float m_animateBlurInTime = 0.4f;
  [CustomEditField(Sections = "Parameters")]
  public float m_animateBlurOutTime = 0.2f;
  [CustomEditField(Sections = "Optional Features")]
  public PegUIElement m_offClickCatcher;
  [CustomEditField(Sections = "Optional Features")]
  public MeshRenderer m_darkenQuad;
  [CustomEditField(Sections = "Optional Features")]
  public PegUIElement m_traySliderButton;
  private bool m_trayShown;
  private bool m_traySliderAnimating;
  private SlidingTray.TrayToggledListener m_trayToggledListener;
  private bool m_startingPositionSet;
  private GameLayer m_hiddenLayer;
  private GameLayer m_shownLayer = GameLayer.IgnoreFullScreenEffects;
  private Color m_quadHiddenColor = Color.white;
  private Color m_quadShownColor = new Color(0.53f, 0.53f, 0.53f, 1f);
  private float m_currentQuadFade;
  private readonly Vector3 INVISIBLE_POSITION = new Vector3(0.0f, 0.0f, -500f);
  private SceneMgr.Mode m_sceneContext;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public event Action OnTransitionComplete;

  [CustomEditField(Hide = true)]
  [Overridable]
  public bool PlayAudioOnSlide
  {
    get => this.m_playAudioOnSlide;
    set => this.m_playAudioOnSlide = value;
  }

  private void Awake()
  {
    int num = (bool) UniversalInputManager.UsePhoneUI ? 1 : 0;
    if ((UnityEngine.Object) this.m_traySliderButton != (UnityEngine.Object) null)
      this.m_traySliderButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTraySliderPressed));
    if ((UnityEngine.Object) this.m_offClickCatcher != (UnityEngine.Object) null)
      this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherPressed));
    if ((UnityEngine.Object) this.m_darkenQuad != (UnityEngine.Object) null)
    {
      this.m_darkenQuad.gameObject.SetActive(false);
      this.m_darkenQuad.GetMaterial().color = this.m_quadHiddenColor;
    }
    if (this.m_invisibleOnHide)
      this.transform.localPosition = this.INVISIBLE_POSITION;
    if (SceneMgr.Get() != null)
      this.m_sceneContext = SceneMgr.Get().GetMode();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Start()
  {
    if (this.m_startingPositionSet)
      return;
    if (this.m_invisibleOnHide)
      this.transform.localPosition = this.INVISIBLE_POSITION;
    else
      this.transform.localPosition = this.m_trayHiddenBone.localPosition;
    this.m_trayShown = false;
    if (this.m_inactivateOnHide)
      this.gameObject.SetActive(false);
    this.m_startingPositionSet = true;
  }

  private void OnDestroy()
  {
    if ((UnityEngine.Object) this.m_offClickCatcher != (UnityEngine.Object) null)
      this.m_offClickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherPressed));
    if ((UnityEngine.Object) this.m_traySliderButton != (UnityEngine.Object) null)
      this.m_traySliderButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTraySliderPressed));
    if (FullScreenFXMgr.Get() == null || this.m_sceneContext == SceneMgr.Mode.GAME_MODE)
      return;
    this.m_screenEffectsHandle.StopEffect(0.0f);
  }

  [ContextMenu("Show")]
  public void ShowTray() => this.ToggleTraySlider(true);

  [ContextMenu("Hide")]
  public void HideTray() => this.ToggleTraySlider(false);

  public void ToggleTraySlider(bool show, Transform target = null, bool animate = true)
  {
    if (this.m_trayShown == show)
      return;
    if (show && (UnityEngine.Object) target != (UnityEngine.Object) null)
      this.m_trayShownBone = target;
    this.m_trayShown = show;
    if (show)
      this.DoShowLogic(animate);
    else
      this.DoHideLogic(animate);
    this.m_startingPositionSet = true;
    if (this.m_trayToggledListener == null)
      return;
    this.m_trayToggledListener(show);
  }

  public bool TraySliderIsAnimating() => this.m_traySliderAnimating;

  public bool IsAnimatingToShow() => this.m_traySliderAnimating && this.m_trayShown;

  public bool IsAnimatingToHide() => this.m_traySliderAnimating && !this.m_trayShown;

  public bool IsTrayInShownPosition() => this.gameObject.transform.localPosition == this.m_trayShownBone.localPosition;

  public bool IsShown() => this.m_trayShown;

  public void RegisterTrayToggleListener(SlidingTray.TrayToggledListener listener) => this.m_trayToggledListener = listener;

  public void UnregisterTrayToggleListener(SlidingTray.TrayToggledListener listener)
  {
    if (this.m_trayToggledListener == listener)
      this.m_trayToggledListener = (SlidingTray.TrayToggledListener) null;
    else
      Log.All.Print("Attempting to unregister a TrayToggleListener that has not been registered!");
  }

  public void SetLayers(GameLayer visible, GameLayer hidden)
  {
    this.m_shownLayer = visible;
    this.m_hiddenLayer = hidden;
  }

  private void DoShowLogic(bool animate)
  {
    if (this.m_useNavigationBack)
      Navigation.Push(new Navigation.NavigateBackHandler(this.BackPressed));
    this.gameObject.SetActive(true);
    if (this.gameObject.activeInHierarchy & animate)
    {
      this.transform.localPosition = this.m_trayHiddenBone.localPosition;
      iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_trayShownBone.localPosition, (object) "isLocal", (object) true, (object) "time", (object) this.m_traySlideDuration, (object) "oncomplete", (object) "OnTraySliderAnimFinished", (object) "oncompletetarget", (object) this.gameObject, (object) "easetype", (object) (iTween.EaseType) (this.m_animateBounce ? 24 : (int) iTween.Defaults.easeType)));
      this.m_traySliderAnimating = true;
      if ((UnityEngine.Object) this.m_offClickCatcher != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_darkenQuad != (UnityEngine.Object) null)
        {
          this.m_darkenQuad.gameObject.SetActive(true);
          iTween.Stop(this.m_darkenQuad.gameObject);
          iTween.ValueTo(this.m_darkenQuad.gameObject, iTween.Hash((object) "from", (object) this.m_currentQuadFade, (object) "to", (object) 1f, (object) "time", (object) this.m_animateBlurInTime, (object) "onupdate", (object) "DarkenQuadFade_Update", (object) "onupdatetarget", (object) this.gameObject));
        }
        else
          this.FadeEffectsIn(this.m_animateBlurInTime);
        this.m_offClickCatcher.gameObject.SetActive(true);
      }
      if (!this.m_playAudioOnSlide)
        return;
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(this.m_SlideOnSFXAssetString), this.gameObject);
    }
    else
    {
      this.gameObject.transform.localPosition = this.m_trayShownBone.localPosition;
      if ((UnityEngine.Object) this.m_darkenQuad != (UnityEngine.Object) null)
      {
        iTween.Stop(this.m_darkenQuad.gameObject);
        this.m_currentQuadFade = 1f;
        this.m_darkenQuad.GetMaterial().color = this.m_quadShownColor;
      }
      this.OnTraySliderAnimFinished();
    }
  }

  private void DoHideLogic(bool animate)
  {
    if (this.m_useNavigationBack)
      Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.BackPressed));
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.m_trayHiddenBone == (UnityEngine.Object) null)
      return;
    if (this.gameObject.activeInHierarchy & animate)
    {
      iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.m_trayHiddenBone.localPosition, (object) "isLocal", (object) true, (object) "oncomplete", (object) "OnTraySliderAnimFinished", (object) "oncompletetarget", (object) this.gameObject, (object) "time", (object) (float) (this.m_animateBounce ? (double) this.m_traySlideDuration : (double) this.m_traySlideDuration / 2.0), (object) "easetype", (object) (iTween.EaseType) (this.m_animateBounce ? 24 : 21)));
      this.m_traySliderAnimating = true;
      if ((UnityEngine.Object) this.m_offClickCatcher != (UnityEngine.Object) null && (UnityEngine.Object) this.m_offClickCatcher.gameObject != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_darkenQuad != (UnityEngine.Object) null && (UnityEngine.Object) this.m_darkenQuad.gameObject != (UnityEngine.Object) null)
        {
          iTween.Stop(this.m_darkenQuad.gameObject);
          iTween.ValueTo(this.m_darkenQuad.gameObject, iTween.Hash((object) "from", (object) this.m_currentQuadFade, (object) "to", (object) 0.0f, (object) "time", (object) this.m_animateBlurOutTime, (object) "onupdate", (object) "DarkenQuadFade_Update", (object) "onupdatetarget", (object) this.gameObject));
        }
        else
          this.FadeEffectsOut(this.m_animateBlurOutTime);
        this.m_offClickCatcher.gameObject.SetActive(false);
      }
      if (!this.m_playAudioOnSlide)
        return;
      SoundManager.Get()?.LoadAndPlay(AssetReference.op_Implicit(this.m_SlideOffSFXAssetString), this.gameObject);
    }
    else
    {
      this.gameObject.transform.localPosition = this.m_trayHiddenBone.localPosition;
      if ((UnityEngine.Object) this.m_darkenQuad != (UnityEngine.Object) null && (UnityEngine.Object) this.m_darkenQuad.gameObject != (UnityEngine.Object) null)
      {
        iTween.Stop(this.m_darkenQuad.gameObject);
        this.m_currentQuadFade = 0.0f;
        Material material = this.m_darkenQuad.GetMaterial();
        if ((UnityEngine.Object) material != (UnityEngine.Object) null)
          material.color = this.m_quadHiddenColor;
      }
      this.OnTraySliderAnimFinished();
    }
  }

  private bool BackPressed()
  {
    this.ToggleTraySlider(false);
    return true;
  }

  private void OnTraySliderAnimFinished()
  {
    this.m_traySliderAnimating = false;
    if (!this.m_trayShown)
    {
      if (this.m_inactivateOnHide)
        this.gameObject.SetActive(false);
      if (this.m_invisibleOnHide)
        this.transform.localPosition = this.INVISIBLE_POSITION;
      if ((UnityEngine.Object) this.m_darkenQuad != (UnityEngine.Object) null)
        this.m_darkenQuad.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_offClickCatcher != (UnityEngine.Object) null)
        this.m_offClickCatcher.gameObject.SetActive(false);
    }
    if (this.OnTransitionComplete == null)
      return;
    this.OnTransitionComplete();
  }

  private void OnTraySliderPressed(UIEvent e)
  {
    if (this.m_useNavigationBack && this.m_trayShown)
      return;
    this.ToggleTraySlider(!this.m_trayShown);
  }

  private void OnClickCatcherPressed(UIEvent e) => this.ToggleTraySlider(false);

  private void FadeEffectsIn(float time)
  {
    LayerUtils.SetLayer(this.gameObject, this.m_shownLayer);
    if (this.m_shownLayer == GameLayer.IgnoreFullScreenEffects)
      LayerUtils.SetLayer(Box.Get().m_letterboxingContainer, this.m_shownLayer);
    SceneMgr sceneMgr = ServiceManager.Get<SceneMgr>();
    FullScreenFXMgr fullScreenFXMgr = ServiceManager.Get<FullScreenFXMgr>();
    if (sceneMgr != null && sceneMgr.IsTransitioning())
      this.StartCoroutine(SlidingTray.BlurScreenAfterTransition(sceneMgr, fullScreenFXMgr, time, this));
    else
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = time
      });
  }

  private static IEnumerator BlurScreenAfterTransition(
    SceneMgr sceneMgr,
    FullScreenFXMgr fullScreenFXMgr,
    float time,
    SlidingTray tray)
  {
    while (sceneMgr.IsTransitioning())
      yield return (object) null;
    yield return (object) new WaitForSeconds(1f);
    ScreenEffectParameters desaturatePerspective = ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = time
    };
    tray.m_screenEffectsHandle.StartEffect(desaturatePerspective);
  }

  private void FadeEffectsOut(float time) => this.m_screenEffectsHandle.StopEffect(time, new Action(this.OnFadeFinished));

  private void OnFadeFinished()
  {
    if ((UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(this.gameObject, this.m_shownLayer);
    if (this.m_hiddenLayer != GameLayer.Default)
      return;
    LayerUtils.SetLayer(Box.Get().m_letterboxingContainer, this.m_hiddenLayer);
  }

  private void DarkenQuadFade_Update(float fade)
  {
    this.m_currentQuadFade = fade;
    Color color = Color.Lerp(this.m_quadHiddenColor, this.m_quadShownColor, this.m_currentQuadFade);
    this.m_darkenQuad.GetMaterial().color = color;
  }

  public delegate void TrayToggledListener(bool shown);
}
