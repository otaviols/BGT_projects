using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomChoiceConcealSpell : CustomChoiceSpell
{
  public Spell m_WrongChoiceSpell;
  public Spell m_CorrectChoiceSpell;
  public Spell m_SuperCorrectChoiceSpell;
  public Spell m_HiddenWrongChoiceSpell;
  public Spell m_HiddenCorrectChoiceSpell;
  public Spell m_HiddenSuperCorrectChoiceSpell;
  public Spell m_CorrectChoiceFadeAwaySpell;
  public float m_SendCardBackToOpponentsDeckDelay = 0.25f;
  private bool m_choseCorrectly;
  private Card m_correctCard;
  private Actor m_fakeCorrectCardActor;
  private Actor m_fakeCorrectCardBackActor;

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if ((Object) this.m_fakeCorrectCardActor != (Object) null)
      this.m_fakeCorrectCardActor.Destroy();
    if (!((Object) this.m_fakeCorrectCardBackActor != (Object) null))
      return;
    this.m_fakeCorrectCardBackActor.Destroy();
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.DoEffects());
  }

  private IEnumerator DoEffects()
  {
    CustomChoiceConcealSpell choiceConcealSpell = this;
    while (!choiceConcealSpell.FindResultOfChoice())
      yield return (object) null;
    if (!choiceConcealSpell.m_choseCorrectly && !choiceConcealSpell.LoadFakeActors())
    {
      choiceConcealSpell.OnSpellFinished();
      choiceConcealSpell.OnStateFinished();
    }
    else
    {
      yield return (object) choiceConcealSpell.StartCoroutine(choiceConcealSpell.PlayChoiceEffects());
      choiceConcealSpell.ResetCorrectCardCardback();
      foreach (Card card in choiceConcealSpell.m_choiceState.m_cards)
      {
        if (!((Object) card == (Object) choiceConcealSpell.m_correctCard) || !choiceConcealSpell.m_choseCorrectly)
          card.HideCard();
      }
      if (!choiceConcealSpell.m_choseCorrectly)
      {
        choiceConcealSpell.m_fakeCorrectCardActor.Show();
        yield return (object) new WaitForSeconds(choiceConcealSpell.m_SendCardBackToOpponentsDeckDelay);
        if (!choiceConcealSpell.m_fakeCorrectCardActor.GetEntity().IsHidden())
        {
          yield return (object) choiceConcealSpell.StartCoroutine(SpellUtils.FlipActorAndReplaceWithOtherActor(choiceConcealSpell.m_fakeCorrectCardActor, choiceConcealSpell.m_fakeCorrectCardBackActor, 0.5f));
        }
        else
        {
          choiceConcealSpell.m_fakeCorrectCardBackActor.Show();
          choiceConcealSpell.m_fakeCorrectCardActor.Hide();
        }
        choiceConcealSpell.PlayFadeAwaySpellThenFinish();
      }
      else
        choiceConcealSpell.FinishSpell();
    }
  }

  private void FinishSpell()
  {
    this.OnSpellFinished();
    this.OnStateFinished();
  }

  private bool LoadFakeActors()
  {
    GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_correctCard.GetActorAssetPath(), AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject1 == (Object) null)
    {
      Log.Spells.PrintError("CustomChoiceConcealSpell.LoadFakeActors(): Failed to load fake actor for card " + (object) this.m_correctCard);
      return false;
    }
    Player.Side oppositePlayerSide = Player.GetOppositePlayerSide(this.m_correctCard.GetControllerSide());
    this.m_fakeCorrectCardActor = gameObject1.GetComponent<Actor>();
    this.m_fakeCorrectCardActor.SetCardDefFromCard(this.m_correctCard);
    this.m_fakeCorrectCardActor.SetEntity(this.m_correctCard.GetEntity());
    this.m_fakeCorrectCardActor.SetEntityDef(this.m_correctCard.GetEntity().GetEntityDef());
    this.m_fakeCorrectCardActor.SetCardBackSideOverride(new Player.Side?(oppositePlayerSide));
    this.m_fakeCorrectCardActor.UpdateAllComponents();
    TransformUtil.CopyWorld((Component) this.m_fakeCorrectCardActor, (Component) this.m_correctCard.GetActor());
    this.m_fakeCorrectCardActor.Hide();
    GameObject gameObject2 = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject2 == (Object) null)
    {
      Log.Spells.PrintError("CustomChoiceConcealSpell.LoadFakeActors(): Failed to load fake card back actor.");
      return false;
    }
    this.m_fakeCorrectCardBackActor = gameObject2.GetComponent<Actor>();
    this.m_fakeCorrectCardBackActor.SetCardBackSideOverride(new Player.Side?(oppositePlayerSide));
    this.m_fakeCorrectCardBackActor.UpdateAllComponents();
    TransformUtil.CopyWorld((Component) this.m_fakeCorrectCardBackActor, (Component) this.m_correctCard.GetActor());
    this.m_fakeCorrectCardBackActor.Hide();
    return true;
  }

  private void OnEffectStateFinished(Spell spell, SpellStateType prevStateType, object userData)
  {
    if (!((Object) spell != (Object) null) || spell.GetActiveState() != SpellStateType.NONE)
      return;
    SpellManager.Get().ReleaseSpell(spell);
  }

  private void OnChoiceEffectSpellEvent(string eventName, object eventData, object userData)
  {
    if (!(eventName == "ResetCorrectCardCardBack"))
      return;
    this.ResetCorrectCardCardback();
  }

  private IEnumerator PlayChoiceEffects()
  {
    CustomChoiceConcealSpell choiceConcealSpell = this;
    int effectsToWaitFor = 0;
    Spell.FinishedCallback callback = (Spell.FinishedCallback) ((spell, userData) => --effectsToWaitFor);
    foreach (Card card in choiceConcealSpell.m_choiceState.m_cards)
    {
      int num = card.GetEntity().IsHidden() ? 1 : 0;
      Spell spell1 = num != 0 ? choiceConcealSpell.m_HiddenCorrectChoiceSpell : choiceConcealSpell.m_CorrectChoiceSpell;
      Spell spell2 = num != 0 ? choiceConcealSpell.m_HiddenSuperCorrectChoiceSpell : choiceConcealSpell.m_SuperCorrectChoiceSpell;
      Spell spell3 = num != 0 ? choiceConcealSpell.m_HiddenWrongChoiceSpell : choiceConcealSpell.m_WrongChoiceSpell;
      Actor actor = card.GetActor();
      SpellManager spellManager = SpellManager.Get();
      if ((Object) card == (Object) choiceConcealSpell.m_correctCard)
      {
        ++effectsToWaitFor;
        Spell c = choiceConcealSpell.m_choseCorrectly ? spellManager.GetSpell(spell2) : spellManager.GetSpell(spell1);
        c.transform.parent = actor.transform;
        TransformUtil.Identity((Component) c);
        c.AddFinishedCallback(callback);
        c.AddStateFinishedCallback(new Spell.StateFinishedCallback(choiceConcealSpell.OnEffectStateFinished));
        c.AddSpellEventCallback(new Spell.SpellEventCallback(choiceConcealSpell.OnChoiceEffectSpellEvent));
        c.Activate();
      }
      else
      {
        ++effectsToWaitFor;
        Spell spell4 = spellManager.GetSpell(spell3);
        spell4.transform.parent = actor.transform;
        TransformUtil.Identity((Component) spell4);
        spell4.AddFinishedCallback(callback);
        spell4.AddStateFinishedCallback(new Spell.StateFinishedCallback(choiceConcealSpell.OnEffectStateFinished));
        spell4.Activate();
      }
    }
    while (effectsToWaitFor > 0)
      yield return (object) null;
  }

  private void PlayFadeAwaySpellThenFinish()
  {
    Spell.FinishedCallback callback = (Spell.FinishedCallback) ((spell, userData) =>
    {
      this.m_fakeCorrectCardBackActor.Hide();
      this.FinishSpell();
    });
    Spell spell1 = SpellManager.Get().GetSpell(this.m_CorrectChoiceFadeAwaySpell);
    spell1.transform.parent = this.m_correctCard.GetActor().transform;
    TransformUtil.Identity((Component) spell1);
    spell1.AddFinishedCallback(callback);
    spell1.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnEffectStateFinished));
    spell1.Activate();
  }

  private void ResetCorrectCardCardback()
  {
    this.m_correctCard.GetActor().SetCardBackSideOverride(new Player.Side?());
    this.m_correctCard.GetActor().UpdateCardBack();
    if (this.m_correctCard.GetControllerSide() != Player.Side.FRIENDLY)
      return;
    this.m_correctCard.SetTransitionStyle(ZoneTransitionStyle.SLOW);
  }

  private bool FindResultOfChoice()
  {
    List<PowerTaskList> powerTaskListList = new List<PowerTaskList>();
    powerTaskListList.Add(GameState.Get().GetPowerProcessor().GetCurrentTaskList());
    foreach (PowerTaskList powerTaskList in GameState.Get().GetPowerProcessor().GetPowerQueue().GetList())
      powerTaskListList.Add(powerTaskList);
    foreach (PowerTaskList powerTaskList in powerTaskListList)
    {
      if (powerTaskList != null)
      {
        foreach (PowerTask task in powerTaskList.GetTaskList())
        {
          if (task.GetPower() is Network.HistMetaData power && power.MetaType == HistoryMeta.Type.TARGET)
          {
            Entity entity = GameState.Get().GetEntity(power.Info[0]);
            if (entity != null)
            {
              foreach (Card card in this.m_choiceState.m_cards)
              {
                if (card.GetEntity() == entity)
                {
                  this.m_correctCard = card;
                  this.m_choseCorrectly = this.m_choiceState.m_chosenEntities.Contains(card.GetEntity());
                  return true;
                }
              }
            }
          }
        }
      }
    }
    return false;
  }
}
