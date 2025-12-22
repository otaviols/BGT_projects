using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class RankedRewardDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_startScale;
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_punchScale;
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_afterPunchScale;
  public PlayMakerFSM m_fsm;
  public PegUIElement m_debugClickCatcher;
  private Widget m_widget;
  private List<RewardListDataModel> m_rewardListDataModels = new List<RewardListDataModel>();
  private Action m_closedCallback;
  private bool m_isHidePending;
  private bool m_isAnimating;
  private int m_numAnimationsRemaining;
  private bool m_doPositionForDebugShow;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    this.Reset();
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null))
      return;
    EndGameScreen.Get().m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
  }

  public void Initialize(
    TranslatedMedalInfo medalInfo,
    List<List<RewardData>> rewardDataList,
    Action callback)
  {
    if (medalInfo == null)
      return;
    this.m_closedCallback = callback;
    this.m_widget.BindDataModel((IDataModel) medalInfo.CreateDataModel(RankedMedal.DisplayMode.Chest));
    foreach (List<RewardData> rewardData1 in rewardDataList)
    {
      RewardListDataModel rewardListDataModel = new RewardListDataModel();
      foreach (RewardData rewardData2 in rewardData1)
      {
        if (rewardData2 != null)
          rewardListDataModel.Items.Add(RewardUtils.RewardDataToRewardItemDataModel(rewardData2));
      }
      if (rewardListDataModel.Items.Count > 0)
        this.m_rewardListDataModels.Add(rewardListDataModel);
    }
    this.m_numAnimationsRemaining = this.m_rewardListDataModels.Count;
    this.BindNextRewardItemDataModel();
    this.m_widget.Hide();
  }

  [ContextMenu("Reset")]
  public void Reset()
  {
    this.m_debugClickCatcher.gameObject.SetActive(false);
    this.m_widget.Hide();
    this.m_isAnimating = false;
    this.m_isHidePending = false;
    this.m_fsm.SendEvent(nameof (Reset));
  }

  public void Show() => this.StartCoroutine(this.ShowWhenReady());

  private bool IsReady => (UnityEngine.Object) this.m_widget != (UnityEngine.Object) null && this.m_widget.IsReady && !this.m_widget.IsChangingStates;

  private IEnumerator ShowWhenReady()
  {
    RankedRewardDisplay rankedRewardDisplay = this;
    while (!rankedRewardDisplay.IsReady)
      yield return (object) null;
    if (rankedRewardDisplay.m_doPositionForDebugShow)
      rankedRewardDisplay.PositionForDebugShow();
    rankedRewardDisplay.m_widget.Show();
    AnimationUtil.ShowWithPunch(rankedRewardDisplay.gameObject, (Vector3) (MobileOverrideValue<Vector3>) rankedRewardDisplay.m_startScale, (Vector3) (MobileOverrideValue<Vector3>) rankedRewardDisplay.m_punchScale, (Vector3) (MobileOverrideValue<Vector3>) rankedRewardDisplay.m_afterPunchScale, "OnShown", true);
    rankedRewardDisplay.m_fsm.SendEvent("Birth");
  }

  private void OnShown()
  {
    if (!((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null))
      return;
    EndGameScreen.Get().m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
  }

  private void BindNextRewardItemDataModel()
  {
    if (this.m_rewardListDataModels.Count <= 0)
      return;
    RewardListDataModel rewardListDataModel = this.m_rewardListDataModels[0];
    this.m_rewardListDataModels.RemoveAt(0);
    this.m_widget.BindDataModel((IDataModel) rewardListDataModel);
  }

  private void OnPlayMakerNextRewardItem()
  {
    this.m_isAnimating = false;
    --this.m_numAnimationsRemaining;
    if (this.m_numAnimationsRemaining > 0)
    {
      this.BindNextRewardItemDataModel();
      this.m_widget.TriggerEvent("RevealRewardItem");
    }
    else
      this.SendPlayMakerDeath();
  }

  private void OnPlayMakerFinished() => this.Hide();

  private void OnClick(UIEvent e)
  {
    if (this.m_numAnimationsRemaining > 0)
    {
      if (this.m_isAnimating)
        return;
      this.m_widget.TriggerEvent("AnimateRewardItem");
      this.m_isAnimating = true;
    }
    else
      this.SendPlayMakerDeath();
  }

  private void SendPlayMakerDeath()
  {
    if (this.m_isHidePending)
      return;
    this.m_fsm.SendEvent("Death");
    this.m_isHidePending = true;
  }

  private void Hide()
  {
    if ((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null)
      EndGameScreen.Get().m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
    if (!((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null))
      return;
    AnimationUtil.ScaleFade(this.gameObject, new Vector3(0.01f, 0.01f, 0.01f), "OnClosed");
  }

  private void OnClosed()
  {
    Action closedCallback = this.m_closedCallback;
    if (closedCallback == null)
      return;
    closedCallback();
  }

  public static void DebugShowFake(
    int leagueId,
    int starLevel,
    FormatType formatType,
    List<List<RewardData>> rewardData)
  {
    TranslatedMedalInfo tmi = MedalInfoTranslator.CreateTranslatedMedalInfo(formatType, leagueId, starLevel, 1337);
    Widget widget = (Widget) WidgetInstance.Create((string) RankMgr.RANKED_REWARD_DISPLAY_PREFAB);
    widget.RegisterReadyListener((Action<object>) (_ =>
    {
      RankedRewardDisplay componentInChildren = widget.GetComponentInChildren<RankedRewardDisplay>();
      componentInChildren.ActivateDebugEquivalentsOfEndGameScreen();
      componentInChildren.Initialize(tmi, rewardData, new Action(componentInChildren.OnDebugShowComplete));
      componentInChildren.Show();
    }), (object) null, true);
  }

  private void ActivateDebugEquivalentsOfEndGameScreen()
  {
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
    this.m_debugClickCatcher.gameObject.SetActive(true);
    this.m_debugClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
    this.m_doPositionForDebugShow = true;
  }

  private void PositionForDebugShow()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.transform.localPosition = new Vector3(0.0f, 156.5f, 1.4f);
    else
      this.transform.localPosition = new Vector3(0.0f, 292f, -9f);
  }

  private void OnDebugShowComplete()
  {
    this.m_screenEffectsHandle.StopEffect();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.transform.parent.gameObject);
  }
}
