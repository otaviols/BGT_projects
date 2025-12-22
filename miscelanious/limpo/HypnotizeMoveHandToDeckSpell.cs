using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypnotizeMoveHandToDeckSpell : SuperSpell
{
  public float m_MoveUpTime;
  public float m_MoveUpOffsetZ;
  public float m_MoveUpScale;
  public float m_MoveToDeckInterval;
  private List<Actor> m_friendlyActors = new List<Actor>();
  private List<Actor> m_opponentActors = new List<Actor>();

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
      if (!((Object) cardFromPowerTask == (Object) null) && this.IsValidSpellTarget(cardFromPowerTask.GetEntity()))
        this.AddTarget(cardFromPowerTask.gameObject);
    }
    return true;
  }

  protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
  {
    Network.PowerHistory power = task.GetPower();
    if (power.Type != Network.PowerType.TAG_CHANGE)
      return (Card) null;
    Network.HistTagChange histTagChange = power as Network.HistTagChange;
    if (histTagChange.Tag != 49)
      return (Card) null;
    if (histTagChange.Value != 2)
      return (Card) null;
    Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
    if (entity == null)
    {
      Debug.LogWarningFormat("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", (object) this, (object) histTagChange.Entity);
      return (Card) null;
    }
    return entity.GetZone() != TAG_ZONE.HAND ? (Card) null : entity.GetCard();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    this.SetActors();
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoActionWithTiming());
  }

  private void SetActors()
  {
    this.m_friendlyActors.Clear();
    this.m_opponentActors.Clear();
    InputManager.Get().DisableInput();
    for (int index = 0; index < this.m_targets.Count; ++index)
    {
      Card component = this.m_targets[index].GetComponent<Card>();
      Entity entity = component.GetEntity();
      Actor actor = component.GetActor();
      if (entity.IsControlledByFriendlySidePlayer())
        this.m_friendlyActors.Add(actor);
      else
        this.m_opponentActors.Add(actor);
    }
  }

  private int FindTaskCountToRun()
  {
    int taskCountToRun = 0;
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      if (task.GetPower().Type == Network.PowerType.SHOW_ENTITY)
        return taskCountToRun;
      ++taskCountToRun;
    }
    return 0;
  }

  private IEnumerator DoActionWithTiming()
  {
    HypnotizeMoveHandToDeckSpell moveHandToDeckSpell = this;
    yield return (object) moveHandToDeckSpell.StartCoroutine(moveHandToDeckSpell.DoMoveEffects());
    yield return (object) moveHandToDeckSpell.StartCoroutine(moveHandToDeckSpell.CompleteTasksUntilDraw());
  }

  private IEnumerator DoMoveEffects()
  {
    if (this.m_friendlyActors.Count > 0)
    {
      while ((bool) (Object) GameState.Get().GetFriendlyCardBeingDrawn())
        yield return (object) null;
      while (GameState.Get().GetFriendlySidePlayer().GetHandZone().IsUpdatingLayout())
        yield return (object) null;
      this.AnimateSpread(this.m_friendlyActors);
    }
    if (this.m_opponentActors.Count > 0)
    {
      while ((bool) (Object) GameState.Get().GetOpponentCardBeingDrawn())
        yield return (object) null;
      while (GameState.Get().GetOpposingSidePlayer().GetHandZone().IsUpdatingLayout())
        yield return (object) null;
      this.AnimateSpread(this.m_opponentActors);
    }
    while (this.m_friendlyActors.Count > 0 || this.m_opponentActors.Count > 0)
      yield return (object) null;
    InputManager.Get().EnableInput();
  }

  private void AnimateSpread(List<Actor> actors)
  {
    for (int index = 0; index < actors.Count; ++index)
    {
      float waitSec = (float) (actors.Count - index - 1) * this.m_MoveToDeckInterval;
      this.StartCoroutine(this.AnimateActor(actors, actors[index], waitSec));
    }
  }

  private IEnumerator AnimateActor(List<Actor> actors, Actor actor, float waitSec)
  {
    HypnotizeMoveHandToDeckSpell moveHandToDeckSpell = this;
    Card card = actor.GetCard();
    Player player = card.GetController();
    ZoneDeck deck = player.GetDeckZone();
    actor.Show();
    float num = player.IsFriendlySide() ? moveHandToDeckSpell.m_MoveUpOffsetZ : -moveHandToDeckSpell.m_MoveUpOffsetZ;
    Vector3 position = new Vector3(card.transform.position.x, card.transform.position.y, card.transform.position.z + num);
    iTween.MoveTo(card.gameObject, position, moveHandToDeckSpell.m_MoveUpTime);
    iTween.ScaleTo(card.gameObject, card.transform.localScale * moveHandToDeckSpell.m_MoveUpScale, moveHandToDeckSpell.m_MoveUpTime);
    yield return (object) new WaitForSeconds(moveHandToDeckSpell.m_MoveUpTime + waitSec);
    bool hideBackSide = !player.IsFriendlySide();
    yield return (object) moveHandToDeckSpell.StartCoroutine(actor.GetCard().AnimatePlayToDeck(actor.gameObject, deck, hideBackSide));
    actors.Remove(actor);
  }

  private IEnumerator CompleteTasksUntilDraw()
  {
    HypnotizeMoveHandToDeckSpell moveHandToDeckSpell = this;
    int taskCountToRun = moveHandToDeckSpell.FindTaskCountToRun();
    if (taskCountToRun <= 0)
    {
      --moveHandToDeckSpell.m_effectsPendingFinish;
      moveHandToDeckSpell.FinishIfPossible();
    }
    else
    {
      bool complete = false;
      moveHandToDeckSpell.m_taskList.DoTasks(0, taskCountToRun, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
      while (!complete)
        yield return (object) null;
      --moveHandToDeckSpell.m_effectsPendingFinish;
      moveHandToDeckSpell.FinishIfPossible();
    }
  }
}
