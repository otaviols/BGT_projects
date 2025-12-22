using UnityEngine;

public class FinisherAuthoringHeroInitializer : MonoBehaviour
{
  [SerializeField]
  private Actor m_Actor;

  private void Start()
  {
    Card card = this.gameObject.AddComponent<Card>();
    if ((Object) null != (Object) card)
    {
      card.SetEntity((Entity) new FinisherAuthoringDummyEntity());
      card.SetActor(this.m_Actor);
    }
    CustomHeroFrameBehaviour componentInChildren = this.GetComponentInChildren<CustomHeroFrameBehaviour>();
    if (!((Object) componentInChildren != (Object) null))
      return;
    componentInChildren.UpdateFrame();
  }
}
