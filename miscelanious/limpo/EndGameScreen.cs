using Assets;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusLettuce;
using PegasusShared;
using SpectatorProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class EndGameScreen : MonoBehaviour
{
  public EndGameTwoScoop m_twoScoop;
  public PegUIElement m_hitbox;
  public UberText m_noGoldRewardText;
  public UberText m_continueText;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_ScoreScreenPrefab;
  public static EndGameScreen.OnTwoScoopsShownHandler OnTwoScoopsShown;
  public static Action OnBackOutOfGameplay;
  private static EndGameScreen s_instance;
  private bool m_shown;
  private bool m_netCacheReady;
  private bool m_achievesReady;
  private bool m_heroRewardEventReady;
  protected List<Achievement> m_completedQuests = new List<Achievement>();
  private bool m_isShowingFixedRewards;
  private List<Reward> m_rewards = new List<Reward>();
  private int m_numRewardsToLoad;
  private bool m_rewardsLoaded;
  private List<Reward> m_genericRewards = new List<Reward>();
  private HashSet<long> m_genericRewardChestNoticeIdsReady = new HashSet<long>();
  private Reward m_currentlyShowingReward;
  private bool m_haveShownTwoScoop;
  private bool m_hasAlreadySetMode;
  private int m_inputBlocker;
  private bool m_playingBlockingAnim;
  private bool m_doneDisplayingRewards;
  private bool m_showingScoreScreen;
  private ScoreScreen m_scoreScreen;
  private GameObject m_rankChangeTwoScoop;
  private bool m_rankChangeReady;
  private bool m_medalInfoUpdated;
  private const int MEDAL_INFO_RETRY_COUNT_MAX = 3;
  private const float MEDAL_INFO_RETRY_INITIAL_DELAY = 1f;
  private int m_medalInfoRetryCount;
  private float m_medalInfoRetryDelay;
  private bool m_shouldShowRankChange;
  private bool m_isShowingRankChange;
  private bool m_hasSentRankedInitTelemetry;
  private float m_endGameScreenStartTime;
  private Widget m_rankedRewardDisplayWidget;
  private RankedRewardDisplay m_rankedRewardDisplay;
  private bool m_isShowingRankedReward;
  private List<List<RewardData>> m_rankedRewardsToDisplay = new List<List<RewardData>>();
  private Widget m_rankedCardBackProgressWidget;
  private RankedCardBackProgressDisplay m_rankedCardBackProgress;
  private bool m_shouldShowRankedCardBackProgress;
  private bool m_isShowingRankedCardBackProgress;
  private bool m_isShowingTrackRewards;
  private bool m_shouldShowRewardXpGains;
  private bool m_isShowingMercenariesExperienceRewards;
  private bool m_finishedShowingMercenariesExperienceRewards;
  private bool m_hasTimedOutAndLogged;
  private float m_timeoutTimerStartTime;
  private ScreenEffectsHandle m_screenEffectsHandle;
  private const float m_maxWaitTime = 5f;

  protected virtual void Awake()
  {
    EndGameScreen.s_instance = this;
    if (GameMgr.Get().IsBattlegrounds())
      this.m_netCacheReady = true;
    this.StartCoroutine(this.WaitForAchieveManager());
    this.ProcessPreviousAchievements();
    AchieveManager.Get().RegisterAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
    AchieveManager.Get().CheckPlayedNearbyPlayerOnSubnet();
    this.m_shouldShowRankChange = !GameMgr.Get().IsSpectator() && GameMgr.Get().IsPlay() && Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);
    this.m_hitbox.gameObject.SetActive(false);
    string key = "GLOBAL_CLICK_TO_CONTINUE";
    if (UniversalInputManager.Get().IsTouchMode())
      key = "GLOBAL_CLICK_TO_CONTINUE_TOUCH";
    this.m_continueText.Text = GameStrings.Get(key);
    this.m_continueText.gameObject.SetActive(false);
    this.m_noGoldRewardText.gameObject.SetActive(false);
    PegUI.Get().AddInputCamera(CameraUtils.FindFirstByLayer(GameLayer.IgnoreFullScreenEffects));
    LayerUtils.SetLayer(this.m_hitbox.gameObject, GameLayer.IgnoreFullScreenEffects);
    LayerUtils.SetLayer(this.m_continueText.gameObject, GameLayer.IgnoreFullScreenEffects);
    if (!Network.ShouldBeConnectedToAurora())
      this.UpdateRewards();
    this.m_genericRewardChestNoticeIdsReady = GenericRewardChestNoticeManager.Get().GetReadyGenericRewardChestNotices();
    GenericRewardChestNoticeManager.Get().RegisterRewardsUpdatedListener(new GenericRewardChestNoticeManager.GenericRewardUpdatedCallback(this.OnGenericRewardUpdated));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected virtual void OnDestroy()
  {
    if (NetCache.Get() != null)
      NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheMedalInfo), new Action(this.OnMedalInfoUpdate));
    if (EndGameScreen.OnTwoScoopsShown != null)
      EndGameScreen.OnTwoScoopsShown(false, this.m_twoScoop);
    if (AchieveManager.Get() != null)
      AchieveManager.Get().RemoveAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
    if (GenericRewardChestNoticeManager.Get() != null)
      GenericRewardChestNoticeManager.Get().RemoveRewardsUpdatedListener(new GenericRewardChestNoticeManager.GenericRewardUpdatedCallback(this.OnGenericRewardUpdated));
    this.m_screenEffectsHandle.StopEffect();
    EndGameScreen.s_instance = (EndGameScreen) null;
  }

  public static EndGameScreen Get() => EndGameScreen.s_instance;

  public virtual void Show()
  {
    if (GameState.Get() != null && GameState.Get().WasRestartRequested())
      return;
    this.m_shown = true;
    this.m_endGameScreenStartTime = Time.time;
    Network.Get().DisconnectFromGameServer();
    InputManager.Get().DisableInput();
    this.m_hitbox.gameObject.SetActive(true);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
    if (GameState.Get() != null && GameState.Get().GetFriendlySidePlayer() != null)
      GameState.Get().GetFriendlySidePlayer().GetHandZone().UpdateLayout((Card) null);
    this.ShowScoreScreen();
    this.ShowStandardFlowIfReady();
  }

  public void SetPlayingBlockingAnim(bool set) => this.m_playingBlockingAnim = set;

  public bool IsPlayingBlockingAnim() => this.m_playingBlockingAnim;

  public void AddInputBlocker() => ++this.m_inputBlocker;

  public void RemoveInputBlocker() => --this.m_inputBlocker;

  private bool IsInputBlocked() => this.m_inputBlocker > 0;

  public bool IsScoreScreenShown() => this.m_showingScoreScreen;

  private void ShowTutorialProgress()
  {
    this.HideTwoScoop();
    this.StartCoroutine(this.LoadTutorialProgress());
  }

  private IEnumerator LoadTutorialProgress()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    EndGameScreen endGameScreen = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "TutorialProgressScreen.prefab:a78bac9caa971494ea8fac23dc1a9bd8", new PrefabCallback<GameObject>(endGameScreen.OnTutorialProgressScreenCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(0.25f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void OnTutorialProgressScreenCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    go.transform.parent = this.transform;
    go.GetComponent<TutorialProgressScreen>().StartTutorialProgress();
  }

  protected void ContinueButtonPress_Common() => LoadingScreen.Get().AddTransitionObject((Component) this);

  protected void ContinueButtonPress_ProceedToError(UIEvent e)
  {
    if (this.IsPlayingBlockingAnim())
      return;
    this.HideScoreScreen();
    this.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_ProceedToError));
  }

  protected void ContinueButtonPress_PrevMode(UIEvent e) => this.ContinueEvents();

  public bool ContinueEvents()
  {
    if (this.ContinueDefaultEvents())
      return true;
    if ((UnityEngine.Object) this.m_twoScoop == (UnityEngine.Object) null)
      return false;
    PlayMakerFSM component = this.m_twoScoop.GetComponent<PlayMakerFSM>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.SendEvent("Death");
    this.ContinueButtonPress_Common();
    this.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_PrevMode));
    this.ReturnToPreviousMode();
    return false;
  }

  protected void ContinueButtonPress_TutorialProgress(UIEvent e) => this.ContinueTutorialEvents();

  public void ContinueTutorialEvents()
  {
    if (this.ContinueDefaultEvents())
      return;
    this.ContinueButtonPress_Common();
    this.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_TutorialProgress));
    this.m_continueText.gameObject.SetActive(false);
    this.ShowTutorialProgress();
  }

  private bool ContinueDefaultEvents()
  {
    if (this.IsPlayingBlockingAnim() || this.IsInputBlocked())
      return true;
    if ((UnityEngine.Object) this.m_currentlyShowingReward != (UnityEngine.Object) null)
    {
      this.m_currentlyShowingReward.Hide(true);
      this.m_currentlyShowingReward = (Reward) null;
    }
    this.HideScoreScreen();
    if (!this.m_haveShownTwoScoop)
      return true;
    this.HideTwoScoop();
    if (this.ShowHeroRewardEvent() && this.m_heroRewardEventReady || this.ShowRewardTrackXpGains() || this.ShowNextRewardTrackAutoClaimedReward() || this.ShowFixedRewards() || this.ShowGoldReward() || this.ShowRankedCardBackProgress() || this.ShowRankChange() || this.ShowRankedRewards() || this.ShowNextProgressionQuestReward() || this.ShowMercenariesExperienceRewards() || this.ShowNextCompletedQuest() || this.ShowNextReward() || this.ShowNextGenericReward() || !SpectatorManager.Get().IsSpectatingOrWatching && TemporaryAccountManager.IsTemporaryAccount() && this.ShowHealUpDialog() || this.ShowPushNotificationPrompt() || this.ShowAppRatingPrompt())
      return true;
    this.m_doneDisplayingRewards = true;
    return false;
  }

  protected virtual void OnTwoScoopShown()
  {
  }

  protected virtual void OnTwoScoopHidden()
  {
  }

  protected virtual void InitGoldRewardUI()
  {
  }

  private static string GetFriendlyChallengeRewardMessage(Achievement achieve)
  {
    if (DemoMgr.Get().IsDemo())
      return (string) null;
    string challengeRewardMessage = (string) null;
    if (achieve.DbfRecord.MaxDefense > 0)
    {
      challengeRewardMessage = EndGameScreen.GetFriendlyChallengeEarlyConcedeMessage(achieve.DbfRecord.MaxDefense);
      if (!string.IsNullOrEmpty(challengeRewardMessage))
        return challengeRewardMessage;
    }
    AchieveRegionDataDbfRecord currentRegionData = achieve.GetCurrentRegionData();
    if (currentRegionData != null && currentRegionData.RewardableLimit > 0 && achieve.IntervalRewardStartDate > 0L && (DateTime.UtcNow - DateTime.FromFileTimeUtc(achieve.IntervalRewardStartDate)).TotalDays < currentRegionData.RewardableInterval && achieve.IntervalRewardCount >= currentRegionData.RewardableLimit)
      challengeRewardMessage = GameStrings.Get("GLOBAL_FRIENDLYCHALLENGE_QUEST_REWARD_AT_LIMIT");
    if (string.IsNullOrEmpty(challengeRewardMessage) && currentRegionData != null && currentRegionData.RewardableLimit > 0 && FriendChallengeMgr.Get().DidReceiveChallenge())
      achieve.IncrementIntervalRewardCount();
    return challengeRewardMessage;
  }

  protected static string GetFriendlyChallengeRewardText()
  {
    if (!FriendChallengeMgr.Get().HasChallenge())
      return (string) null;
    if (DemoMgr.Get().IsDemo())
      return (string) null;
    string challengeRewardText = (string) null;
    AchieveManager achieveManager = AchieveManager.Get();
    PartyQuestInfo partyQuestInfo = FriendChallengeMgr.Get().GetPartyQuestInfo();
    if (partyQuestInfo != null)
    {
      int num = FriendChallengeMgr.Get().DidSendChallenge() ? 1 : 0;
      bool challenge = FriendChallengeMgr.Get().DidReceiveChallenge();
      PegasusShared.PlayerType playerType = PegasusShared.PlayerType.PT_ANY;
      if (num != 0)
        playerType = PegasusShared.PlayerType.PT_FRIENDLY_CHALLENGER;
      if (challenge)
        playerType = PegasusShared.PlayerType.PT_FRIENDLY_CHALLENGEE;
      for (int index = 0; index < partyQuestInfo.QuestIds.Count; ++index)
      {
        Achievement achievement1 = achieveManager.GetAchievement(partyQuestInfo.QuestIds[index]);
        if (achievement1 != null && achievement1.IsValidFriendlyPlayerChallengeType(playerType))
          challengeRewardText = EndGameScreen.GetFriendlyChallengeRewardMessage(achievement1);
        if (string.IsNullOrEmpty(challengeRewardText))
        {
          Achievement achievement2 = achieveManager.GetAchievement(achievement1.DbfRecord.SharedAchieveId);
          if (achievement2 != null && achievement2.IsValidFriendlyPlayerChallengeType(playerType))
            challengeRewardText = EndGameScreen.GetFriendlyChallengeRewardMessage(achievement2);
        }
      }
    }
    if (string.IsNullOrEmpty(challengeRewardText) && SpecialEventManager.Get().IsEventActive(SpecialEventType.FRIEND_WEEK, false))
    {
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      int num = achieveManager.GetActiveQuests().Where<Achievement>((Func<Achievement, bool>) (a => a.IsAffectedByFriendWeek && (a.AchieveTrigger == Achieve.Trigger.WIN || a.AchieveTrigger == Achieve.Trigger.FINISH) && a.GameModeRequiresNonFriendlyChallenge)).Any<Achievement>() ? 1 : 0;
      bool flag1 = false;
      if (FriendChallengeMgr.Get().IsChallengeTavernBrawl() && netObject != null && netObject.FriendWeekAllowsTavernBrawlRecordUpdate)
      {
        BrawlType challengeBrawlType = FriendChallengeMgr.Get().GetChallengeBrawlType();
        TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(challengeBrawlType);
        TavernBrawlPlayerRecord record = TavernBrawlManager.Get().GetRecord(challengeBrawlType);
        bool flag2 = mission != null && (mission.rewardTrigger == RewardTrigger.REWARD_TRIGGER_WIN_GAME || mission.rewardTrigger == RewardTrigger.REWARD_TRIGGER_FINISH_GAME);
        if (((mission == null ? 0 : (mission.rewardType != 0 ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 && record != null && record.RewardProgress < mission.RewardTriggerQuota)
          flag1 = true;
      }
      if (num == 0 && !flag1)
        return (string) null;
      int concederMaxDefense = 0;
      if (netObject != null)
        concederMaxDefense = netObject.FriendWeekConcederMaxDefense;
      challengeRewardText = EndGameScreen.GetFriendlyChallengeEarlyConcedeMessage(concederMaxDefense);
    }
    return challengeRewardText;
  }

  private static string GetFriendlyChallengeEarlyConcedeMessage(int concederMaxDefense)
  {
    if (DemoMgr.Get().IsDemo())
      return (string) null;
    int num1 = 0;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject != null)
      num1 = netObject.FriendWeekConcededGameMinTotalTurns;
    string key = (string) null;
    int num2 = 0;
    GameState gameState = GameState.Get();
    bool flag1 = false;
    foreach (KeyValuePair<int, Player> player1 in gameState.GetPlayerMap())
    {
      Player player2 = player1.Value;
      switch (player2.GetPreGameOverPlayState())
      {
        case TAG_PLAYSTATE.DISCONNECTED:
        case TAG_PLAYSTATE.CONCEDED:
          flag1 = true;
          Entity hero = player2.GetHero();
          if (hero != null)
          {
            num2 = hero.GetCurrentDefense();
            key = player2.GetSide() != Player.Side.FRIENDLY ? "GLOBAL_FRIENDLYCHALLENGE_REWARD_CONCEDED_YOUR_OPPONENT" : "GLOBAL_FRIENDLYCHALLENGE_REWARD_CONCEDED_YOURSELF";
            goto label_11;
          }
          else
            continue;
        default:
          continue;
      }
    }
label_11:
    bool flag2 = concederMaxDefense > 0;
    bool flag3 = !flag1 || flag2 && num2 <= concederMaxDefense;
    bool flag4 = !flag1 || gameState.GetTurn() >= num1;
    return !flag3 && !flag4 ? GameStrings.Get(key) : (string) null;
  }

  protected void BackToMode(SceneMgr.Mode mode)
  {
    AchieveManager.Get().RemoveAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
    this.HideTwoScoop();
    if (EndGameScreen.OnBackOutOfGameplay != null)
      EndGameScreen.OnBackOutOfGameplay();
    if (this.m_hasAlreadySetMode)
      return;
    this.m_hasAlreadySetMode = true;
    this.StartCoroutine(this.ToMode(mode));
    Navigation.Clear();
  }

  private IEnumerator ToMode(SceneMgr.Mode mode)
  {
    yield return (object) new WaitForSeconds(0.5f);
    SceneMgr.Get().SetNextMode(mode);
  }

  private void ReturnToPreviousMode()
  {
    SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
    GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
    if (postGameSceneMode == SceneMgr.Mode.PVP_DUNGEON_RUN)
      DuelsConfig.Get().SetLastGameResult(GameMgr.Get().LastGameData.GameResult);
    this.BackToMode(postGameSceneMode);
  }

  private void ShowScoreScreen()
  {
    if (!GameState.Get().CanShowScoreScreen())
      return;
    this.m_scoreScreen = GameUtils.LoadGameObjectWithComponent<ScoreScreen>(this.m_ScoreScreenPrefab);
    if (!(bool) (UnityEngine.Object) this.m_scoreScreen)
      return;
    TransformUtil.AttachAndPreserveLocalTransform(this.m_scoreScreen.transform, this.transform);
    LayerUtils.SetLayer((Component) this.m_scoreScreen, GameLayer.IgnoreFullScreenEffects);
    this.m_scoreScreen.Show();
    this.m_showingScoreScreen = true;
    this.SetPlayingBlockingAnim(true);
    this.StartCoroutine(this.WaitThenSetPlayingBlockingAnim(0.65f, false));
    if (!Gameplay.Get().HasBattleNetFatalError())
      return;
    this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_ProceedToError));
  }

  private void HideScoreScreen()
  {
    if (!(bool) (UnityEngine.Object) this.m_scoreScreen)
      return;
    this.m_scoreScreen.Hide();
    this.m_showingScoreScreen = false;
    this.SetPlayingBlockingAnim(true);
    this.StartCoroutine(this.WaitThenSetPlayingBlockingAnim(0.25f, false));
  }

  protected void HideTwoScoop()
  {
    if (!this.m_twoScoop.IsShown())
      return;
    this.m_twoScoop.Hide();
    this.m_noGoldRewardText.gameObject.SetActive(false);
    this.OnTwoScoopHidden();
    if (EndGameScreen.OnTwoScoopsShown != null)
      EndGameScreen.OnTwoScoopsShown(false, this.m_twoScoop);
    if (!((UnityEngine.Object) InputManager.Get() != (UnityEngine.Object) null))
      return;
    InputManager.Get().EnableInput();
  }

  protected void ShowTwoScoop() => this.StartCoroutine(this.ShowTwoScoopWhenReady());

  private IEnumerator ShowTwoScoopWhenReady()
  {
    while ((bool) (UnityEngine.Object) this.m_scoreScreen)
    {
      this.SendTelemetryIfTimeout("ScoreScreen");
      yield return (object) null;
    }
    this.ResetTimeoutTimer();
    if (this.ShouldMakeUtilRequests())
    {
      while (!this.m_netCacheReady && this.SendTelemetryIfTimeout("NetCache"))
        yield return (object) null;
      this.ResetTimeoutTimer();
      this.m_netCacheReady = true;
      while (!this.m_achievesReady)
      {
        this.SendTelemetryIfTimeout("Achieves");
        yield return (object) null;
      }
      this.ResetTimeoutTimer();
    }
    while (!this.m_rewardsLoaded)
    {
      this.SendTelemetryIfTimeout("Rewards");
      yield return (object) null;
    }
    this.ResetTimeoutTimer();
    while (!this.m_twoScoop.IsLoaded())
    {
      this.SendTelemetryIfTimeout("TwoScoop");
      yield return (object) null;
    }
    this.ResetTimeoutTimer();
    while (this.JustEarnedHeroReward())
    {
      this.SendTelemetryIfTimeout("HeroReward");
      if (!this.m_heroRewardEventReady)
        yield return (object) null;
      else
        break;
    }
    this.ResetTimeoutTimer();
    this.m_twoScoop.Show();
    if (!SpectatorManager.Get().IsSpectatingOrWatching && this.ShouldMakeUtilRequests())
      this.InitGoldRewardUI();
    this.OnTwoScoopShown();
    this.m_haveShownTwoScoop = true;
    if (EndGameScreen.OnTwoScoopsShown != null)
      EndGameScreen.OnTwoScoopsShown(true, this.m_twoScoop);
  }

  protected IEnumerator WaitThenSetPlayingBlockingAnim(float sec, bool set)
  {
    yield return (object) new WaitForSeconds(sec);
    this.SetPlayingBlockingAnim(set);
  }

  protected bool ShouldMakeUtilRequests() => Network.ShouldBeConnectedToAurora();

  protected bool IsReady()
  {
    if (!this.m_shown || !this.m_netCacheReady || !this.m_achievesReady || !this.m_rewardsLoaded || (!this.m_rankChangeReady || !this.m_medalInfoUpdated) && this.m_shouldShowRankChange || !((UnityEngine.Object) this.m_rankedRewardDisplay != (UnityEngine.Object) null) && this.m_rankedRewardsToDisplay.Count != 0 || !((UnityEngine.Object) this.m_rankedCardBackProgress != (UnityEngine.Object) null) && this.m_shouldShowRankedCardBackProgress)
      return false;
    return RewardXpNotificationManager.Get().IsReady || !this.m_shouldShowRewardXpGains;
  }

  public bool IsDoneDisplayingRewards() => this.m_doneDisplayingRewards;

  private bool ShowStandardFlowIfReady()
  {
    if (!this.IsReady() && (this.ShouldMakeUtilRequests() || !this.m_shown))
      return false;
    this.SendRankedInitTelemetryIfNeeded();
    this.ShowStandardFlow();
    return true;
  }

  protected virtual void ShowStandardFlow()
  {
    this.ShowTwoScoop();
    if (RewardXpNotificationManager.Get().HasXpGainsToShow)
    {
      this.m_shouldShowRewardXpGains = true;
      RewardXpNotificationManager.Get().InitEndOfGameFlow((Action) null);
      RewardXpNotificationManager.Get().ShowRewardTrackXpGains((Action) (() => this.ContinueEvents()), true);
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_continueText.gameObject.SetActive(true);
  }

  protected virtual void OnNetCacheReady()
  {
    this.m_netCacheReady = true;
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if (this.m_shouldShowRankChange)
    {
      this.RetryMedalInfoRequestIfNeeded();
      this.LoadRankChange();
      this.LoadRankedRewardDisplay();
      this.LoadRankedCardBackProgress();
    }
    this.MaybeUpdateRewards();
  }

  private void RetryMedalInfoRequestIfNeeded()
  {
    if (this.IsMedalInfoRetryNeeded())
    {
      this.StartCoroutine(this.RetryMedalInfoRequest());
    }
    else
    {
      NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheMedalInfo), new Action(this.OnMedalInfoUpdate));
      this.m_medalInfoUpdated = true;
    }
    this.ShowStandardFlowIfReady();
  }

  private bool IsMedalInfoRetryNeeded()
  {
    if (!this.ShouldMakeUtilRequests() || !this.m_shouldShowRankChange || this.m_medalInfoRetryCount >= 3)
      return false;
    PegasusShared.FormatType formatType = Options.GetFormatType();
    MedalInfoTranslator localPlayerMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo();
    return localPlayerMedalInfo == null || localPlayerMedalInfo.GetChangeType(formatType) == RankChangeType.NO_GAME_PLAYED;
  }

  private IEnumerator RetryMedalInfoRequest()
  {
    EndGameScreen endGameScreen = this;
    if (endGameScreen.m_medalInfoRetryCount == 0)
    {
      endGameScreen.m_medalInfoRetryDelay = 1f;
      NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheMedalInfo), new Action(endGameScreen.OnMedalInfoUpdate));
    }
    else
      endGameScreen.m_medalInfoRetryDelay *= 2f;
    ++endGameScreen.m_medalInfoRetryCount;
    yield return (object) new WaitForSeconds(endGameScreen.m_medalInfoRetryDelay);
    NetCache.Get().RefreshNetObject<NetCache.NetCacheMedalInfo>();
  }

  private void OnMedalInfoUpdate() => this.RetryMedalInfoRequestIfNeeded();

  private void SendRankedInitTelemetryIfNeeded()
  {
    if (!this.m_shouldShowRankChange || this.m_hasSentRankedInitTelemetry)
      return;
    this.m_hasSentRankedInitTelemetry = true;
    float elapsedTime = Time.time - this.m_endGameScreenStartTime;
    PegasusShared.FormatType formatType = Options.GetFormatType();
    MedalInfoTranslator localPlayerMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo();
    bool medalInfoRetriesTimedOut = this.m_medalInfoRetryCount >= 3 && (localPlayerMedalInfo == null || localPlayerMedalInfo.GetChangeType(formatType) == RankChangeType.NO_GAME_PLAYED);
    if (medalInfoRetriesTimedOut && localPlayerMedalInfo != null)
      Log.All.PrintError("EndGameScreen_MedalInfoTimeOut elapsedTime={0} retries={1} prev={2} curr={3}", (object) elapsedTime, (object) this.m_medalInfoRetryCount, (object) localPlayerMedalInfo.GetPreviousMedal(formatType).ToString(), (object) localPlayerMedalInfo.GetCurrentMedal(formatType).ToString());
    bool showRankedReward = this.m_rankedRewardsToDisplay.Count > 0;
    TelemetryManager.Client().SendEndGameScreenInit(elapsedTime, this.m_medalInfoRetryCount, medalInfoRetriesTimedOut, showRankedReward, this.m_shouldShowRankedCardBackProgress, this.m_rewards.Count);
  }

  private void LoadRankChange()
  {
    AssetReference twoScoopPrefabNew = RankMgr.RANK_CHANGE_TWO_SCOOP_PREFAB_NEW;
    AssetLoader.Get().InstantiatePrefab(twoScoopPrefabNew, new PrefabCallback<GameObject>(this.OnRankChangeLoaded));
  }

  private void OnRankChangeLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_rankChangeTwoScoop = go;
    this.m_rankChangeTwoScoop.gameObject.SetActive(false);
    this.m_rankChangeReady = true;
    this.ShowStandardFlowIfReady();
  }

  private void OnRankChangeClosed()
  {
    this.m_isShowingRankChange = false;
    this.m_shouldShowRankChange = false;
    this.ContinueEvents();
  }

  private void LoadRankedRewardDisplay()
  {
    if (!RankMgr.Get().GetLocalPlayerMedalInfo().GetRankedRewardsEarned(Options.GetFormatType(), ref this.m_rankedRewardsToDisplay) || this.m_rankedRewardsToDisplay.Count == 0)
      return;
    this.m_rankedRewardDisplayWidget = (Widget) WidgetInstance.Create((string) RankMgr.RANKED_REWARD_DISPLAY_PREFAB);
    this.m_rankedRewardDisplayWidget.RegisterReadyListener((Action<object>) (_ => this.OnRankedRewardDisplayWidgetReady()), (object) null, true);
  }

  private void OnRankedRewardDisplayWidgetReady()
  {
    this.m_rankedRewardDisplay = this.m_rankedRewardDisplayWidget.GetComponentInChildren<RankedRewardDisplay>();
    this.ShowStandardFlowIfReady();
  }

  private void LoadRankedCardBackProgress()
  {
    this.m_shouldShowRankedCardBackProgress = RankMgr.Get().GetLocalPlayerMedalInfo().ShouldShowCardBackProgress();
    if (!this.m_shouldShowRankedCardBackProgress)
      return;
    this.m_rankedCardBackProgressWidget = (Widget) WidgetInstance.Create((string) RankMgr.RANKED_CARDBACK_PROGRESS_DISPLAY_PREFAB);
    this.m_rankedCardBackProgressWidget.RegisterReadyListener((Action<object>) (_ => this.OnRankedCardBackProgressWidgetReady()), (object) null, true);
  }

  private void OnRankedCardBackProgressWidgetReady()
  {
    this.m_rankedCardBackProgress = this.m_rankedCardBackProgressWidget.GetComponentInChildren<RankedCardBackProgressDisplay>();
    this.ShowStandardFlowIfReady();
  }

  private IEnumerator WaitForAchieveManager()
  {
    while (!AchieveManager.Get().IsReady())
      yield return (object) null;
    this.m_achievesReady = true;
    this.MaybeUpdateRewards();
  }

  private void ProcessPreviousAchievements() => this.OnAchievesUpdated(new List<Achievement>(), new List<Achievement>(), (object) null);

  private void OnAchievesUpdated(
    List<Achievement> updatedAchieves,
    List<Achievement> completedAchieves,
    object userData)
  {
    List<Achievement> completedAchievesToShow = AchieveManager.Get().GetNewCompletedAchievesToShow();
    bool flag = PopupDisplayManager.ShouldSuppressPopups();
    foreach (Achievement achievement in completedAchievesToShow)
    {
      Achievement achieve = achievement;
      if ((!flag || achieve.Mode == Achieve.GameMode.MERCENARIES) && achieve.RewardTiming == Achieve.RewardTiming.IMMEDIATE && this.m_completedQuests.Find((Predicate<Achievement>) (obj => achieve.ID == obj.ID)) == null)
        this.m_completedQuests.Add(achieve);
    }
  }

  private void OnGenericRewardUpdated(long rewardNoticeId, object userData)
  {
    this.m_genericRewardChestNoticeIdsReady.Add(rewardNoticeId);
    this.UpdateRewards();
  }

  protected bool HasShownScoops() => this.m_haveShownTwoScoop;

  protected void SetHeroRewardEventReady(bool isReady) => this.m_heroRewardEventReady = isReady;

  private void MaybeUpdateRewards()
  {
    if (!this.m_achievesReady || !this.m_netCacheReady)
      return;
    this.UpdateRewards();
    this.ShowStandardFlowIfReady();
  }

  private void LoadRewards(List<RewardData> rewardsToLoad, Reward.DelOnRewardLoaded callback)
  {
    if (rewardsToLoad == null)
      return;
    foreach (RewardData rewardData in rewardsToLoad)
    {
      if (PopupDisplayManager.Get().RewardPopups.UpdateNoticesSeen(rewardData))
      {
        ++this.m_numRewardsToLoad;
        rewardData.LoadRewardObject(callback);
      }
    }
  }

  private void UpdateRewards()
  {
    bool flag = true;
    if (GameMgr.Get().IsTraditionalTutorial())
      flag = GameUtils.IsTraditionalTutorialComplete();
    List<RewardData> rewardsToShow = (List<RewardData>) null;
    List<RewardData> genericRewardChestsToShow = (List<RewardData>) null;
    List<RewardData> purchasedCardRewardsToShow = (List<RewardData>) null;
    if (flag)
    {
      List<NetCache.ProfileNotice> list = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>().Notices.Where<NetCache.ProfileNotice>((Func<NetCache.ProfileNotice, bool>) (n => n.Type != NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST || this.m_genericRewardChestNoticeIdsReady.Any<long>((Func<long, bool>) (r => n.NoticeID == r)))).ToList<NetCache.ProfileNotice>();
      list.RemoveAll((Predicate<NetCache.ProfileNotice>) (n => n.Origin == NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_DUELS));
      RewardUtils.GetViewableRewards(RewardUtils.GetRewards(list), new HashSet<Achieve.RewardTiming>()
      {
        Achieve.RewardTiming.IMMEDIATE
      }, out rewardsToShow, out genericRewardChestsToShow, ref purchasedCardRewardsToShow, ref this.m_completedQuests);
    }
    else
      rewardsToShow = new List<RewardData>();
    this.JustEarnedHeroReward();
    if (!GameMgr.Get().IsSpectator())
    {
      List<RewardData> customRewards = GameState.Get().GetGameEntity().GetCustomRewards();
      if (customRewards != null)
        rewardsToShow.AddRange((IEnumerable<RewardData>) customRewards);
    }
    this.LoadRewards(rewardsToShow, new Reward.DelOnRewardLoaded(this.OnRewardObjectLoaded));
    this.LoadRewards(genericRewardChestsToShow, new Reward.DelOnRewardLoaded(this.OnGenericRewardObjectLoaded));
    if (this.m_numRewardsToLoad != 0)
      return;
    this.m_rewardsLoaded = true;
  }

  private void OnRewardObjectLoaded(Reward reward, object callbackData) => this.LoadReward(reward, ref this.m_rewards);

  private void OnGenericRewardObjectLoaded(Reward reward, object callbackData) => this.LoadReward(reward, ref this.m_genericRewards);

  private void PositionReward(Reward reward)
  {
    reward.transform.parent = this.transform;
    reward.transform.localRotation = Quaternion.identity;
    reward.transform.localPosition = PopupDisplayManager.Get().RewardPopups.GetRewardLocalPos();
  }

  private void LoadReward(Reward reward, ref List<Reward> allRewards)
  {
    reward.Hide();
    this.PositionReward(reward);
    allRewards.Add(reward);
    --this.m_numRewardsToLoad;
    if (this.m_numRewardsToLoad > 0)
      return;
    RewardUtils.SortRewards(ref allRewards);
    this.m_rewardsLoaded = true;
    this.ShowStandardFlowIfReady();
  }

  private void DisplayLoadedRewardObject(Reward reward, object callbackData)
  {
    if ((UnityEngine.Object) this.m_currentlyShowingReward != (UnityEngine.Object) null)
    {
      this.m_currentlyShowingReward.Hide(true);
      this.m_currentlyShowingReward = (Reward) null;
    }
    reward.Hide();
    this.PositionReward(reward);
    this.m_currentlyShowingReward = reward;
    this.SetPlayingBlockingAnim(true);
    LayerUtils.SetLayer(this.m_currentlyShowingReward.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.ShowReward(this.m_currentlyShowingReward);
  }

  private void ShowReward(Reward reward)
  {
    bool updateCacheValues = !(reward is CardReward);
    RewardUtils.ShowReward(UserAttentionBlocker.NONE, reward, updateCacheValues, PopupDisplayManager.Get().RewardPopups.GetRewardPunchScale(), PopupDisplayManager.Get().RewardPopups.GetRewardScale());
    this.StartCoroutine(this.WaitThenSetPlayingBlockingAnim(0.35f, false));
  }

  protected virtual bool ShowHeroRewardEvent() => false;

  protected bool ShowFixedRewards()
  {
    if (this.m_isShowingFixedRewards)
      return true;
    if (PopupDisplayManager.SuppressPopupsTemporarily)
      return false;
    HashSet<Achieve.RewardTiming> rewardVisualTimings = new HashSet<Achieve.RewardTiming>()
    {
      Achieve.RewardTiming.IMMEDIATE
    };
    FixedRewardsMgr.DelOnAllFixedRewardsShown allRewardsShownCallback = (FixedRewardsMgr.DelOnAllFixedRewardsShown) (() =>
    {
      this.m_isShowingFixedRewards = false;
      this.ContinueEvents();
    });
    this.m_isShowingFixedRewards = FixedRewardsMgr.Get().ShowFixedRewards(UserAttentionBlocker.NONE, rewardVisualTimings, allRewardsShownCallback, (FixedRewardsMgr.DelPositionNonToastReward) null);
    return this.m_isShowingFixedRewards;
  }

  private bool ShowGoldReward()
  {
    int index = this.m_rewards.FindIndex((Predicate<Reward>) (reward => reward.Data is GoldRewardData data && data.Origin == NetCache.ProfileNotice.NoticeOrigin.TOURNEY));
    if (index < 0)
      return false;
    Reward reward1 = this.m_rewards[index];
    this.m_rewards.RemoveAt(index);
    this.m_rewards.Insert(0, reward1);
    this.ShowNextReward();
    return true;
  }

  private bool ShowNextProgressionQuestReward() => QuestManager.Get().ShowNextReward((Action) (() => this.ContinueEvents()));

  protected bool ShowNextCompletedQuest()
  {
    if (this.m_completedQuests.Count == 0)
      return false;
    if (QuestToast.IsQuestActive())
      QuestToast.GetCurrentToast().CloseQuestToast();
    Achievement completedQuest = this.m_completedQuests[0];
    this.m_completedQuests.RemoveAt(0);
    if (!completedQuest.UseGenericRewardVisual)
    {
      bool flag = false;
      foreach (RewardData reward in completedQuest.Rewards)
      {
        if (reward.RewardType == Reward.Type.CARD && reward is CardRewardData cardRewardData)
        {
          TAG_CARD_SET cardSetFromCardId = GameUtils.GetCardSetFromCardID(cardRewardData.CardID);
          flag |= !GameDbf.GetIndex().GetCardSet(cardSetFromCardId).IsCoreCardSet;
        }
      }
      QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, new QuestToast.DelOnCloseQuestToast(this.ShowQuestToastCallback), !flag, completedQuest);
      NarrativeManager.Get().OnQuestCompleteShown(completedQuest.ID);
    }
    else
    {
      completedQuest.AckCurrentProgressAndRewardNotices();
      completedQuest.Rewards[0].LoadRewardObject(new Reward.DelOnRewardLoaded(this.DisplayLoadedRewardObject));
    }
    return true;
  }

  protected void ShowQuestToastCallback(object userData)
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null)
      return;
    this.ContinueEvents();
  }

  protected bool ShowRewardTrackXpGains()
  {
    RewardXpNotificationManager notificationManager = RewardXpNotificationManager.Get();
    if (notificationManager.IsShowingXpGains && !notificationManager.JustShowGameXp)
    {
      notificationManager.TerminateEarly();
      return false;
    }
    if (!notificationManager.HasXpGainsToShow && !notificationManager.JustShowGameXp)
      return false;
    if (notificationManager.IsShowingXpGains && notificationManager.JustShowGameXp)
      notificationManager.ContinueNotifications();
    else
      notificationManager.ShowRewardTrackXpGains((Action) (() => this.ContinueEvents()));
    return true;
  }

  protected bool ShowNextRewardTrackAutoClaimedReward()
  {
    if (this.m_isShowingTrackRewards)
      return true;
    Action callback = (Action) (() =>
    {
      this.m_isShowingTrackRewards = false;
      this.ContinueEvents();
    });
    if (!RewardTrackManager.Get().ShowNextReward(callback))
      return false;
    this.m_isShowingTrackRewards = true;
    return true;
  }

  protected bool ShowNextReward()
  {
    if (this.m_rewards.Count == 0)
      return false;
    this.SetPlayingBlockingAnim(true);
    this.m_currentlyShowingReward = this.m_rewards[0];
    this.m_rewards.RemoveAt(0);
    this.ShowReward(this.m_currentlyShowingReward);
    return true;
  }

  protected bool ShowNextGenericReward()
  {
    if (this.m_genericRewards.Count == 0)
      return false;
    this.SetPlayingBlockingAnim(true);
    this.m_currentlyShowingReward = this.m_genericRewards[0];
    this.m_genericRewards.RemoveAt(0);
    QuestToast.ShowGenericRewardQuestToast(UserAttentionBlocker.NONE, new QuestToast.DelOnCloseQuestToast(this.ShowQuestToastCallback), this.m_currentlyShowingReward.Data, this.m_currentlyShowingReward.Data.NameOverride, this.m_currentlyShowingReward.Data.DescriptionOverride);
    this.StartCoroutine(this.WaitThenSetPlayingBlockingAnim(0.35f, false));
    return true;
  }

  private bool ShowRankChange()
  {
    if (!this.m_shouldShowRankChange)
      return false;
    if (this.m_isShowingRankChange)
      return true;
    this.m_rankChangeTwoScoop.gameObject.SetActive(true);
    RankChangeTwoScoop_NEW component = this.m_rankChangeTwoScoop.GetComponent<RankChangeTwoScoop_NEW>();
    component.Initialize(RankMgr.Get().GetLocalPlayerMedalInfo(), Options.GetFormatType(), new Action(this.OnRankChangeClosed));
    component.Show();
    this.m_isShowingRankChange = true;
    return true;
  }

  private bool ShowRankedRewards()
  {
    if (this.m_rankedRewardsToDisplay.Count == 0)
      return false;
    if (this.m_isShowingRankedReward)
      return true;
    this.m_isShowingRankedReward = true;
    PegasusShared.FormatType formatType = Options.GetFormatType();
    this.m_rankedRewardDisplay.Initialize(RankMgr.Get().GetLocalPlayerMedalInfo().GetCurrentMedal(formatType), this.m_rankedRewardsToDisplay, new Action(this.OnRankedRewardsClosed));
    this.m_rankedRewardDisplay.Show();
    return true;
  }

  private void OnRankedRewardsClosed()
  {
    this.m_isShowingRankedReward = false;
    this.m_rankedRewardsToDisplay.Clear();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_rankedRewardDisplayWidget.gameObject);
    this.ContinueEvents();
  }

  private bool ShowRankedCardBackProgress()
  {
    if (!this.m_shouldShowRankedCardBackProgress)
      return false;
    if (this.m_isShowingRankedCardBackProgress)
      return true;
    this.m_isShowingRankedCardBackProgress = true;
    this.m_rankedCardBackProgress.Initialize(RankMgr.Get().GetLocalPlayerMedalInfo(), new Action(this.OnRankedCardBackProgressClosed));
    this.m_rankedCardBackProgress.Show();
    return true;
  }

  private void OnRankedCardBackProgressClosed()
  {
    this.m_shouldShowRankedCardBackProgress = false;
    this.m_isShowingRankedCardBackProgress = false;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_rankedCardBackProgressWidget.gameObject);
    if (this.FindRankedCardBackRewardAndMakeNext())
      this.ShowNextReward();
    else
      this.ContinueEvents();
  }

  private bool FindRankedCardBackRewardAndMakeNext()
  {
    int currentSeasonId = RankMgr.Get().GetLocalPlayerMedalInfo().GetCurrentSeasonId();
    int rankedCardBackId = RankMgr.Get().GetRankedCardBackIdForSeasonId(currentSeasonId);
    int index = this.m_rewards.FindIndex((Predicate<Reward>) (reward => reward.Data is CardBackRewardData data && data.CardBackID == rankedCardBackId));
    if (index < 0)
      return false;
    Reward reward1 = this.m_rewards[index];
    this.m_rewards.RemoveAt(index);
    this.m_rewards.Insert(0, reward1);
    return true;
  }

  protected virtual bool JustEarnedHeroReward() => false;

  protected virtual bool ShowHealUpDialog() => false;

  protected virtual bool ShowPushNotificationPrompt() => false;

  protected virtual bool ShowAppRatingPrompt() => false;

  protected bool ShowMercenariesExperienceRewards()
  {
    if (this.m_isShowingMercenariesExperienceRewards)
      return true;
    if (this.m_finishedShowingMercenariesExperienceRewards)
      return false;
    if (GameState.Get().GetGameEntity() is LettuceMissionEntity)
    {
      LettuceMissionEntity gameEntity = (LettuceMissionEntity) GameState.Get().GetGameEntity();
      List<MercenaryExpRewardData> source = new List<MercenaryExpRewardData>();
      foreach (MercenariesExperienceUpdate experienceUpdate in gameEntity.GetMercenaryExperienceUpdates())
      {
        if (experienceUpdate.PreExp != experienceUpdate.PostExp)
        {
          MercenaryExpRewardData mercenaryExpRewardData = new MercenaryExpRewardData(experienceUpdate.MercenaryId, (int) experienceUpdate.PreExp, (int) experienceUpdate.PostExp, (int) experienceUpdate.ExpDelta);
          source.Add(mercenaryExpRewardData);
        }
      }
      if (source.Count == 0)
      {
        this.m_finishedShowingMercenariesExperienceRewards = true;
        return false;
      }
      List<MercenaryExpRewardData> list = source.OrderByDescending<MercenaryExpRewardData, int>((Func<MercenaryExpRewardData, int>) (r => r.NumberOfLevelUps)).ToList<MercenaryExpRewardData>();
      AssetLoader.Get().InstantiatePrefab((AssetReference) "MercenariesExperienceTwoScoop.prefab:eb825692c63590b4d8a76def17e8aa3a", new PrefabCallback<GameObject>(this.OnMercenariesExperienceTwoScoopLoaded), (object) list);
      this.m_isShowingMercenariesExperienceRewards = true;
      return true;
    }
    this.m_finishedShowingMercenariesExperienceRewards = true;
    return false;
  }

  private void OnMercenariesExperienceTwoScoopLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintError("Failed to load Mercenaries Experience Two Scoop.");
      this.m_isShowingMercenariesExperienceRewards = false;
      this.m_finishedShowingMercenariesExperienceRewards = true;
    }
    else
    {
      MercenariesExperienceTwoScoop component = go.GetComponent<MercenariesExperienceTwoScoop>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Log.Lettuce.PrintError("MercenariesExperienceTwoScoop game object had no script attached!");
        this.m_isShowingMercenariesExperienceRewards = false;
        this.m_finishedShowingMercenariesExperienceRewards = true;
      }
      else
      {
        List<MercenaryExpRewardData> mercenaryExpRewards = (List<MercenaryExpRewardData>) callbackData;
        component.Initialize(mercenaryExpRewards, new Action(this.OnMercenariesExperienceTwoScoopClosed));
      }
    }
  }

  private void OnMercenariesExperienceTwoScoopClosed()
  {
    this.m_isShowingMercenariesExperienceRewards = false;
    this.m_finishedShowingMercenariesExperienceRewards = true;
    this.ContinueEvents();
  }

  private bool SendTelemetryIfTimeout(string culprit)
  {
    if (this.m_hasTimedOutAndLogged)
      return false;
    if ((double) this.m_timeoutTimerStartTime == 0.0)
      this.m_timeoutTimerStartTime = Time.realtimeSinceStartup;
    float num = Time.realtimeSinceStartup - this.m_timeoutTimerStartTime;
    if ((double) num < 5.0)
      return true;
    TelemetryManager.Client().SendLiveIssue("EndGameScreen_NetCacheReadyTimeout", "Timeout occurred when waiting for m_netCacheReady to be ready, " + string.Format("time elapsed: {0} while waiting for {1}.", (object) num, (object) culprit));
    Log.All.PrintError("Timeout occurred when waiting for m_netCacheReady to be ready, " + string.Format("time elapsed: {0} while waiting for {1}.", (object) num, (object) culprit));
    this.m_hasTimedOutAndLogged = true;
    return false;
  }

  private void ResetTimeoutTimer() => this.m_timeoutTimerStartTime = 0.0f;

  public delegate void OnTwoScoopsShownHandler(bool shown, EndGameTwoScoop twoScoops);
}
