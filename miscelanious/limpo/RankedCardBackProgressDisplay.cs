using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class RankedCardBackProgressDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_startScale;
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_punchScale;
  [CustomEditField(Sections = "Animate In")]
  public Vector3_MobileOverride m_afterPunchScale;
  [CustomEditField(Sections = "Progress Bar")]
  public float m_progressBarAnimTime = 2f;
  public PlayMakerFSM m_fsm;
  public PegUIElement m_debugClickCatcher;
  public UberText m_footerText;
  private Widget m_widget;
  private ProgressBar m_progressBar;
  private MedalInfoTranslator m_medalInfo;
  private int m_winsNeeded;
  private int m_prevWins;
  private int m_currWins;
  private Action m_closedCallback;
  private bool m_isDebugShow;
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

  private bool IsReady => (UnityEngine.Object) this.m_widget != (UnityEngine.Object) null && this.m_widget.IsReady && !this.m_widget.IsChangingStates;

  public void Initialize(MedalInfoTranslator medalInfo, Action callback)
  {
    if (medalInfo == null)
      return;
    this.m_medalInfo = medalInfo;
    this.m_closedCallback = callback;
    int currentSeasonId = this.m_medalInfo.GetCurrentSeasonId();
    this.m_winsNeeded = this.m_medalInfo.GetSeasonCardBackMinWins();
    this.m_prevWins = Mathf.Min(this.m_medalInfo.TotalRankedWinsPrevious, this.m_winsNeeded);
    this.m_currWins = Mathf.Min(this.m_medalInfo.TotalRankedWins, this.m_winsNeeded);
    this.m_widget.BindDataModel((IDataModel) this.m_medalInfo.CreateDataModel(Options.GetFormatType(), RankedMedal.DisplayMode.Default, hasEarnedCardBack: this.m_medalInfo.HasEarnedSeasonCardBack()));
    this.m_widget.BindDataModel((IDataModel) new CardBackDataModel()
    {
      CardBackId = RankMgr.Get().GetRankedCardBackIdForSeasonId(currentSeasonId)
    });
    this.m_widget.Hide();
  }

  [ContextMenu("Reset")]
  public void Reset()
  {
    this.m_debugClickCatcher.gameObject.SetActive(false);
    this.m_widget.Hide();
    this.m_fsm.SendEvent(nameof (Reset));
  }

  public void Show() => this.StartCoroutine(this.ShowWhenReady());

  private IEnumerator ShowWhenReady()
  {
    RankedCardBackProgressDisplay backProgressDisplay = this;
    while (!backProgressDisplay.IsReady)
      yield return (object) null;
    if (backProgressDisplay.m_isDebugShow)
      backProgressDisplay.PositionForDebugShow();
    float progress = (float) backProgressDisplay.m_prevWins / (float) backProgressDisplay.m_winsNeeded;
    backProgressDisplay.m_progressBar = backProgressDisplay.m_widget.GetComponentInChildren<ProgressBar>();
    if ((UnityEngine.Object) backProgressDisplay.m_progressBar != (UnityEngine.Object) null)
    {
      backProgressDisplay.m_progressBar.SetLabel(GameStrings.Format("GLOBAL_REWARD_PROGRESS", (object) backProgressDisplay.m_prevWins, (object) backProgressDisplay.m_winsNeeded));
      backProgressDisplay.m_progressBar.SetProgressBar(progress);
    }
    backProgressDisplay.m_footerText.Text = GameStrings.Format("GLOBAL_REMINDER_CARDBACK_SEASON_END_DIALOG", (object) backProgressDisplay.m_medalInfo.GetSeasonCardBackWinsRemaining());
    backProgressDisplay.m_widget.Show();
    AnimationUtil.ShowWithPunch(backProgressDisplay.gameObject, (Vector3) (MobileOverrideValue<Vector3>) backProgressDisplay.m_startScale, (Vector3) (MobileOverrideValue<Vector3>) backProgressDisplay.m_punchScale, (Vector3) (MobileOverrideValue<Vector3>) backProgressDisplay.m_afterPunchScale, "OnShown", true);
    backProgressDisplay.m_fsm.SendEvent("Birth");
  }

  private void OnShown()
  {
    if ((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null)
      EndGameScreen.Get().m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
    if (this.m_currWins <= this.m_prevWins)
      return;
    if ((UnityEngine.Object) this.m_progressBar != (UnityEngine.Object) null)
    {
      float currVal = (float) this.m_currWins / (float) this.m_winsNeeded;
      this.m_progressBar.m_increaseAnimTime = this.m_progressBarAnimTime;
      this.m_progressBar.AnimateProgress(this.m_progressBar.Progress, currVal, iTween.EaseType.easeInOutQuad);
    }
    this.StartCoroutine(this.WaitThenTriggerPlayMaker(this.m_progressBarAnimTime / (float) this.m_winsNeeded));
  }

  private IEnumerator WaitThenTriggerPlayMaker(float delay)
  {
    yield return (object) new WaitForSeconds(delay);
    this.m_progressBar.SetLabel(GameStrings.Format("GLOBAL_REWARD_PROGRESS", (object) this.m_currWins, (object) this.m_winsNeeded));
    if (this.m_currWins > this.m_prevWins && this.m_currWins >= this.m_winsNeeded)
      this.m_fsm.SendEvent("StartAnim");
  }

  private void OnPlayMakerFinished() => this.Hide();

  private void OnClick(UIEvent e)
  {
    this.m_fsm.SendEvent("Death");
    this.m_widget.TriggerEvent("HIDE_FOOTER_TEXT");
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

  public static void DebugShowFake(MedalInfoTranslator medalInfo)
  {
    Widget widget = (Widget) WidgetInstance.Create((string) RankMgr.RANKED_CARDBACK_PROGRESS_DISPLAY_PREFAB);
    widget.RegisterReadyListener((Action<object>) (_ =>
    {
      RankedCardBackProgressDisplay componentInChildren = widget.GetComponentInChildren<RankedCardBackProgressDisplay>();
      componentInChildren.ActivateDebugEquivalentsOfEndGameScreen();
      componentInChildren.Initialize(medalInfo, new Action(componentInChildren.OnDebugShowComplete));
      componentInChildren.Show();
    }), (object) null, true);
  }

  private void ActivateDebugEquivalentsOfEndGameScreen()
  {
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
    this.m_debugClickCatcher.gameObject.SetActive(true);
    this.m_debugClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
    this.m_isDebugShow = true;
  }

  private void PositionForDebugShow()
  {
    Camera mainCamera = CameraUtils.GetMainCamera();
    this.transform.localPosition = mainCamera.transform.position + (mainCamera.nearClipPlane + (float) (0.0399999991059303 * ((double) mainCamera.farClipPlane - (double) mainCamera.nearClipPlane))) * mainCamera.transform.forward;
  }

  private void OnDebugShowComplete()
  {
    this.m_screenEffectsHandle.StopEffect();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.transform.parent.gameObject);
  }
}
