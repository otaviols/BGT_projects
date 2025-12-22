using Blizzard.GameService.SDK.Client.Integration;
using UnityEngine;

public class ChatFrames : MonoBehaviour
{
  public MobileChatLogFrame chatLogFrame;
  private bool wasShowingDialog;

  public BnetPlayer Receiver
  {
    get => this.chatLogFrame.Receiver;
    set
    {
      this.chatLogFrame.Receiver = value;
      if (this.chatLogFrame.Receiver == null)
        ChatMgr.Get().CloseChatUI();
      this.OnFramesMoved();
    }
  }

  private void Awake()
  {
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    Network.Get().OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
    this.chatLogFrame.CloseButtonReleased += new System.Action(this.OnCloseButtonReleased);
    ChatUtils.TrySendDeckcodeFromClipboard(new System.Action<string>(this.chatLogFrame.OnInputComplete));
  }

  private void OnDestroy()
  {
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    if (FatalErrorMgr.Get() != null)
      FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    Network.Get().OnDisconnectedFromBattleNet -= new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
    this.OnFramesMoved();
  }

  public void Show() => this.gameObject.SetActive(true);

  public void Hide() => this.gameObject.SetActive(false);

  private void Update()
  {
    bool flag = DialogManager.Get().ShowingDialog();
    if (flag == this.wasShowingDialog)
      return;
    if (flag && this.chatLogFrame.HasFocus)
      this.OnPopupOpened();
    else if (!flag && (UnityEngine.Object) ChatMgr.Get().FriendListFrame != (UnityEngine.Object) null && !ChatMgr.Get().FriendListFrame.ShowingAddFriendFrame && !this.chatLogFrame.HasFocus)
      this.OnPopupClosed();
    this.wasShowingDialog = flag;
  }

  public void Back()
  {
    if (DialogManager.Get().ShowingDialog())
      return;
    if (ChatMgr.Get().FriendListFrame.ShowingAddFriendFrame)
      ChatMgr.Get().FriendListFrame.CloseAddFriendFrame();
    else if (this.Receiver != null)
      this.Receiver = (BnetPlayer) null;
    else
      ChatMgr.Get().CloseChatUI();
  }

  private void OnFramesMoved()
  {
    if (!((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null))
      return;
    ChatMgr.Get().OnChatFramesMoved();
  }

  private void OnCloseButtonReleased()
  {
    ChatMgr.Get().CloseChatUI();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    ChatMgr.Get().ShowFriendsList();
  }

  private void OnPopupOpened()
  {
    if (!this.chatLogFrame.HasFocus)
      return;
    this.chatLogFrame.Focus(false);
  }

  private void OnPopupClosed()
  {
    if (this.Receiver == null)
      return;
    this.chatLogFrame.Focus(true);
  }

  private void OnDisconnectedFromBattleNet(BattleNetErrors error) => ChatMgr.Get().CleanUp();

  private void OnFatalError(FatalErrorMessage message, object userData) => ChatMgr.Get().CleanUp();

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.FATAL_ERROR)
      return;
    ChatMgr.Get().CleanUp();
  }
}
