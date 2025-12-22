using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Commerce;
using Hearthstone.Core;
using Hearthstone.DataModels;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class CardBackManager : IService
{
  private GameObject m_sceneObject;
  private const int CARD_BACK_PRIMARY_MATERIAL_INDEX = 0;
  private const int CARD_BACK_SECONDARY_MATERIAL_INDEX = 1;
  private Map<int, CardBackData> m_cardBackData;
  private Map<string, CardBack> m_LoadedCardBacks;
  private Map<CardBackManager.CardBackSlot, CardBackManager.CardBackSlotData> m_LoadedCardBacksBySlot;
  private string m_searchText;
  private List<CardBackManager.UpdateCardbacksListener> m_updateCardbacksListeners = new List<CardBackManager.UpdateCardbacksListener>();
  private readonly object cardbackListenerCollectionLock = new object();
  private bool m_shouldSort = true;
  private List<CardBackManager.OwnedCardBack> m_sortedCardBacks;

  public event CardBackManager.FavoriteCardBacksChangedCallback OnFavoriteCardBacksChanged;

  private GameObject SceneObject
  {
    get
    {
      if ((UnityEngine.Object) this.m_sceneObject == (UnityEngine.Object) null)
        this.m_sceneObject = new GameObject("CardBackManagerSceneObject", new System.Type[1]
        {
          typeof (HSDontDestroyOnLoad)
        });
      return this.m_sceneObject;
    }
  }

  public int TheRandomCardBackID { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CardBackManager cardBackManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().Resetting += new Action(cardBackManager.Resetting);
    NetCache netCache = serviceLocator.Get<NetCache>();
    netCache.FavoriteCardBackChanged += new NetCache.DelFavoriteCardBackChangedListener(cardBackManager.OnFavoriteCardBackChanged);
    netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheCardBacks), new Action(cardBackManager.NetCache_OnNetCacheCardBacksUpdated));
    cardBackManager.InitCardBackData();
    Options.Get().RegisterChangedListener(Option.CARD_BACK, new Options.ChangedCallback(cardBackManager.OnCheatOptionChanged));
    Options.Get().RegisterChangedListener(Option.CARD_BACK2, new Options.ChangedCallback(cardBackManager.OnCheatOptionChanged));
    serviceLocator.Get<SceneMgr>().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(cardBackManager.OnSceneLoaded));
    cardBackManager.InitCardBackSlots();
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[4]
  {
    typeof (GameDbf),
    typeof (IAssetLoader),
    typeof (NetCache),
    typeof (SceneMgr)
  };

  public void Shutdown()
  {
    NetCache service;
    if (ServiceManager.TryGet<NetCache>(out service))
      service.FavoriteCardBackChanged -= new NetCache.DelFavoriteCardBackChangedListener(this.OnFavoriteCardBackChanged);
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null))
      return;
    hearthstoneApplication.Resetting -= new Action(this.Resetting);
  }

  private void Resetting() => this.InitCardBackData();

  public static CardBackManager Get() => ServiceManager.Get<CardBackManager>();

  public bool RegisterUpdateCardbacksListener(CardBackManager.UpdateCardbacksCallback callback)
  {
    CardBackManager.UpdateCardbacksListener cardbacksListener = new CardBackManager.UpdateCardbacksListener();
    cardbacksListener.SetCallback(callback);
    if (this.m_updateCardbacksListeners.Contains(cardbacksListener))
      return false;
    lock (this.cardbackListenerCollectionLock)
      this.m_updateCardbacksListeners.Add(cardbacksListener);
    return true;
  }

  public bool UnregisterUpdateCardbacksListener(CardBackManager.UpdateCardbacksCallback callback)
  {
    CardBackManager.UpdateCardbacksListener cardbacksListener = new CardBackManager.UpdateCardbacksListener();
    cardbacksListener.SetCallback(callback);
    lock (this.cardbackListenerCollectionLock)
      return this.m_updateCardbacksListeners.Remove(cardbacksListener);
  }

  public void SetSearchText(string searchText) => this.m_searchText = searchText?.ToLower();

  public CardBack GetFriendlyCardBack() => this.GetCardBackBySlot(CardBackManager.CardBackSlot.FRIENDLY);

  public CardBack GetOpponentCardBack() => this.GetCardBackBySlot(CardBackManager.CardBackSlot.OPPONENT);

  public CardBack GetCardBackForActor(Actor actor) => this.IsActorFriendly(actor) ? this.GetFriendlyCardBack() : this.GetOpponentCardBack();

  public CardBack GetCardBackBySlot(CardBackManager.CardBackSlot slot)
  {
    CardBackManager.CardBackSlotData cardBackSlotData;
    return this.m_LoadedCardBacksBySlot.TryGetValue(slot, out cardBackSlotData) ? cardBackSlotData.m_cardBack : (CardBack) null;
  }

  public bool IsCardBackLoading(CardBackManager.CardBackSlot slot)
  {
    CardBackManager.CardBackSlotData cardBackSlotData;
    return this.m_LoadedCardBacksBySlot.TryGetValue(slot, out cardBackSlotData) && cardBackSlotData.m_isLoading;
  }

  public void UpdateAllCardBacksInSceneWhenReady() => Processor.RunCoroutine(this.UpdateAllCardBacksInSceneWhenReadyImpl());

  public void SetGameCardBackIDs(int friendlyCardBackID, int opponentCardBackID)
  {
    this.LoadCardBackPrefabIntoSlot((AssetReference) this.m_cardBackData[this.GetValidCardBackID(friendlyCardBackID)].PrefabName, CardBackManager.CardBackSlot.FRIENDLY);
    this.LoadCardBackPrefabIntoSlot((AssetReference) this.m_cardBackData[this.GetValidCardBackID(opponentCardBackID)].PrefabName, CardBackManager.CardBackSlot.OPPONENT);
    this.UpdateAllCardBacksInSceneWhenReady();
  }

  public bool LoadCardBackByIndex(
    int cardBackIdx,
    CardBackManager.LoadCardBackData.LoadCardBackCallback callback,
    object callbackData = null)
  {
    string actorName = "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9";
    return this.LoadCardBackByIndex(cardBackIdx, callback, false, actorName, callbackData);
  }

  public bool LoadCardBackByIndex(
    int cardBackIdx,
    CardBackManager.LoadCardBackData.LoadCardBackCallback callback,
    string actorName,
    object callbackData = null)
  {
    return this.LoadCardBackByIndex(cardBackIdx, callback, false, actorName, callbackData);
  }

  public bool LoadCardBackByIndex(
    int cardBackIdx,
    CardBackManager.LoadCardBackData.LoadCardBackCallback callback,
    bool unlit,
    string actorName = "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9",
    object callbackData = null)
  {
    if (!this.m_cardBackData.ContainsKey(cardBackIdx))
    {
      Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - wrong cardBackIdx {0}", (object) cardBackIdx);
      return false;
    }
    AssetLoader.Get().InstantiatePrefab((AssetReference) actorName, new PrefabCallback<GameObject>(this.OnHiddenActorLoaded), (object) new CardBackManager.LoadCardBackData()
    {
      m_CardBackIndex = cardBackIdx,
      m_Callback = callback,
      m_Unlit = unlit,
      m_Name = this.m_cardBackData[cardBackIdx].Name,
      callbackData = callbackData
    }, AssetLoadingOptions.IgnorePrefabPosition);
    return true;
  }

  public CardBackManager.LoadCardBackData LoadCardBackByIndex(
    int cardBackIdx,
    bool unlit = false,
    string actorName = "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9",
    bool shadowActive = false)
  {
    if (!this.m_cardBackData.ContainsKey(cardBackIdx))
    {
      Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - wrong cardBackIdx {0}", (object) cardBackIdx);
      return (CardBackManager.LoadCardBackData) null;
    }
    CardBackManager.LoadCardBackData loadCardBackData = new CardBackManager.LoadCardBackData();
    loadCardBackData.m_CardBackIndex = cardBackIdx;
    loadCardBackData.m_Unlit = unlit;
    loadCardBackData.m_Name = this.m_cardBackData[cardBackIdx].Name;
    loadCardBackData.m_GameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) actorName, AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) loadCardBackData.m_GameObject == (UnityEngine.Object) null)
    {
      Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - failed to load Actor {0}", (object) actorName);
      return (CardBackManager.LoadCardBackData) null;
    }
    string prefabName = this.m_cardBackData[cardBackIdx].PrefabName;
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) prefabName);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - failed to load CardBack {0}", (object) prefabName);
      return (CardBackManager.LoadCardBackData) null;
    }
    CardBack componentInChildren = gameObject.GetComponentInChildren<CardBack>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogWarning((object) "CardBackManager.LoadCardBackByIndex() - cardback=null");
      return (CardBackManager.LoadCardBackData) null;
    }
    loadCardBackData.m_CardBack = componentInChildren;
    Actor component = loadCardBackData.m_GameObject.GetComponent<Actor>();
    CardBackManager.SetCardBack(component.m_cardMesh, loadCardBackData.m_CardBack, loadCardBackData.m_Unlit, shadowActive);
    component.SetCardbackUpdateIgnore(true);
    loadCardBackData.m_CardBack.gameObject.transform.parent = loadCardBackData.m_GameObject.transform;
    return loadCardBackData;
  }

  public static Actor LoadCardBackActorByPrefab(
    string cardBackPrefab,
    bool unlit = false,
    string actorName = "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9",
    bool shadowActive = false)
  {
    if (AssetLoader.Get() == null)
    {
      UnityEngine.Debug.LogWarning((object) "CardBackManager.LoadCardBackActorByPrefab() - AssetLoader not available");
      return (Actor) null;
    }
    GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab((AssetReference) cardBackPrefab);
    if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
    {
      Log.CardbackMgr.Print("CardBackManager.LoadCardBackActorByPrefab() - failed to load CardBack {0}", (object) cardBackPrefab);
      return (Actor) null;
    }
    GameObject gameObject2 = AssetLoader.Get().InstantiatePrefab((AssetReference) actorName, AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
    {
      Log.CardbackMgr.Print("CardBackManager.LoadCardBackActorByPrefab() - failed to load Actor {0}", (object) actorName);
      return (Actor) null;
    }
    Actor component = gameObject2.GetComponent<Actor>();
    CardBack componentInChildren = gameObject1.GetComponentInChildren<CardBack>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogWarning((object) "CardBackManager.LoadCardBackActorByPrefab() - cardback=null");
      return (Actor) null;
    }
    CardBackManager.SetCardBack(component.m_cardMesh, componentInChildren, unlit, shadowActive);
    component.SetCardbackUpdateIgnore(true);
    componentInChildren.gameObject.transform.parent = gameObject2.transform;
    return component;
  }

  public void AddNewCardBack(int cardBackID)
  {
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    if (cardBacks == null)
    {
      UnityEngine.Debug.LogWarning((object) string.Format("CollectionManager.AddNewCardBack({0}): trying to access NetCacheCardBacks before it's been loaded", (object) cardBackID));
    }
    else
    {
      cardBacks.CardBacks.Add(cardBackID);
      this.SetCollectionCardBackOwned(cardBackID);
    }
  }

  public void SetCollectionCardBackOwned(int cardBackId)
  {
    if (this.m_sortedCardBacks == null)
      return;
    CardBackManager.OwnedCardBack ownedCardBack = this.m_sortedCardBacks.Find((Predicate<CardBackManager.OwnedCardBack>) (back => back.m_cardBackId == cardBackId));
    if (ownedCardBack == null)
      return;
    ownedCardBack.m_owned = true;
  }

  public void HandleFavoriteToggle(int cardBackId)
  {
    if (this.MultipleFavoriteCardBacksEnabled())
    {
      this.RequestSetFavoriteCardBack(cardBackId, !this.IsCardBackFavorited(cardBackId));
    }
    else
    {
      foreach (int favoriteCardBack in this.GetCardBacks().FavoriteCardBacks)
        this.RequestSetFavoriteCardBack(favoriteCardBack, false);
      this.RequestSetFavoriteCardBack(cardBackId);
    }
  }

  public void RequestSetFavoriteCardBack(int cardBackID, bool isFavorite = true) => Network.Get().SetFavoriteCardBack(cardBackID, isFavorite);

  public string GetCardBackName(int cardBackId)
  {
    CardBackData cardBackData;
    return this.m_cardBackData.TryGetValue(cardBackId, out cardBackData) ? cardBackData.Name : (string) null;
  }

  public int GetNumCardBacksOwned()
  {
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    if (cardBacks != null)
      return cardBacks.CardBacks.Count;
    UnityEngine.Debug.LogWarning((object) "CardBackManager.GetNumCardBacksOwned(): trying to access NetCacheCardBacks before it's been loaded");
    return 0;
  }

  public HashSet<int> GetCardBacksOwned()
  {
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    if (cardBacks != null)
      return cardBacks.CardBacks;
    UnityEngine.Debug.LogWarning((object) "CardBackManager.GetCardBacksOwned(): trying to access NetCacheCardBacks before it's been loaded");
    return (HashSet<int>) null;
  }

  public NetCache.NetCacheCardBacks GetCardBacks() => NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>() ?? this.GetCardBacksFromOfflineData();

  public NetCache.NetCacheCardBacks GetCardBacksFromOfflineData()
  {
    CardBacks cardBacksFromCache = OfflineDataCache.GetCardBacksFromCache();
    if (cardBacksFromCache == null)
      return (NetCache.NetCacheCardBacks) null;
    return new NetCache.NetCacheCardBacks()
    {
      CardBacks = new HashSet<int>((IEnumerable<int>) cardBacksFromCache.CardBacks_),
      FavoriteCardBacks = new HashSet<int>((IEnumerable<int>) cardBacksFromCache.FavoriteCardBacks)
    };
  }

  public HashSet<int> GetCardBackIds(bool all = true)
  {
    HashSet<int> cardBackIds = new HashSet<int>();
    this.GetCardBacksOwned();
    foreach (KeyValuePair<int, CardBackData> keyValuePair in this.m_cardBackData)
    {
      if (this.ShouldIncludeCardBack(keyValuePair.Value, !all))
        cardBackIds.Add(keyValuePair.Key);
    }
    return cardBackIds;
  }

  public bool IsCardBackOwned(int cardBackID)
  {
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    if (cardBacks != null)
      return cardBacks.CardBacks.Contains(cardBackID);
    UnityEngine.Debug.LogWarning((object) string.Format("CardBackManager.IsCardBackOwned({0}): trying to access NetCacheCardBacks before it's been loaded", (object) cardBackID));
    return false;
  }

  public bool IsCardBackFavorited(int cardBackID)
  {
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    if (cardBacks != null)
      return cardBacks.FavoriteCardBacks.Contains(cardBackID);
    UnityEngine.Debug.LogWarning((object) string.Format("CardBackManager.IsCardBackFavorited({0}): trying to access NetCacheCardBacks before it's been loaded", (object) cardBackID));
    return false;
  }

  public int TotalFavoriteCardBacks()
  {
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    if (cardBacks != null)
      return cardBacks.FavoriteCardBacks.Count;
    UnityEngine.Debug.LogWarning((object) string.Format("CardBackManager.TotalFavoriteCardBacks(): trying to access NetCacheCardBacks before it's been loaded"));
    return 0;
  }

  public bool CanToggleFavoriteCardBack(int cardBackId)
  {
    bool flag1 = this.IsCardBackOwned(cardBackId);
    bool flag2 = this.IsCardBackFavorited(cardBackId);
    bool flag3 = this.TotalFavoriteCardBacks() > 1;
    return !this.MultipleFavoriteCardBacksEnabled() ? flag1 && !flag2 : flag1 && !flag2 | flag3;
  }

  public int GetCollectionManagerCardBackPurchaseProductId(int cardBackId)
  {
    CardBackDbfRecord record = GameDbf.CardBack.GetRecord(cardBackId);
    if (record != null)
      return record.CollectionManagerPurchaseProductId;
    UnityEngine.Debug.LogError((object) ("CardBackManager:GetCollectionManagerCardBackPurchaseProductId failed to find card back " + cardBackId.ToString() + " in the CardBack database"));
    return 0;
  }

  public bool CanBuyCardBackFromCollectionManager(int cardBackId) => !this.IsCardBackOwned(cardBackId) && this.IsCardBackPurchasableFromCollectionManager(cardBackId) && NetCache.Get().GetGoldBalance() >= this.GetCollectionManagerCardBackGoldCost(cardBackId);

  public bool IsCardBackPurchasableFromCollectionManager(int cardBackId)
  {
    if (!StoreManager.Get().IsOpen(false) || !StoreManager.Get().IsBuyCardBacksFromCollectionManagerEnabled() || this.GetCollectionManagerCardBackPurchaseProductId(cardBackId) <= 0)
      return false;
    if (this.GetCollectionManagerCardBackPriceDataModel(cardBackId) != null)
      return true;
    UnityEngine.Debug.LogError((object) ("CardBackManager:IsCardBackPurchasableFromCollectionManager failed to get the price data model for Card Back " + cardBackId.ToString()));
    return false;
  }

  public Network.Bundle GetCollectionManagerCardBackProductBundle(int cardBackId)
  {
    int purchaseProductId = this.GetCollectionManagerCardBackPurchaseProductId(cardBackId);
    if (!ProductId.IsValid((long) purchaseProductId))
      return (Network.Bundle) null;
    Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(ProductId.CreateFrom((long) purchaseProductId));
    if ((Record) fromPmtProductId == (Record) null)
    {
      UnityEngine.Debug.LogError((object) ("CardBackManager:GetCollectionManagerCardBackProductBundle: Did not find a bundle with pmtProductId " + purchaseProductId.ToString() + " for Card Back " + cardBackId.ToString()));
      return (Network.Bundle) null;
    }
    if (fromPmtProductId.Items.Any<Network.BundleItem>((Func<Network.BundleItem, bool>) (x => x.ItemType == ProductType.PRODUCT_TYPE_CARD_BACK && x.ProductData == cardBackId)))
      return fromPmtProductId;
    UnityEngine.Debug.LogError((object) ("CardBackManager:GetCollectionManagerCardBackProductBundle: Did not find any items with type PRODUCT_TYPE_CARD_BACK for bundle with pmtProductId " + purchaseProductId.ToString() + " for Card Back " + cardBackId.ToString()));
    return (Network.Bundle) null;
  }

  public PriceDataModel GetCollectionManagerCardBackPriceDataModel(int cardBackId)
  {
    Network.Bundle backProductBundle = this.GetCollectionManagerCardBackProductBundle(cardBackId);
    if ((Record) backProductBundle == (Record) null)
    {
      UnityEngine.Debug.LogError((object) ("CardBackManager:GetCollectionManagerCardBackPriceDataModel failed to get bundle for Card Back " + cardBackId.ToString()));
      return (PriceDataModel) null;
    }
    long? gtappGoldCost = backProductBundle.GtappGoldCost;
    if (!gtappGoldCost.HasValue)
    {
      UnityEngine.Debug.LogError((object) ("CardBackManager:GetCollectionManagerCardBackPriceDataModel bundle for Card Back " + cardBackId.ToString() + " has no GTAPP gold cost"));
      return (PriceDataModel) null;
    }
    PriceDataModel backPriceDataModel = new PriceDataModel();
    backPriceDataModel.Currency = CurrencyType.GOLD;
    gtappGoldCost = backProductBundle.GtappGoldCost;
    backPriceDataModel.Amount = (float) gtappGoldCost.Value;
    gtappGoldCost = backProductBundle.GtappGoldCost;
    backPriceDataModel.DisplayText = Mathf.RoundToInt((float) gtappGoldCost.Value).ToString();
    return backPriceDataModel;
  }

  private long GetCollectionManagerCardBackGoldCost(int cardBackId)
  {
    Network.Bundle backProductBundle = this.GetCollectionManagerCardBackProductBundle(cardBackId);
    if ((Record) backProductBundle == (Record) null)
    {
      UnityEngine.Debug.LogError((object) ("CardBackManager:GetCollectionManagerCardBackGoldCost called for a card back with no valid product bundle. Card Back Id = " + cardBackId.ToString()));
      return 0;
    }
    long? gtappGoldCost = backProductBundle.GtappGoldCost;
    if (!gtappGoldCost.HasValue)
    {
      UnityEngine.Debug.LogError((object) ("CardBackManager:GetCollectionManagerCardBackGoldCost called for a card back with no gold cost. Card Back Id = " + cardBackId.ToString()));
      return 0;
    }
    gtappGoldCost = backProductBundle.GtappGoldCost;
    return gtappGoldCost.Value;
  }

  public List<CardBackManager.OwnedCardBack> GetPageOfCardBacks(
    bool requireOwned,
    int currentPage)
  {
    int maxCardsPerPage = CollectiblePageDisplay.GetMaxCardsPerPage();
    return this.GetFilteredCardBacks(requireOwned).Skip<CardBackManager.OwnedCardBack>(maxCardsPerPage * (currentPage - 1)).Take<CardBackManager.OwnedCardBack>(maxCardsPerPage).ToList<CardBackManager.OwnedCardBack>();
  }

  public List<CardBackManager.OwnedCardBack> GetFilteredCardBacks(
    bool requireOwned)
  {
    return this.GetAllOrderedCardBacks().Where<CardBackManager.OwnedCardBack>((Func<CardBackManager.OwnedCardBack, bool>) (cardBack => this.ShouldIncludeCardBack(cardBack, requireOwned))).ToList<CardBackManager.OwnedCardBack>();
  }

  public List<CardBackManager.OwnedCardBack> GetAllOrderedCardBacks()
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get()?.GetCollectibleDisplay();
    bool flag = (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.ViewModeChangedListenerExists(new CollectibleDisplay.ViewModeChangedListener(this.OnSwitchViewMode));
    if (((this.m_shouldSort ? 0 : (this.m_sortedCardBacks != null ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      return this.m_sortedCardBacks;
    List<CardBackManager.OwnedCardBack> ownedCardBackList = new List<CardBackManager.OwnedCardBack>();
    foreach (CardBackData cardBackData in this.m_cardBackData.Values)
    {
      if (cardBackData.Enabled)
      {
        CardBackDbfRecord record = GameDbf.CardBack.GetRecord(cardBackData.ID);
        long num = -1;
        if (record.Source == Assets.CardBack.Source.SEASON)
          num = record.Data1;
        ownedCardBackList.Add(new CardBackManager.OwnedCardBack()
        {
          m_cardBackId = cardBackData.ID,
          m_name = cardBackData.Name,
          m_owned = this.IsCardBackOwned(cardBackData.ID),
          m_favorited = this.IsCardBackFavorited(cardBackData.ID),
          m_canBuy = this.CanBuyCardBackFromCollectionManager(cardBackData.ID),
          m_sortOrder = record.SortOrder,
          m_sortCategory = (int) record.SortCategory,
          m_seasonId = num
        });
      }
    }
    ownedCardBackList.Sort((Comparison<CardBackManager.OwnedCardBack>) ((lhs, rhs) =>
    {
      if (this.MultipleFavoriteCardBacksEnabled() && lhs.m_favorited != rhs.m_favorited)
        return !lhs.m_favorited ? 1 : -1;
      if (lhs.m_owned != rhs.m_owned)
        return !lhs.m_owned ? 1 : -1;
      if (lhs.m_canBuy != rhs.m_canBuy)
        return !lhs.m_canBuy ? 1 : -1;
      if (lhs.m_sortCategory != rhs.m_sortCategory)
        return lhs.m_sortCategory >= rhs.m_sortCategory ? 1 : -1;
      if (lhs.m_sortOrder != rhs.m_sortOrder)
        return lhs.m_sortOrder >= rhs.m_sortOrder ? 1 : -1;
      if (lhs.m_seasonId == rhs.m_seasonId)
        return Mathf.Clamp(lhs.m_cardBackId - rhs.m_cardBackId, -1, 1);
      return lhs.m_seasonId <= rhs.m_seasonId ? 1 : -1;
    }));
    this.m_sortedCardBacks = ownedCardBackList;
    this.SetShouldSort(false);
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && !flag)
      collectibleDisplay.OnViewModeChanged += new CollectibleDisplay.ViewModeChangedListener(this.OnSwitchViewMode);
    return this.m_sortedCardBacks;
  }

  private void OnSwitchViewMode(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse)
  {
    if (mode == CollectionUtils.ViewMode.CARD_BACKS)
      return;
    this.SetShouldSort(true);
  }

  public void SetShouldSort(bool shouldSort) => this.m_shouldSort = shouldSort;

  public void SetCardBackTexture(Renderer renderer, int matIdx, CardBackManager.CardBackSlot slot)
  {
    if (this.IsCardBackLoading(slot))
      Processor.RunCoroutine(this.SetTextureWhenLoaded(renderer, matIdx, slot));
    else
      this.SetTexture(renderer, matIdx, slot);
  }

  public void SetCardBackMaterial(Renderer renderer, int matIdx, CardBackManager.CardBackSlot slot)
  {
    if (this.IsCardBackLoading(slot))
      Processor.RunCoroutine(this.SetMaterialWhenLoaded(renderer, matIdx, slot));
    else
      this.SetMaterial(renderer, matIdx, slot);
  }

  public void UpdateCardBack(Actor actor, CardBack cardBack)
  {
    if ((UnityEngine.Object) actor.gameObject == (UnityEngine.Object) null || (UnityEngine.Object) actor.m_cardMesh == (UnityEngine.Object) null || (UnityEngine.Object) cardBack == (UnityEngine.Object) null)
      return;
    CardBackManager.SetCardBack(actor.m_cardMesh, cardBack);
  }

  public void UpdateCardBackWithInternalCardBack(Actor actor)
  {
    if ((UnityEngine.Object) actor.gameObject == (UnityEngine.Object) null || (UnityEngine.Object) actor.m_cardMesh == (UnityEngine.Object) null)
      return;
    CardBack componentInChildren = actor.gameObject.GetComponentInChildren<CardBack>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    CardBackManager.SetCardBack(actor.m_cardMesh, componentInChildren);
  }

  public void UpdateCardBack(GameObject go, CardBackManager.CardBackSlot slot)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    if (this.IsCardBackLoading(slot))
      Processor.RunCoroutine(this.SetCardBackWhenLoaded(go, slot));
    else
      this.SetCardBack(go, slot);
  }

  public void UpdateDeck(GameObject go, CardBackManager.CardBackSlot slot)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    Processor.RunCoroutine(this.SetDeckCardBackWhenLoaded(go, slot));
  }

  public void UpdateDragEffect(GameObject go, CardBackManager.CardBackSlot slot)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    if (this.IsCardBackLoading(slot))
      Processor.RunCoroutine(this.SetDragEffectsWhenLoaded(go, slot));
    else
      this.SetDragEffects(go, slot);
  }

  public bool IsActorFriendly(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Log.CardbackMgr.Print("CardBack IsActorFriendly: actor is null!");
      return true;
    }
    Entity entity = actor.GetEntity();
    if (entity != null)
    {
      Player controller = entity.GetController();
      if (controller != null && controller.GetSide() == Player.Side.OPPOSING)
        return false;
    }
    return true;
  }

  public int GetRandomCardBackIdOwnedByPlayer(bool shouldLimitToFavorites = false)
  {
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    if (cardBacks == null)
    {
      UnityEngine.Debug.LogWarning((object) string.Format("CardBackMaanager.GetRandomCardBackIdOwnedByPlayer({0}): trying to access NetCacheCardBacks before it's been loaded", (object) shouldLimitToFavorites));
      return 0;
    }
    HashSet<int> intSet = shouldLimitToFavorites ? cardBacks.FavoriteCardBacks : cardBacks.CardBacks;
    List<int> intList = new List<int>();
    foreach (int id in intSet)
    {
      CardBackDbfRecord record = GameDbf.CardBack.GetRecord(id);
      if (record.Enabled && !record.IsRandomCardBack)
        intList.Add(id);
    }
    int backIdOwnedByPlayer = 0;
    if (intList.Count > 0)
    {
      int index = UnityEngine.Random.Range(0, intList.Count);
      backIdOwnedByPlayer = intList[index];
    }
    return backIdOwnedByPlayer;
  }

  public void FindCardBackToUse(long deckId, out int cardBackToUse, out int? deckCardBack)
  {
    CollectionDeck deck = CollectionManager.Get()?.GetDeck(deckId);
    deckCardBack = (int?) deck?.CardBackID;
    if (deck == null)
    {
      bool shouldLimitToFavorites = !GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_RANDOM_CARD_BACK_USE_ALL_OWNED);
      cardBackToUse = this.GetRandomCardBackIdOwnedByPlayer(shouldLimitToFavorites);
    }
    else
      cardBackToUse = deckCardBack.HasValue ? deckCardBack.Value : this.GetRandomCardBackIdOwnedByPlayer(true);
    CardBackDbfRecord record = GameDbf.CardBack.GetRecord(cardBackToUse);
    if (!record.IsRandomCardBack || !record.Enabled)
      return;
    cardBackToUse = this.GetRandomCardBackIdOwnedByPlayer();
  }

  public void LoadRandomCardBackIntoFavoriteSlot(bool updateScene)
  {
    GameMgr service;
    if (ServiceManager.TryGet<GameMgr>(out service) && service.IsSpectator())
      return;
    this.LoadCardBackIdIntoSlot(this.GetRandomCardBackIdOwnedByPlayer(!GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_RANDOM_CARD_BACK_USE_ALL_OWNED)), CardBackManager.CardBackSlot.FAVORITE);
    if (!updateScene)
      return;
    this.UpdateAllCardBacksInSceneWhenReady();
  }

  public bool MultipleFavoriteCardBacksEnabled() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Collection.MultipleFavoriteCardBacks;

  private void InitCardBackSlots()
  {
    this.LoadCardBackPrefabIntoSlot((AssetReference) this.m_cardBackData[0].PrefabName, CardBackManager.CardBackSlot.DEFAULT);
    if (!Application.isEditor)
      return;
    if (Options.Get().HasOption(Option.CARD_BACK))
    {
      int key = Options.Get().GetInt(Option.CARD_BACK);
      if (this.m_cardBackData.ContainsKey(key))
        this.LoadCardBackPrefabIntoSlot((AssetReference) this.m_cardBackData[key].PrefabName, CardBackManager.CardBackSlot.FRIENDLY);
    }
    if (!Options.Get().HasOption(Option.CARD_BACK2))
      return;
    int key1 = Options.Get().GetInt(Option.CARD_BACK2);
    if (!this.m_cardBackData.ContainsKey(key1))
      return;
    this.LoadCardBackPrefabIntoSlot((AssetReference) this.m_cardBackData[key1].PrefabName, CardBackManager.CardBackSlot.OPPONENT);
  }

  public void InitCardBackData()
  {
    List<CardBackData> cardBackDataList = new List<CardBackData>();
    foreach (CardBackDbfRecord record in GameDbf.CardBack.GetRecords())
    {
      if (record.IsRandomCardBack)
        this.TheRandomCardBackID = record.ID;
      else
        cardBackDataList.Add(new CardBackData(record.ID, record.Source, record.Data1, (string) record.Name, record.Enabled, record.PrefabName));
    }
    this.m_cardBackData = new Map<int, CardBackData>();
    foreach (CardBackData cardBackData in cardBackDataList)
      this.m_cardBackData[cardBackData.ID] = cardBackData;
    this.m_LoadedCardBacks = new Map<string, CardBack>();
    this.m_LoadedCardBacksBySlot = new Map<CardBackManager.CardBackSlot, CardBackManager.CardBackSlotData>();
  }

  private IEnumerator SetTextureWhenLoaded(
    Renderer renderer,
    int matIdx,
    CardBackManager.CardBackSlot slot)
  {
    while (this.IsCardBackLoading(slot))
      yield return (object) null;
    this.SetTexture(renderer, matIdx, slot);
  }

  private void SetTexture(Renderer renderer, int matIdx, CardBackManager.CardBackSlot slot)
  {
    if ((UnityEngine.Object) renderer == (UnityEngine.Object) null)
      return;
    int count = RendererExtension.GetMaterials(renderer).Count;
    if (matIdx < 0 || matIdx >= count)
    {
      UnityEngine.Debug.LogWarningFormat("CardBackManager SetTexture(): matIdx {0} is not within the bounds of renderer's materials (count {1})", (object) matIdx, (object) count);
    }
    else
    {
      CardBack cardBackBySlot = this.GetCardBackBySlot(slot);
      if ((UnityEngine.Object) cardBackBySlot == (UnityEngine.Object) null)
        return;
      Texture cardBackTexture = (Texture) cardBackBySlot.m_CardBackTexture;
      if ((UnityEngine.Object) cardBackTexture == (UnityEngine.Object) null)
        UnityEngine.Debug.LogWarning((object) string.Format("CardBackManager SetTexture(): texture is null!   obj: {0}  slot: {1}", (object) renderer.gameObject.name, (object) slot));
      else
        RendererExtension.GetMaterial(renderer, matIdx).mainTexture = cardBackTexture;
    }
  }

  private IEnumerator SetMaterialWhenLoaded(
    Renderer renderer,
    int matIdx,
    CardBackManager.CardBackSlot slot)
  {
    while (this.IsCardBackLoading(slot))
      yield return (object) null;
    this.SetMaterial(renderer, matIdx, slot);
  }

  private void SetMaterial(Renderer renderer, int matIdx, CardBackManager.CardBackSlot slot)
  {
    if ((UnityEngine.Object) renderer == (UnityEngine.Object) null)
      return;
    int count = RendererExtension.GetMaterials(renderer).Count;
    if (matIdx < 0 || matIdx >= count)
    {
      UnityEngine.Debug.LogWarningFormat("CardBackManager SetMaterial(): matIdx {0} is not within the bounds of renderer's materials (count {1})", (object) matIdx, (object) count);
    }
    else
    {
      CardBack cardBackBySlot = this.GetCardBackBySlot(slot);
      if ((UnityEngine.Object) cardBackBySlot == (UnityEngine.Object) null)
        return;
      Material cardBackMaterial2D = cardBackBySlot.m_CardBackMaterial2D;
      if ((UnityEngine.Object) cardBackMaterial2D == (UnityEngine.Object) null)
        this.SetTexture(renderer, matIdx, slot);
      else
        RendererExtension.SetSharedMaterial(renderer, matIdx, cardBackMaterial2D);
    }
  }

  private IEnumerator SetCardBackWhenLoaded(
    GameObject go,
    CardBackManager.CardBackSlot slot)
  {
    while (this.IsCardBackLoading(slot))
      yield return (object) null;
    this.SetCardBack(go, slot);
  }

  private void SetCardBack(GameObject go, CardBackManager.CardBackSlot slot)
  {
    CardBack cardBackBySlot = this.GetCardBackBySlot(slot);
    if ((UnityEngine.Object) cardBackBySlot == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogWarningFormat("CardBackManager SetCardBack(): cardback not loaded for Slot: {0}", (object) slot);
      cardBackBySlot = this.GetCardBackBySlot(CardBackManager.CardBackSlot.DEFAULT);
      if ((UnityEngine.Object) cardBackBySlot == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarning((object) "CardBackManager SetCardBack(): default cardback not loaded");
        return;
      }
    }
    CardBackManager.SetCardBack(go, cardBackBySlot);
  }

  public static void SetCardBack(GameObject go, CardBack cardBack) => CardBackManager.SetCardBack(go, cardBack, false, false);

  public static void SetCardBack(GameObject go, CardBack cardBack, bool unlit, bool shadowActive)
  {
    if ((UnityEngine.Object) cardBack == (UnityEngine.Object) null)
      UnityEngine.Debug.LogWarning((object) "CardBackManager SetCardBack() cardback=null");
    else if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      StackTrace stackTrace = new StackTrace();
      UnityEngine.Debug.LogWarningFormat("CardBackManager SetCardBack() go=null, cardBack.name={0}, stacktrace=\n{1}", (object) cardBack.name, (object) stackTrace.ToString());
    }
    else
    {
      Mesh cardBackMesh = cardBack.m_CardBackMesh;
      if ((UnityEngine.Object) cardBackMesh != (UnityEngine.Object) null)
      {
        MeshFilter component = go.GetComponent<MeshFilter>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.mesh = cardBackMesh;
      }
      else
        UnityEngine.Debug.LogWarning((object) "CardBackManager SetCardBack() mesh=null");
      float num1 = 0.0f;
      if (!unlit && SceneMgr.Get() != null && SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
        num1 = 1f;
      Material cardBackMaterial = cardBack.m_CardBackMaterial;
      Material cardBackMaterial1 = cardBack.m_CardBackMaterial1;
      Material[] materialArray = new Material[(UnityEngine.Object) cardBackMaterial1 != (UnityEngine.Object) null ? 2 : 1];
      materialArray[0] = cardBackMaterial;
      if ((UnityEngine.Object) cardBackMaterial1 != (UnityEngine.Object) null)
        materialArray[1] = cardBackMaterial1;
      if (materialArray.Length != 0 && (UnityEngine.Object) materialArray[0] != (UnityEngine.Object) null)
      {
        Renderer component = go.GetComponent<Renderer>();
        RendererExtension.SetSharedMaterials(component, materialArray);
        List<Material> materials = RendererExtension.GetMaterials(component);
        float num2 = UnityEngine.Random.Range(0.0f, 1f);
        foreach (Material material in materials)
        {
          if (!((UnityEngine.Object) material == (UnityEngine.Object) null))
          {
            if (material.HasProperty("_Seed") && (double) material.GetFloat("_Seed") == 0.0)
              material.SetFloat("_Seed", num2);
            if (material.HasProperty("_LightingBlend"))
              material.SetFloat("_LightingBlend", num1);
          }
        }
      }
      else
        UnityEngine.Debug.LogWarning((object) "CardBackManager SetCardBack() material=null");
      if (cardBack.cardBackHelper == CardBack.cardBackHelpers.None)
        CardBackManager.RemoveCardBackHelper<CardBackHelperBubbleLevel>(go);
      else if (cardBack.cardBackHelper == CardBack.cardBackHelpers.CardBackHelperBubbleLevel)
        CardBackManager.AddCardBackHelper<CardBackHelperBubbleLevel>(go);
      Actor componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<Actor>(go);
      if (!((UnityEngine.Object) componentInThisOrParents != (UnityEngine.Object) null))
        return;
      componentInThisOrParents.UpdateMissingCardArt();
      componentInThisOrParents.EnableCardbackShadow(shadowActive);
      HighlightState componentInChildren = componentInThisOrParents.GetComponentInChildren<HighlightState>();
      if (!(bool) (UnityEngine.Object) componentInChildren)
        return;
      componentInChildren.m_StaticSilouetteOverride = cardBack.m_CardBackHighlightTexture;
    }
  }

  private bool ShouldIncludeCardBack(CardBackData cardBackData, bool requireOwned) => cardBackData.Enabled && this.ShouldIncludeCardBack(cardBackData.ID, cardBackData.Name, requireOwned);

  private bool ShouldIncludeCardBack(CardBackManager.OwnedCardBack ownedCardBack, bool requireOwned) => this.ShouldIncludeCardBack(ownedCardBack.m_cardBackId, ownedCardBack.m_name, requireOwned);

  private bool ShouldIncludeCardBack(int cardBackId, string cardBackName, bool requireOwned)
  {
    if (requireOwned && !this.IsCardBackOwned(cardBackId))
      return false;
    if (!string.IsNullOrEmpty(this.m_searchText))
    {
      string str1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_FAVORITE");
      string str2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING");
      string str3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EXTRA");
      string[] source1 = new string[3]{ str1, str2, str3 };
      string[] source2 = this.m_searchText.ToLower().Split(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries);
      if (((IEnumerable<string>) source2).Contains<string>(str3))
        return false;
      if (this.MultipleFavoriteCardBacksEnabled() && ((IEnumerable<string>) source2).Contains<string>(str1))
      {
        bool flag = this.GetCardBacks().FavoriteCardBacks.Count == 0;
        if ((this.IsCardBackFavorited(cardBackId) ? 1 : (!flag ? 0 : (cardBackId == 0 ? 1 : 0))) == 0)
          return false;
      }
      if (((IEnumerable<string>) source2).Contains<string>(str2) && this.IsCardBackOwned(cardBackId))
        return false;
      for (int index = 0; index < source2.Length; ++index)
      {
        string str4 = source2[index];
        if (!((IEnumerable<string>) source1).Contains<string>(str4) && !cardBackName.ToLower().Contains(str4))
          return false;
      }
    }
    return true;
  }

  public static T AddCardBackHelper<T>(GameObject go) where T : MonoBehaviour
  {
    CardBackManager.RemoveCardBackHelper<T>(go);
    return go.AddComponent<T>();
  }

  public static bool RemoveCardBackHelper<T>(GameObject go) where T : MonoBehaviour
  {
    T[] components = go.GetComponents<T>();
    if (components == null)
      return false;
    foreach (T obj in components)
      UnityEngine.Object.Destroy((UnityEngine.Object) obj);
    return true;
  }

  private IEnumerator SetDragEffectsWhenLoaded(
    GameObject go,
    CardBackManager.CardBackSlot slot)
  {
    while (this.IsCardBackLoading(slot))
      yield return (object) null;
    this.SetDragEffects(go, slot);
  }

  private void SetDragEffects(GameObject go, CardBackManager.CardBackSlot slot)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    CardBackDragEffect componentInChildren = go.GetComponentInChildren<CardBackDragEffect>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    CardBack cardBackBySlot = this.GetCardBackBySlot(slot);
    if ((UnityEngine.Object) cardBackBySlot == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) componentInChildren.m_EffectsRoot != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren.m_EffectsRoot);
    if ((UnityEngine.Object) cardBackBySlot.m_DragEffect == (UnityEngine.Object) null)
      return;
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(cardBackBySlot.m_DragEffect);
    componentInChildren.m_EffectsRoot = gameObject;
    gameObject.transform.parent = componentInChildren.gameObject.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.identity;
    gameObject.transform.localScale = Vector3.one;
  }

  private IEnumerator SetDeckCardBackWhenLoaded(
    GameObject cardBackDeckDisplay,
    CardBackManager.CardBackSlot slot)
  {
    while (this.IsCardBackLoading(slot))
      yield return (object) null;
    this.SetDeckCardBack(cardBackDeckDisplay, slot);
  }

  private void SetDeckCardBack(GameObject cardBackDeckDisplay, CardBackManager.CardBackSlot slot)
  {
    if ((UnityEngine.Object) cardBackDeckDisplay == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogWarning((object) "CardBackManager SetDeckCardBack(): cardBackDeckDisplay GameObject is null! GameObject could have been destroyed while card back was loading.");
    }
    else
    {
      CardBack cardBackBySlot = this.GetCardBackBySlot(slot);
      if ((UnityEngine.Object) cardBackBySlot == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarning((object) "CardBackManager SetDeckCardBack(): cardBack is null!");
      }
      else
      {
        ZoneDeck componentInParent = cardBackDeckDisplay.GetComponentInParent<ZoneDeck>();
        if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
        {
          CardBack.CustomDeckMeshes meshes;
          if (cardBackBySlot.GetCustomDeckMeshes(out meshes))
            componentInParent.UpdateToCustomDeckMeshes(meshes);
          else
            componentInParent.TryRestoreOriginalDeckMeshes();
        }
        Texture cardBackTexture = (Texture) cardBackBySlot.m_CardBackTexture;
        if ((UnityEngine.Object) cardBackTexture == (UnityEngine.Object) null)
        {
          UnityEngine.Debug.LogWarning((object) "CardBackManager SetDeckCardBack(): texture is null!");
        }
        else
        {
          foreach (Renderer componentsInChild in cardBackDeckDisplay.GetComponentsInChildren<Renderer>())
            RendererExtension.GetMaterial(componentsInChild).mainTexture = cardBackTexture;
        }
      }
    }
  }

  private void OnCheatOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    Log.CardbackMgr.Print("Cheat Option Change Called");
    int key = Options.Get().GetInt(option, 0);
    if (!this.m_cardBackData.ContainsKey(key))
      return;
    CardBackManager.CardBackSlot slot = CardBackManager.CardBackSlot.FRIENDLY;
    if (option == Option.CARD_BACK2)
      slot = CardBackManager.CardBackSlot.OPPONENT;
    this.LoadCardBackPrefabIntoSlot((AssetReference) this.m_cardBackData[key].PrefabName, slot);
    this.UpdateAllCardBacksInSceneWhenReady();
  }

  private void NetCache_OnNetCacheCardBacksUpdated() => Processor.RunCoroutine(this.HandleNetCacheCardBacksWhenReady());

  private IEnumerator HandleNetCacheCardBacksWhenReady()
  {
    while (this.m_cardBackData == null || FixedRewardsMgr.Get() == null || !FixedRewardsMgr.Get().IsStartupFinished())
      yield return (object) null;
    NetCache.NetCacheCardBacks cardBacks = this.GetCardBacks();
    this.AddNewCardBack(0);
    bool flag = false;
    foreach (int favoriteCardBack in cardBacks.FavoriteCardBacks)
    {
      if (this.m_cardBackData.ContainsKey(favoriteCardBack))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
    {
      Log.CardbackMgr.Print("No valid favorite card backs found, set to CardBackDbId.CLASSIC");
      cardBacks.FavoriteCardBacks = new HashSet<int>() { 0 };
    }
    this.LoadRandomCardBackIntoFavoriteSlot(false);
  }

  private IEnumerator UpdateAllCardBacksInSceneWhenReadyImpl()
  {
    while (this.IsCardBackLoading(CardBackManager.CardBackSlot.FRIENDLY) || this.IsCardBackLoading(CardBackManager.CardBackSlot.OPPONENT) || this.IsCardBackLoading(CardBackManager.CardBackSlot.FAVORITE))
      yield return (object) null;
    lock (this.cardbackListenerCollectionLock)
    {
      foreach (CardBackManager.UpdateCardbacksListener cardbacksListener in this.m_updateCardbacksListeners)
        cardbacksListener.Fire();
    }
  }

  private void LoadCardBackPrefabIntoSlot(
    AssetReference assetRef,
    CardBackManager.CardBackSlot slot)
  {
    string str = assetRef.ToString();
    CardBackManager.CardBackSlotData cardBackSlotData;
    if (!this.m_LoadedCardBacksBySlot.TryGetValue(slot, out cardBackSlotData))
    {
      cardBackSlotData = new CardBackManager.CardBackSlotData();
      this.m_LoadedCardBacksBySlot[slot] = cardBackSlotData;
    }
    if (this.m_LoadedCardBacks.ContainsKey(str))
    {
      if ((UnityEngine.Object) this.m_LoadedCardBacks[str] == (UnityEngine.Object) null)
      {
        this.m_LoadedCardBacks.Remove(str);
      }
      else
      {
        cardBackSlotData.m_isLoading = false;
        cardBackSlotData.m_cardBackAssetString = str;
        cardBackSlotData.m_cardBack = this.m_LoadedCardBacks[str];
        return;
      }
    }
    if (cardBackSlotData.m_cardBackAssetString == str)
      return;
    cardBackSlotData.m_isLoading = true;
    cardBackSlotData.m_cardBackAssetString = str;
    cardBackSlotData.m_cardBack = (CardBack) null;
    AssetLoader.Get().InstantiatePrefab((AssetReference) str, new PrefabCallback<GameObject>(this.OnCardBackLoaded), (object) new CardBackManager.LoadCardBackData()
    {
      m_Slot = slot,
      m_Path = str
    });
  }

  private void OnCardBackLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    CardBackManager.LoadCardBackData loadCardBackData = callbackData as CardBackManager.LoadCardBackData;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogWarningFormat("CardBackManager OnCardBackLoaded(): Failed to load CardBack: {0} For: {1}", (object) assetRef, (object) loadCardBackData.m_Slot);
      this.m_LoadedCardBacksBySlot.Remove(loadCardBackData.m_Slot);
    }
    else
    {
      go.transform.parent = this.SceneObject.transform;
      go.transform.position = new Vector3(1000f, -1000f, -1000f);
      CardBack component = go.GetComponent<CardBack>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        UnityEngine.Debug.LogWarningFormat("CardBackManager OnCardBackLoaded(): Failed to find CardBack component: {0} slot: {1}", (object) loadCardBackData.m_Path, (object) loadCardBackData.m_Slot);
      else if ((UnityEngine.Object) component.m_CardBackMesh == (UnityEngine.Object) null)
        UnityEngine.Debug.LogWarningFormat("CardBackManager OnCardBackLoaded(): cardBack.m_CardBackMesh in null! - {0}", (object) loadCardBackData.m_Path);
      else if ((UnityEngine.Object) component.m_CardBackMaterial == (UnityEngine.Object) null)
        UnityEngine.Debug.LogWarningFormat("CardBackManager OnCardBackLoaded(): cardBack.m_CardBackMaterial in null! - {0}", (object) loadCardBackData.m_Path);
      else if ((UnityEngine.Object) component.m_CardBackTexture == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarningFormat("CardBackManager OnCardBackLoaded(): cardBack.m_CardBackTexture in null! - {0}", (object) loadCardBackData.m_Path);
      }
      else
      {
        this.m_LoadedCardBacks[loadCardBackData.m_Path] = component;
        CardBackManager.CardBackSlotData cardBackSlotData;
        if (!this.m_LoadedCardBacksBySlot.TryGetValue(loadCardBackData.m_Slot, out cardBackSlotData))
          return;
        cardBackSlotData.m_isLoading = false;
        cardBackSlotData.m_cardBack = component;
      }
    }
  }

  private void OnHiddenActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    CardBackManager.LoadCardBackData loadCardBackData = (CardBackManager.LoadCardBackData) callbackData;
    string prefabName = this.m_cardBackData[loadCardBackData.m_CardBackIndex].PrefabName;
    loadCardBackData.m_GameObject = go;
    AssetLoader.Get().InstantiatePrefab((AssetReference) prefabName, new PrefabCallback<GameObject>(this.OnHiddenActorCardBackLoaded), callbackData);
  }

  private void OnHiddenActorCardBackLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("Error", "CardBackManager OnHiddenActorCardBackLoaded() path={0}, gameobject=null", (object) assetRef);
    }
    else
    {
      CardBack componentInChildren = go.GetComponentInChildren<CardBack>();
      if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarningFormat("CardBackManager OnHiddenActorCardBackLoaded() path={0}, gameobject={1}, cardback=null", (object) assetRef, (object) go.name);
      }
      else
      {
        CardBackManager.LoadCardBackData data = (CardBackManager.LoadCardBackData) callbackData;
        data.m_CardBack = componentInChildren;
        Processor.RunCoroutine(this.HiddenActorCardBackLoadedSetup(data));
      }
    }
  }

  private IEnumerator HiddenActorCardBackLoadedSetup(
    CardBackManager.LoadCardBackData data)
  {
    yield return (object) null;
    yield return (object) null;
    if (data != null && !((UnityEngine.Object) data.m_GameObject == (UnityEngine.Object) null))
    {
      CardBackManager.SetCardBack(data.m_GameObject.GetComponent<Actor>().m_cardMesh, data.m_CardBack, data.m_Unlit, data.m_ShadowActive);
      data.m_CardBack.gameObject.transform.parent = data.m_GameObject.transform;
      data.m_Callback(data);
    }
  }

  private int GetValidCardBackID(int cardBackID)
  {
    if (this.m_cardBackData.ContainsKey(cardBackID))
      return cardBackID;
    Log.CardbackMgr.Print("Cardback ID {0} not found, defaulting to Classic", (object) cardBackID);
    return 0;
  }

  public void OnFavoriteCardBackChanged(int newFavoriteCardBackID, bool isFavorite)
  {
    this.LoadRandomCardBackIntoFavoriteSlot(false);
    CardBackManager.FavoriteCardBacksChangedCallback cardBacksChanged = this.OnFavoriteCardBacksChanged;
    if (cardBacksChanged == null)
      return;
    cardBacksChanged(newFavoriteCardBackID, isFavorite);
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (!((UnityEngine.Object) this.GetCardBackBySlot(CardBackManager.CardBackSlot.FRIENDLY) == (UnityEngine.Object) null))
      return;
    this.LoadCardBackIdIntoSlot(0, CardBackManager.CardBackSlot.FRIENDLY);
  }

  private void LoadCardBackIdIntoSlot(int cardBackId, CardBackManager.CardBackSlot slot)
  {
    CardBackData cardBackData;
    if (!this.m_cardBackData.TryGetValue(this.GetValidCardBackID(cardBackId), out cardBackData))
      return;
    this.LoadCardBackPrefabIntoSlot((AssetReference) cardBackData.PrefabName, slot);
  }

  public class LoadCardBackData
  {
    public int m_CardBackIndex;
    public GameObject m_GameObject;
    public CardBack m_CardBack;
    public CardBackManager.LoadCardBackData.LoadCardBackCallback m_Callback;
    public string m_Name;
    public string m_Path;
    public CardBackManager.CardBackSlot m_Slot;
    public bool m_Unlit;
    public bool m_ShadowActive;
    public object callbackData;

    public delegate void LoadCardBackCallback(CardBackManager.LoadCardBackData cardBackData);
  }

  public class OwnedCardBack
  {
    public int m_cardBackId;
    public string m_name;
    public bool m_owned;
    public bool m_favorited;
    public bool m_canBuy;
    public int m_sortOrder;
    public int m_sortCategory;
    public long m_seasonId = -1;
  }

  public enum CardBackSlot
  {
    DEFAULT,
    FRIENDLY,
    OPPONENT,
    FAVORITE,
  }

  public delegate void UpdateCardbacksCallback();

  public delegate void FavoriteCardBacksChangedCallback(int cardBackId, bool isFavorite);

  private class CardBackSlotData
  {
    public CardBack m_cardBack;
    public string m_cardBackAssetString;
    public bool m_isLoading;
  }

  private class UpdateCardbacksListener : EventListener<CardBackManager.UpdateCardbacksCallback>
  {
    public void Fire() => this.m_callback();
  }
}
