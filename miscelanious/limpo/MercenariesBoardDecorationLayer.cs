using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesBoardDecorationLayer : MonoBehaviour
{
  public List<MercenariesBoardDecorationLayer.DecorationObject> m_decorationObjects;
  public List<MercenariesBoardDecorationLayer.WeightedCompatibleDecorationLayer> m_compatibleDecorationLayers;

  public void HideAllDecorations()
  {
    foreach (MercenariesBoardDecorationLayer.DecorationObject decorationObject in this.m_decorationObjects)
      decorationObject.m_gameObject.SetActive(false);
  }

  public void SetDecorationVisible(
    MercenariesBoardDecorationLayer.DecorationPosition decorationPosition,
    bool allowStackingCompatibleDecorations = true)
  {
    foreach (MercenariesBoardDecorationLayer.DecorationObject decorationObject in this.m_decorationObjects)
    {
      if (decorationObject.m_decorationPosition == decorationPosition)
      {
        decorationObject.m_gameObject.SetActive(true);
        if (allowStackingCompatibleDecorations && this.m_compatibleDecorationLayers != null && this.m_compatibleDecorationLayers.Count > 0)
        {
          MercenariesBoardDecorationLayer.WeightedCompatibleDecorationLayer compatibleDecorationLayer = GeneralUtils.RollElementFromWeightedList<MercenariesBoardDecorationLayer.WeightedCompatibleDecorationLayer>(this.m_compatibleDecorationLayers, (GeneralUtils.WeightAccessorDelegate<MercenariesBoardDecorationLayer.WeightedCompatibleDecorationLayer>) (e => e.m_weight));
          if (compatibleDecorationLayer != null && (UnityEngine.Object) compatibleDecorationLayer.m_compatibleDecoration != (UnityEngine.Object) null)
            compatibleDecorationLayer.m_compatibleDecoration.SetDecorationVisible(decorationPosition, false);
        }
      }
    }
  }

  public void HideTopDecorations()
  {
    foreach (MercenariesBoardDecorationLayer.DecorationObject decorationObject in this.m_decorationObjects)
    {
      if (decorationObject.m_decorationPosition == MercenariesBoardDecorationLayer.DecorationPosition.TOP_LEFT || decorationObject.m_decorationPosition == MercenariesBoardDecorationLayer.DecorationPosition.TOP_RIGHT || decorationObject.m_decorationPosition == MercenariesBoardDecorationLayer.DecorationPosition.TOP_CENTER)
        decorationObject.m_gameObject.SetActive(false);
    }
  }

  [Serializable]
  public enum DecorationPosition
  {
    INVALID,
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT,
    TOP_CENTER,
    BOTTOM_CENTER,
  }

  [Serializable]
  public class WeightedCompatibleDecorationLayer
  {
    public MercenariesBoardDecorationLayer m_compatibleDecoration;
    public int m_weight;
  }

  [Serializable]
  public class DecorationObject
  {
    public MercenariesBoardDecorationLayer.DecorationPosition m_decorationPosition;
    public GameObject m_gameObject;
  }
}
