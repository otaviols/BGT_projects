using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesTrainingHallExpRewardPopup : MonoBehaviour
{
  public const string DISMISS_EXP_REWARD = "DISMISS_EXP_REWARD";
  private GameObject m_rewardContainer;
  private Widget m_rootWidget;
  private List<MercenaryExpRewardData> m_mercenaryExpRewards;
  private Action m_onReadyCallback;
  private Action m_onClosedCallback;
  private bool m_isLoading;

  private void LoadWidgetPrefab()
  {
    if (this.m_isLoading)
      return;
    this.m_isLoading = true;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "MercenariesExperienceTrainingHall.prefab:a3944e4a53a98714b8dbb33907caeba6", new PrefabCallback<GameObject>(this.OnExperienceWidgetLoaded));
  }

  private void OnExperienceWidgetLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      this.m_isLoading = false;
      Log.Lettuce.PrintError("Failed to load mercenaries experience reward prefab");
    }
    else
    {
      go.transform.parent = this.transform;
      this.m_rewardContainer = go;
      this.m_rewardContainer.transform.localScale = Vector3.one;
      this.m_rewardContainer.transform.localPosition = Vector3.zero;
      this.m_rootWidget = (Widget) go.GetComponentInChildren<WidgetInstance>();
      this.m_rootWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener));
      this.BindMercenariesExperienceTwoScoopDataModel();
      if (this.m_onReadyCallback != null)
        this.m_onReadyCallback();
      this.m_isLoading = false;
    }
  }

  private void WidgetEventListener(string eventName)
  {
    if (!(eventName == "DISMISS_EXP_REWARD"))
      return;
    this.OnClosed();
  }

  private void BindMercenariesExperienceTwoScoopDataModel()
  {
    if (this.m_mercenaryExpRewards == null || this.m_mercenaryExpRewards.Count == 0 || (UnityEngine.Object) this.m_rootWidget == (UnityEngine.Object) null)
      return;
    LettuceExperienceTwoScoopDataModel twoScoopDataModel = new LettuceExperienceTwoScoopDataModel();
    this.m_rootWidget.BindDataModel((IDataModel) twoScoopDataModel);
    twoScoopDataModel.ExpRewards = new DataModelList<LettuceMercenaryExpRewardDataModel>();
    foreach (MercenaryExpRewardData mercenaryExpReward in this.m_mercenaryExpRewards)
    {
      LettuceMercenaryExpRewardDataModel expRewardDataModel = new LettuceMercenaryExpRewardDataModel();
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryExpReward.MercenaryId);
      expRewardDataModel.Mercenary = MercenaryFactory.CreateMercenaryDataModel(mercenary);
      expRewardDataModel.Mercenary.ExperienceFinal = mercenaryExpReward.FinalExperience;
      expRewardDataModel.Mercenary.ExperienceInitial = mercenaryExpReward.InitialExperience;
      expRewardDataModel.Mercenary.Owned = true;
      expRewardDataModel.Mercenary.Label = GameStrings.Get("MERCENARY_CARD_LABEL_XP");
      int levelFromExperience = GameUtils.GetMercenaryLevelFromExperience(mercenaryExpReward.InitialExperience);
      CollectionUtils.PopulateMercenaryCardDataModel(expRewardDataModel.Mercenary, mercenary.GetEquippedArtVariation());
      CollectionUtils.SetMercenaryStatsByLevel(expRewardDataModel.Mercenary, mercenary.ID, levelFromExperience, mercenary.m_isFullyUpgraded);
      expRewardDataModel.ExperienceDeltaText = GameStrings.Format("GLUE_LETTUCE_MERCENARY_EXP_GAIN", (object) mercenaryExpReward.Amount);
      expRewardDataModel.LeveledUp = mercenaryExpReward.NumberOfLevelUps > 0;
      twoScoopDataModel.ExpRewards.Add(expRewardDataModel);
    }
  }

  public void Initialize(
    List<MercenaryExpRewardData> mercenaryExpRewards,
    Action onReadyCallback,
    Action onClosedCallback)
  {
    this.m_mercenaryExpRewards = mercenaryExpRewards;
    this.m_onClosedCallback = onClosedCallback;
    this.m_onReadyCallback = onReadyCallback;
    this.LoadWidgetPrefab();
  }

  private void OnClosed()
  {
    Action onClosedCallback = this.m_onClosedCallback;
    if (onClosedCallback != null)
      onClosedCallback();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_rewardContainer, 1f);
  }
}
