using Hearthstone.UI;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class CardBackInfoManager : MonoBehaviour, IStore
{
  private const string STATE_MAKE_FAVORITE = "MAKE_FAVORITE";
  private const string STATE_SUFFICIENT_CURRENCY = "SUFFICIENT_CURRENCY";
  private const string STATE_INSUFFICIENT_CURRENCY = "INSUFFICIENT_CURRENCY";
  private const string STATE_DISABLED = "DISABLED";
  private const string STATE_VISIBLE = "VISIBLE";
  private const string STATE_HIDDEN = "HIDDEN";
  private const string STATE_BLOCK_SCREEN = "BLOCK_SCREEN";
  private const string STATE_UNBLOCK_SCREEN = "UNBLOCK_SCREEN";
  public GameObject m_previewPane;
  public GameObject m_cardBackContainer;
  public UberText m_title;
  public UberText m_description;
  public UIBButton m_buyButton;
  public UIBButton m_favoriteButton;
  public PegUIElement m_offClicker;
  public float m_animationTime = 0.5f;
  public AsyncReference m_userActionVisualControllerReference;
  public AsyncReference m_visibilityVisualControllerReference;
  public AsyncReference m_fullScreenBlockerWidgetReference;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_enterPreviewSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_exitPreviewSound;
  private int? m_currentCardBackIdx;
  private GameObject m_currentCardBack;
  private bool m_animating;
  private VisualController m_userActionVisualController;
  private VisualController m_visibilityVisualController;
  private Widget m_fullScreenBlockerWidget;
  private bool m_isStoreOpen;
  private bool m_isStoreTransactionActive;
  private static CardBackInfoManager s_instance;
  private static bool s_isReadyingInstance;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public bool IsPreviewing { get; private set; }

  public event Action OnOpened;

  public event Action<StoreClosedArgs> OnClosed;

  public event Action OnReady;

  public event Action<BuyProductEventArgs> OnProductPurchaseAttempt;

  public event Action OnProductOpened;

  public static CardBackInfoManager Get() => CardBackInfoManager.s_instance;

  public static void EnterPreviewWhenReady(CollectionCardVisual cardVisual)
  {
    CardBackInfoManager cardBackInfoManager = CardBackInfoManager.Get();
    if ((UnityEngine.Object) cardBackInfoManager != (UnityEngine.Object) null)
      cardBackInfoManager.EnterPreview(cardVisual);
    else if (CardBackInfoManager.s_isReadyingInstance)
    {
      Debug.LogWarning((object) "CardBackInfoManager:EnterPreviewWhenReady called while the info manager instance was being readied");
    }
    else
    {
      Widget widget = (Widget) WidgetInstance.Create("CardBackInfoManager.prefab:d53d863de659e4cce97ba2ce0107fb49");
      if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "CardBackInfoManager:EnterPreviewWhenReady failed to create widget instance");
      }
      else
      {
        CardBackInfoManager.s_isReadyingInstance = true;
        widget.RegisterReadyListener((Action<object>) (_ =>
        {
          CardBackInfoManager.s_instance = widget.GetComponentInChildren<CardBackInfoManager>();
          CardBackInfoManager.s_isReadyingInstance = false;
          if ((UnityEngine.Object) CardBackInfoManager.s_instance == (UnityEngine.Object) null)
            Debug.LogError((object) "CardBackInfoManager:EnterPreviewWhenReady created widget instance but failed to get CardBackInfoManager component");
          else
            CardBackInfoManager.s_instance.EnterPreview(cardVisual);
        }), (object) null, true);
      }
    }
  }

  public static bool IsLoadedAndShowingPreview() => (bool) (UnityEngine.Object) CardBackInfoManager.s_instance && CardBackInfoManager.s_instance.IsPreviewing;

  private void Awake()
  {
    this.m_previewPane.SetActive(false);
    this.SetupUI();
  }

  private void Start()
  {
    this.m_userActionVisualControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnUserActionVisualControllerReady));
    this.m_visibilityVisualControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnVisibilityVisualControllerReady));
    this.m_fullScreenBlockerWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnFullScreenBlockerWidgetReady));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy()
  {
    if (CardBackManager.Get() != null)
      CardBackManager.Get().OnFavoriteCardBacksChanged -= new CardBackManager.FavoriteCardBacksChangedCallback(this.OnFavoriteCardBackChanged);
    CardBackInfoManager.s_instance = (CardBackInfoManager) null;
  }

  public void EnterPreview(CollectionCardVisual cardVisual)
  {
    Action onProductOpened = this.OnProductOpened;
    if (onProductOpened != null)
      onProductOpened();
    Actor actor = cardVisual.GetActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to obtain actor from card visual.");
    }
    else
    {
      CollectionCardBack component = actor.GetComponent<CollectionCardBack>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        Debug.LogError((object) "Actor does not contain a CollectionCardBack component!");
      else
        this.EnterPreview(component.GetCardBackId(), cardVisual);
    }
  }

  public void EnterPreview(int cardBackIdx, CollectionCardVisual cardVisual)
  {
    if (this.m_animating)
      return;
    if ((UnityEngine.Object) this.m_currentCardBack != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentCardBack);
      this.m_currentCardBack = (GameObject) null;
    }
    this.m_animating = true;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get()?.GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.HideCardBackTips();
    CardBackDbfRecord record = GameDbf.CardBack.GetRecord(cardBackIdx);
    this.m_title.Text = (string) record.Name;
    this.m_description.Text = (string) record.Description;
    this.m_currentCardBackIdx = new int?(cardBackIdx);
    this.IsPreviewing = true;
    this.SetupCardBackStore();
    this.UpdateView();
    if (!CardBackManager.Get().LoadCardBackByIndex(cardBackIdx, (CardBackManager.LoadCardBackData.LoadCardBackCallback) (cardBackData =>
    {
      GameObject gameObject = cardBackData.m_GameObject;
      gameObject.name = "CARD_BACK_" + (object) cardBackIdx;
      LayerUtils.SetLayer(gameObject, this.m_cardBackContainer.gameObject.layer);
      GameUtils.SetParent(gameObject, this.m_cardBackContainer);
      this.m_currentCardBack = gameObject;
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        this.m_currentCardBack.transform.localPosition = Vector3.zero;
      }
      else
      {
        this.m_currentCardBack.transform.position = cardVisual.transform.position;
        Hashtable tweenHashTable1 = iTweenManager.Get().GetTweenHashTable();
        tweenHashTable1.Add((object) "name", (object) "FinishBigCardMove");
        tweenHashTable1.Add((object) "position", (object) this.m_cardBackContainer.transform.position);
        tweenHashTable1.Add((object) "time", (object) this.m_animationTime);
        iTween.MoveTo(this.m_currentCardBack.gameObject, tweenHashTable1);
        Hashtable tweenHashTable2 = iTweenManager.Get().GetTweenHashTable();
        tweenHashTable2.Add((object) "scale", (object) Vector3.one);
        tweenHashTable2.Add((object) "time", (object) this.m_animationTime);
        tweenHashTable2.Add((object) "easeType", (object) iTween.EaseType.easeOutQuad);
        iTween.ScaleTo(this.m_currentCardBack.gameObject, tweenHashTable2);
        Hashtable tweenHashTable3 = iTweenManager.Get().GetTweenHashTable();
        tweenHashTable3.Add((object) "amount", (object) new Vector3(0.0f, 0.0f, 75f));
        tweenHashTable3.Add((object) "time", (object) 2.5f);
        iTween.PunchRotation(this.m_currentCardBack, tweenHashTable3, false);
      }
      this.m_currentCardBack.transform.localScale = Vector3.one;
      this.m_currentCardBack.transform.localRotation = Quaternion.identity;
      this.m_previewPane.SetActive(true);
      this.m_offClicker.gameObject.SetActive(true);
      Hashtable tweenHashTable = iTweenManager.Get().GetTweenHashTable();
      tweenHashTable.Add((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f));
      tweenHashTable.Add((object) "time", (object) this.m_animationTime);
      tweenHashTable.Add((object) "easeType", (object) iTween.EaseType.easeOutCirc);
      tweenHashTable.Add((object) "oncomplete", (object) (Action<object>) (e => this.m_animating = false));
      iTween.ScaleFrom(this.m_previewPane, tweenHashTable);
    })))
    {
      Debug.LogError((object) string.Format("Unable to load card back ID {0} for preview.", (object) cardBackIdx));
      this.m_animating = false;
    }
    if (!string.IsNullOrEmpty(this.m_enterPreviewSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_enterPreviewSound);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = this.m_animationTime
    });
  }

  public void CancelPreview()
  {
    if (this.m_animating)
      return;
    this.ShutDownCardBackStore();
    Vector3 origScale = this.m_previewPane.transform.localScale;
    this.IsPreviewing = false;
    this.m_animating = true;
    iTween.ScaleTo(this.m_previewPane, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) (Action<object>) (e =>
    {
      this.m_animating = false;
      this.m_previewPane.transform.localScale = origScale;
      this.m_previewPane.SetActive(false);
      this.m_offClicker.gameObject.SetActive(false);
    })));
    iTween.ScaleTo(this.m_currentCardBack, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) (Action<object>) (e => this.m_currentCardBack.SetActive(false))));
    if (!string.IsNullOrEmpty(this.m_exitPreviewSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_exitPreviewSound);
    this.m_screenEffectsHandle.StopEffect();
  }

  private void OnUserActionVisualControllerReady(VisualController visualController)
  {
    this.m_userActionVisualController = visualController;
    this.UpdateView();
    if (this.OnReady == null)
      return;
    this.OnReady();
  }

  private void OnVisibilityVisualControllerReady(VisualController visualController)
  {
    this.m_visibilityVisualController = visualController;
    this.UpdateView();
  }

  private void OnFullScreenBlockerWidgetReady(Widget fullScreenBlockerWidget)
  {
    this.m_fullScreenBlockerWidget = fullScreenBlockerWidget;
    this.UpdateView();
  }

  private void SetupUI()
  {
    this.m_buyButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBuyButtonReleased()));
    this.m_favoriteButton.GetComponentInChildren<UberText>(true).Text = CardBackManager.Get().MultipleFavoriteCardBacksEnabled() ? "GLUE_COLLECTION_MANAGER_FAVORITE_BUTTON_MULTIPLE" : "GLUE_COLLECTION_MANAGER_FAVORITE_BUTTON";
    this.m_favoriteButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      if (!this.m_currentCardBackIdx.HasValue)
      {
        Debug.LogError((object) "CardBackInfoManager:FavoriteButtonRelease: m_currentCardBackIdx did not have a value");
      }
      else
      {
        CardBackManager.Get().HandleFavoriteToggle(this.m_currentCardBackIdx.Value);
        this.CancelPreview();
      }
    }));
    this.m_offClicker.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CancelPreview()));
    this.m_offClicker.AddEventListener(UIEventType.RIGHTCLICK, (UIEvent.Handler) (e => this.CancelPreview()));
    CardBackManager.Get().OnFavoriteCardBacksChanged += new CardBackManager.FavoriteCardBacksChangedCallback(this.OnFavoriteCardBackChanged);
  }

  public void OnFavoriteCardBackChanged(int cardBackId, bool isFavorite) => this.UpdateView();

  private void OnBuyButtonReleased()
  {
    if (!this.m_currentCardBackIdx.HasValue)
    {
      Debug.LogError((object) "CardBackInfoManager:OnBuyButtonReleased: m_currentCardBackIdx did not have a value");
    }
    else
    {
      this.m_visibilityVisualController.SetState("HIDDEN");
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
      info.m_headerText = GameStrings.Format("GLUE_CARD_BACK_PURCHASE_CONFIRMATION_HEADER");
      info.m_text = GameStrings.Format("GLUE_CARD_BACK_PURCHASE_CONFIRMATION_MESSAGE", (object) this.m_title.Text);
      info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
      info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
      AlertPopup.ResponseCallback responseCallback = (AlertPopup.ResponseCallback) ((response, userdata) =>
      {
        if (response == AlertPopup.Response.CONFIRM)
          this.StartPurchaseTransaction();
        else
          this.m_visibilityVisualController.SetState("VISIBLE");
      });
      info.m_responseCallback = responseCallback;
      DialogManager.Get().ShowPopup(info);
    }
  }

  private void UpdateView()
  {
    if ((UnityEngine.Object) this.m_userActionVisualController == (UnityEngine.Object) null || (UnityEngine.Object) this.m_visibilityVisualController == (UnityEngine.Object) null || (UnityEngine.Object) this.m_fullScreenBlockerWidget == (UnityEngine.Object) null || !this.m_currentCardBackIdx.HasValue)
      return;
    CardBackManager cardBackManager = CardBackManager.Get();
    bool enabled = false;
    if (cardBackManager.IsCardBackOwned(this.m_currentCardBackIdx.Value))
      this.m_userActionVisualController.SetState("MAKE_FAVORITE");
    else if (!cardBackManager.IsCardBackPurchasableFromCollectionManager(this.m_currentCardBackIdx.Value))
    {
      this.m_userActionVisualController.SetState("DISABLED");
    }
    else
    {
      this.m_userActionVisualController.BindDataModel((IDataModel) cardBackManager.GetCollectionManagerCardBackPriceDataModel(this.m_currentCardBackIdx.Value));
      if (!cardBackManager.CanBuyCardBackFromCollectionManager(this.m_currentCardBackIdx.Value))
      {
        this.m_userActionVisualController.SetState("INSUFFICIENT_CURRENCY");
      }
      else
      {
        this.m_userActionVisualController.SetState("SUFFICIENT_CURRENCY");
        enabled = true;
      }
    }
    bool flag = cardBackManager.CanToggleFavoriteCardBack(this.m_currentCardBackIdx.Value);
    this.m_favoriteButton.SetEnabled(flag);
    this.m_favoriteButton.Flip(flag);
    this.m_buyButton.SetEnabled(enabled);
    this.m_buyButton.Flip(true);
  }

  private void BlockInputs(bool blocked)
  {
    if ((UnityEngine.Object) this.m_fullScreenBlockerWidget == (UnityEngine.Object) null)
      Debug.LogError((object) "Failed to toggle interface blocker from Duels Popup Manager");
    else if (blocked)
      this.m_fullScreenBlockerWidget.TriggerEvent("BLOCK_SCREEN");
    else
      this.m_fullScreenBlockerWidget.TriggerEvent("UNBLOCK_SCREEN");
  }

  private void SetupCardBackStore()
  {
    if (this.m_isStoreOpen)
      Debug.LogError((object) "CardBackInfoManager:SetupCardBackStore called when the store was already open");
    else if (!this.m_currentCardBackIdx.HasValue)
    {
      Debug.LogError((object) "CardBackInfoManager:SetupCardBackStore: m_currentCardBackIdx did not have a value");
    }
    else
    {
      StoreManager storeManager = StoreManager.Get();
      if (!storeManager.IsOpen())
        return;
      storeManager.SetupCardBackStore(this, this.m_currentCardBackIdx.Value);
      storeManager.RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchase));
      storeManager.RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
      storeManager.RegisterFailedPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnFailedPurchaseAck));
      BnetBar.Get()?.RefreshCurrency();
    }
  }

  private void ShutDownCardBackStore()
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
    storeManager.ShutDownCardBackStore();
    this.OnProductPurchaseAttempt = (Action<BuyProductEventArgs>) null;
    BnetBar.Get()?.RefreshCurrency();
    this.BlockInputs(false);
    this.m_isStoreOpen = false;
  }

  private void OnSuccessfulPurchase(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
  }

  private void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    this.EndPurchaseTransaction();
    CardBackManager.Get().AddNewCardBack(this.m_currentCardBackIdx.Value);
    CollectionManager.Get().RefreshCurrentPageContents();
    this.m_visibilityVisualController.SetState("VISIBLE");
    this.UpdateView();
  }

  private void OnFailedPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    this.EndPurchaseTransaction();
    this.m_visibilityVisualController.SetState("VISIBLE");
    this.UpdateView();
  }

  private void StartPurchaseTransaction()
  {
    if (!this.m_currentCardBackIdx.HasValue)
      Debug.LogError((object) "CardBackInfoManager:StartPurchaseTransaction: m_currentCardBackIdx did not have a value");
    else if (this.m_isStoreTransactionActive)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_CARD_BACK_PURCHASE_ERROR_HEADER"),
        m_text = GameStrings.Get("GLUE_CHECKOUT_ERROR_GENERIC_FAILURE"),
        m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      });
      Debug.LogWarning((object) "CardBackInfoManager:StartPurchaseTransaction: Attempted to start a card back transaction while an existing transaction was in progress");
    }
    else if (this.OnProductPurchaseAttempt == null)
    {
      Debug.LogError((object) "CardBackInfoManager:StartPurchaseTransaction: Attempted to start a card back purchase transaction while OnProductPurchaseAttempt was null");
    }
    else
    {
      this.m_isStoreTransactionActive = true;
      Network.Bundle backProductBundle = CardBackManager.Get().GetCollectionManagerCardBackProductBundle(this.m_currentCardBackIdx.Value);
      if ((Record) backProductBundle == (Record) null)
        Debug.LogError((object) ("CardBackInfoManager:StartPurchaseTransaction: Attempted to start a card back purchase transaction with a null product bundle for card back " + this.m_currentCardBackIdx.Value.ToString()));
      else
        this.OnProductPurchaseAttempt((BuyProductEventArgs) new BuyPmtProductEventArgs(backProductBundle, CurrencyType.GOLD, 1));
    }
  }

  private void CancelPurchaseTransaction() => this.EndPurchaseTransaction();

  private void EndPurchaseTransaction()
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
      Debug.LogError((object) "CardBackInfoManager:IStore.Open: Could not get the Bnet bar to reflect the required currency");
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

  IEnumerable<CurrencyType> IStore.GetVisibleCurrencies() => (IEnumerable<CurrencyType>) new CurrencyType[1]
  {
    CurrencyType.GOLD
  };
}
