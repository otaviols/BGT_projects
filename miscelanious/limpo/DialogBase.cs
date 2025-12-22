using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBase : MonoBehaviour
{
  protected readonly Vector3 START_SCALE = 0.01f * Vector3.one;
  protected Vector3 PUNCH_SCALE = 1.2f * Vector3.one;
  protected DialogBase.ShowAnimState m_showAnimState;
  protected bool m_shown;
  protected Vector3 m_originalPosition;
  protected Vector3 m_originalScale;
  protected DialogBase.ReadyToDestroyCallback m_readyToDestroyCallback;
  private List<DialogBase.HideListener> m_hideListeners = new List<DialogBase.HideListener>();
  private List<DialogBase.HideListener> m_hiddenOrDestroyedListeners = new List<DialogBase.HideListener>();
  private bool m_hiddenOrDestroyedListenersFired;
  protected static ScreenEffectsHandle m_screenEffectsHandle;

  protected virtual CanvasScaleMode ScaleMode() => CanvasScaleMode.HEIGHT;

  protected virtual void Awake()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.PUNCH_SCALE = 1.08f * Vector3.one;
    if ((Object) OverlayUI.Get() != (Object) null)
      OverlayUI.Get().AddGameObject(this.gameObject, scaleMode: this.ScaleMode());
    this.m_originalPosition = this.transform.position;
    this.m_originalScale = this.transform.localScale;
    this.SetHiddenPosition();
    DialogBase.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected virtual void OnDestroy()
  {
    if (this.m_hiddenOrDestroyedListenersFired)
      return;
    this.FireHiddenOrDestroyedListeners();
  }

  public virtual bool HandleKeyboardInput() => false;

  public virtual void GoBack()
  {
  }

  public virtual void Show()
  {
    this.m_shown = true;
    this.SetShownPosition();
  }

  public virtual void Hide()
  {
    this.m_shown = false;
    this.StartCoroutine(this.HideWhenAble());
  }

  public virtual bool IsShown() => this.m_shown;

  public void AddHideListener(DialogBase.HideCallback callback) => this.AddHideListener(callback, (object) null);

  public void AddHideListener(DialogBase.HideCallback callback, object userData)
  {
    DialogBase.HideListener hideListener = new DialogBase.HideListener();
    hideListener.SetCallback(callback);
    hideListener.SetUserData(userData);
    if (this.m_hideListeners.Contains(hideListener))
      return;
    this.m_hideListeners.Add(hideListener);
  }

  public void AddHiddenOrDestroyedListener(DialogBase.HideCallback callback) => this.AddHiddenOrDestroyedListener(callback, (object) null);

  public void AddHiddenOrDestroyedListener(DialogBase.HideCallback callback, object userData)
  {
    DialogBase.HideListener hideListener = new DialogBase.HideListener();
    hideListener.SetCallback(callback);
    hideListener.SetUserData(userData);
    if (this.m_hiddenOrDestroyedListeners.Contains(hideListener))
      return;
    this.m_hiddenOrDestroyedListeners.Add(hideListener);
  }

  public void SetReadyToDestroyCallback(DialogBase.ReadyToDestroyCallback callback) => this.m_readyToDestroyCallback = callback;

  protected void SetShownPosition() => this.transform.position = this.m_originalPosition;

  protected void SetHiddenPosition(Camera referenceCamera = null)
  {
    if ((Object) referenceCamera == (Object) null)
      referenceCamera = PegUI.Get().orthographicUICam;
    this.transform.position = referenceCamera.transform.TransformPoint(0.0f, 0.0f, -1000f);
  }

  protected virtual void DoShowAnimation()
  {
    this.m_showAnimState = DialogBase.ShowAnimState.IN_PROGRESS;
    AnimationUtil.ShowWithPunch(this.gameObject, this.START_SCALE, Vector3.Scale(this.PUNCH_SCALE, this.m_originalScale), this.m_originalScale, "OnShowAnimFinished");
  }

  protected virtual void DoHideAnimation() => AnimationUtil.ScaleFade(this.gameObject, this.START_SCALE, "OnHideAnimFinished");

  protected virtual void OnHideAnimFinished()
  {
    this.SetHiddenPosition();
    UniversalInputManager.Get().SetSystemDialogActive(false);
    this.FireHideListeners();
    this.FireHiddenOrDestroyedListeners();
    if (this.m_readyToDestroyCallback == null)
      return;
    this.m_readyToDestroyCallback(this);
  }

  private void FireHideListeners()
  {
    foreach (DialogBase.HideListener hideListener in this.m_hideListeners)
      hideListener.Fire(this);
  }

  private void FireHiddenOrDestroyedListeners()
  {
    foreach (DialogBase.HideListener destroyedListener in this.m_hiddenOrDestroyedListeners)
      destroyedListener.Fire(this);
    this.m_hiddenOrDestroyedListenersFired = true;
  }

  protected virtual void OnShowAnimFinished() => this.m_showAnimState = DialogBase.ShowAnimState.FINISHED;

  private IEnumerator HideWhenAble()
  {
    while (this.m_showAnimState == DialogBase.ShowAnimState.IN_PROGRESS)
      yield return (object) null;
    this.DoHideAnimation();
  }

  public static void DoBlur() => DialogBase.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);

  public static void EndBlur() => DialogBase.m_screenEffectsHandle.StopEffect();

  public delegate void HideCallback(DialogBase dialog, object userData);

  public delegate void ReadyToDestroyCallback(DialogBase dialog);

  protected class HideListener : EventListener<DialogBase.HideCallback>
  {
    public void Fire(DialogBase dialog) => this.m_callback(dialog, this.m_userData);
  }

  protected enum ShowAnimState
  {
    NOT_CALLED,
    IN_PROGRESS,
    FINISHED,
  }
}
