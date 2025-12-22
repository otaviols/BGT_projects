using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class RankChangeTwoScoop_NEW : MonoBehaviour
{
  [CustomEditField(Sections = "Playmaker Test Data")]
  public League.LeagueType m_testLeagueType = League.LeagueType.NORMAL;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public int m_testStarLevel = 1;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public int m_testStars;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public int m_testStarMultiplier = 1;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public bool m_testWinStreak;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public bool m_testLegend;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public bool m_testWild;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public bool m_testCanLoseStars;
  [CustomEditField(Sections = "Playmaker Test Data")]
  public bool m_testCanLoseLevel;
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_startScale;
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_punchScale;
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_afterPunchScale;
  [CustomEditField(Sections = "Banner")]
  public Float_MobileOverride m_bannerTextWidthMult;
  [CustomEditField(Sections = "Click Blocker")]
  public float m_maxAnimTimeBeforeClickToContinue = 3f;
  public AsyncReference m_prevRankedMedalWidgetReference;
  public AsyncReference m_currRankedMedalWidgetReference;
  public AsyncReference m_currRankedMedalLegendTextReference;
  public RankedStarArray m_prevMedalStars;
  public RankedStarArray m_currMedalStars;
  public RankedStarArray m_newlyEarnedStars;
  public RankedStarArray m_newlyEarnedStarsForRankUpRow1;
  public RankedStarArray m_newlyEarnedStarsForRankUpRow2;
  public GameObject m_medalGodRays;
  public GameObject m_banner;
  public UberText m_prevBannerText;
  public UberText m_currBannerText;
  public PlayMakerFSM m_mainFSM;
  public PlayMakerFSM m_starLossFSM;
  public PlayMakerFSM m_starGainSingleFSM;
  public PlayMakerFSM m_starGainMultiFSM;
  public PlayMakerFSM m_rankUpFSM;
  public PlayMakerFSM m_rankDownFSM;
  public UberText m_winStreakText;
  public UberText m_starMultiplierText;
  public UberText m_cannotLoseStarText;
  public UberText m_cannotLoseLevelText;
  public PegUIElement m_debugClickCatcher;
  private MedalInfoTranslator m_medalInfoTranslator;
  private PegasusShared.FormatType m_formatType = PegasusShared.FormatType.FT_STANDARD;
  private TranslatedMedalInfo m_currMedalInfo;
  private TranslatedMedalInfo m_prevMedalInfo;
  private RankedPlayDataModel m_currMedalDataModel;
  private RankedPlayDataModel m_prevMedalDataModel;
  private RankedMedal m_currRankedMedal;
  private RankedMedal m_prevRankedMedal;
  private Widget m_currRankedMedalWidget;
  private Widget m_prevRankedMedalWidget;
  private UberText m_currRankedMedalLegendText;
  private bool m_isRankChanging;
  private bool m_isOnWinStreak;
  private Action m_closedCallback;
  private bool m_isRankChangeCheat;
  private Coroutine m_clickToContinueCoroutine;
  private bool m_isPlayingAnimWithCancelPoint;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    this.m_prevBannerText.Width *= (float) (MobileOverrideValue<float>) this.m_bannerTextWidthMult;
    this.m_currBannerText.Width *= (float) (MobileOverrideValue<float>) this.m_bannerTextWidthMult;
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    this.Reset();
  }

  private void Start()
  {
    this.m_prevRankedMedalWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnPrevRankedMedalWidgetReady));
    this.m_currRankedMedalWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnCurrRankedMedalWidgetReady));
    this.m_currRankedMedalLegendTextReference.RegisterReadyListener<UberText>(new Action<UberText>(this.OnCurrRankedMedalLegendTextReady));
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null))
      return;
    EndGameScreen.Get().m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
  }

  public void Initialize(
    MedalInfoTranslator medalInfoTranslator,
    PegasusShared.FormatType formatType,
    Action callback)
  {
    if (medalInfoTranslator == null)
      return;
    this.m_closedCallback = callback;
    this.m_medalInfoTranslator = medalInfoTranslator;
    this.m_formatType = formatType;
    this.m_currMedalInfo = this.m_medalInfoTranslator.GetCurrentMedal(this.m_formatType);
    this.m_prevMedalInfo = this.m_medalInfoTranslator.GetPreviousMedal(this.m_formatType);
    RankChangeType changeType = this.m_medalInfoTranslator.GetChangeType(this.m_formatType);
    this.m_isRankChanging = changeType == RankChangeType.RANK_UP || changeType == RankChangeType.RANK_DOWN;
    this.m_isOnWinStreak = this.m_prevMedalInfo.RankConfig.WinStreakThreshold > 0 && this.m_currMedalInfo.winStreak >= this.m_prevMedalInfo.RankConfig.WinStreakThreshold;
  }

  private void InitializeFromDataModels()
  {
    int maxStars1 = this.m_prevMedalDataModel.MaxStars;
    int starCountDarkened1 = maxStars1 - this.m_prevMedalDataModel.Stars;
    this.m_prevMedalStars.Init(maxStars1, starCountDarkened1);
    if (this.m_isRankChanging)
    {
      int maxStars2 = this.m_currMedalDataModel.MaxStars;
      int starCountDarkened2 = maxStars2 - this.m_currMedalDataModel.Stars;
      this.m_currMedalStars.Init(maxStars2, starCountDarkened2);
    }
    if (this.m_isRankChanging)
    {
      if (this.m_prevMedalDataModel.StarMultiplier == 1 && this.m_isOnWinStreak)
      {
        this.m_newlyEarnedStarsForRankUpRow1.Init(2, 0);
      }
      else
      {
        this.m_newlyEarnedStarsForRankUpRow1.Init(this.m_prevMedalDataModel.StarMultiplier, 0);
        if (this.m_isOnWinStreak)
          this.m_newlyEarnedStarsForRankUpRow2.Init(this.m_prevMedalDataModel.StarMultiplier, 0);
      }
    }
    else
    {
      int starCount = this.m_currMedalDataModel.Stars - this.m_prevMedalDataModel.Stars;
      if (starCount > 1)
        this.m_newlyEarnedStars.Init(starCount, 0);
    }
    if (this.m_prevMedalDataModel.StarMultiplier > 1)
      this.m_starMultiplierText.Text = GameStrings.Format("GLOBAL_RANK_STAR_MULT", (object) this.m_prevMedalDataModel.StarMultiplier);
    this.m_prevBannerText.Text = this.m_prevMedalDataModel.RankName;
    this.m_currBannerText.Text = this.m_currMedalDataModel.RankName;
  }

  [ContextMenu("Reset")]
  private void Reset()
  {
    this.m_banner.SetActive(false);
    this.m_winStreakText.gameObject.SetActive(false);
    this.m_starMultiplierText.gameObject.SetActive(false);
    this.m_cannotLoseStarText.gameObject.SetActive(false);
    this.m_cannotLoseLevelText.gameObject.SetActive(false);
    this.m_medalGodRays.SetActive(false);
    this.m_debugClickCatcher.gameObject.SetActive(false);
    this.m_prevMedalStars.Hide();
    this.m_currMedalStars.Hide();
    this.m_newlyEarnedStars.Hide();
    this.m_newlyEarnedStarsForRankUpRow1.Hide();
    this.m_newlyEarnedStarsForRankUpRow2.Hide();
    if ((UnityEngine.Object) this.m_currRankedMedalWidget != (UnityEngine.Object) null)
      this.m_currRankedMedalWidget.Hide();
    if ((UnityEngine.Object) this.m_prevRankedMedalWidget != (UnityEngine.Object) null)
      this.m_prevRankedMedalWidget.Hide();
    if ((UnityEngine.Object) this.m_currRankedMedalLegendText != (UnityEngine.Object) null)
      this.m_currRankedMedalLegendText.Hide();
    this.m_mainFSM.SendEvent(nameof (Reset));
    this.m_starLossFSM.SendEvent(nameof (Reset));
    this.m_starGainSingleFSM.SendEvent(nameof (Reset));
    this.m_starGainMultiFSM.SendEvent(nameof (Reset));
    this.m_rankUpFSM.SendEvent(nameof (Reset));
    this.m_rankDownFSM.SendEvent(nameof (Reset));
    this.m_isPlayingAnimWithCancelPoint = false;
  }

  public void Show() => this.StartCoroutine(this.ShowWhenReady((Action<object>) (_ =>
  {
    AnimationUtil.ShowWithPunch(this.gameObject, (Vector3) (MobileOverrideValue<Vector3>) this.m_startScale, (Vector3) (MobileOverrideValue<Vector3>) this.m_punchScale, (Vector3) (MobileOverrideValue<Vector3>) this.m_afterPunchScale, "OnShown", true);
    this.m_mainFSM.SendEvent("Birth");
  })));

  private bool IsReady => this.m_medalInfoTranslator != null && !((UnityEngine.Object) this.m_prevRankedMedal == (UnityEngine.Object) null) && !((UnityEngine.Object) this.m_currRankedMedal == (UnityEngine.Object) null) && !this.m_currRankedMedalWidget.IsChangingStates && (!this.m_isRankChanging || !this.m_prevRankedMedalWidget.IsChangingStates) && !this.m_newlyEarnedStars.IsLoading() && !this.m_newlyEarnedStarsForRankUpRow1.IsLoading() && !this.m_newlyEarnedStarsForRankUpRow2.IsLoading();

  private void OnPrevRankedMedalWidgetReady(Widget widget)
  {
    this.m_prevRankedMedal = widget.GetComponentInChildren<RankedMedal>();
    this.m_prevRankedMedalWidget = widget;
    this.m_prevRankedMedalWidget.Hide();
  }

  private void OnCurrRankedMedalWidgetReady(Widget widget)
  {
    this.m_currRankedMedal = widget.GetComponentInChildren<RankedMedal>();
    this.m_currRankedMedalWidget = widget;
    this.m_currRankedMedalWidget.Hide();
  }

  private void OnCurrRankedMedalLegendTextReady(UberText text) => this.m_currRankedMedalLegendText = text;

  private IEnumerator ShowWhenReady(Action<object> showFunc)
  {
    RankChangeTwoScoop_NEW changeTwoScoopNew = this;
    while ((UnityEngine.Object) changeTwoScoopNew.m_prevRankedMedalWidget == (UnityEngine.Object) null || (UnityEngine.Object) changeTwoScoopNew.m_currRankedMedalWidget == (UnityEngine.Object) null)
      yield return (object) null;
    changeTwoScoopNew.m_prevMedalDataModel = changeTwoScoopNew.m_prevMedalInfo.CreateDataModel(RankedMedal.DisplayMode.Default);
    changeTwoScoopNew.m_prevRankedMedalWidget.BindDataModel((IDataModel) changeTwoScoopNew.m_prevMedalDataModel);
    changeTwoScoopNew.m_currMedalDataModel = changeTwoScoopNew.m_currMedalInfo.CreateDataModel(RankedMedal.DisplayMode.Default);
    changeTwoScoopNew.m_currRankedMedalWidget.BindDataModel((IDataModel) changeTwoScoopNew.m_currMedalDataModel);
    changeTwoScoopNew.InitializeFromDataModels();
    while (!changeTwoScoopNew.IsReady)
      yield return (object) null;
    changeTwoScoopNew.m_banner.SetActive(true);
    changeTwoScoopNew.m_prevBannerText.gameObject.SetActive(true);
    changeTwoScoopNew.m_currBannerText.gameObject.SetActive(false);
    changeTwoScoopNew.m_medalGodRays.SetActive(true);
    changeTwoScoopNew.m_prevMedalStars.Show();
    if (changeTwoScoopNew.m_isRankChanging)
    {
      changeTwoScoopNew.m_prevRankedMedalWidget.Show();
    }
    else
    {
      changeTwoScoopNew.m_currRankedMedalWidget.Show();
      changeTwoScoopNew.m_currRankedMedalLegendText.Hide();
    }
    showFunc((object) changeTwoScoopNew);
  }

  private void OnShown()
  {
    this.m_clickToContinueCoroutine = this.StartCoroutine(this.EnableClickToContinueAfterDelay(this.m_maxAnimTimeBeforeClickToContinue));
    switch (this.m_medalInfoTranslator.GetChangeType(this.m_formatType))
    {
      case RankChangeType.NO_GAME_PLAYED:
        this.HandleMissingRankChange();
        break;
      case RankChangeType.RANK_UP:
        this.PlayRankUp();
        break;
      case RankChangeType.RANK_DOWN:
        this.PlayRankDown();
        break;
      case RankChangeType.RANK_SAME:
        this.PlayStarChange(this.m_prevMedalInfo.CanLoseStars(), this.m_prevMedalInfo.CanLoseLevel());
        break;
      default:
        this.EnableClickToContinue();
        break;
    }
  }

  private IEnumerator EnableClickToContinueAfterDelay(float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    this.EnableClickToContinue();
  }

  private void EnableClickToContinue()
  {
    if ((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null)
      EndGameScreen.Get().m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
    if (!this.m_isRankChangeCheat)
      return;
    this.m_debugClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
  }

  private void OnPlayMakerCancelPointPassed() => this.m_isPlayingAnimWithCancelPoint = false;

  private void OnPlayMakerFinished()
  {
    this.m_isPlayingAnimWithCancelPoint = false;
    this.EnableClickToContinue();
    if (this.m_medalInfoTranslator.GetChangeType(this.m_formatType) != RankChangeType.RANK_UP || !((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null))
      return;
    Gameplay.Get().UpdateFriendlySideMedalChange(this.m_medalInfoTranslator);
  }

  private void OnClick(UIEvent e)
  {
    if (this.m_isPlayingAnimWithCancelPoint)
    {
      this.m_isPlayingAnimWithCancelPoint = false;
      switch (this.m_medalInfoTranslator.GetChangeType(this.m_formatType))
      {
        case RankChangeType.RANK_UP:
          this.m_rankUpFSM.SendEvent("Cancel");
          break;
        case RankChangeType.RANK_DOWN:
          this.m_rankDownFSM.SendEvent("Cancel");
          break;
      }
    }
    else
      this.Hide();
  }

  private void Hide()
  {
    this.m_mainFSM.SendEvent("Death");
    if (this.m_clickToContinueCoroutine != null)
      this.StopCoroutine(this.m_clickToContinueCoroutine);
    if ((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null)
      EndGameScreen.Get().m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
    if ((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null)
      AnimationUtil.ScaleFade(this.gameObject, new Vector3(0.1f, 0.1f, 0.1f), "DestroyRankChange");
    if (!this.m_isRankChangeCheat)
      return;
    this.m_screenEffectsHandle.StopEffect();
  }

  private void DestroyRankChange()
  {
    if (this.m_closedCallback != null)
      this.m_closedCallback();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void PlayRankUp()
  {
    this.PopuplateBasicFsmVars(this.m_rankUpFSM);
    this.m_currMedalStars.PopulateFsmArrayWithStars(this.m_rankUpFSM, "NewlyEarnedMedalStars", count: this.m_currMedalDataModel.Stars);
    this.m_newlyEarnedStarsForRankUpRow1.PopulateFsmArrayWithStars(this.m_rankUpFSM, "NewlyEarnedStarsRow1");
    if (this.m_isOnWinStreak)
      this.m_newlyEarnedStarsForRankUpRow2.PopulateFsmArrayWithStars(this.m_rankUpFSM, "NewlyEarnedStarsRow2");
    FsmGameObject fsmGameObject = this.m_rankUpFSM.FsmVariables.GetFsmGameObject("LegendRankText");
    if (fsmGameObject != null)
      fsmGameObject.Value = this.m_currRankedMedalLegendText.gameObject;
    this.m_isPlayingAnimWithCancelPoint = true;
    this.m_rankUpFSM.SendEvent("StartAnim");
  }

  private void PlayRankDown()
  {
    this.m_isPlayingAnimWithCancelPoint = true;
    this.m_rankDownFSM.SendEvent("StartAnim");
  }

  private void PlayStarChange(bool canLoseStars, bool canLoseLevel)
  {
    int count = this.m_currMedalDataModel.Stars - this.m_prevMedalDataModel.Stars;
    if (count < 0)
    {
      this.m_prevMedalStars.PopulateFsmArrayWithStars(this.m_starLossFSM, "LostMedalStars", this.m_prevMedalDataModel.Stars - 1, Mathf.Abs(count));
      this.m_starLossFSM.SendEvent("StartAnim");
    }
    else
    {
      switch (count)
      {
        case 0:
          if (this.m_currMedalDataModel.IsLegend)
            this.m_currRankedMedalLegendText.Show();
          else if (!this.m_currMedalDataModel.IsNewPlayer)
          {
            if (!canLoseStars)
              this.m_cannotLoseStarText.gameObject.SetActive(true);
            else if (!canLoseLevel)
              this.m_cannotLoseLevelText.gameObject.SetActive(true);
          }
          this.EnableClickToContinue();
          break;
        case 1:
          if (this.m_prevMedalDataModel.Stars > 0)
            this.m_prevMedalStars.PopulateFsmArrayWithStars(this.m_starGainSingleFSM, "AlreadyEarnedMedalStars", count: this.m_prevMedalDataModel.Stars);
          this.m_prevMedalStars.PopulateFsmArrayWithStars(this.m_starGainSingleFSM, "NewlyEarnedMedalStars", this.m_prevMedalDataModel.Stars, count);
          this.m_starGainSingleFSM.SendEvent("StartAnim");
          break;
        default:
          this.PopuplateBasicFsmVars(this.m_starGainMultiFSM);
          this.m_prevMedalStars.PopulateFsmArrayWithStars(this.m_starGainMultiFSM, "UnearnedMedalStars", this.m_prevMedalDataModel.Stars, count);
          this.m_newlyEarnedStars.PopulateFsmArrayWithStars(this.m_starGainMultiFSM, "NewlyEarnedStars");
          this.m_starGainMultiFSM.SendEvent("StartAnim");
          break;
      }
    }
  }

  private void PopuplateBasicFsmVars(PlayMakerFSM fsm)
  {
    FsmBool fsmBool1 = fsm.FsmVariables.GetFsmBool("IsWinStreak");
    if (fsmBool1 != null)
      fsmBool1.Value = this.m_isOnWinStreak;
    FsmInt fsmInt = fsm.FsmVariables.GetFsmInt("StarMultiplier");
    if (fsmInt != null)
      fsmInt.Value = this.m_prevMedalDataModel.StarMultiplier;
    FsmBool fsmBool2 = fsm.FsmVariables.GetFsmBool("IsLegend");
    if (fsmBool2 == null)
      return;
    fsmBool2.Value = this.m_currMedalDataModel.IsLegend;
  }

  private void HandleMissingRankChange() => this.EnableClickToContinue();

  public static void DebugShowFake(
    int leagueId,
    int starLevel,
    int stars,
    int starsPerWin,
    PegasusShared.FormatType formatType,
    bool isWinStreak,
    bool showWin)
  {
    RankChangeTwoScoop_NEW.DebugShowHelper(MedalInfoTranslator.DebugCreateMedalInfo(leagueId, starLevel, stars, starsPerWin, formatType, isWinStreak, showWin), formatType);
  }

  public static void DebugShowHelper(MedalInfoTranslator medalInfoTranslator, PegasusShared.FormatType formatType)
  {
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      RankChangeTwoScoop_NEW component = go.GetComponent<RankChangeTwoScoop_NEW>();
      if ((bool) UniversalInputManager.UsePhoneUI)
        component.transform.localPosition = new Vector3(0.0f, 156.5f, 1.34f);
      else
        component.transform.localPosition = new Vector3(0.0f, 292f, -9f);
      component.ActivateDebugEquivalentsOfEndGameScreen();
      component.Initialize(medalInfoTranslator, formatType, (Action) null);
      component.Show();
    });
    AssetLoader.Get().InstantiatePrefab(RankMgr.RANK_CHANGE_TWO_SCOOP_PREFAB_NEW, callback);
  }

  private void ActivateDebugEquivalentsOfEndGameScreen()
  {
    this.m_isRankChangeCheat = true;
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
    this.m_debugClickCatcher.gameObject.SetActive(true);
  }

  [ContextMenu("Test StarLoss")]
  private void TestStarLoss()
  {
    this.m_isRankChanging = false;
    this.m_prevMedalDataModel = this.PrepareFakeDataModel();
    this.m_currMedalDataModel = this.PrepareFakeDataModel();
    if (this.m_currMedalDataModel.Stars == 0)
      this.m_prevMedalDataModel.Stars = 1;
    else
      --this.m_currMedalDataModel.Stars;
    this.TestShow((Action<object>) (_ => this.PlayStarChange(this.m_testCanLoseStars, this.m_testCanLoseLevel)));
  }

  [ContextMenu("Test StarGainSingle")]
  private void TestStarGainSingle()
  {
    this.m_isRankChanging = false;
    this.m_isOnWinStreak = this.m_testWinStreak;
    this.m_prevMedalDataModel = this.PrepareFakeDataModel();
    this.m_currMedalDataModel = this.PrepareFakeDataModel();
    ++this.m_currMedalDataModel.Stars;
    this.m_currMedalDataModel.Stars = Mathf.Max(3, this.m_currMedalDataModel.Stars);
    this.TestShow((Action<object>) (_ => this.PlayStarChange(this.m_testCanLoseStars, this.m_testCanLoseLevel)));
  }

  [ContextMenu("Test StarGainMulti")]
  private void TestStarGainMulti()
  {
    this.m_isRankChanging = false;
    this.m_isOnWinStreak = this.m_testWinStreak;
    this.m_prevMedalDataModel = this.PrepareFakeDataModel();
    this.m_currMedalDataModel = this.PrepareFakeDataModel();
    int testStarMultiplier = this.m_testStarMultiplier;
    if (this.m_testWinStreak)
      testStarMultiplier *= 2;
    this.m_currMedalDataModel.Stars += testStarMultiplier;
    this.m_currMedalDataModel.Stars = Mathf.Max(3, this.m_currMedalDataModel.Stars);
    this.TestShow((Action<object>) (_ => this.PlayStarChange(this.m_testCanLoseStars, this.m_testCanLoseLevel)));
  }

  [ContextMenu("Test RankUp")]
  private void TestRankUp()
  {
    this.m_isRankChanging = true;
    this.m_isOnWinStreak = this.m_testWinStreak;
    this.m_prevMedalDataModel = this.PrepareFakeDataModel();
    this.m_currMedalDataModel = this.PrepareFakeDataModel();
    int testStarMultiplier = this.m_testStarMultiplier;
    if (this.m_testWinStreak)
      testStarMultiplier *= 2;
    int num1 = Mathf.Max(1, testStarMultiplier - (this.m_currMedalDataModel.MaxStars - this.m_currMedalDataModel.Stars));
    int num2 = num1 / 3;
    int num3 = num1 % 3;
    this.m_currMedalDataModel.StarLevel += num2;
    this.m_currMedalDataModel.Stars = num3;
    this.TestShow((Action<object>) (_ => this.PlayRankUp()));
  }

  [ContextMenu("Test RankDown")]
  private void TestRankDown()
  {
    this.m_isRankChanging = true;
    this.m_prevMedalDataModel = this.PrepareFakeDataModel();
    this.m_currMedalDataModel = this.PrepareFakeDataModel();
    --this.m_currMedalDataModel.StarLevel;
    this.m_currMedalDataModel.StarLevel = Mathf.Max(1, this.m_currMedalDataModel.StarLevel);
    this.m_currMedalDataModel.Stars = 2;
    this.TestShow((Action<object>) (_ => this.PlayRankDown()));
  }

  private bool TestShow(Action<object> showFunc)
  {
    if (this.m_medalInfoTranslator == null)
      this.m_medalInfoTranslator = new MedalInfoTranslator();
    this.Reset();
    if ((UnityEngine.Object) this.m_prevRankedMedalWidget != (UnityEngine.Object) null)
      this.m_prevRankedMedalWidget.BindDataModel((IDataModel) this.m_prevMedalDataModel);
    if (this.m_isRankChanging && (UnityEngine.Object) this.m_currRankedMedalWidget != (UnityEngine.Object) null)
      this.m_currRankedMedalWidget.BindDataModel((IDataModel) this.m_currMedalDataModel);
    this.InitializeFromDataModels();
    this.StartCoroutine(this.ShowWhenReady(showFunc));
    return true;
  }

  private RankedPlayDataModel PrepareFakeDataModel() => new RankedPlayDataModel()
  {
    Stars = this.m_testStars,
    MaxStars = 3,
    StarMultiplier = this.m_testStarMultiplier,
    StarLevel = this.m_testStarLevel,
    MedalText = this.m_testStarLevel.ToString(),
    RankName = this.m_testStarLevel.ToString(),
    IsNewPlayer = this.m_testLeagueType == League.LeagueType.NEW_PLAYER,
    IsLegend = this.m_testLegend,
    LegendRank = 1337,
    FormatType = this.m_formatType
  };
}
