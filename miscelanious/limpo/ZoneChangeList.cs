using Blizzard.T5.Core;
using Cysharp.Threading.Tasks;
using PegasusGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class ZoneChangeList
{
  private int m_id;
  private int m_predictedPosition;
  private bool m_ignoreCardZoneChanges;
  private bool m_canceledChangeList;
  private bool m_ignoreCardZonePurePosChanges;
  private PowerTaskList m_taskList;
  private List<ZoneChange> m_changes = new List<ZoneChange>();
  private HashSet<Zone> m_dirtyZones = new HashSet<Zone>();
  private List<ZoneChangeList> m_generatedLocalChangeLists = new List<ZoneChangeList>();
  private bool m_complete;
  private ZoneMgr.ChangeCompleteCallback m_completeCallback;
  private object m_completeCallbackUserData;

  public int GetId() => this.m_id;

  public void SetId(int id) => this.m_id = id;

  public bool IsLocal() => this.m_taskList == null;

  public int GetPredictedPosition() => this.m_predictedPosition;

  public void SetPredictedPosition(int pos) => this.m_predictedPosition = pos;

  public void SetIgnoreCardZoneChanges(bool ignore) => this.m_ignoreCardZoneChanges = ignore;

  public void SetIgnoreCardZonePurePosChanges(bool ignore) => this.m_ignoreCardZonePurePosChanges = ignore;

  public bool ShouldIgnoreCardZonePurePosChanges() => this.m_ignoreCardZonePurePosChanges;

  public bool IsCanceledChangeList() => this.m_canceledChangeList;

  public void SetCanceledChangeList(bool canceledChangeList) => this.m_canceledChangeList = canceledChangeList;

  public void SetZoneInputBlocking(bool block)
  {
    for (int index = 0; index < this.m_changes.Count; ++index)
    {
      ZoneChange change = this.m_changes[index];
      Zone sourceZone = change.GetSourceZone();
      if ((UnityEngine.Object) sourceZone != (UnityEngine.Object) null)
        sourceZone.BlockInput(block);
      Zone destinationZone = change.GetDestinationZone();
      if ((UnityEngine.Object) destinationZone != (UnityEngine.Object) null)
        destinationZone.BlockInput(block);
    }
  }

  public bool IsComplete() => this.m_complete;

  public void SetCompleteCallback(ZoneMgr.ChangeCompleteCallback callback) => this.m_completeCallback = callback;

  public void SetCompleteCallbackUserData(object userData) => this.m_completeCallbackUserData = userData;

  public void FireCompleteCallback()
  {
    this.DebugPrint("ZoneChangeList.FireCompleteCallback() - m_id={0} m_taskList={1} m_changes.Count={2} m_complete={3} m_completeCallback={4}", (object) this.m_id, this.m_taskList == null ? (object) "(null)" : (object) this.m_taskList.GetId().ToString(), (object) this.m_changes.Count, (object) this.m_complete, this.m_completeCallback == null ? (object) "(null)" : (object) "(not null)");
    if (this.m_completeCallback == null)
      return;
    this.m_completeCallback(this, this.m_completeCallbackUserData);
  }

  public PowerTaskList GetTaskList() => this.m_taskList;

  public void SetTaskList(PowerTaskList taskList) => this.m_taskList = taskList;

  public List<ZoneChange> GetChanges() => this.m_changes;

  public ZoneChange GetLocalTriggerChange()
  {
    if (!this.IsLocal())
      return (ZoneChange) null;
    return this.m_changes.Count <= 0 ? (ZoneChange) null : this.m_changes[0];
  }

  public Card GetLocalTriggerCard() => this.GetLocalTriggerChange()?.GetEntity().GetCard();

  public void AddChange(ZoneChange change) => this.m_changes.Add(change);

  public void RemoveChange(ZoneChange change) => this.m_changes.Remove(change);

  public async UniTaskVoid ProcessChanges(CancellationToken token)
  {
    ZoneChangeList zoneChangeList = this;
    zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - m_id={0} m_taskList={1} m_changes.Count={2}", (object) zoneChangeList.m_id, zoneChangeList.m_taskList == null ? (object) "(null)" : (object) zoneChangeList.m_taskList.GetId().ToString(), (object) zoneChangeList.m_changes.Count);
    UniTask uniTask;
    while (GameState.Get().MustWaitForChoices())
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    HashSet<Entity> loadingEntities = new HashSet<Entity>();
    Map<Player, DyingSecretGroup> dyingSecretMap = (Map<Player, DyingSecretGroup>) null;
    for (int i = 0; i < zoneChangeList.m_changes.Count; ++i)
    {
      ZoneChange change = zoneChangeList.m_changes[i];
      zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - processing index={0} change={1}", (object) i, (object) change);
      Entity entity = change.GetEntity();
      Card card = entity.GetCard();
      PowerTask powerTask = change.GetPowerTask();
      int srcControllerId = entity.GetControllerId();
      int srcPos = 0;
      Zone srcZone = (Zone) null;
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      {
        srcPos = card.GetZonePosition();
        srcZone = card.GetZone();
      }
      int dstControllerId = change.GetDestinationControllerId();
      int dstPos = change.GetDestinationPosition();
      Zone dstZone = change.GetDestinationZone();
      TAG_ZONE dstZoneTag = change.GetDestinationZoneTag();
      if (powerTask != null)
      {
        if (!powerTask.IsCompleted())
        {
          if (loadingEntities.Contains(entity))
          {
            bool flag = true;
            if ((entity.GetZonePosition() != 0 ? 0 : (entity.GetZone() == TAG_ZONE.PLAY ? 1 : 0)) != 0 && (!(powerTask.GetPower() is Network.HistTagChange power) || power.Entity != entity.GetEntityId() ? 0 : (power.Tag == 263 ? 1 : 0)) != 0)
              flag = false;
            if (flag)
            {
              zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - START waiting for {0} to load (powerTask=(not null))", (object) card);
              uniTask = zoneChangeList.WaitForAndRemoveLoadingEntity(loadingEntities, entity, card, token);
              await uniTask;
              zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - END waiting for {0} to load (powerTask=(not null))", (object) card);
            }
          }
          while (!GameState.Get().GetPowerProcessor().CanDoTask(powerTask))
          {
            uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
            await uniTask;
          }
          while (zoneChangeList.ShouldWaitForOldHero(entity))
          {
            uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
            await uniTask;
          }
          powerTask.DoTask();
          if (entity.IsLoadingAssets())
            loadingEntities.Add(entity);
        }
        else
          continue;
      }
      if (!zoneChangeList.ShouldIgnoreZoneChange(entity))
      {
        bool zoneChanged = dstZoneTag != TAG_ZONE.INVALID && (UnityEngine.Object) srcZone != (UnityEngine.Object) dstZone;
        bool controllerChanged = dstControllerId != 0 && srcControllerId != dstControllerId;
        bool posChanged = zoneChanged || dstPos != 0 && srcPos != dstPos;
        bool revealed = powerTask != null && powerTask.GetPower().Type == Network.PowerType.SHOW_ENTITY;
        if ((bool) UniversalInputManager.UsePhoneUI && zoneChangeList.IsDisplayableDyingSecret(entity, card, srcZone, dstZone))
        {
          if (dyingSecretMap == null)
            dyingSecretMap = new Map<Player, DyingSecretGroup>();
          Player controller = card.GetController();
          DyingSecretGroup dyingSecretGroup;
          if (!dyingSecretMap.TryGetValue(controller, out dyingSecretGroup))
          {
            dyingSecretGroup = new DyingSecretGroup();
            dyingSecretMap.Add(controller, dyingSecretGroup);
          }
          dyingSecretGroup.AddCard(card);
        }
        if (zoneChanged | controllerChanged | revealed)
        {
          bool transitionedZones = zoneChanged | controllerChanged;
          bool flag = revealed && entity.GetZone() == TAG_ZONE.SECRET;
          if (transitionedZones || !flag)
          {
            if ((UnityEngine.Object) srcZone != (UnityEngine.Object) null)
              zoneChangeList.m_dirtyZones.Add(srcZone);
            if ((UnityEngine.Object) dstZone != (UnityEngine.Object) null)
              zoneChangeList.m_dirtyZones.Add(dstZone);
            zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - TRANSITIONING card {0} to {1}", (object) card, (object) dstZone);
          }
          if (loadingEntities.Contains(entity))
          {
            zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - START waiting for {0} to load (zoneChanged={1} controllerChanged={2} powerTask=(not null))", (object) card, (object) zoneChanged, (object) controllerChanged);
            uniTask = zoneChangeList.WaitForAndRemoveLoadingEntity(loadingEntities, entity, card, token);
            await uniTask;
            zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - END waiting for {0} to load (zoneChanged={1} controllerChanged={2} powerTask=(not null))", (object) card, (object) zoneChanged, (object) controllerChanged);
          }
          if (!card.IsActorReady() || card.IsBeingDrawnByOpponent())
          {
            zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - START waiting for {0} to become ready (zoneChanged={1} controllerChanged={2} powerTask=(not null))", (object) card, (object) zoneChanged, (object) controllerChanged);
            if (card.GetPrevZone() is ZoneDeck && card.GetZone() is ZoneHand && card.GetPrevZone().GetController() == card.GetZone().GetController() && TurnStartManager.Get().IsCardDrawHandled(card))
              TurnStartManager.Get().DrawCardImmediately(card);
            while (!card.IsActorReady() || card.IsBeingDrawnByOpponent())
            {
              uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
              await uniTask;
            }
            zoneChangeList.DebugPrint("ZoneChangeList.ProcessChanges() - END waiting for {0} to become ready (zoneChanged={1} controllerChanged={2} powerTask=(not null))", (object) card, (object) zoneChanged, (object) controllerChanged);
          }
          Log.Zone.Print("ZoneChangeList.ProcessChanges() - id={0} local={1} {2} zone from {3} -> {4}", (object) zoneChangeList.m_id, (object) zoneChangeList.IsLocal(), (object) card, (object) srcZone, (object) dstZone);
          if (transitionedZones)
          {
            if (srcZone is ZonePlay && srcZone.m_Side == Player.Side.OPPOSING && dstZone is ZoneHand && dstZone.m_Side == Player.Side.OPPOSING)
            {
              Log.FaceDownCard.Print("ZoneChangeList.ProcessChanges() - id={0} {1}.TransitionToZone(): {2} -> {3}", (object) zoneChangeList.m_id, (object) card, (object) srcZone, (object) dstZone);
              zoneChangeList.m_taskList.DebugDump(Log.FaceDownCard);
            }
            card.SetZonePosition(0);
            card.TransitionToZone(dstZone, change);
          }
          else if (revealed)
            card.UpdateActor();
          if (card.IsActorLoading())
            loadingEntities.Add(entity);
        }
        if (posChanged && (!zoneChangeList.ShouldIgnoreCardZonePurePosChanges() || (UnityEngine.Object) srcZone != (UnityEngine.Object) dstZone || entity.GetZone() != TAG_ZONE.PLAY))
        {
          if ((UnityEngine.Object) srcZone != (UnityEngine.Object) null && !zoneChanged && !controllerChanged)
            zoneChangeList.m_dirtyZones.Add(srcZone);
          if ((UnityEngine.Object) dstZone != (UnityEngine.Object) null)
            zoneChangeList.m_dirtyZones.Add(dstZone);
          if (card.m_minionWasMovedFromSrcToDst != null && !zoneChangeList.IsLocal())
          {
            zoneChangeList.GenerateLocalChangelistForMovedMinionWhileProcessingServerChangelist(card);
          }
          else
          {
            Log.Zone.Print("ZoneChangeList.ProcessChanges() - id={0} local={1} {2} pos from {3} -> {4}", (object) zoneChangeList.m_id, (object) zoneChangeList.IsLocal(), (object) card, (object) srcPos, (object) dstPos);
            card.SetZonePosition(dstPos);
          }
        }
        change = (ZoneChange) null;
        entity = (Entity) null;
        card = (Card) null;
        powerTask = (PowerTask) null;
        srcZone = (Zone) null;
        dstZone = (Zone) null;
      }
    }
    while (zoneChangeList.ShowNewHeroStats())
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    if (zoneChangeList.IsCanceledChangeList())
      zoneChangeList.SetZoneInputBlocking(false);
    zoneChangeList.ProcessDyingSecrets(dyingSecretMap);
    ZoneMgr.Get().ProcessGeneratedLocalChangeLists(zoneChangeList.m_generatedLocalChangeLists, token);
    zoneChangeList.UpdateDirtyZones(loadingEntities, token).Forget();
  }

  private void GenerateLocalChangelistForMovedMinionWhileProcessingServerChangelist(Card card)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null || card.m_minionWasMovedFromSrcToDst == null)
      return;
    ZoneChangeList zoneChangeList = new ZoneChangeList();
    ZoneChange change = new ZoneChange();
    change.SetEntity(card.GetEntity());
    change.SetSourcePosition(card.GetZonePosition());
    change.SetDestinationPosition(card.GetEntity().GetRealTimeZonePosition());
    Log.Zone.Print("ZoneMgr.GenerateLocalChangelistForMovedMinionWhileProcessingServerChangelist() - AddChange() changeList: {0}, change: {1}", (object) zoneChangeList, (object) change);
    zoneChangeList.AddChange(change);
    this.m_generatedLocalChangeLists.Add(zoneChangeList);
  }

  public override string ToString() => string.Format("id={0} changes={1} complete={2} local={3} localTrigger=[{4}]", (object) this.m_id, (object) this.m_changes.Count, (object) this.m_complete, (object) this.IsLocal(), (object) this.GetLocalTriggerChange());

  private bool IsDisplayableDyingSecret(Entity entity, Card card, Zone srcZone, Zone dstZone) => entity.IsSecret() && srcZone is ZoneSecret && dstZone is ZoneGraveyard;

  private void ProcessDyingSecrets(Map<Player, DyingSecretGroup> dyingSecretMap)
  {
    if (dyingSecretMap == null)
      return;
    Map<Player, DeadSecretGroup> deadSecretMap = (Map<Player, DeadSecretGroup>) null;
    foreach (KeyValuePair<Player, DyingSecretGroup> dyingSecret in dyingSecretMap)
    {
      Player key = dyingSecret.Key;
      DyingSecretGroup dyingSecretGroup = dyingSecret.Value;
      Card mainCard = dyingSecretGroup.GetMainCard();
      List<Card> cards = dyingSecretGroup.GetCards();
      List<Actor> actors = dyingSecretGroup.GetActors();
      for (int index = 0; index < cards.Count; ++index)
      {
        Card card = cards[index];
        Actor oldActor = actors[index];
        if (card.WasSecretTriggered())
        {
          oldActor.Destroy();
        }
        else
        {
          if ((UnityEngine.Object) card == (UnityEngine.Object) mainCard && card.CanShowSecretDeath())
            card.ShowSecretDeath(oldActor);
          else
            oldActor.Destroy();
          if (deadSecretMap == null)
            deadSecretMap = new Map<Player, DeadSecretGroup>();
          DeadSecretGroup deadSecretGroup;
          if (!deadSecretMap.TryGetValue(key, out deadSecretGroup))
          {
            deadSecretGroup = new DeadSecretGroup();
            deadSecretGroup.SetMainCard(mainCard);
            deadSecretMap.Add(key, deadSecretGroup);
          }
          deadSecretGroup.AddCard(card);
        }
      }
    }
    BigCard.Get().ShowSecretDeaths(deadSecretMap);
  }

  private async UniTask WaitForAndRemoveLoadingEntity(
    HashSet<Entity> loadingEntities,
    Entity entity,
    Card card,
    CancellationToken token)
  {
    while (this.IsEntityLoading(entity, card))
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    loadingEntities.Remove(entity);
  }

  private bool IsEntityLoading(Entity entity, Card card) => entity.IsLoadingAssets() || (UnityEngine.Object) card != (UnityEngine.Object) null && card.IsActorLoading();

  private async UniTaskVoid UpdateDirtyZones(
    HashSet<Entity> loadingEntities,
    CancellationToken token)
  {
    ZoneChangeList zoneChangeList = this;
    zoneChangeList.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} loadingEntities.Count={1} m_dirtyZones.Count={2}", (object) zoneChangeList.m_id, (object) loadingEntities.Count, (object) zoneChangeList.m_dirtyZones.Count);
    foreach (Entity entity in loadingEntities)
    {
      Card card = entity.GetCard();
      zoneChangeList.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} START waiting for {1} to load (card={2})", (object) zoneChangeList.m_id, (object) entity, (object) card);
      while (zoneChangeList.IsEntityLoading(entity, card))
        await UniTask.Yield(PlayerLoopTiming.Update, token);
      zoneChangeList.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} END waiting for {1} to load (card={2})", (object) zoneChangeList.m_id, (object) entity, (object) card);
      card = (Card) null;
    }
    UniTaskVoid uniTaskVoid;
    if (zoneChangeList.IsDeathBlock())
    {
      float num = ZoneMgr.Get().RemoveNextDeathBlockLayoutDelaySec();
      if ((double) num >= 0.0)
        await UniTask.Delay(TimeSpan.FromSeconds((double) num), cancellationToken: token);
      foreach (Zone dirtyZone in zoneChangeList.m_dirtyZones)
        dirtyZone.UpdateLayout();
      zoneChangeList.m_dirtyZones.Clear();
    }
    else
    {
      Zone[] array = new Zone[zoneChangeList.m_dirtyZones.Count];
      zoneChangeList.m_dirtyZones.CopyTo(array);
      foreach (Zone zone in array)
      {
        zoneChangeList.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} START waiting for zone {1}", (object) zoneChangeList.m_id, (object) zone);
        if (zone is ZoneHand)
        {
          uniTaskVoid = zoneChangeList.ZoneHand_UpdateLayout((ZoneHand) zone, token);
          uniTaskVoid.Forget();
        }
        else
        {
          zone.AddUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(zoneChangeList.OnUpdateLayoutComplete));
          zone.UpdateLayout();
        }
      }
    }
    uniTaskVoid = zoneChangeList.FinishWhenPossible(token);
    uniTaskVoid.Forget();
  }

  private bool IsDeathBlock() => this.m_taskList != null && this.m_taskList.IsDeathBlock();

  private async UniTaskVoid ZoneHand_UpdateLayout(
    ZoneHand zoneHand,
    CancellationToken token)
  {
    ZoneChangeList zoneChangeList = this;
    while (!((UnityEngine.Object) zoneHand.GetCards().Find((Predicate<Card>) (card => (!((UnityEngine.Object) TurnStartManager.Get() != (UnityEngine.Object) null) || !TurnStartManager.Get().IsCardDrawHandled(card)) && !card.IsDoNotSort() && !card.IsActorReady())) == (UnityEngine.Object) null))
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    zoneHand.AddUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(zoneChangeList.OnUpdateLayoutComplete));
    zoneHand.UpdateLayout();
  }

  private void OnUpdateLayoutComplete(Zone zone, object userData)
  {
    this.DebugPrint("ZoneChangeList.OnUpdateLayoutComplete() - m_id={0} END waiting for zone {1}", (object) this.m_id, (object) zone);
    this.m_dirtyZones.Remove(zone);
  }

  private Entity GetNewHeroPlayedFromPowerTaskList()
  {
    PowerTaskList taskList = this.GetTaskList();
    if (taskList == null)
      return (Entity) null;
    Network.HistBlockStart blockStart = taskList.GetBlockStart();
    if (blockStart == null)
      return (Entity) null;
    if (blockStart.BlockType != HistoryBlock.Type.PLAY)
      return (Entity) null;
    Entity sourceEntity = taskList.GetSourceEntity();
    if (sourceEntity == null)
    {
      Log.Zone.PrintWarning("ZoneChangelist.GetNewHeroPlayedFromPowerTaskList() - source is null.");
      return (Entity) null;
    }
    return !sourceEntity.IsHero() ? (Entity) null : sourceEntity;
  }

  private bool ShowNewHeroStats()
  {
    Entity fromPowerTaskList = this.GetNewHeroPlayedFromPowerTaskList();
    if (fromPowerTaskList != null)
    {
      if (!fromPowerTaskList.GetCard().IsActorReady())
        return true;
      Actor actor = fromPowerTaskList.GetCard().GetActor();
      actor.EnableArmorSpellAfterTransition();
      actor.ShowArmorSpell();
      actor.GetHealthObject().Show();
      actor.GetAttackObject().Show();
      if (fromPowerTaskList.GetATK() <= 0)
        actor.GetAttackObject().ImmediatelyScaleToZero();
    }
    return false;
  }

  private bool ShouldWaitForOldHero(Entity entity)
  {
    if (!entity.IsHero())
      return false;
    Entity fromPowerTaskList = this.GetNewHeroPlayedFromPowerTaskList();
    return fromPowerTaskList != null && fromPowerTaskList.GetEntityId() != entity.GetEntityId() && !fromPowerTaskList.GetCard().IsActorReady();
  }

  private bool ShouldIgnoreZoneChange(Entity entity)
  {
    if ((UnityEngine.Object) entity.GetCard() == (UnityEngine.Object) null)
      return true;
    return !this.IsOldHero(entity) && this.m_ignoreCardZoneChanges;
  }

  private bool IsOldHero(Entity entity)
  {
    Entity fromPowerTaskList = this.GetNewHeroPlayedFromPowerTaskList();
    return fromPowerTaskList != null && entity.IsHero() && fromPowerTaskList.GetEntityId() != entity.GetEntityId();
  }

  private async UniTaskVoid FinishWhenPossible(CancellationToken token)
  {
    UniTask uniTask;
    while (this.m_dirtyZones.Count > 0)
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    while (GameState.Get().IsBusy())
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    this.Finish();
  }

  private void Finish()
  {
    this.m_complete = true;
    Log.Zone.Print("ZoneChangeList.Finish() - {0}", (object) this);
  }

  [Conditional("ZONE_CHANGE_DEBUG")]
  private void DebugPrint(string format, params object[] args) => Log.Zone.Print(format, args);
}
