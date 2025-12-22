using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class EventOverride : MonoBehaviour
{
  public List<EventOverride.EventOverrideElement> m_SpecialEvents;

  private List<Spell> m_ActiveSpells { get; set; }

  private void Start()
  {
    this.m_ActiveSpells = new List<Spell>();
    if (GameMgr.Get().IsTraditionalTutorial())
      return;
    foreach (EventOverride.EventOverrideElement specialEvent in this.m_SpecialEvents)
    {
      if (SpecialEventManager.Get().IsEventActive(specialEvent.EventType, false))
        this.LoadSpecialEvent(specialEvent);
    }
  }

  public virtual void LoadSpecialEvent(EventOverride.EventOverrideElement specialEvent)
  {
    if (!SpecialEventManager.Get().IsEventForcedActive(specialEvent.EventType) && (!specialEvent.showToNewPlayers && !AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.DAILY) || !specialEvent.showToReturningPlayers && ReturningPlayerMgr.Get().IsInReturningPlayerMode))
      return;
    string assetRef = specialEvent.EventPrefab;
    if (PlatformSettings.Screen == ScreenCategory.Phone && !string.IsNullOrEmpty(specialEvent.EventPrefab_phone))
      assetRef = specialEvent.EventPrefab_phone;
    if (string.IsNullOrEmpty(assetRef))
      return;
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("Failed to load special event prefab: {0}", (object) assetRef));
    }
    else
    {
      gameObject.transform.SetParent(this.transform, false);
      if ((UnityEngine.Object) specialEvent.Parent != (UnityEngine.Object) null)
        gameObject.transform.SetParent(specialEvent.Parent.transform, true);
      Spell component = gameObject.GetComponent<Spell>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      component.ActivateState(SpellStateType.BIRTH);
      this.m_ActiveSpells.Add(component);
    }
  }

  private void OnDisable()
  {
    foreach (Spell mActiveSpell in this.m_ActiveSpells)
    {
      if ((UnityEngine.Object) mActiveSpell != (UnityEngine.Object) null && mActiveSpell.gameObject.activeSelf && mActiveSpell.IsActive())
        mActiveSpell.ActivateState(SpellStateType.DEATH);
    }
    this.m_ActiveSpells.Clear();
  }

  [Serializable]
  public class EventOverrideElement
  {
    public SpecialEventType EventType;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string EventPrefab;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string EventPrefab_phone;
    public GameObject Parent;
    public bool showToReturningPlayers;
    public bool showToNewPlayers;
  }
}
