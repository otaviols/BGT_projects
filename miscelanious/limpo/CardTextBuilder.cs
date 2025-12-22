using Hearthstone;
using UnityEngine;

public class CardTextBuilder
{
  protected bool m_useEntityForTextInPlay;
  protected bool m_useEntityForTextInHand;
  private static CardTextBuilder m_fallbackCardTextBuilder;

  public CardTextBuilder() => this.m_useEntityForTextInPlay = false;

  public static string GetRawCardTextInHand(string cardId)
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(cardId);
    return cardRecord != null && cardRecord.TextInHand != null ? (string) cardRecord.TextInHand : string.Empty;
  }

  public static string GetRawCardName(EntityDef entityDef)
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(entityDef.GetCardId());
    return cardRecord != null && cardRecord.Name != null ? cardRecord.Name.GetString() ?? entityDef.GetDebugName() : entityDef.GetDebugName();
  }

  public static string GetDefaultCardTextInHand(EntityDef entityDef) => TextUtils.TransformCardText(CardTextBuilder.GetRawCardTextInHand(entityDef.GetCardId()));

  public static CardTextBuilder GetFallbackCardTextBuilder()
  {
    if (CardTextBuilder.m_fallbackCardTextBuilder == null)
      CardTextBuilder.m_fallbackCardTextBuilder = CardTextBuilderFactory.Create(Assets.Card.CardTextBuilderType.DEFAULT);
    return CardTextBuilder.m_fallbackCardTextBuilder;
  }

  public static string GetDefaultCardName(EntityDef entityDef)
  {
    if (GameState.Get() != null && GameState.Get().IsMulliganPhase() && CollectionManager.Get().IsBattlegroundsHeroCard(entityDef.GetCardId()))
    {
      int dbId = GameUtils.TranslateCardIdToDbId(entityDef.GetCardId());
      BattlegroundsHeroSkinId skinId;
      int baseHeroCardId;
      if (CollectionManager.Get().GetBattlegroundsHeroSkinIdForSkinCardId(dbId, out skinId) && CollectionManager.Get().GetBattlegroundsBaseCardIdForHeroSkinId(skinId, out baseHeroCardId))
      {
        CardDbfRecord record = GameDbf.Card.GetRecord(baseHeroCardId);
        if (record != null && record.Name != null && !string.IsNullOrEmpty(record.Name.GetString()))
          return TextUtils.TransformCardText(record.Name.GetString());
      }
    }
    return TextUtils.TransformCardText(CardTextBuilder.GetRawCardName(entityDef));
  }

  public virtual string BuildCardName(Entity entity) => CardTextBuilder.GetDefaultCardName(entity.GetEntityDef());

  public virtual string BuildCardName(EntityDef entityDef) => CardTextBuilder.GetDefaultCardName(entityDef);

  public virtual string BuildCardTextInHand(Entity entity) => TextUtils.TransformCardText(entity, CardTextBuilder.GetRawCardTextInHand(entity.GetCardId()));

  public virtual string BuildCardTextInHand(EntityDef entityDef) => CardTextBuilder.GetDefaultCardTextInHand(entityDef);

  public virtual bool ContainsBonusDamageToken(Entity entity) => TextUtils.HasBonusDamage(CardTextBuilder.GetRawCardTextInHand(entity.GetCardId()));

  public virtual bool ContainsBonusHealingToken(Entity entity) => TextUtils.HasBonusHealing(CardTextBuilder.GetRawCardTextInHand(entity.GetCardId()));

  public virtual string BuildCardTextInHistory(Entity entity)
  {
    CardTextHistoryData cardTextHistoryData = entity.GetCardTextHistoryData();
    if (cardTextHistoryData == null)
    {
      Log.All.Print("CardTextBuilder.BuildCardTextInHistory: entity {0} does not have a CardTextHistoryData object.", (object) entity.GetEntityId());
      return "";
    }
    EntityDef entityDef = entity.GetEntityDef();
    string rawCardTextInHand = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId() ?? entityDef.GetCardId());
    return TextUtils.TransformCardText(cardTextHistoryData, rawCardTextInHand);
  }

  public virtual CardTextHistoryData CreateCardTextHistoryData() => new CardTextHistoryData();

  public virtual string GetTargetingArrowText(Entity entity)
  {
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(entity.GetCardId());
    return cardRecord == null || cardRecord.TargetArrowText == null ? string.Empty : TextUtils.TransformCardText(entity, cardRecord.TargetArrowText.GetString());
  }

  public virtual void OnTagChange(Card card, TagDelta tagChange)
  {
    switch ((GAME_TAG) tagChange.tag)
    {
      case GAME_TAG.CURRENT_SPELLPOWER:
        if (!((Object) card != (Object) null) || !((Object) card.GetActor() != (Object) null))
          break;
        card.GetActor().UpdatePowersText();
        break;
      case GAME_TAG.OVERRIDECARDTEXTBUILDER:
        if (!((Object) card != (Object) null) || !((Object) card.GetActor() != (Object) null))
          break;
        Actor actor = card.GetActor();
        if (actor.GetEntity() != null && actor.GetEntity().GetEntityDef() != null)
          actor.GetEntity().GetEntityDef().ClearCardTextBuilder();
        actor.UpdatePowersText();
        break;
    }
  }

  public bool ShouldUseEntityForTextInPlay() => this.m_useEntityForTextInPlay;

  public bool ShouldUseEntityForTextInHand() => this.m_useEntityForTextInPlay;
}
