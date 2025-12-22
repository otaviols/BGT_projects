using Hearthstone.DataModels;

public static class MercenaryFactory
{
  public static LettuceMercenaryDataModel CreateEmptyMercenaryDataModel() => new LettuceMercenaryDataModel()
  {
    HideXp = false,
    HideWatermark = true,
    HideStats = false
  };

  public static LettuceMercenaryDataModel CreateMercenaryDataModel(
    int mercenaryId,
    int artVariationId,
    TAG_PREMIUM premium,
    LettuceMercenary mercenary = null)
  {
    LettuceMercenaryDbfRecord record = GameDbf.LettuceMercenary.GetRecord(mercenaryId);
    CardDbfRecord cardDbfRecord = artVariationId == 0 ? LettuceMercenary.GetDefaultArtVariationRecord(mercenaryId).CardRecord : GameDbf.MercenaryArtVariation.GetRecord(artVariationId).CardRecord;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardDbfRecord.ID);
    LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateEmptyMercenaryDataModel();
    mercenaryDataModel.MercenaryId = record.ID;
    mercenaryDataModel.MercenaryName = entityDef.GetLocalizedName();
    mercenaryDataModel.MercenaryShortName = entityDef.GetLocalizedShortName();
    mercenaryDataModel.MercenaryRole = entityDef.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    mercenaryDataModel.MercenaryRarity = (TAG_RARITY) record.Rarity;
    int level = 1;
    bool isFullyUpgraded = false;
    if (mercenary != null)
    {
      level = mercenary.m_level;
      isFullyUpgraded = mercenary.m_isFullyUpgraded;
      mercenaryDataModel.ExperienceInitial = mercenaryDataModel.ExperienceFinal = (int) mercenary.m_experience;
      mercenaryDataModel.FullyUpgradedInitial = mercenaryDataModel.FullyUpgradedFinal = mercenary.m_isFullyUpgraded;
      mercenaryDataModel.Owned = mercenary.m_owned;
      mercenaryDataModel.ShowAsNew = CollectionManager.Get().DoesMercenaryNeedToBeAcknowledged(mercenary);
      mercenaryDataModel.NumNewPortraits = CollectionManager.Get().GetNumNewPortraitsToAcknowledgeForMercenary(mercenary);
    }
    int attack;
    int health;
    CollectionUtils.GetMercenaryStatsByLevel(mercenaryId, level, isFullyUpgraded, out attack, out health);
    mercenaryDataModel.MercenaryLevel = level;
    mercenaryDataModel.Card = new CardDataModel()
    {
      CardId = cardDbfRecord.NoteMiniGuid,
      Premium = premium,
      Attack = attack,
      Health = health
    };
    return mercenaryDataModel;
  }

  public static LettuceMercenaryDataModel CreateMercenaryDataModel(
    LettuceMercenary mercenary,
    LettuceMercenary.ArtVariation desiredArtVariation = null)
  {
    return MercenaryFactory.CreatePopulatedMercenaryDataModel(mercenary, CollectionUtils.MercenaryDataPopluateExtra.None, desiredArtVariation);
  }

  public static LettuceMercenaryDataModel CreateMercenaryDataModelWithCoin(
    LettuceMercenary mercenary)
  {
    return MercenaryFactory.CreatePopulatedMercenaryDataModel(mercenary, CollectionUtils.MercenaryDataPopluateExtra.Coin, (LettuceMercenary.ArtVariation) null);
  }

  private static LettuceMercenaryDataModel CreatePopulatedMercenaryDataModel(
    LettuceMercenary mercenary,
    CollectionUtils.MercenaryDataPopluateExtra extraRequests,
    LettuceMercenary.ArtVariation desiredArtVariation)
  {
    LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateEmptyMercenaryDataModel();
    CollectionUtils.PopulateMercenaryDataModel(mercenaryDataModel, mercenary, extraRequests, desiredArtVariation);
    return mercenaryDataModel;
  }

  private static string GetLocalizedName(this EntityDef entityDef)
  {
    string name = entityDef?.GetName();
    return !string.IsNullOrWhiteSpace(name) ? GameStrings.FormatLocalizedString(name) : (string) null;
  }

  private static string GetLocalizedShortName(this EntityDef entityDef)
  {
    string shortName = entityDef?.GetShortName();
    return !string.IsNullOrWhiteSpace(shortName) ? GameStrings.FormatLocalizedString(shortName) : entityDef.GetLocalizedName();
  }
}
