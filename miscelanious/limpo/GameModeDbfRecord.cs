using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameModeDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private string m_gameModeButtonState;
  [SerializeField]
  private string m_linkedScene;
  [SerializeField]
  private SpecialEventType m_showAsNewEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private SpecialEventType m_showAsEarlyAccessEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private SpecialEventType m_showAsBetaEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private int m_featureUnlockId;
  [SerializeField]
  private int m_featureUnlockId2;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("GAME_MODE_BUTTON_STATE")]
  public string GameModeButtonState => this.m_gameModeButtonState;

  [DbfField("LINKED_SCENE")]
  public string LinkedScene => this.m_linkedScene;

  [DbfField("SHOW_AS_NEW_EVENT")]
  public SpecialEventType ShowAsNewEvent => this.m_showAsNewEvent;

  [DbfField("SHOW_AS_EARLY_ACCESS_EVENT")]
  public SpecialEventType ShowAsEarlyAccessEvent => this.m_showAsEarlyAccessEvent;

  [DbfField("SHOW_AS_BETA_EVENT")]
  public SpecialEventType ShowAsBetaEvent => this.m_showAsBetaEvent;

  [DbfField("FEATURE_UNLOCK_ID")]
  public int FeatureUnlockId => this.m_featureUnlockId;

  [DbfField("FEATURE_UNLOCK_ID_2")]
  public int FeatureUnlockId2 => this.m_featureUnlockId2;

  public void SetNoteDesc(string v) => this.m_noteDesc = v;

  public void SetEvent(SpecialEventType v) => this.m_event = v;

  public void SetName(DbfLocValue v)
  {
    this.m_name = v;
    v.SetDebugInfo(this.ID, "NAME");
  }

  public void SetDescription(DbfLocValue v)
  {
    this.m_description = v;
    v.SetDebugInfo(this.ID, "DESCRIPTION");
  }

  public void SetSortOrder(int v) => this.m_sortOrder = v;

  public void SetGameModeButtonState(string v) => this.m_gameModeButtonState = v;

  public void SetLinkedScene(string v) => this.m_linkedScene = v;

  public void SetShowAsNewEvent(SpecialEventType v) => this.m_showAsNewEvent = v;

  public void SetShowAsEarlyAccessEvent(SpecialEventType v) => this.m_showAsEarlyAccessEvent = v;

  public void SetShowAsBetaEvent(SpecialEventType v) => this.m_showAsBetaEvent = v;

  public void SetFeatureUnlockId(int v) => this.m_featureUnlockId = v;

  public void SetFeatureUnlockId2(int v) => this.m_featureUnlockId2 = v;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "DESCRIPTION":
        return (object) this.m_description;
      case "EVENT":
        return (object) this.m_event;
      case "FEATURE_UNLOCK_ID":
        return (object) this.m_featureUnlockId;
      case "FEATURE_UNLOCK_ID_2":
        return (object) this.m_featureUnlockId2;
      case "GAME_MODE_BUTTON_STATE":
        return (object) this.m_gameModeButtonState;
      case "ID":
        return (object) this.ID;
      case "LINKED_SCENE":
        return (object) this.m_linkedScene;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "SHOW_AS_BETA_EVENT":
        return (object) this.m_showAsBetaEvent;
      case "SHOW_AS_EARLY_ACCESS_EVENT":
        return (object) this.m_showAsEarlyAccessEvent;
      case "SHOW_AS_NEW_EVENT":
        return (object) this.m_showAsNewEvent;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 236776447:
        if (!(name == "EVENT"))
          break;
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 237382363:
        if (!(name == "SHOW_AS_EARLY_ACCESS_EVENT"))
          break;
        this.m_showAsEarlyAccessEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 714481862:
        if (!(name == "GAME_MODE_BUTTON_STATE"))
          break;
        this.m_gameModeButtonState = (string) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1697257895:
        if (!(name == "FEATURE_UNLOCK_ID_2"))
          break;
        this.m_featureUnlockId2 = (int) val;
        break;
      case 2279702235:
        if (!(name == "SHOW_AS_NEW_EVENT"))
          break;
        this.m_showAsNewEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3742737450:
        if (!(name == "FEATURE_UNLOCK_ID"))
          break;
        this.m_featureUnlockId = (int) val;
        break;
      case 3832007291:
        if (!(name == "LINKED_SCENE"))
          break;
        this.m_linkedScene = (string) val;
        break;
      case 3884587951:
        if (!(name == "SHOW_AS_BETA_EVENT"))
          break;
        this.m_showAsBetaEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "EVENT":
        return typeof (string);
      case "FEATURE_UNLOCK_ID":
        return typeof (int);
      case "FEATURE_UNLOCK_ID_2":
        return typeof (int);
      case "GAME_MODE_BUTTON_STATE":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "LINKED_SCENE":
        return typeof (string);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "SHOW_AS_BETA_EVENT":
        return typeof (string);
      case "SHOW_AS_EARLY_ACCESS_EVENT":
        return typeof (string);
      case "SHOW_AS_NEW_EVENT":
        return typeof (string);
      case "SORT_ORDER":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadGameModeDbfRecords loadRecords = new LoadGameModeDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    GameModeDbfAsset gameModeDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (GameModeDbfAsset)) as GameModeDbfAsset;
    if ((UnityEngine.Object) gameModeDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("GameModeDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < gameModeDbfAsset.Records.Count; ++index)
      gameModeDbfAsset.Records[index].StripUnusedLocales();
    records = gameModeDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
