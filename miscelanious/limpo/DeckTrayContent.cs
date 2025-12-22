using UnityEngine;

public class DeckTrayContent : MonoBehaviour
{
  private bool m_isModeActive;
  private bool m_isModeTrying;

  protected virtual void Awake()
  {
  }

  protected virtual void OnDestroy()
  {
  }

  public virtual void Show(bool showAll = false)
  {
  }

  public virtual void Hide(bool hideAll = false)
  {
  }

  public virtual bool IsContentLoaded() => true;

  public virtual bool PreAnimateContentEntrance() => true;

  public virtual bool PostAnimateContentEntrance() => true;

  public virtual bool AnimateContentEntranceStart() => true;

  public virtual bool AnimateContentEntranceEnd() => true;

  public virtual bool AnimateContentExitStart() => true;

  public virtual bool AnimateContentExitEnd() => true;

  public virtual bool PreAnimateContentExit() => true;

  public virtual bool PostAnimateContentExit() => true;

  public virtual void OnEditedDeckChanged(
    CollectionDeck newDeck,
    CollectionDeck oldDeck,
    bool isNewDeck)
  {
  }

  public virtual void OnEditingTeamChanged(
    LettuceTeam newTeam,
    LettuceTeam oldTeam,
    bool isNewTeam)
  {
  }

  public bool IsModeActive() => this.m_isModeActive;

  public bool IsModeTryingOrActive() => this.m_isModeTrying || this.m_isModeActive;

  public void SetModeActive(bool active) => this.m_isModeActive = active;

  public void SetModeTrying(bool trying) => this.m_isModeTrying = trying;
}
