using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FixedRewardMapDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_actionId;
  [SerializeField]
  private int m_rewardId;
  [SerializeField]
  private int m_rewardCount = 1;
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private bool m_useQuestToast;
  [SerializeField]
  private string m_rewardTiming = "immediate";
  [SerializeField]
  private DbfLocValue m_toastName;
  [SerializeField]
  private DbfLocValue m_toastDescription;
  [SerializeField]
  private int m_sortOrder;

  [DbfField("ACTION_ID")]
  public int ActionId => this.m_actionId;

  public FixedRewardActionDbfRecord ActionRecord => GameDbf.FixedRewardAction.GetRecord(this.m_actionId);

  [DbfField("REWARD_ID")]
  public int RewardId => this.m_rewardId;

  public FixedRewardDbfRecord RewardRecord => GameDbf.FixedReward.GetRecord(this.m_rewardId);

  [DbfField("REWARD_COUNT")]
  public int RewardCount => this.m_rewardCount;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("USE_QUEST_TOAST")]
  public bool UseQuestToast => this.m_useQuestToast;

  [DbfField("REWARD_TIMING")]
  public string RewardTiming => this.m_rewardTiming;

  [DbfField("TOAST_NAME")]
  public DbfLocValue ToastName => this.m_toastName;

  [DbfField("TOAST_DESCRIPTION")]
  public DbfLocValue ToastDescription => this.m_toastDescription;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACTION_ID":
        return (object) this.m_actionId;
      case "ID":
        return (object) this.ID;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "REWARD_COUNT":
        return (object) this.m_rewardCount;
      case "REWARD_ID":
        return (object) this.m_rewardId;
      case "REWARD_TIMING":
        return (object) this.m_rewardTiming;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "TOAST_DESCRIPTION":
        return (object) this.m_toastDescription;
      case "TOAST_NAME":
        return (object) this.m_toastName;
      case "USE_QUEST_TOAST":
        return (object) this.m_useQuestToast;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 351069574:
        if (!(name == "REWARD_COUNT"))
          break;
        this.m_rewardCount = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1599938405:
        if (!(name == "USE_QUEST_TOAST"))
          break;
        this.m_useQuestToast = (bool) val;
        break;
      case 1736527389:
        if (!(name == "ACTION_ID"))
          break;
        this.m_actionId = (int) val;
        break;
      case 2537485753:
        if (!(name == "REWARD_TIMING"))
          break;
        this.m_rewardTiming = (string) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3384680241:
        if (!(name == "TOAST_DESCRIPTION"))
          break;
        this.m_toastDescription = (DbfLocValue) val;
        break;
      case 3599926738:
        if (!(name == "REWARD_ID"))
          break;
        this.m_rewardId = (int) val;
        break;
      case 4083666494:
        if (!(name == "TOAST_NAME"))
          break;
        this.m_toastName = (DbfLocValue) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACTION_ID":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "REWARD_COUNT":
        return typeof (int);
      case "REWARD_ID":
        return typeof (int);
      case "REWARD_TIMING":
        return typeof (string);
      case "SORT_ORDER":
        return typeof (int);
      case "TOAST_DESCRIPTION":
        return typeof (DbfLocValue);
      case "TOAST_NAME":
        return typeof (DbfLocValue);
      case "USE_QUEST_TOAST":
        return typeof (bool);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadFixedRewardMapDbfRecords loadRecords = new LoadFixedRewardMapDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    FixedRewardMapDbfAsset rewardMapDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (FixedRewardMapDbfAsset)) as FixedRewardMapDbfAsset;
    if ((UnityEngine.Object) rewardMapDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("FixedRewardMapDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardMapDbfAsset.Records.Count; ++index)
      rewardMapDbfAsset.Records[index].StripUnusedLocales();
    records = rewardMapDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_toastName.StripUnusedLocales();
    this.m_toastDescription.StripUnusedLocales();
  }
}
