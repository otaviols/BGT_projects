using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestDialogOnCompleteDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_questDialogId;
  [SerializeField]
  private int m_playOrder;
  [SerializeField]
  private string m_prefabName;
  [SerializeField]
  private string m_audioName;
  [SerializeField]
  private bool m_altBubblePosition;
  [SerializeField]
  private double m_waitBefore;
  [SerializeField]
  private double m_waitAfter;
  [SerializeField]
  private bool m_persistPrefab;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ALT_BUBBLE_POSITION":
        return (object) this.m_altBubblePosition;
      case "AUDIO_NAME":
        return (object) this.m_audioName;
      case "ID":
        return (object) this.ID;
      case "PERSIST_PREFAB":
        return (object) this.m_persistPrefab;
      case "PLAY_ORDER":
        return (object) this.m_playOrder;
      case "PREFAB_NAME":
        return (object) this.m_prefabName;
      case "QUEST_DIALOG_ID":
        return (object) this.m_questDialogId;
      case "WAIT_AFTER":
        return (object) this.m_waitAfter;
      case "WAIT_BEFORE":
        return (object) this.m_waitBefore;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 1026926188:
        if (!(name == "PLAY_ORDER"))
          break;
        this.m_playOrder = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1788065238:
        if (!(name == "WAIT_BEFORE"))
          break;
        this.m_waitBefore = (double) val;
        break;
      case 1952853119:
        if (!(name == "ALT_BUBBLE_POSITION"))
          break;
        this.m_altBubblePosition = (bool) val;
        break;
      case 2300801615:
        if (!(name == "PREFAB_NAME"))
          break;
        this.m_prefabName = (string) val;
        break;
      case 2696302966:
        if (!(name == "PERSIST_PREFAB"))
          break;
        this.m_persistPrefab = (bool) val;
        break;
      case 3082479862:
        if (!(name == "QUEST_DIALOG_ID"))
          break;
        this.m_questDialogId = (int) val;
        break;
      case 3448897561:
        if (!(name == "AUDIO_NAME"))
          break;
        this.m_audioName = (string) val;
        break;
      case 3840082817:
        if (!(name == "WAIT_AFTER"))
          break;
        this.m_waitAfter = (double) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ALT_BUBBLE_POSITION":
        return typeof (bool);
      case "AUDIO_NAME":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "PERSIST_PREFAB":
        return typeof (bool);
      case "PLAY_ORDER":
        return typeof (int);
      case "PREFAB_NAME":
        return typeof (string);
      case "QUEST_DIALOG_ID":
        return typeof (int);
      case "WAIT_AFTER":
        return typeof (double);
      case "WAIT_BEFORE":
        return typeof (double);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadQuestDialogOnCompleteDbfRecords loadRecords = new LoadQuestDialogOnCompleteDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    QuestDialogOnCompleteDbfAsset completeDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (QuestDialogOnCompleteDbfAsset)) as QuestDialogOnCompleteDbfAsset;
    if ((UnityEngine.Object) completeDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("QuestDialogOnCompleteDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < completeDbfAsset.Records.Count; ++index)
      completeDbfAsset.Records[index].StripUnusedLocales();
    records = completeDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
