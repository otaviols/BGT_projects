using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

public class CollectionDeckTileActor : Actor
{
  public Material m_halfPremiumFrameMaterial;
  public Material m_premiumFrameMaterial;
  public Material m_halfNormalSignatureFrameMaterial;
  public Material m_halfGoldSignatureMaterial;
  public Material m_signatureFrameMaterial;
  public Material m_diamondFrameMaterial;
  public GameObject m_frame;
  public GameObject m_frameInterior;
  public GameObject m_uniqueStar;
  public GameObject m_multiCardIcon;
  public GameObject m_highlight;
  public GameObject m_highlightGlow;
  public UberText m_countText;
  public MeshRenderer m_manaGem;
  public MeshRenderer m_slider;
  [Tooltip("Normal Style Settings")]
  public CollectionDeckTileActor.DeckTileFrameColorSet m_normalColorSet = new CollectionDeckTileActor.DeckTileFrameColorSet();
  [Tooltip("Ghost Style Settings")]
  public CollectionDeckTileActor.DeckTileFrameColorSet m_ghostedColorSet = new CollectionDeckTileActor.DeckTileFrameColorSet();
  [Tooltip("Red Style Settings")]
  public CollectionDeckTileActor.DeckTileFrameColorSet m_redColorSet = new CollectionDeckTileActor.DeckTileFrameColorSet();
  [Tooltip("Not Included Style Settings")]
  public CollectionDeckTileActor.DeckTileFrameColorSet m_notIncludedColorSet = new CollectionDeckTileActor.DeckTileFrameColorSet();
  private const float SLIDER_ANIM_TIME = 0.35f;
  private const string NOT_INCLUDED_TEXT = "!";
  private UberText m_countTextMesh;
  private bool m_sliderIsOpen;
  private Vector3 m_originalSliderLocalPos;
  private Vector3 m_openSliderLocalPos;
  private CollectionDeckTileActor.GhostedState m_ghosted;
  private CollectionDeckSlot m_slot;
  private static readonly Vector3 CardNameTextDefaultPositionPC = new Vector3(1.2083f, 0.2267f, 0.0303f);
  private static readonly Vector3 CardNameTextDeathKnightPositionPC = new Vector3(0.8f, 0.2267f, 0.0303f);
  private const float CardNameTextDefaultWidthPC = 17.43f;
  private const float CardNameTextDeathKnightWidthPC = 16f;
  private static readonly Vector3 CardNameTextDefaultPositionPhone = new Vector3(5.24f, 0.23f, 0.03f);
  private static readonly Vector3 CardNameTextDeathKnightPositionPhone = new Vector3(4f, 0.23f, 0.03f);
  private const float CardNameTextDefaultWidthPhone = 8.42f;
  private const float CardNameTextDeathKnightWidthPhone = 8.42f;

  public override void Awake()
  {
    base.Awake();
    this.AssignSlider();
    this.AssignCardCount();
  }

  public static CollectionDeckTileActor.TileIconState GetCorrectTileIconState(
    bool isUnique,
    bool isMulticard)
  {
    if (isMulticard)
      return CollectionDeckTileActor.TileIconState.MULTI_CARD;
    return isUnique ? CollectionDeckTileActor.TileIconState.UNIQUE_STAR : CollectionDeckTileActor.TileIconState.CARD_COUNT;
  }

  public void UpdateDeckCardProperties(
    bool isUnique,
    bool isMultiCard,
    int numCards,
    bool useSliderAnimations)
  {
    this.UpdateDeckCardProperties(CollectionDeckTileActor.GetCorrectTileIconState(isUnique, isMultiCard), numCards, useSliderAnimations);
  }

  public void UpdateDeckCardProperties(
    CollectionDeckTileActor.TileIconState iconState,
    int numCards,
    bool useSliderAnimations)
  {
    if (this.m_ghosted == CollectionDeckTileActor.GhostedState.NOT_INCLUDED)
      iconState = CollectionDeckTileActor.TileIconState.CARD_COUNT;
    switch (iconState)
    {
      case CollectionDeckTileActor.TileIconState.CARD_COUNT:
        this.m_uniqueStar.SetActive(false);
        this.m_countTextMesh.gameObject.SetActive(this.m_shown);
        this.m_multiCardIcon.SetActive(false);
        this.m_countTextMesh.Text = this.m_ghosted == CollectionDeckTileActor.GhostedState.NOT_INCLUDED ? "!" : Convert.ToString(numCards);
        break;
      case CollectionDeckTileActor.TileIconState.UNIQUE_STAR:
        this.m_uniqueStar.SetActive(this.m_shown);
        this.m_countTextMesh.gameObject.SetActive(false);
        this.m_multiCardIcon.SetActive(false);
        break;
      case CollectionDeckTileActor.TileIconState.MULTI_CARD:
        this.m_uniqueStar.SetActive(false);
        this.m_countTextMesh.gameObject.SetActive(false);
        this.m_multiCardIcon.SetActive(this.m_shown);
        break;
    }
    if ((iconState == CollectionDeckTileActor.TileIconState.UNIQUE_STAR ? 1 : (numCards > 1 ? 1 : 0)) != 0)
      this.OpenSlider(useSliderAnimations);
    else
      this.CloseSlider(useSliderAnimations);
  }

  public void UpdateMaterial(Material material)
  {
    MeshRenderer component = this.m_portraitMesh.GetComponent<MeshRenderer>();
    if ((UnityEngine.Object) material == (UnityEngine.Object) null)
    {
      Debug.LogErrorFormat("Null portrait material specified for {0}", (object) this.GetEntityDef().GetCardId());
      Material material1 = component.GetMaterial();
      material1.SetFloat("_OffsetX", 0.0f);
      material1.SetFloat("_OffsetY", 0.0f);
    }
    else
      component.SetMaterial(material);
  }

  public void SetGhosted(CollectionDeckTileActor.GhostedState state) => this.m_ghosted = state;

  public override void SetPremium(TAG_PREMIUM premium)
  {
    base.SetPremium(premium);
    this.UpdateFrameMaterial();
  }

  public CollectionDeckSlot GetSlot() => this.m_slot;

  private static void GetCardNameTextPositionAndWidth(
    bool offsetForRunes,
    out Vector3 position,
    out float width)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (offsetForRunes)
      {
        position = CollectionDeckTileActor.CardNameTextDeathKnightPositionPhone;
        width = 8.42f;
      }
      else
      {
        position = CollectionDeckTileActor.CardNameTextDefaultPositionPhone;
        width = 8.42f;
      }
    }
    else if (offsetForRunes)
    {
      position = CollectionDeckTileActor.CardNameTextDeathKnightPositionPC;
      width = 16f;
    }
    else
    {
      position = CollectionDeckTileActor.CardNameTextDefaultPositionPC;
      width = 17.43f;
    }
  }

  public void SetSlot(CollectionDeckSlot slot) => this.m_slot = slot;

  public void UpdateGhostTileEffect()
  {
    if ((UnityEngine.Object) this.m_manaGem == (UnityEngine.Object) null)
      return;
    this.UpdateFrameMaterial();
    CollectionDeckTileActor.DeckTileFrameColorSet colorSet = this.GetColorSet(this.m_ghosted);
    this.m_manaGem.SetMaterial(colorSet.m_manaGemMaterial);
    this.m_countText.TextColor = colorSet.m_countTextColor;
    this.m_nameTextMesh.TextColor = colorSet.m_nameTextColor;
    this.m_costTextMesh.TextColor = colorSet.m_costTextColor;
    if (this.m_countText.Outline)
      this.m_countText.OutlineColor = colorSet.m_outlineColor;
    if (this.m_nameTextMesh.Outline)
      this.m_nameTextMesh.OutlineColor = colorSet.m_outlineColor;
    if (this.m_costTextMesh.Outline)
      this.m_costTextMesh.OutlineColor = colorSet.m_outlineColor;
    if ((bool) (UnityEngine.Object) this.m_highlight && (bool) (UnityEngine.Object) colorSet.m_highlightMaterial)
      this.m_highlight.GetComponent<Renderer>().SetMaterial(colorSet.m_highlightMaterial);
    if ((bool) (UnityEngine.Object) this.m_highlightGlow && (bool) (UnityEngine.Object) colorSet.m_highlightGlowMaterial)
      this.m_highlightGlow.GetComponent<Renderer>().SetMaterial(colorSet.m_highlightGlowMaterial);
    this.SetDesaturationAmount(this.GetPortraitMaterial(), colorSet);
    this.SetDesaturationAmount(this.m_uniqueStar.GetComponent<MeshRenderer>().GetMaterial(), colorSet);
    this.SetDesaturationAmount(this.m_multiCardIcon.GetComponent<MeshRenderer>().GetMaterial(), colorSet);
  }

  public void UpdateCardRuneBanner(EntityDef entityDef)
  {
    if (entityDef == null || !(bool) (UnityEngine.Object) this.m_cardRuneBanner)
      return;
    RunePattern runePattern = new RunePattern((EntityBase) entityDef);
    if (runePattern.HasRunes)
      this.m_cardRuneBanner.Show(runePattern);
    else
      this.m_cardRuneBanner.Hide();
  }

  public void UpdateNameTextForRuneBar(bool offsetCardNameForRunes)
  {
    Vector3 position;
    float width;
    CollectionDeckTileActor.GetCardNameTextPositionAndWidth(offsetCardNameForRunes, out position, out width);
    this.m_nameTextMesh.transform.localPosition = position;
    this.m_nameTextMesh.Width = width;
  }

  protected override bool IsPremiumPortraitEnabled() => false;

  public override void UpdateAllComponents(bool needsGhostUpdate = true)
  {
    base.UpdateAllComponents(needsGhostUpdate);
    if (this.m_premiumType != TAG_PREMIUM.SIGNATURE || !((UnityEngine.Object) this.DeckCardBarPortrait != (UnityEngine.Object) null))
      return;
    this.UpdateMaterial(this.DeckCardBarPortrait);
  }

  private void SetDesaturationAmount(
    Material material,
    CollectionDeckTileActor.DeckTileFrameColorSet colorSet)
  {
    material.SetColor("_Color", colorSet.m_desatColor);
    material.SetFloat("_Desaturate", colorSet.m_desatAmount);
    material.SetFloat("_Contrast", colorSet.m_desatContrast);
  }

  private void UpdateFrameMaterial()
  {
    CollectionDeckTileActor.DeckTileFrameColorSet colorSet = this.GetColorSet(this.m_ghosted);
    Material material = colorSet.m_frameMaterial;
    Material interiorFrameMaterial = colorSet.m_interiorFrameMaterial;
    if (this.m_ghosted == CollectionDeckTileActor.GhostedState.NONE)
    {
      if (this.m_slot != null)
      {
        int count1 = this.m_slot.GetCount(TAG_PREMIUM.NORMAL);
        int count2 = this.m_slot.GetCount(TAG_PREMIUM.GOLDEN);
        int count3 = this.m_slot.GetCount(TAG_PREMIUM.SIGNATURE);
        int count4 = this.m_slot.GetCount(TAG_PREMIUM.DIAMOND);
        if (count3 > 0)
          material = count2 <= 0 ? (count1 <= 0 ? this.m_signatureFrameMaterial : this.m_halfNormalSignatureFrameMaterial) : this.m_halfGoldSignatureMaterial;
        else if (count1 > 0 && count2 > 0)
          material = this.m_halfPremiumFrameMaterial;
        else if (count2 > 0 && count1 <= 0)
          material = this.m_premiumFrameMaterial;
        else if (count4 > 0)
          material = this.m_diamondFrameMaterial;
      }
      else if (this.m_premiumType == TAG_PREMIUM.GOLDEN)
        material = this.m_premiumFrameMaterial;
      else if (this.m_premiumType == TAG_PREMIUM.SIGNATURE)
        material = this.m_signatureFrameMaterial;
      else if (this.m_premiumType == TAG_PREMIUM.DIAMOND)
        material = this.m_diamondFrameMaterial;
    }
    if ((UnityEngine.Object) material != (UnityEngine.Object) null)
      this.m_frame.GetComponent<Renderer>().SetMaterial(material);
    if (!((UnityEngine.Object) interiorFrameMaterial != (UnityEngine.Object) null))
      return;
    this.m_frameInterior.GetComponent<Renderer>().SetMaterial(interiorFrameMaterial);
  }

  private void AssignSlider()
  {
    this.m_originalSliderLocalPos = this.m_slider.transform.localPosition;
    this.m_openSliderLocalPos = this.m_rootObject.transform.Find("OpenSliderPosition").transform.localPosition;
  }

  private void AssignCardCount() => this.m_countTextMesh = this.m_rootObject.transform.Find("CardCountText").GetComponent<UberText>();

  private void OpenSlider(bool useSliderAnimations)
  {
    if (this.m_sliderIsOpen)
      return;
    this.m_sliderIsOpen = true;
    iTween.StopByName(this.m_slider.gameObject, "position");
    if (useSliderAnimations)
      iTween.MoveTo(this.m_slider.gameObject, iTween.Hash((object) "position", (object) this.m_openSliderLocalPos, (object) "isLocal", (object) true, (object) "time", (object) 0.35f, (object) "easetype", (object) iTween.EaseType.easeOutBounce, (object) "name", (object) "position"));
    else
      this.m_slider.transform.localPosition = this.m_openSliderLocalPos;
  }

  private void CloseSlider(bool useSliderAnimations)
  {
    if (!this.m_sliderIsOpen)
      return;
    this.m_sliderIsOpen = false;
    iTween.StopByName(this.m_slider.gameObject, "position");
    if (useSliderAnimations)
      iTween.MoveTo(this.m_slider.gameObject, iTween.Hash((object) "position", (object) this.m_originalSliderLocalPos, (object) "isLocal", (object) true, (object) "time", (object) 0.35f, (object) "easetype", (object) iTween.EaseType.easeOutBounce, (object) "name", (object) "position"));
    else
      this.m_slider.transform.localPosition = this.m_originalSliderLocalPos;
  }

  private CollectionDeckTileActor.DeckTileFrameColorSet GetColorSet(
    CollectionDeckTileActor.GhostedState state)
  {
    switch (state)
    {
      case CollectionDeckTileActor.GhostedState.BLUE:
        return this.m_ghostedColorSet;
      case CollectionDeckTileActor.GhostedState.RED:
        return this.m_redColorSet;
      case CollectionDeckTileActor.GhostedState.NOT_INCLUDED:
        return this.m_notIncludedColorSet;
      default:
        return this.m_normalColorSet;
    }
  }

  [Serializable]
  public class DeckTileFrameColorSet
  {
    public Color m_desatColor = Color.white;
    public float m_desatContrast;
    public float m_desatAmount;
    public Color m_costTextColor = Color.white;
    public Color m_countTextColor = new Color(1f, 0.9f, 0.0f, 1f);
    public Color m_nameTextColor = Color.white;
    public Color m_sliderColor = new Color(0.62f, 0.62f, 0.62f, 1f);
    public Color m_outlineColor = Color.black;
    public Material m_frameMaterial;
    public Material m_interiorFrameMaterial;
    public Material m_highlightMaterial;
    public Material m_highlightGlowMaterial;
    public Material m_manaGemMaterial;
  }

  public enum GhostedState
  {
    NONE,
    BLUE,
    RED,
    NOT_INCLUDED,
  }

  public enum TileIconState
  {
    CARD_COUNT,
    UNIQUE_STAR,
    MULTI_CARD,
  }
}
