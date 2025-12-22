using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDbf : IService
{
  public static Dbf<AccountLicenseDbfRecord> AccountLicense;
  public static Dbf<AchieveDbfRecord> Achieve;
  public static Dbf<AchieveConditionDbfRecord> AchieveCondition;
  public static Dbf<AchieveRegionDataDbfRecord> AchieveRegionData;
  public static Dbf<AchievementDbfRecord> Achievement;
  public static Dbf<AchievementCategoryDbfRecord> AchievementCategory;
  public static Dbf<AchievementSectionDbfRecord> AchievementSection;
  public static Dbf<AchievementSectionItemDbfRecord> AchievementSectionItem;
  public static Dbf<AchievementSubcategoryDbfRecord> AchievementSubcategory;
  public static Dbf<AdventureDbfRecord> Adventure;
  public static Dbf<AdventureDataDbfRecord> AdventureData;
  public static Dbf<AdventureDeckDbfRecord> AdventureDeck;
  public static Dbf<AdventureGuestHeroesDbfRecord> AdventureGuestHeroes;
  public static Dbf<AdventureHeroPowerDbfRecord> AdventureHeroPower;
  public static Dbf<AdventureLoadoutTreasuresDbfRecord> AdventureLoadoutTreasures;
  public static Dbf<AdventureMissionDbfRecord> AdventureMission;
  public static Dbf<AdventureModeDbfRecord> AdventureMode;
  public static Dbf<BannerDbfRecord> Banner;
  public static Dbf<BattlegroundsBoardSkinDbfRecord> BattlegroundsBoardSkin;
  public static Dbf<BattlegroundsEmoteDbfRecord> BattlegroundsEmote;
  public static Dbf<BattlegroundsFinisherDbfRecord> BattlegroundsFinisher;
  public static Dbf<BattlegroundsGuideSkinDbfRecord> BattlegroundsGuideSkin;
  public static Dbf<BattlegroundsHeroSkinDbfRecord> BattlegroundsHeroSkin;
  public static Dbf<BattlegroundsSeasonDbfRecord> BattlegroundsSeason;
  public static Dbf<BoardDbfRecord> Board;
  public static Dbf<BonusBountyDropChanceDbfRecord> BonusBountyDropChance;
  public static Dbf<BoosterDbfRecord> Booster;
  public static Dbf<BoosterCardSetDbfRecord> BoosterCardSet;
  public static Dbf<BuildingTierDbfRecord> BuildingTier;
  public static Dbf<CardDbfRecord> Card;
  public static Dbf<CardAdditonalSearchTermsDbfRecord> CardAdditonalSearchTerms;
  public static Dbf<CardBackDbfRecord> CardBack;
  public static Dbf<CardChangeDbfRecord> CardChange;
  public static Dbf<CardDiscoverStringDbfRecord> CardDiscoverString;
  public static Dbf<CardEquipmentAltTextDbfRecord> CardEquipmentAltText;
  public static Dbf<CardHeroDbfRecord> CardHero;
  public static Dbf<CardPlayerDeckOverrideDbfRecord> CardPlayerDeckOverride;
  public static Dbf<CardRaceDbfRecord> CardRace;
  public static Dbf<CardSetDbfRecord> CardSet;
  public static Dbf<CardSetSpellOverrideDbfRecord> CardSetSpellOverride;
  public static Dbf<CardSetTimingDbfRecord> CardSetTiming;
  public static Dbf<CardTagDbfRecord> CardTag;
  public static Dbf<CardValueDbfRecord> CardValue;
  public static Dbf<CharacterDialogDbfRecord> CharacterDialog;
  public static Dbf<CharacterDialogItemsDbfRecord> CharacterDialogItems;
  public static Dbf<ClassDbfRecord> Class;
  public static Dbf<ClassExclusionsDbfRecord> ClassExclusions;
  public static Dbf<ClientStringDbfRecord> ClientString;
  public static Dbf<CoinDbfRecord> Coin;
  public static Dbf<CreditsYearDbfRecord> CreditsYear;
  public static Dbf<DeckDbfRecord> Deck;
  public static Dbf<DeckCardDbfRecord> DeckCard;
  public static Dbf<DeckRulesetDbfRecord> DeckRuleset;
  public static Dbf<DeckRulesetRuleDbfRecord> DeckRulesetRule;
  public static Dbf<DeckRulesetRuleSubsetDbfRecord> DeckRulesetRuleSubset;
  public static Dbf<DeckTemplateDbfRecord> DeckTemplate;
  public static Dbf<DetailsVideoCueDbfRecord> DetailsVideoCue;
  public static Dbf<DkRuneListDbfRecord> DkRuneList;
  public static Dbf<DraftContentDbfRecord> DraftContent;
  public static Dbf<ExternalUrlDbfRecord> ExternalUrl;
  public static Dbf<FixedRewardDbfRecord> FixedReward;
  public static Dbf<FixedRewardActionDbfRecord> FixedRewardAction;
  public static Dbf<FixedRewardMapDbfRecord> FixedRewardMap;
  public static Dbf<GameModeDbfRecord> GameMode;
  public static Dbf<GameSaveSubkeyDbfRecord> GameSaveSubkey;
  public static Dbf<GlobalDbfRecord> Global;
  public static Dbf<GuestHeroDbfRecord> GuestHero;
  public static Dbf<GuestHeroSelectionRatioDbfRecord> GuestHeroSelectionRatio;
  public static Dbf<HiddenLicenseDbfRecord> HiddenLicense;
  public static Dbf<InitCardValueDbfRecord> InitCardValue;
  public static Dbf<KeywordTextDbfRecord> KeywordText;
  public static Dbf<LeagueDbfRecord> League;
  public static Dbf<LeagueBgPublicRatingEquivDbfRecord> LeagueBgPublicRatingEquiv;
  public static Dbf<LeagueGameTypeDbfRecord> LeagueGameType;
  public static Dbf<LeagueRankDbfRecord> LeagueRank;
  public static Dbf<LettuceAbilityDbfRecord> LettuceAbility;
  public static Dbf<LettuceAbilityTierDbfRecord> LettuceAbilityTier;
  public static Dbf<LettuceBountyDbfRecord> LettuceBounty;
  public static Dbf<LettuceBountyFinalRewardsDbfRecord> LettuceBountyFinalRewards;
  public static Dbf<LettuceBountySetDbfRecord> LettuceBountySet;
  public static Dbf<LettuceEquipmentDbfRecord> LettuceEquipment;
  public static Dbf<LettuceEquipmentModifierDataDbfRecord> LettuceEquipmentModifierData;
  public static Dbf<LettuceEquipmentTierDbfRecord> LettuceEquipmentTier;
  public static Dbf<LettuceMapBonusRewardsDbfRecord> LettuceMapBonusRewards;
  public static Dbf<LettuceMapNodeTypeDbfRecord> LettuceMapNodeType;
  public static Dbf<LettuceMapNodeTypeAnomalyDbfRecord> LettuceMapNodeTypeAnomaly;
  public static Dbf<LettuceMercenaryDbfRecord> LettuceMercenary;
  public static Dbf<LettuceMercenaryAbilityDbfRecord> LettuceMercenaryAbility;
  public static Dbf<LettuceMercenaryEquipmentDbfRecord> LettuceMercenaryEquipment;
  public static Dbf<LettuceMercenaryLevelDbfRecord> LettuceMercenaryLevel;
  public static Dbf<LettuceMercenaryLevelStatsDbfRecord> LettuceMercenaryLevelStats;
  public static Dbf<LettuceMercenarySpecializationDbfRecord> LettuceMercenarySpecialization;
  public static Dbf<LettuceTreasureDbfRecord> LettuceTreasure;
  public static Dbf<LettuceTreasureTierDbfRecord> LettuceTreasureTier;
  public static Dbf<LettuceTutorialVoDbfRecord> LettuceTutorialVo;
  public static Dbf<LoginPopupSequenceDbfRecord> LoginPopupSequence;
  public static Dbf<LoginPopupSequencePopupDbfRecord> LoginPopupSequencePopup;
  public static Dbf<LoginRewardDbfRecord> LoginReward;
  public static Dbf<LuckyDrawBoxDbfRecord> LuckyDrawBox;
  public static Dbf<LuckyDrawRewardsDbfRecord> LuckyDrawRewards;
  public static Dbf<MercTriggeredEventDbfRecord> MercTriggeredEvent;
  public static Dbf<MercTriggeringEventDbfRecord> MercTriggeringEvent;
  public static Dbf<MercenariesRandomRewardDbfRecord> MercenariesRandomReward;
  public static Dbf<MercenariesRankedSeasonRewardRankDbfRecord> MercenariesRankedSeasonRewardRank;
  public static Dbf<MercenaryArtVariationDbfRecord> MercenaryArtVariation;
  public static Dbf<MercenaryArtVariationPremiumDbfRecord> MercenaryArtVariationPremium;
  public static Dbf<MercenaryBuildingDbfRecord> MercenaryBuilding;
  public static Dbf<MercenaryVillageTriggerDbfRecord> MercenaryVillageTrigger;
  public static Dbf<MercenaryVisitorDbfRecord> MercenaryVisitor;
  public static Dbf<MiniSetDbfRecord> MiniSet;
  public static Dbf<ModifiedLettuceAbilityCardTagDbfRecord> ModifiedLettuceAbilityCardTag;
  public static Dbf<ModifiedLettuceAbilityValueDbfRecord> ModifiedLettuceAbilityValue;
  public static Dbf<ModularBundleDbfRecord> ModularBundle;
  public static Dbf<ModularBundleLayoutDbfRecord> ModularBundleLayout;
  public static Dbf<ModularBundleLayoutNodeDbfRecord> ModularBundleLayoutNode;
  public static Dbf<MultiClassGroupDbfRecord> MultiClassGroup;
  public static Dbf<NextTiersDbfRecord> NextTiers;
  public static Dbf<PowerDefinitionDbfRecord> PowerDefinition;
  public static Dbf<ProductDbfRecord> Product;
  public static Dbf<ProductClientDataDbfRecord> ProductClientData;
  public static Dbf<PvpdrSeasonDbfRecord> PvpdrSeason;
  public static Dbf<QuestDbfRecord> Quest;
  public static Dbf<QuestDialogDbfRecord> QuestDialog;
  public static Dbf<QuestDialogOnCompleteDbfRecord> QuestDialogOnComplete;
  public static Dbf<QuestDialogOnProgress1DbfRecord> QuestDialogOnProgress1;
  public static Dbf<QuestDialogOnProgress2DbfRecord> QuestDialogOnProgress2;
  public static Dbf<QuestDialogOnReceivedDbfRecord> QuestDialogOnReceived;
  public static Dbf<QuestModifierDbfRecord> QuestModifier;
  public static Dbf<QuestPoolDbfRecord> QuestPool;
  public static Dbf<RegionOverridesDbfRecord> RegionOverrides;
  public static Dbf<RepeatableTaskListDbfRecord> RepeatableTaskList;
  public static Dbf<RewardBagDbfRecord> RewardBag;
  public static Dbf<RewardChestDbfRecord> RewardChest;
  public static Dbf<RewardChestContentsDbfRecord> RewardChestContents;
  public static Dbf<RewardItemDbfRecord> RewardItem;
  public static Dbf<RewardListDbfRecord> RewardList;
  public static Dbf<RewardTrackDbfRecord> RewardTrack;
  public static Dbf<RewardTrackLevelDbfRecord> RewardTrackLevel;
  public static Dbf<ScenarioDbfRecord> Scenario;
  public static Dbf<ScenarioGuestHeroesDbfRecord> ScenarioGuestHeroes;
  public static Dbf<ScheduledCharacterDialogDbfRecord> ScheduledCharacterDialog;
  public static Dbf<ScoreLabelDbfRecord> ScoreLabel;
  public static Dbf<SellableDeckDbfRecord> SellableDeck;
  public static Dbf<ShopTierDbfRecord> ShopTier;
  public static Dbf<ShopTierProductSaleDbfRecord> ShopTierProductSale;
  public static Dbf<SubsetDbfRecord> Subset;
  public static Dbf<SubsetCardDbfRecord> SubsetCard;
  public static Dbf<SubsetRuleDbfRecord> SubsetRule;
  public static Dbf<TaskListDbfRecord> TaskList;
  public static Dbf<TavernBrawlTicketDbfRecord> TavernBrawlTicket;
  public static Dbf<TierPropertiesDbfRecord> TierProperties;
  public static Dbf<TriggerDbfRecord> Trigger;
  public static Dbf<VisitorTaskDbfRecord> VisitorTask;
  public static Dbf<VisitorTaskChainDbfRecord> VisitorTaskChain;
  public static Dbf<WingDbfRecord> Wing;
  public static Dbf<XpOnPlacementDbfRecord> XpOnPlacement;
  public static Dbf<XpOnPlacementGameTypeMultiplierDbfRecord> XpOnPlacementGameTypeMultiplier;
  public static Dbf<XpPerTimeGameTypeMultiplierDbfRecord> XpPerTimeGameTypeMultiplier;
  public static bool IsLoaded;
  private static GameDbfIndex s_index;
  private static DOPAsset s_DOPAsset = (DOPAsset) null;
  private static Map<string, IDbf> s_allDbfs = new Map<string, IDbf>();
  public const string kDOPAssetPath = "Assets/Game/DBF-Asset/";
  public const string kDOPAssetName = "/DOPAsset.asset";

  private static Action[] GetLoadDbfActions(DbfFormat format) => new Action[158]
  {
    (Action) (() => GameDbf.AccountLicense = Dbf<AccountLicenseDbfRecord>.Load("ACCOUNT_LICENSE", format)),
    (Action) (() => GameDbf.Achieve = Dbf<AchieveDbfRecord>.Load("ACHIEVE", format)),
    (Action) (() => GameDbf.AchieveCondition = Dbf<AchieveConditionDbfRecord>.Load("ACHIEVE_CONDITION", format)),
    (Action) (() => GameDbf.AchieveRegionData = Dbf<AchieveRegionDataDbfRecord>.Load("ACHIEVE_REGION_DATA", format)),
    (Action) (() => GameDbf.Achievement = Dbf<AchievementDbfRecord>.Load("ACHIEVEMENT", format)),
    (Action) (() => GameDbf.AchievementCategory = Dbf<AchievementCategoryDbfRecord>.Load("ACHIEVEMENT_CATEGORY", format)),
    (Action) (() => GameDbf.AchievementSection = Dbf<AchievementSectionDbfRecord>.Load("ACHIEVEMENT_SECTION", format)),
    (Action) (() => GameDbf.AchievementSectionItem = Dbf<AchievementSectionItemDbfRecord>.Load("ACHIEVEMENT_SECTION_ITEM", format)),
    (Action) (() => GameDbf.AchievementSubcategory = Dbf<AchievementSubcategoryDbfRecord>.Load("ACHIEVEMENT_SUBCATEGORY", format)),
    (Action) (() => GameDbf.Adventure = Dbf<AdventureDbfRecord>.Load("ADVENTURE", format)),
    (Action) (() => GameDbf.AdventureData = Dbf<AdventureDataDbfRecord>.Load("ADVENTURE_DATA", format)),
    (Action) (() => GameDbf.AdventureDeck = Dbf<AdventureDeckDbfRecord>.Load("ADVENTURE_DECK", format)),
    (Action) (() => GameDbf.AdventureGuestHeroes = Dbf<AdventureGuestHeroesDbfRecord>.Load("ADVENTURE_GUEST_HEROES", format)),
    (Action) (() => GameDbf.AdventureHeroPower = Dbf<AdventureHeroPowerDbfRecord>.Load("ADVENTURE_HERO_POWER", format)),
    (Action) (() => GameDbf.AdventureLoadoutTreasures = Dbf<AdventureLoadoutTreasuresDbfRecord>.Load("ADVENTURE_LOADOUT_TREASURES", format)),
    (Action) (() => GameDbf.AdventureMission = Dbf<AdventureMissionDbfRecord>.Load("ADVENTURE_MISSION", format)),
    (Action) (() => GameDbf.AdventureMode = Dbf<AdventureModeDbfRecord>.Load("ADVENTURE_MODE", format)),
    (Action) (() => GameDbf.Banner = Dbf<BannerDbfRecord>.Load("BANNER", format)),
    (Action) (() => GameDbf.BattlegroundsBoardSkin = Dbf<BattlegroundsBoardSkinDbfRecord>.Load("BATTLEGROUNDS_BOARD_SKIN", format)),
    (Action) (() => GameDbf.BattlegroundsEmote = Dbf<BattlegroundsEmoteDbfRecord>.Load("BATTLEGROUNDS_EMOTE", format)),
    (Action) (() => GameDbf.BattlegroundsFinisher = Dbf<BattlegroundsFinisherDbfRecord>.Load("BATTLEGROUNDS_FINISHER", format)),
    (Action) (() => GameDbf.BattlegroundsGuideSkin = Dbf<BattlegroundsGuideSkinDbfRecord>.Load("BATTLEGROUNDS_GUIDE_SKIN", format)),
    (Action) (() => GameDbf.BattlegroundsHeroSkin = Dbf<BattlegroundsHeroSkinDbfRecord>.Load("BATTLEGROUNDS_HERO_SKIN", format)),
    (Action) (() => GameDbf.BattlegroundsSeason = Dbf<BattlegroundsSeasonDbfRecord>.Load("BATTLEGROUNDS_SEASON", format)),
    (Action) (() => GameDbf.Board = Dbf<BoardDbfRecord>.Load("BOARD", format)),
    (Action) (() => GameDbf.BonusBountyDropChance = Dbf<BonusBountyDropChanceDbfRecord>.Load("BONUS_BOUNTY_DROP_CHANCE", format)),
    (Action) (() => GameDbf.Booster = Dbf<BoosterDbfRecord>.Load("BOOSTER", format)),
    (Action) (() => GameDbf.BoosterCardSet = Dbf<BoosterCardSetDbfRecord>.Load("BOOSTER_CARD_SET", format)),
    (Action) (() => GameDbf.BuildingTier = Dbf<BuildingTierDbfRecord>.Load("BUILDING_TIER", format)),
    (Action) (() => GameDbf.Card = Dbf<CardDbfRecord>.Load("CARD", format)),
    (Action) (() => GameDbf.CardAdditonalSearchTerms = Dbf<CardAdditonalSearchTermsDbfRecord>.Load("CARD_ADDITONAL_SEARCH_TERMS", format)),
    (Action) (() => GameDbf.CardBack = Dbf<CardBackDbfRecord>.Load("CARD_BACK", format)),
    (Action) (() => GameDbf.CardChange = Dbf<CardChangeDbfRecord>.Load("CARD_CHANGE", format)),
    (Action) (() => GameDbf.CardDiscoverString = Dbf<CardDiscoverStringDbfRecord>.Load("CARD_DISCOVER_STRING", format)),
    (Action) (() => GameDbf.CardEquipmentAltText = Dbf<CardEquipmentAltTextDbfRecord>.Load("CARD_EQUIPMENT_ALT_TEXT", format)),
    (Action) (() => GameDbf.CardHero = Dbf<CardHeroDbfRecord>.Load("CARD_HERO", format)),
    (Action) (() => GameDbf.CardPlayerDeckOverride = Dbf<CardPlayerDeckOverrideDbfRecord>.Load("CARD_PLAYER_DECK_OVERRIDE", format)),
    (Action) (() => GameDbf.CardRace = Dbf<CardRaceDbfRecord>.Load("CARD_RACE", format)),
    (Action) (() => GameDbf.CardSet = Dbf<CardSetDbfRecord>.Load("CARD_SET", format)),
    (Action) (() => GameDbf.CardSetSpellOverride = Dbf<CardSetSpellOverrideDbfRecord>.Load("CARD_SET_SPELL_OVERRIDE", format)),
    (Action) (() => GameDbf.CardSetTiming = Dbf<CardSetTimingDbfRecord>.Load("CARD_SET_TIMING", format)),
    (Action) (() => GameDbf.CardTag = Dbf<CardTagDbfRecord>.Load("CARD_TAG", format)),
    (Action) (() => GameDbf.CardValue = Dbf<CardValueDbfRecord>.Load("CARD_VALUE", format)),
    (Action) (() => GameDbf.CharacterDialog = Dbf<CharacterDialogDbfRecord>.Load("CHARACTER_DIALOG", format)),
    (Action) (() => GameDbf.CharacterDialogItems = Dbf<CharacterDialogItemsDbfRecord>.Load("CHARACTER_DIALOG_ITEMS", format)),
    (Action) (() => GameDbf.Class = Dbf<ClassDbfRecord>.Load("CLASS", format)),
    (Action) (() => GameDbf.ClassExclusions = Dbf<ClassExclusionsDbfRecord>.Load("CLASS_EXCLUSIONS", format)),
    (Action) (() => GameDbf.ClientString = Dbf<ClientStringDbfRecord>.Load("CLIENT_STRING", format)),
    (Action) (() => GameDbf.Coin = Dbf<CoinDbfRecord>.Load("COIN", format)),
    (Action) (() => GameDbf.CreditsYear = Dbf<CreditsYearDbfRecord>.Load("CREDITS_YEAR", format)),
    (Action) (() => GameDbf.Deck = Dbf<DeckDbfRecord>.Load("DECK", format)),
    (Action) (() => GameDbf.DeckCard = Dbf<DeckCardDbfRecord>.Load("DECK_CARD", format)),
    (Action) (() => GameDbf.DeckRuleset = Dbf<DeckRulesetDbfRecord>.Load("DECK_RULESET", format)),
    (Action) (() => GameDbf.DeckRulesetRule = Dbf<DeckRulesetRuleDbfRecord>.Load("DECK_RULESET_RULE", format)),
    (Action) (() => GameDbf.DeckRulesetRuleSubset = Dbf<DeckRulesetRuleSubsetDbfRecord>.Load("DECK_RULESET_RULE_SUBSET", format)),
    (Action) (() => GameDbf.DeckTemplate = Dbf<DeckTemplateDbfRecord>.Load("DECK_TEMPLATE", format)),
    (Action) (() => GameDbf.DetailsVideoCue = Dbf<DetailsVideoCueDbfRecord>.Load("DETAILS_VIDEO_CUE", format)),
    (Action) (() => GameDbf.DkRuneList = Dbf<DkRuneListDbfRecord>.Load("DK_RUNE_LIST", format)),
    (Action) (() => GameDbf.DraftContent = Dbf<DraftContentDbfRecord>.Load("DRAFT_CONTENT", format)),
    (Action) (() => GameDbf.ExternalUrl = Dbf<ExternalUrlDbfRecord>.Load("EXTERNAL_URL", format)),
    (Action) (() => GameDbf.FixedReward = Dbf<FixedRewardDbfRecord>.Load("FIXED_REWARD", format)),
    (Action) (() => GameDbf.FixedRewardAction = Dbf<FixedRewardActionDbfRecord>.Load("FIXED_REWARD_ACTION", format)),
    (Action) (() => GameDbf.FixedRewardMap = Dbf<FixedRewardMapDbfRecord>.Load("FIXED_REWARD_MAP", format)),
    (Action) (() => GameDbf.GameMode = Dbf<GameModeDbfRecord>.Load("GAME_MODE", format)),
    (Action) (() => GameDbf.GameSaveSubkey = Dbf<GameSaveSubkeyDbfRecord>.Load("GAME_SAVE_SUBKEY", format)),
    (Action) (() => GameDbf.Global = Dbf<GlobalDbfRecord>.Load("GLOBAL", format)),
    (Action) (() => GameDbf.GuestHero = Dbf<GuestHeroDbfRecord>.Load("GUEST_HERO", format)),
    (Action) (() => GameDbf.GuestHeroSelectionRatio = Dbf<GuestHeroSelectionRatioDbfRecord>.Load("GUEST_HERO_SELECTION_RATIO", format)),
    (Action) (() => GameDbf.HiddenLicense = Dbf<HiddenLicenseDbfRecord>.Load("HIDDEN_LICENSE", format)),
    (Action) (() => GameDbf.InitCardValue = Dbf<InitCardValueDbfRecord>.Load("INIT_CARD_VALUE", format)),
    (Action) (() => GameDbf.KeywordText = Dbf<KeywordTextDbfRecord>.Load("KEYWORD_TEXT", format)),
    (Action) (() => GameDbf.League = Dbf<LeagueDbfRecord>.Load("LEAGUE", format)),
    (Action) (() => GameDbf.LeagueBgPublicRatingEquiv = Dbf<LeagueBgPublicRatingEquivDbfRecord>.Load("LEAGUE_BG_PUBLIC_RATING_EQUIV", format)),
    (Action) (() => GameDbf.LeagueGameType = Dbf<LeagueGameTypeDbfRecord>.Load("LEAGUE_GAME_TYPE", format)),
    (Action) (() => GameDbf.LeagueRank = Dbf<LeagueRankDbfRecord>.Load("LEAGUE_RANK", format)),
    (Action) (() => GameDbf.LettuceAbility = Dbf<LettuceAbilityDbfRecord>.Load("LETTUCE_ABILITY", format)),
    (Action) (() => GameDbf.LettuceAbilityTier = Dbf<LettuceAbilityTierDbfRecord>.Load("LETTUCE_ABILITY_TIER", format)),
    (Action) (() => GameDbf.LettuceBounty = Dbf<LettuceBountyDbfRecord>.Load("LETTUCE_BOUNTY", format)),
    (Action) (() => GameDbf.LettuceBountyFinalRewards = Dbf<LettuceBountyFinalRewardsDbfRecord>.Load("LETTUCE_BOUNTY_FINAL_REWARDS", format)),
    (Action) (() => GameDbf.LettuceBountySet = Dbf<LettuceBountySetDbfRecord>.Load("LETTUCE_BOUNTY_SET", format)),
    (Action) (() => GameDbf.LettuceEquipment = Dbf<LettuceEquipmentDbfRecord>.Load("LETTUCE_EQUIPMENT", format)),
    (Action) (() => GameDbf.LettuceEquipmentModifierData = Dbf<LettuceEquipmentModifierDataDbfRecord>.Load("LETTUCE_EQUIPMENT_MODIFIER_DATA", format)),
    (Action) (() => GameDbf.LettuceEquipmentTier = Dbf<LettuceEquipmentTierDbfRecord>.Load("LETTUCE_EQUIPMENT_TIER", format)),
    (Action) (() => GameDbf.LettuceMapBonusRewards = Dbf<LettuceMapBonusRewardsDbfRecord>.Load("LETTUCE_MAP_BONUS_REWARDS", format)),
    (Action) (() => GameDbf.LettuceMapNodeType = Dbf<LettuceMapNodeTypeDbfRecord>.Load("LETTUCE_MAP_NODE_TYPE", format)),
    (Action) (() => GameDbf.LettuceMapNodeTypeAnomaly = Dbf<LettuceMapNodeTypeAnomalyDbfRecord>.Load("LETTUCE_MAP_NODE_TYPE_ANOMALY", format)),
    (Action) (() => GameDbf.LettuceMercenary = Dbf<LettuceMercenaryDbfRecord>.Load("LETTUCE_MERCENARY", format)),
    (Action) (() => GameDbf.LettuceMercenaryAbility = Dbf<LettuceMercenaryAbilityDbfRecord>.Load("LETTUCE_MERCENARY_ABILITY", format)),
    (Action) (() => GameDbf.LettuceMercenaryEquipment = Dbf<LettuceMercenaryEquipmentDbfRecord>.Load("LETTUCE_MERCENARY_EQUIPMENT", format)),
    (Action) (() => GameDbf.LettuceMercenaryLevel = Dbf<LettuceMercenaryLevelDbfRecord>.Load("LETTUCE_MERCENARY_LEVEL", format)),
    (Action) (() => GameDbf.LettuceMercenaryLevelStats = Dbf<LettuceMercenaryLevelStatsDbfRecord>.Load("LETTUCE_MERCENARY_LEVEL_STATS", format)),
    (Action) (() => GameDbf.LettuceMercenarySpecialization = Dbf<LettuceMercenarySpecializationDbfRecord>.Load("LETTUCE_MERCENARY_SPECIALIZATION", format)),
    (Action) (() => GameDbf.LettuceTreasure = Dbf<LettuceTreasureDbfRecord>.Load("LETTUCE_TREASURE", format)),
    (Action) (() => GameDbf.LettuceTreasureTier = Dbf<LettuceTreasureTierDbfRecord>.Load("LETTUCE_TREASURE_TIER", format)),
    (Action) (() => GameDbf.LettuceTutorialVo = Dbf<LettuceTutorialVoDbfRecord>.Load("LETTUCE_TUTORIAL_VO", format)),
    (Action) (() => GameDbf.LoginPopupSequence = Dbf<LoginPopupSequenceDbfRecord>.Load("LOGIN_POPUP_SEQUENCE", format)),
    (Action) (() => GameDbf.LoginPopupSequencePopup = Dbf<LoginPopupSequencePopupDbfRecord>.Load("LOGIN_POPUP_SEQUENCE_POPUP", format)),
    (Action) (() => GameDbf.LoginReward = Dbf<LoginRewardDbfRecord>.Load("LOGIN_REWARD", format)),
    (Action) (() => GameDbf.LuckyDrawBox = Dbf<LuckyDrawBoxDbfRecord>.Load("LUCKY_DRAW_BOX", format)),
    (Action) (() => GameDbf.LuckyDrawRewards = Dbf<LuckyDrawRewardsDbfRecord>.Load("LUCKY_DRAW_REWARDS", format)),
    (Action) (() => GameDbf.MercTriggeredEvent = Dbf<MercTriggeredEventDbfRecord>.Load("MERC_TRIGGERED_EVENT", format)),
    (Action) (() => GameDbf.MercTriggeringEvent = Dbf<MercTriggeringEventDbfRecord>.Load("MERC_TRIGGERING_EVENT", format)),
    (Action) (() => GameDbf.MercenariesRandomReward = Dbf<MercenariesRandomRewardDbfRecord>.Load("MERCENARIES_RANDOM_REWARD", format)),
    (Action) (() => GameDbf.MercenariesRankedSeasonRewardRank = Dbf<MercenariesRankedSeasonRewardRankDbfRecord>.Load("MERCENARIES_RANKED_SEASON_REWARD_RANK", format)),
    (Action) (() => GameDbf.MercenaryArtVariation = Dbf<MercenaryArtVariationDbfRecord>.Load("MERCENARY_ART_VARIATION", format)),
    (Action) (() => GameDbf.MercenaryArtVariationPremium = Dbf<MercenaryArtVariationPremiumDbfRecord>.Load("MERCENARY_ART_VARIATION_PREMIUM", format)),
    (Action) (() => GameDbf.MercenaryBuilding = Dbf<MercenaryBuildingDbfRecord>.Load("MERCENARY_BUILDING", format)),
    (Action) (() => GameDbf.MercenaryVillageTrigger = Dbf<MercenaryVillageTriggerDbfRecord>.Load("MERCENARY_VILLAGE_TRIGGER", format)),
    (Action) (() => GameDbf.MercenaryVisitor = Dbf<MercenaryVisitorDbfRecord>.Load("MERCENARY_VISITOR", format)),
    (Action) (() => GameDbf.MiniSet = Dbf<MiniSetDbfRecord>.Load("MINI_SET", format)),
    (Action) (() => GameDbf.ModifiedLettuceAbilityCardTag = Dbf<ModifiedLettuceAbilityCardTagDbfRecord>.Load("MODIFIED_LETTUCE_ABILITY_CARD_TAG", format)),
    (Action) (() => GameDbf.ModifiedLettuceAbilityValue = Dbf<ModifiedLettuceAbilityValueDbfRecord>.Load("MODIFIED_LETTUCE_ABILITY_VALUE", format)),
    (Action) (() => GameDbf.ModularBundle = Dbf<ModularBundleDbfRecord>.Load("MODULAR_BUNDLE", format)),
    (Action) (() => GameDbf.ModularBundleLayout = Dbf<ModularBundleLayoutDbfRecord>.Load("MODULAR_BUNDLE_LAYOUT", format)),
    (Action) (() => GameDbf.ModularBundleLayoutNode = Dbf<ModularBundleLayoutNodeDbfRecord>.Load("MODULAR_BUNDLE_LAYOUT_NODE", format)),
    (Action) (() => GameDbf.MultiClassGroup = Dbf<MultiClassGroupDbfRecord>.Load("MULTI_CLASS_GROUP", format)),
    (Action) (() => GameDbf.NextTiers = Dbf<NextTiersDbfRecord>.Load("NEXT_TIERS", format)),
    (Action) (() => GameDbf.PowerDefinition = Dbf<PowerDefinitionDbfRecord>.Load("POWER_DEFINITION", format)),
    (Action) (() => GameDbf.Product = Dbf<ProductDbfRecord>.Load("PRODUCT", format)),
    (Action) (() => GameDbf.ProductClientData = Dbf<ProductClientDataDbfRecord>.Load("PRODUCT_CLIENT_DATA", format)),
    (Action) (() => GameDbf.PvpdrSeason = Dbf<PvpdrSeasonDbfRecord>.Load("PVPDR_SEASON", format)),
    (Action) (() => GameDbf.Quest = Dbf<QuestDbfRecord>.Load("QUEST", format)),
    (Action) (() => GameDbf.QuestDialog = Dbf<QuestDialogDbfRecord>.Load("QUEST_DIALOG", format)),
    (Action) (() => GameDbf.QuestDialogOnComplete = Dbf<QuestDialogOnCompleteDbfRecord>.Load("QUEST_DIALOG_ON_COMPLETE", format)),
    (Action) (() => GameDbf.QuestDialogOnProgress1 = Dbf<QuestDialogOnProgress1DbfRecord>.Load("QUEST_DIALOG_ON_PROGRESS1", format)),
    (Action) (() => GameDbf.QuestDialogOnProgress2 = Dbf<QuestDialogOnProgress2DbfRecord>.Load("QUEST_DIALOG_ON_PROGRESS2", format)),
    (Action) (() => GameDbf.QuestDialogOnReceived = Dbf<QuestDialogOnReceivedDbfRecord>.Load("QUEST_DIALOG_ON_RECEIVED", format)),
    (Action) (() => GameDbf.QuestModifier = Dbf<QuestModifierDbfRecord>.Load("QUEST_MODIFIER", format)),
    (Action) (() => GameDbf.QuestPool = Dbf<QuestPoolDbfRecord>.Load("QUEST_POOL", format)),
    (Action) (() => GameDbf.RegionOverrides = Dbf<RegionOverridesDbfRecord>.Load("REGION_OVERRIDES", format)),
    (Action) (() => GameDbf.RepeatableTaskList = Dbf<RepeatableTaskListDbfRecord>.Load("REPEATABLE_TASK_LIST", format)),
    (Action) (() => GameDbf.RewardBag = Dbf<RewardBagDbfRecord>.Load("REWARD_BAG", format)),
    (Action) (() => GameDbf.RewardChest = Dbf<RewardChestDbfRecord>.Load("REWARD_CHEST", format)),
    (Action) (() => GameDbf.RewardChestContents = Dbf<RewardChestContentsDbfRecord>.Load("REWARD_CHEST_CONTENTS", format)),
    (Action) (() => GameDbf.RewardItem = Dbf<RewardItemDbfRecord>.Load("REWARD_ITEM", format)),
    (Action) (() => GameDbf.RewardList = Dbf<RewardListDbfRecord>.Load("REWARD_LIST", format)),
    (Action) (() => GameDbf.RewardTrack = Dbf<RewardTrackDbfRecord>.Load("REWARD_TRACK", format)),
    (Action) (() => GameDbf.RewardTrackLevel = Dbf<RewardTrackLevelDbfRecord>.Load("REWARD_TRACK_LEVEL", format)),
    (Action) (() => GameDbf.Scenario = Dbf<ScenarioDbfRecord>.Load("SCENARIO", format)),
    (Action) (() => GameDbf.ScenarioGuestHeroes = Dbf<ScenarioGuestHeroesDbfRecord>.Load("SCENARIO_GUEST_HEROES", format)),
    (Action) (() => GameDbf.ScheduledCharacterDialog = Dbf<ScheduledCharacterDialogDbfRecord>.Load("SCHEDULED_CHARACTER_DIALOG", format)),
    (Action) (() => GameDbf.ScoreLabel = Dbf<ScoreLabelDbfRecord>.Load("SCORE_LABEL", format)),
    (Action) (() => GameDbf.SellableDeck = Dbf<SellableDeckDbfRecord>.Load("SELLABLE_DECK", format)),
    (Action) (() => GameDbf.ShopTier = Dbf<ShopTierDbfRecord>.Load("SHOP_TIER", format)),
    (Action) (() => GameDbf.ShopTierProductSale = Dbf<ShopTierProductSaleDbfRecord>.Load("SHOP_TIER_PRODUCT_SALE", format)),
    (Action) (() => GameDbf.Subset = Dbf<SubsetDbfRecord>.Load("SUBSET", format)),
    (Action) (() => GameDbf.SubsetCard = Dbf<SubsetCardDbfRecord>.Load("SUBSET_CARD", format)),
    (Action) (() => GameDbf.SubsetRule = Dbf<SubsetRuleDbfRecord>.Load("SUBSET_RULE", format)),
    (Action) (() => GameDbf.TaskList = Dbf<TaskListDbfRecord>.Load("TASK_LIST", format)),
    (Action) (() => GameDbf.TavernBrawlTicket = Dbf<TavernBrawlTicketDbfRecord>.Load("TAVERN_BRAWL_TICKET", format)),
    (Action) (() => GameDbf.TierProperties = Dbf<TierPropertiesDbfRecord>.Load("TIER_PROPERTIES", format)),
    (Action) (() => GameDbf.Trigger = Dbf<TriggerDbfRecord>.Load("TRIGGER", format)),
    (Action) (() => GameDbf.VisitorTask = Dbf<VisitorTaskDbfRecord>.Load("VISITOR_TASK", format)),
    (Action) (() => GameDbf.VisitorTaskChain = Dbf<VisitorTaskChainDbfRecord>.Load("VISITOR_TASK_CHAIN", format)),
    (Action) (() => GameDbf.Wing = Dbf<WingDbfRecord>.Load("WING", format)),
    (Action) (() => GameDbf.XpOnPlacement = Dbf<XpOnPlacementDbfRecord>.Load("XP_ON_PLACEMENT", format)),
    (Action) (() => GameDbf.XpOnPlacementGameTypeMultiplier = Dbf<XpOnPlacementGameTypeMultiplierDbfRecord>.Load("XP_ON_PLACEMENT_GAME_TYPE_MULTIPLIER", format)),
    (Action) (() => GameDbf.XpPerTimeGameTypeMultiplier = Dbf<XpPerTimeGameTypeMultiplierDbfRecord>.Load("XP_PER_TIME_GAME_TYPE_MULTIPLIER", format))
  };

  private static JobResultCollection GetLoadDbfJobs(DbfFormat format) => new JobResultCollection(new IAsyncJobResult[158]
  {
    (IAsyncJobResult) Dbf<AccountLicenseDbfRecord>.CreateLoadAsyncJob("ACCOUNT_LICENSE", format, ref GameDbf.AccountLicense),
    (IAsyncJobResult) Dbf<AchieveDbfRecord>.CreateLoadAsyncJob("ACHIEVE", format, ref GameDbf.Achieve),
    (IAsyncJobResult) Dbf<AchieveConditionDbfRecord>.CreateLoadAsyncJob("ACHIEVE_CONDITION", format, ref GameDbf.AchieveCondition),
    (IAsyncJobResult) Dbf<AchieveRegionDataDbfRecord>.CreateLoadAsyncJob("ACHIEVE_REGION_DATA", format, ref GameDbf.AchieveRegionData),
    (IAsyncJobResult) Dbf<AchievementDbfRecord>.CreateLoadAsyncJob("ACHIEVEMENT", format, ref GameDbf.Achievement),
    (IAsyncJobResult) Dbf<AchievementCategoryDbfRecord>.CreateLoadAsyncJob("ACHIEVEMENT_CATEGORY", format, ref GameDbf.AchievementCategory),
    (IAsyncJobResult) Dbf<AchievementSectionDbfRecord>.CreateLoadAsyncJob("ACHIEVEMENT_SECTION", format, ref GameDbf.AchievementSection),
    (IAsyncJobResult) Dbf<AchievementSectionItemDbfRecord>.CreateLoadAsyncJob("ACHIEVEMENT_SECTION_ITEM", format, ref GameDbf.AchievementSectionItem),
    (IAsyncJobResult) Dbf<AchievementSubcategoryDbfRecord>.CreateLoadAsyncJob("ACHIEVEMENT_SUBCATEGORY", format, ref GameDbf.AchievementSubcategory),
    (IAsyncJobResult) Dbf<AdventureDbfRecord>.CreateLoadAsyncJob("ADVENTURE", format, ref GameDbf.Adventure),
    (IAsyncJobResult) Dbf<AdventureDataDbfRecord>.CreateLoadAsyncJob("ADVENTURE_DATA", format, ref GameDbf.AdventureData),
    (IAsyncJobResult) Dbf<AdventureDeckDbfRecord>.CreateLoadAsyncJob("ADVENTURE_DECK", format, ref GameDbf.AdventureDeck),
    (IAsyncJobResult) Dbf<AdventureGuestHeroesDbfRecord>.CreateLoadAsyncJob("ADVENTURE_GUEST_HEROES", format, ref GameDbf.AdventureGuestHeroes),
    (IAsyncJobResult) Dbf<AdventureHeroPowerDbfRecord>.CreateLoadAsyncJob("ADVENTURE_HERO_POWER", format, ref GameDbf.AdventureHeroPower),
    (IAsyncJobResult) Dbf<AdventureLoadoutTreasuresDbfRecord>.CreateLoadAsyncJob("ADVENTURE_LOADOUT_TREASURES", format, ref GameDbf.AdventureLoadoutTreasures),
    (IAsyncJobResult) Dbf<AdventureMissionDbfRecord>.CreateLoadAsyncJob("ADVENTURE_MISSION", format, ref GameDbf.AdventureMission),
    (IAsyncJobResult) Dbf<AdventureModeDbfRecord>.CreateLoadAsyncJob("ADVENTURE_MODE", format, ref GameDbf.AdventureMode),
    (IAsyncJobResult) Dbf<BannerDbfRecord>.CreateLoadAsyncJob("BANNER", format, ref GameDbf.Banner),
    (IAsyncJobResult) Dbf<BattlegroundsBoardSkinDbfRecord>.CreateLoadAsyncJob("BATTLEGROUNDS_BOARD_SKIN", format, ref GameDbf.BattlegroundsBoardSkin),
    (IAsyncJobResult) Dbf<BattlegroundsEmoteDbfRecord>.CreateLoadAsyncJob("BATTLEGROUNDS_EMOTE", format, ref GameDbf.BattlegroundsEmote),
    (IAsyncJobResult) Dbf<BattlegroundsFinisherDbfRecord>.CreateLoadAsyncJob("BATTLEGROUNDS_FINISHER", format, ref GameDbf.BattlegroundsFinisher),
    (IAsyncJobResult) Dbf<BattlegroundsGuideSkinDbfRecord>.CreateLoadAsyncJob("BATTLEGROUNDS_GUIDE_SKIN", format, ref GameDbf.BattlegroundsGuideSkin),
    (IAsyncJobResult) Dbf<BattlegroundsHeroSkinDbfRecord>.CreateLoadAsyncJob("BATTLEGROUNDS_HERO_SKIN", format, ref GameDbf.BattlegroundsHeroSkin),
    (IAsyncJobResult) Dbf<BattlegroundsSeasonDbfRecord>.CreateLoadAsyncJob("BATTLEGROUNDS_SEASON", format, ref GameDbf.BattlegroundsSeason),
    (IAsyncJobResult) Dbf<BoardDbfRecord>.CreateLoadAsyncJob("BOARD", format, ref GameDbf.Board),
    (IAsyncJobResult) Dbf<BonusBountyDropChanceDbfRecord>.CreateLoadAsyncJob("BONUS_BOUNTY_DROP_CHANCE", format, ref GameDbf.BonusBountyDropChance),
    (IAsyncJobResult) Dbf<BoosterDbfRecord>.CreateLoadAsyncJob("BOOSTER", format, ref GameDbf.Booster),
    (IAsyncJobResult) Dbf<BoosterCardSetDbfRecord>.CreateLoadAsyncJob("BOOSTER_CARD_SET", format, ref GameDbf.BoosterCardSet),
    (IAsyncJobResult) Dbf<BuildingTierDbfRecord>.CreateLoadAsyncJob("BUILDING_TIER", format, ref GameDbf.BuildingTier),
    (IAsyncJobResult) Dbf<CardDbfRecord>.CreateLoadAsyncJob("CARD", format, ref GameDbf.Card),
    (IAsyncJobResult) Dbf<CardAdditonalSearchTermsDbfRecord>.CreateLoadAsyncJob("CARD_ADDITONAL_SEARCH_TERMS", format, ref GameDbf.CardAdditonalSearchTerms),
    (IAsyncJobResult) Dbf<CardBackDbfRecord>.CreateLoadAsyncJob("CARD_BACK", format, ref GameDbf.CardBack),
    (IAsyncJobResult) Dbf<CardChangeDbfRecord>.CreateLoadAsyncJob("CARD_CHANGE", format, ref GameDbf.CardChange),
    (IAsyncJobResult) Dbf<CardDiscoverStringDbfRecord>.CreateLoadAsyncJob("CARD_DISCOVER_STRING", format, ref GameDbf.CardDiscoverString),
    (IAsyncJobResult) Dbf<CardEquipmentAltTextDbfRecord>.CreateLoadAsyncJob("CARD_EQUIPMENT_ALT_TEXT", format, ref GameDbf.CardEquipmentAltText),
    (IAsyncJobResult) Dbf<CardHeroDbfRecord>.CreateLoadAsyncJob("CARD_HERO", format, ref GameDbf.CardHero),
    (IAsyncJobResult) Dbf<CardPlayerDeckOverrideDbfRecord>.CreateLoadAsyncJob("CARD_PLAYER_DECK_OVERRIDE", format, ref GameDbf.CardPlayerDeckOverride),
    (IAsyncJobResult) Dbf<CardRaceDbfRecord>.CreateLoadAsyncJob("CARD_RACE", format, ref GameDbf.CardRace),
    (IAsyncJobResult) Dbf<CardSetDbfRecord>.CreateLoadAsyncJob("CARD_SET", format, ref GameDbf.CardSet),
    (IAsyncJobResult) Dbf<CardSetSpellOverrideDbfRecord>.CreateLoadAsyncJob("CARD_SET_SPELL_OVERRIDE", format, ref GameDbf.CardSetSpellOverride),
    (IAsyncJobResult) Dbf<CardSetTimingDbfRecord>.CreateLoadAsyncJob("CARD_SET_TIMING", format, ref GameDbf.CardSetTiming),
    (IAsyncJobResult) Dbf<CardTagDbfRecord>.CreateLoadAsyncJob("CARD_TAG", format, ref GameDbf.CardTag),
    (IAsyncJobResult) Dbf<CardValueDbfRecord>.CreateLoadAsyncJob("CARD_VALUE", format, ref GameDbf.CardValue),
    (IAsyncJobResult) Dbf<CharacterDialogDbfRecord>.CreateLoadAsyncJob("CHARACTER_DIALOG", format, ref GameDbf.CharacterDialog),
    (IAsyncJobResult) Dbf<CharacterDialogItemsDbfRecord>.CreateLoadAsyncJob("CHARACTER_DIALOG_ITEMS", format, ref GameDbf.CharacterDialogItems),
    (IAsyncJobResult) Dbf<ClassDbfRecord>.CreateLoadAsyncJob("CLASS", format, ref GameDbf.Class),
    (IAsyncJobResult) Dbf<ClassExclusionsDbfRecord>.CreateLoadAsyncJob("CLASS_EXCLUSIONS", format, ref GameDbf.ClassExclusions),
    (IAsyncJobResult) Dbf<ClientStringDbfRecord>.CreateLoadAsyncJob("CLIENT_STRING", format, ref GameDbf.ClientString),
    (IAsyncJobResult) Dbf<CoinDbfRecord>.CreateLoadAsyncJob("COIN", format, ref GameDbf.Coin),
    (IAsyncJobResult) Dbf<CreditsYearDbfRecord>.CreateLoadAsyncJob("CREDITS_YEAR", format, ref GameDbf.CreditsYear),
    (IAsyncJobResult) Dbf<DeckDbfRecord>.CreateLoadAsyncJob("DECK", format, ref GameDbf.Deck),
    (IAsyncJobResult) Dbf<DeckCardDbfRecord>.CreateLoadAsyncJob("DECK_CARD", format, ref GameDbf.DeckCard),
    (IAsyncJobResult) Dbf<DeckRulesetDbfRecord>.CreateLoadAsyncJob("DECK_RULESET", format, ref GameDbf.DeckRuleset),
    (IAsyncJobResult) Dbf<DeckRulesetRuleDbfRecord>.CreateLoadAsyncJob("DECK_RULESET_RULE", format, ref GameDbf.DeckRulesetRule),
    (IAsyncJobResult) Dbf<DeckRulesetRuleSubsetDbfRecord>.CreateLoadAsyncJob("DECK_RULESET_RULE_SUBSET", format, ref GameDbf.DeckRulesetRuleSubset),
    (IAsyncJobResult) Dbf<DeckTemplateDbfRecord>.CreateLoadAsyncJob("DECK_TEMPLATE", format, ref GameDbf.DeckTemplate),
    (IAsyncJobResult) Dbf<DetailsVideoCueDbfRecord>.CreateLoadAsyncJob("DETAILS_VIDEO_CUE", format, ref GameDbf.DetailsVideoCue),
    (IAsyncJobResult) Dbf<DkRuneListDbfRecord>.CreateLoadAsyncJob("DK_RUNE_LIST", format, ref GameDbf.DkRuneList),
    (IAsyncJobResult) Dbf<DraftContentDbfRecord>.CreateLoadAsyncJob("DRAFT_CONTENT", format, ref GameDbf.DraftContent),
    (IAsyncJobResult) Dbf<ExternalUrlDbfRecord>.CreateLoadAsyncJob("EXTERNAL_URL", format, ref GameDbf.ExternalUrl),
    (IAsyncJobResult) Dbf<FixedRewardDbfRecord>.CreateLoadAsyncJob("FIXED_REWARD", format, ref GameDbf.FixedReward),
    (IAsyncJobResult) Dbf<FixedRewardActionDbfRecord>.CreateLoadAsyncJob("FIXED_REWARD_ACTION", format, ref GameDbf.FixedRewardAction),
    (IAsyncJobResult) Dbf<FixedRewardMapDbfRecord>.CreateLoadAsyncJob("FIXED_REWARD_MAP", format, ref GameDbf.FixedRewardMap),
    (IAsyncJobResult) Dbf<GameModeDbfRecord>.CreateLoadAsyncJob("GAME_MODE", format, ref GameDbf.GameMode),
    (IAsyncJobResult) Dbf<GameSaveSubkeyDbfRecord>.CreateLoadAsyncJob("GAME_SAVE_SUBKEY", format, ref GameDbf.GameSaveSubkey),
    (IAsyncJobResult) Dbf<GlobalDbfRecord>.CreateLoadAsyncJob("GLOBAL", format, ref GameDbf.Global),
    (IAsyncJobResult) Dbf<GuestHeroDbfRecord>.CreateLoadAsyncJob("GUEST_HERO", format, ref GameDbf.GuestHero),
    (IAsyncJobResult) Dbf<GuestHeroSelectionRatioDbfRecord>.CreateLoadAsyncJob("GUEST_HERO_SELECTION_RATIO", format, ref GameDbf.GuestHeroSelectionRatio),
    (IAsyncJobResult) Dbf<HiddenLicenseDbfRecord>.CreateLoadAsyncJob("HIDDEN_LICENSE", format, ref GameDbf.HiddenLicense),
    (IAsyncJobResult) Dbf<InitCardValueDbfRecord>.CreateLoadAsyncJob("INIT_CARD_VALUE", format, ref GameDbf.InitCardValue),
    (IAsyncJobResult) Dbf<KeywordTextDbfRecord>.CreateLoadAsyncJob("KEYWORD_TEXT", format, ref GameDbf.KeywordText),
    (IAsyncJobResult) Dbf<LeagueDbfRecord>.CreateLoadAsyncJob("LEAGUE", format, ref GameDbf.League),
    (IAsyncJobResult) Dbf<LeagueBgPublicRatingEquivDbfRecord>.CreateLoadAsyncJob("LEAGUE_BG_PUBLIC_RATING_EQUIV", format, ref GameDbf.LeagueBgPublicRatingEquiv),
    (IAsyncJobResult) Dbf<LeagueGameTypeDbfRecord>.CreateLoadAsyncJob("LEAGUE_GAME_TYPE", format, ref GameDbf.LeagueGameType),
    (IAsyncJobResult) Dbf<LeagueRankDbfRecord>.CreateLoadAsyncJob("LEAGUE_RANK", format, ref GameDbf.LeagueRank),
    (IAsyncJobResult) Dbf<LettuceAbilityDbfRecord>.CreateLoadAsyncJob("LETTUCE_ABILITY", format, ref GameDbf.LettuceAbility),
    (IAsyncJobResult) Dbf<LettuceAbilityTierDbfRecord>.CreateLoadAsyncJob("LETTUCE_ABILITY_TIER", format, ref GameDbf.LettuceAbilityTier),
    (IAsyncJobResult) Dbf<LettuceBountyDbfRecord>.CreateLoadAsyncJob("LETTUCE_BOUNTY", format, ref GameDbf.LettuceBounty),
    (IAsyncJobResult) Dbf<LettuceBountyFinalRewardsDbfRecord>.CreateLoadAsyncJob("LETTUCE_BOUNTY_FINAL_REWARDS", format, ref GameDbf.LettuceBountyFinalRewards),
    (IAsyncJobResult) Dbf<LettuceBountySetDbfRecord>.CreateLoadAsyncJob("LETTUCE_BOUNTY_SET", format, ref GameDbf.LettuceBountySet),
    (IAsyncJobResult) Dbf<LettuceEquipmentDbfRecord>.CreateLoadAsyncJob("LETTUCE_EQUIPMENT", format, ref GameDbf.LettuceEquipment),
    (IAsyncJobResult) Dbf<LettuceEquipmentModifierDataDbfRecord>.CreateLoadAsyncJob("LETTUCE_EQUIPMENT_MODIFIER_DATA", format, ref GameDbf.LettuceEquipmentModifierData),
    (IAsyncJobResult) Dbf<LettuceEquipmentTierDbfRecord>.CreateLoadAsyncJob("LETTUCE_EQUIPMENT_TIER", format, ref GameDbf.LettuceEquipmentTier),
    (IAsyncJobResult) Dbf<LettuceMapBonusRewardsDbfRecord>.CreateLoadAsyncJob("LETTUCE_MAP_BONUS_REWARDS", format, ref GameDbf.LettuceMapBonusRewards),
    (IAsyncJobResult) Dbf<LettuceMapNodeTypeDbfRecord>.CreateLoadAsyncJob("LETTUCE_MAP_NODE_TYPE", format, ref GameDbf.LettuceMapNodeType),
    (IAsyncJobResult) Dbf<LettuceMapNodeTypeAnomalyDbfRecord>.CreateLoadAsyncJob("LETTUCE_MAP_NODE_TYPE_ANOMALY", format, ref GameDbf.LettuceMapNodeTypeAnomaly),
    (IAsyncJobResult) Dbf<LettuceMercenaryDbfRecord>.CreateLoadAsyncJob("LETTUCE_MERCENARY", format, ref GameDbf.LettuceMercenary),
    (IAsyncJobResult) Dbf<LettuceMercenaryAbilityDbfRecord>.CreateLoadAsyncJob("LETTUCE_MERCENARY_ABILITY", format, ref GameDbf.LettuceMercenaryAbility),
    (IAsyncJobResult) Dbf<LettuceMercenaryEquipmentDbfRecord>.CreateLoadAsyncJob("LETTUCE_MERCENARY_EQUIPMENT", format, ref GameDbf.LettuceMercenaryEquipment),
    (IAsyncJobResult) Dbf<LettuceMercenaryLevelDbfRecord>.CreateLoadAsyncJob("LETTUCE_MERCENARY_LEVEL", format, ref GameDbf.LettuceMercenaryLevel),
    (IAsyncJobResult) Dbf<LettuceMercenaryLevelStatsDbfRecord>.CreateLoadAsyncJob("LETTUCE_MERCENARY_LEVEL_STATS", format, ref GameDbf.LettuceMercenaryLevelStats),
    (IAsyncJobResult) Dbf<LettuceMercenarySpecializationDbfRecord>.CreateLoadAsyncJob("LETTUCE_MERCENARY_SPECIALIZATION", format, ref GameDbf.LettuceMercenarySpecialization),
    (IAsyncJobResult) Dbf<LettuceTreasureDbfRecord>.CreateLoadAsyncJob("LETTUCE_TREASURE", format, ref GameDbf.LettuceTreasure),
    (IAsyncJobResult) Dbf<LettuceTreasureTierDbfRecord>.CreateLoadAsyncJob("LETTUCE_TREASURE_TIER", format, ref GameDbf.LettuceTreasureTier),
    (IAsyncJobResult) Dbf<LettuceTutorialVoDbfRecord>.CreateLoadAsyncJob("LETTUCE_TUTORIAL_VO", format, ref GameDbf.LettuceTutorialVo),
    (IAsyncJobResult) Dbf<LoginPopupSequenceDbfRecord>.CreateLoadAsyncJob("LOGIN_POPUP_SEQUENCE", format, ref GameDbf.LoginPopupSequence),
    (IAsyncJobResult) Dbf<LoginPopupSequencePopupDbfRecord>.CreateLoadAsyncJob("LOGIN_POPUP_SEQUENCE_POPUP", format, ref GameDbf.LoginPopupSequencePopup),
    (IAsyncJobResult) Dbf<LoginRewardDbfRecord>.CreateLoadAsyncJob("LOGIN_REWARD", format, ref GameDbf.LoginReward),
    (IAsyncJobResult) Dbf<LuckyDrawBoxDbfRecord>.CreateLoadAsyncJob("LUCKY_DRAW_BOX", format, ref GameDbf.LuckyDrawBox),
    (IAsyncJobResult) Dbf<LuckyDrawRewardsDbfRecord>.CreateLoadAsyncJob("LUCKY_DRAW_REWARDS", format, ref GameDbf.LuckyDrawRewards),
    (IAsyncJobResult) Dbf<MercTriggeredEventDbfRecord>.CreateLoadAsyncJob("MERC_TRIGGERED_EVENT", format, ref GameDbf.MercTriggeredEvent),
    (IAsyncJobResult) Dbf<MercTriggeringEventDbfRecord>.CreateLoadAsyncJob("MERC_TRIGGERING_EVENT", format, ref GameDbf.MercTriggeringEvent),
    (IAsyncJobResult) Dbf<MercenariesRandomRewardDbfRecord>.CreateLoadAsyncJob("MERCENARIES_RANDOM_REWARD", format, ref GameDbf.MercenariesRandomReward),
    (IAsyncJobResult) Dbf<MercenariesRankedSeasonRewardRankDbfRecord>.CreateLoadAsyncJob("MERCENARIES_RANKED_SEASON_REWARD_RANK", format, ref GameDbf.MercenariesRankedSeasonRewardRank),
    (IAsyncJobResult) Dbf<MercenaryArtVariationDbfRecord>.CreateLoadAsyncJob("MERCENARY_ART_VARIATION", format, ref GameDbf.MercenaryArtVariation),
    (IAsyncJobResult) Dbf<MercenaryArtVariationPremiumDbfRecord>.CreateLoadAsyncJob("MERCENARY_ART_VARIATION_PREMIUM", format, ref GameDbf.MercenaryArtVariationPremium),
    (IAsyncJobResult) Dbf<MercenaryBuildingDbfRecord>.CreateLoadAsyncJob("MERCENARY_BUILDING", format, ref GameDbf.MercenaryBuilding),
    (IAsyncJobResult) Dbf<MercenaryVillageTriggerDbfRecord>.CreateLoadAsyncJob("MERCENARY_VILLAGE_TRIGGER", format, ref GameDbf.MercenaryVillageTrigger),
    (IAsyncJobResult) Dbf<MercenaryVisitorDbfRecord>.CreateLoadAsyncJob("MERCENARY_VISITOR", format, ref GameDbf.MercenaryVisitor),
    (IAsyncJobResult) Dbf<MiniSetDbfRecord>.CreateLoadAsyncJob("MINI_SET", format, ref GameDbf.MiniSet),
    (IAsyncJobResult) Dbf<ModifiedLettuceAbilityCardTagDbfRecord>.CreateLoadAsyncJob("MODIFIED_LETTUCE_ABILITY_CARD_TAG", format, ref GameDbf.ModifiedLettuceAbilityCardTag),
    (IAsyncJobResult) Dbf<ModifiedLettuceAbilityValueDbfRecord>.CreateLoadAsyncJob("MODIFIED_LETTUCE_ABILITY_VALUE", format, ref GameDbf.ModifiedLettuceAbilityValue),
    (IAsyncJobResult) Dbf<ModularBundleDbfRecord>.CreateLoadAsyncJob("MODULAR_BUNDLE", format, ref GameDbf.ModularBundle),
    (IAsyncJobResult) Dbf<ModularBundleLayoutDbfRecord>.CreateLoadAsyncJob("MODULAR_BUNDLE_LAYOUT", format, ref GameDbf.ModularBundleLayout),
    (IAsyncJobResult) Dbf<ModularBundleLayoutNodeDbfRecord>.CreateLoadAsyncJob("MODULAR_BUNDLE_LAYOUT_NODE", format, ref GameDbf.ModularBundleLayoutNode),
    (IAsyncJobResult) Dbf<MultiClassGroupDbfRecord>.CreateLoadAsyncJob("MULTI_CLASS_GROUP", format, ref GameDbf.MultiClassGroup),
    (IAsyncJobResult) Dbf<NextTiersDbfRecord>.CreateLoadAsyncJob("NEXT_TIERS", format, ref GameDbf.NextTiers),
    (IAsyncJobResult) Dbf<PowerDefinitionDbfRecord>.CreateLoadAsyncJob("POWER_DEFINITION", format, ref GameDbf.PowerDefinition),
    (IAsyncJobResult) Dbf<ProductDbfRecord>.CreateLoadAsyncJob("PRODUCT", format, ref GameDbf.Product),
    (IAsyncJobResult) Dbf<ProductClientDataDbfRecord>.CreateLoadAsyncJob("PRODUCT_CLIENT_DATA", format, ref GameDbf.ProductClientData),
    (IAsyncJobResult) Dbf<PvpdrSeasonDbfRecord>.CreateLoadAsyncJob("PVPDR_SEASON", format, ref GameDbf.PvpdrSeason),
    (IAsyncJobResult) Dbf<QuestDbfRecord>.CreateLoadAsyncJob("QUEST", format, ref GameDbf.Quest),
    (IAsyncJobResult) Dbf<QuestDialogDbfRecord>.CreateLoadAsyncJob("QUEST_DIALOG", format, ref GameDbf.QuestDialog),
    (IAsyncJobResult) Dbf<QuestDialogOnCompleteDbfRecord>.CreateLoadAsyncJob("QUEST_DIALOG_ON_COMPLETE", format, ref GameDbf.QuestDialogOnComplete),
    (IAsyncJobResult) Dbf<QuestDialogOnProgress1DbfRecord>.CreateLoadAsyncJob("QUEST_DIALOG_ON_PROGRESS1", format, ref GameDbf.QuestDialogOnProgress1),
    (IAsyncJobResult) Dbf<QuestDialogOnProgress2DbfRecord>.CreateLoadAsyncJob("QUEST_DIALOG_ON_PROGRESS2", format, ref GameDbf.QuestDialogOnProgress2),
    (IAsyncJobResult) Dbf<QuestDialogOnReceivedDbfRecord>.CreateLoadAsyncJob("QUEST_DIALOG_ON_RECEIVED", format, ref GameDbf.QuestDialogOnReceived),
    (IAsyncJobResult) Dbf<QuestModifierDbfRecord>.CreateLoadAsyncJob("QUEST_MODIFIER", format, ref GameDbf.QuestModifier),
    (IAsyncJobResult) Dbf<QuestPoolDbfRecord>.CreateLoadAsyncJob("QUEST_POOL", format, ref GameDbf.QuestPool),
    (IAsyncJobResult) Dbf<RegionOverridesDbfRecord>.CreateLoadAsyncJob("REGION_OVERRIDES", format, ref GameDbf.RegionOverrides),
    (IAsyncJobResult) Dbf<RepeatableTaskListDbfRecord>.CreateLoadAsyncJob("REPEATABLE_TASK_LIST", format, ref GameDbf.RepeatableTaskList),
    (IAsyncJobResult) Dbf<RewardBagDbfRecord>.CreateLoadAsyncJob("REWARD_BAG", format, ref GameDbf.RewardBag),
    (IAsyncJobResult) Dbf<RewardChestDbfRecord>.CreateLoadAsyncJob("REWARD_CHEST", format, ref GameDbf.RewardChest),
    (IAsyncJobResult) Dbf<RewardChestContentsDbfRecord>.CreateLoadAsyncJob("REWARD_CHEST_CONTENTS", format, ref GameDbf.RewardChestContents),
    (IAsyncJobResult) Dbf<RewardItemDbfRecord>.CreateLoadAsyncJob("REWARD_ITEM", format, ref GameDbf.RewardItem),
    (IAsyncJobResult) Dbf<RewardListDbfRecord>.CreateLoadAsyncJob("REWARD_LIST", format, ref GameDbf.RewardList),
    (IAsyncJobResult) Dbf<RewardTrackDbfRecord>.CreateLoadAsyncJob("REWARD_TRACK", format, ref GameDbf.RewardTrack),
    (IAsyncJobResult) Dbf<RewardTrackLevelDbfRecord>.CreateLoadAsyncJob("REWARD_TRACK_LEVEL", format, ref GameDbf.RewardTrackLevel),
    (IAsyncJobResult) Dbf<ScenarioDbfRecord>.CreateLoadAsyncJob("SCENARIO", format, ref GameDbf.Scenario),
    (IAsyncJobResult) Dbf<ScenarioGuestHeroesDbfRecord>.CreateLoadAsyncJob("SCENARIO_GUEST_HEROES", format, ref GameDbf.ScenarioGuestHeroes),
    (IAsyncJobResult) Dbf<ScheduledCharacterDialogDbfRecord>.CreateLoadAsyncJob("SCHEDULED_CHARACTER_DIALOG", format, ref GameDbf.ScheduledCharacterDialog),
    (IAsyncJobResult) Dbf<ScoreLabelDbfRecord>.CreateLoadAsyncJob("SCORE_LABEL", format, ref GameDbf.ScoreLabel),
    (IAsyncJobResult) Dbf<SellableDeckDbfRecord>.CreateLoadAsyncJob("SELLABLE_DECK", format, ref GameDbf.SellableDeck),
    (IAsyncJobResult) Dbf<ShopTierDbfRecord>.CreateLoadAsyncJob("SHOP_TIER", format, ref GameDbf.ShopTier),
    (IAsyncJobResult) Dbf<ShopTierProductSaleDbfRecord>.CreateLoadAsyncJob("SHOP_TIER_PRODUCT_SALE", format, ref GameDbf.ShopTierProductSale),
    (IAsyncJobResult) Dbf<SubsetDbfRecord>.CreateLoadAsyncJob("SUBSET", format, ref GameDbf.Subset),
    (IAsyncJobResult) Dbf<SubsetCardDbfRecord>.CreateLoadAsyncJob("SUBSET_CARD", format, ref GameDbf.SubsetCard),
    (IAsyncJobResult) Dbf<SubsetRuleDbfRecord>.CreateLoadAsyncJob("SUBSET_RULE", format, ref GameDbf.SubsetRule),
    (IAsyncJobResult) Dbf<TaskListDbfRecord>.CreateLoadAsyncJob("TASK_LIST", format, ref GameDbf.TaskList),
    (IAsyncJobResult) Dbf<TavernBrawlTicketDbfRecord>.CreateLoadAsyncJob("TAVERN_BRAWL_TICKET", format, ref GameDbf.TavernBrawlTicket),
    (IAsyncJobResult) Dbf<TierPropertiesDbfRecord>.CreateLoadAsyncJob("TIER_PROPERTIES", format, ref GameDbf.TierProperties),
    (IAsyncJobResult) Dbf<TriggerDbfRecord>.CreateLoadAsyncJob("TRIGGER", format, ref GameDbf.Trigger),
    (IAsyncJobResult) Dbf<VisitorTaskDbfRecord>.CreateLoadAsyncJob("VISITOR_TASK", format, ref GameDbf.VisitorTask),
    (IAsyncJobResult) Dbf<VisitorTaskChainDbfRecord>.CreateLoadAsyncJob("VISITOR_TASK_CHAIN", format, ref GameDbf.VisitorTaskChain),
    (IAsyncJobResult) Dbf<WingDbfRecord>.CreateLoadAsyncJob("WING", format, ref GameDbf.Wing),
    (IAsyncJobResult) Dbf<XpOnPlacementDbfRecord>.CreateLoadAsyncJob("XP_ON_PLACEMENT", format, ref GameDbf.XpOnPlacement),
    (IAsyncJobResult) Dbf<XpOnPlacementGameTypeMultiplierDbfRecord>.CreateLoadAsyncJob("XP_ON_PLACEMENT_GAME_TYPE_MULTIPLIER", format, ref GameDbf.XpOnPlacementGameTypeMultiplier),
    (IAsyncJobResult) Dbf<XpPerTimeGameTypeMultiplierDbfRecord>.CreateLoadAsyncJob("XP_PER_TIME_GAME_TYPE_MULTIPLIER", format, ref GameDbf.XpPerTimeGameTypeMultiplier)
  });

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield return (IAsyncJobResult) GameDbf.CreateLoadDbfJob();
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
  }

  public static string GetDOPAssetPath(Locale locale) => "Assets/Game/DBF-Asset/" + (object) locale + "/DOPAsset.asset";

  public static GameDbfIndex GetIndex()
  {
    if (GameDbf.s_index == null)
      GameDbf.s_index = new GameDbfIndex();
    return GameDbf.s_index;
  }

  public static bool ShouldForceXmlLoading()
  {
    if (HearthstoneApplication.IsPublic())
      return false;
    if (HearthstoneApplication.UsingStandaloneLocalData())
      return true;
    object option = Options.Get().GetOption(Option.DBF_XML_LOADING);
    return option != null ? (bool) option : Application.isEditor;
  }

  public static JobDefinition CreateLoadDbfJob() => new JobDefinition("GameDbf.Load", GameDbf.Load(), Array.Empty<IJobDependency>());

  public static void LoadXml()
  {
    IEnumerator enumerator = (IEnumerator) GameDbf.Load(true, false);
    do
      ;
    while (enumerator.MoveNext());
  }

  public static IEnumerator<IAsyncJobResult> Load() => GameDbf.Load(false, true);

  public static IEnumerator<IAsyncJobResult> Load(bool useXmlLoading) => GameDbf.Load(useXmlLoading, true);

  public static IEnumerator<IAsyncJobResult> Load(
    bool useXmlLoading,
    bool useAssetJobs)
  {
    if (HearthstoneApplication.IsHearthstoneRunning)
      yield return (IAsyncJobResult) new WaitForGameDownloadManagerState();
    if (GameDbf.s_index == null)
      GameDbf.s_index = new GameDbfIndex();
    else
      GameDbf.s_index.Initialize();
    if (GameDbf.ShouldForceXmlLoading())
      useXmlLoading = true;
    DbfFormat format = useXmlLoading ? DbfFormat.XML : DbfFormat.ASSET;
    DbfShared.Reset();
    if (!useXmlLoading)
    {
      if (useAssetJobs)
        yield return (IAsyncJobResult) new JobDefinition("GameDbf.LoadDBFSharedAssetBundle", DbfShared.Job_LoadSharedDBFAssetBundle(), Array.Empty<IJobDependency>());
      else
        DbfShared.LoadSharedAssetBundle();
    }
    Log.Dbf?.Print("Loading DBFS with format={0}", (object) format);
    Action[] loadActions = (Action[]) null;
    CPUTimeSoftYield softYielder = new CPUTimeSoftYield((float) (1.0 / (double) Application.targetFrameRate * 0.800000011920929));
    int index;
    if (!useAssetJobs)
    {
      loadActions = GameDbf.GetLoadDbfActions(format);
      Action[] actionArray = loadActions;
      for (index = 0; index < actionArray.Length; ++index)
      {
        actionArray[index]();
        if (softYielder.ShouldSoftYield())
        {
          yield return (IAsyncJobResult) null;
          softYielder.NewFrame();
        }
      }
      actionArray = (Action[]) null;
    }
    else
      yield return (IAsyncJobResult) GameDbf.GetLoadDbfJobs(format);
    loadActions = GameDbf.GetPostProcessDbfActions();
    for (index = 0; index < loadActions.Length; ++index)
    {
      loadActions[index]();
      if (softYielder.ShouldSoftYield())
      {
        yield return (IAsyncJobResult) null;
        softYielder.NewFrame();
      }
    }
    GameDbf.IsLoaded = true;
    GameDbf.SetDbfCallbacksForIndexing();
  }

  private static Action[] GetPostProcessDbfActions() => new Action[20]
  {
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_MercenaryEquipmentUnlock()),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_MercenaryArtVariationUnlock()),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_CardTag(GameDbf.CardTag)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_CardChange(GameDbf.CardChange)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_Card(GameDbf.Card)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_CardDiscoverString(GameDbf.CardDiscoverString)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_CardSetSpellOverride(GameDbf.CardSetSpellOverride)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_DeckRulesetRule(GameDbf.DeckRulesetRule)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_DeckRulesetRuleSubset(GameDbf.DeckRulesetRuleSubset)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_FixedRewardAction(GameDbf.FixedRewardAction)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_FixedRewardMap(GameDbf.FixedRewardMap)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_FixedReward(GameDbf.FixedReward)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_SubsetCard(GameDbf.SubsetCard)),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_CardPlayerDeckOverride(GameDbf.CardPlayerDeckOverride)),
    (Action) (() => RankMgr.Get().PostProcessDbfLoad_League()),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_LettuceEquipmentTier()),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_VisitorTask()),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_Achievement()),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_MercenaryLevel()),
    (Action) (() => GameDbf.s_index.PostProcessDbfLoad_MercenaryArtVariation())
  };

  private static void SetDbfCallbacksForIndexing()
  {
    GameDbf.CardTag.AddListeners(new Dbf<CardTagDbfRecord>.RecordAddedListener(GameDbf.s_index.OnCardTagAdded), new Dbf<CardTagDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnCardTagRemoved));
    GameDbf.CardChange.AddListeners(new Dbf<CardChangeDbfRecord>.RecordAddedListener(GameDbf.s_index.OnCardChangeAdded), new Dbf<CardChangeDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnCardChangeRemoved));
    GameDbf.Card.AddListeners(new Dbf<CardDbfRecord>.RecordAddedListener(GameDbf.s_index.OnCardAdded), new Dbf<CardDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnCardRemoved));
    GameDbf.CardDiscoverString.AddListeners(new Dbf<CardDiscoverStringDbfRecord>.RecordAddedListener(GameDbf.s_index.OnCardDiscoverStringAdded), new Dbf<CardDiscoverStringDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnCardDiscoverStringRemoved));
    GameDbf.CardSetSpellOverride.AddListeners(new Dbf<CardSetSpellOverrideDbfRecord>.RecordAddedListener(GameDbf.s_index.OnCardSetSpellOverrideAdded), new Dbf<CardSetSpellOverrideDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnCardSetSpellOverrideRemoved));
    GameDbf.CardPlayerDeckOverride.AddListeners(new Dbf<CardPlayerDeckOverrideDbfRecord>.RecordAddedListener(GameDbf.s_index.OnCardPlayerDeckOverrideAdded), new Dbf<CardPlayerDeckOverrideDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnCardPlayerDeckOverrideRemoved));
    GameDbf.DeckRulesetRule.AddListeners(new Dbf<DeckRulesetRuleDbfRecord>.RecordAddedListener(GameDbf.s_index.OnDeckRulesetRuleAdded), new Dbf<DeckRulesetRuleDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnDeckRulesetRuleRemoved));
    GameDbf.DeckRulesetRuleSubset.AddListeners(new Dbf<DeckRulesetRuleSubsetDbfRecord>.RecordAddedListener(GameDbf.s_index.OnDeckRulesetRuleSubsetAdded), new Dbf<DeckRulesetRuleSubsetDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnDeckRulesetRuleSubsetRemoved));
    GameDbf.FixedRewardAction.AddListeners(new Dbf<FixedRewardActionDbfRecord>.RecordAddedListener(GameDbf.s_index.OnFixedRewardActionAdded), new Dbf<FixedRewardActionDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnFixedRewardActionRemoved));
    GameDbf.FixedRewardMap.AddListeners(new Dbf<FixedRewardMapDbfRecord>.RecordAddedListener(GameDbf.s_index.OnFixedRewardMapAdded), new Dbf<FixedRewardMapDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnFixedRewardMapRemoved));
    GameDbf.SubsetCard.AddListeners(new Dbf<SubsetCardDbfRecord>.RecordAddedListener(GameDbf.s_index.OnSubsetCardAdded), new Dbf<SubsetCardDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnSubsetCardRemoved));
    GameDbf.LettuceEquipmentTier.AddListeners(new Dbf<LettuceEquipmentTierDbfRecord>.RecordAddedListener(GameDbf.s_index.OnLettuceEquipmentTierAdded), new Dbf<LettuceEquipmentTierDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnLettuceEquipmentTierRemoved));
    GameDbf.LettuceMercenaryLevel.AddListeners(new Dbf<LettuceMercenaryLevelDbfRecord>.RecordAddedListener(GameDbf.s_index.OnMercenaryLevelAdded), new Dbf<LettuceMercenaryLevelDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnMercenaryLevelRemoved));
  }

  public static void Reload(string name, string xml)
  {
    if (!(name == "ACHIEVE"))
    {
      if (name == "CARD_BACK")
      {
        GameDbf.CardBack = Dbf<CardBackDbfRecord>.Load(name, DbfFormat.XML);
        CardBackManager service;
        if (!ServiceManager.TryGet<CardBackManager>(out service))
          return;
        service.InitCardBackData();
      }
      else
        Error.AddDevFatal("Reloading {0} is unsupported", (object) name);
    }
    else
    {
      GameDbf.Achieve = Dbf<AchieveDbfRecord>.Load(name, DbfFormat.XML);
      AchieveManager service;
      if (!ServiceManager.TryGet<AchieveManager>(out service))
        return;
      service.InitAchieveManager();
    }
  }

  public static int GetDataVersion() => GameDbf.GetDOPAsset().DataVersion;

  private static DOPAsset GetDOPAsset()
  {
    if ((UnityEngine.Object) GameDbf.s_DOPAsset == (UnityEngine.Object) null)
    {
      if (Application.isEditor)
      {
        GameDbf.s_DOPAsset = DOPAsset.GenerateDOPAsset();
      }
      else
      {
        AssetBundle assetBundle = DbfShared.GetAssetBundle();
        if ((UnityEngine.Object) assetBundle != (UnityEngine.Object) null)
          GameDbf.s_DOPAsset = assetBundle.LoadAsset<DOPAsset>(GameDbf.GetDOPAssetPath(Localization.GetActualLocale()));
        if ((UnityEngine.Object) GameDbf.s_DOPAsset == (UnityEngine.Object) null)
        {
          Log.Dbf.PrintWarning("Failed to load DOP asset, generating default...");
          GameDbf.s_DOPAsset = DOPAsset.GenerateDOPAsset();
        }
      }
    }
    return GameDbf.s_DOPAsset;
  }

  public static IEnumerable<IDbf> AllDbfs => (IEnumerable<IDbf>) GameDbf.s_allDbfs.Values;

  public static void RegisterDbf(IDbf dbf) => GameDbf.s_allDbfs[dbf.GetName()] = dbf;

  public static IDbf GetIDbf(string name)
  {
    IDbf idbf;
    GameDbf.s_allDbfs.TryGetValue(name, out idbf);
    return idbf;
  }
}
