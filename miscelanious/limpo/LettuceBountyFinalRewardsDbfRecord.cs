using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceBountyFinalRewardsDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceBountyId;
  [SerializeField]
  private int m_rewardMercenaryId;

  [DbfField("LETTUCE_BOUNTY_ID")]
  public int LettuceBountyId => this.m_lettuceBountyId;

  [DbfField("REWARD_MERCENARY_ID")]
  public int RewardMercenaryId => this.m_rewardMercenaryId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_BOUNTY_ID")
      return (object) this.m_lettuceBountyId;
    return name == "REWARD_MERCENARY_ID" ? (object) this.m_rewardMercenaryId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_BOUNTY_ID"))
      {
        if (!(name == "REWARD_MERCENARY_ID"))
          return;
        this.m_rewardMercenaryId = (int) val;
      }
      else
        this.m_lettuceBountyId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_BOUNTY_ID")
      return typeof (int);
    return name == "REWARD_MERCENARY_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceBountyFinalRewardsDbfRecords loadRecords = new LoadLettuceBountyFinalRewardsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceBountyFinalRewardsDbfAsset finalRewardsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceBountyFinalRewardsDbfAsset)) as LettuceBountyFinalRewardsDbfAsset;
    if ((UnityEngine.Object) finalRewardsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceBountyFinalRewardsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < finalRewardsDbfAsset.Records.Count; ++index)
      finalRewardsDbfAsset.Records[index].StripUnusedLocales();
    records = finalRewardsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
