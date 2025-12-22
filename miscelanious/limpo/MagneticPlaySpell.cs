using PegasusGame;
using System.Collections;
using UnityEngine;

public class MagneticPlaySpell : Spell
{
  public float m_AttachSpeed = 3f;

  public override bool AttachPowerTaskList(PowerTaskList taskList)
  {
    base.AttachPowerTaskList(taskList);
    foreach (PowerTask task in taskList.GetTaskList())
    {
      if (task.GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.TARGET)
        return true;
    }
    if (taskList.IsEndOfBlock())
    {
      MagneticPlayData magneticPlayData = this.GetSourceCard().GetMagneticPlayData();
      if (magneticPlayData != null)
      {
        if ((Object) magneticPlayData.m_beamSpell != (Object) null)
          Object.Destroy((Object) magneticPlayData.m_beamSpell.gameObject);
        magneticPlayData.m_playedCard.GetActor().ToggleForceIdle(false);
        magneticPlayData.m_playedCard.UpdateActorState();
        magneticPlayData.m_targetMech.GetActor().ToggleForceIdle(false);
        magneticPlayData.m_targetMech.UpdateActorState();
        SpellUtils.ActivateDeathIfNecessary(magneticPlayData.m_playedCard.GetActorSpell(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
        SpellUtils.ActivateDeathIfNecessary(magneticPlayData.m_playedCard.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_RIGHT));
        SpellUtils.ActivateDeathIfNecessary(magneticPlayData.m_targetMech.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
      }
    }
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoMagneticEffect());
  }

  private IEnumerator DoMagneticEffect()
  {
    MagneticPlaySpell magneticPlaySpell = this;
    Card sourceCard = magneticPlaySpell.GetSourceCard();
    Card targetCard = magneticPlaySpell.GetTargetCard();
    MagneticPlayData magneticPlayData = sourceCard.GetMagneticPlayData();
    if (magneticPlayData == null)
    {
      ZonePlay battlefieldZone = sourceCard.GetController().GetBattlefieldZone();
      magneticPlayData = new MagneticPlayData();
      magneticPlayData.m_playedCard = sourceCard;
      magneticPlayData.m_targetMech = targetCard;
      magneticPlayData.m_beamSpell = (MagneticBeamSpell) SpellManager.Get().GetSpell((Spell) battlefieldZone.GetMagneticBeamSpell());
      sourceCard.SetMagneticPlayData(magneticPlayData);
      targetCard.SetIsMagneticTarget(true);
      magneticPlayData.m_beamSpell.SetSource(sourceCard.gameObject);
      magneticPlayData.m_beamSpell.AddTarget(targetCard.gameObject);
      magneticPlayData.m_beamSpell.Activate();
      SpellUtils.ActivateBirthIfNecessary(sourceCard.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_RIGHT));
      SpellUtils.ActivateBirthIfNecessary(targetCard.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
      yield return (object) new WaitForSeconds(0.5f);
    }
    Card source = magneticPlayData.m_playedCard;
    Card target = magneticPlayData.m_targetMech;
    source.SetDoNotSort(true);
    Vector3 delta = source.transform.position - target.transform.position;
    while ((double) delta.sqrMagnitude != 0.0)
    {
      delta = Vector3.MoveTowards(delta, Vector3.zero, magneticPlaySpell.m_AttachSpeed * Time.deltaTime);
      source.transform.position = target.transform.position + delta;
      SpellUtils.ActivateBirthIfNecessary(magneticPlayData.m_targetMech.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
      yield return (object) null;
    }
    source.HideCard();
    source.SetDoNotSort(false);
    sourceCard.GetActor().ToggleForceIdle(false);
    sourceCard.UpdateActorState();
    targetCard.GetActor().ToggleForceIdle(false);
    targetCard.UpdateActorState();
    SpellUtils.ActivateDeathIfNecessary(magneticPlayData.m_playedCard.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_RIGHT));
    SpellUtils.ActivateDeathIfNecessary(magneticPlayData.m_targetMech.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
    SpellUtils.ActivateDeathIfNecessary((Spell) magneticPlayData.m_beamSpell);
    if ((Object) magneticPlayData.m_beamSpell != (Object) null)
      Object.Destroy((Object) magneticPlayData.m_beamSpell.gameObject);
    else
      Log.Gameplay.PrintError("{0}.DoMagneticEffect(): magneticPlayData.m_beamSpell is null! Source={1}. Target={2}.", (object) magneticPlaySpell, (object) sourceCard, (object) targetCard);
    sourceCard.SetMagneticPlayData((MagneticPlayData) null);
    target.SetIsMagneticTarget(false);
    magneticPlaySpell.OnStateFinished();
    magneticPlaySpell.OnSpellFinished();
  }
}
