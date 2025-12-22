using Blizzard.T5.Services;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CollectibleDisplay : AbsSceneDisplay
{
  [CustomEditField(Sections = "Prefabs")]
  public CollectionCardVisual m_cardVisualPrefab;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_activeSearchBone;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_activeSearchBone_Win8;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_craftingTrayHiddenBone;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_craftingTrayShownBone;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_root;
  [CustomEditField(Sections = "Objects", T = EditType.GAME_OBJECT)]
  [FormerlySerializedAs("m_coverPrefab")]
  public String_MobileOverride m_bookCoverPrefab;
  [CustomEditField(Sections = "Objects")]
  public CollectionCoverDisplay m_cover;
  [CustomEditField(Sections = "Objects")]
  public CollectionSearch m_search;
  [CustomEditField(Sections = "Objects")]
  public CraftingModeButton m_craftingModeButton;
  [CustomEditField(Sections = "Objects")]
  public ActiveFilterButton m_filterButton;
  [CustomEditField(Sections = "Objects")]
  public GameObject m_filterButtonGlow;
  [CustomEditField(Sections = "Objects")]
  public PegUIElement m_inputBlocker;
  [CustomEditField(Sections = "Controls")]
  public CollectionUtils.CollectionPageLayoutSettings m_pageLayoutSettings = new CollectionUtils.CollectionPageLayoutSettings();
  [CustomEditField(Sections = "Materials")]
  public Material m_goldenCardNotOwnedMeshMaterial;
  [CustomEditField(Sections = "Materials")]
  public Material m_cardNotOwnedMeshMaterial;
  protected bool m_netCacheReady;
  protected bool m_gameSaveDataReady;
  protected bool m_isReady;
  protected bool m_unloading;
  protected List<CollectionCardActors> m_cardActors = new List<CollectionCardActors>();
  protected List<CollectionCardActors> m_previousCardActors = new List<CollectionCardActors>();
  protected bool m_setFilterTrayInitialized;
  protected bool m_isBookCoverLoading;
  protected CraftingTrayBase m_craftingTray;
  protected SetFilterTray m_setFilterTray;
  protected CollectionUtils.ViewMode m_currentViewMode;
  protected List<CollectibleDisplay.FilterStateListener> m_searchFilterListeners = new List<CollectibleDisplay.FilterStateListener>();
  protected int m_inputBlockers;
  protected bool m_searchTriggeredCrafting;
  protected bool m_searchTriggeredCraftingInBackground;
  protected const float CRAFTING_TRAY_SLIDE_IN_TIME = 0.25f;
  protected PlatformDependentValue<int> m_onscreenDecks = new PlatformDependentValue<int>(PlatformCategory.Screen)
  {
    PC = 8,
    Phone = 4
  };
  protected readonly PlatformDependentValue<bool> ALWAYS_SHOW_PAGING_ARROWS = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    iOS = true,
    Android = true,
    PC = false,
    Mac = false
  };

  public event CollectibleDisplay.ViewModeChangedListener OnViewModeChanged;

  public override void Start()
  {
    base.Start();
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerRelease));
    this.m_search.RegisterActivatedListener(new CollectionSearch.ActivatedListener(this.OnSearchActivated));
    this.m_search.RegisterDeactivatedListener(new CollectionSearch.DeactivatedListener(this.OnSearchDeactivated));
    this.m_search.RegisterClearedListener(new CollectionSearch.ClearedListener(this.OnSearchCleared));
    int num = Options.Get().GetInt(Option.PAGE_MOUSE_OVERS);
    CollectiblePageManager pageManager = this.GetPageManager();
    if (pageManager.m_numPlageFlipsBeforeStopShowingArrows == 0 || num < pageManager.m_numPlageFlipsBeforeStopShowingArrows || (bool) this.ALWAYS_SHOW_PAGING_ARROWS)
      pageManager.LoadPagingArrows();
    this.m_currentViewMode = this.GetInitialViewMode();
  }

  protected virtual void Awake()
  {
    if (CollectionManager.Get() != null)
      CollectionManager.Get().SetCollectibleDisplay(this);
    if (ServiceManager.Get<IGraphicsManager>().RenderQualityLevel != GraphicsQuality.Low && PlatformSettings.Memory == MemoryCategory.High && (UnityEngine.Object) this.m_cover == (UnityEngine.Object) null && !this.m_isBookCoverLoading)
    {
      this.m_isBookCoverLoading = true;
      AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_bookCoverPrefab, new PrefabCallback<GameObject>(this.OnBookCoverLoaded));
    }
    this.LoadAllTextures();
    this.EnableInput(true);
  }

  protected virtual void OnDestroy()
  {
    if (CollectionManager.Get() == null)
      return;
    CollectionManager.Get().SetCollectibleDisplay((CollectibleDisplay) null);
  }

  public Material GetGoldenCardNotOwnedMeshMaterial() => this.m_goldenCardNotOwnedMeshMaterial;

  public Material GetCardNotOwnedMeshMaterial() => this.m_cardNotOwnedMeshMaterial;

  public CollectionCardVisual GetCardVisualPrefab() => this.m_cardVisualPrefab;

  public abstract CollectiblePageManager GetPageManager();

  public bool IsReady() => this.m_isReady;

  public abstract void Unload();

  public abstract void Exit();

  public abstract void CollectionPageContentsChanged<TCollectible>(
    ICollection<TCollectible> collectiblesToDisplay,
    CollectibleDisplay.CollectionActorsReadyCallback callback,
    object callbackData)
    where TCollectible : ICollectible;

  public abstract void SetViewMode(
    CollectionUtils.ViewMode mode,
    bool triggerResponse,
    CollectionUtils.ViewModeData userdata = null);

  public abstract void HideAllTips();

  public abstract void SetFilterCallback(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    FormatType formatType,
    SetFilterItem item,
    bool transitionPage);

  public abstract void ShowInnkeeperLClickHelp(EntityDef entityDef);

  public bool ShouldShowNewCardGlow(string cardID, TAG_PREMIUM premium)
  {
    CollectibleCard card = CollectionManager.Get().GetCard(cardID, premium);
    return card != null && card.IsNewCard;
  }

  public CollectionUtils.CollectionPageLayoutSettings.Variables GetCurrentPageLayoutSettings() => this.GetPageLayoutSettings(this.m_currentViewMode);

  public CollectionUtils.CollectionPageLayoutSettings.Variables GetPageLayoutSettings(
    CollectionUtils.ViewMode viewMode)
  {
    return this.m_pageLayoutSettings.GetVariables(viewMode);
  }

  public void SetViewMode(CollectionUtils.ViewMode mode, CollectionUtils.ViewModeData userdata = null) => this.SetViewMode(mode, true, userdata);

  public CollectionUtils.ViewMode GetViewMode() => this.m_currentViewMode;

  public bool SetFilterTrayInitialized() => this.m_setFilterTrayInitialized;

  public virtual void FilterBySearchText(string newSearchText) => this.m_search.SetText(newSearchText);

  protected virtual void ShowSpecificCards(List<int> specificCards) => this.GetPageManager().FilterBySpecificCards(specificCards);

  public void GoToPageWithCard(string cardID, TAG_PREMIUM premium)
  {
    if (this.m_currentViewMode == CollectionUtils.ViewMode.DECK_TEMPLATE)
      this.SetViewMode(CollectionUtils.ViewMode.CARDS, new CollectionUtils.ViewModeData()
      {
        m_setPageByCard = cardID,
        m_setPageByPremium = premium
      });
    else
      this.GetPageManager().JumpToPageWithCard(cardID, premium);
  }

  public void UpdateCurrentPageCardLocks(bool playSound = false)
  {
    if ((UnityEngine.Object) this.GetPageManager() == (UnityEngine.Object) null)
      Log.CollectionManager.PrintError("CollectibleDisplay.UpdateCurrentPageCardLocks - GetPageManager returned null!");
    else
      this.GetPageManager().UpdateCurrentPageCardLocks(playSound);
  }

  public bool ViewModeChangedListenerExists(
    CollectibleDisplay.ViewModeChangedListener listener)
  {
    return ((IEnumerable<Delegate>) this.OnViewModeChanged.GetInvocationList()).Contains<Delegate>((Delegate) listener);
  }

  public void RegisterSearchFilterListener(CollectibleDisplay.FilterStateListener listener) => this.m_searchFilterListeners.Add(listener);

  public void UnregisterSearchFilterListener(CollectibleDisplay.FilterStateListener listener) => this.m_searchFilterListeners.Remove(listener);

  public virtual void ResetFilters(bool updateVisuals = true) => this.m_search.ClearFilter(updateVisuals);

  public void EnableInput(bool enable)
  {
    if (!enable)
      ++this.m_inputBlockers;
    else if (this.m_inputBlockers > 0)
      --this.m_inputBlockers;
    bool flag = this.m_inputBlockers > 0;
    if ((UnityEngine.Object) this.m_inputBlocker == (UnityEngine.Object) null)
      Log.CollectionManager.PrintError("CollectibleDisplay.EnableInput - input blocker is null!");
    else
      this.m_inputBlocker.gameObject.SetActive(flag);
  }

  public bool InCraftingMode() => (UnityEngine.Object) this.m_craftingTray != (UnityEngine.Object) null && this.m_craftingTray.IsShown();

  protected override bool ShouldStartShown() => true;

  public override bool IsFinishedLoading(out string failureMessage)
  {
    failureMessage = "CollectibleDisplay is never ready.";
    return this.m_isReady;
  }

  protected void OnCollectionLoaded() => this.GetPageManager().OnCollectionLoaded();

  protected virtual void OnCollectionChanged() => this.GetPageManager().NotifyOfCollectionChanged();

  protected void NotifyFilterUpdate(
    List<CollectibleDisplay.FilterStateListener> listeners,
    bool active,
    object value)
  {
    foreach (CollectibleDisplay.FilterStateListener listener in listeners)
      listener(active, value);
  }

  protected virtual CollectionUtils.ViewMode GetInitialViewMode()
  {
    List<CollectibleCard> ownedCards = CollectionManager.Get().GetOwnedCards();
    if (ownedCards.Any<CollectibleCard>((Func<CollectibleCard, bool>) (x => !x.IsHeroSkin)))
      return CollectionUtils.ViewMode.CARDS;
    if (CardBackManager.Get().GetNumCardBacksOwned() > 0)
      return CollectionUtils.ViewMode.CARD_BACKS;
    if (ownedCards.Any<CollectibleCard>((Func<CollectibleCard, bool>) (x => x.IsHeroSkin)))
      return CollectionUtils.ViewMode.HERO_SKINS;
    if (CoinManager.Get().GetCoinsOwned().Count > 0)
      return CollectionUtils.ViewMode.COINS;
    Debug.Log((object) "CollectibleDisplay:GetInitialViewMode: Player has no cards, card backs, hero skins or coins. Defaulting to Cards view");
    return CollectionUtils.ViewMode.CARDS;
  }

  protected abstract void LoadAllTextures();

  protected abstract void UnloadAllTextures();

  protected virtual void OnBookCoverLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_isBookCoverLoading = false;
    if ((UnityEngine.Object) this.m_root != (UnityEngine.Object) null)
      go.transform.SetParent(this.m_root.transform, true);
    this.m_cover = go.GetComponent<CollectionCoverDisplay>();
  }

  protected void OnInputBlockerRelease(UIEvent e) => this.m_search.Deactivate();

  protected void OnSearchActivated()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.EnableInput(false);
    this.GetPageManager().EnablePageTurn(false);
  }

  protected abstract void OnSearchDeactivated(string oldSearchText, string newSearchText);

  protected virtual void OnSearchCleared(bool transitionPage) => this.GetPageManager().EnablePageTurn(true);

  protected void OnSearchFilterComplete(object callbackdata = null) => this.GetPageManager().EnablePageTurn(true);

  protected void OnCoverOpened() => this.EnableInput(true);

  protected virtual void OnSwitchViewModeResponse(
    bool triggerResponse,
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode newMode,
    CollectionUtils.ViewModeData userdata)
  {
    CollectibleDisplay.ViewModeChangedListener onViewModeChanged = this.OnViewModeChanged;
    if (onViewModeChanged == null)
      return;
    onViewModeChanged(prevMode, newMode, userdata, triggerResponse);
  }

  protected virtual CraftingTrayBase GetCraftingTrayComponent(GameObject go) => (CraftingTrayBase) go.GetComponent<CraftingTray>();

  protected virtual void OnCraftingTrayLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    go.SetActive(false);
    this.m_craftingTray = this.GetCraftingTrayComponent(go);
    go.transform.parent = this.m_craftingTrayShownBone.transform.parent;
    go.transform.localPosition = this.m_craftingTrayHiddenBone.transform.localPosition;
    go.transform.localScale = this.m_craftingTrayHiddenBone.transform.localScale;
  }

  protected void OnCraftingModeButtonReleased(UIEvent e)
  {
    if (this.m_craftingTray.IsShown())
      this.m_craftingTray.Hide();
    else
      this.ShowCraftingTray();
  }

  public virtual void ShowCraftingTray(
    bool? includeUncraftable = null,
    bool? normalOwned = null,
    bool? normalMissing = null,
    bool? premiumOwned = null,
    bool? premiumMissing = null,
    bool updatePage = true)
  {
    this.m_craftingTray.gameObject.SetActive(true);
    this.m_craftingTray.Show(includeUncraftable, normalOwned, normalMissing, premiumOwned, premiumMissing, updatePage);
    Hashtable args = iTween.Hash((object) "position", (object) this.m_craftingTrayShownBone.transform.localPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutBounce);
    iTween.Stop(this.m_craftingTray.gameObject);
    iTween.MoveTo(this.m_craftingTray.gameObject, args);
    this.m_craftingModeButton.ShowActiveGlow(true);
  }

  public virtual void HideCraftingTray()
  {
    this.m_craftingTray.gameObject.SetActive(true);
    Hashtable args = iTween.Hash((object) "position", (object) this.m_craftingTrayHiddenBone.transform.localPosition, (object) "isLocal", (object) true, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutBounce, (object) "oncomplete", (object) (Action<object>) (o => this.m_craftingTray.gameObject.SetActive(false)));
    iTween.Stop(this.m_craftingTray.gameObject);
    iTween.MoveTo(this.m_craftingTray.gameObject, args);
    this.m_craftingModeButton.ShowActiveGlow(false);
  }

  public delegate void DelTextureLoaded(
    TAG_CLASS classTag,
    Texture classTexture,
    object callbackData);

  public delegate void CollectionActorsReadyCallback(
    List<CollectionCardActors> actors,
    List<ICollectible> nonActorCollectibles,
    object callbackData);

  public delegate void ViewModeChangedListener(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse);

  public delegate void FilterStateListener(bool filterActive, object value);

  protected class TextureRequests
  {
    public List<CollectibleDisplay.TextureRequests.Request> m_requests;

    public class Request
    {
      public CollectibleDisplay.DelTextureLoaded m_callback;
      public object m_callbackData;
    }
  }
}
