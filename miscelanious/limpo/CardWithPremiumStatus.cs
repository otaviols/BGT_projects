using System.Collections.Generic;

public class CardWithPremiumStatus
{
  public CardWithPremiumStatus(long id, TAG_PREMIUM tag)
  {
    this.cardId = id;
    this.premium = tag;
  }

  public long cardId { get; }

  public TAG_PREMIUM premium { get; set; }

  public static List<CardWithPremiumStatus> ConvertList(List<long> cards)
  {
    List<CardWithPremiumStatus> withPremiumStatusList = new List<CardWithPremiumStatus>();
    for (int index = 0; index < cards.Count; ++index)
    {
      CardWithPremiumStatus withPremiumStatus = new CardWithPremiumStatus(cards[index], TAG_PREMIUM.NORMAL);
      withPremiumStatusList.Add(withPremiumStatus);
    }
    return withPremiumStatusList;
  }
}
