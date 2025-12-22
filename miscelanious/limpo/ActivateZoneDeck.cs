using UnityEngine;

public class ActivateZoneDeck : MonoBehaviour
{
  public bool m_friendlyDeck;
  private bool onoff = true;

  public void ToggleActive()
  {
    if (GameState.Get() == null || GameState.Get().GetFriendlySidePlayer() == null || GameState.Get().GetOpposingSidePlayer() == null)
    {
      Debug.LogError((object) "ActivateZoneDeck - Game State not yet initialized.");
    }
    else
    {
      ZoneDeck zoneDeck = !this.m_friendlyDeck ? GameState.Get().GetOpposingSidePlayer().GetDeckZone() : GameState.Get().GetFriendlySidePlayer().GetDeckZone();
      if ((Object) zoneDeck == (Object) null)
        Debug.LogError((object) "ActivateZoneDeck - zoneDeck is null!");
      else
        zoneDeck.SetVisibility(this.onoff);
    }
  }
}
