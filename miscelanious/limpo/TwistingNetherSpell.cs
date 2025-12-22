using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistingNetherSpell : SuperSpell
{
  public float m_FinishTime;
  public TwistingNetherLiftInfo m_LiftInfo;
  public TwistingNetherFloatInfo m_FloatInfo;
  public TwistingNetherDrainInfo m_DrainInfo;
  public TwistingNetherSqueezeInfo m_SqueezeInfo;
  private static readonly Vector3 DEAD_SCALE = new Vector3(0.01f, 0.01f, 0.01f);
  private List<TwistingNetherSpell.Victim> m_victims = new List<TwistingNetherSpell.Victim>();

  protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    if (power.Type != Network.PowerType.TAG_CHANGE)
      return (Card) null;
    Network.HistTagChange histTagChange = power as Network.HistTagChange;
    if ((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null)
    {
      if (histTagChange.Tag != 360)
        return (Card) null;
      if (histTagChange.Value != 1)
        return (Card) null;
    }
    Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
    if (entity != null)
      return entity.GetCard();
    Debug.LogWarning((object) string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity));
    return (Card) null;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    if (this.IsFinished())
      return;
    this.Begin();
    if (!this.CanFinish())
      return;
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }

  protected override void CleanUp()
  {
    base.CleanUp();
    this.m_victims.Clear();
  }

  private void Begin()
  {
    ++this.m_effectsPendingFinish;
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) this.m_FinishTime, (object) "onupdate", (object) (Action<object>) (amount => { }), (object) "oncomplete", (object) "OnFinishTimeFinished", (object) "oncompletetarget", (object) this.gameObject));
    this.Setup();
    this.Lift();
  }

  private void Setup()
  {
    foreach (GameObject target in this.GetTargets())
    {
      Card component = target.GetComponent<Card>();
      component.SetDoNotSort(true);
      this.m_victims.Add(new TwistingNetherSpell.Victim()
      {
        m_state = TwistingNetherSpell.VictimState.NONE,
        m_card = component
      });
    }
  }

  private void Lift()
  {
    foreach (TwistingNetherSpell.Victim victim in this.m_victims)
    {
      victim.m_state = TwistingNetherSpell.VictimState.LIFTING;
      Vector3 vector3 = TransformUtil.RandomVector3(this.m_LiftInfo.m_OffsetMin, this.m_LiftInfo.m_OffsetMax);
      Hashtable args1 = iTween.Hash((object) "position", (object) (victim.m_card.transform.position + vector3), (object) "delay", (object) UnityEngine.Random.Range(this.m_LiftInfo.m_DelayMin, this.m_LiftInfo.m_DelayMax), (object) "time", (object) UnityEngine.Random.Range(this.m_LiftInfo.m_DurationMin, this.m_LiftInfo.m_DurationMax), (object) "easeType", (object) this.m_LiftInfo.m_EaseType, (object) "oncomplete", (object) "OnLiftFinished", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) victim);
      Hashtable args2 = iTween.Hash((object) "rotation", (object) new Vector3(UnityEngine.Random.Range(this.m_LiftInfo.m_RotationMin, this.m_LiftInfo.m_RotationMax), UnityEngine.Random.Range(this.m_LiftInfo.m_RotationMin, this.m_LiftInfo.m_RotationMax), UnityEngine.Random.Range(this.m_LiftInfo.m_RotationMin, this.m_LiftInfo.m_RotationMax)), (object) "delay", (object) UnityEngine.Random.Range(this.m_LiftInfo.m_RotDelayMin, this.m_LiftInfo.m_RotDelayMax), (object) "time", (object) UnityEngine.Random.Range(this.m_LiftInfo.m_RotDurationMin, this.m_LiftInfo.m_RotDurationMax), (object) "easeType", (object) this.m_LiftInfo.m_EaseType);
      iTween.MoveTo(victim.m_card.gameObject, args1);
      iTween.RotateTo(victim.m_card.gameObject, args2);
    }
  }

  private void OnLiftFinished(TwistingNetherSpell.Victim victim) => this.Float(victim);

  private void Float(TwistingNetherSpell.Victim victim)
  {
    victim.m_state = TwistingNetherSpell.VictimState.FLOATING;
    Hashtable args = iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) UnityEngine.Random.Range(this.m_FloatInfo.m_DurationMin, this.m_FloatInfo.m_DurationMax), (object) "onupdate", (object) (Action<object>) (amount => { }), (object) "oncomplete", (object) "OnFloatFinished", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) victim);
    iTween.ValueTo(victim.m_card.gameObject, args);
  }

  private void OnFloatFinished(TwistingNetherSpell.Victim victim) => this.Drain(victim);

  private void Drain(TwistingNetherSpell.Victim victim)
  {
    victim.m_state = TwistingNetherSpell.VictimState.LIFTING;
    Hashtable args1 = iTween.Hash((object) "position", (object) this.transform.position, (object) "delay", (object) UnityEngine.Random.Range(this.m_DrainInfo.m_DelayMin, this.m_DrainInfo.m_DelayMax), (object) "time", (object) UnityEngine.Random.Range(this.m_DrainInfo.m_DurationMin, this.m_DrainInfo.m_DurationMax), (object) "easeType", (object) this.m_DrainInfo.m_EaseType, (object) "oncomplete", (object) "OnDrainFinished", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) victim);
    iTween.MoveTo(victim.m_card.gameObject, args1);
    float num1 = UnityEngine.Random.Range(this.m_SqueezeInfo.m_DelayMin, this.m_SqueezeInfo.m_DelayMax);
    float num2 = UnityEngine.Random.Range(this.m_SqueezeInfo.m_DurationMin, this.m_SqueezeInfo.m_DurationMax);
    Hashtable args2 = iTween.Hash((object) "scale", (object) TwistingNetherSpell.DEAD_SCALE, (object) "delay", (object) num1, (object) "time", (object) num2, (object) "easeType", (object) this.m_SqueezeInfo.m_EaseType);
    iTween.ScaleTo(victim.m_card.gameObject, args2);
  }

  private void OnDrainFinished(TwistingNetherSpell.Victim victim) => this.CleanUpVictim(victim);

  private void OnFinishTimeFinished()
  {
    foreach (TwistingNetherSpell.Victim victim in this.m_victims)
      this.CleanUpVictim(victim);
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }

  private void CleanUpVictim(TwistingNetherSpell.Victim victim)
  {
    if (victim.m_state == TwistingNetherSpell.VictimState.DEAD)
      return;
    victim.m_state = TwistingNetherSpell.VictimState.DEAD;
    victim.m_card.SetDoNotSort(false);
  }

  private bool CanFinish()
  {
    foreach (TwistingNetherSpell.Victim victim in this.m_victims)
    {
      if (victim.m_state != TwistingNetherSpell.VictimState.DEAD)
        return false;
    }
    return true;
  }

  private enum VictimState
  {
    NONE,
    LIFTING,
    FLOATING,
    DRAINING,
    DEAD,
  }

  private class Victim
  {
    public TwistingNetherSpell.VictimState m_state;
    public Card m_card;
  }
}
