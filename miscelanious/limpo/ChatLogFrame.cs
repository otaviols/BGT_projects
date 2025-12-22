using Hearthstone.UI;
using System;
using UnityEngine;

public class ChatLogFrame : MonoBehaviour
{
  public ChatLogFrameBones m_Bones;
  public ChatLogFramePrefabs m_Prefabs;
  public UberText m_NameText;
  public ChatLog m_chatLog;
  public GameObject m_medalPatch;
  public AsyncReference m_rankedMedalWidgetReference;
  private PlayerIcon m_playerIcon;
  private BnetPlayer m_receiver;
  private SelectableMedal m_selectableMedal;
  private Widget m_selectableMedalWidget;

  public BnetPlayer Receiver
  {
    get => this.m_receiver;
    set
    {
      if (this.m_receiver == value)
        return;
      this.m_receiver = value;
      if (this.m_receiver == null)
        return;
      this.m_playerIcon.SetPlayer(this.m_receiver);
      this.UpdateReceiver();
      this.m_chatLog.Receiver = this.m_receiver;
    }
  }

  public bool IsWaitingOnMedal
  {
    get
    {
      if (this.Receiver == null)
        return true;
      MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankedMedalFromRankPresenceField(this.Receiver.GetBestGameAccount());
      if (rankPresenceField == null || !rankPresenceField.IsDisplayable())
        return false;
      return (UnityEngine.Object) this.m_selectableMedalWidget == (UnityEngine.Object) null || this.m_selectableMedalWidget.IsChangingStates;
    }
  }

  private void Awake()
  {
    this.InitPlayerIcon();
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
  }

  private void Start()
  {
    this.m_rankedMedalWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnSelectableMedalWidgetReady));
    this.UpdateLayout();
  }

  private void OnDestroy() => BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));

  public void UpdateLayout() => this.OnResize();

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (changelist.FindChange(this.m_receiver) == null)
      return;
    this.UpdateReceiver();
  }

  private void OnSelectableMedalWidgetReady(Widget widget)
  {
    this.m_selectableMedalWidget = widget;
    this.m_selectableMedal = widget.GetComponentInChildren<SelectableMedal>();
    this.UpdateSelectableMedalWidget();
  }

  private void InitPlayerIcon()
  {
    this.m_playerIcon = UnityEngine.Object.Instantiate<PlayerIcon>(this.m_Prefabs.m_PlayerIcon);
    this.m_playerIcon.transform.parent = this.transform;
    TransformUtil.CopyWorld((Component) this.m_playerIcon, (Component) this.m_Bones.m_PlayerIcon);
    LayerUtils.SetLayer((Component) this.m_playerIcon, this.gameObject.layer);
  }

  private void OnResize()
  {
    float viewWindowMaxValue = this.m_chatLog.messageFrames.ViewWindowMaxValue;
    this.m_chatLog.messageFrames.transform.position = (this.m_Bones.m_MessagesTopLeft.position + this.m_Bones.m_MessagesBottomRight.position) / 2f;
    Vector3 vector3 = this.m_Bones.m_MessagesBottomRight.localPosition - this.m_Bones.m_MessagesTopLeft.localPosition;
    this.m_chatLog.messageFrames.ClipSize = new Vector2(vector3.x, Math.Abs(vector3.y));
    this.m_chatLog.messageFrames.ViewWindowMaxValue = viewWindowMaxValue;
    this.m_chatLog.messageFrames.ScrollValue = Mathf.Clamp01(this.m_chatLog.messageFrames.ScrollValue);
    this.m_chatLog.OnResize();
  }

  private void UpdateReceiver()
  {
    this.m_playerIcon.UpdateIcon();
    this.m_NameText.Text = FriendUtils.GetUniqueNameWithColor(this.m_receiver);
    MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankedMedalFromRankPresenceField(this.m_receiver.GetBestGameAccount());
    if (this.m_receiver != null && this.m_receiver.IsDisplayable() && this.m_receiver.IsOnline())
    {
      if (rankPresenceField == null || !rankPresenceField.IsDisplayable())
        this.m_playerIcon.Show();
      else
        this.m_playerIcon.Hide();
    }
    else if (!this.m_receiver.IsOnline())
      this.m_playerIcon.Show();
    this.UpdateSelectableMedalWidget();
  }

  private void UpdateSelectableMedalWidget() => this.m_selectableMedal?.UpdateWidget(this.m_receiver, onDisplayRankedMedal: ((Action) (() => this.m_medalPatch.SetActive(true))), onDisplayNoMedal: ((Action) (() => this.m_medalPatch.SetActive(false))));
}
