using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardItemDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_rewardListId;
  [SerializeField]
  private RewardItem.RewardType m_rewardType;
  [SerializeField]
  private int m_quantity = 1;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private RewardItem.CardPremiumLevel m_cardPremiumLevel;
  [SerializeField]
  private int m_randomCardBoosterCardSetId;
  [SerializeField]
  private int m_boosterId;
  [SerializeField]
  private RewardItem.BoosterSelector m_boosterSelector;
  [SerializeField]
  private int m_cardBackId;
  [SerializeField]
  private int m_customCoinId;
  [SerializeField]
  private int m_subsetId;
  [SerializeField]
  private bool m_isVirtual;
  [SerializeField]
  private int m_battlegroundsHeroSkinId;
  [SerializeField]
  private int m_battlegroundsGuideSkinId;
  [SerializeField]
  private int m_battlegroundsBoardSkinId;
  [SerializeField]
  private int m_battlegroundsFinisherId;
  [SerializeField]
  private int m_battlegroundsEmoteId;
  [SerializeField]
  private RewardItem.BattlegroundsBonusType m_battlegroundsBonusType;
  [SerializeField]
  private int m_mercenaryId;
  [SerializeField]
  private RewardItem.MercenarySelector m_mercenarySelector;
  [SerializeField]
  private int m_mercenaryArtVariationId;
  [SerializeField]
  private RewardItem.MercenariesPremium m_mercenaryArtPremium;
  [SerializeField]
  private int m_mercenaryEquipmentId;
  [SerializeField]
  private int m_mercenaryRarityId;

  [DbfField("REWARD_LIST_ID")]
  public int RewardListId => this.m_rewardListId;

  [DbfField("REWARD_TYPE")]
  public RewardItem.RewardType RewardType => this.m_rewardType;

  [DbfField("QUANTITY")]
  public int Quantity => this.m_quantity;

  [DbfField("CARD")]
  public int Card => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("CARD_PREMIUM_LEVEL")]
  public RewardItem.CardPremiumLevel CardPremiumLevel => this.m_cardPremiumLevel;

  [DbfField("RANDOM_CARD_BOOSTER_CARD_SET")]
  public int RandomCardBoosterCardSet => this.m_randomCardBoosterCardSetId;

  [DbfField("BOOSTER")]
  public int Booster => this.m_boosterId;

  [DbfField("BOOSTER_SELECTOR")]
  public RewardItem.BoosterSelector BoosterSelector => this.m_boosterSelector;

  [DbfField("CARD_BACK")]
  public int CardBack => this.m_cardBackId;

  [DbfField("CUSTOM_COIN")]
  public int CustomCoin => this.m_customCoinId;

  [DbfField("SUBSET_ID")]
  public int SubsetId => this.m_subsetId;

  [DbfField("BATTLEGROUNDS_HERO_SKIN_ID")]
  public int BattlegroundsHeroSkinId => this.m_battlegroundsHeroSkinId;

  public BattlegroundsHeroSkinDbfRecord BattlegroundsHeroSkinRecord => GameDbf.BattlegroundsHeroSkin.GetRecord(this.m_battlegroundsHeroSkinId);

  [DbfField("BATTLEGROUNDS_GUIDE_SKIN_ID")]
  public int BattlegroundsGuideSkinId => this.m_battlegroundsGuideSkinId;

  public BattlegroundsGuideSkinDbfRecord BattlegroundsGuideSkinRecord => GameDbf.BattlegroundsGuideSkin.GetRecord(this.m_battlegroundsGuideSkinId);

  [DbfField("BATTLEGROUNDS_BOARD_SKIN_ID")]
  public int BattlegroundsBoardSkinId => this.m_battlegroundsBoardSkinId;

  [DbfField("BATTLEGROUNDS_FINISHER_ID")]
  public int BattlegroundsFinisherId => this.m_battlegroundsFinisherId;

  [DbfField("BATTLEGROUNDS_EMOTE_ID")]
  public int BattlegroundsEmoteId => this.m_battlegroundsEmoteId;

  [DbfField("BATTLEGROUNDS_BONUS_TYPE")]
  public RewardItem.BattlegroundsBonusType BattlegroundsBonusType => this.m_battlegroundsBonusType;

  [DbfField("MERCENARY")]
  public int Mercenary => this.m_mercenaryId;

  public LettuceMercenaryDbfRecord MercenaryRecord => GameDbf.LettuceMercenary.GetRecord(this.m_mercenaryId);

  [DbfField("MERCENARY_SELECTOR")]
  public RewardItem.MercenarySelector MercenarySelector => this.m_mercenarySelector;

  [DbfField("MERCENARY_ART_VARIATION")]
  public int MercenaryArtVariation => this.m_mercenaryArtVariationId;

  public MercenaryArtVariationDbfRecord MercenaryArtVariationRecord => GameDbf.MercenaryArtVariation.GetRecord(this.m_mercenaryArtVariationId);

  [DbfField("MERCENARY_ART_PREMIUM")]
  public RewardItem.MercenariesPremium MercenaryArtPremium => this.m_mercenaryArtPremium;

  [DbfField("MERCENARY_EQUIPMENT")]
  public int MercenaryEquipment => this.m_mercenaryEquipmentId;

  [DbfField("MERCENARY_RARITY")]
  public int MercenaryRarity => this.m_mercenaryRarityId;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BATTLEGROUNDS_BOARD_SKIN_ID":
        return (object) this.m_battlegroundsBoardSkinId;
      case "BATTLEGROUNDS_BONUS_TYPE":
        return (object) this.m_battlegroundsBonusType;
      case "BATTLEGROUNDS_EMOTE_ID":
        return (object) this.m_battlegroundsEmoteId;
      case "BATTLEGROUNDS_FINISHER_ID":
        return (object) this.m_battlegroundsFinisherId;
      case "BATTLEGROUNDS_GUIDE_SKIN_ID":
        return (object) this.m_battlegroundsGuideSkinId;
      case "BATTLEGROUNDS_HERO_SKIN_ID":
        return (object) this.m_battlegroundsHeroSkinId;
      case "BOOSTER":
        return (object) this.m_boosterId;
      case "BOOSTER_SELECTOR":
        return (object) this.m_boosterSelector;
      case "CARD":
        return (object) this.m_cardId;
      case "CARD_BACK":
        return (object) this.m_cardBackId;
      case "CARD_PREMIUM_LEVEL":
        return (object) this.m_cardPremiumLevel;
      case "CUSTOM_COIN":
        return (object) this.m_customCoinId;
      case "ID":
        return (object) this.ID;
      case "IS_VIRTUAL":
        return (object) this.m_isVirtual;
      case "MERCENARY":
        return (object) this.m_mercenaryId;
      case "MERCENARY_ART_PREMIUM":
        return (object) this.m_mercenaryArtPremium;
      case "MERCENARY_ART_VARIATION":
        return (object) this.m_mercenaryArtVariationId;
      case "MERCENARY_EQUIPMENT":
        return (object) this.m_mercenaryEquipmentId;
      case "MERCENARY_RARITY":
        return (object) this.m_mercenaryRarityId;
      case "MERCENARY_SELECTOR":
        return (object) this.m_mercenarySelector;
      case "QUANTITY":
        return (object) this.m_quantity;
      case "RANDOM_CARD_BOOSTER_CARD_SET":
        return (object) this.m_randomCardBoosterCardSetId;
      case "REWARD_LIST_ID":
        return (object) this.m_rewardListId;
      case "REWARD_TYPE":
        return (object) this.m_rewardType;
      case "SUBSET_ID":
        return (object) this.m_subsetId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 679831291:
        if (!(name == "BOOSTER"))
          break;
        this.m_boosterId = (int) val;
        break;
      case 699650505:
        if (!(name == "SUBSET_ID"))
          break;
        this.m_subsetId = (int) val;
        break;
      case 1019664678:
        if (!(name == "BATTLEGROUNDS_FINISHER_ID"))
          break;
        this.m_battlegroundsFinisherId = (int) val;
        break;
      case 1098446823:
        if (!(name == "REWARD_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardType = RewardItem.RewardType.NONE;
            return;
          case RewardItem.RewardType _:
          case int _:
            this.m_rewardType = (RewardItem.RewardType) val;
            return;
          case string _:
            this.m_rewardType = RewardItem.ParseRewardTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1219178182:
        if (!(name == "BATTLEGROUNDS_BONUS_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_battlegroundsBonusType = RewardItem.BattlegroundsBonusType.DISCOVER_HERO;
            return;
          case RewardItem.BattlegroundsBonusType _:
          case int _:
            this.m_battlegroundsBonusType = (RewardItem.BattlegroundsBonusType) val;
            return;
          case string _:
            this.m_battlegroundsBonusType = RewardItem.ParseBattlegroundsBonusTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1615316160:
        if (!(name == "CARD_PREMIUM_LEVEL"))
          break;
        switch (val)
        {
          case null:
            this.m_cardPremiumLevel = RewardItem.CardPremiumLevel.NORMAL;
            return;
          case RewardItem.CardPremiumLevel _:
          case int _:
            this.m_cardPremiumLevel = (RewardItem.CardPremiumLevel) val;
            return;
          case string _:
            this.m_cardPremiumLevel = RewardItem.ParseCardPremiumLevelValue((string) val);
            return;
          default:
            return;
        }
      case 1619614552:
        if (!(name == "QUANTITY"))
          break;
        this.m_quantity = (int) val;
        break;
      case 1794679851:
        if (!(name == "MERCENARY_ART_PREMIUM"))
          break;
        switch (val)
        {
          case null:
            this.m_mercenaryArtPremium = RewardItem.MercenariesPremium.PREMIUM_NORMAL;
            return;
          case RewardItem.MercenariesPremium _:
          case int _:
            this.m_mercenaryArtPremium = (RewardItem.MercenariesPremium) val;
            return;
          case string _:
            this.m_mercenaryArtPremium = RewardItem.ParseMercenariesPremiumValue((string) val);
            return;
          default:
            return;
        }
      case 1795557686:
        if (!(name == "BATTLEGROUNDS_HERO_SKIN_ID"))
          break;
        this.m_battlegroundsHeroSkinId = (int) val;
        break;
      case 1994457602:
        if (!(name == "BATTLEGROUNDS_EMOTE_ID"))
          break;
        this.m_battlegroundsEmoteId = (int) val;
        break;
      case 2239413407:
        if (!(name == "CARD"))
          break;
        this.m_cardId = (int) val;
        break;
      case 2392581014:
        if (!(name == "CUSTOM_COIN"))
          break;
        this.m_customCoinId = (int) val;
        break;
      case 2645619338:
        if (!(name == "RANDOM_CARD_BOOSTER_CARD_SET"))
          break;
        this.m_randomCardBoosterCardSetId = (int) val;
        break;
      case 2719135948:
        if (!(name == "MERCENARY_EQUIPMENT"))
          break;
        this.m_mercenaryEquipmentId = (int) val;
        break;
      case 2772852763:
        if (!(name == "MERCENARY"))
          break;
        this.m_mercenaryId = (int) val;
        break;
      case 3060627597:
        if (!(name == "IS_VIRTUAL"))
          break;
        this.m_isVirtual = (bool) val;
        break;
      case 3189893097:
        if (!(name == "BOOSTER_SELECTOR"))
          break;
        switch (val)
        {
          case null:
            this.m_boosterSelector = RewardItem.BoosterSelector.NONE;
            return;
          case RewardItem.BoosterSelector _:
          case int _:
            this.m_boosterSelector = (RewardItem.BoosterSelector) val;
            return;
          case string _:
            this.m_boosterSelector = RewardItem.ParseBoosterSelectorValue((string) val);
            return;
          default:
            return;
        }
      case 3253563099:
        if (!(name == "MERCENARY_ART_VARIATION"))
          break;
        this.m_mercenaryArtVariationId = (int) val;
        break;
      case 3549743771:
        if (!(name == "CARD_BACK"))
          break;
        this.m_cardBackId = (int) val;
        break;
      case 3617557759:
        if (!(name == "MERCENARY_RARITY"))
          break;
        this.m_mercenaryRarityId = (int) val;
        break;
      case 3641954198:
        if (!(name == "BATTLEGROUNDS_GUIDE_SKIN_ID"))
          break;
        this.m_battlegroundsGuideSkinId = (int) val;
        break;
      case 3804092358:
        if (!(name == "BATTLEGROUNDS_BOARD_SKIN_ID"))
          break;
        this.m_battlegroundsBoardSkinId = (int) val;
        break;
      case 4014551637:
        if (!(name == "REWARD_LIST_ID"))
          break;
        this.m_rewardListId = (int) val;
        break;
      case 4179078217:
        if (!(name == "MERCENARY_SELECTOR"))
          break;
        switch (val)
        {
          case null:
            this.m_mercenarySelector = RewardItem.MercenarySelector.SPECIFIC;
            return;
          case RewardItem.MercenarySelector _:
          case int _:
            this.m_mercenarySelector = (RewardItem.MercenarySelector) val;
            return;
          case string _:
            this.m_mercenarySelector = RewardItem.ParseMercenarySelectorValue((string) val);
            return;
          default:
            return;
        }
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "BATTLEGROUNDS_BOARD_SKIN_ID":
        return typeof (int);
      case "BATTLEGROUNDS_BONUS_TYPE":
        return typeof (RewardItem.BattlegroundsBonusType);
      case "BATTLEGROUNDS_EMOTE_ID":
        return typeof (int);
      case "BATTLEGROUNDS_FINISHER_ID":
        return typeof (int);
      case "BATTLEGROUNDS_GUIDE_SKIN_ID":
        return typeof (int);
      case "BATTLEGROUNDS_HERO_SKIN_ID":
        return typeof (int);
      case "BOOSTER":
        return typeof (int);
      case "BOOSTER_SELECTOR":
        return typeof (RewardItem.BoosterSelector);
      case "CARD":
        return typeof (int);
      case "CARD_BACK":
        return typeof (int);
      case "CARD_PREMIUM_LEVEL":
        return typeof (RewardItem.CardPremiumLevel);
      case "CUSTOM_COIN":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "IS_VIRTUAL":
        return typeof (bool);
      case "MERCENARY":
        return typeof (int);
      case "MERCENARY_ART_PREMIUM":
        return typeof (RewardItem.MercenariesPremium);
      case "MERCENARY_ART_VARIATION":
        return typeof (int);
      case "MERCENARY_EQUIPMENT":
        return typeof (int);
      case "MERCENARY_RARITY":
        return typeof (int);
      case "MERCENARY_SELECTOR":
        return typeof (RewardItem.MercenarySelector);
      case "QUANTITY":
        return typeof (int);
      case "RANDOM_CARD_BOOSTER_CARD_SET":
        return typeof (int);
      case "REWARD_LIST_ID":
        return typeof (int);
      case "REWARD_TYPE":
        return typeof (RewardItem.RewardType);
      case "SUBSET_ID":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRewardItemDbfRecords loadRecords = new LoadRewardItemDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RewardItemDbfAsset rewardItemDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RewardItemDbfAsset)) as RewardItemDbfAsset;
    if ((UnityEngine.Object) rewardItemDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RewardItemDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardItemDbfAsset.Records.Count; ++index)
      rewardItemDbfAsset.Records[index].StripUnusedLocales();
    records = rewardItemDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
