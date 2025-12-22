using System;
using UnityEngine;

[CustomEditClass]
public class GeneralStoreHeroesContent : GeneralStoreContent
{
  public string m_keyArtFadeAnim = "HeroSkinArt_WipeAway";
  public string m_keyArtAppearAnim = "HeroSkinArtGlowIn";
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_keyArtFadeSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_keyArtAppearSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_previewButtonClick;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_backgroundFlipSound;
  public GameObject m_heroEmptyDisplay;
  public GeneralStoreHeroesContentDisplay m_heroDisplay;
  public MeshRenderer m_renderQuad1;
  public GameObject m_renderToTexture1;
  public MeshRenderer m_renderQuad2;
  public GameObject m_renderToTexture2;
  private GameObject m_currentSelectedHeroBannerFlare;
  private CollectionHeroDef m_currentHeroDef;
  private int m_currentCardBackPreview = -1;
  private int m_currentDisplay = -1;
  private CardHeroDbfRecord m_currentDbfRecord;
  private GeneralStoreHeroesContentDisplay m_heroDisplay1;
  private GeneralStoreHeroesContentDisplay m_heroDisplay2;

  private void Awake()
  {
    this.m_heroDisplay1 = this.m_heroDisplay;
    this.m_heroDisplay2 = UnityEngine.Object.Instantiate<GeneralStoreHeroesContentDisplay>(this.m_heroDisplay);
    this.m_heroDisplay2.transform.parent = this.m_heroDisplay1.transform.parent;
    this.m_heroDisplay2.transform.localPosition = this.m_heroDisplay1.transform.localPosition;
    this.m_heroDisplay2.transform.localScale = this.m_heroDisplay1.transform.localScale;
    this.m_heroDisplay2.transform.localRotation = this.m_heroDisplay1.transform.localRotation;
    this.m_heroDisplay2.gameObject.SetActive(false);
    this.m_heroDisplay1.SetParent(this);
    this.m_heroDisplay2.SetParent(this);
    this.m_heroDisplay1.SetKeyArtRenderer(this.m_renderQuad1);
    this.m_heroDisplay2.SetKeyArtRenderer(this.m_renderQuad2);
    this.m_renderToTexture1.GetComponent<RenderToTexture>().m_RenderToObject = this.m_heroDisplay1.m_renderArtQuad;
    this.m_renderToTexture2.GetComponent<RenderToTexture>().m_RenderToObject = this.m_heroDisplay2.m_renderArtQuad;
  }

  public override bool AnimateEntranceEnd()
  {
    this.m_parentStore.HideAccentTexture();
    return true;
  }

  public CardHeroDbfRecord GetSelectedHero() => this.m_currentDbfRecord;

  public void SelectHero(CardHeroDbfRecord cardHeroDbfRecord, bool animate = true)
  {
    if (cardHeroDbfRecord == this.m_currentDbfRecord)
      return;
    this.m_currentDbfRecord = cardHeroDbfRecord;
    Network.Bundle heroBundle = (Network.Bundle) null;
    StoreManager.Get().GetHeroBundleByCardDbId(cardHeroDbfRecord.CardId, out heroBundle);
    this.SetCurrentMoneyBundle(heroBundle);
    if ((UnityEngine.Object) this.m_currentHeroDef != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentHeroDef.gameObject);
      this.m_currentHeroDef = (CollectionHeroDef) null;
    }
    this.m_currentCardBackPreview = cardHeroDbfRecord.CardBackId;
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(this.m_currentDbfRecord.CardId))
    {
      this.m_currentHeroDef = GameUtils.LoadGameObjectWithComponent<CollectionHeroDef>(cardDef.CardDef.m_CollectionHeroDefPath);
      bool purchased = StoreManager.Get().IsProductAlreadyOwned(heroBundle);
      this.AnimateAndUpdateDisplays(cardHeroDbfRecord, this.m_currentCardBackPreview, this.m_currentHeroDef, purchased);
      this.PlayHeroMusic();
      this.UpdateHeroDescription(purchased);
    }
  }

  public void PlayCurrentHeroPurchaseEmote()
  {
    GeneralStoreHeroesContentDisplay currentDisplay = this.GetCurrentDisplay();
    if (!((UnityEngine.Object) currentDisplay != (UnityEngine.Object) null))
      return;
    currentDisplay.PlayPurchaseEmote();
  }

  public override void StoreShown(bool isCurrent)
  {
    if (this.m_currentDisplay == -1 || !isCurrent)
      return;
    this.PlayHeroMusic();
    this.ResetHeroPreview();
  }

  public override void PreStoreFlipIn() => this.ResetHeroPreview();

  public override void PostStoreFlipIn(bool animatedFlipIn) => this.PlayHeroMusic();

  public override void TryBuyWithMoney(
    Network.Bundle bundle,
    GeneralStoreContent.BuyEvent successBuyCB,
    GeneralStoreContent.BuyEvent failedBuyCB)
  {
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    SpecialEventType eventType = specialEventManager.GetEventType(bundle.ProductEvent);
    if (!specialEventManager.IsEventActive(eventType, false))
    {
      string key = "GLUE_STORE_PRODUCT_NOT_AVAILABLE_TEXT";
      if (specialEventManager.HasEventEnded(eventType))
        key = "GLUE_STORE_PRODUCT_NOT_AVAILABLE_TEXT_HAS_ENDED";
      else if (specialEventManager.GetEventStartTimeUtc(eventType).HasValue && !specialEventManager.HasEventStarted(eventType))
        key = "GLUE_STORE_PRODUCT_NOT_AVAILABLE_TEXT_NOT_YET_STARTED";
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
      info.m_headerText = GameStrings.Get("GLUE_STORE_PRODUCT_NOT_AVAILABLE_HEADER");
      info.m_text = GameStrings.Get(key);
      info.m_showAlertIcon = true;
      info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
      info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, data) =>
      {
        this.m_parentStore.BlockInterface(false);
        if (failedBuyCB == null)
          return;
        failedBuyCB();
      });
      this.m_parentStore.BlockInterface(true);
      DialogManager.Get().ShowPopup(info);
    }
    else
    {
      if (successBuyCB == null)
        return;
      successBuyCB();
    }
  }

  protected override void OnRefresh()
  {
    Network.Bundle heroBundle = (Network.Bundle) null;
    if (this.m_currentDbfRecord != null)
      StoreManager.Get().GetHeroBundleByCardDbId(this.m_currentDbfRecord.CardId, out heroBundle);
    bool flag = StoreManager.Get().IsProductAlreadyOwned(heroBundle);
    this.GetCurrentDisplay().ShowPurchasedCheckmark(flag);
    this.SetCurrentMoneyBundle(heroBundle, true);
    this.UpdateHeroDescription(flag);
  }

  public override bool IsPurchaseDisabled() => this.m_currentDisplay == -1;

  public override string GetMoneyDisplayOwnedText() => GameStrings.Get("GLUE_STORE_HERO_BUTTON_COST_OWNED_TEXT");

  private GameObject GetCurrentDisplayContainer() => this.GetCurrentDisplay().gameObject;

  private GameObject GetNextDisplayContainer() => (this.m_currentDisplay + 1) % 2 != 0 ? this.m_heroDisplay2.gameObject : this.m_heroDisplay1.gameObject;

  private GeneralStoreHeroesContentDisplay GetCurrentDisplay() => this.m_currentDisplay != 0 ? this.m_heroDisplay2 : this.m_heroDisplay1;

  private void AnimateAndUpdateDisplays(
    CardHeroDbfRecord cardHeroDbfRecord,
    int cardBackIdx,
    CollectionHeroDef heroDef,
    bool purchased)
  {
    GameObject currDisplay = (GameObject) null;
    if (this.m_currentDisplay == -1)
    {
      this.m_currentDisplay = 1;
      currDisplay = this.m_heroEmptyDisplay;
    }
    else
      currDisplay = this.GetCurrentDisplayContainer();
    GameObject displayContainer = this.GetNextDisplayContainer();
    GeneralStoreHeroesContentDisplay currentDisplay1 = this.GetCurrentDisplay();
    this.m_currentDisplay = (this.m_currentDisplay + 1) % 2;
    currDisplay.transform.localRotation = Quaternion.identity;
    displayContainer.transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);
    displayContainer.SetActive(true);
    iTween.StopByName(currDisplay, "ROTATION_TWEEN");
    iTween.StopByName(displayContainer, "ROTATION_TWEEN");
    iTween.RotateBy(currDisplay, iTween.Hash((object) "amount", (object) new Vector3(0.5f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "name", (object) "ROTATION_TWEEN", (object) "oncomplete", (object) (Action<object>) (o => currDisplay.SetActive(false))));
    if ((UnityEngine.Object) this.m_currentSelectedHeroBannerFlare != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentSelectedHeroBannerFlare);
      this.m_currentSelectedHeroBannerFlare = (GameObject) null;
    }
    if (this.m_currentDbfRecord != null && !string.IsNullOrEmpty(this.m_currentDbfRecord.StoreBannerPrefab))
    {
      this.m_currentSelectedHeroBannerFlare = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_currentDbfRecord.StoreBannerPrefab);
      if ((UnityEngine.Object) this.m_currentSelectedHeroBannerFlare != (UnityEngine.Object) null)
      {
        GameUtils.SetParent(this.m_currentSelectedHeroBannerFlare, displayContainer);
        this.m_currentSelectedHeroBannerFlare.transform.localPosition = Vector3.zero;
        this.m_currentSelectedHeroBannerFlare.transform.localRotation = Quaternion.identity;
        this.m_currentSelectedHeroBannerFlare.gameObject.SetActive(true);
      }
    }
    iTween.RotateBy(displayContainer, iTween.Hash((object) "amount", (object) new Vector3(0.5f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "name", (object) "ROTATION_TWEEN"));
    if (!string.IsNullOrEmpty(this.m_backgroundFlipSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_backgroundFlipSound);
    GeneralStoreHeroesContentDisplay currentDisplay2 = this.GetCurrentDisplay();
    currentDisplay2.UpdateFrame(cardHeroDbfRecord, cardBackIdx, heroDef);
    currentDisplay2.ShowPurchasedCheckmark(purchased);
    currentDisplay2.ResetPreview();
    currentDisplay1.ResetPreview();
  }

  private void ResetHeroPreview() => this.GetCurrentDisplay().ResetPreview();

  private void PlayHeroMusic()
  {
    if (!((UnityEngine.Object) this.m_currentHeroDef == (UnityEngine.Object) null) && this.m_currentHeroDef.m_heroPlaylist != MusicPlaylistType.Invalid && MusicManager.Get().StartPlaylist(this.m_currentHeroDef.m_heroPlaylist))
      return;
    this.m_parentStore.ResumePreviousMusicPlaylist();
  }

  private void UpdateHeroDescription(bool purchased)
  {
    if (this.m_currentDisplay == -1 || this.m_currentDbfRecord == null)
    {
      this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_HERO"));
    }
    else
    {
      string warning = StoreManager.Get().IsKoreanCustomer() ? GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_HERO") : string.Empty;
      this.m_parentStore.SetDescription(string.Empty, this.GetHeroDescriptionString(), warning);
    }
    this.m_parentStore.HideAccentTexture();
  }

  private string GetHeroDescriptionString() => !(bool) UniversalInputManager.UsePhoneUI ? (string) this.m_currentDbfRecord.StoreDesc : (string) this.m_currentDbfRecord.StoreDescPhone;
}
