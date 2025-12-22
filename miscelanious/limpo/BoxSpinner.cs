using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class BoxSpinner : MonoBehaviour
{
  private Box m_parent;
  private BoxSpinnerStateInfo m_info;
  private bool m_spinning;
  private float m_spinY;
  private Material m_spinnerMat;

  private void Awake() => this.MaterialChanged();

  private void Update()
  {
    if (!this.IsSpinning() || (Object) this.m_spinnerMat == (Object) null || this.m_info == null)
      return;
    this.m_spinnerMat.SetFloat("_RotAngle", this.m_spinY);
    this.m_spinY += (float) ((double) this.m_info.m_DegreesPerSec * (double) Time.deltaTime * 0.00999999977648258);
  }

  private void OnDestroy()
  {
    Object.Destroy((Object) this.m_spinnerMat);
    this.m_spinnerMat = (Material) null;
    this.m_parent = (Box) null;
    this.m_info = (BoxSpinnerStateInfo) null;
  }

  public Box GetParent() => this.m_parent;

  public void SetParent(Box parent) => this.m_parent = parent;

  public BoxSpinnerStateInfo GetInfo() => this.m_info;

  public void SetInfo(BoxSpinnerStateInfo info) => this.m_info = info;

  public void Spin() => this.m_spinning = true;

  public bool IsSpinning() => this.m_spinning;

  public void Stop() => this.m_spinning = false;

  public void Reset()
  {
    this.m_spinning = false;
    this.m_spinnerMat?.SetFloat("_RotAngle", 0.0f);
  }

  public void MaterialChanged() => this.m_spinnerMat = this.GetComponent<Renderer>().GetMaterial();
}
