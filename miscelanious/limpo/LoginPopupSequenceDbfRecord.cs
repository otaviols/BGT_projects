using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoginPopupSequenceDbfRecord : DbfRecord
{
  [SerializeField]
  private SpecialEventType m_eventTiming = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");

  [DbfField("EVENT_TIMING")]
  public SpecialEventType EventTiming => this.m_eventTiming;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "EVENT_TIMING" ? (object) this.m_eventTiming : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "EVENT_TIMING"))
        return;
      this.m_eventTiming = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "EVENT_TIMING" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLoginPopupSequenceDbfRecords loadRecords = new LoadLoginPopupSequenceDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LoginPopupSequenceDbfAsset sequenceDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LoginPopupSequenceDbfAsset)) as LoginPopupSequenceDbfAsset;
    if ((UnityEngine.Object) sequenceDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LoginPopupSequenceDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < sequenceDbfAsset.Records.Count; ++index)
      sequenceDbfAsset.Records[index].StripUnusedLocales();
    records = sequenceDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
