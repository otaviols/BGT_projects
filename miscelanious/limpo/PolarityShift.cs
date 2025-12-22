using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarityShift : SuperSpell
{
  public AnimationCurve m_HeightCurve;
  public float m_RotationDriftAmount;
  public AnimationCurve m_RotationDriftCurve;
  public float m_ParticleHeightOffset = 0.1f;
  public ParticleSystem m_GlowParticle;
  public ParticleSystem m_LightningParticle;
  public ParticleSystem m_ImpactParticle;
  public ParticleEffects m_ParticleEffects;
  public float m_CleanupTime = 2f;
  public float m_SpellFinishTime = 2f;
  private float m_HeightCurveLength;
  private float m_AnimTime;
  private AudioSource m_Sound;

  protected override void Awake()
  {
    this.m_Sound = this.GetComponent<AudioSource>();
    base.Awake();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    if (this.m_HeightCurve.length == 0)
    {
      Debug.LogWarning((object) "PolarityShift Spell height animation curve in not defined");
      base.OnAction(prevStateType);
    }
    else if (this.m_RotationDriftCurve.length == 0)
    {
      Debug.LogWarning((object) "PolarityShift Spell rotation drift animation curve in not defined");
      base.OnAction(prevStateType);
    }
    else
    {
      ++this.m_effectsPendingFinish;
      base.OnAction(prevStateType);
      this.m_HeightCurveLength = this.m_HeightCurve[this.m_HeightCurve.length - 1].time;
      this.m_ParticleEffects.m_ParticleSystems.Clear();
      List<PolarityShift.MinionData> minions = new List<PolarityShift.MinionData>();
      foreach (GameObject target in this.GetTargets())
      {
        PolarityShift.MinionData minionData = new PolarityShift.MinionData();
        minionData.gameObject = target;
        minionData.orgLocPos = target.transform.localPosition;
        minionData.orgLocRot = target.transform.localRotation;
        float x = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
        float y = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value) * 0.1f;
        float z = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
        minionData.rotationDrift = new Vector3(x, y, z);
        minionData.glowParticle = Object.Instantiate<ParticleSystem>(this.m_GlowParticle);
        minionData.glowParticle.transform.position = target.transform.position;
        minionData.glowParticle.transform.Translate(0.0f, this.m_ParticleHeightOffset, 0.0f, Space.World);
        minionData.lightningParticle = Object.Instantiate<ParticleSystem>(this.m_LightningParticle);
        minionData.lightningParticle.transform.position = target.transform.position;
        minionData.lightningParticle.transform.Translate(0.0f, this.m_ParticleHeightOffset, 0.0f, Space.World);
        minionData.impactParticle = Object.Instantiate<ParticleSystem>(this.m_ImpactParticle);
        minionData.impactParticle.transform.position = target.transform.position;
        minionData.impactParticle.transform.Translate(0.0f, this.m_ParticleHeightOffset, 0.0f, Space.World);
        this.m_ParticleEffects.m_ParticleSystems.Add(minionData.lightningParticle);
        if ((Object) this.m_Sound != (Object) null)
          SoundManager.Get().Play(this.m_Sound);
        minions.Add(minionData);
      }
      this.StartCoroutine(this.DoSpellFinished());
      this.StartCoroutine(this.MinionAnimation(minions));
    }
  }

  private IEnumerator DoSpellFinished()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    PolarityShift polarityShift = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      --polarityShift.m_effectsPendingFinish;
      polarityShift.FinishIfPossible();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(polarityShift.m_SpellFinishTime);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator MinionAnimation(List<PolarityShift.MinionData> minions)
  {
    PolarityShift polarityShift = this;
    foreach (PolarityShift.MinionData minion in minions)
      minion.glowParticle.Play();
    polarityShift.m_AnimTime = 0.0f;
    while ((double) polarityShift.m_AnimTime < (double) polarityShift.m_HeightCurveLength)
    {
      polarityShift.m_AnimTime += Time.deltaTime;
      float num1 = polarityShift.m_HeightCurve.Evaluate(polarityShift.m_AnimTime);
      float num2 = polarityShift.m_RotationDriftCurve.Evaluate(polarityShift.m_AnimTime);
      foreach (PolarityShift.MinionData minion in minions)
      {
        minion.gameObject.transform.localPosition = new Vector3(minion.orgLocPos.x, minion.orgLocPos.y + num1, minion.orgLocPos.z);
        minion.gameObject.transform.localRotation = minion.orgLocRot;
        minion.gameObject.transform.Rotate(minion.rotationDrift * num2, Space.Self);
      }
      yield return (object) null;
    }
    foreach (PolarityShift.MinionData minion in minions)
    {
      minion.impactParticle.Play();
      minion.lightningParticle.Play();
      MinionShake.ShakeObject(minion.gameObject, ShakeMinionType.RandomDirection, minion.gameObject.transform.position, ShakeMinionIntensity.MediumShake, 0.0f, 0.0f, 0.0f);
    }
    if (minions.Count > 0)
    {
      polarityShift.ShakeCamera();
      FullScreenEffects fsfx = FullScreenFXMgr.Get().ActiveCameraFullScreenEffects;
      if (!fsfx.IsActive)
      {
        fsfx.SetBlendToColorOverride(1f, Color.white);
        yield return (object) null;
        fsfx.SetBlendToColorOverride(0.67f, Color.white);
        yield return (object) null;
        fsfx.SetBlendToColorOverride(0.33f, Color.white);
        yield return (object) null;
        fsfx.SetBlendToColorOverride(0.0f, Color.white);
        fsfx.DisableBlendToColorOverride();
      }
      fsfx = (FullScreenEffects) null;
    }
    if (minions.Count > 0)
    {
      yield return (object) new WaitForSeconds(polarityShift.m_CleanupTime);
      polarityShift.m_ParticleEffects.m_ParticleSystems.Clear();
      foreach (PolarityShift.MinionData minion in minions)
      {
        Object.Destroy((Object) minion.glowParticle.gameObject);
        Object.Destroy((Object) minion.lightningParticle.gameObject);
        Object.Destroy((Object) minion.impactParticle.gameObject);
      }
    }
    polarityShift.OnStateFinished();
  }

  private void ShakeCamera() => CameraShakeMgr.Shake(Camera.main, new Vector3(0.1f, 0.1f, 0.1f), 0.75f);

  public class MinionData
  {
    public GameObject gameObject;
    public Vector3 orgLocPos;
    public Quaternion orgLocRot;
    public Vector3 rotationDrift;
    public ParticleSystem glowParticle;
    public ParticleSystem lightningParticle;
    public ParticleSystem impactParticle;
  }
}
