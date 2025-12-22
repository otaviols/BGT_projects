using System.Collections;
using UnityEngine;

[RequireComponent(typeof (LineRenderer))]
public class MagneticBeamSpell : Spell
{
  private LineRenderer m_lineRenderer;

  protected override void Awake()
  {
    base.Awake();
    this.m_lineRenderer = this.GetComponent<LineRenderer>();
    this.m_lineRenderer.enabled = false;
  }

  protected override void OnBirth(SpellStateType prevStateType)
  {
    base.OnBirth(prevStateType);
    this.m_lineRenderer.enabled = true;
    this.StartCoroutine(this.DoUpdate());
  }

  private IEnumerator DoUpdate()
  {
    MagneticBeamSpell magneticBeamSpell = this;
    while (true)
    {
      magneticBeamSpell.m_lineRenderer.SetPosition(0, magneticBeamSpell.m_source.transform.position);
      magneticBeamSpell.m_lineRenderer.SetPosition(1, magneticBeamSpell.m_targets[0].transform.position);
      yield return (object) null;
    }
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    this.m_lineRenderer.enabled = false;
    this.StopAllCoroutines();
  }
}
