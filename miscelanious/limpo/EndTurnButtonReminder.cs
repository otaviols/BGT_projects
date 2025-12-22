using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButtonReminder : MonoBehaviour
{
  public float m_MaxDelaySec = 0.3f;
  private List<Card> m_cardsWaitingToRemind = new List<Card>();

  public bool ShowFriendlySidePlayerTurnReminder()
  {
    GameState state = GameState.Get();
    if (state.IsMulliganManagerActive())
      return false;
    Player friendlySidePlayer = state.GetFriendlySidePlayer();
    if (friendlySidePlayer == null || !friendlySidePlayer.IsCurrentPlayer())
      return false;
    ZoneMgr zoneMgr = ZoneMgr.Get();
    if ((Object) zoneMgr == (Object) null)
      return false;
    ZonePlay zoneOfType = zoneMgr.FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
    if ((Object) zoneOfType == (Object) null)
      return false;
    List<Card> cardsToRemindList = this.GenerateCardsToRemindList(state, zoneOfType.GetCards());
    if (cardsToRemindList.Count == 0)
      return true;
    this.PlayReminders(cardsToRemindList);
    return true;
  }

  private List<Card> GenerateCardsToRemindList(GameState state, List<Card> originalList)
  {
    List<Card> cardsToRemindList = new List<Card>();
    GameEntity gameEntity = GameState.Get()?.GetGameEntity();
    foreach (Card original in originalList)
    {
      bool showReminder;
      if (gameEntity != null && gameEntity.OverwriteEndTurnReminder(original.GetEntity(), out showReminder))
      {
        if (showReminder)
          cardsToRemindList.Add(original);
      }
      else if (state.HasResponse(original.GetEntity()))
        cardsToRemindList.Add(original);
    }
    return cardsToRemindList;
  }

  private void PlayReminders(List<Card> cards)
  {
    int index1;
    do
    {
      index1 = Random.Range(0, cards.Count);
    }
    while (this.m_cardsWaitingToRemind.Contains(cards[index1]));
    for (int index2 = 0; index2 < cards.Count; ++index2)
    {
      Card card = cards[index2];
      Spell actorSpell = card.GetActorSpell(SpellType.WIGGLE);
      if (!((Object) actorSpell == (Object) null) && actorSpell.GetActiveState() == SpellStateType.NONE && !this.m_cardsWaitingToRemind.Contains(card))
      {
        if (index2 == index1)
        {
          actorSpell.Activate();
        }
        else
        {
          float num = Random.Range(0.0f, this.m_MaxDelaySec);
          if (Mathf.Approximately(num, 0.0f))
          {
            actorSpell.Activate();
          }
          else
          {
            this.m_cardsWaitingToRemind.Add(card);
            this.StartCoroutine(this.WaitAndPlayReminder(card, actorSpell, num));
          }
        }
      }
    }
  }

  private IEnumerator WaitAndPlayReminder(Card card, Spell reminderSpell, float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    if (GameState.Get().IsFriendlySidePlayerTurn() && card.GetZone() is ZonePlay)
    {
      reminderSpell.Activate();
      this.m_cardsWaitingToRemind.Remove(card);
    }
  }
}
