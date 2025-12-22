using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerDbfRecord : DbfRecord
{
  [SerializeField]
  private Trigger.Triggertype m_triggerType = Trigger.ParseTriggertypeValue("lua");

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "TRIGGER_TYPE" ? (object) this.m_triggerType : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "TRIGGER_TYPE"))
        return;
      switch (val)
      {
        case null:
          this.m_triggerType = Trigger.Triggertype.LUA;
          break;
        case Trigger.Triggertype _:
        case int _:
          this.m_triggerType = (Trigger.Triggertype) val;
          break;
        case string _:
          this.m_triggerType = Trigger.ParseTriggertypeValue((string) val);
          break;
      }
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "TRIGGER_TYPE" ? typeof (Trigger.Triggertype) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadTriggerDbfRecords loadRecords = new LoadTriggerDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    TriggerDbfAsset triggerDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (TriggerDbfAsset)) as TriggerDbfAsset;
    if ((UnityEngine.Object) triggerDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("TriggerDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < triggerDbfAsset.Records.Count; ++index)
      triggerDbfAsset.Records[index].StripUnusedLocales();
    records = triggerDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
