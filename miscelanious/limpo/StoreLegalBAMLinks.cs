using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreLegalBAMLinks : UIBPopup
{
  public GameObject m_root;
  public UIBButton m_paymentMethodButton;
  public UIBButton m_termsOfSaleButton;
  public PegUIElement m_offClickCatcher;
  private static readonly string SEND_TO_BAM_THEN_HIDE_COROUTINE = "SendToBAMThenHide";
  private List<StoreLegalBAMLinks.SendToBAMListener> m_sendToBAMListeners = new List<StoreLegalBAMLinks.SendToBAMListener>();
  private List<StoreLegalBAMLinks.CancelListener> m_cancelListeners = new List<StoreLegalBAMLinks.CancelListener>();

  protected override void Awake()
  {
    base.Awake();
    this.m_termsOfSaleButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTermsOfSalePressed));
    this.m_paymentMethodButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPaymentMethodPressed));
    this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherPressed));
  }

  public override void Show()
  {
    base.Show();
    this.EnableButtons(true);
    int num = this.m_shown ? 1 : 0;
  }

  public void RegisterSendToBAMListener(StoreLegalBAMLinks.SendToBAMListener listener)
  {
    if (this.m_sendToBAMListeners.Contains(listener))
      return;
    this.m_sendToBAMListeners.Add(listener);
  }

  public void RemoveSendToBAMListener(StoreLegalBAMLinks.SendToBAMListener listener) => this.m_sendToBAMListeners.Remove(listener);

  public void RegisterCancelListener(StoreLegalBAMLinks.CancelListener listener)
  {
    if (this.m_cancelListeners.Contains(listener))
      return;
    this.m_cancelListeners.Add(listener);
  }

  public void RemoveCancelListener(StoreLegalBAMLinks.CancelListener listener) => this.m_cancelListeners.Remove(listener);

  private void OnTermsOfSalePressed(UIEvent e)
  {
    this.StopCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE);
    this.StartCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE, (object) StoreLegalBAMLinks.BAMReason.READ_TERMS_OF_SALE);
  }

  private void OnPaymentMethodPressed(UIEvent e)
  {
    this.StopCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE);
    this.StartCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE, (object) StoreLegalBAMLinks.BAMReason.CHANGE_PAYMENT_METHOD);
  }

  private void OnClickCatcherPressed(UIEvent e)
  {
    this.Hide(true);
    foreach (StoreLegalBAMLinks.CancelListener cancelListener in this.m_cancelListeners.ToArray())
      cancelListener();
  }

  private IEnumerator SendToBAMThenHide(StoreLegalBAMLinks.BAMReason reason)
  {
    StoreLegalBAMLinks storeLegalBamLinks = this;
    string url = (string) null;
    storeLegalBamLinks.EnableButtons(false);
    switch (reason)
    {
      case StoreLegalBAMLinks.BAMReason.CHANGE_PAYMENT_METHOD:
        url = ExternalUrlService.Get().GetAddPaymentLink();
        break;
      case StoreLegalBAMLinks.BAMReason.READ_TERMS_OF_SALE:
        url = ExternalUrlService.Get().GetTermsOfSaleLink();
        break;
    }
    if (!string.IsNullOrEmpty(url))
      Application.OpenURL(url);
    yield return (object) new WaitForSeconds(2f);
    storeLegalBamLinks.Hide(true);
    foreach (StoreLegalBAMLinks.SendToBAMListener sendToBamListener in storeLegalBamLinks.m_sendToBAMListeners.ToArray())
      sendToBamListener(reason);
  }

  private void EnableButtons(bool enabled)
  {
    this.m_termsOfSaleButton.SetEnabled(enabled);
    this.m_paymentMethodButton.SetEnabled(enabled);
  }

  public enum BAMReason
  {
    CHANGE_PAYMENT_METHOD,
    READ_TERMS_OF_SALE,
  }

  public delegate void SendToBAMListener(StoreLegalBAMLinks.BAMReason urlType);

  public delegate void CancelListener();
}
