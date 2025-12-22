using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LuckyDrawRewardsDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_luckyDrawBoxId;
  [SerializeField]
  private int m_rewardListId;
  [SerializeField]
  private LuckyDrawRewards.LuckyDrawStyle m_style;

  [DbfField("REWARD_LIST_ID")]
  public int RewardListId => this.m_rewardListId;

  public RewardListDbfRecord RewardListRecord => GameDbf.RewardList.GetRecord(this.m_rewardListId);

  [DbfField("STYLE")]
  public LuckyDrawRewards.LuckyDrawStyle Style => this.m_style;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LUCKY_DRAW_BOX_ID")
      return (object) this.m_luckyDrawBoxId;
    if (name == "REWARD_LIST_ID")
      return (object) this.m_rewardListId;
    return name == "STYLE" ? (object) this.m_style : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LUCKY_DRAW_BOX_ID"))
      {
        if (!(name == "REWARD_LIST_ID"))
        {
          if (!(name == "STYLE"))
            return;
          switch (val)
          {
            case null:
              this.m_style = LuckyDrawRewards.LuckyDrawStyle.COMMON;
              break;
            case LuckyDrawRewards.LuckyDrawStyle _:
            case int _:
              this.m_style = (LuckyDrawRewards.LuckyDrawStyle) val;
              break;
            case string _:
              this.m_style = LuckyDrawRewards.ParseLuckyDrawStyleValue((string) val);
              break;
          }
        }
        else
          this.m_rewardListId = (int) val;
      }
      else
        this.m_luckyDrawBoxId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LUCKY_DRAW_BOX_ID")
      return typeof (int);
    if (name == "REWARD_LIST_ID")
      return typeof (int);
    return name == "STYLE" ? typeof (LuckyDrawRewards.LuckyDrawStyle) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLuckyDrawRewardsDbfRecords loadRecords = new LoadLuckyDrawRewardsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LuckyDrawRewardsDbfAsset drawRewardsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LuckyDrawRewardsDbfAsset)) as LuckyDrawRewardsDbfAsset;
    if ((UnityEngine.Object) drawRewardsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LuckyDrawRewardsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < drawRewardsDbfAsset.Records.Count; ++index)
      drawRewardsDbfAsset.Records[index].StripUnusedLocales();
    records = drawRewardsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
