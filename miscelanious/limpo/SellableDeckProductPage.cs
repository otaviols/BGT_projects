using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SellableDeckProductPage : ProductPage
{
  [SerializeField]
  private AsyncReference classSelectorReference;
  [SerializeField]
  private AsyncReference m_singleDeckPouchReference;
  [SerializeField]
  private AsyncReference m_multiDeckPouchReference1;
  [SerializeField]
  private AsyncReference m_multiDeckPouchReference2;
  [SerializeField]
  private AsyncReference m_multiDeckPouchReference3;
  [SerializeField]
  private PlayMakerFSM m_turnPagePlaymaker;
  [SerializeField]
  private UIBScrollable m_scrollbar;
  private const int DK_DECK_COUNT = 3;
  private const string PAGE_ANIM_NAME_FORMAT = "PageTo{0}";
  private const string PAGE_ANIM_RESET_EVENT = "Reset";
  private const string PAGE_LEFT_PRESSED_EVENT = "PageLeft_code";
  private const string PAGE_RIGHT_PRESSED_EVENT = "PageRight_code";
  private const string ANIMATION_FINISHED_EVENT = "AnimationFinished_code";
  private ProductDataModel m_firstVariant;
  private Widget m_singleDeckPouchWidget;
  private ShopDeckPouchDisplay m_singleDeckPouchDisplay;
  private Widget[] m_multiDeckPouchWidgets = new Widget[3];
  private ShopDeckPouchDisplay[] m_multiDeckPouchDisplays = new ShopDeckPouchDisplay[3];
  private SellableDeckProductPage.VariantStyles m_variantStyle;
  private bool m_useMultiDeckInterface;
  private bool m_isAnimating;
  private (ProductDataModel variant, RewardItemDataModel item) m_queuedCardListDataToSetAfterAnimation;
  private List<DeckCardDbfRecord> m_tmpCardList = new List<DeckCardDbfRecord>();
  private HashSet<int> m_tmpCardSet = new HashSet<int>();
  private ShopCardList m_cardList;
  private PageInfoDataModel m_pageInfoDataModel = new PageInfoDataModel()
  {
    TotalPages = 4
  };

  protected override void Awake()
  {
    base.Awake();
    this.m_singleDeckPouchReference.RegisterReadyListener<Widget>((Action<Widget>) (x =>
    {
      this.m_singleDeckPouchWidget = x;
      this.m_singleDeckPouchDisplay = x.GetComponent<ShopDeckPouchDisplay>();
    }));
    this.m_multiDeckPouchReference1.RegisterReadyListener<Widget>((Action<Widget>) (x => this.OnMultiDeckWidgetReady(x, 0)));
    this.m_multiDeckPouchReference2.RegisterReadyListener<Widget>((Action<Widget>) (x => this.OnMultiDeckWidgetReady(x, 1)));
    this.m_multiDeckPouchReference3.RegisterReadyListener<Widget>((Action<Widget>) (x => this.OnMultiDeckWidgetReady(x, 2)));
  }

  protected override void Start()
  {
    base.Start();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.PaginationEventListener));
    this.m_widget.BindDataModel((IDataModel) this.m_pageInfoDataModel);
  }

  private void OnMultiDeckWidgetReady(Widget widget, int index)
  {
    this.m_multiDeckPouchWidgets[index] = widget;
    this.m_multiDeckPouchDisplays[index] = widget.GetComponent<ShopDeckPouchDisplay>();
    if (this.m_firstVariant == null)
      return;
    foreach (UnityEngine.Object multiDeckPouchWidget in this.m_multiDeckPouchWidgets)
    {
      if (multiDeckPouchWidget == (UnityEngine.Object) null)
        return;
    }
    this.SelectVariant(this.m_firstVariant);
    this.m_firstVariant = (ProductDataModel) null;
  }

  public override void Open()
  {
    this.m_cardList = new ShopCardList(this.m_widget, this.m_scrollbar);
    base.Open();
    this.OnOpened += new EventHandler(this.InitInput);
    if (this.m_variantStyle != SellableDeckProductPage.VariantStyles.Class)
      return;
    this.m_preBuyPopupInfo = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_SELLABLE_DECK_CONFIRMATION_HEADER"),
      m_text = GameStrings.Get("GLUE_SELLABLE_DECK_CONFIRMATION_BODY"),
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_showAlertIcon = true,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle
    };
  }

  public override void Close()
  {
    base.Close();
    this.m_cardList.RemoveListeners();
  }

  private void InitInput(object sender, EventArgs e)
  {
    this.OnOpened -= new EventHandler(this.InitInput);
    this.m_cardList.InitInput();
  }

  private void PaginationEventListener(string eventName)
  {
    if (!(eventName == "PageLeft_code"))
    {
      if (!(eventName == "PageRight_code"))
      {
        if (!(eventName == "AnimationFinished_code"))
          return;
        this.m_isAnimating = false;
        this.ShowQueuedCardList();
      }
      else
      {
        if (this.m_pageInfoDataModel.PageNumber >= this.m_pageInfoDataModel.TotalPages - 1 || this.m_isAnimating)
          return;
        this.SetNewPageNumber(this.m_pageInfoDataModel.PageNumber + 1);
      }
    }
    else
    {
      if (this.m_pageInfoDataModel.PageNumber <= 0 || this.m_isAnimating)
        return;
      this.SetNewPageNumber(this.m_pageInfoDataModel.PageNumber - 1);
    }
  }

  private void SetNewPageNumber(int newPageNumber)
  {
    this.SetTextAndPageButtonStates(newPageNumber);
    this.QueueCardListForPageNumber(this.m_productSelection.Variant, newPageNumber);
    this.m_isAnimating = true;
    this.m_turnPagePlaymaker.SendEvent(string.Format("PageTo{0}", (object) newPageNumber));
    this.m_pageInfoDataModel.PageNumber = newPageNumber;
  }

  protected override ProductDataModel GetFirstVariantToDisplay(
    ProductDataModel chosenProduct,
    ProductDataModel chosenVariant)
  {
    this.m_variantStyle = SellableDeckProductPage.VariantStyles.NoVariant;
    ProductDataModel variantToDisplay = chosenVariant;
    this.m_useMultiDeckInterface = false;
    if (chosenProduct == null || chosenProduct.Variants == null || chosenProduct.Variants.Count == 0)
      variantToDisplay = ProductFactory.CreateEmptyProductDataModel();
    else if (chosenProduct.Variants.Count > 1)
    {
      foreach (ProductDataModel variant in chosenProduct.Variants)
      {
        if (variant.Tags.Contains("golden"))
        {
          variantToDisplay = variant;
          this.m_variantStyle = SellableDeckProductPage.VariantStyles.Golden;
          break;
        }
        if (variant.Tags.Contains("show_class_variants"))
        {
          this.m_variantStyle = SellableDeckProductPage.VariantStyles.Class;
          break;
        }
      }
      switch (this.m_variantStyle)
      {
        case SellableDeckProductPage.VariantStyles.Golden:
          using (IEnumerator<ProductDataModel> enumerator = chosenProduct.Variants.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ProductDataModel current = enumerator.Current;
              current.VariantName = current.Tags.Contains("golden") ? GameStrings.Get("GLUE_STORE_PREMIUM_VARIATION_NAME_GOLDEN") : GameStrings.Get("GLUE_STORE_PREMIUM_VARIATION_NAME_NORMAL");
            }
            break;
          }
        case SellableDeckProductPage.VariantStyles.Class:
          AsyncReference selectorReference = this.classSelectorReference;
          if (selectorReference != null)
          {
            selectorReference.RegisterReadyListener<ShopClassVariantSelector>((Action<ShopClassVariantSelector>) (selector =>
            {
              selector.SetProductPage((ProductPage) this);
              selector.SetProduct(chosenProduct);
            }));
            break;
          }
          break;
        default:
          Log.Store.PrintWarning("[SellableDeckProductPage.GetOpeningVariant] Product {0} (ID: {1}) has variants but can't find one with a golden or a show_class_variant tag!", (object) chosenProduct.Name, (object) chosenProduct.PmtId);
          break;
      }
    }
    this.m_useMultiDeckInterface = variantToDisplay.Items.Count > 1;
    if (this.m_useMultiDeckInterface)
      this.ResetMultiPageVariables();
    this.m_firstVariant = variantToDisplay;
    return variantToDisplay;
  }

  private RewardItemDataModel GetDeckItemAt(ProductDataModel product, int index)
  {
    IEnumerable<RewardItemDataModel> decks = this.GetDecks(product);
    int num = 0;
    foreach (RewardItemDataModel deckItemAt in decks)
    {
      if (num >= index)
        return deckItemAt;
      ++num;
    }
    return (RewardItemDataModel) null;
  }

  public override void SelectVariant(ProductDataModel variant)
  {
    base.SelectVariant(variant);
    if (this.m_useMultiDeckInterface)
    {
      int index = 0;
      foreach (RewardItemDataModel deck in this.GetDecks(variant))
      {
        SellableDeckDbfRecord record = GameDbf.SellableDeck.GetRecord(deck.ItemId);
        if (record != null)
        {
          this.m_multiDeckPouchDisplays[index]?.SetDeckPouchData(this.m_multiDeckPouchWidgets[index], record.DeckTemplateRecord);
          ++index;
        }
      }
      this.QueueCardListForPageNumber(variant, this.m_pageInfoDataModel.PageNumber);
      this.ShowQueuedCardList();
    }
    else
    {
      this.m_singleDeckPouchDisplay.SetDeckPouchData(this.m_singleDeckPouchWidget, GameDbf.SellableDeck.GetRecord(this.GetDeckItemAt(variant, 0).ItemId).DeckTemplateRecord);
      this.PopulateCardListFromSingleDeckItem(variant, this.GetDeckItemAt(variant, 0));
    }
    this.ResetMultiPageVariables();
  }

  private void QueueCardListForPageNumber(ProductDataModel variant, int pageNumber)
  {
    if (pageNumber == 0)
      this.m_queuedCardListDataToSetAfterAnimation = (variant, (RewardItemDataModel) null);
    else
      this.m_queuedCardListDataToSetAfterAnimation = (variant, this.GetDeckItemAt(variant, pageNumber - 1));
  }

  private void ShowQueuedCardList()
  {
    if (this.m_queuedCardListDataToSetAfterAnimation.variant == null)
      return;
    if (this.m_queuedCardListDataToSetAfterAnimation.item == null)
      this.PopulateCardListWithUniqueCardsFromAllDecks(this.m_queuedCardListDataToSetAfterAnimation.variant);
    else
      this.PopulateCardListFromSingleDeckItem(this.m_queuedCardListDataToSetAfterAnimation.variant, this.m_queuedCardListDataToSetAfterAnimation.item);
    this.m_queuedCardListDataToSetAfterAnimation = ((ProductDataModel) null, (RewardItemDataModel) null);
  }

  private IEnumerable<RewardItemDataModel> GetDecks(
    ProductDataModel product)
  {
    return product.Items.Where<RewardItemDataModel>((Func<RewardItemDataModel, bool>) (x => x.ItemType == RewardItemType.SELLABLE_DECK)).OrderBy<RewardItemDataModel, int>((Func<RewardItemDataModel, int>) (x => x.ItemId)).Take<RewardItemDataModel>(3);
  }

  private void PopulateCardListWithUniqueCardsFromAllDecks(ProductDataModel variant)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    BoosterDbId boosterId = BoosterDbId.INVALID;
    TAG_PREMIUM premium = variant.Tags.Contains("golden") ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL;
    foreach (RewardItemDataModel deck in this.GetDecks(variant))
    {
      SellableDeckDbfRecord record = GameDbf.SellableDeck.GetRecord(deck.ItemId);
      if (boosterId == BoosterDbId.INVALID)
        boosterId = this.GetBoosterId(record);
      foreach (DeckCardDbfRecord card1 in record.DeckTemplateRecord.DeckRecord.Cards)
      {
        if (!this.m_tmpCardSet.Contains(card1.CardId))
        {
          string cardId = GameUtils.TranslateDbIdToCardId(card1.CardId);
          CollectibleCard card2 = collectionManager.GetCard(cardId, premium);
          if (card2.IsCraftable)
          {
            this.m_tmpCardSet.Add(card1.CardId);
            int num = card2.Rarity == TAG_RARITY.LEGENDARY ? 1 : 2;
            for (int index = 0; index < num; ++index)
              this.m_tmpCardList.Add(card1);
          }
        }
      }
    }
    List<CardTileDataModel> cardList = new List<CardTileDataModel>();
    if (variant.Items.Count > 0 && variant.Items[0].ItemType == RewardItemType.CARD)
      cardList.Add(new CardTileDataModel()
      {
        CardId = variant.Items[0].Card.CardId,
        Count = 1,
        Premium = variant.Items[0].Card.Premium
      });
    DefLoader loader = DefLoader.Get();
    IEnumerable<CardTileDataModel> collection = this.m_tmpCardList.GroupBy<DeckCardDbfRecord, int>((Func<DeckCardDbfRecord, int>) (cr => cr.CardId)).Select<IGrouping<int, DeckCardDbfRecord>, (EntityDef, int)>((Func<IGrouping<int, DeckCardDbfRecord>, (EntityDef, int)>) (g => (loader.GetEntityDef(g.Key), g.Count<DeckCardDbfRecord>()))).OrderByDescending<(EntityDef, int), TAG_RARITY>((Func<(EntityDef, int), TAG_RARITY>) (ed => ed.Item1.GetRarity())).ThenBy<(EntityDef, int), int>((Func<(EntityDef, int), int>) (ed => ed.Item1.GetCost())).Select<(EntityDef, int), CardTileDataModel>((Func<(EntityDef, int), CardTileDataModel>) (ed => new CardTileDataModel()
    {
      CardId = ed.Item1.GetCardId(),
      Count = ed.Item2,
      Premium = premium
    }));
    cardList.AddRange(collection);
    this.m_cardList.SetData((IEnumerable<CardTileDataModel>) cardList, boosterId);
    this.m_scrollbar.SetScrollImmediate(0.0f);
    this.m_tmpCardList.Clear();
    this.m_tmpCardSet.Clear();
  }

  private void PopulateCardListFromSingleDeckItem(
    ProductDataModel variant,
    RewardItemDataModel item)
  {
    SellableDeckDbfRecord record = GameDbf.SellableDeck.GetRecord(item.ItemId);
    BoosterDbId boosterId = this.GetBoosterId(record);
    DeckDbfRecord deckRecord = record.DeckTemplateRecord.DeckRecord;
    TAG_PREMIUM premium = variant.Tags.Contains("golden") ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL;
    this.m_cardList.SetDataGhostNonCraftableCards(deckRecord.Cards, boosterId, premium);
    this.m_scrollbar.SetScrollImmediate(0.0f);
  }

  private BoosterDbId GetBoosterId(SellableDeckDbfRecord rewardRecord)
  {
    BoosterDbId boosterId = BoosterDbId.INVALID;
    if (rewardRecord?.BoosterRecord != null)
    {
      int id = rewardRecord.BoosterRecord.ID;
      if (!Enum.IsDefined(typeof (BoosterDbId), (object) id))
        Log.Store.PrintWarning("[SellableDeckProductPage.GetBoosterId] The DB record {0} for product {1} (ID: {2}) uses an invalid BoosterDbId ({3})!", (object) rewardRecord.ID, (object) this.Product.Name, (object) this.Product.PmtId, (object) id);
      else
        boosterId = (BoosterDbId) rewardRecord.BoosterRecord.ID;
    }
    return boosterId;
  }

  private void SetTextAndPageButtonStates(int pageNumber)
  {
    this.m_pageInfoDataModel.InfoText = GameStrings.Format("GLUE_PROGRESSION_REWARD_TRACK_PAGE_NUMBER", (object) (pageNumber + 1), (object) this.m_pageInfoDataModel.TotalPages);
    this.m_widget.TriggerEvent(pageNumber != 0 ? "ENABLE_BUTTON_LEFT" : "DISABLE_BUTTON_LEFT");
    this.m_widget.TriggerEvent(pageNumber < this.m_pageInfoDataModel.TotalPages - 1 ? "ENABLE_BUTTON_RIGHT" : "DISABLE_BUTTON_RIGHT");
  }

  private void ResetMultiPageVariables()
  {
    this.m_queuedCardListDataToSetAfterAnimation = ((ProductDataModel) null, (RewardItemDataModel) null);
    this.m_turnPagePlaymaker.SendEvent("Reset");
    this.m_isAnimating = false;
    this.m_pageInfoDataModel.PageNumber = 0;
    this.m_pageInfoDataModel.TotalPages = 4;
    this.SetTextAndPageButtonStates(0);
  }

  private enum VariantStyles
  {
    NoVariant,
    Golden,
    Class,
  }
}
