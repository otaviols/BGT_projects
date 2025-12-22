using Blizzard.T5.AssetManager;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Cysharp.Threading.Tasks;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CustomEditClass]
public class Actor : MonoBehaviour, IVisibleWidgetComponent
{
  protected readonly UnityEngine.Vector2 GEM_TEXTURE_OFFSET_RARE = new UnityEngine.Vector2(0.5f, 0.0f);
  protected readonly UnityEngine.Vector2 GEM_TEXTURE_OFFSET_EPIC = new UnityEngine.Vector2(0.0f, 0.5f);
  protected readonly UnityEngine.Vector2 GEM_TEXTURE_OFFSET_LEGENDARY = new UnityEngine.Vector2(0.5f, 0.5f);
  protected readonly UnityEngine.Vector2 GEM_TEXTURE_OFFSET_COMMON = new UnityEngine.Vector2(0.0f, 0.0f);
  protected readonly Color GEM_COLOR_RARE = new Color(0.1529f, 0.498f, 1f);
  protected readonly Color GEM_COLOR_EPIC = new Color(0.596f, 0.1568f, 0.7333f);
  protected readonly Color GEM_COLOR_LEGENDARY = new Color(1f, 0.5333f, 0.0f);
  protected readonly Color GEM_COLOR_COMMON = new Color(0.549f, 0.549f, 0.549f);
  protected readonly Color CLASS_COLOR_GENERIC = new Color(0.7f, 0.7f, 0.7f);
  protected readonly Color CLASS_COLOR_WARLOCK = new Color(0.33f, 0.2f, 0.4f);
  protected readonly Color CLASS_COLOR_ROGUE = new Color(0.23f, 0.23f, 0.23f);
  protected readonly Color CLASS_COLOR_DRUID = new Color(0.42f, 0.29f, 0.14f);
  protected readonly Color CLASS_COLOR_SHAMAN = new Color(0.0f, 0.32f, 0.71f);
  protected readonly Color CLASS_COLOR_HUNTER = new Color(0.26f, 0.54f, 0.18f);
  protected readonly Color CLASS_COLOR_MAGE = new Color(0.44f, 0.48f, 0.69f);
  protected readonly Color CLASS_COLOR_PALADIN = new Color(0.71f, 0.49f, 0.2f);
  protected readonly Color CLASS_COLOR_PRIEST = new Color(1f, 1f, 1f);
  protected readonly Color CLASS_COLOR_WARRIOR = new Color(0.43f, 0.14f, 0.14f);
  protected readonly Color CLASS_COLOR_DEATHKNIGHT = new Color(0.0666667f, 0.5294f, 0.5843f);
  protected readonly Color CLASS_COLOR_DEMONHUNTER = new Color(0.0902f, 0.2275f, 0.1961f);
  protected readonly Color CLASS_COLOR_LOCATION_GENERIC = new Color(0.7f, 0.7f, 0.7f);
  protected readonly Color CLASS_COLOR_LOCATION_WARLOCK = new Color(0.3967f, 0.1721f, 0.5f);
  protected readonly Color CLASS_COLOR_LOCATION_ROGUE = new Color(0.1981f, 0.1981f, 0.1981f);
  protected readonly Color CLASS_COLOR_LOCATION_DRUID = new Color(0.3301f, 0.2281f, 0.1105f);
  protected readonly Color CLASS_COLOR_LOCATION_SHAMAN = new Color(0.0f, 0.2101f, 0.5377f);
  protected readonly Color CLASS_COLOR_LOCATION_HUNTER = new Color(0.1492f, 0.3679f, 0.0885f);
  protected readonly Color CLASS_COLOR_LOCATION_MAGE = new Color(0.3037f, 0.5386f, 0.8584f);
  protected readonly Color CLASS_COLOR_LOCATION_PALADIN = new Color(0.6792f, 0.4239f, 0.0865f);
  protected readonly Color CLASS_COLOR_LOCATION_PRIEST = new Color(0.8207f, 0.8207f, 0.8207f);
  protected readonly Color CLASS_COLOR_LOCATION_WARRIOR = new Color(0.5566f, 0.1128f, 0.1762f);
  protected readonly Color CLASS_COLOR_LOCATION_DEATHKNIGHT = new Color(0.07f, 0.53f, 0.58f);
  protected readonly Color CLASS_COLOR_LOCATION_DEMONHUNTER = new Color(0.1406f, 0.3773f, 0.3247f);
  private readonly Color MISSING_CARD_WILD_GOLDEN_COLOR = new Color(0.518f, 0.361f, 0.0f, 0.68f);
  private readonly Color MISSING_CARD_STANDARD_GOLDEN_COLOR = new Color(0.867f, 0.675f, 0.22f, 0.53f);
  protected readonly Color MISSING_CARD_WILD_DIAMOND_COLOR = new Color(0.4705f, 0.3058f, 0.0117f, 0.6784f);
  protected readonly string MISSING_CARD_WILD_DIAMOND_CONTRAST_KEY = "_Contrast";
  protected readonly float MISSING_CARD_WILD_DIAMOND_CONTRAST = 0.4f;
  protected readonly string MISSING_CARD_WILD_DIAMOND_INTENSITY_KEY = "_Intensity";
  protected readonly float MISSING_CARD_WILD_DIAMOND_INTENSITY = 1.7f;
  protected readonly float WATERMARK_ALPHA_VALUE = 99f / 128f;
  public GameObject m_cardMesh;
  public int m_cardFrontMatIdx = -1;
  public int m_cardBackMatIdx = -1;
  public int m_premiumRibbon = -1;
  public GameObject m_portraitMesh;
  public GameObject m_portraitMeshRTT;
  public GameObject m_portraitMeshRTT_background;
  public bool m_usePlayPortrait;
  public int m_portraitFrameMatIdx = -1;
  public int m_portraitMatIdx = -1;
  public GameObject m_xpBarRootObject;
  public GameObject m_nameBannerMesh;
  public GameObject m_descriptionMesh;
  public GameObject m_descriptionTrimMesh;
  public GameObject m_baconQuestDescriptionMesh;
  public GameObject m_watermarkMesh;
  public GameObject m_rarityFrameMesh;
  public GameObject m_rarityGemMesh;
  public GameObject m_racePlateMesh;
  public Mesh m_spellDescriptionMeshNeutral;
  public Mesh m_spellDescriptionMeshSchool;
  public GameObject m_attackObject;
  public GameObject m_healthObject;
  public GameObject m_armorObject;
  public GameObject m_manaObject;
  public CardRuneBanner m_cardRuneBanner;
  public GameObject m_deckRunesContainer;
  public RuneSlotVisual m_deckRuneSlotVisual;
  public GameObject m_speedWingObject;
  public GameObject m_racePlateObject;
  public GameObject m_multiRacePlateObject;
  public GameObject m_cardTypeAnchorObject;
  public GameObject m_eliteObject;
  public GameObject m_classIconObject;
  public GameObject m_heroSpotLight;
  public GameObject m_glints;
  public GameObject m_armorSpellBone;
  public GameObject m_decorationRoot;
  public NestedPrefab m_multiClassBannerContainer;
  public NestedPrefab m_tradeableBannerContainer;
  public NestedPrefab m_bannedRibbonContainer;
  public List<MercenaryRoleGemObject> m_mercenaryRoleObjects;
  public MercenaryActorLevelObject m_mercenaryLevelObject;
  public GameObject m_portraitFrameObject;
  public GameObject m_mercenaryTreasureBannerObject;
  public Actor.FactionObject[] m_factionBannerIcons;
  public GameObject m_factionBannerBackground;
  public GameObject m_bannerContainer;
  public GameObject m_banner;
  public GameObject m_bannerBottom;
  public UberText m_bannerText;
  public List<MeshRenderer> m_meshesThatAffectBoundsCalculations;
  public UberText m_costTextMesh;
  public UberText m_attackTextMesh;
  public UberText m_healthTextMesh;
  public UberText m_armorTextMesh;
  public UberText m_nameTextMesh;
  public UberText m_powersTextMesh;
  public UberText m_raceTextMesh;
  public UberText m_multiRaceTextMesh;
  public UberText m_bgQuestPowerTextMesh;
  public UberText m_bgQuestRaceTextMesh;
  public UberText m_secretText;
  public GameObject m_missingCardEffect;
  public GameObject m_ghostCardGameObject;
  public bool m_ghostCardActive;
  public GameObject m_diamondPortraitR2T;
  public LettuceMinionInPlayFrame m_lettuceMinionInPlayFrame;
  public GameObject m_enchantmentBannerAnchorObject;
  public Widget m_amountBannerWidget;
  public bool m_isDebuggingBattlegroundQuestReward;
  private bool m_showUICardText;
  private string m_UICardText;
  private Transform m_spellsParent;
  [CustomEditField(T = EditType.ACTOR)]
  public string m_spellTablePrefab;
  protected Card m_card;
  protected Entity m_entity;
  protected CardDefHandle m_cardDefHandle = new CardDefHandle();
  protected EntityDef m_entityDef;
  protected TAG_PREMIUM m_premiumType;
  protected ProjectedShadow m_projectedShadow;
  protected bool m_ignoreGameEntity;
  protected bool m_shown = true;
  protected bool m_shadowVisible;
  protected ActorStateMgr m_actorStateMgr;
  protected ActorStateType m_actorState = ActorStateType.CARD_IDLE;
  protected bool forceIdleState;
  protected GameObject m_rootObject;
  protected GameObject m_bones;
  protected MeshRenderer m_meshRenderer;
  protected MeshRenderer m_meshRendererPortrait;
  protected int m_legacyPortraitMaterialIndex = -1;
  protected int m_legacyCardColorMaterialIndex = -1;
  protected Material m_initialPortraitMaterial;
  protected Material m_initialPremiumRibbonMaterial;
  protected Material m_initialCardBackMaterial;
  protected SpellTable m_sharedSpellTable;
  protected bool m_useSharedSpellTable;
  protected Dictionary<SpellType, Spell> m_ownedSpells;
  protected SpellTable m_localSpellTable;
  protected ArmorSpell m_armorSpell;
  protected GameObject m_hiddenCardStandIn;
  protected bool m_shadowform;
  protected GhostCard.Type m_ghostCard;
  protected TAG_PREMIUM m_ghostPremium;
  protected bool m_missingcard;
  protected bool m_armorSpellLoading;
  protected bool m_materialEffectsSeeded;
  protected Player.Side? m_cardBackSideOverride;
  protected CardBackManager.CardBackSlot? m_cardBackSlotOverride;
  private string m_cardDefPowerTextOverride;
  protected bool m_ignoreUpdateCardback;
  protected bool isPortraitMaterialDirty;
  protected Texture m_portraitTextureOverride;
  protected bool m_blockTextComponentUpdate;
  protected bool m_armorSpellDisabledForTransition;
  protected MultiClassBannerTransition m_multiClassBanner;
  protected TradeableBanner m_tradeableBanner;
  protected UberShaderController m_uberShaderController;
  protected bool m_ignoreHideStats;
  protected TAG_CARD_SET m_watermarkCardSetOverride;
  protected bool m_useShortName;
  protected GameObject m_bannedRibbon;
  protected bool m_useBGQuestSiloutte;
  protected List<Actor.ContactShadowData> m_contactShadows;
  private bool m_shadowObjectInitialized;
  private int m_initialMissingCardRenderQueue;
  private bool m_usesMultiClassBanner;
  private bool m_isDiamondViewer;
  private GameObject m_diamondModelObject;
  private DiamondRenderToTexture m_diamondRenderToTexture;
  private string m_diamondModelShown;
  private bool m_portraitMeshDirty = true;
  private CustomFrameController m_customFrameController;
  private float m_cachedProjectedShadowAutoDisableHeight;
  private CancellationTokenSource m_updateTokenSource = new CancellationTokenSource();
  private AssetHandle<Texture> m_watermarkTex;
  protected AssetHandle<Texture> m_cardColorTex;
  private IGraphicsManager m_graphicsManager;
  [CustomEditField(Hide = true)]
  public Action DiamondCardArtUpdatedCallback;
  [CustomEditField(Hide = true)]
  public Action OnSetCard;
  [CustomEditField(Hide = true)]
  public Action OnPortraitMaterialUpdated;
  private Actor.PortraitMode m_portraitMode;
  private static readonly float descriptionMesh_WithoutRace_TextureOffset = 0.07f;
  private static readonly float descriptionMesh_WithRace_TextureOffset = 0.0f;

  public bool UseBGQuestSiloutte() => this.m_useBGQuestSiloutte;

  public void SetUseBGQuestSiloutte(bool value) => this.m_useBGQuestSiloutte = value;

  public ILegendaryHeroPortrait LegendaryHeroPortrait { get; private set; }

  private event Actor.CustomFrameChangedEventHandler OnCustomFrameChanged;

  public float ZoneHeroPositionOffset { get; private set; }

  public virtual void Awake()
  {
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.AssignRootObject();
    this.AssignBones();
    this.AssignMeshRenderers();
    this.AssignSpells();
    this.SetUpBanner();
  }

  private void OnEnable()
  {
    if (!this.isPortraitMaterialDirty)
      return;
    this.UpdateAllComponents();
  }

  private void Start() => this.Init();

  private void OnDestroy()
  {
    if (CardBackManager.Get() != null)
      CardBackManager.Get().UnregisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateCardBack));
    this.ReleaseSpells();
    this.ReleaseCardDef();
    if ((bool) (UnityEngine.Object) this.m_diamondPortraitR2T)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_diamondPortraitR2T);
    if ((bool) (UnityEngine.Object) this.m_uberShaderController)
      this.m_uberShaderController.UberShaderAnimation = (UberShaderAnimation) null;
    this.m_updateTokenSource?.Cancel();
    this.m_updateTokenSource?.Dispose();
    this.DestroyCreatedMaterials();
    this.DestroyLegendaryHeroPortrait();
    this.DestroyCustomFrame();
    AssetHandle.SafeDispose<Texture>(ref this.m_watermarkTex);
    AssetHandle.SafeDispose<Texture>(ref this.m_cardColorTex);
  }

  public void Init()
  {
    if (CardBackManager.Get() != null)
      CardBackManager.Get().RegisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateCardBack));
    if ((UnityEngine.Object) this.m_portraitMesh != (UnityEngine.Object) null && this.m_portraitMatIdx >= 0)
      this.m_initialPortraitMaterial = RendererExtension.GetSharedMaterial(this.m_portraitMesh.GetComponent<Renderer>(), this.m_portraitMatIdx);
    else if (this.m_legacyPortraitMaterialIndex >= 0)
      this.m_initialPortraitMaterial = RendererExtension.GetSharedMaterial((Renderer) this.m_meshRenderer, this.m_legacyPortraitMaterialIndex);
    if ((UnityEngine.Object) this.m_cardMesh != (UnityEngine.Object) null && this.m_cardBackMatIdx >= 0)
      this.m_initialCardBackMaterial = RendererExtension.GetSharedMaterial(this.m_cardMesh.GetComponent<Renderer>(), this.m_cardBackMatIdx);
    if (this.m_premiumRibbon > -1)
      this.m_initialPremiumRibbonMaterial = RendererExtension.GetMaterial(this.m_cardMesh.GetComponent<Renderer>(), this.m_premiumRibbon);
    if ((UnityEngine.Object) this.m_rootObject != (UnityEngine.Object) null)
      TransformUtil.Identity((Component) this.m_rootObject.transform);
    if ((UnityEngine.Object) this.m_actorStateMgr != (UnityEngine.Object) null)
      this.m_actorStateMgr.ChangeState(this.m_actorState);
    this.m_projectedShadow = this.GetComponent<ProjectedShadow>();
    if ((UnityEngine.Object) this.m_projectedShadow != (UnityEngine.Object) null)
      this.m_cachedProjectedShadowAutoDisableHeight = this.m_projectedShadow.m_AutoDisableHeight;
    if (this.m_shown)
      this.ShowImpl(false);
    else
      this.HideImpl(false);
  }

  public void Destroy()
  {
    if (!(bool) (UnityEngine.Object) this.gameObject)
      return;
    this.ReleaseSpells();
    this.ReleaseCardDef();
    this.DestroyCreatedMaterials();
    this.DestroyLegendaryHeroPortrait();
    this.DestroyCustomFrame();
    if (!Application.IsPlaying((UnityEngine.Object) this))
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.gameObject);
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void ReleaseSpells()
  {
    SpellManager spellManager = SpellManager.Get();
    if (spellManager == null)
      return;
    List<Spell> spellList = new List<Spell>();
    if (this.m_ownedSpells != null)
    {
      foreach (Spell spell in this.m_ownedSpells.Values)
      {
        if (!((UnityEngine.Object) spell == (UnityEngine.Object) null))
        {
          spell.RemoveSpellReleasedCallback(new Spell.SpellReleasedCallback(this.OnSpellRelease));
          spellList.Add(spell);
        }
      }
      this.m_ownedSpells.Clear();
    }
    for (int index = spellList.Count - 1; index >= 0; --index)
      spellManager.ReleaseSpell(spellList[index]);
    spellList.Clear();
    if (!((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null))
      return;
    for (int index = this.m_localSpellTable.m_Table.Count - 1; index >= 0; --index)
    {
      Spell spell = this.m_localSpellTable.m_Table[index].m_Spell;
      if (!((UnityEngine.Object) spell == (UnityEngine.Object) null))
        spellManager.ReleaseSpell(spell);
    }
  }

  private void DestroyCreatedMaterials()
  {
    if (!((UnityEngine.Object) this.m_initialPremiumRibbonMaterial != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_initialPremiumRibbonMaterial);
    this.m_initialPremiumRibbonMaterial = (Material) null;
  }

  private void DestroyLegendaryHeroPortrait()
  {
    if (this.LegendaryHeroPortrait == null)
      return;
    this.LegendaryHeroPortrait.Dispose();
    this.LegendaryHeroPortrait = (ILegendaryHeroPortrait) null;
  }

  public virtual Actor Clone()
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameObject, this.transform.position, this.transform.rotation);
    Actor component = gameObject.GetComponent<Actor>();
    component.SetEntity(this.m_entity);
    component.SetEntityDef(this.m_entityDef);
    component.SetCard(this.m_card);
    component.SetPremium(this.m_premiumType);
    component.SetWatermarkCardSetOverride(this.m_watermarkCardSetOverride);
    gameObject.transform.localScale = this.gameObject.transform.localScale;
    gameObject.transform.position = this.gameObject.transform.position;
    component.SetActorState(this.m_actorState);
    if (this.m_shown)
      component.ShowImpl(false);
    else
      component.HideImpl(false);
    return component;
  }

  public Card GetCard() => this.m_card;

  public void SetCard(Card card)
  {
    if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) card)
      return;
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
    {
      this.m_card = (Card) null;
      this.transform.parent = (Transform) null;
      Action onSetCard = this.OnSetCard;
      if (onSetCard == null)
        return;
      onSetCard();
    }
    else
    {
      this.m_card = card;
      Action onSetCard = this.OnSetCard;
      if (onSetCard != null)
        onSetCard();
      this.transform.parent = card.transform;
      TransformUtil.Identity((Component) this.transform);
      if (!((UnityEngine.Object) this.m_rootObject != (UnityEngine.Object) null))
        return;
      TransformUtil.Identity((Component) this.m_rootObject.transform);
    }
  }

  public DiamondRenderToTexture GetDiamondRenderToTexture() => this.m_diamondRenderToTexture;

  public void SetDiamondRenderToTexture(DiamondRenderToTexture diamondToRenderTexture) => this.m_diamondRenderToTexture = diamondToRenderTexture;

  public string GetDiamondModelShown() => this.m_diamondModelShown;

  public void SetDiamondModelShown(string diamondModelShown) => this.m_diamondModelShown = diamondModelShown;

  public GameObject GetDiamondModelObject() => this.m_diamondModelObject;

  public void SetDiamondModelObject(GameObject diamondModelObject) => this.m_diamondModelObject = diamondModelObject;

  public bool GetPortraitMeshDirty() => this.m_portraitMeshDirty;

  public void SetPortraitMeshDirty(bool portraitMeshDirty) => this.m_portraitMeshDirty = portraitMeshDirty;

  public void SetFullDefFromEntity(Entity entity)
  {
    if (entity == null)
      return;
    this.SetEntityDef(entity.GetEntityDef());
    this.SetCardDefFromEntity(entity);
  }

  public void SetFullDefFromActor(Actor other)
  {
    if (!((UnityEngine.Object) other != (UnityEngine.Object) null))
      return;
    this.SetCardDefFromActor(other);
    this.SetEntityDef(other.m_entityDef);
  }

  public void SetFullDef(DefLoader.DisposableFullDef fullDef)
  {
    this.SetCardDef(fullDef.DisposableCardDef);
    this.SetEntityDef(fullDef.EntityDef);
  }

  public DefLoader.DisposableCardDef ShareDisposableCardDef() => this.m_cardDefHandle.Share();

  public void SetCardDefFromEntity(Entity entity)
  {
    if (entity == null)
      return;
    using (DefLoader.DisposableCardDef cardDef = entity.ShareDisposableCardDef())
      this.SetCardDef(cardDef);
  }

  public void SetCardDefFromActor(Actor other)
  {
    if (!((UnityEngine.Object) other != (UnityEngine.Object) null))
      return;
    this.m_cardDefHandle.Set(other.m_cardDefHandle);
  }

  public void SetCardDefFromCard(Card card)
  {
    if (!((UnityEngine.Object) card != (UnityEngine.Object) null))
      return;
    using (DefLoader.DisposableCardDef def = card.ShareDisposableCardDef())
    {
      if (!this.m_cardDefHandle.SetCardDef(def))
        return;
      this.LoadArmorSpell();
    }
  }

  public void SetCardDef(DefLoader.DisposableCardDef cardDef)
  {
    if (!this.m_cardDefHandle.SetCardDef(cardDef))
      return;
    this.LoadArmorSpell();
    this.TryLoadLegendaryArt(cardDef);
  }

  private bool TryLoadLegendaryArt(DefLoader.DisposableCardDef cardDef)
  {
    if (cardDef == null)
      return false;
    this.LoadCustomFrame(cardDef.CardDef);
    Entity entity = this.m_entity;
    Player.Side side = entity != null ? entity.GetControllerSide() : Player.Side.NEUTRAL;
    this.UpdateLegendaryCardArt(cardDef.CardDef, side);
    return true;
  }

  public void ReleaseCardDef() => this.m_cardDefHandle.ReleaseCardDef();

  public void SetIgnoreHideStats(bool ignore) => this.m_ignoreHideStats = ignore;

  private bool HasHideStats(EntityBase entity)
  {
    if (this.m_ignoreHideStats)
      return false;
    return entity.HasTag(GAME_TAG.HIDE_STATS) || entity.IsDormant();
  }

  public void SetWatermarkCardSetOverride(TAG_CARD_SET cardSetOverride)
  {
    if (!Enum.IsDefined(typeof (TAG_CARD_SET), (object) cardSetOverride))
      this.m_watermarkCardSetOverride = TAG_CARD_SET.INVALID;
    else
      this.m_watermarkCardSetOverride = cardSetOverride;
  }

  public Entity GetEntity() => this.m_entity;

  public void SetEntity(Entity entity)
  {
    this.m_entity = entity;
    if (this.m_entity == null)
      return;
    this.SetPremium(this.m_entity.GetPremiumType());
    this.SetWatermarkCardSetOverride(this.m_entity.GetWatermarkCardSetOverride());
  }

  public EntityDef GetEntityDef() => this.m_entityDef;

  public void SetEntityDef(EntityDef entityDef)
  {
    this.m_entityDef = entityDef;
    if (this.m_entityDef == null)
      return;
    string cardId = this.m_entityDef.GetCardId();
    this.m_cardDefHandle.SetCardId(cardId);
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get()?.GetCardDef(cardId))
      this.SetCardDef(cardDef);
  }

  public virtual void SetPremium(TAG_PREMIUM premium) => this.m_premiumType = premium;

  public TAG_PREMIUM GetPremium() => this.m_premiumType;

  public TAG_CARD_SET GetCardSet() => this.m_entityDef == null && this.m_entity == null ? TAG_CARD_SET.NONE : (this.m_entityDef == null ? this.m_entity.GetCardSet() : this.m_entityDef.GetCardSet());

  public ActorStateType GetActorStateType() => !((UnityEngine.Object) this.m_actorStateMgr == (UnityEngine.Object) null) ? this.m_actorStateMgr.GetActiveStateType() : ActorStateType.NONE;

  public void SetActorState(ActorStateType stateType)
  {
    this.m_actorState = stateType;
    if ((UnityEngine.Object) this.m_actorStateMgr == (UnityEngine.Object) null)
      return;
    if (this.forceIdleState)
      this.m_actorState = ActorStateType.CARD_IDLE;
    this.m_actorStateMgr.ChangeState(this.m_actorState);
  }

  public void ToggleForceIdle(bool bOn) => this.forceIdleState = bOn;

  public void TurnOffCollider() => this.ToggleCollider(false);

  public void TurnOnCollider() => this.ToggleCollider(true);

  public void ToggleCollider(bool enabled)
  {
    MeshRenderer meshRenderer = this.GetMeshRenderer();
    Collider component;
    if ((UnityEngine.Object) meshRenderer == (UnityEngine.Object) null || !meshRenderer.TryGetComponent<Collider>(out component))
      return;
    component.enabled = enabled;
  }

  public bool IsColliderEnabled()
  {
    MeshRenderer meshRenderer = this.GetMeshRenderer();
    Collider component;
    return !((UnityEngine.Object) meshRenderer == (UnityEngine.Object) null) && meshRenderer.TryGetComponent<Collider>(out component) && component.enabled;
  }

  public TAG_RARITY GetRarity()
  {
    if (this.m_entityDef != null)
      return this.m_entityDef.GetRarity();
    return this.m_entity != null ? this.m_entity.GetRarity() : TAG_RARITY.FREE;
  }

  public bool IsElite()
  {
    if (this.IsLettuceMercenary() || this.IsLettuceAbility())
      return false;
    if (this.m_entityDef != null)
      return this.m_entityDef.IsElite();
    if (this.m_entity != null)
      return this.m_entity.IsElite();
    return this.m_isDiamondViewer;
  }

  public bool IsLettuceMercenary()
  {
    if (this.m_entityDef != null)
      return this.m_entityDef.IsLettuceMercenary();
    return this.m_entity != null && this.m_entity.IsLettuceMercenary();
  }

  public bool IsLettuceAbility()
  {
    if (this.m_entityDef != null)
      return this.m_entityDef.IsLettuceAbility();
    return this.m_entity != null && this.m_entity.IsLettuceAbility();
  }

  public bool IsMultiClass()
  {
    if (this.m_entityDef != null)
      return this.m_entityDef.IsMultiClass();
    return this.m_entity != null && this.m_entity.IsMultiClass();
  }

  public bool IsTradeable()
  {
    if (this.m_entityDef != null)
      return this.m_entityDef.IsTradeable();
    return this.m_entity != null && this.m_entity.IsTradeable();
  }

  public bool IsLocation()
  {
    if (this.m_entityDef != null)
      return this.m_entityDef.IsLocation();
    return this.m_entity != null && this.m_entity.IsLocation();
  }

  public bool HasRuneCost()
  {
    if (this.m_entityDef != null)
      return this.m_entityDef.HasRuneCost;
    return this.m_entity != null && this.m_entity.HasRuneCost;
  }

  public void SetHiddenStandIn(GameObject standIn) => this.m_hiddenCardStandIn = standIn;

  public GameObject GetHiddenStandIn() => this.m_hiddenCardStandIn;

  public void SetShadowform(bool shadowform) => this.m_shadowform = shadowform;

  public UberShaderController GetUberShaderController()
  {
    if ((UnityEngine.Object) this.m_uberShaderController == (UnityEngine.Object) null)
      this.m_uberShaderController = this.m_portraitMesh.GetComponent<UberShaderController>();
    return this.m_uberShaderController;
  }

  public bool UsesMultiClassBanner() => this.m_usesMultiClassBanner;

  public void SetIgnoreGameEntity(bool ignore) => this.m_ignoreGameEntity = ignore;

  protected GameEntity GetGameEntityIfAllowed()
  {
    if (this.m_ignoreGameEntity)
      return (GameEntity) null;
    return GameState.Get()?.GetGameEntity();
  }

  public bool IsDesiredHidden { get; private set; }

  public bool IsDesiredHiddenInHierarchy
  {
    get
    {
      if (this.IsDesiredHidden)
        return true;
      WidgetTemplate componentInParent = this.GetComponentInParent<WidgetTemplate>();
      return (UnityEngine.Object) componentInParent != (UnityEngine.Object) null && componentInParent.IsDesiredHiddenInHierarchy;
    }
  }

  public bool HandlesChildVisibility => true;

  public void SetVisibility(bool isVisible, bool isInternal) => this.SetVisibility(isVisible, false, isInternal);

  protected void SetVisibility(bool isVisible, bool ignoreSpells, bool isInternal)
  {
    if (isVisible == this.m_shown)
      return;
    if (!isInternal)
      this.IsDesiredHidden = !isVisible;
    this.m_shown = isVisible;
    if (isVisible)
      this.ShowImpl(ignoreSpells);
    else
      this.HideImpl(ignoreSpells);
  }

  public bool IsShown() => this.m_shown;

  public void Show() => this.SetVisibility(true, false, false);

  public void Show(bool ignoreSpells) => this.SetVisibility(true, ignoreSpells, false);

  public void ShowSpellTable()
  {
    if (this.m_ownedSpells != null)
    {
      foreach (Spell spell in this.m_ownedSpells.Values)
      {
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
          spell.Show();
      }
    }
    if (!((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null))
      return;
    this.m_localSpellTable.Show();
  }

  public void Hide() => this.SetVisibility(false, false, false);

  public void Hide(bool ignoreSpells) => this.SetVisibility(false, ignoreSpells, false);

  public void HideSpellTable()
  {
    if (this.m_ownedSpells != null)
    {
      foreach (Spell spell in this.m_ownedSpells.Values)
      {
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null && spell.GetSpellType() != SpellType.NONE)
          spell.Hide();
      }
    }
    if (!((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null))
      return;
    this.m_localSpellTable.Hide();
  }

  protected virtual void ShowImpl(bool ignoreSpells)
  {
    if ((UnityEngine.Object) this.m_rootObject != (UnityEngine.Object) null)
      this.m_rootObject.SetActive(true);
    if ((bool) (UnityEngine.Object) this.m_diamondRenderToTexture)
      this.m_diamondRenderToTexture.enabled = true;
    this.ShowArmorSpell();
    this.UpdateAllComponents();
    if ((bool) (UnityEngine.Object) this.m_projectedShadow)
      this.m_projectedShadow.enabled = true;
    if ((UnityEngine.Object) this.m_actorStateMgr != (UnityEngine.Object) null)
      this.m_actorStateMgr.ShowStateMgr();
    if (!ignoreSpells)
      this.ShowSpellTable();
    if ((UnityEngine.Object) this.m_ghostCardGameObject != (UnityEngine.Object) null)
      this.m_ghostCardGameObject.SetActive(true);
    HighlightState componentInChildren = this.GetComponentInChildren<HighlightState>();
    if (!(bool) (UnityEngine.Object) componentInChildren)
      return;
    componentInChildren.Show();
  }

  protected virtual void HideImpl(bool ignoreSpells)
  {
    if ((UnityEngine.Object) this.m_rootObject != (UnityEngine.Object) null)
      this.m_rootObject.SetActive(false);
    this.UpdateContactShadow();
    this.HideArmorSpell();
    if ((UnityEngine.Object) this.m_actorStateMgr != (UnityEngine.Object) null)
      this.m_actorStateMgr.HideStateMgr();
    if ((bool) (UnityEngine.Object) this.m_projectedShadow)
      this.m_projectedShadow.enabled = false;
    if ((UnityEngine.Object) this.m_ghostCardGameObject != (UnityEngine.Object) null)
      this.m_ghostCardGameObject.SetActive(false);
    if (!ignoreSpells)
      this.HideSpellTable();
    if ((UnityEngine.Object) this.m_missingCardEffect != (UnityEngine.Object) null)
      this.UpdateMissingCardArt();
    if ((bool) (UnityEngine.Object) this.m_diamondRenderToTexture)
      this.m_diamondRenderToTexture.enabled = false;
    HighlightState componentInChildren = this.GetComponentInChildren<HighlightState>();
    if (!(bool) (UnityEngine.Object) componentInChildren)
      return;
    componentInChildren.Hide();
  }

  public ActorStateMgr GetActorStateMgr() => this.m_actorStateMgr;

  public Collider GetCollider() => (UnityEngine.Object) this.GetMeshRenderer() == (UnityEngine.Object) null ? (Collider) null : this.GetMeshRenderer().gameObject.GetComponent<Collider>();

  public GameObject GetRootObject() => this.m_rootObject;

  public MeshRenderer GetMeshRenderer(bool getPortrait = false) => this.m_premiumType == TAG_PREMIUM.DIAMOND && getPortrait ? this.m_meshRendererPortrait : this.m_meshRenderer;

  public GameObject GetBones() => this.m_bones;

  public UberText GetPowersText() => this.m_powersTextMesh;

  public UberText GetBGQuestPowersText() => this.m_bgQuestPowerTextMesh;

  public UberText GetBGQuestRaceText() => this.m_bgQuestRaceTextMesh;

  public UberText GetRaceText() => this.m_raceTextMesh;

  public UberText GetNameText() => this.m_nameTextMesh;

  public Light GetHeroSpotlight() => (UnityEngine.Object) this.m_heroSpotLight == (UnityEngine.Object) null ? (Light) null : this.m_heroSpotLight.GetComponent<Light>();

  public GameObject FindBone(string boneName) => (UnityEngine.Object) this.m_bones == (UnityEngine.Object) null ? (GameObject) null : GameObjectUtils.FindChildBySubstring(this.m_bones, boneName);

  public GameObject GetCardTypeBannerAnchor() => (UnityEngine.Object) this.m_cardTypeAnchorObject == (UnityEngine.Object) null ? this.gameObject : this.m_cardTypeAnchorObject;

  public UberText GetAttackText() => this.m_attackTextMesh;

  public GameObject GetAttackTextObject() => (UnityEngine.Object) this.m_attackTextMesh == (UnityEngine.Object) null ? (GameObject) null : this.m_attackTextMesh.gameObject;

  public GemObject GetAttackObject() => (UnityEngine.Object) this.m_attackObject == (UnityEngine.Object) null ? (GemObject) null : this.m_attackObject.GetComponent<GemObject>();

  public GemObject GetHealthObject() => (UnityEngine.Object) this.m_healthObject == (UnityEngine.Object) null ? (GemObject) null : this.m_healthObject.GetComponent<GemObject>();

  public Widget GetAmountBannerWidget() => this.m_amountBannerWidget;

  public GameObject GetWeaponShields() => (UnityEngine.Object) this.m_healthObject != (UnityEngine.Object) null && (UnityEngine.Object) this.m_healthObject.GetComponent<GemObject>() == (UnityEngine.Object) null ? this.m_healthObject : (GameObject) null;

  public GameObject GetWeaponSwords() => (UnityEngine.Object) this.m_attackObject != (UnityEngine.Object) null && (UnityEngine.Object) this.m_attackObject.GetComponent<GemObject>() == (UnityEngine.Object) null ? this.m_attackObject : (GameObject) null;

  public GemObject GetArmorObject() => (UnityEngine.Object) this.m_armorObject == (UnityEngine.Object) null ? (GemObject) null : this.m_armorObject.GetComponent<GemObject>();

  public UberText GetHealthText() => this.m_healthTextMesh;

  public GameObject GetHealthTextObject() => (UnityEngine.Object) this.m_healthTextMesh == (UnityEngine.Object) null ? (GameObject) null : this.m_healthTextMesh.gameObject;

  public UberText GetCostText() => (UnityEngine.Object) this.m_costTextMesh == (UnityEngine.Object) null ? (UberText) null : this.m_costTextMesh;

  public GameObject GetCostTextObject() => (UnityEngine.Object) this.m_costTextMesh == (UnityEngine.Object) null ? (GameObject) null : this.m_costTextMesh.gameObject;

  public UberText GetSecretText() => this.m_secretText;

  public virtual void UpdateAllComponents(bool needsGhostUpdate = true)
  {
    if (!this.m_isDiamondViewer)
    {
      this.UpdateTextComponents();
      this.UpdateMaterials();
      this.UpdateTextures();
      this.UpdateCardBack();
      this.UpdateMeshComponents();
      this.UpdateRootObjectSpellComponents();
      this.UpdateMissingCardArt();
      if (needsGhostUpdate)
        this.UpdateGhostCardEffect();
      this.UpdateDiamondCardArt();
      Entity entity = this.m_entity;
      Player.Side side = entity != null ? entity.GetControllerSide() : Player.Side.NEUTRAL;
      this.UpdateLegendaryCardArt(this.m_cardDefHandle.Get(this.m_premiumType), side);
      this.UpdatePortraitMaterialAnimation();
      this.UpdateContactShadow();
      this.UpdateLettuceMinionInPlayFrame();
    }
    if (PlatformSettings.OS != OSCategory.Mac || !(bool) (UnityEngine.Object) this.m_nameTextMesh)
      return;
    this.DelayedUpdateNameText(this.m_updateTokenSource.Token).Forget();
  }

  private async UniTaskVoid DelayedUpdateNameText(CancellationToken token)
  {
    await UniTask.Yield(PlayerLoopTiming.Update, token);
    if (!(bool) (UnityEngine.Object) this.m_nameTextMesh)
      return;
    this.m_nameTextMesh.UpdateNow();
  }

  public void UpdatePortraitFrameVisibility(bool visible)
  {
    if (this.m_portraitFrameObject.activeSelf == visible)
      return;
    this.m_portraitFrameObject.SetActive(visible);
    this.UpdateAllComponents();
  }

  public bool MissingCardEffect(bool refreshOnFocus = true, bool updateComponents = true)
  {
    if ((bool) (UnityEngine.Object) this.m_missingCardEffect)
    {
      RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
      if ((bool) (UnityEngine.Object) component)
      {
        component.DontRefreshOnFocus = !refreshOnFocus;
        this.m_initialMissingCardRenderQueue = component.m_RenderQueue;
        this.m_missingcard = true;
        if (updateComponents)
          this.UpdateAllComponents();
        return true;
      }
    }
    return false;
  }

  public void DisableMissingCardEffect()
  {
    this.m_missingcard = false;
    if (!(bool) (UnityEngine.Object) this.m_missingCardEffect)
      return;
    RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
    if ((bool) (UnityEngine.Object) component)
      component.enabled = false;
    this.UpdateAllComponents();
    this.MaterialShaderAnimation(true);
  }

  public void UpdateMissingCardArt()
  {
    if (!this.m_missingcard || (UnityEngine.Object) this.m_missingCardEffect == (UnityEngine.Object) null)
      return;
    RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    if (this.m_rootObject.activeSelf)
    {
      this.MaterialShaderAnimation(false);
      if (SceneMgr.Get() != null && SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
      {
        TAG_PREMIUM premium = this.GetPremium();
        bool flag = CollectionManager.Get().GetThemeShowing() == FormatType.FT_WILD;
        if (premium == TAG_PREMIUM.GOLDEN)
          component.m_Material.color = !flag ? this.MISSING_CARD_STANDARD_GOLDEN_COLOR : this.MISSING_CARD_WILD_GOLDEN_COLOR;
        else if (premium == TAG_PREMIUM.DIAMOND & flag)
        {
          Material material = component.m_Material;
          material.color = this.MISSING_CARD_WILD_DIAMOND_COLOR;
          material.SetFloat(this.MISSING_CARD_WILD_DIAMOND_CONTRAST_KEY, this.MISSING_CARD_WILD_DIAMOND_CONTRAST);
          material.SetFloat(this.MISSING_CARD_WILD_DIAMOND_INTENSITY_KEY, this.MISSING_CARD_WILD_DIAMOND_INTENSITY);
        }
      }
      component.enabled = true;
      component.Show(true);
    }
    else
    {
      component.enabled = false;
      component.Hide();
    }
  }

  public void SetMissingCardMaterial(Material missingCardMat)
  {
    if ((UnityEngine.Object) this.m_missingCardEffect == (UnityEngine.Object) null || (UnityEngine.Object) missingCardMat == (UnityEngine.Object) null)
      return;
    RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.m_Material = missingCardMat;
    if (!this.m_rootObject.activeSelf)
      return;
    this.MaterialShaderAnimation(false);
    if (!component.enabled)
      return;
    component.Render();
  }

  public bool isMissingCard()
  {
    if ((UnityEngine.Object) this.m_missingCardEffect == (UnityEngine.Object) null)
      return false;
    RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
    return !((UnityEngine.Object) component == (UnityEngine.Object) null) && component.enabled;
  }

  public void SetMissingCardRenderQueue(bool reset, int renderQueue)
  {
    RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.m_RenderQueue = reset ? this.m_initialMissingCardRenderQueue : renderQueue;
  }

  public void GhostCardEffect(GhostCard.Type ghostType, TAG_PREMIUM premium = TAG_PREMIUM.NORMAL, bool update = true)
  {
    if (this.m_ghostCard == ghostType && this.m_ghostPremium == premium)
      return;
    this.m_ghostCard = ghostType;
    this.m_ghostPremium = premium;
    if (!update)
      return;
    this.UpdateAllComponents();
  }

  private void UpdateGhostCardEffect(bool RTTUpdateOnly = false)
  {
    if ((UnityEngine.Object) this.m_ghostCardGameObject == (UnityEngine.Object) null)
      return;
    GhostCard component = this.m_ghostCardGameObject.GetComponent<GhostCard>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    if (this.m_ghostCard != GhostCard.Type.NONE)
    {
      if (RTTUpdateOnly)
      {
        component.SetRTTDirty();
      }
      else
      {
        component.SetGhostType(this.m_ghostCard);
        component.SetPremium(this.m_ghostPremium);
        component.RenderGhostCard();
      }
    }
    else
      component.DisableGhost();
  }

  public bool isGhostCard() => this.m_ghostCard != GhostCard.Type.NONE && (bool) (UnityEngine.Object) this.m_ghostCardGameObject;

  public bool DoesDiamondModelExistOnCardDef()
  {
    CardDef cardDef = this.m_cardDefHandle.Get(this.m_premiumType);
    return !((UnityEngine.Object) cardDef == (UnityEngine.Object) null) && !string.IsNullOrEmpty(cardDef.m_DiamondModel);
  }

  public bool IsEntityStateBadForDiamondVisuals()
  {
    if (GameState.Get() != null && !GameState.Get().AllowDiamondCards())
      return true;
    this.GetEntity();
    if (this.m_entity == null)
      return false;
    int num1 = this.m_entity.HasTag(GAME_TAG.FROZEN) ? 1 : 0;
    bool flag1 = this.m_entity.HasTag(GAME_TAG.REBORN);
    bool flag2 = this.m_entity.HasTag(GAME_TAG.STEALTH);
    bool flag3 = this.m_entity.HasTag(GAME_TAG.DORMANT);
    bool flag4 = this.m_entity.HasTag(GAME_TAG.ENRAGED);
    bool flag5 = this.m_entity.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_SPELLS) && this.m_entity.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_HERO_POWERS);
    bool flag6 = this.m_entity.HasTag(GAME_TAG.IS_VAMPIRE);
    Card card = this.GetCard();
    if ((UnityEngine.Object) card != (UnityEngine.Object) null)
    {
      Spell actorSpell = card.GetActorSpell(SpellType.DORMANT, false);
      if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null && actorSpell.GetActiveState() != SpellStateType.NONE)
        flag3 = true;
    }
    bool flag7 = false;
    if ((UnityEngine.Object) this.m_card != (UnityEngine.Object) null && this.m_card.GetZone() is ZoneGraveyard)
      flag7 = true;
    int num2 = flag1 ? 1 : 0;
    return (num1 | num2 | (flag2 ? 1 : 0) | (flag3 ? 1 : 0) | (flag4 ? 1 : 0) | (flag5 ? 1 : 0) | (flag7 ? 1 : 0) | (flag6 ? 1 : 0)) != 0;
  }

  public void LoadDiamondCardMesh(GameObject goMeshRTT, AssetReference planeRef)
  {
    if (planeRef == null)
      return;
    MeshFilter component = goMeshRTT.GetComponent<MeshFilter>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || planeRef == null)
      return;
    using (AssetHandle<Mesh> assetHandle = AssetLoader.Get().LoadAsset<Mesh>(planeRef))
    {
      if (assetHandle == null)
        return;
      component.sharedMesh = (Mesh) assetHandle;
    }
  }

  public void UpdateDiamondCardArt()
  {
    if (this.m_premiumType != TAG_PREMIUM.DIAMOND)
      return;
    if ((UnityEngine.Object) this.m_portraitMesh != (UnityEngine.Object) null && (UnityEngine.Object) this.m_portraitMeshRTT != (UnityEngine.Object) null)
    {
      int num = this.IsEntityStateBadForDiamondVisuals() ? 1 : 0;
      bool flag = this.DoesDiamondModelExistOnCardDef();
      if (num != 0 || !flag)
      {
        this.m_portraitMesh.SetActive(true);
        this.m_portraitMeshRTT.SetActive(false);
      }
      else
      {
        this.m_portraitMesh.SetActive(false);
        this.m_portraitMeshRTT.SetActive(true);
      }
    }
    if ((UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType) == (UnityEngine.Object) null)
      return;
    if (this.DoesDiamondModelExistOnCardDef() && (UnityEngine.Object) this.m_rootObject != (UnityEngine.Object) null)
    {
      bool flag = (UnityEngine.Object) this.m_diamondModelObject != (UnityEngine.Object) null;
      string diamondModel = this.m_cardDefHandle.Get(this.m_premiumType).m_DiamondModel;
      if ((bool) (UnityEngine.Object) this.m_diamondPortraitR2T && !(bool) (UnityEngine.Object) this.m_diamondRenderToTexture)
        this.m_diamondRenderToTexture = this.m_diamondPortraitR2T.GetComponent<DiamondRenderToTexture>();
      if (flag && diamondModel != this.m_diamondModelShown)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_diamondModelObject);
        this.m_diamondModelObject = (GameObject) null;
        flag = false;
        if ((bool) (UnityEngine.Object) this.m_diamondRenderToTexture)
          this.m_diamondRenderToTexture.enabled = false;
      }
      if (!flag)
      {
        this.m_diamondModelObject = AssetLoader.Get().InstantiatePrefab((AssetReference) diamondModel, AssetLoadingOptions.IgnorePrefabPosition);
        this.m_diamondModelShown = diamondModel;
        this.m_diamondModelObject.transform.parent = this.m_rootObject.transform;
        if ((bool) (UnityEngine.Object) this.m_diamondRenderToTexture)
        {
          this.m_diamondRenderToTexture.m_ObjectToRender = this.m_diamondModelObject;
          this.m_diamondRenderToTexture.m_ClearColor = this.m_cardDefHandle.Get(this.m_premiumType).m_DiamondPlaneRTT_CearColor;
        }
        this.m_portraitMeshDirty = true;
      }
      else if ((bool) (UnityEngine.Object) this.m_diamondRenderToTexture)
        this.m_diamondRenderToTexture.UpdateMaterialBlend(this.m_usePlayPortrait);
      else
        this.m_diamondModelObject.SetActive(false);
    }
    if (this.m_portraitMeshDirty && (UnityEngine.Object) this.m_portraitMeshRTT != (UnityEngine.Object) null && (UnityEngine.Object) this.m_portraitMeshRTT_background != (UnityEngine.Object) null)
    {
      this.LoadDiamondCardMesh(this.m_portraitMeshRTT, this.GetDiamondPlaneRef(false));
      this.LoadDiamondCardMesh(this.m_portraitMeshRTT_background, this.GetDiamondPlaneRef(true));
      AssetReference portraitTexturePath = (AssetReference) this.m_cardDefHandle.Get(this.m_premiumType).m_DiamondPortraitTexturePath;
      Renderer component = this.m_portraitMeshRTT_background.GetComponent<Renderer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && RendererExtension.GetSharedMaterial(component).HasProperty("_MainTex") && portraitTexturePath != null)
      {
        using (AssetHandle<Texture2D> assetHandle = AssetLoader.Get().LoadAsset<Texture2D>(portraitTexturePath))
        {
          if (assetHandle != null)
            Actor.GetMaterialInstance(component).SetTexture("_MainTex", (Texture) (Texture2D) assetHandle);
        }
      }
      HighlightState componentInChildren = this.GetComponentInChildren<HighlightState>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null && componentInChildren.isActiveAndEnabled)
        componentInChildren.ContinuousUpdate(0.1f);
      this.m_portraitMeshDirty = false;
    }
    if ((bool) (UnityEngine.Object) this.m_diamondRenderToTexture)
      this.m_diamondRenderToTexture.enabled = this.m_shown;
    if (!this.DoesDiamondModelExistOnCardDef() && (UnityEngine.Object) this.m_diamondModelObject != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_diamondModelObject);
      this.m_diamondModelObject = (GameObject) null;
    }
    if ((UnityEngine.Object) this.m_diamondModelObject == (UnityEngine.Object) null && (UnityEngine.Object) this.m_diamondPortraitR2T != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.m_diamondRenderToTexture && this.m_diamondRenderToTexture.enabled)
      this.m_diamondRenderToTexture.enabled = false;
    Action artUpdatedCallback = this.DiamondCardArtUpdatedCallback;
    if (artUpdatedCallback == null)
      return;
    artUpdatedCallback();
  }

  public void UpdateLegendaryCardArt(CardDef cardDef, Player.Side side)
  {
    if ((UnityEngine.Object) cardDef == (UnityEngine.Object) null)
      return;
    string assetPath = cardDef.m_LegendaryModel;
    if (this.m_missingcard || this.isGhostCard())
      assetPath = (string) null;
    if (!string.IsNullOrEmpty(assetPath))
    {
      if (this.LegendaryHeroPortrait != null && !this.LegendaryHeroPortrait.IsValidForPath(assetPath, side))
        this.DestroyLegendaryHeroPortrait();
      if (this.LegendaryHeroPortrait == null)
      {
        LegendaryHeroRenderToTextureService toTextureService = ServiceManager.Get<LegendaryHeroRenderToTextureService>();
        if (toTextureService != null)
        {
          this.LegendaryHeroPortrait = toTextureService.CreatePortrait(assetPath, side);
          this.LegendaryHeroPortrait.AttachToActor(this);
          this.m_portraitMeshDirty = true;
        }
      }
    }
    if (string.IsNullOrEmpty(assetPath) && this.LegendaryHeroPortrait != null)
      this.DestroyLegendaryHeroPortrait();
    if (!this.m_portraitMeshDirty || this.LegendaryHeroPortrait == null)
      return;
    this.UpdateMaterials(cardDef);
    HighlightState componentInChildren = this.GetComponentInChildren<HighlightState>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null && componentInChildren.isActiveAndEnabled)
      componentInChildren.ContinuousUpdate(0.1f);
    this.m_portraitMeshDirty = false;
  }

  private AssetReference GetDiamondPlayMeshName(bool background) => background ? (AssetReference) this.m_cardDefHandle.Get(this.m_premiumType).m_DiamondBackground_Play : (AssetReference) this.m_cardDefHandle.Get(this.m_premiumType).m_DiamondPlaneRTT_Play;

  private AssetReference GetDiamondHandMeshName(bool background) => background ? (AssetReference) this.m_cardDefHandle.Get(this.m_premiumType).m_DiamondBackground_Hand : (AssetReference) this.m_cardDefHandle.Get(this.m_premiumType).m_DiamondPlaneRTT_Hand;

  private AssetReference GetDiamondPlaneRef(bool background)
  {
    AssetReference diamondPlaneRef = (AssetReference) null;
    switch (this.m_portraitMode)
    {
      case Actor.PortraitMode.Default:
        diamondPlaneRef = !((UnityEngine.Object) this.m_card == (UnityEngine.Object) null) ? (this.m_usePlayPortrait ? this.GetDiamondPlayMeshName(background) : this.GetDiamondHandMeshName(background)) : this.GetDiamondHandMeshName(background);
        break;
      case Actor.PortraitMode.ForcedPlayMode:
        diamondPlaneRef = this.GetDiamondPlayMeshName(background);
        break;
      case Actor.PortraitMode.ForcedHandMode:
        diamondPlaneRef = this.GetDiamondHandMeshName(background);
        break;
    }
    return diamondPlaneRef;
  }

  public void SetDiamondPortraitMode(bool playMode, bool forced = false)
  {
    this.m_usePlayPortrait = playMode;
    if (forced)
      this.m_portraitMode = playMode ? Actor.PortraitMode.ForcedPlayMode : Actor.PortraitMode.ForcedHandMode;
    else
      this.m_portraitMode = Actor.PortraitMode.Default;
  }

  public void UpdateMaterials(CardDef cardDef = null)
  {
    if (this.gameObject.activeInHierarchy)
      this.UpdatePortraitMaterials(this.m_updateTokenSource.Token, cardDef).Forget();
    else
      this.isPortraitMaterialDirty = true;
  }

  public void OverrideAllMeshMaterials(Material material)
  {
    if ((UnityEngine.Object) this.m_rootObject == (UnityEngine.Object) null)
      return;
    this.RecursivelyReplaceMaterialsList(this.m_rootObject.transform, material);
  }

  public void SetUnlit() => this.SetLightBlend(0.0f, true);

  public void SetLit() => this.SetLightBlend(1f, true);

  public void SetLightBlend(float blendValue, bool includeInactive = false)
  {
    this.SetLightBlend(this.gameObject, blendValue, includeInactive);
    if (!((UnityEngine.Object) this.m_diamondPortraitR2T != (UnityEngine.Object) null))
      return;
    DiamondRenderToTexture component = this.m_diamondPortraitR2T.GetComponent<DiamondRenderToTexture>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.UpdateMaterialBlend(blendValue);
  }

  private void SetLightBlend(GameObject go, float blendValue, bool includeInactive = false)
  {
    foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>(includeInactive))
    {
      Renderer renderer = componentsInChild;
      if (!renderer.gameObject.activeInHierarchy)
        DeferredEnableHandler.AttachTo((Component) renderer, (Action) (() => this.SetRendererLightBlend(renderer, blendValue)));
      else
        this.SetRendererLightBlend(renderer, blendValue);
    }
    foreach (UberText componentsInChild in go.GetComponentsInChildren<UberText>(includeInactive))
      componentsInChild.AmbientLightBlend = blendValue;
  }

  private void SetRendererLightBlend(Renderer renderer, float blendValue)
  {
    foreach (Material material in RendererExtension.GetMaterials(renderer))
    {
      if (!((UnityEngine.Object) material == (UnityEngine.Object) null) && material.HasProperty("_LightingBlend"))
        material.SetFloat("_LightingBlend", blendValue);
    }
  }

  private void RecursivelyReplaceMaterialsList(
    Transform transformToRecurse,
    Material newMaterialPrefab)
  {
    bool flag = true;
    if ((UnityEngine.Object) transformToRecurse.GetComponent<MaterialReplacementExclude>() != (UnityEngine.Object) null)
      flag = false;
    else if ((UnityEngine.Object) transformToRecurse.GetComponent<UberText>() != (UnityEngine.Object) null)
      flag = false;
    else if ((UnityEngine.Object) transformToRecurse.GetComponent<Renderer>() == (UnityEngine.Object) null)
      flag = false;
    if (flag)
      this.ReplaceMaterialsList(transformToRecurse.GetComponent<Renderer>(), newMaterialPrefab);
    foreach (Transform transformToRecurse1 in transformToRecurse)
      this.RecursivelyReplaceMaterialsList(transformToRecurse1, newMaterialPrefab);
  }

  private void ReplaceMaterialsList(Renderer renderer, Material newMaterialPrefab)
  {
    List<Material> materials = RendererExtension.GetMaterials(renderer);
    int count = materials.Count;
    Material[] materialArray = new Material[count];
    for (int index = 0; index < count; ++index)
    {
      Material oldMaterial = materials[index];
      materialArray[index] = this.CreateReplacementMaterial(oldMaterial, newMaterialPrefab);
    }
    RendererExtension.SetMaterials(renderer, materialArray);
    if ((UnityEngine.Object) renderer != (UnityEngine.Object) this.m_meshRenderer)
      return;
    this.UpdatePortraitTexture();
  }

  private Material CreateReplacementMaterial(
    Material oldMaterial,
    Material newMaterialPrefab)
  {
    Material replacementMaterial = UnityEngine.Object.Instantiate<Material>(newMaterialPrefab);
    replacementMaterial.mainTexture = oldMaterial.mainTexture;
    return replacementMaterial;
  }

  public void SeedMaterialEffects()
  {
    if (this.m_materialEffectsSeeded)
      return;
    this.m_materialEffectsSeeded = true;
    Renderer[] componentsInChildren = this.GetComponentsInChildren<Renderer>();
    float num = UnityEngine.Random.Range(0.0f, 2f);
    foreach (Renderer r in componentsInChildren)
    {
      List<Material> sharedMaterials = RendererExtension.GetSharedMaterials(r);
      if (sharedMaterials.Count == 1)
      {
        Material material = sharedMaterials[0];
        if (material.HasProperty("_Seed") && (double) material.GetFloat("_Seed") == 0.0)
          Actor.GetMaterialInstance(r).SetFloat("_Seed", num);
      }
      else
      {
        List<Material> materials = RendererExtension.GetMaterials(r);
        if (materials != null && materials.Count != 0)
        {
          foreach (Material material in materials)
          {
            if (!((UnityEngine.Object) material == (UnityEngine.Object) null) && material.HasProperty("_Seed") && (double) material.GetFloat("_Seed") == 0.0)
              material.SetFloat("_Seed", num);
          }
        }
      }
    }
  }

  public void MaterialShaderAnimation(bool animationEnabled)
  {
    if ((bool) (UnityEngine.Object) this.m_diamondPortraitR2T)
      return;
    float num = 0.0f;
    if (animationEnabled)
      num = 1f;
    foreach (Renderer componentsInChild in this.GetComponentsInChildren<Renderer>(true))
    {
      foreach (Material sharedMaterial in RendererExtension.GetSharedMaterials(componentsInChild))
      {
        if (!((UnityEngine.Object) sharedMaterial == (UnityEngine.Object) null) && sharedMaterial.HasProperty("_TimeScale"))
          sharedMaterial.SetFloat("_TimeScale", num);
      }
    }
  }

  public CardBackManager.CardBackSlot GetCardBackSlot()
  {
    if (this.m_cardBackSlotOverride.HasValue)
      return this.m_cardBackSlotOverride.Value;
    Player.Side side = Player.Side.NEUTRAL;
    if (this.m_cardBackSideOverride.HasValue)
      side = this.m_cardBackSideOverride.Value;
    else if (this.m_entity != null)
    {
      Player controller = this.m_entity.GetController();
      if (controller != null)
        side = controller.GetSide();
    }
    SceneMgr sceneMgr = SceneMgr.Get();
    CardBackManager.CardBackSlot cardBackSlot = (sceneMgr != null ? (sceneMgr.GetMode() == SceneMgr.Mode.GAMEPLAY ? 1 : 0) : 0) != 0 ? CardBackManager.CardBackSlot.DEFAULT : CardBackManager.CardBackSlot.FAVORITE;
    switch (side)
    {
      case Player.Side.FRIENDLY:
        cardBackSlot = CardBackManager.CardBackSlot.FRIENDLY;
        break;
      case Player.Side.OPPOSING:
        cardBackSlot = CardBackManager.CardBackSlot.OPPONENT;
        break;
    }
    return cardBackSlot;
  }

  public void SetCardBackSideOverride(Player.Side? sideOverride) => this.m_cardBackSideOverride = sideOverride;

  public void SetCardBackSlotOverride(CardBackManager.CardBackSlot? slotOverride) => this.m_cardBackSlotOverride = slotOverride;

  public bool GetCardbackUpdateIgnore() => this.m_ignoreUpdateCardback;

  public void SetCardbackUpdateIgnore(bool ignoreUpdate) => this.m_ignoreUpdateCardback = ignoreUpdate;

  public void UpdateCardBack()
  {
    if (this.m_ignoreUpdateCardback)
      return;
    CardBackManager cardBackManager = CardBackManager.Get();
    if (cardBackManager == null)
      return;
    CardBackManager.CardBackSlot cardBackSlot = this.GetCardBackSlot();
    this.UpdateCardBackDisplay(cardBackSlot);
    this.UpdateCardBackDragEffect();
    if ((UnityEngine.Object) this.m_cardMesh == (UnityEngine.Object) null || this.m_cardBackMatIdx < 0 || (UnityEngine.Object) this.m_initialCardBackMaterial == (UnityEngine.Object) null)
      return;
    Renderer component = this.m_cardMesh.GetComponent<Renderer>();
    RendererExtension.SetSharedMaterial(component, this.m_cardBackMatIdx, this.m_initialCardBackMaterial);
    cardBackManager.SetCardBackMaterial(component, this.m_cardBackMatIdx, cardBackSlot);
  }

  public void EnableCardbackShadow(bool enabled)
  {
    CardBackDisplay componentInChildren = this.GetComponentInChildren<CardBackDisplay>(true);
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    componentInChildren.EnableShadow(enabled);
  }

  private void UpdateCardBackDragEffect()
  {
    if (SceneMgr.Get() == null || SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
      return;
    CardBackDragEffect componentInChildren = this.GetComponentInChildren<CardBackDragEffect>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    componentInChildren.SetEffect();
  }

  private void UpdateCardBackDisplay(CardBackManager.CardBackSlot slot)
  {
    CardBackDisplay componentInChildren = this.GetComponentInChildren<CardBackDisplay>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    componentInChildren.SetCardBack(slot);
  }

  public void UpdateTextures() => this.UpdatePortraitTexture();

  public void UpdatePortraitTexture()
  {
    bool flag = false;
    if ((UnityEngine.Object) this.m_portraitTextureOverride != (UnityEngine.Object) null)
      this.SetPortraitTexture(this.m_portraitTextureOverride);
    else if ((UnityEngine.Object) this.LegendaryHeroPortrait?.PortraitTexture != (UnityEngine.Object) null)
    {
      this.SetPortraitTexture(this.LegendaryHeroPortrait.PortraitTexture);
      flag = true;
    }
    else if ((UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType) != (UnityEngine.Object) null)
      this.SetPortraitTexture(this.m_cardDefHandle.Get(this.m_premiumType).GetPortraitTexture(this.m_premiumType));
    if (flag)
      this.ConnectLegendarySkinToDynamicResolutionController();
    else
      this.DisconnectLegendarySkinToDynamicResolutionController();
  }

  public void SetPortraitTexture(Texture texture)
  {
    CardDef cardDef = this.m_cardDefHandle.Get(this.m_premiumType);
    if ((UnityEngine.Object) cardDef != (UnityEngine.Object) null && (this.m_premiumType >= TAG_PREMIUM.GOLDEN || cardDef.m_AlwaysRenderPremiumPortrait) && (this.m_premiumType == TAG_PREMIUM.SIGNATURE && (UnityEngine.Object) cardDef.GetSignaturePortraitMaterial() != (UnityEngine.Object) null || this.IsPremiumPortraitEnabled() && (UnityEngine.Object) cardDef.GetPremiumPortraitMaterial() != (UnityEngine.Object) null))
      return;
    Material portraitMaterial = this.GetPortraitMaterial();
    if ((UnityEngine.Object) portraitMaterial == (UnityEngine.Object) null)
      return;
    portraitMaterial.mainTexture = texture;
  }

  public void SetPortraitTextureOverride(Texture portrait)
  {
    this.m_portraitTextureOverride = portrait;
    this.UpdatePortraitTexture();
  }

  public Texture GetPortraitTexture()
  {
    Material portraitMaterial = this.GetPortraitMaterial();
    return (UnityEngine.Object) portraitMaterial == (UnityEngine.Object) null ? (Texture) null : portraitMaterial.mainTexture;
  }

  public Texture GetStaticPortraitTexture() => (UnityEngine.Object) this.m_portraitTextureOverride != (UnityEngine.Object) null ? this.m_portraitTextureOverride : this.m_cardDefHandle.Get(this.m_premiumType).GetPortraitTexture(this.m_premiumType);

  private async UniTaskVoid UpdatePortraitMaterials(
    CancellationToken token,
    CardDef alternativeCardDef)
  {
    this.isPortraitMaterialDirty = false;
    if (this.m_shadowform)
      return;
    CardDef cardDef = alternativeCardDef ?? this.m_cardDefHandle.Get(this.m_premiumType);
    if (!(bool) (UnityEngine.Object) cardDef)
      return;
    TAG_PREMIUM portraitPremiumLevel = this.m_premiumType;
    if (cardDef.m_AlwaysRenderPremiumPortrait)
      portraitPremiumLevel = TAG_PREMIUM.GOLDEN;
    if (portraitPremiumLevel == TAG_PREMIUM.SIGNATURE || portraitPremiumLevel == TAG_PREMIUM.GOLDEN && this.IsPremiumPortraitEnabled())
    {
      if (!cardDef.IsPremiumLoaded(this.m_premiumType))
      {
        CardTextureLoader.Load(cardDef, new CardPortraitQuality(3, this.m_premiumType));
        await UniTask.Yield(PlayerLoopTiming.Update, token);
        if ((UnityEngine.Object) (alternativeCardDef ?? this.m_cardDefHandle.Get(this.m_premiumType)) != (UnityEngine.Object) cardDef)
          return;
      }
      Material portraitMaterial = cardDef.GetPortraitMaterial(portraitPremiumLevel);
      if ((UnityEngine.Object) portraitMaterial != (UnityEngine.Object) null)
        this.SetPortraitMaterial(portraitMaterial);
      else if ((UnityEngine.Object) this.m_initialPortraitMaterial != (UnityEngine.Object) null)
        this.SetPortraitMaterial(this.m_initialPortraitMaterial);
    }
    else
      this.SetPortraitMaterial(this.m_initialPortraitMaterial);
    this.UpdatePortraitTexture();
    this.UpdateGhostCardEffect(true);
    Action portraitMaterialUpdated = this.OnPortraitMaterialUpdated;
    if (portraitMaterialUpdated == null)
      return;
    portraitMaterialUpdated();
  }

  private void UpdatePortraitMaterialAnimation()
  {
    if ((UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType) == (UnityEngine.Object) null || (UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType).GetPremiumPortraitAnimation() == (UnityEngine.Object) null || (UnityEngine.Object) this.m_portraitMesh == (UnityEngine.Object) null)
      return;
    this.m_uberShaderController = this.m_portraitMesh.GetComponent<UberShaderController>();
    if ((UnityEngine.Object) this.m_uberShaderController == (UnityEngine.Object) null)
    {
      this.m_uberShaderController = this.m_portraitMesh.gameObject.AddComponent<UberShaderController>();
      this.m_uberShaderController.UberShaderAnimation = UnityEngine.Object.Instantiate<UberShaderAnimation>(this.m_cardDefHandle.Get(this.m_premiumType).GetPremiumPortraitAnimation());
    }
    else
    {
      if (this.m_uberShaderController.UberShaderAnimation.name.Replace("(Clone)", "") == this.m_cardDefHandle.Get(this.m_premiumType).GetPremiumPortraitAnimation().name)
        return;
      this.m_uberShaderController.UberShaderAnimation = UnityEngine.Object.Instantiate<UberShaderAnimation>(this.m_cardDefHandle.Get(this.m_premiumType).GetPremiumPortraitAnimation());
    }
    this.m_uberShaderController.m_MaterialIndex = this.m_portraitMatIdx;
    if (this.isGhostCard() && this.m_ghostCard != GhostCard.Type.DORMANT)
      this.m_uberShaderController.enabled = false;
    else
      this.m_uberShaderController.enabled = true;
  }

  public void SetPortraitMaterial(Material material)
  {
    if ((UnityEngine.Object) material == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.m_portraitMesh != (UnityEngine.Object) null && this.m_portraitMatIdx > -1)
    {
      Renderer component = this.m_portraitMesh.GetComponent<Renderer>();
      Material material1 = RendererExtension.GetMaterial(component, this.m_portraitMatIdx);
      if ((UnityEngine.Object) material1.mainTexture == (UnityEngine.Object) material.mainTexture && (UnityEngine.Object) material1.shader == (UnityEngine.Object) material.shader)
        return;
      RendererExtension.SetMaterial(component, this.m_portraitMatIdx, material);
      float num = 0.0f;
      if ((bool) (UnityEngine.Object) this.m_card)
      {
        switch (this.m_card.GetZone())
        {
          case ZonePlay _:
          case ZoneWeapon _:
          case ZoneHeroPower _:
          case ZoneBattlegroundHeroBuddy _:
            num = 1f;
            break;
        }
      }
      foreach (Material material2 in RendererExtension.GetMaterials(component))
      {
        if (material2.HasProperty("_LightingBlend"))
          material2.SetFloat("_LightingBlend", num);
        if (material2.HasProperty("_Seed") && (double) material2.GetFloat("_Seed") == 0.0)
          material2.SetFloat("_Seed", UnityEngine.Random.Range(0.0f, 2f));
      }
    }
    else
    {
      if (this.m_legacyPortraitMaterialIndex < 0 || (UnityEngine.Object) RendererExtension.GetMaterial((Renderer) this.m_meshRenderer, this.m_legacyPortraitMaterialIndex) == (UnityEngine.Object) material)
        return;
      RendererExtension.SetMaterial((Renderer) this.m_meshRenderer, this.m_legacyPortraitMaterialIndex, material);
    }
  }

  public void SetPortraitDesaturation(float desaturation)
  {
    if (!((UnityEngine.Object) this.m_portraitMesh != (UnityEngine.Object) null) || this.m_portraitMatIdx <= -1)
      return;
    foreach (Material material in RendererExtension.GetMaterials(this.m_portraitMesh.GetComponent<Renderer>()))
    {
      if (material.HasProperty("_Desaturate"))
        material.SetFloat("_Desaturate", desaturation);
    }
  }

  public GameObject GetPortraitMesh() => this.m_portraitMesh;

  public virtual Material GetPortraitMaterial()
  {
    if ((UnityEngine.Object) this.m_portraitMesh != (UnityEngine.Object) null)
    {
      Renderer component = this.m_portraitMesh.GetComponent<Renderer>();
      if (0 <= this.m_portraitMatIdx && this.m_portraitMatIdx < RendererExtension.GetSharedMaterials(component).Count)
        return !Application.isPlaying ? RendererExtension.GetSharedMaterial(component, this.m_portraitMatIdx) : RendererExtension.GetMaterial(component, this.m_portraitMatIdx);
    }
    return this.m_legacyPortraitMaterialIndex >= 0 ? RendererExtension.GetMaterial((Renderer) this.m_meshRenderer, this.m_legacyPortraitMaterialIndex) : (Material) null;
  }

  protected virtual bool IsPremiumPortraitEnabled()
  {
    GameEntity gameEntityIfAllowed = this.GetGameEntityIfAllowed();
    if (gameEntityIfAllowed != null && gameEntityIfAllowed.HasTag(GAME_TAG.DISABLE_NONHERO_GOLDEN_ANIMATIONS) && (this.m_entityDef == null || !this.m_entityDef.IsHero()) && (this.m_entity == null || !this.m_entity.IsHero()))
      return false;
    CardDef cardDef = this.m_cardDefHandle.Get(this.m_premiumType);
    return !((UnityEngine.Object) cardDef == (UnityEngine.Object) null) && string.IsNullOrEmpty(cardDef.m_LegendaryModel) && this.m_graphicsManager != null && !this.m_graphicsManager.isVeryLowQualityDevice();
  }

  public void SetBlockTextComponentUpdate(bool block) => this.m_blockTextComponentUpdate = block;

  public virtual void UpdateTextComponents()
  {
    if (this.m_blockTextComponentUpdate)
      return;
    if (this.m_entityDef != null)
      this.UpdateTextComponentsDef(this.m_entityDef);
    else
      this.UpdateTextComponents(this.m_entity);
  }

  public virtual void UpdateTextComponentsDef(EntityDef entityDef)
  {
    if (entityDef == null)
      return;
    this.UpdateCostTextMesh(entityDef);
    this.UpdateAttackTextMesh(entityDef);
    this.UpdateHealthTextMesh(entityDef);
    this.UpdateArmorTextMesh(entityDef);
    this.UpdateNameText();
    this.UpdatePowersText();
    this.UpdateRace(entityDef.GetRaceText());
    this.UpdateSecretAndQuestText();
    this.UpdateBannedRibbonTextMesh(entityDef);
    this.UpdateMercenaryLevelTextMesh(entityDef);
    this.UpdateMercenaryFactionBannerMesh(entityDef);
  }

  private void UpdateCostTextMesh(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_costTextMesh == (UnityEngine.Object) null)
      return;
    if (this.HasHideStats((EntityBase) entityDef) || entityDef.HasTag(GAME_TAG.HIDE_COST) || this.UseTechLevelManaGem())
    {
      this.m_costTextMesh.Text = "";
    }
    else
    {
      bool flag = GameMgr.Get().IsBattlegrounds();
      if ((!entityDef.IsCardButton() || !entityDef.HasTriggerVisual() ? 0 : (!flag ? 1 : (!entityDef.HasTag(GAME_TAG.HAS_ACTIVATE_POWER) ? 1 : 0))) != 0)
        this.m_costTextMesh.Text = "";
      else
        this.m_costTextMesh.Text = Convert.ToString(entityDef.GetTag(GAME_TAG.COST));
    }
  }

  public void UpdateAttackTextMesh(EntityDef entityDef)
  {
    int tag = entityDef.GetTag(GAME_TAG.ATK);
    if ((UnityEngine.Object) this.m_attackTextMesh != (UnityEngine.Object) null && (this.HasHideStats((EntityBase) entityDef) || entityDef.HasTag(GAME_TAG.HIDE_ATTACK)))
    {
      this.m_attackTextMesh.Text = "";
      this.m_attackTextMesh.gameObject.SetActive(false);
      GemObject componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<GemObject>(this.m_attackTextMesh.gameObject);
      if (!((UnityEngine.Object) componentInThisOrParents != (UnityEngine.Object) null))
        return;
      componentInThisOrParents.Hide();
      componentInThisOrParents.SetHideNumberFlag(true);
    }
    else if (entityDef.IsHero())
    {
      if (tag == 0)
      {
        if ((UnityEngine.Object) this.m_attackObject != (UnityEngine.Object) null && this.m_attackObject.activeSelf)
          this.m_attackObject.SetActive(false);
        if (!((UnityEngine.Object) this.m_attackTextMesh != (UnityEngine.Object) null))
          return;
        this.m_attackTextMesh.Text = "";
      }
      else
      {
        if ((UnityEngine.Object) this.m_attackObject != (UnityEngine.Object) null && !this.m_attackObject.activeSelf)
          this.m_attackObject.SetActive(true);
        if (!((UnityEngine.Object) this.m_attackTextMesh != (UnityEngine.Object) null))
          return;
        this.m_attackTextMesh.Text = Convert.ToString(tag);
      }
    }
    else
    {
      if (!((UnityEngine.Object) this.m_attackTextMesh != (UnityEngine.Object) null))
        return;
      this.m_attackTextMesh.Text = Convert.ToString(tag);
    }
  }

  public void UpdateHealthTextMesh(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_healthTextMesh == (UnityEngine.Object) null)
      return;
    if (this.HasHideStats((EntityBase) entityDef) || entityDef.HasTag(GAME_TAG.HIDE_HEALTH))
    {
      this.m_healthTextMesh.Text = "";
      this.m_healthTextMesh.gameObject.SetActive(false);
      GemObject componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<GemObject>(this.m_healthTextMesh.gameObject);
      if (!((UnityEngine.Object) componentInThisOrParents != (UnityEngine.Object) null))
        return;
      componentInThisOrParents.Hide();
      componentInThisOrParents.SetHideNumberFlag(true);
    }
    else if (entityDef.IsWeapon())
      this.m_healthTextMesh.Text = Convert.ToString(entityDef.GetTag(GAME_TAG.DURABILITY));
    else
      this.m_healthTextMesh.Text = Convert.ToString(entityDef.GetTag(GAME_TAG.HEALTH));
  }

  private void UpdateArmorTextMesh(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_armorTextMesh == (UnityEngine.Object) null)
      return;
    int tag = entityDef.GetTag(GAME_TAG.ARMOR);
    if (tag == 0 || this.HasHideStats((EntityBase) entityDef))
    {
      if ((UnityEngine.Object) this.m_armorObject != (UnityEngine.Object) null && this.m_armorObject.activeSelf)
        this.m_armorObject.SetActive(false);
      this.m_armorTextMesh.Text = "";
    }
    else
    {
      if ((UnityEngine.Object) this.m_armorObject != (UnityEngine.Object) null && !this.m_armorObject.activeSelf)
        this.m_armorObject.SetActive(true);
      this.m_armorTextMesh.Text = Convert.ToString(tag);
    }
  }

  private void UpdateMercenaryLevelTextMesh(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_mercenaryLevelObject == (UnityEngine.Object) null || (UnityEngine.Object) this.m_mercenaryLevelObject.m_levelText == (UnityEngine.Object) null)
      return;
    if (this.HasHideStats((EntityBase) entityDef))
      this.UpdateNumberText(this.m_mercenaryLevelObject.m_levelText, "", true);
    else
      this.m_mercenaryLevelObject.SetLevelText(GameUtils.GetMercenaryLevelFromExperience(entityDef.GetTag(GAME_TAG.LETTUCE_MERCENARY_EXPERIENCE)));
  }

  private void UpdateMercenaryFactionBannerMesh(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_factionBannerBackground == (UnityEngine.Object) null || this.m_factionBannerIcons == null || this.m_factionBannerIcons.Length == 0)
      return;
    TAG_LETTUCE_FACTION tag = entityDef.GetTag<TAG_LETTUCE_FACTION>(GAME_TAG.LETTUCE_FACTION);
    bool flag1 = tag != 0;
    int num = 0;
    this.m_factionBannerBackground.SetActive(flag1);
    foreach (Actor.FactionObject factionBannerIcon in this.m_factionBannerIcons)
    {
      bool flag2 = factionBannerIcon.m_faction == tag & flag1;
      if ((UnityEngine.Object) factionBannerIcon.m_banner != (UnityEngine.Object) null)
      {
        factionBannerIcon.m_banner.SetActive(flag2);
        num += flag2 ? 1 : 0;
      }
    }
    if (!flag1 || num == 1)
      return;
    Debug.LogError((object) string.Format("Error enabling faction banners on {0}. Expected to enable 1 faction icon, instead got {1}. Requested faction is \"{2}\".", (object) entityDef.GetName(), (object) num, (object) tag));
  }

  private void UpdateBannedRibbonTextMesh(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_bannedRibbonContainer == (UnityEngine.Object) null)
      return;
    this.m_bannedRibbonContainer.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_bannedRibbon == (UnityEngine.Object) null || SceneMgr.Get().GetMode() != SceneMgr.Mode.COLLECTIONMANAGER || entityDef.IsCustomCoin() || CraftingManager.GetIsInCraftingMode() || !RankMgr.Get().HasLocalPlayerMedalInfo || !RankMgr.Get().IsCardLockedInCurrentLeague(entityDef))
      return;
    this.m_bannedRibbonContainer.gameObject.SetActive(true);
    this.m_bannedRibbon.SetActive(true);
    this.m_bannedRibbon.GetComponentInChildren<UberText>().Text = (string) RankMgr.Get().GetLocalPlayerStandardLeagueConfig().LockedCardUnplayableText;
  }

  public void UpdateMinionStatsImmediately()
  {
    if (this.m_entity == null || !this.m_entity.IsMinion() || this.HasHideStats((EntityBase) this.m_entity))
      return;
    if ((UnityEngine.Object) this.m_attackTextMesh != (UnityEngine.Object) null && !this.m_entity.HasTag(GAME_TAG.HIDE_ATTACK))
    {
      this.UpdateTextColorToGreenOrWhite(this.m_attackTextMesh, this.m_entity.GetDefATK(), this.m_entity.GetATK());
      this.m_attackTextMesh.Text = Convert.ToString(this.m_entity.GetATK());
    }
    if (!((UnityEngine.Object) this.m_healthTextMesh != (UnityEngine.Object) null) || this.m_entity.HasTag(GAME_TAG.HIDE_HEALTH))
      return;
    int num;
    if (this.m_entity.HasTag(GAME_TAG.ENABLE_HEALTH_DISPLAY))
    {
      num = this.m_entity.GetTag(GAME_TAG.HEALTH_DISPLAY);
      if (this.m_entity.HasTag(GAME_TAG.HEALTH_DISPLAY_NEGATIVE))
        num = -num;
      switch (this.m_entity.GetTag(GAME_TAG.HEALTH_DISPLAY_COLOR))
      {
        case 0:
          this.UpdateTextColor(this.m_healthTextMesh, num, num);
          break;
        case 1:
          this.UpdateTextColor(this.m_healthTextMesh, num + 1, num);
          break;
        case 2:
          this.UpdateTextColor(this.m_healthTextMesh, num - 1, num);
          break;
      }
    }
    else
    {
      int health = this.m_entity.GetHealth();
      int defHealth = this.m_entity.GetDefHealth();
      num = health - this.m_entity.GetDamage();
      if (this.m_entity.GetDamage() > 0)
        this.UpdateTextColor(this.m_healthTextMesh, health, num);
      else if (health > defHealth)
        this.UpdateTextColor(this.m_healthTextMesh, defHealth, num);
      else
        this.UpdateTextColor(this.m_healthTextMesh, num, num);
    }
    this.m_healthTextMesh.Text = Convert.ToString(num);
  }

  public virtual void UpdateTextComponents(Entity entity)
  {
    if (entity == null)
      return;
    this.UpdateCostTextMesh(entity);
    this.UpdateAttackTextMesh(entity);
    this.UpdateHealthTextMesh(entity);
    this.UpdateArmorTextMesh(entity);
    this.UpdateNameText();
    this.UpdatePowersText();
    this.UpdateRace(entity.GetRaceText());
    this.UpdateSecretAndQuestText();
    this.UpdateMercenaryLevelTextMesh(entity);
  }

  private int GetSecretCostByClass(TAG_CLASS classType)
  {
    switch (classType)
    {
      case TAG_CLASS.HUNTER:
      case TAG_CLASS.ROGUE:
        return 2;
      case TAG_CLASS.MAGE:
        return 3;
      case TAG_CLASS.PALADIN:
        return 1;
      case TAG_CLASS.WARRIOR:
        return 0;
      default:
        return -1;
    }
  }

  private void UpdateCostTextMesh(Entity entity)
  {
    if ((UnityEngine.Object) this.m_costTextMesh == (UnityEngine.Object) null)
      return;
    if (this.HasHideStats((EntityBase) this.m_entity) || this.m_entity.HasTag(GAME_TAG.HIDE_COST) || this.UseTechLevelManaGem())
    {
      this.UpdateNumberText(this.m_costTextMesh, "", false);
    }
    else
    {
      if (this.m_entity.IsSecret() && this.m_entity.IsHidden() && this.m_entity.IsControlledByConcealedPlayer())
      {
        int secretCostByClass = this.GetSecretCostByClass(entity.GetClass());
        if (secretCostByClass >= 0)
          this.UpdateTextColor(this.m_costTextMesh, secretCostByClass, entity.GetCost(), true);
        else
          this.m_costTextMesh.TextColor = Color.white;
      }
      else
        this.UpdateTextColor(this.m_costTextMesh, entity.GetDefCost(), entity.GetCost(), true);
      bool flag = GameMgr.Get().IsBattlegrounds();
      if ((!this.m_entity.IsCardButton() || !this.m_entity.HasTriggerVisual() ? 0 : (!flag ? 1 : (!this.m_entity.HasTag(GAME_TAG.HAS_ACTIVATE_POWER) ? 1 : 0))) != 0)
        this.UpdateNumberText(this.m_costTextMesh, "", true);
      else
        this.UpdateNumberText(this.m_costTextMesh, Convert.ToString(entity.GetCost()));
    }
  }

  private void UpdateAttackTextMesh(Entity entity)
  {
    if ((UnityEngine.Object) this.m_attackTextMesh == (UnityEngine.Object) null)
      return;
    if (this.HasHideStats((EntityBase) entity) || entity.HasTag(GAME_TAG.HIDE_ATTACK))
      this.UpdateNumberText(this.m_attackTextMesh, "", true);
    else if (entity.IsHero())
    {
      int atk = entity.GetATK();
      if (atk == 0)
      {
        this.UpdateNumberText(this.m_attackTextMesh, "", true);
      }
      else
      {
        Card weaponCard = entity.GetController().GetWeaponCard();
        int defNumber = 0;
        if ((UnityEngine.Object) weaponCard != (UnityEngine.Object) null)
          defNumber = weaponCard.GetEntity().GetATK();
        this.UpdateTextColorToGreenOrWhite(this.m_attackTextMesh, defNumber, atk);
        this.UpdateNumberText(this.m_attackTextMesh, Convert.ToString(atk));
      }
    }
    else
    {
      int currentNumber = entity.GetATK();
      if (entity.IsDormant() && entity.HasCachedTagForDormant(GAME_TAG.ATK))
        currentNumber = entity.GetCachedTagForDormant(GAME_TAG.ATK);
      this.UpdateTextColorToGreenOrWhite(this.m_attackTextMesh, entity.GetDefATK(), currentNumber);
      this.UpdateNumberText(this.m_attackTextMesh, Convert.ToString(currentNumber));
    }
  }

  private void UpdateHealthTextMesh(Entity entity)
  {
    if (!((UnityEngine.Object) this.m_healthTextMesh != (UnityEngine.Object) null) || entity.IsHero() && entity.GetZone() == TAG_ZONE.GRAVEYARD)
      return;
    if (this.HasHideStats((EntityBase) entity) || entity.HasTag(GAME_TAG.HIDE_HEALTH))
    {
      this.UpdateNumberText(this.m_healthTextMesh, "", true);
    }
    else
    {
      int defNumber1;
      int defNumber2;
      if (entity.IsWeapon())
      {
        defNumber1 = entity.GetDurability();
        defNumber2 = entity.GetDefDurability();
      }
      else
      {
        defNumber1 = entity.GetHealth();
        defNumber2 = entity.GetDefHealth();
      }
      int num1 = entity.GetDamage();
      if (entity.IsDormant())
      {
        if (entity.HasCachedTagForDormant(GAME_TAG.HEALTH))
          defNumber1 = entity.GetCachedTagForDormant(GAME_TAG.HEALTH);
        if (entity.HasCachedTagForDormant(GAME_TAG.DAMAGE))
          num1 = entity.GetCachedTagForDormant(GAME_TAG.DAMAGE);
      }
      int num2 = defNumber1 - num1;
      if (this.m_entity.HasTag(GAME_TAG.ENABLE_HEALTH_DISPLAY))
      {
        num2 = this.m_entity.GetTag(GAME_TAG.HEALTH_DISPLAY);
        if (this.m_entity.HasTag(GAME_TAG.HEALTH_DISPLAY_NEGATIVE))
          num2 = -num2;
        switch (this.m_entity.GetTag(GAME_TAG.HEALTH_DISPLAY_COLOR))
        {
          case 0:
            this.UpdateTextColor(this.m_healthTextMesh, num2, num2);
            break;
          case 1:
            this.UpdateTextColor(this.m_healthTextMesh, num2 + 1, num2);
            break;
          case 2:
            this.UpdateTextColor(this.m_healthTextMesh, num2 - 1, num2);
            break;
        }
      }
      else if (entity.GetDamage() > 0)
        this.UpdateTextColor(this.m_healthTextMesh, defNumber1, num2);
      else if (defNumber1 > defNumber2)
        this.UpdateTextColor(this.m_healthTextMesh, defNumber2, num2);
      else
        this.UpdateTextColor(this.m_healthTextMesh, num2, num2);
      this.UpdateNumberText(this.m_healthTextMesh, Convert.ToString(num2));
    }
  }

  private void UpdateArmorTextMesh(Entity entity)
  {
    if ((UnityEngine.Object) this.m_armorTextMesh == (UnityEngine.Object) null)
      return;
    if (this.HasHideStats((EntityBase) entity))
    {
      this.UpdateNumberText(this.m_armorTextMesh, "", true);
    }
    else
    {
      int armor = entity.GetArmor();
      if (armor == 0)
        this.UpdateNumberText(this.m_armorTextMesh, "", true);
      else
        this.UpdateNumberText(this.m_armorTextMesh, Convert.ToString(armor));
    }
  }

  private void UpdateMercenaryLevelTextMesh(Entity entity)
  {
    if ((UnityEngine.Object) this.m_mercenaryLevelObject == (UnityEngine.Object) null || (UnityEngine.Object) this.m_mercenaryLevelObject.m_levelText == (UnityEngine.Object) null)
      return;
    if (this.HasHideStats((EntityBase) entity))
      this.UpdateNumberText(this.m_mercenaryLevelObject.m_levelText, "", true);
    else
      this.m_mercenaryLevelObject.SetLevelText(GameUtils.GetMercenaryLevelFromExperience(entity.GetTag(GAME_TAG.LETTUCE_MERCENARY_EXPERIENCE)));
  }

  public void SetCardDefPowerTextOverride(string text) => this.m_cardDefPowerTextOverride = text;

  public void UpdatePowersText()
  {
    if ((UnityEngine.Object) this.m_powersTextMesh == (UnityEngine.Object) null && (UnityEngine.Object) this.m_bgQuestPowerTextMesh == (UnityEngine.Object) null)
      return;
    string text = (string) null;
    bool flag = false;
    if (this.IsLettuceMercenary())
    {
      if (GameState.Get() == null && !this.m_showUICardText)
        return;
      Entity entity = this.m_entity;
      if (entity != null && entity.IsHistoryDupe())
        entity = entity.GetCard().GetEntity();
      if (entity == null && (UnityEngine.Object) this.m_card != (UnityEngine.Object) null)
        entity = this.m_card.GetEntity();
      if (entity != null)
      {
        flag = true;
        if (entity.ShouldShowEquipmentTextOnMerc())
        {
          text = entity.GetEquipmentEntity()?.GetCardTextInHand();
          if ((UnityEngine.Object) this.m_watermarkMesh != (UnityEngine.Object) null)
            this.m_watermarkMesh.SetActive(true);
        }
      }
      else if (this.m_showUICardText)
        text = this.m_UICardText;
    }
    if (!this.m_showUICardText && string.IsNullOrEmpty(text))
    {
      if (this.ShouldUseEntityDefForPowersText())
      {
        text = string.IsNullOrEmpty(this.m_cardDefPowerTextOverride) ? this.m_entityDef.GetCardTextInHand() : this.m_cardDefPowerTextOverride;
      }
      else
      {
        text = !this.m_entity.IsSecret() || !this.m_entity.IsHidden() || !this.m_entity.IsControlledByConcealedPlayer() ? (!this.m_entity.IsHistoryDupe() ? this.m_entity.GetCardTextInHand() : this.m_entity.GetCardTextInHistory()) : GameStrings.Get("GAMEPLAY_SECRET_DESC");
        GameEntity gameEntityIfAllowed = this.GetGameEntityIfAllowed();
        if (gameEntityIfAllowed != null)
          text = gameEntityIfAllowed.UpdateCardText(this.m_card, this, text);
      }
    }
    this.UpdateText(this.m_powersTextMesh, text);
    this.UpdateText(this.m_bgQuestPowerTextMesh, text);
    if (!flag && !this.m_showUICardText || !((UnityEngine.Object) this.m_mercenaryLevelObject != (UnityEngine.Object) null))
      return;
    this.m_mercenaryLevelObject.gameObject.SetActive(true);
    this.m_mercenaryLevelObject.m_xpBar.gameObject.SetActive(false);
    this.m_mercenaryLevelObject.m_xpBarBacking.SetActive(false);
    this.m_mercenaryLevelObject.m_xpBarCover.SetActive(false);
    if (!((UnityEngine.Object) this.m_watermarkMesh != (UnityEngine.Object) null))
      return;
    this.m_watermarkMesh.SetActive(string.IsNullOrEmpty(text));
  }

  public void UpdateDynamicTextFromQuestEntity(Entity questEnt)
  {
    if (questEnt == null)
      return;
    if (questEnt.HasTag(GAME_TAG.BACON_MINION_TYPE_REWARD))
      this.SetCardDefPowerTextOverride(string.Format(this.m_entityDef.GetCardTextInHand(), (object) GameStrings.GetRaceNameBattlegrounds((TAG_RACE) questEnt.GetTag(GAME_TAG.BACON_MINION_TYPE_REWARD))));
    if (!questEnt.HasTag(GAME_TAG.BACON_CARD_DBID_REWARD))
      return;
    string cardTextInHand = this.m_entityDef.GetCardTextInHand();
    CardDbfRecord record = GameDbf.Card.GetRecord(questEnt.GetTag(GAME_TAG.BACON_CARD_DBID_REWARD));
    if (record == null)
      return;
    string name = (string) record.Name;
    this.SetCardDefPowerTextOverride(string.Format(cardTextInHand, (object) name));
  }

  private bool ShouldUseEntityDefForPowersText() => this.m_entityDef != null && (this.m_entity == null || !this.m_entity.GetCardTextBuilder().ShouldUseEntityForTextInPlay());

  private void UpdateNumberText(UberText textMesh, string newText) => this.UpdateNumberText(textMesh, newText, false);

  private void UpdateNumberText(UberText textMesh, string newText, bool shouldHide)
  {
    GemObject componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<GemObject>(textMesh.gameObject);
    if ((UnityEngine.Object) componentInThisOrParents != (UnityEngine.Object) null)
    {
      if (!componentInThisOrParents.IsNumberHidden())
      {
        if (shouldHide)
        {
          textMesh.gameObject.SetActive(false);
          if ((UnityEngine.Object) this.GetHistoryCard() != (UnityEngine.Object) null || (UnityEngine.Object) this.GetHistoryChildCard() != (UnityEngine.Object) null)
            componentInThisOrParents.Hide();
          else
            componentInThisOrParents.ScaleToZero();
        }
        else if (textMesh.Text != newText)
          componentInThisOrParents.Jiggle();
      }
      else if (!shouldHide)
      {
        textMesh.gameObject.SetActive(true);
        componentInThisOrParents.SetToZeroThenEnlarge();
      }
      componentInThisOrParents.Initialize();
      componentInThisOrParents.SetHideNumberFlag(shouldHide);
    }
    textMesh.Text = newText;
  }

  public void UpdateNameText()
  {
    if ((UnityEngine.Object) this.m_nameTextMesh == (UnityEngine.Object) null)
      return;
    string text = "";
    bool flag = false;
    if (this.m_entity != null)
    {
      if (this.m_entityDef == null)
        flag = this.m_entity.IsSecret() && this.m_entity.IsHidden() && this.m_entity.IsControlledByConcealedPlayer();
      text = this.m_entity.GetName();
    }
    else if (this.m_entityDef != null)
    {
      string shortName = this.m_entityDef.GetShortName();
      text = !this.m_useShortName || string.IsNullOrEmpty(shortName) ? this.m_entityDef.GetName() : shortName;
    }
    if (flag)
    {
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.USE_SECRET_CLASS_NAMES))
      {
        switch (this.m_entity.GetClass())
        {
          case TAG_CLASS.HUNTER:
            text = GameStrings.Get("GAMEPLAY_SECRET_NAME_HUNTER");
            break;
          case TAG_CLASS.MAGE:
            text = GameStrings.Get("GAMEPLAY_SECRET_NAME_MAGE");
            break;
          case TAG_CLASS.PALADIN:
            text = GameStrings.Get("GAMEPLAY_SECRET_NAME_PALADIN");
            break;
          case TAG_CLASS.ROGUE:
            text = GameStrings.Get("GAMEPLAY_SECRET_NAME_ROGUE");
            break;
          default:
            text = GameStrings.Get("GAMEPLAY_SECRET_NAME");
            break;
        }
      }
      else
        text = GameStrings.Get("GAMEPLAY_SECRET_NAME");
    }
    this.UpdateText(this.m_nameTextMesh, text);
  }

  private void UpdateSecretAndQuestText()
  {
    if (!(bool) (UnityEngine.Object) this.m_secretText)
      return;
    string text = "?";
    if (this.m_entity != null)
    {
      if (this.m_entity.IsQuest() || this.m_entity.IsSideQuest() || this.m_entity.IsQuestline())
        text = "!";
      else if (this.m_entity.IsPuzzle())
        text = "P";
    }
    if ((bool) UniversalInputManager.UsePhoneUI && this.m_entity != null)
    {
      TransformUtil.SetLocalPosZ((Component) this.m_secretText, -0.01f);
      Player controller = this.m_entity.GetController();
      if (controller != null && this.m_entity.IsSecret())
      {
        ZoneSecret secretZone = controller.GetSecretZone();
        if ((bool) (UnityEngine.Object) secretZone)
        {
          int secretCount = secretZone.GetSecretCount();
          if (secretCount > 1)
          {
            text = secretCount.ToString();
            TransformUtil.SetLocalPosZ((Component) this.m_secretText, -0.03f);
          }
        }
      }
      else if (controller != null && this.m_entity.IsSideQuest())
      {
        TransformUtil.SetLocalPosZ((Component) this.m_secretText, 0.01f);
        ZoneSecret secretZone = controller.GetSecretZone();
        if ((bool) (UnityEngine.Object) secretZone)
        {
          int sideQuestCount = secretZone.GetSideQuestCount();
          if (sideQuestCount > 1)
          {
            text = sideQuestCount.ToString();
            TransformUtil.SetLocalPosZ((Component) this.m_secretText, -0.02f);
          }
        }
      }
      Transform transform = this.m_secretText.transform.parent.Find("Secret_mesh");
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null && (UnityEngine.Object) transform.gameObject != (UnityEngine.Object) null)
      {
        SphereCollider component = transform.gameObject.GetComponent<SphereCollider>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.radius = 0.5f;
      }
    }
    this.UpdateText(this.m_secretText, text);
  }

  private void UpdateText(UberText uberTextMesh, string text)
  {
    if ((UnityEngine.Object) uberTextMesh == (UnityEngine.Object) null)
      return;
    uberTextMesh.Text = text;
  }

  private void UpdateTextColor(UberText originalMesh, int defNumber, int currentNumber) => this.UpdateTextColor(originalMesh, defNumber, currentNumber, false);

  private void UpdateTextColor(
    UberText uberTextMesh,
    int defNumber,
    int currentNumber,
    bool higherIsBetter)
  {
    if (defNumber > currentNumber & higherIsBetter || defNumber < currentNumber && !higherIsBetter)
      uberTextMesh.TextColor = Color.green;
    else if (defNumber < currentNumber & higherIsBetter || defNumber > currentNumber && !higherIsBetter)
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        uberTextMesh.TextColor = new Color(1f, 0.1960784f, 0.1960784f);
      else
        uberTextMesh.TextColor = Color.red;
    }
    else
    {
      if (defNumber != currentNumber)
        return;
      uberTextMesh.TextColor = Color.white;
    }
  }

  private void UpdateTextColorToGreenOrWhite(
    UberText uberTextMesh,
    int defNumber,
    int currentNumber)
  {
    if (defNumber < currentNumber)
      uberTextMesh.TextColor = Color.green;
    else
      uberTextMesh.TextColor = Color.white;
  }

  private void DisableTextMesh(UberText mesh)
  {
    if ((UnityEngine.Object) mesh == (UnityEngine.Object) null)
      return;
    mesh.gameObject.SetActive(false);
  }

  public void SetUseShortName(bool useShortName) => this.m_useShortName = useShortName;

  public void SetupUICardText(bool showText, string textToShow = null)
  {
    int num = this.m_showUICardText != showText ? 1 : (!this.m_showUICardText ? 0 : (!string.Equals(this.m_UICardText, textToShow, StringComparison.Ordinal) ? 1 : 0));
    this.m_showUICardText = showText;
    this.m_UICardText = this.m_showUICardText ? textToShow : (string) null;
    if (num == 0)
      return;
    this.UpdatePowersText();
  }

  public void OverrideNameText(UberText newText)
  {
    if ((UnityEngine.Object) this.m_nameTextMesh != (UnityEngine.Object) null)
      this.m_nameTextMesh.gameObject.SetActive(false);
    this.m_nameTextMesh = newText;
    this.UpdateNameText();
    if (!this.m_shown || !((UnityEngine.Object) newText != (UnityEngine.Object) null))
      return;
    newText.gameObject.SetActive(true);
  }

  public void HideAllText() => this.ToggleTextVisibility(false);

  public void ShowAllText() => this.ToggleTextVisibility(true);

  private void ToggleTextVisibility(bool bOn)
  {
    if ((UnityEngine.Object) this.m_healthTextMesh != (UnityEngine.Object) null)
      this.m_healthTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_armorTextMesh != (UnityEngine.Object) null)
      this.m_armorTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_attackTextMesh != (UnityEngine.Object) null)
      this.m_attackTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_nameTextMesh != (UnityEngine.Object) null)
    {
      this.m_nameTextMesh.gameObject.SetActive(bOn);
      if ((bool) (UnityEngine.Object) this.m_nameTextMesh.RenderOnObject)
        this.m_nameTextMesh.RenderOnObject.GetComponent<Renderer>().enabled = bOn;
    }
    if ((UnityEngine.Object) this.m_powersTextMesh != (UnityEngine.Object) null)
      this.m_powersTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_bgQuestPowerTextMesh != (UnityEngine.Object) null)
      this.m_bgQuestPowerTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_costTextMesh != (UnityEngine.Object) null)
      this.m_costTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_raceTextMesh != (UnityEngine.Object) null && this.m_entityDef != null && this.m_entityDef.GetRaceCount() == 1)
      this.m_raceTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_multiRaceTextMesh != (UnityEngine.Object) null && this.m_entityDef != null && this.m_entityDef.GetRaceCount() > 1)
      this.m_multiRaceTextMesh.gameObject.SetActive(bOn);
    if ((UnityEngine.Object) this.m_bgQuestRaceTextMesh != (UnityEngine.Object) null)
      this.m_bgQuestRaceTextMesh.gameObject.SetActive(bOn);
    if (!(bool) (UnityEngine.Object) this.m_secretText)
      return;
    this.m_secretText.gameObject.SetActive(bOn);
  }

  public void CreateBannedRibbon()
  {
    if (!((UnityEngine.Object) this.m_bannedRibbonContainer != (UnityEngine.Object) null))
      return;
    this.m_bannedRibbonContainer.gameObject.SetActive(true);
    this.m_bannedRibbon = this.m_bannedRibbonContainer.PrefabGameObject(true);
    if (!((UnityEngine.Object) this.m_bannedRibbon != (UnityEngine.Object) null))
      return;
    LayerUtils.SetLayer(this.m_bannedRibbon, this.gameObject.layer);
  }

  public bool IsContactShadowEnabled() => this.m_shadowVisible;

  public bool HasContactShadowObject() => this.m_contactShadows != null;

  public void ContactShadow(bool visible)
  {
    this.m_shadowVisible = visible;
    if (!this.m_shadowObjectInitialized)
      this.CacheShadowObjects();
    this.UpdateContactShadow();
  }

  public void UpdateContactShadow()
  {
    bool flag = this.IsElite();
    if (this.m_contactShadows == null)
      return;
    foreach (Actor.ContactShadowData contactShadow in this.m_contactShadows)
      contactShadow.ShadowObject.GetComponent<Renderer>().enabled = this.m_shadowVisible && this.m_shown && (flag && contactShadow.IsUnique || !flag && !contactShadow.IsUnique);
  }

  public void MoveShadowToMissingCard(bool reset, int renderQueue = 0)
  {
    Transform transform;
    if (reset && (UnityEngine.Object) this.m_cardMesh != (UnityEngine.Object) null)
    {
      transform = this.m_cardMesh.transform;
    }
    else
    {
      if (reset || !((UnityEngine.Object) this.m_missingCardEffect != (UnityEngine.Object) null))
        return;
      transform = this.m_missingCardEffect.transform;
    }
    bool flag = this.IsElite();
    if (this.m_contactShadows == null)
      return;
    foreach (Actor.ContactShadowData contactShadow in this.m_contactShadows)
    {
      if (flag == contactShadow.IsUnique)
      {
        Renderer component = contactShadow.ShadowObject.GetComponent<Renderer>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
        {
          int num = reset ? contactShadow.InitialRenderQueue : RendererExtension.GetMaterial(component).renderQueue + renderQueue;
          RendererExtension.GetMaterial(component).renderQueue = num;
          contactShadow.ShadowObject.transform.SetParent(this.transform, true);
          contactShadow.ShadowObject.transform.localPosition = contactShadow.InitialPositionRelativeToActor;
          contactShadow.ShadowObject.transform.SetParent(transform, true);
        }
      }
    }
  }

  public virtual void UpdateMeshComponents()
  {
    this.UpdateRarityComponent();
    this.UpdateDescriptionMesh();
    this.UpdateEliteComponent();
    this.UpdatePremiumComponents();
    this.UpdateCardColor();
    this.UpdateManaGemComponent();
    this.UpdateMercenaryRoleComponents();
    this.UpdateCardRuneBannerComponent();
  }

  private void UpdateRarityComponent()
  {
    if (!(bool) (UnityEngine.Object) this.m_rarityGemMesh)
      return;
    UnityEngine.Vector2 offset;
    Color tint;
    bool rarityTextureOffset = this.GetRarityTextureOffset(out offset, out tint);
    RenderUtils.EnableRenderers(this.m_rarityGemMesh, rarityTextureOffset, true);
    if ((bool) (UnityEngine.Object) this.m_rarityFrameMesh)
      RenderUtils.EnableRenderers(this.m_rarityFrameMesh, rarityTextureOffset, true);
    if (!rarityTextureOffset)
      return;
    Material materialInstance = Actor.GetMaterialInstance(this.m_rarityGemMesh.GetComponent<Renderer>());
    materialInstance.mainTextureOffset = offset;
    materialInstance.SetColor("_tint", tint);
  }

  private bool GetRarityTextureOffset(out UnityEngine.Vector2 offset, out Color tint)
  {
    offset = this.GEM_TEXTURE_OFFSET_COMMON;
    tint = this.GEM_COLOR_COMMON;
    if (this.m_entityDef == null && this.m_entity == null || (this.m_entityDef == null ? this.m_entity.GetCardSet() : this.m_entityDef.GetCardSet()) == TAG_CARD_SET.MISSIONS)
      return false;
    switch (this.GetRarity())
    {
      case TAG_RARITY.COMMON:
        offset = this.GEM_TEXTURE_OFFSET_COMMON;
        tint = this.GEM_COLOR_COMMON;
        break;
      case TAG_RARITY.RARE:
        offset = this.GEM_TEXTURE_OFFSET_RARE;
        tint = this.GEM_COLOR_RARE;
        break;
      case TAG_RARITY.EPIC:
        offset = this.GEM_TEXTURE_OFFSET_EPIC;
        tint = this.GEM_COLOR_EPIC;
        break;
      case TAG_RARITY.LEGENDARY:
        offset = this.GEM_TEXTURE_OFFSET_LEGENDARY;
        tint = this.GEM_COLOR_LEGENDARY;
        break;
      default:
        return false;
    }
    return true;
  }

  private void UpdateDescriptionMesh()
  {
    bool flag1 = true;
    if ((UnityEngine.Object) this.m_descriptionMesh != (UnityEngine.Object) null)
    {
      Renderer component = this.m_descriptionMesh.GetComponent<Renderer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.enabled = flag1;
    }
    if ((UnityEngine.Object) this.m_descriptionTrimMesh != (UnityEngine.Object) null)
    {
      Renderer component = this.m_descriptionTrimMesh.GetComponent<Renderer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.enabled = flag1;
    }
    bool flag2 = this.m_entity != null && this.m_entity.IsQuest();
    bool flag3 = this.m_entityDef != null && this.m_entityDef.IsQuest();
    bool flag4 = (GameMgr.Get() == null ? 0 : (GameMgr.Get().IsBattlegrounds() ? 1 : 0)) != 0 && flag2 | flag3;
    if ((UnityEngine.Object) this.m_descriptionMesh != (UnityEngine.Object) null)
      this.m_descriptionMesh.SetActive(!flag4);
    if ((UnityEngine.Object) this.m_baconQuestDescriptionMesh != (UnityEngine.Object) null)
      this.m_baconQuestDescriptionMesh.SetActive(flag4);
    if (!flag1)
      return;
    this.UpdateWatermark();
  }

  private void UpdateWatermark()
  {
    if (this.m_entityDef == null && this.m_entity == null)
      return;
    string assetRef = (string) null;
    EntityDef entityDef = this.m_entityDef ?? this.m_entity.GetEntityDef();
    string watermarkTextureOverride = entityDef.GetWatermarkTextureOverride();
    TAG_CARD_SET cardSetId = this.GetCardSet();
    if (this.m_watermarkCardSetOverride != TAG_CARD_SET.INVALID)
      cardSetId = this.m_watermarkCardSetOverride;
    else if (!string.IsNullOrEmpty(watermarkTextureOverride))
      assetRef = watermarkTextureOverride;
    if (assetRef == null)
    {
      CardSetDbfRecord cardSet = GameDbf.GetIndex().GetCardSet(cardSetId);
      if (cardSet != null)
        assetRef = cardSet.CardWatermarkTexture;
    }
    if (entityDef.IsCoreCard())
      assetRef = SetRotationIcon.GetYearIconWatermark();
    float num = this.m_entityDef != null && this.m_entityDef.HasTag(GAME_TAG.HIDE_WATERMARK) || this.m_entity != null && this.m_entity.HasTag(GAME_TAG.HIDE_WATERMARK) ? 0.0f : this.WATERMARK_ALPHA_VALUE;
    Renderer component1;
    if ((UnityEngine.Object) this.m_descriptionMesh != (UnityEngine.Object) null && this.m_descriptionMesh.TryGetComponent<Renderer>(out component1) && RendererExtension.GetSharedMaterial(component1).HasProperty("_SecondTint") && RendererExtension.GetSharedMaterial(component1).HasProperty("_SecondTex"))
    {
      if (!string.IsNullOrEmpty(assetRef))
      {
        AssetLoader.Get().LoadAsset<Texture>(ref this.m_watermarkTex, (AssetReference) assetRef);
        Actor.GetMaterialInstance(component1).SetTexture("_SecondTex", (Texture) this.m_watermarkTex);
      }
      else
        num = 0.0f;
      Material materialInstance = Actor.GetMaterialInstance(component1);
      materialInstance.SetColor("_SecondTint", materialInstance.GetColor("_SecondTint") with
      {
        a = num
      });
    }
    Renderer component2;
    if (!((UnityEngine.Object) this.m_watermarkMesh != (UnityEngine.Object) null) || !this.m_watermarkMesh.TryGetComponent<Renderer>(out component2) || !RendererExtension.GetSharedMaterial(component2).HasProperty("_Color") || !RendererExtension.GetSharedMaterial(component2).HasProperty("_MainTex"))
      return;
    if (!string.IsNullOrEmpty(assetRef))
    {
      AssetLoader.Get().LoadAsset<Texture>(ref this.m_watermarkTex, (AssetReference) assetRef);
      Actor.GetMaterialInstance(component2).SetTexture("_MainTex", (Texture) this.m_watermarkTex);
    }
    else
      num = 0.0f;
    Material materialInstance1 = Actor.GetMaterialInstance(component2);
    materialInstance1.SetColor("_Color", materialInstance1.GetColor("_Color") with
    {
      a = num
    });
  }

  private void UpdateEliteComponent()
  {
    if ((UnityEngine.Object) this.m_eliteObject == (UnityEngine.Object) null)
      return;
    RenderUtils.EnableRenderers(this.m_eliteObject, this.IsElite(), true);
  }

  private void UpdateManaGemComponent()
  {
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    if (this.GetGameEntityIfAllowed() != null)
    {
      flag1 = GameState.Get().GetBooleanGameOption(GameEntityOption.DISABLE_NONMERC_MANA_GEM);
      flag2 = GameState.Get().GetBooleanGameOption(GameEntityOption.DISABLE_SPELL_MANA_GEM);
      flag3 = GameState.Get().GetBooleanGameOption(GameEntityOption.SHOW_SPEED_WING_ON_ACTOR);
    }
    bool flag4 = false;
    if (this.m_entity != null)
    {
      if (flag1 && this.m_entity.IsMinion() && !this.m_entity.IsLettuceMercenary())
        flag4 = true;
      else if (flag2 && this.m_entity.IsSpell())
        flag4 = true;
    }
    else if (this.m_entityDef != null)
    {
      if (flag1 && this.m_entityDef.IsMinion() && !this.m_entityDef.IsLettuceMercenary())
        flag4 = true;
      else if (flag2 && this.m_entityDef.IsSpell())
        flag4 = true;
    }
    if (this.UseTechLevelManaGem() || this.UseCoinManaGem())
      flag4 = true;
    if (!flag4)
      return;
    if (flag3 && (UnityEngine.Object) this.m_speedWingObject != (UnityEngine.Object) null)
      this.m_speedWingObject.SetActive(true);
    if (!((UnityEngine.Object) this.m_manaObject != (UnityEngine.Object) null))
      return;
    this.m_manaObject.SetActive(false);
  }

  private void UpdateMercenaryRoleComponents()
  {
    if (this.m_mercenaryRoleObjects == null)
      return;
    foreach (MercenaryRoleGemObject mercenaryRoleObject in this.m_mercenaryRoleObjects)
      mercenaryRoleObject.SetRole(this.GetMercenariesRole());
  }

  private TAG_ROLE GetMercenariesRole()
  {
    TAG_ROLE mercenariesRole = TAG_ROLE.INVALID;
    if (this.m_entity != null)
      mercenariesRole = this.m_entity.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    else if (this.m_entityDef != null)
      mercenariesRole = this.m_entityDef.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    return mercenariesRole;
  }

  public void UpdateLettuceMinionInPlayFrame()
  {
    if ((UnityEngine.Object) this.m_lettuceMinionInPlayFrame == (UnityEngine.Object) null)
      return;
    this.UpdateEliteComponent();
    TAG_ROLE role = TAG_ROLE.INVALID;
    if (this.m_entity != null)
      role = this.m_entity.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    else if (this.m_entityDef != null)
      role = this.m_entityDef.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    this.m_lettuceMinionInPlayFrame.UpdateFrameType(role);
  }

  public void UpdateCardRuneBannerComponent()
  {
    if ((UnityEngine.Object) this.m_cardRuneBanner == (UnityEngine.Object) null)
      return;
    this.m_cardRuneBanner.Hide();
    RunePattern runePattern = new RunePattern();
    if (this.m_entity != null && this.m_entity.HasRuneCost)
    {
      runePattern.SetCostsFromEntity((EntityBase) this.m_entity);
      this.m_cardRuneBanner.Show(runePattern);
    }
    else
    {
      if (this.m_entityDef == null || !this.m_entityDef.HasRuneCost)
        return;
      runePattern.SetCostsFromEntity((EntityBase) this.m_entityDef);
      this.m_cardRuneBanner.Show(runePattern);
    }
  }

  public void UpdateDeckRunesComponent(CollectionDeck deck)
  {
    if ((UnityEngine.Object) this.m_deckRunesContainer == (UnityEngine.Object) null)
      return;
    if (deck.HasClass(TAG_CLASS.DEATHKNIGHT))
    {
      this.m_deckRunesContainer.SetActive(true);
      this.m_deckRuneSlotVisual.Show(deck.Runes);
    }
    else
      this.m_deckRunesContainer.SetActive(false);
  }

  private void UpdatePremiumComponents()
  {
    if (this.m_premiumType == TAG_PREMIUM.NORMAL || (UnityEngine.Object) this.m_glints == (UnityEngine.Object) null)
      return;
    this.m_glints.SetActive(true);
    foreach (Renderer componentsInChild in this.m_glints.GetComponentsInChildren<Renderer>())
      componentsInChild.enabled = true;
  }

  private static void OffsetDescriptionTexture(
    GameObject meshObject,
    string textureId,
    bool withRace)
  {
    if (!((UnityEngine.Object) meshObject != (UnityEngine.Object) null))
      return;
    Renderer component = meshObject.GetComponent<Renderer>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    Material materialInstance = Actor.GetMaterialInstance(component);
    if (!((UnityEngine.Object) materialInstance != (UnityEngine.Object) null) || !materialInstance.HasProperty(textureId))
      return;
    float x = materialInstance.GetTextureOffset(textureId).x;
    float y = withRace ? Actor.descriptionMesh_WithRace_TextureOffset : Actor.descriptionMesh_WithoutRace_TextureOffset;
    materialInstance.SetTextureOffset(textureId, new UnityEngine.Vector2(x, y));
  }

  private void UpdateRace(string raceText)
  {
    if (this.m_entityDef == null && this.m_entity == null)
      return;
    bool flag1 = this.m_entity != null ? this.m_entity.IsMinion() : this.m_entityDef.IsMinion();
    bool flag2 = this.m_entity != null ? this.m_entity.IsLocation() : this.m_entityDef.IsLocation();
    bool flag3 = this.m_entity != null ? this.m_entity.IsSpell() : this.m_entityDef.IsSpell();
    bool flag4 = this.m_entity != null ? this.m_entity.IsWeapon() : this.m_entityDef.IsWeapon();
    bool flag5 = this.m_entity != null ? this.m_entity.IsHero() : this.m_entityDef.IsHero();
    bool flag6 = this.m_entity != null ? this.m_entity.IsLettuceAbility() : this.m_entityDef.IsLettuceAbility();
    if (flag1 | flag2 && (UnityEngine.Object) this.m_racePlateObject == (UnityEngine.Object) null || flag5 || flag3 | flag6 && ((UnityEngine.Object) this.m_descriptionMesh == (UnityEngine.Object) null || (UnityEngine.Object) this.m_spellDescriptionMeshNeutral == (UnityEngine.Object) null || (UnityEngine.Object) this.m_spellDescriptionMeshSchool == (UnityEngine.Object) null))
      return;
    bool flag7 = !string.IsNullOrEmpty(raceText);
    int num = 0;
    if (flag7)
      num = !(flag3 | flag6) ? (this.m_entity != null ? this.m_entity.GetRaceCount() : this.m_entityDef.GetRaceCount()) : (this.m_entity == null ? (this.m_entityDef.GetSpellSchool() != TAG_SPELL_SCHOOL.NONE ? 1 : 0) : (this.m_entity.HasTag(GAME_TAG.SPELL_SCHOOL) ? 1 : 0));
    if (flag1 | flag2)
    {
      if ((UnityEngine.Object) this.m_racePlateObject != (UnityEngine.Object) null)
      {
        bool flag8 = flag7 && num == 1;
        foreach (Renderer component in this.m_racePlateObject.GetComponents<MeshRenderer>())
          component.enabled = flag8;
      }
      if ((UnityEngine.Object) this.m_multiRacePlateObject != (UnityEngine.Object) null)
      {
        bool flag9 = flag7 && num > 1;
        this.m_multiRacePlateObject.SetActive(flag9);
        foreach (Renderer componentsInChild in this.m_multiRacePlateObject.GetComponentsInChildren<MeshRenderer>())
          componentsInChild.enabled = flag9;
      }
    }
    else if (flag3 | flag6)
    {
      foreach (MeshFilter component in this.m_descriptionMesh.GetComponents<MeshFilter>())
        component.sharedMesh = !flag7 ? this.m_spellDescriptionMeshNeutral : this.m_spellDescriptionMeshSchool;
    }
    bool withRace = flag7 | flag4 | flag2;
    Actor.OffsetDescriptionTexture(this.m_descriptionMesh, "_SecondTex", withRace);
    Actor.OffsetDescriptionTexture(this.m_watermarkMesh, "_MainTex", withRace);
    if ((UnityEngine.Object) this.m_raceTextMesh == (UnityEngine.Object) null && (UnityEngine.Object) this.m_bgQuestRaceTextMesh == (UnityEngine.Object) null)
      return;
    if (flag7 && (Localization.GetLocale() == Locale.thTH || num > 1))
    {
      if ((UnityEngine.Object) this.m_raceTextMesh != (UnityEngine.Object) null)
      {
        this.m_raceTextMesh.ResizeToFit = false;
        this.m_raceTextMesh.ResizeToFitAndGrow = false;
      }
      if ((UnityEngine.Object) this.m_bgQuestRaceTextMesh != (UnityEngine.Object) null)
      {
        this.m_bgQuestRaceTextMesh.ResizeToFit = false;
        this.m_bgQuestRaceTextMesh.ResizeToFitAndGrow = false;
      }
    }
    if ((UnityEngine.Object) this.m_raceTextMesh != (UnityEngine.Object) null)
    {
      if (num == 1 & flag7)
      {
        this.m_raceTextMesh.gameObject.SetActive(true);
        this.m_raceTextMesh.Text = raceText;
      }
      else
        this.m_raceTextMesh.gameObject.SetActive(false);
    }
    if ((UnityEngine.Object) this.m_multiRaceTextMesh != (UnityEngine.Object) null && num > 1)
    {
      if (num > 1 & flag7)
      {
        this.m_multiRaceTextMesh.gameObject.SetActive(true);
        this.m_multiRaceTextMesh.Text = raceText;
      }
      else
        this.m_multiRaceTextMesh.gameObject.SetActive(false);
    }
    if (!((UnityEngine.Object) this.m_bgQuestRaceTextMesh != (UnityEngine.Object) null))
      return;
    this.m_bgQuestRaceTextMesh.Text = raceText;
  }

  public static Material GetMaterialInstance(Renderer r) => RendererExtension.GetMaterial(r);

  public MultiClassBannerTransition GetMultiClassBanner() => this.m_multiClassBanner;

  public void UpdateCardColor()
  {
    if (this.m_legacyPortraitMaterialIndex < 0 && (UnityEngine.Object) this.m_cardMesh == (UnityEngine.Object) null || this.GetEntityDef() == null && this.GetEntity() == null)
      return;
    bool flag1 = this.IsMultiClass();
    bool flag2 = false;
    bool flag3 = false;
    this.m_usesMultiClassBanner = false;
    bool flag4 = this.IsTradeable();
    bool flag5 = this.IsLocation();
    TAG_CARDTYPE cardType;
    TAG_CLASS tagClass1;
    TAG_ROLE tagRole;
    int num;
    if (this.m_entityDef != null)
    {
      cardType = this.m_entityDef.GetCardType();
      tagClass1 = this.m_entityDef.GetClass();
      tagRole = (TAG_ROLE) this.m_entityDef.GetTag(GAME_TAG.LETTUCE_ROLE);
      num = this.m_entityDef.GetTag(GAME_TAG.MULTI_CLASS_GROUP);
      flag2 = this.m_entityDef.IsLettuceMercenary();
      flag3 = this.m_entityDef.IsLettuceAbilityMinionSummoning();
      this.m_entityDef.GetCardId();
    }
    else if (this.m_entity != null)
    {
      cardType = this.m_entity.GetCardType();
      tagClass1 = this.m_entity.GetClass();
      tagRole = (TAG_ROLE) this.m_entity.GetTag(GAME_TAG.LETTUCE_ROLE);
      num = this.m_entity.GetTag(GAME_TAG.MULTI_CLASS_GROUP);
      flag2 = this.m_entity.IsLettuceMercenary();
      flag3 = this.m_entity.IsLettuceAbilityMinionSummoning();
      this.m_entity.GetCardId();
    }
    else
    {
      cardType = TAG_CARDTYPE.INVALID;
      tagClass1 = TAG_CLASS.INVALID;
      tagRole = TAG_ROLE.INVALID;
      num = 0;
    }
    Color cardColor = Color.magenta;
    CardColorSwitcher.CardColorType colorType = CardColorSwitcher.CardColorType.TYPE_GENERIC;
    if (flag2)
    {
      int premiumType = (int) this.m_premiumType;
      switch (tagRole)
      {
        case TAG_ROLE.CASTER:
          colorType = (CardColorSwitcher.CardColorType) (22 + premiumType);
          break;
        case TAG_ROLE.FIGHTER:
          colorType = (CardColorSwitcher.CardColorType) (25 + premiumType);
          break;
        case TAG_ROLE.TANK:
          colorType = (CardColorSwitcher.CardColorType) (28 + premiumType);
          break;
        case TAG_ROLE.NEUTRAL:
          colorType = (CardColorSwitcher.CardColorType) (31 + premiumType);
          break;
      }
    }
    else if (cardType == TAG_CARDTYPE.LETTUCE_ABILITY)
    {
      switch (tagRole)
      {
        case TAG_ROLE.INVALID:
          return;
        case TAG_ROLE.CASTER:
          colorType = flag3 ? CardColorSwitcher.CardColorType.TYPE_MERCENARIES_NEUTRAL_TIER_2 : CardColorSwitcher.CardColorType.TYPE_MERCENARIES_NEUTRAL_TIER_1;
          break;
        case TAG_ROLE.FIGHTER:
          colorType = flag3 ? CardColorSwitcher.CardColorType.TYPE_MERCENARIES_ABILITY_FIGHTER_MINION : CardColorSwitcher.CardColorType.TYPE_MERCENARIES_NEUTRAL_TIER_3;
          break;
        case TAG_ROLE.TANK:
          colorType = flag3 ? CardColorSwitcher.CardColorType.TYPE_MERCENARIES_ABILITY_TANK_MINION : CardColorSwitcher.CardColorType.TYPE_MERCENARIES_ABILITY_TANK_SPELL;
          break;
        case TAG_ROLE.NEUTRAL:
          colorType = flag3 ? CardColorSwitcher.CardColorType.TYPE_MERCENARIES_ABILITY_NEUTRAL_MINION : CardColorSwitcher.CardColorType.TYPE_MERCENARIES_ABILITY_NEUTRAL_SPELL;
          break;
      }
    }
    else if (flag5)
    {
      switch (tagClass1)
      {
        case TAG_CLASS.DEATHKNIGHT:
          colorType = CardColorSwitcher.CardColorType.TYPE_DEATHKNIGHT;
          cardColor = this.CLASS_COLOR_LOCATION_DEATHKNIGHT;
          break;
        case TAG_CLASS.DRUID:
          colorType = CardColorSwitcher.CardColorType.TYPE_DRUID;
          cardColor = this.CLASS_COLOR_LOCATION_DRUID;
          break;
        case TAG_CLASS.HUNTER:
          colorType = CardColorSwitcher.CardColorType.TYPE_HUNTER;
          cardColor = this.CLASS_COLOR_LOCATION_HUNTER;
          break;
        case TAG_CLASS.MAGE:
          colorType = CardColorSwitcher.CardColorType.TYPE_MAGE;
          cardColor = this.CLASS_COLOR_LOCATION_MAGE;
          break;
        case TAG_CLASS.PALADIN:
          colorType = CardColorSwitcher.CardColorType.TYPE_PALADIN;
          cardColor = this.CLASS_COLOR_LOCATION_PALADIN;
          break;
        case TAG_CLASS.PRIEST:
          colorType = CardColorSwitcher.CardColorType.TYPE_PRIEST;
          cardColor = this.CLASS_COLOR_LOCATION_PRIEST;
          break;
        case TAG_CLASS.ROGUE:
          colorType = CardColorSwitcher.CardColorType.TYPE_ROGUE;
          cardColor = this.CLASS_COLOR_LOCATION_ROGUE;
          break;
        case TAG_CLASS.SHAMAN:
          colorType = CardColorSwitcher.CardColorType.TYPE_SHAMAN;
          cardColor = this.CLASS_COLOR_LOCATION_SHAMAN;
          break;
        case TAG_CLASS.WARLOCK:
          colorType = CardColorSwitcher.CardColorType.TYPE_WARLOCK;
          cardColor = this.CLASS_COLOR_LOCATION_WARLOCK;
          break;
        case TAG_CLASS.WARRIOR:
          colorType = CardColorSwitcher.CardColorType.TYPE_WARRIOR;
          cardColor = this.CLASS_COLOR_LOCATION_WARRIOR;
          break;
        case TAG_CLASS.DREAM:
          colorType = CardColorSwitcher.CardColorType.TYPE_HUNTER;
          cardColor = this.CLASS_COLOR_LOCATION_HUNTER;
          break;
        case TAG_CLASS.DEMONHUNTER:
          colorType = CardColorSwitcher.CardColorType.TYPE_DEMONHUNTER;
          cardColor = this.CLASS_COLOR_LOCATION_DEMONHUNTER;
          break;
        default:
          colorType = CardColorSwitcher.CardColorType.TYPE_GENERIC;
          cardColor = this.CLASS_COLOR_LOCATION_GENERIC;
          break;
      }
    }
    else
    {
      switch (tagClass1)
      {
        case TAG_CLASS.DEATHKNIGHT:
          colorType = CardColorSwitcher.CardColorType.TYPE_DEATHKNIGHT;
          cardColor = this.CLASS_COLOR_DEATHKNIGHT;
          break;
        case TAG_CLASS.DRUID:
          colorType = CardColorSwitcher.CardColorType.TYPE_DRUID;
          cardColor = this.CLASS_COLOR_DRUID;
          break;
        case TAG_CLASS.HUNTER:
          colorType = CardColorSwitcher.CardColorType.TYPE_HUNTER;
          cardColor = this.CLASS_COLOR_HUNTER;
          break;
        case TAG_CLASS.MAGE:
          colorType = CardColorSwitcher.CardColorType.TYPE_MAGE;
          cardColor = this.CLASS_COLOR_MAGE;
          break;
        case TAG_CLASS.PALADIN:
          colorType = CardColorSwitcher.CardColorType.TYPE_PALADIN;
          cardColor = this.CLASS_COLOR_PALADIN;
          break;
        case TAG_CLASS.PRIEST:
          colorType = CardColorSwitcher.CardColorType.TYPE_PRIEST;
          cardColor = this.CLASS_COLOR_PRIEST;
          break;
        case TAG_CLASS.ROGUE:
          colorType = CardColorSwitcher.CardColorType.TYPE_ROGUE;
          cardColor = this.CLASS_COLOR_ROGUE;
          break;
        case TAG_CLASS.SHAMAN:
          colorType = CardColorSwitcher.CardColorType.TYPE_SHAMAN;
          cardColor = this.CLASS_COLOR_SHAMAN;
          break;
        case TAG_CLASS.WARLOCK:
          colorType = CardColorSwitcher.CardColorType.TYPE_WARLOCK;
          cardColor = this.CLASS_COLOR_WARLOCK;
          break;
        case TAG_CLASS.WARRIOR:
          colorType = CardColorSwitcher.CardColorType.TYPE_WARRIOR;
          cardColor = this.CLASS_COLOR_WARRIOR;
          break;
        case TAG_CLASS.DREAM:
          colorType = CardColorSwitcher.CardColorType.TYPE_HUNTER;
          cardColor = this.CLASS_COLOR_HUNTER;
          break;
        case TAG_CLASS.DEMONHUNTER:
          colorType = CardColorSwitcher.CardColorType.TYPE_DEMONHUNTER;
          cardColor = this.CLASS_COLOR_DEMONHUNTER;
          break;
        default:
          colorType = CardColorSwitcher.CardColorType.TYPE_GENERIC;
          cardColor = this.CLASS_COLOR_GENERIC;
          break;
      }
    }
    if (flag1)
    {
      colorType = CardColorSwitcher.CardColorType.TYPE_GENERIC;
      MultiClassGroupDbfRecord record = GameDbf.MultiClassGroup.GetRecord(num);
      if (record != null)
        colorType = (CardColorSwitcher.CardColorType) record.CardColorType;
      if (record != null && !string.IsNullOrEmpty(record.IconAssetPath) && (UnityEngine.Object) this.m_multiClassBannerContainer != (UnityEngine.Object) null)
      {
        this.m_usesMultiClassBanner = true;
        this.m_multiClassBannerContainer.gameObject.SetActive(true);
        this.m_multiClassBanner = this.m_multiClassBannerContainer.PrefabGameObject(true).GetComponent<MultiClassBannerTransition>();
        if ((UnityEngine.Object) this.m_multiClassBanner != (UnityEngine.Object) null)
        {
          List<TAG_CLASS> classes = new List<TAG_CLASS>();
          if (this.m_entityDef != null)
          {
            this.m_entityDef.GetClasses((IList<TAG_CLASS>) classes);
            classes.Sort(new Comparison<TAG_CLASS>(MultiClassBannerTransition.CompareClasses));
          }
          else if (this.m_entity != null)
          {
            this.m_entity.GetClasses((IList<TAG_CLASS>) classes);
            classes.Sort(new Comparison<TAG_CLASS>(MultiClassBannerTransition.CompareClasses));
            if (this.m_entity.GetZone() == TAG_ZONE.HAND && !this.m_entity.IsHistoryDupe())
            {
              foreach (TAG_CLASS tagClass2 in classes)
              {
                if (tagClass2 == this.m_entity.GetHero().GetClass())
                {
                  classes.Clear();
                  classes.Add(tagClass2);
                  break;
                }
              }
            }
          }
          this.m_multiClassBanner.SetClasses((IEnumerable<TAG_CLASS>) classes);
          this.m_multiClassBanner.SetMultiClassGroup(num);
          if (this.m_premiumType >= TAG_PREMIUM.GOLDEN)
            this.m_multiClassBanner.SetGoldenCardMesh(this.m_cardMesh, this.m_premiumRibbon);
        }
      }
    }
    else
    {
      if (this.m_premiumRibbon > -1 && (UnityEngine.Object) this.m_initialPremiumRibbonMaterial != (UnityEngine.Object) null)
      {
        Renderer component = this.m_cardMesh.GetComponent<Renderer>();
        if (this.m_premiumRibbon < RendererExtension.GetMaterials(component).Count)
          RendererExtension.SetMaterial(component, this.m_premiumRibbon, this.m_initialPremiumRibbonMaterial);
      }
      if ((UnityEngine.Object) this.m_multiClassBannerContainer != (UnityEngine.Object) null)
        this.m_multiClassBannerContainer.gameObject.SetActive(false);
    }
    if (flag4)
    {
      if ((UnityEngine.Object) this.m_tradeableBannerContainer != (UnityEngine.Object) null)
      {
        this.m_tradeableBannerContainer.gameObject.SetActive(true);
        this.m_tradeableBanner = this.m_tradeableBannerContainer.PrefabGameObject(true).GetComponent<TradeableBanner>();
      }
    }
    else if ((UnityEngine.Object) this.m_tradeableBannerContainer != (UnityEngine.Object) null)
    {
      this.m_tradeableBannerContainer.gameObject.SetActive(false);
      this.m_tradeableBanner = (TradeableBanner) null;
    }
    if (flag2 && this.m_premiumType != TAG_PREMIUM.NORMAL)
      this.SetMaterialNormal(cardType, colorType, cardColor);
    this.SetMaterial(cardType, colorType, cardColor);
  }

  public void UpdateManaGemOffset()
  {
    if (!((UnityEngine.Object) this.m_manaObject != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_costTextMesh != (UnityEngine.Object) null))
      return;
    int num1 = this.IsMultiClass() ? 1 : 0;
    bool flag1 = this.IsTradeable();
    bool flag2 = false;
    if (this.m_entity != null && this.m_entity.GetController() != null)
      flag2 = ((this.m_entity.HasTag(GAME_TAG.CARD_COSTS_HEALTH) ? 1 : 0) | (!this.m_entity.GetController().HasTag(GAME_TAG.SPELLS_COST_HEALTH) ? (false ? 1 : 0) : (this.m_entity.IsSpell() ? 1 : 0))) != 0;
    Vector3 localPosition1 = this.m_manaObject.transform.localPosition;
    Vector3 localPosition2 = this.m_costTextMesh.transform.localPosition;
    int num2 = flag1 ? 1 : 0;
    if ((num1 | num2) != 0)
    {
      localPosition1.y = 0.027f;
      localPosition2.y = 0.088f;
    }
    else if (flag2)
    {
      localPosition1.y = 0.087f;
      localPosition2.y = 0.148f;
    }
    this.m_manaObject.transform.localPosition = localPosition1;
    this.m_costTextMesh.transform.localPosition = localPosition2;
  }

  public void SetTradeableHighlightState(TradeableHighlightState state)
  {
    if (!((UnityEngine.Object) this.m_tradeableBanner != (UnityEngine.Object) null))
      return;
    this.m_tradeableBanner.SetHighlightState(state);
  }

  private void SetMaterial(
    TAG_CARDTYPE cardType,
    CardColorSwitcher.CardColorType colorType,
    Color cardColor)
  {
    switch (this.m_premiumType)
    {
      case TAG_PREMIUM.NORMAL:
        this.SetMaterialNormal(cardType, colorType, cardColor);
        break;
      case TAG_PREMIUM.GOLDEN:
      case TAG_PREMIUM.DIAMOND:
      case TAG_PREMIUM.SIGNATURE:
        this.SetMaterialPremium(cardType, colorType, cardColor);
        break;
      default:
        Debug.LogWarning((object) string.Format("Actor.SetMaterial(): unexpected premium type {0}", (object) this.m_premiumType));
        break;
    }
  }

  private void SetHistoryHeroBannerColor()
  {
    if (this.m_entity == null || this.m_entity.IsControlledByFriendlySidePlayer() || !this.m_entity.IsHistoryDupe())
      return;
    Transform transform = this.GetRootObject().transform.Find("History_Hero_Banner");
    if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      return;
    Actor.GetMaterialInstance(transform.GetComponent<Renderer>()).mainTextureOffset = new UnityEngine.Vector2(0.005f, -0.505f);
  }

  private void GetDualClassColors(
    CardColorSwitcher.CardColorType dualClassCombo,
    out Color left,
    out Color right)
  {
    switch (dualClassCombo)
    {
      case CardColorSwitcher.CardColorType.TYPE_PALADIN_PRIEST:
        left = this.CLASS_COLOR_PALADIN;
        right = this.CLASS_COLOR_PRIEST;
        break;
      case CardColorSwitcher.CardColorType.TYPE_WARLOCK_PRIEST:
        left = this.CLASS_COLOR_PRIEST;
        right = this.CLASS_COLOR_WARLOCK;
        break;
      case CardColorSwitcher.CardColorType.TYPE_WARLOCK_DEMONHUNTER:
        left = this.CLASS_COLOR_WARLOCK;
        right = this.CLASS_COLOR_DEMONHUNTER;
        break;
      case CardColorSwitcher.CardColorType.TYPE_HUNTER_DEMONHUNTER:
        left = this.CLASS_COLOR_DEMONHUNTER;
        right = this.CLASS_COLOR_HUNTER;
        break;
      case CardColorSwitcher.CardColorType.TYPE_DRUID_HUNTER:
        left = this.CLASS_COLOR_HUNTER;
        right = this.CLASS_COLOR_DRUID;
        break;
      case CardColorSwitcher.CardColorType.TYPE_DRUID_SHAMAN:
        left = this.CLASS_COLOR_DRUID;
        right = this.CLASS_COLOR_SHAMAN;
        break;
      case CardColorSwitcher.CardColorType.TYPE_SHAMAN_MAGE:
        left = this.CLASS_COLOR_SHAMAN;
        right = this.CLASS_COLOR_MAGE;
        break;
      case CardColorSwitcher.CardColorType.TYPE_MAGE_ROGUE:
        left = this.CLASS_COLOR_MAGE;
        right = this.CLASS_COLOR_ROGUE;
        break;
      case CardColorSwitcher.CardColorType.TYPE_WARRIOR_ROGUE:
        left = this.CLASS_COLOR_ROGUE;
        right = this.CLASS_COLOR_WARRIOR;
        break;
      case CardColorSwitcher.CardColorType.TYPE_WARRIOR_PALADIN:
        left = this.CLASS_COLOR_WARRIOR;
        right = this.CLASS_COLOR_PALADIN;
        break;
      default:
        left = right = Color.magenta;
        break;
    }
  }

  private void SetMaterialPremium(
    TAG_CARDTYPE cardType,
    CardColorSwitcher.CardColorType colorType,
    Color cardColor)
  {
    if ((UnityEngine.Object) this.m_cardMesh != (UnityEngine.Object) null && this.m_premiumRibbon >= 0)
    {
      Material material = RendererExtension.GetMaterial(this.m_cardMesh.GetComponent<Renderer>(), this.m_premiumRibbon);
      if (colorType >= CardColorSwitcher.CardColorType.TYPE_GENERIC && colorType <= CardColorSwitcher.CardColorType.TYPE_DEMONHUNTER)
      {
        material.color = cardColor;
        material.SetFloat("_EnableDualClass", 0.0f);
      }
      else
      {
        Color left;
        Color right;
        this.GetDualClassColors(colorType, out left, out right);
        material.SetFloat("_EnableDualClass", 1f);
        material.SetColor("_Color", left);
        material.SetColor("_SecondColor", right);
      }
    }
    if (cardType != TAG_CARDTYPE.HERO)
      return;
    this.SetHistoryHeroBannerColor();
  }

  private void SetMaterialNormal(
    TAG_CARDTYPE cardType,
    CardColorSwitcher.CardColorType colorType,
    Color cardColor)
  {
    switch (cardType)
    {
      case TAG_CARDTYPE.HERO:
        this.SetMaterialHero(colorType);
        break;
      case TAG_CARDTYPE.MINION:
        this.SetMaterialWithTexture(cardType, colorType);
        break;
      case TAG_CARDTYPE.SPELL:
        this.SetMaterialWithTexture(cardType, colorType);
        break;
      case TAG_CARDTYPE.WEAPON:
        this.SetMaterialWeapon(colorType, cardColor);
        break;
      case TAG_CARDTYPE.LETTUCE_ABILITY:
        this.SetMaterialWithTexture(cardType, colorType);
        break;
      case TAG_CARDTYPE.LOCATION:
        this.SetMaterialWithTexture(cardType, colorType);
        break;
    }
  }

  protected virtual void SetMaterialWithTexture(
    TAG_CARDTYPE cardType,
    CardColorSwitcher.CardColorType colorType)
  {
    if ((UnityEngine.Object) CardColorSwitcher.Get() == (UnityEngine.Object) null)
      return;
    AssetLoader.Get().LoadAsset<Texture>(ref this.m_cardColorTex, CardColorSwitcher.Get().GetTexture(cardType, colorType));
    if ((bool) (UnityEngine.Object) this.m_cardMesh)
    {
      if (this.m_cardFrontMatIdx > -1)
        RendererExtension.GetMaterial(this.m_cardMesh.GetComponent<Renderer>(), this.m_cardFrontMatIdx).mainTexture = (Texture) this.m_cardColorTex;
      switch (cardType)
      {
        case TAG_CARDTYPE.SPELL:
          if (!(bool) (UnityEngine.Object) this.m_portraitMesh || this.m_portraitFrameMatIdx <= -1)
            break;
          RendererExtension.GetMaterial(this.m_portraitMesh.GetComponent<Renderer>(), this.m_portraitFrameMatIdx).mainTexture = (Texture) this.m_cardColorTex;
          break;
        case TAG_CARDTYPE.WEAPON:
          if (colorType != CardColorSwitcher.CardColorType.TYPE_DEATHKNIGHT)
            break;
          goto case TAG_CARDTYPE.SPELL;
      }
    }
    else
    {
      if (this.m_legacyCardColorMaterialIndex < 0 || !((UnityEngine.Object) this.m_meshRenderer != (UnityEngine.Object) null))
        return;
      RendererExtension.GetMaterial((Renderer) this.m_meshRenderer, this.m_legacyCardColorMaterialIndex).mainTexture = (Texture) this.m_cardColorTex;
    }
  }

  private void SetMaterialHero(CardColorSwitcher.CardColorType colorType)
  {
    this.SetMaterialWithTexture(TAG_CARDTYPE.HERO, colorType);
    this.SetHistoryHeroBannerColor();
  }

  private void SetMaterialWeapon(CardColorSwitcher.CardColorType colorType, Color cardColor)
  {
    if ((bool) (UnityEngine.Object) CardColorSwitcher.Get() && !string.IsNullOrEmpty((string) CardColorSwitcher.Get().GetTexture(TAG_CARDTYPE.WEAPON, colorType)))
    {
      this.SetMaterialWithTexture(TAG_CARDTYPE.WEAPON, colorType);
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this.m_descriptionTrimMesh)
        return;
      Actor.GetMaterialInstance(this.m_descriptionTrimMesh.GetComponent<Renderer>()).SetColor("_Color", cardColor);
    }
  }

  public bool UseTechLevelManaGem()
  {
    if (this.m_entity != null && !this.m_entity.IsMinion() || this.m_entityDef != null && !this.m_entityDef.IsMinion())
      return false;
    GameEntity gameEntityIfAllowed = this.GetGameEntityIfAllowed();
    return gameEntityIfAllowed != null && gameEntityIfAllowed.HasTag(GAME_TAG.TECH_LEVEL_MANA_GEM);
  }

  public bool UseCoinManaGem()
  {
    GameEntity gameEntityIfAllowed = this.GetGameEntityIfAllowed();
    return gameEntityIfAllowed != null && gameEntityIfAllowed.HasTag(GAME_TAG.COIN_MANA_GEM);
  }

  public bool UseCoinManaGemForChoiceCard()
  {
    GameEntity gameEntityIfAllowed = this.GetGameEntityIfAllowed();
    if (gameEntityIfAllowed == null || !gameEntityIfAllowed.HasTag(GAME_TAG.COIN_MANA_GEM_FOR_CHOICE_CARDS))
      return false;
    return !GameMgr.Get().IsBattlegrounds() || this.m_entity == null || !this.m_entity.IsQuest();
  }

  public HistoryCard GetHistoryCard() => (UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null ? (HistoryCard) null : this.transform.parent.gameObject.GetComponent<HistoryCard>();

  public HistoryChildCard GetHistoryChildCard() => (UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null ? (HistoryChildCard) null : this.transform.parent.gameObject.GetComponent<HistoryChildCard>();

  public void ConfigureForHistory(HistoryItem item)
  {
    this.transform.parent = item.transform;
    TransformUtil.Identity((Component) this.transform);
    if ((UnityEngine.Object) this.m_rootObject != (UnityEngine.Object) null)
      TransformUtil.Identity((Component) this.m_rootObject.transform);
    if ((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null)
    {
      foreach (SpellTableEntry spellTableEntry in this.m_localSpellTable.m_Table)
      {
        Spell spell = spellTableEntry.m_Spell;
        if (!((UnityEngine.Object) spell == (UnityEngine.Object) null))
          spell.m_BlockServerEvents = false;
      }
    }
    this.TurnOffCollider();
    this.SetActorState(ActorStateType.CARD_HISTORY);
  }

  public void SetHistoryItem(HistoryItem card)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
    {
      this.transform.parent = (Transform) null;
    }
    else
    {
      this.transform.parent = card.transform;
      TransformUtil.Identity((Component) this.transform);
      if ((UnityEngine.Object) this.m_rootObject != (UnityEngine.Object) null)
        TransformUtil.Identity((Component) this.m_rootObject.transform);
      this.m_entity = card.GetEntity();
      this.UpdateTextComponents(this.m_entity);
      this.UpdateMeshComponents();
      if (this.m_premiumType >= TAG_PREMIUM.GOLDEN && (UnityEngine.Object) card.GetPortraitGoldenMaterial() != (UnityEngine.Object) null && this.IsPremiumPortraitEnabled())
        this.SetPortraitMaterial(card.GetPortraitGoldenMaterial());
      else
        this.SetPortraitTextureOverride(card.GetPortraitTexture());
      if (!((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null))
        return;
      foreach (SpellTableEntry spellTableEntry in this.m_localSpellTable.m_Table)
      {
        Spell spell = spellTableEntry.m_Spell;
        if (!((UnityEngine.Object) spell == (UnityEngine.Object) null))
          spell.m_BlockServerEvents = false;
      }
    }
  }

  public SpellTable GetSpellTable() => this.m_localSpellTable;

  public Spell LoadSpell(SpellType spellType)
  {
    Spell spell = (Spell) null;
    if ((UnityEngine.Object) this.m_card != (UnityEngine.Object) null)
      spell = this.m_card.GetSpellTableOverride(spellType);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      CardDef cardDef = this.m_cardDefHandle.Get(this.m_premiumType);
      if ((UnityEngine.Object) cardDef != (UnityEngine.Object) null)
      {
        foreach (SpellTableOverride spellTableOverride in cardDef.m_SpellTableOverrides)
        {
          if (spellTableOverride.m_Type == spellType)
          {
            if (!string.IsNullOrEmpty(spellTableOverride.m_SpellPrefabName))
            {
              spell = SpellManager.Get().GetSpell(spellTableOverride.m_SpellPrefabName);
              if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
              {
                spell.SetSpellType(spellType);
                break;
              }
              break;
            }
            break;
          }
        }
      }
    }
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      TAG_CARD_SET cardSet = this.GetCardSet();
      string setSpellOverride = GameDbf.GetIndex().GetCardSetSpellOverride(cardSet, spellType);
      if (!string.IsNullOrEmpty(setSpellOverride))
      {
        spell = SpellManager.Get().GetSpell(setSpellOverride);
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
          spell.SetSpellType(spellType);
      }
    }
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null && (UnityEngine.Object) this.m_sharedSpellTable != (UnityEngine.Object) null)
      spell = this.m_sharedSpellTable.GetSpellInstance(spellType);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return (Spell) null;
    if (this.m_ownedSpells.ContainsKey(spellType))
      this.m_ownedSpells.Remove(spellType);
    this.m_ownedSpells.Add(spellType, spell);
    Transform transform = spell.gameObject.transform;
    TransformUtil.AttachAndPreserveLocalTransform(transform, this.GetSpellParent());
    transform.localScale.Scale(this.m_sharedSpellTable.gameObject.transform.localScale);
    LayerUtils.SetLayer(spell.gameObject, (GameLayer) this.gameObject.layer);
    spell.AddSpellReleasedCallback(new Spell.SpellReleasedCallback(this.OnSpellRelease));
    spell.OnLoad();
    if ((UnityEngine.Object) this.m_actorStateMgr != (UnityEngine.Object) null)
      spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
    return spell;
  }

  private void OnSpellRelease(Spell spell)
  {
    this.m_ownedSpells.Remove(spell.GetSpellType());
    spell.RemoveSpellReleasedCallback(new Spell.SpellReleasedCallback(this.OnSpellRelease));
  }

  public Spell GetLoadedSpell(SpellType spellType)
  {
    Spell loadedSpell = (Spell) null;
    if (this.m_ownedSpells != null)
      this.m_ownedSpells.TryGetValue(spellType, out loadedSpell);
    if ((UnityEngine.Object) loadedSpell == (UnityEngine.Object) null)
      loadedSpell = this.LoadSpell(spellType);
    return loadedSpell;
  }

  public void ActivateTaunt()
  {
    this.DeactivateTaunt();
    if (this.GetEntity().IsStealthed() && !Options.Get().GetBool(Option.HAS_SEEN_STEALTH_TAUNTER, false))
    {
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_STEALTH_TAUNT3_22"), "VO_INNKEEPER_STEALTH_TAUNT3_22.prefab:7ec7cc35d1556434ebca64bfe4e770cb");
      Options.Get().SetBool(Option.HAS_SEEN_STEALTH_TAUNTER, true);
    }
    bool flag = this.GetEntity().IsStealthed() || this.GetEntity().IsTauntIgnored();
    switch (this.m_premiumType)
    {
      case TAG_PREMIUM.GOLDEN:
        if (flag)
        {
          this.ActivateSpellBirthState(SpellType.TAUNT_PREMIUM_STEALTH);
          break;
        }
        this.ActivateSpellBirthState(SpellType.TAUNT_PREMIUM);
        break;
      case TAG_PREMIUM.DIAMOND:
        if (flag)
        {
          this.ActivateSpellBirthState(SpellType.TAUNT_DIAMOND_STEALTH);
          break;
        }
        this.ActivateSpellBirthState(SpellType.TAUNT_DIAMOND);
        break;
      default:
        if (flag)
        {
          this.ActivateSpellBirthState(SpellType.TAUNT_STEALTH);
          break;
        }
        this.ActivateSpellBirthState(SpellType.TAUNT);
        break;
    }
  }

  public void DeactivateTaunt()
  {
    if (this.IsSpellActive(SpellType.TAUNT))
      this.ActivateSpellDeathState(SpellType.TAUNT);
    if (this.IsSpellActive(SpellType.TAUNT_PREMIUM))
      this.ActivateSpellDeathState(SpellType.TAUNT_PREMIUM);
    if (this.IsSpellActive(SpellType.TAUNT_PREMIUM_STEALTH))
      this.ActivateSpellDeathState(SpellType.TAUNT_PREMIUM_STEALTH);
    if (this.IsSpellActive(SpellType.TAUNT_STEALTH))
      this.ActivateSpellDeathState(SpellType.TAUNT_STEALTH);
    if (this.IsSpellActive(SpellType.TAUNT_DIAMOND))
      this.ActivateSpellDeathState(SpellType.TAUNT_DIAMOND);
    if (!this.IsSpellActive(SpellType.TAUNT_DIAMOND_STEALTH))
      return;
    this.ActivateSpellDeathState(SpellType.TAUNT_DIAMOND_STEALTH);
  }

  public Spell ActivateEvilTwinMustache()
  {
    this.DeactivateEvilTwinMustache();
    return this.m_premiumType == TAG_PREMIUM.GOLDEN ? this.ActivateSpellBirthState(SpellType.EVIL_TWIN_MUSTACHE_PREMIUM) : this.ActivateSpellBirthState(SpellType.EVIL_TWIN_MUSTACHE);
  }

  public void DeactivateEvilTwinMustache()
  {
    if (this.IsSpellActive(SpellType.EVIL_TWIN_MUSTACHE))
      this.ActivateSpellDeathState(SpellType.EVIL_TWIN_MUSTACHE);
    if (!this.IsSpellActive(SpellType.EVIL_TWIN_MUSTACHE_PREMIUM))
      return;
    this.ActivateSpellDeathState(SpellType.EVIL_TWIN_MUSTACHE_PREMIUM);
  }

  public Spell GetSpell(SpellType spellType)
  {
    Spell spell = (Spell) null;
    if (this.m_useSharedSpellTable)
      spell = this.GetLoadedSpell(spellType);
    else if ((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null)
      spell = this.m_localSpellTable.GetLocalSpell(spellType);
    return spell;
  }

  public Spell GetSpellIfLoaded(SpellType spellType)
  {
    Spell result = (Spell) null;
    if (this.m_useSharedSpellTable)
      this.GetSpellIfLoaded(spellType, out result);
    else if ((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null)
      result = this.m_localSpellTable.GetLocalSpell(spellType);
    return result;
  }

  public bool GetSpellIfLoaded(SpellType spellType, out Spell result)
  {
    if (this.m_ownedSpells == null || !this.m_ownedSpells.ContainsKey(spellType))
    {
      result = (Spell) null;
      return false;
    }
    result = this.m_ownedSpells[spellType];
    return (UnityEngine.Object) result != (UnityEngine.Object) null;
  }

  public Spell ActivateSpellBirthState(SpellType spellType)
  {
    Spell spell = this.GetSpell(spellType);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return (Spell) null;
    spell.ActivateState(SpellStateType.BIRTH);
    return spell;
  }

  public bool IsSpellActive(SpellType spellType)
  {
    Spell spellIfLoaded = this.GetSpellIfLoaded(spellType);
    return !((UnityEngine.Object) spellIfLoaded == (UnityEngine.Object) null) && spellIfLoaded.IsActive();
  }

  public void ActivateSpellDeathState(SpellType spellType)
  {
    Spell spellIfLoaded = this.GetSpellIfLoaded(spellType);
    if ((UnityEngine.Object) spellIfLoaded == (UnityEngine.Object) null)
      return;
    spellIfLoaded.ActivateState(SpellStateType.DEATH);
  }

  public void ActivateSpellCancelState(SpellType spellType)
  {
    Spell spellIfLoaded = this.GetSpellIfLoaded(spellType);
    if ((UnityEngine.Object) spellIfLoaded == (UnityEngine.Object) null)
      return;
    spellIfLoaded.ActivateState(SpellStateType.CANCEL);
  }

  public void ActivateAllSpellsDeathStates()
  {
    if (this.m_useSharedSpellTable)
    {
      foreach (Spell spell in this.m_ownedSpells.Values)
      {
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null && spell.IsActive())
          spell.ActivateState(SpellStateType.DEATH);
      }
    }
    else
    {
      if (!((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null))
        return;
      foreach (SpellTableEntry spellTableEntry in this.m_localSpellTable.m_Table)
      {
        Spell spell = spellTableEntry.m_Spell;
        if (!((UnityEngine.Object) spell == (UnityEngine.Object) null) && spell.IsActive())
          spell.ActivateState(SpellStateType.DEATH);
      }
    }
  }

  public void DoCardDeathVisuals()
  {
    foreach (SpellType spellType in Enum.GetValues(typeof (SpellType)))
    {
      if (this.IsSpellActive(spellType) && spellType != SpellType.GHOSTLY_DEATH && spellType != SpellType.DEATH && spellType != SpellType.DEATHRATTLE_DEATH && spellType != SpellType.REBORN_DEATH && spellType != SpellType.DAMAGE)
        this.ActivateSpellDeathState(spellType);
    }
  }

  public void DeactivateAllSpells()
  {
    if (this.m_useSharedSpellTable)
    {
      foreach (SpellType key in new List<SpellType>((IEnumerable<SpellType>) this.m_ownedSpells.Keys))
      {
        Spell ownedSpell = this.m_ownedSpells[key];
        if ((UnityEngine.Object) ownedSpell != (UnityEngine.Object) null)
          ownedSpell.Deactivate();
      }
    }
    else
    {
      if (!((UnityEngine.Object) this.m_localSpellTable != (UnityEngine.Object) null))
        return;
      foreach (SpellTableEntry spellTableEntry in this.m_localSpellTable.m_Table)
      {
        Spell spell = spellTableEntry.m_Spell;
        if (!((UnityEngine.Object) spell == (UnityEngine.Object) null))
          spell.Deactivate();
      }
    }
  }

  public void ReleaseSpell(SpellType spellType)
  {
    if (this.m_useSharedSpellTable)
    {
      Spell spell;
      if (!this.m_ownedSpells.TryGetValue(spellType, out spell))
        return;
      SpellManager.Get().ReleaseSpell(spell);
      this.m_ownedSpells.Remove(spellType);
    }
    else
      Debug.LogError((object) string.Format("Actor.DestroySpell() - FAILED to destroy {0} because the Actor is not using a shared spell table.", (object) spellType));
  }

  public void DisableArmorSpellForTransition() => this.m_armorSpellDisabledForTransition = true;

  public void EnableArmorSpellAfterTransition() => this.m_armorSpellDisabledForTransition = false;

  public void HideArmorSpell()
  {
    if (!((UnityEngine.Object) this.m_armorSpell != (UnityEngine.Object) null))
      return;
    this.m_armorSpell.SetArmor(0);
    this.m_armorSpell.Deactivate();
    this.m_armorSpell.gameObject.SetActive(false);
  }

  public void ShowArmorSpell()
  {
    if (!((UnityEngine.Object) this.m_armorSpell != (UnityEngine.Object) null) || this.m_armorSpellDisabledForTransition)
      return;
    this.m_armorSpell.gameObject.SetActive(true);
    this.UpdateArmorSpell(this.m_updateTokenSource.Token);
  }

  public void HideTavernTierSpell() => this.ReleaseSpell(SpellType.TECH_LEVEL_MANA_GEM);

  public void HideCoinManaGem() => this.ReleaseSpell(SpellType.COIN_MANA_GEM);

  public void ShowTavernTierSpell()
  {
    Spell spell = this.GetSpell(SpellType.TECH_LEVEL_MANA_GEM);
    int techLevel = this.GetEntityDef().GetTechLevel();
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null))
      return;
    spell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("TechLevel").Value = techLevel;
    spell.ActivateState(SpellStateType.BIRTH);
  }

  public void ShowCoinManaGem()
  {
    Spell spell = this.GetSpell(SpellType.COIN_MANA_GEM);
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null))
      return;
    spell.ActivateState(SpellStateType.BIRTH);
  }

  private void UpdateRootObjectSpellComponents()
  {
    if (this.m_entity == null)
      return;
    if (this.m_armorSpellLoading)
      this.UpdateArmorSpellWhenLoaded(this.m_updateTokenSource.Token).Forget();
    if (!((UnityEngine.Object) this.m_armorSpell != (UnityEngine.Object) null))
      return;
    this.UpdateArmorSpell(this.m_updateTokenSource.Token);
  }

  private async UniTaskVoid UpdateArmorSpellWhenLoaded(CancellationToken token)
  {
    while (this.m_armorSpellLoading)
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    this.UpdateArmorSpell(token);
  }

  private void UpdateArmorSpell(CancellationToken token)
  {
    if (!this.m_armorSpell.gameObject.activeInHierarchy || this.m_entity == null)
      return;
    int armor1 = this.m_entity.GetArmor();
    int armor2 = this.m_armorSpell.GetArmor();
    this.m_armorSpell.SetArmor(armor1);
    if (armor1 > 0)
    {
      bool flag = this.m_armorSpell.IsShown();
      if (!flag)
        this.m_armorSpell.Show();
      if (armor2 <= 0)
        this.ActivateArmorSpell(SpellStateType.BIRTH, true, token).Forget();
      else if (armor2 > armor1)
        this.ActivateArmorSpell(SpellStateType.ACTION, true, token).Forget();
      else if (armor2 < armor1)
      {
        this.ActivateArmorSpell(SpellStateType.CANCEL, true, token).Forget();
      }
      else
      {
        if (flag)
          return;
        this.ActivateArmorSpell(SpellStateType.IDLE, true, token).Forget();
      }
    }
    else
    {
      if (armor2 <= 0)
        return;
      this.ActivateArmorSpell(SpellStateType.DEATH, false, token).Forget();
    }
  }

  private async UniTaskVoid ActivateArmorSpell(
    SpellStateType stateType,
    bool armorShouldBeOn,
    CancellationToken token)
  {
    while (this.m_armorSpell.GetActiveState() != SpellStateType.NONE)
      await UniTask.Yield(PlayerLoopTiming.Update, token);
    if (this.m_armorSpell.GetActiveState() == stateType)
      return;
    int armor = this.m_entity.GetArmor();
    if (armorShouldBeOn && armor <= 0 || !armorShouldBeOn && armor > 0)
      return;
    this.m_armorSpell.ActivateState(stateType);
  }

  public void SetArmorSpellState(SpellStateType stateType)
  {
    if ((UnityEngine.Object) this.m_armorSpell == (UnityEngine.Object) null)
      return;
    this.m_armorSpell.ActivateState(stateType);
  }

  private void OnSpellStateStarted(Spell spell, SpellStateType prevStateType, object userData)
  {
    spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
    this.m_actorStateMgr.RefreshStateMgr();
    if (!(bool) (UnityEngine.Object) this.m_projectedShadow)
      return;
    this.m_projectedShadow.UpdateContactShadow();
  }

  private void AssignRootObject() => this.m_rootObject = GameObjectUtils.FindChildBySubstring(this.gameObject, "RootObject");

  private void AssignBones() => this.m_bones = GameObjectUtils.FindChildBySubstring(this.gameObject, "Bones");

  private void AssignMeshRenderers()
  {
    foreach (MeshRenderer componentsInChild1 in this.gameObject.GetComponentsInChildren<MeshRenderer>(true))
    {
      if (componentsInChild1.gameObject.name.Equals("Mesh", StringComparison.OrdinalIgnoreCase))
      {
        this.m_meshRenderer = componentsInChild1;
        foreach (MeshRenderer componentsInChild2 in componentsInChild1.gameObject.GetComponentsInChildren<MeshRenderer>(true))
          this.AssignMaterials(componentsInChild2);
        break;
      }
    }
    if (!((UnityEngine.Object) this.m_portraitMesh != (UnityEngine.Object) null))
      return;
    this.m_meshRendererPortrait = this.m_portraitMesh.GetComponent<MeshRenderer>();
  }

  private void AssignMaterials(MeshRenderer meshRenderer)
  {
    List<Material> sharedMaterials = RendererExtension.GetSharedMaterials((Renderer) meshRenderer);
    for (int index = 0; index < sharedMaterials.Count; ++index)
    {
      Material material = sharedMaterials[index];
      if (!((UnityEngine.Object) material == (UnityEngine.Object) null))
      {
        if (material.name.LastIndexOf("Portrait", StringComparison.OrdinalIgnoreCase) >= 0)
          this.m_legacyPortraitMaterialIndex = index;
        else if (material.name.IndexOf("Card_Inhand_Ability_Warlock", StringComparison.OrdinalIgnoreCase) >= 0)
          this.m_legacyCardColorMaterialIndex = index;
        else if (material.name.IndexOf("Card_Inhand_Warlock", StringComparison.OrdinalIgnoreCase) >= 0)
          this.m_legacyCardColorMaterialIndex = index;
        else if (material.name.IndexOf("Card_Inhand_Weapon_Warlock", StringComparison.OrdinalIgnoreCase) >= 0)
          this.m_legacyCardColorMaterialIndex = index;
      }
    }
  }

  public void ShowSideQuestProgressBanner()
  {
    this.ResetBanner();
    if (this.m_entity == null || (UnityEngine.Object) this.m_banner == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerBottom == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerText == (UnityEngine.Object) null)
      return;
    this.m_banner.SetActive(true);
    this.m_bannerBottom.SetActive(true);
    this.m_bannerText.gameObject.SetActive(true);
    this.m_bannerText.Text = GameStrings.Format("GLUE_SIDEQUEST_PROGRESS_BANNER", (object) this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS), (object) this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL));
  }

  public void ShowObjectiveProgressBanner()
  {
    this.ResetBanner();
    if (this.m_entity == null || (UnityEngine.Object) this.m_banner == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerBottom == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerText == (UnityEngine.Object) null)
      return;
    this.m_banner.SetActive(true);
    this.m_bannerBottom.SetActive(true);
    this.m_bannerText.gameObject.SetActive(true);
    int num = this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL) - this.m_entity.GetTag(GAME_TAG.QUEST_PROGRESS);
    if (num == 1)
      this.m_bannerText.Text = GameStrings.Format("GLUE_OBJECTIVES_BANNER_FINAL_TURN", (object) num);
    else
      this.m_bannerText.Text = GameStrings.Format("GLUE_OBJECTIVES_BANNER", (object) num);
  }

  public void HideSideQuestProgressBanner()
  {
    if ((UnityEngine.Object) this.m_banner == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerBottom == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerText == (UnityEngine.Object) null)
      return;
    this.m_banner.SetActive(false);
    this.m_bannerBottom.SetActive(false);
    this.m_bannerText.gameObject.SetActive(false);
  }

  private void AssignSpells()
  {
    this.m_localSpellTable = this.gameObject.GetComponentInChildren<SpellTable>();
    this.m_actorStateMgr = this.gameObject.GetComponentInChildren<ActorStateMgr>();
    if ((UnityEngine.Object) this.m_localSpellTable == (UnityEngine.Object) null)
    {
      if (string.IsNullOrEmpty(this.m_spellTablePrefab))
        return;
      SpellManager spellManager = SpellManager.Get();
      if (spellManager != null)
      {
        SpellTable spellTable = spellManager.GetSpellTable(this.m_spellTablePrefab);
        if ((UnityEngine.Object) spellTable != (UnityEngine.Object) null)
        {
          this.m_useSharedSpellTable = true;
          this.m_sharedSpellTable = spellTable;
          this.m_ownedSpells = new Dictionary<SpellType, Spell>();
        }
        else
          Debug.LogWarning((object) ("failed to load spell table: " + this.m_spellTablePrefab));
      }
      else
        Debug.LogWarning((object) ("Null spell cache: " + this.m_spellTablePrefab));
    }
    else
    {
      if (!((UnityEngine.Object) this.m_actorStateMgr != (UnityEngine.Object) null))
        return;
      foreach (SpellTableEntry spellTableEntry in this.m_localSpellTable.m_Table)
      {
        if (!((UnityEngine.Object) spellTableEntry.m_Spell == (UnityEngine.Object) null))
          spellTableEntry.m_Spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
      }
    }
  }

  private Transform GetSpellParent()
  {
    if ((UnityEngine.Object) this.m_spellsParent != (UnityEngine.Object) null)
      return this.m_spellsParent;
    this.m_spellsParent = new GameObject("Spells").transform;
    this.m_spellsParent.parent = this.gameObject.transform;
    this.m_spellsParent.localPosition = Vector3.zero;
    this.m_spellsParent.localScale = Vector3.one;
    this.m_spellsParent.localRotation = Quaternion.identity;
    return this.m_spellsParent;
  }

  private void SetUpBanner()
  {
    if ((UnityEngine.Object) this.m_banner == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerBottom == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerText == (UnityEngine.Object) null)
      return;
    this.ResetBanner();
  }

  private void ResetBanner()
  {
    if ((UnityEngine.Object) this.m_banner == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerBottom == (UnityEngine.Object) null || (UnityEngine.Object) this.m_bannerText == (UnityEngine.Object) null)
      return;
    this.m_banner.SetActive(false);
    this.m_bannerBottom.SetActive(false);
    this.m_bannerText.gameObject.SetActive(false);
    this.m_banner.transform.parent = this.transform;
    this.m_bannerBottom.transform.parent = this.transform;
    this.m_bannerText.transform.parent = this.transform;
  }

  private void CacheShadowObjects()
  {
    List<GameObject> childrenByTag1 = GameObjectUtils.FindChildrenByTag(this.gameObject, "FakeShadow", true);
    List<GameObject> childrenByTag2 = GameObjectUtils.FindChildrenByTag(this.gameObject, "FakeShadowUnique", true);
    this.AddToContactShadowList(childrenByTag1, false);
    this.AddToContactShadowList(childrenByTag2, true);
    this.m_shadowObjectInitialized = true;
  }

  private void AddToContactShadowList(List<GameObject> shadowObjects, bool isUnique)
  {
    if (shadowObjects != null && shadowObjects.Count > 0 && this.m_contactShadows == null)
      this.m_contactShadows = new List<Actor.ContactShadowData>();
    if (shadowObjects == null)
      return;
    foreach (GameObject shadowObject in shadowObjects)
    {
      Renderer component = shadowObject.GetComponent<Renderer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        this.m_contactShadows.Add(new Actor.ContactShadowData(isUnique, shadowObject, RendererExtension.GetMaterial(component).renderQueue, component.transform.position - this.transform.position));
    }
  }

  private void LoadArmorSpell()
  {
    if ((UnityEngine.Object) this.m_armorSpellBone == (UnityEngine.Object) null)
      return;
    this.m_armorSpellLoading = true;
    string assetRef = "Hero_Armor.prefab:e4d519d1080fe4656967bf5140ca3587";
    CardDef cardDef = this.m_cardDefHandle.Get(this.m_premiumType);
    if ((UnityEngine.Object) cardDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(cardDef.m_CustomHeroArmorSpell))
      assetRef = cardDef.m_CustomHeroArmorSpell;
    AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, new PrefabCallback<GameObject>(this.OnArmorSpellLoaded));
  }

  private void OnArmorSpellLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0} - Actor.OnArmorSpellLoaded() - failed to load Hero_Armor spell! m_armorSpell GameObject = null!", (object) assetRef));
    }
    else
    {
      this.m_armorSpellLoading = false;
      this.m_armorSpell = go.GetComponent<ArmorSpell>();
      if ((UnityEngine.Object) this.m_armorSpell == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("{0} - Actor.OnArmorSpellLoaded() - failed to load Hero_Armor spell! m_armorSpell Spell = null!", (object) assetRef));
      }
      else
      {
        go.transform.parent = this.m_armorSpellBone.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
      }
    }
  }

  public void LoadCustomFrame(CardDef cardDef)
  {
    if ((UnityEngine.Object) cardDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(cardDef.m_CustomHeroFramePrefab))
    {
      AssetReference customHeroFramePrefab = (AssetReference) cardDef.m_CustomHeroFramePrefab;
      if (this.m_customFrameController == null || this.m_customFrameController.FrameAssetReference != customHeroFramePrefab)
      {
        this.UnloadCustomFrame();
        IAssetLoader assetLoader = AssetLoader.Get();
        if (assetLoader == null)
          return;
        using (AssetHandle<GameObject> instantiateSharedPrefab = assetLoader.GetOrInstantiateSharedPrefab(customHeroFramePrefab))
          this.OnCustomFrameLoaded(customHeroFramePrefab, instantiateSharedPrefab, (object) null);
      }
      else
      {
        if (this.m_customFrameController == null)
          return;
        this.ApplyCustomFrame();
      }
    }
    else
      this.UnloadCustomFrame();
  }

  private void UnloadCustomFrame()
  {
    if (this.m_customFrameController != null)
    {
      this.m_customFrameController.RestoreInitialPortraitMaterial(ref this.m_initialPortraitMaterial);
      this.m_customFrameController.RestoreMeshAndMaterials(ref this.m_portraitMesh, ref this.m_portraitMatIdx, ref this.m_portraitFrameMatIdx);
    }
    this.UpdateMaterials();
    if ((UnityEngine.Object) this.m_decorationRoot != (UnityEngine.Object) null)
      this.m_decorationRoot.transform.localPosition = Vector3.zero;
    this.ZoneHeroPositionOffset = 0.0f;
    if ((UnityEngine.Object) this.m_projectedShadow != (UnityEngine.Object) null)
      this.m_projectedShadow.m_AutoDisableHeight = this.m_cachedProjectedShadowAutoDisableHeight;
    Actor.CustomFrameChangedEventHandler customFrameChanged = this.OnCustomFrameChanged;
    if (customFrameChanged == null)
      return;
    customFrameChanged((CustomFrameController) null);
  }

  private void DestroyCustomFrame()
  {
    this.UnloadCustomFrame();
    if (this.m_customFrameController == null)
      return;
    ((IDisposable) this.m_customFrameController).Dispose();
    this.m_customFrameController = (CustomFrameController) null;
  }

  private void ApplyCustomFrame()
  {
    if (this.m_customFrameController == null)
      return;
    this.m_customFrameController.ApplyCustomMeshAndMaterials(out this.m_portraitMesh);
    this.m_portraitMatIdx = this.m_customFrameController.PortraitMatIdx;
    this.m_portraitFrameMatIdx = this.m_customFrameController.FrameMatIdx;
    this.m_initialPortraitMaterial = this.m_portraitMatIdx < 0 ? (Material) null : RendererExtension.GetSharedMaterial(this.m_portraitMesh.GetComponent<Renderer>(), this.m_portraitMatIdx);
    if ((UnityEngine.Object) this.m_decorationRoot != (UnityEngine.Object) null)
      this.m_decorationRoot.transform.localPosition = new Vector3(0.0f, this.m_customFrameController.DecorationRootOffset, 0.0f);
    this.ZoneHeroPositionOffset = this.m_customFrameController.HeroZonePositionOffset;
    if ((UnityEngine.Object) this.m_projectedShadow != (UnityEngine.Object) null)
      this.m_projectedShadow.m_AutoDisableHeight = this.m_cachedProjectedShadowAutoDisableHeight + this.ZoneHeroPositionOffset;
    this.UpdateMaterials();
    this.SetPortraitMaterial(this.m_initialPortraitMaterial);
    this.UpdatePortraitTexture();
    if (this.isMissingCard())
      this.MissingCardEffect();
    if ((bool) (UnityEngine.Object) this.m_card)
      this.m_card.GetZone()?.UpdateLayout();
    Actor.CustomFrameChangedEventHandler customFrameChanged = this.OnCustomFrameChanged;
    if (customFrameChanged == null)
      return;
    customFrameChanged(this.m_customFrameController);
  }

  private void OnCustomFrameLoaded(
    AssetReference assetRef,
    AssetHandle<GameObject> go,
    object callbackData)
  {
    using (go)
    {
      if (go == null || (UnityEngine.Object) go.Asset == (UnityEngine.Object) null)
        Debug.LogError((object) string.Format("{0} - Actor.OnCustomFrameLoaded() - failed to load Hero_Armor spell! m_armorSpell GameObject = null!", (object) assetRef));
      else if ((UnityEngine.Object) go.Asset.GetComponent<CustomFrameDef>() == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("{0} - Actor.OnCustomFrameLoaded() - failed to load Hero_Armor spell! m_armorSpell CustomFrameDef = null!", (object) assetRef));
      }
      else
      {
        if (this.m_customFrameController == null)
          this.m_customFrameController = new CustomFrameController(this.m_portraitMesh, this.m_portraitMatIdx, this.m_portraitFrameMatIdx);
        this.m_customFrameController.SetAssetHandle(assetRef, go);
        this.m_customFrameController.CacheHighlightState(this.GetComponentInChildren<HighlightState>());
        this.m_customFrameController.CacheInitialPortraitMaterial(this.m_initialPortraitMaterial);
        this.ApplyCustomFrame();
      }
    }
  }

  private void ConnectLegendarySkinToDynamicResolutionController()
  {
    if (this.m_customFrameController == null)
      return;
    LegendarySkinDynamicResController resolutionController = this.m_customFrameController.DynamicResolutionController;
    if (this.LegendaryHeroPortrait != null)
      this.LegendaryHeroPortrait.ConnectDynamicResolutionController(resolutionController);
    else
      resolutionController.Skin = (LegendarySkin) null;
  }

  private void DisconnectLegendarySkinToDynamicResolutionController()
  {
    if (this.m_customFrameController == null)
      return;
    LegendarySkinDynamicResController resolutionController = this.m_customFrameController.DynamicResolutionController;
    if (!((UnityEngine.Object) resolutionController != (UnityEngine.Object) null))
      return;
    resolutionController.Skin = (LegendarySkin) null;
  }

  public void AddCustomFrameCallback(Actor.CustomFrameChangedEventHandler eventHandler)
  {
    if (eventHandler != null)
      eventHandler(this.m_customFrameController);
    this.OnCustomFrameChanged += eventHandler;
  }

  public bool HasCardDef => (UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType) != (UnityEngine.Object) null;

  public bool HasSameCardDef(CardDef cardDef) => (UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType) == (UnityEngine.Object) cardDef;

  public string CardDefName => !this.HasCardDef ? (string) null : this.m_cardDefHandle.Get(this.m_premiumType).name;

  public Material DeckCardBarPortrait => !this.HasCardDef ? (Material) null : this.m_cardDefHandle.Get(this.m_premiumType).GetDeckCardBarPortrait(this.m_premiumType);

  public Texture PortraitTexture => !this.HasCardDef ? (Texture) null : this.m_cardDefHandle.Get(this.m_premiumType).GetPortraitTexture(this.m_premiumType);

  public Material PremiumPortraitMaterial => !this.HasCardDef ? (Material) null : this.m_cardDefHandle.Get(this.m_premiumType).GetPremiumPortraitMaterial();

  public UberShaderAnimation PremiumPortraitAnimation => !this.HasCardDef ? (UberShaderAnimation) null : this.m_cardDefHandle.Get(this.m_premiumType).GetPremiumPortraitAnimation();

  public CardPortraitQuality CardPortraitQuality => CardPortraitQuality.GetFromDef(this.m_cardDefHandle.Get(this.m_premiumType));

  public CardEffectDef PlayEffectDef => !this.HasCardDef ? (CardEffectDef) null : this.m_cardDefHandle.Get(this.m_premiumType).m_PlayEffectDef;

  public bool PremiumAnimationAvailable => CardTextureLoader.PremiumAnimationAvailable(this.m_cardDefHandle.Get(this.m_premiumType));

  public string SocketInEffectFriendly => !this.HasCardDef ? (string) null : this.m_cardDefHandle.Get(this.m_premiumType).m_SocketInEffectFriendly;

  public string SocketInEffectFriendlyPhone => !this.HasCardDef ? (string) null : this.m_cardDefHandle.Get(this.m_premiumType).m_SocketInEffectFriendlyPhone;

  public string SocketInEffectOpponent => !this.HasCardDef ? (string) null : this.m_cardDefHandle.Get(this.m_premiumType).m_SocketInEffectOpponent;

  public string SocketInEffectOpponentPhone => !this.HasCardDef ? (string) null : this.m_cardDefHandle.Get(this.m_premiumType).m_SocketInEffectOpponentPhone;

  public bool SocketInOverrideHeroAnimation => this.HasCardDef && this.m_cardDefHandle.Get(this.m_premiumType).m_SocketInOverrideHeroAnimation;

  public bool SocketInParentEffectToHero => this.HasCardDef && this.m_cardDefHandle.Get(this.m_premiumType).m_SocketInParentEffectToHero;

  public List<EmoteEntryDef> EmoteDefs => !this.HasCardDef ? (List<EmoteEntryDef>) null : this.m_cardDefHandle.Get(this.m_premiumType).m_EmoteDefs;

  public bool AlwaysRenderPremiumPortrait
  {
    get => this.m_cardDefHandle != null && (UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType) != (UnityEngine.Object) null && this.m_cardDefHandle.Get(this.m_premiumType).m_AlwaysRenderPremiumPortrait;
    set
    {
      if (this.m_cardDefHandle == null || !((UnityEngine.Object) this.m_cardDefHandle.Get(this.m_premiumType) != (UnityEngine.Object) null))
        return;
      this.m_cardDefHandle.Get(this.m_premiumType).m_AlwaysRenderPremiumPortrait = value;
    }
  }

  public bool HasSignaturePortraitTexture()
  {
    if (this.m_cardDefHandle == null)
      return false;
    CardDef cardDef = this.m_cardDefHandle.Get(this.m_premiumType);
    return !((UnityEngine.Object) cardDef == (UnityEngine.Object) null) && !string.IsNullOrEmpty(cardDef.m_SignaturePortraitTexturePath);
  }

  public CardSilhouetteOverride CardSilhouetteOverride => !this.HasCardDef ? CardSilhouetteOverride.None : this.m_cardDefHandle.Get(this.m_premiumType).m_CardSilhouetteOverride;

  public BaconLHSConfig LegendaryHeroSkinConfig => !this.HasCardDef ? (BaconLHSConfig) null : this.m_cardDefHandle.Get(this.m_premiumType).m_LegendaryHeroSkinConfig;

  public CardRuneBanner GetRuneBanner() => this.m_cardRuneBanner;

  [Serializable]
  public class FactionObject
  {
    public TAG_LETTUCE_FACTION m_faction;
    public GameObject m_banner;
  }

  protected struct ContactShadowData
  {
    public bool IsUnique { get; private set; }

    public GameObject ShadowObject { get; private set; }

    public int InitialRenderQueue { get; private set; }

    public Vector3 InitialPositionRelativeToActor { get; private set; }

    public ContactShadowData(
      bool isUnique,
      GameObject shadowObject,
      int initialRenderQueue,
      Vector3 initialRelativeToActor)
    {
      this.IsUnique = isUnique;
      this.ShadowObject = shadowObject;
      this.InitialRenderQueue = initialRenderQueue;
      this.InitialPositionRelativeToActor = initialRelativeToActor;
    }
  }

  public delegate void CustomFrameChangedEventHandler(CustomFrameController customFrameController);

  private enum PortraitMode
  {
    Default,
    ForcedPlayMode,
    ForcedHandMode,
  }
}
