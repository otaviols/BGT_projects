using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DraftContentDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_slot;
  [SerializeField]
  private int m_deckId;
  [SerializeField]
  private DraftContent.SlotType m_slotType;

  public override object GetVar(string name)
  {
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "SLOT")
      return (object) this.m_slot;
    if (name == "DECK_ID")
      return (object) this.m_deckId;
    return name == "SLOT_TYPE" ? (object) this.m_slotType : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "NOTE_DESC"))
    {
      if (!(name == "SLOT"))
      {
        if (!(name == "DECK_ID"))
        {
          if (!(name == "SLOT_TYPE"))
            return;
          switch (val)
          {
            case null:
              this.m_slotType = DraftContent.SlotType.NONE;
              break;
            case DraftContent.SlotType _:
            case int _:
              this.m_slotType = (DraftContent.SlotType) val;
              break;
            case string _:
              this.m_slotType = DraftContent.ParseSlotTypeValue((string) val);
              break;
          }
        }
        else
          this.m_deckId = (int) val;
      }
      else
        this.m_slot = (int) val;
    }
    else
      this.m_noteDesc = (string) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "NOTE_DESC")
      return typeof (string);
    if (name == "SLOT")
      return typeof (int);
    if (name == "DECK_ID")
      return typeof (int);
    return name == "SLOT_TYPE" ? typeof (DraftContent.SlotType) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDraftContentDbfRecords loadRecords = new LoadDraftContentDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DraftContentDbfAsset draftContentDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DraftContentDbfAsset)) as DraftContentDbfAsset;
    if ((UnityEngine.Object) draftContentDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DraftContentDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < draftContentDbfAsset.Records.Count; ++index)
      draftContentDbfAsset.Records[index].StripUnusedLocales();
    records = draftContentDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
