using UnityEngine;

public class ScoreValueCountDownCardTextBuilder : CardTextBuilder
{
  public ScoreValueCountDownCardTextBuilder()
  {
    this.m_useEntityForTextInPlay = true;
    this.m_useEntityForTextInHand = true;
  }

  public override string BuildCardTextInHand(Entity entity)
  {
    string text = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId()).Replace("@", this.GetProgressRemaining(entity).ToString());
    return TextUtils.TransformCardText(entity, text);
  }

  public override string BuildCardTextInHistory(Entity entity) => this.BuildCardTextInHand(entity);

  private int GetProgressRemaining(Entity entity)
  {
    int progressRemaining = entity.GetTag(GAME_TAG.SCORE_VALUE_1) - entity.GetTag(GAME_TAG.SCORE_VALUE_2);
    if (progressRemaining < 0)
      progressRemaining = 0;
    return progressRemaining;
  }

  public override string BuildCardTextInHand(EntityDef entityDef) => TextUtils.TransformCardText(CardTextBuilder.GetRawCardTextInHand(entityDef.GetCardId()).Replace("@", entityDef.GetTag(GAME_TAG.SCORE_VALUE_1).ToString()));

  public override void OnTagChange(Card card, TagDelta tagChange)
  {
    if ((Object) card == (Object) null)
      return;
    Actor actor = card.GetActor();
    if ((Object) actor == (Object) null)
      return;
    switch ((GAME_TAG) tagChange.tag)
    {
      case GAME_TAG.SCORE_VALUE_1:
      case GAME_TAG.SCORE_VALUE_2:
        actor.UpdateTextComponents();
        break;
      default:
        base.OnTagChange(card, tagChange);
        break;
    }
  }
}
