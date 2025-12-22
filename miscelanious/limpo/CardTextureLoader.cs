using Blizzard.T5.AssetManager;
using Blizzard.T5.Services;
using Cysharp.Threading.Tasks;
using Hearthstone.Core.Streaming;
using Hearthstone.Streaming;
using System;
using UnityEngine;

public static class CardTextureLoader
{
  private static bool ItNotDownloading
  {
    get
    {
      GameDownloadManager service;
      return ServiceManager.TryGet<GameDownloadManager>(out service) && service.AssetDownloaderState != AssetDownloaderState.DOWNLOADING;
    }
  }

  public static bool Load(CardDef cardDef, CardPortraitQuality quality, bool prohibitRecursion = false)
  {
    if ((UnityEngine.Object) cardDef == (UnityEngine.Object) null)
      return false;
    bool flag1 = false;
    bool flag2 = false;
    cardDef.UpdateSpecialEvent();
    CardPortraitQuality portraitQuality = cardDef.GetPortraitQuality();
    bool flag3 = portraitQuality.TextureQuality < quality.TextureQuality;
    bool flag4 = !PlatformSettings.ShouldFallbackToLowRes;
    bool flag5 = quality.PremiumType == TAG_PREMIUM.SIGNATURE && portraitQuality.PremiumType != TAG_PREMIUM.SIGNATURE && CardTextureLoader.SignatureMaterialExists(cardDef);
    int num1 = quality.PremiumType != TAG_PREMIUM.GOLDEN && !cardDef.m_AlwaysRenderPremiumPortrait || portraitQuality.PremiumType == TAG_PREMIUM.GOLDEN ? 0 : (CardTextureLoader.PremiumAnimationExists(cardDef) ? 1 : 0);
    int num2 = quality.TextureQuality == 3 & flag3 & flag4 ? 1 : 0;
    bool flag6 = portraitQuality.TextureQuality == 0;
    UniTaskVoid uniTaskVoid;
    if (num2 != 0)
    {
      if (CardTextureLoader.HighQualityAvailable(cardDef))
      {
        CardTextureLoader.LoadHighQuality(cardDef);
        flag1 = true;
      }
      else if (!prohibitRecursion)
      {
        uniTaskVoid = CardTextureLoader.LoadDeferred(cardDef, new Func<CardDef, bool>(CardTextureLoader.HighQualityAvailable), quality);
        uniTaskVoid.Forget();
        prohibitRecursion = true;
      }
    }
    if (flag5)
      CardTextureLoader.LoadSignature(cardDef);
    if (num1 != 0)
    {
      if (CardTextureLoader.PremiumAnimationAvailable(cardDef))
      {
        CardTextureLoader.LoadGolden(cardDef);
        flag2 = true;
      }
      else if (!prohibitRecursion)
      {
        uniTaskVoid = CardTextureLoader.LoadDeferred(cardDef, new Func<CardDef, bool>(CardTextureLoader.PremiumAnimationAvailable), quality);
        uniTaskVoid.Forget();
        prohibitRecursion = true;
      }
    }
    if (flag6 && !flag1)
    {
      CardTextureLoader.LoadLowQuality(cardDef);
      flag1 = true;
    }
    CardTextureLoader.LoadBattlegroundHeroBuddy(cardDef);
    return flag1 | flag2;
  }

  private static bool HighQualityAvailable(CardDef cardDef) => !PlatformSettings.ShouldFallbackToLowRes && AssetLoader.Get().IsAssetAvailable(cardDef.GetPortraitRef());

  public static bool PremiumAnimationAvailable(CardDef cardDef)
  {
    if (!(bool) (UnityEngine.Object) cardDef)
      return false;
    IAssetLoader assetLoader = AssetLoader.Get();
    if (!assetLoader.IsAssetAvailable(cardDef.GetPremiumMaterialRef()))
      return false;
    AssetReference premiumPortraitRef = cardDef.GetPremiumPortraitRef();
    if ((premiumPortraitRef == null ? 0 : (!string.IsNullOrEmpty(premiumPortraitRef.guid) ? 1 : 0)) != 0 && !assetLoader.IsAppropriateVariantAvailable(premiumPortraitRef, CardTextureLoader.GetCardTextureOptions(false)))
      return false;
    AssetReference premiumAnimationRef = cardDef.GetPremiumAnimationRef();
    return (premiumAnimationRef == null ? 0 : (!string.IsNullOrEmpty(premiumAnimationRef.guid) ? 1 : 0)) == 0 || assetLoader.IsAssetAvailable(premiumAnimationRef);
  }

  private static bool PremiumAnimationExists(CardDef cardDef)
  {
    if (!(bool) (UnityEngine.Object) cardDef)
      return false;
    AssetReference premiumMaterialRef = cardDef.GetPremiumMaterialRef();
    return premiumMaterialRef != null && !string.IsNullOrEmpty(premiumMaterialRef.guid);
  }

  private static bool SignatureMaterialExists(CardDef cardDef)
  {
    if (!(bool) (UnityEngine.Object) cardDef)
      return false;
    AssetReference signatureMaterialRef = cardDef.GetSignatureMaterialRef();
    return signatureMaterialRef != null && !string.IsNullOrEmpty(signatureMaterialRef.guid);
  }

  private static async UniTaskVoid LoadDeferred(
    CardDef cardDef,
    Func<CardDef, bool> toWaitFor,
    CardPortraitQuality quality)
  {
    while (!toWaitFor(cardDef))
    {
      if (!(bool) (UnityEngine.Object) cardDef || CardTextureLoader.ItNotDownloading)
        return;
      await UniTask.Delay(TimeSpan.FromSeconds(0.300000011920929));
    }
    if (!CardTextureLoader.Load(cardDef, quality, true))
      return;
    foreach (Actor actor in UnityEngine.Object.FindObjectsOfType<Actor>())
    {
      if (actor.HasSameCardDef(cardDef))
        actor.UpdateAllComponents();
    }
  }

  private static void LoadBattlegroundHeroBuddy(CardDef cardDef)
  {
    AssetReference buddyPortraitRef = cardDef.GetBattlegroundHeroBuddyPortraitRef();
    if (buddyPortraitRef == null)
      return;
    using (AssetHandle<Texture> portrait = AssetLoader.Get().LoadAsset<Texture>(buddyPortraitRef, CardTextureLoader.GetCardTextureOptions(true)))
    {
      if (!(bool) portrait)
        Error.AddDevFatalUnlessWorkarounds("CardTextureLoader.LoadBattlegroundHeroBuddy - Failed to load asset for card {0}.  Portrait: {1}", (object) cardDef.name, portrait == null ? (object) "missing" : (object) "loaded");
      else
        cardDef.OnBattlegroundHeroBuddyPortraitLoaded(portrait);
    }
  }

  private static void LoadLowQuality(CardDef cardDef)
  {
    AssetReference portraitRef = cardDef.GetPortraitRef();
    if (portraitRef == null)
      return;
    using (AssetHandle<Texture> portrait = AssetLoader.Get().LoadAsset<Texture>(portraitRef, CardTextureLoader.GetCardTextureOptions(true)))
    {
      if (!(bool) portrait)
        Error.AddDevFatalUnlessWorkarounds("CardTextureLoader.LoadLowQuality - Failed to load asset for card {0}.  Portrait: {1}", (object) cardDef.name, portrait == null ? (object) "missing" : (object) "loaded");
      else
        cardDef.OnPortraitLoaded(portrait, 1);
    }
  }

  private static bool LoadHighQuality(CardDef cardDef)
  {
    AssetReference portraitRef = cardDef.GetPortraitRef();
    if (portraitRef == null)
      return false;
    using (AssetHandle<Texture> portrait = AssetLoader.Get().LoadAsset<Texture>(portraitRef, CardTextureLoader.GetCardTextureOptions(false)))
    {
      if ((bool) portrait)
      {
        cardDef.OnPortraitLoaded(portrait, 3);
        return true;
      }
    }
    using (AssetHandle<Texture> portrait = AssetLoader.Get().LoadAsset<Texture>(portraitRef, CardTextureLoader.GetCardTextureOptions(true)))
    {
      if ((bool) portrait)
        cardDef.OnPortraitLoaded(portrait, 1);
      else
        Error.AddDevFatalUnlessWorkarounds("CardTextureLoader.LoadHighQuality - Failed to load asset for card {0}.  Portrait: {1}", (object) cardDef.name, (object) "missing");
    }
    return false;
  }

  private static void LoadGolden(CardDef cardDef)
  {
    if ((UnityEngine.Object) cardDef == (UnityEngine.Object) null)
      return;
    AssetReference premiumMaterialRef = cardDef.GetPremiumMaterialRef();
    AssetReference premiumPortraitRef = cardDef.GetPremiumPortraitRef();
    AssetReference premiumAnimationRef = cardDef.GetPremiumAnimationRef();
    if (premiumMaterialRef == null)
      return;
    using (AssetHandle<Material> material = AssetLoader.Get().LoadAsset<Material>(premiumMaterialRef))
    {
      using (AssetHandle<UberShaderAnimation> portraitAnimation = premiumAnimationRef != null ? AssetLoader.Get().LoadAsset<UberShaderAnimation>(premiumAnimationRef) : (AssetHandle<UberShaderAnimation>) null)
      {
        using (AssetHandle<Texture> portrait = premiumPortraitRef != null ? AssetLoader.Get().LoadAsset<Texture>(premiumPortraitRef, CardTextureLoader.GetCardTextureOptions(false)) : (AssetHandle<Texture>) null)
        {
          if (!(bool) material)
            Error.AddDevFatalUnlessWorkarounds("CardTextureLoader.LoadGolden - Failed to load asset for card {0}.  Material: {1}, Premium Portrait: {2}, Animation: {3}", (object) cardDef.name, material == null ? (object) "missing" : (object) "loaded", portrait == null ? (object) "missing" : (object) "loaded", portraitAnimation == null ? (object) "missing" : (object) "loaded");
          else
            cardDef.OnPremiumMaterialLoaded(material, portrait, portraitAnimation);
        }
      }
    }
  }

  private static void LoadSignature(CardDef cardDef)
  {
    if ((UnityEngine.Object) cardDef == (UnityEngine.Object) null)
      return;
    AssetReference signatureMaterialRef = cardDef.GetSignatureMaterialRef();
    AssetReference signaturePortraitRef = cardDef.GetSignaturePortraitRef();
    if (signatureMaterialRef == null)
      return;
    using (AssetHandle<Material> material = AssetLoader.Get().LoadAsset<Material>(signatureMaterialRef))
    {
      using (AssetHandle<Texture> portrait = signaturePortraitRef != null ? AssetLoader.Get().LoadAsset<Texture>(signaturePortraitRef, CardTextureLoader.GetCardTextureOptions(false)) : (AssetHandle<Texture>) null)
      {
        if (!(bool) material)
          Error.AddDevFatalUnlessWorkarounds("CardTextureLoader.LoadSignature - Failed to load asset for card " + cardDef.name + ". Material missing,Texture " + (portrait == null ? "missing" : "loaded"));
        else
          cardDef.OnSignatureMaterialLoaded(material, portrait);
      }
    }
  }

  private static AssetLoadingOptions GetCardTextureOptions(bool forceLowRes) => forceLowRes || PlatformSettings.ShouldFallbackToLowRes ? AssetLoadingOptions.UseLowQuality : AssetLoadingOptions.None;
}
