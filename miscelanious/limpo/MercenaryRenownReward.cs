using Hearthstone.UI;
using System;
using UnityEngine;

public class MercenaryRenownReward : Reward
{
  [SerializeField]
  private AsyncReference m_mercenaryRenownReference;
  protected Widget m_mercenaryRenownWidget;
  protected bool m_hidden;

  protected override void Start()
  {
    base.Start();
    this.m_mercenaryRenownReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnWidgetReady));
  }

  private void OnWidgetReady(Widget widget)
  {
    this.m_mercenaryRenownWidget = widget;
    if ((UnityEngine.Object) this.m_mercenaryRenownWidget == (UnityEngine.Object) null || !this.m_hidden)
      return;
    this.m_mercenaryRenownWidget.Hide();
  }

  protected override void InitData() => this.SetData((RewardData) new MercenaryRenownRewardData(), false);

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals || (UnityEngine.Object) this.m_mercenaryRenownWidget == (UnityEngine.Object) null)
      return;
    if (!(this.Data is MercenaryRenownRewardData))
      Debug.LogWarning((object) string.Format("MercenaryCoinReward.OnDataSet() - data {0} is not MercenaryCoinRewardData", (object) this.Data));
    else
      this.SetReady(false);
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_hidden = false;
    this.m_root.SetActive(true);
    if (!((UnityEngine.Object) this.m_mercenaryRenownWidget != (UnityEngine.Object) null))
      return;
    this.m_mercenaryRenownWidget.Show();
    this.OnDataSet(true);
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
    this.m_hidden = true;
    if (!((UnityEngine.Object) this.m_mercenaryRenownWidget != (UnityEngine.Object) null))
      return;
    this.m_mercenaryRenownWidget.Hide();
  }
}
