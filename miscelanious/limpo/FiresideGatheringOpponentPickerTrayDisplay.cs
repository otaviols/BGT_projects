using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class FiresideGatheringOpponentPickerTrayDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "UI")]
  public UberText m_trayLabel;
  [CustomEditField(Sections = "UI")]
  public StandardPegButtonNew m_backButton;
  [CustomEditField(Sections = "UI")]
  public PlayButton m_playButton;
  [CustomEditField(Sections = "UI")]
  public FiresideGatheringPlayButtonLantern m_FiresideGatheringPlayButtonLantern;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_inputBlocker;
  [CustomEditField(Sections = "Opponent Button Settings")]
  public FiresideGatheringOpponentButton m_OpponentButtonPrefab;
  [CustomEditField(Sections = "Opponent Button Settings")]
  public UIBScrollableItem m_HeaderPrefab;
  [CustomEditField(Sections = "Opponent Button Settings")]
  public UIBScrollableItem m_FooterPrefab;
  [CustomEditField(Sections = "Opponent Button Settings")]
  public GameObject m_OpponentButtonsContainer;
  [CustomEditField(Sections = "Opponent Button Settings")]
  public float m_HeaderHeight = 6f;
  [SerializeField]
  private float m_opponentButtonHeight = 5f;
  [CustomEditField(Sections = "Animation Settings")]
  public float m_trayInAnimationTime = 0.5f;
  [CustomEditField(Sections = "Animation Settings")]
  public float m_trayOutAnimationTime;
  [CustomEditField(Sections = "Animation Settings")]
  public iTween.EaseType m_trayInEaseType = iTween.EaseType.easeOutBounce;
  [CustomEditField(Sections = "Animation Settings")]
  public iTween.EaseType m_trayOutEaseType = iTween.EaseType.easeOutCubic;
  private static FiresideGatheringOpponentPickerTrayDisplay s_instance;
  private const float m_fadeOutTime = 0.2f;
  private List<FiresideGatheringOpponentButton> m_opponentButtons = new List<FiresideGatheringOpponentButton>();
  private UIBScrollableItem m_footer;
  private List<Achievement> m_lockedHeroes;
  private FiresideGatheringOpponentButton m_selectedOpponentButton;
  private List<FiresideGatheringOpponentPickerTrayDisplay.TrayLoaded> m_TrayLoadedListeners = new List<FiresideGatheringOpponentPickerTrayDisplay.TrayLoaded>();
  private List<Action> m_trayHiddenListeners = new List<Action>();
  private bool m_buttonsReady;
  private bool m_shown;
  private ScreenEffectsHandle m_screenEffectsHandle;

  [CustomEditField(Sections = "Opponent Button Settings")]
  public float OpponentButtonHeight
  {
    get => this.m_opponentButtonHeight;
    set
    {
      this.m_opponentButtonHeight = value;
      this.UpdateOpponentButtonPositions();
    }
  }

  private void Awake()
  {
    FiresideGatheringOpponentPickerTrayDisplay.s_instance = this;
    foreach (Component component in this.gameObject.GetComponents<Transform>())
      component.gameObject.SetActive(false);
    this.gameObject.SetActive(true);
    if ((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null)
    {
      this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonReleased));
    }
    this.m_trayLabel.Text = GameStrings.Get("GLUE_FIRESIDE_GATHERING_CHOOSE_OPPONENT");
    SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.HideTrayWhenLeavingScreen));
    this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayGameButtonRelease));
    this.m_FiresideGatheringPlayButtonLantern.gameObject.SetActive(false);
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonReleased));
    SoundManager.Get().Load((AssetReference) "choose_opponent_panel_slide_on.prefab:66491d3d01ed663429ab80daf6a5e880");
    SoundManager.Get().Load((AssetReference) "choose_opponent_panel_slide_off.prefab:3139d09eb94899d41b9bf612649f47bf");
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.StartCoroutine(this.NotifyWhenTrayLoaded());
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy()
  {
    FiresideGatheringOpponentPickerTrayDisplay.s_instance = (FiresideGatheringOpponentPickerTrayDisplay) null;
    FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonReleased));
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.HideTrayWhenLeavingScreen));
    if (GameMgr.Get() == null)
      return;
    GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
  }

  private void Start()
  {
    this.m_playButton.SetOriginalLocalPosition();
    this.SetPlayButtonEnabled(false);
  }

  public static FiresideGatheringOpponentPickerTrayDisplay Get() => FiresideGatheringOpponentPickerTrayDisplay.s_instance;

  public void Init()
  {
    this.m_footer = (UIBScrollableItem) GameUtils.Instantiate((Component) this.m_FooterPrefab, this.m_OpponentButtonsContainer);
    LayerUtils.SetLayer((Component) this.m_footer, this.m_OpponentButtonsContainer.gameObject.layer);
  }

  public void Show()
  {
    this.m_shown = true;
    iTween.Stop(this.gameObject);
    this.gameObject.SetActive(true);
    if (CollectionManager.Get().IsInEditMode())
      CollectionManager.Get().DoneEditing();
    this.UpdateOpponentButtons();
    foreach (Component component in this.gameObject.GetComponents<Transform>())
      component.gameObject.SetActive(true);
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) FiresideGatheringDisplay.Get().GetOpponentPickerShowPosition(), (object) "isLocal", (object) true, (object) "time", (object) this.m_trayInAnimationTime, (object) "easetype", (object) this.m_trayInEaseType, (object) "delay", (object) (1f / 1000f)));
    SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_on.prefab:66491d3d01ed663429ab80daf6a5e880");
    if ((UnityEngine.Object) this.m_selectedOpponentButton != (UnityEngine.Object) null)
      this.SetPlayButtonEnabled(true);
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.FadeMobileBlurEffectsIn();
    this.m_inputBlocker.gameObject.SetActive(true);
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
  }

  private void Hide()
  {
    this.m_shown = false;
    iTween.Stop(this.gameObject);
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) FiresideGatheringDisplay.Get().GetOpponentPickerHidePosition(), (object) "isLocal", (object) true, (object) "time", (object) this.m_trayOutAnimationTime, (object) "easetype", (object) this.m_trayOutEaseType, (object) "oncomplete", (object) (Action<object>) (e => this.gameObject.SetActive(false)), (object) "delay", (object) (1f / 1000f)));
    this.m_inputBlocker.gameObject.SetActive(false);
    this.FireTrayHiddenListeners();
    this.m_trayHiddenListeners.Clear();
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.FadeMobileBlurEffectsOut();
    FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_off.prefab:3139d09eb94899d41b9bf612649f47bf");
  }

  public void OnGameDenied() => this.UpdateOpponentButtons();

  public bool IsShown() => this.m_shown;

  public void AddTrayLoadedListener(
    FiresideGatheringOpponentPickerTrayDisplay.TrayLoaded dlg)
  {
    this.m_TrayLoadedListeners.Add(dlg);
  }

  public void RemoveTrayLoadedListener(
    FiresideGatheringOpponentPickerTrayDisplay.TrayLoaded dlg)
  {
    this.m_TrayLoadedListeners.Remove(dlg);
  }

  public bool IsLoaded() => this.m_buttonsReady;

  public void RegisterTrayHiddenListener(Action listener)
  {
    if (this.m_trayHiddenListeners.Contains(listener))
      return;
    this.m_trayHiddenListeners.Add(listener);
  }

  public void UnregisterTrayHiddenListener(Action listener)
  {
    if (!this.m_trayHiddenListeners.Contains(listener))
      return;
    this.m_trayHiddenListeners.Remove(listener);
  }

  public void FireTrayHiddenListeners()
  {
    foreach (Action trayHiddenListener in this.m_trayHiddenListeners)
      trayHiddenListener();
  }

  private void SetSelectedButton(FiresideGatheringOpponentButton button)
  {
    if ((UnityEngine.Object) this.m_selectedOpponentButton != (UnityEngine.Object) null)
      this.m_selectedOpponentButton.Deselect();
    this.m_selectedOpponentButton = button;
  }

  private void DisableOpponentButtons()
  {
    for (int index = 0; index < this.m_opponentButtons.Count; ++index)
      this.m_opponentButtons[index].SetEnabled(false);
  }

  private void EnableOpponentButtons()
  {
    for (int index = 0; index < this.m_opponentButtons.Count; ++index)
      this.m_opponentButtons[index].SetEnabled(true);
  }

  private bool OnNavigateBack()
  {
    this.Hide();
    if ((UnityEngine.Object) DeckPickerTray.GetTray() != (UnityEngine.Object) null)
      DeckPickerTray.GetTray().ResetCurrentMode();
    return true;
  }

  private void BackButtonReleased(UIEvent e)
  {
    FriendChallengeMgr.Get().ClearSelectedDeckAndHeroBeforeSendingChallenge();
    Navigation.GoBack();
  }

  private void PlayGameButtonRelease(UIEvent e)
  {
    e.GetElement().SetEnabled(false);
    this.DisableOpponentButtons();
    FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
    BnetPlayer associatedBnetPlayer = this.m_selectedOpponentButton.AssociatedBnetPlayer;
    TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
    if ((!associatedBnetPlayer.GetHearthstoneGameAccount().CanBeInvitedToGame() || !associatedBnetPlayer.IsOnline() ? 0 : (FiresideGatheringManager.Get().OpponentHasValidDeckForSelectedPlaymode(associatedBnetPlayer) ? 1 : 0)) == 0)
    {
      Navigation.GoBack();
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLOBAL_FIRESIDE_GATHERING"),
        m_text = associatedBnetPlayer.IsOnline() ? GameStrings.Get("GLUE_FIRESIDE_GATHERING_OPPONENT_UNAVAILABLE") : GameStrings.Get("GLUE_FIRESIDE_GATHERING_OPPONENT_OFFLINE"),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseCallback = new AlertPopup.ResponseCallback(this.OnOpponentUnavailableResponse)
      });
    }
    else
    {
      FriendChallengeMgr.Get().SetChallengeMethod(FriendChallengeMgr.ChallengeMethod.FROM_FIRESIDE_GATHERING_OPPONENT_PICKER);
      if (FriendChallengeMgr.Get().HasChallenge() && FriendChallengeMgr.Get().GetMyOpponent() != associatedBnetPlayer)
        FriendChallengeMgr.Get().CancelChallenge();
      switch (FiresideGatheringManager.Get().CurrentFiresideGatheringMode)
      {
        case FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE:
          FormatType formatType = Options.GetFormatType();
          FriendChallengeMgr.Get().SendChallenge(associatedBnetPlayer, formatType, false);
          break;
        case FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE_BRAWL:
          FriendChallengeMgr.Get().SendTavernBrawlChallenge(associatedBnetPlayer, BrawlType.BRAWL_TYPE_TAVERN_BRAWL, tavernBrawlMission.seasonId, tavernBrawlMission.SelectedBrawlLibraryItemId);
          break;
        case FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL:
          FriendChallengeMgr.Get().SendTavernBrawlChallenge(associatedBnetPlayer, BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING, tavernBrawlMission.seasonId, tavernBrawlMission.SelectedBrawlLibraryItemId);
          break;
      }
    }
  }

  private void OnOpponentUnavailableResponse(AlertPopup.Response response, object userData)
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this != (UnityEngine.Object) FiresideGatheringOpponentPickerTrayDisplay.s_instance)
      return;
    this.Show();
  }

  private void OpponentButtonPressed(UIEvent e)
  {
    FiresideGatheringOpponentButton element = (FiresideGatheringOpponentButton) e.GetElement();
    this.SetSelectedButton(element);
    this.SetPlayButtonEnabled(true);
    element.Select();
  }

  private void UpdateOpponentButtons()
  {
    this.UpdateOpponentPlayerButtons();
    this.UpdateOpponentButtonPositions();
    if ((UnityEngine.Object) this.m_selectedOpponentButton == (UnityEngine.Object) null)
      this.SetPlayButtonEnabled(false);
    else
      this.SetPlayButtonEnabled(true);
  }

  private void UpdateOpponentButtonPositions()
  {
    for (int index = 0; index < this.m_opponentButtons.Count; ++index)
      TransformUtil.SetLocalPosZ((Component) this.m_opponentButtons[index], -this.m_opponentButtonHeight * (float) index);
    TransformUtil.SetLocalPosZ((Component) this.m_footer, -this.m_opponentButtonHeight * (float) this.m_opponentButtons.Count);
  }

  private void UpdateOpponentPlayerButtons()
  {
    List<BnetPlayer> source = new List<BnetPlayer>();
    if (FiresideGatheringManager.Get() != null)
    {
      foreach (BnetPlayer displayablePatron in FiresideGatheringManager.Get().DisplayablePatronList)
      {
        if (displayablePatron != null && !(displayablePatron.GetHearthstoneGameAccount() == (BnetGameAccount) null) && FiresideGatheringManager.Get().OpponentHasValidDeckForSelectedPlaymode(displayablePatron) && displayablePatron.GetHearthstoneGameAccount().CanBeInvitedToGame())
          source.Add(displayablePatron);
      }
      source.Sort(new Comparison<BnetPlayer>(FiresideGatheringManager.Get().FiresideGatheringPlayerSort));
    }
    int num = Mathf.Min(FiresideGatheringPresenceManager.MAX_SUBSCRIBED_PATRONS, source.Count<BnetPlayer>());
    if (this.m_opponentButtons.Count < num)
    {
      for (int count = this.m_opponentButtons.Count; count < num; ++count)
      {
        FiresideGatheringOpponentButton c = (FiresideGatheringOpponentButton) GameUtils.Instantiate((Component) this.m_OpponentButtonPrefab, this.m_OpponentButtonsContainer);
        LayerUtils.SetLayer((Component) c, this.m_OpponentButtonsContainer.gameObject.layer);
        this.m_opponentButtons.Add(c);
        c.SetOriginalLocalPosition();
        c.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OpponentButtonPressed));
      }
    }
    else if (this.m_opponentButtons.Count > num)
    {
      for (int count = this.m_opponentButtons.Count; count > num; --count)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_opponentButtons[count - 1].gameObject);
        this.m_opponentButtons.RemoveAt(count - 1);
      }
    }
    this.m_buttonsReady = true;
    bool flag = false;
    for (int index = 0; index < this.m_opponentButtons.Count; ++index)
    {
      if (index < source.Count)
      {
        BnetPlayer player = source[index];
        FiresideGatheringOpponentButton opponentButton = this.m_opponentButtons[index];
        opponentButton.SetIsFriend(BnetFriendMgr.Get().IsFriend(player));
        opponentButton.SetIsFiresideBrawl(FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL);
        opponentButton.SetName(player.GetBestName());
        opponentButton.AssociatedBnetPlayer = player;
        if ((UnityEngine.Object) this.m_selectedOpponentButton != (UnityEngine.Object) null && player == this.m_selectedOpponentButton.AssociatedBnetPlayer)
        {
          opponentButton.Select();
          flag = true;
        }
        else
          opponentButton.Deselect();
      }
      else
      {
        Debug.LogError((object) "Attempting to update more buttons than there are patrons and friends.");
        return;
      }
    }
    if (!flag)
      this.m_selectedOpponentButton = (FiresideGatheringOpponentButton) null;
    FiresideGatheringOpponentPickerFooter component = this.m_footer.GetComponent<FiresideGatheringOpponentPickerFooter>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.SetTextOnFooter(source.Count == 0);
  }

  private IEnumerator NotifyWhenTrayLoaded()
  {
    while (!this.m_buttonsReady)
      yield return (object) null;
    this.FireTrayLoadedEvent();
  }

  private void FireTrayLoadedEvent()
  {
    foreach (FiresideGatheringOpponentPickerTrayDisplay.TrayLoaded trayLoaded in this.m_TrayLoadedListeners.ToArray())
      trayLoaded();
  }

  private void SetPlayButtonEnabled(bool enabled)
  {
    if (enabled)
      this.m_playButton.Enable();
    else
      this.m_playButton.Disable();
    if (!((UnityEngine.Object) FiresideGatheringDisplay.Get() != (UnityEngine.Object) null))
      return;
    this.m_FiresideGatheringPlayButtonLantern.gameObject.SetActive(FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.FIRESIDE_BRAWL);
    this.m_FiresideGatheringPlayButtonLantern.SetLanternLit(enabled);
  }

  private void OnFriendChallengeChanged(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData)
  {
    if (challengeEvent != FriendChallengeEvent.I_RESCINDED_CHALLENGE && challengeEvent != FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE && challengeEvent != FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE && challengeEvent != FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS && challengeEvent != FriendChallengeEvent.QUEUE_CANCELED)
      return;
    this.EnableOpponentButtons();
    this.SetPlayButtonEnabled(true);
    if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null)
    {
      long deckId;
      long heroCardDbId;
      if (challengeData.DidSendChallenge)
      {
        deckId = challengeData.m_challengerDeckId;
        heroCardDbId = challengeData.m_challengerHeroId;
      }
      else
      {
        deckId = challengeData.m_challengeeDeckId;
        heroCardDbId = challengeData.m_challengeeHeroId;
      }
      if (deckId != 0L)
        FriendChallengeMgr.Get().SelectDeckBeforeSendingChallenge(deckId);
      if (heroCardDbId != 0L)
        FriendChallengeMgr.Get().SelectHeroBeforeSendingChallenge(heroCardDbId);
    }
    FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
  }

  private void FadeMobileBlurEffectsIn()
  {
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    LayerUtils.SetLayer(Box.Get().m_letterboxingContainer, GameLayer.IgnoreFullScreenEffects);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = 0.0f
    });
  }

  private void FadeMobileBlurEffectsOut() => this.m_screenEffectsHandle.StopEffect(0.2f, new Action(this.OnFadeMobileBlurEffectsOutFinished));

  private void OnFadeMobileBlurEffectsOutFinished()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null || (UnityEngine.Object) Box.Get() == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    LayerUtils.SetLayer(Box.Get().m_letterboxingContainer, GameLayer.Default);
  }

  private void HideTrayWhenLeavingScreen(
    SceneMgr.Mode prevMode,
    PegasusScene prevScene,
    object userData)
  {
    this.Hide();
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if (!this.IsShown())
      return false;
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        FriendChallengeMgr.Get().CancelChallenge();
        Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
        this.OnNavigateBack();
        break;
    }
    return false;
  }

  public delegate void TrayLoaded();
}
