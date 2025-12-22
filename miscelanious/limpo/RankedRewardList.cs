using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RankedRewardList : MonoBehaviour
{
  public UberText m_cardBackProgressText;
  public RewardListAutoScroller m_autoScroller;
  [SerializeField]
  private List<Widget> m_boosterRewards;
  private Widget m_widget;
  private int m_currentRankSectionIndex;

  private void Awake() => this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();

  public void Initialize(MedalInfoTranslator mit)
  {
    if (mit == null)
      return;
    bool isTooltipEnabled = false;
    bool hasEarnedCardBack = mit.HasEarnedSeasonCardBack();
    RankedPlayDataModel dataModel1 = mit.CreateDataModel(mit.GetBestCurrentRankFormatType(), RankedMedal.DisplayMode.Default, isTooltipEnabled, hasEarnedCardBack);
    this.m_widget.BindDataModel((IDataModel) dataModel1);
    this.m_cardBackProgressText.Text = GameStrings.Format("GLUE_RANKED_REWARD_LIST_CARDBACK_PROGRESS", (object) mit.GetSeasonCardBackWinsRemaining());
    int currentSeasonId = mit.GetCurrentSeasonId();
    this.m_widget.BindDataModel((IDataModel) new CardBackDataModel()
    {
      CardBackId = RankMgr.Get().GetRankedCardBackIdForSeasonId(currentSeasonId)
    });
    PackDataModel packDataModel = new PackDataModel()
    {
      Type = (BoosterDbId) RankMgr.Get().GetRankedRewardBoosterIdForSeasonId(currentSeasonId),
      Quantity = 1
    };
    foreach (Widget boosterReward in this.m_boosterRewards)
      boosterReward.BindDataModel((IDataModel) packDataModel);
    TranslatedMedalInfo currentMedal = mit.GetCurrentMedal(mit.GetBestCurrentRankFormatType());
    List<LeagueRankDbfRecord> ranks = currentMedal.LeagueConfig.Ranks;
    ranks.Sort((Comparison<LeagueRankDbfRecord>) ((a, b) => a.StarLevel - b.StarLevel));
    this.m_currentRankSectionIndex = -1;
    RankedPlayListDataModel playListDataModel = new RankedPlayListDataModel();
    foreach (LeagueRankDbfRecord leagueRankDbfRecord in ranks)
    {
      if (leagueRankDbfRecord.RewardBagId != 0)
      {
        RankedPlayDataModel dataModel2 = MedalInfoTranslator.CreateTranslatedMedalInfo(currentMedal.format, currentMedal.leagueId, leagueRankDbfRecord.StarLevel, 0).CreateDataModel(RankedMedal.DisplayMode.Chest);
        playListDataModel.Items.Add(dataModel2);
        if (dataModel1.StarLevel >= leagueRankDbfRecord.StarLevel)
          ++this.m_currentRankSectionIndex;
      }
    }
    this.m_widget.BindDataModel((IDataModel) playListDataModel);
    this.m_autoScroller.Init(this.m_widget, this.m_currentRankSectionIndex);
  }
}
