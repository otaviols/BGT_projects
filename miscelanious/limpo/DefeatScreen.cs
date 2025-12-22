using UnityEngine;

public class DefeatScreen : EndGameScreen
{
  protected override void Awake()
  {
    base.Awake();
    if (!this.ShouldMakeUtilRequests())
      return;
    NetCache.Get().RegisterScreenEndOfGame(new NetCache.NetCacheCallback(((EndGameScreen) this).OnNetCacheReady));
  }

  protected override void ShowStandardFlow()
  {
    base.ShowStandardFlow();
    if (GameMgr.Get().IsTraditionalTutorial() && !GameMgr.Get().IsSpectator())
      this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((EndGameScreen) this).ContinueButtonPress_TutorialProgress));
    else
      this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((EndGameScreen) this).ContinueButtonPress_PrevMode));
    BattlegroundsEmoteHandler handler;
    if (BattlegroundsEmoteHandler.TryGetActiveInstance(out handler))
      handler.HideEmotes();
    else if ((Object) EmoteHandler.Get() != (Object) null)
      EmoteHandler.Get().HideEmotes();
    if (!((Object) TargetReticleManager.Get() != (Object) null))
      return;
    TargetReticleManager.Get().DestroyEnemyTargetArrow();
    TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
  }

  protected override void InitGoldRewardUI()
  {
    string challengeRewardText = EndGameScreen.GetFriendlyChallengeRewardText();
    if (string.IsNullOrEmpty(challengeRewardText))
      return;
    this.m_noGoldRewardText.gameObject.SetActive(true);
    this.m_noGoldRewardText.Text = challengeRewardText;
  }
}
