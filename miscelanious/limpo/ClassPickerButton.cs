using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class ClassPickerButton : HeroPickerButton
{
  public GameObject m_questBang;
  private AssetHandle<Texture> m_portraitTexture;

  public override void UpdateDisplay(DefLoader.DisposableFullDef def, TAG_PREMIUM premium)
  {
    this.m_heroClass = def?.EntityDef == null ? TAG_CLASS.INVALID : def.EntityDef.GetClass();
    base.UpdateDisplay(def, premium);
    this.SetClassname(GameStrings.GetClassName(this.m_heroClass));
    this.SetClassIcon(this.GetClassIconMaterial(this.m_heroClass));
  }

  protected override void UpdatePortrait()
  {
    if (this.UpdateLegendaryHeroPortrait() || (Object) this.m_fullDef?.CardDef == (Object) null)
      return;
    AssetHandle.Set<Texture>(ref this.m_portraitTexture, this.m_fullDef.CardDef.GetPortraitTextureHandle());
    if (!(bool) this.m_portraitTexture)
      return;
    Material premiumClassMaterial = this.m_fullDef.CardDef.GetPremiumClassMaterial();
    DeckPickerHero component1 = this.GetComponent<DeckPickerHero>();
    Renderer component2 = this.m_buttonFrame.GetComponent<Renderer>();
    List<Material> materials = component2.GetMaterials();
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if ((mode == SceneMgr.Mode.TAVERN_BRAWL || mode == SceneMgr.Mode.FRIENDLY && FriendChallengeMgr.Get().IsChallengeTavernBrawl() ? 1 : (mode != SceneMgr.Mode.FIRESIDE_GATHERING ? 0 : (FiresideGatheringManager.Get().InBrawlMode() ? 1 : 0))) == 0 & !GameUtils.HasUnlockedClass(this.m_heroClass) && (Object) this.m_fullDef.CardDef.m_LockedClassPortrait != (Object) null)
      materials[component1.m_PortraitMaterialIndex] = this.m_fullDef.CardDef.m_LockedClassPortrait;
    else if (this.m_premium == TAG_PREMIUM.GOLDEN && (Object) premiumClassMaterial != (Object) null)
    {
      materials[component1.m_PortraitMaterialIndex] = premiumClassMaterial;
      if (!this.m_seed.HasValue)
        this.m_seed = new float?(Random.value);
      if (materials[component1.m_PortraitMaterialIndex].HasProperty("_Seed"))
        materials[component1.m_PortraitMaterialIndex].SetFloat("_Seed", this.m_seed.Value);
      if ((bool) (Object) this.m_fullDef.CardDef.GetPremiumPortraitAnimation())
      {
        UberShaderController shaderController = this.m_buttonFrame.GetComponent<UberShaderController>();
        if ((Object) shaderController == (Object) null)
          shaderController = this.m_buttonFrame.AddComponent<UberShaderController>();
        shaderController.UberShaderAnimation = Object.Instantiate<UberShaderAnimation>(this.m_fullDef.CardDef.GetPremiumPortraitAnimation());
        shaderController.m_MaterialIndex = component1.m_PortraitMaterialIndex;
      }
    }
    else
    {
      Material cachedMaterial = this.GetCachedMaterial(component1.m_PortraitMaterialIndex);
      if ((Object) cachedMaterial != (Object) null)
        materials[component1.m_PortraitMaterialIndex] = Object.Instantiate<Material>(cachedMaterial);
      materials[component1.m_PortraitMaterialIndex].mainTexture = (Texture) this.m_portraitTexture;
    }
    component2.SetMaterials(materials);
  }

  public override void Lock()
  {
    base.Lock();
    this.ShowQuestBang(true);
    this.m_heroClassIcon.SetActive(false);
    this.m_heroClassIconSepia.SetActive(true);
  }

  public override void Unlock()
  {
    int num = this.IsLocked() ? 1 : 0;
    base.Unlock();
    if (num != 0)
      this.UpdatePortrait();
    this.ShowQuestBang(false);
    this.m_heroClassIcon.SetActive(true);
    this.m_heroClassIconSepia.SetActive(false);
  }

  public void ShowQuestBang(bool shown) => this.m_questBang.SetActive(shown);

  protected override void OnDestroy()
  {
    AssetHandle.SafeDispose<Texture>(ref this.m_portraitTexture);
    base.OnDestroy();
  }
}
