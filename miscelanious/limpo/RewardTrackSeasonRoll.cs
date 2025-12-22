using Assets;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RewardTrackSeasonRoll : MonoBehaviour
{
  public static readonly AssetReference REWARD_TRACK_SEASON_ROLL_PREFAB = new AssetReference("RewardTrackSeasonRoll.prefab:896a446794e9b334d937e067e63613b0");
  private const string CODE_HIDE_AUTO_CLAIMED_REWARDS_POPUP = "CODE_HIDE_AUTO_CLAIMED_REWARDS_POPUP";
  private const string CODE_DISMISS = "CODE_DISMISS";
  public Widget m_forgotGlobalRewardsPopupWidget;
  public Widget m_forgotBattlegroundRewardsPopupWidget;
  public Widget m_forgotEventRewardsPopupWidget;
  public Widget m_chooseOneItemPopupWidget;
  private Widget m_widget;
  private GameObject m_owner;
  private Action m_callback;
  private RewardTrackUnclaimedRewards m_rewardTrackUnclaimedNotification;
  private RewardTrackDataModel m_rewardTrackDataModel = new RewardTrackDataModel();
  private Queue<RewardTrackNodeRewardsDataModel> m_unclaimedRewardTrackNodeDataModels = new Queue<RewardTrackNodeRewardsDataModel>();
  private bool m_hasPaidTrackUnlocked;
  private Widget m_currentForgotPopup;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (eventName == "CODE_HIDE_AUTO_CLAIMED_REWARDS_POPUP")
      {
        this.ShowChooseOneRewardPickerPopup();
      }
      else
      {
        if (!(eventName == "CODE_DISMISS"))
          return;
        this.Hide();
      }
    }));
    this.m_owner = this.gameObject;
    if (!((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null) || !((UnityEngine.Object) this.transform.parent.GetComponent<WidgetInstance>() != (UnityEngine.Object) null))
      return;
    this.m_owner = this.transform.parent.gameObject;
  }

  public void Initialize(
    Action callback,
    RewardTrackUnclaimedRewards rewardTrackUnclaimedRewards)
  {
    this.m_callback = callback;
    this.m_rewardTrackUnclaimedNotification = rewardTrackUnclaimedRewards;
    RewardTrackDbfRecord record = GameDbf.RewardTrack.GetRecord(rewardTrackUnclaimedRewards.RewardTrackId);
    this.m_hasPaidTrackUnlocked = AccountLicenseMgr.Get().OwnsAccountLicense(record?.AccountLicenseRecord?.LicenseId ?? 0L);
    this.m_rewardTrackDataModel.RewardTrackId = rewardTrackUnclaimedRewards.RewardTrackId;
    this.m_rewardTrackDataModel.Name = record.Name?.GetString() ?? string.Empty;
    this.m_rewardTrackDataModel.RewardTrackType = (Global.RewardTrackType) record.RewardTrackType;
    this.m_rewardTrackDataModel.Level = int.MaxValue;
    foreach (PlayerRewardTrackLevelState levelState in rewardTrackUnclaimedRewards.UnclaimedLevel)
    {
      this.HandleUnclaimedRewardTracklevel(levelState, rewardTrackUnclaimedRewards.RewardTrackId, false);
      this.HandleUnclaimedRewardTracklevel(levelState, rewardTrackUnclaimedRewards.RewardTrackId, true);
    }
    this.m_widget.BindDataModel((IDataModel) this.m_rewardTrackDataModel);
    if (this.m_rewardTrackDataModel.RewardTrackType == Global.RewardTrackType.BATTLEGROUNDS)
      this.m_currentForgotPopup = this.m_forgotBattlegroundRewardsPopupWidget;
    else if (this.m_rewardTrackDataModel.RewardTrackType == Global.RewardTrackType.GLOBAL)
    {
      this.m_currentForgotPopup = this.m_forgotGlobalRewardsPopupWidget;
    }
    else
    {
      this.m_currentForgotPopup = this.m_forgotEventRewardsPopupWidget;
      this.m_currentForgotPopup.BindDataModel((IDataModel) RewardTrackManager.Get().GetEventDetailsFromRewardTrack(this.m_rewardTrackDataModel));
    }
  }

  public void Show()
  {
    OverlayUI.Get().AddGameObject(this.transform.parent.gameObject);
    this.m_currentForgotPopup.RegisterDoneChangingStatesListener((Action<object>) (_ =>
    {
      UIContext.GetRoot().ShowPopup(this.m_owner);
      this.m_currentForgotPopup.GetComponentInChildren<RewardTrackForgotRewardsPopup>().Show();
    }), (object) null, true, true);
  }

  public void ShowChooseOneRewardPickerPopup()
  {
    if (this.m_unclaimedRewardTrackNodeDataModels.Count == 0)
    {
      this.Hide();
    }
    else
    {
      this.m_chooseOneItemPopupWidget.gameObject.SetActive(true);
      RewardTrackNodeRewardsDataModel rewardsDataModel = this.m_unclaimedRewardTrackNodeDataModels.Dequeue();
      this.m_chooseOneItemPopupWidget.BindDataModel((IDataModel) rewardsDataModel);
      this.m_chooseOneItemPopupWidget.BindDataModel((IDataModel) rewardsDataModel.Items);
      this.m_chooseOneItemPopupWidget.RegisterDoneChangingStatesListener((Action<object>) (_ => this.m_widget.GetComponentInChildren<RewardTrackForgotRewardsPopup>().Show()), (object) null, true, true);
    }
  }

  public void Hide()
  {
    this.m_widget.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_owner);
  }

  private void OnDestroy()
  {
    UIContext.GetRoot().DismissPopup(this.m_owner);
    Action callback = this.m_callback;
    if (callback == null)
      return;
    callback();
  }

  private void HandleUnclaimedRewardTracklevel(
    PlayerRewardTrackLevelState levelState,
    int rewardTrackId,
    bool forPaidTrack)
  {
    if (!this.m_hasPaidTrackUnlocked & forPaidTrack || ProgressUtils.HasClaimedRewardTrackReward(forPaidTrack ? (Hearthstone.Progression.RewardTrack.RewardStatus) levelState.PaidRewardStatus : (Hearthstone.Progression.RewardTrack.RewardStatus) levelState.FreeRewardStatus))
      return;
    RewardTrackLevelDbfRecord trackLevelDbfRecord = GameDbf.RewardTrack.GetRecord(rewardTrackId)?.Levels.Find((Predicate<RewardTrackLevelDbfRecord>) (r => r.Level == levelState.Level));
    if (trackLevelDbfRecord == null)
    {
      Debug.LogError((object) string.Format("Reward track level asset not found for track id {0} level {1}", (object) rewardTrackId, (object) levelState.Level));
    }
    else
    {
      RewardListDbfRecord record = forPaidTrack ? trackLevelDbfRecord.PaidRewardListRecord : trackLevelDbfRecord.FreeRewardListRecord;
      if (record == null)
        return;
      if (record.ChooseOne)
      {
        this.m_unclaimedRewardTrackNodeDataModels.Enqueue(RewardTrackFactory.CreateRewardTrackNodeRewardsDataModel(record, this.m_rewardTrackDataModel, forPaidTrack, levelState, 0));
      }
      else
      {
        foreach (RewardItemDbfRecord rewardItem in record.RewardItems)
        {
          if (rewardItem.RewardType != RewardItem.RewardType.REWARD_TRACK_XP_BOOST)
            ++this.m_rewardTrackDataModel.Unclaimed;
        }
      }
    }
  }

  public static void DebugShowFakeForgotTrackRewards(int trackId = 2, int trackLevel = 50)
  {
    Widget widget = (Widget) WidgetInstance.Create((string) RewardTrackSeasonRoll.REWARD_TRACK_SEASON_ROLL_PREFAB);
    widget.RegisterReadyListener((Action<object>) (_ =>
    {
      RewardTrackSeasonRoll componentInChildren = widget.GetComponentInChildren<RewardTrackSeasonRoll>();
      RewardTrackUnclaimedRewards rewardTrackUnclaimedRewards = new RewardTrackUnclaimedRewards()
      {
        RewardTrackId = trackId
      };
      PlayerRewardTrackLevelState rewardTrackLevelState = new PlayerRewardTrackLevelState()
      {
        Level = trackLevel,
        FreeRewardStatus = 0,
        PaidRewardStatus = 2
      };
      rewardTrackUnclaimedRewards.UnclaimedLevel.Add(rewardTrackLevelState);
      componentInChildren.Initialize((Action) null, rewardTrackUnclaimedRewards);
      componentInChildren.Show();
    }), (object) null, true);
  }
}
