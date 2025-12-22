using Blizzard.T5.AssetManager;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniSetProductPage : ProductPage
{
  public UIBScrollable m_scrollbar;
  private ShopCardList m_cardList;
  private Maskable m_maskable;

  public override void Open()
  {
    this.m_maskable = this.GetComponentInChildren<Maskable>();
    this.m_maskable.enabled = false;
    if ((UnityEngine.Object) this.m_container != (UnityEngine.Object) null)
      this.m_container.OverrideMusic(MusicPlaylistType.Invalid);
    this.m_cardList = new ShopCardList(this.m_widget, this.m_scrollbar);
    base.Open();
    this.OnOpened += new EventHandler(this.InitInput);
  }

  public override void Close()
  {
    base.Close();
    this.m_cardList.RemoveListeners();
  }

  public void InitInput(object sender, EventArgs e)
  {
    this.OnOpened -= new EventHandler(this.InitInput);
    this.m_cardList.InitInput();
    this.m_maskable.enabled = true;
  }

  protected override ProductDataModel GetFirstVariantToDisplay(
    ProductDataModel chosenProduct,
    ProductDataModel chosenVariant)
  {
    if (chosenProduct.Variants.Count == 2)
    {
      ProductDataModel productDataModel = (ProductDataModel) null;
      foreach (ProductDataModel variant in chosenProduct.Variants)
      {
        bool flag = variant.Tags.Contains("golden");
        variant.VariantName = flag ? GameStrings.Get("GLUE_STORE_PREMIUM_VARIATION_NAME_GOLDEN") : GameStrings.Get("GLUE_STORE_PREMIUM_VARIATION_NAME_NORMAL");
        if (productDataModel == null & flag)
          productDataModel = variant;
      }
      if (productDataModel != null)
        chosenVariant = productDataModel;
    }
    return chosenVariant;
  }

  protected override void OnProductSet()
  {
    base.OnProductSet();
    int itemId = this.Product.Items.First<RewardItemDataModel>().ItemId;
    MiniSetDbfRecord record = GameDbf.MiniSet.GetRecord(itemId);
    DeckDbfRecord deckRecord = record.DeckRecord;
    BoosterDbId id = (BoosterDbId) record.BoosterRecord.ID;
    this.m_cardList.SetData((IEnumerable<DeckCardDbfRecord>) deckRecord.Cards, id);
    using (AssetHandle<GameObject> assetHandle = ShopUtils.LoadStorePackPrefab(id))
    {
      if (!((UnityEngine.Object) this.m_container != (UnityEngine.Object) null))
        return;
      this.m_container.OverrideMusic(assetHandle.Asset.GetComponent<StorePackDef>().GetMiniSetPlaylist());
    }
  }

  public override void SelectVariant(ProductDataModel product)
  {
    base.SelectVariant(product);
    this.m_cardList.SetPremium(product.Tags.Contains("golden") ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL);
  }
}
