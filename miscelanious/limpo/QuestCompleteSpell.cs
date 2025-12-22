using HutongGames.PlayMaker;
using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCompleteSpell : Spell
{
  [Header("Quest card animation settings")]
  public float m_QuestCardScaleTime = 1f;
  public float m_QuestCardHoldTime = 1f;
  public Transform m_QuestStartBone;
  public Transform m_OpponentQuestStartBone;
  public Transform m_QuestEndBone;
  [Header("Quest reward - Default animation settings")]
  public float m_QuestRewardHoldTime = 1f;
  public AnimationEventDispatcher m_AnimationEventDispatcher;
  public Transform m_QuestRewardBone;
  [Header("Quest reward - Custom animation settings")]
  public Spell m_CustomRewardSpellPrefab;
  private Entity m_originalQuestEntity;
  private Actor m_fakeQuestActor;
  private Actor m_fakeQuestRewardActor;
  private Entity m_questReward;
  private int m_questRewardSpawnTaskIndex;

  public override bool AddPowerTargets()
  {
    if (!this.CanAddPowerTargets() || this.m_taskList.GetBlockType() != HistoryBlock.Type.TRIGGER)
      return false;
    this.m_originalQuestEntity = this.m_taskList.GetSourceEntity(false);
    if (!this.m_originalQuestEntity.IsQuest() && !this.m_originalQuestEntity.IsQuestline())
    {
      Log.Spells.PrintError("QuestCompleteSpell.AddPowerTargets(): QuestCompleteSpell has been hooked up to a Card that is not a quest!");
      return false;
    }
    return this.FindQuestRewardFullEntityTask() && this.LoadFakeQuestActors();
  }

  private bool FindQuestRewardFullEntityTask()
  {
    List<PowerTask> taskList = this.m_taskList.GetTaskList();
    for (int index = 0; index < taskList.Count; ++index)
    {
      if (taskList[index].GetPower() is Network.HistFullEntity power)
      {
        this.m_questRewardSpawnTaskIndex = index;
        this.m_questReward = GameState.Get().GetEntity(power.Entity.ID);
        Log.Spells.PrintDebug("QuestCompleteSpell.FindQuestRewardFullEntityTask(): Found reward at task index:{0}, entityId:{1}", (object) this.m_questRewardSpawnTaskIndex, (object) power.Entity.ID);
        return true;
      }
    }
    return false;
  }

  private bool LoadFakeQuestActors() => this.LoadFakeQuestActor() && (!((Object) this.m_CustomRewardSpellPrefab == (Object) null) || this.LoadFakeQuestRewardActor());

  private bool LoadFakeQuestActor()
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(this.m_originalQuestEntity), AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Log.Spells.PrintError("QuestCompleteSpell.LoadFakeQuestActor(): Unable to load hand actor for entity {0}.", (object) this.m_originalQuestEntity);
      return false;
    }
    this.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmGameObject("RewardCard").Value = gameObject;
    this.m_fakeQuestActor = gameObject.GetComponentInChildren<Actor>();
    this.m_fakeQuestActor.SetEntity(this.m_originalQuestEntity);
    this.m_fakeQuestActor.SetCardDefFromEntity(this.m_originalQuestEntity);
    this.m_fakeQuestActor.SetPremium(this.m_originalQuestEntity.GetPremiumType());
    this.m_fakeQuestActor.SetWatermarkCardSetOverride(this.m_originalQuestEntity.GetWatermarkCardSetOverride());
    this.m_fakeQuestActor.UpdateAllComponents();
    this.m_fakeQuestActor.Hide();
    return true;
  }

  private bool LoadFakeQuestRewardActor()
  {
    string cardId = "";
    if (this.m_originalQuestEntity.IsQuest())
      cardId = QuestController.GetRewardCardIDFromQuestCardID(this.m_originalQuestEntity);
    else if (this.m_originalQuestEntity.IsQuestline())
      cardId = QuestlineController.GetRewardCardIDFromQuestCardID(this.m_originalQuestEntity);
    if (string.IsNullOrEmpty(cardId))
    {
      Log.Spells.PrintError("QuestCompleteSpell.LoadFakeQuestRewardActor(): No reward card ID found for quest card ID {0}.", (object) this.m_originalQuestEntity.GetCardId());
      return false;
    }
    if (this.m_questReward.GetCardId() != cardId)
      return false;
    using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardId))
    {
      if ((Object) fullDef?.CardDef == (Object) null || fullDef?.EntityDef == null)
      {
        Log.Spells.PrintError("QuestCompleteSpell.LoadFakeQuestRewardActor(): Unable to load def for card ID {0}.", (object) cardId);
        return false;
      }
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef, this.m_originalQuestEntity.GetPremiumType()), AssetLoadingOptions.IgnorePrefabPosition);
      if ((Object) gameObject == (Object) null)
      {
        Log.Spells.PrintError("QuestCompleteSpell.LoadFakeQuestRewardActor(): Unable to load Hand Actor for entity def {0}.", (object) fullDef.EntityDef);
        return false;
      }
      FsmGameObject fsmGameObject = this.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmGameObject("QuestSecondCard");
      if (fsmGameObject != null)
        fsmGameObject.Value = gameObject;
      this.m_fakeQuestRewardActor = gameObject.GetComponentInChildren<Actor>();
      this.m_fakeQuestRewardActor.SetFullDef(fullDef);
      this.m_fakeQuestRewardActor.SetPremium(this.m_originalQuestEntity.GetPremiumType());
      this.m_fakeQuestRewardActor.SetCardBackSideOverride(new Player.Side?(this.m_originalQuestEntity.GetControllerSide()));
      this.m_fakeQuestRewardActor.SetWatermarkCardSetOverride(this.m_originalQuestEntity.GetWatermarkCardSetOverride());
      this.m_fakeQuestRewardActor.UpdateDynamicTextFromQuestEntity(this.m_originalQuestEntity);
      this.m_fakeQuestRewardActor.UpdateAllComponents();
      this.m_fakeQuestRewardActor.Hide();
      TransformUtil.CopyWorld((Component) this.m_fakeQuestRewardActor, (Component) this.m_QuestRewardBone);
      return true;
    }
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.m_originalQuestEntity.GetCard().HideCard();
    this.StartCoroutine(this.ScaleUpFakeQuestActor());
  }

  private IEnumerator ScaleUpFakeQuestActor()
  {
    QuestCompleteSpell questCompleteSpell = this;
    questCompleteSpell.m_fakeQuestActor.Show();
    Transform source = questCompleteSpell.m_originalQuestEntity.GetControllerSide() == Player.Side.FRIENDLY ? questCompleteSpell.m_QuestStartBone : questCompleteSpell.m_OpponentQuestStartBone;
    if ((Object) source != (Object) null && (Object) questCompleteSpell.m_QuestEndBone != (Object) null)
    {
      TransformUtil.CopyWorld((Component) questCompleteSpell.m_fakeQuestActor, (Component) source);
      iTween.MoveTo(questCompleteSpell.m_fakeQuestActor.gameObject, questCompleteSpell.m_QuestEndBone.position, questCompleteSpell.m_QuestCardScaleTime);
      iTween.ScaleTo(questCompleteSpell.m_fakeQuestActor.gameObject, questCompleteSpell.m_QuestEndBone.localScale, questCompleteSpell.m_QuestCardScaleTime);
    }
    yield return (object) new WaitForSeconds(questCompleteSpell.m_QuestCardScaleTime + questCompleteSpell.m_QuestCardHoldTime);
    questCompleteSpell.ActivateState(SpellStateType.DEATH);
  }

  protected override void OnDeath(SpellStateType prevStateType)
  {
    base.OnDeath(prevStateType);
    if ((Object) this.m_CustomRewardSpellPrefab != (Object) null)
    {
      Log.Spells.PrintDebug("QuestCompleteSpell.OnDeath(): Register custom reward spell");
      this.m_AnimationEventDispatcher.RegisterAnimationEventListener(new OnAnimationEvent(this.OnCustomAnimationEvent));
    }
    else
    {
      Log.Spells.PrintDebug("QuestCompleteSpell.OnDeath(): Register default reward spell");
      this.m_AnimationEventDispatcher.RegisterAnimationEventListener(new OnAnimationEvent(this.OnDefaultAnimationEvent));
    }
  }

  private IEnumerator WaitForRewardActor()
  {
    QuestCompleteSpell questCompleteSpell = this;
    Card questRewardCard = questCompleteSpell.m_questReward.GetCard();
    questRewardCard.SetDoNotSort(true);
    questRewardCard.SetDoNotWarpToNewZone(true);
    questRewardCard.HideCard();
    Log.Spells.PrintDebug("QuestCompleteSpell.WaitForRewardActor(): Start processing tasks up to reward task");
    questCompleteSpell.m_taskList.DoTasks(0, questCompleteSpell.m_questRewardSpawnTaskIndex + 1);
    while ((Object) questRewardCard.GetActor() == (Object) null || questRewardCard.IsActorLoading())
    {
      Log.Spells.PrintDebug("QuestCompleteSpell.WaitForRewardActor(): hasActor: {0}, IsActorLoading:{1}", (object) ((Object) questRewardCard.GetActor() != (Object) null), (object) questRewardCard.IsActorLoading());
      yield return (object) null;
    }
  }

  private void OnCustomAnimationEvent(Object obj)
  {
    this.m_AnimationEventDispatcher.UnregisterAnimationEventListener(new OnAnimationEvent(this.OnCustomAnimationEvent));
    this.m_fakeQuestActor.Hide();
    this.StartCoroutine(this.RunCustomRewardAnimation());
  }

  private IEnumerator RunCustomRewardAnimation()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    QuestCompleteSpell questCompleteSpell = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Log.Spells.PrintDebug("QuestCompleteSpell.RunCustomRewardAnimation(): Reward actor ready");
      Card card = questCompleteSpell.m_questReward.GetCard();
      Transform transformForCard = card.GetZone().GetZoneTransformForCard(card);
      TransformUtil.CopyWorld((Component) card, (Component) transformForCard);
      Spell spell = SpellManager.Get().GetSpell(questCompleteSpell.m_CustomRewardSpellPrefab);
      SpellUtils.SetCustomSpellParent(spell, (Component) card.GetActor());
      spell.SetSource(card.gameObject);
      spell.AddFinishedCallback(new Spell.FinishedCallback(questCompleteSpell.OnCustomRewardSpellFinished));
      spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(questCompleteSpell.OnCustomRewardSpellStateFinished));
      spell.Activate();
      Log.Spells.PrintDebug("QuestCompleteSpell.RunCustomRewardAnimation(): Activated custom spell");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) questCompleteSpell.WaitForRewardActor();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void OnCustomRewardSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    Log.Spells.PrintDebug("QuestCompleteSpell.OnCustomRewardSpellStateFinished(): NONE state reached");
    SpellManager.Get().ReleaseSpell(spell);
    this.OnStateFinished();
  }

  private void OnCustomRewardSpellFinished(Spell spell, object userData)
  {
    Log.Spells.PrintDebug("QuestCompleteSpell.OnCustomRewardSpellFinished()");
    Card card = this.m_questReward.GetCard();
    card.SetDoNotSort(false);
    card.SetDoNotWarpToNewZone(false);
    card.GetZone().UpdateLayout();
    if (card.GetZone() is ZoneHeroPower)
    {
      card.DisableHeroPowerFlipSoundOnce();
      card.ActivateStateSpells();
    }
    this.OnSpellFinished();
  }

  private void OnDefaultAnimationEvent(Object obj)
  {
    this.m_AnimationEventDispatcher.UnregisterAnimationEventListener(new OnAnimationEvent(this.OnDefaultAnimationEvent));
    this.m_fakeQuestRewardActor.Show();
    this.m_fakeQuestActor.Hide();
    this.StartCoroutine(this.MoveRewardToHand());
  }

  private IEnumerator MoveRewardToHand()
  {
    QuestCompleteSpell questCompleteSpell = this;
    yield return (object) new WaitForSeconds(questCompleteSpell.m_QuestRewardHoldTime);
    if (questCompleteSpell.m_questReward.GetZone() != TAG_ZONE.SETASIDE)
    {
      yield return (object) questCompleteSpell.WaitForRewardActor();
      Card questRewardCard = questCompleteSpell.m_questReward.GetCard();
      if (questRewardCard.GetEntity().IsHidden())
      {
        yield return (object) questCompleteSpell.StartCoroutine(SpellUtils.FlipActorAndReplaceWithCard(questCompleteSpell.m_fakeQuestRewardActor, questRewardCard, 0.5f));
      }
      else
      {
        TransformUtil.CopyWorld((Component) questRewardCard, (Component) questCompleteSpell.m_fakeQuestRewardActor);
        questCompleteSpell.m_fakeQuestRewardActor.Hide();
      }
      questRewardCard.SetTransitionStyle(questRewardCard.GetControllerSide() == Player.Side.FRIENDLY ? ZoneTransitionStyle.SLOW : ZoneTransitionStyle.NORMAL);
      questRewardCard.SetDoNotSort(false);
      questRewardCard.SetDoNotWarpToNewZone(false);
      questRewardCard.GetZone().UpdateLayout();
      if (questRewardCard.GetZone() is ZoneBattlegroundQuestReward)
        questRewardCard.ActivateStateSpells();
      questRewardCard = (Card) null;
    }
    else
    {
      yield return (object) new WaitForSeconds(questCompleteSpell.m_QuestRewardHoldTime);
      questCompleteSpell.m_fakeQuestRewardActor.Hide();
    }
    questCompleteSpell.OnSpellFinished();
    questCompleteSpell.OnStateFinished();
  }
}
