using System.Collections;
using UnityEngine;

public class SummonInDungeonCrawl : SpellImpl
{
  public GameObject m_burnIn;
  public GameObject m_blackBits;
  public GameObject m_smokePuff;
  public float m_burnInAnimationSpeed = 1f;
  public bool m_isHeroActor;

  protected override void OnBirth(SpellStateType prevStateType) => this.StartCoroutine(this.BirthState());

  private IEnumerator BirthState()
  {
    SummonInDungeonCrawl summonInDungeonCrawl = this;
    summonInDungeonCrawl.InitActorVariables();
    summonInDungeonCrawl.SetVisibility(summonInDungeonCrawl.m_burnIn, true);
    summonInDungeonCrawl.SetAnimationSpeed(summonInDungeonCrawl.m_burnIn, "AllyInHandScryLines_Forge", summonInDungeonCrawl.m_burnInAnimationSpeed);
    summonInDungeonCrawl.PlayAnimation(summonInDungeonCrawl.m_burnIn, "AllyInHandScryLines_Forge", PlayMode.StopAll);
    summonInDungeonCrawl.PlayParticles(summonInDungeonCrawl.m_smokePuff, false);
    summonInDungeonCrawl.PlayParticles(summonInDungeonCrawl.m_blackBits, false);
    yield return (object) new WaitForSeconds(0.2f);
    summonInDungeonCrawl.SetVisibility(summonInDungeonCrawl.m_burnIn, true);
    Renderer renderer1 = (Object) summonInDungeonCrawl.m_smokePuff != (Object) null ? summonInDungeonCrawl.m_smokePuff.GetComponent<Renderer>() : (Renderer) null;
    if ((Object) renderer1 != (Object) null)
      renderer1.enabled = true;
    Renderer renderer2 = (Object) summonInDungeonCrawl.m_blackBits != (Object) null ? summonInDungeonCrawl.m_blackBits.GetComponent<Renderer>() : (Renderer) null;
    if ((Object) renderer2 != (Object) null)
      renderer2.enabled = true;
    if (summonInDungeonCrawl.m_isHeroActor)
    {
      GameObject actorObject1 = summonInDungeonCrawl.GetActorObject("AttackObject");
      GameObject actorObject2 = summonInDungeonCrawl.GetActorObject("HealthObject");
      summonInDungeonCrawl.SetVisibilityRecursive(actorObject1, false);
      summonInDungeonCrawl.SetVisibilityRecursive(actorObject2, false);
    }
    yield return (object) new WaitForSeconds(0.2f);
    summonInDungeonCrawl.OnSpellFinished();
  }
}
