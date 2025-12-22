using System.Collections.Generic;
using UnityEngine;

public class BoardEvents : MonoBehaviour
{
  private const float AI_PROCESS_INTERVAL = 3.5f;
  private const float PROCESS_INTERVAL = 1.25f;
  private const float FAST_PROCESS_INTERVAL = 0.15f;
  private float m_nextProcessTime;
  private float m_nextFastProcessTime;
  private LinkedList<BoardEvents.EventData> m_events = new LinkedList<BoardEvents.EventData>();
  private LinkedList<BoardEvents.EventData> m_fastEvents = new LinkedList<BoardEvents.EventData>();
  private List<LinkedListNode<BoardEvents.EventData>> m_removeEvents = new List<LinkedListNode<BoardEvents.EventData>>();
  private Dictionary<BoardEvents.EVENT_TYPE, float> m_weights = new Dictionary<BoardEvents.EVENT_TYPE, float>();
  private List<BoardEvents.LargeShakeEventDelegate> m_largeShakeEventCallbacks;
  private List<BoardEvents.EventCallback> m_friendlyHeroDamageCallacks;
  private List<BoardEvents.EventCallback> m_opponentHeroDamageCallacks;
  private List<BoardEvents.EventCallback> m_opponentMinionDamageCallacks;
  private List<BoardEvents.EventCallback> m_friendlyMinionDamageCallacks;
  private List<BoardEvents.EventCallback> m_friendlyHeroHealCallbacks;
  private List<BoardEvents.EventCallback> m_opponentHeroHealCallbacks;
  private List<BoardEvents.EventCallback> m_friendlyMinionHealCallbacks;
  private List<BoardEvents.EventCallback> m_opponentMinionHealCallbacks;
  private List<BoardEvents.EventCallback> m_frindlyLegendaryMinionSpawnCallbacks;
  private List<BoardEvents.EventCallback> m_opponentLegendaryMinionSpawnCallbacks;
  private List<BoardEvents.EventCallback> m_frindlyMinionSpawnCallbacks;
  private List<BoardEvents.EventCallback> m_opponentMinionSpawnCallbacks;
  private List<BoardEvents.EventCallback> m_frindlyLegendaryMinionDeathCallbacks;
  private List<BoardEvents.EventCallback> m_opponentLegendaryMinionDeathCallbacks;
  private List<BoardEvents.EventCallback> m_frindlyMinionDeathCallbacks;
  private List<BoardEvents.EventCallback> m_opponentMinionDeathCallbacks;
  private static BoardEvents s_instance;

  private void Awake() => BoardEvents.s_instance = this;

  private void Update()
  {
    if ((double) Time.timeSinceLevelLoad > (double) this.m_nextFastProcessTime)
    {
      this.m_nextFastProcessTime = Time.timeSinceLevelLoad + 0.15f;
      this.ProcessImmediateEvents();
    }
    else
    {
      if ((double) Time.timeSinceLevelLoad <= (double) this.m_nextProcessTime)
        return;
      this.m_nextProcessTime = Time.timeSinceLevelLoad + 1.25f;
      this.ProcessEvents();
    }
  }

  private void OnDestroy() => BoardEvents.s_instance = (BoardEvents) null;

  public static BoardEvents Get()
  {
    if ((Object) BoardEvents.s_instance == (Object) null)
    {
      Board board = Board.Get();
      if ((Object) board == (Object) null)
        return (BoardEvents) null;
      BoardEvents.s_instance = board.gameObject.AddComponent<BoardEvents>();
    }
    return BoardEvents.s_instance;
  }

  public void RegisterLargeShakeEvent(BoardEvents.LargeShakeEventDelegate callback)
  {
    if (this.m_largeShakeEventCallbacks == null)
      this.m_largeShakeEventCallbacks = new List<BoardEvents.LargeShakeEventDelegate>();
    this.m_largeShakeEventCallbacks.Add(callback);
  }

  public void RegisterFriendlyHeroDamageEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_friendlyHeroDamageCallacks = this.RegisterEvent(this.m_friendlyHeroDamageCallacks, callback, minimumWeight);
  }

  public void RegisterOpponentHeroDamageEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_opponentHeroDamageCallacks = this.RegisterEvent(this.m_opponentHeroDamageCallacks, callback, minimumWeight);
  }

  public void RegisterFriendlyMinionDamageEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_friendlyMinionDamageCallacks = this.RegisterEvent(this.m_friendlyMinionDamageCallacks, callback, minimumWeight);
  }

  public void RegisterOpponentMinionDamageEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_opponentMinionDamageCallacks = this.RegisterEvent(this.m_opponentMinionDamageCallacks, callback, minimumWeight);
  }

  public void RegisterFriendlyHeroHealEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f) => this.m_friendlyHeroHealCallbacks = this.RegisterEvent(this.m_friendlyHeroHealCallbacks, callback, minimumWeight);

  public void RegisterOpponentHeroHealEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f) => this.m_opponentHeroHealCallbacks = this.RegisterEvent(this.m_opponentHeroHealCallbacks, callback, minimumWeight);

  public void RegisterFriendlyMinionHealEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_friendlyMinionHealCallbacks = this.RegisterEvent(this.m_friendlyMinionHealCallbacks, callback, minimumWeight);
  }

  public void RegisterOpponentMinionHealEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_opponentMinionHealCallbacks = this.RegisterEvent(this.m_opponentMinionHealCallbacks, callback, minimumWeight);
  }

  public void RegisterFriendlyLegendaryMinionSpawnEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_frindlyLegendaryMinionSpawnCallbacks = this.RegisterEvent(this.m_frindlyLegendaryMinionSpawnCallbacks, callback, minimumWeight);
  }

  public void RegisterOppenentLegendaryMinionSpawnEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_opponentLegendaryMinionSpawnCallbacks = this.RegisterEvent(this.m_opponentLegendaryMinionSpawnCallbacks, callback, minimumWeight);
  }

  public void RegisterFriendlyMinionSpawnEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_frindlyMinionSpawnCallbacks = this.RegisterEvent(this.m_frindlyMinionSpawnCallbacks, callback, minimumWeight);
  }

  public void RegisterOppenentMinionSpawnEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_opponentMinionSpawnCallbacks = this.RegisterEvent(this.m_opponentMinionSpawnCallbacks, callback, minimumWeight);
  }

  public void RegisterFriendlyLegendaryMinionDeathEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_frindlyLegendaryMinionDeathCallbacks = this.RegisterEvent(this.m_frindlyLegendaryMinionDeathCallbacks, callback, minimumWeight);
  }

  public void RegisterOppenentLegendaryMinionDeathEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_opponentLegendaryMinionDeathCallbacks = this.RegisterEvent(this.m_opponentLegendaryMinionDeathCallbacks, callback, minimumWeight);
  }

  public void RegisterFriendlyMinionDeathEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_frindlyMinionDeathCallbacks = this.RegisterEvent(this.m_frindlyMinionDeathCallbacks, callback, minimumWeight);
  }

  public void RegisterOppenentMinionDeathEvent(
    BoardEvents.EventDelegate callback,
    float minimumWeight = 1f)
  {
    this.m_opponentMinionDeathCallbacks = this.RegisterEvent(this.m_opponentMinionDeathCallbacks, callback, minimumWeight);
  }

  private List<BoardEvents.EventCallback> RegisterEvent(
    List<BoardEvents.EventCallback> eventList,
    BoardEvents.EventDelegate callback,
    float minimumWeight)
  {
    if (eventList == null)
      eventList = new List<BoardEvents.EventCallback>();
    eventList.Add(new BoardEvents.EventCallback()
    {
      callback = callback,
      minimumWeight = minimumWeight
    });
    return eventList;
  }

  public void MinionShakeEvent(ShakeMinionIntensity shakeIntensity, float customIntensity)
  {
    if (shakeIntensity != ShakeMinionIntensity.LargeShake)
      return;
    this.m_fastEvents.AddLast(new BoardEvents.EventData()
    {
      m_timeStamp = Time.timeSinceLevelLoad,
      m_eventType = BoardEvents.EVENT_TYPE.LargeMinionShake
    });
  }

  public void DamageEvent(Card targetCard, float damage)
  {
    Entity entity = targetCard.GetEntity();
    if (entity == null)
      return;
    this.m_events.AddLast(new BoardEvents.EventData()
    {
      m_card = targetCard,
      m_timeStamp = Time.timeSinceLevelLoad,
      m_eventType = !entity.IsHero() ? (targetCard.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentMinionDamage : BoardEvents.EVENT_TYPE.FriendlyMinionDamage) : (targetCard.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentHeroDamage : BoardEvents.EVENT_TYPE.FriendlyHeroDamage),
      m_value = damage,
      m_rarity = entity.GetRarity()
    });
  }

  public void HealEvent(Card targetCard, float health)
  {
    Entity entity = targetCard.GetEntity();
    if (entity == null)
      return;
    this.m_events.AddLast(new BoardEvents.EventData()
    {
      m_card = targetCard,
      m_timeStamp = Time.timeSinceLevelLoad,
      m_eventType = !entity.IsHero() ? (targetCard.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentMinionHeal : BoardEvents.EVENT_TYPE.FriendlyMinionHeal) : (targetCard.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentHeroHeal : BoardEvents.EVENT_TYPE.FriendlyHeroHeal),
      m_value = health,
      m_rarity = entity.GetRarity()
    });
  }

  public void SummonedEvent(Card minionCard)
  {
    Entity entity = minionCard.GetEntity();
    if (entity == null || !entity.IsMinion())
      return;
    this.m_events.AddLast(new BoardEvents.EventData()
    {
      m_card = minionCard,
      m_timeStamp = Time.timeSinceLevelLoad,
      m_eventType = entity.GetRarity() != TAG_RARITY.LEGENDARY ? (minionCard.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentMinionSpawn : BoardEvents.EVENT_TYPE.FriendlyMinionSpawn) : (minionCard.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentLegendaryMinionSpawn : BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionSpawn),
      m_value = (float) entity.GetDefCost(),
      m_rarity = entity.GetRarity()
    });
  }

  public void DeathEvent(Card card)
  {
    Entity entity = card.GetEntity();
    if (entity == null)
      return;
    this.m_events.AddLast(new BoardEvents.EventData()
    {
      m_card = card,
      m_timeStamp = Time.timeSinceLevelLoad,
      m_eventType = entity.GetRarity() != TAG_RARITY.LEGENDARY ? (card.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentMinionDeath : BoardEvents.EVENT_TYPE.FriendlyMinionDeath) : (card.GetControllerSide() != Player.Side.FRIENDLY ? BoardEvents.EVENT_TYPE.OpponentLegendaryMinionDeath : BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionDeath),
      m_value = (float) entity.GetDefCost(),
      m_rarity = entity.GetRarity()
    });
  }

  private void ProcessImmediateEvents()
  {
    if (this.m_fastEvents.Count == 0 || this.m_largeShakeEventCallbacks == null)
      return;
    LinkedListNode<BoardEvents.EventData> linkedListNode1 = this.m_fastEvents.First;
    while (linkedListNode1 != null)
    {
      BoardEvents.EventData eventData = linkedListNode1.Value;
      LinkedListNode<BoardEvents.EventData> linkedListNode2 = linkedListNode1;
      linkedListNode1 = linkedListNode1.Next;
      if ((double) eventData.m_timeStamp + 0.150000005960464 < (double) Time.timeSinceLevelLoad)
        this.m_removeEvents.Add(linkedListNode2);
      else if (eventData.m_eventType == BoardEvents.EVENT_TYPE.LargeMinionShake)
      {
        this.AddWeight(BoardEvents.EVENT_TYPE.LargeMinionShake, 1f);
        this.m_removeEvents.Add(linkedListNode2);
      }
    }
    for (int index = 0; index < this.m_removeEvents.Count; ++index)
    {
      LinkedListNode<BoardEvents.EventData> removeEvent = this.m_removeEvents[index];
      if (removeEvent != null)
        this.m_fastEvents.Remove(removeEvent);
    }
    this.m_removeEvents.Clear();
    if (this.m_weights.ContainsKey(BoardEvents.EVENT_TYPE.LargeMinionShake) && (double) this.m_weights[BoardEvents.EVENT_TYPE.LargeMinionShake] > 0.0)
      this.LargeShakeEvent();
    this.m_weights.Clear();
  }

  private void ProcessEvents()
  {
    if (this.m_events.Count == 0)
      return;
    LinkedListNode<BoardEvents.EventData> linkedListNode1 = this.m_events.First;
    while (linkedListNode1 != null)
    {
      BoardEvents.EventData eventData = linkedListNode1.Value;
      LinkedListNode<BoardEvents.EventData> linkedListNode2 = linkedListNode1;
      linkedListNode1 = linkedListNode1.Next;
      if ((double) eventData.m_timeStamp + 3.5 < (double) Time.timeSinceLevelLoad)
        this.m_removeEvents.Add(linkedListNode2);
      else
        this.AddWeight(eventData.m_eventType, eventData.m_value);
    }
    for (int index = 0; index < this.m_removeEvents.Count; ++index)
    {
      LinkedListNode<BoardEvents.EventData> removeEvent = this.m_removeEvents[index];
      if (removeEvent != null)
        this.m_events.Remove(removeEvent);
    }
    this.m_removeEvents.Clear();
    if (this.m_weights.Count == 0)
      return;
    BoardEvents.EVENT_TYPE? nullable = new BoardEvents.EVENT_TYPE?();
    float weight = -1f;
    foreach (BoardEvents.EVENT_TYPE key in this.m_weights.Keys)
    {
      if ((double) this.m_weights[key] >= (double) weight)
      {
        nullable = new BoardEvents.EVENT_TYPE?(key);
        weight = this.m_weights[key];
      }
    }
    if (!nullable.HasValue)
      return;
    if (nullable.HasValue)
    {
      switch (nullable.GetValueOrDefault())
      {
        case BoardEvents.EVENT_TYPE.FriendlyHeroDamage:
          this.CallbackEvent(this.m_friendlyHeroDamageCallacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentHeroDamage:
          this.CallbackEvent(this.m_opponentHeroDamageCallacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.FriendlyHeroHeal:
          this.CallbackEvent(this.m_friendlyHeroHealCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentHeroHeal:
          this.CallbackEvent(this.m_opponentHeroHealCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionSpawn:
          this.CallbackEvent(this.m_frindlyLegendaryMinionSpawnCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentLegendaryMinionSpawn:
          this.CallbackEvent(this.m_opponentLegendaryMinionSpawnCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionDeath:
          this.CallbackEvent(this.m_frindlyLegendaryMinionDeathCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentLegendaryMinionDeath:
          this.CallbackEvent(this.m_opponentLegendaryMinionDeathCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.FriendlyMinionSpawn:
          this.CallbackEvent(this.m_frindlyMinionSpawnCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentMinionSpawn:
          this.CallbackEvent(this.m_opponentMinionSpawnCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.FriendlyMinionDeath:
          this.CallbackEvent(this.m_frindlyMinionDeathCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentMinionDeath:
          this.CallbackEvent(this.m_opponentMinionDeathCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.FriendlyMinionDamage:
          this.CallbackEvent(this.m_friendlyMinionDamageCallacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentMinionDamage:
          this.CallbackEvent(this.m_opponentMinionDamageCallacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.FriendlyMinionHeal:
          this.CallbackEvent(this.m_friendlyMinionHealCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
        case BoardEvents.EVENT_TYPE.OpponentMinionHeal:
          this.CallbackEvent(this.m_opponentMinionHealCallbacks, weight);
          this.m_events.Clear();
          goto label_41;
      }
    }
    Debug.LogWarning((object) string.Format("BoardEvents: Event type unknown when processing event weights: {0}", (object) nullable));
label_41:
    this.m_weights.Clear();
  }

  private void LargeShakeEvent()
  {
    if (this.m_largeShakeEventCallbacks == null)
      return;
    for (int index = this.m_largeShakeEventCallbacks.Count - 1; index >= 0; --index)
    {
      if (this.m_largeShakeEventCallbacks[index] == null)
        this.m_largeShakeEventCallbacks.RemoveAt(index);
      else
        this.m_largeShakeEventCallbacks[index]();
    }
  }

  private void CallbackEvent(List<BoardEvents.EventCallback> eventList, float weight)
  {
    if (eventList == null)
      return;
    for (int index = eventList.Count - 1; index >= 0; --index)
    {
      if (eventList[index] == null)
        eventList.RemoveAt(index);
      else if ((double) weight >= (double) eventList[index].minimumWeight)
        eventList[index].callback(weight);
    }
  }

  private void AddWeight(BoardEvents.EVENT_TYPE eventType, float weight)
  {
    if (this.m_weights.ContainsKey(eventType))
      this.m_weights[eventType] += weight;
    else
      this.m_weights.Add(eventType, weight);
  }

  public enum EVENT_TYPE
  {
    FriendlyHeroDamage,
    OpponentHeroDamage,
    FriendlyHeroHeal,
    OpponentHeroHeal,
    FriendlyLegendaryMinionSpawn,
    OpponentLegendaryMinionSpawn,
    FriendlyLegendaryMinionDeath,
    OpponentLegendaryMinionDeath,
    FriendlyMinionSpawn,
    OpponentMinionSpawn,
    FriendlyMinionDeath,
    OpponentMinionDeath,
    FriendlyMinionDamage,
    OpponentMinionDamage,
    FriendlyMinionHeal,
    OpponentMinionHeal,
    LargeMinionShake,
  }

  public class EventData
  {
    public BoardEvents.EVENT_TYPE m_eventType;
    public float m_timeStamp;
    public Card m_card;
    public float m_value;
    public TAG_RARITY m_rarity;
  }

  public class EventCallback
  {
    public BoardEvents.EventDelegate callback;
    public float minimumWeight;
  }

  public delegate void LargeShakeEventDelegate();

  public delegate void EventDelegate(float weight);
}
