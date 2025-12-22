using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class ChatLog : MonoBehaviour
{
  public TouchList messageFrames;
  public GameObject cameraTarget;
  public ChatLog.Prefabs prefabs;
  public ChatLog.MessageInfo messageInfo;
  public MobileChatNotification notifications;
  private const int maxMessageFrames = 500;
  private const int frameWidthOffset = 10;
  private const GameLayer messageLayer = GameLayer.BattleNetChat;
  private const CustomViewEntryPoint BattleNetChatViewEntryPoint = CustomViewEntryPoint.BattleNetChat;
  private const string OverridePassName = "BattleNetChatLog";
  private const string BgOverridePassName = "BattleNetChatBG";
  private const uint MessagesRenderingLayerMask = 1;
  private const uint BgRenderingLayerMask = 2;
  private BnetPlayer receiver;
  private IGraphicsManager m_graphicsManager;
  private CameraOverridePass m_messagesCameraOverridePass;
  private CameraOverridePass m_bgCameraOverridePass;

  public BnetPlayer Receiver
  {
    get => this.receiver;
    set
    {
      if (this.receiver == value)
        return;
      this.receiver = value;
      if (this.receiver == null)
        return;
      this.UpdateMessages();
      if (!this.receiver.IsOnline())
        this.AddReceiverOfflineMessage();
      this.messageFrames.ScrollValue = 1f;
    }
  }

  private void Awake()
  {
    this.CreateMessagesCamera();
    if ((UnityEngine.Object) this.notifications != (UnityEngine.Object) null)
      this.notifications.Notified += new MobileChatNotification.NotifiedEvent(this.OnNotified);
    BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.m_graphicsManager.OnResolutionChangedEvent += new System.Action<int, int>(this.OnResizeAfterCurrentFrame);
  }

  private void Start() => this.messageFrames.SelectionEnabled = true;

  private void OnDestroy()
  {
    this.m_messagesCameraOverridePass?.Unschedule();
    this.m_bgCameraOverridePass?.Unschedule();
    if ((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null)
      PegUI.Get().UnregisterFromRenderPassPriorityHitTest((Component) this);
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    BnetWhisperMgr.Get().RemoveWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
    if (this.m_graphicsManager != null)
      this.m_graphicsManager.OnResolutionChangedEvent -= new System.Action<int, int>(this.OnResizeAfterCurrentFrame);
    if (!((UnityEngine.Object) this.notifications != (UnityEngine.Object) null))
      return;
    this.notifications.Notified -= new MobileChatNotification.NotifiedEvent(this.OnNotified);
  }

  public void OnResize()
  {
    this.ResizeMessageFrames();
    this.UpdateMessagesCamera();
  }

  private void ResizeMessageFrames()
  {
    float scrollValue = this.messageFrames.ScrollValue;
    foreach (ITouchListItem renderedItem in this.messageFrames.RenderedItems)
    {
      MobileChatLogMessageFrame chatLogMessageFrame = renderedItem as MobileChatLogMessageFrame;
      if ((UnityEngine.Object) chatLogMessageFrame != (UnityEngine.Object) null)
      {
        chatLogMessageFrame.Width = (float) ((double) this.messageFrames.ClipSize.x - (double) this.messageFrames.padding.x - 10.0);
        chatLogMessageFrame.UpdateLocalBounds();
      }
    }
    this.messageFrames.RecalculateItemSizeAndOffsets(true);
    this.messageFrames.ScrollValue = scrollValue;
  }

  public void OnWhisperFailed()
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer.IsAppearingOffline())
      this.AddAppearOfflineMessage();
    else if (!myPlayer.IsOnline())
      this.AddSenderOfflineMessage();
    else
      this.AddReceiverOfflineMessage();
  }

  private void OnWhisper(BnetWhisper whisper, object userData)
  {
    if (this.receiver == null || !WhisperUtil.IsSpeakerOrReceiver(this.receiver, whisper))
      return;
    this.AddWhisperMessage(whisper);
    this.messageFrames.ScrollValue = 1f;
  }

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    BnetPlayerChange change = changelist.FindChange(this.receiver);
    if (change == null)
      return;
    BnetPlayer oldPlayer = change.GetOldPlayer();
    BnetPlayer newPlayer = change.GetNewPlayer();
    if (oldPlayer != null && oldPlayer.IsOnline() == newPlayer.IsOnline())
      return;
    if (newPlayer.IsOnline())
      this.AddOnlineMessage();
    else
      this.AddReceiverOfflineMessage();
  }

  private void OnNotified(string text) => this.AddSystemMessage(text, this.messageInfo.notificationColor);

  private void UpdateMessages()
  {
    List<MobileChatLogMessageFrame> list = this.messageFrames.RenderedItems.Select<ITouchListItem, MobileChatLogMessageFrame>((Func<ITouchListItem, MobileChatLogMessageFrame>) (i => i.GetComponent<MobileChatLogMessageFrame>())).ToList<MobileChatLogMessageFrame>();
    this.messageFrames.Clear();
    foreach (Component component in list)
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(this.receiver);
    if (whispersWithPlayer != null && whispersWithPlayer.Count > 0)
    {
      for (int index = Mathf.Max(whispersWithPlayer.Count - 500, 0); index < whispersWithPlayer.Count; ++index)
        this.AddWhisperMessage(whispersWithPlayer[index]);
    }
    this.OnMessagesAdded();
  }

  private void AddWhisperMessage(BnetWhisper whisper)
  {
    string message = ChatUtils.GetMessage(whisper);
    string deckName;
    ShareableMercenariesTeam deckCode1 = ShareableMercenariesTeam.ParseDeckCode(message, out deckName);
    if (deckCode1 != null)
    {
      deckCode1.DeckName = deckName;
      this.messageFrames.Add((ITouchListItem) this.CreateDeckcodeMessage(this.prefabs.deckcodeMessage, (ShareableDeck) deckCode1));
    }
    else
    {
      ShareableDeck deckCode2 = ShareableDeck.ParseDeckCode(message, out deckName);
      if (deckCode2 != null)
      {
        deckCode2.DeckName = deckName;
        this.messageFrames.Add((ITouchListItem) this.CreateDeckcodeMessage(this.prefabs.deckcodeMessage, deckCode2));
      }
      else
        this.messageFrames.Add((ITouchListItem) this.CreateMessage(WhisperUtil.IsSpeaker(this.receiver, whisper) ? this.prefabs.theirMessage : this.prefabs.myMessage, message));
    }
  }

  private void AddSystemMessage(string message, Color color)
  {
    this.messageFrames.Add((ITouchListItem) this.CreateMessage(this.prefabs.systemMessage, message, color));
    this.OnMessagesAdded();
  }

  private void AddOnlineMessage() => this.AddSystemMessage(GameStrings.Format("GLOBAL_CHAT_RECEIVER_ONLINE", (object) this.receiver.GetBestName()), this.messageInfo.infoColor);

  private void AddReceiverOfflineMessage() => this.AddSystemMessage(GameStrings.Format("GLOBAL_CHAT_RECEIVER_OFFLINE", (object) this.receiver.GetBestName()), this.messageInfo.errorColor);

  private void AddSenderOfflineMessage() => this.AddSystemMessage(GameStrings.Get("GLOBAL_CHAT_SENDER_OFFLINE"), this.messageInfo.errorColor);

  private void AddAppearOfflineMessage() => this.AddSystemMessage(GameStrings.Get("GLOBAL_CHAT_SENDER_APPEAR_OFFLINE"), this.messageInfo.errorColor);

  private void OnMessagesAdded()
  {
    if (this.messageFrames.RenderedItems.Count<ITouchListItem>() > 500)
    {
      ITouchListItem touchListItem = this.messageFrames.RenderedItems.First<ITouchListItem>();
      this.messageFrames.RemoveAt(0);
      UnityEngine.Object.Destroy((UnityEngine.Object) touchListItem.gameObject);
    }
    this.messageFrames.ScrollValue = 1f;
  }

  private MobileChatLogMessageFrame CreateMessage(
    MobileChatLogMessageFrame prefab,
    string message)
  {
    MobileChatLogMessageFrame c = UnityEngine.Object.Instantiate<MobileChatLogMessageFrame>(prefab);
    c.Width = (float) ((double) this.messageFrames.ClipSize.x - (double) this.messageFrames.padding.x - 10.0);
    c.Message = message;
    LayerUtils.SetLayer((Component) c, GameLayer.BattleNetChat);
    return c;
  }

  private MobileChatLogMessageFrame CreateMessage(
    MobileChatLogMessageFrame prefab,
    string message,
    Color color)
  {
    MobileChatLogMessageFrame message1 = this.CreateMessage(prefab, message);
    message1.Color = color;
    return message1;
  }

  private MobileChatLogDeckcodeMessageFrame CreateDeckcodeMessage(
    MobileChatLogMessageFrame prefab,
    ShareableDeck shareableDeck)
  {
    MobileChatLogDeckcodeMessageFrame c = (MobileChatLogDeckcodeMessageFrame) UnityEngine.Object.Instantiate<MobileChatLogMessageFrame>(prefab);
    c.Width = (float) ((double) this.messageFrames.ClipSize.x - (double) this.messageFrames.padding.x - 10.0);
    c.DeckcodeString = shareableDeck.Serialize(false);
    c.BindClassData(shareableDeck);
    LayerUtils.SetLayer((Component) c, GameLayer.BattleNetChat);
    return c;
  }

  private void CreateMessagesCamera()
  {
    this.m_messagesCameraOverridePass = new CameraOverridePass("BattleNetChatLog", (LayerMask) GameLayer.BattleNetChat.LayerBit());
    this.m_bgCameraOverridePass = new CameraOverridePass("BattleNetChatBG", (LayerMask) GameLayer.BattleNetChat.LayerBit());
    this.UpdateMessagesCamera();
    this.m_bgCameraOverridePass.Schedule(CustomViewEntryPoint.BattleNetChat);
    this.m_messagesCameraOverridePass.Schedule(CustomViewEntryPoint.BattleNetChat);
    this.m_messagesCameraOverridePass.OverrideRenderLayerMask(1U);
    this.m_bgCameraOverridePass.OverrideRenderLayerMask(2U);
    if (!((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null))
      return;
    PegUI.Get().RegisterForRenderPassPriorityHitTest((Component) this);
  }

  private Bounds GetBoundsFromGameObject(GameObject go)
  {
    Renderer component1 = go.GetComponent<Renderer>();
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      return component1.bounds;
    Collider component2 = go.GetComponent<Collider>();
    return (UnityEngine.Object) component2 != (UnityEngine.Object) null ? component2.bounds : new Bounds();
  }

  private void UpdateMessagesCamera()
  {
    Camera bnetCamera = BaseUI.Get().GetBnetCamera();
    Bounds boundsFromGameObject = this.GetBoundsFromGameObject(this.cameraTarget);
    Vector3 screenPoint1 = bnetCamera.WorldToScreenPoint(boundsFromGameObject.min);
    Vector3 screenPoint2 = bnetCamera.WorldToScreenPoint(boundsFromGameObject.max);
    this.m_messagesCameraOverridePass.OverrideScissor(new Rect(screenPoint1.x, screenPoint1.y, screenPoint2.x - screenPoint1.x, screenPoint2.y - screenPoint1.y));
  }

  private void OnResizeAfterCurrentFrame(int width, int height) => this.StartCoroutine(this.UpdateMessagesCameraAfterCurrentFrame());

  private IEnumerator UpdateMessagesCameraAfterCurrentFrame()
  {
    yield return (object) null;
    this.UpdateMessagesCamera();
  }

  [Conditional("CHATLOG_DEBUG")]
  private void AssignMessageFrameNames()
  {
    int num = 0;
    foreach (ITouchListItem renderedItem in this.messageFrames.RenderedItems)
    {
      MobileChatLogMessageFrame component = renderedItem.GetComponent<MobileChatLogMessageFrame>();
      component.name = string.Format("MessageFrame {0} ({1})", (object) num, (object) component.Message);
      ++num;
    }
  }

  [Serializable]
  public class Prefabs
  {
    public MobileChatLogMessageFrame myMessage;
    public MobileChatLogMessageFrame theirMessage;
    public MobileChatLogMessageFrame systemMessage;
    public MobileChatLogMessageFrame deckcodeMessage;
  }

  [Serializable]
  public class MessageInfo
  {
    public Color infoColor = Color.yellow;
    public Color errorColor = Color.red;
    public Color notificationColor = Color.cyan;
  }
}
