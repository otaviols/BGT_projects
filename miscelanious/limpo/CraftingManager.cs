using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class CraftingManager : MonoBehaviour
{
  public Transform m_floatingCardBone;
  public Transform m_faceDownCardBone;
  public Transform m_cardInfoPaneBone;
  public Transform m_cardCounterBone;
  public Transform m_signatureCardCounterBone;
  public Transform m_showCraftingUIBone;
  public Transform m_hideCraftingUIBone;
  public Transform m_showUpgradeToGoldenPopupBone;
  public BoxCollider m_offClickCatcher;
  public CraftCardCountTab m_cardCountTab;
  public Vector3 m_cardCountTabShowScale = Vector3.one;
  public Vector3 m_cardCountTabHideScale = new Vector3(1f, 1f, 0.0f);
  public PegUIElement m_switchPremiumButton;
  public QuestCardRewardOverlay m_questCardRewardOverlay;
  public float m_timeForCardToFlipUp;
  public float m_timeForBackCardToMoveUp;
  public float m_delayBeforeBackCardMovesUp;
  public iTween.EaseType m_easeTypeForCardFlip;
  public iTween.EaseType m_easeTypeForCardMoveUp;
  public Vector3 m_utgAlertPopupOffset = new Vector3(-5f, 0.0f, 0.0f);
  private static CraftingManager s_instance;
  public CraftingUI m_craftingUI;
  private Widget m_upgradeToGoldenWidget;
  private bool m_upgradeToGoldenWidgetShown;
  private Actor m_currentBigActor;
  private bool m_isCurrentActorAGhost;
  private Actor m_upsideDownActor;
  private Actor m_currentRelatedCardActor;
  private Actor m_ghostWeaponActor;
  private Actor m_ghostMinionActor;
  private Actor m_ghostSpellActor;
  private Actor m_ghostHeroActor;
  private Actor m_ghostHeroPowerActor;
  private Actor m_ghostLocationActor;
  private Actor m_templateWeaponActor;
  private Actor m_templateSpellActor;
  private Actor m_templateMinionActor;
  private Actor m_templateHeroActor;
  private Actor m_templateHeroPowerActor;
  private Actor m_templateLocationActor;
  private Actor m_hiddenActor;
  private CardInfoPane m_cardInfoPane;
  private Actor m_templateGoldenWeaponActor;
  private Actor m_templateGoldenSpellActor;
  private Actor m_templateGoldenMinionActor;
  private Actor m_templateGoldenHeroActor;
  private Actor m_templateGoldenHeroPowerActor;
  private Actor m_templateDiamondMinionActor;
  private Actor m_templateSignatureMinionActor;
  private Actor m_templateGoldenLocationActor;
  private Actor m_ghostGoldenWeaponActor;
  private Actor m_ghostGoldenSpellActor;
  private Actor m_ghostGoldenMinionActor;
  private Actor m_ghostGoldenHeroActor;
  private Actor m_ghostGoldenHeroPowerActor;
  private Actor m_ghostDiamondMinionActor;
  private Actor m_ghostSignatureMinionActor;
  private Actor m_ghostGoldenLocationActor;
  private bool m_cancellingCraftMode;
  private long m_unCommitedArcaneDustAdjustments;
  private CraftingPendingTransaction m_pendingClientTransaction;
  private CraftingPendingTransaction m_pendingServerTransaction;
  private Vector3 m_craftSourcePosition;
  private Vector3 m_craftSourceScale;
  private CollectionCardActors m_cardActors;
  private Actor m_collectionCardActor;
  private bool m_elementsLoaded;
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_START_POSITION = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.0f, -0.5f, 0.0f),
    Phone = new Vector3(0.0f, -0.5f, 0.0f)
  };
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_START_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.1f, 0.1f, 0.1f),
    Phone = new Vector3(0.1f, 0.1f, 0.1f)
  };
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_POSITION = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(-2.11f, -0.010312f, -0.06f),
    Phone = new Vector3(-1.97f, -0.0006f, -0.033f)
  };
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.85f, 0.85f, 0.85f),
    Phone = new Vector3(0.76637f, 0.76637f, 0.76637f)
  };
  private static readonly float HERO_POWER_TWEEN_TIME = 0.5f;
  private static readonly AssetReference UPGRADE_TO_GOLDEN_WIDGET_PREFAB = new AssetReference("UpgradeToGoldenPopup.prefab:15b90c0a0040d1445a44a463626214bc");
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    CollectionManager.Get()?.RegisterMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
    if ((UnityEngine.Object) this.m_upgradeToGoldenWidget != (UnityEngine.Object) null)
      this.m_upgradeToGoldenWidget.Hide();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy()
  {
    if (CollectionManager.Get() != null)
      CollectionManager.Get().RemoveMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
    CraftingManager.s_instance = (CraftingManager) null;
  }

  private void Start() => this.LoadElements();

  private void LoadElements()
  {
    if (this.m_elementsLoaded)
      return;
    this.LoadActor("Card_Hand_Weapon.prefab:30888a1fdca5c6c43abcc5d9dca55783", ref this.m_ghostWeaponActor, ref this.m_templateWeaponActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.WEAPON, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenWeaponActor, ref this.m_templateGoldenWeaponActor);
    this.LoadActor("Card_Hand_Ally.prefab:d00eb0f79080e0749993fe4619e9143d", ref this.m_ghostMinionActor, ref this.m_templateMinionActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.MINION, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenMinionActor, ref this.m_templateGoldenMinionActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.MINION, TAG_PREMIUM.SIGNATURE), ref this.m_ghostSignatureMinionActor, ref this.m_templateSignatureMinionActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.MINION, TAG_PREMIUM.DIAMOND), ref this.m_ghostDiamondMinionActor, ref this.m_templateDiamondMinionActor);
    this.LoadActor("Card_Hand_Ability.prefab:3c3f5189f0d0b3745a1c1ca21d41efe0", ref this.m_ghostSpellActor, ref this.m_templateSpellActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.SPELL, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenSpellActor, ref this.m_templateGoldenSpellActor);
    this.LoadActor("Card_Hand_Hero.prefab:a977c49edb5fb5d4c8dee4d2344d1395", ref this.m_ghostHeroActor, ref this.m_templateHeroActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.HERO, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenHeroActor, ref this.m_templateGoldenHeroActor);
    this.LoadActor("History_HeroPower.prefab:e73edf8ccea2b11429093f7a448eef53", ref this.m_ghostHeroPowerActor, ref this.m_templateHeroPowerActor);
    this.LoadActor(ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenHeroPowerActor, ref this.m_templateGoldenHeroPowerActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.LOCATION, TAG_PREMIUM.NORMAL), ref this.m_ghostLocationActor, ref this.m_templateLocationActor);
    this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.LOCATION, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenLocationActor, ref this.m_templateGoldenLocationActor);
    this.LoadActor("Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", ref this.m_hiddenActor);
    this.m_hiddenActor.GetMeshRenderer().transform.localEulerAngles = new Vector3(0.0f, 180f, 180f);
    LayerUtils.SetLayer(this.m_hiddenActor.gameObject, GameLayer.IgnoreFullScreenEffects);
    SoundManager.Get().Load((AssetReference) "Card_Transition_Out.prefab:aecf5b5837772844b9d2db995744df82");
    SoundManager.Get().Load((AssetReference) "Card_Transition_In.prefab:3f3fbe896b8b260448e8c7e5d028d971");
    this.LoadRandomCardBack();
    this.m_elementsLoaded = true;
  }

  public void SwitchPremiumView(TAG_PREMIUM premium)
  {
    if (premium == this.GetShownActor().GetPremium())
      return;
    if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_upsideDownActor.gameObject);
      this.m_upsideDownActor = (Actor) null;
    }
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
    {
      this.m_currentBigActor.GetSpell(SpellType.GHOSTMODE).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentBigActor.gameObject);
      this.m_currentBigActor = (Actor) null;
    }
    string cardId = this.m_collectionCardActor.GetEntityDef().GetCardId();
    this.m_pendingClientTransaction.CardID = !GameUtils.IsClassicCard(cardId) ? cardId : GameUtils.TranslateDbIdToCardId(DefLoader.Get().GetEntityDef(cardId).GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID));
    this.m_pendingClientTransaction.Premium = premium;
    NetCache.CardValue cardValue = this.GetCardValue(this.m_pendingClientTransaction.CardID, premium);
    if (cardValue != null)
      this.m_pendingClientTransaction.CardValueOverridden = cardValue.IsOverrideActive();
    this.MoveCardToBigSpot(this.m_collectionCardActor, premium);
    if (!((UnityEngine.Object) this.m_craftingUI != (UnityEngine.Object) null))
      return;
    this.m_craftingUI.Enable(this.m_showCraftingUIBone.position, this.m_hideCraftingUIBone.position);
  }

  public static bool IsInitialized => (UnityEngine.Object) CraftingManager.s_instance != (UnityEngine.Object) null;

  public static CraftingManager Get()
  {
    if ((UnityEngine.Object) CraftingManager.s_instance == (UnityEngine.Object) null)
    {
      string assetRef = (bool) UniversalInputManager.UsePhoneUI ? "CraftingManager_phone.prefab:d28ac29ae64f14e649186d0d1fe5f7e8" : "CraftingManager.prefab:9dc2dd187dd914959b311d326c3fd5b2";
      CraftingManager.s_instance = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef).GetComponent<CraftingManager>();
      CraftingManager.s_instance.LoadElements();
    }
    return CraftingManager.s_instance;
  }

  public NetCache.CardValue GetCardValue(string cardID, TAG_PREMIUM premium)
  {
    NetCache.CardValue cardValue = new NetCache.CardValue();
    string cardId = cardID;
    if (GameUtils.IsClassicCard(cardID))
      cardId = GameUtils.TranslateDbIdToCardId(DefLoader.Get().GetEntityDef(cardID).GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID));
    if (this.IsLegacyCardValueCacheEnabled())
    {
      NetCache.CardDefinition key = new NetCache.CardDefinition()
      {
        Name = cardId,
        Premium = premium
      };
      NetCache.NetCacheCardValues netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardValues>();
      return netObject == null || !netObject.Values.TryGetValue(key, out cardValue) ? (NetCache.CardValue) null : cardValue;
    }
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    CardValueDbfRecord record1 = GameDbf.CardValue.GetRecord(GameUtils.TranslateCardIdToDbId(cardId));
    InitCardValueDbfRecord record2 = GameDbf.InitCardValue.GetRecord((Predicate<InitCardValueDbfRecord>) (x => entityDef.GetRarity() == (TAG_RARITY) x.Rarity && (TAG_PREMIUM) x.Premium == premium));
    if (record2 == null)
      return (NetCache.CardValue) null;
    cardValue.BaseBuyValue = record2.Buy;
    cardValue.BaseSellValue = record2.Sell;
    cardValue.BaseUpgradeValue = record2.Upgrade;
    if (record1 != null)
    {
      cardValue.BuyValueOverride = record1.Buy;
      cardValue.SellValueOverride = record1.Sell;
      cardValue.OverrideEvent = record1.OverrideEvent;
    }
    return cardValue;
  }

  public bool CanUpgradeCardToGolden(string cardID, TAG_PREMIUM premium)
  {
    int upgradeValue;
    if (!this.HasUpgradeToGoldenEnabled() || premium != TAG_PREMIUM.NORMAL && premium != TAG_PREMIUM.GOLDEN || !CollectionManager.Get().GetCard(cardID, TAG_PREMIUM.NORMAL).IsCraftable || !CollectionManager.Get().GetCard(cardID, TAG_PREMIUM.GOLDEN).IsCraftable || !this.TryGetCardUpgradeValue(cardID, out upgradeValue) || NetCache.Get().GetArcaneDustBalance() < (long) upgradeValue)
      return false;
    bool flag = CollectionManager.Get().GetCard(cardID, premium).Rarity == TAG_RARITY.LEGENDARY;
    int ownedIncludePending1 = this.GetNumOwnedIncludePending(cardID, new TAG_PREMIUM?(TAG_PREMIUM.NORMAL));
    int ownedIncludePending2 = this.GetNumOwnedIncludePending(cardID, new TAG_PREMIUM?(TAG_PREMIUM.GOLDEN));
    return ownedIncludePending2 < CollectionManager.Get().GetCard(cardID, premium).DefaultMaxCopiesPerDeck && (ownedIncludePending1 >= CollectionManager.Get().GetCard(cardID, premium).DefaultMaxCopiesPerDeck || !flag && ownedIncludePending1 == 1 && ownedIncludePending2 == 1 || !flag && ownedIncludePending1 == 1 && premium == TAG_PREMIUM.GOLDEN);
  }

  public bool HasUpgradeToGoldenEnabled() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().EnableUpgradeToGolden;

  public bool IsLegacyCardValueCacheEnabled() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().LegacyCardValueCacheEnabled;

  public bool CanCraftCardRightNow(EntityDef entityDef, TAG_PREMIUM premium)
  {
    NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition()
    {
      Name = entityDef.GetCardId(),
      Premium = premium
    };
    int buyValue;
    return this.GetNumOwnedIncludePending() < (entityDef.IsElite() ? 1 : 2) && this.TryGetCardBuyValue(cardDefinition.Name, cardDefinition.Premium, out buyValue) && NetCache.Get().GetArcaneDustBalance() >= (long) buyValue && !RankMgr.Get().IsCardLockedInCurrentLeague(entityDef);
  }

  public bool TryGetCardBuyValue(string cardID, TAG_PREMIUM premium, out int buyValue)
  {
    NetCache.CardValue cardValue = this.GetCardValue(cardID, premium);
    if (cardValue == null)
    {
      buyValue = 0;
      return false;
    }
    if (this.GetNumClientTransactions() >= 0)
    {
      buyValue = cardValue.GetBuyValue();
      return true;
    }
    buyValue = cardValue.GetSellValue();
    return true;
  }

  public bool TryGetCardSellValue(string cardID, TAG_PREMIUM premium, out int sellValue)
  {
    NetCache.CardValue cardValue = this.GetCardValue(cardID, premium);
    if (cardValue == null)
    {
      sellValue = 0;
      return false;
    }
    if (this.GetNumClientTransactions() <= 0)
    {
      sellValue = cardValue.GetSellValue();
      return true;
    }
    sellValue = cardValue.GetBuyValue();
    return true;
  }

  public bool TryGetCardUpgradeValue(string cardID, out int upgradeValue)
  {
    NetCache.CardValue cardValue = this.GetCardValue(cardID, TAG_PREMIUM.NORMAL);
    if (cardValue == null)
    {
      upgradeValue = 0;
      return false;
    }
    upgradeValue = cardValue.GetUpgradeValue();
    return true;
  }

  public bool IsCardShowing() => (UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null;

  public static bool GetIsInCraftingMode() => (UnityEngine.Object) CraftingManager.s_instance != (UnityEngine.Object) null && CraftingManager.s_instance.IsInCraftingMode;

  private bool IsInCraftingMode { get; set; }

  public bool GetShownCardInfo(out EntityDef entityDef, out TAG_PREMIUM premium)
  {
    entityDef = (EntityDef) null;
    premium = TAG_PREMIUM.NORMAL;
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null)
      return false;
    entityDef = this.m_currentBigActor.GetEntityDef();
    premium = this.m_currentBigActor.GetPremium();
    return entityDef != null;
  }

  public Actor GetShownActor() => this.m_currentBigActor;

  public void OnMassDisenchant(int amount)
  {
    if ((bool) (UnityEngine.Object) MassDisenchant.Get())
      return;
    this.m_craftingUI.UpdateBankText();
  }

  public long GetUnCommitedArcaneDustChanges() => this.m_unCommitedArcaneDustAdjustments;

  public void AdjustUnCommitedArcaneDustChanges(int amount) => this.m_unCommitedArcaneDustAdjustments += (long) amount;

  public void ResetUnCommitedArcaneDustChanges() => this.m_unCommitedArcaneDustAdjustments = 0L;

  public int GetNumClientTransactions() => this.m_pendingClientTransaction == null ? 0 : this.m_pendingClientTransaction.GetTransactionAmount(this.GetShownActor().GetPremium());

  public void NotifyOfTransaction(int amt)
  {
    if (this.m_pendingClientTransaction == null)
      return;
    if (amt > 0)
    {
      if (this.GetPendingClientTransaction().GetLastTransactionWasDisenchant())
      {
        int num = (int) this.GetPendingClientTransaction().Undo();
        return;
      }
      if (this.m_craftingUI.m_buttonCreate.GetCraftingState() == CraftingButton.CraftingState.Create)
      {
        if (this.GetShownActor().GetPremium() == TAG_PREMIUM.NORMAL)
          this.GetPendingClientTransaction().Add(CraftingPendingTransaction.Operation.NormalCreate);
        else if (this.GetShownActor().GetPremium() == TAG_PREMIUM.GOLDEN)
          this.GetPendingClientTransaction().Add(CraftingPendingTransaction.Operation.GoldenCreate);
      }
      else if (this.m_craftingUI.m_buttonCreate.GetCraftingState() == CraftingButton.CraftingState.Upgrade)
      {
        this.GetPendingClientTransaction().Add(CraftingPendingTransaction.Operation.UpgradeToGoldenFromNormal);
        this.SwitchPremiumView(TAG_PREMIUM.GOLDEN);
      }
    }
    if (amt >= 0)
      return;
    if (this.GetPendingClientTransaction().GetLastTransactionWasCrafting())
    {
      int num1 = (int) this.GetPendingClientTransaction().Undo();
    }
    else if (this.GetShownActor().GetPremium() == TAG_PREMIUM.NORMAL)
    {
      this.GetPendingClientTransaction().Add(CraftingPendingTransaction.Operation.NormalDisenchant);
    }
    else
    {
      if (this.GetShownActor().GetPremium() != TAG_PREMIUM.GOLDEN)
        return;
      this.GetPendingClientTransaction().Add(CraftingPendingTransaction.Operation.GoldenDisenchant);
    }
  }

  public bool IsCancelling() => this.m_cancellingCraftMode;

  private Actor CreateActorCopy(Actor actor, TAG_PREMIUM premium)
  {
    string heroSkinOrHandActor = ActorNames.GetHeroSkinOrHandActor(actor.GetEntityDef(), premium);
    Actor component = AssetLoader.Get().InstantiatePrefab((AssetReference) heroSkinOrHandActor, AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
    component.SetFullDefFromActor(actor);
    component.SetEntity(actor.GetEntity());
    component.SetPremium(premium);
    component.UpdateAllComponents();
    return component;
  }

  public void EnterCraftMode(Actor collectionCardActor, Action callback = null)
  {
    this.m_collectionCardActor = collectionCardActor;
    if ((UnityEngine.Object) this.m_collectionCardActor == (UnityEngine.Object) null)
      return;
    this.m_cardActors = new CollectionCardActors();
    switch (collectionCardActor.GetPremium())
    {
      case TAG_PREMIUM.DIAMOND:
        this.m_cardActors.AddCardActor(this.CreateActorCopy(collectionCardActor, TAG_PREMIUM.DIAMOND));
        break;
      case TAG_PREMIUM.SIGNATURE:
        this.m_cardActors.AddCardActor(this.CreateActorCopy(collectionCardActor, TAG_PREMIUM.SIGNATURE));
        break;
      default:
        this.m_cardActors.AddCardActor(this.CreateActorCopy(collectionCardActor, TAG_PREMIUM.NORMAL));
        this.m_cardActors.AddCardActor(this.CreateActorCopy(collectionCardActor, TAG_PREMIUM.GOLDEN));
        break;
    }
    if (this.m_cancellingCraftMode || CollectionDeckTray.Get().IsWaitingToDeleteDeck())
      return;
    CollectionManager.Get().GetCollectibleDisplay().HideAllTips();
    this.m_offClickCatcher.enabled = true;
    TooltipPanelManager.Get().HideKeywordHelp();
    this.SetupActor(this.m_collectionCardActor, this.m_collectionCardActor.GetPremium());
    if ((UnityEngine.Object) this.m_cardInfoPane == (UnityEngine.Object) null && !(bool) UniversalInputManager.UsePhoneUI)
      this.m_cardInfoPane = AssetLoader.Get().InstantiatePrefab((AssetReference) "CardInfoPane.prefab:b9220edd61d504be38fab162c18e56f1").GetComponent<CardInfoPane>();
    if ((UnityEngine.Object) this.m_cardInfoPane != (UnityEngine.Object) null)
      this.m_cardInfoPane.UpdateContent();
    if ((UnityEngine.Object) this.m_craftingUI == (UnityEngine.Object) null)
    {
      string assetRef = (bool) UniversalInputManager.UsePhoneUI ? "CraftingUI_Phone.prefab:3119329ada4ac4a8888187b5b2d373f5" : "CraftingUI.prefab:ef05b5bf5ebb14a22919f0095d75f0b2";
      this.m_craftingUI = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef).GetComponent<CraftingUI>();
      this.m_craftingUI.SetStartingActive();
      GameUtils.SetParent((Component) this.m_craftingUI, this.m_showCraftingUIBone.gameObject);
    }
    this.m_craftingUI.gameObject.SetActive(true);
    this.m_switchPremiumButton.gameObject.SetActive(false);
    this.m_craftingUI.Enable(this.m_showCraftingUIBone.position, this.m_hideCraftingUIBone.position);
    if ((UnityEngine.Object) this.m_upgradeToGoldenWidget == (UnityEngine.Object) null)
    {
      this.m_upgradeToGoldenWidget = (Widget) WidgetInstance.Create((string) CraftingManager.UPGRADE_TO_GOLDEN_WIDGET_PREFAB);
      this.m_upgradeToGoldenWidget.Hide();
      this.m_upgradeToGoldenWidget.RegisterReadyListener((Action<object>) (_ => GameUtils.SetParent((Component) this.m_upgradeToGoldenWidget, this.m_showCraftingUIBone.gameObject)), (object) null, true);
    }
    this.m_upgradeToGoldenWidget.Hide();
    this.FadeEffectsIn();
    this.UpdateCardInfoPane();
    this.ShowLeagueLockedCardPopup();
    this.IsInCraftingMode = true;
    Navigation.Push((Navigation.NavigateBackHandler) (() =>
    {
      int num = this.CancelCraftMode() ? 1 : 0;
      if (callback == null)
        return num != 0;
      callback();
      return num != 0;
    }));
  }

  private void SetupActor(Actor collectionCardActor, TAG_PREMIUM premium)
  {
    if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_upsideDownActor.gameObject);
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentBigActor.gameObject);
    Debug.Log((object) ("setting up actor " + (object) collectionCardActor.GetEntityDef() + " " + (object) premium));
    this.MoveCardToBigSpot(collectionCardActor, premium);
    string cardId = collectionCardActor.GetEntityDef().GetCardId();
    this.m_pendingClientTransaction = new CraftingPendingTransaction();
    this.m_pendingClientTransaction.CardID = !GameUtils.IsClassicCard(cardId) ? cardId : GameUtils.TranslateDbIdToCardId(DefLoader.Get().GetEntityDef(cardId).GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID));
    this.m_pendingClientTransaction.Premium = premium;
    this.m_pendingClientTransaction.ResetTransactionAmount();
    NetCache.CardValue cardValue = this.GetCardValue(this.m_pendingClientTransaction.CardID, premium);
    if (cardValue != null)
      this.m_pendingClientTransaction.CardValueOverridden = cardValue.IsOverrideActive();
    if (!((UnityEngine.Object) this.m_craftingUI != (UnityEngine.Object) null))
      return;
    this.m_craftingUI.Enable(this.m_showCraftingUIBone.position, this.m_hideCraftingUIBone.position);
  }

  public bool CancelCraftMode()
  {
    if ((UnityEngine.Object) this.m_upgradeToGoldenWidget != (UnityEngine.Object) null && this.m_upgradeToGoldenWidgetShown)
    {
      this.HideUpgradeToGoldenWidget();
      return false;
    }
    this.StopAllCoroutines();
    this.m_offClickCatcher.enabled = false;
    this.m_cancellingCraftMode = true;
    Actor actor1 = this.m_upsideDownActor;
    Actor actor2 = this.m_currentBigActor;
    if ((UnityEngine.Object) actor2 == (UnityEngine.Object) null && (UnityEngine.Object) actor1 != (UnityEngine.Object) null)
    {
      actor2 = actor1;
      actor1 = (Actor) null;
    }
    float time = 0.2f;
    if ((UnityEngine.Object) actor2 != (UnityEngine.Object) null)
    {
      iTween.Stop(actor2.gameObject);
      iTween.RotateTo(actor2.gameObject, Vector3.zero, time);
      actor2.ToggleForceIdle(false);
      if ((UnityEngine.Object) actor1 != (UnityEngine.Object) null)
      {
        iTween.Stop(actor1.gameObject);
        actor1.transform.parent = actor2.transform;
      }
      SoundManager.Get().LoadAndPlay((AssetReference) "Card_Transition_In.prefab:3f3fbe896b8b260448e8c7e5d028d971");
      iTween.MoveTo(actor2.gameObject, iTween.Hash((object) "name", (object) nameof (CancelCraftMode), (object) "position", (object) this.m_craftSourcePosition, (object) "time", (object) time, (object) "oncomplete", (object) "FinishActorMoveAway", (object) "oncompletetarget", (object) this.gameObject, (object) "easetype", (object) iTween.EaseType.linear));
      iTween.ScaleTo(actor2.gameObject, iTween.Hash((object) "scale", (object) this.m_craftSourceScale, (object) "time", (object) time, (object) "easetype", (object) iTween.EaseType.linear));
    }
    iTween.Stop(this.m_cardCountTab.gameObject);
    if (this.GetNumOwnedIncludePending() > 0)
    {
      iTween.MoveTo(this.m_cardCountTab.gameObject, iTween.Hash((object) "position", (object) (this.m_craftSourcePosition - new Vector3(0.0f, 12f, 0.0f)), (object) "time", (object) (float) (3.0 * (double) time), (object) "oncomplete", (object) iTween.EaseType.easeInQuad));
      iTween.ScaleTo(this.m_cardCountTab.gameObject, iTween.Hash((object) "scale", (object) (0.1f * Vector3.one), (object) "time", (object) (float) (3.0 * (double) time), (object) "oncomplete", (object) iTween.EaseType.easeInQuad));
    }
    if ((UnityEngine.Object) actor1 != (UnityEngine.Object) null)
    {
      iTween.RotateTo(actor1.gameObject, new Vector3(0.0f, 359f, 180f), time);
      iTween.MoveTo(actor1.gameObject, iTween.Hash((object) "name", (object) "CancelCraftMode2", (object) "position", (object) new Vector3(0.0f, -1f, 0.0f), (object) "time", (object) time, (object) "islocal", (object) true));
      iTween.ScaleTo(actor1.gameObject, new Vector3(actor1.transform.localScale.x * 0.8f, actor1.transform.localScale.y * 0.8f, actor1.transform.localScale.z * 0.8f), time);
    }
    this.HideAndDestroyRelatedBigCard();
    if ((UnityEngine.Object) this.m_craftingUI != (UnityEngine.Object) null && this.m_craftingUI.IsEnabled())
      this.m_craftingUI.Disable(this.m_hideCraftingUIBone.position);
    this.m_cardCountTab.m_shadow.GetComponent<Animation>().Play("Crafting2ndCardShadowOff");
    this.FadeEffectsOut();
    if ((UnityEngine.Object) this.m_cardInfoPane != (UnityEngine.Object) null)
    {
      iTween.Stop(this.m_cardInfoPane.gameObject);
      this.m_cardInfoPane.gameObject.SetActive(false);
    }
    if ((UnityEngine.Object) this.m_upgradeToGoldenWidget != (UnityEngine.Object) null)
    {
      this.m_upgradeToGoldenWidget.Hide();
      this.m_upgradeToGoldenWidget.gameObject.SetActive(false);
    }
    iTween.ScaleTo(this.m_switchPremiumButton.gameObject, this.m_cardCountTabHideScale, 0.4f);
    this.TellServerAboutWhatUserDid();
    this.IsInCraftingMode = false;
    return true;
  }

  public void CreateButtonPressed()
  {
    this.HideAndDestroyRelatedBigCard();
    if (this.m_craftingUI.m_buttonCreate.GetCraftingState() == CraftingButton.CraftingState.CreateUpgrade)
      this.ShowUpgradeToGoldenWidget();
    else if (this.m_craftingUI.m_buttonCreate.GetCraftingState() == CraftingButton.CraftingState.Upgrade)
    {
      if (!GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_SEEN_UTG_ALERT))
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
        info.m_offset = this.m_utgAlertPopupOffset;
        info.m_headerText = GameStrings.Format("GLUE_CRAFTING_UTG_ALERT_HEADER");
        info.m_text = GameStrings.Format("GLUE_CRAFTING_UTG_ALERT_BODY");
        info.m_confirmText = GameStrings.Format("GLUE_CRAFTING_UTG_ALERT_CONFIRM");
        info.m_cancelText = GameStrings.Format("GLUE_CRAFTING_UTG_ALERT_CANCEL");
        info.m_showAlertIcon = false;
        info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
        info.m_alertTextAlignment = UberText.AlignmentOptions.Center;
        AlertPopup.ResponseCallback responseCallback = (AlertPopup.ResponseCallback) ((response, userdata) =>
        {
          GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_SEEN_UTG_ALERT, true);
          this.SetCraftingRelatedActorsActiveForUpgradeToGoldenPopup(true);
          if (response != AlertPopup.Response.CONFIRM)
            return;
          this.m_craftingUI.DoCreate(true);
        });
        info.m_responseCallback = responseCallback;
        this.SetCraftingRelatedActorsActiveForUpgradeToGoldenPopup(false);
        DialogManager.Get().ShowPopup(info);
      }
      else
        this.m_craftingUI.DoCreate(true);
    }
    else
      this.m_craftingUI.DoCreate(false);
  }

  public void DisenchantButtonPressed()
  {
    this.HideAndDestroyRelatedBigCard();
    this.m_craftingUI.DoDisenchant();
  }

  public void UpdateBankText()
  {
    if (!((UnityEngine.Object) this.m_craftingUI != (UnityEngine.Object) null))
      return;
    this.m_craftingUI.UpdateBankText();
  }

  private void TellServerAboutWhatUserDid()
  {
    if ((UnityEngine.Object) this.GetCurrentActor() == (UnityEngine.Object) null)
      return;
    string cardId = this.m_pendingClientTransaction.CardID;
    TAG_PREMIUM premium = this.m_pendingClientTransaction.Premium;
    GameUtils.TranslateCardIdToDbId(cardId);
    if (this.m_pendingClientTransaction.HasPendingTransactions())
      this.m_pendingServerTransaction = this.m_pendingClientTransaction.ShallowCopy();
    int copiesInCollection1 = CollectionManager.Get().GetNumCopiesInCollection(cardId, TAG_PREMIUM.NORMAL);
    int copiesInCollection2 = CollectionManager.Get().GetNumCopiesInCollection(cardId, TAG_PREMIUM.GOLDEN);
    NetCache.CardValue cardValue1 = this.GetCardValue(cardId, premium);
    if (cardValue1 == null)
      return;
    if (cardValue1.IsOverrideActive() == this.m_pendingClientTransaction.CardValueOverridden)
    {
      if (this.m_pendingClientTransaction.HasPendingTransactions())
      {
        int expectedTransactionCost;
        switch (premium)
        {
          case TAG_PREMIUM.NORMAL:
            NetCache.CardValue cardValue2 = this.GetCardValue(cardId, TAG_PREMIUM.GOLDEN);
            expectedTransactionCost = this.m_pendingClientTransaction.GetExpectedTransactionCost(cardValue1, cardValue2);
            break;
          case TAG_PREMIUM.GOLDEN:
            expectedTransactionCost = this.m_pendingClientTransaction.GetExpectedTransactionCost(this.GetCardValue(cardId, TAG_PREMIUM.NORMAL), cardValue1);
            break;
          default:
            expectedTransactionCost = 0;
            break;
        }
        Network.Get().CraftingTransaction(this.m_pendingClientTransaction, expectedTransactionCost, copiesInCollection1, copiesInCollection2);
      }
    }
    else
      this.OnCardValueChangedError((Network.CardSaleResult) null);
    this.m_pendingClientTransaction = (CraftingPendingTransaction) null;
    this.ResetUnCommitedArcaneDustChanges();
    BnetBar.Get().RefreshCurrency();
  }

  public void OnCardGenericError(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_COLLECTION_GENERIC_ERROR"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  public void OnCardPermissionError(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_COLLECTION_CARD_PERMISSION_ERROR"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  public void OnCardDisenchantSoulboundError(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_COLLECTION_CARD_SOULBOUND"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  public void OnCardCountError(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_COLLECTION_GENERIC_ERROR"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  public void OnCardCraftingEventNotActiveError(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_COLLECTION_CARD_CRAFTING_EVENT_NOT_ACTIVE"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  public void OnCardUnknownError(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
      m_text = GameStrings.Format("GLUE_COLLECTION_CARD_UNKNOWN_ERROR", (object) sale.Action),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  public void OnCardValueChangedError(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_COLLECTION_CARD_VALUE_CHANGED_ERROR"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  public void OnCardDisenchanted(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    CollectionCardVisual cardVisual = CollectionManager.Get().GetCollectibleDisplay().GetPageManager().GetCardVisual(sale.AssetName, sale.Premium);
    if (!((UnityEngine.Object) cardVisual != (UnityEngine.Object) null) || !cardVisual.IsShown())
      return;
    cardVisual.OnDoneCrafting();
  }

  public void OnCardCreated(Network.CardSaleResult sale)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    CollectionCardVisual cardVisual = CollectionManager.Get().GetCollectibleDisplay().GetPageManager().GetCardVisual(sale.AssetName, sale.Premium);
    if (!((UnityEngine.Object) cardVisual != (UnityEngine.Object) null) || !cardVisual.IsShown())
      return;
    cardVisual.OnDoneCrafting();
    if (!TemporaryAccountManager.IsTemporaryAccount() || !((UnityEngine.Object) cardVisual.GetActor() != (UnityEngine.Object) null) || sale.Action != Network.CardSaleResult.SaleResult.CARD_WAS_BOUGHT)
      return;
    EntityDef entityDef = cardVisual.GetActor().GetEntityDef();
    if (entityDef == null || entityDef.GetRarity() != TAG_RARITY.EPIC && entityDef.GetRarity() != TAG_RARITY.LEGENDARY)
      return;
    TemporaryAccountManager.Get().ShowEarnCardEventHealUpDialog(TemporaryAccountManager.HealUpReason.CRAFT_CARD);
  }

  public void OnCardUpgraded(Network.CardSaleResult result)
  {
    this.m_pendingServerTransaction = (CraftingPendingTransaction) null;
    CollectiblePageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager();
    CollectionCardVisual cardVisual1 = pageManager.GetCardVisual(result.AssetName, TAG_PREMIUM.NORMAL);
    if ((UnityEngine.Object) cardVisual1 != (UnityEngine.Object) null && cardVisual1.IsShown())
      cardVisual1.OnDoneCrafting();
    CollectionCardVisual cardVisual2 = pageManager.GetCardVisual(result.AssetName, TAG_PREMIUM.GOLDEN);
    if (!((UnityEngine.Object) cardVisual2 != (UnityEngine.Object) null) || !cardVisual2.IsShown())
      return;
    cardVisual2.OnDoneCrafting();
  }

  public void LoadGhostActorIfNecessary()
  {
    if (this.m_cancellingCraftMode)
      return;
    if (this.GetNumOwnedIncludePending() > 0)
    {
      if ((UnityEngine.Object) this.m_upsideDownActor == (UnityEngine.Object) null)
      {
        this.m_currentBigActor = this.GetAndPositionNewActor(this.m_currentBigActor, 1);
        this.m_currentBigActor.name = "CurrentBigActor";
        this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
        this.m_currentBigActor.transform.localScale = this.m_floatingCardBone.localScale;
        this.SetBigActorLayer(true);
      }
      else
      {
        this.m_upsideDownActor.transform.parent = (Transform) null;
        this.m_currentBigActor = this.m_upsideDownActor;
        this.m_currentBigActor.name = "CurrentBigActor";
        this.m_currentBigActor.transform.position = this.m_faceDownCardBone.position;
        this.m_currentBigActor.transform.localScale = this.m_faceDownCardBone.localScale;
        this.m_upsideDownActor = (Actor) null;
      }
    }
    else
    {
      if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
      {
        Log.Crafting.Print("Deleting rogue m_upsideDownActor!");
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_upsideDownActor.gameObject);
      }
      this.m_currentBigActor = this.GetAndPositionNewActor(this.m_currentBigActor, 0);
      this.m_currentBigActor.name = "CurrentBigActor";
      this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
      this.m_currentBigActor.transform.localScale = this.m_floatingCardBone.localScale;
      iTween.ScaleTo(this.m_cardCountTab.gameObject, this.m_cardCountTabHideScale, 0.4f);
      this.m_cardCountTab.transform.position = new Vector3(0.0f, 307f, -10f);
      this.SetBigActorLayer(true);
    }
  }

  public Actor LoadNewActorAndConstructIt()
  {
    if (this.m_cancellingCraftMode)
      return (Actor) null;
    if (!this.m_isCurrentActorAGhost)
    {
      if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null)
      {
        this.m_currentBigActor = this.GetAndPositionNewActor(this.m_upsideDownActor, 0);
      }
      else
      {
        Actor currentBigActor = this.m_currentBigActor;
        this.m_currentBigActor = this.GetAndPositionNewActor(this.m_currentBigActor, 0);
        Debug.LogWarning((object) "Destroying unexpected m_currentBigActor to prevent a lost reference");
        UnityEngine.Object.Destroy((UnityEngine.Object) currentBigActor.gameObject);
      }
      this.m_isCurrentActorAGhost = false;
      this.m_currentBigActor.name = "CurrentBigActor";
      this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
      this.m_currentBigActor.transform.localScale = this.m_floatingCardBone.localScale;
      this.SetBigActorLayer(true);
    }
    SpellType spellType = SpellType.CONSTRUCT;
    EntityDef entityDef = this.m_collectionCardActor.GetEntityDef();
    if (entityDef != null && entityDef.HasClass(TAG_CLASS.DEATHKNIGHT) && entityDef.HasRuneCost)
      spellType = SpellType.DEATH_KNIGHT_CONSTRUCT;
    this.m_currentBigActor.ActivateSpellBirthState(spellType);
    return this.m_currentBigActor;
  }

  public void ForceNonGhostFlagOn() => this.m_isCurrentActorAGhost = false;

  public void FinishCreateAnims()
  {
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null || this.m_cancellingCraftMode)
      return;
    iTween.ScaleTo(this.m_cardCountTab.gameObject, this.m_cardCountTabShowScale, 0.4f);
    this.m_currentBigActor.GetSpell(SpellType.GHOSTMODE).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
    this.m_isCurrentActorAGhost = false;
    this.m_cardCountTab.UpdateText(this.GetNumOwnedIncludePending(), this.m_currentBigActor.GetPremium());
    this.m_cardCountTab.transform.position = this.GetCardCountPosition();
    this.ShowRelatedBigCard(this.m_currentBigActor.GetPremium());
  }

  public void FlipCurrentActor()
  {
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null || this.m_isCurrentActorAGhost)
      return;
    this.m_cardCountTab.transform.localScale = this.m_cardCountTabHideScale;
    if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_upsideDownActor was not null, destroying object to prevent lost reference");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_upsideDownActor.gameObject);
      this.m_upsideDownActor = (Actor) null;
    }
    this.m_upsideDownActor = this.m_currentBigActor;
    this.m_upsideDownActor.name = "UpsideDownActor";
    this.m_upsideDownActor.GetSpell(SpellType.GHOSTMODE).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
    this.m_currentBigActor = (Actor) null;
    iTween.Stop(this.m_upsideDownActor.gameObject);
    iTween.RotateTo(this.m_upsideDownActor.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 350f, 180f), (object) "time", (object) 1f));
    iTween.MoveTo(this.m_upsideDownActor.gameObject, iTween.Hash((object) "name", (object) nameof (FlipCurrentActor), (object) "position", (object) this.m_faceDownCardBone.position, (object) "time", (object) 1f));
    this.StartCoroutine(this.ReplaceFaceDownActorWithHiddenCard());
  }

  public void FinishFlipCurrentActorEarly()
  {
    this.StopAllCoroutines();
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
      iTween.Stop(this.m_currentBigActor.gameObject);
    if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
      iTween.Stop(this.m_upsideDownActor.gameObject);
    this.m_currentBigActor.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
    this.m_currentBigActor.Show();
    GameObject hiddenStandIn = this.m_currentBigActor.GetHiddenStandIn();
    if ((UnityEngine.Object) hiddenStandIn == (UnityEngine.Object) null)
      return;
    hiddenStandIn.SetActive(false);
    UnityEngine.Object.Destroy((UnityEngine.Object) hiddenStandIn);
  }

  public void FlipUpsideDownCard(Actor oldActor)
  {
    if (this.m_cancellingCraftMode)
      return;
    int ownedIncludePending = this.GetNumOwnedIncludePending();
    if (ownedIncludePending > 1)
    {
      this.m_upsideDownActor = this.GetAndPositionNewUpsideDownActor(this.m_currentBigActor, false);
      this.m_upsideDownActor.name = "UpsideDownActor";
      this.StartCoroutine(this.ReplaceFaceDownActorWithHiddenCard());
    }
    if (ownedIncludePending >= 1)
    {
      iTween.ScaleTo(this.m_cardCountTab.gameObject, this.m_cardCountTabShowScale, 0.4f);
      this.m_cardCountTab.transform.position = this.GetCardCountPosition();
      this.m_cardCountTab.UpdateText(ownedIncludePending, this.m_currentBigActor.GetPremium());
    }
    if (this.m_isCurrentActorAGhost)
      this.m_currentBigActor.gameObject.transform.position = this.m_floatingCardBone.position;
    else
      iTween.MoveTo(this.m_currentBigActor.gameObject, iTween.Hash((object) "name", (object) nameof (FlipUpsideDownCard), (object) "position", (object) this.m_floatingCardBone.position, (object) "time", (object) this.m_timeForCardToFlipUp, (object) "easetype", (object) this.m_easeTypeForCardFlip));
    iTween.RotateTo(this.m_currentBigActor.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) this.m_timeForCardToFlipUp, (object) "easetype", (object) this.m_easeTypeForCardFlip, (object) "oncomplete", (object) "OnCardFlipComplete", (object) "oncompletetarget", (object) this.gameObject));
    this.StartCoroutine(this.ReplaceHiddenCardwithRealActor(this.m_currentBigActor));
  }

  private IEnumerator ReplaceFaceDownActorWithHiddenCard()
  {
    while ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null && (double) this.m_upsideDownActor.transform.localEulerAngles.z < 90.0)
      yield return (object) null;
    if (!((UnityEngine.Object) this.m_upsideDownActor == (UnityEngine.Object) null))
    {
      GameObject standIn = UnityEngine.Object.Instantiate<GameObject>(this.m_hiddenActor.gameObject);
      standIn.GetComponent<Actor>().UpdateCardBack();
      standIn.transform.parent = this.m_upsideDownActor.transform;
      standIn.transform.localScale = new Vector3(1f, 1f, 1f);
      standIn.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      standIn.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      this.m_upsideDownActor.Hide();
      this.m_upsideDownActor.SetHiddenStandIn(standIn);
    }
  }

  private IEnumerator ReplaceHiddenCardwithRealActor(Actor actor)
  {
    while ((UnityEngine.Object) actor != (UnityEngine.Object) null && (double) actor.transform.localEulerAngles.z > 90.0 && (double) actor.transform.localEulerAngles.z < 270.0)
      yield return (object) null;
    if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
    {
      actor.Show();
      GameObject hiddenStandIn = actor.GetHiddenStandIn();
      if (!((UnityEngine.Object) hiddenStandIn == (UnityEngine.Object) null))
      {
        hiddenStandIn.SetActive(false);
        UnityEngine.Object.Destroy((UnityEngine.Object) hiddenStandIn);
      }
    }
  }

  private void OnCardFlipComplete()
  {
    if (!(bool) (UnityEngine.Object) this.m_craftingUI || this.m_craftingUI.GetIsAnimating())
      return;
    this.ShowRelatedBigCard(this.m_currentBigActor.GetPremium());
  }

  public CraftingPendingTransaction GetPendingClientTransaction() => this.m_pendingClientTransaction;

  public CraftingPendingTransaction GetPendingServerTransaction() => this.m_pendingServerTransaction;

  public void ShowCraftingUI(UIEvent e)
  {
    if (this.m_craftingUI.IsEnabled())
      this.m_craftingUI.Disable(this.m_hideCraftingUIBone.position);
    else
      this.m_craftingUI.Enable(this.m_showCraftingUIBone.position, this.m_hideCraftingUIBone.position);
  }

  public void SetCraftingUIActive(bool active)
  {
    if (!((UnityEngine.Object) this.m_craftingUI != (UnityEngine.Object) null))
      return;
    this.m_craftingUI.gameObject.SetActive(active);
  }

  public void ShowUpgradeToGoldenWidget()
  {
    if (!((UnityEngine.Object) this.m_upgradeToGoldenWidget != (UnityEngine.Object) null))
      return;
    this.m_upgradeToGoldenWidget.gameObject.SetActive(true);
    UpgradeToGoldenPopup componentInChildren = this.m_upgradeToGoldenWidget.GetComponentInChildren<UpgradeToGoldenPopup>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
    {
      componentInChildren.SetInfo(this.m_pendingClientTransaction, this.m_craftingUI, this.m_showUpgradeToGoldenPopupBone);
      this.StartCoroutine(componentInChildren.ShowWhenReadyRoutine());
    }
    this.m_upgradeToGoldenWidgetShown = true;
  }

  public void HideUpgradeToGoldenWidget()
  {
    if (!((UnityEngine.Object) this.m_upgradeToGoldenWidget != (UnityEngine.Object) null))
      return;
    UpgradeToGoldenPopup componentInChildren = this.m_upgradeToGoldenWidget.GetComponentInChildren<UpgradeToGoldenPopup>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.OnHide();
    this.m_upgradeToGoldenWidgetShown = false;
  }

  public void SetCraftingRelatedActorsActiveForUpgradeToGoldenPopup(bool active)
  {
    if (!this.HasUpgradeToGoldenEnabled())
      return;
    if ((UnityEngine.Object) this.m_craftingUI != (UnityEngine.Object) null)
    {
      if (active)
      {
        this.m_craftingUI.m_buttonDisenchant.GetComponent<Collider>().enabled = true;
        this.m_craftingUI.m_buttonCreate.GetComponent<Collider>().enabled = true;
        if (this.GetNumOwnedIncludePending() == 0)
          this.LoadGhostActorIfNecessary();
        this.m_currentBigActor.Show();
        this.m_craftingUI.Enable(this.m_showCraftingUIBone.position, this.m_hideCraftingUIBone.position);
      }
      else
      {
        this.FinishCreateAnims();
        this.m_craftingUI.Disable(this.m_hideCraftingUIBone.position);
      }
    }
    if ((UnityEngine.Object) this.m_cardInfoPane != (UnityEngine.Object) null)
      this.m_cardInfoPane.gameObject.SetActive(active);
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
      this.m_currentBigActor.gameObject.SetActive(active);
    if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
      this.m_upsideDownActor.gameObject.SetActive(active);
    if (!((UnityEngine.Object) this.m_cardCountTab != (UnityEngine.Object) null))
      return;
    this.m_cardCountTab.gameObject.SetActive(active);
  }

  private Actor GetCurrentActor()
  {
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
      return this.m_currentBigActor;
    return (UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null ? this.m_upsideDownActor : (Actor) null;
  }

  private void MoveCardToBigSpot(Actor collectionCardActor, TAG_PREMIUM premium)
  {
    if ((UnityEngine.Object) collectionCardActor == (UnityEngine.Object) null)
      return;
    EntityDef entityDef = collectionCardActor.GetEntityDef();
    if (entityDef == null)
      return;
    int ownedIncludePending = this.GetNumOwnedIncludePending(entityDef.GetCardId(), new TAG_PREMIUM?(premium));
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_currentBigActor was not null, destroying object before we lose the reference");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentBigActor.gameObject);
      this.m_currentBigActor = (Actor) null;
    }
    this.m_currentBigActor = this.GetAndPositionNewActor(this.m_cardActors.GetActor(premium), ownedIncludePending);
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "CraftingManager.MoveCardToBigSpot - GetAndPositionNewActor returned null");
    }
    else
    {
      this.m_currentBigActor.name = "CurrentBigActor";
      this.m_craftSourcePosition = collectionCardActor.transform.position;
      this.m_craftSourceScale = collectionCardActor.transform.lossyScale;
      this.m_craftSourceScale = Vector3.one * Mathf.Min(this.m_craftSourceScale.x, this.m_craftSourceScale.y, this.m_craftSourceScale.z);
      this.m_currentBigActor.transform.position = this.m_craftSourcePosition;
      TransformUtil.SetWorldScale((Component) this.m_currentBigActor, this.m_craftSourceScale);
      this.SetBigActorLayer(true);
      this.m_currentBigActor.ToggleForceIdle(true);
      this.m_currentBigActor.SetActorState(ActorStateType.CARD_IDLE);
      if (entityDef.IsHeroSkin())
      {
        this.m_cardCountTab.gameObject.SetActive(false);
      }
      else
      {
        this.m_cardCountTab.gameObject.SetActive(true);
        if (ownedIncludePending > 1)
        {
          if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
          {
            Debug.LogError((object) "m_upsideDownActor was not null, destroying object before we lose the reference");
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_upsideDownActor.gameObject);
            this.m_upsideDownActor = (Actor) null;
          }
          this.m_upsideDownActor = this.GetAndPositionNewUpsideDownActor(collectionCardActor, true);
          this.m_upsideDownActor.name = "UpsideDownActor";
          this.StartCoroutine(this.ReplaceFaceDownActorWithHiddenCard());
        }
        if (ownedIncludePending > 0)
        {
          this.m_cardCountTab.UpdateText(ownedIncludePending, premium);
          this.m_cardCountTab.transform.position = new Vector3(collectionCardActor.transform.position.x, collectionCardActor.transform.position.y - 2f, collectionCardActor.transform.position.z);
        }
      }
      this.FinishBigCardMove();
    }
  }

  private string GetRelatedCardId(EntityDef def)
  {
    int tag1 = def.GetTag(GAME_TAG.COLLECTION_RELATED_CARD_DATABASE_ID);
    CardDbfRecord record = GameDbf.Card.GetRecord(tag1);
    if (record != null)
      return record.NoteMiniGuid;
    if (def.IsHero())
      return GameUtils.GetHeroPowerCardIdFromHero(def.GetCardId());
    if (def.IsQuest())
    {
      int tag2 = def.GetTag(GAME_TAG.QUEST_REWARD_DATABASE_ID);
      return GameDbf.Card.GetRecord(tag2)?.NoteMiniGuid;
    }
    if (!def.IsQuestline())
      return (string) null;
    int tag3 = def.GetTag(GAME_TAG.QUESTLINE_FINAL_REWARD_DATABASE_ID);
    return GameDbf.Card.GetRecord(tag3)?.NoteMiniGuid;
  }

  private void ShowRelatedBigCard(TAG_PREMIUM premium)
  {
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unexpected error in ShowRelatedBigCard. Current big actor was null");
    }
    else
    {
      EntityDef entityDef1 = this.m_currentBigActor.GetEntityDef();
      if (entityDef1 == null)
      {
        Debug.LogError((object) "Unexpected error in ShowRelatedBigCard. Current big actor's entity def was null");
      }
      else
      {
        string relatedCardId = this.GetRelatedCardId(entityDef1);
        if (string.IsNullOrEmpty(relatedCardId))
          return;
        int ownedIncludePending = this.GetNumOwnedIncludePending();
        Actor templateActorForType = this.GetTemplateActorForType(entityDef1.GetCardType(), premium);
        if (templateActorForType.GetEntityDef() == null || templateActorForType.GetEntityDef().GetCardId() != relatedCardId || templateActorForType.GetPremium() != premium || premium == TAG_PREMIUM.SIGNATURE)
        {
          using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(relatedCardId, this.m_currentBigActor.CardPortraitQuality))
          {
            templateActorForType.SetEntityDef(fullDef.EntityDef);
            if (premium == TAG_PREMIUM.SIGNATURE)
              premium = templateActorForType.HasSignaturePortraitTexture() ? TAG_PREMIUM.SIGNATURE : TAG_PREMIUM.GOLDEN;
            templateActorForType.SetPremium(premium);
          }
        }
        if ((UnityEngine.Object) this.m_currentRelatedCardActor != (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "Current related card actor was not new when creating a new one. Ensure we cleanup this actor");
          this.HideAndDestroyRelatedBigCard();
        }
        this.m_currentRelatedCardActor = this.GetAndPositionNewActor(templateActorForType, ownedIncludePending);
        if (entityDef1.IsQuest() || entityDef1.IsQuestline())
        {
          EntityDef entityDef2 = DefLoader.Get().GetEntityDef(relatedCardId);
          if (entityDef2 != null)
          {
            bool isPremium = this.m_currentRelatedCardActor.GetPremium() == TAG_PREMIUM.GOLDEN;
            QuestCardRewardOverlay cardRewardOverlay = this.AddQuestOverlay(entityDef2, isPremium, this.m_currentRelatedCardActor.gameObject);
            if (entityDef1.IsQuestline() && cardRewardOverlay != null)
              cardRewardOverlay.EnableRewardObjects();
          }
        }
        LayerUtils.SetLayer(this.m_currentRelatedCardActor.gameObject, GameLayer.IgnoreFullScreenEffects);
        this.m_currentRelatedCardActor.gameObject.transform.parent = this.m_currentBigActor.transform;
        this.StartCoroutine(this.RevealRelatedCard(this.m_currentRelatedCardActor));
      }
    }
  }

  private IEnumerator RevealRelatedCard(Actor actor)
  {
    if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
    {
      Spell ghostSpell = actor.GetSpellIfLoaded(SpellType.GHOSTMODE);
      if ((UnityEngine.Object) ghostSpell != (UnityEngine.Object) null)
      {
        while (!ghostSpell.IsFinished())
          yield return (object) null;
      }
      if (!((UnityEngine.Object) actor.gameObject == (UnityEngine.Object) null))
      {
        actor.Show();
        GameObject gameObject = actor.gameObject;
        Transform transform = gameObject.transform;
        transform.localPosition = (Vector3) CraftingManager.HERO_POWER_START_POSITION;
        transform.localScale = (Vector3) CraftingManager.HERO_POWER_START_SCALE;
        iTween.MoveTo(gameObject, iTween.Hash((object) "position", (object) CraftingManager.HERO_POWER_POSITION.Value, (object) "isLocal", (object) true, (object) "time", (object) CraftingManager.HERO_POWER_TWEEN_TIME));
        iTween.ScaleTo(gameObject, iTween.Hash((object) "scale", (object) CraftingManager.HERO_POWER_SCALE.Value, (object) "isLocal", (object) true, (object) "time", (object) CraftingManager.HERO_POWER_TWEEN_TIME));
      }
    }
  }

  public void HideAndDestroyRelatedBigCard()
  {
    if ((UnityEngine.Object) this.m_currentRelatedCardActor == (UnityEngine.Object) null)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentRelatedCardActor.gameObject);
    this.m_currentRelatedCardActor = (Actor) null;
  }

  private void FinishBigCardMove()
  {
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null)
      return;
    int ownedIncludePending = this.GetNumOwnedIncludePending();
    SoundManager.Get().LoadAndPlay((AssetReference) "Card_Transition_Out.prefab:aecf5b5837772844b9d2db995744df82");
    iTween.MoveTo(this.m_currentBigActor.gameObject, iTween.Hash((object) "name", (object) nameof (FinishBigCardMove), (object) "position", (object) this.m_floatingCardBone.position, (object) "time", (object) 0.4f, (object) "oncomplete", (object) "FinishActorMoveTowardsScreen", (object) "oncompletetarget", (object) this.gameObject));
    iTween.ScaleTo(this.m_currentBigActor.gameObject, iTween.Hash((object) "scale", (object) this.m_floatingCardBone.localScale, (object) "time", (object) 0.4f, (object) "easetype", (object) iTween.EaseType.easeOutQuad));
    if (ownedIncludePending <= 0)
      return;
    this.m_cardCountTab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    iTween.MoveTo(this.m_cardCountTab.gameObject, this.GetCardCountPosition(), 0.4f);
    iTween.ScaleTo(this.m_cardCountTab.gameObject, this.m_cardCountTabShowScale, 0.4f);
  }

  private void UpdateCardInfoPane()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null)
      Debug.LogError((object) "CraftingManager.UpdateCardInfoPane -  m_currentBigActor is null");
    else if ((UnityEngine.Object) this.m_cardInfoPane == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "CraftingManager.UpdateCardInfoPane - m_cardInfoPane is null");
    }
    else
    {
      this.m_cardInfoPane.gameObject.SetActive(true);
      this.m_cardInfoPane.UpdateContent();
      this.m_cardInfoPane.transform.position = this.m_currentBigActor.transform.position - new Vector3(0.0f, 1f, 0.0f);
      Vector3 localScale = this.m_cardInfoPaneBone.localScale;
      this.m_cardInfoPane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
      iTween.MoveTo(this.m_cardInfoPane.gameObject, this.m_cardInfoPaneBone.position, 0.5f);
      iTween.ScaleTo(this.m_cardInfoPane.gameObject, localScale, 0.5f);
    }
  }

  private void FinishActorMoveTowardsScreen()
  {
    if (!(bool) (UnityEngine.Object) this.m_craftingUI || this.m_craftingUI.GetIsAnimating())
      return;
    this.ShowRelatedBigCard(this.m_currentBigActor.GetPremium());
  }

  private void FinishActorMoveAway()
  {
    this.m_cancellingCraftMode = false;
    iTween.Stop(this.m_cardCountTab.gameObject);
    this.m_cardCountTab.transform.position = new Vector3(0.0f, 307f, -10f);
    if ((UnityEngine.Object) this.m_upsideDownActor != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_upsideDownActor.gameObject);
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_currentBigActor.gameObject);
    this.LoadRandomCardBack();
  }

  private void FadeEffectsIn() => this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
  {
    Blur = new BlurParameters(brightness: 1f)
  });

  private void FadeEffectsOut() => this.m_screenEffectsHandle.StopEffect();

  private void OnVignetteFinished()
  {
    this.SetBigActorLayer(false);
    if ((UnityEngine.Object) this.GetCurrentCardVisual() != (UnityEngine.Object) null)
      this.GetCurrentCardVisual().OnDoneCrafting();
    if ((UnityEngine.Object) this.m_currentBigActor != (UnityEngine.Object) null)
    {
      this.m_currentBigActor.name = "USED_TO_BE_CurrentBigActor";
      this.StartCoroutine(this.MakeSureActorIsCleanedUp(this.m_currentBigActor));
    }
    this.m_currentBigActor = (Actor) null;
    this.m_craftingUI.gameObject.SetActive(false);
  }

  private IEnumerator MakeSureActorIsCleanedUp(Actor oldActor)
  {
    yield return (object) new WaitForSeconds(1f);
    if (!((UnityEngine.Object) oldActor == (UnityEngine.Object) null))
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) oldActor);
  }

  private Actor GetAndPositionNewUpsideDownActor(Actor oldActor, bool fromPage)
  {
    Actor positionNewActor = this.GetAndPositionNewActor(oldActor, 1);
    LayerUtils.SetLayer(positionNewActor.gameObject, GameLayer.IgnoreFullScreenEffects);
    if (fromPage)
    {
      positionNewActor.transform.position = oldActor.transform.position + new Vector3(0.0f, -2f, 0.0f);
      positionNewActor.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
      iTween.RotateTo(positionNewActor.gameObject, new Vector3(0.0f, 350f, 180f), 0.4f);
      iTween.MoveTo(positionNewActor.gameObject, iTween.Hash((object) "name", (object) nameof (GetAndPositionNewUpsideDownActor), (object) "position", (object) this.m_faceDownCardBone.position, (object) "time", (object) 0.4f));
      iTween.ScaleTo(positionNewActor.gameObject, this.m_faceDownCardBone.localScale, 0.4f);
    }
    else
    {
      positionNewActor.transform.localEulerAngles = new Vector3(0.0f, 350f, 180f);
      positionNewActor.transform.position = this.m_faceDownCardBone.position + new Vector3(0.0f, -6f, 0.0f);
      positionNewActor.transform.localScale = this.m_faceDownCardBone.localScale;
      iTween.MoveTo(positionNewActor.gameObject, iTween.Hash((object) "name", (object) nameof (GetAndPositionNewUpsideDownActor), (object) "position", (object) this.m_faceDownCardBone.position, (object) "time", (object) this.m_timeForBackCardToMoveUp, (object) "easetype", (object) this.m_easeTypeForCardMoveUp, (object) "delay", (object) this.m_delayBeforeBackCardMovesUp));
    }
    return positionNewActor;
  }

  private Actor GetAndPositionNewActor(Actor oldActor, int numCopies)
  {
    Actor positionNewActor = numCopies != 0 ? this.GetNonGhostActor(oldActor) : this.GetGhostActor(oldActor);
    if ((UnityEngine.Object) positionNewActor != (UnityEngine.Object) null)
      positionNewActor.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    return positionNewActor;
  }

  private Actor GetGhostActor(Actor actor)
  {
    this.m_isCurrentActorAGhost = true;
    bool flag1 = actor.GetPremium() == TAG_PREMIUM.GOLDEN;
    bool flag2 = actor.GetPremium() == TAG_PREMIUM.SIGNATURE;
    bool flag3 = actor.GetPremium() == TAG_PREMIUM.DIAMOND;
    Actor templateActor = this.m_ghostMinionActor;
    switch (actor.GetEntityDef().GetCardType())
    {
      case TAG_CARDTYPE.HERO:
        templateActor = !flag1 ? this.m_ghostHeroActor : this.m_ghostGoldenHeroActor;
        break;
      case TAG_CARDTYPE.MINION:
        templateActor = !flag1 ? (!flag2 ? (!flag3 ? this.m_ghostMinionActor : this.m_ghostDiamondMinionActor) : this.m_ghostSignatureMinionActor) : this.m_ghostGoldenMinionActor;
        break;
      case TAG_CARDTYPE.SPELL:
        templateActor = !flag1 ? this.m_ghostSpellActor : this.m_ghostGoldenSpellActor;
        break;
      case TAG_CARDTYPE.WEAPON:
        templateActor = !flag1 ? this.m_ghostWeaponActor : this.m_ghostGoldenWeaponActor;
        break;
      case TAG_CARDTYPE.HERO_POWER:
        templateActor = !flag1 ? this.m_ghostHeroPowerActor : this.m_ghostGoldenHeroPowerActor;
        break;
      case TAG_CARDTYPE.LOCATION:
        templateActor = !flag1 ? this.m_ghostLocationActor : this.m_ghostGoldenLocationActor;
        break;
      default:
        Debug.LogError((object) "CraftingManager.GetGhostActor() - tried to get a ghost actor for a cardtype that we haven't anticipated!!");
        break;
    }
    return this.SetUpGhostActor(templateActor, actor);
  }

  private Actor GetNonGhostActor(Actor actor)
  {
    this.m_isCurrentActorAGhost = false;
    return this.SetUpNonGhostActor(this.GetTemplateActor(actor), actor);
  }

  private Actor GetTemplateActorForType(TAG_CARDTYPE type, TAG_PREMIUM premium)
  {
    bool flag1 = premium == TAG_PREMIUM.GOLDEN;
    bool flag2 = premium == TAG_PREMIUM.SIGNATURE;
    bool flag3 = premium == TAG_PREMIUM.DIAMOND;
    switch (type)
    {
      case TAG_CARDTYPE.HERO:
        return flag1 ? this.m_templateGoldenHeroActor : this.m_templateHeroActor;
      case TAG_CARDTYPE.MINION:
        if (flag1)
          return this.m_templateGoldenMinionActor;
        if (flag2)
          return this.m_templateSignatureMinionActor;
        return flag3 ? this.m_templateDiamondMinionActor : this.m_templateMinionActor;
      case TAG_CARDTYPE.SPELL:
        return flag1 ? this.m_templateGoldenSpellActor : this.m_templateSpellActor;
      case TAG_CARDTYPE.WEAPON:
        return flag1 ? this.m_templateGoldenWeaponActor : this.m_templateWeaponActor;
      case TAG_CARDTYPE.HERO_POWER:
        return flag1 ? this.m_templateGoldenHeroPowerActor : this.m_templateHeroPowerActor;
      case TAG_CARDTYPE.LOCATION:
        return flag1 ? this.m_templateGoldenLocationActor : this.m_templateLocationActor;
      default:
        Debug.LogError((object) "CraftingManager.GetTemplateActorForType() - tried to get a actor for a cardtype that we haven't anticipated!!");
        return this.m_templateMinionActor;
    }
  }

  private Actor GetTemplateActor(Actor actor) => this.GetTemplateActorForType(actor.GetEntityDef().GetCardType(), actor.GetPremium());

  private Actor SetUpNonGhostActor(Actor templateActor, Actor actor)
  {
    Actor actor1 = UnityEngine.Object.Instantiate<Actor>(templateActor);
    actor1.SetFullDefFromActor(actor);
    actor1.SetPremium(actor.GetPremium());
    actor1.SetUnlit();
    actor1.UpdateAllComponents();
    return actor1;
  }

  private Actor SetUpGhostActor(Actor templateActor, Actor actor)
  {
    if ((UnityEngine.Object) templateActor == (UnityEngine.Object) null || (UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "CraftingManager.SetUpGhostActor - passed arguments are null");
      return (Actor) null;
    }
    Actor actorToShow = UnityEngine.Object.Instantiate<Actor>(templateActor);
    actorToShow.SetFullDefFromActor(actor);
    actorToShow.SetPremium(actor.GetPremium());
    actorToShow.UpdateAllComponents();
    actorToShow.UpdatePortraitTexture();
    actorToShow.UpdateCardColor();
    actorToShow.SetUnlit();
    actorToShow.Hide();
    if (actor.isMissingCard())
      actorToShow.ActivateSpellBirthState(SpellType.MISSING_BIGCARD);
    else
      actorToShow.ActivateSpellBirthState(SpellType.GHOSTMODE);
    this.StartCoroutine(this.ShowAfterTwoFrames(actorToShow));
    return actorToShow;
  }

  private IEnumerator ShowAfterTwoFrames(Actor actorToShow)
  {
    yield return (object) new WaitForEndOfFrame();
    yield return (object) new WaitForEndOfFrame();
    if (!((UnityEngine.Object) actorToShow != (UnityEngine.Object) this.m_currentBigActor))
      actorToShow.Show();
  }

  private void SetBigActorLayer(bool inCraftingMode)
  {
    if ((UnityEngine.Object) this.m_currentBigActor == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(this.m_currentBigActor.gameObject, inCraftingMode ? GameLayer.IgnoreFullScreenEffects : GameLayer.CardRaycast);
  }

  private CollectionCardVisual GetCurrentCardVisual()
  {
    EntityDef entityDef;
    TAG_PREMIUM premium;
    return !this.GetShownCardInfo(out entityDef, out premium) ? (CollectionCardVisual) null : CollectionManager.Get().GetCollectibleDisplay().GetPageManager().GetCardVisual(entityDef.GetCardId(), premium);
  }

  public int GetNumOwnedIncludePending(TAG_PREMIUM? premium) => this.GetNumOwnedIncludePending(this.m_collectionCardActor.GetEntityDef().GetCardId(), premium);

  public int GetNumOwnedIncludePending(string cardId, TAG_PREMIUM? premium)
  {
    this.m_collectionCardActor.GetEntityDef();
    int ownedIncludePending = !premium.HasValue ? CollectionManager.Get().GetTotalNumCopiesInCollection(cardId) : CollectionManager.Get().GetNumCopiesInCollection(cardId, premium.Value);
    if (this.IsPendingTransactionForCard(cardId))
    {
      if (!premium.HasValue)
        ownedIncludePending = ownedIncludePending + this.m_pendingClientTransaction.GetTransactionAmount(TAG_PREMIUM.NORMAL) + this.m_pendingClientTransaction.GetTransactionAmount(TAG_PREMIUM.GOLDEN);
      else
        ownedIncludePending += this.m_pendingClientTransaction.GetTransactionAmount(premium.Value);
    }
    return ownedIncludePending;
  }

  public int GetNumOwnedIncludePending() => this.m_pendingClientTransaction != null ? this.GetNumOwnedIncludePending(new TAG_PREMIUM?(this.m_pendingClientTransaction.Premium)) : this.GetNumOwnedIncludePending(new TAG_PREMIUM?(this.m_collectionCardActor.GetPremium()));

  public bool IsPendingTransactionForCard(string cardId) => this.m_pendingClientTransaction != null && (this.m_pendingClientTransaction.CardID == cardId || GameUtils.IsClassicCard(cardId) && this.m_pendingClientTransaction.CardID == GameUtils.TranslateDbIdToCardId(this.m_collectionCardActor.GetEntityDef().GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID)));

  private QuestCardRewardOverlay AddQuestOverlay(
    EntityDef def,
    bool isPremium,
    GameObject parent)
  {
    if ((UnityEngine.Object) this.m_questCardRewardOverlay == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Attempted to add quest overlay to a card, but no prefab was set on CraftinManager");
      return (QuestCardRewardOverlay) null;
    }
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_questCardRewardOverlay.gameObject, parent.transform);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Could not instantiate a new quest reward overlay from prefab");
      return (QuestCardRewardOverlay) null;
    }
    QuestCardRewardOverlay component = gameObject.GetComponent<QuestCardRewardOverlay>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Newly instantiated quest reward overlay game object does not contain a QuestCardRewardOverlay component.");
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      return (QuestCardRewardOverlay) null;
    }
    component.SetEntityType(def, isPremium);
    return component;
  }

  private void LoadActor(string actorPath, ref Actor actor)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) actorPath, AssetLoadingOptions.IgnorePrefabPosition);
    gameObject.transform.position = new Vector3(-99999f, 99999f, 99999f);
    actor = gameObject.GetComponent<Actor>();
    actor.TurnOffCollider();
  }

  private void LoadActor(string actorPath, ref Actor actor, ref Actor actorCopy)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) actorPath, AssetLoadingOptions.IgnorePrefabPosition);
    gameObject.transform.position = new Vector3(-99999f, 99999f, 99999f);
    actor = gameObject.GetComponent<Actor>();
    actorCopy = UnityEngine.Object.Instantiate<Actor>(actor);
    actor.TurnOffCollider();
    actorCopy.TurnOffCollider();
  }

  private void ShowLeagueLockedCardPopup()
  {
    EntityDef entityDef;
    if (!this.GetShownCardInfo(out entityDef, out TAG_PREMIUM _) || !RankMgr.Get().IsCardLockedInCurrentLeague(entityDef))
      return;
    LeagueDbfRecord standardLeagueConfig = RankMgr.Get().GetLocalPlayerStandardLeagueConfig();
    if (string.IsNullOrEmpty((string) standardLeagueConfig.LockedCardPopupTitleText) || string.IsNullOrEmpty((string) standardLeagueConfig.LockedCardPopupBodyText))
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = (string) standardLeagueConfig.LockedCardPopupTitleText,
      m_text = (string) standardLeagueConfig.LockedCardPopupBodyText,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_layerToUse = new GameLayer?(GameLayer.UI),
      m_showAlertIcon = false
    });
  }

  private void LoadRandomCardBack() => CardBackManager.Get().LoadRandomCardBackIntoFavoriteSlot(true);

  private Vector3 GetCardCountPosition() => this.m_currentBigActor.GetPremium() == TAG_PREMIUM.SIGNATURE ? this.m_signatureCardCounterBone.position : this.m_cardCounterBone.position;
}
