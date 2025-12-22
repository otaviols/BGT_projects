using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LettuceCollectionPageDisplay : CollectiblePageDisplay
{
  public GameObject m_noMatchFoundObject;
  public UberText m_noMatchExplanationText;
  public GameObject m_noMatchSetHintObject;
  public GameObject m_noMatchManaHintObject;
  public GameObject m_noMatchCraftingHintObject;
  public AsyncReference m_cardDisplayReference;
  private VisualController m_cardDisplayVisualController;
  private bool m_cardDisplayFinishedLoading;

  public void Start() => this.m_cardDisplayReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnCardDisplayReady));

  public override bool IsLoaded() => this.m_cardDisplayFinishedLoading;

  public override void UpdateCollectionItems(
    List<CollectionCardActors> actorList,
    List<ICollectible> nonActorCollectionList,
    CollectionUtils.ViewMode mode)
  {
    base.UpdateCollectionItems(actorList, nonActorCollectionList, mode);
  }

  public void UpdateCollectionMercs(
    List<LettuceMercenary> mercList,
    BookPageManager.PageTransitionType transitionType = BookPageManager.PageTransitionType.NONE)
  {
    if (!this.m_cardDisplayFinishedLoading)
      return;
    LettuceCollectionPageDataModel collectionPageDataModel = this.GetCollectionPageDataModel();
    if (collectionPageDataModel == null)
    {
      Log.Lettuce.PrintError("LettuceCollectionPageDisplay.UpdateCollectionMercs - could not find data model!");
    }
    else
    {
      DataModelList<LettuceMercenaryDataModel> dataModelList1;
      if (mercList == null)
      {
        dataModelList1 = (DataModelList<LettuceMercenaryDataModel>) null;
      }
      else
      {
        IEnumerable<LettuceMercenaryDataModel> source = mercList.Select<LettuceMercenary, LettuceMercenaryDataModel>(new Func<LettuceMercenary, LettuceMercenaryDataModel>(MercenaryFactory.CreateMercenaryDataModelWithCoin));
        dataModelList1 = source != null ? source.ToDataModelList<LettuceMercenaryDataModel>() : (DataModelList<LettuceMercenaryDataModel>) null;
      }
      DataModelList<LettuceMercenaryDataModel> dataModelList2 = dataModelList1 ?? new DataModelList<LettuceMercenaryDataModel>();
      collectionPageDataModel.MercenaryList = dataModelList2;
      collectionPageDataModel.CraftingModeActive = CollectionManager.Get().GetCollectibleDisplay().InCraftingMode();
      this.m_cardDisplayVisualController.OwningWidget.TriggerEvent("SHOW_COIN_TRAY", new Widget.TriggerEventParameters());
    }
  }

  public void UpdateMercenaryOnPage(LettuceMercenaryDataModel dataModel)
  {
    LettuceCollectionPageDataModel collectionPageDataModel = this.GetCollectionPageDataModel();
    if (collectionPageDataModel == null)
    {
      Log.Lettuce.PrintError("LettuceCollectionPageDisplay.UpdateMercenaryOnPage - could not find data model!");
    }
    else
    {
      for (int index = 0; index < collectionPageDataModel.MercenaryList.Count; ++index)
      {
        LettuceMercenaryDataModel mercenary = collectionPageDataModel.MercenaryList[index];
        if (mercenary != null && mercenary.MercenaryId == dataModel.MercenaryId)
        {
          collectionPageDataModel.MercenaryList[index] = dataModel.CloneDataModel<LettuceMercenaryDataModel>();
          this.m_cardDisplayVisualController.SetState("UPDATE_UPGRADE_STATUS");
          break;
        }
      }
    }
  }

  public void UpdateAcknowledgeStatusForMercenaryOnPage(int mercId, bool status)
  {
    LettuceMercenaryDataModel mercenaryOnPage = this.GetMercenaryOnPage(mercId);
    if (mercenaryOnPage == null)
      return;
    mercenaryOnPage.ShowAsNew = status;
    if (status)
      return;
    mercenaryOnPage.NumNewPortraits = 0;
  }

  public LettuceMercenaryDataModel GetMercenaryOnPage(int mercenaryId)
  {
    LettuceCollectionPageDataModel collectionPageDataModel = this.GetCollectionPageDataModel();
    if (collectionPageDataModel == null)
    {
      Log.Lettuce.PrintError("LettuceCollectionPageDisplay.UpdateMercenaryOnPage - could not find data model!");
      return (LettuceMercenaryDataModel) null;
    }
    for (int index = 0; index < collectionPageDataModel.MercenaryList.Count; ++index)
    {
      LettuceMercenaryDataModel mercenary = collectionPageDataModel.MercenaryList[index];
      if (mercenary != null && mercenary.MercenaryId == mercenaryId)
        return mercenary;
    }
    return (LettuceMercenaryDataModel) null;
  }

  public override void ShowNoMatchesFound(
    bool show,
    CollectionManager.FindCardsResult findResults = null,
    bool showHints = true)
  {
    this.m_noMatchFoundObject.SetActive(show);
    this.m_noMatchCraftingHintObject.SetActive(false);
    this.m_noMatchSetHintObject.SetActive(false);
    this.m_noMatchManaHintObject.SetActive(false);
    string key = "GLUE_COLLECTION_NO_RESULTS";
    if (show & showHints && findResults != null)
    {
      if (findResults.m_resultsWithoutManaFilterExist)
      {
        this.m_noMatchManaHintObject.SetActive(true);
        key = "GLUE_COLLECTION_NO_RESULTS_IN_SELECTED_COST";
      }
      else if (findResults.m_resultsWithoutSetFilterExist)
      {
        this.m_noMatchSetHintObject.SetActive(true);
        key = "GLUE_COLLECTION_NO_RESULTS_IN_CURRENT_SET";
      }
      else if (findResults.m_resultsUnownedExist)
      {
        this.m_noMatchCraftingHintObject.SetActive(true);
        key = "GLUE_COLLECTION_NO_RESULTS_BUT_CRAFTABLE";
      }
      else if (findResults.m_resultsInWildExist)
        key = "GLUE_COLLECTION_NO_RESULTS_IN_STANDARD";
    }
    this.m_noMatchExplanationText.Text = GameStrings.Get(key);
  }

  public override void UpdateCurrentPageCardLocks(bool playSound = false)
  {
    base.UpdateCurrentPageCardLocks(playSound);
    DeckTrayMercListContent mercsContent = CollectionDeckTray.Get().GetMercsContent();
    if (!mercsContent.IsModeTryingOrActive())
      return;
    LettuceTeamDataModel selectedTeamDataModel = mercsContent.SelectedTeamDataModel;
    if (selectedTeamDataModel == null)
      return;
    foreach (LettuceMercenaryDataModel mercenary1 in this.GetCollectionPageDataModel().MercenaryList)
    {
      mercenary1.InCurrentTeam = false;
      foreach (LettuceMercenaryDataModel mercenary2 in selectedTeamDataModel.MercenaryList)
      {
        if (mercenary1.MercenaryId == mercenary2.MercenaryId)
          mercenary1.InCurrentTeam = true;
      }
    }
  }

  public void ClearCurrentPageCardLocks()
  {
    LettuceCollectionPageDataModel collectionPageDataModel = this.GetCollectionPageDataModel();
    if (collectionPageDataModel == null)
      return;
    foreach (LettuceMercenaryDataModel mercenary in collectionPageDataModel.MercenaryList)
      mercenary.InCurrentTeam = false;
  }

  public override void SetPageType(FormatType formatType)
  {
  }

  public void SetRole(TAG_ROLE? roleTag)
  {
    if (!roleTag.HasValue)
    {
      this.SetPageNameText("");
      if (!((UnityEngine.Object) this.m_pageFlavorHeader != (UnityEngine.Object) null))
        return;
      this.m_pageFlavorHeader.SetActive(false);
    }
    else
    {
      TAG_ROLE tagRole = roleTag.Value;
      this.SetPageNameText(GameStrings.GetRoleName(tagRole));
      LettuceCollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, LettuceCollectionPageDisplay.TagRoleToHeaderRole(tagRole));
    }
  }

  public void WaitForPageUpdate(Action<object> listener, object payload)
  {
    if ((UnityEngine.Object) this.m_cardDisplayVisualController == (UnityEngine.Object) null)
      listener(payload);
    else
      this.m_cardDisplayVisualController.OwningWidget.RegisterDoneChangingStatesListener(listener, payload, true, true);
  }

  private void OnCardDisplayReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController != (UnityEngine.Object) null)
      visualController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(this.CardDisplayEventListener));
    this.m_cardDisplayVisualController = visualController;
    this.m_cardDisplayFinishedLoading = true;
  }

  private LettuceCollectionPageDataModel GetCollectionPageDataModel()
  {
    if ((UnityEngine.Object) this.m_cardDisplayVisualController == (UnityEngine.Object) null)
      return (LettuceCollectionPageDataModel) null;
    Widget owner = (Widget) this.m_cardDisplayVisualController.Owner;
    IDataModel model;
    if (!owner.GetDataModel(259, out model))
    {
      model = (IDataModel) new LettuceCollectionPageDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceCollectionPageDataModel;
  }

  private void CardDisplayEventListener(string eventName)
  {
    if (!(eventName == "MERCENARY_released"))
    {
      if (!(eventName == "MERCENARY_drag_started"))
      {
        if (!(eventName == "MERCENARY_drag_released"))
          return;
        CollectionInputMgr.Get().DropMercenariesModeCard(false);
      }
      else
        this.OnMercenaryDragStart();
    }
    else
      this.OnMercenaryReleased();
  }

  private void OnMercenaryReleased()
  {
    if (CollectionInputMgr.Get().HasHeldCard())
      return;
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_cardDisplayVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to LettuceCollectionPageDisplay");
    }
    else
    {
      LettuceMercenaryDataModel payload = (LettuceMercenaryDataModel) eventDataModel.Payload;
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) payload.MercenaryId);
      LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
      if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
        Log.Lettuce.PrintError("LettuceCollectionPageDisplay.OnMercenaryReleased - LettuceCollectionDisplay is null!");
      else if (mercenary.m_owned)
        collectibleDisplay.ShowMercenaryDetailsDisplay(mercenary);
      else
        collectibleDisplay.ShowMercCraftingPopup(payload);
    }
  }

  private void OnMercenaryDragStart()
  {
    if (CollectionDeckTray.Get().GetCurrentContentType() != DeckTray.DeckContentTypes.Mercs)
      return;
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_cardDisplayVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to LettuceCollectionPageDisplay");
    }
    else
    {
      LettuceMercenaryDataModel payload = (LettuceMercenaryDataModel) eventDataModel.Payload;
      if (!this.CanPickUpMercenary(payload))
        return;
      CollectionInputMgr.Get().GrabMercenariesModeCard((IDataModel) payload, CollectionUtils.MercenariesModeCardType.Mercenary, (InputMgr.OnCardDroppedCallback) null);
    }
  }

  private bool CanPickUpMercenary(LettuceMercenaryDataModel mercData) => !mercData.InCurrentTeam && !this.ShouldIgnoreAllInput() && CollectionDeckTray.Get().CanPickupCard() && CollectionManager.Get().GetMercenary((long) mercData.MercenaryId).m_owned;

  private bool ShouldIgnoreAllInput() => !this.IsShown || (UnityEngine.Object) CollectionInputMgr.Get() != (UnityEngine.Object) null && CollectionInputMgr.Get().IsDraggingScrollbar() || CollectionManager.Get().GetCollectibleDisplay().GetPageManager().ArePagesTurning();

  public static LettuceCollectionPageDisplay.HEADER_ROLE TagRoleToHeaderRole(
    TAG_ROLE roleTag)
  {
    string str = roleTag.ToString();
    return Enum.IsDefined(typeof (LettuceCollectionPageDisplay.HEADER_ROLE), (object) str) ? (LettuceCollectionPageDisplay.HEADER_ROLE) Enum.Parse(typeof (LettuceCollectionPageDisplay.HEADER_ROLE), str) : LettuceCollectionPageDisplay.HEADER_ROLE.INVALID;
  }

  public static void SetPageFlavorTextures(
    GameObject header,
    LettuceCollectionPageDisplay.HEADER_ROLE headerRole)
  {
    if ((UnityEngine.Object) header == (UnityEngine.Object) null)
      return;
    float y = (float) (-(double) (headerRole - 1) / 4.0);
    CollectiblePageDisplay.SetPageFlavorTextures(header, new UnityEngine.Vector2(0.0f, y));
  }

  public enum HEADER_ROLE
  {
    INVALID,
    FIGHTER,
    CASTER,
    TANK,
  }
}
