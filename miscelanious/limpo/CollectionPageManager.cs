using Blizzard.T5.Core;
using Hearthstone;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class CollectionPageManager : CollectiblePageManager
{
  public static readonly Map<TAG_CLASS, UnityEngine.Vector2> s_classTextureOffsets = new Map<TAG_CLASS, UnityEngine.Vector2>()
  {
    {
      TAG_CLASS.MAGE,
      new UnityEngine.Vector2(0.0f, 0.0f)
    },
    {
      TAG_CLASS.PALADIN,
      new UnityEngine.Vector2(0.205f, 0.0f)
    },
    {
      TAG_CLASS.PRIEST,
      new UnityEngine.Vector2(0.392f, 0.0f)
    },
    {
      TAG_CLASS.ROGUE,
      new UnityEngine.Vector2(0.58f, 0.0f)
    },
    {
      TAG_CLASS.SHAMAN,
      new UnityEngine.Vector2(0.774f, 0.0f)
    },
    {
      TAG_CLASS.WARLOCK,
      new UnityEngine.Vector2(0.0f, -0.2f)
    },
    {
      TAG_CLASS.WARRIOR,
      new UnityEngine.Vector2(0.205f, -0.2f)
    },
    {
      TAG_CLASS.DRUID,
      new UnityEngine.Vector2(0.392f, -0.2f)
    },
    {
      TAG_CLASS.HUNTER,
      new UnityEngine.Vector2(0.58f, -0.2f)
    },
    {
      TAG_CLASS.NEUTRAL,
      new UnityEngine.Vector2(0.774f, -0.2f)
    },
    {
      TAG_CLASS.WHIZBANG,
      new UnityEngine.Vector2(0.0f, -0.395f)
    },
    {
      TAG_CLASS.DEMONHUNTER,
      new UnityEngine.Vector2(0.205f, -0.4f)
    },
    {
      TAG_CLASS.DEATHKNIGHT,
      new UnityEngine.Vector2(0.392f, -0.4f)
    }
  };
  private static readonly Map<TAG_CLASS, Color> s_classColors = new Map<TAG_CLASS, Color>()
  {
    {
      TAG_CLASS.DEATHKNIGHT,
      new Color(0.06666667f, 0.5294118f, 0.5843138f)
    },
    {
      TAG_CLASS.MAGE,
      new Color(0.1294118f, 0.2666667f, 0.3882353f)
    },
    {
      TAG_CLASS.PALADIN,
      new Color(0.4392157f, 0.2941177f, 0.09019608f)
    },
    {
      TAG_CLASS.PRIEST,
      new Color(0.5215687f, 0.5215687f, 0.5215687f)
    },
    {
      TAG_CLASS.ROGUE,
      new Color(0.09019608f, 0.07450981f, 0.07450981f)
    },
    {
      TAG_CLASS.SHAMAN,
      new Color(0.1294118f, 0.172549f, 0.372549f)
    },
    {
      TAG_CLASS.WARLOCK,
      new Color(0.2117647f, 0.1098039f, 0.282353f)
    },
    {
      TAG_CLASS.WARRIOR,
      new Color(0.2745098f, 0.05098039f, 0.08235294f)
    },
    {
      TAG_CLASS.DRUID,
      new Color(0.2313726f, 0.1607843f, 0.08627451f)
    },
    {
      TAG_CLASS.HUNTER,
      new Color(0.2235294f, 0.4627451f, 0.1764706f)
    },
    {
      TAG_CLASS.NEUTRAL,
      new Color(0.0f, 0.0f, 0.0f)
    },
    {
      TAG_CLASS.WHIZBANG,
      new Color(0.5647059f, 0.3019608f, 0.5372549f)
    },
    {
      TAG_CLASS.DEMONHUNTER,
      new Color(0.09019608f, 0.227451f, 0.1960784f)
    }
  };
  public static TAG_CLASS[] CLASS_TAB_ORDER = new TAG_CLASS[12]
  {
    TAG_CLASS.DEATHKNIGHT,
    TAG_CLASS.DEMONHUNTER,
    TAG_CLASS.DRUID,
    TAG_CLASS.HUNTER,
    TAG_CLASS.MAGE,
    TAG_CLASS.PALADIN,
    TAG_CLASS.PRIEST,
    TAG_CLASS.ROGUE,
    TAG_CLASS.SHAMAN,
    TAG_CLASS.WARLOCK,
    TAG_CLASS.WARRIOR,
    TAG_CLASS.NEUTRAL
  };
  public CollectionClassTab m_heroSkinsTab;
  public CollectionClassTab m_cardBacksTab;
  public CollectionClassTab m_coinsTab;
  public ClassFilterHeaderButton m_classFilterHeader;
  public CollectionClassTab m_deckTemplateTab;
  [CustomEditField(Sections = "Deck Template", T = EditType.GAME_OBJECT)]
  public string m_deckTemplatePickerPrefab;
  private static CollectionUtils.ViewMode[] TAG_ORDERING = new CollectionUtils.ViewMode[5]
  {
    CollectionUtils.ViewMode.CARDS,
    CollectionUtils.ViewMode.COINS,
    CollectionUtils.ViewMode.CARD_BACKS,
    CollectionUtils.ViewMode.HERO_PICKER,
    CollectionUtils.ViewMode.HERO_SKINS
  };
  private static readonly int NUM_PAGE_FLIPS_BEFORE_SET_FILTER_TUTORIAL = 3;
  private List<CollectionClassTab> m_classTabs = new List<CollectionClassTab>();
  private MassDisenchant m_massDisenchant;
  private DeckTemplatePicker m_deckTemplatePicker;
  private CollectibleCardHeroesFilter m_heroesCollection = new CollectibleCardHeroesFilter();
  private Vector3 m_heroSkinsTabPos;
  private Vector3 m_cardBacksTabPos;
  private Vector3 m_coinsTabPos;
  private bool m_hideNonDeckTemplateTabs;
  private int m_numPageFlipsThisSession;
  protected CollectionTabInfo m_currentClassContext;
  protected ICollectible m_lastCollectibleAnchor;
  private readonly List<CollectionCardVisual> m_ghostedRuneCards = new List<CollectionCardVisual>();
  private string m_searchText;
  private List<CollectibleCard> m_disenchantCards = new List<CollectibleCard>();
  private bool m_deckRunesWereUpdatedOnCurrentPage;
  private RunePattern m_originalDeckRunesForCurrentPage;
  private const float DK_TUTORIAL_RUNE_POPUP_OFFSET_X_PC = 13f;
  private const float DK_TUTORIAL_RUNE_POPUP_OFFSET_X_PHONE = 14f;
  private const float DK_TUTORIAL_RUNE_POPUP_SCALE = 15f;
  private const float DK_TUTORIAL_RUNE_INDICATOR_ARROW_OFFSET_X_PC = -6f;
  private const float DK_TUTORIAL_RUNE_INDICATOR_ARROW_OFFSET_X_PHONE = -9f;
  private const float DK_TUTORIAL_RUNE_INDICATOR_ARROW_SCALE_PC = 7f;
  private const float DK_TUTORIAL_RUNE_INDICATOR_ARROW_SCALE_PHONE = 7f;
  private const float DK_TUTORIAL_RUNE_INDICATOR_ARROW_ROTATION = 90f;
  private Notification m_deathKnightRuneTutorialRunePopup;
  private Notification m_runeIndicatorArrow;

  public static Color ColorForClass(TAG_CLASS tagClass) => CollectionPageManager.s_classColors[tagClass];

  private CollectibleCardClassFilter m_classCardsCollection => (CollectibleCardClassFilter) this.m_cardsCollection;

  protected override void Awake()
  {
    base.Awake();
    this.m_cardsCollection = (CollectibleCardFilter) new CollectibleCardClassFilter();
    this.m_classCardsCollection.Init(CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.CARDS));
    this.m_heroesCollection.Init(CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.HERO_SKINS));
    this.UpdateFilteredCards();
    this.m_heroesCollection.UpdateResults();
    if ((bool) (UnityEngine.Object) this.m_massDisenchant)
      this.m_massDisenchant.Hide();
    CollectionManager.Get()?.RegisterFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
    CollectionPageManager.IsShowingLockedRuneCards = true;
    NetCache.Get().FavoriteCardBackChanged += new NetCache.DelFavoriteCardBackChangedListener(this.OnFavoriteCardBackChanged);
    NetCache.Get().FavoriteBattlegroundsGuideSkinChanged += new NetCache.DelFavoriteBattlegroundsGuideSkinChangedListener(this.OnFavoriteBattlegroundsGuideSkinChanged);
    NetCache.Get().FavoriteCoinChanged += new NetCache.DelFavoriteCoinChangedListener(this.OnFavoriteCoinChanged);
  }

  private void OnEnable()
  {
    CollectionManagerDisplay.HideLockedRunesCheckboxToggled += new Action<bool>(this.OnHideLockedRunesCheckboxToggled);
    RuneIndicatorVisual.RunePatternChanged += new Action<RunePattern>(this.RuneIndicatorVisualOnRunePatternChanged);
    CollectionDeckTray.DeckTrayCardAdded += new Action<CollectionDeck, RunePattern>(this.OnDeckTrayCardAdded);
    CraftingTray.CraftingTrayShown += new Action(this.OnCraftingTrayShown);
    CraftingTray.CraftingTrayHidden += new Action(this.OnCraftingTrayHidden);
  }

  private void OnDisable()
  {
    CollectionManagerDisplay.HideLockedRunesCheckboxToggled -= new Action<bool>(this.OnHideLockedRunesCheckboxToggled);
    RuneIndicatorVisual.RunePatternChanged -= new Action<RunePattern>(this.RuneIndicatorVisualOnRunePatternChanged);
    CollectionDeckTray.DeckTrayCardAdded -= new Action<CollectionDeck, RunePattern>(this.OnDeckTrayCardAdded);
    CraftingTray.CraftingTrayShown -= new Action(this.OnCraftingTrayShown);
    CraftingTray.CraftingTrayHidden -= new Action(this.OnCraftingTrayHidden);
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    CollectionManager.Get()?.RemoveFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
    if (NetCache.Get() == null)
      return;
    NetCache.Get().FavoriteCardBackChanged -= new NetCache.DelFavoriteCardBackChangedListener(this.OnFavoriteCardBackChanged);
    NetCache.Get().FavoriteBattlegroundsGuideSkinChanged -= new NetCache.DelFavoriteBattlegroundsGuideSkinChangedListener(this.OnFavoriteBattlegroundsGuideSkinChanged);
    NetCache.Get().FavoriteCoinChanged -= new NetCache.DelFavoriteCoinChangedListener(this.OnFavoriteCoinChanged);
  }

  public bool HideNonDeckTemplateTabs(bool hide, bool updateTabs = false)
  {
    if (this.m_hideNonDeckTemplateTabs == hide)
      return false;
    this.m_hideNonDeckTemplateTabs = hide;
    if (updateTabs)
      this.UpdateVisibleTabs();
    return true;
  }

  public bool IsNonDeckTemplateTabsHidden() => this.m_hideNonDeckTemplateTabs;

  public void UpdateFiltersForDeck(
    CollectionDeck deck,
    List<TAG_CLASS> deckClasses,
    bool skipPageTurn,
    BookPageManager.DelOnPageTransitionComplete callback = null,
    object callbackData = null)
  {
    this.m_skipNextPageTurn = skipPageTurn;
    bool flag1 = false;
    bool flag2 = false;
    if (deck != null && deck.GetRuleset() != null)
    {
      DeckRuleset ruleset = deck.GetRuleset();
      if (ruleset.EntityInDeckIgnoresRuleset(deck))
      {
        flag1 = true;
      }
      else
      {
        IEnumerable<DeckRule> source = ruleset.Rules.Where<DeckRule>((Func<DeckRule, bool>) (r => r.Type == DeckRule.RuleType.IS_CLASS_CARD_OR_NEUTRAL));
        if (source.Any<DeckRule>((Func<DeckRule, bool>) (r => r.RuleIsNot)))
          flag2 = true;
        else if (!source.Any<DeckRule>())
          flag1 = true;
      }
    }
    if (flag1)
      this.m_classCardsCollection.FilterTheseClasses((TAG_CLASS[]) null);
    else if (flag2)
      this.m_classCardsCollection.FilterTheseClasses(((IEnumerable<TAG_CLASS>) CollectionPageManager.CLASS_TAB_ORDER).Where<TAG_CLASS>((Func<TAG_CLASS, bool>) (tag => !deckClasses.Contains(tag))).ToArray<TAG_CLASS>());
    else
      this.m_classCardsCollection.FilterTheseClasses(new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) deckClasses)
      {
        TAG_CLASS.NEUTRAL
      }.ToArray());
    this.m_heroesCollection.FilterOnlyOwned(true);
    this.m_heroesCollection.UpdateResults();
    this.UpdateFilteredCards();
    this.UpdateVisibleTabs();
    bool flag3 = true;
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    switch (viewMode)
    {
      case CollectionUtils.ViewMode.DECK_TEMPLATE:
      case CollectionUtils.ViewMode.MASS_DISENCHANT:
        flag3 = false;
        break;
    }
    if (!flag3)
      return;
    switch (viewMode)
    {
      case CollectionUtils.ViewMode.CARDS:
        this.JumpToCollectionClassPage(new CollectionTabInfo()
        {
          tagClass = deckClasses.First<TAG_CLASS>()
        }, callback, callbackData);
        break;
      case CollectionUtils.ViewMode.HERO_SKINS:
      case CollectionUtils.ViewMode.CARD_BACKS:
        this.m_currentPageNum = 1;
        this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, callback, callbackData);
        break;
    }
  }

  public override bool JumpToPageWithCard(
    string cardID,
    TAG_PREMIUM premium,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    return this.JumpToPageWithCard(cardID, premium, callback, callbackData, true);
  }

  private bool JumpToPageWithCard(
    string cardID,
    TAG_PREMIUM premium,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool tryClearFilters)
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    CollectionTabInfo tabInfoContext = new CollectionTabInfo()
    {
      tagClass = TAG_CLASS.INVALID
    };
    if (editedDeck != null)
      tabInfoContext.tagClass = editedDeck.GetClass();
    int collectionPage;
    if (this.m_classCardsCollection.GetPageContentsForCard(cardID, premium, out collectionPage, tabInfoContext).Count == 0)
    {
      if (!tryClearFilters)
        return false;
      CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
      if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
        collectibleDisplay.ResetFilters(false);
      return this.JumpToPageWithCard(cardID, premium, callback, callbackData, false);
    }
    if (this.m_currentPageNum == collectionPage)
      return false;
    this.FlipToPage(collectionPage, callback, callbackData);
    return true;
  }

  private void RemoveAllClassFilters() => this.RemoveAllClassFilters((BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  private void RemoveAllClassFilters(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    this.m_cardsCollection.FilterTheseClasses((TAG_CLASS[]) null);
    this.UpdateFilteredCards();
    this.m_heroesCollection.FilterTheseClasses((TAG_CLASS[]) null);
    this.m_heroesCollection.FilterOnlyOwned(false);
    this.m_heroesCollection.UpdateResults();
    this.TransitionPageWhenReady(CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.CARDS ? BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT : BookPageManager.PageTransitionType.NONE, false, callback, callbackData);
  }

  public void FilterByManaCost(int cost, bool transitionPage = true)
  {
    if (cost == -1)
      this.m_cardsCollection.FilterManaCost(new int?());
    else
      this.m_cardsCollection.FilterManaCost(new int?(cost));
    this.UpdateFilteredCards();
    if (!transitionPage)
      return;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public bool IsManaCostFilterActive => this.m_cardsCollection != null && this.m_cardsCollection.IsManaCostFilterActive;

  public override void ChangeSearchTextFilter(
    string newSearchText,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool transitionPage = true)
  {
    if (newSearchText == "")
    {
      this.RemoveSearchTextFilter(callback, callbackData, transitionPage);
    }
    else
    {
      this.m_searchText = newSearchText;
      this.UpdateNonCardSearchTextFilters();
      base.ChangeSearchTextFilter(this.m_searchText, callback, callbackData, transitionPage);
    }
  }

  public override void RemoveSearchTextFilter(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool transitionPage = true)
  {
    this.m_searchText = (string) null;
    this.UpdateNonCardSearchTextFilters();
    base.RemoveSearchTextFilter(callback, callbackData, transitionPage);
  }

  private void UpdateNonCardSearchTextFilters()
  {
    CardBackManager.Get().SetSearchText(this.m_searchText);
    CoinManager.Get().SetSearchText(this.m_searchText);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
    {
      switch (collectibleDisplay.GetViewMode())
      {
        case CollectionUtils.ViewMode.HERO_SKINS:
          if (!this.IsSearching() && !CollectionManager.Get().IsInEditMode())
          {
            collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.HERO_PICKER, false, (CollectionUtils.ViewModeData) null);
            break;
          }
          collectibleDisplay.SetHeroSkinClass(new TAG_CLASS?());
          break;
        case CollectionUtils.ViewMode.HERO_PICKER:
          if (this.IsSearching())
          {
            collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.HERO_SKINS, false, (CollectionUtils.ViewModeData) null);
            break;
          }
          break;
      }
    }
    this.m_heroesCollection.FilterSearchText(this.m_searchText);
    this.m_heroesCollection.UpdateResults();
  }

  public bool IsSearching() => this.m_searchText != null;

  public string GetSearchText() => this.m_searchText;

  public void UpdateClassTabNewCardCounts()
  {
    foreach (CollectionClassTab classTab in this.m_classTabs)
    {
      TAG_CLASS tagClass = classTab.TabInfo.tagClass;
      classTab.UpdateNewItemCount(classTab.m_tabViewMode == CollectionUtils.ViewMode.DECK_TEMPLATE ? 0 : this.GetNumNewCardsForClass(tagClass));
    }
  }

  public int GetNumNewCardsForClass(TAG_CLASS tagClass) => this.m_classCardsCollection.GetNumNewCardsForTab(new CollectionTabInfo()
  {
    tagClass = tagClass
  });

  public override void NotifyOfCollectionChanged() => this.UpdateMassDisenchant();

  public void OnDoneEditingDeck()
  {
    this.RemoveAllClassFilters();
    this.UpdateCraftingModeButtonDustBottleVisibility(CollectionManager.Get().GetCardsToDisenchantCount());
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1"));
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2"));
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS"));
    NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS_NPR"));
    CollectionDeckTray.Get().GetCardsContent().HideDeckHelpPopup();
  }

  public void UpdateCraftingModeButtonDustBottleVisibility(int disenchantCount)
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    bool forceMobileActive = collectibleDisplay.GetViewMode() == CollectionUtils.ViewMode.CARDS;
    bool flag = (bool) UniversalInputManager.UsePhoneUI && collectibleDisplay.GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT;
    bool show = forceMobileActive | flag && disenchantCount > 0;
    CollectionManagerDisplay collectionManagerDisplay = collectibleDisplay as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectionManagerDisplay != (UnityEngine.Object) null))
      return;
    collectionManagerDisplay.m_craftingModeButton.ShowDustBottle(show, forceMobileActive);
  }

  public int GetMassDisenchantAmount() => CollectionManager.Get().GetCardsToDisenchantCount();

  public void LoadMassDisenchantScreen()
  {
    if ((UnityEngine.Object) this.m_massDisenchant != (UnityEngine.Object) null)
      return;
    this.m_massDisenchant = AssetLoader.Get().InstantiatePrefab((AssetReference) "MassDisenchant.prefab:0bfb8a7db15d748b291be3096753ca24").GetComponent<MassDisenchant>();
    this.m_massDisenchant.Hide();
  }

  public bool HasClassCardsAvailable(TAG_CLASS classTag) => this.m_classCardsCollection.GetNumPagesForTab(new CollectionTabInfo()
  {
    tagClass = classTag
  }) > 0;

  public bool HasAnyCardsAvailable() => this.m_classCardsCollection.GetTotalNumPages() > 0;

  public void ShowCraftingModeCards(
    BookPageManager.DelOnPageTransitionComplete callback = null,
    object callbackData = null,
    bool showUncraftable = false,
    bool showNormalOwned = false,
    bool showNormalMissing = false,
    bool showPremiumOwned = false,
    bool showPremiumMissing = false,
    bool updatePage = true,
    bool toggleChanged = false)
  {
    List<CollectibleCardFilter.FilterMask> filterMasks = new List<CollectibleCardFilter.FilterMask>();
    if (showNormalOwned)
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_NORMAL | CollectibleCardFilter.FilterMask.OWNED);
    if (showNormalMissing)
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_NORMAL | CollectibleCardFilter.FilterMask.UNOWNED);
    if (showPremiumOwned)
    {
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_GOLDEN | CollectibleCardFilter.FilterMask.OWNED);
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_DIAMOND | CollectibleCardFilter.FilterMask.OWNED);
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_SIGNATURE | CollectibleCardFilter.FilterMask.OWNED);
    }
    if (showPremiumMissing)
    {
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_GOLDEN | CollectibleCardFilter.FilterMask.UNOWNED);
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_DIAMOND | CollectibleCardFilter.FilterMask.UNOWNED);
      filterMasks.Add(CollectibleCardFilter.FilterMask.PREMIUM_SIGNATURE | CollectibleCardFilter.FilterMask.UNOWNED);
    }
    bool? isCraftable = new bool?();
    if (!showUncraftable)
      isCraftable = new bool?(true);
    this.m_cardsCollection.FilterOnlyOwned(false);
    this.m_cardsCollection.FilterByMask(filterMasks);
    this.m_cardsCollection.FilterByCraftability(isCraftable);
    this.m_cardsCollection.FilterLeagueBannedCardsSubset(RankMgr.Get().GetBannedCardsInCurrentLeague());
    this.UpdateFilteredCards();
    if (toggleChanged)
      this.m_lastCollectibleAnchor = (ICollectible) null;
    if (!updatePage)
      return;
    this.TransitionPageWhenReady(toggleChanged ? BookPageManager.PageTransitionType.MANY_PAGE_LEFT : BookPageManager.PageTransitionType.NONE, false, callback, callbackData);
  }

  protected override bool CanUserTurnPages()
  {
    if (CraftingManager.GetIsInCraftingMode() || SceneMgr.Get().IsInDuelsMode() && !PvPDungeonRunScene.IsEditingDeck())
      return false;
    CardBackInfoManager cardBackInfoManager = CardBackInfoManager.Get();
    if ((UnityEngine.Object) cardBackInfoManager != (UnityEngine.Object) null && cardBackInfoManager.IsPreviewing)
      return false;
    HeroSkinInfoManager heroSkinInfoManager = HeroSkinInfoManager.Get();
    return (!((UnityEngine.Object) heroSkinInfoManager != (UnityEngine.Object) null) || !heroSkinInfoManager.IsShowingPreview) && base.CanUserTurnPages();
  }

  private CollectionPageDisplay PageAsCollectionPage(BookPageDisplay page)
  {
    if (!(page is CollectionPageDisplay))
      Log.CollectionManager.PrintError("Page in CollectionPageManager is not a CollectionPageDisplay!  This should not happen!");
    return page as CollectionPageDisplay;
  }

  protected override bool ShouldShowTab(BookTab tab)
  {
    if (!this.m_initializedTabPositions)
      return true;
    if (this.m_hideNonDeckTemplateTabs)
      return tab.m_tabViewMode == CollectionUtils.ViewMode.DECK_TEMPLATE;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    bool flag = editedDeck != null;
    switch (tab.m_tabViewMode)
    {
      case CollectionUtils.ViewMode.CARDS:
        CollectionClassTab collectionClassTab = tab as CollectionClassTab;
        if ((UnityEngine.Object) collectionClassTab == (UnityEngine.Object) null)
        {
          Log.CollectionManager.PrintError("CollectionPageManager.ShouldShowTab passed a non-CollectionClassTab object.");
          return false;
        }
        TAG_CLASS tagClass = collectionClassTab.TabInfo.tagClass;
        if (this.HasClassCardsAvailable(tagClass))
          return true;
        return flag && !this.HasAnyCardsAvailable() && editedDeck.GetClasses().Contains(tagClass);
      case CollectionUtils.ViewMode.HERO_SKINS:
        if (SceneMgr.Get().IsInDuelsMode())
          return false;
        if (!flag)
          return this.HasAnyCardsAvailable();
        return !editedDeck.HasUIHeroOverride() && CollectionManager.Get().GetCountOfOwnedHeroesForClass(editedDeck.GetClass()) > 1;
      case CollectionUtils.ViewMode.CARD_BACKS:
        if (!flag)
          return this.HasAnyCardsAvailable();
        HashSet<int> cardBacksOwned = CardBackManager.Get().GetCardBacksOwned();
        return cardBacksOwned != null && cardBacksOwned.Count > 1;
      case CollectionUtils.ViewMode.DECK_TEMPLATE:
        return flag && !SceneMgr.Get().IsInTavernBrawlMode();
      case CollectionUtils.ViewMode.COINS:
        return !SceneMgr.Get().IsInDuelsMode() && !flag && this.HasAnyCardsAvailable();
      default:
        return true;
    }
  }

  private void SetupClassTab(
    CollectionClassTab classTab,
    TAG_CLASS classTag,
    string tabName,
    bool isTouch)
  {
    classTab.Init(classTag);
    classTab.transform.localScale = classTab.m_DeselectedLocalScale;
    classTab.transform.localEulerAngles = CollectiblePageManager.TAB_LOCAL_EULERS;
    classTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClassTabPressed));
    classTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver));
    classTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut));
    classTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver_Touch));
    classTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut_Touch));
    classTab.SetReceiveReleaseWithoutMouseDown(isTouch);
    classTab.gameObject.name = tabName;
  }

  protected override void SetUpBookTabs()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    bool flag = UniversalInputManager.Get().IsTouchMode();
    if ((UnityEngine.Object) this.m_deckTemplateTab != (UnityEngine.Object) null && this.m_deckTemplateTab.gameObject.activeSelf)
    {
      this.m_allTabs.Add((BookTab) this.m_deckTemplateTab);
      this.m_classTabs.Add(this.m_deckTemplateTab);
      this.m_deckTemplateTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeckTemplateTabPressed));
      this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver));
      this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut));
      this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver_Touch));
      this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut_Touch));
      this.m_deckTemplateTab.SetReceiveReleaseWithoutMouseDown(flag);
      this.m_tabVisibility[(BookTab) this.m_deckTemplateTab] = true;
    }
    for (int index = 0; index < CollectionPageManager.CLASS_TAB_ORDER.Length; ++index)
    {
      TAG_CLASS classTag = CollectionPageManager.CLASS_TAB_ORDER[index];
      CollectionClassTab collectionClassTab = (CollectionClassTab) GameUtils.Instantiate((Component) this.m_tabPrefab, this.m_tabContainer);
      this.SetupClassTab(collectionClassTab, classTag, classTag.ToString(), flag);
      this.m_allTabs.Add((BookTab) collectionClassTab);
      this.m_classTabs.Add(collectionClassTab);
      this.m_tabVisibility[(BookTab) collectionClassTab] = true;
      if (index <= 0)
        this.m_deselectedTabHalfWidth = collectionClassTab.GetComponent<BoxCollider>().bounds.extents.x;
    }
    if ((UnityEngine.Object) this.m_heroSkinsTab != (UnityEngine.Object) null)
    {
      this.m_heroSkinsTab.Init(TAG_CLASS.NEUTRAL);
      this.m_heroSkinsTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnHeroSkinsTabPressed));
      this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver));
      this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut));
      this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver_Touch));
      this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut_Touch));
      this.m_heroSkinsTab.SetReceiveReleaseWithoutMouseDown(flag);
      this.m_allTabs.Add((BookTab) this.m_heroSkinsTab);
      this.m_tabVisibility[(BookTab) this.m_heroSkinsTab] = true;
      this.m_heroSkinsTabPos = this.m_heroSkinsTab.transform.localPosition;
    }
    if ((UnityEngine.Object) this.m_cardBacksTab != (UnityEngine.Object) null)
    {
      this.m_cardBacksTab.Init(TAG_CLASS.NEUTRAL);
      this.m_cardBacksTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCardBacksTabPressed));
      this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver));
      this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut));
      this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver_Touch));
      this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut_Touch));
      this.m_cardBacksTab.SetReceiveReleaseWithoutMouseDown(flag);
      this.m_allTabs.Add((BookTab) this.m_cardBacksTab);
      this.m_tabVisibility[(BookTab) this.m_cardBacksTab] = true;
      this.m_cardBacksTabPos = this.m_cardBacksTab.transform.localPosition;
    }
    if ((UnityEngine.Object) this.m_coinsTab != (UnityEngine.Object) null)
    {
      this.m_coinsTab.Init(TAG_CLASS.NEUTRAL);
      this.m_coinsTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCoinsTabPressed));
      this.m_coinsTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver));
      this.m_coinsTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut));
      this.m_coinsTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver_Touch));
      this.m_coinsTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut_Touch));
      this.m_coinsTab.SetReceiveReleaseWithoutMouseDown(flag);
      this.m_allTabs.Add((BookTab) this.m_coinsTab);
      this.m_tabVisibility[(BookTab) this.m_coinsTab] = true;
      this.m_coinsTabPos = this.m_coinsTab.transform.localPosition;
    }
    this.PositionBookTabs(false);
    this.m_initializedTabPositions = true;
  }

  protected override void PositionBookTabs(bool animate)
  {
    Vector3 position = this.m_tabContainer.transform.position;
    int length = CollectionPageManager.CLASS_TAB_ORDER.Length;
    if ((UnityEngine.Object) this.m_deckTemplateTab != (UnityEngine.Object) null && this.m_deckTemplateTab.gameObject.activeSelf)
      ++length;
    for (int index = 0; index < length; ++index)
    {
      CollectionClassTab classTab = this.m_classTabs[index];
      Vector3 targetLocalPos;
      if (this.ShouldShowTab((BookTab) classTab))
      {
        classTab.SetTargetVisibility(true);
        position.x += this.m_spaceBetweenTabs;
        position.x += this.m_deselectedTabHalfWidth;
        targetLocalPos = this.m_tabContainer.transform.InverseTransformPoint(position);
        if ((UnityEngine.Object) classTab == (UnityEngine.Object) this.m_currentTab)
        {
          targetLocalPos.y = classTab.m_SelectedLocalYPos;
          targetLocalPos.z = classTab.GetOriginalLocalPosition().z;
        }
        position.x += this.m_deselectedTabHalfWidth;
      }
      else
      {
        classTab.SetTargetVisibility(false);
        targetLocalPos = classTab.transform.localPosition with
        {
          z = CollectiblePageManager.HIDDEN_TAB_LOCAL_Z_POS
        };
      }
      if (animate)
      {
        classTab.SetTargetLocalPosition(targetLocalPos);
      }
      else
      {
        classTab.SetIsVisible(classTab.ShouldBeVisible());
        classTab.transform.localPosition = targetLocalPos;
      }
    }
    this.PositionFixedTab(this.ShouldShowTab((BookTab) this.m_heroSkinsTab), (BookTab) this.m_heroSkinsTab, this.m_heroSkinsTabPos, animate);
    this.PositionFixedTab(this.ShouldShowTab((BookTab) this.m_cardBacksTab), (BookTab) this.m_cardBacksTab, this.m_cardBacksTabPos, animate);
    this.PositionFixedTab(this.ShouldShowTab((BookTab) this.m_coinsTab), (BookTab) this.m_coinsTab, this.m_coinsTabPos, animate);
    if (!animate)
      return;
    this.StopCoroutine(CollectiblePageManager.ANIMATE_TABS_COROUTINE_NAME);
    this.StartCoroutine(CollectiblePageManager.ANIMATE_TABS_COROUTINE_NAME);
  }

  private IEnumerator AnimateTabs()
  {
    CollectionPageManager collectionPageManager = this;
    bool playSounds = (UnityEngine.Object) HeroPickerDisplay.Get() == (UnityEngine.Object) null || !HeroPickerDisplay.Get().IsShown();
    List<CollectionClassTab> collectionClassTabList = new List<CollectionClassTab>();
    List<CollectionClassTab> tabsToShow = new List<CollectionClassTab>();
    List<CollectionClassTab> tabsToMove = new List<CollectionClassTab>();
    foreach (CollectionClassTab classTab in collectionPageManager.m_classTabs)
    {
      if (classTab.IsVisible() || classTab.ShouldBeVisible())
      {
        if (classTab.IsVisible() && classTab.ShouldBeVisible())
          tabsToMove.Add(classTab);
        else if (classTab.IsVisible() && !classTab.ShouldBeVisible())
          collectionClassTabList.Add(classTab);
        else
          tabsToShow.Add(classTab);
      }
    }
    collectionPageManager.m_tabsAreAnimating = true;
    if (collectionClassTabList.Count > 0)
    {
      foreach (CollectionClassTab tab in collectionClassTabList)
      {
        if (playSounds)
          SoundManager.Get().LoadAndPlay((AssetReference) "class_tab_retract.prefab:da79957be76b10343999d6fa92a6a2f0", tab.gameObject);
        yield return (object) new WaitForSeconds(0.03f);
        tab.AnimateToTargetPosition(0.1f, iTween.EaseType.easeOutQuad);
      }
      yield return (object) new WaitForSeconds(0.1f);
    }
    if (tabsToMove.Count > 0)
    {
      foreach (CollectionClassTab collectionClassTab in tabsToMove)
      {
        if (collectionClassTab.WillSlide() & playSounds)
          SoundManager.Get().LoadAndPlay((AssetReference) "class_tab_slides_across_top.prefab:04482bc6f531b76468ff92a5b4e979b6", collectionClassTab.gameObject);
        collectionClassTab.AnimateToTargetPosition(0.25f, iTween.EaseType.easeOutQuad);
      }
      yield return (object) new WaitForSeconds(0.25f);
    }
    if (tabsToShow.Count > 0)
    {
      foreach (CollectionClassTab collectionClassTab in tabsToShow)
      {
        if (playSounds)
          SoundManager.Get().LoadAndPlay((AssetReference) "class_tab_retract.prefab:da79957be76b10343999d6fa92a6a2f0", collectionClassTab.gameObject);
        collectionClassTab.AnimateToTargetPosition(0.4f, iTween.EaseType.easeOutBounce);
      }
      yield return (object) new WaitForSeconds(0.4f);
    }
    foreach (CollectionClassTab classTab in collectionPageManager.m_classTabs)
      classTab.SetIsVisible(classTab.ShouldBeVisible());
    collectionPageManager.m_tabsAreAnimating = false;
  }

  private void RemoveGhostingEffectForRuneCards()
  {
    foreach (CollectionCardVisual ghostedRuneCard in this.m_ghostedRuneCards)
    {
      Actor actor = ghostedRuneCard.GetActor();
      if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
        actor.GhostCardEffect(GhostCard.Type.NONE, actor.GetPremium());
    }
    this.m_ghostedRuneCards.Clear();
  }

  public void AddGhostedRuneCards(List<CollectionCardVisual> runeCards)
  {
    foreach (CollectionCardVisual runeCard in runeCards)
    {
      if (!this.m_ghostedRuneCards.Contains(runeCard))
        this.m_ghostedRuneCards.Add(runeCard);
    }
  }

  private void OnDeckTrayCardAdded(CollectionDeck deck, RunePattern cardRunesAdded)
  {
    this.m_deckRunesWereUpdatedOnCurrentPage = !deck.Runes.Matches(this.m_originalDeckRunesForCurrentPage);
    this.RemoveGhostingEffectForRuneCards();
    this.UpdatePageGhostingForInvalidRunes(deck.Runes);
  }

  private void OnCraftingTrayShown() => this.RemoveGhostingEffectForRuneCards();

  private void OnCraftingTrayHidden()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null || !editedDeck.HasClass(TAG_CLASS.DEATHKNIGHT))
      return;
    this.UpdatePageGhostingForInvalidRunes(editedDeck.Runes);
  }

  private void OnHideLockedRunesCheckboxToggled(bool isChecked)
  {
    CollectionPageManager.IsShowingLockedRuneCards = isChecked;
    if (CollectionPageManager.IsShowingLockedRuneCards)
    {
      if (!this.m_deckRunesWereUpdatedOnCurrentPage && !this.m_classCardsCollection.HasHiddenDeathKnightCards)
        return;
      this.m_classCardsCollection.UpdateResults();
      this.FlipToPage(1, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, BookPageManager.PageTransitionType.MANY_PAGE_LEFT);
    }
    else
      this.FlipToNextFilteredDeathKnightPage(BookPageManager.PageTransitionType.NONE);
  }

  public static bool IsShowingLockedRuneCards { get; private set; }

  private void UpdatePageGhostingForInvalidRunes(RunePattern runes)
  {
    CollectiblePageDisplay currentCollectiblePage = this.GetCurrentCollectiblePage();
    if ((UnityEngine.Object) currentCollectiblePage == (UnityEngine.Object) null)
      return;
    this.AddGhostedRuneCards(currentCollectiblePage.ApplyRuneCardGhostEffectsForCurrentPage(runes));
  }

  private void RuneIndicatorVisualOnRunePatternChanged(RunePattern currentDeckRunes)
  {
    this.m_deckRunesWereUpdatedOnCurrentPage = !currentDeckRunes.Matches(this.m_originalDeckRunesForCurrentPage);
    this.RemoveGhostingEffectForRuneCards();
    this.UpdatePageGhostingForInvalidRunes(currentDeckRunes);
  }

  private void SetCurrentClassTabInfo(CollectionTabInfo tabInfo)
  {
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if ((UnityEngine.Object) this.m_classFilterHeader == (UnityEngine.Object) null)
        Debug.LogError((object) "CollectionPageManager:SetCurrentClassTab: m_classFilterHeader should not be null when UniversalInputManager.UsePhoneUI is true");
      else if (!this.ShouldClassFilterBeVisible())
      {
        this.m_classFilterHeader.gameObject.SetActive(false);
      }
      else
      {
        this.m_classFilterHeader.gameObject.SetActive(true);
        this.m_classFilterHeader.SetMode(viewMode, new TAG_CLASS?(tabInfo.tagClass));
      }
    }
    else
    {
      CollectionClassTab collectionClassTab = (CollectionClassTab) null;
      switch (viewMode)
      {
        case CollectionUtils.ViewMode.CARDS:
          if (tabInfo.tagClass != TAG_CLASS.INVALID)
          {
            collectionClassTab = this.m_classTabs.Find((Predicate<CollectionClassTab>) (obj => obj.TabInfo.tagClass == tabInfo.tagClass && obj.m_tabViewMode != CollectionUtils.ViewMode.DECK_TEMPLATE));
            break;
          }
          break;
        case CollectionUtils.ViewMode.HERO_SKINS:
        case CollectionUtils.ViewMode.HERO_PICKER:
          bool flag1 = viewMode == CollectionUtils.ViewMode.HERO_PICKER;
          bool flag2 = this.IsSearching() || CollectionManager.Get().GetEditedDeck() != null;
          if (flag1 || flag2 && !flag1)
          {
            collectionClassTab = this.m_heroSkinsTab;
            break;
          }
          break;
        case CollectionUtils.ViewMode.CARD_BACKS:
          collectionClassTab = this.m_cardBacksTab;
          break;
        case CollectionUtils.ViewMode.COINS:
          collectionClassTab = this.m_coinsTab;
          break;
        default:
          collectionClassTab = (CollectionClassTab) null;
          break;
      }
      if ((UnityEngine.Object) collectionClassTab == (UnityEngine.Object) this.m_currentTab)
        return;
      this.DeselectCurrentTab();
      this.m_currentTab = (BookTab) collectionClassTab;
      if (!((UnityEngine.Object) this.m_currentTab != (UnityEngine.Object) null))
        return;
      this.StopCoroutine(CollectiblePageManager.SELECT_TAB_COROUTINE_NAME);
      this.StartCoroutine(CollectiblePageManager.SELECT_TAB_COROUTINE_NAME, (object) this.m_currentTab);
    }
  }

  public void SetDeckRuleset(DeckRuleset deckRuleset, bool refresh = false)
  {
    this.m_cardsCollection.SetDeckRuleset(deckRuleset);
    if (!refresh)
      return;
    this.UpdateFilteredCards();
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  private void OnClassTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((UnityEngine.Object) element == (UnityEngine.Object) null || (UnityEngine.Object) element == (UnityEngine.Object) this.m_currentTab)
      return;
    this.JumpToCollectionClassPage(element.TabInfo);
  }

  private void OnDeckTemplateTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.DECK_TEMPLATE);
  }

  private void OnHeroSkinsTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((UnityEngine.Object) element == (UnityEngine.Object) null || (UnityEngine.Object) element == (UnityEngine.Object) this.m_currentTab || !this.ShouldShowTab((BookTab) this.m_heroSkinsTab))
      return;
    CollectionPageDisplay currentCollectiblePage = this.GetCurrentCollectiblePage() as CollectionPageDisplay;
    if ((UnityEngine.Object) currentCollectiblePage != (UnityEngine.Object) null)
    {
      int pageFormatType = (int) currentCollectiblePage.m_pageFormatType;
    }
    if (this.IsSearching() || CollectionManager.Get().GetEditedDeck() != null)
      this.OnHeroClassButtonPressed(e);
    else
      CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.HERO_PICKER);
  }

  private void OnHeroClassButtonPressed(UIEvent e) => CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.HERO_SKINS);

  private void OnCardBacksTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((UnityEngine.Object) element == (UnityEngine.Object) null || (UnityEngine.Object) element == (UnityEngine.Object) this.m_currentTab)
      return;
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.CARD_BACKS);
  }

  private void OnCoinsTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((UnityEngine.Object) element == (UnityEngine.Object) null || (UnityEngine.Object) element == (UnityEngine.Object) this.m_currentTab || !this.ShouldShowTab((BookTab) this.m_coinsTab))
      return;
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.COINS);
  }

  public void UpdateMassDisenchant()
  {
    CraftingTray.Get()?.SetMassDisenchantAmount();
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return;
    int disenchantCount = 0;
    collectionManager.GetMassDisenchantCardsAndCount(this.m_disenchantCards, out disenchantCount);
    this.UpdateCraftingModeButtonDustBottleVisibility(disenchantCount);
    MassDisenchant massDisenchant = MassDisenchant.Get();
    if (!((UnityEngine.Object) massDisenchant != (UnityEngine.Object) null))
      return;
    massDisenchant.UpdateContents(this.m_disenchantCards);
  }

  public void JumpToCollectionClassPage(CollectionTabInfo pageTabInfo) => this.JumpToCollectionClassPage(pageTabInfo, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  public void JumpToCollectionClassPage(TAG_CLASS pageClass) => this.JumpToCollectionClassPage(new CollectionTabInfo()
  {
    tagClass = pageClass
  }, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  public void JumpToCollectionClassPage(
    CollectionTabInfo pageTabInfo,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.GetViewMode() != CollectionUtils.ViewMode.CARDS)
    {
      collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.CARDS, new CollectionUtils.ViewModeData()
      {
        m_setPageByClass = new TAG_CLASS?(pageTabInfo.tagClass)
      });
    }
    else
    {
      CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
      if (editedDeck != null && editedDeck.HasClass(TAG_CLASS.DEATHKNIGHT))
      {
        this.m_classCardsCollection.UpdateResults();
        this.m_deckRunesWereUpdatedOnCurrentPage = false;
      }
      int collectionPage;
      this.m_classCardsCollection.GetPageContentsForTab(pageTabInfo, 1, true, out collectionPage);
      this.FlipToPage(collectionPage, callback, callbackData);
    }
  }

  protected override void AssembleEmptyPageUI(BookPageDisplay page)
  {
    base.AssembleEmptyPageUI(page);
    this.AssembleEmptyPageUI(page as CollectiblePageDisplay, false);
  }

  protected override void AssembleEmptyPageUI(
    CollectiblePageDisplay page,
    bool displayNoMatchesText)
  {
    CollectionPageDisplay collectionPageDisplay = this.PageAsCollectionPage((BookPageDisplay) page);
    if ((UnityEngine.Object) collectionPageDisplay == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("Page in CollectionPageManager is not a CollectionPageDisplay!  This should not happen!");
    }
    else
    {
      collectionPageDisplay.SetClass(new CollectionTabInfo());
      bool showHints = CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.CARDS;
      collectionPageDisplay.ShowNoMatchesFound(displayNoMatchesText, this.m_classCardsCollection.FindCardsResult, showHints);
      if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.CARDS)
        this.DeselectCurrentTab();
      collectionPageDisplay.SetPageCountText(GameStrings.Get("GLUE_COLLECTION_EMPTY_PAGE"));
      collectionPageDisplay.SetPageTextColor();
    }
  }

  private void AssembleMassDisenchantPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    FormatType formatType)
  {
    CollectionPageDisplay page = this.PageAsCollectionPage(transitionReadyCallbackData.m_assembledPage);
    page.ActivatePageCountText(false);
    page.SetPageType(formatType);
    this.AssembleEmptyPageUI((CollectiblePageDisplay) page, false);
    this.SetHasPreviousAndNextPages(false, false);
    page.SetMassDisenchant();
    CollectionManager.Get().GetCollectibleDisplay().CollectionPageContentsChanged<ICollectible>((ICollection<ICollectible>) null, (CollectibleDisplay.CollectionActorsReadyCallback) ((actorList, nonActorCollectibleList, data) =>
    {
      page.UpdatePageWithMassDisenchant();
      this.TransitionPage((object) transitionReadyCallbackData);
    }), (object) null);
  }

  private List<CollectibleCard> GetFilteredDeathKnightCards<TCollectible>(
    ICollection<TCollectible> collectiblesToDisplay)
  {
    if (!(collectiblesToDisplay is List<CollectibleCard> deathKnightCards1))
      return (List<CollectibleCard>) null;
    if (deathKnightCards1.Count == 0)
      return deathKnightCards1;
    if (deathKnightCards1[0].GetEntityDef().GetClass() != TAG_CLASS.DEATHKNIGHT)
      return (List<CollectibleCard>) null;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return (List<CollectibleCard>) null;
    List<CollectibleCard> deathKnightCards2 = new List<CollectibleCard>();
    RunePattern runesToAdd = new RunePattern();
    foreach (CollectibleCard collectibleCard in deathKnightCards1)
    {
      runesToAdd.SetCostsFromEntity((EntityBase) collectibleCard.GetEntityDef());
      if (editedDeck.CanAddRunes(runesToAdd, DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
        deathKnightCards2.Add(collectibleCard);
    }
    return deathKnightCards2;
  }

  protected override bool AssembleCollectiblePage<TCollectible>(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    ICollection<TCollectible> collectiblesToDisplay,
    int totalNumPages)
  {
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    CollectionDeck editedDeck1 = CollectionManager.Get().GetEditedDeck();
    if (this.m_currentClassContext.tagClass == TAG_CLASS.DEATHKNIGHT && editedDeck1 != null)
      this.m_originalDeckRunesForCurrentPage = editedDeck1.Runes;
    if (base.AssembleCollectiblePage<TCollectible>(transitionReadyCallbackData, collectiblesToDisplay, totalNumPages))
    {
      if (!CollectionManager.Get().IsInEditMode() || viewMode != CollectionUtils.ViewMode.CARDS || editedDeck1 == null)
        return true;
      List<TAG_CLASS> classes = editedDeck1.GetClasses();
      if (classes.Count <= 0)
        return true;
      TAG_CLASS tagClass = classes[0];
      this.m_currentClassContext = new CollectionTabInfo()
      {
        tagClass = tagClass
      };
      this.SetCurrentClassTabInfo(this.m_currentClassContext);
      return true;
    }
    CollectionPageDisplay page = this.PageAsCollectionPage(transitionReadyCallbackData.m_assembledPage);
    this.m_lastCollectibleAnchor = (ICollectible) collectiblesToDisplay.FirstOrDefault<TCollectible>();
    if (viewMode == CollectionUtils.ViewMode.HERO_SKINS)
    {
      CollectionDeck editedDeck2 = CollectionManager.Get().GetEditedDeck();
      if (editedDeck2 != null)
      {
        page.SetHeroSkins(new TAG_CLASS?(editedDeck2.GetClass()));
      }
      else
      {
        CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
        TAG_CLASS? classTag = (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null ? collectibleDisplay.GetHeroSkinClass() : new TAG_CLASS?();
        page.SetHeroSkins(classTag);
      }
    }
    else if (viewMode == CollectionUtils.ViewMode.COINS)
    {
      page.SetCoins();
    }
    else
    {
      CollectionTabInfo currentTabInfoFromPage = this.m_classCardsCollection.GetCurrentTabInfoFromPage(this.m_currentPageNum);
      page.SetClass(currentTabInfoFromPage);
      this.m_currentClassContext = currentTabInfoFromPage;
    }
    this.m_deckRunesWereUpdatedOnCurrentPage = false;
    page.SetPageCountText(GameStrings.Format("GLUE_COLLECTION_PAGE_NUM", (object) this.m_currentPageNum));
    page.SetPageTextColor();
    page.ShowNoMatchesFound(false, (CollectionManager.FindCardsResult) null, true);
    this.SetHasPreviousAndNextPages(this.m_currentPageNum > 1, this.m_currentPageNum < totalNumPages);
    CollectionManager.Get().GetCollectibleDisplay().CollectionPageContentsChanged<TCollectible>(collectiblesToDisplay, (CollectibleDisplay.CollectionActorsReadyCallback) ((actorList, nonActorCollectibleList, data) =>
    {
      page.UpdateCollectionItems(actorList, nonActorCollectibleList, viewMode);
      this.TransitionPageNextFrame(transitionReadyCallbackData);
      if (!((UnityEngine.Object) this.m_deckTemplatePicker != (UnityEngine.Object) null))
        return;
      this.StartCoroutine(this.m_deckTemplatePicker.Show(false));
    }), (object) null);
    return true;
  }

  private void AssembleDeckTemplatePage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData)
  {
    FormatType formatType = !((UnityEngine.Object) this.m_deckTemplatePicker != (UnityEngine.Object) null) || this.m_deckTemplatePicker.CurrentSelectedFormat == FormatType.FT_UNKNOWN ? FormatType.FT_STANDARD : this.m_deckTemplatePicker.CurrentSelectedFormat;
    if (this.AssembleCollectionBasePage(transitionReadyCallbackData, false, formatType))
      return;
    CollectionPageDisplay collectionPageDisplay = this.PageAsCollectionPage(transitionReadyCallbackData.m_assembledPage);
    if ((UnityEngine.Object) this.m_deckTemplatePicker == (UnityEngine.Object) null && !string.IsNullOrEmpty(this.m_deckTemplatePickerPrefab))
    {
      this.m_deckTemplatePicker = GameUtils.LoadGameObjectWithComponent<DeckTemplatePicker>(this.m_deckTemplatePickerPrefab);
      if ((UnityEngine.Object) this.m_deckTemplatePicker == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) ("Failed to instantiate deck template picker prefab " + this.m_deckTemplatePickerPrefab));
        return;
      }
      this.m_deckTemplatePicker.RegisterOnTemplateDeckChosen((DeckTemplatePicker.OnTemplateDeckChosen) (() =>
      {
        this.HideNonDeckTemplateTabs(false, true);
        CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.CARDS);
      }));
    }
    collectionPageDisplay.UpdateDeckTemplateHeader(this.m_deckTemplatePicker?.m_pageHeader, formatType);
    collectionPageDisplay.UpdateDeckTemplatePage((Component) this.m_deckTemplatePicker);
    collectionPageDisplay.SetDeckTemplates();
    collectionPageDisplay.ShowNoMatchesFound(false, (CollectionManager.FindCardsResult) null, true);
    collectionPageDisplay.SetPageCountText(string.Empty);
    this.SetHasPreviousAndNextPages(false, false);
    this.UpdateDeckTemplate(this.m_deckTemplatePicker);
    this.TransitionPage((object) transitionReadyCallbackData);
  }

  public DeckTemplatePicker GetDeckTemplatePicker() => this.m_deckTemplatePicker;

  public void UpdateDeckTemplate(DeckTemplatePicker deckTemplatePicker)
  {
    if (!((UnityEngine.Object) deckTemplatePicker != (UnityEngine.Object) null))
      return;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck != null)
      deckTemplatePicker.SetDeckFormatAndClass(editedDeck.FormatType, editedDeck.GetClass());
    this.StartCoroutine(deckTemplatePicker.Show(true));
  }

  private void AssembleCardBackPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    if (!useCurrentPageNum)
      this.m_currentPageNum = 1;
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1} currentPageIsPageA={2} currentPageNum={3}", (object) this.m_transitionPageId, (object) this.m_pagesCurrentlyTurning, (object) this.m_currentPageIsPageA, (object) this.m_currentPageNum);
    int count = this.GetCurrentDeckTrayModeCardBackIds().Count;
    bool emptyPage = count == 0;
    if (this.AssembleCollectionBasePage(transitionReadyCallbackData, emptyPage, FormatType.FT_STANDARD))
      return;
    CollectionPageDisplay page = this.PageAsCollectionPage(transitionReadyCallbackData.m_assembledPage);
    int maxCardsPerPage = CollectiblePageDisplay.GetMaxCardsPerPage();
    int max = count / maxCardsPerPage + (count % maxCardsPerPage > 0 ? 1 : 0);
    this.m_currentPageNum = Mathf.Clamp(this.m_currentPageNum, 1, max);
    page.SetCardBacks();
    page.ShowNoMatchesFound(count == 0, (CollectionManager.FindCardsResult) null, true);
    page.SetPageCountText(GameStrings.Format("GLUE_COLLECTION_PAGE_NUM", (object) this.m_currentPageNum));
    this.SetHasPreviousAndNextPages(this.m_currentPageNum > 1, this.m_currentPageNum < max);
    bool flag = !CollectionManager.Get().IsInEditMode();
    List<CardBackManager.OwnedCardBack> pageOfCardBacks = CardBackManager.Get()?.GetPageOfCardBacks(!flag, this.m_currentPageNum);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.CollectionPageContentsChangedToCardBacks(pageOfCardBacks, (CollectibleDisplay.CollectionActorsReadyCallback) ((actorList, nonActorCollectibleList, data) =>
    {
      page.UpdateCollectionItems(actorList, nonActorCollectibleList, CollectionUtils.ViewMode.CARD_BACKS);
      foreach (CollectionCardActors actor in actorList)
        CardBackManager.Get().UpdateCardBackWithInternalCardBack(actor.GetPreferredActor());
      this.TransitionPage((object) transitionReadyCallbackData);
      if (!((UnityEngine.Object) this.m_deckTemplatePicker != (UnityEngine.Object) null))
        return;
      this.StartCoroutine(this.m_deckTemplatePicker.Show(false));
    }));
  }

  protected void AssembleHeroPickerPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData)
  {
    CollectionPageDisplay page = this.PageAsCollectionPage(transitionReadyCallbackData.m_assembledPage);
    page.ActivatePageCountText(false);
    page.SetPageType(FormatType.FT_STANDARD);
    this.AssembleEmptyPageUI((CollectiblePageDisplay) page, false);
    MassDisenchant massDisenchant = MassDisenchant.Get();
    if ((UnityEngine.Object) massDisenchant != (UnityEngine.Object) null)
      massDisenchant.Hide();
    this.SetHasPreviousAndNextPages(false, false);
    page.SetHeroPicker();
    page.SetPageTextColor();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      return;
    this.m_heroesCollection.SortResults();
    this.m_heroesCollection.FilterHeroesByActiveClass();
    collectibleDisplay.SetHeroSkinClass(new TAG_CLASS?());
    collectibleDisplay.CollectionPageContentsChanged<ICollectible>((ICollection<ICollectible>) null, (CollectibleDisplay.CollectionActorsReadyCallback) ((actorList, nonActorCollectibles, data) =>
    {
      int[] allHeroCounts;
      int[] ownedHeroCounts;
      this.CountClassHeroTotals(out allHeroCounts, out ownedHeroCounts);
      page.UpdatePageWithHeroPicker(allHeroCounts, ownedHeroCounts);
      this.TransitionPage((object) transitionReadyCallbackData);
    }), (object) null);
  }

  protected void CountClassHeroTotals(out int[] allHeroCounts, out int[] ownedHeroCounts)
  {
    List<TAG_CLASS> tagClassList = new List<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES);
    allHeroCounts = new int[tagClassList.Count];
    ownedHeroCounts = new int[tagClassList.Count];
    List<CollectibleCard> allResults = this.m_heroesCollection.GetAllResults();
    for (int index1 = 0; index1 < allResults.Count; ++index1)
    {
      CollectibleCard collectibleCard = allResults[index1];
      TAG_CLASS tagClass = collectibleCard.Class;
      int index2 = tagClassList.IndexOf(tagClass);
      ++allHeroCounts[index2];
      if (collectibleCard.OwnedCount >= 1)
        ++ownedHeroCounts[index2];
    }
  }

  protected void AssembleHeroSkinsPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    this.m_heroesCollection.FilterHeroesByActiveClass();
    if (!useCurrentPageNum)
      this.m_currentPageNum = 1;
    this.m_heroesCollection.GetHeroesContents(this.m_currentPageNum);
    List<CollectibleCard> heroesContents = this.m_heroesCollection.GetHeroesContents(this.m_currentPageNum);
    this.AssembleCollectiblePage<CollectibleCard>(transitionReadyCallbackData, (ICollection<CollectibleCard>) heroesContents, this.m_heroesCollection.GetTotalNumPages());
  }

  protected void AssembleCoinPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    if (!useCurrentPageNum)
      this.m_currentPageNum = 1;
    int maxCardsPerPage = CollectiblePageDisplay.GetMaxCardsPerPage();
    List<CollectibleCard> pageOfCoinCards = CoinManager.Get().GetPageOfCoinCards(this.m_currentPageNum, maxCardsPerPage);
    this.AssembleCollectiblePage<CollectibleCard>(transitionReadyCallbackData, (ICollection<CollectibleCard>) pageOfCoinCards, CoinManager.Get().GetCoinPageCount(maxCardsPerPage));
  }

  protected override void AssemblePage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    if ((UnityEngine.Object) null == (UnityEngine.Object) collectibleDisplay)
      return;
    switch (collectibleDisplay.GetViewMode())
    {
      case CollectionUtils.ViewMode.CARDS:
        if (this.m_classCardsCollection == null)
          break;
        List<CollectibleCard> collectibleCardList = new List<CollectibleCard>();
        List<CollectibleCard> collectiblesToDisplay;
        if (useCurrentPageNum)
          collectiblesToDisplay = this.m_classCardsCollection.GetPageContents(this.m_currentPageNum);
        else if (!(this.m_lastCollectibleAnchor is CollectibleCard collectibleAnchor))
        {
          this.m_currentPageNum = 1;
          collectiblesToDisplay = this.m_cardsCollection.GetPageContents(this.m_currentPageNum);
        }
        else
        {
          int collectionPage;
          collectiblesToDisplay = this.m_classCardsCollection.GetPageContentsForCard(collectibleAnchor.CardId, collectibleAnchor.PremiumType, out collectionPage, this.m_currentClassContext);
          if (collectiblesToDisplay.Count == 0)
            collectiblesToDisplay = this.m_classCardsCollection.GetPageContentsForTab(this.m_currentClassContext, 1, true, out collectionPage);
          if (collectiblesToDisplay.Count == 0)
          {
            collectiblesToDisplay = this.m_cardsCollection.GetPageContents(1);
            collectionPage = 1;
          }
          this.m_currentPageNum = collectiblesToDisplay.Count == 0 ? 0 : collectionPage;
        }
        if (collectiblesToDisplay.Count == 0)
        {
          int collectionPage;
          collectiblesToDisplay = this.m_cardsCollection.GetFirstNonEmptyPage(out collectionPage);
          if (collectiblesToDisplay.Count > 0)
            this.m_currentPageNum = collectionPage;
        }
        this.AssembleCollectiblePage<CollectibleCard>(transitionReadyCallbackData, (ICollection<CollectibleCard>) collectiblesToDisplay, this.m_cardsCollection.GetTotalNumPages());
        CollectionManagerDisplay collectionManagerDisplay = collectibleDisplay as CollectionManagerDisplay;
        if ((UnityEngine.Object) collectionManagerDisplay != (UnityEngine.Object) null)
          collectionManagerDisplay.SetRuneLockedCheckboxVisible(CollectionManager.Get().IsEditingDeathKnightDeck());
        if (CollectionManager.Get().GetCollectibleDisplay().InCraftingMode())
          break;
        CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
        if (editedDeck == null || !editedDeck.HasClass(TAG_CLASS.DEATHKNIGHT))
          break;
        this.UpdatePageGhostingForInvalidRunes(editedDeck.Runes);
        break;
      case CollectionUtils.ViewMode.HERO_SKINS:
        this.AssembleHeroSkinsPage(transitionReadyCallbackData, useCurrentPageNum);
        break;
      case CollectionUtils.ViewMode.CARD_BACKS:
        this.AssembleCardBackPage(transitionReadyCallbackData, useCurrentPageNum);
        break;
      case CollectionUtils.ViewMode.DECK_TEMPLATE:
        this.AssembleDeckTemplatePage(transitionReadyCallbackData);
        break;
      case CollectionUtils.ViewMode.MASS_DISENCHANT:
        FormatType themeShowing = CollectionManager.Get().GetThemeShowing();
        this.AssembleMassDisenchantPage(transitionReadyCallbackData, themeShowing);
        break;
      case CollectionUtils.ViewMode.COINS:
        this.AssembleCoinPage(transitionReadyCallbackData, useCurrentPageNum);
        break;
      case CollectionUtils.ViewMode.HERO_PICKER:
        this.AssembleHeroPickerPage(transitionReadyCallbackData);
        break;
    }
  }

  protected override void UpdateFilteredCards()
  {
    base.UpdateFilteredCards();
    this.UpdateClassTabNewCardCounts();
  }

  protected override void TransitionPage(object callbackData)
  {
    base.TransitionPage(callbackData);
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT)
      this.DeselectCurrentTab();
    else
      this.SetCurrentClassTabInfo(this.m_currentClassContext);
  }

  protected override void PageRight(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    if (!CollectionPageManager.IsEditingDeathKnightDeck(out RunePattern _))
      base.PageRight(callback, callbackData);
    else if (CollectionPageManager.IsShowingLockedRuneCards)
    {
      base.PageRight(callback, callbackData);
    }
    else
    {
      if (this.m_currentClassContext.tagClass == TAG_CLASS.DEATHKNIGHT)
      {
        if (this.m_deckRunesWereUpdatedOnCurrentPage)
        {
          this.FlipToNextFilteredDeathKnightPage(BookPageManager.PageTransitionType.MANY_PAGE_RIGHT);
          return;
        }
      }
      else if (this.IsRightPageInDeathKnightTab())
      {
        this.m_classCardsCollection.UpdateResults();
        int collectionPage;
        this.m_classCardsCollection.GetPageContentsForTab(new CollectionTabInfo()
        {
          tagClass = TAG_CLASS.DEATHKNIGHT
        }, 1, true, out collectionPage);
        this.FlipToPage(collectionPage, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, BookPageManager.PageTransitionType.MANY_PAGE_RIGHT);
        return;
      }
      base.PageRight(callback, callbackData);
    }
  }

  protected override void PageLeft(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    if (!CollectionPageManager.IsEditingDeathKnightDeck(out RunePattern _))
      base.PageLeft(callback, callbackData);
    else if (CollectionPageManager.IsShowingLockedRuneCards)
    {
      base.PageLeft(callback, callbackData);
    }
    else
    {
      if (this.m_currentClassContext.tagClass == TAG_CLASS.DEATHKNIGHT)
      {
        if (this.m_deckRunesWereUpdatedOnCurrentPage)
        {
          this.FlipToNextFilteredDeathKnightPage(BookPageManager.PageTransitionType.MANY_PAGE_LEFT);
          return;
        }
      }
      else if (this.IsLeftPageInDeathKnightTab())
      {
        this.m_classCardsCollection.UpdateResults();
        int collectionPage;
        this.m_classCardsCollection.GetPageContentsForTab(this.m_currentClassContext, 1, true, out collectionPage);
        this.FlipToPage(collectionPage - 1, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, BookPageManager.PageTransitionType.MANY_PAGE_LEFT);
        return;
      }
      base.PageLeft(callback, callbackData);
    }
  }

  public void ShowRuneCardPopupForTutorial()
  {
    CollectibleCard firstRuneCard = this.m_classCardsCollection.GetFirstRuneCard();
    if (firstRuneCard == null)
    {
      Debug.LogWarning((object) "CollectionPageManager.ShowRuneCardPopupForTutorial: There is no valid rune card.");
    }
    else
    {
      int collectionPage;
      this.m_classCardsCollection.GetPageContentsForCard(firstRuneCard.CardId, firstRuneCard.PremiumType, out collectionPage, new CollectionTabInfo()
      {
        tagClass = TAG_CLASS.DEATHKNIGHT
      });
      if (collectionPage != this.m_currentPageNum)
        this.FlipToPage(collectionPage, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
      CollectionCardVisual cardVisual = this.GetCurrentCollectiblePage().GetCardVisual(firstRuneCard.CardId, firstRuneCard.PremiumType);
      Vector3 runeBannerPosition = cardVisual.GetRuneBannerPosition();
      runeBannerPosition.x += (bool) UniversalInputManager.UsePhoneUI ? 14f : 13f;
      this.m_deathKnightRuneTutorialRunePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, runeBannerPosition, 15f * Vector3.one, GameStrings.Get("GLOBAL_RUNE_REQUIREMENT_POPUP_TEXT"));
      this.m_deathKnightRuneTutorialRunePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
      cardVisual.SetRuneBannerHighlighted(true);
    }
  }

  public void ShowRuneIndicatorArrowForTutorial()
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    Vector3 indicatorButtonPosition = collectionDeckTray.GetFirstRuneIndicatorButtonPosition();
    float scaleFactor;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      indicatorButtonPosition.x += -9f;
      scaleFactor = 7f;
    }
    else
    {
      indicatorButtonPosition.x += -6f;
      scaleFactor = 7f;
    }
    this.m_runeIndicatorArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, indicatorButtonPosition, Vector3.down * 90f, false, scaleFactor);
    collectionDeckTray.SetRuneIndicatorHighlighted(true);
  }

  public void DismissRuneCardPopupForTutorial()
  {
    NotificationManager.Get().DestroyNotification(this.m_deathKnightRuneTutorialRunePopup, 0.0f);
    CollectibleCard firstRuneCard = this.m_classCardsCollection.GetFirstRuneCard();
    if (firstRuneCard == null)
      Debug.LogWarning((object) "CollectionPageManager.ShowRuneCardPopupForTutorial: There is no valid rune card.");
    else
      this.GetCurrentCollectiblePage().GetCardVisual(firstRuneCard.CardId, firstRuneCard.PremiumType).SetRuneBannerHighlighted(false);
  }

  public void DismissRuneIndicatorArrowForTutorial()
  {
    NotificationManager.Get().DestroyNotification(this.m_runeIndicatorArrow, 0.0f);
    CollectionDeckTray.Get().SetRuneIndicatorHighlighted(false);
  }

  private void FlipToNextFilteredDeathKnightPage(BookPageManager.PageTransitionType transitionType)
  {
    if (this.m_lastCollectibleAnchor is CollectibleCard collectibleAnchor)
    {
      int collectionPage;
      List<CollectibleCard> pageContentsForCard1 = this.m_classCardsCollection.GetPageContentsForCard(collectibleAnchor.CardId, collectibleAnchor.PremiumType, out collectionPage, this.m_currentClassContext);
      if (pageContentsForCard1.Count > 0)
      {
        this.m_classCardsCollection.UpdateResults();
        List<CollectibleCard> pageContentsForCard2 = this.m_classCardsCollection.GetPageContentsForCard(collectibleAnchor.CardId, collectibleAnchor.PremiumType, out collectionPage, this.m_currentClassContext);
        int newPageNum = collectionPage;
        if (pageContentsForCard2.Count > 0)
        {
          switch (transitionType)
          {
            case BookPageManager.PageTransitionType.MANY_PAGE_RIGHT:
              ++newPageNum;
              break;
            case BookPageManager.PageTransitionType.MANY_PAGE_LEFT:
              --newPageNum;
              break;
          }
          this.FlipToPage(newPageNum, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, transitionType);
          return;
        }
        CollectibleCard card = transitionType != BookPageManager.PageTransitionType.MANY_PAGE_LEFT ? this.m_classCardsCollection.GetNextValidDeathKnightCardRight(collectibleAnchor) : this.m_classCardsCollection.GetNextValidDeathKnightCardLeft(collectibleAnchor);
        if (card != null)
        {
          int pageNumberForCard = this.m_classCardsCollection.GetPageNumberForCard(card, this.m_currentClassContext);
          if (pageContentsForCard1.Contains(card))
          {
            switch (transitionType)
            {
              case BookPageManager.PageTransitionType.MANY_PAGE_RIGHT:
                ++pageNumberForCard;
                break;
              case BookPageManager.PageTransitionType.MANY_PAGE_LEFT:
                --pageNumberForCard;
                break;
            }
          }
          this.FlipToPage(pageNumberForCard, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, transitionType);
          return;
        }
        if (transitionType == BookPageManager.PageTransitionType.MANY_PAGE_LEFT)
        {
          int firstPageForTab = this.m_classCardsCollection.GetFirstPageForTab(this.m_currentClassContext);
          if (firstPageForTab > 0)
          {
            this.FlipToPage(firstPageForTab - 1, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, transitionType);
            return;
          }
        }
        else
        {
          int lastPageForTab = this.m_classCardsCollection.GetLastPageForTab(this.m_currentClassContext);
          if (lastPageForTab > 0)
          {
            this.FlipToPage(lastPageForTab + 1, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, transitionType);
            return;
          }
        }
      }
    }
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.MANY_PAGE_LEFT, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
    this.m_deckRunesWereUpdatedOnCurrentPage = false;
  }

  private bool IsLeftPageInDeathKnightTab() => this.m_classCardsCollection.GetCurrentTabInfoFromPage(this.m_currentPageNum - 1).tagClass == TAG_CLASS.DEATHKNIGHT;

  private bool IsRightPageInDeathKnightTab() => this.m_classCardsCollection.GetCurrentTabInfoFromPage(this.m_currentPageNum + 1).tagClass == TAG_CLASS.DEATHKNIGHT;

  private static bool IsEditingDeathKnightDeck(out RunePattern deckRunes)
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    bool flag = editedDeck != null && editedDeck.HasClass(TAG_CLASS.DEATHKNIGHT);
    deckRunes = flag ? editedDeck.Runes : new RunePattern();
    return flag;
  }

  protected override void OnPageTransitionRequested()
  {
    ++this.m_numPageFlipsThisSession;
    int num = Options.Get().GetInt(Option.PAGE_MOUSE_OVERS);
    int val = num + 1;
    if (num < this.m_numPlageFlipsBeforeStopShowingArrows)
      Options.Get().SetInt(Option.PAGE_MOUSE_OVERS, val);
    this.ShowSetFilterTutorialIfNeeded();
  }

  protected override void OnPageTurnComplete(object callbackData, int operationId)
  {
    if (this.m_numPageFlipsThisSession % CollectiblePageManager.NUM_PAGE_FLIPS_UNTIL_UNLOAD_UNUSED_ASSETS == 0)
    {
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
        hearthstoneApplication.UnloadUnusedAssets();
    }
    CollectionPageDisplay collectionPageDisplay = this.PageAsCollectionPage((callbackData as BookPageManager.TransitionReadyCallbackData).m_otherPage);
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    int num = (UnityEngine.Object) collectionPageDisplay != (UnityEngine.Object) this.PageAsCollectionPage(this.GetCurrentPage()) ? 1 : 0;
    if (num != 0 || viewMode != CollectionUtils.ViewMode.HERO_SKINS)
      collectionPageDisplay.HideHeroSkinsDecor();
    if (num != 0 || viewMode != CollectionUtils.ViewMode.HERO_PICKER)
      collectionPageDisplay.HideHeroPicker();
    if (num != 0 || viewMode != CollectionUtils.ViewMode.COINS)
      collectionPageDisplay.HideFavoriteBanner();
    base.OnPageTurnComplete(callbackData, operationId);
  }

  private void ShowSetFilterTutorialIfNeeded()
  {
    if (Options.Get().GetBool(Option.HAS_SEEN_SET_FILTER_TUTORIAL) || CollectionManager.Get().IsInEditMode() || CollectionManager.Get().GetCollectibleDisplay().GetViewMode() != CollectionUtils.ViewMode.CARDS || !this.m_cardsCollection.CardSetFilterIsAllStandardSets())
      return;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null || collectibleDisplay.IsShowingSetFilterTray() || !CollectionManager.Get().AccountHasWildCards() || !RankMgr.Get().WildCardsAllowedInCurrentLeague() || this.m_numPageFlipsThisSession < CollectionPageManager.NUM_PAGE_FLIPS_BEFORE_SET_FILTER_TUTORIAL || !((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.ShowSetFilterTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
    Options.Get().SetBool(Option.HAS_SEEN_SET_FILTER_TUTORIAL, true);
  }

  protected override void OnCollectionManagerViewModeChanged(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse)
  {
    if (!triggerResponse)
      return;
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1} mode={2}-->{3} triggerResponse={4}", (object) this.m_transitionPageId, (object) this.m_pagesCurrentlyTurning, (object) prevMode, (object) mode, (object) triggerResponse);
    this.UpdateCraftingModeButtonDustBottleVisibility(CollectionManager.Get().GetCardsToDisenchantCount());
    if (mode == CollectionUtils.ViewMode.DECK_TEMPLATE)
      this.HideNonDeckTemplateTabs(true);
    if (mode != CollectionUtils.ViewMode.CARDS)
      CollectionDeckTray.Get().GetCardsContent().HideDeckHelpPopup();
    if (mode != CollectionUtils.ViewMode.HERO_SKINS)
    {
      CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
      if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
        collectibleDisplay.SetHeroSkinClass(new TAG_CLASS?());
    }
    this.m_currentPageNum = 1;
    if (userdata != null)
    {
      if (userdata.m_setPageByClass.HasValue)
      {
        TAG_CLASS tagClass = (TAG_CLASS) ((int) userdata.m_setPageByClass ?? 0);
        this.m_classCardsCollection.GetPageContentsForTab(new CollectionTabInfo()
        {
          tagClass = tagClass
        }, 1, true, out this.m_currentPageNum);
      }
      else if (userdata.m_setPageByCard != null)
        this.m_classCardsCollection.GetPageContentsForCard(userdata.m_setPageByCard, userdata.m_setPageByPremium, out this.m_currentPageNum, this.m_currentClassContext);
    }
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < CollectionPageManager.TAG_ORDERING.Length; ++index)
    {
      if (prevMode == CollectionPageManager.TAG_ORDERING[index])
        num1 = index;
      if (mode == CollectionPageManager.TAG_ORDERING[index])
        num2 = index;
    }
    BookPageManager.PageTransitionType transition = num2 - num1 < 0 ? BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT : BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT;
    BookPageManager.DelOnPageTransitionComplete callback = (BookPageManager.DelOnPageTransitionComplete) null;
    object callbackData = (object) null;
    if (userdata != null)
    {
      callback = userdata.m_pageTransitionCompleteCallback;
      callbackData = userdata.m_pageTransitionCompleteData;
    }
    if (this.m_turnPageCoroutine != null)
      this.StopCoroutine(this.m_turnPageCoroutine);
    CollectionDeckTray.Get().m_decksContent.UpdateDeckName();
    CollectionDeckTray.Get().UpdateDoneButtonText();
    this.m_turnPageCoroutine = this.StartCoroutine(this.ViewModeChangedWaitToTurnPage(transition, prevMode == CollectionUtils.ViewMode.DECK_TEMPLATE, callback, callbackData));
  }

  private IEnumerator ViewModeChangedWaitToTurnPage(
    BookPageManager.PageTransitionType transition,
    bool hideDeckTemplateBottomPanel,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    CollectionPageManager collectionPageManager = this;
    if ((UnityEngine.Object) collectionPageManager.m_deckTemplatePicker != (UnityEngine.Object) null && hideDeckTemplateBottomPanel)
    {
      CollectionManager.Get().GetCollectibleDisplay().m_inputBlocker.gameObject.SetActive(true);
      collectionPageManager.m_deckTemplatePicker.ShowBottomPanel(false);
      while (collectionPageManager.m_deckTemplatePicker.IsShowingBottomPanel())
        yield return (object) null;
      yield return (object) collectionPageManager.StartCoroutine(collectionPageManager.m_deckTemplatePicker.ShowPacks(false));
      CollectionManager.Get().GetCollectibleDisplay().m_inputBlocker.gameObject.SetActive(false);
    }
    collectionPageManager.TransitionPageWhenReady(transition, true, callback, callbackData);
  }

  public void OnFavoriteHeroChanged(
    TAG_CLASS heroClass,
    NetCache.CardDefinition favoriteHero,
    bool isFavorite,
    object userData)
  {
    this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteHeroSkins(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());
  }

  public void OnFavoriteCardBackChanged(int newFavoriteCardBackID, bool isFavorite) => this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteCardBacks(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());

  public void OnFavoriteBattlegroundsGuideSkinChanged(
    BattlegroundsGuideSkinId? newFavoriteBattlegroundsGuideSkinID)
  {
    this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteBattlegroundsGuideSkin(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());
  }

  public void OnFavoriteCoinChanged(int newFavoriteCoinId) => this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteCoin(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());

  private HashSet<int> GetCurrentDeckTrayModeCardBackIds() => CardBackManager.Get().GetCardBackIds(!CollectionManager.Get().IsInEditMode());

  private bool ShouldClassFilterBeVisible() => CollectionManager.Get().OwnsAnyCollectible();
}
