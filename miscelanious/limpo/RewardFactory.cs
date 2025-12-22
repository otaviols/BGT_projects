using Assets;
using Hearthstone;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using PegasusUtil;
using Shared.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;

public class RewardFactory
{
  public static List<RewardItemDataModel> CreateRewardItemDataModel(
    RewardItemDbfRecord itemRecord,
    RewardItemOutputData rewardItemOutputData = null)
  {
    List<RewardItemDataModel> rewardItemDataModel1 = (List<RewardItemDataModel>) null;
    RewardItemDataModel rewardItemDataModel2 = (RewardItemDataModel) null;
    bool flag = false;
    switch (itemRecord.RewardType)
    {
      case RewardItem.RewardType.GOLD:
      case RewardItem.RewardType.TAVERN_TICKET:
      case RewardItem.RewardType.REWARD_TRACK_XP_BOOST:
        rewardItemDataModel2 = RewardFactory.CreateSimpleRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.DUST:
      case RewardItem.RewardType.ARCANE_ORBS:
      case RewardItem.RewardType.RENOWN:
        rewardItemDataModel2 = RewardFactory.CreateCurrencyRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.BOOSTER:
        rewardItemDataModel2 = RewardFactory.CreateBoosterRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.CARD:
        rewardItemDataModel2 = RewardFactory.CreateCardRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.RANDOM_CARD:
        rewardItemDataModel2 = RewardFactory.CreateRandomCardRewardItemDataModel(itemRecord, rewardItemOutputData);
        break;
      case RewardItem.RewardType.CARD_BACK:
        rewardItemDataModel2 = RewardFactory.CreateCardBackRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.HERO_SKIN:
        rewardItemDataModel2 = RewardFactory.CreateHeroSkinRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.CUSTOM_COIN:
        rewardItemDataModel2 = RewardFactory.CreateCustomCoinRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.CARD_SUBSET:
        rewardItemDataModel1 = RewardFactory.CreateCardSubsetRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.MERCENARY_CURRENCY:
        rewardItemDataModel2 = RewardFactory.CreateMercenaryCoinRewardItemDataModel(itemRecord, rewardItemOutputData);
        break;
      case RewardItem.RewardType.MERCENARY_EQUIPMENT:
        rewardItemDataModel2 = RewardFactory.CreateMercenaryEquipRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.MERCENARY_XP:
        rewardItemDataModel2 = RewardFactory.CreateMercenaryXPRewardItemDataModel(itemRecord, rewardItemOutputData);
        break;
      case RewardItem.RewardType.MERCENARY:
        rewardItemDataModel2 = rewardItemOutputData == null || !rewardItemOutputData.HasAmount || rewardItemOutputData.Amount <= 1 || rewardItemOutputData.ArtVariationId != 0 ? RewardFactory.CreateMercenaryRewardItemDataModel(itemRecord, rewardItemOutputData) : RewardFactory.CreateMercenaryCoinRewardItemDataModel(itemRecord, rewardItemOutputData);
        break;
      case RewardItem.RewardType.BATTLEGROUNDS_HERO_SKIN:
        rewardItemDataModel2 = RewardFactory.CreateBattlegroundsHeroSkinRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.BATTLEGROUNDS_GUIDE_SKIN:
        rewardItemDataModel2 = RewardFactory.CreateBattlegroundsGuideSkinRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.BATTLEGROUNDS_BOARD_SKIN:
        rewardItemDataModel2 = RewardFactory.CreateBattlegroundsBoardSkinRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.BATTLEGROUNDS_FINISHER:
        rewardItemDataModel2 = RewardFactory.CreateBattlegroundsFinisherRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.BATTLEGROUNDS_EMOTE:
        rewardItemDataModel2 = RewardFactory.CreateBattlegroundsEmoteRewardItemDataModel(itemRecord);
        break;
      case RewardItem.RewardType.BATTLEGROUNDS_SEASON_BONUS:
        rewardItemDataModel2 = RewardFactory.CreateSeasonBonusRewardItemDataModel(itemRecord);
        break;
      default:
        flag = true;
        break;
    }
    if (rewardItemDataModel1 == null && rewardItemDataModel2 == null)
    {
      if (flag)
        Log.All.PrintWarning(string.Format("RewardItem has unsupported item type [itemid {0}, type {1}]", (object) itemRecord.ID, (object) itemRecord.RewardType));
      else
        Log.All.PrintWarning(string.Format("Failed creating RewardItem data model [itemid {0}, rewardtype {1}]", (object) itemRecord.ID, (object) itemRecord.RewardType));
      return new List<RewardItemDataModel>();
    }
    if (rewardItemDataModel1 != null)
      return rewardItemDataModel1;
    return new List<RewardItemDataModel>()
    {
      rewardItemDataModel2
    };
  }

  private static RewardItemDataModel CreateBoosterRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    int num = record.Booster;
    if (num == 0)
      num = (int) GameUtils.GetRewardableBoosterFromSelector(record.BoosterSelector);
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = num,
      Booster = new PackDataModel()
      {
        Type = (BoosterDbId) num,
        Quantity = record.Quantity
      }
    };
  }

  private static RewardItemDataModel CreateCardRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    CardDbfRecord record1 = GameDbf.Card.GetRecord(record.Card);
    if (record1 == null)
    {
      Log.All.PrintWarning(string.Format("Card Item has unknown card id [{0}]", (object) record.Card));
      return (RewardItemDataModel) null;
    }
    CardTagDbfRecord cardTagDbfRecord = record1.Tags.Find((Predicate<CardTagDbfRecord>) (tagRecord => tagRecord.TagId == 203));
    string str = cardTagDbfRecord == null ? "" : GameStrings.GetRarityTextKey((TAG_RARITY) cardTagDbfRecord.TagValue);
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = record.Card,
      Card = new CardDataModel()
      {
        CardId = record1.NoteMiniGuid,
        Premium = (TAG_PREMIUM) record.CardPremiumLevel,
        FlavorText = (string) record1.FlavorText,
        Rarity = str
      }
    };
  }

  private static List<RewardItemDataModel> CreateCardSubsetRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    List<RewardItemDataModel> rewardItemDataModel = new List<RewardItemDataModel>();
    foreach (string cardId in GameDbf.GetIndex().GetSubsetById(record.SubsetId))
    {
      CardDbfRecord record1 = GameDbf.Card.GetRecord(GameUtils.TranslateCardIdToDbId(cardId));
      rewardItemDataModel.Add(new RewardItemDataModel()
      {
        AssetId = record.ID,
        ItemType = RewardItemType.CARD,
        Quantity = record.Quantity,
        ItemId = record1.ID,
        Card = new CardDataModel()
        {
          CardId = record1.NoteMiniGuid,
          Premium = (TAG_PREMIUM) record.CardPremiumLevel
        }
      });
    }
    return rewardItemDataModel;
  }

  private static RewardItemDataModel CreateMercenaryRewardItemDataModel(
    RewardItemDbfRecord record,
    RewardItemOutputData rewardItemOutputData = null)
  {
    int num = record.Mercenary;
    int artVariationId = record.MercenaryArtVariation;
    TAG_PREMIUM premium = (TAG_PREMIUM) record.MercenaryArtPremium;
    TAG_RARITY mercenaryRarity = (TAG_RARITY) record.MercenaryRarity;
    if (rewardItemOutputData != null)
    {
      if (rewardItemOutputData.HasMercenaryId && rewardItemOutputData.MercenaryId > 0)
        num = rewardItemOutputData.MercenaryId;
      if (rewardItemOutputData.HasArtVariationId && rewardItemOutputData.ArtVariationId > 0)
        artVariationId = rewardItemOutputData.ArtVariationId;
      if (rewardItemOutputData.HasPremium)
        premium = (TAG_PREMIUM) rewardItemOutputData.Premium;
    }
    RewardItemDataModel rewardItemDataModel;
    if (record.MercenarySelector == RewardItem.MercenarySelector.RANDOM && num == 0)
      rewardItemDataModel = new RewardItemDataModel()
      {
        Quantity = 1,
        ItemType = RewardItemType.MERCENARY_RANDOM_MERCENARY,
        RandomMercenary = new LettuceRandomMercenaryDataModel()
        {
          Premium = premium,
          Rarity = mercenaryRarity
        }
      };
    else if (record.MercenarySelector == RewardItem.MercenarySelector.SPECIFIC && artVariationId == 0)
    {
      LettuceMercenary mercenary = CollectionManager.Get()?.GetMercenary((long) num, ReportError: false);
      LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(num, artVariationId, premium, mercenary);
      mercenaryDataModel.Owned = true;
      rewardItemDataModel = new RewardItemDataModel()
      {
        Quantity = 1,
        ItemType = RewardItemType.MERCENARY_KNOCKOUT_SPECIFIC,
        Mercenary = mercenaryDataModel,
        IsMercenaryPortrait = RewardUtils.IsMercenaryRewardPortrait(mercenaryDataModel) && mercenary != null && mercenary.m_owned
      };
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get()?.GetMercenary((long) num, ReportError: false);
      LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(num, artVariationId, premium, mercenary);
      mercenaryDataModel.Owned = true;
      rewardItemDataModel = new RewardItemDataModel()
      {
        Quantity = 1,
        ItemType = RewardItemType.MERCENARY,
        Mercenary = mercenaryDataModel,
        IsMercenaryPortrait = RewardUtils.IsMercenaryRewardPortrait(mercenaryDataModel)
      };
    }
    return rewardItemDataModel;
  }

  public static LettuceMercenaryDataModel CreateFullyUpgradedMercenaryDataModel(
    int MercenaryId)
  {
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) MercenaryId);
    LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(mercenary);
    CollectionUtils.SetMercenaryStatsByLevel(mercenaryDataModel, MercenaryId, mercenary.m_level, false);
    mercenaryDataModel.FullyUpgradedFinal = true;
    return mercenaryDataModel;
  }

  private static int GetMercenariesIdForRewardItem(
    RewardItemDbfRecord record,
    RewardItemOutputData rewardItemOutputData)
  {
    switch (record.MercenarySelector)
    {
      case RewardItem.MercenarySelector.CONTEXT:
        if (LettuceVillageDataUtil.CurrentTaskContext > 0)
          return LettuceVillageDataUtil.CurrentTaskContext;
        if (rewardItemOutputData != null)
          return rewardItemOutputData.MercenaryId;
        break;
      case RewardItem.MercenarySelector.RANDOM:
        if (rewardItemOutputData != null)
          return rewardItemOutputData.MercenaryId;
        break;
    }
    return record.Mercenary;
  }

  private static RewardItemDataModel CreateMercenaryXPRewardItemDataModel(
    RewardItemDbfRecord record,
    RewardItemOutputData rewardItemOutputData = null)
  {
    int mercenariesIdForRewardItem = RewardFactory.GetMercenariesIdForRewardItem(record, rewardItemOutputData);
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenariesIdForRewardItem);
    CardDbfRecord cardRecord = mercenary.GetCardRecord();
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = RewardItemType.MERCENARY_XP,
      Quantity = record.Quantity,
      ItemId = mercenariesIdForRewardItem,
      Card = new CardDataModel()
      {
        CardId = cardRecord.NoteMiniGuid,
        Premium = TAG_PREMIUM.NORMAL,
        FlavorText = (string) cardRecord?.FlavorText
      },
      Mercenary = MercenaryFactory.CreateMercenaryDataModel(mercenary)
    };
  }

  private static RewardItemDataModel CreateMercenaryCoinRewardItemDataModel(
    RewardItemDbfRecord record,
    RewardItemOutputData rewardItemOutputData = null)
  {
    int mercenariesIdForRewardItem = RewardFactory.GetMercenariesIdForRewardItem(record, rewardItemOutputData);
    int num = record.Quantity;
    if (rewardItemOutputData != null && rewardItemOutputData.HasAmount && rewardItemOutputData.Amount > 0)
      num = rewardItemOutputData.Amount;
    int quantity = num;
    return RewardUtils.CreateMercenaryCoinsRewardData(mercenariesIdForRewardItem, quantity, false, false).DataModel;
  }

  private static RewardItemDataModel CreateMercenaryEquipRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    int mercenary1 = record.Mercenary;
    LettuceMercenary mercenary2 = CollectionManager.Get().GetMercenary((long) mercenary1);
    if (mercenary2 == null)
    {
      Log.All.PrintWarning(string.Format("MercenaryID not found [{0}]", (object) mercenary1));
      return (RewardItemDataModel) null;
    }
    LettuceAbility lettuceEquipment = mercenary2.GetLettuceEquipment(record.MercenaryEquipment);
    if (lettuceEquipment == null)
    {
      Log.All.PrintWarning(string.Format("Equipment ID for reward not found [mercid {0}, equipid {1}]", (object) mercenary1, (object) record.MercenaryEquipment));
      return (RewardItemDataModel) null;
    }
    LettuceAbilityDataModel dataModel = new LettuceAbilityDataModel();
    CollectionUtils.PopulateDefaultAbilityDataModelWithTier(dataModel, lettuceEquipment, mercenary2, lettuceEquipment.GetBaseTier());
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = RewardItemType.MERCENARY_EQUIPMENT,
      Quantity = record.Quantity,
      ItemId = mercenary1,
      Mercenary = MercenaryFactory.CreateMercenaryDataModel(mercenary2),
      MercenaryEquip = dataModel
    };
  }

  private static RewardItemDataModel CreateCardBackRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    if (!GameDbf.CardBack.HasRecord(record.CardBack))
    {
      Log.All.PrintWarning(string.Format("Card Back Item has unrecognized card back id [{0}]", (object) record.CardBack));
      return (RewardItemDataModel) null;
    }
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = record.CardBack,
      CardBack = new CardBackDataModel()
      {
        CardBackId = record.CardBack
      }
    };
  }

  private static RewardItemDataModel CreateCurrencyRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    RewardItemType rewardItemType = record.RewardType.ToRewardItemType();
    CurrencyType currencyType = RewardUtils.RewardItemTypeToCurrencyType(rewardItemType);
    if (ShopUtils.IsCurrencyVirtual(currencyType) && !ShopUtils.IsVirtualCurrencyEnabled())
      return (RewardItemDataModel) null;
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = rewardItemType,
      Quantity = record.Quantity,
      Currency = new PriceDataModel()
      {
        Currency = currencyType,
        Amount = (float) record.Quantity,
        DisplayText = record.Quantity.ToString()
      }
    };
  }

  private static RewardItemDataModel CreateCustomCoinRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    CoinDbfRecord record1 = GameDbf.Coin.GetRecord(record.CustomCoin);
    if (record1 == null)
    {
      Log.All.PrintWarning(string.Format("Custom Coin Item has unknown id [{0}]", (object) record.CustomCoin));
      return (RewardItemDataModel) null;
    }
    CardDbfRecord record2 = GameDbf.Card.GetRecord(record1.CardId);
    if (record2 == null)
    {
      Log.All.PrintWarning(string.Format("Custom Coin Item has unknown card id [{0}]", (object) record1.CardId));
      return (RewardItemDataModel) null;
    }
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = record.CustomCoin,
      Card = new CardDataModel()
      {
        CardId = record2.NoteMiniGuid,
        Premium = (TAG_PREMIUM) record.CardPremiumLevel
      }
    };
  }

  private static RewardItemDataModel CreateBattlegroundsEmoteRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    BattlegroundsEmoteDbfRecord record1 = GameDbf.BattlegroundsEmote.GetRecord(record.BattlegroundsEmoteId);
    if (record1 == null)
    {
      Log.All.PrintWarning(string.Format("Battleground Emote has unknown id [{0}]", (object) record.BattlegroundsEmoteId));
      return (RewardItemDataModel) null;
    }
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = record.BattlegroundsEmoteId,
      BGEmote = new BattlegroundsEmoteDataModel()
      {
        DisplayName = (string) record1.CollectionShortName,
        Description = (string) record1.Description,
        EmoteDbiId = record1.ID,
        Animation = record1.AnimationPath,
        IsAnimating = record1.IsAnimating,
        BorderType = record1.BorderType,
        XOffset = (float) record1.XOffset,
        ZOffset = (float) record1.ZOffset,
        Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record1.Rarity))
      }
    };
  }

  private static RewardItemDataModel CreateBattlegroundsBoardSkinRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    BattlegroundsBoardSkinDbfRecord record1 = GameDbf.BattlegroundsBoardSkin.GetRecord(record.BattlegroundsBoardSkinId);
    if (record1 == null)
    {
      Log.All.PrintWarning(string.Format("Battleground Board Skin has unknown id [{0}]", (object) record.BattlegroundsBoardSkinId));
      return (RewardItemDataModel) null;
    }
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = record.BattlegroundsBoardSkinId,
      BGBoardSkin = new BattlegroundsBoardSkinDataModel()
      {
        BoardDbiId = record1.ID,
        DisplayName = (string) record1.CollectionShortName,
        DetailsDisplayName = (string) record1.CollectionName,
        Description = (string) record1.Description,
        ShopDetailsMovie = record1.DetailsMovie,
        ShopDetailsTexture = record1.DetailsTexture,
        BorderType = record1.BorderType,
        Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record1.Rarity))
      }
    };
  }

  private static RewardItemDataModel CreateBattlegroundsFinisherRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    BattlegroundsFinisherDbfRecord record1 = GameDbf.BattlegroundsFinisher.GetRecord(record.BattlegroundsFinisherId);
    if (record1 == null)
    {
      Log.All.PrintWarning(string.Format("Battleground Board Skin has unknown id [{0}]", (object) record.BattlegroundsBoardSkinId));
      return (RewardItemDataModel) null;
    }
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = record.BattlegroundsBoardSkinId,
      BGFinisher = new BattlegroundsFinisherDataModel()
      {
        FinisherDbiId = record1.ID,
        DisplayName = (string) record1.CollectionShortName,
        DetailsDisplayName = (string) record1.CollectionName,
        Description = (string) record1.Description,
        ShopDetailsMovie = record1.DetailsMovie,
        ShopDetailsTexture = record1.DetailsTexture,
        CapsuleType = record1.CapsuleType,
        BodyMaterial = record1.MiniBodyMaterial,
        ArtMaterial = record1.MiniArtMaterial,
        Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record1.Rarity))
      }
    };
  }

  private static RewardItemDataModel CreateSeasonBonusRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      BattlegroundsBonusType = record.BattlegroundsBonusType.ToBattlegroundsBonusType(),
      Quantity = record.Quantity
    };
  }

  private static RewardItemDataModel CreateHeroSkinRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    CardDbfRecord record1 = GameDbf.Card.GetRecord(record.Card);
    if (record1?.CardHero == null)
    {
      Log.All.PrintWarning(string.Format("Hero Skin Item has invalid card id [{0}] where card dbf record has", (object) record.Card) + " no CARD_HERO subtable. NoteMiniGuid = " + record1?.NoteMiniGuid);
      return (RewardItemDataModel) null;
    }
    TAG_PREMIUM premium = (TAG_PREMIUM) record.CardPremiumLevel;
    CardTagDbfRecord cardTagDbfRecord = record1.Tags.Find((Predicate<CardTagDbfRecord>) (tagRecord => tagRecord.TagId == 203));
    string str = cardTagDbfRecord == null ? "" : GameStrings.GetRarityTextKey((TAG_RARITY) cardTagDbfRecord.TagValue);
    if (GameUtils.IsVanillaHero(record.Card))
      premium = TAG_PREMIUM.GOLDEN;
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      ItemType = record.RewardType.ToRewardItemType(),
      Quantity = record.Quantity,
      ItemId = record.Card,
      Card = new CardDataModel()
      {
        CardId = record1.NoteMiniGuid,
        Name = (string) record1.Name,
        FlavorText = (string) record1.FlavorText,
        Premium = premium,
        Owned = CollectionManager.Get().IsCardInCollection(record1.NoteMiniGuid, premium),
        Rarity = str
      }
    };
  }

  private static RewardItemDataModel CreateRandomCardRewardItemDataModel(
    RewardItemDbfRecord record,
    RewardItemOutputData rewardItemOutputData = null)
  {
    RewardItemDataModel rewardItemDataModel = new RewardItemDataModel()
    {
      AssetId = record.ID,
      Quantity = record.Quantity
    };
    if (rewardItemOutputData != null && rewardItemOutputData.HasCardId)
    {
      int cardId = rewardItemOutputData.CardId;
      CardDbfRecord record1 = GameDbf.Card.GetRecord(cardId);
      if (record1 == null)
      {
        Log.All.PrintWarning(string.Format("Random Card Item has unknown output card id [{0}]", (object) cardId));
        return (RewardItemDataModel) null;
      }
      rewardItemDataModel.ItemType = RewardItemType.CARD;
      rewardItemDataModel.ItemId = cardId;
      rewardItemDataModel.Card = new CardDataModel()
      {
        CardId = record1.NoteMiniGuid,
        Premium = (TAG_PREMIUM) record.CardPremiumLevel
      };
    }
    else
    {
      TAG_RARITY randomCardReward = RewardUtils.GetRarityForRandomCardReward(record.RandomCardBoosterCardSet);
      if (randomCardReward == TAG_RARITY.INVALID)
        return (RewardItemDataModel) null;
      rewardItemDataModel.ItemType = record.RewardType.ToRewardItemType();
      rewardItemDataModel.RandomCard = new RandomCardDataModel()
      {
        Premium = (TAG_PREMIUM) record.CardPremiumLevel,
        Rarity = randomCardReward,
        Count = record.Quantity
      };
    }
    return rewardItemDataModel;
  }

  private static RewardItemDataModel CreateSimpleRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      Quantity = record.Quantity,
      ItemType = record.RewardType.ToRewardItemType()
    };
  }

  private static RewardItemDataModel CreateBattlegroundsHeroSkinRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    BattlegroundsHeroSkinId skinId = BattlegroundsHeroSkinId.FromTrustedValue(record.BattlegroundsHeroSkinId);
    int skinHeroCardId;
    if (!CollectionManager.Get().GetBattlegroundsHeroSkinCardIdForSkinId(skinId, out skinHeroCardId))
    {
      Log.All.PrintWarning(string.Format("{0}: Reward record {1} has invalid skin id {2}.", (object) nameof (CreateBattlegroundsHeroSkinRewardItemDataModel), (object) record.ID, (object) record.BattlegroundsGuideSkinId));
      return (RewardItemDataModel) null;
    }
    CardDbfRecord record1 = GameDbf.Card.GetRecord(skinHeroCardId);
    if (record1 == null)
    {
      Log.All.PrintWarning(string.Format("{0}: Reward record {1} has skin id {2} that resolved to invalid skin card id {3}.", (object) nameof (CreateBattlegroundsHeroSkinRewardItemDataModel), (object) record.ID, (object) record.BattlegroundsGuideSkinId, (object) skinHeroCardId));
      return (RewardItemDataModel) null;
    }
    if (record.Quantity != 1)
      Log.All.PrintWarning(string.Format("{0}: Reward record {1} has invalid quantity {2}.", (object) nameof (CreateBattlegroundsHeroSkinRewardItemDataModel), (object) record.ID, (object) record.Quantity));
    BattlegroundsHeroSkinDbfRecord record2 = GameDbf.BattlegroundsHeroSkin.GetRecord(record.BattlegroundsHeroSkinId);
    if (record2 == null)
    {
      Log.All.PrintWarning(string.Format("Battlegrounds Hero Skin has unknown id [{0}]", (object) record.BattlegroundsHeroSkinId));
      return (RewardItemDataModel) null;
    }
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      Quantity = 1,
      ItemType = RewardItemType.BATTLEGROUNDS_HERO_SKIN,
      ItemId = skinHeroCardId,
      Card = new CardDataModel()
      {
        CardId = record1.NoteMiniGuid,
        Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record2.Rarity)),
        Name = (string) record1.Name,
        FlavorText = (string) record1.CardHero?.Description
      }
    };
  }

  private static RewardItemDataModel CreateBattlegroundsGuideSkinRewardItemDataModel(
    RewardItemDbfRecord record)
  {
    BattlegroundsGuideSkinId skinId = BattlegroundsGuideSkinId.FromTrustedValue(record.BattlegroundsGuideSkinId);
    int cardId;
    if (!CollectionManager.Get().GetBattlegroundsGuideSkinCardIdForSkinId(skinId, out cardId))
    {
      Log.All.PrintWarning(string.Format("{0} invalid skin id {1}.", (object) nameof (CreateBattlegroundsGuideSkinRewardItemDataModel), (object) record.BattlegroundsGuideSkinId));
      return (RewardItemDataModel) null;
    }
    CardDbfRecord record1 = GameDbf.Card.GetRecord(cardId);
    if (record1 == null)
    {
      Log.All.PrintWarning(string.Format("{0} invalid skin card id {1}.", (object) nameof (CreateBattlegroundsGuideSkinRewardItemDataModel), (object) cardId));
      return (RewardItemDataModel) null;
    }
    BattlegroundsGuideSkinDbfRecord record2 = GameDbf.BattlegroundsGuideSkin.GetRecord(record.BattlegroundsGuideSkinId);
    if (record2 == null)
    {
      Log.All.PrintWarning(string.Format("Battleground Bartender Skin has unknown id [{0}]", (object) record.BattlegroundsGuideSkinId));
      return (RewardItemDataModel) null;
    }
    return new RewardItemDataModel()
    {
      AssetId = record.ID,
      Quantity = 1,
      ItemType = RewardItemType.BATTLEGROUNDS_GUIDE_SKIN,
      ItemId = cardId,
      Card = new CardDataModel()
      {
        CardId = record1.NoteMiniGuid,
        Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record2.Rarity)),
        Name = (string) record1.Name,
        FlavorText = (string) record1.CardHero?.Description
      }
    };
  }

  public static RewardItemDataModel CreateShopProductRewardItemDataModel(
    ShopProductData.ProductItemData productItemData)
  {
    return new RewardItemDataModel()
    {
      PmtLicenseId = productItemData.licenseId,
      ItemType = productItemData.itemType,
      ItemId = productItemData.itemId,
      Quantity = productItemData.quantity
    };
  }

  public static RewardItemDataModel CreateShopRewardItemDataModel(
    Network.Bundle netBundle,
    Network.BundleItem netBundleItem,
    out bool isValidItem)
  {
    RewardItemDataModel rewardItemDataModel = (RewardItemDataModel) null;
    switch (netBundleItem.ItemType)
    {
      case ProductType.PRODUCT_TYPE_BOOSTER:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.BOOSTER,
          ItemId = netBundleItem.ProductData,
          Quantity = netBundleItem.Quantity
        };
        break;
      case ProductType.PRODUCT_TYPE_DRAFT:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.ARENA_TICKET,
          Quantity = netBundleItem.Quantity
        };
        break;
      case ProductType.PRODUCT_TYPE_NAXX:
      case ProductType.PRODUCT_TYPE_BRM:
      case ProductType.PRODUCT_TYPE_LOE:
      case ProductType.PRODUCT_TYPE_WING:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.ADVENTURE_WING,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_CARD_BACK:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.CARD_BACK,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_HERO:
        RewardItemType rewardItemType = RewardItemType.HERO_SKIN;
        int productData1 = netBundleItem.ProductData;
        if (CollectionManager.Get().IsBattlegroundsHeroSkinCard(productData1))
          rewardItemType = RewardItemType.BATTLEGROUNDS_HERO_SKIN;
        else if (CollectionManager.Get().IsBattlegroundsGuideSkinCard(productData1))
          rewardItemType = RewardItemType.BATTLEGROUNDS_GUIDE_SKIN;
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = rewardItemType,
          ItemId = productData1,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_RANDOM_CARD:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.RANDOM_CARD,
          ItemId = netBundleItem.ProductData,
          Quantity = netBundleItem.Quantity
        };
        break;
      case ProductType.PRODUCT_TYPE_HIDDEN_LICENSE:
      case ProductType.PRODUCT_TYPE_FIXED_LICENSE:
        isValidItem = true;
        return (RewardItemDataModel) null;
      case ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.TAVERN_BRAWL_TICKET,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_CURRENCY:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.UNDEFINED,
          Quantity = netBundleItem.Quantity
        };
        PegasusShared.CurrencyType productData2 = (PegasusShared.CurrencyType) netBundleItem.ProductData;
        switch (productData2)
        {
          case PegasusShared.CurrencyType.CURRENCY_TYPE_DUST:
            rewardItemDataModel.ItemType = RewardItemType.DUST;
            break;
          case PegasusShared.CurrencyType.CURRENCY_TYPE_CN_RUNESTONES:
            rewardItemDataModel.ItemType = RewardItemType.CN_RUNESTONES;
            break;
          case PegasusShared.CurrencyType.CURRENCY_TYPE_CN_ARCANE_ORBS:
            rewardItemDataModel.ItemType = RewardItemType.CN_ARCANE_ORBS;
            break;
          case PegasusShared.CurrencyType.CURRENCY_TYPE_ROW_RUNESTONES:
            rewardItemDataModel.ItemType = RewardItemType.ROW_RUNESTONES;
            break;
          default:
            ProductIssues.LogError(netBundle, string.Format("Has reward with unsupported currency type {0}", (object) productData2));
            isValidItem = false;
            return (RewardItemDataModel) null;
        }
        break;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BONUS:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.BATTLEGROUNDS_BONUS,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_PROGRESSION_BONUS:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.PROGRESSION_BONUS,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        string str;
        if (netBundleItem.Attributes.GetValue("season").TryGetValue(out str))
        {
          rewardItemDataModel.Season = str;
          break;
        }
        break;
      case ProductType.PRODUCT_TYPE_MINI_SET:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.MINI_SET,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_SELLABLE_DECK:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.SELLABLE_DECK,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_MERCENARIES_MERCENARY:
        int result1 = 0;
        TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
        Maybe<string> maybe = netBundleItem.Attributes.GetValue("merc_art_variation_id");
        string s1;
        if (maybe.TryGetValue(out s1) && !int.TryParse(s1, out result1))
        {
          ProductIssues.LogError(netBundle, "Has license with invalid mercenaries art variation ID " + s1);
          isValidItem = false;
          return (RewardItemDataModel) null;
        }
        maybe = netBundleItem.Attributes.GetValue("merc_art_variation_premium");
        string s2;
        if (maybe.TryGetValue(out s2))
        {
          int result2;
          if (int.TryParse(s2, out result2) && result2 >= 0 && result2 <= 2)
          {
            premium = (TAG_PREMIUM) result2;
          }
          else
          {
            ProductIssues.LogError(netBundle, "Has license with invalid mercenaries art variation premium value " + s2);
            isValidItem = false;
            return (RewardItemDataModel) null;
          }
        }
        rewardItemDataModel = RewardUtils.CreateMercenaryRewardItemDataModel(netBundleItem.ProductData, result1, premium);
        break;
      case ProductType.PRODUCT_TYPE_MERCENARIES_CURRENCY:
        rewardItemDataModel = RewardUtils.CreateMercenaryCoinsRewardData(netBundleItem.ProductData, netBundleItem.Quantity, false, false).DataModel;
        break;
      case ProductType.PRODUCT_TYPE_MERCENARIES_BOOSTER:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.MERCENARY_BOOSTER,
          ItemId = netBundleItem.ProductData,
          Quantity = netBundleItem.Quantity
        };
        break;
      case ProductType.PRODUCT_TYPE_MERCENARIES_RANDOM_REWARD:
        if (GameDbf.MercenariesRandomReward.HasRecord(netBundleItem.ProductData))
        {
          MercenariesRandomRewardDbfRecord record = GameDbf.MercenariesRandomReward.GetRecord(netBundleItem.ProductData);
          if (record.RewardType == MercenariesRandomReward.RewardType.REWARD_TYPE_MERCENARY)
          {
            rewardItemDataModel = new RewardItemDataModel()
            {
              ItemType = RewardItemType.MERCENARY_RANDOM_MERCENARY,
              ItemId = netBundleItem.ProductData,
              Quantity = netBundleItem.Quantity,
              RandomMercenary = new LettuceRandomMercenaryDataModel()
              {
                Premium = (TAG_PREMIUM) record.Premium,
                Rarity = (TAG_RARITY) record.Rarity,
                RestrictRarity = record.RestrictRarity
              }
            };
            break;
          }
          if (record.RewardType == MercenariesRandomReward.RewardType.REWARD_TYPE_CURRENCY)
          {
            rewardItemDataModel = new RewardItemDataModel()
            {
              ItemType = RewardItemType.MERCENARY_COIN,
              Quantity = 1,
              MercenaryCoin = new LettuceMercenaryCoinDataModel()
              {
                Quantity = netBundleItem.Quantity,
                GlowActive = true,
                IsRandom = true
              }
            };
            break;
          }
          break;
        }
        ProductIssues.LogError(netBundle, string.Format("Has license with unrecognized mercenaries random reward ID {0}", (object) netBundleItem.ProductData));
        isValidItem = false;
        return (RewardItemDataModel) null;
      case ProductType.PRODUCT_TYPE_MERCENARIES_KNOCKOUT_SPECIFIC:
        rewardItemDataModel = RewardUtils.CreateKnockoutSpecificMercenaryRewardItemDataModel(netBundleItem.ProductData);
        break;
      case ProductType.PRODUCT_TYPE_MERCENARIES_KNOCKOUT_RANDOM:
        if (GameDbf.MercenariesRandomReward.HasRecord(netBundleItem.ProductData))
        {
          MercenariesRandomRewardDbfRecord record = GameDbf.MercenariesRandomReward.GetRecord(netBundleItem.ProductData);
          rewardItemDataModel = new RewardItemDataModel()
          {
            ItemType = RewardItemType.MERCENARY_KNOCKOUT_RANDOM,
            ItemId = netBundleItem.ProductData,
            Quantity = netBundleItem.Quantity,
            RandomMercenary = new LettuceRandomMercenaryDataModel()
            {
              Premium = (TAG_PREMIUM) record.Premium,
              Rarity = (TAG_RARITY) record.Rarity,
              RestrictRarity = record.RestrictRarity
            }
          };
          break;
        }
        ProductIssues.LogError(netBundle, string.Format("Has license with unrecognized mercenaries random reward ID {0}", (object) netBundleItem.ProductData));
        isValidItem = false;
        return (RewardItemDataModel) null;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_BOARD_SKIN:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.BATTLEGROUNDS_BOARD_SKIN,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_FINISHER:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.BATTLEGROUNDS_FINISHER,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_DIAMOND_CARD:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.CARD,
          ItemId = netBundleItem.ProductData,
          Quantity = netBundleItem.Quantity
        };
        break;
      case ProductType.PRODUCT_TYPE_BATTLEGROUNDS_EMOTE:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.BATTLEGROUNDS_EMOTE,
          ItemId = netBundleItem.ProductData,
          Quantity = 1
        };
        break;
      case ProductType.PRODUCT_TYPE_LUCKY_DRAW:
        rewardItemDataModel = new RewardItemDataModel()
        {
          ItemType = RewardItemType.LUCKY_DRAW,
          ItemId = netBundleItem.ProductData,
          Quantity = netBundleItem.Quantity
        };
        break;
      default:
        ProductIssues.LogError(netBundle, string.Format("Has license with unrecognized reward type [{0}", (object) netBundleItem.ItemType));
        isValidItem = false;
        return (RewardItemDataModel) null;
    }
    isValidItem = RewardUtils.InitializeRewardItemDataModelForShop(rewardItemDataModel, netBundleItem, netBundle);
    return rewardItemDataModel;
  }

  public static IEnumerable<RewardItemDataModel> ConsolidateGroup(
    IGrouping<RewardItemType, RewardItemDataModel> group)
  {
    switch (group.Key)
    {
      case RewardItemType.BOOSTER:
        return RewardFactory.ConsolidateBoosterRewardItems((IEnumerable<RewardItemDataModel>) group);
      case RewardItemType.DUST:
      case RewardItemType.CN_ARCANE_ORBS:
        return RewardFactory.ConsolidateCurrencyRewardItems((IEnumerable<RewardItemDataModel>) group);
      case RewardItemType.ARENA_TICKET:
      case RewardItemType.GOLD:
        return RewardFactory.ConsolidateSimpleRewardItems((IEnumerable<RewardItemDataModel>) group);
      case RewardItemType.RANDOM_CARD:
        return RewardFactory.ConsolidateRandomCardRewardItems((IEnumerable<RewardItemDataModel>) group);
      case RewardItemType.CARD:
        return RewardFactory.ConsolidateCardRewardItems((IEnumerable<RewardItemDataModel>) group);
      case RewardItemType.REWARD_TRACK_XP_BOOST:
        return RewardFactory.ConsolidateRewardTrackXpBoostItems((IEnumerable<RewardItemDataModel>) group);
      default:
        return (IEnumerable<RewardItemDataModel>) group;
    }
  }

  private static IEnumerable<RewardItemDataModel> ConsolidateBoosterRewardItems(
    IEnumerable<RewardItemDataModel> rewards)
  {
    return rewards.GroupBy<RewardItemDataModel, int>((Func<RewardItemDataModel, int>) (element => element.ItemId)).Select<IGrouping<int, RewardItemDataModel>, RewardItemDataModel>((Func<IGrouping<int, RewardItemDataModel>, RewardItemDataModel>) (group => group.Aggregate<RewardItemDataModel, RewardItemDataModel>(new RewardItemDataModel()
    {
      ItemType = RewardItemType.BOOSTER,
      Quantity = 0,
      ItemId = group.Key,
      Booster = new PackDataModel()
      {
        Type = (BoosterDbId) group.Key,
        Quantity = 0
      }
    }, (Func<RewardItemDataModel, RewardItemDataModel, RewardItemDataModel>) ((acc, element) =>
    {
      acc.AssetId = element.AssetId;
      acc.Quantity += element.Quantity;
      acc.Booster.Quantity += element.Booster.Quantity;
      return acc;
    }))));
  }

  private static IEnumerable<RewardItemDataModel> ConsolidateCardRewardItems(
    IEnumerable<RewardItemDataModel> rewards)
  {
    return rewards.GroupBy<RewardItemDataModel, (RewardItemType, int, TAG_PREMIUM)>((Func<RewardItemDataModel, (RewardItemType, int, TAG_PREMIUM)>) (element => (element.ItemType, element.ItemId, element.Card.Premium))).Select<IGrouping<(RewardItemType, int, TAG_PREMIUM), RewardItemDataModel>, RewardItemDataModel>((Func<IGrouping<(RewardItemType, int, TAG_PREMIUM), RewardItemDataModel>, RewardItemDataModel>) (group => group.Aggregate<RewardItemDataModel, RewardItemDataModel>(new RewardItemDataModel()
    {
      ItemType = group.Key.ToTuple<RewardItemType, int, TAG_PREMIUM>().Item1,
      Quantity = 0,
      ItemId = group.Key.ToTuple<RewardItemType, int, TAG_PREMIUM>().Item2,
      Card = new CardDataModel()
      {
        Premium = group.Key.ToTuple<RewardItemType, int, TAG_PREMIUM>().Item3
      }
    }, (Func<RewardItemDataModel, RewardItemDataModel, RewardItemDataModel>) ((acc, element) =>
    {
      acc.AssetId = element.AssetId;
      acc.Quantity += element.Quantity;
      acc.Card.CardId = element.Card.CardId;
      return acc;
    }))));
  }

  private static IEnumerable<RewardItemDataModel> ConsolidateCurrencyRewardItems(
    IEnumerable<RewardItemDataModel> rewards)
  {
    return rewards.GroupBy<RewardItemDataModel, RewardItemType>((Func<RewardItemDataModel, RewardItemType>) (element => element.ItemType)).Select<IGrouping<RewardItemType, RewardItemDataModel>, RewardItemDataModel>((Func<IGrouping<RewardItemType, RewardItemDataModel>, RewardItemDataModel>) (group => group.Aggregate<RewardItemDataModel, RewardItemDataModel>(new RewardItemDataModel()
    {
      ItemType = group.Key,
      Quantity = 0,
      Currency = new PriceDataModel()
      {
        Currency = RewardUtils.RewardItemTypeToCurrencyType(group.Key),
        Amount = 0.0f
      }
    }, (Func<RewardItemDataModel, RewardItemDataModel, RewardItemDataModel>) ((acc, element) =>
    {
      acc.AssetId = element.AssetId;
      acc.Quantity += element.Quantity;
      acc.Currency.Amount += element.Currency.Amount;
      acc.Currency.DisplayText = acc.Quantity.ToString();
      return acc;
    }))));
  }

  private static IEnumerable<RewardItemDataModel> ConsolidateRandomCardRewardItems(
    IEnumerable<RewardItemDataModel> rewards)
  {
    return rewards.GroupBy<RewardItemDataModel, (int, TAG_PREMIUM, TAG_RARITY)>((Func<RewardItemDataModel, (int, TAG_PREMIUM, TAG_RARITY)>) (element => (element.ItemId, element.RandomCard.Premium, element.RandomCard.Rarity))).Select<IGrouping<(int, TAG_PREMIUM, TAG_RARITY), RewardItemDataModel>, RewardItemDataModel>((Func<IGrouping<(int, TAG_PREMIUM, TAG_RARITY), RewardItemDataModel>, RewardItemDataModel>) (group => group.Aggregate<RewardItemDataModel, RewardItemDataModel>(new RewardItemDataModel()
    {
      ItemType = RewardItemType.RANDOM_CARD,
      Quantity = 0,
      ItemId = group.Key.ToTuple<int, TAG_PREMIUM, TAG_RARITY>().Item1,
      RandomCard = new RandomCardDataModel()
      {
        Premium = group.Key.ToTuple<int, TAG_PREMIUM, TAG_RARITY>().Item2,
        Rarity = group.Key.ToTuple<int, TAG_PREMIUM, TAG_RARITY>().Item3,
        Count = 0
      }
    }, (Func<RewardItemDataModel, RewardItemDataModel, RewardItemDataModel>) ((acc, element) =>
    {
      acc.AssetId = element.AssetId;
      acc.Quantity += element.Quantity;
      acc.Card.CardId = element.Card.CardId;
      return acc;
    }))));
  }

  private static IEnumerable<RewardItemDataModel> ConsolidateRewardTrackXpBoostItems(
    IEnumerable<RewardItemDataModel> rewards)
  {
    return rewards.GroupBy<RewardItemDataModel, RewardItemType>((Func<RewardItemDataModel, RewardItemType>) (element => element.ItemType)).Select<IGrouping<RewardItemType, RewardItemDataModel>, RewardItemDataModel>((Func<IGrouping<RewardItemType, RewardItemDataModel>, RewardItemDataModel>) (group => group.Aggregate<RewardItemDataModel, RewardItemDataModel>(new RewardItemDataModel()
    {
      ItemType = group.Key,
      Quantity = 0
    }, (Func<RewardItemDataModel, RewardItemDataModel, RewardItemDataModel>) ((acc, element) =>
    {
      acc.AssetId = element.AssetId;
      acc.Quantity = Math.Max(acc.Quantity, element.Quantity);
      return acc;
    }))));
  }

  private static IEnumerable<RewardItemDataModel> ConsolidateSimpleRewardItems(
    IEnumerable<RewardItemDataModel> rewards)
  {
    return rewards.GroupBy<RewardItemDataModel, RewardItemType>((Func<RewardItemDataModel, RewardItemType>) (element => element.ItemType)).Select<IGrouping<RewardItemType, RewardItemDataModel>, RewardItemDataModel>((Func<IGrouping<RewardItemType, RewardItemDataModel>, RewardItemDataModel>) (group => group.Aggregate<RewardItemDataModel, RewardItemDataModel>(new RewardItemDataModel()
    {
      ItemType = group.Key,
      Quantity = 0
    }, (Func<RewardItemDataModel, RewardItemDataModel, RewardItemDataModel>) ((acc, element) =>
    {
      acc.AssetId = element.AssetId;
      acc.Quantity += element.Quantity;
      return acc;
    }))));
  }

  public static RewardListDataModel CreateRewardItemDataModel(PegasusShared.RewardChest chest)
  {
    RewardListDataModel rewardItemDataModel1 = new RewardListDataModel();
    RewardItemDataModel rewardItemDataModel2 = (RewardItemDataModel) null;
    foreach (PegasusShared.RewardBag rewardBag in chest.Bag)
    {
      if (rewardBag.HasRewardMercenariesCurrency && rewardBag.RewardMercenariesCurrency.HasCurrencyDelta)
      {
        string idFromMercenaryId = GameUtils.GetCardIdFromMercenaryId(rewardBag.RewardMercenariesCurrency.MercenaryId);
        EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromMercenaryId);
        if (entityDef == null)
          Log.Lettuce.PrintError("OnMercenaryCoinRewardFullDefLoaded - Failed to load def for card {0}", (object) idFromMercenaryId);
        rewardItemDataModel2 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.MERCENARY_COIN,
          MercenaryCoin = new LettuceMercenaryCoinDataModel()
          {
            MercenaryId = rewardBag.RewardMercenariesCurrency.MercenaryId,
            MercenaryName = entityDef?.GetName(),
            Quantity = (int) rewardBag.RewardMercenariesCurrency.CurrencyDelta,
            GlowActive = true
          }
        };
      }
      else if (rewardBag.HasRewardMercenariesExperience && rewardBag.RewardMercenariesExperience.HasPreExp && rewardBag.RewardMercenariesExperience.HasPostExp)
      {
        LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) rewardBag.RewardMercenariesExperience.MercenaryId);
        rewardItemDataModel2 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.MERCENARY,
          Mercenary = MercenaryFactory.CreateMercenaryDataModel(mercenary)
        };
        rewardItemDataModel2.Mercenary.ExperienceInitial = (int) rewardBag.RewardMercenariesExperience.PreExp;
        rewardItemDataModel2.Mercenary.ExperienceFinal = (int) rewardBag.RewardMercenariesExperience.PostExp;
        rewardItemDataModel2.Mercenary.Owned = true;
        GameUtils.GetMercenaryLevelFromExperience(rewardItemDataModel2.Mercenary.ExperienceInitial);
        CollectionUtils.PopulateMercenaryCardDataModel(rewardItemDataModel2.Mercenary, mercenary.GetEquippedArtVariation());
        CollectionUtils.SetMercenaryStatsByLevel(rewardItemDataModel2.Mercenary, mercenary.ID, mercenary.m_level, mercenary.m_isFullyUpgraded);
      }
      else if (rewardBag.HasRewardMercenariesEquipment)
        Log.Lettuce.PrintError("CreateRewardItemDataModel - Mercenaries Equipment unsupported");
      if (rewardItemDataModel2 != null)
      {
        rewardItemDataModel1.Items.Add(rewardItemDataModel2);
        rewardItemDataModel2 = (RewardItemDataModel) null;
      }
    }
    return rewardItemDataModel1;
  }
}
