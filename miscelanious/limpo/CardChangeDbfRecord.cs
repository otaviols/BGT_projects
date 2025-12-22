using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardChangeDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_tagId;
  [SerializeField]
  private Assets.CardChange.ChangeType m_changeType = Assets.CardChange.ParseChangeTypeValue("Invalid");
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private SpecialEventType m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("TAG_ID")]
  public int TagId => this.m_tagId;

  [DbfField("CHANGE_TYPE")]
  public Assets.CardChange.ChangeType ChangeType => this.m_changeType;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  public override object GetVar(string name)
  {
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "TAG_ID")
      return (object) this.m_tagId;
    if (name == "CHANGE_TYPE")
      return (object) this.m_changeType;
    if (name == "SORT_ORDER")
      return (object) this.m_sortOrder;
    return name == "EVENT" ? (object) this.m_event : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "CARD_ID"))
    {
      if (!(name == "TAG_ID"))
      {
        if (!(name == "CHANGE_TYPE"))
        {
          if (!(name == "SORT_ORDER"))
          {
            if (!(name == "EVENT"))
              return;
            this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
          }
          else
            this.m_sortOrder = (int) val;
        }
        else
        {
          switch (val)
          {
            case null:
              this.m_changeType = Assets.CardChange.ChangeType.INVALID;
              break;
            case Assets.CardChange.ChangeType _:
            case int _:
              this.m_changeType = (Assets.CardChange.ChangeType) val;
              break;
            case string _:
              this.m_changeType = Assets.CardChange.ParseChangeTypeValue((string) val);
              break;
          }
        }
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
    if (name == "CHANGE_TYPE")
      return typeof (Assets.CardChange.ChangeType);
    if (name == "SORT_ORDER")
      return typeof (int);
    return name == "EVENT" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardChangeDbfRecords loadRecords = new LoadCardChangeDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardChangeDbfAsset cardChangeDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardChangeDbfAsset)) as CardChangeDbfAsset;
    if ((UnityEngine.Object) cardChangeDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardChangeDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardChangeDbfAsset.Records.Count; ++index)
      cardChangeDbfAsset.Records[index].StripUnusedLocales();
    records = cardChangeDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
