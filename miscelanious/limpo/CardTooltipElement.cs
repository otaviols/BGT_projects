using UnityEngine;

public class CardTooltipElement : PegUIElement
{
  [SerializeField]
  private TooltipPanelManager.Orientation m_orientation;
  private Hearthstone.UI.Card m_cardRef;

  [ContextMenu("Assign Component References From Children")]
  public void AssignComponentReferencesFromChildren()
  {
    if (!((Object) this.m_cardRef == (Object) null))
      return;
    this.m_cardRef = this.GetComponentInChildren<Hearthstone.UI.Card>(true);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    base.OnOver(oldState);
    this.AssignComponentReferencesFromChildren();
    if ((Object) this.m_cardRef == (Object) null)
      return;
    Actor cardActor = this.m_cardRef.CardActor;
    if ((Object) cardActor == (Object) null)
      return;
    EntityDef entityDef = cardActor.GetEntityDef();
    if (entityDef == null)
      return;
    TooltipPanelManager.Get().UpdateKeywordHelpForCollectionManager(entityDef, cardActor, this.m_orientation);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    base.OnOut(oldState);
    TooltipPanelManager.Get().HideKeywordHelp();
  }

  public override void SetEnabled(bool enabled, bool isInternal = false)
  {
    base.SetEnabled(enabled, isInternal);
    if (enabled)
      return;
    TooltipPanelManager.Get().HideKeywordHelp();
  }
}
