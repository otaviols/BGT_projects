using UnityEngine;

public class StartOfGameSpell : SuperSpell
{
  public GameObject m_InitialVO;
  public GameObject m_ResponseVO;

  public override bool AddPowerTargets()
  {
    Card sourceCard = this.GetSourceCard();
    EntityDef entityDef = sourceCard.GetEntity().GetEntityDef();
    Player controller = sourceCard.GetController();
    if (controller.HasSeenStartOfGameSpell(entityDef))
      return false;
    int num = base.AddPowerTargets() ? 1 : 0;
    if (num == 0)
      return num != 0;
    controller.MarkStartOfGameSpellAsSeen(entityDef);
    return num != 0;
  }

  protected override void OnAction(SpellStateType prevStateType)
  {
    Card sourceCard = this.GetSourceCard();
    EntityDef entityDef = sourceCard.GetEntity().GetEntityDef();
    TAG_PREMIUM premium = sourceCard.GetPremium();
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entityDef, premium), AssetLoadingOptions.IgnorePrefabPosition);
    Actor component1 = gameObject.GetComponent<Actor>();
    component1.SetCardDefFromCard(sourceCard);
    component1.SetEntityDef(entityDef);
    component1.SetPremium(premium);
    component1.UpdateAllComponents();
    gameObject.SetActive(false);
    PlayMakerFSM component2 = this.GetComponent<PlayMakerFSM>();
    component2.FsmVariables.GetFsmGameObject("CardGO").Value = gameObject;
    bool flag = GameState.Get().GetFirstOpponentPlayer(sourceCard.GetController()).HasSeenStartOfGameSpell(entityDef);
    if (!flag && (Object) this.m_InitialVO != (Object) null)
      component2.FsmVariables.GetFsmGameObject("VOLineGO").Value = this.m_InitialVO;
    else if (flag && (Object) this.m_ResponseVO != (Object) null)
      component2.FsmVariables.GetFsmGameObject("VOLineGO").Value = this.m_ResponseVO;
    base.OnAction(prevStateType);
  }
}
