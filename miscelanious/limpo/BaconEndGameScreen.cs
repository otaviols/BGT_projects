using UnityEngine;

public class BaconEndGameScreen : EndGameScreen
{
  public GamesWonIndicator m_gamesWonIndicator;
  private const int ShowWinProgressPlacement = 4;
  private const int ShowAppRatingPromptPlacement = 3;
  private bool m_showWinProgress;
  private int m_place = int.MaxValue;

  private int Place
  {
    get
    {
      if (this.m_place == int.MaxValue && GameState.Get() != null)
      {
        Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
        if (friendlySidePlayer != null && friendlySidePlayer.GetHero() != null)
          this.m_place = friendlySidePlayer.GetHero().GetRealTimePlayerLeaderboardPlace();
      }
      return this.m_place;
    }
  }

  protected override void Awake()
  {
    base.Awake();
    this.m_gamesWonIndicator.Hide();
    if (!this.ShouldMakeUtilRequests())
      return;
    NetCache.Get().RegisterScreenEndOfGame(new NetCache.NetCacheCallback(((EndGameScreen) this).OnNetCacheReady));
  }

  protected override void ShowStandardFlow()
  {
    base.ShowStandardFlow();
    this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((EndGameScreen) this).ContinueButtonPress_PrevMode));
  }

  protected override void OnTwoScoopShown()
  {
    if ((Object) BnetBar.Get() != (Object) null)
      BnetBar.Get().SuppressLoginTooltip(true);
    if (!this.m_showWinProgress)
      return;
    this.m_gamesWonIndicator.Show();
  }

  protected override void OnTwoScoopHidden()
  {
    if (!this.m_showWinProgress)
      return;
    this.m_gamesWonIndicator.Hide();
  }

  protected override void InitGoldRewardUI() => this.m_showWinProgress = this.Place <= 4;

  protected override bool ShowAppRatingPrompt() => this.Place <= 3 && MobileCallbackManager.RequestAppReview();
}
