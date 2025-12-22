using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (LineRenderer))]
[RequireComponent(typeof (UberCurve))]
public class NetherspiteBeamSpell : Spell
{
  [Range(1f, 1000f)]
  public int m_fullPathPolys = 50;
  [Range(1f, 1000f)]
  public int m_blockedPathPolys = 5;
  public bool m_targetMinionToRight = true;
  public List<Vector3> m_sourceCardOffsets;
  public List<Vector3> m_destCardOffsets;
  public List<Vector3> m_fullPathPoints;
  public bool m_visualizeControlPoints;
  public string m_beamFadeInMaterialVar = "";
  private int m_beamFadeInPropertyID;
  public float m_beamFadeInTime = 1f;
  public Spell m_beamSourceSpell;
  public Spell m_beamTargetMinionSpell;
  public Spell m_beamTargetHeroSpell;
  public ParticleSystem m_fullPathParticles;
  public ParticleSystem m_blockedPathParticles;
  private bool m_usingFullPath;
  private Actor m_targetActor;
  private Spell m_beamTargetSpellInstance;
  private Spell m_beamSourceSpellInstance;
  private UberCurve m_uberCurve;
  private LineRenderer m_lineRenderer;
  private Material m_beamMaterial;
  private List<GameObject> m_visualizers = new List<GameObject>();

  protected override void Awake()
  {
    base.Awake();
    this.m_uberCurve = this.GetComponent<UberCurve>();
    this.m_lineRenderer = this.GetComponent<LineRenderer>();
    this.m_beamMaterial = this.m_lineRenderer.GetMaterial();
    if (string.IsNullOrEmpty(this.m_beamFadeInMaterialVar))
      return;
    this.m_beamFadeInPropertyID = Shader.PropertyToID(this.m_beamFadeInMaterialVar);
  }

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    if ((Object) this.m_beamSourceSpell != (Object) null)
    {
      this.m_beamSourceSpellInstance = SpellManager.Get().GetSpell(this.m_beamSourceSpell);
      this.m_beamSourceSpellInstance.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished));
      this.m_beamSourceSpellInstance.transform.parent = this.GetSourceCard().GetActor().transform;
      TransformUtil.Identity((Component) this.m_beamSourceSpellInstance);
      this.m_beamSourceSpellInstance.Activate();
    }
    if ((Object) this.m_fullPathParticles != (Object) null)
    {
      this.m_fullPathParticles.transform.parent = this.GetSourceCard().GetActor().transform;
      TransformUtil.Identity((Component) this.m_fullPathParticles);
    }
    if ((Object) this.m_blockedPathParticles != (Object) null)
    {
      this.m_blockedPathParticles.transform.parent = this.GetSourceCard().GetActor().transform;
      TransformUtil.Identity((Component) this.m_blockedPathParticles);
    }
    this.StartCoroutine("DoUpdate");
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    if ((Object) this.m_beamSourceSpellInstance != (Object) null)
      this.m_beamSourceSpellInstance.ActivateState(SpellStateType.DEATH);
    if ((Object) this.m_fullPathParticles != (Object) null)
    {
      this.m_fullPathParticles.Stop();
      this.m_fullPathParticles.Clear();
    }
    if ((Object) this.m_blockedPathParticles != (Object) null)
    {
      this.m_blockedPathParticles.Stop();
      this.m_blockedPathParticles.Clear();
    }
    this.StopCoroutine("DoUpdate");
  }

  private IEnumerator DoUpdate()
  {
    NetherspiteBeamSpell netherspiteBeamSpell = this;
    while (true)
    {
      Actor minionToRight = netherspiteBeamSpell.GetTargetMinion();
      int num;
      if ((Object) minionToRight == (Object) null)
      {
        netherspiteBeamSpell.m_usingFullPath = true;
        minionToRight = SpellUtils.FindOpponentPlayer((Spell) netherspiteBeamSpell).GetHeroCard().GetActor();
        num = netherspiteBeamSpell.m_fullPathPolys;
        netherspiteBeamSpell.UpdateFullPathControlPoints();
      }
      else
      {
        netherspiteBeamSpell.m_usingFullPath = false;
        num = netherspiteBeamSpell.m_blockedPathPolys;
        netherspiteBeamSpell.UpdateBlockedPathControlPoints(minionToRight);
      }
      if ((Object) minionToRight != (Object) netherspiteBeamSpell.m_targetActor)
      {
        if ((Object) netherspiteBeamSpell.m_beamTargetSpellInstance != (Object) null)
          netherspiteBeamSpell.m_beamTargetSpellInstance.ActivateState(SpellStateType.DEATH);
        if (netherspiteBeamSpell.m_usingFullPath)
        {
          if (!string.IsNullOrEmpty(netherspiteBeamSpell.m_beamFadeInMaterialVar))
          {
            iTween.StopByName(netherspiteBeamSpell.gameObject, "fadeBeam");
            netherspiteBeamSpell.UpdateBeamFade(0.0f);
            Hashtable args = iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) netherspiteBeamSpell.m_beamFadeInTime, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) "UpdateBeamFade", (object) "onupdatetarget", (object) netherspiteBeamSpell.gameObject, (object) "name", (object) "fadeBeam");
            iTween.ValueTo(netherspiteBeamSpell.gameObject, args);
          }
          if ((Object) netherspiteBeamSpell.m_fullPathParticles != (Object) null)
            netherspiteBeamSpell.m_fullPathParticles.Play();
          if ((Object) netherspiteBeamSpell.m_blockedPathParticles != (Object) null)
          {
            netherspiteBeamSpell.m_blockedPathParticles.Stop();
            netherspiteBeamSpell.m_blockedPathParticles.Clear();
          }
        }
        else
        {
          if ((Object) netherspiteBeamSpell.m_fullPathParticles != (Object) null)
          {
            netherspiteBeamSpell.m_fullPathParticles.Stop();
            netherspiteBeamSpell.m_fullPathParticles.Clear();
          }
          if ((Object) netherspiteBeamSpell.m_blockedPathParticles != (Object) null)
            netherspiteBeamSpell.m_blockedPathParticles.Play();
        }
        netherspiteBeamSpell.m_targetActor = minionToRight;
        if ((Object) netherspiteBeamSpell.m_targetActor != (Object) null)
        {
          Spell spell = netherspiteBeamSpell.m_targetActor.GetEntity().GetCardType() == TAG_CARDTYPE.HERO ? netherspiteBeamSpell.m_beamTargetHeroSpell : netherspiteBeamSpell.m_beamTargetMinionSpell;
          if ((Object) spell != (Object) null)
          {
            netherspiteBeamSpell.m_beamTargetSpellInstance = SpellManager.Get().GetSpell(spell);
            netherspiteBeamSpell.m_beamTargetSpellInstance.AddStateFinishedCallback(new Spell.StateFinishedCallback(netherspiteBeamSpell.OnSpellStateFinished));
            netherspiteBeamSpell.m_beamTargetSpellInstance.transform.parent = netherspiteBeamSpell.m_targetActor.transform;
            TransformUtil.Identity((Component) netherspiteBeamSpell.m_beamTargetSpellInstance);
            netherspiteBeamSpell.m_beamTargetSpellInstance.Activate();
          }
          else
            netherspiteBeamSpell.m_beamTargetSpellInstance = (Spell) null;
        }
      }
      netherspiteBeamSpell.m_lineRenderer.positionCount = num;
      for (int index = 0; index < num; ++index)
      {
        float position = (float) index / (float) num;
        netherspiteBeamSpell.m_lineRenderer.SetPosition(index, netherspiteBeamSpell.m_uberCurve.CatmullRomEvaluateWorldPosition(position));
      }
      netherspiteBeamSpell.VisualizeControlPoints();
      yield return (object) null;
    }
  }

  private void UpdateBeamFade(float fadeValue) => this.m_beamMaterial.SetColor(this.m_beamFadeInPropertyID, this.m_beamMaterial.GetColor(this.m_beamFadeInPropertyID) with
  {
    a = fadeValue
  });

  private void UpdateBlockedPathControlPoints(Actor minionToRight)
  {
    int num = this.m_sourceCardOffsets.Count + this.m_destCardOffsets.Count;
    if (this.m_uberCurve.m_controlPoints.Count != num)
    {
      this.m_uberCurve.m_controlPoints.Clear();
      for (int index = 0; index < num; ++index)
        this.m_uberCurve.m_controlPoints.Add(new UberCurve.UberCurveControlPoint());
    }
    int index1 = 0;
    Card sourceCard = this.GetSourceCard();
    int index2 = 0;
    while (index2 < this.m_sourceCardOffsets.Count)
    {
      this.m_uberCurve.m_controlPoints[index1].position = sourceCard.transform.position + this.m_sourceCardOffsets[index2];
      ++index2;
      ++index1;
    }
    int index3 = 0;
    while (index3 < this.m_destCardOffsets.Count)
    {
      this.m_uberCurve.m_controlPoints[index1].position = minionToRight.transform.position + this.m_destCardOffsets[index3];
      ++index3;
      ++index1;
    }
  }

  private void UpdateFullPathControlPoints()
  {
    int num = this.m_sourceCardOffsets.Count + this.m_fullPathPoints.Count;
    if (this.m_uberCurve.m_controlPoints.Count != num)
    {
      this.m_uberCurve.m_controlPoints.Clear();
      for (int index = 0; index < num; ++index)
        this.m_uberCurve.m_controlPoints.Add(new UberCurve.UberCurveControlPoint());
    }
    int index1 = 0;
    Card sourceCard = this.GetSourceCard();
    int index2 = 0;
    while (index2 < this.m_sourceCardOffsets.Count)
    {
      this.m_uberCurve.m_controlPoints[index1].position = sourceCard.transform.position + this.m_sourceCardOffsets[index2];
      ++index2;
      ++index1;
    }
    int index3 = 0;
    while (index3 < this.m_fullPathPoints.Count)
    {
      this.m_uberCurve.m_controlPoints[index1].position = this.m_fullPathPoints[index3];
      ++index3;
      ++index1;
    }
  }

  private Actor GetTargetMinion()
  {
    int zonePosition = this.GetSourceCard().GetZonePosition();
    ZonePlay battlefieldZone = this.GetSourceCard().GetController().GetBattlefieldZone();
    for (int slot = this.m_targetMinionToRight ? zonePosition + 1 : zonePosition - 1; slot > 0 && slot <= battlefieldZone.GetCardCount(); slot += this.m_targetMinionToRight ? 1 : -1)
    {
      Card cardAtSlot = battlefieldZone.GetCardAtSlot(slot);
      if (cardAtSlot.IsActorReady())
        return cardAtSlot.GetActor();
    }
    return (Actor) null;
  }

  private void VisualizeControlPoints()
  {
    if (!this.m_visualizeControlPoints)
    {
      foreach (Object visualizer in this.m_visualizers)
        Object.Destroy(visualizer);
      this.m_visualizers.Clear();
    }
    else if (this.m_visualizers.Count != this.m_uberCurve.m_controlPoints.Count)
    {
      foreach (Object visualizer in this.m_visualizers)
        Object.Destroy(visualizer);
      this.m_visualizers.Clear();
      for (int index = 0; index < this.m_uberCurve.m_controlPoints.Count; ++index)
      {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        primitive.transform.position = this.m_uberCurve.m_controlPoints[index].position;
        primitive.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        this.m_visualizers.Add(primitive);
      }
    }
    else
    {
      for (int index = 0; index < this.m_uberCurve.m_controlPoints.Count; ++index)
        this.m_visualizers[index].transform.position = this.m_uberCurve.transform.TransformPoint(this.m_uberCurve.m_controlPoints[index].position);
    }
  }

  private void OnSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    SpellManager.Get().ReleaseSpell(spell);
  }
}
