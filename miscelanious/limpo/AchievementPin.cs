using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

public class AchievementPin : MonoBehaviour
{
  private Widget m_widget;
  private const string SHOW_NOTIF = "SHOW_NOTIF";
  private const string HIDE_NOTIF = "HIDE_NOTIF";

  private void Awake() => this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();

  private void OnEnable()
  {
    AchievementCategoryDataModel dataModel = this.m_widget.GetDataModel<AchievementCategoryDataModel>();
    if (dataModel == null)
      return;
    if (dataModel.Stats.Unclaimed > 0)
      this.m_widget.TriggerEvent("SHOW_NOTIF");
    else
      this.m_widget.TriggerEvent("HIDE_NOTIF");
  }
}
