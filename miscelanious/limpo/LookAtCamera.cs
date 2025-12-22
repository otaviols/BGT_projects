using UnityEngine;

[ExecuteAlways]
public class LookAtCamera : MonoBehaviour
{
  private readonly Vector3 X_VECTOR = new Vector3(1f, 0.0f, 0.0f);
  private readonly Vector3 Y_VECTOR = new Vector3(0.0f, 1f, 0.0f);
  private readonly Vector3 Z_VECTOR = new Vector3(0.0f, 0.0f, 1f);
  public Vector3 m_LookAtPositionOffset = Vector3.zero;
  private Camera m_MainCamera;
  private GameObject m_LookAtTarget;

  private void Awake() => this.CreateLookAtTarget();

  private void Start() => this.m_MainCamera = Camera.main;

  private void Update()
  {
    if ((Object) this.m_MainCamera == (Object) null)
    {
      this.m_MainCamera = Camera.main;
      if ((Object) this.m_MainCamera == (Object) null)
        return;
    }
    if ((Object) this.m_LookAtTarget == (Object) null)
    {
      this.CreateLookAtTarget();
      if ((Object) this.m_LookAtTarget == (Object) null)
        return;
    }
    this.m_LookAtTarget.transform.position = this.m_MainCamera.transform.position + this.m_LookAtPositionOffset;
    this.transform.LookAt(this.m_LookAtTarget.transform, this.Z_VECTOR);
    this.transform.Rotate(this.X_VECTOR, 90f);
    this.transform.Rotate(this.Y_VECTOR, 180f);
  }

  private void OnDestroy()
  {
    if (!(bool) (Object) this.m_LookAtTarget)
      return;
    Object.Destroy((Object) this.m_LookAtTarget);
  }

  private void CreateLookAtTarget()
  {
    this.m_LookAtTarget = new GameObject();
    this.m_LookAtTarget.name = "LookAtCamera Target";
  }
}
