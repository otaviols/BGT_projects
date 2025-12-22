using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CoinDbfRecord : DbfRecord
{
  [SerializeField]
  private bool m_enabled;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ENABLED")
      return (object) this.m_enabled;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "NAME")
      return (object) this.m_name;
    return name == "DESCRIPTION" ? (object) this.m_description : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ENABLED"))
      {
        if (!(name == "CARD_ID"))
        {
          if (!(name == "NAME"))
          {
            if (!(name == "DESCRIPTION"))
              return;
            this.m_description = (DbfLocValue) val;
          }
          else
            this.m_name = (DbfLocValue) val;
        }
        else
          this.m_cardId = (int) val;
      }
      else
        this.m_enabled = (bool) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "ENABLED")
      return typeof (bool);
    if (name == "CARD_ID")
      return typeof (int);
    if (name == "NAME")
      return typeof (DbfLocValue);
    return name == "DESCRIPTION" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCoinDbfRecords loadRecords = new LoadCoinDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CoinDbfAsset coinDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CoinDbfAsset)) as CoinDbfAsset;
    if ((UnityEngine.Object) coinDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CoinDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < coinDbfAsset.Records.Count; ++index)
      coinDbfAsset.Records[index].StripUnusedLocales();
    records = coinDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
