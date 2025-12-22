using UnityEngine;

public static class LegendaryHeroCardExtensions
{
  public static void ActivateLegendaryHeroAnimEvent(this Card heroCard, string eventName)
  {
    if (!((Object) heroCard != (Object) null))
      return;
    Actor actor = heroCard.GetActor();
    if (!((Object) actor != (Object) null))
      return;
    actor.LegendaryHeroPortrait?.RaiseAnimationEvent(eventName);
  }
}
