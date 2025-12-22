using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScheduledCharacterDialogDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_characterDialogId;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private ScheduledCharacterDialog.Event m_clientEvent = ScheduledCharacterDialog.ParseEventValue("login_flow_complete");
  [SerializeField]
  private long m_clientEventData;
  [SerializeField]
  private bool m_showToReturningPlayer;
  [SerializeField]
  private bool m_showToNewPlayer;
  [SerializeField]
  private string m_enabled = "true";
  [SerializeField]
  private int m_displayOrder;

  [DbfField("CHARACTER_DIALOG_ID")]
  public int CharacterDialogId => this.m_characterDialogId;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("CLIENT_EVENT")]
  public ScheduledCharacterDialog.Event ClientEvent => this.m_clientEvent;

  [DbfField("CLIENT_EVENT_DATA")]
  public long ClientEventData => this.m_clientEventData;

  [DbfField("SHOW_TO_RETURNING_PLAYER")]
  public bool ShowToReturningPlayer => this.m_showToReturningPlayer;

  [DbfField("SHOW_TO_NEW_PLAYER")]
  public bool ShowToNewPlayer => this.m_showToNewPlayer;

  [DbfField("ENABLED")]
  public string Enabled => this.m_enabled;

  [DbfField("DISPLAY_ORDER")]
  public int DisplayOrder => this.m_displayOrder;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "CHARACTER_DIALOG_ID":
        return (object) this.m_characterDialogId;
      case "CLIENT_EVENT":
        return (object) this.m_clientEvent;
      case "CLIENT_EVENT_DATA":
        return (object) this.m_clientEventData;
      case "DISPLAY_ORDER":
        return (object) this.m_displayOrder;
      case "ENABLED":
        return (object) this.m_enabled;
      case "EVENT":
        return (object) this.m_event;
      case "ID":
        return (object) this.ID;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "SHOW_TO_NEW_PLAYER":
        return (object) this.m_showToNewPlayer;
      case "SHOW_TO_RETURNING_PLAYER":
        return (object) this.m_showToReturningPlayer;
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
      case 1427503831:
        if (!(name == "CHARACTER_DIALOG_ID"))
          break;
        this.m_characterDialogId = (int) val;
        break;
      case 1441469002:
        if (!(name == "CLIENT_EVENT_DATA"))
          break;
        this.m_clientEventData = (long) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1832324731:
        if (!(name == "SHOW_TO_RETURNING_PLAYER"))
          break;
        this.m_showToReturningPlayer = (bool) val;
        break;
      case 2100707457:
        if (!(name == "CLIENT_EVENT"))
          break;
        switch (val)
        {
          case null:
            this.m_clientEvent = ScheduledCharacterDialog.Event.LOGIN_FLOW_COMPLETE;
            return;
          case ScheduledCharacterDialog.Event _:
          case int _:
            this.m_clientEvent = (ScheduledCharacterDialog.Event) val;
            return;
          case string _:
            this.m_clientEvent = ScheduledCharacterDialog.ParseEventValue((string) val);
            return;
          default:
            return;
        }
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (string) val;
        break;
      case 2320110678:
        if (!(name == "DISPLAY_ORDER"))
          break;
        this.m_displayOrder = (int) val;
        break;
      case 2435862099:
        if (!(name == "SHOW_TO_NEW_PLAYER"))
          break;
        this.m_showToNewPlayer = (bool) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "CHARACTER_DIALOG_ID":
        return typeof (int);
      case "CLIENT_EVENT":
        return typeof (ScheduledCharacterDialog.Event);
      case "CLIENT_EVENT_DATA":
        return typeof (long);
      case "DISPLAY_ORDER":
        return typeof (int);
      case "ENABLED":
        return typeof (string);
      case "EVENT":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "SHOW_TO_NEW_PLAYER":
        return typeof (bool);
      case "SHOW_TO_RETURNING_PLAYER":
        return typeof (bool);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadScheduledCharacterDialogDbfRecords loadRecords = new LoadScheduledCharacterDialogDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ScheduledCharacterDialogDbfAsset characterDialogDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ScheduledCharacterDialogDbfAsset)) as ScheduledCharacterDialogDbfAsset;
    if ((UnityEngine.Object) characterDialogDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ScheduledCharacterDialogDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < characterDialogDbfAsset.Records.Count; ++index)
      characterDialogDbfAsset.Records[index].StripUnusedLocales();
    records = characterDialogDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
