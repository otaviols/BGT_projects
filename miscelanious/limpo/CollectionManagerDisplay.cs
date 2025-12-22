using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CustomEditClass]
public class CollectionManagerDisplay : CollectibleDisplay
{
  [CustomEditField(Sections = "Bones")]
  public Transform m_deckTemplateHiddenBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_deckTemplateShownBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_deckTemplateTutorialWelcomeBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_deckTemplateTutorialReminderBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_editDeckTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_convertDeckTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_setFilterTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_cardBackDeckTrayTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_multipleFavoriteCardBackTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_multipleFavoriteHeroTutorialBone;
  [CustomEditField(Sections = "Objects")]
  public CollectionPageManager m_pageManager;
  [CustomEditField(Sections = "Objects")]
  public ManaFilterTabManager m_manaTabManager;
  [CustomEditField(Sections = "Objects")]
  public Notification m_deckTemplateCardReplacePopup;
  [CustomEditField(Sections = "Objects")]
  public NestedPrefab m_setFilterTrayContainer;
  [CustomEditField(Sections = "Objects")]
  public GameObject m_runeLockedCheckboxContainer;
  [CustomEditField(Sections = "Objects")]
  public CheckBox m_runelockedCheckbox;
  [CustomEditField(Sections = "Objects")]
  public UberText m_runeLockedCheckboxLabel;
  [CustomEditField(Sections = "Controls")]
  public Texture m_allSetsTexture;
  [CustomEditField(Sections = "Controls")]
  public UnityEngine.Vector2 m_allSetsIconOffset;
  [CustomEditField(Sections = "Controls")]
  public Texture m_wildSetsTexture;
  [CustomEditField(Sections = "Controls")]
  public UnityEngine.Vector2 m_wildSetsIconOffset;
  [CustomEditField(Sections = "Controls")]
  public Texture m_featuredCardsTexture;
  [CustomEditField(Sections = "Controls")]
  public UnityEngine.Vector2 m_featuredCardsIconOffset;
  [CustomEditField(Sections = "Controls")]
  public Texture m_classicSetsTexture;
  [CustomEditField(Sections = "Controls")]
  public UnityEngine.Vector2 m_classicSetsIconOffset;
  [CustomEditField(Sections = "CM Customization Ref")]
  public GameObject m_bookBack;
  [FormerlySerializedAs("m_tavernBrawlBookBackMesh")]
  [CustomEditField(Sections = "CM Customization Ref")]
  public Mesh m_customBookBackMesh;
  [FormerlySerializedAs("m_tavernBrawlObjectsToSwap")]
  [CustomEditField(Sections = "CM Customization Ref")]
  public List<GameObject> m_customObjectsToSwap = new List<GameObject>();
  [CustomEditField(Sections = "Tavern Brawl Changes", T = EditType.TEXTURE)]
  [FormerlySerializedAs("m_corkBackTexture")]
  public string m_tbCorkBackTexture;
  [CustomEditField(Sections = "Tavern Brawl Changes")]
  public Material m_tavernBrawlElements;
  [CustomEditField(Sections = "Duels Changes", T = EditType.TEXTURE)]
  public string m_duelsCorkBackTexture;
  [CustomEditField(Sections = "Duels Changes")]
  public Material m_duelsElements;
  private Map<TAG_CLASS, AssetHandle<Texture>> m_loadedClassTextures = new Map<TAG_CLASS, AssetHandle<Texture>>();
  private AssetHandle<Texture> m_loadedCorkBackTexture;
  private bool m_selectingNewDeckHero;
  private long m_showDeckContentsRequest;
  private bool m_shouldShowMultipleFavoriteCardBackTutorial;
  private bool m_shouldShowMultipleFavoriteHeroTutorial;
  private Notification m_deckHelpPopup;
  private Notification m_innkeeperLClickReminder;
  private List<CollectibleDisplay.FilterStateListener> m_setFilterListeners = new List<CollectibleDisplay.FilterStateListener>();
  private List<CollectibleDisplay.FilterStateListener> m_manaFilterListeners = new List<CollectibleDisplay.FilterStateListener>();
  private DeckTemplatePicker m_deckTemplatePickerPhone;
  private HeroPickerDisplay m_heroPickerDisplay;
  private Notification m_createDeckNotification;
  private Notification m_multipleFavoriteCardBacksNotification;
  private Notification m_multipleFavoriteHeroesNotification;
  private Notification m_convertTutorialPopup;
  private IEnumerator m_showConvertTutorialCoroutine;
  private Notification m_setFilterTutorialPopup;
  private IEnumerator m_showSetFilterTutorialCoroutine;
  private bool m_showingDeckTemplateTips;
  private float m_deckTemplateTipWaitTime;
  private bool m_manaFilterIsFromSearchText;
  private ShareableDeck m_cachedShareableDeck;
  private SpecialEventType m_currentActiveFeaturedCardsEvent;
  protected bool m_viewModeHidingCraftingTray;
  private TAG_CLASS? m_heroSkinClass;

  public static event Action<bool> HideLockedRunesCheckboxToggled;

  public override void Start()
  {
    NetCache.Get().RegisterScreenCollectionManager(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    CollectionManager.Get().RegisterCollectionNetHandlers();
    CollectionManager.Get().RegisterCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(((CollectibleDisplay) this).OnCollectionLoaded));
    CollectionManager.Get().RegisterCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(((CollectibleDisplay) this).OnCollectionChanged));
    CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedByPlayer));
    CollectionManager.Get().RegisterDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
    CollectionManager.Get().RegisterNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
    CollectionManager.Get().RegisterCardRewardsInsertedListener(new CollectionManager.DelOnCardRewardsInserted(this.OnCardRewardsInserted));
    CardBackManager.Get().SetSearchText((string) null);
    CoinManager.Get().SetSearchText((string) null);
    this.m_shouldShowMultipleFavoriteCardBackTutorial = Network.IsLoggedIn() && !GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_MULTIPLE_FAVORITE_CARD_BACKS);
    this.m_shouldShowMultipleFavoriteHeroTutorial = Network.IsLoggedIn() && !GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_MULTIPLE_FAVORITE_HEROES);
    base.Start();
    if ((UnityEngine.Object) this.m_setFilterTrayContainer != (UnityEngine.Object) null)
    {
      this.m_setFilterTray = this.m_setFilterTrayContainer.PrefabGameObject(true).GetComponentInChildren<SetFilterTray>(true);
      this.m_setFilterTray.m_toggleButton.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.OnSetFilterButtonPressed()));
    }
    if ((UnityEngine.Object) this.m_filterButton != (UnityEngine.Object) null)
    {
      if (CollectionManagerDisplay.ShouldSeeFilterButton())
        this.m_filterButton.m_inactiveFilterButton.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.OnPhoneFilterButtonPressed()));
      else
        this.m_filterButton.gameObject.SetActive(false);
    }
    bool show = Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false);
    this.ShowAdvancedCollectionManager(show);
    if (!show)
      Options.Get().RegisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
    this.DoEnterCollectionManagerEvents();
    if (!CollectionManagerDisplay.IsSpecialOneDeckMode())
      MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_CollectionManager);
    if (CollectionManager.Get().ShouldShowWildToStandardTutorial())
      UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
    this.SetTavernBrawlTexturesIfNecessary();
    this.SetDuelsTexturesIfNecessary();
    CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded();
    this.StartCoroutine(this.WaitUntilReady());
  }

  protected override void Awake()
  {
    HearthstonePerformance hearthstonePerformance = HearthstonePerformance.Get();
    if (hearthstonePerformance != null)
      hearthstonePerformance.StartPerformanceFlow(new FlowPerformance.SetupConfig()
      {
        FlowType = Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType.COLLECTION_MANAGER
      });
    this.m_manaTabManager.OnFilterCleared += new Action<bool>(this.ManaFilterTab_OnManaFilterCleared);
    this.m_manaTabManager.OnManaValueActivated += new Action<int, bool>(this.ManaFilterTab_OnManaValueActivated);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_deckTemplatePickerPhone = AssetLoader.Get().InstantiatePrefab((AssetReference) "DeckTemplate_phone.prefab:a8a8fbcd170064edfb0eeac3f836a13b").GetComponent<DeckTemplatePicker>();
      SlidingTray component = this.m_deckTemplatePickerPhone.GetComponent<SlidingTray>();
      component.m_trayHiddenBone = this.m_deckTemplateHiddenBone.transform;
      component.m_trayShownBone = this.m_deckTemplateShownBone.transform;
    }
    CollectionManager.Get().HasSeenOvercappedDeckInfoPopup = false;
    CollectionManager.Get().HasSeenExtraRunesDeckInfoPopup = false;
    base.Awake();
    this.StartCoroutine(this.InitCollectionWhenReady());
  }

  private void OnEnable()
  {
    if ((bool) (UnityEngine.Object) this.m_runelockedCheckbox)
    {
      this.m_runelockedCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleHideRuneCards));
      this.m_runelockedCheckbox.SetChecked(true);
    }
    if (!(bool) (UnityEngine.Object) this.m_runeLockedCheckboxLabel)
      return;
    this.m_runeLockedCheckboxLabel.Text = GameStrings.Get("GLOBAL_DEATHKNIGHT_RUNE_LOCKED");
  }

  private void OnDisable()
  {
    if (!(bool) (UnityEngine.Object) this.m_runelockedCheckbox)
      return;
    this.m_runelockedCheckbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleHideRuneCards));
  }

  private void ToggleHideRuneCards(UIEvent e)
  {
    bool flag = this.m_runelockedCheckbox.IsChecked();
    Action<bool> runesCheckboxToggled = CollectionManagerDisplay.HideLockedRunesCheckboxToggled;
    if (runesCheckboxToggled == null)
      return;
    runesCheckboxToggled(flag);
  }

  protected override void OnDestroy()
  {
    this.m_manaTabManager.OnFilterCleared -= new Action<bool>(this.ManaFilterTab_OnManaFilterCleared);
    this.m_manaTabManager.OnManaValueActivated -= new Action<int, bool>(this.ManaFilterTab_OnManaValueActivated);
    AssetHandle.SafeDispose<Texture>(ref this.m_loadedCorkBackTexture);
    this.m_loadedClassTextures.DisposeValuesAndClear<TAG_CLASS, AssetHandle<Texture>>();
    if ((UnityEngine.Object) this.m_deckTemplatePickerPhone != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_deckTemplatePickerPhone.gameObject);
      this.m_deckTemplatePickerPhone = (DeckTemplatePicker) null;
    }
    UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
    base.OnDestroy();
  }

  private void Update()
  {
    if (HearthstoneApplication.IsInternal() && !UniversalInputManager.Get().IsTextInputActive())
    {
      if (InputCollection.GetKeyDown(KeyCode.Alpha1))
        this.SetViewMode(CollectionUtils.ViewMode.HERO_SKINS);
      else if (InputCollection.GetKeyDown(KeyCode.Alpha2))
        this.SetViewMode(CollectionUtils.ViewMode.CARDS);
      else if (InputCollection.GetKeyDown(KeyCode.Alpha3))
        this.SetViewMode(CollectionUtils.ViewMode.CARD_BACKS);
      else if (InputCollection.GetKeyDown(KeyCode.Alpha4))
        this.SetViewMode(CollectionUtils.ViewMode.DECK_TEMPLATE);
      else if (InputCollection.GetKeyDown(KeyCode.Alpha4))
        this.OnCraftingModeButtonReleased((UIEvent) null);
    }
    this.ShowDeckTemplateTipsIfNeeded();
  }

  private void OnApplicationFocus(bool hasFocus)
  {
    if (!hasFocus)
      return;
    this.StartCoroutine(this.OnApplicationFocusCoroutine());
  }

  private IEnumerator OnApplicationFocusCoroutine()
  {
    yield return (object) null;
    this.CheckClipboardAndPromptPlayerToPaste();
  }

  public override CollectiblePageManager GetPageManager() => (CollectiblePageManager) this.m_pageManager;

  public override void Unload()
  {
    this.m_unloading = true;
    NotificationManager.Get().DestroyAllPopUps();
    this.UnloadAllTextures();
    CollectionDeckTray.Get().GetCardsContent().UnregisterCardTileRightClickedListener(new DeckTrayCardListContent.CardTileRightClicked(this.OnCardTileRightClicked));
    CollectionDeckTray.Get().Unload();
    CollectionInputMgr.Get().Unload();
    CollectionManager.Get().RemoveCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(((CollectibleDisplay) this).OnCollectionLoaded));
    CollectionManager.Get().RemoveCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(((CollectibleDisplay) this).OnCollectionChanged));
    CollectionManager.Get().RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedByPlayer));
    CollectionManager.Get().RemoveDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
    CollectionManager.Get().RemoveNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
    CollectionManager.Get().RemoveCardRewardsInsertedListener(new CollectionManager.DelOnCardRewardsInserted(this.OnCardRewardsInserted));
    CollectionManager.Get().RemoveCollectionNetHandlers();
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    Options.Get().UnregisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
    this.m_unloading = false;
  }

  public override void Exit()
  {
    this.EnableInput(false);
    NotificationManager.Get().DestroyAllPopUps();
    CollectionDeckTray.Get().Exit();
    if ((UnityEngine.Object) this.m_pageManager != (UnityEngine.Object) null)
      this.m_pageManager.Exit();
    CraftingManager.Get().SetCraftingUIActive(false);
    SceneMgr.Mode mode = SceneMgr.Get().GetPrevMode();
    if (mode == SceneMgr.Mode.GAMEPLAY)
      mode = SceneMgr.Mode.HUB;
    if (!Network.IsLoggedIn() && mode != SceneMgr.Mode.HUB)
    {
      DialogManager.Get().ShowReconnectHelperDialog();
      mode = SceneMgr.Mode.HUB;
      Navigation.Clear();
    }
    HearthstonePerformance.Get()?.StopCurrentFlow();
    SceneMgr.TransitionHandlerType transitionHandler = SceneMgr.TransitionHandlerType.SCENEMGR;
    if (SceneMgr.Get().IsInTavernBrawlMode() && mode == SceneMgr.Mode.GAME_MODE)
      transitionHandler = SceneMgr.TransitionHandlerType.NEXT_SCENE;
    SceneMgr.Get().SetNextMode(mode, transitionHandler);
  }

  public override void CollectionPageContentsChanged<TCollectible>(
    ICollection<TCollectible> collectiblesToDisplay,
    CollectibleDisplay.CollectionActorsReadyCallback callback,
    object callbackData)
  {
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1}", (object) this.m_pageManager.GetTransitionPageId(), (object) this.m_pageManager.ArePagesTurning());
    bool flag1 = false;
    if (collectiblesToDisplay == null)
    {
      Log.CollectionManager.Print("artStacksToDisplay is null!");
      flag1 = true;
    }
    else if (collectiblesToDisplay.Count == 0)
    {
      Log.CollectionManager.Print("artStacksToDisplay has a count of 0!");
      flag1 = true;
    }
    if (this.m_unloading)
      return;
    this.ClearCardActors();
    if (flag1)
    {
      if (callback == null)
        return;
      callback(new List<CollectionCardActors>(), new List<ICollectible>(), callbackData);
    }
    else
    {
      long arcaneDustBalance = NetCache.Get().GetArcaneDustBalance();
      DefLoader defLoader = DefLoader.Get();
      List<ICollectible> nonActorCollectibles = new List<ICollectible>();
      foreach (TCollectible collectible1 in (IEnumerable<TCollectible>) collectiblesToDisplay)
      {
        ICollectible collectible2 = (ICollectible) collectible1;
        if (!(collectible2 is CollectibleCard collectibleCard))
        {
          nonActorCollectibles.Add(collectible2);
        }
        else
        {
          string cardId = collectibleCard.CardId;
          EntityDef entityDef = defLoader.GetEntityDef(cardId);
          TAG_PREMIUM premiumType = collectibleCard.PremiumType;
          bool flag2 = entityDef.IsHeroSkin();
          string assetRef = flag2 ? ActorNames.GetHeroSkinOrHandActor(entityDef, premiumType) : ActorNames.GetHandActor(entityDef, premiumType);
          GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
          if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
          {
            Debug.LogError((object) "Unable to load card actor.");
          }
          else
          {
            Actor component = gameObject.GetComponent<Actor>();
            if ((UnityEngine.Object) component == (UnityEngine.Object) null)
            {
              Debug.LogError((object) "Actor object does not contain Actor component.");
            }
            else
            {
              component.SetEntityDef(entityDef);
              component.SetPremium(premiumType);
              component.CreateBannedRibbon();
              if (collectibleCard.OwnedCount == 0)
              {
                if (collectibleCard.IsCraftable && arcaneDustBalance >= (long) collectibleCard.CraftBuyCost)
                  component.GhostCardEffect(GhostCard.Type.MISSING, premiumType, false);
                else if (flag2 && HeroSkinUtils.CanBuyHeroSkinFromCollectionManager(entityDef.GetCardId()))
                  component.GhostCardEffect(GhostCard.Type.PURCHASABLE_HERO_SKIN, premiumType, false);
                else
                  component.MissingCardEffect(updateComponents: false);
              }
              component.UpdateAllComponents(false);
              this.m_cardActors.Add(new CollectionCardActors(component));
            }
          }
        }
      }
      if (callback == null)
        return;
      callback(this.m_cardActors, nonActorCollectibles, callbackData);
    }
  }

  public void CollectionPageContentsChangedToCardBacks(
    List<CardBackManager.OwnedCardBack> cardBacksToDisplay,
    CollectibleDisplay.CollectionActorsReadyCallback callback)
  {
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1}", (object) this.m_pageManager.GetTransitionPageId(), (object) this.m_pageManager.ArePagesTurning());
    List<CollectionCardActors> result = new List<CollectionCardActors>();
    this.ClearCardActors();
    if (cardBacksToDisplay.Count == 0)
    {
      if (callback == null)
        return;
      callback(result, (List<ICollectible>) null, (object) null);
    }
    else
    {
      int numCardBacksToLoad = cardBacksToDisplay.Count;
      Action<int, CardBackManager.OwnedCardBack, Actor> cbLoadedCallback = (Action<int, CardBackManager.OwnedCardBack, Actor>) ((index, cardBack, actor) =>
      {
        if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
        {
          result[index] = new CollectionCardActors(actor);
          actor.SetCardbackUpdateIgnore(true);
          CollectionCardBack component = actor.GetComponent<CollectionCardBack>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            component.SetCardBackId(cardBack.m_cardBackId);
            component.SetCardBackName(CardBackManager.Get().GetCardBackName(cardBack.m_cardBackId));
          }
          else
            Debug.LogError((object) "CollectionCardBack component does not exist on actor!");
          if (!cardBack.m_owned)
          {
            if (cardBack.m_canBuy)
              actor.GhostCardEffect(GhostCard.Type.MISSING);
            else
              actor.MissingCardEffect();
          }
        }
        --numCardBacksToLoad;
        if (numCardBacksToLoad != 0 || callback == null)
          return;
        callback(result, (List<ICollectible>) null, (object) null);
      });
      for (int index = 0; index < cardBacksToDisplay.Count; ++index)
      {
        int currIndex = index;
        CardBackManager.OwnedCardBack cardBackLoad = cardBacksToDisplay[index];
        int cardBackId = cardBackLoad.m_cardBackId;
        result.Add((CollectionCardActors) null);
        CardBackManager cardBackManager = CardBackManager.Get();
        if ((cardBackManager == null ? 0 : (cardBackManager.LoadCardBackByIndex(cardBackId, (CardBackManager.LoadCardBackData.LoadCardBackCallback) (cardBackData =>
        {
          GameObject gameObject1 = cardBackData.m_GameObject;
          gameObject1.transform.parent = this.transform;
          GameObject gameObject2 = gameObject1;
          gameObject2.name = gameObject2.name + "_" + (object) cardBackData.m_CardBackIndex;
          Actor component = gameObject1.GetComponent<Actor>();
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) gameObject1);
          }
          else
          {
            GameObject cardMesh = component.m_cardMesh;
            component.SetCardbackUpdateIgnore(true);
            component.SetUnlit();
            if ((UnityEngine.Object) cardMesh != (UnityEngine.Object) null)
            {
              Material material = RendererExtension.GetMaterial(cardMesh.GetComponent<Renderer>());
              if (material.HasProperty("_SpecularIntensity"))
                material.SetFloat("_SpecularIntensity", 0.0f);
            }
            this.m_cardActors.Add(new CollectionCardActors(component));
          }
          cbLoadedCallback(currIndex, cardBackLoad, component);
        }), "Collection_Card_Back.prefab:a208f592a46e4f447b3026e82444177e", (object) null) ? 1 : 0)) == 0)
          cbLoadedCallback(currIndex, cardBackLoad, (Actor) null);
      }
    }
  }

  public void RequestContentsToShowDeck(long deckID)
  {
    this.m_showDeckContentsRequest = deckID;
    CollectionManager.Get().RequestDeckContents(this.m_showDeckContentsRequest);
  }

  public void ShowPhoneDeckTemplateTray()
  {
    this.m_pageManager.UpdateDeckTemplate(this.m_deckTemplatePickerPhone);
    SlidingTray component = this.m_deckTemplatePickerPhone.GetComponent<SlidingTray>();
    component.RegisterTrayToggleListener(new SlidingTray.TrayToggledListener(this.m_deckTemplatePickerPhone.OnTrayToggled));
    component.ShowTray();
  }

  public DeckTemplatePicker GetPhoneDeckTemplateTray() => this.m_deckTemplatePickerPhone;

  public override void SetViewMode(
    CollectionUtils.ViewMode mode,
    bool triggerResponse,
    CollectionUtils.ViewModeData userdata = null)
  {
    Log.CollectionManager.Print("mode={0}-->{1} triggerResponse={2} isUpdatingTrayMode={3}", (object) this.m_currentViewMode, (object) mode, (object) triggerResponse, (object) CollectionDeckTray.Get().IsUpdatingTrayMode());
    if (this.m_currentViewMode == mode || CollectionDeckTray.Get().IsUpdatingTrayMode() && (mode == CollectionUtils.ViewMode.HERO_SKINS || mode == CollectionUtils.ViewMode.CARD_BACKS || mode == CollectionUtils.ViewMode.COINS))
      return;
    if (mode == CollectionUtils.ViewMode.DECK_TEMPLATE)
    {
      if (!CollectionManager.Get().IsInEditMode() || SceneMgr.Get().IsInTavernBrawlMode())
        return;
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.ShowPhoneDeckTemplateTray();
    }
    CollectionUtils.ViewMode currentViewMode = this.m_currentViewMode;
    this.m_currentViewMode = mode;
    this.OnSwitchViewModeResponse(triggerResponse, currentViewMode, mode, userdata);
  }

  public bool ViewModeHasVisibleDeckList() => this.m_currentViewMode != CollectionUtils.ViewMode.DECK_TEMPLATE && this.m_currentViewMode != CollectionUtils.ViewMode.MASS_DISENCHANT;

  public void OnDoneEditingDeck()
  {
    this.ShowAppropriateSetFilters();
    if (this.m_currentViewMode == CollectionUtils.ViewMode.DECK_TEMPLATE)
      this.SetViewMode(CollectionUtils.ViewMode.CARDS, false, (CollectionUtils.ViewModeData) null);
    if (!SceneMgr.Get().IsInTavernBrawlMode())
      this.m_pageManager.SetDeckRuleset((DeckRuleset) null);
    FiresideGatheringManager.Get().UpdateDeckValidity();
    this.SetRuneLockedCheckboxVisible(false);
    this.m_pageManager.OnDoneEditingDeck();
  }

  public bool IsManaFilterEvenValues => this.m_manaTabManager.IsFilterEvenValues;

  public bool IsManaFilterOddValues => this.m_manaTabManager.IsFilterOddValues;

  private void ManaFilterTab_OnManaFilterCleared(bool transitionPage)
  {
    this.ManaFilterTab_OnManaValueActivated(-1, transitionPage);
    this.m_manaFilterIsFromSearchText = false;
  }

  public void ManaFilterTab_OnManaValueActivated(int cost, bool transitionPage)
  {
    if (this.m_manaFilterIsFromSearchText)
      this.RemoveManaTokenFromSearchText(false);
    this.NotifyFilterUpdate(this.m_manaFilterListeners, this.m_manaTabManager.IsManaValueActive(cost), cost < 7 ? (object) cost.ToString() : (object) (cost.ToString() + "+"));
    this.m_pageManager.FilterByManaCost(cost, transitionPage);
  }

  public override void FilterBySearchText(string newSearchText)
  {
    string text = this.m_search.GetText();
    base.FilterBySearchText(newSearchText);
    this.OnSearchDeactivated_Internal(text, newSearchText, true);
  }

  private void RemoveManaTokenFromSearchText(bool updateManaFilterToMatchSearchText)
  {
    string text = this.m_search.GetText();
    if (string.IsNullOrEmpty(text))
      return;
    string[] source = text.Split(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries);
    if (source.Length == 0)
      return;
    bool hasManaToken = false;
    Func<string, bool> isManaToken = this.GetIsManaSearchTokenFunc();
    string[] array = ((IEnumerable<string>) source).Where<string>((Func<string, bool>) (t =>
    {
      if (!isManaToken(t))
        return true;
      hasManaToken = true;
      return false;
    })).ToArray<string>();
    if (!hasManaToken)
      return;
    this.m_search.SetText(string.Join(new string(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters[0], 1), array));
    this.OnSearchDeactivated_Internal(text, this.m_search.GetText(), updateManaFilterToMatchSearchText);
  }

  private void UpdateManaFilterToMatchSearchText(string searchText, bool transitionPage = true)
  {
    if (string.IsNullOrEmpty(searchText) || !this.m_manaTabManager.Enabled)
    {
      this.m_manaTabManager.ClearFilter(transitionPage);
    }
    else
    {
      Func<string, bool> manaSearchTokenFunc = this.GetIsManaSearchTokenFunc();
      string str1 = ((IEnumerable<string>) searchText.Split(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries)).FirstOrDefault<string>(manaSearchTokenFunc);
      if (str1 != null)
      {
        if (this.m_pageManager.IsManaCostFilterActive)
          this.m_pageManager.FilterByManaCost(-1, transitionPage);
        string val = str1.Split(CollectibleFilteredSet<ICollectible>.SearchTagColons, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        bool isNumericalValue;
        int minVal;
        int maxVal;
        GeneralUtils.ParseNumericRange(val, out isNumericalValue, out minVal, out maxVal);
        string str2 = (string) null;
        if (isNumericalValue)
        {
          this.m_manaTabManager.SetFilter_Range(minVal, maxVal);
          str2 = val;
        }
        else
        {
          string lower1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EVEN_MANA").ToLower();
          string lower2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ODD_MANA").ToLower();
          string lower3 = val.ToLower();
          bool flag = lower3 == lower1;
          bool isOdd = !flag && lower3 == lower2;
          if (isOdd | flag)
          {
            this.m_manaTabManager.SetFilter_EvenOdd(isOdd);
            str2 = CollectibleCardFilter.CreateSearchTerm_Mana_OddEven(isOdd);
          }
        }
        if (str2 == null)
          return;
        this.m_manaFilterIsFromSearchText = true;
        this.NotifyFilterUpdate(this.m_manaFilterListeners, true, (object) str2);
      }
      else
        this.m_manaTabManager.ClearFilter(transitionPage);
    }
  }

  private Func<string, bool> GetIsManaSearchTokenFunc()
  {
    string manaToken = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MANA").ToLower();
    string evenTokenValue = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EVEN_MANA").ToLower();
    string oddTokenValue = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ODD_MANA").ToLower();
    return (Func<string, bool>) (token =>
    {
      string[] strArray = token.Split(CollectibleFilteredSet<ICollectible>.SearchTagColons, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length >= 2 && strArray[0].Trim().ToLower() == manaToken)
      {
        string val = strArray[1].Trim();
        bool isNumericalValue;
        GeneralUtils.ParseNumericRange(val, out isNumericalValue, out int _, out int _);
        if (isNumericalValue)
          return true;
        string lower = val.ToLower();
        if (lower == oddTokenValue || lower == evenTokenValue)
          return true;
      }
      return false;
    });
  }

  public override void HideAllTips()
  {
    if ((UnityEngine.Object) this.m_innkeeperLClickReminder != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_innkeeperLClickReminder);
    this.HideDeckHelpPopup();
    if ((UnityEngine.Object) this.m_convertTutorialPopup != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_convertTutorialPopup);
    if ((UnityEngine.Object) this.m_createDeckNotification != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_createDeckNotification);
    if (!((UnityEngine.Object) this.m_multipleFavoriteCardBacksNotification != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_multipleFavoriteCardBacksNotification);
  }

  public void HideDeckHelpPopup()
  {
    if ((UnityEngine.Object) this.m_deckHelpPopup != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_deckHelpPopup);
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if (!((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null))
      return;
    collectionDeckTray.GetCardsContent()?.HideDeckHelpPopup();
    collectionDeckTray.GetCardBackContent()?.HideTutorials();
  }

  public override void ShowInnkeeperLClickHelp(EntityDef entityDef) => this.ShowInnkeeperLClickHelp(entityDef != null && entityDef.IsHeroSkin());

  private void ShowInnkeeperLClickHelp(bool isHero)
  {
    if (CollectionDeckTray.Get().IsShowingDeckContents())
      return;
    if (isHero)
      this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK_HERO"), "", 3f);
    else
      this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK"), "", 3f);
  }

  public void ShowPremiumCardsNotOwned(bool show) => this.m_pageManager.ShowCardsNotOwned(show);

  private void FeaturedCardsSetFilterCallback(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    PegasusShared.FormatType formatType,
    SetFilterItem item,
    bool transitionPage)
  {
    this.SetLastSeenFeaturedCardsEvent(this.m_currentActiveFeaturedCardsEvent, GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_ITEM);
    item.SetIconFxActive(false);
    this.SetFilterCallback(cardSets, specificCards, formatType, item, transitionPage);
  }

  public override void SetFilterCallback(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    PegasusShared.FormatType formatType,
    SetFilterItem item,
    bool transitionPage)
  {
    bool active = formatType == PegasusShared.FormatType.FT_WILD;
    if (active && !CollectionManager.Get().AccountHasUnlockedWild() && !SceneMgr.Get().IsInDuelsMode() && !SceneMgr.Get().IsInTavernBrawlMode())
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_COLLECTION_SET_FILTER_WILD_SET_HEADER"),
        m_text = GameStrings.Get("GLUE_COLLECTION_SET_FILTER_WILD_SET_BODY"),
        m_cancelText = GameStrings.Get("GLOBAL_CANCEL"),
        m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response == AlertPopup.Response.CONFIRM)
            this.ShowSetFilterCards(cardSets, specificCards, transitionPage);
          else
            this.m_setFilterTray.SelectPreviouslySelectedItem();
        })
      };
      DialogManager.Get().ShowPopup(info);
    }
    else
    {
      this.m_search.SetWildModeActive(active);
      this.ShowSetFilterCards(cardSets, specificCards, transitionPage);
    }
  }

  private void ShowSetFilterCards(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    bool transitionPage = true)
  {
    if (specificCards != null)
      this.ShowSpecificCards(specificCards);
    else
      this.ShowSets(cardSets, transitionPage);
  }

  private void ShowSets(List<TAG_CARD_SET> cardSets, bool transitionPage = true)
  {
    this.m_pageManager.FilterByCardSets(cardSets, transitionPage);
    this.NotifyFilterUpdate(this.m_setFilterListeners, cardSets != null, (object) null);
  }

  protected override void ShowSpecificCards(List<int> specificCards)
  {
    base.ShowSpecificCards(specificCards);
    this.NotifyFilterUpdate(this.m_setFilterListeners, specificCards != null, (object) null);
  }

  public HeroPickerDisplay GetHeroPickerDisplay() => this.m_heroPickerDisplay;

  public void EnterSelectNewDeckHeroMode()
  {
    if (this.m_selectingNewDeckHero)
      return;
    this.EnableInput(false);
    this.m_selectingNewDeckHero = true;
    this.m_heroPickerDisplay = AssetLoader.Get().InstantiatePrefab((AssetReference) "HeroPicker.prefab:59e2d2f899d09f4488a194df18967915").GetComponent<HeroPickerDisplay>();
    NotificationManager.Get().DestroyAllPopUps();
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
      this.m_pageManager.HideNonDeckTemplateTabs(true);
    this.CheckClipboardAndPromptPlayerToPaste();
  }

  public void ExitSelectNewDeckHeroMode() => this.m_selectingNewDeckHero = false;

  public void CancelSelectNewDeckHeroMode()
  {
    this.EnableInput(true);
    this.m_pageManager.HideNonDeckTemplateTabs(false, true);
    this.ExitSelectNewDeckHeroMode();
  }

  public bool CanViewHeroSkins()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    return editedDeck == null || collectionManager.GetCountOfOwnedHeroesForClass(editedDeck.GetClass()) > 1;
  }

  public bool CanViewCardBacks() => CardBackManager.Get().GetCardBacksOwned().Count > 1;

  public bool CanViewCoins()
  {
    if (CollectionManager.Get().GetEditedDeck() != null)
      return false;
    HashSet<int> coinsOwned = CoinManager.Get().GetCoinsOwned();
    // ISSUE: explicit non-virtual call
    return (coinsOwned != null ? __nonvirtual (coinsOwned.Count) : 0) > 1;
  }

  public void RegisterManaFilterListener(CollectibleDisplay.FilterStateListener listener) => this.m_manaFilterListeners.Add(listener);

  public void UnregisterManaFilterListener(CollectibleDisplay.FilterStateListener listener) => this.m_manaFilterListeners.Remove(listener);

  public void RegisterSetFilterListener(CollectibleDisplay.FilterStateListener listener) => this.m_setFilterListeners.Add(listener);

  public void UnregisterSetFilterListener(CollectibleDisplay.FilterStateListener listener) => this.m_setFilterListeners.Remove(listener);

  public override void ResetFilters(bool updateVisuals = true)
  {
    if ((UnityEngine.Object) this.m_manaTabManager != (UnityEngine.Object) null)
      this.m_manaTabManager.ClearFilter(false);
    if ((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null)
      this.m_setFilterTray.ClearFilter(false);
    base.ResetFilters(updateVisuals);
  }

  public void ShowAppropriateSetFilters()
  {
    bool showUnownedSets = this.InCraftingMode();
    bool editingDeck = CollectionManager.Get().IsInEditMode();
    PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_STANDARD;
    if (editingDeck)
      formatType = CollectionManager.Get().GetEditedDeck().FormatType;
    else if (RankMgr.Get().WildCardsAllowedInCurrentLeague())
    {
      if (CollectionManager.Get().ShouldAccountSeeStandardWild() | showUnownedSets)
        formatType = PegasusShared.FormatType.FT_WILD;
    }
    else if (showUnownedSets)
      formatType = PegasusShared.FormatType.FT_STANDARD;
    else if (CollectionManager.Get().AccountHasUnlockedWild())
      formatType = PegasusShared.FormatType.FT_WILD;
    this.UpdateSetFilters(formatType, editingDeck, showUnownedSets);
  }

  public void UpdateSetFilters(PegasusShared.FormatType formatType, bool editingDeck, bool showUnownedSets = false) => this.m_setFilterTray.UpdateSetFilters(formatType, editingDeck, showUnownedSets);

  public ActiveFilterButton GetFilterButton() => this.m_filterButton;

  public void HideFilterTrayOnStartDragCard()
  {
    if (!this.IsShowingSetFilterTray())
      return;
    this.m_filterButton.m_setFilterTray.ToggleTraySlider(false);
  }

  public void UnhideFilterTrayOnStopDragCard()
  {
    if (!this.IsShowingSetFilterTray())
      return;
    this.m_filterButton.m_setFilterTray.ToggleTraySlider(true);
  }

  public void WaitThenUnhideFilterTrayOnStopDragCard()
  {
    if (!this.IsShowingSetFilterTray())
      return;
    this.StartCoroutine(this.WaitThenUnhideFilterTrayOnStopDragCard_Coroutine());
  }

  private IEnumerator WaitThenUnhideFilterTrayOnStopDragCard_Coroutine()
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    yield return (object) new WaitForSeconds(0.5f);
    if ((UnityEngine.Object) (CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay) != (UnityEngine.Object) null && collectionManagerDisplay.IsShowingSetFilterTray() && (UnityEngine.Object) CollectionInputMgr.Get() != (UnityEngine.Object) null && !CollectionInputMgr.Get().HasHeldCard())
      collectionManagerDisplay.m_filterButton.m_setFilterTray.ToggleTraySlider(true);
  }

  public bool SetFilterIsDefaultSelection() => (UnityEngine.Object) this.m_setFilterTray == (UnityEngine.Object) null || !this.m_setFilterTray.HasActiveFilter();

  public bool IsShowingSetFilterTray() => !((UnityEngine.Object) this.m_setFilterTray == (UnityEngine.Object) null) && this.m_setFilterTray.IsShown();

  public bool IsSelectingNewDeckHero() => this.m_selectingNewDeckHero;

  private void OnDeckContents(long deckID)
  {
    if (deckID == this.m_showDeckContentsRequest)
    {
      this.m_showDeckContentsRequest = 0L;
      this.ShowDeck(deckID, false, false);
    }
    else
      CollectionDeckTray.Get().GetDecksContent().OnDeckContentsUpdated(deckID);
  }

  private void OnDeckCreatedByPlayer(long deckID, string name)
  {
    bool showDeckTemplatePage = false;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
    {
      CollectionManager collectionManager = CollectionManager.Get();
      if (collectionManager == null)
      {
        Debug.LogError((object) "CollectionManagerDisplay.OnDeckCreatedByPlayer: CollectionManager.Get() returned null");
        return;
      }
      CollectionDeck deck = collectionManager.GetDeck(deckID);
      if (deck == null)
      {
        Debug.LogError((object) ("CollectionManagerDisplay.OnDeckCreatedByPlayer: Could not get deck " + deckID.ToString()));
        return;
      }
      if (CollectionManager.Get().GetNonStarterTemplateDecks(deck.FormatType, deck.GetClass()).Count > 0)
        showDeckTemplatePage = true;
    }
    this.ShowDeck(deckID, true, showDeckTemplatePage);
  }

  private void OnNewCardSeen(string cardID, TAG_PREMIUM premium) => this.m_pageManager.UpdateClassTabNewCardCounts();

  private void OnCardRewardsInserted(List<string> cardID, List<TAG_PREMIUM> premium) => this.m_pageManager.RefreshCurrentPageContents();

  protected override void OnCollectionChanged()
  {
    if (this.m_currentViewMode == CollectionUtils.ViewMode.MASS_DISENCHANT)
      return;
    this.m_pageManager.NotifyOfCollectionChanged();
  }

  private void ClearCardActors()
  {
    foreach (CollectionCardActors previousCardActor in this.m_previousCardActors)
      previousCardActor.Destroy();
    this.m_previousCardActors.Clear();
    this.m_previousCardActors = this.m_cardActors;
    this.m_cardActors = new List<CollectionCardActors>();
  }

  private IEnumerator WaitUntilReady()
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    while (!collectionManagerDisplay.m_netCacheReady && Network.IsLoggedIn())
      yield return (object) 0;
    if (SceneMgr.Get().IsInDuelsMode())
    {
      while (!CollectionManager.Get().IsDuelsSessionInfoLoaded())
        yield return (object) null;
    }
    collectionManagerDisplay.InitDeckTray();
  }

  private void InitDeckTray()
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    collectionDeckTray.Initialize();
    collectionDeckTray.RegisterModeSwitchedListener((DeckTray.ModeSwitched) (() => this.UpdateCurrentPageCardLocks()));
    collectionDeckTray.GetCardsContent().RegisterCardTileRightClickedListener(new DeckTrayCardListContent.CardTileRightClicked(this.OnCardTileRightClicked));
    this.m_isReady = true;
  }

  private IEnumerator InitCollectionWhenReady()
  {
    while (!this.m_pageManager.IsFullyLoaded())
      yield return (object) null;
    this.m_pageManager.LoadMassDisenchantScreen();
    this.m_pageManager.OnCollectionLoaded();
  }

  protected override bool ShouldStartShown() => !(bool) UniversalInputManager.UsePhoneUI && base.ShouldStartShown();

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Collection.Manager)
    {
      if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        return;
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_COLLECTION");
    }
    else
      this.m_netCacheReady = true;
  }

  private void OnShowAdvancedCMChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    bool show = Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false);
    if (show)
      Options.Get().UnregisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
    this.ShowAdvancedCollectionManager(show);
    this.m_manaTabManager.ActivateTabs(true);
  }

  private void OnCardTileRightClicked(DeckTrayDeckTileVisual cardTile)
  {
    if (this.GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE)
      return;
    if (!cardTile.GetSlot().Owned && !DuelsConfig.IsCardLoadoutTreasure(cardTile.GetCardID()))
      CraftingManager.Get().EnterCraftMode((Actor) cardTile.GetActor());
    this.GoToPageWithCard(cardTile.GetCardID(), cardTile.GetPremium());
  }

  protected override void LoadAllTextures()
  {
    foreach (TAG_CLASS tagClass in Enum.GetValues(typeof (TAG_CLASS)))
    {
      string textureAssetPath = CollectionManagerDisplay.GetClassTextureAssetPath(tagClass);
      if (!string.IsNullOrEmpty(textureAssetPath))
        AssetLoader.Get().LoadAsset<Texture>((AssetReference) textureAssetPath, new AssetHandleCallback<Texture>(this.OnClassTextureLoaded), (object) tagClass);
    }
  }

  protected override void UnloadAllTextures() => this.m_loadedClassTextures.DisposeValuesAndClear<TAG_CLASS, AssetHandle<Texture>>();

  public static string GetClassTextureAssetPath(TAG_CLASS classTag)
  {
    switch (classTag)
    {
      case TAG_CLASS.DRUID:
        return "Druid.psd:e2417dc1394f54349956b2e24a27f923";
      case TAG_CLASS.HUNTER:
        return "Hunter.psd:16178c8d6ed14814dae893bad9de80d5";
      case TAG_CLASS.MAGE:
        return "Mage.psd:8dcb9bd578b6c01448cf1021c6157dfd";
      case TAG_CLASS.PALADIN:
        return "Paladin.psd:50ba8fc595684d440866ac130c146d57";
      case TAG_CLASS.PRIEST:
        return "Priest.psd:5fa4606c71c0dff4eb0b07b88ba83197";
      case TAG_CLASS.ROGUE:
        return "Rogue.psd:47dc46a5269d7fc4a8a9ebada8f2d890";
      case TAG_CLASS.SHAMAN:
        return "Shaman.psd:2e468e3b0f7a7804a9335333c9e673e2";
      case TAG_CLASS.WARLOCK:
        return "Warlock.psd:d6077adee4894df43a67617620de56a9";
      case TAG_CLASS.WARRIOR:
        return "Warrior.psd:5376d479d4155ca419f8afa5e42ba505";
      default:
        return "";
    }
  }

  private void SetTavernBrawlTexturesIfNecessary()
  {
    if (!SceneMgr.Get().IsInTavernBrawlMode())
      return;
    if ((UnityEngine.Object) this.m_bookBack != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.m_tbCorkBackTexture) && (UnityEngine.Object) this.m_customBookBackMesh != (UnityEngine.Object) null)
    {
      this.m_bookBack.GetComponent<MeshFilter>().mesh = this.m_customBookBackMesh;
      AssetLoader.Get().LoadAsset<Texture>(ref this.m_loadedCorkBackTexture, (AssetReference) this.m_tbCorkBackTexture);
      RendererExtension.GetMaterial((Renderer) this.m_bookBack.GetComponent<MeshRenderer>()).SetTexture("_MainTex", (Texture) this.m_loadedCorkBackTexture);
      this.m_setFilterTray.m_toggleButton.SetButtonBackgroundMaterial();
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    foreach (GameObject gameObject in this.m_customObjectsToSwap)
    {
      Renderer component = gameObject.GetComponent<Renderer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        RendererExtension.SetMaterial(component, this.m_tavernBrawlElements);
      else
        Debug.LogErrorFormat("Failed to swap material for TavernBrawl object: {0}", (object) gameObject.name);
    }
  }

  private void SetDuelsTexturesIfNecessary()
  {
    if (!SceneMgr.Get().IsInDuelsMode())
      return;
    if ((UnityEngine.Object) this.m_bookBack != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.m_duelsCorkBackTexture) && (UnityEngine.Object) this.m_customBookBackMesh != (UnityEngine.Object) null)
    {
      this.m_bookBack.GetComponent<MeshFilter>().mesh = this.m_customBookBackMesh;
      AssetLoader.Get().LoadAsset<Texture>(ref this.m_loadedCorkBackTexture, (AssetReference) this.m_duelsCorkBackTexture);
      RendererExtension.GetMaterial((Renderer) this.m_bookBack.GetComponent<MeshRenderer>()).SetTexture("_MainTex", (Texture) this.m_loadedCorkBackTexture);
      this.m_setFilterTray.m_toggleButton.SetButtonBackgroundMaterial();
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    foreach (GameObject gameObject in this.m_customObjectsToSwap)
    {
      Renderer component = gameObject.GetComponent<Renderer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        RendererExtension.SetMaterial(component, this.m_duelsElements);
      else
        Debug.LogErrorFormat("Failed to swap material for TavernBrawl object: {0}", (object) gameObject.name);
    }
  }

  private void OnClassTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> loadedTexture,
    object callbackData)
  {
    if (loadedTexture == null)
      Debug.LogWarning((object) string.Format("CollectionManagerDisplay.OnClassTextureLoaded(): asset for {0} is null!", (object) assetRef));
    else
      this.m_loadedClassTextures.SetOrReplaceDisposable<TAG_CLASS, AssetHandle<Texture>>((TAG_CLASS) callbackData, loadedTexture);
  }

  public void ShowCurrentEditedDeck()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return;
    List<TAG_CLASS> classes = editedDeck.GetClasses();
    this.ShowDeckHelper(editedDeck, classes, false, false);
  }

  public void ShowDeck(
    long deckID,
    bool isNewDeck,
    bool showDeckTemplatePage,
    CollectionUtils.ViewMode? setNewViewMode = null)
  {
    CollectionDeck deck = CollectionManager.Get().GetDeck(deckID);
    if (deck == null)
      return;
    List<TAG_CLASS> deckHeroClasses = this.GetDeckHeroClasses(deckID);
    this.ShowDeckHelper(deck, deckHeroClasses, isNewDeck, showDeckTemplatePage, setNewViewMode);
    if (showDeckTemplatePage)
      return;
    this.SetRuneLockedCheckboxVisible(GameUtils.HasClassTag(TAG_CLASS.DEATHKNIGHT, deckHeroClasses));
  }

  private void ShowDeckHelper(
    CollectionDeck currDeck,
    List<TAG_CLASS> deckClasses,
    bool isNewDeck,
    bool showDeckTemplatePage,
    CollectionUtils.ViewMode? setNewViewMode = null)
  {
    if (currDeck.HasUIHeroOverride() && this.m_currentViewMode == CollectionUtils.ViewMode.HERO_SKINS)
      this.m_pageManager.JumpToCollectionClassPage(deckClasses.First<TAG_CLASS>());
    if (!showDeckTemplatePage)
      this.m_pageManager.HideNonDeckTemplateTabs(false);
    CollectionManager.Get().StartEditingDeck(currDeck, (object) isNewDeck);
    if (showDeckTemplatePage)
      setNewViewMode = new CollectionUtils.ViewMode?(CollectionUtils.ViewMode.DECK_TEMPLATE);
    else if (this.m_currentViewMode == CollectionUtils.ViewMode.HERO_SKINS && !this.CanViewHeroSkins() || this.m_currentViewMode == CollectionUtils.ViewMode.CARD_BACKS && !this.CanViewCardBacks() || this.m_currentViewMode == CollectionUtils.ViewMode.COINS && !this.CanViewCoins() || SceneMgr.Get().IsInDuelsMode())
      setNewViewMode = new CollectionUtils.ViewMode?(CollectionUtils.ViewMode.CARDS);
    CollectionDeckTray.Get().ShowDeck((CollectionUtils.ViewMode) ((int) setNewViewMode ?? (int) this.GetViewMode()));
    if (setNewViewMode.HasValue)
    {
      bool triggerResponse = showDeckTemplatePage;
      this.SetViewMode(setNewViewMode.Value, triggerResponse, (CollectionUtils.ViewModeData) null);
    }
    this.UpdateSetFilters(currDeck.FormatType, true);
    this.m_pageManager.UpdateFiltersForDeck(currDeck, deckClasses, isNewDeck);
    this.m_pageManager.UpdateCraftingModeButtonDustBottleVisibility(CollectionManager.Get().GetCardsToDisenchantCount());
    NotificationManager.Get().DestroyNotification(this.m_createDeckNotification, 0.25f);
  }

  private List<TAG_CLASS> GetDeckHeroClasses(long deckID)
  {
    List<TAG_CLASS> deckHeroClasses = new List<TAG_CLASS>();
    CollectionDeck deck = CollectionManager.Get().GetDeck(deckID);
    if (deck != null)
      return deck.GetClasses();
    Log.CollectionManager.Print(string.Format("CollectionManagerDisplay no deck with ID {0}!", (object) deckID));
    deckHeroClasses.Add(TAG_CLASS.INVALID);
    return deckHeroClasses;
  }

  private IEnumerator DoBookOpeningAnimations()
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    while (collectionManagerDisplay.m_isBookCoverLoading)
      yield return (object) null;
    if ((UnityEngine.Object) collectionManagerDisplay.m_cover != (UnityEngine.Object) null)
      collectionManagerDisplay.m_cover.Open(new CollectionCoverDisplay.DelOnOpened(((CollectibleDisplay) collectionManagerDisplay).OnCoverOpened));
    else
      collectionManagerDisplay.OnCoverOpened();
    collectionManagerDisplay.m_manaTabManager.ActivateTabs(true);
  }

  private IEnumerator SetBookToOpen()
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    while (collectionManagerDisplay.m_isBookCoverLoading)
      yield return (object) null;
    if ((UnityEngine.Object) collectionManagerDisplay.m_cover != (UnityEngine.Object) null)
      collectionManagerDisplay.m_cover.SetOpenState();
    collectionManagerDisplay.m_manaTabManager.ActivateTabs(true);
  }

  private void DoBookClosingAnimations()
  {
    if ((UnityEngine.Object) this.m_cover != (UnityEngine.Object) null)
      this.m_cover.Close();
    this.m_manaTabManager.ActivateTabs(false);
  }

  private void ShowAdvancedCollectionManager(bool show)
  {
    show |= (bool) UniversalInputManager.UsePhoneUI;
    this.m_manaTabManager.gameObject.SetActive(show);
    if ((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null)
      this.m_setFilterTray.SetButtonShown(show && !(bool) UniversalInputManager.UsePhoneUI);
    if ((UnityEngine.Object) this.m_craftingTray == (UnityEngine.Object) null)
      AssetLoader.Get().InstantiatePrefab((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "CraftingTray_phone.prefab:bd4719b05f6f24870be20fa595b2032a" : "CraftingTray.prefab:dae9f103e23a53f459baeef392daa984"), new PrefabCallback<GameObject>(((CollectibleDisplay) this).OnCraftingTrayLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    if (CollectionManagerDisplay.ShouldSeeCraftingButton())
    {
      this.m_craftingModeButton.gameObject.SetActive(true);
      this.m_craftingModeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(((CollectibleDisplay) this).OnCraftingModeButtonReleased));
    }
    else
      this.m_craftingModeButton.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null & show && !this.m_setFilterTrayInitialized)
    {
      this.m_setFilterTray.AddItemUsingTexture(GameStrings.Get("GLUE_COLLECTION_ALL_STANDARD_CARDS"), this.m_allSetsTexture, new UnityEngine.Vector2?(this.m_allSetsIconOffset), new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback), new List<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) GameUtils.GetStandardSets()), (List<int>) null, PegasusShared.FormatType.FT_STANDARD, true, true, GameStrings.Get("GLUE_TOOLTIP_HEADER_ALL_STANDARD_CARDS"), GameStrings.Get("GLUE_TOOLTIP_DESCRIPTION_ALL_STANDARD_CARDS"));
      this.m_setFilterTray.AddItemUsingTexture(GameStrings.Get("GLUE_COLLECTION_WILD_CARDS"), this.m_wildSetsTexture, new UnityEngine.Vector2?(this.m_wildSetsIconOffset), new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback), new List<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) GameUtils.GetAllWildPlayableSets()), (List<int>) null, PegasusShared.FormatType.FT_WILD, tooltipActive: true, tooltipHeadline: GameStrings.Get("GLUE_TOOLTIP_HEADER_WILD_CARDS"), tooltipDescription: GameStrings.Get("GLUE_TOOLTIP_DESCRIPTION_WILD_CARDS"));
      SetFilterTray setFilterTray = this.m_setFilterTray;
      string itemName = GameStrings.Get("GLUE_COLLECTION_VANILLA_CARDS");
      Texture classicSetsTexture = this.m_classicSetsTexture;
      UnityEngine.Vector2? iconOffset = new UnityEngine.Vector2?(this.m_classicSetsIconOffset);
      SetFilterItem.ItemSelectedCallback callback = new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback);
      List<TAG_CARD_SET> cardSets = new List<TAG_CARD_SET>();
      cardSets.Add(TAG_CARD_SET.VANILLA);
      string tooltipHeadline = GameStrings.Get("GLUE_TOOLTIP_HEADER_VANILLA_CARDS");
      string tooltipDescription = GameStrings.Get("GLUE_TOOLTIP_DESCRIPTION_VANILLA_CARDS");
      setFilterTray.AddItemUsingTexture(itemName, classicSetsTexture, iconOffset, callback, cardSets, (List<int>) null, PegasusShared.FormatType.FT_CLASSIC, tooltipActive: true, tooltipHeadline: tooltipHeadline, tooltipDescription: tooltipDescription);
      List<int> featuredCards = CollectionManager.GetFeaturedCards();
      if (featuredCards.Any<int>())
      {
        SetFilterItem setFilterItem = this.m_setFilterTray.AddItemUsingTexture(GameStrings.Get("GLUE_COLLECTION_NEW_CARDS"), this.m_featuredCardsTexture, new UnityEngine.Vector2?(this.m_featuredCardsIconOffset), new SetFilterItem.ItemSelectedCallback(this.FeaturedCardsSetFilterCallback), (List<TAG_CARD_SET>) null, featuredCards, PegasusShared.FormatType.FT_STANDARD);
        this.m_currentActiveFeaturedCardsEvent = GameDbf.Card.GetRecord(featuredCards.First<int>()).FeaturedCardsEvent;
        this.StartCoroutine(this.SetIconFxIfFeaturedCardsEventNotSeen(setFilterItem, this.m_currentActiveFeaturedCardsEvent));
        this.StartCoroutine(this.SetFeaturedCardsSetFilterGlowIfNotSeen(this.m_currentActiveFeaturedCardsEvent));
      }
      this.PopulateSetFilters();
      this.m_setFilterTrayInitialized = true;
    }
    else if (!show)
      this.ShowSets(new List<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) GameUtils.GetStandardSets()));
    this.ShowAppropriateSetFilters();
    if (!show)
      return;
    this.m_manaTabManager.SetUpTabs();
  }

  private void AddDuelsSetFilters()
  {
    foreach (TAG_CARD_SET duelsSet in DuelsConfig.GetDuelsSets())
      this.AddSetFilter(duelsSet);
  }

  private void AddSetFilters(bool isWild)
  {
    foreach (TAG_CARD_SET cardSet in (IEnumerable<TAG_CARD_SET>) CollectionManager.Get().GetDisplayableCardSets().Where<TAG_CARD_SET>((Func<TAG_CARD_SET, bool>) (cardSetId =>
    {
      if (GameUtils.IsWildCardSet(cardSetId) != isWild || GameUtils.IsClassicCardSet(cardSetId))
        return false;
      return !GameUtils.IsLegacySet(cardSetId) || cardSetId == TAG_CARD_SET.LEGACY;
    })).OrderByDescending<TAG_CARD_SET, int>((Func<TAG_CARD_SET, int>) (cardSetId =>
    {
      CardSetDbfRecord cardSet = GameDbf.GetIndex().GetCardSet(cardSetId);
      return cardSet == null ? 0 : cardSet.ReleaseOrder;
    })))
      this.AddSetFilter(cardSet);
  }

  private void AddSetFilter(TAG_CARD_SET cardSet)
  {
    List<TAG_CARD_SET> data = new List<TAG_CARD_SET>();
    if (cardSet == TAG_CARD_SET.LEGACY)
      data.AddRange((IEnumerable<TAG_CARD_SET>) GameUtils.GetLegacySets());
    else if (!GameUtils.IsLegacySet(cardSet))
      data.Add(cardSet);
    string iconTextureAssetRef = (string) null;
    UnityEngine.Vector2? iconOffset = new UnityEngine.Vector2?();
    CardSetDbfRecord cardSet1 = GameDbf.GetIndex().GetCardSet(cardSet);
    if (cardSet1 != null)
    {
      if (cardSet1.IsCoreCardSet)
      {
        iconTextureAssetRef = "Filter_Icons_Core.tif:effec2b862f39224bac756f4a498164a";
        iconOffset = new UnityEngine.Vector2?(SetRotationIcon.GetYearIconTextureOffset() / 2f);
      }
      else
      {
        iconTextureAssetRef = cardSet1.FilterIconTexture;
        iconOffset = new UnityEngine.Vector2?(new UnityEngine.Vector2((float) cardSet1.FilterIconOffsetX, (float) cardSet1.FilterIconOffsetY));
      }
    }
    this.m_setFilterTray.AddItem(GameStrings.GetCardSetNameShortened(cardSet), iconTextureAssetRef, iconOffset, new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback), data, GameUtils.GetCardSetFormat(cardSet));
  }

  public void PopulateSetFilters(bool shouldReset = false)
  {
    if (shouldReset)
      this.m_setFilterTray.RemoveAllItems();
    if (SceneMgr.Get().IsInDuelsMode())
    {
      this.m_setFilterTray.AddItemUsingTexture(GameStrings.Get("GLUE_COLLECTION_WILD_CARDS"), this.m_wildSetsTexture, new UnityEngine.Vector2?(this.m_wildSetsIconOffset), new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback), new List<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) GameUtils.GetAllWildPlayableSets()), (List<int>) null, PegasusShared.FormatType.FT_WILD, tooltipActive: true, tooltipHeadline: GameStrings.Get("GLUE_TOOLTIP_HEADER_WILD_CARDS"), tooltipDescription: GameStrings.Get("GLUE_TOOLTIP_DESCRIPTION_WILD_CARDS"));
      this.m_setFilterTray.AddHeader(GameStrings.Get("GLUE_COLLECTION_ALL_SETS"), PegasusShared.FormatType.FT_STANDARD);
      this.AddDuelsSetFilters();
    }
    else
    {
      this.m_setFilterTray.AddHeader(GameStrings.Get("GLUE_COLLECTION_STANDARD_SETS"), PegasusShared.FormatType.FT_STANDARD);
      this.AddSetFilters(false);
      this.m_setFilterTray.AddHeader(GameStrings.Get("GLUE_COLLECTION_WILD_SETS"), PegasusShared.FormatType.FT_WILD);
      this.AddSetFilters(true);
      if (CollectionManager.Get().GetDisplayableCardSets().Contains(TAG_CARD_SET.SLUSH))
        this.AddSetFilter(TAG_CARD_SET.SLUSH);
    }
    if (Options.GetInRankedPlayMode() && !SceneMgr.Get().IsInDuelsMode())
    {
      if (this.m_setFilterTray.SelectFirstItemWithFormat(Options.GetFormatType()))
        return;
      this.m_setFilterTray.SelectFirstItem();
    }
    else
      this.m_setFilterTray.SelectFirstItem();
  }

  private long GetLastSeenFeaturedCardsEvent(GameSaveKeySubkeyId gameSaveSubkeyId)
  {
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.COLLECTION_MANAGER, gameSaveSubkeyId, out values);
    long featuredCardsEvent = 0;
    if (values != null && values.Any<long>())
      featuredCardsEvent = values.First<long>();
    return featuredCardsEvent;
  }

  private IEnumerator SetIconFxIfFeaturedCardsEventNotSeen(
    SetFilterItem setFilterItem,
    SpecialEventType currentActiveFeaturedCardsEvent)
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    while (!collectionManagerDisplay.m_isReady)
      yield return (object) null;
    long featuredCardsEvent = collectionManagerDisplay.GetLastSeenFeaturedCardsEvent(GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_ITEM);
    long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(currentActiveFeaturedCardsEvent);
    if (eventIdFromEventName != -1L && eventIdFromEventName != featuredCardsEvent)
      setFilterItem.SetIconFxActive(true);
  }

  private IEnumerator SetFeaturedCardsSetFilterGlowIfNotSeen(
    SpecialEventType currentActiveFeaturedCardsEvent)
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    while (!collectionManagerDisplay.m_isReady)
      yield return (object) null;
    long featuredCardsEvent = collectionManagerDisplay.GetLastSeenFeaturedCardsEvent(GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_BUTTON);
    long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(currentActiveFeaturedCardsEvent);
    if (eventIdFromEventName != -1L && eventIdFromEventName != featuredCardsEvent)
    {
      collectionManagerDisplay.m_setFilterTray.SetFilterButtonGlowActive(true);
      if ((UnityEngine.Object) collectionManagerDisplay.m_filterButtonGlow != (UnityEngine.Object) null)
        collectionManagerDisplay.m_filterButtonGlow.SetActive(true);
    }
  }

  private void SetLastSeenFeaturedCardsEvent(
    SpecialEventType currentActiveFeaturedCardsEvent,
    GameSaveKeySubkeyId subkeyId)
  {
    if (currentActiveFeaturedCardsEvent == SpecialEventType.UNKNOWN)
      return;
    long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(currentActiveFeaturedCardsEvent);
    if (eventIdFromEventName == -1L || this.GetLastSeenFeaturedCardsEvent(subkeyId) == eventIdFromEventName)
      return;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.COLLECTION_MANAGER, subkeyId, new long[1]
    {
      eventIdFromEventName
    }));
  }

  protected override void OnSearchDeactivated(string oldSearchText, string newSearchText) => this.OnSearchDeactivated_Internal(oldSearchText, newSearchText, true);

  private void OnSearchDeactivated_Internal(
    string oldSearchText,
    string newSearchText,
    bool updateManaFilterToMatchSearchText)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.EnableInput(true);
    if (oldSearchText == newSearchText)
    {
      this.OnSearchFilterComplete();
    }
    else
    {
      if (!this.m_craftingTray.IsShown() && newSearchText.ToLower() == GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING"))
      {
        if (this.m_currentViewMode == CollectionUtils.ViewMode.CARDS)
        {
          this.ShowCraftingTray(new bool?(), new bool?(), new bool?(), new bool?(), new bool?(), false);
        }
        else
        {
          (this.m_craftingTray as CraftingTray).EnableCraftingInBackground();
          this.m_searchTriggeredCraftingInBackground = true;
        }
        this.m_searchTriggeredCrafting = true;
      }
      else if (newSearchText.ToLower() != GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING"))
        this.ResetFilterSettingsFromSearch();
      this.NotifyFilterUpdate(this.m_searchFilterListeners, !string.IsNullOrEmpty(newSearchText), (object) newSearchText);
      if (updateManaFilterToMatchSearchText)
        this.UpdateManaFilterToMatchSearchText(newSearchText, false);
      this.m_pageManager.ChangeSearchTextFilter(newSearchText, new BookPageManager.DelOnPageTransitionComplete(((CollectibleDisplay) this).OnSearchFilterComplete), (object) null, true);
    }
  }

  protected override void OnSearchCleared(bool transitionPage)
  {
    this.ResetFilterSettingsFromSearch();
    this.NotifyFilterUpdate(this.m_searchFilterListeners, false, (object) "");
    this.m_pageManager.ChangeSearchTextFilter("", transitionPage);
    if (this.m_manaFilterIsFromSearchText)
      this.m_manaTabManager.ClearFilter();
    base.OnSearchCleared(transitionPage);
  }

  private void ResetFilterSettingsFromSearch()
  {
    if (this.m_searchTriggeredCrafting)
    {
      this.m_viewModeHidingCraftingTray = false;
      if (this.m_craftingTray.IsShown())
        this.m_craftingTray.Hide();
      else
        (this.m_craftingTray as CraftingTray).EnableCraftingInBackground(false);
    }
    this.m_searchTriggeredCrafting = false;
    this.m_searchTriggeredCraftingInBackground = false;
  }

  public void ShowTavernBrawlDeck(long deckID)
  {
    CollectionDeckTray.Get().GetDecksContent().SetEditingTraySection(0);
    CollectionDeckTray.Get().SetTrayMode(DeckTray.DeckContentTypes.Decks);
    this.RequestContentsToShowDeck(deckID);
  }

  public void ShowDuelsDeckHeader()
  {
    CollectionDeckTray.Get().GetDecksContent().SetEditingTraySection(0);
    CollectionDeckTray.Get().GetDecksContent().GetEditingTraySection().m_deckBox.HideBanner();
  }

  private void DoEnterCollectionManagerEvents()
  {
    if (CollectionManager.Get().HasVisitedCollection() || CollectionManagerDisplay.IsSpecialOneDeckMode())
    {
      this.EnableInput(true);
      this.OpenBookImmediately();
    }
    else
    {
      CollectionManager.Get().SetHasVisitedCollection(true);
      this.EnableInput(false);
      this.StartCoroutine(this.OpenBookWhenReady());
    }
  }

  private void OpenBookImmediately()
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.COLLECTION);
    this.StartCoroutine(this.SetBookToOpen());
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.COLLECTIONMANAGER)
      return;
    this.StartCoroutine(this.ShowCollectionTipsIfNeeded());
  }

  private IEnumerator OpenBookWhenReady()
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    while (CollectionManager.Get().IsWaitingForBoxTransition())
      yield return (object) null;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.COLLECTION);
    collectionManagerDisplay.m_pageManager.OnBookOpening();
    collectionManagerDisplay.StartCoroutine(collectionManagerDisplay.DoBookOpeningAnimations());
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
      collectionManagerDisplay.StartCoroutine(collectionManagerDisplay.ShowCollectionTipsIfNeeded());
  }

  private void ShowCraftingTipIfNeeded()
  {
    if (Options.Get().GetBool(Option.TIP_CRAFTING_UNLOCKED, false) || !UserAttentionManager.CanShowAttentionGrabber("CollectionManagerDisplay.ShowCraftingTipIfNeeded"))
      return;
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_DISENCHANT_31"), "VO_INNKEEPER_DISENCHANT_31.prefab:4a0246488dc2d8146b1db88de5c603ff");
    Options.Get().SetBool(Option.TIP_CRAFTING_UNLOCKED, true);
  }

  private Vector3 GetNewDeckPosition()
  {
    Vector3 vector3 = (bool) UniversalInputManager.UsePhoneUI ? new Vector3(25.7f, 2.6f, 0.0f) : new Vector3(17.5f, 0.0f, 0.0f);
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    return (UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null ? collectionDeckTray.GetDecksContent().GetNewDeckButtonPosition() - vector3 : new Vector3(0.0f, 0.0f, 0.0f);
  }

  private Vector3 GetLastDeckPosition()
  {
    Vector3 vector3 = (bool) UniversalInputManager.UsePhoneUI ? new Vector3(15.8f, 0.0f, 6f) : new Vector3(9.6f, 0.0f, 3f);
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    return (UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null ? collectionDeckTray.GetDecksContent().GetLastUsedTraySection().transform.position - vector3 : new Vector3(0.0f, 0.0f, 0.0f);
  }

  private Vector3 GetMiddleDeckPosition()
  {
    int index = 4;
    Vector3 vector3 = (bool) UniversalInputManager.UsePhoneUI ? new Vector3(15.8f, 0.0f, 6f) : new Vector3(9.6f, 0.0f, 3f);
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    return (UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null ? collectionDeckTray.GetDecksContent().GetTraySection(index).transform.position - vector3 : new Vector3(0.0f, 0.0f, 0.0f);
  }

  private void ShowSetRotationNewDeckIndicator(float f)
  {
    string text;
    Vector3 position;
    if (CollectionManager.Get().GetNumberOfWildDecks() >= 27)
    {
      text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL15");
      position = this.GetMiddleDeckPosition();
    }
    else
    {
      if (CollectionManager.Get().GetNumberOfWildDecks() <= 0)
        return;
      if (CollectionManager.Get().GetNumberOfStandardDecks() > 0)
      {
        text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL14");
        position = this.GetLastDeckPosition();
      }
      else
      {
        text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL10");
        CollectionDeckTray.Get().GetDecksContent().m_newDeckButton.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
        position = this.GetNewDeckPosition();
      }
    }
    this.m_createDeckNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, position, this.m_editDeckTutorialBone.localScale, text);
    if (!((UnityEngine.Object) this.m_createDeckNotification != (UnityEngine.Object) null))
      return;
    this.m_createDeckNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
    this.m_createDeckNotification.PulseReminderEveryXSeconds(3f);
  }

  public IEnumerator ShowCollectionTipsIfNeeded()
  {
    CollectionManagerDisplay collectionManagerDisplay = this;
    while (CollectionManager.Get().IsWaitingForBoxTransition())
      yield return (object) null;
    int deckCount = CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK).Count;
    if (UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, "CollectionManagerDisplay.ShowCollectionTipsIfNeeded:ShowSetRotationTutorial") && CollectionManager.Get().ShouldShowWildToStandardTutorial())
    {
      CollectionDeckTray deckTray = CollectionDeckTray.Get();
      while (deckTray.IsUpdatingTrayMode() || !deckTray.GetDecksContent().IsDoneEntering())
        yield return (object) null;
      if (deckCount >= 27)
      {
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, GameStrings.Get("GLUE_COLLECTION_TUTORIAL11"), "VO_INNKEEPER_Male_Dwarf_FULL_DECKS_06.prefab:21adedb0a5456c24da1b2918c3d04e5a");
        collectionManagerDisplay.ShowSetRotationNewDeckIndicator(0.0f);
      }
      else if (deckCount > (int) collectionManagerDisplay.m_onscreenDecks)
        deckTray.m_scrollbar.SetScroll(1f, new UIBScrollable.OnScrollComplete(collectionManagerDisplay.ShowSetRotationNewDeckIndicator), iTween.EaseType.easeOutBounce, 0.75f, true);
      else
        collectionManagerDisplay.ShowSetRotationNewDeckIndicator(0.0f);
    }
    else
    {
      if (Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false))
        Options.Get().SetBool(Option.HAS_SEEN_COLLECTIONMANAGER_AFTER_PRACTICE, true);
      if (!Options.Get().GetBool(Option.HAS_SEEN_COLLECTIONMANAGER, false) && UserAttentionManager.CanShowAttentionGrabber("UserAttentionManager.CanShowAttentionGrabber:" + (object) Option.HAS_SEEN_COLLECTIONMANAGER))
      {
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_WELCOME"), "VO_INNKEEPER_Male_Dwarf_CM_WELCOME_23.prefab:c8afdeaaf2189eb42aad9d29f6a97994");
        Options.Get().SetBool(Option.HAS_SEEN_COLLECTIONMANAGER, true);
        yield return (object) new WaitForSeconds(3.5f);
      }
      else
        yield return (object) new WaitForSeconds(1f);
      int num = UserAttentionManager.CanShowAttentionGrabber("CollectionManagerDisplay.ShowCollectionTipsIfNeeded:" + (object) Option.HAS_STARTED_A_DECK) ? 1 : 0;
      bool flag = Options.Get().GetBool(Option.HAS_STARTED_A_DECK, false);
      if (num != 0 && !flag && deckCount > 0)
      {
        collectionManagerDisplay.m_deckHelpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, collectionManagerDisplay.m_editDeckTutorialBone.position, collectionManagerDisplay.m_editDeckTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL07"));
        collectionManagerDisplay.m_deckHelpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
        collectionManagerDisplay.m_deckHelpPopup.PulseReminderEveryXSeconds(3f);
      }
    }
  }

  private void ShowDeckTemplateTipsIfNeeded()
  {
    bool flag1 = (UnityEngine.Object) this.m_deckHelpPopup != (UnityEngine.Object) null && (UnityEngine.Object) this.m_deckHelpPopup.gameObject != (UnityEngine.Object) null;
    Notification deckHelpPopup = CollectionDeckTray.Get().GetCardsContent().GetDeckHelpPopup();
    bool flag2 = (UnityEngine.Object) deckHelpPopup != (UnityEngine.Object) null && (UnityEngine.Object) deckHelpPopup.gameObject != (UnityEngine.Object) null;
    bool flag3 = (UnityEngine.Object) this.m_createDeckNotification != (UnityEngine.Object) null && (UnityEngine.Object) this.m_createDeckNotification.gameObject != (UnityEngine.Object) null;
    bool flag4 = (((UnityEngine.Object) this.m_craftingTray != (UnityEngine.Object) null && this.m_craftingTray.IsShown() || CraftingManager.GetIsInCraftingMode() ? 1 : (DeckHelper.Get().IsActive() ? 1 : 0)) | (flag1 ? 1 : 0) | (flag2 ? 1 : 0) | (flag3 ? 1 : 0)) != 0 || SceneMgr.Get().IsInDuelsMode() || CollectionDeckTray.Get().GetDecksContent().IsShowingDeckOptions;
    CollectionDeckSlot invalidSlot = CollectionDeckTray.Get().GetCardsContent().FindInvalidSlot();
    if (invalidSlot != null && !flag4)
    {
      if (this.m_showingDeckTemplateTips || this.m_currentViewMode != CollectionUtils.ViewMode.DECK_TEMPLATE && (CollectionDeckTray.Get().GetCurrentContentType() != DeckTray.DeckContentTypes.Cards || !CollectionDeckTray.Get().GetCardsContent().HasFinishedEntering()))
        return;
      string text;
      if (invalidSlot.Owned)
      {
        if (invalidSlot.Owned && Options.Get().GetBool(Option.HAS_SEEN_INVALID_ROTATED_CARD))
          return;
        text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS_NPR");
      }
      else
      {
        if (Options.Get().GetBool(Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD) || !UserAttentionManager.CanShowAttentionGrabber("CollectionManagerDisplay.ShowDeckTemplateTipsIfNeeded:" + (object) Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD))
          return;
        if (this.m_currentViewMode == CollectionUtils.ViewMode.DECK_TEMPLATE)
        {
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            invalidSlot = this.m_deckTemplatePickerPhone.m_phoneTray.GetCardsContent().FindInvalidSlot();
            if (invalidSlot == null)
            {
              Debug.LogError((object) "Phone Template Tray and CollectionDeckTray mismatch. Missing invalid card on Template.");
              return;
            }
          }
          text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1");
          if ((double) this.m_deckTemplateTipWaitTime < 0.5)
          {
            this.m_deckTemplateTipWaitTime += Time.deltaTime;
            return;
          }
        }
        else
        {
          text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2");
          if ((double) this.m_deckTemplateTipWaitTime < 1.0)
          {
            this.m_deckTemplateTipWaitTime += Time.deltaTime;
            return;
          }
        }
      }
      DeckTrayDeckTileVisual cardTileVisual = CollectionDeckTray.Get().GetCardsContent().GetCardTileVisual(invalidSlot.Index);
      if ((UnityEngine.Object) cardTileVisual == (UnityEngine.Object) null)
        return;
      float num = -60f;
      Vector3 relativePosition = OverlayUI.Get().GetRelativePosition(cardTileVisual.transform.position, Box.Get().m_Camera.GetComponent<Camera>(), OverlayUI.Get().m_heightScale.m_Center);
      Vector3 scale;
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        relativePosition.x -= 95.395f;
        relativePosition.z -= 0.25f;
        scale = 27.5f * Vector3.one;
        if ((double) relativePosition.z < (double) num)
          relativePosition.z = num;
      }
      else
      {
        relativePosition.x -= 50.5f;
        relativePosition.z -= 0.25f;
        scale = NotificationManager.NOTIFICATITON_WORLD_SCALE;
      }
      if (this.m_currentViewMode == CollectionUtils.ViewMode.DECK_TEMPLATE)
      {
        this.m_deckTemplateCardReplacePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, relativePosition, scale, text, false);
        if ((UnityEngine.Object) this.m_deckTemplateCardReplacePopup != (UnityEngine.Object) null)
        {
          this.m_deckTemplateCardReplacePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
          NotificationManager.Get().DestroyNotification(this.m_deckTemplateCardReplacePopup, 3.5f);
        }
      }
      else
      {
        this.m_deckTemplateCardReplacePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, relativePosition, scale, text, false);
        if ((UnityEngine.Object) this.m_deckTemplateCardReplacePopup != (UnityEngine.Object) null)
        {
          this.m_deckTemplateCardReplacePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
          this.m_deckTemplateCardReplacePopup.PulseReminderEveryXSeconds(3f);
        }
      }
      this.m_deckTemplateTipWaitTime = 0.0f;
      this.m_showingDeckTemplateTips = true;
    }
    else
    {
      if (this.m_showingDeckTemplateTips)
      {
        NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1"));
        NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2"));
        NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS"));
        NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS_NPR"));
      }
      this.m_deckTemplateTipWaitTime = 0.0f;
      this.m_showingDeckTemplateTips = false;
    }
  }

  public void ShowCardBackTipsIfNeeded()
  {
    if (!this.m_shouldShowMultipleFavoriteCardBackTutorial || CollectionManager.Get().IsInEditMode() || CardBackManager.Get().GetNumCardBacksOwned() <= 3)
      return;
    if ((UnityEngine.Object) this.m_multipleFavoriteCardBackTutorialBone == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "No bone for multiple card back tutorial. Did you forget a connection in CollectionManagerDisplay?");
    }
    else
    {
      string text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_MULTIPLE_FAVORITE_CARD_BACKS");
      this.m_multipleFavoriteCardBacksNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, this.m_multipleFavoriteCardBackTutorialBone, text);
      if (!((UnityEngine.Object) this.m_multipleFavoriteCardBacksNotification != (UnityEngine.Object) null))
        return;
      this.m_multipleFavoriteCardBacksNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
      this.m_multipleFavoriteCardBacksNotification.PulseReminderEveryXSeconds(3f);
      this.m_shouldShowMultipleFavoriteCardBackTutorial = false;
      GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_MULTIPLE_FAVORITE_CARD_BACKS, true);
    }
  }

  public void HideCardBackTips()
  {
    if (!((UnityEngine.Object) this.m_multipleFavoriteCardBacksNotification != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_multipleFavoriteCardBacksNotification);
  }

  public void ShowHeroTipsIfNeeded()
  {
    if (!this.m_heroSkinClass.HasValue || !this.m_shouldShowMultipleFavoriteHeroTutorial || CollectionManager.Get().IsInEditMode() || CollectionManager.Get().GetCountOfOwnedHeroesForClass(this.m_heroSkinClass.Value) < 2)
      return;
    if ((UnityEngine.Object) this.m_multipleFavoriteHeroTutorialBone == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "No bone for multiple favorite heroes tutorial. Did you forget a connection in CollectionManagerDisplay?");
    }
    else
    {
      string text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_MULTIPLE_FAVORITE_HEROES");
      this.m_multipleFavoriteHeroesNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, this.m_multipleFavoriteHeroTutorialBone, text);
      if (!((UnityEngine.Object) this.m_multipleFavoriteHeroesNotification != (UnityEngine.Object) null))
        return;
      this.m_multipleFavoriteHeroesNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
      this.m_multipleFavoriteHeroesNotification.PulseReminderEveryXSeconds(3f);
      this.m_shouldShowMultipleFavoriteHeroTutorial = false;
      GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_HAS_SEEN_MULTIPLE_FAVORITE_HEROES, true);
    }
  }

  public void HideHeroTips()
  {
    if (!((UnityEngine.Object) this.m_multipleFavoriteHeroesNotification != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_multipleFavoriteHeroesNotification);
  }

  public void HideAllCosmeticTips()
  {
    this.HideCardBackTips();
    this.HideHeroTips();
  }

  protected override void OnSwitchViewModeResponse(
    bool triggerResponse,
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode newMode,
    CollectionUtils.ViewModeData userdata)
  {
    base.OnSwitchViewModeResponse(triggerResponse, prevMode, newMode, userdata);
    this.EnableSetAndManaFiltersByViewMode(newMode);
    this.EnableCraftingByViewMode(newMode);
    this.EnableTutorialsByViewMode(newMode);
  }

  private void EnableSetAndManaFiltersByViewMode(CollectionUtils.ViewMode viewMode)
  {
    bool isEnabled = viewMode == CollectionUtils.ViewMode.CARDS;
    this.m_manaTabManager.Enabled = isEnabled;
    if ((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null)
    {
      this.m_setFilterTray.SetButtonEnabled(isEnabled);
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.m_setFilterTray.gameObject.SetActive(isEnabled);
    }
    this.m_search.SetEnabled(true);
  }

  private void EnableCraftingByViewMode(CollectionUtils.ViewMode viewMode)
  {
    bool enabled = viewMode == CollectionUtils.ViewMode.CARDS || viewMode == CollectionUtils.ViewMode.MASS_DISENCHANT;
    this.m_craftingModeButton.Enable(enabled);
    bool flag = this.m_viewModeHidingCraftingTray || this.m_searchTriggeredCraftingInBackground;
    if (!enabled)
    {
      CraftingTray craftingTray = this.m_craftingTray as CraftingTray;
      if (!craftingTray.IsShown())
        return;
      craftingTray.Hide(false);
      this.m_viewModeHidingCraftingTray = true;
    }
    else
    {
      if (!(enabled & flag))
        return;
      this.ShowCraftingTray(new bool?(), new bool?(), new bool?(), new bool?(), new bool?(), false);
      this.m_viewModeHidingCraftingTray = false;
      this.m_searchTriggeredCraftingInBackground = false;
    }
  }

  public void EnableTutorialsByViewMode(CollectionUtils.ViewMode viewMode)
  {
    this.HideAllCosmeticTips();
    if (viewMode != CollectionUtils.ViewMode.HERO_SKINS)
    {
      if (viewMode != CollectionUtils.ViewMode.CARD_BACKS)
        return;
      this.ShowCardBackTipsIfNeeded();
    }
    else
      this.ShowHeroTipsIfNeeded();
  }

  private void OnSetFilterButtonPressed()
  {
    this.SetLastSeenFeaturedCardsEvent(this.m_currentActiveFeaturedCardsEvent, GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_BUTTON);
    this.m_setFilterTray.SetFilterButtonGlowActive(false);
  }

  private void OnPhoneFilterButtonPressed()
  {
    this.SetLastSeenFeaturedCardsEvent(this.m_currentActiveFeaturedCardsEvent, GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_BUTTON);
    this.m_filterButtonGlow.SetActive(false);
  }

  protected override CraftingTrayBase GetCraftingTrayComponent(GameObject go) => (CraftingTrayBase) go.GetComponent<CraftingTray>();

  protected override void OnCraftingTrayLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    base.OnCraftingTrayLoaded(assetRef, go, callbackData);
    this.m_pageManager.UpdateMassDisenchant();
  }

  public override void ShowCraftingTray(
    bool? includeUncraftable = null,
    bool? normalOwned = null,
    bool? normalMissing = null,
    bool? premiumOwned = null,
    bool? premiumMissing = null,
    bool updatePage = true)
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if ((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null)
    {
      DeckTrayDeckListContent decksContent = collectionDeckTray.GetDecksContent();
      if ((UnityEngine.Object) decksContent != (UnityEngine.Object) null)
        decksContent.CancelRenameEditingDeck();
    }
    this.HideDeckHelpPopup();
    base.ShowCraftingTray(includeUncraftable, normalOwned, normalMissing, premiumOwned, premiumMissing, updatePage);
    this.ShowAppropriateSetFilters();
  }

  public override void HideCraftingTray()
  {
    base.HideCraftingTray();
    this.ShowAppropriateSetFilters();
  }

  public void ShowConvertTutorial(UserAttentionBlocker blocker)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "CollectionManagerDisplay.ShowConvertTutorial"))
      return;
    this.m_showConvertTutorialCoroutine = this.ShowConvertTutorialCoroutine(blocker);
    this.StartCoroutine(this.m_showConvertTutorialCoroutine);
  }

  private IEnumerator ShowConvertTutorialCoroutine(UserAttentionBlocker blocker)
  {
    if ((UnityEngine.Object) this.m_createDeckNotification != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotification(this.m_createDeckNotification, 0.25f);
    CollectionDeckTray deckTray = CollectionDeckTray.Get();
    while (deckTray.IsUpdatingTrayMode() || !deckTray.GetDecksContent().IsDoneEntering())
      yield return (object) null;
    yield return (object) new WaitForSeconds(0.5f);
    if (this.ViewModeHasVisibleDeckList())
    {
      this.m_convertTutorialPopup = NotificationManager.Get().CreatePopupText(blocker, this.m_convertDeckTutorialBone.position, this.m_convertDeckTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL12"));
      if ((UnityEngine.Object) this.m_convertTutorialPopup != (UnityEngine.Object) null)
      {
        this.m_convertTutorialPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
        this.m_convertTutorialPopup.PulseReminderEveryXSeconds(3f);
      }
      this.m_showConvertTutorialCoroutine = (IEnumerator) null;
    }
  }

  public void HideConvertTutorial()
  {
    if (this.m_showConvertTutorialCoroutine != null)
    {
      this.StopCoroutine(this.m_showConvertTutorialCoroutine);
      this.m_showConvertTutorialCoroutine = (IEnumerator) null;
    }
    if (!((UnityEngine.Object) this.m_convertTutorialPopup != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotification(this.m_convertTutorialPopup, 0.25f);
  }

  public void ShowSetFilterTutorial(UserAttentionBlocker blocker)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "CollectionManagerDisplay.ShowSetFilterTutorial"))
      return;
    this.m_showSetFilterTutorialCoroutine = this.ShowSetFilterTutorialCoroutine(blocker);
    this.StartCoroutine(this.m_showSetFilterTutorialCoroutine);
  }

  private IEnumerator ShowSetFilterTutorialCoroutine(UserAttentionBlocker blocker)
  {
    if ((UnityEngine.Object) this.m_setFilterTutorialPopup != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotification(this.m_setFilterTutorialPopup, 0.0f);
    this.m_setFilterTutorialPopup = NotificationManager.Get().CreatePopupText(blocker, this.m_setFilterTutorialBone.position, this.m_setFilterTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL17"));
    if ((UnityEngine.Object) this.m_setFilterTutorialPopup != (UnityEngine.Object) null)
    {
      this.m_setFilterTutorialPopup.ShowPopUpArrow((bool) UniversalInputManager.UsePhoneUI ? Notification.PopUpArrowDirection.Up : Notification.PopUpArrowDirection.LeftDown);
      this.m_setFilterTutorialPopup.PulseReminderEveryXSeconds(3f);
    }
    yield return (object) new WaitForSeconds(6f);
    this.HideSetFilterTutorial();
  }

  public void HideSetFilterTutorial()
  {
    if (this.m_showSetFilterTutorialCoroutine != null)
    {
      this.StopCoroutine(this.m_showSetFilterTutorialCoroutine);
      this.m_showSetFilterTutorialCoroutine = (IEnumerator) null;
    }
    if (!((UnityEngine.Object) this.m_setFilterTutorialPopup != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotification(this.m_setFilterTutorialPopup, 0.25f);
  }

  public void SetRuneLockedCheckboxVisible(bool visible)
  {
    this.m_runeLockedCheckboxContainer.SetActive(visible);
    this.m_runelockedCheckbox.SetChecked(CollectionPageManager.IsShowingLockedRuneCards);
  }

  public void ShowStandardInfoTutorial(UserAttentionBlocker blocker) => NotificationManager.Get().CreateInnkeeperQuote(blocker, GameStrings.Get("GLUE_COLLECTION_TUTORIAL13"), "VO_INNKEEPER_Male_Dwarf_STANDARD_WELCOME3_14.prefab:51e1d835435b64542b9a77944e00cc19");

  public void CheckClipboardAndPromptPlayerToPaste()
  {
    if (!this.CheckIfClipboardNotificationHasBeenShown())
      return;
    string message;
    if (!this.CheckClipboardAndGetValidityMessaging(out message))
    {
      if (!(message != string.Empty))
        return;
      CollectionInputMgr.AlertPlayerOnInvalidDeckPaste(message);
    }
    else
    {
      string str1 = GameStrings.Get("GLUE_COLLECTION_DECK_VALID_PASTE_BODY");
      string str2 = GameStrings.Get("GLUE_COLLECTION_DECK_VALID_PASTE_HEADER");
      if (CollectionManager.Get().IsInEditMode() && CollectionManager.Get().GetEditedDeck().GetTotalCardCount() > 0)
      {
        str1 = GameStrings.Get("GLUE_COLLECTION_DECK_OVERWRITE_BODY");
        str2 = GameStrings.Get("GLUE_COLLECTION_DECK_OVERWRITE_HEADER");
      }
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = str2,
        m_text = str1,
        m_cancelText = GameStrings.Get("GLUE_COLLECTION_DECK_SAVE_ANYWAY"),
        m_confirmText = GameStrings.Get("GLUE_COLLECTION_DECK_FINISH_FOR_ME"),
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response == AlertPopup.Response.CANCEL)
            this.RejectDeckFromClipboard();
          else
            this.CreateDeckFromClipboard(this.m_cachedShareableDeck);
        })
      };
      DialogManager.Get().ShowPopup(info);
    }
  }

  private bool CheckIfClipboardNotificationHasBeenShown()
  {
    if (PlatformSettings.OS != OSCategory.iOS || Options.Get().GetBool(Option.HAS_SEEN_CLIPBOARD_NOTIFICATION, false))
      return true;
    if (DialogManager.Get().ShowingDialog())
      return false;
    string str1 = GameStrings.Get("GLUE_COLLECTION_DECK_CLIPBOARD_ACCESS_HEADER");
    string str2 = GameStrings.Get("GLUE_COLLECTION_DECK_CLIPBOARD_ACCESS_BODY");
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = str1,
      m_text = str2,
      m_showAlertIcon = false,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => Options.Get().SetBool(Option.HAS_SEEN_CLIPBOARD_NOTIFICATION, true))
    };
    DialogManager.Get().ShowPopup(info);
    return false;
  }

  public void PasteFromClipboardIfValidOrShowStatusMessage()
  {
    if (!this.CheckIfClipboardNotificationHasBeenShown())
      return;
    string message;
    if (!this.CheckClipboardAndGetValidityMessaging(out message))
    {
      UIStatus.Get().AddInfo(message);
    }
    else
    {
      ClipboardUtils.CopyToClipboard(string.Empty);
      this.CreateDeckFromClipboard(this.m_cachedShareableDeck);
    }
  }

  private bool CheckClipboardAndGetValidityMessaging(out string message)
  {
    message = string.Empty;
    ShareableDeck shareableDeck = ShareableDeck.DeserializeFromClipboard();
    if (shareableDeck == null)
      return false;
    if (DialogManager.Get().ShowingDialog())
    {
      if (this.m_cachedShareableDeck != null && this.m_cachedShareableDeck.Equals((object) shareableDeck) || !this.CanPasteShareableDeck(shareableDeck))
        return false;
      DialogManager.Get().ClearAllImmediately();
    }
    this.m_cachedShareableDeck = shareableDeck;
    return this.CanPasteShareableDeck(this.m_cachedShareableDeck, out message);
  }

  private bool CanPasteShareableDeck(ShareableDeck shareableDeck) => this.CanPasteShareableDeck(shareableDeck, out string _);

  private bool CanPasteShareableDeck(ShareableDeck shareableDeck, out string alertMessage)
  {
    alertMessage = string.Empty;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER && !CollectionManager.Get().IsInEditMode() && !CollectionDeckTray.Get().m_decksContent.CanShowNewDeckButton() || SceneMgr.Get().IsInTavernBrawlMode() && !TavernBrawlDisplay.Get().IsInDeckEditMode() && (UnityEngine.Object) HeroPickerDisplay.Get() == (UnityEngine.Object) null || SceneMgr.Get().IsInDuelsMode() && !DuelsConfig.CanImportDecks() || CraftingTray.Get().IsShown())
      return false;
    if (!CollectionManager.Get().ShouldAccountSeeStandardWild() && shareableDeck.FormatType == PegasusShared.FormatType.FT_WILD)
    {
      alertMessage = GameStrings.Get("GLUE_COLLECTION_DECK_WILD_NOT_UNLOCKED");
      return false;
    }
    string cardId = GameUtils.TranslateDbIdToCardId(shareableDeck.HeroCardDbId);
    if (string.IsNullOrEmpty(cardId))
      return false;
    ScenarioDbId id = ScenarioDbId.INVALID;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
      id = (ScenarioDbId) TavernBrawlManager.Get().CurrentMission().missionId;
    List<TAG_CLASS> tagClassList = new List<TAG_CLASS>();
    DefLoader.Get().GetEntityDef(cardId).GetClasses((IList<TAG_CLASS>) tagClassList);
    if (id != ScenarioDbId.INVALID)
    {
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) id);
      if (record != null)
      {
        foreach (ClassExclusionsDbfRecord classExclusion in record.ClassExclusions)
        {
          foreach (TAG_CLASS tagClass in tagClassList)
          {
            if ((TAG_CLASS) classExclusion.ClassId == tagClass)
              return false;
          }
        }
      }
    }
    if (!SceneMgr.Get().IsInDuelsMode())
    {
      foreach (TAG_CLASS heroClass in tagClassList)
      {
        if (!GameUtils.HasUnlockedClass(heroClass))
        {
          alertMessage = GameStrings.Get("GLUE_COLLECTION_DECK_HERO_NOT_UNLOCKED");
          return false;
        }
      }
    }
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (CollectionManager.Get().IsInEditMode() && (!this.IsValidHeroClassesForCollectionDeck(tagClassList, editedDeck) || editedDeck.GetShareableDeck().Equals((object) this.m_cachedShareableDeck)))
      return false;
    if (NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().ShouldPrevalidatePastedDeckCodes)
    {
      CollectionDeck deck = new CollectionDeck();
      if (!deck.FillFromShareableDeck(shareableDeck))
        return false;
      DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset() ?? deck.GetRuleset();
      if (deckRuleset != null)
        return deckRuleset.IsDeckValid(deck, CollectionDeck.DefaultIgnoreRules.ToArray());
    }
    return true;
  }

  private void CreateDeckFromClipboard(ShareableDeck shareableDeck)
  {
    bool flag = SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER;
    List<TAG_CLASS> tagClassList = new List<TAG_CLASS>();
    DefLoader.Get().GetEntityDef(shareableDeck.HeroCardDbId).GetClasses((IList<TAG_CLASS>) tagClassList);
    if (tagClassList.Count == 0)
    {
      Debug.LogError((object) string.Format("CollectionManagerDisplay.CreateDeckFromClipboard(): no hero classes for hero card id; shareableDeck.HeroCardDbId={0}", (object) shareableDeck.HeroCardDbId));
    }
    else
    {
      TAG_CLASS tagClass = tagClassList[0];
      NetCache.CardDefinition randomFavoriteHero = CollectionManager.Get().GetRandomFavoriteHero(tagClass);
      string heroCardID = randomFavoriteHero != null ? randomFavoriteHero.Name : CollectionManager.GetVanillaHero(tagClass);
      if (flag)
      {
        PegasusShared.FormatType formatType = Options.GetFormatType();
        if (formatType == PegasusShared.FormatType.FT_UNKNOWN)
        {
          RankMgr.LogMessage("Options.GetFormatType() = FT_UNKOWN", nameof (CreateDeckFromClipboard), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Game\\CollectionManager\\CollectionManagerDisplay.cs", 3214);
          return;
        }
        CollectionManager.s_PreHeroPickerFormat = formatType;
        Options.SetFormatType(shareableDeck.FormatType);
      }
      string customDeckName = (string) null;
      if (!string.IsNullOrEmpty(shareableDeck.DeckName))
        customDeckName = shareableDeck.DeckName;
      if (!CollectionManager.Get().IsInEditMode())
      {
        CollectionDeckTray.Get().GetDecksContent().CreateNewDeckFromUserSelection(tagClass, heroCardID, customDeckName, DeckSourceType.DECK_SOURCE_TYPE_PASTED_DECK, shareableDeck.Serialize(false));
        CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedFromClipboard));
        CollectionManager.Get().RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedByPlayer));
        if (!((UnityEngine.Object) HeroPickerDisplay.Get() != (UnityEngine.Object) null) || !HeroPickerDisplay.Get().IsShown())
          return;
        DeckPickerTrayDisplay.Get().SkipHeroSelectionAndCloseTray();
      }
      else
      {
        CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
        if (!this.IsValidHeroClassesForCollectionDeck(tagClassList, editedDeck))
        {
          AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
          {
            m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_PASTE_TOOLTIP_HEADLINE"),
            m_text = GameStrings.Get("GLUE_COLLECTION_DECK_PASTE_INVALID_CLASS_BODY"),
            m_confirmText = GameStrings.Get("GLOBAL_OKAY"),
            m_showAlertIcon = true,
            m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM
          };
          DialogManager.Get().ShowPopup(info);
        }
        else
          this.OnDeckCreatedFromClipboard(editedDeck.ID, editedDeck.Name);
      }
    }
  }

  private bool IsValidHeroClassesForCollectionDeck(List<TAG_CLASS> heroClasses, CollectionDeck deck)
  {
    if (heroClasses == null || deck == null)
      return false;
    List<TAG_CLASS> classes = deck.GetClasses();
    foreach (TAG_CLASS heroClass in heroClasses)
    {
      if (!classes.Contains(heroClass))
        return false;
    }
    return true;
  }

  private void OnDeckCreatedFromClipboard(long deckId, string name)
  {
    CollectionManager.Get().RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedFromClipboard));
    CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedByPlayer));
    bool flag = CollectionManager.Get().IsInEditMode();
    if (this.GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE)
    {
      DeckTemplatePicker deckTemplatePicker = (bool) UniversalInputManager.UsePhoneUI ? this.GetPhoneDeckTemplateTray() : this.m_pageManager.GetDeckTemplatePicker();
      if ((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null)
        Navigation.RemoveHandler(new Navigation.NavigateBackHandler(deckTemplatePicker.OnNavigateBack));
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.StartCoroutine(deckTemplatePicker.EnterDeckPhone());
    }
    if (CollectionDeckTray.Get().GetCurrentContentType() != DeckTray.DeckContentTypes.Cards)
    {
      CollectionDeckTray.Get().RegisterModeSwitchedListener(new DeckTray.ModeSwitched(this.OnCollectionDeckTrayModeSwitched));
      this.ShowDeck(deckId, !flag, false, new CollectionUtils.ViewMode?(CollectionUtils.ViewMode.CARDS));
    }
    else
    {
      this.ShowDeck(deckId, !flag, false, new CollectionUtils.ViewMode?(CollectionUtils.ViewMode.CARDS));
      this.OnCollectionDeckTrayModeSwitched();
    }
  }

  private void OnCollectionDeckTrayModeSwitched()
  {
    CollectionDeckTray.Get().UnregisterModeSwitchedListener(new DeckTray.ModeSwitched(this.OnCollectionDeckTrayModeSwitched));
    if (this.m_cachedShareableDeck != null)
      CollectionInputMgr.PasteDeckInEditModeFromShareableDeck(this.m_cachedShareableDeck);
    else
      CollectionInputMgr.PasteDeckFromClipboard();
    ClipboardUtils.CopyToClipboard(string.Empty);
    this.m_cachedShareableDeck = (ShareableDeck) null;
  }

  private void RejectDeckFromClipboard()
  {
    ClipboardUtils.CopyToClipboard(string.Empty);
    this.m_cachedShareableDeck = (ShareableDeck) null;
  }

  public void SetHeroSkinClass(TAG_CLASS? newClass) => this.m_heroSkinClass = newClass;

  public TAG_CLASS? GetHeroSkinClass() => this.m_heroSkinClass;

  public static bool ShouldShowDeckOptionsMenu() => true;

  public static bool ShouldShowDeckHeaderInfo() => true;

  public static bool IsSpecialOneDeckMode() => SceneMgr.Get().IsInTavernBrawlMode() || SceneMgr.Get().IsInDuelsMode();

  public static bool ShouldSeeFilterButton() => CollectionManager.Get().GetOwnedCards().Count > 0;

  public static bool ShouldSeeCraftingButton() => CollectionManager.Get().GetOwnedCards().Count > 0;
}
