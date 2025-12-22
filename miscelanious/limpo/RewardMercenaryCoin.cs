using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class RewardMercenaryCoin : MonoBehaviour
{
  public AsyncReference m_mercenaryCoinReference;
  private Widget m_mercenaryCoinWidget;
  private MercenaryCoinRewardData m_rewardData;

  public void Initialize(MercenaryCoinRewardData rewardData) => this.m_rewardData = rewardData;

  private void Start() => this.m_mercenaryCoinReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnWidgetReady));

  private LettuceMercenaryCoinDataModel GetMercenaryCoinDataModel()
  {
    if ((UnityEngine.Object) this.m_mercenaryCoinWidget == (UnityEngine.Object) null)
      return (LettuceMercenaryCoinDataModel) null;
    IDataModel model;
    if (!this.m_mercenaryCoinWidget.GetDataModel(238, out model))
    {
      model = (IDataModel) new LettuceMercenaryCoinDataModel();
      this.m_mercenaryCoinWidget.BindDataModel(model);
    }
    return model as LettuceMercenaryCoinDataModel;
  }

  private void OnWidgetReady(Widget widget)
  {
    this.m_mercenaryCoinWidget = widget;
    if ((UnityEngine.Object) this.m_mercenaryCoinWidget == (UnityEngine.Object) null)
      return;
    string idFromMercenaryId = GameUtils.GetCardIdFromMercenaryId(this.m_rewardData.MercenaryId);
    EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromMercenaryId);
    LettuceMercenaryCoinDataModel mercenaryCoinDataModel = this.GetMercenaryCoinDataModel();
    mercenaryCoinDataModel.MercenaryId = this.m_rewardData.MercenaryId;
    mercenaryCoinDataModel.MercenaryName = entityDef.GetName();
    mercenaryCoinDataModel.Quantity = this.m_rewardData.Quantity;
    mercenaryCoinDataModel.GlowActive = true;
    mercenaryCoinDataModel.NameActive = true;
    LayerUtils.SetLayer(widget.gameObject, this.gameObject.layer);
  }
}
