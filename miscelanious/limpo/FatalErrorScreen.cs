using Hearthstone;
using Hearthstone.Core;
using System.Collections;
using UnityEngine;

public class FatalErrorScreen : MonoBehaviour
{
  public UberText m_closedSignText;
  public UberText m_closedSignTitle;
  public UberText m_reconnectTip;
  public UberText m_errorCodeText;
  private Camera m_camera;
  private PegUIElement m_inputBlocker;
  private bool m_allowClick;
  private bool m_redirectToStore;
  public float m_delayBeforeNextReset;
  private bool m_isUnrecoverable;

  private void Awake()
  {
    LogoAnimation logoAnimation = LogoAnimation.Get();
    if ((Object) logoAnimation != (Object) null)
      logoAnimation.HideLogo();
    this.m_closedSignTitle.Text = GameStrings.Get("GLOBAL_SPLASH_CLOSED_SIGN_TITLE");
    if (FatalErrorMgr.Get().HasError())
    {
      FatalErrorMessage[] messages = FatalErrorMgr.Get().GetMessages();
      this.m_closedSignText.Text = messages[0].m_text;
      this.m_allowClick = messages[0].m_allowClick;
      this.m_redirectToStore = messages[0].m_redirectToStore;
      this.m_delayBeforeNextReset = messages[0].m_delayBeforeNextReset;
    }
    else if (Application.isEditor)
      this.m_closedSignText.Text = "Please make it sure FatalError scene is NOT in your Hierarchy window.";
    this.m_isUnrecoverable = FatalErrorMgr.Get().IsUnrecoverable;
  }

  private void Start()
  {
    if ((bool) HearthstoneApplication.AllowResetFromFatalError)
    {
      if (this.m_isUnrecoverable)
      {
        this.m_allowClick = false;
        this.m_reconnectTip.gameObject.SetActive(true);
        this.m_reconnectTip.SetText(GameStrings.Get("GLOBAL_MOBILE_RESTART_APPLICATION"));
      }
      else if (this.m_allowClick)
      {
        this.m_reconnectTip.gameObject.SetActive(true);
        this.m_reconnectTip.SetText(GameStrings.Get(this.m_redirectToStore ? "GLOBAL_MOBILE_TAP_TO_UPDATE" : "GLOBAL_MOBILE_TAP_TO_RECONNECT"));
      }
    }
    this.StartCoroutine(this.WaitForUIThenFinishSetup());
  }

  private void Update()
  {
    if (!this.m_reconnectTip.gameObject.activeSelf)
      return;
    this.m_reconnectTip.TextAlpha = (float) (((double) Mathf.Sin((float) ((double) Time.time * 3.14159274101257 / 1.0)) + 1.0) / 2.0);
  }

  private void OnDestroy()
  {
    if (!((Object) PegUI.Get() != (Object) null))
      return;
    PegUI.Get().RemoveInputCamera(this.m_camera);
  }

  public void Show()
  {
    this.gameObject.SetActive(true);
    iTween.FadeTo(this.gameObject, iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutCubic));
  }

  private IEnumerator WaitForUIThenFinishSetup()
  {
    FatalErrorScreen parent = this;
    while ((Object) PegUI.Get() == (Object) null || (Object) OverlayUI.Get() == (Object) null)
      yield return (object) null;
    OverlayUI.Get().AddGameObject(parent.gameObject);
    parent.Show();
    parent.m_camera = CameraUtils.FindFirstByLayer(parent.gameObject.layer);
    PegUI.Get().AddInputCamera(parent.m_camera);
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(parent.m_camera, "ClosedSignInputBlocker", (Component) parent);
    LayerUtils.SetLayer(inputBlocker, parent.gameObject.layer);
    parent.m_inputBlocker = inputBlocker.AddComponent<PegUIElement>();
    if (parent.m_allowClick)
      parent.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(parent.OnClick));
    if (FatalErrorMgr.Get().GetFormattedErrorCode() != null)
    {
      parent.m_errorCodeText.gameObject.SetActive(true);
      parent.m_errorCodeText.Text = FatalErrorMgr.Get().GetFormattedErrorCode();
      OverlayUI.Get().AddGameObject(parent.m_errorCodeText.gameObject, CanvasAnchor.TOP_RIGHT);
    }
    if (parent.m_isUnrecoverable)
      Processor.TerminateAllProcessing();
  }

  private void OnClick(UIEvent e)
  {
    if ((bool) HearthstoneApplication.AllowResetFromFatalError)
    {
      if (this.m_redirectToStore)
      {
        UpdateUtils.OpenAppStore();
      }
      else
      {
        float waitDuration = HearthstoneApplication.Get().LastResetTime() + this.m_delayBeforeNextReset - Time.realtimeSinceStartup;
        if ((double) waitDuration > 0.0)
        {
          this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
          this.m_closedSignText.Text = GameStrings.Get("GLOBAL_SPLASH_CLOSED_RECONNECTING");
          this.m_allowClick = false;
          this.m_reconnectTip.gameObject.SetActive(false);
          this.StartCoroutine(this.WaitBeforeReconnecting(waitDuration));
        }
        else
        {
          Debug.Log((object) "resetting!");
          this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
          HearthstoneApplication.Get().Reset();
        }
      }
    }
    else
    {
      this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
      HearthstoneApplication.Get().Exit();
    }
  }

  private IEnumerator WaitBeforeReconnecting(float waitDuration)
  {
    yield return (object) new WaitForSeconds(waitDuration);
    HearthstoneApplication.Get().Reset();
  }
}
