using Blizzard.T5.Core;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class PracticePickerTrayDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "UI")]
  public UberText m_trayLabel;
  [CustomEditField(Sections = "UI")]
  public StandardPegButtonNew m_backButton;
  [CustomEditField(Sections = "UI")]
  public PlayButton m_playButton;
  [CustomEditField(Sections = "AI Button Settings")]
  public PracticeAIButton m_AIButtonPrefab;
  [CustomEditField(Sections = "AI Button Settings")]
  public GameObject m_AIButtonsContainer;
  [SerializeField]
  private float m_AIButtonHeight = 5f;
  [CustomEditField(Sections = "Animation Settings")]
  public float m_trayAnimationTime = 0.5f;
  [CustomEditField(Sections = "Animation Settings")]
  public iTween.EaseType m_trayInEaseType = iTween.EaseType.easeOutBounce;
  [CustomEditField(Sections = "Animation Settings")]
  public iTween.EaseType m_trayOutEaseType = iTween.EaseType.easeOutCubic;
  private static PracticePickerTrayDisplay s_instance;
  private List<ScenarioDbfRecord> m_sortedMissionRecords = new List<ScenarioDbfRecord>();
  private List<PracticeAIButton> m_practiceAIButtons = new List<PracticeAIButton>();
  private List<Achievement> m_lockedHeroes = new List<Achievement>();
  private PracticeAIButton m_selectedPracticeAIButton;
  private Map<string, DefLoader.DisposableFullDef> m_heroDefs = new Map<string, DefLoader.DisposableFullDef>();
  private int m_heroDefsToLoad;
  private List<PracticePickerTrayDisplay.TrayLoaded> m_TrayLoadedListeners = new List<PracticePickerTrayDisplay.TrayLoaded>();
  private bool m_buttonsCreated;
  private bool m_buttonsReady;
  private bool m_heroesLoaded;
  private bool m_shown;
  private const float PRACTICE_TRAY_MATERIAL_Y_OFFSET = -0.045f;

  [CustomEditField(Sections = "AI Button Settings")]
  public float AIButtonHeight
  {
    get => this.m_AIButtonHeight;
    set
    {
      this.m_AIButtonHeight = value;
      this.UpdateAIButtonPositions();
    }
  }

  private void Awake()
  {
    PracticePickerTrayDisplay.s_instance = this;
    this.InitMissionRecords();
    foreach (Component component in this.gameObject.GetComponents<Transform>())
      component.gameObject.SetActive(false);
    this.gameObject.SetActive(true);
    if ((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null)
    {
      this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonReleased));
    }
    this.m_trayLabel.Text = GameStrings.Get("GLUE_CHOOSE_OPPONENT");
    this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayGameButtonRelease));
    this.m_heroDefsToLoad = this.m_sortedMissionRecords.Count;
    foreach (DbfRecord sortedMissionRecord in this.m_sortedMissionRecords)
    {
      string missionHeroCardId = GameUtils.GetMissionHeroCardId(sortedMissionRecord.ID);
      DefLoader.Get().LoadFullDef(missionHeroCardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded));
    }
    SoundManager.Get().Load((AssetReference) "choose_opponent_panel_slide_on.prefab:66491d3d01ed663429ab80daf6a5e880");
    SoundManager.Get().Load((AssetReference) "choose_opponent_panel_slide_off.prefab:3139d09eb94899d41b9bf612649f47bf");
    this.InitButtons();
    this.StartCoroutine(this.NotifyWhenTrayLoaded());
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
  }

  private void OnDestroy()
  {
    GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.m_heroDefs.DisposeValuesAndClear<string, DefLoader.DisposableFullDef>();
    PracticePickerTrayDisplay.s_instance = (PracticePickerTrayDisplay) null;
  }

  private void Start()
  {
    this.m_playButton.SetText(GameStrings.Get("GLOBAL_PLAY"));
    this.m_playButton.SetOriginalLocalPosition();
    this.m_playButton.Disable();
  }

  public static PracticePickerTrayDisplay Get() => PracticePickerTrayDisplay.s_instance;

  public void Init()
  {
    int count = this.m_sortedMissionRecords.Count;
    for (int index = 0; index < count; ++index)
    {
      PracticeAIButton c = (PracticeAIButton) GameUtils.Instantiate((Component) this.m_AIButtonPrefab, this.m_AIButtonsContainer);
      LayerUtils.SetLayer((Component) c, this.m_AIButtonsContainer.gameObject.layer);
      this.m_practiceAIButtons.Add(c);
    }
    this.UpdateAIButtonPositions();
    foreach (PracticeAIButton practiceAiButton in this.m_practiceAIButtons)
    {
      practiceAiButton.SetOriginalLocalPosition();
      practiceAiButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.AIButtonPressed));
    }
    this.m_buttonsCreated = true;
    LoanerDeckDisplay loanerDeckDisplay = LoanerDeckDisplay.Get();
    if (!((UnityEngine.Object) loanerDeckDisplay != (UnityEngine.Object) null))
      return;
    loanerDeckDisplay.LoanerDeckInfoDataModel.CurrentSceneMode = "PRACTICE";
  }

  public void Show()
  {
    this.m_shown = true;
    iTween.Stop(this.gameObject);
    foreach (Component component in this.gameObject.GetComponents<Transform>())
      component.gameObject.SetActive(true);
    this.gameObject.SetActive(true);
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) PracticeDisplay.Get().GetPracticePickerShowPosition(), (object) "isLocal", (object) true, (object) "time", (object) this.m_trayAnimationTime, (object) "easetype", (object) this.m_trayInEaseType, (object) "delay", (object) (1f / 1000f)));
    SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_on.prefab:66491d3d01ed663429ab80daf6a5e880");
    if (!Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_TRAY, false) && UserAttentionManager.CanShowAttentionGrabber("PracticePickerTrayDisplay.Show:" + (object) Option.HAS_SEEN_PRACTICE_TRAY))
    {
      Options.Get().SetBool(Option.HAS_SEEN_PRACTICE_TRAY, true);
      this.StartCoroutine(this.DoPickHeroLines());
    }
    if ((UnityEngine.Object) this.m_selectedPracticeAIButton != (UnityEngine.Object) null)
      this.m_playButton.Enable();
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
  }

  private IEnumerator DoPickHeroLines()
  {
    Notification firstPart = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_PRACTICE_INST1_07"), "VO_INNKEEPER_UNLOCK_HEROES.prefab:9a11f2d877b018043a6c85883cdd1761");
    while ((UnityEngine.Object) firstPart.GetAudio() == (UnityEngine.Object) null)
      yield return (object) null;
    yield return (object) new WaitForSeconds(firstPart.GetAudio().clip.length);
    yield return (object) new WaitForSeconds(6f);
    if (!this.m_playButton.IsEnabled() && !GameMgr.Get().IsTransitionPopupShown())
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_PRACTICE_INST2_08"), "VO_INNKEEPER_PRACTICE_INST2_08.prefab:7f8a9981df8853d44b3cc423d4f44f52", 2f);
  }

  public void Hide()
  {
    this.m_shown = false;
    iTween.Stop(this.gameObject);
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) PracticeDisplay.Get().GetPracticePickerHidePosition(), (object) "isLocal", (object) true, (object) "time", (object) this.m_trayAnimationTime, (object) "easetype", (object) this.m_trayOutEaseType, (object) "oncomplete", (object) (Action<object>) (e => this.gameObject.SetActive(false)), (object) "delay", (object) (1f / 1000f)));
    SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_off.prefab:3139d09eb94899d41b9bf612649f47bf");
  }

  public void OnGameDenied() => this.UpdateAIButtons();

  public bool IsShown() => this.m_shown;

  public void AddTrayLoadedListener(PracticePickerTrayDisplay.TrayLoaded dlg) => this.m_TrayLoadedListeners.Add(dlg);

  public void RemoveTrayLoadedListener(PracticePickerTrayDisplay.TrayLoaded dlg) => this.m_TrayLoadedListeners.Remove(dlg);

  public bool IsLoaded() => this.m_buttonsReady;

  private void InitMissionRecords()
  {
    int practiceDbId = 2;
    int modeDbId = (int) AdventureConfig.Get().GetSelectedMode();
    this.m_sortedMissionRecords = GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => r.AdventureId == practiceDbId && r.ModeId == modeDbId));
    this.m_sortedMissionRecords.Sort(new Comparison<ScenarioDbfRecord>(GameUtils.MissionSortComparison));
  }

  private void InitButtons() => this.StartCoroutine(this.InitButtonsWhenReady());

  private IEnumerator InitButtonsWhenReady()
  {
    while (!this.m_buttonsCreated)
      yield return (object) null;
    while (!this.m_heroesLoaded)
      yield return (object) null;
    this.UpdateAIButtons();
    this.m_buttonsReady = true;
  }

  private void OnFullDefLoaded(string cardId, DefLoader.DisposableFullDef def, object userData)
  {
    this.m_heroDefs.SetOrReplaceDisposable<string, DefLoader.DisposableFullDef>(cardId, def);
    --this.m_heroDefsToLoad;
    if (this.m_heroDefsToLoad > 0)
      return;
    this.m_heroesLoaded = true;
  }

  private void SetSelectedButton(PracticeAIButton button)
  {
    if ((UnityEngine.Object) this.m_selectedPracticeAIButton != (UnityEngine.Object) null)
      this.m_selectedPracticeAIButton.Deselect();
    this.m_selectedPracticeAIButton = button;
  }

  private void DisableAIButtons()
  {
    for (int index = 0; index < this.m_practiceAIButtons.Count; ++index)
      this.m_practiceAIButtons[index].SetEnabled(false);
  }

  private void EnableAIButtons()
  {
    for (int index = 0; index < this.m_practiceAIButtons.Count; ++index)
      this.m_practiceAIButtons[index].SetEnabled(true);
  }

  private bool OnNavigateBack()
  {
    this.Hide();
    if ((UnityEngine.Object) DeckPickerTray.GetTray() != (UnityEngine.Object) null)
      DeckPickerTray.GetTray().ResetCurrentMode();
    return true;
  }

  private void BackButtonReleased(UIEvent e) => Navigation.GoBack();

  private void PlayGameButtonRelease(UIEvent e)
  {
    LayerUtils.SetLayer(PracticeDisplay.Get().gameObject, GameLayer.Default);
    CollectionDeck selectedCollectionDeck = DeckPickerTrayDisplay.Get().GetSelectedCollectionDeck();
    if (selectedCollectionDeck == null)
    {
      Debug.LogError((object) "Trying to play practice game with deck null deck!");
    }
    else
    {
      e.GetElement().SetEnabled(false);
      this.DisableAIButtons();
      if (AdventureConfig.Get().GetSelectedMode() == AdventureModeDbId.EXPERT && !Options.Get().GetBool(Option.HAS_PLAYED_EXPERT_AI, false))
        Options.Get().SetBool(Option.HAS_PLAYED_EXPERT_AI, true);
      if (selectedCollectionDeck.IsLoanerDeck && FreeDeckMgr.Get().Status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD)
      {
        int deckTemplateId = selectedCollectionDeck.DeckTemplateId;
        if (deckTemplateId <= 0)
          Debug.LogError((object) "Trying to play practice game with deck template ID 0!");
        else
          GameMgr.Get().FindGame(GameType.GT_VS_AI, FormatType.FT_WILD, this.m_selectedPracticeAIButton.GetMissionID(), deckTemplateId: deckTemplateId);
      }
      else
      {
        long selectedDeckId = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
        if (selectedDeckId <= 0L)
          Debug.LogError((object) "Trying to play practice game with deck ID 0!");
        else
          GameMgr.Get().FindGame(GameType.GT_VS_AI, FormatType.FT_WILD, this.m_selectedPracticeAIButton.GetMissionID(), deckId: selectedDeckId);
      }
    }
  }

  private void AIButtonPressed(UIEvent e)
  {
    PracticeAIButton element = (PracticeAIButton) e.GetElement();
    this.SetSelectedButton(element);
    this.m_playButton.Enable();
    element.Select();
  }

  private void UpdateAIButtons()
  {
    this.UpdateAIDeckButtons();
    if ((UnityEngine.Object) this.m_selectedPracticeAIButton == (UnityEngine.Object) null)
      this.m_playButton.Disable();
    else
      this.m_playButton.Enable();
  }

  private void UpdateAIButtonPositions()
  {
    int num = 0;
    foreach (Component practiceAiButton in this.m_practiceAIButtons)
      TransformUtil.SetLocalPosZ(practiceAiButton, -this.m_AIButtonHeight * (float) num++);
  }

  private void UpdateAIDeckButtons()
  {
    for (int index = 0; index < this.m_sortedMissionRecords.Count; ++index)
    {
      ScenarioDbfRecord sortedMissionRecord = this.m_sortedMissionRecords[index];
      int id = sortedMissionRecord.ID;
      DefLoader.DisposableFullDef heroDef = this.m_heroDefs[GameUtils.GetMissionHeroCardId(id)];
      TAG_CLASS buttonClass = heroDef.EntityDef.GetClass();
      string shortName = (string) sortedMissionRecord.ShortName;
      PracticeAIButton practiceAiButton = this.m_practiceAIButtons[index];
      practiceAiButton.SetInfo(shortName, buttonClass, heroDef.DisposableCardDef, id, false);
      bool shown = false;
      foreach (Achievement lockedHero in this.m_lockedHeroes)
      {
        if (lockedHero.ClassReward.Value == buttonClass)
        {
          shown = true;
          break;
        }
      }
      practiceAiButton.ShowQuestBang(shown);
      if ((UnityEngine.Object) practiceAiButton == (UnityEngine.Object) this.m_selectedPracticeAIButton)
        practiceAiButton.Select();
      else
        practiceAiButton.Deselect();
    }
    int num = AdventureConfig.Get().GetSelectedMode() == AdventureModeDbId.EXPERT ? 1 : 0;
    bool flag = Options.Get().GetBool(Option.HAS_SEEN_EXPERT_AI, false);
    if (num == 0 || flag)
      return;
    Options.Get().SetBool(Option.HAS_SEEN_EXPERT_AI, true);
  }

  private IEnumerator NotifyWhenTrayLoaded()
  {
    while (!this.m_buttonsReady)
      yield return (object) null;
    this.FireTrayLoadedEvent();
  }

  private void FireTrayLoadedEvent()
  {
    foreach (PracticePickerTrayDisplay.TrayLoaded trayLoaded in this.m_TrayLoadedListeners.ToArray())
      trayLoaded();
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if (eventData.m_state == FindGameState.INVALID)
      this.EnableAIButtons();
    return false;
  }

  public delegate void TrayLoaded();
}
