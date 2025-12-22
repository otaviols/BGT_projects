using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InitCardValueDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_premium;
  [SerializeField]
  private int m_rarityId;
  [SerializeField]
  private int m_buy;
  [SerializeField]
  private int m_sell;
  [SerializeField]
  private int m_upgrade;

  [DbfField("PREMIUM")]
  public int Premium => this.m_premium;

  [DbfField("RARITY")]
  public int Rarity => this.m_rarityId;

  [DbfField("BUY")]
  public int Buy => this.m_buy;

  [DbfField("SELL")]
  public int Sell => this.m_sell;

  [DbfField("UPGRADE")]
  public int Upgrade => this.m_upgrade;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "PREMIUM")
      return (object) this.m_premium;
    if (name == "RARITY")
      return (object) this.m_rarityId;
    if (name == "BUY")
      return (object) this.m_buy;
    if (name == "SELL")
      return (object) this.m_sell;
    return name == "UPGRADE" ? (object) this.m_upgrade : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "PREMIUM"))
      {
        if (!(name == "RARITY"))
        {
          if (!(name == "BUY"))
          {
            if (!(name == "SELL"))
            {
              if (!(name == "UPGRADE"))
                return;
              this.m_upgrade = (int) val;
            }
            else
              this.m_sell = (int) val;
          }
          else
            this.m_buy = (int) val;
        }
        else
          this.m_rarityId = (int) val;
      }
      else
        this.m_premium = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "PREMIUM")
      return typeof (int);
    if (name == "RARITY")
      return typeof (int);
    if (name == "BUY")
      return typeof (int);
    if (name == "SELL")
      return typeof (int);
    return name == "UPGRADE" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadInitCardValueDbfRecords loadRecords = new LoadInitCardValueDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    InitCardValueDbfAsset cardValueDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (InitCardValueDbfAsset)) as InitCardValueDbfAsset;
    if ((UnityEngine.Object) cardValueDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("InitCardValueDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardValueDbfAsset.Records.Count; ++index)
      cardValueDbfAsset.Records[index].StripUnusedLocales();
    records = cardValueDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
