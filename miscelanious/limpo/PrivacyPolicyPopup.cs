using Blizzard.T5.Core.Utils;
using Hearthstone;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyPopup : DialogBase
{
  private const string PrivacyPolicyUrl = "https://oribos.blizzard.cn/legal/privacy";
  private const string EulaUrl = "https://oribos.blizzard.cn/legal/eula";
  public PegUIElement m_confirmButton;
  public PegUIElement m_rejectButton;
  public PegUIElement m_privacyPolicyButton;
  public PegUIElement m_eulaButton;
  private Vector3 m_buttonOffset = new Vector3(0.2f, 0.0f, 0.6f);
  private bool m_confirmedPrivacyPolicy;
  private Camera referenceCamera;
  private PrivacyPolicyPopup.ResponseCallback m_responseCallback;

  protected override void Awake()
  {
    base.Awake();
    this.referenceCamera = CameraUtils.FindFirstByLayer(GameLayer.UI);
    this.transform.position = this.referenceCamera.transform.TransformPoint(0.0f, 0.0f, 200f);
  }

  private void Start()
  {
    this.GetComponent<WidgetTemplate>().InitializeWidgetBehaviors();
    List<Component> components = new List<Component>();
    GameObjectUtils.WalkSelfAndChildren(this.transform, (Func<Transform, bool>) (current =>
    {
      bool flag = true;
      current.GetComponents<Component>(components);
      foreach (Component component in components)
      {
        if (component is Maskable maskable2)
        {
          maskable2.OverrideRenderPassEntryPoint(CustomViewEntryPoint.BattleNetChat);
          flag = false;
          break;
        }
      }
      if (flag)
        current.gameObject.layer = 18;
      components.Clear();
      return flag;
    }));
    this.m_confirmButton.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.ConfirmButtonReleaseAll));
    this.m_rejectButton.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.RejectButtonReleaseAll));
    this.m_privacyPolicyButton.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.PrivacyPolicyButtonReleaseAll));
    this.m_eulaButton.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.EULAButtonReleaseAll));
    this.m_privacyPolicyButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.PrivacyPolicyButtonPress));
    this.m_eulaButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.EULAButtonPress));
  }

  public override void Show()
  {
    base.Show();
    this.m_showAnimState = DialogBase.ShowAnimState.IN_PROGRESS;
    UniversalInputManager.Get().SetSystemDialogActive(true);
  }

  public void SetInfo(PrivacyPolicyPopup.Info info) => this.m_responseCallback = info.m_callback;

  protected void DownScale() => iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "delay", (object) 0.1, (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "oncomplete", (object) "OnHideAnimFinished", (object) "time", (object) 0.2f));

  protected override void OnHideAnimFinished()
  {
    base.OnHideAnimFinished();
    this.m_shown = false;
    this.OnPrivacyPolicyPopupResponse(this.m_confirmedPrivacyPolicy);
  }

  private void ConfirmButtonReleaseAll(UIEvent e)
  {
    if (!((UIReleaseAllEvent) e).GetMouseIsOver())
      return;
    this.m_confirmedPrivacyPolicy = true;
    this.ScaleAway();
  }

  private void RejectButtonReleaseAll(UIEvent e)
  {
    if (!((UIReleaseAllEvent) e).GetMouseIsOver())
      return;
    this.m_confirmedPrivacyPolicy = false;
    this.ScaleAway();
  }

  private void PrivacyPolicyButtonReleaseAll(UIEvent e)
  {
    this.m_privacyPolicyButton.transform.localPosition -= this.m_buttonOffset;
    if (!((UIReleaseAllEvent) e).GetMouseIsOver())
      return;
    Application.OpenURL("https://oribos.blizzard.cn/legal/privacy");
  }

  private void EULAButtonReleaseAll(UIEvent e)
  {
    this.m_eulaButton.transform.localPosition -= this.m_buttonOffset;
    if (!((UIReleaseAllEvent) e).GetMouseIsOver())
      return;
    Application.OpenURL("https://oribos.blizzard.cn/legal/eula");
  }

  private void PrivacyPolicyButtonPress(UIEvent e) => this.m_privacyPolicyButton.transform.localPosition += this.m_buttonOffset;

  private void EULAButtonPress(UIEvent e) => this.m_eulaButton.transform.localPosition += this.m_buttonOffset;

  private void ScaleAway() => iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.Scale(this.PUNCH_SCALE, this.gameObject.transform.localScale), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "oncomplete", (object) "DownScale", (object) "time", (object) 0.1f));

  private void OnPrivacyPolicyPopupResponse(bool confirmedPrivacyPolicy)
  {
    if (confirmedPrivacyPolicy)
    {
      Options.Get().SetBool(Option.HAS_ACCEPTED_PRIVACY_POLICY_AND_EULA, true);
      HearthstoneApplication.Get().DataTransferDependency.Callback();
    }
    else
    {
      GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/EmbeddedAlertPopup")) as GameObject;
      if ((UnityEngine.Object) this.referenceCamera != (UnityEngine.Object) null)
        gameObject.transform.position = this.referenceCamera.transform.TransformPoint(0.0f, 0.0f, 200f);
      gameObject.GetComponent<AlertPopup>().UpdateInfo(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_PRIVACY_POLICY_EULA_TITLE"),
        m_text = GameStrings.Get("GLUE_PRIVACY_POLICY_EULA_CONFIRMATION_TEXT"),
        m_confirmText = GameStrings.Get("GLUE_PRIVACY_POLICY_EULA_CONFIRMATION_ACCEPT"),
        m_cancelText = GameStrings.Get("GLUE_PRIVACY_POLICY_EULA_CONFIRMATION_REJECT"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response == AlertPopup.Response.CONFIRM)
          {
            Options.Get().SetBool(Option.HAS_ACCEPTED_PRIVACY_POLICY_AND_EULA, true);
            HearthstoneApplication.Get().DataTransferDependency.Callback();
          }
          else
            HearthstoneApplication.Get().Exit();
        })
      });
    }
  }

  public delegate void ResponseCallback(bool confirmedPrivacyPolicy);

  public class Info
  {
    public PrivacyPolicyPopup.ResponseCallback m_callback;
  }
}
