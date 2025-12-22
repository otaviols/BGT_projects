using Hearthstone.UI;
using System;
using UnityEngine;

public abstract class AbsSceneDisplay : MonoBehaviour
{
  public GameObject m_clickBlocker;
  public SlidingTray m_slidingTray;
  public AsyncReference m_sceneDisplayWidgetReference;
  private Action m_onSceneTransitionCompleteCallback;
  protected object m_sceneTransitionPayload;
  protected Widget m_sceneDisplayWidget;
  private bool m_sceneDisplayWidgetDoneChangingStates;

  protected abstract bool ShouldStartShown();

  public abstract bool IsFinishedLoading(out string failureMessage);

  public virtual void Start()
  {
    this.SetClickBlockerActive(false);
    if ((UnityEngine.Object) this.m_slidingTray != (UnityEngine.Object) null)
    {
      this.m_slidingTray.OnTransitionComplete += new Action(this.OnSlidingTrayAnimationComplete);
      this.InitializeSlidingTray();
    }
    if (this.m_sceneDisplayWidgetReference != null)
      this.m_sceneDisplayWidgetReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnSceneDisplayWidgetReady));
    else
      this.m_sceneDisplayWidgetDoneChangingStates = true;
  }

  public void ShowSlidingTrayAfterSceneLoad(Action onCompleteCallback)
  {
    this.SetClickBlockerActive(true);
    this.m_onSceneTransitionCompleteCallback = onCompleteCallback;
    if ((UnityEngine.Object) this.m_slidingTray != (UnityEngine.Object) null)
      this.m_slidingTray.ShowTray();
    else
      this.OnSlidingTrayAnimationComplete();
  }

  public void SetSceneTransitionPayload(object payload) => this.m_sceneTransitionPayload = payload;

  public void SetClickBlockerActive(bool active)
  {
    if (!((UnityEngine.Object) this.m_clickBlocker != (UnityEngine.Object) null))
      return;
    this.m_clickBlocker.SetActive(active);
  }

  public bool IsRootWidgetDoneChangingStates() => this.m_sceneDisplayWidgetDoneChangingStates;

  public void SetNextModeAndHandleTransition(SceneMgr.Mode nextMode, object sceneTransitionPayload = null) => this.SetNextModeAndHandleTransition(nextMode, SceneMgr.TransitionHandlerType.CURRENT_SCENE, sceneTransitionPayload);

  public void SetNextModeAndHandleTransition(
    SceneMgr.Mode nextMode,
    SceneMgr.TransitionHandlerType type,
    object sceneTransitionPayload = null)
  {
    this.SetClickBlockerActive(true);
    SceneMgr.Get().SetNextMode(nextMode, type, new SceneMgr.OnSceneLoadCompleteForSceneDrivenTransition(this.OnSceneLoadCompleteHandleTransition), sceneTransitionPayload);
  }

  public virtual bool IsBlockingPopupDisplayManager() => false;

  protected void InitializeSlidingTray()
  {
    if ((UnityEngine.Object) this.m_slidingTray == (UnityEngine.Object) null)
      return;
    this.m_slidingTray.ToggleTraySlider(this.ShouldStartShown(), animate: false);
  }

  protected void OnSceneLoadCompleteHandleTransition(Action onTransitionComplete)
  {
    this.m_onSceneTransitionCompleteCallback = onTransitionComplete;
    if ((UnityEngine.Object) this.m_slidingTray != (UnityEngine.Object) null)
      this.m_slidingTray.HideTray();
    else
      this.OnSlidingTrayAnimationComplete();
  }

  protected void OnSlidingTrayAnimationComplete()
  {
    this.SetClickBlockerActive(false);
    if (this.m_onSceneTransitionCompleteCallback == null)
      return;
    this.m_onSceneTransitionCompleteCallback();
    this.m_onSceneTransitionCompleteCallback = (Action) null;
  }

  private void OnSceneDisplayWidgetReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
    {
      this.m_sceneDisplayWidgetDoneChangingStates = true;
    }
    else
    {
      this.m_sceneDisplayWidget = (Widget) visualController.Owner;
      this.m_sceneDisplayWidget.RegisterDoneChangingStatesListener((Action<object>) (_ => this.m_sceneDisplayWidgetDoneChangingStates = true), (object) null, true, false);
    }
  }
}
