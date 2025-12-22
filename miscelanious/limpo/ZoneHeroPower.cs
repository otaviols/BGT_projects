using UnityEngine;

public class ZoneHeroPower : Zone
{
  public override string ToString() => string.Format("{0} (Hero Power)", (object) base.ToString());

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return base.CanAcceptTags(controllerId, zoneTag, cardType, entity) && cardType == TAG_CARDTYPE.HERO_POWER;
  }

  public override Transform GetZoneTransformForCard(Card card) => this.transform;
}
