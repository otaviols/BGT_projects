using Assets;
using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureBonusChallengeDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "Buttons")]
  public PlayButton m_playButton;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_backButton;
  [CustomEditField(Sections = "Buttons")]
  public PegUIElement m_rewardChest;
  [CustomEditField(Sections = "Text")]
  public UberText m_bonusChallengeLabel;
  [CustomEditField(Sections = "Text")]
  public UberText m_headerText;
  [CustomEditField(Sections = "Text")]
  public UberText m_footerText;
  [CustomEditField(Sections = "Rewards")]
  public GameObject m_rewardsPreview;
  [CustomEditField(Sections = "Rewards")]
  public GameObject m_rewardContainer;
  [CustomEditField(Sections = "Rewards")]
  public UberText m_rewardsText;
  [CustomEditField(Sections = "Rewards")]
  public Material m_chestOpenMaterial;
  [CustomEditField(Sections = "VO")]
  public float m_delayBeforeEntryVO;
  [CustomEditField(Sections = "VO")]
  public float m_delayBeforeCompleteVO;
  [CustomEditField(Sections = "Phone")]
  public PegUIElement m_rewardOffClickCatcher;
  private string m_headerString;
  private string m_footerString;
  private Vector3 m_rewardsScale;
  private GameObject m_rewardObject;
  private WingDbId m_wingId;

  private void Awake()
  {
    Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureBonusChallengeDisplay.OnNavigateBack));
    this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButton));
    this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButton));
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((Predicate<ScenarioDbfRecord>) (r => (AdventureDbId) r.AdventureId == AdventureConfig.Get().GetSelectedAdventure() && (AdventureModeDbId) r.ModeId == AdventureConfig.Get().GetSelectedMode()));
    if (record != null)
    {
      AdventureConfig.Get().SetMission((ScenarioDbId) record.ID);
      this.m_headerString = (string) record.Name;
      this.m_footerString = (string) (!(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty((string) record.ShortDescription) ? record.Description : record.ShortDescription);
      this.m_wingId = (WingDbId) record.WingId;
    }
    this.SetUpUberText();
    this.InitializeRewardDisplay();
    AdventureSubScene component = this.GetComponent<AdventureSubScene>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.AddSubSceneTransitionFinishedListener(new AdventureSubScene.SubSceneTransitionFinished(this.OnSubSceneTransitionComplete));
    component.SetIsLoaded(true);
  }

  private void SetUpUberText()
  {
    if ((UnityEngine.Object) this.m_bonusChallengeLabel != (UnityEngine.Object) null)
      this.m_bonusChallengeLabel.Text = GameStrings.Get("GLUE_ADVENTURE_BONUS_CHALLENGE_LABEL");
    if ((UnityEngine.Object) this.m_headerText != (UnityEngine.Object) null)
      this.m_headerText.Text = this.m_headerString;
    if (!((UnityEngine.Object) this.m_footerText != (UnityEngine.Object) null))
      return;
    this.m_footerText.Text = this.m_footerString;
  }

  private void OnPlayButton(UIEvent e) => GameMgr.Get().FindGame(GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, (int) AdventureConfig.Get().GetMission());

  private static bool OnNavigateBack()
  {
    AdventureConfig.Get().SubSceneGoBack();
    return true;
  }

  private void OnBackButton(UIEvent e) => Navigation.GoBack();

  private void OnSubSceneTransitionComplete()
  {
  }

  private void InitializeRewardDisplay()
  {
    int mission = (int) AdventureConfig.Get().GetMission();
    if (this.GetFirstRewardFromScenario(mission) == null)
      return;
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && AchieveManager.Get().GetNewCompletedAchievesToShow().Count > 0)
    {
      this.m_playButton.SetEnabled(false);
      LoadingScreen.Get().RegisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
    }
    if (AdventureProgressMgr.Get().HasDefeatedScenario(mission))
    {
      this.m_rewardChest.GetComponent<Renderer>().SetMaterial(this.m_chestOpenMaterial);
      this.m_rewardChest.SetEnabled(false);
    }
    else
    {
      this.StartCoroutine(this.PlayEntryQuoteWithTiming());
      AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) AdventureConfig.Get().GetSelectedAdventure(), (int) AdventureConfig.Get().GetSelectedMode());
      if (adventureDataRecord != null)
        this.m_rewardsText.Text = (string) adventureDataRecord.RewardsDescription;
      if ((UnityEngine.Object) this.m_rewardOffClickCatcher != (UnityEngine.Object) null)
      {
        this.m_rewardChest.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ShowNonSessionRewardPreview));
        this.m_rewardOffClickCatcher.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.HideNonSessionRewardPreview));
      }
      else
      {
        this.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowNonSessionRewardPreview));
        this.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideNonSessionRewardPreview));
      }
      this.m_rewardsScale = this.m_rewardsPreview.transform.localScale;
      this.m_rewardsPreview.transform.localScale = Vector3.one * 0.01f;
    }
  }

  private void OnTransitionFromGameplayFinished(bool cutoff, object userData)
  {
    PopupDisplayManager.Get().ShowAnyOutstandingPopups((Action) (() => Navigation.GoBack()));
    int mission = (int) AdventureConfig.Get().GetMission();
    if (AdventureProgressMgr.Get().HasDefeatedScenario(mission))
      this.StartCoroutine(this.PlayCompleteQuoteWithTiming());
    LoadingScreen.Get().UnregisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
  }

  private RewardData GetFirstRewardFromScenario(int scenarioDbId)
  {
    HashSet<Achieve.RewardTiming> rewardTimings = new HashSet<Achieve.RewardTiming>()
    {
      Achieve.RewardTiming.ADVENTURE_CHEST
    };
    List<RewardData> defeatingScenario = AdventureProgressMgr.Get().GetRewardsForDefeatingScenario((int) AdventureConfig.Get().GetMission(), rewardTimings);
    return defeatingScenario == null || defeatingScenario.Count == 0 ? (RewardData) null : defeatingScenario[0];
  }

  private void ShowNonSessionRewardPreview(UIEvent e)
  {
    if (AdventureConfig.Get().GetMission() == ScenarioDbId.INVALID)
      return;
    RewardData rewardFromScenario = this.GetFirstRewardFromScenario((int) AdventureConfig.Get().GetMission());
    if (rewardFromScenario == null)
      return;
    if (rewardFromScenario.RewardType == Reward.Type.CARD_BACK)
    {
      if ((UnityEngine.Object) this.m_rewardObject == (UnityEngine.Object) null)
      {
        int cardBackId = (rewardFromScenario as CardBackRewardData).CardBackID;
        CardBackManager.LoadCardBackData loadCardBackData = CardBackManager.Get().LoadCardBackByIndex(cardBackId, shadowActive: true);
        if (loadCardBackData == null)
        {
          Debug.LogErrorFormat("AdventureBonusChallengeDisplay.ShowReward() - Could not load cardback ID {0}!", (object) cardBackId);
          return;
        }
        this.m_rewardObject = loadCardBackData.m_GameObject;
        GameUtils.SetParent(this.m_rewardObject, this.m_rewardContainer);
      }
      this.m_rewardsPreview.SetActive(true);
      iTween.Stop(this.m_rewardsPreview);
      iTween.ScaleTo(this.m_rewardsPreview, iTween.Hash((object) "scale", (object) this.m_rewardsScale, (object) "time", (object) 0.15f));
    }
    else
      Debug.LogErrorFormat("Adventure Bonus Challenge reward type currently not supported! Add type {0} to AdventureBonusChallengeDisplay.ShowReward().", (object) rewardFromScenario.RewardType);
  }

  private void HideNonSessionRewardPreview(UIEvent e)
  {
    iTween.Stop(this.m_rewardsPreview);
    iTween.ScaleTo(this.m_rewardsPreview, iTween.Hash((object) "scale", (object) (Vector3.one * 0.01f), (object) "time", (object) 0.15f, (object) "oncomplete", (object) (Action<object>) (o => this.m_rewardsPreview.SetActive(false))));
  }

  private IEnumerator PlayEntryQuoteWithTiming()
  {
    yield return (object) new WaitForSeconds(this.m_delayBeforeEntryVO);
    AdventureWingDef wingDef = AdventureScene.Get().GetWingDef(this.m_wingId);
    if (AdventureUtils.CanPlayWingOpenQuote(wingDef))
    {
      string legacyAssetName = new AssetReference(wingDef.m_OpenQuoteVOLine).GetLegacyAssetName();
      NotificationManager.Get().CreateCharacterQuote(wingDef.m_OpenQuotePrefab, NotificationManager.CHARACTER_POS_ABOVE_QUEST_TOAST, GameStrings.Get(legacyAssetName), wingDef.m_OpenQuoteVOLine, false);
    }
  }

  private IEnumerator PlayCompleteQuoteWithTiming()
  {
    yield return (object) new WaitForSeconds(this.m_delayBeforeCompleteVO);
    AdventureWingDef wingDef = AdventureScene.Get().GetWingDef(this.m_wingId);
    if (AdventureUtils.CanPlayWingCompleteQuote(wingDef))
    {
      string legacyAssetName = new AssetReference(wingDef.m_CompleteQuoteVOLine).GetLegacyAssetName();
      NotificationManager.Get().CreateCharacterQuote(wingDef.m_CompleteQuotePrefab, NotificationManager.CHARACTER_POS_ABOVE_QUEST_TOAST, GameStrings.Get(legacyAssetName), wingDef.m_CompleteQuoteVOLine, false);
    }
  }
}
