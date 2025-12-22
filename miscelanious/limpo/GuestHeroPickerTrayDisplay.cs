using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GuestHeroPickerTrayDisplay : AbsDeckPickerTrayDisplay
{
  public UberText m_heroDescription;
  public UberText m_chooseHeroLabel;
  [CustomEditField(Sections = "Hero Divot Textures")]
  public Texture m_divotTextureDefault;
  [CustomEditField(Sections = "Hero Divot Textures")]
  public Texture m_divotTextureDalaran;
  [CustomEditField(Sections = "Hero Divot Textures")]
  public Texture m_divotTextureUldum;
  [CustomEditField(Sections = "Hero Divot Textures")]
  public Texture m_divotTexturePVPDR;
  private static GuestHeroPickerTrayDisplay s_instance;

  public override void Awake()
  {
    base.Awake();
    GuestHeroPickerTrayDisplay.s_instance = this;
    HeroPickerDataModel heroPickerDataModel = this.GetHeroPickerDataModel();
    if (heroPickerDataModel != null)
      heroPickerDataModel.HasGuestHeroes = true;
    VisualController visualController = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) visualController != (UnityEngine.Object) null)
      visualController.Owner.RegisterReadyListener((Action<object>) (_ => this.OnHeroPickerWidgetReady(visualController.Owner)), (object) null, true);
    else
      Debug.LogError((object) "AbsDeckPickerTrayDisplay.Awake - could not find visual controller. Ensure that this component is on the same object as the visual controller.");
  }

  private void Start() => Navigation.PushIfNotOnTop(new Navigation.NavigateBackHandler(GuestHeroPickerTrayDisplay.OnNavigateBack));

  private void Update()
  {
    if ((UnityEngine.Object) AdventureScene.Get() == (UnityEngine.Object) null || !AdventureScene.Get().IsDevMode || !InputCollection.GetKeyDown(KeyCode.Z))
      return;
    this.Cheat_LockAllButtons();
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (!((UnityEngine.Object) GuestHeroPickerTrayDisplay.Get() == (UnityEngine.Object) this))
      return;
    GuestHeroPickerTrayDisplay.s_instance = (GuestHeroPickerTrayDisplay) null;
  }

  public static bool OnNavigateBack()
  {
    if ((UnityEngine.Object) GuestHeroPickerTrayDisplay.Get() != (UnityEngine.Object) null)
      return GuestHeroPickerTrayDisplay.Get().OnNavigateBackImplementation();
    Debug.LogError((object) "GuestHeroPickerTrayDisplay: tried to navigate back but had null instance!");
    return false;
  }

  protected override IEnumerator InitModeWhenReady()
  {
    GuestHeroPickerTrayDisplay pickerTrayDisplay = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) pickerTrayDisplay.StartCoroutine(pickerTrayDisplay.\u003C\u003En__0());
    if (SceneMgr.Get().IsInDuelsMode())
      pickerTrayDisplay.SetBackButtonEnabled(false);
    pickerTrayDisplay.ShowFirstPage();
  }

  protected override void InitForMode(SceneMgr.Mode mode)
  {
    this.GetComponent<VisualController>();
    switch (mode)
    {
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        string key = TavernBrawlManager.Get().CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_HEROIC ? "GLOBAL_HEROIC_BRAWL" : "GLOBAL_TAVERN_BRAWL";
        TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
        ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(tavernBrawlMission.missionId);
        this.SetHeaderText(GameStrings.Get(key));
        if (record.ChooseHeroText != null)
          this.SetChooseHeroText((string) record.ChooseHeroText);
        if ((UnityEngine.Object) GuestHeroPickerDisplay.Get() != (UnityEngine.Object) null && mode != SceneMgr.Mode.FRIENDLY)
        {
          GuestHeroPickerDisplay.Get().ShowTray();
          break;
        }
        break;
      case SceneMgr.Mode.ADVENTURE:
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
        AdventureConfig adventureConfig = AdventureConfig.Get();
        this.SetHeaderText((string) GameUtils.GetAdventureDataRecord((int) adventureConfig.GetSelectedAdventure(), (int) adventureConfig.GetSelectedMode()).Name);
        AdventureSubScene component = GuestHeroPickerDisplay.Get().GetComponent<AdventureSubScene>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.SetIsLoaded(true);
        if (mode == SceneMgr.Mode.PVP_DUNGEON_RUN && (UnityEngine.Object) GuestHeroPickerDisplay.Get() != (UnityEngine.Object) null)
        {
          GuestHeroPickerDisplay.Get().ShowTray();
          break;
        }
        break;
    }
    this.SetPlayButtonText(GameStrings.Get("GLOBAL_PLAY"));
    base.InitForMode(mode);
  }

  protected override void InitHeroPickerButtons()
  {
    base.InitHeroPickerButtons();
    List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer> guestHeroes = this.GetGuestHeroes();
    if (guestHeroes == null)
    {
      Debug.LogError((object) "InitHeroPickerButtons: Unable to get guest heroes to display.");
    }
    else
    {
      IDataModel model;
      if (!GlobalDataContext.Get().GetDataModel(7, out model))
      {
        model = (IDataModel) new AdventureDataModel();
        GlobalDataContext.Get().BindDataModel(model);
      }
      if (!(model is AdventureDataModel adventureDataModel))
        Log.Adventures.PrintWarning("AdventureDataModel is null!");
      Texture texture;
      switch (adventureDataModel.SelectedAdventure)
      {
        case AdventureDbId.DALARAN:
          texture = this.m_divotTextureDalaran;
          break;
        case AdventureDbId.ULDUM:
          texture = this.m_divotTextureUldum;
          break;
        default:
          texture = SceneMgr.Get().IsInDuelsMode() ? this.m_divotTexturePVPDR : this.m_divotTextureDefault;
          break;
      }
      this.m_heroDefsLoading = guestHeroes.Count;
      for (int index = 0; index < guestHeroes.Count; ++index)
      {
        if (index >= this.m_heroButtons.Count || (UnityEngine.Object) this.m_heroButtons[index] == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "InitHeroPickerButtons: not enough buttons for total guest heroes.");
          break;
        }
        GuestHeroPickerButton heroButton = this.m_heroButtons[index] as GuestHeroPickerButton;
        if ((UnityEngine.Object) heroButton == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "InitHeroPickerButtons: attempted to display null button.");
          --this.m_heroDefsLoading;
        }
        else
        {
          GuestHeroDbfRecord guestHeroRecord = guestHeroes[index].GuestHeroRecord;
          if (guestHeroRecord == null)
          {
            heroButton.Lock();
            heroButton.Activate(false);
          }
          else
          {
            heroButton.Unlock();
            heroButton.Activate(true);
          }
          long preconDeckID = 0;
          TAG_CLASS heroClass = guestHeroRecord == null ? TAG_CLASS.INVALID : GameUtils.GetTagClassFromCardDbId(guestHeroRecord.CardId);
          if (heroClass != TAG_CLASS.INVALID)
          {
            CollectionManager.PreconDeck preconDeck = CollectionManager.Get().GetPreconDeck(heroClass);
            if (preconDeck != null)
              preconDeckID = preconDeck.ID;
          }
          heroButton.SetPreconDeckID(preconDeckID);
          heroButton.SetGuestHero(guestHeroRecord);
          AdventureGuestHeroesDbfRecord adventureGuestHeroRecord = guestHeroes[index].AdventureGuestHeroRecord;
          if (adventureGuestHeroRecord != null && !SceneMgr.Get().IsInDuelsMode())
            this.HandleAdventureGuestHeroUnlockData(adventureGuestHeroRecord, (HeroPickerButton) heroButton);
          if (guestHeroRecord == null)
          {
            --this.m_heroDefsLoading;
            heroButton.UpdateDisplay((DefLoader.DisposableFullDef) null, TAG_PREMIUM.NORMAL);
          }
          else
          {
            string cardId = GameUtils.TranslateDbIdToCardId(guestHeroRecord.CardId);
            AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData userData = new AbsDeckPickerTrayDisplay.HeroFullDefLoadedCallbackData((HeroPickerButton) heroButton, TAG_PREMIUM.NORMAL);
            DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(((AbsDeckPickerTrayDisplay) this).OnHeroFullDefLoaded), (object) userData);
          }
          heroButton.SetDivotTexture(texture);
        }
      }
      if (!this.IsChoosingHeroForDungeonCrawlAdventure())
        return;
      this.SetUpHeroCrowns();
    }
  }

  protected override int ValidateHeroCount() => this.GetGuestHeroes().Count;

  protected override bool OnNavigateBackImplementation()
  {
    if (!base.OnNavigateBackImplementation())
      return false;
    switch (SceneMgr.Get() != null ? SceneMgr.Get().GetMode() : SceneMgr.Mode.INVALID)
    {
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        if ((UnityEngine.Object) GuestHeroPickerDisplay.Get() != (UnityEngine.Object) null)
        {
          GuestHeroPickerDisplay.Get().HideTray();
          break;
        }
        break;
      case SceneMgr.Mode.ADVENTURE:
        if ((UnityEngine.Object) AdventureConfig.Get() != (UnityEngine.Object) null)
        {
          AdventureConfig.Get().SubSceneGoBack();
          break;
        }
        break;
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
        if ((UnityEngine.Object) PvPDungeonRunDisplay.Get() != (UnityEngine.Object) null)
        {
          PvPDungeonRunScene.Get().TransitionBackFromGuestHeroPicker();
          break;
        }
        break;
    }
    return true;
  }

  public override void PreUnload()
  {
    if (!this.m_randomDeckPickerTray.activeSelf)
      return;
    this.m_randomDeckPickerTray.SetActive(false);
  }

  protected override void UpdateHeroInfo(HeroPickerButton button)
  {
    using (DefLoader.DisposableFullDef fullDef = button.ShareFullDef())
    {
      string name = fullDef.EntityDef.GetName();
      string heroDescription = string.Empty;
      GuestHeroDbfRecord guestHero = button.GetGuestHero();
      if (guestHero != null)
        heroDescription = (string) guestHero.FlavorText;
      TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
      if (SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE)
        premium = TAG_PREMIUM.GOLDEN;
      this.UpdateHeroInfo(fullDef, name, heroDescription, premium);
    }
  }

  protected override void PlayGame()
  {
    base.PlayGame();
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.ADVENTURE:
        int selectedHeroId = this.GetSelectedHeroID();
        AdventureConfig ac = AdventureConfig.Get();
        if (this.OnPlayButtonPressed_SaveHeroAndAdvanceToDungeonRunIfNecessary())
        {
          GuestHeroDbfRecord guestHero = this.m_selectedHeroButton.GetGuestHero();
          if (guestHero != null)
          {
            AdventureGuestHeroesDbfRecord record = GameDbf.AdventureGuestHeroes.GetRecord((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == ac.GetSelectedAdventure() && r.GuestHeroId == guestHero.ID));
            if (record != null && record.CustomScenario != 0)
            {
              ac.SetMission((ScenarioDbId) record.CustomScenario);
              break;
            }
            break;
          }
          break;
        }
        ac.SubSceneGoBack(false);
        ScenarioDbId missionToPlay = ac.GetMissionToPlay();
        GameMgr.Get().FindGameWithHero(GameType.GT_VS_AI, FormatType.FT_WILD, (int) missionToPlay, 0, selectedHeroId);
        break;
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
        PvPDungeonRunScene.Get().OnGuestHeroSelected(this.m_selectedHeroButton.m_heroClass, this.m_selectedHeroButton.GetGuestHero());
        this.EnableBackButton(false);
        break;
    }
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_slidingTray.ToggleTraySlider(false);
  }

  protected override void ShowHero()
  {
    this.UpdateHeroInfo(this.m_selectedHeroButton);
    base.ShowHero();
  }

  protected override void SelectHero(HeroPickerButton button, bool showTrayForPhone = true)
  {
    base.SelectHero(button, showTrayForPhone);
    this.UpdateSelectedHeroClasses(button);
    if (SceneMgr.Get() == null || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
      return;
    HeroPickerOptionDataModel dataModel = button.GetDataModel();
    int num = dataModel == null ? 0 : (dataModel.IsTimelocked ? 1 : 0);
    bool flag = dataModel != null && dataModel.IsUnowned;
    if (num != 0)
    {
      string description = string.Format("{0} ({1})", (object) dataModel.ComingSoonText, (object) dataModel.UnlockCriteriaText);
      this.AddHeroLockedTooltip(GameStrings.Get("GLOBAL_NOT_AVAILABLE"), description);
    }
    else if (flag)
    {
      this.AddHeroLockedTooltip(GameStrings.Get("GLUE_HERO_LOCKED_NAME"), dataModel.UnlockCriteriaText);
    }
    else
    {
      if (button.IsLocked())
        return;
      AdventureDataDbfRecord adventureDataRecord = AdventureConfig.Get().GetSelectedAdventureDataRecord();
      List<long> values = (List<long>) null;
      if (!GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_UNLOCKED_HEROES, out values))
        values = new List<long>();
      if (values.Contains((long) button.GetGuestHero().CardId))
        return;
      values.Add((long) this.m_selectedHeroButton.GetGuestHero().CardId);
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest((GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_UNLOCKED_HEROES, values.ToArray()));
      button.GetDataModel().IsNewlyUnlocked = false;
    }
  }

  protected override bool IsHeroPlayable(HeroPickerButton button)
  {
    HeroPickerOptionDataModel dataModel = button.GetDataModel();
    if (button.IsLocked())
      return false;
    return dataModel == null || !dataModel.IsUnowned;
  }

  protected override bool ShouldShowHeroPower() => true;

  protected override void GoBackUntilOnNavigateBackCalled() => Navigation.GoBackUntilOnNavigateBackCalled(new Navigation.NavigateBackHandler(GuestHeroPickerTrayDisplay.OnNavigateBack));

  public void EnableBackButton(bool enabled)
  {
    if (!((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null))
      return;
    this.m_backButton.SetEnabled(enabled);
    this.m_backButton.Flip(enabled);
  }

  private void OnHeroPickerWidgetReady(WidgetTemplate widget)
  {
    if (!((UnityEngine.Object) this.m_collectionButton != (UnityEngine.Object) null))
      return;
    this.SetCollectionButtonEnabled(false);
  }

  private void HandleAdventureGuestHeroUnlockData(
    AdventureGuestHeroesDbfRecord adventureGuestHeroRecord,
    HeroPickerButton button)
  {
    if (adventureGuestHeroRecord == null)
    {
      Debug.LogError((object) "HandleGuestHeroUnlockEvents: No adventure guest hero passed in.");
    }
    else
    {
      WingDbfRecord record = GameDbf.Wing.GetRecord(adventureGuestHeroRecord.WingId);
      bool flag = AdventureProgressMgr.IsWingEventActive(adventureGuestHeroRecord.WingId);
      string lockedReasonText = this.GetButtonLockedReasonText(record);
      button.SetLockReasonText(lockedReasonText);
      List<long> values = (List<long>) null;
      AdventureDataDbfRecord adventureDataRecord = AdventureConfig.Get().GetSelectedAdventureDataRecord();
      GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_UNLOCKED_HEROES, out values);
      HeroPickerOptionDataModel dataModel = button.GetDataModel();
      dataModel.IsTimelocked = !flag;
      dataModel.TimeLockInfoText = lockedReasonText;
      dataModel.ComingSoonText = (string) adventureGuestHeroRecord.ComingSoonText;
      dataModel.UnlockCriteriaText = (string) adventureGuestHeroRecord.UnlockCriteriaText;
      dataModel.IsUnowned = !AdventureProgressMgr.Get().OwnsWing(adventureGuestHeroRecord.WingId);
      dataModel.IsNewlyUnlocked = AdventureUtils.DoesAdventureShowNewlyUnlockedGuestHeroTreatment((AdventureDbId) adventureDataRecord.AdventureId) & flag && !dataModel.IsUnowned && (values == null || button.GetGuestHero() != null && !values.Contains((long) button.GetGuestHero().CardId));
      if (flag)
        return;
      button.Lock();
      button.Activate(false);
    }
  }

  private List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer> GetGuestHeroes()
  {
    List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer> guestHeroes = (List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer>) null;
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.TAVERN_BRAWL:
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        guestHeroes = this.GetGuestHeroes(this.GetScenarioGuestHeroes());
        break;
      case SceneMgr.Mode.ADVENTURE:
        guestHeroes = this.GetGuestHeroes(this.GetSortedAdventureGuestHeroes());
        break;
      case SceneMgr.Mode.PVP_DUNGEON_RUN:
        guestHeroes = this.GetGuestHeroes(this.GetDuelsDraftHeroes());
        break;
    }
    return guestHeroes;
  }

  private List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer> GetGuestHeroes(
    List<AdventureGuestHeroesDbfRecord> adventureGuestHeroes)
  {
    List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer> guestHeroes = new List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer>();
    foreach (AdventureGuestHeroesDbfRecord adventureGuestHero in adventureGuestHeroes)
    {
      GuestHeroDbfRecord record = GameDbf.GuestHero.GetRecord(adventureGuestHero.GuestHeroId);
      guestHeroes.Add(new GuestHeroPickerTrayDisplay.GuestHeroRecordContainer()
      {
        GuestHeroRecord = record,
        AdventureGuestHeroRecord = adventureGuestHero
      });
    }
    return guestHeroes;
  }

  private List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer> GetGuestHeroes(
    List<ScenarioGuestHeroesDbfRecord> scenarioGuestHeroes)
  {
    List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer> guestHeroes = new List<GuestHeroPickerTrayDisplay.GuestHeroRecordContainer>();
    foreach (ScenarioGuestHeroesDbfRecord scenarioGuestHero in scenarioGuestHeroes)
    {
      GuestHeroDbfRecord record = GameDbf.GuestHero.GetRecord(scenarioGuestHero.GuestHeroId);
      guestHeroes.Add(new GuestHeroPickerTrayDisplay.GuestHeroRecordContainer()
      {
        GuestHeroRecord = record,
        ScenarioGuestHeroRecord = scenarioGuestHero
      });
    }
    return guestHeroes;
  }

  private List<AdventureGuestHeroesDbfRecord> GetSortedAdventureGuestHeroes()
  {
    AdventureDbId currentAdventure = AdventureConfig.Get().GetSelectedAdventure();
    List<AdventureGuestHeroesDbfRecord> records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == currentAdventure));
    records.Sort((Comparison<AdventureGuestHeroesDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  private List<AdventureGuestHeroesDbfRecord> GetDuelsDraftHeroes()
  {
    List<long> draftedHeroes = DuelsConfig.GetDraftHeroesFromGSD();
    AdventureDbId currentAdventure = AdventureConfig.Get().GetSelectedAdventure();
    List<AdventureGuestHeroesDbfRecord> records;
    if (draftedHeroes != null && draftedHeroes.Count > 0)
    {
      records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == currentAdventure && draftedHeroes.Contains((long) r.GuestHeroId)));
    }
    else
    {
      PvpdrSeasonDbfRecord seasonDbfRecord = DuelsConfig.GetSeasonDBFRecord();
      int limit = 3;
      if (seasonDbfRecord != null)
        limit = seasonDbfRecord.MaxHeroesDrafted;
      records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == currentAdventure), limit);
    }
    records.Sort((Comparison<AdventureGuestHeroesDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  private List<ScenarioGuestHeroesDbfRecord> GetScenarioGuestHeroes()
  {
    TavernBrawlMission currentMission = TavernBrawlManager.Get().CurrentMission();
    List<ScenarioGuestHeroesDbfRecord> records = GameDbf.ScenarioGuestHeroes.GetRecords((Predicate<ScenarioGuestHeroesDbfRecord>) (r => r.ScenarioId == currentMission.missionId));
    records.Sort((Comparison<ScenarioGuestHeroesDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  private List<ScenarioGuestHeroesDbfRecord> GetPvPDungeonRunGuestHeroes()
  {
    int missionId = (int) AdventureConfig.Get().GetMission();
    List<ScenarioGuestHeroesDbfRecord> records = GameDbf.ScenarioGuestHeroes.GetRecords((Predicate<ScenarioGuestHeroesDbfRecord>) (r => r.ScenarioId == missionId));
    records.Sort((Comparison<ScenarioGuestHeroesDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder)));
    return records;
  }

  private string GetButtonLockedReasonText(WingDbfRecord wingRecord)
  {
    if (!wingRecord.UseUnlockCountdown)
      return (string) wingRecord.ComingSoonLabel;
    SpecialEventType wingEventTiming = AdventureProgressMgr.GetWingEventTiming(wingRecord.ID);
    DateTime? eventStartTimeUtc = SpecialEventManager.Get().GetEventStartTimeUtc(wingEventTiming);
    DateTime utcNow = DateTime.UtcNow;
    TimeSpan? nullable = eventStartTimeUtc.HasValue ? new TimeSpan?(eventStartTimeUtc.GetValueOrDefault() - utcNow) : new TimeSpan?();
    if (!nullable.HasValue)
      return GameStrings.Get("GLOBAL_DATETIME_COMING_SOON");
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_weeks = "GLOBAL_DATETIME_UNLOCKS_SOON_WEEKS"
    };
    return TimeUtils.GetElapsedTimeString((long) nullable.Value.TotalSeconds, stringSet, true);
  }

  private void ShowFirstPage()
  {
    if (iTween.Count(this.m_randomDeckPickerTray) > 0)
      return;
    this.m_randomDeckPickerTray.SetActive(true);
    this.ShowPreconHighlights();
  }

  private void ShowPreconHighlights()
  {
    if (!(bool) AbsDeckPickerTrayDisplay.HIGHLIGHT_SELECTED_DECK)
      return;
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      if ((UnityEngine.Object) heroButton == (UnityEngine.Object) this.m_selectedHeroButton)
        heroButton.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    }
  }

  private void UpdateHeroInfo(
    DefLoader.DisposableFullDef fullDef,
    string heroName,
    string heroDescription,
    TAG_PREMIUM premium)
  {
    this.m_heroName.Text = heroName;
    if ((UnityEngine.Object) this.m_heroDescription != (UnityEngine.Object) null)
      this.m_heroDescription.Text = heroDescription;
    this.m_selectedHeroName = fullDef.EntityDef.GetName();
    this.m_heroActor.SetPremium(premium);
    this.m_heroActor.SetEntityDef(fullDef.EntityDef);
    this.m_heroActor.SetCardDef(fullDef.DisposableCardDef);
    this.m_heroActor.UpdateAllComponents();
    this.m_heroActor.SetUnlit();
    string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(fullDef.EntityDef.GetCardId());
    if (!string.IsNullOrEmpty(powerCardIdFromHero))
    {
      this.UpdateHeroPowerInfo(this.m_heroPowerDefs[powerCardIdFromHero], premium);
    }
    else
    {
      this.SetHeroPowerActorColliderEnabled(false);
      this.HideHeroPowerActor();
    }
  }

  private void Cheat_LockAllButtons()
  {
    foreach (HeroPickerButton heroButton in this.m_heroButtons)
    {
      heroButton.Lock();
      heroButton.Activate(false);
    }
  }

  public static GuestHeroPickerTrayDisplay Get() => GuestHeroPickerTrayDisplay.s_instance;

  public void SetChooseHeroText(string text)
  {
    if (!((UnityEngine.Object) this.m_chooseHeroLabel != (UnityEngine.Object) null))
      return;
    this.m_chooseHeroLabel.Text = text;
  }

  private void UpdateSelectedHeroClasses(HeroPickerButton button)
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    HeroClassIconsDataModel classIconsDataModel = new HeroClassIconsDataModel();
    using (DefLoader.DisposableFullDef disposableFullDef = button.ShareFullDef())
    {
      EntityDef entityDef = disposableFullDef?.EntityDef;
      if (entityDef == null)
      {
        Debug.LogWarning((object) "GuestHeroPickerTrayDisplay.UpdateSelectedHeroClasses - button did not contain an entity def!");
      }
      else
      {
        classIconsDataModel.Classes.Clear();
        entityDef.GetClasses((IList<TAG_CLASS>) classIconsDataModel.Classes);
        classIconsDataModel.Classes.Sort((Comparison<TAG_CLASS>) ((a, b) => a != TAG_CLASS.NEUTRAL ? -1 : 1));
        component.Owner.BindDataModel((IDataModel) classIconsDataModel, false);
      }
    }
  }

  public delegate void GuestHeroSelectedCallback(TAG_CLASS classId, GuestHeroDbfRecord record);

  private struct GuestHeroRecordContainer
  {
    public GuestHeroDbfRecord GuestHeroRecord;
    public AdventureGuestHeroesDbfRecord AdventureGuestHeroRecord;
    public ScenarioGuestHeroesDbfRecord ScenarioGuestHeroRecord;
  }
}
