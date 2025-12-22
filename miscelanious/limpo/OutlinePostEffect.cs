using UnityEngine;

[ExecuteInEditMode]
public class OutlinePostEffect : MonoBehaviour
{
  private Camera m_AttachedCamera;
  public Shader m_RTTOutlineGlow;
  public Shader m_DrawSimple;
  public Camera m_TempCam;
  public Material m_PostMat;

  private void Start()
  {
    this.m_AttachedCamera = this.GetComponent<Camera>();
    if (!(bool) (Object) this.m_TempCam)
      this.MakeTempCam();
    this.m_PostMat = new Material(this.m_RTTOutlineGlow);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (!(bool) (Object) this.m_AttachedCamera)
      this.m_AttachedCamera = this.GetComponent<Camera>();
    if (!(bool) (Object) this.m_TempCam)
      this.MakeTempCam();
    if (!(bool) (Object) this.m_PostMat)
      this.m_PostMat = new Material(this.m_RTTOutlineGlow);
    this.m_TempCam.CopyFrom(this.m_AttachedCamera);
    this.m_TempCam.clearFlags = CameraClearFlags.Color;
    this.m_TempCam.backgroundColor = Color.black;
    this.m_TempCam.cullingMask = 1 << LayerMask.NameToLayer("Unused16");
    RenderTexture source1 = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32);
    source1.Create();
    this.m_TempCam.targetTexture = source1;
    this.m_PostMat.SetTexture("_SceneTex", (Texture) source);
    this.m_TempCam.RenderWithShader(this.m_DrawSimple, "");
    Graphics.Blit((Texture) source1, destination, this.m_PostMat);
    source1.Release();
  }

  private void MakeTempCam()
  {
    this.m_TempCam = new GameObject().AddComponent<Camera>();
    this.m_TempCam.enabled = false;
    this.m_TempCam.name = "TempCam";
  }
}
