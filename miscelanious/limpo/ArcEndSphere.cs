using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class ArcEndSphere : MonoBehaviour
{
  private void Update()
  {
    Material material = this.GetComponent<Renderer>().GetMaterial();
    Vector2 mainTextureOffset = material.mainTextureOffset;
    mainTextureOffset.x += Time.deltaTime * 1f;
    material.mainTextureOffset = mainTextureOffset;
  }
}
