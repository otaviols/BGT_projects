using Blizzard.T5.Core;
using System.Collections;
using UnityEngine;

public class SpawnToHandSpell : SuperSpell
{
  public float m_CardStartScale = 0.1f;
  public float m_CardDelay = 1f;
  public float m_CardStaggerMin;
  public float m_CardStaggerMax;
  public bool m_AccumulateStagger = true;
  public bool m_Shake = true;
  public float m_ShakeDelay;
  public ShakeMinionIntensity m_ShakeIntensity = ShakeMinionIntensity.MediumShake;
  public Spell m_SpellPrefab;
  protected Map<int, Card> m_targetToOriginMap;

  public override bool AddPowerTargets() => this.AddPowerTargetsInternal(false);

  public override void RemoveAllTargets()
  {
    base.RemoveAllTargets();
    if (this.m_targetToOriginMap == null)
      return;
    this.m_targetToOriginMap.Clear();
  }

  protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    if (power.Type != Network.PowerType.FULL_ENTITY)
      return (Card) null;
    Network.Entity entity1 = (power as Network.HistFullEntity).Entity;
    Entity entity2 = GameState.Get().GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Debug.LogWarning((object) string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) entity1.ID));
      return (Card) null;
    }
    return entity2.GetZone() != TAG_ZONE.HAND ? (Card) null : entity2.GetCard();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    this.FillUniqueOriginForTargets();
    this.StartCoroutine(this.DoEffectWithTiming());
  }

  protected virtual Vector3 GetOriginForTarget(int targetIndex = 0)
  {
    Card card;
    return this.m_targetToOriginMap == null || !this.m_targetToOriginMap.TryGetValue(targetIndex, out card) ? this.GetFallbackOriginPosition() : card.transform.position;
  }

  protected void AddOriginForTarget(int targetIndex, Card card)
  {
    if (this.m_targetToOriginMap == null)
      this.m_targetToOriginMap = new Map<int, Card>();
    this.m_targetToOriginMap[targetIndex] = card;
  }

  protected bool AddUniqueOriginForTarget(int targetIndex, Card card)
  {
    if (this.m_targetToOriginMap != null && this.m_targetToOriginMap.ContainsValue(card))
      return false;
    this.AddOriginForTarget(targetIndex, card);
    return true;
  }

  private void FillUniqueOriginForTargets()
  {
    ZonePlay battlefieldZone = this.GetSourceCard().GetEntity().GetController().GetBattlefieldZone();
    for (int index1 = 0; index1 < this.m_targets.Count; ++index1)
    {
      for (int index2 = 0; index2 < battlefieldZone.GetCardCount(); ++index2)
      {
        Card cardAtIndex = battlefieldZone.GetCardAtIndex(index2);
        Card component = this.m_targets[index1].GetComponent<Card>();
        if (cardAtIndex.GetEntity().IsMinion() && component.GetEntity().GetCreator() == cardAtIndex.GetEntity() && this.AddUniqueOriginForTarget(index1, cardAtIndex))
          break;
      }
    }
  }

  protected virtual IEnumerator DoEffectWithTiming()
  {
    SpawnToHandSpell spawnToHandSpell = this;
    GameObject sourceObject = spawnToHandSpell.GetSource();
    Actor actor = sourceObject.GetComponent<Card>().GetActor();
    if ((bool) (Object) actor && spawnToHandSpell.m_Shake)
    {
      GameObject gameObject = actor.gameObject;
      MinionShake.ShakeObject(gameObject, ShakeMinionType.RandomDirection, gameObject.transform.position, spawnToHandSpell.m_ShakeIntensity, 0.1f, 0.0f, spawnToHandSpell.m_ShakeDelay, true);
    }
    yield return (object) new WaitForSeconds(spawnToHandSpell.m_CardDelay);
    spawnToHandSpell.AddTransitionDelays();
    for (int index = 0; index < spawnToHandSpell.m_targets.Count; ++index)
    {
      GameObject target = spawnToHandSpell.m_targets[index];
      Card component = target.GetComponent<Card>();
      component.transform.position = spawnToHandSpell.GetOriginForTarget(index);
      float transitionDelay = component.GetTransitionDelay();
      if ((Object) spawnToHandSpell.m_SpellPrefab != (Object) null)
      {
        Spell spell = spawnToHandSpell.CloneSpell(spawnToHandSpell.m_SpellPrefab, finishedCallback: ((Spell.FinishedCallback) ((s, o) => { })));
        spell.SetSource(sourceObject);
        spell.AddTarget(target);
        spell.SetPosition(component.transform.position);
        spawnToHandSpell.StartCoroutine(spawnToHandSpell.ActivateSpellAfterDelay(spell, transitionDelay));
      }
      component.transform.localScale = new Vector3(spawnToHandSpell.m_CardStartScale, spawnToHandSpell.m_CardStartScale, spawnToHandSpell.m_CardStartScale);
      component.SetTransitionStyle(ZoneTransitionStyle.VERY_SLOW);
      component.SetDoNotWarpToNewZone(true);
    }
    --spawnToHandSpell.m_effectsPendingFinish;
    spawnToHandSpell.FinishIfPossible();
  }

  protected IEnumerator ActivateSpellAfterDelay(Spell spell, float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    spell.Activate();
  }

  protected string GetCardIdForTarget(int targetIndex) => this.m_targets[targetIndex].GetComponent<Card>().GetEntity().GetCardId();

  protected Vector3 GetFallbackOriginPosition()
  {
    Card component = this.GetSource().GetComponent<Card>();
    if (!component.GetEntity().HasTag(GAME_TAG.USE_LEADERBOARD_AS_SPAWN_ORIGIN) || !((Object) PlayerLeaderboardManager.Get() != (Object) null))
      return this.transform.position;
    PlayerLeaderboardCard tileForPlayerId = PlayerLeaderboardManager.Get().GetTileForPlayerId(component.GetEntity().GetTag(GAME_TAG.PLAYER_ID));
    return (Object) tileForPlayerId != (Object) null ? tileForPlayerId.m_PlayerLeaderboardTile.transform.position : PlayerLeaderboardManager.Get().transform.position;
  }

  private void AddTransitionDelays()
  {
    if ((double) this.m_CardStaggerMin <= 0.0 && (double) this.m_CardStaggerMax <= 0.0)
      return;
    if (this.m_AccumulateStagger)
    {
      float num1 = 0.0f;
      for (int index = 0; index < this.m_targets.Count; ++index)
      {
        Card component = this.m_targets[index].GetComponent<Card>();
        float num2 = Random.Range(this.m_CardStaggerMin, this.m_CardStaggerMax);
        num1 += num2;
        double delay = (double) num1;
        component.SetTransitionDelay((float) delay);
      }
    }
    else
    {
      for (int index = 0; index < this.m_targets.Count; ++index)
        this.m_targets[index].GetComponent<Card>().SetTransitionDelay(Random.Range(this.m_CardStaggerMin, this.m_CardStaggerMax));
    }
  }
}
