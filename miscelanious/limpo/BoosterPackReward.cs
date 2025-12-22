using Assets;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using UnityEngine;

public class BoosterPackReward : Reward
{
  public bool m_RotateIn = true;
  public GameObject m_BoosterPackBone;
  public GameLayer m_Layer = GameLayer.IgnoreFullScreenEffects;
  public Material m_PackGlowMaterial;
  public AnimationCurve m_RotationCurve;
  private bool m_AllowMultiStack = true;
  private UnopenedPack m_unopenedPack;
  [Header("Mercenaries")]
  public RewardBanner m_mercenariesRewardBannerPrefab;

  public bool AllowMultiStack
  {
    get => this.m_AllowMultiStack;
    set
    {
      this.m_AllowMultiStack = value;
      this.UpdatePackStacks();
    }
  }

  protected override void InitData() => this.SetData((RewardData) new BoosterPackRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    LayerUtils.SetLayer(this.m_root, this.m_Layer);
    if (!((UnityEngine.Object) this.m_unopenedPack != (UnityEngine.Object) null))
      return;
    Vector3 localScale = this.m_unopenedPack.transform.localScale;
    this.m_unopenedPack.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    iTween.ScaleTo(this.m_unopenedPack.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    if (!this.m_RotateIn)
      return;
    this.PlayRotateInAnimation();
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    this.m_BoosterPackBone.gameObject.SetActive(false);
    BoosterPackRewardData boosterRewardData = this.Data as BoosterPackRewardData;
    string empty1 = string.Empty;
    string empty2 = string.Empty;
    string source = string.Empty;
    string headline;
    if (boosterRewardData.Id == 629)
    {
      this.m_rewardBannerPrefab = this.m_mercenariesRewardBannerPrefab;
      this.UpdateBannerObject();
      headline = GameStrings.Get("GLOBAL_LETTUCE_REWARD_BANNER_TEXT");
    }
    else if (this.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE)
    {
      BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterRewardData.Id);
      if (record == null)
        return;
      headline = boosterRewardData.Count > 1 ? GameStrings.Get("GLOBAL_REWARD_BOOSTER_HEADLINE_OUT_OF_BAND_MULTI") : GameStrings.Get("GLOBAL_REWARD_BOOSTER_HEADLINE_OUT_OF_BAND");
      SpecialEventManager specialEventManager = SpecialEventManager.Get();
      SpecialEventType buyWithGoldEvent = record.BuyWithGoldEvent;
      if (!specialEventManager.IsEventActive(buyWithGoldEvent, false) && specialEventManager.GetEventStartTimeUtc(buyWithGoldEvent).HasValue && !specialEventManager.HasEventStarted(buyWithGoldEvent))
        source = GameStrings.Format("GLOBAL_REWARD_BOOSTER_DETAILS_PRESALE_OUT_OF_BAND", (object) boosterRewardData.Count);
      else
        source = GameStrings.Format("GLOBAL_REWARD_BOOSTER_DETAILS_OUT_OF_BAND", (object) boosterRewardData.Count);
    }
    else if (boosterRewardData.Count <= 1)
      headline = GameStrings.Get("GLOBAL_REWARD_BOOSTER_HEADLINE_GENERIC");
    else
      headline = GameStrings.Format("GLOBAL_REWARD_BOOSTER_HEADLINE_MULTIPLE", (object) boosterRewardData.Count);
    this.SetRewardText(headline, empty2, source);
    BoosterDbfRecord record1 = GameDbf.Booster.GetRecord(boosterRewardData.Id);
    if (record1 == null)
    {
      RewardBagDbfRecord record2 = GameDbf.RewardBag.GetRecord((Predicate<RewardBagDbfRecord>) (r => r.BagId == boosterRewardData.RewardChestBagNum.Value));
      switch (record2.Reward)
      {
        case RewardBag.Reward.LATEST_PACK:
          record1 = GameDbf.Booster.GetRecord((int) GameUtils.GetLatestRewardableBooster());
          break;
        case RewardBag.Reward.PACK_OFFSET_FROM_LATEST:
          record1 = GameDbf.Booster.GetRecord((int) GameUtils.GetRewardableBoosterOffsetFromLatest(record2.RewardData));
          break;
        default:
          Debug.LogWarning((object) string.Format("Unhandled RewardBag type: {0}", (object) record2.Reward));
          break;
      }
    }
    if (record1 == null)
      return;
    this.SetReady(false);
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) record1.PackOpeningPrefab, AssetLoadingOptions.IgnorePrefabPosition);
    gameObject.transform.parent = this.m_BoosterPackBone.transform.parent;
    gameObject.transform.localPosition = this.m_BoosterPackBone.transform.localPosition;
    gameObject.transform.rotation = this.m_BoosterPackBone.transform.rotation;
    gameObject.transform.localScale = this.m_BoosterPackBone.transform.localScale;
    this.m_unopenedPack = gameObject.GetComponent<UnopenedPack>();
    this.m_MeshRoot = gameObject;
    if ((UnityEngine.Object) this.m_unopenedPack.m_SingleStack.m_MeshRenderer != (UnityEngine.Object) null)
    {
      Texture mainTexture = RendererExtension.GetSharedMaterial(this.m_unopenedPack.m_SingleStack.m_MeshRenderer).mainTexture;
      RendererExtension.SetMaterial(this.m_unopenedPack.m_SingleStack.m_MeshRenderer, this.m_PackGlowMaterial);
      RendererExtension.GetMaterial(this.m_unopenedPack.m_SingleStack.m_MeshRenderer).mainTexture = mainTexture;
      if ((UnityEngine.Object) this.m_unopenedPack.m_SingleStack.m_Shadow != (UnityEngine.Object) null)
        this.m_unopenedPack.m_SingleStack.m_Shadow.SetActive(false);
    }
    if ((UnityEngine.Object) this.m_unopenedPack.m_MultipleStack.m_MeshRenderer != (UnityEngine.Object) null)
    {
      Texture mainTexture = RendererExtension.GetSharedMaterial(this.m_unopenedPack.m_MultipleStack.m_MeshRenderer).mainTexture;
      RendererExtension.SetMaterial(this.m_unopenedPack.m_MultipleStack.m_MeshRenderer, this.m_PackGlowMaterial);
      RendererExtension.GetMaterial(this.m_unopenedPack.m_MultipleStack.m_MeshRenderer).mainTexture = mainTexture;
      if ((UnityEngine.Object) this.m_unopenedPack.m_MultipleStack.m_Shadow != (UnityEngine.Object) null)
        this.m_unopenedPack.m_MultipleStack.m_Shadow.SetActive(false);
    }
    this.UpdatePackStacks();
    this.SetReady(true);
  }

  [ContextMenu("Play Rotate In Animation")]
  public void PlayRotateInAnimation() => this.StartCoroutine(this.RotateAnimation());

  private void UpdatePackStacks()
  {
    if (!(this.Data is BoosterPackRewardData data))
    {
      Debug.LogWarning((object) string.Format("BoosterPackReward.UpdatePackStacks() - Data {0} is not CardRewardData", (object) this.Data));
    }
    else
    {
      this.m_unopenedPack.SetBoosterId(data.Id);
      this.m_unopenedPack.SetCount(data.Count);
      bool flag1 = this.m_unopenedPack.CanOpenPack();
      bool flag2 = data.Count > 1;
      this.m_unopenedPack.m_SingleStack.m_RootObject.SetActive(!this.m_AllowMultiStack || !flag2 || !flag1);
      this.m_unopenedPack.m_MultipleStack.m_RootObject.SetActive(flag2 & flag1 && this.m_AllowMultiStack);
      this.m_unopenedPack.m_AmountBanner.SetActive(flag2);
      this.m_unopenedPack.m_AmountText.enabled = flag2;
      if (!flag2)
        return;
      this.m_unopenedPack.m_AmountText.Text = data.Count.ToString();
    }
  }

  private IEnumerator RotateAnimation()
  {
    float startTime = Time.timeSinceLevelLoad;
    while ((double) Time.timeSinceLevelLoad - (double) startTime < (double) this.m_RotationCurve.length)
    {
      this.m_unopenedPack.transform.localEulerAngles = new Vector3(this.m_unopenedPack.transform.localEulerAngles.x, this.m_unopenedPack.transform.localEulerAngles.y, this.m_RotationCurve.Evaluate(Time.timeSinceLevelLoad - startTime));
      yield return (object) null;
    }
  }
}
