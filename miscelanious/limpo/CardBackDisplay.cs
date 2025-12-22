using System.Collections;
using UnityEngine;

public class CardBackDisplay : MonoBehaviour
{
  public Actor m_Actor;
  public GameObject m_Shadow;
  public CardBackManager.CardBackSlot m_CardBackSlot = CardBackManager.CardBackSlot.FAVORITE;
  private CardBackManager m_CardBackManager;

  private void Start()
  {
    this.m_CardBackManager = CardBackManager.Get();
    if (this.m_CardBackManager == null)
    {
      Debug.LogError((object) "Failed to get CardBackManager!");
      this.enabled = false;
    }
    else
      this.m_CardBackManager.RegisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateCardBack));
    this.UpdateCardBack();
  }

  private void OnDestroy()
  {
    if (CardBackManager.Get() == null)
      return;
    CardBackManager.Get().UnregisterUpdateCardbacksListener(new CardBackManager.UpdateCardbacksCallback(this.UpdateCardBack));
  }

  public void UpdateCardBack()
  {
    if (this.m_CardBackManager == null || !this.gameObject.activeInHierarchy)
      return;
    this.StartCoroutine(this.SetCardBackDisplay());
  }

  public void SetCardBack(CardBackManager.CardBackSlot slot)
  {
    if (this.m_CardBackManager == null)
      this.m_CardBackManager = CardBackManager.Get();
    this.m_CardBackManager.UpdateCardBack(this.gameObject, slot);
  }

  public void EnableShadow(bool enabled)
  {
    if (!((Object) this.m_Shadow != (Object) null))
      return;
    this.m_Shadow.SetActive(enabled);
  }

  private IEnumerator SetCardBackDisplay()
  {
    CardBackDisplay cardBackDisplay = this;
    if ((Object) cardBackDisplay.m_Actor == (Object) null)
      cardBackDisplay.m_CardBackManager.UpdateCardBack(cardBackDisplay.gameObject, cardBackDisplay.m_CardBackSlot);
    else if (!cardBackDisplay.m_Actor.GetCardbackUpdateIgnore())
    {
      if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
      {
        while (cardBackDisplay.m_Actor.GetEntity() == null)
          yield return (object) null;
      }
      cardBackDisplay.m_CardBackManager.UpdateCardBack(cardBackDisplay.gameObject, cardBackDisplay.m_Actor.GetCardBackSlot());
      cardBackDisplay.m_Actor.SeedMaterialEffects();
    }
  }
}
