using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdventureMissionDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_scenarioId;
  [SerializeField]
  private string m_noteDesc = "SYSDATE ";
  [SerializeField]
  private int m_reqWingId;
  [SerializeField]
  private int m_reqProgress;
  [SerializeField]
  private ulong m_reqFlags;
  [SerializeField]
  private int m_grantsWingId;
  [SerializeField]
  private int m_grantsProgress;
  [SerializeField]
  private ulong m_grantsFlags;
  [SerializeField]
  private string m_bossDefAssetPath;
  [SerializeField]
  private string m_classChallengePrefabPopup;

  [DbfField("SCENARIO_ID")]
  public int ScenarioId => this.m_scenarioId;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("REQ_WING_ID")]
  public int ReqWingId => this.m_reqWingId;

  [DbfField("REQ_PROGRESS")]
  public int ReqProgress => this.m_reqProgress;

  [DbfField("REQ_FLAGS")]
  public ulong ReqFlags => this.m_reqFlags;

  [DbfField("GRANTS_WING_ID")]
  public int GrantsWingId => this.m_grantsWingId;

  [DbfField("GRANTS_PROGRESS")]
  public int GrantsProgress => this.m_grantsProgress;

  [DbfField("GRANTS_FLAGS")]
  public ulong GrantsFlags => this.m_grantsFlags;

  [DbfField("BOSS_DEF_ASSET_PATH")]
  public string BossDefAssetPath => this.m_bossDefAssetPath;

  [DbfField("CLASS_CHALLENGE_PREFAB_POPUP")]
  public string ClassChallengePrefabPopup => this.m_classChallengePrefabPopup;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BOSS_DEF_ASSET_PATH":
        return (object) this.m_bossDefAssetPath;
      case "CLASS_CHALLENGE_PREFAB_POPUP":
        return (object) this.m_classChallengePrefabPopup;
      case "GRANTS_FLAGS":
        return (object) this.m_grantsFlags;
      case "GRANTS_PROGRESS":
        return (object) this.m_grantsProgress;
      case "GRANTS_WING_ID":
        return (object) this.m_grantsWingId;
      case "ID":
        return (object) this.ID;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "REQ_FLAGS":
        return (object) this.m_reqFlags;
      case "REQ_PROGRESS":
        return (object) this.m_reqProgress;
      case "REQ_WING_ID":
        return (object) this.m_reqWingId;
      case "SCENARIO_ID":
        return (object) this.m_scenarioId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 283653000:
        if (!(name == "GRANTS_WING_ID"))
          break;
        this.m_grantsWingId = (int) val;
        break;
      case 600120207:
        if (!(name == "REQ_PROGRESS"))
          break;
        this.m_reqProgress = (int) val;
        break;
      case 693605261:
        if (!(name == "SCENARIO_ID"))
          break;
        this.m_scenarioId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2150619894:
        if (!(name == "GRANTS_FLAGS"))
          break;
        this.m_grantsFlags = (ulong) val;
        break;
      case 2682090115:
        if (!(name == "BOSS_DEF_ASSET_PATH"))
          break;
        this.m_bossDefAssetPath = (string) val;
        break;
      case 2785465717:
        if (!(name == "CLASS_CHALLENGE_PREFAB_POPUP"))
          break;
        this.m_classChallengePrefabPopup = (string) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3117134272:
        if (!(name == "GRANTS_PROGRESS"))
          break;
        this.m_grantsProgress = (int) val;
        break;
      case 3810699191:
        if (!(name == "REQ_FLAGS"))
          break;
        this.m_reqFlags = (ulong) val;
        break;
      case 3979977061:
        if (!(name == "REQ_WING_ID"))
          break;
        this.m_reqWingId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "BOSS_DEF_ASSET_PATH":
        return typeof (string);
      case "CLASS_CHALLENGE_PREFAB_POPUP":
        return typeof (string);
      case "GRANTS_FLAGS":
        return typeof (ulong);
      case "GRANTS_PROGRESS":
        return typeof (int);
      case "GRANTS_WING_ID":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "REQ_FLAGS":
        return typeof (ulong);
      case "REQ_PROGRESS":
        return typeof (int);
      case "REQ_WING_ID":
        return typeof (int);
      case "SCENARIO_ID":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAdventureMissionDbfRecords loadRecords = new LoadAdventureMissionDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AdventureMissionDbfAsset adventureMissionDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AdventureMissionDbfAsset)) as AdventureMissionDbfAsset;
    if ((UnityEngine.Object) adventureMissionDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AdventureMissionDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < adventureMissionDbfAsset.Records.Count; ++index)
      adventureMissionDbfAsset.Records[index].StripUnusedLocales();
    records = adventureMissionDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
