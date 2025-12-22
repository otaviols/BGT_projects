using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RankedMedalWrapper : MonoBehaviour
{
  private Widget m_widget;

  private void Awake() => this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();

  public void BindRankedPlayDataModel(RankedPlayDataModel dataModel)
  {
    if (dataModel == this.GetRankedPlayDataModel())
      return;
    this.m_widget.BindDataModel((IDataModel) dataModel);
  }

  public bool IsReady => !((Object) this.m_widget == (Object) null) && this.m_widget.IsReady && !this.m_widget.IsChangingStates && !((Object) this.m_widget.GetComponentInChildren<RankedMedal>(false) == (Object) null);

  public void Hide() => this.gameObject.SetActive(false);

  public void Show(bool useLegacyRankedPlay)
  {
    this.gameObject.SetActive(true);
    if (useLegacyRankedPlay)
      this.m_widget.TriggerEvent("SHOW_LEGACY");
    else
      this.m_widget.TriggerEvent("SHOW_NEW");
  }

  private RankedPlayDataModel GetRankedPlayDataModel()
  {
    IDataModel model;
    this.m_widget.GetDataModel(123, out model);
    return model as RankedPlayDataModel;
  }
}
