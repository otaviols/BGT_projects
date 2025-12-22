using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCartRushArt : MonoBehaviour
{
  public List<Texture2D> m_portraits = new List<Texture2D>();
  public Spell m_portraitSwapSpell;
  public float m_portraitSwapDelay = 0.5f;

  public void DoPortraitSwap(Actor actor) => this.StartCoroutine(this.DoPortraitSwapWithTiming(actor));

  private IEnumerator DoPortraitSwapWithTiming(Actor actor)
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
      actor.SetPortraitTextureOverride((Texture) this.GetNextPortrait());
    }
  }

  private Texture2D GetNextPortrait()
  {
    if (this.m_portraits.Count == 0)
      return (Texture2D) null;
    if (this.m_portraits.Count == 1)
      return this.m_portraits[0];
    Texture2D portrait = this.m_portraits[0];
    int index = Random.Range(1, this.m_portraits.Count);
    this.m_portraits[0] = this.m_portraits[index];
    this.m_portraits[index] = portrait;
    return this.m_portraits[0];
  }
}
