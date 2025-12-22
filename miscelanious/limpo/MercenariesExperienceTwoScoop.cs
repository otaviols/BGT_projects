using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesExperienceTwoScoop : MonoBehaviour
{
  public AsyncReference m_rootWidgetReference;
  private Widget m_rootWidget;
  private List<MercenaryExpRewardData> m_mercenaryExpRewards;
  private Action m_onClosedCallback;
  private bool m_initialized;

  private void Start() => this.m_rootWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRootWidgetReady));

  private void WidgetEventListener(string eventName)
  {
    if (!(eventName == "DISMISS_TWO_SCOOP"))
      return;
    this.OnClosed();
  }

  public void OnRootWidgetReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "Root Widget could not be found!");
    }
    else
    {
      this.m_rootWidget = widget;
      this.m_rootWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener));
      this.BindMercenariesExperienceTwoScoopDataModel();
    }
  }

  private void BindMercenariesExperienceTwoScoopDataModel()
  {
    if (this.m_initialized || this.m_mercenaryExpRewards == null || this.m_mercenaryExpRewards.Count == 0 || (UnityEngine.Object) this.m_rootWidget == (UnityEngine.Object) null)
      return;
    this.m_initialized = true;
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
      expRewardDataModel.Mercenary.IsMaxLevel = levelFromExperience >= GameUtils.GetMaxMercenaryLevel();
      expRewardDataModel.ExperienceDeltaText = GameStrings.Format("GLUE_LETTUCE_MERCENARY_EXP_GAIN", (object) mercenaryExpReward.Amount);
      expRewardDataModel.LeveledUp = mercenaryExpReward.NumberOfLevelUps > 0;
      twoScoopDataModel.ExpRewards.Add(expRewardDataModel);
    }
  }

  private void OnClosed()
  {
    Action onClosedCallback = this.m_onClosedCallback;
    if (onClosedCallback != null)
      onClosedCallback();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, 5f);
  }

  public void Initialize(List<MercenaryExpRewardData> mercenaryExpRewards, Action onClosedCallback)
  {
    this.m_mercenaryExpRewards = mercenaryExpRewards;
    this.m_onClosedCallback = onClosedCallback;
    this.BindMercenariesExperienceTwoScoopDataModel();
  }

  public void ResetData()
  {
    this.m_mercenaryExpRewards = (List<MercenaryExpRewardData>) null;
    this.m_onClosedCallback = (Action) null;
    this.m_initialized = false;
    this.m_rootWidget.TriggerEvent("RESET_DATA");
  }
}
