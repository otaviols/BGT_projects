using UnityEngine;

public class PuzzleProgressUI : MonoBehaviour
{
  public UberText m_ProgressText;
  public UberText m_PuzzleNameText;
  public UberText m_PuzzleDescriptionText;
  private int m_currentPuzzleProgress;
  private int m_totalPuzzleProgress;

  public void Show() => this.gameObject.SetActive(true);

  public void Hide() => this.gameObject.SetActive(false);

  public void UpdateNameAndText(string puzzleName, string puzzleText)
  {
    this.m_PuzzleNameText.Text = puzzleName;
    this.m_PuzzleDescriptionText.Text = puzzleText;
  }

  public void UpdateProgressValues(int puzzleProgress, int totalPuzzleProgress)
  {
    this.m_currentPuzzleProgress = puzzleProgress;
    this.m_totalPuzzleProgress = totalPuzzleProgress;
    this.m_ProgressText.Text = string.Format("{0}/{1}", (object) puzzleProgress, (object) totalPuzzleProgress);
  }

  public void UpdateText(Entity puzzleEntity)
  {
    this.m_currentPuzzleProgress = puzzleEntity.GetTag(GAME_TAG.PUZZLE_PROGRESS);
    this.m_totalPuzzleProgress = puzzleEntity.GetTag(GAME_TAG.PUZZLE_PROGRESS_TOTAL);
    this.m_ProgressText.Text = string.Format("{0}/{1}", (object) this.m_currentPuzzleProgress, (object) this.m_totalPuzzleProgress);
    this.m_PuzzleNameText.Text = puzzleEntity.GetName();
    this.m_PuzzleDescriptionText.Text = puzzleEntity.GetCardTextInHand();
  }

  public void IncrementPuzzleProgress() => this.UpdateProgressValues(this.m_currentPuzzleProgress + 1, this.m_totalPuzzleProgress);
}
