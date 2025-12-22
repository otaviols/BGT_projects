using System.Collections.Generic;

public class DeadSecretGroup
{
  private Card m_mainCard;
  private List<Card> m_cards = new List<Card>();

  public Card GetMainCard() => this.m_mainCard;

  public void SetMainCard(Card card) => this.m_mainCard = card;

  public List<Card> GetCards() => this.m_cards;

  public void AddCard(Card card) => this.m_cards.Add(card);
}
