using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSpellController : SpellController
{
  private Spell m_powerSpell;
  private List<CardSoundSpell> m_powerSoundSpells = new List<CardSoundSpell>();
  private Entity m_ownerHeroEntity;
  private Entity m_powerSourceEntity;
  private int m_cardEffectsBlockingFinish;
  private int m_cardEffectsBlockingTaskListFinish;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    if (!this.HasSourceCard(taskList))
      return false;
    Entity sourceEntity = taskList.GetSourceEntity();
    Card card = sourceEntity.GetCard();
    CardEffect effect = PowerSpellController.GetOrCreateEffect(card, this.m_taskList);
    if (effect == null)
      return false;
    if (sourceEntity.IsMinion() || sourceEntity.IsHero() || sourceEntity.IsLocation())
    {
      if (sourceEntity.IsLocation())
      {
        bool flag = false;
        PowerTaskList parent = taskList.GetParent();
        if (parent != null)
        {
          if (parent.HasZoneChanges())
          {
            flag = true;
          }
          else
          {
            foreach (PowerTask task in parent.GetTaskList())
            {
              if (task.GetPower() is Network.HistTagChange power && power.Tag == 261 && power.Value != 0)
              {
                flag = true;
                break;
              }
            }
          }
        }
        if (flag)
        {
          Spell spell = effect.GetSpell();
          if ((Object) spell != (Object) null && !spell.IsActive())
          {
            this.Reset();
            return false;
          }
        }
      }
      if (!this.InitPowerSpell(effect, card))
      {
        if (!SpellUtils.CanAddPowerTargets(taskList))
        {
          this.Reset();
          return false;
        }
        if ((Object) this.GetActorBattlecrySpell(card) == (Object) null)
        {
          this.Reset();
          return false;
        }
      }
    }
    else
    {
      this.InitPowerSpell(effect, card);
      this.InitPowerSounds(effect, card);
      if ((Object) this.m_powerSpell == (Object) null && this.m_powerSoundSpells.Count == 0)
      {
        this.Reset();
        return false;
      }
    }
    this.SetSource(card);
    return true;
  }

  protected override void OnProcessTaskList()
  {
    if (this.ActivateActorBattlecrySpell() || this.ActivateCardEffects())
      return;
    base.OnProcessTaskList();
  }

  protected override void OnFinished()
  {
    if (this.m_processingTaskList)
      this.m_pendingFinish = true;
    else
      this.StartCoroutine(this.WaitThenFinish());
  }

  public override bool ShouldReconnectIfStuck() => (Object) this.m_powerSpell != (Object) null ? this.m_powerSpell.ShouldReconnectIfStuck() : base.ShouldReconnectIfStuck();

  private void Reset()
  {
    if ((Object) this.m_powerSpell != (Object) null && this.m_powerSpell.GetPowerTaskList().GetId() == this.m_taskListId)
      SpellUtils.PurgeSpell(this.m_powerSpell);
    if (this.m_powerSoundSpells != null)
    {
      for (int index = 0; index < this.m_powerSoundSpells.Count; ++index)
      {
        CardSoundSpell powerSoundSpell = this.m_powerSoundSpells[index];
        if ((Object) powerSoundSpell != (Object) null && powerSoundSpell.GetPowerTaskList().GetId() == this.m_taskListId)
          SpellUtils.PurgeSpell((Spell) powerSoundSpell);
      }
    }
    this.m_powerSpell = (Spell) null;
    this.m_powerSoundSpells.Clear();
    this.m_cardEffectsBlockingFinish = 0;
    this.m_cardEffectsBlockingTaskListFinish = 0;
  }

  private IEnumerator WaitThenFinish()
  {
    yield return (object) new WaitForSeconds(10f);
    this.Reset();
    base.OnFinished();
  }

  private Spell GetActorBattlecrySpell(Card card)
  {
    Spell actorSpell = card.GetActorSpell(SpellType.BATTLECRY);
    if ((Object) actorSpell == (Object) null)
      return (Spell) null;
    return !actorSpell.HasUsableState(SpellStateType.ACTION) ? (Spell) null : actorSpell;
  }

  private bool ActivateActorBattlecrySpell()
  {
    Card source = this.GetSource();
    if (!this.CanActivateActorBattlecrySpell(source))
      return false;
    Spell actorBattlecrySpell = this.GetActorBattlecrySpell(source);
    if ((Object) actorBattlecrySpell == (Object) null)
      return false;
    this.m_taskList.SetActivateBattlecrySpellState();
    this.StartCoroutine(this.WaitThenActivateActorBattlecrySpell(actorBattlecrySpell));
    return true;
  }

  private bool CanActivateActorBattlecrySpell(Card card)
  {
    Entity entity = card.GetEntity();
    if (entity.GetZone() != TAG_ZONE.PLAY || entity.HasTag(GAME_TAG.FAST_BATTLECRY))
      return false;
    Spell actorBattlecrySpell = this.GetActorBattlecrySpell(card);
    return (Object) actorBattlecrySpell != (Object) null && actorBattlecrySpell.GetActiveState() == SpellStateType.BIRTH || this.m_taskList.ShouldActivateBattlecrySpell() && (entity.HasBattlecry() || entity.HasCombo() && entity.GetController().IsComboActive());
  }

  private IEnumerator WaitThenActivateActorBattlecrySpell(Spell actorBattlecrySpell)
  {
    yield return (object) new WaitForSeconds(0.2f);
    actorBattlecrySpell.ActivateState(SpellStateType.ACTION);
    if (!this.ActivateCardEffects())
      base.OnProcessTaskList();
  }

  public static CardEffect GetOrCreateEffect(Card card, PowerTaskList taskList)
  {
    if ((Object) card == (Object) null)
      return (CardEffect) null;
    CardEffect effect = (CardEffect) null;
    Network.HistBlockStart blockStart = taskList.GetBlockStart();
    string effectCardId = taskList.GetEffectCardId();
    int subOption = blockStart.SubOption;
    int effectIndex = blockStart.EffectIndex;
    Entity entity = card.GetEntity();
    string cardId = entity?.GetCardId();
    if (string.IsNullOrEmpty(effectCardId) || string.IsNullOrEmpty(cardId) || cardId == effectCardId)
    {
      if (subOption >= 0)
        effect = card.GetSubOptionEffect(subOption, effectIndex);
      else if (!entity.HasTag(GAME_TAG.IS_USING_TRADE_OPTION))
        effect = card.GetPlayEffect(effectIndex);
    }
    else
    {
      using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(effectCardId))
      {
        CardEffectDef proxyEffectDef;
        if (subOption >= 0)
        {
          if (effectIndex > 0)
          {
            if (cardDef.CardDef.m_AdditionalSubOptionEffectDefs == null || subOption >= cardDef.CardDef.m_AdditionalSubOptionEffectDefs.Count)
              return (CardEffect) null;
            List<CardEffectDef> subOptionEffectDef = cardDef.CardDef.m_AdditionalSubOptionEffectDefs[subOption];
            int index = effectIndex - 1;
            if (index >= subOptionEffectDef.Count)
              return (CardEffect) null;
            proxyEffectDef = subOptionEffectDef[index];
          }
          else
          {
            if (cardDef.CardDef.m_SubOptionEffectDefs == null || subOption >= cardDef.CardDef.m_SubOptionEffectDefs.Count)
              return (CardEffect) null;
            proxyEffectDef = cardDef.CardDef.m_SubOptionEffectDefs[subOption];
          }
        }
        else if (effectIndex > 0)
        {
          if (cardDef.CardDef.m_AdditionalPlayEffectDefs == null)
            return (CardEffect) null;
          int index = effectIndex - 1;
          if (index >= cardDef.CardDef.m_AdditionalPlayEffectDefs.Count)
            return (CardEffect) null;
          proxyEffectDef = cardDef.CardDef.m_AdditionalPlayEffectDefs[index];
        }
        else
          proxyEffectDef = cardDef.CardDef.m_PlayEffectDef;
        effect = card.GetOrCreateProxyEffect(blockStart, proxyEffectDef);
      }
    }
    return effect;
  }

  private bool ActivateCardEffects() => this.ActivatePowerSpell() | this.ActivatePowerSounds();

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
    this.OnFinishedTaskList();
  }

  private void CardSpellNoneStateEntered()
  {
    --this.m_cardEffectsBlockingFinish;
    if (this.m_cardEffectsBlockingFinish > 0)
      return;
    this.OnFinished();
  }

  private bool InitPowerSpell(CardEffect effect, Card card)
  {
    Spell spell = effect.GetSpell();
    if ((Object) spell == (Object) null)
      return false;
    if (!spell.HasUsableState(SpellStateType.ACTION))
    {
      Log.Power.PrintWarning("{0}.InitPowerSpell() - spell {1} for Card {2} has no {3} state", (object) this.name, (object) spell, (object) card, (object) SpellStateType.ACTION);
      return false;
    }
    if (!spell.AttachPowerTaskList(this.m_taskList))
    {
      Log.Power.Print("{0}.InitPowerSpell() - FAILED to attach task list to spell {1} for Card {2}", (object) this.name, (object) spell, (object) card);
      return false;
    }
    if (spell.GetActiveState() != SpellStateType.NONE)
      spell.ActivateState(SpellStateType.NONE);
    this.m_powerSpell = spell;
    this.InitPowerSpellOwnerHero();
    ++this.m_cardEffectsBlockingFinish;
    ++this.m_cardEffectsBlockingTaskListFinish;
    return true;
  }

  private void InitPowerSpellOwnerHero()
  {
    if ((Object) this.m_powerSpell == (Object) null)
      return;
    Player controller = this.m_powerSpell.GetPowerSourceCard().GetController();
    if (controller == null)
      return;
    this.m_ownerHeroEntity = controller.GetHero();
    this.m_powerSourceEntity = this.m_powerSpell.GetPowerSource();
  }

  private bool ActivatePowerSpell()
  {
    if ((Object) this.m_powerSpell == (Object) null)
      return false;
    this.m_powerSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnCardSpellFinished));
    this.m_powerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnCardSpellStateFinished));
    this.m_powerSpell.ActivateState(SpellStateType.ACTION);
    return true;
  }

  private bool InitPowerSounds(CardEffect effect, Card card)
  {
    List<CardSoundSpell> soundSpells = effect.GetSoundSpells();
    if (soundSpells == null || soundSpells.Count == 0)
      return false;
    foreach (CardSoundSpell cardSoundSpell in soundSpells)
    {
      if ((bool) (Object) cardSoundSpell)
      {
        if (!cardSoundSpell.AttachPowerTaskList(this.m_taskList))
          Log.Power.Print("{0}.InitPowerSounds() - FAILED to attach task list to PowerSoundSpell {1} for Card {2}", (object) this.name, (object) cardSoundSpell, (object) card);
        else
          this.m_powerSoundSpells.Add(cardSoundSpell);
      }
    }
    if (this.m_powerSoundSpells.Count == 0)
      return false;
    ++this.m_cardEffectsBlockingFinish;
    ++this.m_cardEffectsBlockingTaskListFinish;
    return true;
  }

  private bool ActivatePowerSounds()
  {
    if (this.m_powerSoundSpells.Count == 0)
      return false;
    Card source = this.GetSource();
    foreach (CardSoundSpell powerSoundSpell in this.m_powerSoundSpells)
    {
      if ((bool) (Object) powerSoundSpell)
        source.ActivateSoundSpell(powerSoundSpell);
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
}
