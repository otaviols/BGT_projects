using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Highlight Render Overrides")]
public class HighlightRenderOverrides : ScriptableObject
{
  [Header("Render Plane")]
  public bool OverrideTransform;
  public Vector3 Position;
  public float Scale = 1f;
  [Header("Render Settings")]
  public float SilouetteRenderSize = 1f;
  public float SilouetteClipSize = 1f;
}
