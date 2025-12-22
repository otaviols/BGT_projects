using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TavernBrawlTicketDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private bool m_canBeOwned;
  [SerializeField]
  private bool m_canBePurchased;
  [SerializeField]
  private DbfLocValue m_storeName;

  [DbfField("STORE_NAME")]
  public DbfLocValue StoreName => this.m_storeName;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "CAN_BE_OWNED")
      return (object) this.m_canBeOwned;
    if (name == "CAN_BE_PURCHASED")
      return (object) this.m_canBePurchased;
    return name == "STORE_NAME" ? (object) this.m_storeName : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "CAN_BE_OWNED"))
        {
          if (!(name == "CAN_BE_PURCHASED"))
          {
            if (!(name == "STORE_NAME"))
              return;
            this.m_storeName = (DbfLocValue) val;
          }
          else
            this.m_canBePurchased = (bool) val;
        }
        else
          this.m_canBeOwned = (bool) val;
      }
      else
        this.m_noteDesc = (string) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "NOTE_DESC")
      return typeof (string);
    if (name == "CAN_BE_OWNED")
      return typeof (bool);
    if (name == "CAN_BE_PURCHASED")
      return typeof (bool);
    return name == "STORE_NAME" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadTavernBrawlTicketDbfRecords loadRecords = new LoadTavernBrawlTicketDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    TavernBrawlTicketDbfAsset brawlTicketDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (TavernBrawlTicketDbfAsset)) as TavernBrawlTicketDbfAsset;
    if ((UnityEngine.Object) brawlTicketDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("TavernBrawlTicketDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < brawlTicketDbfAsset.Records.Count; ++index)
      brawlTicketDbfAsset.Records[index].StripUnusedLocales();
    records = brawlTicketDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_storeName.StripUnusedLocales();
}
