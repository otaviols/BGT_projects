using Hearthstone.UI;
using System;
using UnityEngine;

public class DungeonCrawlDeckSelect : MonoBehaviour
{
  public AsyncReference m_heroDetailsWigetReference;
  public AsyncReference m_deckListWidgetReference;
  private SlidingTray m_slidingTray;
  private DungeonCrawlHeroDetails m_heroDetails;
  private AdventureDungeonCrawlDeckTray m_deckTray;
  private PlayButton m_playButton;

  public bool isReady => this.m_heroDetailsWigetReference.IsReady && this.m_deckListWidgetReference.IsReady && (UnityEngine.Object) this.m_heroDetails != (UnityEngine.Object) null && (UnityEngine.Object) this.m_deckTray != (UnityEngine.Object) null;

  public SlidingTray slidingTray => this.m_slidingTray;

  public DungeonCrawlHeroDetails heroDetails => this.m_heroDetails;

  public AdventureDungeonCrawlDeckTray deckTray => this.m_deckTray;

  public PlayButton playButton => this.m_playButton;

  private void Awake()
  {
    this.m_heroDetailsWigetReference.RegisterReadyListener<DungeonCrawlHeroDetails>(new Action<DungeonCrawlHeroDetails>(this.OnHeroDetailsWidgetReady));
    this.m_deckListWidgetReference.RegisterReadyListener<AdventureDungeonCrawlDeckTray>(new Action<AdventureDungeonCrawlDeckTray>(this.OnDeckListWidgetReady));
    this.m_slidingTray = this.GetComponentInParent<SlidingTray>();
    if (!((UnityEngine.Object) this.m_slidingTray != (UnityEngine.Object) null))
      return;
    this.m_slidingTray.RegisterTrayToggleListener(new SlidingTray.TrayToggledListener(this.OnSlidingTrayToggled));
  }

  private void OnHeroDetailsWidgetReady(DungeonCrawlHeroDetails details)
  {
    this.m_heroDetails = details;
    this.m_playButton = this.m_heroDetails.GetComponentInChildren<PlayButton>();
    this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonReleased));
  }

  private void OnDeckListWidgetReady(AdventureDungeonCrawlDeckTray tray) => this.m_deckTray = tray;

  private void OnPlayButtonReleased(UIEvent e)
  {
    this.slidingTray.ToggleTraySlider(false);
    this.m_playButton.SetEnabled(false);
  }

  private void OnSlidingTrayToggled(bool isShowing) => this.m_playButton.SetEnabled(isShowing);
}
