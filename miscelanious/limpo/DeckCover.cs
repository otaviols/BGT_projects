using UnityEngine;

public class DeckCover : MonoBehaviour
{
  public Spell m_OpenDeckCoverSpell;
  public MeshRenderer m_HighlightMeshRenderer;

  public void SetDeckVisualRootObject(GameObject deckVisualRootObject)
  {
    if ((Object) this.m_OpenDeckCoverSpell == (Object) null || (Object) deckVisualRootObject == (Object) null)
      return;
    PlayMakerFSM component = this.m_OpenDeckCoverSpell.GetComponent<PlayMakerFSM>();
    if ((Object) component == (Object) null)
      return;
    component.FsmVariables.GetFsmGameObject("DeckVisual").Value = deckVisualRootObject;
  }

  public void OpenDeckCover()
  {
    if ((Object) this.m_OpenDeckCoverSpell == (Object) null || this.m_OpenDeckCoverSpell.GetActiveState() == SpellStateType.BIRTH || this.m_OpenDeckCoverSpell.GetActiveState() == SpellStateType.IDLE)
      return;
    this.m_OpenDeckCoverSpell.ActivateState(SpellStateType.BIRTH);
  }

  public void CloseDeckCover()
  {
    if ((Object) this.m_OpenDeckCoverSpell == (Object) null || this.m_OpenDeckCoverSpell.GetActiveState() == SpellStateType.DEATH || this.m_OpenDeckCoverSpell.GetActiveState() == SpellStateType.NONE)
      return;
    this.m_OpenDeckCoverSpell.ActivateState(SpellStateType.DEATH);
  }

  public void SetDeckCoverHighlightVisibility(bool isVisible)
  {
    if (!((Object) this.m_HighlightMeshRenderer != (Object) null))
      return;
    this.m_HighlightMeshRenderer.enabled = isVisible;
  }

  public virtual void UpdateVisual(Player.Side side)
  {
  }
}
