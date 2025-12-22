using UnityEngine;

public class MeshAnimation : MonoBehaviour
{
  public Mesh[] Meshes;
  public bool Loop;
  public float FrameDuration;
  private int m_Index;
  private bool m_Playing;
  private float m_FrameTime;
  private MeshFilter m_Mesh;

  private void Start() => this.m_Mesh = this.GetComponent<MeshFilter>();

  private void Update()
  {
    if (!this.m_Playing)
      return;
    this.m_FrameTime += Time.deltaTime;
    if ((double) this.m_FrameTime < (double) this.FrameDuration)
      return;
    this.m_Index = (this.m_Index + 1) % this.Meshes.Length;
    this.m_FrameTime -= this.FrameDuration;
    if (!this.Loop && this.m_Index == 0)
    {
      this.m_Playing = false;
      this.enabled = false;
    }
    else
      this.m_Mesh.mesh = this.Meshes[this.m_Index];
  }

  public void Play()
  {
    this.enabled = true;
    this.m_Playing = true;
  }

  public void Stop()
  {
    this.m_Playing = false;
    this.enabled = false;
  }

  public void Reset()
  {
    this.m_Mesh.mesh = this.Meshes[0];
    this.m_FrameTime = 0.0f;
    this.m_Index = 0;
  }
}
