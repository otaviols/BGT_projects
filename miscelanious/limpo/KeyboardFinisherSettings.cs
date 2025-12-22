using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prototyping/Keyboard Finisher Settings")]
public class KeyboardFinisherSettings : ScriptableObject
{
  [Tooltip("A list of the bindings from keyboard keys to finisher data.")]
  public List<KeyboardFinisherSettings.KeyAndFinisherTriggerPair> Settings = new List<KeyboardFinisherSettings.KeyAndFinisherTriggerPair>();

  public KeyboardFinisherSettings.KeyAndFinisherTriggerPair this[int idx]
  {
    get => this.Settings[idx];
    set => this.Settings[idx] = value;
  }

  public int Count => this.Settings.Count;

  public enum DamageLevel
  {
    Small,
    Large,
  }

  public enum LethalLevel
  {
    Nonlethal,
    Lethal,
    FirstPlaceVictory,
  }

  [Serializable]
  public class KeyAndFinisherTriggerPair
  {
    public KeyCode KeyboardKey;
    public FinisherGameplaySettings Finisher;
    public KeyboardFinisherSettings.DamageLevel DamageLevel;
    public KeyboardFinisherSettings.LethalLevel LethalLevel;
    public int ImpactDamage = 1;
    public FinisherAuthoringList AllFinishers;
  }
}
