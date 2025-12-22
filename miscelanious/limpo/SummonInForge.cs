using System.Collections;
using UnityEngine;

public class SummonInForge : SpellImpl
{
  public GameObject m_burnIn;
  public GameObject m_blackBits;
  public GameObject m_smokePuff;
  public float m_burnInAnimationSpeed = 1f;
  public bool m_isHeroActor;
  public static string ACTOR_VISIBLE_EVENT = "ActorVisible";

  protected override void OnBirth(SpellStateType prevStateType) => this.StartCoroutine(this.BirthState());

  private IEnumerator BirthState()
  {
    SummonInForge summonInForge = this;
    summonInForge.InitActorVariables();
    summonInForge.SetActorVisibility(false, true);
    summonInForge.SetVisibility(summonInForge.m_burnIn, true);
    summonInForge.SetAnimationSpeed(summonInForge.m_burnIn, "AllyInHandScryLines_Forge", summonInForge.m_burnInAnimationSpeed);
    summonInForge.PlayAnimation(summonInForge.m_burnIn, "AllyInHandScryLines_Forge", PlayMode.StopAll);
    summonInForge.PlayParticles(summonInForge.m_smokePuff, false);
    summonInForge.PlayParticles(summonInForge.m_blackBits, false);
    yield return (object) new WaitForSeconds(0.2f);
    summonInForge.SetVisibility(summonInForge.m_burnIn, true);
    Renderer renderer1 = (Object) summonInForge.m_smokePuff != (Object) null ? summonInForge.m_smokePuff.GetComponent<Renderer>() : (Renderer) null;
    if ((Object) renderer1 != (Object) null)
      renderer1.enabled = true;
    Renderer renderer2 = (Object) summonInForge.m_blackBits != (Object) null ? summonInForge.m_blackBits.GetComponent<Renderer>() : (Renderer) null;
    if ((Object) renderer2 != (Object) null)
      renderer2.enabled = true;
    summonInForge.SetActorVisibility(true, true);
    summonInForge.OnSpellEvent(SummonInForge.ACTOR_VISIBLE_EVENT, (object) null);
    if (summonInForge.m_isHeroActor)
    {
      GameObject actorObject1 = summonInForge.GetActorObject("AttackObject");
      GameObject actorObject2 = summonInForge.GetActorObject("HealthObject");
      summonInForge.SetVisibilityRecursive(actorObject1, false);
      summonInForge.SetVisibilityRecursive(actorObject2, false);
    }
    yield return (object) new WaitForSeconds(0.2f);
    summonInForge.OnSpellFinished();
  }
}
