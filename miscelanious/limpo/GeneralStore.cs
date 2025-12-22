using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStore : Store
{
  [CustomEditField(Sections = "General Store")]
  public GameObject m_mainPanel;
  [CustomEditField(Sections = "General Store")]
  public GameObject m_buyGoldPanel;
  [CustomEditField(Sections = "General Store")]
  public GameObject m_buyMoneyPanel;
  [CustomEditField(Sections = "General Store")]
  public GameObject m_buyEmptyPanel;
  [CustomEditField(ListTable = true, Sections = "General Store")]
  public List<GeneralStore.ModeObjects> m_modeObjects = new List<GeneralStore.ModeObjects>();
  [CustomEditField(Sections = "General Store")]
  public MeshRenderer m_accentIcon;
  [CustomEditField(Sections = "General Store/Mode Buttons")]
  public GameObject m_modeButtonBlocker;
  [CustomEditField(Sections = "General Store/Text")]
  public UberText m_moneyCostText;
  [CustomEditField(Sections = "General Store/Text")]
  public UberText m_goldCostText;
  [CustomEditField(Sections = "General Store/Text")]
  public MultiSliceElement m_productDetailsContainer;
  [CustomEditField(Sections = "General Store/Text")]
  public UberText m_productDetailsHeadlineText;
  [CustomEditField(Sections = "General Store/Text")]
  public UberText m_productDetailsText;
  [CustomEditField(Sections = "General Store/Text")]
  public float m_productDetailsRegularHeight = 13f;
  [CustomEditField(Sections = "General Store/Text")]
  public float m_productDetailsExtendedHeight = 15.5f;
  [CustomEditField(Sections = "General Store/Text")]
  public UberText m_koreanProductDetailsText;
  [CustomEditField(Sections = "General Store/Text")]
  public UberText m_koreanWarningText;
  [CustomEditField(Sections = "General Store/Text")]
  public float m_koreanProductDetailsRegularHeight = 8f;
  [CustomEditField(Sections = "General Store/Text")]
  public float m_koreanProductDetailsExtendedHeight = 10.5f;
  [CustomEditField(Sections = "General Store/Text")]
  public GameObject m_chooseArrowContainer;
  [CustomEditField(Sections = "General Store/Text")]
  public UberText m_chooseArrowText;
  [CustomEditField(Sections = "General Store/Content")]
  public float m_contentFlipAnimationTime = 0.5f;
  [CustomEditField(Sections = "General Store/Content")]
  public iTween.EaseType m_contentFlipEaseType = iTween.EaseType.easeOutBounce;
  [CustomEditField(Sections = "General Store/Panes")]
  public GeneralStorePane m_defaultPane;
  [CustomEditField(Sections = "General Store/Panes")]
  public Vector3 m_paneSwapOutOffset = new Vector3(0.05f, 0.0f, 0.0f);
  [CustomEditField(Sections = "General Store/Panes")]
  public Vector3 m_paneSwapInOffset = new Vector3(0.0f, -0.05f, 0.0f);
  [CustomEditField(Sections = "General Store/Panes")]
  public float m_paneSwapAnimationTime = 1f;
  [CustomEditField(Sections = "General Store/Panes")]
  public UIBScrollable m_paneScrollbar;
  [CustomEditField(Sections = "General Store/Sounds", T = EditType.SOUND_PREFAB)]
  public string m_contentFlipSound;
  [CustomEditField(Sections = "Aspect Ratio")]
  public float m_rootScaleExtraWideAspectRatio = 1.9f;
  [CustomEditField(Sections = "Aspect Ratio")]
  public float m_rootXPosExtraWideAspectRatio = 0.077f;
  [CustomEditField(Sections = "Aspect Ratio")]
  public float m_rootZPosExtraWideAspectRatio = 0.0431f;
  private static readonly int MIN_GOLD_FOR_CHANGE_QTY_TOOLTIP = 500;
  private static readonly float FLIP_BUY_PANEL_ANIM_TIME = 0.1f;
  private static readonly Vector3 MAIN_PANEL_ANGLE_TO_ROTATE = new Vector3(0.3333333f, 0.0f, 0.0f);
  private static readonly GeneralStoreMode[] s_ContentOrdering = new GeneralStoreMode[2]
  {
    GeneralStoreMode.ADVENTURE,
    GeneralStoreMode.CARDS
  };
  private static readonly Vector3[] s_ContentTriangularPositions = new Vector3[3]
  {
    new Vector3(0.0f, 0.125f, 0.0f),
    new Vector3(0.0f, -0.064f, -0.109f),
    new Vector3(0.0f, -0.064f, 0.109f)
  };
  private static readonly Vector3[] s_ContentTriangularRotations = new Vector3[3]
  {
    new Vector3(-60f, 0.0f, -180f),
    new Vector3(0.0f, -180f, 0.0f),
    new Vector3(60f, 0.0f, 180f)
  };
  private static readonly Vector3[] s_MainPanelTriangularRotations = new Vector3[3]
  {
    new Vector3(0.0f, 0.0f, 0.0f),
    new Vector3(-240f, 0.0f, 0.0f),
    new Vector3(-120f, 0.0f, 0.0f)
  };
  private static GeneralStore s_instance;
  private GeneralStore.BuyPanelState m_buyPanelState;
  private bool m_staticTextResized;
  private GeneralStoreMode m_currentMode;
  private int m_settingNewModeCount;
  private ShakePane m_shakePane;
  private List<GeneralStore.ModeChanged> m_modeChangedListeners = new List<GeneralStore.ModeChanged>();
  private int m_currentContentPositionIdx;
  private MusicPlaylistType m_prevPlaylist;
  private Map<GeneralStoreMode, Vector3> m_paneStartPositions = new Map<GeneralStoreMode, Vector3>();
  private AssetHandle<Texture> m_accentTexture;

  protected override void Start()
  {
    base.Start();
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.SuccessfulPurchaseAckEvent));
    SoundManager.Get().Load((AssetReference) "gold_spend_plate_flip_on.prefab:e490542c7405fce45a46c7b9aad5aeab");
    SoundManager.Get().Load((AssetReference) "gold_spend_plate_flip_off.prefab:8e19277d18c845547af53064aade9b2c");
    this.UpdateModeButtons(this.m_currentMode);
    foreach (GeneralStore.ModeObjects modeObject in this.m_modeObjects)
    {
      if ((UnityEngine.Object) modeObject.m_content != (UnityEngine.Object) null)
        modeObject.m_content.gameObject.SetActive(modeObject.m_mode == this.m_currentMode);
    }
    this.m_shakePane = this.GetComponent<ShakePane>();
    if (!((UnityEngine.Object) this.m_offClicker != (UnityEngine.Object) null))
      return;
    this.m_offClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClosePressed));
  }

  protected override void Awake()
  {
    GeneralStore.s_instance = this;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_scaleMode = TransformUtil.IsExtraWideAspectRatio() ? CanvasScaleMode.HEIGHT : CanvasScaleMode.WIDTH;
    base.Awake();
    this.m_buyWithMoneyButton.SetText(GameStrings.Get("GLUE_STORE_BUY_TEXT"));
    this.m_buyWithGoldButton.SetText(GameStrings.Get("GLUE_STORE_BUY_TEXT"));
    foreach (GeneralStore.ModeObjects modeObject in this.m_modeObjects)
    {
      GeneralStoreContent content = modeObject.m_content;
      UIBButton button = modeObject.m_button;
      GeneralStoreMode mode = modeObject.m_mode;
      GeneralStorePane pane = modeObject.m_pane;
      if ((UnityEngine.Object) content != (UnityEngine.Object) null)
      {
        content.SetParentStore(this);
        content.RegisterCurrentBundleChanged((GeneralStoreContent.BundleChanged) ((goldBundle, moneyBundle) => this.UpdateCostAndButtonState(goldBundle, moneyBundle)));
      }
      if ((UnityEngine.Object) button != (UnityEngine.Object) null)
        button.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.SetMode(mode)));
      if ((UnityEngine.Object) pane != (UnityEngine.Object) null)
      {
        pane.transform.localPosition = this.m_paneSwapOutOffset;
        this.m_paneStartPositions[mode] = pane.m_paneContainer.transform.localPosition;
      }
    }
    if (!((UnityEngine.Object) this.m_defaultPane != (UnityEngine.Object) null))
      return;
    this.m_defaultPane.transform.localPosition = this.m_paneSwapOutOffset;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.SuccessfulPurchaseAckEvent));
    AssetHandle.SafeDispose<Texture>(ref this.m_accentTexture);
    this.m_mainPanel = (GameObject) null;
    GeneralStore.s_instance = (GeneralStore) null;
  }

  public GeneralStoreContent GetCurrentContent() => this.GetContent(this.m_currentMode);

  public GeneralStorePane GetCurrentPane() => this.GetPane(this.m_currentMode);

  public GeneralStoreContent GetContent(GeneralStoreMode mode) => this.m_modeObjects.Find((Predicate<GeneralStore.ModeObjects>) (obj => obj.m_mode == mode))?.m_content;

  public GeneralStorePane GetPane(GeneralStoreMode mode)
  {
    GeneralStore.ModeObjects modeObjects = this.m_modeObjects.Find((Predicate<GeneralStore.ModeObjects>) (obj => obj.m_mode == mode));
    return modeObjects != null && (UnityEngine.Object) modeObjects.m_pane != (UnityEngine.Object) null ? modeObjects.m_pane : this.m_defaultPane;
  }

  public void Close(bool closeWithAnimation)
  {
    if (!this.m_shown)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
      Navigation.RemoveHandler(new Navigation.NavigateBackHandler(GeneralStorePhoneCover.OnNavigateBack));
    Navigation.Pop();
    this.CloseImpl(closeWithAnimation);
  }

  public override void Close()
  {
    if (!this.m_shown)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
      Navigation.RemoveHandler(new Navigation.NavigateBackHandler(GeneralStorePhoneCover.OnNavigateBack));
    if (this.m_settingNewModeCount != 0)
      return;
    Navigation.GoBack();
  }

  public void SetMode(GeneralStoreMode mode) => this.StartCoroutine(this.AnimateAndUpdateStoreMode(this.m_currentMode, mode));

  public GeneralStoreMode GetMode() => this.m_currentMode;

  public void ShakeStore(
    float xRotationAmount,
    float shakeTime,
    float delay = 0.0f,
    float translateAmount = 0.0f)
  {
    if ((UnityEngine.Object) this.m_shakePane == (UnityEngine.Object) null || GeneralStoreMode.CARDS != this.m_currentMode)
      return;
    this.m_shakePane.Shake(xRotationAmount, shakeTime, delay, translateAmount);
  }

  public void SetDescription(string title, string desc, string warning = null)
  {
    this.HideChooseDescription();
    if ((UnityEngine.Object) this.m_productDetailsContainer != (UnityEngine.Object) null)
      this.m_productDetailsContainer.gameObject.SetActive(true);
    bool flag1 = StoreManager.Get().IsKoreanCustomer();
    bool flag2 = !string.IsNullOrEmpty(title);
    this.m_productDetailsHeadlineText.gameObject.SetActive(flag2);
    this.m_productDetailsText.gameObject.SetActive(!flag1);
    this.m_koreanProductDetailsText.gameObject.SetActive(flag1);
    this.m_koreanWarningText.gameObject.SetActive(flag1);
    this.m_productDetailsText.Height = flag2 ? this.m_productDetailsRegularHeight : this.m_productDetailsExtendedHeight;
    this.m_productDetailsHeadlineText.Text = title;
    this.m_koreanProductDetailsText.Text = desc;
    this.m_productDetailsText.Text = desc;
    this.m_koreanProductDetailsText.Height = flag2 ? this.m_koreanProductDetailsRegularHeight : this.m_koreanProductDetailsExtendedHeight;
    this.m_koreanWarningText.Text = warning == null ? string.Empty : warning;
    if (!((UnityEngine.Object) this.m_productDetailsContainer != (UnityEngine.Object) null))
      return;
    this.m_productDetailsContainer.UpdateSlices();
  }

  public void HideDescription()
  {
    if (!((UnityEngine.Object) this.m_productDetailsContainer != (UnityEngine.Object) null))
      return;
    this.m_productDetailsContainer.gameObject.SetActive(false);
  }

  public void SetChooseDescription(string chooseText)
  {
    this.HideDescription();
    this.SetAccentTexture((AssetHandle<Texture>) null);
    if ((UnityEngine.Object) this.m_chooseArrowContainer != (UnityEngine.Object) null)
      this.m_chooseArrowContainer.SetActive(true);
    if (!((UnityEngine.Object) this.m_chooseArrowText != (UnityEngine.Object) null))
      return;
    this.m_chooseArrowText.Text = chooseText;
  }

  public void HideChooseDescription()
  {
    if (!((UnityEngine.Object) this.m_chooseArrowContainer != (UnityEngine.Object) null))
      return;
    this.m_chooseArrowContainer.SetActive(false);
  }

  public void SetAccentTexture(AssetHandle<Texture> texture)
  {
    if (!((UnityEngine.Object) this.m_accentIcon != (UnityEngine.Object) null))
      return;
    bool flag = texture != null;
    this.m_accentIcon.gameObject.SetActive(flag);
    if (!flag)
      return;
    AssetHandle.Set<Texture>(ref this.m_accentTexture, texture);
    RendererExtension.GetMaterial((Renderer) this.m_accentIcon).mainTexture = (Texture) this.m_accentTexture;
  }

  public void HideAccentTexture()
  {
    if (!((UnityEngine.Object) this.m_accentIcon != (UnityEngine.Object) null))
      return;
    this.m_accentIcon.gameObject.SetActive(false);
  }

  public void HidePacksPane(bool hide)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (hide)
        this.StartCoroutine(this.AnimateAndUpdateStoreMode(GeneralStoreMode.CARDS, GeneralStoreMode.NONE));
      else
        this.StartCoroutine(this.AnimateAndUpdateStoreMode(GeneralStoreMode.NONE, GeneralStoreMode.CARDS));
    }
    else
      this.StartCoroutine(this.AnimateHideStorePane(hide));
  }

  public void ResumePreviousMusicPlaylist()
  {
    if (this.m_prevPlaylist == MusicPlaylistType.Invalid)
      return;
    MusicManager.Get().StartPlaylist(this.m_prevPlaylist);
  }

  public void RegisterModeChangedListener(GeneralStore.ModeChanged dlg) => this.m_modeChangedListeners.Add(dlg);

  public void UnregisterModeChangedListener(GeneralStore.ModeChanged dlg) => this.m_modeChangedListeners.Remove(dlg);

  public static GeneralStore Get() => GeneralStore.s_instance;

  public override bool IsReady() => true;

  public override void OnMoneySpent()
  {
    GeneralStoreContent currentContent = this.GetCurrentContent();
    if ((UnityEngine.Object) currentContent != (UnityEngine.Object) null)
      currentContent.Refresh();
    GeneralStorePane currentPane = this.GetCurrentPane();
    if (!((UnityEngine.Object) currentPane != (UnityEngine.Object) null))
      return;
    currentPane.Refresh();
  }

  public override void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance) => this.UpdateGoldButtonState(balance);

  protected override void ShowImpl(bool isTotallyFake)
  {
    if (this.m_shown)
      return;
    if ((UnityEngine.Object) this.m_root != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI && TransformUtil.IsExtraWideAspectRatio())
    {
      this.m_root.transform.localScale = Vector3.one * this.m_rootScaleExtraWideAspectRatio;
      TransformUtil.SetLocalPosX((Component) this.m_root.transform, this.m_rootXPosExtraWideAspectRatio);
      TransformUtil.SetLocalPosZ((Component) this.m_root.transform, this.m_rootZPosExtraWideAspectRatio);
    }
    this.m_prevPlaylist = MusicManager.Get().GetCurrentPlaylist();
    foreach (GeneralStore.ModeObjects modeObject in this.m_modeObjects)
    {
      GeneralStoreContent content = modeObject.m_content;
      GeneralStorePane pane = modeObject.m_pane;
      if ((UnityEngine.Object) content != (UnityEngine.Object) null)
        content.StoreShown((UnityEngine.Object) this.GetCurrentContent() == (UnityEngine.Object) content);
      if ((UnityEngine.Object) pane != (UnityEngine.Object) null)
        pane.StoreShown((UnityEngine.Object) this.GetCurrentPane() == (UnityEngine.Object) pane);
    }
    ShownUIMgr.Get().SetShownUI(ShownUIMgr.UI_WINDOW.GENERAL_STORE);
    FriendChallengeMgr.Get().OnStoreOpened();
    this.PreRender();
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.STORE);
    if (!(bool) UniversalInputManager.UsePhoneUI && !Options.Get().GetBool(Option.HAS_SEEN_GOLD_QTY_INSTRUCTION, false) && UserAttentionManager.CanShowAttentionGrabber("GeneralStore.Show:" + (object) Option.HAS_SEEN_GOLD_QTY_INSTRUCTION) && NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>().GetTotal() >= (long) GeneralStore.MIN_GOLD_FOR_CHANGE_QTY_TOOLTIP)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_attentionCategory = UserAttentionBlocker.NONE,
        m_headerText = GameStrings.Get("GLUE_STORE_GOLD_QTY_CHANGE_HEADER"),
        m_text = !UniversalInputManager.Get().IsTouchMode() ? GameStrings.Get("GLUE_STORE_GOLD_QTY_CHANGE_DESC") : GameStrings.Get("GLUE_STORE_GOLD_QTY_CHANGE_DESC_TOUCH"),
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      });
      Options.Get().SetBool(Option.HAS_SEEN_GOLD_QTY_INSTRUCTION, true);
    }
    this.UpdateGoldButtonState();
    this.m_shown = true;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.EnableFullScreenEffects(true);
    SoundManager.Get().LoadAndPlay((AssetReference) "Store_window_expand.prefab:050bf879a3e32d04999427c262baaf09", this.gameObject);
    this.DoShowAnimation((UIBPopup.OnAnimationComplete) (() =>
    {
      if (!(bool) UniversalInputManager.UsePhoneUI)
      {
        this.transform.localPosition = this.m_showPosition;
        this.m_root.transform.localPosition = Vector3.zero;
      }
      this.FireOpenedEvent();
    }));
  }

  private void OnClosePressed(UIEvent e)
  {
    HearthstoneCheckout service;
    if (!this.m_shown || ServiceManager.TryGet<HearthstoneCheckout>(out service) && service.IsInProgress || !StoreManager.Get().CanTapOutConfirmationUI())
      return;
    this.Close();
  }

  protected override void BuyWithMoney(UIEvent e)
  {
    GeneralStoreContent currentContent = this.GetCurrentContent();
    Network.Bundle bundle = currentContent.GetCurrentMoneyBundle();
    if ((Record) bundle == (Record) null)
    {
      Debug.LogWarning((object) "GeneralStore.OnBuyWithMoneyPressed(): SelectedBundle is null");
    }
    else
    {
      GeneralStoreContent.BuyEvent successBuyCB = (GeneralStoreContent.BuyEvent) (() => this.FireBuyWithMoneyEvent(bundle, 1));
      currentContent.TryBuyWithMoney(bundle, successBuyCB, (GeneralStoreContent.BuyEvent) null);
    }
  }

  protected override void BuyWithGold(UIEvent e)
  {
    GeneralStoreContent currentContent = this.GetCurrentContent();
    NoGTAPPTransactionData bundle = currentContent.GetCurrentGoldBundle();
    if (bundle == null)
    {
      Debug.LogWarning((object) "GeneralStore.OnBuyWithGoldPressed(): SelectedGoldPrice is null");
    }
    else
    {
      GeneralStoreContent.BuyEvent buyEvent = (GeneralStoreContent.BuyEvent) (() => this.FireBuyWithGoldEventNoGTAPP(bundle));
      currentContent.TryBuyWithGold(buyEvent, buyEvent);
    }
  }

  private void UpdateMoneyButtonState()
  {
    Store.BuyButtonState state = Store.BuyButtonState.ENABLED;
    if (!StoreManager.Get().IsOpen())
      state = Store.BuyButtonState.DISABLED;
    else if (!StoreManager.Get().IsBattlePayFeatureEnabled())
    {
      state = Store.BuyButtonState.DISABLED_FEATURE;
    }
    else
    {
      Network.Bundle currentMoneyBundle = this.GetCurrentContent().GetCurrentMoneyBundle();
      if ((Record) currentMoneyBundle == (Record) null || StoreManager.Get().IsProductAlreadyOwned(currentMoneyBundle))
        state = Store.BuyButtonState.DISABLED_OWNED;
    }
    this.SetMoneyButtonState(state);
  }

  private void UpdateGoldButtonState(NetCache.NetCacheGoldBalance balance)
  {
    Store.BuyButtonState state = Store.BuyButtonState.ENABLED;
    GeneralStoreContent currentContent = this.GetCurrentContent();
    if ((UnityEngine.Object) currentContent == (UnityEngine.Object) null)
      return;
    NoGTAPPTransactionData currentGoldBundle = currentContent.GetCurrentGoldBundle();
    if (currentGoldBundle == null)
      state = Store.BuyButtonState.DISABLED;
    else if (!StoreManager.Get().IsOpen())
      state = Store.BuyButtonState.DISABLED;
    else if (!StoreManager.Get().IsBuyWithGoldFeatureEnabled())
      state = Store.BuyButtonState.DISABLED_FEATURE;
    else if (balance == null)
    {
      state = Store.BuyButtonState.DISABLED;
    }
    else
    {
      long cost;
      if (!StoreManager.Get().GetGoldCostNoGTAPP(currentGoldBundle, out cost))
        state = Store.BuyButtonState.DISABLED_NO_TOOLTIP;
      else if (balance.GetTotal() < cost)
        state = Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD;
    }
    this.SetGoldButtonState(state);
  }

  private void UpdateGoldButtonState() => this.UpdateGoldButtonState(NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>());

  private void UpdateCostDisplay(NoGTAPPTransactionData goldBundle)
  {
    long cost;
    if (goldBundle == null || !StoreManager.Get().GetGoldCostNoGTAPP(goldBundle, out cost))
      this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_GOLD, string.Empty);
    else
      this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_GOLD, cost.ToString());
  }

  private void UpdateCostDisplay(Network.Bundle moneyBundle)
  {
    if ((Record) moneyBundle == (Record) null)
      this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_MONEY, GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_COST_OWNED_TEXT"));
    else
      this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_MONEY, StoreManager.Get().FormatCostBundle(moneyBundle));
  }

  private void UpdateCostDisplay(GeneralStore.BuyPanelState newPanelState, string costText = "")
  {
    switch (newPanelState)
    {
      case GeneralStore.BuyPanelState.BUY_GOLD:
        this.m_goldCostText.Text = costText;
        this.m_goldCostText.UpdateNow();
        break;
      case GeneralStore.BuyPanelState.BUY_MONEY:
        this.m_moneyCostText.Text = costText;
        this.m_moneyCostText.UpdateNow();
        break;
    }
    this.ShowBuyPanel(newPanelState);
  }

  private void ShowBuyPanel(GeneralStore.BuyPanelState setPanelState)
  {
    if (this.m_buyPanelState == setPanelState)
      return;
    GameObject buyPanelObject = this.GetBuyPanelObject(setPanelState);
    GameObject oldPanelObject = this.GetBuyPanelObject(this.m_buyPanelState);
    this.m_buyPanelState = setPanelState;
    iTween.StopByName(buyPanelObject, "rotation");
    iTween.StopByName(oldPanelObject, "rotation");
    buyPanelObject.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
    oldPanelObject.transform.localEulerAngles = Vector3.zero;
    buyPanelObject.SetActive(true);
    iTween.RotateTo(oldPanelObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 180f), (object) "isLocal", (object) true, (object) "time", (object) GeneralStore.FLIP_BUY_PANEL_ANIM_TIME, (object) "easeType", (object) iTween.EaseType.linear, (object) "oncomplete", (object) (Action<object>) (o => oldPanelObject.SetActive(false)), (object) "name", (object) "rotation"));
    iTween.RotateTo(buyPanelObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "isLocal", (object) true, (object) "time", (object) GeneralStore.FLIP_BUY_PANEL_ANIM_TIME, (object) "easeType", (object) iTween.EaseType.linear, (object) "name", (object) "rotation"));
    SoundManager.Get().LoadAndPlay((AssetReference) (setPanelState == GeneralStore.BuyPanelState.BUY_GOLD ? "gold_spend_plate_flip_on.prefab:e490542c7405fce45a46c7b9aad5aeab" : "gold_spend_plate_flip_off.prefab:8e19277d18c845547af53064aade9b2c"));
  }

  private GameObject GetBuyPanelObject(GeneralStore.BuyPanelState buyPanelState)
  {
    if (buyPanelState == GeneralStore.BuyPanelState.BUY_GOLD)
      return this.m_buyGoldPanel;
    return buyPanelState == GeneralStore.BuyPanelState.BUY_MONEY ? this.m_buyMoneyPanel : this.m_buyEmptyPanel;
  }

  public void RefreshContent()
  {
    GeneralStoreContent currentContent = this.GetCurrentContent();
    GeneralStorePane currentPane = this.GetCurrentPane();
    StoreManager storeManager = StoreManager.Get();
    this.BlockInterface(storeManager.TransactionInProgress() || storeManager.IsPromptShowing);
    if ((UnityEngine.Object) currentContent != (UnityEngine.Object) null)
      currentContent.Refresh();
    if (!((UnityEngine.Object) currentPane != (UnityEngine.Object) null))
      return;
    currentPane.Refresh();
  }

  protected override void Hide(bool animate)
  {
    if (this.m_settingNewModeCount > 0)
      return;
    if (ShownUIMgr.Get() != null)
      ShownUIMgr.Get().ClearShownUI();
    FriendChallengeMgr.Get().OnStoreClosed();
    GeneralStoreContent content = this.GetContent(GeneralStoreMode.CARDS);
    if ((UnityEngine.Object) content != (UnityEngine.Object) null && (UnityEngine.Object) this.GetCurrentContent() == (UnityEngine.Object) content)
    {
      GeneralStorePacksContent storePacksContent = content as GeneralStorePacksContent;
      if ((UnityEngine.Object) storePacksContent.m_quantityPrompt != (UnityEngine.Object) null)
        storePacksContent.m_quantityPrompt.Hide();
    }
    this.ResumePreviousMusicPlaylist();
    this.DoHideAnimation(!animate, new UIBPopup.OnAnimationComplete(((UIBPopup) this).OnHidden));
  }

  protected override void OnHidden()
  {
    this.m_shown = false;
    foreach (GeneralStore.ModeObjects modeObject in this.m_modeObjects)
    {
      GeneralStorePane pane = modeObject.m_pane;
      GeneralStoreContent content = modeObject.m_content;
      if ((UnityEngine.Object) pane != (UnityEngine.Object) null)
        pane.StoreHidden((UnityEngine.Object) this.GetCurrentPane() == (UnityEngine.Object) pane);
      if ((UnityEngine.Object) content != (UnityEngine.Object) null)
        content.StoreHidden((UnityEngine.Object) this.GetCurrentContent() == (UnityEngine.Object) content);
    }
  }

  private void PreRender()
  {
    if (!this.m_staticTextResized)
    {
      this.m_buyWithMoneyButton.m_ButtonText.UpdateNow();
      this.m_buyWithGoldButton.m_ButtonText.UpdateNow();
      this.m_staticTextResized = true;
    }
    this.RefreshContent();
  }

  private bool IsContentFlipClockwise(GeneralStoreMode oldMode, GeneralStoreMode newMode)
  {
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < GeneralStore.s_ContentOrdering.Length; ++index)
    {
      if (GeneralStore.s_ContentOrdering[index] == oldMode)
        num1 = index;
      else if (GeneralStore.s_ContentOrdering[index] == newMode)
        num2 = index;
    }
    return num1 < num2;
  }

  private IEnumerator AnimateAndUpdateStoreMode(
    GeneralStoreMode oldMode,
    GeneralStoreMode newMode)
  {
    GeneralStore generalStore = this;
    generalStore.ResetAnimations();
    while (generalStore.m_settingNewModeCount > 0)
      yield return (object) null;
    generalStore.FireModeChangedEvent(oldMode, newMode);
    if (generalStore.m_currentMode != newMode)
    {
      ++generalStore.m_settingNewModeCount;
      if ((UnityEngine.Object) generalStore.m_modeButtonBlocker != (UnityEngine.Object) null)
        generalStore.m_modeButtonBlocker.SetActive(true);
      generalStore.UpdateModeButtons(newMode);
      generalStore.m_currentMode = newMode;
      generalStore.StartCoroutine(generalStore.AnimateAndUpdateStorePane(oldMode, newMode));
      GeneralStoreContent prevContent = generalStore.GetContent(oldMode);
      GeneralStoreContent nextContent = generalStore.GetContent(newMode);
      if ((UnityEngine.Object) prevContent != (UnityEngine.Object) null)
      {
        prevContent.SetContentActive(false);
        prevContent.PreStoreFlipOut();
        while (!prevContent.AnimateExitStart())
          yield return (object) null;
        while (!prevContent.AnimateExitEnd())
          yield return (object) null;
      }
      bool clockwise = generalStore.IsContentFlipClockwise(oldMode, newMode);
      Vector3 contentPosition;
      Vector3 contentRotation;
      Vector3 lastPanelRotation;
      Vector3 newPanelRotation;
      generalStore.GetContentPositionIndex(clockwise, out contentPosition, out contentRotation, out lastPanelRotation, out newPanelRotation);
      if ((UnityEngine.Object) nextContent != (UnityEngine.Object) null)
      {
        nextContent.transform.localPosition = contentPosition;
        nextContent.transform.localEulerAngles = contentRotation;
        nextContent.gameObject.SetActive(true);
      }
      iTween.StopByName(generalStore.m_mainPanel, "PANEL_ROTATION");
      generalStore.m_mainPanel.transform.localEulerAngles = lastPanelRotation;
      bool rotationDone = false;
      float flipAnimTime = generalStore.m_contentFlipAnimationTime;
      float num = clockwise ? 1f : -1f;
      generalStore.ShakeStore(10f * num, 1.5f, flipAnimTime * 0.3f);
      if (!string.IsNullOrEmpty(generalStore.m_contentFlipSound))
        SoundManager.Get().LoadAndPlay((AssetReference) generalStore.m_contentFlipSound);
      Action<object> action = (Action<object>) (o =>
      {
        this.m_mainPanel.transform.localEulerAngles = newPanelRotation;
        rotationDone = true;
        if (!((UnityEngine.Object) prevContent != (UnityEngine.Object) null))
          return;
        prevContent.gameObject.SetActive(false);
      });
      if ((double) flipAnimTime > 0.0)
        iTween.RotateBy(generalStore.m_mainPanel, iTween.Hash((object) "name", (object) "PANEL_ROTATION", (object) "amount", (object) (GeneralStore.MAIN_PANEL_ANGLE_TO_ROTATE * num), (object) "time", (object) flipAnimTime, (object) "easetype", (object) generalStore.m_contentFlipEaseType, (object) "oncomplete", (object) action));
      else
        action((object) null);
      if ((UnityEngine.Object) nextContent != (UnityEngine.Object) null)
        nextContent.PreStoreFlipIn();
      while (!rotationDone)
        yield return (object) null;
      if ((UnityEngine.Object) nextContent != (UnityEngine.Object) null)
      {
        generalStore.UpdateCostAndButtonState(nextContent.GetCurrentGoldBundle(), nextContent.GetCurrentMoneyBundle());
        while (!nextContent.AnimateEntranceStart())
          yield return (object) null;
        while (!nextContent.AnimateEntranceEnd())
          yield return (object) null;
        nextContent.SetContentActive(true);
        nextContent.PostStoreFlipIn((double) flipAnimTime > 0.0);
      }
      if ((UnityEngine.Object) prevContent != (UnityEngine.Object) null)
        prevContent.PostStoreFlipOut();
      --generalStore.m_settingNewModeCount;
      generalStore.RefreshContent();
      while (generalStore.m_settingNewModeCount > 0)
        yield return (object) null;
      int currencyChangedVersion = StoreManager.Get().GetCurrencyChangedVersion();
      if (currencyChangedVersion != 0 && currencyChangedVersion != Options.Get().GetInt(Option.LATEST_SEEN_CURRENCY_CHANGED_VERSION) && UserAttentionManager.CanShowAttentionGrabber("GeneralStore.AnimateAndUpdateStoreMode:" + (object) Option.LATEST_SEEN_CURRENCY_CHANGED_VERSION))
      {
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_attentionCategory = UserAttentionBlocker.NONE,
          m_headerText = GameStrings.Get("GLUE_STORE_CURRENCY_CHANGED_HEADER"),
          m_text = GameStrings.Get("GLUE_STORE_CURRENCY_CHANGED_DESC"),
          m_showAlertIcon = false,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK
        });
        Options.Get().SetInt(Option.LATEST_SEEN_CURRENCY_CHANGED_VERSION, currencyChangedVersion);
      }
      if ((UnityEngine.Object) generalStore.m_modeButtonBlocker != (UnityEngine.Object) null)
        generalStore.m_modeButtonBlocker.SetActive(false);
      if (newMode == GeneralStoreMode.NONE)
        generalStore.ResumePreviousMusicPlaylist();
    }
  }

  private IEnumerator AnimateHideStorePane(bool hide)
  {
    GeneralStorePane nextPane;
    GeneralStorePane prevPane;
    if (hide)
    {
      prevPane = this.GetPane(GeneralStoreMode.CARDS);
      nextPane = this.m_defaultPane;
    }
    else
    {
      prevPane = this.m_defaultPane;
      nextPane = this.GetPane(GeneralStoreMode.CARDS);
    }
    ++this.m_settingNewModeCount;
    if ((UnityEngine.Object) prevPane != (UnityEngine.Object) null)
    {
      prevPane.PrePaneSwappedOut();
      while (!prevPane.AnimateExitStart())
        yield return (object) null;
      while (!prevPane.AnimateExitEnd())
        yield return (object) null;
      prevPane.PostPaneSwappedOut();
    }
    if ((double) this.m_paneSwapAnimationTime > 0.0)
    {
      if (!string.IsNullOrEmpty(this.m_contentFlipSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_contentFlipSound);
      int swapCount = 0;
      float num = 0.0f;
      if ((UnityEngine.Object) prevPane != (UnityEngine.Object) null)
      {
        ++swapCount;
        prevPane.gameObject.SetActive(true);
        prevPane.transform.localPosition = Vector3.zero;
        iTween.MoveTo(prevPane.gameObject, iTween.Hash((object) "position", (object) this.m_paneSwapOutOffset, (object) "islocal", (object) true, (object) "time", (object) this.m_paneSwapAnimationTime, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) (Action<object>) (o =>
        {
          if ((UnityEngine.Object) prevPane != (UnityEngine.Object) null && (UnityEngine.Object) prevPane.gameObject != (UnityEngine.Object) null)
            prevPane.gameObject.SetActive(false);
          --swapCount;
        })));
        num = this.m_paneSwapAnimationTime;
      }
      if ((UnityEngine.Object) nextPane != (UnityEngine.Object) null)
      {
        ++swapCount;
        nextPane.gameObject.SetActive(true);
        nextPane.transform.localPosition = this.m_paneSwapInOffset;
        iTween.MoveTo(nextPane.gameObject, iTween.Hash((object) "position", (object) Vector3.zero, (object) "islocal", (object) true, (object) "time", (object) this.m_paneSwapAnimationTime, (object) "delay", (object) num, (object) "oncomplete", (object) (Action<object>) (o => --swapCount)));
      }
      while (swapCount > 0)
        yield return (object) null;
    }
    else
    {
      prevPane.transform.localPosition = this.m_paneSwapOutOffset;
      nextPane.transform.localPosition = Vector3.zero;
      prevPane.gameObject.SetActive(false);
      nextPane.gameObject.SetActive(true);
    }
    --this.m_settingNewModeCount;
  }

  private IEnumerator AnimateAndUpdateStorePane(
    GeneralStoreMode oldMode,
    GeneralStoreMode newMode)
  {
    GeneralStorePane prevPane = this.GetPane(oldMode);
    GeneralStorePane nextPane = this.GetPane(newMode);
    if (oldMode != newMode)
    {
      ++this.m_settingNewModeCount;
      if ((UnityEngine.Object) this.m_paneScrollbar != (UnityEngine.Object) null)
      {
        this.m_paneScrollbar.SaveScroll("STORE_MODE_" + (object) oldMode);
        this.m_paneScrollbar.ScrollObject = (GameObject) null;
      }
      if ((UnityEngine.Object) this.m_paneScrollbar != (UnityEngine.Object) null && (UnityEngine.Object) nextPane != (UnityEngine.Object) null && (UnityEngine.Object) nextPane.m_paneContainer != (UnityEngine.Object) null)
      {
        Vector3 position;
        this.m_paneStartPositions.TryGetValue(newMode, out position);
        this.m_paneScrollbar.ScrollObject = nextPane.m_paneContainer;
        this.m_paneScrollbar.ResetScrollStartPosition(position);
        this.m_paneScrollbar.LoadScroll("STORE_MODE_" + (object) newMode, true);
        this.m_paneScrollbar.EnableIfNeeded();
      }
      if ((UnityEngine.Object) prevPane != (UnityEngine.Object) null)
      {
        prevPane.PrePaneSwappedOut();
        while (!prevPane.AnimateExitStart())
          yield return (object) null;
        while (!prevPane.AnimateExitEnd())
          yield return (object) null;
        prevPane.PostPaneSwappedOut();
      }
      if ((double) this.m_paneSwapAnimationTime > 0.0)
      {
        int swapCount = 0;
        float num = 0.0f;
        if ((UnityEngine.Object) prevPane != (UnityEngine.Object) null)
        {
          ++swapCount;
          prevPane.transform.localPosition = Vector3.zero;
          prevPane.gameObject.SetActive(true);
          iTween.MoveTo(prevPane.gameObject, iTween.Hash((object) "position", (object) this.m_paneSwapOutOffset, (object) "islocal", (object) true, (object) "time", (object) this.m_paneSwapAnimationTime, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) (Action<object>) (o =>
          {
            if ((UnityEngine.Object) prevPane != (UnityEngine.Object) null && (UnityEngine.Object) prevPane.gameObject != (UnityEngine.Object) null)
              prevPane.gameObject.SetActive(false);
            --swapCount;
          })));
          num = this.m_paneSwapAnimationTime;
        }
        if ((UnityEngine.Object) nextPane != (UnityEngine.Object) null)
        {
          ++swapCount;
          nextPane.transform.localPosition = this.m_paneSwapInOffset;
          nextPane.gameObject.SetActive(true);
          iTween.MoveTo(nextPane.gameObject, iTween.Hash((object) "position", (object) Vector3.zero, (object) "islocal", (object) true, (object) "time", (object) this.m_paneSwapAnimationTime, (object) "delay", (object) num, (object) "oncomplete", (object) (Action<object>) (o => --swapCount)));
        }
        while (swapCount > 0)
          yield return (object) null;
      }
      else
      {
        prevPane.transform.localPosition = this.m_paneSwapOutOffset;
        nextPane.transform.localPosition = Vector3.zero;
        prevPane.gameObject.SetActive(false);
        nextPane.gameObject.SetActive(true);
      }
      if ((UnityEngine.Object) nextPane != (UnityEngine.Object) null)
      {
        nextPane.PrePaneSwappedIn();
        while (!nextPane.AnimateEntranceStart())
          yield return (object) null;
        while (!nextPane.AnimateEntranceEnd())
          yield return (object) null;
        nextPane.PostPaneSwappedIn();
      }
      --this.m_settingNewModeCount;
    }
  }

  private void ResetAnimations()
  {
    if (!((UnityEngine.Object) this.m_shakePane != (UnityEngine.Object) null))
      return;
    this.m_shakePane.Reset();
  }

  private void UpdateModeButtons(GeneralStoreMode mode)
  {
    foreach (GeneralStore.ModeObjects modeObject in this.m_modeObjects)
    {
      if (!((UnityEngine.Object) modeObject.m_button == (UnityEngine.Object) null))
      {
        UIBHighlight component = modeObject.m_button.GetComponent<UIBHighlight>();
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
        {
          if (mode == modeObject.m_mode)
            component.SelectNoSound();
          else
            component.Reset();
        }
      }
    }
  }

  private void GetContentPositionIndex(
    bool clockwise,
    out Vector3 contentPosition,
    out Vector3 contentRotation,
    out Vector3 lastPanelRotation,
    out Vector3 newPanelRotation)
  {
    lastPanelRotation = GeneralStore.s_MainPanelTriangularRotations[this.m_currentContentPositionIdx];
    if (clockwise)
    {
      this.m_currentContentPositionIdx = (this.m_currentContentPositionIdx + 1) % GeneralStore.s_ContentTriangularPositions.Length;
    }
    else
    {
      --this.m_currentContentPositionIdx;
      if (this.m_currentContentPositionIdx < 0)
        this.m_currentContentPositionIdx = GeneralStore.s_ContentTriangularPositions.Length - 1;
    }
    contentPosition = GeneralStore.s_ContentTriangularPositions[this.m_currentContentPositionIdx];
    contentRotation = GeneralStore.s_ContentTriangularRotations[this.m_currentContentPositionIdx];
    newPanelRotation = GeneralStore.s_MainPanelTriangularRotations[this.m_currentContentPositionIdx];
  }

  private void SuccessfulPurchaseAckEvent(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (this.IsShown() && SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE)
      this.Close();
    else
      this.RefreshContent();
  }

  private void UpdateCostAndButtonState(
    NoGTAPPTransactionData goldBundle,
    Network.Bundle moneyBundle)
  {
    if ((Record) moneyBundle != (Record) null && !StoreManager.Get().IsProductAlreadyOwned(moneyBundle))
    {
      this.UpdateCostDisplay(moneyBundle);
      this.UpdateMoneyButtonState();
    }
    else if (goldBundle != null)
    {
      this.UpdateCostDisplay(goldBundle);
      this.UpdateGoldButtonState();
    }
    else
    {
      GeneralStoreContent currentContent = this.GetCurrentContent();
      if ((UnityEngine.Object) currentContent == (UnityEngine.Object) null || currentContent.IsPurchaseDisabled())
      {
        this.UpdateCostDisplay(GeneralStore.BuyPanelState.DISABLED);
      }
      else
      {
        this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_MONEY, currentContent.GetMoneyDisplayOwnedText());
        this.UpdateMoneyButtonState();
      }
    }
  }

  private void FireModeChangedEvent(GeneralStoreMode oldMode, GeneralStoreMode newMode)
  {
    foreach (GeneralStore.ModeChanged modeChanged in this.m_modeChangedListeners.ToArray())
      modeChanged(oldMode, newMode);
  }

  private bool OnNavigateBack()
  {
    this.CloseImpl(true);
    return true;
  }

  private void CloseImpl(bool closeWithAnimation)
  {
    if (this.m_settingNewModeCount > 0)
      return;
    PresenceMgr.Get().SetPrevStatus();
    this.Hide(closeWithAnimation);
    SoundManager.Get().LoadAndPlay((AssetReference) "Store_window_shrink.prefab:b68247126e211224e8a904142d2a9895", this.gameObject);
    this.EnableFullScreenEffects(false);
    this.FireExitEvent(false);
  }

  protected override string GetOwnedTooltipString()
  {
    switch (this.m_currentMode)
    {
      case GeneralStoreMode.CARDS:
        return GameStrings.Get("GLUE_STORE_PACK_BUTTON_TEXT_PURCHASED");
      case GeneralStoreMode.ADVENTURE:
        return GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_TEXT_PURCHASED");
      case GeneralStoreMode.HEROES:
        return GameStrings.Get("GLUE_STORE_HERO_BUTTON_TEXT_PURCHASED");
      default:
        return string.Empty;
    }
  }

  public delegate void ModeChanged(GeneralStoreMode oldMode, GeneralStoreMode newMode);

  [Serializable]
  public class ModeObjects
  {
    public GeneralStoreMode m_mode;
    public GeneralStoreContent m_content;
    public GeneralStorePane m_pane;
    public UIBButton m_button;
  }

  private enum BuyPanelState
  {
    DISABLED,
    BUY_GOLD,
    BUY_MONEY,
  }
}
