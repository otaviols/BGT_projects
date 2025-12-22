using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class MercenariesAbilityUnlockReward : Reward
{
  public AsyncReference m_mercenaryCardReference;
  public AsyncReference m_abilityCardReference;
  public AsyncReference m_rootWidgetReference;
  public AsyncReference m_unlockAbilitySuperReference;
  public AsyncReference m_blackBGReference;
  protected Widget m_mercenaryCardWidget;
  protected Widget m_abilityCardWidget;
  protected Widget m_rootWidget;
  protected bool m_hidden;
  protected PlayMakerFSM m_unlockAbilitySuperFsm;
  private ScreenEffectsHandle m_screenEffectsHandle;
  private const string FsmDeathEvent = "Death";

  protected override void Start()
  {
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    base.Start();
    this.m_mercenaryCardReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnMercenaryCardReady));
    this.m_abilityCardReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnAbilityCardReady));
    this.m_rootWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRootWidgetReady));
    this.m_unlockAbilitySuperReference.RegisterReadyListener<PlayMakerFSM>(new Action<PlayMakerFSM>(this.OnPlaymakerReady));
    this.m_blackBGReference.RegisterReadyListener<Transform>(new Action<Transform>(this.OnBlackBGReady));
  }

  private void OnMercenaryCardReady(Widget widget)
  {
    this.m_mercenaryCardWidget = widget;
    if ((UnityEngine.Object) this.m_mercenaryCardWidget == (UnityEngine.Object) null)
      return;
    this.m_mercenaryCardWidget.BindDataModel((IDataModel) MercenaryFactory.CreateEmptyMercenaryDataModel());
    if (!this.m_hidden)
      return;
    this.m_mercenaryCardWidget.Hide();
  }

  private void OnAbilityCardReady(Widget widget)
  {
    this.m_abilityCardWidget = widget;
    if ((UnityEngine.Object) this.m_abilityCardWidget == (UnityEngine.Object) null)
      return;
    this.m_abilityCardWidget.BindDataModel((IDataModel) new LettuceAbilityDataModel());
    if (!this.m_hidden)
      return;
    this.m_abilityCardWidget.Hide();
  }

  private void OnRootWidgetReady(Widget widget)
  {
    this.m_rootWidget = widget;
    if ((UnityEngine.Object) this.m_rootWidget == (UnityEngine.Object) null || !this.m_hidden)
      return;
    this.m_rootWidget.Hide();
  }

  private void OnPlaymakerReady(PlayMakerFSM playMaker)
  {
    this.m_unlockAbilitySuperFsm = playMaker;
    int num = (UnityEngine.Object) this.m_unlockAbilitySuperFsm == (UnityEngine.Object) null ? 1 : 0;
  }

  private void OnBlackBGReady(Transform bg)
  {
    if (!((UnityEngine.Object) bg != (UnityEngine.Object) null) || SceneMgr.Get().GetMode() != SceneMgr.Mode.LETTUCE_VILLAGE)
      return;
    bg.gameObject.SetActive(true);
  }

  protected override void InitData() => this.SetData((RewardData) new MercenariesAbilityUnlockRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || this.m_hidden || (UnityEngine.Object) this.m_mercenaryCardWidget == (UnityEngine.Object) null || (UnityEngine.Object) this.m_abilityCardWidget == (UnityEngine.Object) null)
      return;
    if (!(this.Data is MercenariesAbilityUnlockRewardData data))
    {
      Debug.LogWarning((object) string.Format("MercenariesAbilityUnlockReward.OnDataSet() - data {0} is not MercenariesAbilityUnlockRewardData", (object) this.Data));
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) data.MercenaryId);
      if (mercenary == null)
      {
        Debug.LogWarning((object) string.Format("MercenariesAbilityUnlockReward.OnDataSet() - No mercenary with id {0}", (object) data.MercenaryId));
      }
      else
      {
        CollectionUtils.PopulateMercenaryDataModel(this.m_mercenaryCardWidget.GetDataModel<LettuceMercenaryDataModel>(), mercenary, CollectionUtils.MercenaryDataPopluateExtra.None);
        LettuceAbility lettuceAbility = mercenary.GetLettuceAbility(data.AbilityId);
        if (lettuceAbility == null)
          Debug.LogWarning((object) string.Format("MercenariesAbilityUnlockReward.OnDataSet() - No lettuce ability found for ability id={0}", (object) data.AbilityId));
        else
          CollectionUtils.PopulateAbilityDataModel(this.m_abilityCardWidget.GetDataModel<LettuceAbilityDataModel>(), lettuceAbility, mercenary, (LettuceEquipmentModifierDataDbfRecord) null);
      }
    }
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_hidden = false;
    this.m_root.SetActive(true);
    this.StartCoroutine(this.ShowRewardWhenReady());
  }

  private IEnumerator ShowRewardWhenReady()
  {
    MercenariesAbilityUnlockReward abilityUnlockReward = this;
    while ((UnityEngine.Object) abilityUnlockReward.m_mercenaryCardWidget == (UnityEngine.Object) null || (UnityEngine.Object) abilityUnlockReward.m_abilityCardWidget == (UnityEngine.Object) null)
      yield return (object) null;
    abilityUnlockReward.m_mercenaryCardWidget.Show();
    abilityUnlockReward.m_abilityCardWidget.Show();
    abilityUnlockReward.m_rootWidget.Show();
    abilityUnlockReward.m_rootWidget.TriggerEvent("SHOW");
    abilityUnlockReward.OnDataSet(true);
    ScreenEffectParameters desaturatePerspective = ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Blur = new BlurParameters(brightness: 1f),
      Desaturate = new DesaturateParameters(0.0f),
      Time = 0.4f
    };
    abilityUnlockReward.m_screenEffectsHandle.StartEffect(desaturatePerspective);
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
    this.m_hidden = true;
    if ((UnityEngine.Object) this.m_mercenaryCardWidget != (UnityEngine.Object) null)
      this.m_mercenaryCardWidget.Hide();
    if ((UnityEngine.Object) this.m_abilityCardWidget != (UnityEngine.Object) null)
      this.m_abilityCardWidget.Hide();
    if ((UnityEngine.Object) this.m_unlockAbilitySuperFsm != (UnityEngine.Object) null)
      this.m_unlockAbilitySuperFsm.SendEvent("Death");
    this.m_screenEffectsHandle.StopEffect(RewardUtils.MercRewardEndBlurTime);
  }
}
