using Assets;
using System;
using System.Collections.Generic;

public class AchievementPopups : IDisposable
{
  private List<Achievement> m_progressedAchieves = new List<Achievement>();
  private List<Achievement> m_completedAchieves = new List<Achievement>();
  private Action<List<Achievement>> OnUpdateReward;
  private PopupDisplayManager m_popupDisplayManager;

  public List<Achievement> CompletedAchieves => this.m_completedAchieves;

  public List<Achievement> ProgressedAchieves => this.m_progressedAchieves;

  public AchievementPopups(
    PopupDisplayManager popupDisplayManager,
    Action<List<Achievement>> updateRewardCallback)
  {
    this.m_popupDisplayManager = popupDisplayManager;
    this.OnUpdateReward = updateRewardCallback;
    AchieveManager.Get().RegisterAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
  }

  public void Dispose() => AchieveManager.Get().RemoveAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated), (object) null);

  private void OnAchievesUpdated(
    List<Achievement> updatedAchieves,
    List<Achievement> completedAchieves,
    object userData)
  {
    HashSet<Achieve.RewardTiming> rewardTimings = new HashSet<Achieve.RewardTiming>()
    {
      Achieve.RewardTiming.IMMEDIATE,
      Achieve.RewardTiming.OUT_OF_BAND
    };
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE || !SceneMgr.Get().IsTransitioning())
      rewardTimings.Add(Achieve.RewardTiming.ADVENTURE_CHEST);
    this.PrepareNewlyProgressedAchievesToBeShown();
    this.PrepareNewlyCompletedAchievesToBeShown(rewardTimings);
  }

  private void PrepareNewlyProgressedAchievesToBeShown() => this.m_progressedAchieves = AchieveManager.Get().GetNewlyProgressedQuests();

  public void PrepareNewlyCompletedAchievesToBeShown(HashSet<Achieve.RewardTiming> rewardTimings)
  {
    if (!this.m_popupDisplayManager.CanShowPopups())
      return;
    foreach (Achievement achievement in AchieveManager.Get().GetNewCompletedAchievesToShow())
    {
      Achievement achieve = achievement;
      if (this.m_completedAchieves.Find((Predicate<Achievement>) (obj => achieve.ID == obj.ID)) != null)
        Log.Achievements.Print("PopupDisplayManager: skipping completed achievement already being processed: " + (object) achieve);
      else if (rewardTimings == null || !rewardTimings.Contains(achieve.RewardTiming))
      {
        Log.Achievements.PrintDebug("PopupDisplayManager: skipping completed achievement with {0} reward timing: {1}", (object) achieve.RewardTiming, (object) achieve);
      }
      else
      {
        Log.Achievements.Print("PopupDisplayManager: adding completed achievement " + (object) achieve);
        this.m_completedAchieves.Add(achieve);
      }
    }
    Action<List<Achievement>> onUpdateReward = this.OnUpdateReward;
    if (onUpdateReward == null)
      return;
    onUpdateReward(this.m_completedAchieves);
  }
}
