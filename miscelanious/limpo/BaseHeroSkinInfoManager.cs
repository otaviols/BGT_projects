using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.UI;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public abstract class BaseHeroSkinInfoManager : MonoBehaviour, IStore
{
  protected const string STATE_SHOW_VANILLA_HERO = "SHOW_VANILLA_HERO";
  protected const string STATE_SHOW_NEW_HERO = "SHOW_NEW_HERO";
  protected const string STATE_SHOW_CUSTOM_HERO = "SHOW_CUSTOM_HERO";
  protected const string STATE_INVALID_HERO = "INVALID_HERO";
  protected const string STATE_HIDDEN = "HIDDEN";
  protected const string MAKE_FAVORITE_STATE = "MAKE_FAVORITE";
  protected const string SUFFICIENT_CURRENCY_STATE = "SUFFICIENT_CURRENCY";
  protected const string INSUFFICIENT_CURRENCY_STATE = "INSUFFICIENT_CURRENCY";
  protected const string DISABLED_STATE = "DISABLED";
  protected const string STATE_BLOCK_SCREEN = "BLOCK_SCREEN";
  protected const string STATE_UNBLOCK_SCREEN = "UNBLOCK_SCREEN";
  public GameObject m_previewPane;
  public GameObject m_vanillaHeroFrame;
  public MeshRenderer m_vanillaHeroPreviewQuad;
  public UberText m_vanillaHeroTitle;
  public UberText m_vanillaHeroDescription;
  public UIBButton m_vanillaHeroFavoriteButton;
  public UIBButton m_vanillaHeroBuyButton;
  public GameObject m_newHeroFrame;
  public MeshRenderer m_newHeroPreviewQuad;
  public UberText m_newHeroTitle;
  public UberText m_newHeroDescription;
  public UIBButton m_newHeroFavoriteButton;
  public UIBButton m_newHeroBuyButton;
  public GameObject m_customHeroFrameRoot;
  protected GameObject m_customHeroFrameInstance;
  public PegUIElement m_offClicker;
  public float m_animationTime = 0.5f;
  public Material m_defaultPreviewMaterial;
  public Material m_vanillaHeroNonPremiumMaterial;
  public AsyncReference m_visibilityVisualControllerReference;
  public AsyncReference m_userActionVisualControllerReference;
  public AsyncReference m_vanillaHeroCurrencyIconWidgetReference;
  public AsyncReference m_newHeroCurrencyIconWidgetReference;
  public AsyncReference m_fullScreenBlockerWidgetReference;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_enterPreviewSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_exitPreviewSound;
  public MusicPlaylistType m_defaultHeroMusic = MusicPlaylistType.UI_CMHeroSkinPreview;
  protected WidgetTemplate m_widget;
  protected string m_currentCardId;
  protected DefLoader.DisposableCardDef m_currentHeroCardDef;
  protected CollectionHeroDef m_currentHeroDef;
  protected AssetHandle<UberShaderAnimation> m_currentHeroGoldenAnimation;
  protected CardHeroDbfRecord m_currentHeroRecord;
  protected EntityDef m_currentEntityDef;
  protected TAG_PREMIUM m_currentPremium;
  protected bool m_animating;
  protected bool m_hasEnteredHeroSkinPreview;
  protected MusicPlaylistType m_prevPlaylist;
  protected string m_desiredVisibilityState = "INVALID_HERO";
  protected VisualController m_visibilityVisualController;
  protected VisualController m_userActionVisualController;
  protected Widget m_fullScreenBlockerWidget;
  protected Widget m_vanillaHeroCurrencyButtonWidget;
  protected Widget m_newHeroCurrencyButtonWidget;
  protected bool m_isStoreOpen;
  protected bool m_isStoreTransactionActive;
  private readonly HashSet<CurrencyType> m_activeHeroCardCurrencyTypes = new HashSet<CurrencyType>()
  {
    CurrencyType.GOLD
  };
  private ScreenEffectsHandle m_screenEffectsHandle;

  public event Action OnOpened;

  public event Action<StoreClosedArgs> OnClosed;

  public event Action OnReady;

  public event Action<BuyProductEventArgs> OnProductPurchaseAttempt;

  public event Action OnProductOpened;

  public bool IsShowingPreview => this.m_animating || this.m_hasEnteredHeroSkinPreview;

  protected virtual void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_previewPane.SetActive(false);
    this.SetupUI();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Start()
  {
    this.m_visibilityVisualControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnVisibilityVisualControllerReady));
    this.m_userActionVisualControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnUserActionVisualControllerReady));
    this.m_fullScreenBlockerWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnFullScreenBlockerWidgetReady));
    this.m_vanillaHeroCurrencyIconWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnVanillaHeroCurrencyButtonWidgetReady));
    this.m_newHeroCurrencyIconWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnNewHeroCurrencyButtonWidgetReady));
  }

  public virtual void EnterPreview(CollectionCardVisual cardVisual)
  {
    if (this.m_animating)
      return;
    this.m_activeHeroCardCurrencyTypes.Clear();
    BnetBar.Get()?.RefreshCurrency();
    Action onProductOpened = this.OnProductOpened;
    if (onProductOpened != null)
      onProductOpened();
    Actor actor = cardVisual.GetActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("BaseHeroSkinInfoManager.EnterPreview - Could not get actor from card visual. Not displaying preview");
    }
    else
    {
      this.m_currentEntityDef = actor.GetEntityDef();
      this.m_currentPremium = actor.GetPremium();
      if (this.m_currentEntityDef == null)
      {
        Log.CollectionManager.PrintError("BaseHeroSkinInfoManager.EnterPreview - Actor entity def not set. Not displaying preview");
      }
      else
      {
        CollectionManagerDisplay collectibleDisplay = CollectionManager.Get()?.GetCollectibleDisplay() as CollectionManagerDisplay;
        if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
          collectibleDisplay.HideHeroTips();
        CardDataModel cardDataModel1 = new CardDataModel();
        cardDataModel1.CardId = this.m_currentCardId;
        CardDataModel cardDataModel2 = cardDataModel1;
        string str;
        if (!string.IsNullOrWhiteSpace(this.m_currentEntityDef.GetArtistName(this.m_currentPremium)))
          str = GameStrings.Format("GLUE_COLLECTION_ARTIST", (object) this.m_currentEntityDef.GetArtistName(this.m_currentPremium));
        else
          str = string.Empty;
        cardDataModel2.ArtistCredit = str;
        this.m_widget.BindDataModel((IDataModel) cardDataModel1, false);
        this.PushNavigateBack();
        string cardId = this.m_currentEntityDef.GetCardId();
        this.m_currentHeroRecord = GameDbf.CardHero.GetRecords().Find((Predicate<CardHeroDbfRecord>) (r => GameUtils.TranslateDbIdToCardId(r.CardId) == cardId));
        int num1 = CollectionManager.GetHeroCardId(this.m_currentEntityDef.GetClass(), CardHero.HeroType.HONORED) == cardId ? 1 : 0;
        bool flag1 = GameUtils.IsVanillaHero(cardId);
        bool flag2 = this.m_currentHeroRecord.HeroType == CardHero.HeroType.BATTLEGROUNDS_HERO;
        bool flag3 = actor.PremiumAnimationAvailable && flag1 && this.m_currentPremium == TAG_PREMIUM.GOLDEN || !flag1;
        int num2 = flag1 ? 1 : 0;
        bool flag4 = (num1 | num2 | (flag2 ? 1 : 0)) != 0;
        if (this.LoadHeroDef(cardId))
        {
          if (this.m_currentHeroDef.m_collectionManagerPreviewEmote != EmoteType.INVALID)
            GameUtils.LoadCardDefEmoteSound(cardVisual.GetActor().EmoteDefs, this.m_currentHeroDef.m_collectionManagerPreviewEmote, (GameUtils.EmoteSoundLoaded) (cardSpell =>
            {
              if (!((UnityEngine.Object) cardSpell != (UnityEngine.Object) null))
                return;
              cardSpell.AddFinishedCallback((Spell.FinishedCallback) ((spell, data) => UnityEngine.Object.Destroy((UnityEngine.Object) cardSpell.gameObject)));
              cardSpell.Reactivate();
            }));
          bool flag5 = flag4 || !this.m_currentHeroDef.m_previewMaterial.IsInitialized();
          if ((bool) (UnityEngine.Object) this.m_customHeroFrameInstance)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_customHeroFrameInstance);
            this.m_customHeroFrameInstance = (GameObject) null;
          }
          if ((UnityEngine.Object) this.m_customHeroFrameRoot != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.m_currentHeroCardDef.CardDef.m_CustomHeroInfoFramePrefab))
            this.m_customHeroFrameInstance = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_currentHeroCardDef.CardDef.m_CustomHeroInfoFramePrefab);
          if ((UnityEngine.Object) this.m_customHeroFrameInstance != (UnityEngine.Object) null)
          {
            this.m_customHeroFrameInstance.transform.SetParent(this.m_customHeroFrameRoot.transform);
            TransformUtil.Identity(this.m_customHeroFrameInstance);
            CustomFrameButtonReskinController component1 = this.m_customHeroFrameRoot.GetComponent<CustomFrameButtonReskinController>();
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
            {
              CustomFrameButtonReskinData component2 = this.m_customHeroFrameInstance.GetComponent<CustomFrameButtonReskinData>();
              component1.UpdateMaterials(component2);
            }
            this.m_vanillaHeroTitle.Text = this.m_currentEntityDef.GetName();
            this.m_vanillaHeroDescription.Text = (string) this.m_currentHeroRecord.Description;
            this.m_desiredVisibilityState = "SHOW_CUSTOM_HERO";
          }
          else
          {
            if ((flag3 ? (UnityEngine.Object) cardVisual.GetActor().PremiumPortraitMaterial : (UnityEngine.Object) null) == (UnityEngine.Object) null)
            {
              this.m_currentHeroDef.m_previewMaterial.GetMaterial();
              flag5 = true;
            }
            Material material1 = (Material) null;
            if (!flag5)
              material1 = this.m_currentHeroDef.m_previewMaterial.GetMaterial();
            Texture portraitTexture = actor.LegendaryHeroPortrait == null ? actor.GetPortraitTexture() : actor.GetStaticPortraitTexture();
            bool flag6 = flag5 || (UnityEngine.Object) material1 == (UnityEngine.Object) null;
            if (!flag6)
            {
              string shaderAnimationPath = this.m_currentHeroDef.GetHeroUberShaderAnimationPath();
              int num3 = !string.IsNullOrWhiteSpace(shaderAnimationPath) ? 1 : 0;
              if (num3 != 0)
              {
                AssetHandle.SafeDispose<UberShaderAnimation>(ref this.m_currentHeroGoldenAnimation);
                AssetLoader.Get().LoadAsset<UberShaderAnimation>(ref this.m_currentHeroGoldenAnimation, (AssetReference) shaderAnimationPath);
              }
              if (num3 != 0 && this.m_currentHeroGoldenAnimation == null)
                Debug.LogWarning((object) string.Format("BaseHeroSkinInfoManager.EnterPreview - {0} hero shader could not be loaded {1}", (object) cardId, (object) shaderAnimationPath));
            }
            if (flag6)
            {
              this.m_vanillaHeroTitle.Text = this.m_currentEntityDef.GetName();
              this.m_vanillaHeroDescription.Text = (string) this.m_currentHeroRecord.Description;
              Material material2 = this.m_vanillaHeroNonPremiumMaterial;
              if (flag3)
              {
                Material portraitMaterial = cardVisual.GetActor().PremiumPortraitMaterial;
                if ((UnityEngine.Object) portraitMaterial != (UnityEngine.Object) null)
                {
                  material2 = portraitMaterial;
                  portraitTexture = material2.mainTexture;
                }
                else
                  Log.CollectionManager.PrintWarning(string.Format("BaseHeroSkinInfoManager.EnterPreview - premium material missing for {0}", (object) cardId));
              }
              this.AssignVanillaHeroPreviewMaterial(material2, portraitTexture, cardVisual.GetActor().PremiumPortraitAnimation, cardVisual.GetActor().m_portraitMatIdx);
            }
            else
            {
              this.m_newHeroTitle.Text = this.m_currentEntityDef.GetName();
              this.m_newHeroDescription.Text = (string) this.m_currentHeroRecord.Description;
              this.AssignNewHeroPreviewMaterial(material1, portraitTexture, (UberShaderAnimation) this.m_currentHeroGoldenAnimation);
            }
            this.m_desiredVisibilityState = flag6 ? "SHOW_VANILLA_HERO" : "SHOW_NEW_HERO";
          }
          this.m_hasEnteredHeroSkinPreview = true;
          this.m_previewPane.SetActive(true);
          this.m_offClicker.gameObject.SetActive(true);
          this.m_animating = true;
          iTween.ScaleFrom(this.m_previewPane, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) (Action<object>) (e => this.m_animating = false)));
          this.SetupHeroSkinStore();
          this.UpdateView();
          if (!string.IsNullOrEmpty(this.m_enterPreviewSound))
            SoundManager.Get().LoadAndPlay((AssetReference) this.m_enterPreviewSound);
          this.PlayHeroMusic();
          this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
          {
            Time = this.m_animationTime
          });
        }
        else
        {
          Debug.LogError((object) "Could not load entity def for hero skin, preview will not be shown. Set the CollectionHeroDefPath on the HERO_0X.prefab.");
          this.m_desiredVisibilityState = "INVALID_HERO";
          this.SetupHeroSkinStore();
          this.UpdateView();
        }
      }
    }
  }

  public void CancelPreview()
  {
    this.RemoveNavigateBack();
    if (this.m_animating || !this.m_hasEnteredHeroSkinPreview)
      return;
    this.m_hasEnteredHeroSkinPreview = false;
    this.ShutDownHeroSkinStore();
    Vector3 origScale = this.m_previewPane.transform.localScale;
    this.m_animating = true;
    iTween.ScaleTo(this.m_previewPane, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) (Action<object>) (e =>
    {
      this.m_animating = false;
      if ((bool) (UnityEngine.Object) this.m_previewPane)
      {
        this.m_previewPane.transform.localScale = origScale;
        this.m_previewPane.SetActive(false);
      }
      if ((bool) (UnityEngine.Object) this.m_offClicker)
        this.m_offClicker.gameObject.SetActive(false);
      if (!((UnityEngine.Object) this.m_customHeroFrameInstance != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_customHeroFrameInstance);
      this.m_customHeroFrameInstance = (GameObject) null;
    })));
    if (!string.IsNullOrEmpty(this.m_exitPreviewSound))
      SoundManager.Get()?.LoadAndPlay((AssetReference) this.m_exitPreviewSound);
    this.StopHeroMusic();
    this.m_screenEffectsHandle.StopEffect();
  }

  protected void OnVisibilityVisualControllerReady(VisualController visualController)
  {
    this.m_visibilityVisualController = visualController;
    this.UpdateView();
    if (this.OnReady == null)
      return;
    this.OnReady();
  }

  protected void OnUserActionVisualControllerReady(VisualController visualController)
  {
    this.m_userActionVisualController = visualController;
    this.UpdateView();
  }

  protected void OnFullScreenBlockerWidgetReady(Widget fullScreenBlockerWidget)
  {
    this.m_fullScreenBlockerWidget = fullScreenBlockerWidget;
    this.UpdateView();
  }

  protected void OnVanillaHeroCurrencyButtonWidgetReady(Widget currencyButtonWidget)
  {
    this.m_vanillaHeroCurrencyButtonWidget = currencyButtonWidget;
    this.UpdateView();
  }

  protected void OnNewHeroCurrencyButtonWidgetReady(Widget currencyButtonWidget)
  {
    this.m_newHeroCurrencyButtonWidget = currencyButtonWidget;
    this.UpdateView();
  }

  public void OnFavoriteHeroChanged(
    TAG_CLASS heroClass,
    NetCache.CardDefinition favoriteHero,
    bool isFavorite,
    object userData)
  {
    this.UpdateFavoriteButton();
  }

  protected abstract bool CanToggleFavorite();

  protected void UpdateFavoriteButton()
  {
    bool flag = this.CanToggleFavorite();
    UIBButton uibButton = this.m_desiredVisibilityState == "SHOW_VANILLA_HERO" ? this.m_vanillaHeroFavoriteButton : this.m_newHeroFavoriteButton;
    if (uibButton.IsEnabled() == flag)
      return;
    uibButton.SetEnabled(flag);
    uibButton.Flip(flag);
  }

  protected void UpdateView()
  {
    if ((UnityEngine.Object) this.m_visibilityVisualController == (UnityEngine.Object) null || (UnityEngine.Object) this.m_userActionVisualController == (UnityEngine.Object) null || (UnityEngine.Object) this.m_fullScreenBlockerWidget == (UnityEngine.Object) null || (UnityEngine.Object) this.m_vanillaHeroCurrencyButtonWidget == (UnityEngine.Object) null || (UnityEngine.Object) this.m_newHeroCurrencyButtonWidget == (UnityEngine.Object) null || this.m_currentEntityDef == null || this.m_currentHeroRecord == null || string.IsNullOrEmpty(this.m_currentCardId))
      return;
    this.m_activeHeroCardCurrencyTypes.Clear();
    this.m_activeHeroCardCurrencyTypes.Add(CurrencyType.GOLD);
    this.m_visibilityVisualController.SetState(this.m_desiredVisibilityState);
    bool enabled = false;
    if (HeroSkinUtils.IsHeroSkinOwned(this.m_currentEntityDef.GetCardId()))
      this.m_userActionVisualController.SetState("MAKE_FAVORITE");
    else if (HeroSkinUtils.IsHeroSkinPurchasableFromCollectionManager(this.m_currentEntityDef.GetCardId()))
    {
      PriceDataModel skinPriceDataModel = HeroSkinUtils.GetCollectionManagerHeroSkinPriceDataModel(this.m_currentEntityDef.GetCardId());
      if (skinPriceDataModel != null)
      {
        if (skinPriceDataModel.Currency != CurrencyType.NONE && skinPriceDataModel.Currency != CurrencyType.REAL_MONEY)
          this.m_activeHeroCardCurrencyTypes.Add(skinPriceDataModel.Currency);
        this.m_userActionVisualController.BindDataModel((IDataModel) skinPriceDataModel);
        if (!HeroSkinUtils.CanBuyHeroSkinFromCollectionManager(this.m_currentEntityDef.GetCardId(), skinPriceDataModel.Currency, skinPriceDataModel))
        {
          this.m_userActionVisualController.SetState("INSUFFICIENT_CURRENCY");
        }
        else
        {
          this.m_userActionVisualController.SetState("SUFFICIENT_CURRENCY");
          enabled = true;
        }
      }
      else
        this.m_userActionVisualController.SetState("DISABLED");
    }
    else
      this.m_userActionVisualController.SetState("DISABLED");
    BnetBar.Get()?.RefreshCurrency();
    this.UpdateFavoriteButton();
    UIBButton uibButton = this.m_desiredVisibilityState == "SHOW_VANILLA_HERO" ? this.m_vanillaHeroBuyButton : this.m_newHeroBuyButton;
    uibButton.SetEnabled(enabled);
    uibButton.Flip(true);
  }

  protected abstract void PushNavigateBack();

  protected abstract void RemoveNavigateBack();

  protected bool LoadHeroDef(string cardId)
  {
    if (this.m_currentCardId == cardId && string.IsNullOrEmpty(cardId))
      return true;
    this.m_currentHeroCardDef?.Dispose();
    this.m_currentHeroCardDef = DefLoader.Get().GetCardDef(cardId);
    if ((UnityEngine.Object) this.m_currentHeroCardDef?.CardDef == (UnityEngine.Object) null || string.IsNullOrEmpty(this.m_currentHeroCardDef.CardDef.m_CollectionHeroDefPath))
      return false;
    CollectionHeroDef collectionHeroDef = GameUtils.LoadGameObjectWithComponent<CollectionHeroDef>(this.m_currentHeroCardDef.CardDef.m_CollectionHeroDefPath);
    if ((UnityEngine.Object) collectionHeroDef == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("Hero def does not exist on object: {0}", (object) this.m_currentHeroCardDef.CardDef.m_CollectionHeroDefPath));
      return false;
    }
    if ((UnityEngine.Object) this.m_currentHeroDef != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentHeroDef.gameObject);
    this.m_currentCardId = cardId;
    this.m_currentHeroDef = collectionHeroDef;
    return true;
  }

  protected void SetupUI()
  {
    this.m_newHeroFavoriteButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      this.SetFavoriteHero();
      this.CancelPreview();
    }));
    if ((UnityEngine.Object) this.m_vanillaHeroFavoriteButton != (UnityEngine.Object) null && (UnityEngine.Object) this.m_vanillaHeroFavoriteButton != (UnityEngine.Object) this.m_newHeroFavoriteButton)
      this.m_vanillaHeroFavoriteButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
      {
        this.SetFavoriteHero();
        this.CancelPreview();
      }));
    this.m_newHeroBuyButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBuyButtonReleased()));
    if ((UnityEngine.Object) this.m_vanillaHeroBuyButton != (UnityEngine.Object) null && (UnityEngine.Object) this.m_vanillaHeroBuyButton != (UnityEngine.Object) this.m_newHeroBuyButton)
      this.m_vanillaHeroBuyButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBuyButtonReleased()));
    this.m_offClicker.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CancelPreview()));
    this.m_offClicker.AddEventListener(UIEventType.RIGHTCLICK, (UIEvent.Handler) (e => this.CancelPreview()));
    CollectionManager.Get().RegisterFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
  }

  private void OnBuyButtonReleased()
  {
    if (!this.IsHeroSkinCardIdValid())
    {
      Debug.LogError((object) "BaseHeroSkinInfoManager:OnBuyButtonReleased called when the hero skin card id was invalid");
    }
    else
    {
      this.m_visibilityVisualController.SetState("HIDDEN");
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
      info.m_headerText = GameStrings.Format("GLUE_HERO_SKIN_PURCHASE_CONFIRMATION_HEADER");
      info.m_text = GameStrings.Format("GLUE_HERO_SKIN_PURCHASE_CONFIRMATION_MESSAGE", (object) this.m_currentEntityDef.GetName());
      info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
      info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
      AlertPopup.ResponseCallback responseCallback = (AlertPopup.ResponseCallback) ((response, userdata) =>
      {
        if (response == AlertPopup.Response.CONFIRM)
          this.StartPurchaseTransaction();
        else
          this.UpdateView();
      });
      info.m_responseCallback = responseCallback;
      DialogManager.Get().ShowPopup(info);
    }
  }

  protected abstract void SetFavoriteHero();

  private void AssignVanillaHeroPreviewMaterial(
    Material material,
    Texture portraitTexture,
    UberShaderAnimation portraitAnimation,
    int portraitMaterialIndex)
  {
    Renderer component = this.m_vanillaHeroPreviewQuad.GetComponent<Renderer>();
    if ((UnityEngine.Object) portraitTexture != (UnityEngine.Object) null)
    {
      RendererExtension.SetMaterial(component, portraitMaterialIndex, material);
      RendererExtension.GetMaterial(component, portraitMaterialIndex).SetTexture("_MainTex", portraitTexture);
    }
    else
      RendererExtension.SetMaterial(component, portraitMaterialIndex, material);
    this.AssignVanillaHeroUberAnimation(portraitAnimation, portraitMaterialIndex);
  }

  private void AssignNewHeroPreviewMaterial(
    Material material,
    Texture portraitTexture,
    UberShaderAnimation portraitAnimation)
  {
    Renderer component = this.m_newHeroPreviewQuad.GetComponent<Renderer>();
    if ((UnityEngine.Object) material != (UnityEngine.Object) null)
    {
      RendererExtension.SetMaterial(component, material);
    }
    else
    {
      RendererExtension.SetMaterial(component, this.m_defaultPreviewMaterial);
      RendererExtension.GetMaterial(component).mainTexture = portraitTexture;
    }
    this.AssignNewHeroUberAnimation(portraitAnimation);
  }

  private void AssignVanillaHeroUberAnimation(
    UberShaderAnimation portraitAnimation,
    int portraitMaterialIndex)
  {
    UberShaderController shaderController = this.m_vanillaHeroPreviewQuad.GetComponent<UberShaderController>();
    if ((UnityEngine.Object) portraitAnimation != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) shaderController == (UnityEngine.Object) null)
        shaderController = this.m_vanillaHeroPreviewQuad.gameObject.AddComponent<UberShaderController>();
      shaderController.UberShaderAnimation = UnityEngine.Object.Instantiate<UberShaderAnimation>(portraitAnimation);
      shaderController.m_MaterialIndex = portraitMaterialIndex;
      shaderController.enabled = true;
    }
    else
    {
      if (!((UnityEngine.Object) shaderController != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) shaderController);
    }
  }

  private void AssignNewHeroUberAnimation(UberShaderAnimation portraitAnimation)
  {
    UberShaderController shaderController = this.m_newHeroPreviewQuad.GetComponent<UberShaderController>();
    if ((UnityEngine.Object) portraitAnimation != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) shaderController == (UnityEngine.Object) null)
        shaderController = this.m_newHeroPreviewQuad.gameObject.AddComponent<UberShaderController>();
      shaderController.UberShaderAnimation = UnityEngine.Object.Instantiate<UberShaderAnimation>(portraitAnimation);
      shaderController.m_MaterialIndex = 0;
      shaderController.enabled = true;
    }
    else
    {
      if (!((UnityEngine.Object) shaderController != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) shaderController);
    }
  }

  private void PlayHeroMusic()
  {
    MusicPlaylistType type = (UnityEngine.Object) this.m_currentHeroDef == (UnityEngine.Object) null || this.m_currentHeroDef.m_heroPlaylist == MusicPlaylistType.Invalid ? this.m_defaultHeroMusic : this.m_currentHeroDef.m_heroPlaylist;
    if (type == MusicPlaylistType.Invalid)
      return;
    this.m_prevPlaylist = MusicManager.Get().GetCurrentPlaylist();
    MusicManager.Get().StartPlaylist(type);
  }

  private void StopHeroMusic() => MusicManager.Get().StartPlaylist(this.m_prevPlaylist);

  private void BlockInputs(bool blocked)
  {
    if ((UnityEngine.Object) this.m_fullScreenBlockerWidget == (UnityEngine.Object) null)
      Debug.LogError((object) "Failed to toggle interface blocker from Duels Popup Manager");
    else
      this.m_fullScreenBlockerWidget.TriggerEvent(blocked ? "BLOCK_SCREEN" : "UNBLOCK_SCREEN");
  }

  private bool IsHeroSkinCardIdValid() => this.m_currentEntityDef != null && !string.IsNullOrEmpty(this.m_currentEntityDef.GetCardId());

  protected virtual void SetupHeroSkinStore()
  {
  }

  protected void ShutDownHeroSkinStore()
  {
    if (!this.m_isStoreOpen)
      return;
    this.CancelPurchaseTransaction();
    Action<StoreClosedArgs> onClosed = this.OnClosed;
    if (onClosed != null)
      onClosed(new StoreClosedArgs());
    StoreManager storeManager = StoreManager.Get();
    storeManager.RemoveFailedPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnFailedPurchaseAck));
    storeManager.RemoveSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchase));
    storeManager.RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    storeManager.ShutDownHeroSkinStore();
    this.OnProductPurchaseAttempt = (Action<BuyProductEventArgs>) null;
    this.m_activeHeroCardCurrencyTypes.Clear();
    BnetBar.Get()?.RefreshCurrency();
    this.BlockInputs(false);
    this.m_isStoreOpen = false;
  }

  protected void OnSuccessfulPurchase(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
  }

  protected void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    this.EndPurchaseTransaction();
    this.UpdateView();
  }

  protected void OnFailedPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    this.EndPurchaseTransaction();
    this.UpdateView();
  }

  protected void StartPurchaseTransaction()
  {
    if (!this.IsHeroSkinCardIdValid())
      Debug.LogError((object) "BaseHeroSkinInfoManager:StartPurchaseTransaction called when the hero skin card id was invalid");
    else if (this.m_isStoreTransactionActive)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_HERO_SKIN_PURCHASE_ERROR_HEADER"),
        m_text = GameStrings.Get("GLUE_CHECKOUT_ERROR_GENERIC_FAILURE"),
        m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      });
      Debug.LogWarning((object) ("Attempted to start a hero skin purchase transaction while an existing transaction was in progress (CardId = " + this.m_currentEntityDef.GetCardId() + ")"));
    }
    else if (this.OnProductPurchaseAttempt == null)
    {
      Debug.LogError((object) ("Attempted to start a hero skin purchase transaction while OnProductPurchaseAttempt was null (CardId = " + this.m_currentEntityDef.GetCardId() + ")"));
    }
    else
    {
      Network.Bundle skinProductBundle = HeroSkinUtils.GetCollectionManagerHeroSkinProductBundle(this.m_currentEntityDef.GetCardId());
      if ((Record) skinProductBundle == (Record) null)
      {
        Debug.LogError((object) ("Attempted to start a hero skin purchase transaction with a null bundle (CardId = " + this.m_currentEntityDef.GetCardId() + ")"));
      }
      else
      {
        PriceDataModel skinPriceDataModel = HeroSkinUtils.GetCollectionManagerHeroSkinPriceDataModel(this.m_currentEntityDef.GetCardId());
        if (skinPriceDataModel.Currency == CurrencyType.NONE || (double) skinPriceDataModel.Amount == 0.0)
        {
          Debug.LogError((object) ("Attempted to start a hero skin purchase transaction with " + string.Format("Currency: {0} Amount: {1} for card {2}", (object) skinPriceDataModel.Currency, (object) skinPriceDataModel.Amount, (object) this.m_currentEntityDef.GetCardId())));
        }
        else
        {
          this.m_isStoreTransactionActive = true;
          this.OnProductPurchaseAttempt((BuyProductEventArgs) new BuyPmtProductEventArgs(skinProductBundle, skinPriceDataModel.Currency, 1));
        }
      }
    }
  }

  protected void CancelPurchaseTransaction() => this.EndPurchaseTransaction();

  protected void EndPurchaseTransaction()
  {
    if (!this.m_isStoreTransactionActive)
      return;
    this.m_isStoreTransactionActive = false;
  }

  void IStore.Open()
  {
    Shop.Get().RefreshDataModel();
    this.m_isStoreOpen = true;
    Action onOpened = this.OnOpened;
    if (onOpened != null)
      onOpened();
    BnetBar bnetBar = BnetBar.Get();
    if ((UnityEngine.Object) bnetBar != (UnityEngine.Object) null)
      bnetBar.RefreshCurrency();
    else
      Debug.LogError((object) "BaseHeroSkinInfoManager:IStore.Open: Could not get the Bnet bar to reflect the required currency");
  }

  void IStore.Close()
  {
    if (!this.m_isStoreTransactionActive)
      return;
    this.CancelPurchaseTransaction();
  }

  void IStore.BlockInterface(bool blocked) => this.BlockInputs(blocked);

  bool IStore.IsReady() => true;

  bool IStore.IsOpen() => this.m_isStoreOpen;

  void IStore.Unload()
  {
  }

  IEnumerable<CurrencyType> IStore.GetVisibleCurrencies()
  {
    this.m_activeHeroCardCurrencyTypes.Add(CurrencyType.GOLD);
    return (IEnumerable<CurrencyType>) this.m_activeHeroCardCurrencyTypes;
  }
}
