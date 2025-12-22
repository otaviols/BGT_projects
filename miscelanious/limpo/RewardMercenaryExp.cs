using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class RewardMercenaryExp : MonoBehaviour
{
  public AsyncReference m_mercenaryRewardReference;
  protected Widget m_mercenaryCardWidget;
  private MercenaryExpRewardData m_rewardData;

  public void Initialize(MercenaryExpRewardData rewardData) => this.m_rewardData = rewardData;

  private void Start() => this.m_mercenaryRewardReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnWidgetReady));

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

  private void OnWidgetReady(Widget widget)
  {
    this.m_mercenaryCardWidget = widget;
    if ((UnityEngine.Object) this.m_mercenaryCardWidget == (UnityEngine.Object) null)
      return;
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) this.m_rewardData.MercenaryId);
    LettuceMercenaryExpRewardDataModel expRewardDataModel = this.GetMercenaryExpRewardDataModel();
    expRewardDataModel.Mercenary = MercenaryFactory.CreateMercenaryDataModel(mercenary);
    expRewardDataModel.Mercenary.ExperienceFinal = (int) mercenary.m_experience;
    expRewardDataModel.Mercenary.ExperienceInitial = (int) mercenary.m_experience - this.m_rewardData.Amount;
    expRewardDataModel.Mercenary.Owned = true;
    expRewardDataModel.Mercenary.Label = GameStrings.Get("MERCENARY_CARD_LABEL_XP");
    GameUtils.GetMercenaryLevelFromExperience(expRewardDataModel.Mercenary.ExperienceInitial);
    CollectionUtils.PopulateMercenaryCardDataModel(expRewardDataModel.Mercenary, mercenary.GetEquippedArtVariation());
    CollectionUtils.SetMercenaryStatsByLevel(expRewardDataModel.Mercenary, mercenary.ID, mercenary.m_level, mercenary.m_isFullyUpgraded);
    expRewardDataModel.ExperienceDeltaText = GameStrings.Format("GLUE_LETTUCE_MERCENARY_EXP_GAIN", (object) this.m_rewardData.Amount);
    expRewardDataModel.LeveledUp = this.m_rewardData.NumberOfLevelUps > 0;
  }
}
