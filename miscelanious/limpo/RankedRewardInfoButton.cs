using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
[RequireComponent(typeof (WidgetTemplate))]
public class RankedRewardInfoButton : MonoBehaviour
{
  public Clickable m_buttonClickable;
  public UberText m_buttonText;
  [CustomEditField(Sections = "Reward List")]
  public Vector3_MobileOverride m_rewardListPos;
  [CustomEditField(Sections = "Reward List")]
  public Float_MobileOverride m_rewardListDeviceScale = new Float_MobileOverride(1f);
  [CustomEditField(Sections = "Reward List")]
  public float m_rewardListScaleSmall;
  [CustomEditField(Sections = "Reward List")]
  public float m_rewardListScaleWide;
  [CustomEditField(Sections = "Reward List")]
  public float m_rewardListScaleExtraWide;
  private Widget m_widget;
  private WidgetInstance m_rankedRewardListWidget;
  private MedalInfoTranslator m_medalInfo;
  private TranslatedMedalInfo m_currentMedal;
  private long m_lastRewardsVersionSeen;
  private bool m_isShowingRewardsList;
  private TooltipZone m_tooltipZone;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener));
    this.m_tooltipZone = this.GetComponent<TooltipZone>();
  }

  private void OnDestroy() => this.DestroyRankedRewardsList();

  public void Initialize(MedalInfoTranslator mit)
  {
    if (mit == null)
      return;
    this.m_medalInfo = mit;
    this.m_currentMedal = this.m_medalInfo.GetCurrentMedal(this.m_medalInfo.GetBestCurrentRankFormatType());
    bool isTooltipEnabled = false;
    bool hasEarnedCardBack = this.m_medalInfo.GetSeasonCardBackWinsRemaining() == 0;
    this.m_currentMedal.starLevel = this.m_currentMedal.bestStarLevel;
    RankedPlayDataModel dataModel = this.m_currentMedal.CreateDataModel(RankedMedal.DisplayMode.Chest, isTooltipEnabled, hasEarnedCardBack);
    this.m_widget.BindDataModel((IDataModel) dataModel);
    this.InitButtonText(dataModel);
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_LAST_REWARDS_VERSION_SEEN, out this.m_lastRewardsVersionSeen);
  }

  public void Show() => this.StartCoroutine(this.ShowWhenReady());

  private bool IsReady => (UnityEngine.Object) this.m_widget != (UnityEngine.Object) null && this.m_widget.IsReady && !this.m_widget.IsChangingStates;

  private IEnumerator ShowWhenReady()
  {
    while (!this.IsReady)
      yield return (object) null;
    this.m_widget.Show();
  }

  private bool HasSeenLatestRewardsVersion() => this.m_lastRewardsVersionSeen >= (long) this.m_currentMedal.LeagueConfig.RewardsVersion;

  private void WidgetEventListener(string eventName)
  {
    if (eventName.Equals("OnClickRewardQuestLogButton"))
      this.ShowRankedRewardList();
    else if (eventName.Equals("RollOver"))
    {
      this.OnRollOver();
    }
    else
    {
      if (!eventName.Equals("RollOut"))
        return;
      this.OnRollOut();
    }
  }

  private void WidgetEventListener_RewardsList(string eventName)
  {
    if (!eventName.Equals("HIDE"))
      return;
    this.HideRankedRewardsList();
  }

  private void HideRankedRewardsList()
  {
    if ((UnityEngine.Object) this.m_rankedRewardListWidget != (UnityEngine.Object) null)
      UIContext.GetRoot().DismissPopup(this.m_rankedRewardListWidget.gameObject);
    this.m_isShowingRewardsList = false;
    this.m_buttonClickable.Active = true;
  }

  private void ShowRankedRewardList()
  {
    if (this.m_isShowingRewardsList)
      return;
    this.m_isShowingRewardsList = true;
    this.m_buttonClickable.Active = false;
    if ((UnityEngine.Object) this.m_rankedRewardListWidget == (UnityEngine.Object) null)
    {
      this.m_rankedRewardListWidget = WidgetInstance.Create((string) RankMgr.RANKED_REWARD_LIST_POPUP);
      this.m_rankedRewardListWidget.WillLoadSynchronously = true;
      OverlayUI.Get().AddGameObject(this.m_rankedRewardListWidget.gameObject);
      UIContext.GetRoot().ShowPopup(this.m_rankedRewardListWidget.gameObject);
      this.m_rankedRewardListWidget.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_rewardListPos;
      this.m_rankedRewardListWidget.transform.localScale = Vector3.one * TransformUtil.GetAspectRatioDependentValue(this.m_rewardListScaleSmall, this.m_rewardListScaleWide, this.m_rewardListScaleExtraWide) * (float) (MobileOverrideValue<float>) this.m_rewardListDeviceScale;
      this.m_rankedRewardListWidget.RegisterReadyListener((Action<object>) (_ =>
      {
        RankedRewardList componentInChildren = this.m_rankedRewardListWidget.GetComponentInChildren<RankedRewardList>();
        if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
          return;
        componentInChildren.Initialize(this.m_medalInfo);
        this.m_rankedRewardListWidget.TriggerEvent("SHOW", new Widget.TriggerEventParameters());
      }), (object) null, true);
      this.m_rankedRewardListWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener_RewardsList));
    }
    else
    {
      UIContext.GetRoot().ShowPopup(this.m_rankedRewardListWidget.gameObject);
      this.m_rankedRewardListWidget.TriggerEvent("SHOW", new Widget.TriggerEventParameters());
    }
    if (this.HasSeenLatestRewardsVersion())
      return;
    this.m_lastRewardsVersionSeen = (long) this.m_currentMedal.LeagueConfig.RewardsVersion;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_LAST_REWARDS_VERSION_SEEN, new long[1]
    {
      this.m_lastRewardsVersionSeen
    }));
  }

  private void DestroyRankedRewardsList()
  {
    if ((UnityEngine.Object) this.m_rankedRewardListWidget != (UnityEngine.Object) null)
    {
      UIContext.GetRoot().DismissPopup(this.m_rankedRewardListWidget.gameObject);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_rankedRewardListWidget.gameObject);
    }
    this.m_isShowingRewardsList = false;
  }

  private void InitButtonText(RankedPlayDataModel rankedPlayDataModel)
  {
    if (!rankedPlayDataModel.HasEarnedCardBack)
      this.m_buttonText.Text = GameStrings.Format("GLUE_RANKED_REWARD_QUEST_LOG_CARDBACK_PROGRESS", (object) this.m_medalInfo.GetSeasonCardBackWinsRemaining());
    else if (rankedPlayDataModel.IsNewPlayer)
    {
      int leagueId = this.m_currentMedal.leagueId;
      int num = rankedPlayDataModel.StarLevel - 1;
      int starLevel = num - num % 5 + 5 + 1;
      if (starLevel < RankMgr.Get().GetMaxStarLevel(leagueId))
        this.m_buttonText.Text = GameStrings.Format("GLUE_RANKED_REWARD_QUEST_LOG_LABEL_RANK_REWARD", (object) RankMgr.Get().GetLeagueRankRecord(leagueId, starLevel).MedalText.GetString());
      else
        this.m_buttonText.Text = "";
    }
    else
    {
      int leagueId = this.m_currentMedal.leagueId;
      int maxStarLevel = RankMgr.Get().GetMaxStarLevel(leagueId);
      int starLevel = 1;
      LeagueRankDbfRecord leagueRankDbfRecord = (LeagueRankDbfRecord) null;
      bool flag = false;
      for (; starLevel < maxStarLevel; ++starLevel)
      {
        LeagueRankDbfRecord leagueRankRecord = RankMgr.Get().GetLeagueRankRecord(leagueId, starLevel);
        if (leagueRankRecord.RewardBagId != 0)
        {
          if (rankedPlayDataModel.StarLevel >= starLevel)
          {
            flag = true;
            break;
          }
          leagueRankDbfRecord = leagueRankRecord;
          break;
        }
      }
      if (!flag && leagueRankDbfRecord != null)
        this.m_buttonText.Text = GameStrings.Format("GLUE_RANKED_REWARD_QUEST_LOG_LABEL_RANK_REQUIRED", (object) leagueRankDbfRecord.RankName.GetString());
      else
        this.m_buttonText.Text = "";
    }
  }

  private void OnRollOver() => this.m_tooltipZone.ShowLayerTooltip(GameStrings.Get("GLOBAL_PROGRESSION_RANKED_REWARDS_TOOLTIP_TITLE"), GameStrings.Get("GLOBAL_PROGRESSION_RANKED_REWARDS_TOOLTIP"));

  private void OnRollOut() => this.m_tooltipZone.HideTooltip();
}
