using Hearthstone.DungeonCrawl;
using UnityEngine;

public class AdventureSubSceneDungeonRun : AdventureSubSceneDisplay
{
  private DungeonCrawlServices m_dungeonCrawlServices;
  private GameObject m_dungeonCrawlDisplay;

  private void Start()
  {
    if (!(bool) (Object) this.m_dungeonCrawlDisplay)
      DungeonCrawlUtil.LoadDungeonRunPrefab(new DungeonCrawlUtil.DungeonRunLoadCallback(this.OnDungeonRunLoaded));
    else
      this.OnDungeonRunLoaded(this.m_dungeonCrawlDisplay);
  }

  private void OnDungeonRunLoaded(GameObject go)
  {
    this.m_dungeonCrawlDisplay = go;
    this.m_dungeonCrawlServices = DungeonCrawlUtil.CreateAdventureDungeonCrawlServices(this.AssetLoadingHelper);
    AdventureDungeonCrawlDisplay component = go.GetComponent<AdventureDungeonCrawlDisplay>();
    if ((Object) component != (Object) null)
    {
      GameUtils.SetParent(go, (Component) this.transform);
      component.StartRun(this.m_dungeonCrawlServices);
    }
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_dungeonCrawlDisplay.transform.localPosition = new Vector3(0.0f, 5.5f, 0.0f);
  }

  protected override void OnSubSceneTransitionComplete() => this.m_dungeonCrawlServices.SubsceneController.OnTransitionComplete();
}
