using System;
using System.Collections.Generic;

public class DyingSecretGroup
{
  private Card m_mainCard;
  private List<Card> m_cards = new List<Card>();
  private List<Actor> m_actors = new List<Actor>();

  public Card GetMainCard() => this.m_mainCard;

  public List<Actor> GetActors() => this.m_actors;

  public List<Card> GetCards() => this.m_cards;

  public void AddCard(Card card)
  {
    if ((UnityEngine.Object) this.m_mainCard == (UnityEngine.Object) null)
      this.m_mainCard = card.GetZone().GetCards().Find((Predicate<Card>) (currCard => currCard.IsShown()));
    this.m_cards.Add(card);
    this.m_actors.Add(card.GetActor());
  }
}
