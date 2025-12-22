using Hearthstone.DataModels;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class AdventureDeckReward : WidgetReward
{
  private AdventureDeckDbfRecord m_deckRecord;

  protected override void Start()
  {
    base.Start();
    this.StartCoroutine(this.SetDeckPouchPropertiesWhenStateReady());
  }

  private IEnumerator SetDeckPouchPropertiesWhenStateReady()
  {
    AdventureDeckReward adventureDeckReward = this;
    while ((Object) adventureDeckReward.m_rewardWidget == (Object) null)
      yield return (object) null;
    while (adventureDeckReward.m_deckRecord == null)
      yield return (object) null;
    string deckName;
    CollectionManager.Get().LoadDeckFromDBF(adventureDeckReward.m_deckRecord.DeckId, out deckName, out string _);
    AdventureLoadoutOptionDataModel dataModel = new AdventureLoadoutOptionDataModel();
    dataModel.Name = deckName;
    if (!string.IsNullOrEmpty((string) adventureDeckReward.m_deckRecord.UnlockedDescriptionText))
      dataModel.LockedText = string.Format((string) adventureDeckReward.m_deckRecord.UnlockedDescriptionText, (object) adventureDeckReward.m_deckRecord.UnlockValue);
    dataModel.DisplayColor = CollectionPageManager.ColorForClass((TAG_CLASS) adventureDeckReward.m_deckRecord.ClassId);
    bool waitingForTexture = false;
    if (string.IsNullOrEmpty(adventureDeckReward.m_deckRecord.DisplayTexture))
    {
      dataModel.DisplayTexture = (Material) null;
    }
    else
    {
      ObjectCallback callback = (ObjectCallback) ((assetRef, materialObj, data) =>
      {
        dataModel.DisplayTexture = materialObj as Material;
        waitingForTexture = false;
      });
      waitingForTexture = true;
      AssetLoader.Get().LoadMaterial((AssetReference) adventureDeckReward.m_deckRecord.DisplayTexture, callback);
    }
    adventureDeckReward.m_rewardWidget.BindDataModel((IDataModel) dataModel);
    while (waitingForTexture || adventureDeckReward.m_rewardWidget.IsChangingStates)
      yield return (object) null;
    adventureDeckReward.SetReady(true);
  }

  protected override void InitData() => this.SetData((RewardData) new AdventureDeckRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is AdventureDeckRewardData data))
    {
      Debug.LogWarningFormat("AdventureDeckReward.OnDataSet() - Data {0} is not DeckRewardData", (object) this.Data);
    }
    else
    {
      this.SetReady(false);
      this.m_deckRecord = data.DeckRecord;
    }
  }
}
