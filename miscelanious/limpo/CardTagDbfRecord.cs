using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardTagDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_tagId;
  [SerializeField]
  private int m_tagValue;
  [SerializeField]
  private bool m_isReferenceTag;
  [SerializeField]
  private bool m_isPowerKeywordTag;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("TAG_ID")]
  public int TagId => this.m_tagId;

  [DbfField("TAG_VALUE")]
  public int TagValue => this.m_tagValue;

  [DbfField("IS_REFERENCE_TAG")]
  public bool IsReferenceTag => this.m_isReferenceTag;

  [DbfField("IS_POWER_KEYWORD_TAG")]
  public bool IsPowerKeywordTag => this.m_isPowerKeywordTag;

  public void SetCardId(int v) => this.m_cardId = v;

  public void SetTagId(int v) => this.m_tagId = v;

  public void SetTagValue(int v) => this.m_tagValue = v;

  public override object GetVar(string name)
  {
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "TAG_ID")
      return (object) this.m_tagId;
    if (name == "TAG_VALUE")
      return (object) this.m_tagValue;
    if (name == "IS_REFERENCE_TAG")
      return (object) this.m_isReferenceTag;
    return name == "IS_POWER_KEYWORD_TAG" ? (object) this.m_isPowerKeywordTag : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "CARD_ID"))
    {
      if (!(name == "TAG_ID"))
      {
        if (!(name == "TAG_VALUE"))
        {
          if (!(name == "IS_REFERENCE_TAG"))
          {
            if (!(name == "IS_POWER_KEYWORD_TAG"))
              return;
            this.m_isPowerKeywordTag = (bool) val;
          }
          else
            this.m_isReferenceTag = (bool) val;
        }
        else
          this.m_tagValue = (int) val;
      }
      else
        this.m_tagId = (int) val;
    }
    else
      this.m_cardId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "CARD_ID")
      return typeof (int);
    if (name == "TAG_ID")
      return typeof (int);
    if (name == "TAG_VALUE")
      return typeof (int);
    if (name == "IS_REFERENCE_TAG")
      return typeof (bool);
    return name == "IS_POWER_KEYWORD_TAG" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardTagDbfRecords loadRecords = new LoadCardTagDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardTagDbfAsset cardTagDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardTagDbfAsset)) as CardTagDbfAsset;
    if ((UnityEngine.Object) cardTagDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardTagDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardTagDbfAsset.Records.Count; ++index)
      cardTagDbfAsset.Records[index].StripUnusedLocales();
    records = cardTagDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
