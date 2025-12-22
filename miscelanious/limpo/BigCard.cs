using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class BigCard : MonoBehaviour
{
  public BigCard.LayoutData m_LayoutData;
  public BigCard.SecretLayoutData m_SecretLayoutData;
  private static readonly Vector3 INVISIBLE_SCALE = new Vector3(0.0001f, 0.0001f, 0.0001f);
  private const string GHOST_CARD_BOTTOM = "GhostedCard_Bottom";
  private static BigCard s_instance;
  private Card m_card;
  private Actor m_bigCardActor;
  private TooltipPanel m_bigCardAsTooltip;
  private Actor m_twinCardActor;
  private Actor m_evolvingCardActor;
  private List<Actor> m_phoneSecretActors;
  private List<Actor> m_phoneSideQuestActors;
  private List<Actor> m_phoneSigilActors;
  private List<Actor> m_phoneObjectivesActors;
  private readonly PlatformDependentValue<float> PLATFORM_SCALING_FACTOR;
  private EnchantmentBanner m_enchantmentBanner;
  private Actor m_extraBigCardActor;

  public BigCard() => this.PLATFORM_SCALING_FACTOR = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 1f,
    Tablet = 1f,
    Phone = 1.3f,
    MiniTablet = 1f
  };

  public PlatformDependentValue<float> GetPlatformScalingFactor() => this.PLATFORM_SCALING_FACTOR;

  private void Awake()
  {
    BigCard.s_instance = this;
    this.m_enchantmentBanner = AssetLoader.Get().InstantiatePrefab((AssetReference) "EnchantmentBanner.prefab:e7058664cd0b13f4bb45e5b5f0385f34", AssetLoadingOptions.IgnorePrefabPosition).GetComponent<EnchantmentBanner>();
  }

  private void OnDestroy() => BigCard.s_instance = (BigCard) null;

  public static BigCard Get() => BigCard.s_instance;

  public Card GetCard() => this.m_card;

  public Actor GetExtraBigCardActor() => this.m_extraBigCardActor;

  public Actor GetBigCardActor() => this.m_bigCardActor;

  public void Show(Card card)
  {
    this.m_card = card;
    if (GameState.Get() != null && !GameState.Get().GetGameEntity().NotifyOfCardTooltipDisplayShow(card))
      return;
    Zone zone = card.GetZone();
    if ((bool) UniversalInputManager.UsePhoneUI && zone is ZoneSecret)
    {
      if (card.GetEntity().IsBobQuest())
        this.DisplayBigCardAsTooltip();
      else if (card.GetEntity().IsSideQuest())
        this.LoadAndDisplayTooltipPhoneSideQuests();
      else if (card.GetEntity().IsSigil())
        this.LoadAndDisplayTooltipPhoneSigils();
      else if (card.GetEntity().IsObjective())
        this.LoadAndDisplayTooltipPhoneObjectives();
      else
        this.LoadAndDisplayTooltipPhoneSecrets();
    }
    else
      this.LoadAndDisplayBigCard();
  }

  public void Hide()
  {
    if (GameState.Get() != null)
      GameState.Get().GetGameEntity().NotifyOfCardTooltipDisplayHide(this.m_card);
    this.HideBigCard();
    this.HideTooltipPhoneSecrets();
    this.HideTooltipPhoneSideQuests();
    this.HideTooltipPhoneSigils();
    this.HideTooltipPhoneObjectives();
    this.m_card = (Card) null;
  }

  public bool Hide(Card card)
  {
    if ((UnityEngine.Object) this.m_card != (UnityEngine.Object) card)
      return false;
    this.Hide();
    return true;
  }

  public void ShowSecretDeaths(Map<Player, DeadSecretGroup> deadSecretMap)
  {
    if (deadSecretMap == null || deadSecretMap.Count == 0)
      return;
    int num = 0;
    foreach (DeadSecretGroup deadSecretGroup in deadSecretMap.Values)
    {
      Card mainCard = deadSecretGroup.GetMainCard();
      List<Card> cards = deadSecretGroup.GetCards();
      List<Actor> actors = new List<Actor>();
      for (int index = 0; index < cards.Count; ++index)
      {
        Actor actor = this.LoadPhoneSecret(cards[index]);
        actors.Add(actor);
      }
      this.DisplayPhoneSecrets(mainCard, actors, true);
      ++num;
    }
  }

  private void LoadAndDisplayBigCard()
  {
    if ((bool) (UnityEngine.Object) this.m_extraBigCardActor)
      this.m_extraBigCardActor.Destroy();
    if ((bool) (UnityEngine.Object) this.m_bigCardActor)
      this.m_bigCardActor.Destroy();
    if (ActorNames.ShouldDisplayTooltipInsteadOfBigCard(this.m_card.GetEntity()))
    {
      this.DisplayBigCardAsTooltip();
    }
    else
    {
      string bigCardActor1 = ActorNames.GetBigCardActor(this.m_card.GetEntity());
      if (bigCardActor1 == "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9")
        return;
      this.m_bigCardActor = AssetLoader.Get().InstantiatePrefab((AssetReference) bigCardActor1, AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
      this.SetupActor(this.m_card, this.m_bigCardActor);
      int tag1 = this.m_card.GetEntity().GetTag(GAME_TAG.DISGUISED_TWIN);
      if (tag1 > 0)
      {
        using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(tag1))
        {
          string handActor = ActorNames.GetHandActor(fullDef?.EntityDef, this.m_card.GetEntity().GetPremiumType());
          this.m_twinCardActor = AssetLoader.Get().InstantiatePrefab((AssetReference) handActor, AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
          LayerUtils.SetLayer((Component) this.m_twinCardActor, GameLayer.Tooltip);
          this.m_twinCardActor.SetFullDef(fullDef);
          this.m_twinCardActor.SetPremium(this.m_card.GetEntity().GetPremiumType());
          this.m_twinCardActor.SetCardBackSideOverride(new Player.Side?(this.m_card.GetEntity().GetControllerSide()));
          this.m_twinCardActor.SetWatermarkCardSetOverride(this.m_card.GetEntity().GetWatermarkCardSetOverride());
          this.m_twinCardActor.UpdateAllComponents();
        }
      }
      Entity overBigCardEntity = GameState.Get().GetGameEntity().GetExtraMouseOverBigCardEntity(this.m_card.GetEntity());
      if (overBigCardEntity != null)
      {
        string bigCardActor2 = ActorNames.GetBigCardActor(overBigCardEntity);
        if (bigCardActor2 != "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9")
        {
          this.m_extraBigCardActor = AssetLoader.Get().InstantiatePrefab((AssetReference) bigCardActor2, AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
          this.SetupActor(overBigCardEntity.GetCard(), this.m_extraBigCardActor);
          this.m_extraBigCardActor.transform.parent = this.m_bigCardActor.transform;
          Vector3 vector3 = new Vector3(0.75f, 1f, 0.75f);
          if (UniversalInputManager.Get().IsTouchMode() && GameState.Get() != null && GameState.Get().GetGameEntity() != null && GameState.Get().GetGameEntity().GetGameOptions().GetBooleanOption(GameEntityOption.CAN_ADJUST_BIG_CARD_HORIZONTALLY))
          {
            vector3.x *= 0.92f;
            vector3.z *= 0.92f;
          }
          this.m_extraBigCardActor.transform.localScale = vector3;
        }
      }
      int tag2 = this.m_card.GetEntity().GetTag(GAME_TAG.BACON_EVOLUTION_CARD_ID);
      if (tag2 > 0)
      {
        using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(tag2))
        {
          string handActor = ActorNames.GetHandActor(fullDef?.EntityDef, this.m_card.GetEntity().GetPremiumType());
          GameObject parentObject = AssetLoader.Get().InstantiatePrefab((AssetReference) handActor, AssetLoadingOptions.IgnorePrefabPosition);
          this.m_evolvingCardActor = parentObject.GetComponent<Actor>();
          this.SetupActor(this.m_card, this.m_evolvingCardActor);
          this.m_evolvingCardActor.SetEntity((Entity) null);
          this.m_evolvingCardActor.transform.parent = this.m_bigCardActor.transform;
          this.m_evolvingCardActor.transform.localScale = Vector3.one;
          GameObject childBySubstring = GameObjectUtils.FindChildBySubstring(parentObject, "EvolutionVFX");
          if ((UnityEngine.Object) childBySubstring != (UnityEngine.Object) null)
            childBySubstring.SetActive(true);
          this.m_evolvingCardActor.SetFullDef(fullDef);
          this.m_evolvingCardActor.SetPremium(this.m_card.GetEntity().GetPremiumType());
          this.m_evolvingCardActor.SetCardBackSideOverride(new Player.Side?(this.m_card.GetEntity().GetControllerSide()));
          this.m_evolvingCardActor.SetWatermarkCardSetOverride(this.m_card.GetEntity().GetWatermarkCardSetOverride());
          this.m_evolvingCardActor.UpdateAllComponents();
        }
      }
      if (this.ShouldUseBonesForBigCardPlacement())
        this.DisplayBigCardWithBones();
      else
        this.DisplayBigCard();
    }
  }

  private bool ShouldUseBonesForBigCardPlacement()
  {
    if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bigCardActor == (UnityEngine.Object) null || GameState.Get() == null)
      return false;
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (gameEntity == null || !gameEntity.GetGameOptions().GetBooleanOption(GameEntityOption.USE_BONES_FOR_BIG_CARD_PLACEMENT) || (UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
      return false;
    switch (this.m_card.GetZone())
    {
      case ZonePlay _:
      case ZoneLettuceAbility _:
        Actor actor = this.m_card.GetActor();
        if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
          return false;
        if (this.m_card.GetEntity().IsMinion())
        {
          BigCardDisplayBones componentInChildren = actor.GetComponentInChildren<BigCardDisplayBones>();
          if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null || !componentInChildren.HasBonesForCurrentPlatform())
            return false;
        }
        else
        {
          if (!this.m_card.GetEntity().IsLettuceAbility())
            return false;
          MercenariesAbilityTray abilityTray = ZoneMgr.Get().GetLettuceZoneController().GetAbilityTray();
          if ((UnityEngine.Object) abilityTray == (UnityEngine.Object) null)
            return false;
          GameObject left;
          GameObject right;
          abilityTray.GetBigCardBones(out left, out right);
          if ((UnityEngine.Object) left == (UnityEngine.Object) null || (UnityEngine.Object) right == (UnityEngine.Object) null)
            return false;
        }
        return GameState.Get().MercenariesAllowBigCardBones();
      default:
        return false;
    }
  }

  private void HideBigCard()
  {
    if ((bool) (UnityEngine.Object) this.m_extraBigCardActor)
    {
      this.m_extraBigCardActor.Destroy();
      this.m_extraBigCardActor = (Actor) null;
    }
    if ((bool) (UnityEngine.Object) this.m_bigCardActor)
    {
      Card card = this.m_bigCardActor.GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      {
        Actor actor = card.GetActor();
        if ((UnityEngine.Object) actor != (UnityEngine.Object) null && (UnityEngine.Object) actor.gameObject != (UnityEngine.Object) null)
        {
          HeroBuddyWidget component = actor.GetComponent<HeroBuddyWidget>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            component.ShowProgressText(false);
        }
      }
      this.m_enchantmentBanner.ResetEnchantments();
      iTween.Stop(this.m_bigCardActor.gameObject);
      this.m_bigCardActor.Destroy();
      this.m_bigCardActor = (Actor) null;
      TooltipPanelManager.Get().HideKeywordHelp();
    }
    if ((bool) (UnityEngine.Object) this.m_bigCardAsTooltip)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bigCardAsTooltip.gameObject);
    if ((bool) (UnityEngine.Object) this.m_twinCardActor)
    {
      iTween.Stop(this.m_twinCardActor.gameObject);
      this.m_twinCardActor.Destroy();
      this.m_twinCardActor = (Actor) null;
    }
    if (!(bool) (UnityEngine.Object) this.m_evolvingCardActor)
      return;
    iTween.Stop(this.m_evolvingCardActor.gameObject);
    this.m_evolvingCardActor.Destroy();
    this.m_evolvingCardActor = (Actor) null;
  }

  private void DisplayBigCardAsTooltip()
  {
    if ((UnityEngine.Object) this.m_bigCardAsTooltip != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bigCardAsTooltip.gameObject);
    Vector3 vector3_1;
    if (this.m_card.GetEntity().IsBobQuest())
    {
      vector3_1 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(0.0f, 0.0f, 1.33f) : new Vector3(0.0f, 0.0f, 2.478f);
      if (this.m_card.GetControllerSide() == Player.Side.OPPOSING)
        vector3_1.z *= -1f;
    }
    else
      vector3_1 = !this.ShowBigCardOnRight() ? new Vector3(-2f, 0.0f, 0.0f) : new Vector3(2f, 0.0f, 0.0f);
    Vector3 vector3_2 = this.m_card.transform.position + vector3_1;
    this.m_bigCardAsTooltip = TooltipPanelManager.Get().CreateKeywordPanel(0);
    this.m_bigCardAsTooltip.Reset();
    this.m_bigCardAsTooltip.Initialize(this.m_card.GetEntity().GetName(), this.m_card.GetEntity().GetCardTextInHand());
    this.m_bigCardAsTooltip.SetScale((float) TooltipPanel.GAMEPLAY_SCALE);
    this.m_bigCardAsTooltip.transform.position = vector3_2;
    RenderUtils.SetAlpha(this.m_bigCardAsTooltip.gameObject, 0.0f);
    iTween.FadeTo(this.m_bigCardAsTooltip.gameObject, iTween.Hash((object) "alpha", (object) 1, (object) "time", (object) 0.1f));
  }

  private BigCard.BigCardDisplay_RelativeBoardPosition GetBoardPositionOfSourceCard()
  {
    if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
      return BigCard.BigCardDisplay_RelativeBoardPosition.INVALID;
    float f = (float) this.m_card.GetZonePosition() - (float) (this.m_card.GetZone().GetCardCount() + 1) / 2f;
    if ((double) Mathf.Abs(f) <= 0.5)
      return BigCard.BigCardDisplay_RelativeBoardPosition.MIDDLE;
    return (double) f < 0.0 ? BigCard.BigCardDisplay_RelativeBoardPosition.LEFT : BigCard.BigCardDisplay_RelativeBoardPosition.RIGHT;
  }

  private Vector3 GetScaleForCard(BigCardBoneLayout.ScaleSettings platformScale, Card card)
  {
    if (platformScale == null || (UnityEngine.Object) card == (UnityEngine.Object) null)
      return Vector3.one;
    if (card.GetEntity().IsMinion())
      return Vector3.one * platformScale.m_BigCardScale_Minion;
    return card.GetEntity().IsLettuceAbility() ? Vector3.one * platformScale.m_BigCardScale_LettuceAbility : Vector3.one;
  }

  private Vector3 AdjustYValueToBeLevelOnBoard(Vector3 bonePosition, Zone ownerZone)
  {
    ZonePlay zonePlay = ownerZone as ZonePlay;
    if ((UnityEngine.Object) zonePlay == (UnityEngine.Object) null)
      return bonePosition;
    float num = zonePlay.GetComponent<Collider>().bounds.center.y + (!(bool) UniversalInputManager.UsePhoneUI ? 0.33f : 0.3f);
    bonePosition.y = num;
    return bonePosition;
  }

  private void BigCardBones_UpdateEnchantmentBanner()
  {
    if (this.m_bigCardActor.GetCard().GetZone() is ZoneHand)
    {
      this.m_bigCardActor.SetEntity(this.m_bigCardActor.GetEntity());
      this.m_bigCardActor.UpdateTextComponents(this.m_bigCardActor.GetEntity());
    }
    else if ((UnityEngine.Object) this.m_twinCardActor == (UnityEngine.Object) null)
      this.m_enchantmentBanner.UpdateEnchantments(this.m_card, this.m_bigCardActor);
    else
      this.m_enchantmentBanner.ResetEnchantments();
  }

  private void BigCardBones_ShowTooltips(bool showOnRight)
  {
    if (GameState.Get() != null)
      GameState.Get().GetGameEntity().NotifyOfCardTooltipBigCardActorShow();
    BigCard.KeywordArgs keywordArgs = new BigCard.KeywordArgs()
    {
      card = this.m_card,
      actor = this.m_bigCardActor,
      showOnRight = showOnRight
    };
    float? overrideScale = new float?();
    if (this.m_card.GetEntity().IsHeroPower())
      overrideScale = new float?(0.6f);
    TooltipPanelManager.Get().UpdateKeywordHelp(keywordArgs.card, keywordArgs.actor, keywordArgs.showOnRight, overrideScale);
  }

  private void BigCardBones_ShowStateSpells()
  {
    if (!this.m_card.GetEntity().IsSilenced())
      return;
    this.m_bigCardActor.ActivateSpellBirthState(SpellType.SILENCE);
    if (!((UnityEngine.Object) this.m_twinCardActor != (UnityEngine.Object) null))
      return;
    this.m_twinCardActor.ActivateSpellBirthState(SpellType.SILENCE);
  }

  private void BigCardBones_ScaleAndPlaceBigCard(
    Actor bigCardActor,
    Zone actorZone,
    Vector3 scale,
    GameObject bone)
  {
    if ((UnityEngine.Object) bigCardActor == (UnityEngine.Object) null || (UnityEngine.Object) actorZone == (UnityEngine.Object) null || (UnityEngine.Object) bone == (UnityEngine.Object) null)
      return;
    bigCardActor.transform.position = this.AdjustYValueToBeLevelOnBoard(bone.transform.position, actorZone);
    bigCardActor.transform.localScale = scale;
  }

  private void BigCardBones_ActivateAndScaleIn(bool showTooltipsOnRight)
  {
    if ((UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null)
    {
      Vector3 localScale = this.m_bigCardActor.transform.localScale;
      this.m_bigCardActor.transform.localScale = Vector3.one;
      bool? nullable1 = new bool?(showTooltipsOnRight);
      iTween.ScaleTo(this.m_bigCardActor.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) this.m_LayoutData.m_ScaleSec, (object) "oncompleteparams", (object) nullable1, (object) "oncomplete", (object) (Action<object>) (tooltipsOnRight =>
      {
        if (!(tooltipsOnRight is bool?))
          return;
        bool? nullable2 = tooltipsOnRight as bool?;
        this.BigCardBones_ShowTooltips(nullable2.HasValue && nullable2.Value);
      })));
    }
    if ((UnityEngine.Object) this.m_extraBigCardActor != (UnityEngine.Object) null)
    {
      Vector3 localScale = this.m_bigCardActor.transform.localScale;
      this.m_extraBigCardActor.transform.localScale = Vector3.one;
      iTween.ScaleTo(this.m_extraBigCardActor.gameObject, localScale, this.m_LayoutData.m_ScaleSec);
    }
    if (!((UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null))
      return;
    this.m_bigCardActor.Show();
  }

  private void DisplayCardInPlayWithBones(out bool showTooltipsOnRight)
  {
    Actor actor = this.m_card.GetActor();
    showTooltipsOnRight = false;
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    BigCardDisplayBones componentInChildren = actor.GetComponentInChildren<BigCardDisplayBones>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    GameObject rig;
    BigCardBoneLayout.ScaleSettings scale;
    componentInChildren.GetRigForCurrentPlatform(out rig, out scale);
    if ((UnityEngine.Object) rig == (UnityEngine.Object) null)
      return;
    BigCardBoneLayout component = rig.GetComponent<BigCardBoneLayout>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    GameObject bone1;
    GameObject bone2;
    switch (this.GetBoardPositionOfSourceCard())
    {
      case BigCard.BigCardDisplay_RelativeBoardPosition.LEFT:
        bone1 = component.m_InnerRightBone;
        bone2 = component.m_OuterRightBone;
        showTooltipsOnRight = false;
        break;
      case BigCard.BigCardDisplay_RelativeBoardPosition.RIGHT:
        bone1 = component.m_InnerLeftBone;
        bone2 = component.m_OuterLeftBone;
        showTooltipsOnRight = true;
        break;
      case BigCard.BigCardDisplay_RelativeBoardPosition.MIDDLE:
        bone1 = component.m_InnerLeftBone;
        bone2 = component.m_InnerRightBone;
        showTooltipsOnRight = false;
        break;
      default:
        Log.Gameplay.PrintError("Unknown value for BigCardDisplay_RelativeBoardPosition.");
        return;
    }
    Transform parent = rig.transform.parent;
    Vector3 localScale = rig.transform.localScale;
    rig.transform.parent = (Transform) null;
    rig.transform.localScale = Vector3.one;
    Zone zone = this.m_bigCardActor.GetCard().GetZone();
    if ((UnityEngine.Object) this.m_bigCardActor != (UnityEngine.Object) null)
    {
      Vector3 scaleForCard = this.GetScaleForCard(scale, this.m_bigCardActor.GetCard());
      this.BigCardBones_ScaleAndPlaceBigCard(this.m_bigCardActor, zone, scaleForCard, bone1);
      this.BigCardBones_UpdateEnchantmentBanner();
    }
    if ((UnityEngine.Object) this.m_extraBigCardActor != (UnityEngine.Object) null)
    {
      Vector3 scaleForCard = this.GetScaleForCard(scale, this.m_extraBigCardActor.GetCard());
      this.BigCardBones_ScaleAndPlaceBigCard(this.m_extraBigCardActor, zone, scaleForCard, bone2);
    }
    rig.transform.localScale = localScale;
    rig.transform.parent = parent;
  }

  private void DisplayLettuceAbilitiesWithBones(out bool showTooltipsOnRight)
  {
    MercenariesAbilityTray abilityTray = ZoneMgr.Get().GetLettuceZoneController().GetAbilityTray();
    GameObject left;
    GameObject right;
    abilityTray.GetBigCardBones(out left, out right);
    GameObject bone;
    if (abilityTray.GetTrayPositionOfAbility(this.m_card) < 2)
    {
      bone = left;
      showTooltipsOnRight = true;
    }
    else
    {
      bone = right;
      showTooltipsOnRight = false;
    }
    if ((UnityEngine.Object) bone == (UnityEngine.Object) null)
      return;
    float num = abilityTray.GetAbilityPreviewScaleForCurrentPlatform();
    if ((double) num <= 0.0)
    {
      Debug.LogError((object) string.Format("Getting the ability card scale from the ability tray's scale settings returned an invalid scale value of {0} when it should be a positive value. Changing value to 1.0.", (object) num));
      num = 1f;
    }
    Vector3 scale = Vector3.one * num;
    this.BigCardBones_ScaleAndPlaceBigCard(this.m_bigCardActor, this.m_bigCardActor.GetCard().GetZone(), scale, bone);
  }

  private void DisplayBigCardWithBones()
  {
    bool showTooltipsOnRight = false;
    Entity entity = this.m_card.GetEntity();
    if (entity == null)
      return;
    if (entity.IsLettuceAbility())
      this.DisplayLettuceAbilitiesWithBones(out showTooltipsOnRight);
    else if (entity.IsMinion())
      this.DisplayCardInPlayWithBones(out showTooltipsOnRight);
    this.FitInsideScreenVerticalAxis();
    this.BigCardBones_ShowStateSpells();
    this.BigCardBones_ActivateAndScaleIn(showTooltipsOnRight);
  }

  private void DisplayBigCard()
  {
    Entity entity = this.m_card.GetEntity();
    bool flag1 = entity.GetController().IsFriendlySide();
    Zone zone = this.m_card.GetZone();
    Bounds bounds = this.m_bigCardActor.GetMeshRenderer().bounds;
    Vector3 position1 = this.m_card.GetActor().transform.position;
    Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 vector3_2 = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 scale = new Vector3(1.1f, 1.1f, 1.1f);
    float? overrideScale = new float?();
    switch (zone)
    {
      case ZoneHero _:
        vector3_1 = !flag1 ? new Vector3(0.0f, 4f, (float) (-(double) bounds.size.z * 0.699999988079071)) : new Vector3(0.0f, 4f, 0.0f);
        break;
      case ZoneWeapon _:
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          if (flag1)
          {
            scale = new Vector3(1.98f, 1.27f, 1.98f);
            vector3_1 = new Vector3(5.45f, 0.0f, bounds.size.z * 0.9f);
          }
          else
          {
            scale = new Vector3(1.65f, 1.65f, 1.65f);
            vector3_1 = new Vector3(-1.57f, 0.0f, -1f);
          }
        }
        else
        {
          scale = new Vector3(1.65f, 1.65f, 1.65f);
          vector3_1 = !flag1 ? new Vector3(-1.57f, 0.0f, -1f) : new Vector3(0.0f, 0.0f, bounds.size.z * 0.9f);
        }
        scale *= (float) this.PLATFORM_SCALING_FACTOR;
        break;
      case ZoneHeroPower _:
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          scale = new Vector3(1.3f, 1f, 1.3f);
          vector3_1 = !flag1 ? new Vector3(-3.5f, 8f, -3.35f) : new Vector3(-3.5f, 8f, 3.5f);
        }
        else
          vector3_1 = !flag1 ? new Vector3(0.0f, 4f, -2.6f) : new Vector3(0.0f, 4f, 2.69f);
        overrideScale = new float?(0.6f);
        if ((bool) (UnityEngine.Object) this.m_evolvingCardActor)
        {
          bool flag2 = this.ShowBigCardOnRight();
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            scale = new Vector3(1.1f, 1f, 1.1f);
            Vector3 vector3_3 = new Vector3(1.5f, 0.0f, 0.0f);
            if (flag2)
              vector3_1 -= vector3_3;
            else
              vector3_1 += vector3_3;
          }
          if (flag2)
          {
            this.m_evolvingCardActor.transform.localPosition = new Vector3(2.2f, 0.0f, 0.0f);
            break;
          }
          this.m_evolvingCardActor.transform.localPosition = new Vector3(-2.2f, 0.0f, 0.0f);
          break;
        }
        break;
      case ZoneBattlegroundHeroBuddy _:
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          scale = new Vector3(1f, 1f, 1f);
          vector3_1 = !flag1 ? new Vector3(-0.38f, 8f, -4.15f) : new Vector3(-0.38f, 8f, 4.1f);
          break;
        }
        vector3_1 = !flag1 ? new Vector3(0.33f, 4f, -3.7f) : new Vector3(0.4f, 4f, 3.17f);
        break;
      case ZoneBattlegroundQuestReward _:
        PlatformDependentVector3 dependentVector3_1 = new PlatformDependentVector3(PlatformCategory.Screen);
        dependentVector3_1.PC = new Vector3(-3.11f, 0.0f, 0.0f);
        dependentVector3_1.Phone = new Vector3(-6.71f, 0.0f, 0.0f);
        PlatformDependentVector3 dependentVector3_2 = dependentVector3_1;
        PlatformDependentVector3 dependentVector3_3 = new PlatformDependentVector3(PlatformCategory.Screen);
        dependentVector3_3.PC = new Vector3(1.5f, 4f, 3.17f);
        dependentVector3_3.Phone = new Vector3(4.33f, 4.1f, 3.1f);
        PlatformDependentVector3 dependentVector3_4 = dependentVector3_3;
        PlatformDependentVector3 dependentVector3_5 = new PlatformDependentVector3(PlatformCategory.Screen);
        dependentVector3_5.PC = new Vector3(1.54f, 4f, -2.8f);
        dependentVector3_5.Phone = new Vector3(4.17f, 4.1f, -2.45f);
        PlatformDependentVector3 dependentVector3_6 = dependentVector3_5;
        vector3_1 = !flag1 ? ((zone as ZoneBattlegroundQuestReward).m_isHeroPower ? dependentVector3_6.Value + dependentVector3_2.Value : (Vector3) (PlatformDependentValue<Vector3>) dependentVector3_6) : ((zone as ZoneBattlegroundQuestReward).m_isHeroPower ? dependentVector3_4.Value + dependentVector3_2.Value : (Vector3) (PlatformDependentValue<Vector3>) dependentVector3_4);
        scale = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(1.3f, 1f, 1.3f) : new Vector3(1.85f, 1f, 1.85f);
        break;
      case ZoneSecret _:
        scale = new Vector3(1.65f, 1.65f, 1.65f);
        vector3_1 = new Vector3(bounds.size.x + 0.3f, 0.0f, 0.0f);
        break;
      case ZoneHand _:
        vector3_1 = new Vector3(bounds.size.x * 0.7f, 4f, (float) (-(double) bounds.size.z * 0.800000011920929));
        scale = new Vector3(1.65f, 1.65f, 1.65f);
        break;
      case ZoneLettuceAbility _:
        MercenariesAbilityTray abilityTray = ZoneMgr.Get().GetLettuceZoneController().GetAbilityTray();
        GameObject left;
        GameObject right;
        abilityTray.GetBigCardBones(out left, out right);
        GameObject gameObject = abilityTray.GetTrayPositionOfAbility(this.m_card) < 2 ? left : right;
        if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        {
          position1 = gameObject.gameObject.transform.position;
          vector3_1 = Vector3.zero;
        }
        scale = new Vector3(1.65f, 1.65f, 1.65f) * (float) this.PLATFORM_SCALING_FACTOR;
        break;
      default:
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          scale = new Vector3(2f, 2f, 2f);
          vector3_1 = !this.ShowBigCardOnRight() ? new Vector3((float) (-(double) bounds.size.x - 2.20000004768372), 0.0f, 0.0f) : new Vector3(bounds.size.x + 2.2f, 0.0f, 0.0f);
        }
        else
        {
          scale = new Vector3(1.65f, 1.65f, 1.65f);
          if ((bool) (UnityEngine.Object) this.m_twinCardActor)
          {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
              vector3_1 = new Vector3(bounds.size.x + 0.7f, 0.0f, 0.0f);
              vector3_2 = new Vector3((float) (-(double) bounds.size.x - 0.699999988079071), 0.0f, 0.0f);
            }
            else
            {
              vector3_1 = new Vector3((float) (-(double) bounds.size.x - 0.699999988079071), 0.0f, 0.0f);
              vector3_2 = new Vector3(bounds.size.x + 0.7f, 0.0f, 0.0f);
            }
          }
          else
            vector3_1 = !this.ShowBigCardOnRight() ? new Vector3((float) (-(double) bounds.size.x - 0.699999988079071), 0.0f, 0.0f) : new Vector3(bounds.size.x + 0.7f, 0.0f, 0.0f);
        }
        if (zone is ZonePlay)
        {
          if ((bool) (UnityEngine.Object) this.m_extraBigCardActor)
          {
            bool flag3 = this.ShowBigCardOnRight();
            if (flag3)
              this.m_extraBigCardActor.transform.localPosition = new Vector3(1.9f, 0.1f, 0.07f);
            else
              this.m_extraBigCardActor.transform.localPosition = new Vector3(-1.9f, 0.1f, 0.07f);
            if (UniversalInputManager.Get().IsTouchMode() && GameState.Get() != null && GameState.Get().GetGameEntity() != null && GameState.Get().GetGameEntity().GetGameOptions().GetBooleanOption(GameEntityOption.CAN_ADJUST_BIG_CARD_HORIZONTALLY))
            {
              scale *= 0.8f;
              if (flag3)
                vector3_1 += new Vector3(-2f, 0.0f, 0.0f);
              else
                vector3_1 += new Vector3(2f, 0.0f, 0.0f);
            }
          }
          vector3_1 += new Vector3(0.0f, 0.1f, 0.0f);
          vector3_2 += new Vector3(0.0f, 0.1f, 0.0f);
          scale *= (float) this.PLATFORM_SCALING_FACTOR;
          break;
        }
        break;
    }
    Vector3 vector3_4 = new Vector3(0.02f, 0.02f, 0.02f);
    Vector3 vector3_5 = position1 + vector3_1 + vector3_4;
    if (zone is ZonePlay && entity.IsMinion() && entity.HasTag(GAME_TAG.LETTUCE_CONTROLLER))
    {
      float num = (zone as ZonePlay).GetComponent<Collider>().bounds.center.y + (!(bool) UniversalInputManager.UsePhoneUI ? 0.2f : 0.3f);
      vector3_5.y = num;
    }
    Vector3 vector3_6 = new Vector3(1f, 1f, 1f);
    Transform parent = this.m_bigCardActor.transform.parent;
    this.m_bigCardActor.transform.localScale = scale;
    this.m_bigCardActor.transform.position = vector3_5;
    this.m_bigCardActor.transform.parent = (Transform) null;
    Transform transform = (Transform) null;
    if ((bool) (UnityEngine.Object) this.m_twinCardActor)
      transform = this.m_twinCardActor.transform.parent;
    Vector3 vector3_7 = position1 + vector3_2 + vector3_4;
    if (this.m_card.GetEntity().GetTag(GAME_TAG.DISGUISED_TWIN) > 0 && (UnityEngine.Object) this.m_twinCardActor != (UnityEngine.Object) null)
    {
      this.m_twinCardActor.transform.localScale = scale;
      this.m_twinCardActor.transform.position = vector3_7;
      this.m_twinCardActor.transform.parent = (Transform) null;
    }
    if (zone is ZoneHand)
    {
      this.m_bigCardActor.SetEntity(entity);
      this.m_bigCardActor.UpdateTextComponents(entity);
    }
    else
    {
      if ((UnityEngine.Object) this.m_twinCardActor == (UnityEngine.Object) null)
        this.m_enchantmentBanner.UpdateEnchantments(this.m_card, this.m_bigCardActor);
      else
        this.m_enchantmentBanner.ResetEnchantments();
      if ((bool) UniversalInputManager.UsePhoneUI && this.m_enchantmentBanner.IsBannerVisible())
      {
        float num = this.m_enchantmentBanner.GetEnchantmentCount() > 1 ? 0.75f : 0.85f;
        scale *= num;
        this.m_bigCardActor.transform.localScale = scale;
      }
    }
    this.FitInsideScreenVerticalAxis();
    this.m_bigCardActor.transform.parent = parent;
    this.m_bigCardActor.transform.localScale = vector3_6;
    if ((bool) (UnityEngine.Object) this.m_twinCardActor)
    {
      this.m_twinCardActor.transform.parent = transform;
      this.m_twinCardActor.transform.localScale = vector3_6;
    }
    Vector3 position2 = this.m_bigCardActor.transform.position;
    this.m_bigCardActor.transform.position -= vector3_4;
    Vector3 position3 = new Vector3(0.0f, 0.0f, 0.0f);
    if ((bool) (UnityEngine.Object) this.m_twinCardActor)
    {
      position3 = this.m_twinCardActor.transform.position;
      this.m_twinCardActor.transform.position -= vector3_4;
    }
    BigCard.KeywordArgs keywordArgs1 = new BigCard.KeywordArgs();
    keywordArgs1.card = this.m_card;
    keywordArgs1.actor = this.m_bigCardActor;
    keywordArgs1.showOnRight = this.ShowBigCardOnRight();
    if (zone is ZoneHand)
    {
      iTween.ScaleTo(this.m_bigCardActor.gameObject, iTween.Hash((object) "scale", (object) scale, (object) "time", (object) this.m_LayoutData.m_ScaleSec, (object) "oncompleteparams", (object) keywordArgs1, (object) "oncomplete", (object) (Action<object>) (obj =>
      {
        if ((bool) UniversalInputManager.UsePhoneUI)
          return;
        BigCard.KeywordArgs keywordArgs2 = (BigCard.KeywordArgs) obj;
        TooltipPanelManager.Get().UpdateKeywordHelp(keywordArgs2.card, keywordArgs2.actor, keywordArgs2.showOnRight);
      })));
    }
    else
    {
      iTween.ScaleTo(this.m_bigCardActor.gameObject, scale, this.m_LayoutData.m_ScaleSec);
      if ((UnityEngine.Object) this.m_twinCardActor != (UnityEngine.Object) null)
      {
        BigCard.KeywordArgs keywordArgs3 = new BigCard.KeywordArgs()
        {
          card = this.m_card,
          actor = this.m_twinCardActor,
          showOnRight = this.ShowBigCardOnRight()
        };
        iTween.ScaleTo(this.m_twinCardActor.gameObject, scale, this.m_LayoutData.m_ScaleSec);
        iTween.MoveTo(this.m_twinCardActor.gameObject, position3, this.m_LayoutData.m_DriftSec);
        this.m_twinCardActor.transform.rotation = Quaternion.identity;
        this.m_twinCardActor.Show();
      }
      else if (!(bool) UniversalInputManager.UsePhoneUI)
      {
        if ((UnityEngine.Object) this.m_extraBigCardActor != (UnityEngine.Object) null || (UnityEngine.Object) this.m_evolvingCardActor != (UnityEngine.Object) null)
          keywordArgs1.showOnRight = !keywordArgs1.showOnRight;
        TooltipPanelManager.Get().UpdateKeywordHelp(keywordArgs1.card, keywordArgs1.actor, keywordArgs1.showOnRight, overrideScale);
      }
    }
    iTween.MoveTo(this.m_bigCardActor.gameObject, position2, this.m_LayoutData.m_DriftSec);
    this.m_bigCardActor.transform.rotation = Quaternion.identity;
    if (GameState.Get() != null)
      GameState.Get().GetGameEntity().NotifyOfCardTooltipBigCardActorShow();
    this.m_bigCardActor.Show();
    if ((bool) UniversalInputManager.UsePhoneUI)
      TooltipPanelManager.Get().UpdateKeywordHelp(this.m_card, this.m_bigCardActor, this.ShowKeywordOnRight(), overrideScale);
    if (!entity.IsSilenced())
      return;
    this.m_bigCardActor.ActivateSpellBirthState(SpellType.SILENCE);
    if (!((UnityEngine.Object) this.m_twinCardActor != (UnityEngine.Object) null))
      return;
    this.m_twinCardActor.ActivateSpellBirthState(SpellType.SILENCE);
  }

  private bool ShowBigCardOnRight() => UniversalInputManager.Get().IsTouchMode() ? this.ShowBigCardOnRightTouch() : this.ShowBigCardOnRightMouse();

  private bool ShowBigCardOnRightMouse()
  {
    if (this.m_card.GetEntity().IsHero() || this.m_card.GetEntity().IsHeroPower() || this.m_card.GetEntity().IsSecret())
      return true;
    if (this.m_card.GetEntity().GetCardId() == "TU4c_007")
      return false;
    ZonePlay zone = this.m_card.GetZone() as ZonePlay;
    if ((UnityEngine.Object) zone != (UnityEngine.Object) null)
    {
      Actor actor = this.m_card.GetActor();
      if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
      {
        MeshRenderer meshRenderer = actor.GetMeshRenderer();
        if ((UnityEngine.Object) meshRenderer != (UnityEngine.Object) null)
          return (double) meshRenderer.bounds.center.x < (double) (zone.GetComponent<BoxCollider>().bounds.center.x + zone.m_BigCardCenterOffset);
      }
    }
    return true;
  }

  private bool ShowBigCardOnRightTouch()
  {
    if (this.m_card.GetEntity().IsHero() || this.m_card.GetEntity().IsHeroPower() || this.m_card.GetEntity().IsSecret() || this.m_card.GetEntity().GetCardId() == "TU4c_007")
      return false;
    ZonePlay zone = this.m_card.GetZone() as ZonePlay;
    if ((UnityEngine.Object) zone != (UnityEngine.Object) null)
    {
      float num = !GameState.Get().GetGameEntity().GetGameOptions().GetBooleanOption(GameEntityOption.CAN_ADJUST_BIG_CARD_HORIZONTALLY) ? (!(bool) UniversalInputManager.UsePhoneUI ? -2.5f : 0.0f) : (!(bool) UniversalInputManager.UsePhoneUI ? 1f : 0.5f);
      Actor actor = this.m_card.GetActor();
      if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
      {
        MeshRenderer meshRenderer = actor.GetMeshRenderer();
        if ((UnityEngine.Object) meshRenderer != (UnityEngine.Object) null)
          return (double) meshRenderer.bounds.center.x < (double) (zone.GetComponent<BoxCollider>().bounds.center.x + num);
      }
    }
    return false;
  }

  private bool ShowKeywordOnRight()
  {
    if (this.m_card.GetEntity().IsHeroPower())
      return true;
    if (this.m_card.GetEntity().IsWeapon() || this.m_card.GetEntity().IsHero() || this.m_card.GetEntity().IsSecretLike() || this.m_card.GetEntity().GetCardId() == "TU4c_007")
      return false;
    ZonePlay zone = this.m_card.GetZone() as ZonePlay;
    if (!((UnityEngine.Object) zone != (UnityEngine.Object) null))
      return false;
    Actor actor = this.m_card.GetActor();
    MeshRenderer meshRenderer = (MeshRenderer) null;
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
    {
      meshRenderer = actor.GetMeshRenderer();
      if ((UnityEngine.Object) meshRenderer == (UnityEngine.Object) null)
        return false;
    }
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return (double) meshRenderer.bounds.center.x < (double) zone.GetComponent<BoxCollider>().bounds.center.x + 0.0299999993294477;
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    return gameEntity != null && gameEntity.GetGameOptions().GetBooleanOption(GameEntityOption.CAN_ADJUST_BIG_CARD_HORIZONTALLY) ? (double) meshRenderer.bounds.center.x > (double) zone.GetComponent<BoxCollider>().bounds.center.x + 0.5 : (double) meshRenderer.bounds.center.x > (double) zone.GetComponent<BoxCollider>().bounds.center.x;
  }

  private void FitInsideScreenVerticalAxis()
  {
    this.FitInsideScreenBottom();
    this.FitInsideScreenTop();
  }

  private Bounds CalculateBoundsOfSeveralMeshes(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return new Bounds(Vector3.zero, Vector3.zero);
    Bounds bounds = actor.GetMeshRenderer().bounds;
    if (actor.m_meshesThatAffectBoundsCalculations != null)
    {
      foreach (MeshRenderer boundsCalculation in actor.m_meshesThatAffectBoundsCalculations)
      {
        if ((UnityEngine.Object) boundsCalculation == (UnityEngine.Object) null)
          Debug.LogWarning((object) ("Actor \"" + actor.gameObject.name + "\" has a null entry in the m_meshesThatAffectBoundsCalculations array."));
        else
          bounds.Encapsulate(boundsCalculation.bounds);
      }
    }
    return bounds;
  }

  private Bounds CalculateLowerMeshBounds(Actor actor = null)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      actor = this.m_bigCardActor;
    if (this.m_enchantmentBanner.IsBannerVisible())
      return this.m_enchantmentBanner.GetLowerMeshBounds();
    return actor.m_meshesThatAffectBoundsCalculations != null && actor.m_meshesThatAffectBoundsCalculations.Count > 0 ? this.CalculateBoundsOfSeveralMeshes(actor) : actor.GetMeshRenderer().bounds;
  }

  private bool FitInsideScreenBottom()
  {
    Bounds lowerMeshBounds = this.CalculateLowerMeshBounds();
    Vector3 center = lowerMeshBounds.center;
    if ((bool) UniversalInputManager.UsePhoneUI)
      center.z -= 0.4f;
    Vector3 origin = new Vector3(center.x, center.y, center.z - lowerMeshBounds.extents.z);
    Ray ray = new Ray(origin, origin - center);
    Plane bottomPlane = CameraUtils.CreateBottomPlane(CameraUtils.FindFirstByLayer(GameLayer.Tooltip));
    float enter = 0.0f;
    if (bottomPlane.Raycast(ray, out enter) || Mathf.Approximately(enter, 0.0f))
      return false;
    TransformUtil.SetPosZ(this.m_bigCardActor.gameObject, this.m_bigCardActor.transform.position.z - enter);
    return true;
  }

  private Bounds CalculateMeshBoundsIncludingGem(Actor actor = null)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      actor = this.m_bigCardActor;
    if (actor.m_meshesThatAffectBoundsCalculations != null && actor.m_meshesThatAffectBoundsCalculations.Count > 0)
      return this.CalculateBoundsOfSeveralMeshes(actor);
    Bounds bounds1 = actor.GetMeshRenderer().bounds;
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null && actor.GetEntity() != null && (actor.GetEntity().IsSideQuest() || actor.GetEntity().IsSigil() || actor.GetEntity().IsObjective()))
    {
      foreach (MeshRenderer componentsInChild in actor.GetRootObject().GetComponentsInChildren<MeshRenderer>())
      {
        if (componentsInChild.gameObject.name.Equals("gem_mana", StringComparison.InvariantCultureIgnoreCase))
        {
          Bounds bounds2 = componentsInChild.bounds;
          bounds1.Encapsulate(bounds2);
          break;
        }
      }
    }
    return bounds1;
  }

  private bool FitInsideScreenTop()
  {
    Bounds boundsIncludingGem = this.CalculateMeshBoundsIncludingGem();
    Vector3 center = boundsIncludingGem.center;
    if ((bool) UniversalInputManager.UsePhoneUI && !(this.m_card.GetZone() is ZoneHeroPower))
      ++center.z;
    Vector3 origin = new Vector3(center.x, center.y, center.z + boundsIncludingGem.extents.z);
    Ray ray = new Ray(origin, origin - center);
    Plane topPlane = CameraUtils.CreateTopPlane(CameraUtils.FindFirstByLayer(GameLayer.Tooltip));
    float enter = 0.0f;
    if (topPlane.Raycast(ray, out enter) || Mathf.Approximately(enter, 0.0f))
      return false;
    TransformUtil.SetPosZ(this.m_bigCardActor.gameObject, this.m_bigCardActor.transform.position.z + enter);
    return true;
  }

  private void FitInsideScreenHorizontalAxis()
  {
    this.FitInsideScreenLeft();
    this.FitInsideScreenRight();
  }

  private bool FitInsideScreenLeft()
  {
    Bounds extraCardAndTooltips = this.ComputeBoundsOfBigCardExtraCardAndTooltips();
    Vector3 center = extraCardAndTooltips.center;
    Vector3 origin = new Vector3(center.x - extraCardAndTooltips.extents.x, center.y, center.z);
    Ray ray = new Ray(origin, origin - center);
    float enter;
    if (CameraUtils.CreateLeftPlane(CameraUtils.FindFirstByLayer(GameLayer.Tooltip)).Raycast(ray, out enter) || Mathf.Approximately(enter, 0.0f))
      return false;
    TransformUtil.SetPosX(this.m_bigCardActor.gameObject, this.m_bigCardActor.transform.position.x + enter);
    return true;
  }

  private bool FitInsideScreenRight()
  {
    Bounds extraCardAndTooltips = this.ComputeBoundsOfBigCardExtraCardAndTooltips();
    Vector3 center = extraCardAndTooltips.center;
    Vector3 origin = new Vector3(center.x + extraCardAndTooltips.extents.x, center.y, center.z);
    Ray ray = new Ray(origin, origin - center);
    float enter;
    if (CameraUtils.CreateRightPlane(CameraUtils.FindFirstByLayer(GameLayer.Tooltip)).Raycast(ray, out enter) || Mathf.Approximately(enter, 0.0f))
      return false;
    TransformUtil.SetPosX(this.m_bigCardActor.gameObject, this.m_bigCardActor.transform.position.x + enter);
    return true;
  }

  private Bounds ComputeBoundsOfBigCardExtraCardAndTooltips()
  {
    Bounds bounds = this.m_bigCardActor.GetMeshRenderer().bounds;
    List<TooltipPanel> currentTooltipPanels = TooltipPanelManager.Get().GetCurrentTooltipPanels();
    if (currentTooltipPanels != null)
    {
      foreach (Component component in currentTooltipPanels)
      {
        MeshRenderer[] componentsInChildren = component.gameObject.GetComponentsInChildren<MeshRenderer>();
        if (componentsInChildren != null)
        {
          foreach (MeshRenderer meshRenderer in componentsInChildren)
            bounds.Encapsulate(meshRenderer.bounds);
        }
      }
    }
    if ((UnityEngine.Object) this.m_extraBigCardActor != (UnityEngine.Object) null)
    {
      MeshRenderer[] componentsInChildren = this.m_extraBigCardActor.GetComponentsInChildren<MeshRenderer>();
      if (componentsInChildren != null)
      {
        foreach (MeshRenderer meshRenderer in componentsInChildren)
          bounds.Encapsulate(meshRenderer.bounds);
      }
    }
    return bounds;
  }

  public void ActivateBigCardStateSpells(
    Entity entity,
    Actor cardActor,
    Actor bigCardActor,
    EntityDef bigCardEntityDef = null)
  {
    if ((UnityEngine.Object) cardActor == (UnityEngine.Object) null)
      return;
    int num = 0;
    if (bigCardEntityDef != null)
    {
      if (bigCardEntityDef.UseTechLevelManaGem())
        num = bigCardEntityDef.GetTechLevel();
    }
    else if (cardActor.UseTechLevelManaGem())
      num = entity.GetTechLevel();
    if (num != 0)
    {
      bigCardActor.m_manaObject.SetActive(false);
      Spell spell = bigCardActor.GetSpell(SpellType.TECH_LEVEL_MANA_GEM);
      if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
      {
        spell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("TechLevel").Value = num;
        spell.ActivateState(SpellStateType.BIRTH);
      }
    }
    if (!cardActor.UseCoinManaGem())
      return;
    bigCardActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
  }

  private void LoadAndDisplayTooltipPhoneSigils()
  {
    if (this.m_phoneSigilActors == null)
    {
      this.m_phoneSigilActors = new List<Actor>();
    }
    else
    {
      foreach (Actor phoneSigilActor in this.m_phoneSigilActors)
        phoneSigilActor.Destroy();
      this.m_phoneSigilActors.Clear();
    }
    ZoneSecret zone = this.m_card.GetZone() as ZoneSecret;
    if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("BigCard.LoadAndDisplayTooltipPhoneSigils() called for a card that is not in a Secret Zone.");
    }
    else
    {
      List<Card> sigilCards = zone.GetSigilCards();
      for (int index = 0; index < sigilCards.Count; ++index)
        this.m_phoneSigilActors.Add(this.LoadPhoneSecret(sigilCards[index]));
      this.DisplayPhoneSecrets(this.m_card, this.m_phoneSigilActors, false);
    }
  }

  private void HideTooltipPhoneSigils()
  {
    if (this.m_phoneSigilActors == null)
      return;
    foreach (Actor phoneSigilActor in this.m_phoneSigilActors)
      this.HidePhoneSecret(phoneSigilActor);
    this.m_phoneSigilActors.Clear();
  }

  private void LoadAndDisplayTooltipPhoneObjectives()
  {
    if (this.m_phoneObjectivesActors == null)
    {
      this.m_phoneObjectivesActors = new List<Actor>();
    }
    else
    {
      foreach (Actor phoneObjectivesActor in this.m_phoneObjectivesActors)
        phoneObjectivesActor.Destroy();
      this.m_phoneObjectivesActors.Clear();
    }
    ZoneSecret zone = this.m_card.GetZone() as ZoneSecret;
    if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("BigCard.LoadAndDisplayTooltipPhoneObjectives() called for a card that is not in a Secret Zone.");
    }
    else
    {
      List<Card> objectiveCards = zone.GetObjectiveCards();
      for (int index = 0; index < objectiveCards.Count; ++index)
        this.m_phoneObjectivesActors.Add(this.LoadPhoneSecret(objectiveCards[index]));
      this.DisplayPhoneSecrets(this.m_card, this.m_phoneObjectivesActors, false);
    }
  }

  private void HideTooltipPhoneObjectives()
  {
    if (this.m_phoneObjectivesActors == null)
      return;
    foreach (Actor phoneObjectivesActor in this.m_phoneObjectivesActors)
      this.HidePhoneSecret(phoneObjectivesActor);
    this.m_phoneObjectivesActors.Clear();
  }

  private void LoadAndDisplayTooltipPhoneSecrets()
  {
    if (this.m_phoneSecretActors == null)
    {
      this.m_phoneSecretActors = new List<Actor>();
    }
    else
    {
      foreach (Actor phoneSecretActor in this.m_phoneSecretActors)
        phoneSecretActor.Destroy();
      this.m_phoneSecretActors.Clear();
    }
    ZoneSecret zone = this.m_card.GetZone() as ZoneSecret;
    if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("BigCard.LoadAndDisplayTooltipPhoneSecrets() called for a card that is not in a Secret Zone.");
    }
    else
    {
      List<Card> secretCards = zone.GetSecretCards();
      for (int index = 0; index < secretCards.Count; ++index)
        this.m_phoneSecretActors.Add(this.LoadPhoneSecret(secretCards[index]));
      this.DisplayPhoneSecrets(this.m_card, this.m_phoneSecretActors, false);
    }
  }

  private void HideTooltipPhoneSecrets()
  {
    if (this.m_phoneSecretActors == null)
      return;
    foreach (Actor phoneSecretActor in this.m_phoneSecretActors)
      this.HidePhoneSecret(phoneSecretActor);
    this.m_phoneSecretActors.Clear();
  }

  private void LoadAndDisplayTooltipPhoneSideQuests()
  {
    if (this.m_phoneSideQuestActors == null)
    {
      this.m_phoneSideQuestActors = new List<Actor>();
    }
    else
    {
      foreach (Actor phoneSideQuestActor in this.m_phoneSideQuestActors)
        phoneSideQuestActor.Destroy();
      this.m_phoneSideQuestActors.Clear();
    }
    ZoneSecret zone = this.m_card.GetZone() as ZoneSecret;
    if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("BigCard.LoadAndDisplayTooltipPhoneSideQuests() called for a card that is not in a Secret Zone.");
    }
    else
    {
      List<Card> sideQuestCards = zone.GetSideQuestCards();
      for (int index = 0; index < sideQuestCards.Count; ++index)
        this.m_phoneSideQuestActors.Add(this.LoadPhoneSecret(sideQuestCards[index]));
      this.DisplayPhoneSecrets(this.m_card, this.m_phoneSideQuestActors, false);
    }
  }

  private void HideTooltipPhoneSideQuests()
  {
    if (this.m_phoneSideQuestActors == null)
      return;
    foreach (Actor phoneSideQuestActor in this.m_phoneSideQuestActors)
      this.HidePhoneSecret(phoneSideQuestActor);
    this.m_phoneSideQuestActors.Clear();
  }

  private Actor LoadPhoneSecret(Card card)
  {
    string bigCardActor = ActorNames.GetBigCardActor(card.GetEntity());
    Actor component = AssetLoader.Get().InstantiatePrefab((AssetReference) bigCardActor, AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
    this.SetupActor(card, component);
    return component;
  }

  private Vector3 PhoneMoveSideQuestBigCardToTopOfScreen(
    Actor actor,
    Vector3 initialPosition)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null || !(bool) UniversalInputManager.UsePhoneUI)
      return initialPosition;
    Vector3 position = actor.transform.position;
    try
    {
      actor.transform.position = initialPosition;
      Bounds boundsIncludingGem = this.CalculateMeshBoundsIncludingGem(actor);
      Vector3 center = boundsIncludingGem.center;
      Vector3 origin = new Vector3(center.x, center.y, center.z + boundsIncludingGem.extents.z);
      Ray ray = new Ray(origin, origin - center);
      Plane topPlane = CameraUtils.CreateTopPlane(CameraUtils.FindFirstByLayer(GameLayer.Tooltip));
      float enter = 0.0f;
      topPlane.Raycast(ray, out enter);
      return initialPosition + new Vector3(0.0f, 0.0f, enter);
    }
    finally
    {
      actor.transform.position = position;
    }
  }

  private void DisplayPhoneSecrets(Card mainCard, List<Actor> actors, bool showDeath)
  {
    Vector3 initialOffset;
    Vector3 spacing;
    Vector3 drift;
    this.DetermineSecretLayoutOffsets(mainCard, actors, out initialOffset, out spacing, out drift);
    bool flag1 = GeneralUtils.IsOdd(actors.Count);
    Player controller = mainCard.GetController();
    ZoneSecret secretZone = controller.GetSecretZone();
    Actor actor1 = mainCard.GetActor();
    Vector3 initialPosition = secretZone.transform.position + initialOffset;
    for (int index = 0; index < actors.Count; ++index)
    {
      Actor actor2 = actors[index];
      Vector3 vector3;
      if (index == 0 & flag1)
      {
        vector3 = actors.Count != 1 || !actor2.GetCard().GetEntity().IsSideQuest() || !controller.IsFriendlySide() ? initialPosition : this.PhoneMoveSideQuestBigCardToTopOfScreen(actor2, initialPosition);
      }
      else
      {
        bool flag2 = GeneralUtils.IsOdd(index);
        bool flag3 = flag1 == flag2;
        double num1 = flag1 ? (double) Mathf.Ceil(0.5f * (float) index) : (double) Mathf.Floor(0.5f * (float) index);
        float num2 = (float) num1 * spacing.x;
        if (!flag1)
          num2 += 0.5f * spacing.x;
        if (flag3)
          num2 = -num2;
        float num3 = (float) num1 * spacing.z;
        vector3 = new Vector3(initialPosition.x + num2, initialPosition.y, initialPosition.z + num3);
      }
      actor2.transform.position = actor1.transform.position;
      actor2.transform.rotation = actor1.transform.rotation;
      actor2.transform.localScale = BigCard.INVISIBLE_SCALE;
      float time = showDeath ? this.m_SecretLayoutData.m_DeathShowAnimTime : this.m_SecretLayoutData.m_ShowAnimTime;
      Hashtable args1 = iTween.Hash((object) "position", (object) (vector3 - drift), (object) "time", (object) time, (object) "easeType", (object) iTween.EaseType.easeOutExpo);
      iTween.MoveTo(actor2.gameObject, args1);
      Hashtable args2 = iTween.Hash((object) "position", (object) vector3, (object) "delay", (object) time, (object) "time", (object) this.m_SecretLayoutData.m_DriftSec, (object) "easeType", (object) iTween.EaseType.easeOutExpo);
      iTween.MoveTo(actor2.gameObject, args2);
      iTween.ScaleTo(actor2.gameObject, this.transform.localScale, time);
      if (mainCard.GetEntity().IsSideQuest())
        actor2.ShowSideQuestProgressBanner();
      else if (mainCard.GetEntity().IsObjective())
        actor2.ShowObjectiveProgressBanner();
      else
        actor2.HideSideQuestProgressBanner();
      if (showDeath)
        this.ShowPhoneSecretDeath(actor2);
    }
  }

  private void DetermineSecretLayoutOffsets(
    Card mainCard,
    List<Actor> actors,
    out Vector3 initialOffset,
    out Vector3 spacing,
    out Vector3 drift)
  {
    Player controller = mainCard.GetController();
    bool flag = controller.IsFriendlySide();
    int num = controller.IsRevealed() ? 1 : 0;
    int minCardThreshold = this.m_SecretLayoutData.m_MinCardThreshold;
    int maxCardThreshold = this.m_SecretLayoutData.m_MaxCardThreshold;
    BigCard.SecretLayoutOffsets minCardOffsets = this.m_SecretLayoutData.m_MinCardOffsets;
    BigCard.SecretLayoutOffsets maxCardOffsets = this.m_SecretLayoutData.m_MaxCardOffsets;
    float t = Mathf.InverseLerp((float) minCardThreshold, (float) maxCardThreshold, (float) actors.Count);
    if (num != 0)
    {
      initialOffset = !flag ? Vector3.Lerp(minCardOffsets.m_OpponentInitialOffset, maxCardOffsets.m_OpponentInitialOffset, t) : Vector3.Lerp(minCardOffsets.m_InitialOffset, maxCardOffsets.m_InitialOffset, t);
      spacing = this.m_SecretLayoutData.m_Spacing;
    }
    else
    {
      initialOffset = !flag ? Vector3.Lerp(minCardOffsets.m_HiddenOpponentInitialOffset, maxCardOffsets.m_HiddenOpponentInitialOffset, t) : Vector3.Lerp(minCardOffsets.m_HiddenInitialOffset, maxCardOffsets.m_HiddenInitialOffset, t);
      spacing = this.m_SecretLayoutData.m_HiddenSpacing;
    }
    if (flag)
    {
      spacing.z = -spacing.z;
      drift = this.m_SecretLayoutData.m_DriftOffset;
    }
    else
      drift = -this.m_SecretLayoutData.m_DriftOffset;
  }

  private void ShowPhoneSecretDeath(Actor actor)
  {
    Spell.StateFinishedCallback deathSpellStateFinished = (Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
    {
      if (spell.GetActiveState() == SpellStateType.NONE)
        return;
      actor.Destroy();
    });
    Hashtable args = iTween.Hash((object) "time", (object) this.m_SecretLayoutData.m_TimeUntilDeathSpell, (object) "oncomplete", (object) (Action<object>) (obj =>
    {
      Spell spell = actor.GetSpell(SpellType.DEATH);
      spell.AddStateFinishedCallback(deathSpellStateFinished);
      spell.Activate();
    }));
    iTween.Timer(actor.gameObject, args);
  }

  private void HidePhoneSecret(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null || (UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
      return;
    Actor actor1 = this.m_card.GetActor();
    if ((UnityEngine.Object) actor1 != (UnityEngine.Object) null)
      iTween.MoveTo(actor.gameObject, actor1.transform.position, this.m_SecretLayoutData.m_HideAnimTime);
    iTween.ScaleTo(actor.gameObject, BigCard.INVISIBLE_SCALE, this.m_SecretLayoutData.m_HideAnimTime);
    Hashtable args = iTween.Hash((object) "time", (object) this.m_SecretLayoutData.m_HideAnimTime, (object) "oncomplete", (object) (Action<object>) (obj => actor.Destroy()));
    iTween.Timer(actor.gameObject, args);
  }

  private void SetupActor(Card card, Actor actor)
  {
    bool ignore = false;
    Entity entity = card.GetEntity();
    if (this.ShouldActorUseEntity(entity))
    {
      actor.SetEntity(entity);
      ignore = entity.HasTag(GAME_TAG.IGNORE_HIDE_STATS_FOR_BIG_CARD) || entity.IsDormant() && !entity.HasTag(GAME_TAG.HIDE_STATS);
    }
    GhostCard.Type ghostType = (GhostCard.Type) entity.GetTag(GAME_TAG.MOUSE_OVER_CARD_APPEARANCE);
    if (card.GetEntity().IsDormant())
      ghostType = GhostCard.Type.DORMANT;
    actor.GhostCardEffect(ghostType, entity.GetPremiumType(), false);
    EntityDef entityDef1 = entity.GetEntityDef();
    DefLoader.DisposableCardDef cardDef1 = card.ShareDisposableCardDef();
    int dbId = entity.GetTag(GAME_TAG.ALTERNATE_MOUSE_OVER_CARD);
    bool flag1 = entity.GetCardType() == TAG_CARDTYPE.BATTLEGROUND_HERO_BUDDY;
    if (flag1)
    {
      Entity hero = entity?.GetController()?.GetHero();
      if (hero != null)
      {
        dbId = hero.GetHeroBuddyCardId();
        actor.SetEntity((Entity) null);
      }
    }
    EntityDef entityDef2 = (EntityDef) null;
    if (dbId != 0)
    {
      entityDef2 = DefLoader.Get().GetEntityDef(dbId);
      if (entityDef2 == null)
        Log.Gameplay.PrintError("BigCard.SetupActor(): Unable to load EntityDef for card ID {0}.", (object) dbId);
      else
        entityDef1 = entityDef2;
      DefLoader.DisposableCardDef cardDef2 = DefLoader.Get().GetCardDef(dbId);
      if (cardDef2 == null)
      {
        Log.Spells.PrintError("BigCard.SetupActor(): Unable to load CardDef for card ID {0}.", (object) dbId);
      }
      else
      {
        cardDef1?.Dispose();
        cardDef1 = cardDef2;
      }
    }
    using (cardDef1)
    {
      if (this.ShouldActorUseEntityDef(entity))
      {
        actor.SetEntityDef(entityDef1);
        ignore = entityDef1.HasTag(GAME_TAG.IGNORE_HIDE_STATS_FOR_BIG_CARD) || entityDef1.IsDormant();
      }
      actor.SetPremium(entity.GetPremiumType());
      actor.SetCard(card);
      actor.SetCardDef(cardDef1);
      actor.SetIgnoreHideStats(ignore);
      actor.SetWatermarkCardSetOverride(entity.GetWatermarkCardSetOverride());
      actor.UpdateAllComponents();
      this.ActivateBigCardStateSpells(entity, card.GetActor(), actor, flag1 ? entityDef2 : (EntityDef) null);
      if (flag1)
      {
        GameObject childBySubstring1 = GameObjectUtils.FindChildBySubstring(this.m_bigCardActor.gameObject, "GhostCard");
        GameObject childBySubstring2 = GameObjectUtils.FindChildBySubstring(this.m_bigCardActor.gameObject, "RootObject");
        if ((UnityEngine.Object) childBySubstring1 != (UnityEngine.Object) null && (UnityEngine.Object) childBySubstring2 != (UnityEngine.Object) null)
        {
          GhostCard component = childBySubstring1.GetComponent<GhostCard>();
          component.enabled = true;
          component.SetGhostType(GhostCard.Type.DORMANT);
          component.RenderGhostCard(true);
          actor.GhostCardEffect(GhostCard.Type.DORMANT);
        }
        GameObject childBySubstring3 = GameObjectUtils.FindChildBySubstring(actor.gameObject, "GhostedCard_Bottom");
        if ((UnityEngine.Object) childBySubstring3 != (UnityEngine.Object) null)
        {
          bool flag2 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_2) != 0;
          childBySubstring3.SetActive(flag2);
        }
        else
          Debug.LogWarning((object) "BigCard.SetupActor - Bottom ghost card is missing");
        HeroBuddyWidget component1 = card.GetActor()?.GetComponent<HeroBuddyWidget>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          component1.ShowProgressText(true);
      }
      BoxCollider componentInChildren = actor.GetComponentInChildren<BoxCollider>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        componentInChildren.enabled = false;
      actor.name = "BigCard_" + actor.name;
      LayerUtils.SetLayer((Component) actor, GameLayer.Tooltip);
    }
  }

  private bool ShouldActorUseEntity(Entity entity) => entity.IsHidden() || (entity.GetZone() == TAG_ZONE.PLAY || entity.GetZone() == TAG_ZONE.SECRET) && entity.GetCardTextBuilder().ShouldUseEntityForTextInPlay() || entity.GetZone() == TAG_ZONE.HAND && entity.GetCardTextBuilder().ShouldUseEntityForTextInHand() || entity.IsDormant() || entity.IsSideQuest() || entity.IsSigil() || entity.IsSecret() || entity.IsObjective() || entity.IsCardButton() || GameMgr.Get().IsSpectator() && entity.GetZone() == TAG_ZONE.HAND && entity.GetController().IsOpposingSide();

  private bool ShouldActorUseEntityDef(Entity entity) => !entity.IsHidden() && !entity.IsCardButton() && !entity.IsDormant() && entity.GetZone() != TAG_ZONE.SECRET && (!GameMgr.Get().IsSpectator() || entity.GetZone() != TAG_ZONE.HAND || !entity.GetController().IsOpposingSide());

  [Serializable]
  public class LayoutData
  {
    public float m_ScaleSec = 0.15f;
    public float m_DriftSec = 10f;
  }

  [Serializable]
  public class SecretLayoutOffsets
  {
    public Vector3 m_InitialOffset = new Vector3(0.1f, 5f, 3.3f);
    public Vector3 m_OpponentInitialOffset = new Vector3(0.1f, 5f, -3.3f);
    public Vector3 m_HiddenInitialOffset = new Vector3(0.0f, 4f, 4f);
    public Vector3 m_HiddenOpponentInitialOffset = new Vector3(0.0f, 4f, -4f);
  }

  [Serializable]
  public class SecretLayoutData
  {
    public float m_ShowAnimTime = 0.15f;
    public float m_HideAnimTime = 0.15f;
    public float m_DeathShowAnimTime = 1f;
    public float m_TimeUntilDeathSpell = 1.5f;
    public float m_DriftSec = 5f;
    public Vector3 m_DriftOffset = new Vector3(0.0f, 0.0f, 0.05f);
    public Vector3 m_Spacing = new Vector3(2.1f, 0.0f, 0.7f);
    public Vector3 m_HiddenSpacing = new Vector3(2.4f, 0.0f, 0.7f);
    public int m_MinCardThreshold = 1;
    public int m_MaxCardThreshold = 5;
    public BigCard.SecretLayoutOffsets m_MinCardOffsets = new BigCard.SecretLayoutOffsets();
    public BigCard.SecretLayoutOffsets m_MaxCardOffsets = new BigCard.SecretLayoutOffsets();
  }

  private struct KeywordArgs
  {
    public Card card;
    public Actor actor;
    public bool showOnRight;
  }

  private enum BigCardDisplay_RelativeBoardPosition
  {
    INVALID,
    LEFT,
    RIGHT,
    MIDDLE,
  }
}
