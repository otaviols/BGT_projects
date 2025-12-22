using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (VisualController))]
public class BaconEmoteTray : MonoBehaviour
{
  [SerializeField]
  private Widget[] m_emoteWidgets;
  [Tooltip("Pointers for each tray slot's nested BattlegroundsImageWidget")]
  [SerializeField]
  private AsyncReference[] m_asyncImageWidgetReferences;
  private readonly Widget[] m_nestedImageWidgets = new Widget[6];
  private List<Vector3> m_emotePositions;
  [SerializeField]
  private iTween.EaseType m_shuffleEase;
  [SerializeField]
  private float m_shuffleTime;
  protected VisualController m_vc;
  protected Widget m_widget;
  private BattlegroundsEmoteLoadoutDataModel m_loadoutToSave;
  private const int INVALID_EMOTE_INDEX = -1;
  private const int LOADOUT_SIZE = 6;
  private int m_draggedIndex = -1;
  private int m_hoveredEmoteIndex = -1;
  private int m_readyImageWidgets;
  private bool m_trayHovered;

  public void Start()
  {
    this.m_vc = this.gameObject.GetComponent<VisualController>();
    if ((UnityEngine.Object) this.m_vc == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("BaconEmoteTray was initialized without a visual controller defined.");
    }
    else
    {
      this.m_widget = (Widget) this.m_vc.Owner;
      if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      {
        Log.CollectionManager.PrintError("BaconEmoteTray was initialized without a widget defined.");
      }
      else
      {
        this.m_widget.BindDataModel((IDataModel) CollectionManager.Get().CreateEmoteLoadoutDataModel());
        this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.EmoteDisplayEventListener));
        this.m_emotePositions = new List<Vector3>();
        foreach (Component emoteWidget in this.m_emoteWidgets)
          this.m_emotePositions.Add(emoteWidget.transform.localPosition);
        if (this.m_asyncImageWidgetReferences.Length != 6)
        {
          Log.CollectionManager.PrintError(string.Format("BaconEmoteTray was initialized with incorrect number of async image widget references. Expected {0}, found {1}", (object) 6, (object) this.m_asyncImageWidgetReferences.Length));
        }
        else
        {
          for (int index = 0; index < this.m_asyncImageWidgetReferences.Length; ++index)
          {
            int imageIndex = index;
            this.m_asyncImageWidgetReferences[index].RegisterReadyListener<Widget>((Action<Widget>) (widget =>
            {
              this.m_nestedImageWidgets[imageIndex] = widget;
              ++this.m_readyImageWidgets;
            }));
          }
        }
      }
    }
  }

  public void Show(BattlegroundsEmoteLoadoutDataModel dataModel)
  {
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("BaconEmoteTray was shown without a widget defined.");
    }
    else
    {
      this.SetLoadoutDataModel(dataModel);
      this.StartCoroutine(this.ShowWhenReady(dataModel));
    }
  }

  private IEnumerator ShowWhenReady(BattlegroundsEmoteLoadoutDataModel dataModel)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BaconEmoteTray baconEmoteTray = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      baconEmoteTray.UpdateImageWidgetVisibility(dataModel);
      baconEmoteTray.m_widget.TriggerEvent("SHOW");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u003E2__current = (object) new WaitUntil(new Func<bool>(baconEmoteTray.\u003CShowWhenReady\u003Eb__17_0));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void UpdateImageWidgetVisibility(BattlegroundsEmoteLoadoutDataModel dataModel)
  {
    for (int index = 0; index < dataModel.EmoteList.Count; ++index)
    {
      if (dataModel.EmoteList[index].EmoteDbiId == 0)
        this.m_nestedImageWidgets[index].Hide();
      else
        this.m_nestedImageWidgets[index].Show();
    }
  }

  public void Hide()
  {
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("BaconEmoteTray was hidden without a widget defined.");
    }
    else
    {
      if (this.GetLoadoutDataModel() != null)
      {
        Network.Get().SetBattlegroundsEmoteLoadout(BattlegroundsEmoteLoadout.MakeFromDatamodel(this.GetLoadoutDataModel()));
        this.m_widget.UnbindDataModel(645);
      }
      this.m_widget.TriggerEvent("HIDE");
    }
  }

  public void OnDestroy()
  {
    if (this.m_loadoutToSave == null)
      return;
    Network network = Network.Get();
    if (network != null)
      network.SetBattlegroundsEmoteLoadout(BattlegroundsEmoteLoadout.MakeFromDatamodel(this.m_loadoutToSave));
    else
      Debug.Log((object) "Unable to set new BGS emote loadout on network.");
  }

  public void Unload()
  {
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      Log.CollectionManager.PrintError("BaconEmoteTray was unloaded without a widget defined.");
    else
      this.m_widget.RemoveEventListener(new Widget.EventListenerDelegate(this.EmoteDisplayEventListener));
  }

  public void SetLoadoutDataModel(BattlegroundsEmoteLoadoutDataModel dataModel)
  {
    if (dataModel == null)
    {
      Log.CollectionManager.PrintError("BaconEmoteTray.SetLoadoutDataModel - received null datamodel");
    }
    else
    {
      this.m_widget.BindDataModel((IDataModel) dataModel);
      this.m_loadoutToSave = dataModel;
      this.m_widget.TriggerEvent("UPDATE");
    }
  }

  public void DropOverEmoteTray(BattlegroundsEmoteDataModel dataModel)
  {
    if (this.IsShufflingEmotes())
      return;
    BattlegroundsEmoteLoadoutDataModel loadoutDataModel = this.GetLoadoutDataModel();
    if (loadoutDataModel == null)
      Log.CollectionManager.PrintError("BaconEmoteTray - No bound datamodel for emote operations.");
    else if (loadoutDataModel.EmoteList == null)
      Log.CollectionManager.PrintError("BaconEmoteTray - Bound datamodel doesn't contain a valid emote loadout.");
    else if (dataModel == null)
    {
      Log.CollectionManager.PrintError("BaconEmoteTray - New Emote datamodel is null.");
    }
    else
    {
      int newEmoteIndex = this.m_hoveredEmoteIndex;
      if (newEmoteIndex == -1)
      {
        DataModelList<BattlegroundsEmoteDataModel> emoteList = this.GetLoadoutDataModel().EmoteList;
        for (int index = 0; index < emoteList.Count; ++index)
        {
          if (emoteList[index].EmoteDbiId == 0 || emoteList[index].EmoteDbiId == dataModel.EmoteDbiId)
          {
            newEmoteIndex = index;
            break;
          }
        }
      }
      if (newEmoteIndex != -1)
      {
        IDataModel model;
        if (!this.m_widget.GetDataModel(645, out model))
        {
          Debug.LogWarning((object) "BaconEmoteTray - no valid data model bound to the widget");
          return;
        }
        BattlegroundsEmoteLoadoutDataModel dataModel1 = (BattlegroundsEmoteLoadoutDataModel) model;
        this.m_emoteWidgets[newEmoteIndex].TriggerEvent("DROP_EFFECTS");
        if (this.m_draggedIndex != -1)
        {
          bool isFillingEmptySlot = dataModel1.EmoteList[newEmoteIndex].EmoteDbiId == 0;
          int draggedEmoteIndex = this.m_draggedIndex;
          this.SwapEmoteDatamodels(newEmoteIndex, this.m_draggedIndex, dataModel1);
          this.m_widget.RegisterDoneChangingStatesListener((Action<object>) (_ => this.FinishSwappingEmoteImages(isFillingEmptySlot, draggedEmoteIndex, newEmoteIndex)), (object) null, true, true);
        }
        else
        {
          BaconCollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as BaconCollectionPageManager;
          if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
          {
            if (dataModel1.EmoteList[newEmoteIndex] != null)
              pageManager.SetEmoteEquippedState(BattlegroundsEmoteId.FromTrustedValue(dataModel1.EmoteList[newEmoteIndex].EmoteDbiId), false);
            pageManager.SetEmoteEquippedState(BattlegroundsEmoteId.FromTrustedValue(dataModel.EmoteDbiId), true);
          }
          dataModel1.EmoteList[newEmoteIndex] = dataModel;
          this.SetLoadoutDataModel(dataModel1);
          this.m_emoteWidgets[newEmoteIndex].RegisterDoneChangingStatesListener((Action<object>) (_ => this.m_nestedImageWidgets[newEmoteIndex].Show()), (object) null, true, true);
        }
      }
      this.m_draggedIndex = -1;
    }
  }

  private void FinishSwappingEmoteImages(
    bool isFillingEmptySlot,
    int draggedIndex,
    int hoveredIndex)
  {
    if (!isFillingEmptySlot)
    {
      this.m_nestedImageWidgets[draggedIndex].Show();
      iTween.Stop(this.m_emoteWidgets[draggedIndex].gameObject);
      this.m_emoteWidgets[draggedIndex].transform.localPosition = this.m_emotePositions[hoveredIndex];
      iTween.MoveTo(this.m_emoteWidgets[draggedIndex].gameObject, iTween.Hash((object) "position", (object) this.m_emotePositions[draggedIndex], (object) "time", (object) this.m_shuffleTime, (object) "easeType", (object) this.m_shuffleEase, (object) "islocal", (object) true));
    }
    else
    {
      this.m_nestedImageWidgets[draggedIndex].Hide();
      this.m_nestedImageWidgets[hoveredIndex].Show();
    }
  }

  public bool IsEmoteOverTray() => this.m_trayHovered || this.m_hoveredEmoteIndex != -1;

  public bool IsLoadoutValid() => this.GetLoadoutDataModel() != null;

  public bool IsEmoteInLoadout(int emoteId)
  {
    foreach (BattlegroundsEmoteDataModel emote in this.GetLoadoutDataModel().EmoteList)
    {
      if (emote.EmoteDbiId == emoteId)
        return true;
    }
    return false;
  }

  public void RemoveEmote(BattlegroundsEmoteDataModel dataModel)
  {
    if (this.m_draggedIndex == -1)
    {
      Debug.LogError((object) "Tried to remove emote from loadout without a held emote index saved.");
    }
    else
    {
      this.m_nestedImageWidgets[this.m_draggedIndex].Hide();
      IDataModel model;
      if (!this.m_widget.GetDataModel(645, out model))
      {
        Log.CollectionManager.PrintError("BaconEmoteTray - no valid data model bound to the widget");
      }
      else
      {
        BattlegroundsEmoteLoadoutDataModel loadoutDataModel = (BattlegroundsEmoteLoadoutDataModel) model;
        BaconCollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as BaconCollectionPageManager;
        if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null && loadoutDataModel.EmoteList[this.m_draggedIndex] != null)
          pageManager.SetEmoteEquippedState(BattlegroundsEmoteId.FromTrustedValue(loadoutDataModel.EmoteList[this.m_draggedIndex].EmoteDbiId), false);
        loadoutDataModel.EmoteList[this.m_draggedIndex] = new BattlegroundsEmoteDataModel();
        this.m_widget.BindDataModel((IDataModel) loadoutDataModel);
        this.m_loadoutToSave = loadoutDataModel;
        this.m_draggedIndex = -1;
      }
    }
  }

  public void UpdateTrayHighlight(bool trayHovered)
  {
    if (this.m_trayHovered == trayHovered)
      return;
    this.m_trayHovered = trayHovered;
    string eventName = trayHovered ? "SHOW_TRAY_HIGHLIGHT" : "HIDE_TRAY_HIGHLIGHT";
    foreach (Widget emoteWidget in this.m_emoteWidgets)
      emoteWidget.TriggerEvent(eventName);
  }

  private void EmoteDisplayEventListener(string eventName)
  {
    if (!(eventName == "EMOTE_drag_started"))
    {
      if (!(eventName == "EMOTE_drag_released"))
      {
        if (!(eventName == "EMOTE_mouse_over"))
        {
          if (!(eventName == "EMOTE_mouse_out"))
            return;
          this.OnEmoteMouseOut();
        }
        else
          this.OnEmoteMouseOver();
      }
      else
        CollectionInputMgr.Get().DropBattlegroundsEmote(false, false);
    }
    else
      this.OnEmoteDragStart();
  }

  private void OnEmoteDragStart()
  {
    if (this.IsShufflingEmotes())
      return;
    EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
    if (dataModel == null)
    {
      Log.CollectionManager.PrintError("No event data model attached to BaconEmoteTray");
    }
    else
    {
      BattlegroundsEmoteDataModel payload = (BattlegroundsEmoteDataModel) dataModel.Payload;
      if (payload == null || payload.EmoteDbiId == 0)
        return;
      this.m_draggedIndex = -1;
      DataModelList<BattlegroundsEmoteDataModel> emoteList = this.GetLoadoutDataModel().EmoteList;
      for (int index = 0; index < emoteList.Count; ++index)
      {
        if (emoteList[index].EmoteDbiId == payload.EmoteDbiId)
        {
          this.m_draggedIndex = index;
          break;
        }
      }
      if (this.m_draggedIndex == -1)
        Debug.LogError((object) "Unable to determine which emote was dragged.");
      else
        CollectionInputMgr.Get().GrabBattlegroundsEmote((IDataModel) payload, CollectionUtils.BattlegroundsModeDraggableType.TrayEmote, sourceWidget: this.m_nestedImageWidgets[this.m_draggedIndex]);
    }
  }

  private void OnEmoteMouseOver()
  {
    EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
    if (dataModel == null)
      Log.CollectionManager.PrintError("No event data model attached to BaconEmoteTray");
    else if (dataModel.Payload is IConvertible payload)
    {
      this.m_hoveredEmoteIndex = Convert.ToInt32((object) payload);
    }
    else
    {
      Log.CollectionManager.PrintError("Unrecognized event payload in OnEmoteMouseOver().");
      this.m_hoveredEmoteIndex = -1;
    }
  }

  private void OnEmoteMouseOut()
  {
    EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
    if (dataModel == null)
      Log.CollectionManager.PrintError("No event data model attached to BaconEmoteTray");
    else if (dataModel.Payload is IConvertible payload)
    {
      if (Convert.ToInt32((object) payload) != this.m_hoveredEmoteIndex)
        return;
      this.m_hoveredEmoteIndex = -1;
    }
    else
      Log.CollectionManager.PrintError("Unrecognized event payload in OnEmoteMouseOver().");
  }

  private void SwapEmoteDatamodels(
    int slot1,
    int slot2,
    BattlegroundsEmoteLoadoutDataModel dataModel)
  {
    if (slot1 < 0 || slot1 >= 6 || slot2 < 0 || slot2 >= 6)
      Log.CollectionManager.PrintError("BaconEmoteTray - Attempted to swap emote at invalid index");
    else if (dataModel == null)
    {
      Log.CollectionManager.PrintError("BaconEmoteTray - Attempted to swap emote with null datamodel.");
    }
    else
    {
      BattlegroundsEmoteDataModel emote = dataModel.EmoteList[slot1];
      dataModel.EmoteList[slot1] = dataModel.EmoteList[slot2];
      dataModel.EmoteList[slot2] = emote;
      this.SetLoadoutDataModel(dataModel);
    }
  }

  public BattlegroundsEmoteLoadoutDataModel GetLoadoutDataModel()
  {
    IDataModel model;
    this.m_widget.GetDataModel(645, out model);
    return model as BattlegroundsEmoteLoadoutDataModel;
  }

  public void ShuffleEmotePositions(List<int> newIndices)
  {
    for (int index = 0; index < this.m_emoteWidgets.Length; ++index)
    {
      if (index != newIndices[index])
      {
        iTween.Stop(this.m_emoteWidgets[index].gameObject);
        this.m_emoteWidgets[index].transform.localPosition = this.m_emotePositions[newIndices[index]];
        iTween.MoveTo(this.m_emoteWidgets[index].gameObject, iTween.Hash((object) "position", (object) this.m_emotePositions[index], (object) "time", (object) this.m_shuffleTime, (object) "easeType", (object) this.m_shuffleEase, (object) "islocal", (object) true));
      }
    }
  }

  private bool IsShufflingEmotes()
  {
    foreach (Component emoteWidget in this.m_emoteWidgets)
    {
      if (iTween.Count(emoteWidget.gameObject) > 0)
        return true;
    }
    return false;
  }
}
