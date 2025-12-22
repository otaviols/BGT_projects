using UnityEngine;

public class ZoneHero : Zone
{
  private Vector3? m_originalPosition;

  public override string ToString() => string.Format("{0} (Hero)", (object) base.ToString());

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return base.CanAcceptTags(controllerId, zoneTag, cardType, entity) && cardType == TAG_CARDTYPE.HERO;
  }

  public override void OnHealingDoesDamageEntityEnteredPlay()
  {
  }

  public override void OnHealingDoesDamageEntityMousedOut()
  {
  }

  public override void OnHealingDoesDamageEntityMousedOver()
  {
  }

  public override void OnLifestealDoesDamageEntityEnteredPlay()
  {
  }

  public override void OnLifestealDoesDamageEntityMousedOut()
  {
  }

  public override void OnLifestealDoesDamageEntityMousedOver()
  {
  }

  public override void UpdateLayout()
  {
    if (!this.m_originalPosition.HasValue)
      this.m_originalPosition = new Vector3?(this.transform.localPosition);
    Actor actor = this.GetFirstCard()?.GetActor();
    if ((bool) (Object) actor)
      this.transform.localPosition = this.m_originalPosition.Value + new Vector3(0.0f, actor.ZoneHeroPositionOffset, 0.0f);
    else
      this.transform.localPosition = this.m_originalPosition.Value;
    base.UpdateLayout();
  }

  public Vector3 OriginalPosition
  {
    get
    {
      if (!this.m_originalPosition.HasValue)
        this.m_originalPosition = new Vector3?(this.transform.localPosition);
      return (Object) this.transform.parent != (Object) null ? this.transform.parent.TransformPoint(this.m_originalPosition.Value) : this.m_originalPosition.Value;
    }
  }
}
