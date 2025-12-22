using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class MercenaryExpReward : Reward
{
  public AsyncReference m_mercenaryExpRewardReference;
  protected Widget m_mercenaryCardWidget;
  protected bool m_hidden;

  protected override void Start()
  {
    base.Start();
    this.m_mercenaryExpRewardReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnWidgetReady));
  }

  private void OnWidgetReady(Widget widget)
  {
    this.m_mercenaryCardWidget = widget;
    if ((UnityEngine.Object) this.m_mercenaryCardWidget == (UnityEngine.Object) null || !this.m_hidden)
      return;
    this.m_mercenaryCardWidget.Hide();
  }

  private LettuceMercenaryExpRewardDataModel GetMercenaryExpRewardDataModel()
  {
    if ((UnityEngine.Object) this.m_mercenaryCardWidget == (UnityEngine.Object) null)
      return (LettuceMercenaryExpRewardDataModel) null;
    IDataModel model;
    if (!this.m_mercenaryCardWidget.GetDataModel(251, out model))
    {
      model = (IDataModel) new LettuceMercenaryExpRewardDataModel();
      this.m_mercenaryCardWidget.BindDataModel(model);
    }
    return model as LettuceMercenaryExpRewardDataModel;
  }

  protected override void InitData() => this.SetData((RewardData) new MercenaryExpRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || this.m_hidden || (UnityEngine.Object) this.m_mercenaryCardWidget == (UnityEngine.Object) null)
      return;
    if (!(this.Data is MercenaryExpRewardData data))
    {
      Debug.LogWarning((object) string.Format("MercenaryExpReward.OnDataSet() - data {0} is not MercenaryExpRewardData", (object) this.Data));
    }
    else
    {
      LettuceMercenaryDbfRecord record = GameDbf.LettuceMercenary.GetRecord(data.MercenaryId);
      if (record == null)
      {
        Debug.LogWarning((object) string.Format("MercenaryExpReward.OnDataSet() - data {0} has invalid mercenary id", (object) data.MercenaryId));
      }
      else
      {
        LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) record.ID);
        LettuceMercenaryExpRewardDataModel expRewardDataModel = this.GetMercenaryExpRewardDataModel();
        expRewardDataModel.Mercenary = MercenaryFactory.CreateMercenaryDataModel(mercenary);
        expRewardDataModel.Mercenary.ExperienceFinal = data.FinalExperience;
        expRewardDataModel.Mercenary.ExperienceInitial = data.InitialExperience;
        expRewardDataModel.Mercenary.Owned = true;
        expRewardDataModel.Mercenary.Label = GameStrings.Get("MERCENARY_CARD_LABEL_XP");
        int levelFromExperience = GameUtils.GetMercenaryLevelFromExperience(data.InitialExperience);
        CollectionUtils.PopulateMercenaryCardDataModel(expRewardDataModel.Mercenary, mercenary.GetEquippedArtVariation());
        CollectionUtils.SetMercenaryStatsByLevel(expRewardDataModel.Mercenary, mercenary.ID, levelFromExperience, mercenary.m_isFullyUpgraded);
        expRewardDataModel.ExperienceDeltaText = GameStrings.Format("GLUE_LETTUCE_MERCENARY_EXP_GAIN", (object) data.Amount);
        expRewardDataModel.LeveledUp = data.NumberOfLevelUps > 0;
      }
    }
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_hidden = false;
    this.m_root.SetActive(true);
    if (!((UnityEngine.Object) this.m_mercenaryCardWidget != (UnityEngine.Object) null))
      return;
    this.m_mercenaryCardWidget.Show();
    this.OnDataSet(true);
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
    this.m_hidden = true;
    if (!((UnityEngine.Object) this.m_mercenaryCardWidget != (UnityEngine.Object) null))
      return;
    this.m_mercenaryCardWidget.Hide();
  }
}
