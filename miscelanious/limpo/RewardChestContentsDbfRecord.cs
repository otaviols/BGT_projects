using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardChestContentsDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_rewardChestId;
  [SerializeField]
  private int m_rewardLevel;
  [SerializeField]
  private int m_bag1;
  [SerializeField]
  private int m_bag2;
  [SerializeField]
  private int m_bag3;
  [SerializeField]
  private int m_bag4;
  [SerializeField]
  private int m_bag5;
  [SerializeField]
  private int m_bag6;
  [SerializeField]
  private string m_iconTexture;
  [SerializeField]
  private double m_iconOffsetX;
  [SerializeField]
  private double m_iconOffsetY;

  [DbfField("REWARD_CHEST_ID")]
  public int RewardChestId => this.m_rewardChestId;

  [DbfField("REWARD_LEVEL")]
  public int RewardLevel => this.m_rewardLevel;

  [DbfField("BAG1")]
  public int Bag1 => this.m_bag1;

  [DbfField("BAG2")]
  public int Bag2 => this.m_bag2;

  [DbfField("BAG3")]
  public int Bag3 => this.m_bag3;

  [DbfField("BAG4")]
  public int Bag4 => this.m_bag4;

  [DbfField("BAG5")]
  public int Bag5 => this.m_bag5;

  [DbfField("ICON_TEXTURE")]
  public string IconTexture => this.m_iconTexture;

  [DbfField("ICON_OFFSET_X")]
  public double IconOffsetX => this.m_iconOffsetX;

  [DbfField("ICON_OFFSET_Y")]
  public double IconOffsetY => this.m_iconOffsetY;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BAG1":
        return (object) this.m_bag1;
      case "BAG2":
        return (object) this.m_bag2;
      case "BAG3":
        return (object) this.m_bag3;
      case "BAG4":
        return (object) this.m_bag4;
      case "BAG5":
        return (object) this.m_bag5;
      case "BAG6":
        return (object) this.m_bag6;
      case "ICON_OFFSET_X":
        return (object) this.m_iconOffsetX;
      case "ICON_OFFSET_Y":
        return (object) this.m_iconOffsetY;
      case "ICON_TEXTURE":
        return (object) this.m_iconTexture;
      case "ID":
        return (object) this.ID;
      case "REWARD_CHEST_ID":
        return (object) this.m_rewardChestId;
      case "REWARD_LEVEL":
        return (object) this.m_rewardLevel;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 697381708:
        if (!(name == "ICON_TEXTURE"))
          break;
        this.m_iconTexture = (string) val;
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
      case 2911217546:
        if (!(name == "ICON_OFFSET_Y"))
          break;
        this.m_iconOffsetY = (double) val;
        break;
      case 2927995165:
        if (!(name == "ICON_OFFSET_X"))
          break;
        this.m_iconOffsetX = (double) val;
        break;
      case 3309372896:
        if (!(name == "BAG1"))
          break;
        this.m_bag1 = (int) val;
        break;
      case 3342928134:
        if (!(name == "BAG3"))
          break;
        this.m_bag3 = (int) val;
        break;
      case 3359705753:
        if (!(name == "BAG2"))
          break;
        this.m_bag2 = (int) val;
        break;
      case 3376483372:
        if (!(name == "BAG5"))
          break;
        this.m_bag5 = (int) val;
        break;
      case 3393260991:
        if (!(name == "BAG4"))
          break;
        this.m_bag4 = (int) val;
        break;
      case 3426816229:
        if (!(name == "BAG6"))
          break;
        this.m_bag6 = (int) val;
        break;
      case 3456804607:
        if (!(name == "REWARD_LEVEL"))
          break;
        this.m_rewardLevel = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "BAG1":
        return typeof (int);
      case "BAG2":
        return typeof (int);
      case "BAG3":
        return typeof (int);
      case "BAG4":
        return typeof (int);
      case "BAG5":
        return typeof (int);
      case "BAG6":
        return typeof (int);
      case "ICON_OFFSET_X":
        return typeof (double);
      case "ICON_OFFSET_Y":
        return typeof (double);
      case "ICON_TEXTURE":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "REWARD_CHEST_ID":
        return typeof (int);
      case "REWARD_LEVEL":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRewardChestContentsDbfRecords loadRecords = new LoadRewardChestContentsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RewardChestContentsDbfAsset contentsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RewardChestContentsDbfAsset)) as RewardChestContentsDbfAsset;
    if ((UnityEngine.Object) contentsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RewardChestContentsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < contentsDbfAsset.Records.Count; ++index)
      contentsDbfAsset.Records[index].StripUnusedLocales();
    records = contentsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
