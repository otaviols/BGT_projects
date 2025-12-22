using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterDialogDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_onCompleteBannerId;
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private bool m_ignorePopups = true;
  [SerializeField]
  private bool m_blockInput;
  [SerializeField]
  private bool m_deferOnComplete = true;

  [DbfField("ON_COMPLETE_BANNER_ID")]
  public int OnCompleteBannerId => this.m_onCompleteBannerId;

  [DbfField("IGNORE_POPUPS")]
  public bool IgnorePopups => this.m_ignorePopups;

  [DbfField("BLOCK_INPUT")]
  public bool BlockInput => this.m_blockInput;

  [DbfField("DEFER_ON_COMPLETE")]
  public bool DeferOnComplete => this.m_deferOnComplete;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ON_COMPLETE_BANNER_ID")
      return (object) this.m_onCompleteBannerId;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "IGNORE_POPUPS")
      return (object) this.m_ignorePopups;
    if (name == "BLOCK_INPUT")
      return (object) this.m_blockInput;
    return name == "DEFER_ON_COMPLETE" ? (object) this.m_deferOnComplete : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ON_COMPLETE_BANNER_ID"))
      {
        if (!(name == "NOTE_DESC"))
        {
          if (!(name == "IGNORE_POPUPS"))
          {
            if (!(name == "BLOCK_INPUT"))
            {
              if (!(name == "DEFER_ON_COMPLETE"))
                return;
              this.m_deferOnComplete = (bool) val;
            }
            else
              this.m_blockInput = (bool) val;
          }
          else
            this.m_ignorePopups = (bool) val;
        }
        else
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
    if (name == "NOTE_DESC")
      return typeof (string);
    if (name == "IGNORE_POPUPS")
      return typeof (bool);
    if (name == "BLOCK_INPUT")
      return typeof (bool);
    return name == "DEFER_ON_COMPLETE" ? typeof (bool) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCharacterDialogDbfRecords loadRecords = new LoadCharacterDialogDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CharacterDialogDbfAsset characterDialogDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CharacterDialogDbfAsset)) as CharacterDialogDbfAsset;
    if ((UnityEngine.Object) characterDialogDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CharacterDialogDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < characterDialogDbfAsset.Records.Count; ++index)
      characterDialogDbfAsset.Records[index].StripUnusedLocales();
    records = characterDialogDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
