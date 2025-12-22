using Hearthstone;
using UnityEngine;

public class CardBackDeckDisplay : MonoBehaviour
{
  public bool m_FriendlyDeck = true;
  private CardBackManager m_CardBackManager;

  private void Start()
  {
    this.m_CardBackManager = CardBackManager.Get();
    if (this.m_CardBackManager == null)
    {
      if ((Object) HearthstoneApplication.Get() != (Object) null)
        Debug.LogError((object) "Failed to get CardBackManager!");
      this.enabled = false;
    }
    else
      this.m_CardBackManager.RegisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateDeckCardBacks));
    this.UpdateDeckCardBacks();
  }

  private void OnDestroy()
  {
    if (CardBackManager.Get() == null)
      return;
    CardBackManager.Get().UnregisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateDeckCardBacks));
  }

  public void UpdateDeckCardBacks()
  {
    if (this.m_CardBackManager == null)
      return;
    this.m_CardBackManager.UpdateDeck(this.gameObject, this.m_FriendlyDeck ? CardBackManager.CardBackSlot.FRIENDLY : CardBackManager.CardBackSlot.OPPONENT);
  }
}
