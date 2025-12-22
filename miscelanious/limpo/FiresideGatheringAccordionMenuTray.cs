using PegasusShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class FiresideGatheringAccordionMenuTray : AccordionMenuTray
{
  [CustomEditField(Sections = "Chooser Button", T = EditType.TEXTURE)]
  public string m_BrawlsButtonTexture;
  [CustomEditField(Sections = "Chooser Buttons")]
  public UnityEngine.Vector2 m_BrawlsTextureTiling;
  [CustomEditField(Sections = "Chooser Buttons")]
  public UnityEngine.Vector2 m_BrawlsTextureOffset;
  [CustomEditField(Sections = "Chooser Button", T = EditType.TEXTURE)]
  public string m_DuelsButtonTexture;
  [CustomEditField(Sections = "Chooser Buttons")]
  public UnityEngine.Vector2 m_DuelsTextureTiling;
  [CustomEditField(Sections = "Chooser Buttons")]
  public UnityEngine.Vector2 m_DuelsTextureOffset;
  [CustomEditField(Sections = "Prefab Bones")]
  public GameObject m_DeckPickerTrayContainer;
  [CustomEditField(Sections = "Choose Frame")]
  public FiresideGatheringPlayButtonLantern m_FiresideGatheringPlayButtonLantern;
  private FiresideGatheringManager.FiresideGatheringMode m_selectedMode;
  private FormatType m_selectedFormatType = FormatType.FT_STANDARD;
  private FiresideGatheringChooserButton m_brawlsButton;
  private FiresideGatheringChooserButton m_duelsButton;

  private void Awake()
  {
    this.m_ChooseButton.Disable();
    this.m_BackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBackButton()));
    this.m_ChooseButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.GoToSelectedMode()));
    if ((Object) this.m_ChooseFrameScroller == (Object) null || (Object) this.m_ChooseFrameScroller.ScrollObject == (Object) null)
    {
      Debug.LogError((object) "m_ChooseFrameScroller or m_ChooseFrameScroller.m_ScrollObject cannot be null. Unable to create button.", (Object) this);
    }
    else
    {
      this.CreateChooserButtons();
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
      this.OnButtonVisualUpdated();
      Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
      this.m_FiresideGatheringPlayButtonLantern.gameObject.SetActive(false);
    }
  }

  private void OnDestroy()
  {
    if (!((Object) Box.Get() != (Object) null))
      return;
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
  }

  private void Start()
  {
    Navigation.PushUnique(new Navigation.NavigateBackHandler(FiresideGatheringAccordionMenuTray.OnNavigateBack));
    this.m_isStarted = true;
  }

  private void OnBackButton() => Navigation.GoBack();

  private static bool OnNavigateBack()
  {
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    FiresideGatheringManager.Get().CurrentFiresideGatheringMode = FiresideGatheringManager.FiresideGatheringMode.NONE;
    FiresideGatheringManager.Get().ShowReturnToFSGSceneTooltip();
    return true;
  }

  private void GoToSelectedMode() => this.GoToSpecifiedModeAutomatically(this.m_selectedMode, this.m_selectedFormatType);

  public void GoToSpecifiedModeAutomatically(
    FiresideGatheringManager.FiresideGatheringMode newMode,
    FormatType newFormatType)
  {
    this.m_ChooseButton.SetText(GameStrings.Get("GLUE_LOADING"));
    this.m_ChooseButton.Disable();
    this.m_BackButton.SetEnabled(false);
    this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(false);
    this.StartCoroutine(this.WaitThenGoToSelectedMode(newMode, newFormatType));
  }

  private IEnumerator WaitThenGoToSelectedMode(
    FiresideGatheringManager.FiresideGatheringMode newMode,
    FormatType newFormatType)
  {
    FiresideGatheringAccordionMenuTray accordionMenuTray = this;
    yield return (object) null;
    FiresideGatheringManager.Get().CurrentFiresideGatheringMode = newMode;
    if (newMode == FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE_BRAWL || newMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL)
      TavernBrawlManager.Get().CurrentBrawlType = newMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL ? BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING : BrawlType.BRAWL_TYPE_TAVERN_BRAWL;
    if (newMode == FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE)
    {
      Options.SetFormatType(newFormatType);
      FiresideGatheringDisplay.Get().ShowDeckPickerTray();
      Navigation.Push(new Navigation.NavigateBackHandler(accordionMenuTray.OnFriendlyBackButtonReleased));
    }
    else
    {
      FiresideGatheringDisplay.Get().ShowTavernBrawlTray();
      Navigation.Push(new Navigation.NavigateBackHandler(accordionMenuTray.OnTavernBrawlBackButtonReleased));
    }
  }

  private bool OnFriendlyBackButtonReleased()
  {
    FiresideGatheringDisplay.Get().HideDeckPickerTray();
    this.ReenableButtons();
    return true;
  }

  private bool OnTavernBrawlBackButtonReleased()
  {
    FiresideGatheringDisplay.Get().HideTavernBrawlTray();
    this.ReenableButtons();
    return true;
  }

  private void ReenableButtons()
  {
    if ((Object) this.m_SelectedSubButton != (Object) null)
    {
      this.m_ChooseButton.SetText(GameStrings.Get("GLOBAL_ADVENTURE_CHOOSE_BUTTON_TEXT"));
      this.m_ChooseButton.Enable();
      this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(true);
    }
    else
    {
      this.m_ChooseButton.Disable();
      this.m_ChooseButton.SetText(string.Empty);
      this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(false);
    }
    this.m_BackButton.SetEnabled(true);
  }

  private void OnBoxTransitionFinished(object userData)
  {
    if (!this.m_isStarted || SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING || !this.m_ChooseButton.IsEnabled())
      return;
    PlayMakerFSM component = this.m_ChooseButton.GetComponent<PlayMakerFSM>();
    if (!((Object) component != (Object) null))
      return;
    component.SendEvent("Burst");
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    this.CreateFiresideBrawlSubButtons(this.m_brawlsButton);
    this.m_brawlsButton.Toggle = true;
  }

  private void CreateChooserButtons()
  {
    this.m_brawlsButton = GameUtils.LoadGameObjectWithComponent<FiresideGatheringChooserButton>(this.m_DefaultChooserButtonPrefab);
    this.m_duelsButton = GameUtils.LoadGameObjectWithComponent<FiresideGatheringChooserButton>(this.m_DefaultChooserButtonPrefab);
    if ((Object) this.m_brawlsButton == (Object) null || (Object) this.m_duelsButton == (Object) null)
      return;
    GameUtils.SetParent((Component) this.m_brawlsButton, this.m_ChooseFrameScroller.ScrollObject);
    GameUtils.SetParent((Component) this.m_duelsButton, this.m_ChooseFrameScroller.ScrollObject);
    this.m_brawlsButton.SetButtonText(GameStrings.Get("GLUE_FIRESIDE_GATHERING_BRAWL"));
    this.m_brawlsButton.SetPortraitTexture(this.m_BrawlsButtonTexture);
    this.m_brawlsButton.SetPortraitTiling(this.m_BrawlsTextureTiling);
    this.m_brawlsButton.SetPortraitOffset(this.m_BrawlsTextureOffset);
    this.m_brawlsButton.ShowLantern();
    this.m_duelsButton.SetButtonText(GameStrings.Get("GLUE_FIRESIDE_GATHERING_DUELS_BUTTON"));
    this.m_duelsButton.SetPortraitTexture(this.m_DuelsButtonTexture);
    this.m_duelsButton.SetPortraitTiling(this.m_DuelsTextureTiling);
    this.m_duelsButton.SetPortraitOffset(this.m_DuelsTextureOffset);
    this.m_duelsButton.ShowSwords();
    this.CreateFriendlyDuelSubButtons(this.m_duelsButton);
    this.m_brawlsButton.AddVisualUpdatedListener(new ChooserButton.VisualUpdated(((AccordionMenuTray) this).OnButtonVisualUpdated));
    this.m_duelsButton.AddVisualUpdatedListener(new ChooserButton.VisualUpdated(((AccordionMenuTray) this).OnButtonVisualUpdated));
    this.m_brawlsButton.Toggle = false;
    this.m_duelsButton.Toggle = false;
    this.m_brawlsButton.AddToggleListener((ChooserButton.Toggled) (toggle =>
    {
      this.OnChooserButtonToggled((ChooserButton) this.m_brawlsButton, toggle, 0);
      this.m_ChooseButton.Disable();
      this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(false);
    }));
    this.m_brawlsButton.AddModeSelectionListener(new ChooserButton.ModeSelection(this.ButtonModeSelected));
    this.m_brawlsButton.AddExpandedListener(new ChooserButton.Expanded(this.ButtonExpanded));
    this.m_ChooserButtons.Add((ChooserButton) this.m_brawlsButton);
    this.m_duelsButton.AddToggleListener((ChooserButton.Toggled) (toggle =>
    {
      this.OnChooserButtonToggled((ChooserButton) this.m_duelsButton, toggle, 1);
      this.m_ChooseButton.Disable();
      this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(false);
    }));
    this.m_duelsButton.AddModeSelectionListener(new ChooserButton.ModeSelection(this.ButtonModeSelected));
    this.m_duelsButton.AddExpandedListener(new ChooserButton.Expanded(this.ButtonExpanded));
    this.m_ChooserButtons.Add((ChooserButton) this.m_duelsButton);
  }

  protected void ButtonExpanded(ChooserButton button, bool expand)
  {
    if (!expand)
      return;
    this.ToggleScrollable(true);
  }

  private void ButtonModeSelected(ChooserSubButton btn)
  {
    this.m_SelectedSubButton = btn;
    FiresideGatheringChooserSubButton chooserSubButton = (FiresideGatheringChooserSubButton) btn;
    if (chooserSubButton.AssociatedFormatType == FormatType.FT_UNKNOWN)
    {
      RankMgr.LogMessage("fsgButton.AssociatedFormatType == FT_UNKOWN", nameof (ButtonModeSelected), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Game\\FiresideGathering\\FiresideGatheringAccordionMenuTray.cs", 297);
    }
    else
    {
      this.m_selectedFormatType = chooserSubButton.AssociatedFormatType;
      this.m_selectedMode = chooserSubButton.AssociatedMode;
      TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING)?.SetSelectedBrawlLibraryItemId(chooserSubButton.AssociatedBrawlLibraryItemId);
      this.OnSelectedModeChanged();
    }
  }

  private void CreateFiresideBrawlSubButtons(FiresideGatheringChooserButton brawlsButton)
  {
    TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING);
    if (mission == null)
      return;
    List<GameContentScenario> currentFsgBrawls = FiresideGatheringManager.Get().CurrentFsgBrawls;
    for (int index = 0; index < currentFsgBrawls.Count; ++index)
    {
      GameContentScenario gameContentScenario = currentFsgBrawls[index];
      bool useAsLastSelected = gameContentScenario.LibraryItemId == mission.SelectedBrawlLibraryItemId;
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(gameContentScenario.ScenarioId);
      string str = record != null ? (string) record.Name : GameStrings.Get("GLUE_TOOLTIP_BUTTON_TAVERN_BRAWL_HEADLINE");
      FiresideGatheringChooserSubButton subButton = brawlsButton.CreateSubButton(this.m_DefaultChooserSubButtonPrefab, useAsLastSelected);
      subButton.SetButtonText(str);
      subButton.SetOfficialBrawlRotationIcon(false);
      subButton.AssociatedFormatType = gameContentScenario.FormatType;
      subButton.AssociatedMode = FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL;
      subButton.AssociatedBrawlLibraryItemId = gameContentScenario.LibraryItemId;
      subButton.SetMaterialFromButtonIndex(0);
    }
  }

  private void CreateFriendlyDuelSubButtons(FiresideGatheringChooserButton duelsButton)
  {
    int num1 = 0;
    FiresideGatheringChooserSubButton subButton1 = duelsButton.CreateSubButton(this.m_DefaultChooserSubButtonPrefab, false);
    subButton1.SetButtonText(GameStrings.Get("GLUE_COLLECTION_DECK_FORMAT_STANDARD"));
    subButton1.AssociatedFormatType = FormatType.FT_STANDARD;
    subButton1.AssociatedMode = FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE;
    subButton1.AssociatedBrawlLibraryItemId = 0;
    int index1 = num1;
    int num2 = index1 + 1;
    subButton1.SetMaterialFromButtonIndex(index1);
    if (CollectionManager.Get().ShouldAccountSeeStandardWild())
    {
      FiresideGatheringChooserSubButton subButton2 = duelsButton.CreateSubButton(this.m_DefaultChooserSubButtonPrefab, false);
      subButton2.SetButtonText(GameStrings.Get("GLUE_COLLECTION_DECK_FORMAT_WILD"));
      subButton2.AssociatedFormatType = FormatType.FT_WILD;
      subButton2.AssociatedMode = FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE;
      subButton2.AssociatedBrawlLibraryItemId = 0;
      subButton2.SetMaterialFromButtonIndex(num2++);
    }
    if (!TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
      return;
    FiresideGatheringChooserSubButton subButton3 = duelsButton.CreateSubButton(this.m_DefaultChooserSubButtonPrefab, false);
    subButton3.SetButtonText(GameStrings.Get("GLOBAL_TAVERN_BRAWL"));
    subButton3.AssociatedFormatType = FormatType.FT_UNKNOWN;
    subButton3.AssociatedMode = FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE_BRAWL;
    subButton3.AssociatedBrawlLibraryItemId = 0;
    int index2 = num2;
    int num3 = index2 + 1;
    subButton3.SetMaterialFromButtonIndex(index2);
  }

  private void OnSelectedModeChanged()
  {
    this.m_ChooseButton.Enable();
    this.m_ChooseButton.SetText(GameStrings.Get("GLOBAL_ADVENTURE_CHOOSE_BUTTON_TEXT"));
    this.m_FiresideGatheringPlayButtonLantern.gameObject.SetActive(this.m_selectedMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL);
    this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(true);
    if (!this.m_ChooseButton.IsEnabled())
      return;
    PlayMakerFSM component = this.m_ChooseButton.GetComponent<PlayMakerFSM>();
    if (!((Object) component != (Object) null))
      return;
    component.SendEvent("Burst");
  }
}
