using Blizzard.T5.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneHand : Zone
{
  public GameObject m_iPhoneCardPosition;
  public GameObject m_leftArrow;
  public GameObject m_rightArrow;
  public GameObject m_manaGemPosition;
  public ManaCrystalMgr m_manaGemMgr;
  public GameObject m_playCardButton;
  public GameObject m_iPhonePreviewBone;
  public Float_MobileOverride m_SelectCardOffsetZ;
  public Float_MobileOverride m_SelectCardScale;
  public Float_MobileOverride m_TouchDragResistanceFactorY;
  public TwinspellHoldSpell m_TwinspellHoldSpell;
  public Vector3 m_enlargedHandPosition;
  public Vector3 m_enlargedHandScale;
  public Vector3 m_enlargedHandCardScale;
  public float m_enlargedHandDefaultCardSpacing;
  public float m_enlargedHandCardMinX;
  public float m_enlargedHandCardMaxX;
  public float m_heroWidthInHand;
  public float m_handHidingDistance;
  public GameObject m_heroHitbox;
  public float m_tinyHandMouseOverYOffset = 4.6f;
  public float m_tinyHandMouseOverZOffset = 3.3f;
  public float m_tinyHandMobileHoverDelaySeconds = 0.25f;
  public const float MOUSE_OVER_SCALE = 1.5f;
  public const float HAND_SCALE = 0.62f;
  public const float HAND_SCALE_Y = 0.225f;
  public const float HAND_SCALE_OPPONENT = 0.682f;
  public const float HAND_SCALE_OPPONENT_Y = 0.225f;
  private const float ANGLE_OF_CARDS = 40f;
  private const float DEFAULT_ANIMATE_TIME = 0.35f;
  private const float DRIFT_AMOUNT = 0.08f;
  private const float Z_ROTATION_ON_LEFT = 354.5f;
  private const float Z_ROTATION_ON_RIGHT = 3f;
  private const float RESISTANCE_BASE = 10f;
  private Card m_lastMousedOverCard;
  private float m_maxWidth;
  private bool m_doNotUpdateLayout = true;
  private Vector3 centerOfHand;
  private bool enemyHand;
  private bool m_handEnlarged;
  private Vector3 m_startingPosition;
  private Vector3 m_startingScale;
  private bool m_handMoving;
  private bool m_targetingMode;
  private int m_touchedSlot;
  private CardStandIn m_hiddenStandIn;
  private TwinspellHoldSpell m_twinspellHoldSpellInstance;
  private int m_playingTwinspellEntityId = -1;
  private int m_reservedSlot = -1;
  private List<CardStandIn> standIns;
  private bool m_flipHandCards;

  public CardStandIn CurrentStandIn => (UnityEngine.Object) this.m_lastMousedOverCard == (UnityEngine.Object) null ? (CardStandIn) null : this.GetStandIn(this.m_lastMousedOverCard);

  public bool IsCardFocused { get; private set; }

  private void Awake()
  {
    this.enemyHand = this.m_Side == Player.Side.OPPOSING;
    this.m_startingPosition = this.gameObject.transform.localPosition;
    this.m_startingScale = this.gameObject.transform.localScale;
    this.UpdateCenterAndWidth();
  }

  private void Start() => GameState.Get().RegisterCantPlayListener(new GameState.CantPlayCallback(this.OnCantPlay));

  public Card GetLastMousedOverCard() => this.m_lastMousedOverCard;

  public bool IsHandScrunched(int cardCount)
  {
    if (this.m_handEnlarged && cardCount > 3)
      return true;
    double defaultCardSpacing = (double) this.GetDefaultCardSpacing();
    if (!this.enemyHand)
      cardCount -= TurnStartManager.Get().GetNumCardsToDraw();
    double num = (double) cardCount;
    return defaultCardSpacing * num > (double) this.MaxHandWidth();
  }

  public void SetDoNotUpdateLayout(bool enable) => this.m_doNotUpdateLayout = enable;

  public bool IsDoNotUpdateLayout() => this.m_doNotUpdateLayout;

  public override void OnSpellPowerEntityEnteredPlay(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    foreach (Card card in this.m_cards)
    {
      if (card.CanPlaySpellPowerHint(spellSchool))
      {
        Spell actorSpell = card.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          actorSpell.Reactivate();
      }
    }
  }

  public override void OnSpellPowerEntityMousedOver(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    if (TargetReticleManager.Get().IsActive())
      return;
    foreach (Card card in this.m_cards)
    {
      if (card.CanPlaySpellPowerHint(spellSchool))
      {
        Spell actorSpell1 = card.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST);
        if ((UnityEngine.Object) actorSpell1 != (UnityEngine.Object) null)
          actorSpell1.Reactivate();
        Spell actorSpell2 = card.GetActorSpell(SpellType.SPELL_POWER_HINT_IDLE);
        if ((UnityEngine.Object) actorSpell2 != (UnityEngine.Object) null)
          actorSpell2.ActivateState(SpellStateType.BIRTH);
      }
    }
  }

  public override void OnSpellPowerEntityMousedOut(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    foreach (Card card in this.m_cards)
    {
      Spell actorSpell = card.GetActorSpell(SpellType.SPELL_POWER_HINT_IDLE);
      if (!((UnityEngine.Object) actorSpell == (UnityEngine.Object) null) && actorSpell.IsActive())
        actorSpell.ActivateState(SpellStateType.DEATH);
    }
  }

  public float GetDefaultCardSpacing() => (bool) UniversalInputManager.UsePhoneUI && this.m_handEnlarged ? this.m_enlargedHandDefaultCardSpacing : 1.270804f;

  public int GetVisualCardCount() => this.m_reservedSlot == -1 ? this.m_cards.Count : this.m_cards.Count + 1;

  public void ReserveCardSlot(int slot) => this.m_reservedSlot = slot;

  public void SortWithSpotForReservedCard(int slot)
  {
    this.m_reservedSlot = slot;
    this.UpdateLayout();
  }

  public void ClearReservedCard() => this.SortWithSpotForReservedCard(-1);

  public override void UpdateLayout()
  {
    if (!GameState.Get().IsMulliganManagerActive() && !this.enemyHand)
    {
      this.BlowUpOldStandins();
      for (int index = 0; index < this.m_cards.Count; ++index)
        this.CreateCardStandIn(this.m_cards[index]);
    }
    this.UpdateLayout((Card) null, true, -1);
  }

  public void ForceStandInUpdate()
  {
    this.BlowUpOldStandins();
    for (int index = 0; index < this.m_cards.Count; ++index)
      this.CreateCardStandIn(this.m_cards[index]);
  }

  public void UpdateLayout(Card cardMousedOver) => this.UpdateLayout(cardMousedOver, false, -1);

  public void UpdateLayout(Card cardMousedOver, bool forced) => this.UpdateLayout(cardMousedOver, forced, -1);

  public void UpdateLayout(Card cardMousedOver, bool forced, int overrideCardCount)
  {
    ++this.m_updatingLayout;
    if (this.IsBlockingLayout())
    {
      this.UpdateLayoutFinished();
    }
    else
    {
      for (int index = 0; index < this.m_cards.Count; ++index)
      {
        Card card = this.m_cards[index];
        if (!card.IsDoNotSort() && card.GetTransitionStyle() != ZoneTransitionStyle.VERY_SLOW && !this.IsCardNotInEnemyHandAnymore(card) && !card.HasBeenGrabbedByEnemyActionHandler())
        {
          Spell bestSummonSpell = card.GetBestSummonSpell();
          if (!((UnityEngine.Object) bestSummonSpell != (UnityEngine.Object) null) || !bestSummonSpell.IsActive())
            card.ShowCard();
        }
      }
      if (this.m_doNotUpdateLayout)
      {
        this.UpdateLayoutFinished();
      }
      else
      {
        if ((UnityEngine.Object) cardMousedOver != (UnityEngine.Object) null && this.GetCardSlot(cardMousedOver) < 0)
          cardMousedOver = (Card) null;
        if (!forced && (UnityEngine.Object) cardMousedOver == (UnityEngine.Object) this.m_lastMousedOverCard)
        {
          --this.m_updatingLayout;
          this.UpdateKeywordPanelsPosition(cardMousedOver);
        }
        else
        {
          this.m_cards.Sort(new Comparison<Card>(Zone.CardSortComparison));
          this.FocusCard(cardMousedOver, overrideCardCount);
        }
      }
    }
  }

  public void HideCards()
  {
    foreach (Card card in this.m_cards)
      card.GetActor().gameObject.SetActive(false);
  }

  public void ShowCards()
  {
    foreach (Card card in this.m_cards)
      card.GetActor().gameObject.SetActive(true);
  }

  public float GetCardWidth()
  {
    int visualCardCount = this.GetVisualCardCount();
    if (!this.enemyHand)
      visualCardCount -= TurnStartManager.Get().GetNumCardsToDraw();
    float cardSpacing = this.GetCardSpacing(visualCardCount);
    Vector3 centerOfHand1 = this.centerOfHand;
    centerOfHand1.x -= cardSpacing / 2f;
    Vector3 centerOfHand2 = this.centerOfHand;
    centerOfHand2.x += cardSpacing / 2f;
    Vector3 screenPoint = Camera.main.WorldToScreenPoint(centerOfHand1);
    return Camera.main.WorldToScreenPoint(centerOfHand2).x - screenPoint.x;
  }

  public bool TouchReceived()
  {
    RaycastHit hitInfo;
    if (!UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.CardRaycast.LayerBit(), out hitInfo))
      this.m_touchedSlot = -1;
    CardStandIn componentInParents = GameObjectUtils.FindComponentInParents<CardStandIn>((Component) hitInfo.transform);
    if ((UnityEngine.Object) componentInParents != (UnityEngine.Object) null)
    {
      this.m_touchedSlot = this.GetCardSlot(componentInParents.linkedCard);
      return true;
    }
    this.m_touchedSlot = -1;
    return false;
  }

  public void HandleInput()
  {
    Card cardMousedOver1 = (Card) null;
    RemoteActionHandler remoteActionHandler = RemoteActionHandler.Get();
    if ((UnityEngine.Object) remoteActionHandler != (UnityEngine.Object) null)
    {
      Card friendlyHoverCard = remoteActionHandler.GetFriendlyHoverCard();
      if ((UnityEngine.Object) friendlyHoverCard != (UnityEngine.Object) null && friendlyHoverCard.GetController().IsFriendlySide() && friendlyHoverCard.GetZone() is ZoneHand)
        cardMousedOver1 = friendlyHoverCard;
    }
    UniversalInputManager universalInputManager = UniversalInputManager.Get();
    if (universalInputManager.IsTouchMode())
    {
      InputManager inputManager = InputManager.Get();
      if (!inputManager.LeftMouseButtonDown || this.m_touchedSlot < 0)
      {
        this.m_touchedSlot = -1;
        this.UpdateLayout(cardMousedOver1);
      }
      else
      {
        Vector3 mousePosition = InputCollection.GetMousePosition();
        Vector3 mouseDownPosition = inputManager.LastMouseDownPosition;
        float num1 = mousePosition.x - mouseDownPosition.x;
        float num2 = Mathf.Max(0.0f, mousePosition.y - mouseDownPosition.y);
        int cardSlot = this.GetCardSlot(this.m_lastMousedOverCard);
        float cardWidth = this.GetCardWidth();
        int touchedSlot = this.m_touchedSlot;
        float a = (float) (cardSlot - touchedSlot) * cardWidth;
        float num3 = (float) (10.0 + (double) num2 * (double) (float) (MobileOverrideValue<float>) this.m_TouchDragResistanceFactorY);
        int index = this.m_touchedSlot + (int) Math.Truncate(((double) num1 >= (double) a ? (double) Mathf.Max(a, num1 - num3) : (double) Mathf.Min(a, num1 + num3)) / (double) cardWidth);
        Card cardMousedOver2 = (Card) null;
        if (index >= 0 && index < this.m_cards.Count)
          cardMousedOver2 = this.m_cards[index];
        this.UpdateLayout(cardMousedOver2);
      }
    }
    else
    {
      CardStandIn cardStandIn = (CardStandIn) null;
      Card cardMousedOver3 = (Card) null;
      RaycastHit hitInfo;
      if (!universalInputManager.InputHitAnyObject(Camera.main, GameLayer.InvisibleHitBox1) || !universalInputManager.GetInputHitInfo(Camera.main, GameLayer.CardRaycast, out hitInfo))
      {
        if ((UnityEngine.Object) cardMousedOver1 == (UnityEngine.Object) null)
        {
          this.UpdateLayout((Card) null);
          return;
        }
      }
      else
        cardStandIn = GameObjectUtils.FindComponentInParents<CardStandIn>((Component) hitInfo.transform);
      if ((UnityEngine.Object) cardStandIn == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) cardMousedOver1 == (UnityEngine.Object) null)
        {
          this.UpdateLayout((Card) null);
          return;
        }
      }
      else
        cardMousedOver3 = cardStandIn.linkedCard;
      if ((UnityEngine.Object) cardMousedOver3 == (UnityEngine.Object) this.m_lastMousedOverCard)
        this.UpdateKeywordPanelsPosition(cardMousedOver3);
      else if ((UnityEngine.Object) cardMousedOver3 == (UnityEngine.Object) null && (UnityEngine.Object) cardMousedOver1 != (UnityEngine.Object) null)
        this.UpdateLayout(cardMousedOver1);
      else
        this.UpdateLayout(cardMousedOver3);
    }
  }

  private void UpdateKeywordPanelsPosition(Card cardMousedOver)
  {
    if ((UnityEngine.Object) cardMousedOver == (UnityEngine.Object) null || cardMousedOver.GetEntity() != null && cardMousedOver.GetEntity().IsHero())
      return;
    bool showOnRight = this.ShouldShowCardTooltipOnRight(cardMousedOver);
    TooltipPanelManager.Get().UpdateKeywordPanelsPosition(cardMousedOver, showOnRight);
  }

  public bool ShouldShowCardTooltipOnRight(Card card)
  {
    if (GameState.Get().IsMulliganManagerActive())
    {
      int num = (int) Mathf.Ceil((float) card.GetZone().GetCardCount() / 2f);
      return card.GetZonePosition() <= num;
    }
    if (InputManager.Get().HasPlayFromMiniHandEnabled() && (bool) UniversalInputManager.UsePhoneUI && !this.m_handEnlarged)
      return false;
    return card.GetEntity().HasTag(GAME_TAG.COLOSSAL) || card.GetEntity().HasTag(GAME_TAG.DISPLAY_CARD_ON_MOUSEOVER) ? card.GetEntity().HasTag(GAME_TAG.COLOSSAL_LIMB_ON_LEFT) : !((UnityEngine.Object) card.GetActor() == (UnityEngine.Object) null) && !((UnityEngine.Object) card.GetActor().GetMeshRenderer() == (UnityEngine.Object) null) && (double) card.GetActor().GetMeshRenderer().bounds.center.x < (double) this.GetComponent<BoxCollider>().bounds.center.x;
  }

  public void ShowManaGems()
  {
    Vector3 position = this.m_manaGemPosition.transform.position;
    position.x += -0.5f * this.m_manaGemMgr.GetWidth();
    this.m_manaGemMgr.gameObject.transform.position = position;
    this.m_manaGemMgr.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
  }

  public void HideManaGems() => this.m_manaGemMgr.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

  public void SetHandEnlarged(bool enlarged)
  {
    this.m_handEnlarged = enlarged;
    if (enlarged)
    {
      this.gameObject.transform.localPosition = this.m_enlargedHandPosition;
      this.gameObject.transform.localScale = this.m_enlargedHandScale;
      ManaCrystalMgr.Get().ShowPhoneManaTray();
    }
    else
    {
      this.gameObject.transform.localPosition = this.m_startingPosition;
      this.gameObject.transform.localScale = this.m_startingScale;
      ManaCrystalMgr.Get().HidePhoneManaTray();
    }
    this.UpdateCenterAndWidth();
    this.m_handMoving = true;
    this.UpdateLayout((Card) null, true);
    this.m_handMoving = false;
  }

  public bool HandEnlarged() => this.m_handEnlarged;

  public void SetFriendlyHeroTargetingMode(bool enable)
  {
    if (!enable && (UnityEngine.Object) this.m_hiddenStandIn != (UnityEngine.Object) null)
      this.m_hiddenStandIn.gameObject.SetActive(true);
    if (this.m_targetingMode == enable)
      return;
    this.m_targetingMode = enable;
    this.m_heroHitbox.SetActive(enable);
    if (!this.m_handEnlarged)
      return;
    if (enable)
    {
      this.m_hiddenStandIn = this.CurrentStandIn;
      if ((UnityEngine.Object) this.m_hiddenStandIn != (UnityEngine.Object) null)
        this.m_hiddenStandIn.gameObject.SetActive(false);
      Vector3 enlargedHandPosition = this.m_enlargedHandPosition;
      enlargedHandPosition.z -= this.m_handHidingDistance;
      this.gameObject.transform.localPosition = enlargedHandPosition;
    }
    else
      this.gameObject.transform.localPosition = this.m_enlargedHandPosition;
    this.UpdateCenterAndWidth();
  }

  private void FocusCard(Card cardMousedOver, int overrideCardCount)
  {
    if ((UnityEngine.Object) this.m_lastMousedOverCard != (UnityEngine.Object) cardMousedOver && (UnityEngine.Object) this.m_lastMousedOverCard != (UnityEngine.Object) null)
    {
      if (!InputManager.Get().HasPlayFromMiniHandEnabled() || this.IsCardFocused)
      {
        this.IsCardFocused = false;
        if (this.CanAnimateCard(this.m_lastMousedOverCard) && this.GetCardSlot(this.m_lastMousedOverCard) >= 0)
        {
          iTween.Stop(this.m_lastMousedOverCard.gameObject);
          if (!this.enemyHand)
          {
            this.m_lastMousedOverCard.transform.position = new Vector3(this.GetMouseOverCardPosition(this.m_lastMousedOverCard).x, this.centerOfHand.y, this.GetCardPosition(this.m_lastMousedOverCard, overrideCardCount).z + 0.5f);
            this.m_lastMousedOverCard.transform.localScale = this.GetCardScale();
            this.m_lastMousedOverCard.transform.localEulerAngles = this.GetCardRotation(this.m_lastMousedOverCard);
          }
          GameLayer layer = GameLayer.Default;
          if (this.m_Side == Player.Side.OPPOSING && this.m_controller.IsRevealed())
            layer = GameLayer.CardRaycast;
          LayerUtils.SetLayer(this.m_lastMousedOverCard.gameObject, layer);
        }
      }
      this.m_lastMousedOverCard.NotifyMousedOut();
    }
    int num1 = 0;
    float delaySec = 0.0f;
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      Card card = this.m_cards[index];
      if (this.CanAnimateCard(card))
      {
        ++num1;
        float z = this.m_flipHandCards ? 534.5f : 354.5f;
        card.transform.rotation = Quaternion.Euler(new Vector3(card.transform.localEulerAngles.x, card.transform.localEulerAngles.y, z));
        float num2 = 0.5f;
        if (this.m_handMoving)
          num2 = 0.25f;
        if (this.enemyHand)
          num2 = 1.5f;
        float num3 = 0.25f;
        iTween.EaseType easeType = iTween.EaseType.easeOutExpo;
        float transitionDelay = card.GetTransitionDelay();
        card.SetTransitionDelay(0.0f);
        ZoneTransitionStyle transitionStyle = card.GetTransitionStyle();
        card.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
        switch (transitionStyle)
        {
          case ZoneTransitionStyle.NORMAL:
            Vector3 vector3_1 = this.GetCardPosition(card, overrideCardCount);
            Vector3 vector3_2 = this.GetCardRotation(card, overrideCardCount);
            Vector3 vector3_3 = this.GetCardScale();
            if ((UnityEngine.Object) card == (UnityEngine.Object) cardMousedOver && this.ShouldCheckTapWhenClickingMiniHand())
            {
              this.IsCardFocused = true;
              easeType = iTween.EaseType.easeOutExpo;
              if (this.enemyHand)
              {
                num3 = 0.15f;
                float num4 = 0.3f;
                vector3_1 = new Vector3(vector3_1.x, vector3_1.y, vector3_1.z - num4);
              }
              else
              {
                float selectCardScale1 = (float) (MobileOverrideValue<float>) this.m_SelectCardScale;
                float selectCardScale2 = (float) (MobileOverrideValue<float>) this.m_SelectCardScale;
                vector3_2 = new Vector3(0.0f, 0.0f, 0.0f);
                vector3_3 = new Vector3(selectCardScale1, vector3_3.y, selectCardScale2);
                card.transform.localScale = vector3_3;
                float num5 = 0.1f;
                vector3_1 = this.GetMouseOverCardPosition(card);
                float x = vector3_1.x;
                if ((bool) UniversalInputManager.UsePhoneUI)
                {
                  vector3_1.x = Mathf.Max(vector3_1.x, this.m_enlargedHandCardMinX);
                  vector3_1.x = Mathf.Min(vector3_1.x, this.m_enlargedHandCardMaxX);
                }
                card.transform.position = new Vector3((double) x != (double) vector3_1.x ? vector3_1.x : card.transform.position.x, vector3_1.y, vector3_1.z - num5);
                card.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                iTween.Stop(card.gameObject);
                easeType = iTween.EaseType.easeOutExpo;
                if ((bool) (UnityEngine.Object) CardTypeBanner.Get())
                  CardTypeBanner.Get().Show(card);
                InputManager.Get().SetMousedOverCard(card);
                if (card.GetEntity() == null || !card.GetEntity().IsHero())
                {
                  bool showOnRight = this.ShouldShowCardTooltipOnRight(card);
                  TooltipPanelManager.Get().UpdateKeywordHelp(card, card.GetActor(), showOnRight);
                }
                LayerUtils.SetLayer(card.gameObject, GameLayer.Tooltip);
              }
            }
            else if ((UnityEngine.Object) this.GetStandIn(card) != (UnityEngine.Object) null)
            {
              CardStandIn standIn = this.GetStandIn(card);
              iTween.Stop(standIn.gameObject);
              standIn.transform.position = vector3_1;
              standIn.transform.localEulerAngles = vector3_2;
              standIn.transform.localScale = vector3_3;
              if (!card.CardStandInIsInteractive())
                standIn.DisableStandIn();
              else
                standIn.EnableStandIn();
            }
            if (transitionStyle == ZoneTransitionStyle.INSTANT)
            {
              card.EnableTransitioningZones(false);
              card.transform.position = vector3_1;
              card.transform.localEulerAngles = vector3_2;
              card.transform.localScale = vector3_3;
              continue;
            }
            card.EnableTransitioningZones(true);
            string tweenName = ZoneMgr.Get().GetTweenName<ZoneHand>();
            Hashtable args1 = iTween.Hash((object) "scale", (object) vector3_3, (object) "delay", (object) transitionDelay, (object) "time", (object) num3, (object) "easeType", (object) easeType, (object) "name", (object) tweenName);
            iTween.ScaleTo(card.gameObject, args1);
            Hashtable args2 = iTween.Hash((object) "rotation", (object) vector3_2, (object) "delay", (object) transitionDelay, (object) "time", (object) num3, (object) "easeType", (object) easeType, (object) "name", (object) tweenName);
            iTween.RotateTo(card.gameObject, args2);
            Hashtable args3 = iTween.Hash((object) "position", (object) vector3_1, (object) "delay", (object) transitionDelay, (object) "time", (object) num2, (object) "easeType", (object) easeType, (object) "name", (object) tweenName);
            iTween.MoveTo(card.gameObject, args3);
            delaySec = Mathf.Max(delaySec, transitionDelay + num2, transitionDelay + num3);
            continue;
          case ZoneTransitionStyle.SLOW:
            easeType = iTween.EaseType.easeInExpo;
            num3 = num2;
            goto default;
          case ZoneTransitionStyle.VERY_SLOW:
            easeType = iTween.EaseType.easeInOutCubic;
            num3 = 1f;
            num2 = 1f;
            goto default;
          default:
            card.GetActor().TurnOnCollider();
            goto case ZoneTransitionStyle.NORMAL;
        }
      }
    }
    this.m_lastMousedOverCard = cardMousedOver;
    if (num1 > 0)
      this.StartFinishLayoutTimer(delaySec);
    else
      this.UpdateLayoutFinished();
  }

  private bool ShouldCheckTapWhenClickingMiniHand() => !InputManager.Get().HasPlayFromMiniHandEnabled() || !(bool) UniversalInputManager.UsePhoneUI || this.m_handEnlarged || !InputManager.Get().WaitingForTouchDelay();

  private void CreateCardStandIn(Card card)
  {
    Actor actor = card.GetActor();
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null && (UnityEngine.Object) actor.GetMeshRenderer() != (UnityEngine.Object) null)
      actor.GetMeshRenderer().gameObject.layer = 0;
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Collider_Standin.prefab:06f88b48f6884bf4cafbd6696a28ede4", AssetLoadingOptions.IgnorePrefabPosition);
    gameObject.transform.localEulerAngles = this.GetCardRotation(card);
    gameObject.transform.position = this.GetCardPosition(card);
    gameObject.transform.localScale = this.GetCardScale();
    CardStandIn component = gameObject.GetComponent<CardStandIn>();
    component.linkedCard = card;
    this.standIns.Add(component);
    if (component.linkedCard.CardStandInIsInteractive())
      return;
    component.DisableStandIn();
  }

  private CardStandIn GetStandIn(Card card)
  {
    if (this.standIns == null)
      return (CardStandIn) null;
    foreach (CardStandIn standIn in this.standIns)
    {
      if (!((UnityEngine.Object) standIn == (UnityEngine.Object) null) && (UnityEngine.Object) standIn.linkedCard == (UnityEngine.Object) card)
        return standIn;
    }
    return (CardStandIn) null;
  }

  public void MakeStandInInteractive(Card card)
  {
    if ((UnityEngine.Object) this.GetStandIn(card) == (UnityEngine.Object) null)
      return;
    this.GetStandIn(card).EnableStandIn();
  }

  private void BlowUpOldStandins()
  {
    if (this.standIns == null)
    {
      this.standIns = new List<CardStandIn>();
    }
    else
    {
      foreach (CardStandIn standIn in this.standIns)
      {
        if (!((UnityEngine.Object) standIn == (UnityEngine.Object) null))
          UnityEngine.Object.Destroy((UnityEngine.Object) standIn.gameObject);
      }
      this.standIns = new List<CardStandIn>();
    }
  }

  public int GetCardSlot(Card card)
  {
    int cardSlot = this.m_cards.IndexOf(card);
    if (this.m_reservedSlot != -1 && cardSlot >= this.m_reservedSlot)
      ++cardSlot;
    return cardSlot;
  }

  public Vector3 GetCardPosition(Card card) => this.GetCardPosition(card, -1);

  public Vector3 GetCardPosition(Card card, int overrideCardCount) => this.GetCardPosition(this.GetCardSlot(card), overrideCardCount);

  public Vector3 GetCardPosition(int slot, int overrideCardCount)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    int cardCount = this.GetVisualCardCount();
    if (overrideCardCount >= 0)
      cardCount = overrideCardCount;
    int num4 = this.IsHandScrunched(cardCount) ? 1 : 0;
    if (!this.enemyHand)
      cardCount -= TurnStartManager.Get().GetNumCardsToDraw();
    if (num4 != 0 && cardCount > 1)
    {
      num3 = 1f;
      float num5 = 40f;
      if (!this.enemyHand)
        num5 += (float) cardCount;
      num1 = num5 / (float) (cardCount - 1);
      num2 = (float) (-(double) num5 / 2.0);
    }
    float num6 = 0.0f;
    float f = !this.enemyHand ? num6 + (num1 * (float) slot + num2) : (float) (0.0 - (double) num1 * (double) slot) - num2;
    float cardSpacing = this.GetCardSpacing(cardCount);
    float num7 = 0.0f;
    if (this.enemyHand && (double) f < 0.0 || !this.enemyHand && (double) f > 0.0)
      num7 = (float) ((double) Mathf.Sin((float) ((double) Mathf.Abs(f) * 3.14159274101257 / 180.0)) * (double) cardSpacing / 2.0);
    float x = this.centerOfHand.x - cardSpacing / 2f * (float) (cardCount - 1 - slot * 2);
    if (this.m_handEnlarged && this.m_targetingMode)
    {
      if (cardCount % 2 > 0)
      {
        if (slot < (cardCount + 1) / 2)
          x -= this.m_heroWidthInHand;
      }
      else if (slot < cardCount / 2)
        x -= this.m_heroWidthInHand / 2f;
      else
        x += this.m_heroWidthInHand / 2f;
    }
    float y = this.centerOfHand.y;
    float z = this.centerOfHand.z;
    if (cardCount > 1)
    {
      if (this.enemyHand)
        z += Mathf.Pow(Mathf.Abs((float) slot + 0.5f - (float) (cardCount / 2)), 2f) / (float) (6 * cardCount) * num3 + num7;
      else
        z = this.centerOfHand.z - Mathf.Pow(Mathf.Abs((float) slot + 0.5f - (float) (cardCount / 2)), 2f) / (float) (6 * cardCount) * num3 - num7;
    }
    if (this.enemyHand && this.m_controller.IsRevealed())
      z -= 0.2f;
    return new Vector3(x, y, z);
  }

  public Vector3 GetCardRotation(Card card) => this.GetCardRotation(card, -1);

  public Vector3 GetCardRotation(Card card, int overrideCardCount) => this.GetCardRotation(this.GetCardSlot(card), overrideCardCount);

  public Vector3 GetCardRotation(int slot, int overrideCardCount)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    int cardCount = this.GetVisualCardCount();
    if (overrideCardCount >= 0)
      cardCount = overrideCardCount;
    int num3 = this.IsHandScrunched(cardCount) ? 1 : 0;
    if (!this.enemyHand)
      cardCount -= TurnStartManager.Get().GetNumCardsToDraw();
    if (num3 != 0 && cardCount > 1)
    {
      float num4 = 40f;
      if (!this.enemyHand)
        num4 += (float) (cardCount * 2);
      num1 = num4 / (float) (cardCount - 1);
      num2 = (float) (-(double) num4 / 2.0);
    }
    float num5 = 0.0f;
    float y = !this.enemyHand ? num5 + (num1 * (float) slot + num2) : (float) (0.0 - (double) num1 * (double) slot) - num2;
    if (this.enemyHand && this.m_controller.IsRevealed())
      y += 180f;
    float z = this.m_flipHandCards ? 534.5f : 354.5f;
    return new Vector3(0.0f, y, z);
  }

  public Vector3 GetCardScale()
  {
    if (this.enemyHand)
      return new Vector3(0.682f, 0.225f, 0.682f);
    return (bool) UniversalInputManager.UsePhoneUI ? this.m_enlargedHandCardScale : new Vector3(0.62f, 0.225f, 0.62f);
  }

  private Vector3 GetMouseOverCardPosition(Card card)
  {
    Vector3 cardPosition = this.GetCardPosition(card);
    bool flag = (bool) UniversalInputManager.UsePhoneUI && !this.m_handEnlarged;
    return new Vector3(cardPosition.x, (float) ((double) this.centerOfHand.y + 1.0 + (flag ? (double) this.m_tinyHandMouseOverYOffset : 0.0)), (float) ((double) this.transform.Find("MouseOverCardHeight").position.z + (double) (float) (MobileOverrideValue<float>) this.m_SelectCardOffsetZ + (flag ? (double) this.m_tinyHandMouseOverZOffset : 0.0)));
  }

  private float GetCardSpacing(int cardCount)
  {
    float cardSpacing = this.GetDefaultCardSpacing();
    double num1 = (double) cardSpacing * (double) cardCount;
    float num2 = this.MaxHandWidth();
    double num3 = (double) num2;
    if (num1 > num3)
      cardSpacing = num2 / (float) cardCount;
    return cardSpacing;
  }

  private float MaxHandWidth()
  {
    float maxWidth = this.m_maxWidth;
    if (this.m_handEnlarged && this.m_targetingMode)
      maxWidth -= this.m_heroWidthInHand;
    return maxWidth;
  }

  protected bool CanAnimateCard(Card card)
  {
    bool flag = this.enemyHand && card.GetPrevZone() is ZonePlay;
    if (card.IsDoNotSort())
    {
      if (flag)
        Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED card.IsDoNotSort()", (object) card);
      return false;
    }
    if (!card.IsActorReady())
    {
      if (flag)
        Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED !card.IsActorReady()", (object) card);
      return false;
    }
    if (this.m_controller.IsFriendlySide() && (bool) (UnityEngine.Object) TurnStartManager.Get() && TurnStartManager.Get().IsCardDrawHandled(card))
      return false;
    if (this.IsCardNotInEnemyHandAnymore(card))
    {
      if (flag)
        Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED IsCardNotInEnemyHandAnymore()", (object) card);
      return false;
    }
    if (!card.HasBeenGrabbedByEnemyActionHandler())
      return true;
    if (flag)
      Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED card.HasBeenGrabbedByEnemyActionHandler()", (object) card);
    return false;
  }

  private bool IsCardNotInEnemyHandAnymore(Card card) => card.GetEntity().GetZone() != TAG_ZONE.HAND && this.enemyHand;

  private void UpdateCenterAndWidth()
  {
    Collider component = this.GetComponent<Collider>();
    this.centerOfHand = component.bounds.center;
    this.m_maxWidth = component.bounds.size.x;
  }

  public void OnCardGrabbed(Card card)
  {
    Entity entity = card.GetEntity();
    if (entity == null)
      return;
    Player controller = entity.GetController();
    if (controller == null)
      return;
    if (InputManager.Get().HasPlayFromMiniHandEnabled())
      card.transform.localEulerAngles = Vector3.zero;
    if ((!controller.HasTag(GAME_TAG.HEALING_DOES_DAMAGE) || !card.CanPlayHealingDoesDamageHint()) && (!controller.HasTag(GAME_TAG.LIFESTEAL_DAMAGES_OPPOSING_HERO) || !card.CanPlayLifestealDoesDamageHint()))
      return;
    Spell actorSpell = card.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
    if (!((UnityEngine.Object) actorSpell != (UnityEngine.Object) null))
      return;
    actorSpell.Reactivate();
  }

  public void OnCardHeld(Card heldCard)
  {
    if ((UnityEngine.Object) heldCard == (UnityEngine.Object) null || heldCard.GetEntity() == null || !heldCard.GetEntity().IsTwinspell())
      return;
    this.OnTwinspellHeld(heldCard);
  }

  private void OnTwinspellHeld(Card heldCard)
  {
    if ((UnityEngine.Object) this.m_twinspellHoldSpellInstance == (UnityEngine.Object) null)
    {
      this.m_twinspellHoldSpellInstance = (TwinspellHoldSpell) SpellManager.Get().GetSpell((Spell) this.m_TwinspellHoldSpell);
      this.m_twinspellHoldSpellInstance.Initialize(heldCard.GetEntity().GetEntityId(), heldCard.GetZonePosition());
    }
    else if (this.m_twinspellHoldSpellInstance.GetOriginalSpellEntityId() != heldCard.GetEntity().GetEntityId() || this.m_twinspellHoldSpellInstance.GetFakeTwinspellZonePosition() != heldCard.GetEntity().GetZonePosition())
      this.m_twinspellHoldSpellInstance.Initialize(heldCard.GetEntity().GetEntityId(), heldCard.GetZonePosition());
    heldCard.GetActor().ToggleForceIdle(true);
    heldCard.UpdateActorState();
    SpellUtils.ActivateBirthIfNecessary((Spell) this.m_twinspellHoldSpellInstance);
  }

  public void OnTwinspellPlayed(Card playedCard)
  {
    if (!playedCard.GetEntity().IsTwinspell())
      return;
    playedCard.GetActor().ToggleForceIdle(false);
    playedCard.UpdateActorState();
    this.ReserveCardSlot(this.GetCardSlot(playedCard));
    this.m_playingTwinspellEntityId = playedCard.GetEntity().GetEntityId();
    if (!((UnityEngine.Object) this.m_twinspellHoldSpellInstance != (UnityEngine.Object) null))
      return;
    this.m_twinspellHoldSpellInstance.ActivateState(SpellStateType.ACTION);
  }

  public void OnTwinspellDropped(Card droppedCard)
  {
    if (!droppedCard.GetEntity().IsTwinspell())
      return;
    this.ActivateTwinspellSpellDeath();
    droppedCard.GetActor().ToggleForceIdle(false);
    droppedCard.UpdateActorState();
  }

  public void ActivateTwinspellSpellDeath()
  {
    if ((UnityEngine.Object) this.m_twinspellHoldSpellInstance != (UnityEngine.Object) null)
      SpellUtils.ActivateDeathIfNecessary((Spell) this.m_twinspellHoldSpellInstance);
    this.m_playingTwinspellEntityId = -1;
  }

  public bool IsTwinspellBeingPlayed(Entity twinspellEntity) => twinspellEntity != null && twinspellEntity.GetEntityId() == this.m_playingTwinspellEntityId;

  private void OnCantPlay(Entity entity, object userData)
  {
    if (!entity.IsControlledByFriendlySidePlayer())
      return;
    if (entity.IsTwinspell())
    {
      this.ActivateTwinspellSpellDeath();
      this.ClearReservedCard();
    }
    if (!entity.IsMinion())
      return;
    Card card = entity.GetCard();
    if (!((UnityEngine.Object) card != (UnityEngine.Object) null))
      return;
    MagneticPlayData magneticPlayData = card.GetMagneticPlayData();
    if (magneticPlayData != null)
      SpellUtils.ActivateDeathIfNecessary((Spell) magneticPlayData.m_beamSpell);
    ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY).OnMagneticDropped(entity.GetCard());
  }

  public override bool AddCard(Card card) => base.AddCard(card);
}
