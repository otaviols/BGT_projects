using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class MercenariesEquipmentReward : Reward
{
  public AsyncReference m_mercenaryCardReference;
  public AsyncReference m_equipmentCardReference;
  public AsyncReference m_rootWidgetReference;
  public AsyncReference m_unlockEquipmentSuperReference;
  protected Widget m_mercenaryCardWidget;
  protected Widget m_equipmentCardWidget;
  protected Widget m_rootWidget;
  protected bool m_hidden;
  protected PlayMakerFSM m_unlockEquipmentSuperFsm;
  private const string FsmDeathEvent = "Death";

  protected override void Start()
  {
    base.Start();
    this.m_mercenaryCardReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnMercenaryCardReady));
    this.m_equipmentCardReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnEquipmentCardReady));
    this.m_rootWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRootWidgetReady));
    this.m_unlockEquipmentSuperReference.RegisterReadyListener<PlayMakerFSM>(new Action<PlayMakerFSM>(this.OnPlaymakerReady));
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

  private void OnEquipmentCardReady(Widget widget)
  {
    this.m_equipmentCardWidget = widget;
    if ((UnityEngine.Object) this.m_equipmentCardWidget == (UnityEngine.Object) null)
      return;
    this.m_equipmentCardWidget.BindDataModel((IDataModel) new LettuceAbilityDataModel());
    if (!this.m_hidden)
      return;
    this.m_equipmentCardWidget.Hide();
  }

  private void OnRootWidgetReady(Widget widget)
  {
    this.m_rootWidget = widget;
    if ((UnityEngine.Object) this.m_rootWidget == (UnityEngine.Object) null)
      return;
    this.m_rootWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.RootWidgetEventListener));
    if (!this.m_hidden)
      return;
    this.m_rootWidget.Hide();
  }

  private void OnPlaymakerReady(PlayMakerFSM playmaker)
  {
    this.m_unlockEquipmentSuperFsm = playmaker;
    int num = (UnityEngine.Object) this.m_unlockEquipmentSuperFsm == (UnityEngine.Object) null ? 1 : 0;
  }

  private void RootWidgetEventListener(string eventName)
  {
    if (!(eventName == "PLAY_FTUE_EQUIPMENT_UNLOCK_code"))
      return;
    LettuceTutorialUtils.FireEvent(LettuceTutorialVo.LettuceTutorialEvent.FIRST_EQUIPMENT_UNLOCKED, this.gameObject);
  }

  protected override void InitData() => this.SetData((RewardData) new MercenariesEquipmentRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || this.m_hidden || (UnityEngine.Object) this.m_mercenaryCardWidget == (UnityEngine.Object) null || (UnityEngine.Object) this.m_equipmentCardWidget == (UnityEngine.Object) null)
      return;
    if (!(this.Data is MercenariesEquipmentRewardData data))
    {
      Debug.LogWarning((object) string.Format("MercenariesEquipmentUnlockReward.OnDataSet() - data {0} is not MercenariesEquipmentUnlockRewardData", (object) this.Data));
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) data.MercenaryId);
      if (mercenary == null)
      {
        Debug.LogWarning((object) string.Format("MercenariesEquipmentUnlockReward.OnDataSet() - No mercenary with id {0}", (object) data.MercenaryId));
      }
      else
      {
        CollectionUtils.PopulateMercenaryDataModel(this.m_mercenaryCardWidget.GetDataModel<LettuceMercenaryDataModel>(), mercenary, CollectionUtils.MercenaryDataPopluateExtra.None);
        LettuceEquipmentDbfRecord record = GameDbf.LettuceEquipment.GetRecord(data.EquipmentId);
        if (record == null)
        {
          Debug.LogWarning((object) string.Format("MercenariesEquipmentUnlockReward.OnDataSet() - No record found for equipment id={0}", (object) data.EquipmentId));
        }
        else
        {
          string str = (string) null;
          foreach (LettuceEquipmentTierDbfRecord lettuceEquipmentTier in record.LettuceEquipmentTiers)
          {
            if (lettuceEquipmentTier.Tier == data.EquipmentTier)
            {
              str = GameUtils.TranslateDbIdToCardId(lettuceEquipmentTier.CardId, true);
              break;
            }
          }
          if (string.IsNullOrEmpty(str))
          {
            Debug.LogWarning((object) string.Format("MercenariesEquipmentUnlockReward.OnDataSet() - No card for equipment id={0}, tier={1}", (object) data.EquipmentId, (object) data.EquipmentTier));
          }
          else
          {
            LettuceAbility lettuceEquipment = mercenary.GetLettuceEquipment(data.EquipmentId);
            CollectionUtils.PopulateDefaultAbilityDataModelWithTier(this.m_equipmentCardWidget.GetDataModel<LettuceAbilityDataModel>(), lettuceEquipment, mercenary, data.EquipmentTier);
          }
        }
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
    MercenariesEquipmentReward mercenariesEquipmentReward = this;
    while ((UnityEngine.Object) mercenariesEquipmentReward.m_mercenaryCardWidget == (UnityEngine.Object) null || (UnityEngine.Object) mercenariesEquipmentReward.m_equipmentCardWidget == (UnityEngine.Object) null)
      yield return (object) null;
    mercenariesEquipmentReward.m_mercenaryCardWidget.Show();
    mercenariesEquipmentReward.m_equipmentCardWidget.Show();
    mercenariesEquipmentReward.m_rootWidget.Show();
    mercenariesEquipmentReward.m_rootWidget.TriggerEvent("SHOW");
    mercenariesEquipmentReward.OnDataSet(true);
    mercenariesEquipmentReward.EnableClickCatcher(true);
    UIContext.GetRoot().ShowPopup(mercenariesEquipmentReward.gameObject);
  }

  protected override void HideReward()
  {
    if ((UnityEngine.Object) this.m_rootWidget != (UnityEngine.Object) null)
      UIContext.GetRoot().DismissPopup(this.gameObject);
    base.HideReward();
    this.m_root.SetActive(false);
    this.m_hidden = true;
    if ((UnityEngine.Object) this.m_mercenaryCardWidget != (UnityEngine.Object) null)
      this.m_mercenaryCardWidget.Hide();
    if ((UnityEngine.Object) this.m_equipmentCardWidget != (UnityEngine.Object) null)
      this.m_equipmentCardWidget.Hide();
    if (!((UnityEngine.Object) this.m_unlockEquipmentSuperFsm != (UnityEngine.Object) null))
      return;
    this.m_unlockEquipmentSuperFsm.SendEvent("Death");
  }
}
