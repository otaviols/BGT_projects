using Hearthstone.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobileChatLogFrame : MonoBehaviour
{
  public Spawner playerIconRef;
  public TouchList messageFrames;
  public MobileChatLogFrame.InputInfo inputInfo;
  public TextField inputTextField;
  public MobileChatLogFrame.MessageInfo messageInfo;
  public NineSliceElement window;
  public UberText nameText;
  public UIBButton closeButton;
  public MobileChatNotification notifications;
  public AsyncReference m_rankedMedalWidgetReference;
  public ChatLog chatLog;
  public MobileChatLogFrame.Followers followers;
  private PlayerIcon playerIcon;
  private BnetPlayer receiver;
  private SelectableMedal m_selectableMedal;
  private Widget m_selectableMedalWidget;

  public bool HasFocus => this.inputTextField.Active;

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
      this.playerIcon.SetPlayer(this.receiver);
      this.UpdateReceiver();
      this.chatLog.Receiver = this.receiver;
    }
  }

  public bool IsWaitingOnMedal
  {
    get
    {
      MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankedMedalFromRankPresenceField(this.receiver.GetBestGameAccount());
      if (rankPresenceField == null || !rankPresenceField.IsDisplayable())
        return false;
      return (UnityEngine.Object) this.m_selectableMedalWidget == (UnityEngine.Object) null || !this.m_selectableMedalWidget.IsReady || this.m_selectableMedalWidget.IsChangingStates;
    }
  }

  public event Action InputCanceled;

  public event Action CloseButtonReleased;

  private void Awake()
  {
    this.playerIcon = this.playerIconRef.Spawn<PlayerIcon>();
    this.UpdateBackgroundCollider();
    this.inputTextField.maxCharacters = 512;
    this.inputTextField.Changed += new Action<string>(this.OnInputChanged);
    this.inputTextField.Submitted += new Action<string>(this.OnInputComplete);
    this.inputTextField.Canceled += new Action(this.OnInputCanceled);
    this.closeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCloseButtonReleased));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
  }

  private void Start()
  {
    if (this.receiver == null)
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      string pendingMessage = ChatMgr.Get().GetPendingMessage(this.receiver.GetAccountId());
      if (pendingMessage != null)
        this.inputTextField.Text = pendingMessage;
    }
    this.m_rankedMedalWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnSelectableMedalWidgetReady));
  }

  private void OnDestroy() => BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));

  public void Focus(bool focus)
  {
    if (focus && !this.inputTextField.Active)
    {
      this.inputTextField.Activate();
    }
    else
    {
      if (focus || !this.inputTextField.Active)
        return;
      this.inputTextField.Deactivate();
    }
  }

  public void SetWorldRect(float x, float z, float width, float height)
  {
    bool activeSelf = this.gameObject.activeSelf;
    this.gameObject.SetActive(true);
    float viewWindowMaxValue = this.messageFrames.ViewWindowMaxValue;
    this.window.SetEntireSize(width, height);
    Vector3 worldPoint = TransformUtil.ComputeWorldPoint(TransformUtil.ComputeSetPointBounds((Component) this.window), new Vector3(0.0f, 0.0f, 1f));
    this.transform.Translate(new Vector3(x, worldPoint.y, z) - worldPoint, Space.World);
    this.messageFrames.transform.position = (this.messageInfo.messagesTopLeft.position + this.messageInfo.messagesBottomRight.position) / 2f;
    Vector3 vector3 = (this.messageInfo.messagesBottomRight.position - this.messageInfo.messagesTopLeft.position) * 4f;
    this.messageFrames.ClipSize = new Vector2(vector3.x, Math.Abs(vector3.z));
    this.messageFrames.ViewWindowMaxValue = viewWindowMaxValue;
    this.messageFrames.ScrollValue = Mathf.Clamp01(this.messageFrames.ScrollValue);
    this.chatLog.OnResize();
    this.UpdateBackgroundCollider();
    this.UpdateFollowers();
    this.gameObject.SetActive(activeSelf);
  }

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (changelist.FindChange(this.receiver) == null)
      return;
    this.UpdateReceiver();
  }

  private void OnSelectableMedalWidgetReady(Widget widget)
  {
    this.m_selectableMedalWidget = widget;
    this.m_selectableMedal = widget.GetComponentInChildren<SelectableMedal>();
    this.UpdateSelectableMedalWidget();
  }

  private void OnCloseButtonReleased(UIEvent e)
  {
    if (this.CloseButtonReleased == null)
      return;
    this.CloseButtonReleased();
  }

  private bool IsFullScreenKeyboard() => (double) ChatMgr.Get().KeyboardRect.height == (double) Screen.height;

  private void OnInputChanged(string input) => ChatMgr.Get().SetPendingMessage(this.receiver.GetAccountId(), input);

  public void OnInputComplete(string input)
  {
    if (string.IsNullOrEmpty(input))
      return;
    if (!BnetWhisperMgr.Get().SendWhisper(this.receiver, input))
      this.chatLog.OnWhisperFailed();
    ChatMgr.Get().SetPendingMessage(this.receiver.GetAccountId(), (string) null);
    ChatMgr.Get().AddRecentWhisperPlayerToTop(this.receiver);
  }

  private void OnInputCanceled()
  {
    if (this.InputCanceled == null)
      return;
    this.InputCanceled();
  }

  private void UpdateReceiver()
  {
    this.playerIcon.UpdateIcon();
    this.nameText.Text = string.Format("<color=#{0}>{1}</color>", this.receiver.IsOnline() ? (object) "5ecaf0ff" : (object) "999999ff", (object) this.receiver.GetBestName());
    this.UpdateSelectableMedalWidget();
  }

  private void UpdateSelectableMedalWidget()
  {
    if ((UnityEngine.Object) this.m_selectableMedal == (UnityEngine.Object) null || !this.receiver.IsOnline())
    {
      this.playerIcon.Show();
    }
    else
    {
      this.playerIcon.Hide();
      this.m_selectableMedal.gameObject.SetActive(true);
      this.m_selectableMedal.UpdateWidget(this.receiver, onDisplayNoMedal: ((Action) (() =>
      {
        this.playerIcon.Show();
        this.m_selectableMedal.gameObject.SetActive(false);
      })));
    }
  }

  private void UpdateBackgroundCollider()
  {
    BoxCollider boxCollider = this.GetComponent<BoxCollider>();
    if ((UnityEngine.Object) boxCollider == (UnityEngine.Object) null)
      boxCollider = this.gameObject.AddComponent<BoxCollider>();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      boxCollider.center = new Vector3(0.0f, 0.0f, 50f);
      boxCollider.size = new Vector3(10000f, 10000f, 0.0f);
    }
    else
    {
      Bounds bounds = ((IEnumerable<Renderer>) this.window.GetComponentsInChildren<Renderer>()).Aggregate<Renderer, Bounds>(new Bounds(this.transform.position, Vector3.zero), (Func<Bounds, Renderer, Bounds>) ((aggregate, renderer) =>
      {
        if ((double) renderer.bounds.size.x != 0.0 && (double) renderer.bounds.size.y != 0.0 && (double) renderer.bounds.size.z != 0.0)
          aggregate.Encapsulate(renderer.bounds);
        return aggregate;
      }));
      Vector3 vector3_1 = this.transform.InverseTransformPoint(bounds.min);
      Vector3 vector3_2 = this.transform.InverseTransformPoint(bounds.max);
      boxCollider.center = (vector3_1 + vector3_2) / 2f + Vector3.forward;
      boxCollider.size = vector3_2 - vector3_1;
      boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.0f);
    }
  }

  private void UpdateFollowers() => this.followers.UpdateFollowPosition();

  [Serializable]
  public class MessageInfo
  {
    public Transform messagesTopLeft;
    public Transform messagesBottomRight;
  }

  [Serializable]
  public class InputInfo
  {
    public Transform inputTopLeft;
    public Transform inputBottomRight;
  }

  [Serializable]
  public class Followers
  {
    public UIBFollowObject playerInfoFollower;
    public UIBFollowObject closeButtonFollower;
    public UIBFollowObject bubbleFollower;

    public void UpdateFollowPosition()
    {
      this.playerInfoFollower.UpdateFollowPosition();
      this.closeButtonFollower.UpdateFollowPosition();
      this.bubbleFollower.UpdateFollowPosition();
    }
  }
}
