using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class SpellTable : MonoBehaviour
{
  public List<SpellTableEntry> m_Table = new List<SpellTableEntry>();
  private Dictionary<SpellType, SpellTableEntry> m_Entries;

  public bool TryGetEntry(SpellType type, out SpellTableEntry entry)
  {
    if (this.m_Entries == null)
      this.IntialzieSpellTable();
    return this.m_Entries.TryGetValue(type, out entry);
  }

  private void IntialzieSpellTable()
  {
    this.m_Entries = new Dictionary<SpellType, SpellTableEntry>();
    foreach (SpellTableEntry spellTableEntry in this.m_Table)
    {
      if (this.m_Entries.ContainsKey(spellTableEntry.m_Type))
        Error.AddDevWarning("Spell Table", "Spell Table: {0} Entry List contains more than one entry for spell type {1}, please remove the duplicate.", (object) this.name, (object) spellTableEntry.m_Type);
      else
        this.m_Entries.Add(spellTableEntry.m_Type, spellTableEntry);
    }
  }

  private Spell GetSpell(SpellType spellType, bool isLocal = false)
  {
    SpellTableEntry entry;
    if (!this.TryGetEntry(spellType, out entry))
      return (Spell) null;
    if (isLocal && (Object) entry.m_Spell != (Object) null)
      return entry.m_Spell;
    if (string.IsNullOrEmpty(entry.m_SpellPrefabName))
    {
      Error.AddDevWarning("Spell Table", "The Spell Prefab Name for {0} is empty.", (object) entry.m_Type);
      return (Spell) null;
    }
    string spellPrefabName = entry.m_SpellPrefabName;
    Spell spell = SpellManager.Get().GetSpell(spellPrefabName, true);
    spell.SetSpellType(spellType);
    if (isLocal)
    {
      entry.m_Spell = spell;
      TransformUtil.AttachAndPreserveLocalTransform(spell.gameObject.transform, this.gameObject.transform);
    }
    return spell;
  }

  public Spell GetSpellInstance(SpellType spellType) => this.GetSpell(spellType);

  public Spell GetLocalSpell(SpellType spellType) => this.GetSpell(spellType, true);

  public void ReleaseAllSpells()
  {
    foreach (SpellTableEntry spellTableEntry in this.m_Table)
    {
      if ((Object) spellTableEntry.m_Spell != (Object) null)
      {
        Object.DestroyImmediate((Object) spellTableEntry.m_Spell.gameObject);
        Object.DestroyImmediate((Object) spellTableEntry.m_Spell);
        spellTableEntry.m_Spell = (Spell) null;
      }
    }
  }

  public void Show()
  {
    foreach (SpellTableEntry spellTableEntry in this.m_Table)
    {
      if (!((Object) spellTableEntry.m_Spell == (Object) null) && spellTableEntry.m_Type != SpellType.NONE)
        spellTableEntry.m_Spell.Show();
    }
  }

  public void Hide()
  {
    foreach (SpellTableEntry spellTableEntry in this.m_Table)
    {
      if (!((Object) spellTableEntry.m_Spell == (Object) null))
        spellTableEntry.m_Spell.Hide();
    }
  }
}
