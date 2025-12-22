using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class SpriteSheetImage : MonoBehaviour
{
  public int spriteIndex;
  private Material spriteMaterial;

  private void Start() => this.UpdateSprite();

  private void UpdateSprite()
  {
    this.spriteMaterial = this.GetComponent<MeshRenderer>().GetMaterial();
    if ((Object) this.spriteMaterial == (Object) null)
      return;
    float x = this.spriteMaterial.mainTextureScale.x;
    float y = this.spriteMaterial.mainTextureScale.y;
    int num = (int) (1.0 / (double) x);
    this.spriteMaterial.mainTextureOffset = new Vector2(this.spriteIndex <= num ? (float) this.spriteIndex * x : (float) (this.spriteIndex % num) * x, (float) (1.0 - ((double) Mathf.CeilToInt((float) (this.spriteIndex / num)) * (double) y + (double) y)));
    this.spriteMaterial.renderQueue += 1000;
  }
}
