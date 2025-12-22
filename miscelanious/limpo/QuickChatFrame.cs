using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Fonts;
using Blizzard.T5.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickChatFrame : MonoBehaviour
{
  public QuickChatFrameBones m_Bones;
  public QuickChatFramePrefabs m_Prefabs;
  public GameObject m_Background;
  public UberText m_ReceiverNameText;
  public UberText m_LastMessageText;
  public GameObject m_LastMessageShadow;
  public PegUIElement m_ChatLogButton;
  public Font m_InputFont;
  private DropdownControl m_recentPlayerDropdown;
  private ChatLogFrame m_chatLogFrame;
  private PegUIElement m_inputBlocker;
  private List<BnetPlayer> m_recentPlayers = new List<BnetPlayer>();
  private BnetPlayer m_receiver;
  private float m_initialLastMessageTextHeight;
  private float m_initialLastMessageShadowScaleZ;
  private Font m_localizedInputFont;
  private IFontTable m_fontTable;
  private Map<Renderer, int> m_chatLogOriginalLayers = new Map<Renderer, int>();

  private void Awake()
  {
    this.m_fontTable = ServiceManager.Get<IFontTable>();
    this.InitRecentPlayers();
    if (!this.InitReceiver())
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
      BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
      this.InitTransform();
      this.InitInputBlocker();
      this.InitChatLogFrame();
      this.InitInput();
      this.ShowInput(true);
      ChatUtils.TrySendDeckcodeFromClipboard(new System.Action<string>(this.OnInputComplete));
    }
  }

  private void Start()
  {
    this.InitLastMessage();
    this.InitRecentPlayerDropdown();
    if (ChatMgr.Get().IsChatLogFrameShown())
      this.ShowChatLogFrame(true);
    this.UpdateReceiver();
    ChatMgr.Get().OnChatReceiverChanged(this.m_receiver);
  }

  private void OnDestroy()
  {
    BnetWhisperMgr.Get().RemoveWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    if (UniversalInputManager.Get() != null)
      UniversalInputManager.Get().CancelTextInput(this.gameObject);
    ChatMgr.Get().OnQuickChatFrameClosed();
  }

  public ChatLogFrame GetChatLogFrame() => this.m_chatLogFrame;

  public BnetPlayer GetReceiver() => this.m_receiver;

  public void SetReceiver(BnetPlayer player)
  {
    if (this.m_receiver == player)
      return;
    this.m_receiver = player;
    this.UpdateReceiver();
    this.m_recentPlayerDropdown.setSelection((object) player);
    ChatMgr.Get().OnChatReceiverChanged(player);
  }

  public void UpdateLayout()
  {
    if (!((UnityEngine.Object) this.m_chatLogFrame != (UnityEngine.Object) null))
      return;
    this.m_chatLogFrame.UpdateLayout();
  }

  private void InitRecentPlayers() => this.UpdateRecentPlayers();

  private void UpdateRecentPlayers()
  {
    this.m_recentPlayers.Clear();
    List<BnetPlayer> recentWhisperPlayers = ChatMgr.Get().GetRecentWhisperPlayers();
    for (int index = 0; index < recentWhisperPlayers.Count; ++index)
      this.m_recentPlayers.Add(recentWhisperPlayers[index]);
  }

  private bool InitReceiver()
  {
    this.m_receiver = (BnetPlayer) null;
    if (this.m_recentPlayers.Count == 0)
    {
      string message = BnetFriendMgr.Get().GetOnlineFriendCount() != 0 ? GameStrings.Get("GLOBAL_CHAT_NO_RECENT_CONVERSATIONS") : GameStrings.Get("GLOBAL_CHAT_NO_FRIENDS_ONLINE");
      UIStatus.Get().AddError(message);
      return false;
    }
    this.m_receiver = this.m_recentPlayers[0];
    return true;
  }

  private void OnWhisper(BnetWhisper whisper, object userData)
  {
    if (this.m_receiver == null || !WhisperUtil.IsSpeaker(this.m_receiver, whisper))
      return;
    this.UpdateReceiver();
  }

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    BnetPlayerChange change = changelist.FindChange(this.m_receiver);
    if (change == null)
      return;
    BnetPlayer oldPlayer = change.GetOldPlayer();
    BnetPlayer newPlayer = change.GetNewPlayer();
    if (oldPlayer != null && oldPlayer.IsOnline() == newPlayer.IsOnline())
      return;
    this.UpdateReceiver();
  }

  private BnetWhisper FindLastWhisperFromReceiver()
  {
    List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(this.m_receiver);
    if (whispersWithPlayer == null)
      return (BnetWhisper) null;
    for (int index = whispersWithPlayer.Count - 1; index >= 0; --index)
    {
      BnetWhisper whisper = whispersWithPlayer[index];
      if (WhisperUtil.IsSpeaker(this.m_receiver, whisper))
        return whisper;
    }
    return (BnetWhisper) null;
  }

  private void InitTransform()
  {
    this.transform.parent = BaseUI.Get().transform;
    this.DefaultChatTransform();
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    if ((!UniversalInputManager.Get().UseWindowsTouch() || !touchScreenService.IsTouchSupported()) && !touchScreenService.IsVirtualKeyboardVisible())
      return;
    this.TransformChatForKeyboard();
  }

  private void InitLastMessage()
  {
    this.m_LastMessageText.Text = "*** DO NOT DELETE. THIS TEXT IS USED FOR SIZE COMPUTATIONS. ***";
    this.m_initialLastMessageTextHeight = this.m_LastMessageText.GetTextWorldSpaceBounds().size.y;
    this.m_LastMessageText.Text = string.Empty;
    this.m_initialLastMessageShadowScaleZ = this.m_LastMessageShadow.transform.localScale.z;
  }

  private void InitInputBlocker()
  {
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "QuickChatInputBlocker", (Component) this, this.m_Bones.m_InputBlocker.position.z - this.transform.position.z);
    inputBlocker.layer = 26;
    this.m_inputBlocker = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerReleased));
  }

  private void OnInputBlockerReleased(UIEvent e) => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  private void InitChatLogFrame() => this.m_ChatLogButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChatLogButtonReleased));

  private void OnChatLogButtonReleased(UIEvent e)
  {
    if (ChatMgr.Get().IsChatLogFrameShown())
      this.HideChatLogFrame();
    else
      this.ShowChatLogFrame();
    this.UpdateReceiver();
    UniversalInputManager.Get().FocusTextInput(this.gameObject);
  }

  private void ShowChatLogFrame(bool onStart = false)
  {
    this.m_chatLogFrame = UnityEngine.Object.Instantiate<ChatLogFrame>(this.m_Prefabs.m_ChatLogFrame);
    bool flag = this.transform.localScale == BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.localScale;
    ITouchScreenService touchScreenService = ServiceManager.Get<ITouchScreenService>();
    if (((!UniversalInputManager.Get().IsTouchMode() || !touchScreenService.IsTouchSupported() ? (touchScreenService.IsVirtualKeyboardVisible() ? 1 : 0) : 1) & (flag ? 1 : 0) | (flag ? 1 : 0)) != 0)
      this.DefaultChatTransform();
    this.m_chatLogFrame.transform.parent = this.transform;
    this.m_chatLogFrame.transform.position = this.m_Bones.m_ChatLog.position;
    if (((!UniversalInputManager.Get().UseWindowsTouch() || !touchScreenService.IsTouchSupported() ? (touchScreenService.IsVirtualKeyboardVisible() ? 1 : 0) : 1) & (flag ? 1 : 0) | (flag ? 1 : 0)) != 0)
      this.TransformChatForKeyboard();
    this.StartCoroutine(this.ShowChatLogFrameWhenReady(onStart ? this.gameObject : this.m_chatLogFrame.gameObject));
  }

  private IEnumerator ShowChatLogFrameWhenReady(GameObject obj)
  {
    while ((UnityEngine.Object) this.m_chatLogFrame == (UnityEngine.Object) null || this.m_chatLogFrame.IsWaitingOnMedal)
    {
      if ((UnityEngine.Object) this.m_chatLogFrame == (UnityEngine.Object) null)
        yield break;
      else
        yield return (object) null;
    }
    ChatMgr.Get().OnChatLogFrameShown();
  }

  private void HideChatLogFrame()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_chatLogFrame.gameObject);
    this.m_chatLogFrame = (ChatLogFrame) null;
    ChatMgr.Get().OnChatLogFrameHidden();
  }

  private void InitRecentPlayerDropdown()
  {
    this.m_recentPlayerDropdown = UnityEngine.Object.Instantiate<DropdownControl>(this.m_Prefabs.m_Dropdown, this.transform);
    this.m_recentPlayerDropdown.transform.position = this.m_Bones.m_RecentPlayerDropdown.position;
    this.m_recentPlayerDropdown.setItemTextCallback(new DropdownControl.itemTextCallback(this.OnRecentPlayerDropdownText));
    this.m_recentPlayerDropdown.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnRecentPlayerDropdownItemChosen));
    this.UpdateRecentPlayerDropdown();
    this.m_recentPlayerDropdown.setSelection((object) this.m_receiver);
  }

  private void UpdateRecentPlayerDropdown()
  {
    this.m_recentPlayerDropdown.clearItems();
    for (int index = 0; index < this.m_recentPlayers.Count; ++index)
      this.m_recentPlayerDropdown.addItem((object) this.m_recentPlayers[index]);
  }

  private string OnRecentPlayerDropdownText(object val) => FriendUtils.GetUniqueName((BnetPlayer) val);

  private void OnRecentPlayerDropdownItemChosen(object selection, object prevSelection) => this.SetReceiver((BnetPlayer) selection);

  private void UpdateReceiver()
  {
    this.UpdateLastMessage();
    if (!((UnityEngine.Object) this.m_chatLogFrame != (UnityEngine.Object) null))
      return;
    this.m_chatLogFrame.Receiver = this.m_receiver;
  }

  private void UpdateLastMessage()
  {
    if ((UnityEngine.Object) this.m_chatLogFrame != (UnityEngine.Object) null)
    {
      this.HideLastMessage();
    }
    else
    {
      BnetWhisper whisperFromReceiver = this.FindLastWhisperFromReceiver();
      if (whisperFromReceiver == null)
      {
        this.HideLastMessage();
      }
      else
      {
        this.m_LastMessageText.gameObject.SetActive(true);
        string message = ChatUtils.GetMessage(whisperFromReceiver);
        string formattedDeckcodeMessage;
        this.m_LastMessageText.Text = !ChatUtils.TryGetFormattedDeckcodeMessage(message, false, out formattedDeckcodeMessage) ? message : formattedDeckcodeMessage;
        TransformUtil.SetPoint((Component) this.m_LastMessageText, Anchor.BOTTOM_LEFT, (Component) this.m_Bones.m_LastMessage, Anchor.TOP_LEFT);
        this.m_ReceiverNameText.gameObject.SetActive(true);
        this.m_ReceiverNameText.TextColor = !this.m_receiver.IsOnline() ? GameColors.PLAYER_NAME_OFFLINE : GameColors.PLAYER_NAME_ONLINE;
        this.m_ReceiverNameText.Text = FriendUtils.GetUniqueName(this.m_receiver);
        TransformUtil.SetPoint((Component) this.m_ReceiverNameText, Anchor.BOTTOM_LEFT_XZ, (Component) this.m_LastMessageText, Anchor.TOP_LEFT_XZ);
        this.m_LastMessageShadow.SetActive(true);
        Bounds worldSpaceBounds1 = this.m_LastMessageText.GetTextWorldSpaceBounds();
        Bounds worldSpaceBounds2 = this.m_ReceiverNameText.GetTextWorldSpaceBounds();
        TransformUtil.SetLocalScaleZ(this.m_LastMessageShadow, (Mathf.Max(worldSpaceBounds1.max.y, worldSpaceBounds2.max.y) - Mathf.Min(worldSpaceBounds1.min.y, worldSpaceBounds2.min.y)) * this.m_initialLastMessageShadowScaleZ / this.m_initialLastMessageTextHeight);
      }
    }
  }

  private void HideLastMessage()
  {
    this.m_ReceiverNameText.gameObject.SetActive(false);
    this.m_LastMessageText.gameObject.SetActive(false);
    this.m_LastMessageShadow.SetActive(false);
  }

  private void CyclePrevReceiver()
  {
    int index = this.m_recentPlayers.FindIndex((Predicate<BnetPlayer>) (currReceiver => this.m_receiver == currReceiver));
    this.SetReceiver(index != 0 ? this.m_recentPlayers[index - 1] : this.m_recentPlayers[this.m_recentPlayers.Count - 1]);
  }

  private void CycleNextReceiver()
  {
    int index = this.m_recentPlayers.FindIndex((Predicate<BnetPlayer>) (currReceiver => this.m_receiver == currReceiver));
    this.SetReceiver(index != this.m_recentPlayers.Count - 1 ? this.m_recentPlayers[index + 1] : this.m_recentPlayers[0]);
  }

  private void InitInput()
  {
    FontDefinition fontDef = this.m_fontTable.GetFontDef(this.m_InputFont);
    if ((UnityEngine.Object) fontDef == (UnityEngine.Object) null)
      this.m_localizedInputFont = this.m_InputFont;
    else
      this.m_localizedInputFont = fontDef.m_Font;
  }

  private void ShowInput(bool fromAwake)
  {
    Camera bnetCamera = BaseUI.Get().GetBnetCamera();
    Rect rect = CameraUtils.CreateGUIViewportRect(bnetCamera, (Component) this.m_Bones.m_InputTopLeft, (Component) this.m_Bones.m_InputBottomRight);
    if (Localization.GetLocale() == 15)
    {
      Vector3 vector3_1 = bnetCamera.WorldToViewportPoint(this.m_Bones.m_InputTopLeft.position);
      Vector3 vector3_2 = bnetCamera.WorldToViewportPoint(this.m_Bones.m_InputBottomRight.position);
      float num = (float) (((double) vector3_1.y - (double) vector3_2.y) * 0.100000001490116);
      vector3_1 = new Vector3(vector3_1.x, vector3_1.y - num, vector3_1.z);
      vector3_2 = new Vector3(vector3_2.x, vector3_2.y + num, vector3_2.z);
      rect = new Rect(vector3_1.x, 1f - vector3_1.y, vector3_2.x - vector3_1.x, vector3_1.y - vector3_2.y);
    }
    string pendingMessage = ChatMgr.Get().GetPendingMessage(this.m_receiver.GetAccountId());
    UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams()
    {
      m_owner = this.gameObject,
      m_rect = rect,
      m_preprocessCallback = new UniversalInputManager.TextInputPreprocessCallback(this.OnInputPreprocess),
      m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
      m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
      m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(this.OnInputChanged),
      m_font = this.m_localizedInputFont,
      m_maxCharacters = 512,
      m_touchScreenKeyboardHideInput = true,
      m_showVirtualKeyboard = fromAwake,
      m_hideVirtualKeyboardOnComplete = fromAwake,
      m_text = pendingMessage
    };
    UniversalInputManager.Get().UseTextInput(parms);
  }

  private bool OnInputPreprocess()
  {
    if (this.m_recentPlayers.Count < 2)
      return false;
    bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Tab) & flag)
    {
      this.CyclePrevReceiver();
      return true;
    }
    if (!Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.Tab))
      return false;
    this.CycleNextReceiver();
    return true;
  }

  private void OnInputChanged(string input) => ChatMgr.Get().SetPendingMessage(this.m_receiver.GetAccountId(), input);

  private void OnInputComplete(string input)
  {
    if (!string.IsNullOrEmpty(input))
    {
      BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
      if (!BnetWhisperMgr.Get().SendWhisper(this.m_receiver, input))
      {
        if (ChatMgr.Get().IsChatLogFrameShown())
          this.m_chatLogFrame.m_chatLog.OnWhisperFailed();
        else if (!this.m_receiver.IsOnline())
        {
          string message = GameStrings.Format("GLOBAL_CHAT_RECEIVER_OFFLINE", (object) this.m_receiver.GetBestName());
          UIStatus.Get().AddError(message);
        }
        else if (myPlayer.IsAppearingOffline())
        {
          string message = GameStrings.Get("GLOBAL_CHAT_SENDER_APPEAR_OFFLINE");
          UIStatus.Get().AddError(message);
        }
        ChatMgr.Get().AddRecentWhisperPlayerToTop(this.m_receiver);
      }
    }
    ChatMgr.Get().SetPendingMessage(this.m_receiver.GetAccountId(), (string) null);
    if (ChatMgr.Get().IsChatLogFrameShown())
      this.ShowInput(false);
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void OnInputCanceled(bool userRequested, GameObject requester) => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  private void DefaultChatTransform()
  {
    this.transform.position = BaseUI.Get().m_Bones.m_QuickChat.position;
    this.transform.localScale = BaseUI.Get().m_Bones.m_QuickChat.localScale;
    if (!((UnityEngine.Object) this.m_chatLogFrame != (UnityEngine.Object) null))
      return;
    this.m_chatLogFrame.UpdateLayout();
  }

  private void TransformChatForKeyboard()
  {
    this.transform.position = BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.position;
    this.transform.localScale = BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.localScale;
    this.m_Prefabs.m_Dropdown.transform.localScale = new Vector3(50f, 50f, 50f);
    if (!((UnityEngine.Object) this.m_chatLogFrame != (UnityEngine.Object) null))
      return;
    this.m_chatLogFrame.UpdateLayout();
  }
}
