using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureOffsetStates : MonoBehaviour
{
  public TextureOffsetState[] m_states;
  private string m_currentState;
  private Material m_originalMaterial;

  private void Awake() => this.m_originalMaterial = this.GetComponent<Renderer>().GetSharedMaterial();

  public string CurrentState
  {
    get => this.m_currentState;
    set
    {
      TextureOffsetState textureOffsetState = ((IEnumerable<TextureOffsetState>) this.m_states).FirstOrDefault<TextureOffsetState>((Func<TextureOffsetState, bool>) (s => s.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase)));
      if (textureOffsetState == null)
        return;
      this.m_currentState = value;
      Renderer component = this.GetComponent<Renderer>();
      if ((UnityEngine.Object) textureOffsetState.Material == (UnityEngine.Object) null)
        component.SetMaterial(this.m_originalMaterial);
      else
        component.SetMaterial(textureOffsetState.Material);
      component.GetMaterial().mainTextureOffset = textureOffsetState.Offset;
    }
  }
}
