using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prototyping/Keyboard Animation Settings")]
public class KeyboardAnimationSettings : ScriptableObject
{
  [Tooltip("A list of the bindings from keyboard keys to animation trigger names.")]
  public List<KeyboardAnimationSettings.KeyAndAnimationTriggerPair> Settings = new List<KeyboardAnimationSettings.KeyAndAnimationTriggerPair>();

  public KeyboardAnimationSettings.KeyAndAnimationTriggerPair this[int idx]
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
    [Tooltip("The name of the animation trigger (not the state) to set when the keyboard key is pressed.")]
    public string AnimationTrigger;
  }
}
