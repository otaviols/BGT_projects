using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBlade : SuperSpell
{
  private const float DAMAGE_SPLAT_DELAY = 0.0f;
  private const float BLADE_BIRTH_TIME = 0.3f;
  private const int OFFSCREEN_HIT_PERCENT = 5;
  public GameObject m_BladeRoot;
  public GameObject m_Blade;
  public GameObject m_Trail;
  public GameObject m_HitBonesRoot;
  public List<ParticleSystem> m_SparkParticles;
  public ParticleSystem m_EndSparkParticles;
  public ParticleSystem m_EndBigSparkParticles;
  public List<BouncingBlade.HitBonesType> m_HitBones;
  public AudioSource m_BladeSpinning;
  public AudioSource m_BladeSpinningContinuous;
  public AudioSource m_BladeHitMinion;
  public AudioSource m_BladeHitBoardCorner;
  public AudioSource m_BladeHitOffScreen;
  public AudioSource m_StartSound;
  public AudioSource m_EndSound;
  public float m_BladeAnimationSpeed = 50f;
  public float m_BladeSpinningMinVol;
  public float m_BladeSpinningMaxVol = 1f;
  public float m_BladeSpinningRampTime = 0.3f;
  private bool m_Running;
  private List<BouncingBlade.Target> m_TargetQueue = new List<BouncingBlade.Target>();
  private Vector3? m_NextPosition;
  private bool m_Animating;
  private bool m_isDone;
  private BouncingBlade.HitBonesType m_PreviousHitBone;
  private Vector3 m_OrgBladeScale;

  protected override void Awake()
  {
    base.Awake();
    this.m_BladeRoot.SetActive(false);
    this.m_PreviousHitBone = this.m_HitBones[this.m_HitBones.Count - 1];
    this.m_OrgBladeScale = this.m_BladeRoot.transform.localScale;
    this.m_BladeRoot.transform.localScale = Vector3.zero;
  }

  protected override void Start()
  {
    base.Start();
    this.SetupBounceLocations();
  }

  public override bool ShouldReconnectIfStuck() => false;

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    if (this.m_targets.Count == 0)
    {
      this.m_isDone = true;
      this.m_BladeRoot.SetActive(false);
      --this.m_effectsPendingFinish;
      this.FinishIfPossible();
    }
    else
    {
      if (!this.m_Running)
      {
        this.m_BladeRoot.SetActive(true);
        this.m_Blade.SetActive(false);
        this.m_Trail.SetActive(false);
        this.m_Running = true;
        this.StartCoroutine(this.BladeRunner());
      }
      this.m_BladeRoot.transform.localScale = this.m_OrgBladeScale;
      this.m_isDone = false;
      bool flag = this.IsHandlingLastTaskList();
      for (int index = 0; index < this.m_targets.Count; ++index)
      {
        GameObject target1 = this.m_targets[index];
        int dataIndexForTarget = this.GetMetaDataIndexForTarget(index);
        BouncingBlade.Target target2 = new BouncingBlade.Target();
        target2.VisualTarget = target1;
        target2.TargetPosition = target1.transform.position;
        target2.MetaDataIdx = dataIndexForTarget;
        target2.isMinion = true;
        if (index == this.m_targets.Count - 1)
          target2.LastTarget = true;
        if (flag)
          target2.LastBlock = true;
        this.m_TargetQueue.Add(target2);
        if (!target2.LastTarget)
        {
          BouncingBlade.Target target3 = new BouncingBlade.Target();
          target3.TargetPosition = this.AcquireRandomBoardTarget(out target3.Offscreen);
          target3.isMinion = false;
          target3.LastTarget = false;
          this.m_TargetQueue.Add(target3);
        }
      }
    }
  }

  private IEnumerator BladeRunner()
  {
    BouncingBlade bouncingBlade = this;
    while (!bouncingBlade.m_isDone)
    {
      while (bouncingBlade.m_TargetQueue.Count > 0)
      {
        if (!bouncingBlade.m_Blade.activeSelf)
        {
          bouncingBlade.m_Blade.SetActive(true);
          if ((UnityEngine.Object) bouncingBlade.m_BladeSpinning != (UnityEngine.Object) null)
          {
            bouncingBlade.m_BladeSpinning.gameObject.SetActive(true);
            SoundManager.Get().Play(bouncingBlade.m_BladeSpinning);
          }
          if ((UnityEngine.Object) bouncingBlade.m_BladeSpinningContinuous != (UnityEngine.Object) null)
          {
            bouncingBlade.m_BladeSpinningContinuous.gameObject.SetActive(true);
            SoundManager.Get().Play(bouncingBlade.m_BladeSpinningContinuous);
          }
          if ((UnityEngine.Object) bouncingBlade.m_StartSound != (UnityEngine.Object) null)
            SoundManager.Get().Play(bouncingBlade.m_StartSound);
        }
        if (!bouncingBlade.m_Trail.activeSelf)
          bouncingBlade.m_Trail.SetActive(true);
        BouncingBlade.Target target = bouncingBlade.m_TargetQueue[0];
        if (target.isMinion)
        {
          int metaDataIdx = target.MetaDataIdx;
          yield return (object) bouncingBlade.StartCoroutine(bouncingBlade.CompleteTasksUntilMetaData(metaDataIdx));
          bouncingBlade.AnimateToNextTarget(target);
          while (bouncingBlade.m_Animating)
            yield return (object) null;
          if (metaDataIdx > 0)
            yield return (object) bouncingBlade.StartCoroutine(bouncingBlade.CompleteTasksFromMetaData(metaDataIdx, 0.0f));
          if (target.LastBlock && target.LastTarget)
          {
            bouncingBlade.m_EndSparkParticles.Play();
            bouncingBlade.m_EndBigSparkParticles.Play();
            bouncingBlade.m_Blade.SetActive(false);
            if ((UnityEngine.Object) bouncingBlade.m_BladeSpinning != (UnityEngine.Object) null)
              SoundManager.Get().Stop(bouncingBlade.m_BladeSpinning);
            if ((UnityEngine.Object) bouncingBlade.m_BladeSpinningContinuous != (UnityEngine.Object) null)
              SoundManager.Get().Stop(bouncingBlade.m_BladeSpinningContinuous);
            if ((UnityEngine.Object) bouncingBlade.m_EndSound != (UnityEngine.Object) null)
              SoundManager.Get().Play(bouncingBlade.m_EndSound);
            yield return (object) new WaitForSeconds(0.8f);
            --bouncingBlade.m_effectsPendingFinish;
            bouncingBlade.FinishIfPossible();
            bouncingBlade.m_BladeRoot.SetActive(true);
            bouncingBlade.m_Running = false;
            bouncingBlade.m_TargetQueue.Clear();
            yield break;
          }
          else if (!target.LastBlock && target.LastTarget)
          {
            --bouncingBlade.m_effectsPendingFinish;
            bouncingBlade.FinishIfPossible();
          }
        }
        else
        {
          bouncingBlade.AnimateToNextTarget(target);
          while (bouncingBlade.m_Animating)
            yield return (object) null;
        }
        bouncingBlade.m_TargetQueue.RemoveAt(0);
        yield return (object) null;
        target = (BouncingBlade.Target) null;
      }
      BouncingBlade.Target target1 = new BouncingBlade.Target();
      target1.TargetPosition = bouncingBlade.AcquireRandomBoardTarget(out target1.Offscreen);
      target1.isMinion = false;
      target1.LastTarget = false;
      bouncingBlade.AnimateToNextTarget(target1);
      while (bouncingBlade.m_Animating)
        yield return (object) null;
    }
  }

  private void SetupBounceLocations()
  {
    Vector3 position = Board.Get().FindBone("CenterPointBone").transform.position;
    Vector3 localPosition = this.m_HitBonesRoot.transform.localPosition;
    this.m_HitBonesRoot.transform.position = position;
    foreach (BouncingBlade.HitBonesType hitBone in this.m_HitBones)
      hitBone.SetPosition(hitBone.Bone.transform.position);
    this.m_HitBonesRoot.transform.localPosition = localPosition;
  }

  private void AnimateToNextTarget(BouncingBlade.Target target)
  {
    this.m_Animating = true;
    iTween.MoveTo(this.m_BladeRoot, iTween.Hash((object) "position", (object) target.TargetPosition, (object) "speed", (object) this.m_BladeAnimationSpeed, (object) "orienttopath", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "AnimationComplete", (object) "oncompleteparams", (object) target));
  }

  private void RampBladeVolume()
  {
    iTween.StopByName(this.m_BladeSpinning.gameObject, "BladeSpinningSound");
    SoundManager.Get().SetVolume(this.m_BladeSpinning, this.m_BladeSpinningMinVol);
    iTween.ValueTo(this.m_BladeSpinning.gameObject, iTween.Hash((object) "name", (object) "BladeSpinningSound", (object) "from", (object) this.m_BladeSpinningMinVol, (object) "to", (object) this.m_BladeSpinningMaxVol, (object) "time", (object) this.m_BladeSpinningRampTime, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) (Action<object>) (amount => SoundManager.Get().SetVolume(this.m_BladeSpinning, (float) amount)), (object) "onupdatetarget", (object) this.m_BladeSpinning.gameObject));
  }

  private void AnimationComplete(BouncingBlade.Target target)
  {
    this.m_Animating = false;
    this.AnimateSparks();
    if (!target.LastBlock && !target.LastTarget)
      this.RampBladeVolume();
    AudioSource source = !target.isMinion ? (!target.Offscreen ? this.m_BladeHitBoardCorner : this.m_BladeHitOffScreen) : this.m_BladeHitMinion;
    if (!((UnityEngine.Object) source != (UnityEngine.Object) null))
      return;
    source.gameObject.transform.position = target.TargetPosition;
    SoundManager.Get().Play(source);
  }

  private void AnimateSparks()
  {
    foreach (ParticleSystem sparkParticle in this.m_SparkParticles)
      sparkParticle.Play();
  }

  private Vector3 AcquireRandomBoardTarget(out bool offscreen)
  {
    offscreen = false;
    if (UnityEngine.Random.Range(1, 100) < 5)
      offscreen = true;
    List<BouncingBlade.HitBonesType> hitBonesTypeList = new List<BouncingBlade.HitBonesType>();
    if (offscreen)
    {
      foreach (BouncingBlade.HitBonesType hitBone in this.m_HitBones)
      {
        if (hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.E && hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.NE && hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.NW && hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.SE && hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.SW && hitBone.Direction != this.m_PreviousHitBone.Direction)
          hitBonesTypeList.Add(hitBone);
      }
    }
    else
    {
      foreach (BouncingBlade.HitBonesType hitBone in this.m_HitBones)
      {
        if (hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.E_OFFSCREEN && hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.N_OFFSCREEN && hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.S_OFFSCREEN && hitBone.Direction != BouncingBlade.HIT_DIRECTIONS.W_OFFSCREEN && hitBone.Direction != this.m_PreviousHitBone.Direction)
          hitBonesTypeList.Add(hitBone);
      }
    }
    int index = UnityEngine.Random.Range(0, hitBonesTypeList.Count);
    this.m_PreviousHitBone = hitBonesTypeList[index];
    return hitBonesTypeList[index].GetPosition();
  }

  public enum HIT_DIRECTIONS
  {
    NW,
    NE,
    E,
    SW,
    SE,
    N_OFFSCREEN,
    E_OFFSCREEN,
    W_OFFSCREEN,
    S_OFFSCREEN,
  }

  [Serializable]
  public class HitBonesType
  {
    public BouncingBlade.HIT_DIRECTIONS Direction;
    public GameObject Bone;
    private Vector3 m_Position;

    public void SetPosition(Vector3 pos) => this.m_Position = pos;

    public Vector3 GetPosition() => this.m_Position;
  }

  [Serializable]
  public class Target
  {
    public GameObject VisualTarget;
    public Vector3 TargetPosition;
    public bool isMinion;
    public int MetaDataIdx;
    public bool LastTarget;
    public bool LastBlock;
    public bool Offscreen;
  }
}
