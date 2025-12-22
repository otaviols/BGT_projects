using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardRaceDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_isRaceTagId;

  [DbfField("IS_RACE_TAG")]
  public int IsRaceTag => this.m_isRaceTagId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "IS_RACE_TAG" ? (object) this.m_isRaceTagId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "IS_RACE_TAG"))
        return;
      this.m_isRaceTagId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "IS_RACE_TAG" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardRaceDbfRecords loadRecords = new LoadCardRaceDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardRaceDbfAsset cardRaceDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardRaceDbfAsset)) as CardRaceDbfAsset;
    if ((UnityEngine.Object) cardRaceDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardRaceDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardRaceDbfAsset.Records.Count; ++index)
      cardRaceDbfAsset.Records[index].StripUnusedLocales();
    records = cardRaceDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
