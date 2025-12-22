using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class ShopCardList
{
  private UIBScrollable m_scrollbar;
  private MiniSetDetailsDataModel m_dataModel;
  private CardTileDataModel m_clickedTile;
  private CardDataModel m_selectedCard = new CardDataModel()
  {
    CardId = ""
  };
  private Widget m_parentWidget;

  public ShopCardList(Widget parentWidget, UIBScrollable scrollbar)
  {
    this.m_parentWidget = parentWidget;
    this.m_scrollbar = scrollbar;
  }

  public void InitInput()
  {
    this.m_parentWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleMouseEvents));
    this.m_scrollbar.SetScrollImmediate(0.0f);
    this.m_scrollbar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.BindNoCard));
  }

  public void RemoveListeners()
  {
    this.m_parentWidget.RemoveEventListener(new Widget.EventListenerDelegate(this.HandleMouseEvents));
    this.m_scrollbar.RemoveTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.BindNoCard));
  }

  public void SetData(IEnumerable<CardTileDataModel> cardList, BoosterDbId boosterId) => this.BindDataModel(cardList, boosterId);

  public void SetData(IEnumerable<DeckCardDbfRecord> cardList, BoosterDbId boosterId)
  {
    DefLoader loader = DefLoader.Get();
    this.BindDataModel(cardList.GroupBy<DeckCardDbfRecord, int>((Func<DeckCardDbfRecord, int>) (cr => cr.CardId)).Select<IGrouping<int, DeckCardDbfRecord>, (EntityDef, int)>((Func<IGrouping<int, DeckCardDbfRecord>, (EntityDef, int)>) (g => (loader.GetEntityDef(g.Key), g.Count<DeckCardDbfRecord>()))).OrderByDescending<(EntityDef, int), TAG_RARITY>((Func<(EntityDef, int), TAG_RARITY>) (ed => ed.Item1.GetRarity())).ThenBy<(EntityDef, int), int>((Func<(EntityDef, int), int>) (ed => ed.Item1.GetCost())).Select<(EntityDef, int), CardTileDataModel>((Func<(EntityDef, int), CardTileDataModel>) (ed => new CardTileDataModel()
    {
      CardId = ed.Item1.GetCardId(),
      Count = ed.Item2,
      Premium = TAG_PREMIUM.NORMAL
    })), boosterId);
  }

  public void SetDataGhostNonCraftableCards(
    List<DeckCardDbfRecord> cardList,
    BoosterDbId boosterId,
    TAG_PREMIUM premium)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    this.BindDataModel(cardList.GroupBy<DeckCardDbfRecord, int>((Func<DeckCardDbfRecord, int>) (cr => cr.CardId)).Select<IGrouping<int, DeckCardDbfRecord>, (EntityDef, int, bool)>((Func<IGrouping<int, DeckCardDbfRecord>, (EntityDef, int, bool)>) (g =>
    {
      CollectibleCard card = collectionManager.GetCard(GameUtils.TranslateDbIdToCardId(g.Key), premium);
      return (card.GetEntityDef(), g.Count<DeckCardDbfRecord>(), card.IsCraftable);
    })).OrderBy<(EntityDef, int, bool), int>((Func<(EntityDef, int, bool), int>) (ed => !ed.IsCraftable ? 1 : 0)).ThenByDescending<(EntityDef, int, bool), TAG_RARITY>((Func<(EntityDef, int, bool), TAG_RARITY>) (ed => ed.Item1.GetRarity())).ThenBy<(EntityDef, int, bool), int>((Func<(EntityDef, int, bool), int>) (ed => ed.Item1.GetCost())).Select<(EntityDef, int, bool), CardTileDataModel>((Func<(EntityDef, int, bool), CardTileDataModel>) (ed => new CardTileDataModel()
    {
      CardId = ed.Item1.GetCardId(),
      Count = ed.Item2,
      Premium = ed.IsCraftable ? premium : TAG_PREMIUM.NORMAL,
      ForceGhostDisplayStyle = ed.IsCraftable ? CollectionDeckTileActor.GhostedState.NONE : CollectionDeckTileActor.GhostedState.NOT_INCLUDED
    })), boosterId);
    this.m_dataModel.SelectedCard.Premium = premium;
    this.m_selectedCard.Premium = premium;
  }

  private void BindDataModel(IEnumerable<CardTileDataModel> cards, BoosterDbId boosterId)
  {
    DataModelList<CardTileDataModel> dataModelList = new DataModelList<CardTileDataModel>();
    dataModelList.AddRange(cards);
    this.m_dataModel = new MiniSetDetailsDataModel()
    {
      CardTiles = dataModelList,
      Pack = new PackDataModel() { Type = boosterId },
      SelectedCard = this.m_selectedCard
    };
    this.BindNoCard();
    this.m_parentWidget.BindDataModel((IDataModel) this.m_dataModel);
  }

  public void SetPremium(TAG_PREMIUM premium)
  {
    foreach (CardTileDataModel cardTile in this.m_dataModel.CardTiles)
      cardTile.Premium = premium;
    this.m_dataModel.SelectedCard.Premium = premium;
    this.m_selectedCard.Premium = premium;
  }

  private void BindNoCard()
  {
    this.m_selectedCard.CardId = "";
    this.m_dataModel.SelectedCardNotIncluded = false;
    if (this.m_clickedTile == null)
      return;
    this.m_clickedTile.Selected = false;
    this.m_clickedTile = (CardTileDataModel) null;
  }

  private CardTileDataModel GetEventPayload() => this.m_parentWidget.GetDataModel<EventDataModel>().Payload as CardTileDataModel;

  private void HandleMouseEvents(string eventName)
  {
    if (this.m_scrollbar.IsTouchDragging())
      this.BindNoCard();
    else if (!(eventName == "TILE_OVER_code"))
    {
      if (!(eventName == "TILE_OUT_code"))
      {
        if (!(eventName == "TILE_CLICKED_code"))
        {
          if (!(eventName == "TILE_RELEASED_code"))
            return;
          this.GetEventPayload().Selected = false;
          this.m_clickedTile = (CardTileDataModel) null;
        }
        else
        {
          CardTileDataModel eventPayload = this.GetEventPayload();
          eventPayload.Selected = true;
          this.m_clickedTile = eventPayload;
        }
      }
      else
        this.BindNoCard();
    }
    else
    {
      CardTileDataModel eventPayload = this.GetEventPayload();
      CollectionManager collectionManager = CollectionManager.Get();
      this.m_selectedCard.CardId = eventPayload.CardId;
      this.m_selectedCard.Premium = eventPayload.Premium;
      if (eventPayload.ForceGhostDisplayStyle == CollectionDeckTileActor.GhostedState.NOT_INCLUDED)
      {
        this.m_dataModel.NotIncludedText = GameStrings.Format("GLUE_SHOP_CARD_INCLUDED_FREE", (object) GameStrings.GetCardSetName(collectionManager.GetCard(eventPayload.CardId, eventPayload.Premium).Set));
        this.m_dataModel.SelectedCardNotIncluded = true;
      }
      else
        this.m_dataModel.SelectedCardNotIncluded = false;
    }
  }
}
