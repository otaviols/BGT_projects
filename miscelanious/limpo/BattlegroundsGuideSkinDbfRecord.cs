using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlegroundsGuideSkinDbfRecord : DbfRecord
{
  [SerializeField]
  private bool m_enabled = true;
  [SerializeField]
  private int m_rarityId = 2;
  [SerializeField]
  private int m_skinCardId;

  [DbfField("ENABLED")]
  public bool Enabled => this.m_enabled;

  [DbfField("RARITY")]
  public int Rarity => this.m_rarityId;

  [DbfField("SKIN_CARD_ID")]
  public int SkinCardId => this.m_skinCardId;

  public CardDbfRecord SkinCardRecord => GameDbf.Card.GetRecord(this.m_skinCardId);

  public void SetEnabled(bool v) => this.m_enabled = v;

  public void SetRarity(int v) => this.m_rarityId = v;

  public void SetSkinCardId(int v) => this.m_skinCardId = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ENABLED")
      return (object) this.m_enabled;
    if (name == "RARITY")
      return (object) this.m_rarityId;
    return name == "SKIN_CARD_ID" ? (object) this.m_skinCardId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ENABLED"))
      {
        if (!(name == "RARITY"))
        {
          if (!(name == "SKIN_CARD_ID"))
            return;
          this.m_skinCardId = (int) val;
        }
        else
          this.m_rarityId = (int) val;
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
    if (name == "RARITY")
      return typeof (int);
    return name == "SKIN_CARD_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBattlegroundsGuideSkinDbfRecords loadRecords = new LoadBattlegroundsGuideSkinDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BattlegroundsGuideSkinDbfAsset guideSkinDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BattlegroundsGuideSkinDbfAsset)) as BattlegroundsGuideSkinDbfAsset;
    if ((UnityEngine.Object) guideSkinDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BattlegroundsGuideSkinDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < guideSkinDbfAsset.Records.Count; ++index)
      guideSkinDbfAsset.Records[index].StripUnusedLocales();
    records = guideSkinDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
