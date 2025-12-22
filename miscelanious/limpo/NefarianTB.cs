using UnityEngine;

public class NefarianTB : Spell
{
  protected override void OnAction(SpellStateType prevStateType)
  {
    this.BlockZoneLayout();
    base.OnAction(prevStateType);
  }

  private void BlockZoneLayout()
  {
    Card sourceCard = this.GetSourceCard();
    if ((Object) sourceCard == (Object) null)
      return;
    Player controller = sourceCard.GetController();
    if (controller == null)
      return;
    ZonePlay battlefieldZone = controller.GetBattlefieldZone();
    if ((Object) battlefieldZone == (Object) null)
      return;
    battlefieldZone.AddLayoutBlocker();
  }
}
