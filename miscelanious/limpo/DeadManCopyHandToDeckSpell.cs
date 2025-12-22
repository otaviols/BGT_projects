using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadManCopyHandToDeckSpell : SuperSpell
{
  public float m_MoveUpTime;
  public float m_MoveUpOffsetZ;
  public float m_MoveUpScale;
  public float m_MoveToDeckInterval;
  public bool m_ShuffleRealHandToDeck;
  private int m_taskCountToRunFirst;
  private bool m_waitForTasksToComplete;
  private List<Entity> m_entitiesToDrawBeforeFX = new List<Entity>();
  private List<Actor> m_actors = new List<Actor>();
  private List<Actor> m_friendlyActors = new List<Actor>();
  private List<Actor> m_opposingActors = new List<Actor>();
  private int m_numActorsInLoading;

  public override bool AddPowerTargets()
  {
    this.m_visualToTargetIndexMap.Clear();
    this.m_targetToMetaDataMap.Clear();
    this.m_targets.Clear();
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      PowerTask task = taskList[index];
      Card cardFromPowerTask = this.GetTargetCardFromPowerTask(index, task);
      if (!((UnityEngine.Object) cardFromPowerTask == (UnityEngine.Object) null) && this.IsValidSpellTarget(cardFromPowerTask.GetEntity()) && !this.m_targets.Contains(cardFromPowerTask.gameObject))
      {
        this.AddTarget(cardFromPowerTask.gameObject);
        cardFromPowerTask.SuppressHandToDeckTransition();
        if (this.m_targets.Count == 1)
          this.m_taskCountToRunFirst = index;
      }
    }
    return this.m_targets.Count != 0;
  }

  protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    if (power.Type == Network.PowerType.TAG_CHANGE)
    {
      Network.HistTagChange histTagChange = (Network.HistTagChange) power;
      if (histTagChange.Tag != 49)
        return (Card) null;
      if (histTagChange.Value != 2)
        return (Card) null;
      Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
      if (entity == null)
        return (Card) null;
      return entity.GetZone() != TAG_ZONE.HAND && entity.GetZone() != TAG_ZONE.SETASIDE ? (Card) null : entity.GetCard();
    }
    if (power.Type == Network.PowerType.HIDE_ENTITY)
    {
      Network.HistHideEntity histHideEntity = (Network.HistHideEntity) power;
      if (histHideEntity.Zone != 2)
        return (Card) null;
      Entity entity = GameState.Get().GetEntity(histHideEntity.Entity);
      if (entity == null)
        return (Card) null;
      return entity.GetZone() != TAG_ZONE.HAND ? (Card) null : entity.GetCard();
    }
    if (power.Type != Network.PowerType.FULL_ENTITY)
      return (Card) null;
    Network.Entity entity1 = ((Network.HistFullEntity) power).Entity;
    if (entity1 == null)
      return (Card) null;
    return GameState.Get().GetEntity(entity1.ID)?.GetCard();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    this.FindEntitiesToDrawBeforeFX();
    this.DoTasks();
    this.StartCoroutine(this.DoActionWithTiming());
  }

  protected override void DoActionNow()
  {
  }

  private IEnumerator DoActionWithTiming()
  {
    DeadManCopyHandToDeckSpell copyHandToDeckSpell = this;
    if (copyHandToDeckSpell.m_ShuffleRealHandToDeck)
      yield return (object) copyHandToDeckSpell.StartCoroutine(copyHandToDeckSpell.WaitForPendingCardDraw());
    yield return (object) copyHandToDeckSpell.StartCoroutine(copyHandToDeckSpell.WaitForTasksAndDrawing());
    yield return (object) copyHandToDeckSpell.StartCoroutine(copyHandToDeckSpell.LoadAssets());
    yield return (object) copyHandToDeckSpell.StartCoroutine(copyHandToDeckSpell.DoEffects());
  }

  private void FindEntitiesToDrawBeforeFX()
  {
    Card sourceCard = this.GetSourceCard();
    this.m_entitiesToDrawBeforeFX.Clear();
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < this.m_taskCountToRunFirst; ++index)
    {
      PowerTask powerTask = taskList[index];
      if (sourceCard.GetControllerSide() == Player.Side.FRIENDLY)
      {
        this.FindRevealedEntitiesToDrawBeforeFX(powerTask.GetPower());
      }
      else
      {
        this.FindRevealedEntitiesToDrawBeforeFX(powerTask.GetPower());
        this.FindHiddenEntitiesToDrawBeforeFX(powerTask.GetPower());
      }
    }
  }

  private void FindRevealedEntitiesToDrawBeforeFX(Network.PowerHistory power)
  {
    if (power.Type != Network.PowerType.SHOW_ENTITY)
      return;
    Network.HistShowEntity histShowEntity = (Network.HistShowEntity) power;
    Entity entity = GameState.Get().GetEntity(histShowEntity.Entity.ID);
    if (entity == null || entity.GetZone() != TAG_ZONE.DECK)
      return;
    if (histShowEntity.Entity.Tags.Exists((Predicate<Network.Entity.Tag>) (tag => tag.Name == 49 && tag.Value == 3)))
    {
      this.m_entitiesToDrawBeforeFX.Add(entity);
    }
    else
    {
      if (!histShowEntity.Entity.Tags.Exists((Predicate<Network.Entity.Tag>) (tag => tag.Name == 49 && tag.Value == 4)))
        return;
      this.m_entitiesToDrawBeforeFX.Add(entity);
    }
  }

  private void FindHiddenEntitiesToDrawBeforeFX(Network.PowerHistory power)
  {
    if (power.Type != Network.PowerType.TAG_CHANGE)
      return;
    Network.HistTagChange histTagChange = (Network.HistTagChange) power;
    if (histTagChange.Tag != 49 || histTagChange.Value != 3 && histTagChange.Value != 4)
      return;
    Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
    if (entity == null)
    {
      Debug.LogWarningFormat("{0}.FindOpponentEntitiesToDrawBeforeFX() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity);
    }
    else
    {
      if (entity.GetZone() != TAG_ZONE.DECK)
        return;
      this.m_entitiesToDrawBeforeFX.Add(entity);
    }
  }

  private void DoTasks()
  {
    if (this.m_taskCountToRunFirst <= 0)
    {
      this.m_waitForTasksToComplete = false;
    }
    else
    {
      this.m_waitForTasksToComplete = true;
      this.m_taskList.DoTasks(0, this.m_taskCountToRunFirst, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => this.m_waitForTasksToComplete = false));
    }
  }

  private IEnumerator LoadAssets()
  {
    DeadManCopyHandToDeckSpell copyHandToDeckSpell = this;
    copyHandToDeckSpell.m_numActorsInLoading = copyHandToDeckSpell.m_targets.Count;
    copyHandToDeckSpell.m_actors.Clear();
    copyHandToDeckSpell.m_friendlyActors.Clear();
    copyHandToDeckSpell.m_opposingActors.Clear();
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < copyHandToDeckSpell.m_targets.Count; ++index)
    {
      if (copyHandToDeckSpell.m_targets[index].GetComponent<Card>().GetEntity().IsControlledByFriendlySidePlayer())
        ++num1;
      else
        ++num2;
    }
    int num3 = 0;
    int num4 = 0;
    for (int index = 0; index < copyHandToDeckSpell.m_targets.Count; ++index)
    {
      Entity entity = copyHandToDeckSpell.m_targets[index].GetComponent<Card>().GetEntity();
      bool flag = entity.IsControlledByFriendlySidePlayer();
      copyHandToDeckSpell.m_actors.Add((Actor) null);
      if (flag)
        copyHandToDeckSpell.m_friendlyActors.Add((Actor) null);
      else
        copyHandToDeckSpell.m_opposingActors.Add((Actor) null);
      string zoneActor = ActorNames.GetZoneActor(entity, TAG_ZONE.HAND);
      DeadManCopyHandToDeckSpell.ActorCallbackData callbackData = new DeadManCopyHandToDeckSpell.ActorCallbackData()
      {
        targetIndex = index,
        handIndex = flag ? num3++ : num4++,
        handSize = flag ? num1 : num2
      };
      AssetLoader.Get().InstantiatePrefab((AssetReference) zoneActor, new PrefabCallback<GameObject>(copyHandToDeckSpell.OnActorLoaded), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
    }
    while (copyHandToDeckSpell.m_numActorsInLoading > 0)
      yield return (object) null;
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    --this.m_numActorsInLoading;
    DeadManCopyHandToDeckSpell.ActorCallbackData actorCallbackData = (DeadManCopyHandToDeckSpell.ActorCallbackData) callbackData;
    int targetIndex = actorCallbackData.targetIndex;
    int handIndex = actorCallbackData.handIndex;
    int handSize = actorCallbackData.handSize;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("DeadManCopyHandToDeckSpell.OnActorLoaded() - FAILED to load actor {0} (targetIndex {1})", (object) assetRef, (object) targetIndex);
    }
    else
    {
      Actor component1 = go.GetComponent<Actor>();
      Card component2 = this.m_targets[targetIndex].GetComponent<Card>();
      Entity entity = component2.GetEntity();
      ZoneHand handZone = component2.GetController().GetHandZone();
      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && !component2.HasCardDef && entity != null)
      {
        string cardId = entity.GetCardId();
        if (!string.IsNullOrEmpty(cardId))
          component2.SetCardDef(DefLoader.Get().GetCardDef(cardId), false);
      }
      component2.SetDoNotSort(true);
      component1.SetCard(component2);
      component1.SetCardDefFromCard(component2);
      component1.SetEntity(entity);
      component1.SetEntityDef(entity.GetEntityDef());
      component1.SetCardBackSideOverride(new Player.Side?(entity.GetControllerSide()));
      component1.UpdateAllComponents();
      component2.transform.position = handZone.GetCardPosition(handIndex, handSize);
      component2.transform.localEulerAngles = handZone.GetCardRotation(handIndex, handSize);
      component2.transform.localScale = handZone.GetCardScale();
      component1.Hide();
      this.m_actors[targetIndex] = component1;
      if (entity.IsControlledByFriendlySidePlayer())
        this.m_friendlyActors[handIndex] = component1;
      else
        this.m_opposingActors[handIndex] = component1;
    }
  }

  private IEnumerator WaitForPendingCardDraw()
  {
    Card sourceCard = this.GetSourceCard();
    if (!((UnityEngine.Object) sourceCard == (UnityEngine.Object) null))
    {
      Entity entity = sourceCard.GetEntity();
      if (entity != null)
      {
        if (entity.IsControlledByFriendlySidePlayer())
        {
          while ((bool) (UnityEngine.Object) GameState.Get().GetFriendlyCardBeingDrawn())
            yield return (object) null;
          while (GameState.Get().GetFriendlySidePlayer().GetHandZone().IsUpdatingLayout())
            yield return (object) null;
        }
        else
        {
          while ((bool) (UnityEngine.Object) GameState.Get().GetOpponentCardBeingDrawn())
            yield return (object) null;
          while (GameState.Get().GetOpposingSidePlayer().GetHandZone().IsUpdatingLayout())
            yield return (object) null;
        }
      }
    }
  }

  private bool IsDrawing()
  {
    foreach (Entity entity in this.m_entitiesToDrawBeforeFX)
    {
      Card card = entity.GetCard();
      switch (entity.GetZone())
      {
        case TAG_ZONE.HAND:
          if (!(card.GetZone() is ZoneHand) || card.IsDoNotSort())
            return true;
          if (entity.IsControlledByFriendlySidePlayer())
          {
            if (!card.CardStandInIsInteractive())
              return true;
            continue;
          }
          if (card.IsBeingDrawnByOpponent())
            return true;
          continue;
        case TAG_ZONE.GRAVEYARD:
          if (!card.IsActorReady())
            return true;
          continue;
        default:
          continue;
      }
    }
    return false;
  }

  private IEnumerator WaitForTasksAndDrawing()
  {
    while (this.m_waitForTasksToComplete)
      yield return (object) null;
    while (this.IsDrawing())
      yield return (object) null;
  }

  private void CheckHideOriginalHandActors()
  {
    if (!this.m_ShuffleRealHandToDeck)
      return;
    if (this.m_opposingActors.Count > 0)
    {
      foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetHandZone().GetCards())
        card.GetActor().Hide();
    }
    if (this.m_friendlyActors.Count <= 0)
      return;
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards())
      card.GetActor().Hide();
  }

  private IEnumerator DoEffects()
  {
    DeadManCopyHandToDeckSpell copyHandToDeckSpell = this;
    // ISSUE: reference to a compiler-generated method
    copyHandToDeckSpell.\u003C\u003En__0();
    copyHandToDeckSpell.CheckHideOriginalHandActors();
    copyHandToDeckSpell.AnimateSpread();
    Actor livingActor = (Actor) null;
    do
    {
      livingActor = copyHandToDeckSpell.m_actors.Find((Predicate<Actor>) (currActor => (bool) (UnityEngine.Object) currActor));
      if ((bool) (UnityEngine.Object) livingActor)
        yield return (object) null;
    }
    while ((bool) (UnityEngine.Object) livingActor);
    --copyHandToDeckSpell.m_effectsPendingFinish;
    copyHandToDeckSpell.FinishIfPossible();
  }

  private void AnimateSpread()
  {
    for (int index = 0; index < this.m_opposingActors.Count || index < this.m_friendlyActors.Count; ++index)
    {
      if (index < this.m_opposingActors.Count)
      {
        float waitSec = (float) (this.m_opposingActors.Count - index - 1) * this.m_MoveToDeckInterval;
        this.StartCoroutine(this.AnimateActor(this.m_opposingActors[index], waitSec));
      }
      if (index < this.m_friendlyActors.Count)
      {
        float waitSec = (float) (this.m_friendlyActors.Count - index - 1) * this.m_MoveToDeckInterval;
        this.StartCoroutine(this.AnimateActor(this.m_friendlyActors[index], waitSec));
      }
    }
  }

  private IEnumerator AnimateActor(Actor actor, float waitSec)
  {
    DeadManCopyHandToDeckSpell copyHandToDeckSpell = this;
    Card card = actor.GetCard();
    Player controller = card.GetController();
    ZoneDeck deck = controller.GetDeckZone();
    actor.Show();
    iTween.MoveTo(card.gameObject, new Vector3(card.transform.position.x, card.transform.position.y, card.transform.position.z + (controller.IsFriendlySide() ? copyHandToDeckSpell.m_MoveUpOffsetZ : -copyHandToDeckSpell.m_MoveUpOffsetZ)), copyHandToDeckSpell.m_MoveUpTime);
    iTween.ScaleTo(card.gameObject, card.transform.localScale * copyHandToDeckSpell.m_MoveUpScale, copyHandToDeckSpell.m_MoveUpTime);
    yield return (object) new WaitForSeconds(copyHandToDeckSpell.m_MoveUpTime + waitSec);
    bool hideBackSide = actor.GetEntityDef().GetCardType() == TAG_CARDTYPE.INVALID;
    yield return (object) copyHandToDeckSpell.StartCoroutine(actor.GetCard().AnimatePlayToDeck(actor.gameObject, deck, hideBackSide));
    actor.Destroy();
    card.SetDoNotSort(false);
  }

  private struct ActorCallbackData
  {
    public int targetIndex;
    public int handIndex;
    public int handSize;
  }
}
