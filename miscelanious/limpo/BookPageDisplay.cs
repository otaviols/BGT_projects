using UnityEngine;

public abstract class BookPageDisplay : MonoBehaviour
{
  public MeshRenderer m_basePageRenderer;

  protected bool IsShown { get; private set; }

  public abstract bool IsLoaded();

  public virtual void Show() => this.IsShown = true;

  public virtual void Hide() => this.IsShown = false;
}
