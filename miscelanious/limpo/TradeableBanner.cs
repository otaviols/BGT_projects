using UnityEngine;

public class TradeableBanner : MonoBehaviour
{
  public MeshRenderer m_tradeableHighlight_Green;
  public MeshRenderer m_tradeableHighlight_Blue;

  public void SetHighlightState(TradeableHighlightState state)
  {
    this.m_tradeableHighlight_Green.enabled = state == TradeableHighlightState.Green;
    this.m_tradeableHighlight_Blue.enabled = state == TradeableHighlightState.Blue;
  }
}
