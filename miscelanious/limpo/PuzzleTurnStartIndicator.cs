using UnityEngine;

public class PuzzleTurnStartIndicator : TurnStartIndicator
{
  public UberText m_ProgressText;
  public UberText m_PuzzleNameText;

  public override void Show()
  {
    if ((Object) this.m_ProgressText == (Object) null)
      Log.Gameplay.PrintError("PuzzleTurnStartIndicator.Show(): m_ProgressText on {0} is null, please assign an UberText!", (object) this);
    else if ((Object) this.m_ProgressText == (Object) null)
    {
      Log.Gameplay.PrintError("PuzzleTurnStartIndicator.Show(): m_PuzzleNameText on {0} is null, please assign an UberText!", (object) this);
    }
    else
    {
      ZoneSecret secretZone = GameState.Get().GetFriendlySidePlayer().GetSecretZone();
      GameEntity gameEntity = GameState.Get().GetGameEntity();
      Entity puzzleEntity = secretZone.GetPuzzleEntity();
      if (puzzleEntity != null)
      {
        this.m_ProgressText.Text = string.Format(GameStrings.Get("BOTA_PUZZLE_PROGRESS"), (object) puzzleEntity.GetTag(GAME_TAG.PUZZLE_PROGRESS), (object) puzzleEntity.GetTag(GAME_TAG.PUZZLE_PROGRESS_TOTAL));
        int tag = gameEntity.GetTag(GAME_TAG.PUZZLE_NAME);
        EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
        if (entityDef != null)
        {
          this.m_PuzzleNameText.Text = entityDef.GetName();
        }
        else
        {
          Log.Gameplay.PrintError("PuzzleTurnStartIndicator.Show(): could not find name for card ID {0}, puzzle {1}/{2}.", (object) tag, (object) puzzleEntity.GetTag(GAME_TAG.PUZZLE_PROGRESS), (object) puzzleEntity.GetTag(GAME_TAG.PUZZLE_PROGRESS_TOTAL));
          this.m_PuzzleNameText.Text = "";
        }
      }
      base.Show();
    }
  }
}
