using Hearthstone.UI;
using UnityEngine;

public class StoreQuantityPrompt : UIBPopup
{
  public UIBButton m_okayButton;
  public UIBButton m_cancelButton;
  public Clickable m_clickCatcherClickable;
  public UberText m_messageText;
  public UberText m_quantityText;
  private int m_currentMaxQuantity;
  private StoreQuantityPrompt.OkayListener m_currentOkayListener;
  private StoreQuantityPrompt.CancelListener m_currentCancelListener;

  protected override void Awake()
  {
    base.Awake();
    this.m_quantityText.RichText = false;
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSubmitPressed));
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
    this.m_clickCatcherClickable.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
    Debug.Log((object) ("show postition = " + (object) this.m_showPosition + " " + (object) this.m_showScale));
  }

  public bool Show(
    int maxQuantity,
    StoreQuantityPrompt.OkayListener delOkay = null,
    StoreQuantityPrompt.CancelListener delCancel = null)
  {
    if (this.m_shown)
      return false;
    this.m_currentMaxQuantity = maxQuantity;
    this.m_messageText.Text = GameStrings.Format("GLUE_STORE_QUANTITY_MESSAGE", (object) maxQuantity);
    this.m_shown = true;
    this.m_currentOkayListener = delOkay;
    this.m_currentCancelListener = delCancel;
    this.m_quantityText.Text = string.Empty;
    this.gameObject.SetActive(true);
    Debug.Log((object) ("show postition2 = " + (object) this.m_showPosition + " " + (object) this.m_showScale));
    this.DoShowAnimation(new UIBPopup.OnAnimationComplete(this.ShowInput));
    return true;
  }

  protected override void Hide(bool animate)
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    this.HideInput();
    this.DoHideAnimation(!animate, (UIBPopup.OnAnimationComplete) (() => this.gameObject.SetActive(false)));
  }

  private bool GetQuantity(out int quantity)
  {
    quantity = -1;
    if (!int.TryParse(this.m_quantityText.Text, out quantity))
    {
      Debug.LogWarning((object) string.Format("GeneralStore.OnStoreQuantityOkayPressed: invalid quantity='{0}'", (object) this.m_quantityText.Text));
      return false;
    }
    if (quantity <= 0)
    {
      Log.Store.Print(string.Format("GeneralStore.OnStoreQuantityOkayPressed: quantity {0} must be positive", (object) quantity));
      return false;
    }
    if (quantity <= this.m_currentMaxQuantity)
      return true;
    Log.Store.Print(string.Format("GeneralStore.OnStoreQuantityOkayPressed: quantity {0} is larger than max allowed quantity ({1})", (object) quantity, (object) this.m_currentMaxQuantity));
    return false;
  }

  private void Submit()
  {
    this.Hide(true);
    int quantity = -1;
    if (this.GetQuantity(out quantity))
      this.FireOkayEvent(quantity);
    else
      this.FireCancelEvent();
  }

  public void Cancel()
  {
    this.Hide(true);
    this.FireCancelEvent();
  }

  private void OnSubmitPressed(UIEvent e) => this.Submit();

  private void OnCancelPressed(UIEvent e) => this.Cancel();

  private void FireOkayEvent(int quantity)
  {
    if (this.m_currentOkayListener != null)
      this.m_currentOkayListener(quantity);
    this.m_currentOkayListener = (StoreQuantityPrompt.OkayListener) null;
  }

  private void FireCancelEvent()
  {
    if (this.m_currentCancelListener != null)
      this.m_currentCancelListener();
    this.m_currentCancelListener = (StoreQuantityPrompt.CancelListener) null;
  }

  private void ShowInput()
  {
    this.m_quantityText.gameObject.SetActive(false);
    Camera projectionCameraForObject = CameraUtils.FindProjectionCameraForObject(this.gameObject);
    Bounds bounds = this.m_quantityText.GetBounds();
    Vector3 min = bounds.min;
    Vector3 max = bounds.max;
    Rect guiViewportRect = CameraUtils.CreateGUIViewportRect(projectionCameraForObject, min, max);
    UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams()
    {
      m_owner = this.gameObject,
      m_number = true,
      m_rect = guiViewportRect,
      m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(this.OnInputUpdated),
      m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
      m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
      m_font = this.m_quantityText.GetLocalizedFont(),
      m_alignment = new TextAnchor?(TextAnchor.MiddleCenter),
      m_maxCharacters = 2,
      m_touchScreenKeyboardHideInput = true,
      m_touchScreenKeyboardType = 0
    };
    UniversalInputManager.Get().UseTextInput(parms);
  }

  private void HideInput()
  {
    UniversalInputManager.Get().CancelTextInput(this.gameObject);
    this.m_quantityText.gameObject.SetActive(true);
  }

  private void ClearInput() => UniversalInputManager.Get().SetInputText("");

  private void OnInputUpdated(string input) => this.m_quantityText.Text = input;

  private void OnInputComplete(string input)
  {
    this.m_quantityText.Text = input;
    this.Submit();
  }

  private void OnInputCanceled(bool userRequested, GameObject requester)
  {
    this.m_quantityText.Text = string.Empty;
    this.Cancel();
  }

  public delegate void OkayListener(int quantity);

  public delegate void CancelListener();
}
