using UnityEngine;

public class BaconCollectionCardVisual : CollectionCardVisual
{
  protected override bool ShouldShowNewItemGlow(Actor actor)
  {
    string cardId = actor.GetEntityDef().GetCardId();
    CollectionUtils.ViewMode visualType = this.GetVisualType();
    switch (visualType)
    {
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        return CollectionManager.Get().ShouldShowNewBattlegroundsGuideSkinGlow(cardId);
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        return CollectionManager.Get().ShouldShowNewBattlegroundsHeroSkinGlow(cardId);
      default:
        Debug.LogWarning((object) string.Format("{0}.{1}: Unexpected visual type '{2}' for card '{3}'", (object) nameof (BaconCollectionCardVisual), (object) nameof (ShouldShowNewItemGlow), (object) visualType, (object) cardId));
        return false;
    }
  }

  protected override bool IsInCollection(TAG_PREMIUM premium)
  {
    Actor actor = this.GetActor();
    if ((Object) actor == (Object) null)
      return false;
    EntityDef entityDef = actor.GetEntityDef();
    if (entityDef == null)
      return false;
    string cardId = entityDef.GetCardId();
    CollectionUtils.ViewMode visualType = this.GetVisualType();
    switch (visualType)
    {
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        return !entityDef.HasTag(GAME_TAG.BACON_BOB_SKIN) || CollectionManager.Get().OwnsBattlegroundsGuideSkin(cardId);
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        return !entityDef.HasTag(GAME_TAG.BACON_SKIN) || CollectionManager.Get().OwnsBattlegroundsHeroSkin(cardId);
      default:
        Debug.LogWarning((object) string.Format("{0}.{1}: Unexpected visual type '{2}' for card '{3}'", (object) nameof (BaconCollectionCardVisual), (object) nameof (IsInCollection), (object) visualType, (object) cardId));
        return false;
    }
  }

  public override void MarkAsSeen()
  {
    string cardId = this.CardId;
    if (string.IsNullOrEmpty(cardId))
      return;
    CollectionUtils.ViewMode visualType = this.GetVisualType();
    switch (visualType)
    {
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        CollectionManager.Get().MarkBattlegroundsGuideSkinSeen(cardId, this.Premium);
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        CollectionManager.Get().MarkBattlegroundsHeroSkinSeen(cardId, this.Premium);
        break;
      default:
        Debug.LogWarning((object) string.Format("{0}.{1}: Unexpected visual type '{2}' for card '{3}'", (object) nameof (BaconCollectionCardVisual), (object) nameof (MarkAsSeen), (object) visualType, (object) cardId));
        break;
    }
  }
}
