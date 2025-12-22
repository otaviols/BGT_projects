using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class DecalProjector : MonoBehaviour
{
  private float m_aspectRatio = 1f;
  private float m_orthographicSize = 1f;
  private float m_nearClipPlane;
  private float m_farClipPlane = 1f;
  public GameObject m_RenderCube;
  public MeshRenderer m_RenderCubeMeshRenderer;

  public float AspectRatio
  {
    get => this.m_aspectRatio;
    set
    {
      this.m_aspectRatio = value;
      this.UpdateCubePosition();
    }
  }

  public float OrthographicSize
  {
    get => this.m_orthographicSize;
    set
    {
      this.m_orthographicSize = value;
      this.UpdateCubePosition();
    }
  }

  public float NearClipPlane
  {
    get => this.m_nearClipPlane;
    set
    {
      this.m_nearClipPlane = value;
      this.UpdateCubePosition();
    }
  }

  public float FarClipPlane
  {
    get => this.m_farClipPlane;
    set
    {
      this.m_farClipPlane = value;
      this.UpdateCubePosition();
    }
  }

  public Material Material
  {
    get => this.m_RenderCubeMeshRenderer.material;
    set => this.m_RenderCubeMeshRenderer.SetMaterial(value);
  }

  public Renderer Renderer => (Renderer) this.m_RenderCubeMeshRenderer;

  private void UpdateCubePosition()
  {
    Vector3 lossyScale = this.transform.lossyScale;
    Transform transform = this.m_RenderCube.transform;
    transform.localScale = new Vector3((float) ((double) this.m_orthographicSize * (double) this.m_aspectRatio * 2.0) / lossyScale.x, this.m_orthographicSize * 2f / lossyScale.y, (this.m_farClipPlane - this.m_nearClipPlane) / lossyScale.z);
    transform.localPosition = new Vector3(0.0f, 0.0f, ((float) (((double) this.m_farClipPlane - (double) this.m_nearClipPlane) * 0.5) + this.m_nearClipPlane) / lossyScale.z);
  }

  private void OnEnable() => DecalRendererFeature.s_decals.Add(this);

  private void OnDisable() => DecalRendererFeature.s_decals.Remove(this);
}
