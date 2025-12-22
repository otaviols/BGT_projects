using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStorePacksContentDisplay : MonoBehaviour
{
  public MeshRenderer m_background;
  public List<GameObject> m_packStacks = new List<GameObject>();
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_dustJar;
  public UberText m_dustAmountText;
  public int m_dustAmountTextFontSize;
  public int m_dustAmountTextFontSizeForBonus;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_hiddenCard;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_giftDescription;
  public UberText m_giftDescriptionText;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_firstPurchaseBundleGiftDescription;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_hiddenLicenseBundleGiftDescription;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_leavingSoonBannerPrefab;
  public GameObject m_jarFlash;
  public Animator m_jarFlashAnimController;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_dustJarBone;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_prePurchaseDustJarBone;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_bundleDustJarBone;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_giftDescriptionBone;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_prePurchaseGiftDescriptionBone;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_leavingSoonBone;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public GameObject m_nodeLayoutBone;
  private GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE m_packDisplayType;
  private GeneralStorePacksContent m_parent;
  private List<AnimatedLowPolyPack> m_showingPacks = new List<AnimatedLowPolyPack>();
  private List<ModularBundleNodeLayout> m_showingModularBundleNodeLayouts = new List<ModularBundleNodeLayout>();
  private List<AnimatedLeavingSoonSign> m_showingLeavingSoonSigns = new List<AnimatedLeavingSoonSign>();
  private static readonly Vector3 PACK_SCALE = new Vector3(0.06f, 0.03f, 0.06f);
  private static readonly Vector3 BOX_BUNDLE_DUST_SCALE = new Vector3(0.045f, 0.03f, 0.045f);
  private static readonly float PACK_X_VARIATION_MAG = 0.015f;
  private static readonly float PACK_Y_OFFSET = 0.02f;
  private static readonly float PACK_Z_VARIATION_MAG = 0.01f;
  private static readonly float PACK_FLY_OUT_X_DEG_VARIATION_MAG = 10f;
  private static readonly float PACK_FLY_OUT_Z_DEG_VARIATION_MAG = 10f;
  private static readonly float BOX_FLY_OUT_X_DEG_VARIATION_MAG = 0.0f;
  private static readonly float BOX_FLY_OUT_Z_DEG_VARIATION_MAG = 0.0f;
  private static readonly int PACK_STACK_SEED = 2;
  private int m_lastVisiblePacks;
  private int m_lastVisibleDust;
  private int m_lastVisibleDustBonus;
  private bool m_dustJarFlashing;
  private bool m_loadingModularBundle;
  private AssetHandle<Texture> m_packBackgroundTexture;
  private AssetHandle<Material> m_packBackgroundMaterial;
  private static Map<int, AnimatedLowPolyPack> s_packTemplates = new Map<int, AnimatedLowPolyPack>();

  public void SetParent(GeneralStorePacksContent parent) => this.m_parent = parent;

  public GeneralStorePacksContent GetParent() => this.m_parent;

  private void OnDestroy()
  {
    AssetHandle.SafeDispose<Texture>(ref this.m_packBackgroundTexture);
    AssetHandle.SafeDispose<Material>(ref this.m_packBackgroundMaterial);
  }

  public int ShowPacks(
    int numVisiblePacks,
    float flyInTime,
    float flyOutTime,
    float flyInDelay,
    float flyOutDelay,
    bool forceImmediate = false,
    bool showAsSingleStack = false)
  {
    if (showAsSingleStack)
      return this.ShowPacksAsSingleStack(numVisiblePacks, flyInTime, flyOutTime, flyInDelay, flyOutDelay, forceImmediate);
    this.m_packDisplayType = GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE.PACK;
    bool flag = this.m_parent.IsContentActive();
    AnimatedLowPolyPack[] currentPackLayout = this.ConfigureAndGetCurrentPackLayout(this.m_parent.GetStorePackId(), numVisiblePacks);
    if (currentPackLayout.Length != 0 && (UnityEngine.Object) currentPackLayout[0] != (UnityEngine.Object) null)
      currentPackLayout[0].HideBanner();
    if (this.m_lastVisiblePacks == numVisiblePacks)
      return 0;
    int numPacksFlyingOut = 0;
    for (int index = currentPackLayout.Length - 1; index >= numVisiblePacks; --index)
    {
      AnimatedLowPolyPack animatedLowPolyPack = currentPackLayout[index];
      if (flag && !forceImmediate)
      {
        if (animatedLowPolyPack.FlyOut(flyOutTime, flyOutDelay * (float) numPacksFlyingOut))
          ++numPacksFlyingOut;
      }
      else
        animatedLowPolyPack.FlyOutImmediate();
    }
    int numPacksFlyingIn = 0;
    for (int index = 0; index < numVisiblePacks; ++index)
    {
      AnimatedLowPolyPack animatedLowPolyPack = currentPackLayout[index];
      if (flag && !forceImmediate)
      {
        if (animatedLowPolyPack.FlyIn(flyInTime, flyInDelay * (float) numPacksFlyingIn))
          ++numPacksFlyingIn;
      }
      else
        animatedLowPolyPack.FlyInImmediate();
    }
    this.FlyLeavingSoonBanner(numPacksFlyingIn, numPacksFlyingOut, flyInTime, flyOutTime, flyInDelay, flyOutDelay, numVisiblePacks, flag && !forceImmediate);
    this.m_lastVisiblePacks = numVisiblePacks;
    return numPacksFlyingIn > numPacksFlyingOut ? numPacksFlyingIn : -numPacksFlyingOut;
  }

  public int ShowModularBundle(
    ModularBundleDbfRecord modularBundleRecord,
    bool forceImmediate,
    out float delay,
    out int weight,
    out ModularBundleNodeLayout prevLayout,
    int selectedIndex = 0)
  {
    List<ModularBundleLayoutDbfRecord> layoutsForBundle = StoreManager.Get().GetRegionNodeLayoutsForBundle(modularBundleRecord.ID);
    if (selectedIndex >= layoutsForBundle.Count)
    {
      Log.Store.PrintWarning(string.Format("Selected invalid sub-bundle at index={0}. Using sub-bundle at index=0", (object) selectedIndex));
      selectedIndex = 0;
    }
    ModularBundleLayoutDbfRecord currentLayoutRecord = layoutsForBundle[selectedIndex];
    prevLayout = this.m_showingModularBundleNodeLayouts.Count > 0 ? this.m_showingModularBundleNodeLayouts[0] : (ModularBundleNodeLayout) null;
    if ((UnityEngine.Object) prevLayout != (UnityEngine.Object) null && currentLayoutRecord.ID == prevLayout.LayoutID)
    {
      weight = 0;
      delay = 0.0f;
      this.m_parent.DoneAnimatingPacks();
      return 0;
    }
    List<ModularBundleLayoutNodeDbfRecord> records = GameDbf.ModularBundleLayoutNode.GetRecords((Predicate<ModularBundleLayoutNodeDbfRecord>) (r => r.NodeLayoutId == currentLayoutRecord.ID));
    records.Sort((Comparison<ModularBundleLayoutNodeDbfRecord>) ((l, r) => l.NodeIndex.CompareTo(r.NodeIndex)));
    weight = 0;
    int num = 0;
    foreach (ModularBundleLayoutNodeDbfRecord layoutNodeDbfRecord in records)
    {
      if (layoutNodeDbfRecord.ShakeWeight > 0)
      {
        ++num;
        weight += layoutNodeDbfRecord.ShakeWeight;
      }
    }
    if (this.m_loadingModularBundle)
    {
      delay = 0.0f;
      return 0;
    }
    this.m_loadingModularBundle = true;
    delay = (float) currentLayoutRecord.StoreShakeDelay;
    ModularBundleNodeLayout.NodeCallbackData callbackData = new ModularBundleNodeLayout.NodeCallbackData(currentLayoutRecord.ID, records, currentLayoutRecord.Prefab, forceImmediate);
    if ((UnityEngine.Object) prevLayout != (UnityEngine.Object) null)
    {
      prevLayout.PlayExitAnimationsInSequence(forceImmediate, new ModularBundleNodeLayout.OnModularBundleAnimationsFinished(this.OnPreviousModularBundleFinishAnimating), (object) callbackData);
      int outAnimWeight = 0;
      prevLayout.Nodes.ForEach((Action<ModularBundleNode>) (n => outAnimWeight += n.GetNodeShakeWeight()));
      this.m_parent.ShakeStore(prevLayout.Nodes.Count, 10f, weight: outAnimWeight);
    }
    else
      this.OnPreviousModularBundleFinishAnimating((object) callbackData);
    return num;
  }

  private void OnPreviousModularBundleFinishAnimating(object callbackData)
  {
    ModularBundleNodeLayout.NodeCallbackData callbackData1 = (ModularBundleNodeLayout.NodeCallbackData) callbackData;
    AssetLoader.Get().InstantiatePrefab(new AssetReference(callbackData1.prefab), new PrefabCallback<GameObject>(this.OnModularBundleNodeLayoutLoaded), (object) callbackData1);
  }

  private void OnModularBundleNodeLayoutLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    ModularBundleNodeLayout.NodeCallbackData nodeCallbackData = (ModularBundleNodeLayout.NodeCallbackData) callbackData;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null || !go.activeInHierarchy)
    {
      this.m_loadingModularBundle = false;
      this.m_parent.DoneAnimatingPacks();
    }
    else
    {
      this.ClearContents();
      ModularBundleNodeLayout component = go.GetComponent<ModularBundleNodeLayout>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        this.m_loadingModularBundle = false;
        this.m_parent.DoneAnimatingPacks();
      }
      else
      {
        GameUtils.SetParent((Component) component, this.m_nodeLayoutBone, true);
        component.Initialize(this, nodeCallbackData.layoutId, nodeCallbackData.layoutNodes);
        this.m_showingModularBundleNodeLayouts.Add(component);
        component.PlayEntranceAnimationsInSequence(nodeCallbackData.forceImmediate, new ModularBundleNodeLayout.OnModularBundleAnimationsFinished(this.OnModularBundleDoneAnimatingIn), (object) null);
        this.m_loadingModularBundle = false;
      }
    }
  }

  private void OnModularBundleDoneAnimatingIn(object callbackData) => this.m_parent.DoneAnimatingPacks();

  public IEnumerator ShowDustJar(
    int dustAmount,
    int dustAmountBonus,
    bool prePurchase,
    StorePackId selectedStorePackId)
  {
    if (!((UnityEngine.Object) this.m_dustJar == (UnityEngine.Object) null) && !((UnityEngine.Object) this.m_dustAmountText == (UnityEngine.Object) null))
    {
      this.m_dustJar.SetActive(true);
      if (prePurchase)
        TransformUtil.AttachAndPreserveLocalTransform(this.m_dustJar.transform, this.m_prePurchaseDustJarBone.transform);
      else if (GameUtils.IsHiddenLicenseBundleBooster(selectedStorePackId))
        TransformUtil.AttachAndPreserveLocalTransform(this.m_dustJar.transform, this.m_bundleDustJarBone.transform);
      else
        TransformUtil.AttachAndPreserveLocalTransform(this.m_dustJar.transform, this.m_dustJarBone.transform);
      if (dustAmount == this.m_lastVisibleDust && dustAmountBonus == this.m_lastVisibleDustBonus)
      {
        this.UpdateDustJarAmountText(dustAmount, dustAmountBonus);
      }
      else
      {
        if (this.m_dustJarFlashing)
          this.UpdateDustJarAmountText(this.m_lastVisibleDust, this.m_lastVisibleDustBonus);
        this.m_lastVisibleDust = dustAmount;
        this.m_lastVisibleDustBonus = dustAmountBonus;
        if ((UnityEngine.Object) this.m_jarFlash != (UnityEngine.Object) null && (UnityEngine.Object) this.m_jarFlashAnimController != (UnityEngine.Object) null)
        {
          this.m_jarFlash.SetActive(false);
          this.m_jarFlash.SetActive(true);
          this.m_jarFlashAnimController.enabled = true;
          this.m_jarFlashAnimController.StopPlayback();
          yield return (object) new WaitForEndOfFrame();
          if ((UnityEngine.Object) this.m_jarFlashAnimController == (UnityEngine.Object) null)
          {
            yield break;
          }
          else
          {
            this.m_jarFlashAnimController.Play("Flash");
            this.m_dustJarFlashing = true;
            while ((UnityEngine.Object) this.m_jarFlashAnimController != (UnityEngine.Object) null && (double) this.m_jarFlashAnimController.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5)
              yield return (object) null;
          }
        }
        this.UpdateDustJarAmountText(dustAmount, dustAmountBonus);
        this.m_dustJarFlashing = false;
      }
    }
  }

  private void UpdateDustJarAmountText(int dustAmount, int dustAmountBonus)
  {
    if (dustAmountBonus > 0)
    {
      this.m_dustAmountText.FontSize = this.m_dustAmountTextFontSizeForBonus;
      this.m_dustAmountText.Text = GameStrings.Format("GLUE_CHINA_STORE_DUST_PLUS_BONUS", (object) dustAmount, (object) dustAmountBonus);
    }
    else
    {
      this.m_dustAmountText.FontSize = this.m_dustAmountTextFontSize;
      this.m_dustAmountText.Text = dustAmount.ToString();
    }
  }

  public void HideDustJar()
  {
    if ((UnityEngine.Object) this.m_dustJar == (UnityEngine.Object) null)
      return;
    this.m_dustJar.SetActive(false);
  }

  public void ShowGiftDescription(
    int dustAmount,
    int dustBonusAmount,
    bool prePurchase,
    StorePackId selectedStorePackId)
  {
    if ((UnityEngine.Object) this.m_giftDescription == (UnityEngine.Object) null || (UnityEngine.Object) this.m_giftDescriptionText == (UnityEngine.Object) null || (UnityEngine.Object) this.m_firstPurchaseBundleGiftDescription == (UnityEngine.Object) null || (UnityEngine.Object) this.m_prePurchaseGiftDescriptionBone == (UnityEngine.Object) null || (UnityEngine.Object) this.m_giftDescriptionBone == (UnityEngine.Object) null || (UnityEngine.Object) this.m_hiddenLicenseBundleGiftDescription == (UnityEngine.Object) null)
      return;
    if (GameUtils.IsFirstPurchaseBundleBooster(selectedStorePackId))
    {
      this.m_giftDescription.SetActive(false);
      this.m_hiddenLicenseBundleGiftDescription.SetActive(false);
      this.m_firstPurchaseBundleGiftDescription.SetActive(true);
    }
    else if (GameUtils.IsHiddenLicenseBundleBooster(selectedStorePackId))
    {
      this.m_giftDescription.SetActive(false);
      this.m_firstPurchaseBundleGiftDescription.SetActive(false);
      this.m_hiddenLicenseBundleGiftDescription.SetActive(true);
    }
    else
    {
      this.m_giftDescription.SetActive(true);
      this.m_firstPurchaseBundleGiftDescription.SetActive(false);
      this.m_hiddenLicenseBundleGiftDescription.SetActive(false);
      if (prePurchase)
        this.m_giftDescriptionText.Text = GameStrings.Format("GLUE_CHINA_STORE_BOOSTER_GIFT_PREORDER_BONUS", (object) dustAmount);
      else if (dustBonusAmount > 0)
        this.m_giftDescriptionText.Text = GameStrings.Format("GLUE_CHINA_STORE_BOOSTER_GIFT_PLUS_BONUS", (object) dustAmount, (object) dustBonusAmount);
      else
        this.m_giftDescriptionText.Text = GameStrings.Format("GLUE_CHINA_STORE_BOOSTER_GIFT", (object) dustAmount);
    }
    if (prePurchase)
      TransformUtil.AttachAndPreserveLocalTransform(this.m_giftDescription.transform, this.m_prePurchaseGiftDescriptionBone.transform);
    else
      TransformUtil.AttachAndPreserveLocalTransform(this.m_giftDescription.transform, this.m_giftDescriptionBone.transform);
  }

  public void HideGiftDescription()
  {
    if ((UnityEngine.Object) this.m_giftDescription != (UnityEngine.Object) null)
      this.m_giftDescription.SetActive(false);
    if ((UnityEngine.Object) this.m_firstPurchaseBundleGiftDescription != (UnityEngine.Object) null)
      this.m_firstPurchaseBundleGiftDescription.SetActive(false);
    if (!((UnityEngine.Object) this.m_hiddenLicenseBundleGiftDescription != (UnityEngine.Object) null))
      return;
    this.m_hiddenLicenseBundleGiftDescription.SetActive(false);
  }

  public void ShowHiddenBundleCard()
  {
    if (!((UnityEngine.Object) this.m_hiddenCard != (UnityEngine.Object) null))
      return;
    this.m_hiddenCard.SetActive(true);
  }

  public void HideHiddenBundleCard()
  {
    if (!((UnityEngine.Object) this.m_hiddenCard != (UnityEngine.Object) null))
      return;
    this.m_hiddenCard.SetActive(false);
  }

  public int ShowBundleBox(
    float flyInTime,
    float flyOutTime,
    float flyInDelay,
    float flyOutDelay,
    float delay = 0.0f,
    bool forceImmediate = false)
  {
    Log.Store.Print("ShowBundleBox()");
    int numPacksFlyingIn = 1;
    if (this.m_lastVisiblePacks == numPacksFlyingIn)
      return 0;
    this.m_packDisplayType = GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE.BOX;
    bool animated = this.m_parent.IsContentActive();
    AnimatedLowPolyPack[] currentPackLayout = this.ConfigureAndGetCurrentPackLayout(this.m_parent.GetStorePackId(), 1);
    int num = 0;
    AnimatedLowPolyPack animatedLowPolyPack = currentPackLayout[0];
    if (!forceImmediate)
    {
      animatedLowPolyPack.FlyIn(flyInTime, delay);
      ++num;
    }
    else
      animatedLowPolyPack.FlyInImmediate();
    this.FlyLeavingSoonBanner(numPacksFlyingIn, 1, flyInTime, flyOutTime, flyInDelay, flyOutDelay, 1, animated);
    this.m_lastVisiblePacks = numPacksFlyingIn;
    return num;
  }

  public void PurchaseBundleBox(CardReward rewardCard)
  {
    AnimatedLowPolyPack[] currentPackLayout = this.ConfigureAndGetCurrentPackLayout(this.m_parent.GetStorePackId(), 1);
    CardRewardData cardRewardData = new CardRewardData();
    if ((UnityEngine.Object) rewardCard != (UnityEngine.Object) null)
      cardRewardData = rewardCard.Data as CardRewardData;
    if (currentPackLayout == null || currentPackLayout.Length < 1)
    {
      Debug.LogWarningFormat("PurchaseBundleBox() didn't caontain any packs for cardID {0}", (object) cardRewardData.CardID);
    }
    else
    {
      AnimatedLowPolyPack animatedLowPolyPack = currentPackLayout[0];
      if ((UnityEngine.Object) animatedLowPolyPack == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("PurchaseBundleBox() failed to get AnimatedLowPolyPack for cardID {0}", (object) cardRewardData.CardID);
      }
      else
      {
        FirstPurchaseBox firstPurchaseBox = animatedLowPolyPack.GetFirstPurchaseBox();
        if ((UnityEngine.Object) firstPurchaseBox == (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) rewardCard != (UnityEngine.Object) null)
          {
            rewardCard.transform.localPosition = this.GetRewardLocalPos();
            LayerUtils.SetLayer((Component) rewardCard, GameLayer.PerspectiveUI);
            RewardUtils.ShowReward(UserAttentionBlocker.NONE, (Reward) rewardCard, true, this.GetRewardPunchScale(), this.GetRewardScale(), new AnimationUtil.DelOnShownWithPunch(this.OnRewardShown), (object) rewardCard);
          }
          else
            Debug.LogWarning((object) "Null reference on rewardCard object.");
        }
        else
          firstPurchaseBox.PurchaseBundle(cardRewardData.CardID);
      }
    }
  }

  public void UpdatePackType(IStorePackDef packDef)
  {
    this.ClearContents();
    if ((UnityEngine.Object) this.m_background == (UnityEngine.Object) null || packDef == null)
      return;
    AssetLoader.Get().LoadAsset<Material>(ref this.m_packBackgroundMaterial, (AssetReference) packDef.GetBackgroundMaterial());
    if ((bool) this.m_packBackgroundMaterial)
      RendererExtension.SetMaterial((Renderer) this.m_background, (Material) this.m_packBackgroundMaterial);
    AssetLoader.Get().LoadAsset<Texture>(ref this.m_packBackgroundTexture, (AssetReference) packDef.GetBackgroundTexture());
    if (!(bool) this.m_packBackgroundTexture)
      return;
    RendererExtension.GetMaterial((Renderer) this.m_background).mainTexture = (Texture) this.m_packBackgroundTexture;
  }

  public void ClearContents()
  {
    foreach (Component showingPack in this.m_showingPacks)
      UnityEngine.Object.Destroy((UnityEngine.Object) showingPack.gameObject);
    this.m_showingPacks.Clear();
    foreach (Component showingLeavingSoonSign in this.m_showingLeavingSoonSigns)
      UnityEngine.Object.Destroy((UnityEngine.Object) showingLeavingSoonSign.gameObject);
    this.m_showingLeavingSoonSigns.Clear();
    foreach (Component bundleNodeLayout in this.m_showingModularBundleNodeLayouts)
      UnityEngine.Object.Destroy((UnityEngine.Object) bundleNodeLayout.gameObject);
    this.m_showingModularBundleNodeLayouts.Clear();
    this.m_lastVisiblePacks = 0;
    this.m_lastVisibleDust = 0;
    if ((UnityEngine.Object) this.m_dustJar != (UnityEngine.Object) null)
    {
      this.m_dustJar.SetActive(false);
      this.m_dustJarFlashing = false;
    }
    if ((UnityEngine.Object) this.m_hiddenCard != (UnityEngine.Object) null)
      this.m_hiddenCard.SetActive(false);
    if ((UnityEngine.Object) this.m_giftDescription != (UnityEngine.Object) null)
      this.m_giftDescription.SetActive(false);
    if ((UnityEngine.Object) this.m_firstPurchaseBundleGiftDescription != (UnityEngine.Object) null)
      this.m_firstPurchaseBundleGiftDescription.SetActive(false);
    if ((UnityEngine.Object) this.m_hiddenLicenseBundleGiftDescription != (UnityEngine.Object) null)
      this.m_hiddenLicenseBundleGiftDescription.SetActive(false);
    this.m_loadingModularBundle = false;
  }

  private int ShowPacksAsSingleStack(
    int numVisiblePacks,
    float flyInTime,
    float flyOutTime,
    float flyInDelay,
    float flyOutDelay,
    bool forceImmediate = false)
  {
    this.m_packDisplayType = GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE.PACK;
    bool flag = this.m_parent.IsContentActive();
    int num1 = GameUtils.IsFirstPurchaseBundleBooster(this.m_parent.GetStorePackId()) ? 1 : this.m_parent.GetStorePackId().Id;
    AnimatedLowPolyPack[] currentPackLayout = this.ConfigureAndGetCurrentPackLayout(new StorePackId()
    {
      Type = StorePackType.BOOSTER,
      Id = num1
    }, 1);
    this.FlyLeavingSoonBanner(0, 0, flyInTime, flyOutTime, flyInDelay, flyOutDelay, numVisiblePacks, flag && !forceImmediate);
    if (this.m_lastVisiblePacks == 1)
    {
      if (currentPackLayout.Length != 0 && (UnityEngine.Object) currentPackLayout[0] != (UnityEngine.Object) null)
        currentPackLayout[0].UpdateBannerCount(numVisiblePacks);
      return 0;
    }
    int num2 = 0;
    for (int index = currentPackLayout.Length - 1; index >= 1; --index)
    {
      AnimatedLowPolyPack animatedLowPolyPack = currentPackLayout[index];
      if (flag && !forceImmediate)
      {
        if (animatedLowPolyPack.FlyOut(flyOutTime, flyOutDelay * (float) num2))
          ++num2;
      }
      else
        animatedLowPolyPack.FlyOutImmediate();
    }
    currentPackLayout[0].FlyInImmediate();
    currentPackLayout[0].UpdateBannerCount(numVisiblePacks);
    this.m_lastVisiblePacks = 1;
    return 0;
  }

  private AnimatedLowPolyPack[] ConfigureAndGetCurrentPackLayout(
    StorePackId storePackId,
    int count)
  {
    if (count > this.m_showingPacks.Count)
    {
      AnimatedLowPolyPack original = (AnimatedLowPolyPack) null;
      if (!GeneralStorePacksContentDisplay.s_packTemplates.TryGetValue(storePackId.Id, out original) || !(bool) (UnityEngine.Object) original)
      {
        IStorePackDef storePackDef = this.m_parent.GetStorePackDef(storePackId);
        if (string.IsNullOrEmpty(storePackDef.GetLowPolyPrefab()) && string.IsNullOrEmpty(storePackDef.GetLowPolyDustPrefab()))
          return this.m_showingPacks.ToArray();
        original = (!GameUtils.IsHiddenLicenseBundleBooster(this.m_parent.GetStorePackId()) || GameUtils.IsFirstPurchaseBundleBooster(this.m_parent.GetStorePackId()) || !this.m_parent.SelectedBundleFeaturesDustJar() || string.IsNullOrEmpty(storePackDef.GetLowPolyDustPrefab()) ? AssetLoader.Get().InstantiatePrefab((AssetReference) storePackDef.GetLowPolyPrefab()) : AssetLoader.Get().InstantiatePrefab((AssetReference) storePackDef.GetLowPolyDustPrefab())).GetComponent<AnimatedLowPolyPack>();
        GeneralStorePacksContentDisplay.s_packTemplates[storePackId.Id] = original;
        original.gameObject.SetActive(false);
      }
      for (int count1 = this.m_showingPacks.Count; count1 < count; ++count1)
      {
        AnimatedLowPolyPack pack = UnityEngine.Object.Instantiate<AnimatedLowPolyPack>(original);
        this.SetupLowPolyPack(pack, count1, false);
        this.m_showingPacks.Add(pack);
      }
    }
    return this.m_showingPacks.ToArray();
  }

  private void SetupLowPolyPack(AnimatedLowPolyPack pack, int i, bool useVisiblePacksOnly)
  {
    pack.gameObject.SetActive(true);
    bool forceLastColumn = pack.m_isLeavingSoonBanner && this.m_parent.SelectedBundleFeaturesDustJar();
    int packColumn = this.DeterminePackColumn(i, forceLastColumn);
    GameUtils.SetParent((Component) pack, this.m_packStacks[packColumn], true);
    if (GameUtils.IsHiddenLicenseBundleBooster(this.m_parent.GetStorePackId()) && !GameUtils.IsFirstPurchaseBundleBooster(this.m_parent.GetStorePackId()) && this.m_parent.SelectedBundleFeaturesDustJar())
      pack.transform.localScale = GeneralStorePacksContentDisplay.BOX_BUNDLE_DUST_SCALE;
    else
      pack.transform.localScale = GeneralStorePacksContentDisplay.PACK_SCALE;
    pack.Init(packColumn, this.DeterminePackLocalPos(packColumn, this.m_showingPacks, useVisiblePacksOnly), new Vector3(0.0f, 3.5f, -0.1f));
    LayerUtils.SetLayer((Component) pack, this.m_packStacks[packColumn].layer);
    float y = 0.0f;
    float x = 0.0f;
    float z = 0.0f;
    Log.Store.Print("SetupLowPolyPack pack display type: {0}", (object) this.m_packDisplayType);
    if (this.m_packDisplayType == GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE.BOX)
    {
      y = UnityEngine.Random.Range(-this.m_parent.m_BoxYDegreeVariationMag, this.m_parent.m_BoxYDegreeVariationMag);
      x = UnityEngine.Random.Range(-GeneralStorePacksContentDisplay.BOX_FLY_OUT_X_DEG_VARIATION_MAG, GeneralStorePacksContentDisplay.BOX_FLY_OUT_X_DEG_VARIATION_MAG);
      z = UnityEngine.Random.Range(-GeneralStorePacksContentDisplay.BOX_FLY_OUT_Z_DEG_VARIATION_MAG, GeneralStorePacksContentDisplay.BOX_FLY_OUT_Z_DEG_VARIATION_MAG);
    }
    else
    {
      if (pack.m_isLeavingSoonBanner && this.m_parent.SelectedBundleFeaturesDustJar())
      {
        Vector3 vector3 = new Vector3(this.m_leavingSoonBone.transform.localEulerAngles.x, this.m_leavingSoonBone.transform.localEulerAngles.y, this.m_leavingSoonBone.transform.localEulerAngles.z);
        pack.SetFlyingLocalRotations(vector3, vector3);
        return;
      }
      if (this.m_packDisplayType == GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE.PACK)
      {
        y = UnityEngine.Random.Range(-this.m_parent.m_PackYDegreeVariationMag, this.m_parent.m_PackYDegreeVariationMag);
        x = UnityEngine.Random.Range(-GeneralStorePacksContentDisplay.PACK_FLY_OUT_X_DEG_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_FLY_OUT_X_DEG_VARIATION_MAG);
        z = UnityEngine.Random.Range(-GeneralStorePacksContentDisplay.PACK_FLY_OUT_Z_DEG_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_FLY_OUT_Z_DEG_VARIATION_MAG);
      }
    }
    Vector3 flyInLocalAngles = new Vector3(0.0f, y, 0.0f);
    Vector3 flyOutLocalAngles = new Vector3(x, 0.0f, z);
    pack.SetFlyingLocalRotations(flyInLocalAngles, flyOutLocalAngles);
  }

  private Vector3 DeterminePackLocalPos(
    int column,
    List<AnimatedLowPolyPack> packs,
    bool useVisiblePacksOnly)
  {
    List<AnimatedLowPolyPack> all = packs.FindAll((Predicate<AnimatedLowPolyPack>) (obj =>
    {
      if (obj.Column != column)
        return false;
      return !useVisiblePacksOnly || obj.GetState() == AnimatedLowPolyPack.State.FLOWN_IN || obj.GetState() == AnimatedLowPolyPack.State.FLYING_IN;
    }));
    Vector3 packLocalPos = Vector3.zero;
    if (this.m_packDisplayType == GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE.BOX && GameUtils.IsHiddenLicenseBundleBooster(this.m_parent.GetStorePackId()) && !GameUtils.IsFirstPurchaseBundleBooster(this.m_parent.GetStorePackId()) && this.m_parent.SelectedBundleFeaturesDustJar())
      packLocalPos = new Vector3(-0.06f, 0.0f, -0.03f);
    else if (this.m_packDisplayType != GeneralStorePacksContentDisplay.PACK_DISPLAY_TYPE.BOX && !GameUtils.IsHiddenLicenseBundleBooster(this.m_parent.GetStorePackId()))
    {
      packLocalPos.x = UnityEngine.Random.Range(-GeneralStorePacksContentDisplay.PACK_X_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_X_VARIATION_MAG);
      packLocalPos.y = GeneralStorePacksContentDisplay.PACK_Y_OFFSET * (float) all.Count;
      packLocalPos.z = UnityEngine.Random.Range(-GeneralStorePacksContentDisplay.PACK_Z_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_Z_VARIATION_MAG);
    }
    if (useVisiblePacksOnly && this.m_parent.SelectedBundleFeaturesDustJar())
      packLocalPos = this.m_leavingSoonBone.transform.localPosition;
    if (column % 2 == 0)
      packLocalPos.y += 0.03f;
    return packLocalPos;
  }

  private int DeterminePackColumn(int packNumber, bool forceLastColumn = false)
  {
    if (forceLastColumn)
      return this.m_packStacks.Count - 1;
    double num1 = new System.Random(GeneralStorePacksContentDisplay.PACK_STACK_SEED + packNumber).NextDouble();
    double num2 = 0.0;
    float num3 = 1f / (float) this.m_packStacks.Count;
    int packColumn;
    for (packColumn = 0; packColumn < this.m_packStacks.Count - 1; ++packColumn)
    {
      num2 += (double) num3;
      if (num1 <= num2)
        break;
    }
    return packColumn;
  }

  private void FlyLeavingSoonBanner(
    int numPacksFlyingIn,
    int numPacksFlyingOut,
    float flyInTime,
    float flyOutTime,
    float flyInDelay,
    float flyOutDelay,
    int numVisiblePacks,
    bool animated)
  {
    foreach (AnimatedLeavingSoonSign showingLeavingSoonSign in this.m_showingLeavingSoonSigns)
    {
      if (animated)
        showingLeavingSoonSign.FlyOut(flyOutTime, 0.0f);
      else
        showingLeavingSoonSign.FlyOutImmediate();
    }
    foreach (Component component in this.m_showingLeavingSoonSigns.FindAll((Predicate<AnimatedLeavingSoonSign>) (l => l.GetState() == AnimatedLowPolyPack.State.HIDDEN)))
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    this.m_showingLeavingSoonSigns.RemoveAll((Predicate<AnimatedLeavingSoonSign>) (l => l.GetState() == AnimatedLowPolyPack.State.HIDDEN));
    if (string.IsNullOrEmpty(this.m_leavingSoonBannerPrefab))
      return;
    BoosterDbfRecord boosterRecord = GameDbf.Booster.GetRecord(this.m_parent.GetStorePackId().Id);
    if (boosterRecord == null || !boosterRecord.LeavingSoon)
      return;
    AnimatedLeavingSoonSign pack = GameUtils.LoadGameObjectWithComponent<AnimatedLeavingSoonSign>(this.m_leavingSoonBannerPrefab);
    if ((UnityEngine.Object) pack == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) pack.m_leavingSoonButton != (UnityEngine.Object) null)
      pack.m_leavingSoonButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnLeavingSoonButtonClicked((string) boosterRecord.LeavingSoonText)));
    pack.m_isLeavingSoonBanner = true;
    this.SetupLowPolyPack((AnimatedLowPolyPack) pack, numVisiblePacks, true);
    this.m_showingLeavingSoonSigns.Add(pack);
    if (animated)
      pack.FlyIn(flyInTime, flyInDelay * (float) numPacksFlyingIn);
    else
      pack.FlyInImmediate();
  }

  private void OnLeavingSoonButtonClicked(string leavingSoonText) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLUE_STORE_EXPANSION_LEAVING_SOON"),
    m_text = leavingSoonText,
    m_showAlertIcon = true,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK
  });

  private Vector3 GetRewardLocalPos() => (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(2.4f, 55f, 306.2f),
    Phone = new Vector3(2.42f, 422.45f, 275f)
  };

  private Vector3 GetRewardScale() => (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(41f, 41f, 41f),
    Phone = new Vector3(14f, 14f, 14f)
  };

  private Vector3 GetRewardPunchScale() => (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(41.2f, 41.2f, 41.2f),
    Phone = new Vector3(14.2f, 14.2f, 14.2f)
  };

  private void OnRewardShown(object callbackData)
  {
    Reward reward = callbackData as Reward;
    if ((UnityEngine.Object) reward == (UnityEngine.Object) null)
      return;
    reward.RegisterClickListener(new Reward.OnClickedCallback(this.OnRewardClicked));
    reward.EnableClickCatcher(true);
    if (!(reward.Data is CardRewardData data))
      return;
    TAG_CLASS cardClass = DefLoader.Get().GetEntityDef(data.CardID).GetClass();
    NotificationManager.Get().PlayBundleInnkeeperLineForClass(cardClass);
  }

  private void OnRewardClicked(Reward reward, object userData)
  {
    reward.RemoveClickListener(new Reward.OnClickedCallback(this.OnRewardClicked));
    reward.Hide(true);
    ((GeneralStorePacksPane) ((GeneralStore) StoreManager.Get().GetCurrentStore()).GetCurrentPane()).RemoveFirstPurchaseBundle(0.0f);
  }

  public enum PACK_DISPLAY_TYPE
  {
    PACK,
    BOX,
  }
}
