using Blizzard.T5.Configuration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusUtil;
using System;
using System.Collections.Generic;

public class SpecialEventManager : IService
{
  private Dictionary<SpecialEventType, List<SpecialEventManager.EventAddedListener>> m_allEventAddedListeners = new Dictionary<SpecialEventType, List<SpecialEventManager.EventAddedListener>>();
  private int m_nextEventTimingId = 10000000;
  private Dictionary<string, SpecialEventType> m_eventTimingIdByEventName = new Dictionary<string, SpecialEventType>();
  private Dictionary<SpecialEventType, SpecialEventManager.EventTiming> m_eventTimings = new Dictionary<SpecialEventType, SpecialEventManager.EventTiming>((IEqualityComparer<SpecialEventType>) new SpecialEventManager.SpecialEventTypeComparer());
  private HashSet<SpecialEventType> m_forcedInactiveEvents;
  private HashSet<SpecialEventType> m_forcedActiveEvents;

  public event SpecialEventManager.OnReceivedEventTimingsFromServerDelegate OnReceivedEventTimingsFromServer = () => { };

  public bool AddEventAddedListener(
    SpecialEventManager.EventAddedCallback callback,
    SpecialEventType eventType,
    object userData = null)
  {
    if (callback == null)
      return false;
    SpecialEventManager.EventAddedListener eventAddedListener = new SpecialEventManager.EventAddedListener();
    eventAddedListener.SetCallback(callback);
    eventAddedListener.SetUserData(userData);
    if (!this.m_allEventAddedListeners.ContainsKey(eventType))
      this.m_allEventAddedListeners[eventType] = new List<SpecialEventManager.EventAddedListener>();
    this.m_allEventAddedListeners[eventType].Add(eventAddedListener);
    return true;
  }

  private void FireEventAddedEvents(SpecialEventType eventType)
  {
    List<SpecialEventManager.EventAddedListener> eventAddedListenerList;
    if (!this.m_allEventAddedListeners.TryGetValue(eventType, out eventAddedListenerList))
      return;
    foreach (SpecialEventManager.EventAddedListener eventAddedListener in eventAddedListenerList)
      eventAddedListener.Fire();
  }

  public HashSet<SpecialEventType> AllKnownEvents => new HashSet<SpecialEventType>((IEnumerable<SpecialEventType>) this.m_eventTimings.Keys);

  public bool HasReceivedEventTimingsFromServer { get; private set; }

  public long DevTimeOffsetSeconds { get; private set; }

  public SpecialEventVisualMgr Visuals { get; private set; }

  private SpecialEventType RegisterEventTimingName(string eventName)
  {
    SpecialEventType eventType = this.GetEventType(eventName);
    if (eventType != SpecialEventType.UNKNOWN)
      return eventType;
    SpecialEventType specialEventType = (SpecialEventType) ++this.m_nextEventTimingId;
    this.m_eventTimingIdByEventName[eventName] = specialEventType;
    return specialEventType;
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    SpecialEventManager specialEventManager = this;
    InstantiatePrefab loadVisuals;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      specialEventManager.Visuals = loadVisuals.InstantiatedPrefab.GetComponent<SpecialEventVisualMgr>();
      HearthstoneApplication.Get().WillReset += new Action(specialEventManager.WillReset);
      specialEventManager.InitializeHardcodedEvents();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    loadVisuals = new InstantiatePrefab((AssetReference) "SpecialEventVisualMgr.prefab:9e2d0e3e4eb236f418ecaf0fa12732e4");
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (IAsyncJobResult) loadVisuals;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (IAssetLoader)
  };

  public void Shutdown()
  {
  }

  private void WillReset()
  {
    this.m_eventTimingIdByEventName.Clear();
    this.m_eventTimings.Clear();
    if (this.m_forcedInactiveEvents != null)
      this.m_forcedInactiveEvents.Clear();
    if (this.m_forcedActiveEvents != null)
      this.m_forcedActiveEvents.Clear();
    this.HasReceivedEventTimingsFromServer = false;
    this.m_allEventAddedListeners.Clear();
    this.InitializeEventNames();
    this.InitializeHardcodedEvents();
  }

  public static SpecialEventManager Get() => ServiceManager.Get<SpecialEventManager>();

  private void InitializeEventNames()
  {
    SpecialEventMap eventMap = DbfShared.GetEventMap();
    for (int index = 0; index < eventMap.Keys.Count; ++index)
    {
      if (!this.m_eventTimingIdByEventName.ContainsKey(eventMap.Keys[index]))
        this.m_eventTimingIdByEventName.Add(eventMap.Keys[index], (SpecialEventType) eventMap.Values[index]);
    }
    this.m_nextEventTimingId = eventMap.CurrentId;
  }

  private void InitializeHardcodedEvents()
  {
    DateTime utcNow = DateTime.UtcNow;
    this.m_eventTimings[SpecialEventType.SPECIAL_EVENT_ALWAYS] = new SpecialEventManager.EventTiming(EnumUtils.GetString<SpecialEventType>(SpecialEventType.SPECIAL_EVENT_ALWAYS), 650L, SpecialEventType.SPECIAL_EVENT_ALWAYS, new DateTime?(), new DateTime?());
    this.m_eventTimings[SpecialEventType.SPECIAL_EVENT_NEVER] = new SpecialEventManager.EventTiming(EnumUtils.GetString<SpecialEventType>(SpecialEventType.SPECIAL_EVENT_NEVER), 320L, SpecialEventType.SPECIAL_EVENT_NEVER, new DateTime?(utcNow.AddSeconds(-1.0)), new DateTime?(utcNow.AddSeconds(-2.0)));
  }

  public void InitEventTimingsFromServer(
    long devTimeOffsetSeconds,
    IList<SpecialEventTiming> serverEventTimingList)
  {
    this.m_forcedActiveEvents = this.m_forcedInactiveEvents = (HashSet<SpecialEventType>) null;
    this.DevTimeOffsetSeconds = devTimeOffsetSeconds;
    this.m_eventTimingIdByEventName.Clear();
    this.m_eventTimings.Clear();
    this.InitializeEventNames();
    this.InitializeHardcodedEvents();
    List<SpecialEventType> specialEventTypeList = new List<SpecialEventType>();
    DateTime utcNow = DateTime.UtcNow;
    for (int index = 0; index < serverEventTimingList.Count; ++index)
    {
      SpecialEventTiming serverEventTiming = serverEventTimingList[index];
      SpecialEventType specialEventType = this.RegisterEventTimingName(serverEventTiming.EventName);
      DateTime? startTimeUtc = new DateTime?();
      if (serverEventTiming.HasSecondsToStart)
        startTimeUtc = new DateTime?(utcNow.AddSeconds((double) serverEventTiming.SecondsToStart));
      DateTime? endTimeUtc = new DateTime?();
      if (serverEventTiming.HasSecondsToEnd)
        endTimeUtc = new DateTime?(utcNow.AddSeconds((double) serverEventTiming.SecondsToEnd));
      this.m_eventTimings[specialEventType] = new SpecialEventManager.EventTiming(serverEventTiming.EventName, serverEventTiming.EventId, specialEventType, startTimeUtc, endTimeUtc);
      specialEventTypeList.Add(specialEventType);
    }
    this.HasReceivedEventTimingsFromServer = true;
    SpecialEventManager.OnReceivedEventTimingsFromServerDelegate timingsFromServer = this.OnReceivedEventTimingsFromServer;
    if (timingsFromServer != null)
      timingsFromServer();
    foreach (SpecialEventType eventType in specialEventTypeList)
      this.FireEventAddedEvents(eventType);
  }

  public DateTime? GetEventStartTimeUtc(SpecialEventType eventType)
  {
    SpecialEventManager.EventTiming eventTiming;
    return this.m_eventTimings.TryGetValue(eventType, out eventTiming) && eventTiming != null ? eventTiming.StartTimeUtc : new DateTime?();
  }

  public DateTime? GetEventEndTimeUtc(SpecialEventType eventType)
  {
    SpecialEventManager.EventTiming eventTiming;
    return this.m_eventTimings.TryGetValue(eventType, out eventTiming) && eventTiming != null ? eventTiming.EndTimeUtc : new DateTime?();
  }

  public TimeSpan GetTimeUntilEventStart(SpecialEventType eventType)
  {
    DateTime? eventStartTimeUtc = this.GetEventStartTimeUtc(eventType);
    return !eventStartTimeUtc.HasValue ? TimeSpan.Zero : eventStartTimeUtc.Value - DateTime.UtcNow;
  }

  public TimeSpan GetTimeLeftForEvent(SpecialEventType eventType)
  {
    SpecialEventManager.EventTiming eventTiming;
    this.m_eventTimings.TryGetValue(eventType, out eventTiming);
    TimeSpan timeLeftForEvent = new TimeSpan();
    if (eventTiming != null && eventTiming.EndTimeUtc.HasValue)
    {
      DateTime utcNow = DateTime.UtcNow;
      timeLeftForEvent = eventTiming.EndTimeUtc.Value - utcNow;
    }
    return timeLeftForEvent;
  }

  public bool GetEventRangeUtc(SpecialEventType eventType, out DateTime? start, out DateTime? end)
  {
    SpecialEventManager.EventTiming eventTiming;
    if (this.m_eventTimings.TryGetValue(eventType, out eventTiming) && eventTiming != null)
    {
      start = eventTiming.StartTimeUtc;
      end = eventTiming.EndTimeUtc;
      return true;
    }
    start = new DateTime?();
    end = new DateTime?();
    return false;
  }

  public bool HasEventStarted(SpecialEventType eventType)
  {
    if (this.IsEventForcedInactive(eventType))
      return false;
    if (this.IsEventForcedActive(eventType))
      return true;
    SpecialEventManager.EventTiming eventTiming;
    return this.m_eventTimings.TryGetValue(eventType, out eventTiming) && eventTiming.HasStarted(DateTime.UtcNow);
  }

  public bool IsStartTimeInTheFuture(SpecialEventType eventType)
  {
    SpecialEventManager.EventTiming eventTiming;
    return eventType != SpecialEventType.UNKNOWN && !this.IsEventForcedInactive(eventType) && !this.IsEventForcedActive(eventType) && this.m_eventTimings.TryGetValue(eventType, out eventTiming) && eventTiming.IsStartTimeInTheFuture(DateTime.UtcNow);
  }

  public bool HasEventEnded(SpecialEventType eventType)
  {
    SpecialEventManager.EventTiming eventTiming;
    return !this.IsEventForcedInactive(eventType) && !this.IsEventForcedActive(eventType) && this.m_eventTimings.TryGetValue(eventType, out eventTiming) && eventTiming.HasEnded(DateTime.UtcNow);
  }

  public bool IsEventActive(SpecialEventType eventType, bool activeIfDoesNotExist) => this.IsEventActive(eventType, activeIfDoesNotExist, DateTime.UtcNow);

  public bool IsEventActive(
    SpecialEventType eventType,
    bool activeIfDoesNotExist,
    DateTime utcTimestamp)
  {
    return this.IsEventActive_Impl(eventType, activeIfDoesNotExist, utcTimestamp);
  }

  public bool IsEventActive(string eventName, bool activeIfDoesNotExist) => this.IsEventActive(eventName, activeIfDoesNotExist, DateTime.UtcNow);

  public bool IsEventActive(string eventName, bool activeIfDoesNotExist, DateTime utcTimestamp)
  {
    if (string.IsNullOrEmpty(eventName))
      return activeIfDoesNotExist;
    SpecialEventType eventType = this.GetEventType(eventName);
    return eventType == SpecialEventType.UNKNOWN ? activeIfDoesNotExist : this.IsEventActive(eventType, activeIfDoesNotExist, utcTimestamp);
  }

  public SpecialEventType GetEventType(string eventName)
  {
    SpecialEventType specialEventType;
    return eventName == null || !this.m_eventTimingIdByEventName.TryGetValue(eventName, out specialEventType) ? SpecialEventType.UNKNOWN : specialEventType;
  }

  public string GetName(SpecialEventType eventType)
  {
    SpecialEventManager.EventTiming eventTiming;
    return this.m_eventTimings.TryGetValue(eventType, out eventTiming) && eventTiming != null ? eventTiming.Name : EnumUtils.GetString<SpecialEventType>(eventType);
  }

  private bool IsEventActive_Impl(
    SpecialEventType eventType,
    bool activeIfDoesNotExist,
    DateTime localTimestamp)
  {
    if (eventType == SpecialEventType.SPECIAL_EVENT_ALWAYS)
      return true;
    if (eventType == SpecialEventType.SPECIAL_EVENT_NEVER || this.IsEventForcedInactive(eventType))
      return false;
    if (this.IsEventForcedActive(eventType))
      return true;
    SpecialEventManager.EventTiming eventTiming;
    return !this.m_eventTimings.TryGetValue(eventType, out eventTiming) ? activeIfDoesNotExist : eventTiming.IsActiveNow(localTimestamp);
  }

  public bool IsEventForcedInactive(SpecialEventType eventType) => this.IsEventTimingForced(eventType, "Events.ForceInactive", ref this.m_forcedInactiveEvents);

  public bool IsEventForcedActive(SpecialEventType eventType) => this.IsEventTimingForced(eventType, "Events.ForceActive", ref this.m_forcedActiveEvents);

  public SpecialEventType GetActiveEventType()
  {
    if (this.IsEventActive(SpecialEventType.GVG_PROMOTION, false))
      return SpecialEventType.GVG_PROMOTION;
    return this.IsEventActive(SpecialEventType.SPECIAL_EVENT_PRE_TAVERN_BRAWL, false) ? SpecialEventType.SPECIAL_EVENT_PRE_TAVERN_BRAWL : SpecialEventType.IGNORE;
  }

  private bool IsEventTimingForced(
    SpecialEventType eventType,
    string clientConfigVarKey,
    ref HashSet<SpecialEventType> forcedEventSet)
  {
    if (!HearthstoneApplication.IsInternal())
      return false;
    if (forcedEventSet == null)
    {
      forcedEventSet = new HashSet<SpecialEventType>((IEqualityComparer<SpecialEventType>) new SpecialEventManager.SpecialEventTypeComparer());
      string str1 = Vars.Key(clientConfigVarKey).GetStr((string) null);
      if (string.IsNullOrEmpty(str1))
        return false;
      string str2 = str1;
      char[] separator = new char[3]{ ' ', ',', ';' };
      foreach (string eventName in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        SpecialEventType eventType1 = this.GetEventType(eventName);
        if (eventType1 != SpecialEventType.UNKNOWN)
          forcedEventSet.Add(eventType1);
      }
    }
    return forcedEventSet.Contains(eventType);
  }

  public long GetEventIdFromEventName(SpecialEventType eventName)
  {
    SpecialEventManager.EventTiming eventTiming;
    return this.m_eventTimings.TryGetValue(eventName, out eventTiming) ? eventTiming.Id : -1L;
  }

  public long GetEventIdFromEventName(string eventName)
  {
    foreach (KeyValuePair<SpecialEventType, SpecialEventManager.EventTiming> eventTiming in this.m_eventTimings)
    {
      if (eventTiming.Value.Name.Equals(eventName, StringComparison.OrdinalIgnoreCase))
        return eventTiming.Value.Id;
    }
    return -1;
  }

  public delegate void OnReceivedEventTimingsFromServerDelegate();

  private class EventTiming
  {
    public EventTiming(
      string name,
      long id,
      SpecialEventType eventType,
      DateTime? startTimeUtc,
      DateTime? endTimeUtc)
    {
      this.Id = id;
      this.Name = name;
      this.Type = eventType;
      this.StartTimeUtc = startTimeUtc;
      this.EndTimeUtc = endTimeUtc;
      DateTime? nullable;
      DateTime dateTime;
      if (this.StartTimeUtc.HasValue && this.StartTimeUtc.Value.Kind != DateTimeKind.Utc)
      {
        nullable = this.StartTimeUtc;
        dateTime = nullable.Value;
        this.StartTimeUtc = new DateTime?(dateTime.ToUniversalTime());
      }
      nullable = this.EndTimeUtc;
      if (!nullable.HasValue)
        return;
      nullable = this.EndTimeUtc;
      dateTime = nullable.Value;
      if (dateTime.Kind == DateTimeKind.Utc)
        return;
      nullable = this.EndTimeUtc;
      dateTime = nullable.Value;
      this.EndTimeUtc = new DateTime?(dateTime.ToUniversalTime());
    }

    public string Name { get; private set; }

    public long Id { get; private set; }

    private SpecialEventType Type
    {
      set => this.\u003CType\u003Ek__BackingField = value;
    }

    public DateTime? StartTimeUtc { get; private set; }

    public DateTime? EndTimeUtc { get; private set; }

    public bool HasStarted(DateTime utcTimestamp)
    {
      if (!this.StartTimeUtc.HasValue)
        return true;
      if (utcTimestamp.Kind != DateTimeKind.Utc)
        utcTimestamp = utcTimestamp.ToUniversalTime();
      return utcTimestamp >= this.StartTimeUtc.Value;
    }

    public bool HasEnded(DateTime utcTimestamp)
    {
      if (!this.EndTimeUtc.HasValue)
        return false;
      if (utcTimestamp.Kind != DateTimeKind.Utc)
        utcTimestamp = utcTimestamp.ToUniversalTime();
      return utcTimestamp > this.EndTimeUtc.Value;
    }

    public bool IsActiveNow(DateTime utcTimestamp)
    {
      if (this.StartTimeUtc.HasValue && this.EndTimeUtc.HasValue && this.EndTimeUtc.Value < this.StartTimeUtc.Value)
        return false;
      if (utcTimestamp.Kind != DateTimeKind.Utc)
        utcTimestamp = utcTimestamp.ToUniversalTime();
      return this.HasStarted(utcTimestamp) && !this.HasEnded(utcTimestamp);
    }

    public bool IsStartTimeInTheFuture(DateTime utcTimestamp)
    {
      if (utcTimestamp.Kind != DateTimeKind.Utc)
        utcTimestamp = utcTimestamp.ToUniversalTime();
      DateTime? startTimeUtc = this.StartTimeUtc;
      if (!startTimeUtc.HasValue)
        return false;
      startTimeUtc = this.StartTimeUtc;
      return startTimeUtc.Value > utcTimestamp;
    }
  }

  private class SpecialEventTypeComparer : IEqualityComparer<SpecialEventType>
  {
    public bool Equals(SpecialEventType x, SpecialEventType y) => x == y;

    public int GetHashCode(SpecialEventType obj) => (int) obj;
  }

  public delegate void EventAddedCallback(object userData);

  private class EventAddedListener : EventListener<SpecialEventManager.EventAddedCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
