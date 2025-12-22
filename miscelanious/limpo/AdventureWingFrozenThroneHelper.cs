using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof (AdventureWing))]
[CustomEditClass]
public class AdventureWingFrozenThroneHelper : MonoBehaviour
{
  public NestedPrefab m_secondaryBigChestContainer;
  public int m_secondaryChestVariation;
  public TooltipZone m_chestTwoColumnTooltipZone;
  public TooltipZone m_chestNormalTooltipZone;
  public FrozenThroneEventTable m_frozenThroneEventTable;
  public Vector3_MobileOverride m_tooltipOffsetFromReward;
  private AdventureWing m_adventureWing;
  private PegUIElement m_BigChestSecondary;
  private List<Achievement> m_classSpecificAchieves;
  private List<Achievement> m_newlyCompletedAchieves;
  private int m_numClassesAlreadyCompleted;
  private bool m_needToAnimateBigChest;
  private TooltipZone m_currentChestTooltipZone;
  private bool m_waitingForRuneAnimationEnd;

  private void Awake()
  {
    if (!((UnityEngine.Object) this.m_secondaryBigChestContainer != (UnityEngine.Object) null))
      return;
    AdventureWingRewardsChest_ICC componentInChildren = this.m_secondaryBigChestContainer.PrefabGameObject(true).GetComponentInChildren<AdventureWingRewardsChest_ICC>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    componentInChildren.ActivateChest(this.m_secondaryChestVariation);
    PegUIElement component = componentInChildren.GetComponent<PegUIElement>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    this.m_BigChestSecondary = component;
  }

  private void Start()
  {
    this.m_adventureWing = this.GetComponent<AdventureWing>();
    if ((UnityEngine.Object) this.m_adventureWing == (UnityEngine.Object) null)
    {
      Log.All.PrintError("AdventureWingKarazhanHelper could not find an AdventureWing component on the same GameObject!");
    }
    else
    {
      if ((UnityEngine.Object) this.m_BigChestSecondary != (UnityEngine.Object) null)
      {
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          this.m_BigChestSecondary.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ShowBigChestRewards));
        }
        else
        {
          this.m_BigChestSecondary.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowBigChestRewards));
          this.m_BigChestSecondary.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideBigChestRewards));
        }
      }
      AdventureMissionDisplay.Get().AddProgressStepCompletedListener(new AdventureMissionDisplay.ProgressStepCompletedCallback(this.OnAdventureProgressStepCompleted));
      this.m_frozenThroneEventTable.AddAnimateRuneEventEndListener(new StateEventTable.StateEventTrigger(this.RuneAnimationEndEvent));
    }
  }

  private void OnAdventureProgressStepCompleted(AdventureMissionDisplay.ProgressStep step)
  {
    if (AdventureMissionDisplay.ProgressStep.WING_COINS_AND_CHESTS_UPDATED != step)
      return;
    this.StartCoroutine(this.PlayRuneAnimationsIfNecessary());
  }

  private void Update()
  {
    if (!this.m_adventureWing.IsDevMode)
      return;
    if (InputCollection.GetKeyDown(KeyCode.Q))
      this.AnimateRuneActivation(0);
    if (InputCollection.GetKeyDown(KeyCode.W))
      this.AnimateRuneActivation(1);
    if (InputCollection.GetKeyDown(KeyCode.E))
      this.AnimateRuneActivation(2);
    if (InputCollection.GetKeyDown(KeyCode.R))
      this.AnimateRuneActivation(3);
    if (InputCollection.GetKeyDown(KeyCode.T))
      this.AnimateRuneActivation(4);
    if (InputCollection.GetKeyDown(KeyCode.Y))
      this.AnimateRuneActivation(5);
    if (InputCollection.GetKeyDown(KeyCode.U))
      this.AnimateRuneActivation(6);
    if (InputCollection.GetKeyDown(KeyCode.I))
      this.AnimateRuneActivation(7);
    if (!InputCollection.GetKeyDown(KeyCode.O))
      return;
    this.AnimateRuneActivation(8);
  }

  public void SetBigChestRewards(WingDbId wingId)
  {
    if (AdventureConfig.Get().GetSelectedMode() != AdventureModeDbId.LINEAR)
      return;
    HashSet<Achieve.RewardTiming> rewardTimings = new HashSet<Achieve.RewardTiming>()
    {
      Achieve.RewardTiming.ADVENTURE_CHEST
    };
    List<RewardData> rewardsForAchieve = AchieveManager.Get().GetRewardsForAchieve(768, rewardTimings);
    if ((UnityEngine.Object) this.m_BigChestSecondary != (UnityEngine.Object) null)
      this.m_BigChestSecondary.SetData((object) rewardsForAchieve);
    this.m_classSpecificAchieves = this.GetClassSpecificAchievementsForWing(wingId);
    this.PrepareRuneAnimations(this.m_classSpecificAchieves);
  }

  public List<RewardData> GetBigChestRewards() => !((UnityEngine.Object) this.m_BigChestSecondary != (UnityEngine.Object) null) ? (List<RewardData>) null : (List<RewardData>) this.m_BigChestSecondary.GetData();

  private void ShowBigChestRewards(UIEvent e)
  {
    List<RewardData> bigChestRewards = this.GetBigChestRewards();
    if (bigChestRewards == null)
      return;
    this.m_adventureWing.FireShowRewardsEvent(bigChestRewards, this.m_BigChestSecondary.transform.position);
    AdventureMissionDisplay.Get().m_RewardsDisplay.AddRewardsHiddenListener(new AdventureRewardsDisplayArea.RewardsHidden(this.SecondaryChestRewardsHidden));
    this.ShowProgressTooltip();
  }

  private void ShowProgressTooltip()
  {
    if (this.m_numClassesAlreadyCompleted == 0)
    {
      this.m_currentChestTooltipZone = this.m_chestNormalTooltipZone;
      this.RepositionChestTooltip(this.m_currentChestTooltipZone);
      this.m_currentChestTooltipZone.ShowTooltip(GameStrings.Get("GLUE_FROSTMOURNE_REWARD_HEADER"), GameStrings.Get("GLUE_FROSTMOURNE_WING_INCOMPLETE_REWARD_BODY"), 4f);
    }
    else
    {
      List<StringBuilder> stringBuilderList = new List<StringBuilder>(2);
      stringBuilderList.Add(new StringBuilder());
      stringBuilderList.Add(new StringBuilder());
      bool flag = false;
      int index = 0;
      foreach (Achievement classSpecificAchieve in this.m_classSpecificAchieves)
      {
        TAG_CLASS? classRequirement = classSpecificAchieve.MyHeroClassRequirement;
        if (!classRequirement.HasValue)
        {
          Log.All.PrintWarning("Something is wrong - achievement {0} has no MyHeroClass!", (object) classSpecificAchieve);
        }
        else
        {
          classRequirement = classSpecificAchieve.MyHeroClassRequirement;
          TAG_CLASS tag = classRequirement.Value;
          if (!classSpecificAchieve.IsCompleted())
          {
            flag = true;
            if (stringBuilderList[index].Length > 0)
              stringBuilderList[index].Append("\n");
            stringBuilderList[index].Append(string.Format("- {0}", (object) GameStrings.GetClassName(tag)));
            index = 1 - index;
          }
          else
            Log.Adventures.Print("AdventureWingFrozenThroneHelper.ShowProgressTooltip(): Achievement for class {0} is completed.", (object) GameStrings.GetClassName(tag));
        }
      }
      if (flag)
      {
        this.m_currentChestTooltipZone = this.m_chestTwoColumnTooltipZone;
        this.RepositionChestTooltip(this.m_currentChestTooltipZone);
        this.m_currentChestTooltipZone.ShowMultiColumnTooltip(GameStrings.Get("GLUE_FROSTMOURNE_REWARD_HEADER"), GameStrings.Get("GLUE_FROSTMOURNE_REWARD_BODY"), new string[2]
        {
          stringBuilderList[0].ToString(),
          stringBuilderList[1].ToString()
        }, 4f);
      }
      else
        Log.All.PrintWarning("AdventureWingFrozenThroneHelper.ShowProgressTooltip(): No classes to add to the tooltip! We should not be showing the tooltip in this case!");
    }
  }

  private void RepositionChestTooltip(TooltipZone tooltipZone)
  {
    List<GameObject> currentShownRewards = AdventureMissionDisplay.Get().m_RewardsDisplay.GetCurrentShownRewards();
    if (currentShownRewards.Count <= 0)
      return;
    Vector3 position = tooltipZone.tooltipDisplayLocation.transform.position;
    Vector3 offsetFromReward = (Vector3) (MobileOverrideValue<Vector3>) this.m_tooltipOffsetFromReward;
    position.x = currentShownRewards[0].transform.position.x + offsetFromReward.x;
    position.z = currentShownRewards[0].transform.position.z + offsetFromReward.z;
    tooltipZone.tooltipDisplayLocation.transform.position = position;
  }

  private void HideBigChestRewards(UIEvent e)
  {
    List<RewardData> bigChestRewards = this.GetBigChestRewards();
    if (bigChestRewards == null)
      return;
    this.m_adventureWing.FireHideRewardsEvent(bigChestRewards);
  }

  private void SecondaryChestRewardsHidden()
  {
    AdventureMissionDisplay.Get().m_RewardsDisplay.RemoveRewardsHiddenListener(new AdventureRewardsDisplayArea.RewardsHidden(this.SecondaryChestRewardsHidden));
    this.HideProgressTooltip();
  }

  private void HideProgressTooltip()
  {
    if (!((UnityEngine.Object) this.m_currentChestTooltipZone != (UnityEngine.Object) null))
      return;
    this.m_currentChestTooltipZone.HideTooltip();
  }

  private List<Achievement> GetClassSpecificAchievementsForWing(WingDbId wingId)
  {
    List<Achievement> achievementsForWing = new List<Achievement>();
    foreach (Achievement achievement in AchieveManager.Get().GetAchievesForAdventureWing((int) wingId))
    {
      if (achievement.AchieveType == Achieve.Type.HIDDEN && achievement.MyHeroClassRequirement.HasValue && achievement.MyHeroClassRequirement.Value != TAG_CLASS.INVALID)
        achievementsForWing.Add(achievement);
    }
    return achievementsForWing;
  }

  private void PrepareRuneAnimations(List<Achievement> classSpecificAchieves)
  {
    if (classSpecificAchieves == null)
    {
      Log.All.PrintWarning("AdventureWingFrozenThroneHelper.PrepareRuneAnimations() - Attempting to prepare rune animations, but classSpecificAchieves is null!");
    }
    else
    {
      this.m_numClassesAlreadyCompleted = 0;
      this.m_newlyCompletedAchieves = new List<Achievement>();
      foreach (Achievement classSpecificAchieve in classSpecificAchieves)
      {
        if (classSpecificAchieve.IsCompleted())
        {
          if (classSpecificAchieve.IsNewlyCompleted())
            this.m_newlyCompletedAchieves.Add(classSpecificAchieve);
          else
            ++this.m_numClassesAlreadyCompleted;
        }
      }
      for (int rune = 0; rune < this.m_numClassesAlreadyCompleted; ++rune)
        this.m_frozenThroneEventTable.SetRuneInitiallyActivated(rune);
      Log.Adventures.Print("{0} runes already animated, {1} waiting for animation.", (object) this.m_numClassesAlreadyCompleted, (object) this.m_newlyCompletedAchieves.Count);
      Achievement achievement = AchieveManager.Get().GetAchievement(768);
      if (achievement == null || !achievement.IsCompleted())
        return;
      this.m_needToAnimateBigChest = achievement.IsNewlyCompleted();
      if (this.m_needToAnimateBigChest)
        return;
      this.m_frozenThroneEventTable.BigChestSecondaryStayOpen();
    }
  }

  private IEnumerator PlayRuneAnimationsIfNecessary()
  {
    if (this.m_newlyCompletedAchieves == null)
    {
      Log.All.PrintWarning("AdventureWingFrozenThroneHelper.PlayRuneAnimationIfNecessary() - Attempting to play rune animations for newly completed achieves, but m_newlyCompletedAchieves is null!");
    }
    else
    {
      if (this.m_newlyCompletedAchieves.Count > 0)
      {
        AdventureMissionDisplay.Get().GetExternalUILock();
        this.m_adventureWing.BringToFocus();
        foreach (Achievement achieve in this.m_newlyCompletedAchieves)
        {
          Log.Adventures.Print("Playing animation for rune {0}, for class {1}", (object) this.m_numClassesAlreadyCompleted, (object) achieve.MyHeroClassRequirement.Value);
          this.m_waitingForRuneAnimationEnd = true;
          this.AnimateRuneActivation(this.m_numClassesAlreadyCompleted);
          while (this.m_waitingForRuneAnimationEnd)
            yield return (object) null;
          achieve.AckCurrentProgressAndRewardNotices();
          ++this.m_numClassesAlreadyCompleted;
        }
        AdventureMissionDisplay.Get().ReleaseExternalUILock();
        this.m_newlyCompletedAchieves.Clear();
      }
      Log.Adventures.Print("Finished animating runes, if applicable.");
      if (this.m_needToAnimateBigChest)
      {
        AdventureMissionDisplay.Get().GetExternalUILock();
        this.m_adventureWing.BringToFocus();
        bool waitingForNextStep = true;
        this.m_frozenThroneEventTable.AddChestOpenEndEventListener((StateEventTable.StateEventTrigger) (s => waitingForNextStep = false), true);
        this.OpenBigChestSecondary();
        while (waitingForNextStep)
          yield return (object) null;
        if (UserAttentionManager.CanShowAttentionGrabber("AdventureMissionDisplay.ShowFixedRewards"))
        {
          waitingForNextStep = true;
          PopupDisplayManager.Get().ShowAnyOutstandingPopups((Action) (() => waitingForNextStep = false));
          while (waitingForNextStep)
            yield return (object) null;
        }
        AdventureMissionDisplay.Get().ReleaseExternalUILock();
        this.m_needToAnimateBigChest = false;
      }
    }
  }

  private void AnimateRuneActivation(int rune) => this.m_frozenThroneEventTable.AnimateRuneActivation(rune);

  private void RuneAnimationEndEvent(Spell s) => this.m_waitingForRuneAnimationEnd = false;

  private void OpenBigChestSecondary()
  {
    this.m_frozenThroneEventTable.BigChestSecondaryOpen();
    if (!((UnityEngine.Object) this.m_BigChestSecondary != (UnityEngine.Object) null))
      return;
    this.m_BigChestSecondary.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ShowBigChestRewards));
    this.m_BigChestSecondary.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowBigChestRewards));
    this.m_BigChestSecondary.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideBigChestRewards));
  }
}
