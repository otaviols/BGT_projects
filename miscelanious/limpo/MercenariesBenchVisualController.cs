using System;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesBenchVisualController : MonoBehaviour
{
  public GameObject m_banner;
  public UberText m_bannerText;
  public GameObject m_cardDisplayCenterBone;
  public float m_cardOffsetWidth;
  public ActorStateType m_assignedActorState = ActorStateType.CARD_IDLE;
  [Header("Full Screen FX")]
  public float m_fullScreenFXTransitionTime;
  public iTween.EaseType m_fullScreenFXEaseType;
  public float m_vignetteAmount;
  public float m_desaturateAmount;
  private readonly List<Actor> m_actors = new List<Actor>();
  private readonly Pool<EnchantmentBanner> m_bannerPool = new Pool<EnchantmentBanner>();
  private ScreenEffectsHandle m_screenEffectsHandle;
  private const float ENCHANTMENT_SCALING_FACTOR = 0.6060606f;

  public void Awake()
  {
    if ((UnityEngine.Object) this.m_banner != (UnityEngine.Object) null)
      this.m_banner.SetActive(false);
    this.m_bannerPool.SetCreateItemCallback(new Pool<EnchantmentBanner>.CreateItemCallback(this.CreateEnchantmentBanner));
    this.m_bannerPool.SetDestroyItemCallback(new Pool<EnchantmentBanner>.DestroyItemCallback(this.DestroyEnchantmentBanner));
    this.m_bannerPool.SetExtensionCount(1);
    this.m_bannerPool.SetMaxReleasedItemCount(6);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private EnchantmentBanner CreateEnchantmentBanner(int index)
  {
    EnchantmentBanner component = AssetLoader.Get().InstantiatePrefab((AssetReference) "EnchantmentBanner.prefab:e7058664cd0b13f4bb45e5b5f0385f34", AssetLoadingOptions.IgnorePrefabPosition).GetComponent<EnchantmentBanner>();
    component.name = string.Format("{0}{1}", (object) "EnchantmentBanner", (object) index);
    component.transform.parent = this.transform;
    return component;
  }

  private void DestroyEnchantmentBanner(EnchantmentBanner panel) => UnityEngine.Object.Destroy((UnityEngine.Object) panel.gameObject);

  public void OnFriendlyBenchMouseOver(Action<string, string> showRegularTooltip)
  {
    ZoneDeck zoneOfType1 = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(Player.Side.FRIENDLY);
    if (zoneOfType1.GetCardCount() > 0)
    {
      if ((UnityEngine.Object) zoneOfType1.m_DeckCover != (UnityEngine.Object) null)
      {
        LayerUtils.SetLayer((Component) zoneOfType1.m_DeckCover, GameLayer.Tooltip);
        zoneOfType1.m_DeckCover.SetDeckCoverHighlightVisibility(true);
      }
      foreach (Card card in zoneOfType1.GetCards())
        this.LoadCardActor(card);
      this.LayoutCardActors();
      if ((UnityEngine.Object) this.m_banner != (UnityEngine.Object) null)
        this.m_banner.SetActive(true);
      List<EnchantmentBanner> activeList = this.m_bannerPool.GetActiveList();
      int count = this.m_actors.Count - activeList.Count;
      if (count > 0)
        this.m_bannerPool.AcquireBatch(count);
      else if (count < 0)
        this.m_bannerPool.ReleaseBatch(this.m_actors.Count, -count);
      for (int index = 0; index < this.m_actors.Count; ++index)
      {
        Actor actor = this.m_actors[index];
        Card card = actor.GetCard();
        activeList[index].UpdateEnchantments(card, actor, 0.6060606f);
      }
      this.m_screenEffectsHandle.StartEffect(new ScreenEffectParameters(ScreenEffectType.VIGNETTE | ScreenEffectType.DESATURATE, time: this.m_fullScreenFXTransitionTime, easeType: this.m_fullScreenFXEaseType, vignette: new VignetteParameters?(new VignetteParameters(this.m_vignetteAmount)), desaturate: new DesaturateParameters?(new DesaturateParameters(this.m_desaturateAmount))));
    }
    else
    {
      string str1 = GameStrings.Get("GAMEPLAY_TOOLTIP_LETTUCE_BENCH_HEADLINE");
      ZoneHand zoneOfType2 = ZoneMgr.Get().FindZoneOfType<ZoneHand>(Player.Side.FRIENDLY);
      string str2 = GameStrings.Format("GAMEPLAY_TOOLTIP_LETTUCE_BENCH_DESCRIPTION", (object) (zoneOfType1.GetCardCount() + zoneOfType2.GetCardCount()));
      showRegularTooltip(str1, str2);
    }
  }

  public void OnFriendlyBenchMouseOut()
  {
    if (this.m_actors.Count <= 0)
      return;
    ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(Player.Side.FRIENDLY);
    if ((UnityEngine.Object) zoneOfType.m_DeckCover != (UnityEngine.Object) null)
    {
      LayerUtils.SetLayer((Component) zoneOfType.m_DeckCover, GameLayer.Default);
      zoneOfType.m_DeckCover.SetDeckCoverHighlightVisibility(false);
    }
    foreach (Actor actor in this.m_actors)
      actor.Destroy();
    this.m_actors.Clear();
    if ((UnityEngine.Object) this.m_banner != (UnityEngine.Object) null)
      this.m_banner.SetActive(false);
    foreach (EnchantmentBanner active in this.m_bannerPool.GetActiveList())
      active.ResetEnchantments();
    this.m_screenEffectsHandle.StopEffect();
  }

  private void LayoutCardActors()
  {
    float platformScalingFactor = (float) BigCard.Get().GetPlatformScalingFactor();
    Vector3 position = this.m_cardDisplayCenterBone.transform.position;
    float num1 = this.m_cardOffsetWidth * platformScalingFactor;
    position.x -= (float) ((double) (this.m_actors.Count - 1) * (double) num1 / 2.0);
    foreach (Actor actor in this.m_actors)
    {
      actor.transform.localScale *= platformScalingFactor;
      float num2 = (float) ((double) actor.GetMeshRenderer().bounds.size.z * ((double) platformScalingFactor - 1.0) / 2.0);
      actor.transform.position = new Vector3(position.x, position.y, position.z - num2);
      position.x += num1;
    }
  }

  private void LoadCardActor(Card card)
  {
    string bigCardActor = ActorNames.GetBigCardActor(card.GetEntity());
    Actor component = AssetLoader.Get().InstantiatePrefab((AssetReference) bigCardActor, AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
    this.m_actors.Add(component);
    using (DefLoader.DisposableCardDef cardDef = card.ShareDisposableCardDef())
      component.SetCardDef(cardDef);
    Entity entity = card.GetEntity();
    component.SetEntity(entity);
    component.SetPremium(entity.GetPremiumType());
    component.SetCard(card);
    component.SetWatermarkCardSetOverride(entity.GetWatermarkCardSetOverride());
    component.UpdateAllComponents();
    component.SetActorState(this.m_assignedActorState);
    BoxCollider componentInChildren = component.GetComponentInChildren<BoxCollider>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.enabled = false;
    component.name = "MercBenchCard_" + component.name;
    component.transform.parent = this.transform;
    TransformUtil.Identity((Component) component.transform);
    LayerUtils.SetLayer((Component) component, GameLayer.Tooltip);
  }
}
