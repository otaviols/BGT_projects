using Blizzard.T5.Core;
using PegasusGame;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ZoneMgr : MonoBehaviour
{
  private Map<System.Type, string> m_tweenNames = new Map<System.Type, string>()
  {
    {
      typeof (ZoneHand),
      "ZoneHandUpdateLayout"
    },
    {
      typeof (ZonePlay),
      "ZonePlayUpdateLayout"
    },
    {
      typeof (ZoneWeapon),
      "ZoneWeaponUpdateLayout"
    },
    {
      typeof (ZoneBattlegroundHeroBuddy),
      "ZoneBattlegroundHeroBuddyUpdateLayout"
    },
    {
      typeof (ZoneBattlegroundQuestReward),
      "ZoneBattlegroundQuestRewardUpdateLayout"
    }
  };
  private static ZoneMgr s_instance;
  private List<Zone> m_zones;
  private int m_nextLocalChangeListId = 1;
  private int m_nextServerChangeListId = 1;
  private Queue<ZoneChangeList> m_pendingServerChangeLists = new Queue<ZoneChangeList>();
  private ZoneChangeList m_activeServerChangeList;
  private Map<int, Entity> m_tempEntityMap = new Map<int, Entity>();
  private Map<Zone, ZoneMgr.TempZone> m_tempZoneMap = new Map<Zone, ZoneMgr.TempZone>();
  private List<ZoneChangeList> m_activeLocalChangeLists = new List<ZoneChangeList>();
  private List<ZoneChangeList> m_pendingLocalChangeLists = new List<ZoneChangeList>();
  private QueueList<ZoneChangeList> m_localChangeListHistory = new QueueList<ZoneChangeList>();
  private bool m_doAutoCorrection;
  private float m_nextDeathBlockLayoutDelaySec;
  private LettuceZoneController m_lettuceZoneController;
  private CancellationTokenSource m_updateChangeCancelTokenSource;

  private void Awake()
  {
    ZoneMgr.s_instance = this;
    this.m_updateChangeCancelTokenSource = new CancellationTokenSource();
    this.m_zones = new List<Zone>();
    this.gameObject.GetComponentsInChildren<Zone>(this.m_zones);
    if (GameState.Get() == null)
      return;
    GameState.Get().RegisterCurrentPlayerChangedListener(new GameState.CurrentPlayerChangedCallback(this.OnCurrentPlayerChanged));
    GameState.Get().RegisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected));
    this.m_lettuceZoneController = new LettuceZoneController(GameState.Get(), InputManager.Get());
  }

  private void Start()
  {
    InputManager inputManager = InputManager.Get();
    if (!((UnityEngine.Object) inputManager != (UnityEngine.Object) null))
      return;
    inputManager.StartWatchingForInput();
  }

  private void Update()
  {
    this.UpdateLocalChangeLists(this.m_updateChangeCancelTokenSource.Token);
    this.UpdateServerChangeLists(this.m_updateChangeCancelTokenSource.Token);
  }

  private void OnDestroy()
  {
    if (GameState.Get() != null)
    {
      GameState.Get().UnregisterCurrentPlayerChangedListener(new GameState.CurrentPlayerChangedCallback(this.OnCurrentPlayerChanged));
      GameState.Get().UnregisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected));
    }
    ZoneMgr.s_instance = (ZoneMgr) null;
    this.m_zones = (List<Zone>) null;
    if (this.m_updateChangeCancelTokenSource == null)
      return;
    this.m_updateChangeCancelTokenSource.Cancel();
    this.m_updateChangeCancelTokenSource.Dispose();
  }

  public static ZoneMgr Get() => ZoneMgr.s_instance;

  public List<Zone> GetZones() => this.m_zones;

  public Zone FindZoneForTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    if (controllerId == 0)
      return (Zone) null;
    if (zoneTag == TAG_ZONE.INVALID)
      return (Zone) null;
    foreach (Zone zone in this.m_zones)
    {
      if (zone.CanAcceptTags(controllerId, zoneTag, cardType, entity))
        return zone;
    }
    return (Zone) null;
  }

  public Zone FindZoneForEntity(Entity entity)
  {
    if (entity.GetZone() == TAG_ZONE.INVALID)
      return (Zone) null;
    foreach (Zone zone in this.m_zones)
    {
      if (zone.CanAcceptTags(entity.GetControllerId(), entity.GetZone(), entity.GetCardType(), entity))
        return zone;
    }
    return (Zone) null;
  }

  public Zone FindZoneForEntityAndZoneTag(Entity entity, TAG_ZONE zoneTag)
  {
    if (zoneTag == TAG_ZONE.INVALID)
      return (Zone) null;
    foreach (Zone zone in this.m_zones)
    {
      if (zone.CanAcceptTags(entity.GetControllerId(), zoneTag, entity.GetCardType(), entity))
        return zone;
    }
    return (Zone) null;
  }

  public T FindZoneOfType<T>(Player.Side side) where T : Zone
  {
    System.Type type = typeof (T);
    foreach (Zone zone in this.m_zones)
    {
      if (!(((object) zone).GetType() != type) && zone.m_Side == side)
        return (T) zone;
    }
    return default (T);
  }

  public List<Zone> FindZonesForSide(Player.Side playerSide) => this.FindZonesOfType<Zone>(playerSide);

  public List<T> FindZonesOfType<T>() where T : Zone => this.FindZonesOfType<T, T>();

  public List<ReturnType> FindZonesOfType<ReturnType, ArgType>()
    where ReturnType : Zone
    where ArgType : Zone
  {
    List<ReturnType> zonesOfType = new List<ReturnType>();
    System.Type type = typeof (ArgType);
    foreach (Zone zone in this.m_zones)
    {
      if (!(((object) zone).GetType() != type))
        zonesOfType.Add((ReturnType) zone);
    }
    return zonesOfType;
  }

  public List<T> FindZonesOfType<T>(Player.Side side) where T : Zone => this.FindZonesOfType<T, T>(side);

  public List<ReturnType> FindZonesOfType<ReturnType, ArgType>(Player.Side side)
    where ReturnType : Zone
    where ArgType : Zone
  {
    List<ReturnType> zonesOfType = new List<ReturnType>();
    foreach (Zone zone in this.m_zones)
    {
      if (zone is ArgType && zone.m_Side == side)
        zonesOfType.Add((ReturnType) zone);
    }
    return zonesOfType;
  }

  public List<Zone> FindZonesForTag(TAG_ZONE zoneTag)
  {
    List<Zone> zonesForTag = new List<Zone>();
    foreach (Zone zone in this.m_zones)
    {
      if (zone.m_ServerTag == zoneTag)
        zonesForTag.Add(zone);
    }
    return zonesForTag;
  }

  public Map<System.Type, string> GetTweenNames() => this.m_tweenNames;

  public string GetTweenName<T>() where T : Zone
  {
    System.Type key = typeof (T);
    string tweenName = "";
    this.m_tweenNames.TryGetValue(key, out tweenName);
    return tweenName;
  }

  public void RequestNextDeathBlockLayoutDelaySec(float sec) => this.m_nextDeathBlockLayoutDelaySec = Mathf.Max(this.m_nextDeathBlockLayoutDelaySec, sec);

  public float RemoveNextDeathBlockLayoutDelaySec()
  {
    double blockLayoutDelaySec = (double) this.m_nextDeathBlockLayoutDelaySec;
    this.m_nextDeathBlockLayoutDelaySec = 0.0f;
    return (float) blockLayoutDelaySec;
  }

  public int PredictZonePosition(Zone zone, int pos)
  {
    ZoneMgr.TempZone tempZone = this.BuildTempZone(zone);
    this.PredictZoneFromPowerProcessor(tempZone);
    this.RemoveDraggedMinionsFromTempZone(zone, tempZone);
    int insertionPosition = this.FindBestMinionInsertionPosition(tempZone, pos - 1, pos);
    int num = this.ValidatePredictedMinion(tempZone, insertionPosition);
    this.m_tempZoneMap.Clear();
    this.m_tempEntityMap.Clear();
    return num;
  }

  private void RemoveDraggedMinionsFromTempZone(Zone originalZone, ZoneMgr.TempZone tempZone)
  {
    foreach (Card card in originalZone.GetCards())
    {
      if (card.IsBeingDragged)
        tempZone.RemoveEntityById(card.GetEntity().GetEntityId());
    }
  }

  public bool HasPredictedCards() => this.HasPredictedCards<ZoneSecret>(TAG_ZONE.SECRET) || this.HasPredictedCards<ZoneWeapon>(TAG_ZONE.PLAY) || this.HasPredictedCards<ZoneHero>(TAG_ZONE.PLAY) || this.HasPredictedCards<ZoneGraveyard>(TAG_ZONE.GRAVEYARD);

  public bool HasPredictedMovedMinion()
  {
    foreach (Zone zone in this.FindZonesOfType<Zone>(Player.Side.FRIENDLY))
    {
      foreach (Card card in zone.GetCards())
      {
        if (card.m_minionWasMovedFromSrcToDst != null)
          return true;
      }
    }
    return false;
  }

  private bool IsCardInValidZoneForPredictedPosition(Card card, Zone zone)
  {
    Entity entity = card.GetEntity();
    if (entity == null || !(zone is ZonePlay))
      return true;
    return entity.GetZone() != TAG_ZONE.GRAVEYARD && entity.GetZone() != TAG_ZONE.SETASIDE;
  }

  public bool HasPredictedPositions()
  {
    foreach (Zone zone in this.FindZonesOfType<Zone>(Player.Side.FRIENDLY))
    {
      foreach (Card card in zone.GetCards())
      {
        if (this.IsCardInValidZoneForPredictedPosition(card, zone) && card.GetPredictedZonePosition() != 0)
          return true;
      }
    }
    return false;
  }

  public bool HasPredictedCards<T>(TAG_ZONE predictedZone) where T : Zone
  {
    foreach (T obj in this.FindZonesOfType<T>(Player.Side.FRIENDLY))
    {
      foreach (Card card in obj.GetCards())
      {
        if (card.GetEntity().GetZone() != predictedZone)
          return true;
      }
    }
    return false;
  }

  public void DebugPrintZonePos()
  {
    foreach (Zone zone in this.FindZonesOfType<ZonePlay>(Player.Side.FRIENDLY))
    {
      foreach (Card card in zone.GetCards())
      {
        Entity entity = card.GetEntity();
        int zonePosition1 = card.GetZonePosition();
        int zonePosition2 = entity.GetZonePosition();
        int timeZonePosition = entity.GetRealTimeZonePosition();
        Debug.Log((object) string.Format("card : {0} cp: {1} ep: {2} rep: {3}", (object) entity.GetName(), (object) zonePosition1, (object) zonePosition2, (object) timeZonePosition));
      }
    }
  }

  public bool ShouldIgnorePosChange() => GameState.Get().GetGameEntity().IsCurrentTurnRealTime() && this.IsBattlegroundShoppingPhase();

  public bool IsBattlegroundShoppingPhase() => GameState.Get()?.GetGameEntity() is TB_BaconShop gameEntity && gameEntity.IsShopPhase();

  public void OnRealTimeZonePosChange(Entity entity)
  {
    if (!this.ShouldIgnorePosChange())
      return;
    ZoneChangeList changeList = (ZoneChangeList) null;
    Card card = entity.GetCard();
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("[OnRealTimeZonePosChange] - entity [{0}]'s card is null", (object) entity));
    }
    else
    {
      Zone zone = card.GetZone();
      if ((UnityEngine.Object) zone == (UnityEngine.Object) null || card.IsMagneticTarget() || entity.GetRealTimeZone() != zone.m_ServerTag || entity.HasQueuedControllerTagChange())
        return;
      int zonePosition = card.GetZonePosition();
      int timeZonePosition = entity.GetRealTimeZonePosition();
      if (zonePosition == timeZonePosition)
        return;
      ZoneChange change = new ZoneChange();
      change.SetEntity(entity);
      change.SetSourceZone(zone);
      change.SetSourceZoneTag(zone.m_ServerTag);
      change.SetSourcePosition(zonePosition);
      change.SetDestinationPosition(timeZonePosition);
      change.SetDestinationZone(zone);
      change.SetDestinationZoneTag(zone.m_ServerTag);
      if (changeList == null)
      {
        int localChangeListId = this.GetNextLocalChangeListId();
        changeList = new ZoneChangeList();
        changeList.SetId(localChangeListId);
        changeList.AddChange(change);
      }
      if (changeList == null)
        return;
      this.ProcessLocalChangeList(changeList, this.m_updateChangeCancelTokenSource.Token);
    }
  }

  public IEnumerable<ZoneChangeList> GetActivateLocalChangeList() => (IEnumerable<ZoneChangeList>) this.m_activeLocalChangeLists;

  public bool HasActiveLocalChange() => this.m_activeLocalChangeLists.Count > 0;

  public bool HasPendingLocalChange() => this.m_pendingLocalChangeLists.Count > 0;

  public bool HasUnresolvedLocalChange() => this.m_localChangeListHistory.Count > 0;

  public bool HasTriggeredActiveLocalChange(Card card) => this.FindTriggeredActiveLocalChangeIndex(card) >= 0;

  public ZoneChangeList AddLocalZoneChange(Card triggerCard, TAG_ZONE zoneTag)
  {
    Zone entityAndZoneTag = this.FindZoneForEntityAndZoneTag(triggerCard.GetEntity(), zoneTag);
    return this.AddLocalZoneChange(triggerCard, entityAndZoneTag, zoneTag, 0, (ZoneMgr.ChangeCompleteCallback) null, (object) null);
  }

  public ZoneChangeList AddLocalZoneChange(
    Card triggerCard,
    Zone destinationZone,
    int destinationPos)
  {
    if (!((UnityEngine.Object) destinationZone == (UnityEngine.Object) null))
      return this.AddLocalZoneChange(triggerCard, destinationZone, destinationZone.m_ServerTag, destinationPos, (ZoneMgr.ChangeCompleteCallback) null, (object) null);
    Debug.LogWarning((object) string.Format("ZoneMgr.AddLocalZoneChange() - illegal zone change to null zone for card {0}", (object) triggerCard));
    return (ZoneChangeList) null;
  }

  public ZoneChangeList AddLocalZoneChange(
    Card triggerCard,
    Zone destinationZone,
    TAG_ZONE destinationZoneTag,
    int destinationPos,
    ZoneMgr.ChangeCompleteCallback callback,
    object userData)
  {
    if (destinationZoneTag == TAG_ZONE.INVALID)
    {
      Debug.LogWarning((object) string.Format("ZoneMgr.AddLocalZoneChange() - illegal zone change to {0} for card {1}", (object) destinationZoneTag, (object) triggerCard));
      return (ZoneChangeList) null;
    }
    if ((destinationZone is ZonePlay ? 1 : (destinationZone is ZoneHand ? 1 : 0)) != 0 && destinationPos <= 0)
    {
      Debug.LogWarning((object) string.Format("ZoneMgr.AddLocalZoneChange() - destinationPos {0} is too small for zone {1}, min is 1", (object) destinationPos, (object) destinationZone));
      return (ZoneChangeList) null;
    }
    ZoneChangeList localChangeList = this.CreateLocalChangeList(triggerCard, destinationZone, destinationZoneTag, destinationPos, callback, userData);
    this.ProcessOrEnqueueLocalChangeList(localChangeList, this.m_updateChangeCancelTokenSource.Token);
    this.m_localChangeListHistory.Enqueue(localChangeList);
    return localChangeList;
  }

  public ZoneChangeList AddPredictedLocalZoneChange(
    Card triggerCard,
    Zone destinationZone,
    int destinationPos,
    int predictedPos)
  {
    if ((UnityEngine.Object) triggerCard == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("ZoneMgr.AddPredictedLocalZoneChange() - triggerCard is null"));
      return (ZoneChangeList) null;
    }
    ZoneChangeList zoneChangeList = this.AddLocalZoneChange(triggerCard, destinationZone, destinationPos);
    if (zoneChangeList == null)
      return (ZoneChangeList) null;
    triggerCard.SetPredictedZonePosition(predictedPos);
    zoneChangeList.SetPredictedPosition(predictedPos);
    return zoneChangeList;
  }

  public ZoneChangeList CancelLocalZoneChange(
    ZoneChangeList changeList,
    ZoneMgr.ChangeCompleteCallback callback = null,
    object userData = null)
  {
    if (changeList == null)
    {
      Debug.LogWarning((object) string.Format("ZoneMgr.CancelLocalZoneChange() - changeList is null"));
      return (ZoneChangeList) null;
    }
    if (!this.m_localChangeListHistory.Remove(changeList))
    {
      Debug.LogWarning((object) string.Format("ZoneMgr.CancelLocalZoneChange() - changeList {0} is not in history", (object) changeList.GetId()));
      return (ZoneChangeList) null;
    }
    ZoneChange localTriggerChange = changeList.GetLocalTriggerChange();
    Entity entity = localTriggerChange.GetEntity();
    Card card = entity.GetCard();
    Zone sourceZone = localTriggerChange.GetSourceZone();
    int sourcePosition = localTriggerChange.GetSourcePosition();
    ZoneChangeList localChangeList = this.CreateLocalChangeList(card, sourceZone, sourceZone.m_ServerTag, sourcePosition, callback, userData);
    if (entity.IsHero())
      this.AddOldHeroCanceledChange(localChangeList, card);
    localChangeList.SetCanceledChangeList(true);
    localChangeList.SetZoneInputBlocking(true);
    this.ProcessOrEnqueueLocalChangeList(localChangeList, this.m_updateChangeCancelTokenSource.Token);
    return localChangeList;
  }

  private void AddOldHeroCanceledChange(ZoneChangeList canceledChangeList, Card triggerCard)
  {
    Player controller = triggerCard.GetController();
    Card heroCard = controller.GetHeroCard();
    ZoneChange change = new ZoneChange();
    change.SetParentList(canceledChangeList);
    change.SetEntity(heroCard.GetEntity());
    change.SetDestinationZone((Zone) controller.GetHeroZone());
    change.SetDestinationZoneTag(controller.GetHeroZone().m_ServerTag);
    change.SetDestinationPosition(0);
    Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - AddChange() canceledChangeList: {0},  triggerChange: {1}", (object) canceledChangeList, (object) change);
    canceledChangeList.AddChange(change);
  }

  public static bool IsHandledPower(Network.PowerHistory power)
  {
    switch (power.Type)
    {
      case Network.PowerType.FULL_ENTITY:
        Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
        bool flag = false;
        foreach (Network.Entity.Tag tag in histFullEntity.Entity.Tags)
        {
          if (tag.Name == 202)
          {
            if (tag.Value == 1 || tag.Value == 2)
              return false;
          }
          else if (tag.Name == 49 || tag.Name == 263 || tag.Name == 50 || tag.Name == 1702 || tag.Name == 1703 || tag.Name == 2032)
            flag = true;
        }
        return flag;
      case Network.PowerType.SHOW_ENTITY:
        return true;
      case Network.PowerType.HIDE_ENTITY:
        return true;
      case Network.PowerType.TAG_CHANGE:
        Network.HistTagChange histTagChange = power as Network.HistTagChange;
        if (histTagChange.Tag != 49 && histTagChange.Tag != 263 && histTagChange.Tag != 50 && histTagChange.Tag != 1702 && histTagChange.Tag != 1703 && histTagChange.Tag != 2032)
          return false;
        Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
        return entity == null || !entity.IsPlayer() && !entity.IsGame();
      default:
        return false;
    }
  }

  public bool HasActiveServerChange() => this.m_activeServerChangeList != null;

  public bool HasPendingServerChange() => this.m_pendingServerChangeLists.Count > 0;

  public ZoneChangeList AddServerZoneChanges(
    PowerTaskList taskList,
    int taskStartIndex,
    int taskEndIndex,
    ZoneMgr.ChangeCompleteCallback callback,
    object userData)
  {
    int serverChangeListId = this.GetNextServerChangeListId();
    ZoneChangeList parentList = new ZoneChangeList();
    parentList.SetId(serverChangeListId);
    parentList.SetTaskList(taskList);
    parentList.SetCompleteCallback(callback);
    parentList.SetCompleteCallbackUserData(userData);
    parentList.SetIgnoreCardZonePurePosChanges(this.ShouldIgnorePosChange());
    Log.Zone.Print("ZoneMgr.AddServerZoneChanges() - taskListId={0} changeListId={1} taskStart={2} taskEnd={3}", (object) taskList.GetId(), (object) serverChangeListId, (object) taskStartIndex, (object) taskEndIndex);
    List<PowerTask> taskList1 = taskList.GetTaskList();
    for (int index = taskStartIndex; index <= taskEndIndex; ++index)
    {
      PowerTask powerTask = taskList1[index];
      Network.PowerHistory power = powerTask.GetPower();
      Network.PowerType type = power.Type;
      ZoneChange change;
      switch (type)
      {
        case Network.PowerType.FULL_ENTITY:
          change = this.CreateZoneChangeFromFullEntity((Network.HistFullEntity) power);
          break;
        case Network.PowerType.SHOW_ENTITY:
          change = this.CreateZoneChangeFromEntity(((Network.HistShowEntity) power).Entity);
          break;
        case Network.PowerType.HIDE_ENTITY:
          change = this.CreateZoneChangeFromHideEntity((Network.HistHideEntity) power);
          break;
        case Network.PowerType.TAG_CHANGE:
          change = this.CreateZoneChangeFromTagChange((Network.HistTagChange) power);
          break;
        case Network.PowerType.CREATE_GAME:
        case Network.PowerType.RESET_GAME:
        case Network.PowerType.SUB_SPELL_START:
        case Network.PowerType.SUB_SPELL_END:
        case Network.PowerType.VO_SPELL:
        case Network.PowerType.CACHED_TAG_FOR_DORMANT_CHANGE:
        case Network.PowerType.SHUFFLE_DECK:
          change = this.CreateZoneChangeForNonZoneTask();
          break;
        case Network.PowerType.META_DATA:
          change = this.CreateZoneChangeFromMetaData((Network.HistMetaData) power);
          break;
        case Network.PowerType.CHANGE_ENTITY:
          change = this.CreateZoneChangeFromEntity(((Network.HistChangeEntity) power).Entity);
          break;
        default:
          Debug.LogError((object) string.Format("ZoneMgr.AddServerZoneChanges() - id={0} received unhandled power of type {1}", (object) parentList.GetId(), (object) type));
          return (ZoneChangeList) null;
      }
      if (change != null)
      {
        change.SetParentList(parentList);
        change.SetPowerTask(powerTask);
        Log.Zone.Print("ZoneMgr.AddServerZoneChanges() - AddChange() changeList: {0},  change: {1}", (object) parentList, (object) change);
        parentList.AddChange(change);
      }
    }
    for (int index1 = 0; index1 < parentList.GetChanges().Count; ++index1)
    {
      ZoneChange change1 = parentList.GetChanges()[index1];
      if (change1.GetPowerTask().GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.CONTROLLER_AND_ZONE_CHANGE)
      {
        if (power.Info.Count != 5)
          Log.Zone.PrintError("CONTROLLER_AND_ZONE_CHANGE MetaData task found ({0}), but info array isn't of size 5!");
        ZoneChange zoneChange1 = (ZoneChange) null;
        ZoneChange zoneChange2 = (ZoneChange) null;
        int controllerId = power.Info[1];
        int num = power.Info[2];
        TAG_ZONE zoneTag = (TAG_ZONE) power.Info[3];
        TAG_ZONE tagZone = (TAG_ZONE) power.Info[4];
        for (int index2 = index1 + 1; index2 < parentList.GetChanges().Count; ++index2)
        {
          ZoneChange change2 = parentList.GetChanges()[index2];
          if (change2.GetEntity() == change1.GetEntity())
          {
            if (change2.HasDestinationControllerId() && change2.GetDestinationControllerId() == num && change2.GetDestinationZoneTag() != tagZone)
              zoneChange1 = change2;
            else if (change2.HasDestinationControllerId() && change2.GetDestinationControllerId() == num && change2.HasDestinationZoneChange() && change2.GetDestinationZoneTag() == tagZone)
            {
              zoneChange1 = change2;
              zoneChange2 = change2;
            }
            else if (!change2.HasDestinationControllerId() && change2.HasDestinationZoneChange() && change2.GetDestinationZoneTag() == tagZone)
              zoneChange2 = change2;
            if (zoneChange1 != null && zoneChange2 != null)
              break;
          }
        }
        if (zoneChange1 != null && zoneChange2 != null)
        {
          Entity entity = zoneChange1.GetEntity();
          Zone zoneForTags = this.FindZoneForTags(controllerId, zoneTag, entity.GetCardType(), entity);
          zoneChange2.SetSourceZone(zoneForTags);
          zoneChange2.SetDestinationControllerId(zoneChange1.GetDestinationControllerId());
          zoneChange2.SetSourceControllerId(controllerId);
          if (zoneChange2 != zoneChange1)
          {
            zoneChange1.ClearDestinationControllerId();
            zoneChange1.SetDestinationZone((Zone) null);
          }
        }
        else
          Log.Zone.PrintError("CONTROLLER_AND_ZONE_CHANGE MetaData task found ({0}), but couldn't find both controller ({1}) and zone ({2}) changes in tasklist!", (object) change1, (object) zoneChange1, (object) zoneChange2);
      }
    }
    this.m_tempEntityMap.Clear();
    this.m_pendingServerChangeLists.Enqueue(parentList);
    return parentList;
  }

  private void UpdateLocalChangeLists(CancellationToken token)
  {
    List<ZoneChangeList> zoneChangeListList = (List<ZoneChangeList>) null;
    int index = 0;
    while (index < this.m_activeLocalChangeLists.Count)
    {
      ZoneChangeList activeLocalChangeList = this.m_activeLocalChangeLists[index];
      if (!activeLocalChangeList.IsComplete())
      {
        ++index;
      }
      else
      {
        activeLocalChangeList.FireCompleteCallback();
        this.m_activeLocalChangeLists.RemoveAt(index);
        if (zoneChangeListList == null)
          zoneChangeListList = new List<ZoneChangeList>();
        zoneChangeListList.Add(activeLocalChangeList);
      }
    }
    if (zoneChangeListList == null)
      return;
    foreach (ZoneChangeList zoneChangeList in zoneChangeListList)
    {
      ZoneChange localTriggerChange = zoneChangeList.GetLocalTriggerChange();
      Card card = localTriggerChange.GetEntity().GetCard();
      if (zoneChangeList.IsCanceledChangeList())
      {
        card.SetPredictedZonePosition(0);
        if (card.m_minionWasMovedFromSrcToDst != null && card.m_minionWasMovedFromSrcToDst.m_destinationZonePosition == localTriggerChange.GetDestinationPosition())
          card.m_minionWasMovedFromSrcToDst = (ZonePositionChange) null;
      }
      int localChangeIndex = this.FindTriggeredPendingLocalChangeIndex(card);
      if (localChangeIndex >= 0)
      {
        ZoneChangeList pendingLocalChangeList = this.m_pendingLocalChangeLists[localChangeIndex];
        this.m_pendingLocalChangeLists.RemoveAt(localChangeIndex);
        this.CreateLocalChangesFromTrigger(pendingLocalChangeList, pendingLocalChangeList.GetLocalTriggerChange());
        this.ProcessLocalChangeList(pendingLocalChangeList, token);
      }
    }
  }

  private void UpdateServerChangeLists(CancellationToken token)
  {
    if (this.m_activeServerChangeList != null && this.m_activeServerChangeList.IsComplete())
    {
      this.m_activeServerChangeList.FireCompleteCallback();
      this.m_activeServerChangeList = (ZoneChangeList) null;
      this.m_doAutoCorrection = true;
    }
    if (this.HasPendingServerChange() && !this.HasActiveServerChange())
    {
      this.m_activeServerChangeList = this.m_pendingServerChangeLists.Dequeue();
      this.PostProcessServerChangeList(this.m_activeServerChangeList);
      this.m_activeServerChangeList.ProcessChanges(token).Forget();
    }
    if (!this.m_doAutoCorrection || !this.AutoCorrectZonesAfterServerChange(token))
      return;
    this.m_doAutoCorrection = false;
  }

  private bool HasLocalChangeExitingZone(Entity entity, Zone zone) => this.HasLocalChangeExitingZone(entity, zone, this.m_activeLocalChangeLists) || this.HasLocalChangeExitingZone(entity, zone, this.m_pendingLocalChangeLists);

  private bool HasLocalChangeExitingZone(
    Entity entity,
    Zone zone,
    List<ZoneChangeList> changeLists)
  {
    TAG_ZONE serverTag = zone.m_ServerTag;
    foreach (ZoneChangeList changeList in changeLists)
    {
      foreach (ZoneChange change in changeList.GetChanges())
      {
        if (entity == change.GetEntity() && serverTag == change.GetSourceZoneTag() && serverTag != change.GetDestinationZoneTag())
          return true;
      }
    }
    return false;
  }

  private void PredictZoneFromPowerProcessor(ZoneMgr.TempZone tempZone)
  {
    PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
    tempZone.PreprocessChanges();
    Action<int, PowerTaskList> predicate = (Action<int, PowerTaskList>) ((queueIndex, taskList) => this.PredictZoneFromPowerTaskList(tempZone, taskList));
    powerProcessor.ForEachTaskList(predicate);
    tempZone.Sort();
    tempZone.PostprocessChanges();
  }

  private void PredictZoneFromPowerTaskList(ZoneMgr.TempZone tempZone, PowerTaskList taskList)
  {
    List<PowerTask> taskList1 = taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      Network.PowerHistory power = taskList1[index].GetPower();
      this.PredictZoneFromPower(tempZone, power);
    }
  }

  private void PredictZoneFromPower(ZoneMgr.TempZone tempZone, Network.PowerHistory power)
  {
    switch (power.Type)
    {
      case Network.PowerType.FULL_ENTITY:
        this.PredictZoneFromFullEntity(tempZone, (Network.HistFullEntity) power);
        break;
      case Network.PowerType.SHOW_ENTITY:
        this.PredictZoneFromShowEntity(tempZone, (Network.HistShowEntity) power);
        break;
      case Network.PowerType.HIDE_ENTITY:
        this.PredictZoneFromHideEntity(tempZone, (Network.HistHideEntity) power);
        break;
      case Network.PowerType.TAG_CHANGE:
        this.PredictZoneFromTagChange(tempZone, (Network.HistTagChange) power);
        break;
    }
  }

  private void PredictZoneFromFullEntity(
    ZoneMgr.TempZone tempZone,
    Network.HistFullEntity fullEntity)
  {
    Entity entity = this.RegisterTempEntity(fullEntity.Entity);
    if (entity == null)
      return;
    Zone zone = tempZone.GetZone();
    int num = entity.GetZone() == zone.m_ServerTag ? 1 : 0;
    bool flag = entity.GetControllerId() == zone.GetControllerId();
    if (num == 0 || !flag)
      return;
    tempZone.AddEntity(entity);
  }

  private void PredictZoneFromShowEntity(
    ZoneMgr.TempZone tempZone,
    Network.HistShowEntity showEntity)
  {
    Entity tempEntity = this.RegisterTempEntity(showEntity.Entity);
    foreach (Network.Entity.Tag tag in showEntity.Entity.Tags)
      this.PredictZoneByApplyingTag(tempZone, tempEntity, (GAME_TAG) tag.Name, tag.Value);
  }

  private void PredictZoneFromHideEntity(
    ZoneMgr.TempZone tempZone,
    Network.HistHideEntity hideEntity)
  {
    Entity tempEntity = this.RegisterTempEntity(hideEntity.Entity);
    this.PredictZoneByApplyingTag(tempZone, tempEntity, GAME_TAG.ZONE, hideEntity.Zone);
  }

  private void PredictZoneFromTagChange(ZoneMgr.TempZone tempZone, Network.HistTagChange tagChange)
  {
    Entity tempEntity = this.RegisterTempEntity(tagChange.Entity);
    this.PredictZoneByApplyingTag(tempZone, tempEntity, (GAME_TAG) tagChange.Tag, tagChange.Value);
  }

  private void PredictZoneByApplyingTag(
    ZoneMgr.TempZone tempZone,
    Entity tempEntity,
    GAME_TAG tag,
    int val)
  {
    if (tempEntity == null)
      return;
    if ((tag == GAME_TAG.ZONE || tag == GAME_TAG.CONTROLLER || tag == GAME_TAG.FAKE_ZONE ? 1 : (tag == GAME_TAG.FAKE_CONTROLLER ? 1 : 0)) == 0)
    {
      tempEntity.SetTag(tag, val);
    }
    else
    {
      Zone zone = tempZone.GetZone();
      if (tempEntity.GetZone() == zone.m_ServerTag & tempEntity.GetControllerId() == zone.GetControllerId())
        tempZone.RemoveEntity(tempEntity);
      tempEntity.SetTag(tag, val);
      if (!(tempEntity.GetZone() == zone.m_ServerTag & tempEntity.GetControllerId() == zone.GetControllerId()))
        return;
      tempZone.AddEntity(tempEntity);
    }
  }

  private ZoneChange CreateZoneChange(
    Card triggerCard,
    Zone destinationZone,
    TAG_ZONE destinationZoneTag,
    int destinationPos)
  {
    Entity entity = triggerCard.GetEntity();
    Zone zone = triggerCard.GetZone();
    TAG_ZONE tag = (UnityEngine.Object) zone == (UnityEngine.Object) null ? TAG_ZONE.INVALID : zone.m_ServerTag;
    int zonePosition = triggerCard.GetZonePosition();
    ZoneChange zoneChange = new ZoneChange();
    zoneChange.SetEntity(entity);
    zoneChange.SetSourceZone(zone);
    zoneChange.SetSourceZoneTag(tag);
    zoneChange.SetSourcePosition(zonePosition);
    zoneChange.SetDestinationZone(destinationZone);
    zoneChange.SetDestinationZoneTag(destinationZoneTag);
    zoneChange.SetDestinationPosition(destinationPos);
    return zoneChange;
  }

  private ZoneChangeList CreateLocalChangeList(
    Card triggerCard,
    Zone destinationZone,
    TAG_ZONE destinationZoneTag,
    int destinationPos,
    ZoneMgr.ChangeCompleteCallback callback,
    object userData)
  {
    int localChangeListId = this.GetNextLocalChangeListId();
    Log.Zone.Print("ZoneMgr.CreateLocalChangeList() - changeListId={0}", (object) localChangeListId);
    ZoneChangeList parentList = new ZoneChangeList();
    parentList.SetId(localChangeListId);
    parentList.SetCompleteCallback(callback);
    parentList.SetCompleteCallbackUserData(userData);
    ZoneChange zoneChange = this.CreateZoneChange(triggerCard, destinationZone, destinationZoneTag, destinationPos);
    zoneChange.SetParentList(parentList);
    Log.Zone.Print("ZoneMgr.CreateLocalChangeList() - AddChange() changeList: {0}, triggerChange: {1}", (object) parentList, (object) zoneChange);
    parentList.AddChange(zoneChange);
    return parentList;
  }

  private void ProcessOrEnqueueLocalChangeList(ZoneChangeList changeList, CancellationToken token)
  {
    ZoneChange localTriggerChange = changeList.GetLocalTriggerChange();
    if (this.HasTriggeredActiveLocalChange(localTriggerChange.GetEntity().GetCard()) && !this.IsBattlegroundShoppingPhase())
    {
      this.m_pendingLocalChangeLists.Add(changeList);
    }
    else
    {
      this.CreateLocalChangesFromTrigger(changeList, localTriggerChange);
      this.ProcessLocalChangeList(changeList, token);
    }
  }

  private void CreateLocalChangesFromTrigger(ZoneChangeList changeList, ZoneChange triggerChange)
  {
    Log.Zone.Print(string.Format("ZoneMgr.CreateLocalChangesFromTrigger() - {0}", (object) changeList));
    Entity entity = triggerChange.GetEntity();
    Zone sourceZone = triggerChange.GetSourceZone();
    int sourcePosition = triggerChange.GetSourcePosition();
    Zone destinationZone = triggerChange.GetDestinationZone();
    int destinationPosition = triggerChange.GetDestinationPosition();
    if ((UnityEngine.Object) sourceZone != (UnityEngine.Object) destinationZone)
    {
      TAG_ZONE sourceZoneTag = triggerChange.GetSourceZoneTag();
      TAG_ZONE destinationZoneTag = triggerChange.GetDestinationZoneTag();
      this.CreateLocalChangesFromTrigger(changeList, entity, sourceZone, sourceZoneTag, sourcePosition, destinationZone, destinationZoneTag, destinationPosition);
    }
    else
    {
      if (sourcePosition == destinationPosition)
        return;
      this.CreateLocalPosOnlyChangesFromTrigger(changeList, entity, sourceZone, sourcePosition, destinationPosition);
    }
  }

  private void CreateLocalChangesFromTrigger(
    ZoneChangeList changeList,
    Entity triggerEntity,
    Zone sourceZone,
    TAG_ZONE sourceZoneTag,
    int sourcePos,
    Zone destinationZone,
    TAG_ZONE destinationZoneTag,
    int destinationPos)
  {
    Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - triggerEntity={0} srcZone={1} srcPos={2} dstZone={3} dstPos={4}", (object) triggerEntity, (object) sourceZoneTag, (object) sourcePos, (object) destinationZoneTag, (object) destinationPos);
    if (sourcePos != destinationPos)
      Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - srcPos={0} destPos={1}", (object) sourcePos, (object) destinationPos);
    if ((UnityEngine.Object) sourceZone != (UnityEngine.Object) null && !(sourceZone is ZoneHero))
    {
      foreach (Card card in sourceZone.GetCards())
      {
        int zonePosition = card.GetZonePosition();
        if (zonePosition > sourcePos)
        {
          Entity entity = card.GetEntity();
          ZoneChange change = new ZoneChange();
          change.SetParentList(changeList);
          change.SetEntity(entity);
          int pos = zonePosition - 1;
          change.SetSourcePosition(zonePosition);
          change.SetDestinationPosition(pos);
          Log.Zone.Print(string.Format("ZoneMgr.CreateLocalChangesFromTrigger() - srcZone card {0} zonePos {1} -> {2}", (object) card, (object) card.GetZonePosition(), (object) pos));
          Log.Zone.Print(string.Format("ZoneMgr.CreateLocalChangesFromTrigger() 3 - AddChange() changeList: {0}, change: {1}", (object) changeList, (object) change));
          changeList.AddChange(change);
        }
      }
    }
    if (!((UnityEngine.Object) destinationZone != (UnityEngine.Object) null))
      return;
    switch (destinationZone)
    {
      case ZoneSecret _:
        break;
      case ZoneWeapon _:
      case ZoneBattlegroundQuestReward _:
        List<Card> cards1 = destinationZone.GetCards();
        if (cards1.Count <= 0)
          break;
        Entity entity1 = cards1[0].GetEntity();
        ZoneChange change1 = new ZoneChange();
        change1.SetParentList(changeList);
        change1.SetEntity(entity1);
        change1.SetDestinationZone((Zone) this.FindZoneOfType<ZoneGraveyard>(destinationZone.m_Side));
        change1.SetDestinationZoneTag(TAG_ZONE.GRAVEYARD);
        Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() 4 - AddChange() changeList: {0}, change: {1}", (object) changeList, (object) change1);
        changeList.AddChange(change1);
        break;
      case ZonePlay _:
      case ZoneHand _:
        List<Card> cards2 = destinationZone.GetCards();
        ZonePlay zonePlay = destinationZone as ZonePlay;
        for (int index = 0; index < cards2.Count; ++index)
        {
          Card card = cards2[index];
          int num = (UnityEngine.Object) zonePlay != (UnityEngine.Object) null ? zonePlay.GetSlotOfCardAtIndex(index) : index + 1;
          if (num >= destinationPos)
          {
            Entity entity2 = card.GetEntity();
            int pos = num + 1;
            ZoneChange change2 = new ZoneChange();
            change2.SetParentList(changeList);
            change2.SetEntity(entity2);
            change2.SetDestinationPosition(pos);
            Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - dstZone card {0} zonePos {1} -> {2}", (object) card, (object) entity2.GetZonePosition(), (object) pos);
            Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() 5 - AddChange() changeList: {0}, change: {1}", (object) changeList, (object) change2);
            changeList.AddChange(change2);
          }
        }
        break;
      case ZoneHero _:
        break;
      default:
        Debug.LogError((object) string.Format("ZoneMgr.CreateLocalChangesFromTrigger() - don't know how to predict zone position changes for zone {0}", (object) destinationZone));
        break;
    }
  }

  private void CreateLocalPosOnlyChangesFromTrigger(
    ZoneChangeList changeList,
    Entity triggerEntity,
    Zone sourceZone,
    int sourcePos,
    int destinationPos)
  {
    List<Card> cards = sourceZone.GetCards();
    if (sourcePos < destinationPos)
    {
      for (int index = 0; index < cards.Count; ++index)
      {
        Card card = cards[index];
        Entity entity = card.GetEntity();
        int zonePosition = card.GetZonePosition();
        if (zonePosition <= destinationPos && zonePosition >= sourcePos)
        {
          int pos = zonePosition - 1;
          if (entity == triggerEntity)
            pos = destinationPos;
          ZoneChange change = new ZoneChange();
          change.SetParentList(changeList);
          change.SetEntity(entity);
          change.SetSourcePosition(card.GetZonePosition());
          change.SetDestinationPosition(pos);
          Log.Zone.Print("ZoneMgr.CreateLocalPosOnlyChangesFromTrigger() 1 - AddChange() changeList: {0}, change: {1}", (object) changeList, (object) change);
          changeList.AddChange(change);
        }
      }
    }
    else
    {
      for (int index = 0; index < cards.Count; ++index)
      {
        Card card = cards[index];
        Entity entity = card.GetEntity();
        int zonePosition = card.GetZonePosition();
        if (zonePosition <= sourcePos && zonePosition >= destinationPos)
        {
          int pos = zonePosition + 1;
          if (entity == triggerEntity)
            pos = destinationPos;
          ZoneChange change = new ZoneChange();
          change.SetParentList(changeList);
          change.SetEntity(entity);
          change.SetSourcePosition(card.GetZonePosition());
          change.SetDestinationPosition(pos);
          Log.Zone.Print("ZoneMgr.CreateLocalPosOnlyChangesFromTrigger() 2 - AddChange() changeList: {0}, change: {1}", (object) changeList, (object) change);
          changeList.AddChange(change);
        }
      }
    }
  }

  private void ProcessLocalChangeList(ZoneChangeList changeList, CancellationToken token)
  {
    Log.Zone.Print("ZoneMgr.ProcessLocalChangeList() - [{0}]", (object) changeList);
    this.m_activeLocalChangeLists.Add(changeList);
    changeList.ProcessChanges(token).Forget();
  }

  private void OnCurrentPlayerChanged(Player player, object userData)
  {
    if (!player.IsLocalUser())
      return;
    this.m_localChangeListHistory.Clear();
  }

  public void ClearLocalChangeListHistory() => this.m_localChangeListHistory.Clear();

  private void OnOptionRejected(Network.Options.Option option, object userData)
  {
    if (option.Type != Network.Options.Option.OptionType.POWER)
      return;
    Entity entity = GameState.Get().GetEntity(option.Main.ID);
    ZoneChangeList rejectedLocalZoneChange = this.FindRejectedLocalZoneChange(entity);
    if (rejectedLocalZoneChange == null)
    {
      Log.Zone.Print("ZoneMgr.RejectLocalZoneChange() - did not find a zone change to reject for {0}", (object) entity);
    }
    else
    {
      Card card = entity.GetCard();
      card.SetPredictedZonePosition(0);
      ZoneChange localTriggerChange = rejectedLocalZoneChange.GetLocalTriggerChange();
      if (card.m_minionWasMovedFromSrcToDst != null && card.m_minionWasMovedFromSrcToDst.m_destinationZonePosition == localTriggerChange.GetDestinationPosition())
        card.m_minionWasMovedFromSrcToDst = (ZonePositionChange) null;
      this.CancelLocalZoneChange(rejectedLocalZoneChange);
    }
  }

  private ZoneChangeList FindRejectedLocalZoneChange(Entity triggerEntity)
  {
    List<ZoneChangeList> list = this.m_localChangeListHistory.GetList();
    for (int index1 = 0; index1 < list.Count; ++index1)
    {
      ZoneChangeList rejectedLocalZoneChange = list[index1];
      List<ZoneChange> changes = rejectedLocalZoneChange.GetChanges();
      for (int index2 = 0; index2 < changes.Count; ++index2)
      {
        ZoneChange zoneChange = changes[index2];
        if (zoneChange.GetEntity() == triggerEntity && zoneChange.GetDestinationZoneTag() == TAG_ZONE.PLAY)
          return rejectedLocalZoneChange;
      }
    }
    return (ZoneChangeList) null;
  }

  private ZoneChange CreateZoneChangeForNonZoneTask()
  {
    ZoneChange changeForNonZoneTask = new ZoneChange();
    changeForNonZoneTask.SetEntity((Entity) GameState.Get().GetGameEntity());
    return changeForNonZoneTask;
  }

  private ZoneChange CreateZoneChangeFromFullEntity(Network.HistFullEntity fullEntity)
  {
    Network.Entity entity1 = fullEntity.Entity;
    Entity entity2 = GameState.Get().GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Debug.LogWarning((object) string.Format("ZoneMgr.CreateZoneChangeFromFullEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID));
      return (ZoneChange) null;
    }
    ZoneChange changeFromFullEntity = new ZoneChange();
    changeFromFullEntity.SetEntity(entity2);
    if ((UnityEngine.Object) entity2.GetCard() == (UnityEngine.Object) null)
      return changeFromFullEntity;
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    foreach (Network.Entity.Tag tag in entity1.Tags)
    {
      switch ((GAME_TAG) tag.Name)
      {
        case GAME_TAG.ZONE:
        case GAME_TAG.FAKE_ZONE:
          flag1 = true;
          continue;
        case GAME_TAG.CONTROLLER:
        case GAME_TAG.FAKE_CONTROLLER:
          flag3 = true;
          continue;
        case GAME_TAG.ZONE_POSITION:
        case GAME_TAG.FAKE_ZONE_POSITION:
          flag2 = true;
          continue;
        default:
          continue;
      }
    }
    if (flag1)
      changeFromFullEntity.SetDestinationZoneTag(entity2.GetZone());
    if (flag2)
      changeFromFullEntity.SetDestinationPosition(entity2.GetZonePosition());
    if (flag3)
      changeFromFullEntity.SetDestinationControllerId(entity2.GetControllerId());
    if (flag1 | flag3)
      changeFromFullEntity.SetDestinationZone(this.FindZoneForEntity(entity2));
    return changeFromFullEntity;
  }

  private ZoneChange CreateZoneChangeFromEntity(Network.Entity netEnt)
  {
    Entity entity1 = GameState.Get().GetEntity(netEnt.ID);
    if (entity1 == null)
    {
      if (!GameState.Get().EntityRemovedFromGame(netEnt.ID))
        Debug.LogWarning((object) string.Format("ZoneMgr.CreateZoneChangeFromEntity() - WARNING entity {0} DOES NOT EXIST!", (object) netEnt.ID));
      return (ZoneChange) null;
    }
    ZoneChange changeFromEntity = new ZoneChange();
    changeFromEntity.SetEntity(entity1);
    if ((UnityEngine.Object) entity1.GetCard() == (UnityEngine.Object) null)
      return changeFromEntity;
    Entity entity2 = this.RegisterTempEntity(netEnt.ID, entity1);
    if (entity2 == null)
      return changeFromEntity;
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    foreach (Network.Entity.Tag tag in netEnt.Tags)
    {
      entity2.SetTag(tag.Name, tag.Value);
      switch ((GAME_TAG) tag.Name)
      {
        case GAME_TAG.ZONE:
        case GAME_TAG.FAKE_ZONE:
          flag1 = true;
          continue;
        case GAME_TAG.CONTROLLER:
        case GAME_TAG.FAKE_CONTROLLER:
          flag3 = true;
          continue;
        case GAME_TAG.ZONE_POSITION:
        case GAME_TAG.FAKE_ZONE_POSITION:
          flag2 = true;
          continue;
        default:
          continue;
      }
    }
    if (flag1)
      changeFromEntity.SetDestinationZoneTag(entity2.GetZone());
    if (flag2)
      changeFromEntity.SetDestinationPosition(entity2.GetZonePosition());
    if (flag3)
      changeFromEntity.SetDestinationControllerId(entity2.GetControllerId());
    if (flag1 | flag3)
      changeFromEntity.SetDestinationZone(this.FindZoneForEntity(entity2));
    return changeFromEntity;
  }

  private ZoneChange CreateZoneChangeFromHideEntity(Network.HistHideEntity hideEntity)
  {
    Entity entity1 = GameState.Get().GetEntity(hideEntity.Entity);
    if (entity1 == null)
    {
      if (!GameState.Get().EntityRemovedFromGame(hideEntity.Entity))
        Debug.LogWarning((object) string.Format("ZoneMgr.CreateZoneChangeFromHideEntity() - WARNING entity {0} DOES NOT EXIST! zone={1}", (object) hideEntity.Entity, (object) hideEntity.Zone));
      return (ZoneChange) null;
    }
    ZoneChange changeFromHideEntity = new ZoneChange();
    changeFromHideEntity.SetEntity(entity1);
    if ((UnityEngine.Object) entity1.GetCard() == (UnityEngine.Object) null)
      return changeFromHideEntity;
    Entity entity2 = this.RegisterTempEntity(hideEntity.Entity, entity1);
    if (entity2 == null)
      return changeFromHideEntity;
    entity2.SetTag(GAME_TAG.ZONE, hideEntity.Zone);
    TAG_ZONE zone = (TAG_ZONE) hideEntity.Zone;
    changeFromHideEntity.SetDestinationZoneTag(zone);
    changeFromHideEntity.SetDestinationZone(this.FindZoneForEntity(entity2));
    return changeFromHideEntity;
  }

  private ZoneChange CreateZoneChangeFromTagChange(Network.HistTagChange tagChange)
  {
    Entity entity1 = GameState.Get().GetEntity(tagChange.Entity);
    if (entity1 == null)
    {
      if (!GameState.Get().EntityRemovedFromGame(tagChange.Entity))
        Debug.LogError((object) string.Format("ZoneMgr.CreateZoneChangeFromTagChange() - Entity {0} does not exist", (object) tagChange.Entity));
      return (ZoneChange) null;
    }
    ZoneChange changeFromTagChange = new ZoneChange();
    changeFromTagChange.SetEntity(entity1);
    if ((UnityEngine.Object) entity1.GetCard() == (UnityEngine.Object) null)
      return changeFromTagChange;
    Entity entity2 = this.RegisterTempEntity(tagChange.Entity, entity1);
    if (entity2 == null)
      return changeFromTagChange;
    entity2.SetTag(tagChange.Tag, tagChange.Value);
    switch ((GAME_TAG) tagChange.Tag)
    {
      case GAME_TAG.ZONE:
      case GAME_TAG.FAKE_ZONE:
        changeFromTagChange.SetDestinationZoneTag(entity2.GetZone());
        changeFromTagChange.SetDestinationZone(this.FindZoneForEntity(entity2));
        break;
      case GAME_TAG.CONTROLLER:
      case GAME_TAG.FAKE_CONTROLLER:
        changeFromTagChange.SetDestinationControllerId(entity2.GetControllerId());
        changeFromTagChange.SetDestinationZone(this.FindZoneForEntity(entity2));
        break;
      case GAME_TAG.ZONE_POSITION:
      case GAME_TAG.FAKE_ZONE_POSITION:
        changeFromTagChange.SetDestinationPosition(entity2.GetZonePosition());
        break;
    }
    return changeFromTagChange;
  }

  private ZoneChange CreateZoneChangeFromMetaData(Network.HistMetaData metaData)
  {
    if (metaData.Info.Count <= 0)
      return (ZoneChange) null;
    Entity entity = GameState.Get().GetEntity(metaData.Info[0]);
    if (entity == null)
    {
      Debug.LogError((object) string.Format("ZoneMgr.CreateZoneChangeFromMetaData() - Entity {0} does not exist", (object) metaData.Info[0]));
      return (ZoneChange) null;
    }
    ZoneChange changeFromMetaData = new ZoneChange();
    changeFromMetaData.SetEntity(entity);
    return changeFromMetaData;
  }

  private Entity RegisterTempEntity(int id)
  {
    Entity entity = GameState.Get().GetEntity(id);
    return this.RegisterTempEntity(id, entity);
  }

  private Entity RegisterTempEntity(Network.Entity netEnt)
  {
    Entity entity = GameState.Get().GetEntity(netEnt.ID);
    return this.RegisterTempEntity(netEnt.ID, entity);
  }

  private Entity RegisterTempEntity(Entity entity) => this.RegisterTempEntity(entity == null ? -1 : entity.GetEntityId(), entity);

  private Entity RegisterTempEntity(int id, Entity entity)
  {
    if (entity == null)
    {
      string str = string.Format("{0}.RegisterTempEntity(): Attempting to register an invalid entity! No dbid {1} exists.", (object) this, (object) id);
      TelemetryManager.Client().SendLiveIssue("Gameplay_ZoneManager", str);
      Log.Zone.PrintWarning(str);
    }
    Entity entity1 = (Entity) null;
    if (!this.m_tempEntityMap.TryGetValue(id, out entity1) && entity != null)
    {
      entity1 = entity.CloneForZoneMgr();
      this.m_tempEntityMap.Add(id, entity1);
    }
    return entity1;
  }

  private void PostProcessServerChangeList(ZoneChangeList serverChangeList)
  {
    if (!this.ShouldPostProcessServerChangeList(serverChangeList) || this.CheckAndIgnoreServerChangeList(serverChangeList) || this.ReplaceRemoteWeaponInServerChangeList(serverChangeList) || this.PreventLastStandingMinionFromLeavingPlay(serverChangeList))
      return;
    this.MergeServerChangeList(serverChangeList);
  }

  private bool ShouldPostProcessServerChangeList(ZoneChangeList changeList)
  {
    List<ZoneChange> changes = changeList.GetChanges();
    for (int index = 0; index < changes.Count; ++index)
    {
      if (changes[index].HasDestinationData())
        return true;
    }
    return false;
  }

  private bool CheckAndIgnoreServerChangeList(ZoneChangeList serverChangeList)
  {
    Network.HistBlockStart blockStart = serverChangeList.GetTaskList().GetBlockStart();
    if (blockStart == null || blockStart.BlockType != HistoryBlock.Type.PLAY && blockStart.BlockType != HistoryBlock.Type.MOVE_MINION)
      return false;
    ZoneChangeList serverChangeList1 = this.FindLocalChangeListMatchingServerChangeList(serverChangeList);
    if (serverChangeList1 == null)
      return false;
    serverChangeList.SetIgnoreCardZoneChanges(true);
    Card localTriggerCard = serverChangeList1.GetLocalTriggerCard();
    if (blockStart.BlockType == HistoryBlock.Type.MOVE_MINION && (UnityEngine.Object) localTriggerCard != (UnityEngine.Object) null && localTriggerCard.m_minionWasMovedFromSrcToDst != null)
    {
      foreach (ZoneChange change in serverChangeList1.GetChanges())
      {
        if (change.GetDestinationPosition() == localTriggerCard.m_minionWasMovedFromSrcToDst.m_destinationZonePosition && change.GetEntity() == localTriggerCard.GetEntity())
        {
          localTriggerCard.m_minionWasMovedFromSrcToDst = (ZonePositionChange) null;
          break;
        }
      }
    }
    while (this.m_localChangeListHistory.Count > 0)
    {
      ZoneChangeList zoneChangeList = this.m_localChangeListHistory.Dequeue();
      if (serverChangeList1 == zoneChangeList)
      {
        serverChangeList1.GetLocalTriggerCard().SetPredictedZonePosition(0);
        break;
      }
    }
    return true;
  }

  private ZoneChangeList FindLocalChangeListMatchingServerChangeList(
    ZoneChangeList serverChangeList)
  {
    foreach (ZoneChangeList serverChangeList1 in this.m_localChangeListHistory)
    {
      int predictedPosition = serverChangeList1.GetPredictedPosition();
      foreach (ZoneChange change in serverChangeList1.GetChanges())
      {
        Entity entity1 = change.GetEntity();
        TAG_ZONE destinationZoneTag1 = change.GetDestinationZoneTag();
        TAG_ZONE sourceZoneTag = change.GetSourceZoneTag();
        if (destinationZoneTag1 != TAG_ZONE.INVALID)
        {
          bool flag = sourceZoneTag != destinationZoneTag1;
          List<ZoneChange> changes = serverChangeList.GetChanges();
          for (int index = 0; index < changes.Count; ++index)
          {
            ZoneChange zoneChange = changes[index];
            Entity entity2 = zoneChange.GetEntity();
            if (entity1 == entity2)
            {
              if (flag)
              {
                TAG_ZONE destinationZoneTag2 = zoneChange.GetDestinationZoneTag();
                if (destinationZoneTag1 == destinationZoneTag2)
                {
                  if (destinationZoneTag1 == TAG_ZONE.PLAY && entity1.HasTag(GAME_TAG.TRANSFORMED_FROM_CARD) && entity1.GetTag(GAME_TAG.TRANSFORMED_FROM_CARD) != entity1.GetTag(GAME_TAG.DATABASE_ID))
                  {
                    int tag = entity1.GetTag(GAME_TAG.LAST_AFFECTED_BY);
                    Entity entity3 = GameState.Get().GetEntity(tag);
                    if (entity3 != null && GameUtils.TranslateCardIdToDbId(entity3.GetCardId()) == 61187)
                      continue;
                  }
                }
                else
                  continue;
              }
              ZoneChange nextDstPosChange = this.FindNextDstPosChange(serverChangeList, index, entity2);
              int num = nextDstPosChange == null ? entity2.GetZonePosition() : nextDstPosChange.GetDestinationPosition();
              if (predictedPosition == num)
                return serverChangeList1;
            }
          }
        }
      }
    }
    return (ZoneChangeList) null;
  }

  private ZoneChange FindNextDstPosChange(
    ZoneChangeList changeList,
    int index,
    Entity entity)
  {
    List<ZoneChange> changes = changeList.GetChanges();
    for (int index1 = index; index1 < changes.Count; ++index1)
    {
      ZoneChange zoneChange = changes[index1];
      if (zoneChange.HasDestinationZoneChange() && index1 != index)
        return (ZoneChange) null;
      if (zoneChange.HasDestinationPosition())
        return zoneChange.GetEntity() != entity ? (ZoneChange) null : zoneChange;
    }
    return (ZoneChange) null;
  }

  private bool ReplaceRemoteWeaponInServerChangeList(ZoneChangeList serverChangeList)
  {
    List<ZoneChange> changes = serverChangeList.GetChanges();
    List<ZoneChange> all = changes.FindAll((Predicate<ZoneChange>) (change =>
    {
      if (!(change.GetDestinationZone() is ZoneWeapon))
        return false;
      PowerTask powerTask = change.GetPowerTask();
      return powerTask == null || !powerTask.IsCompleted();
    }));
    bool flag1 = false;
    foreach (ZoneChange zoneChange1 in all)
    {
      Zone destinationZone = zoneChange1.GetDestinationZone();
      if (destinationZone.GetCardCount() != 0)
      {
        Entity entity = destinationZone.GetCardAtIndex(0).GetEntity();
        bool flag2 = false;
        foreach (ZoneChange zoneChange2 in changes)
        {
          PowerTask powerTask = zoneChange2.GetPowerTask();
          if (powerTask != null && powerTask.GetPower() is Network.HistTagChange power && power.Entity == entity.GetEntityId() && power.Tag == 360 && power.Value > 0)
          {
            flag2 = true;
            break;
          }
        }
        if (flag2)
        {
          Zone zoneForTags = this.FindZoneForTags(entity.GetControllerId(), TAG_ZONE.GRAVEYARD, TAG_CARDTYPE.WEAPON, entity);
          ZoneChange change = new ZoneChange();
          change.SetEntity(entity);
          change.SetDestinationZone(zoneForTags);
          change.SetDestinationZoneTag(TAG_ZONE.GRAVEYARD);
          change.SetDestinationPosition(0);
          change.SetParentList(serverChangeList);
          Log.Zone.Print("ZoneMgr.ReplaceRemoteWeaponInServerChangeList() - AddChange() serverChangeList: {0}, graveyardChange: {1}", (object) serverChangeList, (object) change);
          serverChangeList.AddChange(change);
          flag1 = true;
        }
      }
    }
    return flag1;
  }

  private HashSet<int> GetLastStandingMinions(ZoneChangeList serverChangeList)
  {
    Network.HistTagChange gameOverTagChange = GameState.Get().GetRealTimeGameOverTagChange();
    if (gameOverTagChange == null)
      return (HashSet<int>) null;
    bool flag1 = gameOverTagChange.Value != 4;
    bool flag2 = gameOverTagChange.Value == 4;
    HashSet<int> other1 = new HashSet<int>();
    HashSet<int> other2 = new HashSet<int>();
    foreach (ZoneChange change in serverChangeList.GetChanges())
    {
      Entity entity = change.GetEntity();
      if (entity.IsMinion() && ((entity.GetZone() == TAG_ZONE.PLAY ? 1 : 0) & (!change.HasDestinationZoneTag() ? (false ? 1 : 0) : (change.GetDestinationZoneTag() != TAG_ZONE.PLAY ? 1 : 0))) != 0)
      {
        Player.Side controllerSide = entity.GetControllerSide();
        if (controllerSide == Player.Side.FRIENDLY & flag1)
          other1.Add(entity.GetEntityId());
        else if (controllerSide == Player.Side.OPPOSING & flag2)
          other2.Add(entity.GetEntityId());
      }
    }
    bool flag3 = other1.Count != 0;
    bool flag4 = other2.Count != 0;
    if (!flag3 && !flag4)
      return (HashSet<int>) null;
    bool flag5 = false;
    bool flag6 = false;
    foreach (PowerTaskList power1 in (QueueList<PowerTaskList>) GameState.Get().GetPowerProcessor().GetPowerQueue())
    {
      bool flag7 = false;
      foreach (PowerTask task in power1.GetTaskList())
      {
        Network.PowerHistory power2 = task.GetPower();
        if (ZoneMgr.IsHandledPower(power2))
        {
          ZoneChange zoneChange = (ZoneChange) null;
          switch (power2.Type)
          {
            case Network.PowerType.FULL_ENTITY:
              zoneChange = this.CreateZoneChangeFromFullEntity((Network.HistFullEntity) power2);
              break;
            case Network.PowerType.SHOW_ENTITY:
              zoneChange = this.CreateZoneChangeFromEntity(((Network.HistShowEntity) power2).Entity);
              break;
            case Network.PowerType.HIDE_ENTITY:
              zoneChange = this.CreateZoneChangeFromHideEntity((Network.HistHideEntity) power2);
              break;
            case Network.PowerType.TAG_CHANGE:
              zoneChange = this.CreateZoneChangeFromTagChange((Network.HistTagChange) power2);
              break;
            case Network.PowerType.META_DATA:
              zoneChange = this.CreateZoneChangeFromMetaData((Network.HistMetaData) power2);
              break;
            case Network.PowerType.CHANGE_ENTITY:
              zoneChange = this.CreateZoneChangeFromEntity(((Network.HistChangeEntity) power2).Entity);
              break;
          }
          if (zoneChange != null)
          {
            Entity entity = zoneChange.GetEntity();
            if (entity.IsMinion())
            {
              if (((entity.GetZone() != TAG_ZONE.PLAY || !zoneChange.HasDestinationZoneTag() ? 0 : (zoneChange.GetDestinationZoneTag() != TAG_ZONE.PLAY ? 1 : 0)) | (!zoneChange.HasDestinationZoneTag() ? (false ? 1 : 0) : (zoneChange.GetDestinationZoneTag() == TAG_ZONE.PLAY ? 1 : 0))) != 0)
              {
                switch (entity.GetControllerSide())
                {
                  case Player.Side.FRIENDLY:
                    flag5 = true;
                    break;
                  case Player.Side.OPPOSING:
                    flag6 = true;
                    break;
                }
              }
              if (!flag3 | flag5 && !flag4 | flag6)
              {
                flag7 = true;
                break;
              }
            }
          }
        }
      }
      if (flag7)
        break;
    }
    HashSet<int> lastStandingMinions = new HashSet<int>();
    if (flag1 && !flag5)
      lastStandingMinions.UnionWith((IEnumerable<int>) other1);
    if (flag2 && !flag6)
      lastStandingMinions.UnionWith((IEnumerable<int>) other2);
    return lastStandingMinions;
  }

  private bool PreventLastStandingMinionFromLeavingPlay(ZoneChangeList serverChangeList)
  {
    if (!GameState.Get().GetGameEntity().HasTag(GAME_TAG.LETTUCE_KEEP_LAST_STANDING_MINION_ACTOR))
      return false;
    HashSet<int> lastStandingMinions = this.GetLastStandingMinions(serverChangeList);
    if (lastStandingMinions == null || lastStandingMinions.Count == 0)
      return false;
    List<ZoneChange> zoneChangeList = new List<ZoneChange>();
    foreach (ZoneChange change in serverChangeList.GetChanges())
    {
      Entity entity = change.GetEntity();
      if (lastStandingMinions.Contains(entity.GetEntityId()))
      {
        if (change.HasDestinationPosition())
          zoneChangeList.Add(change);
        else if (change.HasDestinationZone())
          zoneChangeList.Add(change);
        else if (change.GetPowerTask()?.GetPower() is Network.HistTagChange power && power.Tag == 44 && power.Value == 0)
          zoneChangeList.Add(change);
      }
    }
    foreach (ZoneChange change in zoneChangeList)
    {
      change.GetPowerTask()?.SetCompleted(true);
      serverChangeList.RemoveChange(change);
    }
    return true;
  }

  private bool MergeServerChangeList(ZoneChangeList serverChangeList)
  {
    Log.Zone.Print("ZoneMgr.MergeServerChangeList() Start - serverChangeList: {0}, m_tempZoneMap.Count: {1}, m_tempEntityMap.Count: {2}", (object) serverChangeList, (object) this.m_tempZoneMap.Count, (object) this.m_tempEntityMap.Count);
    foreach (Zone zone in this.m_zones)
    {
      if (this.IsZoneInLocalHistory(zone))
      {
        ZoneMgr.TempZone tempZone = this.BuildTempZone(zone);
        this.m_tempZoneMap[zone] = tempZone;
        tempZone.PreprocessChanges();
      }
    }
    List<ZoneChange> changes = serverChangeList.GetChanges();
    for (int index = 0; index < changes.Count; ++index)
      this.TempApplyZoneChange(changes[index]);
    bool flag = false;
    foreach (ZoneMgr.TempZone tempZone in this.m_tempZoneMap.Values)
    {
      tempZone.Sort();
      tempZone.PostprocessChanges();
      Zone zone = tempZone.GetZone();
      Log.Zone.Print("ZoneMgr.MergeServerChangeList() zone: {0}", (object) zone);
      foreach (Card card in zone.GetCards())
        Log.Zone.Print("\tzone card: {0}", (object) card);
      Log.Zone.Print("ZoneMgr.MergeServerChangeList() tempZone: {0}", (object) tempZone);
      foreach (Entity entity in tempZone.GetEntities())
        Log.Zone.Print("\ttempZone entity: {0}", (object) entity);
      for (int slot = 1; slot <= zone.GetLastSlot(); ++slot)
      {
        Card cardAtSlot = zone.GetCardAtSlot(slot);
        if (!((UnityEngine.Object) cardAtSlot == (UnityEngine.Object) null))
        {
          Entity entity = cardAtSlot.GetEntity();
          if (cardAtSlot.GetPredictedZonePosition() != 0 && !tempZone.ContainsEntity(entity.GetEntityId()))
          {
            int insertionPosition = this.FindBestMinionInsertionPosition(tempZone, slot - 1, slot + 1);
            Log.Zone.Print("ZoneMgr.MergeServerChangeList() InsertEntityAtSlot() - tempZone: {0}, insertionPos: {1}, entity: {2}", (object) tempZone, (object) insertionPosition, (object) entity);
            tempZone.InsertEntityAtSlot(insertionPosition, entity, true);
          }
        }
      }
      if (tempZone.IsModified())
      {
        flag = true;
        for (int index = 1; index <= tempZone.GetLastSlot(); ++index)
        {
          Entity entityAtSlot = tempZone.GetEntityAtSlot(index);
          if (entityAtSlot != null)
          {
            Entity entity = entityAtSlot.GetCard().GetEntity();
            ZoneChange change = new ZoneChange();
            change.SetEntity(entity);
            change.SetDestinationZone(zone);
            change.SetDestinationZoneTag(zone.m_ServerTag);
            change.SetDestinationPosition(index);
            change.SetParentList(serverChangeList);
            Log.Zone.Print("ZoneMgr.MergeServerChangeList() - AddChange() tempZone:{0}, serverChangeList: {1}, graveyardChange: {2}", (object) tempZone, (object) serverChangeList, (object) change);
            serverChangeList.AddChange(change);
          }
        }
      }
    }
    this.m_tempZoneMap.Clear();
    this.m_tempEntityMap.Clear();
    return flag;
  }

  private bool IsZoneInLocalHistory(Zone zone)
  {
    foreach (ZoneChangeList zoneChangeList in this.m_localChangeListHistory)
    {
      foreach (ZoneChange change in zoneChangeList.GetChanges())
      {
        Zone sourceZone = change.GetSourceZone();
        Zone destinationZone = change.GetDestinationZone();
        if ((UnityEngine.Object) zone == (UnityEngine.Object) sourceZone || (UnityEngine.Object) zone == (UnityEngine.Object) destinationZone)
          return true;
      }
    }
    return false;
  }

  private void TempApplyZoneChange(ZoneChange change)
  {
    Log.Zone.Print("ZoneMgr.TempApplyZoneChange() - change: {0}, changeList: {1}", (object) change, (object) change.GetParentList());
    Network.PowerHistory power = change.GetPowerTask().GetPower();
    Entity entity = this.RegisterTempEntity(change.GetEntity());
    if (entity == null)
      return;
    if (!change.HasDestinationZoneChange())
    {
      GameUtils.ApplyPower(entity, power);
    }
    else
    {
      ZoneMgr.TempZone tempZoneForZone1 = this.FindTempZoneForZone(change.HasSourceZone() ? change.GetSourceZone() : this.FindZoneForEntity(entity));
      if (tempZoneForZone1 != null)
      {
        bool flag = tempZoneForZone1.RemoveEntity(entity);
        Log.Zone.Print("ZoneMgr.TempApplyZoneChange() - RemoveEntity() srcTempZone: {0}, tempEntity: {1}, result: {2}", (object) tempZoneForZone1, (object) entity, (object) flag);
      }
      GameUtils.ApplyPower(entity, power);
      ZoneMgr.TempZone tempZoneForZone2 = this.FindTempZoneForZone(change.GetDestinationZone());
      if (tempZoneForZone2 == null)
        return;
      tempZoneForZone2.AddEntity(entity);
      Log.Zone.Print("ZoneMgr.TempApplyZoneChange() - AddEntity() dstTempZone: {0}, tempEntity: {1}", (object) tempZoneForZone2, (object) entity);
    }
  }

  private ZoneMgr.TempZone BuildTempZone(Zone zone)
  {
    ZoneMgr.TempZone tempZone = new ZoneMgr.TempZone();
    tempZone.SetZone(zone);
    List<Card> cards = zone.GetCards();
    for (int index = 0; index < cards.Count; ++index)
    {
      Card card = cards[index];
      if (card.GetPredictedZonePosition() == 0 && (!card.IsBeingDragged || zone is ZoneHand))
      {
        Entity entity = this.RegisterTempEntity(card.GetEntity());
        if (entity != null)
          tempZone.AddInitialEntity(entity);
      }
    }
    return tempZone;
  }

  private ZoneMgr.TempZone FindTempZoneForZone(Zone zone)
  {
    if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
      return (ZoneMgr.TempZone) null;
    ZoneMgr.TempZone tempZoneForZone = (ZoneMgr.TempZone) null;
    this.m_tempZoneMap.TryGetValue(zone, out tempZoneForZone);
    return tempZoneForZone;
  }

  private int FindBestMinionInsertionPosition(ZoneMgr.TempZone tempZone, int leftPos, int rightPos)
  {
    Zone zone = tempZone.GetZone();
    int slot1 = 0;
    for (int slot2 = leftPos; slot2 >= 1; --slot2)
    {
      Card cardAtSlot = zone.GetCardAtSlot(slot2);
      if (!((UnityEngine.Object) cardAtSlot == (UnityEngine.Object) null))
      {
        Entity entity = cardAtSlot.GetEntity();
        slot1 = tempZone.FindEntityPosWithReplacements(entity.GetEntityId());
        if (slot1 != 0)
          break;
      }
    }
    int slot3;
    if (slot1 == 0)
    {
      slot3 = 1;
    }
    else
    {
      Entity entityAtSlot1 = tempZone.GetEntityAtSlot(slot1);
      slot3 = slot1 + 1;
      if (entityAtSlot1 != null)
      {
        int entityId = entityAtSlot1.GetEntityId();
        for (; slot3 <= tempZone.GetLastSlot(); ++slot3)
        {
          Entity entityAtSlot2 = tempZone.GetEntityAtSlot(slot3);
          if (entityAtSlot2 == null || entityAtSlot2.GetCreatorId() != entityId || zone.ContainsCard(entityAtSlot2.GetCard()))
            break;
        }
      }
    }
    int slot4 = 0;
    for (int slot5 = rightPos; slot5 <= zone.GetLastSlot(); ++slot5)
    {
      Card cardAtSlot = zone.GetCardAtSlot(slot5);
      if (!((UnityEngine.Object) cardAtSlot == (UnityEngine.Object) null))
      {
        Entity entity = cardAtSlot.GetEntity();
        slot4 = tempZone.FindEntityPosWithReplacements(entity.GetEntityId());
        if (slot4 != 0)
          break;
      }
    }
    int num;
    if (slot4 <= 0)
    {
      num = tempZone.GetLastSlot() + 1;
    }
    else
    {
      Entity entityAtSlot3 = tempZone.GetEntityAtSlot(slot4);
      int slot6 = slot4 - 1;
      if (entityAtSlot3 != null)
      {
        int entityId = entityAtSlot3.GetEntityId();
        for (; slot6 > 0; --slot6)
        {
          Entity entityAtSlot4 = tempZone.GetEntityAtSlot(slot6);
          if (entityAtSlot4 == null || entityAtSlot4.GetCreatorId() == 0 || entityAtSlot4.GetCreatorId() != entityId || zone.ContainsCard(entityAtSlot4.GetCard()))
            break;
        }
      }
      num = slot6 + 1;
    }
    return Mathf.CeilToInt(0.5f * (float) (slot3 + num));
  }

  private int ValidatePredictedMinion(ZoneMgr.TempZone tempZone, int predictedPos) => tempZone.GetZone().m_ServerTag != TAG_ZONE.PLAY || tempZone.GetLastSlot() != 7 ? predictedPos : -1;

  public int GetNextLocalChangeListId()
  {
    int localChangeListId = this.m_nextLocalChangeListId;
    this.m_nextLocalChangeListId = this.m_nextLocalChangeListId == int.MaxValue ? 1 : this.m_nextLocalChangeListId + 1;
    return localChangeListId;
  }

  private int GetNextServerChangeListId()
  {
    int serverChangeListId = this.m_nextServerChangeListId;
    this.m_nextServerChangeListId = this.m_nextServerChangeListId == int.MaxValue ? 1 : this.m_nextServerChangeListId + 1;
    return serverChangeListId;
  }

  private int FindTriggeredActiveLocalChangeIndex(Card card)
  {
    for (int index = 0; index < this.m_activeLocalChangeLists.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_activeLocalChangeLists[index].GetLocalTriggerCard() == (UnityEngine.Object) card)
        return index;
    }
    return -1;
  }

  private int FindTriggeredPendingLocalChangeIndex(Card card)
  {
    for (int index = 0; index < this.m_pendingLocalChangeLists.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_pendingLocalChangeLists[index].GetLocalTriggerCard() == (UnityEngine.Object) card)
        return index;
    }
    return -1;
  }

  private bool AutoCorrectZonesAfterServerChange(CancellationToken token)
  {
    if (this.HasActiveLocalChange())
    {
      Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange() - HasActiveLocalChange()");
      return false;
    }
    if (this.HasPendingLocalChange())
    {
      Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange() - HasPendingLocalChange()");
      return false;
    }
    if (this.HasActiveServerChange())
    {
      Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange() - HasActiveServerChange()");
      return false;
    }
    if (this.HasPendingServerChange())
    {
      Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange() - HasPendingServerChange()");
      return false;
    }
    if (this.HasPredictedPositions())
    {
      Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange() - HasPredictedPositions()");
      return false;
    }
    if (this.HasPredictedCards())
    {
      Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange() - HasPredictedCards()");
      return false;
    }
    if (this.HasPredictedMovedMinion())
    {
      Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange() - HasPredictedMovedMinion()");
      return false;
    }
    Log.Zone.Print("ZoneMgr.AutoCorrectZonesAfterServerChange()");
    this.AutoCorrectZones(token, this.ShouldIgnorePosChange());
    return true;
  }

  public CancellationToken GetCancellationToken() => this.m_updateChangeCancelTokenSource.Token;

  public void AutoCorrectZones(CancellationToken token, bool ignorePurePosChange)
  {
    ZoneChangeList changeList = (ZoneChangeList) null;
    foreach (Zone zone in this.FindZonesOfType<Zone>(Player.Side.FRIENDLY))
    {
      if (GameState.Get().GetGameEntity() == null || GameState.Get().GetGameEntity().ShouldAutoCorrectZone(zone))
      {
        foreach (Card card in zone.GetCards())
        {
          Entity entity = card.GetEntity();
          TAG_ZONE tag1 = entity.GetZone();
          int controllerId1 = entity.GetControllerId();
          int pos = entity.GetZonePosition();
          TAG_ZONE serverTag = zone.m_ServerTag;
          int controllerId2 = zone.GetControllerId();
          int zonePosition = card.GetZonePosition();
          TAG_ZONE tag2 = entity.GetTag<TAG_ZONE>(GAME_TAG.FAKE_ZONE);
          if (tag2 != TAG_ZONE.INVALID)
          {
            tag1 = tag2;
            controllerId1 = controllerId2;
          }
          int tag3 = entity.GetTag(GAME_TAG.FAKE_ZONE_POSITION);
          if (tag3 > 0)
            pos = tag3;
          int num1 = tag1 == serverTag ? 1 : 0;
          bool flag1 = controllerId1 == controllerId2;
          bool flag2 = pos == 0 || pos == zonePosition;
          int num2 = flag1 ? 1 : 0;
          if ((num1 & num2 & (flag2 ? 1 : 0)) == 0)
          {
            if (changeList == null)
            {
              int localChangeListId = this.GetNextLocalChangeListId();
              Log.Zone.Print("ZoneMgr.AutoCorrectZones() CreateLocalChangeList - changeListId={0}", (object) localChangeListId);
              changeList = new ZoneChangeList();
              changeList.SetId(localChangeListId);
              changeList.SetIgnoreCardZonePurePosChanges(ignorePurePosChange);
            }
            ZoneChange change = new ZoneChange();
            change.SetEntity(entity);
            change.SetSourcePosition(zonePosition);
            change.SetDestinationZoneTag(tag1);
            change.SetDestinationZone(this.FindZoneForEntity(entity));
            change.SetDestinationControllerId(controllerId1);
            change.SetDestinationPosition(pos);
            Log.Zone.Print("ZoneMgr.AutoCorrectZones() - AddChange() changeList: {0}, change: {1}", (object) changeList, (object) change);
            changeList.AddChange(change);
          }
        }
      }
    }
    if (changeList == null)
      return;
    this.ProcessLocalChangeList(changeList, token);
  }

  public void ProcessGeneratedLocalChangeLists(
    List<ZoneChangeList> generatedChangeLists,
    CancellationToken token)
  {
    foreach (ZoneChangeList generatedChangeList in generatedChangeLists)
    {
      int localChangeListId = this.GetNextLocalChangeListId();
      generatedChangeList.SetId(localChangeListId);
      this.ProcessLocalChangeList(generatedChangeList, token);
    }
  }

  public void OnHealingDoesDamageEntityMousedOver()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnHealingDoesDamageEntityMousedOver();
  }

  public void OnHealingDoesDamageEntityMousedOut()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnHealingDoesDamageEntityMousedOut();
  }

  public void OnLifestealDoesDamageEntityMousedOver()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnLifestealDoesDamageEntityMousedOver();
  }

  public void OnLifestealDoesDamageEntityMousedOut()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnLifestealDoesDamageEntityMousedOut();
  }

  public void OnHealingDoesDamageEntityEnteredPlay()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnHealingDoesDamageEntityEnteredPlay();
  }

  public void OnLifestealDoesDamageEntityEnteredPlay()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnLifestealDoesDamageEntityEnteredPlay();
  }

  public void OnSpellPowerEntityMousedOver(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnSpellPowerEntityMousedOver(spellSchool);
  }

  public void OnSpellPowerEntityMousedOut(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnSpellPowerEntityMousedOut(spellSchool);
  }

  public void OnDiedLastCombatMousedOver()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnDiedLastCombatMousedOver();
  }

  public void OnDiedLastCombatMousedOut()
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnDiedLastCombatMousedOut();
  }

  public void OnSpellPowerEntityEnteredPlay(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    foreach (Zone zone in this.FindZonesForSide(Player.Side.FRIENDLY))
      zone.OnSpellPowerEntityEnteredPlay(spellSchool);
  }

  public Entity GetLettuceAbilitiesSourceEntity() => this.m_lettuceZoneController?.GetLettuceAbilitiesSourceEntity();

  public void DisplayLettuceAbilitiesForEntity(Entity entity) => this.m_lettuceZoneController?.DisplayLettuceAbilitiesForEntity(entity);

  public void DismissMercenariesAbilityTray() => this.m_lettuceZoneController?.ClearDisplayedLettuceAbilities();

  public void TemporarilyDismissMercenariesAbilityTray() => this.m_lettuceZoneController?.ClearDisplayedLettuceAbilities(false, true);

  public void DisplayLettuceAbilitiesForPreviouslySelectedCard() => this.m_lettuceZoneController?.DisplayLettuceAbilitiesForPreviouslySelectedCard();

  public List<Card> GetDisplayedLettuceAbilityCards() => this.m_lettuceZoneController?.GetDisplayedLettuceAbilityCards();

  public bool IsMercenariesAbilityTrayVisible() => this.m_lettuceZoneController?.GetAbilityTray()?.IsVisible() ?? false;

  public LettuceZoneController GetLettuceZoneController() => this.m_lettuceZoneController;

  public delegate void ChangeCompleteCallback(ZoneChangeList changeList, object userData);

  private class TempZone
  {
    private Zone m_zone;
    private bool m_modified;
    private List<Entity> m_prevEntities = new List<Entity>();
    private List<Entity> m_entities = new List<Entity>();
    private Map<int, int> m_replacedEntities = new Map<int, int>();

    public Zone GetZone() => this.m_zone;

    public void SetZone(Zone zone) => this.m_zone = zone;

    public bool IsModified() => this.m_modified;

    public List<Entity> GetEntities() => this.m_entities;

    public Entity GetEntityAtSlot(int slot)
    {
      int num = 1;
      for (int index = 0; index < this.m_entities.Count; ++index)
      {
        Entity entity = this.m_entities[index];
        if (entity != null)
        {
          if (num == slot)
            return entity;
          ++num;
        }
      }
      return (Entity) null;
    }

    public void AddInitialEntity(Entity entity) => this.m_entities.Add(entity);

    public bool CanAcceptEntity(Entity entity) => (UnityEngine.Object) ZoneMgr.Get().FindZoneForEntityAndZoneTag(entity, this.m_zone.m_ServerTag) == (UnityEngine.Object) this.m_zone;

    public void AddEntity(Entity entity)
    {
      if (!this.CanAcceptEntity(entity) || this.m_entities.Contains(entity))
        return;
      this.m_entities.Add(entity);
      this.m_modified = true;
    }

    public void InsertEntityAtIndex(int index, Entity entity, bool bypassCanAcceptEntityCheck = false)
    {
      if (!bypassCanAcceptEntityCheck && !this.CanAcceptEntity(entity) || index < 0 || index > this.m_entities.Count || index < this.m_entities.Count && this.m_entities[index] == entity)
        return;
      this.m_entities.Insert(index, entity);
      this.m_modified = true;
    }

    public void InsertEntityAtSlot(int slot, Entity entity, bool bypassCanAcceptEntityCheck = false)
    {
      int index;
      for (index = 0; index < this.m_entities.Count; ++index)
      {
        int slotOfEntitAtIndex = this.GetSlotOfEntitAtIndex(index);
        if (slot <= slotOfEntitAtIndex)
          break;
      }
      this.InsertEntityAtIndex(index, entity, bypassCanAcceptEntityCheck);
    }

    public bool RemoveEntity(Entity entity)
    {
      if (!this.m_entities.Remove(entity))
        return false;
      this.m_modified = true;
      return true;
    }

    public bool RemoveEntityById(int entityId)
    {
      Entity entity1 = (Entity) null;
      foreach (Entity entity2 in this.m_entities)
      {
        if (entity2.GetEntityId() == entityId)
        {
          entity1 = entity2;
          break;
        }
      }
      if (entity1 == null)
        return false;
      this.m_entities.Remove(entity1);
      this.m_modified = true;
      return true;
    }

    public int GetLastSlot() => this.m_entities.Count;

    public int FindEntityPos(int entityId) => 1 + this.m_entities.FindIndex((Predicate<Entity>) (currEntity => currEntity.GetEntityId() == entityId));

    public bool ContainsEntity(int entityId) => this.FindEntityPos(entityId) > 0;

    public int FindEntityPosWithReplacements(int entityId)
    {
      for (int entityId1 = entityId; entityId1 != 0; this.m_replacedEntities.TryGetValue(entityId1, out entityId1))
      {
        int index = this.m_entities.FindIndex((Predicate<Entity>) (currEntity => currEntity.GetEntityId() == entityId1));
        int withReplacements = 0;
        if (index >= 0 && index < this.m_entities.Count)
          withReplacements = this.GetSlotOfEntitAtIndex(index);
        if (withReplacements > 0)
          return withReplacements;
      }
      return 0;
    }

    private int GetSlotOfEntitAtIndex(int index)
    {
      if (index < 0 || index >= this.m_entities.Count)
        return -1;
      Entity entity = this.m_entities[index];
      if (entity == null)
        return -1;
      entity.GetEntityId();
      int slotOfEntitAtIndex = 1;
      for (int index1 = 0; index1 <= index; ++index1)
      {
        if (index1 == index)
          return slotOfEntitAtIndex;
        if (this.m_entities[index1] != null)
          ++slotOfEntitAtIndex;
      }
      return -1;
    }

    public void Sort()
    {
      if (this.m_modified)
      {
        this.m_entities.Sort(new Comparison<Entity>(this.SortComparison));
      }
      else
      {
        Entity[] array = this.m_entities.ToArray();
        this.m_entities.Sort(new Comparison<Entity>(this.SortComparison));
        for (int index = 0; index < this.m_entities.Count; ++index)
        {
          if (array[index] != this.m_entities[index])
          {
            this.m_modified = true;
            break;
          }
        }
      }
    }

    public void PreprocessChanges()
    {
      this.m_prevEntities.Clear();
      for (int index = 0; index < this.m_entities.Count; ++index)
        this.m_prevEntities.Add(this.m_entities[index]);
    }

    public void PostprocessChanges()
    {
      for (int index = 0; index < this.m_prevEntities.Count; ++index)
      {
        if (index >= this.m_entities.Count)
          break;
        Entity prevEntity = this.m_prevEntities[index];
        if (this.m_entities.FindIndex((Predicate<Entity>) (currEntity => currEntity == prevEntity)) < 0)
        {
          Entity entity = this.m_entities[index];
          if (!this.m_prevEntities.Contains(entity))
            this.m_replacedEntities[prevEntity.GetEntityId()] = entity.GetEntityId();
        }
      }
    }

    public override string ToString() => string.Format("{0} ({1} entities)", (object) this.m_zone, (object) this.m_entities.Count);

    private int SortComparison(Entity entity1, Entity entity2) => entity1.GetZonePosition() - entity2.GetZonePosition();
  }
}
