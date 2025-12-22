using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardTrackLevelDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_rewardTrackId;
  [SerializeField]
  private int m_level;
  [SerializeField]
  private int m_xpNeeded;
  [SerializeField]
  private string m_styleName;
  [SerializeField]
  private int m_freeRewardListId;
  [SerializeField]
  private int m_paidRewardListId;

  [DbfField("REWARD_TRACK_ID")]
  public int RewardTrackId => this.m_rewardTrackId;

  [DbfField("LEVEL")]
  public int Level => this.m_level;

  [DbfField("XP_NEEDED")]
  public int XpNeeded => this.m_xpNeeded;

  [DbfField("STYLE_NAME")]
  public string StyleName => this.m_styleName;

  [DbfField("FREE_REWARD_LIST")]
  public int FreeRewardList => this.m_freeRewardListId;

  public RewardListDbfRecord FreeRewardListRecord => GameDbf.RewardList.GetRecord(this.m_freeRewardListId);

  [DbfField("PAID_REWARD_LIST")]
  public int PaidRewardList => this.m_paidRewardListId;

  public RewardListDbfRecord PaidRewardListRecord => GameDbf.RewardList.GetRecord(this.m_paidRewardListId);

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "FREE_REWARD_LIST":
        return (object) this.m_freeRewardListId;
      case "ID":
        return (object) this.ID;
      case "LEVEL":
        return (object) this.m_level;
      case "PAID_REWARD_LIST":
        return (object) this.m_paidRewardListId;
      case "REWARD_TRACK_ID":
        return (object) this.m_rewardTrackId;
      case "STYLE_NAME":
        return (object) this.m_styleName;
      case "XP_NEEDED":
        return (object) this.m_xpNeeded;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 91504271:
        if (!(name == "XP_NEEDED"))
          break;
        this.m_xpNeeded = (int) val;
        break;
      case 258433776:
        if (!(name == "STYLE_NAME"))
          break;
        this.m_styleName = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2129446653:
        if (!(name == "LEVEL"))
          break;
        this.m_level = (int) val;
        break;
      case 2932297230:
        if (!(name == "REWARD_TRACK_ID"))
          break;
        this.m_rewardTrackId = (int) val;
        break;
      case 3512786996:
        if (!(name == "FREE_REWARD_LIST"))
          break;
        this.m_freeRewardListId = (int) val;
        break;
      case 3839112424:
        if (!(name == "PAID_REWARD_LIST"))
          break;
        this.m_paidRewardListId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "FREE_REWARD_LIST":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "LEVEL":
        return typeof (int);
      case "PAID_REWARD_LIST":
        return typeof (int);
      case "REWARD_TRACK_ID":
        return typeof (int);
      case "STYLE_NAME":
        return typeof (string);
      case "XP_NEEDED":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRewardTrackLevelDbfRecords loadRecords = new LoadRewardTrackLevelDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RewardTrackLevelDbfAsset trackLevelDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RewardTrackLevelDbfAsset)) as RewardTrackLevelDbfAsset;
    if ((UnityEngine.Object) trackLevelDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RewardTrackLevelDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < trackLevelDbfAsset.Records.Count; ++index)
      trackLevelDbfAsset.Records[index].StripUnusedLocales();
    records = trackLevelDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
