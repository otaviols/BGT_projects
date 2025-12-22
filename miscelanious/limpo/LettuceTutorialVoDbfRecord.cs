using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceTutorialVoDbfRecord : DbfRecord
{
  [SerializeField]
  private LettuceTutorialVo.LettuceTutorialEvent m_tutorialEvent;
  [SerializeField]
  private LettuceTutorialVo.LettuceTutorialEvent m_triggerEventOnComplete;
  [SerializeField]
  private int m_nodeTypeId;
  [SerializeField]
  private int m_requiredActiveBountyId;
  [SerializeField]
  private int m_requiredActiveVisitorId;
  [SerializeField]
  private int m_requiredActiveTaskId;
  [SerializeField]
  private bool m_onlyShowOnce = true;
  [SerializeField]
  private int m_showChance = 100;
  [SerializeField]
  private string m_uiEvent;
  [SerializeField]
  private string m_popup;
  [SerializeField]
  private int m_tutorialDialogId;

  [DbfField("TUTORIAL_EVENT")]
  public LettuceTutorialVo.LettuceTutorialEvent TutorialEvent => this.m_tutorialEvent;

  [DbfField("TRIGGER_EVENT_ON_COMPLETE")]
  public LettuceTutorialVo.LettuceTutorialEvent TriggerEventOnComplete => this.m_triggerEventOnComplete;

  [DbfField("NODE_TYPE_ID")]
  public int NodeTypeId => this.m_nodeTypeId;

  [DbfField("REQUIRED_ACTIVE_BOUNTY")]
  public int RequiredActiveBounty => this.m_requiredActiveBountyId;

  [DbfField("REQUIRED_ACTIVE_VISITOR")]
  public int RequiredActiveVisitor => this.m_requiredActiveVisitorId;

  [DbfField("REQUIRED_ACTIVE_TASK")]
  public int RequiredActiveTask => this.m_requiredActiveTaskId;

  [DbfField("ONLY_SHOW_ONCE")]
  public bool OnlyShowOnce => this.m_onlyShowOnce;

  [DbfField("SHOW_CHANCE")]
  public int ShowChance => this.m_showChance;

  [DbfField("UI_EVENT")]
  public string UiEvent => this.m_uiEvent;

  [DbfField("POPUP")]
  public string Popup => this.m_popup;

  [DbfField("TUTORIAL_DIALOG")]
  public int TutorialDialog => this.m_tutorialDialogId;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ID":
        return (object) this.ID;
      case "NODE_TYPE_ID":
        return (object) this.m_nodeTypeId;
      case "ONLY_SHOW_ONCE":
        return (object) this.m_onlyShowOnce;
      case "POPUP":
        return (object) this.m_popup;
      case "REQUIRED_ACTIVE_BOUNTY":
        return (object) this.m_requiredActiveBountyId;
      case "REQUIRED_ACTIVE_TASK":
        return (object) this.m_requiredActiveTaskId;
      case "REQUIRED_ACTIVE_VISITOR":
        return (object) this.m_requiredActiveVisitorId;
      case "SHOW_CHANCE":
        return (object) this.m_showChance;
      case "TRIGGER_EVENT_ON_COMPLETE":
        return (object) this.m_triggerEventOnComplete;
      case "TUTORIAL_DIALOG":
        return (object) this.m_tutorialDialogId;
      case "TUTORIAL_EVENT":
        return (object) this.m_tutorialEvent;
      case "UI_EVENT":
        return (object) this.m_uiEvent;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 913553762:
        if (!(name == "REQUIRED_ACTIVE_VISITOR"))
          break;
        this.m_requiredActiveVisitorId = (int) val;
        break;
      case 1364445728:
        if (!(name == "TUTORIAL_DIALOG"))
          break;
        this.m_tutorialDialogId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1582547108:
        if (!(name == "UI_EVENT"))
          break;
        this.m_uiEvent = (string) val;
        break;
      case 1814957419:
        if (!(name == "REQUIRED_ACTIVE_BOUNTY"))
          break;
        this.m_requiredActiveBountyId = (int) val;
        break;
      case 3277070541:
        if (!(name == "REQUIRED_ACTIVE_TASK"))
          break;
        this.m_requiredActiveTaskId = (int) val;
        break;
      case 3749565941:
        if (!(name == "SHOW_CHANCE"))
          break;
        this.m_showChance = (int) val;
        break;
      case 4000098685:
        if (!(name == "POPUP"))
          break;
        this.m_popup = (string) val;
        break;
      case 4071894838:
        if (!(name == "NODE_TYPE_ID"))
          break;
        this.m_nodeTypeId = (int) val;
        break;
      case 4124404046:
        if (!(name == "TRIGGER_EVENT_ON_COMPLETE"))
          break;
        switch (val)
        {
          case null:
            this.m_triggerEventOnComplete = LettuceTutorialVo.LettuceTutorialEvent.INVALID;
            return;
          case LettuceTutorialVo.LettuceTutorialEvent _:
          case int _:
            this.m_triggerEventOnComplete = (LettuceTutorialVo.LettuceTutorialEvent) val;
            return;
          case string _:
            this.m_triggerEventOnComplete = LettuceTutorialVo.ParseLettuceTutorialEventValue((string) val);
            return;
          default:
            return;
        }
      case 4236113055:
        if (!(name == "ONLY_SHOW_ONCE"))
          break;
        this.m_onlyShowOnce = (bool) val;
        break;
      case 4270993048:
        if (!(name == "TUTORIAL_EVENT"))
          break;
        switch (val)
        {
          case null:
            this.m_tutorialEvent = LettuceTutorialVo.LettuceTutorialEvent.INVALID;
            return;
          case LettuceTutorialVo.LettuceTutorialEvent _:
          case int _:
            this.m_tutorialEvent = (LettuceTutorialVo.LettuceTutorialEvent) val;
            return;
          case string _:
            this.m_tutorialEvent = LettuceTutorialVo.ParseLettuceTutorialEventValue((string) val);
            return;
          default:
            return;
        }
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ID":
        return typeof (int);
      case "NODE_TYPE_ID":
        return typeof (int);
      case "ONLY_SHOW_ONCE":
        return typeof (bool);
      case "POPUP":
        return typeof (string);
      case "REQUIRED_ACTIVE_BOUNTY":
        return typeof (int);
      case "REQUIRED_ACTIVE_TASK":
        return typeof (int);
      case "REQUIRED_ACTIVE_VISITOR":
        return typeof (int);
      case "SHOW_CHANCE":
        return typeof (int);
      case "TRIGGER_EVENT_ON_COMPLETE":
        return typeof (LettuceTutorialVo.LettuceTutorialEvent);
      case "TUTORIAL_DIALOG":
        return typeof (int);
      case "TUTORIAL_EVENT":
        return typeof (LettuceTutorialVo.LettuceTutorialEvent);
      case "UI_EVENT":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceTutorialVoDbfRecords loadRecords = new LoadLettuceTutorialVoDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceTutorialVoDbfAsset tutorialVoDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceTutorialVoDbfAsset)) as LettuceTutorialVoDbfAsset;
    if ((UnityEngine.Object) tutorialVoDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceTutorialVoDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < tutorialVoDbfAsset.Records.Count; ++index)
      tutorialVoDbfAsset.Records[index].StripUnusedLocales();
    records = tutorialVoDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
