using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePlay : Zone
{
  public int m_MaxSlots = 7;
  public float m_BigCardCenterOffset = 2.5f;
  public MagneticBeamSpell m_MagneticBeamSpell;
  private const float DEFAULT_TRANSITION_TIME = 1f;
  private const float PHONE_CARD_SCALE = 1.15f;
  private float[] PHONE_WIDTH_MODIFIERS = new float[8]
  {
    0.25f,
    0.25f,
    0.25f,
    0.25f,
    0.22f,
    0.19f,
    0.15f,
    0.1f
  };
  private int m_mousedOverSlot = -1;
  private int m_lettuceAbilityReservedSlot = -1;
  private float m_slotWidth;
  private float m_transitionTime = 1f;
  private float m_baseTransitionTime = 1f;
  private MagneticBeamSpell m_magneticBeamSpellInstance;
  private Card m_previousHeldCard;
  protected Vector3 m_defaultScale;

  private void Awake() => this.m_slotWidth = this.GetComponent<Collider>().bounds.size.x / (float) this.m_MaxSlots;

  public float GetTransitionTime() => this.m_transitionTime;

  public void SetTransitionTime(float transitionTime) => this.m_transitionTime = transitionTime;

  public void ResetTransitionTime() => this.m_transitionTime = this.m_baseTransitionTime;

  public void OverrideBaseTransitionTime(float newTransitionTime) => this.m_baseTransitionTime = newTransitionTime;

  public void SortWithSpotForHeldCard(int slot)
  {
    this.m_mousedOverSlot = slot;
    this.UpdateLayout();
  }

  public override Card GetCardAtSlot(int slot)
  {
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      if (this.GetSlotOfCardAtIndex(index) == slot)
        return this.m_cards[index];
    }
    return (Card) null;
  }

  public void SortWithSpotForLettuceAbilityCard(int slot)
  {
    this.m_lettuceAbilityReservedSlot = slot;
    this.UpdateLayout();
  }

  public MagneticBeamSpell GetMagneticBeamSpell() => this.m_MagneticBeamSpell;

  public void OnMagneticHeld(Card heldCard)
  {
    if (this.m_mousedOverSlot < 1 || !heldCard.GetEntity().HasTag(GAME_TAG.MODULAR) || heldCard.GetZone() is ZonePlay)
      return;
    if ((UnityEngine.Object) this.m_magneticBeamSpellInstance == (UnityEngine.Object) null)
      this.m_magneticBeamSpellInstance = (MagneticBeamSpell) SpellManager.Get().GetSpell((Spell) this.m_MagneticBeamSpell);
    Card card1 = (Card) null;
    List<Card> cardList = new List<Card>();
    int mousedOverSlot = this.m_mousedOverSlot;
    int num = ZoneMgr.Get().PredictZonePosition((Zone) this, mousedOverSlot);
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      Card card2 = this.m_cards[index];
      Entity entity = card2.GetEntity();
      if (entity.HasTag(GAME_TAG.UNTOUCHABLE) || !entity.HasRace(TAG_RACE.MECHANICAL) || entity.GetRealTimeZone() != TAG_ZONE.PLAY)
      {
        SpellUtils.ActivateDeathIfNecessary(card2.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card2.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card2.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT_DIMMED));
      }
      else
      {
        cardList.Add(card2);
        if (entity.GetRealTimeZonePosition() == num)
          card1 = card2;
      }
    }
    heldCard.GetActor().ToggleForceIdle(true);
    heldCard.UpdateActorState();
    foreach (Card card3 in cardList)
    {
      card3.GetActor().ToggleForceIdle(true);
      card3.UpdateActorState();
      if ((UnityEngine.Object) card1 == (UnityEngine.Object) card3)
      {
        SpellUtils.ActivateBirthIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT_DIMMED));
      }
      else if ((UnityEngine.Object) card1 == (UnityEngine.Object) null)
      {
        SpellUtils.ActivateBirthIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT_DIMMED));
      }
      else
      {
        SpellUtils.ActivateBirthIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT_DIMMED));
        SpellUtils.ActivateDeathIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card3.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
      }
    }
    if (cardList.Count > 0)
    {
      if ((UnityEngine.Object) card1 != (UnityEngine.Object) null)
      {
        this.m_magneticBeamSpellInstance.SetSource(heldCard.gameObject);
        if ((UnityEngine.Object) this.m_magneticBeamSpellInstance.GetTarget() != (UnityEngine.Object) card1.gameObject)
        {
          this.m_magneticBeamSpellInstance.RemoveAllTargets();
          this.m_magneticBeamSpellInstance.AddTarget(card1.gameObject);
        }
        SpellUtils.ActivateBirthIfNecessary((Spell) this.m_magneticBeamSpellInstance);
        SpellUtils.ActivateBirthIfNecessary(heldCard.GetActorSpell(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
        SpellUtils.ActivateDeathIfNecessary(heldCard.GetActorSpell(SpellType.MAGNETIC_HAND_UNLINKED));
      }
      else
      {
        SpellUtils.ActivateDeathIfNecessary((Spell) this.m_magneticBeamSpellInstance);
        SpellUtils.ActivateBirthIfNecessary(heldCard.GetActorSpell(SpellType.MAGNETIC_HAND_UNLINKED));
        SpellUtils.ActivateDeathIfNecessary(heldCard.GetActorSpell(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
      }
    }
    else
    {
      SpellUtils.ActivateDeathIfNecessary((Spell) this.m_magneticBeamSpellInstance);
      SpellUtils.ActivateDeathIfNecessary(heldCard.GetActorSpell(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
      SpellUtils.ActivateDeathIfNecessary(heldCard.GetActorSpell(SpellType.MAGNETIC_HAND_UNLINKED));
    }
  }

  public void OnMagneticPlay(Card playedCard, int zonePos)
  {
    if (!playedCard.GetEntity().HasTag(GAME_TAG.MODULAR))
    {
      if (!((UnityEngine.Object) this.m_magneticBeamSpellInstance != (UnityEngine.Object) null))
        return;
      SpellUtils.ActivateDeathIfNecessary((Spell) this.m_magneticBeamSpellInstance);
    }
    else
    {
      Card card1 = (Card) null;
      for (int index = 0; index < this.m_cards.Count; ++index)
      {
        Card card2 = this.m_cards[index];
        Entity entity = card2.GetEntity();
        if (!entity.HasTag(GAME_TAG.UNTOUCHABLE) && entity.HasRace(TAG_RACE.MECHANICAL) && entity.GetRealTimeZone() == TAG_ZONE.PLAY)
        {
          if (card2.GetEntity().GetRealTimeZonePosition() == zonePos)
          {
            card1 = card2;
          }
          else
          {
            card2.GetActor().ToggleForceIdle(false);
            card2.UpdateActorState();
            SpellUtils.ActivateDeathIfNecessary(card2.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
            SpellUtils.ActivateDeathIfNecessary(card2.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT));
            SpellUtils.ActivateDeathIfNecessary(card2.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT_DIMMED));
          }
        }
      }
      if ((UnityEngine.Object) card1 != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_magneticBeamSpellInstance == (UnityEngine.Object) null)
          this.m_magneticBeamSpellInstance = (MagneticBeamSpell) SpellManager.Get().GetSpell((Spell) this.m_MagneticBeamSpell);
        this.m_magneticBeamSpellInstance.SetSource(playedCard.gameObject);
        this.m_magneticBeamSpellInstance.RemoveAllTargets();
        this.m_magneticBeamSpellInstance.AddTarget(card1.gameObject);
        playedCard.SetMagneticPlayData(new MagneticPlayData()
        {
          m_playedCard = playedCard,
          m_targetMech = card1,
          m_beamSpell = this.m_magneticBeamSpellInstance
        });
        card1.SetIsMagneticTarget(true);
        playedCard.GetActor().ToggleForceIdle(true);
        playedCard.UpdateActorState();
        card1.GetActor().ToggleForceIdle(true);
        card1.UpdateActorState();
        this.m_magneticBeamSpellInstance = (MagneticBeamSpell) null;
        SpellUtils.ActivateBirthIfNecessary(playedCard.GetActorSpell(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
        SpellUtils.ActivateBirthIfNecessary(card1.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(playedCard.GetActorSpell(SpellType.MAGNETIC_HAND_UNLINKED));
        SpellUtils.ActivateDeathIfNecessary(card1.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card1.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT_DIMMED));
      }
      else
      {
        playedCard.GetActor().ToggleForceIdle(false);
        playedCard.UpdateActorState();
        SpellUtils.ActivateDeathIfNecessary((Spell) this.m_magneticBeamSpellInstance);
        SpellUtils.ActivateDeathIfNecessary(playedCard.GetActorSpell(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
        SpellUtils.ActivateDeathIfNecessary(playedCard.GetActorSpell(SpellType.MAGNETIC_HAND_UNLINKED));
      }
    }
  }

  public void OnMagneticDropped(Card droppedCard)
  {
    if (!droppedCard.GetEntity().HasTag(GAME_TAG.MODULAR))
      return;
    SpellUtils.ActivateDeathIfNecessary((Spell) this.m_magneticBeamSpellInstance);
    SpellUtils.ActivateDeathIfNecessary(droppedCard.GetActorSpell(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
    SpellUtils.ActivateDeathIfNecessary(droppedCard.GetActorSpell(SpellType.MAGNETIC_HAND_UNLINKED));
    droppedCard.GetActor().ToggleForceIdle(false);
    droppedCard.UpdateActorState();
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      Card card = this.m_cards[index];
      Entity entity = card.GetEntity();
      if (!entity.HasTag(GAME_TAG.UNTOUCHABLE) && entity.HasRace(TAG_RACE.MECHANICAL))
      {
        card.GetActor().ToggleForceIdle(false);
        card.UpdateActorState();
        SpellUtils.ActivateDeathIfNecessary(card.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT));
        SpellUtils.ActivateDeathIfNecessary(card.GetActorSpell(SpellType.MAGNETIC_PLAY_UNLINKED_LEFT_DIMMED));
      }
    }
  }

  public override void OnDiedLastCombatMousedOver()
  {
    foreach (Card card in this.m_cards)
    {
      Actor actor = card.GetActor();
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
      {
        Entity entity = actor.GetEntity();
        if (entity != null && entity.HasTag(GAME_TAG.BACON_DIED_LAST_COMBAT))
          actor.SetActorState(ActorStateType.CARD_VALID_TARGET);
      }
    }
  }

  public override void OnDiedLastCombatMousedOut()
  {
    foreach (Card card in this.m_cards)
    {
      Actor actor = card.GetActor();
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
      {
        Entity entity = actor.GetEntity();
        if (entity != null && entity.HasTag(GAME_TAG.BACON_DIED_LAST_COMBAT))
          actor.SetActorState(ActorStateType.CARD_IDLE);
      }
    }
  }

  public int GetSlotMousedOver() => this.m_mousedOverSlot;

  public bool HasMousedOverSlotChanged(int slot) => this.m_mousedOverSlot != slot;

  public float GetSlotWidth()
  {
    this.m_slotWidth = this.GetComponent<Collider>().bounds.size.x / (float) this.m_MaxSlots;
    int count = this.m_cards.Count;
    if (this.m_mousedOverSlot >= 1)
      ++count;
    if (this.m_lettuceAbilityReservedSlot >= 0)
      ++count;
    int index = Mathf.Clamp(count, 0, this.m_MaxSlots);
    float num = 1f;
    if ((bool) UniversalInputManager.UsePhoneUI)
      num += this.PHONE_WIDTH_MODIFIERS[index];
    ZonePlay.PlayZoneSizeOverride zoneSizeOverride = GameState.Get().GetGameEntity().GetPlayZoneSizeOverride();
    if (zoneSizeOverride != null)
      num = zoneSizeOverride.m_slotWidthModifier;
    return this.m_slotWidth * num;
  }

  public void UnhideCardZzzEffects()
  {
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      Card card = this.m_cards[index];
      if (card.GetEntity().IsAsleep())
        SpellUtils.ActivateBirthIfNecessary(card.GetActorSpell(SpellType.Zzz));
    }
  }

  public void HideCardZzzEffects()
  {
    for (int index = 0; index < this.m_cards.Count; ++index)
      SpellUtils.ActivateDeathIfNecessary(this.m_cards[index].GetActorSpell(SpellType.Zzz));
  }

  public Vector3 GetCardPosition(Card card) => this.GetCardPosition(this.m_cards.FindIndex((Predicate<Card>) (currCard => (UnityEngine.Object) currCard == (UnityEngine.Object) card)));

  public override int GetLastSlot() => this.m_cards.Count;

  public int GetSlotOfCardAtIndex(int index)
  {
    if (index < 0 || index >= this.m_cards.Count)
      return -1;
    Entity entity = this.m_cards[index].GetEntity();
    if (entity == null)
      return -1;
    entity.GetEntityId();
    int slotOfCardAtIndex = 1;
    for (int index1 = 0; index1 <= index; ++index1)
    {
      if (index1 == index)
        return slotOfCardAtIndex;
      if (this.m_cards[index1].GetEntity() != null)
        ++slotOfCardAtIndex;
    }
    return -1;
  }

  public Vector3 GetCardPosition(int index)
  {
    if (index < 0)
      return this.transform.position;
    int lastSlot = this.GetLastSlot();
    if (this.m_mousedOverSlot >= 0)
      ++lastSlot;
    if (this.m_lettuceAbilityReservedSlot >= 0)
      ++lastSlot;
    Vector3 center = this.GetComponent<Collider>().bounds.center;
    float num1 = 0.5f * this.GetSlotWidth();
    float num2 = (float) lastSlot * num1;
    float num3 = center.x - num2 + num1;
    float num4 = 0.0f;
    int slotOfCardAtIndex = this.GetSlotOfCardAtIndex(index);
    if (this.m_mousedOverSlot >= 0 && this.m_mousedOverSlot <= slotOfCardAtIndex)
      ++num4;
    else if (this.m_lettuceAbilityReservedSlot >= 0 && index >= this.m_lettuceAbilityReservedSlot)
      ++num4;
    int num5 = 0;
    int num6 = 0;
    for (int index1 = 0; index1 < index; ++index1)
    {
      ++num6;
      if (!this.CanAnimateCard(this.m_cards[index1]))
        ++num5;
    }
    float num7 = (float) num6 + num4 - (float) num5;
    return new Vector3(num3 + num7 * this.GetSlotWidth(), center.y, center.z);
  }

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return base.CanAcceptTags(controllerId, zoneTag, cardType, entity) && (cardType == TAG_CARDTYPE.MINION || cardType == TAG_CARDTYPE.LOCATION);
  }

  public override void UpdateLayout()
  {
    ++this.m_updatingLayout;
    if (this.IsBlockingLayout())
    {
      this.UpdateLayoutFinished();
    }
    else
    {
      if (!GameMgr.Get().IsMercenaries())
        this.UpdatePlayZoneScale();
      if ((UnityEngine.Object) InputManager.Get() != (UnityEngine.Object) null && (UnityEngine.Object) InputManager.Get().GetHeldCard() == (UnityEngine.Object) null)
        this.m_mousedOverSlot = -1;
      if (ZoneMgr.Get().GetLettuceAbilitiesSourceEntity() == null)
        this.m_lettuceAbilityReservedSlot = -1;
      int num1 = 0;
      this.m_cards.Sort(new Comparison<Card>(Zone.CardSortComparison));
      float num2 = 0.0f;
      for (int index = 0; index < this.m_cards.Count; ++index)
      {
        Card card = this.m_cards[index];
        if (!((UnityEngine.Object) card == (UnityEngine.Object) null) && this.CanAnimateCard(card))
        {
          string tweenName = ZoneMgr.Get().GetTweenName<ZonePlay>();
          if (this.m_Side == Player.Side.OPPOSING)
            iTween.StopOthersByName(card.gameObject, tweenName);
          Vector3 vector3 = this.transform.localScale;
          if ((bool) UniversalInputManager.UsePhoneUI)
            vector3 *= 1.15f;
          ZonePlay.PlayZoneSizeOverride zoneSizeOverride = GameState.Get().GetGameEntity().GetPlayZoneSizeOverride();
          if (zoneSizeOverride != null)
            vector3 = this.transform.localScale * zoneSizeOverride.m_scale;
          Vector3 cardPosition = this.GetCardPosition(index);
          float transitionDelay = card.GetTransitionDelay();
          card.SetTransitionDelay(0.0f);
          int transitionStyle = (int) card.GetTransitionStyle();
          card.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
          if (transitionStyle == 3)
          {
            card.EnableTransitioningZones(false);
            card.transform.position = cardPosition;
            card.transform.rotation = this.transform.rotation;
            card.transform.localScale = vector3;
          }
          else
          {
            card.EnableTransitioningZones(true);
            ++num1;
            Hashtable args1 = iTween.Hash((object) "scale", (object) vector3, (object) "delay", (object) transitionDelay, (object) "time", (object) this.m_transitionTime, (object) "name", (object) tweenName);
            iTween.ScaleTo(card.gameObject, args1);
            Hashtable args2 = iTween.Hash((object) "rotation", (object) this.transform.eulerAngles, (object) "delay", (object) transitionDelay, (object) "time", (object) this.m_transitionTime, (object) "name", (object) tweenName);
            iTween.RotateTo(card.gameObject, args2);
            Hashtable args3 = iTween.Hash((object) "position", (object) cardPosition, (object) "delay", (object) transitionDelay, (object) "time", (object) this.m_transitionTime, (object) "name", (object) tweenName);
            iTween.MoveTo(card.gameObject, args3);
            num2 = Mathf.Max(num2, transitionDelay + this.m_transitionTime);
          }
        }
      }
      if (num1 > 0)
        this.StartFinishLayoutTimer(num2);
      else
        this.UpdateLayoutFinished();
    }
  }

  private bool DoesCardNeedSpaceOnBoard(Card card)
  {
    if ((UnityEngine.Object) card != (UnityEngine.Object) null && !(card.GetZone() is ZonePlay))
    {
      Entity entity = card.GetEntity();
      if (entity != null && (entity.IsLocation() || entity.IsMinion()))
        return true;
    }
    return false;
  }

  private void UpdatePlayZoneScale()
  {
    ZonePlay battlefieldZone1 = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone();
    ZonePlay battlefieldZone2 = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone();
    if (battlefieldZone1.m_defaultScale == Vector3.zero)
      battlefieldZone1.m_defaultScale = battlefieldZone1.gameObject.transform.localScale;
    if (battlefieldZone2.m_defaultScale == Vector3.zero)
      battlefieldZone2.m_defaultScale = battlefieldZone2.gameObject.transform.localScale;
    int b1 = GameState.Get().GetGameEntity().GetTag(GAME_TAG.MAX_SLOTS_PER_PLAYER_OVERRIDE);
    if (b1 == 0)
      b1 = this.m_MaxSlots;
    int num1 = 0;
    if (this.DoesCardNeedSpaceOnBoard(InputManager.Get().GetHeldCard()))
      num1 = 1;
    int b2 = Mathf.Max(battlefieldZone1.m_cards.Count + num1, b1);
    int num2 = Mathf.Max(battlefieldZone2.m_cards.Count, b2);
    battlefieldZone1.transform.localScale = (float) this.m_MaxSlots / (float) num2 * this.m_defaultScale;
    battlefieldZone2.transform.localScale = (float) this.m_MaxSlots / (float) num2 * this.m_defaultScale;
  }

  protected bool CanAnimateCard(Card card) => !card.IsDoNotSort();

  public class PlayZoneSizeOverride
  {
    public float m_scale;
    public float m_slotWidthModifier;
  }
}
