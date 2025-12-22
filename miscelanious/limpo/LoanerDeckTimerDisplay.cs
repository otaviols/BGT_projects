using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

public class LoanerDeckTimerDisplay : MonoBehaviour
{
  private FreeDeckMgr m_freeDeckManager;
  private LoanerDecksInfoDataModel m_loanerDeckInfoDataModel;
  private Widget m_widget;
  private LoanerDeckDisplay m_loanerDeckDisplay;

  private void Start()
  {
    this.m_freeDeckManager = FreeDeckMgr.Get();
    if (this.m_freeDeckManager.Status != FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD)
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      this.m_widget = this.GetComponent<Widget>();
      this.m_loanerDeckDisplay = LoanerDeckDisplay.Get();
      if (!((Object) this.m_widget != (Object) null) || !((Object) this.m_loanerDeckDisplay != (Object) null))
        return;
      this.m_loanerDeckInfoDataModel = this.m_loanerDeckDisplay.LoanerDeckInfoDataModel;
      if (this.m_loanerDeckInfoDataModel != null)
        this.m_widget.BindDataModel((IDataModel) this.m_loanerDeckInfoDataModel);
      this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.m_loanerDeckDisplay.OpenDeckDetailsWidget));
      this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.m_loanerDeckDisplay.HideDeckDetailsWidget));
      this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.m_loanerDeckDisplay.ConfirmDeckSelection));
    }
  }

  private void OnDestroy()
  {
    if (!((Object) this.m_widget != (Object) null))
      return;
    this.m_widget.UnbindDataModel(478);
  }
}
