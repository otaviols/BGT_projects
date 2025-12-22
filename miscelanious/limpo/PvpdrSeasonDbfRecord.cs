using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PvpdrSeasonDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_scenarioId;
  [SerializeField]
  private int m_maxWins;
  [SerializeField]
  private int m_maxLosses;
  [SerializeField]
  private int m_deckDisplayRulesetId;
  [SerializeField]
  private int m_maxHeroesDrafted;
  [SerializeField]
  private int m_rewardChestId;

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  public AdventureDbfRecord AdventureRecord => GameDbf.Adventure.GetRecord(this.m_adventureId);

  [DbfField("SCENARIO_ID")]
  public int ScenarioId => this.m_scenarioId;

  [DbfField("DECK_DISPLAY_RULESET_ID")]
  public int DeckDisplayRulesetId => this.m_deckDisplayRulesetId;

  [DbfField("MAX_HEROES_DRAFTED")]
  public int MaxHeroesDrafted => this.m_maxHeroesDrafted;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "DECK_DISPLAY_RULESET_ID":
        return (object) this.m_deckDisplayRulesetId;
      case "EVENT":
        return (object) this.m_event;
      case "ID":
        return (object) this.ID;
      case "MAX_HEROES_DRAFTED":
        return (object) this.m_maxHeroesDrafted;
      case "MAX_LOSSES":
        return (object) this.m_maxLosses;
      case "MAX_WINS":
        return (object) this.m_maxWins;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "REWARD_CHEST_ID":
        return (object) this.m_rewardChestId;
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
      case 190718801:
        if (!(name == "ADVENTURE_ID"))
          break;
        this.m_adventureId = (int) val;
        break;
      case 236776447:
        if (!(name == "EVENT"))
          break;
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 544621747:
        if (!(name == "MAX_HEROES_DRAFTED"))
          break;
        this.m_maxHeroesDrafted = (int) val;
        break;
      case 693605261:
        if (!(name == "SCENARIO_ID"))
          break;
        this.m_scenarioId = (int) val;
        break;
      case 767343776:
        if (!(name == "DECK_DISPLAY_RULESET_ID"))
          break;
        this.m_deckDisplayRulesetId = (int) val;
        break;
      case 807866572:
        if (!(name == "REWARD_CHEST_ID"))
          break;
        this.m_rewardChestId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1831643509:
        if (!(name == "MAX_WINS"))
          break;
        this.m_maxWins = (int) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 4242835337:
        if (!(name == "MAX_LOSSES"))
          break;
        this.m_maxLosses = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return typeof (int);
      case "DECK_DISPLAY_RULESET_ID":
        return typeof (int);
      case "EVENT":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "MAX_HEROES_DRAFTED":
        return typeof (int);
      case "MAX_LOSSES":
        return typeof (int);
      case "MAX_WINS":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "REWARD_CHEST_ID":
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
    LoadPvpdrSeasonDbfRecords loadRecords = new LoadPvpdrSeasonDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    PvpdrSeasonDbfAsset pvpdrSeasonDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (PvpdrSeasonDbfAsset)) as PvpdrSeasonDbfAsset;
    if ((UnityEngine.Object) pvpdrSeasonDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("PvpdrSeasonDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < pvpdrSeasonDbfAsset.Records.Count; ++index)
      pvpdrSeasonDbfAsset.Records[index].StripUnusedLocales();
    records = pvpdrSeasonDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
