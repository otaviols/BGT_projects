using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using Hearthstone.Util;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class OfflineDataCache
{
  public static void CacheLocalAndOriginalDeckList(
    ref OfflineDataCache.OfflineData data,
    List<DeckInfo> localDecklist,
    List<DeckInfo> originalDecklist)
  {
    data.LocalDeckList = localDecklist;
    data.OriginalDeckList = originalDecklist;
    Log.Offline.PrintDebug("OfflineDataCache: Caching local deck list. Local Count={0}, Original Count={1}", (object) localDecklist.Count, (object) originalDecklist.Count);
  }

  public static void CacheLocalAndOriginalDeckList(
    List<DeckInfo> localDecklist,
    List<DeckInfo> originalDecklist)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    data.LocalDeckList = localDecklist;
    data.OriginalDeckList = originalDecklist;
    OfflineDataCache.WriteOfflineDataToFile(data);
    Log.Offline.PrintDebug("OfflineDataCache: Caching local deck list. Local Count={0}, Original Count={1}", (object) localDecklist.Count, (object) originalDecklist.Count);
  }

  public static void CacheLocalAndOriginalDeckContents(
    ref OfflineDataCache.OfflineData data,
    PegasusUtil.DeckContents localDeckContents,
    PegasusUtil.DeckContents originalDeckContents)
  {
    OfflineDataCache.SetLocalDeckContentsInOfflineData(ref data, localDeckContents);
    OfflineDataCache.SetOriginalDeckContentsInOfflineData(ref data, originalDeckContents);
  }

  private static void SetOriginalDeckContentsInOfflineData(
    ref OfflineDataCache.OfflineData data,
    PegasusUtil.DeckContents packet)
  {
    if (data.OriginalDeckContents == null)
      data.OriginalDeckContents = new List<PegasusUtil.DeckContents>();
    data.OriginalDeckContents.RemoveAll((Predicate<PegasusUtil.DeckContents>) (c => c.DeckId == packet.DeckId));
    data.OriginalDeckContents.Add(packet);
    Log.Offline.PrintDebug("OfflineDataCache: Caching original deck contents: id={0}", (object) packet.DeckId);
  }

  private static void SetLocalDeckContentsInOfflineData(
    ref OfflineDataCache.OfflineData data,
    PegasusUtil.DeckContents packet)
  {
    if (data.LocalDeckContents == null)
      data.LocalDeckContents = new List<PegasusUtil.DeckContents>();
    data.LocalDeckContents.RemoveAll((Predicate<PegasusUtil.DeckContents>) (c => c.DeckId == packet.DeckId));
    data.LocalDeckContents.Add(packet);
    Log.Offline.PrintDebug("OfflineDataCache: Caching local deck contents: id={0}", (object) packet.DeckId);
  }

  public static void CacheFavoriteHeroes(
    ref OfflineDataCache.OfflineData data,
    FavoriteHeroesResponse packet)
  {
    data.FavoriteHeroes = new List<FavoriteHero>((IEnumerable<FavoriteHero>) packet.FavoriteHeroes);
    Log.Offline.PrintDebug("OfflineDataCache: Caching favorite heroes: {0}", (object) packet.ToHumanReadableString());
  }

  public static void CacheCardBacks(ref OfflineDataCache.OfflineData data, CardBacks packet)
  {
    data.CardBacks = packet;
    Log.Offline.PrintDebug("OfflineDataCache: Caching favorite card backs: {0}", (object) packet.ToHumanReadableString());
  }

  public static void CacheCoins(ref OfflineDataCache.OfflineData data, Coins packet)
  {
    data.Coins = packet;
    Log.Offline.PrintDebug("OfflineDataCache: Caching favorite coins: {0}", (object) packet.ToHumanReadableString());
  }

  public static List<PegasusUtil.DeckContents> GetLocalDeckContentsFromCache() => OfflineDataCache.ReadOfflineDataFromFile().LocalDeckContents;

  public static List<FavoriteHero> GetFavoriteHeroesFromCache() => OfflineDataCache.ReadOfflineDataFromFile().FavoriteHeroes;

  public static CardBacks GetCardBacksFromCache() => OfflineDataCache.ReadOfflineDataFromFile().CardBacks;

  public static Coins GetCoinsFromCache() => OfflineDataCache.ReadOfflineDataFromFile().Coins;

  public static DeckInfo GetDeckInfoFromDeckList(long deckId, List<DeckInfo> deckList)
  {
    if (deckList == null)
      return (DeckInfo) null;
    int num = 0;
    foreach (DeckInfo deck in deckList)
    {
      if (deck.Id == deckId)
        ++num;
    }
    if (num > 1)
      Log.Offline.PrintError("GetDeckInfoFromDeckList: Found multiple decks in cache with id: {0}", (object) deckId);
    foreach (DeckInfo deck in deckList)
    {
      if (deck.Id == deckId)
        return deck;
    }
    Log.Offline.PrintWarning("GetDeckInfoFromDeckList: No deck header found with id: {0}", (object) deckId);
    return (DeckInfo) null;
  }

  public static PegasusUtil.DeckContents GetDeckContentsFromDeckContentsList(
    long deckId,
    List<PegasusUtil.DeckContents> list)
  {
    if (list == null)
      return (PegasusUtil.DeckContents) null;
    if (list.Count<PegasusUtil.DeckContents>((Func<PegasusUtil.DeckContents, bool>) (d => d.DeckId == deckId)) > 1)
      Log.Offline.PrintError("GetDeckContentsFromDeckContentsList: Found multiple decks in cache with id: {0}", (object) deckId);
    foreach (PegasusUtil.DeckContents deckContentsList in list)
    {
      if (deckContentsList.DeckId == deckId)
        return deckContentsList;
    }
    Log.Offline.PrintWarning("GetDeckContentsFromDeckContentsList: No deck contents found with id: {0}", (object) deckId);
    return (PegasusUtil.DeckContents) null;
  }

  public static List<long> GetFakeDeckIds(OfflineDataCache.OfflineData data = null)
  {
    if (data == null)
      data = OfflineDataCache.ReadOfflineDataFromFile();
    List<long> fakeDeckIds = new List<long>();
    if (data.FakeDeckIds == null)
      return fakeDeckIds;
    foreach (long fakeDeckId in data.FakeDeckIds)
    {
      if (OfflineDataCache.IsValidFakeId(fakeDeckId) && !fakeDeckIds.Contains(fakeDeckId))
        fakeDeckIds.Add(fakeDeckId);
    }
    return fakeDeckIds;
  }

  public static List<DeckInfo> GetFakeDeckInfos(OfflineDataCache.OfflineData data)
  {
    List<DeckInfo> fakeDeckInfos = new List<DeckInfo>();
    foreach (long fakeDeckId in OfflineDataCache.GetFakeDeckIds(data))
    {
      DeckInfo deckInfo = (DeckInfo) null;
      foreach (DeckInfo localDeck in data.LocalDeckList)
      {
        if (localDeck.Id == fakeDeckId)
        {
          deckInfo = localDeck;
          break;
        }
      }
      if (deckInfo != null)
        fakeDeckInfos.Add(deckInfo);
    }
    return fakeDeckInfos;
  }

  public static void ClearFakeDeckIds(ref OfflineDataCache.OfflineData data)
  {
    if (data.FakeDeckIds == null)
      return;
    foreach (long fakeDeckId in data.FakeDeckIds)
      OfflineDataCache.DeleteDeck(fakeDeckId);
    data.FakeDeckIds = new List<long>();
    --data.UniqueFakeDeckId;
    Log.Offline.PrintDebug("OfflineDataCache: Clearing Fake Deck Ids");
  }

  public static bool UpdateDeckWithNewId(long oldId, long newId)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    Log.Offline.PrintDebug("OfflineDataCache: Updating deck {0} with new id {1}", (object) oldId, (object) newId);
    DeckInfo infoFromDeckList = OfflineDataCache.GetDeckInfoFromDeckList(oldId, data.LocalDeckList);
    PegasusUtil.DeckContents deckContentsList = OfflineDataCache.GetDeckContentsFromDeckContentsList(oldId, data.LocalDeckContents);
    if (infoFromDeckList != null)
    {
      infoFromDeckList.Id = newId;
      if (deckContentsList != null)
      {
        deckContentsList.DeckId = newId;
        OfflineDataCache.WriteOfflineDataToFile(data);
        return true;
      }
      Log.Offline.PrintError("UpdateDeckWithNewId: No deck contents found in Offline Data Cache with old id: {0}", (object) oldId);
      return false;
    }
    Log.Offline.PrintError("UpdateDeckWithNewId: No deck info found in Offline Data Cache with old id: {0}", (object) oldId);
    return false;
  }

  public static void RenameDeck(long deckId, string newName)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    DeckInfo infoFromDeckList = OfflineDataCache.GetDeckInfoFromDeckList(deckId, data.LocalDeckList);
    if (infoFromDeckList == null)
    {
      Log.Offline.PrintError("Received a rename command for deck id={0}, name={1}, but a deck with that id was not found in the OfflineDataCache.", (object) deckId, (object) newName);
    }
    else
    {
      infoFromDeckList.Name = newName;
      Log.Offline.PrintDebug("OfflineDataCache: Renaming deck {0} to {1}", (object) deckId, (object) newName);
      OfflineDataCache.WriteOfflineDataToFile(data);
    }
  }

  public static void SetFavoriteCardBack(int cardBackId, bool isFavorite = true)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    if (isFavorite)
    {
      data.CardBacks.FavoriteCardBacks.Remove(cardBackId);
      Log.Offline.PrintDebug("OfflineDataCache: Removed favorite card back {0}", (object) cardBackId);
    }
    else
    {
      data.CardBacks.FavoriteCardBacks.Add(cardBackId);
      Log.Offline.PrintDebug("OfflineDataCache: Added favorite card back {0}", (object) cardBackId);
    }
    data.m_hasChangedCardBacksOffline = true;
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  public static void ClearCardBackDirtyFlag(ref OfflineDataCache.OfflineData data)
  {
    data.m_hasChangedCardBacksOffline = false;
    Log.Offline.PrintDebug("OfflineDataCache: Clearing card back flag");
  }

  public static void SetFavoriteCoin(ref OfflineDataCache.OfflineData data, int coinId)
  {
    data.Coins.FavoriteCoin = coinId;
    data.m_hasChangedCoinsOffline = true;
    Log.Offline.PrintDebug("OfflineDataCache: Set favorite coin to {0}", (object) coinId);
  }

  public static void ClearCoinDirtyFlag(ref OfflineDataCache.OfflineData data)
  {
    data.m_hasChangedCoinsOffline = false;
    Log.Offline.PrintDebug("OfflineDataCache: Clearing coin flag");
  }

  public static void SetFavoriteHero(
    int heroClass,
    PegasusShared.CardDef cardDef,
    bool wasCalledOffline,
    bool isFavorite)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    FavoriteHero favoriteHero = data.FavoriteHeroes.Find((Predicate<FavoriteHero>) (favorite => favorite.Hero.Asset == cardDef.Asset));
    if (favoriteHero != null)
    {
      if (isFavorite)
        favoriteHero.Hero = cardDef;
      else
        data.FavoriteHeroes.Remove(favoriteHero);
    }
    else if (isFavorite)
      data.FavoriteHeroes.Add(new FavoriteHero()
      {
        ClassId = heroClass,
        Hero = cardDef
      });
    if (wasCalledOffline)
      data.m_hasChangedFavoriteHeroesOffline = true;
    Log.Offline.PrintDebug("OfflineDataCache: Setting favorite hero for class {0} to {1}", (object) heroClass, (object) cardDef.ToHumanReadableString());
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  public static void ClearFavoriteHeroesDirtyFlag()
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    data.m_hasChangedFavoriteHeroesOffline = false;
    Log.Offline.PrintDebug("OfflineDataCache: Clearing favorite hero flag");
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  public static long GetCachedCollectionVersion(OfflineDataCache.OfflineData data = null)
  {
    if (data == null)
      data = OfflineDataCache.ReadOfflineDataFromFile();
    return data != null && data.Collection != null && data.Collection.HasCollectionVersion ? data.Collection.CollectionVersion : 0L;
  }

  public static long GetCachedCollectionVersionLastModified(OfflineDataCache.OfflineData data = null)
  {
    if (data == null)
      data = OfflineDataCache.ReadOfflineDataFromFile();
    return data != null && data.Collection != null && data.Collection.HasCollectionVersionLastModified ? data.Collection.CollectionVersionLastModified : 0L;
  }

  public static void CacheCollection(ref OfflineDataCache.OfflineData data, Collection collection) => data.Collection = collection;

  public static List<GetAssetsVersion.DeckModificationTimes> GetCachedDeckContentsTimes(
    OfflineDataCache.OfflineData data = null)
  {
    List<GetAssetsVersion.DeckModificationTimes> deckContentsTimes = new List<GetAssetsVersion.DeckModificationTimes>();
    if (data == null)
      data = OfflineDataCache.ReadOfflineDataFromFile();
    if (data == null || data.LocalDeckContents == null || data.LocalDeckList == null)
      return deckContentsTimes;
    foreach (PegasusUtil.DeckContents localDeckContent in data.LocalDeckContents)
    {
      PegasusUtil.DeckContents deckContent = localDeckContent;
      DeckInfo deckInfo = data.LocalDeckList.Find((Predicate<DeckInfo>) (list => list.Id == deckContent.DeckId));
      if (deckInfo != null)
        deckContentsTimes.Add(new GetAssetsVersion.DeckModificationTimes()
        {
          DeckId = deckContent.DeckId,
          LastModified = deckInfo.LastModified
        });
    }
    return deckContentsTimes;
  }

  public static void ApplyDeckSetDataLocally(DeckSetData packet)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    OfflineDataCache.ApplyDeckSetDataToDeck(packet, data.LocalDeckList, data.LocalDeckContents);
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  public static void ApplyDeckSetDataToOriginalDeck(DeckSetData packet)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    OfflineDataCache.ApplyDeckSetDataToDeck(packet, data.OriginalDeckList, data.OriginalDeckContents);
    Log.Offline.PrintDebug("OfflineDataCache: Applying deck changes to deck. Changes: {0}", (object) packet.ToHumanReadableString());
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  public static void ApplyDeckSetDataToDeck(
    DeckSetData packet,
    List<DeckInfo> deckList,
    List<PegasusUtil.DeckContents> deckContentsList)
  {
    DeckInfo deckInfo = (DeckInfo) null;
    foreach (DeckInfo deck in deckList)
    {
      if (deck.Id == packet.Deck)
      {
        deckInfo = deck;
        break;
      }
    }
    if (deckInfo == null)
    {
      deckInfo = new DeckInfo();
      deckInfo.Id = packet.Deck;
      deckList.Add(deckInfo);
    }
    PegasusUtil.DeckContents deckContents1 = (PegasusUtil.DeckContents) null;
    foreach (PegasusUtil.DeckContents deckContents2 in deckContentsList)
    {
      if (deckContents2.DeckId == packet.Deck)
      {
        deckContents1 = deckContents2;
        break;
      }
    }
    if (deckContents1 == null)
    {
      deckContents1 = new PegasusUtil.DeckContents();
      deckContents1.DeckId = packet.Deck;
      deckContentsList.Add(deckContents1);
    }
    if (packet.HasCardBack)
    {
      deckInfo.HasCardBack = true;
      deckInfo.CardBack = packet.CardBack;
    }
    else if (packet.HasRemovingCardBack)
      deckInfo.HasCardBack = false;
    if (packet.HasHero)
      deckInfo.Hero = packet.Hero;
    if (packet.HasSortOrder)
      deckInfo.SortOrder = packet.SortOrder;
    if (packet.HasPastedDeckHash)
      deckInfo.PastedDeckHash = packet.PastedDeckHash;
    if (packet.Cards != null)
    {
      foreach (DeckCardData card1 in packet.Cards)
      {
        bool flag = false;
        foreach (DeckCardData card2 in deckContents1.Cards)
        {
          if (card2.Def.Asset == card1.Def.Asset && card2.Def.Premium == card1.Def.Premium)
          {
            card2.Qty = card1.Qty;
            flag = true;
            break;
          }
        }
        if (!flag)
          deckContents1.Cards.Add(card1);
      }
    }
    if (deckInfo.Name == null)
      deckInfo.Name = "Unknown";
    deckInfo.LastModified = (long) TimeUtils.DateTimeToUnixTimeStamp(DateTime.Now);
  }

  public static bool GenerateDeckSetDataFromDiff(
    long deckId,
    DeckInfo patchingDeckInfo,
    DeckInfo originalDeckInfo,
    PegasusUtil.DeckContents patchingDeckContents,
    PegasusUtil.DeckContents originalDeckContents,
    out DeckSetData deckSetData)
  {
    deckSetData = new DeckSetData();
    deckSetData.Deck = deckId;
    bool deckSetDataFromDiff = false;
    if (!patchingDeckInfo.HasCardBack && originalDeckInfo.HasCardBack)
    {
      deckSetData.RemovingCardBack = true;
      deckSetData.HasCardBack = false;
      deckSetDataFromDiff = true;
    }
    else if (patchingDeckInfo.HasCardBack && (!originalDeckInfo.HasCardBack || patchingDeckInfo.CardBack != originalDeckInfo.CardBack))
    {
      deckSetData.HasCardBack = true;
      deckSetData.CardBack = patchingDeckInfo.CardBack;
      deckSetDataFromDiff = true;
    }
    if (patchingDeckInfo.Hero != originalDeckInfo.Hero)
    {
      deckSetData.Hero = patchingDeckInfo.Hero;
      deckSetDataFromDiff = true;
    }
    if (patchingDeckInfo.RandomHeroUseFavorite != originalDeckInfo.RandomHeroUseFavorite)
    {
      deckSetData.RandomHeroUseFavorite = patchingDeckInfo.RandomHeroUseFavorite;
      deckSetDataFromDiff = true;
    }
    if (!string.Equals(patchingDeckInfo.PastedDeckHash, originalDeckInfo.PastedDeckHash))
    {
      deckSetData.PastedDeckHash = patchingDeckInfo.PastedDeckHash;
      deckSetDataFromDiff = true;
    }
    if (patchingDeckInfo.SortOrder != originalDeckInfo.SortOrder)
    {
      deckSetData.SortOrder = patchingDeckInfo.SortOrder;
      deckSetDataFromDiff = true;
    }
    deckSetData.Cards = OfflineDataCache.GetDeckContentsDelta(patchingDeckContents, originalDeckContents);
    if (deckSetData.Cards.Any<DeckCardData>())
      deckSetDataFromDiff = true;
    return deckSetDataFromDiff;
  }

  public static bool GenerateDeckSetDataFromDiff(
    long deckId,
    List<DeckInfo> patchingDeckList,
    List<DeckInfo> originalDeckList,
    List<PegasusUtil.DeckContents> patchingDeckContentsList,
    List<PegasusUtil.DeckContents> originalDeckContentsList,
    out DeckSetData deckSetData)
  {
    DeckInfo patchingDeckInfo = OfflineDataCache.GetDeckInfoFromDeckList(deckId, patchingDeckList);
    DeckInfo originalDeckInfo = OfflineDataCache.GetDeckInfoFromDeckList(deckId, originalDeckList);
    PegasusUtil.DeckContents patchingDeckContents = OfflineDataCache.GetDeckContentsFromDeckContentsList(deckId, patchingDeckContentsList);
    PegasusUtil.DeckContents originalDeckContents = OfflineDataCache.GetDeckContentsFromDeckContentsList(deckId, originalDeckContentsList);
    if (patchingDeckInfo == null)
      patchingDeckInfo = new DeckInfo();
    if (originalDeckInfo == null)
      originalDeckInfo = new DeckInfo();
    if (patchingDeckContents == null)
      patchingDeckContents = new PegasusUtil.DeckContents();
    if (originalDeckContents == null)
      originalDeckContents = new PegasusUtil.DeckContents();
    return OfflineDataCache.GenerateDeckSetDataFromDiff(deckId, patchingDeckInfo, originalDeckInfo, patchingDeckContents, originalDeckContents, out deckSetData);
  }

  public static PegasusUtil.RenameDeck GenerateRenameDeckFromDiff(
    long deckId,
    DeckInfo patchingDeckInfo,
    DeckInfo originalDeckInfo)
  {
    if (string.Equals(patchingDeckInfo.Name, originalDeckInfo.Name))
      return (PegasusUtil.RenameDeck) null;
    return new PegasusUtil.RenameDeck()
    {
      Deck = deckId,
      Name = patchingDeckInfo.Name
    };
  }

  public static void DeleteDeck(long deckId)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    data.LocalDeckList.RemoveAll((Predicate<DeckInfo>) (d => d.Id == deckId));
    data.LocalDeckContents.RemoveAll((Predicate<PegasusUtil.DeckContents>) (d => d.DeckId == deckId));
    Log.Offline.PrintDebug("OfflineDataCache: Deleting deck: {0}", (object) deckId);
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  public static void RemoveAllOldDecksContents(ref OfflineDataCache.OfflineData data)
  {
    if (data.LocalDeckContents != null)
    {
      foreach (PegasusUtil.DeckContents deckContents1 in data.LocalDeckContents.ToArray())
      {
        PegasusUtil.DeckContents deckContents = deckContents1;
        if (!data.LocalDeckList.Any<DeckInfo>((Func<DeckInfo, bool>) (d => d.Id == deckContents.DeckId)))
          data.LocalDeckContents.Remove(deckContents);
      }
    }
    if (data.OriginalDeckContents == null)
      return;
    foreach (PegasusUtil.DeckContents deckContents2 in data.OriginalDeckContents.ToArray())
    {
      PegasusUtil.DeckContents deckContents = deckContents2;
      if (!data.OriginalDeckList.Any<DeckInfo>((Func<DeckInfo, bool>) (d => d.Id == deckContents.DeckId)))
        data.OriginalDeckContents.Remove(deckContents);
    }
  }

  public static DeckInfo CreateDeck(
    DeckType deckType,
    string name,
    int heroDbId,
    FormatType formatType,
    long sortOrder,
    DeckSourceType sourceType,
    string pastedDeckHash = null)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    long recordNextFakeId = OfflineDataCache.GetAndRecordNextFakeId(data.FakeDeckIds, data);
    DeckInfo deckInfo = new DeckInfo()
    {
      Id = recordNextFakeId,
      DeckType = deckType,
      Name = name,
      Hero = heroDbId,
      SourceType = sourceType,
      SortOrder = sortOrder,
      PastedDeckHash = pastedDeckHash,
      Validity = formatType == FormatType.FT_STANDARD ? 128UL : 0UL,
      FormatType = formatType
    };
    data.LocalDeckList.Add(deckInfo);
    data.LocalDeckContents.Add(new PegasusUtil.DeckContents()
    {
      DeckId = recordNextFakeId
    });
    Log.Offline.PrintDebug("OfflineDataCache: Creating offline deck: id={0}", (object) recordNextFakeId);
    return !OfflineDataCache.WriteOfflineDataToFile(data) ? (DeckInfo) null : deckInfo;
  }

  public static List<PegasusUtil.SetFavoriteCardBack> GenerateSetFavoriteCardBackFromDiff(
    OfflineDataCache.OfflineData data,
    List<int> receivedFavoriteCardBacks)
  {
    List<PegasusUtil.SetFavoriteCardBack> cardBackFromDiff = new List<PegasusUtil.SetFavoriteCardBack>();
    if (!data.m_hasChangedCardBacksOffline || data.CardBacks == null)
      return (List<PegasusUtil.SetFavoriteCardBack>) null;
    List<int> list1 = data.CardBacks.FavoriteCardBacks.Except<int>((IEnumerable<int>) receivedFavoriteCardBacks).ToList<int>();
    if (list1.Count > 0)
    {
      for (int index = 0; index < list1.Count; ++index)
        cardBackFromDiff.Add(new PegasusUtil.SetFavoriteCardBack()
        {
          CardBack = list1[index],
          IsFavorite = true
        });
    }
    List<int> list2 = receivedFavoriteCardBacks.Except<int>((IEnumerable<int>) data.CardBacks.FavoriteCardBacks).ToList<int>();
    if (list2.Count > 0)
    {
      for (int index = 0; index < list2.Count; ++index)
        cardBackFromDiff.Add(new PegasusUtil.SetFavoriteCardBack()
        {
          CardBack = list2[index],
          IsFavorite = false
        });
    }
    return cardBackFromDiff;
  }

  public static PegasusUtil.SetFavoriteCoin GenerateSetFavoriteCoinFromDiff(
    OfflineDataCache.OfflineData data,
    int receivedFavoriteCoin)
  {
    if (!data.m_hasChangedCoinsOffline)
      return (PegasusUtil.SetFavoriteCoin) null;
    if (data.Coins == null || data.Coins.FavoriteCoin == receivedFavoriteCoin)
      return (PegasusUtil.SetFavoriteCoin) null;
    return new PegasusUtil.SetFavoriteCoin()
    {
      Coin = data.Coins.FavoriteCoin
    };
  }

  public static List<PegasusUtil.SetFavoriteHero> GenerateSetFavoriteHeroFromDiff(
    OfflineDataCache.OfflineData data,
    NetCache.NetCacheFavoriteHeroes receivedFavoriteHeroes)
  {
    List<PegasusUtil.SetFavoriteHero> favoriteHeroFromDiff = new List<PegasusUtil.SetFavoriteHero>();
    if (!data.m_hasChangedFavoriteHeroesOffline || data.FavoriteHeroes == null)
      return favoriteHeroFromDiff;
    foreach (FavoriteHero favoriteHero in data.FavoriteHeroes)
    {
      FavoriteHero localFavorite = favoriteHero;
      if (!receivedFavoriteHeroes.FavoriteHeroes.Any<(TAG_CLASS, NetCache.CardDefinition)>((Func<(TAG_CLASS, NetCache.CardDefinition), bool>) (hero => hero.Item2.Name == GameUtils.TranslateDbIdToCardId(localFavorite.Hero.Asset))))
      {
        PegasusUtil.SetFavoriteHero setFavoriteHero = new PegasusUtil.SetFavoriteHero()
        {
          FavoriteHero = new FavoriteHero()
          {
            ClassId = localFavorite.ClassId,
            Hero = localFavorite.Hero
          },
          IsFavorite = true
        };
        favoriteHeroFromDiff.Add(setFavoriteHero);
      }
    }
    foreach ((_, _) in receivedFavoriteHeroes.FavoriteHeroes)
    {
      (TAG_CLASS, NetCache.CardDefinition) remoteFavorite;
      if (!data.FavoriteHeroes.Any<FavoriteHero>((Func<FavoriteHero, bool>) (favorite => GameUtils.TranslateDbIdToCardId(favorite.Hero.Asset) == remoteFavorite.Item2.Name)))
      {
        PegasusUtil.SetFavoriteHero setFavoriteHero = new PegasusUtil.SetFavoriteHero()
        {
          FavoriteHero = new FavoriteHero()
          {
            ClassId = (int) remoteFavorite.Item1,
            Hero = new PegasusShared.CardDef()
            {
              Asset = GameUtils.TranslateCardIdToDbId(remoteFavorite.Item2.Name),
              Premium = (int) remoteFavorite.Item2.Premium
            }
          },
          IsFavorite = false
        };
        favoriteHeroFromDiff.Add(setFavoriteHero);
      }
    }
    return favoriteHeroFromDiff;
  }

  private static string GetCacheFolderPath() => string.Format("{0}/{1}", (object) PlatformFilePaths.CachePath, (object) "Offline");

  private static string GetCacheFilePath()
  {
    string cacheFolderPath = OfflineDataCache.GetCacheFolderPath();
    BnetGameAccountId bnetGameAccountId = Network.Get().GetMyGameAccountId() ?? new BnetGameAccountId(0UL, 0UL);
    string str1 = string.Format("{0}_{1}", (object) bnetGameAccountId.High, (object) bnetGameAccountId.Low);
    string str2 = Network.Get().GetCurrentRegion().ToString();
    string str3 = "";
    if (HearthstoneApplication.IsInternal())
      str3 = string.Format("_{0}", (object) "25.0").Replace(".", "_");
    return string.Format("{0}/offlineData_{1}_{2}{3}.cache", (object) cacheFolderPath, (object) str1, (object) str2, (object) str3);
  }

  private static void CreateCacheFolder()
  {
    string cacheFolderPath = OfflineDataCache.GetCacheFolderPath();
    if (Directory.Exists(cacheFolderPath))
      return;
    try
    {
      Directory.CreateDirectory(cacheFolderPath);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) string.Format("UberText.CreateCacheFolder() - Failed to create {0}. Reason={1}", (object) cacheFolderPath, (object) ex.Message));
    }
  }

  public static bool WriteOfflineDataToFile(OfflineDataCache.OfflineData data)
  {
    OfflineDataCache.CreateCacheFolder();
    string cacheFilePath = OfflineDataCache.GetCacheFilePath();
    try
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) File.Open(cacheFilePath, FileMode.Create, FileAccess.Write)))
      {
        writer.Write(1);
        IOfflineDataSerializer serializer = OfflineDataSerializer.GetSerializer(1);
        if (serializer == null)
        {
          Debug.LogErrorFormat("Could not find serializer for writing version {0}. Make sure a new seralizer is added when incrementing versions.", (object) 1);
          return false;
        }
        serializer.Serialize(data, writer);
      }
    }
    catch (IOException ex)
    {
      Log.Offline.PrintError("WriteOfflineDataToFile - Is disk full? - Exception: {0}", (object) ex.InnerException);
      return false;
    }
    catch (UnauthorizedAccessException ex)
    {
      Log.Offline.PrintError("WriteOfflineDataToFile - Are write permissions correctly applied to the file attempting to be accessed? - Exception: {0}", (object) ex.InnerException);
      return false;
    }
    catch (Exception ex)
    {
      Log.Offline.PrintError("WriteOfflineDataToFile - Unexpected exception thrown - Exception: {0}", (object) ex.InnerException);
      return false;
    }
    return true;
  }

  public static OfflineDataCache.OfflineData ReadOfflineDataFromFile()
  {
    OfflineDataCache.OfflineData offlineData = new OfflineDataCache.OfflineData();
    string cacheFolderPath = OfflineDataCache.GetCacheFolderPath();
    string cacheFilePath = OfflineDataCache.GetCacheFilePath();
    if (!Directory.Exists(cacheFolderPath) || !File.Exists(cacheFilePath))
      return offlineData;
    bool flag = false;
    try
    {
      using (BinaryReader reader = new BinaryReader((Stream) File.Open(cacheFilePath, FileMode.Open)))
      {
        int serializerVersion = reader.ReadInt32();
        IOfflineDataSerializer serializer = OfflineDataSerializer.GetSerializer(serializerVersion);
        if (serializer == null)
        {
          Debug.LogWarningFormat("Could not find serializer for offline data version {0}", (object) serializerVersion);
          flag = true;
        }
        else
          offlineData = serializer.Deserialize(reader);
      }
    }
    catch (EndOfStreamException ex)
    {
      Log.Offline.PrintError("ReadOfflineDataFromFile - Not all protos are represented. Is this a new cache file?");
      flag = true;
    }
    catch (ProtocolBufferException ex)
    {
      Log.Offline.PrintError("Error parsing cached protobufs from cache. Recreating cache file.");
      flag = true;
    }
    if (flag)
      OfflineDataCache.ClearLocalCacheFile();
    return offlineData;
  }

  public static void ClearLocalCacheFile()
  {
    OfflineDataCache.OfflineData data = new OfflineDataCache.OfflineData();
    Log.Offline.PrintDebug("OfflineDataCache: Clearing local cache file");
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  private static List<DeckCardData> GetDeckContentsDelta(
    PegasusUtil.DeckContents deckContentsLocal,
    PegasusUtil.DeckContents deckContentsOriginal)
  {
    List<DeckCardData> cards1 = deckContentsLocal.Cards;
    List<DeckCardData> cards2 = deckContentsOriginal.Cards;
    List<DeckCardData> deckContentsDelta = new List<DeckCardData>();
    foreach (PegasusShared.CardDef cardDef1 in new HashSet<PegasusShared.CardDef>(cards1.Union<DeckCardData>((IEnumerable<DeckCardData>) cards2).Except<DeckCardData>(cards1.Intersect<DeckCardData>((IEnumerable<DeckCardData>) cards2)).ToList<DeckCardData>().Select<DeckCardData, PegasusShared.CardDef>((Func<DeckCardData, PegasusShared.CardDef>) (c => c.Def))))
    {
      PegasusShared.CardDef cardDef = cardDef1;
      DeckCardData deckCardData1 = cards1.FirstOrDefault<DeckCardData>((Func<DeckCardData, bool>) (c => c.Def.Asset == cardDef.Asset && c.Def.Premium == cardDef.Premium));
      DeckCardData deckCardData2 = cards2.FirstOrDefault<DeckCardData>((Func<DeckCardData, bool>) (c => c.Def.Asset == cardDef.Asset && c.Def.Premium == cardDef.Premium));
      int num1 = deckCardData1 == null ? 0 : deckCardData1.Qty;
      int num2 = deckCardData2 == null ? 0 : deckCardData2.Qty;
      if (num1 != num2)
      {
        DeckCardData deckCardData3 = new DeckCardData()
        {
          Def = cardDef,
          Qty = num1
        };
        deckContentsDelta.Add(deckCardData3);
      }
    }
    return deckContentsDelta;
  }

  private static long GetAndRecordNextFakeId(List<long> usedIds, OfflineDataCache.OfflineData data)
  {
    if (usedIds == null)
      usedIds = new List<long>();
    while (usedIds.Contains((long) data.UniqueFakeDeckId))
      --data.UniqueFakeDeckId;
    usedIds.Add((long) data.UniqueFakeDeckId);
    return (long) data.UniqueFakeDeckId;
  }

  private static bool IsValidFakeId(long id) => id < 0L;

  public class OfflineData
  {
    public int UniqueFakeDeckId = -999;
    public List<long> FakeDeckIds;
    public List<DeckInfo> OriginalDeckList;
    public List<DeckInfo> LocalDeckList;
    public List<PegasusUtil.DeckContents> OriginalDeckContents;
    public List<PegasusUtil.DeckContents> LocalDeckContents;
    public bool m_hasChangedFavoriteHeroesOffline;
    public List<FavoriteHero> FavoriteHeroes;
    public bool m_hasChangedCardBacksOffline;
    public CardBacks CardBacks;
    public Collection Collection;
    public bool m_hasChangedCoinsOffline;
    public Coins Coins;
  }
}
