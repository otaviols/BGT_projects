using System;
using UnityEngine;

[Serializable]
public class TagVisualConfigurationEntry
{
  [CustomEditField(SortPopupByName = true)]
  public GAME_TAG m_Tag;
  [CustomEditField(HidePredicate = "IsReferenceTag", Sections = "Settings")]
  public bool m_IgnoreCanShowActorVisuals;
  [CustomEditField(Sections = "Settings", SortPopupByName = true)]
  [Tooltip("Use this to avoid repeating yourself when the Tags do the same thing (e.g. Shifting, Shifting_Weapon, Shifting_Spell).")]
  public GAME_TAG m_ReferenceTag;
  [CustomEditField(HidePredicate = "IsReferenceTag", Sections = "Settings")]
  public bool m_IsPlayStateSpell;
  [CustomEditField(HidePredicate = "IsReferenceTag", Sections = "Settings")]
  public bool m_IsHandStateSpell;
  [CustomEditField(HidePredicate = "IsReferenceTag", Sections = "Settings")]
  [Tooltip("A list of actions to perform every time the Tag changes, do these before handling \"Tag Added\", \"Tag Removed\", or \"After Always\" actions.")]
  public TagVisualStateConfiguration m_BeforeAlways;
  [Tooltip("A list of actions to perform every time a Tag changes from \"0\".")]
  [CustomEditField(HidePredicate = "IsReferenceTag", Sections = "Settings")]
  public TagVisualStateConfiguration m_TagAdded;
  [Tooltip("A list of actions to perform every time a Tag changes to \"0\".")]
  [CustomEditField(HidePredicate = "IsReferenceTag", Sections = "Settings")]
  public TagVisualStateConfiguration m_TagRemoved;
  [Tooltip("A list of actions to perform every time the Tag changes, do these after handling \"Tag Added\", \"Tag Removed\", or \"Before Always\" actions.")]
  [CustomEditField(HidePredicate = "IsReferenceTag", Sections = "Settings")]
  public TagVisualStateConfiguration m_AfterAlways;
}
