using Blizzard.T5.Core.Utils;
using Blizzard.Telemetry.WTCG.Client;
using Game.Shop;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSlot : ShopBrowserElement
{
  [SerializeField]
  private ShopSlot.SlotSize m_slotSize;
  protected BoxCollider m_boxCollider;
  protected Widget m_widget;
  protected ShopSection m_section;
  protected bool m_isFilled;
  protected bool m_inputBlocked;
  private ShopCard m_shopCardTelemetry;

  [Overridable]
  public string Size
  {
    get => this.m_slotSize.ToString();
    set
    {
      this.m_slotSize = ShopSlot.GetSlotSizeFromString(value);
      this.UpdateSize();
    }
  }

  private void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_shopCardTelemetry = new ShopCard();
    this.m_isFilled = false;
    if ((UnityEngine.Object) this.m_boxCollider == (UnityEngine.Object) null)
    {
      this.m_boxCollider = this.GetComponent<BoxCollider>();
      Clickable component = this.GetComponent<Clickable>();
      if ((bool) (UnityEngine.Object) component)
      {
        component.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRelease));
        component.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRollOver));
        component.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRollOut));
      }
    }
    this.RefreshEnableInput();
    this.UpdateSize();
    this.DataModel = new ShopBrowserButtonDataModel()
    {
      SlotWidth = this.Width,
      SlotHeight = this.Height
    };
    this.m_section = GameObjectUtils.FindComponentInParents<ShopSection>(this.gameObject);
    if (!((UnityEngine.Object) this.m_section != (UnityEngine.Object) null))
      return;
    this.m_section.RegisterSlot(this);
  }

  public bool IsFilled => this.m_isFilled;

  public void Reset()
  {
    if (!this.m_isFilled)
      return;
    this.SetBrowserButton(new ShopBrowserButtonDataModel()
    {
      SlotWidth = this.Width,
      SlotHeight = this.Height
    });
  }

  public static Vector2 GetSlotSizeDims(ShopSlot.SlotSize size)
  {
    switch (size)
    {
      case ShopSlot.SlotSize.M:
        return new Vector2(1f, 1.3f);
      case ShopSlot.SlotSize.MWide:
        return new Vector2(2f, 1.3f);
      case ShopSlot.SlotSize.L:
        return new Vector2(2f, 2f);
      case ShopSlot.SlotSize.XL:
        return new Vector2(3f, 2f);
      case ShopSlot.SlotSize.XXL:
        return new Vector2(4f, 2f);
      default:
        return new Vector2(0.0f, 0.0f);
    }
  }

  public static ShopSlot.SlotSize GetSlotSizeFromString(string size)
  {
    string upper = size.ToUpper();
    if (upper == "M")
      return ShopSlot.SlotSize.M;
    if (upper == "MWIDE")
      return ShopSlot.SlotSize.MWide;
    if (upper == "L")
      return ShopSlot.SlotSize.L;
    if (upper == "XL")
      return ShopSlot.SlotSize.XL;
    return upper == "XXL" ? ShopSlot.SlotSize.XXL : ShopSlot.SlotSize.Custom;
  }

  protected ShopBrowserButtonDataModel DataModel
  {
    get
    {
      IDataModel model = (IDataModel) null;
      if ((UnityEngine.Object) this.m_widget != (UnityEngine.Object) null)
        this.m_widget.GetDataModel(19, out model);
      return model as ShopBrowserButtonDataModel;
    }
    set
    {
      if (!((UnityEngine.Object) this.m_widget != (UnityEngine.Object) null))
        return;
      value.DisplayProduct = value.DisplayProduct ?? ProductFactory.CreateEmptyProductDataModel();
      this.m_widget.BindDataModel((IDataModel) value);
      this.UpdateShopCardTelemetry();
    }
  }

  public ShopCard GetShopCardTelemetry()
  {
    this.UpdateShopCardTelemetryTimeRemaining();
    return this.m_shopCardTelemetry;
  }

  public void SetBrowserButton(ShopBrowserButtonDataModel buttonDataModel)
  {
    this.DataModel = buttonDataModel;
    ProductDataModel productDataModel = buttonDataModel.DisplayProduct ?? ProductFactory.CreateEmptyProductDataModel();
    this.m_isFilled = productDataModel != ProductFactory.CreateEmptyProductDataModel();
    if (this.CheckEmoteFanLayout(productDataModel.RewardList.Items))
      productDataModel.Tags.Add("use_bgemote_fan_layout");
    this.PileEmotes(productDataModel.RewardList.Items);
    this.RefreshEnableInput();
    if ((UnityEngine.Object) this.m_widget != (UnityEngine.Object) null)
      this.m_widget.BindDataModel((IDataModel) productDataModel);
    this.StartCoroutine(this.DisableChildCollidersCoroutine());
    this.UpdateSize();
  }

  public void EnableInput(bool enabled)
  {
    this.m_inputBlocked = !enabled;
    this.RefreshEnableInput();
  }

  protected void RefreshEnableInput()
  {
    if ((UnityEngine.Object) this.m_boxCollider == (UnityEngine.Object) null)
      return;
    this.m_boxCollider.enabled = this.m_isFilled && !this.m_inputBlocked;
  }

  private void UpdateSize()
  {
    if (this.m_slotSize != ShopSlot.SlotSize.Custom)
    {
      Vector2 slotSizeDims = ShopSlot.GetSlotSizeDims(this.m_slotSize);
      this.Bounds.Set((float) (-(double) slotSizeDims.x / 2.0), (float) (-(double) slotSizeDims.y / 2.0), slotSizeDims.x, slotSizeDims.y);
    }
    this.OnElementBoundsChanged();
  }

  protected override void OnElementBoundsChanged()
  {
    ShopBrowserButtonDataModel dataModel = this.DataModel;
    if (dataModel != null)
    {
      dataModel.SlotWidth = this.Width;
      dataModel.SlotHeight = this.Height;
    }
    if (!((UnityEngine.Object) this.m_boxCollider != (UnityEngine.Object) null))
      return;
    this.m_boxCollider.transform.localPosition = new Vector3(this.Bounds.center.x, this.m_boxCollider.transform.localPosition.y, this.Bounds.center.y);
    this.m_boxCollider.size = new Vector3(this.Width, this.m_boxCollider.size.y, this.Height);
  }

  private void OnRelease(UIEvent e)
  {
    if (!this.m_isFilled)
    {
      Log.Store.PrintWarning("Ignoring click on shop slot that is not filled. The clickable for this ShopSlot should be disabled.");
    }
    else
    {
      TelemetryManager.Client().SendShopCardClick(this.m_shopCardTelemetry, StoreManager.Get().CurrentShopType.ToString());
      ProductDataModel displayProduct = this.DataModel.DisplayProduct;
      if (displayProduct == null)
        return;
      if (displayProduct.Tags.Contains("vc") && displayProduct.Variants.Count > 1)
      {
        ProductDataModel vcVariant = displayProduct;
        ProductDataModel specialOfferVariant;
        if (VariantUtils.TryFindSpecialOfferVariant(displayProduct, out specialOfferVariant))
          vcVariant = specialOfferVariant;
        global::Shop.Get().OpenVirtualCurrencyPurchase(vcVariant, displayProduct);
      }
      else
        global::Shop.Get().OpenProductPage(displayProduct);
    }
  }

  private void OnRollOver(UIEvent e)
  {
    if (this.DataModel == null)
      return;
    this.DataModel.Hovered = true;
  }

  private void OnRollOut(UIEvent e)
  {
    if (this.DataModel == null)
      return;
    this.DataModel.Hovered = false;
  }

  private IEnumerator DisableChildCollidersCoroutine()
  {
    ShopSlot shopSlot = this;
    while (shopSlot.m_widget.IsChangingStates)
      yield return (object) null;
    foreach (Collider componentsInChild in shopSlot.GetComponentsInChildren<Collider>())
    {
      if (!((UnityEngine.Object) componentsInChild == (UnityEngine.Object) shopSlot.m_boxCollider))
        componentsInChild.enabled = false;
    }
  }

  private void UpdateShopCardTelemetry()
  {
    if ((UnityEngine.Object) this.m_section == (UnityEngine.Object) null)
      return;
    this.m_shopCardTelemetry = new ShopCard();
    ProductCatalog catalog = StoreManager.Get().Catalog;
    ProductTierDataModel tierDataModel = this.m_section.GetTierDataModel();
    if (tierDataModel != null)
    {
      this.m_shopCardTelemetry.SectionIndex = catalog.GetTiers_Current().IndexOf(tierDataModel);
      Network.ShopSection networkSection = catalog.GetNetworkSection(tierDataModel);
      if (networkSection != null)
        this.m_shopCardTelemetry.SectionName = networkSection.InternalName;
    }
    this.m_shopCardTelemetry.SlotIndex = this.m_section.GetSortedEnabledSlots().IndexOf(this);
    if (this.DataModel.DisplayProduct != ProductFactory.CreateEmptyProductDataModel())
      this.m_shopCardTelemetry.Product = new Product()
      {
        ProductId = this.DataModel.DisplayProduct.PmtId
      };
    this.UpdateShopCardTelemetryTimeRemaining();
  }

  private void UpdateShopCardTelemetryTimeRemaining()
  {
    if (!ProductId.IsValid(this.DataModel.DisplayProduct.PmtId))
      return;
    Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(ProductId.CreateFrom(this.DataModel.DisplayProduct.PmtId));
    if ((Record) fromPmtProductId == (Record) null)
      return;
    ProductAvailabilityRange availabilityRange = StoreManager.Get().GetBundleAvailabilityRange(fromPmtProductId);
    if (availabilityRange == null)
      return;
    DateTime? endDateTime = availabilityRange.EndDateTime;
    if (!endDateTime.HasValue)
      return;
    DateTime utcNow = DateTime.UtcNow;
    this.m_shopCardTelemetry.SecondsRemaining = (int) Math.Min((endDateTime.Value - utcNow).TotalSeconds, (double) int.MaxValue);
  }

  private void PileEmotes(DataModelList<RewardItemDataModel> items)
  {
    DataModelList<BattlegroundsEmoteDataModel> dataModelList = new DataModelList<BattlegroundsEmoteDataModel>();
    List<int> intList = new List<int>();
    for (int index = 0; index < items.Count; ++index)
    {
      RewardItemDataModel rewardItemDataModel = items[index];
      if (rewardItemDataModel.ItemType == RewardItemType.BATTLEGROUNDS_EMOTE)
      {
        dataModelList.Add(rewardItemDataModel.BGEmote);
        intList.Add(index);
      }
    }
    if (dataModelList.Count <= 1 || dataModelList.Count == items.Count)
      return;
    RewardItemDataModel rewardItemDataModel1 = new RewardItemDataModel()
    {
      ItemType = RewardItemType.BATTLEGROUNDS_EMOTE_PILE,
      ItemId = 0,
      BGEmotePile = dataModelList
    };
    items.Insert(intList[0], rewardItemDataModel1);
    for (int index = intList.Count - 1; index >= 0; --index)
      items.RemoveAt(intList[index] + 1);
  }

  private bool CheckEmoteFanLayout(DataModelList<RewardItemDataModel> items)
  {
    int num = 0;
    foreach (RewardItemDataModel rewardItemDataModel in items)
    {
      if (rewardItemDataModel.ItemType == RewardItemType.BATTLEGROUNDS_EMOTE)
        ++num;
    }
    return num == 6 && items.Count == num;
  }

  public enum SlotSize
  {
    Custom,
    M,
    MWide,
    L,
    XL,
    XXL,
  }
}
