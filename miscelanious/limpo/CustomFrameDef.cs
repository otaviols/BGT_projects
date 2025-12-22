using UnityEngine;

[RequireComponent(typeof (HSDontDestroyOnLoad))]
public class CustomFrameDef : MonoBehaviour
{
  [Header("Mesh replacement")]
  public MeshRenderer Mesh;
  public int FrameMatIdx;
  public int PortraitMatIdx;
  [Header("Texture replacement")]
  public Texture2D Silhouette;
  [Header("Highlight")]
  public HighlightRenderOverrides HighlightOverrides;
  public HighlightRenderOverrides CollectionOverrides;
  public HighlightRenderOverrides CardOverrides;
  [Header("Calibration")]
  public float DecorationRootOffset;
  public float HeroClassIconOffset;
  public float AvoidShadowPlaneOffset;
  public float HeroZonePositionOffset;
  public float HeroPickerRaiseAndLowerLimit = -1f;
  public float HeroPowerContainerOffset;
}
