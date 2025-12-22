using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class Rune : MonoBehaviour
{
  public MeshRenderer m_runeRenderer;
  public MeshRenderer m_highlightRenderer;
  public Material m_runeMaterialEmpty;
  public List<RuneAssetPaths> m_runeAssetTable = new List<RuneAssetPaths>();
  private RuneType m_runeType;
  private RuneState m_state = RuneState.Default;

  public RuneType GetRuneType() => this.m_runeType;

  public void ShowRune(RuneType type, RuneState state)
  {
    this.ShowRune();
    this.SetMaterial(type, state);
  }

  public void ShowRune()
  {
    if ((Object) this.m_runeRenderer == (Object) null)
      return;
    this.m_runeRenderer.enabled = true;
  }

  public void SetHighlighted(bool highlighted)
  {
    if ((Object) this.m_highlightRenderer == (Object) null)
      return;
    this.m_highlightRenderer.enabled = highlighted;
    if (this.m_runeType == RuneType.RT_NONE)
      return;
    Material highlightMaterial = this.GetHighlightMaterial();
    if (!(bool) (Object) highlightMaterial)
      return;
    this.m_highlightRenderer.SetMaterial(highlightMaterial);
  }

  private void SetMaterial(RuneType type, RuneState state)
  {
    this.m_runeType = type;
    this.m_state = state;
    Material runeMaterial = this.GetRuneMaterial();
    if (!((Object) runeMaterial != (Object) null))
      return;
    this.m_runeRenderer.SetMaterial(runeMaterial);
  }

  public void HideRune()
  {
    if ((Object) this.m_runeRenderer == (Object) null)
      return;
    this.m_runeRenderer.enabled = false;
  }

  private Material GetHighlightMaterial()
  {
    if (this.m_runeType == RuneType.RT_NONE)
      return this.m_runeMaterialEmpty;
    foreach (RuneAssetPaths runeAssetPaths in this.m_runeAssetTable)
    {
      if (runeAssetPaths.m_assetRuneType == this.m_runeType)
        return runeAssetPaths.m_runeMaterialHighlighted;
    }
    return (Material) null;
  }

  private Material GetRuneMaterial()
  {
    if (this.m_runeType == RuneType.RT_NONE)
      return this.m_runeMaterialEmpty;
    foreach (RuneAssetPaths runeAssetPaths in this.m_runeAssetTable)
    {
      if (runeAssetPaths.m_assetRuneType == this.m_runeType)
      {
        switch (this.m_state)
        {
          case RuneState.Default:
            return runeAssetPaths.m_runeMaterial;
          case RuneState.Highlighted:
            return runeAssetPaths.m_runeMaterialHighlighted;
          case RuneState.Disabled:
            return runeAssetPaths.m_runeMaterialGhosted;
          default:
            return runeAssetPaths.m_runeMaterial;
        }
      }
    }
    return (Material) null;
  }
}
