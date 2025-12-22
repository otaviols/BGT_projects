using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

public class TemporaryAccountSignUpPopUp : UIBPopup
{
  public UIBButton m_backButton;
  public UIBButton m_signUpButton;
  public UberText m_headlineText;
  public UberText m_messageText;
  public UberText m_backButtonText;
  public UberText m_signUpButtonText;
  public GameObject m_inputBlockerRenderer;
  private static readonly Vector3 SHOW_POS_PHONE = new Vector3(0.0f, 15f, -2f);
  private static readonly Vector3 SHOW_SCALE_PHONE = new Vector3(85f, 85f, 85f);
  private static readonly TemporaryAccountSignUpPopUp.PopupTextParameters DEFAULT_STRINGS = new TemporaryAccountSignUpPopUp.PopupTextParameters()
  {
    Header = "GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_01",
    Body = "GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_01",
    CancelButton = "GLOBAL_BACK",
    SignUpButton = "GLUE_TEMPORARY_ACCOUNT_SIGN_UP"
  };
  private PegUIElement m_inputBlockerPegUIElement;
  private TemporaryAccountSignUpPopUp.OnSignUpPopUpBack m_signUpPopUpBackHandler;

  protected override void Awake()
  {
    base.Awake();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_showPosition = TemporaryAccountSignUpPopUp.SHOW_POS_PHONE;
      this.m_showScale = TemporaryAccountSignUpPopUp.SHOW_SCALE_PHONE;
    }
    this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackPressed));
    this.m_signUpButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSignUpPressed));
  }

  private void OnDestroy()
  {
    this.m_signUpPopUpBackHandler = (TemporaryAccountSignUpPopUp.OnSignUpPopUpBack) null;
    this.Hide(false);
  }

  public void Show(
    TemporaryAccountSignUpPopUp.PopupTextParameters textArgs,
    TemporaryAccountSignUpPopUp.OnSignUpPopUpBack onSignUpPopUpBack)
  {
    this.SetTextStrings(textArgs);
    this.m_signUpPopUpBackHandler = onSignUpPopUpBack;
    this.Show();
  }

  private void SetTextStrings(
    TemporaryAccountSignUpPopUp.PopupTextParameters textArgs)
  {
    UberText headlineText = this.m_headlineText;
    TemporaryAccountSignUpPopUp.PopupTextParameters defaultStrings;
    string header;
    if (string.IsNullOrEmpty(textArgs.Header))
    {
      defaultStrings = TemporaryAccountSignUpPopUp.DEFAULT_STRINGS;
      header = defaultStrings.Header;
    }
    else
      header = textArgs.Header;
    headlineText.Text = header;
    UberText messageText = this.m_messageText;
    string body;
    if (string.IsNullOrEmpty(textArgs.Body))
    {
      defaultStrings = TemporaryAccountSignUpPopUp.DEFAULT_STRINGS;
      body = defaultStrings.Body;
    }
    else
      body = textArgs.Body;
    messageText.Text = body;
    UberText backButtonText = this.m_backButtonText;
    string cancelButton;
    if (string.IsNullOrEmpty(textArgs.CancelButton))
    {
      defaultStrings = TemporaryAccountSignUpPopUp.DEFAULT_STRINGS;
      cancelButton = defaultStrings.CancelButton;
    }
    else
      cancelButton = textArgs.CancelButton;
    backButtonText.Text = cancelButton;
    UberText signUpButtonText = this.m_signUpButtonText;
    string signUpButton;
    if (string.IsNullOrEmpty(textArgs.SignUpButton))
    {
      defaultStrings = TemporaryAccountSignUpPopUp.DEFAULT_STRINGS;
      signUpButton = defaultStrings.SignUpButton;
    }
    else
      signUpButton = textArgs.SignUpButton;
    signUpButtonText.Text = signUpButton;
  }

  public override void Show()
  {
    if (this.IsShown())
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.Show(true);
    this.gameObject.SetActive(true);
    if ((UnityEngine.Object) this.m_inputBlockerPegUIElement != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "TemporaryAccountSignUpPopUpInputBlocker");
    LayerUtils.SetLayer(inputBlocker, this.gameObject.layer);
    this.m_inputBlockerPegUIElement = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlockerPegUIElement.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerRelease));
    TransformUtil.SetPosY((Component) this.m_inputBlockerPegUIElement, this.gameObject.transform.position.y - 5f);
    this.DarkenInputBlocker(inputBlocker, 0.5f);
  }

  protected override void Hide(bool animate)
  {
    if (!this.IsShown())
      return;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    if ((UnityEngine.Object) this.m_inputBlockerPegUIElement != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    this.gameObject.SetActive(false);
    base.Hide(animate);
  }

  private void OnBackPressed(UIEvent e)
  {
    if (this.m_signUpPopUpBackHandler != null)
    {
      this.m_signUpPopUpBackHandler();
      this.m_signUpPopUpBackHandler = (TemporaryAccountSignUpPopUp.OnSignUpPopUpBack) null;
    }
    this.Hide(true);
  }

  private void OnSignUpPressed(UIEvent e)
  {
    this.Hide(false);
    TemporaryAccountManager.Get().ShowHealUpPage((Action<bool>) (isResetting =>
    {
      if (isResetting)
        return;
      TemporaryAccountSignUpPopUp.OnSignUpPopUpBack popUpBackHandler = this.m_signUpPopUpBackHandler;
      if (popUpBackHandler == null)
        return;
      popUpBackHandler();
    }));
  }

  private bool OnNavigateBack()
  {
    if (this.m_signUpPopUpBackHandler != null)
    {
      this.m_signUpPopUpBackHandler();
      this.m_signUpPopUpBackHandler = (TemporaryAccountSignUpPopUp.OnSignUpPopUpBack) null;
    }
    this.Hide(true);
    return true;
  }

  private void OnInputBlockerRelease(UIEvent e)
  {
    if (this.m_signUpPopUpBackHandler != null)
    {
      this.m_signUpPopUpBackHandler();
      this.m_signUpPopUpBackHandler = (TemporaryAccountSignUpPopUp.OnSignUpPopUpBack) null;
    }
    this.Hide(true);
  }

  private void DarkenInputBlocker(GameObject inputBlockerObject, float alpha)
  {
    inputBlockerObject.AddComponent<MeshRenderer>().SetMaterial(this.m_inputBlockerRenderer.GetComponent<MeshRenderer>().GetMaterial());
    inputBlockerObject.AddComponent<MeshFilter>().SetMesh(this.m_inputBlockerRenderer.GetComponent<MeshFilter>().GetMesh());
    BoxCollider component = inputBlockerObject.GetComponent<BoxCollider>();
    TransformUtil.SetLocalScaleXY(inputBlockerObject, component.size.x, component.size.y);
    component.size = new Vector3(1f, 1f, 0.0f);
    TransformUtil.SetLocalEulerAngleX(inputBlockerObject, 90f);
    RenderUtils.SetAlpha(inputBlockerObject, alpha);
  }

  public delegate void OnSignUpPopUpBack();

  public struct PopupTextParameters
  {
    public string Header { get; set; }

    public string Body { get; set; }

    public string SignUpButton { get; set; }

    public string CancelButton { get; set; }
  }
}
