using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class LuckyDrawTile : MonoBehaviour
{
  private Widget m_widget;
  [SerializeField]
  private WidgetInstance rewardItemDisplayWidget;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      Log.ErrorReporter.PrintError("Error [LuckyDrawTile] WidgetTemplate not found on {0}", (object) this.gameObject.name);
    else if ((UnityEngine.Object) this.rewardItemDisplayWidget == (UnityEngine.Object) null)
      Log.ErrorReporter.PrintError("Error [LuckyDrawTile] rewardItemDisplayWidget was not found on {0}", (object) this.gameObject.name);
    else
      this.rewardItemDisplayWidget.RegisterReadyListener((Action<object>) (_ => this.InitializeRewardItemWidget()), (object) null, true);
  }

  private void InitializeRewardItemWidget()
  {
    WidgetTemplate componentInChildren = this.m_widget.GetComponentInChildren<WidgetTemplate>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
    {
      Log.ErrorReporter.PrintError("Error [LuckyDrawTile] InitializeRewardItemWidget() Could not find WidgetTemplate child on m_widget! From object {0}", (object) this.gameObject.name);
    }
    else
    {
      IDataModel model;
      if (!this.rewardItemDisplayWidget.GetDataModel(34, out model))
        return;
      RewardListDataModel rewardListDataModel = model as RewardListDataModel;
      componentInChildren.BindDataModel((IDataModel) rewardListDataModel, false);
    }
  }

  public LuckyDrawRewardDataModel GetBoundRewardDataModel()
  {
    IDataModel model;
    return this.m_widget.GetDataModel(667, out model) ? model as LuckyDrawRewardDataModel : (LuckyDrawRewardDataModel) null;
  }
}
