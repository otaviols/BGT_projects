using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class UIBPopup : MonoBehaviour
{
  [CustomEditField(Sections = "Animation & Positioning")]
  public bool m_useStartingPositionForShow;
  [CustomEditField(Sections = "Animation & Positioning")]
  public Vector3 m_showPosition = Vector3.zero;
  [CustomEditField(Sections = "Animation & Positioning")]
  public bool m_useStartingScaleForShow;
  [CustomEditField(Sections = "Animation & Positioning")]
  public Vector3 m_showScale = Vector3.one;
  [CustomEditField(Sections = "Animation & Positioning")]
  public float m_showAnimTime = 0.5f;
  [CustomEditField(Sections = "Animation & Positioning")]
  public Vector3 m_hidePosition = new Vector3(-1000f, 0.0f, 0.0f);
  [CustomEditField(Sections = "Animation & Positioning")]
  public float m_hideAnimTime = 0.1f;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_showAnimationSound = "Expand_Up.prefab:775d97ea42498c044897f396362b9db3";
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public bool m_playShowSoundWithNoAnimation;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_hideAnimationSound = "Shrink_Down.prefab:a6d5184049ac041418cd5896e7d9a87a";
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public bool m_playHideSoundWithNoAnimation;
  [CustomEditField(Sections = "Click Blockers")]
  public BoxCollider m_animationClickBlocker;
  private const string s_ShowiTweenAnimationName = "SHOW_ANIMATION";
  protected bool m_shown;
  protected CanvasScaleMode m_scaleMode = CanvasScaleMode.HEIGHT;
  protected bool m_destroyOnSceneLoad = true;
  protected bool m_useOverlayUI = true;

  protected virtual void Awake()
  {
    if (this.m_useStartingPositionForShow)
      this.m_showPosition = this.transform.localPosition;
    if (this.m_useStartingScaleForShow)
      this.m_showScale = this.transform.localScale;
    WidgetTemplate component = this.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      this.m_useOverlayUI = false;
      this.m_destroyOnSceneLoad = false;
    }
    WidgetTemplate widgetTemplate = component ?? this.GetComponentInParent<WidgetTemplate>();
    if (!((UnityEngine.Object) widgetTemplate != (UnityEngine.Object) null))
      return;
    widgetTemplate.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleWidgetEvent));
  }

  protected virtual void Start()
  {
  }

  public virtual bool IsShown() => this.m_shown;

  public virtual void Show() => this.Show(true);

  public virtual void Show(bool useOverlayUI)
  {
    if (this.m_shown)
      return;
    this.m_useOverlayUI = useOverlayUI;
    if (this.m_useOverlayUI)
    {
      OverlayUI overlayUi1 = OverlayUI.Get();
      if ((UnityEngine.Object) overlayUi1 != (UnityEngine.Object) null)
      {
        OverlayUI overlayUi2 = overlayUi1;
        GameObject gameObject = this.gameObject;
        CanvasScaleMode scaleMode1 = this.m_scaleMode;
        int num = this.m_destroyOnSceneLoad ? 1 : 0;
        int scaleMode2 = (int) scaleMode1;
        overlayUi2.AddGameObject(gameObject, destroyOnSceneLoad: (num != 0), scaleMode: ((CanvasScaleMode) scaleMode2));
      }
    }
    this.m_shown = true;
    this.DoShowAnimation();
  }

  public virtual void Hide() => this.Hide(false);

  protected virtual void Hide(bool animate)
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    this.DoHideAnimation(!animate, new UIBPopup.OnAnimationComplete(this.OnHidden));
  }

  protected virtual void OnHidden()
  {
  }

  protected void DoShowAnimation(UIBPopup.OnAnimationComplete animationDoneCallback = null) => this.DoShowAnimation(false, animationDoneCallback);

  protected void DoShowAnimation(
    bool disableAnimation,
    UIBPopup.OnAnimationComplete animationDoneCallback = null)
  {
    this.transform.localPosition = this.m_showPosition;
    if (this.m_useOverlayUI)
    {
      OverlayUI overlayUi1 = OverlayUI.Get();
      if ((UnityEngine.Object) overlayUi1 != (UnityEngine.Object) null)
      {
        OverlayUI overlayUi2 = overlayUi1;
        GameObject gameObject = this.gameObject;
        CanvasScaleMode scaleMode1 = this.m_scaleMode;
        int num = this.m_destroyOnSceneLoad ? 1 : 0;
        int scaleMode2 = (int) scaleMode1;
        overlayUi2.AddGameObject(gameObject, destroyOnSceneLoad: (num != 0), scaleMode: ((CanvasScaleMode) scaleMode2));
      }
    }
    this.EnableAnimationClickBlocker(true);
    if (!disableAnimation && (double) this.m_showAnimTime > 0.0)
    {
      this.transform.localScale = this.m_showScale * 0.01f;
      if (!string.IsNullOrEmpty(this.m_showAnimationSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_showAnimationSound);
      Hashtable args = iTween.Hash((object) "scale", (object) this.m_showScale, (object) "isLocal", (object) false, (object) "time", (object) this.m_showAnimTime, (object) "easetype", (object) iTween.EaseType.easeOutBounce, (object) "name", (object) "SHOW_ANIMATION");
      if (animationDoneCallback != null)
        args.Add((object) "oncomplete", (object) (Action<object>) (o =>
        {
          this.EnableAnimationClickBlocker(false);
          animationDoneCallback();
        }));
      iTween.StopByName(this.gameObject, "SHOW_ANIMATION");
      iTween.ScaleTo(this.gameObject, args);
    }
    else
    {
      if (this.m_playShowSoundWithNoAnimation && !string.IsNullOrEmpty(this.m_showAnimationSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_showAnimationSound);
      this.transform.localScale = this.m_showScale;
      if (animationDoneCallback != null)
      {
        this.EnableAnimationClickBlocker(false);
        animationDoneCallback();
      }
    }
    WidgetTemplate componentInParent = this.GetComponentInParent<WidgetTemplate>();
    if (!((UnityEngine.Object) componentInParent != (UnityEngine.Object) null))
      return;
    componentInParent.TriggerEvent("POPUP_SHOWN", new Widget.TriggerEventParameters());
  }

  protected void DoHideAnimation(UIBPopup.OnAnimationComplete animationDoneCallback = null) => this.DoHideAnimation(false, animationDoneCallback);

  protected void DoHideAnimation(
    bool disableAnimation,
    UIBPopup.OnAnimationComplete animationDoneCallback = null)
  {
    Action setHidePosition = (Action) (() =>
    {
      if (!((UnityEngine.Object) this.transform != (UnityEngine.Object) null))
        return;
      this.transform.localPosition = this.m_hidePosition;
      this.transform.localScale = this.m_showScale;
    });
    if (!disableAnimation && (double) this.m_hideAnimTime > 0.0)
    {
      if (!string.IsNullOrEmpty(this.m_hideAnimationSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_hideAnimationSound);
      Hashtable args = iTween.Hash((object) "scale", (object) (this.m_showScale * 0.01f), (object) "isLocal", (object) true, (object) "time", (object) this.m_hideAnimTime, (object) "easetype", (object) iTween.EaseType.linear, (object) "name", (object) "SHOW_ANIMATION");
      if (animationDoneCallback != null)
        args.Add((object) "oncomplete", (object) (Action<object>) (o =>
        {
          setHidePosition();
          animationDoneCallback();
        }));
      else
        args.Add((object) "oncomplete", (object) (Action<object>) (o => setHidePosition()));
      iTween.StopByName(this.gameObject, "SHOW_ANIMATION");
      iTween.ScaleTo(this.gameObject, args);
    }
    else
    {
      if (this.m_playHideSoundWithNoAnimation && !string.IsNullOrEmpty(this.m_hideAnimationSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_hideAnimationSound);
      setHidePosition();
      if (animationDoneCallback != null)
        animationDoneCallback();
    }
    WidgetTemplate componentInParent = this.GetComponentInParent<WidgetTemplate>();
    if (!((UnityEngine.Object) componentInParent != (UnityEngine.Object) null))
      return;
    componentInParent.TriggerEvent("POPUP_HIDDEN", new Widget.TriggerEventParameters());
  }

  private void EnableAnimationClickBlocker(bool enable)
  {
    if (!((UnityEngine.Object) this.m_animationClickBlocker != (UnityEngine.Object) null))
      return;
    this.m_animationClickBlocker.gameObject.SetActive(enable);
  }

  public void HandleWidgetEvent(string eventName)
  {
    if (!(eventName == "CODE_SHOW_POPUP"))
    {
      if (!(eventName == "CODE_HIDE_POPUP"))
        return;
      this.Hide();
    }
    else
      this.Show(false);
  }

  public delegate void OnAnimationComplete();
}
