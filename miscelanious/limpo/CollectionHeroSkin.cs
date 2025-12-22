using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CollectionHeroSkin : CollectibleSkin
{
  public MeshRenderer m_classIcon;
  public Spell m_socketFX;

  public void SetClass(TAG_CLASS classTag)
  {
    if ((Object) this.m_classIcon != (Object) null)
    {
      Vector2 classTextureOffset = CollectionPageManager.s_classTextureOffsets[classTag];
      Renderer component = this.m_classIcon.GetComponent<Renderer>();
      (Application.isPlaying ? component.GetMaterial() : component.GetSharedMaterial()).SetTextureOffset("_MainTex", classTextureOffset);
    }
    if (!((Object) this.m_favoriteBannerText != (Object) null))
      return;
    this.m_favoriteBannerText.Text = GameStrings.Format("GLUE_COLLECTION_MANAGER_FAVORITE_DEFAULT_TEXT", (object) GameStrings.GetClassName(classTag));
  }

  public void ShowSocketFX()
  {
    if ((Object) this.m_socketFX == (Object) null || !this.m_socketFX.gameObject.activeInHierarchy)
      return;
    this.m_socketFX.gameObject.SetActive(true);
    this.m_socketFX.Activate();
  }

  public void HideSocketFX()
  {
    if (!((Object) this.m_socketFX != (Object) null))
      return;
    this.m_socketFX.Deactivate();
  }
}
