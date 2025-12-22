using Blizzard.T5.Core;
using PegasusGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class TriggerSpellController : SpellController
{
  public List<AuxiliaryTriggerSpellEntry> m_AuxiliaryTriggerSpells = new List<AuxiliaryTriggerSpellEntry>();
  private Map<int, Spell> m_triggerSpellByEntityId = new Map<int, Spell>();
  private List<CardSoundSpell> m_triggerSoundSpells = new List<CardSoundSpell>();
  private Map<int, Spell> m_actorTriggerSpellByEntityId = new Map<int, Spell>();
  private Map<int, Spell> m_auxiliaryTriggerSpellByEntityId = new Map<int, Spell>();
  private static readonly float BAUBLE_WAIT_TIME_SEC = 1f;
  private int m_cardEffectsBlockingFinish;
  private int m_cardEffectsBlockingTaskListFinish;
  private int m_actorEffectsBlockingFinish;
  private bool m_waitingForBauble;
  private bool m_baubleBlockedFinish;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    if (!this.HasSourceCard(taskList))
      return false;
    List<Entity> sourceEntities = taskList.GetSourceEntities();
    List<Card> cards = new List<Card>();
    foreach (Entity entity in sourceEntities)
    {
      if (entity != null && (UnityEngine.Object) entity.GetCard() != (UnityEngine.Object) null)
        cards.Add(entity.GetCard());
    }
    GameState gameState = GameState.Get();
    foreach (Card card1 in cards)
    {
      Card card2 = card1;
      Entity entity1 = card1.GetEntity();
      int entityId = entity1.GetEntityId();
      if (this.CanPlayActorTriggerSpell(entity1))
        this.m_actorTriggerSpellByEntityId.Add(entityId, this.GetActorTriggerSpell(entity1));
      CardEffect effect = this.InitEffect(card1);
      if (effect != null && this.CanPlayTriggerSpell(taskList))
      {
        this.InitTriggerSpell(effect, card1);
        this.InitTriggerSounds(effect, card1);
      }
      if (entity1.IsEnchantment())
      {
        Entity entity2 = gameState.GetEntity(entity1.GetAttached());
        if (entity2 != null)
          card2 = entity2.GetCard();
      }
      if ((UnityEngine.Object) card2 != (UnityEngine.Object) null)
      {
        Spell auxiliaryTriggerSpell = this.GetAuxiliaryTriggerSpell();
        if ((UnityEngine.Object) auxiliaryTriggerSpell != (UnityEngine.Object) null)
        {
          this.m_auxiliaryTriggerSpellByEntityId.Add(entityId, auxiliaryTriggerSpell);
          auxiliaryTriggerSpell.SetSource(card2.gameObject);
          if (!auxiliaryTriggerSpell.AttachPowerTaskList(this.m_taskList))
          {
            Log.Power.Print("{0}.AddPowerSourceAndTargets() - FAILED to add targets to spell for {1}", (object) this, (object) this.m_auxiliaryTriggerSpellByEntityId);
            this.m_auxiliaryTriggerSpellByEntityId.Remove(entityId);
          }
        }
      }
    }
    if (this.m_triggerSpellByEntityId.Count == 0 && this.m_triggerSoundSpells.Count == 0 && this.m_actorTriggerSpellByEntityId.Count == 0 && this.m_auxiliaryTriggerSpellByEntityId.Count == 0)
    {
      this.Reset();
      return TurnStartManager.Get().IsCardDrawHandled(cards.Count > 0 ? cards[0] : (Card) null) || TurnStartManager.Get().IsCardDrawHandled(taskList.GetStartDrawMetaDataCard());
    }
    this.SetSource(cards);
    return true;
  }

  protected override bool HasSourceCard(PowerTaskList taskList)
  {
    List<Entity> sourceEntities = taskList.GetSourceEntities();
    if (sourceEntities == null || sourceEntities.Count == 0)
      return false;
    if (this.GetCardsWithActorTrigger(taskList).Count != 0)
      return true;
    Card drawMetaDataCard = taskList.GetStartDrawMetaDataCard();
    return (UnityEngine.Object) drawMetaDataCard != (UnityEngine.Object) null && TurnStartManager.Get().IsCardDrawHandled(drawMetaDataCard);
  }

  protected override void OnProcessTaskList() => this.StartCoroutine(this.OnProcessTaskListImpl());

  private IEnumerator OnProcessTaskListImpl()
  {
    if (GameState.Get().IsTurnStartManagerActive())
    {
      TurnStartManager.Get().NotifyOfTriggerVisual();
      while (TurnStartManager.Get().IsTurnStartIndicatorShowing())
        yield return (object) null;
    }
    if (!this.ActivateInitialSpell())
      base.OnProcessTaskList();
  }

  protected override void OnFinished()
  {
    if (this.m_processingTaskList)
      this.m_pendingFinish = true;
    else
      this.StartCoroutine(this.WaitThenFinish());
  }

  public override bool ShouldReconnectIfStuck()
  {
    if (this.m_triggerSpellByEntityId.Count <= 0)
      return base.ShouldReconnectIfStuck();
    foreach (KeyValuePair<int, Spell> keyValuePair in this.m_triggerSpellByEntityId)
    {
      if ((UnityEngine.Object) keyValuePair.Value != (UnityEngine.Object) null && keyValuePair.Value.ShouldReconnectIfStuck())
        return true;
    }
    return false;
  }

  private void Reset()
  {
    foreach (KeyValuePair<int, Spell> keyValuePair in this.m_triggerSpellByEntityId)
    {
      Spell spell = keyValuePair.Value;
      if (!((UnityEngine.Object) spell == (UnityEngine.Object) null) && spell.GetPowerTaskList() != null && spell.GetPowerTaskList().GetId() == this.m_taskListId)
        SpellUtils.PurgeSpell(spell);
    }
    for (int index = 0; index < this.m_triggerSoundSpells.Count; ++index)
    {
      CardSoundSpell triggerSoundSpell = this.m_triggerSoundSpells[index];
      if ((UnityEngine.Object) triggerSoundSpell != (UnityEngine.Object) null && triggerSoundSpell.GetPowerTaskList().GetId() == this.m_taskListId)
        SpellUtils.PurgeSpell((Spell) triggerSoundSpell);
    }
    foreach (KeyValuePair<int, Spell> keyValuePair in this.m_auxiliaryTriggerSpellByEntityId)
    {
      Spell spell = keyValuePair.Value;
      if (!((UnityEngine.Object) spell == (UnityEngine.Object) null) && spell.GetPowerTaskList() != null && spell.GetPowerTaskList().GetId() == this.m_taskListId)
        SpellUtils.PurgeSpell(spell);
    }
    foreach (KeyValuePair<int, Spell> keyValuePair in this.m_actorTriggerSpellByEntityId)
    {
      Spell spell = keyValuePair.Value;
      if (!((UnityEngine.Object) spell == (UnityEngine.Object) null) && spell.GetPowerTaskList() != null && spell.GetPowerTaskList().GetId() == this.m_taskListId)
        SpellUtils.PurgeSpell(spell);
    }
    this.m_triggerSpellByEntityId.Clear();
    this.m_auxiliaryTriggerSpellByEntityId.Clear();
    this.m_triggerSoundSpells.Clear();
    this.m_actorTriggerSpellByEntityId.Clear();
    this.m_cardEffectsBlockingFinish = 0;
    this.m_cardEffectsBlockingTaskListFinish = 0;
    this.m_actorEffectsBlockingFinish = 0;
  }

  private IEnumerator WaitThenFinish()
  {
    yield return (object) new WaitForSeconds(10f);
    this.Reset();
    base.OnFinished();
  }

  private bool ActivateInitialSpell()
  {
    List<Entity> sourceEntities = this.m_taskList.GetSourceEntities();
    bool flag = false;
    foreach (Entity entity in sourceEntities)
    {
      if (this.ActivateActorTriggerSpell(entity.GetEntityId()))
      {
        flag = true;
      }
      else
      {
        this.ActivateAuxiliaryTriggerSpell(entity.GetEntityId());
        if (this.ActivateCardEffects(entity.GetEntityId()))
          flag = true;
      }
    }
    return flag;
  }

  private void ProcessCurrentTaskList()
  {
    if (this.m_taskList == null)
      return;
    this.m_taskList.DoAllTasks();
  }

  private List<Card> GetCardsWithActorTrigger(PowerTaskList taskList) => this.GetCardsWithActorTrigger(taskList.GetSourceEntities());

  private List<Card> GetCardsWithActorTrigger(List<Entity> entities)
  {
    List<Card> withActorTrigger1 = new List<Card>();
    if (entities == null || entities.Count == 0)
      return withActorTrigger1;
    foreach (Entity entity in entities)
    {
      Card withActorTrigger2 = this.GetCardWithActorTrigger(entity);
      if ((UnityEngine.Object) withActorTrigger2 != (UnityEngine.Object) null)
        withActorTrigger1.Add(withActorTrigger2);
    }
    return withActorTrigger1;
  }

  private Card GetCardWithActorTrigger(Entity entity)
  {
    if (entity == null)
      return (Card) null;
    Card card;
    if (entity.IsEnchantment())
    {
      Entity entity1 = GameState.Get().GetEntity(entity.GetAttached());
      if (entity1 == null)
        return (Card) null;
      card = entity1.GetCard();
    }
    else
      card = entity.GetCard();
    return card;
  }

  private bool CanPlayTriggerSpell(PowerTaskList taskList) => !SpellUtils.IsNonMetaTaskListInMetaBlock(taskList);

  private bool CanPlayActorTriggerSpell(Entity entity)
  {
    if (entity.GetController() != null && !entity.GetController().IsFriendlySide() && entity.IsObfuscated() || !this.m_taskList.IsOrigin())
      return false;
    Card withActorTrigger = this.GetCardWithActorTrigger(entity);
    if ((UnityEngine.Object) withActorTrigger == (UnityEngine.Object) null || withActorTrigger.WillSuppressActorTriggerSpell() || !withActorTrigger.CanShowActorVisuals())
      return false;
    int count = this.m_triggerSpellByEntityId.Count;
    return true;
  }

  private Spell GetActorTriggerSpell(Entity entity)
  {
    Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
    if (blockStart == null)
      return (Spell) null;
    SpellType triggerSpellType = this.GetActorTriggerSpellType(blockStart.TriggerKeyword, entity);
    if (triggerSpellType == SpellType.NONE)
      return (Spell) null;
    Card withActorTrigger = this.GetCardWithActorTrigger(entity);
    if (withActorTrigger == null)
      return (Spell) null;
    Spell actorSpell = withActorTrigger.GetActorSpell(triggerSpellType);
    if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
      actorSpell.SetSource(withActorTrigger.gameObject);
    return actorSpell;
  }

  private SpellType GetActorTriggerSpellType(int triggerKeyword, Entity entity)
  {
    SpellType triggerSpellType = SpellType.NONE;
    switch (triggerKeyword)
    {
      case 32:
      case 424:
        triggerSpellType = entity.GetTriggerSpellType();
        break;
      case 340:
        triggerSpellType = SpellType.SPELLBURST;
        break;
      case 363:
      case 1944:
        triggerSpellType = SpellType.POISONOUS;
        break;
      case 403:
        triggerSpellType = SpellType.INSPIRE;
        break;
      case 685:
      case 1675:
        triggerSpellType = SpellType.LIFESTEAL;
        break;
      case 923:
        triggerSpellType = SpellType.OVERKILL;
        break;
      case 1427:
      case 2672:
        triggerSpellType = SpellType.SPELLBURST;
        break;
      case 1637:
        triggerSpellType = SpellType.FRENZY;
        break;
      case 1920:
        triggerSpellType = SpellType.HONORABLEKILL;
        break;
      case 2129:
        triggerSpellType = SpellType.AVENGE;
        break;
    }
    return triggerSpellType;
  }

  private bool ActivateActorTriggerSpell(int entityId)
  {
    if (!this.m_actorTriggerSpellByEntityId.ContainsKey(entityId))
      return false;
    Spell spell = this.m_actorTriggerSpellByEntityId[entityId];
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return false;
    Entity entity = this.m_taskList.GetSourceEntities().Find((Predicate<Entity>) (e => e.GetEntityId() == entityId));
    Card withActorTrigger = this.GetCardWithActorTrigger(entity);
    if ((UnityEngine.Object) withActorTrigger == (UnityEngine.Object) null)
      return false;
    if (withActorTrigger.IsBaubleAnimating())
      Log.Gameplay.PrintError("TriggerSpellController.ActivateTriggerSpell(): Clobbering bauble that is currently animating on Card {0}.", (object) withActorTrigger);
    withActorTrigger.DeactivateBaubles();
    withActorTrigger.SetIsBaubleAnimating(true);
    ++this.m_actorEffectsBlockingFinish;
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnActorTriggerSpellStateFinished), (object) entity);
    spell.ClearPositionDirtyFlag();
    spell.ActivateState(SpellStateType.ACTION);
    return true;
  }

  private void OnActorTriggerSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (prevStateType != SpellStateType.ACTION)
      return;
    spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnActorTriggerSpellStateFinished), userData);
    this.StartCoroutine(this.FinishActorTriggerSpell(spell, prevStateType, userData));
  }

  private IEnumerator FinishActorTriggerSpell(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    TriggerSpellController triggerSpellController = this;
    Entity entity = (Entity) userData;
    triggerSpellController.m_baubleBlockedFinish = false;
    triggerSpellController.m_waitingForBauble = true;
    bool activatedCardEffects = triggerSpellController.ActivateCardEffects(entity.GetEntityId());
    if (!activatedCardEffects)
      triggerSpellController.ProcessCurrentTaskList();
    triggerSpellController.ActivateAuxiliaryTriggerSpell(entity.GetEntityId());
    switch (triggerSpellController.m_actorTriggerSpellByEntityId[entity.GetEntityId()].GetSpellType())
    {
      case SpellType.TRIGGER:
      case SpellType.POISONOUS:
      case SpellType.FAST_TRIGGER:
      case SpellType.INSPIRE:
      case SpellType.LIFESTEAL:
      case SpellType.DORMANT:
      case SpellType.OVERKILL:
      case SpellType.HONORABLEKILL:
      case SpellType.AVENGE:
        yield return (object) null;
        break;
      default:
        yield return (object) new WaitForSeconds(TriggerSpellController.BAUBLE_WAIT_TIME_SEC);
        break;
    }
    Card withActorTrigger = triggerSpellController.GetCardWithActorTrigger(entity);
    withActorTrigger.SetIsBaubleAnimating(false);
    if (withActorTrigger.CanShowActorVisuals())
      withActorTrigger.UpdateBauble();
    triggerSpellController.m_waitingForBauble = false;
    --triggerSpellController.m_actorEffectsBlockingFinish;
    if (triggerSpellController.m_actorEffectsBlockingFinish <= 0)
    {
      if (!activatedCardEffects)
      {
        // ISSUE: reference to a compiler-generated method
        triggerSpellController.\u003C\u003En__0();
      }
      else if (triggerSpellController.m_baubleBlockedFinish)
        triggerSpellController.OnFinishedTaskList();
    }
  }

  private CardEffect InitEffect(Card card)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return (CardEffect) null;
    Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
    int entityId = card.GetEntity().GetEntityId();
    string effectCardId = this.m_taskList.GetEffectCardId(entityId);
    int index = blockStart.EffectIndex;
    if (index < 0)
    {
      if (string.IsNullOrEmpty(effectCardId) || this.m_taskList.IsEffectCardIdClientCached(entityId))
        return (CardEffect) null;
      index = 0;
    }
    string cardId = card.GetEntity()?.GetCardId();
    if (string.IsNullOrEmpty(effectCardId) || cardId == effectCardId)
      return card.GetTriggerEffect(index);
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(effectCardId))
      return cardDef.CardDef.m_TriggerEffectDefs == null || index >= cardDef.CardDef.m_TriggerEffectDefs.Count ? (CardEffect) null : new CardEffect(cardDef.CardDef.m_TriggerEffectDefs[index], card);
  }

  private bool ActivateCardEffects(int entityId) => this.ActivateTriggerSpell(entityId) | this.ActivateTriggerSounds();

  private void OnCardSpellFinished(Spell spell, object userData) => this.CardSpellFinished();

  private void OnCardSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    this.CardSpellNoneStateEntered();
  }

  private void CardSpellFinished()
  {
    --this.m_cardEffectsBlockingTaskListFinish;
    if (this.m_cardEffectsBlockingTaskListFinish > 0)
      return;
    if (this.m_waitingForBauble)
    {
      this.m_baubleBlockedFinish = true;
      this.ProcessCurrentTaskList();
    }
    else
      this.OnFinishedTaskList();
  }

  private void CardSpellNoneStateEntered()
  {
    --this.m_cardEffectsBlockingFinish;
    if (this.m_cardEffectsBlockingFinish > 0)
      return;
    this.OnFinished();
  }

  private void InitTriggerSpell(CardEffect effect, Card card)
  {
    Spell spell = effect.GetSpell();
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return;
    if (!spell.AttachPowerTaskList(this.m_taskList))
    {
      Log.Power.Print("{0}.InitTriggerSpell() - FAILED to add targets to spell for {1}", (object) this, (object) card);
    }
    else
    {
      this.m_triggerSpellByEntityId.Add(card.GetEntity().GetEntityId(), spell);
      ++this.m_cardEffectsBlockingFinish;
      ++this.m_cardEffectsBlockingTaskListFinish;
    }
  }

  private bool ActivateTriggerSpell(int entityId)
  {
    if (!this.m_triggerSpellByEntityId.ContainsKey(entityId))
      return false;
    Spell spell = this.m_triggerSpellByEntityId[entityId];
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return false;
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnCardSpellFinished));
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnCardSpellStateFinished));
    spell.ActivateState(SpellStateType.ACTION);
    return true;
  }

  private bool InitTriggerSounds(CardEffect effect, Card card)
  {
    List<CardSoundSpell> soundSpells = effect.GetSoundSpells();
    if (soundSpells == null || soundSpells.Count == 0)
      return false;
    foreach (CardSoundSpell cardSoundSpell in soundSpells)
    {
      if ((bool) (UnityEngine.Object) cardSoundSpell)
      {
        if (!cardSoundSpell.AttachPowerTaskList(this.m_taskList))
          Log.Power.Print("{0}.InitTriggerSounds() - FAILED to attach task list to TriggerSoundSpell {1} for Card {2}", (object) this.name, (object) cardSoundSpell, (object) card);
        else
          this.m_triggerSoundSpells.Add(cardSoundSpell);
      }
    }
    if (this.m_triggerSoundSpells.Count == 0)
      return false;
    ++this.m_cardEffectsBlockingFinish;
    ++this.m_cardEffectsBlockingTaskListFinish;
    return true;
  }

  private bool ActivateTriggerSounds()
  {
    if (this.m_triggerSoundSpells.Count == 0)
      return false;
    Card source = this.GetSource();
    foreach (CardSoundSpell triggerSoundSpell in this.m_triggerSoundSpells)
    {
      if ((bool) (UnityEngine.Object) triggerSoundSpell)
        source.ActivateSoundSpell(triggerSoundSpell);
    }
    if (this.m_taskList.IsOrigin() && !this.m_taskList.DoesBlockHaveEffectTimingMetaData())
      this.m_taskList.CreateTask((Network.PowerHistory) new Network.HistMetaData()
      {
        MetaType = HistoryMeta.Type.EFFECT_TIMING
      });
    this.CardSpellFinished();
    this.CardSpellNoneStateEntered();
    return true;
  }

  private Spell GetAuxiliaryTriggerSpell()
  {
    int triggerKeyword = this.m_taskList.GetBlockStart().TriggerKeyword;
    for (int index = 0; index < this.m_AuxiliaryTriggerSpells.Count; ++index)
    {
      if (this.m_AuxiliaryTriggerSpells[index].m_TriggerKeyword == (GAME_TAG) triggerKeyword)
      {
        Spell spell = SpellManager.Get().GetSpell(this.m_AuxiliaryTriggerSpells[index].m_Spell);
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
          return spell;
        Log.Gameplay.PrintError("{0}.GetAuxiliaryTriggerSpell(): keyword:{1}, spell:{2}", (object) this, (object) triggerKeyword, (object) this.m_AuxiliaryTriggerSpells[index].m_Spell);
        return (Spell) null;
      }
    }
    return (Spell) null;
  }

  private void ActivateAuxiliaryTriggerSpell(int entityId)
  {
    if (!this.m_auxiliaryTriggerSpellByEntityId.ContainsKey(entityId) || (UnityEngine.Object) this.m_auxiliaryTriggerSpellByEntityId[entityId] == (UnityEngine.Object) null)
      return;
    this.m_auxiliaryTriggerSpellByEntityId[entityId].ActivateState(SpellStateType.ACTION);
  }

  protected override float GetLostFrameTimeCatchUpSeconds() => 0.2f;
}
