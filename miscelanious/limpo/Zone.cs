using System;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
  public TAG_ZONE m_ServerTag;
  public Player.Side m_Side;
  public const float TRANSITION_SEC = 1f;
  protected Player m_controller;
  protected List<Card> m_cards = new List<Card>();
  protected bool m_layoutDirty = true;
  protected int m_updatingLayout;
  protected List<Zone.UpdateLayoutCompleteListener> m_completeListeners = new List<Zone.UpdateLayoutCompleteListener>();
  protected int m_inputBlockerCount;
  protected int m_layoutBlockerCount;

  public override string ToString() => string.Format("{1} {0}", (object) this.m_ServerTag, (object) this.m_Side);

  public Player GetController() => this.m_controller;

  public int GetControllerId() => this.m_controller != null ? this.m_controller.GetPlayerId() : 0;

  public void SetController(Player controller) => this.m_controller = controller;

  public List<Card> GetCards() => this.m_cards;

  public int GetCardCount() => this.m_cards.Count;

  public virtual int GetLastSlot() => this.GetCardCount();

  public Card GetFirstCard() => this.m_cards.Count <= 0 ? (Card) null : this.m_cards[0];

  public Card GetLastCard() => this.m_cards.Count <= 0 ? (Card) null : this.m_cards[this.m_cards.Count - 1];

  public Card GetCardAtIndex(int index)
  {
    if (index < 0)
      return (Card) null;
    return index >= this.m_cards.Count ? (Card) null : this.m_cards[index];
  }

  public virtual Card GetCardAtSlot(int slot) => this.GetCardAtIndex(slot - 1);

  public int GetLastPos() => this.m_cards.Count + 1;

  public int FindCardPos(Card card) => 1 + this.m_cards.FindIndex((Predicate<Card>) (currCard => (UnityEngine.Object) currCard == (UnityEngine.Object) card));

  public bool ContainsCard(Card card) => this.FindCardPos(card) > 0;

  public bool IsOnlyCard(Card card) => this.m_cards.Count == 1 && (UnityEngine.Object) this.m_cards[0] == (UnityEngine.Object) card;

  public void DirtyLayout() => this.m_layoutDirty = true;

  public bool IsLayoutDirty() => this.m_layoutDirty;

  public bool IsUpdatingLayout() => this.m_updatingLayout > 0;

  public bool IsInputEnabled() => this.m_inputBlockerCount <= 0;

  public int GetInputBlockerCount() => this.m_inputBlockerCount;

  public void AddInputBlocker() => this.AddInputBlocker(1);

  public void RemoveInputBlocker() => this.AddInputBlocker(-1);

  public void BlockInput(bool block) => this.AddInputBlocker(block ? 1 : -1);

  public void AddInputBlocker(int count)
  {
    int inputBlockerCount = this.m_inputBlockerCount;
    this.m_inputBlockerCount += count;
    if (inputBlockerCount == this.m_inputBlockerCount || inputBlockerCount * this.m_inputBlockerCount != 0)
      return;
    this.UpdateInput();
  }

  public bool IsBlockingLayout() => this.m_layoutBlockerCount > 0;

  public int GetLayoutBlockerCount() => this.m_layoutBlockerCount;

  public void AddLayoutBlocker() => ++this.m_layoutBlockerCount;

  public void RemoveLayoutBlocker() => --this.m_layoutBlockerCount;

  public bool AddUpdateLayoutCompleteCallback(Zone.UpdateLayoutCompleteCallback callback) => this.AddUpdateLayoutCompleteCallback(callback, (object) null);

  public bool AddUpdateLayoutCompleteCallback(
    Zone.UpdateLayoutCompleteCallback callback,
    object userData)
  {
    Zone.UpdateLayoutCompleteListener completeListener = new Zone.UpdateLayoutCompleteListener();
    completeListener.SetCallback(callback);
    completeListener.SetUserData(userData);
    if (this.m_completeListeners.Contains(completeListener))
      return false;
    this.m_completeListeners.Add(completeListener);
    return true;
  }

  public bool RemoveUpdateLayoutCompleteCallback(Zone.UpdateLayoutCompleteCallback callback) => this.RemoveUpdateLayoutCompleteCallback(callback, (object) null);

  public bool RemoveUpdateLayoutCompleteCallback(
    Zone.UpdateLayoutCompleteCallback callback,
    object userData)
  {
    Zone.UpdateLayoutCompleteListener completeListener = new Zone.UpdateLayoutCompleteListener();
    completeListener.SetCallback(callback);
    completeListener.SetUserData(userData);
    return this.m_completeListeners.Remove(completeListener);
  }

  public virtual bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return this.m_ServerTag == zoneTag && (this.m_controller == null || this.m_controller.GetPlayerId() == controllerId) && cardType != TAG_CARDTYPE.ENCHANTMENT;
  }

  public virtual bool AddCard(Card card)
  {
    this.m_cards.Add(card);
    this.DirtyLayout();
    return true;
  }

  public virtual bool InsertCard(int index, Card card)
  {
    this.m_cards.Insert(index, card);
    this.DirtyLayout();
    return true;
  }

  public virtual int RemoveCard(Card card)
  {
    for (int index = 0; index < this.m_cards.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_cards[index] == (UnityEngine.Object) card)
      {
        this.m_cards.RemoveAt(index);
        this.DirtyLayout();
        return index;
      }
    }
    if (!GameState.Get().EntityRemovedFromGame(card.GetEntity().GetEntityId()))
      Debug.LogWarning((object) string.Format("{0}.RemoveCard() - FAILED: {1} tried to remove {2}", (object) this, (object) this.m_controller, (object) card));
    return -1;
  }

  public virtual void Reset()
  {
    this.m_cards.Clear();
    this.m_inputBlockerCount = 0;
    this.UpdateInput();
  }

  public virtual Transform GetZoneTransformForCard(Card card) => this.transform;

  public virtual void UpdateLayout()
  {
    if (this.m_cards.Count == 0)
      this.UpdateLayoutFinished();
    else if (GameState.Get().IsMulliganManagerActive())
    {
      this.UpdateLayoutFinished();
    }
    else
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
          if (!card.IsDoNotSort())
          {
            card.ShowCard();
            card.EnableTransitioningZones(true);
            Transform transformForCard = this.GetZoneTransformForCard(card);
            iTween.MoveTo(card.gameObject, transformForCard.position, 1f);
            iTween.RotateTo(card.gameObject, transformForCard.localEulerAngles, 1f);
            iTween.ScaleTo(card.gameObject, transformForCard.localScale, 1f);
          }
        }
        this.StartFinishLayoutTimer(1f);
      }
    }
  }

  public static int CardSortComparison(Card card1, Card card2)
  {
    int zonePosition1 = card1.GetZonePosition();
    int zonePosition2 = card2.GetZonePosition();
    return zonePosition1 != zonePosition2 ? zonePosition1 - zonePosition2 : card1.GetEntity().GetZonePosition() - card2.GetEntity().GetZonePosition();
  }

  public virtual void OnHealingDoesDamageEntityMousedOver()
  {
    if (TargetReticleManager.Get().IsActive())
      return;
    foreach (Card card in this.m_cards)
    {
      if (card.CanPlayHealingDoesDamageHint())
      {
        Spell actorSpell1 = card.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
        if ((UnityEngine.Object) actorSpell1 != (UnityEngine.Object) null)
          actorSpell1.Reactivate();
        Spell actorSpell2 = card.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_IDLE);
        if ((UnityEngine.Object) actorSpell2 != (UnityEngine.Object) null)
          actorSpell2.ActivateState(SpellStateType.BIRTH);
      }
    }
  }

  public virtual void OnHealingDoesDamageEntityMousedOut()
  {
    foreach (Card card in this.m_cards)
    {
      if (!card.GetEntity().HasTag(GAME_TAG.HEALING_DOES_DAMAGE_HINT))
      {
        Spell actorSpell = card.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_IDLE);
        if (!((UnityEngine.Object) actorSpell == (UnityEngine.Object) null) && actorSpell.IsActive())
          actorSpell.ActivateState(SpellStateType.DEATH);
      }
    }
  }

  public virtual void OnHealingDoesDamageEntityEnteredPlay()
  {
    foreach (Card card in this.m_cards)
    {
      if (card.CanPlayHealingDoesDamageHint())
      {
        Spell actorSpell = card.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          actorSpell.Reactivate();
      }
    }
  }

  public virtual void OnLifestealDoesDamageEntityMousedOver()
  {
    if (TargetReticleManager.Get().IsActive())
      return;
    foreach (Card card in this.m_cards)
    {
      if (card.CanPlayLifestealDoesDamageHint())
      {
        Spell actorSpell1 = card.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
        if ((UnityEngine.Object) actorSpell1 != (UnityEngine.Object) null)
          actorSpell1.Reactivate();
        Spell actorSpell2 = card.GetActorSpell(SpellType.LIFESTEAL_DOES_DAMAGE_HINT_IDLE);
        if ((UnityEngine.Object) actorSpell2 != (UnityEngine.Object) null)
          actorSpell2.ActivateState(SpellStateType.BIRTH);
      }
    }
  }

  public virtual void OnLifestealDoesDamageEntityMousedOut()
  {
    foreach (Card card in this.m_cards)
    {
      if (!card.GetEntity().HasTag(GAME_TAG.LIFESTEAL_DOES_DAMAGE_HINT))
      {
        Spell actorSpell = card.GetActorSpell(SpellType.LIFESTEAL_DOES_DAMAGE_HINT_IDLE);
        if (!((UnityEngine.Object) actorSpell == (UnityEngine.Object) null) && actorSpell.IsActive())
          actorSpell.ActivateState(SpellStateType.DEATH);
      }
    }
  }

  public virtual void OnLifestealDoesDamageEntityEnteredPlay()
  {
    foreach (Card card in this.m_cards)
    {
      if (card.CanPlayLifestealDoesDamageHint())
      {
        Spell actorSpell = card.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          actorSpell.Reactivate();
      }
    }
  }

  public virtual void OnSpellPowerEntityEnteredPlay(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
  }

  public virtual void OnSpellPowerEntityMousedOver(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
  }

  public virtual void OnSpellPowerEntityMousedOut(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
  }

  public virtual void OnDiedLastCombatMousedOver()
  {
  }

  public virtual void OnDiedLastCombatMousedOut()
  {
  }

  protected void UpdateInput()
  {
    bool enabled = this.IsInputEnabled();
    foreach (Card card in this.m_cards)
    {
      Actor actor = card.GetActor();
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
      {
        actor.ToggleForceIdle(!enabled);
        actor.ToggleCollider(enabled);
        card.UpdateActorState();
      }
    }
    Card mousedOverCard = InputManager.Get().GetMousedOverCard();
    if (!enabled || !this.m_cards.Contains(mousedOverCard))
      return;
    mousedOverCard.UpdateProposedManaUsage();
  }

  protected void StartFinishLayoutTimer(float delaySec)
  {
    if ((double) delaySec <= (double) Mathf.Epsilon)
      this.UpdateLayoutFinished();
    else if ((UnityEngine.Object) this.m_cards.Find((Predicate<Card>) (card => card.IsTransitioningZones())) == (UnityEngine.Object) null)
      this.UpdateLayoutFinished();
    else
      iTween.Timer(this.gameObject, iTween.Hash((object) "time", (object) delaySec, (object) "oncomplete", (object) "UpdateLayoutFinished", (object) "oncompletetarget", (object) this.gameObject));
  }

  protected void UpdateLayoutFinished()
  {
    for (int index = 0; index < this.m_cards.Count; ++index)
      this.m_cards[index].EnableTransitioningZones(false);
    --this.m_updatingLayout;
    this.m_layoutDirty = false;
    this.FireUpdateLayoutCompleteCallbacks();
  }

  protected void FireUpdateLayoutCompleteCallbacks()
  {
    if (this.m_completeListeners.Count == 0)
      return;
    Zone.UpdateLayoutCompleteListener[] array = this.m_completeListeners.ToArray();
    this.m_completeListeners.Clear();
    for (int index = 0; index < array.Length; ++index)
      array[index].Fire(this);
  }

  public delegate void UpdateLayoutCompleteCallback(Zone zone, object userData);

  protected class UpdateLayoutCompleteListener : EventListener<Zone.UpdateLayoutCompleteCallback>
  {
    public void Fire(Zone zone) => this.m_callback(zone, this.m_userData);
  }
}
