using PegasusShared;
using UnityEngine;

public class DeckTrayReorderableContent : DeckTrayContent
{
  [CustomEditField(Sections = "Deck Button Settings")]
  public Vector3 m_rearrangeWiggleAxis = new Vector3(0.0f, 1f, 0.0f);
  [CustomEditField(Sections = "Deck Button Settings")]
  public float m_rearrangeWiggleAmplitude = 0.85f;
  [CustomEditField(Sections = "Deck Button Settings")]
  public float m_rearrangeWiggleFrequency = 15f;
  [CustomEditField(Sections = "Deck Button Settings")]
  public float m_rearrangeStartStopTweenDuration = 0.1f;
  [CustomEditField(Sections = "Deck Button Settings")]
  public float m_rearrangeEnlargeScale = 1.05f;
  protected IDraggableCollectionVisual m_draggingDeckBox;
  [CustomEditField(Sections = "Scroll Settings")]
  public UIBScrollable m_scrollbar;

  public bool IsTouchDragging => (Object) this.m_scrollbar != (Object) null && this.m_scrollbar.IsTouchDragging();

  public IDraggableCollectionVisual DraggingDeckBox => this.m_draggingDeckBox;

  public virtual void StartDragToReorder(IDraggableCollectionVisual draggingDeckBox)
  {
    if (this.m_draggingDeckBox == draggingDeckBox)
      return;
    if (this.m_draggingDeckBox != null)
      this.StopDragToReorder();
    this.m_draggingDeckBox = draggingDeckBox;
    this.m_scrollbar.Pause(true);
    this.m_scrollbar.PauseUpdateScrollHeight(true);
  }

  public virtual void StopDragToReorder()
  {
    if (this.m_draggingDeckBox != null)
    {
      foreach (CollectionDeck deck in CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK))
        deck.SendChanges(CollectionDeck.ChangeSource.StopDragToReorder);
      this.m_draggingDeckBox.OnStopDragToReorder();
    }
    this.m_draggingDeckBox = (IDraggableCollectionVisual) null;
    this.m_scrollbar.Pause(false);
    this.m_scrollbar.PauseUpdateScrollHeight(false);
  }

  protected virtual void UpdateDragToReorder()
  {
  }
}
