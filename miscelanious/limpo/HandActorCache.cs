using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HandActorCache
{
  private readonly TAG_CARDTYPE[] ACTOR_CARD_TYPES = new TAG_CARDTYPE[5]
  {
    TAG_CARDTYPE.MINION,
    TAG_CARDTYPE.SPELL,
    TAG_CARDTYPE.WEAPON,
    TAG_CARDTYPE.HERO,
    TAG_CARDTYPE.LOCATION
  };
  private Map<HandActorCache.ActorKey, Actor> m_actorMap = new Map<HandActorCache.ActorKey, Actor>();
  private List<HandActorCache.ActorLoadedListener> m_loadedListeners = new List<HandActorCache.ActorLoadedListener>();

  public void Initialize()
  {
    foreach (TAG_CARDTYPE tagCardtype in this.ACTOR_CARD_TYPES)
    {
      foreach (TAG_PREMIUM tagPremium in Enum.GetValues(typeof (TAG_PREMIUM)))
      {
        if (tagCardtype == TAG_CARDTYPE.HERO)
        {
          string heroSkinOrHandActor = ActorNames.GetHeroSkinOrHandActor(tagCardtype, tagPremium);
          HandActorCache.ActorKey callbackData1 = this.MakeActorKey(tagCardtype, tagPremium, true);
          AssetLoader.Get().InstantiatePrefab((AssetReference) heroSkinOrHandActor, new PrefabCallback<GameObject>(this.OnActorLoaded), (object) callbackData1, AssetLoadingOptions.IgnorePrefabPosition);
          string handActor = ActorNames.GetHandActor(tagCardtype, tagPremium);
          HandActorCache.ActorKey callbackData2 = this.MakeActorKey(tagCardtype, tagPremium);
          AssetLoader.Get().InstantiatePrefab((AssetReference) handActor, new PrefabCallback<GameObject>(this.OnActorLoaded), (object) callbackData2, AssetLoadingOptions.IgnorePrefabPosition);
        }
        else
        {
          string heroSkinOrHandActor = ActorNames.GetHeroSkinOrHandActor(tagCardtype, tagPremium);
          HandActorCache.ActorKey callbackData = this.MakeActorKey(tagCardtype, tagPremium);
          AssetLoader.Get().InstantiatePrefab((AssetReference) heroSkinOrHandActor, new PrefabCallback<GameObject>(this.OnActorLoaded), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
        }
      }
    }
  }

  public bool IsInitializing()
  {
    foreach (TAG_CARDTYPE cardType in this.ACTOR_CARD_TYPES)
    {
      foreach (TAG_PREMIUM premiumType in Enum.GetValues(typeof (TAG_PREMIUM)))
      {
        if (cardType == TAG_CARDTYPE.HERO)
        {
          HandActorCache.ActorKey key1 = this.MakeActorKey(cardType, premiumType, true);
          HandActorCache.ActorKey key2 = this.MakeActorKey(cardType, premiumType);
          if (!this.m_actorMap.ContainsKey(key1) || !this.m_actorMap.ContainsKey(key2))
            return true;
        }
        else if (!this.m_actorMap.ContainsKey(this.MakeActorKey(cardType, premiumType)))
          return true;
      }
    }
    return false;
  }

  public Actor GetActor(EntityDef entityDef, TAG_PREMIUM premium)
  {
    Actor actor;
    if (this.m_actorMap.TryGetValue(this.MakeActorKey(entityDef, premium, entityDef.IsHeroSkin()), out actor))
      return actor;
    Debug.LogError((object) string.Format("HandActorCache.GetActor() - FAILED to get actor with cardType={0} premiumType={1}", (object) entityDef.GetCardType(), (object) premium));
    return (Actor) null;
  }

  public void AddActorLoadedListener(HandActorCache.ActorLoadedCallback callback) => this.AddActorLoadedListener(callback, (object) null);

  public void AddActorLoadedListener(HandActorCache.ActorLoadedCallback callback, object userData)
  {
    HandActorCache.ActorLoadedListener actorLoadedListener = new HandActorCache.ActorLoadedListener();
    actorLoadedListener.SetCallback(callback);
    actorLoadedListener.SetUserData(userData);
    if (this.m_loadedListeners.Contains(actorLoadedListener))
      return;
    this.m_loadedListeners.Add(actorLoadedListener);
  }

  private HandActorCache.ActorKey MakeActorKey(
    EntityDef entityDef,
    TAG_PREMIUM premium,
    bool isHeroSkin = false)
  {
    return this.MakeActorKey(entityDef.GetCardType(), premium, isHeroSkin);
  }

  private HandActorCache.ActorKey MakeActorKey(
    TAG_CARDTYPE cardType,
    TAG_PREMIUM premiumType,
    bool isHeroSkin = false)
  {
    return new HandActorCache.ActorKey()
    {
      m_cardType = cardType,
      m_premiumType = premiumType,
      m_isHeroSkin = isHeroSkin
    };
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("HandActorCache.OnActorLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("HandActorCache.OnActorLoaded() - ERROR \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        go.transform.position = new Vector3(-99999f, -99999f, 99999f);
        HandActorCache.ActorKey key = (HandActorCache.ActorKey) callbackData;
        if (this.m_actorMap.ContainsKey(key))
        {
          Debug.LogWarning((object) string.Format("HandActorCache.OnActorLoaded() - ERROR \"{0}\" key (cardtype={1} cardpremium={2}) already exists in the dictionary", (object) assetRef, (object) key.m_cardType, (object) key.m_premiumType));
        }
        else
        {
          this.m_actorMap.Add(key, component);
          this.FireActorLoadedListeners(assetRef.ToString(), component);
        }
      }
    }
  }

  private void FireActorLoadedListeners(string assetRef, Actor actor)
  {
    foreach (HandActorCache.ActorLoadedListener actorLoadedListener in this.m_loadedListeners.ToArray())
      actorLoadedListener.Fire(assetRef, actor);
  }

  public delegate void ActorLoadedCallback(string name, Actor actor, object userData);

  private class ActorLoadedListener : EventListener<HandActorCache.ActorLoadedCallback>
  {
    public void Fire(string name, Actor actor) => this.m_callback(name, actor, this.m_userData);
  }

  private class ActorKey
  {
    public TAG_CARDTYPE m_cardType;
    public TAG_PREMIUM m_premiumType;
    public bool m_isHeroSkin;

    public override bool Equals(object obj) => obj != null && this.Equals(obj as HandActorCache.ActorKey);

    public bool Equals(HandActorCache.ActorKey other) => other != null && this.m_cardType == other.m_cardType && this.m_premiumType == other.m_premiumType && this.m_isHeroSkin == other.m_isHeroSkin;

    public override int GetHashCode() => ((23 * 17 + this.m_cardType.GetHashCode()) * 17 + this.m_premiumType.GetHashCode()) * 17 + this.m_isHeroSkin.GetHashCode();
  }
}
