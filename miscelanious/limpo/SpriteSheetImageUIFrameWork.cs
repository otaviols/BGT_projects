using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.UI.Core;
using UnityEngine;

public class SpriteSheetImageUIFrameWork : MonoBehaviour
{
  private int m_spriteIndex;
  private Material spriteMaterial;

  [Overridable]
  public int spriteIndex
  {
    get => this.m_spriteIndex;
    set
    {
      this.m_spriteIndex = value;
      this.UpdateSprite();
    }
  }

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
    this.spriteMaterial.renderQueue = RenderUtils.ClampRenderQueueValue(this.spriteMaterial.renderQueue + 1000);
  }
}
