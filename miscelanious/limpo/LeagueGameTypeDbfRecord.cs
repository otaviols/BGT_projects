using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeagueGameTypeDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_leagueId;
  [SerializeField]
  private LeagueGameType.FormatType m_formatType;
  [SerializeField]
  private LeagueGameType.BnetGameType m_bnetGameType;

  [DbfField("LEAGUE_ID")]
  public int LeagueId => this.m_leagueId;

  [DbfField("FORMAT_TYPE")]
  public LeagueGameType.FormatType FormatType => this.m_formatType;

  [DbfField("BNET_GAME_TYPE")]
  public LeagueGameType.BnetGameType BnetGameType => this.m_bnetGameType;

  public void SetLeagueId(int v) => this.m_leagueId = v;

  public void SetFormatType(LeagueGameType.FormatType v) => this.m_formatType = v;

  public void SetBnetGameType(LeagueGameType.BnetGameType v) => this.m_bnetGameType = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LEAGUE_ID")
      return (object) this.m_leagueId;
    if (name == "FORMAT_TYPE")
      return (object) this.m_formatType;
    return name == "BNET_GAME_TYPE" ? (object) this.m_bnetGameType : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LEAGUE_ID"))
      {
        if (!(name == "FORMAT_TYPE"))
        {
          if (!(name == "BNET_GAME_TYPE"))
            return;
          switch (val)
          {
            case null:
              this.m_bnetGameType = LeagueGameType.BnetGameType.BGT_UNKNOWN;
              break;
            case LeagueGameType.BnetGameType _:
            case int _:
              this.m_bnetGameType = (LeagueGameType.BnetGameType) val;
              break;
            case string _:
              this.m_bnetGameType = LeagueGameType.ParseBnetGameTypeValue((string) val);
              break;
          }
        }
        else
        {
          switch (val)
          {
            case null:
              this.m_formatType = LeagueGameType.FormatType.FT_UNKNOWN;
              break;
            case LeagueGameType.FormatType _:
            case int _:
              this.m_formatType = (LeagueGameType.FormatType) val;
              break;
            case string _:
              this.m_formatType = LeagueGameType.ParseFormatTypeValue((string) val);
              break;
          }
        }
      }
      else
        this.m_leagueId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LEAGUE_ID")
      return typeof (int);
    if (name == "FORMAT_TYPE")
      return typeof (LeagueGameType.FormatType);
    return name == "BNET_GAME_TYPE" ? typeof (LeagueGameType.BnetGameType) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLeagueGameTypeDbfRecords loadRecords = new LoadLeagueGameTypeDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LeagueGameTypeDbfAsset gameTypeDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LeagueGameTypeDbfAsset)) as LeagueGameTypeDbfAsset;
    if ((UnityEngine.Object) gameTypeDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LeagueGameTypeDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < gameTypeDbfAsset.Records.Count; ++index)
      gameTypeDbfAsset.Records[index].StripUnusedLocales();
    records = gameTypeDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
