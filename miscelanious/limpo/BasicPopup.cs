using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class BasicPopup : DialogBase
{
  public UIBButton m_cancelButton;
  public UIBButton m_customButton;
  public UberText m_headerText;
  public UberText m_bodyText;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_showAnimationSound = "Expand_Up.prefab:775d97ea42498c044897f396362b9db3";
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_hideAnimationSound = "Shrink_Down_Quicker.prefab:2fe963b171811ca4b8d544fa53e3330c";
  protected BasicPopup.PopupInfo m_popupInfo;

  protected override void Awake()
  {
    base.Awake();
    if ((Object) this.m_cancelButton != (Object) null)
      this.m_cancelButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ButtonPress(BasicPopup.Response.CANCEL)));
    if (!((Object) this.m_customButton != (Object) null))
      return;
    this.m_customButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ButtonPress(BasicPopup.Response.CUSTOM_RESPONSE)));
  }

  public override bool HandleKeyboardInput()
  {
    if (!InputCollection.GetKeyUp(KeyCode.Escape))
      return false;
    this.GoBack();
    return true;
  }

  public override void GoBack() => this.ButtonPress(BasicPopup.Response.CANCEL);

  public void SetInfo(BasicPopup.PopupInfo info) => this.m_popupInfo = info;

  public override void Show()
  {
    base.Show();
    this.InitInfo();
    if (this.m_popupInfo.m_disableBnetBar)
      BnetBar.Get().DisableButtonsByDialog((DialogBase) this);
    if (this.m_popupInfo.m_blurWhenShown)
      DialogBase.DoBlur();
    this.DoShowAnimation();
    if (string.IsNullOrEmpty(this.m_showAnimationSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_showAnimationSound);
  }

  public override void Hide()
  {
    base.Hide();
    if (!this.m_popupInfo.m_blurWhenShown)
      return;
    DialogBase.EndBlur();
  }

  protected override void OnHideAnimFinished()
  {
    UniversalInputManager.Get().SetSystemDialogActive(false);
    base.OnHideAnimFinished();
    if (string.IsNullOrEmpty(this.m_hideAnimationSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_hideAnimationSound);
  }

  private void InitInfo()
  {
    if (this.m_popupInfo == null)
      this.m_popupInfo = new BasicPopup.PopupInfo();
    if ((Object) this.m_headerText != (Object) null && this.m_popupInfo.m_headerText != null)
      this.m_headerText.Text = this.m_popupInfo.m_headerText;
    if (!((Object) this.m_bodyText != (Object) null) || this.m_popupInfo.m_bodyText == null)
      return;
    this.m_bodyText.Text = this.m_popupInfo.m_bodyText;
  }

  private void ButtonPress(BasicPopup.Response response)
  {
    if (this.m_popupInfo.m_responseCallback != null)
      this.m_popupInfo.m_responseCallback(response, this.m_popupInfo.m_responseUserData);
    this.Hide();
  }

  public enum Response
  {
    CANCEL,
    CUSTOM_RESPONSE,
  }

  public delegate void ResponseCallback(BasicPopup.Response response, object userData);

  public class PopupInfo
  {
    public readonly List<string> m_prefabAssetRefs = new List<string>();
    public BasicPopup.ResponseCallback m_responseCallback;
    public object m_responseUserData;
    public string m_headerText;
    public string m_bodyText;
    public bool m_disableBnetBar;
    public bool m_blurWhenShown;
  }
}
