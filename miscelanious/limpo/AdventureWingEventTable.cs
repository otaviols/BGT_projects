using System.Collections.Generic;

[CustomEditClass]
public class AdventureWingEventTable : StateEventTable
{
  private const string s_EventPlateActivate = "PlateActivate";
  private const string s_EventPlateDeactivate = "PlateDeactivate";
  private const string s_EventPlateInitialText = "PlateInitialText";
  private const string s_EventPlateBuy = "PlateBuy";
  private const string s_EventPlateInitialBuy = "PlateInitialBuy";
  private const string s_EventPlateKey = "PlateKey";
  private const string s_EventPlateKeyNotRecommended = "PlateKeyNotRecommended";
  private const string s_EventPlateInitialKey = "PlateInitialKey";
  private const string s_EventPlateInitialKeyNotRecommended = "PlateInitialKeyNotRecommended";
  private const string s_EventPlateOpen = "PlateOpen";
  private const string s_EventBigChestShow = "BigChestShow";
  private const string s_EventBigChestStayOpen = "BigChestStayOpen";
  private const string s_EventBigChestOpen = "BigChestOpen";
  private const string s_EventBigChestCover = "BigChestCover";
  private const string s_EventPlateCoverPreviewChest = "PlateCoverPreviewChest";
  private const string s_EventPlateReset = "PlateReset";
  public List<string> m_PlateOpenEvents;
  public List<string> m_PlateAlreadyOpenEvents;

  public bool IsPlatePartiallyOpen()
  {
    string lastState = this.GetLastState();
    return this.m_PlateAlreadyOpenEvents != null && this.m_PlateAlreadyOpenEvents.Count > 0 && this.m_PlateAlreadyOpenEvents.Contains(lastState);
  }

  public bool IsPlateKey()
  {
    string lastState = this.GetLastState();
    return lastState == "PlateKey" || lastState == "PlateInitialKey" || lastState == "PlateKeyNotRecommended" || lastState == "PlateInitialKeyNotRecommended";
  }

  public bool IsPlateBuy()
  {
    string lastState = this.GetLastState();
    return lastState == "PlateBuy" || lastState == "PlateInitialBuy";
  }

  public bool IsPlateInitialText() => this.GetLastState() == "PlateInitialText";

  public bool IsPlateInOrGoingToAnActiveState()
  {
    switch (this.GetLastState())
    {
      case "PlateActivate":
      case "PlateBuy":
      case "PlateInitialBuy":
      case "PlateInitialKey":
      case "PlateInitialKeyNotRecommended":
      case "PlateInitialText":
      case "PlateKey":
      case "PlateKeyNotRecommended":
        return true;
      default:
        return false;
    }
  }

  public void DoStatePlateActivate() => this.TriggerState("PlateActivate");

  public void DoStatePlateDeactivate() => this.TriggerState("PlateDeactivate");

  public void DoStatePlateBuy(bool initial = false)
  {
    if (this.IsPlateBuy())
      return;
    this.TriggerState(initial ? "PlateInitialBuy" : "PlateBuy");
  }

  public void DoStatePlateInitialText() => this.TriggerState("PlateInitialText");

  public void DoStatePlateKey(bool isRecommended, bool initial)
  {
    if (!isRecommended)
    {
      string eventName = initial ? "PlateInitialKeyNotRecommended" : "PlateKeyNotRecommended";
      if (this.GetStateEvent(eventName) != null)
      {
        this.TriggerState(eventName);
        return;
      }
    }
    this.TriggerState(initial ? "PlateInitialKey" : "PlateKey");
  }

  public void DoStatePlateOpen(int plateOpenEventIndex, float delay = 0.0f)
  {
    this.SetFloatVar("PlateOpen", "PostAnimationDelay", delay);
    if (this.m_PlateOpenEvents == null || this.m_PlateOpenEvents.Count == 0)
    {
      this.TriggerState("PlateOpen");
    }
    else
    {
      if (this.m_PlateOpenEvents.Count <= plateOpenEventIndex)
        return;
      this.TriggerState(this.m_PlateOpenEvents[plateOpenEventIndex]);
    }
  }

  public void DoStatePlateAlreadyOpen(int plateAlreadyOpenEventIndex)
  {
    if (plateAlreadyOpenEventIndex >= this.m_PlateAlreadyOpenEvents.Count)
      this.TriggerState("PlateDeactivate");
    else
      this.TriggerState(this.m_PlateAlreadyOpenEvents[plateAlreadyOpenEventIndex]);
  }

  public bool SupportsIncrementalOpening() => this.m_PlateOpenEvents != null && this.m_PlateOpenEvents.Count > 0;

  public void DoStatePlateCoverPreviewChest() => this.TriggerState("PlateCoverPreviewChest", false);

  public void DoStatePlateReset() => this.TriggerState("PlateReset", false);

  public void DoStateBigChestShow() => this.TriggerState("BigChestShow");

  public void DoStateBigChestStayOpen() => this.TriggerState("BigChestStayOpen");

  public void DoStateBigChestOpen() => this.TriggerState("BigChestOpen");

  public void DoStateBigChestCover() => this.TriggerState("BigChestCover");

  public void AddOpenPlateStartEventListener(StateEventTable.StateEventTrigger dlg, bool once = false) => this.AddStateEventStartListener("PlateOpen", dlg, once);

  public void RemoveOpenPlateStartEventListener(StateEventTable.StateEventTrigger dlg) => this.RemoveStateEventStartListener("PlateOpen", dlg);

  public void AddOpenPlateEndEventListener(StateEventTable.StateEventTrigger dlg, bool once = false)
  {
    this.AddStateEventEndListener("PlateOpen", dlg, once);
    foreach (string plateOpenEvent in this.m_PlateOpenEvents)
      this.AddStateEventEndListener(plateOpenEvent, dlg, once);
    foreach (string alreadyOpenEvent in this.m_PlateAlreadyOpenEvents)
      this.AddStateEventEndListener(alreadyOpenEvent, dlg, once);
  }

  public void RemoveOpenPlateEndEventListener(StateEventTable.StateEventTrigger dlg)
  {
    this.RemoveStateEventEndListener("PlateOpen", dlg);
    foreach (string plateOpenEvent in this.m_PlateOpenEvents)
      this.RemoveStateEventEndListener(plateOpenEvent, dlg);
    foreach (string alreadyOpenEvent in this.m_PlateAlreadyOpenEvents)
      this.RemoveStateEventEndListener(alreadyOpenEvent, dlg);
  }

  public void AddOpenChestStartEventListener(StateEventTable.StateEventTrigger dlg, bool once = false) => this.AddStateEventStartListener("BigChestOpen", dlg, once);

  public void RemoveOpenChestStartEventListener(StateEventTable.StateEventTrigger dlg) => this.RemoveStateEventStartListener("BigChestOpen", dlg);

  public void AddOpenChestEndEventListener(StateEventTable.StateEventTrigger dlg, bool once = false) => this.AddStateEventEndListener("BigChestOpen", dlg, once);

  public void RemoveOpenChestEndEventListener(StateEventTable.StateEventTrigger dlg) => this.RemoveStateEventEndListener("BigChestOpen", dlg);
}
