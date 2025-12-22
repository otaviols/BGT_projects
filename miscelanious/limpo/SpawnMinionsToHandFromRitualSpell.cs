using UnityEngine;

public class SpawnMinionsToHandFromRitualSpell : SpawnToHandSpell
{
  public string m_friendlyInvokeBoneName = "FriendlyRitual";
  public string m_opponentInvokeBoneName = "OpponentRitual";

  protected override Vector3 GetOriginForTarget(int targetIndex = 0)
  {
    Entity entity = this.GetSourceCard().GetEntity();
    Player controller = entity.GetController();
    if (controller.GetTag(GAME_TAG.MAIN_GALAKROND) == controller.GetHero().GetEntityId())
      return base.GetOriginForTarget(targetIndex);
    string name = entity.GetControllerSide() == Player.Side.FRIENDLY ? this.m_friendlyInvokeBoneName : this.m_opponentInvokeBoneName;
    return Board.Get().FindBone(name).position;
  }
}
