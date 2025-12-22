using Assets;
using Blizzard.T5.Core;
using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class LettuceCollectionPageManager : CollectiblePageManager
{
  public static readonly Map<TAG_ROLE, UnityEngine.Vector2> s_roleTextureOffsets = new Map<TAG_ROLE, UnityEngine.Vector2>()
  {
    {
      TAG_ROLE.CASTER,
      new UnityEngine.Vector2(0.0f, 0.0f)
    },
    {
      TAG_ROLE.FIGHTER,
      new UnityEngine.Vector2(0.205f, -0.2f)
    },
    {
      TAG_ROLE.TANK,
      new UnityEngine.Vector2(0.205f, 0.0f)
    }
  };
  public static TAG_ROLE[] ROLE_TAB_ORDER = new TAG_ROLE[3]
  {
    TAG_ROLE.TANK,
    TAG_ROLE.FIGHTER,
    TAG_ROLE.CASTER
  };
  private static readonly float MOBILE_HIDDEN_TAB_LOCAL_Z_POS = -10f;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_lettuceCollectionPageManagerAudioReference;
  private VisualController m_lettuceCollectionPageManagerAudio;
  private List<LettuceRoleTab> m_roleTabs = new List<LettuceRoleTab>();
  private int m_numPageFlipsThisSession;
  protected TAG_ROLE m_currentRoleContext;
  private LettuceMercenary m_lastMercAnchor;
  private Coroutine m_animatingTabsCoroutine;

  public TAG_ROLE CurrentRoleContext => this.m_currentRoleContext;

  private bool UsesTabs => (UnityEngine.Object) this.m_tabContainer != (UnityEngine.Object) null;

  private CollectibleCardRoleFilter m_roleCardsCollection => (CollectibleCardRoleFilter) this.m_cardsCollection;

  public event EventHandler PageTransitioned;

  protected override void Awake()
  {
    this.m_cardsCollection = (CollectibleCardFilter) new CollectibleCardRoleFilter();
    base.Awake();
    this.m_roleCardsCollection.Init(LettuceCollectionPageManager.ROLE_TAB_ORDER, CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.CARDS));
    this.UpdateFilteredCards();
  }

  protected override void Start()
  {
    base.Start();
    this.m_lettuceCollectionPageManagerAudioReference.RegisterReadyListener<VisualController>((Action<VisualController>) (vc => this.m_lettuceCollectionPageManagerAudio = vc));
  }

  public override void OnDestroy() => base.OnDestroy();

  public override bool JumpToPageWithCard(
    string cardID,
    TAG_PREMIUM premium,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    int collectionPage;
    if (this.m_roleCardsCollection.GetPageContentsForMercenary(CollectionManager.Get().GetMercenary(cardID), out collectionPage).Count == 0 || this.m_currentPageNum == collectionPage)
      return false;
    this.FlipToPage(collectionPage, callback, callbackData);
    return true;
  }

  public void UpdateTabNewCardCounts()
  {
    foreach (LettuceRoleTab roleTab in this.m_roleTabs)
      roleTab.UpdateNewItemCount(this.GetNumNewCardsForRole(roleTab.GetRole()));
  }

  public int GetNumNewCardsForRole(TAG_ROLE tagRole) => this.m_roleCardsCollection.GetNumNewCardsForRole(tagRole);

  public List<LettuceMercenary> GetCurrentMercenaryResults() => this.m_roleCardsCollection.FindMercenariesResult.m_mercenaries;

  public List<LettuceMercenary> GetRoleSortedMercenaryResults() => this.m_roleCardsCollection.GetAllRoleResults();

  public void OnDoneEditingTeam()
  {
    LettuceTeamDataModel selectedTeamDataModel = CollectionDeckTray.Get().GetMercsContent().SelectedTeamDataModel;
    if (selectedTeamDataModel != null)
    {
      foreach (LettuceMercenaryDataModel mercenary in selectedTeamDataModel.MercenaryList)
        mercenary.InCurrentTeam = false;
    }
    LettuceCollectionPageDisplay currentCollectiblePage = this.GetCurrentCollectiblePage() as LettuceCollectionPageDisplay;
    if (!((UnityEngine.Object) currentCollectiblePage != (UnityEngine.Object) null))
      return;
    currentCollectiblePage.ClearCurrentPageCardLocks();
  }

  public override void NotifyOfCollectionChanged()
  {
  }

  public bool HasRoleCardsAvailable(TAG_ROLE roleTag) => this.m_roleCardsCollection.GetNumPagesForRole(roleTag) > 0;

  public void ShowCraftingModeMercs(
    BookPageManager.DelOnPageTransitionComplete callback = null,
    object callbackData = null,
    bool showCraftableMercs = true,
    bool showOnlyPromotableMercs = false,
    bool updatePage = true,
    bool toggleChanged = false)
  {
    if (!(this.m_cardsCollection is CollectibleCardRoleFilter cardsCollection))
      return;
    cardsCollection.FilterOnlyOwned(showCraftableMercs);
    cardsCollection.FilterOnlyUpgradeableMercs(showOnlyPromotableMercs);
    this.UpdateFilteredCards();
    BookPageManager.PageTransitionType transitionType = toggleChanged ? BookPageManager.PageTransitionType.MANY_PAGE_LEFT : BookPageManager.PageTransitionType.NONE;
    if (toggleChanged)
      this.m_lastMercAnchor = (LettuceMercenary) null;
    if (!updatePage)
      return;
    this.TransitionPageWhenReady(transitionType, false, callback, callbackData);
  }

  public override void HideCraftingModeCards(
    BookPageManager.PageTransitionType transitionType = BookPageManager.PageTransitionType.NONE,
    bool updatePage = true)
  {
    if (this.m_cardsCollection is CollectibleCardRoleFilter cardsCollection)
      cardsCollection.FilterOnlyUpgradeableMercs(false);
    base.HideCraftingModeCards(transitionType);
  }

  public void UpdatePageMercenary(LettuceMercenaryDataModel dataModel)
  {
    LettuceCollectionPageDisplay currentCollectiblePage = this.GetCurrentCollectiblePage() as LettuceCollectionPageDisplay;
    if ((UnityEngine.Object) currentCollectiblePage == (UnityEngine.Object) null)
      return;
    currentCollectiblePage.UpdateMercenaryOnPage(dataModel);
  }

  public void UpdateAcknowledgedStatusForPageMercenary(int mercID, bool status)
  {
    LettuceCollectionPageDisplay currentCollectiblePage = this.GetCurrentCollectiblePage() as LettuceCollectionPageDisplay;
    if ((UnityEngine.Object) currentCollectiblePage == (UnityEngine.Object) null)
      return;
    currentCollectiblePage.UpdateAcknowledgeStatusForMercenaryOnPage(mercID, status);
  }

  public LettuceMercenaryDataModel GetMercenaryOnPage(int mercenaryId)
  {
    LettuceCollectionPageDisplay currentCollectiblePage = this.GetCurrentCollectiblePage() as LettuceCollectionPageDisplay;
    return (UnityEngine.Object) currentCollectiblePage == (UnityEngine.Object) null ? (LettuceMercenaryDataModel) null : currentCollectiblePage.GetMercenaryOnPage(mercenaryId);
  }

  protected override bool ShouldShowTab(BookTab tab)
  {
    if (!this.m_initializedTabPositions)
      return true;
    LettuceRoleTab lettuceRoleTab = tab as LettuceRoleTab;
    if (!((UnityEngine.Object) lettuceRoleTab == (UnityEngine.Object) null))
      return this.HasRoleCardsAvailable(lettuceRoleTab.GetRole());
    Log.CollectionManager.PrintError("CollectionPageManager.ShouldShowTab passed a non-LettuceRoleTab object.");
    return false;
  }

  protected override void SetUpBookTabs()
  {
    if (!this.UsesTabs)
      return;
    bool receiveReleaseWithoutMouseDown = UniversalInputManager.Get().IsTouchMode();
    for (int index = 0; index < LettuceCollectionPageManager.ROLE_TAB_ORDER.Length; ++index)
    {
      TAG_ROLE roleTag = LettuceCollectionPageManager.ROLE_TAB_ORDER[index];
      LettuceRoleTab key = (LettuceRoleTab) GameUtils.Instantiate((Component) this.m_tabPrefab, this.m_tabContainer);
      key.Init(roleTag);
      key.transform.localScale = (bool) UniversalInputManager.UsePhoneUI ? key.m_MobileDeselectedLocalScale : key.m_DeselectedLocalScale;
      key.transform.localEulerAngles = CollectiblePageManager.TAB_LOCAL_EULERS;
      key.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRoleTabPressed));
      key.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver));
      key.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut));
      key.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver_Touch));
      key.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut_Touch));
      key.SetReceiveReleaseWithoutMouseDown(receiveReleaseWithoutMouseDown);
      key.gameObject.name = roleTag.ToString();
      this.m_allTabs.Add((BookTab) key);
      this.m_roleTabs.Add(key);
      this.m_tabVisibility[(BookTab) key] = true;
      if (index <= 0)
        this.m_deselectedTabHalfWidth = key.GetComponent<BoxCollider>().bounds.extents.x;
    }
    this.PositionBookTabs(false);
    this.m_initializedTabPositions = true;
  }

  private void SetupBookTab(bool show, bool animate)
  {
    if (!this.UsesTabs)
      return;
    Vector3 position = this.m_tabContainer.transform.position;
    int length = LettuceCollectionPageManager.ROLE_TAB_ORDER.Length;
    for (int index = 0; index < length; ++index)
    {
      LettuceRoleTab roleTab = this.m_roleTabs[index];
      Vector3 targetLocalPos;
      if ((!show ? 0 : (this.ShouldShowTab((BookTab) roleTab) ? 1 : 0)) != 0)
      {
        roleTab.SetTargetVisibility(true);
        position.x += this.m_spaceBetweenTabs;
        position.x += this.m_deselectedTabHalfWidth;
        targetLocalPos = this.m_tabContainer.transform.InverseTransformPoint(position);
        if ((UnityEngine.Object) roleTab == (UnityEngine.Object) this.m_currentTab)
        {
          targetLocalPos.y = roleTab.m_SelectedLocalYPos;
          targetLocalPos += roleTab.m_SelectedLocalOffset;
        }
        position.x += this.m_deselectedTabHalfWidth;
      }
      else
      {
        roleTab.SetTargetVisibility(false);
        targetLocalPos = roleTab.transform.localPosition with
        {
          z = (bool) UniversalInputManager.UsePhoneUI ? LettuceCollectionPageManager.MOBILE_HIDDEN_TAB_LOCAL_Z_POS : CollectiblePageManager.HIDDEN_TAB_LOCAL_Z_POS
        };
      }
      if (animate)
      {
        roleTab.SetTargetLocalPosition(targetLocalPos);
      }
      else
      {
        roleTab.SetIsVisible(roleTab.ShouldBeVisible());
        roleTab.transform.localPosition = targetLocalPos;
      }
    }
  }

  protected override void PositionBookTabs(bool animate)
  {
    this.SetupBookTab(true, animate);
    if (!animate)
      return;
    if (this.m_animatingTabsCoroutine != null)
      this.StopCoroutine(this.m_animatingTabsCoroutine);
    this.m_animatingTabsCoroutine = this.StartCoroutine(this.AnimateTabs());
  }

  private IEnumerator AnimateTabs(bool allowSFX = true)
  {
    LettuceCollectionPageManager collectionPageManager = this;
    bool playSounds = allowSFX && ((UnityEngine.Object) HeroPickerDisplay.Get() == (UnityEngine.Object) null || !HeroPickerDisplay.Get().IsShown());
    List<LettuceRoleTab> lettuceRoleTabList = new List<LettuceRoleTab>();
    List<LettuceRoleTab> tabsToShow = new List<LettuceRoleTab>();
    List<LettuceRoleTab> tabsToMove = new List<LettuceRoleTab>();
    foreach (LettuceRoleTab roleTab in collectionPageManager.m_roleTabs)
    {
      if (roleTab.IsVisible() || roleTab.ShouldBeVisible())
      {
        if (roleTab.IsVisible() && roleTab.ShouldBeVisible())
          tabsToMove.Add(roleTab);
        else if (roleTab.IsVisible() && !roleTab.ShouldBeVisible())
          lettuceRoleTabList.Add(roleTab);
        else
          tabsToShow.Add(roleTab);
      }
    }
    collectionPageManager.m_tabsAreAnimating = true;
    if (lettuceRoleTabList.Count > 0)
    {
      foreach (LettuceRoleTab tab in lettuceRoleTabList)
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
      foreach (LettuceRoleTab lettuceRoleTab in tabsToMove)
      {
        if (lettuceRoleTab.WillSlide() & playSounds)
          SoundManager.Get().LoadAndPlay((AssetReference) "class_tab_slides_across_top.prefab:04482bc6f531b76468ff92a5b4e979b6", lettuceRoleTab.gameObject);
        lettuceRoleTab.AnimateToTargetPosition(0.25f, iTween.EaseType.easeOutQuad);
      }
      yield return (object) new WaitForSeconds(0.25f);
    }
    if (tabsToShow.Count > 0)
    {
      foreach (LettuceRoleTab lettuceRoleTab in tabsToShow)
      {
        if (playSounds)
          SoundManager.Get().LoadAndPlay((AssetReference) "class_tab_retract.prefab:da79957be76b10343999d6fa92a6a2f0", lettuceRoleTab.gameObject);
        lettuceRoleTab.AnimateToTargetPosition(0.4f, iTween.EaseType.easeOutBounce);
      }
      yield return (object) new WaitForSeconds(0.4f);
    }
    foreach (LettuceRoleTab roleTab in collectionPageManager.m_roleTabs)
      roleTab.SetIsVisible(roleTab.ShouldBeVisible());
    collectionPageManager.m_tabsAreAnimating = false;
  }

  public void PlayTabTuckAnimation(bool forward, bool animate = true, bool allowSFX = true)
  {
    this.SetupBookTab(!forward, animate);
    if (!animate)
      return;
    if (this.m_animatingTabsCoroutine != null)
      this.StopCoroutine(this.m_animatingTabsCoroutine);
    this.m_animatingTabsCoroutine = this.StartCoroutine(this.AnimateTabs(allowSFX));
  }

  private void SetCurrentRoleTab(TAG_ROLE? tabRole)
  {
    LettuceRoleTab lettuceRoleTab = (LettuceRoleTab) null;
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.CARDS)
    {
      if (tabRole.HasValue)
        lettuceRoleTab = this.m_roleTabs.Find((Predicate<LettuceRoleTab>) (obj => obj.GetRole() == tabRole.Value && obj.m_tabViewMode != CollectionUtils.ViewMode.DECK_TEMPLATE));
    }
    else
      lettuceRoleTab = (LettuceRoleTab) null;
    if ((UnityEngine.Object) lettuceRoleTab == (UnityEngine.Object) this.m_currentTab)
      return;
    this.DeselectCurrentTab();
    TAG_ROLE? nullable = tabRole;
    if (nullable.HasValue)
    {
      switch (nullable.GetValueOrDefault())
      {
        case TAG_ROLE.CASTER:
          MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesCMCaster);
          break;
        case TAG_ROLE.FIGHTER:
          MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesCMFighter);
          break;
        case TAG_ROLE.TANK:
          MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesCMTank);
          break;
      }
    }
    this.m_currentTab = (BookTab) lettuceRoleTab;
    if (!((UnityEngine.Object) this.m_currentTab != (UnityEngine.Object) null))
      return;
    this.StopCoroutine(CollectiblePageManager.SELECT_TAB_COROUTINE_NAME);
    this.StartCoroutine(CollectiblePageManager.SELECT_TAB_COROUTINE_NAME, (object) this.m_currentTab);
  }

  public void SelectRole(TAG_ROLE role)
  {
    if (!this.CanUserTurnPages())
      return;
    this.OnRoleSelected(role);
  }

  private void OnRoleTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    LettuceRoleTab element = e.GetElement() as LettuceRoleTab;
    if ((UnityEngine.Object) element == (UnityEngine.Object) null || (UnityEngine.Object) element == (UnityEngine.Object) this.m_currentTab)
      return;
    element.PlayClickFX();
    this.OnRoleSelected(element.GetRole());
  }

  private void OnRoleSelected(TAG_ROLE role)
  {
    this.m_lettuceCollectionPageManagerAudio.SetState(role.ToString() + "_TAB_CLICKED_code");
    this.JumpToCollectionRolePage(role);
  }

  public void JumpToCollectionRolePage(TAG_ROLE pageRole) => this.JumpToCollectionRolePage(pageRole, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  public void JumpToCollectionRolePage(
    TAG_ROLE pageRole,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.GetViewMode() != CollectionUtils.ViewMode.CARDS)
    {
      collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.CARDS, new CollectionUtils.ViewModeData()
      {
        m_setPageByRole = new TAG_ROLE?(pageRole)
      });
    }
    else
    {
      int collectionPage = 0;
      this.m_roleCardsCollection.GetPageContentsForRole(pageRole, 1, true, out collectionPage);
      this.FlipToPage(collectionPage, callback, callbackData);
    }
  }

  protected override void AssembleEmptyPageUI(BookPageDisplay page)
  {
    base.AssembleEmptyPageUI(page);
    this.AssembleEmptyPageUI((CollectiblePageDisplay) (page as LettuceCollectionPageDisplay), false);
  }

  protected override void AssembleEmptyPageUI(
    CollectiblePageDisplay page,
    bool displayNoMatchesText)
  {
    LettuceCollectionPageDisplay collectionPageDisplay = page as LettuceCollectionPageDisplay;
    if ((UnityEngine.Object) collectionPageDisplay == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("Page in LettuceCollectionPageManager is not a LettuceCollectionPageDisplay! This should not happen!");
    }
    else
    {
      collectionPageDisplay.SetRole(new TAG_ROLE?());
      collectionPageDisplay.ShowNoMatchesFound(displayNoMatchesText, this.m_roleCardsCollection.FindCardsResult, true);
      collectionPageDisplay.UpdateCollectionMercs((List<LettuceMercenary>) null);
      this.DeselectCurrentTab();
      collectionPageDisplay.SetPageCountText(GameStrings.Get("GLUE_COLLECTION_EMPTY_PAGE"));
    }
  }

  protected bool AssembleMercenaryPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    List<LettuceMercenary> cardsToDisplay,
    int totalNumPages)
  {
    bool emptyPage = cardsToDisplay == null || cardsToDisplay.Count == 0;
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1} currentPageIsPageA={2} emptyPage={3} viewMode={4}", (object) this.m_transitionPageId, (object) this.m_pagesCurrentlyTurning, (object) this.m_currentPageIsPageA, (object) emptyPage, (object) viewMode);
    if (this.AssembleCollectionBasePage(transitionReadyCallbackData, emptyPage, PegasusShared.FormatType.FT_STANDARD))
      return true;
    LettuceCollectionPageDisplay assembledPage = transitionReadyCallbackData.m_assembledPage as LettuceCollectionPageDisplay;
    this.m_lastMercAnchor = cardsToDisplay[0];
    TAG_ROLE currentRoleFromPage = this.m_roleCardsCollection.GetCurrentRoleFromPage(this.m_currentPageNum);
    assembledPage.SetRole(new TAG_ROLE?(currentRoleFromPage));
    this.m_currentRoleContext = currentRoleFromPage;
    assembledPage.SetPageCountText(GameStrings.Format("GLUE_COLLECTION_PAGE_NUM", (object) this.m_currentPageNum));
    assembledPage.ShowNoMatchesFound(false, (CollectionManager.FindCardsResult) null, true);
    this.SetHasPreviousAndNextPages(this.m_currentPageNum > 1, this.m_currentPageNum < totalNumPages);
    assembledPage.UpdateCollectionMercs(cardsToDisplay, transitionReadyCallbackData.m_transitionType);
    if (transitionReadyCallbackData.m_transitionType == BookPageManager.PageTransitionType.NONE)
      assembledPage.WaitForPageUpdate(new Action<object>(((BookPageManager) this).TransitionPage), (object) transitionReadyCallbackData);
    else
      this.TransitionPageNextFrame(transitionReadyCallbackData);
    return true;
  }

  protected override void AssemblePage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() == (UnityEngine.Object) null)
      return;
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    if (this.m_roleCardsCollection == null)
    {
      Log.Lettuce.PrintError("LettuceCollectionPageManager.AssemblePage - card collection is null!");
    }
    else
    {
      if (viewMode != CollectionUtils.ViewMode.CARDS)
        return;
      List<LettuceMercenary> cardsToDisplay;
      if (useCurrentPageNum)
      {
        cardsToDisplay = this.m_roleCardsCollection.GetMercenariesPageContents(this.m_currentPageNum);
      }
      else
      {
        if (!LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_END))
          this.m_lastMercAnchor = CollectionManager.Get().FindMercenaries(isOwned: new bool?(true)).m_mercenaries.FirstOrDefault<LettuceMercenary>((Func<LettuceMercenary, bool>) (m => m.CanAnyAbilityBeUpgraded()));
        if (this.m_lastMercAnchor == null)
        {
          this.m_currentPageNum = 1;
          cardsToDisplay = this.m_roleCardsCollection.GetMercenariesPageContents(this.m_currentPageNum);
        }
        else
        {
          if (this.m_roleCardsCollection == null)
          {
            Log.Lettuce.PrintError("LettuceCollectionPageManager.AssemblePage - role collection is null!");
            return;
          }
          int collectionPage;
          cardsToDisplay = this.m_roleCardsCollection.GetPageContentsForMercenary(this.m_lastMercAnchor, out collectionPage);
          if (cardsToDisplay.Count == 0)
            cardsToDisplay = this.m_roleCardsCollection.GetPageContentsForRole(this.m_currentRoleContext, 1, true, out collectionPage);
          if (cardsToDisplay.Count > 1)
          {
            foreach (LettuceMercenary lettuceMercenary in cardsToDisplay)
            {
              if (lettuceMercenary.ID == 69)
              {
                cardsToDisplay.Remove(lettuceMercenary);
                cardsToDisplay.Insert(0, lettuceMercenary);
                break;
              }
            }
          }
          if (cardsToDisplay.Count == 0)
          {
            cardsToDisplay = this.m_roleCardsCollection.GetMercenariesPageContents(1);
            collectionPage = 1;
          }
          this.m_currentPageNum = cardsToDisplay.Count == 0 ? 0 : collectionPage;
        }
        LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
        if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.CanShowAppearanceTip(false) && cardsToDisplay.Count > 1)
        {
          foreach (LettuceMercenary lettuceMercenary in cardsToDisplay)
          {
            if (lettuceMercenary.ID == 18)
            {
              cardsToDisplay.Remove(lettuceMercenary);
              cardsToDisplay.Insert(0, lettuceMercenary);
              break;
            }
          }
        }
      }
      if (cardsToDisplay == null || cardsToDisplay.Count == 0)
      {
        int collectionPage;
        cardsToDisplay = this.m_roleCardsCollection.GetFirstNonEmptyMercenaryPage(out collectionPage);
        if (cardsToDisplay.Count > 0)
          this.m_currentPageNum = collectionPage;
      }
      this.AssembleMercenaryPage(transitionReadyCallbackData, cardsToDisplay, this.m_roleCardsCollection.GetTotalNumPages());
      this.UpdateCurrentPageCardLocks(false);
    }
  }

  protected override void UpdateFilteredCards()
  {
    base.UpdateFilteredCards();
    this.UpdateTabNewCardCounts();
  }

  protected override void TransitionPage(object callbackData)
  {
    base.TransitionPage(callbackData);
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT)
    {
      this.DeselectCurrentTab();
    }
    else
    {
      this.SetCurrentRoleTab(new TAG_ROLE?(this.m_currentRoleContext));
      EventHandler pageTransitioned = this.PageTransitioned;
      if (pageTransitioned == null)
        return;
      pageTransitioned((object) this, new EventArgs());
    }
  }

  protected override void TransitionPageWhenReady(
    BookPageManager.PageTransitionType transitionType,
    bool useCurrentPageNum,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    if (transitionType != BookPageManager.PageTransitionType.NONE && !LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_END))
      return;
    base.TransitionPageWhenReady(transitionType, useCurrentPageNum, callback, callbackData);
  }

  protected override void OnPageTransitionRequested()
  {
    ++this.m_numPageFlipsThisSession;
    int num = Options.Get().GetInt(Option.PAGE_MOUSE_OVERS);
    int val = num + 1;
    if (num < this.m_numPlageFlipsBeforeStopShowingArrows)
      Options.Get().SetInt(Option.PAGE_MOUSE_OVERS, val);
    (CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay).HideHelpPopups();
  }

  protected override void OnPageTurnComplete(object callbackData, int operationId)
  {
    if (this.m_numPageFlipsThisSession % CollectiblePageManager.NUM_PAGE_FLIPS_UNTIL_UNLOAD_UNUSED_ASSETS == 0)
    {
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
        hearthstoneApplication.UnloadUnusedAssets();
    }
    base.OnPageTurnComplete(callbackData, operationId);
    (CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay).TryShowCollectionTips();
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
    if (mode != CollectionUtils.ViewMode.CARDS)
      CollectionDeckTray.Get().GetCardsContent().HideDeckHelpPopup();
    this.m_currentPageNum = 1;
    if (userdata != null)
    {
      if (userdata.m_setPageByRole.HasValue)
        this.m_roleCardsCollection.GetPageContentsForRole(userdata.m_setPageByRole.Value, 1, true, out this.m_currentPageNum);
      else if (userdata.m_setPageByCard != null)
        this.m_roleCardsCollection.GetPageContentsForMercenary(CollectionManager.Get().GetMercenary(userdata.m_setPageByCard), out this.m_currentPageNum);
    }
    BookPageManager.PageTransitionType transitionType = -0 < 0 ? BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT : BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT;
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
    this.TransitionPageWhenReady(transitionType, true, callback, callbackData);
  }

  private HashSet<int> GetCurrentDeckTrayModeCardBackIds() => CardBackManager.Get().GetCardBackIds(!CollectionManager.Get().IsInEditMode());
}
