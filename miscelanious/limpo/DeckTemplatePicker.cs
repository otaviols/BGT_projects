using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.Core;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckTemplatePicker : MonoBehaviour
{
  public GameObject m_root;
  public GameObject m_pageHeader;
  public UberText m_pageHeaderText;
  public UIBObjectSpacing m_pickerButtonRoot;
  public DeckTemplatePickerButton m_pickerButtonTpl;
  public DeckTemplatePickerButton m_customDeckButton;
  public UberText m_deckTemplateDescription;
  public UberText m_deckTemplatePhoneName;
  public PlayButton m_chooseButton;
  public GameObject m_bottomPanel;
  public Material m_deckArtMaterial;
  public DeckTemplatePhoneTray m_phoneTray;
  public UIBButton m_phoneBackButton;
  public RuneIndicatorVisual m_runeIndicatorVisual;
  public Vector3 m_bottomPanelHideOffset = new Vector3(0.0f, 0.0f, 25f);
  public float m_bottomPanelSlideInWaitDelay = 0.25f;
  public float m_bottomPanelAnimateTime = 0.25f;
  public float m_packAnimInTime = 0.25f;
  public float m_packAnimOutTime = 0.2f;
  public Vector3 m_offscreenPackOffset;
  public Transform m_ghostCardTipBone;
  private List<DeckTemplatePickerButton> m_pickerButtons = new List<DeckTemplatePickerButton>();
  private CollectionManager.TemplateDeck m_customDeck = new CollectionManager.TemplateDeck();
  private TAG_CLASS m_currentSelectedClass;
  private FormatType m_currentSelectedFormat;
  private CollectionManager.TemplateDeck m_currentSelectedDeck;
  private List<DeckTemplatePicker.OnTemplateDeckChosen> m_templateDeckChosenListeners = new List<DeckTemplatePicker.OnTemplateDeckChosen>();
  private Vector3 m_origBottomPanelPos;
  private bool m_showingBottomPanel;
  private TransformProps m_customDeckInitialPosition;
  private bool m_packsShown;

  private void Awake()
  {
    this.m_currentSelectedDeck = this.m_customDeck;
    for (int index = 0; index < 3; ++index)
    {
      int idx = index;
      DeckTemplatePickerButton comp = (DeckTemplatePickerButton) GameUtils.Instantiate((Component) this.m_pickerButtonTpl, this.m_pickerButtonRoot.gameObject, true);
      Vector3 zero = Vector3.zero;
      if ((bool) UniversalInputManager.UsePhoneUI)
        zero.x = 0.75f;
      this.m_pickerButtonRoot.AddObject((Component) comp, zero);
      comp.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.SelectButtonWithIndex(idx)));
      comp.gameObject.SetActive(true);
      this.m_pickerButtons.Add(comp);
    }
    if (this.m_pickerButtons.Count > 0)
      this.m_pickerButtons[0].SetIsCoreDeck(true);
    this.m_pickerButtonRoot.UpdatePositions();
    this.m_pickerButtonTpl.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_customDeckButton != (UnityEngine.Object) null)
    {
      this.m_customDeckButton.gameObject.SetActive(true);
      this.m_customDeckButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.SelectCustomDeckButton()));
    }
    if ((UnityEngine.Object) this.m_chooseButton != (UnityEngine.Object) null)
    {
      this.m_chooseButton.Disable();
      this.m_chooseButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ChooseRecipeAndFillInCards()));
    }
    if ((UnityEngine.Object) this.m_phoneTray != (UnityEngine.Object) null)
    {
      this.m_phoneTray.m_scrollbar.SaveScroll("start");
      this.m_phoneTray.gameObject.SetActive(false);
    }
    if ((UnityEngine.Object) this.m_bottomPanel != (UnityEngine.Object) null)
      this.m_origBottomPanelPos = this.m_bottomPanel.transform.localPosition;
    if ((UnityEngine.Object) this.m_phoneBackButton != (UnityEngine.Object) null)
      this.m_phoneBackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBackButtonPressed(e)));
    this.m_customDeckInitialPosition = TransformUtil.GetLocalTransformProps((Component) this.m_customDeckButton.transform);
  }

  private void OnBackButtonPressed(UIEvent e) => Navigation.GoBack();

  private IEnumerator BackOut()
  {
    DeckTemplatePicker deckTemplatePicker = this;
    CollectionManager.Get().GetCollectibleDisplay().EnableInput(false);
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(((DeckTray) CollectionDeckTray.Get()).OnBackOutOfContainerContents));
    yield return (object) deckTemplatePicker.StartCoroutine(deckTemplatePicker.ShowPacks(false));
    CollectionDeckTray deckTray = CollectionDeckTray.Get();
    deckTray.OnBackOutOfDeckContentsImpl(true);
    while (!deckTray.m_cardsContent.HasFinishedExiting())
      yield return (object) null;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
    {
      collectibleDisplay.EnterSelectNewDeckHeroMode();
      HeroPickerDisplay heroPickerDisplay = collectibleDisplay.GetHeroPickerDisplay();
      while ((UnityEngine.Object) heroPickerDisplay != (UnityEngine.Object) null && !heroPickerDisplay.IsShown())
        yield return (object) null;
      heroPickerDisplay = (HeroPickerDisplay) null;
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      deckTemplatePicker.StartCoroutine(deckTemplatePicker.HideTrays());
    CollectionManager.Get().GetCollectibleDisplay().EnableInput(true);
  }

  public FormatType CurrentSelectedFormat => this.m_currentSelectedFormat;

  public bool OnNavigateBack()
  {
    this.StartCoroutine(this.BackOut());
    return true;
  }

  public void RegisterOnTemplateDeckChosen(DeckTemplatePicker.OnTemplateDeckChosen dlg) => this.m_templateDeckChosenListeners.Add(dlg);

  public void UnregisterOnTemplateDeckChosen(DeckTemplatePicker.OnTemplateDeckChosen dlg) => this.m_templateDeckChosenListeners.Remove(dlg);

  public bool IsShowingBottomPanel() => this.m_showingBottomPanel;

  public bool IsShowingPacks() => this.m_packsShown;

  public IEnumerator Show(bool show)
  {
    DeckTemplatePicker deckTemplatePicker = this;
    CollectionDeckTray deckTray = CollectionDeckTray.Get();
    CollectionManager collectionManager = CollectionManager.Get();
    DeckTrayCardListContent cardsContent = (DeckTrayCardListContent) null;
    CollectionDeck collectionDeck = (CollectionDeck) null;
    if ((UnityEngine.Object) deckTray != (UnityEngine.Object) null)
      cardsContent = deckTray.GetCardsContent();
    if (!((UnityEngine.Object) cardsContent == (UnityEngine.Object) null))
    {
      if (collectionManager != null)
        collectionDeck = collectionManager.GetEditedDeck();
      if (show)
      {
        deckTemplatePicker.m_root.SetActive(true);
        deckTemplatePicker.m_showingBottomPanel = false;
        deckTemplatePicker.m_packsShown = false;
        deckTemplatePicker.m_pickerButtonRoot.UpdatePositions();
        TransformUtil.CopyLocal((Component) deckTemplatePicker.m_customDeckButton.transform, deckTemplatePicker.m_customDeckInitialPosition);
        deckTemplatePicker.m_customDeckButton.GetComponentInChildren<UberText>().Text = GameStrings.Get(GameStrings.Get("GLUE_DECK_TEMPLATE_CUSTOM_DECK"));
        if (collectionDeck != null)
        {
          deckTemplatePicker.SetupTemplateButtons(deckTemplatePicker.m_customDeck);
          deckTemplatePicker.m_chooseButton.Disable();
          if ((UnityEngine.Object) deckTemplatePicker.m_deckTemplateDescription != (UnityEngine.Object) null)
            deckTemplatePicker.m_deckTemplateDescription.Text = GameStrings.Get("GLUE_COLLECTION_DECK_TEMPLATE_SELECT_A_DECK");
          cardsContent.ResetFakeDeck();
          if ((UnityEngine.Object) deckTemplatePicker.m_phoneTray != (UnityEngine.Object) null)
            deckTemplatePicker.m_phoneTray.m_cardsContent.ResetFakeDeck();
          deckTemplatePicker.FillWithCustomDeck();
          if (!(bool) UniversalInputManager.UsePhoneUI)
            deckTray.DisableRuneIndicatorVisualButtons();
          deckTemplatePicker.m_currentSelectedDeck = deckTemplatePicker.m_customDeck;
          if (!(bool) UniversalInputManager.UsePhoneUI)
            deckTemplatePicker.OnTrayToggled(true);
          Navigation.Push(new Navigation.NavigateBackHandler(deckTemplatePicker.OnNavigateBack));
          if (!CollectionManager.Get().ShouldShowDeckTemplatePageForClass(deckTemplatePicker.m_currentSelectedClass) && !(bool) UniversalInputManager.UsePhoneUI)
            deckTemplatePicker.SelectCustomDeckButton(true);
          deckTemplatePicker.ShowBottomPanel(true);
          yield return (object) deckTemplatePicker.StartCoroutine(deckTemplatePicker.ShowPacks(true));
          while ((UnityEngine.Object) deckTray == (UnityEngine.Object) null || deckTray.GetCurrentContentType() != DeckTray.DeckContentTypes.Cards)
            yield return (object) null;
        }
      }
      else if (deckTemplatePicker.m_root.activeSelf)
      {
        yield return (object) deckTemplatePicker.StartCoroutine(deckTemplatePicker.ShowPacks(false));
        cardsContent.ResetFakeDeck();
        deckTray.EnableRuneIndicatorVisualButtons();
        deckTemplatePicker.ShowBottomPanel(true);
        deckTemplatePicker.m_root.SetActive(false);
      }
    }
  }

  private void SetupTemplateButtons(CollectionManager.TemplateDeck refDeck)
  {
    List<CollectionManager.TemplateDeck> starterTemplateDecks = CollectionManager.Get().GetNonStarterTemplateDecks(this.m_currentSelectedFormat, this.m_currentSelectedClass);
    if (starterTemplateDecks == null)
    {
      Log.Decks.PrintWarning("SetupTemplateButtons with class {0} which had no template decks", (object) this.m_currentSelectedClass);
    }
    else
    {
      for (int index = 0; index < this.m_pickerButtons.Count && index < starterTemplateDecks.Count; ++index)
      {
        CollectionManager.TemplateDeck templateDeck = starterTemplateDecks[index];
        int num = refDeck == templateDeck ? 1 : 0;
        if (num != 0)
          this.m_currentSelectedDeck = templateDeck;
        this.m_pickerButtons[index].SetSelected(false);
        if (num != 0 && (UnityEngine.Object) this.m_deckTemplateDescription != (UnityEngine.Object) null)
          this.m_deckTemplateDescription.Text = templateDeck.m_description;
        if (num != 0 && (UnityEngine.Object) this.m_deckTemplatePhoneName != (UnityEngine.Object) null)
          this.m_deckTemplatePhoneName.Text = templateDeck.m_title;
        this.m_pickerButtons[index].transform.localEulerAngles = Vector3.zero;
        this.m_pickerButtons[index].GetComponent<RandomTransform>().Apply();
        AnimatedLowPolyPack component = this.m_pickerButtons[index].GetComponent<AnimatedLowPolyPack>();
        component.Init(0, this.m_pickerButtons[index].transform.localPosition, this.m_pickerButtons[index].transform.localPosition + this.m_offscreenPackOffset, false, false);
        component.SetFlyingLocalRotations(this.m_pickerButtons[index].transform.localEulerAngles, this.m_pickerButtons[index].transform.localEulerAngles);
      }
      if (!((UnityEngine.Object) this.m_customDeckButton != (UnityEngine.Object) null))
        return;
      this.m_customDeckButton.SetSelected(false);
      this.m_customDeckButton.transform.localEulerAngles = Vector3.zero;
      AnimatedLowPolyPack component1 = this.m_customDeckButton.GetComponent<AnimatedLowPolyPack>();
      component1.Init(0, this.m_customDeckButton.transform.localPosition, this.m_customDeckButton.transform.localPosition + this.m_offscreenPackOffset, false, false);
      component1.SetFlyingLocalRotations(this.m_customDeckButton.transform.localEulerAngles, this.m_customDeckButton.transform.localEulerAngles);
    }
  }

  public IEnumerator ShowPacks(bool show)
  {
    float delay = 0.0f;
    if (show)
    {
      CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
      if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      {
        HeroPickerDisplay heroPickerDisplay = collectibleDisplay.GetHeroPickerDisplay();
        while ((UnityEngine.Object) heroPickerDisplay != (UnityEngine.Object) null && !heroPickerDisplay.IsHidden())
          yield return (object) new WaitForEndOfFrame();
        heroPickerDisplay = (HeroPickerDisplay) null;
      }
    }
    DeckTemplatePickerButton[] array = this.m_pickerButtons.ToArray();
    GeneralUtils.Shuffle<DeckTemplatePickerButton>((IList<DeckTemplatePickerButton>) array);
    DeckTemplatePickerButton[] templatePickerButtonArray = array;
    for (int index = 0; index < templatePickerButtonArray.Length; ++index)
    {
      AnimatedLowPolyPack component = templatePickerButtonArray[index].GetComponent<AnimatedLowPolyPack>();
      if (show)
        component.FlyIn(this.m_packAnimInTime, delay);
      else
        component.FlyOut(this.m_packAnimOutTime, delay);
      yield return (object) new WaitForSeconds(UnityEngine.Random.Range(0.2f * this.m_packAnimInTime, 0.4f * this.m_packAnimInTime));
    }
    templatePickerButtonArray = (DeckTemplatePickerButton[]) null;
    AnimatedLowPolyPack component1 = this.m_customDeckButton.GetComponent<AnimatedLowPolyPack>();
    if (show)
    {
      component1.FlyIn(this.m_packAnimInTime, delay);
      yield return (object) new WaitForSeconds(this.m_packAnimInTime + delay);
    }
    else
    {
      component1.FlyOut(this.m_packAnimOutTime, delay);
      yield return (object) new WaitForSeconds(this.m_packAnimOutTime + delay);
    }
    this.m_packsShown = show;
  }

  public void ShowBottomPanel(bool show)
  {
    if (!((UnityEngine.Object) this.m_bottomPanel != (UnityEngine.Object) null))
      return;
    Vector3 origBottomPanelPos1 = this.m_origBottomPanelPos;
    Vector3 origBottomPanelPos2 = this.m_origBottomPanelPos;
    float num = 0.0f;
    if (show)
    {
      origBottomPanelPos2 += this.m_bottomPanelHideOffset;
      num = this.m_bottomPanelSlideInWaitDelay;
      this.m_showingBottomPanel = true;
    }
    else
    {
      origBottomPanelPos1 += this.m_bottomPanelHideOffset;
      Processor.ScheduleCallback(this.m_bottomPanelAnimateTime, false, (Processor.ScheduledCallback) (o => this.m_showingBottomPanel = show));
    }
    iTween.Stop(this.m_bottomPanel);
    this.m_bottomPanel.transform.localPosition = origBottomPanelPos2;
    iTween.MoveTo(this.m_bottomPanel, iTween.Hash((object) "position", (object) origBottomPanelPos1, (object) "isLocal", (object) true, (object) "time", (object) this.m_bottomPanelAnimateTime, (object) "delay", (object) num));
  }

  public void OnTrayToggled(bool shown)
  {
    if (shown)
      this.StartCoroutine(this.ShowTutorialPopup());
    else
      CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.CARDS, true);
  }

  private IEnumerator ShowTutorialPopup()
  {
    yield return (object) new WaitForSeconds(0.5f);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && !Options.Get().GetBool(Option.HAS_SEEN_DECK_TEMPLATE_SCREEN, false) && UserAttentionManager.CanShowAttentionGrabber("DeckTemplatePicker.ShowTutorialPopup:" + (object) Option.HAS_SEEN_DECK_TEMPLATE_SCREEN))
    {
      Transform tutorialWelcomeBone = collectibleDisplay.m_deckTemplateTutorialWelcomeBone;
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, tutorialWelcomeBone.localPosition, GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_WELCOME"), "VO_INNKEEPER_Male_Dwarf_RECIPE1_01.prefab:0261ef622a5e2b945a8f89e87cbe01a7", 3f);
      Options.Get().SetBool(Option.HAS_SEEN_DECK_TEMPLATE_SCREEN, true);
    }
  }

  public void SetDeckFormatAndClass(FormatType deckFormat, TAG_CLASS deckClass)
  {
    this.m_currentSelectedFormat = deckFormat;
    this.m_currentSelectedClass = deckClass;
    List<CollectionManager.TemplateDeck> starterTemplateDecks = CollectionManager.Get().GetNonStarterTemplateDecks(this.m_currentSelectedFormat, this.m_currentSelectedClass);
    int num1 = starterTemplateDecks != null ? starterTemplateDecks.Count : 0;
    Color color = CollectionPageManager.ColorForClass(deckClass);
    this.m_pageHeaderText.Text = GameStrings.Format("GLUE_DECK_TEMPLATE_CHOOSE_DECK", (object) GameStrings.GetClassName(deckClass));
    CollectionPageDisplay.SetPageFlavorTextures(this.m_pageHeader, CollectionPageDisplay.TagClassToHeaderClass(deckClass));
    for (int index = 0; index < this.m_pickerButtons.Count; ++index)
    {
      DeckTemplatePickerButton pickerButton = this.m_pickerButtons[index];
      bool flag = index < num1;
      pickerButton.gameObject.SetActive(flag);
      if (flag)
      {
        CollectionManager.TemplateDeck templateDeck = starterTemplateDecks[index];
        pickerButton.SetTitleText(templateDeck.m_title);
        int count = 0;
        int total = 0;
        foreach (KeyValuePair<string, int> cardId in templateDeck.m_cardIds)
        {
          int normal;
          int golden;
          int signature;
          int diamond;
          CollectionManager.Get().GetOwnedCardCount(cardId.Key, out normal, out golden, out signature, out diamond);
          int num2 = Mathf.Min(normal + golden + signature + diamond, cardId.Value);
          count += num2;
          total += cardId.Value;
        }
        pickerButton.SetCardCountText(count, total);
        pickerButton.m_packRibbon.GetMaterial().color = color;
        DeckTemplateDbfRecord record = GameDbf.DeckTemplate.GetRecord(templateDeck.m_deckTemplateId);
        if (record != null && record.DisplayCardId != 0)
          pickerButton.SetDeckArtByCardId(record.DisplayCardId, this.m_deckArtMaterial, record);
        else
          pickerButton.SetDeckArtByMaterialPath(templateDeck.m_displayTexture, record);
      }
    }
    if (!((UnityEngine.Object) this.m_customDeckButton != (UnityEngine.Object) null))
      return;
    this.m_customDeckButton.m_deckTexture.GetMaterial().mainTextureOffset = CollectionPageManager.s_classTextureOffsets[deckClass];
    this.m_customDeckButton.m_packRibbon.GetMaterial().color = color;
  }

  private void SelectButtonWithIndex(int index) => ((Action) (() =>
  {
    if ((UnityEngine.Object) this.m_chooseButton != (UnityEngine.Object) null)
      this.m_chooseButton.Enable();
    List<CollectionManager.TemplateDeck> starterTemplateDecks = CollectionManager.Get().GetNonStarterTemplateDecks(this.m_currentSelectedFormat, this.m_currentSelectedClass);
    CollectionManager.TemplateDeck customDeck = this.m_customDeck;
    if (starterTemplateDecks != null && index < starterTemplateDecks.Count)
      customDeck = starterTemplateDecks[index];
    for (int index1 = 0; index1 < this.m_pickerButtons.Count; ++index1)
      this.m_pickerButtons[index1].SetSelected(index1 == index);
    if ((UnityEngine.Object) this.m_deckTemplateDescription != (UnityEngine.Object) null)
      this.m_deckTemplateDescription.Text = customDeck.m_description;
    if ((UnityEngine.Object) this.m_deckTemplatePhoneName != (UnityEngine.Object) null)
      this.m_deckTemplatePhoneName.Text = customDeck.m_title;
    if ((UnityEngine.Object) this.m_customDeckButton != (UnityEngine.Object) null)
      this.m_customDeckButton.SetSelected(false);
    this.m_currentSelectedDeck = customDeck;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      SlidingTray component = this.m_phoneTray.GetComponent<SlidingTray>();
      if (component.TraySliderIsAnimating())
        return;
      this.m_phoneTray.gameObject.SetActive(true);
      component.ShowTray();
      this.m_phoneTray.m_scrollbar.LoadScroll("start", false);
      this.m_phoneTray.FlashDeckTemplateHighlight();
    }
    else
    {
      CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
      if ((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null)
        collectionDeckTray.FlashDeckTemplateHighlight();
    }
    this.FillDeckWithTemplate(this.m_currentSelectedDeck);
    this.StartCoroutine(this.ShowTips());
  }))();

  public IEnumerator ShowTips()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      while (this.m_phoneTray.GetComponent<SlidingTray>().TraySliderIsAnimating())
        yield return (object) null;
    }
  }

  private void FillDeckWithTemplate(CollectionManager.TemplateDeck tplDeck)
  {
    CollectionDeckTray deckTray = CollectionDeckTray.Get();
    if ((UnityEngine.Object) deckTray == (UnityEngine.Object) null)
    {
      Log.ErrorReporter.PrintError("DeckTemplatePicker::FillDeckWithTemplate deckTray is null!");
    }
    else
    {
      DeckTrayCardListContent cardsContent = deckTray.GetCardsContent();
      if ((UnityEngine.Object) cardsContent == (UnityEngine.Object) null)
      {
        Log.ErrorReporter.PrintError("DeckTemplatePicker::FillDeckWithTemplate cardListContent is null!");
      }
      else
      {
        CollectionDeck editingDeck1 = cardsContent.GetEditingDeck();
        if (editingDeck1 == null)
        {
          Log.ErrorReporter.PrintError("DeckTemplatePicker::FillDeckWithTemplate currentDeck is null!");
        }
        else
        {
          if (tplDeck == null)
          {
            CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
            editingDeck1.CopyFrom(editedDeck);
          }
          else
            editingDeck1.FillFromTemplateDeck(tplDeck);
          deckTray.m_cardsContent.UpdateCardList();
          deckTray.m_decksContent.UpdateDeckName();
          deckTray.InitializeRuneIndicatorVisual(editingDeck1);
          if (!((UnityEngine.Object) this.m_phoneTray != (UnityEngine.Object) null))
            return;
          CollectionDeck editingDeck2 = this.m_phoneTray.m_cardsContent.GetEditingDeck();
          if (tplDeck == null)
          {
            CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
            editingDeck2.CopyFrom(editedDeck);
          }
          else
            editingDeck2.FillFromTemplateDeck(tplDeck);
          if (editingDeck2.HasClass(TAG_CLASS.DEATHKNIGHT))
          {
            this.m_runeIndicatorVisual.Show();
            this.m_runeIndicatorVisual.Initialize(editingDeck2, deckTray);
            this.m_phoneTray.m_cardsContent.SetRuneIndicatorSpacerVisible(true);
            this.m_runeIndicatorVisual.DisableRuneButtons();
          }
          else
          {
            this.m_runeIndicatorVisual.Hide();
            this.m_phoneTray.m_cardsContent.SetRuneIndicatorSpacerVisible(false);
          }
          this.m_phoneTray.m_cardsContent.UpdateCardList();
          LayerUtils.SetLayer((Component) this.m_phoneTray, GameLayer.IgnoreFullScreenEffects);
        }
      }
    }
  }

  private void FillWithCustomDeck() => this.FillDeckWithTemplate((CollectionManager.TemplateDeck) null);

  private void FireOnTemplateDeckChosenEvent()
  {
    foreach (DeckTemplatePicker.OnTemplateDeckChosen templateDeckChosen in this.m_templateDeckChosenListeners.ToArray())
      templateDeckChosen();
  }

  private IEnumerator HideTrays()
  {
    DeckTemplatePicker deckTemplatePicker = this;
    SlidingTray phoneTray = deckTemplatePicker.m_phoneTray.GetComponent<SlidingTray>();
    phoneTray.HideTray();
    while (phoneTray.isActiveAndEnabled && !phoneTray.IsTrayInShownPosition())
      yield return (object) new WaitForEndOfFrame();
    deckTemplatePicker.GetComponent<SlidingTray>().HideTray();
  }

  private void ChooseRecipeAndFillInCards()
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if ((UnityEngine.Object) collectionDeckTray == (UnityEngine.Object) null)
    {
      Log.ErrorReporter.PrintError("DeckTemplatePicker::ChooseRecipeAndFillInCards deckTray is null!");
    }
    else
    {
      DeckTrayCardListContent cardsContent = collectionDeckTray.GetCardsContent();
      if ((UnityEngine.Object) cardsContent == (UnityEngine.Object) null)
      {
        Log.ErrorReporter.PrintError("DeckTemplatePicker::ChooseRecipeAndFillInCards cardListContent is null!");
      }
      else
      {
        CollectionManager collectionManager = CollectionManager.Get();
        if (collectionManager == null)
        {
          Log.ErrorReporter.PrintError("DeckTemplatePicker::ChooseRecipeAndFillInCards collectionManager is null!");
        }
        else
        {
          cardsContent.CommitFakeDeckChanges();
          collectionManager.SetShowDeckTemplatePageForClass(this.m_currentSelectedClass, this.m_currentSelectedDeck != this.m_customDeck);
          this.FireOnTemplateDeckChosenEvent();
          CollectionDeck editedDeck = collectionManager.GetEditedDeck();
          collectionDeckTray.InitializeRuneIndicatorVisual(editedDeck);
          if (this.m_currentSelectedDeck != this.m_customDeck)
          {
            editedDeck.SourceType = DeckSourceType.DECK_SOURCE_TYPE_TEMPLATE;
            Network.Get().SetDeckTemplateSource(editedDeck.ID, this.m_currentSelectedDeck.m_id);
          }
          Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
          if ((bool) UniversalInputManager.UsePhoneUI)
            this.StartCoroutine(this.EnterDeckPhone());
          CollectionManagerDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay() as CollectionManagerDisplay;
          if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectionManager.ShouldShowWildToStandardTutorial() && editedDeck.FormatType == FormatType.FT_STANDARD)
            collectibleDisplay.ShowStandardInfoTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
          if (!editedDeck.HasClass(TAG_CLASS.DEATHKNIGHT))
            return;
          TutorialDeathKnightDeckBuilding.ShowTutorial(UIVoiceLinesManager.TriggerType.STARTED_EDITING_DEATH_KNIGHT_DECK);
        }
      }
    }
  }

  private void SelectCustomDeckButton(bool preselect = false)
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if ((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null && !preselect)
      collectionDeckTray.FlashDeckTemplateHighlight();
    if ((UnityEngine.Object) this.m_chooseButton != (UnityEngine.Object) null)
      this.m_chooseButton.Enable();
    for (int index = 0; index < this.m_pickerButtons.Count; ++index)
      this.m_pickerButtons[index].SetSelected(false);
    if ((UnityEngine.Object) this.m_customDeckButton != (UnityEngine.Object) null)
      this.m_customDeckButton.SetSelected(true);
    if ((UnityEngine.Object) this.m_deckTemplateDescription != (UnityEngine.Object) null)
      this.m_deckTemplateDescription.Text = GameStrings.Get("GLUE_DECK_TEMPLATE_CUSTOM_DECK_DESCRIPTION");
    this.FillWithCustomDeck();
    this.m_currentSelectedDeck = this.m_customDeck;
    if (!(bool) UniversalInputManager.UsePhoneUI || preselect)
      return;
    this.ChooseRecipeAndFillInCards();
  }

  public IEnumerator EnterDeckPhone()
  {
    DeckTemplatePicker deckTemplatePicker = this;
    yield return (object) deckTemplatePicker.StartCoroutine(deckTemplatePicker.ShowPacks(false));
    yield return (object) deckTemplatePicker.StartCoroutine(deckTemplatePicker.HideTrays());
  }

  public delegate void OnTemplateDeckChosen();
}
