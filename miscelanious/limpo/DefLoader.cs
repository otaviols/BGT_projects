using Blizzard.T5.AssetManager;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DefLoader
{
  private static DefLoader s_instance;
  private bool m_loadedEntityDefs;
  private Dictionary<string, EntityDef> m_entityDefCache = new Dictionary<string, EntityDef>();
  private bool m_isPlaying;

  public static DefLoader Get()
  {
    if (DefLoader.s_instance != null && DefLoader.s_instance.m_isPlaying != Application.isPlaying)
      DefLoader.s_instance = (DefLoader) null;
    if (DefLoader.s_instance == null)
    {
      DefLoader.s_instance = new DefLoader();
      DefLoader.s_instance.m_isPlaying = Application.isPlaying;
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
        hearthstoneApplication.WillReset += new Action(DefLoader.s_instance.WillReset);
      else if (Application.isPlaying)
        Log.All.PrintWarning("DefLoader being initialized before HearthstoneApplication is initialized! This is very bad if you're running the game!");
    }
    return DefLoader.s_instance;
  }

  public void Initialize() => this.LoadAllEntityDefs();

  public void Clear() => this.ClearEntityDefs();

  public Dictionary<string, EntityDef> GetAllEntityDefs() => this.m_entityDefCache;

  public void ClearEntityDefs()
  {
    this.m_entityDefCache.Clear();
    this.m_loadedEntityDefs = false;
  }

  public EntityDef GetEntityDef(string cardId)
  {
    if (string.IsNullOrEmpty(cardId))
      return (EntityDef) null;
    if (cardId.Equals("None"))
      return (EntityDef) null;
    EntityDef entityDef1 = (EntityDef) null;
    this.m_entityDefCache.TryGetValue(cardId, out entityDef1);
    if (entityDef1 == null)
    {
      if (HearthstoneApplication.UseDevWorkarounds())
      {
        Debug.LogErrorFormat("DefLoader.GetEntityDef() - Failed to load {0}. Loading {1} instead.", (object) cardId, (object) "PlaceholderCard");
        EntityDef entityDef2;
        this.m_entityDefCache.TryGetValue("PlaceholderCard", out entityDef2);
        if (entityDef2 == null)
        {
          Error.AddDevFatal("DefLoader.GetEntityDef() - Failed to load {0} in place of {1}", (object) "PlaceholderCard", (object) cardId);
          return (EntityDef) null;
        }
        entityDef1 = entityDef2.Clone();
        entityDef1.SetCardId(cardId);
        this.m_entityDefCache[cardId] = entityDef1;
      }
      else
        Error.AddDevFatal("DefLoader.GetEntityDef() - Failed to load {0}", (object) cardId);
    }
    return entityDef1;
  }

  public EntityDef GetEntityDef(int dbId, bool displayError = true)
  {
    string cardId = GameUtils.TranslateDbIdToCardId(dbId);
    if (!(cardId == null & displayError))
      return this.GetEntityDef(cardId);
    Debug.LogErrorFormat("DefLoader.GetEntityDef() - dbId {0} does not map to a cardId", (object) dbId);
    return (EntityDef) null;
  }

  public void LoadAllEntityDefs()
  {
    List<string> allCardIds = GameUtils.GetAllCardIds();
    if (!allCardIds.Contains("PlaceholderCard"))
      allCardIds.Add("PlaceholderCard");
    List<string> failedCardIds;
    this.m_entityDefCache = EntityDef.LoadBatchCardEntityDefs(allCardIds, out failedCardIds);
    this.m_loadedEntityDefs = true;
    if (failedCardIds.Count <= 0)
      return;
    if (Application.isEditor)
      Debug.LogWarningFormat("LoadAllEntityDefs: Missing Cards! Proceed with caution. - Failed to load {0} card(s) on startup - {1}", (object) failedCardIds.Count, (object) string.Join(", ", failedCardIds.ToArray()));
    else
      Error.AddDevWarning("Missing Cards", "Failed to load {0} card(s) on startup!\n\n{1}", (object) failedCardIds.Count, (object) string.Join(", ", failedCardIds.ToArray()));
  }

  public bool HasLoadedEntityDefs() => this.m_loadedEntityDefs;

  public void LoadCardDef(
    string cardId,
    DefLoader.LoadDefCallback<DefLoader.DisposableCardDef> callback,
    object userData = null,
    CardPortraitQuality quality = null)
  {
    DefLoader.DisposableCardDef cardDef = this.GetCardDef(cardId, quality);
    callback(cardId, cardDef, userData);
  }

  public DefLoader.DisposableCardDef GetCardDef(int dbId)
  {
    string cardId = GameUtils.TranslateDbIdToCardId(dbId);
    if (cardId != null)
      return this.GetCardDef(cardId);
    Debug.LogError((object) string.Format("DefLoader.GetCardDef() - dbId {0} does not map to a cardId", (object) dbId));
    return (DefLoader.DisposableCardDef) null;
  }

  public DefLoader.DisposableCardDef GetCardDef(string cardId, TAG_PREMIUM premiumType)
  {
    CardPortraitQuality quality = CardPortraitQuality.GetDefault();
    quality.PremiumType = premiumType;
    return this.GetCardDef(cardId, quality);
  }

  public DefLoader.DisposableCardDef GetCardDef(string cardId, CardPortraitQuality quality = null)
  {
    if (string.IsNullOrEmpty(cardId) || AssetLoader.Get() == null)
      return (DefLoader.DisposableCardDef) null;
    if (cardId.Equals("None"))
      return (DefLoader.DisposableCardDef) null;
    if (quality == null)
      quality = CardPortraitQuality.GetDefault();
    if (PlatformSettings.ShouldFallbackToLowRes && quality.TextureQuality > 1)
      quality.TextureQuality = 1;
    AssetReference assetRefFromCardId = ServiceManager.Get<IAliasedAssetResolver>().GetCardDefAssetRefFromCardId(cardId);
    AssetHandle<GameObject> cardPrefabInstance = AssetLoader.Get().GetOrInstantiateSharedPrefab(assetRefFromCardId);
    CardDef cardDef = (bool) cardPrefabInstance ? cardPrefabInstance.Asset.GetComponent<CardDef>() : (CardDef) null;
    if ((UnityEngine.Object) cardDef == (UnityEngine.Object) null)
    {
      cardPrefabInstance?.Dispose();
      if (HearthstoneApplication.UseDevWorkarounds())
      {
        Debug.LogErrorFormat("DefLoader.GetCardDef() - Failed to load {0}. Using {1} instead.", (object) cardId, (object) "PlaceholderCard");
        cardPrefabInstance = this.LoadPlaceholderCardPrefab();
        if (!(bool) cardPrefabInstance)
        {
          Error.AddDevFatal("DefLoader.GetCardDef() - Failed to load {0} in place of {1}", (object) "PlaceholderCard", (object) cardId);
          return (DefLoader.DisposableCardDef) null;
        }
        cardDef = cardPrefabInstance.Asset.GetComponent<CardDef>();
      }
      else
      {
        Error.AddDevFatal("DefLoader.GetCardDef() - Failed to load {0}", (object) cardId);
        return (DefLoader.DisposableCardDef) null;
      }
    }
    if (CardPortraitQuality.GetFromDef(cardDef).TextureQuality < quality.TextureQuality || !cardDef.IsPremiumLoaded(quality.PremiumType))
      CardTextureLoader.Load(cardDef, quality);
    return new DefLoader.DisposableCardDef(cardPrefabInstance);
  }

  private AssetHandle<GameObject> LoadPlaceholderCardPrefab()
  {
    AssetReference assetRefFromCardId = ServiceManager.Get<IAliasedAssetResolver>().GetCardDefAssetRefFromCardId("PlaceholderCard");
    AssetHandle<GameObject> instantiateSharedPrefab = AssetLoader.Get().GetOrInstantiateSharedPrefab(assetRefFromCardId);
    if ((bool) instantiateSharedPrefab)
      return instantiateSharedPrefab;
    Debug.LogErrorFormat("DefLoader.LoadPlaceholderCardPrefab() - Failed to load {0}", (object) "PlaceholderCard");
    return (AssetHandle<GameObject>) null;
  }

  private void WillReset() => this.ClearEntityDefs();

  public DefLoader.DisposableFullDef GetFullDef(int dbId)
  {
    string cardId = GameUtils.TranslateDbIdToCardId(dbId);
    if (cardId != null)
      return this.GetFullDef(cardId);
    Debug.LogError((object) string.Format("DefLoader.GetCardDef() - dbId {0} does not map to a cardId", (object) dbId));
    return (DefLoader.DisposableFullDef) null;
  }

  public DefLoader.DisposableFullDef GetFullDef(string cardId, CardPortraitQuality quality = null) => new DefLoader.DisposableFullDef(this.GetEntityDef(cardId), this.GetCardDef(cardId, quality));

  public void LoadFullDef(
    string cardId,
    DefLoader.LoadDefCallback<DefLoader.DisposableFullDef> callback,
    object userData = null,
    CardPortraitQuality quality = null)
  {
    callback(cardId, this.GetFullDef(cardId, quality), userData);
  }

  public class DisposableCardDef : IDisposable
  {
    private AssetHandle<GameObject> m_cardPrefabInstance;

    public CardDef CardDef { get; private set; }

    public DisposableCardDef(AssetHandle<GameObject> cardPrefabInstance)
    {
      this.m_cardPrefabInstance = cardPrefabInstance;
      this.CardDef = (bool) this.m_cardPrefabInstance ? this.m_cardPrefabInstance.Asset.GetComponent<CardDef>() : (CardDef) null;
    }

    public void Dispose() => this.m_cardPrefabInstance?.Dispose();

    public DefLoader.DisposableCardDef Share() => new DefLoader.DisposableCardDef(this.m_cardPrefabInstance.Share());
  }

  public class DisposableFullDef : IDisposable
  {
    public CardDef CardDef => this.DisposableCardDef?.CardDef;

    public DefLoader.DisposableCardDef DisposableCardDef { get; }

    public EntityDef EntityDef { get; private set; }

    public DisposableFullDef(EntityDef entityDef, DefLoader.DisposableCardDef cardDef)
    {
      this.EntityDef = entityDef;
      this.DisposableCardDef = cardDef;
    }

    public void Dispose() => this.DisposableCardDef?.Dispose();

    public DefLoader.DisposableFullDef Share() => new DefLoader.DisposableFullDef(this.EntityDef, this.DisposableCardDef?.Share());
  }

  public delegate void LoadDefCallback<T>(string cardId, T def, object userData);
}
