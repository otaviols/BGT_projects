using Blizzard.T5.Core.Utils;
using PegasusGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class JoustSpellController : SpellController
{
  public Spell m_WinnerSpellPrefab;
  public Spell m_LoserSpellPrefab;
  public Spell m_NoJousterSpellPrefab;
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
  private int m_joustTaskIndex;
  private const int ONE_SIDED_JOUST = 1;
  private const int TWO_SIDED_JOUST = 2;
  private int m_joustType;
  private JoustSpellController.Jouster m_friendlyJouster;
  private JoustSpellController.Jouster m_opponentJouster;
  private JoustSpellController.Jouster m_winningJouster;
  private JoustSpellController.Jouster m_sourceJouster;

  protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
  {
    if (!this.HasSourceCard(taskList))
      return false;
    this.m_joustTaskIndex = -1;
    List<PowerTask> taskList1 = taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      if (taskList1[index].GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.JOUST)
      {
        this.m_joustTaskIndex = index;
        if (power.AdditionalData != null && power.AdditionalData.Count > 0)
        {
          int num = power.AdditionalData[0];
          switch (num)
          {
            case 1:
            case 2:
              this.m_joustType = num;
              continue;
            default:
              this.m_joustType = 2;
              continue;
          }
        }
        else
          this.m_joustType = 2;
      }
    }
    if (this.m_joustTaskIndex < 0)
      return false;
    this.SetSource(taskList.GetSourceEntity().GetCard());
    return true;
  }

  protected override void OnProcessTaskList() => this.StartCoroutine(this.DoEffectWithTiming());

  private IEnumerator DoEffectWithTiming()
  {
    JoustSpellController joustSpellController = this;
    yield return (object) joustSpellController.StartCoroutine(joustSpellController.WaitForShowEntities());
    joustSpellController.CreateJousters();
    yield return (object) joustSpellController.StartCoroutine(joustSpellController.ShowJousters());
    yield return (object) joustSpellController.StartCoroutine(joustSpellController.Joust());
    yield return (object) joustSpellController.StartCoroutine(joustSpellController.HideJousters());
    joustSpellController.DestroyJousters();
    // ISSUE: reference to a compiler-generated method
    joustSpellController.\u003C\u003En__0();
  }

  private IEnumerator WaitForShowEntities()
  {
    JoustSpellController joustSpellController = this;
    bool complete = false;
    PowerTaskList.CompleteCallback callback = (PowerTaskList.CompleteCallback) ((taskList, startIndex, count, userData) => complete = true);
    joustSpellController.m_taskList.DoTasks(0, joustSpellController.m_joustTaskIndex, callback);
    while (!complete)
      yield return (object) null;
  }

  private void CreateJousters()
  {
    Network.HistMetaData power = (Network.HistMetaData) this.m_taskList.GetTaskList()[this.m_joustTaskIndex].GetPower();
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    this.m_friendlyJouster = this.CreateJouster(friendlySidePlayer, power);
    this.m_opponentJouster = this.CreateJouster(opposingSidePlayer, power);
    this.DetermineWinner(power);
    this.DetermineSourceJouster();
  }

  private JoustSpellController.Jouster CreateJouster(
    Player player,
    Network.HistMetaData metaData)
  {
    Entity entity = (Entity) null;
    foreach (int id in metaData.Info)
    {
      Entity entity1 = GameState.Get().GetEntity(id);
      if (entity1 != null && entity1.GetController() == player)
      {
        entity = entity1;
        break;
      }
    }
    if (entity == null)
      return (JoustSpellController.Jouster) null;
    Card card = entity.GetCard();
    card.SetInputEnabled(false);
    GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition);
    GameObject gameObject2 = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entity), AssetLoadingOptions.IgnorePrefabPosition);
    JoustSpellController.Jouster jouster = new JoustSpellController.Jouster();
    jouster.m_player = player;
    jouster.m_card = card;
    jouster.m_initialActor = gameObject1.GetComponent<Actor>();
    jouster.m_revealedActor = gameObject2.GetComponent<Actor>();
    Action<Actor> action = (Action<Actor>) (actor =>
    {
      actor.SetEntity(entity);
      actor.SetCard(card);
      actor.SetCardDefFromCard(card);
      actor.UpdateAllComponents();
      actor.Hide();
    });
    action(jouster.m_initialActor);
    action(jouster.m_revealedActor);
    return jouster;
  }

  private void DetermineWinner(Network.HistMetaData metaData)
  {
    Card joustWinner = GameUtils.GetJoustWinner(metaData);
    if (!(bool) (UnityEngine.Object) joustWinner)
      return;
    if (joustWinner.GetController().IsFriendlySide())
      this.m_winningJouster = this.m_friendlyJouster;
    else
      this.m_winningJouster = this.m_opponentJouster;
  }

  private void DetermineSourceJouster()
  {
    Player controller = this.GetSource().GetController();
    if (this.m_friendlyJouster != null && this.m_friendlyJouster.m_card.GetController() == controller)
    {
      this.m_sourceJouster = this.m_friendlyJouster;
    }
    else
    {
      if (this.m_opponentJouster == null || this.m_opponentJouster.m_card.GetController() != controller)
        return;
      this.m_sourceJouster = this.m_opponentJouster;
    }
  }

  private IEnumerator ShowJousters()
  {
    if (!string.IsNullOrEmpty(this.m_DrawStingerPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_DrawStingerPrefab);
    string friendlyBoneName = this.m_FriendlyBoneName;
    string opponentBoneName = this.m_OpponentBoneName;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      friendlyBoneName += "_phone";
      opponentBoneName += "_phone";
    }
    Transform bone1 = Board.Get().FindBone(friendlyBoneName);
    Transform bone2 = Board.Get().FindBone(opponentBoneName);
    Quaternion rotation = Quaternion.LookRotation(bone2.position - bone1.position);
    if (this.m_friendlyJouster != null)
    {
      Vector3 localScale = bone1.localScale;
      Vector3 position = bone1.position;
      float randomSec = this.GetRandomSec();
      float showSec = this.m_ShowTime + this.GetRandomSec();
      this.ShowJouster(this.m_friendlyJouster, localScale, rotation, position, randomSec, showSec);
    }
    else if (this.m_joustType == 2)
      this.PlayNoJousterSpell(GameState.Get().GetFriendlySidePlayer());
    if (this.m_opponentJouster != null)
    {
      Vector3 localScale = bone2.localScale;
      Vector3 position = bone2.position;
      float randomSec = this.GetRandomSec();
      float showSec = this.m_ShowTime + this.GetRandomSec();
      this.ShowJouster(this.m_opponentJouster, localScale, rotation, position, randomSec, showSec);
    }
    else if (this.m_joustType == 2)
      this.PlayNoJousterSpell(GameState.Get().GetOpposingSidePlayer());
    while (this.IsJousterBusy(this.m_friendlyJouster) || this.IsJousterBusy(this.m_opponentJouster))
      yield return (object) null;
  }

  private void ShowJouster(
    JoustSpellController.Jouster jouster,
    Vector3 localScale,
    Quaternion rotation,
    Vector3 position,
    float delaySec,
    float showSec)
  {
    ++jouster.m_effectsPendingFinish;
    Card card = jouster.m_card;
    ZoneDeck deckZone = jouster.m_player.GetDeckZone();
    Actor thicknessForLayout = deckZone.GetThicknessForLayout();
    jouster.m_deckIndex = deckZone.RemoveCard(card);
    Card firstCard = deckZone.GetFirstCard();
    deckZone.RemoveCard(firstCard);
    deckZone.SetSuppressEmotes(true);
    deckZone.UpdateLayout();
    if ((UnityEngine.Object) firstCard != (UnityEngine.Object) null)
      deckZone.InsertCard(0, firstCard);
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
    jouster.m_initialActor.Show();
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
      --jouster.m_effectsPendingFinish;
      this.DriftJouster(jouster);
    });
    iTween.Timer(card.gameObject, iTween.Hash((object) "delay", (object) delaySec, (object) "time", (object) showSec, (object) "oncomplete", (object) action));
  }

  private void PlayNoJousterSpell(Player player)
  {
    ZoneDeck deckZone = player.GetDeckZone();
    Spell spell1 = SpellManager.Get().GetSpell(this.m_NoJousterSpellPrefab);
    spell1.SetPosition(deckZone.transform.position);
    spell1.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
    {
      if (spell.GetActiveState() != SpellStateType.NONE)
        return;
      SpellManager.Get().ReleaseSpell(spell);
    }));
    spell1.Activate();
  }

  private void DriftJouster(JoustSpellController.Jouster jouster)
  {
    Card card = jouster.m_card;
    Vector3 position = card.transform.position;
    float num1 = 0.02f * jouster.m_initialActor.GetMeshRenderer().bounds.size.z;
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

  private IEnumerator Joust()
  {
    JoustSpellController joustSpellController = this;
    if (joustSpellController.m_friendlyJouster != null)
    {
      float revealSec = joustSpellController.m_RevealTime + joustSpellController.GetRandomSec();
      joustSpellController.RevealJouster(joustSpellController.m_friendlyJouster, revealSec);
    }
    if (joustSpellController.m_opponentJouster != null)
    {
      float revealSec = joustSpellController.m_RevealTime + joustSpellController.GetRandomSec();
      joustSpellController.RevealJouster(joustSpellController.m_opponentJouster, revealSec);
    }
    if (joustSpellController.m_sourceJouster != null)
    {
      while (joustSpellController.IsJousterBusy(joustSpellController.m_friendlyJouster) || joustSpellController.IsJousterBusy(joustSpellController.m_opponentJouster))
        yield return (object) null;
      Spell spellPrefab = joustSpellController.m_joustType != 1 ? (joustSpellController.m_sourceJouster == joustSpellController.m_winningJouster ? joustSpellController.m_WinnerSpellPrefab : joustSpellController.m_LoserSpellPrefab) : (!joustSpellController.m_sourceJouster.m_player.IsFriendlySide() ? joustSpellController.m_LoserSpellPrefab : (joustSpellController.m_sourceJouster == joustSpellController.m_winningJouster ? joustSpellController.m_WinnerSpellPrefab : joustSpellController.m_LoserSpellPrefab));
      joustSpellController.PlaySpellOnActor(joustSpellController.m_sourceJouster, joustSpellController.m_sourceJouster.m_revealedActor, spellPrefab);
    }
    if (joustSpellController.m_friendlyJouster != null || joustSpellController.m_opponentJouster != null)
      iTween.Timer(joustSpellController.gameObject, iTween.Hash((object) "time", (object) joustSpellController.m_HoldTime));
    while (joustSpellController.IsJousterBusy(joustSpellController.m_friendlyJouster) || joustSpellController.IsJousterBusy(joustSpellController.m_opponentJouster) || iTween.HasTween(joustSpellController.gameObject))
      yield return (object) null;
  }

  private void RevealJouster(JoustSpellController.Jouster jouster, float revealSec)
  {
    if (this.m_joustType == 1 && !this.m_sourceJouster.m_player.IsFriendlySide())
      return;
    ++jouster.m_effectsPendingFinish;
    Card card = jouster.m_card;
    Actor hiddenActor = jouster.m_initialActor;
    Actor revealedActor = jouster.m_revealedActor;
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
      --jouster.m_effectsPendingFinish;
    });
    iTween.Timer(card.gameObject, iTween.Hash((object) "time", (object) revealSec, (object) "onupdate", (object) action1, (object) "oncomplete", (object) action2));
  }

  private IEnumerator HideJousters()
  {
    if (!string.IsNullOrEmpty(this.m_HideStingerPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_HideStingerPrefab);
    if (this.m_friendlyJouster != null)
      this.HideJouster(this.m_friendlyJouster, this.GetRandomSec(), this.m_HideTime + this.GetRandomSec());
    if (this.m_opponentJouster != null)
      this.HideJouster(this.m_opponentJouster, this.GetRandomSec(), this.m_HideTime + this.GetRandomSec());
    while (this.IsJousterBusy(this.m_friendlyJouster) || this.IsJousterBusy(this.m_opponentJouster))
      yield return (object) null;
  }

  private void HideJouster(JoustSpellController.Jouster jouster, float delaySec, float hideSec)
  {
    ++jouster.m_effectsPendingFinish;
    Card card = jouster.m_card;
    ZoneDeck deck = jouster.m_player.GetDeckZone();
    Vector3 center = deck.GetThicknessForLayout().GetMeshRenderer().bounds.center;
    float num = 0.5f * hideSec;
    Vector3 position = card.transform.position;
    Vector3 vector3_1 = center + Card.ABOVE_DECK_OFFSET;
    Vector3 vector3_2 = center + Card.IN_DECK_OFFSET;
    Vector3 inDeckAngles = Card.IN_DECK_ANGLES;
    if (this.m_joustType == 1 && !this.m_sourceJouster.m_player.IsFriendlySide())
      inDeckAngles.x *= -1f;
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
      --jouster.m_effectsPendingFinish;
      jouster.m_initialActor.GetCard().HideCard();
      deck.InsertCard(jouster.m_deckIndex, card);
      deck.UpdateLayout();
      deck.SetSuppressEmotes(false);
    });
    iTween.Timer(card.gameObject, iTween.Hash((object) "delay", (object) delaySec, (object) "time", (object) hideSec, (object) "oncomplete", (object) action));
  }

  private void DestroyJousters()
  {
    if (this.m_friendlyJouster != null)
    {
      this.DestroyJouster(this.m_friendlyJouster);
      this.m_friendlyJouster = (JoustSpellController.Jouster) null;
    }
    if (this.m_opponentJouster == null)
      return;
    this.DestroyJouster(this.m_opponentJouster);
    this.m_opponentJouster = (JoustSpellController.Jouster) null;
  }

  private void DestroyJouster(JoustSpellController.Jouster jouster)
  {
    if (jouster == null)
      return;
    jouster.m_card.SetInputEnabled(true);
    jouster.m_initialActor.Destroy();
    jouster.m_revealedActor.Destroy();
  }

  private float GetRandomSec() => UnityEngine.Random.Range(this.m_RandomSecMin, this.m_RandomSecMax);

  private bool PlaySpellOnActor(
    JoustSpellController.Jouster jouster,
    Actor actor,
    Spell spellPrefab)
  {
    if (!(bool) (UnityEngine.Object) spellPrefab)
      return false;
    ++jouster.m_effectsPendingFinish;
    Card card = actor.GetCard();
    Spell spell1 = SpellManager.Get().GetSpell(spellPrefab);
    spell1.transform.parent = actor.transform;
    spell1.AddFinishedCallback((Spell.FinishedCallback) ((spell, spellUserData) => --jouster.m_effectsPendingFinish));
    spell1.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
    {
      if (spell.GetActiveState() != SpellStateType.NONE)
        return;
      SpellManager.Get().ReleaseSpell(spell);
    }));
    spell1.SetSource(card.gameObject);
    spell1.Activate();
    return true;
  }

  private bool IsJousterBusy(JoustSpellController.Jouster jouster) => jouster != null && jouster.m_effectsPendingFinish > 0;

  private class Jouster
  {
    public Player m_player;
    public Card m_card;
    public int m_deckIndex;
    public Actor m_initialActor;
    public Actor m_revealedActor;
    public int m_effectsPendingFinish;
  }
}
