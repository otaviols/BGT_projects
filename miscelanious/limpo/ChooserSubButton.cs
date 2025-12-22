using System.Collections;
using UnityEngine;

[CustomEditClass]
public abstract class ChooserSubButton : AdventureGenericButton
{
  protected const string s_EventFlash = "Flash";
  public GameObject m_NewModePopupBone;
  [CustomEditField(Sections = "Event Table")]
  public StateEventTable m_StateTable;
  public float m_NewModePopupAutomaticHideTime = 1f;
  protected bool m_Glow;
  private Notification m_NewModePopup;

  public void SetHighlight(bool enable)
  {
    UIBHighlightStateControl component1 = this.GetComponent<UIBHighlightStateControl>();
    if ((Object) component1 != (Object) null)
    {
      if (this.m_Glow)
        component1.Select(true, true);
      else
        component1.Select(enable);
    }
    UIBHighlight component2 = this.GetComponent<UIBHighlight>();
    if (!((Object) component2 != (Object) null))
      return;
    if (enable)
      component2.Select();
    else
      component2.Reset();
  }

  public void SetNewGlow(bool enable)
  {
    this.m_Glow = enable;
    UIBHighlightStateControl component = this.GetComponent<UIBHighlightStateControl>();
    if (!((Object) component != (Object) null))
      return;
    component.Select(enable, true);
  }

  public void ShowNewModePopup(string message)
  {
    if ((Object) this.m_NewModePopupBone == (Object) null)
      return;
    this.m_NewModePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_NewModePopupBone.transform.position, this.m_NewModePopupBone.transform.localScale, message);
    this.m_NewModePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
  }

  public void HideNewModePopupAfterDelay() => this.StartCoroutine(this.HideNewModePopupAfterDelayCoroutine());

  public void Flash() => this.m_StateTable.TriggerState(nameof (Flash));

  public bool IsReady()
  {
    UIBHighlightStateControl component = this.GetComponent<UIBHighlightStateControl>();
    return (Object) component != (Object) null && component.IsReady();
  }

  protected override void OnDestroy()
  {
    if ((Object) this.m_NewModePopup != (Object) null)
      this.m_NewModePopup.Shrink();
    base.OnDestroy();
  }

  public void OnDisable()
  {
    if (!((Object) this.m_NewModePopup != (Object) null))
      return;
    this.m_NewModePopup.Shrink();
  }

  private IEnumerator HideNewModePopupAfterDelayCoroutine()
  {
    float timer = this.m_NewModePopupAutomaticHideTime;
    while ((double) timer > 0.0)
    {
      timer -= Time.deltaTime;
      yield return (object) new WaitForEndOfFrame();
    }
    if ((Object) this.m_NewModePopup != (Object) null)
      this.m_NewModePopup.Shrink();
  }
}
