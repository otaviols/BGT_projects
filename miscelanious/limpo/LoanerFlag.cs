using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

public class LoanerFlag : MonoBehaviour
{
  private LoanerDecksInfoDataModel m_loanerDeckInfoDataModel;
  private Widget m_widget;

  private void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    LoanerDeckDisplay loanerDeckDisplay = LoanerDeckDisplay.Get();
    if ((Object) loanerDeckDisplay == (Object) null)
      return;
    this.m_loanerDeckInfoDataModel = loanerDeckDisplay.LoanerDeckInfoDataModel;
    if (this.m_loanerDeckInfoDataModel == null || !((Object) this.m_widget != (Object) null))
      return;
    this.m_widget.BindDataModel((IDataModel) this.m_loanerDeckInfoDataModel);
  }
}
