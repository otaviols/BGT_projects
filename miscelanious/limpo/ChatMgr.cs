using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChatMgr : MonoBehaviour
{
  public ChatMgrPrefabs m_Prefabs;
  public ChatMgrBubbleInfo m_ChatBubbleInfo;
  public Float_MobileOverride m_friendsListXOffset;
  public Float_MobileOverride m_friendsListYOffset;
  public Float_MobileOverride m_friendsListWidthPadding;
  public Float_MobileOverride m_friendsListHeightPadding;
  public float m_chatLogXOffset;
  public Float_MobileOverride m_friendsListWidth;
  private static ChatMgr s_instance;
  private List<ChatBubbleFrame> m_chatBubbleFrames = new List<ChatBubbleFrame>();
  private IChatLogUI m_chatLogUI;
  private FriendListFrame m_friendListFrame;
  private PegUIElement m_closeCatcher;
  private List<BnetPlayer> m_recentWhisperPlayers = new List<BnetPlayer>();
  private Map<BnetAccountId, string> m_pendingChatMessages = new Map<BnetAccountId, string>();
  private bool m_chatLogFrameShown;
  private bool m_isChatFeatureEnabled;
  private PrivacyFeaturesPopup m_chatPrivacyPopup;
  private Map<BnetPlayer, PlayerChatInfo> m_playerChatInfos = new Map<BnetPlayer, PlayerChatInfo>();
  private List<ChatMgr.PlayerChatInfoChangedListener> m_playerChatInfoChangedListeners = new List<ChatMgr.PlayerChatInfoChangedListener>();
  private ChatMgr.KeyboardState keyboardState;
  private Rect keyboardArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
  private FatalErrorMgr m_fatalErrorMgr;
  private Map<Renderer, int> m_friendListOriginalLayers = new Map<Renderer, int>();

  public event ChatMgr.FriendListToggled OnFriendListToggled;

  public event System.Action OnChatLogShown;

  public static event System.Action OnStarted;

  public FriendListFrame FriendListFrame => this.m_friendListFrame;

  public Rect KeyboardRect => this.keyboardArea;

  private void Awake()
  {
    ChatMgr.s_instance = this;
    this.m_fatalErrorMgr = FatalErrorMgr.Get();
    BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    this.m_fatalErrorMgr.AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    touchScreenService.AddOnVirtualKeyboardShowListener(new System.Action(this.OnKeyboardShow));
    touchScreenService.AddOnVirtualKeyboardHideListener(new System.Action(this.OnKeyboardHide));
    HearthstoneApplication.Get().WillReset += new System.Action(this.WillReset);
    this.InitCloseCatcher();
    this.InitChatLogUI();
  }

  private void OnDestroy()
  {
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.WillReset -= new System.Action(this.WillReset);
    ITouchScreenService service;
    if (ServiceManager.TryGet<ITouchScreenService>(out service))
    {
      service.RemoveOnVirtualKeyboardShowListener(new System.Action(this.OnKeyboardShow));
      service.RemoveOnVirtualKeyboardHideListener(new System.Action(this.OnKeyboardHide));
    }
    this.OnChatLogShown = (System.Action) null;
    ChatMgr.s_instance = (ChatMgr) null;
    this.m_fatalErrorMgr.RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
  }

  private void Start()
  {
    SoundManager.Get().Load((AssetReference) "receive_message.prefab:8e90a827cd4a0e849953158396cd1ee1");
    this.UpdateLayout();
    if (ServiceManager.Get<ITouchScreenService>().IsVirtualKeyboardVisible())
      this.OnKeyboardShow();
    if (ChatMgr.OnStarted == null)
      return;
    ChatMgr.OnStarted();
  }

  private void Update()
  {
    Rect keyboardArea = this.keyboardArea;
    this.keyboardArea = TextField.KeyboardArea;
    if (!(this.keyboardArea != keyboardArea))
      return;
    this.UpdateLayout();
  }

  public static ChatMgr Get() => ChatMgr.s_instance;

  private void WillReset() => this.CleanUp();

  private void DisplayAlertPopup(string message)
  {
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER"),
      m_text = message,
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
    Log.Privacy.PrintWarning("Chat is disabled. Will not show Friends List");
  }

  private ChatMgr.KeyboardState ComputeKeyboardState()
  {
    if ((double) this.keyboardArea.height <= 0.0)
      return ChatMgr.KeyboardState.None;
    return (double) this.keyboardArea.y <= (double) ((float) Screen.height - this.keyboardArea.yMax) ? ChatMgr.KeyboardState.Above : ChatMgr.KeyboardState.Below;
  }

  private void InitCloseCatcher()
  {
    this.m_closeCatcher = CameraUtils.CreateInputBlocker(BaseUI.Get().GetBnetCamera(), "CloseCatcher", (Component) this).AddComponent<PegUIElement>();
    this.m_closeCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCloseCatcherRelease));
    this.m_closeCatcher.gameObject.SetActive(false);
  }

  private void InitChatLogUI()
  {
    if (this.IsMobilePlatform())
      this.m_chatLogUI = (IChatLogUI) new MobileChatLogUI();
    else
      this.m_chatLogUI = (IChatLogUI) new DesktopChatLogUI();
  }

  private FriendListFrame CreateFriendsListUI()
  {
    string assetRef = (bool) UniversalInputManager.UsePhoneUI ? "FriendListFrame_phone.prefab:91e737585d7bfd2449b46fbecb87ded7" : "FriendListFrame.prefab:cdf3b7f04b5ed45cb8ba0160d43a5bf6";
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return (FriendListFrame) null;
    gameObject.transform.parent = this.transform;
    return gameObject.GetComponent<FriendListFrame>();
  }

  public void SetChatFeatureStatus(bool isEnabled) => this.m_isChatFeatureEnabled = isEnabled;

  public void UpdateLayout()
  {
    if ((UnityEngine.Object) this.m_friendListFrame != (UnityEngine.Object) null || this.m_chatLogUI.IsShowing)
      this.UpdateLayoutForOnScreenKeyboard();
    this.UpdateChatBubbleParentLayout();
  }

  private void UpdateLayoutForOnScreenKeyboard()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.UpdateLayoutForOnScreenKeyboardOnPhone();
    }
    else
    {
      this.keyboardState = this.ComputeKeyboardState();
      bool flag = this.IsMobilePlatform();
      if (TemporaryAccountManager.IsTemporaryAccount())
        flag = false;
      Camera bnetCamera = BaseUI.Get().GetBnetCamera();
      float num1 = bnetCamera.orthographicSize * 2f;
      float num2 = num1 * bnetCamera.aspect;
      float num3 = bnetCamera.transform.position.z + num1 / 2f;
      float num4 = bnetCamera.transform.position.x - num2 / 2f;
      float num5 = 0.0f;
      if (this.keyboardState != 0 & flag)
        num5 = num1 * this.keyboardArea.height / (float) Screen.height;
      float num6 = 0.0f;
      if ((UnityEngine.Object) this.m_friendListFrame != (UnityEngine.Object) null)
      {
        OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds(BaseUI.Get().m_BnetBar.m_friendButton.gameObject);
        if (flag)
        {
          float num7 = this.keyboardState == ChatMgr.KeyboardState.Below ? num5 : orientedWorldBounds.Extents[1].z * 2f;
          this.m_friendListFrame.SetWorldHeight(num1 - num7);
        }
        OrientedBounds frameWorldBounds = this.m_friendListFrame.ComputeFrameWorldBounds();
        if (frameWorldBounds != null)
        {
          if (!flag || this.keyboardState != ChatMgr.KeyboardState.Below)
            this.m_friendListFrame.SetWorldPosition(num4 + frameWorldBounds.Extents[0].x + frameWorldBounds.CenterOffset.x + (float) (MobileOverrideValue<float>) this.m_friendsListXOffset, orientedWorldBounds.GetTrueCenterPosition().z + orientedWorldBounds.Extents[1].z + frameWorldBounds.Extents[1].z + frameWorldBounds.CenterOffset.z);
          else if (flag && this.keyboardState == ChatMgr.KeyboardState.Below)
            this.m_friendListFrame.SetWorldPosition(num4 + frameWorldBounds.Extents[0].x + frameWorldBounds.CenterOffset.x + (float) (MobileOverrideValue<float>) this.m_friendsListXOffset, bnetCamera.transform.position.z - num1 / 2f + num5 + frameWorldBounds.Extents[1].z + frameWorldBounds.CenterOffset.z);
          num6 = frameWorldBounds.Extents[0].magnitude * 2f;
        }
      }
      if (this.m_chatLogUI.IsShowing)
      {
        ChatFrames component = this.m_chatLogUI.GameObject.GetComponent<ChatFrames>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          float z = num3;
          if (this.keyboardState == ChatMgr.KeyboardState.Above)
            z -= num5;
          float height = num1 - num5;
          if (this.keyboardState == ChatMgr.KeyboardState.None & flag)
          {
            OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds(BaseUI.Get().m_BnetBar.m_friendButton.gameObject);
            height -= orientedWorldBounds.Extents[1].z * 2f;
          }
          float x = num4;
          if (!(bool) UniversalInputManager.UsePhoneUI)
            x += num6 + (float) (MobileOverrideValue<float>) this.m_friendsListXOffset + this.m_chatLogXOffset;
          float width = num2;
          if (!(bool) UniversalInputManager.UsePhoneUI)
            width -= num6 + (float) (MobileOverrideValue<float>) this.m_friendsListXOffset + this.m_chatLogXOffset;
          component.chatLogFrame.SetWorldRect(x, z, width, height);
        }
      }
      this.OnChatFramesMoved();
    }
  }

  private void UpdateLayoutForOnScreenKeyboardOnPhone()
  {
    this.keyboardState = this.ComputeKeyboardState();
    bool flag = UniversalInputManager.Get().IsTouchMode();
    float horizontalMargin = BnetBar.Get().HorizontalMargin;
    Camera bnetCamera = BaseUI.Get().GetBnetCamera();
    float num1 = bnetCamera.orthographicSize * 2f;
    float num2 = (float) ((double) num1 * (double) bnetCamera.aspect - (double) horizontalMargin / 2.0);
    float num3 = bnetCamera.transform.position.z + num1 / 2f;
    float num4 = bnetCamera.transform.position.x - num2 / 2f;
    float num5 = 0.0f;
    float num6 = 0.0f;
    float num7 = 0.0f;
    if (this.keyboardState != 0 & flag)
    {
      num5 = num1 * this.keyboardArea.height / (float) Screen.height;
      num6 = num2 * this.keyboardArea.width / (float) Screen.width;
      num7 = num2 * this.keyboardArea.xMin / (float) Screen.width;
    }
    if ((UnityEngine.Object) this.m_friendListFrame != (UnityEngine.Object) null)
      this.m_friendListFrame.SetWorldRect(num4 + (float) (MobileOverrideValue<float>) this.m_friendsListXOffset, num3 + (float) (MobileOverrideValue<float>) this.m_friendsListYOffset, (float) (MobileOverrideValue<float>) this.m_friendsListWidth + (float) (MobileOverrideValue<float>) this.m_friendsListWidthPadding, num1 + (float) (MobileOverrideValue<float>) this.m_friendsListHeightPadding);
    if (this.m_chatLogUI.IsShowing)
    {
      ChatFrames component = this.m_chatLogUI.GameObject.GetComponent<ChatFrames>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        float z = num3;
        if (this.keyboardState == ChatMgr.KeyboardState.Above)
          z -= num5;
        float height = num1 - num5;
        float x = num4 + num7;
        if (!(bool) UniversalInputManager.UsePhoneUI)
          x += (float) (MobileOverrideValue<float>) this.m_friendsListWidth;
        float width = (double) num6 == 0.0 ? num2 : num6;
        if (!(bool) UniversalInputManager.UsePhoneUI)
          width -= (float) (MobileOverrideValue<float>) this.m_friendsListWidth;
        component.chatLogFrame.SetWorldRect(x, z, width, height);
      }
    }
    this.OnChatFramesMoved();
  }

  public bool IsChatLogFrameShown() => this.IsMobilePlatform() ? this.IsChatLogUIShowing() : this.m_chatLogFrameShown;

  public bool IsChatLogUIShowing() => this.m_chatLogUI.IsShowing;

  private void OnCloseCatcherRelease(UIEvent e)
  {
    if (this.m_chatLogUI != null && this.m_chatLogUI.IsShowing)
      this.m_chatLogUI.Hide();
    if ((UnityEngine.Object) this.FriendListFrame != (UnityEngine.Object) null && this.FriendListFrame.IsInEditMode)
      this.FriendListFrame.ExitRemoveFriendsMode();
    else if ((UnityEngine.Object) this.FriendListFrame != (UnityEngine.Object) null && this.FriendListFrame.IsFlyoutOpen)
      this.FriendListFrame.CloseFlyoutMenu();
    else
      this.CloseFriendsList();
  }

  public bool IsFriendListShowing() => !((UnityEngine.Object) this.m_friendListFrame == (UnityEngine.Object) null) && this.m_friendListFrame.gameObject.activeSelf;

  public void ShowFriendsList()
  {
    if (SetRotationManager.Get() != null && SetRotationManager.Get().CheckForSetRotationRollover() || PlayerMigrationManager.Get() != null && PlayerMigrationManager.Get().CheckForPlayerMigrationRequired())
      return;
    if ((UnityEngine.Object) this.m_friendListFrame == (UnityEngine.Object) null)
      this.m_friendListFrame = this.CreateFriendsListUI();
    this.m_friendListFrame.gameObject.SetActive(true);
    this.m_closeCatcher.gameObject.SetActive(true);
    this.UpdateLayout();
    TransformUtil.SetPosY((Component) this.m_closeCatcher, this.m_friendListFrame.transform.position.y - 100f);
    this.m_friendListFrame.UpdateFriendItems();
    ChatMgr.Get().FriendListFrame.items.RecalculateItemSizeAndOffsets(true);
    if (this.OnFriendListToggled == null)
      return;
    this.OnFriendListToggled(true);
  }

  private void HideFriendsList()
  {
    if (FiresideGatheringManager.Get() != null)
      FiresideGatheringManager.Get().m_activeFSGMenu = -1L;
    if (this.IsFriendListShowing())
      this.m_friendListFrame.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_closeCatcher != (UnityEngine.Object) null)
      this.m_closeCatcher.gameObject.SetActive(false);
    if (this.OnFriendListToggled == null)
      return;
    this.OnFriendListToggled(false);
  }

  public void CloseFriendsList() => this.DestroyFriendListFrame();

  public void GoBack()
  {
    if (this.IsFriendListShowing())
    {
      this.CloseChatUI();
    }
    else
    {
      if (!this.m_chatLogUI.IsShowing)
        return;
      this.m_chatLogUI.Hide();
      this.ShowFriendsList();
    }
  }

  public void CloseChatUI(bool closeFriendList = true)
  {
    if (this.m_chatLogUI.IsShowing)
      this.m_chatLogUI.Hide();
    if (!closeFriendList)
      return;
    this.CloseFriendsList();
  }

  public void CleanUp() => this.DestroyFriendListFrame();

  private void DestroyFriendListFrame()
  {
    this.HideFriendsList();
    if ((UnityEngine.Object) this.m_friendListFrame == (UnityEngine.Object) null)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_friendListFrame.gameObject);
    this.m_friendListFrame = (FriendListFrame) null;
  }

  public void SetPendingMessage(BnetAccountId playerID, string message) => this.m_pendingChatMessages[playerID] = message;

  public string GetPendingMessage(BnetAccountId playerID)
  {
    string pendingMessage = "";
    this.m_pendingChatMessages.TryGetValue(playerID, out pendingMessage);
    return pendingMessage;
  }

  public List<BnetPlayer> GetRecentWhisperPlayers() => this.m_recentWhisperPlayers;

  public void AddRecentWhisperPlayerToTop(BnetPlayer player)
  {
    int index = this.m_recentWhisperPlayers.FindIndex((Predicate<BnetPlayer>) (currPlayer => currPlayer == player));
    if (index < 0)
    {
      if (this.m_recentWhisperPlayers.Count == 10)
        this.m_recentWhisperPlayers.RemoveAt(this.m_recentWhisperPlayers.Count - 1);
    }
    else
      this.m_recentWhisperPlayers.RemoveAt(index);
    this.m_recentWhisperPlayers.Insert(0, player);
  }

  public void AddRecentWhisperPlayerToBottom(BnetPlayer player)
  {
    if (this.m_recentWhisperPlayers.Contains(player))
      return;
    if (this.m_recentWhisperPlayers.Count == 10)
      this.m_recentWhisperPlayers.RemoveAt(this.m_recentWhisperPlayers.Count - 1);
    this.m_recentWhisperPlayers.Add(player);
  }

  public void AddPlayerChatInfoChangedListener(ChatMgr.PlayerChatInfoChangedCallback callback) => this.AddPlayerChatInfoChangedListener(callback, (object) null);

  public void AddPlayerChatInfoChangedListener(
    ChatMgr.PlayerChatInfoChangedCallback callback,
    object userData)
  {
    ChatMgr.PlayerChatInfoChangedListener infoChangedListener = new ChatMgr.PlayerChatInfoChangedListener();
    infoChangedListener.SetCallback(callback);
    infoChangedListener.SetUserData(userData);
    if (this.m_playerChatInfoChangedListeners.Contains(infoChangedListener))
      return;
    this.m_playerChatInfoChangedListeners.Add(infoChangedListener);
  }

  public bool RemovePlayerChatInfoChangedListener(ChatMgr.PlayerChatInfoChangedCallback callback) => this.RemovePlayerChatInfoChangedListener(callback, (object) null);

  public bool RemovePlayerChatInfoChangedListener(
    ChatMgr.PlayerChatInfoChangedCallback callback,
    object userData)
  {
    ChatMgr.PlayerChatInfoChangedListener infoChangedListener = new ChatMgr.PlayerChatInfoChangedListener();
    infoChangedListener.SetCallback(callback);
    infoChangedListener.SetUserData(userData);
    return this.m_playerChatInfoChangedListeners.Remove(infoChangedListener);
  }

  public PlayerChatInfo GetPlayerChatInfo(BnetPlayer player)
  {
    PlayerChatInfo playerChatInfo = (PlayerChatInfo) null;
    this.m_playerChatInfos.TryGetValue(player, out playerChatInfo);
    return playerChatInfo;
  }

  public PlayerChatInfo RegisterPlayerChatInfo(BnetPlayer player)
  {
    PlayerChatInfo playerChatInfo;
    if (!this.m_playerChatInfos.TryGetValue(player, out playerChatInfo))
    {
      playerChatInfo = new PlayerChatInfo();
      playerChatInfo.SetPlayer(player);
      this.m_playerChatInfos.Add(player, playerChatInfo);
    }
    return playerChatInfo;
  }

  public void UpdateFriendItemsWhenAvailable()
  {
    if (!((UnityEngine.Object) this.m_friendListFrame != (UnityEngine.Object) null))
      return;
    this.m_friendListFrame.UpdateFriendItemsWhenAvailable();
  }

  public void OnFriendListOpened()
  {
    if (ServiceManager.Get<ITouchScreenService>().IsVirtualKeyboardVisible())
      this.OnKeyboardShow();
    else
      this.UpdateChatBubbleParentLayout();
  }

  public void OnFriendListClosed()
  {
    if (ServiceManager.Get<ITouchScreenService>().IsVirtualKeyboardVisible())
      this.OnKeyboardShow();
    else
      this.UpdateChatBubbleParentLayout();
  }

  public void OnFriendListFriendSelected(BnetPlayer friend)
  {
    this.ShowChatForPlayer(friend);
    if (!((UnityEngine.Object) this.m_friendListFrame != (UnityEngine.Object) null))
      return;
    this.m_friendListFrame.SelectFriend(friend);
  }

  public void OnChatLogFrameShown() => this.m_chatLogFrameShown = true;

  public void OnChatLogFrameHidden() => this.m_chatLogFrameShown = false;

  public void OnChatReceiverChanged(BnetPlayer player) => this.UpdatePlayerFocusTime(player);

  public void OnChatFramesMoved() => this.UpdateChatBubbleParentLayout();

  public void OnQuickChatFrameClosed()
  {
    if (!((UnityEngine.Object) this.m_friendListFrame != (UnityEngine.Object) null))
      return;
    this.m_friendListFrame.ClearHighlights();
  }

  public bool HandleKeyboardInput()
  {
    if (this.m_fatalErrorMgr.HasError())
      return false;
    if (InputCollection.GetKeyUp(KeyCode.Escape) && this.m_chatLogUI.IsShowing)
    {
      this.m_chatLogUI.Hide();
      return true;
    }
    if (!this.IsMobilePlatform() || !this.m_chatLogUI.IsShowing || !InputCollection.GetKeyUp(KeyCode.Escape))
      return false;
    this.m_chatLogUI.GoBack();
    return true;
  }

  public void HandleGUIInput()
  {
    if (this.m_fatalErrorMgr.HasError() || this.IsMobilePlatform())
      return;
    this.HandleGUIInputForQuickChat();
  }

  private void OnWhisper(BnetWhisper whisper, object userData)
  {
    if (!this.m_isChatFeatureEnabled)
    {
      Log.Privacy.PrintDebug("Receiving chat messages is not enabled by Privacy settings");
    }
    else
    {
      BnetPlayer theirPlayer = WhisperUtil.GetTheirPlayer(whisper);
      this.AddRecentWhisperPlayerToTop(theirPlayer);
      BnetRecentPlayerMgr.Get().AddRecentPlayer(theirPlayer, BnetRecentPlayerMgr.RecentReason.RECENT_CHATTED);
      PlayerChatInfo chatInfo = this.RegisterPlayerChatInfo(WhisperUtil.GetTheirPlayer(whisper));
      try
      {
        if (this.m_chatLogUI.IsShowing && WhisperUtil.IsSpeakerOrReceiver(this.m_chatLogUI.Receiver, whisper) && this.IsMobilePlatform())
          chatInfo.SetLastSeenWhisper(whisper);
        else
          this.PopupNewChatBubble(whisper);
      }
      finally
      {
        this.FireChatInfoChangedEvent(chatInfo);
      }
    }
  }

  private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
  {
    List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
    if (removedFriends == null)
      return;
    foreach (BnetPlayer bnetPlayer in removedFriends)
    {
      BnetPlayer friend = bnetPlayer;
      int index = this.m_recentWhisperPlayers.FindIndex((Predicate<BnetPlayer>) (player => friend == player));
      if (index >= 0)
        this.m_recentWhisperPlayers.RemoveAt(index);
    }
  }

  private void OnFatalError(FatalErrorMessage message, object userData) => this.CleanUp();

  private void HandleGUIInputForQuickChat()
  {
    if (this.m_chatLogUI == null)
      return;
    if (!this.m_chatLogUI.IsShowing)
    {
      if (!Input.GetKeyDown(KeyCode.Return))
        return;
      this.ShowChatForPlayer(this.GetMostRecentWhisperedPlayer());
    }
    else
    {
      if (!Input.GetKeyUp(KeyCode.Escape))
        return;
      this.m_chatLogUI.Hide();
    }
  }

  public bool IsMobilePlatform() => UniversalInputManager.Get().IsTouchMode() && PlatformSettings.OS != OSCategory.PC;

  private void ShowChatForPlayer(BnetPlayer player)
  {
    if (!this.m_isChatFeatureEnabled)
    {
      if (!((UnityEngine.Object) this.m_chatPrivacyPopup == (UnityEngine.Object) null))
        return;
      this.m_chatPrivacyPopup = AssetLoader.Get().InstantiatePrefab((AssetReference) "PrivacyPopups.prefab:99a8f571a8a35a54e90790c904bc94f8").GetComponent<PrivacyFeaturesPopup>();
      this.m_chatPrivacyPopup.Set(PrivacyFeatures.CHAT, this.m_isChatFeatureEnabled, (System.Action) (() => PrivacyGate.Get().SetFeature(PrivacyFeatures.CHAT, true)), (System.Action) (() => this.OnChatPopupSuccess(player, this.m_chatPrivacyPopup)), (System.Action) (() => this.OnChatPopupCanceled(this.m_chatPrivacyPopup)));
      this.CloseChatUI();
      this.m_chatPrivacyPopup.Show();
    }
    else
      this.OnShowChatForPlayerAllowed(player);
  }

  private void OnChatPopupSuccess(BnetPlayer player, PrivacyFeaturesPopup privacyPopup)
  {
    privacyPopup.Hide();
    this.ShowFriendsList();
    this.OnShowChatForPlayerAllowed(player);
    this.m_chatPrivacyPopup = (PrivacyFeaturesPopup) null;
    UnityEngine.Object.Destroy((UnityEngine.Object) privacyPopup.gameObject, 1f);
  }

  private void OnChatPopupCanceled(PrivacyFeaturesPopup privacyPopup)
  {
    privacyPopup.Hide();
    this.ShowFriendsList();
    this.m_chatPrivacyPopup = (PrivacyFeaturesPopup) null;
    UnityEngine.Object.Destroy((UnityEngine.Object) privacyPopup.gameObject, 1f);
  }

  private void OnShowChatForPlayerAllowed(BnetPlayer player)
  {
    if (player != null)
    {
      this.AddRecentWhisperPlayerToTop(player);
      PlayerChatInfo chatInfo = this.RegisterPlayerChatInfo(player);
      List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(player);
      if (whispersWithPlayer != null)
      {
        chatInfo.SetLastSeenWhisper(whispersWithPlayer.LastOrDefault<BnetWhisper>((Func<BnetWhisper, bool>) (whisper => WhisperUtil.IsSpeaker(player, whisper))));
        this.FireChatInfoChangedEvent(chatInfo);
      }
    }
    if (this.m_chatLogUI.IsShowing)
      this.m_chatLogUI.Hide();
    if ((UnityEngine.Object) this.FriendListFrame != (UnityEngine.Object) null && this.FriendListFrame.IsFlyoutOpen)
      this.FriendListFrame.CloseFlyoutMenu();
    if (this.m_chatLogUI.IsShowing)
      return;
    if ((UnityEngine.Object) OptionsMenu.Get() != (UnityEngine.Object) null && OptionsMenu.Get().IsShown())
      OptionsMenu.Get().Hide();
    if ((UnityEngine.Object) MiscellaneousMenu.Get() != (UnityEngine.Object) null && MiscellaneousMenu.Get().IsShown())
      MiscellaneousMenu.Get().Hide();
    if ((UnityEngine.Object) BnetBar.Get() != (UnityEngine.Object) null)
      BnetBar.Get().HideGameMenu();
    this.m_chatLogUI.ShowForPlayer(this.GetMostRecentWhisperedPlayer());
    this.UpdateLayout();
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.CloseFriendsList();
    if (this.OnChatLogShown == null)
      return;
    this.OnChatLogShown();
  }

  private BnetPlayer GetMostRecentWhisperedPlayer() => this.m_recentWhisperPlayers.Count <= 0 ? (BnetPlayer) null : this.m_recentWhisperPlayers[0];

  private void UpdatePlayerFocusTime(BnetPlayer player)
  {
    PlayerChatInfo chatInfo = this.RegisterPlayerChatInfo(player);
    chatInfo.SetLastFocusTime(Time.realtimeSinceStartup);
    this.FireChatInfoChangedEvent(chatInfo);
  }

  private void FireChatInfoChangedEvent(PlayerChatInfo chatInfo)
  {
    foreach (ChatMgr.PlayerChatInfoChangedListener infoChangedListener in this.m_playerChatInfoChangedListeners.ToArray())
      infoChangedListener.Fire(chatInfo);
  }

  private void UpdateChatBubbleParentLayout()
  {
    if (!((UnityEngine.Object) BaseUI.Get().GetChatBubbleBone() != (UnityEngine.Object) null))
      return;
    this.m_ChatBubbleInfo.m_Parent.transform.position = BaseUI.Get().GetChatBubbleBone().transform.position;
  }

  private void UpdateChatBubbleLayout()
  {
    int count = this.m_chatBubbleFrames.Count;
    if (count == 0)
      return;
    Component dst = (Component) this.m_ChatBubbleInfo.m_Parent;
    for (int index = count - 1; index >= 0; --index)
    {
      ChatBubbleFrame chatBubbleFrame = this.m_chatBubbleFrames[index];
      Anchor dstAnchor = (bool) UniversalInputManager.UsePhoneUI ? Anchor.BOTTOM_LEFT_XZ : Anchor.TOP_LEFT_XZ;
      TransformUtil.SetPoint((Component) chatBubbleFrame, (bool) UniversalInputManager.UsePhoneUI ? Anchor.TOP_LEFT_XZ : Anchor.BOTTOM_LEFT_XZ, dst, dstAnchor, Vector3.zero);
      dst = (Component) chatBubbleFrame;
    }
  }

  private void PopupNewChatBubble(BnetWhisper whisper)
  {
    ChatBubbleFrame chatBubble = this.CreateChatBubble(whisper);
    this.m_chatBubbleFrames.Add(chatBubble);
    this.UpdateChatBubbleParentLayout();
    chatBubble.transform.parent = this.m_ChatBubbleInfo.m_Parent.transform;
    chatBubble.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) chatBubble.m_ScaleOverride;
    SoundManager.Get().LoadAndPlay((AssetReference) "receive_message.prefab:8e90a827cd4a0e849953158396cd1ee1");
    Hashtable args = iTween.Hash((object) "scale", (object) chatBubble.m_VisualRoot.transform.localScale, (object) "time", (object) this.m_ChatBubbleInfo.m_ScaleInSec, (object) "easeType", (object) this.m_ChatBubbleInfo.m_ScaleInEaseType, (object) "oncomplete", (object) "OnChatBubbleScaleInComplete", (object) "oncompleteparams", (object) chatBubble, (object) "oncompletetarget", (object) this.gameObject);
    chatBubble.m_VisualRoot.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
    iTween.ScaleTo(chatBubble.m_VisualRoot, args);
    this.MoveChatBubbles(chatBubble);
  }

  private ChatBubbleFrame CreateChatBubble(BnetWhisper whisper)
  {
    ChatBubbleFrame c = this.InstantiateChatBubble(this.m_Prefabs.m_ChatBubbleOneLineFrame, whisper);
    if (!c.DoesMessageFit())
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) c.gameObject);
      c = this.InstantiateChatBubble(this.m_Prefabs.m_ChatBubbleSmallFrame, whisper);
    }
    LayerUtils.SetLayer((Component) c, GameLayer.BattleNetDialog);
    return c;
  }

  private ChatBubbleFrame InstantiateChatBubble(
    ChatBubbleFrame prefab,
    BnetWhisper whisper)
  {
    ChatBubbleFrame chatBubbleFrame = UnityEngine.Object.Instantiate<ChatBubbleFrame>(prefab);
    chatBubbleFrame.SetWhisper(whisper);
    chatBubbleFrame.GetComponent<PegUIElement>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChatBubbleReleased));
    return chatBubbleFrame;
  }

  private void MoveChatBubbles(ChatBubbleFrame newBubbleFrame)
  {
    Anchor dstAnchor = Anchor.TOP_LEFT_XZ;
    Anchor srcAnchor = Anchor.BOTTOM_LEFT_XZ;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      dstAnchor = Anchor.BOTTOM_LEFT_XZ;
      srcAnchor = Anchor.TOP_LEFT_XZ;
    }
    TransformUtil.SetPoint((Component) newBubbleFrame, srcAnchor, (Component) this.m_ChatBubbleInfo.m_Parent, dstAnchor, Vector3.zero);
    int count = this.m_chatBubbleFrames.Count;
    if (count == 1)
      return;
    Vector3[] vector3Array = new Vector3[count - 1];
    Component dst = (Component) newBubbleFrame;
    for (int index = count - 2; index >= 0; --index)
    {
      ChatBubbleFrame chatBubbleFrame = this.m_chatBubbleFrames[index];
      vector3Array[index] = chatBubbleFrame.transform.position;
      TransformUtil.SetPoint((Component) chatBubbleFrame, srcAnchor, dst, dstAnchor, Vector3.zero);
      dst = (Component) chatBubbleFrame;
    }
    for (int index = count - 2; index >= 0; --index)
    {
      ChatBubbleFrame chatBubbleFrame = this.m_chatBubbleFrames[index];
      Hashtable args = iTween.Hash((object) "islocal", (object) true, (object) "position", (object) chatBubbleFrame.transform.localPosition, (object) "time", (object) this.m_ChatBubbleInfo.m_MoveOverSec, (object) "easeType", (object) this.m_ChatBubbleInfo.m_MoveOverEaseType);
      chatBubbleFrame.transform.position = vector3Array[index];
      iTween.Stop(chatBubbleFrame.gameObject, "move");
      iTween.MoveTo(chatBubbleFrame.gameObject, args);
    }
  }

  private void OnChatBubbleScaleInComplete(ChatBubbleFrame bubbleFrame)
  {
    Hashtable args = iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) this.m_ChatBubbleInfo.m_HoldSec, (object) "time", (object) this.m_ChatBubbleInfo.m_FadeOutSec, (object) "easeType", (object) this.m_ChatBubbleInfo.m_FadeOutEaseType, (object) "oncomplete", (object) "OnChatBubbleFadeOutComplete", (object) "oncompleteparams", (object) bubbleFrame, (object) "oncompletetarget", (object) this.gameObject);
    iTween.FadeTo(bubbleFrame.gameObject, args);
  }

  private void OnChatBubbleFadeOutComplete(ChatBubbleFrame bubbleFrame)
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) bubbleFrame.gameObject);
    this.m_chatBubbleFrames.Remove(bubbleFrame);
  }

  private void RemoveAllChatBubbles()
  {
    foreach (Component chatBubbleFrame in this.m_chatBubbleFrames)
      UnityEngine.Object.Destroy((UnityEngine.Object) chatBubbleFrame.gameObject);
    this.m_chatBubbleFrames.Clear();
  }

  private void OnChatBubbleReleased(UIEvent e)
  {
    this.ShowChatForPlayer(WhisperUtil.GetTheirPlayer(e.GetElement().GetComponent<ChatBubbleFrame>().GetWhisper()));
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.RemoveAllChatBubbles();
  }

  public void OnKeyboardShow()
  {
    if (this.m_chatLogUI.IsShowing && BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.position != this.m_chatLogUI.GameObject.transform.position)
    {
      ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
      touchScreenService.RemoveOnVirtualKeyboardShowListener(new System.Action(this.OnKeyboardShow));
      touchScreenService.RemoveOnVirtualKeyboardHideListener(new System.Action(this.OnKeyboardHide));
      this.m_chatLogUI.Hide();
      this.m_chatLogUI.ShowForPlayer(this.GetMostRecentWhisperedPlayer());
      touchScreenService.AddOnVirtualKeyboardShowListener(new System.Action(this.OnKeyboardShow));
      touchScreenService.AddOnVirtualKeyboardHideListener(new System.Action(this.OnKeyboardHide));
    }
    if ((bool) (UnityEngine.Object) BnetBarFriendButton.Get())
    {
      Vector2 offset = new Vector2(0.0f, (float) (Screen.height - 150));
      TransformUtil.SetPoint((Component) this.m_ChatBubbleInfo.m_Parent, Anchor.BOTTOM_LEFT_XZ, BnetBarFriendButton.Get().gameObject, Anchor.BOTTOM_RIGHT_XZ, (Vector3) offset);
    }
    int count = this.m_chatBubbleFrames.Count;
    if (count == 0)
      return;
    Component dst = (Component) this.m_ChatBubbleInfo.m_Parent;
    for (int index = count - 1; index >= 0; --index)
    {
      ChatBubbleFrame chatBubbleFrame = this.m_chatBubbleFrames[index];
      TransformUtil.SetPoint((Component) chatBubbleFrame, Anchor.TOP_LEFT_XZ, dst, Anchor.BOTTOM_LEFT_XZ, Vector3.zero);
      dst = (Component) chatBubbleFrame;
    }
  }

  public void OnKeyboardHide()
  {
    this.UpdateLayout();
    this.UpdateChatBubbleLayout();
  }

  public delegate void PlayerChatInfoChangedCallback(PlayerChatInfo chatInfo, object userData);

  public delegate void FriendListToggled(bool open);

  private class PlayerChatInfoChangedListener : EventListener<ChatMgr.PlayerChatInfoChangedCallback>
  {
    public void Fire(PlayerChatInfo chatInfo) => this.m_callback(chatInfo, this.m_userData);
  }

  private enum KeyboardState
  {
    None,
    Below,
    Above,
  }
}
