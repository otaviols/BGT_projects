using Hearthstone.DataModels;
using Hearthstone.UI;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesSeasonRewardsDialog : DialogBase
{
  public AsyncReference m_chestWidgetReference;
  public Transform m_rewardBoxesBone;
  public UberText m_footer;
  private MercenariesSeasonRewardsDialog.Info m_info;
  private Widget m_chestWidget;
  private bool m_chestOpened;
  private List<RewardData> m_boxRewards;
  private Queue<RewardData> m_bannerRewards;

  private void Start() => this.m_chestWidgetReference.RegisterReadyListener<Widget>((Action<Widget>) (w => this.m_chestWidget = w));

  public void SetInfo(MercenariesSeasonRewardsDialog.Info info) => this.m_info = info;

  public override void Show() => this.StartCoroutine(this.ShowWhenReady());

  private IEnumerator ShowWhenReady()
  {
    MercenariesSeasonRewardsDialog seasonRewardsDialog = this;
    while ((UnityEngine.Object) seasonRewardsDialog.m_chestWidget == (UnityEngine.Object) null || !seasonRewardsDialog.m_chestWidget.IsReady)
      yield return (object) null;
    seasonRewardsDialog.m_chestWidget.RegisterEventListener(new Widget.EventListenerDelegate(seasonRewardsDialog.ChestEventListener));
    List<MercenariesRankedSeasonRewardRankDbfRecord> sortedRewardRecords = LettucePlayDisplay.SortedRewardRecords;
    // ISSUE: reference to a compiler-generated method
    int index1 = sortedRewardRecords.FindIndex(new Predicate<MercenariesRankedSeasonRewardRankDbfRecord>(seasonRewardsDialog.\u003CShowWhenReady\u003Eb__12_0));
    int publicRatingUnlock1 = sortedRewardRecords[index1].MinPublicRatingUnlock;
    seasonRewardsDialog.m_chestWidget.BindDataModel((IDataModel) new LettucePlayDisplayDataModel()
    {
      HighRatingTierIndex = index1,
      Rating = publicRatingUnlock1
    });
    int index2 = index1 + 1;
    if (index2 == sortedRewardRecords.Count)
    {
      seasonRewardsDialog.m_footer.gameObject.SetActive(false);
    }
    else
    {
      int publicRatingUnlock2 = sortedRewardRecords[index2].MinPublicRatingUnlock;
      seasonRewardsDialog.m_footer.Text = GameStrings.Format("GLUE_LETTUCE_SEASON_RATING_REWARD_FOOTER", (object) publicRatingUnlock2);
    }
    while (seasonRewardsDialog.m_chestWidget.IsChangingStates)
      yield return (object) null;
    LayerUtils.SetLayer(seasonRewardsDialog.m_chestWidget.gameObject, seasonRewardsDialog.gameObject.layer);
    SeasonEndDialog.FadeEffectsIn();
    // ISSUE: reference to a compiler-generated method
    seasonRewardsDialog.\u003C\u003En__0();
    seasonRewardsDialog.DoShowAnimation();
    UniversalInputManager.Get().SetGameDialogActive(true);
    SeasonEndDialog.PlayShowSound();
  }

  public override void Hide()
  {
    base.Hide();
    SeasonEndDialog.FadeEffectsOut();
    SeasonEndDialog.PlayHideSound();
  }

  private void ChestEventListener(string eventName)
  {
    if (!(eventName == "MERC_REWARD_3D_CHEST_CLICKED") || this.m_chestOpened)
      return;
    this.m_chestOpened = true;
    PlayMakerFSM componentInChildren = this.m_chestWidget.GetComponentInChildren<PlayMakerFSM>();
    FsmGameObject fsmGameObject = componentInChildren.FsmVariables.GetFsmGameObject("OwnerObject");
    if (fsmGameObject != null)
      fsmGameObject.Value = this.gameObject;
    componentInChildren.SendEvent("StartAnim");
  }

  private void OpenRewards()
  {
    this.m_boxRewards = new List<RewardData>(this.m_info.m_rewards.Count);
    this.m_bannerRewards = new Queue<RewardData>();
    foreach (RewardData reward in this.m_info.m_rewards)
    {
      if (reward.RewardType != Reward.Type.REWARD_ITEM && reward.RewardType != Reward.Type.MERCENARY_MERCENARY && reward.RewardType != Reward.Type.MERCENARY_KNOCKOUT && reward.RewardType != Reward.Type.MERCENARY_RANDOM_MERCENARY && reward.RewardType != Reward.Type.MERCENARY_EQUIPMENT)
        this.m_boxRewards.Add(reward);
      else
        this.m_bannerRewards.Enqueue(reward);
    }
    RewardUtils.ShowRewardBoxes(this.m_boxRewards, new Action(this.RewardBoxesDoneCallback), this.m_rewardBoxesBone, true, GameLayer.PerspectiveUI, true);
  }

  private void RewardBoxesDoneCallback()
  {
    if (this.m_bannerRewards.Count > 0)
    {
      RewardData rewardData = this.m_bannerRewards.Dequeue();
      QuestToast.ShowGenericRewardQuestToast(UserAttentionBlocker.NONE, (QuestToast.DelOnCloseQuestToast) (_ => this.RewardBoxesDoneCallback()), (object) null, rewardData, rewardData.NameOverride, rewardData.DescriptionOverride, false);
    }
    else
      this.AckAndHide();
  }

  private void AckAndHide()
  {
    Network.Get().AckNotice(this.m_info.m_noticeId);
    this.Hide();
  }

  protected override void DoShowAnimation()
  {
    this.m_showAnimState = DialogBase.ShowAnimState.IN_PROGRESS;
    AnimationUtil.ShowWithPunch(this.gameObject, this.START_SCALE, Vector3.Scale(this.PUNCH_SCALE, this.m_originalScale), this.m_originalScale, "OnShowAnimFinished", true);
  }

  protected override void OnHideAnimFinished()
  {
    UniversalInputManager.Get().SetGameDialogActive(false);
    base.OnHideAnimFinished();
    MercenariesSeasonRewardsDialog.Info info = this.m_info;
    if (info == null)
      return;
    Action doneCallback = info.m_doneCallback;
    if (doneCallback == null)
      return;
    doneCallback();
  }

  public class Info
  {
    public long m_noticeId;
    public List<RewardData> m_rewards;
    public int m_rewardAssetId;
    public Action m_doneCallback;
  }
}
