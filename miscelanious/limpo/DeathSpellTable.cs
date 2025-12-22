using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DeathSpellTable : MonoBehaviour
{
  public const string DeathSpellTableReference = "Player_DeathSpellTable.prefab:596edf7d7ce58964b8c34eb1383f04e1";
  private static DeathSpellTable m_instance;
  public List<DeathSpellTableEntry> m_Table = new List<DeathSpellTableEntry>();
  private Dictionary<DeathSpellType, DeathSpellTableEntry> m_Entries;

  public static DeathSpellTable Get()
  {
    if ((Object) DeathSpellTable.m_instance == (Object) null)
    {
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Player_DeathSpellTable.prefab:596edf7d7ce58964b8c34eb1383f04e1");
      if ((Object) gameObject == (Object) null)
      {
        Error.AddDevWarning("Death Spell Table", "Death Spell Table: {0} failed to create game object.", (object) "Player_DeathSpellTable.prefab:596edf7d7ce58964b8c34eb1383f04e1");
        return (DeathSpellTable) null;
      }
      DeathSpellTable.m_instance = gameObject.GetComponent<DeathSpellTable>();
      if ((Object) DeathSpellTable.m_instance == (Object) null)
      {
        Error.AddDevWarning("Death Spell Table", "Death Spell Table: {0} doesn't have a death spell table component.", (object) "Player_DeathSpellTable.prefab:596edf7d7ce58964b8c34eb1383f04e1");
        return (DeathSpellTable) null;
      }
    }
    return DeathSpellTable.m_instance;
  }

  private bool TryGetEntry(DeathSpellType type, out DeathSpellTableEntry entry)
  {
    if (this.m_Entries == null)
      this.IntialzieSpellTable();
    return this.m_Entries.TryGetValue(type, out entry);
  }

  private void IntialzieSpellTable()
  {
    this.m_Entries = new Dictionary<DeathSpellType, DeathSpellTableEntry>();
    foreach (DeathSpellTableEntry deathSpellTableEntry in this.m_Table)
    {
      if (this.m_Entries.ContainsKey(deathSpellTableEntry.m_Type))
        Error.AddDevWarning("Spell Table", "Spell Table: {0} Entry List contains more than one entry for spell type {1}, please remove the duplicate.", (object) this.name, (object) deathSpellTableEntry.m_Type);
      else
        this.m_Entries.Add(deathSpellTableEntry.m_Type, deathSpellTableEntry);
    }
  }

  public Spell GetSpell(DeathSpellType spellType)
  {
    DeathSpellTableEntry entry;
    if (!this.TryGetEntry(spellType, out entry))
      return (Spell) null;
    string spellPrefabName = entry.m_SpellPrefabName;
    if (string.IsNullOrEmpty(spellPrefabName))
    {
      Error.AddDevWarning("Spell Table", "The Spell Prefab Name for {0} is empty.", (object) entry.m_Type);
      return (Spell) null;
    }
    Spell spell = SpellManager.Get().GetSpell(spellPrefabName);
    spell.SetSpellType(SpellType.DEATH);
    return spell;
  }
}
