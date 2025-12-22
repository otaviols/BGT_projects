using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TransitionPopup : MonoBehaviour
{
  public UberText m_title;
  public MatchingQueueTab m_queueTab;
  public UIBButton m_cancelButton;
  public Vector3_MobileOverride m_startPosition = new Vector3_MobileOverride(new Vector3(-0.05f, 8.2f, -1.8f));
  public Float_MobileOverride m_endScale;
  public Float_MobileOverride m_scaleAfterPunch;
  protected bool m_shown;
  protected bool m_blockingLoadingScreen;
  protected Camera m_fullScreenEffectsCamera;
  protected List<TransitionPopup.MatchCanceledEvent> m_matchCanceledListeners = new List<TransitionPopup.MatchCanceledEvent>();
  protected AdventureDbId m_adventureId;
  protected FormatType m_formatType;
  protected GameType m_gameType;
  protected long? m_deckId;
  protected int m_scenarioId;
  protected bool m_showAnimationFinished;
  private float POPUP_TIME = 0.3f;
  private float START_SCALE_VAL = 0.1f;
  private Vector3 END_POSITION;
  private bool m_blurEnabled;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public event Action<TransitionPopup> OnHidden;

  public event Action OnPopupDestroyed;

  public void SetAdventureId(AdventureDbId adventureId) => this.m_adventureId = adventureId;

  public void SetFormatType(FormatType formatType) => this.m_formatType = formatType;

  public void SetGameType(GameType gameType) => this.m_gameType = gameType;

  public void SetDeckId(long? deckId) => this.m_deckId = deckId;

  public void SetScenarioId(int scenarioId) => this.m_scenarioId = scenarioId;

  protected virtual void Awake()
  {
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    this.m_fullScreenEffectsCamera = Camera.main;
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonReleased));
    this.m_cancelButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnCancelButtonOver));
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.gameObject.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_startPosition;
  }

  protected virtual void Start()
  {
    if ((UnityEngine.Object) this.m_fullScreenEffectsCamera == (UnityEngine.Object) null)
      this.m_fullScreenEffectsCamera = Camera.main;
    if (this.m_shown)
      return;
    iTween.FadeTo(this.gameObject, 0.0f, 0.0f);
    this.gameObject.SetActive(false);
  }

  protected virtual void OnDestroy()
  {
    if (FullScreenFXMgr.Get() != null)
      this.DisableFullScreenBlur();
    this.StopBlockingTransition();
    if (GameMgr.Get() != null)
      GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    if (this.m_shown && this.OnHidden != null)
      this.OnHidden(this);
    if (this.OnPopupDestroyed == null)
      return;
    this.OnPopupDestroyed();
  }

  public bool IsShown() => this.m_shown;

  public virtual void Show()
  {
    if (this.m_shown)
      return;
    this.AnimateShow();
  }

  public virtual void Hide()
  {
    if (!this.m_shown)
      return;
    this.AnimateHide();
  }

  public void Cancel()
  {
    if (!this.m_shown || (UnityEngine.Object) this.m_fullScreenEffectsCamera == (UnityEngine.Object) null)
      return;
    this.DisableFullScreenBlur();
  }

  public void RegisterMatchCanceledEvent(TransitionPopup.MatchCanceledEvent callback) => this.m_matchCanceledListeners.Add(callback);

  public bool UnregisterMatchCanceledEvent(TransitionPopup.MatchCanceledEvent callback) => this.m_matchCanceledListeners.Remove(callback);

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if (!this.m_shown)
      return false;
    switch (eventData.m_state)
    {
      case FindGameState.BNET_QUEUE_ENTERED:
        this.OnGameEntered(eventData);
        break;
      case FindGameState.BNET_QUEUE_DELAYED:
        this.OnGameDelayed(eventData);
        break;
      case FindGameState.BNET_QUEUE_UPDATED:
        this.OnGameUpdated(eventData);
        break;
      case FindGameState.SERVER_GAME_CONNECTING:
        this.OnGameConnecting(eventData);
        break;
      case FindGameState.SERVER_GAME_STARTED:
        this.OnGameStarted(eventData);
        break;
    }
    return false;
  }

  protected virtual void OnGameEntered(FindGameEventData eventData) => this.m_queueTab.UpdateDisplay(eventData.m_queueMinSeconds, eventData.m_queueMaxSeconds);

  protected virtual void OnGameDelayed(FindGameEventData eventData)
  {
  }

  protected virtual void OnGameUpdated(FindGameEventData eventData) => this.m_queueTab.UpdateDisplay(eventData.m_queueMinSeconds, eventData.m_queueMaxSeconds);

  protected virtual void OnGameConnecting(FindGameEventData eventData) => this.DisableCancelButton();

  protected virtual void OnGameStarted(FindGameEventData eventData)
  {
    this.StartBlockingTransition();
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
  }

  protected virtual bool EnableCancelButtonIfPossible()
  {
    if (!this.m_showAnimationFinished || GameMgr.Get().IsAboutToStopFindingGame() || this.m_cancelButton.IsEnabled())
      return false;
    this.EnableCancelButton();
    return true;
  }

  protected virtual void EnableCancelButton()
  {
    this.m_cancelButton.Flip(true);
    this.m_cancelButton.SetEnabled(true);
  }

  protected virtual void DisableCancelButton()
  {
    this.m_cancelButton.Flip(false);
    this.m_cancelButton.SetEnabled(false);
  }

  protected virtual void OnCancelButtonReleased(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Back_Click.prefab:f7df4bfeab7ccff4198e670ca516da2e");
    this.DisableCancelButton();
  }

  protected virtual void OnCancelButtonOver(UIEvent e) => SoundManager.Get().LoadAndPlay((AssetReference) "Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9");

  protected void FireMatchCanceledEvent()
  {
    TransitionPopup.MatchCanceledEvent[] array = this.m_matchCanceledListeners.ToArray();
    if (array.Length == 0)
      Debug.LogError((object) "TransitionPopup.FireMatchCanceledEvent() - Cancel triggered, but nobody was listening!!");
    foreach (TransitionPopup.MatchCanceledEvent matchCanceledEvent in array)
      matchCanceledEvent();
  }

  protected virtual void AnimateShow()
  {
    iTween.Stop(this.gameObject);
    this.m_shown = true;
    this.m_showAnimationFinished = false;
    this.gameObject.SetActive(true);
    RenderUtils.EnableRenderers(this.gameObject, false);
    this.DisableCancelButton();
    this.ShowPopup();
    this.AnimateBlurBlendOn();
  }

  protected virtual void ShowPopup()
  {
    RenderUtils.EnableRenderers(this.gameObject, true);
    iTween.FadeTo(this.gameObject, 1f, this.POPUP_TIME);
    this.gameObject.transform.localScale = new Vector3(this.START_SCALE_VAL, this.START_SCALE_VAL, this.START_SCALE_VAL);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3((float) (MobileOverrideValue<float>) this.m_endScale, (float) (MobileOverrideValue<float>) this.m_endScale, (float) (MobileOverrideValue<float>) this.m_endScale), (object) "time", (object) this.POPUP_TIME, (object) "oncomplete", (object) "PunchPopup", (object) "oncompletetarget", (object) this.gameObject));
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.gameObject.transform.localPosition + new Vector3(0.02f, 0.02f, 0.02f)), (object) "time", (object) 1.5f, (object) "islocal", (object) true));
    this.m_queueTab.ResetTimer();
  }

  private void PunchPopup()
  {
    iTween.ScaleTo(this.gameObject, new Vector3((float) (MobileOverrideValue<float>) this.m_scaleAfterPunch, (float) (MobileOverrideValue<float>) this.m_scaleAfterPunch, (float) (MobileOverrideValue<float>) this.m_scaleAfterPunch), 0.15f);
    this.OnAnimateShowFinished();
  }

  protected virtual void OnAnimateShowFinished() => this.m_showAnimationFinished = true;

  protected virtual void AnimateHide()
  {
    this.m_shown = false;
    this.DisableCancelButton();
    iTween.FadeTo(this.gameObject, 0.0f, this.POPUP_TIME);
    Hashtable args = iTween.Hash((object) "scale", (object) new Vector3(this.START_SCALE_VAL, this.START_SCALE_VAL, this.START_SCALE_VAL), (object) "time", (object) this.POPUP_TIME);
    if (this.OnHidden != null)
      args[(object) "oncomplete"] = (object) (Action<object>) (data => this.OnHidden(this));
    iTween.ScaleTo(this.gameObject, args);
    this.AnimateBlurBlendOff();
  }

  private void AnimateBlurBlendOn() => this.EnableFullScreenBlur();

  protected void AnimateBlurBlendOff()
  {
    this.DisableFullScreenBlur();
    this.StartCoroutine(this.DelayDeactivatePopup(0.5f));
  }

  private IEnumerator DelayDeactivatePopup(float waitTime)
  {
    yield return (object) new WaitForSeconds(waitTime);
    if (!this.m_shown)
      this.DeactivatePopup();
  }

  protected void DeactivatePopup()
  {
    this.gameObject.SetActive(false);
    this.StopBlockingTransition();
  }

  protected void StartBlockingTransition()
  {
    this.m_blockingLoadingScreen = true;
    LoadingScreen.Get().AddTransitionBlocker();
    LoadingScreen.Get().AddTransitionObject(this.gameObject);
  }

  protected void StopBlockingTransition()
  {
    if (!this.m_blockingLoadingScreen)
      return;
    this.m_blockingLoadingScreen = false;
    if (!(bool) (UnityEngine.Object) LoadingScreen.Get())
      return;
    LoadingScreen.Get().NotifyTransitionBlockerComplete();
  }

  protected virtual void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (!this.m_shown)
      return;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    this.OnGameplaySceneLoaded();
  }

  private void EnableFullScreenBlur()
  {
    if (this.m_blurEnabled)
      return;
    this.m_blurEnabled = true;
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurDesaturatePerspective);
  }

  private void DisableFullScreenBlur()
  {
    if (!this.m_blurEnabled)
      return;
    this.m_blurEnabled = false;
    this.m_screenEffectsHandle.StopEffect();
  }

  protected abstract void OnGameplaySceneLoaded();

  public delegate void MatchCanceledEvent();
}
