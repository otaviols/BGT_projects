using Hearthstone.Progression;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleCardBaconHeroesFilter : CollectibleCardFilter
{
  private int m_heroesPerPage = 6;
  private int m_heroCount;
  private int m_totalPages;
  private List<CollectibleCard> m_allBGHeroes = new List<CollectibleCard>();
  private static IComparer<CollectibleCard> s_HeroSkinComparerAllMode = (IComparer<CollectibleCard>) new CollectibleCardBaconHeroesFilter.HeroSkinComparerAllMode();
  private static IComparer<CollectibleCard> s_HeroSkinComparerDefaultMode = (IComparer<CollectibleCard>) new CollectibleCardBaconHeroesFilter.HeroSkinComparerDefaultMode();

  public void Init(int heroesPerPage) => this.m_heroesPerPage = heroesPerPage;

  public override void UpdateResults()
  {
    CollectionUtils.BattlegroundsHeroSkinFilterMode heroSkinFilterMode = (CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay).GetHeroSkinFilterMode();
    this.m_allBGHeroes.Clear();
    List<CollectionManager.CollectibleCardFilterFunc> collectibleCardFilterFuncList = new List<CollectionManager.CollectibleCardFilterFunc>();
    if (!string.IsNullOrEmpty(this.m_filterText))
      collectibleCardFilterFuncList.AddRange((IEnumerable<CollectionManager.CollectibleCardFilterFunc>) this.FiltersFromSearchString(this.m_filterText));
    List<string> battlegroundsHeroCardIds = CollectionManager.Get().GetAllBattlegroundsHeroCardIds();
    for (int index1 = 0; index1 < battlegroundsHeroCardIds.Count; ++index1)
    {
      string str = battlegroundsHeroCardIds[index1];
      EntityDef entityDef1 = DefLoader.Get().GetEntityDef(str);
      CollectibleCard card = new CollectibleCard(GameUtils.GetCardRecord(str), entityDef1, TAG_PREMIUM.NORMAL);
      card.OwnedCount = !entityDef1.HasTag(GAME_TAG.BACON_SKIN) ? 1 : (CollectionManager.Get().OwnsBattlegroundsHeroSkin(card.CardId) ? 1 : 0);
      if (card.OwnedCount != 0 || heroSkinFilterMode != CollectionUtils.BattlegroundsHeroSkinFilterMode.DEFAULT)
      {
        card.SeenCount = card.OwnedCount;
        if (card.SeenCount > 0 && CollectionManager.Get().ShouldShowNewBattlegroundsHeroSkinGlow(str))
          card.SeenCount = 0;
        string battlegroundsBaseHeroCardId = CollectionManager.Get().GetBattlegroundsBaseHeroCardId(str);
        CardDbfRecord cardRecord = GameUtils.GetCardRecord(battlegroundsBaseHeroCardId);
        EntityDef entityDef2 = DefLoader.Get().GetEntityDef(battlegroundsBaseHeroCardId);
        if ((BaconHeroSkinUtils.GetBattleGroundsHeroRotationType(cardRecord, entityDef2) != BaconHeroSkinUtils.RotationType.Resting || heroSkinFilterMode != CollectionUtils.BattlegroundsHeroSkinFilterMode.DEFAULT && !entityDef1.HasTag(GAME_TAG.BACON_OMIT_WHEN_OUT_OF_ROTATION)) && (heroSkinFilterMode != CollectionUtils.BattlegroundsHeroSkinFilterMode.DEFAULT || RewardTrackManager.Get().HasBattlegroundsPreviewHeroes() || BaconHeroSkinUtils.GetBattleGroundsHeroRotationType(cardRecord, entityDef2) != BaconHeroSkinUtils.RotationType.Preview))
        {
          bool flag = false;
          for (int index2 = 0; index2 < collectibleCardFilterFuncList.Count; ++index2)
          {
            if (!collectibleCardFilterFuncList[index2](card))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            this.m_allBGHeroes.Add(card);
        }
      }
    }
    switch (heroSkinFilterMode)
    {
      case CollectionUtils.BattlegroundsHeroSkinFilterMode.DEFAULT:
        this.m_allBGHeroes.Sort(CollectibleCardBaconHeroesFilter.s_HeroSkinComparerDefaultMode);
        break;
      case CollectionUtils.BattlegroundsHeroSkinFilterMode.ALL:
        this.m_allBGHeroes.Sort(CollectibleCardBaconHeroesFilter.s_HeroSkinComparerAllMode);
        break;
      default:
        Log.CollectionManager.PrintError("Battlegrounds heroes filtered by an unknown mode type.");
        break;
    }
    this.m_heroCount = this.m_allBGHeroes.Count;
    this.m_totalPages = this.m_heroCount / this.m_heroesPerPage + (this.m_heroCount % this.m_heroesPerPage > 0 ? 1 : 0);
  }

  public override List<CollectibleCard> GetPageContents(int page) => this.m_allBGHeroes.Skip<CollectibleCard>(this.m_heroesPerPage * (page - 1)).Take<CollectibleCard>(this.m_heroesPerPage).ToList<CollectibleCard>();

  public override List<CollectibleCard> GetFirstNonEmptyPage(
    out int collectionPage)
  {
    collectionPage = 0;
    for (int currentPageNumber = 0; currentPageNumber < this.GetTotalNumPages(); ++currentPageNumber)
    {
      List<CollectibleCard> pageContents = this.GetPageContents(currentPageNumber);
      if (pageContents.Count > 0)
      {
        collectionPage = currentPageNumber;
        return pageContents;
      }
    }
    return new List<CollectibleCard>();
  }

  public override int GetTotalNumPages() => this.m_totalPages;

  private class HeroSkinComparerAllMode : IComparer<CollectibleCard>
  {
    public int Compare(CollectibleCard card1, CollectibleCard card2)
    {
      string str1 = GameStrings.Get(card1.Name);
      string str2 = str1;
      bool flag1 = card1.GetEntityDef().HasTag(GAME_TAG.BACON_SKIN);
      if (flag1)
      {
        int tag = card1.GetEntityDef().GetTag(GAME_TAG.BACON_SKIN_PARENT_ID);
        string cardId = GameUtils.TranslateDbIdToCardId(tag);
        if (cardId == null)
        {
          Debug.LogError((object) string.Format("BattlegroundsCollectibleCardHeroesFilter.HeroSkinComparer: Could not find card with asset ID {0} in our card manifest", (object) tag));
          return 0;
        }
        str2 = GameStrings.Get((string) GameUtils.GetCardRecord(cardId).Name);
      }
      string strB1 = GameStrings.Get(card2.Name);
      string strB2 = strB1;
      bool flag2 = card2.GetEntityDef().HasTag(GAME_TAG.BACON_SKIN);
      if (flag2)
      {
        int tag = card2.GetEntityDef().GetTag(GAME_TAG.BACON_SKIN_PARENT_ID);
        string cardId = GameUtils.TranslateDbIdToCardId(tag);
        if (cardId == null)
        {
          Debug.LogError((object) string.Format("BattlegroundsCollectibleCardHeroesFilter.HeroSkinComparer: Could not find card with asset ID {0} in our card manifest", (object) tag));
          return 0;
        }
        strB2 = GameStrings.Get((string) GameUtils.GetCardRecord(cardId).Name);
      }
      if (str2 != strB2)
        return str2.CompareTo(strB2);
      if (flag1 && !flag2)
        return 1;
      return flag2 && !flag1 ? -1 : str1.CompareTo(strB1);
    }
  }

  private class HeroSkinComparerDefaultMode : IComparer<CollectibleCard>
  {
    public int Compare(CollectibleCard card1, CollectibleCard card2)
    {
      if (card1.OwnedCount == 0 && card2.OwnedCount > 0)
        return 1;
      if (card1.OwnedCount > 0 && card2.OwnedCount == 0)
        return -1;
      int num = !card1.GetEntityDef().HasTag(GAME_TAG.BACON_SKIN) ? 1 : 0;
      bool flag1 = !card2.GetEntityDef().HasTag(GAME_TAG.BACON_SKIN);
      bool flag2 = num != 0 && !CollectionManager.Get().OwnsAssociatedBattlegroundsHeroSkin(card1.CardDbId);
      bool flag3 = flag1 && !CollectionManager.Get().OwnsAssociatedBattlegroundsHeroSkin(card2.CardDbId);
      if (flag2 && !flag3)
        return 1;
      return !flag2 & flag3 ? -1 : CollectibleCardBaconHeroesFilter.s_HeroSkinComparerAllMode.Compare(card1, card2);
    }
  }
}
