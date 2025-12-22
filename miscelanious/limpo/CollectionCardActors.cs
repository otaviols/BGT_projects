using System.Collections.Generic;
using System.Linq;

public class CollectionCardActors
{
  protected List<Actor> m_cardActors = new List<Actor>();

  public CollectionCardActors()
  {
  }

  public CollectionCardActors(Actor actor) => this.AddCardActor(actor);

  public void AddCardActor(Actor actor) => this.m_cardActors.Add(actor);

  public Actor GetPreferredActor() => this.GetActor(CollectionManager.Get().GetPreferredPremium());

  public Actor GetActor(TAG_PREMIUM premium)
  {
    for (int index = 0; index < this.m_cardActors.Count; ++index)
    {
      if (this.m_cardActors[index].GetPremium() == premium)
        return this.m_cardActors[index];
    }
    return this.m_cardActors.Last<Actor>();
  }

  public void Destroy()
  {
    for (int index = 0; index < this.m_cardActors.Count; ++index)
      this.m_cardActors[index].Destroy();
  }
}
