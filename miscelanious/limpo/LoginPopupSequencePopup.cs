using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class LoginPopupSequencePopup : BasicPopup
{
  public Transform m_cardBone;
  public Renderer m_background;
  private LoginPopupSequencePopup.Info m_info;
  private Material m_backgroundMaterial;
  private bool m_backgroundReady = true;
  private Actor m_cardActor;
  private bool m_cardReady = true;

  public void SetInfo(LoginPopupSequencePopup.Info info)
  {
    this.m_info = info;
    if (this.m_info.m_callbackOnHide == null)
      return;
    this.AddHideListener(this.m_info.m_callbackOnHide);
  }

  public void LoadAssetsAndShowWhenReady()
  {
    if (!string.IsNullOrEmpty((string) this.m_info.m_backgroundMaterialReference))
    {
      this.m_backgroundReady = false;
      AssetLoader.Get().LoadMaterial(this.m_info.m_backgroundMaterialReference, new ObjectCallback(this.OnBackgroundMaterialLoaded));
    }
    if (this.m_info.m_card != null)
    {
      this.m_cardReady = false;
      DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(this.m_info.m_card.CardId);
      AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef, this.m_info.m_card.PremiumType), new PrefabCallback<GameObject>(this.OnCardActorLoaded), (object) new LoginPopupSequencePopup.CardActorLoadedData()
      {
        m_fullDef = fullDef,
        m_premium = this.m_info.m_card.PremiumType
      }, AssetLoadingOptions.IgnorePrefabPosition);
    }
    this.StartCoroutine(this.ShowWhenReady());
  }

  public override void Show()
  {
    base.Show();
    this.SetUpPopup(this.m_info);
    DialogBase.DoBlur();
  }

  public override void Hide()
  {
    base.Hide();
    DialogBase.EndBlur();
  }

  protected override void Awake()
  {
    base.Awake();
    this.m_cancelButton?.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Hide()));
  }

  private void OnBackgroundMaterialLoaded(AssetReference assetRef, Object obj, object callbackData)
  {
    this.m_backgroundMaterial = (Material) obj;
    this.m_backgroundReady = true;
  }

  private void OnCardActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    LoginPopupSequencePopup.CardActorLoadedData cardActorLoadedData = (LoginPopupSequencePopup.CardActorLoadedData) callbackData;
    using (cardActorLoadedData.m_fullDef)
    {
      this.m_cardReady = true;
      this.m_cardActor = go.GetComponent<Actor>();
      this.m_cardActor.SetCardDef(cardActorLoadedData.m_fullDef.DisposableCardDef);
      this.m_cardActor.SetEntityDef(cardActorLoadedData.m_fullDef.EntityDef);
      this.m_cardActor.ContactShadow(true);
      this.m_cardActor.SetPremium(cardActorLoadedData.m_premium);
      this.m_cardActor.UpdateAllComponents();
      GameUtils.SetParent((Component) this.m_cardActor, (Component) this.m_cardBone, true);
      LayerUtils.SetLayer((Component) this.m_cardActor, this.m_cardBone.gameObject.layer);
    }
  }

  private IEnumerator ShowWhenReady()
  {
    LoginPopupSequencePopup popupSequencePopup = this;
    while (!popupSequencePopup.m_backgroundReady || !popupSequencePopup.m_cardReady)
      yield return (object) new WaitForEndOfFrame();
    popupSequencePopup.Show();
  }

  private void SetUpPopup(LoginPopupSequencePopup.Info info)
  {
    if ((Object) this.m_headerText != (Object) null)
      this.m_headerText.Text = info.m_headerText;
    if ((Object) this.m_bodyText != (Object) null)
      this.m_bodyText.Text = info.m_bodyText;
    if ((Object) this.m_cancelButton != (Object) null)
      this.m_cancelButton.SetText(info.m_buttonText);
    if (!((Object) this.m_backgroundMaterial != (Object) null))
      return;
    RendererExtension.SetMaterial(this.m_background, this.m_backgroundMaterial);
  }

  public class Info
  {
    public string m_prefabAssetReference;
    public string m_headerText;
    public string m_bodyText;
    public string m_buttonText;
    public CollectibleCard m_card;
    public AssetReference m_backgroundMaterialReference;
    public DialogBase.HideCallback m_callbackOnHide;
  }

  private struct CardActorLoadedData
  {
    public DefLoader.DisposableFullDef m_fullDef;
    public TAG_PREMIUM m_premium;
  }
}
