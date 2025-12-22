using UnityEngine;

public class DiamondViewerClearCardsButton : MonoBehaviour
{
  public void OnButtonPress()
  {
    foreach (Actor actor in Object.FindObjectsOfType<Actor>())
    {
      Debug.Log((object) ("Deleting : " + actor.name));
      actor.Destroy();
    }
  }
}
