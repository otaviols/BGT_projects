using Blizzard.T5.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class PlaySpellCardFromDeckSepll : Spell
{
  public float m_ShowTime = 1.2f;
  public float m_RandomSecMin = 0.1f;
  public float m_RandomSecMax = 0.25f;
  public float m_DriftCycleTime = 10f;
  public float m_RevealTime = 0.5f;
  public float m_HoldTime = 1.2f;
  public iTween.EaseType m_RevealEaseType = iTween.EaseType.easeOutBack;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_DrawStingerPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_ShowSoundPrefab;
  private PlaySpellCardFromDeckSepll.RevealedCard m_revealedCard;

  public override bool AddPowerTargets()
  {
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
        this.AddTarget(GameState.Get().GetEntity(histFullEntity.Entity.ID).GetCard().gameObject);
        return true;
      }
    }
    return false;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    this.StartCoroutine(this.DoEffectWithTiming());
    base.OnAction(prevStateType);
  }

  private IEnumerator DoEffectWithTiming()
  {
    PlaySpellCardFromDeckSepll cardFromDeckSepll = this;
    yield return (object) cardFromDeckSepll.StartCoroutine(cardFromDeckSepll.CompleteTasksUntilShowTargetEntity());
    cardFromDeckSepll.CreateRevealedCardActors();
    if (cardFromDeckSepll.m_revealedCard != null)
    {
      yield return (object) cardFromDeckSepll.StartCoroutine(cardFromDeckSepll.PullRevealedCardFromDeck());
      yield return (object) cardFromDeckSepll.StartCoroutine(cardFromDeckSepll.FlipRevealedCard());
      yield return (object) cardFromDeckSepll.StartCoroutine(cardFromDeckSepll.PlayRevealedCard());
      cardFromDeckSepll.DestroyRevealedCard();
      cardFromDeckSepll.OnSpellFinished();
    }
    else
      Log.Gameplay.PrintError("{0}.DoEffectWithTiming() - Failed to find revealed card", (object) cardFromDeckSepll);
  }

  private int FindTaskCountToRun()
  {
    Entity entity = this.GetTargetCard().GetEntity();
    int taskCountToRun = 0;
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      ++taskCountToRun;
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.SHOW_ENTITY && ((Network.HistShowEntity) power).Entity.ID == entity.GetEntityId())
        return taskCountToRun;
    }
    int num1 = 0;
    foreach (PowerTask task in this.m_taskList.GetTaskList())
    {
      ++num1;
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (histTagChange.Entity == entity.GetEntityId() && histTagChange.Tag == 49)
        {
          int num2;
          return num2 = num1 - 1;
        }
      }
    }
    Log.Gameplay.PrintError("{0}.FindTaskCountToRun() - Failed to find tasks to run.", (object) this);
    return 0;
  }

  private IEnumerator CompleteTasksUntilShowTargetEntity()
  {
    PlaySpellCardFromDeckSepll cardFromDeckSepll = this;
    int taskCountToRun = cardFromDeckSepll.FindTaskCountToRun();
    if (taskCountToRun > 0)
    {
      bool complete = false;
      cardFromDeckSepll.m_taskList.DoTasks(0, taskCountToRun, (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true));
      while (!complete)
        yield return (object) null;
    }
  }

  private void CreateRevealedCardActors()
  {
    Card card = this.GetTargetCard();
    Entity entity = card.GetEntity();
    card.SetInputEnabled(false);
    GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("{0}.CreateRevealedCardActors() - Failed to load HIDDEN actor.", (object) this);
    }
    else
    {
      string empty = string.Empty;
      string assetRef = !entity.IsControlledByOpposingSidePlayer() || !entity.IsSecret() ? ActorNames.GetHandActor(entity) : ActorNames.GetHistorySecretActor(entity);
      GameObject gameObject2 = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
      if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
      {
        Log.Gameplay.PrintError("{0}.CreateRevealedCardActors() - Failed to load HAND actor.", (object) this);
      }
      else
      {
        this.m_revealedCard = new PlaySpellCardFromDeckSepll.RevealedCard();
        this.m_revealedCard.m_player = entity.GetController();
        this.m_revealedCard.m_card = card;
        this.m_revealedCard.m_initialActor = gameObject1.GetComponent<Actor>();
        this.m_revealedCard.m_revealedActor = gameObject2.GetComponent<Actor>();
        Action<Actor> action = (Action<Actor>) (actor =>
        {
          actor.SetEntity(entity);
          actor.SetCard(card);
          actor.SetCardDefFromCard(card);
          actor.UpdateAllComponents();
          actor.Hide();
        });
        action(this.m_revealedCard.m_initialActor);
        action(this.m_revealedCard.m_revealedActor);
      }
    }
  }

  private IEnumerator PullRevealedCardFromDeck()
  {
    if (!string.IsNullOrEmpty(this.m_DrawStingerPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_DrawStingerPrefab);
    string bigCardBoneName = HistoryManager.Get().GetBigCardBoneName();
    Transform bone = Board.Get().FindBone(bigCardBoneName);
    Vector3 localScale = bone.localScale;
    Vector3 position = bone.position;
    Quaternion rotation = bone.rotation;
    float randomSec = this.GetRandomSec();
    float showSec = this.m_ShowTime + this.GetRandomSec();
    this.PullRevealedCardFromDeckAnim(localScale, rotation, position, randomSec, showSec);
    while (this.IsRevealedCardBusy())
      yield return (object) null;
  }

  private void PullRevealedCardFromDeckAnim(
    Vector3 localScale,
    Quaternion rotation,
    Vector3 position,
    float delaySec,
    float showSec)
  {
    ++this.m_revealedCard.m_effectsPendingFinish;
    Card card = this.m_revealedCard.m_card;
    ZoneDeck deckZone = this.m_revealedCard.m_player.GetDeckZone();
    Actor thicknessForLayout = deckZone.GetThicknessForLayout();
    this.m_revealedCard.m_deckIndex = deckZone.RemoveCard(card);
    deckZone.SetSuppressEmotes(true);
    deckZone.UpdateLayout();
    float num = 0.5f * showSec;
    Vector3 vector3_1 = thicknessForLayout.GetMeshRenderer().bounds.center + Card.IN_DECK_OFFSET;
    Vector3 vector3_2 = vector3_1 + Card.ABOVE_DECK_OFFSET;
    Vector3 vector3_3 = position;
    Vector3 eulerAngles = rotation.eulerAngles;
    Vector3 vector3_4 = localScale;
    Vector3[] vector3Array = new Vector3[3]
    {
      vector3_1,
      vector3_2,
      vector3_3
    };
    card.ShowCard();
    this.m_revealedCard.m_initialActor.Show();
    card.transform.position = vector3_1;
    card.transform.rotation = Card.IN_DECK_HIDDEN_ROTATION;
    card.transform.localScale = Card.IN_DECK_SCALE;
    iTween.MoveTo(card.gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "delay", (object) delaySec, (object) "time", (object) showSec, (object) "easetype", (object) iTween.EaseType.easeInOutQuart));
    iTween.RotateTo(card.gameObject, iTween.Hash((object) nameof (rotation), (object) eulerAngles, (object) "delay", (object) (float) ((double) delaySec + (double) num), (object) "time", (object) num, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    iTween.ScaleTo(card.gameObject, iTween.Hash((object) "scale", (object) vector3_4, (object) "delay", (object) (float) ((double) delaySec + (double) num), (object) "time", (object) num, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
    if (!string.IsNullOrEmpty(this.m_ShowSoundPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_ShowSoundPrefab);
    Action<object> action = (Action<object>) (tweenUserData =>
    {
      --this.m_revealedCard.m_effectsPendingFinish;
      this.DriftRevealedCard();
    });
    iTween.Timer(card.gameObject, iTween.Hash((object) "delay", (object) delaySec, (object) "time", (object) showSec, (object) "oncomplete", (object) action));
  }

  private void DriftRevealedCard()
  {
    Card card = this.m_revealedCard.m_card;
    Vector3 position = card.transform.position;
    float num1 = 0.02f * this.m_revealedCard.m_initialActor.GetMeshRenderer().bounds.size.z;
    Vector3 vector3_1 = GeneralUtils.RandomSign() * num1 * card.transform.up;
    Vector3 vector3_2 = -vector3_1;
    Vector3 vector3_3 = GeneralUtils.RandomSign() * num1 * card.transform.right;
    Vector3 vector3_4 = -vector3_3;
    List<Vector3> vector3List = new List<Vector3>();
    vector3List.Add(position + vector3_1 + vector3_3);
    vector3List.Add(position + vector3_2 + vector3_3);
    vector3List.Add(position);
    vector3List.Add(position + vector3_1 + vector3_4);
    vector3List.Add(position + vector3_2 + vector3_4);
    vector3List.Add(position);
    float num2 = this.m_DriftCycleTime + this.GetRandomSec();
    Hashtable args = iTween.Hash((object) "path", (object) vector3List.ToArray(), (object) "time", (object) num2, (object) "easetype", (object) iTween.EaseType.linear, (object) "looptype", (object) iTween.LoopType.loop);
    iTween.MoveTo(card.gameObject, args);
  }

  private IEnumerator FlipRevealedCard()
  {
    PlaySpellCardFromDeckSepll cardFromDeckSepll = this;
    float revealSec = cardFromDeckSepll.m_RevealTime + cardFromDeckSepll.GetRandomSec();
    cardFromDeckSepll.FlipRevealedCardAnim(revealSec);
    while (cardFromDeckSepll.IsRevealedCardBusy())
      yield return (object) null;
    iTween.Timer(cardFromDeckSepll.gameObject, iTween.Hash((object) "time", (object) cardFromDeckSepll.m_HoldTime));
    while (iTween.HasTween(cardFromDeckSepll.gameObject))
      yield return (object) null;
  }

  private void FlipRevealedCardAnim(float revealSec)
  {
    ++this.m_revealedCard.m_effectsPendingFinish;
    Card card = this.m_revealedCard.m_card;
    Actor hiddenActor = this.m_revealedCard.m_initialActor;
    Actor revealedActor = this.m_revealedCard.m_revealedActor;
    TransformUtil.SetEulerAngleZ(revealedActor.gameObject, -180f);
    iTween.RotateAdd(hiddenActor.gameObject, iTween.Hash((object) "z", (object) 180f, (object) "time", (object) revealSec, (object) "easetype", (object) this.m_RevealEaseType));
    iTween.RotateAdd(revealedActor.gameObject, iTween.Hash((object) "z", (object) 180f, (object) "time", (object) revealSec, (object) "easetype", (object) this.m_RevealEaseType));
    float startAngleZ = revealedActor.transform.rotation.eulerAngles.z;
    Action<object> action1 = (Action<object>) (tweenUserData =>
    {
      if ((double) Mathf.DeltaAngle(startAngleZ, revealedActor.transform.rotation.eulerAngles.z) < 90.0)
        return;
      revealedActor.Show();
      hiddenActor.Hide();
    });
    Action<object> action2 = (Action<object>) (tweenUserData =>
    {
      revealedActor.Show();
      hiddenActor.Hide();
      --this.m_revealedCard.m_effectsPendingFinish;
    });
    iTween.Timer(card.gameObject, iTween.Hash((object) "time", (object) revealSec, (object) "onupdate", (object) action1, (object) "oncomplete", (object) action2));
  }

  private IEnumerator PlayRevealedCard()
  {
    PlaySpellCardFromDeckSepll cardFromDeckSepll = this;
    Spell powerUpSpell = cardFromDeckSepll.m_revealedCard.m_revealedActor.GetSpell(SpellType.POWER_UP);
    if (!((UnityEngine.Object) powerUpSpell == (UnityEngine.Object) null))
    {
      // ISSUE: reference to a compiler-generated method
      powerUpSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(cardFromDeckSepll.\u003CPlayRevealedCard\u003Eb__22_0));
      ++cardFromDeckSepll.m_revealedCard.m_effectsPendingFinish;
      powerUpSpell.ActivateState(SpellStateType.BIRTH);
      while (cardFromDeckSepll.IsRevealedCardBusy())
        yield return (object) null;
      powerUpSpell.Deactivate();
    }
  }

  private void DestroyRevealedCard()
  {
    this.m_revealedCard.m_card.SetInputEnabled(true);
    this.m_revealedCard.m_initialActor.Destroy();
    this.m_revealedCard.m_revealedActor.Destroy();
    this.m_revealedCard = (PlaySpellCardFromDeckSepll.RevealedCard) null;
  }

  private float GetRandomSec() => UnityEngine.Random.Range(this.m_RandomSecMin, this.m_RandomSecMax);

  private bool IsRevealedCardBusy() => this.m_revealedCard != null && this.m_revealedCard.m_effectsPendingFinish > 0;

  private class RevealedCard
  {
    public Player m_player;
    public Card m_card;
    public int m_deckIndex;
    public Actor m_initialActor;
    public Actor m_revealedActor;
    public int m_effectsPendingFinish;
  }
}
