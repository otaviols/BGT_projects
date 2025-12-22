using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardListDbfRecord : DbfRecord
{
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private bool m_chooseOne;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("CHOOSE_ONE")]
  public bool ChooseOne => this.m_chooseOne;

  public List<RewardItemDbfRecord> RewardItems
  {
    get
    {
      int id = this.ID;
      List<RewardItemDbfRecord> rewardItems = new List<RewardItemDbfRecord>();
      List<RewardItemDbfRecord> records = GameDbf.RewardItem.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        RewardItemDbfRecord rewardItemDbfRecord = records[index];
        if (rewardItemDbfRecord.RewardListId == id)
          rewardItems.Add(rewardItemDbfRecord);
      }
      return rewardItems;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "DESCRIPTION")
      return (object) this.m_description;
    return name == "CHOOSE_ONE" ? (object) this.m_chooseOne : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "DESCRIPTION"))
      {
        if (!(name == "CHOOSE_ONE"))
          return;
        this.m_chooseOne = (bool) val;
      }
      else
        this.m_description = (DbfLocValue) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "DESCRIPTION")
      return typeof (DbfLocValue);
    return name == "CHOOSE_ONE" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRewardListDbfRecords loadRecords = new LoadRewardListDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RewardListDbfAsset rewardListDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RewardListDbfAsset)) as RewardListDbfAsset;
    if ((UnityEngine.Object) rewardListDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RewardListDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardListDbfAsset.Records.Count; ++index)
      rewardListDbfAsset.Records[index].StripUnusedLocales();
    records = rewardListDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_description.StripUnusedLocales();
}
