using PegasusUtil;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class GeneralStorePackSelectorButton : PegUIElement
{
  public UberText m_packText;
  public HighlightState m_highlight;
  public GameObject m_ribbonIndicator;
  public UberText m_ribbonIndicatorText;
  public GameObject m_packAmountBanner;
  public UberText m_packAmountBannerText;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_selectSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_unselectSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_mouseOverSound;
  public bool m_checkNewPlayer;
  [CustomEditField(Parent = "m_checkNewPlayer")]
  public int m_recommendedExpertSetOwnedCardCount = 100;
  public bool m_useScrollableItemBoundsToStack;
  private bool m_selected;
  private DbfRecord m_dbfRecord;
  private StorePackId m_storePackId;
  private bool m_isLatestExpansion;
  private float m_collectionManagerLastModifiedTime = float.NaN;
  private bool m_cachedRecommendedForNewPlayer;

  public void SetStorePackId(StorePackId storePackId)
  {
    this.m_storePackId = storePackId;
    if (storePackId.Type == StorePackType.BOOSTER)
    {
      this.m_dbfRecord = (DbfRecord) GameDbf.Booster.GetRecord(storePackId.Id);
      this.m_isLatestExpansion = GameUtils.IsBoosterLatestActiveExpansion(storePackId.Id);
      this.SetBoosterName((string) ((BoosterDbfRecord) this.m_dbfRecord).Name);
    }
    else
    {
      if (storePackId.Type != StorePackType.MODULAR_BUNDLE)
        return;
      ModularBundleDbfRecord record = GameDbf.ModularBundle.GetRecord(storePackId.Id);
      this.m_dbfRecord = (DbfRecord) record;
      this.SetBoosterName((string) record.Name);
      if (!((Object) this.m_packAmountBanner != (Object) null) || !((Object) this.m_packAmountBannerText != (Object) null))
        return;
      if (record.SelectorPackAmountBanner > 0)
      {
        this.m_packAmountBanner.SetActive(true);
        this.m_packAmountBannerText.Text = record.SelectorPackAmountBanner.ToString();
      }
      else
        this.m_packAmountBanner.SetActive(false);
    }
  }

  public void SetBoosterName(string name)
  {
    if (!((Object) this.m_packText != (Object) null))
      return;
    this.m_packText.Text = name;
  }

  public StorePackId GetStorePackId() => this.m_storePackId;

  public DbfRecord GetRecord() => this.m_dbfRecord;

  public void Select()
  {
    if (this.m_selected)
      return;
    this.m_selected = true;
    this.m_highlight.ChangeState(this.GetInteractionState() == PegUIElement.InteractionState.Up ? ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    if (string.IsNullOrEmpty(this.m_selectSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_selectSound);
  }

  public void Unselect()
  {
    if (!this.m_selected)
      return;
    this.m_selected = false;
    this.m_highlight.ChangeState(ActorStateType.NONE);
    if (string.IsNullOrEmpty(this.m_unselectSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_unselectSound);
  }

  public bool UpdateRibbonIndicator(bool hideRibbon)
  {
    if ((Object) this.m_ribbonIndicator == (Object) null || this.GetStorePackId().Type == StorePackType.INVALID)
      return false;
    if (hideRibbon)
    {
      this.m_ribbonIndicator.SetActive(false);
      return false;
    }
    bool flag = false;
    StorePackId storePackId = this.GetStorePackId();
    if (GameUtils.IsFirstPurchaseBundleBooster(storePackId))
    {
      flag = true;
      this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKBUY_BEST_VALUE");
    }
    else if (this.IsPreorder())
    {
      flag = true;
      this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKS_PREORDER_TEXT");
    }
    else if (GameUtils.IsLimitedTimeOffer(storePackId))
    {
      flag = true;
      this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKBUY_LIMITED_TIME");
    }
    else if (this.IsRecommendedForNewPlayer() && StoreManager.IsFirstPurchaseBundleOwned())
    {
      flag = true;
      this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKBUY_SUGGESTION");
    }
    else if (this.IsLatestExpansion())
    {
      flag = true;
      this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKS_LATEST_EXPANSION");
    }
    this.m_ribbonIndicator.SetActive(flag);
    return flag;
  }

  public bool HasPurchasableProducts()
  {
    StorePackId storePackId = this.GetStorePackId();
    int countFromStorePackId = GameUtils.GetProductDataCountFromStorePackId(storePackId);
    for (int selectedIndex = 0; selectedIndex < countFromStorePackId; ++selectedIndex)
    {
      if (StoreManager.Get().EnumerateBundlesForProductType(StorePackId.GetProductTypeFromStorePackType(storePackId), true, GameUtils.GetProductDataFromStorePackId(storePackId, selectedIndex)).Any<Network.Bundle>())
        return true;
    }
    if (storePackId.Type == StorePackType.BOOSTER)
    {
      BoosterDbfRecord record = GameDbf.Booster.GetRecord(storePackId.Id);
      if (record != null && SpecialEventManager.Get().IsEventActive(record.BuyWithGoldEvent, false))
        return true;
    }
    return false;
  }

  public bool IsRecommendedForNewPlayer()
  {
    float num1 = CollectionManager.Get().CollectionLastModifiedTime();
    if ((double) this.m_collectionManagerLastModifiedTime == (double) num1)
      return this.m_cachedRecommendedForNewPlayer;
    this.m_collectionManagerLastModifiedTime = num1;
    if (this.m_checkNewPlayer)
    {
      int num2 = CollectionManager.Get().NumCardsOwnedInSet(TAG_CARD_SET.EXPERT1);
      if (BoosterPackUtils.GetBoosterCount(1) * 5 + num2 <= this.m_recommendedExpertSetOwnedCardCount)
      {
        this.m_cachedRecommendedForNewPlayer = true;
        return true;
      }
    }
    this.m_cachedRecommendedForNewPlayer = false;
    return false;
  }

  public bool IsPreorder()
  {
    Network.Bundle preOrderBundle = (Network.Bundle) null;
    StorePackId storePackId = this.GetStorePackId();
    int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(storePackId);
    ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(storePackId);
    return StoreManager.Get().IsBoosterPreorderActive(dataFromStorePackId, fromStorePackType, out preOrderBundle);
  }

  public bool IsLatestExpansion() => this.m_isLatestExpansion && !this.IsPreorder();

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    base.OnOver(oldState);
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
    if (string.IsNullOrEmpty(this.m_mouseOverSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_mouseOverSound);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    base.OnOut(oldState);
    this.m_highlight.ChangeState(this.m_selected ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.NONE);
  }

  protected override void OnRelease()
  {
    base.OnRelease();
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
  }

  protected override void OnPress()
  {
    base.OnPress();
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }
}
