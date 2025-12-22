using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardSetTimingDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_cardSetId;
  [SerializeField]
  private SpecialEventType m_eventTimingEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("CARD_SET_ID")]
  public int CardSetId => this.m_cardSetId;

  [DbfField("EVENT_TIMING_EVENT")]
  public SpecialEventType EventTimingEvent => this.m_eventTimingEvent;

  public override object GetVar(string name)
  {
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "CARD_SET_ID")
      return (object) this.m_cardSetId;
    return name == "EVENT_TIMING_EVENT" ? (object) this.m_eventTimingEvent : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "CARD_ID"))
    {
      if (!(name == "CARD_SET_ID"))
      {
        if (!(name == "EVENT_TIMING_EVENT"))
          return;
        this.m_eventTimingEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
      }
      else
        this.m_cardSetId = (int) val;
    }
    else
      this.m_cardId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "CARD_ID")
      return typeof (int);
    if (name == "CARD_SET_ID")
      return typeof (int);
    return name == "EVENT_TIMING_EVENT" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardSetTimingDbfRecords loadRecords = new LoadCardSetTimingDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardSetTimingDbfAsset setTimingDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardSetTimingDbfAsset)) as CardSetTimingDbfAsset;
    if ((UnityEngine.Object) setTimingDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardSetTimingDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < setTimingDbfAsset.Records.Count; ++index)
      setTimingDbfAsset.Records[index].StripUnusedLocales();
    records = setTimingDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
