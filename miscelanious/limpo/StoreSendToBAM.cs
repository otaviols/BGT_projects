using Blizzard.T5.Core;
using Hearthstone.Commerce;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class StoreSendToBAM : UIBPopup
{
  public UIBButton m_okayButton;
  public UIBButton m_cancelButton;
  public UberText m_headlineText;
  public UberText m_messageText;
  public MultiSliceElement m_allSections;
  public GameObject m_midSection;
  public GameObject m_sendToBAMRoot;
  public Transform m_sendToBAMRootWithSummaryBone;
  public StoreMiniSummary m_miniSummary;
  public PegUIElement m_offClickCatcher;
  private static readonly string SEND_TO_BAM_THEN_HIDE_COROUTINE = "SendToBAMThenHide";
  private static readonly PlatformDependentValue<string> GLUE_STORE_PAYMENT_INFO_DETAILS = new PlatformDependentValue<string>(PlatformCategory.OS)
  {
    PC = nameof (GLUE_STORE_PAYMENT_INFO_DETAILS),
    iOS = "GLUE_MOBILE_STORE_PAYMENT_INFO_DETAILS_APPLE",
    Android = "GLUE_MOBILE_STORE_PAYMENT_INFO_DETAILS_ANDROID"
  };
  private static readonly PlatformDependentValue<string> GLUE_STORE_PAYMENT_INFO_URL_DETAILS = new PlatformDependentValue<string>(PlatformCategory.OS)
  {
    PC = nameof (GLUE_STORE_PAYMENT_INFO_URL_DETAILS),
    iOS = "GLUE_MOBILE_STORE_PAYMENT_INFO_URL_DETAILS",
    Android = "GLUE_MOBILE_STORE_PAYMENT_INFO_URL_DETAILS"
  };
  private static readonly Vector3 SHOW_MINI_SUMMARY_SCALE_PHONE = new Vector3(80f, 80f, 80f);
  private Vector3 m_originalShowScale = Vector3.zero;
  private List<StoreSendToBAM.DelOKListener> m_okayListeners = new List<StoreSendToBAM.DelOKListener>();
  private List<StoreSendToBAM.DelCancelListener> m_cancelListeners = new List<StoreSendToBAM.DelCancelListener>();
  private StoreSendToBAM.BAMReason m_sendToBAMReason;
  private MoneyOrGTAPPTransaction m_moneyOrGTAPPTransaction;
  private string m_errorCode = "";
  private static Map<StoreSendToBAM.BAMReason, StoreSendToBAM.SendToBAMText> s_bamTextMap;

  protected override void Awake()
  {
    base.Awake();
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_scaleMode = CanvasScaleMode.WIDTH;
    if ((Object) this.m_offClickCatcher != (Object) null)
      this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
    StoreSendToBAM.s_bamTextMap = new Map<StoreSendToBAM.BAMReason, StoreSendToBAM.SendToBAMText>()
    {
      {
        StoreSendToBAM.BAMReason.PAYMENT_INFO,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_PAYMENT_INFO_HEADLINE", (string) StoreSendToBAM.GLUE_STORE_PAYMENT_INFO_DETAILS, (string) StoreSendToBAM.GLUE_STORE_PAYMENT_INFO_URL_DETAILS, ExternalUrlService.Get().GetPaymentInfoLink())
      },
      {
        StoreSendToBAM.BAMReason.NEED_PASSWORD_RESET,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_FORGOT_PWD_HEADLINE", "GLUE_STORE_FORGOT_PWD_DETAILS", "GLUE_STORE_FORGOT_PWD_URL_DETAILS", ExternalUrlService.Get().GetResetPasswordLink())
      },
      {
        StoreSendToBAM.BAMReason.NO_VALID_PAYMENT_METHOD,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_NO_PAYMENT_HEADLINE", "GLUE_STORE_NO_PAYMENT_DETAILS", "GLUE_STORE_NO_PAYMENT_URL_DETAILS", ExternalUrlService.Get().GetGenericPurchaseErrorLink())
      },
      {
        StoreSendToBAM.BAMReason.CREDIT_CARD_EXPIRED,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE", "GLUE_STORE_CC_EXPIRY_DETAILS", "GLUE_STORE_GENERIC_BP_FAIL_URL_DETAILS", ExternalUrlService.Get().GetAddPaymentLink())
      },
      {
        StoreSendToBAM.BAMReason.GENERIC_PAYMENT_FAIL,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE", "GLUE_STORE_GENERIC_BP_FAIL_DETAILS", "GLUE_STORE_GENERIC_BP_FAIL_URL_DETAILS", ExternalUrlService.Get().GetGenericPurchaseErrorLink())
      },
      {
        StoreSendToBAM.BAMReason.EULA_AND_TOS,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_EULA_AND_TOS_HEADLINE", "GLUE_STORE_EULA_AND_TOS_DETAILS", "GLUE_STORE_EULA_AND_TOS_URL_DETAILS", ExternalUrlService.Get().GetTermsOfSaleLink())
      },
      {
        StoreSendToBAM.BAMReason.PRODUCT_UNIQUENESS_VIOLATED,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_PURCHASE_LOCK_HEADER", "GLUE_STORE_FAIL_PRODUCT_UNIQUENESS_VIOLATED", "GLUE_STORE_FAIL_PRODUCT_UNIQUENESS_VIOLATED_URL", ExternalUrlService.Get().GetDuplicatePurchaseErrorLink())
      },
      {
        StoreSendToBAM.BAMReason.GENERIC_PURCHASE_FAIL_RETRY_CONTACT_CS_IF_PERSISTS,
        new StoreSendToBAM.SendToBAMText("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE", "GLUE_STORE_GENERIC_BP_FAIL_RETRY_CONTACT_CS_IF_PERSISTS_DETAILS", "GLUE_STORE_GENERIC_BP_FAIL_RETRY_CONTACT_CS_IF_PERSISTS_URL_DETAILS", ExternalUrlService.Get().GetGenericPurchaseErrorLink())
      }
    };
    this.m_okayButton.SetText(GameStrings.Get("GLOBAL_MORE"));
    this.m_cancelButton.SetText(GameStrings.Get("GLOBAL_CANCEL"));
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOkayPressed));
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
  }

  public void Show(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    StoreSendToBAM.BAMReason reason,
    string errorCode,
    bool fromPreviousPurchase)
  {
    this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
    this.m_sendToBAMReason = reason;
    this.m_errorCode = errorCode;
    this.UpdateText();
    if ((moneyOrGTAPPTransaction == null ? 0 : (fromPreviousPurchase ? 1 : (moneyOrGTAPPTransaction.ShouldShowMiniSummary() ? 1 : 0))) != 0)
    {
      ProductId from = ProductId.CreateFrom(this.m_moneyOrGTAPPTransaction.PMTProductID.GetValueOrDefault());
      this.m_sendToBAMRoot.transform.position = this.m_sendToBAMRootWithSummaryBone.position;
      this.m_miniSummary.SetDetails(from, 1);
      this.m_miniSummary.gameObject.SetActive(true);
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        this.m_originalShowScale = this.m_showScale;
        this.m_showScale = StoreSendToBAM.SHOW_MINI_SUMMARY_SCALE_PHONE;
      }
    }
    else
    {
      this.m_sendToBAMRoot.transform.localPosition = Vector3.zero;
      this.m_miniSummary.gameObject.SetActive(false);
      if ((bool) UniversalInputManager.UsePhoneUI && this.m_originalShowScale != Vector3.zero)
      {
        this.m_showScale = this.m_originalShowScale;
        this.m_originalShowScale = Vector3.zero;
      }
    }
    if (this.m_shown)
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnCancel));
    this.m_shown = true;
    this.m_headlineText.UpdateNow();
    this.LayoutMessageText();
    this.DoShowAnimation();
  }

  public void RegisterOkayListener(StoreSendToBAM.DelOKListener listener)
  {
    if (this.m_okayListeners.Contains(listener))
      return;
    this.m_okayListeners.Add(listener);
  }

  public void RemoveOkayListener(StoreSendToBAM.DelOKListener listener) => this.m_okayListeners.Remove(listener);

  public void RegisterCancelListener(StoreSendToBAM.DelCancelListener listener)
  {
    if (this.m_cancelListeners.Contains(listener))
      return;
    this.m_cancelListeners.Add(listener);
  }

  public void RemoveCancelListener(StoreSendToBAM.DelCancelListener listener) => this.m_cancelListeners.Remove(listener);

  protected override void OnHidden()
  {
    this.m_okayButton.SetEnabled(true);
    this.m_okayButton.TriggerOut();
  }

  private void OnOkayPressed(UIEvent e)
  {
    this.StopCoroutine(StoreSendToBAM.SEND_TO_BAM_THEN_HIDE_COROUTINE);
    this.StartCoroutine(StoreSendToBAM.SEND_TO_BAM_THEN_HIDE_COROUTINE);
  }

  private IEnumerator SendToBAMThenHide()
  {
    this.m_okayButton.SetEnabled(false);
    string url = "";
    StoreSendToBAM.SendToBAMText bamText = StoreSendToBAM.s_bamTextMap[this.m_sendToBAMReason];
    if (bamText == null)
      Debug.LogError((object) string.Format("StoreSendToBAM.SendToBAMThenHide(): can't get URL for BAM reason {0}", (object) this.m_sendToBAMReason));
    else
      url = bamText.GetUrl();
    if (!string.IsNullOrEmpty(url))
      Application.OpenURL(url);
    yield return (object) new WaitForSeconds(2f);
    Navigation.Pop();
    this.HideOrInactivate(true);
    foreach (StoreSendToBAM.DelOKListener delOkListener in this.m_okayListeners.ToArray())
      delOkListener(this.m_moneyOrGTAPPTransaction, this.m_sendToBAMReason);
  }

  private void HideOrInactivate(bool animate)
  {
    if (!this.m_shown)
    {
      this.OnHidden();
      this.gameObject.SetActive(false);
    }
    else
      this.Hide(animate);
  }

  private bool OnCancel()
  {
    this.StopCoroutine(StoreSendToBAM.SEND_TO_BAM_THEN_HIDE_COROUTINE);
    this.HideOrInactivate(true);
    foreach (StoreSendToBAM.DelCancelListener delCancelListener in this.m_cancelListeners.ToArray())
      delCancelListener(this.m_moneyOrGTAPPTransaction);
    return true;
  }

  private void OnCancelPressed(UIEvent e) => Navigation.GoBack();

  private void UpdateText()
  {
    StoreSendToBAM.SendToBAMText bamText = StoreSendToBAM.s_bamTextMap[this.m_sendToBAMReason];
    if (bamText == null)
    {
      Debug.LogError((object) string.Format("StoreSendToBAM.UpdateText(): don't know how to update text for BAM reason {0}", (object) this.m_sendToBAMReason));
      this.m_headlineText.Text = "";
      this.m_messageText.Text = "";
    }
    else
    {
      string str1 = bamText.GetDetails();
      if (!string.IsNullOrEmpty(this.m_errorCode))
        str1 = str1 + " " + GameStrings.Format("GLUE_STORE_FAIL_DETAILS_ERROR_CODE", (object) this.m_errorCode);
      string str2 = str1 + "\n\n" + bamText.GetGoToURLDetails(this.m_okayButton.m_ButtonText.Text);
      this.m_headlineText.Text = bamText.GetHeadline();
      this.m_messageText.Text = str2;
    }
  }

  private void LayoutMessageText()
  {
    this.m_messageText.UpdateNow();
    TransformUtil.SetLocalScaleZ(this.m_midSection, 1f);
    float num = TransformUtil.ComputeOrientedWorldBounds(this.m_midSection).Extents[2].magnitude * 2f;
    TransformUtil.SetLocalScaleZ(this.m_midSection, this.m_messageText.GetTextWorldSpaceBounds().size.z / num);
    this.m_allSections.UpdateSlices();
  }

  public enum BAMReason
  {
    PAYMENT_INFO,
    NEED_PASSWORD_RESET,
    NO_VALID_PAYMENT_METHOD,
    CREDIT_CARD_EXPIRED,
    GENERIC_PAYMENT_FAIL,
    EULA_AND_TOS,
    PRODUCT_UNIQUENESS_VIOLATED,
    GENERIC_PURCHASE_FAIL_RETRY_CONTACT_CS_IF_PERSISTS,
  }

  private class SendToBAMText
  {
    private string m_headlineKey;
    private string m_detailsKey;
    private string m_goToURLKey;
    private string m_url;

    public SendToBAMText(string headlineKey, string detailsKey, string goToURLKey, string url)
    {
      this.m_headlineKey = headlineKey;
      this.m_detailsKey = detailsKey;
      this.m_goToURLKey = goToURLKey;
      this.m_url = url;
    }

    public string GetHeadline() => GameStrings.Get(this.m_headlineKey);

    public string GetDetails() => GameStrings.Get(this.m_detailsKey);

    public string GetGoToURLDetails(string buttonName) => GameStrings.Format(this.m_goToURLKey, (object) buttonName);

    public string GetUrl() => this.m_url;
  }

  public delegate void DelOKListener(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    StoreSendToBAM.BAMReason reason);

  public delegate void DelCancelListener(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction);
}
