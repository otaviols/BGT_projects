using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinManager : IService
{
  private List<CollectibleCard> m_coinCards = new List<CollectibleCard>();
  private Map<int, int> m_cardIdCoinIdMap = new Map<int, int>();
  private string m_searchText;
  private bool m_shouldSort = true;
  private List<CollectibleCard> m_sortedCoinCards;
  public static readonly AssetReference COIN_PREVIEW_PREFAB = new AssetReference("CoinPreview.prefab:4c9e68cbb43064f4287a44286773f026");

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (NetCache),
    typeof (SceneMgr)
  };

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CoinManager coinManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().Resetting += new Action(coinManager.Resetting);
    NetCache netCache = serviceLocator.Get<NetCache>();
    netCache.FavoriteCoinChanged += new NetCache.DelFavoriteCoinChangedListener(coinManager.OnFavoriteCoinChanged);
    netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheCoins), new Action(coinManager.NetCache_OnNetCacheCoinsUpdated));
    serviceLocator.Get<Network>().RegisterNetHandler((object) CoinUpdate.PacketID.ID, new Network.NetHandler(coinManager.ReceiveCoinUpdateMessage));
    coinManager.InitCoinData();
    return false;
  }

  public void Shutdown()
  {
    NetCache service;
    if (ServiceManager.TryGet<NetCache>(out service))
      service.FavoriteCoinChanged -= new NetCache.DelFavoriteCoinChangedListener(this.OnFavoriteCoinChanged);
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null))
      return;
    hearthstoneApplication.Resetting -= new Action(this.Resetting);
  }

  private void Resetting() => this.InitCoinData();

  public static CoinManager Get() => ServiceManager.Get<CoinManager>();

  private void InitCoinData() => Processor.RunCoroutine(this.InitCoinDataWhenReady());

  private bool ShouldIncludeCoin(CollectibleCard coinCard)
  {
    if (string.IsNullOrEmpty(this.m_searchText))
      return true;
    int id;
    if (!this.m_cardIdCoinIdMap.TryGetValue(coinCard.CardDbId, out id))
    {
      Log.CoinManager.PrintWarning("ShouldIncludeCoin: Coin id for card not found.");
      return false;
    }
    CoinDbfRecord record = GameDbf.Coin.GetRecord(id);
    if (record == null)
    {
      Log.CoinManager.PrintWarning("ShouldIncludeCoin: Coin record not found.");
      return false;
    }
    string str1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_FAVORITE");
    string str2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING");
    string str3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EXTRA");
    string[] source1 = new string[3]{ str1, str2, str3 };
    string[] source2 = this.m_searchText.ToLower().Split(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries);
    if (((IEnumerable<string>) source2).Contains<string>(str3))
      return false;
    if (CardBackManager.Get().MultipleFavoriteCardBacksEnabled() && ((IEnumerable<string>) source2).Contains<string>(str1))
    {
      int favoriteCoinId = this.GetFavoriteCoinId();
      bool flag = favoriteCoinId == -1;
      if ((favoriteCoinId == id ? 1 : (!flag ? 0 : (id == 1 ? 1 : 0))) == 0)
        return false;
    }
    if (((IEnumerable<string>) source2).Contains<string>(str2) && this.IsCoinCardOwned(coinCard.CardId))
      return false;
    for (int index = 0; index < source2.Length; ++index)
    {
      string str4 = source2[index];
      if (!((IEnumerable<string>) source1).Contains<string>(str4) && !record.Name.GetString().ToLower().Contains(str4))
        return false;
    }
    return true;
  }

  private IEnumerator InitCoinDataWhenReady()
  {
    DefLoader defLoader = DefLoader.Get();
    while (!defLoader.HasLoadedEntityDefs())
      yield return (object) null;
    NetCache.NetCacheCoins coins = this.GetCoins();
    if (coins != null)
    {
      this.AddNewCoin(1);
      if (coins.FavoriteCoin == 0)
        coins.FavoriteCoin = 1;
      this.m_coinCards.Clear();
      this.m_cardIdCoinIdMap.Clear();
      foreach (CoinDbfRecord record in GameDbf.Coin.GetRecords())
      {
        CardDbfRecord cardRecord = record.CardRecord;
        EntityDef entityDef = defLoader.GetEntityDef(cardRecord.NoteMiniGuid);
        CollectibleCard collectibleCard = new CollectibleCard(cardRecord, entityDef, TAG_PREMIUM.NORMAL);
        this.m_coinCards.Add(collectibleCard);
        this.m_cardIdCoinIdMap.Add(collectibleCard.CardDbId, record.ID);
      }
      this.UpdateCoinCards();
    }
  }

  public int GetCoinCount() => this.m_coinCards.Count;

  public int GetCoinPageCount(int coinsPerPage) => Mathf.CeilToInt((float) this.GetCoinCount() / (float) coinsPerPage);

  public List<CollectibleCard> GetPageOfCoinCards(
    int currentPage,
    int coinsPerPage)
  {
    int coinPageCount = this.GetCoinPageCount(coinsPerPage);
    currentPage = Mathf.Min(currentPage, coinPageCount);
    return this.GetFilteredCoins().Skip<CollectibleCard>(coinsPerPage * (currentPage - 1)).Take<CollectibleCard>(coinsPerPage).ToList<CollectibleCard>();
  }

  public List<CollectibleCard> GetFilteredCoins() => this.GetOrderedCoinCards().Where<CollectibleCard>((Func<CollectibleCard, bool>) (cardBack => this.ShouldIncludeCoin(cardBack))).ToList<CollectibleCard>();

  public List<CollectibleCard> GetOrderedCoinCards()
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get()?.GetCollectibleDisplay();
    bool flag1 = (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.ViewModeChangedListenerExists(new CollectibleDisplay.ViewModeChangedListener(this.OnSwitchViewMode));
    if (((this.m_shouldSort ? 0 : (this.m_sortedCoinCards != null ? 1 : 0)) & (flag1 ? 1 : 0)) != 0)
      return this.m_sortedCoinCards;
    List<CollectibleCard> collectibleCardList = new List<CollectibleCard>((IEnumerable<CollectibleCard>) this.m_coinCards);
    this.GetCoinsOwned();
    collectibleCardList.Sort((Comparison<CollectibleCard>) ((lhs, rhs) =>
    {
      if (CardBackManager.Get().MultipleFavoriteCardBacksEnabled())
      {
        bool flag2 = this.IsCoinCardFavorited(lhs.CardId);
        bool flag3 = this.IsCoinCardFavorited(rhs.CardId);
        if (flag2 != flag3)
          return !flag2 ? 1 : -1;
      }
      bool flag4 = this.IsCoinCardOwned(lhs.CardId);
      bool flag5 = this.IsCoinCardOwned(rhs.CardId);
      if (flag4 != flag5)
        return !flag4 ? 1 : -1;
      CardSetDbfRecord cardSet1 = GameDbf.GetIndex().GetCardSet(lhs.Set);
      CardSetDbfRecord cardSet2 = GameDbf.GetIndex().GetCardSet(rhs.Set);
      if (cardSet1 != null && cardSet2 != null)
      {
        int num = cardSet1.ReleaseOrder.CompareTo(cardSet2.ReleaseOrder);
        return num != 0 ? cardSet1.ReleaseOrder.CompareTo(cardSet2.ReleaseOrder) : num;
      }
      int num1;
      this.m_cardIdCoinIdMap.TryGetValue(lhs.CardDbId, out num1);
      int num2;
      this.m_cardIdCoinIdMap.TryGetValue(rhs.CardDbId, out num2);
      return num1.CompareTo(num2);
    }));
    this.m_sortedCoinCards = collectibleCardList;
    this.SetShouldSort(false);
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && !flag1)
      collectibleDisplay.OnViewModeChanged += new CollectibleDisplay.ViewModeChangedListener(this.OnSwitchViewMode);
    return this.m_sortedCoinCards;
  }

  private void OnSwitchViewMode(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse)
  {
    if (mode == CollectionUtils.ViewMode.COINS)
      return;
    this.SetShouldSort(true);
  }

  public void SetShouldSort(bool shouldSort) => this.m_shouldSort = shouldSort;

  public int GetFavoriteCoinId()
  {
    NetCache.NetCacheCoins coins = this.GetCoins();
    return coins == null ? 1 : coins.FavoriteCoin;
  }

  public string GetFavoriteCoinCardId()
  {
    int favoriteCoinId = this.GetFavoriteCoinId();
    foreach (CollectibleCard coinCard in this.m_coinCards)
    {
      if (this.m_cardIdCoinIdMap[coinCard.CardDbId] == favoriteCoinId)
        return coinCard.CardId;
    }
    Log.CoinManager.PrintWarning("GetFavoriteCoinCardId(): Favorite coin's card could not be found.");
    return "GAME_005";
  }

  public void UpdateCoinCards()
  {
    HashSet<int> coinsOwned = this.GetCoinsOwned();
    if (coinsOwned == null)
      return;
    foreach (CollectibleCard coinCard in this.m_coinCards)
    {
      int cardIdCoinId = this.m_cardIdCoinIdMap[coinCard.CardDbId];
      coinCard.OwnedCount = coinsOwned.Contains(cardIdCoinId) ? 1 : 0;
    }
  }

  public void AddNewCoin(int coinId)
  {
    NetCache.NetCacheCoins coins = this.GetCoins();
    if (coins == null)
      Log.CoinManager.PrintWarning(string.Format("AddNewCoin({0}): trying to access NetCacheCoins before it's been loaded", (object) coinId));
    else
      coins.Coins.Add(coinId);
  }

  public void RequestSetFavoriteCoin(int newFavoriteCoinID)
  {
    OfflineDataCache.OfflineData data = OfflineDataCache.ReadOfflineDataFromFile();
    Network.Get().SetFavoriteCoin(ref data, newFavoriteCoinID);
    OfflineDataCache.WriteOfflineDataToFile(data);
  }

  public void OnFavoriteCoinChanged(int newFavoriteCoinID) => Log.CoinManager.Print(string.Format("CoinManager - Favorite Coin Changed" + string.Format(" ID: {0}", (object) newFavoriteCoinID)));

  public bool IsCoinCardFavorited(string coinCardId) => this.GetFavoriteCoinCardId() == coinCardId;

  private void NetCache_OnNetCacheCoinsUpdated() => this.InitCoinData();

  private void ReceiveCoinUpdateMessage()
  {
    CoinUpdate coinUpdate = Network.Get().GetCoinUpdate();
    if (coinUpdate == null)
      return;
    NetCache.NetCacheCoins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCoins>();
    if (netObject == null)
      return;
    foreach (int num in coinUpdate.AddCoinId)
    {
      netObject.Coins.Add(num);
      Log.CoinManager.Print(string.Format(string.Format("CoinManager - Coin added. ID: {0}", (object) num)));
    }
    foreach (int num in coinUpdate.RemoveCoinId)
    {
      netObject.Coins.Remove(num);
      Log.CoinManager.Print(string.Format(string.Format("CoinManager - Coin removed. ID: {0}", (object) num)));
    }
    if (coinUpdate.HasFavoriteCoinId)
    {
      netObject.FavoriteCoin = coinUpdate.FavoriteCoinId;
      Log.CoinManager.Print(string.Format("CoinManager - Coin Favorite Set. " + string.Format("ID: {0}", (object) coinUpdate.FavoriteCoinId)));
    }
    this.UpdateCoinCards();
  }

  private NetCache.NetCacheCoins GetCoinsFromOfflineData()
  {
    Coins coinsFromCache = OfflineDataCache.GetCoinsFromCache();
    if (coinsFromCache == null)
      return (NetCache.NetCacheCoins) null;
    return new NetCache.NetCacheCoins()
    {
      Coins = new HashSet<int>((IEnumerable<int>) coinsFromCache.Coins_),
      FavoriteCoin = coinsFromCache.FavoriteCoin
    };
  }

  public NetCache.NetCacheCoins GetCoins() => NetCache.Get().GetNetObject<NetCache.NetCacheCoins>() ?? this.GetCoinsFromOfflineData();

  public HashSet<int> GetCoinsOwned()
  {
    NetCache.NetCacheCoins coins = this.GetCoins();
    if (coins != null)
      return coins.Coins;
    Log.CoinManager.PrintWarning("GetCoinsOwned: Trying to access NetCacheCoins before it's been loaded");
    return (HashSet<int>) null;
  }

  public bool IsCoinCardOwned(string cardId)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId);
    if (cardRecord == null)
    {
      Log.CoinManager.PrintWarning("IsCoinCardOwned: Card record not found.");
      return false;
    }
    int num;
    if (!this.m_cardIdCoinIdMap.TryGetValue(cardRecord.ID, out num))
    {
      Log.CoinManager.PrintWarning("IsCoinCardOwned: Coin id for card not found.");
      return false;
    }
    HashSet<int> coinsOwned = this.GetCoinsOwned();
    // ISSUE: explicit non-virtual call
    return coinsOwned != null && __nonvirtual (coinsOwned.Contains(num));
  }

  public void ShowCoinPreview(string cardId, Transform startTransform)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId);
    if (cardRecord == null)
    {
      Log.CoinManager.PrintWarning("ShowCoinPreview: Card record not found.");
    }
    else
    {
      int coinId;
      if (!this.m_cardIdCoinIdMap.TryGetValue(cardRecord.ID, out coinId))
      {
        Log.CoinManager.PrintWarning("ShowCoinPreview: Coin id for card not found.");
      }
      else
      {
        CoinDbfRecord coinRecord = GameDbf.Coin.GetRecord(coinId);
        if (coinRecord == null)
        {
          Log.CoinManager.PrintWarning("ShowCoinPreview: Coin record not found.");
        }
        else
        {
          Widget widget = (Widget) WidgetInstance.Create((string) CoinManager.COIN_PREVIEW_PREFAB);
          widget.RegisterReadyListener((Action<object>) (_ =>
          {
            CoinPreview componentInChildren = widget.GetComponentInChildren<CoinPreview>();
            CardDataModel cardDataModel1 = new CardDataModel();
            cardDataModel1.CardId = cardRecord.NoteMiniGuid;
            cardDataModel1.Name = (string) coinRecord.Name;
            cardDataModel1.FlavorText = (string) cardRecord.FlavorText;
            cardDataModel1.Premium = TAG_PREMIUM.NORMAL;
            CardDataModel cardDataModel2 = cardDataModel1;
            string str;
            if (!string.IsNullOrWhiteSpace(coinRecord.CardRecord.ArtistName))
              str = GameStrings.Format("GLUE_COLLECTION_ARTIST", (object) coinRecord.CardRecord.ArtistName);
            else
              str = string.Empty;
            cardDataModel2.ArtistCredit = str;
            CardDataModel cardDataModel3 = cardDataModel1;
            int coinId1 = coinId;
            Transform startTransform1 = startTransform;
            componentInChildren.Initialize(cardDataModel3, coinId1, startTransform1);
          }), (object) null, true);
        }
      }
    }
  }

  public void SetSearchText(string searchText) => this.m_searchText = searchText?.ToLower();
}
