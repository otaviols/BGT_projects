using UnityEngine;

public class DraftCardVisual : CardSelectionHandler
{
  private Actor m_subActor;
  private bool m_chosen;

  protected override void Awake()
  {
    base.Awake();
    this.SetChosenCallback(new CardSelectionHandler.CardChosenCallback(this.ChooseThisCard));
  }

  public void SetSubActor(Actor actor) => this.m_subActor = actor;

  public Actor GetSubActor() => this.m_subActor;

  public void ChooseThisCard()
  {
    if (GameUtils.IsAnyTransitionActive() || !DraftDisplay.Get().DraftAnimationIsComplete())
      return;
    Log.Arena.Print(string.Format("Client chooses: {0} ({1})", (object) this.m_actor.GetEntityDef().GetName(), (object) this.m_actor.GetEntityDef().GetCardId()));
    if (this.m_actor.GetEntityDef().IsHeroSkin() || this.m_actor.GetEntityDef().IsHeroPower())
    {
      DraftDisplay.Get().OnHeroClicked(this.m_cardChoice);
    }
    else
    {
      this.m_chosen = true;
      DraftManager.Get().MakeChoice(this.m_cardChoice, this.m_actor.GetPremium());
    }
  }

  public bool IsChosen() => this.m_chosen;

  public void SetChosenFlag(bool bOn) => this.m_chosen = bOn;

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    base.OnOver(oldState);
    if (!((Object) this.m_subActor != (Object) null))
      return;
    this.m_subActor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    base.OnOut(oldState);
    if (!((Object) this.m_subActor != (Object) null))
      return;
    this.m_subActor.SetActorState(ActorStateType.CARD_IDLE);
  }
}
