using UnityEngine;

public class BaconDeckCapToggle : MonoBehaviour
{
  public GameObject[] deckCapObjects;

  private void Awake()
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    if (!gameState.IsGameCreated())
      gameState.RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnGameCreated));
    else
      this.UpdateVisibility();
  }

  private void OnGameCreated(GameState.CreateGamePhase phase, object userData) => this.UpdateVisibility();

  private void UpdateVisibility()
  {
    bool flag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.DARKMOON_FAIRE_PRIZES_ACTIVE) == 1;
    foreach (GameObject deckCapObject in this.deckCapObjects)
      deckCapObject.SetActive(!flag);
  }

  private void OnDestroy() => GameState.Get()?.UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnGameCreated));
}
