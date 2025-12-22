using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardSetSpellOverrideDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardSetId;
  [SerializeField]
  private string m_spellType = "NONE";
  [SerializeField]
  private string m_overridePrefab;

  [DbfField("CARD_SET_ID")]
  public int CardSetId => this.m_cardSetId;

  [DbfField("SPELL_TYPE")]
  public string SpellType => this.m_spellType;

  [DbfField("OVERRIDE_PREFAB")]
  public string OverridePrefab => this.m_overridePrefab;

  public override object GetVar(string name)
  {
    if (name == "CARD_SET_ID")
      return (object) this.m_cardSetId;
    if (name == "SPELL_TYPE")
      return (object) this.m_spellType;
    return name == "OVERRIDE_PREFAB" ? (object) this.m_overridePrefab : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "CARD_SET_ID"))
    {
      if (!(name == "SPELL_TYPE"))
      {
        if (!(name == "OVERRIDE_PREFAB"))
          return;
        this.m_overridePrefab = (string) val;
      }
      else
        this.m_spellType = (string) val;
    }
    else
      this.m_cardSetId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "CARD_SET_ID")
      return typeof (int);
    if (name == "SPELL_TYPE")
      return typeof (string);
    return name == "OVERRIDE_PREFAB" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardSetSpellOverrideDbfRecords loadRecords = new LoadCardSetSpellOverrideDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardSetSpellOverrideDbfAsset overrideDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardSetSpellOverrideDbfAsset)) as CardSetSpellOverrideDbfAsset;
    if ((UnityEngine.Object) overrideDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardSetSpellOverrideDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < overrideDbfAsset.Records.Count; ++index)
      overrideDbfAsset.Records[index].StripUnusedLocales();
    records = overrideDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
