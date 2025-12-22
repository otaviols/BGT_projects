using Blizzard.T5.Core.Utils;
using Blizzard.Telemetry.WTCG.Client;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditClass]
public class CollectionCardVisual : PegUIElement
{
  public CollectionCardCount m_count;
  public CollectionCardLock m_cardLock;
  public GameObject m_newCardCallout;
  public Vector3 m_boxColliderCenter = new Vector3(0.0f, 0.14f, 0.0f);
  public Vector3 m_boxColliderSize = new Vector3(2f, 0.21f, 2.7f);
  public Vector3 m_heroSkinBoxColliderCenter = new Vector3(0.0f, 0.14f, -0.58f);
  public Vector3 m_heroSkinBoxColliderSize = new Vector3(2f, 0.21f, 2f);
  [CustomEditField(Sections = "Diamond")]
  public Vector3_MobileOverride m_diamondScale;
  [CustomEditField(Sections = "Diamond")]
  public Vector3_MobileOverride m_diamondPositionOffset;
  [CustomEditField(Sections = "Signature")]
  public Vector3_MobileOverride m_signatureScale;
  [CustomEditField(Sections = "Signature")]
  public Vector3_MobileOverride m_signaturePositionOffset;
  private const string ADD_CARD_TO_DECK_SOUND = "collection_manager_card_add_to_deck_instant.prefab:06df359c4026d7e47b06a4174f33e3ef";
  private const string CARD_LIMIT_UNLOCK_SOUND = "card_limit_unlock.prefab:83ffc974654bdd84f84ecbbaf7ba8e5e";
  private const string CARD_LIMIT_LOCK_SOUND = "card_limit_lock.prefab:68e3525ae3fa8634ab19fde893d7e15b";
  private const string CARD_MOUSE_OVER_SOUND = "collection_manager_card_mouse_over.prefab:0d4e20bc78956bc48b5e2963ec39211c";
  private const string CARD_MOVE_INVALID_OR_CLICK_SOUND = "collection_manager_card_move_invalid_or_click.prefab:777caa6f44f027747a03f3d85bcc897c";
  private CollectionCardActors m_actors;
  private CollectionCardVisual.LockType m_lockType;
  private bool m_shown;
  private CollectionUtils.ViewMode m_visualType;
  private int m_cmRow;
  private bool m_lastClickLeft;
  private Transform m_clickedActorTransform;
  private Vector3 m_originalScale;
  private Vector3 m_currentPositionOffset = Vector3.zero;
  private bool m_isScaled;
  private TAG_PREMIUM m_cardVisualPremium;
  private List<Renderer> m_cacheActorRenderers;

  public static event Action<CollectionCardVisual> CollectionCardOver;

  public static event Action<CollectionCardVisual> CollectionCardOut;

  public static event Action<CollectionCardVisual> CollectionCardReleased;

  public string CardId
  {
    get
    {
      if (this.m_actors == null)
        return string.Empty;
      Actor preferredActor = this.m_actors.GetPreferredActor();
      if ((UnityEngine.Object) preferredActor == (UnityEngine.Object) null)
        return string.Empty;
      EntityDef entityDef = preferredActor.GetEntityDef();
      return entityDef == null ? string.Empty : entityDef.GetCardId();
    }
  }

  public TAG_PREMIUM Premium
  {
    get
    {
      if (this.m_actors == null)
        return TAG_PREMIUM.NORMAL;
      Actor preferredActor = this.m_actors.GetPreferredActor();
      return (UnityEngine.Object) preferredActor == (UnityEngine.Object) null ? TAG_PREMIUM.NORMAL : preferredActor.GetPremium();
    }
  }

  public Vector3 GetRuneBannerPosition()
  {
    Actor actor = this.GetActor();
    if (!(bool) (UnityEngine.Object) actor)
      return this.transform.position;
    CardRuneBanner runeBanner = actor.GetRuneBanner();
    return !((UnityEngine.Object) runeBanner == (UnityEngine.Object) null) ? runeBanner.transform.position : Vector3.zero;
  }

  protected override void Awake()
  {
    base.Awake();
    if ((UnityEngine.Object) this.gameObject.GetComponent<AudioSource>() == (UnityEngine.Object) null)
      this.gameObject.AddComponent<AudioSource>();
    this.SetDragTolerance(5f);
    SoundManager.Get().Load((AssetReference) "collection_manager_card_add_to_deck_instant.prefab:06df359c4026d7e47b06a4174f33e3ef");
  }

  public bool IsShown() => this.m_shown;

  public void ShowLock(CollectionCardVisual.LockType type) => this.ShowLock(type, (string) null, false);

  public void ShowLock(CollectionCardVisual.LockType lockType, string reason, bool playSound)
  {
    CollectionCardVisual.LockType lockType1 = this.m_lockType;
    this.m_lockType = lockType;
    this.UpdateCardCountVisibility();
    if (this.m_actors == null)
      return;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    if ((UnityEngine.Object) this.m_cardLock != (UnityEngine.Object) null)
      this.m_cardLock.UpdateLockVisual(preferredActor, lockType, reason);
    if (!playSound)
      return;
    if (this.m_lockType == CollectionCardVisual.LockType.NONE && lockType1 != CollectionCardVisual.LockType.NONE)
      SoundManager.Get().LoadAndPlay((AssetReference) "card_limit_unlock.prefab:83ffc974654bdd84f84ecbbaf7ba8e5e");
    if (this.m_lockType == CollectionCardVisual.LockType.NONE || lockType1 != CollectionCardVisual.LockType.NONE)
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) "card_limit_lock.prefab:68e3525ae3fa8634ab19fde893d7e15b");
  }

  public void OnDoneCrafting() => this.UpdateCardCount();

  private void HidePreferredActorIfNecessary(CollectionCardActors actors)
  {
    if (actors == null)
      return;
    Actor preferredActor = actors.GetPreferredActor();
    if (!((UnityEngine.Object) preferredActor != (UnityEngine.Object) null) || !((UnityEngine.Object) preferredActor.transform.parent == (UnityEngine.Object) this.transform))
      return;
    if (preferredActor.GetEntityDef() != null)
      preferredActor.ReleaseCardDef();
    preferredActor.Hide();
  }

  public void SetActors(CollectionCardActors actors, CollectionUtils.ViewMode type = CollectionUtils.ViewMode.CARDS)
  {
    this.HidePreferredActorIfNecessary(this.m_actors);
    this.m_actors = actors;
    this.UpdateCardCount();
    this.m_visualType = type;
    if (actors == null)
      return;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    this.HidePreferredActorIfNecessary(this.m_actors);
    if ((UnityEngine.Object) preferredActor == (UnityEngine.Object) null)
      return;
    GameUtils.SetParent((Component) preferredActor, (Component) this);
    this.ShowNewCardCallout(preferredActor.GetActorStateMgr().GetActiveStateType() == ActorStateType.CARD_RECENTLY_ACQUIRED);
  }

  public Actor GetActor() => this.m_actors == null ? (Actor) null : this.m_actors.GetPreferredActor();

  public CollectionCardActors GetCollectionCardActors() => this.m_actors;

  public CollectionUtils.ViewMode GetVisualType() => this.m_visualType;

  public void SetCMRow(int rowNum) => this.m_cmRow = rowNum;

  public int GetCMRow() => this.m_cmRow;

  public static void ShowActorShadow(Actor actor, bool show)
  {
    string tag1 = "FakeShadow";
    string tag2 = "FakeShadowUnique";
    GameObject childByTag1 = GameObjectUtils.FindChildByTag(actor.gameObject, tag1);
    GameObject childByTag2 = GameObjectUtils.FindChildByTag(actor.gameObject, tag2);
    EntityDef entityDef = actor.GetEntityDef();
    if (show)
    {
      if (entityDef != null && entityDef.IsElite())
      {
        if ((UnityEngine.Object) childByTag1 != (UnityEngine.Object) null)
          childByTag1.GetComponent<Renderer>().enabled = false;
        if (!((UnityEngine.Object) childByTag2 != (UnityEngine.Object) null))
          return;
        childByTag2.GetComponent<Renderer>().enabled = true;
      }
      else
      {
        if ((UnityEngine.Object) childByTag1 != (UnityEngine.Object) null)
          childByTag1.GetComponent<Renderer>().enabled = true;
        if (!((UnityEngine.Object) childByTag2 != (UnityEngine.Object) null))
          return;
        childByTag2.GetComponent<Renderer>().enabled = false;
      }
    }
    else
    {
      if ((UnityEngine.Object) childByTag1 != (UnityEngine.Object) null)
        childByTag1.GetComponent<Renderer>().enabled = false;
      if (!((UnityEngine.Object) childByTag2 != (UnityEngine.Object) null))
        return;
      childByTag2.GetComponent<Renderer>().enabled = false;
    }
  }

  public void Show()
  {
    this.m_shown = true;
    this.SetEnabled(true);
    this.GetComponent<Collider>().enabled = true;
    if (this.m_actors == null)
      return;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    if ((UnityEngine.Object) preferredActor == (UnityEngine.Object) null || preferredActor.GetEntityDef() == null)
      return;
    bool show1 = this.ShouldShowNewItemGlow(preferredActor);
    this.ShowNewCardCallout(show1);
    preferredActor.Show();
    ActorStateType stateType = show1 ? ActorStateType.CARD_RECENTLY_ACQUIRED : ActorStateType.CARD_IDLE;
    preferredActor.SetActorState(stateType);
    if (this.m_cacheActorRenderers == null)
      this.m_cacheActorRenderers = new List<Renderer>();
    preferredActor.gameObject.GetComponentsInChildren<Renderer>(this.m_cacheActorRenderers);
    foreach (Renderer cacheActorRenderer in this.m_cacheActorRenderers)
      cacheActorRenderer.shadowCastingMode = ShadowCastingMode.Off;
    EntityDef entityDef = preferredActor.GetEntityDef();
    bool show2 = CollectionManager.Get().IsCardInCollection(entityDef.GetCardId(), preferredActor.GetPremium()) || this.IsInCollection(preferredActor.GetPremium());
    CollectionCardVisual.ShowActorShadow(preferredActor, show2);
  }

  protected virtual bool ShouldShowNewItemGlow(Actor actor)
  {
    if (this.m_visualType != CollectionUtils.ViewMode.CARDS)
      return false;
    string cardId = actor.GetEntityDef().GetCardId();
    TAG_PREMIUM premium = actor.GetPremium();
    return CollectionManager.Get().GetCollectibleDisplay().ShouldShowNewCardGlow(cardId, premium);
  }

  public void Hide()
  {
    this.m_shown = false;
    this.SetEnabled(false);
    this.GetComponent<Collider>().enabled = false;
    this.ShowLock(CollectionCardVisual.LockType.NONE);
    this.ShowNewCardCallout(false);
    if ((UnityEngine.Object) this.m_count != (UnityEngine.Object) null)
      this.m_count.Hide();
    if (this.m_actors == null)
      return;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    if ((UnityEngine.Object) preferredActor != (UnityEngine.Object) null)
      preferredActor.Hide();
    UberText componentInChildren = this.GetComponentInChildren<UberText>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.Hide();
    PegUI.Get().RemoveAsMouseDownElement((PegUIElement) this);
  }

  public void UpdateSpecialCaseTransform()
  {
    if (this.m_actors == null)
      return;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    if ((UnityEngine.Object) preferredActor == (UnityEngine.Object) null)
      return;
    TAG_PREMIUM premium = preferredActor.GetPremium();
    if (premium == this.m_cardVisualPremium)
      return;
    if (this.m_isScaled)
      this.SetOriginalCardTransform();
    if (this.m_visualType == CollectionUtils.ViewMode.CARDS)
    {
      switch (premium)
      {
        case TAG_PREMIUM.DIAMOND:
          this.SetDiamondCardTransform();
          break;
        case TAG_PREMIUM.SIGNATURE:
          this.SetSignatureCardTransform();
          break;
      }
    }
    this.m_cardVisualPremium = premium;
  }

  public void SetHeroSkinBoxCollider()
  {
    BoxCollider component = this.GetComponent<BoxCollider>();
    component.center = this.m_heroSkinBoxColliderCenter;
    component.size = this.m_heroSkinBoxColliderSize;
  }

  public void SetDefaultBoxCollider()
  {
    BoxCollider component = this.GetComponent<BoxCollider>();
    component.center = this.m_boxColliderCenter;
    component.size = this.m_boxColliderSize;
  }

  private void SetDiamondCardTransform()
  {
    this.m_originalScale = this.gameObject.transform.localScale;
    this.gameObject.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_diamondScale;
    this.gameObject.transform.localPosition -= (Vector3) (MobileOverrideValue<Vector3>) this.m_diamondPositionOffset;
    this.m_currentPositionOffset = (Vector3) (MobileOverrideValue<Vector3>) this.m_diamondPositionOffset;
    this.m_isScaled = true;
  }

  private void SetSignatureCardTransform()
  {
    this.m_originalScale = this.gameObject.transform.localScale;
    this.gameObject.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_signatureScale;
    this.gameObject.transform.localPosition -= (Vector3) (MobileOverrideValue<Vector3>) this.m_signaturePositionOffset;
    this.m_currentPositionOffset = (Vector3) (MobileOverrideValue<Vector3>) this.m_signaturePositionOffset;
    this.m_isScaled = true;
  }

  private void SetOriginalCardTransform()
  {
    this.gameObject.transform.localPosition += this.m_currentPositionOffset;
    this.gameObject.transform.localScale = this.m_originalScale;
    this.m_currentPositionOffset = Vector3.zero;
    this.m_isScaled = false;
  }

  private bool CheckCardSeen()
  {
    if (this.m_actors == null)
      return false;
    int num = this.m_actors.GetPreferredActor().GetActorStateMgr().GetActiveStateType() == ActorStateType.CARD_RECENTLY_ACQUIRED ? 1 : 0;
    if (num == 0)
      return num != 0;
    this.MarkAsSeen();
    return num != 0;
  }

  public virtual void MarkAsSeen()
  {
    string cardId = this.CardId;
    if (string.IsNullOrEmpty(cardId))
      return;
    CollectionManager.Get().MarkAllInstancesAsSeen(cardId, this.Premium);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if (this.ShouldIgnoreAllInput() || this.m_actors == null)
      return;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    EntityDef entityDef = preferredActor.GetEntityDef();
    if (entityDef != null)
    {
      TooltipPanelManager.Orientation orientation = this.m_cmRow > 0 ? TooltipPanelManager.Orientation.RightBottom : TooltipPanelManager.Orientation.RightTop;
      TooltipPanelManager.Get().UpdateKeywordHelpForCollectionManager(entityDef, preferredActor, orientation);
    }
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_mouse_over.prefab:0d4e20bc78956bc48b5e2963ec39211c", this.gameObject);
    if (!this.IsInCollection(preferredActor.GetPremium()))
      return;
    ActorStateType stateType = ActorStateType.CARD_MOUSE_OVER;
    if (this.CheckCardSeen())
      stateType = ActorStateType.CARD_RECENTLY_ACQUIRED_MOUSE_OVER;
    preferredActor.SetActorState(stateType);
    if (this.m_visualType != CollectionUtils.ViewMode.CARDS)
      return;
    Action<CollectionCardVisual> collectionCardOver = CollectionCardVisual.CollectionCardOver;
    if (collectionCardOver == null)
      return;
    collectionCardOver(this);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    TooltipPanelManager.Get().HideKeywordHelp();
    if (this.ShouldIgnoreAllInput() || this.m_actors == null)
      return;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    if (!this.IsInCollection(preferredActor.GetPremium()))
      return;
    this.CheckCardSeen();
    preferredActor.SetActorState(ActorStateType.CARD_IDLE);
    this.ShowNewCardCallout(false);
    if (this.m_visualType != CollectionUtils.ViewMode.CARDS)
      return;
    Action<CollectionCardVisual> collectionCardOut = CollectionCardVisual.CollectionCardOut;
    if (collectionCardOut == null)
      return;
    collectionCardOut(this);
  }

  protected override void OnDrag()
  {
    if (!this.CanPickUpCard())
      return;
    CollectionInputMgr.Get().GrabCardVisual(this);
  }

  protected override void OnRelease()
  {
    if (this.IsTransactionPendingOnThisCard() || CollectionInputMgr.Get().HasHeldCard())
      return;
    if (this.m_visualType == CollectionUtils.ViewMode.CARDS)
    {
      Action<CollectionCardVisual> collectionCardReleased = CollectionCardVisual.CollectionCardReleased;
      if (collectionCardReleased != null)
        collectionCardReleased(this);
    }
    Actor preferredActor = this.m_actors.GetPreferredActor();
    if (UniversalInputManager.Get().IsTouchMode() || (UnityEngine.Object) CraftingTray.Get() != (UnityEngine.Object) null && CraftingTray.Get().IsShown())
    {
      this.CheckCardSeen();
      this.ShowNewCardCallout(false);
      preferredActor.SetActorState(ActorStateType.CARD_IDLE);
      this.EnterCraftingMode();
    }
    else
    {
      Spell spell = preferredActor.GetSpell(SpellType.DEATHREVERSE);
      if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
        spell.gameObject.GetComponentInChildren<ParticleSystem>().main.simulationSpace = ParticleSystemSimulationSpace.Local;
      if (this.m_visualType == CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS || this.m_visualType == CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS)
        this.EnterCraftingMode();
      else if (!this.CanPickUpCard())
      {
        this.m_lastClickLeft = true;
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_move_invalid_or_click.prefab:777caa6f44f027747a03f3d85bcc897c");
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
          spell.ActivateState(SpellStateType.BIRTH);
        CollectionManager.Get().GetCollectibleDisplay().ShowInnkeeperLClickHelp(preferredActor.GetEntityDef());
        if (this.m_visualType != CollectionUtils.ViewMode.CARDS)
          return;
        CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
        if (editedDeck == null)
          return;
        EntityDef entityDef = preferredActor.GetEntityDef();
        if (entityDef == null)
          return;
        RunePattern runesToAdd = new RunePattern((EntityBase) entityDef);
        if (!runesToAdd.HasRunes || editedDeck.CanAddRunes(runesToAdd, DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
          return;
        GameplayErrorManager.Get().DisplayMessage(GameStrings.Get("GLUE_COLLECTION_INCOMPATIBLE_RUNES_HEADER"));
        TutorialDeathKnightDeckBuilding.ShowTutorial(UIVoiceLinesManager.TriggerType.CANNOT_ADD_RUNES);
      }
      else if (this.m_visualType == CollectionUtils.ViewMode.CARDS)
      {
        EntityDef entityDef = preferredActor.GetEntityDef();
        if (entityDef == null)
          return;
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
          spell.ActivateState(SpellStateType.BIRTH);
        if (CollectionDeckTray.Get().AddCard(entityDef, preferredActor.GetPremium(), false, preferredActor, DeckRule.RuleType.DEATHKNIGHT_RUNE_LIMIT))
          CollectionDeckTray.Get().OnCardManuallyAddedByUser_CheckSuggestions(entityDef);
        else
          SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_move_invalid_or_click.prefab:777caa6f44f027747a03f3d85bcc897c");
      }
      else if (this.m_visualType == CollectionUtils.ViewMode.CARD_BACKS)
      {
        CollectionDeckTray.Get().AnimateInCardBack(preferredActor);
      }
      else
      {
        if (this.m_visualType != CollectionUtils.ViewMode.HERO_SKINS)
          return;
        CollectionDeckTray.Get().SetHeroSkin(preferredActor);
      }
    }
  }

  protected override void OnRightClick()
  {
    if (this.IsTransactionPendingOnThisCard())
      return;
    if (!Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false))
      Options.Get().SetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, true);
    Actor preferredActor = this.m_actors.GetPreferredActor();
    if (this.m_lastClickLeft)
    {
      this.m_lastClickLeft = false;
      this.SendLeftRightClickTelemetry(preferredActor);
    }
    this.ShowNewCardCallout(false);
    preferredActor.SetActorState(ActorStateType.CARD_IDLE);
    this.m_clickedActorTransform = preferredActor.transform;
    this.EnterCraftingMode();
  }

  private void EnterCraftingMode()
  {
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    if (this.m_visualType != viewMode)
      return;
    switch (viewMode)
    {
      case CollectionUtils.ViewMode.CARDS:
        if ((UnityEngine.Object) CraftingManager.Get() != (UnityEngine.Object) null)
        {
          CraftingManager.Get().EnterCraftMode(this.GetActor());
          break;
        }
        break;
      case CollectionUtils.ViewMode.HERO_SKINS:
        HeroSkinInfoManager.EnterPreviewWhenReady(this);
        break;
      case CollectionUtils.ViewMode.CARD_BACKS:
        CardBackInfoManager.EnterPreviewWhenReady(this);
        break;
      case CollectionUtils.ViewMode.COINS:
        CoinManager coinManager = CoinManager.Get();
        if (coinManager != null)
        {
          coinManager.ShowCoinPreview(this.CardId, this.m_clickedActorTransform);
          break;
        }
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        BaconGuideSkinInfoManager.EnterPreviewWhenReady(this);
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        BaconHeroSkinInfoManager.EnterPreviewWhenReady(this);
        break;
    }
    CollectionDeckTray.Get()?.CancelRenamingDeck();
  }

  private bool IsTransactionPendingOnThisCard()
  {
    if (this.m_actors == null)
      return false;
    Actor preferredActor = this.m_actors.GetPreferredActor();
    CraftingManager craftingManager = CraftingManager.Get();
    if ((UnityEngine.Object) craftingManager == (UnityEngine.Object) null)
      return false;
    CraftingPendingTransaction serverTransaction = craftingManager.GetPendingServerTransaction();
    if (serverTransaction == null)
      return false;
    EntityDef entityDef = preferredActor.GetEntityDef();
    return entityDef != null && !(serverTransaction.CardID != entityDef.GetCardId()) && serverTransaction.Premium == preferredActor.GetPremium();
  }

  private bool ShouldIgnoreAllInput() => !this.m_shown || (UnityEngine.Object) CollectionInputMgr.Get() != (UnityEngine.Object) null && CollectionInputMgr.Get().IsDraggingScrollbar() || (UnityEngine.Object) CraftingManager.Get() != (UnityEngine.Object) null && CraftingManager.Get().IsCardShowing() || CollectionManager.Get().GetCollectibleDisplay().GetPageManager().ArePagesTurning();

  protected virtual bool IsInCollection(TAG_PREMIUM premium)
  {
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.COINS)
      return CoinManager.Get().IsCoinCardOwned(this.CardId);
    if (this.m_actors != null)
    {
      CollectionCardBack component = this.m_actors.GetPreferredActor().GetComponent<CollectionCardBack>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && CardBackManager.Get().IsCardBackOwned(component.GetCardBackId()))
        return true;
    }
    int num = 0;
    if ((UnityEngine.Object) this.m_count != (UnityEngine.Object) null)
      num = this.m_count.GetCount(premium);
    return num > 0;
  }

  private bool IsUnlocked()
  {
    Actor preferredActor = this.m_actors.GetPreferredActor();
    return !RankMgr.Get().IsCardLockedInCurrentLeague(preferredActor.GetEntityDef()) && this.m_lockType == CollectionCardVisual.LockType.NONE;
  }

  private bool CanPickUpCard()
  {
    if (this.ShouldIgnoreAllInput() || CollectionManager.Get().GetCollectibleDisplay().GetViewMode() != this.m_visualType || (UnityEngine.Object) CollectionDeckTray.Get() == (UnityEngine.Object) null || !CollectionDeckTray.Get().CanPickupCard())
      return false;
    switch (this.m_visualType)
    {
      case CollectionUtils.ViewMode.CARDS:
        if (this.m_actors == null)
          return false;
        Actor preferredActor = this.m_actors.GetPreferredActor();
        if ((bool) (UnityEngine.Object) preferredActor && !this.IsInCollection(preferredActor.GetPremium()) || !this.IsUnlocked())
          return false;
        break;
      case CollectionUtils.ViewMode.HERO_SKINS:
        if (HeroSkinInfoManager.IsLoadedAndShowingPreview())
          return false;
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        return false;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        return false;
    }
    return true;
  }

  public void ShowNewCardCallout(bool show)
  {
    if ((UnityEngine.Object) this.m_newCardCallout == (UnityEngine.Object) null)
      return;
    this.m_newCardCallout.SetActive(show);
  }

  private void UpdateCardCount()
  {
    int normalCount = 0;
    int goldenCount = 0;
    int signatureCount = 0;
    int diamondCount = 0;
    TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
    if (this.m_actors != null)
    {
      Actor preferredActor = this.m_actors.GetPreferredActor();
      EntityDef entityDef = preferredActor.GetEntityDef();
      if (entityDef != null)
      {
        premium = preferredActor.GetPremium();
        CollectibleCard card1 = CollectionManager.Get().GetCard(entityDef.GetCardId(), TAG_PREMIUM.NORMAL);
        if (card1 != null)
          normalCount = card1.OwnedCount;
        CollectibleCard card2 = CollectionManager.Get().GetCard(entityDef.GetCardId(), TAG_PREMIUM.GOLDEN);
        if (card2 != null)
          goldenCount = card2.OwnedCount;
        CollectibleCard card3 = CollectionManager.Get().GetCard(entityDef.GetCardId(), TAG_PREMIUM.SIGNATURE);
        if (card3 != null)
          signatureCount = card3.OwnedCount;
        CollectibleCard card4 = CollectionManager.Get().GetCard(entityDef.GetCardId(), TAG_PREMIUM.DIAMOND);
        if (card4 != null)
          diamondCount = card4.OwnedCount;
      }
    }
    if ((UnityEngine.Object) this.m_count != (UnityEngine.Object) null)
      this.m_count.SetCount(normalCount, goldenCount, signatureCount, diamondCount, premium);
    this.UpdateCardCountVisibility();
  }

  private void UpdateCardCountVisibility()
  {
    if (!((UnityEngine.Object) this.m_count != (UnityEngine.Object) null))
      return;
    if ((this.m_lockType == CollectionCardVisual.LockType.NONE || this.m_lockType == CollectionCardVisual.LockType.BANNED) && (this.m_visualType == CollectionUtils.ViewMode.CARDS || this.m_visualType == CollectionUtils.ViewMode.COINS) && this.Premium != TAG_PREMIUM.DIAMOND)
      this.m_count.Show();
    else
      this.m_count.Hide();
  }

  private void SendLeftRightClickTelemetry(Actor actor)
  {
    CollectionLeftRightClick.Target target_ = CollectionLeftRightClick.Target.CARD;
    EntityDef entityDef = actor.GetEntityDef();
    if (entityDef == null)
      target_ = CollectionLeftRightClick.Target.CARD_BACK;
    else if (entityDef.IsHeroSkin())
      target_ = CollectionLeftRightClick.Target.HERO_SKIN;
    TelemetryManager.Client().SendCollectionLeftRightClick(target_);
  }

  protected override void OnDestroy() => this.m_cacheActorRenderers = (List<Renderer>) null;

  public void SetRuneBannerHighlighted(bool highlight)
  {
    CardRuneBanner runeBanner = this.GetActor().GetRuneBanner();
    if (!(bool) (UnityEngine.Object) runeBanner)
      return;
    runeBanner.SetHighlighted(highlight);
  }

  public enum LockType
  {
    NONE,
    MAX_COPIES_IN_DECK,
    NO_MORE_INSTANCES,
    NOT_PLAYABLE,
    BANNED,
  }
}
