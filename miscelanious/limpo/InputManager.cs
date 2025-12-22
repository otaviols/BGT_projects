using Blizzard.T5.Core.Utils;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
  public float m_MouseOverDelay = 0.4f;
  public float m_TouchHoldDuration = 0.1f;
  public DragRotatorInfo m_DragRotatorInfo = new DragRotatorInfo()
  {
    m_PitchInfo = new DragRotatorAxisInfo()
    {
      m_ForceMultiplier = 25f,
      m_MinDegrees = -40f,
      m_MaxDegrees = 40f,
      m_RestSeconds = 2f
    },
    m_RollInfo = new DragRotatorAxisInfo()
    {
      m_ForceMultiplier = 25f,
      m_MinDegrees = -45f,
      m_MaxDegrees = 45f,
      m_RestSeconds = 2f
    }
  };
  private readonly PlatformDependentValue<float> MIN_GRAB_Y = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    Tablet = 80f,
    Phone = 80f
  };
  private const float MOBILE_TARGETTING_Y_OFFSET = 0.8f;
  private const float MOBILE_TARGETTING_XY_SCALE = 1.08f;
  private static InputManager s_instance;
  private UniversalInputManager m_universalInputManager;
  private GameState m_gameState;
  private TargetReticleManager m_targetReticleManager;
  private ZoneHand m_myHandZone;
  private ZonePlay m_myPlayZone;
  private ZoneWeapon m_myWeaponZone;
  private ZoneHand m_enemyHandZone;
  private ZonePlay m_enemyPlayZone;
  private Card m_heldCard;
  private bool m_checkForInput;
  private GameObject m_lastObjectMousedDown;
  private GameObject m_lastObjectRightMousedDown;
  private Vector3 m_lastMouseDownPosition;
  private bool m_leftMouseButtonIsDown;
  private bool m_dragging;
  private bool m_lastInputDrag;
  private Card m_mousedOverCard;
  private GameObject m_mousedOverObject;
  private float m_mousedOverTimer;
  private bool m_heldCardWasInTradeAreaLastFrame;
  private bool m_hadPendingChoiceTargetLastFrame;
  private ZoneChangeList m_lastZoneChangeList;
  private Card m_battlecrySourceCard;
  private List<Card> m_cancelingBattlecryCards = new List<Card>();
  private bool m_cardWasInsideHandLastFrame;
  private bool m_isInBattleCryEffect;
  private List<Entity> m_entitiesThatPredictedMana = new List<Entity>();
  private IGraphicsManager m_graphicsManager;
  private List<Actor> m_mobileTargettingEffectActors = new List<Actor>();
  private Card m_lastPreviewedCard;
  private bool m_touchDraggingCard;
  private bool m_useHandEnlarge;
  private bool m_hideHandAfterPlayingCard;
  private bool m_targettingHeroPower;
  private bool m_touchedDownOnSmallHand;
  private float m_touchedDownOnSmallHandStartTime;
  private bool m_enlargeHandAfterDropCard;
  private bool m_handIsEnlarging;
  private int m_telemetryNumDragAttacks;
  private int m_telemetryNumClickAttacks;
  private const int RAYCAST_MAXTOUCHNUMBER = 30;
  private RaycastHit[] m_cachedDustBlockers = new RaycastHit[30];
  private InputManager.RaycastHitComparer m_hitPointComparer = new InputManager.RaycastHitComparer();
  private ScreenEffectsHandle m_screenEffectHandle;
  private List<InputManager.PhoneHandShownListener> m_phoneHandShownListener = new List<InputManager.PhoneHandShownListener>();
  private List<InputManager.PhoneHandHiddenListener> m_phoneHandHiddenListener = new List<InputManager.PhoneHandHiddenListener>();

  private void Awake()
  {
    InputManager.s_instance = this;
    this.m_useHandEnlarge = (bool) UniversalInputManager.UsePhoneUI;
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.SetDragging(this.m_dragging);
    this.UpdateManagers();
    if (this.m_gameState != null)
    {
      this.m_gameState.RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
      this.m_gameState.RegisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected));
      this.m_gameState.RegisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
      this.m_gameState.RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    }
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    this.m_screenEffectHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy()
  {
    this.UpdateManagers();
    if (this.m_gameState != null)
    {
      this.m_gameState.UnregisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
      this.m_gameState.UnregisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected));
      this.m_gameState.UnregisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
      this.m_gameState.UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    }
    FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    InputManager.s_instance = (InputManager) null;
    this.m_cachedDustBlockers = (RaycastHit[]) null;
    this.m_hitPointComparer = (InputManager.RaycastHitComparer) null;
  }

  private void OnFatalError(FatalErrorMessage message, object userData) => this.DisableInput();

  private bool IsInputOverCard(Card wantedCard)
  {
    if ((UnityEngine.Object) wantedCard == (UnityEngine.Object) null)
      return false;
    Actor actor = wantedCard.GetActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return false;
    RaycastHit hitInfo;
    if (!actor.IsColliderEnabled())
    {
      actor.ToggleCollider(true);
      this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo);
      actor.ToggleCollider(false);
    }
    else
      this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo);
    if (!((UnityEngine.Object) hitInfo.collider != (UnityEngine.Object) null))
      return false;
    Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) hitInfo.transform);
    return !((UnityEngine.Object) componentInParents == (UnityEngine.Object) null) && (UnityEngine.Object) componentInParents.GetCard() == (UnityEngine.Object) wantedCard;
  }

  private bool ShouldCancelTargeting(bool hitBattlefieldHitbox)
  {
    bool flag1 = false;
    if (!hitBattlefieldHitbox && (UnityEngine.Object) this.GetBattlecrySourceCard() == (UnityEngine.Object) null && (UnityEngine.Object) ChoiceCardMgr.Get().GetSubOptionParentCard() == (UnityEngine.Object) null && !this.HasPendingChoiceTarget())
    {
      flag1 = true;
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        bool flag2 = this.m_universalInputManager.InputHitAnyObject(Camera.main, GameLayer.InvisibleHitBox3);
        if (((this.m_targettingHeroPower ? 1 : (this.m_gameState.IsSelectedOptionFriendlyHero() ? 1 : 0)) | (flag2 ? 1 : 0)) != 0)
          flag1 = false;
      }
      else if (this.m_gameState.IsSelectedOptionFriendlyHero())
      {
        Player friendlySidePlayer = this.m_gameState.GetFriendlySidePlayer();
        Card heroCard = friendlySidePlayer.GetHeroCard();
        Card weaponCard = friendlySidePlayer.GetWeaponCard();
        if (this.IsInputOverCard(heroCard) || this.IsInputOverCard(weaponCard))
          flag1 = false;
      }
      else if (this.m_gameState.IsSelectedOptionFriendlyHeroPower())
      {
        if (this.IsInputOverCard(this.m_gameState.GetFriendlySidePlayer().GetHeroPowerCard()))
          flag1 = false;
      }
      else if (this.m_gameState.IsSelectedOptionMercenariesAbility())
        flag1 = false;
    }
    if ((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null)
    {
      Entity entity = this.m_heldCard.GetEntity();
      RaycastHit hitInfo;
      if (entity != null && entity.HasTag(GAME_TAG.TRADEABLE) && this.m_universalInputManager.GetInputHitInfo(Camera.main, GameLayer.DragPlane, out hitInfo) && this.m_heldCard.IsInTradeArea(hitInfo.point))
      {
        flag1 = true;
        this.m_heldCard.transform.position = hitInfo.point;
      }
    }
    return flag1;
  }

  private void UpdateTargetingArrow()
  {
    if ((bool) (UnityEngine.Object) this.m_targetReticleManager && this.m_targetReticleManager.IsActive())
    {
      if (this.ShouldCancelTargeting(this.m_universalInputManager.InputHitAnyObject(Camera.main, GameLayer.InvisibleHitBox2)))
      {
        bool flag = true;
        if ((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null && this.m_heldCard.GetEntity().HasTag(GAME_TAG.TRADEABLE) && this.m_heldCard.IsInTradeArea())
        {
          this.CancelTargetMode();
          flag = false;
        }
        if (flag && this.m_targetReticleManager.IsLocalArrow())
          this.CancelOption();
        if (this.m_useHandEnlarge)
          this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
        if (!((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null))
          return;
        this.PositionHeldCard();
      }
      else
      {
        this.m_targetReticleManager.UpdateArrowPosition();
        if (!((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null))
          return;
        this.m_myHandZone.OnCardHeld(this.m_heldCard);
      }
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this.m_heldCard)
        return;
      this.HandleUpdateWhileHoldingCard(this.m_universalInputManager.InputHitAnyObject(Camera.main, GameLayer.InvisibleHitBox2));
      if (!((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null))
        return;
      bool flag = this.m_heldCard.IsInTradeArea();
      if (flag && !this.m_heldCardWasInTradeAreaLastFrame)
      {
        this.m_heldCard.UpdateActorState(true);
        this.m_heldCardWasInTradeAreaLastFrame = true;
      }
      else
      {
        if (flag || !this.m_heldCardWasInTradeAreaLastFrame)
          return;
        this.m_heldCard.UpdateActorState(true);
        this.m_heldCardWasInTradeAreaLastFrame = false;
      }
    }
  }

  private void UpdateChoiceTargeting()
  {
    int count = GameState.Get().GetPowerProcessor().GetPowerQueue().Count;
    bool flag = this.HasPendingChoiceTarget() && count <= 0;
    if (flag && !this.m_hadPendingChoiceTargetLastFrame)
    {
      this.m_hadPendingChoiceTargetLastFrame = true;
      this.StartPendingChoiceTarget();
    }
    else
    {
      if (flag || !this.m_hadPendingChoiceTargetLastFrame)
        return;
      this.FinishPendingChoiceTarget();
      this.m_hadPendingChoiceTargetLastFrame = false;
    }
  }

  public void ForceRefreshTargetingArrowText()
  {
    if (!this.HasPendingChoiceTarget())
      return;
    Network.EntityChoices friendlyEntityChoices = this.m_gameState.GetFriendlyEntityChoices();
    if (friendlyEntityChoices == null)
      return;
    Entity entity = this.m_gameState.GetEntity(friendlyEntityChoices.Source);
    if (!(bool) (UnityEngine.Object) this.m_targetReticleManager)
      return;
    this.m_targetReticleManager.RefreshTargetingArrowText(entity);
  }

  private bool HasPendingChoiceTarget()
  {
    Network.EntityChoices friendlyEntityChoices = this.m_gameState.GetFriendlyEntityChoices();
    return friendlyEntityChoices != null && friendlyEntityChoices.ChoiceType == CHOICE_TYPE.TARGET;
  }

  private void UpdateTradeableDeckGlow()
  {
    ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(Player.Side.FRIENDLY);
    if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
      return;
    bool flag = false;
    if ((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null)
    {
      Entity entity = this.m_heldCard.GetEntity();
      if (entity != null && entity.IsTradeable() && this.m_gameState.IsValidOption(entity, new bool?(true)))
        flag = true;
    }
    if (flag)
      zoneOfType.ShowTradeableGlow();
    else
      zoneOfType.HideTradeableGlow();
  }

  private void UpdateManagers()
  {
    this.m_universalInputManager = UniversalInputManager.Get();
    this.m_gameState = GameState.Get();
    this.m_targetReticleManager = TargetReticleManager.Get();
  }

  private void Update()
  {
    if (!this.m_checkForInput)
      return;
    this.UpdateManagers();
    if (InputCollection.GetMouseButtonDown(0))
      this.HandleLeftMouseDown();
    if (InputCollection.GetMouseButtonUp(0))
    {
      this.m_touchDraggingCard = false;
      this.HandleLeftMouseUp();
    }
    if (InputCollection.GetMouseButtonDown(1))
      this.HandleRightMouseDown();
    if (InputCollection.GetMouseButtonUp(1))
      this.HandleRightMouseUp();
    this.HandleMouseMove();
    if ((UnityEngine.Object) this.m_heldCard == (UnityEngine.Object) null)
    {
      if (this.m_leftMouseButtonIsDown)
      {
        this.HandleUpdateWhileLeftMouseButtonIsDown();
        if (this.m_universalInputManager.IsTouchMode() && !this.m_touchDraggingCard)
          this.HandleUpdateWhileNotHoldingCard();
      }
      else
        this.HandleUpdateWhileNotHoldingCard();
    }
    if (this.PermitDecisionMakingInput())
    {
      this.UpdateTargetingArrow();
      this.UpdateChoiceTargeting();
      this.UpdateTradeableDeckGlow();
    }
    EmoteHandler emoteHandler = EmoteHandler.Get();
    if ((UnityEngine.Object) emoteHandler != (UnityEngine.Object) null && emoteHandler.AreEmotesActive())
      emoteHandler.HandleInput();
    EnemyEmoteHandler enemyEmoteHandler = EnemyEmoteHandler.Get();
    if ((UnityEngine.Object) enemyEmoteHandler != (UnityEngine.Object) null && enemyEmoteHandler.AreEmotesActive())
      enemyEmoteHandler.HandleInput();
    this.ShowTooltipIfNecessary();
  }

  public static InputManager Get() => InputManager.s_instance;

  public bool HandleKeyboardInput()
  {
    if (this.HandleUniversalHotkeys())
      return true;
    return this.m_gameState != null && this.m_gameState.IsMulliganManagerActive() ? this.HandleMulliganHotkeys() : this.HandleGameHotkeys();
  }

  public Card GetMousedOverCard() => this.m_mousedOverCard;

  public void SetMousedOverCard(Card card)
  {
    if ((UnityEngine.Object) this.m_mousedOverCard == (UnityEngine.Object) card)
      return;
    if ((UnityEngine.Object) this.m_mousedOverCard != (UnityEngine.Object) null && !(this.m_mousedOverCard.GetZone() is ZoneHand))
      this.HandleMouseOffCard();
    if (!card.IsInputEnabled())
      return;
    this.m_mousedOverCard = card;
    card.NotifyMousedOver();
  }

  public Card GetBattlecrySourceCard() => this.m_battlecrySourceCard;

  public void StartWatchingForInput()
  {
    if (this.m_checkForInput)
      return;
    this.m_checkForInput = true;
    foreach (Zone zone in ZoneMgr.Get().GetZones())
    {
      if (zone.m_Side == Player.Side.FRIENDLY)
      {
        switch (zone)
        {
          case ZoneHand _:
            this.m_myHandZone = (ZoneHand) zone;
            continue;
          case ZonePlay _:
            this.m_myPlayZone = (ZonePlay) zone;
            continue;
          case ZoneWeapon _:
            this.m_myWeaponZone = (ZoneWeapon) zone;
            continue;
          default:
            continue;
        }
      }
      else if (zone is ZonePlay)
        this.m_enemyPlayZone = (ZonePlay) zone;
      else if (zone is ZoneHand)
        this.m_enemyHandZone = (ZoneHand) zone;
    }
  }

  public void DisableInput()
  {
    this.m_checkForInput = false;
    this.HandleMouseOff();
    this.m_targetReticleManager?.DestroyFriendlyTargetArrow(false);
  }

  public bool PermitDecisionMakingInput()
  {
    GameMgr gameMgr = GameMgr.Get();
    if (gameMgr != null && gameMgr.IsSpectator())
      return false;
    if (this.m_gameState != null)
    {
      Player friendlySidePlayer = this.m_gameState.GetFriendlySidePlayer();
      if (friendlySidePlayer != null && friendlySidePlayer.HasTag(GAME_TAG.AI_MAKES_DECISIONS_FOR_PLAYER))
        return false;
    }
    return true;
  }

  public Card GetHeldCard() => this.m_heldCard;

  public void EnableInput() => this.m_checkForInput = true;

  public void OnMulliganEnded()
  {
    if (!(bool) (UnityEngine.Object) this.m_mousedOverCard)
      return;
    this.SetShouldShowTooltip();
  }

  private void SetShouldShowTooltip()
  {
    this.m_mousedOverTimer = 0.0f;
    this.m_mousedOverCard.SetShouldShowTooltip();
  }

  public bool LeftMouseButtonDown => this.m_leftMouseButtonIsDown;

  public Vector3 LastMouseDownPosition => this.m_lastMouseDownPosition;

  public ZoneHand GetFriendlyHand() => this.m_myHandZone;

  public ZoneHand GetEnemyHand() => this.m_enemyHandZone;

  public bool UseHandEnlarge() => this.m_useHandEnlarge;

  public void SetHandEnlarge(bool set) => this.m_useHandEnlarge = set;

  public bool DoesHideHandAfterPlayingCard() => this.m_hideHandAfterPlayingCard;

  public void SetHideHandAfterPlayingCard(bool set) => this.m_hideHandAfterPlayingCard = set;

  public bool DropHeldCard() => this.DropHeldCard(false);

  private void HandleLeftMouseDown()
  {
    this.m_touchedDownOnSmallHand = false;
    bool flag = true;
    GameObject hitObject = (GameObject) null;
    RaycastHit hitInfo;
    if (this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo))
    {
      hitObject = hitInfo.collider.gameObject;
      if ((UnityEngine.Object) hitObject.GetComponent<EndTurnButtonReminder>() != (UnityEngine.Object) null)
        return;
      CardStandIn componentInParents1 = GameObjectUtils.FindComponentInParents<CardStandIn>((Component) hitInfo.transform);
      if ((UnityEngine.Object) componentInParents1 != (UnityEngine.Object) null && this.m_gameState != null && !this.m_gameState.IsMulliganManagerActive())
      {
        if (this.IsCancelingBattlecryCard(componentInParents1.linkedCard))
          return;
        if ((UnityEngine.Object) this.m_myHandZone == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) ("HandZone not set for CardStandIn " + (componentInParents1.name ?? "Unknown")));
          return;
        }
        if (this.m_useHandEnlarge && !this.m_myHandZone.HandEnlarged())
        {
          this.m_leftMouseButtonIsDown = true;
          this.m_touchedDownOnSmallHand = true;
          if (!this.HasPlayFromMiniHandEnabled())
            return;
          this.m_touchedDownOnSmallHandStartTime = Time.realtimeSinceStartup;
        }
        this.m_lastObjectMousedDown = componentInParents1.gameObject;
        this.m_lastMouseDownPosition = InputCollection.GetMousePosition();
        this.m_leftMouseButtonIsDown = true;
        if (this.m_universalInputManager.IsTouchMode())
        {
          this.m_touchDraggingCard = this.m_myHandZone.TouchReceived();
          this.m_lastPreviewedCard = componentInParents1.linkedCard;
        }
        if (!((UnityEngine.Object) this.m_heldCard == (UnityEngine.Object) null))
          return;
        this.m_myHandZone.HandleInput();
        return;
      }
      if ((UnityEngine.Object) hitObject.GetComponent<EndTurnButton>() != (UnityEngine.Object) null && this.PermitDecisionMakingInput() && !EndTurnButton.Get().IsInputBlocked())
      {
        EndTurnButton.Get().PlayPushDownAnimation();
        this.m_lastObjectMousedDown = hitObject;
        return;
      }
      if ((UnityEngine.Object) hitObject.GetComponent<GameOpenPack>() != (UnityEngine.Object) null)
      {
        this.m_lastObjectMousedDown = hitObject;
        return;
      }
      Actor componentInParents2 = GameObjectUtils.FindComponentInParents<Actor>((Component) hitInfo.transform);
      if (((UnityEngine.Object) componentInParents2 == (UnityEngine.Object) null || componentInParents2.GetEntity() == null || !componentInParents2.GetEntity().IsControlledByFriendlySidePlayer()) && this.PermitDecisionMakingInput())
        this.ManuallyDismissMercenariesAbilityTray();
      BattlegroundsEmoteHandler handler;
      if (BattlegroundsEmoteHandler.TryGetActiveInstance(out handler) && (UnityEngine.Object) hitObject != (UnityEngine.Object) handler.gameObject && !hitObject.TryGetComponent<BattlegroundsEmoteOption>(out BattlegroundsEmoteOption _))
        handler.HideEmotes();
      if ((UnityEngine.Object) componentInParents2 == (UnityEngine.Object) null)
        return;
      Card card = componentInParents2.GetCard();
      if (this.m_universalInputManager.IsTouchMode() && (UnityEngine.Object) this.m_battlecrySourceCard != (UnityEngine.Object) null && (UnityEngine.Object) card == (UnityEngine.Object) this.m_battlecrySourceCard)
      {
        this.SetDragging(true);
        this.m_targetReticleManager.ShowArrow(true);
        return;
      }
      if ((UnityEngine.Object) card != (UnityEngine.Object) null && (this.IsCancelingBattlecryCard(card) || (UnityEngine.Object) this.m_myHandZone == (UnityEngine.Object) null || card.GetEntity() == null || this.m_useHandEnlarge && this.m_myHandZone.HandEnlarged() && card.GetEntity().IsHeroPower() && card.GetEntity().IsControlledByLocalUser() && this.m_myHandZone.GetCardCount() > 1))
        return;
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
        this.m_lastObjectMousedDown = card.gameObject;
      else if ((UnityEngine.Object) componentInParents2.GetHistoryCard() != (UnityEngine.Object) null)
        this.m_lastObjectMousedDown = componentInParents2.transform.parent.gameObject;
      else
        Debug.LogWarning((object) "You clicked on something that is not being handled by InputManager.  Alert The Brode!");
      this.m_lastMouseDownPosition = InputCollection.GetMousePosition();
      this.m_leftMouseButtonIsDown = true;
      flag = componentInParents2.GetEntity() != null && componentInParents2.GetEntity().IsGameModeButton();
    }
    if (this.m_useHandEnlarge && (UnityEngine.Object) this.m_myHandZone != (UnityEngine.Object) null && this.m_myHandZone.HandEnlarged() && (UnityEngine.Object) ChoiceCardMgr.Get().GetSubOptionParentCard() == (UnityEngine.Object) null && (UnityEngine.Object) hitObject == (UnityEngine.Object) null | flag)
      this.HidePhoneHand();
    if ((UnityEngine.Object) hitObject == (UnityEngine.Object) null && this.PermitDecisionMakingInput())
      this.ManuallyDismissMercenariesAbilityTray();
    BattlegroundsEmoteHandler handler1;
    if ((UnityEngine.Object) hitObject == (UnityEngine.Object) null && BattlegroundsEmoteHandler.TryGetActiveInstance(out handler1))
      handler1.HideEmotes();
    this.HandleMemberClick(hitObject);
  }

  private void ShowPhoneHand()
  {
    if (this.m_gameState.IsMulliganPhaseNowOrPending() || this.m_gameState.IsGameOver() || !this.m_useHandEnlarge || this.m_myHandZone.HandEnlarged())
      return;
    this.m_handIsEnlarging = true;
    this.m_myHandZone.AddUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(this.OnHandEnlargeComplete));
    this.m_myHandZone.SetHandEnlarged(true);
    foreach (InputManager.PhoneHandShownListener handShownListener in this.m_phoneHandShownListener.ToArray())
      handShownListener.Fire();
  }

  public void HidePhoneHand()
  {
    if (!this.m_useHandEnlarge || !((UnityEngine.Object) this.m_myHandZone != (UnityEngine.Object) null) || !this.m_myHandZone.HandEnlarged() || this.m_handIsEnlarging)
      return;
    this.m_myHandZone.SetHandEnlarged(false);
    foreach (InputManager.PhoneHandHiddenListener handHiddenListener in this.m_phoneHandHiddenListener.ToArray())
      handHiddenListener.Fire();
  }

  private void OnHandEnlargeComplete(Zone zone, object userData)
  {
    zone.RemoveUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(this.OnHandEnlargeComplete));
    if (this.m_leftMouseButtonIsDown && this.m_universalInputManager.InputHitAnyObject(GameLayer.CardRaycast))
      this.HandleLeftMouseDown();
    this.m_handIsEnlarging = false;
  }

  private void HidePhoneHandIfOutOfServerPlays()
  {
    if (this.m_gameState.HasHandPlays())
      return;
    this.HidePhoneHand();
  }

  private bool HasLocalHandPlays()
  {
    List<Card> cards = this.m_myHandZone.GetCards();
    if (cards.Count == 0)
      return false;
    int spendableManaCrystals = ManaCrystalMgr.Get().GetSpendableManaCrystals();
    foreach (Card card in cards)
    {
      if (card.GetEntity().GetRealTimeCost() <= spendableManaCrystals)
        return true;
    }
    return false;
  }

  private void HandleLeftMouseUp()
  {
    PegCursor.Get().SetMode(PegCursor.Mode.UP);
    this.m_lastInputDrag = this.m_dragging;
    this.SetDragging(false);
    this.m_leftMouseButtonIsDown = false;
    this.m_targettingHeroPower = false;
    GameObject objectMousedDown = this.m_lastObjectMousedDown;
    this.m_lastObjectMousedDown = (GameObject) null;
    if (this.m_universalInputManager.WasTouchCanceled())
      this.CancelOption();
    else if ((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null && (this.m_gameState.GetResponseMode() == GameState.ResponseMode.OPTION || this.m_gameState.GetResponseMode() == GameState.ResponseMode.NONE))
    {
      this.DropHeldCard();
    }
    else
    {
      BattlegroundsEmoteHandler handler;
      if (BattlegroundsEmoteHandler.TryGetActiveInstance(out handler))
      {
        if (handler.IsMouseOverEmoteOption)
        {
          handler.HandleEmoteClicked();
          return;
        }
        if (this.m_universalInputManager.IsTouchMode())
        {
          handler.HideEmotes();
          return;
        }
      }
      bool flag1 = this.m_universalInputManager.IsTouchMode() && this.m_gameState.IsInTargetMode();
      ChoiceCardMgr choiceCardMgr = ChoiceCardMgr.Get();
      bool flag2 = (UnityEngine.Object) choiceCardMgr.GetSubOptionParentCard() != (UnityEngine.Object) null;
      RaycastHit hitInfo;
      if (this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo))
      {
        GameObject gameObject = hitInfo.collider.gameObject;
        if ((UnityEngine.Object) gameObject.GetComponent<EndTurnButtonReminder>() != (UnityEngine.Object) null)
          return;
        if ((UnityEngine.Object) gameObject.GetComponent<EndTurnButton>() != (UnityEngine.Object) null && (UnityEngine.Object) gameObject == (UnityEngine.Object) objectMousedDown && this.PermitDecisionMakingInput() && !EndTurnButton.Get().IsInputBlocked())
        {
          EndTurnButton.Get().PlayButtonUpAnimation();
          this.DoEndTurnButton();
          this.ManuallyDismissMercenariesAbilityTray();
        }
        else
        {
          GameOpenPack component = gameObject.GetComponent<GameOpenPack>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) gameObject == (UnityEngine.Object) objectMousedDown)
          {
            component.HandleClick();
          }
          else
          {
            Actor componentInParents1 = GameObjectUtils.FindComponentInParents<Actor>((Component) hitInfo.transform);
            if ((UnityEngine.Object) componentInParents1 != (UnityEngine.Object) null)
            {
              Card card = componentInParents1.GetCard();
              if ((UnityEngine.Object) card != (UnityEngine.Object) null)
              {
                if (((UnityEngine.Object) card.gameObject == (UnityEngine.Object) objectMousedDown || this.m_lastInputDrag) && !this.IsCancelingBattlecryCard(card))
                  this.HandleClickOnCard(card.gameObject, (UnityEngine.Object) card.gameObject == (UnityEngine.Object) objectMousedDown);
              }
              else if ((UnityEngine.Object) componentInParents1.GetHistoryCard() != (UnityEngine.Object) null)
                HistoryManager.Get().HandleClickOnBigCard(componentInParents1.GetHistoryCard());
              else if (this.m_gameState.IsMulliganManagerActive())
                MulliganManager.Get().ToggleHoldState(componentInParents1);
            }
            CardStandIn componentInParents2 = GameObjectUtils.FindComponentInParents<CardStandIn>((Component) hitInfo.transform);
            if ((UnityEngine.Object) componentInParents2 != (UnityEngine.Object) null)
            {
              if (this.m_useHandEnlarge && this.m_touchedDownOnSmallHand)
              {
                if (this.HasPlayFromMiniHandEnabled())
                {
                  if (this.WaitingForTouchDelay())
                    this.ShowPhoneHand();
                  else
                    this.TryHandleClickOnCard(objectMousedDown, componentInParents2);
                }
                else
                {
                  this.ShowPhoneHand();
                  this.TryHandleClickOnCard(objectMousedDown, componentInParents2);
                }
              }
              else
                this.TryHandleClickOnCard(objectMousedDown, componentInParents2);
            }
            if (this.m_universalInputManager.IsTouchMode() && (UnityEngine.Object) componentInParents1 != (UnityEngine.Object) null && (UnityEngine.Object) choiceCardMgr.GetSubOptionParentCard() != (UnityEngine.Object) null)
            {
              Card card = componentInParents1.GetCard();
              foreach (UnityEngine.Object friendlyCard in choiceCardMgr.GetFriendlyCards())
              {
                if (friendlyCard == (UnityEngine.Object) card)
                {
                  flag2 = false;
                  break;
                }
              }
            }
          }
        }
      }
      if (flag1)
        this.CancelOption();
      if (!(this.m_universalInputManager.IsTouchMode() & flag2) || !((UnityEngine.Object) choiceCardMgr.GetSubOptionParentCard() != (UnityEngine.Object) null))
        return;
      this.CancelSubOptionMode();
    }
  }

  public bool WaitingForTouchDelay() => (double) Time.realtimeSinceStartup - (double) this.m_touchedDownOnSmallHandStartTime <= (double) this.m_TouchHoldDuration;

  private void TryHandleClickOnCard(GameObject lastCardDown, CardStandIn standIn)
  {
    if (!((UnityEngine.Object) lastCardDown == (UnityEngine.Object) standIn.gameObject) || !((UnityEngine.Object) standIn.linkedCard != (UnityEngine.Object) null) || this.m_gameState == null || this.m_gameState.IsMulliganManagerActive() || this.IsCancelingBattlecryCard(standIn.linkedCard))
      return;
    this.HandleClickOnCard(standIn.linkedCard.gameObject, true);
  }

  private void HandleRightMouseDown()
  {
    RaycastHit hitInfo;
    if (!this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo))
      return;
    GameObject gameObject = hitInfo.collider.gameObject;
    if ((UnityEngine.Object) gameObject.GetComponent<EndTurnButtonReminder>() != (UnityEngine.Object) null || (UnityEngine.Object) gameObject.GetComponent<EndTurnButton>() != (UnityEngine.Object) null)
      return;
    Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) hitInfo.transform);
    if ((UnityEngine.Object) componentInParents == (UnityEngine.Object) null)
      return;
    Card card = componentInParents.GetCard();
    if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      this.m_lastObjectRightMousedDown = card.gameObject;
    else if ((UnityEngine.Object) componentInParents.GetHistoryCard() != (UnityEngine.Object) null)
      this.m_lastObjectRightMousedDown = componentInParents.transform.parent.gameObject;
    else
      Debug.LogWarning((object) "You clicked on something that is not being handled by InputManager.  Alert The Brode!");
  }

  private void HandleRightMouseUp()
  {
    PegCursor.Get().SetMode(PegCursor.Mode.UP);
    GameObject objectRightMousedDown = this.m_lastObjectRightMousedDown;
    this.m_lastObjectRightMousedDown = (GameObject) null;
    this.m_lastObjectMousedDown = (GameObject) null;
    this.m_leftMouseButtonIsDown = false;
    this.SetDragging(false);
    RaycastHit hitInfo;
    if (this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo))
    {
      Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) hitInfo.transform);
      if ((UnityEngine.Object) componentInParents == (UnityEngine.Object) null || (UnityEngine.Object) componentInParents.GetCard() == (UnityEngine.Object) null)
        this.HandleRightClick();
      else if ((UnityEngine.Object) componentInParents.GetCard().gameObject == (UnityEngine.Object) objectRightMousedDown)
        this.HandleRightClickOnCard(componentInParents.GetCard());
      else
        this.HandleRightClick();
    }
    else
      this.HandleRightClick();
  }

  private void HandleRightClick()
  {
    if (!this.HasPendingChoiceTarget() && this.CancelOption())
    {
      if (!((UnityEngine.Object) this.m_mousedOverCard != (UnityEngine.Object) null) || !(this.m_mousedOverCard.GetZone() is ZonePlay) || !this.m_mousedOverCard.GetEntity().IsMinion())
        return;
      this.m_mousedOverCard.SetShouldShowTooltip();
      this.m_mousedOverCard.ShowTooltip();
    }
    else
    {
      EmoteHandler emoteHandler = EmoteHandler.Get();
      if ((UnityEngine.Object) emoteHandler != (UnityEngine.Object) null)
      {
        if (emoteHandler.AreEmotesActive())
          emoteHandler.HideEmotes();
      }
      else
      {
        BattlegroundsEmoteHandler handler;
        if (BattlegroundsEmoteHandler.TryGetActiveInstance(out handler))
          handler.HideEmotes();
      }
      EnemyEmoteHandler enemyEmoteHandler = EnemyEmoteHandler.Get();
      if (!((UnityEngine.Object) enemyEmoteHandler != (UnityEngine.Object) null) || !enemyEmoteHandler.AreEmotesActive())
        return;
      enemyEmoteHandler.HideEmotes();
    }
  }

  private bool CancelOption(bool timeout = false)
  {
    bool flag = false;
    if (this.m_gameState.IsInMainOptionMode())
      this.m_gameState.CancelCurrentOptionMode();
    if (this.CancelTargetMode())
      flag = true;
    if (this.CancelSubOptionMode(timeout))
      flag = true;
    if (this.DropHeldCard(true))
      flag = true;
    if ((bool) (UnityEngine.Object) this.m_mousedOverCard)
      this.m_mousedOverCard.UpdateProposedManaUsage();
    return flag;
  }

  private bool CancelTargetMode()
  {
    if (!this.m_gameState.IsInTargetMode() && !this.m_hadPendingChoiceTargetLastFrame)
      return false;
    bool flag = true;
    Network.Options.Option selectedNetworkOption = this.m_gameState.GetSelectedNetworkOption();
    if (selectedNetworkOption != null)
    {
      Entity entity = this.m_gameState.GetEntity(selectedNetworkOption.Main.ID);
      if (entity != null && entity.IsLettuceAbility())
        flag = false;
    }
    if (flag)
      SoundManager.Get().LoadAndPlay((AssetReference) "CancelAttack.prefab:9cde7207a78024e46aa5a0a657807845");
    if ((bool) (UnityEngine.Object) this.m_mousedOverCard)
      this.DisableSkullIfNeeded(this.m_mousedOverCard);
    this.m_targetReticleManager?.DestroyFriendlyTargetArrow(true);
    ZoneMgr.Get().DisplayLettuceAbilitiesForPreviouslySelectedCard();
    this.ResetBattlecrySourceCard();
    this.CancelSubOptions();
    this.m_gameState.CancelCurrentOptionMode();
    return true;
  }

  private bool CancelSubOptionMode(bool timeout = false)
  {
    if (!this.m_gameState.IsInSubOptionMode())
      return false;
    if (ChoiceCardMgr.Get().IsWaitingToShowSubOptions())
    {
      if (timeout)
        this.StartCoroutine(this.WaitAndCancelSubOptionMode());
      return false;
    }
    this.CancelSubOptions();
    this.m_gameState.CancelCurrentOptionMode();
    return true;
  }

  private IEnumerator WaitAndCancelSubOptionMode()
  {
    ChoiceCardMgr choiceCardMgr = ChoiceCardMgr.Get();
    choiceCardMgr.QuenePendingCancelSubOptions();
    while (choiceCardMgr.IsWaitingToShowSubOptions())
      yield return (object) null;
    if (choiceCardMgr.HasPendingCancelSubOptions())
    {
      this.CancelSubOptions();
      if (this.m_gameState.IsInSubOptionMode())
        this.m_gameState.CancelCurrentOptionMode();
    }
    choiceCardMgr.ClearPendingCancelSubOptions();
  }

  private bool AllowMovingMinionAcrossPlayZone() => false;

  private bool IsOverFriendlyPlayZone(RaycastHit hitInfo) => (double) hitInfo.point.z < -4.0;

  private void PositionHeldCard()
  {
    Card heldCard = this.m_heldCard;
    Entity entity = heldCard.GetEntity();
    ZonePlay controllersPlayZone = this.GetControllersPlayZone(entity);
    MoveMinionHoverTarget minionHoverTarget = this.GetMoveMinionHoverTarget(heldCard);
    if ((UnityEngine.Object) minionHoverTarget != (UnityEngine.Object) null)
    {
      controllersPlayZone.SortWithSpotForHeldCard(-1);
      heldCard.NotifyOverMoveMinionTarget(minionHoverTarget);
    }
    else
    {
      RaycastHit hitInfo;
      if (this.m_universalInputManager.GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out hitInfo))
      {
        if (!heldCard.IsOverPlayfield())
        {
          if (this.m_gameState.HasResponse(entity))
            heldCard.NotifyOverPlayfield();
          else if (!entity.IsTradeable() || !this.m_gameState.IsFriendlySidePlayerTurn() || !this.m_gameState.IsCombatStep())
          {
            this.m_leftMouseButtonIsDown = false;
            this.m_lastObjectMousedDown = (GameObject) null;
            this.SetDragging(false);
            this.DropHeldCard();
            return;
          }
        }
        if (entity.IsMinion())
        {
          if (this.AllowMovingMinionAcrossPlayZone())
          {
            ZoneMgr zoneMgr = ZoneMgr.Get();
            ZonePlay zoneOfType1 = zoneMgr.FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
            ZonePlay zoneOfType2 = zoneMgr.FindZoneOfType<ZonePlay>(Player.Side.OPPOSING);
            ZonePlay playZone = this.IsOverFriendlyPlayZone(hitInfo) ? zoneOfType1 : zoneOfType2;
            ZonePlay zonePlay = zoneOfType1.GetSlotMousedOver() > -1 ? zoneOfType1 : zoneOfType2;
            if ((UnityEngine.Object) playZone != (UnityEngine.Object) zonePlay)
              zonePlay.SortWithSpotForHeldCard(-1);
            int slot = this.PlayZoneSlotMousedOver(playZone, heldCard);
            if (slot >= 0 && playZone.HasMousedOverSlotChanged(slot))
              playZone.SortWithSpotForHeldCard(slot);
          }
          else
          {
            int slot = this.PlayZoneSlotMousedOver(controllersPlayZone, heldCard);
            if (slot >= 0 && controllersPlayZone.HasMousedOverSlotChanged(slot))
              controllersPlayZone.SortWithSpotForHeldCard(slot);
          }
        }
        else if (entity.IsLocation())
        {
          int slot = this.PlayZoneSlotMousedOver(controllersPlayZone, heldCard);
          if (slot >= 0 && controllersPlayZone.HasMousedOverSlotChanged(slot))
            controllersPlayZone.SortWithSpotForHeldCard(slot);
        }
      }
      else
      {
        bool flag = entity.GetZone() == TAG_ZONE.PLAY;
        if (heldCard.IsOverPlayfield() && !flag)
        {
          heldCard.NotifyLeftPlayfield();
          controllersPlayZone.SortWithSpotForHeldCard(-1);
        }
      }
    }
    if ((UnityEngine.Object) minionHoverTarget == (UnityEngine.Object) null && heldCard.IsOverMoveMinionTarget())
      heldCard.NotifyLeftMoveMinionTarget();
    RaycastHit hitInfo1;
    if (!this.m_universalInputManager.GetInputHitInfo(Camera.main, GameLayer.DragPlane, out hitInfo1))
      return;
    heldCard.transform.position = hitInfo1.point;
  }

  private int GetNumberOfUsedSlotsInPlay(ZonePlay play) => play.GetCards().Count<Card>((Func<Card, bool>) (c => !c.IsBeingDragged));

  public bool IsHeldCardLocation() => this.IsHeldCardLocation(out Entity _);

  public bool IsHeldCardLocation(out Entity location)
  {
    Card heldCard = this.GetHeldCard();
    if ((UnityEngine.Object) heldCard != (UnityEngine.Object) null && heldCard.GetEntity() != null)
    {
      Entity entity = heldCard.GetEntity();
      if (entity.IsLocation())
      {
        location = entity;
        return true;
      }
    }
    location = (Entity) null;
    return false;
  }

  public bool IsHeldCardMinion() => this.IsHeldCardMinion(out Entity _);

  public bool IsHeldCardMinion(out Entity minion)
  {
    Card heldCard = this.GetHeldCard();
    if ((UnityEngine.Object) heldCard != (UnityEngine.Object) null && heldCard.GetEntity() != null)
    {
      Entity entity = heldCard.GetEntity();
      if (entity.IsMinion())
      {
        minion = entity;
        return true;
      }
    }
    minion = (Entity) null;
    return false;
  }

  private int PlayZoneSlotMousedOver(ZonePlay playZone, Card card)
  {
    if ((UnityEngine.Object) playZone == (UnityEngine.Object) null)
      return -1;
    int num1 = 0;
    RaycastHit hitInfo;
    if (this.m_universalInputManager.GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out hitInfo))
    {
      int ofUsedSlotsInPlay = this.GetNumberOfUsedSlotsInPlay(playZone);
      int friendlySlotsPerPlayer = this.m_gameState.GetMaxFriendlySlotsPerPlayer();
      Entity entity = card.GetEntity();
      if (ofUsedSlotsInPlay >= friendlySlotsPerPlayer && entity != null && !GameState.Get().IsValidOption(entity))
        return -1;
      float slotWidth = playZone.GetSlotWidth();
      float num2 = playZone.transform.position.x - (float) ((double) (ofUsedSlotsInPlay + 1) * (double) slotWidth / 2.0);
      num1 = (int) Mathf.Ceil((hitInfo.point.x - num2) / slotWidth) - 1;
      if (num1 < 0 || num1 > ofUsedSlotsInPlay)
        num1 = (double) card.transform.position.x >= (double) playZone.transform.position.x ? ofUsedSlotsInPlay : 0;
    }
    return num1 + 1;
  }

  private void HandleUpdateWhileLeftMouseButtonIsDown()
  {
    if (this.HasPlayFromMiniHandEnabled() && !this.m_myHandZone.IsCardFocused && !this.WaitingForTouchDelay())
      this.m_myHandZone.UpdateLayout((Card) null, true);
    if (this.m_universalInputManager.IsTouchMode() && (UnityEngine.Object) this.m_heldCard == (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) this.GetBattlecrySourceCard() == (UnityEngine.Object) null)
        this.m_myHandZone.HandleInput();
      Card card = (UnityEngine.Object) this.m_myHandZone.CurrentStandIn != (UnityEngine.Object) null ? this.m_myHandZone.CurrentStandIn.linkedCard : (Card) null;
      if ((UnityEngine.Object) card != (UnityEngine.Object) this.m_lastPreviewedCard)
      {
        if ((UnityEngine.Object) card != (UnityEngine.Object) null)
          this.m_lastMouseDownPosition.y = InputCollection.GetMousePosition().y;
        this.m_lastPreviewedCard = card;
      }
    }
    if (this.m_dragging || (UnityEngine.Object) this.m_lastObjectMousedDown == (UnityEngine.Object) null)
      return;
    if ((bool) (UnityEngine.Object) this.m_lastObjectMousedDown.GetComponent<HistoryCard>())
    {
      this.m_lastObjectMousedDown = (GameObject) null;
      this.m_leftMouseButtonIsDown = false;
    }
    else
    {
      Vector3 mousePosition = InputCollection.GetMousePosition();
      float num1 = mousePosition.y - this.m_lastMouseDownPosition.y;
      float num2 = mousePosition.x - this.m_lastMouseDownPosition.x;
      if ((double) num2 > -20.0 && (double) num2 < 20.0 && (double) num1 > -20.0 && (double) num1 < 20.0)
        return;
      bool flag1 = !this.m_universalInputManager.IsTouchMode() || (double) num1 > (double) (float) this.MIN_GRAB_Y;
      CardStandIn cardStandIn = this.m_lastObjectMousedDown.GetComponent<CardStandIn>();
      if ((UnityEngine.Object) cardStandIn != (UnityEngine.Object) null && this.m_gameState != null && !this.m_gameState.IsMulliganManagerActive())
      {
        if (this.m_universalInputManager.IsTouchMode())
        {
          if (!flag1)
            return;
          cardStandIn = this.m_myHandZone.CurrentStandIn;
          if ((UnityEngine.Object) cardStandIn == (UnityEngine.Object) null)
            return;
        }
        if (ChoiceCardMgr.Get().IsFriendlyShown() || this.m_gameState.IsInChoiceMode() || !((UnityEngine.Object) this.GetBattlecrySourceCard() == (UnityEngine.Object) null) || !this.IsInZone(cardStandIn.linkedCard, TAG_ZONE.HAND))
          return;
        this.SetDragging(true);
        this.GrabCard(cardStandIn.linkedCard.gameObject);
      }
      else
      {
        if (this.m_gameState.IsMulliganManagerActive() || this.m_gameState.IsInTargetMode())
          return;
        Card component = this.m_lastObjectMousedDown.GetComponent<Card>();
        Entity entity = component.GetEntity();
        if (this.IsInZone(component, TAG_ZONE.HAND))
        {
          if (!entity.IsControlledByLocalUser() || !flag1 || this.m_universalInputManager.IsTouchMode() && !this.m_gameState.HasResponse(entity) || component.GetZone().m_ServerTag != TAG_ZONE.HAND && !this.m_gameState.HasResponse(entity) || ChoiceCardMgr.Get().IsFriendlyShown() || !((UnityEngine.Object) this.GetBattlecrySourceCard() == (UnityEngine.Object) null))
            return;
          this.SetDragging(true);
          this.GrabCard(this.m_lastObjectMousedDown);
        }
        else
        {
          if (!this.IsInZone(component, TAG_ZONE.PLAY))
            return;
          bool flag2 = entity.IsCardButton();
          if ((flag2 || entity.IsMoveMinionHoverTarget()) && (!flag2 || !this.m_gameState.EntityHasTargets(entity)))
            return;
          this.SetDragging(true);
          this.HandleClickOnCardInBattlefield(entity);
        }
      }
    }
  }

  private void HandleUpdateWhileHoldingCard(bool hitBattlefield)
  {
    PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
    Card heldCard = this.m_heldCard;
    if (!heldCard.IsInputEnabled())
    {
      this.DropHeldCard();
    }
    else
    {
      Entity entity = heldCard.GetEntity();
      if (hitBattlefield && (bool) (UnityEngine.Object) this.m_targetReticleManager && !this.m_targetReticleManager.IsActive() && this.m_gameState.EntityHasTargets(entity) && entity.GetCardType() != TAG_CARDTYPE.MINION && !this.m_gameState.EntityOnlyTrades(entity) && !this.m_heldCard.IsInTradeArea())
      {
        if (!this.DoNetworkResponse(entity))
        {
          this.PositionHeldCard();
          return;
        }
        DragCardSoundEffects component = heldCard.GetComponent<DragCardSoundEffects>();
        if ((bool) (UnityEngine.Object) component)
          component.Disable();
        RemoteActionHandler remoteActionHandler = RemoteActionHandler.Get();
        remoteActionHandler.NotifyOpponentOfCardPickedUp(heldCard);
        remoteActionHandler.NotifyOpponentOfTargetModeBegin(heldCard);
        bool useHandAsOrigin = entity.GetHero() == null;
        this.m_targetReticleManager.CreateFriendlyTargetArrow(entity, true, useHandAsOrigin: useHandAsOrigin);
        this.ActivatePowerUpSpell(heldCard);
        this.ActivatePlaySpell(heldCard);
      }
      else
      {
        bool insideHandLastFrame = this.m_cardWasInsideHandLastFrame;
        if (hitBattlefield && this.m_cardWasInsideHandLastFrame)
        {
          RemoteActionHandler.Get().NotifyOpponentOfCardPickedUp(heldCard);
          this.m_cardWasInsideHandLastFrame = false;
        }
        else if (!hitBattlefield)
          this.m_cardWasInsideHandLastFrame = true;
        this.PositionHeldCard();
        if (hitBattlefield)
        {
          this.m_myPlayZone.OnMagneticHeld(this.m_heldCard);
          this.m_myHandZone.OnCardHeld(this.m_heldCard);
        }
        else if (insideHandLastFrame)
        {
          this.m_myHandZone.OnTwinspellDropped(this.m_heldCard);
          this.m_myPlayZone.OnMagneticDropped(this.m_heldCard);
        }
        if (this.m_gameState.GetResponseMode() == GameState.ResponseMode.SUB_OPTION)
          this.CancelSubOptionMode();
      }
      if (!this.m_universalInputManager.IsTouchMode() || hitBattlefield || !((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null) || (double) InputCollection.GetMousePosition().y - (double) this.m_lastMouseDownPosition.y >= (double) (float) this.MIN_GRAB_Y || this.IsInZone(this.m_heldCard, TAG_ZONE.PLAY))
        return;
      this.m_myHandZone.OnTwinspellDropped(this.m_heldCard);
      this.m_myPlayZone.OnMagneticDropped(this.m_heldCard);
      PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
      this.ReturnHeldCardToHand();
    }
  }

  private MoveMinionHoverTarget GetMoveMinionHoverTarget(Card heldCard)
  {
    if ((UnityEngine.Object) heldCard == (UnityEngine.Object) null)
      return (MoveMinionHoverTarget) null;
    RaycastHit hitInfo;
    if (this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo))
    {
      MoveMinionHoverTarget componentInParent = hitInfo.transform.gameObject.GetComponentInParent<MoveMinionHoverTarget>();
      if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
        return componentInParent;
    }
    return (MoveMinionHoverTarget) null;
  }

  private void ActivatePowerUpSpell(Card card)
  {
    Entity entity = card.GetEntity();
    if (entity.IsSpell() || entity.IsMinion() || entity.IsLettuceAbility())
    {
      Spell actorSpell = card.GetActorSpell(SpellType.POWER_UP);
      if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
        actorSpell.ActivateState(SpellStateType.BIRTH);
    }
    card.DeactivateHandStateSpells();
  }

  private void ActivatePlaySpell(Card card)
  {
    Entity entity = card.GetEntity();
    if (entity.HasTag(GAME_TAG.CARD_DOES_NOTHING))
      return;
    Entity parentEntity = entity.GetParentEntity();
    Spell spell = parentEntity != null ? parentEntity.GetCard().GetSubOptionSpell(parentEntity.GetSubCardIndex(entity), 0) : card.GetPlaySpell(0);
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || spell.GetActiveState() != SpellStateType.NONE)
      return;
    spell.ActivateState(SpellStateType.BIRTH);
  }

  private void HandleMouseMove()
  {
    if (this.m_gameState == null || !this.m_gameState.IsInTargetMode())
      return;
    this.HandleUpdateWhileNotHoldingCard();
  }

  private void HandleUpdateWhileNotHoldingCard()
  {
    if (!this.m_universalInputManager.IsTouchMode() || !this.m_targetReticleManager.IsLocalArrowActive())
      this.m_myHandZone.HandleInput();
    RaycastHit hitInfo;
    if ((!this.m_universalInputManager.IsTouchMode() ? 0 : (!InputCollection.GetMouseButton(0) ? 1 : 0)) == 0 && this.m_universalInputManager.GetInputHitInfo(GameLayer.CardRaycast, out hitInfo))
    {
      CardStandIn cardStandIn = (CardStandIn) null;
      Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>((Component) hitInfo.transform);
      if ((UnityEngine.Object) componentInParents == (UnityEngine.Object) null)
      {
        cardStandIn = GameObjectUtils.FindComponentInParents<CardStandIn>((Component) hitInfo.transform);
        if ((UnityEngine.Object) cardStandIn == (UnityEngine.Object) null)
        {
          this.HandleMouseOverObjectWhileNotHoldingCard(hitInfo);
          return;
        }
      }
      if ((UnityEngine.Object) this.m_mousedOverObject != (UnityEngine.Object) null)
        this.HandleMouseOffLastObject();
      Card card = (Card) null;
      if ((UnityEngine.Object) componentInParents != (UnityEngine.Object) null)
        card = componentInParents.GetCard();
      if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      {
        if (this.m_gameState == null || this.m_gameState.IsMulliganManagerActive())
        {
          if (!((UnityEngine.Object) this.m_mousedOverCard != (UnityEngine.Object) null))
            return;
          this.HandleMouseOffCard();
          return;
        }
        if ((UnityEngine.Object) cardStandIn == (UnityEngine.Object) null)
          return;
        card = cardStandIn.linkedCard;
      }
      if (this.IsCancelingBattlecryCard(card) || this.m_useHandEnlarge && this.m_myHandZone.HandEnlarged() && card.GetEntity().IsCardButton() && !card.GetEntity().IsLocation() && card.GetEntity().IsControlledByLocalUser() && this.m_myHandZone.GetCardCount() > 1)
        return;
      if ((UnityEngine.Object) card != (UnityEngine.Object) this.m_mousedOverCard && ((UnityEngine.Object) card.GetZone() != (UnityEngine.Object) this.m_myHandZone || this.m_gameState.IsMulliganManagerActive()))
      {
        if ((UnityEngine.Object) this.m_mousedOverCard != (UnityEngine.Object) null)
          this.HandleMouseOffCard();
        this.HandleMouseOverCard(card);
      }
      PegCursor.Get().SetMode(PegCursor.Mode.OVER);
    }
    else
      this.HandleMouseOff();
  }

  private void HandleMouseOverObjectWhileNotHoldingCard(RaycastHit hitInfo)
  {
    if ((UnityEngine.Object) this.m_mousedOverCard != (UnityEngine.Object) null)
      this.HandleMouseOffCard();
    if (this.m_universalInputManager.IsTouchMode() && !InputCollection.GetMouseButton(0))
    {
      if (!((UnityEngine.Object) this.m_mousedOverObject != (UnityEngine.Object) null))
        return;
      this.HandleMouseOffLastObject();
    }
    else
    {
      bool flag1 = (UnityEngine.Object) this.m_targetReticleManager != (UnityEngine.Object) null && this.m_targetReticleManager.IsLocalArrowActive();
      bool flag2 = this.PermitDecisionMakingInput();
      if (!flag2)
        flag1 = false;
      GameObject gameObject = hitInfo.collider.gameObject;
      if ((UnityEngine.Object) gameObject.GetComponent<HistoryManager>() != (UnityEngine.Object) null && !flag1)
      {
        this.m_mousedOverObject = gameObject;
        HistoryManager.Get().NotifyOfInput(hitInfo.point.z);
      }
      else if ((UnityEngine.Object) gameObject.GetComponent<PlayerLeaderboardManager>() != (UnityEngine.Object) null && !flag1)
      {
        this.m_mousedOverObject = gameObject;
        PlayerLeaderboardManager.Get().NotifyOfInput(hitInfo.point);
      }
      else
      {
        BattlegroundsEmoteHandler handler;
        if (BattlegroundsEmoteHandler.TryGetActiveInstance(out handler))
        {
          BattlegroundsEmoteOption component;
          if (gameObject.TryGetComponent<BattlegroundsEmoteOption>(out component))
          {
            this.m_mousedOverObject = gameObject;
            handler.HandleMouseOver(component);
            return;
          }
          if ((UnityEngine.Object) gameObject == (UnityEngine.Object) handler.gameObject)
          {
            this.m_mousedOverObject = gameObject;
            handler.HandleMouseOut();
            return;
          }
        }
        if ((UnityEngine.Object) this.m_mousedOverObject == (UnityEngine.Object) gameObject)
          return;
        if ((UnityEngine.Object) this.m_mousedOverObject != (UnityEngine.Object) null)
          this.HandleMouseOffLastObject();
        if ((bool) (UnityEngine.Object) EndTurnButton.Get() & flag2 && !EndTurnButton.Get().IsInputBlocked())
        {
          if ((UnityEngine.Object) gameObject.GetComponent<EndTurnButton>() != (UnityEngine.Object) null)
          {
            this.m_mousedOverObject = gameObject;
            EndTurnButton.Get().HandleMouseOver();
          }
          else
          {
            EndTurnButtonReminder component = gameObject.GetComponent<EndTurnButtonReminder>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.ShowFriendlySidePlayerTurnReminder())
              this.m_mousedOverObject = gameObject;
          }
        }
        TooltipZone component1 = gameObject.GetComponent<TooltipZone>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        {
          this.m_mousedOverObject = gameObject;
          this.ShowTooltipZone(gameObject, component1);
        }
        GameOpenPack component2 = gameObject.GetComponent<GameOpenPack>();
        if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
        {
          this.m_mousedOverObject = gameObject;
          component2.NotifyOfMouseOver();
        }
        int num = (UnityEngine.Object) this.GetBattlecrySourceCard() != (UnityEngine.Object) null ? 1 : 0;
      }
    }
  }

  private void HandleMouseOff()
  {
    if ((bool) (UnityEngine.Object) this.m_mousedOverCard && (UnityEngine.Object) this.m_mousedOverCard != (UnityEngine.Object) RemoteActionHandler.Get().GetFriendlyHoverCard())
      this.HandleMouseOffCard();
    if (!(bool) (UnityEngine.Object) this.m_mousedOverObject)
      return;
    this.HandleMouseOffLastObject();
  }

  private void HandleMouseOffLastObject()
  {
    EndTurnButton component1 = this.m_mousedOverObject.GetComponent<EndTurnButton>();
    if ((bool) (UnityEngine.Object) component1)
    {
      component1.HandleMouseOut();
      this.m_lastObjectMousedDown = (GameObject) null;
    }
    else if ((bool) (UnityEngine.Object) this.m_mousedOverObject.GetComponent<EndTurnButtonReminder>())
    {
      this.m_lastObjectMousedDown = (GameObject) null;
    }
    else
    {
      TooltipZone component2 = this.m_mousedOverObject.GetComponent<TooltipZone>();
      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
      {
        component2.HideTooltip();
        this.m_lastObjectMousedDown = (GameObject) null;
      }
      else if ((UnityEngine.Object) this.m_mousedOverObject.GetComponent<HistoryManager>() != (UnityEngine.Object) null)
        HistoryManager.Get().NotifyOfMouseOff();
      else if ((UnityEngine.Object) this.m_mousedOverObject.GetComponent<PlayerLeaderboardManager>() != (UnityEngine.Object) null)
      {
        PlayerLeaderboardManager.Get().NotifyOfMouseOff();
      }
      else
      {
        BattlegroundsEmoteHandler handler;
        if (BattlegroundsEmoteHandler.TryGetActiveInstance(out handler) && ((UnityEngine.Object) handler.gameObject == (UnityEngine.Object) this.m_mousedOverObject || this.m_mousedOverObject.TryGetComponent<BattlegroundsEmoteOption>(out BattlegroundsEmoteOption _)))
        {
          handler.HideEmotes();
          this.m_lastObjectMousedDown = (GameObject) null;
        }
        else
        {
          GameOpenPack component3 = this.m_mousedOverObject.GetComponent<GameOpenPack>();
          if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
          {
            component3.NotifyOfMouseOff();
            this.m_lastObjectMousedDown = (GameObject) null;
          }
        }
      }
    }
    this.m_mousedOverObject = (GameObject) null;
  }

  private void SetHeldCardValue(Card newValue)
  {
    if ((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null && (UnityEngine.Object) newValue == (UnityEngine.Object) null)
    {
      Entity entity = this.m_heldCard.GetEntity();
      if (entity != null && entity.HasTag(GAME_TAG.TRADEABLE) && this.m_myHandZone.HandEnlarged())
        ManaCrystalMgr.Get().ShowPhoneManaTray();
      DragRotator component = this.m_heldCard.GetComponent<DragRotator>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) component);
    }
    else if ((UnityEngine.Object) this.m_heldCard == (UnityEngine.Object) null && (UnityEngine.Object) newValue != (UnityEngine.Object) null)
    {
      Entity entity = newValue.GetEntity();
      if (entity != null && entity.HasTag(GAME_TAG.TRADEABLE) && this.m_myHandZone.HandEnlarged())
        ManaCrystalMgr.Get().HidePhoneManaTray();
    }
    this.m_heldCard = newValue;
  }

  private void GrabCard(GameObject cardObject)
  {
    if (!this.PermitDecisionMakingInput())
      return;
    Card component = cardObject.GetComponent<Card>();
    if (!component.IsInputEnabled() || !this.m_gameState.GetGameEntity().ShouldAllowCardGrab(component.GetEntity()))
      return;
    Zone zone = component.GetZone();
    if (!zone.IsInputEnabled())
      return;
    component.SetDoNotSort(true);
    float num = 0.7f;
    switch (zone)
    {
      case ZoneHand _:
        ZoneHand zoneHand = (ZoneHand) zone;
        if (!this.m_universalInputManager.IsTouchMode())
          zoneHand.UpdateLayout((Card) null);
        zoneHand.OnCardGrabbed(component);
        break;
      case ZonePlay _:
        ZonePlay zonePlay = (ZonePlay) zone;
        zonePlay.RemoveCard(component);
        zonePlay.UpdateLayout();
        component.HideTooltip();
        num = 0.9f;
        break;
    }
    this.SetHeldCardValue(component);
    component.IsBeingDragged = true;
    SoundManager.Get().LoadAndPlay((AssetReference) "FX_MinionSummon01_DrawFromHand_01.prefab:c8adc026a7f5d0a4cb0706627a980c58", cardObject);
    DragCardSoundEffects cardSoundEffects = this.m_heldCard.GetComponent<DragCardSoundEffects>();
    if ((bool) (UnityEngine.Object) cardSoundEffects)
      cardSoundEffects.enabled = true;
    else
      cardSoundEffects = cardObject.AddComponent<DragCardSoundEffects>();
    cardSoundEffects.Restart();
    cardObject.AddComponent<DragRotator>().SetInfo(this.m_DragRotatorInfo);
    ProjectedShadow componentInChildren = component.GetActor().GetComponentInChildren<ProjectedShadow>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.EnableShadow(0.15f);
    iTween.Stop(cardObject);
    iTween.ScaleTo(cardObject, new Vector3(num, num, num), 0.2f);
    TooltipPanelManager.Get().HideKeywordHelp();
    CardTypeBanner.Get()?.Hide();
    component.NotifyPickedUp();
    this.m_gameState.GetGameEntity().NotifyOfCardGrabbed(component.GetEntity());
    LayerUtils.SetLayer((Component) component, GameLayer.Default);
  }

  private void DropCanceledHeldCard(Card card)
  {
    this.SetHeldCardValue((Card) null);
    RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
    ZonePlay controllersPlayZone = this.GetControllersPlayZone(card.GetEntity());
    this.m_myHandZone.UpdateLayout((Card) null, true);
    controllersPlayZone.SortWithSpotForHeldCard(-1);
    controllersPlayZone.OnMagneticDropped(card);
    this.m_myHandZone.OnTwinspellDropped(card);
    this.SendDragDropCancelPlayTelemetry(card.GetEntity());
    card.IsBeingDragged = false;
  }

  public void ReturnHeldCardToHand()
  {
    if ((UnityEngine.Object) this.m_heldCard == (UnityEngine.Object) null)
      return;
    Log.Hand.Print("ReturnHeldCardToHand()");
    Card heldCard = this.m_heldCard;
    heldCard.SetDoNotSort(false);
    iTween.Stop(this.m_heldCard.gameObject);
    Entity entity = heldCard.GetEntity();
    heldCard.NotifyLeftPlayfield();
    this.m_gameState.GetGameEntity().NotifyOfCardDropped(entity);
    DragCardSoundEffects component = heldCard.GetComponent<DragCardSoundEffects>();
    if ((bool) (UnityEngine.Object) component)
      component.Disable();
    ProjectedShadow componentInChildren = heldCard.GetActor().GetComponentInChildren<ProjectedShadow>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.DisableShadow();
    RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
    if (this.m_useHandEnlarge)
      this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
    this.m_myHandZone.UpdateLayout(this.m_myHandZone.GetLastMousedOverCard(), true);
    this.m_heldCard.IsBeingDragged = false;
    this.SetDragging(false);
    this.SetHeldCardValue((Card) null);
  }

  private bool DropHeldCard(bool wasCancelled)
  {
    Log.Hand.Print("DropHeldCard - cancelled? " + wasCancelled.ToString());
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    if (this.m_enlargeHandAfterDropCard)
    {
      this.m_enlargeHandAfterDropCard = false;
      this.ShowPhoneHand();
    }
    if (this.m_useHandEnlarge)
    {
      this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
      if (this.m_hideHandAfterPlayingCard)
        this.HidePhoneHand();
      else
        this.m_myHandZone.UpdateLayout((Card) null, true);
    }
    if ((UnityEngine.Object) this.m_heldCard == (UnityEngine.Object) null)
      return false;
    Card heldCard = this.m_heldCard;
    heldCard.SetDoNotSort(false);
    iTween.Stop(this.m_heldCard.gameObject);
    Entity entity = heldCard.GetEntity();
    heldCard.NotifyLeftPlayfield();
    heldCard.NotifyLeftMoveMinionTarget();
    this.m_gameState.GetGameEntity().NotifyOfCardDropped(entity);
    DragCardSoundEffects component = heldCard.GetComponent<DragCardSoundEffects>();
    if ((bool) (UnityEngine.Object) component)
      component.Disable();
    this.SetHeldCardValue((Card) null);
    ProjectedShadow componentInChildren = heldCard.GetActor().GetComponentInChildren<ProjectedShadow>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.DisableShadow();
    if (this.IsInZone(heldCard, TAG_ZONE.PLAY) && heldCard.IsInputEnabled())
    {
      MoveMinionHoverTarget minionHoverTarget = this.GetMoveMinionHoverTarget(heldCard);
      if ((UnityEngine.Object) minionHoverTarget != (UnityEngine.Object) null && !wasCancelled)
        minionHoverTarget.DropCardOnHoverTarget(heldCard);
      else
        this.AddHeldCardBackToPlayZone(heldCard);
      this.m_gameState.ExitMoveMinionMode();
    }
    if (this.IsInZone(heldCard, TAG_ZONE.PLAY))
      LayerUtils.SetLayer((Component) heldCard, GameLayer.CardRaycast);
    else
      LayerUtils.SetLayer((Component) heldCard, GameLayer.Default);
    if (wasCancelled)
    {
      if (entity != null && entity.HasTag(GAME_TAG.TRADEABLE))
        heldCard.HideTradeableHover();
      this.DropCanceledHeldCard(heldCard);
      return true;
    }
    bool notifyEnemyOfTargetArrow = false;
    if (this.IsInZone(heldCard, TAG_ZONE.HAND))
    {
      bool flag = entity.HasTag(GAME_TAG.TRADEABLE) && heldCard.IsInTradeArea();
      if (flag)
      {
        bool cancelDrop = false;
        this.DropHeldTadeable(entity, ref cancelDrop);
        if (cancelDrop)
        {
          this.DropCanceledHeldCard(heldCard);
          return true;
        }
        ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(Player.Side.FRIENDLY);
        if ((UnityEngine.Object) zoneOfType != (UnityEngine.Object) null)
          zoneOfType.HideTradeableGlow(true);
        Actor actor = heldCard.GetActor();
        if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
        {
          Spell spell = actor.GetSpell(SpellType.TRADEABLE_HOVER);
          if ((UnityEngine.Object) spell != (UnityEngine.Object) null && spell.GetActiveState() != SpellStateType.CANCEL)
            SpellUtils.ActivateDeathIfNecessary(spell);
        }
      }
      else if (entity.IsMinion() || entity.IsWeapon() || entity.IsLocation())
      {
        this.DropHeldMinionLikeCard(heldCard, entity, ref notifyEnemyOfTargetArrow);
        if (entity.IsMinion() && (UnityEngine.Object) heldCard.GetActor() != (UnityEngine.Object) null && !this.m_universalInputManager.IsTouchMode())
          heldCard.GetActor().TurnOffCollider();
      }
      else if (entity.IsSpell() || entity.IsHero() || entity.IsLettuceAbility() || entity.IsBattlegroundQuestReward())
      {
        bool cancelDrop = false;
        this.DropHeldSpellLikeCard(heldCard, entity, ref cancelDrop);
        if (cancelDrop)
        {
          this.DropCanceledHeldCard(entity.GetCard());
          return true;
        }
      }
      if (!flag)
        this.m_myHandZone.UpdateLayout((Card) null, true);
      this.m_myPlayZone.SortWithSpotForHeldCard(-1);
    }
    if (this.IsInZone(heldCard, TAG_ZONE.PLAY))
    {
      if (entity.IsMinion())
        this.DropHeldMinionLikeCard(heldCard, entity, ref notifyEnemyOfTargetArrow);
      this.GetControllersPlayZone(heldCard.GetEntity()).SortWithSpotForHeldCard(-1);
    }
    if (notifyEnemyOfTargetArrow)
      RemoteActionHandler.Get()?.NotifyOpponentOfTargetModeBegin(heldCard);
    else if (this.m_gameState.GetResponseMode() != GameState.ResponseMode.SUB_OPTION)
      RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
    return true;
  }

  public ZonePlay GetControllersPlayZone(Entity entity) => !entity.IsControlledByFriendlySidePlayer() ? this.m_enemyPlayZone : this.m_myPlayZone;

  public void AddHeldCardBackToPlayZone(Card card) => this.GetControllersPlayZone(card.GetEntity()).AddCard(card);

  private void SendDragDropCancelPlayTelemetry(Entity cancelledEntity)
  {
    if (cancelledEntity == null || GameMgr.Get() == null)
      return;
    TelemetryManager.Client().SendDragDropCancelPlayCard((long) GameMgr.Get().GetMissionId(), ((TAG_CARDTYPE) cancelledEntity.GetTag(GAME_TAG.CARDTYPE)).ToString());
  }

  private void DropHeldMinionLikeCard(Card card, Entity entity, ref bool notifyEnemyOfTargetArrow)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null || entity == null)
    {
      Debug.LogWarningFormat("DropHeldMinionLikeCard() is called with the invalid card or entity.");
    }
    else
    {
      ZonePlay controllersPlayZone = this.GetControllersPlayZone(card.GetEntity());
      bool flag1 = entity.IsMinion() || entity.IsLocation();
      bool flag2 = entity.IsWeapon();
      if (!flag1 && !flag2)
      {
        Debug.LogWarningFormat("DropHeldMinionLikeCard() is called with the card: {0}", (object) entity.GetCardId());
        card.IsBeingDragged = false;
      }
      else
      {
        RaycastHit hitInfo;
        if (!this.m_universalInputManager.GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out hitInfo))
        {
          controllersPlayZone.OnMagneticDropped(card);
          this.SendDragDropCancelPlayTelemetry(entity);
          card.IsBeingDragged = false;
        }
        else
        {
          Zone zone = !flag2 ? (Zone) controllersPlayZone : (Zone) this.m_myWeaponZone;
          bool flag3 = this.AllowMovingMinionAcrossPlayZone();
          if (flag1 & flag3)
          {
            Player.Side side = this.IsOverFriendlyPlayZone(hitInfo) ? Player.Side.FRIENDLY : Player.Side.OPPOSING;
            zone = (Zone) ZoneMgr.Get().FindZoneOfType<ZonePlay>(side);
          }
          if ((bool) (UnityEngine.Object) zone)
          {
            int num1 = 0;
            int num2 = 0;
            if (flag1)
            {
              num1 = this.PlayZoneSlotMousedOver(zone as ZonePlay, card);
              if (num1 < 0)
              {
                PlayErrors.DisplayPlayError(PlayErrors.ErrorType.REQ_MINION_CAP, new int?(), entity);
                return;
              }
              if (flag3)
              {
                num2 = num1;
                this.m_gameState.SetSelectedOptionPosition(num2);
                this.m_gameState.SetSelectedOptionTarget(zone.GetController().GetEntityId());
              }
              else
              {
                num2 = ZoneMgr.Get().PredictZonePosition(zone, num1);
                this.m_gameState.SetSelectedOptionPosition(num2);
              }
              if (num2 < 0)
                return;
            }
            if (this.DoNetworkResponse(entity))
            {
              if (this.IsInZone(card, TAG_ZONE.HAND))
              {
                this.m_lastZoneChangeList = ZoneMgr.Get().AddPredictedLocalZoneChange(card, zone, num1, num2);
                this.PredictSpentMana(entity);
                controllersPlayZone.OnMagneticPlay(card, num2);
                if (flag1 && this.m_gameState.EntityHasTargets(entity))
                {
                  notifyEnemyOfTargetArrow = true;
                  bool showArrow = !this.m_universalInputManager.IsTouchMode();
                  this.m_targetReticleManager?.CreateFriendlyTargetArrow(entity, true, showArrow);
                  this.m_battlecrySourceCard = card;
                  if (this.m_universalInputManager.IsTouchMode())
                    this.StartBattleCryEffect(entity);
                }
              }
              else if (this.IsInZone(card, TAG_ZONE.PLAY) && ((UnityEngine.Object) card.GetZone() != (UnityEngine.Object) zone || card.GetZonePosition() != num2))
              {
                this.m_lastZoneChangeList = ZoneMgr.Get().AddPredictedLocalZoneChange(card, zone, num1, num2);
                card.m_minionWasMovedFromSrcToDst = new ZonePositionChange()
                {
                  m_sourceZonePosition = card.GetZonePosition(),
                  m_destinationZonePosition = num1
                };
              }
            }
            else
              this.m_gameState.SetSelectedOptionPosition(0);
          }
          card.IsBeingDragged = false;
        }
      }
    }
  }

  private void DropHeldSpellLikeCard(Card card, Entity entity, ref bool cancelDrop)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null || entity == null)
      Debug.LogWarningFormat("DropHeldSpellLikeCard() is called with the invalid card or entity.");
    else if (!entity.IsSpell() && !entity.IsHero() && !entity.IsLettuceAbility() && !entity.IsBattlegroundQuestReward())
      Debug.LogWarningFormat("DropHeldSpellLikeCard() is called with the card: {0}", (object) entity.GetCardId());
    else if (this.m_gameState.EntityHasTargets(entity) && !entity.HasTag(GAME_TAG.TRADEABLE))
      cancelDrop = true;
    else if (!this.m_universalInputManager.GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out RaycastHit _))
    {
      this.m_myHandZone.OnTwinspellDropped(card);
      this.SendDragDropCancelPlayTelemetry(entity);
    }
    else if (!this.m_gameState.HasResponse(entity, new bool?(false)))
    {
      PlayErrors.DisplayPlayError(this.m_gameState.GetErrorType(entity), this.m_gameState.GetErrorParam(entity), entity);
    }
    else
    {
      this.m_myHandZone.OnTwinspellPlayed(card);
      this.DoNetworkResponse(entity);
      this.m_lastZoneChangeList = ZoneMgr.Get().AddLocalZoneChange(card, TAG_ZONE.PLAY);
      this.PredictSpentMana(entity);
      if (!entity.IsSpell())
        return;
      if (this.m_gameState.HasSubOptions(entity))
      {
        card.DeactivateHandStateSpells();
      }
      else
      {
        this.ActivatePowerUpSpell(card);
        this.ActivatePlaySpell(card);
      }
    }
  }

  private void DropHeldTadeable(Entity entity, ref bool cancelDrop)
  {
    bool flag = this.DoNetworkResponse(entity, wantTradeOption: true);
    if (!flag)
    {
      Card card = entity.GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null && !card.HasEnoughManaToTrade())
        PlayErrors.DisplayPlayError(PlayErrors.ErrorType.REQ_ENOUGH_MANA, new int?(), entity);
      else
        PlayErrors.DisplayPlayError(PlayErrors.ErrorType.REQ_TRADEABLE, new int?(), entity);
    }
    cancelDrop = !flag;
  }

  private void HandleRightClickOnCard(Card card)
  {
    if (this.m_gameState.IsInTargetMode() || this.m_gameState.IsInSubOptionMode() || (UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null)
    {
      this.HandleRightClick();
    }
    else
    {
      Entity entity = card.GetEntity();
      if (!entity.IsHero())
        return;
      if (entity.IsControlledByLocalUser())
      {
        EmoteHandler emoteHandler = EmoteHandler.Get();
        if ((UnityEngine.Object) emoteHandler != (UnityEngine.Object) null)
        {
          if (emoteHandler.AreEmotesActive())
            emoteHandler.HideEmotes();
          else
            emoteHandler.ShowEmotes();
        }
        else
        {
          BattlegroundsEmoteHandler battlegroundsEmoteHandler = BattlegroundsEmoteHandler.Get();
          if (!GameMgr.Get().IsBattlegroundsMatchOrTutorial() || !((UnityEngine.Object) battlegroundsEmoteHandler != (UnityEngine.Object) null))
            return;
          if (battlegroundsEmoteHandler.AreEmotesActive())
            battlegroundsEmoteHandler.HideEmotes();
          else
            battlegroundsEmoteHandler.ShowEmotes();
        }
      }
      else
      {
        EnemyEmoteHandler enemyEmoteHandler = EnemyEmoteHandler.Get();
        bool flag = (UnityEngine.Object) enemyEmoteHandler != (UnityEngine.Object) null;
        if (GameMgr.Get().IsSpectator() && entity.GetControllerSide() != Player.Side.OPPOSING)
          flag = false;
        if (!flag)
          return;
        if (enemyEmoteHandler.AreEmotesActive())
          enemyEmoteHandler.HideEmotes();
        else
          enemyEmoteHandler.ShowEmotes();
      }
    }
  }

  private void HandleClickOnCard(GameObject upClickedCard, bool wasMouseDownTarget)
  {
    EmoteHandler emoteHandler = EmoteHandler.Get();
    if ((UnityEngine.Object) emoteHandler != (UnityEngine.Object) null)
    {
      if (emoteHandler.IsMouseOverEmoteOption())
        return;
      emoteHandler.HideEmotes();
    }
    BattlegroundsEmoteHandler battlegroundsEmoteHandler = BattlegroundsEmoteHandler.Get();
    if (GameMgr.Get().IsBattlegroundsMatchOrTutorial() && (UnityEngine.Object) battlegroundsEmoteHandler != (UnityEngine.Object) null)
    {
      if (battlegroundsEmoteHandler.IsMouseOverEmoteOption)
        return;
      battlegroundsEmoteHandler.HideEmotes();
    }
    EnemyEmoteHandler enemyEmoteHandler = EnemyEmoteHandler.Get();
    if ((UnityEngine.Object) enemyEmoteHandler != (UnityEngine.Object) null)
    {
      if (enemyEmoteHandler.IsMouseOverEmoteOption())
        return;
      enemyEmoteHandler.HideEmotes();
    }
    Card component = upClickedCard.GetComponent<Card>();
    Entity entity = component.GetEntity();
    Log.Hand.Print("HandleClickOnCard - Card zone: " + (object) component.GetZone());
    if (((!this.m_universalInputManager.IsTouchMode() || !entity.IsHero() || !(component.GetZone() is ZoneHero) ? 0 : (!this.m_gameState.IsInTargetMode() ? 1 : 0)) & (wasMouseDownTarget ? 1 : 0)) != 0)
    {
      if (entity.IsControlledByLocalUser())
      {
        if ((UnityEngine.Object) emoteHandler != (UnityEngine.Object) null)
        {
          emoteHandler.ShowEmotes();
          return;
        }
        if (!GameMgr.Get().IsBattlegroundsMatchOrTutorial() || !((UnityEngine.Object) battlegroundsEmoteHandler != (UnityEngine.Object) null))
          return;
        battlegroundsEmoteHandler.ShowEmotes();
        return;
      }
      if (!GameMgr.Get().IsSpectator() && (UnityEngine.Object) enemyEmoteHandler != (UnityEngine.Object) null)
      {
        enemyEmoteHandler.ShowEmotes();
        return;
      }
    }
    if (component.GetEntity().IsMoveMinionHoverTarget())
      return;
    if ((UnityEngine.Object) component == (UnityEngine.Object) ChoiceCardMgr.Get().GetSubOptionParentCard())
    {
      this.CancelOption();
    }
    else
    {
      GameState.ResponseMode responseMode = this.m_gameState.GetResponseMode();
      if (this.IsInZone(component, TAG_ZONE.HAND))
      {
        if (this.m_gameState.IsMulliganManagerActive())
        {
          if (!this.PermitDecisionMakingInput())
            return;
          MulliganManager.Get().ToggleHoldState(component);
        }
        else
        {
          if (component.IsAttacking() || this.m_gameState.IsInChoiceMode() || this.m_gameState.IsInTargetMode() || this.m_universalInputManager.IsTouchMode() || !component.GetEntity().IsControlledByLocalUser() || ChoiceCardMgr.Get().IsFriendlyShown() || !((UnityEngine.Object) this.GetBattlecrySourceCard() == (UnityEngine.Object) null) || component.GetZone().m_ServerTag != TAG_ZONE.HAND && !this.m_gameState.HasResponse(entity))
            return;
          this.GrabCard(upClickedCard);
        }
      }
      else if (responseMode == GameState.ResponseMode.SUB_OPTION)
        this.HandleClickOnSubOption(entity);
      else if (responseMode == GameState.ResponseMode.CHOICE)
      {
        this.HandleClickOnChoice(entity);
      }
      else
      {
        if (!this.IsInZone(component, TAG_ZONE.PLAY))
          return;
        this.HandleClickOnCardInBattlefield(entity, wasMouseDownTarget);
      }
    }
  }

  public float GetMouseOverDelay(Entity entity) => this.m_gameState.GetGameEntity().ShowMouseOverBigCardImmediately(entity) ? 0.0f : this.m_MouseOverDelay;

  private void HandleClickOnCardInBattlefield(Entity clickedEntity, bool wasMouseDownTarget = true)
  {
    if (!this.PermitDecisionMakingInput())
      return;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
    if (this.m_universalInputManager.IsTouchMode() && clickedEntity.IsCardButton() && !this.m_gameState.IsInTargetMode() && (double) this.m_mousedOverTimer > (double) this.GetMouseOverDelay(clickedEntity))
      return;
    Card card = clickedEntity.GetCard();
    if (clickedEntity.IsGameModeButton() && (UnityEngine.Object) card != (UnityEngine.Object) null && (UnityEngine.Object) card.GetPlaySpell(0) != (UnityEngine.Object) null && card.GetPlaySpell(0).GetActiveState() != SpellStateType.NONE)
      return;
    GameEntity gameEntity = this.m_gameState.GetGameEntity();
    if (!gameEntity.NotifyOfBattlefieldCardClicked(clickedEntity, this.m_gameState.IsInTargetMode()))
      return;
    if (this.m_gameState.IsInTargetMode())
    {
      this.DisableSkullIfNeeded(card);
      Network.Options.Option.SubOption networkSubOption = this.m_gameState.GetSelectedNetworkSubOption();
      if (networkSubOption.ID == clickedEntity.GetEntityId())
      {
        this.CancelOption();
      }
      else
      {
        this.UpdateTelemetryAttackInputCounts(this.m_gameState.GetEntity(networkSubOption.ID));
        if (!this.DoNetworkResponse(clickedEntity) || !((UnityEngine.Object) this.m_heldCard != (UnityEngine.Object) null))
          return;
        Card heldCard = this.m_heldCard;
        this.m_myHandZone.OnTwinspellPlayed(heldCard);
        this.SetHeldCardValue((Card) null);
        heldCard.SetDoNotSort(false);
        this.m_lastZoneChangeList = ZoneMgr.Get().AddLocalZoneChange(heldCard, TAG_ZONE.PLAY);
      }
    }
    else
    {
      ACTION_STEP_TYPE actionStepType = this.m_gameState.GetActionStepType();
      bool mouseButtonUp = InputCollection.GetMouseButtonUp(0);
      if (gameEntity is LettuceMissionEntity && this.m_gameState.IsActionStep() && actionStepType == ACTION_STEP_TYPE.DEFAULT)
      {
        if (!card.IsInputEnabled())
          return;
        int tag = clickedEntity.GetTag(GAME_TAG.LETTUCE_CONTROLLER);
        if (((mouseButtonUp ? 1 : (this.m_dragging ? 1 : 0)) & (wasMouseDownTarget ? 1 : 0)) != 0 && tag != 0 && tag == this.m_gameState.GetLocalSidePlayer().GetPlayerId())
        {
          if (clickedEntity.IsMinion())
          {
            if (this.m_gameState.IsResponsePacketBlocked() || ZoneMgr.Get().GetLettuceAbilitiesSourceEntity() == clickedEntity)
              return;
            this.HandleMouseOffCard();
            ZoneMgr.Get().DisplayLettuceAbilitiesForEntity(clickedEntity);
            RemoteActionHandler.Get().NotifyOpponentOfSelection(clickedEntity.GetEntityId());
            return;
          }
          if (clickedEntity.IsLettuceAbility())
          {
            Entity lettuceAbilityOwner = clickedEntity.GetLettuceAbilityOwner();
            if (lettuceAbilityOwner != null && lettuceAbilityOwner.GetSelectedLettuceAbilityID() == clickedEntity.GetEntityId())
            {
              this.CancelSelectedLettuceAbilityForEntity(lettuceAbilityOwner);
              return;
            }
          }
        }
      }
      if (mouseButtonUp && this.m_universalInputManager.IsTouchMode() && this.m_gameState.EntityHasTargets(clickedEntity))
      {
        if (card.IsShowingTooltip() || !this.m_gameState.IsFriendlySidePlayerTurn())
          return;
        PlayErrors.DisplayPlayError(PlayErrors.ErrorType.REQ_DRAG_TO_PLAY, new int?(), clickedEntity);
      }
      else if (clickedEntity.IsWeapon() && clickedEntity.IsControlledByLocalUser() && !this.m_gameState.IsValidOption(clickedEntity))
        this.HandleClickOnCardInBattlefield(this.m_gameState.GetFriendlySidePlayer().GetHero());
      else if (gameEntity.GetTag(GAME_TAG.ALLOW_MOVE_MINION) > 0 && card.GetEntity().IsMinion())
      {
        if (!card.IsInputEnabled() || card.GetEntity().HasTag(GAME_TAG.CANT_MOVE_MINION) || this.m_universalInputManager.IsTouchMode() && ((double) this.m_mousedOverTimer > (double) this.GetMouseOverDelay(clickedEntity) || InputCollection.GetMouseButtonUp(0)))
          return;
        PlayErrors.ErrorType mainOptionPlayError;
        if (!this.AllowMovingMinionAcrossPlayZone() && !clickedEntity.IsControlledByFriendlySidePlayer() && !this.m_gameState.HasValidHoverTargetForMovedMinion(card.GetEntity(), out mainOptionPlayError))
        {
          PlayErrors.DisplayPlayError(mainOptionPlayError, new int?(), clickedEntity);
        }
        else
        {
          this.GrabCard(card.gameObject);
          this.m_gameState.EnterMoveMinionMode(card.GetEntity());
        }
      }
      else
      {
        if (!this.DoNetworkResponse(clickedEntity))
          return;
        if (card.GetActor() is LettuceAbilityActor actor)
          actor.PlayMouseClickedSound();
        if (!this.m_gameState.IsInTargetMode())
        {
          if (!clickedEntity.IsCardButton())
            return;
          if (!clickedEntity.HasSubCards())
            this.ActivatePlaySpell(card);
          if (!clickedEntity.IsHeroPower() && !clickedEntity.IsGameModeButton())
            return;
          clickedEntity.SetTagAndHandleChange<int>(GAME_TAG.EXHAUSTED, 1);
          this.PredictSpentMana(clickedEntity);
        }
        else
        {
          RemoteActionHandler.Get().NotifyOpponentOfTargetModeBegin(card);
          if ((bool) (UnityEngine.Object) this.m_targetReticleManager)
          {
            bool showDamageIndicatorText = false;
            if (clickedEntity.IsLettuceAbility())
            {
              showDamageIndicatorText = true;
              ZoneMgr.Get().TemporarilyDismissMercenariesAbilityTray();
            }
            this.m_targetReticleManager.CreateFriendlyTargetArrow(clickedEntity, showDamageIndicatorText, isAttackArrow: true);
          }
          if (clickedEntity.IsCardButton())
          {
            this.m_targettingHeroPower = true;
            this.ActivatePlaySpell(card);
          }
          else
          {
            if (!clickedEntity.IsCharacter())
              return;
            card.ActivateCharacterAttackEffects();
            if (!clickedEntity.HasTag(GAME_TAG.IGNORE_TAUNT))
              this.m_gameState.ShowEnemyTauntCharacters();
            if (card.IsAttacking())
              return;
            Spell attackSpellForInput = card.GetActorAttackSpellForInput();
            if (!((UnityEngine.Object) attackSpellForInput != (UnityEngine.Object) null))
              return;
            if (clickedEntity.GetRealTimeIsImmuneWhileAttacking())
              card.GetActor().ActivateSpellBirthState(SpellType.IMMUNE);
            attackSpellForInput.ActivateState(SpellStateType.BIRTH);
          }
        }
      }
    }
  }

  public void CancelSelectedLettuceAbilityForEntity(Entity mercenaryEntity)
  {
    if (this.m_gameState.IsResponsePacketBlocked() || !(this.m_gameState.GetGameEntity() is LettuceMissionEntity gameEntity) || !this.DoNetworkOptions(mercenaryEntity))
      return;
    gameEntity.SetEntityThatJustCancelledAbilitySelection(mercenaryEntity);
  }

  private void ManuallyDismissMercenariesAbilityTray()
  {
    if (this.m_universalInputManager.IsDialogActive())
      return;
    ChoiceCardMgr choiceCardMgr = ChoiceCardMgr.Get();
    if (choiceCardMgr.IsShown() || choiceCardMgr.IsWaitingToShowSubOptions() || this.m_gameState.GetBooleanGameOption(GameEntityOption.DISABLE_MANUAL_DISMISSAL_OF_MERC_ABILITY_TRAY) || GameState.Get().IsInTargetMode())
      return;
    ZoneMgr.Get().DismissMercenariesAbilityTray();
    RemoteActionHandler.Get().NotifyOpponentOfSelection(0);
  }

  private void UpdateTelemetryAttackInputCounts(Entity sourceEntity)
  {
    if (sourceEntity == null || (UnityEngine.Object) this.m_battlecrySourceCard != (UnityEngine.Object) null || !sourceEntity.IsMinion() && !sourceEntity.IsHero())
      return;
    if (this.m_lastInputDrag)
      ++this.m_telemetryNumDragAttacks;
    else
      ++this.m_telemetryNumClickAttacks;
  }

  public void HandleClickOnSubOption(Entity entity, bool isSimulated = false)
  {
    if (!isSimulated && !this.PermitDecisionMakingInput())
      return;
    if ((isSimulated || this.m_gameState.HasResponse(entity)) && entity != null)
    {
      bool flag = false;
      ChoiceCardMgr choiceCardMgr = ChoiceCardMgr.Get();
      Card optionParentCard = choiceCardMgr.GetSubOptionParentCard();
      if (!isSimulated)
      {
        flag = this.m_gameState.SubEntityHasTargets(entity);
        if (flag)
        {
          RemoteActionHandler.Get().NotifyOpponentOfTargetModeBegin(optionParentCard);
          string overrideText = entity.GetTargetingArrowText();
          if (string.IsNullOrEmpty(overrideText))
            overrideText = UberText.RemoveMarkupAndCollapseWhitespaces(entity.GetCardTextInHand(), true, true);
          this.m_targetReticleManager.CreateFriendlyTargetArrow(optionParentCard.GetEntity(), true, !this.m_universalInputManager.IsTouchMode(), overrideText);
        }
      }
      Card card = entity.GetCard();
      if (!isSimulated)
        this.DoNetworkResponse(entity);
      this.ActivatePowerUpSpell(card);
      if (!isSimulated && !optionParentCard.IsLettuceAbility())
        this.ActivatePlaySpell(card);
      if (entity.IsMinion() || entity.IsHero())
        card.HideCard();
      choiceCardMgr.OnSubOptionClicked(entity);
      if (isSimulated)
        choiceCardMgr.ClearSubOptions();
      else if (!flag)
        this.FinishSubOptions();
      if (((!this.m_universalInputManager.IsTouchMode() ? 0 : (!isSimulated ? 1 : 0)) & (flag ? 1 : 0)) == 0)
        return;
      this.StartMobileTargetingEffect(this.m_gameState.GetSelectedNetworkSubOption().Targets);
    }
    else
      PlayErrors.DisplayPlayError(this.m_gameState.GetErrorType(entity), this.m_gameState.GetErrorParam(entity), entity);
  }

  private void HandleClickOnChoice(Entity entity)
  {
    if (!this.PermitDecisionMakingInput())
      return;
    if (this.DoNetworkResponse(entity))
      SoundManager.Get().LoadAndPlay((AssetReference) "HeroDropItem1.prefab:587232e6704b20942af1205d00cfc0f9");
    else
      PlayErrors.DisplayPlayError(this.m_gameState.GetErrorType(entity), this.m_gameState.GetErrorParam(entity), entity);
  }

  public void ResetBattlecrySourceCard()
  {
    if ((UnityEngine.Object) this.m_battlecrySourceCard == (UnityEngine.Object) null)
      return;
    if (this.m_universalInputManager.IsTouchMode())
    {
      string message = !this.m_battlecrySourceCard.GetEntity().HasTag(GAME_TAG.BATTLECRY) ? GameStrings.Get("GAMEPLAY_MOBILE_TARGETING_CANCELED") : GameStrings.Get("GAMEPLAY_MOBILE_BATTLECRY_CANCELED");
      GameplayErrorManager.Get().DisplayMessage(message);
    }
    this.m_cancelingBattlecryCards.Add(this.m_battlecrySourceCard);
    Entity entity = this.m_battlecrySourceCard.GetEntity();
    Spell actorSpell = this.m_battlecrySourceCard.GetActorSpell(SpellType.BATTLECRY);
    if ((bool) (UnityEngine.Object) actorSpell)
      actorSpell.ActivateState(SpellStateType.CANCEL);
    Spell playSpell = this.m_battlecrySourceCard.GetPlaySpell(0);
    if ((bool) (UnityEngine.Object) playSpell)
      playSpell.ActivateState(SpellStateType.CANCEL);
    Spell customSummonSpell = this.m_battlecrySourceCard.GetCustomSummonSpell();
    if ((bool) (UnityEngine.Object) customSummonSpell)
      customSummonSpell.ActivateState(SpellStateType.CANCEL);
    ZoneMgr.ChangeCompleteCallback callback = (ZoneMgr.ChangeCompleteCallback) ((changeList, userData) => this.m_cancelingBattlecryCards.Remove((Card) userData));
    ZoneMgr.Get().CancelLocalZoneChange(this.m_lastZoneChangeList, callback, (object) this.m_battlecrySourceCard);
    this.m_lastZoneChangeList = (ZoneChangeList) null;
    this.RollbackSpentMana(entity);
    this.ClearBattlecrySourceCard();
  }

  private bool IsCancelingBattlecryCard(Card card) => this.m_cancelingBattlecryCards.Contains(card);

  public void DoEndTurnButton()
  {
    if (!this.PermitDecisionMakingInput() || this.m_gameState.IsResponsePacketBlocked() || EndTurnButton.Get().IsInputBlocked() || EndTurnButton.Get().IsDisabled)
      return;
    this.DoEndTurnInternal();
  }

  private void DoEndTurnInternal()
  {
    switch (this.m_gameState.GetResponseMode())
    {
      case GameState.ResponseMode.OPTION:
        List<Network.Options.Option> list = this.m_gameState.GetOptionsPacket().List;
        for (int index = 0; index < list.Count; ++index)
        {
          switch (list[index].Type)
          {
            case Network.Options.Option.OptionType.PASS:
            case Network.Options.Option.OptionType.END_TURN:
              if (!this.m_gameState.GetGameEntity().NotifyOfEndTurnButtonPushed())
                return;
              this.m_gameState.SetSelectedOption(index);
              this.m_gameState.SendOption();
              this.HidePhoneHand();
              this.DoEndTurnButton_Option_OnEndTurnRequested();
              return;
            default:
              continue;
          }
        }
        break;
      case GameState.ResponseMode.CHOICE:
        this.m_gameState.SendChoices();
        break;
    }
  }

  public void DoEndTurn_Cheat() => this.DoEndTurnInternal();

  private void DoEndTurnButton_Option_OnEndTurnRequested()
  {
    TurnTimer.Get()?.OnEndTurnRequested();
    EndTurnButton.Get().OnEndTurnRequested();
  }

  public bool DoNetworkResponse(Entity entity, bool checkValidInput = true, bool wantTradeOption = false)
  {
    ThinkEmoteManager.Get()?.NotifyOfActivity();
    if (checkValidInput && !this.m_gameState.IsEntityInputEnabled(entity))
      return false;
    GameState.ResponseMode responseMode = this.m_gameState.GetResponseMode();
    bool flag = false;
    switch (responseMode)
    {
      case GameState.ResponseMode.OPTION:
        flag = this.DoNetworkOptions(entity, wantTradeOption);
        break;
      case GameState.ResponseMode.SUB_OPTION:
        flag = this.DoNetworkSubOptions(entity);
        break;
      case GameState.ResponseMode.OPTION_TARGET:
        flag = this.DoNetworkOptionTarget(entity);
        break;
      case GameState.ResponseMode.CHOICE:
        flag = this.DoNetworkChoice(entity);
        break;
    }
    if (flag)
      entity.GetCard().UpdateActorState();
    return flag;
  }

  private void OnOptionsReceived(object userData)
  {
    if ((bool) (UnityEngine.Object) this.m_mousedOverCard)
      this.m_mousedOverCard.UpdateProposedManaUsage();
    this.HidePhoneHandIfOutOfServerPlays();
  }

  private void OnCurrentPlayerChanged(Player player)
  {
    if (!player.IsLocalUser())
      return;
    this.m_entitiesThatPredictedMana.Clear();
  }

  private void OnOptionRejected(Network.Options.Option option, object userData)
  {
    if (option.Type == Network.Options.Option.OptionType.POWER)
    {
      Entity entity = this.m_gameState.GetEntity(option.Main.ID);
      if (entity == null)
      {
        Debug.LogError((object) "OnOptionRejected - Null Entity");
        return;
      }
      entity.GetCard().NotifyTargetingCanceled();
      if (entity.IsHeroPower() || entity.IsGameModeButton())
        entity.SetTagAndHandleChange<int>(GAME_TAG.EXHAUSTED, 0);
      this.RollbackSpentMana(entity);
      if (entity.IsTwinspell())
      {
        ZoneHand friendlyHand = this.GetFriendlyHand();
        friendlyHand.ActivateTwinspellSpellDeath();
        friendlyHand.ClearReservedCard();
      }
    }
    string message = GameStrings.Get("GAMEPLAY_ERROR_PLAY_REJECTED");
    GameplayErrorManager.Get().DisplayMessage(message);
  }

  private void OnTurnTimerUpdate(TurnTimerUpdate update, object userData)
  {
    if ((double) update.GetSecondsRemaining() > (double) Mathf.Epsilon && !GameUtils.IsWaitingForOpponentReconnect())
      return;
    this.CancelOption(true);
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData)
  {
    this.HidePhoneHand();
    this.CancelOption();
    this.SendGameOverTelemetry();
  }

  private void SendGameOverTelemetry()
  {
    int totalNumAttacks = this.m_telemetryNumClickAttacks + this.m_telemetryNumDragAttacks;
    int percentClickAttacks = totalNumAttacks == 0 ? 0 : (int) ((double) this.m_telemetryNumClickAttacks * 100.0 / (double) totalNumAttacks);
    int percentDragAttacks = totalNumAttacks == 0 ? 0 : (int) ((double) this.m_telemetryNumDragAttacks * 100.0 / (double) totalNumAttacks);
    TelemetryManager.Client().SendAttackInputMethod((long) totalNumAttacks, (long) this.m_telemetryNumClickAttacks, percentClickAttacks, (long) this.m_telemetryNumDragAttacks, percentDragAttacks);
    this.m_telemetryNumDragAttacks = 0;
    this.m_telemetryNumClickAttacks = 0;
  }

  private bool DoNetworkChoice(Entity entity)
  {
    if (!this.m_gameState.IsChoosableEntity(entity))
    {
      PlayErrors.DisplayPlayError(PlayErrors.ErrorType.INVALID, new int?(), entity);
      return false;
    }
    this.m_targetReticleManager?.DestroyFriendlyTargetArrow(false);
    if (this.m_gameState.RemoveChosenEntity(entity))
      return true;
    this.m_gameState.AddChosenEntity(entity);
    if (this.m_gameState.GetFriendlyEntityChoices().IsSingleChoice() && (!this.m_gameState.GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE) || (UnityEngine.Object) MulliganManager.Get() == (UnityEngine.Object) null || !MulliganManager.Get().IsMulliganActive()))
      this.m_gameState.SendChoices();
    return true;
  }

  private bool DoNetworkOptions(Entity entity, bool wantTradeOption = false)
  {
    int entityId = entity.GetEntityId();
    List<Network.Options.Option> list = this.m_gameState.GetOptionsPacket().List;
    for (int index = 0; index < list.Count; ++index)
    {
      Network.Options.Option option = list[index];
      if (option.Type == Network.Options.Option.OptionType.POWER)
      {
        Network.Options.Option.SubOption main = option.Main;
        if (main.PlayErrorInfo.IsValid() && main.ID == entityId)
        {
          bool flag = main.IsTradeOption();
          if (flag == wantTradeOption)
          {
            this.m_gameState.SetSelectedOption(index);
            if (!option.HasValidSubOption())
            {
              List<Network.Options.Option.TargetOption> targets = main.Targets;
              if (targets == null || targets.Count == 0)
                this.m_gameState.SendOption();
              else if (flag)
              {
                this.m_gameState.SetSelectedOptionTarget(entityId);
                this.m_gameState.SendOption();
              }
              else if (this.AllowMovingMinionAcrossPlayZone() && this.m_gameState.GetSelectedOptionTarget() > 0)
                this.m_gameState.SendOption();
              else
                this.EnterOptionTargetMode();
            }
            else
            {
              if (entity.IsLettuceAbility())
                this.HandleMouseOffCard();
              this.m_gameState.EnterSubOptionMode();
              Card card = entity.GetCard();
              ChoiceCardMgr.Get().ShowSubOptions(card);
            }
            return true;
          }
        }
      }
    }
    if (!this.m_universalInputManager.IsTouchMode() || !entity.GetCard().IsShowingTooltip())
      PlayErrors.DisplayPlayError(this.m_gameState.GetErrorType(entity), this.m_gameState.GetErrorParam(entity), entity);
    return false;
  }

  private bool DoNetworkSubOptions(Entity entity)
  {
    int entityId = entity.GetEntityId();
    GameState gameState = this.m_gameState;
    List<Network.Options.Option.SubOption> subs = gameState.GetSelectedNetworkOption().Subs;
    for (int index = 0; index < subs.Count; ++index)
    {
      Network.Options.Option.SubOption subOption = subs[index];
      if (subOption.PlayErrorInfo.IsValid() && subOption.ID == entityId)
      {
        gameState.SetSelectedSubOption(index);
        List<Network.Options.Option.TargetOption> targets = subOption.Targets;
        if (targets == null || targets.Count == 0)
          gameState.SendOption();
        else
          this.EnterOptionTargetMode();
        return true;
      }
    }
    return false;
  }

  private bool DoNetworkOptionTarget(Entity entity)
  {
    int entityId1 = entity.GetEntityId();
    Network.Options.Option.SubOption networkSubOption = this.m_gameState.GetSelectedNetworkSubOption();
    Entity entity1 = this.m_gameState.GetEntity(networkSubOption.ID);
    if (!networkSubOption.IsValidTarget(entityId1))
    {
      int entityId2 = this.m_gameState.GetEntity(entityId1).GetEntityId();
      PlayErrors.DisplayPlayError(networkSubOption.GetErrorForTarget(entityId2), networkSubOption.GetErrorParamForTarget(entityId2), entity1);
      return false;
    }
    this.m_targetReticleManager?.DestroyFriendlyTargetArrow(false);
    RemoteActionHandler.Get()?.NotifyOpponentOfCardDropped();
    this.FinishBattlecrySourceCard();
    this.FinishSubOptions();
    if (entity1.IsHeroPower() || entity1.IsGameModeButton())
    {
      entity1.SetTagAndHandleChange<int>(GAME_TAG.EXHAUSTED, 1);
      this.PredictSpentMana(entity1);
    }
    this.m_gameState.SetSelectedOptionTarget(entityId1);
    this.m_gameState.SendOption();
    return true;
  }

  private void EnterOptionTargetMode()
  {
    this.m_gameState.EnterOptionTargetMode();
    if (!this.m_useHandEnlarge)
      return;
    this.m_myHandZone.SetFriendlyHeroTargetingMode(this.m_gameState.FriendlyHeroIsTargetable());
    bool flag = this.m_myHandZone.HandEnlarged();
    this.m_enlargeHandAfterDropCard = flag || ChoiceCardMgr.Get().RestoreEnlargedHandAfterChoice();
    if (flag)
      this.HidePhoneHand();
    else
      this.m_myHandZone.UpdateLayout((Card) null, true);
  }

  private void FinishBattlecrySourceCard()
  {
    if ((UnityEngine.Object) this.m_battlecrySourceCard == (UnityEngine.Object) null)
      return;
    this.ClearBattlecrySourceCard();
  }

  private void ClearBattlecrySourceCard()
  {
    if (this.m_isInBattleCryEffect && (UnityEngine.Object) this.m_battlecrySourceCard != (UnityEngine.Object) null)
      this.EndBattleCryEffect();
    this.m_battlecrySourceCard = (Card) null;
    RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
    if (!this.m_useHandEnlarge)
      return;
    this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
    this.m_myHandZone.UpdateLayout((Card) null, true);
  }

  private void CancelSubOptions()
  {
    ChoiceCardMgr choiceCardMgr = ChoiceCardMgr.Get();
    Card optionParentCard = choiceCardMgr.GetSubOptionParentCard();
    if ((UnityEngine.Object) optionParentCard == (UnityEngine.Object) null)
      return;
    choiceCardMgr.CancelSubOptions();
    Entity entity = optionParentCard.GetEntity();
    if (entity.IsTwinspell())
    {
      this.m_myHandZone.OnTwinspellDropped(optionParentCard);
      this.m_myHandZone.ClearReservedCard();
    }
    if (!entity.IsCardButton())
    {
      ZoneMgr.Get().CancelLocalZoneChange(this.m_lastZoneChangeList);
      this.m_lastZoneChangeList = (ZoneChangeList) null;
    }
    this.RollbackSpentMana(entity);
    this.DropSubOptionParentCard();
  }

  private void FinishSubOptions()
  {
    if ((UnityEngine.Object) ChoiceCardMgr.Get().GetSubOptionParentCard() == (UnityEngine.Object) null)
      return;
    this.DropSubOptionParentCard();
  }

  public void DropSubOptionParentCard()
  {
    Log.Hand.Print("DropSubOptionParentCard()");
    ChoiceCardMgr.Get().ClearSubOptions();
    RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
    if (this.m_useHandEnlarge)
    {
      this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
      this.m_myHandZone.UpdateLayout((Card) null, true);
    }
    if (!this.m_universalInputManager.IsTouchMode())
      return;
    this.EndMobileTargetingEffect();
  }

  public void StartPendingChoiceTarget()
  {
    Network.EntityChoices friendlyEntityChoices = this.m_gameState.GetFriendlyEntityChoices();
    if (friendlyEntityChoices == null)
      return;
    Entity entity = this.m_gameState.GetEntity(friendlyEntityChoices.Source);
    if (!(bool) (UnityEngine.Object) this.m_targetReticleManager)
      return;
    if (this.m_universalInputManager.IsTouchMode())
    {
      this.m_targetReticleManager.CreateFriendlyTargetArrow(entity, true, false);
      this.StartMobileTargetingEffect(friendlyEntityChoices.Entities);
    }
    else
      this.m_targetReticleManager.CreateFriendlyTargetArrow(entity, true);
  }

  public void FinishPendingChoiceTarget()
  {
    this.CancelOption();
    if (!this.m_universalInputManager.IsTouchMode())
      return;
    this.EndMobileTargetingEffect();
  }

  private void StartMobileTargetingEffect(List<Network.Options.Option.TargetOption> targets)
  {
    if (targets == null || targets.Count == 0)
      return;
    List<int> entityIDs = new List<int>();
    foreach (Network.Options.Option.TargetOption target in targets)
    {
      if (target.PlayErrorInfo.IsValid())
        entityIDs.Add(target.ID);
    }
    this.StartMobileTargetingEffect(entityIDs);
  }

  private void StartMobileTargetingEffect(List<int> entityIDs)
  {
    if (entityIDs == null || entityIDs.Count == 0)
      return;
    this.m_mobileTargettingEffectActors.Clear();
    foreach (int entityId in entityIDs)
    {
      Card card = this.m_gameState.GetEntity(entityId).GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      {
        Actor actor = card.GetActor();
        this.m_mobileTargettingEffectActors.Add(actor);
        this.ApplyMobileTargettingEffectToActor(actor);
      }
    }
    this.m_screenEffectHandle.StartEffect(ScreenEffectParameters.DesaturatePerspective);
  }

  private bool IsMobileTargetingEffectActive() => this.m_mobileTargettingEffectActors.Count > 0;

  private void EndMobileTargetingEffect()
  {
    foreach (Actor targettingEffectActor in this.m_mobileTargettingEffectActors)
      this.RemoveMobileTargettingEffectFromActor(targettingEffectActor);
    this.m_mobileTargettingEffectActors.Clear();
    this.m_screenEffectHandle.StopEffect();
  }

  private void StartBattleCryEffect(Entity entity)
  {
    this.m_isInBattleCryEffect = true;
    Network.Options.Option selectedNetworkOption = this.m_gameState.GetSelectedNetworkOption();
    if (selectedNetworkOption == null)
    {
      Debug.LogError((object) "No targets for BattleCry.");
    }
    else
    {
      this.StartMobileTargetingEffect(selectedNetworkOption.Main.Targets);
      this.m_battlecrySourceCard.SetBattleCrySource(true);
    }
  }

  private void EndBattleCryEffect()
  {
    this.m_isInBattleCryEffect = false;
    this.EndMobileTargetingEffect();
    this.m_battlecrySourceCard.SetBattleCrySource(false);
  }

  private void ApplyMobileTargettingEffectToActor(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    GameObject gameObject = actor.gameObject;
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(gameObject, GameLayer.IgnoreFullScreenEffects);
    Hashtable args1 = iTween.Hash((object) "y", (object) 0.8f, (object) "time", (object) 0.4f, (object) "easeType", (object) iTween.EaseType.easeOutQuad, (object) "name", (object) "position", (object) "isLocal", (object) true);
    Hashtable args2 = iTween.Hash((object) "x", (object) 1.08f, (object) "z", (object) 1.08f, (object) "time", (object) 0.4f, (object) "easeType", (object) iTween.EaseType.easeOutQuad, (object) "name", (object) "scale");
    iTween.StopByName(gameObject, "position");
    iTween.StopByName(gameObject, "scale");
    iTween.MoveTo(gameObject, args1);
    iTween.ScaleTo(gameObject, args2);
  }

  private void RemoveMobileTargettingEffectFromActor(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    GameObject gameObject = actor.gameObject;
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(gameObject, GameLayer.Default);
    MeshRenderer meshRenderer = actor.GetMeshRenderer();
    if ((UnityEngine.Object) meshRenderer != (UnityEngine.Object) null)
      LayerUtils.SetLayer(meshRenderer.gameObject, GameLayer.CardRaycast);
    Hashtable args1 = iTween.Hash((object) "x", (object) 0.0f, (object) "y", (object) 0.0f, (object) "z", (object) 0.0f, (object) "time", (object) 0.5f, (object) "easeType", (object) iTween.EaseType.easeOutQuad, (object) "name", (object) "position", (object) "isLocal", (object) true);
    Hashtable args2 = iTween.Hash((object) "x", (object) 1f, (object) "z", (object) 1f, (object) "time", (object) 0.4f, (object) "easeType", (object) iTween.EaseType.easeOutQuad, (object) "name", (object) "scale");
    iTween.StopByName(gameObject, "position");
    iTween.StopByName(gameObject, "scale");
    iTween.MoveTo(gameObject, args1);
    iTween.ScaleTo(gameObject, args2);
  }

  private bool HandleMulliganHotkeys()
  {
    MulliganManager mulliganManager = MulliganManager.Get();
    if ((UnityEngine.Object) mulliganManager == (UnityEngine.Object) null || GameMgr.Get().IsBattlegrounds() || !HearthstoneApplication.IsInternal() || !InputCollection.GetKeyUp(KeyCode.Escape) || GameMgr.Get().IsTraditionalTutorial() || PlatformSettings.IsMobile())
      return false;
    mulliganManager.SetAllMulliganCardsToHold();
    this.DoEndTurnButton();
    TurnStartManager.Get().BeginListeningForTurnEvents();
    mulliganManager.SkipMulliganForDev();
    return true;
  }

  private bool HandleUniversalHotkeys() => false;

  private bool HandleGameHotkeys() => (this.m_gameState == null || !this.m_gameState.IsMulliganManagerActive()) && !this.HasPendingChoiceTarget() && InputCollection.GetKeyUp(KeyCode.Escape) && this.CancelOption();

  private void ShowBullseyeIfNeeded()
  {
    if ((UnityEngine.Object) this.m_targetReticleManager == (UnityEngine.Object) null || !this.m_targetReticleManager.IsActive())
      return;
    bool show = false;
    if ((UnityEngine.Object) this.m_mousedOverCard != (UnityEngine.Object) null)
      show = !this.HasPendingChoiceTarget() ? this.m_gameState.IsValidOptionTarget(this.m_mousedOverCard.GetEntity(), false) : this.m_gameState.GetFriendlyEntityChoices().Entities.Contains(this.m_mousedOverCard.GetEntity().GetEntityId());
    this.m_targetReticleManager.ShowBullseye(show);
  }

  private bool EntityIsPoisonousForSkullPreview(Entity entity)
  {
    if (entity.GetRealTimeAttack() <= 0)
      return false;
    if (entity.GetRealTimeIsPoisonous())
      return true;
    if (entity.IsHero())
    {
      Card weaponCard = entity.GetWeaponCard();
      Entity entity1 = (bool) (UnityEngine.Object) weaponCard ? weaponCard.GetEntity() : (Entity) null;
      if (entity1 != null && entity1.GetRealTimeIsPoisonous())
        return true;
    }
    return false;
  }

  private void ShowSkullIfNeeded()
  {
    if ((UnityEngine.Object) this.GetBattlecrySourceCard() != (UnityEngine.Object) null)
      return;
    Network.Options.Option.SubOption networkSubOption = this.m_gameState.GetSelectedNetworkSubOption();
    if (networkSubOption == null)
      return;
    Entity entity1 = this.m_gameState.GetEntity(networkSubOption.ID);
    if (entity1 == null || !entity1.IsMinion() && !entity1.IsHero())
      return;
    Entity entity2 = this.m_mousedOverCard.GetEntity();
    if (!entity2.IsMinion() && !entity2.IsHero() || !this.m_gameState.IsValidOptionTarget(entity2, false) || entity2.IsObfuscated())
      return;
    this.ShowSkull(entity1, entity2, this.m_mousedOverCard);
    this.ShowSkull(entity2, entity1, entity1.GetCard());
  }

  private void ShowSkull(Entity entity1, Entity entity2, Card card)
  {
    int a = entity1.GetRealTimeAttack();
    if (entity2.HasTag(GAME_TAG.HEAVILY_ARMORED))
      a = Mathf.Min(a, 1);
    if (!entity2.CanBeDamagedRealTime() || a < entity2.GetRealTimeRemainingHP() && (!this.EntityIsPoisonousForSkullPreview(entity1) || !entity2.IsMinion()))
      return;
    if (this.EntityIsPoisonousForSkullPreview(entity1))
    {
      DamageSplatSpell damageSplatSpell = card.ActivateActorSpell(SpellType.DAMAGE) as DamageSplatSpell;
      if (!((UnityEngine.Object) damageSplatSpell != (UnityEngine.Object) null))
        return;
      damageSplatSpell.SetPoisonous(true);
      damageSplatSpell.ActivateState(SpellStateType.IDLE);
      damageSplatSpell.transform.localScale = Vector3.zero;
      iTween.ScaleTo(damageSplatSpell.gameObject, iTween.Hash((object) "scale", (object) Vector3.one, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    }
    else
    {
      Spell spell = card.ActivateActorSpell(SpellType.SKULL);
      if (!((UnityEngine.Object) spell != (UnityEngine.Object) null))
        return;
      spell.transform.localScale = Vector3.zero;
      iTween.ScaleTo(spell.gameObject, iTween.Hash((object) "scale", (object) Vector3.one, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    }
  }

  private void DisableSkullIfNeeded(Card mousedOverCard)
  {
    this.DisableSkull(mousedOverCard);
    if (this.m_gameState == null)
      return;
    Network.Options.Option.SubOption networkSubOption = this.m_gameState.GetSelectedNetworkSubOption();
    if (networkSubOption == null)
      return;
    Entity entity = this.m_gameState.GetEntity(networkSubOption.ID);
    if (entity == null)
      return;
    Card card = entity.GetCard();
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return;
    this.DisableSkull(card);
  }

  private void DisableSkull(Card card)
  {
    Spell actorSpell1 = card.GetActorSpell(SpellType.SKULL);
    if ((UnityEngine.Object) actorSpell1 != (UnityEngine.Object) null)
    {
      iTween.Stop(actorSpell1.gameObject);
      actorSpell1.transform.localScale = Vector3.zero;
      actorSpell1.Deactivate();
    }
    Spell actorSpell2 = card.GetActorSpell(SpellType.DAMAGE);
    if (!((UnityEngine.Object) actorSpell2 != (UnityEngine.Object) null) || !card.GetEntity().IsMinion())
      return;
    iTween.Stop(actorSpell2.gameObject);
    actorSpell2.transform.localScale = Vector3.zero;
    actorSpell2.Deactivate();
  }

  private void HandleMouseOverCard(Card card)
  {
    if (!card.IsInputEnabled() || this.m_gameState.GetGameEntity().ShouldSuppressCardMouseOver(card.GetEntity()))
      return;
    this.m_mousedOverCard = card;
    bool flag = this.m_gameState.IsFriendlySidePlayerTurn() && (bool) (UnityEngine.Object) this.m_targetReticleManager && this.m_targetReticleManager.ShouldPreventMouseOverBigCard();
    if (!this.PermitDecisionMakingInput())
      flag = false;
    if (this.m_gameState.IsMainPhase() && (UnityEngine.Object) this.m_heldCard == (UnityEngine.Object) null && !ChoiceCardMgr.Get().HasSubOption() && !flag && (!this.m_universalInputManager.IsTouchMode() || (UnityEngine.Object) card.gameObject == (UnityEngine.Object) this.m_lastObjectMousedDown))
      this.SetShouldShowTooltip();
    card.NotifyMousedOver();
    if (this.m_gameState.IsMulliganManagerActive() && card.GetEntity().IsControlledByFriendlySidePlayer() && card.GetZone() is ZoneHand && !(bool) UniversalInputManager.UsePhoneUI)
      TooltipPanelManager.Get().UpdateKeywordHelpForMulliganCard(card.GetEntity(), card.GetActor());
    this.ShowBullseyeIfNeeded();
    this.ShowSkullIfNeeded();
  }

  public void NotifyCardDestroyed(Card destroyedCard)
  {
    if (!((UnityEngine.Object) destroyedCard == (UnityEngine.Object) this.m_mousedOverCard))
      return;
    this.HandleMouseOffCard();
  }

  private void HandleMouseOffCard()
  {
    if ((UnityEngine.Object) this.m_mousedOverCard == (UnityEngine.Object) null)
      return;
    PegCursor.Get().SetMode(PegCursor.Mode.UP);
    Card mousedOverCard = this.m_mousedOverCard;
    this.m_mousedOverCard = (Card) null;
    mousedOverCard.HideTooltip();
    mousedOverCard.NotifyMousedOut();
    this.ShowBullseyeIfNeeded();
    this.DisableSkullIfNeeded(mousedOverCard);
  }

  public void HandleMemberClick(GameObject hitObject)
  {
    if (!((UnityEngine.Object) this.m_mousedOverObject == (UnityEngine.Object) null))
      return;
    RaycastHit hitInfo;
    if (this.m_universalInputManager.GetInputHitInfo(Camera.main, GameLayer.PlayAreaCollision, out hitInfo))
    {
      if (!((UnityEngine.Object) hitObject == (UnityEngine.Object) null))
        return;
      int allInputHitInfo = UniversalInputManager.Get().GetAllInputHitInfo((LayerMask) -1, ref this.m_cachedDustBlockers);
      if (allInputHitInfo > 0)
      {
        Array.Sort<RaycastHit>(this.m_cachedDustBlockers, 0, allInputHitInfo, (IComparer<RaycastHit>) this.m_hitPointComparer);
        for (int index = 0; index < allInputHitInfo; ++index)
        {
          GameObject gameObject = this.m_cachedDustBlockers[index].collider.gameObject;
          if (gameObject.layer != 10)
          {
            if (gameObject.layer == 8 || (UnityEngine.Object) gameObject.GetComponent<BoardClickableDustBlocker>() != (UnityEngine.Object) null)
              return;
          }
          else
            break;
        }
      }
      Board.Get().BoardClicked(hitInfo);
    }
    else
    {
      if (!((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null))
        return;
      SoundManager.Get().LoadAndPlay((AssetReference) "UI_MouseClick_01.prefab:fa537702a0db1c3478c989967458788b");
    }
  }

  private void ShowTooltipIfNecessary()
  {
    if ((UnityEngine.Object) this.m_mousedOverCard == (UnityEngine.Object) null || !this.m_mousedOverCard.GetShouldShowTooltip())
      return;
    bool resetTimer;
    if (this.m_gameState.GetGameEntity().SuppressMousedOverCardTooltip(out resetTimer))
    {
      if (!resetTimer)
        return;
      this.m_mousedOverTimer = 0.0f;
    }
    else
    {
      this.m_mousedOverTimer += Time.unscaledDeltaTime;
      if (!this.m_mousedOverCard.IsActorReady())
        return;
      if (this.m_gameState.GetBooleanGameOption(GameEntityOption.MOUSEOVER_DELAY_OVERRIDDEN))
        this.m_mousedOverCard.ShowTooltip();
      else if (this.m_mousedOverCard.GetZone() is ZoneHand)
      {
        this.m_mousedOverCard.ShowTooltip();
      }
      else
      {
        if ((double) this.m_mousedOverTimer < (double) this.GetMouseOverDelay(this.m_mousedOverCard.GetEntity()))
          return;
        this.m_mousedOverCard.ShowTooltip();
      }
    }
  }

  private void ShowTooltipZone(GameObject hitObject, TooltipZone tooltip)
  {
    if (this.m_gameState.IsMulliganManagerActive())
      return;
    GameEntity gameEntity = this.m_gameState.GetGameEntity();
    if (gameEntity == null || gameEntity.GetGameOptions().GetBooleanOption(GameEntityOption.DISABLE_TOOLTIPS) || gameEntity.NotifyOfTooltipDisplay(tooltip))
      return;
    InputManager.ZoneTooltipSettings zoneTooltipSettings = gameEntity.GetZoneTooltipSettings();
    if (this.ShowTooltipManaCrystalManager(tooltip, zoneTooltipSettings) || this.ShowTooltipDeckZone(tooltip, zoneTooltipSettings) || this.ShowTooltipOpposingHandZone(tooltip, zoneTooltipSettings) || this.ShowTooltipCorpseCounter(tooltip, zoneTooltipSettings))
      return;
    this.ShowTooltipManaCounterZone(tooltip, zoneTooltipSettings);
  }

  private bool ShowTooltipManaCrystalManager(
    TooltipZone tooltip,
    InputManager.ZoneTooltipSettings zoneTooltipSettings)
  {
    ManaCrystalMgr component = tooltip.targetObject.GetComponent<ManaCrystalMgr>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null || !zoneTooltipSettings.FriendlyMana.Allowed)
      return false;
    string headline1 = (string) null;
    string description1 = (string) null;
    if (zoneTooltipSettings.FriendlyMana.GetTooltipOverrideContent(ref headline1, ref description1))
      this.ShowTooltipInZone(tooltip, headline1, description1);
    if (component.ShouldShowTooltip(ManaCrystalType.DEFAULT))
    {
      Player friendlySidePlayer = this.m_gameState.GetFriendlySidePlayer();
      int tag1 = friendlySidePlayer.GetTag(GAME_TAG.OVERLOAD_OWED);
      if (tag1 > 0)
      {
        string headline2 = GameStrings.Format("GAMEPLAY_TOOLTIP_MANA_OVERLOAD_HEADLINE");
        string description2 = GameStrings.Format("GAMEPLAY_TOOLTIP_MANA_OVERLOAD_DESCRIPTION", (object) tag1);
        this.ShowTooltipInZone(tooltip, headline2, description2);
      }
      else
        this.ShowTooltipInZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_DESCRIPTION"));
      int tag2 = friendlySidePlayer.GetTag(GAME_TAG.OVERLOAD_LOCKED);
      if (tag2 > 0)
      {
        string headline3 = GameStrings.Format("GAMEPLAY_TOOLTIP_MANA_LOCKED_HEADLINE");
        string description3 = GameStrings.Format("GAMEPLAY_TOOLTIP_MANA_LOCKED_DESCRIPTION", (object) tag2);
        this.AddTooltipInZone(tooltip, headline3, description3);
      }
    }
    else if (component.ShouldShowTooltip(ManaCrystalType.COIN))
      this.ShowTooltipInZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_COIN_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_COIN_DESCRIPTION"));
    return true;
  }

  private bool ShowTooltipDeckZone(
    TooltipZone tooltip,
    InputManager.ZoneTooltipSettings zoneTooltipSettings)
  {
    ZoneDeck component = tooltip.targetObject.GetComponent<ZoneDeck>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return false;
    if (component.m_Side == Player.Side.FRIENDLY)
      this.ShowTooltipFriendlyDeckZone(tooltip, zoneTooltipSettings, component);
    else if (component.m_Side == Player.Side.OPPOSING)
      this.ShowTooltipOpposingDeckZone(tooltip, zoneTooltipSettings, component);
    return true;
  }

  private void ShowTooltipFriendlyDeckZone(
    TooltipZone tooltip,
    InputManager.ZoneTooltipSettings zoneTooltipSettings,
    ZoneDeck deck)
  {
    if (zoneTooltipSettings.FriendlyDeck.IsCustomHandler)
    {
      tooltip.RegisterOnTooltipHiddenCallback((Action) (() => zoneTooltipSettings.FriendlyDeck.FireOnTooltipHidden()));
      zoneTooltipSettings.FriendlyDeck.FireOnTooltipShown((Action<string, string>) ((headline, description) => this.ShowTooltipInZone(tooltip, headline, description)));
    }
    else
    {
      if (zoneTooltipSettings.FriendlyDeck.Allowed)
      {
        Vector3 localOffset = Vector3.zero;
        string headline = (string) null;
        string description = (string) null;
        if (!zoneTooltipSettings.FriendlyDeck.GetTooltipOverrideContent(ref headline, ref description))
        {
          if (deck.IsFatigued())
          {
            if ((bool) UniversalInputManager.UsePhoneUI)
              localOffset = new Vector3(0.0f, 0.0f, 0.562f);
            headline = GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_DECK_HEADLINE");
            description = GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_DECK_DESCRIPTION");
          }
          else
          {
            headline = GameStrings.Format("GAMEPLAY_TOOLTIP_DECK_HEADLINE");
            description = GameStrings.Format("GAMEPLAY_TOOLTIP_DECK_DESCRIPTION", (object) deck.GetCards().Count);
          }
        }
        this.ShowTooltipInZone(tooltip, headline, description, localOffset);
      }
      if (!((UnityEngine.Object) deck.m_playerHandTooltipZone != (UnityEngine.Object) null) || !zoneTooltipSettings.FriendlyHand.Allowed)
        return;
      Player friendlySidePlayer = this.m_gameState.GetFriendlySidePlayer();
      int count = friendlySidePlayer.GetHandZone().GetCards().Count;
      if (count < 5 || GameMgr.Get().IsTraditionalTutorial())
        return;
      string headline1 = (string) null;
      string description1 = (string) null;
      if (!zoneTooltipSettings.FriendlyHand.GetTooltipOverrideContent(ref headline1, ref description1))
      {
        headline1 = GameStrings.Get("GAMEPLAY_TOOLTIP_HAND_HEADLINE");
        description1 = GameStrings.Format("GAMEPLAY_TOOLTIP_HAND_DESCRIPTION", (object) count);
        if (count >= friendlySidePlayer.GetTag(GAME_TAG.MAXHANDSIZE))
        {
          headline1 = GameStrings.Get("GAMEPLAY_TOOLTIP_HAND_FULL_HEADLINE");
          description1 = GameStrings.Format("GAMEPLAY_TOOLTIP_HAND_FULL_DESCRIPTION", (object) count);
        }
      }
      this.ShowTooltipInZone(deck.m_playerHandTooltipZone, headline1, description1);
    }
  }

  private void ShowTooltipOpposingDeckZone(
    TooltipZone tooltip,
    InputManager.ZoneTooltipSettings zoneTooltipSettings,
    ZoneDeck deck)
  {
    if (zoneTooltipSettings.EnemyDeck.IsCustomHandler)
    {
      tooltip.RegisterOnTooltipHiddenCallback((Action) (() => zoneTooltipSettings.EnemyDeck.FireOnTooltipHidden()));
      zoneTooltipSettings.EnemyDeck.FireOnTooltipShown((Action<string, string>) ((headline, description) => this.ShowTooltipInZone(tooltip, headline, description)));
    }
    else
    {
      if (zoneTooltipSettings.EnemyDeck.Allowed)
      {
        string headline = (string) null;
        string description = (string) null;
        if (!zoneTooltipSettings.EnemyDeck.GetTooltipOverrideContent(ref headline, ref description))
        {
          if (deck.IsFatigued())
          {
            headline = GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_ENEMYDECK_HEADLINE");
            description = GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_ENEMYDECK_DESCRIPTION");
          }
          else
          {
            headline = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYDECK_HEADLINE");
            description = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYDECK_DESC", (object) deck.GetCards().Count);
          }
        }
        this.ShowTooltipInZone(tooltip, headline, description);
        if (zoneTooltipSettings.EnemyDeck.GetTooltipOverrideContent(ref headline, ref description, 1))
          this.AddTooltipInZone(tooltip, headline, description);
      }
      if ((UnityEngine.Object) deck.m_playerHandTooltipZone != (UnityEngine.Object) null && zoneTooltipSettings.EnemyHand.Allowed)
      {
        int count = this.m_gameState.GetOpposingSidePlayer().GetHandZone().GetCards().Count;
        if (count >= 5 && !GameMgr.Get().IsTraditionalTutorial())
        {
          string headline = (string) null;
          string description = (string) null;
          if (!zoneTooltipSettings.EnemyHand.GetTooltipOverrideContent(ref headline, ref description))
          {
            headline = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_HEADLINE");
            description = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYHAND_DESC", (object) count);
          }
          this.ShowTooltipInZone(deck.m_playerHandTooltipZone, headline, description);
        }
      }
      int tag = this.m_gameState.GetOpposingSidePlayer().GetTag(GAME_TAG.OVERLOAD_OWED);
      if (!zoneTooltipSettings.EnemyMana.Allowed || tag <= 0)
        return;
      if ((bool) UniversalInputManager.UsePhoneUI && (UnityEngine.Object) deck.m_playerHandTooltipZone != (UnityEngine.Object) null)
      {
        string headline = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_HEADLINE");
        string description = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_DESC", (object) tag);
        this.AddTooltipInZone(deck.m_playerHandTooltipZone, headline, description);
      }
      else
      {
        if ((bool) UniversalInputManager.UsePhoneUI || !((UnityEngine.Object) deck.m_playerManaTooltipZone != (UnityEngine.Object) null))
          return;
        string headline = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_HEADLINE");
        string description = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_DESC", (object) tag);
        this.ShowTooltipInZone(deck.m_playerManaTooltipZone, headline, description);
      }
    }
  }

  private bool ShowTooltipOpposingHandZone(
    TooltipZone tooltip,
    InputManager.ZoneTooltipSettings zoneTooltipSettings)
  {
    ZoneHand component = tooltip.targetObject.GetComponent<ZoneHand>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null || component.m_Side != Player.Side.OPPOSING)
      return false;
    if (GameMgr.Get().IsTraditionalTutorial())
      this.ShowTooltipInZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_DESC_TUT"));
    else if (zoneTooltipSettings.EnemyHand.Allowed)
    {
      string headline1 = (string) null;
      string description1 = (string) null;
      if (!zoneTooltipSettings.EnemyHand.GetTooltipOverrideContent(ref headline1, ref description1))
      {
        int cardCount = component.GetCardCount();
        if (cardCount == 1)
        {
          headline1 = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_HEADLINE");
          description1 = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYHAND_DESC_SINGLE", (object) cardCount);
        }
        else
        {
          headline1 = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_HEADLINE");
          description1 = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYHAND_DESC", (object) cardCount);
        }
      }
      this.ShowTooltipInZone(tooltip, headline1, description1);
      if ((bool) UniversalInputManager.UsePhoneUI && zoneTooltipSettings.EnemyMana.Allowed)
      {
        int tag = this.m_gameState.GetOpposingSidePlayer().GetTag(GAME_TAG.OVERLOAD_OWED);
        if (tag > 0)
        {
          string headline2 = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_HEADLINE");
          string description2 = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_DESC", (object) tag);
          this.AddTooltipInZone(tooltip, headline2, description2);
        }
      }
    }
    return true;
  }

  private bool ShowTooltipCorpseCounter(
    TooltipZone tooltip,
    InputManager.ZoneTooltipSettings zoneTooltipSettings)
  {
    CorpseCounter corpseCounter = (UnityEngine.Object) tooltip.targetObject != (UnityEngine.Object) null ? tooltip.targetObject.GetComponent<CorpseCounter>() : (CorpseCounter) null;
    if (!((UnityEngine.Object) corpseCounter != (UnityEngine.Object) null) || !corpseCounter.IsShown())
      return false;
    int availableCorpses = (corpseCounter.m_side == Player.Side.FRIENDLY ? this.m_gameState.GetFriendlySidePlayer() : this.m_gameState.GetOpposingSidePlayer()).GetNumAvailableCorpses();
    string headline = GameStrings.Get("GAMEPLAY_TOOLTIP_CORPSES");
    string description = GameStrings.Format("GAMEPLAY_TOOLTIP_CORPSES_DESCRIPTION", (object) availableCorpses);
    this.ShowTooltipInZone(tooltip, headline, description);
    return true;
  }

  private bool ShowTooltipManaCounterZone(
    TooltipZone tooltip,
    InputManager.ZoneTooltipSettings zoneTooltipSettings)
  {
    ManaCounter component = tooltip.targetObject.GetComponent<ManaCounter>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.m_Side == Player.Side.OPPOSING && zoneTooltipSettings.EnemyMana.Allowed)
    {
      int tag = this.m_gameState.GetOpposingSidePlayer().GetTag(GAME_TAG.OVERLOAD_OWED);
      if (tag > 0)
      {
        string headline = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_HEADLINE");
        string description = GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYOVERLOAD_DESC", (object) tag);
        this.ShowTooltipInZone(tooltip, headline, description);
        return true;
      }
    }
    return false;
  }

  private void AddTooltipInZone(TooltipZone tooltip, string headline, string description)
  {
    for (int index = 0; index < 10; ++index)
    {
      if (!tooltip.IsShowingTooltip(index))
      {
        this.ShowTooltipInZone(tooltip, headline, description, Vector3.zero, index);
        return;
      }
    }
    Debug.LogError((object) ("You are trying to add too many tooltips. TooltipZone = [" + tooltip.gameObject.name + "] MAX_TOOLTIPS = [" + (object) 10 + "]"));
  }

  private void ShowTooltipInZone(
    TooltipZone tooltip,
    string headline,
    string description,
    int index = 0)
  {
    this.ShowTooltipInZone(tooltip, headline, description, Vector3.zero, index);
  }

  private void ShowTooltipInZone(
    TooltipZone tooltip,
    string headline,
    string description,
    Vector3 localOffset,
    int index = 0)
  {
    this.m_gameState.GetGameEntity().NotifyOfTooltipZoneMouseOver(tooltip);
    if (this.m_universalInputManager.IsTouchMode())
      tooltip.ShowGameplayTooltipLarge(headline, description, localOffset, index);
    else
      tooltip.ShowGameplayTooltip(headline, description, localOffset, index);
  }

  private void PredictSpentMana(Entity entity)
  {
    Player friendlySidePlayer = this.m_gameState.GetFriendlySidePlayer();
    if (friendlySidePlayer.GetRealTimeSpellsCostHealth() && entity.GetRealTimeCardType() == TAG_CARDTYPE.SPELL || entity.GetRealTimeCardCostsHealth() || entity.GetRealTimeCardCostsArmor())
      return;
    int realTimeCost = entity.GetRealTimeCost();
    int realTimeTempMana = friendlySidePlayer.GetRealTimeTempMana();
    if (realTimeTempMana > 0)
    {
      int num = Mathf.Clamp(realTimeCost, 0, realTimeTempMana);
      friendlySidePlayer.NotifyOfUsedTempMana(num);
      ManaCrystalMgr.Get().DestroyTempManaCrystals(num);
    }
    int num1 = realTimeCost - realTimeTempMana;
    if (num1 > 0 && !entity.HasTag(GAME_TAG.RED_MANA_CRYSTALS))
    {
      friendlySidePlayer.NotifyOfSpentMana(num1);
      ManaCrystalMgr.Get().UpdateSpentMana(num1);
    }
    friendlySidePlayer.UpdateManaCounter();
    this.m_entitiesThatPredictedMana.Add(entity);
  }

  private void RollbackSpentMana(Entity entity)
  {
    int index = this.m_entitiesThatPredictedMana.IndexOf(entity);
    if (index < 0)
      return;
    this.m_entitiesThatPredictedMana.RemoveAt(index);
    Player friendlySidePlayer = this.m_gameState.GetFriendlySidePlayer();
    int realTimeCost = entity.GetRealTimeCost();
    int realTimeTempMana = friendlySidePlayer.GetRealTimeTempMana();
    if (friendlySidePlayer.GetRealTimeTempMana() > 0)
    {
      int numCrystals = Mathf.Clamp(realTimeCost, 0, realTimeTempMana);
      friendlySidePlayer.NotifyOfUsedTempMana(-numCrystals);
      ManaCrystalMgr.Get().AddTempManaCrystals(numCrystals);
    }
    int num = -realTimeCost + realTimeTempMana;
    if (num < 0)
    {
      friendlySidePlayer.NotifyOfSpentMana(num);
      ManaCrystalMgr.Get().UpdateSpentMana(num);
    }
    friendlySidePlayer.UpdateManaCounter();
  }

  public void OnManaCrystalMgrManaSpent()
  {
    if (!(bool) (UnityEngine.Object) this.m_mousedOverCard)
      return;
    this.m_mousedOverCard.UpdateProposedManaUsage();
  }

  private bool IsInZone(Entity entity, TAG_ZONE zoneTag) => this.IsInZone(entity.GetCard(), zoneTag);

  private bool IsInZone(Card card, TAG_ZONE zoneTag)
  {
    if ((UnityEngine.Object) card.GetZone() == (UnityEngine.Object) null)
      return false;
    Entity entity = card.GetEntity();
    if (entity == null)
      return false;
    TAG_ZONE finalZoneForEntity = GameUtils.GetFinalZoneForEntity(entity);
    if (finalZoneForEntity == zoneTag)
      return true;
    GameEntity gameEntity = GameState.Get()?.GetGameEntity();
    bool isInZone;
    return gameEntity != null && gameEntity.Overwrite_IsInZone_ForInputManager(entity, zoneTag, finalZoneForEntity, out isInZone) && isInZone;
  }

  private void SetDragging(bool dragging)
  {
    this.m_dragging = dragging;
    this.m_graphicsManager?.SetDraggingFramerate(dragging);
  }

  public bool RegisterPhoneHandShownListener(InputManager.PhoneHandShownCallback callback) => this.RegisterPhoneHandShownListener(callback, (object) null);

  public bool RegisterPhoneHandShownListener(
    InputManager.PhoneHandShownCallback callback,
    object userData)
  {
    InputManager.PhoneHandShownListener handShownListener = new InputManager.PhoneHandShownListener();
    handShownListener.SetCallback(callback);
    handShownListener.SetUserData(userData);
    if (this.m_phoneHandShownListener.Contains(handShownListener))
      return false;
    this.m_phoneHandShownListener.Add(handShownListener);
    return true;
  }

  public bool RemovePhoneHandShownListener(InputManager.PhoneHandShownCallback callback) => this.RemovePhoneHandShownListener(callback, (object) null);

  public bool RemovePhoneHandShownListener(
    InputManager.PhoneHandShownCallback callback,
    object userData)
  {
    InputManager.PhoneHandShownListener handShownListener = new InputManager.PhoneHandShownListener();
    handShownListener.SetCallback(callback);
    handShownListener.SetUserData(userData);
    return this.m_phoneHandShownListener.Remove(handShownListener);
  }

  public bool RegisterPhoneHandHiddenListener(InputManager.PhoneHandHiddenCallback callback) => this.RegisterPhoneHandHiddenListener(callback, (object) null);

  public bool RegisterPhoneHandHiddenListener(
    InputManager.PhoneHandHiddenCallback callback,
    object userData)
  {
    InputManager.PhoneHandHiddenListener handHiddenListener = new InputManager.PhoneHandHiddenListener();
    handHiddenListener.SetCallback(callback);
    handHiddenListener.SetUserData(userData);
    if (this.m_phoneHandHiddenListener.Contains(handHiddenListener))
      return false;
    this.m_phoneHandHiddenListener.Add(handHiddenListener);
    return true;
  }

  public bool RemovePhoneHandHiddenListener(InputManager.PhoneHandHiddenCallback callback) => this.RemovePhoneHandHiddenListener(callback, (object) null);

  public bool RemovePhoneHandHiddenListener(
    InputManager.PhoneHandHiddenCallback callback,
    object userData)
  {
    InputManager.PhoneHandHiddenListener handHiddenListener = new InputManager.PhoneHandHiddenListener();
    handHiddenListener.SetCallback(callback);
    handHiddenListener.SetUserData(userData);
    return this.m_phoneHandHiddenListener.Remove(handHiddenListener);
  }

  public bool HasPlayFromMiniHandEnabled() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().EnablePlayingFromMiniHand;

  public delegate bool TooltipContentDelegate(
    ref string headline,
    ref string description,
    int index);

  public delegate void OnTooltipShownDelegate(Action<string, string> showRegularTooltip);

  public delegate void OnTooltipHiddenDelegate();

  public class TooltipSettings
  {
    private InputManager.TooltipContentDelegate m_overrideContentDelegate;
    private InputManager.OnTooltipShownDelegate m_onTooltipShown;
    private InputManager.OnTooltipHiddenDelegate m_onTooltipHidden;

    public TooltipSettings()
    {
    }

    public TooltipSettings(bool allowed)
    {
      this.Allowed = allowed;
      this.m_overrideContentDelegate = (InputManager.TooltipContentDelegate) null;
    }

    public TooltipSettings(
      bool allowed,
      InputManager.TooltipContentDelegate contentDelegate)
    {
      this.Allowed = allowed;
      this.m_overrideContentDelegate = contentDelegate;
    }

    public static InputManager.TooltipSettings CreateCustomHandler(
      InputManager.OnTooltipShownDelegate onTooltipShown,
      InputManager.OnTooltipHiddenDelegate onTooltipHidden)
    {
      return new InputManager.TooltipSettings()
      {
        IsCustomHandler = true,
        m_onTooltipShown = onTooltipShown,
        m_onTooltipHidden = onTooltipHidden
      };
    }

    public bool Allowed { get; private set; }

    public bool IsCustomHandler { get; private set; }

    public void FireOnTooltipShown(Action<string, string> showRegularTooltip)
    {
      InputManager.OnTooltipShownDelegate onTooltipShown = this.m_onTooltipShown;
      if (onTooltipShown == null)
        return;
      onTooltipShown(showRegularTooltip);
    }

    public void FireOnTooltipHidden()
    {
      InputManager.OnTooltipHiddenDelegate onTooltipHidden = this.m_onTooltipHidden;
      if (onTooltipHidden == null)
        return;
      onTooltipHidden();
    }

    public bool GetTooltipOverrideContent(ref string headline, ref string description, int index = 0) => this.m_overrideContentDelegate != null && this.m_overrideContentDelegate(ref headline, ref description, index);
  }

  public class ZoneTooltipSettings
  {
    public InputManager.TooltipSettings EnemyHand = new InputManager.TooltipSettings(true);
    public InputManager.TooltipSettings EnemyDeck = new InputManager.TooltipSettings(true);
    public InputManager.TooltipSettings EnemyMana = new InputManager.TooltipSettings(true);
    public InputManager.TooltipSettings FriendlyHand = new InputManager.TooltipSettings(true);
    public InputManager.TooltipSettings FriendlyDeck = new InputManager.TooltipSettings(true);
    public InputManager.TooltipSettings FriendlyMana = new InputManager.TooltipSettings(true);
  }

  private class RaycastHitComparer : IComparer<RaycastHit>
  {
    public int Compare(RaycastHit hit1, RaycastHit hit2)
    {
      float y = hit1.point.y;
      return hit2.point.y.CompareTo(y);
    }
  }

  public delegate void PhoneHandShownCallback(object userData);

  private class PhoneHandShownListener : EventListener<InputManager.PhoneHandShownCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }

  public delegate void PhoneHandHiddenCallback(object userData);

  private class PhoneHandHiddenListener : EventListener<InputManager.PhoneHandHiddenCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
