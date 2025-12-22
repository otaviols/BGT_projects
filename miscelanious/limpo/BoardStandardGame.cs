using Blizzard.T5.MaterialService.Extensions;

public class BoardStandardGame : BoardLayout
{
  public Actor[] m_DeckActors;
  private static BoardStandardGame s_instance;

  private void Start() => this.DeckColors();

  public void DeckColors()
  {
    foreach (Actor deckActor in this.m_DeckActors)
      deckActor.GetMeshRenderer().GetMaterial().color = Board.Get().m_DeckColor;
  }
}
