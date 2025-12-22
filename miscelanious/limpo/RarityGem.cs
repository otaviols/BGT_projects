using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class RarityGem : MonoBehaviour
{
  public void SetRarityGem(TAG_RARITY rarity, TAG_CARD_SET cardSet)
  {
    Renderer component = this.GetComponent<Renderer>();
    if (rarity == TAG_RARITY.FREE)
    {
      component.enabled = false;
    }
    else
    {
      component.enabled = true;
      switch (rarity - 1)
      {
        case TAG_RARITY.FREE:
          component.GetMaterial().mainTextureOffset = new Vector2(0.118f, 0.0f);
          break;
        case TAG_RARITY.RARE:
          component.GetMaterial().mainTextureOffset = new Vector2(0.239f, 0.0f);
          break;
        case TAG_RARITY.EPIC:
          component.GetMaterial().mainTextureOffset = new Vector2(0.3575f, 0.0f);
          break;
        default:
          component.GetMaterial().mainTextureOffset = new Vector2(0.0f, 0.0f);
          break;
      }
    }
  }
}
