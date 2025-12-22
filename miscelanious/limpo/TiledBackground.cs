using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class TiledBackground : MonoBehaviour
{
  private Renderer m_renderer;
  private Material m_material;
  public float Depth;

  private Renderer TiledRenderer
  {
    get
    {
      if ((Object) this.m_renderer == (Object) null)
        this.m_renderer = this.GetComponent<Renderer>();
      return this.m_renderer;
    }
  }

  private Material TiledMaterial
  {
    get
    {
      if ((Object) this.m_material == (Object) null && (Object) this.TiledRenderer != (Object) null)
        this.m_material = this.TiledRenderer.GetMaterial();
      return this.m_material;
    }
  }

  public Vector2 Offset
  {
    get
    {
      if ((Object) this.TiledMaterial == (Object) null)
        return Vector2.zero;
      Vector3 mainTextureOffset = (Vector3) this.TiledMaterial.mainTextureOffset;
      Vector3 mainTextureScale = (Vector3) this.TiledMaterial.mainTextureScale;
      return new Vector2(mainTextureOffset.x / mainTextureScale.x, mainTextureOffset.y / mainTextureScale.y);
    }
    set
    {
      if ((Object) this.TiledMaterial == (Object) null)
        return;
      Vector3 mainTextureScale = (Vector3) this.TiledMaterial.mainTextureScale;
      this.TiledMaterial.mainTextureOffset = new Vector2(mainTextureScale.x * value.x, mainTextureScale.y * value.y);
    }
  }

  private void Awake()
  {
    if (!((Object) this.TiledMaterial == (Object) null))
      return;
    Debug.LogError((object) "TiledBackground requires the mesh renderer and for it to have a material!");
    Object.Destroy((Object) this);
  }

  public void SetBounds(Bounds bounds)
  {
    if ((Object) this.TiledRenderer == (Object) null)
    {
      Debug.LogError((object) "TiledBackground.SetBounds - no renderer was found on this game object!");
    }
    else
    {
      this.transform.localScale = Vector3.one;
      Bounds bounds1 = this.TiledRenderer.bounds;
      Vector3 position1 = bounds1.min;
      Vector3 position2 = bounds1.max;
      if ((Object) this.transform.parent != (Object) null)
      {
        position1 = this.transform.parent.InverseTransformPoint(position1);
        position2 = this.transform.parent.InverseTransformPoint(position2);
      }
      Vector3 vector3_1 = VectorUtils.Abs(position2 - position1);
      Vector3 vector3_2 = new Vector3((double) vector3_1.x > 0.0 ? bounds.size.x / vector3_1.x : 0.0f, (double) vector3_1.y > 0.0 ? bounds.size.y / vector3_1.y : 0.0f, (double) vector3_1.z > 0.0 ? bounds.size.z / vector3_1.z : 0.0f);
      this.transform.localScale = vector3_2;
      this.transform.localPosition = bounds.center + new Vector3(0.0f, 0.0f, -this.Depth);
      if ((Object) this.TiledMaterial == (Object) null)
        Debug.LogError((object) "TiledBackground.SetBounds - no material was found on this component!");
      else
        this.TiledMaterial.mainTextureScale = (Vector2) vector3_2;
    }
  }
}
