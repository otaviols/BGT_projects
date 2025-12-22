using Hearthstone;
using UnityEngine;

public class DrustvarHorrorDebugManager : MonoBehaviour
{
  private static DrustvarHorrorDebugManager s_instance;

  public static DrustvarHorrorDebugManager Get()
  {
    if ((Object) DrustvarHorrorDebugManager.s_instance == (Object) null)
    {
      GameObject gameObject = new GameObject();
      DrustvarHorrorDebugManager.s_instance = gameObject.AddComponent<DrustvarHorrorDebugManager>();
      gameObject.name = "DrustvarHorrorDebugManager (Dynamically created)";
    }
    return DrustvarHorrorDebugManager.s_instance;
  }

  private void Update()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    Player friendlySidePlayer = gameState.GetFriendlySidePlayer();
    if (friendlySidePlayer == null)
      return;
    int tag = friendlySidePlayer.GetTag(GAME_TAG.DRUSTVAR_HORROR_DEBUG_CURRENT_SPELL_DATABASE_ID);
    if (tag == 0)
      return;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
    string str = "Unknown";
    if (entityDef != null)
      str = entityDef.GetName();
    string text = string.Format("Horror being generated: {0}\nGenerated: {1}/{2}", (object) str, (object) friendlySidePlayer.GetTag(GAME_TAG.DRUSTVAR_HORROR_DEBUG_CURRENT_ITERATION), (object) friendlySidePlayer.GetTag(GAME_TAG.DRUSTVAR_HORROR_DEBUG_MAX_ITERATIONS));
    Vector3 position = new Vector3((float) Screen.width, (float) Screen.height, 0.0f);
    DebugTextManager.Get().DrawDebugText(text, position, 0.0f, true);
  }
}
