using Hearthstone.Commerce;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class StorePurchaseAuth : UIBPopup
{
  private const string s_OkButtonText = "GLOBAL_OKAY";
  private const string s_BackButtonText = "GLOBAL_BACK";
  private const string s_CancelButtonText = "GLOBAL_CANCEL";
  [CustomEditField(Sections = "Base UI")]
  public MultiSliceElement m_root;
  [CustomEditField(Sections = "Swirly Animation")]
  public Spell m_spell;
  [CustomEditField(Sections = "Base UI")]
  public UIBButton m_okButton;
  [CustomEditField(Sections = "Text")]
  public UberText m_waitingForAuthText;
  [CustomEditField(Sections = "Text")]
  public UberText m_successHeadlineText;
  [CustomEditField(Sections = "Text")]
  public UberText m_failHeadlineText;
  [CustomEditField(Sections = "Text")]
  public UberText m_failDetailsText;
  [CustomEditField(Sections = "Base UI")]
  public StoreMiniSummary m_miniSummary;
  private bool m_showingSuccess;
  private MoneyOrGTAPPTransaction m_moneyOrGTAPPTransaction;
  private List<StorePurchaseAuth.AckPurchaseResultListener> m_ackPurchaseResultListeners = new List<StorePurchaseAuth.AckPurchaseResultListener>();
  private List<Action> m_cancelButtonListeners = new List<Action>();
  private List<Action> m_exitListeners = new List<Action>();
  private StorePurchaseAuth.InternalButtonStyle m_buttonStyle;

  protected override void Awake()
  {
    base.Awake();
    this.m_miniSummary.gameObject.SetActive(false);
    this.m_okButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOkayButtonPressed));
    this.SetButtonStyle(StorePurchaseAuth.InternalButtonStyle.NoButton);
  }

  public void Show(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    bool isZeroCostLicense,
    StorePurchaseAuth.ButtonStyle waitButtonStyle = StorePurchaseAuth.ButtonStyle.NoButton)
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    this.StartNewTransaction(moneyOrGTAPPTransaction, isZeroCostLicense, waitButtonStyle);
    this.m_spell.ActivateState(SpellStateType.BIRTH);
    if (this.m_moneyOrGTAPPTransaction != null && this.m_moneyOrGTAPPTransaction.ShouldShowMiniSummary())
      this.ShowMiniSummary();
    else
      this.m_root.UpdateSlices();
    Navigation.PushBlockBackingOut();
    this.DoShowAnimation();
  }

  public void StartNewTransaction(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    bool isZeroCostLicense,
    StorePurchaseAuth.ButtonStyle waitButtonStyle = StorePurchaseAuth.ButtonStyle.NoButton)
  {
    this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
    this.m_showingSuccess = false;
    if (isZeroCostLicense)
    {
      this.m_waitingForAuthText.Text = GameStrings.Get("GLUE_STORE_AUTH_ZERO_COST_WAITING");
      this.m_successHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_ZERO_COST_SUCCESS_HEADLINE");
      this.m_failHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_ZERO_COST_FAIL_HEADLINE");
    }
    else
    {
      this.m_waitingForAuthText.Text = GameStrings.Get("GLUE_STORE_AUTH_WAITING");
      this.m_successHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_SUCCESS_HEADLINE");
      this.m_failHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_FAIL_HEADLINE");
    }
    this.SetButtonStyle((StorePurchaseAuth.InternalButtonStyle) waitButtonStyle);
    this.m_waitingForAuthText.gameObject.SetActive(true);
    this.m_successHeadlineText.gameObject.SetActive(false);
    this.m_failHeadlineText.gameObject.SetActive(false);
    this.m_failDetailsText.gameObject.SetActive(false);
    if (moneyOrGTAPPTransaction == null || !moneyOrGTAPPTransaction.PMTProductID.HasValue || !(bool) (UnityEngine.Object) this.m_miniSummary || !this.m_miniSummary.gameObject.activeSelf || !ProductId.IsValid(moneyOrGTAPPTransaction.PMTProductID.Value))
      return;
    this.m_miniSummary.SetDetails(ProductId.CreateFrom(moneyOrGTAPPTransaction.PMTProductID.Value), 1);
  }

  public void ShowPurchaseLocked(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    bool isZeroCostLicense,
    StorePurchaseAuth.ButtonStyle waitButtonStyle,
    StorePurchaseAuth.PurchaseLockedDialogCallback purchaseLockedCallback)
  {
    this.Show(moneyOrGTAPPTransaction, isZeroCostLicense, waitButtonStyle);
    string empty = string.Empty;
    if (moneyOrGTAPPTransaction.Provider.HasValue)
    {
      switch (moneyOrGTAPPTransaction.Provider.Value)
      {
        case BattlePayProvider.BP_PROVIDER_APPLE:
          empty = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_APPLE");
          break;
        case BattlePayProvider.BP_PROVIDER_GOOGLE_PLAY:
          empty = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_GOOGLE");
          break;
        case BattlePayProvider.BP_PROVIDER_AMAZON:
          empty = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_AMAZON");
          break;
        default:
          empty = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_DEFAULT");
          break;
      }
    }
    string str = GameStrings.Format("GLUE_STORE_PURCHASE_LOCK_DESCRIPTION", (object) empty);
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_STORE_PURCHASE_LOCK_HEADER"),
      m_confirmText = GameStrings.Get("GLOBAL_CANCEL"),
      m_cancelText = GameStrings.Get("GLOBAL_HELP"),
      m_text = str,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_iconSet = AlertPopup.PopupInfo.IconSet.Alternate,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, data) =>
      {
        if (purchaseLockedCallback == null)
          return;
        purchaseLockedCallback(response == AlertPopup.Response.CANCEL);
      })
    });
  }

  public override void Hide()
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    Navigation.PopBlockBackingOut();
    this.DoHideAnimation((UIBPopup.OnAnimationComplete) (() =>
    {
      this.SetButtonStyle(StorePurchaseAuth.InternalButtonStyle.NoButton);
      this.m_miniSummary.gameObject.SetActive(false);
      this.m_spell.ActivateState(SpellStateType.NONE);
    }));
  }

  public bool CompletePurchaseSuccess(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction)
  {
    if (!this.gameObject.activeInHierarchy)
      return false;
    bool showMiniSummary = false;
    if (moneyOrGTAPPTransaction != null)
      showMiniSummary = moneyOrGTAPPTransaction.ShouldShowMiniSummary();
    this.ShowPurchaseSuccess(moneyOrGTAPPTransaction, showMiniSummary);
    return true;
  }

  public bool CompletePurchaseFailure(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    string failDetails,
    Network.PurchaseErrorInfo.ErrorType error)
  {
    if (!this.gameObject.activeInHierarchy)
      return false;
    bool showMiniSummary = false;
    if (moneyOrGTAPPTransaction != null)
      showMiniSummary = moneyOrGTAPPTransaction.ShouldShowMiniSummary();
    this.ShowPurchaseFailure(moneyOrGTAPPTransaction, failDetails, showMiniSummary, error);
    return true;
  }

  public void ShowPreviousPurchaseSuccess(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    StorePurchaseAuth.ButtonStyle buttonStyle = StorePurchaseAuth.ButtonStyle.NoButton)
  {
    this.Show(moneyOrGTAPPTransaction, false, buttonStyle);
    this.ShowPurchaseSuccess(moneyOrGTAPPTransaction, true);
  }

  public void ShowPreviousPurchaseFailure(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    string failDetails,
    StorePurchaseAuth.ButtonStyle buttonStyle,
    Network.PurchaseErrorInfo.ErrorType error)
  {
    this.Show(moneyOrGTAPPTransaction, false, buttonStyle);
    this.ShowPurchaseFailure(moneyOrGTAPPTransaction, failDetails, true, error);
  }

  public void ShowPurchaseMethodFailure(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    string failDetails,
    StorePurchaseAuth.ButtonStyle buttonStyle,
    Network.PurchaseErrorInfo.ErrorType error)
  {
    this.Show(moneyOrGTAPPTransaction, false, buttonStyle);
    this.ShowPurchaseFailure(moneyOrGTAPPTransaction, failDetails, false, error);
  }

  public void RegisterAckPurchaseResultListener(
    StorePurchaseAuth.AckPurchaseResultListener listener)
  {
    if (this.m_ackPurchaseResultListeners.Contains(listener))
      return;
    this.m_ackPurchaseResultListeners.Add(listener);
  }

  public void RemoveAckPurchaseResultListener(
    StorePurchaseAuth.AckPurchaseResultListener listener)
  {
    this.m_ackPurchaseResultListeners.Remove(listener);
  }

  public void RegisterCancelButtonListener(Action listener)
  {
    if (this.m_cancelButtonListeners.Contains(listener))
      return;
    this.m_cancelButtonListeners.Add(listener);
  }

  public void RemoveCancelButtonListener(Action listener) => this.m_cancelButtonListeners.Remove(listener);

  public void RegisterExitListener(Action listener)
  {
    if (this.m_exitListeners.Contains(listener))
      return;
    this.m_exitListeners.Add(listener);
  }

  public void RemoveExitListener(Action listener) => this.m_exitListeners.Remove(listener);

  public bool HideCancelButton()
  {
    if (this.m_showingSuccess || this.m_buttonStyle != StorePurchaseAuth.InternalButtonStyle.Cancel)
      return false;
    this.SetButtonStyle(StorePurchaseAuth.InternalButtonStyle.NoButton);
    return true;
  }

  private void SetButtonStyle(StorePurchaseAuth.InternalButtonStyle buttonStyle)
  {
    if (buttonStyle == this.m_buttonStyle)
      return;
    this.m_buttonStyle = buttonStyle;
    string text;
    switch (buttonStyle)
    {
      case StorePurchaseAuth.InternalButtonStyle.Ok:
      case StorePurchaseAuth.InternalButtonStyle.BackWithOkText:
        text = "GLOBAL_OKAY";
        break;
      case StorePurchaseAuth.InternalButtonStyle.Back:
        text = "GLOBAL_BACK";
        break;
      case StorePurchaseAuth.InternalButtonStyle.Cancel:
        text = "GLOBAL_CANCEL";
        break;
      default:
        text = (string) null;
        break;
    }
    if (text == null)
    {
      this.m_okButton.gameObject.SetActive(false);
    }
    else
    {
      this.m_okButton.SetText(text);
      this.m_okButton.gameObject.SetActive(true);
      LayerUtils.SetLayer((Component) this.m_okButton, GameLayer.HighPriorityUI);
    }
  }

  private void OnOkayButtonPressed(UIEvent e)
  {
    if (this.m_showingSuccess)
    {
      MoneyOrGTAPPTransaction gtappTransaction = this.m_moneyOrGTAPPTransaction;
      long? pmtProductId;
      int num;
      if (gtappTransaction == null)
      {
        num = 0;
      }
      else
      {
        pmtProductId = gtappTransaction.PMTProductID;
        num = pmtProductId.HasValue ? 1 : 0;
      }
      Network.Bundle bundle1;
      if (num != 0)
      {
        pmtProductId = this.m_moneyOrGTAPPTransaction.PMTProductID;
        if (ProductId.IsValid(pmtProductId.Value))
        {
          StoreManager storeManager = StoreManager.Get();
          pmtProductId = this.m_moneyOrGTAPPTransaction.PMTProductID;
          ProductId from = ProductId.CreateFrom(pmtProductId.Value);
          bundle1 = storeManager.GetBundleFromPmtProductId(from);
          goto label_8;
        }
      }
      bundle1 = (Network.Bundle) null;
label_8:
      Network.Bundle bundle2 = bundle1;
      string str = (string) null;
      if ((Record) bundle2 != (Record) null && bundle2.Items != null)
      {
        Network.BundleItem bundleItem = bundle2.Items.FirstOrDefault<Network.BundleItem>((Func<Network.BundleItem, bool>) (i => i.ItemType == ProductType.PRODUCT_TYPE_HERO));
        if ((Record) bundleItem != (Record) null)
        {
          string boughtHeroCardId = GameUtils.TranslateDbIdToCardId(bundleItem.ProductData);
          CardHeroDbfRecord record = GameDbf.CardHero.GetRecord((Predicate<CardHeroDbfRecord>) (dbf => GameUtils.TranslateDbIdToCardId(dbf.CardId) == boughtHeroCardId));
          if (record != null)
            str = (string) record.PurchaseCompleteMsg;
        }
      }
      if (!string.IsNullOrEmpty(str))
      {
        this.Hide();
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_STORE_AUTH_SUCCESS_HEADLINE"),
          m_text = str,
          m_showAlertIcon = false,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, data) => this.OnOkayButtonPressed_Finish())
        };
        DialogManager.Get().ShowPopup(info);
        return;
      }
    }
    this.OnOkayButtonPressed_Finish();
  }

  private void OnOkayButtonPressed_Finish()
  {
    switch (this.m_buttonStyle)
    {
      case StorePurchaseAuth.InternalButtonStyle.Ok:
        this.Hide();
        foreach (StorePurchaseAuth.AckPurchaseResultListener purchaseResultListener in this.m_ackPurchaseResultListeners.ToArray())
          purchaseResultListener(this.m_showingSuccess, this.m_moneyOrGTAPPTransaction);
        break;
      case StorePurchaseAuth.InternalButtonStyle.Back:
      case StorePurchaseAuth.InternalButtonStyle.BackWithOkText:
        foreach (Action action in this.m_exitListeners.ToArray())
          action();
        break;
      case StorePurchaseAuth.InternalButtonStyle.Cancel:
        this.Hide();
        foreach (Action action in this.m_cancelButtonListeners.ToArray())
          action();
        break;
      default:
        this.Hide();
        break;
    }
  }

  private void ShowPurchaseSuccess(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    bool showMiniSummary)
  {
    this.m_showingSuccess = true;
    this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
    this.SetButtonStyle(StorePurchaseAuth.InternalButtonStyle.Ok);
    if (showMiniSummary)
      this.ShowMiniSummary();
    this.m_waitingForAuthText.gameObject.SetActive(false);
    this.m_successHeadlineText.gameObject.SetActive(true);
    this.m_failHeadlineText.gameObject.SetActive(false);
    this.m_failDetailsText.gameObject.SetActive(false);
    this.m_spell.ActivateState(SpellStateType.ACTION);
  }

  private void ShowPurchaseFailure(
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction,
    string failDetails,
    bool showMiniSummary,
    Network.PurchaseErrorInfo.ErrorType error)
  {
    this.m_showingSuccess = false;
    this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
    if (error == Network.PurchaseErrorInfo.ErrorType.PRODUCT_EVENT_HAS_ENDED && (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.TAVERN_BRAWL) || SceneMgr.Get().IsModeRequested(SceneMgr.Mode.DRAFT)))
      this.SetButtonStyle(StorePurchaseAuth.InternalButtonStyle.BackWithOkText);
    else
      this.SetButtonStyle(StorePurchaseAuth.InternalButtonStyle.Ok);
    if (showMiniSummary)
      this.ShowMiniSummary();
    this.m_failDetailsText.Text = failDetails;
    this.m_waitingForAuthText.gameObject.SetActive(false);
    this.m_successHeadlineText.gameObject.SetActive(false);
    this.m_failHeadlineText.gameObject.SetActive(true);
    this.m_failDetailsText.gameObject.SetActive(true);
    this.m_spell.ActivateState(SpellStateType.DEATH);
  }

  private void ShowMiniSummary()
  {
    MoneyOrGTAPPTransaction gtappTransaction = this.m_moneyOrGTAPPTransaction;
    if ((gtappTransaction != null ? (!gtappTransaction.PMTProductID.HasValue ? 1 : 0) : 1) != 0)
      return;
    long? pmtProductId = this.m_moneyOrGTAPPTransaction.PMTProductID;
    if (!ProductId.IsValid(pmtProductId.Value))
      return;
    StoreMiniSummary miniSummary = this.m_miniSummary;
    pmtProductId = this.m_moneyOrGTAPPTransaction.PMTProductID;
    ProductId from = ProductId.CreateFrom(pmtProductId.Value);
    miniSummary.SetDetails(from, 1);
    this.m_miniSummary.gameObject.SetActive(true);
    this.m_root.UpdateSlices();
  }

  public delegate void AckPurchaseResultListener(
    bool success,
    MoneyOrGTAPPTransaction moneyOrGTAPPTransaction);

  public delegate void PurchaseLockedDialogCallback(bool showHelp);

  private enum InternalButtonStyle
  {
    Unset,
    NoButton,
    Ok,
    Back,
    BackWithOkText,
    Cancel,
  }

  public enum ButtonStyle
  {
    NoButton = 1,
    Back = 3,
    Cancel = 5,
  }
}
