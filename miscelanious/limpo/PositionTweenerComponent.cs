using UnityEngine;

public class PositionTweenerComponent : MonoBehaviour
{
  [SerializeField]
  private Transform m_transformComponent;
  [SerializeField]
  private PositionTweener m_settings = new PositionTweener();

  private void Reset()
  {
    if ((Object) this.m_transformComponent == (Object) null)
      this.m_transformComponent = this.transform;
    this.SetCurrentAsInitialPosition();
    this.SetCurrentAsFinalPosition();
  }

  [ContextMenu("Play Forward")]
  public void PlayForward() => this.m_settings.Play(this.gameObject, true);

  [ContextMenu("Play Reverse")]
  public void PlayReverse() => this.m_settings.Play(this.gameObject, false);

  [ContextMenu("Set Current As Initial Position")]
  public void SetCurrentAsInitialPosition() => this.m_settings.SetInitialPosition(this.m_settings.IsLocal ? this.m_transformComponent.localPosition : this.m_transformComponent.position);

  [ContextMenu("Set Current As Final Position")]
  public void SetCurrentAsFinalPosition() => this.m_settings.SetFinalPosition(this.m_settings.IsLocal ? this.m_transformComponent.localPosition : this.m_transformComponent.position);

  [ContextMenu("Reset To Beginning")]
  public void ResetToBeginning()
  {
    if (this.m_settings.IsLocal)
      this.m_transformComponent.localPosition = this.m_settings.InitialPosition;
    else
      this.m_transformComponent.position = this.m_settings.InitialPosition;
  }

  [ContextMenu("Set To End")]
  public void SetToEnd()
  {
    if (this.m_settings.IsLocal)
      this.m_transformComponent.localPosition = this.m_settings.FinalPosition;
    else
      this.m_transformComponent.position = this.m_settings.FinalPosition;
  }
}
