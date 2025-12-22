using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestModifierDbfRecord : DbfRecord
{
  [SerializeField]
  private SpecialEventType m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent("none");
  [SerializeField]
  private int m_quota;
  [SerializeField]
  private string m_description;
  [SerializeField]
  private string m_styleName;
  [SerializeField]
  private DbfLocValue m_questName;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "EVENT")
      return (object) this.m_event;
    if (name == "QUOTA")
      return (object) this.m_quota;
    if (name == "DESCRIPTION")
      return (object) this.m_description;
    if (name == "STYLE_NAME")
      return (object) this.m_styleName;
    return name == "QUEST_NAME" ? (object) this.m_questName : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "EVENT"))
      {
        if (!(name == "QUOTA"))
        {
          if (!(name == "DESCRIPTION"))
          {
            if (!(name == "STYLE_NAME"))
            {
              if (!(name == "QUEST_NAME"))
                return;
              this.m_questName = (DbfLocValue) val;
            }
            else
              this.m_styleName = (string) val;
          }
          else
            this.m_description = (string) val;
        }
        else
          this.m_quota = (int) val;
      }
      else
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "EVENT")
      return typeof (string);
    if (name == "QUOTA")
      return typeof (int);
    if (name == "DESCRIPTION")
      return typeof (string);
    if (name == "STYLE_NAME")
      return typeof (string);
    return name == "QUEST_NAME" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadQuestModifierDbfRecords loadRecords = new LoadQuestModifierDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    QuestModifierDbfAsset modifierDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (QuestModifierDbfAsset)) as QuestModifierDbfAsset;
    if ((UnityEngine.Object) modifierDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("QuestModifierDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < modifierDbfAsset.Records.Count; ++index)
      modifierDbfAsset.Records[index].StripUnusedLocales();
    records = modifierDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_questName.StripUnusedLocales();
}
