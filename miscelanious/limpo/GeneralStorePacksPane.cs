using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Services;
using Hearthstone;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class GeneralStorePacksPane : GeneralStorePane
{
  [CustomEditField(Sections = "Layout")]
  [SerializeField]
  public Vector3 m_packButtonSpacing;
  [CustomEditField(Sections = "Content")]
  [SerializeField]
  public int m_maxRibbons;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  [SerializeField]
  public string m_boosterSelectionSound;
  [SerializeField]
  [CustomEditField(Sections = "Purchase Flow")]
  public GameObject m_purchaseAnimationBlocker;
  private List<GeneralStorePackSelectorButton> m_packButtons = new List<GeneralStorePackSelectorButton>();
  private GeneralStorePacksContent m_packsContent;
  private bool m_paneInitialized;
  private bool m_inRemovingBundleFlow;
  private CardReward m_randomCardReward;
  private bool m_deprioritizeClassic;

  private void Awake()
  {
    this.m_packsContent = this.m_parentContent as GeneralStorePacksContent;
    this.m_purchaseAnimationBlocker.SetActive(false);
    if ((UnityEngine.Object) this.m_packsContent == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_packsContent is not the correct type: GeneralStorePacksContent");
    }
    else
    {
      NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
      StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnItemPurchased));
    }
  }

  private void OnDestroy()
  {
    NetCache service;
    if (ServiceManager.TryGet<NetCache>(out service))
      service.RemoveNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnItemPurchased));
  }

  public override void StoreShown(bool isCurrent)
  {
    if (!this.m_paneInitialized)
    {
      this.m_paneInitialized = true;
      this.SetupPackButtons();
      this.SetupInitialSelectedPack();
    }
    this.UpdatePackButtonPositions();
    this.UpdatePackButtonRecommendedIndicators();
    AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_ADVENTURE);
  }

  public override void PrePaneSwappedIn()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI || !this.m_inRemovingBundleFlow)
      return;
    this.OnPackSelectorButtonClicked(this.m_packButtons[0], this.m_packButtons[0].GetStorePackId());
    this.m_inRemovingBundleFlow = false;
  }

  public void RemoveFirstPurchaseBundle(float glowOutLength)
  {
    if (!StoreManager.IsFirstPurchaseBundleOwned())
      return;
    this.StartCoroutine(this.AnimateRemoveFirstPurchaseBundle(glowOutLength));
  }

  private void OnItemPurchased(Network.Bundle bundle, PaymentMethod purchaseMethod)
  {
    if ((Record) bundle == (Record) null || bundle.Items == null)
      return;
    foreach (Network.BundleItem bundleItem in bundle.Items)
    {
      StorePackId storePackId = this.m_packsContent.GetStorePackId();
      if ((Record) bundleItem != (Record) null && bundleItem.ItemType == ProductType.PRODUCT_TYPE_RANDOM_CARD && storePackId.Type == StorePackType.BOOSTER && storePackId.Id == 181)
      {
        this.OnRandomCardPurchased(this.m_randomCardReward);
        break;
      }
      if ((Record) bundleItem != (Record) null && bundleItem.ItemType == ProductType.PRODUCT_TYPE_HIDDEN_LICENSE && storePackId.Type == StorePackType.MODULAR_BUNDLE)
      {
        this.m_packsContent.AnimateModularBundleAfterPurchase(storePackId);
        break;
      }
    }
  }

  private void OnRandomCardPurchased(CardReward cardReward)
  {
    if ((UnityEngine.Object) this.m_packsContent == (UnityEngine.Object) null)
      Debug.LogWarningFormat("OnRandomCardPurchased() m_packsContent == null for cardID {0}", (object) (cardReward.Data as CardRewardData).CardID);
    else
      this.m_packsContent.FirstPurchaseBundlePurchased(cardReward);
  }

  private void OnPackSelectorButtonClicked(
    GeneralStorePackSelectorButton btn,
    StorePackId storePackId)
  {
    if (!this.m_parentContent.IsContentActive())
      return;
    this.m_packsContent.SetBoosterId(storePackId);
    foreach (GeneralStorePackSelectorButton packButton in this.m_packButtons)
      packButton.Unselect();
    btn.Select();
    Options.Get().SetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, btn.GetStorePackId().Id);
    Options.Get().SetInt(Option.LAST_SELECTED_STORE_PACK_TYPE, (int) btn.GetStorePackId().Type);
    if (string.IsNullOrEmpty(this.m_boosterSelectionSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_boosterSelectionSound);
  }

  private void SetupPackButtons()
  {
    Map<StorePackId, IStorePackDef> storePackDefs = this.m_packsContent.GetStorePackDefs();
    StorePackId storePackId1 = this.m_packsContent.GetStorePackId();
    bool flag1 = !HearthstoneApplication.IsPublic() && Vars.Key("ModularBundle.ShowAll").GetBool(false);
    foreach (KeyValuePair<StorePackId, IStorePackDef> keyValuePair in storePackDefs)
    {
      StorePackId storePackId = keyValuePair.Key;
      ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(storePackId);
      if (fromStorePackType != ProductType.PRODUCT_TYPE_BOOSTER || this.CanShowBooster(storePackId.Id))
      {
        if (GameUtils.IsHiddenLicenseBundleBooster(storePackId))
        {
          bool flag2 = false;
          bool flag3 = false;
          int countFromStorePackId = GameUtils.GetProductDataCountFromStorePackId(storePackId);
          for (int selectedIndex = 0; selectedIndex < countFromStorePackId; ++selectedIndex)
          {
            int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(storePackId, selectedIndex);
            Network.Bundle bundle = StoreManager.Get().EnumerateBundlesForProductType(fromStorePackType, true, dataFromStorePackId).FirstOrDefault<Network.Bundle>();
            if ((Record) bundle != (Record) null)
            {
              flag2 = true;
              if (!StoreManager.Get().IsProductAlreadyOwned(bundle))
                flag3 = true;
            }
          }
          bool flag4;
          if (storePackId.Type == StorePackType.MODULAR_BUNDLE)
          {
            ModularBundleDbfRecord record = GameDbf.ModularBundle.GetRecord(storePackId.Id);
            flag4 = flag1 || flag2 && (flag3 || record.ShowAfterPurchase);
          }
          else
            flag4 = flag3;
          if (!flag4)
            continue;
        }
        IStorePackDef storePackDef = keyValuePair.Value;
        GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) storePackDef.GetSelectorButtonPrefab(), AssetLoadingOptions.IgnorePrefabPosition);
        GameUtils.SetParent(gameObject, this.m_paneContainer, true);
        LayerUtils.SetLayer(gameObject, this.m_paneContainer.layer);
        GeneralStorePackSelectorButton newPackButton = gameObject.GetComponent<GeneralStorePackSelectorButton>();
        newPackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnPackSelectorButtonClicked(newPackButton, storePackId)));
        newPackButton.SetStorePackId(storePackId);
        if (storePackId == storePackId1)
        {
          newPackButton.Select();
          StoreManager.Get().SetCurrentlySelectedStorePack(storePackId1);
        }
        this.m_packButtons.Add(newPackButton);
      }
    }
    this.UpdatePackButtonPositions();
  }

  private bool CanShowBooster(int boosterDbId)
  {
    BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterDbId);
    return record != null && SpecialEventManager.Get().IsEventActive(record.BuyWithGoldEvent, false) && !GameUtils.IsBoosterWild(record);
  }

  private void SortPackButtons() => this.m_packButtons.Sort((Comparison<GeneralStorePackSelectorButton>) ((lhs, rhs) =>
  {
    bool flag1 = GameUtils.IsFirstPurchaseBundleBooster(lhs.GetStorePackId());
    bool flag2 = GameUtils.IsFirstPurchaseBundleBooster(rhs.GetStorePackId());
    if (flag1 != flag2)
      return !flag1 ? 1 : -1;
    bool flag3 = lhs.IsRecommendedForNewPlayer();
    bool flag4 = rhs.IsRecommendedForNewPlayer();
    bool flag5 = GameUtils.IsHiddenLicenseBundleBooster(lhs.GetStorePackId());
    bool flag6 = GameUtils.IsHiddenLicenseBundleBooster(rhs.GetStorePackId());
    if (flag5 != flag6)
      return !flag5 ? 1 : -1;
    bool flag7 = lhs.GetStorePackId().Type == StorePackType.MODULAR_BUNDLE;
    bool flag8 = rhs.GetStorePackId().Type == StorePackType.MODULAR_BUNDLE;
    if (flag7 != flag8)
      return !flag7 ? 1 : -1;
    if (flag7 & flag8)
    {
      ModularBundleDbfRecord record1 = (ModularBundleDbfRecord) lhs.GetRecord();
      ModularBundleDbfRecord record2 = (ModularBundleDbfRecord) rhs.GetRecord();
      int num1 = record1 == null ? 0 : record1.SortOrder;
      int num2 = record2 == null ? 0 : record2.SortOrder;
      if (num1 != num2)
        return Mathf.Clamp(num2 - num1, -1, 1);
      return (record1 == null ? 0 : record1.ID) >= (record2 == null ? 0 : record2.ID) ? 1 : -1;
    }
    if (flag3 != flag4)
      return !flag3 ? 1 : -1;
    bool flag9 = lhs.IsPreorder();
    bool flag10 = rhs.IsPreorder();
    if (flag9 != flag10)
      return !flag9 ? 1 : -1;
    bool flag11 = lhs.IsLatestExpansion();
    bool flag12 = rhs.IsLatestExpansion();
    if (flag11 != flag12)
      return !flag11 ? 1 : -1;
    BoosterDbfRecord record3 = (BoosterDbfRecord) lhs.GetRecord();
    BoosterDbfRecord record4 = (BoosterDbfRecord) rhs.GetRecord();
    bool flag13 = record3 != null && record3.ID == 1;
    bool flag14 = record4 != null && record4.ID == 1;
    if (flag13 != flag14)
      return this.m_deprioritizeClassic ? (!flag13 ? -1 : 1) : (!flag13 ? 1 : -1);
    int num3 = record3 == null ? 0 : record3.ListDisplayOrderCategory;
    int num4 = record4 == null ? 0 : record4.ListDisplayOrderCategory;
    if (num3 != num4)
      return Mathf.Clamp(num4 - num3, -1, 1);
    int num5 = record3 == null ? 0 : record3.ListDisplayOrder;
    return Mathf.Clamp((record4 == null ? 0 : record4.ListDisplayOrder) - num5, -1, 1);
  }));

  private void UpdatePackButtonPositions()
  {
    NetCache.NetCacheFeatures netObject1 = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject1 != null)
    {
      NetCache.NetCacheBoosters netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
      if (netObject2 != null)
      {
        int num = 0;
        NetCache.BoosterStack boosterStack = netObject2.GetBoosterStack(1);
        if (boosterStack != null)
          num = boosterStack.EverGrantedCount;
        this.m_deprioritizeClassic = netObject1.Store.NumClassicPacksUntilDeprioritize >= 0 && num >= netObject1.Store.NumClassicPacksUntilDeprioritize;
      }
    }
    this.SortPackButtons();
    Vector3 zero = Vector3.zero;
    Vector3 onNormal = Vector3.Normalize(this.m_packButtonSpacing);
    for (int index = 0; index < this.m_packButtons.Count; ++index)
    {
      GeneralStorePackSelectorButton packButton = this.m_packButtons[index];
      bool flag = packButton.HasPurchasableProducts();
      packButton.gameObject.SetActive(flag);
      if (flag)
      {
        packButton.transform.localPosition = zero;
        Vector3 vector3 = this.m_packButtonSpacing;
        if (packButton.m_useScrollableItemBoundsToStack)
        {
          UIBScrollableItem component = packButton.GetComponent<UIBScrollableItem>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            vector3 = Vector3.Project((Vector3) (Matrix4x4.TRS(Vector3.zero, component.transform.localRotation, component.transform.localScale) * (Vector4) component.m_size), onNormal);
        }
        zero += vector3;
      }
    }
  }

  private void UpdatePackButtonRecommendedIndicators()
  {
    int num = 0;
    foreach (GeneralStorePackSelectorButton packSelectorButton in this.m_packButtons.ToArray())
    {
      bool hideRibbon = num >= this.m_maxRibbons;
      if (packSelectorButton.UpdateRibbonIndicator(hideRibbon))
        ++num;
    }
  }

  private bool ShouldResetPackSelection()
  {
    List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(ProductType.PRODUCT_TYPE_BOOSTER, true);
    List<string> stringList1 = new List<string>((IEnumerable<string>) Options.Get().GetString(Option.SEEN_PACK_PRODUCT_LIST, string.Empty).Split(':'));
    bool flag = false;
    foreach (Network.Bundle bundle in bundlesForProduct)
    {
      List<string> stringList2 = stringList1;
      long? pmtProductId = bundle.PMTProductID;
      string str1 = pmtProductId.ToString();
      if (!stringList2.Contains(str1))
      {
        List<string> stringList3 = stringList1;
        pmtProductId = bundle.PMTProductID;
        string str2 = pmtProductId.ToString();
        stringList3.Add(str2);
        flag = true;
      }
    }
    Options.Get().SetString(Option.SEEN_PACK_PRODUCT_LIST, string.Join(":", stringList1.ToArray()));
    return flag;
  }

  private void SetupInitialSelectedPack()
  {
    StorePackId storePackId = new StorePackId();
    if (this.ShouldResetPackSelection())
    {
      Options.Get().SetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, 0);
      Options.Get().SetInt(Option.LAST_SELECTED_STORE_PACK_TYPE, 0);
    }
    else
    {
      storePackId.Id = Options.Get().GetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, 0);
      storePackId.Type = (StorePackType) Options.Get().GetInt(Option.LAST_SELECTED_STORE_PACK_TYPE, 0);
      StoreManager.Get().SetCurrentlySelectedStorePack(storePackId);
    }
    foreach (GeneralStorePackSelectorButton packButton in this.m_packButtons)
    {
      if (packButton.GetStorePackId() == storePackId)
      {
        this.m_packsContent.SetBoosterId(storePackId, true, true);
        packButton.Select();
        break;
      }
    }
  }

  private IEnumerator AnimateRemoveFirstPurchaseBundle(float glowOutLength)
  {
    this.m_purchaseAnimationBlocker.SetActive(true);
    this.m_inRemovingBundleFlow = true;
    yield return (object) new WaitForSeconds(glowOutLength + 1f);
    GeneralStorePackSelectorButton buttonToRemove = (GeneralStorePackSelectorButton) null;
    foreach (GeneralStorePackSelectorButton packButton in this.m_packButtons)
    {
      StorePackId storePackId = packButton.GetStorePackId();
      if (storePackId.Type == StorePackType.BOOSTER && storePackId.Id == 181)
      {
        buttonToRemove = packButton;
        break;
      }
    }
    if ((UnityEngine.Object) buttonToRemove != (UnityEngine.Object) null)
    {
      GeneralStore.Get().HidePacksPane(true);
      if (!(bool) UniversalInputManager.UsePhoneUI)
        yield return (object) new WaitForSeconds(0.25f);
      this.m_packButtons.Remove(buttonToRemove);
      UnityEngine.Object.Destroy((UnityEngine.Object) buttonToRemove.gameObject);
      this.UpdatePackButtonPositions();
      this.UpdatePackButtonRecommendedIndicators();
      if (!(bool) UniversalInputManager.UsePhoneUI)
      {
        this.OnPackSelectorButtonClicked(this.m_packButtons[0], this.m_packButtons[0].GetStorePackId());
        this.m_inRemovingBundleFlow = false;
        yield return (object) new WaitForSeconds(0.75f);
      }
      GeneralStore.Get().HidePacksPane(false);
    }
    this.m_purchaseAnimationBlocker.SetActive(false);
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    if (newNotices.FirstOrDefault<NetCache.ProfileNotice>(new Func<NetCache.ProfileNotice, bool>(this.WillStoreDisplayNotice)) == null)
      return;
    RewardUtils.GetRewards(newNotices)[0].LoadRewardObject(new Reward.DelOnRewardLoaded(this.RewardObjectLoaded));
  }

  public bool WillStoreDisplayNotice(NetCache.ProfileNotice notice) => this.WillStoreDisplayNotice(notice.Origin, notice.Type, notice.OriginData);

  public bool WillStoreDisplayNotice(
    NetCache.ProfileNotice.NoticeOrigin noticeOrigin,
    NetCache.ProfileNotice.NoticeType noticeType,
    long noticeOriginData)
  {
    if (noticeOrigin == NetCache.ProfileNotice.NoticeOrigin.FROM_PURCHASE && noticeType == NetCache.ProfileNotice.NoticeType.REWARD_CARD && (!((UnityEngine.Object) this.m_packsContent != (UnityEngine.Object) null) || this.m_packsContent.GetStorePackId().Type != StorePackType.BOOSTER ? 0 : (this.m_packsContent.GetStorePackId().Id == 181 ? 1 : 0)) != 0)
    {
      if (StoreManager.Get().IsSimpleCheckoutFeatureEnabled())
      {
        BattlePayProvider? nullable = StoreManager.Get().ActiveTransactionProvider();
        BattlePayProvider battlePayProvider = BattlePayProvider.BP_PROVIDER_BLIZZARD;
        if (nullable.GetValueOrDefault() == battlePayProvider & nullable.HasValue)
        {
          if (StoreManager.Get().IsPMTProductIDActiveTransaction(noticeOriginData))
            return true;
          goto label_7;
        }
      }
      if (StoreManager.Get().IsIdActiveTransaction(noticeOriginData))
        return true;
    }
label_7:
    return false;
  }

  private void RewardObjectLoaded(Reward reward, object callbackData)
  {
    reward.Hide();
    this.m_randomCardReward = reward as CardReward;
  }
}
