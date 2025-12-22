using Hearthstone;

[CustomEditClass]
public static class BaconHeroSkinUtils
{
  public static bool CanFavoriteBattlegroundsHeroSkin(EntityDef entityDef)
  {
    if (!CollectionManager.Get().IsBattlegroundsHeroCard(entityDef.GetCardId()))
      return false;
    int dbId = GameUtils.TranslateCardIdToDbId(entityDef.GetCardId());
    if (CollectionManager.Get().IsBattlegroundsBaseHeroCardWithSkin(dbId))
      return CollectionManager.Get().GetFavoriteBattlegroundsHeroSkin(dbId, out BattlegroundsHeroSkinId _);
    BattlegroundsHeroSkinId skinId;
    int baseHeroCardId;
    if (!CollectionManager.Get().IsBattlegroundsHeroSkinCard(dbId) || !CollectionManager.Get().OwnsBattlegroundsHeroSkin(entityDef.GetCardId()) || !CollectionManager.Get().GetBattlegroundsHeroSkinIdForSkinCardId(dbId, out skinId) || !CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(skinId, out baseHeroCardId))
      return false;
    BattlegroundsHeroSkinId favoriteSkinId;
    return !CollectionManager.Get().GetFavoriteBattlegroundsHeroSkin(baseHeroCardId, out favoriteSkinId) || skinId != favoriteSkinId;
  }

  public static bool IsBattlegroundsHeroSkinFavorited(EntityDef entityDef)
  {
    if (!CollectionManager.Get().IsBattlegroundsHeroCard(entityDef.GetCardId()))
      return false;
    int dbId = GameUtils.TranslateCardIdToDbId(entityDef.GetCardId());
    BattlegroundsHeroSkinId skinId;
    int baseHeroCardId;
    BattlegroundsHeroSkinId favoriteSkinId;
    return CollectionManager.Get().IsBattlegroundsBaseHeroCardWithSkin(dbId) ? !CollectionManager.Get().GetFavoriteBattlegroundsHeroSkin(dbId, out BattlegroundsHeroSkinId _) && CollectionManager.Get().OwnsAssociatedBattlegroundsHeroSkin(dbId) : CollectionManager.Get().IsBattlegroundsHeroSkinCard(dbId) && CollectionManager.Get().GetBattlegroundsHeroSkinIdForSkinCardId(dbId, out skinId) && CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(skinId, out baseHeroCardId) && CollectionManager.Get().GetFavoriteBattlegroundsHeroSkin(baseHeroCardId, out favoriteSkinId) && skinId == favoriteSkinId;
  }

  public static bool CanFavoriteBattlegroundsGuideSkin(EntityDef entityDef)
  {
    if (!CollectionManager.Get().IsBattlegroundsGuideCardId(entityDef.GetCardId()))
      return false;
    int dbId = GameUtils.TranslateCardIdToDbId(entityDef.GetCardId());
    BattlegroundsGuideSkinId skinId;
    if (!CollectionManager.Get().GetBattlegroundsGuideSkinIdForCardId(dbId, out skinId))
      return CollectionManager.Get().HasFavoriteBattlegroundsGuideSkin();
    if (!CollectionManager.Get().OwnsBattlegroundsGuideSkin(dbId))
      return false;
    BattlegroundsGuideSkinId favoriteSkinId;
    return !CollectionManager.Get().GetFavoriteBattlegroundsGuideSkin(out favoriteSkinId) || favoriteSkinId != skinId;
  }

  public static bool IsBattlegroundsGuideSkinFavorited(EntityDef entityDef)
  {
    if (!CollectionManager.Get().IsBattlegroundsGuideCardId(entityDef.GetCardId()))
      return false;
    int dbId = GameUtils.TranslateCardIdToDbId(entityDef.GetCardId());
    BattlegroundsGuideSkinId skinId;
    BattlegroundsGuideSkinId favoriteSkinId;
    return CollectionManager.Get().GetBattlegroundsGuideSkinIdForCardId(dbId, out skinId) ? CollectionManager.Get().GetFavoriteBattlegroundsGuideSkin(out favoriteSkinId) && favoriteSkinId == skinId : !CollectionManager.Get().HasFavoriteBattlegroundsGuideSkin() && CollectionManager.Get().OwnsAnyBattlegroundsGuideSkin();
  }

  public static BaconHeroSkinUtils.RotationType GetBattleGroundsHeroRotationType(
    CardDbfRecord cardRecord,
    EntityDef cardDef)
  {
    if (!cardDef.HasTag(GAME_TAG.BACON_HERO_CAN_BE_DRAFTED) || !SpecialEventManager.Get().IsEventActive(cardRecord.BattlegroundsActiveEvent, false))
      return BaconHeroSkinUtils.RotationType.Resting;
    return SpecialEventManager.Get().IsEventActive(cardRecord.BattlegroundsEarlyAccessEvent, false) ? BaconHeroSkinUtils.RotationType.Preview : BaconHeroSkinUtils.RotationType.Active;
  }

  public enum RotationType
  {
    Active,
    Resting,
    Preview,
  }
}
