using Hearthstone.UI;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncBehaviorUtils
{
  public static List<IAsyncInitializationBehavior> GetAsyncBehaviors(
    Component component)
  {
    Component[] components = component.GetComponents<Component>();
    List<IAsyncInitializationBehavior> asyncBehaviors = new List<IAsyncInitializationBehavior>();
    foreach (Component component1 in components)
    {
      if (component1 is IAsyncInitializationBehavior initializationBehavior)
        asyncBehaviors.Add(initializationBehavior);
    }
    return asyncBehaviors;
  }
}
