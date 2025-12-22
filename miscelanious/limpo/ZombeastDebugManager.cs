using Hearthstone;
using UnityEngine;

public class ZombeastDebugManager : MonoBehaviour
{
  private static ZombeastDebugManager s_instance;

  public static ZombeastDebugManager Get()
  {
    if ((Object) ZombeastDebugManager.s_instance == (Object) null)
    {
      GameObject gameObject = new GameObject();
      ZombeastDebugManager.s_instance = gameObject.AddComponent<ZombeastDebugManager>();
      gameObject.name = "ZombeastDebugManager (Dynamically created)";
    }
    return ZombeastDebugManager.s_instance;
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
    int tag = friendlySidePlayer.GetTag(GAME_TAG.ZOMBEAST_DEBUG_CURRENT_BEAST_DATABASE_ID);
    if (tag == 0)
      return;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
    string str = "Unknown";
    if (entityDef != null)
      str = entityDef.GetName();
    string text = string.Format("Zombeast being generated: {0}\nGenerated: {1}/{2}", (object) str, (object) friendlySidePlayer.GetTag(GAME_TAG.ZOMBEAST_DEBUG_CURRENT_ITERATION), (object) friendlySidePlayer.GetTag(GAME_TAG.ZOMBEAST_DEBUG_MAX_ITERATIONS));
    Vector3 position = new Vector3((float) Screen.width, (float) Screen.height, 0.0f);
    DebugTextManager.Get().DrawDebugText(text, position, 0.0f, true);
  }
}
