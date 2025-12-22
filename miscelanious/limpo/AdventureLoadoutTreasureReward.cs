using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class AdventureLoadoutTreasureReward : WidgetReward
{
  private AdventureLoadoutTreasuresDbfRecord m_loadoutTreasureRecord;

  protected override void Start() => base.Start();

  private IEnumerator SetDataWhenLoaded()
  {
    AdventureLoadoutTreasureReward loadoutTreasureReward = this;
    while ((Object) loadoutTreasureReward.m_rewardWidget == (Object) null)
      yield return (object) null;
    AdventureLoadoutTreasureRewardData data = loadoutTreasureReward.Data as AdventureLoadoutTreasureRewardData;
    AdventureLoadoutOptionDataModel loadoutOptionDataModel = new AdventureLoadoutOptionDataModel();
    bool flag = false;
    if (data != null && data.IsUpgrade)
    {
      flag = true;
      if (!string.IsNullOrEmpty((string) loadoutTreasureReward.m_loadoutTreasureRecord.UpgradedDescriptionText))
        loadoutOptionDataModel.LockedText = string.Format((string) loadoutTreasureReward.m_loadoutTreasureRecord.UpgradedDescriptionText, (object) loadoutTreasureReward.m_loadoutTreasureRecord.UpgradeValue);
      loadoutOptionDataModel.IsUpgraded = true;
    }
    else
    {
      if (!string.IsNullOrEmpty((string) loadoutTreasureReward.m_loadoutTreasureRecord.UnlockedDescriptionText))
      {
        int num1 = 0;
        if (loadoutTreasureReward.m_loadoutTreasureRecord.UnlockAchievement > 0)
          num1 = AchievementManager.Get().GetAchievementDataModel(loadoutTreasureReward.m_loadoutTreasureRecord.UnlockAchievement).Quota;
        int num2 = loadoutTreasureReward.m_loadoutTreasureRecord.UnlockValue + num1;
        loadoutOptionDataModel.LockedText = string.Format((string) loadoutTreasureReward.m_loadoutTreasureRecord.UnlockedDescriptionText, (object) num2);
      }
      loadoutOptionDataModel.IsUpgraded = false;
    }
    loadoutTreasureReward.m_rewardWidget.BindDataModel((IDataModel) loadoutOptionDataModel);
    string cardId = GameUtils.TranslateDbIdToCardId(flag ? loadoutTreasureReward.m_loadoutTreasureRecord.UpgradedCardId : loadoutTreasureReward.m_loadoutTreasureRecord.CardId);
    if (cardId == null)
      Debug.LogWarningFormat("AdventureLoadoutTreasureReward.SetLoadoutTreasureWhenReady() - No CardId found for DbId {0}!", (object) loadoutTreasureReward.m_loadoutTreasureRecord.CardId);
    CardDataModel cardDataModel = new CardDataModel();
    CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(cardId);
    cardDataModel.CardId = cardId;
    cardDataModel.FlavorText = (string) cardRecord?.FlavorText;
    loadoutTreasureReward.m_rewardWidget.BindDataModel((IDataModel) cardDataModel);
    while (loadoutTreasureReward.m_rewardWidget.IsChangingStates)
      yield return (object) null;
    loadoutTreasureReward.SetReady(true);
  }

  protected override void InitData() => this.SetData((RewardData) new AdventureLoadoutTreasureRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is AdventureLoadoutTreasureRewardData data))
    {
      Debug.LogWarningFormat("AdventureLoadoutTreasureReward.OnDataSet() - Data {0} is not LoadoutTreasureRewardData", (object) this.Data);
    }
    else
    {
      this.m_loadoutTreasureRecord = data.LoadoutTreasureRecord;
      if (this.m_loadoutTreasureRecord == null)
      {
        Debug.LogWarningFormat("AdventureLoadoutTreasureReward.OnDataSet() - LoadoutTreasureRecord is null!");
      }
      else
      {
        this.SetReady(false);
        this.StartCoroutine(this.SetDataWhenLoaded());
      }
    }
  }
}
