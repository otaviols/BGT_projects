using UnityEngine;

public class ExistingAccountPopup : DialogBase
{
  public PegUIElement m_haveAccountButton;
  public PegUIElement m_noAccountButton;
  public GameObject m_bubble;
  public ExistingAccoundSound m_sound;
  private Vector3 m_buttonOffset = new Vector3(0.2f, 0.0f, 0.6f);
  private bool m_haveAccount;
  private ExistingAccountPopup.ResponseCallback m_responseCallback;

  private void Start()
  {
    this.transform.position = new Vector3(this.transform.position.x, -525f, 800f);
    if ((bool) (Object) this.m_haveAccountButton)
    {
      this.m_haveAccountButton.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.HaveAccountButtonReleaseAll));
      this.m_haveAccountButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.HaveAccountButtonPress));
    }
    if ((bool) (Object) this.m_noAccountButton)
    {
      this.m_noAccountButton.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.NoAccountButtonReleaseAll));
      this.m_noAccountButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.NoAccountButtonPress));
    }
    this.FadeEffectsIn();
  }

  public override void Show()
  {
    base.Show();
    BaseUI.Get().m_BnetBar.DisableButtonsByDialog((DialogBase) this);
    BaseUI.Get().m_BnetBar.HideGameMenu();
    BaseUI.Get().m_BnetBar.HideOptionsMenu();
    this.m_bubble.SetActive(true);
    iTween.FadeTo(this.m_bubble, iTween.Hash((object) "time", (object) 0.0f, (object) "amount", (object) 1f, (object) "oncomplete", (object) "ShowBubble", (object) "oncompletetarget", (object) this.gameObject));
    this.m_showAnimState = DialogBase.ShowAnimState.IN_PROGRESS;
    UniversalInputManager.Get().SetSystemDialogActive(true);
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_sound.m_popupShow);
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_sound.m_innkeeperWelcome);
  }

  public void SetInfo(ExistingAccountPopup.Info info) => this.m_responseCallback = info.m_callback;

  protected void FadeBubble() => iTween.FadeTo(this.m_bubble, iTween.Hash((object) "delay", (object) 6f, (object) "time", (object) 1f, (object) "amount", (object) 0.0f));

  protected void ShowBubble() => iTween.FadeFrom(this.m_bubble, iTween.Hash((object) "delay", (object) 1f, (object) "time", (object) 0.5f, (object) "amount", (object) 0.0f, (object) "oncomplete", (object) "FadeBubble", (object) "oncompletetarget", (object) this.gameObject));

  protected void DownScale() => iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "delay", (object) 0.1, (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "oncomplete", (object) "OnHideAnimFinished", (object) "time", (object) 0.2f));

  protected override void OnHideAnimFinished()
  {
    base.OnHideAnimFinished();
    this.m_shown = false;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_sound.m_popupHide);
    this.m_responseCallback(this.m_haveAccount);
  }

  private void HaveAccountButtonReleaseAll(UIEvent e)
  {
    this.m_haveAccountButton.transform.localPosition -= this.m_buttonOffset;
    if (!((UIReleaseAllEvent) e).GetMouseIsOver())
      return;
    TelemetryManager.Client().SendButtonPressed("HaveAccount");
    this.m_haveAccount = true;
    this.ScaleAway();
  }

  private void NoAccountButtonReleaseAll(UIEvent e)
  {
    this.m_noAccountButton.transform.localPosition -= this.m_buttonOffset;
    if (!((UIReleaseAllEvent) e).GetMouseIsOver())
      return;
    TelemetryManager.Client().SendButtonPressed("NoAccount");
    this.m_haveAccount = false;
    this.FadeEffectsOut();
  }

  private void HaveAccountButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_sound.m_buttonClick);
    this.m_haveAccountButton.transform.localPosition += this.m_buttonOffset;
  }

  private void NoAccountButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_sound.m_buttonClick);
    this.m_noAccountButton.transform.localPosition += this.m_buttonOffset;
  }

  private void ScaleAway() => iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.Scale(this.PUNCH_SCALE, this.gameObject.transform.localScale), (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "oncomplete", (object) "DownScale", (object) "time", (object) 0.1f));

  private void FadeEffectsIn()
  {
    ScreenEffectParameters vignettePerspective = ScreenEffectParameters.BlurVignettePerspective with
    {
      Blur = new BlurParameters(brightness: 1f)
    };
    DialogBase.m_screenEffectsHandle.StartEffect(vignettePerspective);
  }

  private void FadeEffectsOut()
  {
    DialogBase.m_screenEffectsHandle.StopEffect();
    this.ScaleAway();
  }

  public delegate void ResponseCallback(bool hasAccount);

  public class Info
  {
    public ExistingAccountPopup.ResponseCallback m_callback;
  }
}
