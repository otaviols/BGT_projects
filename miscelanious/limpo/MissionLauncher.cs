using Hearthstone.UI;
using PegasusShared;
using UnityEngine;

public class MissionLauncher : MonoBehaviour
{
  public GameType GameType;
  public FormatType FormatType;
  public int MissionId;
  public Clickable Button;

  private void Awake() => this.Button?.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HandleClick));

  private void HandleClick(UIEvent e)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.MissionId);
    GameMgr.Get().FindGameWithHero(this.GameType, this.FormatType, record.ID, 0, record.Player1HeroCardId, (long) record.Player1DeckId);
  }
}
