using Assets;
using Blizzard.T5.Configuration;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class QuestLog : UIBPopup
{
  public const int QUEST_LOG_MAX_COUNT = 3;
  public GameObject m_root;
  public UberText m_winsCountText;
  public UberText m_forgeRecordCountText;
  public UberText m_totalLevelsText;
  public Transform m_arenaMedalBone;
  public ArenaMedal m_arenaMedalPrefab;
  public PegUIElement m_offClickCatcher;
  public List<ClassProgressBar> m_classProgressBars;
  public List<ClassProgressInfo> m_classProgressInfos;
  public AsyncReference m_rankedMedalWidgetReference;
  public AsyncReference m_rankedRewardInfoButtonWidgetReference;
  public GameObject m_questTilePrefab;
  public List<Transform> m_questBones;
  public UberText m_noQuestText;
  public UIBButton m_closeButton;
  [CustomEditField(Sections = "Aspect Ratio Positioning")]
  public float m_extraWideScale = 150f;
  private List<QuestTile> m_currentQuests;
  private static QuestLog s_instance;
  private int m_justCanceledQuestID;
  private Widget m_rankedMedalWidget;
  private RankedMedal m_rankedMedal;
  private Widget m_rankedRewardInfoButtonWidget;
  private RankedRewardInfoButton m_rankedRewardInfoButton;
  private ArenaMedal m_arenaMedal;
  private Enum[] m_presencePrevStatus;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void Awake()
  {
    base.Awake();
    QuestLog.s_instance = this;
    AchieveManager.Get().RegisterAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
    if ((UnityEngine.Object) this.m_closeButton != (UnityEngine.Object) null)
      this.m_closeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCloseButtonReleased));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected override void Start()
  {
    base.Start();
    this.m_rankedMedalWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRankedMedalWidgetReady));
    this.m_rankedRewardInfoButtonWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRankedRewardInfoButtonWidgetReady));
    for (int index = 0; index < this.m_classProgressInfos.Count; ++index)
    {
      ClassProgressInfo classProgressInfo = this.m_classProgressInfos[index];
      TAG_CLASS tagClass = classProgressInfo.m_class;
      ClassProgressBar frame = classProgressInfo.m_frame;
      LayerUtils.SetLayer((Component) frame, classProgressInfo.m_frame.gameObject.layer);
      frame.m_class = tagClass;
      this.m_classProgressBars.Add(frame);
    }
    this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnQuestLogCloseEvent));
    BnetBar bnetBar = BnetBar.Get();
    if (!((UnityEngine.Object) bnetBar != (UnityEngine.Object) null))
      return;
    bnetBar.OnMenuOpened += new Action(this.OnMenuOpened);
  }

  private void OnDestroy()
  {
    if (ShownUIMgr.Get() != null)
      ShownUIMgr.Get().ClearShownUI();
    if (AchieveManager.Get() != null)
    {
      AchieveManager.Get().RemoveAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
      AchieveManager.Get().RemoveQuestCanceledListener(new AchieveManager.AchieveCanceledCallback(this.OnQuestCanceled));
    }
    this.m_screenEffectsHandle.StopEffect();
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.Hide(false);
    BnetBar bnetBar = BnetBar.Get();
    if ((UnityEngine.Object) bnetBar != (UnityEngine.Object) null)
      bnetBar.OnMenuOpened -= new Action(this.OnMenuOpened);
    QuestLog.s_instance = (QuestLog) null;
  }

  public static QuestLog Get() => QuestLog.s_instance;

  public void StartHidden() => this.DoHideAnimation(true);

  public void SetCloseButtonActive(bool active)
  {
    if (!((UnityEngine.Object) this.m_closeButton != (UnityEngine.Object) null))
      return;
    this.m_closeButton.gameObject.SetActive(active);
  }

  public override void Show()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null)
    {
      Debug.Log((object) "QuestLog: Attempting to Show after the QuestLog component has already been destroyed.");
    }
    else
    {
      this.m_presencePrevStatus = PresenceMgr.Get().GetStatus();
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.QUESTLOG);
      AchieveManager.Get().RegisterQuestCanceledListener(new AchieveManager.AchieveCanceledCallback(this.OnQuestCanceled));
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 0.1f
      });
      Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
      if ((bool) UniversalInputManager.UsePhoneUI && (UnityEngine.Object) this.m_root != (UnityEngine.Object) null)
      {
        float num = 1f;
        this.m_scaleMode = CanvasScaleMode.WIDTH;
        if (TransformUtil.IsExtraWideAspectRatio())
        {
          num = this.m_extraWideScale;
          this.m_scaleMode = CanvasScaleMode.HEIGHT;
        }
        OverlayUI.Get().AddGameObject(this.gameObject, scaleMode: this.m_scaleMode);
        this.m_root.transform.localScale = Vector3.one * num;
      }
      this.StartCoroutine(this.ShowWhenReady());
    }
  }

  private IEnumerator ShowWhenReady()
  {
    while ((UnityEngine.Object) this.m_rankedMedalWidget == (UnityEngine.Object) null || (UnityEngine.Object) this.m_rankedRewardInfoButtonWidget == (UnityEngine.Object) null)
      yield return (object) null;
    this.UpdateData();
    while (this.m_rankedMedalWidget.IsChangingStates || this.m_rankedRewardInfoButtonWidget.IsChangingStates)
      yield return (object) null;
    base.Show();
  }

  protected override void Hide(bool animate)
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null)
    {
      Debug.Log((object) "QuestLog: Attempting to Hide after the QuestLog component has already been destroyed.");
    }
    else
    {
      if (this.m_presencePrevStatus == null)
        this.m_presencePrevStatus = new Enum[1]
        {
          (Enum) Global.PresenceStatus.HUB
        };
      PresenceMgr.Get().SetStatus(this.m_presencePrevStatus);
      if (ShownUIMgr.Get() != null)
        ShownUIMgr.Get().ClearShownUI();
      foreach (QuestTile currentQuest in this.m_currentQuests)
      {
        if ((UnityEngine.Object) currentQuest != (UnityEngine.Object) null)
          currentQuest.OnClose();
      }
      this.DoHideAnimation(!animate, (UIBPopup.OnAnimationComplete) (() =>
      {
        if (AchieveManager.Get() != null)
          AchieveManager.Get().RemoveQuestCanceledListener(new AchieveManager.AchieveCanceledCallback(this.OnQuestCanceled));
        this.DeleteQuests();
        if (FullScreenFXMgr.Get() != null)
          this.m_screenEffectsHandle.StopEffect();
        this.m_shown = false;
      }));
    }
  }

  private void DeleteQuests()
  {
    if (this.m_currentQuests == null || this.m_currentQuests.Count == 0)
      return;
    foreach (QuestTile currentQuest in this.m_currentQuests)
    {
      if ((UnityEngine.Object) currentQuest != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) currentQuest.gameObject);
    }
  }

  private void OnQuestLogCloseEvent(UIEvent e) => Navigation.GoBack();

  private bool OnNavigateBack()
  {
    this.Hide(true);
    return true;
  }

  private void UpdateData()
  {
    this.UpdateClassProgress();
    this.UpdateActiveQuests();
    this.UpdateRankedMedal();
    this.UpdateRankedRewardInfo();
    this.UpdateBestArenaMedal();
    this.UpdateTotalWins();
  }

  private void UpdateTotalWins()
  {
    int num1 = 0;
    int num2 = 0;
    foreach (NetCache.PlayerRecord record in NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>().Records)
    {
      if (record.Data == 0)
      {
        switch (record.RecordType)
        {
          case GameType.GT_ARENA:
            num2 += record.Wins;
            continue;
          case GameType.GT_RANKED:
          case GameType.GT_CASUAL:
          case GameType.GT_TAVERNBRAWL:
            num1 += record.Wins;
            continue;
          default:
            continue;
        }
      }
    }
    this.m_winsCountText.Text = num1.ToString();
    this.m_forgeRecordCountText.Text = num2.ToString();
  }

  private void UpdateBestArenaMedal()
  {
    NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
    if ((UnityEngine.Object) this.m_arenaMedal == (UnityEngine.Object) null)
    {
      this.m_arenaMedal = (ArenaMedal) GameUtils.Instantiate((Component) this.m_arenaMedalPrefab, this.m_arenaMedalBone.gameObject, true);
      LayerUtils.SetLayer((Component) this.m_arenaMedal, this.m_arenaMedalBone.gameObject.layer);
      this.m_arenaMedal.transform.localScale = Vector3.one;
    }
    if (netObject.LastForgeDate != 0L)
    {
      this.m_arenaMedal.gameObject.SetActive(true);
      this.m_arenaMedal.SetMedal(netObject.BestForgeWins);
    }
    else
      this.m_arenaMedal.gameObject.SetActive(false);
  }

  private void OnRankedMedalWidgetReady(Widget widget)
  {
    this.m_rankedMedalWidget = widget;
    this.m_rankedMedal = this.m_rankedMedalWidget.GetComponentInChildren<RankedMedal>();
  }

  private void OnRankedRewardInfoButtonWidgetReady(Widget widget)
  {
    this.m_rankedRewardInfoButtonWidget = widget;
    this.m_rankedRewardInfoButtonWidget.Hide();
    this.m_rankedRewardInfoButton = this.m_rankedRewardInfoButtonWidget.GetComponentInChildren<RankedRewardInfoButton>();
  }

  private void UpdateRankedMedal()
  {
    if ((UnityEngine.Object) this.m_rankedMedalWidget == (UnityEngine.Object) null || (UnityEngine.Object) this.m_rankedMedal == (UnityEngine.Object) null)
      return;
    MedalInfoTranslator localPlayerMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo();
    this.m_rankedMedal.BindRankedPlayDataModel(localPlayerMedalInfo.CreateDataModel(localPlayerMedalInfo.GetBestCurrentRankFormatType(), RankedMedal.DisplayMode.Default, true));
  }

  private void UpdateRankedRewardInfo()
  {
    MedalInfoTranslator localPlayerMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo();
    if (!((UnityEngine.Object) this.m_rankedRewardInfoButton != (UnityEngine.Object) null))
      return;
    this.m_rankedRewardInfoButton.Initialize(localPlayerMedalInfo);
    this.m_rankedRewardInfoButton.Show();
  }

  private void UpdateClassProgress()
  {
    if (this.m_classProgressBars.Count == 0)
      return;
    int num = 0;
    List<Achievement> achievesInGroup = AchieveManager.Get().GetAchievesInGroup(Achieve.Type.GOLDHERO, true);
    NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
    foreach (ClassProgressBar classProgressBar in this.m_classProgressBars)
    {
      ClassProgressBar classProgress = classProgressBar;
      NetCache.HeroLevel heroLevel = netObject.Levels.Find((Predicate<NetCache.HeroLevel>) (obj => obj.Class == classProgress.m_class));
      Achievement achievement = achievesInGroup.Find((Predicate<Achievement>) (obj => obj.MyHeroClassRequirement.HasValue && obj.MyHeroClassRequirement.Value == classProgress.m_class));
      classProgress.SetPremium(achievement != null);
      if (heroLevel != null)
      {
        classProgress.m_classLockedGO.SetActive(false);
        classProgress.m_levelText.Text = heroLevel.CurrentLevel.Level.ToString();
        int nextRewardLevel = 0;
        RewardData nextHeroLevelReward = FixedRewardsMgr.Get().GetNextHeroLevelReward(heroLevel.Class, heroLevel.CurrentLevel.Level, out nextRewardLevel);
        if (nextHeroLevelReward != null)
          classProgress.SetTooltipText(GameStrings.Format("GLOBAL_HERO_LEVEL_NEXT_REWARD_TITLE", (object) nextRewardLevel), RewardUtils.GetRewardText(nextHeroLevelReward), heroLevel.CurrentLevel.Level.ToString());
        num += heroLevel.CurrentLevel.Level;
        if (heroLevel.CurrentLevel.IsMaxLevel())
          classProgress.m_progressBar.SetProgressBar(1f);
        else
          classProgress.m_progressBar.SetProgressBar((float) heroLevel.CurrentLevel.XP / (float) heroLevel.CurrentLevel.MaxXP);
      }
      else
      {
        classProgress.m_levelText.Text = "0";
        classProgress.Lock();
      }
    }
    if (!((UnityEngine.Object) this.m_totalLevelsText != (UnityEngine.Object) null))
      return;
    this.m_totalLevelsText.Text = string.Format(GameStrings.Get("GLUE_QUEST_LOG_TOTAL_LEVELS"), (object) num);
  }

  private void UpdateActiveQuests()
  {
    List<Achievement> activeQuests = AchieveManager.Get().GetActiveQuests();
    this.m_currentQuests = new List<QuestTile>();
    for (int index = 0; index < activeQuests.Count; ++index)
    {
      if (index < 3)
        this.AddCurrentQuestTile(activeQuests[index], index);
    }
    if (this.m_currentQuests.Count == 0)
    {
      this.m_noQuestText.gameObject.SetActive(true);
      if (AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.DAILY))
      {
        this.m_noQuestText.Text = GameStrings.Get("GLUE_QUEST_LOG_NO_QUESTS_DAILIES_UNLOCKED");
        if (Options.Get().GetBool(Option.HAS_RUN_OUT_OF_QUESTS, false) || !UserAttentionManager.CanShowAttentionGrabber("QuestLog.UpdateActiveQuests:" + (object) Option.HAS_RUN_OUT_OF_QUESTS))
          return;
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, 0.0f, 34.5f), GameStrings.Get("VO_INNKEEPER_OUT_OF_QUESTS"), "VO_INNKEEPER_OUT_OF_QUESTS.prefab:b0073c56bf38c664dab532ad92f3baf9");
        Options.Get().SetBool(Option.HAS_RUN_OUT_OF_QUESTS, true);
      }
      else
        this.m_noQuestText.Text = GameStrings.Get("GLUE_QUEST_LOG_NO_QUESTS");
    }
    else
      this.m_noQuestText.gameObject.SetActive(false);
  }

  private void AddCurrentQuestTile(Achievement achieveQuest, int slot)
  {
    if ((UnityEngine.Object) this.m_questTilePrefab == (UnityEngine.Object) null || this.m_questBones == null || (UnityEngine.Object) this.m_questBones[slot] == (UnityEngine.Object) null || this.m_currentQuests == null)
    {
      Debug.Log((object) "QuestLog: AddCurrentQuestTile failed, because a required object is null.");
    }
    else
    {
      GameObject go = (GameObject) GameUtils.Instantiate(this.m_questTilePrefab, this.m_questBones[slot].gameObject, true);
      LayerUtils.SetLayer(go, this.m_questBones[slot].gameObject.layer);
      go.transform.localScale = Vector3.one;
      QuestTile component = go.GetComponent<QuestTile>();
      component.SetupTile(achieveQuest, QuestTile.FsmEvent.QuestShownInQuestLog);
      component.SetCanShowCancelButton(true);
      this.m_currentQuests.Add(component);
    }
  }

  private void OnQuestCanceled(int achieveID, bool canceled, object userData)
  {
    if (!canceled)
      return;
    this.m_justCanceledQuestID = achieveID;
  }

  private void OnAchievesUpdated(
    List<Achievement> updatedAchieves,
    List<Achievement> completedAchieves,
    object userData)
  {
    if (this.m_justCanceledQuestID == 0)
      return;
    List<Achievement> activeQuests = AchieveManager.Get().GetActiveQuests(true);
    if (activeQuests.Count <= 0)
      return;
    if (activeQuests.Count > 1 && !Vars.Key("Quests.CanCancelManyTimes").GetBool(false) && !Vars.Key("Quests.CancelGivesManyNewQuests").GetBool(false))
    {
      Debug.LogError((object) string.Format("QuestLog.OnActiveAchievesUpdated(): expecting ONE new active quest after a quest cancel but received {0}", (object) activeQuests.Count));
      this.Hide();
    }
    else
    {
      int justCanceledQuest = this.m_justCanceledQuestID;
      this.m_justCanceledQuestID = 0;
      QuestTile questTile = this.m_currentQuests.Find((Predicate<QuestTile>) (obj => obj.GetQuestID() == justCanceledQuest));
      if ((UnityEngine.Object) questTile == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("QuestLog.OnActiveAchievesUpdated(): could not find tile for just canceled quest (quest ID {0})", (object) justCanceledQuest));
        this.Hide();
      }
      else
      {
        Log.Achievements.Print("Adding QuestLog tile for: {0}", (object) activeQuests[0]);
        questTile.SetupTile(activeQuests[0], QuestTile.FsmEvent.QuestRerolled);
        for (int index = 1; index < activeQuests.Count; ++index)
        {
          int count = this.m_currentQuests.Count;
          if (count < this.m_questBones.Count)
            this.AddCurrentQuestTile(activeQuests[index], count);
          else
            break;
        }
        foreach (QuestTile currentQuest in this.m_currentQuests)
          currentQuest.UpdateCancelButtonVisibility();
      }
    }
  }

  private void OnCloseButtonReleased(UIEvent e) => this.OnNavigateBack();

  private void OnMenuOpened()
  {
    if (!this.m_shown)
      return;
    this.Hide(false);
  }
}
