using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BonusBountyDropChanceDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceEquipmentTierId;
  [SerializeField]
  private int m_lettuceBountyId;

  [DbfField("LETTUCE_EQUIPMENT_TIER_ID")]
  public int LettuceEquipmentTierId => this.m_lettuceEquipmentTierId;

  public LettuceBountyDbfRecord LettuceBountyRecord => GameDbf.LettuceBounty.GetRecord(this.m_lettuceBountyId);

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_EQUIPMENT_TIER_ID")
      return (object) this.m_lettuceEquipmentTierId;
    return name == "LETTUCE_BOUNTY_ID" ? (object) this.m_lettuceBountyId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_EQUIPMENT_TIER_ID"))
      {
        if (!(name == "LETTUCE_BOUNTY_ID"))
          return;
        this.m_lettuceBountyId = (int) val;
      }
      else
        this.m_lettuceEquipmentTierId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_EQUIPMENT_TIER_ID")
      return typeof (int);
    return name == "LETTUCE_BOUNTY_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBonusBountyDropChanceDbfRecords loadRecords = new LoadBonusBountyDropChanceDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BonusBountyDropChanceDbfAsset dropChanceDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BonusBountyDropChanceDbfAsset)) as BonusBountyDropChanceDbfAsset;
    if ((UnityEngine.Object) dropChanceDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BonusBountyDropChanceDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < dropChanceDbfAsset.Records.Count; ++index)
      dropChanceDbfAsset.Records[index].StripUnusedLocales();
    records = dropChanceDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
