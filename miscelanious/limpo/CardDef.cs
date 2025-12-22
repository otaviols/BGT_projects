using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class CardDef : MonoBehaviour
{
  [CustomEditField(Sections = "Portrait", T = EditType.ARTBUNDLE)]
  public Portrait m_Portrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_PortraitTexturePath;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.MATERIAL)]
  public string m_PremiumPortraitMaterialPath;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.UBERANIMATION)]
  public string m_PremiumUberShaderAnimationPath;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_PremiumPortraitTexturePath;
  [CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_SignaturePortraitTexturePath;
  [CustomEditField(Sections = "Portrait", T = EditType.MATERIAL)]
  public string m_SignaturePortraitMaterialPath;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.MESH)]
  public string m_DiamondPlaneRTT_Hand;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.MESH)]
  public string m_DiamondPlaneRTT_Play;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.MESH)]
  public string m_DiamondBackground_Hand;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.MESH)]
  public string m_DiamondBackground_Play;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Color m_DiamondPlaneRTT_CearColor = Color.clear;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_DiamondPortraitTexturePath;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.GAME_OBJECT)]
  public string m_DiamondModel;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.GAME_OBJECT)]
  public string m_LegendaryModel;
  [CustomEditField(Sections = "Portrait")]
  public int m_PreferredActorPortraitIndex;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_DeckCardBarPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_SignatureDeckCardBarPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_EnchantmentPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_HistoryTileHalfPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_HistoryTileFullPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_HistoryTileFullSignaturePortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_HistoryTileHalfSignaturePortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_LeaderboardTileFullPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material_MobileOverride m_CustomDeckPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material_MobileOverride m_DeckPickerPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_BattlegroundHeroBuddyPortraitTexturePath;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.MATERIAL)]
  public Material m_BattlegroundHeroBuddyPortraitMaterial;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait", T = EditType.MATERIAL)]
  public Material m_BattlegroundsQuestRewardsMaterial;
  [CustomEditField(Sections = "Portrait", T = EditType.TEXTURE)]
  public string m_CustomRenderDisplayOverride;
  [CustomEditField(Sections = "Portrait")]
  public Material m_LockedClassPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_PracticeAIPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_DeckBoxPortrait;
  [CustomEditField(Hide = true, HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_MercenaryBarPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_MercenaryCoinPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_MercenaryMapBossCoinPortrait;
  [CustomEditField(Hide = true, HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public Material m_TeamTray;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public bool m_AlwaysRenderPremiumPortrait;
  [CustomEditField(HidePredicate = "HideIfPortrait", Sections = "Portrait")]
  public CardSilhouetteOverride m_CardSilhouetteOverride;
  [CustomEditField(Sections = "Portrait")]
  public GameObject m_FrameMeshOverride;
  [CustomEditField(Sections = "Play")]
  public CardEffectDef m_PlayEffectDef;
  [CustomEditField(Sections = "Play")]
  public List<CardEffectDef> m_AdditionalPlayEffectDefs;
  [CustomEditField(Sections = "Attack")]
  public CardEffectDef m_AttackEffectDef;
  [CustomEditField(Sections = "Death")]
  public CardEffectDef m_DeathEffectDef;
  [CustomEditField(Sections = "Lifetime")]
  public CardEffectDef m_LifetimeEffectDef;
  [CustomEditField(Sections = "Trigger")]
  public List<CardEffectDef> m_TriggerEffectDefs;
  [CustomEditField(Sections = "SubOption")]
  public List<CardEffectDef> m_SubOptionEffectDefs;
  [CustomEditField(Sections = "SubOption")]
  public List<List<CardEffectDef>> m_AdditionalSubOptionEffectDefs;
  [CustomEditField(Sections = "ResetGame")]
  public List<CardEffectDef> m_ResetGameEffectDefs;
  [CustomEditField(Sections = "Sub-Spells")]
  public List<CardEffectDef> m_SubSpellEffectDefs;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_CustomSummonSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_GoldenCustomSummonSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_DiamondCustomSummonSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_CustomSpawnSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_GoldenCustomSpawnSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_DiamondCustomSpawnSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_CustomDeathSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_GoldenCustomDeathSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_DiamondCustomDeathSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_CustomDiscardSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_GoldenCustomDiscardSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_DiamondCustomDiscardSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_CustomKeywordSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_CustomChoiceRevealSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public string m_CustomChoiceConcealSpellPath;
  [CustomEditField(Sections = "Custom", T = EditType.SPELL)]
  public List<SpellTableOverride> m_SpellTableOverrides;
  [CustomEditField(Sections = "Hero", T = EditType.GAME_OBJECT)]
  public string m_CollectionHeroDefPath;
  [CustomEditField(Sections = "Hero", T = EditType.SPELL)]
  public string m_CustomHeroArmorSpell;
  [CustomEditField(Sections = "Hero", T = EditType.SPELL)]
  public string m_SocketInEffectFriendly;
  [CustomEditField(Sections = "Hero", T = EditType.SPELL)]
  public string m_SocketInEffectOpponent;
  [CustomEditField(Sections = "Hero", T = EditType.SPELL)]
  public string m_SocketInEffectFriendlyPhone;
  [CustomEditField(Sections = "Hero", T = EditType.SPELL)]
  public string m_SocketInEffectOpponentPhone;
  [CustomEditField(Sections = "Hero")]
  public bool m_SocketInOverrideHeroAnimation;
  [CustomEditField(Sections = "Hero")]
  public bool m_SocketInParentEffectToHero = true;
  [CustomEditField(Sections = "Hero", T = EditType.TEXTURE)]
  public string m_CustomHeroTray;
  [CustomEditField(Sections = "Hero", T = EditType.TEXTURE)]
  public string m_CustomHeroTrayGolden;
  [CustomEditField(Sections = "Hero")]
  public bool m_DisablePremiumHeroTray;
  [CustomEditField(Sections = "Hero")]
  public List<Board.CustomTraySettings> m_CustomHeroTraySettings;
  [CustomEditField(Sections = "Hero", T = EditType.TEXTURE)]
  public string m_CustomHeroPhoneTray;
  [CustomEditField(Sections = "Hero", T = EditType.TEXTURE)]
  public string m_CustomHeroPhoneManaGem;
  [CustomEditField(Sections = "Hero", T = EditType.SOUND_PREFAB)]
  public string m_AnnouncerLinePath;
  [CustomEditField(Sections = "Hero", T = EditType.SOUND_PREFAB)]
  public string m_AnnouncerLineBeforeVersusPath;
  [CustomEditField(Sections = "Hero", T = EditType.SOUND_PREFAB)]
  public string m_AnnouncerLineAfterVersusPath;
  [CustomEditField(Sections = "Hero", T = EditType.SOUND_PREFAB)]
  public string m_HeroPickerSelectedPrefab;
  [CustomEditField(Sections = "Hero")]
  public List<EmoteEntryDef> m_EmoteDefs;
  [CustomEditField(Sections = "Hero")]
  public BaconLHSConfig m_LegendaryHeroSkinConfig;
  [CustomEditField(Sections = "Misc", T = EditType.GAME_OBJECT)]
  public string m_StoreItemDisplayPath;
  [CustomEditField(Sections = "HeroFrame", T = EditType.GAME_OBJECT)]
  public string m_CustomHeroFramePrefab;
  [CustomEditField(Sections = "HeroFrame", T = EditType.GAME_OBJECT)]
  public string m_CustomHeroInfoFramePrefab;
  [CustomEditField(Sections = "Misc")]
  public bool m_SuppressDeathrattleDeath;
  [CustomEditField(Sections = "Misc")]
  public bool m_SuppressPlaySoundsOnSummon;
  [CustomEditField(Sections = "Misc")]
  public bool m_SuppressPlaySoundsDuringMulligan;
  [CustomEditField(Sections = "Special Events")]
  public List<CardDefSpecialEvent> m_SpecialEvents;
  private static IMaterialService s_materialService;
  private Material m_LoadedPremiumPortraitMaterial;
  private Material m_LoadedPremiumClassMaterial;
  private Material m_LoadedDeckCardBarPortrait;
  private Material m_LoadedSignatureDeckCardBarPortrait;
  private Material m_LoadedEnchantmentPortrait;
  private Material m_LoadedHistoryTileFullPortrait;
  private Material m_LoadedHistoryTileHalfPortrait;
  private Material m_LoadedHistoryTileFullSignaturePortrait;
  private Material m_LoadedHistoryTileHalfSignaturePortrait;
  private Material m_LoadedLeaderboardTileFullPortrait;
  private Material m_LoadedCustomDeckPortrait;
  private Material m_LoadedDeckPickerPortrait;
  private Material m_LoadedPracticeAIPortrait;
  private Material m_LoadedDeckBoxPortrait;
  private Material m_LoadedSignaturePortraitMaterial;
  private CardPortraitQuality m_portraitQuality = CardPortraitQuality.GetUnloaded();
  private CardDefSpecialEvent m_currentSpecialEvent;
  private AssetHandle<Texture> m_LoadedPortraitTexture;
  private AssetHandle<Texture> m_loadedPremiumPortraitTexture;
  private AssetHandle<Texture> m_loadedSignaturePortraitTexture;
  private AssetHandle<Material> m_premiumMaterialHandle;
  private AssetHandle<Material> m_signatureMaterialHandle;
  private AssetHandle<UberShaderAnimation> m_premiumPortraitAnimation;
  private AssetHandle<Texture> m_lowQualityPortrait;
  private AssetHandle<Texture> m_LoadedBattlegroundHeroBuddyPortraitTexture;
  protected const int LARGE_MINION_COST = 7;
  protected const int MEDIUM_MINION_COST = 4;

  public bool HideIfPortrait() => (Object) this.m_Portrait != (Object) null;

  public string PortraitTexturePath => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_PortraitTexturePath : this.m_PortraitTexturePath;

  public string GoldenPortraitMaterialPath => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_PremiumPortraitMaterialPath : this.m_PremiumPortraitMaterialPath;

  public string GoldenUberShaderAnimationPath => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_PremiumUberShaderAnimationPath : this.m_PremiumUberShaderAnimationPath;

  public string SignaturePortraitMaterialPath => this.m_SignaturePortraitMaterialPath;

  public void Awake()
  {
    if (string.IsNullOrEmpty(this.m_PortraitTexturePath))
    {
      this.m_portraitQuality.TextureQuality = 3;
      this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
    }
    else
    {
      if (!string.IsNullOrEmpty(this.m_PremiumPortraitMaterialPath))
        return;
      this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
    }
  }

  public virtual string DetermineActorPathForZone(Entity entity, TAG_ZONE zoneTag) => ActorNames.GetZoneActor(entity, zoneTag);

  public void OnDestroy()
  {
    if ((Object) this.m_Portrait != (Object) null)
      Object.Destroy((Object) this.m_Portrait);
    if ((bool) (Object) this.m_LoadedPremiumPortraitMaterial)
      Object.Destroy((Object) this.m_LoadedPremiumPortraitMaterial);
    if ((bool) (Object) this.m_LoadedSignaturePortraitMaterial)
      Object.Destroy((Object) this.m_LoadedSignaturePortraitMaterial);
    if ((bool) (Object) this.m_LoadedPremiumClassMaterial)
      Object.Destroy((Object) this.m_LoadedPremiumClassMaterial);
    if ((bool) (Object) this.m_LoadedDeckCardBarPortrait)
      Object.Destroy((Object) this.m_LoadedDeckCardBarPortrait);
    if ((bool) (Object) this.m_LoadedSignatureDeckCardBarPortrait)
      Object.Destroy((Object) this.m_LoadedSignatureDeckCardBarPortrait);
    if ((bool) (Object) this.m_LoadedEnchantmentPortrait)
      Object.Destroy((Object) this.m_LoadedEnchantmentPortrait);
    if ((bool) (Object) this.m_LoadedHistoryTileFullPortrait)
      Object.Destroy((Object) this.m_LoadedHistoryTileFullPortrait);
    if ((bool) (Object) this.m_LoadedHistoryTileHalfPortrait)
      Object.Destroy((Object) this.m_LoadedHistoryTileHalfPortrait);
    if ((bool) (Object) this.m_LoadedHistoryTileFullSignaturePortrait)
      Object.Destroy((Object) this.m_LoadedHistoryTileFullSignaturePortrait);
    if ((bool) (Object) this.m_LoadedHistoryTileHalfSignaturePortrait)
      Object.Destroy((Object) this.m_LoadedHistoryTileHalfSignaturePortrait);
    if ((bool) (Object) this.m_LoadedLeaderboardTileFullPortrait)
      Object.Destroy((Object) this.m_LoadedLeaderboardTileFullPortrait);
    if ((bool) (Object) this.m_LoadedCustomDeckPortrait)
      Object.Destroy((Object) this.m_LoadedCustomDeckPortrait);
    if ((bool) (Object) this.m_LoadedDeckPickerPortrait)
      Object.Destroy((Object) this.m_LoadedDeckPickerPortrait);
    if ((bool) (Object) this.m_LoadedPracticeAIPortrait)
      Object.Destroy((Object) this.m_LoadedPracticeAIPortrait);
    if ((bool) (Object) this.m_LoadedDeckBoxPortrait)
      Object.Destroy((Object) this.m_LoadedDeckBoxPortrait);
    AssetHandle.SafeDispose<Texture>(ref this.m_LoadedPortraitTexture);
    AssetHandle.SafeDispose<UberShaderAnimation>(ref this.m_premiumPortraitAnimation);
    AssetHandle.SafeDispose<Material>(ref this.m_premiumMaterialHandle);
    AssetHandle.SafeDispose<Material>(ref this.m_signatureMaterialHandle);
    AssetHandle.SafeDispose<Texture>(ref this.m_loadedPremiumPortraitTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_loadedSignaturePortraitTexture);
    AssetHandle.SafeDispose<Texture>(ref this.m_lowQualityPortrait);
    AssetHandle.SafeDispose<Texture>(ref this.m_LoadedBattlegroundHeroBuddyPortraitTexture);
  }

  public virtual SpellType DetermineSummonInSpell_HandToPlay(
    Card card,
    bool useFastAnimations)
  {
    Entity entity = card.GetEntity();
    if (entity.IsHero())
      return SpellType.SUMMON_IN_HERO;
    switch (entity.GetTag(GAME_TAG.LETTUCE_ROLE))
    {
      case 1:
        return SpellType.LETTUCE_COME_IN_PLAY_CASTER;
      case 2:
        return SpellType.LETTUCE_COME_IN_PLAY_FIGHTER;
      case 3:
        return SpellType.LETTUCE_COME_IN_PLAY_PROTECTOR;
      default:
        int cost = entity.GetEntityDef().GetCost();
        TAG_PREMIUM premiumType = entity.GetPremiumType();
        bool flag = entity.GetController().IsFriendlySide();
        if (useFastAnimations)
        {
          switch (premiumType)
          {
            case TAG_PREMIUM.NORMAL:
            case TAG_PREMIUM.SIGNATURE:
              return flag ? SpellType.SUMMON_IN_FAST : SpellType.SUMMON_IN_OPPONENT_FAST;
            case TAG_PREMIUM.GOLDEN:
              return flag ? SpellType.SUMMON_IN_PREMIUM_FAST : SpellType.SUMMON_IN_OPPONENT_FAST;
            case TAG_PREMIUM.DIAMOND:
              return flag ? SpellType.SUMMON_IN_DIAMOND_FAST : SpellType.SUMMON_IN_OPPONENT_FAST;
            default:
              Debug.LogWarning((object) string.Format("CardDef.DetermineSummonInSpell_HandToPlay() - unexpected premium type {0}", (object) premiumType));
              goto case TAG_PREMIUM.NORMAL;
          }
        }
        else if (cost >= 7)
        {
          switch (premiumType)
          {
            case TAG_PREMIUM.NORMAL:
            case TAG_PREMIUM.SIGNATURE:
              return flag ? SpellType.SUMMON_IN_LARGE : SpellType.SUMMON_IN_OPPONENT_LARGE;
            case TAG_PREMIUM.GOLDEN:
              return flag ? SpellType.SUMMON_IN_LARGE_PREMIUM : SpellType.SUMMON_IN_OPPONENT_LARGE_PREMIUM;
            case TAG_PREMIUM.DIAMOND:
              return flag ? SpellType.SUMMON_IN_LARGE_DIAMOND : SpellType.SUMMON_IN_OPPONENT_LARGE_DIAMOND;
            default:
              Debug.LogWarning((object) string.Format("CardDef.DetermineSummonInSpell_HandToPlay() - unexpected premium type {0}", (object) premiumType));
              goto case TAG_PREMIUM.NORMAL;
          }
        }
        else if (cost >= 4)
        {
          switch (premiumType)
          {
            case TAG_PREMIUM.NORMAL:
            case TAG_PREMIUM.SIGNATURE:
              return flag ? SpellType.SUMMON_IN_MEDIUM : SpellType.SUMMON_IN_OPPONENT_MEDIUM;
            case TAG_PREMIUM.GOLDEN:
              return flag ? SpellType.SUMMON_IN_MEDIUM_PREMIUM : SpellType.SUMMON_IN_OPPONENT_MEDIUM_PREMIUM;
            case TAG_PREMIUM.DIAMOND:
              return flag ? SpellType.SUMMON_IN_MEDIUM_DIAMOND : SpellType.SUMMON_IN_OPPONENT_MEDIUM_DIAMOND;
            default:
              Debug.LogWarning((object) string.Format("CardDef.DetermineSummonInSpell_HandToPlay() - unexpected premium type {0}", (object) premiumType));
              goto case TAG_PREMIUM.NORMAL;
          }
        }
        else
        {
          switch (premiumType)
          {
            case TAG_PREMIUM.NORMAL:
            case TAG_PREMIUM.SIGNATURE:
              return flag ? SpellType.SUMMON_IN : SpellType.SUMMON_IN_OPPONENT;
            case TAG_PREMIUM.GOLDEN:
              return flag ? SpellType.SUMMON_IN_PREMIUM : SpellType.SUMMON_IN_OPPONENT_PREMIUM;
            case TAG_PREMIUM.DIAMOND:
              return flag ? SpellType.SUMMON_IN_DIAMOND : SpellType.SUMMON_IN_OPPONENT_DIAMOND;
            default:
              Debug.LogWarning((object) string.Format("CardDef.DetermineSummonInSpell_HandToPlay() - unexpected premium type {0}", (object) premiumType));
              goto case TAG_PREMIUM.NORMAL;
          }
        }
    }
  }

  public virtual SpellType DetermineSummonOutSpell_HandToPlay(Card card)
  {
    Entity entity = card.GetEntity();
    if (entity.IsHero())
      return SpellType.SUMMON_OUT_HERO;
    if (entity.IsMercenary())
      return SpellType.SUMMON_OUT_MERCENARY;
    if (!entity.GetController().IsFriendlySide())
      return SpellType.SUMMON_OUT;
    int cost = entity.GetEntityDef().GetCost();
    TAG_PREMIUM premiumType = entity.GetPremiumType();
    if ((Object) card.GetActor() != (Object) null && card.GetActor().UseTechLevelManaGem())
    {
      switch (premiumType)
      {
        case TAG_PREMIUM.NORMAL:
        case TAG_PREMIUM.SIGNATURE:
          return SpellType.SUMMON_OUT_TECH_LEVEL;
        case TAG_PREMIUM.GOLDEN:
          return SpellType.SUMMON_OUT_TECH_LEVEL_PREMIUM;
        default:
          Debug.LogWarning((object) string.Format("CardDef.DetermineSummonOutSpell_HandToPlay(): unexpected premium type {0}", (object) premiumType));
          goto case TAG_PREMIUM.NORMAL;
      }
    }
    else if (cost >= 7)
    {
      switch (premiumType)
      {
        case TAG_PREMIUM.NORMAL:
        case TAG_PREMIUM.SIGNATURE:
          return SpellType.SUMMON_OUT_LARGE;
        case TAG_PREMIUM.GOLDEN:
          return SpellType.SUMMON_OUT_PREMIUM;
        case TAG_PREMIUM.DIAMOND:
          return SpellType.SUMMON_OUT_DIAMOND;
        default:
          Debug.LogWarning((object) string.Format("CardDef.DetermineSummonOutSpell_HandToPlay(): unexpected premium type {0}", (object) premiumType));
          goto case TAG_PREMIUM.NORMAL;
      }
    }
    else if (cost >= 4)
    {
      switch (premiumType)
      {
        case TAG_PREMIUM.NORMAL:
        case TAG_PREMIUM.SIGNATURE:
          return SpellType.SUMMON_OUT_MEDIUM;
        case TAG_PREMIUM.GOLDEN:
          return SpellType.SUMMON_OUT_PREMIUM;
        case TAG_PREMIUM.DIAMOND:
          return SpellType.SUMMON_OUT_DIAMOND;
        default:
          Debug.LogWarning((object) string.Format("CardDef.DetermineSummonOutSpell_HandToPlay(): unexpected premium type {0}", (object) premiumType));
          goto case TAG_PREMIUM.NORMAL;
      }
    }
    else
    {
      switch (premiumType)
      {
        case TAG_PREMIUM.NORMAL:
        case TAG_PREMIUM.SIGNATURE:
          return SpellType.SUMMON_OUT;
        case TAG_PREMIUM.GOLDEN:
          return SpellType.SUMMON_OUT_PREMIUM;
        case TAG_PREMIUM.DIAMOND:
          return SpellType.SUMMON_OUT_DIAMOND;
        default:
          Debug.LogWarning((object) string.Format("CardDef.DetermineSummonOutSpell_HandToPlay(): unexpected premium type {0}", (object) premiumType));
          goto case TAG_PREMIUM.NORMAL;
      }
    }
  }

  private static void SetTextureIfNotNull(Material baseMat, ref Material targetMat, Texture tex)
  {
    if ((Object) baseMat == (Object) null)
      return;
    if ((Object) targetMat == (Object) null)
    {
      targetMat = Object.Instantiate<Material>(baseMat);
      CardDef.GetMaterialService()?.IgnoreMaterial(targetMat);
    }
    targetMat.mainTexture = tex;
  }

  private static IMaterialService GetMaterialService()
  {
    if (CardDef.s_materialService == null)
      CardDef.s_materialService = ServiceManager.Get<IMaterialService>();
    return CardDef.s_materialService;
  }

  public void OnBattlegroundHeroBuddyPortraitLoaded(AssetHandle<Texture> portrait) => AssetHandle.Set<Texture>(ref this.m_LoadedBattlegroundHeroBuddyPortraitTexture, portrait);

  public void OnPortraitLoaded(AssetHandle<Texture> portrait, int quality)
  {
    if ((Object) this.m_Portrait != (Object) null)
      this.m_Portrait.OnPortraitLoaded(portrait, quality);
    else if (quality <= this.m_portraitQuality.TextureQuality)
    {
      Debug.LogWarning((object) string.Format("Loaded texture of quality lower or equal to what was was already available ({0} <= {1}), texture={2}", (object) quality, (object) this.m_portraitQuality, (object) portrait));
    }
    else
    {
      this.m_portraitQuality.TextureQuality = quality;
      if ((bool) this.m_LoadedPortraitTexture)
        AssetHandle.Set<Texture>(ref this.m_lowQualityPortrait, this.m_LoadedPortraitTexture);
      AssetHandle.Set<Texture>(ref this.m_LoadedPortraitTexture, portrait);
      if ((Object) this.m_LoadedSignaturePortraitMaterial != (Object) null && string.IsNullOrEmpty(this.m_SignaturePortraitTexturePath))
      {
        this.m_LoadedSignaturePortraitMaterial.mainTexture = (Texture) this.m_LoadedPortraitTexture;
        this.m_portraitQuality.PremiumType = TAG_PREMIUM.SIGNATURE;
      }
      else if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null && string.IsNullOrEmpty(this.m_PremiumPortraitTexturePath))
      {
        this.m_LoadedPremiumPortraitMaterial.mainTexture = (Texture) this.m_LoadedPortraitTexture;
        this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
      }
      if ((Object) this.m_LoadedPremiumClassMaterial != (Object) null && string.IsNullOrEmpty(this.m_PremiumPortraitTexturePath))
        this.m_LoadedPremiumClassMaterial.mainTexture = (Texture) this.m_LoadedPortraitTexture;
      CardDef.SetTextureIfNotNull(this.m_DeckCardBarPortrait, ref this.m_LoadedDeckCardBarPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_SignatureDeckCardBarPortrait, ref this.m_LoadedSignatureDeckCardBarPortrait, (Texture) this.m_loadedSignaturePortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_EnchantmentPortrait, ref this.m_LoadedEnchantmentPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_HistoryTileFullPortrait, ref this.m_LoadedHistoryTileFullPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_HistoryTileHalfPortrait, ref this.m_LoadedHistoryTileHalfPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_HistoryTileFullSignaturePortrait, ref this.m_LoadedHistoryTileFullSignaturePortrait, (Texture) this.m_loadedSignaturePortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_HistoryTileHalfSignaturePortrait, ref this.m_LoadedHistoryTileHalfSignaturePortrait, (Texture) this.m_loadedSignaturePortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_LeaderboardTileFullPortrait, ref this.m_LoadedLeaderboardTileFullPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull((Material) (MobileOverrideValue<Material>) this.m_CustomDeckPortrait, ref this.m_LoadedCustomDeckPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull((Material) (MobileOverrideValue<Material>) this.m_DeckPickerPortrait, ref this.m_LoadedDeckPickerPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_PracticeAIPortrait, ref this.m_LoadedPracticeAIPortrait, (Texture) this.m_LoadedPortraitTexture);
      CardDef.SetTextureIfNotNull(this.m_DeckBoxPortrait, ref this.m_LoadedDeckBoxPortrait, (Texture) this.m_LoadedPortraitTexture);
    }
  }

  public void OnPremiumMaterialLoaded(
    AssetHandle<Material> material,
    AssetHandle<Texture> portrait,
    AssetHandle<UberShaderAnimation> portraitAnimation)
  {
    if ((Object) this.m_Portrait != (Object) null)
      this.m_Portrait.OnPremiumMaterialLoaded(material, portrait, portraitAnimation);
    else if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null)
    {
      if (!Application.isPlaying)
        return;
      Debug.LogWarning((object) string.Format("Loaded premium material twice: {0}", (object) material));
    }
    else
    {
      if ((bool) material)
      {
        AssetHandle.Set<Material>(ref this.m_premiumMaterialHandle, material);
        this.m_LoadedPremiumPortraitMaterial = (Material) Object.Instantiate((Object) (Material) material);
        this.m_LoadedPremiumClassMaterial = (Material) Object.Instantiate((Object) (Material) material);
        IMaterialService materialService = CardDef.GetMaterialService();
        if (materialService != null)
        {
          materialService.IgnoreMaterial(this.m_LoadedPremiumPortraitMaterial);
          materialService.IgnoreMaterial(this.m_LoadedPremiumClassMaterial);
        }
      }
      AssetHandle.Set<UberShaderAnimation>(ref this.m_premiumPortraitAnimation, portraitAnimation);
      if ((bool) this.m_LoadedPortraitTexture)
      {
        if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null)
          this.m_LoadedPremiumPortraitMaterial.mainTexture = (Texture) this.m_LoadedPortraitTexture;
        if ((Object) this.m_LoadedPremiumClassMaterial != (Object) null)
          this.m_LoadedPremiumClassMaterial.mainTexture = (Texture) this.m_LoadedPortraitTexture;
        this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
      }
      if (!(bool) portrait)
        return;
      AssetHandle.Set<Texture>(ref this.m_loadedPremiumPortraitTexture, portrait);
      if ((Object) this.m_LoadedPremiumPortraitMaterial != (Object) null)
        this.m_LoadedPremiumPortraitMaterial.mainTexture = (Texture) portrait;
      if ((Object) this.m_LoadedPremiumClassMaterial != (Object) null)
        this.m_LoadedPremiumClassMaterial.mainTexture = (Texture) portrait;
      this.m_portraitQuality.PremiumType = TAG_PREMIUM.GOLDEN;
    }
  }

  public void OnSignatureMaterialLoaded(
    AssetHandle<Material> material,
    AssetHandle<Texture> portrait)
  {
    if ((bool) material)
    {
      AssetHandle.Set<Material>(ref this.m_signatureMaterialHandle, material);
      this.m_LoadedSignaturePortraitMaterial = (Material) Object.Instantiate((Object) (Material) material);
      CardDef.GetMaterialService()?.IgnoreMaterial(this.m_LoadedSignaturePortraitMaterial);
    }
    if ((bool) this.m_loadedSignaturePortraitTexture)
    {
      if ((Object) this.m_LoadedSignaturePortraitMaterial != (Object) null)
        this.m_LoadedSignaturePortraitMaterial.mainTexture = (Texture) this.m_loadedSignaturePortraitTexture;
      this.m_portraitQuality.PremiumType = TAG_PREMIUM.SIGNATURE;
    }
    if (!(bool) portrait)
      return;
    AssetHandle.Set<Texture>(ref this.m_loadedSignaturePortraitTexture, portrait);
    if ((Object) this.m_LoadedSignaturePortraitMaterial != (Object) null && (Object) this.m_LoadedSignaturePortraitMaterial.mainTexture == (Object) null)
      this.m_LoadedSignaturePortraitMaterial.mainTexture = (Texture) portrait;
    if ((Object) this.m_LoadedSignatureDeckCardBarPortrait != (Object) null && (Object) this.m_LoadedSignatureDeckCardBarPortrait.mainTexture == (Object) null)
      this.m_LoadedSignatureDeckCardBarPortrait.mainTexture = (Texture) portrait;
    if ((Object) this.m_LoadedHistoryTileFullSignaturePortrait != (Object) null && (Object) this.m_LoadedHistoryTileFullSignaturePortrait.mainTexture == (Object) null)
      this.m_LoadedHistoryTileFullSignaturePortrait.mainTexture = (Texture) portrait;
    if ((Object) this.m_LoadedHistoryTileHalfSignaturePortrait != (Object) null && (Object) this.m_LoadedHistoryTileHalfSignaturePortrait.mainTexture == (Object) null)
      this.m_LoadedHistoryTileHalfSignaturePortrait.mainTexture = (Texture) portrait;
    this.m_portraitQuality.PremiumType = TAG_PREMIUM.SIGNATURE;
  }

  public CardPortraitQuality GetPortraitQuality() => this.m_portraitQuality;

  public Texture GetBattlegroundHeroBuddyTexture() => (Texture) this.GetBattlegroundHeroBuddyTextureHandle();

  public Texture GetBattlegroundHeroBuddyTextureFromMat() => this.m_BattlegroundHeroBuddyPortraitMaterial?.mainTexture;

  public Material GetBattlegroundHeroBuddyMaterial() => this.m_BattlegroundHeroBuddyPortraitMaterial;

  public Material GetBattlegroundsQuestRewardPortraitMaterial() => this.m_BattlegroundsQuestRewardsMaterial;

  public Texture GetPortraitTexture(TAG_PREMIUM premium)
  {
    if (premium == TAG_PREMIUM.SIGNATURE)
    {
      Texture portraitTextureHandle = (Texture) this.GetSignaturePortraitTextureHandle();
      if ((Object) portraitTextureHandle != (Object) null)
        return portraitTextureHandle;
    }
    return (Texture) this.GetPortraitTextureHandle();
  }

  public bool TryGetPortraitTexture(TAG_PREMIUM premium, out Texture portraitTexture)
  {
    portraitTexture = this.GetPortraitTexture(premium);
    return (Object) portraitTexture != (Object) null;
  }

  public AssetHandle<Texture> GetPortraitTextureHandle() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.LoadedPortraitTexture : this.m_LoadedPortraitTexture;

  public AssetHandle<Texture> GetSignaturePortraitTextureHandle() => this.m_loadedSignaturePortraitTexture;

  public AssetHandle<Texture> GetBattlegroundHeroBuddyTextureHandle() => this.m_LoadedBattlegroundHeroBuddyPortraitTexture;

  public bool IsPremiumLoaded(TAG_PREMIUM premium)
  {
    switch (premium)
    {
      case TAG_PREMIUM.NORMAL:
      case TAG_PREMIUM.DIAMOND:
        return this.m_LoadedPortraitTexture != null;
      case TAG_PREMIUM.GOLDEN:
        return (Object) this.m_LoadedPremiumPortraitMaterial != (Object) null;
      case TAG_PREMIUM.SIGNATURE:
        return (Object) this.m_LoadedSignaturePortraitMaterial != (Object) null;
      default:
        return false;
    }
  }

  public Material GetPremiumPortraitMaterial() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedPremiumPortraitMaterial : this.m_LoadedPremiumPortraitMaterial;

  public UberShaderAnimation GetPremiumPortraitAnimation() => (Object) this.m_Portrait != (Object) null ? (UberShaderAnimation) this.m_Portrait.PremiumPortraitAnimation : (UberShaderAnimation) this.m_premiumPortraitAnimation;

  public Material GetSignaturePortraitMaterial() => this.m_LoadedSignaturePortraitMaterial;

  public Material GetPortraitMaterial(TAG_PREMIUM premium)
  {
    if (premium == TAG_PREMIUM.GOLDEN)
      return this.GetPremiumPortraitMaterial();
    if (premium == TAG_PREMIUM.SIGNATURE)
      return this.GetSignaturePortraitMaterial();
    Debug.LogError((object) string.Format("Attempting to get portrait material for unexpected premium level {0}.", (object) premium));
    return (Material) null;
  }

  public Material GetDeckCardBarPortrait(TAG_PREMIUM premium)
  {
    if (premium == TAG_PREMIUM.SIGNATURE && (Object) this.m_LoadedSignatureDeckCardBarPortrait != (Object) null)
      return this.m_LoadedSignatureDeckCardBarPortrait;
    return (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedDeckCardBarPortrait : this.m_LoadedDeckCardBarPortrait;
  }

  public bool TryGetEnchantmentPortrait(out Material enchantmentPortraitMat)
  {
    enchantmentPortraitMat = !((Object) this.m_Portrait != (Object) null) ? this.m_LoadedEnchantmentPortrait : this.m_Portrait.m_LoadedEnchantmentPortrait;
    return (Object) enchantmentPortraitMat != (Object) null;
  }

  public bool TryGetHistoryTileFullPortrait(TAG_PREMIUM premium, out Material fullHistoryTileMat)
  {
    fullHistoryTileMat = premium != TAG_PREMIUM.SIGNATURE || !((Object) this.m_LoadedHistoryTileFullSignaturePortrait != (Object) null) ? (!((Object) this.m_Portrait != (Object) null) ? this.m_LoadedHistoryTileFullPortrait : this.m_Portrait.m_LoadedHistoryTileFullPortrait) : this.m_LoadedHistoryTileFullSignaturePortrait;
    return (Object) fullHistoryTileMat != (Object) null;
  }

  public bool TryGetHistoryTileHalfPortrait(TAG_PREMIUM premium, out Material halfHistoryTileMat)
  {
    halfHistoryTileMat = premium != TAG_PREMIUM.SIGNATURE || !((Object) this.m_LoadedHistoryTileHalfSignaturePortrait != (Object) null) ? (!((Object) this.m_Portrait != (Object) null) ? this.m_LoadedHistoryTileHalfPortrait : this.m_Portrait.m_LoadedHistoryTileHalfPortrait) : this.m_LoadedHistoryTileHalfSignaturePortrait;
    return (Object) halfHistoryTileMat != (Object) null;
  }

  public Material GetLeaderboardTileFullPortrait() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedLeaderboardTileFullPortrait : this.m_LoadedLeaderboardTileFullPortrait;

  public Material GetCustomDeckPortrait() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedCustomDeckPortrait : this.m_LoadedCustomDeckPortrait;

  public Material GetDeckPickerPortrait() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedDeckPickerPortrait : this.m_LoadedDeckPickerPortrait;

  public Material GetPracticeAIPortrait() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedPracticeAIPortrait : this.m_LoadedPracticeAIPortrait;

  public Material GetDeckBoxPortrait() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedDeckBoxPortrait : this.m_LoadedDeckBoxPortrait;

  public AssetReference GetBattlegroundHeroBuddyPortraitRef() => AssetReference.op_Implicit(this.m_BattlegroundHeroBuddyPortraitTexturePath);

  public AssetReference GetPortraitRef()
  {
    if ((Object) this.m_Portrait != (Object) null)
      return this.m_Portrait.GetPortraitRef();
    return this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PortraitTextureOverride) ? AssetReference.op_Implicit(this.m_currentSpecialEvent.m_PortraitTextureOverride) : AssetReference.op_Implicit(this.m_PortraitTexturePath);
  }

  public AssetReference GetPremiumMaterialRef()
  {
    if ((Object) this.m_Portrait != (Object) null)
      return this.m_Portrait.GetPremiumMaterialRef();
    return this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PremiumPortraitMaterialOverride) ? AssetReference.op_Implicit(this.m_currentSpecialEvent.m_PremiumPortraitMaterialOverride) : AssetReference.op_Implicit(this.m_PremiumPortraitMaterialPath);
  }

  public AssetReference GetPremiumPortraitRef()
  {
    if ((Object) this.m_Portrait != (Object) null)
      return this.m_Portrait.GetPremiumPortraitRef();
    return this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PremiumPortraitTextureOverride) ? AssetReference.op_Implicit(this.m_currentSpecialEvent.m_PremiumPortraitTextureOverride) : AssetReference.op_Implicit(this.m_PremiumPortraitTexturePath);
  }

  public AssetReference GetPremiumAnimationRef()
  {
    if ((Object) this.m_Portrait != (Object) null)
      return this.m_Portrait.GetPremiumAnimationRef();
    return this.m_currentSpecialEvent != null && !string.IsNullOrEmpty(this.m_currentSpecialEvent.m_PremiumUberShaderAnimationOverride) ? AssetReference.op_Implicit(this.m_currentSpecialEvent.m_PremiumUberShaderAnimationOverride) : AssetReference.op_Implicit(this.m_PremiumUberShaderAnimationPath);
  }

  public Material GetPremiumClassMaterial() => (Object) this.m_Portrait != (Object) null ? this.m_Portrait.m_LoadedPremiumClassMaterial : this.m_LoadedPremiumClassMaterial;

  public AssetReference GetSignaturePortraitRef() => AssetReference.op_Implicit(this.m_SignaturePortraitTexturePath);

  public AssetReference GetSignatureMaterialRef() => AssetReference.op_Implicit(this.SignaturePortraitMaterialPath);

  public void UpdateSpecialEvent() => this.m_currentSpecialEvent = CardDefSpecialEvent.FindActiveEvent(this);
}
