using Assets;
using Blizzard.T5.Services;
using Hearthstone.Login;
using Hearthstone.Progression;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class VictoryScreen : EndGameScreen
{
  public GamesWonIndicator m_gamesWonIndicator;
  public Transform m_goldenHeroEventBone;
  private bool m_showWinProgress;
  private bool m_showHeroRewardEvent;
  private bool m_heroRewardCardDefReady;
  private string m_heroRewardCardID;
  private HeroRewardEvent m_heroRewardEvent;
  private DefLoader.DisposableCardDef m_heroRewardCardDef;
  protected int m_heroRewardAchievementID;
  private const string NO_HERO_REWARD = "none";
  public bool hasCheckedForNewlyEarnedHeroRewards;
  private int? m_newlyCompletedHeroSkinRewardAchievementId;

  protected override void Awake()
  {
    base.Awake();
    this.m_gamesWonIndicator?.Hide();
    if (!this.ShouldMakeUtilRequests())
      return;
    if (GameMgr.Get().IsTraditionalTutorial())
      NetCache.Get().RegisterTutorialEndGameScreen(new NetCache.NetCacheCallback(((EndGameScreen) this).OnNetCacheReady));
    else
      NetCache.Get().RegisterScreenEndOfGame(new NetCache.NetCacheCallback(((EndGameScreen) this).OnNetCacheReady));
    AchievementManager.Get().OnStatusChanged += new AchievementManager.StatusChangedDelegate(this.OnAchievementStatusChanged);
    this.QueueCompletedHeroSkinAchievements();
  }

  protected override void OnDestroy()
  {
    this.m_heroRewardCardDef?.Dispose();
    this.m_heroRewardCardDef = (DefLoader.DisposableCardDef) null;
    base.OnDestroy();
  }

  protected override void ShowStandardFlow()
  {
    base.ShowStandardFlow();
    BattlegroundsEmoteHandler handler;
    if (BattlegroundsEmoteHandler.TryGetActiveInstance(out handler))
      handler.HideEmotes();
    else if ((UnityEngine.Object) EmoteHandler.Get() != (UnityEngine.Object) null)
      EmoteHandler.Get().HideEmotes();
    if ((UnityEngine.Object) TargetReticleManager.Get() != (UnityEngine.Object) null)
    {
      TargetReticleManager.Get().DestroyEnemyTargetArrow();
      TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
    }
    if (!GameMgr.Get().IsTraditionalTutorial() || GameMgr.Get().IsSpectator())
      this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((EndGameScreen) this).ContinueButtonPress_PrevMode));
    else if (GameUtils.IsTraditionalTutorialComplete())
    {
      LoadingScreen.Get().SetFadeColor(Color.white);
      this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_FirstTimeHub));
    }
    else if (DemoMgr.Get().GetMode() == DemoMode.BLIZZ_MUSEUM && GameUtils.GetNextTutorial() == 0)
      this.StartCoroutine(DemoMgr.Get().CompleteBlizzMuseumDemo());
    else
      this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((EndGameScreen) this).ContinueButtonPress_TutorialProgress));
  }

  protected override bool ShowHeroRewardEvent()
  {
    if ((UnityEngine.Object) this.m_heroRewardEvent == (UnityEngine.Object) null)
      return false;
    if (this.m_heroRewardEvent.gameObject.activeInHierarchy)
    {
      this.m_heroRewardEvent.Hide();
      this.m_showHeroRewardEvent = false;
      return false;
    }
    AchievementManager.Get().ClaimAchievementReward(this.m_heroRewardAchievementID);
    this.SetPlayingBlockingAnim(true);
    this.m_heroRewardEvent.RegisterAnimationDoneListener(new HeroRewardEvent.AnimationDoneListener(this.NotifyOfGoldenHeroAnimComplete));
    this.m_twoScoop.StopAnimating();
    this.m_heroRewardEvent.Show();
    this.m_twoScoop.m_heroActor.transform.parent = this.m_heroRewardEvent.m_heroBone;
    this.m_twoScoop.m_heroActor.transform.localPosition = Vector3.zero;
    this.m_twoScoop.m_heroActor.transform.localScale = new Vector3(1.375f, 1.375f, 1.375f);
    return true;
  }

  private bool CheckForNewlyEarnedHeroReward(
    out bool showHeroRewardEvent,
    out string heroRewardCardID,
    out int heroRewardAchievementID)
  {
    showHeroRewardEvent = this.m_showHeroRewardEvent;
    heroRewardCardID = string.Empty;
    heroRewardAchievementID = 0;
    if (this.hasCheckedForNewlyEarnedHeroRewards || !this.GetNewHeroRewardCardIdAndAchievement(out heroRewardCardID, out heroRewardAchievementID))
      return false;
    this.hasCheckedForNewlyEarnedHeroRewards = true;
    showHeroRewardEvent = heroRewardCardID != "none";
    return true;
  }

  private void LoadAnimatedPrefabsForHeroSkins(string heroRewardCardID, int heroRewardAchievementID)
  {
    if (!(heroRewardCardID != "none"))
      return;
    CardPortraitQuality quality = new CardPortraitQuality(3, TAG_PREMIUM.GOLDEN);
    DefLoader.Get().LoadCardDef(heroRewardCardID, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnHeroRewardEventLoaded), quality: quality);
    if (GameUtils.IsHonored1KHeroSkinAchievement(heroRewardAchievementID))
    {
      AssetLoader.Get().InstantiatePrefab((AssetReference) "Hero2PremiumHero.prefab:1115650b4bc229d49a8d45470424f5cd", new PrefabCallback<GameObject>(this.OnHeroRewardEventLoaded));
    }
    else
    {
      if (!GameUtils.IsGolden500HeroSkinAchievement(heroRewardAchievementID))
        return;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "Hero2GoldHero.prefab:a83a85837f828844caba16593ea3c1d0", new PrefabCallback<GameObject>(this.OnHeroRewardEventLoaded));
    }
  }

  private bool TryGetLatestHeroRewardCardIdAndLoadPrefabs()
  {
    bool showHeroRewardEvent;
    string heroRewardCardID;
    int heroRewardAchievementID;
    if (!this.CheckForNewlyEarnedHeroReward(out showHeroRewardEvent, out heroRewardCardID, out heroRewardAchievementID))
      return false;
    this.m_showHeroRewardEvent = showHeroRewardEvent;
    this.m_heroRewardCardID = heroRewardCardID;
    this.m_heroRewardAchievementID = heroRewardAchievementID;
    this.LoadAnimatedPrefabsForHeroSkins(heroRewardCardID, heroRewardAchievementID);
    return true;
  }

  protected override bool JustEarnedHeroReward() => this.TryGetLatestHeroRewardCardIdAndLoadPrefabs();

  protected override bool ShowHealUpDialog() => TemporaryAccountManager.Get().ShowHealUpDialog(GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_02"), GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_04"), TemporaryAccountManager.HealUpReason.WIN_GAME, false, new TemporaryAccountManager.OnHealUpDialogDismissed(this.OnHealUpDialogDismissed));

  private void OnHealUpDialogDismissed() => this.ContinueEvents();

  protected override bool ShowPushNotificationPrompt() => PushNotificationManager.Get().ShowPushNotificationContext(new Action(this.OnPushNotificationDialogDismissed));

  private void OnPushNotificationDialogDismissed() => this.ContinueEvents();

  protected void ContinueButtonPress_FirstTimeHub(UIEvent e)
  {
    if (!this.HasShownScoops())
      return;
    this.HideTwoScoop();
    if (this.ShowNextReward())
    {
      SoundManager.Get().LoadAndPlay((AssetReference) "VO_INNKEEPER_TUT_COMPLETE_05.prefab:c8d19a552e18c7c429946f62102c9460");
    }
    else
    {
      if (this.ShowNextCompletedQuest())
        return;
      this.ContinueButtonPress_Common();
      this.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_FirstTimeHub));
      if (Network.ShouldBeConnectedToAurora())
      {
        this.BackToMode(SceneMgr.Mode.HUB);
      }
      else
      {
        NotificationManager.Get().CreateTutorialDialog("GLOBAL_MEDAL_REWARD_CONGRATULATIONS", "TUTORIAL_MOBILE_COMPLETE_CONGRATS", "GLOBAL_OKAY", new UIEvent.Handler(this.UserPressedStartButton), new Vector2(0.5f, 0.0f), true);
        this.m_hitbox.gameObject.SetActive(false);
        this.m_continueText.gameObject.SetActive(false);
      }
    }
  }

  protected void UserPressedStartButton(UIEvent e)
  {
    ServiceManager.Get<ILoginService>()?.ClearAuthentication();
    this.BackToMode(SceneMgr.Mode.RESET);
  }

  protected override void OnTwoScoopShown()
  {
    if ((UnityEngine.Object) BnetBar.Get() != (UnityEngine.Object) null)
      BnetBar.Get().SuppressLoginTooltip(true);
    if (!this.m_showWinProgress)
      return;
    this.m_gamesWonIndicator?.Show();
  }

  protected override void OnTwoScoopHidden()
  {
    if (!this.m_showWinProgress)
      return;
    this.m_gamesWonIndicator?.Hide();
  }

  private void OnHeroRewardEventLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_heroRewardEvent = go.GetComponent<HeroRewardEvent>();
    this.m_heroRewardEvent.LoadHeroCardDefs(this.m_heroRewardCardID);
    this.StartCoroutine(this.WaitUntilTwoScoopLoaded((AssetReference) this.name, go));
  }

  public void NotifyOfGoldenHeroAnimComplete()
  {
    this.SetPlayingBlockingAnim(false);
    this.m_heroRewardEvent.RemoveAnimationDoneListener(new HeroRewardEvent.AnimationDoneListener(this.NotifyOfGoldenHeroAnimComplete));
  }

  private IEnumerator WaitUntilTwoScoopLoaded(AssetReference assetRef, GameObject go)
  {
    VictoryScreen victoryScreen = this;
    while ((UnityEngine.Object) victoryScreen.m_twoScoop == (UnityEngine.Object) null || !victoryScreen.m_twoScoop.IsLoaded())
      yield return (object) null;
    while (!victoryScreen.m_heroRewardCardDefReady)
      yield return (object) null;
    go.SetActive(false);
    TransformUtil.AttachAndPreserveLocalTransform(go.transform, victoryScreen.m_goldenHeroEventBone);
    Texture portraitTexture = victoryScreen.m_heroRewardCardDef.CardDef.GetPortraitTexture(TAG_PREMIUM.NORMAL);
    victoryScreen.m_heroRewardEvent.SetHeroBurnAwayTexture(portraitTexture);
    victoryScreen.m_heroRewardEvent.SetVictoryTwoScoop((VictoryTwoScoop) victoryScreen.m_twoScoop);
    victoryScreen.SetHeroRewardEventReady(true);
  }

  protected override void InitGoldRewardUI() => this.m_showWinProgress = true;

  private bool GetNewHeroRewardCardIdAndAchievement(
    out string heroRewardCardId,
    out int heroRewardAchievementID)
  {
    heroRewardCardId = "none";
    heroRewardAchievementID = 0;
    if (this.m_newlyCompletedHeroSkinRewardAchievementId.HasValue)
    {
      RewardListDbfRecord rewardListRecord = GameDbf.Achievement.GetRecord(this.m_newlyCompletedHeroSkinRewardAchievementId.Value)?.RewardListRecord;
      if (rewardListRecord == null)
      {
        Log.Gameplay.PrintError("GetNewHeroRewardCardIdAndAchievement no achievement data model for {0}.", (object) this.m_newlyCompletedHeroSkinRewardAchievementId.Value);
        return false;
      }
      foreach (RewardItemDbfRecord rewardItem in rewardListRecord.RewardItems)
      {
        if (rewardItem.RewardType == RewardItem.RewardType.HERO_SKIN)
        {
          heroRewardCardId = rewardItem.CardRecord.NoteMiniGuid;
          heroRewardAchievementID = this.m_newlyCompletedHeroSkinRewardAchievementId.Value;
          return true;
        }
      }
    }
    return false;
  }

  private void OnHeroRewardEventLoaded(
    string cardId,
    DefLoader.DisposableCardDef def,
    object userData)
  {
    this.m_heroRewardCardDef?.Dispose();
    this.m_heroRewardCardDef = def;
    this.m_heroRewardCardDefReady = true;
  }

  public void OnAchievementStatusChanged(
    int achievementId,
    AchievementManager.AchievementStatus status)
  {
    if (status != AchievementManager.AchievementStatus.COMPLETED || !GameUtils.IsGolden500HeroSkinAchievement(achievementId) && !GameUtils.IsHonored1KHeroSkinAchievement(achievementId))
      return;
    AchievementDbfRecord record = GameDbf.Achievement.GetRecord(achievementId);
    if (record?.RewardListRecord == null || record.RewardListRecord.RewardItems.FirstOrDefault<RewardItemDbfRecord>((Func<RewardItemDbfRecord, bool>) (x => x.RewardType == RewardItem.RewardType.HERO_SKIN)) == null)
      return;
    this.m_newlyCompletedHeroSkinRewardAchievementId = new int?(achievementId);
    this.hasCheckedForNewlyEarnedHeroRewards = false;
  }

  private void QueueCompletedHeroSkinAchievements()
  {
    if (SpectatorManager.Get().IsSpectatingOrWatching)
      return;
    Entity hero = GameState.Get().GetLocalSidePlayer()?.GetHero();
    if (hero == null)
      return;
    TAG_CLASS key = hero.GetClass();
    GameUtils.HeroSkinAchievements skinAchievements;
    if (!GameUtils.HERO_SKIN_ACHIEVEMENTS.TryGetValue(key, out skinAchievements))
      return;
    if (AchievementManager.Get().GetAchievementDataModel(skinAchievements.Golden500Win).Status == AchievementManager.AchievementStatus.COMPLETED)
    {
      this.m_newlyCompletedHeroSkinRewardAchievementId = new int?(skinAchievements.Golden500Win);
    }
    else
    {
      if (AchievementManager.Get().GetAchievementDataModel(skinAchievements.Honored1kWin).Status != AchievementManager.AchievementStatus.COMPLETED)
        return;
      this.m_newlyCompletedHeroSkinRewardAchievementId = new int?(skinAchievements.Honored1kWin);
    }
  }
}
