using System;

public class ModularEntityCardTextBuilder : CardTextBuilder
{
  public ModularEntityCardTextBuilder() => this.m_useEntityForTextInPlay = true;

  public override string BuildCardTextInHand(Entity entity)
  {
    string text = this.BuildFormattedText(entity);
    return TextUtils.TransformCardText(entity, text).Trim(Environment.NewLine.ToCharArray());
  }

  public override string BuildCardTextInHand(EntityDef entityDef) => string.Empty;

  public override bool ContainsBonusDamageToken(Entity entity) => TextUtils.HasBonusDamage(this.BuildFormattedText(entity));

  public override bool ContainsBonusHealingToken(Entity entity) => TextUtils.HasBonusHealing(this.BuildFormattedText(entity));

  public virtual string GetRawCardTextInHandForCardBeingBuilt(Entity ent) => CardTextBuilder.GetRawCardTextInHand(ent.GetCardId());

  public override string BuildCardTextInHistory(Entity entity)
  {
    string power1;
    string power2;
    this.GetPowersText(entity, out power1, out power2);
    CardTextHistoryData cardTextHistoryData = entity.GetCardTextHistoryData();
    if (cardTextHistoryData == null)
    {
      Log.All.Print("ModularEntityCardTextBuilder.BuildCardTextInHistory: entity {0} does not have a CardTextHistoryData object.", (object) entity.GetEntityId());
      return "";
    }
    string text = string.Format(this.GetRawCardTextInHandForCardBeingBuilt(entity), (object) power1, (object) power2);
    return TextUtils.TransformCardText(cardTextHistoryData, text).Trim(Environment.NewLine.ToCharArray());
  }

  protected void GetPowersText(Entity entity, out string power1, out string power2)
  {
    power1 = string.Empty;
    if (entity.HasTag(GAME_TAG.MODULAR_ENTITY_PART_1))
    {
      int tag = entity.GetTag(GAME_TAG.MODULAR_ENTITY_PART_1);
      EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
      if (entityDef != null)
      {
        power1 = CardTextBuilder.GetRawCardTextInHand(entityDef.GetCardId());
        power1 = this.GetPowerTextSubstring(power1);
      }
    }
    power2 = string.Empty;
    if (!entity.HasTag(GAME_TAG.MODULAR_ENTITY_PART_2))
      return;
    int tag1 = entity.GetTag(GAME_TAG.MODULAR_ENTITY_PART_2);
    EntityDef entityDef1 = DefLoader.Get().GetEntityDef(tag1);
    if (entityDef1 == null)
      return;
    power2 = CardTextBuilder.GetRawCardTextInHand(entityDef1.GetCardId());
    power2 = this.GetPowerTextSubstring(power2);
  }

  private string BuildFormattedText(Entity entity)
  {
    string power1;
    string power2;
    this.GetPowersText(entity, out power1, out power2);
    return string.Format(this.GetRawCardTextInHandForCardBeingBuilt(entity), (object) power1, (object) power2);
  }

  private string GetPowerTextSubstring(string powerText)
  {
    int num = powerText.IndexOf('@');
    return num >= 0 ? powerText.Substring(num + 1) : powerText;
  }
}
