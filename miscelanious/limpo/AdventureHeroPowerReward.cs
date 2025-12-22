using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class AdventureHeroPowerReward : WidgetReward
{
  private AdventureHeroPowerDbfRecord m_heroPowerRecord;
  private DefLoader.DisposableFullDef m_heroPowerFullDef;

  protected override void Start()
  {
    base.Start();
    this.StartCoroutine(this.SetHeroPowerWhenReady());
  }

  protected override void OnDestroy()
  {
    this.m_heroPowerFullDef?.Dispose();
    this.m_heroPowerFullDef = (DefLoader.DisposableFullDef) null;
    base.OnDestroy();
  }

  private IEnumerator SetHeroPowerWhenReady()
  {
    AdventureHeroPowerReward adventureHeroPowerReward = this;
    while ((Object) adventureHeroPowerReward.m_rewardWidget == (Object) null)
      yield return (object) null;
    while (adventureHeroPowerReward.m_heroPowerRecord == null)
      yield return (object) null;
    AdventureLoadoutOptionDataModel loadoutOptionDataModel = new AdventureLoadoutOptionDataModel();
    if (!string.IsNullOrEmpty((string) adventureHeroPowerReward.m_heroPowerRecord.UnlockedDescriptionText))
    {
      int num1 = 0;
      if (adventureHeroPowerReward.m_heroPowerRecord.UnlockAchievement > 0)
        num1 = AchievementManager.Get().GetAchievementDataModel(adventureHeroPowerReward.m_heroPowerRecord.UnlockAchievement).Quota;
      int num2 = adventureHeroPowerReward.m_heroPowerRecord.UnlockValue + num1;
      loadoutOptionDataModel.LockedText = string.Format((string) adventureHeroPowerReward.m_heroPowerRecord.UnlockedDescriptionText, (object) num2);
    }
    adventureHeroPowerReward.m_rewardWidget.BindDataModel((IDataModel) loadoutOptionDataModel);
    while (adventureHeroPowerReward.m_rewardWidget.IsChangingStates)
      yield return (object) null;
    while (adventureHeroPowerReward.m_heroPowerFullDef == null)
      yield return (object) null;
    Actor componentInChildren = adventureHeroPowerReward.m_rewardWidget.GetComponentInChildren<Actor>();
    componentInChildren.SetFullDef(adventureHeroPowerReward.m_heroPowerFullDef);
    componentInChildren.UpdateAllComponents();
    adventureHeroPowerReward.SetReady(true);
  }

  private void OnFullDefLoaded(string cardID, DefLoader.DisposableFullDef def, object userData)
  {
    if (def == null)
    {
      Debug.LogErrorFormat("Unable to load FullDef for cardID={0}", (object) cardID);
    }
    else
    {
      this.m_heroPowerFullDef?.Dispose();
      this.m_heroPowerFullDef = def.Share();
    }
  }

  protected override void InitData() => this.SetData((RewardData) new AdventureHeroPowerRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is AdventureHeroPowerRewardData data))
    {
      Debug.LogWarningFormat("AdventureHeroPowerReward.OnDataSet() - Data {0} is not HeroPowerRewardData", (object) this.Data);
    }
    else
    {
      this.m_heroPowerRecord = data.HeroPowerRecord;
      if (this.m_heroPowerRecord == null)
      {
        Debug.LogWarningFormat("AdventureHeroPowerReward.OnDataSet() - HeroPowerRecord is null!");
      }
      else
      {
        string cardId = GameUtils.TranslateDbIdToCardId(this.m_heroPowerRecord.CardId);
        if (cardId == null)
        {
          Debug.LogWarningFormat("AdventureHeroPowerReward.OnDataSet() - No CardId found for DbId {0}!", (object) this.m_heroPowerRecord.CardId);
        }
        else
        {
          this.SetReady(false);
          DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded));
        }
      }
    }
  }
}
