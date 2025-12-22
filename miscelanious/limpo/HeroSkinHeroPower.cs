using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class HeroSkinHeroPower : MonoBehaviour
{
  public Actor m_Actor;
  public Texture m_OriginalFrontTexture;
  public Texture m_OriginalBackTexture;

  private void Start()
  {
    if (!SceneMgr.Get().IsInGame())
      return;
    this.StartCoroutine(this.HeroSkinCustomHeroPowerTextures());
  }

  public void SetFrontTexture(Texture tex) => RendererExtension.GetMaterial(this.GetComponent<Renderer>()).mainTexture = tex;

  public void SetBackTexture(Texture tex)
  {
    Renderer component = this.GetComponent<Renderer>();
    RendererExtension.GetMaterial(component, 1).SetTexture("_SecondTex", tex);
    RendererExtension.GetMaterial(component, 2).mainTexture = tex;
  }

  private IEnumerator HeroSkinCustomHeroPowerTextures()
  {
    Card card = this.m_Actor.GetCard();
    while ((Object) card == (Object) null)
    {
      card = this.m_Actor.GetCard();
      yield return (object) 0;
    }
    Card heroCard = card.GetHeroCard();
    while ((Object) heroCard == (Object) null)
    {
      heroCard = card.GetHeroCard();
      yield return (object) 0;
    }
    if (!heroCard.HasCardDef)
      Debug.LogWarning((object) "HeroSkinHeroPower: heroCardDef is null!");
  }

  private void OnFrontTextureLoaded(AssetReference assetRef, Object asset, object callbackData) => this.SetFrontTexture((Texture) (asset as Texture2D));

  private void OnBackTextureLoaded(AssetReference assetRef, Object asset, object callbackData) => this.SetBackTexture((Texture) (asset as Texture2D));
}
