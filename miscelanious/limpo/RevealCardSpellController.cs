using Blizzard.T5.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class RevealCardSpellController : SpellController
{
  public Spell m_NoRevealedCardSpellPrefab;
  public float m_RandomSecMin = 0.1f;
  public float m_RandomSecMax = 0.25f;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_ShowSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_DrawStingerPrefab;
  public float m_ShowTime = 1.2f;
  public float m_DriftCycleTime = 10f;
  public float m_RevealTime = 0.5f;
  public iTween.EaseType m_RevealEaseType = iTween.EaseType.easeOutBack;
  public float m_HoldTime = 1.2f;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HideSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HideStingerPrefab;
  public float m_HideTime = 0.8f;
  public string m_FriendlyBoneName = "FriendlyJoust";
  public string m_OpponentBoneName = "OpponentJoust";
  private int m_hideEntityTask;
  private RevealCardSpellController.RevealedCard m_revealedCard;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    if (!this.HasSourceCard(taskList))
      return false;
    this.SetSource(taskList.GetSourceEntity().GetCard());
    this.m_hideEntityTask = -1;
    if (taskList.HasTargetEntity())
    {
      int entityId = taskList.GetTargetEntity().GetEntityId();
      List<PowerTask> taskList1 = taskList.GetTaskList();
      for (int index = 0; index < taskList1.Count; ++index)
      {
        if (taskList1[index].GetPower() is Network.HistHideEntity power && power.Entity == entityId)
          this.m_hideEntityTask = index;
      }
      if (this.m_hideEntityTask < 0)
        return false;
    }
    else if (!taskList.IsStartOfBlock())
      return false;
    return true;
  }

  protected override void OnProcessTaskList() => this.StartCoroutine(this.DoEffectWithTiming());

  private IEnumerator DoEffectWithTiming()
  {
    RevealCardSpellController cardSpellController = this;
    yield return (object) cardSpellController.StartCoroutine(cardSpellController.CompleteTasksBeforeHideEntity());
    cardSpellController.CreateRevealedCardActors();
    if (cardSpellController.m_revealedCard != null)
    {
      yield return (object) cardSpellController.StartCoroutine(cardSpellController.PullRevealedCardFromDeck());
      yield return (object) cardSpellController.StartCoroutine(cardSpellController.FlipRevealedCard());
      yield return (object) cardSpellController.StartCoroutine(cardSpellController.HideRevealedCardAnim());
      cardSpellController.DestroyRevealedCard();
    }
    else
      yield return (object) cardSpellController.StartCoroutine(cardSpellController.PlayNoRevealedCardSpell());
    // ISSUE: reference to a compiler-generated method
    cardSpellController.\u003C\u003En__0();
  }

  private IEnumerator CompleteTasksBeforeHideEntity()
  {
    RevealCardSpellController cardSpellController = this;
    if (cardSpellController.m_hideEntityTask > 0)
    {
      bool complete = false;
      PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
      cardSpellController.m_taskList.DoTasks(0, cardSpellController.m_hideEntityTask, callback);
      while (!complete)
        yield return (object) null;
    }
  }

  private void CreateRevealedCardActors()
  {
    if (!this.m_taskList.HasTargetEntity())
      return;
    Entity entity = this.m_taskList.GetTargetEntity();
    Card card = entity.GetCard();
    card.SetInputEnabled(false);
    GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("{0}.CreateRevealedCardActors() - Failed to load HIDDEN actor.", (object) this);
    }
    else
    {
      GameObject gameObject2 = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entity), AssetLoadingOptions.IgnorePrefabPosition);
      if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
      {
        Log.Gameplay.PrintError("{0}.CreateRevealedCardActors() - Failed to load HAND actor.", (object) this);
      }
      else
      {
        this.m_revealedCard = new RevealCardSpellController.RevealedCard();
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
    string name = this.m_revealedCard.m_player.GetSide() == Player.Side.FRIENDLY ? this.m_FriendlyBoneName : this.m_OpponentBoneName;
    if ((bool) UniversalInputManager.UsePhoneUI)
      name += "_phone";
    Transform bone = Board.Get().FindBone(name);
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
    RevealCardSpellController cardSpellController = this;
    float revealSec = cardSpellController.m_RevealTime + cardSpellController.GetRandomSec();
    cardSpellController.FlipRevealedCardAnim(revealSec);
    while (cardSpellController.IsRevealedCardBusy())
      yield return (object) null;
    iTween.Timer(cardSpellController.gameObject, iTween.Hash((object) "time", (object) cardSpellController.m_HoldTime));
    while (iTween.HasTween(cardSpellController.gameObject))
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

  private IEnumerator HideRevealedCardAnim()
  {
    if (!string.IsNullOrEmpty(this.m_HideStingerPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_HideStingerPrefab);
    this.HideRevealedCardAnim(this.GetRandomSec(), this.m_HideTime + this.GetRandomSec());
    while (this.IsRevealedCardBusy())
      yield return (object) null;
  }

  private void HideRevealedCardAnim(float delaySec, float hideSec)
  {
    ++this.m_revealedCard.m_effectsPendingFinish;
    Card card = this.m_revealedCard.m_card;
    ZoneDeck deck = this.m_revealedCard.m_player.GetDeckZone();
    Vector3 center = deck.GetThicknessForLayout().GetMeshRenderer().bounds.center;
    float num = 0.5f * hideSec;
    Vector3 position = card.transform.position;
    Vector3 vector3_1 = center + Card.ABOVE_DECK_OFFSET;
    Vector3 vector3_2 = center + Card.IN_DECK_OFFSET;
    Vector3 inDeckAngles = Card.IN_DECK_ANGLES;
    Vector3 inDeckScale = Card.IN_DECK_SCALE;
    Vector3[] vector3Array = new Vector3[3]
    {
      position,
      vector3_1,
      vector3_2
    };
    iTween.MoveTo(card.gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "delay", (object) delaySec, (object) "time", (object) hideSec, (object) "easetype", (object) iTween.EaseType.easeInOutQuad));
    iTween.RotateTo(card.gameObject, iTween.Hash((object) "rotation", (object) inDeckAngles, (object) "delay", (object) delaySec, (object) "time", (object) num, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    iTween.ScaleTo(card.gameObject, iTween.Hash((object) "scale", (object) inDeckScale, (object) "delay", (object) (float) ((double) delaySec + (double) num), (object) "time", (object) num, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
    if (!string.IsNullOrEmpty(this.m_HideSoundPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_HideSoundPrefab);
    Action<object> action = (Action<object>) (userData =>
    {
      --this.m_revealedCard.m_effectsPendingFinish;
      this.m_revealedCard.m_initialActor.GetCard().HideCard();
      deck.InsertCard(this.m_revealedCard.m_deckIndex, card);
      deck.UpdateLayout();
      deck.SetSuppressEmotes(false);
    });
    iTween.Timer(card.gameObject, iTween.Hash((object) "delay", (object) delaySec, (object) "time", (object) hideSec, (object) "oncomplete", (object) action));
  }

  private void DestroyRevealedCard()
  {
    this.m_revealedCard.m_card.SetInputEnabled(true);
    this.m_revealedCard.m_initialActor.Destroy();
    this.m_revealedCard.m_revealedActor.Destroy();
    this.m_revealedCard = (RevealCardSpellController.RevealedCard) null;
  }

  private IEnumerator PlayNoRevealedCardSpell()
  {
    RevealCardSpellController cardSpellController = this;
    ZoneDeck deckZone = cardSpellController.GetSource().GetController().GetDeckZone();
    if (!((UnityEngine.Object) deckZone == (UnityEngine.Object) null))
    {
      Spell noCardSpellInstance = SpellManager.Get().GetSpell(cardSpellController.m_NoRevealedCardSpellPrefab);
      if (!((UnityEngine.Object) noCardSpellInstance == (UnityEngine.Object) null))
      {
        noCardSpellInstance.SetPosition(deckZone.transform.position);
        noCardSpellInstance.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
        {
          if (spell.GetActiveState() != SpellStateType.NONE)
            return;
          SpellManager.Get().ReleaseSpell(spell);
        }));
        noCardSpellInstance.Activate();
        while (!noCardSpellInstance.IsFinished())
          yield return (object) null;
      }
    }
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
