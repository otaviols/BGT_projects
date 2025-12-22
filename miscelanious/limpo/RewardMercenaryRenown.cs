using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class RewardMercenaryRenown : MonoBehaviour
{
  [SerializeField]
  private AsyncReference m_mercenaryRenownReference;
  private Widget m_mercenaryRenownWidget;
  private MercenaryRenownRewardData m_rewardData;

  public void Initialize(MercenaryRenownRewardData rewardData) => this.m_rewardData = rewardData;

  private void Start() => this.m_mercenaryRenownReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnWidgetReady));

  private LettuceMercenaryCoinDataModel GetMercenaryCoinDataModel()
  {
    if ((UnityEngine.Object) this.m_mercenaryRenownWidget == (UnityEngine.Object) null)
      return (LettuceMercenaryCoinDataModel) null;
    IDataModel model;
    if (!this.m_mercenaryRenownWidget.GetDataModel(238, out model))
    {
      model = (IDataModel) new LettuceMercenaryCoinDataModel();
      this.m_mercenaryRenownWidget.BindDataModel(model);
    }
    return model as LettuceMercenaryCoinDataModel;
  }

  private void OnWidgetReady(Widget widget)
  {
    this.m_mercenaryRenownWidget = widget;
    if ((UnityEngine.Object) this.m_mercenaryRenownWidget == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(widget.gameObject, this.gameObject.layer);
  }
}
