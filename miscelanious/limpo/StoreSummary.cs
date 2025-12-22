using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.Commerce;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class StoreSummary : UIBPopup
{
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_confirmButton;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_cancelButton;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_infoButton;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_termsOfSaleButton;
  [CustomEditField(Sections = "Text")]
  public UberText m_headlineText;
  [CustomEditField(Sections = "Text")]
  public UberText m_itemsHeadlineText;
  [CustomEditField(Sections = "Text")]
  public UberText m_itemsText;
  [CustomEditField(Sections = "Text")]
  public UberText m_priceHeadlineText;
  [CustomEditField(Sections = "Text")]
  public UberText m_priceText;
  [CustomEditField(Sections = "Text")]
  public UberText m_taxDisclaimerText;
  [CustomEditField(Sections = "Text")]
  public UberText m_chargeDetailsText;
  [CustomEditField(Sections = "Objects")]
  public GameObject m_bottomSectionRoot;
  [CustomEditField(Sections = "Objects")]
  public GameObject m_confirmButtonCheckMark;
  [CustomEditField(Sections = "Objects")]
  public GameObject m_midMesh;
  [CustomEditField(Sections = "Materials")]
  public Material m_enabledConfirmButtonMaterial;
  [CustomEditField(Sections = "Materials")]
  public Material m_disabledConfirmButtonMaterial;
  [CustomEditField(Sections = "Materials")]
  public Material m_enabledConfirmCheckMarkMaterial;
  [CustomEditField(Sections = "Materials")]
  public Material m_disabledConfirmCheckMarkMaterial;
  [CustomEditField(Sections = "Korean Specific Info")]
  public GameObject m_koreanRequirementRoot;
  [CustomEditField(Sections = "Korean Specific Info")]
  public Transform m_koreanBottomSectionBone;
  [CustomEditField(Sections = "Korean Specific Info")]
  public UberText m_koreanAgreementTermsText;
  [CustomEditField(Sections = "Korean Specific Info")]
  public CheckBox m_koreanAgreementCheckBox;
  [CustomEditField(Sections = "Korean Specific Info")]
  public float m_koreanBodySize;
  [CustomEditField(Sections = "Terms of Sale")]
  public GameObject m_termsOfSaleRoot;
  [CustomEditField(Sections = "Terms of Sale")]
  public Transform m_termsOfSaleBottomSectionBone;
  [CustomEditField(Sections = "Terms of Sale")]
  public UberText m_termsOfSaleAgreementText;
  [CustomEditField(Sections = "Terms of Sale")]
  public float m_termsBodySize;
  [CustomEditField(Sections = "Click Catchers")]
  public PegUIElement m_offClickCatcher;
  private static readonly Color ENABLED_CONFIRM_BUTTON_TEXT_COLOR = new Color(0.239f, 0.184f, 0.098f);
  private static readonly Color DISABLED_CONFIRM_BUTTON_TEXT_COLOR = new Color(0.1176f, 0.1176f, 0.1176f);
  private Network.Bundle m_bundle;
  private int m_quantity;
  private bool m_staticTextResized;
  private bool m_confirmButtonEnabled = true;
  private bool m_textInitialized;
  private List<StoreSummary.ConfirmListener> m_confirmListeners = new List<StoreSummary.ConfirmListener>();
  private List<StoreSummary.CancelListener> m_cancelListeners = new List<StoreSummary.CancelListener>();
  private List<StoreSummary.InfoListener> m_infoListeners = new List<StoreSummary.InfoListener>();
  private List<StoreSummary.PaymentAndTOSListener> m_paymentAndTOSListeners = new List<StoreSummary.PaymentAndTOSListener>();

  protected override void Awake()
  {
    base.Awake();
    this.m_confirmButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnConfirmPressed));
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
    if ((Object) this.m_offClickCatcher != (Object) null)
      this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
    this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoPressed));
    this.m_termsOfSaleButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTermsOfSalePressed));
    this.m_koreanAgreementCheckBox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleKoreanAgreement));
  }

  public void Show(ProductId productID, int quantity, string paymentMethodName)
  {
    this.SetDetails(productID, quantity, paymentMethodName);
    this.Show();
  }

  public bool RegisterConfirmListener(StoreSummary.ConfirmCallback callback, object userData = null)
  {
    StoreSummary.ConfirmListener confirmListener = new StoreSummary.ConfirmListener();
    confirmListener.SetCallback(callback);
    confirmListener.SetUserData(userData);
    if (this.m_confirmListeners.Contains(confirmListener))
      return false;
    this.m_confirmListeners.Add(confirmListener);
    return true;
  }

  public bool RemoveConfirmListener(StoreSummary.ConfirmCallback callback, object userData = null)
  {
    StoreSummary.ConfirmListener confirmListener = new StoreSummary.ConfirmListener();
    confirmListener.SetCallback(callback);
    confirmListener.SetUserData(userData);
    return this.m_confirmListeners.Remove(confirmListener);
  }

  public bool RegisterCancelListener(StoreSummary.CancelCallback callback, object userData = null)
  {
    StoreSummary.CancelListener cancelListener = new StoreSummary.CancelListener();
    cancelListener.SetCallback(callback);
    cancelListener.SetUserData(userData);
    if (this.m_cancelListeners.Contains(cancelListener))
      return false;
    this.m_cancelListeners.Add(cancelListener);
    return true;
  }

  public bool RemoveCancelListener(StoreSummary.CancelCallback callback, object userData = null)
  {
    StoreSummary.CancelListener cancelListener = new StoreSummary.CancelListener();
    cancelListener.SetCallback(callback);
    cancelListener.SetUserData(userData);
    return this.m_cancelListeners.Remove(cancelListener);
  }

  public bool RegisterInfoListener(StoreSummary.InfoCallback callback, object userData = null)
  {
    StoreSummary.InfoListener infoListener = new StoreSummary.InfoListener();
    infoListener.SetCallback(callback);
    infoListener.SetUserData(userData);
    if (this.m_infoListeners.Contains(infoListener))
      return false;
    this.m_infoListeners.Add(infoListener);
    return true;
  }

  public bool RemoveInfoListener(StoreSummary.InfoCallback callback, object userData = null)
  {
    StoreSummary.InfoListener infoListener = new StoreSummary.InfoListener();
    infoListener.SetCallback(callback);
    infoListener.SetUserData(userData);
    return this.m_infoListeners.Remove(infoListener);
  }

  public bool RegisterPaymentAndTOSListener(
    StoreSummary.PaymentAndTOSCallback callback,
    object userData = null)
  {
    StoreSummary.PaymentAndTOSListener paymentAndTosListener = new StoreSummary.PaymentAndTOSListener();
    paymentAndTosListener.SetCallback(callback);
    paymentAndTosListener.SetUserData(userData);
    if (this.m_paymentAndTOSListeners.Contains(paymentAndTosListener))
      return false;
    this.m_paymentAndTOSListeners.Add(paymentAndTosListener);
    return true;
  }

  public bool RemovePaymentAndTOSListener(
    StoreSummary.PaymentAndTOSCallback callback,
    object userData = null)
  {
    StoreSummary.PaymentAndTOSListener paymentAndTosListener = new StoreSummary.PaymentAndTOSListener();
    paymentAndTosListener.SetCallback(callback);
    paymentAndTosListener.SetUserData(userData);
    return this.m_paymentAndTOSListeners.Remove(paymentAndTosListener);
  }

  private void SetDetails(ProductId productID, int quantity, string paymentMethodName)
  {
    this.m_bundle = StoreManager.Get().GetBundleFromPmtProductId(productID);
    this.m_quantity = quantity;
    this.m_itemsText.Text = this.GetItemsText();
    this.m_priceText.Text = this.GetPriceText();
    this.m_taxDisclaimerText.Text = StoreManager.Get().GetTaxText();
    UberText chargeDetailsText = this.m_chargeDetailsText;
    string str1;
    if (paymentMethodName != null)
      str1 = GameStrings.Format("GLUE_STORE_SUMMARY_CHARGE_DETAILS", (object) paymentMethodName);
    else
      str1 = string.Empty;
    chargeDetailsText.Text = str1;
    string str2 = string.Empty;
    HashSet<ProductType> productsInBundle = StoreManager.Get().GetProductsInBundle(this.m_bundle);
    if (productsInBundle.Contains(ProductType.PRODUCT_TYPE_BOOSTER))
      str2 = !StoreManager.Get().IsProductPrePurchase(this.m_bundle) ? (!StoreManager.Get().IsProductFirstPurchaseBundle(this.m_bundle) ? GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_EXPERT_PACK") : GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_FIRST_PURCHASE_BUNDLE")) : GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_PACK_PREORDER");
    else if (productsInBundle.Contains(ProductType.PRODUCT_TYPE_DRAFT))
      str2 = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_FORGE_TICKET");
    else if (productsInBundle.Contains(ProductType.PRODUCT_TYPE_NAXX) || productsInBundle.Contains(ProductType.PRODUCT_TYPE_BRM) || productsInBundle.Contains(ProductType.PRODUCT_TYPE_LOE) || productsInBundle.Contains(ProductType.PRODUCT_TYPE_WING))
      str2 = 1 != this.m_bundle.Items.Count ? GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_ADVENTURE_BUNDLE") : GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_ADVENTURE_SINGLE");
    else if (productsInBundle.Contains(ProductType.PRODUCT_TYPE_HERO))
      str2 = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_HERO");
    else if (productsInBundle.Contains(ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET))
      str2 = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_TAVERN_BRAWL_TICKET");
    this.m_koreanAgreementTermsText.Text = str2;
  }

  private string GetItemsText() => GameStrings.Format("GLUE_STORE_SUMMARY_ITEM_ORDERED", (object) this.m_quantity, (object) StoreManager.Get().GetProductName(this.m_bundle));

  private string GetPriceText() => StoreManager.Get().FormatCostBundle(this.m_bundle);

  protected void UpdateMidMeshScale(float newSize)
  {
    Vector3 localScale = this.m_midMesh.transform.localScale;
    this.m_midMesh.transform.localScale = new Vector3(localScale.x, localScale.y, newSize);
  }

  public override void Show()
  {
    if (this.m_shown)
      return;
    this.m_infoButton.SetEnabled(true);
    this.m_termsOfSaleButton.SetEnabled(true);
    this.m_cancelButton.SetEnabled(true);
    this.m_confirmButton.SetEnabled(true);
    this.m_cancelButton.GetComponent<UIBHighlight>().Reset();
    this.m_confirmButton.GetComponent<UIBHighlight>().Reset();
    bool flag = StoreManager.Get().IsEuropeanCustomer();
    if (!this.m_textInitialized)
    {
      this.m_textInitialized = true;
      this.m_headlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_HEADLINE");
      this.m_itemsHeadlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_ITEMS_ORDERED_HEADLINE");
      this.m_priceHeadlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_PRICE_HEADLINE");
      this.m_infoButton.SetText(GameStrings.Get("GLUE_STORE_INFO_BUTTON_TEXT"));
      string text1 = GameStrings.Get("GLUE_STORE_TERMS_OF_SALE_BUTTON_TEXT");
      this.m_termsOfSaleButton.SetText(text1);
      string text2 = GameStrings.Get("GLUE_STORE_SUMMARY_PAY_NOW_TEXT");
      this.m_confirmButton.SetText(text2);
      this.m_cancelButton.SetText(GameStrings.Get("GLOBAL_CANCEL"));
      this.m_termsOfSaleAgreementText.Text = GameStrings.Format(flag ? "GLUE_STORE_SUMMARY_TOS_AGREEMENT_EU" : "GLUE_STORE_SUMMARY_TOS_AGREEMENT", (object) text2, (object) text1);
    }
    if (StoreManager.Get().IsKoreanCustomer())
    {
      this.m_bottomSectionRoot.transform.localPosition = this.m_koreanBottomSectionBone.localPosition;
      this.m_koreanRequirementRoot.gameObject.SetActive(true);
      this.m_koreanAgreementCheckBox.SetChecked(false);
      this.m_infoButton.gameObject.SetActive(true);
      this.UpdateMidMeshScale(this.m_koreanBodySize);
      this.EnableConfirmButton(false);
    }
    else
    {
      this.m_koreanRequirementRoot.gameObject.SetActive(false);
      this.m_infoButton.gameObject.SetActive(false);
      this.EnableConfirmButton(true);
    }
    if (flag || StoreManager.Get().IsNorthAmericanCustomer())
    {
      this.m_bottomSectionRoot.transform.localPosition = this.m_termsOfSaleBottomSectionBone.localPosition;
      this.UpdateMidMeshScale(this.m_termsBodySize);
      this.m_termsOfSaleRoot.gameObject.SetActive(true);
      this.m_termsOfSaleButton.gameObject.SetActive(true);
    }
    else
    {
      this.m_termsOfSaleRoot.gameObject.SetActive(false);
      this.m_termsOfSaleButton.gameObject.SetActive(false);
    }
    this.PreRender();
    this.m_shown = true;
    this.DoShowAnimation();
  }

  protected override void Hide(bool animate)
  {
    if (!this.m_shown)
      return;
    this.m_termsOfSaleButton.SetEnabled(false);
    this.m_infoButton.SetEnabled(false);
    this.m_cancelButton.SetEnabled(false);
    this.m_confirmButton.SetEnabled(false);
    this.m_shown = false;
    this.DoHideAnimation(!animate);
  }

  private void OnConfirmPressed(UIEvent e)
  {
    if (!this.m_confirmButtonEnabled)
      return;
    long num;
    if (!((Record) this.m_bundle == (Record) null))
    {
      long? pmtProductId = this.m_bundle.PMTProductID;
      if (!pmtProductId.HasValue)
      {
        num = 0L;
      }
      else
      {
        pmtProductId = this.m_bundle.PMTProductID;
        num = pmtProductId.Value;
      }
    }
    else
      num = 0L;
    long pmtProductId1 = num;
    TelemetryManager.Client().SendPurchasePayNowClicked(pmtProductId1);
    this.Hide(true);
    foreach (StoreSummary.ConfirmListener confirmListener in this.m_confirmListeners.ToArray())
      confirmListener.Fire(this.m_quantity);
  }

  private void OnCancelPressed(UIEvent e)
  {
    long num;
    if (!((Record) this.m_bundle == (Record) null))
    {
      long? pmtProductId = this.m_bundle.PMTProductID;
      if (!pmtProductId.HasValue)
      {
        num = 0L;
      }
      else
      {
        pmtProductId = this.m_bundle.PMTProductID;
        num = pmtProductId.Value;
      }
    }
    else
      num = 0L;
    long pmtProductId1 = num;
    TelemetryManager.Client().SendPurchaseCancelClicked(pmtProductId1);
    this.Hide(true);
    foreach (StoreSummary.CancelListener cancelListener in this.m_cancelListeners.ToArray())
      cancelListener.Fire();
  }

  private void OnInfoPressed(UIEvent e)
  {
    this.Hide(true);
    foreach (StoreSummary.InfoListener infoListener in this.m_infoListeners.ToArray())
      infoListener.Fire();
  }

  private void OnTermsOfSalePressed(UIEvent e)
  {
    this.Hide(true);
    foreach (StoreSummary.PaymentAndTOSListener paymentAndTosListener in this.m_paymentAndTOSListeners.ToArray())
      paymentAndTosListener.Fire();
  }

  private void PreRender()
  {
    this.m_itemsText.UpdateNow();
    this.m_priceText.UpdateNow();
    this.m_koreanAgreementTermsText.UpdateNow();
    if (this.m_staticTextResized)
      return;
    this.m_headlineText.UpdateNow();
    this.m_itemsHeadlineText.UpdateNow();
    this.m_priceHeadlineText.UpdateNow();
    this.m_taxDisclaimerText.UpdateNow();
    this.m_koreanAgreementCheckBox.m_uberText.UpdateNow();
    this.m_staticTextResized = true;
  }

  private void ToggleKoreanAgreement(UIEvent e) => this.EnableConfirmButton(this.m_koreanAgreementCheckBox.IsChecked());

  private void EnableConfirmButton(bool enabled)
  {
    this.m_confirmButtonEnabled = enabled;
    Material material1 = this.m_confirmButtonEnabled ? this.m_enabledConfirmButtonMaterial : this.m_disabledConfirmButtonMaterial;
    foreach (MultiSliceElement componentsInChild in this.m_confirmButton.m_RootObject.GetComponentsInChildren<MultiSliceElement>())
    {
      foreach (MultiSliceElement.Slice slice in componentsInChild.m_slices)
      {
        MeshRenderer component = slice.m_slice.GetComponent<MeshRenderer>();
        if ((Object) component != (Object) null)
          component.SetMaterial(material1);
      }
    }
    Material material2 = this.m_confirmButtonEnabled ? this.m_enabledConfirmCheckMarkMaterial : this.m_disabledConfirmCheckMarkMaterial;
    this.m_confirmButtonCheckMark.GetComponent<MeshRenderer>().SetMaterial(material2);
    this.m_confirmButton.m_ButtonText.TextColor = this.m_confirmButtonEnabled ? StoreSummary.ENABLED_CONFIRM_BUTTON_TEXT_COLOR : StoreSummary.DISABLED_CONFIRM_BUTTON_TEXT_COLOR;
  }

  public delegate void ConfirmCallback(int quantity, object userData);

  private class ConfirmListener : EventListener<StoreSummary.ConfirmCallback>
  {
    public void Fire(int quantity) => this.m_callback(quantity, this.m_userData);
  }

  public delegate void CancelCallback(object userData);

  private class CancelListener : EventListener<StoreSummary.CancelCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }

  public delegate void InfoCallback(object userData);

  private class InfoListener : EventListener<StoreSummary.InfoCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }

  public delegate void PaymentAndTOSCallback(object userData);

  private class PaymentAndTOSListener : EventListener<StoreSummary.PaymentAndTOSCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
