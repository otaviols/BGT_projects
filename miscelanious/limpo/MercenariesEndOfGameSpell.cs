using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesEndOfGameSpell : SuperSpell
{
  public float m_secondsOffsetToStartExplosionFx;
  private readonly List<Card> m_cardsToExplode = new List<Card>();
  private bool m_isExplosionFxStarted;

  private void AddPowerTargets_FromActorsInPlay(Player.Side side)
  {
    ZonePlay zoneOfType = ZoneMgr.Get().FindZoneOfType<ZonePlay>(side);
    if (!((Object) zoneOfType != (Object) null))
      return;
    this.m_cardsToExplode.AddRange((IEnumerable<Card>) zoneOfType.GetCards());
  }

  public override bool AddPowerTargets()
  {
    if (GameState.Get().GetOpposingSidePlayer().GetTag<TAG_PLAYSTATE>(GAME_TAG.PLAYSTATE) != TAG_PLAYSTATE.WON)
      this.AddPowerTargets_FromActorsInPlay(Player.Side.OPPOSING);
    if (GameState.Get().GetFriendlySidePlayer().GetTag<TAG_PLAYSTATE>(GAME_TAG.PLAYSTATE) != TAG_PLAYSTATE.WON)
      this.AddPowerTargets_FromActorsInPlay(Player.Side.FRIENDLY);
    return true;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.m_effectsPendingFinish += this.m_cardsToExplode.Count;
    base.OnAction(prevStateType);
    LettuceMissionEntity letlMissionEntity = GameState.Get()?.GetGameEntity() as LettuceMissionEntity;
    if (this.m_taskList.HasTasks())
      letlMissionEntity?.RegisterOnEmoteBanterPlayedEvent(new LettuceMissionEntity.OnEmoteBanterPlayedDelegate(this.OnEmoteBanterPlayed));
    this.m_taskList.DoAllTasks((PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) =>
    {
      letlMissionEntity?.UnregisterOnEmoteBanterPlayedEvent(new LettuceMissionEntity.OnEmoteBanterPlayedDelegate(this.OnEmoteBanterPlayed));
      this.PlayActorExplosionFx();
    }));
  }

  private void OnEmoteBanterPlayed(
    LettuceMissionEntity letlMissionEntity,
    EmoteType emoteType,
    AudioSource audioSource)
  {
    if (emoteType != EmoteType.WELL_PLAYED)
      return;
    letlMissionEntity.UnregisterOnEmoteBanterPlayedEvent(new LettuceMissionEntity.OnEmoteBanterPlayedDelegate(this.OnEmoteBanterPlayed));
    float delaySec = audioSource.clip.length - this.m_secondsOffsetToStartExplosionFx;
    if ((double) delaySec > 0.0)
      this.StartCoroutine(this.WaitForSecondsThenPlayActorExplosionFx(delaySec));
    else
      this.PlayActorExplosionFx();
  }

  private IEnumerator WaitForSecondsThenPlayActorExplosionFx(float delaySec)
  {
    yield return (object) new WaitForSeconds(delaySec);
    this.PlayActorExplosionFx();
  }

  private void PlayActorExplosionFx()
  {
    if (this.m_isExplosionFxStarted)
      return;
    this.m_isExplosionFxStarted = true;
    if (this.m_cardsToExplode.Count == 0)
    {
      this.FinishIfPossible();
    }
    else
    {
      foreach (Card card in this.m_cardsToExplode)
      {
        Actor actor = card.GetActor();
        if ((Object) actor == (Object) null)
        {
          this.OnExplodeSpellFinished();
        }
        else
        {
          actor.ActivateAllSpellsDeathStates();
          actor.ToggleForceIdle(true);
          actor.SetActorState(ActorStateType.CARD_IDLE);
          if ((Object) card.ActivateActorSpell(SpellType.MERCENARIES_PORTRAIT_EXPLODE, (Spell.FinishedCallback) ((dummySpell, userData) => this.OnExplodeSpellFinished())) == (Object) null)
            this.OnExplodeSpellFinished();
        }
      }
    }
  }

  private void OnExplodeSpellFinished()
  {
    --this.m_effectsPendingFinish;
    this.FinishIfPossible();
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    GameState.Get()?.GetGameEntity()?.ActivateEndOfGameSpellState(SpellStateType.DEATH);
  }
}
