using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaconParty : MonoBehaviour
{
  public const string ShowKickButtonEvent = "ShowKickButton";
  public const string HideKickButtonEvent = "HideKickButton";
  private const float SpectateEffectFxTime = 0.25f;
  public AsyncReference m_PartyPanelReference;
  public AsyncReference m_Member0;
  public AsyncReference m_Member1;
  public AsyncReference m_Member2;
  public AsyncReference m_Member3;
  public AsyncReference m_Member4;
  public AsyncReference m_Member5;
  public AsyncReference m_Member6;
  public AsyncReference m_Member7;
  public GameObject m_ClickBlocker;
  private Widget m_partyPanel;
  private List<Widget> m_members;
  private Dictionary<int, Widget> m_memberWidgetByWidgetIndex = new Dictionary<int, Widget>();
  private List<BaconParty.BaconPartyMemberInfo> m_memberInfo;
  private Queue<BaconParty.AnimatedEvent> m_animQueue;
  private bool m_animating;
  private bool m_panelLoaded;
  private int m_membersLoaded;
  private static BaconParty s_instance;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public static BaconParty Get() => BaconParty.s_instance;

  public void Start()
  {
    BaconParty.s_instance = this;
    this.m_animQueue = new Queue<BaconParty.AnimatedEvent>();
    this.m_members = new List<Widget>(PartyManager.BATTLEGROUNDS_PARTY_LIMIT);
    this.m_memberInfo = new List<BaconParty.BaconPartyMemberInfo>(PartyManager.BATTLEGROUNDS_PARTY_LIMIT);
    this.m_panelLoaded = false;
    this.m_membersLoaded = 0;
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    for (int index = 0; index < PartyManager.BATTLEGROUNDS_PARTY_LIMIT; ++index)
    {
      this.m_members.Add((Widget) null);
      this.m_memberInfo.Add(new BaconParty.BaconPartyMemberInfo());
    }
    this.m_PartyPanelReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnPartyPanelReady));
    this.m_Member0.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 0)));
    this.m_Member1.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 1)));
    this.m_Member2.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 2)));
    this.m_Member3.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 3)));
    this.m_Member4.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 4)));
    this.m_Member5.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 5)));
    this.m_Member6.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 6)));
    this.m_Member7.RegisterReadyListener<Widget>((System.Action<Widget>) (c => this.OnMemberReady(c, 7)));
    PartyManager.Get().AddChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    PartyManager.Get().AddMemberAttributeChangedListener(new PartyManager.MemberAttributeChangedCallback(this.OnMemberAttributeChange));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPresenceUpdated));
    BnetNearbyPlayerMgr.Get().AddChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersUpdated));
    SpectatorManager.Get().OnSpectateRejected += new System.Action(this.OnSpectateRejected);
    this.StartCoroutine(this.ReconcileWhenReady());
  }

  public void OnDestroy()
  {
    if (PartyManager.Get() != null)
    {
      PartyManager.Get().RemoveChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
      PartyManager.Get().RemoveMemberAttributeChangedListener(new PartyManager.MemberAttributeChangedCallback(this.OnMemberAttributeChange));
    }
    if (BnetPresenceMgr.Get() != null)
      BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPresenceUpdated));
    if (BnetNearbyPlayerMgr.Get() != null)
      BnetNearbyPlayerMgr.Get().RemoveChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersUpdated));
    if (SpectatorManager.Get() == null)
      return;
    SpectatorManager.Get().OnSpectateRejected -= new System.Action(this.OnSpectateRejected);
  }

  private bool IsLoadedAndReady() => this.m_panelLoaded && this.m_membersLoaded == PartyManager.BATTLEGROUNDS_PARTY_LIMIT;

  private IEnumerator ReconcileWhenReady()
  {
    while (!this.IsLoadedAndReady())
      yield return (object) new WaitForEndOfFrame();
    if (PartyManager.Get().IsInBattlegroundsParty())
    {
      while (PartyManager.Get().GetCurrentAndPendingPartyMembers().Count == 0)
        yield return (object) new WaitForEndOfFrame();
    }
    if (PartyManager.Get().GetCurrentPartySize() == 1)
      this.LeaveParty();
    this.RefreshDisplay();
    this.UpdateDataModelData();
    FriendChallengeMgr.Get().UpdateMyAvailability();
  }

  private void OnPartyChanged(
    PartyManager.PartyInviteEvent inviteEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData data,
    object userData)
  {
    Log.Party.PrintDebug("BaconParty.OnPartyChanged(): Event={0}, gameAccountId={1}", (object) inviteEvent, (object) playerGameAccountId);
    switch (inviteEvent)
    {
      case PartyManager.PartyInviteEvent.I_CREATED_PARTY:
        this.RefreshDisplay();
        break;
      case PartyManager.PartyInviteEvent.I_SENT_INVITE:
      case PartyManager.PartyInviteEvent.FRIEND_RECEIVED_INVITE:
        this.AddPartyMember(playerGameAccountId);
        break;
      case PartyManager.PartyInviteEvent.I_RESCINDED_INVITE:
      case PartyManager.PartyInviteEvent.FRIEND_DECLINED_INVITE:
      case PartyManager.PartyInviteEvent.INVITE_EXPIRED:
      case PartyManager.PartyInviteEvent.FRIEND_LEFT:
        this.RemovePartyMember(playerGameAccountId);
        break;
      case PartyManager.PartyInviteEvent.FRIEND_ACCEPTED_INVITE:
        this.SetReady(playerGameAccountId);
        break;
      case PartyManager.PartyInviteEvent.I_ACCEPTED_INVITE:
        this.StartCoroutine(this.ReconcileWhenReady());
        break;
      case PartyManager.PartyInviteEvent.LEADER_DISSOLVED_PARTY:
        this.RefreshDisplay();
        break;
    }
    this.UpdateDataModelData();
  }

  private void OnMemberAttributeChange(
    BnetGameAccountId playerGameAccountId,
    Blizzard.GameService.Protocol.V2.Client.Attribute attribute,
    object userData)
  {
    if (!PartyManager.Get().IsPlayerInCurrentParty(playerGameAccountId))
      return;
    BaconParty.BaconPartyMemberInfo baconPartyMemberInfo = this.m_memberInfo.Find((Predicate<BaconParty.BaconPartyMemberInfo>) (Info => (BnetEntityId) Info.playerGameAccountId == (BnetEntityId) playerGameAccountId));
    if (baconPartyMemberInfo == null)
      return;
    BaconParty.Status statusForPartyMember = BaconParty.GetReadyStatusForPartyMember(playerGameAccountId);
    if (baconPartyMemberInfo.status == BaconParty.Status.Waiting && statusForPartyMember == BaconParty.Status.NotReady)
      return;
    baconPartyMemberInfo.status = statusForPartyMember;
  }

  private void OnPresenceUpdated(BnetPlayerChangelist changelist, object userData)
  {
    List<BnetPlayer> changedPlayers = new List<BnetPlayer>();
    foreach (BnetPlayerChange change in changelist.GetChanges())
    {
      BnetPlayer player = change.GetPlayer();
      changedPlayers.Add(player);
    }
    this.UpdateChangedPlayersFromPresenceUpdate(changedPlayers);
  }

  private void OnNearbyPlayersUpdated(
    BnetRecentOrNearbyPlayerChangelist changelist,
    object userData)
  {
    this.UpdateChangedPlayersFromPresenceUpdate(changelist.GetUpdatedStrangers());
  }

  private void UpdateChangedPlayersFromPresenceUpdate(List<BnetPlayer> changedPlayers)
  {
    if (changedPlayers == null)
      return;
    bool flag = false;
    foreach (BnetPlayer changedPlayer in changedPlayers)
    {
      if (changedPlayer == BnetPresenceMgr.Get().GetMyPlayer() && changedPlayer.IsAppearingOffline())
      {
        this.LeaveParty();
        this.UpdateDataModelData();
        return;
      }
      if (PartyManager.Get().IsPlayerInCurrentPartyOrPending(changedPlayer.GetBestGameAccountId()))
      {
        for (int index = 0; index < this.m_memberInfo.Count; ++index)
        {
          if ((BnetEntityId) this.m_memberInfo[index].playerGameAccountId == (BnetEntityId) changedPlayer.GetBestGameAccountId())
          {
            this.m_memberInfo[index].status = BaconParty.GetReadyStatusForPartyMember(this.m_memberInfo[index].playerGameAccountId);
            flag = true;
            break;
          }
        }
      }
    }
    if (!flag)
      return;
    this.RefreshVisuals();
  }

  private void OnSpectateRejected() => this.CleanUpSpectateFx();

  private void OnPartyPanelReady(Widget controller)
  {
    this.m_partyPanel = controller;
    this.m_panelLoaded = true;
  }

  private void OnMemberReady(Widget widget, int index)
  {
    this.m_memberWidgetByWidgetIndex.Add(index, widget);
    this.m_members[index] = widget;
    widget.RegisterEventListener((Widget.EventListenerDelegate) (s => this.OnPartyMemberEvent(index, s)));
    ++this.m_membersLoaded;
  }

  private void OnPartyMemberEvent(int index, string eventString)
  {
    if (!this.m_memberWidgetByWidgetIndex.ContainsKey(index))
      Debug.LogErrorFormat("OnPartyMemberEvent() - No party member widget at index {0}", (object) index);
    else if (eventString == "SPECTATE_BUTTON_PRESSED")
    {
      int index1 = this.m_members.IndexOf(this.m_memberWidgetByWidgetIndex[index]);
      if (index1 == -1)
      {
        Debug.LogErrorFormat("OnPartyMemberEvent() - Widget at index {0} not found in m_members list.", (object) index);
      }
      else
      {
        this.m_ClickBlocker.SetActive(true);
        this.StartCoroutine(this.SpectatePlayerWithAnimations(this.m_memberInfo[index1].playerGameAccountId));
      }
    }
    else
    {
      if (!(eventString == "KICK_BUTTON_PRESSED"))
        return;
      int index2 = this.m_members.IndexOf(this.m_memberWidgetByWidgetIndex[index]);
      if (index2 == -1)
      {
        Debug.LogErrorFormat("OnPartyMemberEvent() - Widget at index {0} not found in m_members list.", (object) index);
      }
      else
      {
        BnetGameAccountId playerGameAccountId = this.m_memberInfo[index2].playerGameAccountId;
        PartyManager.Get().KickPlayerFromParty(playerGameAccountId);
      }
    }
  }

  public BaconPartyDataModel GetBaconPartyDataModel()
  {
    IDataModel model;
    if (!this.m_partyPanel.GetDataModel(154, out model))
    {
      model = (IDataModel) new BaconPartyDataModel();
      this.m_partyPanel.BindDataModel(model);
    }
    return model as BaconPartyDataModel;
  }

  private void RefreshDisplay()
  {
    if (!PartyManager.Get().IsInBattlegroundsParty())
      return;
    BnetGameAccountId leaderGameAccountId = PartyManager.Get().GetLeader();
    BnetGameAccountId myselfGameAccountId = BnetPresenceMgr.Get().GetMyPlayer().GetBestGameAccountId();
    List<BnetGameAccountId> pendingPartyMembers = PartyManager.Get().GetCurrentAndPendingPartyMembers();
    if (!PartyManager.Get().IsPartyLeader())
      pendingPartyMembers.Sort((Comparison<BnetGameAccountId>) ((m1, m2) =>
      {
        if ((BnetEntityId) m1 == (BnetEntityId) leaderGameAccountId)
          return -1;
        if ((BnetEntityId) m2 == (BnetEntityId) leaderGameAccountId)
          return 1;
        if ((BnetEntityId) m1 == (BnetEntityId) myselfGameAccountId)
          return -1;
        return (BnetEntityId) m2 == (BnetEntityId) myselfGameAccountId ? 1 : 0;
      }));
    for (int index = 0; index < PartyManager.BATTLEGROUNDS_PARTY_LIMIT; ++index)
    {
      BnetGameAccountId playerGameAccountId = (BnetGameAccountId) null;
      if (index < pendingPartyMembers.Count)
        playerGameAccountId = pendingPartyMembers[index];
      if (PartyManager.Get().IsPlayerInCurrentParty(playerGameAccountId))
      {
        this.m_memberInfo[index].status = BaconParty.GetReadyStatusForPartyMember(playerGameAccountId);
        this.m_memberInfo[index].playerGameAccountId = playerGameAccountId;
      }
      else if ((BnetEntityId) playerGameAccountId != (BnetEntityId) null)
      {
        this.m_memberInfo[index].status = BaconParty.Status.Waiting;
        this.m_memberInfo[index].playerGameAccountId = playerGameAccountId;
      }
      else
      {
        this.m_memberInfo[index].status = BaconParty.Status.Inactive;
        this.m_memberInfo[index].playerGameAccountId = (BnetGameAccountId) null;
      }
    }
    this.RefreshVisuals();
  }

  private void UpdateDataModelData()
  {
    if (!this.IsLoadedAndReady())
      return;
    BaconPartyDataModel baconPartyDataModel = this.GetBaconPartyDataModel();
    baconPartyDataModel.Active = PartyManager.Get().IsInBattlegroundsParty();
    baconPartyDataModel.Size = PartyManager.Get().GetCurrentPartySize();
    baconPartyDataModel.PrivateGame = baconPartyDataModel.Size > PartyManager.Get().GetBattlegroundsMaxRankedPartySize();
  }

  public void LeaveParty() => PartyManager.Get().LeaveParty();

  private void AddPartyMember(BnetGameAccountId playerGameAccountId, bool isReady = false)
  {
    if (PartyManager.Get().GetCurrentPartySize() > PartyManager.BATTLEGROUNDS_PARTY_LIMIT)
      return;
    this.m_animQueue.Enqueue(new BaconParty.AnimatedEvent()
    {
      type = BaconParty.Event.Add,
      playerGameAccountId = playerGameAccountId,
      isReady = isReady
    });
    this.Animate();
  }

  private void Animate()
  {
    if (this.m_animating || this.m_animQueue.Count == 0)
      return;
    BaconParty.AnimatedEvent animatedEvent = this.m_animQueue.Dequeue();
    switch (animatedEvent.type)
    {
      case BaconParty.Event.Add:
        this.StartCoroutine(this.AddPartyMemberWithAnims(animatedEvent.playerGameAccountId, animatedEvent.isReady));
        break;
      case BaconParty.Event.Remove:
        this.StartCoroutine(this.RemovePartyMemberWithAnims(animatedEvent.playerGameAccountId));
        break;
    }
  }

  private IEnumerator AddPartyMemberWithAnims(
    BnetGameAccountId playerGameAccountId,
    bool isReady)
  {
    this.m_animating = true;
    while (!this.IsLoadedAndReady())
      yield return (object) null;
    int index1 = -1;
    for (int index2 = 0; index2 < this.m_memberInfo.Count; ++index2)
    {
      if (this.m_memberInfo[index2].status == BaconParty.Status.Inactive)
      {
        index1 = index2;
        break;
      }
    }
    if (index1 == -1)
    {
      Log.Party.PrintError("AddPartyMemberWithAnims - No inactive members, unable to add new member.");
      this.m_animating = false;
      this.Animate();
    }
    else
    {
      this.m_memberInfo[index1].status = isReady ? BaconParty.GetReadyStatusForPartyMember(playerGameAccountId) : BaconParty.Status.Waiting;
      this.m_memberInfo[index1].playerGameAccountId = playerGameAccountId;
      this.RefreshVisuals();
      yield return (object) new WaitForSeconds(0.5f);
      this.m_animating = false;
      this.Animate();
    }
  }

  private void RemovePartyMember(BnetGameAccountId playerGameAccountId)
  {
    if (PartyManager.Get().GetCurrentPartySize() == 1)
    {
      this.LeaveParty();
    }
    else
    {
      this.m_animQueue.Enqueue(new BaconParty.AnimatedEvent()
      {
        type = BaconParty.Event.Remove,
        playerGameAccountId = playerGameAccountId
      });
      this.Animate();
    }
  }

  private IEnumerator RemovePartyMemberWithAnims(BnetGameAccountId playerGameAccountId)
  {
    this.m_animating = true;
    while (!this.IsLoadedAndReady())
      yield return (object) null;
    int index = this.GetIndexOfPartyMemberFromGameAccountId(playerGameAccountId);
    if (index < 0 || index >= PartyManager.BATTLEGROUNDS_PARTY_LIMIT)
    {
      Log.Party.PrintError("RemovePartyMemberWithAnims() - Unable to find party member with id {0}.", (object) playerGameAccountId);
      this.m_animating = false;
      this.Animate();
    }
    else
    {
      this.m_members[index].TriggerEvent("Leave");
      yield return (object) new WaitForSeconds(0.5f);
      Vector3[] positions = new Vector3[PartyManager.BATTLEGROUNDS_PARTY_LIMIT];
      for (int index1 = index; index1 < PartyManager.BATTLEGROUNDS_PARTY_LIMIT; ++index1)
        positions[index1] = new Vector3(this.m_members[index1].gameObject.transform.localPosition.x, this.m_members[index1].gameObject.transform.localPosition.y, this.m_members[index1].gameObject.transform.localPosition.z);
      for (int index2 = index + 1; index2 < PartyManager.BATTLEGROUNDS_PARTY_LIMIT; ++index2)
        iTween.MoveTo(this.m_members[index2].gameObject, new Hashtable()
        {
          [(object) "easetype"] = (object) "easeOutBounce",
          [(object) "position"] = (object) positions[index2 - 1],
          [(object) "islocal"] = (object) true,
          [(object) "time"] = (object) 0.5f
        });
      yield return (object) new WaitForSeconds(0.5f);
      this.m_memberInfo.RemoveAt(index);
      this.m_memberInfo.Add(new BaconParty.BaconPartyMemberInfo()
      {
        status = BaconParty.Status.Inactive
      });
      Widget member = this.m_members[index];
      this.m_members.RemoveAt(index);
      this.m_members.Add(member);
      member.transform.localPosition = positions[PartyManager.BATTLEGROUNDS_PARTY_LIMIT - 1];
      this.m_animating = false;
      this.Animate();
    }
  }

  private int GetIndexOfPartyMemberFromGameAccountId(BnetGameAccountId playerGameAccountId)
  {
    for (int index = 0; index < PartyManager.BATTLEGROUNDS_PARTY_LIMIT; ++index)
    {
      if (this.m_memberInfo[index] != null && (BnetEntityId) this.m_memberInfo[index].playerGameAccountId == (BnetEntityId) playerGameAccountId)
        return index;
    }
    return -1;
  }

  private void SetReady(BnetGameAccountId playerGameAccountId)
  {
    int index1 = -1;
    for (int index2 = 0; index2 < this.m_memberInfo.Count; ++index2)
    {
      if ((BnetEntityId) this.m_memberInfo[index2].playerGameAccountId == (BnetEntityId) playerGameAccountId)
      {
        index1 = index2;
        break;
      }
    }
    if (index1 == -1)
    {
      this.AddPartyMember(playerGameAccountId, true);
    }
    else
    {
      if (index1 <= 0 || index1 >= PartyManager.BATTLEGROUNDS_PARTY_LIMIT)
        return;
      this.m_memberInfo[index1].status = BaconParty.Status.Ready;
      this.m_members[index1].TriggerEvent(BaconParty.Status.Ready.ToString());
    }
  }

  private void RefreshVisuals()
  {
    for (int index = 0; index < this.m_members.Count; ++index)
    {
      this.m_members[index].TriggerEvent(this.m_memberInfo[index].status.ToString());
      if ((BnetEntityId) this.m_memberInfo[index].playerGameAccountId != (BnetEntityId) null)
      {
        string partyMemberName = PartyManager.Get().GetPartyMemberName(this.m_memberInfo[index].playerGameAccountId);
        this.m_members[index].transform.Find("BaconPartyMember/Root/Name").gameObject.GetComponent<UberText>().Text = partyMemberName;
        if (PartyManager.Get().IsPartyLeader() && (BnetEntityId) BattleNet.GetMyGameAccountId() != (BnetEntityId) this.m_memberInfo[index].playerGameAccountId)
          this.m_members[index].TriggerEvent("ShowKickButton");
        else
          this.m_members[index].TriggerEvent("HideKickButton");
      }
    }
  }

  public static BaconParty.Status GetReadyStatusForPartyMember(
    BnetGameAccountId playerGameAccountId)
  {
    BnetPlayer player = BnetUtils.GetPlayer(playerGameAccountId);
    bool flag = BnetFriendMgr.Get().IsFriend(player) || BnetNearbyPlayerMgr.Get().IsNearbyPlayer(player);
    if ((BnetEntityId) playerGameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())
      return !PartyManager.Get().IsPartyLeader() ? BaconParty.Status.Ready : BaconParty.Status.Leader;
    if (!PartyManager.Get().IsPlayerInCurrentParty(playerGameAccountId))
      return BaconParty.Status.NotReady;
    if (PartyManager.Get().CanSpectatePartyMember(playerGameAccountId))
      return BaconParty.Status.Spectate;
    if (player == null || !flag)
      return BaconParty.Status.Ready;
    return (BnetEntityId) PartyManager.Get().GetLeader() == (BnetEntityId) playerGameAccountId ? (!FriendChallengeMgr.Get().IsOpponentAvailable(player) ? BaconParty.Status.NotReady : BaconParty.Status.Leader) : (FriendChallengeMgr.Get().IsOpponentAvailable(player) || Network.Get().IsFindingGame() ? BaconParty.Status.Ready : BaconParty.Status.NotReady);
  }

  private IEnumerator SpectatePlayerWithAnimations(BnetGameAccountId playerGameAccountId)
  {
    this.m_screenEffectsHandle.StartEffect(new ScreenEffectParameters(ScreenEffectType.BLUR | ScreenEffectType.DESATURATE, time: 0.25f, easeType: iTween.EaseType.linear, desaturate: new DesaturateParameters?(new DesaturateParameters())));
    yield return (object) new WaitForSeconds(0.25f);
    if (!PartyManager.Get().SpectatePartyMember(playerGameAccountId))
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_BACON_PARTY_SPECTATE_ERROR_HEADER"),
        m_text = GameStrings.Get("GLUE_BACON_PARTY_SPECTATE_ERROR_BODY"),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_showAlertIcon = false,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
      this.CleanUpSpectateFx();
    }
    this.m_ClickBlocker.SetActive(false);
  }

  private void CleanUpSpectateFx() => this.m_screenEffectsHandle.StopEffect();

  private class BaconPartyMemberInfo
  {
    public BaconParty.Status status;
    public BnetGameAccountId playerGameAccountId;
  }

  public enum Status
  {
    Inactive,
    Waiting,
    Ready,
    Leader,
    Leave,
    NotReady,
    Spectate,
  }

  private class AnimatedEvent
  {
    public BaconParty.Event type;
    public BnetGameAccountId playerGameAccountId;
    public bool isReady;
  }

  private enum Event
  {
    Add,
    Remove,
    Ready,
  }
}
