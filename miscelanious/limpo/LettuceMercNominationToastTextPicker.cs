using UnityEngine;

public class LettuceMercNominationToastTextPicker : MonoBehaviour
{
  public LettuceMercNominationToastTextPicker.TextState GetToastTextState()
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return LettuceMercNominationToastTextPicker.TextState.Nominate;
    GameEntity gameEntity = gameState.GetGameEntity();
    if (gameEntity == null || gameEntity.GetTag(GAME_TAG.TURN) <= 1)
      return LettuceMercNominationToastTextPicker.TextState.Nominate;
    Player localSidePlayer = gameState.GetLocalSidePlayer();
    if (localSidePlayer == null)
      return LettuceMercNominationToastTextPicker.TextState.Nominate;
    return localSidePlayer.HasTag(GAME_TAG.LETTUCE_MERCENARIES_TO_NOMINATE) ? LettuceMercNominationToastTextPicker.TextState.Replace : LettuceMercNominationToastTextPicker.TextState.Reorder;
  }

  public enum TextState
  {
    Nominate,
    Replace,
    Reorder,
  }
}
