using UnityEngine;

public class TemporaryAccountGameMenu : UIBPopup, GameMenuInterface
{
  public UIBButton m_signUpButton;
  public UIBButton m_optionsButton;
  public GameObject m_koreanRatings;
  private static readonly Vector3 SHOW_POS_PHONE = new Vector3(0.0f, 0.0f, 0.0f);
  private static readonly Vector3 SHOW_SCALE_PHONE = new Vector3(75f, 75f, 75f);
  private static readonly Vector3 SHOW_POS_PHONE_KR = new Vector3(0.0f, 0.0f, 15f);
  private static readonly Vector3 SHOW_SCALE_PHONE_KR = new Vector3(65f, 65f, 65f);
  private GameMenuBase m_gameMenuBase;
  private PegUIElement m_inputBlockerPegUIElement;

  protected override void Awake()
  {
    base.Awake();
    this.m_destroyOnSceneLoad = false;
    this.m_gameMenuBase = new GameMenuBase();
    this.m_gameMenuBase.m_showCallback = new GameMenuBase.ShowCallback(((UIBPopup) this).Show);
    this.m_gameMenuBase.m_hideCallback = new GameMenuBase.HideCallback(((UIBPopup) this).Hide);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_showPosition = this.m_gameMenuBase.UseKoreanRating() ? TemporaryAccountGameMenu.SHOW_POS_PHONE_KR : TemporaryAccountGameMenu.SHOW_POS_PHONE;
      this.m_showScale = this.m_gameMenuBase.UseKoreanRating() ? TemporaryAccountGameMenu.SHOW_SCALE_PHONE_KR : TemporaryAccountGameMenu.SHOW_SCALE_PHONE;
    }
    this.m_signUpButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSignUpPressed));
    this.m_optionsButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOptionsPressed));
  }

  private void OnDestroy() => this.m_gameMenuBase.DestroyOptionsMenu();

  public bool GameMenuIsShown() => this.IsShown();

  public void GameMenuShow() => this.Show();

  public void GameMenuHide() => this.Hide();

  public void GameMenuShowOptionsMenu() => this.ShowOptionsMenu();

  public GameObject GameMenuGetGameObject() => this.gameObject;

  public override void Show()
  {
    if (this.IsShown())
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.Show(true);
    if (this.m_gameMenuBase.UseKoreanRating())
      this.m_koreanRatings.SetActive(true);
    this.gameObject.SetActive(true);
    if ((Object) this.m_inputBlockerPegUIElement != (Object) null)
    {
      Object.Destroy((Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "TemporaryAccountGameMenuInputBlocker");
    LayerUtils.SetLayer(inputBlocker, this.gameObject.layer);
    this.m_inputBlockerPegUIElement = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlockerPegUIElement.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerRelease));
    TransformUtil.SetPosY((Component) this.m_inputBlockerPegUIElement, this.gameObject.transform.position.y - 5f);
    BnetBar.Get().m_menuButton.SetSelected(true);
  }

  public override void Hide()
  {
    this.Hide(false);
    BnetBar.Get().m_menuButton.SetSelected(false);
  }

  protected override void Hide(bool animate)
  {
    if (!this.IsShown())
      return;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    if ((Object) this.m_inputBlockerPegUIElement != (Object) null)
    {
      Object.Destroy((Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    if ((Object) this.gameObject != (Object) null)
      this.gameObject.SetActive(false);
    base.Hide(animate);
  }

  public void ShowOptionsMenu()
  {
    if (this.m_gameMenuBase == null)
      return;
    this.m_gameMenuBase.ShowOptionsMenu();
  }

  private void OnSignUpPressed(UIEvent e)
  {
    this.Hide();
    TemporaryAccountManager.Get().ShowHealUpPage(TemporaryAccountManager.HealUpReason.GAME_MENU);
  }

  private void OnOptionsPressed(UIEvent e) => this.ShowOptionsMenu();

  private bool OnNavigateBack()
  {
    this.Hide();
    return true;
  }

  private void OnInputBlockerRelease(UIEvent e) => this.Hide();
}
