using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleArt : MonoBehaviour
{
  public List<Texture2D> m_portraits;
  public Spell m_portraitSwapSpell;
  public float m_portraitSwapDelay = 0.5f;

  public void DoPortraitSwap(Actor actor, int turn) => this.StartCoroutine(this.DoPortraitSwapWithTiming(actor, turn));

  private IEnumerator DoPortraitSwapWithTiming(Actor actor, int turn)
  {
    if (!((Object) actor == (Object) null))
    {
      if ((Object) this.m_portraitSwapSpell != (Object) null)
      {
        SpellManager spellManager = SpellManager.Get();
        Spell spell1 = spellManager.GetSpell(this.m_portraitSwapSpell);
        spell1.transform.parent = actor.transform;
        spell1.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
        {
          if (spell.GetActiveState() != SpellStateType.NONE)
            return;
          spellManager.ReleaseSpell(spell);
        }));
        spell1.SetSource(actor.gameObject);
        spell1.Activate();
        yield return (object) new WaitForSeconds(this.m_portraitSwapDelay);
      }
      actor.SetPortraitTextureOverride((Texture) this.GetArtForTurn(turn));
    }
  }

  private Texture2D GetArtForTurn(int turn) => this.m_portraits[turn];
}
