using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (VisualController))]
public class BaconEmoteCollectionLayout : MonoBehaviour
{
  private VisualController m_mainController;
  [SerializeField]
  private float m_animationTime = 0.15f;
  private bool m_animating;
  [SerializeField]
  private Hearthstone.UI.Widget[] m_emoteWidgets;
  private List<BaconCollectionEmoteLayoutWidgetBehaviour> m_emoteBehaviors = new List<BaconCollectionEmoteLayoutWidgetBehaviour>();
  [SerializeField]
  private Hearthstone.UI.Widget m_draggableWidget;
  [SerializeField]
  private AsyncReference m_draggableReference;
  [SerializeField]
  private int m_dragSortOffset;
  private BaconCollectionEmoteLayoutWidgetBehaviour m_draggableBehavior;
  private BattlegroundsEmoteDataModel m_draggedDatamodel;
  private int m_draggedIndex = -1;
  private bool m_draggingEmote;
  private bool m_allowDrag = true;
  [SerializeField]
  private float m_returnTime;
  [SerializeField]
  private iTween.EaseType m_returnEase;
  private Vector3 m_offScreenPosition;
  private Camera m_fxCamera;
  private const int INVALID_EMOTE_INDEX = -1;
  private List<int> m_updatedEmoteIndices = new List<int>();
  private BaconEmoteTray m_emoteTray;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public void Show(BattlegroundsEmoteLoadoutDataModel dataModel, BaconEmoteTray tray)
  {
    if (this.m_animating)
      return;
    if ((UnityEngine.Object) this.m_mainController == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "BaconEmoteCollectionLayout was shown without a m_mainController defined.");
    }
    else
    {
      this.m_emoteTray = tray;
      this.m_mainController.Owner.BindDataModel((IDataModel) dataModel, false);
      this.m_mainController.Owner.TriggerEvent("UPDATE", new Hearthstone.UI.Widget.TriggerEventParameters());
      CollectiblePageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager();
      if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
      {
        pageManager.EnablePageTurn(false);
        pageManager.EnablePageTurnArrows(false);
      }
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = this.m_animationTime
      });
      this.gameObject.SetActive(true);
      this.m_animating = true;
      iTween.ScaleFrom(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) (Action<object>) (e => this.m_animating = false)));
      this.m_fxCamera = CameraUtils.FindFullScreenEffectsCamera(false);
      this.m_updatedEmoteIndices.Clear();
      for (int index = 0; index < this.m_emoteWidgets.Length; ++index)
        this.m_updatedEmoteIndices.Add(index);
    }
  }

  public void Hide()
  {
    if (this.m_animating)
      return;
    if ((UnityEngine.Object) this.m_mainController == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "BaconEmoteCollectionLayout was hidden without a m_mainController defined.");
    }
    else
    {
      CollectiblePageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager();
      if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
      {
        pageManager.EnablePageTurn(true);
        pageManager.EnablePageTurnArrows(true);
      }
      this.m_screenEffectsHandle.StopEffect();
      Vector3 origScale = this.transform.localScale;
      this.m_animating = true;
      iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_animationTime, (object) "easeType", (object) iTween.EaseType.easeOutCirc, (object) "oncomplete", (object) (Action<object>) (e =>
      {
        this.m_animating = false;
        if (!((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null))
          return;
        this.transform.localScale = origScale;
        this.gameObject.SetActive(false);
        this.m_draggableWidget.Hide();
      })));
      BattlegroundsEmoteLoadoutDataModel loadoutDataModel = this.GetLoadoutDataModel();
      if (loadoutDataModel == null)
      {
        Debug.LogWarning((object) "Tried to save new emote layout without a bound datamodel.");
      }
      else
      {
        BattlegroundsEmoteLoadout loadout = BattlegroundsEmoteLoadout.MakeFromDatamodel(loadoutDataModel);
        Network.Get().SetBattlegroundsEmoteLoadout(loadout);
        ((BaconCollectionDisplay) CollectionManager.Get().GetCollectibleDisplay()).SetEmoteLoadout(loadoutDataModel);
        this.m_emoteTray.ShuffleEmotePositions(this.m_updatedEmoteIndices);
      }
    }
  }

  private void Start()
  {
    this.m_mainController = this.gameObject.GetComponent<VisualController>();
    this.m_mainController.GetComponent<Hearthstone.UI.Widget>().RegisterEventListener(new Hearthstone.UI.Widget.EventListenerDelegate(this.LayoutEventListener));
    this.gameObject.SetActive(false);
    this.m_offScreenPosition = this.m_draggableWidget.transform.position;
    this.m_draggableBehavior = this.m_draggableWidget.GetComponent<BaconCollectionEmoteLayoutWidgetBehaviour>();
    this.m_draggableReference.RegisterReadyListener<Transform>((Action<Transform>) (unused => this.m_draggableWidget.GetComponentInChildren<SortingGroup>().sortingOrder += this.m_dragSortOffset));
    foreach (Component emoteWidget in this.m_emoteWidgets)
      this.m_emoteBehaviors.Add(emoteWidget.GetComponent<BaconCollectionEmoteLayoutWidgetBehaviour>());
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Update()
  {
    if (!this.m_draggingEmote)
      return;
    RaycastHit hitInfo;
    if (!CameraUtils.Raycast(this.m_fxCamera, InputCollection.GetMousePosition(), (LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo) || !InputCollection.GetMouseButton(0) || !InputUtil.IsMouseOnScreen())
      this.OnEmoteDrop();
    else
      this.m_draggableWidget.gameObject.transform.position = hitInfo.point;
  }

  public void Unload() => this.m_mainController.GetComponent<Hearthstone.UI.Widget>().RemoveEventListener(new Hearthstone.UI.Widget.EventListenerDelegate(this.LayoutEventListener));

  public void SwapEmotes(int slot1, int slot2)
  {
    BattlegroundsEmoteLoadoutDataModel loadoutDataModel = this.GetLoadoutDataModel();
    if (loadoutDataModel == null || loadoutDataModel.EmoteList == null || loadoutDataModel.EmoteList.Count != 6)
    {
      Debug.LogWarning((object) "Unable to retrieve datamodel with a valid emote loadout.");
    }
    else
    {
      BattlegroundsEmoteDataModel emote = loadoutDataModel.EmoteList[slot1];
      loadoutDataModel.EmoteList[slot1] = loadoutDataModel.EmoteList[slot2];
      loadoutDataModel.EmoteList[slot2] = emote;
      int updatedEmoteIndex = this.m_updatedEmoteIndices[slot1];
      this.m_updatedEmoteIndices[slot1] = this.m_updatedEmoteIndices[slot2];
      this.m_updatedEmoteIndices[slot2] = updatedEmoteIndex;
      this.m_mainController.Owner.BindDataModel((IDataModel) loadoutDataModel, false);
      this.m_mainController.Owner.TriggerEvent("UPDATE", new Hearthstone.UI.Widget.TriggerEventParameters());
    }
  }

  private int GetHoveredEmote()
  {
    for (int index = 0; index < this.m_emoteBehaviors.Count; ++index)
    {
      if (UniversalInputManager.Get().ForcedUnblockableInputIsOver(CameraUtils.FindFullScreenEffectsCamera(false), this.m_emoteBehaviors[index].GetDragCollider().gameObject, out RaycastHit _))
        return index;
    }
    return -1;
  }

  private void LayoutEventListener(string eventName)
  {
    if (!(eventName == "OffDialogClick_code"))
    {
      if (!(eventName == "EMOTE_drag_started"))
      {
        if (!(eventName == "EMOTE_drag_released"))
          return;
        this.OnEmoteDrop();
      }
      else
        this.OnEmoteDragStart();
    }
    else
      this.Hide();
  }

  private void OnEmoteDragStart()
  {
    EventDataModel dataModel = this.m_mainController.Owner.GetDataModel<EventDataModel>();
    if (dataModel == null)
      Log.All.PrintError("Tried to drag without event datamodel");
    else if (!(dataModel.Payload is BattlegroundsEmoteDataModel payload))
    {
      Log.All.PrintError("Recieved event without emote datamodel.");
    }
    else
    {
      if (payload.EmoteDbiId == 0 || !this.m_allowDrag)
        return;
      this.m_allowDrag = false;
      this.m_draggedDatamodel = payload;
      this.m_draggedIndex = -1;
      int num = 0;
      foreach (BattlegroundsEmoteDataModel emote in this.GetLoadoutDataModel().EmoteList)
      {
        if (emote.EmoteDbiId == this.m_draggedDatamodel.EmoteDbiId)
        {
          this.m_draggedIndex = num;
          break;
        }
        ++num;
      }
      this.m_draggableWidget.Hide();
      this.BindAndConfigureDraggableWidget(payload);
      this.m_draggableWidget.RegisterDoneChangingStatesListener((Action<object>) (_ => this.PickUpEmote()), (object) null, true, true);
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_pick_up_card.prefab:f7fb595cdc26f2f4997b4a10eaf1d0e1", this.m_draggableWidget.gameObject);
    }
  }

  private void PickUpEmote()
  {
    PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
    this.m_draggingEmote = true;
    this.m_emoteWidgets[this.m_draggedIndex].Hide();
    this.m_emoteWidgets[this.m_draggedIndex].GetComponentInChildren<PegUIElement>().SetEnabled(true);
    this.m_draggableWidget.Show();
    this.m_draggableWidget.TriggerEvent("PICKUP_EFFECTS");
  }

  private void OnEmoteDrop()
  {
    if (!this.m_draggingEmote)
      return;
    this.m_draggingEmote = false;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    bool showDroppedEmoteOnTop = false;
    int droppedSortOffset = this.m_dragSortOffset * 2;
    Vector3 vector3 = new Vector3();
    int hoveredEmoteIndex = this.GetHoveredEmote();
    Vector3 position;
    if (hoveredEmoteIndex != -1 && hoveredEmoteIndex != this.m_draggedIndex)
    {
      this.BindAndConfigureDraggableWidget(this.GetLoadoutDataModel().EmoteList[hoveredEmoteIndex]);
      this.m_emoteWidgets[hoveredEmoteIndex].TriggerEvent("SHOW");
      showDroppedEmoteOnTop = true;
      this.m_emoteBehaviors[hoveredEmoteIndex].IncreaseSpriteSortOrder(droppedSortOffset);
      this.SwapEmotes(hoveredEmoteIndex, this.m_draggedIndex);
      position = this.m_emoteWidgets[hoveredEmoteIndex].transform.position;
      this.m_emoteWidgets[hoveredEmoteIndex].TriggerEvent("DROP_EFFECTS");
    }
    else
      position = this.m_draggableWidget.gameObject.transform.position;
    int returnIndex = this.m_draggedIndex;
    this.m_draggableWidget.transform.position = position;
    iTween.MoveTo(this.m_draggableWidget.gameObject, iTween.Hash((object) "position", (object) this.m_emoteWidgets[returnIndex].transform.position, (object) "time", (object) this.m_returnTime, (object) "easeType", (object) this.m_returnEase, (object) "oncomplete", (object) (Action<object>) (e =>
    {
      if (showDroppedEmoteOnTop)
        this.m_emoteBehaviors[hoveredEmoteIndex].IncreaseSpriteSortOrder(-droppedSortOffset);
      this.m_draggableWidget.gameObject.transform.position = this.m_offScreenPosition;
      this.m_emoteWidgets[returnIndex].GetComponent<Hearthstone.UI.Widget>().Show();
      this.m_allowDrag = true;
    })));
    this.m_draggedIndex = -1;
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_drop_card.prefab:8275e45efb8280347b35c2548e706d84", this.m_draggableWidget.gameObject);
  }

  private void BindAndConfigureDraggableWidget(BattlegroundsEmoteDataModel emoteDataModel)
  {
    this.m_draggableWidget.BindDataModel((IDataModel) emoteDataModel);
    if (this.m_emoteBehaviors[this.m_draggedIndex].m_flipBubble)
      this.m_draggableWidget.TriggerEvent("POINT_RIGHT");
    else
      this.m_draggableWidget.TriggerEvent("POINT_LEFT");
  }

  private BattlegroundsEmoteLoadoutDataModel GetLoadoutDataModel() => this.m_mainController.Owner.GetDataModel<BattlegroundsEmoteLoadoutDataModel>();
}
