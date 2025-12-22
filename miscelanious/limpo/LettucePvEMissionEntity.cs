using Blizzard.T5.Core;
using System;
using System.Collections.Generic;

public class LettucePvEMissionEntity : LettuceMissionEntity
{
  private bool m_skipTutorial;
  private static Map<GameEntityOption, bool> s_booleanOptions = LettucePvEMissionEntity.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = LettucePvEMissionEntity.InitStringOptions();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public LettucePvEMissionEntity(bool skipTutorial = false, VoPlaybackHandler voHandler = null)
    : base(voHandler)
  {
    this.m_gameOptions.AddOptions(LettucePvEMissionEntity.s_booleanOptions, LettucePvEMissionEntity.s_stringOptions);
    this.m_skipTutorial = skipTutorial;
  }

  public override void UpdateAllMercenaryAbilityOrderBubbleText(bool hideUnselectedAbilityBubbles = false)
  {
    if (this.m_gamePhase == 3 || !this.m_abilityOrderSpeechBubblesEnabled)
      return;
    List<Card> cardList = this.SortDeterministicActionOrder(this.GetAllMinionsInPlay());
    int num = 1;
    for (int index = 0; index < cardList.Count; ++index)
    {
      Card card = cardList[index];
      if (!((UnityEngine.Object) card == (UnityEngine.Object) null) && (this.m_enemyAbilityOrderSpeechBubblesEnabled || !card.GetEntity().IsControlledByOpposingSidePlayer()))
      {
        card.SetLettuceAbilityActionOrder(num++, false);
        card.UpdateLettuceSpeechBubbleText(hideUnselectedAbilityBubbles);
      }
    }
  }

  private List<Card> SortDeterministicActionOrder(List<Card> cardsInPlay)
  {
    if (cardsInPlay == null || cardsInPlay.Count == 0)
      return new List<Card>();
    SortedDictionary<int, List<Card>> sortedDictionary = new SortedDictionary<int, List<Card>>((IComparer<int>) new LettuceMissionEntity.CardSpeedCamparer(this.ShouldSortAbilitiesLowToHigh()));
    foreach (Card card in cardsInPlay)
    {
      if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
      {
        int abilitySpeedValue = card.GetPreparedLettuceAbilitySpeedValue();
        if (sortedDictionary.ContainsKey(abilitySpeedValue))
          sortedDictionary[abilitySpeedValue].Add(card);
        else
          sortedDictionary.Add(abilitySpeedValue, new List<Card>()
          {
            card
          });
      }
    }
    List<Card> cardList1 = new List<Card>(12);
    List<Card> cardList2 = new List<Card>(12);
    List<Card> cardList3 = new List<Card>(12);
    foreach (KeyValuePair<int, List<Card>> keyValuePair in sortedDictionary)
    {
      List<Card> cardList4 = keyValuePair.Value;
      if (cardList4.Count == 1)
      {
        cardList3.Add(cardList4[0]);
      }
      else
      {
        cardList1.Clear();
        cardList2.Clear();
        foreach (Card card in cardList4)
        {
          if (card.GetEntity().IsControlledByFriendlySidePlayer())
            cardList1.Add(card);
          else
            cardList2.Add(card);
        }
        cardList1.Sort((Comparison<Card>) ((c1, c2) => c1.GetEntity().GetTag(GAME_TAG.LETTUCE_SELECTED_ABILITY_QUEUE_ORDER).CompareTo(c2.GetEntity().GetTag(GAME_TAG.LETTUCE_SELECTED_ABILITY_QUEUE_ORDER))));
        cardList2.Sort((Comparison<Card>) ((c1, c2) => c1.GetEntity().GetTag(GAME_TAG.ZONE_POSITION).CompareTo(c2.GetEntity().GetTag(GAME_TAG.ZONE_POSITION))));
        foreach (Card card in cardList1)
          cardList3.Add(card);
        foreach (Card card in cardList2)
          cardList3.Add(card);
      }
    }
    return cardList3;
  }
}
