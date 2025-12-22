using UnityEngine;

public class Spell_SuppressPlayAudio : Spell
{
  public override void SetSource(GameObject go)
  {
    this.m_source = go;
    if ((Object) this.m_source == (Object) null)
      return;
    Card component = this.m_source.GetComponent<Card>();
    if (!((Object) component != (Object) null))
      return;
    component.SuppressPlaySounds(true);
  }
}
