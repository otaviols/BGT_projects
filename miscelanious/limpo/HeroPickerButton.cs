using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPickerButton : PegUIElement
{
  public GameObject m_heroClassIcon;
  public GameObject m_heroClassIconSepia;
  public UberText m_classLabel;
  public GameObject m_labelGradient;
  public GameObject m_button;
  public GameObject m_buttonFrame;
  public TAG_CLASS m_heroClass;
  public List<Material> CLASS_MATERIALS = new List<Material>();
  public HeroPickerButtonBones m_bones;
  public TooltipZone m_tooltipZone;
  public GameObject m_crown;
  public UberText m_lockReasonText;
  public GameObject m_raiseAndLowerRoot;
  public GameObject m_heroClassIconOffset;
  protected DefLoader.DisposableFullDef m_fullDef;
  protected TAG_PREMIUM m_premium;
  protected float? m_seed;
  private bool m_isSelected;
  private HighlightState m_highlightState;
  private bool m_locked;
  private long m_preconDeckID;
  private Renderer m_buttonRenderer;
  private readonly List<Material> m_cachedMaterials = new List<Material>();
  private ILegendaryHeroPortrait m_legendaryHeroPortrait;
  private CustomFrameController m_customFrameController;
  private static readonly Color BASIC_SET_COLOR_IN_PROGRESS = new Color(0.97f, 0.82f, 0.22f);

  protected override void Awake()
  {
    base.Awake();
    if (!((UnityEngine.Object) this.m_buttonFrame != (UnityEngine.Object) null))
      return;
    this.m_buttonRenderer = this.m_buttonFrame.GetComponent<Renderer>();
    if (!((UnityEngine.Object) this.m_buttonRenderer != (UnityEngine.Object) null))
      return;
    this.m_buttonRenderer.GetSharedMaterials(this.m_cachedMaterials);
  }

  protected override void OnDestroy()
  {
    this.ReleaseFullDef();
    this.DestroyLegendaryHeroPortrait();
    this.DestroyCustomFrame();
    base.OnDestroy();
  }

  public void SetPreconDeckID(long preconDeckID) => this.m_preconDeckID = preconDeckID;

  public long GetPreconDeckID() => this.m_preconDeckID;

  public int HeroCardDbId => GameUtils.TranslateCardIdToDbId(this.m_fullDef?.EntityDef?.GetCardId());

  public virtual void UpdateDisplay(DefLoader.DisposableFullDef def, TAG_PREMIUM premium)
  {
    this.SetFullDef(def);
    this.SetPremium(premium);
  }

  public void SetClassIcon(Material mat)
  {
    Renderer component1 = this.m_heroClassIcon.GetComponent<Renderer>();
    RendererExtension.SetMaterial(component1, mat);
    RendererExtension.GetMaterial(component1).renderQueue = 3007;
    if (!((UnityEngine.Object) this.m_heroClassIconSepia != (UnityEngine.Object) null))
      return;
    Renderer component2 = this.m_heroClassIconSepia.GetComponent<Renderer>();
    RendererExtension.GetMaterial(component2).SetTextureOffset("_MainTex", RendererExtension.GetMaterial(component1).GetTextureOffset("_MainTex"));
    RendererExtension.GetMaterial(component2).SetTextureScale("_MainTex", RendererExtension.GetMaterial(component1).GetTextureScale("_MainTex"));
    RendererExtension.GetMaterial(component2).renderQueue = 3007;
  }

  public void SetClassname(string s) => this.m_classLabel.Text = s;

  public virtual GuestHeroDbfRecord GetGuestHero() => (GuestHeroDbfRecord) null;

  public void HideTextAndGradient()
  {
    this.m_classLabel.Hide();
    this.m_labelGradient.SetActive(false);
  }

  public void SetFullDef(DefLoader.DisposableFullDef def)
  {
    this.ReleaseFullDef();
    this.m_fullDef = def?.Share();
    this.UpdatePortrait();
  }

  public EntityDef GetEntityDef() => this.m_fullDef?.EntityDef;

  public DefLoader.DisposableCardDef ShareEntityDef() => this.m_fullDef?.DisposableCardDef?.Share();

  public DefLoader.DisposableFullDef ShareFullDef() => this.m_fullDef?.Share();

  public void SetSelected(bool isSelected)
  {
    this.m_isSelected = isSelected;
    if (isSelected)
      this.Lower();
    else
      this.Raise();
  }

  public bool IsSelected() => this.m_isSelected;

  public void SetLockReasonText(string text)
  {
    if ((UnityEngine.Object) this.m_lockReasonText == (UnityEngine.Object) null)
      return;
    this.m_lockReasonText.Text = text;
  }

  public void Lower()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.Activate(false);
    float num = !this.m_locked ? -0.7f : 0.7f;
    Vector3 vector3 = new Vector3(this.GetOriginalLocalPosition().x, this.GetOriginalLocalPosition().y + num, this.GetOriginalLocalPosition().z);
    if (this.m_customFrameController != null)
      vector3.y = Mathf.Max(vector3.y, this.m_customFrameController.RaiseAndLowerLimit);
    Hashtable args = iTween.Hash((object) "position", (object) vector3, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true);
    iTween.MoveTo((UnityEngine.Object) this.m_raiseAndLowerRoot != (UnityEngine.Object) null ? this.m_raiseAndLowerRoot : this.gameObject, args);
  }

  public void Raise()
  {
    if (this.m_isSelected)
      return;
    this.Activate(true);
    iTween.MoveTo(this.m_raiseAndLowerRoot, iTween.Hash((object) "position", (object) new Vector3(this.GetOriginalLocalPosition().x, this.GetOriginalLocalPosition().y, this.GetOriginalLocalPosition().z), (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));
  }

  public void SetHighlightState(ActorStateType stateType)
  {
    if ((UnityEngine.Object) this.m_highlightState == (UnityEngine.Object) null)
      this.m_highlightState = this.GetComponentInChildren<HighlightState>();
    if (!((UnityEngine.Object) this.m_highlightState != (UnityEngine.Object) null))
      return;
    this.m_highlightState.ChangeState(stateType);
  }

  public void Activate(bool enable) => this.SetEnabled(enable);

  public virtual void Lock() => this.m_locked = true;

  public virtual void Unlock() => this.m_locked = false;

  public bool IsLocked() => this.m_locked;

  public TAG_PREMIUM GetPremium() => this.m_premium;

  public void SetPremium(TAG_PREMIUM premium)
  {
    this.m_premium = premium;
    this.UpdatePortrait();
  }

  public HeroPickerOptionDataModel GetDataModel()
  {
    WidgetTemplate component = this.GetComponent<WidgetTemplate>();
    IDataModel model = (IDataModel) null;
    if ((UnityEngine.Object) component != (UnityEngine.Object) null && !component.GetDataModel(6, out model))
    {
      model = (IDataModel) new HeroPickerOptionDataModel();
      component.BindDataModel(model, false);
    }
    return model as HeroPickerOptionDataModel;
  }

  public bool HasCardDef => (UnityEngine.Object) this.m_fullDef?.CardDef != (UnityEngine.Object) null;

  public string HeroPickerSelectedPrefab => !this.HasCardDef ? (string) null : this.m_fullDef.CardDef.m_HeroPickerSelectedPrefab;

  protected Material GetClassIconMaterial(TAG_CLASS classTag)
  {
    int index = 0;
    switch (classTag)
    {
      case TAG_CLASS.INVALID:
      case TAG_CLASS.NEUTRAL:
        index = 11;
        break;
      case TAG_CLASS.DEATHKNIGHT:
        index = 10;
        break;
      case TAG_CLASS.DRUID:
        index = 5;
        break;
      case TAG_CLASS.HUNTER:
        index = 4;
        break;
      case TAG_CLASS.MAGE:
        index = 7;
        break;
      case TAG_CLASS.PALADIN:
        index = 3;
        break;
      case TAG_CLASS.PRIEST:
        index = 8;
        break;
      case TAG_CLASS.ROGUE:
        index = 2;
        break;
      case TAG_CLASS.SHAMAN:
        index = 1;
        break;
      case TAG_CLASS.WARLOCK:
        index = 6;
        break;
      case TAG_CLASS.WARRIOR:
        index = 0;
        break;
      case TAG_CLASS.DEMONHUNTER:
        index = 9;
        break;
    }
    return this.CLASS_MATERIALS[index];
  }

  protected virtual void UpdatePortrait()
  {
    if (this.UpdateLegendaryHeroPortrait())
      return;
    CardDef cardDef = this.m_fullDef?.CardDef;
    if ((UnityEngine.Object) cardDef == (UnityEngine.Object) null)
      return;
    Material deckPickerPortrait = cardDef.GetDeckPickerPortrait();
    if ((UnityEngine.Object) deckPickerPortrait == (UnityEngine.Object) null)
      return;
    DeckPickerHero component1 = this.GetComponent<DeckPickerHero>();
    Renderer component2 = component1.m_PortraitMesh.GetComponent<Renderer>();
    List<Material> materials = RendererExtension.GetMaterials(component2);
    Material portraitMaterial = cardDef.GetPremiumPortraitMaterial();
    if (this.m_premium == TAG_PREMIUM.GOLDEN && (UnityEngine.Object) portraitMaterial != (UnityEngine.Object) null)
    {
      materials[component1.m_PortraitMaterialIndex] = UnityEngine.Object.Instantiate<Material>(portraitMaterial);
      materials[component1.m_PortraitMaterialIndex].mainTextureOffset = deckPickerPortrait.mainTextureOffset;
      materials[component1.m_PortraitMaterialIndex].mainTextureScale = deckPickerPortrait.mainTextureScale;
      materials[component1.m_PortraitMaterialIndex].SetTexture("_ShadowTex", (Texture) null);
      if (!this.m_seed.HasValue)
        this.m_seed = new float?(UnityEngine.Random.value);
      Material material = RendererExtension.GetMaterial(component2);
      if (material.HasProperty("_Seed"))
        material.SetFloat("_Seed", this.m_seed.Value);
    }
    else
    {
      Material cachedMaterial = this.GetCachedMaterial(component1.m_PortraitMaterialIndex);
      if ((UnityEngine.Object) cachedMaterial != (UnityEngine.Object) null)
        materials[component1.m_PortraitMaterialIndex] = UnityEngine.Object.Instantiate<Material>(cachedMaterial);
      materials[component1.m_PortraitMaterialIndex] = deckPickerPortrait;
    }
    RendererExtension.SetMaterials(component2, materials);
    if (!(bool) (UnityEngine.Object) cardDef.GetPremiumPortraitAnimation())
      return;
    UberShaderController shaderController = component1.m_PortraitMesh.GetComponent<UberShaderController>();
    if ((UnityEngine.Object) shaderController == (UnityEngine.Object) null)
      shaderController = component1.m_PortraitMesh.AddComponent<UberShaderController>();
    shaderController.UberShaderAnimation = UnityEngine.Object.Instantiate<UberShaderAnimation>(cardDef.GetPremiumPortraitAnimation());
    shaderController.m_MaterialIndex = 0;
  }

  protected bool UpdateLegendaryHeroPortrait()
  {
    if ((UnityEngine.Object) this.m_fullDef?.CardDef == (UnityEngine.Object) null)
    {
      this.DestroyLegendaryHeroPortrait();
      this.UnloadCustomFrame();
      return false;
    }
    if (string.IsNullOrEmpty(this.m_fullDef.CardDef.m_CustomHeroFramePrefab))
    {
      this.DestroyLegendaryHeroPortrait();
      this.UnloadCustomFrame();
      return false;
    }
    this.UpdateLegendaryCardArt(this.m_fullDef.CardDef);
    this.LoadCustomFrame(this.m_fullDef.CardDef);
    return this.m_legendaryHeroPortrait != null;
  }

  private void LoadCustomFrame(CardDef cardDef)
  {
    if ((UnityEngine.Object) cardDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(cardDef.m_CustomHeroFramePrefab))
    {
      AssetReference customHeroFramePrefab = (AssetReference) cardDef.m_CustomHeroFramePrefab;
      if (this.m_customFrameController == null || this.m_customFrameController.FrameAssetReference != customHeroFramePrefab)
      {
        this.UnloadCustomFrame();
        IAssetLoader assetLoader = AssetLoader.Get();
        if (assetLoader == null)
          return;
        using (AssetHandle<GameObject> instantiateSharedPrefab = assetLoader.GetOrInstantiateSharedPrefab(customHeroFramePrefab))
          this.OnCustomFrameLoaded(customHeroFramePrefab, instantiateSharedPrefab, (object) null);
      }
      else
      {
        if (this.m_customFrameController == null)
          return;
        this.ApplyCustomFrame();
      }
    }
    else
      this.UnloadCustomFrame();
  }

  private void UnloadCustomFrame()
  {
    if (this.m_customFrameController != null)
      this.m_customFrameController.RestoreMeshAndMaterials(ref this.m_buttonFrame);
    if (!((UnityEngine.Object) this.m_heroClassIconOffset != (UnityEngine.Object) null))
      return;
    this.m_heroClassIconOffset.transform.localPosition = Vector3.zero;
  }

  private void DestroyCustomFrame()
  {
    this.UnloadCustomFrame();
    if (this.m_customFrameController == null)
      return;
    ((IDisposable) this.m_customFrameController).Dispose();
    this.m_customFrameController = (CustomFrameController) null;
  }

  private void ApplyCustomFrame()
  {
    if (this.m_customFrameController == null)
      return;
    this.m_customFrameController.ApplyCustomMeshAndMaterials(out this.m_buttonFrame);
    if ((UnityEngine.Object) this.m_heroClassIconOffset != (UnityEngine.Object) null)
      this.m_heroClassIconOffset.transform.localPosition = new Vector3(0.0f, -this.m_customFrameController.HeroClassIconOffset, 0.0f);
    Material sharedMaterial = RendererExtension.GetSharedMaterial(this.m_buttonFrame.GetComponent<Renderer>(), this.m_customFrameController.PortraitMatIdx);
    if (this.m_legendaryHeroPortrait != null)
    {
      sharedMaterial.mainTexture = this.m_legendaryHeroPortrait.PortraitTexture;
      this.ConnectLegendarySkinToDynamicResolutionController();
    }
    else
    {
      CardDef cardDef = this.m_fullDef?.CardDef;
      if (!((UnityEngine.Object) cardDef != (UnityEngine.Object) null))
        return;
      Material deckPickerPortrait = cardDef.GetDeckPickerPortrait();
      if (!((UnityEngine.Object) deckPickerPortrait != (UnityEngine.Object) null))
        return;
      sharedMaterial.mainTexture = deckPickerPortrait.mainTexture;
      sharedMaterial.mainTextureOffset = deckPickerPortrait.mainTextureOffset;
      sharedMaterial.mainTextureScale = deckPickerPortrait.mainTextureScale;
    }
  }

  private void OnCustomFrameLoaded(
    AssetReference assetRef,
    AssetHandle<GameObject> go,
    object callbackData)
  {
    using (go)
    {
      if (go == null || (UnityEngine.Object) go.Asset == (UnityEngine.Object) null)
        Debug.LogError((object) string.Format("{0} - HeroPickerButton.OnCustomFrameLoaded() - failed to load legendary hero skin! GameObject = null!", (object) assetRef));
      else if ((UnityEngine.Object) go.Asset.GetComponent<CustomFrameDef>() == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("{0} - HeroPickerButton.OnCustomFrameLoaded() - failed to load legendary hero skin! CustomFrameDef = null!", (object) assetRef));
      }
      else
      {
        if (this.m_customFrameController == null)
          this.m_customFrameController = new CustomFrameController(this.m_buttonFrame);
        this.m_customFrameController.SetAssetHandle(assetRef, go);
        this.m_customFrameController.CacheHighlightState(this.GetComponentInChildren<HighlightState>());
        this.ApplyCustomFrame();
      }
    }
  }

  protected override void OnRelease() => this.Lower();

  protected void ReleaseFullDef()
  {
    this.m_fullDef?.Dispose();
    this.m_fullDef = (DefLoader.DisposableFullDef) null;
  }

  public void SetDivotTexture(Texture texture) => RendererExtension.GetMaterial((Renderer) this.GetComponent<DeckPickerHero>().m_DivotMesh).mainTexture = texture;

  public void SetDivotVisible(bool visible) => this.GetComponent<DeckPickerHero>().m_DivotMesh.gameObject.SetActive(visible);

  protected Material GetCachedMaterial(int materialIdx) => this.m_cachedMaterials != null && materialIdx < this.m_cachedMaterials.Count ? this.m_cachedMaterials[materialIdx] : (Material) null;

  public void UpdateLegendaryCardArt(CardDef cardDef)
  {
    if ((UnityEngine.Object) cardDef == (UnityEngine.Object) null)
      return;
    IGraphicsManager graphicsManager = ServiceManager.Get<IGraphicsManager>();
    if (graphicsManager != null && graphicsManager.isVeryLowQualityDevice())
      return;
    string legendaryModel = cardDef.m_LegendaryModel;
    if (!string.IsNullOrEmpty(legendaryModel))
    {
      if (this.m_legendaryHeroPortrait != null && !this.m_legendaryHeroPortrait.IsValidForPath(legendaryModel, Player.Side.NEUTRAL))
        this.DestroyLegendaryHeroPortrait();
      if (this.m_legendaryHeroPortrait != null)
        return;
      LegendaryHeroRenderToTextureService toTextureService = ServiceManager.Get<LegendaryHeroRenderToTextureService>();
      if (toTextureService == null)
        return;
      this.m_legendaryHeroPortrait = toTextureService.CreatePortrait(legendaryModel, Player.Side.NEUTRAL);
    }
    else
      this.DestroyLegendaryHeroPortrait();
  }

  private void DestroyLegendaryHeroPortrait()
  {
    if (this.m_legendaryHeroPortrait == null)
      return;
    this.m_legendaryHeroPortrait.Dispose();
    this.m_legendaryHeroPortrait = (ILegendaryHeroPortrait) null;
  }

  private void ConnectLegendarySkinToDynamicResolutionController()
  {
    if (this.m_customFrameController == null)
      return;
    LegendarySkinDynamicResController resolutionController = this.m_customFrameController.DynamicResolutionController;
    if (this.m_legendaryHeroPortrait != null)
      this.m_legendaryHeroPortrait.ConnectDynamicResolutionController(resolutionController);
    else
      resolutionController.Skin = (LegendarySkin) null;
  }
}
