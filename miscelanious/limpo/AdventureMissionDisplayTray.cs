using Assets;
using UnityEngine;

public class AdventureMissionDisplayTray : MonoBehaviour
{
  public SlidingTray m_slidingTray;
  public PegUIElement m_rewardsChest;
  public AdventureRewardsDisplayArea m_rewardsDisplay;
  public Transform m_rewardsDisplayBone;

  private void Awake()
  {
    AdventureConfig adventureConfig = AdventureConfig.Get();
    adventureConfig.AddAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSelected));
    adventureConfig.AddSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubsceneChanged));
    if ((Object) this.m_rewardsChest != (Object) null)
    {
      this.m_rewardsChest.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.ShowRewards()));
      this.m_rewardsChest.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.HideRewards()));
    }
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
  }

  private void OnDestroy()
  {
    AdventureConfig adventureConfig = AdventureConfig.Get();
    if ((Object) adventureConfig != (Object) null)
    {
      adventureConfig.RemoveAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSelected));
      adventureConfig.RemoveSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubsceneChanged));
    }
    if (GameMgr.Get() == null)
      return;
    GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
  }

  private void OnMissionSelected(ScenarioDbId mission, bool showDetails)
  {
    if (mission == ScenarioDbId.INVALID)
      return;
    if (showDetails)
      this.m_slidingTray.ToggleTraySlider(true);
    this.ShowRewardsChest();
  }

  public void EnableRewardsChest(bool enabled)
  {
    if ((Object) this.m_rewardsChest == (Object) null)
      return;
    this.m_rewardsChest.SetEnabled(enabled);
  }

  public void ShowRewardsChest()
  {
    if ((Object) this.m_rewardsChest == (Object) null)
      return;
    ScenarioDbId mission = AdventureConfig.Get().GetMission();
    bool flag = AdventureProgressMgr.Get().HasDefeatedScenario((int) mission);
    this.m_rewardsChest.gameObject.SetActive(AdventureProgressMgr.Get().ScenarioHasRewardData((int) mission) && !flag);
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        this.EnableRewardsChest(true);
        break;
    }
    return false;
  }

  private void ShowRewards()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      NotificationManager.Get().DestroyActiveQuote(0.2f);
    this.m_rewardsDisplay.ShowRewardsNoFullscreen(AdventureProgressMgr.Get().GetImmediateRewardsForDefeatingScenario((int) AdventureConfig.Get().GetMission()), this.m_rewardsDisplayBone.position, new Vector3?(this.m_rewardsChest.transform.position));
  }

  private void HideRewards() => this.m_rewardsDisplay.HideRewards();

  private void OnSubsceneChanged(AdventureData.Adventuresubscene newscene, bool forward) => this.m_slidingTray.ToggleTraySlider(false);
}
