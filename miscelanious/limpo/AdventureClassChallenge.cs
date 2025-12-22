using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureClassChallenge : MonoBehaviour
{
  private readonly float[] EMPTY_SLOT_UV_OFFSET = new float[6]
  {
    0.0f,
    0.223f,
    0.377f,
    0.535f,
    0.69f,
    0.85f
  };
  private const float CHALLENGE_BUTTON_OFFSET = 4.3f;
  private const int VISIBLE_SLOT_COUNT = 10;
  [CustomEditField(Sections = "DBF Stuff")]
  public UberText m_ModeName;
  [CustomEditField(Sections = "Class Challenge Buttons")]
  public GameObject m_ClassChallengeButtonPrefab;
  [CustomEditField(Sections = "Class Challenge Buttons")]
  public Vector3 m_ClassChallengeButtonSpacing;
  [CustomEditField(Sections = "Class Challenge Buttons")]
  public GameObject m_ChallengeButtonContainer;
  [CustomEditField(Sections = "Class Challenge Buttons")]
  public GameObject m_EmptyChallengeButtonSlot;
  [CustomEditField(Sections = "Class Challenge Buttons")]
  public float m_ChallengeButtonHeight;
  [CustomEditField(Sections = "Class Challenge Buttons")]
  public UIBScrollable m_ChallengeButtonScroller;
  [CustomEditField(Sections = "Hero Portraits")]
  public GameObject m_LeftHeroContainer;
  [CustomEditField(Sections = "Hero Portraits")]
  public GameObject m_RightHeroContainer;
  [CustomEditField(Sections = "Hero Portraits")]
  public UberText m_LeftHeroName;
  [CustomEditField(Sections = "Hero Portraits")]
  public UberText m_RightHeroName;
  [CustomEditField(Sections = "Versus Text", T = EditType.GAME_OBJECT)]
  public string m_VersusTextPrefab;
  [CustomEditField(Sections = "Versus Text")]
  public GameObject m_VersusTextContainer;
  [CustomEditField(Sections = "Versus Text")]
  public Color m_VersusTextColor;
  [CustomEditField(Sections = "Text")]
  public UberText m_ChallengeTitle;
  [CustomEditField(Sections = "Text")]
  public UberText m_ChallengeDescription;
  [CustomEditField(Sections = "Basic UI")]
  public PlayButton m_PlayButton;
  [CustomEditField(Sections = "Basic UI")]
  public UIBButton m_BackButton;
  [CustomEditField(Sections = "Reward UI")]
  public AdventureClassChallengeChestButton m_ChestButton;
  [CustomEditField(Sections = "Reward UI")]
  public GameObject m_ChestButtonCover;
  [CustomEditField(Sections = "Reward UI")]
  public Transform m_RewardBone;
  private List<AdventureClassChallenge.ClassChallengeData> m_ClassChallenges = new List<AdventureClassChallenge.ClassChallengeData>();
  private Map<int, int> m_ScenarioChallengeLookup = new Map<int, int>();
  private int m_UVoffset;
  private AdventureClassChallengeButton m_SelectedButton;
  private GameObject m_LeftHero;
  private GameObject m_RightHero;
  private int m_SelectedScenario;
  private bool m_gameDenied;

  private void Awake()
  {
    this.transform.position = new Vector3(-500f, 0.0f, 0.0f);
    this.m_BackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.BackButton()));
    this.m_PlayButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Play()));
    this.m_EmptyChallengeButtonSlot.SetActive(false);
    AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_VersusTextPrefab, new PrefabCallback<GameObject>(this.OnVersusLettersLoaded));
  }

  private void Start()
  {
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.InitModeName();
    this.InitAdventureChallenges();
    Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureClassChallenge.OnNavigateBack));
    this.StartCoroutine(this.CreateChallengeButtons());
  }

  private void OnDestroy() => GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));

  private void InitModeName()
  {
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) AdventureConfig.Get().GetSelectedAdventure(), (int) AdventureConfig.Get().GetSelectedMode());
    this.m_ModeName.Text = (string) ((bool) UniversalInputManager.UsePhoneUI ? adventureDataRecord.ShortName : adventureDataRecord.Name);
  }

  private void InitAdventureChallenges()
  {
    List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords();
    records.Sort((Comparison<ScenarioDbfRecord>) ((a, b) => a.SortOrder - b.SortOrder));
    foreach (ScenarioDbfRecord scenarioDbfRecord in records)
    {
      if ((AdventureDbId) scenarioDbfRecord.AdventureId == AdventureConfig.Get().GetSelectedAdventure() && scenarioDbfRecord.ModeId == 4)
      {
        int player1HeroCardId = scenarioDbfRecord.Player1HeroCardId;
        int player2HeroCardId = scenarioDbfRecord.ClientPlayer2HeroCardId;
        if (player2HeroCardId == 0)
          player2HeroCardId = scenarioDbfRecord.Player2HeroCardId;
        AdventureClassChallenge.ClassChallengeData classChallengeData = new AdventureClassChallenge.ClassChallengeData();
        classChallengeData.scenarioRecord = scenarioDbfRecord;
        classChallengeData.heroID0 = GameUtils.TranslateDbIdToCardId(player1HeroCardId);
        classChallengeData.heroID1 = GameUtils.TranslateDbIdToCardId(player2HeroCardId);
        classChallengeData.unlocked = AdventureProgressMgr.Get().CanPlayScenario(scenarioDbfRecord.ID);
        classChallengeData.defeated = AdventureProgressMgr.Get().HasDefeatedScenario(scenarioDbfRecord.ID);
        classChallengeData.name = (string) scenarioDbfRecord.ShortName;
        classChallengeData.title = (string) scenarioDbfRecord.Name;
        classChallengeData.description = (string) (!(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty((string) scenarioDbfRecord.ShortDescription) ? scenarioDbfRecord.Description : scenarioDbfRecord.ShortDescription);
        classChallengeData.completedDescription = (string) scenarioDbfRecord.CompletedDescription;
        classChallengeData.opponentName = (string) scenarioDbfRecord.OpponentName;
        this.m_ScenarioChallengeLookup.Add(scenarioDbfRecord.ID, this.m_ClassChallenges.Count);
        this.m_ClassChallenges.Add(classChallengeData);
      }
    }
  }

  private int BossCreateParamsSortComparison(
    AdventureClassChallenge.ClassChallengeData data1,
    AdventureClassChallenge.ClassChallengeData data2)
  {
    return GameUtils.MissionSortComparison(data1.scenarioRecord, data2.scenarioRecord);
  }

  private IEnumerator CreateChallengeButtons()
  {
    AdventureClassChallenge adventureClassChallenge = this;
    int num1 = 0;
    int lastSelectedMission = (int) AdventureConfig.Get().GetLastSelectedMission();
    for (int index = 0; index < adventureClassChallenge.m_ClassChallenges.Count; ++index)
    {
      AdventureClassChallenge.ClassChallengeData classChallenge = adventureClassChallenge.m_ClassChallenges[index];
      if (classChallenge.unlocked)
      {
        GameObject gameObject = (GameObject) GameUtils.Instantiate(adventureClassChallenge.m_ClassChallengeButtonPrefab, adventureClassChallenge.m_ChallengeButtonContainer);
        gameObject.transform.localPosition = adventureClassChallenge.m_ClassChallengeButtonSpacing * (float) num1;
        AdventureClassChallengeButton component = gameObject.GetComponent<AdventureClassChallengeButton>();
        component.m_Text.Text = classChallenge.name;
        component.m_ScenarioID = classChallenge.scenarioRecord.ID;
        bool flag = AdventureProgressMgr.Get().ScenarioHasRewardData(component.m_ScenarioID);
        component.m_Chest.SetActive(!classChallenge.defeated & flag);
        component.m_Checkmark.SetActive(classChallenge.defeated);
        component.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(adventureClassChallenge.ButtonPressed));
        adventureClassChallenge.LoadButtonPortrait(component, classChallenge.heroID1);
        if (lastSelectedMission == component.m_ScenarioID || !(bool) (UnityEngine.Object) adventureClassChallenge.m_SelectedButton)
          adventureClassChallenge.m_SelectedButton = component;
        ++num1;
      }
    }
    int num2 = 10 - num1;
    if (num2 <= 0)
    {
      Debug.LogError((object) string.Format("Adventure Class Challenge tray UI doesn't support scrolling yet. More than {0} buttons where added.", (object) 10));
    }
    else
    {
      for (int index = 0; index < num2; ++index)
      {
        GameObject gameObject = (GameObject) GameUtils.Instantiate(adventureClassChallenge.m_EmptyChallengeButtonSlot, adventureClassChallenge.m_ChallengeButtonContainer);
        gameObject.transform.localPosition = adventureClassChallenge.m_ClassChallengeButtonSpacing * (float) (num1 + index);
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
        RendererExtension.GetMaterial(gameObject.GetComponentInChildren<Renderer>()).mainTextureOffset = new UnityEngine.Vector2(0.0f, adventureClassChallenge.EMPTY_SLOT_UV_OFFSET[adventureClassChallenge.m_UVoffset]);
        ++adventureClassChallenge.m_UVoffset;
        if (adventureClassChallenge.m_UVoffset > 5)
          adventureClassChallenge.m_UVoffset = 0;
      }
      yield return (object) null;
      if ((UnityEngine.Object) adventureClassChallenge.m_SelectedButton == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "AdventureClassChallenge.m_SelectedButton is null!\nThis it's likely that this means there are no valid class challenges available but we still tried to load the screen.");
        Navigation.RemoveHandler(new Navigation.NavigateBackHandler(AdventureClassChallenge.OnNavigateBack));
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      }
      else
      {
        adventureClassChallenge.SetSelectedButton(adventureClassChallenge.m_SelectedButton);
        adventureClassChallenge.m_SelectedButton.Select(false);
        adventureClassChallenge.GetRewardCardForSelectedScenario();
        adventureClassChallenge.m_PlayButton.Enable();
        if ((UnityEngine.Object) adventureClassChallenge.m_ChallengeButtonScroller != (UnityEngine.Object) null)
        {
          // ISSUE: reference to a compiler-generated method
          adventureClassChallenge.m_ChallengeButtonScroller.SetScrollHeightCallback(new UIBScrollable.ScrollHeightCallback(adventureClassChallenge.\u003CCreateChallengeButtons\u003Eb__40_0));
        }
        adventureClassChallenge.GetComponent<AdventureSubScene>().SetIsLoaded(true);
      }
    }
  }

  private void ButtonPressed(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_ChallengeButtonScroller != (UnityEngine.Object) null && this.m_ChallengeButtonScroller.IsTouchDragging())
      return;
    AdventureClassChallengeButton element = (AdventureClassChallengeButton) e.GetElement();
    this.m_SelectedButton.Deselect();
    this.SetSelectedButton(element);
    element.Select(true);
    this.m_SelectedScenario = element.m_ScenarioID;
    this.m_SelectedButton = element;
    this.GetRewardCardForSelectedScenario();
  }

  private void SetSelectedButton(AdventureClassChallengeButton button)
  {
    int scenarioId = button.m_ScenarioID;
    AdventureConfig.Get().SetMission((ScenarioDbId) scenarioId);
    this.SetScenario(scenarioId);
  }

  private void LoadButtonPortrait(AdventureClassChallengeButton button, string heroID) => DefLoader.Get().LoadCardDef(heroID, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnButtonCardDefLoaded), (object) button);

  private void OnButtonCardDefLoaded(
    string cardId,
    DefLoader.DisposableCardDef disposableCardDef,
    object userData)
  {
    AdventureClassChallengeButton classChallengeButton = (AdventureClassChallengeButton) userData;
    ServiceManager.Get<DisposablesCleaner>()?.Attach(classChallengeButton.gameObject, (IDisposable) disposableCardDef);
    Material practiceAiPortrait = disposableCardDef.CardDef.GetPracticeAIPortrait();
    if (!((UnityEngine.Object) practiceAiPortrait != (UnityEngine.Object) null))
      return;
    practiceAiPortrait.mainTexture = disposableCardDef.CardDef.GetPortraitTexture(TAG_PREMIUM.NORMAL);
    classChallengeButton.SetPortraitMaterial(practiceAiPortrait);
  }

  private void SetScenario(int scenarioID)
  {
    this.m_SelectedScenario = scenarioID;
    AdventureClassChallenge.ClassChallengeData classChallenge = this.m_ClassChallenges[this.m_ScenarioChallengeLookup[scenarioID]];
    this.LoadHero(0, classChallenge.heroID0);
    this.LoadHero(1, classChallenge.heroID1);
    this.m_RightHeroName.Text = classChallenge.opponentName;
    this.m_ChallengeTitle.Text = classChallenge.title;
    this.m_ChallengeDescription.Text = !classChallenge.defeated ? classChallenge.description : classChallenge.completedDescription;
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    bool flag = AdventureProgressMgr.Get().ScenarioHasRewardData(scenarioID);
    if (this.m_ClassChallenges[this.m_ScenarioChallengeLookup[scenarioID]].defeated || !flag)
    {
      this.m_ChestButton.gameObject.SetActive(false);
      this.m_ChestButtonCover.SetActive(true);
    }
    else
    {
      this.m_ChestButton.gameObject.SetActive(true);
      this.m_ChestButtonCover.SetActive(false);
    }
  }

  private void LoadHero(int heroNum, string heroID) => DefLoader.Get().LoadFullDef(heroID, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroFullDefLoaded), (object) new AdventureClassChallenge.HeroLoadData()
  {
    heroNum = heroNum,
    heroID = heroID
  });

  private void OnHeroFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef fullDef,
    object userData)
  {
    if (fullDef == null)
    {
      Debug.LogWarning((object) string.Format("AdventureClassChallenge.OnHeroFullDefLoaded() - FAILED to load \"{0}\"", (object) cardId));
    }
    else
    {
      AdventureClassChallenge.HeroLoadData callbackData = (AdventureClassChallenge.HeroLoadData) userData;
      callbackData.fulldef = fullDef;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d", new PrefabCallback<GameObject>(this.OnActorLoaded), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
    }
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    AdventureClassChallenge.HeroLoadData heroLoadData = (AdventureClassChallenge.HeroLoadData) callbackData;
    using (heroLoadData.fulldef)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AdventureClassChallenge.OnActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      }
      else
      {
        Actor component = go.GetComponent<Actor>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("AdventureClassChallenge.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) this.name));
        }
        else
        {
          component.TurnOffCollider();
          component.SetUnlit();
          UnityEngine.Object.Destroy((UnityEngine.Object) component.m_healthObject);
          UnityEngine.Object.Destroy((UnityEngine.Object) component.m_attackObject);
          component.SetEntityDef(heroLoadData.fulldef.EntityDef);
          component.SetCardDef(heroLoadData.fulldef.DisposableCardDef);
          component.SetPremium(TAG_PREMIUM.NORMAL);
          component.UpdateAllComponents();
          GameObject parent = this.m_LeftHeroContainer;
          if (heroLoadData.heroNum == 0)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_LeftHero);
            this.m_LeftHero = go;
            this.m_LeftHeroName.Text = heroLoadData.fulldef.EntityDef.GetName();
          }
          else
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_RightHero);
            this.m_RightHero = go;
            parent = this.m_RightHeroContainer;
          }
          GameUtils.SetParent((Component) component, parent);
          component.transform.localRotation = Quaternion.identity;
          component.transform.localScale = Vector3.one;
          component.GetAttackObject().Hide();
          component.Show();
        }
      }
    }
  }

  private void OnVersusLettersLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("AdventureClassChallenge.OnVersusLettersLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      GameUtils.SetParent(go, this.m_VersusTextContainer);
      go.GetComponentInChildren<VS>().ActivateShadow();
      go.transform.localRotation = Quaternion.identity;
      go.transform.Rotate(new Vector3(0.0f, 180f, 0.0f));
      go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
      Component[] componentsInChildren = go.GetComponentsInChildren(typeof (Renderer));
      for (int index = 0; index < componentsInChildren.Length - 1; ++index)
        RendererExtension.GetMaterial((Renderer) componentsInChildren[index]).SetColor("_Color", this.m_VersusTextColor);
    }
  }

  private static bool OnNavigateBack()
  {
    AdventureConfig.Get().SubSceneGoBack();
    return true;
  }

  private void BackButton() => Navigation.GoBack();

  private void Play()
  {
    this.m_PlayButton.Disable();
    GameMgr.Get().FindGame(GameType.GT_VS_AI, FormatType.FT_WILD, this.m_SelectedScenario);
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    if (eventData.m_state == FindGameState.INVALID)
      this.m_PlayButton.Enable();
    return false;
  }

  private void GetRewardCardForSelectedScenario()
  {
    if ((UnityEngine.Object) this.m_RewardBone == (UnityEngine.Object) null)
      return;
    this.m_ChestButton.m_IsRewardLoading = true;
    List<RewardData> defeatingScenario = AdventureProgressMgr.Get().GetImmediateRewardsForDefeatingScenario(this.m_SelectedScenario);
    if (defeatingScenario == null || defeatingScenario.Count <= 0)
      return;
    defeatingScenario[0].LoadRewardObject(new Reward.DelOnRewardLoaded(this.RewardCardLoaded));
  }

  private void RewardCardLoaded(Reward reward, object callbackData)
  {
    if ((UnityEngine.Object) reward == (UnityEngine.Object) null)
      Debug.LogWarning((object) string.Format("AdventureClassChallenge.RewardCardLoaded() - FAILED to load reward \"{0}\"", (object) this.name));
    else if ((UnityEngine.Object) reward.gameObject == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AdventureClassChallenge.RewardCardLoaded() - Reward GameObject is null \"{0}\"", (object) this.name));
    }
    else
    {
      reward.gameObject.transform.parent = this.m_ChestButton.transform;
      CardReward component = reward.GetComponent<CardReward>();
      if ((UnityEngine.Object) this.m_ChestButton.m_RewardCard != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_ChestButton.m_RewardCard);
      this.m_ChestButton.m_RewardCard = component.m_nonHeroCardsRoot;
      GameUtils.SetParent(component.m_nonHeroCardsRoot, (Component) this.m_RewardBone);
      component.m_nonHeroCardsRoot.SetActive(false);
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
      this.m_ChestButton.m_IsRewardLoading = false;
    }
  }

  private void OnBoxTransitionFinished(object userData) => Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));

  private class ClassChallengeData
  {
    public ScenarioDbfRecord scenarioRecord;
    public bool unlocked;
    public bool defeated;
    public string heroID0;
    public string heroID1;
    public string name;
    public string title;
    public string description;
    public string completedDescription;
    public string opponentName;
  }

  private class HeroLoadData
  {
    public int heroNum;
    public string heroID;
    public DefLoader.DisposableFullDef fulldef;
  }
}
