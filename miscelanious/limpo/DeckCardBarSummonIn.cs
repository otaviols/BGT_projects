using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class DeckCardBarSummonIn : SpellImpl
{
  public GameObject m_echoQuad;
  public GameObject m_fxEvaporate;

  private void OnDisable()
  {
    if ((Object) this.m_echoQuad != (Object) null)
      this.m_echoQuad.GetComponent<Renderer>().GetMaterial().color = Color.clear;
    if (!((Object) this.m_fxEvaporate != (Object) null))
      return;
    this.m_fxEvaporate.GetComponent<ParticleSystem>().Clear();
  }

  protected override void OnBirth(SpellStateType prevStateType) => this.StartCoroutine(this.BirthState());

  private IEnumerator BirthState()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DeckCardBarSummonIn deckCardBarSummonIn = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      deckCardBarSummonIn.OnSpellFinished();
      deckCardBarSummonIn.SetVisibility(deckCardBarSummonIn.m_echoQuad, false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    deckCardBarSummonIn.InitActorVariables();
    GameObject actorObject = deckCardBarSummonIn.GetActorObject("Frame");
    deckCardBarSummonIn.SetVisibilityRecursive(actorObject, false);
    deckCardBarSummonIn.SetVisibility(deckCardBarSummonIn.m_echoQuad, true);
    deckCardBarSummonIn.SetVisibilityRecursive(actorObject, true);
    deckCardBarSummonIn.PlayParticles(deckCardBarSummonIn.m_fxEvaporate, false);
    deckCardBarSummonIn.SetAnimationSpeed(deckCardBarSummonIn.m_echoQuad, "Secret_AbilityEchoFade", 0.5f);
    deckCardBarSummonIn.PlayAnimation(deckCardBarSummonIn.m_echoQuad, "Secret_AbilityEchoFade", PlayMode.StopAll);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(1f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
