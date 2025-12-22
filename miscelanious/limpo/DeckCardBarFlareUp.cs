using System.Collections;
using UnityEngine;

public class DeckCardBarFlareUp : SpellImpl
{
  public GameObject m_fuseQuad;
  public GameObject m_fxSparks;

  protected override void OnBirth(SpellStateType prevStateType)
  {
    if (!this.gameObject.activeSelf)
      return;
    this.StartCoroutine(this.BirthState());
  }

  private IEnumerator BirthState()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DeckCardBarFlareUp deckCardBarFlareUp = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      deckCardBarFlareUp.SetVisibility(deckCardBarFlareUp.m_fuseQuad, false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    deckCardBarFlareUp.SetVisibility(deckCardBarFlareUp.m_fuseQuad, true);
    deckCardBarFlareUp.PlayParticles(deckCardBarFlareUp.m_fxSparks, false);
    deckCardBarFlareUp.PlayAnimation(deckCardBarFlareUp.m_fuseQuad, "DeckCardBar_FuseInOut", PlayMode.StopAll);
    deckCardBarFlareUp.OnSpellFinished();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(2f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
