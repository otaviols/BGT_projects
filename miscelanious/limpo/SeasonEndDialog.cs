using Assets;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone.DataModels;
using Hearthstone.UI;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonEndDialog : DialogBase
{
  public GameObject m_root;
  public UIBButton m_okayButton;
  public GameObject m_boostedMedalBone;
  public GameObject m_boostedMedalLeftFiligreeBone;
  public GameObject m_boostedMedalRightFiligreeBone;
  public GameObject m_rewardChestPage;
  public PegUIElement m_rewardChestLegacy;
  public UberText m_rewardChestHeader;
  public UberText m_rewardChestInstructions;
  public GameObject m_rewardChestLeftFiligreeBone;
  public GameObject m_rewardChestRightFiligreeBone;
  public GameObject m_rewardBoxesBone;
  public AsyncReference m_rankedMedalWidgetReference;
  public AsyncReference m_starMultiplierWidgetReference;
  public AsyncReference m_rankedRewardChestWidgetReference;
  public UberText m_header;
  public UberText m_rankAchieved;
  public UberText m_rankName;
  public GameObject m_ribbon;
  public GameObject m_nameFlourish;
  public GameObject m_boostedFlourish;
  public GameObject m_welcomeItems;
  public GameObject m_leftFiligree;
  public GameObject m_rightFiligree;
  public UberText m_welcomeDetails;
  public UberText m_welcomeTitle;
  public GameObject m_shieldIcon;
  public GameObject m_bonusStarItems;
  public UberText m_bonusStarTitle;
  public UberText m_bonusStarLabel;
  public UberText m_bonusStarFinePrint;
  public GameObject m_bonusStarFlourish;
  public Material m_transparentMaterial;
  public PlayMakerFSM m_medalPlayMaker;
  public GameObject m_seasonFramePage;
  public GameObject m_legendaryGem;
  public List<PegUIElement> m_rewardChests;
  public GameObject m_reminderChestRightFiligreeBone;
  public GameObject m_reminderChestLeftFiligreeBone;
  public GameObject m_reminderRewardsChest;
  public ProgressBar m_progressBar;
  public AsyncReference m_rankedCardBackProgressWidgetReference;
  public UberText m_cardBackReminderDetails;
  public AsyncReference m_rankedIntroPopUpWidgetReference;
  private SeasonEndDialog.SeasonEndInfo m_seasonEndInfo;
  private TranslatedMedalInfo m_seasonBestMedalInfo;
  private TranslatedMedalInfo m_seasonEndMedalInfo;
  private TranslatedMedalInfo m_currentMedalInfo;
  private bool m_earnedRewardChest;
  private bool m_wasPrevSeasonLegacy;
  private bool m_isNewSeasonLegacy;
  private SeasonEndDialog.MODE m_currentMode;
  private RankedMedalWrapper m_rankedMedal;
  private RankedPlayDataModel m_seasonBestRankedDataModel;
  private RankedPlayDataModel m_currentRankedDataModel;
  private RankedPlayDataModel m_rankedChestDataModel;
  private Widget m_rankedMedalWidget;
  private bool m_showMedal;
  private bool m_chestOpened;
  private Widget m_starMultiplierWidget;
  private Widget m_rankedCardBackProgressWidget;
  private Widget m_rankedIntroPopUpWidget;
  private bool m_skipRankedIntroPopup;
  private Widget m_rankedRewardChestWidget;
  private NetCache.NetCacheRewardProgress m_rewardProgress;
  private bool m_isOkayButtonHidden;
  private const string REWARD_CHEST_NAME_STRING_FORMAT = "GLOBAL_REWARD_CHEST_TIER{0}";
  private const string REWARD_CHEST_EARNED_STRING_FORMAT = "GLOBAL_REWARD_CHEST_TIER{0}_EARNED";

  protected override void Awake()
  {
    base.Awake();
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    if (UniversalInputManager.Get().IsTouchMode())
      this.m_rewardChestInstructions.Text = GameStrings.Format("GLOBAL_SEASON_END_CHEST_INSTRUCTIONS_TOUCH");
    this.m_okayButton.SetText(GameStrings.Get("GLOBAL_BUTTON_NEXT"));
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OkayButtonReleased));
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheRewardProgress), new Action(this.OnNetCacheRewardProgressUpdated));
    NetCache.Get().ReloadNetObject<NetCache.NetCacheRewardProgress>();
  }

  private void Start()
  {
    this.m_rankedMedalWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRankedMedalWidgetReady));
    this.m_starMultiplierWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnStarMultiplierWidgetReady));
    this.m_rankedCardBackProgressWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRankedCardBackProgressWidgetReady));
    this.m_rankedIntroPopUpWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRankedIntroPopUpWidgetReady));
    this.m_rankedRewardChestWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRankedRewardChestWidgetReady));
  }

  public void Init(SeasonEndDialog.SeasonEndInfo info)
  {
    this.m_seasonEndInfo = info;
    this.m_header.Text = GameUtils.GetRankedSeasonName(info.m_seasonID);
    this.m_earnedRewardChest = info.m_rankedRewards != null && info.m_rankedRewards.Count > 0;
    this.m_seasonEndMedalInfo = MedalInfoTranslator.CreateTranslatedMedalInfo(info.m_formatType, info.m_leagueId, info.m_starLevelAtEndOfSeason, info.m_legendIndex);
    this.m_seasonBestMedalInfo = MedalInfoTranslator.CreateTranslatedMedalInfo(info.m_formatType, info.m_leagueId, info.m_bestStarLevelAtEndOfSeason, info.m_legendIndex);
    this.m_currentMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo().GetCurrentMedal(info.m_formatType);
    this.m_wasPrevSeasonLegacy = RankMgr.Get().UseLegacyRankedPlay(this.m_seasonEndInfo.m_leagueId);
    this.m_isNewSeasonLegacy = RankMgr.Get().UseLegacyRankedPlay(this.m_currentMedalInfo.leagueId);
    this.m_seasonBestRankedDataModel = this.m_seasonBestMedalInfo.CreateDataModel(RankedMedal.DisplayMode.Default);
    this.m_rankedChestDataModel = this.m_seasonBestMedalInfo.CreateDataModel(RankedMedal.DisplayMode.Chest);
    this.m_currentRankedDataModel = this.m_currentMedalInfo.CreateDataModel(RankedMedal.DisplayMode.Default);
    this.m_showMedal = true;
    this.m_rankName.Text = this.m_seasonBestMedalInfo.GetRankName();
    this.m_cardBackReminderDetails.Text = GameStrings.Format("GLOBAL_REMINDER_CARDBACK_SEASON_END_DIALOG", (object) RankMgr.Get().GetLocalPlayerMedalInfo().GetSeasonCardBackMinWins());
    foreach (Component rewardChest in this.m_rewardChests)
      rewardChest.gameObject.SetActive(false);
    if (this.m_earnedRewardChest && this.m_wasPrevSeasonLegacy)
      this.InitLegacyChest();
    this.m_progressBar.SetProgressBar(0.0f);
  }

  private void InitLegacyChest()
  {
    this.m_rewardChestLegacy = this.m_rewardChests[this.m_seasonBestMedalInfo.RankConfig.RewardChestVisualIndex];
    this.m_rewardChestLegacy.gameObject.SetActive(true);
    this.m_rewardChestLegacy.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.LegacyChestButtonReleased));
    this.m_medalPlayMaker.FsmVariables.GetFsmGameObject("RankChest").Value = this.m_rewardChestLegacy.gameObject;
    UberText[] componentsInChildren = this.m_rewardChestLegacy.GetComponentsInChildren<UberText>(true);
    if (componentsInChildren.Length != 0)
      componentsInChildren[0].Text = this.m_seasonBestMedalInfo.GetMedalText();
    this.m_rewardChestHeader.Text = this.GetChestEarnedText();
  }

  private void InitNewChest()
  {
    PlayMakerFSM componentInChildren = this.m_rankedRewardChestWidget.GetComponentInChildren<PlayMakerFSM>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      this.m_medalPlayMaker.FsmVariables.GetFsmGameObject("RankChest").Value = componentInChildren.gameObject;
    this.m_rewardChestHeader.Text = "GLOBAL_REWARD_CHEST_HEADER";
  }

  protected override void OnDestroy()
  {
    SceneMgr service;
    if (!ServiceManager.TryGet<SceneMgr>(out service))
      return;
    service.UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
  }

  public void ShowMedal()
  {
    this.m_showMedal = true;
    this.UpdateRankedMedalWidget();
  }

  public void HideMedal()
  {
    this.m_showMedal = false;
    this.UpdateRankedMedalWidget();
  }

  private void ShowNewRewardChestWidget()
  {
    if (this.m_wasPrevSeasonLegacy)
      return;
    this.m_rankedRewardChestWidget.Show();
  }

  public void ShowRewardChestPage()
  {
    this.m_rewardChestPage.SetActive(true);
    this.m_leftFiligree.transform.position = this.m_rewardChestLeftFiligreeBone.transform.position;
    this.m_rightFiligree.transform.position = this.m_rewardChestRightFiligreeBone.transform.position;
    iTween.FadeTo(this.m_leftFiligree.gameObject, 1f, 0.5f);
    iTween.FadeTo(this.m_rightFiligree.gameObject, 1f, 0.5f);
    if (!this.m_wasPrevSeasonLegacy || !this.m_seasonBestMedalInfo.IsLegendRank())
      return;
    this.m_legendaryGem.SetActive(true);
  }

  public void HideRewardChestPage()
  {
    this.m_rewardChestPage.SetActive(false);
    if (this.m_wasPrevSeasonLegacy)
      return;
    this.m_rankedRewardChestWidget.Hide();
  }

  private void DisableOkayButton(bool hideButton)
  {
    if (hideButton && !this.m_isOkayButtonHidden)
    {
      this.m_okayButton.Flip(false);
      this.m_isOkayButtonHidden = true;
    }
    this.m_okayButton.SetEnabled(false);
    this.m_okayButton.GetComponent<UIBHighlight>().Reset();
  }

  private void EnableOkayButton()
  {
    if (this.m_isOkayButtonHidden)
    {
      this.m_okayButton.Flip(true);
      this.m_isOkayButtonHidden = false;
    }
    this.m_okayButton.SetEnabled(true);
  }

  public void MedalAnimationFinished()
  {
    if (this.m_currentMode == SeasonEndDialog.MODE.REDUCED_WELCOME)
    {
      if (this.m_isNewSeasonLegacy)
        this.GotoChestReminder();
      else
        this.GoToCardBackReminder();
    }
    else if (this.m_earnedRewardChest)
    {
      this.DisableOkayButton(true);
      this.m_currentMode = SeasonEndDialog.MODE.CHEST_EARNED;
      this.m_medalPlayMaker.SendEvent("RevealRewardChest");
      iTween.FadeTo(this.m_rankAchieved.gameObject, 0.0f, 0.5f);
    }
    else
      this.GotoBonusStarsOrWelcome();
  }

  public void GotoBonusStarsOrWelcome()
  {
    if (!this.m_isNewSeasonLegacy && this.m_seasonEndMedalInfo.LeagueConfig.LeagueType != League.LeagueType.NEW_PLAYER)
    {
      long num = 0;
      int introSeenRequirement = this.m_currentMedalInfo.LeagueConfig.RankedIntroSeenRequirement;
      GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_INTRO_SEEN_COUNT, out num);
      if (num < (long) introSeenRequirement)
        this.GoToRankedIntroPopUp();
      else
        this.m_skipRankedIntroPopup = true;
    }
    string rankedSeasonName = GameUtils.GetRankedSeasonName(this.m_rewardProgress.Season);
    this.m_header.Text = rankedSeasonName;
    if (this.m_currentMedalInfo.starsPerWin > 1 && !this.m_isNewSeasonLegacy)
      this.GoToStarMultiplier();
    else if (this.m_currentMedalInfo.starLevel < this.m_seasonEndMedalInfo.starLevel && this.m_wasPrevSeasonLegacy)
      this.GotoReducedMedal();
    else if (!this.m_earnedRewardChest)
      this.GotoSeasonWelcome(rankedSeasonName);
    else if (this.m_isNewSeasonLegacy)
      this.GotoChestReminder();
    else
      this.GoToCardBackReminder();
  }

  public void GoToStarMultiplier()
  {
    this.m_currentMode = SeasonEndDialog.MODE.STAR_MULTIPLIER;
    this.m_welcomeItems.SetActive(false);
    if (this.m_skipRankedIntroPopup)
    {
      this.StartCoroutine(this.DoPageTear());
    }
    else
    {
      this.HideRewardChestPage();
      this.m_bonusStarItems.SetActive(true);
      this.m_bonusStarTitle.Text = GameStrings.Get("GLOBAL_SEASON_END_STAR_MULTIPLIER_TITLE");
      this.m_bonusStarLabel.Text = GameStrings.Get("GLOBAL_SEASON_END_STAR_MULTIPLIER_LABEL");
      this.StartCoroutine(this.FadeWidgetIn(this.m_starMultiplierWidget, 0.0f));
      iTween.FadeTo(this.m_bonusStarItems, 1f, 0.0f);
      this.EnableOkayButton();
    }
  }

  public void GotoReducedMedal()
  {
    this.m_currentMode = SeasonEndDialog.MODE.REDUCED_WELCOME;
    this.StartCoroutine(this.DoPageTear());
    this.HideRewardChestPage();
    this.m_welcomeItems.SetActive(false);
    this.m_bonusStarItems.SetActive(true);
    this.UpdateRankedMedalWidget();
    this.m_bonusStarLabel.Text = this.m_currentMedalInfo.GetRankName();
    this.m_bonusStarTitle.Text = GameStrings.Get("GLOBAL_SEASON_END_BONUS_STAR_TITLE");
    this.UpdateBonusStarFinePrint();
  }

  public void GotoChestReminder()
  {
    this.m_currentMode = SeasonEndDialog.MODE.REMINDER_CHEST;
    this.HideRewardChestPage();
    this.m_welcomeItems.SetActive(false);
    this.m_bonusStarItems.SetActive(false);
    this.StartCoroutine(this.DoPageTear());
    this.m_progressBar.SetLabel(GameStrings.Format("GLOBAL_REWARD_PROGRESS", (object) 0, (object) RankMgr.Get().GetLeagueRecord(this.m_seasonEndInfo.m_leagueId).SeasonRollRewardMinWins));
  }

  public void GoToCardBackReminder()
  {
    this.m_currentMode = SeasonEndDialog.MODE.REMINDER_CARDBACK;
    this.HideRewardChestPage();
    this.m_welcomeItems.SetActive(false);
    this.m_bonusStarItems.SetActive(false);
    this.StartCoroutine(this.DoPageTear());
  }

  public void GoToRankedIntroPopUp()
  {
    iTween.ScaleTo(this.m_root, new Vector3(0.0f, 0.0f, 0.0f), 0.5f);
    this.m_rankedIntroPopUpWidget.TriggerEvent("CODE_DIALOGMANAGER_SHOW");
  }

  private void ReminderChestSummonOutFinished() => this.Finish();

  public void GotoSeasonWelcome(string newSeasonName)
  {
    this.m_currentMode = SeasonEndDialog.MODE.SEASON_WELCOME;
    this.StartCoroutine(this.DoPageTear());
    this.m_welcomeItems.SetActive(true);
    this.HideRewardChestPage();
    this.m_bonusStarItems.SetActive(false);
    this.m_welcomeDetails.Text = GameStrings.Format("GLOBAL_SEASON_END_NEW_SEASON", (object) newSeasonName);
  }

  public IEnumerator DoPageTear()
  {
    SeasonEndDialog seasonEndDialog = this;
    seasonEndDialog.m_medalPlayMaker.SendEvent("PageTear");
    yield return (object) new WaitForSeconds(0.69f);
    bool flag = false;
    if (seasonEndDialog.m_currentMode == SeasonEndDialog.MODE.REMINDER_CHEST)
    {
      seasonEndDialog.m_leftFiligree.transform.position = seasonEndDialog.m_reminderChestLeftFiligreeBone.transform.position;
      seasonEndDialog.m_rightFiligree.transform.position = seasonEndDialog.m_reminderChestRightFiligreeBone.transform.position;
      iTween.FadeTo(seasonEndDialog.m_leftFiligree.gameObject, 1f, 0.5f);
      iTween.FadeTo(seasonEndDialog.m_rightFiligree.gameObject, 1f, 0.5f);
      seasonEndDialog.m_reminderRewardsChest.SetActive(true);
      seasonEndDialog.m_reminderRewardsChest.GetComponent<PlayMakerFSM>().SendEvent("SummonIn");
      seasonEndDialog.EnableOkayButton();
      seasonEndDialog.m_okayButton.SetText("GLOBAL_DONE");
    }
    else if (seasonEndDialog.m_currentMode == SeasonEndDialog.MODE.REDUCED_WELCOME)
    {
      seasonEndDialog.m_leftFiligree.transform.position = seasonEndDialog.m_boostedMedalLeftFiligreeBone.transform.position;
      seasonEndDialog.m_rightFiligree.transform.position = seasonEndDialog.m_boostedMedalRightFiligreeBone.transform.position;
      if (seasonEndDialog.m_seasonBestMedalInfo.IsLegendRank())
        seasonEndDialog.m_medalPlayMaker.SendEvent("JustMedalIn");
      else
        seasonEndDialog.m_medalPlayMaker.SendEvent("MedalBannerIn");
      flag = true;
    }
    else if (seasonEndDialog.m_currentMode == SeasonEndDialog.MODE.STAR_MULTIPLIER)
    {
      seasonEndDialog.HideRewardChestPage();
      seasonEndDialog.m_bonusStarItems.SetActive(true);
      seasonEndDialog.m_bonusStarTitle.Text = GameStrings.Get("GLOBAL_SEASON_END_STAR_MULTIPLIER_TITLE");
      seasonEndDialog.m_bonusStarLabel.Text = GameStrings.Get("GLOBAL_SEASON_END_STAR_MULTIPLIER_LABEL");
      seasonEndDialog.StartCoroutine(seasonEndDialog.FadeWidgetIn(seasonEndDialog.m_starMultiplierWidget, 0.5f));
      iTween.FadeTo(seasonEndDialog.m_bonusStarItems, 1f, 0.5f);
      seasonEndDialog.EnableOkayButton();
    }
    else if (seasonEndDialog.m_currentMode == SeasonEndDialog.MODE.REMINDER_CARDBACK)
    {
      seasonEndDialog.m_rankedCardBackProgressWidget.Show();
      seasonEndDialog.m_cardBackReminderDetails.Show();
      seasonEndDialog.m_okayButton.SetText("GLOBAL_DONE");
    }
    if (!flag)
      seasonEndDialog.EnableOkayButton();
  }

  public void MedalInFinished() => this.EnableOkayButton();

  public override void Show() => this.StartCoroutine(this.ShowWhenReady());

  private IEnumerator ShowWhenReady()
  {
    SeasonEndDialog seasonEndDialog = this;
    while (seasonEndDialog.m_rewardProgress == null || (UnityEngine.Object) seasonEndDialog.m_rankedMedal == (UnityEngine.Object) null || seasonEndDialog.m_rankedMedalWidget.IsChangingStates || (UnityEngine.Object) seasonEndDialog.m_starMultiplierWidget == (UnityEngine.Object) null || seasonEndDialog.m_starMultiplierWidget.IsChangingStates || (UnityEngine.Object) seasonEndDialog.m_rankedCardBackProgressWidget == (UnityEngine.Object) null || seasonEndDialog.m_rankedCardBackProgressWidget.IsChangingStates || (UnityEngine.Object) seasonEndDialog.m_rankedRewardChestWidget == (UnityEngine.Object) null || seasonEndDialog.m_rankedRewardChestWidget.IsChangingStates)
      yield return (object) null;
    if (seasonEndDialog.m_earnedRewardChest && !seasonEndDialog.m_wasPrevSeasonLegacy)
      seasonEndDialog.InitNewChest();
    SeasonEndDialog.FadeEffectsIn();
    // ISSUE: reference to a compiler-generated method
    seasonEndDialog.\u003C\u003En__0();
    seasonEndDialog.DoShowAnimation();
    UniversalInputManager.Get().SetGameDialogActive(true);
    SeasonEndDialog.PlayShowSound();
  }

  public override void Hide()
  {
    this.m_seasonFramePage.SetActive(false);
    base.Hide();
    SeasonEndDialog.FadeEffectsOut();
    SeasonEndDialog.PlayHideSound();
  }

  protected override void OnHideAnimFinished()
  {
    UniversalInputManager.Get().SetGameDialogActive(false);
    base.OnHideAnimFinished();
  }

  private void Finish()
  {
    this.DisableOkayButton(false);
    this.Hide();
    foreach (long id in this.m_seasonEndInfo.m_noticesToAck)
      Network.Get().AckNotice(id);
  }

  private void OkayButtonReleased(UIEvent e)
  {
    this.DisableOkayButton(false);
    if (this.m_currentMode == SeasonEndDialog.MODE.REMINDER_CHEST)
      this.m_reminderRewardsChest.GetComponent<PlayMakerFSM>().SendEvent("SummonOut");
    else if (this.m_currentMode == SeasonEndDialog.MODE.SEASON_WELCOME || this.m_currentMode == SeasonEndDialog.MODE.REDUCED_WELCOME)
    {
      RendererExtension.SetMaterial(this.m_boostedFlourish.GetComponent<Renderer>(), this.m_transparentMaterial);
      iTween.FadeTo(this.m_bonusStarItems.gameObject, 0.0f, 0.5f);
      iTween.FadeTo(this.m_boostedFlourish.gameObject, 0.0f, 0.5f);
      iTween.FadeTo(this.m_leftFiligree.gameObject, 0.0f, 0.5f);
      iTween.FadeTo(this.m_rightFiligree.gameObject, 0.0f, 0.5f);
      if (this.m_currentMode == SeasonEndDialog.MODE.SEASON_WELCOME)
      {
        this.m_welcomeItems.SetActive(false);
        if (this.m_isNewSeasonLegacy)
          this.GotoChestReminder();
        else
          this.GoToCardBackReminder();
      }
      else
        this.m_medalPlayMaker.SendEvent("JustMedalNoRibbon");
    }
    else if (this.m_currentMode == SeasonEndDialog.MODE.RANK_EARNED)
    {
      RendererExtension.SetMaterial(this.m_ribbon.GetComponent<Renderer>(), this.m_transparentMaterial);
      RendererExtension.SetMaterial(this.m_nameFlourish.GetComponent<Renderer>(), this.m_transparentMaterial);
      iTween.FadeTo(this.m_nameFlourish.gameObject, 0.0f, 0.5f);
      iTween.FadeTo(this.m_rankName.gameObject, iTween.Hash((object) "alpha", (object) 0, (object) "time", (object) 0.5f, (object) "oncomplete", (object) "OnRankNameHidden", (object) "oncompletetarget", (object) this.gameObject));
      iTween.FadeTo(this.m_rankAchieved.gameObject, 0.0f, 0.5f);
      iTween.FadeTo(this.m_leftFiligree.gameObject, 0.0f, 0.5f);
      iTween.FadeTo(this.m_rightFiligree.gameObject, 0.0f, 0.5f);
      if (this.m_seasonBestMedalInfo.IsLegendRank())
        this.m_medalPlayMaker.SendEvent("JustMedal");
      else
        this.m_medalPlayMaker.SendEvent("MedalBanner");
    }
    else if (this.m_currentMode == SeasonEndDialog.MODE.STAR_MULTIPLIER)
    {
      this.StartCoroutine(this.FadeWidgetOut(this.m_starMultiplierWidget, 0.5f));
      this.GoToCardBackReminder();
    }
    else
    {
      if (this.m_currentMode != SeasonEndDialog.MODE.REMINDER_CARDBACK)
        return;
      this.m_rankedCardBackProgressWidget.Hide();
      this.Finish();
    }
  }

  private void LegacyChestButtonReleased(UIEvent e)
  {
    if (this.m_chestOpened)
      return;
    this.m_chestOpened = true;
    this.m_rewardChestLegacy.GetComponent<PlayMakerFSM>().SendEvent("StartAnim");
  }

  private void OpenRewards()
  {
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      if (SoundManager.Get() != null)
        SoundManager.Get().LoadAndPlay((AssetReference) "card_turn_over_legendary.prefab:a8140f686bff601459e954bc23de35e0");
      RewardBoxesDisplay component = go.GetComponent<RewardBoxesDisplay>();
      component.SetRewards(this.m_seasonEndInfo.m_rankedRewards);
      component.m_playBoxFlyoutSound = false;
      component.SetLayer(GameLayer.PerspectiveUI);
      component.UseDarkeningClickCatcher(true);
      component.RegisterDoneCallback((Action) (() =>
      {
        if (this.m_wasPrevSeasonLegacy)
        {
          this.m_rewardChestLegacy.GetComponent<PlayMakerFSM>().SendEvent("SummonOut");
        }
        else
        {
          PlayMakerFSM componentInChildren = this.m_rankedRewardChestWidget.GetComponentInChildren<PlayMakerFSM>();
          FsmGameObject fsmGameObject = componentInChildren.FsmVariables.GetFsmGameObject("OwnerObject");
          if (fsmGameObject != null)
            fsmGameObject.Value = this.gameObject;
          componentInChildren.SendEvent("SummonOut");
        }
      }));
      component.transform.localPosition = this.m_rewardBoxesBone.transform.localPosition;
      component.transform.localRotation = this.m_rewardBoxesBone.transform.localRotation;
      component.transform.localScale = this.m_rewardBoxesBone.transform.localScale;
      component.AnimateRewards();
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) RewardBoxesDisplay.GetPrefab(this.m_seasonEndInfo.m_rankedRewards), callback);
    iTween.FadeTo(this.m_rewardChestInstructions.gameObject, 0.0f, 0.5f);
  }

  public static void FadeEffectsIn()
  {
    ScreenEffectParameters vignettePerspective = ScreenEffectParameters.BlurVignettePerspective with
    {
      Blur = new BlurParameters(brightness: 1f)
    };
    DialogBase.m_screenEffectsHandle.StartEffect(vignettePerspective);
  }

  public static void FadeEffectsOut() => DialogBase.m_screenEffectsHandle.StopEffect();

  public static void PlayShowSound() => SoundManager.Get().LoadAndPlay((AssetReference) "rank_window_expand.prefab:9f3f1c260a5d8b34f9705caf4925f5cb");

  public static void PlayHideSound() => SoundManager.Get().LoadAndPlay((AssetReference) "rank_window_shrink.prefab:9c6393a1d207a07439c22f31ef405a7c");

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode == SceneMgr.Mode.HUB)
      return;
    this.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void UpdateBonusStarFinePrint()
  {
    if (this.m_seasonEndInfo.m_wasLimitedByBestEverStarLevel)
    {
      this.m_bonusStarFinePrint.Text = GameStrings.Format("GLOBAL_SEASON_END_BEST_EVER_ABOVE_MAX", (object) this.m_currentMedalInfo.GetMedalText());
      this.m_bonusStarFinePrint.Show();
    }
    else
      this.m_bonusStarFinePrint.Hide();
  }

  private void OnRankNameHidden() => this.m_rankName.gameObject.SetActive(false);

  private void OnRankedMedalWidgetReady(Widget widget)
  {
    this.m_rankedMedalWidget = widget;
    this.m_rankedMedal = widget.GetComponentInChildren<RankedMedalWrapper>();
    this.UpdateRankedMedalWidget();
  }

  private void OnStarMultiplierWidgetReady(Widget widget)
  {
    this.m_starMultiplierWidget = widget;
    IDataModel model;
    if (!this.m_starMultiplierWidget.GetDataModel(123, out model))
    {
      model = (IDataModel) new RankedPlayDataModel();
      this.m_starMultiplierWidget.BindDataModel(model);
    }
    if (model is RankedPlayDataModel rankedPlayDataModel)
      rankedPlayDataModel.StarMultiplier = this.m_currentMedalInfo.starsPerWin;
    this.StartCoroutine(this.FadeWidgetOut(this.m_starMultiplierWidget, 0.0f));
  }

  private void OnRankedCardBackProgressWidgetReady(Widget widget)
  {
    this.m_rankedCardBackProgressWidget = widget;
    this.UpdateRankedCardBackWidget();
    this.m_cardBackReminderDetails.Hide();
    this.m_rankedCardBackProgressWidget.Hide();
  }

  private void OnRankedIntroPopUpWidgetReady(Widget widget)
  {
    this.m_rankedIntroPopUpWidget = widget;
    widget.RegisterEventListener(new Widget.EventListenerDelegate(this.RankedIntroPopUpEventListener));
  }

  private void RankedIntroPopUpEventListener(string eventName)
  {
    if (!eventName.Equals("HIDE"))
      return;
    iTween.ScaleTo(this.m_root, new Vector3(1f, 1f, 1f), 0.5f);
  }

  private void OnRankedRewardChestWidgetReady(Widget widget)
  {
    this.m_rankedRewardChestWidget = widget;
    if (this.m_wasPrevSeasonLegacy)
    {
      this.m_rankedRewardChestWidget.gameObject.SetActive(false);
    }
    else
    {
      this.m_rankedRewardChestWidget.Hide();
      this.m_rankedRewardChestWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.RankedChestEventListener));
      this.m_rankedRewardChestWidget.BindDataModel((IDataModel) this.m_rankedChestDataModel);
    }
  }

  private void RankedChestEventListener(string eventName)
  {
    if (!eventName.Equals("CLICKED") || this.m_chestOpened)
      return;
    this.m_chestOpened = true;
    PlayMakerFSM componentInChildren = this.m_rankedRewardChestWidget.GetComponentInChildren<PlayMakerFSM>();
    FsmGameObject fsmGameObject = componentInChildren.FsmVariables.GetFsmGameObject("OwnerObject");
    if (fsmGameObject != null)
      fsmGameObject.Value = this.gameObject;
    componentInChildren.SendEvent("StartAnim");
  }

  private void UpdateRankedCardBackWidget()
  {
    if ((UnityEngine.Object) this.m_rankedCardBackProgressWidget == (UnityEngine.Object) null || this.m_rewardProgress == null)
      return;
    IDataModel model;
    if (!this.m_rankedCardBackProgressWidget.GetDataModel(26, out model))
    {
      model = (IDataModel) new CardBackDataModel();
      this.m_rankedCardBackProgressWidget.BindDataModel(model);
    }
    if (model is CardBackDataModel cardBackDataModel)
      cardBackDataModel.CardBackId = RankMgr.Get().GetRankedCardBackIdForSeasonId(this.m_rewardProgress.Season);
    ProgressBar componentInChildren = this.m_rankedCardBackProgressWidget.GetComponentInChildren<ProgressBar>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    int seasonCardBackMinWins = RankMgr.Get().GetLocalPlayerMedalInfo().GetSeasonCardBackMinWins();
    componentInChildren.SetLabel(GameStrings.Format("GLOBAL_REWARD_PROGRESS", (object) 0, (object) seasonCardBackMinWins));
    componentInChildren.SetProgressBar(0.0f);
  }

  private void UpdateRankedMedalWidget()
  {
    if ((UnityEngine.Object) this.m_rankedMedal == (UnityEngine.Object) null)
      return;
    if (this.m_showMedal)
    {
      this.m_rankedMedal.gameObject.SetActive(true);
      RankedPlayDataModel dataModel;
      if (this.m_currentMode == SeasonEndDialog.MODE.REDUCED_WELCOME)
      {
        this.m_rankedMedal.transform.position = this.m_boostedMedalBone.transform.position;
        dataModel = this.m_currentRankedDataModel;
      }
      else
        dataModel = this.m_seasonBestRankedDataModel;
      this.m_rankedMedal.BindRankedPlayDataModel(dataModel);
      this.m_rankedMedal.Show(this.m_wasPrevSeasonLegacy);
    }
    else
      this.m_rankedMedal.gameObject.SetActive(false);
  }

  private IEnumerator FadeWidgetIn(Widget widget, float time)
  {
    while (!widget.IsReady || widget.IsChangingStates)
      yield return (object) null;
    iTween.FadeTo(widget.gameObject, 1f, time);
  }

  private IEnumerator FadeWidgetOut(Widget widget, float time)
  {
    while (!widget.IsReady || widget.IsChangingStates)
      yield return (object) null;
    iTween.FadeTo(widget.gameObject, 0.0f, time);
  }

  private void OnNetCacheRewardProgressUpdated()
  {
    this.m_rewardProgress = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
    this.UpdateRankedCardBackWidget();
  }

  private int GetChestRewardTier() => 1 + (RankMgr.Get().GetMaxRewardChestVisualIndex() - this.m_seasonBestMedalInfo.RankConfig.RewardChestVisualIndex);

  public string GetChestName() => GameStrings.Get(string.Format("GLOBAL_REWARD_CHEST_TIER{0}", (object) this.GetChestRewardTier()));

  public string GetChestEarnedText() => GameStrings.Get(string.Format("GLOBAL_REWARD_CHEST_TIER{0}_EARNED", (object) this.GetChestRewardTier()));

  protected override void DoShowAnimation()
  {
    this.m_showAnimState = DialogBase.ShowAnimState.IN_PROGRESS;
    AnimationUtil.ShowWithPunch(this.gameObject, this.START_SCALE, Vector3.Scale(this.PUNCH_SCALE, this.m_originalScale), this.m_originalScale, "OnShowAnimFinished", true);
  }

  public class SeasonEndInfo
  {
    public int m_seasonID;
    public int m_leagueId;
    public int m_starLevelAtEndOfSeason;
    public int m_bestStarLevelAtEndOfSeason;
    public int m_legendIndex;
    public List<RewardData> m_rankedRewards;
    public List<long> m_noticesToAck = new List<long>();
    public PegasusShared.FormatType m_formatType;
    public bool m_wasLimitedByBestEverStarLevel;
  }

  private enum MODE
  {
    RANK_EARNED,
    CHEST_EARNED,
    SEASON_WELCOME,
    REDUCED_WELCOME,
    REMINDER_CHEST,
    STAR_MULTIPLIER,
    REMINDER_CARDBACK,
  }
}
