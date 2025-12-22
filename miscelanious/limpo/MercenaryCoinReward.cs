using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class MercenaryCoinReward : Reward
{
  public AsyncReference m_mercenaryCoinReference;
  protected Widget m_mercenaryCoinWidget;
  protected bool m_hidden;

  protected override void Start()
  {
    base.Start();
    this.m_mercenaryCoinReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnWidgetReady));
  }

  private void OnWidgetReady(Widget widget)
  {
    this.m_mercenaryCoinWidget = widget;
    if ((UnityEngine.Object) this.m_mercenaryCoinWidget == (UnityEngine.Object) null || !this.m_hidden)
      return;
    this.m_mercenaryCoinWidget.Hide();
  }

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

  protected override void InitData() => this.SetData((RewardData) new MercenaryCoinRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || (UnityEngine.Object) this.m_mercenaryCoinWidget == (UnityEngine.Object) null)
      return;
    if (!(this.Data is MercenaryCoinRewardData data))
    {
      Debug.LogWarning((object) string.Format("MercenaryCoinReward.OnDataSet() - data {0} is not MercenaryCoinRewardData", (object) this.Data));
    }
    else
    {
      LettuceMercenaryDbfRecord record = GameDbf.LettuceMercenary.GetRecord(data.MercenaryId);
      if (record == null)
      {
        Debug.LogWarning((object) string.Format("MercenaryCoinReward.OnDataSet() - data {0} has invalid mercenary id", (object) data.MercenaryId));
      }
      else
      {
        this.SetReady(false);
        string idFromMercenaryId = GameUtils.GetCardIdFromMercenaryId(record.ID);
        EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromMercenaryId);
        LettuceMercenaryCoinDataModel mercenaryCoinDataModel = this.GetMercenaryCoinDataModel();
        mercenaryCoinDataModel.MercenaryId = mercenaryCoinDataModel.MercenaryId;
        mercenaryCoinDataModel.MercenaryName = entityDef.GetName();
        mercenaryCoinDataModel.Quantity = mercenaryCoinDataModel.Quantity;
        mercenaryCoinDataModel.GlowActive = true;
      }
    }
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_hidden = false;
    this.m_root.SetActive(true);
    if (!((UnityEngine.Object) this.m_mercenaryCoinWidget != (UnityEngine.Object) null))
      return;
    this.m_mercenaryCoinWidget.Show();
    this.OnDataSet(true);
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
    this.m_hidden = true;
    if (!((UnityEngine.Object) this.m_mercenaryCoinWidget != (UnityEngine.Object) null))
      return;
    this.m_mercenaryCoinWidget.Hide();
  }
}
