using Blizzard.T5.Core.Utils;
using System.Collections;
using UnityEngine;

public class CardBurn : Spell
{
  public GameObject m_BurnCardQuad;
  public string m_BurnCardAnim = "CardBurnUpFire";
  public ParticleSystem m_EdgeEmbers;

  protected override void OnBirth(SpellStateType prevStateType) => this.StartCoroutine(this.BirthAction());

  private IEnumerator BirthAction()
  {
    CardBurn cardBurn = this;
    if ((bool) (Object) cardBurn.m_BurnCardQuad)
    {
      cardBurn.m_BurnCardQuad.GetComponent<Renderer>().enabled = true;
      cardBurn.m_BurnCardQuad.GetComponent<Animation>().Play(cardBurn.m_BurnCardAnim, PlayMode.StopAll);
    }
    if ((bool) (Object) cardBurn.m_EdgeEmbers)
      cardBurn.m_EdgeEmbers.Play();
    yield return (object) new WaitForSeconds(0.15f);
    Actor componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<Actor>(cardBurn.gameObject);
    if (!((Object) componentInThisOrParents == (Object) null))
    {
      componentInThisOrParents.Hide();
      cardBurn.OnSpellFinished();
    }
  }
}
