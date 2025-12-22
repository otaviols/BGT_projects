using Assets;
using Blizzard.T5.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureChooserTray : AccordionMenuTray
{
  private const string s_DefaultPortraitMaterialTextureName = "_MainTex";
  private const int s_DefaultPortraitMaterialIndex = 0;
  [SerializeField]
  [CustomEditField(Sections = "Sub Scene")]
  public AdventureSubScene m_ParentSubScene;
  [CustomEditField(Sections = "Choose Frame")]
  [SerializeField]
  public GameObject m_ComingSoonCoverUpSign;
  [SerializeField]
  [CustomEditField(Sections = "Choose Frame")]
  public UberText m_ComingSoonCoverUpSignHeaderText;
  [CustomEditField(Sections = "Choose Frame")]
  [SerializeField]
  public UberText m_ComingSoonCoverUpSignDescriptionText;
  private AdventureChooserDescription m_CurrentChooserDescription;
  private Map<AdventureDbId, Map<AdventureModeDbId, AdventureChooserDescription>> m_Descriptions = new Map<AdventureDbId, Map<AdventureModeDbId, AdventureChooserDescription>>();
  private bool m_isTransitioning = true;

  private void Awake()
  {
    this.m_ChooseButton.Disable();
    this.m_BackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBackButton()));
    this.m_ChooseButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ChangeSubScene()));
    AdventureConfig.Get().AddSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChange));
    AdventureProgressMgr.Get().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdated));
    Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    this.StartCoroutine(this.InitTrayWhenReady());
  }

  private void Start()
  {
    Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureChooserTray.OnNavigateBack));
    this.m_isStarted = true;
  }

  protected IEnumerator InitTrayWhenReady()
  {
    AdventureChooserTray context = this;
    if ((UnityEngine.Object) context.m_ChooseFrameScroller == (UnityEngine.Object) null || (UnityEngine.Object) context.m_ChooseFrameScroller.ScrollObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_ChooseFrameScroller or m_ChooseFrameScroller.m_ScrollObject cannot be null. Unable to create button.", (UnityEngine.Object) context);
    }
    else
    {
      int num = AdventureConfig.Get().PreviousSubScene != AdventureData.Adventuresubscene.INVALID ? 1 : 0;
      int latestActiveAdventureWing = 0;
      AdventureDbId adventurePlayerShouldSee = AdventureConfig.GetAdventurePlayerShouldSee(out latestActiveAdventureWing);
      if (!Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false))
      {
        Log.Adventures.Print("HAS_SEEN_PRACTICE_MODE set to true.");
        Options.Get().SetBool(Option.HAS_SEEN_PRACTICE_MODE, true);
      }
      if (num == 0 && adventurePlayerShouldSee != AdventureDbId.INVALID)
      {
        AdventureConfig.Get().SetSelectedAdventureMode(adventurePlayerShouldSee, AdventureConfig.GetDefaultModeDbIdForAdventure(adventurePlayerShouldSee));
        if (latestActiveAdventureWing != 0)
          GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LATEST_ADVENTURE_WING_SEEN, new long[1]
          {
            (long) latestActiveAdventureWing
          }));
      }
      List<AdventureDef> sortedAdventureDefs = AdventureScene.Get().GetSortedAdventureDefs();
      Map<AdventureDbId, List<AdventureDef>> map = new Map<AdventureDbId, List<AdventureDef>>();
      foreach (AdventureDef adventureDef in sortedAdventureDefs)
      {
        AdventureDbId adventureToNestUnder = adventureDef.m_AdventureToNestUnder;
        if (adventureToNestUnder != AdventureDbId.INVALID)
        {
          if (!map.ContainsKey(adventureToNestUnder))
            map.Add(adventureToNestUnder, new List<AdventureDef>());
          map[adventureToNestUnder].Add(adventureDef);
        }
      }
      List<Widget> buttonWidgets = new List<Widget>();
      foreach (AdventureDef advDef in sortedAdventureDefs)
      {
        if (AdventureConfig.ShouldDisplayAdventure(advDef.GetAdventureId()) && !advDef.IsNestedUnderAnotherAdventureOnChooserScreen)
        {
          List<AdventureDef> nestedAdvDefs = (List<AdventureDef>) null;
          if (map.ContainsKey(advDef.GetAdventureId()))
            nestedAdvDefs = map[advDef.GetAdventureId()];
          Widget adventureChooserButton = context.CreateAdventureChooserButton(advDef, nestedAdvDefs);
          if ((UnityEngine.Object) adventureChooserButton != (UnityEngine.Object) null)
            buttonWidgets.Add(adventureChooserButton);
        }
      }
      while (!buttonWidgets.TrueForAll((Predicate<Widget>) (w => w.IsReady && !w.IsChangingStates)))
        yield return (object) null;
      context.OnButtonVisualUpdated();
      if ((UnityEngine.Object) context.m_SelectedSubButton != (UnityEngine.Object) null && (UnityEngine.Object) context.m_ChooseFrameScroller != (UnityEngine.Object) null)
      {
        context.m_ChooseFrameScroller.UpdateScroll();
        context.m_ChooseFrameScroller.CenterObjectInView(context.m_SelectedSubButton.gameObject, 0.0f, (UIBScrollable.OnScrollComplete) null, iTween.EaseType.easeOutCubic, 0.0f);
      }
      if ((UnityEngine.Object) context.m_ParentSubScene != (UnityEngine.Object) null)
      {
        context.m_ParentSubScene.SetIsLoaded(true);
        context.m_ParentSubScene.AddSubSceneTransitionFinishedListener(new AdventureSubScene.SubSceneTransitionFinished(context.OnSubSceneTransitionFinished));
      }
      AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
      context.ShowComingSoonCoverUpSignIfActive(selectedAdventure);
    }
  }

  private void OnDestroy()
  {
    if ((UnityEngine.Object) AdventureConfig.Get() != (UnityEngine.Object) null)
      AdventureConfig.Get().RemoveSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChange));
    if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
      Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    if (AdventureProgressMgr.Get() != null)
      AdventureProgressMgr.Get().RemoveProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdated));
    this.CancelInvoke("ShowDisabledAdventureModeRequirementsWarning");
  }

  private void OnBackButton() => Navigation.GoBack();

  private static bool OnNavigateBack()
  {
    AdventureChooserTray.DisableTrayButtons();
    if (GameModeUtils.CanAccessGameModes())
      AdventureChooserTray.BackToGameModes();
    else
      AdventureChooserTray.BackToHub();
    return true;
  }

  private static void BackToGameModes() => SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAME_MODE, SceneMgr.TransitionHandlerType.NEXT_SCENE);

  private static void BackToHub() => SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);

  private static void DisableTrayButtons()
  {
    foreach (AdventureChooserTray adventureChooserTray in UnityEngine.Object.FindObjectsOfType<AdventureChooserTray>())
      adventureChooserTray.DisableAllButtons();
  }

  private Widget CreateAdventureChooserButton(
    AdventureDef advDef,
    List<AdventureDef> nestedAdvDefs)
  {
    string chooserButtonPrefab = this.m_DefaultChooserButtonPrefab;
    if (!string.IsNullOrEmpty(advDef.m_ChooserButtonPrefab))
      chooserButtonPrefab = advDef.m_ChooserButtonPrefab;
    Widget widget = (Widget) WidgetInstance.Create(chooserButtonPrefab);
    widget.RegisterReadyListener((Action<object>) (_ =>
    {
      AdventureChooserButton newbutton = widget.transform.GetComponentInChildren<AdventureChooserButton>();
      if ((UnityEngine.Object) newbutton == (UnityEngine.Object) null)
        return;
      GameUtils.SetParent((Component) widget, this.m_ChooseFrameScroller.ScrollObject);
      AdventureDbId adventureId = advDef.GetAdventureId();
      newbutton.gameObject.name = string.Format("{0}_{1}", (object) newbutton.gameObject.name, (object) adventureId);
      newbutton.SetAdventure(adventureId);
      newbutton.SetButtonText(advDef.GetAdventureName());
      newbutton.SetPortraitTexture(advDef.m_Texture);
      newbutton.SetPortraitTiling(advDef.m_TextureTiling);
      newbutton.SetPortraitOffset(advDef.m_TextureOffset);
      AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
      AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(selectedAdventure);
      if (selectedAdventure == adventureId || (UnityEngine.Object) adventureDef != (UnityEngine.Object) null && adventureDef.m_AdventureToNestUnder == adventureId)
        newbutton.Toggle = true;
      if (AdventureConfig.IsAdventureComingSoon(advDef.GetAdventureId()) && this.AreAllAdventuresComingSoon(nestedAdvDefs))
      {
        this.CreateAdventureChooserComingSoonSubButton(advDef, newbutton);
      }
      else
      {
        this.CreateAdventureChooserModeSubButtons(advDef, newbutton);
        if (nestedAdvDefs != null)
        {
          nestedAdvDefs.Sort((Comparison<AdventureDef>) ((l, r) => l.GetSortOrder() - r.GetSortOrder()));
          foreach (AdventureDef nestedAdvDef in nestedAdvDefs)
            this.CreateAdventureChooserModeSubButtons(nestedAdvDef, newbutton);
        }
      }
      newbutton.AddVisualUpdatedListener(new ChooserButton.VisualUpdated(((AccordionMenuTray) this).OnButtonVisualUpdated));
      int index = this.m_ChooserButtons.Count;
      newbutton.AddToggleListener((ChooserButton.Toggled) (toggle => this.OnChooserButtonToggled((ChooserButton) newbutton, toggle, index)));
      newbutton.AddModeSelectionListener(new ChooserButton.ModeSelection(this.ButtonModeSelected));
      newbutton.AddExpandedListener(new ChooserButton.Expanded(this.ButtonExpanded));
      this.m_ChooserButtons.Add((ChooserButton) newbutton);
      newbutton.FireVisualUpdatedEvent();
    }), (object) null, true);
    return widget;
  }

  private bool AreAllAdventuresComingSoon(List<AdventureDef> advDefs, bool emptyListDefault = true)
  {
    if (advDefs == null || advDefs.Count == 0)
      return emptyListDefault;
    foreach (AdventureDef advDef in advDefs)
    {
      if (!AdventureConfig.IsAdventureComingSoon(advDef.GetAdventureId()))
        return false;
    }
    return true;
  }

  private void CreateAdventureChooserModeSubButtons(
    AdventureDef advDef,
    AdventureChooserButton newbutton)
  {
    List<AdventureSubDef> sortedSubDefs = advDef.GetSortedSubDefs();
    AdventureDbId adventureId = advDef.GetAdventureId();
    AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
    AdventureModeDbId chooserAdventureMode = AdventureConfig.Get().GetClientChooserAdventureMode(adventureId);
    string chooserSubButtonPrefab = this.m_DefaultChooserSubButtonPrefab;
    if (!string.IsNullOrEmpty(advDef.m_ChooserSubButtonPrefab))
      chooserSubButtonPrefab = advDef.m_ChooserSubButtonPrefab;
    bool flag1 = true;
    if (advDef.IsNestedUnderAnotherAdventureOnChooserScreen)
      flag1 = !AdventureConfig.IsAdventureComingSoon(adventureId) && AdventureProgressMgr.Get().IsAdventureComplete(advDef.m_AdventureToNestUnder);
    foreach (AdventureSubDef subDef in sortedSubDefs)
    {
      AdventureModeDbId adventureModeId = subDef.GetAdventureModeId();
      AdventureChooserSubButton subButton = newbutton.CreateSubButton(adventureId, adventureModeId, subDef, chooserSubButtonPrefab, flag1 && chooserAdventureMode == adventureModeId);
      if (!((UnityEngine.Object) subButton == (UnityEngine.Object) null))
      {
        bool active = newbutton.Toggle && selectedAdventure == advDef.GetAdventureId() && chooserAdventureMode == adventureModeId;
        if (active)
        {
          subButton.SetHighlight(true);
          this.UpdateChooseButton(adventureId, adventureModeId);
          this.SetTitleText(adventureId, adventureModeId);
          this.m_SelectedSubButton = (ChooserSubButton) subButton;
        }
        else if (AdventureConfig.IsFeaturedMode(adventureId, adventureModeId))
          subButton.SetNewGlow(true);
        bool flag2 = AdventureConfig.CanPlayMode(adventureId, adventureModeId);
        subButton.SetDesaturate(!flag2);
        if (selectedAdventure == AdventureDbId.PRACTICE && adventureModeId == AdventureModeDbId.EXPERT && !flag2)
          subButton.SetContrast(0.3f);
        this.CreateAdventureChooserDescriptionFromPrefab(adventureId, subDef, active);
      }
    }
  }

  private void CreateAdventureChooserComingSoonSubButton(
    AdventureDef advDef,
    AdventureChooserButton newbutton)
  {
    AdventureDbId adventureId = advDef.GetAdventureId();
    AdventureModeDbId chooserAdventureMode = AdventureConfig.Get().GetClientChooserAdventureMode(adventureId);
    List<AdventureSubDef> sortedSubDefs = advDef.GetSortedSubDefs();
    AdventureSubDef subDef = new AdventureSubDef();
    AdventureModeDbId adventureModeDbId = AdventureModeDbId.LINEAR;
    if (sortedSubDefs.Count > 0)
    {
      subDef = sortedSubDefs[0];
      adventureModeDbId = sortedSubDefs[0].GetAdventureModeId();
    }
    ChooserSubButton comingSoonSubButton = (ChooserSubButton) newbutton.CreateComingSoonSubButton(adventureModeDbId, this.m_DefaultChooserComingSoonSubButtonPrefab);
    if ((UnityEngine.Object) comingSoonSubButton == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "newSubButton cannot be null. Unable to create newSubButton.", (UnityEngine.Object) this);
    }
    else
    {
      if ((!newbutton.Toggle ? 0 : (chooserAdventureMode == adventureModeDbId ? 1 : 0)) != 0)
      {
        comingSoonSubButton.SetHighlight(true);
        this.UpdateChooseButton(adventureId, adventureModeDbId);
        this.SetTitleText(adventureId, adventureModeDbId);
        this.m_SelectedSubButton = comingSoonSubButton;
      }
      this.CreateAdventureChooserDescriptionFromPrefab(adventureId, subDef, newbutton.Toggle);
    }
  }

  private void CreateAdventureChooserDescriptionFromPrefab(
    AdventureDbId adventureId,
    AdventureSubDef subDef,
    bool active)
  {
    if (string.IsNullOrEmpty((string) (MobileOverrideValue<string>) subDef.m_ChooserDescriptionPrefab))
      return;
    Map<AdventureModeDbId, AdventureChooserDescription> map;
    if (!this.m_Descriptions.TryGetValue(adventureId, out map))
    {
      map = new Map<AdventureModeDbId, AdventureChooserDescription>();
      this.m_Descriptions[adventureId] = map;
    }
    string descText = subDef.GetDescription();
    string requiredText = (string) null;
    if (!AdventureConfig.CanPlayMode(adventureId, subDef.GetAdventureModeId(), false))
    {
      requiredText = subDef.GetRequirementsDescription();
      if (!string.IsNullOrEmpty(subDef.GetLockedDescription()))
        descText = subDef.GetLockedDescription();
    }
    AdventureChooserDescription child = GameUtils.LoadGameObjectWithComponent<AdventureChooserDescription>((string) (MobileOverrideValue<string>) subDef.m_ChooserDescriptionPrefab);
    if ((UnityEngine.Object) child == (UnityEngine.Object) null)
      return;
    GameUtils.SetParent((Component) child, this.m_DescriptionContainer);
    child.SetText(requiredText, descText);
    child.m_WidgetElement.RegisterReadyListener<Widget>((Action<Widget>) (w =>
    {
      if (!((UnityEngine.Object) w != (UnityEngine.Object) null))
        return;
      AdventureChooserDescriptionDataModel dataModel = new AdventureChooserDescriptionDataModel();
      dataModel.Heroes = AdventureUtils.GetAvailableGuestHeroesAsCardListSortedByReleaseDate(adventureId);
      this.StartCoroutine(this.UpdateDataModelWhenGameSaveDataIsReady(dataModel, adventureId, subDef.GetAdventureModeId(), active));
      w.BindDataModel((IDataModel) dataModel);
    }));
    child.gameObject.SetActive(active);
    map[subDef.GetAdventureModeId()] = child;
    if (!active)
      return;
    this.m_CurrentChooserDescription = child;
  }

  private IEnumerator UpdateDataModelWhenGameSaveDataIsReady(
    AdventureChooserDescriptionDataModel dataModel,
    AdventureDbId adventureId,
    AdventureModeDbId modeId,
    bool active)
  {
    dataModel.HasNewHero = false;
    if (AdventureUtils.DoesAdventureShowNewlyUnlockedGuestHeroTreatment(adventureId))
    {
      GameSaveKeyId adventureClientKey = (GameSaveKeyId) AdventureConfig.GetAdventureDataRecord(adventureId, modeId).GameSaveDataClientKey;
      if (GameSaveDataManager.IsGameSaveKeyValid(adventureClientKey))
      {
        if (active && !GameSaveDataManager.Get().IsDataReady(adventureClientKey))
        {
          GameSaveDataManager.Get().Request(adventureClientKey, (GameSaveDataManager.OnRequestDataResponseDelegate) (success =>
          {
            if (success)
              dataModel.HasNewHero = AdventureUtils.DoesAdventureHaveUnseenGuestHeroes(adventureId, modeId);
            else
              Log.Adventures.PrintWarning("Unable to set AdventureChooserDescriptionDataModel.HasNewHero - GameSaveData request failed!");
          }));
        }
        else
        {
          while (!GameSaveDataManager.Get().IsDataReady(adventureClientKey))
          {
            Log.Adventures.Print("Waiting for client key {0} before updating DataModel for that Adventure Chooser Description!", (object) adventureClientKey);
            yield return (object) null;
          }
          dataModel.HasNewHero = AdventureUtils.DoesAdventureHaveUnseenGuestHeroes(adventureId, modeId);
        }
      }
    }
  }

  private AdventureChooserDescription GetAdventureChooserDescription(
    AdventureDbId adventureId,
    AdventureModeDbId modeId)
  {
    Map<AdventureModeDbId, AdventureChooserDescription> map;
    if (!this.m_Descriptions.TryGetValue(adventureId, out map))
      return (AdventureChooserDescription) null;
    AdventureChooserDescription chooserDescription;
    return !map.TryGetValue(modeId, out chooserDescription) ? (AdventureChooserDescription) null : chooserDescription;
  }

  private void ButtonModeSelected(ChooserSubButton btn)
  {
    foreach (ChooserButton chooserButton in this.m_ChooserButtons)
      chooserButton.DisableSubButtonHighlights();
    AdventureChooserSubButton chooserSubButton = (AdventureChooserSubButton) btn;
    this.m_SelectedSubButton = (ChooserSubButton) chooserSubButton;
    if (AdventureConfig.MarkFeaturedMode(chooserSubButton.GetAdventure(), chooserSubButton.GetMode()))
      btn.SetNewGlow(false);
    AdventureConfig.Get().SetSelectedAdventureMode(chooserSubButton.GetAdventure(), chooserSubButton.GetMode());
    this.SetTitleText(chooserSubButton.GetAdventure(), chooserSubButton.GetMode());
  }

  protected void ButtonExpanded(ChooserButton button, bool expand)
  {
    if (!expand)
      return;
    this.ToggleScrollable(true);
    AdventureChooserButton adventureChooserButton = (AdventureChooserButton) button;
    foreach (ChooserSubButton subButton1 in adventureChooserButton.GetSubButtons())
    {
      AdventureChooserSubButton subButton2 = (AdventureChooserSubButton) subButton1;
      if (AdventureConfig.IsFeaturedMode(adventureChooserButton.GetAdventure(), subButton2.GetMode()))
        subButton1.Flash();
      if (AdventureConfig.ShouldShowNewModePopup(adventureChooserButton.GetAdventure(), subButton2.GetMode()))
        this.StartCoroutine(this.ShowNewModePopupOnSubButtonAfterScrollingFinished(subButton2));
    }
  }

  private IEnumerator ShowNewModePopupOnSubButtonAfterScrollingFinished(
    AdventureChooserSubButton subButton)
  {
    yield return (object) new WaitForEndOfFrame();
    yield return (object) new WaitForEndOfFrame();
    while (this.m_isTransitioning)
      yield return (object) new WaitForEndOfFrame();
    subButton.ShowNewModePopup(GameStrings.Get("GLUE_ADVENTURE_NEW_MODE_UNLOCKED_POPUP_TEXT"));
    subButton.HideNewModePopupAfterDelay();
    AdventureConfig.MarkHasSeenNewModePopup(subButton.GetAdventure(), subButton.GetMode());
  }

  private void SetTitleText(AdventureDbId adventureId, AdventureModeDbId modeId) => this.m_DescriptionTitleObject.Text = (string) GameUtils.GetAdventureDataRecord((int) adventureId, (int) modeId).Name;

  private void OnSelectedModeChange(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    AdventureChooserDescription chooserDescription = this.GetAdventureChooserDescription(adventureId, modeId);
    if ((UnityEngine.Object) this.m_CurrentChooserDescription != (UnityEngine.Object) chooserDescription)
    {
      if ((UnityEngine.Object) this.m_CurrentChooserDescription != (UnityEngine.Object) null)
        this.m_CurrentChooserDescription.gameObject.SetActive(false);
      this.m_CurrentChooserDescription = chooserDescription;
      if ((UnityEngine.Object) this.m_CurrentChooserDescription != (UnityEngine.Object) null)
        this.m_CurrentChooserDescription.gameObject.SetActive(true);
    }
    this.UpdateChooseButton(adventureId, modeId);
    if (this.m_ChooseButton.IsEnabled())
    {
      PlayMakerFSM component = this.m_ChooseButton.GetComponent<PlayMakerFSM>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.SendEvent("Burst");
    }
    this.ShowComingSoonCoverUpSignIfActive(adventureId);
    if (AdventureConfig.CanPlayMode(adventureId, modeId, false))
      return;
    if (!this.m_isStarted)
      this.Invoke("ShowDisabledAdventureModeRequirementsWarning", 0.0f);
    else
      this.ShowDisabledAdventureModeRequirementsWarning();
  }

  private void ShowComingSoonCoverUpSignIfActive(AdventureDbId adventureId)
  {
    if (AdventureConfig.IsAdventureComingSoon(adventureId))
    {
      this.m_ComingSoonCoverUpSign.SetActive(true);
      this.SetComingSoonCoverUpSignText(adventureId);
    }
    else
      this.m_ComingSoonCoverUpSign.SetActive(false);
  }

  private void SetComingSoonCoverUpSignText(AdventureDbId adventureId)
  {
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) adventureId);
    this.m_ComingSoonCoverUpSignHeaderText.Text = (string) record.ComingSoonText;
    this.m_ComingSoonCoverUpSignDescriptionText.Text = TimeUtils.GetComingSoonText(record.ComingSoonEvent);
  }

  private void ShowDisabledAdventureModeRequirementsWarning()
  {
    this.CancelInvoke(nameof (ShowDisabledAdventureModeRequirementsWarning));
    if (!this.m_isStarted || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE || !((UnityEngine.Object) this.m_ChooseButton != (UnityEngine.Object) null) || this.m_ChooseButton.IsEnabled())
      return;
    AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
    AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
    if (AdventureConfig.CanPlayMode(selectedAdventure, selectedMode, false))
      return;
    string requirementsDescription = (string) GameUtils.GetAdventureDataRecord((int) selectedAdventure, (int) selectedMode).RequirementsDescription;
    if (string.IsNullOrEmpty(requirementsDescription))
      return;
    Error.AddWarning(GameStrings.Get("GLUE_ADVENTURE_LOCKED"), requirementsDescription);
  }

  private void UpdateChooseButton(AdventureDbId adventureId, AdventureModeDbId modeId)
  {
    if (!this.m_AttemptedLoad && AdventureConfig.CanPlayMode(adventureId, modeId) && AdventureConfig.IsAdventureEventActive(adventureId))
    {
      this.m_ChooseButton.SetText(GameStrings.Get("GLOBAL_ADVENTURE_CHOOSE_BUTTON_TEXT"));
      if (this.m_ChooseButton.IsEnabled())
        return;
      this.m_ChooseButton.Enable();
    }
    else
    {
      this.m_ChooseButton.SetText(GameStrings.Get("GLUE_QUEST_LOG_CLASS_LOCKED"));
      this.m_ChooseButton.Disable();
    }
  }

  private void OnBoxTransitionFinished(object userData)
  {
    if (!this.m_isStarted || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
      return;
    if (this.m_ChooseButton.IsEnabled())
    {
      PlayMakerFSM component = this.m_ChooseButton.GetComponent<PlayMakerFSM>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.SendEvent("Burst");
    }
    else
      this.ShowDisabledAdventureModeRequirementsWarning();
    this.m_isTransitioning = false;
  }

  private void OnSubSceneTransitionFinished()
  {
    if (AdventureConfig.Get().CurrentSubScene != AdventureData.Adventuresubscene.CHOOSER || SceneMgr.Get().IsTransitioning())
      return;
    this.m_isTransitioning = false;
  }

  private void ChangeSubScene()
  {
    this.m_AttemptedLoad = true;
    this.m_ChooseButton.SetText(GameStrings.Get("GLUE_LOADING"));
    this.DisableAllButtons();
    this.StartCoroutine(this.WaitThenChangeSubScene());
  }

  private void DisableAllButtons()
  {
    this.m_ChooseButton.Disable();
    this.m_BackButton.Flip(false);
    this.m_BackButton.SetEnabled(false);
    foreach (PegUIElement chooserButton in this.m_ChooserButtons)
      chooserButton.SetEnabled(false);
  }

  private IEnumerator WaitThenChangeSubScene()
  {
    yield return (object) null;
    AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
  }

  private void OnAdventureProgressUpdated(
    bool isStartupAction,
    AdventureMission.WingProgress oldProgress,
    AdventureMission.WingProgress newProgress,
    object userData)
  {
    if (newProgress == null || oldProgress != null && oldProgress.IsOwned() || !newProgress.IsOwned() || GameDbf.Wing.GetRecord(newProgress.Wing) == null)
      return;
    foreach (ChooserButton chooserButton in this.m_ChooserButtons)
    {
      foreach (ChooserSubButton subButton in chooserButton.GetSubButtons())
      {
        AdventureChooserSubButton chooserSubButton = subButton as AdventureChooserSubButton;
        if ((UnityEngine.Object) chooserSubButton == (UnityEngine.Object) null)
        {
          Debug.LogErrorFormat("AdventureChooserTray: Button is either null or not of type AdventureChooserSubButton: {0}", (object) chooserSubButton);
        }
        else
        {
          chooserSubButton.ShowRemainingProgressCount();
          bool flag = AdventureConfig.CanPlayMode(chooserSubButton.GetAdventure(), chooserSubButton.GetMode());
          chooserSubButton.SetDesaturate(!flag);
        }
      }
    }
  }
}
