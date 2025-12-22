using Blizzard.T5.Core;
using Cysharp.Threading.Tasks;
using Hearthstone;
using PegasusGame;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PowerProcessor
{
  private const string ATTACK_SPELL_CONTROLLER_PREFAB_PATH = "AttackSpellController.prefab:12acecc85ac575e43b87ec141b89269a";
  private const string SECRET_SPELL_CONTROLLER_PREFAB_PATH = "SecretSpellController.prefab:553af99c12154c547bc05dc3d9832931";
  private const string SIDE_QUEST_SPELL_CONTROLLER_PREFAB_PATH = "SideQuestSpellController.prefab:63762d08481f04642bbf3cde299feea2";
  private const string SIGIL_SPELL_CONTROLLER_PREFAB_PATH = "SigilSpellController.prefab:1f80634fbf70a654bbae7bf796bf11b2";
  private const string OBJECTIVE_SPELL_CONTROLLER_PREFAB_PATH = "ObjectiveSpellController.prefab:a3d627bc67f24e740a2e967b383ecc6e";
  private const string JOUST_SPELL_CONTROLLER_PREFAB_PATH = "JoustSpellController.prefab:89ac256005a4a8a46939a84460c2c221";
  private const string RITUAL_SPELL_CONTROLLER_PREFAB_PATH = "RitualSpellController.prefab:27c7bd4ffaa54fb4e9e64dad14a6e701";
  private const string REVEAL_CARD_SPELL_CONTROLLER_PREFAB_PATH = "RevealCardSpellController.prefab:17fd7ea79bfd4c24389d535a074199b6";
  private const string TRIGGER_SPELL_CONTROLLER_PREFAB_PATH = "TriggerSpellController.prefab:e0a2661f98a720d47ad4b85de228f4b4";
  private const string RESET_GAME_SPELL_CONTROLLER_PREFAB_PATH = "ResetGameSpellController.prefab:d8c1994d523574e42bffa17990917754";
  private const string SUB_SPELL_CONTROLLER_PREFAB_PATH = "SubSpellController.prefab:34966ff41154fce469d3ccb6d3b1655e";
  private const string INVOKE_SPELL_CONTROLLER_PREFAB_PATH = "InvokeSpellController.prefab:333b9273e033dd348ab0d5f81a5bbbcd";
  private int m_nextTaskListId = 1;
  private bool m_buildingTaskList;
  private int m_totalSlushTime;
  private PowerHistoryTimeline m_currentTimeline;
  private List<PowerProcessor.OnTaskEvent> m_taskEventListeners = new List<PowerProcessor.OnTaskEvent>();
  private Stack<PowerTaskList> m_previousStack = new Stack<PowerTaskList>();
  private Stack<List<PowerTaskList>> m_deferredStack = new Stack<List<PowerTaskList>>();
  private Stack<PowerTaskList> m_subSpellOriginStack = new Stack<PowerTaskList>();
  private Queue<PowerProcessor.DelayedRealTimeTask> m_delayedRealTimeTasks = new Queue<PowerProcessor.DelayedRealTimeTask>();
  private PowerQueue m_powerQueue = new PowerQueue();
  private PowerTaskList m_currentTaskList;
  private PowerTaskList m_previousTaskList;
  private SubSpellController m_subSpellController;
  private bool m_historyBlocking;
  private bool m_artificialPauseFromMetadata;
  private PowerTaskList m_historyBlockingTaskList;
  private PowerTaskList m_busyTaskList;
  private PowerTaskList m_earlyConcedeTaskList;
  private bool m_handledFirstEarlyConcede;
  private PowerTaskList m_gameOverTaskList;
  private List<PowerHistoryTimeline> m_powerHistoryTimeline = new List<PowerHistoryTimeline>();
  private Map<int, int> m_powerHistoryTimelineIdIndex = new Map<int, int>();
  private PowerTaskList m_powerHistoryFirstTaskList;
  private PowerTaskList m_powerHistoryLastTaskList;

  public PowerProcessor() => this.m_deferredStack.Push(new List<PowerTaskList>());

  public PowerTaskList GetCurrentTaskList() => this.m_currentTaskList;

  public PowerQueue GetPowerQueue() => this.m_powerQueue;

  public void AddTaskEventListener(PowerProcessor.OnTaskEvent listener) => this.m_taskEventListeners.Add(listener);

  public void RemoveTaskEventListener(PowerProcessor.OnTaskEvent listener) => this.m_taskEventListeners.Remove(listener);

  public void FireTaskEvent(float expectedDiff)
  {
    foreach (PowerProcessor.OnTaskEvent onTaskEvent in this.m_taskEventListeners.ToArray())
      onTaskEvent(expectedDiff);
  }

  public void OnMetaData(Network.HistMetaData metaData)
  {
    if (metaData.MetaType == HistoryMeta.Type.SHOW_BIG_CARD)
    {
      int data = metaData.Data;
      Player player = GameState.Get().GetPlayer(data);
      if (player != null && player.GetSide() != Player.Side.FRIENDLY && InputManager.Get().PermitDecisionMakingInput())
        return;
      int id = metaData.Info[0];
      Entity entity = GameState.Get().GetEntity(id);
      if (entity == null || string.IsNullOrEmpty(entity.GetCardId()))
        return;
      this.SetHistoryBlockingTaskList();
      Entity sourceEntity = this.m_currentTaskList.GetSourceEntity();
      HistoryBlock.Type blockType = this.m_currentTaskList.GetBlockType();
      if (sourceEntity != null && sourceEntity.HasTag(GAME_TAG.FAST_BATTLECRY) && blockType == HistoryBlock.Type.POWER)
      {
        HistoryManager.Get().CreateFastBigCardFromMetaData(entity);
      }
      else
      {
        int displayTimeMS = metaData.Info.Count > 1 ? metaData.Info[1] : 0;
        HistoryManager.Get().CreatePlayedBigCard(entity, new HistoryManager.BigCardStartedCallback(this.OnBigCardStarted), new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), true, false, displayTimeMS);
      }
    }
    else if (metaData.MetaType == HistoryMeta.Type.BEGIN_LISTENING_FOR_TURN_EVENTS)
    {
      TurnStartManager.Get().BeginListeningForTurnEvents(true);
    }
    else
    {
      if (metaData.MetaType != HistoryMeta.Type.ARTIFICIAL_PAUSE)
        return;
      int data = metaData.Data;
      if (!((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null))
        return;
      this.ArtificiallyPausePowerProcessor((float) data, Gameplay.Get().PausePowerToken).Forget();
    }
  }

  public async UniTaskVoid ArtificiallyPausePowerProcessor(
    float pauseTimeMS,
    CancellationToken token)
  {
    this.m_artificialPauseFromMetadata = true;
    float timeToWait = pauseTimeMS / 1000f;
    float timeWaited = 0.0f;
    if ((double) timeToWait > 0.0)
    {
      GameState.Get().GetFriendlySidePlayer().GetHandZone().UpdateLayout();
      GameState.Get().GetOpposingSidePlayer().GetHandZone().UpdateLayout();
      GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().UpdateLayout();
      GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().UpdateLayout();
    }
    for (; (double) timeWaited < (double) timeToWait; timeWaited += Time.deltaTime)
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    this.m_artificialPauseFromMetadata = false;
  }

  public bool IsHistoryBlocking() => this.m_historyBlocking;

  public PowerTaskList GetHistoryBlockingTaskList() => this.m_historyBlockingTaskList;

  public void SetHistoryBlockingTaskList()
  {
    if (this.m_historyBlockingTaskList != null)
      return;
    this.m_historyBlockingTaskList = this.m_currentTaskList;
  }

  public void ForceStopHistoryBlocking()
  {
    this.m_historyBlocking = false;
    this.m_historyBlockingTaskList = (PowerTaskList) null;
  }

  public PowerTaskList GetLastTaskList()
  {
    int count = this.m_powerQueue.Count;
    return count > 0 ? this.m_powerQueue[count - 1] : this.m_currentTaskList;
  }

  public bool HasEarlyConcedeTaskList() => this.m_earlyConcedeTaskList != null;

  public bool HasGameOverTaskList() => this.m_gameOverTaskList != null;

  public bool CanDoRealTimeTask()
  {
    GameState gameState = GameState.Get();
    return gameState != null && !gameState.IsResetGamePending();
  }

  public bool CanDoTask(PowerTask task)
  {
    if (task.IsCompleted())
      return true;
    Network.PowerHistory power = task.GetPower();
    return (power.Type != Network.PowerType.META_DATA || ((Network.HistMetaData) power).MetaType != HistoryMeta.Type.SHOW_BIG_CARD || !HistoryManager.Get().IsShowingBigCard()) && !GameState.Get().IsBusy() && !this.m_artificialPauseFromMetadata;
  }

  public void ForEachTaskList(Action<int, PowerTaskList> predicate)
  {
    if (this.m_currentTaskList != null)
      predicate(-1, this.m_currentTaskList);
    for (int index = 0; index < this.m_powerQueue.Count; ++index)
      predicate(index, this.m_powerQueue[index]);
  }

  public bool HasTaskLists() => this.m_currentTaskList != null || this.m_powerQueue.Count > 0;

  public bool HasTaskList(PowerTaskList taskList) => taskList != null && (this.m_currentTaskList == taskList || this.m_powerQueue.Contains(taskList));

  public void OnPowerHistory(List<Network.PowerHistory> powerList)
  {
    this.m_totalSlushTime = 0;
    this.m_buildingTaskList = true;
    this.m_powerHistoryFirstTaskList = (PowerTaskList) null;
    this.m_powerHistoryLastTaskList = (PowerTaskList) null;
    this.m_currentTimeline = new PowerHistoryTimeline();
    for (int index = 0; index < powerList.Count; ++index)
    {
      PowerTaskList taskList1 = new PowerTaskList();
      if (this.m_previousStack.Count > 0)
      {
        PowerTaskList taskList2 = this.m_previousStack.Pop();
        taskList1.SetPrevious(taskList2);
        this.m_previousStack.Push(taskList1);
      }
      if (this.m_subSpellOriginStack.Count > 0)
      {
        PowerTaskList taskList3 = this.m_subSpellOriginStack.Peek();
        if (taskList1.GetOrigin() == taskList3.GetOrigin())
          taskList1.SetSubSpellOrigin(taskList3);
      }
      this.BuildTaskList(powerList, ref index, taskList1);
    }
    if (GameState.Get().AllowBatchedPowers())
    {
      for (int index1 = this.m_powerQueue.Count - 1; index1 > 0; --index1)
      {
        PowerTaskList power1 = this.m_powerQueue[index1];
        if (power1.IsBatchable())
        {
          int index2 = index1 - 1;
          PowerTaskList power2 = this.m_powerQueue[index2];
          while ((power2.IsSlushTimeHelper() || !power2.HasAnyTasksInImmediate()) && index2 > 0)
            power2 = this.m_powerQueue[--index2];
          if (power1.IsBatchable() && power2.IsBatchable() && power1.GetBlockStart().TriggerKeyword == power2.GetBlockStart().TriggerKeyword)
          {
            power1.FillMetaDataTargetSourceData();
            power2.FillMetaDataTargetSourceData();
            power2.AddTasks(power1);
            foreach (int entity in power1.GetBlockStart().Entities)
            {
              if (!power2.GetBlockStart().Entities.Contains(entity))
                power2.GetBlockStart().Entities.Add(entity);
            }
            this.m_powerQueue.RemoveAt(index1);
          }
        }
      }
    }
    if (GameState.Get().AllowDeferredPowers())
    {
      this.FixUpOutOfOrderDeferredTasks();
      for (int index = this.m_powerQueue.Count - 1; index > 0; --index)
      {
        PowerTaskList power3 = this.m_powerQueue[index];
        if (power3.GetPrevious() == this.m_powerQueue[index - 1] && power3.IsCollapsible(false) && power3.GetPrevious().IsCollapsible(true))
        {
          power3.GetPrevious().AddTasks(power3);
          power3.GetPrevious().SetNext((PowerTaskList) null);
          if (power3.GetBlockEnd() != null)
            power3.GetPrevious().SetBlockEnd(power3.GetBlockEnd());
          foreach (PowerTaskList power4 in (QueueList<PowerTaskList>) this.m_powerQueue)
          {
            if (power4.GetPrevious() == power3)
              power4.SetPrevious(power3.GetPrevious());
          }
          this.m_powerQueue.RemoveAt(index);
        }
      }
    }
    if (this.m_totalSlushTime > 0 && this.m_powerHistoryFirstTaskList != null && this.m_powerHistoryLastTaskList != null)
    {
      PowerTaskList historyFirstTaskList = this.m_powerHistoryFirstTaskList;
      PowerTaskList historyLastTaskList = this.m_powerHistoryLastTaskList;
      historyFirstTaskList.SetHistoryBlockStart(true);
      historyLastTaskList.SetHistoryBlockEnd(true);
      this.m_currentTimeline.m_firstTaskId = historyFirstTaskList.GetId();
      this.m_currentTimeline.m_lastTaskId = historyLastTaskList.GetId();
      this.m_currentTimeline.m_slushTime = this.m_totalSlushTime;
      this.m_powerHistoryTimeline.Add(this.m_currentTimeline);
      this.m_powerHistoryTimelineIdIndex.Add(this.m_currentTimeline.m_firstTaskId, this.m_powerHistoryTimeline.Count - 1);
      this.m_powerHistoryTimelineIdIndex.Add(this.m_currentTimeline.m_lastTaskId, this.m_powerHistoryTimeline.Count - 1);
      foreach (PowerHistoryTimelineEntry orderedEvent in this.m_currentTimeline.m_orderedEvents)
      {
        if (!this.m_powerHistoryTimelineIdIndex.ContainsKey(orderedEvent.taskId))
          this.m_powerHistoryTimelineIdIndex.Add(orderedEvent.taskId, this.m_powerHistoryTimeline.Count - 1);
      }
    }
    this.m_buildingTaskList = false;
  }

  private void FixUpOutOfOrderDeferredTasks()
  {
    if (!GameState.Get().AllowDeferredPowers())
      return;
    for (int index = this.m_powerQueue.Count - 1; index >= 0; --index)
    {
      PowerTaskList power = this.m_powerQueue[index];
      if (power.IsDeferrable())
        this.FixUpOutOfOrderDeferredTasksInTasklist(power);
    }
  }

  private void FixUpOutOfOrderDeferredTasksInTasklist(PowerTaskList deferredTaskList)
  {
    if (!GameState.Get().AllowDeferredPowers())
      return;
    Map<int, Map<int, List<int>>> changesForTaskList1 = this.GetEntityChangesForTaskList(deferredTaskList);
    for (int index = 0; index < this.m_powerQueue.Count; ++index)
    {
      PowerTaskList power = this.m_powerQueue[index];
      if (power.GetId() == deferredTaskList.GetId())
        break;
      if (power.GetId() >= deferredTaskList.GetDeferredSourceId())
      {
        if (power.IsDeferrable())
          break;
        Map<int, Map<int, List<int>>> changesForTaskList2 = this.GetEntityChangesForTaskList(power);
        foreach (KeyValuePair<int, Map<int, List<int>>> keyValuePair1 in changesForTaskList1)
        {
          int key1 = keyValuePair1.Key;
          Map<int, List<int>> map1 = keyValuePair1.Value;
          if (changesForTaskList2.ContainsKey(key1))
          {
            Map<int, List<int>> map2 = changesForTaskList2[key1];
            foreach (KeyValuePair<int, List<int>> keyValuePair2 in map1)
            {
              int key2 = keyValuePair2.Key;
              List<int> intList1 = keyValuePair2.Value;
              if (map2.ContainsKey(key2))
              {
                List<int> intList2 = map2[key2];
                int newValue1 = intList1[intList1.Count - 1];
                int newValue2 = intList2[intList2.Count - 1];
                deferredTaskList.FixupLastTagChangeForEntityTag(key1, key2, newValue2);
                power.FixupLastTagChangeForEntityTag(key1, key2, newValue1, false);
              }
            }
          }
        }
      }
    }
  }

  private Map<int, Map<int, List<int>>> GetEntityChangesForTaskList(PowerTaskList taskList)
  {
    Map<int, Map<int, List<int>>> changesForTaskList = new Map<int, Map<int, List<int>>>();
    foreach (PowerTask tagChangeTask in taskList.GetTagChangeTasks())
    {
      Network.HistTagChange power = tagChangeTask.GetPower() as Network.HistTagChange;
      if (!changesForTaskList.ContainsKey(power.Entity))
        changesForTaskList.Add(power.Entity, new Map<int, List<int>>());
      if (!changesForTaskList[power.Entity].ContainsKey(power.Tag))
        changesForTaskList[power.Entity].Add(power.Tag, new List<int>());
      changesForTaskList[power.Entity][power.Tag].Add(power.Value);
    }
    return changesForTaskList;
  }

  public void HandleTimelineStartEvent(
    int tasklistId,
    float time,
    bool isBlockStart,
    Network.HistBlockStart blockStart)
  {
    if (!this.m_powerHistoryTimelineIdIndex.ContainsKey(tasklistId))
      return;
    PowerHistoryTimeline powerHistoryTimeline = this.m_powerHistoryTimeline[this.m_powerHistoryTimelineIdIndex[tasklistId]];
    if (isBlockStart)
    {
      powerHistoryTimeline.m_startTime = time;
      if (!HearthstoneApplication.IsPublic())
        Debug.Log((object) string.Format("Timeline start event: (TasklistId: {0}) ---- (Expected Duration: {1})", (object) tasklistId, (object) (float) ((double) powerHistoryTimeline.m_slushTime * (1.0 / 1000.0))));
    }
    if (!powerHistoryTimeline.m_orderedEventIndexLookup.ContainsKey(tasklistId))
      return;
    int index = powerHistoryTimeline.m_orderedEventIndexLookup[tasklistId];
    PowerHistoryTimelineEntry orderedEvent = powerHistoryTimeline.m_orderedEvents[index];
    orderedEvent.entityId = blockStart != null ? blockStart.Entities[0] : 0;
    float num1 = (float) orderedEvent.expectedStartOffset * (1f / 1000f);
    float num2 = time - powerHistoryTimeline.m_startTime;
    orderedEvent.actualStartTime = num2;
    this.FireTaskEvent(num2 - num1);
    if (HearthstoneApplication.IsPublic())
      return;
    Debug.Log((object) string.Format("Task start event: (TasklistId: {0}) ---- (Expected: {1} ---- (Actual: {2}))", (object) tasklistId, (object) num1, (object) num2));
  }

  public void HandleTimelineEndEvent(int tasklistId, float time, bool isBlockEnd)
  {
    if (!this.m_powerHistoryTimelineIdIndex.ContainsKey(tasklistId))
      return;
    PowerHistoryTimeline powerHistoryTimeline = this.m_powerHistoryTimeline[this.m_powerHistoryTimelineIdIndex[tasklistId]];
    if (powerHistoryTimeline.m_orderedEventIndexLookup.ContainsKey(tasklistId))
    {
      int index = powerHistoryTimeline.m_orderedEventIndexLookup[tasklistId];
      PowerHistoryTimelineEntry orderedEvent = powerHistoryTimeline.m_orderedEvents[index];
      float expectedEnd = (float) (orderedEvent.expectedStartOffset + orderedEvent.expectedTime) * (1f / 1000f);
      float actualEnd = time - powerHistoryTimeline.m_startTime;
      this.FireTaskEvent(actualEnd - expectedEnd);
      if (!HearthstoneApplication.IsPublic())
      {
        Debug.Log((object) string.Format("Task end event: (TasklistId: {0}) ---- (Expected: {1} ---- (Actual: {2}))", (object) tasklistId, (object) expectedEnd, (object) actualEnd));
        SceneDebugger.Get().AddSlushTimeEntry(tasklistId, (float) orderedEvent.expectedStartOffset * (1f / 1000f), expectedEnd, orderedEvent.actualStartTime, actualEnd, orderedEvent.entityId);
      }
    }
    if (!isBlockEnd)
      return;
    powerHistoryTimeline.m_endTime = time;
    if (HearthstoneApplication.IsPublic())
      return;
    Debug.Log((object) string.Format("Timeline end event: (TasklistId: {0}) ---- (Expected: {1}) ---- (Actual: {2})", (object) tasklistId, (object) (float) ((double) powerHistoryTimeline.m_slushTime * (1.0 / 1000.0)), (object) (float) ((double) powerHistoryTimeline.m_endTime - (double) powerHistoryTimeline.m_startTime)));
  }

  public void ProcessPowerQueue()
  {
    while (GameState.Get().CanProcessPowerQueue())
    {
      if (this.m_busyTaskList != null)
      {
        this.m_busyTaskList = (PowerTaskList) null;
      }
      else
      {
        PowerTaskList taskList = this.m_powerQueue.Peek();
        if ((UnityEngine.Object) HistoryManager.Get() != (UnityEngine.Object) null && HistoryManager.Get().IsShowingBigCard())
        {
          if (this.m_historyBlockingTaskList != null && !taskList.IsDescendantOfBlock(this.m_historyBlockingTaskList) || this.m_historyBlockingTaskList == null)
            break;
        }
        else
          this.m_historyBlockingTaskList = (PowerTaskList) null;
        this.OnWillProcessTaskList(taskList);
        if (GameState.Get().IsBusy())
        {
          this.m_busyTaskList = taskList;
          break;
        }
      }
      if (this.CanEarlyConcede())
      {
        if (this.m_earlyConcedeTaskList == null && !this.m_handledFirstEarlyConcede)
        {
          this.DoEarlyConcedeVisuals();
          this.m_handledFirstEarlyConcede = true;
        }
        while (this.m_powerQueue.Count > 0)
        {
          this.m_currentTaskList = this.m_powerQueue.Dequeue();
          this.m_currentTaskList.DebugDump();
          this.CancelSpellsForEarlyConcede(this.m_currentTaskList);
          bool flag = false;
          if (GameState.Get().IsFinalWrapupStep() && GameState.Get().GetBooleanGameOption(GameEntityOption.EARLY_CONCEDE_PROCESS_SUB_SPELL_IN_FINAL_WRAPUP_STEP) && this.m_currentTaskList.IsSubSpellTaskList())
            flag = this.DoSubSpellTaskListWithController(this.m_currentTaskList);
          if (!flag)
            this.m_currentTaskList.DoEarlyConcedeTasks();
          this.m_currentTaskList = (PowerTaskList) null;
        }
        break;
      }
      this.m_currentTaskList = this.m_powerQueue.Dequeue();
      if (this.m_previousTaskList == null || this.m_previousTaskList.GetOrigin() != this.m_currentTaskList.GetOrigin() || this.m_previousTaskList.GetParent() != this.m_currentTaskList.GetParent())
      {
        GameState.Get().ResetFriendlyCardDrawCounter();
        GameState.Get().ResetOpponentCardDrawCounter();
      }
      this.m_currentTaskList.DebugDump();
      this.OnProcessTaskList();
      this.StartCurrentTaskList();
    }
  }

  private int GetNextTaskListId()
  {
    int nextTaskListId = this.m_nextTaskListId;
    this.m_nextTaskListId = this.m_nextTaskListId == int.MaxValue ? 1 : this.m_nextTaskListId + 1;
    return nextTaskListId;
  }

  private bool CanDeferTaskList(Network.PowerHistory power) => GameState.Get().AllowDeferredPowers() && power is Network.HistBlockStart histBlockStart && histBlockStart.IsDeferrable;

  private bool CanBatchTaskList(Network.PowerHistory power) => GameState.Get().AllowBatchedPowers() && power is Network.HistBlockStart histBlockStart && histBlockStart.IsBatchable;

  private bool IsDeferBlockerTaskList(Network.PowerHistory power)
  {
    if (!(power is Network.HistBlockStart histBlockStart))
      return false;
    if (histBlockStart.IsDeferBlocker || histBlockStart.BlockType == HistoryBlock.Type.TRIGGER && !histBlockStart.IsDeferrable)
      return true;
    return histBlockStart.BlockType == HistoryBlock.Type.ATTACK && !histBlockStart.IsDeferrable;
  }

  private void BuildTaskList(
    List<Network.PowerHistory> powerList,
    ref int index,
    PowerTaskList taskList)
  {
    while (index < powerList.Count)
    {
      Network.PowerHistory power = powerList[index];
      Network.PowerType type = power.Type;
      switch (type)
      {
        case Network.PowerType.BLOCK_START:
          if (!taskList.IsEmpty())
          {
            this.EnqueueTaskList(taskList);
            if (taskList.IsDeferrable())
            {
              taskList.SetDeferrable(false);
              List<PowerTaskList> powerTaskListList = this.m_deferredStack.Pop();
              if (this.m_deferredStack.Count > 0 && this.m_deferredStack.Peek().Contains(taskList))
                this.m_deferredStack.Peek().Remove(taskList);
              this.m_deferredStack.Push(powerTaskListList);
            }
          }
          PowerTaskList taskList1 = new PowerTaskList();
          taskList1.SetBlockStart((Network.HistBlockStart) power);
          PowerTaskList origin = taskList.GetOrigin();
          if (origin.IsStartOfBlock())
            taskList1.SetParent(origin);
          this.m_previousStack.Push(taskList1);
          if (this.IsDeferBlockerTaskList(power))
          {
            this.EnqueueDeferredTaskLists(false);
            this.m_deferredStack.Push(new List<PowerTaskList>());
          }
          if (this.CanDeferTaskList(power))
          {
            if (this.m_deferredStack.Count > 0)
            {
              this.m_deferredStack.Peek().Add(taskList1);
              taskList1.SetDeferrable(true);
            }
          }
          else
            taskList1.SetBatchable(this.CanBatchTaskList(power));
          this.m_deferredStack.Push(new List<PowerTaskList>());
          ++index;
          this.BuildTaskList(powerList, ref index, taskList1);
          return;
        case Network.PowerType.BLOCK_END:
          taskList.SetBlockEnd((Network.HistBlockEnd) power);
          if (this.m_previousStack.Count > 0)
          {
            this.m_previousStack.Pop();
            if (!taskList.IsDeferrable())
            {
              this.EnqueueTaskList(taskList);
              this.EnqueueDeferredTaskLists(true);
              return;
            }
            if (this.m_powerQueue.Count > 0)
              this.m_powerQueue.GetItem(this.m_powerQueue.Count - 1).SetCollapsible(true);
            taskList.SetDeferredSourceId(this.m_nextTaskListId);
            if (this.m_deferredStack.Count <= 0)
              return;
            List<PowerTaskList> collection = this.m_deferredStack.Pop();
            if (this.m_deferredStack.Count > 0)
            {
              this.m_deferredStack.Peek()?.AddRange((IEnumerable<PowerTaskList>) collection);
              return;
            }
            this.m_deferredStack.Push(collection);
            return;
          }
          goto label_53;
        case Network.PowerType.SUB_SPELL_START:
          if (!taskList.HasTasks())
          {
            Network.HistMetaData netPower = new Network.HistMetaData()
            {
              MetaType = HistoryMeta.Type.ARTIFICIAL_HISTORY_INTERRUPT
            };
            taskList.CreateTask((Network.PowerHistory) netPower);
          }
          this.EnqueueTaskList(taskList);
          if (taskList.IsDeferrable())
          {
            taskList.SetDeferrable(false);
            List<PowerTaskList> powerTaskListList = this.m_deferredStack.Pop();
            if (this.m_deferredStack.Count > 0 && this.m_deferredStack.Peek().Contains(taskList))
              this.m_deferredStack.Peek().Remove(taskList);
            this.m_deferredStack.Push(powerTaskListList);
          }
          PowerTaskList taskList2 = new PowerTaskList();
          taskList2.SetPrevious(taskList);
          taskList2.SetParent(taskList.GetParent());
          taskList2.SetSubSpellOrigin(taskList2);
          taskList2.SetSubSpellStart((Network.HistSubSpellStart) power);
          this.m_subSpellOriginStack.Push(taskList2);
          if (this.m_previousStack.Count > 0 && this.m_previousStack.Peek() == taskList)
          {
            this.m_previousStack.Pop();
            this.m_previousStack.Push(taskList2);
          }
          taskList = taskList2;
          goto default;
        case Network.PowerType.SUB_SPELL_END:
          taskList.CreateTask(power);
          taskList.SetSubSpellEnd((Network.HistSubSpellEnd) power);
          this.EnqueueTaskList(taskList);
          if (this.m_subSpellOriginStack.Count > 0)
          {
            if (this.m_subSpellOriginStack.Pop() != taskList.GetSubSpellOrigin())
              Log.Power.PrintError("{0}.BuildTaskList(): Mismatch between SUB_SPELL_END task and current task list's SubSpellOrigin!", (object) this);
          }
          else
            Log.Power.PrintError("{0}.BuildTaskList(): Hit a SUB_SPELL_END task without a corresponding open SubSpellOrigin!", (object) this);
          if (index + 1 < powerList.Count)
          {
            PowerTaskList powerTaskList = new PowerTaskList();
            powerTaskList.SetPrevious(taskList);
            powerTaskList.SetParent(taskList.GetParent());
            if (this.m_subSpellOriginStack.Count > 0 && this.m_subSpellOriginStack.Peek().GetParent() == taskList.GetParent())
              powerTaskList.SetSubSpellOrigin(this.m_subSpellOriginStack.Peek());
            if (this.m_previousStack.Count > 0 && this.m_previousStack.Peek() == taskList)
            {
              this.m_previousStack.Pop();
              this.m_previousStack.Push(powerTaskList);
            }
            taskList = powerTaskList;
            break;
          }
          goto default;
        default:
          PowerTask task = taskList.CreateTask(power);
          if (type == Network.PowerType.META_DATA && ((Network.HistMetaData) power).MetaType == HistoryMeta.Type.ARTIFICIAL_HISTORY_INTERRUPT)
          {
            this.EnqueueTaskList(taskList);
            return;
          }
          if (this.CanDoRealTimeTask())
          {
            task.DoRealTimeTask(powerList, index);
            break;
          }
          this.m_delayedRealTimeTasks.Enqueue(new PowerProcessor.DelayedRealTimeTask()
          {
            m_index = index,
            m_powerTask = task,
            m_powerHistory = new List<Network.PowerHistory>((IEnumerable<Network.PowerHistory>) powerList)
          });
          break;
      }
      ++index;
    }
label_53:
    if (!taskList.IsEmpty())
      this.EnqueueTaskList(taskList);
    if (this.m_deferredStack.Count == 0)
      return;
    this.EnqueueDeferredTaskLists(true);
    if (this.m_deferredStack.Count != 0)
      return;
    this.m_deferredStack.Push(new List<PowerTaskList>());
  }

  private void EnqueueDeferredTaskLists(bool combine)
  {
    if (this.m_deferredStack.Count <= 0)
      return;
    List<PowerTaskList> powerTaskListList = this.m_deferredStack.Pop();
    for (int index1 = powerTaskListList.Count - 1; index1 > 0; --index1)
    {
      PowerTaskList otherTaskList = powerTaskListList[index1];
      if (otherTaskList.GetBlockStart() != null && combine)
      {
        for (int index2 = index1 - 1; index2 >= 0; --index2)
        {
          PowerTaskList powerTaskList = powerTaskListList[index2];
          if (powerTaskList.GetBlockStart() != null && powerTaskList.GetBlockStart().Entities.Count == otherTaskList.GetBlockStart().Entities.Count)
          {
            bool flag = true;
            foreach (int entity in powerTaskList.GetBlockStart().Entities)
            {
              if (!otherTaskList.GetBlockStart().Entities.Contains(entity))
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              powerTaskList.AddTasks(otherTaskList);
              powerTaskListList.RemoveAt(index1);
              break;
            }
          }
        }
      }
    }
    foreach (PowerTaskList taskList in powerTaskListList)
      this.EnqueueTaskList(taskList);
  }

  public bool EntityHasPendingTasks(Entity entity)
  {
    int entityId = entity.GetEntityId();
    foreach (PowerTaskList power in (QueueList<PowerTaskList>) this.m_powerQueue)
    {
      List<Entity> sourceEntities1 = power.GetSourceEntities(false);
      if (sourceEntities1 != null && sourceEntities1.Exists((Predicate<Entity>) (e => e != null && e.GetEntityId() == entityId)))
        return true;
      Entity targetEntity1 = power.GetTargetEntity(false);
      if (targetEntity1 != null && targetEntity1.GetEntityId() == entityId)
        return true;
      PowerTaskList parent = power.GetParent();
      if (parent != null)
      {
        List<Entity> sourceEntities2 = parent.GetSourceEntities(false);
        if (sourceEntities2 != null && sourceEntities2.Exists((Predicate<Entity>) (e => e != null && e.GetEntityId() == entityId)))
          return true;
        Entity targetEntity2 = parent.GetTargetEntity(false);
        if (targetEntity2 != null && targetEntity2.GetEntityId() == entityId)
          return true;
      }
    }
    return false;
  }

  public void FlushDelayedRealTimeTasks()
  {
    while (this.CanDoRealTimeTask() && this.m_delayedRealTimeTasks.Count > 0)
    {
      PowerProcessor.DelayedRealTimeTask delayedRealTimeTask = this.m_delayedRealTimeTasks.Dequeue();
      delayedRealTimeTask.m_powerTask.DoRealTimeTask(delayedRealTimeTask.m_powerHistory, delayedRealTimeTask.m_index);
    }
  }

  private void EnqueueTaskList(PowerTaskList taskList)
  {
    this.m_totalSlushTime += taskList.GetTotalSlushTime();
    if (this.m_powerHistoryFirstTaskList == null)
      this.m_powerHistoryFirstTaskList = taskList;
    else
      this.m_powerHistoryLastTaskList = taskList;
    taskList.SetId(this.GetNextTaskListId());
    this.m_powerQueue.Enqueue(taskList);
    if (this.m_currentTimeline != null && taskList.GetTotalSlushTime() > 0)
      this.m_currentTimeline.AddTimelineEntry(taskList.GetId(), taskList.GetTotalSlushTime());
    if (taskList.HasFriendlyConcede())
      this.m_earlyConcedeTaskList = taskList;
    if (!taskList.HasGameOver())
      return;
    this.m_gameOverTaskList = taskList;
  }

  private void OnWillProcessTaskList(PowerTaskList taskList)
  {
    if ((bool) (UnityEngine.Object) ThinkEmoteManager.Get())
      ThinkEmoteManager.Get().NotifyOfActivity();
    if (!taskList.IsStartOfBlock() || taskList.GetBlockStart().BlockType != HistoryBlock.Type.PLAY)
      return;
    Entity sourceEntity = taskList.GetSourceEntity(false);
    if (!sourceEntity.GetController().IsOpposingSide())
      return;
    string cardId = sourceEntity.GetCardId();
    if (string.IsNullOrEmpty(cardId))
      cardId = this.FindRevealedCardId(taskList);
    GameState.Get().GetGameEntity().NotifyOfOpponentWillPlayCard(cardId, sourceEntity);
  }

  private bool ContainsBurnedCard(PowerTaskList taskList) => this.ContainsMetaDataTaskWithInfo(taskList, HistoryMeta.Type.BURNED_CARD);

  private bool ContainsPoisonousEffect(PowerTaskList taskList) => this.ContainsMetaDataTaskWithInfo(taskList, HistoryMeta.Type.POISONOUS);

  private bool ContainsCriticalHitEffect(PowerTaskList taskList) => this.ContainsMetaDataTaskWithInfo(taskList, HistoryMeta.Type.CRITICAL_HIT);

  private bool ContainsMetaDataTaskWithInfo(PowerTaskList taskList, HistoryMeta.Type metaType)
  {
    List<PowerTask> taskList1 = taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      Network.PowerHistory power = taskList1[index].GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == metaType)
        {
          if (histMetaData.Info.Count == 0)
          {
            Log.Power.PrintError("PowerProcessor.ContainsMetaDataTaskWithInfo(): metaData.Info.Count is 0, metaType: {0}", (object) metaType);
          }
          else
          {
            if (GameState.Get().GetEntity(histMetaData.Info[0]) != null)
              return true;
            Log.Power.PrintError("PowerProcessor.ContainsMetaDataTaskWithInfo(): metaData.Info contains an invalid entity (ID {0}), metaType: {1}", (object) histMetaData.Info[0], (object) metaType);
          }
        }
      }
    }
    return false;
  }

  private string FindRevealedCardId(PowerTaskList taskList)
  {
    taskList.GetBlockStart();
    List<PowerTask> taskList1 = taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      Network.HistShowEntity showEntity = taskList1[index].GetPower() as Network.HistShowEntity;
      if (showEntity != null && taskList.GetSourceEntities() != null && taskList.GetSourceEntities().Exists((Predicate<Entity>) (e => e != null && e.GetEntityId() == showEntity.Entity.ID)))
        return showEntity.Entity.CardID;
    }
    return (string) null;
  }

  private void OnProcessTaskList()
  {
    if (this.m_currentTaskList.IsStartOfBlock())
    {
      Network.HistBlockStart blockStart = this.m_currentTaskList.GetBlockStart();
      switch (blockStart.BlockType)
      {
        case HistoryBlock.Type.ATTACK:
          Entity attacker = this.m_currentTaskList.GetAttacker();
          Entity defender = (Entity) null;
          switch (this.m_currentTaskList.GetAttackType())
          {
            case AttackType.REGULAR:
              defender = this.m_currentTaskList.GetDefender();
              break;
            case AttackType.CANCELED:
              defender = this.m_currentTaskList.GetProposedDefender();
              break;
          }
          if (attacker != null && defender != null)
          {
            GameState.Get().GetGameEntity().NotifyOfEntityAttacked(attacker, defender);
            break;
          }
          break;
        case HistoryBlock.Type.POWER:
          Entity sourceEntity1 = this.m_currentTaskList.GetSourceEntity(false);
          Entity targetEntity1 = this.m_currentTaskList.GetTargetEntity(false);
          Card heroCard1 = sourceEntity1?.GetController()?.GetHeroCard();
          if ((UnityEngine.Object) heroCard1 != (UnityEngine.Object) null)
          {
            if (sourceEntity1.IsWeapon())
            {
              heroCard1.NotifyOfWeaponPlayed(sourceEntity1);
              break;
            }
            if (sourceEntity1.IsSpell())
            {
              heroCard1.NotifyOfSpellPlayed(sourceEntity1, targetEntity1);
              break;
            }
            if (sourceEntity1.IsHeroPower())
            {
              heroCard1.NotifyOfHeroPowerPlayed(sourceEntity1, targetEntity1);
              break;
            }
            break;
          }
          break;
        case HistoryBlock.Type.DEATHS:
          using (List<PowerTask>.Enumerator enumerator = this.m_currentTaskList.GetTaskList().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Network.PowerHistory power = enumerator.Current.GetPower();
              if (power.Type == Network.PowerType.TAG_CHANGE)
              {
                Network.HistTagChange tagChange = power as Network.HistTagChange;
                if (GameUtils.IsEntityDeathTagChange(tagChange))
                {
                  Entity entity = GameState.Get().GetEntity(tagChange.Entity);
                  if (entity.IsMinion())
                    GameState.Get().GetGameEntity().NotifyOfMinionDied(entity);
                  else if (entity.IsHero())
                    GameState.Get().GetGameEntity().NotifyOfHeroDied(entity);
                  else if (entity.IsWeapon())
                  {
                    GameState.Get().GetGameEntity().NotifyOfWeaponDestroyed(entity);
                    Player controller = entity.GetController();
                    if (controller != null)
                    {
                      Card heroCard2 = controller.GetHeroCard();
                      if ((UnityEngine.Object) heroCard2 != (UnityEngine.Object) null)
                        heroCard2.NotifyOfWeaponDestroyed(entity);
                    }
                  }
                }
              }
            }
            break;
          }
        case HistoryBlock.Type.PLAY:
          Entity sourceEntity2 = this.m_currentTaskList.GetSourceEntity(false);
          if (sourceEntity2.IsControlledByFriendlySidePlayer())
            GameState.Get().GetGameEntity().NotifyOfFriendlyPlayedCard(sourceEntity2);
          else
            GameState.Get().GetGameEntity().NotifyOfOpponentPlayedCard(sourceEntity2);
          if (sourceEntity2.IsMinion())
          {
            GameState.Get().GetGameEntity().NotifyOfMinionPlayed(sourceEntity2);
            break;
          }
          if (sourceEntity2.IsHero())
          {
            GameState.Get().GetGameEntity().NotifyOfHeroChanged(sourceEntity2);
            break;
          }
          if (sourceEntity2.IsWeapon())
          {
            GameState.Get().GetGameEntity().NotifyOfWeaponEquipped(sourceEntity2);
            break;
          }
          if (sourceEntity2.IsSpell())
          {
            Entity targetEntity2 = this.m_currentTaskList.GetTargetEntity(false);
            GameState.Get().GetGameEntity().NotifyOfSpellPlayed(sourceEntity2, targetEntity2);
            break;
          }
          if (sourceEntity2.IsHeroPower())
          {
            Entity targetEntity3 = this.m_currentTaskList.GetTargetEntity(false);
            GameState.Get().GetGameEntity().NotifyOfHeroPowerUsed(sourceEntity2, targetEntity3);
            break;
          }
          break;
      }
      if (blockStart.BlockType == HistoryBlock.Type.POWER || blockStart.BlockType == HistoryBlock.Type.TRIGGER)
      {
        for (int index = 0; index < blockStart.EffectCardId.Count; ++index)
        {
          if (string.IsNullOrEmpty(blockStart.EffectCardId[index]))
          {
            List<Entity> sourceEntities = this.m_currentTaskList.GetSourceEntities();
            if (sourceEntities != null && index < sourceEntities.Count && sourceEntities[index] != null)
            {
              blockStart.EffectCardId[index] = sourceEntities[index].GetCardId();
              blockStart.IsEffectCardIdClientCached[index] = true;
            }
          }
        }
      }
    }
    this.PrepareHistoryForCurrentTaskList();
    this.m_currentTaskList.CreateArtificialHistoryTilesFromMetadata();
  }

  private void PrepareHistoryForCurrentTaskList()
  {
    Log.Power.Print("PowerProcessor.PrepareHistoryForCurrentTaskList() - m_currentTaskList={0}", (object) this.m_currentTaskList.GetId());
    Network.HistBlockStart blockStart = this.m_currentTaskList.GetBlockStart();
    if (blockStart == null)
      return;
    List<Entity> sourceEntities = this.m_currentTaskList.GetSourceEntities();
    if (sourceEntities != null && sourceEntities.Exists((Predicate<Entity>) (e => e != null && e.HasTag(GAME_TAG.CARD_DOES_NOTHING))))
      return;
    switch (blockStart.BlockType)
    {
      case HistoryBlock.Type.ATTACK:
        AttackType attackType = this.m_currentTaskList.GetAttackType();
        Entity attacker = (Entity) null;
        Entity defender = (Entity) null;
        switch (attackType)
        {
          case AttackType.REGULAR:
            attacker = this.m_currentTaskList.GetAttacker();
            defender = this.m_currentTaskList.GetDefender();
            break;
          case AttackType.CANCELED:
            attacker = this.m_currentTaskList.GetAttacker();
            defender = this.m_currentTaskList.GetProposedDefender();
            break;
        }
        if (attacker != null && defender != null)
        {
          HistoryManager.Get().CreateAttackTile(attacker, defender, this.m_currentTaskList);
          this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
        }
        if (!HistoryManager.Get().HasHistoryEntry())
          break;
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
      case HistoryBlock.Type.JOUST:
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
      case HistoryBlock.Type.POWER:
        if (!HistoryManager.Get().HasHistoryEntry())
          break;
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
      case HistoryBlock.Type.TRIGGER:
        Entity sourceEntity1 = this.m_currentTaskList.GetSourceEntity(false);
        if (sourceEntity1 == null)
          break;
        if (sourceEntity1.IsSecret() || blockStart.TriggerKeyword == 1192 || blockStart.TriggerKeyword == 1749)
        {
          if (this.m_currentTaskList.IsStartOfBlock())
          {
            HistoryManager.Get().CreateTriggerTile(sourceEntity1);
            this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
            this.SetHistoryBlockingTaskList();
            HistoryManager.Get().CreateTriggeredBigCard(sourceEntity1, new HistoryManager.BigCardStartedCallback(this.OnBigCardStarted), new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), false, true);
          }
          this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
          break;
        }
        bool flag = false;
        if (!this.m_currentTaskList.IsStartOfBlock())
          flag = this.GetTriggerTaskListThatShouldCompleteHistoryEntry().WillBlockCompleteHistoryEntry();
        else if (blockStart.ShowInHistory)
        {
          if (sourceEntity1.HasTag(GAME_TAG.HISTORY_PROXY))
          {
            Entity entity = GameState.Get().GetEntity(sourceEntity1.GetTag(GAME_TAG.HISTORY_PROXY));
            HistoryManager.Get().CreatePlayedTile(entity, (Entity) null);
            if (sourceEntity1.GetController() != GameState.Get().GetFriendlySidePlayer() || !sourceEntity1.HasTag(GAME_TAG.HISTORY_PROXY_NO_BIG_CARD))
            {
              this.SetHistoryBlockingTaskList();
              HistoryManager.Get().CreateTriggeredBigCard(entity, new HistoryManager.BigCardStartedCallback(this.OnBigCardStarted), new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), false, false);
            }
          }
          else
          {
            if (this.ShouldShowTriggeredBigCard(sourceEntity1))
            {
              this.SetHistoryBlockingTaskList();
              HistoryManager.Get().CreateTriggeredBigCard(sourceEntity1, new HistoryManager.BigCardStartedCallback(this.OnBigCardStarted), new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), false, false);
            }
            HistoryManager.Get().CreateTriggerTile(sourceEntity1);
          }
          this.GetTriggerTaskListThatShouldCompleteHistoryEntry().SetWillCompleteHistoryEntry(true);
          flag = true;
        }
        else if ((blockStart.TriggerKeyword == 685 || blockStart.TriggerKeyword == 923 || blockStart.TriggerKeyword == 363 || blockStart.TriggerKeyword == 1944 || blockStart.TriggerKeyword == 1675 || blockStart.TriggerKeyword == 1920) && HistoryManager.Get().HasHistoryEntry())
          flag = true;
        else if (this.ContainsBurnedCard(this.m_currentTaskList))
        {
          if (this.m_currentTaskList.IsStartOfBlock())
          {
            HistoryManager.Get().CreateBurnedCardsTile();
            this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
          }
          this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        }
        else if (this.ContainsPoisonousEffect(this.m_currentTaskList) || this.ContainsCriticalHitEffect(this.m_currentTaskList))
          flag = true;
        if (!flag)
          break;
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
      case HistoryBlock.Type.PLAY:
        Entity sourceEntity2 = this.m_currentTaskList.GetSourceEntity(false);
        if (sourceEntity2 == null)
          break;
        if (this.m_currentTaskList.IsStartOfBlock())
        {
          if (this.m_currentTaskList.ShouldCreatePlayBlockHistoryTile())
          {
            Entity entity = GameState.Get().GetEntity(blockStart.Target);
            HistoryManager.Get().CreatePlayedTile(sourceEntity2, entity);
            this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
          }
          if (this.ShouldShowPlayedBigCard(sourceEntity2, blockStart))
          {
            bool countered = this.m_currentTaskList.WasThePlayedSpellCountered(sourceEntity2);
            this.SetHistoryBlockingTaskList();
            HistoryManager.Get().CreatePlayedBigCard(sourceEntity2, new HistoryManager.BigCardStartedCallback(this.OnBigCardStarted), new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), false, countered, 0);
          }
        }
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
      case HistoryBlock.Type.FATIGUE:
        if (this.m_currentTaskList.IsStartOfBlock())
        {
          HistoryManager.Get().CreateFatigueTile();
          this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
        }
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
      case HistoryBlock.Type.REVEAL_CARD:
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
      case HistoryBlock.Type.TRADE:
        Entity sourceEntity3 = this.m_currentTaskList.GetSourceEntity(false);
        if (sourceEntity3 == null)
          break;
        if (this.m_currentTaskList.IsStartOfBlock())
        {
          Entity entity = GameState.Get().GetEntity(blockStart.Target);
          HistoryManager.Get().CreatePlayedTile(sourceEntity3, entity);
          this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
          if (this.ShouldShowPlayedBigCard(sourceEntity3, blockStart))
          {
            this.SetHistoryBlockingTaskList();
            HistoryManager.Get().CreatePlayedBigCard(sourceEntity3, new HistoryManager.BigCardStartedCallback(this.OnBigCardStarted), new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), false, false, 0);
          }
        }
        this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
        break;
    }
  }

  private void OnBigCardStarted() => this.m_historyBlocking = true;

  private void OnBigCardFinished() => this.m_historyBlocking = false;

  private bool ShouldShowPlayedBigCard(Entity sourceEntity, Network.HistBlockStart blockStart) => GameState.Get().GetBooleanGameOption(GameEntityOption.USES_BIG_CARDS) && (!InputManager.Get().PermitDecisionMakingInput() || sourceEntity.IsControlledByOpposingSidePlayer() || blockStart.ForceShowBigCard || sourceEntity.IsLettuceAbility());

  private bool ShouldShowTriggeredBigCard(Entity sourceEntity) => sourceEntity.GetZone() == TAG_ZONE.HAND && !sourceEntity.IsHidden() && sourceEntity.HasTriggerVisual();

  private PowerTaskList GetTriggerTaskListThatShouldCompleteHistoryEntry()
  {
    if (this.m_currentTaskList.GetBlockType() != HistoryBlock.Type.TRIGGER)
      return (PowerTaskList) null;
    PowerTaskList parent = this.m_currentTaskList.GetParent();
    return parent != null && parent.GetBlockType() == HistoryBlock.Type.RITUAL ? parent : this.m_currentTaskList.GetOrigin();
  }

  private bool CanEarlyConcede()
  {
    if (!GameState.Get().IsGameCreated())
      return false;
    if (this.m_earlyConcedeTaskList != null)
      return true;
    if (GameState.Get().IsGameOver() || !GameState.Get().WasConcedeRequested())
      return false;
    Network.HistTagChange gameOverTagChange = GameState.Get().GetRealTimeGameOverTagChange();
    return gameOverTagChange != null && gameOverTagChange.Value != 4;
  }

  private void DoEarlyConcedeVisuals()
  {
    if (GameUtils.IsWaitingForOpponentReconnect())
      return;
    GameState.Get().GetFriendlySidePlayer()?.PlayConcedeEmote();
  }

  private void CancelSpellsForEarlyConcede(PowerTaskList taskList)
  {
    List<Entity> sourceEntities = taskList.GetSourceEntities();
    if (sourceEntities == null)
      return;
    foreach (Entity entity in sourceEntities)
    {
      if (entity != null)
      {
        Card card = entity.GetCard();
        if ((bool) (UnityEngine.Object) card && taskList.GetBlockStart().BlockType == HistoryBlock.Type.POWER)
        {
          Spell playSpell = card.GetPlaySpell(0);
          if ((bool) (UnityEngine.Object) playSpell)
          {
            switch (playSpell.GetActiveState())
            {
              case SpellStateType.NONE:
              case SpellStateType.CANCEL:
                continue;
              default:
                playSpell.ActivateState(SpellStateType.CANCEL);
                continue;
            }
          }
        }
      }
    }
  }

  private void StartCurrentTaskList()
  {
    this.m_currentTaskList.SetProcessStartTime();
    GameState state = GameState.Get();
    if (!this.m_currentTaskList.IsSubSpellTaskList())
    {
      Network.HistBlockStart blockStart = this.m_currentTaskList.GetBlockStart();
      if (blockStart == null)
      {
        this.DoCurrentTaskList();
        return;
      }
      int entityId = blockStart.Entities.Count == 0 ? 0 : blockStart.Entities[0];
      if (this.m_currentTaskList.GetSourceEntities() == null || this.m_currentTaskList.GetSourceEntity() == null)
      {
        if (!state.EntityRemovedFromGame(entityId))
          Debug.LogErrorFormat("PowerProcessor.StartCurrentTaskList() - WARNING got a power with a null source entity (ID={0})", (object) entityId);
        this.DoCurrentTaskList();
        return;
      }
    }
    if (this.DoTaskListWithSpellController(state, this.m_currentTaskList, this.m_currentTaskList.GetSourceEntity()))
      return;
    this.DoCurrentTaskList();
  }

  private void DoCurrentTaskList() => this.m_currentTaskList.DoAllTasks((PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => this.EndCurrentTaskList()));

  private void EndCurrentTaskList()
  {
    Log.Power.Print("PowerProcessor.EndCurrentTaskList() - m_currentTaskList={0}", this.m_currentTaskList == null ? (object) "null" : (object) this.m_currentTaskList.GetId().ToString());
    if (this.m_currentTaskList == null)
    {
      GameState.Get().OnTaskListEnded((PowerTaskList) null);
    }
    else
    {
      if (this.m_currentTaskList.GetBlockEnd() != null)
      {
        if (this.m_currentTaskList.GetOrigin() == this.m_historyBlockingTaskList && this.m_currentTaskList.GetNext() == null)
          this.m_historyBlockingTaskList = (PowerTaskList) null;
        if (this.m_currentTaskList.IsRitualBlock() && HistoryManager.Get().HasHistoryEntry())
          this.AddCthunToHistory();
        Entity sourceEntity = this.m_currentTaskList.GetSourceEntity();
        if (sourceEntity != null && sourceEntity.IsTwinspell())
          this.CleanupTwinspellEffects(sourceEntity);
        if (this.m_currentTaskList.WillBlockCompleteHistoryEntry())
          HistoryManager.Get().MarkCurrentHistoryEntryAsCompleted();
      }
      GameState.Get().OnTaskListEnded(this.m_currentTaskList);
      this.m_previousTaskList = this.m_currentTaskList;
      this.m_currentTaskList = (PowerTaskList) null;
    }
  }

  private void AddCthunToHistory()
  {
    Entity ritualEntityClone = this.m_currentTaskList.GetOrigin().GetRitualEntityClone();
    if (ritualEntityClone == null)
      return;
    Entity sourceEntity = this.m_currentTaskList.GetSourceEntity();
    if (sourceEntity.HasTag(GAME_TAG.PIECE_OF_CTHUN))
    {
      HistoryManager.Get().NotifyEntityAffected(ritualEntityClone, true, false);
    }
    else
    {
      int tag = sourceEntity.GetController().GetTag(GAME_TAG.PROXY_CTHUN);
      Entity entity = GameState.Get().GetEntity(tag);
      if (entity == null || entity.GetTag(GAME_TAG.ATK) == ritualEntityClone.GetTag(GAME_TAG.ATK) && entity.GetTag(GAME_TAG.HEALTH) == ritualEntityClone.GetTag(GAME_TAG.HEALTH) && entity.GetTag(GAME_TAG.TAUNT) == ritualEntityClone.GetTag(GAME_TAG.TAUNT))
        return;
      HistoryManager.Get().NotifyEntityAffected(entity, true, false);
    }
  }

  private void CleanupTwinspellEffects(Entity twinspellEntity)
  {
    if (!InputManager.Get().GetFriendlyHand().IsTwinspellBeingPlayed(twinspellEntity))
      return;
    InputManager.Get().GetFriendlyHand().ActivateTwinspellSpellDeath();
    InputManager.Get().GetFriendlyHand().ClearReservedCard();
  }

  public bool PerformTaskListOnCurrentGameState(PowerTaskList taskList) => this.DoTaskListWithSpellController(GameState.Get(), taskList, (Entity) null);

  private bool DoTaskListWithSpellController(
    GameState state,
    PowerTaskList taskList,
    Entity sourceEntity)
  {
    HistoryBlock.Type blockType = taskList.GetBlockType();
    Network.HistBlockStart blockStart = taskList.GetBlockStart();
    if (taskList.IsSubSpellTaskList())
      return this.DoSubSpellTaskListWithController(taskList);
    switch (blockType)
    {
      case HistoryBlock.Type.ATTACK:
        AttackSpellController attackSpellController = this.CreateAttackSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) attackSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) attackSpellController);
        return false;
      case HistoryBlock.Type.JOUST:
        JoustSpellController joustSpellController = this.CreateJoustSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) joustSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) joustSpellController);
        return false;
      case HistoryBlock.Type.POWER:
        PowerSpellController powerSpellController = this.CreatePowerSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) powerSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) powerSpellController);
        return false;
      case HistoryBlock.Type.TRIGGER:
        if (sourceEntity != null && sourceEntity.IsSecret())
        {
          SecretSpellController secretSpellController = this.CreateSecretSpellController(taskList);
          if (!this.DoTaskListUsingController((SpellController) secretSpellController, taskList))
          {
            this.DestroySpellController((SpellController) secretSpellController);
            return false;
          }
        }
        else if (blockStart != null && blockStart.TriggerKeyword == 1192)
        {
          SideQuestSpellController questSpellController = this.CreateSideQuestSpellController(taskList);
          if (!this.DoTaskListUsingController((SpellController) questSpellController, taskList))
          {
            this.DestroySpellController((SpellController) questSpellController);
            return false;
          }
        }
        else if (blockStart != null && blockStart.TriggerKeyword == 1749)
        {
          SigilSpellController sigilSpellController = this.CreateSigilSpellController(taskList);
          if (!this.DoTaskListUsingController((SpellController) sigilSpellController, taskList))
          {
            this.DestroySpellController((SpellController) sigilSpellController);
            return false;
          }
        }
        else if (blockStart != null && blockStart.TriggerKeyword == 2311)
        {
          ObjectiveSpellController objectiveSpellController = this.CreateObjectiveSpellController(taskList);
          if (!this.DoTaskListUsingController((SpellController) objectiveSpellController, taskList))
          {
            this.DestroySpellController((SpellController) objectiveSpellController);
            return false;
          }
        }
        else
        {
          TriggerSpellController triggerSpellController = this.CreateTriggerSpellController(taskList);
          Card card = sourceEntity?.GetCard();
          Card drawMetaDataCard = taskList.GetStartDrawMetaDataCard();
          if (TurnStartManager.Get().IsCardDrawHandled(card) || TurnStartManager.Get().IsCardDrawHandled(drawMetaDataCard))
          {
            if (!triggerSpellController.AttachPowerTaskList(taskList))
            {
              Log.Power.PrintWarning("TurnStartManager failed to handle a trigger. sourceCard:{0}, metadataCard:{1}, taskList:{2}", (object) card, (object) drawMetaDataCard, (object) taskList);
              this.DestroySpellController((SpellController) triggerSpellController);
              return false;
            }
            triggerSpellController.AddFinishedTaskListCallback(new SpellController.FinishedTaskListCallback(this.OnSpellControllerFinishedTaskList));
            triggerSpellController.AddFinishedCallback(new SpellController.FinishedCallback(this.OnSpellControllerFinished));
            TurnStartManager.Get().NotifyOfSpellController((SpellController) triggerSpellController);
          }
          else if (!this.DoTaskListUsingController((SpellController) triggerSpellController, taskList))
          {
            this.DestroySpellController((SpellController) triggerSpellController);
            return false;
          }
        }
        return true;
      case HistoryBlock.Type.DEATHS:
        DeathSpellController deathSpellController = this.CreateDeathSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) deathSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) deathSpellController);
        return false;
      case HistoryBlock.Type.PLAY:
        this.CheckDeactivatePlaySpellForSpellPlayBlock(taskList);
        this.CheckDeactivatePlaySpellForTransformation(taskList);
        this.TriggerLettuceSpeedTileVisual(taskList);
        break;
      case HistoryBlock.Type.FATIGUE:
        FatigueSpellController fatigueSpellController = this.CreateFatigueSpellController(taskList);
        if (!fatigueSpellController.AttachPowerTaskList(taskList))
        {
          this.DestroySpellController((SpellController) fatigueSpellController);
          return false;
        }
        fatigueSpellController.AddFinishedTaskListCallback(new SpellController.FinishedTaskListCallback(this.OnSpellControllerFinishedTaskList));
        fatigueSpellController.AddFinishedCallback(new SpellController.FinishedCallback(this.OnSpellControllerFinished));
        if (state.IsTurnStartManagerActive())
          TurnStartManager.Get().NotifyOfSpellController((SpellController) fatigueSpellController);
        else
          fatigueSpellController.DoPowerTaskList();
        return true;
      case HistoryBlock.Type.RITUAL:
        RitualSpellController ritualSpellController = this.CreateRitualSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) ritualSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) ritualSpellController);
        return false;
      case HistoryBlock.Type.REVEAL_CARD:
        RevealCardSpellController cardSpellController = this.CreateRevealCardSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) cardSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) cardSpellController);
        return false;
      case HistoryBlock.Type.GAME_RESET:
        ResetGameSpellController gameSpellController = this.CreateResetGameSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) gameSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) gameSpellController);
        return false;
      case HistoryBlock.Type.MOVE_MINION:
        MoveMinionSpellController minionSpellController = this.CreateMoveMinionSpellController(taskList);
        if (this.DoTaskListUsingController((SpellController) minionSpellController, taskList))
          return true;
        this.DestroySpellController((SpellController) minionSpellController);
        return false;
    }
    Log.Power.Print("PowerProcessor.DoTaskListForCard() - unhandled BlockType {0} for sourceEntity {1}", (object) blockType, (object) sourceEntity);
    return false;
  }

  private void TriggerLettuceSpeedTileVisual(PowerTaskList taskList)
  {
    if (!taskList.IsStartOfBlock())
      return;
    Card card = taskList?.GetSourceEntity()?.GetLettuceAbilityOwner()?.GetCard();
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return;
    card.ActivateActorSpell(SpellType.MERCENARIES_COMBAT_BOOSH);
    card.ActivateActorSpell(SpellType.MERCENARIES_HIGHLIGHT_ACTING_MINION);
    if (!((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null))
      return;
    this.WaitForLettuceAbilityBigCardThenContinuePowerProcessing(card, Gameplay.Get().LettuceAbilityToken).Forget();
  }

  private async UniTaskVoid WaitForLettuceAbilityBigCardThenContinuePowerProcessing(
    Card actingMerc,
    CancellationToken token)
  {
    UniTask uniTask;
    while (!HistoryManager.Get().HasBigCard())
    {
      uniTask = UniTask.Yield(PlayerLoopTiming.Update, token);
      await uniTask;
    }
    HistoryManager.Get().NotifyOfLettuceSpeedTileSpellFinished();
    LayerUtils.SetLayer(HistoryManager.Get().GetCurrentBigCard().m_mainCardActor.gameObject, GameLayer.IgnoreFullScreenEffects);
    GameState.Get().SetBusy(false);
    uniTask = this.DisableMercenaryHighlightAfterBigCardFinishes(actingMerc, token);
    await uniTask;
  }

  private async UniTask DisableMercenaryHighlightAfterBigCardFinishes(
    Card activeMercenary,
    CancellationToken token)
  {
    if ((UnityEngine.Object) activeMercenary == (UnityEngine.Object) null)
      return;
    while (HistoryManager.Get().HasBigCard())
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    Spell actorSpell = activeMercenary.GetActorSpell(SpellType.MERCENARIES_HIGHLIGHT_ACTING_MINION, false);
    if (!((UnityEngine.Object) actorSpell != (UnityEngine.Object) null))
      return;
    actorSpell.ActivateState(SpellStateType.DEATH);
  }

  private void CheckDeactivatePlaySpellForSpellPlayBlock(PowerTaskList taskList)
  {
    if (taskList.GetOrigin() != taskList)
      return;
    PowerTaskList powerTaskList = this.GetPowerQueue().Count > 0 ? this.GetPowerQueue().Peek() : (PowerTaskList) null;
    if (powerTaskList != null && powerTaskList.GetParent() == taskList)
      return;
    Entity sourceEntity = taskList.GetSourceEntity();
    if (sourceEntity == null || sourceEntity.GetCardType() != TAG_CARDTYPE.SPELL)
      return;
    Card card = sourceEntity.GetCard();
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return;
    card.DeactivatePlaySpell();
  }

  private void CheckDeactivatePlaySpellForTransformation(PowerTaskList taskList)
  {
    if (taskList.GetBlockEnd() == null)
      return;
    PowerTaskList powerTaskList = this.GetPowerQueue().Count > 0 ? this.GetPowerQueue().Peek() : (PowerTaskList) null;
    if (powerTaskList != null && powerTaskList.GetParent() == taskList)
      return;
    Entity sourceEntity = taskList.GetSourceEntity();
    if (sourceEntity == null || !sourceEntity.HasTag(GAME_TAG.TRANSFORMED_FROM_CARD) || sourceEntity.GetCardType() != TAG_CARDTYPE.SPELL)
      return;
    Card card = sourceEntity.GetCard();
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return;
    card.DeactivatePlaySpell();
  }

  private bool DoSubSpellTaskListWithController(PowerTaskList taskList)
  {
    if ((UnityEngine.Object) this.m_subSpellController == (UnityEngine.Object) null)
      this.m_subSpellController = this.CreateSpellController<SubSpellController>(prefabPath: "SubSpellController.prefab:34966ff41154fce469d3ccb6d3b1655e");
    if (!this.m_subSpellController.AttachPowerTaskList(taskList))
      return false;
    this.m_subSpellController.AddFinishedTaskListCallback(new SpellController.FinishedTaskListCallback(this.OnSpellControllerFinishedTaskList));
    this.m_subSpellController.DoPowerTaskList();
    return true;
  }

  private bool DoTaskListUsingController(SpellController spellController, PowerTaskList taskList)
  {
    if ((UnityEngine.Object) spellController == (UnityEngine.Object) null)
    {
      Log.Power.Print("PowerProcessor.DoTaskListUsingController() - spellController=null");
      return false;
    }
    if (!spellController.AttachPowerTaskList(taskList))
      return false;
    spellController.AddFinishedTaskListCallback(new SpellController.FinishedTaskListCallback(this.OnSpellControllerFinishedTaskList));
    spellController.AddFinishedCallback(new SpellController.FinishedCallback(this.OnSpellControllerFinished));
    spellController.DoPowerTaskList();
    return true;
  }

  private void OnSpellControllerFinishedTaskList(SpellController spellController)
  {
    spellController.DetachPowerTaskList();
    if (this.m_currentTaskList == null)
      return;
    this.DoCurrentTaskList();
  }

  private void OnSpellControllerFinished(SpellController spellController) => this.DestroySpellController(spellController);

  private AttackSpellController CreateAttackSpellController(
    PowerTaskList taskList)
  {
    string prefabPath = "AttackSpellController.prefab:12acecc85ac575e43b87ec141b89269a";
    if (GameState.Get() != null && GameState.Get().GetGameEntity() != null)
    {
      string controllerOverride = GameState.Get().GetGameEntity().GetAttackSpellControllerOverride(taskList.GetAttacker());
      if (!string.IsNullOrEmpty(controllerOverride))
        prefabPath = controllerOverride;
    }
    return this.CreateSpellController<AttackSpellController>(taskList, prefabPath);
  }

  private MoveMinionSpellController CreateMoveMinionSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<MoveMinionSpellController>(taskList);
  }

  private SecretSpellController CreateSecretSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<SecretSpellController>(taskList, "SecretSpellController.prefab:553af99c12154c547bc05dc3d9832931");
  }

  private SigilSpellController CreateSigilSpellController(PowerTaskList taskList) => this.CreateSpellController<SigilSpellController>(taskList, "SigilSpellController.prefab:1f80634fbf70a654bbae7bf796bf11b2");

  private ObjectiveSpellController CreateObjectiveSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<ObjectiveSpellController>(taskList, "ObjectiveSpellController.prefab:a3d627bc67f24e740a2e967b383ecc6e");
  }

  private SideQuestSpellController CreateSideQuestSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<SideQuestSpellController>(taskList, "SideQuestSpellController.prefab:63762d08481f04642bbf3cde299feea2");
  }

  private PowerSpellController CreatePowerSpellController(PowerTaskList taskList) => this.CreateSpellController<PowerSpellController>(taskList);

  private TriggerSpellController CreateTriggerSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<TriggerSpellController>(taskList, "TriggerSpellController.prefab:e0a2661f98a720d47ad4b85de228f4b4");
  }

  private DeathSpellController CreateDeathSpellController(PowerTaskList taskList) => this.CreateSpellController<DeathSpellController>(taskList);

  private FatigueSpellController CreateFatigueSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<FatigueSpellController>(taskList);
  }

  private JoustSpellController CreateJoustSpellController(PowerTaskList taskList) => this.CreateSpellController<JoustSpellController>(taskList, "JoustSpellController.prefab:89ac256005a4a8a46939a84460c2c221");

  private RitualSpellController CreateRitualSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<RitualSpellController>(taskList, "RitualSpellController.prefab:27c7bd4ffaa54fb4e9e64dad14a6e701");
  }

  private RevealCardSpellController CreateRevealCardSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<RevealCardSpellController>(taskList, "RevealCardSpellController.prefab:17fd7ea79bfd4c24389d535a074199b6");
  }

  private ResetGameSpellController CreateResetGameSpellController(
    PowerTaskList taskList)
  {
    return this.CreateSpellController<ResetGameSpellController>(taskList, "ResetGameSpellController.prefab:d8c1994d523574e42bffa17990917754");
  }

  private T CreateSpellController<T>(PowerTaskList taskList = null, string prefabPath = null) where T : SpellController
  {
    GameObject gameObject;
    T spellController;
    if (prefabPath == null)
    {
      gameObject = new GameObject();
      spellController = gameObject.AddComponent<T>();
    }
    else
    {
      gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) prefabPath);
      spellController = gameObject.GetComponent<T>();
    }
    if (taskList != null)
      gameObject.name = string.Format("{0} [taskListId={1}]", (object) typeof (T), (object) taskList.GetId());
    else
      gameObject.name = string.Format("{0}", (object) typeof (T));
    return spellController;
  }

  private void DestroySpellController(SpellController spellController) => UnityEngine.Object.Destroy((UnityEngine.Object) spellController.gameObject);

  public delegate void OnTaskEvent(float scheduleDiff);

  private class DelayedRealTimeTask
  {
    public PowerTask m_powerTask;
    public List<Network.PowerHistory> m_powerHistory;
    public int m_index;
  }
}
