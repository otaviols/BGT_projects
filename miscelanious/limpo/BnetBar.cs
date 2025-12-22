using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class BnetBar : MonoBehaviour
{
  public UberText m_currentTime;
  public BnetBarMenuButton m_menuButton;
  public GameObject m_menuButtonMesh;
  public BnetBarFriendButton m_friendButton;
  public GameObject m_currencyFrameContainer;
  public Flipbook m_batteryLevel;
  public Flipbook m_batteryLevelPhone;
  public GameObject m_socialToastBone;
  public ConnectionIndicator m_connectionIndicator;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_spectatorCountPrefabPath;
  public TooltipZone m_spectatorCountTooltipZone;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_spectatorModeIndicatorPrefab;
  [Header("Phone Aspect Ratio")]
  public float HorizontalMarginMinAspectRatio;
  public float HorizontalMarginWideAspectRatio;
  public float HorizontalMarginExtraWideAspectRatio;
  public static readonly int CameraDepth = 47;
  private static BnetBar s_instance;
  private float m_initialWidth;
  private float m_initialFriendButtonScaleX;
  private float m_initialMenuButtonScaleX;
  private float m_initialSpectatorModeIndicatorScaleX;
  private float m_initialSpectatorCountScaleX;
  private GameMenuInterface m_gameMenu;
  private bool m_gameMenuLoading;
  private bool m_isInitting = true;
  private GameObject m_loginTooltip;
  private bool m_hasUnacknowledgedPendingInvites;
  private GameObject m_spectatorCountPanel;
  private GameObject m_spectatorModeIndicator;
  private bool m_isLoggedIn;
  private bool m_buttonsEnabled;
  private bool m_buttonsDisabledPermanently;
  private int m_buttonsDisabledByRefCount;
  private HashSet<DialogBase> m_buttonsDisabledByDialog = new HashSet<DialogBase>();
  private bool m_suppressLoginTooltip;
  private float m_lastClockUpdate;
  private bool m_lastClockUpdateCanShowServerTime;
  private double m_serverClientOffsetInSec;
  private const float MENU_BUTTON_LOCAL_X_OFFSET = 0.14f;
  private const float CURRENCY_CONTAINER_LOCAL_Y = -2.850989f;
  private const float CURRENCY_CONTAINER_LOCAL_Y_MOBILE = 189.703f;
  private readonly Vector3 BATTERY_LEVEL_LAYOUT_OFFSET = new Vector3(3f, 1.25f, 0.0f);
  private List<CurrencyFrame> m_currencyFrames = new List<CurrencyFrame>();
  private VarKey m_showServerTime = Vars.Key("Application.ShowServerTime");
  private static readonly Vector3 LAYOUT_TOPLEFT_START_POINT = new Vector3(1.5f, 189f, 0.0f);
  private static readonly Vector3 LAYOUT_BOTTOMLEFT_START_POINT = new Vector3(1.5f, 0.0f, 1.25f);
  private static readonly PlatformDependentValue<Vector3> LAYOUT_OFFSET_PADDING = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.0f, 0.0f, 0.0f),
    Tablet = new Vector3(4f, 0.0f, 0.0f),
    MiniTablet = new Vector3(4f, 0.0f, 0.0f),
    Phone = new Vector3(4f, 0.0f, 8f)
  };
  private static readonly PlatformDependentValue<Vector3> LAYOUT_OFFSET_CURRENCY = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.0f, 0.0f, 1f),
    Phone = new Vector3(0.0f, 0.0f, -3.4f)
  };
  private static readonly PlatformDependentValue<Vector3> LAYOUT_OFFSET_SPECTATOR_WIDGET = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(2f, 0.0f, 0.0f),
    Phone = new Vector3(8f, 0.0f, 0.0f)
  };
  private static readonly PlatformDependentValue<Vector3> LAYOUT_OFFSET_SPECTATOR_COUNT_WIDGET = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(4f, 0.0f, 1f),
    Phone = new Vector3(0.0f, 0.0f, 0.0f)
  };

  public event System.Action OnMenuOpened;

  [CustomEditField(Hide = true)]
  public float HorizontalMargin => (bool) UniversalInputManager.UsePhoneUI ? TransformUtil.GetAspectRatioDependentValue(this.HorizontalMarginMinAspectRatio, this.HorizontalMarginWideAspectRatio, this.HorizontalMarginExtraWideAspectRatio) : 0.0f;

  private void Awake()
  {
    BnetBar.s_instance = this;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_menuButton.transform.localScale *= 2f;
      this.m_friendButton.transform.localScale *= 2f;
    }
    else
      this.m_connectionIndicator.gameObject.SetActive(false);
    this.m_initialWidth = this.GetComponent<Renderer>().bounds.size.x;
    this.m_initialFriendButtonScaleX = this.m_friendButton.transform.localScale.x;
    this.m_initialMenuButtonScaleX = this.m_menuButton.transform.localScale.x;
    this.m_menuButton.StateChanged = new System.Action(this.UpdateLayout);
    this.m_menuButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnMenuButtonReleased));
    this.m_friendButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFriendButtonReleased));
    this.UpdateButtonEnableState();
    this.m_batteryLevel.gameObject.SetActive(false);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_batteryLevel = this.m_batteryLevelPhone;
      this.m_currentTime.gameObject.SetActive(false);
    }
    this.m_menuButton.SetPhoneStatusBarState(0);
    this.m_friendButton.gameObject.SetActive(false);
    this.ToggleActive(false);
  }

  private void OnDestroy()
  {
    if (!HearthstoneApplication.IsHearthstoneClosing)
    {
      SpectatorManager.Get().OnInviteReceived -= new SpectatorManager.InviteReceivedHandler(this.SpectatorManager_OnInviteReceived);
      SpectatorManager.Get().OnSpectatorToMyGame -= new SpectatorManager.SpectatorToMyGameHandler(this.SpectatorManager_OnSpectatorToMyGame);
      SpectatorManager.Get().OnSpectatorModeChanged -= new SpectatorManager.SpectatorModeChangedHandler(this.SpectatorManager_OnSpectatorModeChanged);
      if (Network.Get() != null)
        Network.Get().RemoveNetHandler((object) GetServerTimeResponse.PacketID.ID, new Network.NetHandler(this.OnRequestGetServerTimeResponse));
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
        hearthstoneApplication.WillReset -= new System.Action(this.WillReset);
    }
    BnetBar.s_instance = (BnetBar) null;
  }

  private void Start()
  {
    if (SceneMgr.Get() != null)
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    if (SpectatorManager.Get() != null)
    {
      SpectatorManager.Get().OnInviteReceived += new SpectatorManager.InviteReceivedHandler(this.SpectatorManager_OnInviteReceived);
      SpectatorManager.Get().OnSpectatorToMyGame += new SpectatorManager.SpectatorToMyGameHandler(this.SpectatorManager_OnSpectatorToMyGame);
      SpectatorManager.Get().OnSpectatorModeChanged += new SpectatorManager.SpectatorModeChangedHandler(this.SpectatorManager_OnSpectatorModeChanged);
    }
    if (Network.Get() != null)
      Network.Get().RegisterNetHandler((object) GetServerTimeResponse.PacketID.ID, new Network.NetHandler(this.OnRequestGetServerTimeResponse));
    HearthstoneApplication.Get().WillReset += new System.Action(this.WillReset);
    this.m_friendButton.gameObject.SetActive(false);
    if (!((UnityEngine.Object) this.m_friendButton != (UnityEngine.Object) null))
      return;
    this.m_friendButton.ShowPendingInvitesIcon(this.m_hasUnacknowledgedPendingInvites);
  }

  private void Update()
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    if ((double) realtimeSinceStartup - (double) this.m_lastClockUpdate <= 1.0)
      return;
    this.m_lastClockUpdate = realtimeSinceStartup;
    bool flag = !HearthstoneApplication.IsPublic() && this.m_showServerTime.GetBool(true);
    DateTime serverTime;
    if (flag && this.TryGetServerTime(out serverTime))
      this.m_currentTime.Text = GameStrings.Format("GLOBAL_CURRENT_TIME_AND_DATE_DEV", (object) GameStrings.Format("GLOBAL_CURRENT_TIME", (object) DateTime.Now), (object) GameStrings.Format("GLOBAL_CURRENT_DATE", (object) serverTime), (object) GameStrings.Format("GLOBAL_CURRENT_TIME", (object) serverTime));
    else if (Localization.GetLocale() == Locale.enGB)
      this.m_currentTime.Text = string.Format("{0:HH:mm}", (object) DateTime.Now);
    else
      this.m_currentTime.Text = GameStrings.Format("GLOBAL_CURRENT_TIME", (object) DateTime.Now);
    if (Localization.GetLocale() == Locale.koKR)
      this.m_currentTime.Text = this.m_currentTime.Text.Replace("AM", GameStrings.Format("GLOBAL_CURRENT_TIME_AM")).Replace("PM", GameStrings.Format("GLOBAL_CURRENT_TIME_PM"));
    if (flag == this.m_lastClockUpdateCanShowServerTime)
      return;
    this.UpdateLayout();
    this.m_lastClockUpdateCanShowServerTime = flag;
  }

  public static BnetBar Get() => BnetBar.s_instance;

  public void OnLoggedIn()
  {
    if (Network.ShouldBeConnectedToAurora())
      this.m_friendButton.gameObject.SetActive(true);
    Network.Get().GetServerTimeRequest();
    this.m_isLoggedIn = true;
    this.ToggleActive(true);
    this.Update();
    this.UpdateLayout();
  }

  public void UpdateLayout()
  {
    if (!this.m_isLoggedIn)
      return;
    float num1 = 0.5f;
    Bounds nearClipBounds = CameraUtils.GetNearClipBounds(PegUI.Get().orthographicUICam);
    nearClipBounds.size = new Vector3(nearClipBounds.size.x, nearClipBounds.size.z, nearClipBounds.size.y);
    nearClipBounds.min += new Vector3(this.HorizontalMargin / 4f, 0.0f, 0.0f);
    nearClipBounds.max -= new Vector3(this.HorizontalMargin / 4f, 0.0f, 0.0f);
    float x1 = (nearClipBounds.size.x + num1) / this.m_initialWidth;
    float num2 = x1 * 0.25f;
    TransformUtil.SetLocalPosX(this.gameObject, (float) (((double) nearClipBounds.min.x - (double) this.transform.parent.localPosition.x - (double) num1) * 4.0));
    TransformUtil.SetLocalScaleX(this.gameObject, x1);
    float x2 = -0.03f * num2;
    if (GeneralUtils.IsDevelopmentBuildTextVisible())
      x2 -= CameraUtils.ScreenToWorldDist(PegUI.Get().orthographicUICam, 115f);
    float y = 1f * this.transform.localScale.y;
    bool flag1 = true;
    if (!DemoMgr.Get().IsHubEscMenuEnabled(SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY))
      flag1 = false;
    this.m_menuButton.gameObject.SetActive(flag1);
    TransformUtil.SetLocalScaleX((Component) this.m_menuButton, this.m_initialMenuButtonScaleX / x1);
    TransformUtil.SetPoint((Component) this.m_menuButton, Anchor.RIGHT, this.gameObject, Anchor.RIGHT, new Vector3(x2, y, 0.0f) - (Vector3) BnetBar.LAYOUT_OFFSET_PADDING);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      TransformUtil.SetPoint((Component) this.m_menuButton, Anchor.RIGHT, this.gameObject, Anchor.RIGHT, new Vector3(x2 * 4f, y, 0.0f));
      TransformUtil.SetLocalPosX((Component) this.m_menuButton, this.m_menuButton.transform.localPosition.x + 0.14f);
      TransformUtil.SetLocalPosY((Component) this.m_menuButton, BnetBar.LAYOUT_TOPLEFT_START_POINT.y);
      this.m_batteryLevel.gameObject.SetActive(true);
      this.m_menuButton.SetPhoneStatusBarState(1 + (this.m_connectionIndicator.IsVisible() ? 1 : 0));
      TransformUtil.SetLocalScaleX(this.m_currencyFrameContainer, 2f / x1);
      TransformUtil.SetLocalScaleY(this.m_currencyFrameContainer, 0.4f);
      if (flag1)
        this.PositionCurrencyFrame(this.m_batteryLevel.gameObject, new Vector3(this.m_menuButton.GetCurrencyFrameOffsetX(), 0.0f, BnetBar.LAYOUT_OFFSET_CURRENCY.Value.z));
      else
        this.PositionCurrencyFrame(this.m_batteryLevel.gameObject, new Vector3(100f, 0.0f, BnetBar.LAYOUT_OFFSET_CURRENCY.Value.z));
    }
    else
    {
      TransformUtil.SetPoint((Component) this.m_menuButton, Anchor.RIGHT, this.gameObject, Anchor.RIGHT, new Vector3(x2, y, 0.0f));
      TransformUtil.SetLocalScaleX(this.m_currencyFrameContainer, 1f / x1);
      this.PositionCurrencyFrame(this.m_menuButton.gameObject, new Vector3(this.m_menuButton.GetCurrencyFrameOffsetX(), 0.0f, BnetBar.LAYOUT_OFFSET_CURRENCY.Value.z));
    }
    bool flag2 = (UnityEngine.Object) this.m_spectatorCountPanel != (UnityEngine.Object) null && this.m_spectatorCountPanel.activeInHierarchy && SpectatorManager.Get().IsBeingSpectated();
    bool show = !flag2 && (UnityEngine.Object) this.m_spectatorModeIndicator != (UnityEngine.Object) null && this.ShouldShowSpectatorModeIndicator;
    if ((bool) UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
    {
      flag2 = false;
      show = false;
    }
    this.ShowSpectatorModeIndicator(show);
    GameObject previousWidget = (GameObject) null;
    bool flag3 = false;
    if (this.m_friendButton.gameObject.activeInHierarchy)
    {
      TransformUtil.SetLocalScaleX((Component) this.m_friendButton, this.m_initialFriendButtonScaleX / x1);
      BnetBar.LayoutWidget_BottomLeft_Relative(this.m_friendButton.transform, ref previousWidget);
      TransformUtil.SetLocalScaleX(this.m_socialToastBone, 1f / x1);
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        previousWidget = (GameObject) null;
        TransformUtil.SetLocalPosY((Component) this.m_friendButton, BnetBar.LAYOUT_TOPLEFT_START_POINT.y);
        if ((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null && (UnityEngine.Object) ChatMgr.Get().FriendListFrame != (UnityEngine.Object) null)
        {
          TransformUtil.SetPosY((Component) this.m_friendButton, ChatMgr.Get().FriendListFrame.transform.position.y - 1f);
          flag3 = true;
        }
      }
    }
    if (flag2)
    {
      TransformUtil.SetLocalScaleX(this.m_spectatorCountPanel, this.m_initialSpectatorCountScaleX / x1);
      BnetBar.LayoutWidget_BottomLeft_Relative(this.m_spectatorCountPanel.transform, ref previousWidget, (Vector3) BnetBar.LAYOUT_OFFSET_SPECTATOR_COUNT_WIDGET);
      if (flag3)
        TransformUtil.SetPosY(this.m_spectatorCountPanel, ChatMgr.Get().FriendListFrame.transform.position.y + 1f);
    }
    if (show)
    {
      TransformUtil.SetLocalScaleX(this.m_spectatorModeIndicator, this.m_initialSpectatorModeIndicatorScaleX / x1);
      BnetBar.LayoutWidget_BottomLeft_Relative(this.m_spectatorModeIndicator.transform, ref previousWidget, (Vector3) BnetBar.LAYOUT_OFFSET_SPECTATOR_WIDGET);
      if (flag3)
        TransformUtil.SetPosY(this.m_spectatorModeIndicator, ChatMgr.Get().FriendListFrame.transform.position.y + 1f);
    }
    GameObject dst = previousWidget;
    Vector3 vector3;
    Vector3 offsetFromPrevious;
    if ((UnityEngine.Object) previousWidget == (UnityEngine.Object) null)
    {
      vector3 = BnetBar.LAYOUT_BOTTOMLEFT_START_POINT;
      offsetFromPrevious = Vector3.zero;
    }
    else if ((UnityEngine.Object) previousWidget == (UnityEngine.Object) this.m_friendButton.gameObject)
    {
      vector3 = new Vector3(3.75f, 1f, 4f);
      offsetFromPrevious = new Vector3(5.5f, 0.0f, BnetBar.LAYOUT_OFFSET_CURRENCY.Value.z);
    }
    else
    {
      vector3 = new Vector3(1.75f, 1f, 4f);
      offsetFromPrevious = new Vector3(3.5f, 0.0f, BnetBar.LAYOUT_OFFSET_CURRENCY.Value.z);
    }
    Vector3 offset = vector3 + (Vector3) BnetBar.LAYOUT_OFFSET_PADDING;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      dst = this.m_friendButton.gameObject;
      if (!this.m_friendButton.gameObject.activeInHierarchy)
      {
        dst = (GameObject) null;
        offset = BnetBar.LAYOUT_TOPLEFT_START_POINT;
      }
    }
    offset.z = -1f;
    TransformUtil.SetPoint(this.m_socialToastBone, Anchor.LEFT_XZ, dst, Anchor.RIGHT_XZ, offset);
    TransformUtil.SetLocalScaleX((Component) this.m_currentTime, 1f / x1);
    BnetBar.LayoutWidget_BottomLeft_Relative(this.m_currentTime.transform, ref previousWidget, offsetFromPrevious);
    if (PlatformSettings.IsTablet && this.m_isLoggedIn)
    {
      this.m_batteryLevel.gameObject.SetActive(true);
      BnetBar.LayoutWidget_LeftAligned_SetExactOffset(this.m_batteryLevel.transform, this.m_currentTime.gameObject, new Vector3(12f, 5f, 0.0f));
    }
    this.UpdateLoginTooltip();
    if (this.m_isInitting)
    {
      foreach (CurrencyFrame currencyFrame in this.m_currencyFrames)
        currencyFrame.Hide(true);
      this.m_isInitting = false;
    }
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.UpdateForPhone();
  }

  public bool TryGetRelevantCurrencyFrame(
    CurrencyType currencyType,
    out CurrencyFrame currencyFrame)
  {
    currencyFrame = (CurrencyFrame) null;
    foreach (CurrencyFrame currencyFrame1 in this.m_currencyFrames)
    {
      if (currencyFrame1.CurrentCurrencyType == currencyType)
      {
        currencyFrame = currencyFrame1;
        return true;
      }
    }
    return false;
  }

  public void RefreshCurrency()
  {
    List<CurrencyType> list = CurrencyFrame.GetVisibleCurrencies().ToList<CurrencyType>();
    if (list.Count > this.m_currencyFrames.Count)
      Log.BattleNet.PrintWarning("More visible currencies then there are existing Currency Frames. This will lead to some currencies not being displayed");
    for (int index = 0; index < this.m_currencyFrames.Count; ++index)
    {
      CurrencyFrame currencyFrame = this.m_currencyFrames[index];
      if (index < list.Count)
      {
        currencyFrame.Bind(list[index]);
        currencyFrame.Show();
      }
      else
      {
        currencyFrame.Bind(CurrencyType.NONE);
        currencyFrame.Hide();
      }
    }
    this.UpdateLayout();
  }

  public void RegisterCurrencyFrame(CurrencyFrame currencyFrame)
  {
    if (this.m_currencyFrames.Contains(currencyFrame))
      return;
    for (int index = 0; index < this.m_currencyFrameContainer.transform.childCount; ++index)
    {
      if ((UnityEngine.Object) this.m_currencyFrameContainer.transform.GetChild(index).GetComponentInChildren<CurrencyFrame>(true) == (UnityEngine.Object) currencyFrame)
      {
        if (index < 0 || index >= this.m_currencyFrames.Count)
        {
          this.m_currencyFrames.Add(currencyFrame);
          return;
        }
        this.m_currencyFrames.Insert(index, currencyFrame);
        return;
      }
    }
    this.m_currencyFrames.Add(currencyFrame);
  }

  public void SetBlockCurrencyFrames(bool isBlocked)
  {
    foreach (CurrencyFrame currencyFrame in this.m_currencyFrames)
      currencyFrame.SetBlocked(isBlocked);
  }

  public void ShowCurrencyFrames(bool isImmediate = false)
  {
    foreach (CurrencyFrame currencyFrame in this.m_currencyFrames)
      currencyFrame.Show(isImmediate);
  }

  public void HideCurrencyFrames(bool isImmediate = false)
  {
    foreach (CurrencyFrame currencyFrame in this.m_currencyFrames)
      currencyFrame.Hide(isImmediate);
  }

  public bool IsCurrencyFrameActive()
  {
    foreach (CurrencyFrame currencyFrame in this.m_currencyFrames)
    {
      if (currencyFrame.IsShown())
        return true;
    }
    return false;
  }

  public bool TryGetServerTime(out DateTime serverTime)
  {
    if (this.m_serverClientOffsetInSec != double.MaxValue)
    {
      ref DateTime local = ref serverTime;
      DateTime dateTime = DateTime.UtcNow;
      dateTime = dateTime.AddSeconds(this.m_serverClientOffsetInSec);
      DateTime localTime = dateTime.ToLocalTime();
      local = localTime;
      return true;
    }
    serverTime = DateTime.UtcNow;
    return false;
  }

  private static void LayoutWidget_LeftAligned_SetExactOffset(
    Transform transform,
    GameObject previousWidget,
    Vector3 exactOffset)
  {
    if (!transform.gameObject.activeInHierarchy)
      return;
    if ((UnityEngine.Object) previousWidget == (UnityEngine.Object) null)
      TransformUtil.SetPoint((Component) transform, Anchor.LEFT, BnetBar.Get().gameObject, Anchor.LEFT, exactOffset);
    else
      TransformUtil.SetPoint((Component) transform, Anchor.LEFT, previousWidget, Anchor.RIGHT, exactOffset);
  }

  private static void LayoutWidget_BottomLeft_Relative(
    Transform transform,
    ref GameObject previousWidget,
    Vector3 offsetFromPrevious = default (Vector3))
  {
    if (!transform.gameObject.activeInHierarchy)
      return;
    if ((UnityEngine.Object) previousWidget == (UnityEngine.Object) null)
    {
      BnetBar.LayoutWidget_LeftAligned_SetExactOffset(transform, previousWidget, BnetBar.LAYOUT_BOTTOMLEFT_START_POINT);
      previousWidget = transform.gameObject;
    }
    else
    {
      BnetBar.LayoutWidget_LeftAligned_SetExactOffset(transform, previousWidget, offsetFromPrevious + (Vector3) BnetBar.LAYOUT_OFFSET_PADDING);
      previousWidget = transform.gameObject;
    }
  }

  private void PositionCurrencyFrame(GameObject parent, Vector3 offset)
  {
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (CurrencyFrame currencyFrame in this.m_currencyFrames)
    {
      GameObject tooltipObject = currencyFrame.GetTooltipObject();
      if ((UnityEngine.Object) tooltipObject != (UnityEngine.Object) null)
      {
        tooltipObject.SetActive(false);
        gameObjectList.Add(tooltipObject);
      }
    }
    TransformUtil.SetPoint(this.m_currencyFrameContainer, Anchor.RIGHT, parent, Anchor.LEFT, offset, false);
    if (this.m_currencyFrames.Count > 1)
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        TransformUtil.SetLocalPosY(this.m_currencyFrameContainer, 189.703f);
      else
        TransformUtil.SetLocalPosY(this.m_currencyFrameContainer, -2.850989f);
    }
    gameObjectList.ForEach((System.Action<GameObject>) (obj => obj.SetActive(true)));
  }

  public bool HandleKeyboardInput()
  {
    if (InputCollection.GetKeyUp(BackButton.backKey) || InputCollection.GetKeyUp(KeyCode.Escape))
      return this.HandleEscapeKey();
    ChatMgr chatMgr = ChatMgr.Get();
    return (UnityEngine.Object) chatMgr != (UnityEngine.Object) null && chatMgr.HandleKeyboardInput();
  }

  public void ToggleGameMenu()
  {
    if (this.m_gameMenu == null)
      this.LoadGameMenu();
    else if (this.m_gameMenu.GameMenuIsShown())
    {
      this.HideGameMenu();
    }
    else
    {
      this.m_gameMenu.GameMenuShow();
      if (this.OnMenuOpened == null)
        return;
      this.OnMenuOpened();
    }
  }

  public bool IsActive() => this.gameObject.activeSelf;

  public void ToggleActive(bool active)
  {
    this.gameObject.SetActive(active);
    if (!active)
      return;
    this.UpdateLayout();
  }

  public void PermanentlyDisableButtons()
  {
    this.m_buttonsDisabledPermanently = true;
    this.UpdateButtonEnableState();
  }

  public void ForceEnableButtons()
  {
    this.m_buttonsDisabledPermanently = false;
    this.m_buttonsDisabledByDialog.Clear();
    this.m_buttonsDisabledByRefCount = 0;
    this.UpdateButtonEnableState();
  }

  public void DisableButtonsByDialog(DialogBase dialog)
  {
    dialog.AddHiddenOrDestroyedListener(new DialogBase.HideCallback(this.OnDisablingDialogHiddenOrDestroyed));
    this.m_buttonsDisabledByDialog.Add(dialog);
    this.UpdateButtonEnableState();
  }

  public void RequestDisableButtons()
  {
    ++this.m_buttonsDisabledByRefCount;
    this.UpdateButtonEnableState();
  }

  public void CancelRequestToDisableButtons()
  {
    --this.m_buttonsDisabledByRefCount;
    this.UpdateButtonEnableState();
  }

  private void OnDisablingDialogHiddenOrDestroyed(DialogBase dialog, object userData)
  {
    this.m_buttonsDisabledByDialog.Remove(dialog);
    this.UpdateButtonEnableState();
  }

  public bool AreButtonsEnabled() => this.m_buttonsEnabled;

  public void HideGameMenu()
  {
    if (this.m_gameMenu == null || !this.m_gameMenu.GameMenuIsShown())
      return;
    this.m_gameMenu.GameMenuHide();
  }

  public void HideOptionsMenu()
  {
    if (!((UnityEngine.Object) OptionsMenu.Get() != (UnityEngine.Object) null) || !OptionsMenu.Get().IsShown())
      return;
    OptionsMenu.Get().Hide();
  }

  public void HideMiscellaneousMenu()
  {
    if (!((UnityEngine.Object) MiscellaneousMenu.Get() != (UnityEngine.Object) null) || !MiscellaneousMenu.Get().IsShown())
      return;
    MiscellaneousMenu.Get().Hide();
  }

  public bool IsGameMenuShown() => this.m_gameMenu != null && this.m_gameMenu.GameMenuIsShown();

  public void UpdateForPhone()
  {
    int num;
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.LOGIN:
      case SceneMgr.Mode.HUB:
      case SceneMgr.Mode.GAMEPLAY:
      case SceneMgr.Mode.LETTUCE_VILLAGE:
        num = 1;
        break;
      default:
        num = this.IsCurrencyFrameActive() ? 1 : 0;
        break;
    }
    this.m_menuButton.gameObject.SetActive(num != 0);
  }

  public void UpdateLoginTooltip()
  {
    if ((Network.ShouldBeConnectedToAurora() || this.m_suppressLoginTooltip || !SceneMgr.Get().IsInGame() || !GameMgr.Get().IsTraditionalTutorial() || GameMgr.Get().IsSpectator() ? 0 : (DemoMgr.Get().GetMode() != DemoMode.BLIZZ_MUSEUM ? 1 : 0)) != 0)
    {
      if ((UnityEngine.Object) this.m_loginTooltip == (UnityEngine.Object) null)
      {
        this.m_loginTooltip = AssetLoader.Get().InstantiatePrefab((AssetReference) "LoginPointer.prefab:e26056ee6e4b89c45899d54bc9497bb0");
        this.m_loginTooltip.transform.localScale = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(40f, 40f, 40f) : new Vector3(60f, 60f, 60f);
        TransformUtil.SetEulerAngleX(this.m_loginTooltip, 270f);
        LayerUtils.SetLayer(this.m_loginTooltip, GameLayer.BattleNet);
        this.m_loginTooltip.transform.parent = this.transform;
      }
      if ((bool) UniversalInputManager.UsePhoneUI)
        TransformUtil.SetPoint(this.m_loginTooltip, Anchor.RIGHT, this.m_batteryLevel.gameObject, Anchor.LEFT, new Vector3(-32f, 0.0f, 0.0f));
      else
        TransformUtil.SetPoint(this.m_loginTooltip, Anchor.RIGHT, (Component) this.m_menuButton, Anchor.LEFT, new Vector3(-80f, 0.0f, 0.0f));
    }
    else
      this.DestroyLoginTooltip();
  }

  private void DestroyLoginTooltip()
  {
    if (!((UnityEngine.Object) this.m_loginTooltip != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_loginTooltip);
    this.m_loginTooltip = (GameObject) null;
  }

  public void SuppressLoginTooltip(bool val)
  {
    this.m_suppressLoginTooltip = val;
    this.UpdateLayout();
  }

  private void ShowFriendList()
  {
    ChatMgr.Get().ShowFriendsList();
    this.m_hasUnacknowledgedPendingInvites = false;
    this.m_friendButton.ShowPendingInvitesIcon(this.m_hasUnacknowledgedPendingInvites);
  }

  public void HideFriendList() => ChatMgr.Get()?.CloseChatUI();

  private void OnFriendButtonReleased(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    this.ToggleFriendListShowing();
    this.UpdateLayout();
  }

  private void ToggleFriendListShowing()
  {
    if (ChatMgr.Get().IsFriendListShowing())
      this.HideFriendList();
    else
      this.ShowFriendList();
    this.m_friendButton.HideTooltip();
  }

  private void UpdateButtonEnableState()
  {
    if (this.m_buttonsDisabledPermanently || this.m_buttonsDisabledByRefCount > 0 || this.m_buttonsDisabledByDialog.Any<DialogBase>())
    {
      this.m_buttonsEnabled = false;
      this.m_menuButton.SetEnabled(false, false);
      this.m_friendButton.SetEnabled(false, false);
      this.SetBlockCurrencyFrames(true);
      this.HideMiscellaneousMenu();
      this.HideOptionsMenu();
      this.HideGameMenu();
      this.HideFriendList();
    }
    else
    {
      this.m_buttonsEnabled = true;
      this.m_menuButton.SetEnabled(true, false);
      this.m_friendButton.SetEnabled(true, false);
      this.SetBlockCurrencyFrames(false);
    }
  }

  private void WillReset()
  {
    if (this.m_gameMenu != null)
    {
      if (this.m_gameMenu.GameMenuIsShown())
        this.m_gameMenu.GameMenuHide();
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_gameMenu.GameMenuGetGameObject());
      this.m_gameMenu = (GameMenuInterface) null;
    }
    this.DestroyLoginTooltip();
    this.ToggleActive(false);
    this.m_isLoggedIn = false;
  }

  private bool HandleEscapeKey()
  {
    if (this.m_gameMenu != null && this.m_gameMenu.GameMenuIsShown())
    {
      this.m_gameMenu.GameMenuHide();
      return true;
    }
    if ((UnityEngine.Object) OptionsMenu.Get() != (UnityEngine.Object) null && OptionsMenu.Get().IsShown())
    {
      OptionsMenu.Get().Hide();
      return true;
    }
    if ((UnityEngine.Object) MiscellaneousMenu.Get() != (UnityEngine.Object) null && MiscellaneousMenu.Get().IsShown())
    {
      MiscellaneousMenu.Get().Hide();
      return true;
    }
    if ((UnityEngine.Object) QuestLog.Get() != (UnityEngine.Object) null && QuestLog.Get().IsShown())
    {
      QuestLog.Get().Hide();
      return true;
    }
    if ((UnityEngine.Object) GeneralStore.Get() != (UnityEngine.Object) null && GeneralStore.Get().IsShown())
    {
      GeneralStore.Get().Close();
      return true;
    }
    ChatMgr chatMgr = ChatMgr.Get();
    if ((UnityEngine.Object) chatMgr != (UnityEngine.Object) null && chatMgr.HandleKeyboardInput())
      return true;
    if ((UnityEngine.Object) CraftingTray.Get() != (UnityEngine.Object) null && CraftingTray.Get().IsShown())
    {
      CraftingTray.Get().Hide(true);
      return true;
    }
    if ((UnityEngine.Object) PrivacyMenu.Get() != (UnityEngine.Object) null && PrivacyMenu.Get().IsShown())
    {
      PrivacyMenu.Get().Hide();
      if ((UnityEngine.Object) OptionsMenu.Get() != (UnityEngine.Object) null)
        OptionsMenu.Get().Show();
      return true;
    }
    if ((UnityEngine.Object) PrivacySettingsMenu.Get() != (UnityEngine.Object) null && PrivacySettingsMenu.Get().IsShown())
    {
      PrivacySettingsMenu.Get().Hide();
      if ((UnityEngine.Object) PrivacyMenu.Get() != (UnityEngine.Object) null)
        PrivacyMenu.Get().Show();
      return true;
    }
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    switch (mode)
    {
      case SceneMgr.Mode.STARTUP:
        return true;
      case SceneMgr.Mode.LOGIN:
        return true;
      case SceneMgr.Mode.FATAL_ERROR:
        return true;
      default:
        if (!DemoMgr.Get().IsHubEscMenuEnabled(mode == SceneMgr.Mode.GAMEPLAY))
          return true;
        this.ToggleGameMenu();
        return true;
    }
  }

  private void OnMenuButtonReleased(UIEvent e)
  {
    if (!GameMgr.Get().IsSpectator() && GameState.Get() != null && GameState.Get().IsInTargetMode())
      return;
    this.ToggleGameMenu();
  }

  private void LoadGameMenu()
  {
    if (this.m_gameMenuLoading || this.m_gameMenu != null)
      return;
    this.m_gameMenuLoading = true;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "GameMenu.prefab:dc76cbcfb64a34d7e93755df33db2f80", new PrefabCallback<GameObject>(this.ShowGameMenu));
  }

  private void ShowGameMenu(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_gameMenu = (GameMenuInterface) go.GetComponent<GameMenu>();
    this.m_gameMenu.GameMenuShow();
    if (this.OnMenuOpened != null)
      this.OnMenuOpened();
    this.m_gameMenuLoading = false;
  }

  private void UpdateForDemoMode()
  {
    if (!DemoMgr.Get().IsExpoDemo())
      return;
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    bool flag1 = true;
    bool flag2;
    switch (DemoMgr.Get().GetMode())
    {
      case DemoMode.PAX_EAST_2013:
      case DemoMode.BLIZZCON_2013:
      case DemoMode.BLIZZCON_2015:
      case DemoMode.BLIZZCON_2017_ADVENTURE:
      case DemoMode.BLIZZCON_2017_BRAWL:
        flag2 = mode == SceneMgr.Mode.GAMEPLAY;
        flag1 = false;
        this.m_currencyFrameContainer.SetActive(false);
        break;
      case DemoMode.BLIZZCON_2014:
        flag1 = flag2 = mode != SceneMgr.Mode.FRIENDLY;
        break;
      case DemoMode.BLIZZ_MUSEUM:
        flag2 = flag1 = false;
        break;
      case DemoMode.ANNOUNCEMENT_5_0:
        flag1 = true;
        flag2 = true;
        break;
      case DemoMode.BLIZZCON_2016:
      case DemoMode.BLIZZCON_2018_BRAWL:
      case DemoMode.BLIZZCON_2019_BATTLEGROUNDS:
        flag2 = mode == SceneMgr.Mode.GAMEPLAY;
        flag1 = mode == SceneMgr.Mode.HUB;
        break;
      default:
        flag2 = mode != SceneMgr.Mode.FRIENDLY && mode != SceneMgr.Mode.TOURNAMENT;
        break;
    }
    switch (mode)
    {
      case SceneMgr.Mode.GAMEPLAY:
      case SceneMgr.Mode.TOURNAMENT:
      case SceneMgr.Mode.FRIENDLY:
        if (DemoMgr.Get().GetMode() != DemoMode.ANNOUNCEMENT_5_0)
        {
          flag1 = false;
          break;
        }
        break;
    }
    if (!flag2)
      this.m_menuButton.gameObject.SetActive(false);
    if (flag1)
      return;
    this.m_friendButton.gameObject.SetActive(false);
  }

  private void UpdateForTutorialPreviewVideos(SceneMgr.Mode mode)
  {
    if (mode == SceneMgr.Mode.HUB && !GameUtils.IsAnyTutorialComplete())
    {
      this.m_friendButton.gameObject.SetActive(false);
      this.m_currencyFrameContainer.SetActive(false);
      this.m_currentTime.gameObject.SetActive(false);
    }
    else
    {
      this.m_currencyFrameContainer.SetActive(true);
      this.m_currentTime.gameObject.SetActive(true);
      this.m_friendButton.gameObject.SetActive(Network.ShouldBeConnectedToAurora());
    }
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode == SceneMgr.Mode.FATAL_ERROR)
      return;
    this.m_suppressLoginTooltip = false;
    this.RefreshCurrency();
    int num = mode == SceneMgr.Mode.INVALID ? 0 : (mode != SceneMgr.Mode.FATAL_ERROR ? 1 : 0);
    if (num != 0)
    {
      if (SpectatorManager.Get().IsInSpectatorMode())
        this.SpectatorManager_OnSpectatorModeChanged(OnlineEventType.ADDED, (BnetPlayer) null);
    }
    else if ((UnityEngine.Object) this.m_spectatorModeIndicator != (UnityEngine.Object) null && this.m_spectatorModeIndicator.activeSelf)
      this.m_spectatorModeIndicator.SetActive(false);
    if (num != 0 && (UnityEngine.Object) this.m_spectatorCountPanel != (UnityEngine.Object) null)
    {
      bool flag = SpectatorManager.Get().IsBeingSpectated();
      if ((bool) UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
        flag = false;
      this.m_spectatorCountPanel.SetActive(flag);
    }
    this.UpdateForTutorialPreviewVideos(mode);
    this.UpdateForDemoMode();
    this.UpdateLayout();
  }

  private void SpectatorManager_OnInviteReceived(OnlineEventType evt, BnetPlayer inviter)
  {
    this.m_hasUnacknowledgedPendingInvites = !ChatMgr.Get().IsFriendListShowing() && SpectatorManager.Get().HasAnyReceivedInvites() && (this.m_hasUnacknowledgedPendingInvites || evt == OnlineEventType.ADDED);
    if (!((UnityEngine.Object) this.m_friendButton != (UnityEngine.Object) null))
      return;
    this.m_friendButton.ShowPendingInvitesIcon(this.m_hasUnacknowledgedPendingInvites);
  }

  private void SpectatorManager_OnSpectatorToMyGame(OnlineEventType evt, BnetPlayer spectator)
  {
    int countSpectatingMe = SpectatorManager.Get().GetCountSpectatingMe();
    if (countSpectatingMe <= 0)
    {
      if ((UnityEngine.Object) this.m_spectatorCountPanel == (UnityEngine.Object) null)
        return;
    }
    else if ((UnityEngine.Object) this.m_spectatorCountPanel == (UnityEngine.Object) null)
    {
      string spectatorCountPrefabPath = this.m_spectatorCountPrefabPath;
      AssetLoader.Get().InstantiatePrefab((AssetReference) spectatorCountPrefabPath, (PrefabCallback<GameObject>) ((n, go, d) =>
      {
        BnetBar bnetBar = BnetBar.Get();
        if ((UnityEngine.Object) bnetBar == (UnityEngine.Object) null)
          return;
        if ((UnityEngine.Object) bnetBar.m_spectatorCountPanel != (UnityEngine.Object) null)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) go);
        }
        else
        {
          bnetBar.m_spectatorCountPanel = go;
          bnetBar.m_spectatorCountPanel.transform.parent = bnetBar.transform;
          bnetBar.m_spectatorCountPanel.transform.localEulerAngles = Vector3.zero;
          TransformOverride component1 = bnetBar.m_spectatorCountPanel.GetComponent<TransformOverride>();
          if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          {
            int bestScreenMatch = PlatformSettings.GetBestScreenMatch(component1.m_screenCategory);
            this.m_initialSpectatorCountScaleX = component1.m_localScale[bestScreenMatch].x;
          }
          PegUIElement component2 = go.GetComponent<PegUIElement>();
          if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
          {
            component2.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(BnetBar.SpectatorCount_OnRollover));
            component2.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(BnetBar.SpectatorCount_OnRollout));
          }
          Material material = RendererExtension.GetMaterial(bnetBar.m_spectatorCountPanel.transform.Find("BeingWatchedHighlight").gameObject.GetComponent<Renderer>());
          material.color = material.color with { a = 0.0f };
        }
        BnetBar.Get().SpectatorManager_OnSpectatorToMyGame(evt, spectator);
      }));
      return;
    }
    this.m_spectatorCountPanel.transform.Find("UberText").GetComponent<UberText>().Text = countSpectatingMe.ToString();
    bool flag = countSpectatingMe > 0;
    if ((bool) UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
      flag = false;
    this.m_spectatorCountPanel.SetActive(flag);
    this.UpdateLayout();
    GameObject gameObject = this.m_spectatorCountPanel.transform.Find("BeingWatchedHighlight").gameObject;
    iTween.Stop(gameObject, true);
    Hashtable args = iTween.Hash((object) "alpha", (object) 1f, (object) "time", (object) 0.5f, (object) "oncomplete", (object) (System.Action<object>) (ud =>
    {
      if ((UnityEngine.Object) BnetBar.Get() == (UnityEngine.Object) null)
        return;
      iTween.FadeTo(BnetBar.Get().m_spectatorCountPanel.transform.Find("BeingWatchedHighlight").gameObject, 0.0f, 0.5f);
    }));
    iTween.FadeTo(gameObject, args);
  }

  private static void SpectatorCount_OnRollover(UIEvent evt)
  {
    BnetBar bnetBar = BnetBar.Get();
    if ((UnityEngine.Object) bnetBar == (UnityEngine.Object) null)
      return;
    string headline = GameStrings.Get("GLOBAL_SPECTATOR_COUNT_PANEL_HEADER");
    BnetGameAccountId[] spectatorPartyMembers = SpectatorManager.Get().GetSpectatorPartyMembers();
    string bodytext;
    if (spectatorPartyMembers.Length == 1)
      bodytext = GameStrings.Format("GLOBAL_SPECTATOR_COUNT_PANEL_TEXT_ONE", (object) BnetUtils.GetPlayerBestName(spectatorPartyMembers[0]));
    else
      bodytext = string.Join(", ", ((IEnumerable<BnetGameAccountId>) spectatorPartyMembers).Select<BnetGameAccountId, string>((Func<BnetGameAccountId, string>) (id => BnetUtils.GetPlayerBestName(id))).ToArray<string>());
    bnetBar.m_spectatorCountTooltipZone.ShowSocialTooltip(bnetBar.m_spectatorCountPanel, headline, bodytext, 18.75f, GameLayer.BattleNetDialog);
    bnetBar.m_spectatorCountTooltipZone.AnchorTooltipTo(bnetBar.m_spectatorCountPanel, Anchor.TOP_LEFT_XZ, Anchor.BOTTOM_LEFT_XZ);
  }

  private static void SpectatorCount_OnRollout(UIEvent evt)
  {
    BnetBar bnetBar = BnetBar.Get();
    if ((UnityEngine.Object) bnetBar == (UnityEngine.Object) null)
      return;
    bnetBar.m_spectatorCountTooltipZone.HideTooltip();
  }

  private bool ShouldShowSpectatorModeIndicator
  {
    get
    {
      bool flag1 = false;
      bool flag2 = SpectatorManager.Get().IsInSpectatorMode();
      bool spectatorModeIndicator = (0 | (flag1 ? 1 : 0) | (flag2 ? 1 : 0)) != 0;
      if ((bool) UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
        spectatorModeIndicator = false;
      if (SpectatorManager.Get().IsBeingSpectated())
        spectatorModeIndicator = false;
      return spectatorModeIndicator;
    }
  }

  private void ShowSpectatorModeIndicator(bool show)
  {
    if ((UnityEngine.Object) this.m_spectatorModeIndicator != (UnityEngine.Object) null)
      this.m_spectatorModeIndicator.SetActive(show);
    if (!show)
      return;
    UberText componentInChildren = this.m_spectatorModeIndicator.GetComponentInChildren<UberText>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null) || !SpectatorManager.Get().IsInSpectatorMode())
      return;
    componentInChildren.Text = GameStrings.Get("GLOBAL_SPECTATOR_MODE_INDICATOR_TEXT");
  }

  private void CheckSpectatorModeIndicator()
  {
    if (this.ShouldShowSpectatorModeIndicator && (UnityEngine.Object) this.m_spectatorModeIndicator == (UnityEngine.Object) null)
    {
      string modeIndicatorPrefab = this.m_spectatorModeIndicatorPrefab;
      AssetLoader.Get().InstantiatePrefab((AssetReference) modeIndicatorPrefab, (PrefabCallback<GameObject>) ((n, go, d) =>
      {
        BnetBar bnetBar = BnetBar.Get();
        if ((UnityEngine.Object) bnetBar == (UnityEngine.Object) null || (UnityEngine.Object) go == (UnityEngine.Object) null)
          return;
        if ((UnityEngine.Object) bnetBar.m_spectatorModeIndicator != (UnityEngine.Object) null)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) go);
        }
        else
        {
          bnetBar.m_spectatorModeIndicator = go;
          bnetBar.m_spectatorModeIndicator.transform.parent = bnetBar.transform;
          TransformOverride component = go.GetComponent<TransformOverride>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            int bestScreenMatch = PlatformSettings.GetBestScreenMatch(component.m_screenCategory);
            this.m_initialSpectatorModeIndicatorScaleX = component.m_localScale[bestScreenMatch].x;
          }
        }
        BnetBar.Get().CheckSpectatorModeIndicator();
      }));
    }
    else
    {
      if ((UnityEngine.Object) this.m_spectatorModeIndicator == (UnityEngine.Object) null)
        return;
      this.UpdateLayout();
    }
  }

  private void SpectatorManager_OnSpectatorModeChanged(OnlineEventType evt, BnetPlayer spectatee) => this.CheckSpectatorModeIndicator();

  private void OnRequestGetServerTimeResponse() => this.m_serverClientOffsetInSec = (double) (Network.Get().GetServerTimeResponse().Response.ServerUnixTime - (long) TimeUtils.DateTimeToUnixTimeStamp(DateTime.Now));
}
