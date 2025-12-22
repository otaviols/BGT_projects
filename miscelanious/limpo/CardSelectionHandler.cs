using UnityEngine;

public class CardSelectionHandler : PegUIElement
{
  protected Actor m_actor;
  protected int m_cardChoice = -1;
  private CardSelectionHandler.CardChosenCallback m_cardChosenCallback;
  private const float MOUSE_OVER_DELAY = 0.4f;
  private float m_mouseOverTimer;

  public void SetActor(Actor actor) => this.m_actor = actor;

  public Actor GetActor() => this.m_actor;

  public void SetChoiceNum(int num) => this.m_cardChoice = num;

  public int GetChoiceNum() => this.m_cardChoice;

  public void SetChosenCallback(CardSelectionHandler.CardChosenCallback callback) => this.m_cardChosenCallback = callback;

  protected override void OnPress() => this.m_mouseOverTimer = Time.realtimeSinceStartup;

  protected override void OnRelease()
  {
    if (UniversalInputManager.Get().IsTouchMode() && (double) Time.realtimeSinceStartup - (double) this.m_mouseOverTimer >= 0.400000005960464 || this.m_cardChosenCallback == null)
      return;
    this.m_cardChosenCallback();
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if (this.m_actor.GetEntityDef().IsHeroSkin() || this.m_actor.GetEntityDef().IsHeroPower())
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6");
    else
      SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_mouse_over.prefab:0d4e20bc78956bc48b5e2963ec39211c");
    this.m_actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
    TooltipPanelManager.Get().UpdateKeywordHelpForForge(this.m_actor.GetEntityDef(), this.m_actor, this.m_cardChoice);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
    TooltipPanelManager.Get().HideKeywordHelp();
  }

  public delegate void CardChosenCallback();
}
