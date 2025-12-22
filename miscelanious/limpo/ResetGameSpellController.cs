using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameSpellController : SpellController
{
  public Spell m_DefaultHideScreenSpell;
  private static Spell s_hideScreenSpellInstance;
  private int m_resetGameTaskIndex;
  private Entity m_clonedSourceEntity;
  private Entity m_prevGameEntity;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    ResetGameSpellController.s_hideScreenSpellInstance = (Spell) null;
    Entity sourceEntity = taskList.GetSourceEntity();
    if (sourceEntity != null)
    {
      Card card = sourceEntity.GetCard();
      CardEffect effect = this.InitEffect(card);
      if (effect != null)
        ResetGameSpellController.s_hideScreenSpellInstance = this.InitResetGameSpell(effect, card);
    }
    if (!taskList.IsStartOfBlock() || !taskList.IsEndOfBlock())
      Log.Gameplay.PrintWarning(string.Format("{0}.AddPowerSourceAndTargets(): ResetGame power block was split into multiple tasklists.", (object) this));
    this.m_resetGameTaskIndex = -1;
    List<PowerTask> taskList1 = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      if (taskList1[index].GetPower() is Network.HistResetGame)
      {
        this.m_resetGameTaskIndex = index;
        break;
      }
    }
    if (this.m_clonedSourceEntity == null && taskList.GetSourceEntity() != null)
      this.m_clonedSourceEntity = taskList.GetSourceEntity().CloneForZoneMgr();
    return true;
  }

  protected override void OnProcessTaskList() => this.StartCoroutine(this.DoEffectsWithTiming());

  private IEnumerator DoEffectsWithTiming()
  {
    ResetGameSpellController gameSpellController = this;
    if (gameSpellController.m_taskList.IsStartOfBlock())
    {
      if (gameSpellController.m_prevGameEntity == null)
        gameSpellController.m_prevGameEntity = (Entity) GameState.Get().GetGameEntity();
      GameState.Get().GetGameEntity().NotifyOfResetGameStarted();
    }
    if (gameSpellController.m_resetGameTaskIndex != -1)
    {
      if ((Object) ResetGameSpellController.s_hideScreenSpellInstance == (Object) null)
        ResetGameSpellController.s_hideScreenSpellInstance = SpellManager.Get().GetSpell(gameSpellController.m_DefaultHideScreenSpell);
      ResetGameSpellController.s_hideScreenSpellInstance.ActivateState(SpellStateType.BIRTH);
      while (ResetGameSpellController.s_hideScreenSpellInstance.GetActiveState() != SpellStateType.IDLE)
        yield return (object) null;
      PowerTask resetGameTask = gameSpellController.m_taskList.GetTaskList()[gameSpellController.m_resetGameTaskIndex];
      gameSpellController.m_taskList.DoTasks(0, gameSpellController.m_resetGameTaskIndex + 1);
      while (!resetGameTask.IsCompleted())
        yield return (object) null;
      resetGameTask = (PowerTask) null;
    }
    List<Card> recreatedCards = new List<Card>();
    List<PowerTask> taskList = gameSpellController.m_taskList.GetTaskList();
    for (int resetGameTaskIndex = gameSpellController.m_resetGameTaskIndex; resetGameTaskIndex < taskList.Count; ++resetGameTaskIndex)
    {
      if (taskList[resetGameTaskIndex].GetPower() is Network.HistFullEntity power)
      {
        Entity entity = GameState.Get().GetEntity(power.Entity.ID);
        if (entity != null)
        {
          Card card = entity.GetCard();
          if (!((Object) card == (Object) null))
          {
            card.SuppressPlaySounds(true);
            card.SetTransitionStyle(ZoneTransitionStyle.INSTANT);
            recreatedCards.Add(card);
          }
        }
      }
    }
    gameSpellController.m_taskList.DoAllTasks();
    while (!gameSpellController.m_taskList.IsComplete())
      yield return (object) null;
    foreach (Card card in recreatedCards)
    {
      card.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
      card.SuppressPlaySounds(false);
      Entity entity = card.GetEntity();
      switch (entity.GetZone())
      {
        case TAG_ZONE.PLAY:
        case TAG_ZONE.SECRET:
          card.ShowExhaustedChange(entity.IsExhausted());
          continue;
        default:
          continue;
      }
    }
    if (gameSpellController.m_taskList.IsEndOfBlock())
    {
      EndTurnButton.Get().Reset();
      ResetGameSpellController.s_hideScreenSpellInstance.ActivateState(SpellStateType.DEATH);
      while (ResetGameSpellController.s_hideScreenSpellInstance.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
      SpellManager.Get().ReleaseSpell(ResetGameSpellController.s_hideScreenSpellInstance);
      ResetGameSpellController.s_hideScreenSpellInstance = (Spell) null;
      GameState.Get().GetGameEntity().NotifyOfResetGameFinished(gameSpellController.m_clonedSourceEntity, gameSpellController.m_prevGameEntity);
      gameSpellController.m_prevGameEntity = (Entity) null;
    }
    gameSpellController.OnFinishedTaskList();
    gameSpellController.OnFinished();
  }

  private CardEffect InitEffect(Card card)
  {
    if ((Object) card == (Object) null)
      return (CardEffect) null;
    int effectIndex = this.m_taskList.GetBlockStart().EffectIndex;
    return effectIndex < 0 ? (CardEffect) null : card.GetResetGameEffect(effectIndex);
  }

  private Spell InitResetGameSpell(CardEffect effect, Card card)
  {
    Spell spell = effect.GetSpell();
    if ((Object) spell == (Object) null)
      return (Spell) null;
    if (spell.AttachPowerTaskList(this.m_taskList))
      return spell;
    Log.Power.Print("{0}.InitResetGameSpell() - FAILED to add targets to spell for {1}", (object) this, (object) card);
    return (Spell) null;
  }
}
