using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteActionHandler : MonoBehaviour
{
  public const string TWEEN_NAME = "RemoteActionHandler";
  private const float DRIFT_TIME = 10f;
  private const float LOW_FREQ_SEND_TIME = 0.35f;
  private const float HIGH_FREQ_SEND_TIME = 0.25f;
  private const float ENEMY_TARGET_ARROW_DESTROY_DELAY = 0.25f;
  private static RemoteActionHandler s_instance;
  private int myCurrentEntitySelection;
  private int myLastEntitySelection;
  private int myLastUnsentEntitySelection = -1;
  private RemoteActionHandler.UserUI myCurrentUI = new RemoteActionHandler.UserUI();
  private RemoteActionHandler.UserUI myLastUI = new RemoteActionHandler.UserUI();
  private RemoteActionHandler.UserUI myLastUnsentUI = new RemoteActionHandler.UserUI();
  private RemoteActionHandler.UserUI enemyWantedUI = new RemoteActionHandler.UserUI();
  private RemoteActionHandler.UserUI enemyActualUI = new RemoteActionHandler.UserUI();
  private RemoteActionHandler.UserUI friendlyWantedUI = new RemoteActionHandler.UserUI();
  private RemoteActionHandler.UserUI friendlyActualUI = new RemoteActionHandler.UserUI();
  private float m_lastSendTime;
  private IEnumerator m_destroyEnemyTargetArrowCoroutine;

  private void Awake()
  {
    RemoteActionHandler.s_instance = this;
    this.m_lastSendTime = Time.realtimeSinceStartup;
    if (GameState.Get() == null)
      Debug.LogError((object) string.Format("RemoteActionHandler.Awake() - GameState already Shutdown before RemoteActionHandler was loaded."));
    else
      GameState.Get().RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
  }

  private void OnDestroy()
  {
    RemoteActionHandler.s_instance = (RemoteActionHandler) null;
    this.StopAllCoroutines();
  }

  private void Update()
  {
    if ((UnityEngine.Object) TargetReticleManager.Get() != (UnityEngine.Object) null)
      TargetReticleManager.Get().UpdateArrowPosition();
    this.ProcessUserUI();
    this.ProcessUserSelection();
  }

  public static RemoteActionHandler Get() => RemoteActionHandler.s_instance;

  public Card GetOpponentHeldCard() => this.enemyActualUI.held.card;

  public Card GetFriendlyHoverCard() => this.friendlyActualUI.over.card;

  public Card GetFriendlyHeldCard() => this.friendlyActualUI.held.card;

  public void NotifyOpponentOfSelection(int entityID) => this.myCurrentEntitySelection = entityID;

  public void NotifyOpponentOfMouseOverEntity(Card card) => this.myCurrentUI.over.card = card;

  public void NotifyOpponentOfMouseOut() => this.myCurrentUI.over.card = (Card) null;

  public void NotifyOpponentOfTargetModeBegin(Card card) => this.myCurrentUI.origin.card = card;

  public void NotifyOpponentOfTargetEnd() => this.myCurrentUI.origin.card = (Card) null;

  public void NotifyOpponentOfCardPickedUp(Card card) => this.myCurrentUI.held.card = card;

  public void NotifyOpponentOfCardDropped() => this.myCurrentUI.held.card = (Card) null;

  public void HandleAction(Network.UserUI newData)
  {
    bool flag = false;
    if (newData.playerId.HasValue)
    {
      Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
      if (friendlySidePlayer != null)
      {
        flag = friendlySidePlayer.GetPlayerId() == newData.playerId.Value;
        if (!flag)
        {
          Player player = GameState.Get().GetPlayer(newData.playerId.Value);
          flag = player != null && friendlySidePlayer.GetTeamId() == player.GetTeamId();
        }
      }
    }
    if (newData.mouseInfo != null)
    {
      if (flag)
      {
        this.friendlyWantedUI.held.ID = newData.mouseInfo.HeldCardID;
        this.friendlyWantedUI.over.ID = newData.mouseInfo.OverCardID;
        this.friendlyWantedUI.origin.ID = newData.mouseInfo.ArrowOriginID;
      }
      else
      {
        this.enemyWantedUI.held.ID = newData.mouseInfo.HeldCardID;
        this.enemyWantedUI.over.ID = newData.mouseInfo.OverCardID;
        this.enemyWantedUI.origin.ID = newData.mouseInfo.ArrowOriginID;
      }
      this.UpdateCardOver();
      this.UpdateCardHeld();
      this.MaybeDestroyArrow();
      this.MaybeCreateArrow();
      this.UpdateTargetArrow();
    }
    else if (newData.emoteInfo != null)
    {
      EmoteType emote = (EmoteType) newData.emoteInfo.Emote;
      if (flag)
      {
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.HAS_ALTERNATE_ENEMY_EMOTE_ACTOR))
          GameState.Get().GetGameEntity().PlayAlternateEnemyEmote(newData.playerId.Value, emote, newData.emoteInfo.BattlegroundsEmoteId);
        else
          GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(emote);
      }
      else
      {
        if (!this.CanReceiveEnemyEmote(emote, newData.playerId.Value))
          return;
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.HAS_ALTERNATE_ENEMY_EMOTE_ACTOR))
          GameState.Get().GetGameEntity().PlayAlternateEnemyEmote(newData.playerId.Value, emote, newData.emoteInfo.BattlegroundsEmoteId);
        else
          GameState.Get().GetOpposingSidePlayer().GetHeroCard().PlayEmote(emote);
      }
    }
    else
    {
      if (newData.selectionInfo == null || !flag || !GameMgr.Get().IsSpectator())
        return;
      Entity entity = GameState.Get().GetEntity(newData.selectionInfo.SelectedEntityID);
      if (entity == null)
        ZoneMgr.Get().DismissMercenariesAbilityTray();
      else
        ZoneMgr.Get().DisplayLettuceAbilitiesForEntity(entity);
    }
  }

  private void ProcessUserUI()
  {
    if (this.myCurrentUI.SameAs(this.myLastUI))
      return;
    if (!this.CanSendUI())
    {
      if (this.myCurrentUI.IsSourceOrTargetNull())
        return;
      this.myLastUnsentUI.CopyFrom(this.myCurrentUI);
    }
    else
    {
      if (!this.myCurrentUI.SameAs(this.myLastUnsentUI) && this.myCurrentUI.IsSourceOrTargetNull() && !this.myLastUnsentUI.IsSourceOrTargetNull())
        Network.Get().SendUserUI(this.myLastUnsentUI.over.ID, this.myLastUnsentUI.held.ID, this.myLastUnsentUI.origin.ID, 0, 0);
      else
        Network.Get().SendUserUI(this.myCurrentUI.over.ID, this.myCurrentUI.held.ID, this.myCurrentUI.origin.ID, 0, 0);
      this.myLastUI.CopyFrom(this.myCurrentUI);
      this.myLastUnsentUI.Clear();
    }
  }

  private void ProcessUserSelection()
  {
    if (this.myCurrentEntitySelection == this.myLastEntitySelection)
      return;
    if (!this.CanSendSelection())
    {
      this.myLastEntitySelection = this.myCurrentEntitySelection;
    }
    else
    {
      if (this.myLastUnsentEntitySelection >= 0 && this.myCurrentEntitySelection != this.myLastUnsentEntitySelection)
        Network.Get().SendSelection(this.myLastUnsentEntitySelection);
      else
        Network.Get().SendSelection(this.myCurrentEntitySelection);
      this.myLastEntitySelection = this.myCurrentEntitySelection;
      this.myLastUnsentEntitySelection = -1;
    }
  }

  private bool CanSendUI()
  {
    if (GameMgr.Get() == null || !InputManager.Get().PermitDecisionMakingInput() || GameMgr.Get().IsAI() && !SpectatorManager.Get().MyGameHasSpectators())
      return false;
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    float num = realtimeSinceStartup - this.m_lastSendTime;
    if (this.IsSendingTargetingArrow() && (double) num > 0.25)
    {
      this.m_lastSendTime = realtimeSinceStartup;
      return true;
    }
    if ((double) num < 0.349999994039536)
      return false;
    this.m_lastSendTime = realtimeSinceStartup;
    return true;
  }

  private bool CanSendSelection() => GameMgr.Get() != null && InputManager.Get().PermitDecisionMakingInput();

  private bool IsSendingTargetingArrow() => !((UnityEngine.Object) this.myCurrentUI.origin.card == (UnityEngine.Object) null) && !((UnityEngine.Object) this.myCurrentUI.over.card == (UnityEngine.Object) null) && !((UnityEngine.Object) this.myCurrentUI.over.card == (UnityEngine.Object) this.myCurrentUI.origin.card) && ((UnityEngine.Object) this.myCurrentUI.origin.card != (UnityEngine.Object) this.myLastUI.origin.card || (UnityEngine.Object) this.myCurrentUI.over.card != (UnityEngine.Object) this.myLastUI.over.card);

  private void UpdateCardOver()
  {
    Card card1 = this.enemyActualUI.over.card;
    Card card2 = this.enemyWantedUI.over.card;
    if ((UnityEngine.Object) card1 != (UnityEngine.Object) card2)
    {
      this.enemyActualUI.over.card = card2;
      if (!GameState.Get().GetGameEntity().HasTag(GAME_TAG.REVEAL_CHOICES))
      {
        if ((UnityEngine.Object) card1 != (UnityEngine.Object) null)
          card1.NotifyOpponentMousedOffThisCard();
        if ((UnityEngine.Object) card2 != (UnityEngine.Object) null)
          card2.NotifyOpponentMousedOverThisCard();
      }
      ZoneMgr.Get().FindZoneOfType<ZoneHand>(Player.Side.OPPOSING).UpdateLayout(card2);
    }
    if (!GameMgr.Get().IsSpectator())
      return;
    Card card3 = this.friendlyActualUI.over.card;
    Card card4 = this.friendlyWantedUI.over.card;
    if (!((UnityEngine.Object) card3 != (UnityEngine.Object) card4))
      return;
    this.friendlyActualUI.over.card = card4;
    if ((UnityEngine.Object) card3 != (UnityEngine.Object) null)
    {
      ZoneHand zone = card3.GetZone() as ZoneHand;
      if ((UnityEngine.Object) zone != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) zone.CurrentStandIn == (UnityEngine.Object) null)
          zone.UpdateLayout((Card) null);
      }
      else
        card3.NotifyMousedOut();
    }
    if (!((UnityEngine.Object) card4 != (UnityEngine.Object) null))
      return;
    ZoneHand zone1 = card4.GetZone() as ZoneHand;
    if ((UnityEngine.Object) zone1 != (UnityEngine.Object) null)
    {
      if (!((UnityEngine.Object) zone1.CurrentStandIn == (UnityEngine.Object) null))
        return;
      zone1.UpdateLayout(card4);
    }
    else
      card4.NotifyMousedOver();
  }

  private void UpdateCardHeld()
  {
    Card card1 = this.enemyActualUI.held.card;
    Card card2 = this.enemyWantedUI.held.card;
    if ((UnityEngine.Object) card1 != (UnityEngine.Object) card2)
    {
      this.enemyActualUI.held.card = card2;
      if ((UnityEngine.Object) card1 != (UnityEngine.Object) null)
        card1.MarkAsGrabbedByEnemyActionHandler(false);
      if (this.IsCardInHand(card1))
        card1.GetZone().UpdateLayout();
      if (this.CanAnimateHeldCard(card2))
      {
        card2.MarkAsGrabbedByEnemyActionHandler(true);
        if (SpectatorManager.Get().IsSpectatingOpposingSide())
          this.StandUpright(false);
        Hashtable args = iTween.Hash((object) "name", (object) nameof (RemoteActionHandler), (object) "position", (object) Board.Get().FindBone("OpponentCardPlayingSpot").position, (object) "time", (object) 1f, (object) "oncomplete", (object) (Action<object>) (o => this.StartDrift(false)), (object) "oncompletetarget", (object) this.gameObject);
        iTween.MoveTo(card2.gameObject, args);
      }
    }
    if (!GameMgr.Get().IsSpectator())
      return;
    Card card3 = this.friendlyActualUI.held.card;
    Card card4 = this.friendlyWantedUI.held.card;
    if (!((UnityEngine.Object) card3 != (UnityEngine.Object) card4))
      return;
    this.friendlyActualUI.held.card = card4;
    if ((UnityEngine.Object) card3 != (UnityEngine.Object) null)
      card3.MarkAsGrabbedByEnemyActionHandler(false);
    if (this.IsCardInHand(card3))
      card3.GetZone().UpdateLayout();
    if (!this.CanAnimateHeldCard(card4))
      return;
    card4.MarkAsGrabbedByEnemyActionHandler(true);
    ZoneHand zone = card4.GetZone() as ZoneHand;
    if ((UnityEngine.Object) zone != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) zone.CurrentStandIn == (UnityEngine.Object) null || (UnityEngine.Object) zone.CurrentStandIn.linkedCard == (UnityEngine.Object) card4)
        card4.NotifyMousedOut();
      Hashtable args = iTween.Hash((object) "scale", (object) zone.GetCardScale(), (object) "time", (object) 0.15f, (object) "easeType", (object) iTween.EaseType.easeOutExpo, (object) "name", (object) nameof (RemoteActionHandler));
      iTween.ScaleTo(card4.gameObject, args);
    }
    Hashtable args1 = iTween.Hash((object) "name", (object) nameof (RemoteActionHandler), (object) "position", (object) Board.Get().FindBone("FriendlyCardPlayingSpot").position, (object) "time", (object) 1f, (object) "oncomplete", (object) (Action<object>) (o => this.StartDrift(true)), (object) "oncompletetarget", (object) this.gameObject);
    iTween.MoveTo(card4.gameObject, args1);
    LayerUtils.SetLayer((Component) card4, GameLayer.Default);
  }

  private void StartDrift(bool isFriendlySide)
  {
    if (isFriendlySide || !GameState.Get().GetOpposingSidePlayer().IsRevealed())
      this.StandUpright(isFriendlySide);
    this.DriftLeftAndRight(isFriendlySide);
  }

  private void DriftLeftAndRight(bool isFriendlySide)
  {
    Card card = isFriendlySide ? this.friendlyActualUI.held.card : this.enemyActualUI.held.card;
    if (!this.CanAnimateHeldCard(card))
      return;
    Vector3[] vector3Array;
    if (isFriendlySide)
    {
      iTweenPath iTweenPath;
      if (!iTweenPath.paths.TryGetValue(iTweenPath.FixupPathName("driftPath1_friendly"), out iTweenPath))
      {
        Transform bone1 = Board.Get().FindBone("OpponentCardPlayingSpot");
        Transform bone2 = Board.Get().FindBone("FriendlyCardPlayingSpot");
        Vector3 vector3 = bone2.position - bone1.position;
        iTweenPath path = iTweenPath.paths[iTweenPath.FixupPathName("driftPath1")];
        iTweenPath = bone2.gameObject.AddComponent<iTweenPath>();
        iTweenPath.pathVisible = true;
        iTweenPath.pathName = "driftPath1_friendly";
        iTweenPath.pathColor = path.pathColor;
        iTweenPath.nodes = new List<Vector3>((IEnumerable<Vector3>) path.nodes);
        for (int index = 0; index < iTweenPath.nodes.Count; ++index)
          iTweenPath.nodes[index] = path.nodes[index] + vector3;
        iTweenPath.enabled = false;
        iTweenPath.enabled = true;
      }
      vector3Array = iTweenPath.nodes.ToArray();
    }
    else
      vector3Array = iTweenPath.GetPath("driftPath1");
    Hashtable args = iTween.Hash((object) "name", (object) nameof (RemoteActionHandler), (object) "path", (object) vector3Array, (object) "time", (object) 10f, (object) "easetype", (object) iTween.EaseType.linear, (object) "looptype", (object) iTween.LoopType.pingPong);
    iTween.MoveTo(card.gameObject, args);
  }

  private void StandUpright(bool isFriendlySide)
  {
    Card card = isFriendlySide ? this.friendlyActualUI.held.card : this.enemyActualUI.held.card;
    if (!this.CanAnimateHeldCard(card))
      return;
    float num = 5f;
    if (!isFriendlySide && GameState.Get().GetOpposingSidePlayer().IsRevealed())
      num = 0.3f;
    Hashtable args = iTween.Hash((object) "name", (object) nameof (RemoteActionHandler), (object) "rotation", (object) Vector3.zero, (object) "time", (object) num, (object) "easetype", (object) iTween.EaseType.easeInOutSine);
    iTween.RotateTo(card.gameObject, args);
  }

  private void MaybeDestroyArrow()
  {
    if ((UnityEngine.Object) TargetReticleManager.Get() == (UnityEngine.Object) null || !TargetReticleManager.Get().IsActive())
      return;
    bool flag1 = GameState.Get() != null && GameState.Get().IsFriendlySidePlayerTurn();
    RemoteActionHandler.UserUI userUi1 = flag1 ? this.friendlyWantedUI : this.enemyWantedUI;
    RemoteActionHandler.UserUI userUi2 = flag1 ? this.friendlyActualUI : this.enemyActualUI;
    if ((UnityEngine.Object) userUi1.origin.card == (UnityEngine.Object) userUi2.origin.card)
      return;
    if ((UnityEngine.Object) userUi2.origin.card != (UnityEngine.Object) null && (UnityEngine.Object) userUi2.origin.card.GetActor() != (UnityEngine.Object) null && !userUi2.origin.card.ShouldShowImmuneVisuals())
      userUi2.origin.card.GetActor().ActivateSpellDeathState(SpellType.IMMUNE);
    bool flag2 = userUi2.origin.entity != null && userUi2.origin.entity.IsLettuceAbility();
    userUi2.origin.card = (Card) null;
    if (flag1)
    {
      if (flag2)
        ZoneMgr.Get().DisplayLettuceAbilitiesForPreviouslySelectedCard();
      TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
    }
    else
    {
      this.m_destroyEnemyTargetArrowCoroutine = this.DestroyEnemyTargetArrow();
      this.StartCoroutine(this.m_destroyEnemyTargetArrowCoroutine);
    }
  }

  private void MaybeCreateArrow()
  {
    if ((UnityEngine.Object) TargetReticleManager.Get() == (UnityEngine.Object) null || TargetReticleManager.Get().IsActive() && !TargetReticleManager.Get().IsStaticArrow())
      return;
    bool flag = GameState.Get() != null && GameState.Get().IsFriendlySidePlayerTurn();
    RemoteActionHandler.UserUI userUi1 = flag ? this.friendlyWantedUI : this.enemyWantedUI;
    RemoteActionHandler.UserUI userUi2 = flag ? this.friendlyActualUI : this.enemyActualUI;
    if ((UnityEngine.Object) userUi1.origin.card == (UnityEngine.Object) null || (UnityEngine.Object) userUi2.over.card == (UnityEngine.Object) null || (UnityEngine.Object) userUi2.over.card.GetActor() == (UnityEngine.Object) null || !userUi2.over.card.GetActor().IsShown() || (UnityEngine.Object) userUi2.over.card == (UnityEngine.Object) userUi1.origin.card)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (currentPlayer == null || currentPlayer.IsLocalUser())
      return;
    userUi2.origin.card = userUi1.origin.card;
    if (flag)
    {
      bool showDamageIndicatorText = false;
      if (userUi2.origin.entity != null && userUi2.origin.entity.IsLettuceAbility())
      {
        showDamageIndicatorText = true;
        ZoneMgr.Get().TemporarilyDismissMercenariesAbilityTray();
      }
      TargetReticleManager.Get().CreateFriendlyTargetArrow(userUi2.origin.entity, showDamageIndicatorText);
    }
    else
    {
      if (this.m_destroyEnemyTargetArrowCoroutine != null)
        this.StopCoroutine(this.m_destroyEnemyTargetArrowCoroutine);
      TargetReticleManager.Get().CreateEnemyTargetArrow(userUi2.origin.entity);
    }
    if (userUi2.origin.entity.GetRealTimeIsImmuneWhileAttacking())
      userUi2.origin.card.ActivateActorSpell(SpellType.IMMUNE);
    this.SetArrowTarget();
  }

  private IEnumerator DestroyEnemyTargetArrow()
  {
    yield return (object) new WaitForSeconds(0.25f);
    TargetReticleManager.Get().DestroyEnemyTargetArrow();
  }

  private void UpdateTargetArrow()
  {
    if ((UnityEngine.Object) TargetReticleManager.Get() == (UnityEngine.Object) null || !TargetReticleManager.Get().IsActive())
      return;
    this.SetArrowTarget();
  }

  private void SetArrowTarget()
  {
    int num = GameState.Get() == null ? 0 : (GameState.Get().IsFriendlySidePlayerTurn() ? 1 : 0);
    RemoteActionHandler.UserUI userUi1 = num != 0 ? this.friendlyWantedUI : this.enemyWantedUI;
    RemoteActionHandler.UserUI userUi2 = num != 0 ? this.friendlyActualUI : this.enemyActualUI;
    if ((UnityEngine.Object) userUi2.over.card == (UnityEngine.Object) null || (UnityEngine.Object) userUi2.origin.card == (UnityEngine.Object) null || (UnityEngine.Object) userUi2.over.card.GetActor() == (UnityEngine.Object) null || !userUi2.over.card.GetActor().IsShown() || (UnityEngine.Object) userUi2.over.card == (UnityEngine.Object) userUi1.origin.card)
      return;
    Vector3 position1 = Camera.main.transform.position;
    Vector3 position2 = userUi2.over.card.transform.position;
    RaycastHit hitInfo;
    if (!Physics.Raycast(new Ray(position1, position2 - position1), out hitInfo, Camera.main.farClipPlane, GameLayer.DragPlane.LayerBit()))
      return;
    TargetReticleManager.Get().SetRemotePlayerArrowPosition(hitInfo.point);
  }

  private bool IsCardInHand(Card card) => !((UnityEngine.Object) card == (UnityEngine.Object) null) && card.GetZone() is ZoneHand && card.GetEntity().GetZone() == TAG_ZONE.HAND;

  private bool CanAnimateHeldCard(Card card)
  {
    if (!this.IsCardInHand(card))
      return false;
    string tweenName = ZoneMgr.Get().GetTweenName<ZoneHand>();
    return !iTween.HasNameNotInList(card.gameObject, nameof (RemoteActionHandler), tweenName);
  }

  private void OnTurnChanged(int oldTurn, int newTurn, object userData)
  {
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (currentPlayer != null && !currentPlayer.IsLocalUser() && !GameMgr.Get().IsSpectator() || (UnityEngine.Object) TargetReticleManager.Get() == (UnityEngine.Object) null)
      return;
    RemoteActionHandler.UserUI userUi;
    if (currentPlayer.IsFriendlySide())
    {
      userUi = this.friendlyActualUI;
      if (TargetReticleManager.Get().IsEnemyArrowActive())
        TargetReticleManager.Get().DestroyEnemyTargetArrow();
    }
    else
    {
      userUi = this.enemyActualUI;
      if (TargetReticleManager.Get().IsLocalArrowActive())
        TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
    }
    if (userUi.origin == null || userUi.origin.entity == null || !((UnityEngine.Object) userUi.origin.card != (UnityEngine.Object) null) || userUi.origin.card.ShouldShowImmuneVisuals())
      return;
    userUi.origin.card.GetActor().ActivateSpellDeathState(SpellType.IMMUNE);
  }

  private bool CanReceiveEnemyEmote(EmoteType emoteType, int playerId)
  {
    if ((UnityEngine.Object) EnemyEmoteHandler.Get() == (UnityEngine.Object) null && !GameState.Get().GetBooleanGameOption(GameEntityOption.USES_PREMIUM_EMOTES) || (UnityEngine.Object) EnemyEmoteHandler.Get() != (UnityEngine.Object) null && EnemyEmoteHandler.Get().IsSquelched(playerId))
      return false;
    if (emoteType == EmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE && GameMgr.Get().IsBattlegrounds())
      return true;
    return !((UnityEngine.Object) EmoteHandler.Get() == (UnityEngine.Object) null) && EmoteHandler.Get().IsValidEmoteTypeForOpponent(emoteType);
  }

  private class CardAndID
  {
    private int m_ID;
    private Entity m_entity;
    private Card m_card;

    public Card card
    {
      get => this.m_card;
      set
      {
        if ((UnityEngine.Object) value == (UnityEngine.Object) this.m_card)
          return;
        if ((UnityEngine.Object) value == (UnityEngine.Object) null)
        {
          this.Clear();
        }
        else
        {
          this.m_card = value;
          this.m_entity = value.GetEntity();
          if (this.m_entity == null)
          {
            Debug.LogWarning((object) "RemoteActionHandler--card has no entity");
            this.Clear();
          }
          else
          {
            this.m_ID = this.m_entity.GetEntityId();
            if (this.m_ID >= 1)
              return;
            Debug.LogWarning((object) "RemoteActionHandler--invalid entity ID");
            this.Clear();
          }
        }
      }
    }

    public int ID
    {
      get => this.m_ID;
      set
      {
        if (value == this.m_ID)
          return;
        if (value == 0)
        {
          this.Clear();
        }
        else
        {
          this.m_ID = value;
          this.m_entity = GameState.Get().GetEntity(value);
          if (this.m_entity == null)
          {
            Debug.LogWarning((object) "RemoteActionHandler--no entity found for ID");
            this.Clear();
          }
          else
          {
            this.m_card = this.m_entity.GetCard();
            if (!((UnityEngine.Object) this.m_card == (UnityEngine.Object) null))
              return;
            Debug.LogWarning((object) "RemoteActionHandler--entity has no card");
            this.Clear();
          }
        }
      }
    }

    public Entity entity => this.m_entity;

    private void Clear()
    {
      this.m_ID = 0;
      this.m_entity = (Entity) null;
      this.m_card = (Card) null;
    }
  }

  private class UserUI
  {
    public RemoteActionHandler.CardAndID over = new RemoteActionHandler.CardAndID();
    public RemoteActionHandler.CardAndID held = new RemoteActionHandler.CardAndID();
    public RemoteActionHandler.CardAndID origin = new RemoteActionHandler.CardAndID();

    public bool SameAs(RemoteActionHandler.UserUI compare) => !((UnityEngine.Object) this.held.card != (UnityEngine.Object) compare.held.card) && !((UnityEngine.Object) this.over.card != (UnityEngine.Object) compare.over.card) && !((UnityEngine.Object) this.origin.card != (UnityEngine.Object) compare.origin.card);

    public void CopyFrom(RemoteActionHandler.UserUI source)
    {
      this.held.ID = source.held.ID;
      this.over.ID = source.over.ID;
      this.origin.ID = source.origin.ID;
    }

    public bool IsSourceOrTargetNull() => (UnityEngine.Object) this.over.card == (UnityEngine.Object) null || (UnityEngine.Object) this.origin.card == (UnityEngine.Object) null;

    public void Clear()
    {
      this.held.card = (Card) null;
      this.over.card = (Card) null;
      this.origin.card = (Card) null;
    }
  }
}
