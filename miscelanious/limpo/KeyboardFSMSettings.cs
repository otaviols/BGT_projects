using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prototyping/Keyboard FSM Settings")]
public class KeyboardFSMSettings : ScriptableObject
{
  [Tooltip("A list of the bindings from keyboard keys to animation trigger names.")]
  public List<KeyboardFSMSettings.KeyAndAnimationTriggerPair> Settings = new List<KeyboardFSMSettings.KeyAndAnimationTriggerPair>();

  public KeyboardFSMSettings.KeyAndAnimationTriggerPair this[int idx]
  {
    get => this.Settings[idx];
    set => this.Settings[idx] = value;
  }

  public int Count => this.Settings.Count;

  [Serializable]
  public class KeyAndAnimationTriggerPair
  {
    [Tooltip("The keyboard key to press to trigger the animation.")]
    public KeyCode KeyboardKey;
    [Tooltip("The name of the Playmaker state to enter when the keyboard key is pressed. Leave blank to prevent running a playmaker state.")]
    public string PlaymakerState;
  }
}
