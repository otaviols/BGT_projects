using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using UnityEngine;

public class FriendListFSGFrame : FriendListUIElement
{
  public GameObject m_FSGFlyoutMenuTwoButtonFrame;
  public GameObject m_FSGFlyoutMenuThreeButtonFrame;
  public GameObject m_FSGJoinMenu;
  public GameObject m_FSGFlyout;
  public PegUIElement m_FSGJoinButton;
  public PegUIElement m_FSGEnterButton;
  public PegUIElement m_FSGLeaveButton;
  public PegUIElement m_FSGUpdateButton;
  public Renderer m_FSGEnterButtonRenderer;
  public Transform m_FSGPatronEnterButtonBone;
  public Transform m_FSGPatronLeaveButtonBone;
  public Transform m_FSGInnkeeperEnterButtonBone;
  public Transform m_FSGInnkeeperUpdateButtonBone;
  public Transform m_FSGInnkeeperLeaveButtonBone;
  public UberText m_FSGTitleText;
  public UberText m_TavernNameText;
  public UberText m_TavernNameJoinText;
  public GameObject m_Background;
  public GameObject m_HighlightBackground;
  public GameObject m_inFSGGradient;
  public GameObject m_LanternIcon;
  public GameObject m_ArrowIcon;
  public GameObject m_ArrowIconHighlight;
  public float m_TextXOffsetWithoutLantern;
  private long FSGID = -1;
  private PegUIElement m_FSGMenuInputBlocker;
  private bool m_FSGMenuOpen;
  private Vector3? m_fsgMenuOrigLocalPos;
  public bool m_isInnkeeperSetup;

  private bool IsCheckedIn => FiresideGatheringManager.Get().IsCheckedInToFSG(this.FSGID);

  private GameObject FSGMenu => !this.IsCheckedIn ? this.m_FSGJoinMenu : this.m_FSGFlyout;

  public void InitFrame(FSGConfig gathering)
  {
    this.FSGID = gathering.FsgId;
    this.m_isInnkeeperSetup = gathering.IsInnkeeper && !gathering.IsSetupComplete;
    if (this.m_isInnkeeperSetup)
    {
      string key = "GLUE_FIRESIDE_GATHERING_INNKEEPER_CLICK_TO_SETUP";
      if ((bool) UniversalInputManager.UsePhoneUI)
        key += "_PHONE";
      this.m_TavernNameText.Text = GameStrings.Get(key);
    }
    else
      this.m_TavernNameText.Text = FiresideGatheringManager.Get().GetTavernName_FriendsList(gathering);
    this.m_TavernNameJoinText.Text = FiresideGatheringManager.Get().GetTavernName_FriendsList(gathering);
    this.m_FSGTitleText.Text = !this.m_isInnkeeperSetup ? (this.IsCheckedIn ? GameStrings.Get("GLOBAL_FIRESIDE_GATHERING") : GameStrings.Get("GLUE_FSG_FOUND")) : GameStrings.Get("GLUE_FIRESIDE_GATHERING_INNKEEPER_CLICK_TO_SETUP_TITLE");
    this.m_FSGJoinButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnJoinButton));
    this.m_FSGEnterButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnEnterButton));
    this.m_FSGLeaveButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnLeaveButton));
    this.m_FSGUpdateButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnUpdateButton));
    bool fsg = FiresideGatheringManager.Get().IsCheckedInToFSG(this.FSGID);
    this.m_LanternIcon.SetActive(fsg);
    this.m_inFSGGradient.SetActive(fsg);
    if (fsg)
    {
      if (gathering.IsInnkeeper)
      {
        this.m_FSGFlyoutMenuTwoButtonFrame.SetActive(false);
        this.m_FSGFlyoutMenuThreeButtonFrame.SetActive(true);
        this.m_FSGUpdateButton.gameObject.SetActive(true);
        GameUtils.SetParent((Component) this.m_FSGEnterButton.transform, (Component) this.m_FSGInnkeeperEnterButtonBone);
        GameUtils.SetParent((Component) this.m_FSGUpdateButton.transform, (Component) this.m_FSGInnkeeperUpdateButtonBone);
        GameUtils.SetParent((Component) this.m_FSGLeaveButton.transform, (Component) this.m_FSGInnkeeperLeaveButtonBone);
      }
      else
      {
        this.m_FSGFlyoutMenuTwoButtonFrame.SetActive(true);
        this.m_FSGFlyoutMenuThreeButtonFrame.SetActive(false);
        this.m_FSGUpdateButton.gameObject.SetActive(false);
        GameUtils.SetParent((Component) this.m_FSGEnterButton.transform, (Component) this.m_FSGPatronEnterButtonBone);
        GameUtils.SetParent((Component) this.m_FSGLeaveButton.transform, (Component) this.m_FSGPatronLeaveButtonBone);
      }
      this.SetEnabled(this.ShouldEnableEnterButton(), this.m_FSGEnterButton, this.m_FSGEnterButtonRenderer);
    }
    else
    {
      Vector3 localPosition1 = this.m_TavernNameText.transform.localPosition;
      Vector3 localPosition2 = this.m_FSGTitleText.transform.localPosition;
      localPosition1.x = this.m_TextXOffsetWithoutLantern;
      localPosition2.x = this.m_TextXOffsetWithoutLantern;
      this.m_TavernNameText.transform.localPosition = localPosition1;
      this.m_FSGTitleText.transform.localPosition = localPosition2;
    }
    if (FiresideGatheringManager.Get().m_activeFSGMenu != this.FSGID)
      return;
    this.OpenFSGMenu();
  }

  private bool ShouldEnableEnterButton()
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    return UserAttentionManager.GetAvailabilityBlockerReason(false) == AvailabilityBlockerReasons.NONE && mode != SceneMgr.Mode.FIRESIDE_GATHERING;
  }

  protected override void OnDestroy()
  {
    this.m_FSGJoinButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnJoinButton));
    this.m_FSGLeaveButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnLeaveButton));
    this.m_FSGEnterButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnEnterButton));
    this.m_FSGUpdateButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnUpdateButton));
    base.OnDestroy();
  }

  protected override void OnRelease()
  {
    if (this.m_isInnkeeperSetup)
      FiresideGatheringManager.Get().ShowFiresideGatheringInnkeeperSetupDialog();
    else if (this.m_FSGMenuOpen)
      this.CloseFSGMenu();
    else
      this.OpenFSGMenu();
  }

  private void OpenFSGMenu()
  {
    if ((Object) this.FSGMenu == (Object) null || this.m_FSGMenuOpen)
      return;
    FiresideGatheringManager.Get().m_activeFSGMenu = this.FSGID;
    this.m_HighlightBackground.SetActive(true);
    this.m_FSGMenuOpen = true;
    this.FSGMenu.gameObject.SetActive(true);
    if (this.m_fsgMenuOrigLocalPos.HasValue)
      this.FSGMenu.gameObject.transform.localPosition = this.m_fsgMenuOrigLocalPos.Value;
    Bounds bounds = this.FSGMenu.GetComponent<Collider>().bounds;
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.FSGMenu.layer);
    Vector3 screenPoint1 = firstByLayer.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.center.z));
    if ((double) screenPoint1.y < 0.0)
    {
      if (!this.m_fsgMenuOrigLocalPos.HasValue)
        this.m_fsgMenuOrigLocalPos = new Vector3?(this.FSGMenu.gameObject.transform.localPosition);
      Vector3 screenPoint2 = firstByLayer.WorldToScreenPoint(this.FSGMenu.gameObject.transform.position);
      this.FSGMenu.gameObject.transform.position = firstByLayer.ScreenToWorldPoint(new Vector3(screenPoint2.x, screenPoint2.y - screenPoint1.y, screenPoint2.z));
    }
    this.InitFSGMenuInputBlocker();
  }

  private void CloseFSGMenu()
  {
    if ((Object) this.FSGMenu == (Object) null || !this.m_FSGMenuOpen)
      return;
    FiresideGatheringManager.Get().m_activeFSGMenu = -1L;
    this.m_HighlightBackground.SetActive(false);
    this.m_FSGMenuOpen = false;
    this.FSGMenu.gameObject.SetActive(false);
    if (!((Object) this.m_FSGMenuInputBlocker != (Object) null))
      return;
    Object.Destroy((Object) this.m_FSGMenuInputBlocker.gameObject);
    this.m_FSGMenuInputBlocker = (PegUIElement) null;
  }

  private void InitFSGMenuInputBlocker()
  {
    if ((Object) this.m_FSGMenuInputBlocker != (Object) null)
    {
      Object.Destroy((Object) this.m_FSGMenuInputBlocker.gameObject);
      this.m_FSGMenuInputBlocker = (PegUIElement) null;
    }
    this.m_FSGMenuInputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.FSGMenu.layer), "FSGMenuInputBlocker", (Component) this.FSGMenu.transform).AddComponent<PegUIElement>();
    this.m_FSGMenuInputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFSGMenuInputBlockerReleased));
    this.m_FSGMenuInputBlocker.gameObject.layer = this.FSGMenu.layer;
    TransformUtil.SetPosZ((Component) this.m_FSGMenuInputBlocker, this.FSGMenu.transform.position.z + 1f);
  }

  private void OnFSGMenuInputBlockerReleased(UIEvent e) => this.CloseFSGMenu();

  private void OnEnterButton(UIEvent e)
  {
    if (!this.ShouldEnableEnterButton() || SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FIRESIDE_GATHERING))
      return;
    Navigation.Clear();
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.FIRESIDE_GATHERING);
    BnetBar.Get().HideFriendList();
    FiresideGatheringManager.Get().EnableTransitionInputBlocker(true);
  }

  private void SetEnabled(bool enabled, PegUIElement button, Renderer buttonRenderer)
  {
    button.SetEnabled(enabled);
    buttonRenderer.GetMaterial().SetFloat("_Desaturate", enabled ? 0.0f : 1f);
  }

  private void OnJoinButton(UIEvent e)
  {
    ChatMgr.Get().CloseChatUI();
    FiresideGatheringManager.Get().CheckInToFSG(this.FSGID);
  }

  private void OnLeaveButton(UIEvent e)
  {
    ChatMgr.Get().CloseChatUI();
    FiresideGatheringManager.Get().CheckOutOfFSG(true);
  }

  private void OnUpdateButton(UIEvent e)
  {
    ChatMgr.Get().CloseChatUI();
    FiresideGatheringManager.Get().CheckOutOfFSG(true);
    FiresideGatheringManager.Get().ShowFiresideGatheringInnkeeperSetupDialog();
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    base.OnOver(oldState);
    this.m_Background.SetActive(false);
    this.m_HighlightBackground.SetActive(true);
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9"));
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    base.OnOut(oldState);
    this.m_Background.SetActive(true);
    if (this.m_FSGMenuOpen)
      return;
    this.m_HighlightBackground.SetActive(false);
  }
}
