using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardChestDbfRecord : DbfRecord
{
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private bool m_showToReturningPlayer;
  [SerializeField]
  private string m_chestPrefab;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("SHOW_TO_RETURNING_PLAYER")]
  public bool ShowToReturningPlayer => this.m_showToReturningPlayer;

  [DbfField("CHEST_PREFAB")]
  public string ChestPrefab => this.m_chestPrefab;

  public void SetShowToReturningPlayer(bool v) => this.m_showToReturningPlayer = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NAME")
      return (object) this.m_name;
    if (name == "DESCRIPTION")
      return (object) this.m_description;
    if (name == "SHOW_TO_RETURNING_PLAYER")
      return (object) this.m_showToReturningPlayer;
    return name == "CHEST_PREFAB" ? (object) this.m_chestPrefab : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NAME"))
      {
        if (!(name == "DESCRIPTION"))
        {
          if (!(name == "SHOW_TO_RETURNING_PLAYER"))
          {
            if (!(name == "CHEST_PREFAB"))
              return;
            this.m_chestPrefab = (string) val;
          }
          else
            this.m_showToReturningPlayer = (bool) val;
        }
        else
          this.m_description = (DbfLocValue) val;
      }
      else
        this.m_name = (DbfLocValue) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "NAME")
      return typeof (DbfLocValue);
    if (name == "DESCRIPTION")
      return typeof (DbfLocValue);
    if (name == "SHOW_TO_RETURNING_PLAYER")
      return typeof (bool);
    return name == "CHEST_PREFAB" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRewardChestDbfRecords loadRecords = new LoadRewardChestDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RewardChestDbfAsset rewardChestDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RewardChestDbfAsset)) as RewardChestDbfAsset;
    if ((UnityEngine.Object) rewardChestDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RewardChestDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardChestDbfAsset.Records.Count; ++index)
      rewardChestDbfAsset.Records[index].StripUnusedLocales();
    records = rewardChestDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
