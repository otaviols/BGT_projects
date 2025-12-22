using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

[CustomEditClass]
public class GeneralStoreHeroesContentDisplay : MonoBehaviour
{
  public UberText m_heroName;
  public UberText m_className;
  public GameObject m_renderArtQuad;
  public UIBButton m_previewToggle;
  public Animator m_keyArtAnimation;
  public MeshRenderer m_classIcon;
  public MeshRenderer m_fauxPlateTexture;
  public MeshRenderer m_backgroundFrame;
  public int m_backgroundMaterialIndex;
  private Texture m_defaultBackgroundTexture;
  public GameObject m_heroContainer;
  public GameObject m_heroPowerContainer;
  public GameObject m_cardBackContainer;
  public GameObject m_previewButtonFX;
  public GameObject m_purchasedCheckMark;
  public GeneralStoreHeroesContentLite m_parentLite;
  private GeneralStoreHeroesContent m_parent;
  private CollectionHeroDef m_currentHeroAsset;
  private GameObject m_cardBack;
  private Actor m_heroActor;
  private Actor m_heroPowerActor;
  private bool m_keyArtShowing = true;
  private CardSoundSpell m_previewEmote;
  private CardSoundSpell m_purchaseEmote;
  private MeshRenderer m_keyArt;

  private void Awake()
  {
    if ((Object) this.m_defaultBackgroundTexture == (Object) null && (Object) this.m_backgroundFrame != (Object) null && this.m_backgroundMaterialIndex >= 0 && this.m_backgroundMaterialIndex < this.m_backgroundFrame.GetMaterials().Count)
      this.m_defaultBackgroundTexture = this.m_backgroundFrame.GetMaterial(this.m_backgroundMaterialIndex).GetTexture("_MainTex");
    this.m_previewToggle.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.TogglePreview()));
  }

  public void SetKeyArtRenderer(MeshRenderer keyArtRenderer) => this.m_keyArt = keyArtRenderer;

  public void PlayPreviewEmote()
  {
    if ((Object) this.m_previewEmote == (Object) null || (Object) Box.Get() == (Object) null || (Object) Box.Get().GetCamera() == (Object) null)
      return;
    this.m_previewEmote.SetPosition(Box.Get().GetCamera().transform.position);
    this.m_previewEmote.Reactivate();
  }

  public void PlayPurchaseEmote()
  {
    if ((Object) this.m_purchaseEmote == (Object) null)
      return;
    this.m_purchaseEmote.SetPosition(Box.Get().GetCamera().transform.position);
    this.m_purchaseEmote.Reactivate();
  }

  public void SetParent(GeneralStoreHeroesContent parent) => this.m_parent = parent;

  public void Init()
  {
    if ((Object) this.m_heroActor == (Object) null)
    {
      this.m_heroActor = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit("Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d"), (AssetLoadingOptions) 2).GetComponent<Actor>();
      this.m_heroActor.SetUnlit();
      this.m_heroActor.Show();
      this.m_heroActor.GetHealthObject().Hide();
      this.m_heroActor.GetAttackObject().Hide();
      GameUtils.SetParent((Component) this.m_heroActor, this.m_heroContainer, true);
      LayerUtils.SetLayer((Component) this.m_heroActor, this.m_heroContainer.layer);
    }
    if (!((Object) this.m_heroPowerActor == (Object) null))
      return;
    this.m_heroPowerActor = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit("Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af"), (AssetLoadingOptions) 2).GetComponent<Actor>();
    this.m_heroPowerActor.SetUnlit();
    this.m_heroPowerActor.Show();
    GameUtils.SetParent((Component) this.m_heroPowerActor, this.m_heroPowerContainer, true);
    LayerUtils.SetLayer((Component) this.m_heroPowerActor, this.m_heroPowerContainer.layer);
  }

  public void ShowPurchasedCheckmark(bool show)
  {
    if (!((Object) this.m_purchasedCheckMark != (Object) null))
      return;
    this.m_purchasedCheckMark.SetActive(show);
  }

  public void UpdateFrame(
    CardHeroDbfRecord cardHeroDbfRecord,
    int cardBackIdx,
    CollectionHeroDef heroDef)
  {
    this.Init();
    if ((Object) heroDef.m_fauxPlateTexture != (Object) null)
      this.m_fauxPlateTexture.GetMaterial().SetTexture("_MainTex", heroDef.m_fauxPlateTexture);
    this.m_keyArt.SetMaterial(heroDef.m_previewMaterial.GetMaterial());
    string shaderAnimationPath = heroDef.GetHeroUberShaderAnimationPath();
    if (!string.IsNullOrEmpty(shaderAnimationPath))
    {
      UberShaderAnimation uberShaderAnimation = (AssetLoader.Get() as AssetLoader).LoadUberAnimation(AssetReference.op_Implicit(shaderAnimationPath), false);
      if ((Object) uberShaderAnimation == (Object) null)
      {
        Error.AddDevFatal("Failed to load animation {0} for {1}", (object) shaderAnimationPath, (object) heroDef);
      }
      else
      {
        UberShaderController shaderController = this.m_keyArt.GetComponent<UberShaderController>();
        if ((Object) shaderController == (Object) null)
          shaderController = this.m_keyArt.gameObject.AddComponent<UberShaderController>();
        shaderController.UberShaderAnimation = uberShaderAnimation;
        shaderController.m_MaterialIndex = 0;
      }
    }
    DefLoader.Get().LoadFullDef(GameUtils.TranslateDbIdToCardId(cardHeroDbfRecord.CardId), (DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>) ((heroCardId, heroFullDef, data1) =>
    {
      using (heroFullDef)
      {
        this.m_heroActor.SetPremium(TAG_PREMIUM.NORMAL);
        this.m_heroActor.SetFullDef(heroFullDef);
        this.m_heroActor.UpdateAllComponents();
        this.m_heroActor.Hide();
        this.m_heroName.Text = heroFullDef.EntityDef.GetName();
        this.m_className.Text = GameStrings.GetClassName(heroFullDef.EntityDef.GetClass());
        string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(heroCardId);
        DefLoader.Get().LoadFullDef(powerCardIdFromHero, (DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>) ((powerCardId, powerDef, data2) =>
        {
          using (powerDef)
          {
            this.m_heroPowerActor.SetPremium(TAG_PREMIUM.GOLDEN);
            this.m_heroPowerActor.SetFullDef(powerDef);
            this.m_heroPowerActor.UpdateAllComponents();
            this.m_heroPowerActor.Hide();
          }
        }));
        Vector2 vector2;
        if (CollectionPageManager.s_classTextureOffsets.TryGetValue(heroFullDef.EntityDef.GetClass(), out vector2))
          this.m_classIcon.GetMaterial().SetTextureOffset("_MainTex", vector2);
        this.ClearEmotes();
        if (heroDef.m_storePreviewEmote != EmoteType.INVALID)
          GameUtils.LoadCardDefEmoteSound(heroFullDef.CardDef.m_EmoteDefs, heroDef.m_storePreviewEmote, (GameUtils.EmoteSoundLoaded) (spell =>
          {
            if ((Object) spell == (Object) null)
              return;
            this.m_previewEmote = spell;
            GameUtils.SetParent((Component) this.m_previewEmote, (Component) this);
          }));
        if (heroDef.m_storePurchaseEmote == EmoteType.INVALID)
          return;
        GameUtils.LoadCardDefEmoteSound(heroFullDef.CardDef.m_EmoteDefs, heroDef.m_storePurchaseEmote, (GameUtils.EmoteSoundLoaded) (spell =>
        {
          if ((Object) spell == (Object) null)
            return;
          this.m_purchaseEmote = spell;
          GameUtils.SetParent((Component) this.m_purchaseEmote, (Component) this);
        }));
      }
    }));
    if ((Object) this.m_cardBack != (Object) null)
    {
      Object.Destroy((Object) this.m_cardBack);
      this.m_cardBack = (GameObject) null;
    }
    if (cardBackIdx != 0)
      CardBackManager.Get().LoadCardBackByIndex(cardBackIdx, (CardBackManager.LoadCardBackData.LoadCardBackCallback) (cardBackData =>
      {
        GameObject gameObject = cardBackData.m_GameObject;
        gameObject.name = "CARD_BACK_" + (object) cardBackIdx;
        this.m_cardBack = gameObject;
        LayerUtils.SetLayer(gameObject, this.m_cardBackContainer.gameObject.layer);
        GameUtils.SetParent(gameObject, this.m_cardBackContainer);
        this.m_cardBack.transform.localPosition = Vector3.zero;
        this.m_cardBack.transform.localScale = Vector3.one;
        this.m_cardBack.transform.localRotation = Quaternion.identity;
        AnimationUtil.FloatyPosition(this.m_cardBack, 0.05f, 10f);
      }));
    if (!((Object) this.m_backgroundFrame != (Object) null) || this.m_backgroundMaterialIndex < 0 || this.m_backgroundMaterialIndex > this.m_backgroundFrame.GetMaterials().Count)
      return;
    Texture texture1 = this.m_defaultBackgroundTexture;
    if (!string.IsNullOrEmpty(cardHeroDbfRecord.StoreBackgroundTexture))
    {
      Texture texture2 = AssetLoader.Get().LoadTexture(AssetReference.op_Implicit(cardHeroDbfRecord.StoreBackgroundTexture), false, false);
      if ((Object) texture2 != (Object) null)
        texture1 = texture2;
    }
    if (!((Object) texture1 != (Object) null))
      return;
    this.m_backgroundFrame.GetMaterial(this.m_backgroundMaterialIndex).SetTexture("_MainTex", texture1);
  }

  public void TogglePreview()
  {
    if ((Object) this.m_parentLite != (Object) null)
    {
      if (!string.IsNullOrEmpty(this.m_parentLite.m_previewButtonClick))
        SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(this.m_parentLite.m_previewButtonClick));
    }
    else if (!string.IsNullOrEmpty(this.m_parent.m_previewButtonClick))
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(this.m_parent.m_previewButtonClick));
    this.PlayKeyArtAnimation(this.m_keyArtShowing);
    this.m_keyArtShowing = !this.m_keyArtShowing;
    if (!this.m_keyArtShowing)
    {
      this.m_heroActor.Show();
      this.m_heroPowerActor.Show();
      this.PlayPreviewEmote();
    }
    else
    {
      this.m_heroActor.Hide();
      this.m_heroPowerActor.Hide();
    }
  }

  public void ResetPreview()
  {
    this.m_keyArtShowing = true;
    this.m_keyArtAnimation.enabled = true;
    this.m_keyArtAnimation.StopPlayback();
    if ((Object) this.m_parentLite != (Object) null)
      this.m_keyArtAnimation.Play(this.m_parentLite.m_keyArtAppearAnim, -1, 1f);
    else
      this.m_keyArtAnimation.Play(this.m_parent.m_keyArtAppearAnim, -1, 1f);
    this.m_previewButtonFX.SetActive(false);
  }

  private void PlayKeyArtAnimation(bool showPreview)
  {
    string stateName;
    string str;
    if (showPreview)
    {
      if ((Object) this.m_parentLite != (Object) null)
      {
        stateName = this.m_parentLite.m_keyArtFadeAnim;
        str = this.m_parentLite.m_keyArtFadeSound;
      }
      else
      {
        stateName = this.m_parent.m_keyArtFadeAnim;
        str = this.m_parent.m_keyArtFadeSound;
      }
    }
    else if ((Object) this.m_parentLite != (Object) null)
    {
      stateName = this.m_parentLite.m_keyArtAppearAnim;
      str = this.m_parentLite.m_keyArtAppearSound;
    }
    else
    {
      stateName = this.m_parent.m_keyArtAppearAnim;
      str = this.m_parent.m_keyArtAppearSound;
    }
    this.m_previewButtonFX.SetActive(showPreview);
    if (!string.IsNullOrEmpty(str))
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(str));
    this.m_keyArtAnimation.enabled = true;
    this.m_keyArtAnimation.StopPlayback();
    this.m_keyArtAnimation.Play(stateName, -1, 0.0f);
  }

  private void ClearEmotes()
  {
    if ((Object) this.m_previewEmote != (Object) null)
      Object.Destroy((Object) this.m_previewEmote.gameObject);
    if (!((Object) this.m_purchaseEmote != (Object) null))
      return;
    Object.Destroy((Object) this.m_purchaseEmote.gameObject);
  }
}
