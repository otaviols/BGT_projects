using Blizzard.T5.Core.Utils;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LettuceMapCoin : MonoBehaviour
{
  public List<LettuceMapLine> m_ConnectionLines;
  public TooltipZone m_tooltipZone;
  public float m_tooltipScale = 5f;
  public AsyncReference m_RootVisualControllerReference;
  public AsyncReference m_CheckMarkContainerReference;
  private LettuceMap m_lettuceMap;
  private VisualController m_checkmarkVisualController;
  private int m_nextConnectionLineIndex;

  public int NodeId { get; private set; }

  private void Start()
  {
    LettuceMapCoinDataModel mapCoinDataModel = this.GetMapCoinDataModel();
    this.m_lettuceMap = GameObjectUtils.FindComponentInParents<LettuceMap>(this.gameObject);
    if ((UnityEngine.Object) this.m_lettuceMap != (UnityEngine.Object) null)
      this.m_lettuceMap.RegisterCoin(this, mapCoinDataModel);
    this.m_CheckMarkContainerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnCheckmarkReadyReady));
    this.m_RootVisualControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnRootReady));
    this.m_ConnectionLines = this.m_ConnectionLines.OrderBy<LettuceMapLine, Guid>((Func<LettuceMapLine, Guid>) (a => Guid.NewGuid())).ToList<LettuceMapLine>();
    if (mapCoinDataModel == null)
      return;
    this.NodeId = mapCoinDataModel.Id;
  }

  public void OnCheckmarkReadyReady(VisualController visualController) => this.m_checkmarkVisualController = visualController;

  public void OnRootReady(VisualController visualController) => visualController.Owner.RegisterEventListener(new Widget.EventListenerDelegate(this.OnMapCoinEvent));

  public void OnMapCoinEvent(string eventName)
  {
    if (!(eventName == "ON_MOUSE_OVER"))
    {
      if (!(eventName == "ON_MOUSE_OUT"))
        return;
      this.HideTooltip();
    }
    else
      this.ShowTooltip();
  }

  public void DrawLineToObjectOnNextRow(
    GameObject destination,
    int currentConnectionIndex,
    int numConnectionsComingFromLeft,
    int numConnectionsComingFromRight)
  {
    if (this.m_nextConnectionLineIndex >= this.m_ConnectionLines.Count)
    {
      Debug.LogError((object) "LettuceMapCoin.DrawLineToObjectOnNextRow() - Not enough lines! Tried to draw too many!");
    }
    else
    {
      LettuceMapLine connectionLine = this.m_ConnectionLines[this.m_nextConnectionLineIndex];
      ++this.m_nextConnectionLineIndex;
      connectionLine.gameObject.SetActive(true);
      connectionLine.m_StartBone = this.transform;
      connectionLine.m_EndBone = destination.transform;
      connectionLine.m_ConnectionType = LettuceMapLine.ConnectionType.NEXT_ROW;
      connectionLine.m_ConnectionIndex = currentConnectionIndex;
      connectionLine.m_NumParentConnectionsComingFromLeft = numConnectionsComingFromLeft;
      connectionLine.m_NumParentConnectionsComingFromRight = numConnectionsComingFromRight;
      connectionLine.RefreshLine();
    }
  }

  public void DrawLineToObjectOnSameRow(GameObject destination)
  {
    if (this.m_nextConnectionLineIndex >= this.m_ConnectionLines.Count)
    {
      Debug.LogError((object) "LettuceMapCoin.DrawLineToObjectOnSameRow() - Not enough lines! Tried to draw too many!");
    }
    else
    {
      LettuceMapLine connectionLine = this.m_ConnectionLines[this.m_nextConnectionLineIndex];
      ++this.m_nextConnectionLineIndex;
      connectionLine.gameObject.SetActive(true);
      connectionLine.m_StartBone = this.transform;
      connectionLine.m_EndBone = destination.transform;
      connectionLine.m_ConnectionType = LettuceMapLine.ConnectionType.SAME_ROW;
      connectionLine.RefreshLine();
    }
  }

  public void FlashCheckMark()
  {
    if (!((UnityEngine.Object) this.m_checkmarkVisualController != (UnityEngine.Object) null))
      return;
    this.m_checkmarkVisualController.SetState("FLASH_CHECK_MARK");
  }

  public LettuceMapCoinDataModel GetMapCoinDataModel()
  {
    Widget component = this.GetComponent<Widget>();
    if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
      return component.GetDataModel<LettuceMapCoinDataModel>();
    Debug.LogError((object) "GetMapCoinDataModel() - Coin had no widget!");
    return (LettuceMapCoinDataModel) null;
  }

  private void ShowTooltip()
  {
    LettuceMapCoinDataModel mapCoinDataModel = this.GetMapCoinDataModel();
    if (mapCoinDataModel.GrantedAnomalyCard != null)
      return;
    this.m_tooltipZone.ShowTooltip(mapCoinDataModel.HoverTooltipHeader, mapCoinDataModel.HoverTooltipBody, this.m_tooltipScale);
  }

  private void HideTooltip() => this.m_tooltipZone.HideTooltip();
}
