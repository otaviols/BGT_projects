using Blizzard.T5.AssetManager;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class GeneralStorePacksContent : GeneralStoreContent
{
  public StoreQuantityPrompt m_quantityPrompt;
  public GameObject m_packContainer;
  public GameObject m_packEmptyDisplay;
  public GeneralStorePacksContentDisplay m_packDisplay;
  [CustomEditField(Sections = "Pack Buy Buttons")]
  public GameObject m_packBuyContainer;
  [CustomEditField(Sections = "Pack Buy Buttons")]
  public MultiSliceElement m_packBuyButtonContainer;
  [CustomEditField(Sections = "Pack Buy Buttons")]
  public GeneralStorePackBuyButton m_packBuyButtonPrefab;
  [CustomEditField(Sections = "Pack Buy Buttons")]
  public MultiSliceElement m_packBuyFrameContainer;
  [CustomEditField(ListTable = true, Sections = "Pack Buy Buttons")]
  public List<GeneralStorePacksContent.ToggleableButtonFrame> m_toggleableButtonFrames = new List<GeneralStorePacksContent.ToggleableButtonFrame>();
  [CustomEditField(ListTable = true, Sections = "Pack Buy Buttons")]
  public List<GeneralStorePacksContent.MultiSliceEndCaps> m_buyBarEndCaps = new List<GeneralStorePacksContent.MultiSliceEndCaps>();
  [CustomEditField(Sections = "Pack Buy Buttons/Bonus Packs")]
  public GeneralStorePackBuyCallout m_packBuyBonusCallout;
  [CustomEditField(Sections = "Pack Buy Buttons/Bonus Packs")]
  public bool m_packBuyBonusCalloutOnlyOncePerSession;
  [CustomEditField(Sections = "Pack Buy Buttons/Bonus Packs")]
  public int m_packBuyBonusCalloutDebugForceDisplay;
  [CustomEditField(Sections = "Pack Buy Buttons/Bonus Packs")]
  public UberText m_packBuyBonusText;
  [CustomEditField(Sections = "Pack Buy Buttons/Limited Time Offer")]
  public UberText m_limitedTimeOfferText;
  [CustomEditField(Sections = "Pack Buy Buttons/Limited Time Offer")]
  public bool m_showLimitedTimeOfferText;
  [CustomEditField(Sections = "Pack Buy Buttons/Limited Time Offer")]
  public Transform m_limitedTimeOfferBone;
  [CustomEditField(Sections = "Pack Buy Buttons/Limited Time Offer")]
  public Transform m_limitedTimeOfferDustBone;
  [CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
  public GameObject m_packBuyPreorderContainer;
  [CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
  public GeneralStorePackBuyButton m_packBuyPreorderButtonPrefab;
  [CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
  public MultiSliceElement m_packBuyPreorderButtonContainer;
  [CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
  public MultiSliceElement m_packBuyPreorderFrameContainer;
  [CustomEditField(ListTable = true, Sections = "Pack Buy Buttons/Preorder")]
  public List<GeneralStorePacksContent.ToggleableButtonFrame> m_toggleablePreorderButtonFrames = new List<GeneralStorePacksContent.ToggleableButtonFrame>();
  [CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
  public UberText m_availableDateText;
  [CustomEditField(Sections = "China Button")]
  public UIBButton m_ChinaInfoButton;
  [CustomEditField(Sections = "Packs")]
  public int m_maxPackBuyButtons = 10;
  [CustomEditField(Sections = "Packs")]
  public GeneralStorePacksContent.LogoAnimation m_logoAnimation;
  [CustomEditField(Sections = "Animation")]
  public float m_packFlyOutAnimTime = 0.1f;
  [CustomEditField(Sections = "Animation")]
  public float m_packFlyOutDelay = 0.005f;
  [CustomEditField(Sections = "Animation")]
  public float m_packFlyInAnimTime = 0.2f;
  [CustomEditField(Sections = "Animation")]
  public float m_packFlyInDelay = 0.01f;
  [CustomEditField(Sections = "Animation")]
  public float m_boxFlyOutAnimTime = 0.2f;
  [CustomEditField(Sections = "Animation")]
  public float m_boxFlyOutDelay = 0.005f;
  [CustomEditField(Sections = "Animation")]
  public float m_boxFlyInAnimTime = 0.5f;
  [CustomEditField(Sections = "Animation")]
  public float m_boxFlyInDelay = 0.1f;
  [CustomEditField(Sections = "Animation")]
  public float m_boxFlyInXShake = 35f;
  [CustomEditField(Sections = "Animation")]
  public float m_boxStoreImpactTranslation = -70f;
  [CustomEditField(Sections = "Animation")]
  public float m_shakeObjectDelayMultiplier = 0.7f;
  [CustomEditField(Sections = "Animation")]
  public float m_backgroundFlipAnimTime = 0.5f;
  [CustomEditField(Sections = "Animation")]
  public float m_maxPackFlyInXShake = 20f;
  [CustomEditField(Sections = "Animation")]
  public float m_maxPackFlyOutXShake = 12f;
  [CustomEditField(Sections = "Animation")]
  public float m_packFlyShakeTime = 2f;
  [CustomEditField(Sections = "Animation")]
  public float m_backgroundFlipShake = 20f;
  [CustomEditField(Sections = "Animation")]
  public float m_backgroundFlipShakeDelay;
  [CustomEditField(Sections = "Animation")]
  public float m_PackYDegreeVariationMag = 2f;
  [CustomEditField(Sections = "Animation")]
  public float m_BoxYDegreeVariationMag = 1f;
  [CustomEditField(Sections = "Animation/Appear")]
  public GameObject m_logoAnimationStartBone;
  [CustomEditField(Sections = "Animation/Appear")]
  public GameObject m_logoAnimationEndBone;
  [CustomEditField(Sections = "Animation/Appear")]
  public MeshRenderer m_logoMesh;
  [CustomEditField(Sections = "Animation/Appear")]
  public MeshRenderer m_logoGlowMesh;
  [CustomEditField(Sections = "Animation/Appear")]
  public Vector3 m_punchAmount;
  [CustomEditField(Sections = "Animation/Appear")]
  public float m_logoHoldTime = 1f;
  [CustomEditField(Sections = "Animation/Appear")]
  public float m_logoDisplayPunchTime = 0.5f;
  [CustomEditField(Sections = "Animation/Appear")]
  public float m_logoIntroTime = 0.25f;
  [CustomEditField(Sections = "Animation/Appear")]
  public float m_logoOutroTime = 0.25f;
  [CustomEditField(Sections = "Animation/Appear")]
  public Vector3 m_logoAppearOffset;
  [CustomEditField(Sections = "Animation/Preorder")]
  public GeneralStoreRewardsCardBack m_preorderCardBackReward;
  [CustomEditField(Sections = "Sounds & Music", T = EditType.SOUND_PREFAB)]
  public string m_backgroundFlipSound;
  public const bool REQUIRE_REAL_MONEY_BUNDLE_OPTION = true;
  private static readonly int MAX_QUANTITY_BOUGHT_WITH_GOLD = 50;
  private const float FIRST_PURCHASE_BUNDLE_INIT_DELAY = 0.5f;
  private StorePackId m_selectedStorePackId;
  private List<GeneralStorePackBuyButton> m_packBuyButtons = new List<GeneralStorePackBuyButton>();
  private List<GeneralStorePackBuyButton> m_packPreorderBuyButtons = new List<GeneralStorePackBuyButton>();
  private int m_currentGoldPackQuantity = 1;
  private int m_visiblePackCount;
  private int m_visibleDustCount;
  private int m_visibleDustBonusCount;
  private bool m_selectedBoosterIsPrePurchase;
  private int m_lastBundleIndex;
  private int m_currentDisplay = -1;
  private Map<StorePackId, IStorePackDef> m_storePackDefs = new Map<StorePackId, IStorePackDef>();
  private HashSet<StorePackId> m_packBuyBonusCalloutSeenForPackId = new HashSet<StorePackId>();
  private const string PREV_PLAYLIST_NAME = "StorePrevCurrentPlaylist";
  private GeneralStorePacksContentDisplay m_packDisplay1;
  private GeneralStorePacksContentDisplay m_packDisplay2;
  private MeshRenderer m_logoMesh1;
  private MeshRenderer m_logoMesh2;
  private MeshRenderer m_logoGlowMesh1;
  private MeshRenderer m_logoGlowMesh2;
  private Coroutine m_logoAnimCoroutine;
  private Coroutine m_packAnimCoroutine;
  private Coroutine m_limitedTimeOfferAnimCoroutine;
  private Coroutine m_bonusPacksCalloutCoroutine;
  private Vector3 m_savedLocalPosition;
  private Vector3 m_limitedTimeTextOrigScale;
  private bool m_animatingLogo;
  private bool m_animatingPacks;
  private bool m_hasLogo;
  private bool m_waitingForBoxAnim;
  private bool m_loadingLogoTexture;
  private bool m_loadingLogoGlowTexture;

  public override void PostStoreFlipIn(bool animatedFlipIn)
  {
    this.UpdatePacksTypeMusic();
    this.AnimateLogo(animatedFlipIn);
    if (GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
    {
      this.HandleMoneyPackBuyButtonClick(this.GetFirstValidBundleIndex(this.m_selectedStorePackId));
      Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
      if (StoreManager.Get().ShouldShowFeaturedDustJar(currentMoneyBundle))
      {
        this.AnimatePacksFlying(this.m_visiblePackCount, true, showAsSingleStack: true);
        this.StartCoroutine(this.ShowFeaturedDustJar());
        if (GameUtils.IsFirstPurchaseBundleBooster(this.m_selectedStorePackId))
          this.ShowHiddenBundleCard();
      }
      else
      {
        float delay = 0.0f;
        if ((bool) UniversalInputManager.UsePhoneUI)
          delay = 1f;
        this.AnimatePacksFlying(this.m_visiblePackCount, delay: delay);
      }
    }
    else
    {
      this.AnimatePacksFlying(this.m_visiblePackCount, !animatedFlipIn);
      this.HideDust();
    }
    this.UpdateKoreaInfoButton();
    this.m_savedLocalPosition = this.gameObject.transform.localPosition;
  }

  public override void PreStoreFlipOut()
  {
    this.ResetAnimations();
    this.GetCurrentDisplay().ClearContents();
    this.UpdateKoreaInfoButton();
  }

  public override void StoreShown(bool isCurrent)
  {
    if (!isCurrent)
      return;
    this.AnimateLogo(false);
    if (GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
    {
      this.HandleMoneyPackBuyButtonClick(this.GetFirstValidBundleIndex(this.m_selectedStorePackId));
      Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
      if (StoreManager.Get().ShouldShowFeaturedDustJar(currentMoneyBundle))
      {
        this.AnimatePacksFlying(this.m_visiblePackCount, true, showAsSingleStack: true);
        this.StartCoroutine(this.ShowFeaturedDustJar());
        if (GameUtils.IsFirstPurchaseBundleBooster(this.m_selectedStorePackId))
          this.ShowHiddenBundleCard();
      }
      else
        this.AnimatePacksFlying(this.m_visiblePackCount, true);
    }
    else
    {
      Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
      bool flag = false;
      if ((Record) currentMoneyBundle != (Record) null)
        flag = StoreManager.Get().ShouldShowFeaturedDustJar(currentMoneyBundle);
      if (flag)
      {
        this.AnimatePacksFlying(this.m_visiblePackCount, true, showAsSingleStack: true, waitForLogo: ((bool) UniversalInputManager.UsePhoneUI));
      }
      else
      {
        this.AnimatePacksFlying(this.m_visiblePackCount, true);
        this.HideDust();
      }
    }
    this.UpdatePackBuyButtons();
    this.UpdatePacksTypeMusic();
    this.UpdateKoreaInfoButton();
  }

  public override void StoreHidden(bool isCurrent)
  {
    if (!isCurrent)
      return;
    this.ResetAnimations();
    this.GetCurrentDisplay().ClearContents();
  }

  public override bool IsPurchaseDisabled() => this.IsPackIdInvalid(this.m_selectedStorePackId);

  public override string GetMoneyDisplayOwnedText() => GameStrings.Get("GLUE_STORE_PACK_BUTTON_COST_OWNED_TEXT");

  public void SetBoosterId(StorePackId storePackId, bool forceImmediate = false, bool InitialSelection = false)
  {
    if (this.m_selectedStorePackId == storePackId)
      return;
    int num = this.IsPackIdInvalid(this.m_selectedStorePackId) ? 1 : 0;
    StoreManager.Get().SetCurrentlySelectedStorePack(storePackId);
    this.GetCurrentDisplay().ClearContents();
    this.m_visiblePackCount = 0;
    this.m_visibleDustCount = 0;
    this.m_selectedStorePackId = storePackId;
    if (num != 0)
      this.UpdateSelectedBundle();
    this.ResetAnimations();
    this.AnimateAndUpdateDisplay(storePackId, forceImmediate);
    if (InitialSelection)
      this.GetCurrentLogo().gameObject.SetActive(false);
    this.AnimateLogo(!forceImmediate, InitialSelection);
    bool flag = false;
    if (GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
    {
      this.HandleMoneyPackBuyButtonClick(this.GetFirstValidBundleIndex(this.m_selectedStorePackId));
      Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
      flag = StoreManager.Get().ShouldShowFeaturedDustJar(currentMoneyBundle);
      this.m_selectedBoosterIsPrePurchase = false;
    }
    else if (this.GetCurrentGoldBundle() != null)
      this.SetCurrentGoldBundle(this.GetCurrentGTAPPTransactionData());
    else if ((Record) this.GetCurrentMoneyBundle() != (Record) null)
    {
      this.HandleMoneyPackBuyButtonClick(this.m_lastBundleIndex);
      Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
      flag = StoreManager.Get().ShouldShowFeaturedDustJar(currentMoneyBundle);
      this.m_selectedBoosterIsPrePurchase = StoreManager.Get().IsProductPrePurchase(currentMoneyBundle);
    }
    Log.Store.Print("InitialSelection = {0}", (object) InitialSelection);
    if (GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
    {
      float delay = InitialSelection ? 0.5f : 0.0f;
      Log.Store.Print("InitialSelection delay={0}", (object) delay);
      if (flag)
      {
        this.AnimatePacksFlying(this.m_visiblePackCount, true, showAsSingleStack: true);
        this.StartCoroutine(this.ShowFeaturedDustJar());
        if (GameUtils.IsFirstPurchaseBundleBooster(this.m_selectedStorePackId))
          this.ShowHiddenBundleCard();
      }
      else
        this.AnimatePacksFlying(this.m_visiblePackCount, forceImmediate, delay);
    }
    else if (flag)
    {
      bool usePhoneUi = (bool) UniversalInputManager.UsePhoneUI;
      this.AnimatePacksFlying(this.m_visiblePackCount, true, showAsSingleStack: (!this.m_selectedBoosterIsPrePurchase), waitForLogo: usePhoneUi);
      this.StartCoroutine(this.ShowFeaturedDustJar(usePhoneUi));
    }
    else
    {
      this.AnimatePacksFlying(this.m_visiblePackCount, forceImmediate);
      this.HideDust();
    }
    this.UpdatePackBuyButtons();
    this.UpdatePacksDescriptionFromSelectedStorePack();
    this.UpdatePacksTypeMusic();
    this.UpdateKoreaInfoButton();
  }

  public StorePackId GetStorePackId() => this.m_selectedStorePackId;

  private int GetFirstValidBundleIndex(StorePackId storePackId)
  {
    int countFromStorePackId = GameUtils.GetProductDataCountFromStorePackId(storePackId);
    for (int selectedIndex = 0; selectedIndex < countFromStorePackId; ++selectedIndex)
    {
      int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(storePackId, selectedIndex);
      if (StoreManager.Get().EnumerateBundlesForProductType(ProductType.PRODUCT_TYPE_HIDDEN_LICENSE, true, dataFromStorePackId).Any<Network.Bundle>())
        return selectedIndex;
    }
    return 0;
  }

  public int GetLastBundleIndex() => this.m_lastBundleIndex;

  public bool SelectedBundleFeaturesDustJar()
  {
    Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
    return StoreManager.Get().ShouldShowFeaturedDustJar(currentMoneyBundle);
  }

  public void FirstPurchaseBundlePurchased(CardReward cardReward)
  {
    GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
    if ((UnityEngine.Object) currentDisplay == (UnityEngine.Object) null)
      Debug.LogWarningFormat("FirstPurchaseBundlePurchased() failed to get GeneralStorePacksContentDisplay for cardID {0}", (object) (cardReward.Data as CardRewardData).CardID);
    else
      currentDisplay.PurchaseBundleBox(cardReward);
  }

  public Map<StorePackId, IStorePackDef> GetStorePackDefs() => this.m_storePackDefs;

  public IStorePackDef GetStorePackDef(StorePackId packId)
  {
    IStorePackDef storePackDef = (IStorePackDef) null;
    this.m_storePackDefs.TryGetValue(packId, out storePackDef);
    return storePackDef;
  }

  public void ShakeStore(
    int numPacks,
    float maxXRotation,
    float delay = 0.0f,
    float translationAmount = 0.0f,
    int weight = 0)
  {
    if (numPacks == 0)
      return;
    int b = 1;
    float xRotationAmount = 0.0f;
    List<Network.Bundle> packBundles = this.GetPackBundles(false);
    if (this.m_selectedStorePackId.Type == StorePackType.BOOSTER)
    {
      foreach (Network.Bundle bundle in packBundles)
      {
        Network.BundleItem bundleItemFromBundle = GeneralStorePacksContent.GetPacksBundleItemFromBundle(bundle);
        if (!((Record) bundleItemFromBundle == (Record) null))
        {
          b = Mathf.Max(bundleItemFromBundle.Quantity, b);
          int num = b - 1;
          if (num == 0)
            return;
          xRotationAmount = (float) numPacks / (float) num * maxXRotation;
        }
      }
    }
    else if (this.m_selectedStorePackId.Type == StorePackType.MODULAR_BUNDLE)
    {
      int num = 100;
      if (weight > num)
        weight = num;
      xRotationAmount = maxXRotation * (float) weight / (float) num;
    }
    float translateAmount = 0.0f;
    if (GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
      translateAmount = translationAmount;
    this.m_parentStore.ShakeStore(xRotationAmount, this.m_packFlyShakeTime, delay, translateAmount);
  }

  public void StartAnimatingPacks() => this.m_animatingPacks = true;

  public void DoneAnimatingPacks() => this.m_animatingPacks = false;

  protected override void OnBundleChanged(
    NoGTAPPTransactionData goldBundle,
    Network.Bundle moneyBundle)
  {
    if (this.IsPackIdFirstPurchaseBundle(this.m_selectedStorePackId) && (Record) moneyBundle == (Record) null)
    {
      this.HandleMoneyPackBuyButtonClick(this.GetFirstValidBundleIndex(this.m_selectedStorePackId));
    }
    else
    {
      if (this.m_selectedStorePackId.Type == StorePackType.BOOSTER && GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId) && !StoreManager.Get().ShouldShowFeaturedDustJar(moneyBundle))
        return;
      bool flag1 = false;
      bool flag2 = false;
      if (goldBundle != null)
      {
        this.m_visiblePackCount = goldBundle.Quantity;
        this.m_visibleDustCount = 0;
        this.m_selectedBoosterIsPrePurchase = false;
      }
      else if ((Record) moneyBundle != (Record) null)
      {
        this.m_visiblePackCount = StoreManager.Get().PackQuantityInBundle(moneyBundle);
        int num1 = StoreManager.Get().DustQuantityInBundle(moneyBundle);
        int num2 = StoreManager.Get().DustBaseQuantityInBundle(moneyBundle);
        if (num2 > 0)
        {
          this.m_visibleDustCount = num2;
          this.m_visibleDustBonusCount = Math.Max(num1 - num2, 0);
        }
        else
        {
          this.m_visibleDustCount = num1;
          this.m_visibleDustBonusCount = 0;
        }
        flag1 = this.m_visibleDustCount > 0;
        flag2 = StoreManager.Get().ShouldShowFeaturedDustJar(moneyBundle);
        this.m_selectedBoosterIsPrePurchase = StoreManager.Get().IsProductPrePurchase(moneyBundle);
      }
      if (flag1 & flag2)
      {
        bool usePhoneUi = (bool) UniversalInputManager.UsePhoneUI;
        this.AnimatePacksFlying(this.m_visiblePackCount, true, showAsSingleStack: (!this.m_selectedBoosterIsPrePurchase), waitForLogo: usePhoneUi);
        this.StartCoroutine(this.ShowFeaturedDustJar(usePhoneUi));
      }
      else
      {
        this.AnimatePacksFlying(this.m_visiblePackCount);
        this.HideDust();
        this.HideHiddenBundleCard();
      }
    }
  }

  protected override void OnRefresh()
  {
    this.UpdatePackBuyButtons();
    this.UpdatePacksDescriptionFromSelectedStorePack();
    if (this.HasBundleSet() || this.IsPackIdInvalid(this.m_selectedStorePackId))
      return;
    this.UpdateSelectedBundle(true);
  }

  private void Awake()
  {
    this.m_packDisplay1 = this.m_packDisplay;
    this.m_packDisplay2 = UnityEngine.Object.Instantiate<GeneralStorePacksContentDisplay>(this.m_packDisplay);
    this.m_packDisplay2.transform.parent = this.m_packDisplay1.transform.parent;
    this.m_packDisplay2.transform.localPosition = this.m_packDisplay1.transform.localPosition;
    this.m_packDisplay2.transform.localScale = this.m_packDisplay1.transform.localScale;
    this.m_packDisplay2.transform.localRotation = this.m_packDisplay1.transform.localRotation;
    this.m_packDisplay2.gameObject.SetActive(false);
    this.m_logoMesh1 = this.m_logoMesh;
    this.m_logoMesh2 = UnityEngine.Object.Instantiate<MeshRenderer>(this.m_logoMesh);
    this.m_logoMesh2.transform.parent = this.m_logoMesh1.transform.parent;
    this.m_logoMesh2.transform.localPosition = this.m_logoMesh1.transform.localPosition;
    this.m_logoMesh2.transform.localScale = this.m_logoMesh1.transform.localScale;
    this.m_logoMesh2.transform.localRotation = this.m_logoMesh1.transform.localRotation;
    this.m_logoMesh2.gameObject.SetActive(false);
    this.m_logoGlowMesh1 = this.m_logoGlowMesh;
    this.m_logoGlowMesh2 = this.m_logoMesh2.transform.GetChild(0).GetComponentInChildren<MeshRenderer>();
    this.m_packDisplay1.SetParent(this);
    this.m_packDisplay2.SetParent(this);
    this.m_packBuyContainer.SetActive(false);
    if ((UnityEngine.Object) this.m_limitedTimeOfferText != (UnityEngine.Object) null)
      this.m_limitedTimeTextOrigScale = this.m_limitedTimeOfferText.transform.localScale;
    if ((UnityEngine.Object) this.m_packBuyBonusCallout != (UnityEngine.Object) null)
      this.m_packBuyBonusCallout.Init();
    if ((UnityEngine.Object) this.m_packBuyBonusText != (UnityEngine.Object) null)
      this.m_packBuyBonusText.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_ChinaInfoButton != (UnityEngine.Object) null)
      this.m_ChinaInfoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnKoreaInfoPressed));
    foreach (BoosterDbfRecord boosterDbfRecord in GameUtils.GetPackRecordsWithStorePrefab())
    {
      int id = boosterDbfRecord.ID;
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) boosterDbfRecord.StorePrefab, AssetLoadingOptions.IgnorePrefabPosition);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("Unable to load store pack def: {0}", (object) boosterDbfRecord.StorePrefab));
      }
      else
      {
        IStorePackDef component = (IStorePackDef) gameObject.GetComponent<StorePackDef>();
        if (component == null)
          Debug.LogError((object) string.Format("StorePackDef component not found: {0}", (object) boosterDbfRecord.StorePrefab));
        else
          this.m_storePackDefs.Add(new StorePackId()
          {
            Type = StorePackType.BOOSTER,
            Id = id
          }, component);
      }
    }
    foreach (ModularBundleDbfRecord record in GameDbf.ModularBundle.GetRecords())
      this.m_storePackDefs.Add(new StorePackId()
      {
        Type = StorePackType.MODULAR_BUNDLE,
        Id = record.ID
      }, (IStorePackDef) new ModularBundleStorePackDef(record));
    this.UpdateKoreaInfoButton();
  }

  private GameObject GetCurrentDisplayContainer() => this.GetCurrentDisplay().gameObject;

  private GameObject GetNextDisplayContainer() => (this.m_currentDisplay + 1) % 2 != 0 ? this.m_packDisplay2.gameObject : this.m_packDisplay1.gameObject;

  private GeneralStorePacksContentDisplay GetCurrentDisplay() => this.m_currentDisplay != 0 ? this.m_packDisplay2 : this.m_packDisplay1;

  private MeshRenderer GetCurrentLogo() => this.m_currentDisplay != 0 ? this.m_logoMesh2 : this.m_logoMesh1;

  private MeshRenderer GetCurrentGlowLogo() => this.m_currentDisplay != 0 ? this.m_logoGlowMesh2 : this.m_logoGlowMesh1;

  private void UpdateSelectedBundle(bool forceUpdate = false)
  {
    ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(this.m_selectedStorePackId);
    int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(this.m_selectedStorePackId, this.m_lastBundleIndex);
    NoGTAPPTransactionData gtappTransactionData = new NoGTAPPTransactionData()
    {
      Product = fromStorePackType,
      ProductData = dataFromStorePackId,
      Quantity = 1
    };
    if (StoreManager.Get().GetGoldCostNoGTAPP(gtappTransactionData, out long _))
    {
      this.SetCurrentGoldBundle(gtappTransactionData);
    }
    else
    {
      Network.Bundle lowestCostBundle = StoreManager.Get().GetLowestCostBundle(fromStorePackType, false, dataFromStorePackId);
      if (!((Record) lowestCostBundle != (Record) null))
        return;
      this.SetCurrentMoneyBundle(lowestCostBundle, forceUpdate);
    }
  }

  private void UpdatePacksDescriptionFromSelectedStorePack()
  {
    if (this.IsPackIdInvalid(this.m_selectedStorePackId))
    {
      this.m_parentStore.HideAccentTexture();
      this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_PACK"));
    }
    else if (this.m_selectedStorePackId.Type == StorePackType.BOOSTER)
    {
      this.UpdatePacksDescriptionForBooster();
    }
    else
    {
      if (this.m_selectedStorePackId.Type != StorePackType.MODULAR_BUNDLE)
        return;
      this.UpdatePacksDescriptionForModularBundle();
    }
  }

  private void UpdatePacksDescriptionForBooster()
  {
    BoosterDbfRecord record = GameDbf.Booster.GetRecord(this.m_selectedStorePackId.Id);
    string name = (string) record.Name;
    string packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_PACK");
    string packDescription = GameStrings.Format("GLUE_STORE_PRODUCT_DETAILS_PACK", (object) name);
    Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
    bool isPreorder = false;
    if ((Record) currentMoneyBundle != (Record) null)
    {
      isPreorder = StoreManager.Get().IsProductPrePurchase(currentMoneyBundle);
      bool flag1 = GameUtils.IsFirstPurchaseBundleBooster(this.m_selectedStorePackId);
      bool flag2 = StoreManager.Get().ShouldShowFeaturedDustJar(currentMoneyBundle);
      if (((isPreorder ? 0 : (!flag1 ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
      {
        packDescription = GameStrings.Format("GLUE_STORE_PRODUCT_DETAILS_DUST", (object) name);
        packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_DUST");
      }
      if (isPreorder && 10 == record.ID)
      {
        packDescription = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_TGT_PACK_PRESALE");
        packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_TGT_PACK_PRESALE");
      }
      if (isPreorder && 11 == record.ID)
      {
        packDescription = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_OG_PACK_PRESALE");
        packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_OG_PACK_PRESALE");
      }
      if (isPreorder && 20 == record.ID)
      {
        packDescription = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_GORO_PACK_PRESALE");
        packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_GORO_PACK_PRESALE");
      }
      if (GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
      {
        if (flag1)
        {
          packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_FIRST_PURCHASE_BUNDLE");
          packDescription = !flag2 ? GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_FIRST_PURCHASE_BUNDLE") : GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_FIRST_PURCHASE_BUNDLE_DUST");
        }
        else if (GameUtils.IsMammothBundleBooster(this.m_selectedStorePackId))
        {
          packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_MAMMOTH_BUNDLE");
          packDescription = !flag2 ? GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_MAMMOTH_BUNDLE") : GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_MAMMOTH_BUNDLE_DUST");
        }
      }
      if (isPreorder && 21 == record.ID)
      {
        packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_ICC_PACK_PRESALE");
        packDescription = !flag2 ? GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_ICC_PACK_PRESALE") : GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_ICC_CN_DUST_PRESALE");
      }
      if (isPreorder && 30 == record.ID)
      {
        packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_LOOT_PACK_PRESALE");
        packDescription = !flag2 ? GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_LOOT_PACK_PRESALE") : GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_LOOT_CN_DUST_PRESALE");
      }
      if (isPreorder && 31 == record.ID)
      {
        packDescriptionHeadline = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_GIL_PACK_PRESALE");
        packDescription = !flag2 ? GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_GIL_PACK_PRESALE") : GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_GIL_CN_DUST_PRESALE");
      }
    }
    string accentTextureName = "";
    IStorePackDef storePackDef = this.GetStorePackDef(this.m_selectedStorePackId);
    if (storePackDef != null)
      accentTextureName = storePackDef.GetAccentTextureName();
    this.UpdatePacksDescription(packDescriptionHeadline, packDescription, accentTextureName, isPreorder);
  }

  private void UpdatePacksDescriptionForModularBundle()
  {
    int id = this.m_selectedStorePackId.Id;
    List<ModularBundleLayoutDbfRecord> layoutsForBundle = StoreManager.Get().GetRegionNodeLayoutsForBundle(id);
    if (this.m_lastBundleIndex >= layoutsForBundle.Count)
    {
      Log.Store.PrintWarning(string.Format("Selected invalid layout at index={0}. Defaulting to layout at index=0.", (object) this.m_lastBundleIndex));
      this.m_lastBundleIndex = 0;
    }
    ModularBundleLayoutDbfRecord bundleLayoutDbfRecord = layoutsForBundle[this.m_lastBundleIndex];
    Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
    bool isPreorder = StoreManager.Get().IsProductPrePurchase(currentMoneyBundle);
    this.UpdatePacksDescription((string) bundleLayoutDbfRecord.DescriptionHeadline, (string) bundleLayoutDbfRecord.Description, bundleLayoutDbfRecord.AccentTexture, isPreorder);
  }

  private void UpdatePacksDescription(
    string packDescriptionHeadline,
    string packDescription,
    string accentTextureName,
    bool isPreorder)
  {
    string warning = string.Empty;
    if (StoreManager.Get().IsKoreanCustomer())
      warning = !isPreorder ? (!GameUtils.IsFirstPurchaseBundleBooster(this.m_selectedStorePackId) ? (!((Record) this.GetCurrentMoneyBundle() != (Record) null) || !AdventureUtils.IsAdventureBundle(this.GetCurrentMoneyBundle()) ? GameStrings.Get("GLUE_STORE_KOREAN_PRODUCT_DETAILS_EXPERT_PACK") : GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_ADVENTURE_BUNDLE")) : GameStrings.Get("GLUE_STORE_KOREAN_PRODUCT_DETAILS_FIRST_PURCHASE_BUNDLE")) : GameStrings.Get("GLUE_STORE_KOREAN_PRODUCT_DETAILS_PACKS_PREORDER");
    this.m_parentStore.SetDescription(packDescriptionHeadline, packDescription, warning);
    using (AssetHandle<Texture> texture = AssetLoader.Get().LoadAsset<Texture>((AssetReference) accentTextureName))
      this.m_parentStore.SetAccentTexture(texture);
  }

  private NoGTAPPTransactionData GetCurrentGTAPPTransactionData()
  {
    ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(this.m_selectedStorePackId);
    return new NoGTAPPTransactionData()
    {
      Product = fromStorePackType,
      ProductData = this.m_selectedStorePackId.Id,
      Quantity = this.m_currentGoldPackQuantity
    };
  }

  private void UpdatePackBuyButtons()
  {
    if (this.IsPackIdInvalid(this.m_selectedStorePackId))
      return;
    Network.Bundle hiddenLicenseBundle;
    if (StoreManager.Get().IsBoosterHiddenLicenseBundle(this.m_selectedStorePackId, out hiddenLicenseBundle) && this.m_selectedStorePackId.Type != StorePackType.MODULAR_BUNDLE)
      this.ShowHiddenLicenseBundleBuyButtons(hiddenLicenseBundle);
    else if (this.m_selectedStorePackId.Type == StorePackType.MODULAR_BUNDLE)
      this.ShowModularBundleBuyButtons();
    else
      this.ShowStandardBuyButtons();
  }

  private static Network.BundleItem GetPacksBundleItemFromBundle(Network.Bundle bundle) => (Record) bundle == (Record) null ? (Network.BundleItem) null : bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER));

  private void ShowStandardBuyButtons()
  {
    this.m_packBuyPreorderContainer.SetActive(false);
    this.m_packBuyContainer.SetActive(true);
    this.ClearButtonEventListeners();
    int num = 0;
    GeneralStorePackBuyButton goldButton = this.GetPackBuyButton(num);
    if ((UnityEngine.Object) goldButton == (UnityEngine.Object) null)
      goldButton = this.CreatePackBuyButton(num);
    goldButton.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e =>
    {
      if (!this.IsContentActive())
        return;
      this.HandleGoldPackBuyButtonClick();
      this.SelectPackBuyButton(goldButton);
    }));
    if (!(bool) UniversalInputManager.UsePhoneUI)
      goldButton.AddEventListener(UIEventType.DOUBLECLICK, (UIEvent.Handler) (e => this.HandleGoldPackBuyButtonDoubleClick(goldButton)));
    if (!this.IsPackIdInvalid(this.m_selectedStorePackId))
      goldButton.UpdateFromGTAPP(this.GetCurrentGTAPPTransactionData());
    Action action = (Action) (() =>
    {
      this.HandleGoldPackBuyButtonClick();
      this.SelectPackBuyButton(goldButton);
    });
    goldButton.Unselect();
    List<Network.Bundle> bundleList = this.GetPackBundles(true);
    if (bundleList.Count > this.m_maxPackBuyButtons - 1)
      bundleList = bundleList.GetRange(0, this.m_maxPackBuyButtons - 1);
    bool flag1 = false;
    int index1 = 0;
    int index2 = 0;
    for (int index3 = 0; index3 < bundleList.Count; ++index3)
    {
      ++num;
      int bundleIndexCopy = index3;
      Network.Bundle bundle = bundleList[index3];
      Network.BundleItem bundleItem = bundle.Items.Find((Predicate<Network.BundleItem>) (obj => obj.ItemType == ProductType.PRODUCT_TYPE_BOOSTER));
      if ((Record) bundleItem == (Record) null)
      {
        Debug.LogWarning((object) string.Format("GeneralStorePacksContent.UpdatePackBuyButtons() bundle {0} has no packs bundle item!", (object) bundle.PMTProductID));
      }
      else
      {
        GeneralStorePackBuyButton moneyButton = this.GetPackBuyButton(num);
        if ((UnityEngine.Object) moneyButton == (UnityEngine.Object) null)
          moneyButton = this.CreatePackBuyButton(num);
        moneyButton.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e =>
        {
          if (!this.IsContentActive())
            return;
          this.HandleMoneyPackBuyButtonClick(bundleIndexCopy);
          this.SelectPackBuyButton(moneyButton);
        }));
        string packBuyButtonText = this.GetPackBuyButtonText(bundle, bundleItem);
        if (bundleItem.BaseQuantity > 0)
        {
          if (!flag1)
          {
            flag1 = true;
            index1 = num;
          }
          index2 = num;
        }
        moneyButton.SetMoneyValue(bundle, bundleItem, packBuyButtonText);
        moneyButton.gameObject.SetActive(true);
        if (moneyButton.IsSelected() || (Record) this.GetCurrentMoneyBundle() == (Record) bundle)
          action = (Action) (() =>
          {
            this.HandleMoneyPackBuyButtonClick(bundleIndexCopy);
            this.SelectPackBuyButton(moneyButton);
          });
        moneyButton.Unselect();
      }
    }
    bool flag2 = StoreManager.Get().CanBuyStorePackWithGold(this.m_selectedStorePackId);
    goldButton.gameObject.SetActive(flag2);
    for (int index4 = num + 1; index4 < this.m_packBuyButtons.Count; ++index4)
    {
      GeneralStorePackBuyButton packBuyButton = this.m_packBuyButtons[index4];
      if ((UnityEngine.Object) packBuyButton != (UnityEngine.Object) null)
        packBuyButton.gameObject.SetActive(false);
    }
    int numSectionsNeeded = num + 1;
    this.UpdateToggleableSections(this.m_toggleableButtonFrames, numSectionsNeeded);
    bool flag3 = numSectionsNeeded >= this.m_toggleableButtonFrames.Count;
    foreach (GeneralStorePacksContent.MultiSliceEndCaps buyBarEndCap in this.m_buyBarEndCaps)
    {
      buyBarEndCap.m_FullBar.SetActive(flag3);
      buyBarEndCap.m_SmallerBar.SetActive(!flag3);
    }
    if ((UnityEngine.Object) this.m_packBuyFrameContainer != (UnityEngine.Object) null)
      this.m_packBuyFrameContainer.UpdateSlices();
    this.m_packBuyButtonContainer.UpdateSlices();
    if (action != null)
      action();
    if (!((UnityEngine.Object) this.m_packBuyBonusCallout != (UnityEngine.Object) null))
      return;
    if (this.m_packBuyBonusCalloutOnlyOncePerSession && this.m_packBuyBonusCalloutSeenForPackId.Contains(this.m_selectedStorePackId))
      flag1 = false;
    if (flag1 || this.m_packBuyBonusCalloutDebugForceDisplay > 0)
    {
      this.HideBonusPacksText();
      int numButtons;
      GeneralStorePackBuyButton packBuyButton1;
      GeneralStorePackBuyButton packBuyButton2;
      if (this.m_packBuyBonusCalloutDebugForceDisplay > 0)
      {
        numButtons = this.m_packBuyBonusCalloutDebugForceDisplay;
        packBuyButton1 = this.GetPackBuyButton(Math.Max(num - (numButtons - 1), 0));
        packBuyButton2 = this.GetPackBuyButton(num);
      }
      else
      {
        numButtons = 1 + Math.Max(index2 - index1, 0);
        packBuyButton1 = this.GetPackBuyButton(index1);
        packBuyButton2 = this.GetPackBuyButton(index2);
      }
      if (this.m_bonusPacksCalloutCoroutine != null)
        this.StopCoroutine(this.m_bonusPacksCalloutCoroutine);
      this.m_bonusPacksCalloutCoroutine = this.StartCoroutine(this.DelayedShowBonusPacksCallout(1f, packBuyButton1, packBuyButton2, numButtons));
    }
    else
      this.m_packBuyBonusCallout.HideCallout();
  }

  private IEnumerator DelayedShowBonusPacksCallout(
    float delay,
    GeneralStorePackBuyButton firstButton,
    GeneralStorePackBuyButton lastButton,
    int numButtons)
  {
    yield return (object) new WaitForSeconds(delay);
    this.m_packBuyBonusCallout.ShowCallout(firstButton, lastButton, numButtons);
  }

  private void UpdateBonusPacksUI(Network.Bundle bundle)
  {
    int numBonusPacks = 0;
    if ((Record) bundle != (Record) null)
    {
      Network.BundleItem bundleItemFromBundle = GeneralStorePacksContent.GetPacksBundleItemFromBundle(bundle);
      if ((Record) bundleItemFromBundle != (Record) null && bundleItemFromBundle.BaseQuantity > 0)
        numBonusPacks = Math.Max(bundleItemFromBundle.Quantity - bundleItemFromBundle.BaseQuantity, 0);
    }
    if (numBonusPacks > 0)
    {
      if ((UnityEngine.Object) this.m_packBuyBonusCallout != (UnityEngine.Object) null)
      {
        if (this.m_packBuyBonusCallout.IsShown())
          this.m_packBuyBonusCalloutSeenForPackId.Add(this.m_selectedStorePackId);
        this.m_packBuyBonusCallout.HideCallout();
      }
      this.ShowBonusPacksText(numBonusPacks, StoreManager.Get().ShouldShowFeaturedDustJar(bundle));
    }
    else
      this.HideBonusPacksText();
  }

  private void ShowBonusPacksText(int numBonusPacks, bool isShowingDustJar)
  {
    this.m_packBuyBonusText.gameObject.SetActive(true);
    if (isShowingDustJar)
      this.m_packBuyBonusText.Text = GameStrings.Format("GLUE_CHINA_STORE_DUST_PLUS_BONUS_DETAILED", (object) numBonusPacks, (object) numBonusPacks);
    else
      this.m_packBuyBonusText.Text = GameStrings.Format("GLUE_STORE_BONUS_PACKS", (object) numBonusPacks);
  }

  private void HideBonusPacksText() => this.m_packBuyBonusText.gameObject.SetActive(false);

  private void UpdateToggleableSections(
    List<GeneralStorePacksContent.ToggleableButtonFrame> sections,
    int numSectionsNeeded)
  {
    int num = numSectionsNeeded - 1;
    for (int index = 0; index < sections.Count; ++index)
    {
      GeneralStorePacksContent.ToggleableButtonFrame section = sections[index];
      bool flag = index <= num;
      if ((UnityEngine.Object) section.m_IBar != (UnityEngine.Object) null)
        section.m_IBar.SetActive(flag);
      section.m_Middle.SetActive(flag);
    }
  }

  private void ShowModularBundleBuyButtons()
  {
    Action action = (Action) null;
    Action<GeneralStorePackBuyButton> selectButtonFunc = (Action<GeneralStorePackBuyButton>) null;
    ModularBundleDbfRecord record = GameDbf.ModularBundle.GetRecord(this.m_selectedStorePackId.Id);
    GeneralStorePacksContent.ModularBundleLayoutButtonSize layoutButtonSize = EnumUtils.SafeParse<GeneralStorePacksContent.ModularBundleLayoutButtonSize>(record.LayoutButtonSize, ignoreCase: true);
    bool useLargeButtons = layoutButtonSize == GeneralStorePacksContent.ModularBundleLayoutButtonSize.Large;
    Func<int, GeneralStorePackBuyButton> func1;
    Func<int, GeneralStorePackBuyButton> func2;
    MultiSliceElement multiSliceElement1;
    MultiSliceElement multiSliceElement2;
    List<GeneralStorePacksContent.ToggleableButtonFrame> sections;
    List<GeneralStorePackBuyButton> storePackBuyButtonList;
    if (useLargeButtons)
    {
      this.m_packBuyContainer.SetActive(false);
      this.m_packBuyPreorderContainer.SetActive(true);
      func1 = new Func<int, GeneralStorePackBuyButton>(this.GetPackPreorderBuyButton);
      func2 = new Func<int, GeneralStorePackBuyButton>(this.CreatePackPreorderBuyButton);
      selectButtonFunc = new Action<GeneralStorePackBuyButton>(this.SelectPackBuyPreorderButton);
      multiSliceElement1 = this.m_packBuyPreorderFrameContainer;
      multiSliceElement2 = this.m_packBuyPreorderButtonContainer;
      sections = this.m_toggleablePreorderButtonFrames;
      storePackBuyButtonList = this.m_packPreorderBuyButtons;
    }
    else
    {
      this.m_packBuyContainer.SetActive(true);
      this.m_packBuyPreorderContainer.SetActive(false);
      func1 = new Func<int, GeneralStorePackBuyButton>(this.GetPackBuyButton);
      func2 = new Func<int, GeneralStorePackBuyButton>(this.CreatePackBuyButton);
      selectButtonFunc = new Action<GeneralStorePackBuyButton>(this.SelectPackBuyButton);
      multiSliceElement1 = this.m_packBuyFrameContainer;
      multiSliceElement2 = this.m_packBuyButtonContainer;
      sections = this.m_toggleableButtonFrames;
      storePackBuyButtonList = this.m_packBuyButtons;
    }
    bool isDev = !HearthstoneApplication.IsPublic() && Vars.Key("ModularBundle.ShowAll").GetBool(false);
    ModularBundleLayoutDbfRecord[] array = StoreManager.Get().GetRegionNodeLayoutsForBundle(record.ID).ToArray();
    if (array.Length < 2 || layoutButtonSize == GeneralStorePacksContent.ModularBundleLayoutButtonSize.None)
    {
      this.m_packBuyContainer.SetActive(false);
      this.m_packBuyPreorderContainer.SetActive(false);
      this.HandleMoneyBuyModularBundleButtonClick(0, isDev);
    }
    else
    {
      this.ClearButtonEventListeners();
      int numSectionsNeeded = 0;
      for (int selectedIndex = 0; selectedIndex < array.Length; ++selectedIndex)
      {
        GeneralStorePackBuyButton moneyButton = func1(numSectionsNeeded);
        int bundleIndexCopy = selectedIndex;
        if ((UnityEngine.Object) moneyButton == (UnityEngine.Object) null)
          moneyButton = func2(selectedIndex);
        moneyButton.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e =>
        {
          if (!this.IsContentActive())
            return;
          this.HandleMoneyBuyModularBundleButtonClick(bundleIndexCopy, isDev);
          selectButtonFunc(moneyButton);
        }));
        if (selectedIndex == 0)
          action = (Action) (() => selectButtonFunc(moneyButton));
        int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(this.m_selectedStorePackId, selectedIndex);
        Network.Bundle bundle = StoreManager.Get().EnumerateBundlesForProductType(ProductType.PRODUCT_TYPE_HIDDEN_LICENSE, true, dataFromStorePackId).FirstOrDefault<Network.Bundle>();
        if (isDev || !((Record) bundle == (Record) null))
        {
          Network.BundleItem bundleItemFromBundle = GeneralStorePacksContent.GetPacksBundleItemFromBundle(bundle);
          string packBuyButtonText = this.GetPackBuyButtonText(bundle, bundleItemFromBundle, useLargeButtons);
          moneyButton.SetMoneyValue(bundle, bundleItemFromBundle, packBuyButtonText);
          moneyButton.gameObject.SetActive(true);
          if (moneyButton.IsSelected() || (Record) this.GetCurrentMoneyBundle() == (Record) bundle)
            action = (Action) (() =>
            {
              this.HandleMoneyBuyModularBundleButtonClick(bundleIndexCopy, isDev);
              selectButtonFunc(moneyButton);
            });
          moneyButton.Unselect();
          ++numSectionsNeeded;
        }
      }
      if (numSectionsNeeded == 0)
      {
        this.m_packBuyPreorderContainer.SetActive(false);
        this.m_packBuyContainer.SetActive(false);
      }
      for (int index = numSectionsNeeded; index < storePackBuyButtonList.Count; ++index)
      {
        GeneralStorePackBuyButton storePackBuyButton = storePackBuyButtonList[index];
        if ((UnityEngine.Object) storePackBuyButton != (UnityEngine.Object) null)
          storePackBuyButton.gameObject.SetActive(false);
      }
      this.UpdateToggleableSections(sections, numSectionsNeeded);
      if ((UnityEngine.Object) multiSliceElement1 != (UnityEngine.Object) null)
        multiSliceElement1.UpdateSlices();
      multiSliceElement2.UpdateSlices();
      if (action != null)
        action();
      this.UpdatePacksDescriptionFromSelectedStorePack();
    }
  }

  public string GetPackBuyButtonText(
    Network.Bundle bundle,
    Network.BundleItem bundleItem,
    bool useLargeButtons = false)
  {
    if ((Record) bundle == (Record) null || (Record) bundleItem == (Record) null)
      return string.Empty;
    bool flag = StoreManager.Get().ShouldShowFeaturedDustJar(bundle);
    return useLargeButtons ? (flag ? GameStrings.Format("GLUE_STORE_QUANTITY_DUST_BUNDLE", (object) bundleItem.Quantity) : GameStrings.Format("GLUE_STORE_QUANTITY_PACK_BUNDLE", (object) bundleItem.Quantity)) : (flag ? StoreManager.Get().GetProductQuantityText(ProductType.PRODUCT_TYPE_CURRENCY, bundleItem.ProductData, bundleItem.Quantity, bundleItem.BaseQuantity) : StoreManager.Get().GetProductQuantityText(bundleItem.ItemType, bundleItem.ProductData, bundleItem.Quantity, bundleItem.BaseQuantity));
  }

  private void ShowHiddenLicenseBundleBuyButtons(Network.Bundle bundle)
  {
    this.m_packBuyContainer.SetActive(false);
    this.m_packBuyPreorderContainer.SetActive(false);
    this.HandleMoneyPackBuyButtonClick(this.GetFirstValidBundleIndex(this.m_selectedStorePackId));
  }

  private void UpdatePacksTypeMusic()
  {
    if (this.m_parentStore.GetMode() == GeneralStoreMode.NONE)
      return;
    IStorePackDef storePackDef = this.GetStorePackDef(this.m_selectedStorePackId);
    if (storePackDef != null && storePackDef.GetPlaylist() != MusicPlaylistType.Invalid && MusicManager.Get().StartPlaylist(storePackDef.GetPlaylist()))
      return;
    this.m_parentStore.ResumePreviousMusicPlaylist();
  }

  private void HandleGoldPackBuyButtonClick()
  {
    ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(this.m_selectedStorePackId);
    this.SetCurrentGoldBundle(new NoGTAPPTransactionData()
    {
      Product = fromStorePackType,
      ProductData = this.m_selectedStorePackId.Id,
      Quantity = this.m_currentGoldPackQuantity
    });
    this.UpdatePacksDescriptionFromSelectedStorePack();
  }

  private void HandleGoldPackBuyButtonDoubleClick(GeneralStorePackBuyButton button)
  {
    if (this.m_selectedStorePackId.Type == StorePackType.BOOSTER)
      TelemetryManager.Client().SendChangePackQuantity(this.m_selectedStorePackId.Id);
    this.m_parentStore.BlockInterface(true);
    this.m_quantityPrompt.Show(GeneralStorePacksContent.MAX_QUANTITY_BOUGHT_WITH_GOLD, (StoreQuantityPrompt.OkayListener) (quantity =>
    {
      this.m_parentStore.BlockInterface(false);
      this.m_currentGoldPackQuantity = quantity;
      NoGTAPPTransactionData gtappTransactionData = this.GetCurrentGTAPPTransactionData();
      button.UpdateFromGTAPP(gtappTransactionData);
      this.SetCurrentGoldBundle(gtappTransactionData);
    }), (StoreQuantityPrompt.CancelListener) (() => this.m_parentStore.BlockInterface(false)));
  }

  private void HandleMoneyPackBuyButtonClick(int bundleIndex)
  {
    Network.Bundle bundle = (Network.Bundle) null;
    List<Network.Bundle> packBundles = this.GetPackBundles(true);
    if (packBundles != null && packBundles.Count > 0)
    {
      if (bundleIndex >= packBundles.Count)
        bundleIndex = 0;
      bundle = packBundles[bundleIndex];
    }
    this.SetCurrentMoneyBundle(bundle, true);
    this.m_lastBundleIndex = bundleIndex;
    this.UpdatePacksDescriptionFromSelectedStorePack();
    this.UpdateBonusPacksUI(bundle);
  }

  private void HandleMoneyBuyModularBundleButtonClick(int bundleIndex, bool isDev = false)
  {
    List<ModularBundleLayoutDbfRecord> records = GameDbf.ModularBundleLayout.GetRecords((Predicate<ModularBundleLayoutDbfRecord>) (r => r.ModularBundleId == this.m_selectedStorePackId.Id));
    if (bundleIndex >= records.Count)
      bundleIndex = 0;
    int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(this.m_selectedStorePackId, bundleIndex);
    Network.Bundle bundle = StoreManager.Get().EnumerateBundlesForProductType(ProductType.PRODUCT_TYPE_HIDDEN_LICENSE, true, dataFromStorePackId).FirstOrDefault<Network.Bundle>();
    if (!isDev && (Record) bundle == (Record) null)
      return;
    this.m_lastBundleIndex = bundleIndex;
    this.SetCurrentMoneyBundle(bundle, true);
    this.UpdatePacksDescriptionFromSelectedStorePack();
  }

  private void SelectPackBuyButton(GeneralStorePackBuyButton packBuyBtn)
  {
    foreach (GeneralStorePackBuyButton packBuyButton in this.m_packBuyButtons)
      packBuyButton.Unselect();
    packBuyBtn.Select();
  }

  private void SelectPackBuyPreorderButton(GeneralStorePackBuyButton packBuyBtn)
  {
    foreach (GeneralStorePackBuyButton preorderBuyButton in this.m_packPreorderBuyButtons)
      preorderBuyButton.Unselect();
    packBuyBtn.Select();
  }

  private GeneralStorePackBuyButton GetPackBuyButton(int index) => index < this.m_packBuyButtons.Count ? this.m_packBuyButtons[index] : (GeneralStorePackBuyButton) null;

  private GeneralStorePackBuyButton CreatePackBuyButton(int buttonIndex)
  {
    if (buttonIndex >= this.m_packBuyButtons.Count)
    {
      int num = buttonIndex - this.m_packBuyButtons.Count + 1;
      for (int index = 0; index < num; ++index)
      {
        GeneralStorePackBuyButton storePackBuyButton = (GeneralStorePackBuyButton) GameUtils.Instantiate((Component) this.m_packBuyButtonPrefab, this.m_packBuyButtonContainer.gameObject, true);
        LayerUtils.SetLayer(storePackBuyButton.gameObject, this.m_packBuyButtonContainer.gameObject.layer);
        storePackBuyButton.transform.localRotation = Quaternion.identity;
        storePackBuyButton.transform.localScale = Vector3.one;
        this.m_packBuyButtonContainer.AddSlice(storePackBuyButton.gameObject);
        this.m_packBuyButtons.Add(storePackBuyButton);
      }
      this.m_packBuyButtonContainer.UpdateSlices();
    }
    return this.m_packBuyButtons[buttonIndex];
  }

  private GeneralStorePackBuyButton GetPackPreorderBuyButton(int index) => index < this.m_packPreorderBuyButtons.Count ? this.m_packPreorderBuyButtons[index] : (GeneralStorePackBuyButton) null;

  private GeneralStorePackBuyButton CreatePackPreorderBuyButton(
    int buttonIndex)
  {
    if (buttonIndex >= this.m_packPreorderBuyButtons.Count)
    {
      int num = buttonIndex - this.m_packPreorderBuyButtons.Count + 1;
      for (int index = 0; index < num; ++index)
      {
        GeneralStorePackBuyButton storePackBuyButton = (GeneralStorePackBuyButton) GameUtils.Instantiate((Component) this.m_packBuyPreorderButtonPrefab, this.m_packBuyPreorderButtonContainer.gameObject, true);
        LayerUtils.SetLayer(storePackBuyButton.gameObject, this.m_packBuyPreorderButtonContainer.gameObject.layer);
        storePackBuyButton.transform.localRotation = Quaternion.identity;
        storePackBuyButton.transform.localScale = Vector3.one;
        this.m_packBuyPreorderButtonContainer.AddSlice(storePackBuyButton.gameObject);
        this.m_packPreorderBuyButtons.Add(storePackBuyButton);
      }
      this.m_packBuyPreorderButtonContainer.UpdateSlices();
    }
    return this.m_packPreorderBuyButtons[buttonIndex];
  }

  private List<Network.Bundle> GetPackBundles(bool sortByPackQuantity)
  {
    ProductType selectedProductType = StorePackId.GetProductTypeFromStorePackType(this.m_selectedStorePackId);
    List<Network.Bundle> first = new List<Network.Bundle>();
    int countFromStorePackId = GameUtils.GetProductDataCountFromStorePackId(this.m_selectedStorePackId);
    for (int selectedIndex = 0; selectedIndex < countFromStorePackId; ++selectedIndex)
    {
      List<Network.Bundle> bundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(selectedProductType, true, GameUtils.GetProductDataFromStorePackId(this.m_selectedStorePackId, selectedIndex));
      first = first.Concat<Network.Bundle>((IEnumerable<Network.Bundle>) bundlesForProduct).ToList<Network.Bundle>();
    }
    if (!GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
      first.RemoveAll((Predicate<Network.Bundle>) (obj => (Record) obj.Items.Find((Predicate<Network.BundleItem>) (item => item.ItemType == ProductType.PRODUCT_TYPE_HIDDEN_LICENSE)) != (Record) null));
    if (sortByPackQuantity)
      first.Sort((Comparison<Network.Bundle>) ((left, right) => ((Record) left == (Record) null ? 0 : left.Items.Where<Network.BundleItem>((Func<Network.BundleItem, bool>) (i => i.ItemType == selectedProductType)).Max<Network.BundleItem>((Func<Network.BundleItem, int>) (i => i.Quantity))) - ((Record) right == (Record) null ? 0 : right.Items.Where<Network.BundleItem>((Func<Network.BundleItem, bool>) (i => i.ItemType == selectedProductType)).Max<Network.BundleItem>((Func<Network.BundleItem, int>) (i => i.Quantity)))));
    return first;
  }

  private void AnimateLogo(bool animateLogo, bool isFirstStoreOpen = false)
  {
    if (!this.m_hasLogo || !this.gameObject.activeInHierarchy || this.IsPackIdInvalid(this.m_selectedStorePackId))
      return;
    MeshRenderer currentLogo = this.GetCurrentLogo();
    switch (this.m_logoAnimation)
    {
      case GeneralStorePacksContent.LogoAnimation.Slam:
        if (animateLogo)
        {
          this.m_logoAnimCoroutine = this.StartCoroutine(this.AnimateSlamLogo(currentLogo));
          break;
        }
        if (this.m_animatingLogo || isFirstStoreOpen)
          break;
        currentLogo.transform.localPosition = this.m_logoAnimationEndBone.transform.localPosition;
        currentLogo.gameObject.SetActive(true);
        break;
      case GeneralStorePacksContent.LogoAnimation.Fade:
        if (animateLogo)
        {
          this.m_logoAnimCoroutine = this.StartCoroutine(this.AnimateFadeLogo(currentLogo));
          break;
        }
        if (this.m_animatingLogo)
          break;
        currentLogo.gameObject.SetActive(false);
        break;
    }
  }

  private void AnimatePacksFlying(
    int numVisiblePacks,
    bool forceImmediate = false,
    float delay = 0.0f,
    bool showAsSingleStack = false,
    bool waitForLogo = true)
  {
    if (!this.gameObject.activeInHierarchy)
      return;
    GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
    if (this.m_packAnimCoroutine != null)
      this.StopCoroutine(this.m_packAnimCoroutine);
    if (this.m_limitedTimeOfferAnimCoroutine != null)
      this.StopCoroutine(this.m_limitedTimeOfferAnimCoroutine);
    if (GameUtils.IsHiddenLicenseBundleBooster(this.m_selectedStorePackId))
    {
      if (this.m_selectedStorePackId.Type == StorePackType.MODULAR_BUNDLE)
        this.m_packAnimCoroutine = this.StartCoroutine(this.AnimateModularBundle(currentDisplay, forceImmediate, delay, waitForLogo));
      else if (showAsSingleStack && GameUtils.IsFirstPurchaseBundleBooster(this.m_selectedStorePackId))
      {
        this.m_packAnimCoroutine = this.StartCoroutine(this.AnimatePacks(currentDisplay, numVisiblePacks, forceImmediate, showAsSingleStack, waitForLogo));
      }
      else
      {
        if (StoreManager.IsHiddenLicenseBundleOwned(GameUtils.GetProductDataFromStorePackId(this.m_selectedStorePackId, this.m_lastBundleIndex)) && GameUtils.IsFirstPurchaseBundleBooster(this.m_selectedStorePackId))
          forceImmediate = true;
        this.m_packAnimCoroutine = this.StartCoroutine(this.AnimateBundleBox(currentDisplay, delay, forceImmediate));
      }
    }
    else
      this.m_packAnimCoroutine = this.StartCoroutine(this.AnimatePacks(currentDisplay, numVisiblePacks, forceImmediate, showAsSingleStack, waitForLogo));
    this.m_limitedTimeOfferAnimCoroutine = this.StartCoroutine(this.AnimateLimitedTimeOfferUI(currentDisplay, waitForLogo));
  }

  private IEnumerator AnimateFadeLogo(MeshRenderer logo)
  {
    if (!((UnityEngine.Object) logo == (UnityEngine.Object) null) && this.m_hasLogo && logo.transform.parent.gameObject.activeInHierarchy)
    {
      while (this.m_animatingLogo || this.m_loadingLogoTexture || this.m_loadingLogoGlowTexture)
        yield return (object) null;
      logo.gameObject.SetActive(true);
      this.m_animatingLogo = true;
      PlayMakerFSM logoFSM = logo.GetComponent<PlayMakerFSM>();
      logo.transform.localPosition = this.m_logoAnimationStartBone.transform.localPosition;
      iTween.MoveFrom(logo.gameObject, iTween.Hash((object) "position", (object) (logo.transform.localPosition - this.m_logoAppearOffset), (object) "easetype", (object) iTween.EaseType.easeInQuint, (object) "time", (object) this.m_logoIntroTime, (object) "islocal", (object) true));
      AnimationUtil.FadeTexture(logo, 0.0f, 1f, this.m_logoIntroTime, 0.0f);
      if ((UnityEngine.Object) logoFSM != (UnityEngine.Object) null)
        logoFSM.SendEvent("FadeIn");
      if ((double) this.m_logoHoldTime > 0.0)
        yield return (object) new WaitForSeconds(this.m_logoHoldTime);
      AnimationUtil.FadeTexture(logo, 1f, 0.0f, this.m_logoOutroTime, 0.0f);
      if ((UnityEngine.Object) logoFSM != (UnityEngine.Object) null)
        logoFSM.SendEvent("FadeOut");
      yield return (object) new WaitForSeconds(this.m_logoOutroTime);
      this.m_animatingLogo = false;
    }
  }

  private IEnumerator AnimateSlamLogo(MeshRenderer logo)
  {
    GeneralStorePacksContent storePacksContent = this;
    if (!((UnityEngine.Object) logo == (UnityEngine.Object) null) && storePacksContent.m_hasLogo && logo.transform.parent.gameObject.activeInHierarchy)
    {
      while (storePacksContent.m_animatingLogo || storePacksContent.m_loadingLogoTexture || storePacksContent.m_loadingLogoGlowTexture)
        yield return (object) null;
      logo.gameObject.SetActive(true);
      storePacksContent.m_animatingLogo = true;
      PlayMakerFSM logoFSM = logo.GetComponent<PlayMakerFSM>();
      logo.transform.localPosition = storePacksContent.m_logoAnimationStartBone.transform.localPosition;
      iTween.MoveFrom(logo.gameObject, iTween.Hash((object) "position", (object) (logo.transform.localPosition - storePacksContent.m_logoAppearOffset), (object) "easetype", (object) iTween.EaseType.easeInQuint, (object) "time", (object) storePacksContent.m_logoIntroTime, (object) "islocal", (object) true));
      AnimationUtil.FadeTexture(logo, 0.0f, 1f, storePacksContent.m_logoIntroTime, 0.0f);
      if ((UnityEngine.Object) logoFSM != (UnityEngine.Object) null)
        logoFSM.SendEvent("FadeIn");
      yield return (object) new WaitForSeconds(storePacksContent.m_logoIntroTime);
      if ((double) storePacksContent.m_logoHoldTime > 0.0)
        yield return (object) new WaitForSeconds(storePacksContent.m_logoHoldTime);
      iTween.MoveTo(logo.gameObject, iTween.Hash((object) "position", (object) storePacksContent.m_logoAnimationEndBone.transform.localPosition, (object) "easetype", (object) iTween.EaseType.easeInQuint, (object) "time", (object) storePacksContent.m_logoOutroTime, (object) "islocal", (object) true));
      yield return (object) new WaitForSeconds(storePacksContent.m_logoOutroTime);
      if ((UnityEngine.Object) logoFSM != (UnityEngine.Object) null)
        logoFSM.SendEvent("PostSlamIn");
      storePacksContent.gameObject.transform.localPosition = storePacksContent.m_savedLocalPosition;
      iTween.Stop(storePacksContent.gameObject);
      iTween.PunchScale(storePacksContent.gameObject, storePacksContent.m_punchAmount, storePacksContent.m_logoDisplayPunchTime);
      yield return (object) new WaitForSeconds(storePacksContent.m_logoDisplayPunchTime * 0.5f);
      storePacksContent.m_animatingLogo = false;
    }
  }

  private IEnumerator AnimatePacks(
    GeneralStorePacksContentDisplay display,
    int numVisiblePacks,
    bool forceImmediate,
    bool showAsSingleStack,
    bool waitForLogo)
  {
    while (((this.m_animatingLogo || this.m_loadingLogoTexture ? 1 : (this.m_loadingLogoGlowTexture ? 1 : 0)) & (waitForLogo ? 1 : 0)) != 0)
      yield return (object) null;
    this.StartAnimatingPacks();
    int num = display.ShowPacks(numVisiblePacks, this.m_packFlyInAnimTime, this.m_packFlyOutAnimTime, this.m_packFlyInDelay, this.m_packFlyOutDelay, forceImmediate, showAsSingleStack);
    if (!forceImmediate && num != 0)
    {
      int numPacks = Mathf.Abs(num);
      float maxXRotation = numPacks > 0 ? this.m_maxPackFlyInXShake : this.m_maxPackFlyOutXShake;
      float seconds = numPacks > 0 ? this.m_packFlyInDelay : this.m_packFlyOutDelay;
      this.ShakeStore(numPacks, maxXRotation, (float) numPacks * seconds * this.m_shakeObjectDelayMultiplier);
      yield return (object) new WaitForSeconds(seconds);
    }
    this.DoneAnimatingPacks();
  }

  public void AnimateModularBundleAfterPurchase(StorePackId storePack)
  {
    List<ModularBundleLayoutDbfRecord> layoutsForBundle = StoreManager.Get().GetRegionNodeLayoutsForBundle(storePack.Id);
    if (this.m_lastBundleIndex >= layoutsForBundle.Count)
    {
      Log.Store.PrintWarning(string.Format("Selected invalid layout at index={0}. Skipping post-purchase animation.", (object) this.m_lastBundleIndex));
    }
    else
    {
      if (!layoutsForBundle[this.m_lastBundleIndex].AnimateAfterPurchase)
        return;
      this.StartCoroutine(this.AnimateModularBundle(this.GetCurrentDisplay(), false, 0.0f, true));
    }
  }

  private IEnumerator AnimateModularBundle(
    GeneralStorePacksContentDisplay display,
    bool forceImmediate,
    float delayAnim,
    bool waitForLogo)
  {
    while (((this.m_animatingLogo || this.m_loadingLogoTexture || this.m_loadingLogoGlowTexture ? 1 : (this.m_animatingPacks ? 1 : 0)) & (waitForLogo ? 1 : 0)) != 0)
      yield return (object) null;
    this.StartAnimatingPacks();
    ModularBundleDbfRecord record = GameDbf.ModularBundle.GetRecord(this.m_selectedStorePackId.Id);
    float storeShakeDelay = 0.0f;
    int storeShakeAmount = 0;
    ModularBundleNodeLayout previousBundle = (ModularBundleNodeLayout) null;
    int nodesAnimatingIn = display.ShowModularBundle(record, forceImmediate, out storeShakeDelay, out storeShakeAmount, out previousBundle, this.m_lastBundleIndex);
    if (!forceImmediate && nodesAnimatingIn != 0)
    {
      while ((UnityEngine.Object) previousBundle != (UnityEngine.Object) null && previousBundle.IsAnimating)
        yield return (object) null;
      this.ShakeStore(nodesAnimatingIn, this.m_maxPackFlyInXShake, storeShakeDelay, weight: storeShakeAmount);
      yield return (object) new WaitForSeconds(delayAnim + 1f);
    }
  }

  private IEnumerator ShowFeaturedDustJar(bool waitForLogo = false)
  {
    GeneralStorePacksContent storePacksContent = this;
    while (((storePacksContent.m_animatingLogo || storePacksContent.m_loadingLogoTexture ? 1 : (storePacksContent.m_loadingLogoGlowTexture ? 1 : 0)) & (waitForLogo ? 1 : 0)) != 0)
      yield return (object) null;
    if (storePacksContent.m_visibleDustCount == 0)
    {
      storePacksContent.HideDust();
    }
    else
    {
      GeneralStorePacksContentDisplay currentDisplay = storePacksContent.GetCurrentDisplay();
      if ((UnityEngine.Object) currentDisplay != (UnityEngine.Object) null)
      {
        storePacksContent.StartCoroutine(currentDisplay.ShowDustJar(storePacksContent.m_visibleDustCount, storePacksContent.m_visibleDustBonusCount, storePacksContent.m_selectedBoosterIsPrePurchase, storePacksContent.m_selectedStorePackId));
        storePacksContent.ShowGiftDescription(storePacksContent.m_visibleDustCount, storePacksContent.m_visibleDustBonusCount, storePacksContent.m_selectedBoosterIsPrePurchase, storePacksContent.m_selectedStorePackId);
      }
    }
  }

  private void HideDust()
  {
    GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
    if (!((UnityEngine.Object) currentDisplay != (UnityEngine.Object) null))
      return;
    currentDisplay.HideDustJar();
    this.HideGiftDescription();
  }

  private void ShowGiftDescription(
    int amount,
    int bonusAmount,
    bool prePurchase,
    StorePackId selectedStorePackId)
  {
    GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
    if (!((UnityEngine.Object) currentDisplay != (UnityEngine.Object) null))
      return;
    currentDisplay.ShowGiftDescription(amount, bonusAmount, prePurchase, selectedStorePackId);
  }

  private void HideGiftDescription()
  {
    GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
    if (!((UnityEngine.Object) currentDisplay != (UnityEngine.Object) null))
      return;
    currentDisplay.HideGiftDescription();
  }

  private void ShowHiddenBundleCard()
  {
    GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
    if (!((UnityEngine.Object) currentDisplay != (UnityEngine.Object) null))
      return;
    currentDisplay.ShowHiddenBundleCard();
  }

  private void HideHiddenBundleCard()
  {
    GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
    if (!((UnityEngine.Object) currentDisplay != (UnityEngine.Object) null))
      return;
    currentDisplay.HideHiddenBundleCard();
  }

  private IEnumerator AnimateBundleBox(
    GeneralStorePacksContentDisplay display,
    float delayAnim,
    bool forceImmediate)
  {
    if (!this.m_waitingForBoxAnim)
    {
      while (this.m_animatingLogo)
        yield return (object) null;
      Log.Store.Print("AnimateBundleBox: delay = {0}", (object) delayAnim);
      this.StartAnimatingPacks();
      if ((double) delayAnim > 0.0)
        this.m_waitingForBoxAnim = true;
      int num = display.ShowBundleBox(this.m_boxFlyInAnimTime, this.m_boxFlyOutAnimTime, this.m_boxFlyInDelay, this.m_boxFlyOutDelay, delayAnim, forceImmediate);
      if (!forceImmediate && num != 0)
      {
        this.ShakeStore(1, this.m_boxFlyInXShake, delayAnim + this.m_boxFlyInAnimTime, this.m_boxStoreImpactTranslation);
        yield return (object) new WaitForSeconds(delayAnim + 1f);
      }
      this.DoneAnimatingPacks();
      this.m_waitingForBoxAnim = false;
    }
  }

  private IEnumerator AnimateLimitedTimeOfferUI(
    GeneralStorePacksContentDisplay display,
    bool waitForLogo)
  {
    GeneralStorePacksContent storePacksContent = this;
    if ((UnityEngine.Object) storePacksContent.m_limitedTimeOfferText != (UnityEngine.Object) null)
    {
      storePacksContent.m_limitedTimeOfferText.gameObject.SetActive(false);
      storePacksContent.m_limitedTimeOfferText.transform.localScale = storePacksContent.m_limitedTimeTextOrigScale;
    }
    if (storePacksContent.IsContentActive() && !storePacksContent.IsPackIdInvalid(storePacksContent.m_selectedStorePackId) && storePacksContent.m_showLimitedTimeOfferText && GameUtils.IsLimitedTimeOffer(storePacksContent.m_selectedStorePackId))
    {
      while (storePacksContent.m_animatingLogo & waitForLogo || storePacksContent.m_animatingPacks)
        yield return (object) null;
      if ((UnityEngine.Object) storePacksContent.m_limitedTimeOfferText != (UnityEngine.Object) null)
      {
        Network.Bundle hiddenLicenseBundle;
        StoreManager.Get().IsBoosterHiddenLicenseBundle(storePacksContent.m_selectedStorePackId, out hiddenLicenseBundle);
        if (StoreManager.Get().ShouldShowFeaturedDustJar(hiddenLicenseBundle))
          storePacksContent.m_limitedTimeOfferText.transform.position = storePacksContent.m_limitedTimeOfferDustBone.position;
        else
          storePacksContent.m_limitedTimeOfferText.transform.position = storePacksContent.m_limitedTimeOfferBone.position;
        storePacksContent.m_limitedTimeOfferText.Text = GameStrings.Get("GLUE_STORE_LIMITED_TIME_OFFER");
        storePacksContent.m_limitedTimeOfferText.gameObject.SetActive(true);
        storePacksContent.m_limitedTimeOfferText.transform.localScale = storePacksContent.m_limitedTimeTextOrigScale * 0.01f;
        iTween.ScaleTo(storePacksContent.m_limitedTimeOfferText.gameObject, iTween.Hash((object) "scale", (object) storePacksContent.m_limitedTimeTextOrigScale, (object) "time", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutQuad));
      }
    }
  }

  private void ResetAnimations()
  {
    if ((UnityEngine.Object) this.m_preorderCardBackReward != (UnityEngine.Object) null)
      this.m_preorderCardBackReward.HideCardBackReward();
    if ((UnityEngine.Object) this.m_availableDateText != (UnityEngine.Object) null)
      this.m_availableDateText.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_limitedTimeOfferText != (UnityEngine.Object) null)
      this.m_limitedTimeOfferText.gameObject.SetActive(false);
    if (this.m_logoAnimCoroutine != null)
    {
      iTween.Stop(this.m_logoMesh1.gameObject);
      iTween.Stop(this.m_logoMesh2.gameObject);
      this.StopCoroutine(this.m_logoAnimCoroutine);
    }
    this.m_logoMesh1.gameObject.SetActive(false);
    this.m_logoMesh2.gameObject.SetActive(false);
    if (this.m_packAnimCoroutine != null)
      this.StopCoroutine(this.m_packAnimCoroutine);
    if (this.m_limitedTimeOfferAnimCoroutine != null)
      this.StopCoroutine(this.m_limitedTimeOfferAnimCoroutine);
    if ((UnityEngine.Object) this.m_packBuyBonusCallout != (UnityEngine.Object) null)
      this.m_packBuyBonusCallout.DeactivateCallout();
    if (this.m_bonusPacksCalloutCoroutine != null)
      this.StopCoroutine(this.m_bonusPacksCalloutCoroutine);
    this.m_animatingLogo = false;
    this.m_animatingPacks = false;
    this.m_waitingForBoxAnim = false;
  }

  private void AnimateAndUpdateDisplay(StorePackId storePackId, bool forceImmediate = false)
  {
    if ((UnityEngine.Object) this.m_preorderCardBackReward != (UnityEngine.Object) null)
      this.m_preorderCardBackReward.HideCardBackReward();
    GameObject currDisplay = (GameObject) null;
    if (this.m_currentDisplay == -1)
    {
      this.m_currentDisplay = 1;
      currDisplay = this.m_packEmptyDisplay;
    }
    else
      currDisplay = this.GetCurrentDisplayContainer();
    GameObject displayContainer = this.GetNextDisplayContainer();
    this.GetCurrentLogo().gameObject.SetActive(false);
    this.GetCurrentDisplay().ClearContents();
    this.m_currentDisplay = (this.m_currentDisplay + 1) % 2;
    displayContainer.SetActive(true);
    if (!forceImmediate)
    {
      currDisplay.transform.localRotation = Quaternion.identity;
      displayContainer.transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);
      iTween.StopByName(currDisplay, "ROTATION_TWEEN");
      iTween.StopByName(displayContainer, "ROTATION_TWEEN");
      iTween.RotateBy(currDisplay, iTween.Hash((object) "amount", (object) new Vector3(0.5f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "name", (object) "ROTATION_TWEEN", (object) "oncomplete", (object) (Action<object>) (o => currDisplay.SetActive(false))));
      iTween.RotateBy(displayContainer, iTween.Hash((object) "amount", (object) new Vector3(0.5f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "name", (object) "ROTATION_TWEEN"));
      if (!string.IsNullOrEmpty(this.m_backgroundFlipSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_backgroundFlipSound);
    }
    else
    {
      displayContainer.transform.localRotation = Quaternion.identity;
      currDisplay.transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);
      currDisplay.SetActive(false);
    }
    IStorePackDef packDef = this.GetStorePackDef(storePackId);
    this.GetCurrentDisplay().UpdatePackType(packDef);
    MeshRenderer currLogo = this.GetCurrentLogo();
    if ((UnityEngine.Object) currLogo != (UnityEngine.Object) null)
    {
      this.m_hasLogo = !string.IsNullOrEmpty(packDef.GetLogoTextureName());
      if (this.m_hasLogo)
      {
        this.m_loadingLogoTexture = true;
        AssetHandleCallback<Texture> onTextureLoaded = (AssetHandleCallback<Texture>) null;
        onTextureLoaded = (AssetHandleCallback<Texture>) ((name, loadedTexture, data) =>
        {
          this.m_loadingLogoTexture = false;
          if (loadedTexture == null)
          {
            if ((bool) data)
            {
              Error.AddDevFatal("Loading localized logo failed.  This is normal if we're on android and just switched.  Trying unlocalized.");
              this.m_loadingLogoTexture = true;
              AssetLoader.Get().LoadAsset<Texture>((AssetReference) packDef.GetLogoTextureName(), onTextureLoaded, (object) false, AssetLoadingOptions.DisableLocalization);
            }
            else
              Debug.LogError((object) string.Format("Failed to load logo with texture {0}!", (object) this.name));
          }
          else if ((UnityEngine.Object) currLogo != (UnityEngine.Object) null)
          {
            RendererExtension.GetMaterial((Renderer) currLogo).mainTexture = (Texture) loadedTexture;
            ServiceManager.Get<DisposablesCleaner>()?.Attach((Component) currLogo, (IDisposable) loadedTexture);
          }
          else
            loadedTexture.Dispose();
        });
        AssetLoader.Get().LoadAsset<Texture>((AssetReference) packDef.GetLogoTextureName(), onTextureLoaded, (object) true);
        MeshRenderer glowLogo = this.GetCurrentGlowLogo();
        if ((UnityEngine.Object) glowLogo != (UnityEngine.Object) null)
        {
          this.m_loadingLogoGlowTexture = true;
          AssetLoader.Get().LoadAsset<Texture>((AssetReference) packDef.GetLogoTextureGlowName(), (AssetHandleCallback<Texture>) ((name, loadedTexture, data) =>
          {
            this.m_loadingLogoGlowTexture = false;
            if (loadedTexture == null)
              Debug.LogError((object) string.Format("Failed to load texture {0}!", (object) this.name));
            else if ((UnityEngine.Object) glowLogo != (UnityEngine.Object) null)
            {
              RendererExtension.GetMaterial((Renderer) glowLogo).mainTexture = (Texture) loadedTexture;
              ServiceManager.Get<DisposablesCleaner>()?.Attach((Component) glowLogo, (IDisposable) loadedTexture);
            }
            else
              loadedTexture.Dispose();
          }));
        }
      }
    }
    this.AnimateBuyBar();
  }

  private void AnimateBuyBar()
  {
    GameObject target = StoreManager.Get().IsBoosterPreorderActive(GameUtils.GetProductDataFromStorePackId(this.m_selectedStorePackId, this.m_lastBundleIndex), StorePackId.GetProductTypeFromStorePackType(this.m_selectedStorePackId), out Network.Bundle _) ? this.m_packBuyContainer : this.m_packBuyPreorderContainer;
    if (this.IsPackIdInvalid(this.m_selectedStorePackId))
      return;
    iTween.Stop(target);
    target.transform.localRotation = Quaternion.identity;
    iTween.RotateBy(target, iTween.Hash((object) "amount", (object) new Vector3(-1f, 0.0f, 0.0f), (object) "time", (object) this.m_backgroundFlipAnimTime, (object) "delay", (object) (1f / 1000f)));
  }

  private void UpdateKoreaInfoButton()
  {
    if ((UnityEngine.Object) this.m_ChinaInfoButton == (UnityEngine.Object) null)
      return;
    this.m_ChinaInfoButton.gameObject.SetActive(StoreManager.Get().IsKoreanCustomer() && this.IsContentActive() && !this.IsPackIdInvalid(this.m_selectedStorePackId));
  }

  private void OnKoreaInfoPressed(UIEvent e) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLUE_STORE_KOREAN_DISCLAIMER_HEADLINE"),
    m_text = GameStrings.Get("GLUE_STORE_KOREAN_DISCLAIMER_DETAILS"),
    m_showAlertIcon = true,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK
  });

  private bool IsPackIdFirstPurchaseBundle(StorePackId storePackId) => storePackId.Type == StorePackType.BOOSTER && storePackId.Id == 181;

  private bool IsPackIdInvalid(StorePackId storePackId) => storePackId.Type != StorePackType.BOOSTER && storePackId.Type != StorePackType.MODULAR_BUNDLE || storePackId.Id == 0;

  private void ClearButtonEventListeners()
  {
    foreach (PegUIElement packBuyButton in this.m_packBuyButtons)
      packBuyButton.ClearEventListeners();
    foreach (PegUIElement preorderBuyButton in this.m_packPreorderBuyButtons)
      preorderBuyButton.ClearEventListeners();
  }

  [Serializable]
  public class ToggleableButtonFrame
  {
    public GameObject m_Middle;
    public GameObject m_IBar;
  }

  [Serializable]
  public class MultiSliceEndCaps
  {
    public GameObject m_FullBar;
    public GameObject m_SmallerBar;
  }

  public enum LogoAnimation
  {
    None,
    Slam,
    Fade,
  }

  public enum ModularBundleLayoutButtonSize
  {
    None,
    Small,
    Large,
  }
}
