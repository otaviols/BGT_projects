using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScenarioGuestHeroesDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_scenarioId;
  [SerializeField]
  private int m_guestHeroId;
  [SerializeField]
  private int m_sortOrder;

  [DbfField("SCENARIO_ID")]
  public int ScenarioId => this.m_scenarioId;

  [DbfField("GUEST_HERO_ID")]
  public int GuestHeroId => this.m_guestHeroId;

  public GuestHeroDbfRecord GuestHeroRecord => GameDbf.GuestHero.GetRecord(this.m_guestHeroId);

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  public void SetScenarioId(int v) => this.m_scenarioId = v;

  public void SetGuestHeroId(int v) => this.m_guestHeroId = v;

  public void SetSortOrder(int v) => this.m_sortOrder = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "SCENARIO_ID")
      return (object) this.m_scenarioId;
    if (name == "GUEST_HERO_ID")
      return (object) this.m_guestHeroId;
    return name == "SORT_ORDER" ? (object) this.m_sortOrder : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "SCENARIO_ID"))
      {
        if (!(name == "GUEST_HERO_ID"))
        {
          if (!(name == "SORT_ORDER"))
            return;
          this.m_sortOrder = (int) val;
        }
        else
          this.m_guestHeroId = (int) val;
      }
      else
        this.m_scenarioId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "SCENARIO_ID")
      return typeof (int);
    if (name == "GUEST_HERO_ID")
      return typeof (int);
    return name == "SORT_ORDER" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadScenarioGuestHeroesDbfRecords loadRecords = new LoadScenarioGuestHeroesDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ScenarioGuestHeroesDbfAsset guestHeroesDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ScenarioGuestHeroesDbfAsset)) as ScenarioGuestHeroesDbfAsset;
    if ((UnityEngine.Object) guestHeroesDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ScenarioGuestHeroesDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < guestHeroesDbfAsset.Records.Count; ++index)
      guestHeroesDbfAsset.Records[index].StripUnusedLocales();
    records = guestHeroesDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
