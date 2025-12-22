using Blizzard.T5.AssetManager;
using Hearthstone.UI;
using System;
using UnityEngine;

[CustomEditClass]
public class HeroSkinInfoManager : BaseHeroSkinInfoManager
{
  private static HeroSkinInfoManager s_instance;
  private static bool s_isReadyingInstance;

  public static HeroSkinInfoManager Get() => HeroSkinInfoManager.s_instance;

  public static void EnterPreviewWhenReady(CollectionCardVisual cardVisual)
  {
    HeroSkinInfoManager heroSkinInfoManager = HeroSkinInfoManager.Get();
    if ((UnityEngine.Object) heroSkinInfoManager != (UnityEngine.Object) null)
      heroSkinInfoManager.EnterPreview(cardVisual);
    else if (HeroSkinInfoManager.s_isReadyingInstance)
    {
      Debug.LogWarning((object) "HeroSkinInfoManager:EnterPreviewWhenReady called while the info manager instance was being readied");
    }
    else
    {
      Widget widget = (Widget) WidgetInstance.Create("HeroSkinInfoManager.prefab:9d5b641eb672c491f8cbd2f20d2cbb61");
      if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "HeroSkinInfoManager:EnterPreviewWhenReady failed to create widget instance");
      }
      else
      {
        HeroSkinInfoManager.s_isReadyingInstance = true;
        widget.RegisterReadyListener((Action<object>) (_ =>
        {
          HeroSkinInfoManager.s_instance = widget.GetComponentInChildren<HeroSkinInfoManager>();
          HeroSkinInfoManager.s_isReadyingInstance = false;
          if ((UnityEngine.Object) HeroSkinInfoManager.s_instance == (UnityEngine.Object) null)
            Debug.LogError((object) "HeroSkinInfoManager:EnterPreviewWhenReady created widget instance but failed to get HeroSkinInfoManager component");
          else
            HeroSkinInfoManager.s_instance.EnterPreview(cardVisual);
        }), (object) null, true);
      }
    }
  }

  public static bool IsLoadedAndShowingPreview() => (bool) (UnityEngine.Object) HeroSkinInfoManager.s_instance && HeroSkinInfoManager.s_instance.IsShowingPreview;

  private void OnDestroy()
  {
    this.m_currentHeroCardDef?.Dispose();
    this.m_currentHeroCardDef = (DefLoader.DisposableCardDef) null;
    AssetHandle.SafeDispose<UberShaderAnimation>(ref this.m_currentHeroGoldenAnimation);
    this.CancelPreview();
    HeroSkinInfoManager.s_instance = (HeroSkinInfoManager) null;
  }

  protected override void PushNavigateBack() => Navigation.PushUnique(new Navigation.NavigateBackHandler(HeroSkinInfoManager.OnNavigateBack));

  protected override void RemoveNavigateBack() => Navigation.RemoveHandler(new Navigation.NavigateBackHandler(HeroSkinInfoManager.OnNavigateBack));

  private static bool OnNavigateBack()
  {
    HeroSkinInfoManager heroSkinInfoManager = HeroSkinInfoManager.Get();
    if ((UnityEngine.Object) heroSkinInfoManager != (UnityEngine.Object) null)
      heroSkinInfoManager.CancelPreview();
    return true;
  }

  protected override void SetFavoriteHero()
  {
    string cardId = this.m_currentEntityDef.GetCardId();
    TAG_CLASS heroClass = this.m_currentEntityDef.GetClass();
    NetCache.CardDefinition hero = CollectionManager.Get().GetFavoriteHero(cardId);
    bool isFavorite = hero == null;
    if (isFavorite)
      hero = new NetCache.CardDefinition()
      {
        Name = cardId,
        Premium = this.m_currentPremium
      };
    Network.Get().SetFavoriteHero(heroClass, hero, isFavorite);
    if (Network.IsLoggedIn())
      return;
    CollectionManager.Get().UpdateFavoriteHero(heroClass, cardId, this.m_currentPremium, isFavorite);
  }

  protected override bool CanToggleFavorite() => HeroSkinUtils.CanToggleFavoriteHeroSkin(this.m_currentEntityDef.GetClass(), this.m_currentEntityDef.GetCardId());

  protected override void SetupHeroSkinStore()
  {
    if (this.m_isStoreOpen)
      Debug.LogError((object) "CardBackInfoManager:SetupHeroSkinStore called when the store was already open");
    else if (this.m_currentHeroRecord == null)
    {
      Debug.LogError((object) "CardBackInfoManager:SetupHeroSkinStore: m_currentHeroRecord was null");
    }
    else
    {
      StoreManager storeManager = StoreManager.Get();
      if (!storeManager.IsOpen())
        return;
      storeManager.SetupHeroSkinStore(this, this.m_currentHeroRecord.CardId);
      storeManager.RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(((BaseHeroSkinInfoManager) this).OnSuccessfulPurchase));
      storeManager.RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(((BaseHeroSkinInfoManager) this).OnSuccessfulPurchaseAck));
      storeManager.RegisterFailedPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(((BaseHeroSkinInfoManager) this).OnFailedPurchaseAck));
      BnetBar.Get()?.RefreshCurrency();
    }
  }
}
