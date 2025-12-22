using Assets;
using Game.Shop;
using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyFrame : MonoBehaviour
{
  public GameObject m_dustFX;
  public GameObject m_explodeFX_Common;
  public GameObject m_explodeFX_Rare;
  public GameObject m_explodeFX_Epic;
  public GameObject m_explodeFX_Legendary;
  [SerializeField]
  protected GameObject m_currencyIconContainer;
  [SerializeField]
  protected Clickable m_clickable;
  [SerializeField]
  protected Vector3 m_helperTipPopupOffsetPC;
  [SerializeField]
  protected Vector3 m_helperTipPopupOffsetMobile;
  private Widget m_widget;
  private CurrencyFrame.State m_state = CurrencyFrame.State.SHOWN;
  private bool m_isBlocked;
  private const float CURRENCY_FRAME_OFFSET_LOCAL_Y = -63f;
  private const float CURRENCY_FRAME_OFFSET_WORLD_Z = 7f;
  private Notification m_rechargeHelpPopup;

  public CurrencyType CurrentCurrencyType { get; private set; }

  public GameObject CurrencyIconContainer => this.m_currencyIconContainer;

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnWidgetEvent));
    if (!((Object) this.m_clickable != (Object) null))
      return;
    this.m_clickable.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnFrameMouseOver));
    this.m_clickable.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnFrameMouseOut));
  }

  private void Start()
  {
    BnetBar bnetBar = BnetBar.Get();
    if ((Object) bnetBar != (Object) null)
      bnetBar.RegisterCurrencyFrame(this);
    this.Bind(CurrencyType.NONE);
  }

  public void Bind(CurrencyType currencyType)
  {
    this.CurrentCurrencyType = currencyType;
    if ((Object) global::Shop.Get() == (Object) null)
      return;
    Widget.TriggerEventParameters triggerEventParameters = new Widget.TriggerEventParameters();
    triggerEventParameters.NoDownwardPropagation = true;
    Widget.TriggerEventParameters parameters1 = triggerEventParameters;
    if ((Object) this.m_rechargeHelpPopup != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_rechargeHelpPopup);
    switch (currencyType)
    {
      case CurrencyType.GOLD:
        this.m_widget.TriggerEvent("GOLD", parameters1);
        break;
      case CurrencyType.DUST:
        this.m_widget.TriggerEvent("DUST", parameters1);
        break;
      case CurrencyType.CN_RUNESTONES:
      case CurrencyType.ROW_RUNESTONES:
        this.m_widget.TriggerEvent("VIRTUAL_CURRENCY", parameters1);
        break;
      case CurrencyType.CN_ARCANE_ORBS:
        this.m_widget.TriggerEvent("BOOSTER_CURRENCY", parameters1);
        break;
      case CurrencyType.RENOWN:
        this.m_widget.TriggerEvent("RENOWN", parameters1);
        if (!LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_RENOWN_POPUP))
        {
          Vector3 position = this.transform.position;
          Vector3 vector3;
          Notification.PopUpArrowDirection direction;
          float num;
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            vector3 = position + this.m_helperTipPopupOffsetMobile;
            direction = Notification.PopUpArrowDirection.Up;
            num = 0.9f;
          }
          else
          {
            vector3 = position + this.m_helperTipPopupOffsetPC;
            direction = Notification.PopUpArrowDirection.Down;
            num = 1f;
          }
          this.m_rechargeHelpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, Vector3.zero, TutorialEntity.GetTextScale() * num, GameStrings.Get("GLUE_LETTUCE_RENOWN_CONVERSION_HELPER_POPUP"));
          this.m_rechargeHelpPopup.gameObject.transform.position = vector3;
          this.m_rechargeHelpPopup.ShowPopUpArrow(direction);
          this.m_rechargeHelpPopup.PulseReminderEveryXSeconds(3f);
          break;
        }
        break;
      default:
        this.m_widget.TriggerEvent("NONE", parameters1);
        this.Hide(true);
        break;
    }
    SceneMgr sceneMgr = SceneMgr.Get();
    bool flag = (sceneMgr == null ? 0 : (int) sceneMgr.GetMode()) != 3;
    Widget widget = this.m_widget;
    string eventName = flag ? "FADE_BACKGROUND" : "SOLID_BACKGROUND";
    triggerEventParameters = new Widget.TriggerEventParameters();
    Widget.TriggerEventParameters parameters2 = triggerEventParameters;
    widget.TriggerEvent(eventName, parameters2);
  }

  public void Show(bool isImmediate = false)
  {
    if (this.m_isBlocked || this.CurrentCurrencyType == CurrencyType.NONE || this.m_state == CurrencyFrame.State.SHOWN || this.m_state == CurrencyFrame.State.ANIMATE_IN)
      return;
    if (DemoMgr.Get() != null && !DemoMgr.Get().IsCurrencyEnabled())
    {
      this.Hide(true);
    }
    else
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        isImmediate = true;
      this.gameObject.SetActive(true);
      this.m_state = CurrencyFrame.State.ANIMATE_IN;
      Hashtable args = iTween.Hash((object) "amount", (object) 1f, (object) "delay", (object) 0.0f, (object) "time", (object) (float) (isImmediate ? 0.0 : 0.25), (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "FinaliseShow", (object) "oncompletetarget", (object) this.gameObject);
      iTween.Stop(this.gameObject);
      iTween.FadeTo(this.gameObject, args);
    }
  }

  public void Hide(bool isImmediate = false)
  {
    if (this.m_state == CurrencyFrame.State.HIDDEN || this.m_state == CurrencyFrame.State.ANIMATE_OUT)
      return;
    this.m_state = CurrencyFrame.State.ANIMATE_OUT;
    Hashtable args = iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) 0.0f, (object) "time", (object) (float) (isImmediate ? 0.0 : 0.25), (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "FinaliseHide", (object) "oncompletetarget", (object) this.gameObject);
    iTween.Stop(this.gameObject);
    iTween.FadeTo(this.gameObject, args);
  }

  public void SetBlocked(bool isBlocked)
  {
    this.m_isBlocked = isBlocked;
    if (isBlocked)
      return;
    this.Bind(this.CurrentCurrencyType);
  }

  public GameObject GetTooltipObject()
  {
    TooltipZone component = this.GetComponent<TooltipZone>();
    return (Object) component != (Object) null ? component.GetTooltipObject() : (GameObject) null;
  }

  public bool IsShown() => this.m_state == CurrencyFrame.State.ANIMATE_IN || this.m_state == CurrencyFrame.State.SHOWN;

  private void FinaliseShow()
  {
    iTween.Stop(this.gameObject, true);
    this.m_state = CurrencyFrame.State.SHOWN;
  }

  private void FinaliseHide()
  {
    iTween.Stop(this.gameObject, true);
    this.gameObject.SetActive(false);
    this.m_state = CurrencyFrame.State.HIDDEN;
  }

  private void OnFrameMouseOver(UIEvent e)
  {
    if (this.m_isBlocked)
      return;
    string key1 = "";
    string key2 = "";
    switch (this.CurrentCurrencyType)
    {
      case CurrencyType.GOLD:
        key1 = "GLUE_TOOLTIP_GOLD_HEADER";
        key2 = "GLUE_TOOLTIP_GOLD_DESCRIPTION";
        break;
      case CurrencyType.DUST:
        key1 = "GLUE_CRAFTING_ARCANEDUST";
        key2 = "GLUE_CRAFTING_ARCANEDUST_DESCRIPTION";
        break;
      case CurrencyType.CN_RUNESTONES:
        key1 = "GLUE_TOOLTIP_VIRTUAL_CURRENCY_HEADER";
        key2 = "GLUE_TOOLTIP_VIRTUAL_CURRENCY_DESCRIPTION";
        break;
      case CurrencyType.CN_ARCANE_ORBS:
        key1 = "GLUE_TOOLTIP_BOOSTER_CURRENCY_HEADER";
        key2 = "GLUE_TOOLTIP_BOOSTER_CURRENCY_DESCRIPTION";
        break;
      case CurrencyType.ROW_RUNESTONES:
        key1 = "GLUE_TOOLTIP_VIRTUAL_CURRENCY_HEADER";
        key2 = "GLUE_TOOLTIP_VIRTUAL_CURRENCY_ROW_DESCRIPTION";
        break;
      case CurrencyType.RENOWN:
        key1 = "GLUE_TOOLTIP_RENOWN_HEADER";
        key2 = "GLUE_TOOLTIP_RENOWN_DESCRIPTION";
        break;
    }
    if (key1 == "")
      return;
    TooltipPanel src = this.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get(key1), GameStrings.Get(key2), 0.7f);
    LayerUtils.SetLayer(src.gameObject, GameLayer.BattleNet);
    src.transform.localEulerAngles = new Vector3(270f, 0.0f, 0.0f);
    src.transform.localScale = new Vector3(70f, 70f, 70f);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      TransformUtil.SetPoint((Component) src, Anchor.TOP, (Component) this.m_clickable, Anchor.BOTTOM, Vector3.zero);
      TransformUtil.SetLocalPosY((Component) src, -63f);
    }
    else
      TransformUtil.SetPoint((Component) src, Anchor.BOTTOM, (Component) this.m_clickable, Anchor.TOP, new Vector3(0.0f, 0.0f, 7f));
  }

  private void OnFrameMouseOut(UIEvent e) => this.GetComponent<TooltipZone>().HideTooltip();

  private void OnWidgetEvent(string eventName)
  {
    if (!(eventName == "RECHARGE"))
      return;
    this.OnAttemptRecharge();
  }

  private void OnAttemptRecharge()
  {
    if (this.m_isBlocked)
      return;
    if (ShopUtils.IsCurrencyVirtual(this.CurrentCurrencyType))
    {
      global::Shop shop = global::Shop.Get();
      if ((Object) shop == (Object) null || !shop.CanSafelyOpenCurrencyPage() || !ShopUtils.IsVirtualCurrencyEnabled())
        return;
      if (ShopUtils.IsMainVirtualCurrencyType(this.CurrentCurrencyType))
      {
        ProductDataModel specialOfferVariant;
        if (VariantUtils.TryFindSpecialOfferVariant(shop.ShopData?.VirtualCurrency, out specialOfferVariant))
          shop.OpenVirtualCurrencyPurchase(specialOfferVariant);
        else
          shop.OpenVirtualCurrencyPurchase();
      }
      else
      {
        if (!ShopUtils.IsBoosterVirtualCurrencyType(this.CurrentCurrencyType))
          return;
        shop.OpenBoosterCurrencyPurchase();
      }
    }
    else
    {
      if (this.CurrentCurrencyType != CurrencyType.RENOWN || PresenceMgr.Get().CurrentStatus == Global.PresenceStatus.MERCENARIES_VILLAGE_RENOWN_CONVERSION)
        return;
      LettuceVillagePopupManager villagePopupManager = LettuceVillagePopupManager.Get();
      if (!((Object) villagePopupManager != (Object) null))
        return;
      villagePopupManager.Show(LettuceVillagePopupManager.PopupType.RENOWNCONVERSION);
    }
  }

  public static IEnumerable<CurrencyType> GetVisibleCurrencies()
  {
    IStore currentStore = StoreManager.Get().GetCurrentStore();
    if (currentStore != null && currentStore.IsOpen())
      return currentStore.GetVisibleCurrencies();
    List<CurrencyType> visibleCurrencies = new List<CurrencyType>();
    SceneMgr sceneMgr = SceneMgr.Get();
    switch (sceneMgr == null ? 0 : (int) sceneMgr.GetMode())
    {
      case 2:
      case 9:
      case 12:
        return (IEnumerable<CurrencyType>) visibleCurrencies;
      case 3:
        visibleCurrencies.Add(CurrencyType.GOLD);
        break;
      case 5:
        visibleCurrencies.Add(CurrencyType.DUST);
        break;
      case 6:
      case 7:
      case 8:
      case 10:
      case 13:
      case 16:
        if (!(bool) UniversalInputManager.UsePhoneUI)
        {
          visibleCurrencies.Add(CurrencyType.GOLD);
          break;
        }
        break;
      case 14:
      case 15:
        if (!(bool) UniversalInputManager.UsePhoneUI)
        {
          TavernBrawlDisplay tavernBrawlDisplay = TavernBrawlDisplay.Get();
          if ((Object) tavernBrawlDisplay != (Object) null && tavernBrawlDisplay.IsInDeckEditMode())
          {
            visibleCurrencies.Add(CurrencyType.DUST);
            break;
          }
          visibleCurrencies.Add(CurrencyType.GOLD);
          break;
        }
        break;
      case 18:
        if (!(bool) UniversalInputManager.UsePhoneUI || (Object) PvPDungeonRunScene.Get() != (Object) null && (Object) PvPDungeonRunScene.Get().GetPopupManager() != (Object) null && PvPDungeonRunScene.Get().GetPopupManager().ShouldShowCoinCounter())
        {
          visibleCurrencies.Add(CurrencyType.GOLD);
          CurrencyType currencyType;
          if (ShopUtils.IsVirtualCurrencyEnabled() && ShopUtils.TryGetMainVirtualCurrencyType(out currencyType))
          {
            visibleCurrencies.Add(currencyType);
            break;
          }
          break;
        }
        break;
      case 20:
      case 24:
        if (StoreManager.Get().CurrentShopType == ShopType.MERCENARIES_WORKSHOP)
        {
          visibleCurrencies.Add(CurrencyType.GOLD);
          break;
        }
        if ((PresenceMgr.Get().CurrentStatus == Global.PresenceStatus.MERCENARIES_VILLAGE_TASKBOARD || PresenceMgr.Get().CurrentStatus == Global.PresenceStatus.MERCENARIES_VILLAGE_RENOWN_CONVERSION) && LettuceRenownUtil.HasUnlockedRenownOffers())
        {
          visibleCurrencies.Add(CurrencyType.RENOWN);
          break;
        }
        break;
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
      visibleCurrencies.Remove(CurrencyType.DUST);
    return (IEnumerable<CurrencyType>) visibleCurrencies;
  }

  public enum State
  {
    ANIMATE_IN,
    ANIMATE_OUT,
    HIDDEN,
    SHOWN,
  }
}
