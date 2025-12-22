using Blizzard.T5.Core.Utils;
using HutongGames.PlayMaker;
using PegasusShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingPopupDisplay : TransitionPopup
{
  public UberText m_tipOfTheDay;
  public GameObject m_nameContainer;
  public GameObject m_wildVines;
  public GameObject m_classicPewter;
  private List<GameObject> m_spinnerTexts = new List<GameObject>();
  private SceneMgr.Mode m_gameMode;
  private const int NUM_SPINNER_ENTRIES = 10;

  protected override void Awake()
  {
    base.Awake();
    this.m_nameContainer.SetActive(false);
    this.m_title.gameObject.SetActive(false);
    this.m_tipOfTheDay.gameObject.SetActive(false);
    this.m_wildVines.SetActive(false);
    this.m_classicPewter.SetActive(false);
    SoundManager.Get().Load((AssetReference) "FindOpponent_mechanism_start.prefab:effa04f444ca08840b677d98fc8abf39");
  }

  public override void Hide()
  {
    if (!this.m_shown)
      return;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    base.Hide();
  }

  public override void Show()
  {
    this.SetupSpinnerText();
    this.UpdateTipOfTheDay();
    this.GenerateRandomSpinnerTexts(this.IsMultiOpponentGame());
    this.m_title.Text = this.GetTitleTextBasedOnScenario();
    base.Show();
  }

  protected override void OnGameConnecting(FindGameEventData eventData)
  {
    base.OnGameConnecting(eventData);
    this.IncreaseTooltipProgress();
  }

  protected override void OnGameEntered(FindGameEventData eventData) => this.EnableCancelButtonIfPossible();

  protected override void OnGameDelayed(FindGameEventData eventData) => this.EnableCancelButtonIfPossible();

  protected override void OnAnimateShowFinished()
  {
    base.OnAnimateShowFinished();
    this.EnableCancelButtonIfPossible();
  }

  private void SetupSpinnerText()
  {
    for (int index = 1; index <= 10; ++index)
      this.m_spinnerTexts.Add(GameObjectUtils.FindChild(this.gameObject, "NAME_" + (object) index).gameObject);
  }

  private void GenerateRandomSpinnerTexts(bool isPlural)
  {
    string str1 = isPlural ? "GLUE_SPINNER_PLURAL_" : "GLUE_SPINNER_";
    int num = 1;
    List<string> stringList = new List<string>();
    while (true)
    {
      string str2 = GameStrings.Get(str1 + (object) num);
      if (!(str2 == str1 + (object) num))
      {
        stringList.Add(str2);
        ++num;
      }
      else
        break;
    }
    GameObjectUtils.FindChild(this.gameObject, "NAME_PerfectOpponent").gameObject.GetComponent<UberText>().Text = this.GetWorthyOpponentTextBasedOnScenario();
    for (int index1 = 0; index1 < 10; ++index1)
    {
      int index2 = Mathf.FloorToInt(Random.value * (float) stringList.Count);
      this.m_spinnerTexts[index1].GetComponent<UberText>().Text = stringList[index2];
      stringList.RemoveAt(index2);
    }
  }

  private IEnumerator StopSpinnerDelay()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MatchingPopupDisplay matchingPopupDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      matchingPopupDisplay.Hide();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(3.5f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private bool OnNavigateBack()
  {
    if (!this.m_cancelButton.gameObject.activeSelf)
      return false;
    this.GetComponent<PlayMakerFSM>().SendEvent("Cancel");
    this.FireMatchCanceledEvent();
    if (FriendChallengeMgr.Get() != null)
      FriendChallengeMgr.Get().CancelChallenge();
    if (PartyManager.Get().IsInParty() && PartyManager.Get().IsPartyLeader())
      PartyManager.Get().CancelQueue();
    return true;
  }

  protected override void OnCancelButtonReleased(UIEvent e)
  {
    base.OnCancelButtonReleased(e);
    if (PartyManager.Get().IsInParty() && !PartyManager.Get().IsPartyLeader())
      PartyManager.Get().CancelQueue();
    else
      Navigation.GoBack();
  }

  private void UpdateTipOfTheDay()
  {
    this.m_gameMode = SceneMgr.Get().GetMode();
    if (this.m_gameMode == SceneMgr.Mode.TOURNAMENT)
      this.m_tipOfTheDay.Text = GameStrings.GetTip(TipCategory.PLAY, new int?(Options.Get().GetInt(Option.TIP_PLAY_PROGRESS, 0)));
    else if (this.m_gameMode == SceneMgr.Mode.DRAFT)
      this.m_tipOfTheDay.Text = GameStrings.GetTip(TipCategory.FORGE, new int?(Options.Get().GetInt(Option.TIP_FORGE_PROGRESS, 0)));
    else if (this.m_gameMode == SceneMgr.Mode.BACON)
      this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.BACON);
    else if (SceneMgr.Get().IsInLettuceMode())
      this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.LETTUCE);
    else if (this.m_gameMode == SceneMgr.Mode.TAVERN_BRAWL)
    {
      if (TavernBrawlManager.Get().IsCurrentSeasonSessionBased)
        this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.HEROICBRAWL);
      else
        this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.TAVERNBRAWL);
    }
    else if (this.m_gameMode == SceneMgr.Mode.PVP_DUNGEON_RUN)
      this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.DUELS);
    else
      this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.DEFAULT);
  }

  private void IncreaseTooltipProgress()
  {
    if (this.m_gameMode == SceneMgr.Mode.TOURNAMENT)
    {
      Options.Get().SetInt(Option.TIP_PLAY_PROGRESS, Options.Get().GetInt(Option.TIP_PLAY_PROGRESS, 0) + 1);
    }
    else
    {
      if (this.m_gameMode != SceneMgr.Mode.DRAFT)
        return;
      Options.Get().SetInt(Option.TIP_FORGE_PROGRESS, Options.Get().GetInt(Option.TIP_FORGE_PROGRESS, 0) + 1);
    }
  }

  protected override void ShowPopup()
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "FindOpponent_mechanism_start.prefab:effa04f444ca08840b677d98fc8abf39");
    base.ShowPopup();
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    FsmBool fsmBool = component.FsmVariables.FindFsmBool("PlaySpinningMusic");
    if (fsmBool != null)
      fsmBool.Value = this.m_gameMode != SceneMgr.Mode.TAVERN_BRAWL;
    component.SendEvent("Birth");
    RenderUtils.EnableRenderers(this.m_nameContainer, false);
    this.m_title.gameObject.SetActive(true);
    this.m_tipOfTheDay.gameObject.SetActive(true);
    bool flag1 = false;
    bool flag2 = false;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT)
    {
      switch (this.m_gameType)
      {
        case GameType.GT_RANKED:
          flag1 = this.m_formatType == FormatType.FT_WILD;
          flag2 = this.m_formatType == FormatType.FT_CLASSIC;
          break;
        case GameType.GT_CASUAL:
          if (this.m_deckId.HasValue)
          {
            CollectionManager collectionManager = CollectionManager.Get();
            if (collectionManager != null)
            {
              CollectionDeck deck = collectionManager.GetDeck(this.m_deckId.Value);
              if (deck != null)
              {
                flag1 = deck.FormatType == FormatType.FT_WILD;
                flag2 = deck.FormatType == FormatType.FT_CLASSIC;
                break;
              }
              break;
            }
            break;
          }
          break;
      }
    }
    this.m_wildVines.SetActive(flag1);
    this.m_classicPewter.SetActive(flag2);
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
  }

  protected override void OnGameplaySceneLoaded()
  {
    this.m_nameContainer.SetActive(true);
    this.GetComponent<PlayMakerFSM>().SendEvent("Death");
    this.StartCoroutine(this.StopSpinnerDelay());
    Navigation.Clear();
  }

  private string GetTitleTextBasedOnScenario() => !this.IsMultiOpponentGame() ? GameStrings.Get("GLUE_MATCHMAKER_FINDING_OPPONENT") : GameStrings.Get("GLUE_MATCHMAKER_FINDING_OPPONENTS");

  private string GetWorthyOpponentTextBasedOnScenario() => !this.IsMultiOpponentGame() ? GameStrings.Get("GLUE_MATCHMAKER_PERFECT_OPPONENT") : GameStrings.Get("GLUE_MATCHMAKER_PERFECT_OPPONENTS");

  private bool IsMultiOpponentGame()
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_scenarioId);
    return record != null && record.Players > 2;
  }
}
