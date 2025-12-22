using System.Collections.Generic;
using UnityEngine;

public class CardEffect
{
  private Spell m_spell;
  private List<CardSoundSpell> m_soundSpells;
  private string m_spellPath;
  private List<string> m_soundSpellPaths;
  private Card m_owner;

  public CardEffect(CardEffectDef def, Card owner)
  {
    this.m_spellPath = def.m_SpellPath;
    this.m_soundSpellPaths = def.m_SoundSpellPaths;
    this.m_owner = owner;
    if (this.m_soundSpellPaths == null)
      return;
    this.m_soundSpells = new List<CardSoundSpell>(this.m_soundSpellPaths.Count);
    for (int index = 0; index < this.m_soundSpellPaths.Count; ++index)
      this.m_soundSpells.Add((CardSoundSpell) null);
  }

  public CardEffect(string spellPath, Card owner)
  {
    this.m_spellPath = spellPath;
    this.m_owner = owner;
  }

  public Spell GetSpell(bool loadIfNeeded = true)
  {
    if (((!((Object) this.m_spell == (Object) null) ? 0 : (!string.IsNullOrEmpty(this.m_spellPath) ? 1 : 0)) & (loadIfNeeded ? 1 : 0)) != 0)
      this.m_spell = SpellUtils.LoadAndSetupSpell(this.m_spellPath, (Component) this.m_owner);
    return this.m_spell;
  }

  public Spell LoadSpell()
  {
    if (!string.IsNullOrEmpty(this.m_spellPath))
      this.m_spell = SpellUtils.LoadAndSetupSpell(this.m_spellPath, (Component) this.m_owner);
    return this.m_spell;
  }

  public void LoadSoundSpell(int index)
  {
    if (index < 0 || this.m_soundSpellPaths == null || index >= this.m_soundSpellPaths.Count || string.IsNullOrEmpty(this.m_soundSpellPaths[index]) || !((Object) this.m_soundSpells[index] == (Object) null))
      return;
    string soundSpellPath = this.m_soundSpellPaths[index];
    CardSoundSpell spell = SpellManager.Get().GetSpell(soundSpellPath) as CardSoundSpell;
    this.m_soundSpells[index] = spell;
    if ((Object) spell == (Object) null)
    {
      Error.AddDevFatal("CardEffect.LoadSoundSpell() - FAILED TO LOAD \"{0}\" (PATH: \"{1}\") (index {2})", (object) this.m_spellPath, (object) soundSpellPath, (object) index);
    }
    else
    {
      if (!((Object) this.m_owner != (Object) null))
        return;
      SpellUtils.SetupSoundSpell(spell, (Component) this.m_owner);
    }
  }

  public List<CardSoundSpell> GetSoundSpells(bool loadIfNeeded = true)
  {
    if (this.m_soundSpells == null)
      return (List<CardSoundSpell>) null;
    if (loadIfNeeded)
    {
      for (int index = 0; index < this.m_soundSpells.Count; ++index)
        this.LoadSoundSpell(index);
    }
    return this.m_soundSpells;
  }

  public void Clear()
  {
    SpellManager spellManager = SpellManager.Get();
    if (spellManager == null)
      return;
    if ((Object) this.m_spell != (Object) null)
      spellManager.ReleaseSpell(this.m_spell);
    if (this.m_soundSpells == null)
      return;
    for (int index = 0; index < this.m_soundSpells.Count; ++index)
    {
      Spell soundSpell = (Spell) this.m_soundSpells[index];
      if ((Object) soundSpell != (Object) null)
        spellManager.ReleaseSpell(soundSpell);
    }
  }

  public void LoadAll()
  {
    this.GetSpell();
    if (this.m_soundSpellPaths == null)
      return;
    for (int index = 0; index < this.m_soundSpellPaths.Count; ++index)
      this.LoadSoundSpell(index);
  }

  public void PurgeSpells()
  {
    SpellUtils.PurgeSpell(this.m_spell);
    SpellUtils.PurgeSpells<CardSoundSpell>(this.m_soundSpells);
  }
}
