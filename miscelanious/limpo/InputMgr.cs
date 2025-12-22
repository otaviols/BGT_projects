using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : MonoBehaviour
{
  [SerializeField]
  protected CollectionDraggableCardVisual m_heldCardVisual;
  [SerializeField]
  protected AsyncReference m_mercenariesDraggablesReference;
  [SerializeField]
  protected Collider TooltipPlane;
  public AsyncReference m_battlegroundsDraggablesReference;
  public AsyncReference m_battlegroundsDragEmoteSpriteReference;
  public AsyncReference m_battlegroundsEmoteTrayReference;
  public static readonly PlatformDependentValue<float> PHONE_HEIGHT_OFFSET = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    Phone = 10f
  };
  protected static List<InputMgr> s_instances = new List<InputMgr>();
  protected bool m_heldCardOffscreen;
  protected Widget m_mercenariesDraggablesWidget;
  protected Widget m_battlegroundsDraggablesWidget;
  protected SpriteRenderer m_battlegroundsDragEmoteSprite;
  protected BaconEmoteTray m_battlegroundsEmoteTray;
  protected CollectionUtils.MercenariesModeCardType m_heldType;
  protected CollectionUtils.BattlegroundsModeDraggableType m_bgHeldType;
  protected string m_heldMercenariesModeCardId;
  protected string m_heldBattlegroundsEmoteCardId;
  protected Vector3 m_offScreenPosition;
  private bool m_wasMouseOverDeck;
  public InputMgr.OnCardDroppedCallback m_cardDroppedCallback;

  public event Action<CollectionUtils.MercenariesModeCardType, string> OnDropMercenariesModeCard;

  protected virtual bool MouseIsOverDeck { get; set; }

  protected virtual void Awake()
  {
    InputMgr.s_instances.Add(this);
    UniversalInputManager.Get().RegisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
    this.m_mercenariesDraggablesReference?.RegisterReadyListener<Widget>(new Action<Widget>(this.OnMercenariesDraggablesReady));
    this.m_battlegroundsDraggablesReference?.RegisterReadyListener<Widget>(new Action<Widget>(this.OnBattlegroundsDraggablesReady));
    this.m_battlegroundsDragEmoteSpriteReference?.RegisterReadyListener<SpriteRenderer>(new Action<SpriteRenderer>(this.OnBattlegroundsDragEmoteSpriteReady));
    this.m_battlegroundsEmoteTrayReference?.RegisterReadyListener<BaconEmoteTray>(new Action<BaconEmoteTray>(this.OnBattlegroundsEmoteTrayReady));
  }

  protected virtual void OnDestroy() => InputMgr.s_instances.Remove(this);

  protected void Update() => this.UpdateHeldCard();

  public static InputMgr Get()
  {
    int count = InputMgr.s_instances.Count;
    return count == 0 ? (InputMgr) null : InputMgr.s_instances[count - 1];
  }

  public void Unload() => UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));

  public virtual bool HandleKeyboardInput() => false;

  public virtual bool GrabMercenariesModeCard(
    IDataModel dataModel,
    CollectionUtils.MercenariesModeCardType cardType,
    InputMgr.OnCardDroppedCallback callback = null)
  {
    RaycastHit hitInfo;
    if (dataModel == null || !this.CanGrabMercenariesModeItem(cardType) || (UnityEngine.Object) this.m_mercenariesDraggablesWidget == (UnityEngine.Object) null || !UniversalInputManager.Get().GetInputHitInfo(Box.Get().GetCamera(), (LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
      return false;
    this.m_cardDroppedCallback = callback;
    this.m_mercenariesDraggablesWidget.BindDataModel(dataModel);
    PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
    string eventName1 = (string) null;
    string eventName2 = (string) null;
    switch (cardType)
    {
      case CollectionUtils.MercenariesModeCardType.Mercenary:
        eventName1 = "START_MERC_OVER_COLLECTION_code";
        eventName2 = "HOLD_MERC_OVER_TEAM_TRAY_code";
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        eventName1 = "HOLD_ABILITY_OVER_COLLECTION_code";
        eventName2 = "HOLD_ABILITY_OVER_TEAM_TRAY_code";
        break;
    }
    this.SetHeldMercenaryCard(dataModel, cardType);
    this.m_mercenariesDraggablesWidget.TriggerEvent(eventName1);
    this.DisableDraggableColliders();
    if (this.MouseIsOverDeck)
      this.m_mercenariesDraggablesWidget.TriggerEvent(eventName2);
    else
      this.m_mercenariesDraggablesWidget.TriggerEvent(eventName1);
    this.m_offScreenPosition = this.m_mercenariesDraggablesWidget.gameObject.transform.position;
    this.m_mercenariesDraggablesWidget.gameObject.transform.position = hitInfo.point;
    return true;
  }

  public virtual void SetHeldMercenaryCard(
    IDataModel dataModel,
    CollectionUtils.MercenariesModeCardType cardType)
  {
    this.m_heldType = cardType;
    switch (cardType)
    {
      case CollectionUtils.MercenariesModeCardType.Mercenary:
        if (!(dataModel is LettuceMercenaryDataModel mercenaryDataModel))
        {
          Log.Lettuce.PrintWarning("CollectionInputMgr.SetHeldMercenaryCard - mercenary data model is not valid!");
          break;
        }
        this.m_heldMercenariesModeCardId = CollectionManager.Get().GetMercenary((long) mercenaryDataModel.MercenaryId).GetCardId();
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        if (!(dataModel is LettuceAbilityDataModel abilityDataModel))
        {
          Log.Lettuce.PrintWarning("CollectionInputMgr.SetHeldMercenaryCard - ability data model is not valid!");
          break;
        }
        LettuceAbilityTierDataModel abilityTier = abilityDataModel.AbilityTiers[abilityDataModel.CurrentTier - 1];
        if (abilityTier == null)
        {
          Log.Lettuce.PrintWarning("CollectionInputMgr.SetHeldMercenaryCard - ability tier data model is not valid!");
          break;
        }
        this.m_heldMercenariesModeCardId = abilityTier.AbilityTierCard.CardId;
        break;
    }
  }

  public CollectionDraggableCardVisual GetHeldCardVisual() => this.m_heldCardVisual;

  public bool BattlegroundsIsDragging() => this.m_bgHeldType != 0;

  private void UpdateHeldCard()
  {
    if (this.m_heldType != CollectionUtils.MercenariesModeCardType.None)
      this.UpdateHeldMercenariesModeCard();
    else if (this.BattlegroundsIsDragging())
    {
      this.UpdateBattlegroundsModeEmote();
    }
    else
    {
      if (!((UnityEngine.Object) this.m_heldCardVisual != (UnityEngine.Object) null) || !this.m_heldCardVisual.IsShown())
        return;
      this.UpdateHeldCardVisual();
    }
  }

  protected virtual bool CanGrabMercenariesModeItem(CollectionUtils.MercenariesModeCardType itemType) => this.m_heldType == CollectionUtils.MercenariesModeCardType.None;

  protected virtual void UpdateHeldCardVisual()
  {
    RaycastHit hitInfo;
    if (!UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
      return;
    if ((UnityEngine.Object) this.m_heldCardVisual != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
    {
      foreach (Component componentsInChild in this.m_heldCardVisual.GetComponentsInChildren<Transform>())
        componentsInChild.gameObject.layer = 19;
    }
    Vector3 point = hitInfo.point;
    if ((bool) UniversalInputManager.UsePhoneUI)
      point.y += (float) InputMgr.PHONE_HEIGHT_OFFSET;
    this.m_heldCardVisual.transform.position = point;
  }

  private void UpdateHeldMercenariesModeCard()
  {
    RaycastHit hitInfo;
    if (!UniversalInputManager.Get().GetInputHitInfo(Box.Get().GetCamera(), (LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
      return;
    Vector3 point = hitInfo.point;
    if ((bool) UniversalInputManager.UsePhoneUI)
      point.y += (float) InputMgr.PHONE_HEIGHT_OFFSET;
    this.m_mercenariesDraggablesWidget.gameObject.transform.position = point;
    this.UpdateMercenariesHeldVisual(this.m_heldType);
    if (!InputCollection.GetMouseButtonUp(0))
      return;
    this.DropMercenariesModeCard(false);
  }

  private void UpdateBattlegroundsModeEmote()
  {
    if (!InputUtil.IsMouseOnScreen())
    {
      this.DropBattlegroundsEmote(false, true);
      this.m_battlegroundsEmoteTray.UpdateTrayHighlight(false);
    }
    else
    {
      RaycastHit hitInfo;
      if (!UniversalInputManager.Get().GetInputHitInfo(Box.Get().GetCamera(), (LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo) || !InputCollection.GetMouseButton(0))
      {
        this.DropBattlegroundsEmote(false);
        this.m_battlegroundsEmoteTray.UpdateTrayHighlight(false);
      }
      else
      {
        this.m_battlegroundsEmoteTray.UpdateTrayHighlight(UniversalInputManager.Get().ForcedUnblockableInputIsOver(Box.Get().GetCamera(), this.m_battlegroundsEmoteTray.gameObject, out RaycastHit _));
        Vector3 point = hitInfo.point;
        if ((bool) UniversalInputManager.UsePhoneUI)
          point.y += (float) InputMgr.PHONE_HEIGHT_OFFSET;
        this.m_battlegroundsDraggablesWidget.gameObject.transform.position = point;
      }
    }
  }

  protected virtual void UpdateMercenariesHeldVisual(
    CollectionUtils.MercenariesModeCardType heldType)
  {
    string eventName = "";
    bool mouseIsOverDeck = this.MouseIsOverDeck;
    switch (heldType)
    {
      case CollectionUtils.MercenariesModeCardType.Mercenary:
        if (mouseIsOverDeck && !this.m_wasMouseOverDeck)
        {
          eventName = "HOLD_MERC_OVER_TEAM_TRAY_code";
          break;
        }
        if (!mouseIsOverDeck && this.m_wasMouseOverDeck)
        {
          eventName = "HOLD_MERC_OVER_COLLECTION_code";
          break;
        }
        break;
      case CollectionUtils.MercenariesModeCardType.Equipment:
        if (mouseIsOverDeck && !this.m_wasMouseOverDeck)
        {
          eventName = "HOLD_ABILITY_OVER_TEAM_TRAY_code";
          break;
        }
        if (!mouseIsOverDeck && this.m_wasMouseOverDeck)
        {
          eventName = "HOLD_ABILITY_OVER_COLLECTION_code";
          break;
        }
        break;
    }
    this.m_wasMouseOverDeck = mouseIsOverDeck;
    if (string.IsNullOrEmpty(eventName))
      return;
    this.m_mercenariesDraggablesWidget.TriggerEvent(eventName);
  }

  protected virtual void DropCard(bool dragCanceled)
  {
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    if ((UnityEngine.Object) this.m_heldCardVisual == (UnityEngine.Object) null)
      return;
    if (!dragCanceled)
    {
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_drop_card.prefab:8275e45efb8280347b35c2548e706d84", this.m_heldCardVisual.gameObject);
      if (this.m_cardDroppedCallback != null)
      {
        this.m_cardDroppedCallback();
        this.m_cardDroppedCallback = (InputMgr.OnCardDroppedCallback) null;
      }
    }
    this.m_heldCardVisual.Hide();
  }

  public virtual void DropMercenariesModeCard(bool dragCanceled)
  {
    if (this.m_heldType == CollectionUtils.MercenariesModeCardType.None)
      return;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    if ((UnityEngine.Object) this.m_mercenariesDraggablesWidget == (UnityEngine.Object) null)
      return;
    if (!dragCanceled)
    {
      Action<CollectionUtils.MercenariesModeCardType, string> mercenariesModeCard = this.OnDropMercenariesModeCard;
      if (mercenariesModeCard != null)
        mercenariesModeCard(this.m_heldType, this.m_heldMercenariesModeCardId);
      if (this.m_cardDroppedCallback != null)
      {
        this.m_cardDroppedCallback();
        this.m_cardDroppedCallback = (InputMgr.OnCardDroppedCallback) null;
      }
    }
    this.m_mercenariesDraggablesWidget.gameObject.transform.position = this.m_offScreenPosition;
    this.m_heldMercenariesModeCardId = string.Empty;
    this.m_heldType = CollectionUtils.MercenariesModeCardType.None;
  }

  public virtual void DropBattlegroundsEmote(bool dragCanceled, bool trayDropCanceled = false)
  {
  }

  protected virtual void OnMouseOnOrOffScreen(bool onScreen)
  {
    if ((UnityEngine.Object) this.m_heldCardVisual == (UnityEngine.Object) null || (UnityEngine.Object) this.m_heldCardVisual.gameObject == (UnityEngine.Object) null)
      return;
    if (onScreen)
    {
      if (!this.m_heldCardOffscreen)
        return;
      this.m_heldCardOffscreen = false;
      if (InputCollection.GetMouseButton(0))
        this.m_heldCardVisual.Show(this.MouseIsOverDeck);
      else
        this.DropCard(true);
    }
    else
    {
      if (!this.m_heldCardVisual.IsShown())
        return;
      this.m_heldCardVisual.Hide();
      this.m_heldCardOffscreen = true;
    }
  }

  protected void DisableDraggableColliders()
  {
    BoxCollider[] componentsInChildren = this.m_mercenariesDraggablesWidget.gameObject.GetComponentsInChildren<BoxCollider>(true);
    if (componentsInChildren == null)
      return;
    foreach (Collider collider in componentsInChildren)
      collider.enabled = false;
  }

  protected void DisableBattlegroundsDraggableColliders()
  {
    BoxCollider[] componentsInChildren = this.m_battlegroundsDraggablesWidget.gameObject.GetComponentsInChildren<BoxCollider>(true);
    if (componentsInChildren == null)
      return;
    foreach (Collider collider in componentsInChildren)
      collider.enabled = false;
  }

  private void OnMercenariesDraggablesReady(Widget widget)
  {
    if (!((UnityEngine.Object) widget != (UnityEngine.Object) null))
      return;
    this.m_mercenariesDraggablesWidget = widget;
    this.DisableDraggableColliders();
  }

  private void OnBattlegroundsDraggablesReady(Widget widget)
  {
    if (!((UnityEngine.Object) widget != (UnityEngine.Object) null))
      return;
    this.m_battlegroundsDraggablesWidget = widget;
    this.DisableBattlegroundsDraggableColliders();
  }

  private void OnBattlegroundsDragEmoteSpriteReady(SpriteRenderer spriteRenderer)
  {
    if (!((UnityEngine.Object) spriteRenderer != (UnityEngine.Object) null))
      return;
    this.m_battlegroundsDragEmoteSprite = spriteRenderer;
  }

  private void OnBattlegroundsEmoteTrayReady(BaconEmoteTray baconEmoteTray)
  {
    SceneMgr sceneMgr = SceneMgr.Get();
    SceneMgr.Mode mode = sceneMgr == null ? SceneMgr.Mode.INVALID : sceneMgr.GetMode();
    if ((UnityEngine.Object) baconEmoteTray == (UnityEngine.Object) null && (mode == SceneMgr.Mode.BACON || mode == SceneMgr.Mode.BACON_COLLECTION))
      Log.CollectionManager.PrintError("BaconEmoteTray not found on Battlegrounds emote tray widget reference");
    else
      this.m_battlegroundsEmoteTray = baconEmoteTray;
  }

  public delegate void OnCardDroppedCallback();
}
