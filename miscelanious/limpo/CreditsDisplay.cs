using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CreditsDisplay : MonoBehaviour
{
  public GameObject m_creditsRoot;
  public UberText m_creditsText1;
  public UberText m_creditsText2;
  private UberText m_currentText;
  public Transform m_offscreenCardBone;
  public Transform m_cardBone;
  public UIBButton m_doneButton;
  public UIBButton m_yearButton1;
  public UIBButton m_yearButton2;
  public UIBButton m_fasterButton;
  public UIBButton m_slowerButton;
  public Transform m_flopPoint;
  public GameObject m_doneArrowInButton;
  [SerializeField]
  private float m_firstCardDelay = 4f;
  [SerializeField]
  private float m_cardDelay = 5f;
  private const string CREDITS_TEXT_CARD_CUTOFF_LINE = "<!-- no more cards displayed after this line -->";
  private float m_creditsScrollSpeed = 3.5f;
  private const float CREDITS_SCROLL_SPEED_DEFAULT = 3.5f;
  private const float CREDITS_SCROLL_SPEED_STEP = 0.75f;
  private const int CREDITS_SCROLL_STEP_LIMIT = 3;
  private int m_creditsScrollSpeedCurrentStep;
  private const int MAX_LINES_PER_CHUNK = 70;
  private static CreditsDisplay s_instance;
  private string[] m_creditLines;
  private int m_currentLine;
  private List<Actor> m_fakeCards;
  private List<DefLoader.DisposableFullDef> m_creditsDefs = new List<DefLoader.DisposableFullDef>();
  private bool started;
  private bool m_creditsTextLoaded;
  private bool m_creditsTextLoadSucceeded;
  private bool m_creditsDone;
  private Actor m_shownCreditsCard;
  private Vector3 creditsRootStartLocalPosition;
  private Vector3 creditsText1StartLocalPosition;
  private Vector3 creditsText2StartLocalPosition;
  private int m_lastCard = 1;
  private List<string> m_cardsToLoad = new List<string>();
  private bool m_sortedCards;
  private Dictionary<string, string> m_creditsCardsByName;
  private Coroutine END_CREDITS_COROUTINE;
  private Coroutine START_CREDITS_COROUTINE;
  private Coroutine SHOW_NEW_CARD_COROUTINE;
  private AssetReference[] s_credits_card_embers;
  private AssetReference[] s_credits_card_enter;
  private AssetReference[] s_tavern_crowd_play_reaction_positive;
  private int m_creditsYearIndex = -1;
  private CreditsYearDbfRecord[] m_creditsYearsAvailable = new CreditsYearDbfRecord[0];

  public static CreditsDisplay Get() => CreditsDisplay.s_instance;

  private void Awake()
  {
    this.s_credits_card_embers = new AssetReference[3]
    {
      new AssetReference("credits_card_embers_1.prefab:4648803a81d87474796231e996fb0d13"),
      new AssetReference("credits_card_embers_2.prefab:4078df663ba798940b2421bcbc3158b4"),
      new AssetReference("credits_card_embers_3.prefab:bd7299f68ec58234e907a520a605c2ec")
    };
    this.s_credits_card_enter = new AssetReference[3]
    {
      new AssetReference("credits_card_enter_1.prefab:d7f2bfe2038cc5b4db0d62d0583b00d5"),
      new AssetReference("credits_card_enter_2.prefab:e13890ae6bc727c438226f1f6097b7ee"),
      new AssetReference("credits_card_enter_3.prefab:5f352f8760ce4a346b9e6800cc1e8aac")
    };
    this.s_tavern_crowd_play_reaction_positive = new AssetReference[5]
    {
      new AssetReference("tavern_crowd_play_reaction_positive_1.prefab:83877aea3ad648a48929d10bd1c2241b"),
      new AssetReference("tavern_crowd_play_reaction_positive_2.prefab:f034e34549f86b44683db038fc04cb68"),
      new AssetReference("tavern_crowd_play_reaction_positive_3.prefab:d62c8a96c4fb6f14990d0f1dc089e50a"),
      new AssetReference("tavern_crowd_play_reaction_positive_4.prefab:ed271df67f20c6847833c88cee921c53"),
      new AssetReference("tavern_crowd_play_reaction_positive_5.prefab:cb3d351beea04f54fafc21eb44618108")
    };
    CreditsDisplay.s_instance = this;
    this.m_fakeCards = new List<Actor>();
    this.creditsRootStartLocalPosition = this.m_creditsRoot.transform.localPosition;
    this.creditsText1StartLocalPosition = this.m_creditsText1.transform.localPosition;
    this.creditsText2StartLocalPosition = this.m_creditsText2.transform.localPosition;
    this.m_doneButton.SetText(GameStrings.Get("GLOBAL_BACK"));
    this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDonePressed));
    this.m_yearButton1.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnYearPressed1));
    this.m_yearButton2.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnYearPressed2));
    this.UpdateYearButtons();
    this.m_fasterButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFasterButtonPressed));
    this.m_slowerButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSlowerButtonPressed));
    this.m_fasterButton.SetText("Faster");
    this.m_fasterButton.gameObject.SetActive(false);
    this.m_slowerButton.SetText("Slower");
    this.m_slowerButton.gameObject.SetActive(false);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      Box.Get().m_tableTop.SetActive(false);
      Box.Get().m_letterboxingContainer.SetActive(false);
      this.m_doneButton.SetText("");
      this.m_doneArrowInButton.SetActive(true);
    }
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Ally.prefab:d00eb0f79080e0749993fe4619e9143d", new PrefabCallback<GameObject>(this.ActorLoadedCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hand_Ally.prefab:d00eb0f79080e0749993fe4619e9143d", new PrefabCallback<GameObject>(this.ActorLoadedCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    this.m_creditsYearsAvailable = GameDbf.CreditsYear.GetRecords().OrderBy<CreditsYearDbfRecord, int>((Func<CreditsYearDbfRecord, int>) (r => r.ID)).ToArray<CreditsYearDbfRecord>();
    this.m_creditsYearIndex = this.m_creditsYearsAvailable.Length - 1;
    this.UpdateYearButtons();
    this.PopulateCreditsCardsByName();
    this.LoadCreditsText();
  }

  private void OnDestroy()
  {
    CreditsDisplay.s_instance = (CreditsDisplay) null;
    this.ReleaseAllCreditsCards();
  }

  private void StopAndClearCoroutine(ref Coroutine co)
  {
    if (co == null)
      return;
    this.StopCoroutine(co);
    co = (Coroutine) null;
  }

  private void PopulateCreditsCardsByName()
  {
    this.m_creditsCardsByName = new Dictionary<string, string>();
    foreach (CardDbfRecord record in GameDbf.Card.GetRecords())
    {
      if (record.CardSetTimings.Any<CardSetTimingDbfRecord>((Func<CardSetTimingDbfRecord, bool>) (cardSetTiming => cardSetTiming.CardSetId == 16)))
      {
        if (record.Name != null)
          this.m_creditsCardsByName[record.Name.GetString().Trim()] = record.NoteMiniGuid;
        if (!string.IsNullOrEmpty(record.CreditsCardName))
          this.m_creditsCardsByName[record.CreditsCardName.Trim()] = record.NoteMiniGuid;
      }
    }
  }

  private void LoadAllCreditsCards()
  {
    this.ReleaseAllCreditsCards();
    foreach (string cardId in this.m_cardsToLoad)
      DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnFullDefLoaded));
  }

  private void ReleaseAllCreditsCards() => this.m_creditsDefs.DisposeValuesAndClear<DefLoader.DisposableFullDef>();

  private void LoadCreditsText()
  {
    this.m_creditsTextLoadSucceeded = false;
    this.m_cardsToLoad.Clear();
    string creditsFilename = (string) null;
    if (this.m_creditsYearIndex >= 0 && this.m_creditsYearIndex < this.m_creditsYearsAvailable.Length)
      creditsFilename = this.m_creditsYearsAvailable[this.m_creditsYearIndex].ContentsFilename;
    string filePath = this.GetFilePath(creditsFilename);
    if (filePath == null)
    {
      Error.AddDevWarning("Credits Error", "CreditsDisplay.LoadCreditsText() - Failed to find file for CREDITS: {0}", (object) creditsFilename);
      this.m_creditsTextLoaded = true;
    }
    else
    {
      try
      {
        this.m_creditLines = File.ReadAllLines(filePath);
        this.m_creditsTextLoadSucceeded = true;
      }
      catch (Exception ex)
      {
        Error.AddDevWarning("Credits Error", "CreditsDisplay.LoadCreditsText() - Failed to read \"{0}\".\n\nException: {1}", (object) filePath, (object) ex.Message);
      }
      for (int index = 0; index < this.m_creditLines.Length; ++index)
      {
        string key = this.m_creditLines[index].Trim();
        if (key == "<!-- no more cards displayed after this line -->")
        {
          this.m_creditLines[index] = string.Empty;
          break;
        }
        string str;
        if (this.m_creditsCardsByName != null && this.m_creditsCardsByName.TryGetValue(key, out str) && !this.m_cardsToLoad.Contains(str))
          this.m_cardsToLoad.Add(str);
      }
      this.m_creditsTextLoaded = true;
      this.LoadAllCreditsCards();
    }
  }

  private string GetFilePath(string creditsFilename)
  {
    if (creditsFilename == null)
      return (string) null;
    string assetPath = GameStrings.GetAssetPath(Localization.GetActualLocale(), creditsFilename);
    return assetPath != null && File.Exists(assetPath) ? assetPath : (string) null;
  }

  private void FlopCredits()
  {
    this.m_currentText = !((UnityEngine.Object) this.m_currentText == (UnityEngine.Object) this.m_creditsText1) ? this.m_creditsText1 : this.m_creditsText2;
    this.m_currentText.Text = this.GetNextCreditsChunk();
    this.DropText();
  }

  private void DropText()
  {
    UberText uberText = this.m_creditsText1;
    if ((UnityEngine.Object) this.m_currentText == (UnityEngine.Object) this.m_creditsText1)
      uberText = this.m_creditsText2;
    float z = 1.8649f;
    TransformUtil.SetPoint(this.m_currentText.gameObject, Anchor.FRONT, uberText.gameObject, Anchor.BACK, new Vector3(0.0f, 0.0f, z));
  }

  private string GetNextCreditsChunk()
  {
    string nextCreditsChunk = "";
    int currentLine = this.m_currentLine;
    int num = 70;
    for (int index = 0; index < num; ++index)
    {
      if (this.m_creditLines.Length < index + currentLine + 1)
      {
        this.m_creditsDone = true;
        this.StartEndCreditsTimer();
        return nextCreditsChunk;
      }
      string creditLine = this.m_creditLines[index + currentLine];
      if (creditLine.Length > 38)
      {
        num -= Mathf.CeilToInt((float) (creditLine.Length / 38));
        if (index > num && index > 60)
          break;
      }
      nextCreditsChunk = nextCreditsChunk + creditLine + Environment.NewLine;
      ++this.m_currentLine;
    }
    return nextCreditsChunk;
  }

  private void ActorLoadedCallback(AssetReference assetRef, GameObject go, object callbackData) => this.m_fakeCards.Add(go.GetComponent<Actor>());

  private void Start()
  {
    Navigation.Push(new Navigation.NavigateBackHandler(this.EndCredits));
    this.StartCoroutine(this.NotifySceneLoadedWhenReady());
  }

  private IEnumerator NotifySceneLoadedWhenReady()
  {
    CreditsDisplay creditsDisplay = this;
    while (creditsDisplay.m_fakeCards.Count < 2)
      yield return (object) null;
    while (!creditsDisplay.m_creditsTextLoaded)
      yield return (object) null;
    Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(creditsDisplay.OnBoxOpened));
    SceneMgr.Get().NotifySceneLoaded();
  }

  private void OnBoxOpened(object userData)
  {
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxOpened));
    if (!this.m_creditsTextLoadSucceeded)
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    else
    {
      MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Credits);
      this.START_CREDITS_COROUTINE = this.StartCoroutine(this.StartCredits());
    }
  }

  private IEnumerator StartCredits()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CreditsDisplay creditsDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      creditsDisplay.SHOW_NEW_CARD_COROUTINE = creditsDisplay.StartCoroutine(creditsDisplay.ShowNewCard());
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    creditsDisplay.m_creditsText2.Text = creditsDisplay.GetNextCreditsChunk();
    creditsDisplay.m_currentText = creditsDisplay.m_creditsText2;
    creditsDisplay.FlopCredits();
    creditsDisplay.started = true;
    creditsDisplay.m_creditsRoot.SetActive(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(creditsDisplay.m_firstCardDelay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator ShowNewCard()
  {
    CreditsDisplay creditsDisplay = this;
    if (creditsDisplay.m_creditsDefs != null && creditsDisplay.m_creditsDefs.Count != 0)
    {
      float time = 1f;
      int index1 = 0;
      if (creditsDisplay.m_lastCard == 0)
        index1 = 1;
      creditsDisplay.m_lastCard = index1;
      creditsDisplay.m_shownCreditsCard = creditsDisplay.m_fakeCards[index1];
      int index2 = creditsDisplay.m_sortedCards ? 0 : UnityEngine.Random.Range(0, creditsDisplay.m_creditsDefs.Count);
      creditsDisplay.m_shownCreditsCard.SetCardDef(creditsDisplay.m_creditsDefs[index2].DisposableCardDef);
      EntityDef entityDef = creditsDisplay.m_creditsDefs[index2].EntityDef;
      int num = entityDef.GetCardId() == "CRED_10" ? 1 : 0;
      if (num != 0)
        entityDef.SetTag<TAG_RACE>(GAME_TAG.CARDRACE, TAG_RACE.PIRATE);
      creditsDisplay.m_shownCreditsCard.SetEntityDef(entityDef);
      creditsDisplay.m_creditsDefs.DisposeAndRemoveAt<DefLoader.DisposableFullDef>(index2);
      creditsDisplay.m_shownCreditsCard.UpdateAllComponents();
      creditsDisplay.m_shownCreditsCard.Show();
      if (num != 0)
        creditsDisplay.m_shownCreditsCard.GetRaceText().Text = GameStrings.Get("GLUE_NINJA");
      creditsDisplay.m_shownCreditsCard.transform.position = creditsDisplay.m_offscreenCardBone.position;
      creditsDisplay.m_shownCreditsCard.transform.localScale = creditsDisplay.m_offscreenCardBone.localScale;
      creditsDisplay.m_shownCreditsCard.transform.localEulerAngles = creditsDisplay.m_offscreenCardBone.localEulerAngles;
      SoundManager.Get().LoadAndPlay(creditsDisplay.s_credits_card_enter[UnityEngine.Random.Range(0, 2)]);
      iTween.MoveTo(creditsDisplay.m_shownCreditsCard.gameObject, creditsDisplay.m_cardBone.position, time);
      iTween.RotateTo(creditsDisplay.m_shownCreditsCard.gameObject, creditsDisplay.m_cardBone.localEulerAngles, time);
      Actor oldActor = creditsDisplay.m_shownCreditsCard;
      yield return (object) new WaitForSeconds(0.5f);
      SoundManager.Get().LoadAndPlay(creditsDisplay.s_tavern_crowd_play_reaction_positive[UnityEngine.Random.Range(0, 4)]);
      yield return (object) new WaitForSeconds(7.5f);
      if ((UnityEngine.Object) creditsDisplay.m_shownCreditsCard != (UnityEngine.Object) null)
      {
        creditsDisplay.m_shownCreditsCard.ActivateSpellBirthState(SpellType.BURN);
        SoundManager.Get().LoadAndPlay(creditsDisplay.s_credits_card_embers[UnityEngine.Random.Range(0, 2)]);
        if ((UnityEngine.Object) creditsDisplay.m_shownCreditsCard == (UnityEngine.Object) oldActor)
          creditsDisplay.m_shownCreditsCard = (Actor) null;
      }
      yield return (object) new WaitForSeconds(creditsDisplay.m_cardDelay);
      creditsDisplay.SHOW_NEW_CARD_COROUTINE = creditsDisplay.StartCoroutine(creditsDisplay.ShowNewCard());
    }
  }

  private void Update()
  {
    Network.Get().ProcessNetwork();
    if (!this.started)
      return;
    this.m_creditsRoot.transform.localPosition += new Vector3(0.0f, 0.0f, this.m_creditsScrollSpeed * Time.deltaTime);
    if (this.m_creditsDone || (UnityEngine.Object) this.m_currentText == (UnityEngine.Object) null)
      return;
    if ((double) this.GetTopOfCurrentCredits() > (double) this.m_flopPoint.position.z)
      this.FlopCredits();
    this.ReadKeyboardInput();
  }

  private float GetTopOfCurrentCredits()
  {
    Bounds worldSpaceBounds = this.m_currentText.GetTextWorldSpaceBounds();
    return worldSpaceBounds.center.z + worldSpaceBounds.extents.z;
  }

  private void OnFullDefLoaded(string cardID, DefLoader.DisposableFullDef def, object userData) => this.m_creditsDefs.Add(def);

  private void OnDonePressed(UIEvent e)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      Box.Get().m_letterboxingContainer.SetActive(true);
    Navigation.GoBack();
  }

  private void OnYearPressed1(UIEvent e)
  {
    if (this.m_creditsYearIndex <= 0)
      ++this.m_creditsYearIndex;
    else if (this.m_creditsYearIndex >= this.m_creditsYearsAvailable.Length - 1)
      this.m_creditsYearIndex = this.m_creditsYearsAvailable.Length - 3;
    else
      --this.m_creditsYearIndex;
    if (this.m_creditsYearIndex < 0 || this.m_creditsYearIndex >= this.m_creditsYearsAvailable.Length)
      this.m_creditsYearIndex = -1;
    this.OnYearPressed();
  }

  private void OnYearPressed2(UIEvent e)
  {
    if (this.m_creditsYearIndex <= 0)
      this.m_creditsYearIndex += 2;
    else if (this.m_creditsYearIndex >= this.m_creditsYearsAvailable.Length - 1)
      this.m_creditsYearIndex = this.m_creditsYearsAvailable.Length - 2;
    else
      ++this.m_creditsYearIndex;
    if (this.m_creditsYearIndex < 0 || this.m_creditsYearIndex >= this.m_creditsYearsAvailable.Length)
      this.m_creditsYearIndex = -1;
    this.OnYearPressed();
  }

  private void OnYearPressed()
  {
    this.StopAndClearCoroutine(ref this.START_CREDITS_COROUTINE);
    this.StopAndClearCoroutine(ref this.SHOW_NEW_CARD_COROUTINE);
    if ((UnityEngine.Object) this.m_shownCreditsCard != (UnityEngine.Object) null)
    {
      this.m_shownCreditsCard.ActivateSpellBirthState(SpellType.BURN);
      SoundManager.Get().LoadAndPlay(this.s_credits_card_enter[UnityEngine.Random.Range(0, 2)]);
      this.m_shownCreditsCard = (Actor) null;
    }
    this.StartCoroutine(this.ResetCredits());
  }

  private void OnFasterButtonPressed(UIEvent e)
  {
    ++this.m_creditsScrollSpeedCurrentStep;
    if (this.m_creditsScrollSpeedCurrentStep == 3)
      this.m_fasterButton.gameObject.SetActive(false);
    this.m_slowerButton.gameObject.SetActive(true);
    this.m_creditsScrollSpeed = (float) (3.5 + (double) this.m_creditsScrollSpeedCurrentStep * 0.75);
  }

  private void OnSlowerButtonPressed(UIEvent e)
  {
    --this.m_creditsScrollSpeedCurrentStep;
    if (this.m_creditsScrollSpeedCurrentStep == -3)
      this.m_slowerButton.gameObject.SetActive(false);
    this.m_fasterButton.gameObject.SetActive(true);
    this.m_creditsScrollSpeed = (float) (3.5 + (double) this.m_creditsScrollSpeedCurrentStep * 0.75);
  }

  private void UpdateYearButtons()
  {
    int index1;
    int index2;
    if (this.m_creditsYearIndex <= 0)
    {
      index1 = 1;
      index2 = 2;
    }
    else if (this.m_creditsYearIndex >= this.m_creditsYearsAvailable.Length - 1)
    {
      index1 = this.m_creditsYearsAvailable.Length - 3;
      index2 = this.m_creditsYearsAvailable.Length - 2;
    }
    else
    {
      index1 = this.m_creditsYearIndex - 1;
      index2 = this.m_creditsYearIndex + 1;
    }
    if (index1 >= 0 && index1 < this.m_creditsYearsAvailable.Length)
    {
      this.m_yearButton1.gameObject.SetActive(true);
      string buttonLabel = (string) this.m_creditsYearsAvailable[index1].ButtonLabel;
      if (string.IsNullOrEmpty(buttonLabel))
        buttonLabel = this.m_creditsYearsAvailable[index1].ID.ToString();
      this.m_yearButton1.SetText(buttonLabel);
    }
    else
      this.m_yearButton1.gameObject.SetActive(false);
    if (index2 >= 0 && index2 < this.m_creditsYearsAvailable.Length)
    {
      this.m_yearButton2.gameObject.SetActive(true);
      string buttonLabel = (string) this.m_creditsYearsAvailable[index2].ButtonLabel;
      if (string.IsNullOrEmpty(buttonLabel))
        buttonLabel = this.m_creditsYearsAvailable[index2].ID.ToString();
      this.m_yearButton2.SetText(buttonLabel);
    }
    else
      this.m_yearButton2.gameObject.SetActive(false);
  }

  private IEnumerator ResetCredits()
  {
    CreditsDisplay creditsDisplay = this;
    creditsDisplay.m_currentText = (UberText) null;
    creditsDisplay.m_creditsText1.Text = "";
    creditsDisplay.m_creditsText2.Text = "";
    creditsDisplay.started = false;
    creditsDisplay.m_creditsTextLoaded = false;
    creditsDisplay.m_creditsTextLoadSucceeded = false;
    creditsDisplay.m_creditsDone = false;
    creditsDisplay.m_currentLine = 0;
    creditsDisplay.m_creditLines = (string[]) null;
    creditsDisplay.UpdateYearButtons();
    creditsDisplay.m_creditsText1.transform.localPosition = creditsDisplay.creditsText1StartLocalPosition;
    creditsDisplay.m_creditsText2.transform.localPosition = creditsDisplay.creditsText2StartLocalPosition;
    creditsDisplay.m_creditsRoot.transform.localPosition = creditsDisplay.creditsRootStartLocalPosition;
    creditsDisplay.m_lastCard = 1;
    creditsDisplay.ReleaseAllCreditsCards();
    creditsDisplay.LoadCreditsText();
    while (!creditsDisplay.m_creditsTextLoaded)
      yield return (object) null;
    creditsDisplay.StopAndClearCoroutine(ref creditsDisplay.END_CREDITS_COROUTINE);
    creditsDisplay.StopAndClearCoroutine(ref creditsDisplay.START_CREDITS_COROUTINE);
    creditsDisplay.START_CREDITS_COROUTINE = creditsDisplay.StartCoroutine(creditsDisplay.StartCredits());
  }

  private bool EndCredits()
  {
    iTween.FadeTo(this.m_creditsText1.gameObject, 0.0f, 0.1f);
    iTween.FadeTo(this.m_creditsText2.gameObject, 0.0f, 0.1f);
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    return true;
  }

  private void StartEndCreditsTimer() => this.END_CREDITS_COROUTINE = this.StartCoroutine(this.EndCreditsTimer());

  private IEnumerator EndCreditsTimer()
  {
    yield return (object) new WaitForSeconds(300f);
    if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB) && SceneMgr.Get().GetMode() == SceneMgr.Mode.CREDITS)
      Navigation.GoBack();
  }

  private void ReadKeyboardInput()
  {
    int num = 0;
    int touchCount = Input.touchCount;
    for (int index = 0; index < touchCount; ++index)
    {
      if (Input.GetTouch(index).phase == TouchPhase.Began)
        ++num;
    }
    bool flag = !HearthstoneApplication.IsPublic();
    if (InputCollection.GetKeyDown(KeyCode.N) || num == 2)
    {
      if (this.m_creditsDefs == null || this.m_creditsDefs.Count == 0)
      {
        this.LoadAllCreditsCards();
        if (!flag)
          return;
        UIStatus.Get().AddInfo(string.Format("Reset cards list: {0} to display", (object) this.m_creditsDefs.Count));
      }
      else
      {
        this.StopAndClearCoroutine(ref this.SHOW_NEW_CARD_COROUTINE);
        if ((UnityEngine.Object) this.m_shownCreditsCard != (UnityEngine.Object) null)
        {
          this.m_shownCreditsCard.ActivateSpellBirthState(SpellType.BURN);
          this.m_shownCreditsCard = (Actor) null;
        }
        this.SHOW_NEW_CARD_COROUTINE = this.StartCoroutine(this.ShowNewCard());
      }
    }
    else
    {
      if (!flag)
        return;
      if (InputCollection.GetKeyDown(KeyCode.Plus) || InputCollection.GetKeyDown(KeyCode.KeypadPlus))
      {
        this.OnFasterButtonPressed((UIEvent) null);
        this.m_slowerButton.gameObject.SetActive(false);
        this.m_fasterButton.gameObject.SetActive(false);
      }
      else if (InputCollection.GetKeyDown(KeyCode.Minus) || InputCollection.GetKeyDown(KeyCode.KeypadMinus))
      {
        this.OnSlowerButtonPressed((UIEvent) null);
        this.m_slowerButton.gameObject.SetActive(false);
        this.m_fasterButton.gameObject.SetActive(false);
      }
      else if (InputCollection.GetKeyDown(KeyCode.Space))
      {
        this.LoadAllCreditsCards();
        UIStatus.Get().AddInfo(string.Format("Reset cards list: {0} to display", (object) this.m_creditsDefs.Count));
      }
      else if (InputCollection.GetKeyDown(KeyCode.S) || num == 5)
      {
        if (this.m_creditsDefs == null)
          return;
        this.m_sortedCards = !this.m_sortedCards;
        if (this.m_sortedCards)
          this.m_creditsDefs.Sort((Comparison<DefLoader.DisposableFullDef>) ((a, b) => this.m_cardsToLoad.IndexOf(a.EntityDef.GetCardId()).CompareTo(this.m_cardsToLoad.IndexOf(b.EntityDef.GetCardId()))));
        UIStatus.Get().AddInfo(string.Format("{0} remaining cards to display {1}.", (object) this.m_creditsDefs.Count, this.m_sortedCards ? (object) "sorted" : (object) "randomized"));
      }
      else
      {
        if (!InputCollection.GetKeyDown(KeyCode.D))
          return;
        // ISSUE: object of a compiler-generated type is created
        string[] array = this.m_creditsDefs.Select(d => new \u003C\u003Ef__AnonymousType0<DefLoader.DisposableFullDef, EntityDef>(d, d.EntityDef)).Select(_param1 => new
        {
          \u003C\u003Eh__TransparentIdentifier0 = _param1,
          c = _param1.e == null ? (CardDbfRecord) null : GameDbf.GetIndex().GetCardRecord(_param1.e.GetCardId())
        }).Select(_param1 =>
        {
          if (_param1.\u003C\u003Eh__TransparentIdentifier0.e == null)
            return (string) null;
          return string.Format("{0}-{1} {2}{3}", (object) GameUtils.TranslateCardIdToDbId(_param1.\u003C\u003Eh__TransparentIdentifier0.e.GetCardId()), (object) _param1.\u003C\u003Eh__TransparentIdentifier0.e.GetCardId(), (object) _param1.\u003C\u003Eh__TransparentIdentifier0.e.GetName(), _param1.c == null || string.IsNullOrEmpty(_param1.c.CreditsCardName) ? (object) string.Empty : (object) string.Format(" ({0})", (object) _param1.c.CreditsCardName));
        }).Where<string>((Func<string, bool>) (n => !string.IsNullOrEmpty(n))).ToArray<string>();
        Log.All.Print("Credits Cards to show:\n{0}", (object) string.Join("\n", array));
        UIStatus.Get().AddInfo(string.Format("Dumped to log: {0} remaining cards to display.", (object) array.Length));
      }
    }
  }
}
