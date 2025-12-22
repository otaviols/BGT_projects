using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaconCollectionPageDisplay : CollectiblePageDisplay
{
  public GameObject m_favoriteBanner;
  public GameObject m_heroSkinsDecor;
  public GameObject[] m_heroSkinFrames;
  public GameObject m_guideSkinsDecor;
  public GameObject[] m_guideSkinFrames;
  public GameObject m_noMatchFoundObject;
  public UberText m_noMatchExplanationText;
  public Widget m_BoardSkinsWidget;
  public AsyncReference m_boardDisplayReference;
  public Widget m_FinishersWidget;
  public AsyncReference m_finisherDisplayReference;
  public Widget m_EmotesWidget;
  public AsyncReference m_emoteDisplayReference;
  private Widget m_BoardSkinsWidgetInstance;
  private Widget m_FinishersWidgetInstance;
  private Widget m_EmotesWidgetInstance;

  public void Start()
  {
    this.m_boardDisplayReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnBoardDisplayReady));
    this.m_finisherDisplayReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnFinisherDisplayReady));
    this.m_emoteDisplayReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnEmoteDisplayReady));
  }

  public override void UpdateCollectionItems(
    List<CollectionCardActors> actorList,
    List<ICollectible> nonActorCollectibles,
    CollectionUtils.ViewMode mode)
  {
    base.UpdateCollectionItems(actorList, nonActorCollectibles, mode);
    for (int index = 0; index < actorList.Count && index < CollectiblePageDisplay.GetMaxCardsPerPage(); ++index)
    {
      CollectionCardVisual collectionCardVisual = this.GetCollectionCardVisual(index);
      if (mode == CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS || mode == CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS)
        collectionCardVisual.SetHeroSkinBoxCollider();
      else
        collectionCardVisual.SetDefaultBoxCollider();
    }
    List<CollectibleBattlegroundsBoard> boardList = new List<CollectibleBattlegroundsBoard>();
    foreach (ICollectible actorCollectible in nonActorCollectibles)
    {
      if (actorCollectible is CollectibleBattlegroundsBoard battlegroundsBoard)
        boardList.Add(battlegroundsBoard);
    }
    List<CollectibleBattlegroundsFinisher> finisherList = new List<CollectibleBattlegroundsFinisher>();
    foreach (ICollectible actorCollectible in nonActorCollectibles)
    {
      if (actorCollectible is CollectibleBattlegroundsFinisher battlegroundsFinisher)
        finisherList.Add(battlegroundsFinisher);
    }
    List<CollectibleBattlegroundsEmote> emoteList = new List<CollectibleBattlegroundsEmote>();
    foreach (ICollectible actorCollectible in nonActorCollectibles)
    {
      if (actorCollectible is CollectibleBattlegroundsEmote battlegroundsEmote)
        emoteList.Add(battlegroundsEmote);
    }
    this.UpdateFavoriteHeroSkins(mode);
    this.UpdateFavoriteGuideSkins(mode);
    this.UpdateCollectionBoards(boardList, mode);
    this.UpdateCollectionFinishers(finisherList, mode);
    this.UpdateCollectionEmotes(emoteList, mode);
    this.UpdateHeroSkinNames(mode);
    this.UpdateGuideSkinNames(mode);
    this.UpdateHeroSkinHeroPowers(mode);
  }

  public void UpdateCollectionBoards(
    List<CollectibleBattlegroundsBoard> boardList,
    CollectionUtils.ViewMode mode,
    BookPageManager.PageTransitionType transitionType = BookPageManager.PageTransitionType.NONE)
  {
    bool flag = mode == CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS;
    this.m_BoardSkinsWidget.gameObject.SetActive(flag);
    if (!flag)
    {
      this.m_BoardSkinsWidget.UnbindDataModel(565);
    }
    else
    {
      BattlegroundsBoardSkinCollectionPageDataModel collectionPageDataModel = this.GetOrCreateBoardCollectionPageDataModel();
      if (collectionPageDataModel == null)
      {
        Log.All.PrintError("BaconCollectionPageDisplay.UpdateCollectionBoards - could not find data model!");
      }
      else
      {
        this.m_BoardSkinsWidget.BindDataModel((IDataModel) collectionPageDataModel);
        DataModelList<BattlegroundsBoardSkinDataModel> dataModelList = new DataModelList<BattlegroundsBoardSkinDataModel>();
        if (dataModelList != null)
        {
          foreach (CollectibleBattlegroundsBoard board in boardList)
          {
            BattlegroundsBoardSkinDataModel boardDataModel = board.CreateBoardDataModel();
            dataModelList.Add(boardDataModel);
          }
        }
        collectionPageDataModel.BoardSkinList = dataModelList;
      }
    }
  }

  public void UpdateCollectionFinishers(
    List<CollectibleBattlegroundsFinisher> finisherList,
    CollectionUtils.ViewMode mode,
    BookPageManager.PageTransitionType transitionType = BookPageManager.PageTransitionType.NONE)
  {
    bool flag = mode == CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS;
    this.m_FinishersWidget.gameObject.SetActive(flag);
    if (!flag)
    {
      this.m_FinishersWidget.UnbindDataModel(568);
    }
    else
    {
      BattlegroundsFinisherCollectionPageDataModel collectionPageDataModel = this.GetOrCreateFinisherCollectionPageDataModel();
      if (collectionPageDataModel == null)
      {
        Log.All.PrintError("BaconCollectionPageDisplay.UpdateCollectionFinishers - could not find data model!");
      }
      else
      {
        this.m_FinishersWidget.BindDataModel((IDataModel) collectionPageDataModel);
        DataModelList<BattlegroundsFinisherDataModel> dataModelList = new DataModelList<BattlegroundsFinisherDataModel>();
        if (dataModelList != null)
        {
          foreach (CollectibleBattlegroundsFinisher finisher in finisherList)
          {
            BattlegroundsFinisherDataModel finisherDataModel = finisher.CreateFinisherDataModel();
            dataModelList.Add(finisherDataModel);
          }
        }
        collectionPageDataModel.FinisherList = dataModelList;
      }
    }
  }

  public void UpdateCollectionEmotes(
    List<CollectibleBattlegroundsEmote> emoteList,
    CollectionUtils.ViewMode mode,
    BookPageManager.PageTransitionType transitionType = BookPageManager.PageTransitionType.NONE)
  {
    bool flag = mode == CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES;
    this.m_EmotesWidget.gameObject.SetActive(flag);
    if (!flag)
    {
      this.m_EmotesWidget.UnbindDataModel(639);
    }
    else
    {
      BattlegroundsEmoteCollectionPageDataModel collectionPageDataModel = this.GetOrCreateEmoteCollectionPageDataModel();
      if (collectionPageDataModel == null)
      {
        Log.All.PrintError("BaconCollectionPageDisplay.UpdateCollectionEmotes - could not find data model!");
      }
      else
      {
        this.m_EmotesWidget.BindDataModel((IDataModel) collectionPageDataModel);
        DataModelList<BattlegroundsEmoteDataModel> dataModelList = new DataModelList<BattlegroundsEmoteDataModel>();
        if (dataModelList != null)
        {
          foreach (CollectibleBattlegroundsEmote emote in emoteList)
          {
            BattlegroundsEmoteDataModel emoteDataModel = emote.CreateEmoteDataModel();
            dataModelList.Add(emoteDataModel);
          }
        }
        collectionPageDataModel.EmoteList = dataModelList;
      }
    }
  }

  private BattlegroundsBoardSkinCollectionPageDataModel GetOrCreateBoardCollectionPageDataModel()
  {
    IDataModel model;
    if (!this.m_BoardSkinsWidget.GetDataModel(565, out model))
      model = (IDataModel) new BattlegroundsBoardSkinCollectionPageDataModel();
    return model as BattlegroundsBoardSkinCollectionPageDataModel;
  }

  private BattlegroundsFinisherCollectionPageDataModel GetOrCreateFinisherCollectionPageDataModel()
  {
    IDataModel model;
    if (!this.m_FinishersWidget.GetDataModel(568, out model))
      model = (IDataModel) new BattlegroundsFinisherCollectionPageDataModel();
    return model as BattlegroundsFinisherCollectionPageDataModel;
  }

  private BattlegroundsEmoteCollectionPageDataModel GetOrCreateEmoteCollectionPageDataModel()
  {
    IDataModel model;
    if (!this.m_EmotesWidget.GetDataModel(639, out model))
      model = (IDataModel) new BattlegroundsEmoteCollectionPageDataModel();
    return model as BattlegroundsEmoteCollectionPageDataModel;
  }

  public override void UpdateCurrentPageCardLocks(bool playSound = false)
  {
    base.UpdateCurrentPageCardLocks(playSound);
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
      collectionCardVisual.ShowLock(CollectionCardVisual.LockType.NONE);
  }

  public void UpdateFavoriteHeroSkins(CollectionUtils.ViewMode mode)
  {
    bool flag = mode == CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS;
    if ((UnityEngine.Object) this.m_heroSkinsDecor != (UnityEngine.Object) null)
      this.m_heroSkinsDecor.SetActive(flag);
    if (!flag)
      return;
    int num = 0;
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        Actor actor = collectionCardVisual.GetActor();
        BaconCollectionHeroSkin component = actor.GetComponent<BaconCollectionHeroSkin>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
        {
          component.ShowShadow(actor.IsShown());
          EntityDef entityDef = actor.GetEntityDef();
          if (entityDef != null)
            component.ShowFavoriteBanner(BaconHeroSkinUtils.IsBattlegroundsHeroSkinFavorited(entityDef));
        }
        else
          continue;
      }
      if (num < this.m_heroSkinFrames.Length)
        this.m_heroSkinFrames[num++].SetActive(collectionCardVisual.IsShown());
    }
  }

  public void UpdateFavoriteGuideSkins(CollectionUtils.ViewMode mode)
  {
    bool flag = mode == CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS;
    if ((UnityEngine.Object) this.m_guideSkinsDecor != (UnityEngine.Object) null)
      this.m_guideSkinsDecor.SetActive(flag);
    if (!flag)
      return;
    int num = 0;
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        Actor actor = collectionCardVisual.GetActor();
        BaconCollectionGuideSkin component = actor.GetComponent<BaconCollectionGuideSkin>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
        {
          component.ShowShadow(actor.IsShown());
          EntityDef entityDef = actor.GetEntityDef();
          if (entityDef != null)
            component.ShowFavoriteBanner(BaconHeroSkinUtils.IsBattlegroundsGuideSkinFavorited(entityDef));
        }
        else
          continue;
      }
      if (num < this.m_guideSkinFrames.Length)
        this.m_guideSkinFrames[num++].SetActive(collectionCardVisual.IsShown());
    }
  }

  public void UpdateFavoriteBoardSkins(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS)
      return;
    foreach (BattlegroundsBoardSkinDataModel boardSkin in this.m_BoardSkinsWidget.GetDataModel<BattlegroundsBoardSkinCollectionPageDataModel>().BoardSkinList)
      boardSkin.IsFavorite = CollectionManager.Get().IsFavoriteBattlegroundsBoardSkin(BattlegroundsBoardSkinId.FromTrustedValue(boardSkin.BoardDbiId));
  }

  public void UpdateFavoriteFinisherSkins(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS)
      return;
    foreach (BattlegroundsFinisherDataModel finisher in this.m_FinishersWidget.GetDataModel<BattlegroundsFinisherCollectionPageDataModel>().FinisherList)
      finisher.IsFavorite = CollectionManager.Get().IsFavoriteBattlegroundsFinisher(BattlegroundsFinisherId.FromTrustedValue(finisher.FinisherDbiId));
  }

  public void UpdateHeroSkinHeroPowers(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS)
      return;
    foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        Actor actor = collectionCardVisual.GetActor();
        BaconCollectionHeroSkin component = actor.GetComponent<BaconCollectionHeroSkin>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
        {
          EntityDef entityDef = actor.GetEntityDef();
          if (entityDef != null)
          {
            string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(entityDef.GetCardId());
            component.SetHeroPower(powerCardIdFromHero);
          }
        }
      }
    }
  }

  public void UpdateHeroSkinNames(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS)
      return;
    this.StartCoroutine(this.WaitThenUpdateHeroSkinNames(mode));
  }

  private IEnumerator WaitThenUpdateHeroSkinNames(CollectionUtils.ViewMode mode)
  {
    BaconCollectionPageDisplay collectionPageDisplay = this;
    yield return (object) null;
    foreach (CollectionCardVisual collectionCardVisual in collectionPageDisplay.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        CollectionHeroSkin component = collectionCardVisual.GetActor().GetComponent<CollectionHeroSkin>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
          component.ShowCollectionManagerText();
      }
    }
  }

  public void UpdateGuideSkinNames(CollectionUtils.ViewMode mode)
  {
    if (mode != CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS)
      return;
    this.StartCoroutine(this.WaitThenUpdateGuideSkinNames(mode));
  }

  public void SetEmoteEquippedState(BattlegroundsEmoteId emoteId, bool isEquipped)
  {
    foreach (BattlegroundsEmoteDataModel emote in this.GetOrCreateEmoteCollectionPageDataModel().EmoteList)
    {
      if (emote.EmoteDbiId.Equals(emoteId.ToValue()))
        emote.IsEquipped = isEquipped;
    }
  }

  private IEnumerator WaitThenUpdateGuideSkinNames(CollectionUtils.ViewMode mode)
  {
    BaconCollectionPageDisplay collectionPageDisplay = this;
    yield return (object) null;
    foreach (CollectionCardVisual collectionCardVisual in collectionPageDisplay.m_collectionCardVisuals)
    {
      if (collectionCardVisual.IsShown())
      {
        BaconCollectionGuideSkin component = collectionCardVisual.GetActor().GetComponent<BaconCollectionGuideSkin>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
          component.ShowCollectionManagerText();
      }
    }
  }

  public override void ShowNoMatchesFound(
    bool show,
    CollectionManager.FindCardsResult findResults = null,
    bool showHints = true)
  {
    this.m_noMatchFoundObject.SetActive(show);
    this.m_noMatchExplanationText.Text = GameStrings.Get("GLUE_COLLECTION_NO_RESULTS");
  }

  public override void SetPageType(FormatType inputFormatType)
  {
  }

  public void SetHeroSkins()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_COLLECTION_MANAGER_HERO_SKINS_TITLE"));
    BaconCollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, BaconCollectionPageDisplay.HEADER_CLASS.HEROSKINS);
  }

  public void SetGuideSkins()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_GUIDE_SKINS_TITLE"));
    BaconCollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, BaconCollectionPageDisplay.HEADER_CLASS.GUIDESKINS);
  }

  public void SetBoardSkins()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_BOARD_SKINS_TITLE"));
    BaconCollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, BaconCollectionPageDisplay.HEADER_CLASS.BOARDSKINS);
  }

  public void SetFinishers()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_FINISHERS_TITLE"));
    BaconCollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, BaconCollectionPageDisplay.HEADER_CLASS.FINISHERS);
  }

  public void SetEmotes()
  {
    this.SetPageNameText(GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_EMOTES_TITLE"));
    BaconCollectionPageDisplay.SetPageFlavorTextures(this.m_pageFlavorHeader, BaconCollectionPageDisplay.HEADER_CLASS.EMOTES);
  }

  private void BoardSkinDisplayEventListener(string eventName)
  {
    EventDataModel dataModel = this.m_BoardSkinsWidgetInstance.GetDataModel<EventDataModel>();
    if (dataModel == null)
    {
      Log.All.PrintError("No event data model attached to BaconCollectionPageDisplay");
    }
    else
    {
      BattlegroundsBoardSkinDataModel payload = (BattlegroundsBoardSkinDataModel) dataModel.Payload;
      if (!(eventName == "BOARD_SKIN_clicked"))
      {
        if (!(eventName == "BOARD_SKIN_hover_end"))
          return;
        BaconCollectionPageDisplay.MarkBoardSeen(payload);
      }
      else
      {
        BattlegroundsBoardSkinCollectionPageDataModel collectionPageDataModel = this.GetOrCreateBoardCollectionPageDataModel();
        BaconCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay;
        if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
          Log.CollectionManager.PrintError("BaconCollectionPageDisplay.BOARD_SKIN_clicked - BaconCollectionDisplay is null!");
        collectibleDisplay.ShowBoardDetailsDisplay(payload, collectionPageDataModel);
        BaconCollectionPageDisplay.MarkBoardSeen(payload);
      }
    }
  }

  private void FinisherDisplayEventListener(string eventName)
  {
    EventDataModel dataModel = this.m_FinishersWidgetInstance.GetDataModel<EventDataModel>();
    if (dataModel == null)
    {
      Log.All.PrintError("No event data model attached to BaconCollectionPageDisplay");
    }
    else
    {
      BattlegroundsFinisherDataModel payload = (BattlegroundsFinisherDataModel) dataModel.Payload;
      if (!(eventName == "FINISHER_clicked"))
      {
        if (!(eventName == "FINISHER_hover_end"))
          return;
        BaconCollectionPageDisplay.MarkFinisherSeen(payload);
      }
      else
      {
        BattlegroundsFinisherCollectionPageDataModel collectionPageDataModel = this.GetOrCreateFinisherCollectionPageDataModel();
        BaconCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay;
        if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
        {
          Log.CollectionManager.PrintError("BaconCollectionPageDisplay.FINISHER_clicked - BaconCollectionDisplay is null!");
        }
        else
        {
          collectibleDisplay.ShowFinisherDetailsDisplay(payload, collectionPageDataModel);
          BaconCollectionPageDisplay.MarkFinisherSeen(payload);
        }
      }
    }
  }

  private void EmoteDisplayEventListener(string eventName)
  {
    if (!(eventName == "EMOTE_clicked"))
    {
      if (!(eventName == "EMOTE_hover_end"))
      {
        if (!(eventName == "EMOTE_drag_started"))
        {
          if (!(eventName == "EMOTE_drag_released"))
            return;
          CollectionInputMgr.Get().DropBattlegroundsEmote(false, false);
        }
        else
          this.OnEmoteDragStart();
      }
      else
      {
        BattlegroundsEmoteDataModel eventEmoteDataModel = this.GetEventEmoteDataModel();
        if (eventEmoteDataModel == null)
          Log.CollectionManager.PrintError("Unable to retrieve emote from event");
        else
          BaconCollectionPageDisplay.MarkEmoteSeen(eventEmoteDataModel);
      }
    }
    else
    {
      if (CollectionInputMgr.Get().HasHeldEmote())
        return;
      BattlegroundsEmoteCollectionPageDataModel collectionPageDataModel = this.GetOrCreateEmoteCollectionPageDataModel();
      BaconCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay;
      if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      {
        Log.CollectionManager.PrintError("BaconCollectionPageDisplay.EMOTE_clicked - BaconCollectionDisplay is null!");
      }
      else
      {
        BattlegroundsEmoteDataModel eventEmoteDataModel = this.GetEventEmoteDataModel();
        if (eventEmoteDataModel == null)
        {
          Log.CollectionManager.PrintError("Unable to retrieve emote from event");
        }
        else
        {
          collectibleDisplay.ShowEmoteDetailsDisplay(eventEmoteDataModel, collectionPageDataModel);
          BaconCollectionPageDisplay.MarkEmoteSeen(eventEmoteDataModel);
        }
      }
    }
  }

  public override void MarkAllShownCardsSeen()
  {
    base.MarkAllShownCardsSeen();
    this.MarkAllShownBoardsSeen();
    this.MarkAllShownFinishersSeen();
    this.MarkAllShownEmotesSeen();
  }

  private static void MarkBoardSeen(BattlegroundsBoardSkinDataModel boardData)
  {
    if (boardData == null)
    {
      Error.AddDevFatal("BaconCollectionPageDisplay.MarkBoardSeen - null board data model!");
    }
    else
    {
      boardData.IsNew = false;
      CollectionManager.Get().MarkBattlegroundsBoardSkinSeen(BattlegroundsBoardSkinId.FromTrustedValue(boardData.BoardDbiId));
    }
  }

  private static void MarkFinisherSeen(BattlegroundsFinisherDataModel finisherData)
  {
    if (finisherData == null)
    {
      Error.AddDevFatal("BaconCollectionPageDisplay.MarkFinisherSeen - null finisher data model");
    }
    else
    {
      finisherData.IsNew = false;
      CollectionManager.Get().MarkBattlegroundsFinisherSeen(BattlegroundsFinisherId.FromTrustedValue(finisherData.FinisherDbiId));
    }
  }

  private static void MarkEmoteSeen(BattlegroundsEmoteDataModel emoteData)
  {
    if (emoteData == null)
    {
      Error.AddDevFatal("BaconCollectionPageDisplay.MarkEmoteSeen - null emote data model");
    }
    else
    {
      emoteData.IsNew = false;
      CollectionManager.Get().MarkBattlegroundsEmoteSeen(BattlegroundsEmoteId.FromTrustedValue(emoteData.EmoteDbiId));
    }
  }

  private void OnBoardDisplayReady(Widget widget)
  {
    if ((UnityEngine.Object) widget != (UnityEngine.Object) null)
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.BoardSkinDisplayEventListener));
    this.m_BoardSkinsWidgetInstance = widget;
  }

  private void OnFinisherDisplayReady(Widget widget)
  {
    if ((UnityEngine.Object) widget != (UnityEngine.Object) null)
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.FinisherDisplayEventListener));
    this.m_FinishersWidgetInstance = widget;
  }

  private void OnEmoteDisplayReady(Widget widget)
  {
    if ((UnityEngine.Object) widget != (UnityEngine.Object) null)
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.EmoteDisplayEventListener));
    this.m_EmotesWidgetInstance = widget;
  }

  private void MarkAllShownBoardsSeen()
  {
    IDataModel model;
    if (!this.m_BoardSkinsWidget.GetDataModel(565, out model))
      return;
    if (!(model is BattlegroundsBoardSkinCollectionPageDataModel collectionPageDataModel))
      Log.All.PrintError("BaconCollectionPageDisplay.MarkAllShownBoardsSeen - data model of unexpected type!");
    else if (collectionPageDataModel.BoardSkinList == null)
    {
      Log.All.PrintError("BaconCollectionPageDisplay.MarkAllShownBoardsSeen - data model list was null!");
    }
    else
    {
      foreach (BattlegroundsBoardSkinDataModel boardSkin in collectionPageDataModel.BoardSkinList)
        BaconCollectionPageDisplay.MarkBoardSeen(boardSkin);
    }
  }

  private void MarkAllShownFinishersSeen()
  {
    IDataModel model;
    if (!this.m_FinishersWidget.GetDataModel(568, out model))
      return;
    if (!(model is BattlegroundsFinisherCollectionPageDataModel collectionPageDataModel))
      Log.All.PrintError("BaconCollectionPageDisplay.MarkAllShownFinishersSeen - data model of unexpected type!");
    else if (collectionPageDataModel.FinisherList == null)
    {
      Log.All.PrintError("BaconCollectionPageDisplay.MarkAllShownFinishersSeen - data model list was null!");
    }
    else
    {
      foreach (BattlegroundsFinisherDataModel finisher in collectionPageDataModel.FinisherList)
        BaconCollectionPageDisplay.MarkFinisherSeen(finisher);
    }
  }

  private void MarkAllShownEmotesSeen()
  {
    IDataModel model;
    if (!this.m_EmotesWidget.GetDataModel(639, out model))
      return;
    if (!(model is BattlegroundsEmoteCollectionPageDataModel collectionPageDataModel))
      Log.All.PrintError("BaconCollectionPageDisplay.MarkAllShownEmotesSeen - data model of unexpected type!");
    else if (collectionPageDataModel.EmoteList == null)
    {
      Log.All.PrintError("BaconCollectionPageDisplay.MarkAllShownEmotesSeen - data model list was null!");
    }
    else
    {
      foreach (BattlegroundsEmoteDataModel emote in collectionPageDataModel.EmoteList)
        BaconCollectionPageDisplay.MarkEmoteSeen(emote);
    }
  }

  private void OnEmoteDragStart()
  {
    if ((UnityEngine.Object) this.m_EmotesWidgetInstance == (UnityEngine.Object) null)
      return;
    BaconCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.IsEmoteDetailsShowing())
      return;
    BattlegroundsEmoteDataModel eventEmoteDataModel = this.GetEventEmoteDataModel();
    if (eventEmoteDataModel == null)
      Log.CollectionManager.PrintError("Unable to retrieve emote from event");
    else if (!eventEmoteDataModel.IsOwned)
      Log.CollectionManager.PrintError("Emote not owned");
    else
      CollectionInputMgr.Get().GrabBattlegroundsEmote((IDataModel) eventEmoteDataModel, CollectionUtils.BattlegroundsModeDraggableType.CollectionEmote);
  }

  private BattlegroundsEmoteDataModel GetEventEmoteDataModel()
  {
    EventDataModel dataModel = this.m_EmotesWidgetInstance.GetDataModel<EventDataModel>();
    if (dataModel != null)
      return dataModel.Payload as BattlegroundsEmoteDataModel;
    Log.CollectionManager.PrintError("No event data model attached to BaconCollectionPageDisplay");
    return (BattlegroundsEmoteDataModel) null;
  }

  public static void SetPageFlavorTextures(
    GameObject header,
    BaconCollectionPageDisplay.HEADER_CLASS headerClass)
  {
    if ((UnityEngine.Object) header == (UnityEngine.Object) null)
      return;
    int num = (int) headerClass;
    float x = (float) (num / 8) * 0.5f;
    float y = (float) num / -8f;
    CollectiblePageDisplay.SetPageFlavorTextures(header, new UnityEngine.Vector2(x, y));
  }

  public enum HEADER_CLASS
  {
    INVALID = -1, // 0xFFFFFFFF
    HEROSKINS = 0,
    GUIDESKINS = 1,
    BOARDSKINS = 2,
    FINISHERS = 3,
    EMOTES = 4,
  }
}
