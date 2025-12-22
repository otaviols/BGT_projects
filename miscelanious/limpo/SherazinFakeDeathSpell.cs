using PegasusGame;
using UnityEngine;

public class SherazinFakeDeathSpell : OverrideCustomSpawnSpell
{
  private bool m_mustPlayFakeDeath;
  public float m_delayBeforeHideActor = 3f;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets() || this.m_taskList.GetBlockType() != HistoryBlock.Type.TRIGGER)
      return false;
    if ((Object) this.GetSourceCard() == (Object) null || this.GetSourceCard().GetEntity() == null)
    {
      Log.Spells.PrintError("SherazinFakeDeathSpell.AddPowerTargets(): Failed to find source entity for Sherazin.");
      return false;
    }
    if (this.GetSourceCard().GetEntity().GetZone() == TAG_ZONE.PLAY)
    {
      this.m_mustPlayFakeDeath = true;
      return true;
    }
    this.m_mustPlayFakeDeath = false;
    return true;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    if (this.m_mustPlayFakeDeath)
    {
      ++this.m_effectsPendingFinish;
      base.OnAction(prevStateType);
      this.GetSourceCard().FakeDeath();
      this.GetSourceCard().SetDelayBeforeHideInNullZoneVisuals(this.m_delayBeforeHideActor);
      --this.m_effectsPendingFinish;
      this.OnSpellFinished();
      this.OnStateFinished();
    }
    else
      base.OnAction(prevStateType);
  }
}
