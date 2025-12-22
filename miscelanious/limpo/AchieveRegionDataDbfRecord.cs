using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchieveRegionDataDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_achieveId;
  [SerializeField]
  private int m_region;
  [SerializeField]
  private int m_rewardableLimit;
  [SerializeField]
  private double m_rewardableInterval;
  [SerializeField]
  private SpecialEventType m_progressableEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("none");
  [SerializeField]
  private SpecialEventType m_activateEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("none");

  [DbfField("ACHIEVE_ID")]
  public int AchieveId => this.m_achieveId;

  [DbfField("REGION")]
  public int Region => this.m_region;

  [DbfField("REWARDABLE_LIMIT")]
  public int RewardableLimit => this.m_rewardableLimit;

  [DbfField("REWARDABLE_INTERVAL")]
  public double RewardableInterval => this.m_rewardableInterval;

  [DbfField("PROGRESSABLE_EVENT")]
  public SpecialEventType ProgressableEvent => this.m_progressableEvent;

  [DbfField("ACTIVATE_EVENT")]
  public SpecialEventType ActivateEvent => this.m_activateEvent;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACHIEVE_ID":
        return (object) this.m_achieveId;
      case "ACTIVATE_EVENT":
        return (object) this.m_activateEvent;
      case "ID":
        return (object) this.ID;
      case "PROGRESSABLE_EVENT":
        return (object) this.m_progressableEvent;
      case "REGION":
        return (object) this.m_region;
      case "REWARDABLE_INTERVAL":
        return (object) this.m_rewardableInterval;
      case "REWARDABLE_LIMIT":
        return (object) this.m_rewardableLimit;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 655212868:
        if (!(name == "ACHIEVE_ID"))
          break;
        this.m_achieveId = (int) val;
        break;
      case 1046164302:
        if (!(name == "REWARDABLE_LIMIT"))
          break;
        this.m_rewardableLimit = (int) val;
        break;
      case 1135546125:
        if (!(name == "PROGRESSABLE_EVENT"))
          break;
        this.m_progressableEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1454802138:
        if (!(name == "REWARDABLE_INTERVAL"))
          break;
        this.m_rewardableInterval = (double) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2317380439:
        if (!(name == "ACTIVATE_EVENT"))
          break;
        this.m_activateEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 3781468093:
        if (!(name == "REGION"))
          break;
        this.m_region = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACHIEVE_ID":
        return typeof (int);
      case "ACTIVATE_EVENT":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "PROGRESSABLE_EVENT":
        return typeof (string);
      case "REGION":
        return typeof (int);
      case "REWARDABLE_INTERVAL":
        return typeof (double);
      case "REWARDABLE_LIMIT":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAchieveRegionDataDbfRecords loadRecords = new LoadAchieveRegionDataDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AchieveRegionDataDbfAsset regionDataDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AchieveRegionDataDbfAsset)) as AchieveRegionDataDbfAsset;
    if ((UnityEngine.Object) regionDataDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AchieveRegionDataDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < regionDataDbfAsset.Records.Count; ++index)
      regionDataDbfAsset.Records[index].StripUnusedLocales();
    records = regionDataDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
