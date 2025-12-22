using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
  public const float FINISH_FUDGE_SEC = 10f;
  private static readonly PlatformDependentValue<bool> ALLOW_LOST_FRAME_TIME_CATCH_UP = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    PC = false,
    Mac = false,
    iOS = true,
    Android = true
  };
  private List<SpellController.FinishedTaskListCallback> m_finishedTaskListListeners = new List<SpellController.FinishedTaskListCallback>();
  private List<SpellController.FinishedCallback> m_finishedListeners = new List<SpellController.FinishedCallback>();
  protected List<Card> m_sources = new List<Card>();
  protected List<Card> m_targets = new List<Card>();
  protected PowerTaskList m_taskList;
  protected int m_taskListId;
  protected bool m_processingTaskList;
  protected bool m_pendingFinish;

  public Card GetSource() => this.m_sources == null || this.m_sources.Count <= 0 ? (Card) null : this.m_sources[0];

  public List<Card> GetSources() => this.m_sources;

  public void SetSource(Card card)
  {
    this.m_sources.Clear();
    this.m_sources.Add(card);
  }

  public void SetSource(List<Card> cards)
  {
    this.m_sources.Clear();
    this.m_sources.AddRange((IEnumerable<Card>) cards);
  }

  public bool IsSource(Card card) => this.m_sources.Contains(card);

  public void RemoveSource() => this.m_sources.Clear();

  public List<Card> GetTargets() => this.m_targets;

  public Card GetTarget() => this.m_targets.Count != 0 ? this.m_targets[0] : (Card) null;

  public void AddTarget(Card card) => this.m_targets.Add(card);

  public void RemoveTarget(Card card) => this.m_targets.Remove(card);

  public void RemoveAllTargets() => this.m_targets.Clear();

  public bool IsTarget(Card card) => this.m_targets.Contains(card);

  public void AddFinishedTaskListCallback(SpellController.FinishedTaskListCallback callback)
  {
    if (this.m_finishedTaskListListeners.Contains(callback))
      return;
    this.m_finishedTaskListListeners.Add(callback);
  }

  public void AddFinishedCallback(SpellController.FinishedCallback callback)
  {
    if (this.m_finishedListeners.Contains(callback))
      return;
    this.m_finishedListeners.Add(callback);
  }

  public bool IsProcessingTaskList() => this.m_processingTaskList;

  public PowerTaskList GetPowerTaskList() => this.m_taskList;

  public bool AttachPowerTaskList(PowerTaskList taskList)
  {
    if (this.m_taskList != taskList)
    {
      this.DetachPowerTaskList();
      this.m_taskList = taskList;
    }
    this.m_taskListId = this.m_taskList.GetId();
    return this.AddPowerSourceAndTargets(taskList);
  }

  public void SetPowerTaskList(PowerTaskList taskList)
  {
    if (this.m_taskList == taskList)
      return;
    this.DetachPowerTaskList();
    this.m_taskList = taskList;
  }

  public PowerTaskList DetachPowerTaskList()
  {
    PowerTaskList taskList = this.m_taskList;
    this.RemoveSource();
    this.RemoveAllTargets();
    this.m_taskList = (PowerTaskList) null;
    return taskList;
  }

  public void DoPowerTaskList()
  {
    this.m_processingTaskList = true;
    if (this.IsLostFrameTimeCatchUpEnabled())
    {
      float catchUpThreshold = GameState.Get().GetClientLostTimeCatchUpThreshold();
      float timeCatchUpSeconds = this.GetLostFrameTimeCatchUpSeconds();
      if ((double) timeCatchUpSeconds > 0.0 && (double) catchUpThreshold > 0.0 && (double) GameState.Get().GetTimeTracker().GetAccruedLostTimeInSeconds() > (double) Math.Max(timeCatchUpSeconds, catchUpThreshold))
      {
        if (GameState.Get().GetTimeTracker() is GameStateFrameTimeTracker)
          GameState.Get().GetTimeTracker().AdjustAccruedLostTime(-timeCatchUpSeconds);
        this.OnFinishedTaskList();
        this.OnFinished();
        return;
      }
    }
    this.gameObject.SetActive(true);
    GameState.Get().AddServerBlockingSpellController(this);
    this.StartCoroutine(this.WaitForCardsThenDoTaskList());
  }

  public void ForceKill() => this.OnFinishedTaskList();

  public virtual bool ShouldReconnectIfStuck() => true;

  protected virtual void OnProcessTaskList()
  {
    this.OnFinishedTaskList();
    this.OnFinished();
  }

  protected virtual void OnFinishedTaskList()
  {
    if (GameState.Get() != null)
      GameState.Get().RemoveServerBlockingSpellController(this);
    this.m_processingTaskList = false;
    this.FireFinishedTaskListCallbacks();
    if (!this.m_pendingFinish)
      return;
    this.m_pendingFinish = false;
    this.OnFinished();
  }

  protected virtual void OnFinished()
  {
    if (this.m_processingTaskList)
    {
      this.m_pendingFinish = true;
    }
    else
    {
      this.gameObject.SetActive(false);
      this.FireFinishedCallbacks();
    }
  }

  protected virtual bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    if (!this.HasSourceCard(taskList) || !SpellUtils.CanAddPowerTargets(taskList))
      return false;
    List<Entity> sourceEntities = taskList.GetSourceEntities();
    List<Card> cards = new List<Card>();
    foreach (Entity entity in sourceEntities)
    {
      if (entity != null)
        cards.Add(entity.GetCard());
    }
    this.SetSource(cards);
    List<PowerTask> taskList1 = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      Card cardFromPowerTask = this.GetTargetCardFromPowerTask(taskList1[index]);
      if (!((UnityEngine.Object) cardFromPowerTask == (UnityEngine.Object) null) && !cards.Contains(cardFromPowerTask) && !this.IsTarget(cardFromPowerTask))
        this.AddTarget(cardFromPowerTask);
    }
    return cards.Count > 0 && !cards.Exists((Predicate<Card>) (c => (UnityEngine.Object) c == (UnityEngine.Object) null)) || this.m_targets.Count > 0;
  }

  protected virtual bool HasSourceCard(PowerTaskList taskList)
  {
    List<Entity> sourceEntities = taskList.GetSourceEntities();
    if (sourceEntities == null || sourceEntities.Count == 0)
      return false;
    List<Card> cardList = new List<Card>();
    foreach (Entity entity in sourceEntities)
    {
      if (entity != null)
        cardList.Add(entity.GetCard());
    }
    return cardList != null && cardList.Count != 0 && !cardList.Exists((Predicate<Card>) (c => (UnityEngine.Object) c == (UnityEngine.Object) null));
  }

  protected virtual float GetLostFrameTimeCatchUpSeconds() => 0.0f;

  private IEnumerator WaitForCardsThenDoTaskList()
  {
    Card sourceCard = this.GetSource();
    if ((UnityEngine.Object) sourceCard != (UnityEngine.Object) null)
    {
      while (this.IsCardBusy(sourceCard))
        yield return (object) null;
    }
    foreach (Card target in this.m_targets)
    {
      Card targetCard = target;
      if (!((UnityEngine.Object) targetCard == (UnityEngine.Object) null))
      {
        while (this.IsCardBusy(targetCard))
          yield return (object) null;
        targetCard = (Card) null;
      }
    }
    this.OnProcessTaskList();
  }

  protected bool IsLostFrameTimeCatchUpEnabled() => (bool) SpellController.ALLOW_LOST_FRAME_TIME_CATCH_UP && GameState.Get() != null && GameState.Get().GetGameEntity() != null && GameState.Get().AreLostTimeGuardianConditionsMet() && GameState.Get().GetGameEntity().IsGameSpeedupConditionInEffect();

  protected bool IsCardBusy(Card card)
  {
    Entity entity = card.GetEntity();
    return !this.WillEntityLoadCard(entity) && (entity.IsLoadingAssets() || (!(bool) (UnityEngine.Object) TurnStartManager.Get() || !TurnStartManager.Get().IsCardDrawHandled(card)) && !card.IsActorReady());
  }

  private bool WillEntityLoadCard(Entity entity)
  {
    int entityId = entity.GetEntityId();
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      switch (power.Type)
      {
        case Network.PowerType.FULL_ENTITY:
          Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
          if (entityId == histFullEntity.Entity.ID)
            return true;
          continue;
        case Network.PowerType.SHOW_ENTITY:
          Network.HistShowEntity histShowEntity = power as Network.HistShowEntity;
          if (entityId == histShowEntity.Entity.ID)
            return true;
          continue;
        default:
          continue;
      }
    }
    return false;
  }

  private void FireFinishedTaskListCallbacks()
  {
    SpellController.FinishedTaskListCallback[] array = this.m_finishedTaskListListeners.ToArray();
    this.m_finishedTaskListListeners.Clear();
    for (int index = 0; index < array.Length; ++index)
      array[index](this);
  }

  private void FireFinishedCallbacks()
  {
    SpellController.FinishedCallback[] array = this.m_finishedListeners.ToArray();
    this.m_finishedListeners.Clear();
    for (int index = 0; index < array.Length; ++index)
      array[index](this);
  }

  protected Card GetTargetCardFromPowerTask(PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    if (power.Type != Network.PowerType.TAG_CHANGE)
      return (Card) null;
    Network.HistTagChange histTagChange = power as Network.HistTagChange;
    Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
    if (entity != null)
      return entity.GetCard();
    Debug.LogWarning((object) string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity));
    return (Card) null;
  }

  public delegate void FinishedTaskListCallback(SpellController spellController);

  public delegate void FinishedCallback(SpellController spellController);
}
