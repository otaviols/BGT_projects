using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopClassVariantSelector : MonoBehaviour
{
  public AsyncReference m_chooseDeckReference;
  public AsyncReference[] m_classButtonReferences;
  private DeckChoiceDataModel m_deckChoiceDataModel;
  private DeckChoiceDataModel[] m_buttonDataModels;
  private Widget[] m_classButtonWidgets;
  private Widget m_chooseDeckWidget;
  private ProductPage m_productPage;
  private List<DeckTemplateDbfRecord> m_deckTemplates;
  private TAG_CLASS[] m_classByButtonIndex = new TAG_CLASS[11]
  {
    TAG_CLASS.DRUID,
    TAG_CLASS.HUNTER,
    TAG_CLASS.MAGE,
    TAG_CLASS.PALADIN,
    TAG_CLASS.PRIEST,
    TAG_CLASS.ROGUE,
    TAG_CLASS.SHAMAN,
    TAG_CLASS.WARLOCK,
    TAG_CLASS.WARRIOR,
    TAG_CLASS.DEMONHUNTER,
    TAG_CLASS.DEATHKNIGHT
  };

  protected virtual void Start()
  {
    this.m_classButtonWidgets = new Widget[this.m_classButtonReferences.Length];
    this.m_buttonDataModels = new DeckChoiceDataModel[this.m_classButtonReferences.Length];
    for (int index = 0; index < this.m_classButtonReferences.Length; ++index)
    {
      int classIndex = index;
      this.m_classButtonReferences[classIndex].RegisterReadyListener<Widget>((Action<Widget>) (w => this.SetupDataModelForButton(w, classIndex)));
    }
    this.m_deckChoiceDataModel = new DeckChoiceDataModel();
    this.m_chooseDeckReference.RegisterReadyListener<Widget>((Action<Widget>) (w =>
    {
      this.m_chooseDeckWidget = w;
      w.BindDataModel((IDataModel) this.m_deckChoiceDataModel);
    }));
  }

  public void SetProductPage(ProductPage productPage) => this.m_productPage = productPage;

  public void SetProduct(ProductDataModel product)
  {
    DataModelList<ProductDataModel> variants = product.Variants;
    int count = variants.Count;
    if (count > this.m_classButtonWidgets.Length)
      return;
    for (int index = 0; index < count; ++index)
      this.SetupDataModelForButton(index, variants[index]);
  }

  public void SetSelectedButtonIndex(int index)
  {
    this.m_deckChoiceDataModel = this.m_classButtonWidgets[index].GetDataModel<DeckChoiceDataModel>();
    if (!((UnityEngine.Object) this.m_productPage != (UnityEngine.Object) null))
      return;
    this.m_productPage.SelectVariantByIndex(index);
  }

  private void SetupDataModelForButton(Widget w, int index)
  {
    string str = this.m_classByButtonIndex[index].ToString();
    DeckChoiceDataModel deckChoiceDataModel = new DeckChoiceDataModel();
    deckChoiceDataModel.ButtonClass = str;
    this.m_classButtonWidgets[index] = w;
    this.m_buttonDataModels[index] = deckChoiceDataModel;
    w.BindDataModel((IDataModel) deckChoiceDataModel);
  }

  private void SetupDataModelForButton(int index, ProductDataModel productVariant)
  {
    DeckTemplateDbfRecord recordForProduct = this.GetDeckTemplateRecordForProduct(productVariant);
    if (recordForProduct == null)
      return;
    DeckChoiceDataModel deckChoiceDataModel = new DeckChoiceDataModel();
    int classId = recordForProduct.ClassId;
    deckChoiceDataModel.ChoiceClassID = classId;
    TAG_CLASS tagClass = (TAG_CLASS) classId;
    deckChoiceDataModel.ButtonClass = tagClass.ToString();
    this.m_buttonDataModels[index] = deckChoiceDataModel;
    this.m_classButtonWidgets[index].BindDataModel((IDataModel) deckChoiceDataModel);
    this.m_classButtonWidgets[index].TriggerEvent("Default");
  }

  private DeckTemplateDbfRecord GetDeckTemplateRecordForProduct(
    ProductDataModel productVariant)
  {
    if (productVariant.Items.Count < 1 || productVariant.Items[0].ItemType != RewardItemType.SELLABLE_DECK)
    {
      Log.Store.PrintWarning("[ShopClassVariantSelector.OnProductSet] Failed to find variant item!");
      return (DeckTemplateDbfRecord) null;
    }
    int itemId = productVariant.Items[0].ItemId;
    SellableDeckDbfRecord record = GameDbf.SellableDeck.GetRecord(itemId);
    if (record == null)
    {
      Log.Store.PrintWarning("[ShopClassVariantSelector.OnProductSet] Failed to find DB record {0}!", (object) itemId);
      return (DeckTemplateDbfRecord) null;
    }
    if (record.DeckTemplateRecord != null && record.DeckTemplateRecord.DeckRecord != null)
      return record.DeckTemplateRecord;
    Log.Store.PrintWarning("[ShopClassVariantSelector.OnProductSet] The DB record {0} does NOT have a deck template with a valid deck record!", (object) record.ID);
    return (DeckTemplateDbfRecord) null;
  }
}
