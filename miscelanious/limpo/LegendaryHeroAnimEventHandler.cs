using UnityEngine;

public class LegendaryHeroAnimEventHandler : MonoBehaviour
{
  private Card m_heroCard;

  public void SetActor(GameObject actorObject)
  {
    Card card = (Card) null;
    if ((Object) actorObject != (Object) null)
    {
      Actor component = actorObject.GetComponent<Actor>();
      if ((Object) component != (Object) null)
        card = component.GetCard();
    }
    this.m_heroCard = card;
  }

  public void RaiseEvent(string eventName)
  {
    if (!((Object) this.m_heroCard != (Object) null))
      return;
    this.m_heroCard.ActivateLegendaryHeroAnimEvent(eventName);
  }
}
