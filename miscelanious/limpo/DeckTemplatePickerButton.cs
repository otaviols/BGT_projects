using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DeckTemplatePickerButton : PegUIElement
{
  public MeshRenderer m_deckTexture;
  public MeshRenderer m_packRibbon;
  public GameObject m_selectGlow;
  public UberText m_title;
  public List<UberText> m_cardCountTexts = new List<UberText>();
  public GameObject m_incompleteTextRibbon;
  public GameObject m_completeTextRibbon;
  public static readonly int s_MinimumRecommendedSize = 25;
  private bool m_isCoreDeck;
  private int m_ownedCardCount;
  private AssetHandle<Texture> m_deckArtTextureHandle;
  private Material m_deckArtMaterial;
  public MeshRenderer m_packFlap;
  public MeshRenderer m_packInner;
  public Material m_packGoldenMaterial;
  public Material m_packCustomMaterial;
  public GameObject m_runes;
  public Material m_runeNoneMaterial;
  public Material m_runeBloodSlotMaterial;
  public Material m_runeFrostSlotMaterial;
  public Material m_runeUnholySlotMaterial;
  public Material m_runeBloodStandardMaterial;
  public Material m_runeFrostStandardMaterial;
  public Material m_runeUnholyStandardMaterial;
  public Material m_runeBloodGoldenMaterial;
  public Material m_runeFrostGoldenMaterial;
  public Material m_runeUnholyGoldenMaterial;
  public MeshRenderer m_rune1;
  public MeshRenderer m_rune2;
  public MeshRenderer m_rune3;
  public MeshRenderer m_slot1;
  public MeshRenderer m_slot2;
  public MeshRenderer m_slot3;

  protected override void OnDestroy()
  {
    this.UnloadDeckArt();
    base.OnDestroy();
  }

  public void SetIsCoreDeck(bool isCore) => this.m_isCoreDeck = isCore;

  public bool IsCoreDeck() => this.m_isCoreDeck;

  public void SetSelected(bool selected)
  {
    if (!((Object) this.m_selectGlow != (Object) null))
      return;
    this.m_selectGlow.SetActive(selected);
  }

  public void SetTitleText(string titleText)
  {
    if (!((Object) this.m_title != (Object) null))
      return;
    this.m_title.Text = titleText;
  }

  public void SetCardCountText(int count, int total)
  {
    this.m_ownedCardCount = count;
    foreach (UberText cardCountText in this.m_cardCountTexts)
      cardCountText.Text = string.Format("{0}/{1}", (object) count, (object) total);
    bool flag = count < DeckTemplatePickerButton.s_MinimumRecommendedSize && !this.m_isCoreDeck;
    if ((Object) this.m_incompleteTextRibbon != (Object) null)
      this.m_incompleteTextRibbon.SetActive(flag);
    if (!((Object) this.m_completeTextRibbon != (Object) null))
      return;
    this.m_completeTextRibbon.SetActive(!flag);
  }

  public int GetOwnedCardCount() => this.m_ownedCardCount;

  public void SetDeckArtByMaterialPath(string materialPath, DeckTemplateDbfRecord record)
  {
    this.UnloadDeckArt();
    if ((Object) this.m_deckTexture == (Object) null || string.IsNullOrEmpty(materialPath))
      return;
    AssetLoader.Get().LoadMaterial((AssetReference) materialPath, new ObjectCallback(this.SetDeckMaterial));
    bool isPremium = false;
    Material material = this.m_packCustomMaterial;
    if (isPremium)
      material = this.m_packGoldenMaterial;
    RendererExtension.SetMaterial((Renderer) this.m_packFlap, material);
    RendererExtension.SetMaterial((Renderer) this.m_packInner, material);
    this.SetAllRuneMaterials(record, isPremium);
  }

  public void SetDeckArtByCardId(int cardId, Material sourceMaterial, DeckTemplateDbfRecord record)
  {
    this.UnloadDeckArt();
    if ((Object) this.m_deckTexture == (Object) null)
      return;
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(cardId))
    {
      if (cardDef == null)
        return;
      AssetHandle.Set<Texture>(ref this.m_deckArtTextureHandle, cardDef.CardDef.GetPortraitTextureHandle());
      this.m_deckArtMaterial = new Material(sourceMaterial);
      this.m_deckArtMaterial.mainTexture = (Texture) this.m_deckArtTextureHandle;
      RendererExtension.SetMaterial((Renderer) this.m_deckTexture, this.m_deckArtMaterial);
      bool isPremium = false;
      Material material = this.m_packCustomMaterial;
      if (isPremium)
        material = this.m_packGoldenMaterial;
      RendererExtension.SetMaterial((Renderer) this.m_packFlap, material);
      RendererExtension.SetMaterial((Renderer) this.m_packInner, material);
      this.SetAllRuneMaterials(record, isPremium);
    }
  }

  private void SetAllRuneMaterials(DeckTemplateDbfRecord record, bool isPremium = false)
  {
    if (record == null)
    {
      this.m_runes.SetActive(false);
    }
    else
    {
      List<DkRuneListDbfRecord> dkRunes = record.DKRunes;
      if (record.ClassId != 1 || dkRunes.Count == 0)
      {
        this.m_runes.SetActive(false);
      }
      else
      {
        this.m_runes.SetActive(true);
        if (dkRunes[0] != null)
          this.SetRuneMaterials(this.m_rune1, this.m_slot1, (RuneType) dkRunes[0].Rune, isPremium);
        if (dkRunes[1] != null)
          this.SetRuneMaterials(this.m_rune2, this.m_slot2, (RuneType) dkRunes[1].Rune, isPremium);
        if (dkRunes[2] == null)
          return;
        this.SetRuneMaterials(this.m_rune3, this.m_slot3, (RuneType) dkRunes[2].Rune, isPremium);
      }
    }
  }

  private void SetRuneMaterials(
    MeshRenderer runeMeshRenderer,
    MeshRenderer slotMeshRenderer,
    RuneType runeType,
    bool isPremium = false)
  {
    Material materialForRuneType1 = this.GetRuneMaterialForRuneType(runeType, isPremium);
    if ((bool) (Object) materialForRuneType1)
    {
      runeMeshRenderer.enabled = true;
      RendererExtension.SetMaterial((Renderer) runeMeshRenderer, materialForRuneType1);
    }
    else
      runeMeshRenderer.enabled = false;
    Material materialForRuneType2 = this.GetSlotMaterialForRuneType(runeType);
    if (!(bool) (Object) materialForRuneType2)
      return;
    RendererExtension.SetMaterial((Renderer) slotMeshRenderer, materialForRuneType2);
  }

  private Material GetRuneMaterialForRuneType(RuneType runeType, bool isPremium = false)
  {
    Material materialForRuneType = (Material) null;
    switch (runeType)
    {
      case RuneType.RT_NONE:
        return (Material) null;
      case RuneType.RT_BLOOD:
        materialForRuneType = isPremium ? this.m_runeBloodGoldenMaterial : this.m_runeBloodStandardMaterial;
        break;
      case RuneType.RT_FROST:
        materialForRuneType = isPremium ? this.m_runeFrostGoldenMaterial : this.m_runeFrostStandardMaterial;
        break;
      case RuneType.RT_UNHOLY:
        materialForRuneType = isPremium ? this.m_runeUnholyGoldenMaterial : this.m_runeUnholyStandardMaterial;
        break;
      default:
        Debug.LogError((object) "DeckTemplatePickerButton::GetMaterialForRuneType material for rune type not found.");
        break;
    }
    return materialForRuneType;
  }

  private Material GetSlotMaterialForRuneType(RuneType runeType)
  {
    Material materialForRuneType = (Material) null;
    switch (runeType)
    {
      case RuneType.RT_NONE:
        materialForRuneType = this.m_runeNoneMaterial;
        break;
      case RuneType.RT_BLOOD:
        materialForRuneType = this.m_runeBloodSlotMaterial;
        break;
      case RuneType.RT_FROST:
        materialForRuneType = this.m_runeFrostSlotMaterial;
        break;
      case RuneType.RT_UNHOLY:
        materialForRuneType = this.m_runeUnholySlotMaterial;
        break;
      default:
        Debug.LogError((object) "DeckTemplatePickerButton::GetSlotMaterialForRuneType material for rune type not found.");
        break;
    }
    return materialForRuneType;
  }

  private void SetDeckMaterial(AssetReference assetRef, Object obj, object callbackData)
  {
    Material material = obj as Material;
    if (!((Object) material != (Object) null))
      return;
    RendererExtension.SetMaterial((Renderer) this.m_deckTexture, material);
  }

  private void UnloadDeckArt()
  {
    if ((Object) this.m_deckArtMaterial != (Object) null)
    {
      Object.Destroy((Object) this.m_deckArtMaterial);
      this.m_deckArtMaterial = (Material) null;
    }
    AssetHandle.SafeDispose<Texture>(ref this.m_deckArtTextureHandle);
  }
}
