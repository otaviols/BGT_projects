using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CenterDiskSetRotation : MonoBehaviour
{
  [SerializeField]
  private GameObject m_diskMesh;
  [SerializeField]
  private GameObject m_buttonMesh;

  public void ApplyBoxDressingMaterials(EventBoxDressing.BoxDressingMaterials materials)
  {
    if (materials == null || (Object) materials.BoxMaterial == (Object) null || (Object) materials.SetRotationButtonMaterial == (Object) null)
      return;
    Renderer component1 = this.m_diskMesh?.GetComponent<Renderer>();
    if ((Object) this.m_diskMesh != (Object) null && (Object) component1 != (Object) null)
    {
      component1.SetMaterial(0, materials.BoxMaterial);
      component1.SetMaterial(2, materials.SetRotationButtonMaterial);
    }
    Renderer component2 = this.m_buttonMesh?.GetComponent<Renderer>();
    if (!((Object) this.m_buttonMesh != (Object) null) || !((Object) component2 != (Object) null))
      return;
    component2.SetMaterial(materials.SetRotationButtonMaterial);
  }
}
