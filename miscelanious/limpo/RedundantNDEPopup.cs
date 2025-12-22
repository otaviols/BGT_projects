using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class RedundantNDEPopup : MonoBehaviour
{
  public UIBButton m_rerollButton;
  public UIBButton m_refuseButton;
  public Widget m_rootWidget;
  private GameObject m_owner;

  public event Action RerollSelected;

  public event Action RefuseSelected;

  public event Action OnDismissAnimationComplete;

  private void Awake()
  {
    if ((bool) (UnityEngine.Object) this.m_rerollButton)
      this.m_rerollButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRerollSelected));
    if ((bool) (UnityEngine.Object) this.m_refuseButton)
      this.m_refuseButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRefuseSelected));
    this.m_owner = this.gameObject;
    if ((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null && (UnityEngine.Object) this.transform.parent.GetComponent<WidgetInstance>() != (UnityEngine.Object) null)
      this.m_owner = this.transform.parent.gameObject;
    OverlayUI.Get().AddGameObject(this.m_owner);
  }

  private void OnDestroy() => UIContext.GetRoot().DismissPopup(this.m_owner);

  private void OnRerollSelected(UIEvent e)
  {
    Action rerollSelected = this.RerollSelected;
    if (rerollSelected == null)
      return;
    rerollSelected();
  }

  private void OnRefuseSelected(UIEvent e)
  {
    Action refuseSelected = this.RefuseSelected;
    if (refuseSelected == null)
      return;
    refuseSelected();
  }

  public void Show() => UIContext.GetRoot().ShowPopup(this.m_owner);

  public IEnumerator Hide()
  {
    this.m_rootWidget.TriggerEvent("Popup_Outro");
    yield return (object) new WaitForSeconds(0.4f);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_owner);
    Action animationComplete = this.OnDismissAnimationComplete;
    if (animationComplete != null)
      animationComplete();
  }
}
