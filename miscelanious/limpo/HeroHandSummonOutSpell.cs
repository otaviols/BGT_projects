using UnityEngine;

public class HeroHandSummonOutSpell : Spell
{
  private const string FRIENDLY_BONE_NAME = "FriendlyHeroSummonOut";
  private const string OPPONENT_BONE_NAME = "OpponentHeroSummonOut";
  public float m_MoveTime;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    this.MoveToTarget();
  }

  private void MoveToTarget()
  {
    Card sourceCard = this.GetSourceCard();
    string name = sourceCard.GetControllerSide() == Player.Side.FRIENDLY ? "FriendlyHeroSummonOut" : "OpponentHeroSummonOut";
    Transform bone = Board.Get().FindBone(name);
    if ((Object) bone == (Object) null)
    {
      Debug.LogErrorFormat("Failed to find a target bone: {0}, card: {1}", (object) name, (object) sourceCard);
    }
    else
    {
      sourceCard.SetDoNotSort(true);
      iTween.MoveTo(sourceCard.gameObject, bone.position, this.m_MoveTime);
      iTween.RotateTo(sourceCard.gameObject, bone.localEulerAngles, this.m_MoveTime);
      iTween.ScaleTo(sourceCard.gameObject, bone.localScale, this.m_MoveTime);
    }
  }

  public override void OnSpellFinished()
  {
    this.GetSourceCard().SetDoNotSort(false);
    base.OnSpellFinished();
  }
}
