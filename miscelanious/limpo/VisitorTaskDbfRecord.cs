using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisitorTaskDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_mercenaryVisitorId;
  [SerializeField]
  private int m_mercenaryOverrideId;
  [SerializeField]
  private DbfLocValue m_taskTitle;
  [SerializeField]
  private DbfLocValue m_taskDescription;
  [SerializeField]
  private DbfLocValue m_mercenaryQuote;
  [SerializeField]
  private string m_mercenaryTaskBarkVo;
  [SerializeField]
  private int m_onAssignedDialogId;
  [SerializeField]
  private int m_onCompleteDialogId;
  [SerializeField]
  private int m_quota = 1;
  [SerializeField]
  private VisitorTask.VillageTutorialServerEvent m_tutorialEventType;
  [SerializeField]
  private int m_tutorialEventValue;
  [SerializeField]
  private int m_rewardListId;

  [DbfField("MERCENARY_VISITOR_ID")]
  public int MercenaryVisitorId => this.m_mercenaryVisitorId;

  [DbfField("MERCENARY_OVERRIDE")]
  public int MercenaryOverride => this.m_mercenaryOverrideId;

  [DbfField("TASK_TITLE")]
  public DbfLocValue TaskTitle => this.m_taskTitle;

  [DbfField("TASK_DESCRIPTION")]
  public DbfLocValue TaskDescription => this.m_taskDescription;

  [DbfField("MERCENARY_QUOTE")]
  public DbfLocValue MercenaryQuote => this.m_mercenaryQuote;

  [DbfField("MERCENARY_TASK_BARK_VO")]
  public string MercenaryTaskBarkVo => this.m_mercenaryTaskBarkVo;

  [DbfField("ON_ASSIGNED_DIALOG")]
  public int OnAssignedDialog => this.m_onAssignedDialogId;

  [DbfField("ON_COMPLETE_DIALOG")]
  public int OnCompleteDialog => this.m_onCompleteDialogId;

  [DbfField("QUOTA")]
  public int Quota => this.m_quota;

  public RewardListDbfRecord RewardListRecord => GameDbf.RewardList.GetRecord(this.m_rewardListId);

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ID":
        return (object) this.ID;
      case "MERCENARY_OVERRIDE":
        return (object) this.m_mercenaryOverrideId;
      case "MERCENARY_QUOTE":
        return (object) this.m_mercenaryQuote;
      case "MERCENARY_TASK_BARK_VO":
        return (object) this.m_mercenaryTaskBarkVo;
      case "MERCENARY_VISITOR_ID":
        return (object) this.m_mercenaryVisitorId;
      case "ON_ASSIGNED_DIALOG":
        return (object) this.m_onAssignedDialogId;
      case "ON_COMPLETE_DIALOG":
        return (object) this.m_onCompleteDialogId;
      case "QUOTA":
        return (object) this.m_quota;
      case "REWARD_LIST":
        return (object) this.m_rewardListId;
      case "TASK_DESCRIPTION":
        return (object) this.m_taskDescription;
      case "TASK_TITLE":
        return (object) this.m_taskTitle;
      case "TUTORIAL_EVENT_TYPE":
        return (object) this.m_tutorialEventType;
      case "TUTORIAL_EVENT_VALUE":
        return (object) this.m_tutorialEventValue;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 91158759:
        if (!(name == "REWARD_LIST"))
          break;
        this.m_rewardListId = (int) val;
        break;
      case 416172651:
        if (!(name == "QUOTA"))
          break;
        this.m_quota = (int) val;
        break;
      case 644321962:
        if (!(name == "TUTORIAL_EVENT_VALUE"))
          break;
        this.m_tutorialEventValue = (int) val;
        break;
      case 717079821:
        if (!(name == "TUTORIAL_EVENT_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_tutorialEventType = VisitorTask.VillageTutorialServerEvent.NONE;
            return;
          case VisitorTask.VillageTutorialServerEvent _:
          case int _:
            this.m_tutorialEventType = (VisitorTask.VillageTutorialServerEvent) val;
            return;
          case string _:
            this.m_tutorialEventType = VisitorTask.ParseVillageTutorialServerEventValue((string) val);
            return;
          default:
            return;
        }
      case 811875621:
        if (!(name == "TASK_TITLE"))
          break;
        this.m_taskTitle = (DbfLocValue) val;
        break;
      case 961678632:
        if (!(name == "MERCENARY_QUOTE"))
          break;
        this.m_mercenaryQuote = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1705389014:
        if (!(name == "MERCENARY_VISITOR_ID"))
          break;
        this.m_mercenaryVisitorId = (int) val;
        break;
      case 1961762331:
        if (!(name == "ON_COMPLETE_DIALOG"))
          break;
        this.m_onCompleteDialogId = (int) val;
        break;
      case 2756122900:
        if (!(name == "MERCENARY_OVERRIDE"))
          break;
        this.m_mercenaryOverrideId = (int) val;
        break;
      case 2805530386:
        if (!(name == "MERCENARY_TASK_BARK_VO"))
          break;
        this.m_mercenaryTaskBarkVo = (string) val;
        break;
      case 2948354685:
        if (!(name == "TASK_DESCRIPTION"))
          break;
        this.m_taskDescription = (DbfLocValue) val;
        break;
      case 3288947502:
        if (!(name == "ON_ASSIGNED_DIALOG"))
          break;
        this.m_onAssignedDialogId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ID":
        return typeof (int);
      case "MERCENARY_OVERRIDE":
        return typeof (int);
      case "MERCENARY_QUOTE":
        return typeof (DbfLocValue);
      case "MERCENARY_TASK_BARK_VO":
        return typeof (string);
      case "MERCENARY_VISITOR_ID":
        return typeof (int);
      case "ON_ASSIGNED_DIALOG":
        return typeof (int);
      case "ON_COMPLETE_DIALOG":
        return typeof (int);
      case "QUOTA":
        return typeof (int);
      case "REWARD_LIST":
        return typeof (int);
      case "TASK_DESCRIPTION":
        return typeof (DbfLocValue);
      case "TASK_TITLE":
        return typeof (DbfLocValue);
      case "TUTORIAL_EVENT_TYPE":
        return typeof (VisitorTask.VillageTutorialServerEvent);
      case "TUTORIAL_EVENT_VALUE":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadVisitorTaskDbfRecords loadRecords = new LoadVisitorTaskDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    VisitorTaskDbfAsset visitorTaskDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (VisitorTaskDbfAsset)) as VisitorTaskDbfAsset;
    if ((UnityEngine.Object) visitorTaskDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("VisitorTaskDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < visitorTaskDbfAsset.Records.Count; ++index)
      visitorTaskDbfAsset.Records[index].StripUnusedLocales();
    records = visitorTaskDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_taskTitle.StripUnusedLocales();
    this.m_taskDescription.StripUnusedLocales();
    this.m_mercenaryQuote.StripUnusedLocales();
  }
}
