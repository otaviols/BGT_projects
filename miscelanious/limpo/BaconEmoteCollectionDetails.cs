using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class BaconEmoteCollectionDetails : BaconCollectionDetails
{
  [Tooltip("Reference to the GameObject on the image widget that contains the AnimationOverrideWidgetBehavior component")]
  [SerializeField]
  private AsyncReference m_asyncAnimationOverrideReference;
  [SerializeField]
  [Tooltip("Number of seconds to wait after the details view has fully scaled-up, before playing the emote")]
  [Min(0.0f)]
  private float m_entranceDelaySeconds;
  [SerializeField]
  [Tooltip("Number of seconds to wait in between consecutive loops of the emote")]
  [Min(0.0f)]
  private float m_intervalDelaySeconds;
  [Tooltip("If true, emote will pause on the first frame before starting. If false, emote will pause on the animation's configured display frame before starting")]
  [SerializeField]
  private bool m_shouldStartOnFirstFrame;
  [SerializeField]
  [Tooltip("If true, emote will transition to the first frame before starting the interval delay between loops (if one exists). If false, stay on last frame")]
  private bool m_shouldFinishOnFirstFrame;
  private BattlegroundsEmoteDataModel m_dataModel;
  private Animator m_animator;
  private AnimationOverrideWidgetBehaviour m_animationOverrideWidgetBehaviour;
  private WaitUntil m_waitUntilAnimationReady;
  private WaitForSeconds m_entranceWaitForSeconds;
  private WaitForSeconds m_intervalWaitForSeconds;
  private bool m_isAnimationReady;
  private int m_dataVersion;
  private float m_animationLength;
  private int m_animationHash;
  private float m_displayFrameNormalizedTime;
  private const string SetSpeechBubbleEventName = "DEFAULT_BOTTOM_LEFT";

  protected override string DebugTextValue => string.Format("Emote ID: {0}", (object) this.m_dataModel?.EmoteDbiId);

  private void Awake()
  {
    if (this.m_asyncAnimationOverrideReference != null)
      return;
    Debug.LogError((object) "BaconEmoteCollectionDetails: Missing required async AnimationOverrideWidgetBehaviour reference");
  }

  protected override void Start()
  {
    base.Start();
    this.CreateYieldInstructions();
    this.m_asyncAnimationOverrideReference.RegisterReadyListener<AnimationOverrideWidgetBehaviour>(new Action<AnimationOverrideWidgetBehaviour>(this.OnAnimationOverrideReady));
  }

  public override void AssignDataModels(IDataModel dataModel, IDataModel pageDataModel)
  {
    this.m_dataModel = dataModel as BattlegroundsEmoteDataModel;
    this.m_widget.BindDataModel(dataModel);
    this.m_widget.TriggerEvent("DEFAULT_BOTTOM_LEFT");
    if (!((UnityEngine.Object) this.m_animationOverrideWidgetBehaviour != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_animator != (UnityEngine.Object) null) || this.m_animationOverrideWidgetBehaviour.GetLocalDataVersion() != this.m_dataVersion)
      return;
    this.m_isAnimationReady = true;
  }

  private void OnAnimationOverrideReady(AnimationOverrideWidgetBehaviour animationOverride)
  {
    if ((UnityEngine.Object) animationOverride == (UnityEngine.Object) null || !animationOverride.TryGetComponent<Animator>(out this.m_animator))
    {
      Debug.LogError((object) "BaconEmoteCollectionDetails: Failed to load animation reference, animation may not play correctly.");
    }
    else
    {
      this.m_animationOverrideWidgetBehaviour = animationOverride;
      this.m_animationOverrideWidgetBehaviour.RegisterDoneChangingStatesListener((Action<object>) (_ => this.OnAnimationLoaded()), (object) null, true, false);
    }
  }

  private void OnAnimationLoaded()
  {
    this.m_dataVersion = this.m_animationOverrideWidgetBehaviour.GetLocalDataVersion();
    AnimatorStateInfo animatorStateInfo = this.m_animator.GetCurrentAnimatorStateInfo(0);
    this.m_animationLength = animatorStateInfo.length;
    this.m_animationHash = animatorStateInfo.fullPathHash;
    this.m_displayFrameNormalizedTime = animatorStateInfo.normalizedTime;
    this.m_isAnimationReady = true;
    if (!this.m_shouldStartOnFirstFrame)
      return;
    this.PauseAtNormalizedTime(0.0f);
  }

  protected override bool ValidateDataModels(IDataModel dataModel, IDataModel pageDataModel) => dataModel is BattlegroundsEmoteDataModel && pageDataModel is BattlegroundsEmoteCollectionPageDataModel;

  protected override void ClearDataModels() => this.m_dataModel = (BattlegroundsEmoteDataModel) null;

  protected override void DetailsEventListener(string eventName)
  {
    if (eventName == "OffDialogClick_code")
    {
      if (!this.CanHide())
        return;
      this.Hide();
    }
    else
      Debug.LogWarning((object) ("Unrecognized event handled in BaconEmoteCollectionDetails: " + eventName));
  }

  protected override void OnShowAnimationComplete(object objectData)
  {
    base.OnShowAnimationComplete(objectData);
    this.StartCoroutine(this.PlayEmoteOnLoop());
  }

  protected override void OnHideAnimationComplete(object objectData)
  {
    this.PauseAtNormalizedTime(this.m_shouldStartOnFirstFrame ? 0.0f : this.m_displayFrameNormalizedTime);
    this.m_isAnimationReady = false;
    base.OnHideAnimationComplete(objectData);
  }

  private void PauseAtNormalizedTime(float normalizedTime)
  {
    if ((UnityEngine.Object) this.m_animator == (UnityEngine.Object) null || !this.m_isAnimationReady)
      return;
    this.m_animator.Play(this.m_animationHash, -1, normalizedTime);
    this.m_animator.Update(0.0f);
    this.m_animator.enabled = false;
  }

  private IEnumerator PlayEmoteOnLoop()
  {
    BaconEmoteCollectionDetails collectionDetails = this;
    yield return (object) collectionDetails.m_entranceWaitForSeconds;
    yield return (object) collectionDetails.m_waitUntilAnimationReady;
    WaitForSeconds animationWaitForSeconds = new WaitForSeconds(collectionDetails.m_animationLength);
    while (collectionDetails.m_isShown)
    {
      collectionDetails.m_animator.enabled = true;
      collectionDetails.m_animator.Play(collectionDetails.m_animationHash, -1, 0.0f);
      yield return (object) animationWaitForSeconds;
      if (collectionDetails.m_shouldFinishOnFirstFrame)
        collectionDetails.PauseAtNormalizedTime(0.0f);
      yield return (object) collectionDetails.m_intervalWaitForSeconds;
    }
  }

  private void CreateYieldInstructions()
  {
    this.m_waitUntilAnimationReady = new WaitUntil((Func<bool>) (() => this.m_isAnimationReady));
    this.m_entranceWaitForSeconds = new WaitForSeconds(this.m_entranceDelaySeconds);
    this.m_intervalWaitForSeconds = new WaitForSeconds(this.m_intervalDelaySeconds);
  }
}
