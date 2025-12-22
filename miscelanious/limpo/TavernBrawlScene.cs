using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class TavernBrawlScene : PegasusScene
{
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_CollectionManagerPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_TavernBrawlPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_TavernBrawlNoDeckPrefab;
  private bool m_unloading;
  private GameObject m_tavernBrawlRoot;
  private bool m_collectionManagerNeeded;
  private GameObject m_collectionManager;
  private bool m_pendingSessionBegin;

  protected override void Awake() => base.Awake();

  private void Start()
  {
    Network.Get().RegisterNetHandler((object) TavernBrawlRequestSessionBeginResponse.PacketID.ID, new Network.NetHandler(this.OnSessionBeginResponse));
    TavernBrawlManager.Get().EnsureAllDataReady(new TavernBrawlManager.CallbackEnsureServerDataReady(this.OnServerDataReady));
  }

  private void Update() => Network.Get().ProcessNetwork();

  public override bool IsUnloading() => this.m_unloading;

  public override void Unload()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      BnetBar.Get().ToggleActive(true);
    this.m_unloading = true;
    if ((UnityEngine.Object) CollectionManager.Get().GetCollectibleDisplay() != (UnityEngine.Object) null)
      CollectionManager.Get().GetCollectibleDisplay().Unload();
    if ((UnityEngine.Object) TavernBrawlDisplay.Get() != (UnityEngine.Object) null)
      TavernBrawlDisplay.Get().Unload();
    Network.Get().SendAckCardsSeen();
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnTavernBrawlTicketPurchaseAck));
    if ((UnityEngine.Object) this.m_tavernBrawlRoot != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_tavernBrawlRoot);
    if ((UnityEngine.Object) this.m_collectionManager != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_collectionManager);
    this.m_unloading = false;
  }

  private void OnServerDataReady()
  {
    if (TavernBrawlManager.Get().PlayerStatus == TavernBrawlStatus.TB_STATUS_INVALID)
    {
      if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      if (!TavernBrawlManager.Get().IsCurrentBrawlTypeActive)
        return;
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
      if (TavernBrawlManager.Get().CurrentMission().brawlMode == TavernBrawlMode.TB_MODE_HEROIC)
      {
        info.m_headerText = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR_TITLE");
        info.m_text = GameStrings.Get("GLUE_HEROIC_BRAWL_SESSION_ERROR");
      }
      else
      {
        info.m_headerText = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR_TITLE");
        info.m_text = GameStrings.Get("GLUE_BRAWLISEUM_SESSION_ERROR");
      }
      info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => TavernBrawlManager.Get().RefreshServerData());
      info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
      info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
      DialogManager.Get().ShowPopup(info);
    }
    else
    {
      CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
      if (TavernBrawlManager.Get().CurrentSession != null && collectionDeck != null)
        collectionDeck.Locked = TavernBrawlManager.Get().CurrentSession.DeckLocked;
      this.m_collectionManagerNeeded = TavernBrawlManager.Get().CurrentMission() != null && TavernBrawlManager.Get().CurrentMission().canEditDeck;
      bool flag = SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING;
      if (this.m_collectionManagerNeeded)
      {
        AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_TavernBrawlPrefab, new PrefabCallback<GameObject>(this.OnTavernBrawlLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
        AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_CollectionManagerPrefab, new PrefabCallback<GameObject>(this.OnCollectionManagerLoaded), options: (flag ? AssetLoadingOptions.None : AssetLoadingOptions.IgnorePrefabPosition));
      }
      else
        AssetLoader.Get().InstantiatePrefab((AssetReference) (string) (MobileOverrideValue<string>) this.m_TavernBrawlNoDeckPrefab, new PrefabCallback<GameObject>(this.OnTavernBrawlLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      if (TavernBrawlManager.Get().PlayerStatus == TavernBrawlStatus.TB_STATUS_TICKET_REQUIRED && !TavernBrawlManager.Get().IsEligibleForFreeTicket())
      {
        this.m_pendingSessionBegin = true;
        Network.Get().RequestTavernBrawlSessionBegin();
      }
      this.StartCoroutine(this.NotifySceneLoadedWhenReady());
    }
  }

  private IEnumerator NotifySceneLoadedWhenReady()
  {
    TavernBrawlScene tavernBrawlScene = this;
    while ((UnityEngine.Object) tavernBrawlScene.m_tavernBrawlRoot == (UnityEngine.Object) null)
      yield return (object) 0;
    while (tavernBrawlScene.m_collectionManagerNeeded && ((UnityEngine.Object) tavernBrawlScene.m_collectionManager == (UnityEngine.Object) null || !CollectionManager.Get().GetCollectibleDisplay().IsReady()))
      yield return (object) 0;
    while (tavernBrawlScene.m_pendingSessionBegin)
      yield return (object) 0;
    TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
    CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (tavernBrawlMission != null && tavernBrawlMission.canCreateDeck && collectionDeck != null && (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.ShowTavernBrawlDeck(collectionDeck.ID);
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(tavernBrawlScene.OnTavernBrawlTicketPurchaseAck));
    SceneMgr.Get().NotifySceneLoaded();
  }

  private void OnCollectionManagerLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_collectionManager = go;
    if (!((UnityEngine.Object) go == (UnityEngine.Object) null))
      return;
    Debug.LogError((object) string.Format("TavernBrawlScene.OnCollectionManagerLoaded() - failed to load screen {0}", (object) assetRef));
  }

  private void OnTavernBrawlLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_tavernBrawlRoot = go;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("TavernBrawlScene.OnTavernBrawlLoaded() - failed to load screen {0}", (object) assetRef));
    }
    else
    {
      if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING)
        return;
      go.transform.position = Vector3.zero;
    }
  }

  private void OnSessionBeginResponse() => this.m_pendingSessionBegin = false;

  private void OnTavernBrawlTicketPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    Log.TavernBrawl.Print("TavernBrawlScene.OnTavernBrawlTicketPurchaseAck");
    if ((Record) bundle == (Record) null || bundle.Items == null)
      return;
    foreach (Network.BundleItem bundleItem in bundle.Items)
    {
      if ((Record) bundleItem != (Record) null && bundleItem.ItemType == ProductType.PRODUCT_TYPE_TAVERN_BRAWL_TICKET && SceneMgr.Get().IsModeRequested(SceneMgr.Mode.TAVERN_BRAWL) || SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FIRESIDE_GATHERING))
      {
        TavernBrawlManager.Get().RequestSessionBegin();
        return;
      }
    }
    Log.TavernBrawl.PrintError("TavernBrawlScene.OnTavernBrawlTicketPurchaseAck ERROR: Got a purchase ack in the Tavern Brawl scene for a product we don't recognize");
  }
}
