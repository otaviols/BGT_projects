using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof (PlayMakerFSM))]
public class BattlegroundsEmoteNotification : Notification
{
  [SerializeField]
  [Header("-Battlegrounds Emote Parameters-")]
  private WidgetInstance m_widget;
  [SerializeField]
  private PlayMakerFSM m_playMakerFsm;
  [SerializeField]
  private AsyncReference m_asyncAnimationOverrideReference;
  [SerializeField]
  private AsyncReference m_asyncImageWidgetColliderReference;
  [SerializeField]
  [Header("-PlayMaker Parameters-")]
  [Tooltip("Delay at the start of the animation loop, after scaling up, holding the first frame of animation")]
  [Min(0.0f)]
  private float m_entranceDelaySeconds;
  [SerializeField]
  [Tooltip("Delay after the final loop of animation is played, before scaling down, holding the last frame of animation")]
  [Min(0.0f)]
  private float m_exitDelaySeconds;
  [Tooltip("If true, emote will pause on the first frame before starting. If false, emote will pause on the animation's configured display frame before starting")]
  [SerializeField]
  private bool m_shouldStartOnFirstFrame = true;
  private Animator m_animator;
  private bool m_isAnimationReady;
  private const string BirthEventName = "Birth";
  private const float CheckIfReadyIntervalSeconds = 0.1f;
  private const string SetSpeechBubbleEventName = "GAMEPLAY_LEFT";

  public float EntranceDelaySeconds => this.m_entranceDelaySeconds;

  public float ExitDelaySeconds => this.m_exitDelaySeconds;

  private void Awake()
  {
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      Debug.LogError((object) "BattlegroundsEmoteNotification: Missing required widget reference");
    if ((UnityEngine.Object) this.m_playMakerFsm == (UnityEngine.Object) null)
      Debug.LogError((object) "BattlegroundsEmoteNotification: Missing required PlayMakerFSM component");
    if (this.m_asyncAnimationOverrideReference == null)
      Debug.LogError((object) "BattlegroundsEmoteNotification: Missing required async AnimationOverrideWidgetBehaviour reference");
    if (this.m_asyncImageWidgetColliderReference != null)
      return;
    Debug.LogError((object) "BattlegroundsEmoteNotification: Missing a required async Collider reference");
  }

  private void Start()
  {
    this.m_widget.Hide();
    this.m_asyncAnimationOverrideReference.RegisterReadyListener<AnimationOverrideWidgetBehaviour>(new Action<AnimationOverrideWidgetBehaviour>(this.OnAnimationOverrideReady));
    this.m_asyncImageWidgetColliderReference.RegisterReadyListener<Collider>(new Action<Collider>(this.OnImageWidgetColliderReady));
  }

  public void BindEmoteDataModel(int battlegroundsEmoteId)
  {
    BattlegroundsEmoteDbfRecord record = GameDbf.BattlegroundsEmote.GetRecord(battlegroundsEmoteId);
    if (record == null)
    {
      Debug.Log((object) ("BattlegroundsEmoteNotification: No emote DBF record found for binding to " + this.m_widget.name + ". Emote notification will be empty."));
    }
    else
    {
      this.m_widget.BindDataModel((IDataModel) new BattlegroundsEmoteDataModel()
      {
        EmoteDbiId = record.ID,
        Animation = record.AnimationPath,
        IsAnimating = record.IsAnimating,
        BorderType = record.BorderType,
        XOffset = (float) record.XOffset,
        ZOffset = (float) record.ZOffset
      }, false);
      this.m_widget.TriggerEvent("GAMEPLAY_LEFT", new Widget.TriggerEventParameters());
    }
  }

  public override void PlayBirth() => Processor.RunCoroutine(this.WaitUntilReadyThenPlay());

  public void DestroyNotification() => NotificationManager.Get().DestroyNotification((Notification) this, 0.0f);

  public GameObject GetAnimatorGameObject() => !((UnityEngine.Object) this.m_animator != (UnityEngine.Object) null) ? (GameObject) null : this.m_animator.gameObject;

  public void EnableAnimatorComponent(bool isEnabled)
  {
    if ((UnityEngine.Object) this.m_animator == (UnityEngine.Object) null)
      return;
    this.m_animator.enabled = isEnabled;
  }

  private void OnImageWidgetColliderReady(Collider imageWidgetCollider)
  {
    if ((UnityEngine.Object) imageWidgetCollider == (UnityEngine.Object) null)
      Debug.LogError((object) "BattlegroundsEmoteNotification: Failed to load collider reference, notification will block raycasts.");
    else
      imageWidgetCollider.enabled = false;
  }

  private void OnAnimationOverrideReady(AnimationOverrideWidgetBehaviour animationOverride)
  {
    if ((UnityEngine.Object) animationOverride == (UnityEngine.Object) null || !animationOverride.TryGetComponent<Animator>(out this.m_animator))
      Debug.LogError((object) "BattlegroundsEmoteNotification: Failed to load animation reference, animation may not play correctly.");
    else
      animationOverride.RegisterDoneChangingStatesListener(new Action<object>(this.OnAnimationReady), (object) null, true, false);
  }

  private void OnAnimationReady(object _)
  {
    if (this.m_shouldStartOnFirstFrame)
    {
      this.m_animator.Play(0, -1, 0.0f);
      this.m_animator.Update(0.0f);
      this.m_animator.enabled = false;
    }
    this.m_isAnimationReady = true;
  }

  private IEnumerator WaitUntilReadyThenPlay()
  {
    while (!this.m_isAnimationReady || !this.m_asyncImageWidgetColliderReference.IsReady)
      yield return (object) new WaitForSeconds(0.1f);
    this.m_playMakerFsm.SendEvent("Birth");
  }
}
