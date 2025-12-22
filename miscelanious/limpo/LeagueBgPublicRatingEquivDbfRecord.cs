using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeagueBgPublicRatingEquivDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_leagueId;
  [SerializeField]
  private LeagueBgPublicRatingEquiv.FormatType m_formatType;
  [SerializeField]
  private LeagueBgPublicRatingEquiv.Region m_region;
  [SerializeField]
  private int m_starLevel;
  [SerializeField]
  private int m_legendRank;
  [SerializeField]
  private int m_bgPublicRatingEquiv;

  [DbfField("LEAGUE_ID")]
  public int LeagueId => this.m_leagueId;

  [DbfField("FORMAT_TYPE")]
  public LeagueBgPublicRatingEquiv.FormatType FormatType => this.m_formatType;

  [DbfField("REGION")]
  public LeagueBgPublicRatingEquiv.Region Region => this.m_region;

  [DbfField("STAR_LEVEL")]
  public int StarLevel => this.m_starLevel;

  [DbfField("LEGEND_RANK")]
  public int LegendRank => this.m_legendRank;

  [DbfField("BG_PUBLIC_RATING_EQUIV")]
  public int BgPublicRatingEquiv => this.m_bgPublicRatingEquiv;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BG_PUBLIC_RATING_EQUIV":
        return (object) this.m_bgPublicRatingEquiv;
      case "FORMAT_TYPE":
        return (object) this.m_formatType;
      case "ID":
        return (object) this.ID;
      case "LEAGUE_ID":
        return (object) this.m_leagueId;
      case "LEGEND_RANK":
        return (object) this.m_legendRank;
      case "REGION":
        return (object) this.m_region;
      case "STAR_LEVEL":
        return (object) this.m_starLevel;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 96691247:
        if (!(name == "FORMAT_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_formatType = LeagueBgPublicRatingEquiv.FormatType.FT_UNKNOWN;
            return;
          case LeagueBgPublicRatingEquiv.FormatType _:
          case int _:
            this.m_formatType = (LeagueBgPublicRatingEquiv.FormatType) val;
            return;
          case string _:
            this.m_formatType = LeagueBgPublicRatingEquiv.ParseFormatTypeValue((string) val);
            return;
          default:
            return;
        }
      case 614329163:
        if (!(name == "LEGEND_RANK"))
          break;
        this.m_legendRank = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1655746952:
        if (!(name == "STAR_LEVEL"))
          break;
        this.m_starLevel = (int) val;
        break;
      case 3353298088:
        if (!(name == "LEAGUE_ID"))
          break;
        this.m_leagueId = (int) val;
        break;
      case 3671770999:
        if (!(name == "BG_PUBLIC_RATING_EQUIV"))
          break;
        this.m_bgPublicRatingEquiv = (int) val;
        break;
      case 3781468093:
        if (!(name == "REGION"))
          break;
        switch (val)
        {
          case null:
            this.m_region = LeagueBgPublicRatingEquiv.Region.REGION_UNKNOWN;
            return;
          case LeagueBgPublicRatingEquiv.Region _:
          case int _:
            this.m_region = (LeagueBgPublicRatingEquiv.Region) val;
            return;
          case string _:
            this.m_region = LeagueBgPublicRatingEquiv.ParseRegionValue((string) val);
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
      case "BG_PUBLIC_RATING_EQUIV":
        return typeof (int);
      case "FORMAT_TYPE":
        return typeof (LeagueBgPublicRatingEquiv.FormatType);
      case "ID":
        return typeof (int);
      case "LEAGUE_ID":
        return typeof (int);
      case "LEGEND_RANK":
        return typeof (int);
      case "REGION":
        return typeof (LeagueBgPublicRatingEquiv.Region);
      case "STAR_LEVEL":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLeagueBgPublicRatingEquivDbfRecords loadRecords = new LoadLeagueBgPublicRatingEquivDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LeagueBgPublicRatingEquivDbfAsset ratingEquivDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LeagueBgPublicRatingEquivDbfAsset)) as LeagueBgPublicRatingEquivDbfAsset;
    if ((UnityEngine.Object) ratingEquivDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LeagueBgPublicRatingEquivDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < ratingEquivDbfAsset.Records.Count; ++index)
      ratingEquivDbfAsset.Records[index].StripUnusedLocales();
    records = ratingEquivDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
