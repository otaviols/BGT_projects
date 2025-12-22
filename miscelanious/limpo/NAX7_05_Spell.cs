using System.Collections;
using UnityEngine;

public class NAX7_05_Spell : Spell
{
  protected override void OnBirth(SpellStateType prevStateType) => this.StartCoroutine(this.SpellEffect(prevStateType));

  private IEnumerator SpellEffect(SpellStateType prevStateType)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    NAX7_05_Spell naX705Spell = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    PlayMakerFSM component = Board.Get().transform.Find("Board_NAX").Find("NAX_Crystal_Skinned").GetComponent<PlayMakerFSM>();
    if ((Object) component == (Object) null)
    {
      Debug.LogWarning((object) "NAX7_05_Spell unable to get playmaker fsm");
      return false;
    }
    component.SendEvent("ClickTop");
    // ISSUE: reference to a compiler-generated method
    naX705Spell.\u003C\u003En__0(prevStateType);
    naX705Spell.OnSpellFinished();
    return false;
  }
}
