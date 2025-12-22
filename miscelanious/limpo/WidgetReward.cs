using Hearthstone.UI;
using System;

public abstract class WidgetReward : Reward
{
  public AsyncReference m_rewardWidgetReference;
  protected Widget m_rewardWidget;
  protected bool m_hidden;
  private const string PLAY_REWARD_UNLOCK_ANIM_EVENT_NAME = "PLAY_REWARD_UNLOCKED_ANIM";
  private const string STOP_REWARD_UNLOCK_ANIM_EVENT_NAME = "STOP_REWARD_UNLOCKED_ANIM";

  protected override void Start()
  {
    base.Start();
    this.m_rewardWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRewardWidgetReady));
  }

  private void OnRewardWidgetReady(Widget widget)
  {
    this.m_rewardWidget = widget;
    if ((UnityEngine.Object) this.m_rewardWidget == (UnityEngine.Object) null || !this.m_hidden)
      return;
    this.m_rewardWidget.Hide();
  }

  public override void Hide(bool animate = false)
  {
    if (this.m_shown)
      this.ScreenEffectsHandle.StopEffect();
    base.Hide(animate);
  }

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_hidden = false;
    this.m_rewardWidget.Show();
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_rewardWidget.TriggerEvent("PLAY_REWARD_UNLOCKED_ANIM");
    this.ScreenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
  }

  protected override void HideReward()
  {
    base.HideReward();
    if ((UnityEngine.Object) this.m_rewardWidget != (UnityEngine.Object) null)
    {
      this.m_rewardWidget.TriggerEvent("STOP_REWARD_UNLOCKED_ANIM");
      this.m_rewardWidget.Hide();
    }
    this.m_hidden = true;
  }
}
