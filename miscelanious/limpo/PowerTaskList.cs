using Cysharp.Threading.Tasks;
using PegasusGame;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PowerTaskList
{
  private int m_id;
  private Network.HistBlockStart m_blockStart;
  private Network.HistBlockEnd m_blockEnd;
  private List<PowerTask> m_tasks = new List<PowerTask>();
  private ZoneChangeList m_zoneChangeList;
  private int m_pendingTasks;
  private bool m_isBatchable;
  private bool m_isDeferrable;
  private int m_deferredSourceId;
  private PowerTaskList m_previous;
  private PowerTaskList m_next;
  private PowerTaskList m_subSpellOrigin;
  private Network.HistSubSpellStart m_subSpellStart;
  private Network.HistSubSpellEnd m_subSpellEnd;
  private PowerTaskList m_parent;
  private bool m_attackDataBuilt;
  private AttackInfo m_attackInfo;
  private AttackType m_attackType;
  private Entity m_attacker;
  private Entity m_defender;
  private Entity m_proposedDefender;
  private bool m_repeatProposed;
  private bool m_willCompleteHistoryEntry;
  private Entity m_ritualEntityClone;
  private Entity m_invokeEntityClone;
  private int? m_lastBattlecryEffectIndex;
  private float m_taskListStartTime;
  private float m_taskListEndTime;
  private int m_taskListSlushTimeMilliseconds = -1;
  private bool m_isHistoryBlockStart;
  private bool m_isHistoryBlockEnd;
  private bool m_collapsible;

  public int GetId() => this.m_id;

  public void SetId(int id) => this.m_id = id;

  public int GetDeferredSourceId() => this.m_deferredSourceId;

  public void SetDeferredSourceId(int id) => this.m_deferredSourceId = id;

  public void AddTasks(PowerTaskList otherTaskList)
  {
    this.m_tasks.AddRange((IEnumerable<PowerTask>) otherTaskList.m_tasks);
    this.m_pendingTasks += otherTaskList.m_pendingTasks;
  }

  public void SetProcessStartTime()
  {
    this.m_taskListStartTime = Time.realtimeSinceStartup;
    GameState.Get().GetPowerProcessor().HandleTimelineStartEvent(this.m_id, this.m_taskListStartTime, this.m_isHistoryBlockStart, this.GetBlockStart());
  }

  public void SetProcessEndTime()
  {
    this.m_taskListEndTime = Time.realtimeSinceStartup;
    GameState.Get().GetPowerProcessor().HandleTimelineEndEvent(this.m_id, this.m_taskListEndTime, this.m_isHistoryBlockEnd);
  }

  public void SetDeferrable(bool deferrable) => this.m_isDeferrable = deferrable;

  public bool IsDeferrable() => this.m_isDeferrable;

  public void SetBatchable(bool batchable) => this.m_isBatchable = batchable;

  public bool IsBatchable() => this.m_isBatchable && this.m_blockStart != null && this.m_blockEnd != null;

  public bool IsCollapsible(bool isEarlier)
  {
    if (isEarlier && !this.m_collapsible)
      return false;
    bool flag = false;
    if (this.m_tasks.Count > 0)
    {
      PowerTask task = this.m_tasks[this.m_tasks.Count - 1];
      if (task.GetPower() is Network.HistMetaData)
        flag = ((Network.HistMetaData) task.GetPower()).MetaType == HistoryMeta.Type.ARTIFICIAL_HISTORY_INTERRUPT;
    }
    return (this.m_subSpellStart == null || isEarlier) && !(this.m_subSpellEnd != null & isEarlier) && (!this.m_isHistoryBlockStart || isEarlier) && !(this.m_isHistoryBlockEnd & isEarlier) && !flag;
  }

  public void SetCollapsible(bool collapsible) => this.m_collapsible = collapsible;

  public bool IsSlushTimeHelper() => this.m_tasks.Count == 1 && this.m_tasks[0].GetPower() is Network.HistMetaData && ((Network.HistMetaData) this.m_tasks[0].GetPower()).MetaType == HistoryMeta.Type.SLUSH_TIME;

  public bool HasAnyTasksInImmediate() => this.m_tasks.Count > 0;

  public void SetHistoryBlockStart(bool isStart) => this.m_isHistoryBlockStart = isStart;

  public void SetHistoryBlockEnd(bool isEnd) => this.m_isHistoryBlockEnd = isEnd;

  public void OnTaskCompleted()
  {
    if (--this.m_pendingTasks != 0)
      return;
    this.OnTaskListCompleted();
  }

  public bool IsEmpty()
  {
    PowerTaskList origin = this.GetOrigin();
    return origin.m_blockStart == null && origin.m_blockEnd == null && origin.m_tasks.Count <= 0;
  }

  public bool IsOrigin() => this.m_previous == null;

  public void FillMetaDataTargetSourceData()
  {
    foreach (PowerTask task in this.m_tasks)
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.TARGET && histMetaData.Data == 0)
        {
          Entity sourceEntity = this.GetSourceEntity();
          histMetaData.Data = sourceEntity == null ? 0 : sourceEntity.GetEntityId();
        }
      }
    }
  }

  public PowerTaskList GetOrigin()
  {
    PowerTaskList origin = this;
    while (origin.m_previous != null)
      origin = origin.m_previous;
    return origin;
  }

  public PowerTaskList GetPrevious() => this.m_previous;

  public void SetPrevious(PowerTaskList taskList)
  {
    this.m_previous = taskList;
    taskList.m_next = this;
  }

  public PowerTaskList GetNext() => this.m_next;

  public void SetNext(PowerTaskList next) => this.m_next = next;

  public PowerTaskList GetLast()
  {
    PowerTaskList last = this;
    while (last.m_next != null)
      last = last.m_next;
    return last;
  }

  public Network.HistBlockStart GetBlockStart() => this.GetOrigin().m_blockStart;

  public void SetBlockStart(Network.HistBlockStart blockStart) => this.m_blockStart = blockStart;

  public Network.HistBlockEnd GetBlockEnd() => this.m_blockEnd;

  public void SetBlockEnd(Network.HistBlockEnd blockEnd) => this.m_blockEnd = blockEnd;

  public PowerTaskList GetParent() => this.GetOrigin().m_parent;

  public void SetParent(PowerTaskList parent) => this.m_parent = parent;

  public PowerTaskList GetParentWithBlockType(HistoryBlock.Type type)
  {
    for (PowerTaskList parent = this.GetParent(); parent != null; parent = parent.GetParent())
    {
      if (parent.IsBlockType(type))
        return parent;
    }
    return (PowerTaskList) null;
  }

  public Network.HistSubSpellStart GetSubSpellStart() => this.m_subSpellStart;

  public void SetSubSpellStart(Network.HistSubSpellStart subSpellStart) => this.m_subSpellStart = subSpellStart;

  public Network.HistSubSpellEnd GetSubSpellEnd() => this.m_subSpellEnd;

  public void SetSubSpellEnd(Network.HistSubSpellEnd subSpellEnd) => this.m_subSpellEnd = subSpellEnd;

  public PowerTaskList GetSubSpellOrigin() => this.m_subSpellOrigin;

  public void SetSubSpellOrigin(PowerTaskList taskList) => this.m_subSpellOrigin = taskList;

  public bool IsBlock() => this.GetOrigin().m_blockStart != null;

  public bool IsStartOfBlock() => this.IsBlock() && this.m_blockStart != null;

  public bool IsEndOfBlock() => this.IsBlock() && this.m_blockEnd != null;

  public bool IsEarlierInBlockThan(PowerTaskList taskList)
  {
    if (taskList == null)
      return false;
    for (PowerTaskList previous = taskList.m_previous; previous != null; previous = previous.m_previous)
    {
      if (this == previous)
        return true;
    }
    return false;
  }

  public bool IsLaterInBlockThan(PowerTaskList taskList)
  {
    if (taskList == null)
      return false;
    for (PowerTaskList next = taskList.m_next; next != null; next = next.m_next)
    {
      if (this == next)
        return true;
    }
    return false;
  }

  public bool IsInBlock(PowerTaskList taskList) => this == taskList || this.IsEarlierInBlockThan(taskList) || this.IsLaterInBlockThan(taskList);

  public bool IsDescendantOfBlock(PowerTaskList taskList)
  {
    if (taskList == null)
      return false;
    if (this.IsInBlock(taskList))
      return true;
    PowerTaskList origin = taskList.GetOrigin();
    for (PowerTaskList parent = this.GetParent(); parent != null; parent = parent.m_parent)
    {
      if (parent == origin)
        return true;
    }
    return false;
  }

  public List<PowerTask> GetTaskList() => this.m_tasks;

  public bool HasTasks() => this.m_tasks.Count > 0;

  public PowerTask CreateTask(Network.PowerHistory netPower)
  {
    ++this.m_pendingTasks;
    PowerTask task = new PowerTask();
    task.SetPower(netPower);
    task.SetTaskCompleteCallback(new PowerTask.TaskCompleteCallback(this.OnTaskCompleted));
    this.m_tasks.Add(task);
    return task;
  }

  public Entity GetSourceEntity(bool warnIfNull = true)
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return (Entity) null;
    using (List<int>.Enumerator enumerator = blockStart.Entities.GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        int current = enumerator.Current;
        Entity entity = GameState.Get().GetEntity(current);
        if (!(entity == null & warnIfNull) || GameState.Get().EntityRemovedFromGame(current))
          return entity;
        Log.Power.PrintWarning(string.Format("PowerProcessor.GetSourceEntity() - task list {0} has a source entity with id {1} but there is no entity with that id", (object) this.m_id, (object) current));
        return (Entity) null;
      }
    }
    return (Entity) null;
  }

  public List<Entity> GetSourceEntities(bool warnIfNull = true)
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return (List<Entity>) null;
    List<int> entities = blockStart.Entities;
    List<Entity> sourceEntities = new List<Entity>();
    foreach (int num in entities)
    {
      Entity entity = GameState.Get().GetEntity(num);
      if (entity == null & warnIfNull && !GameState.Get().EntityRemovedFromGame(num))
      {
        Log.Power.PrintWarning(string.Format("PowerProcessor.GetSourceEntity() - task list {0} has a source entity with id {1} but there is no entity with that id", (object) this.m_id, (object) num));
        return (List<Entity>) null;
      }
      sourceEntities.Add(entity);
    }
    return sourceEntities;
  }

  public bool IsEffectCardIdClientCached(int entityId)
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return false;
    int index = 0;
    foreach (int entity in blockStart.Entities)
    {
      if (entity != entityId)
        ++index;
      else
        break;
    }
    return index < blockStart.IsEffectCardIdClientCached.Count && blockStart.IsEffectCardIdClientCached[index];
  }

  public string GetEffectCardId(int entityId)
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return (string) null;
    int index = 0;
    foreach (int entity in blockStart.Entities)
    {
      if (entity != entityId)
        ++index;
      else
        break;
    }
    if (index >= blockStart.EffectCardId.Count)
      return (string) null;
    string effectCardId = blockStart.EffectCardId[index];
    if (!string.IsNullOrEmpty(effectCardId))
      return effectCardId;
    return this.GetSourceEntity()?.GetCardId();
  }

  public EntityDef GetEffectEntityDef(int entityId)
  {
    string effectCardId = this.GetEffectCardId(entityId);
    return string.IsNullOrEmpty(effectCardId) ? (EntityDef) null : DefLoader.Get().GetEntityDef(effectCardId);
  }

  public string GetEffectCardId()
  {
    Entity sourceEntity = this.GetSourceEntity();
    return sourceEntity == null ? (string) null : this.GetEffectCardId(sourceEntity.GetEntityId());
  }

  public EntityDef GetEffectEntityDef()
  {
    Entity sourceEntity = this.GetSourceEntity();
    return sourceEntity == null ? (EntityDef) null : this.GetEffectEntityDef(sourceEntity.GetEntityId());
  }

  public Entity GetTargetEntity(bool warnIfNull = true)
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return (Entity) null;
    int target = blockStart.Target;
    Entity entity = GameState.Get().GetEntity(target);
    if (!(entity == null & warnIfNull) || GameState.Get().EntityRemovedFromGame(target))
      return entity;
    Log.Power.PrintWarning(string.Format("PowerProcessor.GetTargetEntity() - task list {0} has a target entity with id {1} but there is no entity with that id", (object) this.m_id, (object) target));
    return (Entity) null;
  }

  public bool HasTargetEntity()
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return false;
    int target = blockStart.Target;
    return GameState.Get().GetEntity(target) != null;
  }

  public bool HasMetaDataTasks()
  {
    foreach (PowerTask task in this.m_tasks)
    {
      if (task.GetPower().Type == Network.PowerType.META_DATA)
        return true;
    }
    return false;
  }

  public bool DoesBlockHaveMetaDataTasks()
  {
    for (PowerTaskList powerTaskList = this.GetOrigin(); powerTaskList != null; powerTaskList = powerTaskList.m_next)
    {
      if (powerTaskList.HasMetaDataTasks())
        return true;
    }
    return false;
  }

  public bool HasCardDraw()
  {
    foreach (PowerTask task in this.m_tasks)
    {
      if (task.IsCardDraw())
        return true;
    }
    return false;
  }

  public bool HasCardMill()
  {
    foreach (PowerTask task in this.m_tasks)
    {
      if (task.IsCardMill())
        return true;
    }
    return false;
  }

  public bool HasFatigue()
  {
    foreach (PowerTask task in this.m_tasks)
    {
      if (task.IsFatigue())
        return true;
    }
    return false;
  }

  public int GetTotalSlushTime()
  {
    if (this.m_taskListSlushTimeMilliseconds > -1)
      return this.m_taskListSlushTimeMilliseconds;
    int totalSlushTime = 0;
    foreach (PowerTask task in this.m_tasks)
    {
      if (task.GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.SLUSH_TIME)
        totalSlushTime += power.Data;
    }
    this.m_taskListSlushTimeMilliseconds = totalSlushTime;
    return totalSlushTime;
  }

  public bool HasEffectTimingMetaData()
  {
    foreach (PowerTask task in this.m_tasks)
    {
      if (task.GetPower() is Network.HistMetaData power && (power.MetaType == HistoryMeta.Type.TARGET || power.MetaType == HistoryMeta.Type.EFFECT_TIMING))
        return true;
    }
    return false;
  }

  public List<PowerTask> GetTagChangeTasks()
  {
    List<PowerTask> tagChangeTasks = new List<PowerTask>();
    foreach (PowerTask task in this.m_tasks)
    {
      if (task.GetPower() is Network.HistTagChange)
        tagChangeTasks.Add(task);
    }
    return tagChangeTasks;
  }

  public bool DoesBlockHaveEffectTimingMetaData()
  {
    for (PowerTaskList powerTaskList = this.GetOrigin(); powerTaskList != null; powerTaskList = powerTaskList.m_next)
    {
      if (powerTaskList.GetSubSpellOrigin() == this.GetSubSpellOrigin() && powerTaskList.HasEffectTimingMetaData())
        return true;
    }
    return false;
  }

  public HistoryBlock.Type GetBlockType()
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    return blockStart == null ? HistoryBlock.Type.INVALID : blockStart.BlockType;
  }

  public bool IsBlockType(HistoryBlock.Type type)
  {
    Network.HistBlockStart blockStart = this.GetBlockStart();
    return blockStart != null && blockStart.BlockType == type;
  }

  public bool IsPlayBlock() => this.IsBlockType(HistoryBlock.Type.PLAY);

  public bool IsTriggerBlock() => this.IsBlockType(HistoryBlock.Type.TRIGGER);

  public bool IsDeathBlock() => this.IsBlockType(HistoryBlock.Type.DEATHS);

  public bool IsRitualBlock() => this.IsBlockType(HistoryBlock.Type.RITUAL);

  public bool IsSubSpellTaskList() => this.m_subSpellOrigin != null;

  public void DoTasks(int startIndex, int count) => this.DoTasks(startIndex, count, (PowerTaskList.CompleteCallback) null, (object) null);

  public void DoTasks(int startIndex, int count, PowerTaskList.CompleteCallback callback) => this.DoTasks(startIndex, count, callback, (object) null);

  public void DoTasks(
    int startIndex,
    int count,
    PowerTaskList.CompleteCallback callback,
    object userData)
  {
    bool flag = false;
    int num1 = -1;
    int num2 = Mathf.Min(startIndex + count - 1, this.m_tasks.Count - 1);
    for (int index = startIndex; index <= num2; ++index)
    {
      PowerTask task = this.m_tasks[index];
      if (!task.IsCompleted())
      {
        if (num1 < 0)
          num1 = index;
        if (ZoneMgr.IsHandledPower(task.GetPower()))
        {
          flag = true;
          break;
        }
      }
    }
    if (num1 < 0)
      num1 = startIndex;
    if (flag)
    {
      this.m_zoneChangeList = ZoneMgr.Get().AddServerZoneChanges(this, num1, num2, new ZoneMgr.ChangeCompleteCallback(this.OnZoneChangeComplete), (object) new PowerTaskList.ZoneChangeCallbackData()
      {
        m_startIndex = startIndex,
        m_count = count,
        m_taskListCallback = callback,
        m_taskListUserData = userData
      });
      if (this.m_zoneChangeList != null)
        return;
    }
    if ((Object) Gameplay.Get() != (Object) null)
      this.WaitForGameStateAndDoTasks(num1, num2, startIndex, count, callback, userData, Gameplay.Get().TaskToken).Forget();
    else
      this.DoTasks(num1, num2, startIndex, count, callback, userData);
  }

  public void DoAllTasks(PowerTaskList.CompleteCallback callback) => this.DoTasks(0, this.m_tasks.Count, callback, (object) null);

  public void DoAllTasks() => this.DoTasks(0, this.m_tasks.Count, (PowerTaskList.CompleteCallback) null, (object) null);

  public void DoEarlyConcedeTasks()
  {
    for (int index = 0; index < this.m_tasks.Count; ++index)
      this.m_tasks[index].DoEarlyConcedeTask();
  }

  public bool IsComplete() => this.AreTasksComplete() && this.AreZoneChangesComplete();

  public bool AreTasksComplete()
  {
    foreach (PowerTask task in this.m_tasks)
    {
      if (!task.IsCompleted())
        return false;
    }
    return true;
  }

  public Card GetStartDrawMetaDataCard()
  {
    for (int index = 0; index < this.m_tasks.Count; ++index)
    {
      Network.PowerHistory power = this.m_tasks[index].GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.START_DRAW)
        {
          Entity entity = GameState.Get().GetEntity(histMetaData.Info[0]);
          if (entity != null)
            return entity.GetCard();
        }
      }
    }
    return (Card) null;
  }

  public int FindEarlierIncompleteTaskIndex(int taskIndex)
  {
    for (int index = taskIndex - 1; index >= 0; --index)
    {
      if (!this.m_tasks[index].IsCompleted())
        return index;
    }
    return -1;
  }

  public bool HasEarlierIncompleteTask(int taskIndex) => this.FindEarlierIncompleteTaskIndex(taskIndex) >= 0;

  public bool HasZoneChanges() => this.m_zoneChangeList != null;

  public bool AreZoneChangesComplete() => this.m_zoneChangeList == null || this.m_zoneChangeList.IsComplete();

  public AttackType GetAttackType()
  {
    this.BuildAttackData();
    return this.m_attackType;
  }

  public Entity GetAttacker()
  {
    this.BuildAttackData();
    return this.m_attacker;
  }

  public Entity GetDefender()
  {
    this.BuildAttackData();
    return this.m_defender;
  }

  public Entity GetProposedDefender()
  {
    this.BuildAttackData();
    return this.m_proposedDefender;
  }

  public bool IsRepeatProposedAttack()
  {
    this.BuildAttackData();
    return this.m_repeatProposed;
  }

  public bool HasGameOver()
  {
    for (int index = 0; index < this.m_tasks.Count; ++index)
    {
      Network.PowerHistory power = this.m_tasks[index].GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (GameUtils.IsGameOverTag(histTagChange.Entity, histTagChange.Tag, histTagChange.Value))
          return true;
      }
    }
    return false;
  }

  public bool HasFriendlyConcede()
  {
    for (int index = 0; index < this.m_tasks.Count; ++index)
    {
      Network.PowerHistory power = this.m_tasks[index].GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE && GameUtils.IsFriendlyConcede((Network.HistTagChange) power))
        return true;
    }
    return false;
  }

  public PowerTaskList.DamageInfo GetDamageInfo(Entity entity)
  {
    if (entity == null)
      return (PowerTaskList.DamageInfo) null;
    int entityId = entity.GetEntityId();
    foreach (PowerTask task in this.m_tasks)
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = power as Network.HistTagChange;
        if (histTagChange.Tag == 44 && histTagChange.Entity == entityId)
        {
          PowerTaskList.DamageInfo damageInfo = new PowerTaskList.DamageInfo()
          {
            m_entity = GameState.Get().GetEntity(histTagChange.Entity)
          };
          damageInfo.m_damage = histTagChange.Value - damageInfo.m_entity.GetDamage();
          return damageInfo;
        }
      }
    }
    return (PowerTaskList.DamageInfo) null;
  }

  public void SetWillCompleteHistoryEntry(bool set) => this.m_willCompleteHistoryEntry = set;

  public bool WillCompleteHistoryEntry() => this.m_willCompleteHistoryEntry;

  public bool WillBlockCompleteHistoryEntry()
  {
    for (PowerTaskList powerTaskList = this.GetOrigin(); powerTaskList != null; powerTaskList = powerTaskList.m_next)
    {
      if (powerTaskList.WillCompleteHistoryEntry())
        return true;
    }
    return false;
  }

  public Entity GetRitualEntityClone() => this.m_ritualEntityClone;

  public void SetRitualEntityClone(Entity ent) => this.m_ritualEntityClone = ent;

  public bool WasThePlayedSpellCountered(Entity entity)
  {
    foreach (PowerTask task in this.m_tasks)
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = power as Network.HistTagChange;
        if (histTagChange.Entity == entity.GetEntityId() && histTagChange.Tag == 231 && histTagChange.Value == 1)
          return true;
      }
    }
    foreach (PowerTaskList powerTaskList in GameState.Get().GetPowerProcessor().GetPowerQueue().GetList())
    {
      foreach (PowerTask task in powerTaskList.GetTaskList())
      {
        Network.PowerHistory power = task.GetPower();
        if (power.Type == Network.PowerType.TAG_CHANGE)
        {
          Network.HistTagChange histTagChange = power as Network.HistTagChange;
          if (histTagChange.Entity == entity.GetEntityId() && histTagChange.Tag == 231 && histTagChange.Value == 1)
            return true;
        }
      }
      if (powerTaskList.GetBlockEnd() != null && powerTaskList.GetBlockStart().BlockType == HistoryBlock.Type.PLAY)
        return false;
    }
    return false;
  }

  public void CreateArtificialHistoryTilesFromMetadata()
  {
    List<PowerTask> tasksToInclude = new List<PowerTask>();
    bool flag = false;
    foreach (PowerTask task in this.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.BEGIN_ARTIFICIAL_HISTORY_TILE || histMetaData.MetaType == HistoryMeta.Type.BEGIN_ARTIFICIAL_HISTORY_TRIGGER_TILE)
        {
          int id = histMetaData.Info[0];
          Entity entity = GameState.Get().GetEntity(id);
          if (entity != null)
          {
            if (flag)
            {
              this.NotifyHistoryOfAdditionalTargets(tasksToInclude);
              HistoryManager.Get().MarkCurrentHistoryEntryAsCompleted();
              tasksToInclude.Clear();
            }
            else
              flag = true;
            if (histMetaData.MetaType == HistoryMeta.Type.BEGIN_ARTIFICIAL_HISTORY_TRIGGER_TILE)
              HistoryManager.Get().CreateTriggerTile(entity);
            else
              HistoryManager.Get().CreatePlayedTile(entity, (Entity) null);
          }
        }
        else if (flag && histMetaData.MetaType == HistoryMeta.Type.END_ARTIFICIAL_HISTORY_TILE)
        {
          flag = false;
          this.NotifyHistoryOfAdditionalTargets(tasksToInclude);
          HistoryManager.Get().MarkCurrentHistoryEntryAsCompleted();
          tasksToInclude.Clear();
        }
        else if (flag)
          tasksToInclude.Add(task);
      }
      else if (flag)
        tasksToInclude.Add(task);
    }
    if (!flag)
      return;
    this.NotifyHistoryOfAdditionalTargets(tasksToInclude);
    HistoryManager.Get().MarkCurrentHistoryEntryAsCompleted();
  }

  public void NotifyHistoryOfAdditionalTargets(List<PowerTask> tasksToInclude = null)
  {
    if (tasksToInclude == null)
      tasksToInclude = this.GetTaskList();
    bool flag1 = false;
    Network.HistBlockStart blockStart = this.GetBlockStart();
    List<int> intList1 = blockStart == null ? (List<int>) null : blockStart.Entities;
    List<int> intList2 = new List<int>();
    List<int> intList3 = new List<int>();
    bool flag2 = true;
    foreach (PowerTask powerTask in tasksToInclude)
    {
      Network.PowerHistory power = powerTask.GetPower();
      if (flag1)
      {
        if (power.Type == Network.PowerType.META_DATA && ((Network.HistMetaData) power).MetaType == HistoryMeta.Type.END_ARTIFICIAL_HISTORY_TILE)
          flag1 = false;
      }
      else if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.TARGET)
        {
          for (int index = 0; index < histMetaData.Info.Count; ++index)
            HistoryManager.Get().NotifyEntityAffected(histMetaData.Info[index], false, false);
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.DAMAGE || histMetaData.MetaType == HistoryMeta.Type.HEALING)
          flag2 = false;
        else if (histMetaData.MetaType == HistoryMeta.Type.OVERRIDE_HISTORY)
          HistoryManager.Get().OverrideCurrentHistoryEntryWithMetaData();
        else if (histMetaData.MetaType == HistoryMeta.Type.HISTORY_TARGET)
        {
          for (int index = 0; index < histMetaData.Info.Count; ++index)
          {
            int id = histMetaData.Info[index];
            Entity entity = GameState.Get().GetEntity(id);
            if (entity != null)
              HistoryManager.Get().NotifyEntityAffected(entity, false, true);
          }
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.HISTORY_TRIGGER_SOURCE)
        {
          if (histMetaData.Info.Count > 0)
          {
            Entity entity = GameState.Get().GetEntity(histMetaData.Info[0]);
            HistoryManager.Get().OverrideCurrentHistoryTriggerSource(entity);
          }
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.HISTORY_SOURCE_OWNER)
        {
          if (histMetaData.Info.Count > 0)
          {
            Entity entity = GameState.Get().GetEntity(histMetaData.Info[0]);
            HistoryManager.Get().OverrideCurrentHistorySourceOwner(entity);
          }
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.BURNED_CARD)
        {
          for (int index = 0; index < histMetaData.Info.Count; ++index)
          {
            int id = histMetaData.Info[index];
            Entity entity = GameState.Get().GetEntity(id);
            if (entity != null)
              HistoryManager.Get().NotifyEntityAffected(entity, false, true, isBurnedCard: true);
          }
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.POISONOUS)
        {
          for (int index = 0; index < histMetaData.Info.Count; ++index)
          {
            int id = histMetaData.Info[index];
            Entity entity = GameState.Get().GetEntity(id);
            if (entity != null)
              HistoryManager.Get().NotifyEntityAffected(entity, false, true, isPoisonous: true);
          }
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.CRITICAL_HIT)
        {
          for (int index = 0; index < histMetaData.Info.Count; ++index)
          {
            int id = histMetaData.Info[index];
            Entity entity = GameState.Get().GetEntity(id);
            if (entity != null)
              HistoryManager.Get().NotifyEntityAffected(entity, false, true, isCriticalHit: true);
          }
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.HISTORY_TARGET_DONT_DUPLICATE_UNTIL_END)
        {
          for (int index = 0; index < histMetaData.Info.Count; ++index)
          {
            int id = histMetaData.Info[index];
            Entity entity = GameState.Get().GetEntity(id);
            if (entity != null)
              HistoryManager.Get().NotifyEntityAffected(entity, true, true, true);
          }
        }
        else if (histMetaData.MetaType == HistoryMeta.Type.BEGIN_ARTIFICIAL_HISTORY_TILE || histMetaData.MetaType == HistoryMeta.Type.BEGIN_ARTIFICIAL_HISTORY_TRIGGER_TILE)
          flag1 = true;
      }
      else if (power.Type == Network.PowerType.SHOW_ENTITY)
      {
        Network.HistShowEntity histShowEntity = (Network.HistShowEntity) power;
        bool flag3 = false;
        bool flag4 = false;
        bool flag5 = false;
        Entity entity = GameState.Get().GetEntity(histShowEntity.Entity.ID);
        bool flag6 = entity.GetZone() == TAG_ZONE.HAND;
        bool flag7 = entity.GetZone() == TAG_ZONE.SETASIDE;
        foreach (Network.Entity.Tag tag in histShowEntity.Entity.Tags)
        {
          if (tag.Name == 202 && tag.Value == 6)
          {
            flag3 = true;
            break;
          }
          if (tag.Name == 49)
          {
            if (tag.Value == 4)
              flag4 = true;
            else if (tag.Value == 6)
              flag5 = true;
          }
        }
        if (!flag3 && !(flag4 & flag7) && !(flag5 & flag7))
        {
          if (flag4 && !flag6)
            HistoryManager.Get().NotifyEntityDied(histShowEntity.Entity.ID);
          else
            HistoryManager.Get().NotifyEntityAffected(histShowEntity.Entity.ID, false, false);
        }
      }
      else if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        Network.HistFullEntity histFullEntity = (Network.HistFullEntity) power;
        bool flag8 = false;
        bool flag9 = false;
        bool flag10 = false;
        foreach (Network.Entity.Tag tag in histFullEntity.Entity.Tags)
        {
          GAME_TAG name = (GAME_TAG) tag.Name;
          if (name == GAME_TAG.DONT_SHOW_IN_HISTORY && tag.Value != 0)
          {
            flag8 = true;
            break;
          }
          if (name == GAME_TAG.CARDTYPE && tag.Value == 6)
          {
            flag8 = true;
            break;
          }
          if (name == GAME_TAG.ZONE && (tag.Value == 1 || tag.Value == 7))
            flag9 = true;
          else if (name == GAME_TAG.DISPLAYED_CREATOR && intList1 != null && intList1.Contains(tag.Value))
            flag10 = true;
        }
        if (!flag8 && (flag9 || flag10))
          HistoryManager.Get().NotifyEntityAffected(histFullEntity.Entity.ID, false, false);
      }
      else if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange tagChange = (Network.HistTagChange) power;
        if (!tagChange.ChangeDef)
        {
          Entity entity = GameState.Get().GetEntity(tagChange.Entity);
          if (tagChange.Tag == 44)
          {
            if (!intList2.Contains(tagChange.Entity) && !flag2)
            {
              HistoryManager.Get().NotifyDamageChanged(entity, tagChange.Value);
              flag2 = true;
            }
          }
          else if (tagChange.Tag == 292)
          {
            if (!intList2.Contains(tagChange.Entity) && !intList3.Contains(tagChange.Entity))
              HistoryManager.Get().NotifyArmorChanged(entity, tagChange.Value);
          }
          else if (tagChange.Tag == 45)
          {
            if (!intList2.Contains(tagChange.Entity))
              HistoryManager.Get().NotifyHealthChanged(entity, tagChange.Value);
          }
          else if (tagChange.Tag == 318)
            HistoryManager.Get().NotifyEntityAffected(entity, false, false);
          else if (tagChange.Tag == 385 && intList1 != null && intList1.Contains(tagChange.Value))
            HistoryManager.Get().NotifyEntityAffected(entity, false, false);
          else if (tagChange.Tag == 262)
            HistoryManager.Get().NotifyEntityAffected(entity, false, false);
          if (GameUtils.IsHistoryDeathTagChange(tagChange))
          {
            HistoryManager.Get().NotifyEntityDied(entity);
            intList2.Add(tagChange.Entity);
          }
          if (GameUtils.IsHistoryMovedToSetAsideTagChange(tagChange))
            intList3.Add(tagChange.Entity);
          if (GameUtils.IsHistoryDiscardTagChange(tagChange))
            HistoryManager.Get().NotifyEntityAffected(entity, false, false);
        }
      }
    }
  }

  public bool ShouldCreatePlayBlockHistoryTile()
  {
    if ((Object) HistoryManager.Get() == (Object) null || !HistoryManager.Get().IsHistoryEnabled() || !this.IsPlayBlock())
      return false;
    PowerTaskList parent = this.GetParent();
    if (parent == null)
      return true;
    Entity sourceEntity = parent.GetSourceEntity();
    return sourceEntity == null || !sourceEntity.HasTag(GAME_TAG.CAST_RANDOM_SPELLS);
  }

  public void SetActivateBattlecrySpellState()
  {
    PowerTaskList parentWithBlockType = this.GetParentWithBlockType(HistoryBlock.Type.PLAY);
    if (parentWithBlockType == null)
      return;
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return;
    parentWithBlockType.m_lastBattlecryEffectIndex = new int?(blockStart.EffectIndex);
  }

  public bool ShouldActivateBattlecrySpell()
  {
    if (!this.IsOrigin())
      return false;
    PowerTaskList parentWithBlockType = this.GetParentWithBlockType(HistoryBlock.Type.PLAY);
    if (parentWithBlockType == null)
      return false;
    Network.HistBlockStart blockStart = this.GetBlockStart();
    if (blockStart == null)
      return false;
    if (parentWithBlockType.m_lastBattlecryEffectIndex.HasValue)
    {
      int? battlecryEffectIndex = parentWithBlockType.m_lastBattlecryEffectIndex;
      int effectIndex = blockStart.EffectIndex;
      if (!(battlecryEffectIndex.GetValueOrDefault() == effectIndex & battlecryEffectIndex.HasValue))
        return false;
    }
    return true;
  }

  public void DebugDump() => this.DebugDump(Log.Power);

  public void DebugDump(Logger logger)
  {
    if (!logger.CanPrint())
      return;
    GameState gameState = GameState.Get();
    string indentation = string.Empty;
    int num1 = this.m_parent == null ? 0 : this.m_parent.GetId();
    int num2 = this.m_previous == null ? 0 : this.m_previous.GetId();
    logger.Print("PowerTaskList.DebugDump() - ID={0} ParentID={1} PreviousID={2} TaskCount={3}", (object) this.m_id, (object) num1, (object) num2, (object) this.m_tasks.Count);
    if (this.m_blockStart == null)
    {
      logger.Print("PowerTaskList.DebugDump() - {0}Block Start=(null)", (object) indentation);
      indentation += "    ";
    }
    else
      gameState.DebugPrintPower(logger, nameof (PowerTaskList), (Network.PowerHistory) this.m_blockStart, ref indentation);
    for (int index = 0; index < this.m_tasks.Count; ++index)
    {
      Network.PowerHistory power = this.m_tasks[index].GetPower();
      gameState.DebugPrintPower(logger, nameof (PowerTaskList), power, ref indentation);
    }
    if (this.m_blockEnd == null)
    {
      if (indentation.Length >= "    ".Length)
        indentation = indentation.Remove(indentation.Length - "    ".Length);
      logger.Print("PowerTaskList.DebugDump() - {0}Block End=(null)", (object) indentation);
    }
    else
      gameState.DebugPrintPower(logger, nameof (PowerTaskList), (Network.PowerHistory) this.m_blockEnd, ref indentation);
  }

  public override string ToString() => string.Format("id={0} tasks={1} prevId={2} nextId={3} parentId={4}", (object) this.m_id, (object) this.m_tasks.Count, (object) (this.m_previous == null ? 0 : this.m_previous.GetId()), (object) (this.m_next == null ? 0 : this.m_next.GetId()), (object) (this.m_parent == null ? 0 : this.m_parent.GetId()));

  private void OnZoneChangeComplete(ZoneChangeList changeList, object userData)
  {
    PowerTaskList.ZoneChangeCallbackData changeCallbackData = (PowerTaskList.ZoneChangeCallbackData) userData;
    if (changeCallbackData.m_taskListCallback == null)
      return;
    changeCallbackData.m_taskListCallback(this, changeCallbackData.m_startIndex, changeCallbackData.m_count, changeCallbackData.m_taskListUserData);
  }

  private void OnTaskListCompleted() => this.SetProcessEndTime();

  private async UniTaskVoid WaitForGameStateAndDoTasks(
    int incompleteStartIndex,
    int endIndex,
    int startIndex,
    int count,
    PowerTaskList.CompleteCallback callback,
    object userData,
    CancellationToken token)
  {
    PowerTaskList taskList = this;
    for (int i = incompleteStartIndex; i <= endIndex; ++i)
    {
      PowerTask task = taskList.m_tasks[i];
      UniTask uniTask;
      while (!GameState.Get().GetPowerProcessor().CanDoTask(task))
      {
        uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
        await uniTask;
      }
      task.DoTask();
      while (GameState.Get().IsMulliganBusy())
      {
        uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
        await uniTask;
      }
      task = (PowerTask) null;
    }
    if (callback == null)
      return;
    callback(taskList, startIndex, count, userData);
  }

  private void DoTasks(
    int incompleteStartIndex,
    int endIndex,
    int startIndex,
    int count,
    PowerTaskList.CompleteCallback callback,
    object userData)
  {
    for (int index = incompleteStartIndex; index <= endIndex; ++index)
      this.m_tasks[index].DoTask();
    if (callback == null)
      return;
    callback(this, startIndex, count, userData);
  }

  private void BuildAttackData()
  {
    if (this.m_attackDataBuilt)
      return;
    this.m_attackInfo = this.BuildAttackInfo();
    AttackInfo info;
    this.m_attackType = this.DetermineAttackType(out info);
    this.m_attacker = (Entity) null;
    this.m_defender = (Entity) null;
    this.m_proposedDefender = (Entity) null;
    switch (this.m_attackType)
    {
      case AttackType.REGULAR:
        this.m_attacker = info.m_attacker;
        this.m_defender = info.m_defender;
        break;
      case AttackType.PROPOSED:
        this.m_attacker = info.m_proposedAttacker;
        this.m_defender = info.m_proposedDefender;
        this.m_proposedDefender = info.m_proposedDefender;
        this.m_repeatProposed = info.m_repeatProposed;
        break;
      case AttackType.CANCELED:
        this.m_attacker = this.m_previous.GetAttacker();
        this.m_proposedDefender = this.m_previous.GetProposedDefender();
        break;
      case AttackType.ONLY_ATTACKER:
        this.m_attacker = info.m_attacker;
        break;
      case AttackType.ONLY_DEFENDER:
        this.m_defender = info.m_defender;
        break;
      case AttackType.ONLY_PROPOSED_ATTACKER:
        this.m_attacker = info.m_proposedAttacker;
        break;
      case AttackType.ONLY_PROPOSED_DEFENDER:
        this.m_proposedDefender = info.m_proposedDefender;
        this.m_defender = info.m_proposedDefender;
        break;
      case AttackType.WAITING_ON_PROPOSED_ATTACKER:
      case AttackType.WAITING_ON_PROPOSED_DEFENDER:
      case AttackType.WAITING_ON_ATTACKER:
      case AttackType.WAITING_ON_DEFENDER:
        this.m_attacker = this.m_previous.GetAttacker();
        this.m_defender = this.m_previous.GetDefender();
        break;
    }
    this.m_attackDataBuilt = true;
  }

  private AttackInfo BuildAttackInfo()
  {
    GameState gameState = GameState.Get();
    AttackInfo attackInfo = new AttackInfo();
    bool flag = false;
    foreach (PowerTask task in this.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = power as Network.HistTagChange;
        if (histTagChange.Tag == 36)
        {
          attackInfo.m_defenderTagValue = new int?(histTagChange.Value);
          if (histTagChange.Value == 1)
            attackInfo.m_defender = gameState.GetEntity(histTagChange.Entity);
          flag = true;
        }
        else if (histTagChange.Tag == 38)
        {
          attackInfo.m_attackerTagValue = new int?(histTagChange.Value);
          if (histTagChange.Value == 1)
            attackInfo.m_attacker = gameState.GetEntity(histTagChange.Entity);
          flag = true;
        }
        else if (histTagChange.Tag == 39)
        {
          attackInfo.m_proposedAttackerTagValue = new int?(histTagChange.Value);
          if (histTagChange.Value != 0)
            attackInfo.m_proposedAttacker = gameState.GetEntity(histTagChange.Value);
          flag = true;
        }
        else if (histTagChange.Tag == 37)
        {
          attackInfo.m_proposedDefenderTagValue = new int?(histTagChange.Value);
          if (histTagChange.Value != 0)
            attackInfo.m_proposedDefender = gameState.GetEntity(histTagChange.Value);
          flag = true;
        }
      }
    }
    return flag ? attackInfo : (AttackInfo) null;
  }

  private AttackType DetermineAttackType(out AttackInfo info)
  {
    info = this.m_attackInfo;
    GameState gameState = GameState.Get();
    GameEntity gameEntity = gameState.GetGameEntity();
    Entity entity1 = gameState.GetEntity(gameEntity.GetTag(GAME_TAG.PROPOSED_ATTACKER));
    Entity entity2 = gameState.GetEntity(gameEntity.GetTag(GAME_TAG.PROPOSED_DEFENDER));
    AttackType attackType = AttackType.INVALID;
    Entity entity3 = (Entity) null;
    Entity entity4 = (Entity) null;
    if (this.m_previous != null)
    {
      attackType = this.m_previous.GetAttackType();
      entity3 = this.m_previous.GetAttacker();
      entity4 = this.m_previous.GetDefender();
    }
    if (this.m_attackInfo != null)
    {
      if (this.m_attackInfo.m_attacker != null || this.m_attackInfo.m_defender != null)
      {
        if (this.m_attackInfo.m_attacker == null)
        {
          if (attackType != AttackType.ONLY_ATTACKER && attackType != AttackType.WAITING_ON_DEFENDER)
            return AttackType.ONLY_DEFENDER;
          info = new AttackInfo();
          info.m_attacker = entity3;
          info.m_defender = this.m_attackInfo.m_defender;
          return AttackType.REGULAR;
        }
        if (this.m_attackInfo.m_defender != null)
          return AttackType.REGULAR;
        if (attackType != AttackType.ONLY_DEFENDER && attackType != AttackType.WAITING_ON_ATTACKER)
          return AttackType.ONLY_ATTACKER;
        info = new AttackInfo();
        info.m_attacker = this.m_attackInfo.m_attacker;
        info.m_defender = entity4;
        return AttackType.REGULAR;
      }
      if (this.m_attackInfo.m_proposedAttacker != null || this.m_attackInfo.m_proposedDefender != null)
      {
        if (this.m_attackInfo.m_proposedAttacker == null)
        {
          if (entity1 == null)
            return AttackType.ONLY_PROPOSED_DEFENDER;
          info = new AttackInfo();
          info.m_proposedAttacker = entity1;
          info.m_proposedDefender = this.m_attackInfo.m_proposedDefender;
          return AttackType.PROPOSED;
        }
        if (this.m_attackInfo.m_proposedDefender != null)
          return AttackType.PROPOSED;
        if (entity2 == null)
          return AttackType.ONLY_PROPOSED_ATTACKER;
        info = new AttackInfo();
        info.m_proposedAttacker = this.m_attackInfo.m_proposedAttacker;
        info.m_proposedDefender = entity2;
        return AttackType.PROPOSED;
      }
      if (attackType == AttackType.REGULAR || attackType == AttackType.INVALID)
        return AttackType.INVALID;
    }
    switch (attackType)
    {
      case AttackType.PROPOSED:
        if (entity1 != null && entity1.GetZone() != TAG_ZONE.PLAY || entity2 != null && entity2.GetZone() != TAG_ZONE.PLAY || entity1 != null && entity1.IsDormant() || entity2 != null && entity2.IsDormant())
          return AttackType.CANCELED;
        if (entity3 != entity1 || entity4 != entity2)
        {
          info = new AttackInfo();
          info.m_proposedAttacker = entity1;
          info.m_proposedDefender = entity2;
          return AttackType.PROPOSED;
        }
        if (entity1 == null || entity2 == null || this.IsEndOfBlock())
          return AttackType.CANCELED;
        info = new AttackInfo();
        info.m_proposedAttacker = entity1;
        info.m_proposedDefender = entity2;
        info.m_repeatProposed = true;
        return AttackType.PROPOSED;
      case AttackType.CANCELED:
        return AttackType.INVALID;
      default:
        if (this.IsEndOfBlock())
        {
          if (attackType == AttackType.ONLY_ATTACKER || attackType == AttackType.WAITING_ON_DEFENDER)
            return AttackType.CANCELED;
          Debug.LogWarningFormat("AttackSpellController.DetermineAttackType() - INVALID ATTACK prevAttackType={0} prevAttacker={1} prevDefender={2}", (object) attackType, (object) entity3, (object) entity4);
          return AttackType.INVALID;
        }
        switch (attackType)
        {
          case AttackType.ONLY_ATTACKER:
          case AttackType.WAITING_ON_DEFENDER:
            return AttackType.WAITING_ON_DEFENDER;
          case AttackType.ONLY_DEFENDER:
          case AttackType.WAITING_ON_ATTACKER:
            return AttackType.WAITING_ON_ATTACKER;
          case AttackType.ONLY_PROPOSED_ATTACKER:
          case AttackType.WAITING_ON_PROPOSED_DEFENDER:
            return AttackType.WAITING_ON_PROPOSED_DEFENDER;
          case AttackType.ONLY_PROPOSED_DEFENDER:
          case AttackType.WAITING_ON_PROPOSED_ATTACKER:
            return AttackType.WAITING_ON_PROPOSED_ATTACKER;
          default:
            return AttackType.INVALID;
        }
    }
  }

  public void FixupLastTagChangeForEntityTag(
    int changeEntity,
    int changeTag,
    int newValue,
    bool fixLast = true)
  {
    if (fixLast)
    {
      for (int index = this.m_tasks.Count - 1; index >= 0; --index)
      {
        if (this.m_tasks[index].GetPower() is Network.HistTagChange power && changeEntity == power.Entity && changeTag == power.Tag)
        {
          power.Value = newValue;
          break;
        }
      }
    }
    else
    {
      for (int index = 0; index < this.m_tasks.Count; ++index)
      {
        if (this.m_tasks[index].GetPower() is Network.HistTagChange power && changeEntity == power.Entity && changeTag == power.Tag)
        {
          power.Value = newValue;
          break;
        }
      }
    }
  }

  public delegate void CompleteCallback(
    PowerTaskList taskList,
    int startIndex,
    int count,
    object userData);

  public class DamageInfo
  {
    public Entity m_entity;
    public int m_damage;
  }

  private class ZoneChangeCallbackData
  {
    public int m_startIndex;
    public int m_count;
    public PowerTaskList.CompleteCallback m_taskListCallback;
    public object m_taskListUserData;
  }
}
