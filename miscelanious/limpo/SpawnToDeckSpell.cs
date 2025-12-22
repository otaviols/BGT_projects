using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnToDeckSpell : SuperSpell
{
  private const float PHONE_HAND_OFFSET = 1.5f;
  private const int SEQUENCE_BATCH_SIZE = 5;
  private const float SEQUENCE_BATCH_REVEAL_TIME = 0.3f;
  private const float SEQUENCE_BATCH_HOLD_TIME = 0.0f;
  private const float SEQUENCE_BATCH_NEXT_CARD_HOLD_TIME = 0.2f;
  public SpawnToDeckSpell.HandActorSource m_HandActorSource;
  public string m_OverrideCardId;
  public List<string> m_OverrideCardIds = new List<string>();
  public float m_CardDelay;
  public float m_CardAnimatePlayToDeckTimeScale = 1f;
  public float m_RevealStartScale = 0.1f;
  public float m_RevealYOffsetMin = 5f;
  public float m_RevealYOffsetMax = 5f;
  public float m_RevealFriendlySideZOffset;
  public float m_RevealOpponentSideZOffset;
  public Vector3 m_RevealBaseOffset = Vector3.zero;
  public SpawnToDeckSpell.SpreadType m_SpreadType;
  public SpawnToDeckSpell.StackData m_StackData = new SpawnToDeckSpell.StackData();
  public SpawnToDeckSpell.SequenceData m_SequenceData = new SpawnToDeckSpell.SequenceData();
  public Spell m_customRevealSpell;
  public bool m_VisibleByDefault = true;
  private List<DefLoader.DisposableCardDef> m_overrideCardDefs = new List<DefLoader.DisposableCardDef>();
  private List<Actor> m_loadedActors;
  [HideInInspector]
  public bool m_finishedLoading;

  protected override void OnDestroy()
  {
    this.m_overrideCardDefs.DisposeValuesAndClear<DefLoader.DisposableCardDef>();
    base.OnDestroy();
  }

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
      if (!((UnityEngine.Object) cardFromPowerTask == (UnityEngine.Object) null) && this.IsValidSpellTarget(cardFromPowerTask.GetEntity()))
        this.AddTarget(cardFromPowerTask.gameObject);
    }
    return this.m_targets.Count > 0;
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
    return entity2.GetZone() != TAG_ZONE.DECK ? (Card) null : entity2.GetCard();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    ++this.m_effectsPendingFinish;
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoActionWithTiming());
  }

  private IEnumerator ProcessShowEntityForTargets()
  {
    SpawnToDeckSpell spawnToDeckSpell = this;
    foreach (PowerTask task in spawnToDeckSpell.GetPowerTaskList().GetTaskList())
    {
      if (task.GetPower() is Network.HistShowEntity power)
      {
        Network.Entity entity = power.Entity;
        Entity target = spawnToDeckSpell.FindTargetEntity(entity.ID);
        if (target != null)
        {
          foreach (Network.Entity.Tag tag in entity.Tags)
            target.SetTag(tag.Name, tag.Value);
          target.LoadCard(entity.CardID);
          while (target.IsLoadingAssets())
            yield return (object) null;
          target = (Entity) null;
        }
      }
    }
  }

  private Entity FindTargetEntity(int entityID)
  {
    foreach (GameObject target in this.m_targets)
    {
      Card component = target.GetComponent<Card>();
      if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
      {
        Entity entity = component.GetEntity();
        if (entity != null && entity.GetEntityId() == entityID)
          return entity;
      }
    }
    Entity powerTarget = this.GetPowerTarget();
    return powerTarget != null && powerTarget.GetEntityId() == entityID ? powerTarget : (Entity) null;
  }

  private IEnumerator DoActionWithTiming()
  {
    SpawnToDeckSpell spawnToDeckSpell = this;
    spawnToDeckSpell.m_loadedActors = new List<Actor>(spawnToDeckSpell.m_targets.Count);
    yield return (object) spawnToDeckSpell.StartCoroutine(spawnToDeckSpell.ProcessShowEntityForTargets());
    yield return (object) spawnToDeckSpell.StartCoroutine(spawnToDeckSpell.LoadAssets(spawnToDeckSpell.m_loadedActors));
    yield return (object) new WaitForSeconds(spawnToDeckSpell.m_CardDelay);
    yield return (object) spawnToDeckSpell.StartCoroutine(spawnToDeckSpell.DoEffects(spawnToDeckSpell.m_loadedActors));
  }

  private IEnumerator LoadAssets(List<Actor> actors)
  {
    SpawnToDeckSpell spawnToDeckSpell = this;
    if (spawnToDeckSpell.transform.position == Vector3.zero)
    {
      Player.Side controllerSide = spawnToDeckSpell.GetSourceCard().GetControllerSide();
      spawnToDeckSpell.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(controllerSide).transform.position;
    }
    bool loadingOverrideCardDef = false;
    if (spawnToDeckSpell.m_OverrideCardIds.Count == 0)
      spawnToDeckSpell.m_OverrideCardIds.Add(spawnToDeckSpell.m_OverrideCardId);
    for (int j = 0; j < spawnToDeckSpell.m_OverrideCardIds.Count; ++j)
    {
      if (!string.IsNullOrEmpty(spawnToDeckSpell.m_OverrideCardIds[j]))
      {
        loadingOverrideCardDef = true;
        DefLoader.LoadDefCallback<DefLoader.DisposableCardDef> callback = closure_2 ?? (closure_2 = (DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>) ((cardId, def, userData) =>
        {
          loadingOverrideCardDef = false;
          if (def == null)
            Error.AddDevFatal("SpawnToDeckSpell.LoadAssets() - FAILED to load CardDef for {0}", (object) cardId);
          else
            this.m_overrideCardDefs.Add(def);
        }));
        DefLoader.Get().LoadCardDef(spawnToDeckSpell.m_OverrideCardIds[j], callback);
        while (loadingOverrideCardDef)
          yield return (object) null;
      }
      int assetsLoading = 1;
      if (j == spawnToDeckSpell.m_OverrideCardIds.Count - 1 && spawnToDeckSpell.m_targets.Count > spawnToDeckSpell.m_OverrideCardIds.Count)
        assetsLoading = spawnToDeckSpell.m_targets.Count - spawnToDeckSpell.m_OverrideCardIds.Count + 1;
      PrefabCallback<GameObject> callback1 = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
      {
        --assetsLoading;
        int index = (int) callbackData;
        if (index > this.m_targets.Count - 1)
          return;
        if ((UnityEngine.Object) go == (UnityEngine.Object) null)
        {
          Error.AddDevFatal("SpawnToDeckSpell.LoadAssets() - FAILED to load actor {0} (targetIndex {1})", (object) this.name, (object) index);
        }
        else
        {
          Actor component1 = go.GetComponent<Actor>();
          Card component2 = this.m_targets[index].GetComponent<Card>();
          Entity entity = component2.GetEntity();
          if (entity.GetLoadState() == Entity.LoadState.DONE)
          {
            component1.SetEntity(entity);
          }
          else
          {
            component1.SetPremium(this.GetPremium(entity));
            if (this.m_HandActorSource == SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET)
            {
              Entity powerTarget = this.GetPowerTarget();
              if (powerTarget != null)
              {
                string cardTextInHand = powerTarget.GetCardTextInHand();
                component1.SetCardDefPowerTextOverride(cardTextInHand);
              }
            }
          }
          if (this.m_HandActorSource != SpawnToDeckSpell.HandActorSource.ENTITY_TARGET)
            component1.SetEntityDef(this.GetEntityDef(entity, index));
          using (DefLoader.DisposableCardDef cardDef = this.ShareDisposableCardDef(component2, index))
            component1.SetCardDef(cardDef);
          component1.SetCardBackSideOverride(new Player.Side?(entity.GetControllerSide()));
          component1.UpdateAllComponents();
          component1.Hide();
          actors[index] = component1;
          this.OnActorLoaded(component1);
        }
      });
      int num = assetsLoading;
      for (int index = 0; index < num; ++index)
      {
        Entity entity = spawnToDeckSpell.m_targets[Math.Min(j + index, spawnToDeckSpell.m_targets.Count - 1)].GetComponent<Card>().GetEntity();
        TAG_PREMIUM premium = spawnToDeckSpell.GetPremium(entity);
        string assetRef = spawnToDeckSpell.GetAssetRef(entity, premium, j + index);
        if (actors.Count < spawnToDeckSpell.m_targets.Count)
          actors.Add((Actor) null);
        AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, callback1, (object) (j + index), AssetLoadingOptions.IgnorePrefabPosition);
      }
      while (assetsLoading > 0)
        yield return (object) null;
    }
    spawnToDeckSpell.m_finishedLoading = true;
  }

  protected virtual void OnActorLoaded(Actor actor)
  {
  }

  private IEnumerator DoEffects(List<Actor> actors)
  {
    SpawnToDeckSpell spawnToDeckSpell = this;
    spawnToDeckSpell.StartCoroutine(spawnToDeckSpell.AnimateSpread(actors));
    Actor livingActor = (Actor) null;
    do
    {
      livingActor = actors.Find((Predicate<Actor>) (currActor => (bool) (UnityEngine.Object) currActor));
      if ((bool) (UnityEngine.Object) livingActor)
        yield return (object) null;
    }
    while ((bool) (UnityEngine.Object) livingActor);
    --spawnToDeckSpell.m_effectsPendingFinish;
    spawnToDeckSpell.FinishIfPossible();
  }

  private float GetRevealSec(int iterationCount) => iterationCount > 0 ? 0.3f : this.m_SequenceData.m_RevealTime;

  private float GetHoldSec(int iterationCount) => iterationCount > 0 ? 0.0f : this.m_SequenceData.m_HoldTime;

  private float GetNextCardHoldSec(int iterationCount) => iterationCount > 0 ? 0.2f : this.m_SequenceData.m_NextCardHoldTime;

  private IEnumerator WaitForBatchToAnimate(int batchSize, int iterationCount)
  {
    yield return (object) new WaitForSeconds((this.m_SequenceData.m_NextCardRevealTimeMax + this.GetNextCardHoldSec(iterationCount)) * (float) (batchSize - 1) + this.GetRevealSec(iterationCount) + this.GetHoldSec(iterationCount));
  }

  private void AnimateSequence(List<Actor> actors, int iterationCount)
  {
    List<Vector3> revealPositions = new List<Vector3>();
    float num1 = -0.5f * (float) (actors.Count - 1) * this.m_SequenceData.m_Spacing;
    for (int index = 0; index < actors.Count; ++index)
    {
      float num2 = (float) index * this.m_SequenceData.m_Spacing;
      Vector3 revealPosition = this.ComputeRevealPosition(new Vector3(num1 + num2, 0.0f, 0.0f));
      revealPositions.Add(revealPosition);
    }
    this.BoundRevealPositions(actors, revealPositions);
    this.PreventHandOverlapPhone(actors, revealPositions);
    float revealSec1 = this.GetRevealSec(iterationCount);
    float holdSec = this.GetHoldSec(iterationCount);
    float nextCardHoldSec = this.GetNextCardHoldSec(iterationCount);
    List<float> floatList = this.RandomizeRevealTimes(actors.Count, revealSec1, this.m_SequenceData.m_NextCardRevealTimeMin, this.m_SequenceData.m_NextCardRevealTimeMax);
    float num3 = Mathf.Max(floatList.ToArray());
    for (int index = 0; index < actors.Count; ++index)
    {
      Vector3 revealPos = revealPositions[index];
      float revealSec2 = floatList[index];
      float num4 = (float) (actors.Count - 1 - index) * nextCardHoldSec;
      float num5 = holdSec + num4;
      float waitSec = num3 + num5;
      this.StartCoroutine(this.AnimateActor(actors, index, revealSec2, revealPos, waitSec));
    }
  }

  private IEnumerator AnimateSpread(List<Actor> actors)
  {
    SpawnToDeckSpell spawnToDeckSpell = this;
    int iterationCount;
    if (spawnToDeckSpell.m_SpreadType == SpawnToDeckSpell.SpreadType.SEQUENCE)
    {
      List<Actor> batchedActors = new List<Actor>();
      iterationCount = 0;
      foreach (Actor actor in actors)
      {
        if (batchedActors.Count == 5)
        {
          spawnToDeckSpell.AnimateSequence(batchedActors, iterationCount);
          yield return (object) spawnToDeckSpell.WaitForBatchToAnimate(batchedActors.Count, iterationCount);
          ++iterationCount;
          batchedActors.Clear();
        }
        batchedActors.Add(actor);
      }
      if (batchedActors.Count > 0)
      {
        spawnToDeckSpell.AnimateSequence(batchedActors, iterationCount);
        yield return (object) spawnToDeckSpell.WaitForBatchToAnimate(batchedActors.Count, iterationCount);
      }
      batchedActors = (List<Actor>) null;
    }
    else if (spawnToDeckSpell.m_SpreadType == SpawnToDeckSpell.SpreadType.STACK)
    {
      for (iterationCount = 0; iterationCount < actors.Count; ++iterationCount)
      {
        Vector3 revealPosition = spawnToDeckSpell.ComputeRevealPosition(Vector3.zero);
        spawnToDeckSpell.StartCoroutine(spawnToDeckSpell.AnimateActor(actors, iterationCount, spawnToDeckSpell.m_StackData.m_RevealTime, revealPosition, spawnToDeckSpell.m_StackData.m_RevealTime));
        if (iterationCount < actors.Count - 1)
          yield return (object) new WaitForSeconds(spawnToDeckSpell.m_StackData.m_StaggerTime);
      }
    }
    else if (spawnToDeckSpell.m_SpreadType == SpawnToDeckSpell.SpreadType.CUSTOM_SPELL)
    {
      for (int index = 0; index < actors.Count; ++index)
        spawnToDeckSpell.AnimateActorUsingSpell(actors, index);
    }
  }

  private void AnimateActorUsingSpell(List<Actor> actors, int index)
  {
    Actor actor = actors[index];
    GameObject target = this.m_targets[index];
    Card component = target.GetComponent<Card>();
    actor.transform.localScale = new Vector3(this.m_RevealStartScale, this.m_RevealStartScale, this.m_RevealStartScale);
    actor.transform.rotation = this.transform.rotation;
    actor.transform.position = this.transform.position;
    if (this.m_VisibleByDefault)
      actor.Show();
    SpawnToDeckSpell.RevealSpellFinishedCallbackData userData = new SpawnToDeckSpell.RevealSpellFinishedCallbackData();
    userData.actor = actor;
    userData.card = component;
    userData.id = index;
    if ((UnityEngine.Object) this.m_customRevealSpell == (UnityEngine.Object) null)
    {
      Log.Spells.PrintError("SpawnToDeckSpell.AnimateSpread(): m_SpreadType is set to CUSTOM_SPELL, but m_customRevealSpell is null!");
      this.OnRevealSpellFinished((Spell) null, (object) userData);
    }
    else
    {
      Spell spell = SpellManager.Get().GetSpell(this.m_customRevealSpell);
      SpellUtils.SetCustomSpellParent(spell, (Component) actor);
      spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnRevealSpellFinished), (object) userData);
      spell.SetSource(this.GetSource());
      spell.AddTarget(target);
      spell.Activate();
    }
  }

  public void OnRevealSpellFinished(Spell spell, object userData)
  {
    SpawnToDeckSpell.RevealSpellFinishedCallbackData finishedCallbackData = (SpawnToDeckSpell.RevealSpellFinishedCallbackData) userData;
    Actor actor = finishedCallbackData.actor;
    Card card = finishedCallbackData.card;
    Entity entity = card.GetEntity();
    ZoneDeck deckZone = entity.GetController().GetDeckZone();
    bool hideBackSide = this.GetEntityDef(entity, finishedCallbackData.id).GetCardType() == TAG_CARDTYPE.INVALID;
    this.StartCoroutine(this.AnimateRevealedCardToDeck(actor, card, deckZone, hideBackSide));
  }

  public IEnumerator AnimateRevealedCardToDeck(
    Actor actor,
    Card card,
    ZoneDeck deck,
    bool hideBackSide)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    SpawnToDeckSpell spawnToDeckSpell = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      actor.Destroy();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) spawnToDeckSpell.StartCoroutine(card.AnimatePlayToDeck(actor.gameObject, deck, hideBackSide, spawnToDeckSpell.m_CardAnimatePlayToDeckTimeScale));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private Vector3 ComputeRevealPosition(Vector3 offset)
  {
    Vector3 position = this.transform.position;
    float num = UnityEngine.Random.Range(this.m_RevealYOffsetMin, this.m_RevealYOffsetMax);
    position.y += num;
    switch (this.GetSourceCard().GetControllerSide())
    {
      case Player.Side.FRIENDLY:
        position.z += this.m_RevealFriendlySideZOffset;
        break;
      case Player.Side.OPPOSING:
        position.z += this.m_RevealOpponentSideZOffset;
        break;
    }
    return position + this.m_RevealBaseOffset + offset;
  }

  private void PreventHandOverlapPhone(List<Actor> actors, List<Vector3> revealPositions)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    Entity powerTarget = this.GetPowerTarget();
    if (powerTarget != null)
    {
      if (powerTarget.GetControllerSide() == Player.Side.OPPOSING)
        return;
    }
    else
    {
      Card sourceCard = this.GetSourceCard();
      if ((UnityEngine.Object) sourceCard != (UnityEngine.Object) null && sourceCard.GetControllerSide() == Player.Side.OPPOSING)
        return;
    }
    for (int index = 0; index < revealPositions.Count; ++index)
    {
      Vector3 vector3 = revealPositions[index];
      vector3 = new Vector3(vector3.x, vector3.y, vector3.z + 1.5f);
      revealPositions[index] = vector3;
    }
  }

  private void BoundRevealPositions(List<Actor> actors, List<Vector3> revealPositions)
  {
    float val2 = float.MinValue;
    float val1 = float.MaxValue;
    for (int index = 0; index < revealPositions.Count; ++index)
    {
      ZoneDeck deckZone = this.m_targets[index].GetComponent<Card>().GetEntity().GetController().GetDeckZone();
      float num1 = 0.0f;
      Actor activeThickness = deckZone.GetActiveThickness();
      Bounds bounds;
      if ((UnityEngine.Object) activeThickness != (UnityEngine.Object) null)
      {
        bounds = activeThickness.GetMeshRenderer().bounds;
        num1 = bounds.extents.x;
      }
      Vector3 position = deckZone.transform.position;
      position.x -= num1;
      Vector3 revealPosition1 = revealPositions[index];
      ref float local1 = ref revealPosition1.x;
      double num2 = (double) local1;
      bounds = actors[index].GetMeshRenderer().bounds;
      double x1 = (double) bounds.extents.x;
      local1 = (float) (num2 + x1);
      Vector3 revealPosition2 = revealPositions[index];
      ref float local2 = ref revealPosition2.x;
      double num3 = (double) local2;
      bounds = actors[index].GetMeshRenderer().bounds;
      double x2 = (double) bounds.extents.x;
      local2 = (float) (num3 - x2);
      Vector3 screenPoint1 = Camera.main.WorldToScreenPoint(position);
      Vector3 screenPoint2 = Camera.main.WorldToScreenPoint(revealPosition1);
      Vector3 screenPoint3 = Camera.main.WorldToScreenPoint(revealPosition2);
      float num4 = screenPoint2.x - screenPoint1.x;
      if ((double) num4 > (double) val2)
        val2 = num4;
      float x3 = screenPoint3.x;
      if ((double) x3 < (double) val1)
        val1 = x3;
    }
    if ((double) val1 >= 0.0 && (double) val2 <= 0.0)
      return;
    float worldDist = CameraUtils.ScreenToWorldDist(Camera.main, (double) val2 <= 0.0 ? Math.Max(val1, val2) : val2, revealPositions[0]);
    for (int index = 0; index < revealPositions.Count; ++index)
    {
      Vector3 revealPosition = revealPositions[index];
      revealPosition.x -= worldDist;
      revealPositions[index] = revealPosition;
    }
  }

  private List<float> RandomizeRevealTimes(
    int count,
    float revealSec,
    float nextRevealSecMin,
    float nextRevealSecMax)
  {
    List<float> floatList = new List<float>(count);
    List<int> intList = new List<int>(count);
    for (int index = 0; index < count; ++index)
    {
      floatList.Add(0.0f);
      intList.Add(index);
    }
    float num1 = revealSec;
    for (int index1 = 0; index1 < count; ++index1)
    {
      int index2 = UnityEngine.Random.Range(0, intList.Count);
      int index3 = intList[index2];
      intList.RemoveAt(index2);
      floatList[index3] = num1;
      float num2 = UnityEngine.Random.Range(nextRevealSecMin, nextRevealSecMax);
      num1 += num2;
    }
    return floatList;
  }

  private IEnumerator AnimateActor(
    List<Actor> actors,
    int index,
    float revealSec,
    Vector3 revealPos,
    float waitSec)
  {
    SpawnToDeckSpell spawnToDeckSpell = this;
    Actor actor = actors[index];
    Card card = spawnToDeckSpell.m_targets[index].GetComponent<Card>();
    Entity entity = card.GetEntity();
    Player controller = entity.GetController();
    ZonePlay battlefieldZone = controller.GetBattlefieldZone();
    ZoneDeck deck = controller.GetDeckZone();
    actor.transform.localScale = new Vector3(spawnToDeckSpell.m_RevealStartScale, spawnToDeckSpell.m_RevealStartScale, spawnToDeckSpell.m_RevealStartScale);
    actor.transform.rotation = spawnToDeckSpell.transform.rotation;
    actor.transform.position = spawnToDeckSpell.transform.position;
    if (spawnToDeckSpell.m_VisibleByDefault)
      actor.Show();
    Vector3 one = Vector3.one;
    Vector3 eulerAngles = battlefieldZone.transform.rotation.eulerAngles;
    iTween.MoveTo(actor.gameObject, iTween.Hash((object) "position", (object) revealPos, (object) "time", (object) revealSec, (object) "easetype", (object) iTween.EaseType.easeOutExpo));
    iTween.RotateTo(actor.gameObject, iTween.Hash((object) "rotation", (object) eulerAngles, (object) "time", (object) revealSec, (object) "easetype", (object) iTween.EaseType.easeOutExpo));
    iTween.ScaleTo(actor.gameObject, iTween.Hash((object) "scale", (object) one, (object) "time", (object) revealSec, (object) "easetype", (object) iTween.EaseType.easeOutExpo));
    if ((double) waitSec > 0.0)
      yield return (object) new WaitForSeconds(waitSec);
    bool hideBackSide = spawnToDeckSpell.GetEntityDef(entity, index).GetCardType() == TAG_CARDTYPE.INVALID;
    yield return (object) spawnToDeckSpell.StartCoroutine(card.AnimatePlayToDeck(actor.gameObject, deck, hideBackSide, spawnToDeckSpell.m_CardAnimatePlayToDeckTimeScale));
    actor.Destroy();
  }

  private TAG_PREMIUM GetPremium(Entity entity)
  {
    TAG_PREMIUM premiumType1 = this.GetSourceCard().GetEntity().GetPremiumType();
    switch (this.m_HandActorSource)
    {
      case SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET:
        TAG_PREMIUM premiumType2 = this.GetPowerTarget().GetPremiumType();
        return premiumType1 <= premiumType2 ? premiumType2 : premiumType1;
      case SpawnToDeckSpell.HandActorSource.OVERRIDE:
        return premiumType1;
      default:
        return entity.GetPremiumType();
    }
  }

  private string GetAssetRef(Entity entity, TAG_PREMIUM premium, int index = 0)
  {
    string handActor;
    switch (this.m_HandActorSource)
    {
      case SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET:
        handActor = ActorNames.GetHandActor(this.GetPowerTarget().GetEntityDef(), premium);
        break;
      case SpawnToDeckSpell.HandActorSource.OVERRIDE:
        handActor = ActorNames.GetHandActor(DefLoader.Get().GetEntityDef(this.m_OverrideCardIds[Math.Min(index, this.m_OverrideCardIds.Count - 1)]), premium);
        break;
      case SpawnToDeckSpell.HandActorSource.SPELL_TARGET:
        handActor = ActorNames.GetHandActor(entity.GetEntityDef(), premium);
        break;
      case SpawnToDeckSpell.HandActorSource.ENTITY_TARGET:
        handActor = ActorNames.GetHandActor(entity);
        break;
      default:
        handActor = ActorNames.GetHandActor(entity.GetEntityDef(), premium);
        break;
    }
    return handActor;
  }

  private EntityDef GetEntityDef(Entity entity, int index = 0)
  {
    switch (this.m_HandActorSource)
    {
      case SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET:
        return this.GetPowerTarget().GetEntityDef();
      case SpawnToDeckSpell.HandActorSource.OVERRIDE:
        return DefLoader.Get().GetEntityDef(this.m_OverrideCardIds[Math.Min(index, this.m_OverrideCardIds.Count - 1)]);
      default:
        return entity.GetEntityDef();
    }
  }

  private DefLoader.DisposableCardDef ShareDisposableCardDef(Card card, int index = 0)
  {
    switch (this.m_HandActorSource)
    {
      case SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET:
        return this.GetPowerTargetCard().ShareDisposableCardDef();
      case SpawnToDeckSpell.HandActorSource.OVERRIDE:
        return this.m_overrideCardDefs[Math.Min(index, this.m_overrideCardDefs.Count - 1)]?.Share();
      default:
        return card.ShareDisposableCardDef();
    }
  }

  public List<Actor> GetLoadedActors() => this.m_loadedActors;

  public enum HandActorSource
  {
    CHOSEN_TARGET,
    OVERRIDE,
    SPELL_TARGET,
    ENTITY_TARGET,
  }

  public enum SpreadType
  {
    STACK,
    SEQUENCE,
    CUSTOM_SPELL,
  }

  [Serializable]
  public class StackData
  {
    public float m_RevealTime = 1f;
    public float m_StaggerTime = 1.2f;
  }

  [Serializable]
  public class SequenceData
  {
    public float m_Spacing = 2.1f;
    public float m_RevealTime = 0.6f;
    public float m_NextCardRevealTimeMin = 0.1f;
    public float m_NextCardRevealTimeMax = 0.2f;
    public float m_HoldTime = 0.3f;
    public float m_NextCardHoldTime = 0.4f;
  }

  private struct RevealSpellFinishedCallbackData
  {
    public Actor actor;
    public Card card;
    public int id;
  }
}
