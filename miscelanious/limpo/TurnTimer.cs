using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class TurnTimer : MonoBehaviour
{
  public float m_DebugTimeout = 30f;
  public float m_DebugTimeoutStart = 20f;
  public float m_RopeCapSeconds = 20f;
  public GameObject m_SparksObject;
  public Transform m_SparksStartBone;
  public Transform m_SparksFinishBone;
  public UberText m_CountdownText;
  public Color m_CountdownTextColorNormal;
  public Color m_CountdownTextColorRope;
  public GameObject m_FuseWickObject;
  public GameObject m_FuseShadowObject;
  public string m_FuseMatValName = "_Xamount";
  public float m_FuseMatValStart = 0.42f;
  public float m_FuseMatValFinish = -1.5f;
  public float m_FuseXamountAnimation = -1.5f;
  public SoundDef m_TickSound;
  public SoundDef m_FinalTickSound;
  private const float BIRTH_ANIMATION_TIME = 1f;
  private static TurnTimer s_instance;
  private Spell m_spell;
  private TurnTimerState m_state;
  private float m_countdownTimeoutSec;
  private float m_countdownEndTimestamp;
  private uint m_currentMoveAnimId;
  private uint m_currentMatValAnimId;
  private bool m_currentTimerBelongsToFriendlySidePlayer;
  private bool m_waitingForTurnStartManagerFinish;
  private int m_lastTickSecondNumber;
  private Coroutine m_countdownAnimsWhenBelowCapCoroutine;
  private TurnTimerGameModeSettings m_gameModeSettings;

  private void Awake()
  {
    TurnTimer.s_instance = this;
    this.m_spell = this.GetComponent<Spell>();
    this.m_spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
    if (GameState.Get() != null)
    {
      GameState.Get().RegisterCurrentPlayerChangedListener(new GameState.CurrentPlayerChangedCallback(this.OnCurrentPlayerChanged));
      GameState.Get().RegisterFriendlyTurnStartedListener(new GameState.FriendlyTurnStartedCallback(this.OnFriendlyTurnStarted));
      GameState.Get().RegisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
      GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    }
    this.SetGameModeSettings(new TurnTimerGameModeSettings());
  }

  private void OnDestroy()
  {
    TurnTimer.s_instance = (TurnTimer) null;
    if (GameState.Get() == null)
      return;
    GameState.Get().UnregisterCurrentPlayerChangedListener(new GameState.CurrentPlayerChangedCallback(this.OnCurrentPlayerChanged));
    GameState.Get().UnregisterFriendlyTurnStartedListener(new GameState.FriendlyTurnStartedCallback(this.OnFriendlyTurnStarted));
    GameState.Get().UnregisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
    GameState.Get().UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
  }

  private void Update() => this.UpdateCountdownText();

  public static TurnTimer Get() => TurnTimer.s_instance;

  public bool HasCountdownTimeout() => (double) this.m_countdownTimeoutSec > (double) Mathf.Epsilon;

  public void OnEndTurnRequested()
  {
    if (!this.HasCountdownTimeout())
      return;
    this.ChangeState(TurnTimerState.KILL);
  }

  public void OnMercenariesPhaseChange()
  {
    if (this.m_state != TurnTimerState.COUNTDOWN && this.m_state != TurnTimerState.START)
      return;
    this.ChangeState(TurnTimerState.KILL);
  }

  public bool IsRopeActive() => this.m_state == TurnTimerState.COUNTDOWN;

  public void SetGameModeSettings(TurnTimerGameModeSettings settings)
  {
    this.m_gameModeSettings = settings;
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    if ((Object) component == (Object) null)
    {
      Debug.LogError((object) "No playmaker attached to TurnTimer!");
    }
    else
    {
      component.FsmVariables.GetFsmBool("PlayTimeoutFx").Value = settings.m_PlayTimeoutFx;
      component.FsmVariables.GetFsmBool("PlayMusicStinger").Value = settings.m_PlayMusicStinger;
      component.FsmVariables.GetFsmFloat("RopeFuseVolume").Value = settings.m_RopeFuseVolume;
      component.FsmVariables.GetFsmFloat("RopeRolloutVolume").Value = settings.m_RopeRolloutVolume;
      component.FsmVariables.GetFsmFloat("EndTurnButtonExplosionVolume").Value = settings.m_EndTurnButtonExplosionVolume;
    }
  }

  private void ChangeState(TurnTimerState state) => this.ChangeSpellState(state);

  private void ChangeStateImpl(TurnTimerState state)
  {
    switch (state)
    {
      case TurnTimerState.START:
        this.ChangeState_Start();
        break;
      case TurnTimerState.COUNTDOWN:
        this.ChangeState_Countdown();
        break;
      case TurnTimerState.TIMEOUT:
        this.ChangeState_Timeout();
        break;
      case TurnTimerState.KILL:
        this.ChangeState_Kill();
        break;
    }
  }

  private void ChangeState_Start()
  {
    this.m_state = TurnTimerState.START;
    if (GameState.Get() == null || GameState.Get().GetCurrentPlayer() == null)
      return;
    Card heroCard = GameState.Get().GetCurrentPlayer().GetHeroCard();
    if ((Object) heroCard != (Object) null)
      heroCard.PlayEmote(EmoteType.TIME);
    this.m_currentTimerBelongsToFriendlySidePlayer = GameState.Get().IsFriendlySidePlayerTurn();
  }

  private void ChangeState_Countdown()
  {
    this.m_state = TurnTimerState.COUNTDOWN;
    this.m_countdownTimeoutSec = this.ComputeCountdownRemainingSec();
    this.StartCountdownAnimsWhenBelowCap(this.m_countdownTimeoutSec);
  }

  private void ChangeState_Timeout()
  {
    this.m_state = TurnTimerState.TIMEOUT;
    this.m_countdownEndTimestamp = 0.0f;
    if ((Object) EndTurnButton.Get() != (Object) null)
      EndTurnButton.Get().OnTurnTimerEnded(this.m_currentTimerBelongsToFriendlySidePlayer);
    GameState.Get()?.GetGameEntity()?.OnTurnTimerEnded(this.m_currentTimerBelongsToFriendlySidePlayer);
    this.StopCountdownAnims();
    double num = (double) this.UpdateCountdownAnims(0.0f);
  }

  private void ChangeState_Kill()
  {
    this.m_state = TurnTimerState.KILL;
    this.m_countdownEndTimestamp = 0.0f;
    this.StopCountdownAnims();
    double num = (double) this.UpdateCountdownAnims(0.0f);
  }

  private void ChangeSpellState(TurnTimerState timerState)
  {
    this.m_spell.ActivateState(this.TranslateTimerStateToSpellState(timerState));
    if (timerState != TurnTimerState.START)
      return;
    this.StartCoroutine(this.TimerBirthAnimateMaterialValues());
  }

  private IEnumerator TimerBirthAnimateMaterialValues()
  {
    float endTime = Time.timeSinceLevelLoad + 1f;
    while ((double) Time.timeSinceLevelLoad < (double) endTime)
    {
      this.OnUpdateFuseMatVal(this.m_FuseXamountAnimation);
      yield return (object) null;
    }
  }

  private void OnSpellStateStarted(Spell spell, SpellStateType prevStateType, object userData) => this.ChangeStateImpl(this.TranslateSpellStateToTimerState(spell.GetActiveState()));

  private SpellStateType TranslateTimerStateToSpellState(TurnTimerState timerState)
  {
    switch (timerState)
    {
      case TurnTimerState.START:
        return SpellStateType.BIRTH;
      case TurnTimerState.COUNTDOWN:
        return SpellStateType.IDLE;
      case TurnTimerState.TIMEOUT:
        return SpellStateType.DEATH;
      case TurnTimerState.KILL:
        return SpellStateType.CANCEL;
      default:
        return SpellStateType.NONE;
    }
  }

  private TurnTimerState TranslateSpellStateToTimerState(SpellStateType spellState)
  {
    switch (spellState)
    {
      case SpellStateType.BIRTH:
        return TurnTimerState.START;
      case SpellStateType.IDLE:
        return TurnTimerState.COUNTDOWN;
      case SpellStateType.CANCEL:
        return TurnTimerState.KILL;
      case SpellStateType.DEATH:
        return TurnTimerState.TIMEOUT;
      default:
        return TurnTimerState.NONE;
    }
  }

  private bool ShouldUpdateCountdownRemaining() => this.m_state == TurnTimerState.COUNTDOWN;

  private void StopCountdownAnims()
  {
    iTween.StopByName(this.m_SparksObject, this.GenerateMoveAnimName());
    iTween.StopByName(this.m_FuseWickObject, this.GenerateMatValAnimName());
  }

  private float UpdateCountdownAnims(float countdownRemainingSec)
  {
    float countdownProgress = this.ComputeCountdownProgress(countdownRemainingSec);
    this.m_SparksObject.transform.position = Vector3.Lerp(this.m_SparksFinishBone.position, this.m_SparksStartBone.position, countdownProgress);
    float num = Mathf.Lerp(this.m_FuseMatValFinish, this.m_FuseMatValStart, countdownProgress);
    this.m_FuseWickObject.GetComponent<Renderer>().GetMaterial().SetFloat(this.m_FuseMatValName, num);
    this.m_FuseShadowObject.GetComponent<Renderer>().GetMaterial().SetFloat(this.m_FuseMatValName, num);
    return num;
  }

  private void StartCountdownAnimsWhenBelowCap(float countdownRemainingSec)
  {
    if (this.m_countdownAnimsWhenBelowCapCoroutine != null)
      this.StopCoroutine(this.m_countdownAnimsWhenBelowCapCoroutine);
    this.m_countdownAnimsWhenBelowCapCoroutine = this.StartCoroutine(this.StartCountdownAnimsWhenBelowCapCoroutine(countdownRemainingSec));
  }

  private IEnumerator StartCountdownAnimsWhenBelowCapCoroutine(
    float countdownRemainingSec)
  {
    float secondsRemaining = countdownRemainingSec;
    if ((double) countdownRemainingSec > (double) this.m_RopeCapSeconds)
    {
      yield return (object) new WaitForSecondsRealtime(countdownRemainingSec - this.m_RopeCapSeconds);
      secondsRemaining = this.m_RopeCapSeconds;
    }
    this.HandleTurnTimerUpdateAnims(secondsRemaining);
    this.m_countdownAnimsWhenBelowCapCoroutine = (Coroutine) null;
  }

  private void StartCountdownAnims(float startingMatVal, float countdownRemainingSec)
  {
    this.m_lastTickSecondNumber = Mathf.CeilToInt(this.m_RopeCapSeconds);
    ++this.m_currentMoveAnimId;
    ++this.m_currentMatValAnimId;
    iTween.MoveTo(this.m_SparksObject, iTween.Hash((object) "name", (object) this.GenerateMoveAnimName(), (object) "time", (object) countdownRemainingSec, (object) "position", (object) this.m_SparksFinishBone.position, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
    iTween.ValueTo(this.m_FuseWickObject, iTween.Hash((object) "name", (object) this.GenerateMatValAnimName(), (object) "time", (object) countdownRemainingSec, (object) "from", (object) startingMatVal, (object) "to", (object) this.m_FuseMatValFinish, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) "OnUpdateFuseMatVal", (object) "onupdatetarget", (object) this.gameObject));
  }

  private string GenerateMoveAnimName() => string.Format("SparksMove{0}", (object) this.m_currentMoveAnimId);

  private string GenerateMatValAnimName() => string.Format("FuseMatVal{0}", (object) this.m_currentMatValAnimId);

  private void OnUpdateFuseMatVal(float val)
  {
    this.m_FuseWickObject.GetComponent<Renderer>().GetMaterial().SetFloat(this.m_FuseMatValName, val);
    this.m_FuseShadowObject.GetComponent<Renderer>().GetMaterial().SetFloat(this.m_FuseMatValName, val);
  }

  private void RestartCountdownAnims(float countdownRemainingSec)
  {
    this.StopCountdownAnims();
    this.StartCountdownAnims(this.UpdateCountdownAnims(countdownRemainingSec), countdownRemainingSec);
  }

  private void UpdateCountdownTimeout()
  {
    this.m_countdownTimeoutSec = 0.0f;
    if (GameState.Get() == null)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (currentPlayer == null || !currentPlayer.HasTag(GAME_TAG.TIMEOUT))
      return;
    this.m_countdownTimeoutSec = (float) currentPlayer.GetTag(GAME_TAG.TIMEOUT);
  }

  private float ComputeCountdownRemainingSec()
  {
    float num = this.m_countdownEndTimestamp - Time.realtimeSinceStartup;
    return (double) num < 0.0 ? 0.0f : num;
  }

  private float ComputeCountdownProgress(float countdownRemainingSec) => (double) countdownRemainingSec <= (double) Mathf.Epsilon ? 0.0f : countdownRemainingSec / this.m_countdownTimeoutSec;

  private void OnCurrentPlayerChanged(Player player, object userData)
  {
    if (this.m_state == TurnTimerState.COUNTDOWN || this.m_state == TurnTimerState.START)
      this.ChangeState(TurnTimerState.KILL);
    this.UpdateCountdownTimeout();
  }

  private void OnFriendlyTurnStarted(object userData)
  {
    if (!this.HasCountdownTimeout() && !this.m_waitingForTurnStartManagerFinish)
      return;
    if (this.m_waitingForTurnStartManagerFinish)
      this.ChangeState(TurnTimerState.START);
    this.m_waitingForTurnStartManagerFinish = false;
  }

  private void OnTurnTimerUpdate(TurnTimerUpdate update, object userData)
  {
    this.m_countdownEndTimestamp = update.GetEndTimestamp();
    if (!update.ShouldShow())
    {
      if (this.m_state != TurnTimerState.COUNTDOWN && this.m_state != TurnTimerState.START)
        return;
      this.ChangeState(TurnTimerState.KILL);
    }
    else
    {
      float secondsRemaining = update.GetSecondsRemaining();
      if ((double) secondsRemaining <= (double) Mathf.Epsilon)
        this.OnTurnTimedOut();
      else if ((double) secondsRemaining > (double) this.m_RopeCapSeconds)
        this.StartCountdownAnimsWhenBelowCap(secondsRemaining);
      else
        this.HandleTurnTimerUpdateAnims(secondsRemaining);
    }
  }

  private void HandleTurnTimerUpdateAnims(float secondsRemaining)
  {
    if (GameState.Get() != null && GameState.Get().IsGameOverNowOrPending())
      return;
    if (this.m_state == TurnTimerState.COUNTDOWN)
    {
      this.RestartCountdownAnims(secondsRemaining);
    }
    else
    {
      if ((double) this.ComputeCountdownRemainingSec() == 0.0)
        return;
      if (GameState.Get().IsTurnStartManagerActive())
        this.m_waitingForTurnStartManagerFinish = true;
      else
        this.StartCoroutine(this.EnterStartStateWhenReady());
    }
  }

  private IEnumerator EnterStartStateWhenReady()
  {
    while (GameState.Get() == null || GameState.Get().GetCurrentPlayer() == null)
      yield return (object) null;
    this.ChangeState(TurnTimerState.START);
  }

  private void OnTurnTimedOut()
  {
    if (!this.HasCountdownTimeout())
      return;
    this.ChangeState(TurnTimerState.TIMEOUT);
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData)
  {
    if (this.m_state != TurnTimerState.COUNTDOWN && this.m_state != TurnTimerState.START)
      return;
    this.ChangeState(TurnTimerState.KILL);
  }

  private void UpdateCountdownText()
  {
    if (GameState.Get() == null || GameState.Get().GetGameEntity() == null || GameState.Get().IsGameOver())
      return;
    float countdownRemainingSec = this.ComputeCountdownRemainingSec();
    this.m_CountdownText.Text = GameState.Get().GetGameEntity().GetTurnTimerCountdownText(countdownRemainingSec);
    this.m_CountdownText.TextColor = (double) countdownRemainingSec <= 0.0 || (double) countdownRemainingSec >= (double) this.m_RopeCapSeconds ? this.m_CountdownTextColorNormal : this.m_CountdownTextColorRope;
    if (!this.m_gameModeSettings.m_PlayTickSound)
      return;
    int num = Mathf.CeilToInt(countdownRemainingSec);
    if (this.m_lastTickSecondNumber <= num)
      return;
    this.m_lastTickSecondNumber = num;
    SoundManager.Get().Play(num == 0 ? this.m_FinalTickSound.GetComponent<AudioSource>() : this.m_TickSound.GetComponent<AudioSource>());
  }
}
