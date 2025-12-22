using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Attribution;
using Hearthstone.CRM;
using MiniJSON;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdTrackingManager : IService
{
  private static long s_lastTrackedGoldBalanceThisSession;
  private static long s_lastTrackedDustBalanceThisSession;
  private bool m_isLoginFlowCompleted;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AdTrackingManager adTrackingManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    NetCache netCache = serviceLocator.Get<NetCache>();
    try
    {
      GameState.RegisterGameStateInitializedListener(new GameState.GameStateInitializedCallback(adTrackingManager.HandleGameCreated));
      netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheGoldBalance), new Action(adTrackingManager.TrackGoldBalanceChanges));
      netCache.RegisterUpdatedListener(typeof (NetCache.NetCacheArcaneDustBalance), new Action(adTrackingManager.TrackDustBalanceChanges));
      // ISSUE: reference to a compiler-generated method
      serviceLocator.Get<LoginManager>().OnFullLoginFlowComplete += new Action(adTrackingManager.\u003CInitialize\u003Eb__3_0);
      // ISSUE: reference to a compiler-generated method
      HearthstoneApplication.Get().WillReset += new Action(adTrackingManager.\u003CInitialize\u003Eb__3_1);
      StoreManager.Get().RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(adTrackingManager.HandleItemPurchase));
      StoreManager.Get().RegisterStoreShownListener(new Action(adTrackingManager.OnStoreShown));
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(adTrackingManager.OnSceneLoaded));
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ex);
    }
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (NetCache),
    typeof (LoginManager)
  };

  public void Shutdown()
  {
  }

  public static AdTrackingManager Get() => ServiceManager.Get<AdTrackingManager>();

  public void TrackLogin()
  {
    BlizzardAttributionManager.Get().SendEvent_Login();
    BlizzardCRMManager.Get().SendEvent_SessionStart((JsonNode) null);
  }

  public void TrackFirstLogin() => BlizzardAttributionManager.Get().SendEvent_FirstLogin();

  public void TrackAccountCreated() => BlizzardAttributionManager.Get().SendEvent_Registration();

  public void TrackHeadlessAccountCreated(string accountId = null)
  {
    Blizzard.Telemetry.Context messasgeContext = (Blizzard.Telemetry.Context) null;
    ulong result;
    if (ulong.TryParse(accountId, out result))
      messasgeContext = new Blizzard.Telemetry.Context()
      {
        BnetId = new ulong?(result)
      };
    BlizzardAttributionManager.Get().SendEvent_HeadlessAccountCreated(messasgeContext);
  }

  public void TrackHeadlessAccountHealedUp(string temporaryGameAccountId) => BlizzardAttributionManager.Get().SendEvent_HeadlessAccountHealedUp(temporaryGameAccountId);

  public void TrackAdventureProgress(string description)
  {
    Log.AdTracking.Print("Adventure Progress=" + description);
    string contentId = string.Format("Adventure_{0}", (object) description);
    BlizzardAttributionManager.Get().SendEvent_ContentUnlocked(contentId);
  }

  public void TrackTutorialProgress(TutorialProgress description, bool isVictory = true)
  {
  }

  public void TrackSale(double price, string currencyCode, string productId, string transactionId)
  {
    BlizzardAttributionManager.Get().SendEvent_Purchase(productId, transactionId, 1, currencyCode, false, (float) price);
    BlizzardCRMManager.Get().SendEvent_RealMoneyTransaction(productId, transactionId, 1, currencyCode, (float) price);
  }

  private void TrackGoldBalanceChanges()
  {
    NetCache.NetCacheGoldBalance balanceObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
    if (balanceObject == null)
      return;
    this.TrackGenericBalanceChanges("gold", ref AdTrackingManager.s_lastTrackedGoldBalanceThisSession, (Func<long>) (() => balanceObject.GetTotal()));
  }

  private void TrackDustBalanceChanges()
  {
    NetCache.NetCacheArcaneDustBalance balanceObject = NetCache.Get().GetNetObject<NetCache.NetCacheArcaneDustBalance>();
    if (balanceObject == null)
      return;
    this.TrackGenericBalanceChanges("dust", ref AdTrackingManager.s_lastTrackedDustBalanceThisSession, (Func<long>) (() => balanceObject.Balance));
  }

  private void TrackGenericBalanceChanges(
    string currencyName,
    ref long lastTrackedBalance,
    Func<long> balanceGetter)
  {
    long num = balanceGetter();
    if (!this.m_isLoginFlowCompleted)
    {
      lastTrackedBalance = num;
    }
    else
    {
      long amount = num - lastTrackedBalance;
      if (amount != 0L)
        BlizzardAttributionManager.Get().SendEvent_VirtualCurrencyTransaction((int) amount, currencyName);
      lastTrackedBalance = num;
    }
  }

  private void HandleItemPurchase(Network.Bundle bundle, PaymentMethod purchaseMethod)
  {
    StorePackId currentlySelectedId = StoreManager.Get().CurrentlySelectedId;
    if (currentlySelectedId.Type != StorePackType.BOOSTER || purchaseMethod == PaymentMethod.MONEY)
      return;
    BlizzardAttributionManager.Get().SendEvent_Purchase(currentlySelectedId.Id.ToString(), "", 1, "gold", true, 100f);
    BlizzardCRMManager.Get().SendEvent_VirtualCurrencyTransaction(currentlySelectedId.Id.ToString(), 100, 1, "gold", (JsonNode) null);
  }

  private void HandleGameCreated(GameState instance, object userData)
  {
    try
    {
      instance.RegisterGameOverListener(new GameState.GameOverCallback(this.HandleGameEnded));
      FormatType formatType = GameMgr.Get().GetFormatType();
      BlizzardAttributionManager.Get().SendEvent_GameRoundStart(GameMgr.Get().GetGameType().ToString(), formatType);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ex);
    }
  }

  private void HandleGameEnded(TAG_PLAYSTATE playState, object userData)
  {
    try
    {
      GameState gameState = GameState.Get();
      if (GameMgr.Get().IsAI())
      {
        int bossId = 0;
        Player opposingSidePlayer = gameState.GetOpposingSidePlayer();
        if (opposingSidePlayer != null)
        {
          Card heroCard = opposingSidePlayer.GetHeroCard();
          if ((UnityEngine.Object) heroCard != (UnityEngine.Object) null && heroCard.GetEntity() != null)
            bossId = GameUtils.TranslateCardIdToDbId(heroCard.GetEntity().GetCardId());
        }
        BlizzardAttributionManager.Get().SendEvent_ScenarioResult(GameMgr.Get().GetMissionId(), playState.ToString(), bossId);
      }
      else
      {
        FormatType formatType = GameMgr.Get().GetFormatType();
        BlizzardAttributionManager.Get().SendEvent_GameRoundEnd(GameMgr.Get().GetGameType().ToString(), playState.ToString(), formatType);
      }
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ex);
    }
  }

  private void OnStoreShown() => BlizzardAttributionManager.Get().SendEvent_FirstShopVisit();

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.HUB || !GameUtils.IsAnyTutorialComplete())
      return;
    string tutorialCompleted = string.Empty;
    if (GameUtils.IsTraditionalTutorialComplete())
      tutorialCompleted = "traditional";
    else if (GameUtils.IsMercenariesVillageTutorialComplete())
      tutorialCompleted = "mercenaries";
    else if (GameUtils.IsBattleGroundsTutorialComplete())
      tutorialCompleted = "battlegrounds";
    BlizzardAttributionManager.Get().SendEvent_BoxAfterTutorial(tutorialCompleted);
  }
}
