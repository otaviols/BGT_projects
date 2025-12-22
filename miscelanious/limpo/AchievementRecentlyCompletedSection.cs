using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class AchievementRecentlyCompletedSection : MonoBehaviour
{
  public const string SHOW_ACHIEVEMENT_TILE = "CODE_SHOW_ACHIEVEMENT_TILE";
  public const string HIDE_ACHIEVEMENT_TILE = "CODE_HIDE_ACHIEVEMENT_TILE";
  public const string SHOW_COMPLETION_DATE = "SHOW_COMPLETION_DATE";
  public const string START_HIDE_SEQUENCE = "START_HIDE_SEQUENCE";
  public const string HIDE_ANIMATION_COMPLETED = "CODE_HIDE_ANIMATION_COMPLETED";
  public const string CLAIM_ANIMATION_STARTED = "CODE_CLAIM_ANIMATION_STARTED";
  public const string CLAIM_ANIMATION_COMPLETED = "CODE_CLAIM_ANIMATION_COMPLETED";
  public const string SHOW_WIDGET = "SHOW";
  [SerializeField]
  private UberText m_HeaderText;
  [SerializeField]
  private int m_maxAchievementsToShow;
  private Widget m_widget;
  private Listable m_listable;
  private readonly AchievementListDataModel m_achievementListDataModel = new AchievementListDataModel();
  private int m_numAnimationsPlaying;
  private Coroutine m_updatePositionCoroutine;
  private Coroutine m_updateDisplayOrderCoroutine;

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_HeaderText.Text = GameStrings.Get("GLUE_PROGRESSION_ACHIEVEMENTS_RECENTLY_COMPLETED");
    this.m_widget.BindDataModel((IDataModel) this.m_achievementListDataModel);
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
    this.m_widget.RegisterReadyListener((Action<object>) (_ => this.m_listable = this.GetComponentInChildren<Listable>()), (object) null, true);
    AchievementManager.Get().OnStatusChanged += new AchievementManager.StatusChangedDelegate(this.OnAchievementStatusChanged);
  }

  public void OnDestroy()
  {
    AchievementManager achievementManager = AchievementManager.Get();
    if (achievementManager == null)
      return;
    achievementManager.OnStatusChanged -= new AchievementManager.StatusChangedDelegate(this.OnAchievementStatusChanged);
    this.KillCoroutines();
  }

  private void HandleEvent(string eventName)
  {
    if (!(eventName == "CODE_HIDE_ANIMATION_COMPLETED"))
    {
      if (!(eventName == "CODE_CLAIM_ANIMATION_STARTED"))
      {
        if (!(eventName == "CODE_CLAIM_ANIMATION_COMPLETED"))
        {
          if (!(eventName == "SHOW"))
            return;
          AchievementManager.Get().ClearClaimedDates();
          this.UpdateAchievementDisplayOrder();
        }
        else
          this.HandleClaimAnimationComplete();
      }
      else
        ++this.m_numAnimationsPlaying;
    }
    else
    {
      --this.m_numAnimationsPlaying;
      this.UpdateAchievementDisplayOrder();
    }
  }

  public void OnDisable()
  {
    this.KillCoroutines();
    this.m_numAnimationsPlaying = 0;
  }

  private void KillCoroutines()
  {
    if (this.m_updatePositionCoroutine != null)
      this.StopCoroutine(this.m_updatePositionCoroutine);
    if (this.m_updateDisplayOrderCoroutine == null)
      return;
    this.StopCoroutine(this.m_updateDisplayOrderCoroutine);
  }

  private void UpdateAchievementDisplayOrder()
  {
    if (this.m_updateDisplayOrderCoroutine != null)
      return;
    this.m_updateDisplayOrderCoroutine = this.StartCoroutine(this.UpdateAchievementDisplayOrderRoutine());
  }

  private IEnumerator UpdateAchievementDisplayOrderRoutine()
  {
    while (this.m_numAnimationsPlaying > 0)
      yield return (object) null;
    DataModelList<AchievementDataModel> dataModelList = AchievementManager.Get().GetRecentlyCompletedAchievements().GetCurrentSortedAchievements().SortByStatusThenClaimedDate().Take<AchievementDataModel>(this.m_maxAchievementsToShow).ToDataModelList<AchievementDataModel>();
    AchievementManager.Get().LoadRewards(dataModelList);
    this.m_achievementListDataModel.Achievements.OverwriteDataModels<AchievementDataModel>(dataModelList);
    this.m_updateDisplayOrderCoroutine = (Coroutine) null;
  }

  private void UpdateSingleAchievement(
    AchievementDataModel oldAchievement,
    AchievementDataModel newAchievement)
  {
    int index = this.m_achievementListDataModel.Achievements.IndexOf(oldAchievement);
    AchievementManager.Get().LoadReward(newAchievement);
    this.m_achievementListDataModel.Achievements[index] = newAchievement;
  }

  private IEnumerator UpdateListPositions()
  {
    while (this.m_numAnimationsPlaying > 0)
    {
      this.m_listable.UpdatePositions();
      yield return (object) null;
    }
    for (int frameCount = 2; frameCount > 0; --frameCount)
    {
      this.m_listable.UpdatePositions();
      yield return (object) null;
    }
    this.m_updatePositionCoroutine = (Coroutine) null;
  }

  private void OnAchievementStatusChanged(
    int achievementId,
    AchievementManager.AchievementStatus status)
  {
    if (status == AchievementManager.AchievementStatus.COMPLETED)
    {
      this.UpdateAchievementDisplayOrder();
    }
    else
    {
      AchievementDbfRecord record = GameDbf.Achievement.GetRecord(achievementId);
      if (record == null || record.RewardListRecord == null || !record.RewardListRecord.ChooseOne || !ProgressUtils.IsAchievementClaimed(status))
        return;
      this.UpdateAchievementDisplayOrder();
    }
  }

  private bool IsAchievementLastClaimableInList(AchievementDataModel achievement)
  {
    int index = this.m_achievementListDataModel.Achievements.IndexOf(achievement) + 1;
    return index >= this.m_achievementListDataModel.Achievements.Count || this.m_achievementListDataModel.Achievements[index].Status != AchievementManager.AchievementStatus.COMPLETED;
  }

  private void HandleClaimAnimationComplete()
  {
    --this.m_numAnimationsPlaying;
    this.m_widget.TriggerEvent("SHOW_COMPLETION_DATE");
    EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
    if (dataModel.Payload is IConvertible)
    {
      int int32 = Convert.ToInt32(dataModel.Payload);
      AchievementDataModel achievementDataModel = AchievementManager.Get().GetAchievementDataModel(int32);
      if (achievementDataModel == null)
      {
        this.UpdateAchievementDisplayOrder();
      }
      else
      {
        AchievementDataModel nextAchievement = achievementDataModel.FindNextAchievement(AchievementManager.Get().GetRecentlyCompletedAchievements());
        if (nextAchievement != null && nextAchievement.Status == AchievementManager.AchievementStatus.COMPLETED)
        {
          this.UpdateSingleAchievement(achievementDataModel, nextAchievement);
        }
        else
        {
          if (this.IsAchievementLastClaimableInList(achievementDataModel))
            return;
          ++this.m_numAnimationsPlaying;
          if (this.m_updatePositionCoroutine == null)
            this.m_updatePositionCoroutine = this.StartCoroutine(this.UpdateListPositions());
          this.m_widget.TriggerEvent("START_HIDE_SEQUENCE");
        }
      }
    }
    else
      this.UpdateAchievementDisplayOrder();
  }
}
