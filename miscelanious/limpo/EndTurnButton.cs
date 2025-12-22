using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
  public ActorStateMgr m_ActorStateMgr;
  public UberText m_MyTurnText;
  public UberText m_WaitingText;
  public GameObject m_GreenHighlight;
  public GameObject m_WhiteHighlight;
  public GameObject m_EndTurnButtonMesh;
  public List<Material> m_AlternativeMaterials;
  private static EndTurnButton s_instance;
  private bool m_inputBlockedInternally;
  private bool m_pressed;
  private bool m_playedNmpSoundThisTurn;
  private bool m_mousedOver;
  private bool m_disabled;
  private int m_inputBlockers;
  private List<EndTurnButton.ButtonUnblockedListener> m_buttonUnblockedListeners = new List<EndTurnButton.ButtonUnblockedListener>();

  public bool IsDisabled => this.m_disabled;

  private void Awake()
  {
    EndTurnButton.s_instance = this;
    this.m_MyTurnText.Text = this.GetEndTurnText();
    this.m_WaitingText.Text = "";
    this.GetComponent<Collider>().enabled = false;
  }

  private void OnDestroy() => EndTurnButton.s_instance = (EndTurnButton) null;

  private void Start() => this.StartCoroutine(this.WaitAFrameAndThenChangeState());

  public bool RegisterButtonUnblockedListener(EndTurnButton.OnButtonUnblocked callback)
  {
    EndTurnButton.ButtonUnblockedListener unblockedListener = new EndTurnButton.ButtonUnblockedListener();
    unblockedListener.SetCallback(callback);
    if (this.m_buttonUnblockedListeners.Contains(unblockedListener))
      return false;
    this.m_buttonUnblockedListeners.Add(unblockedListener);
    return true;
  }

  public bool UnregisterButtonUnblockedListener(EndTurnButton.OnButtonUnblocked callback)
  {
    EndTurnButton.ButtonUnblockedListener unblockedListener = new EndTurnButton.ButtonUnblockedListener();
    unblockedListener.SetCallback(callback);
    return this.m_buttonUnblockedListeners.Remove(unblockedListener);
  }

  private void FireButtonUnblockedEvent()
  {
    foreach (EndTurnButton.ButtonUnblockedListener unblockedListener in this.m_buttonUnblockedListeners.ToArray())
      unblockedListener.Fire();
  }

  public static EndTurnButton Get() => EndTurnButton.s_instance;

  public void Reset()
  {
    bool flag1 = this.HasExtraTurn();
    TurnStartManager.Get().NotifyOfExtraTurn(TurnStartManager.Get().GetExtraTurnSpell(), !flag1);
    bool flag2 = this.OpponentHasExtraTurn();
    TurnStartManager.Get().NotifyOfExtraTurn(TurnStartManager.Get().GetExtraTurnSpell(false), !flag2, false);
    this.UpdateState();
    GameState gameState = GameState.Get();
    Collider component = this.GetComponent<Collider>();
    if (gameState.IsPastBeginPhase() && gameState.IsLocalSidePlayerTurn())
      component.enabled = true;
    else
      component.enabled = false;
  }

  public GameObject GetButtonContainer() => this.transform.Find("ButtonContainer").gameObject;

  public void PlayPushDownAnimation()
  {
    if (this.InputBlockedInternally || this.IsInWaitingState() || this.m_pressed)
      return;
    this.m_pressed = true;
    this.GetButtonContainer().GetComponent<Animation>().Play("ENDTURN_PRESSED_DOWN");
    SoundManager.Get().LoadAndPlay((AssetReference) "FX_EndTurn_Down.prefab:7f967e178760e5d409cec10ad56cc3ff");
  }

  public void PlayButtonUpAnimation()
  {
    if (this.InputBlockedInternally || this.IsInWaitingState() || !this.m_pressed)
      return;
    this.m_pressed = false;
    this.GetButtonContainer().GetComponent<Animation>().Play("ENDTURN_PRESSED_UP");
    SoundManager.Get().LoadAndPlay((AssetReference) "FX_EndTurn_Up.prefab:aa092f360d27b5244b030e737d720ba6");
  }

  public bool IsInWaitingState()
  {
    switch (this.m_ActorStateMgr.GetActiveStateType())
    {
      case ActorStateType.ENDTURN_WAITING:
        return true;
      case ActorStateType.ENDTURN_NMP_2_WAITING:
        return true;
      case ActorStateType.ENDTURN_WAITING_TIMER:
        return true;
      default:
        return false;
    }
  }

  public bool IsInNMPState()
  {
    switch (this.m_ActorStateMgr.GetActiveStateType())
    {
      case ActorStateType.ENDTURN_NO_MORE_PLAYS:
        return true;
      case ActorStateType.EXTRATURN_NO_MORE_PLAYS:
        return true;
      default:
        return false;
    }
  }

  public bool IsInYouHavePlaysState()
  {
    switch (this.m_ActorStateMgr.GetActiveStateType())
    {
      case ActorStateType.ENDTURN_YOUR_TURN:
        return true;
      case ActorStateType.EXTRATURN_YOUR_TURN:
        return true;
      default:
        return false;
    }
  }

  public bool HasNoMorePlays()
  {
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    bool hasNoMorePlay;
    if (gameEntity != null && gameEntity.ShouldOverwriteEndTurnButtonNoMorePlaysState(out hasNoMorePlay))
      return hasNoMorePlay;
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    return optionsPacket != null && !optionsPacket.HasValidOption();
  }

  public bool IsInputBlocked() => this.InputBlockedInternally || this.m_inputBlockers > 0;

  public void AddInputBlocker() => ++this.m_inputBlockers;

  public void RemoveInputBlocker()
  {
    int num = this.IsInputBlocked() ? 1 : 0;
    --this.m_inputBlockers;
    bool flag = this.IsInputBlocked();
    if (num == 0 || flag)
      return;
    this.FireButtonUnblockedEvent();
  }

  private bool InputBlockedInternally
  {
    get => this.m_inputBlockedInternally;
    set
    {
      int num = this.IsInputBlocked() ? 1 : 0;
      this.m_inputBlockedInternally = value;
      bool flag = this.IsInputBlocked();
      if (num == 0 || flag)
        return;
      this.FireButtonUnblockedEvent();
    }
  }

  public void HandleMouseOver()
  {
    this.m_mousedOver = true;
    if (this.InputBlockedInternally)
      return;
    this.PutInMouseOverState();
  }

  public void HandleMouseOut()
  {
    this.m_mousedOver = false;
    if (this.InputBlockedInternally)
      return;
    if (this.m_pressed)
      this.PlayButtonUpAnimation();
    this.PutInMouseOffState();
  }

  private void PutInMouseOverState()
  {
    if (this.IsInNMPState())
    {
      this.m_WhiteHighlight.SetActive(false);
      this.m_GreenHighlight.SetActive(true);
      Hashtable args = iTween.Hash((object) "from", (object) RendererExtension.GetMaterial(this.m_GreenHighlight.GetComponent<Renderer>()).GetFloat("_Intensity"), (object) "to", (object) 1.4f, (object) "time", (object) 0.15f, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) "OnUpdateIntensityValue", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "ENDTURN_INTENSITY");
      iTween.StopByName(this.gameObject, "ENDTURN_INTENSITY");
      iTween.ValueTo(this.gameObject, args);
    }
    else if (this.IsInYouHavePlaysState())
    {
      this.m_WhiteHighlight.SetActive(true);
      this.m_GreenHighlight.SetActive(false);
    }
    else
    {
      this.m_WhiteHighlight.SetActive(false);
      this.m_GreenHighlight.SetActive(false);
    }
  }

  private void PutInMouseOffState()
  {
    this.m_WhiteHighlight.SetActive(false);
    if (this.IsInNMPState())
    {
      this.m_GreenHighlight.SetActive(true);
      Hashtable args = iTween.Hash((object) "from", (object) RendererExtension.GetMaterial(this.m_GreenHighlight.GetComponent<Renderer>()).GetFloat("_Intensity"), (object) "to", (object) 1.1f, (object) "time", (object) 0.15f, (object) "easetype", (object) iTween.EaseType.linear, (object) "onupdate", (object) "OnUpdateIntensityValue", (object) "onupdatetarget", (object) this.gameObject, (object) "name", (object) "ENDTURN_INTENSITY");
      iTween.StopByName(this.gameObject, "ENDTURN_INTENSITY");
      iTween.ValueTo(this.gameObject, args);
    }
    else
      this.m_GreenHighlight.SetActive(false);
  }

  private void OnUpdateIntensityValue(float newValue) => RendererExtension.GetMaterial(this.m_GreenHighlight.GetComponent<Renderer>()).SetFloat("_Intensity", newValue);

  private IEnumerator WaitAFrameAndThenChangeState()
  {
    EndTurnButton endTurnButton = this;
    yield return (object) null;
    if (GameState.Get() == null)
      Log.Gameplay.PrintError("EndTurnButton.WaitAFrameAndThenChangeState(): Game state does not exist.");
    else if (GameState.Get().IsGameCreated())
    {
      endTurnButton.HandleGameStart();
    }
    else
    {
      endTurnButton.m_ActorStateMgr.ChangeState(ActorStateType.ENDTURN_WAITING);
      GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(endTurnButton.OnCreateGame));
    }
  }

  private void HandleGameStart()
  {
    this.UpdateState();
    this.ApplyAlternativeAppearance();
    GameState gameState = GameState.Get();
    if (!gameState.IsPastBeginPhase() || !gameState.IsLocalSidePlayerTurn())
      return;
    this.GetComponent<Collider>().enabled = true;
    GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
  }

  private int GetCurrentAlternativeAppearanceIndex()
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return 0;
    GameEntity gameEntity = gameState.GetGameEntity();
    return gameEntity == null ? 0 : gameEntity.GetTag(GAME_TAG.END_TURN_BUTTON_ALTERNATIVE_APPEARANCE);
  }

  public void ApplyAlternativeAppearance()
  {
    int alternativeAppearanceIndex = this.GetCurrentAlternativeAppearanceIndex();
    if (alternativeAppearanceIndex == 1)
    {
      if (this.m_AlternativeMaterials.Count >= alternativeAppearanceIndex && (Object) this.m_AlternativeMaterials[alternativeAppearanceIndex - 1] != (Object) null)
        RendererExtension.SetMaterial(this.m_EndTurnButtonMesh.GetComponent<Renderer>(), this.m_AlternativeMaterials[alternativeAppearanceIndex - 1]);
      else
        Log.Gameplay.PrintError("EndTurnButton.ApplyAlternativeAppearance(): No material exists for appearance  {0}.", (object) alternativeAppearanceIndex);
    }
  }

  private void SetButtonState(ActorStateType stateType)
  {
    if ((Object) this.m_ActorStateMgr == (Object) null)
    {
      Debug.Log((object) "End Turn Button Actor State Manager is missing!");
    }
    else
    {
      if (this.m_ActorStateMgr.GetActiveStateType() == stateType || this.IsInputBlocked() && stateType != ActorStateType.ENDTURN_NO_MORE_PLAYS || this.m_disabled && stateType != ActorStateType.ENDTURN_WAITING)
        return;
      this.m_ActorStateMgr.ChangeState(stateType);
      if (stateType != ActorStateType.ENDTURN_YOUR_TURN && stateType != ActorStateType.ENDTURN_WAITING_TIMER)
        return;
      this.InputBlockedInternally = true;
      this.StartCoroutine(this.WaitUntilAnimationIsCompleteAndThenUnblockInput(stateType));
    }
  }

  private IEnumerator WaitUntilAnimationIsCompleteAndThenUnblockInput(
    ActorStateType stateType)
  {
    yield return (object) new WaitForSeconds(this.m_ActorStateMgr.GetMaximumAnimationTimeOfActiveStates());
    this.InputBlockedInternally = false;
    if (stateType == ActorStateType.ENDTURN_YOUR_TURN)
    {
      this.m_EndTurnButtonMesh.transform.localEulerAngles = Vector3.zero;
      if (this.HasNoMorePlays())
        this.SetStateToNoMorePlays();
    }
  }

  private void UpdateState()
  {
    if (GameState.Get().IsMulliganManagerActive() || GameState.Get().IsTurnStartManagerBlockingInput())
      return;
    if (!GameState.Get().IsLocalSidePlayerTurn() || !GameState.Get().GetGameEntity().IsCurrentTurnRealTime())
    {
      this.UpdateButtonText();
      this.SetStateToWaiting();
    }
    else
    {
      if (GameState.Get().GetResponseMode() == GameState.ResponseMode.NONE)
        return;
      this.SetStateToYourTurn();
    }
  }

  public void DisplayExtraTurnState() => this.UpdateState();

  private bool HasExtraTurn() => GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.NUM_TURNS_LEFT) > 1;

  private bool OpponentHasExtraTurn() => GameState.Get().GetOpposingSidePlayer().GetTag(GAME_TAG.NUM_TURNS_LEFT) > 1;

  private ActorStateType GetAppropriateYourTurnState()
  {
    if (!this.HasExtraTurn())
      return ActorStateType.ENDTURN_YOUR_TURN;
    return this.IsInWaitingState() ? ActorStateType.WAITING_TO_EXTRATURN : ActorStateType.EXTRATURN_YOUR_TURN;
  }

  private ActorStateType GetAppropriateYourTurnNMPState() => this.HasExtraTurn() ? ActorStateType.EXTRATURN_NO_MORE_PLAYS : ActorStateType.ENDTURN_NO_MORE_PLAYS;

  private string GetEndTurnText()
  {
    switch (this.GetCurrentAlternativeAppearanceIndex())
    {
      case 1:
      case 3:
        return "";
      case 2:
        return GameStrings.Get("GAMEPLAY_DONE_TURN");
      default:
        return GameStrings.Get("GAMEPLAY_END_TURN");
    }
  }

  private string GetEnemyTurnText()
  {
    string waitingText;
    if (GameState.Get().GetGameEntity().GetAlternativeEndTurnButtonText(out string _, out waitingText))
      return waitingText;
    switch (this.GetCurrentAlternativeAppearanceIndex())
    {
      case 1:
      case 2:
      case 3:
        return "";
      default:
        return GameStrings.Get("GAMEPLAY_ENEMY_TURN");
    }
  }

  public void UpdateButtonText()
  {
    string myTurnText;
    string waitingText;
    if (GameState.Get().GetGameEntity().GetAlternativeEndTurnButtonText(out myTurnText, out waitingText))
    {
      this.m_MyTurnText.SetText(GameStrings.Get(myTurnText));
      this.m_WaitingText.SetText(GameStrings.Get(waitingText));
    }
    else
    {
      switch (this.GetCurrentAlternativeAppearanceIndex())
      {
        case 1:
          this.m_MyTurnText.SetText(GameStrings.Get(""));
          this.m_WaitingText.SetText(GameStrings.Get(""));
          break;
        case 2:
          this.m_MyTurnText.SetText(GameStrings.Get("GAMEPLAY_DONE_TURN"));
          this.m_WaitingText.SetText(GameStrings.Get(""));
          break;
        case 3:
          this.m_MyTurnText.SetText(GameStrings.Get(""));
          this.m_WaitingText.SetText(GameStrings.Get(""));
          break;
        default:
          if (this.HasExtraTurn())
          {
            this.m_MyTurnText.SetText(GameStrings.Get("GAMEPLAY_NEXT_TURN"));
            this.m_WaitingText.SetText(GameStrings.Get("GAMEPLAY_NEXT_TURN"));
            break;
          }
          this.m_MyTurnText.SetText(GameStrings.Get("GAMEPLAY_END_TURN"));
          this.m_WaitingText.SetText(GameStrings.Get("GAMEPLAY_ENEMY_TURN"));
          break;
      }
    }
    this.m_MyTurnText.UpdateText();
    this.m_WaitingText.UpdateText();
  }

  private void SetStateToYourTurn()
  {
    this.UpdateButtonText();
    if ((Object) this.m_ActorStateMgr == (Object) null)
      return;
    if (this.HasNoMorePlays())
    {
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.FLIP_END_TURN_BUTTON_WHEN_ENTERING_NO_MORE_PLAY) && !this.IsInNMPState())
        this.SetStateToWaiting();
      this.SetStateToNoMorePlays();
    }
    else
    {
      this.SetButtonState(this.GetAppropriateYourTurnState());
      if (this.m_mousedOver)
        this.PutInMouseOverState();
      else
        this.PutInMouseOffState();
    }
  }

  private void SetStateToNoMorePlays()
  {
    if ((Object) this.m_ActorStateMgr == (Object) null)
      return;
    if (this.IsInWaitingState())
    {
      this.SetButtonState(this.GetAppropriateYourTurnState());
    }
    else
    {
      this.SetButtonState(this.GetAppropriateYourTurnNMPState());
      if (this.m_mousedOver)
        this.PutInMouseOverState();
      else
        this.PutInMouseOffState();
    }
    if (this.m_playedNmpSoundThisTurn || GameState.Get().GetGameEntity().HasTag(GAME_TAG.SUPPRESS_JOBS_DONE_VO))
      return;
    this.m_playedNmpSoundThisTurn = true;
    this.StartCoroutine(this.PlayEndTurnSound());
  }

  private void SetStateToWaiting()
  {
    if ((Object) this.m_ActorStateMgr == (Object) null || this.IsInWaitingState() || GameState.Get().IsGameOver())
      return;
    if (this.IsInNMPState())
      this.SetButtonState(ActorStateType.ENDTURN_NMP_2_WAITING);
    else
      this.SetButtonState(ActorStateType.ENDTURN_WAITING);
    this.PutInMouseOffState();
  }

  private IEnumerator PlayEndTurnSound()
  {
    EndTurnButton endTurnButton = this;
    yield return (object) new WaitForSeconds(1.5f);
    if (endTurnButton.IsInNMPState())
      SoundManager.Get().LoadAndPlay((AssetReference) "VO_JobsDone.prefab:88cda3fac32785c4d8101966b7604cc3", endTurnButton.gameObject);
  }

  private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
  {
    if (phase != GameState.CreateGamePhase.CREATED)
      return;
    GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
    this.HandleGameStart();
  }

  public void OnMulliganEnded() => this.m_WaitingText.Text = this.GetEnemyTurnText();

  public void OnTurnStartManagerFinished()
  {
    if (!GameState.Get().GetGameEntity().IsCurrentTurnRealTime())
      return;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
    this.m_playedNmpSoundThisTurn = false;
    this.SetStateToYourTurn();
    this.GetComponent<Collider>().enabled = true;
    GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
  }

  public void OnTurnChanged() => this.UpdateState();

  public void OnEndTurnRequested()
  {
    PegCursor.Get().SetMode(PegCursor.Mode.WAITING);
    this.SetStateToWaiting();
    this.GetComponent<Collider>().enabled = false;
    GameState.Get().UnregisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
  }

  private void OnOptionsReceived(object userData) => this.UpdateState();

  public void OnTurnTimerStart()
  {
    if (this.InputBlockedInternally)
      return;
    int num = this.m_mousedOver ? 1 : 0;
  }

  public void OnTurnTimerEnded(bool isFriendlyPlayerTurnTimer)
  {
    if (!isFriendlyPlayerTurnTimer)
      return;
    this.SetButtonState(ActorStateType.ENDTURN_WAITING_TIMER);
  }

  public void SetDisabled(bool disabled)
  {
    this.m_disabled = disabled;
    if (!this.m_disabled)
      return;
    this.SetButtonState(ActorStateType.ENDTURN_WAITING);
  }

  public delegate void OnButtonUnblocked(object userData);

  private class ButtonUnblockedListener : EventListener<EndTurnButton.OnButtonUnblocked>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
