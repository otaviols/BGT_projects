using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceTreasureTierDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceTreasureId;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_levelId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_TREASURE_ID")
      return (object) this.m_lettuceTreasureId;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    return name == "LEVEL_ID" ? (object) this.m_levelId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_TREASURE_ID"))
      {
        if (!(name == "CARD_ID"))
        {
          if (!(name == "LEVEL_ID"))
            return;
          this.m_levelId = (int) val;
        }
        else
          this.m_cardId = (int) val;
      }
      else
        this.m_lettuceTreasureId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_TREASURE_ID")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    return name == "LEVEL_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceTreasureTierDbfRecords loadRecords = new LoadLettuceTreasureTierDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceTreasureTierDbfAsset treasureTierDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceTreasureTierDbfAsset)) as LettuceTreasureTierDbfAsset;
    if ((UnityEngine.Object) treasureTierDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceTreasureTierDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < treasureTierDbfAsset.Records.Count; ++index)
      treasureTierDbfAsset.Records[index].StripUnusedLocales();
    records = treasureTierDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
