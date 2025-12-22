using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomChoiceRevealSpell : CustomChoiceSpell
{
  public List<Transform> m_bones;
  private List<Actor> m_fakeActors;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.DoEffect();
  }

  public override void OnSpellEvent(string eventName, object eventData)
  {
    base.OnSpellEvent(eventName, eventData);
    if (!(eventName == "showCards"))
      return;
    this.StartCoroutine(this.ShowCards());
  }

  private void DoEffect()
  {
    foreach (Card card in this.m_choiceState.m_cards)
      card.SetInputEnabled(false);
    this.LoadFakeActors();
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    component.FsmVariables.GetFsmGameObject("CardA").Value = this.m_fakeActors[0].gameObject;
    component.FsmVariables.GetFsmGameObject("CardB").Value = this.m_fakeActors[1].gameObject;
    component.FsmVariables.GetFsmGameObject("CardC").Value = this.m_fakeActors[2].gameObject;
  }

  private void LoadFakeActors()
  {
    Player.Side side = this.m_choiceState.m_isFriendly ? Player.Side.OPPOSING : Player.Side.FRIENDLY;
    this.m_fakeActors = new List<Actor>();
    for (int index = 0; index < this.m_choiceState.m_cards.Count; ++index)
    {
      Actor component = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition).GetComponent<Actor>();
      component.SetCardBackSideOverride(new Player.Side?(side));
      component.UpdateAllComponents();
      TransformUtil.CopyWorld((Component) component, (Component) this.m_choiceState.m_cards[index].GetActor());
      this.m_fakeActors.Add(component);
      if (index < this.m_bones.Count)
      {
        component.transform.parent = this.m_bones[index];
        component.transform.position = this.m_bones[index].position;
      }
    }
  }

  private IEnumerator ShowCards()
  {
    CustomChoiceRevealSpell choiceRevealSpell = this;
    for (int i = 0; i < choiceRevealSpell.m_fakeActors.Count; ++i)
    {
      Card card = choiceRevealSpell.m_choiceState.m_cards[i];
      Actor fakeActor = choiceRevealSpell.m_fakeActors[i];
      if (!card.GetEntity().IsHidden())
      {
        if (i == choiceRevealSpell.m_fakeActors.Count - 1)
          yield return (object) choiceRevealSpell.StartCoroutine(SpellUtils.FlipActorAndReplaceWithCard(fakeActor, card, 0.5f));
        else
          choiceRevealSpell.StartCoroutine(SpellUtils.FlipActorAndReplaceWithCard(fakeActor, card, 0.5f));
      }
      else
      {
        card.ShowCard();
        Player.Side side = choiceRevealSpell.m_choiceState.m_isFriendly ? Player.Side.OPPOSING : Player.Side.FRIENDLY;
        card.GetActor().SetCardBackSideOverride(new Player.Side?(side));
        card.GetActor().UpdateAllComponents();
        fakeActor.Hide();
      }
    }
    choiceRevealSpell.OnSpellFinished();
    choiceRevealSpell.OnStateFinished();
    foreach (Card card in choiceRevealSpell.m_choiceState.m_cards)
      card.SetInputEnabled(true);
    foreach (Actor fakeActor in choiceRevealSpell.m_fakeActors)
      fakeActor.Destroy();
  }
}
