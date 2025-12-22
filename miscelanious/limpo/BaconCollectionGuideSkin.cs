using Hearthstone;

public class BaconCollectionGuideSkin : BaconCollectionSkin
{
  protected override string GetFavoritedText() => GameStrings.Get("GLUE_BACON_COLLECTION_FAVORITE_GUIDE");

  public void SetCardStateDisplay(CollectibleCard card)
  {
    if (!CollectionManager.Get().GetBattlegroundsGuideSkinIdForCardId(card.CardDbId, out BattlegroundsGuideSkinId _) || CollectionManager.Get().OwnsBattlegroundsGuideSkin(card.CardDbId))
      return;
    this.gameObject.GetComponent<Actor>().MissingCardEffect();
  }
}
