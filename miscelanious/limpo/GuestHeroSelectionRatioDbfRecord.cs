using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GuestHeroSelectionRatioDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_pvpdrSeasonId;
  [SerializeField]
  private int m_guestHeroId;
  [SerializeField]
  private double m_weight;

  [DbfField("PVPDR_SEASON_ID")]
  public int PvpdrSeasonId => this.m_pvpdrSeasonId;

  [DbfField("GUEST_HERO_ID")]
  public int GuestHeroId => this.m_guestHeroId;

  public GuestHeroDbfRecord GuestHeroRecord => GameDbf.GuestHero.GetRecord(this.m_guestHeroId);

  [DbfField("WEIGHT")]
  public double Weight => this.m_weight;

  public void SetPvpdrSeasonId(int v) => this.m_pvpdrSeasonId = v;

  public void SetGuestHeroId(int v) => this.m_guestHeroId = v;

  public void SetWeight(double v) => this.m_weight = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "PVPDR_SEASON_ID")
      return (object) this.m_pvpdrSeasonId;
    if (name == "GUEST_HERO_ID")
      return (object) this.m_guestHeroId;
    return name == "WEIGHT" ? (object) this.m_weight : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "PVPDR_SEASON_ID"))
      {
        if (!(name == "GUEST_HERO_ID"))
        {
          if (!(name == "WEIGHT"))
            return;
          this.m_weight = (double) val;
        }
        else
          this.m_guestHeroId = (int) val;
      }
      else
        this.m_pvpdrSeasonId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "PVPDR_SEASON_ID")
      return typeof (int);
    if (name == "GUEST_HERO_ID")
      return typeof (int);
    return name == "WEIGHT" ? typeof (double) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadGuestHeroSelectionRatioDbfRecords loadRecords = new LoadGuestHeroSelectionRatioDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    GuestHeroSelectionRatioDbfAsset selectionRatioDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (GuestHeroSelectionRatioDbfAsset)) as GuestHeroSelectionRatioDbfAsset;
    if ((UnityEngine.Object) selectionRatioDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("GuestHeroSelectionRatioDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < selectionRatioDbfAsset.Records.Count; ++index)
      selectionRatioDbfAsset.Records[index].StripUnusedLocales();
    records = selectionRatioDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
