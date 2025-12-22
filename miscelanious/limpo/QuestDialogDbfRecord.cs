using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestDialogDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_onCompleteBannerId;
  [SerializeField]
  private string m_noteDesc;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ON_COMPLETE_BANNER_ID")
      return (object) this.m_onCompleteBannerId;
    return name == "NOTE_DESC" ? (object) this.m_noteDesc : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ON_COMPLETE_BANNER_ID"))
      {
        if (!(name == "NOTE_DESC"))
          return;
        this.m_noteDesc = (string) val;
      }
      else
        this.m_onCompleteBannerId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "ON_COMPLETE_BANNER_ID")
      return typeof (int);
    return name == "NOTE_DESC" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadQuestDialogDbfRecords loadRecords = new LoadQuestDialogDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    QuestDialogDbfAsset questDialogDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (QuestDialogDbfAsset)) as QuestDialogDbfAsset;
    if ((UnityEngine.Object) questDialogDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("QuestDialogDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < questDialogDbfAsset.Records.Count; ++index)
      questDialogDbfAsset.Records[index].StripUnusedLocales();
    records = questDialogDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
