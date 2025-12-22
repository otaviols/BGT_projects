using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone.UI.Core;
using UnityEngine;

public class DeckTrayMercenaryVisual : MonoBehaviour
{
  public GameObject m_portraitObject;
  public int m_portraitMaterialIndex;
  public DeckTrayMercenaryVisual.CardDefMaterialType m_materialType;
  public Renderer m_Frame;
  public Material m_NormalFrameMaterial;
  public Material m_GoldenFrameMaterial;
  public Material m_DiamondFrameMaterial;
  private LettuceMercenary.ArtVariation m_artVariation;
  private DefLoader.DisposableCardDef m_disposableDef;
  private string m_cardId;
  private bool m_isCardPremiumTagSet;
  private TAG_PREMIUM m_cardPremiumTag;
  private string m_desiredCardID;
  private bool m_isDesiredCardPremiumTagSet;
  private TAG_PREMIUM m_desiredCardPremiumTag;

  [Overridable]
  public int TeamId
  {
    set
    {
      LettuceMercenary lettuceMercenary = (LettuceMercenary) null;
      LettuceMercenary.Loadout loadout = (LettuceMercenary.Loadout) null;
      LettuceTeam team = CollectionManager.Get().GetTeam((long) value);
      if (team != null)
      {
        lettuceMercenary = team.GetLeader();
        if (lettuceMercenary != null)
          loadout = team.GetLoadout(lettuceMercenary);
      }
      this.UpdateVisuals(lettuceMercenary, loadout);
    }
  }

  [Overridable]
  public int MercenaryId
  {
    set
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) value);
      this.UpdateVisuals(mercenary, mercenary?.GetCurrentLoadout());
    }
  }

  [Overridable]
  public string CardId
  {
    set
    {
      if (this.m_isDesiredCardPremiumTagSet)
      {
        this.UpdateVisuals(value, this.m_desiredCardPremiumTag);
        this.m_desiredCardID = (string) null;
        this.m_isDesiredCardPremiumTagSet = false;
      }
      else
        this.m_desiredCardID = value;
    }
  }

  [Overridable]
  public TAG_PREMIUM CardPremium
  {
    set
    {
      if (this.m_desiredCardID != null)
      {
        this.UpdateVisuals(this.m_desiredCardID, value);
        this.m_desiredCardID = (string) null;
        this.m_isDesiredCardPremiumTagSet = false;
      }
      else
      {
        this.m_isDesiredCardPremiumTagSet = true;
        this.m_desiredCardPremiumTag = value;
      }
    }
  }

  private void OnDestroy()
  {
    this.m_disposableDef?.Dispose();
    this.m_disposableDef = (DefLoader.DisposableCardDef) null;
  }

  private Material GetMaterial(TAG_PREMIUM premium)
  {
    switch (this.m_materialType)
    {
      case DeckTrayMercenaryVisual.CardDefMaterialType.CustomDeck:
        return this.m_disposableDef?.CardDef?.GetCustomDeckPortrait();
      case DeckTrayMercenaryVisual.CardDefMaterialType.DeckCardBar:
        return this.m_disposableDef?.CardDef?.GetDeckCardBarPortrait(premium);
      default:
        return (Material) null;
    }
  }

  private void UpdateVisuals(string cardId, TAG_PREMIUM premium)
  {
    this.m_artVariation = (LettuceMercenary.ArtVariation) null;
    if (this.m_cardId == cardId && this.m_isCardPremiumTagSet && this.m_cardPremiumTag == premium)
      return;
    this.m_cardId = cardId;
    this.m_cardPremiumTag = premium;
    this.m_isCardPremiumTagSet = true;
    this.m_disposableDef?.Dispose();
    this.m_disposableDef = DefLoader.Get()?.GetCardDef(cardId, premium);
    if ((Object) this.m_disposableDef?.CardDef == (Object) null)
      Log.Lettuce.PrintError("Card Def is null");
    else
      this.UpdatePortraiteMaterial(this.m_cardPremiumTag);
  }

  private void UpdateVisuals(LettuceMercenary mercenary, LettuceMercenary.Loadout loadout)
  {
    this.m_cardId = string.Empty;
    this.m_isCardPremiumTagSet = false;
    if (mercenary == null)
    {
      this.m_disposableDef?.Dispose();
      this.m_disposableDef = (DefLoader.DisposableCardDef) null;
      this.m_artVariation = (LettuceMercenary.ArtVariation) null;
    }
    else
    {
      LettuceMercenary.ArtVariation artVariation = loadout != null ? mercenary.GetOwnedArtVariation(loadout.m_artVariationRecord.ID, loadout.m_artVariationPremium) : mercenary.GetDefaultOrFirstAvailableArtVariation();
      if (this.m_artVariation == artVariation)
        return;
      this.m_disposableDef?.Dispose();
      this.m_artVariation = artVariation;
      this.m_disposableDef = DefLoader.Get().GetCardDef(this.m_artVariation.m_record.CardId);
      this.UpdatePortraiteMaterial(this.m_artVariation.m_premium);
    }
  }

  private void UpdatePortraiteMaterial(TAG_PREMIUM cardPremiumTag)
  {
    Renderer component = this.m_portraitObject.GetComponent<Renderer>();
    component.SetSharedMaterial(this.m_portraitMaterialIndex, this.GetMaterial(cardPremiumTag));
    if ((Object) this.m_Frame != (Object) null)
    {
      switch (cardPremiumTag)
      {
        case TAG_PREMIUM.NORMAL:
          this.m_Frame.SetMaterial(this.m_NormalFrameMaterial);
          break;
        case TAG_PREMIUM.GOLDEN:
          this.m_Frame.SetMaterial(this.m_GoldenFrameMaterial);
          break;
        case TAG_PREMIUM.DIAMOND:
          this.m_Frame.SetMaterial(this.m_DiamondFrameMaterial);
          break;
      }
    }
    if (this.m_materialType == DeckTrayMercenaryVisual.CardDefMaterialType.DeckCardBar || ServiceManager.Get<IGraphicsManager>().isVeryLowQualityDevice() || cardPremiumTag == TAG_PREMIUM.NORMAL)
      return;
    Material portraitMaterial = this.m_disposableDef.CardDef.GetPremiumPortraitMaterial();
    if ((Object) portraitMaterial != (Object) null)
    {
      Material material1 = component.GetMaterial(this.m_portraitMaterialIndex);
      Texture texture = (Texture) null;
      if (material1.HasProperty("_ShadowTex"))
        texture = material1.GetTexture("_ShadowTex");
      component.SetMaterial(this.m_portraitMaterialIndex, portraitMaterial);
      Material material2 = component.GetMaterial(this.m_portraitMaterialIndex);
      material2.SetTexture("_ShadowTex", texture);
      material2.mainTextureOffset = material1.mainTextureOffset;
      material2.mainTextureScale = material1.mainTextureScale;
    }
    UberShaderAnimation portraitAnimation = this.m_disposableDef.CardDef.GetPremiumPortraitAnimation();
    if (!((Object) portraitAnimation != (Object) null))
      return;
    UberShaderController shaderController = this.m_portraitObject.GetComponent<UberShaderController>();
    if ((Object) shaderController == (Object) null)
      shaderController = this.m_portraitObject.AddComponent<UberShaderController>();
    shaderController.UberShaderAnimation = Object.Instantiate<UberShaderAnimation>(portraitAnimation);
    shaderController.m_MaterialIndex = this.m_portraitMaterialIndex;
  }

  public enum CardDefMaterialType
  {
    CustomDeck,
    DeckCardBar,
  }
}
