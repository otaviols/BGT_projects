using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AlertPopup : DialogBase
{
  public AlertPopup.Header m_header;
  public NineSliceElement m_body;
  public GameObject m_alertIcon;
  public MultiSliceElement m_buttonContainer;
  public UIBButton m_okayButton;
  public UIBButton m_confirmButton;
  public UIBButton m_cancelButton;
  public GameObject m_clickCatcher;
  public UberText m_alertText;
  public Vector3 m_alertIconOffset;
  public float m_padding;
  public Vector3 m_loadPosition;
  public Vector3 m_showPosition;
  public List<GameObject> m_buttonIconsSet1 = new List<GameObject>();
  public List<GameObject> m_buttonIconsSet2 = new List<GameObject>();
  private const float BUTTON_FRAME_WIDTH = 80f;
  private AlertPopup.PopupInfo m_popupInfo;
  private AlertPopup.PopupInfo m_updateInfo;
  private float m_alertTextInitialWidth;
  private bool m_wasPressed;
  private AssetHandle<Texture> m_loadedIconTexture;

  protected override void Awake()
  {
    this.m_alertTextInitialWidth = this.m_alertText.Width;
    base.Awake();
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ButtonPress(AlertPopup.Response.OK)));
    this.m_confirmButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ButtonPress(AlertPopup.Response.CONFIRM)));
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ButtonPress(AlertPopup.Response.CANCEL)));
  }

  private void Start()
  {
    if (!string.IsNullOrEmpty(this.m_alertText.Text))
      return;
    this.m_alertText.Text = GameStrings.Get("GLOBAL_OKAY");
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (UniversalInputManager.Get() != null)
      UniversalInputManager.Get().SetSystemDialogActive(false);
    AssetHandle.SafeDispose<Texture>(ref this.m_loadedIconTexture);
  }

  public override bool HandleKeyboardInput()
  {
    if (!InputCollection.GetKeyUp(KeyCode.Escape) || this.m_popupInfo == null || !this.m_popupInfo.m_keyboardEscIsCancel || !this.m_cancelButton.enabled || !this.m_cancelButton.gameObject.activeSelf)
      return false;
    this.GoBack();
    return true;
  }

  public override void GoBack() => this.ButtonPress(AlertPopup.Response.CANCEL);

  public void SetInfo(AlertPopup.PopupInfo info) => this.m_popupInfo = info;

  public AlertPopup.PopupInfo GetInfo() => this.m_popupInfo;

  public override void Show()
  {
    base.Show();
    this.InitInfo();
    this.UpdateAll(this.m_popupInfo);
    this.transform.localPosition += this.m_popupInfo.m_offset;
    if (this.m_popupInfo.m_layerToUse.HasValue)
      LayerUtils.SetLayer((Component) this, this.m_popupInfo.m_layerToUse.Value);
    if (this.m_popupInfo.m_disableBlocker)
      this.m_clickCatcher.SetActive(false);
    if (this.m_popupInfo.m_disableBnetBar)
      BnetBar.Get().DisableButtonsByDialog((DialogBase) this);
    if (this.m_popupInfo.m_blurWhenShown)
      DialogBase.DoBlur();
    this.DoShowAnimation();
    bool active = this.m_popupInfo == null || !this.m_popupInfo.m_layerToUse.HasValue || this.m_popupInfo.m_layerToUse.Value == GameLayer.UI || this.m_popupInfo.m_layerToUse.Value == GameLayer.HighPriorityUI;
    UniversalInputManager.Get().SetSystemDialogActive(active);
  }

  public override void Hide()
  {
    base.Hide();
    if (!this.m_popupInfo.m_blurWhenShown)
      return;
    DialogBase.EndBlur();
  }

  public void UpdateInfo(AlertPopup.PopupInfo info)
  {
    this.m_updateInfo = info;
    this.UpdateButtons(this.m_updateInfo.m_responseDisplay);
    if (this.m_showAnimState == DialogBase.ShowAnimState.IN_PROGRESS)
      return;
    this.UpdateInfoAfterAnim();
  }

  public string BodyText
  {
    get => this.m_alertText.Text;
    set
    {
      this.m_alertText.Text = value;
      if (this.m_popupInfo == null)
        return;
      this.UpdateLayout();
    }
  }

  protected override void OnHideAnimFinished()
  {
    UniversalInputManager.Get().SetSystemDialogActive(false);
    base.OnHideAnimFinished();
  }

  protected override void OnShowAnimFinished()
  {
    base.OnShowAnimFinished();
    if (this.m_updateInfo == null)
      return;
    this.UpdateInfoAfterAnim();
  }

  private void InitInfo()
  {
    if (this.m_popupInfo == null)
      this.m_popupInfo = new AlertPopup.PopupInfo();
    if (this.m_popupInfo.m_headerText != null)
      return;
    this.m_popupInfo.m_headerText = GameStrings.Get("GLOBAL_DEFAULT_ALERT_HEADER");
  }

  private void UpdateButtons(AlertPopup.ResponseDisplay displayType)
  {
    this.m_confirmButton.gameObject.SetActive(false);
    this.m_cancelButton.gameObject.SetActive(false);
    this.m_okayButton.gameObject.SetActive(false);
    switch (displayType)
    {
      case AlertPopup.ResponseDisplay.OK:
        this.m_okayButton.gameObject.SetActive(true);
        break;
      case AlertPopup.ResponseDisplay.CONFIRM:
        this.m_confirmButton.gameObject.SetActive(true);
        break;
      case AlertPopup.ResponseDisplay.CANCEL:
        this.m_cancelButton.gameObject.SetActive(true);
        break;
      case AlertPopup.ResponseDisplay.CONFIRM_CANCEL:
        this.m_confirmButton.gameObject.SetActive(true);
        this.m_cancelButton.gameObject.SetActive(true);
        break;
    }
    this.m_buttonContainer.UpdateSlices();
  }

  private void UpdateTexts(AlertPopup.PopupInfo popupInfo)
  {
    this.m_alertText.RichText = this.m_popupInfo.m_richTextEnabled;
    this.m_alertText.Alignment = this.m_popupInfo.m_alertTextAlignment;
    this.m_alertText.Anchor = this.m_popupInfo.m_alertTextAlignmentAnchor;
    if (popupInfo.m_headerText == null)
      popupInfo.m_headerText = GameStrings.Get("GLOBAL_DEFAULT_ALERT_HEADER");
    this.m_alertText.Text = popupInfo.m_text;
    this.m_okayButton.SetText(popupInfo.m_okText == null ? GameStrings.Get("GLOBAL_OKAY") : popupInfo.m_okText);
    this.m_confirmButton.SetText(popupInfo.m_confirmText == null ? GameStrings.Get("GLOBAL_CONFIRM") : popupInfo.m_confirmText);
    this.m_cancelButton.SetText(popupInfo.m_cancelText == null ? GameStrings.Get("GLOBAL_CANCEL") : popupInfo.m_cancelText);
  }

  private void UpdateIcons(AlertPopup.PopupInfo popupInfo)
  {
    this.m_alertIcon.SetActive(popupInfo.m_showAlertIcon);
    if (AssetLoader.Get() != null && !string.IsNullOrEmpty((string) popupInfo.m_iconTexture))
    {
      MeshRenderer component = this.m_alertIcon.GetComponent<MeshRenderer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        AssetLoader.Get().LoadAsset<Texture>(ref this.m_loadedIconTexture, popupInfo.m_iconTexture);
        if (this.m_loadedIconTexture != null)
          RendererExtension.GetMaterial((Renderer) component).SetTexture("_MainTex", (Texture) this.m_loadedIconTexture);
      }
    }
    bool flag1 = popupInfo.m_iconSet == AlertPopup.PopupInfo.IconSet.Default;
    bool flag2 = popupInfo.m_iconSet == AlertPopup.PopupInfo.IconSet.Alternate;
    for (int index = 0; index < this.m_buttonIconsSet1.Count; ++index)
      this.m_buttonIconsSet1[index].SetActive(flag1);
    for (int index = 0; index < this.m_buttonIconsSet2.Count; ++index)
      this.m_buttonIconsSet2[index].SetActive(flag2);
  }

  private void UpdateInfoAfterAnim()
  {
    this.m_popupInfo = this.m_updateInfo;
    this.m_updateInfo = (AlertPopup.PopupInfo) null;
    this.UpdateAll(this.m_popupInfo);
  }

  private void UpdateAll(AlertPopup.PopupInfo popupInfo)
  {
    this.UpdateIcons(popupInfo);
    this.UpdateHeaderText(popupInfo.m_headerText);
    this.UpdateTexts(popupInfo);
    this.UpdateLayout();
  }

  private void UpdateLayout()
  {
    int num1 = this.m_alertIcon.activeSelf ? 1 : 0;
    Bounds textBounds = this.m_alertText.GetTextBounds();
    float x = textBounds.size.x;
    float a = textBounds.size.y + this.m_padding + this.m_popupInfo.m_padding;
    float num2 = 0.0f;
    float b = 0.0f;
    if (num1 != 0)
    {
      OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_alertIcon);
      num2 = orientedWorldBounds.Extents[0].magnitude * 2f;
      b = orientedWorldBounds.Extents[1].magnitude * 2f;
    }
    this.UpdateButtons(this.m_popupInfo.m_responseDisplay);
    this.m_body.SetSize(Mathf.Max(TransformUtil.GetBoundsOfChildren((Component) this.m_confirmButton).size.x * 2f, x) + num2, Mathf.Max(a, b));
    Vector3 offset = new Vector3(0.0f, 0.01f, 0.0f);
    TransformUtil.SetPoint(this.m_alertIcon, Anchor.TOP_LEFT_XZ, (GameObject) this.m_body.m_middle, Anchor.TOP_LEFT_XZ, offset);
    this.m_alertIcon.transform.localPosition += this.m_alertIconOffset;
    Anchor anchor = Anchor.TOP_LEFT_XZ;
    if (this.m_popupInfo.m_alertTextAlignment == UberText.AlignmentOptions.Center)
    {
      anchor = Anchor.TOP_XZ;
      if (this.m_popupInfo.m_showAlertIcon)
        anchor = Anchor.TOP_LEFT_XZ;
    }
    if (this.m_alertText.Anchor == UberText.AnchorOptions.Middle)
    {
      switch (anchor)
      {
        case Anchor.TOP_LEFT_XZ:
          anchor = Anchor.LEFT_XZ;
          break;
        case Anchor.TOP_XZ:
          anchor = Anchor.CENTER_XZ;
          break;
        case Anchor.TOP_RIGHT_XZ:
          anchor = Anchor.RIGHT_XZ;
          break;
      }
    }
    TransformUtil.SetPoint((Component) this.m_alertText, anchor, (GameObject) this.m_body.m_middle, anchor, offset);
    Vector3 position = this.m_alertText.transform.position;
    position.x += num2 + this.m_alertIconOffset.x;
    ++position.y;
    this.m_alertText.transform.position = position;
    if (this.m_popupInfo.m_alertTextAlignment == UberText.AlignmentOptions.Center)
      this.m_alertText.Width = this.m_alertTextInitialWidth - num2 * this.m_alertText.transform.localScale.x;
    this.m_header.m_container.transform.position = this.m_body.m_top.m_slice.transform.position;
    this.m_buttonContainer.transform.position = this.m_body.m_bottom.m_slice.transform.position;
    if (!this.m_popupInfo.m_scaleOverride.HasValue)
      return;
    this.m_originalScale = this.m_popupInfo.m_scaleOverride.Value;
  }

  private void ButtonPress(AlertPopup.Response response)
  {
    if (this.m_wasPressed)
      return;
    this.m_wasPressed = true;
    if (this.m_popupInfo.m_responseCallback != null)
      this.m_popupInfo.m_responseCallback(response, this.m_popupInfo.m_responseUserData);
    this.Hide();
  }

  private void UpdateHeaderText(string text)
  {
    bool flag = string.IsNullOrEmpty(text);
    this.m_header.m_container.gameObject.SetActive(!flag);
    if (flag)
      return;
    this.m_header.m_text.ResizeToFit = false;
    this.m_header.m_text.Text = text;
    this.m_header.m_text.UpdateNow();
    MeshRenderer component = this.m_body.m_middle.m_slice.GetComponent<MeshRenderer>();
    float x1 = this.m_header.m_text.GetTextBounds().size.x;
    float x2 = this.m_header.m_text.transform.worldToLocalMatrix.MultiplyVector(this.m_header.m_text.GetTextBounds().size).x;
    float num = 0.8f * this.m_header.m_text.transform.worldToLocalMatrix.MultiplyVector(component.GetComponent<Renderer>().bounds.size).x;
    if ((double) x2 > (double) num)
    {
      this.m_header.m_text.Width = num;
      this.m_header.m_text.ResizeToFit = true;
      this.m_header.m_text.UpdateNow();
      x1 = this.m_header.m_text.GetTextBounds().size.x;
    }
    else
      this.m_header.m_text.Width = x2;
    TransformUtil.SetLocalScaleToWorldDimension(this.m_header.m_middle, new WorldDimensionIndex(x1, 0));
    this.m_header.m_container.UpdateSlices();
  }

  [Serializable]
  public class Header
  {
    public MultiSliceElement m_container;
    public GameObject m_middle;
    public UberText m_text;
  }

  public enum Response
  {
    OK,
    CONFIRM,
    CANCEL,
  }

  public enum ResponseDisplay
  {
    NONE,
    OK,
    CONFIRM,
    CANCEL,
    CONFIRM_CANCEL,
  }

  public delegate void ResponseCallback(AlertPopup.Response response, object userData);

  public class PopupInfo
  {
    public UserAttentionBlocker m_attentionCategory = UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE;
    public string m_id;
    public string m_headerText;
    public string m_text;
    public string m_okText;
    public string m_confirmText;
    public string m_cancelText;
    public bool m_showAlertIcon = true;
    public AssetReference m_iconTexture;
    public AlertPopup.ResponseDisplay m_responseDisplay = AlertPopup.ResponseDisplay.OK;
    public AlertPopup.ResponseCallback m_responseCallback;
    public object m_responseUserData;
    public Vector3 m_offset = Vector3.zero;
    public float m_padding;
    public Vector3? m_scaleOverride;
    public bool m_richTextEnabled = true;
    public bool m_disableBlocker;
    public AlertPopup.PopupInfo.IconSet m_iconSet;
    public UberText.AlignmentOptions m_alertTextAlignment;
    public UberText.AnchorOptions m_alertTextAlignmentAnchor;
    public GameLayer? m_layerToUse;
    public bool m_keyboardEscIsCancel = true;
    public bool m_disableBnetBar;
    public bool m_blurWhenShown;

    public enum IconSet
    {
      Default,
      Alternate,
      None,
    }
  }
}
