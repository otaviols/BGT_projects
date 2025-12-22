using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class WeaponUVWorldspace : MonoBehaviour
{
  public float xOffset;
  public float yOffset;
  public float scale = 0.7f;
  private Material m_material;

  private void Start() => this.m_material = this.gameObject.GetComponent<Renderer>().GetMaterial();

  private void Update()
  {
    Vector3 vector3 = this.transform.position * this.scale;
    this.m_material.SetFloat("_OffsetX", -vector3.z - this.xOffset);
    this.m_material.SetFloat("_OffsetY", -vector3.x - this.yOffset);
  }
}
