using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class SpecialEventVisualMgr : MonoBehaviour
{
  public List<SpecialEventVisualDef> m_EventDefs = new List<SpecialEventVisualDef>();

  public bool LoadEvent(SpecialEventType eventType)
  {
    for (int index = 0; index < this.m_EventDefs.Count; ++index)
    {
      SpecialEventVisualDef eventDef = this.m_EventDefs[index];
      if (eventDef.m_EventType == eventType)
      {
        AssetLoader.Get().InstantiatePrefab((AssetReference) eventDef.m_Prefab, (PrefabCallback<GameObject>) null);
        return true;
      }
    }
    return false;
  }

  public bool UnloadEvent(SpecialEventType eventType)
  {
    for (int index = 0; index < this.m_EventDefs.Count; ++index)
    {
      if (this.m_EventDefs[index].m_EventType == eventType)
      {
        GameObject gameObject = GameObject.Find(this.name);
        if ((Object) gameObject != (Object) null)
          Object.Destroy((Object) gameObject);
      }
    }
    return false;
  }

  private void OnEventFinished(Spell spell, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) spell.gameObject);
  }
}
