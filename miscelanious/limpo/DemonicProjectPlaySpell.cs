using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonicProjectPlaySpell : Spell
{
  [SerializeField]
  private string m_FriendlyBoneName = "FriendlyJoust";
  [SerializeField]
  private string m_OpponentBoneName = "OpponentJoust";
  [SerializeField]
  private float m_MoveOldCardTime = 1f;
  [SerializeField]
  private float m_ShowNewCardTime = 1f;
  [SerializeField]
  private Spell m_TransformSpell;
  private List<int> m_newEntityIDs = new List<int>();
  private Actor[] m_newActors = new Actor[2];
  private int m_numNewActorsInLoading;
  private int m_numOldActorsInMoving;
  private List<Spell> m_activeSpells = new List<Spell>();

  protected override void OnAction(SpellStateType prevStateType)
  {
    InputManager.Get().DisableInput();
    this.StartCoroutine(this.DoEffectWithTiming());
    base.OnAction(prevStateType);
  }

  public override void OnSpellFinished()
  {
    InputManager.Get().EnableInput();
    base.OnSpellFinished();
  }

  private IEnumerator DoEffectWithTiming()
  {
    DemonicProjectPlaySpell projectPlaySpell = this;
    projectPlaySpell.AddNewEntities();
    yield return (object) projectPlaySpell.StartCoroutine(projectPlaySpell.CompleteTasksBeforeSetAside());
    yield return (object) projectPlaySpell.StartCoroutine(projectPlaySpell.LoadAssets());
    yield return (object) projectPlaySpell.StartCoroutine(projectPlaySpell.MoveOldCards());
    yield return (object) projectPlaySpell.StartCoroutine(projectPlaySpell.PlayTransformFX());
    yield return (object) projectPlaySpell.StartCoroutine(projectPlaySpell.ShowNewCards());
    yield return (object) projectPlaySpell.StartCoroutine(projectPlaySpell.SwitchToRealCards());
    yield return (object) projectPlaySpell.StartCoroutine(projectPlaySpell.WaitAndDeactivate());
  }

  private void AddNewEntities()
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      Network.PowerHistory power = taskList[index].GetPower();
      if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        Network.HistFullEntity histFullEntity = (Network.HistFullEntity) power;
        Network.Entity.Tag tag = histFullEntity.Entity.Tags.Find((Predicate<Network.Entity.Tag>) (item => item.Name == 49));
        if (tag != null && tag.Value == 6)
          this.m_newEntityIDs.Add(histFullEntity.Entity.ID);
      }
    }
  }

  private int FindLastFullEntityTaskIndex()
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = taskList.Count - 1; index >= 0; --index)
    {
      if (taskList[index].GetPower().Type == Network.PowerType.FULL_ENTITY)
        return index;
    }
    return -1;
  }

  private IEnumerator CompleteTasksBeforeSetAside()
  {
    DemonicProjectPlaySpell projectPlaySpell = this;
    int fullEntityTaskIndex = projectPlaySpell.FindLastFullEntityTaskIndex();
    if (fullEntityTaskIndex == -1)
    {
      projectPlaySpell.OnSpellFinished();
    }
    else
    {
      int total = fullEntityTaskIndex + 1;
      projectPlaySpell.m_taskList.DoTasks(0, total);
      List<PowerTask> powerTaskList = projectPlaySpell.m_taskList.GetTaskList();
      for (int i = 0; i < total; ++i)
      {
        PowerTask task = powerTaskList[i];
        while (!task.IsCompleted())
          yield return (object) null;
        task = (PowerTask) null;
      }
    }
  }

  private IEnumerator LoadAssets()
  {
    DemonicProjectPlaySpell projectPlaySpell = this;
    Entity entity = projectPlaySpell.GetSourceCard().GetEntity();
    projectPlaySpell.LoadActor(GAME_TAG.TAG_SCRIPT_DATA_ENT_1, GAME_TAG.TAG_SCRIPT_DATA_NUM_1, entity.IsControlledByFriendlySidePlayer());
    projectPlaySpell.LoadActor(GAME_TAG.TAG_SCRIPT_DATA_ENT_2, GAME_TAG.TAG_SCRIPT_DATA_NUM_2, !entity.IsControlledByFriendlySidePlayer());
    if (projectPlaySpell.m_numNewActorsInLoading == 0)
    {
      projectPlaySpell.OnSpellFinished();
    }
    else
    {
      while (projectPlaySpell.m_numNewActorsInLoading > 0)
        yield return (object) null;
    }
  }

  private void LoadActor(GAME_TAG tagDataEntity, GAME_TAG tagDataNum, bool friendly)
  {
    Entity entity1 = this.GetSourceCard().GetEntity();
    if (!entity1.HasTag(tagDataNum))
      return;
    ++this.m_numNewActorsInLoading;
    Entity entity2 = GameState.Get().GetEntity(entity1.GetTag(tagDataEntity));
    int tag = entity1.GetTag(tagDataNum);
    TAG_PREMIUM premiumType = entity2.HasTag(GAME_TAG.PREMIUM) || entity1.HasTag(GAME_TAG.PREMIUM) ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL;
    Player.Side side = friendly ? Player.Side.FRIENDLY : Player.Side.OPPOSING;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
    string handActor = ActorNames.GetHandActor(entityDef, premiumType);
    DemonicProjectPlaySpell.ActorLoadData callbackData = new DemonicProjectPlaySpell.ActorLoadData()
    {
      entityDef = entityDef,
      playerSide = side,
      premium = premiumType
    };
    AssetLoader.Get().InstantiatePrefab((AssetReference) handActor, new PrefabCallback<GameObject>(this.OnActorLoaded), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    --this.m_numNewActorsInLoading;
    Actor component = go.GetComponent<Actor>();
    DemonicProjectPlaySpell.ActorLoadData actorLoadData = (DemonicProjectPlaySpell.ActorLoadData) callbackData;
    component.SetEntityDef(actorLoadData.entityDef);
    component.SetPremium(actorLoadData.premium);
    component.SetCardBackSideOverride(new Player.Side?(actorLoadData.playerSide));
    component.UpdateAllComponents();
    component.Hide();
    this.m_newActors[(int) (actorLoadData.playerSide - 1)] = component;
  }

  private IEnumerator MoveOldCards()
  {
    DemonicProjectPlaySpell projectPlaySpell = this;
    projectPlaySpell.MoveOldCard(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
    projectPlaySpell.MoveOldCard(GAME_TAG.TAG_SCRIPT_DATA_ENT_2);
    if (projectPlaySpell.m_numOldActorsInMoving == 0)
    {
      projectPlaySpell.OnSpellFinished();
    }
    else
    {
      while (projectPlaySpell.m_numOldActorsInMoving > 0)
        yield return (object) null;
    }
  }

  private void MoveOldCard(GAME_TAG tag)
  {
    Entity entity1 = this.GetSourceCard().GetEntity();
    if (!entity1.HasTag(tag))
      return;
    ++this.m_numOldActorsInMoving;
    Entity entity2 = GameState.Get().GetEntity(entity1.GetTag(tag));
    Card card = entity2.GetCard();
    if (entity2.IsControlledByOpposingSidePlayer())
    {
      string handActor = ActorNames.GetHandActor(entity2);
      card.UpdateActor(actorPath: handActor);
    }
    string name = tag != GAME_TAG.TAG_SCRIPT_DATA_ENT_1 ? (entity1.IsControlledByFriendlySidePlayer() ? this.m_OpponentBoneName : this.m_FriendlyBoneName) : (entity1.IsControlledByFriendlySidePlayer() ? this.m_FriendlyBoneName : this.m_OpponentBoneName);
    if ((bool) UniversalInputManager.UsePhoneUI)
      name += "_phone";
    Transform bone = Board.Get().FindBone(name);
    Vector3 localScale = bone.localScale;
    Vector3 position = bone.position;
    Quaternion rotation = bone.rotation;
    Action<object> action = (Action<object>) (tweenUserData => --this.m_numOldActorsInMoving);
    iTween.MoveTo(card.gameObject, iTween.Hash((object) "position", (object) position, (object) "time", (object) this.m_MoveOldCardTime, (object) "easetype", (object) iTween.EaseType.easeInOutQuart, (object) "oncomplete", (object) action));
    iTween.RotateTo(card.gameObject, iTween.Hash((object) "rotation", (object) rotation.eulerAngles, (object) "time", (object) this.m_MoveOldCardTime, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    iTween.ScaleTo(card.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) this.m_MoveOldCardTime, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
  }

  private IEnumerator PlayTransformFX()
  {
    this.ActivateTransformSpell(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
    this.ActivateTransformSpell(GAME_TAG.TAG_SCRIPT_DATA_ENT_2);
    foreach (Spell spell in this.m_activeSpells)
    {
      while (!spell.IsFinished())
        yield return (object) null;
    }
  }

  private void ActivateTransformSpell(GAME_TAG tag)
  {
    Entity entity1 = this.GetSourceCard().GetEntity();
    if (!entity1.HasTag(tag))
      return;
    Spell spell = SpellManager.Get().GetSpell(this.m_TransformSpell);
    Entity entity2 = GameState.Get().GetEntity(entity1.GetTag(tag));
    spell.SetSource(entity2.GetCard().gameObject);
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished));
    spell.ActivateState(SpellStateType.ACTION);
    this.m_activeSpells.Add(spell);
  }

  private void OnSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    this.m_activeSpells.Remove(spell);
    UnityEngine.Object.Destroy((UnityEngine.Object) spell);
  }

  private IEnumerator ShowNewCards()
  {
    this.ShowNewCard(Player.Side.FRIENDLY);
    this.ShowNewCard(Player.Side.OPPOSING);
    yield return (object) new WaitForSeconds(this.m_ShowNewCardTime);
  }

  private void ShowNewCard(Player.Side side)
  {
    Actor newActor = this.m_newActors[(int) (side - 1)];
    if ((UnityEngine.Object) newActor == (UnityEngine.Object) null)
      return;
    Entity entity1 = this.GetSourceCard().GetEntity();
    GAME_TAG enumTag = !entity1.IsControlledByFriendlySidePlayer() ? (side == Player.Side.FRIENDLY ? GAME_TAG.TAG_SCRIPT_DATA_ENT_2 : GAME_TAG.TAG_SCRIPT_DATA_ENT_1) : (side == Player.Side.FRIENDLY ? GAME_TAG.TAG_SCRIPT_DATA_ENT_1 : GAME_TAG.TAG_SCRIPT_DATA_ENT_2);
    Entity entity2 = GameState.Get().GetEntity(entity1.GetTag(enumTag));
    TransformUtil.CopyWorld(newActor.gameObject, entity2.GetCard().gameObject);
    entity2.GetCard().TransitionToZone((Zone) null);
    newActor.Show();
  }

  private IEnumerator SwitchToRealCards()
  {
    DemonicProjectPlaySpell projectPlaySpell = this;
    foreach (int newEntityId in projectPlaySpell.m_newEntityIDs)
    {
      Entity entity = GameState.Get().GetEntity(newEntityId);
      Card card = entity.GetCard();
      card.SetDoNotSort(true);
      card.SetDoNotWarpToNewZone(true);
      card.TransitionToZone((Zone) entity.GetController().GetHandZone());
      int index = entity.IsControlledByFriendlySidePlayer() ? 0 : 1;
      Actor actor = projectPlaySpell.m_newActors[index];
      while (card.IsActorLoading())
        yield return (object) null;
      actor.Hide();
      TransformUtil.CopyWorld(card.gameObject, actor.gameObject);
      card.SetDoNotSort(false);
      card.SetDoNotWarpToNewZone(false);
      card = (Card) null;
      actor = (Actor) null;
    }
    projectPlaySpell.OnSpellFinished();
  }

  private IEnumerator WaitAndDeactivate()
  {
    DemonicProjectPlaySpell projectPlaySpell = this;
    while (projectPlaySpell.m_activeSpells.Count > 0)
      yield return (object) null;
    foreach (UnityEngine.Object newActor in projectPlaySpell.m_newActors)
      UnityEngine.Object.Destroy(newActor);
    if (projectPlaySpell.m_newEntityIDs != null)
      projectPlaySpell.m_newEntityIDs.Clear();
    projectPlaySpell.Deactivate();
  }

  private class ActorLoadData
  {
    public EntityDef entityDef;
    public Player.Side playerSide;
    public TAG_PREMIUM premium;
  }
}
