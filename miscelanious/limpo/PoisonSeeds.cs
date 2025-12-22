using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSeeds : SuperSpell
{
  public Spell m_CustomSpawnSpell;
  public Spell m_CustomDeathSpell;
  public float m_StartDeathSpellAdjustment = 0.01f;
  public AnimationCurve m_HeightCurve;
  public float m_RotationDriftAmount;
  public AnimationCurve m_RotationDriftCurve;
  public ParticleSystem m_ImpactParticles;
  public ParticleSystem m_DustParticles;
  private PoisonSeeds.SpellTargetType m_TargetType;
  private float m_HeightCurveLength;
  private float m_AnimTime;
  private AudioSource m_Sound;

  protected override void Awake()
  {
    this.m_Sound = this.GetComponent<AudioSource>();
    base.Awake();
  }

  public override bool AddPowerTargets()
  {
    this.m_visualToTargetIndexMap.Clear();
    this.m_targetToMetaDataMap.Clear();
    this.m_targets.Clear();
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      PowerTask task = taskList[index];
      Card cardFromPowerTask = this.GetTargetCardFromPowerTask(index, task);
      if (!((Object) cardFromPowerTask == (Object) null))
        this.m_targets.Add(cardFromPowerTask.gameObject);
    }
    return this.m_targets.Count > 0;
  }

  protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    int id;
    if (power.Type == Network.PowerType.FULL_ENTITY)
    {
      this.m_TargetType = PoisonSeeds.SpellTargetType.Create;
      id = (power as Network.HistFullEntity).Entity.ID;
    }
    else
    {
      if (!(power is Network.HistTagChange histTagChange) || histTagChange.Tag != 360 || histTagChange.Value <= 0)
        return (Card) null;
      this.m_TargetType = PoisonSeeds.SpellTargetType.Death;
      id = histTagChange.Entity;
    }
    Entity entity = GameState.Get().GetEntity(id);
    if (entity != null)
      return entity.GetCard();
    Debug.LogWarning((object) string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) id));
    return (Card) null;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    if (this.m_TargetType == PoisonSeeds.SpellTargetType.Death)
      this.DeathEffect();
    else if (this.m_TargetType == PoisonSeeds.SpellTargetType.Create)
    {
      this.StartCoroutine(this.CreateEffect());
    }
    else
    {
      --this.m_effectsPendingFinish;
      this.FinishIfPossible();
    }
  }

  private IEnumerator CreateEffect()
  {
    PoisonSeeds poisonSeeds = this;
    foreach (GameObject target in poisonSeeds.GetTargets())
    {
      if (!((Object) target == (Object) null))
      {
        Card component = target.GetComponent<Card>();
        if (!((Object) component == (Object) null))
        {
          component.OverrideCustomSpawnSpell(SpellManager.Get().GetSpell(poisonSeeds.m_CustomSpawnSpell));
          ZonePlay zone = (ZonePlay) component.GetZone();
          if (!((Object) zone == (Object) null))
            zone.SetTransitionTime(0.01f);
        }
      }
    }
    --poisonSeeds.m_effectsPendingFinish;
    poisonSeeds.FinishIfPossible();
    poisonSeeds.ShakeCamera();
    yield return (object) new WaitForSeconds(1f);
    foreach (GameObject target in poisonSeeds.GetTargets())
    {
      Card component = target.GetComponent<Card>();
      if (!((Object) component == (Object) null))
      {
        ZonePlay zone = (ZonePlay) component.GetZone();
        if (!((Object) zone == (Object) null))
          zone.ResetTransitionTime();
      }
    }
  }

  private void DeathEffect()
  {
    if (this.m_HeightCurve.length == 0)
      Debug.LogWarning((object) "PoisonSeeds Spell height animation curve in not defined");
    else if (this.m_RotationDriftCurve.length == 0)
    {
      Debug.LogWarning((object) "PoisonSeeds Spell rotation drift animation curve in not defined");
    }
    else
    {
      if ((Object) this.m_CustomDeathSpell != (Object) null)
      {
        foreach (GameObject target in this.GetTargets())
        {
          if (!((Object) target == (Object) null))
            target.GetComponent<Card>().OverrideCustomDeathSpell(SpellManager.Get().GetSpell(this.m_CustomDeathSpell));
        }
      }
      this.m_HeightCurveLength = this.m_HeightCurve[this.m_HeightCurve.length - 1].time;
      List<PoisonSeeds.MinionData> minions = new List<PoisonSeeds.MinionData>();
      foreach (GameObject target in this.GetTargets())
      {
        PoisonSeeds.MinionData minionData = new PoisonSeeds.MinionData();
        minionData.card = target.GetComponent<Card>();
        minionData.gameObject = target;
        minionData.orgLocPos = target.transform.localPosition;
        minionData.orgLocRot = target.transform.localRotation;
        float x = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
        float y = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value) * 0.1f;
        float z = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
        minionData.rotationDrift = new Vector3(x, y, z);
        minions.Add(minionData);
      }
      this.StartCoroutine(this.AnimateDeathEffect(minions));
    }
  }

  private IEnumerator AnimateDeathEffect(List<PoisonSeeds.MinionData> minions)
  {
    PoisonSeeds poisonSeeds = this;
    if ((Object) poisonSeeds.m_Sound != (Object) null)
      SoundManager.Get().Play(poisonSeeds.m_Sound);
    List<ParticleSystem> impactParticles = new List<ParticleSystem>();
    foreach (PoisonSeeds.MinionData minion in minions)
    {
      GameObject gameObject1 = Object.Instantiate<GameObject>(poisonSeeds.m_ImpactParticles.gameObject);
      gameObject1.transform.parent = poisonSeeds.transform;
      gameObject1.transform.position = minion.gameObject.transform.position;
      impactParticles.Add(gameObject1.GetComponentInChildren<ParticleSystem>());
      GameObject gameObject2 = Object.Instantiate<GameObject>(poisonSeeds.m_DustParticles.gameObject);
      gameObject2.transform.parent = poisonSeeds.transform;
      gameObject2.transform.position = minion.gameObject.transform.position;
      gameObject2.GetComponent<ParticleSystem>().Play();
    }
    poisonSeeds.m_AnimTime = 0.0f;
    bool finished = false;
    while ((double) poisonSeeds.m_AnimTime < (double) poisonSeeds.m_HeightCurveLength)
    {
      poisonSeeds.m_AnimTime += Time.deltaTime;
      float num1 = poisonSeeds.m_HeightCurve.Evaluate(poisonSeeds.m_AnimTime);
      float num2 = poisonSeeds.m_RotationDriftCurve.Evaluate(poisonSeeds.m_AnimTime);
      foreach (PoisonSeeds.MinionData minion in minions)
      {
        minion.gameObject.transform.localPosition = new Vector3(minion.orgLocPos.x, minion.orgLocPos.y + num1, minion.orgLocPos.z);
        minion.gameObject.transform.localRotation = minion.orgLocRot;
        minion.gameObject.transform.Rotate(minion.rotationDrift * num2, Space.Self);
      }
      if ((double) poisonSeeds.m_AnimTime > (double) poisonSeeds.m_HeightCurveLength - (double) poisonSeeds.m_StartDeathSpellAdjustment && !finished)
      {
        foreach (PoisonSeeds.MinionData minion in minions)
        {
          if (minion != null && (Object) minion.card != (Object) null && (Object) minion.card.GetActor() != (Object) null)
            minion.card.GetActor().DoCardDeathVisuals();
        }
        --poisonSeeds.m_effectsPendingFinish;
        poisonSeeds.FinishIfPossible();
        finished = true;
      }
      yield return (object) null;
    }
    foreach (ParticleSystem particleSystem in impactParticles)
      particleSystem.Play();
    poisonSeeds.ShakeCamera();
  }

  private void ShakeCamera() => CameraShakeMgr.Shake(Camera.main, new Vector3(0.15f, 0.15f, 0.15f), 0.9f);

  public class MinionData
  {
    public GameObject gameObject;
    public Vector3 orgLocPos;
    public Quaternion orgLocRot;
    public Vector3 rotationDrift;
    public Card card;
  }

  private enum SpellTargetType
  {
    None,
    Death,
    Create,
  }
}
