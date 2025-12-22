using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (PlayMakerFSM), typeof (Collider))]
public class BattlegroundsEmoteOption : MonoBehaviour
{
  [SerializeField]
  private PlayMakerFSM m_initialStatePlayMakerFSM;
  [SerializeField]
  private PlayMakerFSM m_mouseOverPlayMakerFSM;
  [SerializeField]
  private Widget m_widget;
  [SerializeField]
  private bool m_hasLeftSideBubble;
  [SerializeField]
  private Collider m_collider;
  [SerializeField]
  private AsyncReference m_asyncEmoteSpriteReference;
  [SerializeField]
  private AsyncReference m_asyncImageWidgetColliderReference;
  [SerializeField]
  private AsyncReference m_asyncAnimationOverrideReference;
  private int m_emoteId;
  private bool m_isSlotEmpty;
  private bool m_isAnimationLoaded;
  private const string InitializeEventName = "INITIALIZE";
  private const string EmptyEventName = "SLOT_EMPTY";
  private const string FilledEventName = "SLOT_FILLED";
  private const string MouseOverEventName = "MOUSE_OVER";
  private const string MouseOutEventName = "MOUSE_OUT";
  private const string OnCooldownBottomLeftEventName = "ON_COOLDOWN_BOTTOM_LEFT";
  private const string OnCooldownBottomRightEventName = "ON_COOLDOWN_BOTTOM_RIGHT";
  private const string OffCooldownBottomLeftEventName = "OFF_COOLDOWN_BOTTOM_LEFT";
  private const string OffCooldownBottomRightEventName = "OFF_COOLDOWN_BOTTOM_RIGHT";
  private const string SetBottomRightSpeechBubbleEventName = "GAMEPLAY_BOTTOM_RIGHT";
  private const string SetBottomLeftSpeechBubbleEventName = "GAMEPLAY_BOTTOM_LEFT";

  public GameObject EmoteSpriteGameObject { get; private set; }

  public GameObject BubbleSpriteGameObject { get; private set; }

  public GameObject ImageWidgetColliderGameObject { get; private set; }

  private event BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback OnBattlegroundsEmoteOptionReady;

  private void Awake()
  {
    if ((UnityEngine.Object) this.m_initialStatePlayMakerFSM == (UnityEngine.Object) null || (UnityEngine.Object) this.m_mouseOverPlayMakerFSM == (UnityEngine.Object) null)
      Debug.LogError((object) "BattlegroundsEmoteOption: Missing required PlaymakerFSM component");
    if (this.m_asyncImageWidgetColliderReference == null || this.m_asyncEmoteSpriteReference == null)
      Debug.LogError((object) "BattlegroundsEmoteOption: Missing a required AsyncReference to an image widget component");
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      Debug.LogError((object) "BattlegroundsEmoteOption: Missing required Widget component");
    if (!((UnityEngine.Object) this.m_collider == (UnityEngine.Object) null))
      return;
    Debug.LogError((object) "BattlegroundsEmoteOption: Missing required Collider component");
  }

  private void Start()
  {
    this.m_asyncEmoteSpriteReference.RegisterReadyListener<SpriteRenderer>(new Action<SpriteRenderer>(this.OnEmoteSpriteReady));
    this.m_asyncImageWidgetColliderReference.RegisterReadyListener<Collider>(new Action<Collider>(this.OnImageWidgetColliderReady));
    this.m_asyncAnimationOverrideReference.RegisterReadyListener<AnimationOverrideWidgetBehaviour>(new Action<AnimationOverrideWidgetBehaviour>(this.OnAnimationOverrideReady));
  }

  public void BindAndInitializeWidget(BattlegroundsEmoteDbfRecord dbfRecord)
  {
    if (dbfRecord == null)
    {
      Debug.Log((object) ("BattlegroundsEmoteHandler: No emote DBF record found for binding to " + this.m_widget.name + ". Slot will be empty."));
      this.m_isSlotEmpty = true;
    }
    else
    {
      this.m_widget.BindDataModel((IDataModel) new BattlegroundsEmoteDataModel()
      {
        EmoteDbiId = dbfRecord.ID,
        Animation = dbfRecord.AnimationPath,
        IsAnimating = dbfRecord.IsAnimating,
        BorderType = dbfRecord.BorderType,
        XOffset = (float) dbfRecord.XOffset,
        ZOffset = (float) dbfRecord.ZOffset,
        Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) dbfRecord.Rarity))
      });
      this.m_widget.TriggerEvent(this.m_hasLeftSideBubble ? "GAMEPLAY_BOTTOM_RIGHT" : "GAMEPLAY_BOTTOM_LEFT");
      this.m_emoteId = dbfRecord.ID;
      this.m_isSlotEmpty = false;
    }
    this.InvokeOrRegisterReadyListener(new BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback(this.SetInitialImageWidgetState));
  }

  public void InvokeOrRegisterReadyListener(
    BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback listener)
  {
    if (this.IsReadyToShow())
    {
      if (listener == null)
        return;
      listener();
    }
    else
      this.RegisterReadyListener(listener);
  }

  public void RegisterReadyListener(
    BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback listener)
  {
    this.OnBattlegroundsEmoteOptionReady += listener;
  }

  public void UnregisterReadyListener(
    BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback listener)
  {
    this.OnBattlegroundsEmoteOptionReady -= listener;
  }

  public void SendBattlegroundsEmote()
  {
    if (GameMgr.Get().IsBattlegroundsTutorial())
    {
      int playerId = GameState.Get().GetFriendlySidePlayer().GetPlayerId();
      GameState.Get().GetGameEntity().PlayAlternateEnemyEmote(playerId, EmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE, this.m_emoteId);
    }
    else
      Network.Get().SendBattlegroundsEmote(EmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE, this.m_emoteId);
  }

  public void SetCooldown(bool isOnCooldown)
  {
    if (isOnCooldown)
      this.m_widget.TriggerEvent(this.m_hasLeftSideBubble ? "ON_COOLDOWN_BOTTOM_LEFT" : "ON_COOLDOWN_BOTTOM_RIGHT");
    else
      this.m_widget.TriggerEvent(this.m_hasLeftSideBubble ? "OFF_COOLDOWN_BOTTOM_LEFT" : "OFF_COOLDOWN_BOTTOM_RIGHT");
  }

  public void HandleMouseOver() => this.m_mouseOverPlayMakerFSM.SendEvent("MOUSE_OVER");

  public void HandleMouseOut() => this.m_mouseOverPlayMakerFSM.SendEvent("MOUSE_OUT");

  private void SetInitialImageWidgetState()
  {
    this.InitializePlayMakers();
    this.m_initialStatePlayMakerFSM.SendEvent(this.m_isSlotEmpty ? "SLOT_EMPTY" : "SLOT_FILLED");
  }

  private void InitializePlayMakers()
  {
    this.m_mouseOverPlayMakerFSM.SendEvent("INITIALIZE");
    this.m_initialStatePlayMakerFSM.SendEvent("INITIALIZE");
  }

  private bool IsReadyToShow() => (this.m_isAnimationLoaded || this.m_isSlotEmpty) && this.m_asyncEmoteSpriteReference.IsReady && this.m_asyncImageWidgetColliderReference.IsReady;

  private void NotifyListenersIfReady()
  {
    if (!this.IsReadyToShow())
      return;
    BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback emoteOptionReady = this.OnBattlegroundsEmoteOptionReady;
    if (emoteOptionReady == null)
      return;
    emoteOptionReady();
  }

  private void OnEmoteSpriteReady(SpriteRenderer emoteSprite)
  {
    if ((UnityEngine.Object) emoteSprite == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "BattlegroundsEmoteOption: Failed to load emote sprite reference.");
    }
    else
    {
      this.EmoteSpriteGameObject = emoteSprite.gameObject;
      this.NotifyListenersIfReady();
    }
  }

  private void OnImageWidgetColliderReady(Collider imageWidgetCollider)
  {
    if ((UnityEngine.Object) imageWidgetCollider == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "BattlegroundsEmoteOption: Failed to load image widget collider reference.");
    }
    else
    {
      this.ImageWidgetColliderGameObject = imageWidgetCollider.gameObject;
      this.NotifyListenersIfReady();
    }
  }

  private void OnAnimationOverrideReady(AnimationOverrideWidgetBehaviour animationOverride)
  {
    if ((UnityEngine.Object) animationOverride == (UnityEngine.Object) null)
      Debug.LogError((object) "BattlegroundsEmoteOption: Failed to load animation override reference.");
    else
      animationOverride.RegisterDoneChangingStatesListener((Action<object>) (_ => this.OnAnimationLoaded()), (object) null, true, false);
  }

  private void OnAnimationLoaded()
  {
    this.m_isAnimationLoaded = true;
    this.NotifyListenersIfReady();
  }

  public delegate void BattlegroundsEmoteOptionReadyCallback();
}
