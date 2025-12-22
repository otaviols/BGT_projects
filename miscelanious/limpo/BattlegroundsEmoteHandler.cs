using Hearthstone;
using Hearthstone.Core;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof (PlayMakerFSM), typeof (Collider))]
public class BattlegroundsEmoteHandler : MonoBehaviour
{
  [SerializeField]
  private BattlegroundsEmoteOption[] m_battlegroundsEmoteOptions;
  [SerializeField]
  private PlayMakerFSM m_visibilityPlayMakerFsm;
  [SerializeField]
  private Collider m_collider;
  private static BattlegroundsEmoteHandler s_instance;
  private BattlegroundsEmoteOption m_mousedOverOption;
  private bool m_emotesShown;
  private float m_timeLastEmoteSent;
  private int m_totalEmotesSent;
  private int m_chainedEmotesSent;
  private bool m_initialized;
  private bool m_isGameStateBusy;
  private int m_emoteOptionsReady;
  private const int EmoteCount = 6;
  private const float NetCacheQueryInterval = 2f;
  private const float MinTimeBetweenEmotes = 4f;
  private const int NumEmotesBeforeConsideredASpammer = 20;
  private const float SpammerMinTimeBetweenEmotes = 15f;
  private const int NumEmotesBeforeConsideredUberSpammer = 25;
  private const float UberSpammerMinTimeBetweenEmotes = 45f;
  private const int NumChainEmotesBeforeConsideredSpam = 2;
  private const float TimeWindowToBeConsideredAChain = 5f;
  private const string InitializeEventName = "INITIALIZE";
  private const string ShowEventName = "SHOW";
  private const string HideEventName = "HIDE";

  public bool IsMouseOverEmoteOption => (Object) this.m_mousedOverOption != (Object) null;

  private void Awake()
  {
    BattlegroundsEmoteHandler.s_instance = this;
    if (6 != this.m_battlegroundsEmoteOptions.Length)
      Debug.LogError((object) string.Format("{0}: Incorrect number of emote slots found. Expected {1}, counted {2}", (object) nameof (BattlegroundsEmoteHandler), (object) 6, (object) this.m_battlegroundsEmoteOptions.Length));
    if ((Object) this.m_visibilityPlayMakerFsm == (Object) null)
      Debug.LogError((object) "BattlegroundsEmoteHandler: Missing required PlaymakerFSM component");
    if (!((Object) this.m_collider == (Object) null))
      return;
    Debug.LogError((object) "BattlegroundsEmoteHandler: Missing required Collider component");
  }

  private void Start()
  {
    Processor.RunCoroutine(this.CheckNetCacheForEmoteLoadout());
    this.HideEmotes(true);
    GameState gameState = GameState.Get();
    this.m_isGameStateBusy = gameState != null && gameState.IsBusy();
    GameState.Get()?.RegisterBusyStateChangedListener(new GameState.BusyStateChangedCallback(this.OnBusyStateChanged));
  }

  private void OnDestroy()
  {
    BattlegroundsEmoteHandler.s_instance = (BattlegroundsEmoteHandler) null;
    GameState.Get()?.UnregisterBusyStateChangedListener(new GameState.BusyStateChangedCallback(this.OnBusyStateChanged));
    foreach (BattlegroundsEmoteOption battlegroundsEmoteOption in this.m_battlegroundsEmoteOptions)
    {
      if ((Object) battlegroundsEmoteOption != (Object) null)
        battlegroundsEmoteOption.UnregisterReadyListener(new BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback(this.OnBattlegroundsEmoteOptionReady));
    }
  }

  public static BattlegroundsEmoteHandler Get() => BattlegroundsEmoteHandler.s_instance;

  public static bool TryGetActiveInstance(out BattlegroundsEmoteHandler handler)
  {
    handler = BattlegroundsEmoteHandler.s_instance;
    return GameMgr.Get().IsBattlegroundsMatchOrTutorial() && (Object) BattlegroundsEmoteHandler.s_instance != (Object) null && BattlegroundsEmoteHandler.s_instance.AreEmotesActive();
  }

  public bool AreEmotesActive() => this.m_emotesShown;

  public void ShowEmotes()
  {
    if (this.m_emotesShown || this.m_isGameStateBusy || !this.m_initialized)
      return;
    this.m_visibilityPlayMakerFsm.SendEvent("SHOW");
    this.m_emotesShown = true;
    this.m_collider.enabled = true;
  }

  public void HideEmotes(bool shouldForceHide = false)
  {
    if (!this.m_emotesShown && !shouldForceHide)
      return;
    this.m_visibilityPlayMakerFsm.SendEvent("HIDE");
    this.m_mousedOverOption = (BattlegroundsEmoteOption) null;
    this.m_emotesShown = false;
    this.m_collider.enabled = false;
  }

  public void HandleMouseOver(BattlegroundsEmoteOption battlegroundsEmoteOption)
  {
    if ((Object) battlegroundsEmoteOption == (Object) null || (Object) this.m_mousedOverOption == (Object) battlegroundsEmoteOption)
      return;
    if ((Object) this.m_mousedOverOption != (Object) null)
      this.m_mousedOverOption.HandleMouseOut();
    this.m_mousedOverOption = battlegroundsEmoteOption;
    this.m_mousedOverOption.HandleMouseOver();
  }

  public void HandleMouseOut()
  {
    if ((Object) this.m_mousedOverOption == (Object) null)
      return;
    this.m_mousedOverOption.HandleMouseOut();
    this.m_mousedOverOption = (BattlegroundsEmoteOption) null;
  }

  public void HandleEmoteClicked()
  {
    if (!this.m_emotesShown || (Object) this.m_mousedOverOption == (Object) null || this.EmoteSpamBlocked())
      return;
    this.m_mousedOverOption.SendBattlegroundsEmote();
    ++this.m_totalEmotesSent;
    this.ResetTimeSinceLastEmote();
    Processor.RunCoroutine(this.BeginCooldownTimer());
    this.HideEmotes();
  }

  private IEnumerator CheckNetCacheForEmoteLoadout()
  {
    NetCache.NetCacheBattlegroundsEmotes netObject;
    for (netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheBattlegroundsEmotes>(); netObject == null; netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheBattlegroundsEmotes>())
      yield return (object) new WaitForSeconds(2f);
    this.CreateAndBindLoadoutDataModels(netObject);
  }

  private void CreateAndBindLoadoutDataModels(
    NetCache.NetCacheBattlegroundsEmotes battlegroundsEmotes)
  {
    BattlegroundsEmoteId[] emotes = battlegroundsEmotes.CurrentLoadout.Emotes;
    if (emotes.Length != this.m_battlegroundsEmoteOptions.Length)
      Debug.LogError((object) "BattlegroundsEmoteHandler: Emote loadout does not equal available UI slots. Filling slots up to capacity.");
    for (int index = 0; index < emotes.Length && index < this.m_battlegroundsEmoteOptions.Length; ++index)
    {
      BattlegroundsEmoteDbfRecord record = GameDbf.BattlegroundsEmote.GetRecord(emotes[index].ToValue());
      BattlegroundsEmoteOption battlegroundsEmoteOption = this.m_battlegroundsEmoteOptions[index];
      battlegroundsEmoteOption.BindAndInitializeWidget(record);
      battlegroundsEmoteOption.InvokeOrRegisterReadyListener(new BattlegroundsEmoteOption.BattlegroundsEmoteOptionReadyCallback(this.OnBattlegroundsEmoteOptionReady));
    }
  }

  private void OnBattlegroundsEmoteOptionReady()
  {
    if (++this.m_emoteOptionsReady != this.m_battlegroundsEmoteOptions.Length)
      return;
    this.InitializePlayMakers();
    this.m_initialized = true;
  }

  private void InitializePlayMakers() => this.m_visibilityPlayMakerFsm.SendEvent("INITIALIZE");

  private IEnumerator BeginCooldownTimer()
  {
    foreach (BattlegroundsEmoteOption battlegroundsEmoteOption in this.m_battlegroundsEmoteOptions)
      battlegroundsEmoteOption.SetCooldown(true);
    yield return (object) new WaitForSeconds(this.GetCooldownDuration());
    foreach (BattlegroundsEmoteOption battlegroundsEmoteOption in this.m_battlegroundsEmoteOptions)
      battlegroundsEmoteOption.SetCooldown(false);
  }

  private bool EmoteSpamBlocked() => !GameMgr.Get().IsFriendly() && !GameMgr.Get().IsAI() && (double) Time.time - (double) this.m_timeLastEmoteSent < (double) this.GetCooldownDuration();

  private void ResetTimeSinceLastEmote()
  {
    if ((double) Time.time - (double) this.m_timeLastEmoteSent < 9.0)
      ++this.m_chainedEmotesSent;
    else
      this.m_chainedEmotesSent = 0;
    this.m_timeLastEmoteSent = Time.time;
  }

  private float GetCooldownDuration()
  {
    if (this.m_totalEmotesSent >= 25)
      return 45f;
    return this.m_totalEmotesSent >= 20 || this.m_chainedEmotesSent >= 2 ? 15f : 4f;
  }

  private void OnBusyStateChanged(bool isGameStateBusy, object userData)
  {
    if (this.m_isGameStateBusy == isGameStateBusy)
      return;
    this.m_isGameStateBusy = isGameStateBusy;
    if (!(this.m_emotesShown & isGameStateBusy))
      return;
    this.HideEmotes();
  }
}
