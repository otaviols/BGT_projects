using Assets;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Time;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass(DefaultCollapsed = true)]
public class MulliganManager : MonoBehaviour
{
  public AnimationClip cardAnimatesFromBoardToDeck;
  public AnimationClip cardAnimatesFromBoardToDeck_iPhone;
  public AnimationClip cardAnimatesFromTableToSky;
  public AnimationClip cardAnimatesFromDeckToBoard;
  public AnimationClip shuffleDeck;
  public AnimationClip myheroAnimatesToPosition;
  public AnimationClip hisheroAnimatesToPosition;
  public AnimationClip myheroAnimatesToPosition_iPhone;
  public AnimationClip hisheroAnimatesToPosition_iPhone;
  public GameObject coinPrefab;
  public GameObject weldPrefab;
  public GameObject mulliganChooseBannerPrefab;
  public GameObject mulliganDetailLabelPrefab;
  public GameObject mulliganKeepLabelPrefab;
  public MulliganReplaceLabel mulliganReplaceLabelPrefab;
  public GameObject mulliganXlabelPrefab;
  public GameObject mulliganTimerPrefab;
  public GameObject heroLabelPrefab;
  public MulliganButton mulliganButtonWidget;
  public UberText conditionalHelperTextLabel;
  public bool mulliganRefreshButtonEnabled;
  [CustomEditField(Label = "Tag Conditional VFX Prefabs", SearchField = "m_requiredTag")]
  public List<MulliganManager.TagConditionalVFX> tagConditionalVFXPrefabs = new List<MulliganManager.TagConditionalVFX>();
  private const float PHONE_HEIGHT_OFFSET = 7f;
  private const float PHONE_CARD_Z_OFFSET = 0.2f;
  private const float PHONE_CARD_SCALE = 0.9f;
  private const float PHONE_ZONE_SIZE_ADJUST = 0.55f;
  private const string MULLIGAN_BUTTON_PREFAB = "MulliganButton.prefab:f58c065fc711b604c891cefd1faf722a";
  private const float REFRESH_BUTTON_X_OFFSET = 2f;
  public const float BATTLEGROUNDS_HERO_ENDING_POSITION_X = -7.7726f;
  public const float BATTLEGROUNDS_HERO_ENDING_POSITION_Y = 0.0055918f;
  public const float BATTLEGROUNDS_HERO_ENDING_POSITION_Z = -8.054f;
  public const float BATTLEGROUNDS_HERO_ENDING_SCALE = 1.134f;
  public static readonly PlatformDependentValue<Vector3> FRIENDLY_PLAYER_CARD_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(1.1f, 0.28f, 1.1f),
    Phone = new Vector3(0.9f, 0.28f, 0.9f)
  };
  private static MulliganManager s_instance;
  private bool mulliganActive;
  private MulliganTimer m_mulliganTimer;
  private NormalButton mulliganButton;
  private NormalButton m_refreshButton;
  private GameObject myWeldEffect;
  private GameObject hisWeldEffect;
  private GameObject coinObject;
  private GameObject startingHandZone;
  private GameObject coinTossText;
  private ZoneHand friendlySideHandZone;
  private ZoneHand opposingSideHandZone;
  private ZoneDeck friendlySideDeck;
  private ZoneDeck opposingSideDeck;
  private Actor myHeroCardActor;
  private Actor hisHeroCardActor;
  private Actor myHeroPowerCardActor;
  private Actor hisHeroPowerCardActor;
  private Map<Card, Actor> opponentHeroActors = new Map<Card, Actor>();
  private Map<Card, Actor> choiceHeroActors = new Map<Card, Actor>();
  private List<Actor> fakeCardsOnLeft = new List<Actor>();
  private List<Actor> fakeCardsOnRight = new List<Actor>();
  private bool waitingForVersusText;
  private GameStartVsLetters versusText;
  private bool waitingForVersusVo;
  private AudioSource versusVo;
  private bool introComplete;
  private bool skipCardChoosing;
  private List<Card> m_startingCards;
  private List<Card> m_startingOppCards;
  private int m_coinCardIndex = -1;
  private int m_bonusCardIndex = -1;
  private GameObject mulliganChooseBanner;
  private GameObject mulliganDetailLabel;
  private List<MulliganReplaceLabel> m_replaceLabels;
  private GameObject[] m_xLabels;
  private List<GameObject> m_tagConditionalVFXs;
  private GameObject m_overrideMulliganChooseBannerPrefab;
  private bool[] m_handCardsMarkedForReplace = new bool[4];
  private Vector3 coinLocation;
  private bool friendlyPlayerGoesFirst;
  private HeroLabel myheroLabel;
  private HeroLabel hisheroLabel;
  private Spell m_MyCustomSocketInSpell;
  private Spell m_HisCustomSocketInSpell;
  private bool m_isLoadingMyCustomSocketIn;
  private bool m_isLoadingHisCustomSocketIn;
  private int pendingHeroCount;
  private int pendingFakeHeroCount;
  public static readonly float ANIMATION_TIME_DEAL_CARD = 1.5f;
  private bool friendlyPlayerHasReplacementCards;
  private bool opponentPlayerHasReplacementCards;
  private bool m_waitingForUserInput;
  private Notification innkeeperMulliganDialog;
  private bool m_resuming;
  private Coroutine m_customIntroCoroutine;
  private IEnumerator m_DimLightsOnceBoardLoads;
  private IEnumerator m_WaitForBoardThenLoadButton;
  private IEnumerator m_WaitForHeroesAndStartAnimations;
  private IEnumerator m_ResumeMulligan;
  private IEnumerator m_DealStartingCards;
  private IEnumerator m_ShowMultiplayerWaitingArea;
  private IEnumerator m_RemoveOldCardsAnimation;
  private IEnumerator m_PlayStartingTaunts;
  private IEnumerator m_Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen;
  private IEnumerator m_ContinueMulliganWhenBoardLoads;
  private IEnumerator m_WaitAFrameBeforeSendingEventToMulliganButton;
  private IEnumerator m_WaitAFrameBeforeSendingEventToMulliganRefreshButton;
  private IEnumerator m_ShrinkStartingHandBanner;
  private IEnumerator m_AnimateCoinTossText;
  private IEnumerator m_UpdateChooseBanner;
  private IEnumerator m_RemoveUIButtons;
  private IEnumerator m_WaitForOpponentToFinishMulligan;
  private IEnumerator m_EndMulliganWithTiming;
  private IEnumerator m_HandleCoinCard;
  private IEnumerator m_EnableHandCollidersAfterCardsAreDealt;
  private IEnumerator m_SkipMulliganForResume;
  private IEnumerator m_SkipMulliganWhenIntroComplete;
  private IEnumerator m_WaitForBoardAnimToCompleteThenStartTurn;

  private void Awake() => MulliganManager.s_instance = this;

  private void OnDestroy()
  {
    if (GameState.Get() != null)
    {
      GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
      GameState.Get().UnregisterMulliganTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnMulliganTimerUpdate));
      GameState.Get().UnregisterEntitiesChosenReceivedListener(new GameState.EntitiesChosenReceivedCallback(this.OnEntitiesChosenReceived));
      GameState.Get().UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    }
    MulliganManager.s_instance = (MulliganManager) null;
  }

  private void Start()
  {
    if (GameState.Get() == null)
    {
      Debug.LogError((object) string.Format("MulliganManager.Start() - GameState already Shutdown before MulliganManager was loaded."));
    }
    else
    {
      if (GameState.Get().IsGameCreatedOrCreating())
        this.HandleGameStart();
      else
        GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
      GameState.Get().RegisterMulliganTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnMulliganTimerUpdate));
      GameState.Get().RegisterEntitiesChosenReceivedListener(new GameState.EntitiesChosenReceivedCallback(this.OnEntitiesChosenReceived));
      GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
      if (!(bool) UniversalInputManager.UsePhoneUI)
        return;
      this.myheroAnimatesToPosition = this.myheroAnimatesToPosition_iPhone;
      this.hisheroAnimatesToPosition = this.hisheroAnimatesToPosition_iPhone;
      this.cardAnimatesFromBoardToDeck = this.cardAnimatesFromBoardToDeck_iPhone;
    }
  }

  public static MulliganManager Get() => MulliganManager.s_instance;

  public bool IsCustomIntroActive() => this.m_customIntroCoroutine != null;

  public bool IsMulliganActive() => this.mulliganActive;

  public bool IsMulliganIntroActive() => !this.introComplete;

  private void EnableDamageCapFX(bool enable)
  {
    PlayerLeaderboardManager leaderboardManager = PlayerLeaderboardManager.Get();
    if (!((UnityEngine.Object) leaderboardManager != (UnityEngine.Object) null))
      return;
    leaderboardManager.EnableDamageCapFX(enable);
  }

  public void ForceMulliganActive(bool active)
  {
    this.mulliganActive = active;
    if (this.mulliganActive)
    {
      GameState.Get().HideZzzEffects();
      if (this.skipCardChoosing)
        return;
      this.EnableDamageCapFX(false);
    }
    else
    {
      GameState.Get().UnhideZzzEffects();
      this.EnableDamageCapFX(true);
    }
  }

  public void LoadMulliganButton()
  {
    if (this.m_WaitForBoardThenLoadButton != null)
      this.StopCoroutine(this.m_WaitForBoardThenLoadButton);
    this.m_WaitForBoardThenLoadButton = this.WaitForBoardThenLoadButton();
    this.StartCoroutine(this.m_WaitForBoardThenLoadButton);
  }

  private IEnumerator WaitForBoardThenLoadButton()
  {
    MulliganManager mulliganManager = this;
    while ((UnityEngine.Object) Gameplay.Get().GetBoardLayout() == (UnityEngine.Object) null)
      yield return (object) null;
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
      AssetLoader.Get().InstantiatePrefab((AssetReference) "MulliganButton.prefab:f58c065fc711b604c891cefd1faf722a", new PrefabCallback<GameObject>(mulliganManager.OnMulliganButtonLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
      AssetLoader.Get().InstantiatePrefab((AssetReference) "MulliganButton.prefab:f58c065fc711b604c891cefd1faf722a", new PrefabCallback<GameObject>(mulliganManager.OnMulliganRefreshButtonLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) mulliganManager.conditionalHelperTextLabel != (UnityEngine.Object) null)
      mulliganManager.conditionalHelperTextLabel.transform.position = Board.Get().FindBone("MulliganHelperTextPosition").position;
  }

  private void OnMulliganButtonLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("MulliganManager.OnMulliganButtonLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      this.mulliganButton = go.GetComponent<NormalButton>();
      if ((UnityEngine.Object) this.mulliganButton == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("MulliganManager.OnMulliganButtonLoaded() - ERROR \"{0}\" has no {1} component", (object) assetRef, (object) typeof (NormalButton)));
      }
      else
      {
        this.mulliganButton.SetText(GameStrings.Get("GLOBAL_CONFIRM"));
        this.mulliganButtonWidget.SetText(GameStrings.Get("GLOBAL_CONFIRM"));
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
        {
          this.mulliganButton.SetEnabled(false);
          this.mulliganButton.gameObject.SetActive(false);
          this.mulliganButtonWidget.SetEnabled(false);
        }
        else
          this.mulliganButtonWidget.gameObject.SetActive(false);
      }
    }
  }

  private void OnMulliganRefreshButtonLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("MulliganManager.OnMulliganRefreshButtonLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_refreshButton = go.GetComponent<NormalButton>();
      if ((UnityEngine.Object) this.m_refreshButton == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("MulliganManager.OnMulliganRefreshButtonLoaded() - ERROR \"{0}\" has no {1} component", (object) assetRef, (object) typeof (NormalButton)));
      }
      else
      {
        this.m_refreshButton.SetText(GameStrings.Get("GLOBAL_REFRESH"));
        this.UpdateBGRefreshButton();
      }
    }
  }

  public void OnFriendlyPlayerNumberRefreshAvailableChanged(int newValue) => this.UpdateBGRefreshButton(newValue);

  private void UpdateBGRefreshButton(int numRefreshesAvail = -1)
  {
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE) || !this.mulliganRefreshButtonEnabled)
    {
      this.m_refreshButton.gameObject.SetActive(false);
    }
    else
    {
      if (numRefreshesAvail == -1)
      {
        Player friendlySidePlayer = GameState.Get()?.GetFriendlySidePlayer();
        if (friendlySidePlayer != null)
          numRefreshesAvail = friendlySidePlayer.GetTag(GAME_TAG.BACON_NUMBER_HERO_REFRESH_AVAILABLE);
      }
      this.m_refreshButton.gameObject.SetActive(numRefreshesAvail > 0);
      if (numRefreshesAvail <= 0)
        return;
      this.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton = this.WaitAFrameBeforeSendingEventToMulliganButton(this.m_refreshButton);
      this.StartCoroutine(this.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton);
    }
  }

  private void OnVersusVoLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.waitingForVersusVo = false;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("MulliganManager.OnVersusVoLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      this.versusVo = go.GetComponent<AudioSource>();
      if (!((UnityEngine.Object) this.versusVo == (UnityEngine.Object) null))
        return;
      Debug.LogError((object) string.Format("MulliganManager.OnVersusVoLoaded() - ERROR \"{0}\" has no {1} component", (object) assetRef, (object) typeof (AudioSource)));
    }
  }

  private void OnVersusTextLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.waitingForVersusText = false;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("MulliganManager.OnVersusTextLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      this.versusText = go.GetComponent<GameStartVsLetters>();
      if (!((UnityEngine.Object) this.versusText == (UnityEngine.Object) null))
        return;
      Log.All.PrintError("MulliganManager.OnVersusTextLoaded() object loaded does not have a GameStartVsLetters component");
    }
  }

  private IEnumerator WaitForHeroesAndStartAnimations()
  {
    MulliganManager mulliganManager = this;
    Log.LoadingScreen.Print("MulliganManager.WaitForHeroesAndStartAnimations()");
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.SKIP_HERO_LOAD))
    {
      while (gameEntity.IsPreloadingAssets())
        yield return (object) null;
      gameEntity.NotifyOfMulliganInitialized();
      GameState.Get().GetGameEntity().DoAlternateMulliganIntro();
      mulliganManager.introComplete = true;
    }
    else
    {
      Player friendlyPlayer;
      for (friendlyPlayer = GameState.Get().GetFriendlySidePlayer(); friendlyPlayer == null; friendlyPlayer = GameState.Get().GetFriendlySidePlayer())
        yield return (object) null;
      Player opposingPlayer;
      for (opposingPlayer = GameState.Get().GetOpposingSidePlayer(); opposingPlayer == null; opposingPlayer = GameState.Get().GetOpposingSidePlayer())
        yield return (object) null;
      Card myHeroCard = (Card) null;
      while ((UnityEngine.Object) mulliganManager.myHeroCardActor == (UnityEngine.Object) null)
      {
        myHeroCard = friendlyPlayer.GetHeroCard();
        if ((UnityEngine.Object) myHeroCard != (UnityEngine.Object) null)
          mulliganManager.myHeroCardActor = myHeroCard.GetActor();
        yield return (object) null;
      }
      Card hisHeroCard = (Card) null;
      while ((UnityEngine.Object) mulliganManager.hisHeroCardActor == (UnityEngine.Object) null)
      {
        hisHeroCard = opposingPlayer.GetHeroCard();
        if ((UnityEngine.Object) hisHeroCard != (UnityEngine.Object) null)
          mulliganManager.hisHeroCardActor = hisHeroCard.GetActor();
        yield return (object) null;
      }
      while (friendlyPlayer.GetHeroPower() != null && (UnityEngine.Object) mulliganManager.myHeroPowerCardActor == (UnityEngine.Object) null)
      {
        Card heroPowerCard = friendlyPlayer.GetHeroPowerCard();
        if ((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null)
        {
          mulliganManager.myHeroPowerCardActor = heroPowerCard.GetActor();
          if ((UnityEngine.Object) mulliganManager.myHeroPowerCardActor != (UnityEngine.Object) null)
            mulliganManager.myHeroPowerCardActor.TurnOffCollider();
        }
        yield return (object) null;
      }
      while (opposingPlayer.GetHeroPower() != null && (UnityEngine.Object) mulliganManager.hisHeroPowerCardActor == (UnityEngine.Object) null)
      {
        Card heroPowerCard = opposingPlayer.GetHeroPowerCard();
        if ((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null)
        {
          mulliganManager.hisHeroPowerCardActor = heroPowerCard.GetActor();
          if ((UnityEngine.Object) mulliganManager.hisHeroPowerCardActor != (UnityEngine.Object) null)
            mulliganManager.hisHeroPowerCardActor.TurnOffCollider();
        }
        yield return (object) null;
      }
      while (GameState.Get() == null || GameState.Get().GetGameEntity().IsPreloadingAssets())
        yield return (object) null;
      while (!mulliganManager.myHeroCardActor.HasCardDef)
        yield return (object) null;
      while (!mulliganManager.hisHeroCardActor.HasCardDef)
        yield return (object) null;
      mulliganManager.LoadMyHeroSkinSocketInEffect(mulliganManager.myHeroCardActor);
      mulliganManager.LoadHisHeroSkinSocketInEffect(mulliganManager.hisHeroCardActor);
      while (mulliganManager.m_isLoadingMyCustomSocketIn || mulliganManager.m_isLoadingHisCustomSocketIn)
        yield return (object) null;
      List<Material> materials1 = RendererExtension.GetMaterials(mulliganManager.myHeroCardActor.m_portraitMesh.GetComponent<Renderer>());
      Material myHeroMat = materials1[mulliganManager.myHeroCardActor.m_portraitMatIdx];
      CustomHeroFrameBehaviour component = mulliganManager.myHeroCardActor.gameObject.GetComponent<CustomHeroFrameBehaviour>();
      Material myHeroFrameMat;
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        List<Material> materials2 = RendererExtension.GetMaterials(component.GetMeshObject().GetComponentInChildren<Renderer>());
        myHeroFrameMat = materials2.Count > 0 ? materials2[0] : (Material) null;
      }
      else
        myHeroFrameMat = materials1[mulliganManager.myHeroCardActor.m_portraitFrameMatIdx];
      if ((UnityEngine.Object) myHeroMat != (UnityEngine.Object) null && myHeroMat.HasProperty("_LightingBlend"))
        myHeroMat.SetFloat("_LightingBlend", 0.0f);
      if ((UnityEngine.Object) myHeroFrameMat != (UnityEngine.Object) null && myHeroFrameMat.HasProperty("_LightingBlend"))
        myHeroFrameMat.SetFloat("_LightingBlend", 0.0f);
      float num = GameState.Get().GetBooleanGameOption(GameEntityOption.DIM_OPPOSING_HERO_DURING_MULLIGAN) ? 1f : 0.0f;
      List<Material> materials3 = RendererExtension.GetMaterials(mulliganManager.hisHeroCardActor.m_portraitMesh.GetComponent<Renderer>());
      Material hisHeroMat = materials3[mulliganManager.hisHeroCardActor.m_portraitMatIdx];
      Material hisHeroFrameMat = materials3[mulliganManager.hisHeroCardActor.m_portraitFrameMatIdx];
      if ((UnityEngine.Object) hisHeroMat != (UnityEngine.Object) null && hisHeroMat.HasProperty("_LightingBlend"))
        hisHeroMat.SetFloat("_LightingBlend", num);
      if ((UnityEngine.Object) hisHeroFrameMat != (UnityEngine.Object) null && hisHeroFrameMat.HasProperty("_LightingBlend"))
        hisHeroFrameMat.SetFloat("_LightingBlend", num);
      if ((UnityEngine.Object) mulliganManager.myHeroPowerCardActor != (UnityEngine.Object) null && (UnityEngine.Object) mulliganManager.myHeroPowerCardActor.m_portraitMesh != (UnityEngine.Object) null)
      {
        List<Material> materials4 = RendererExtension.GetMaterials(mulliganManager.myHeroPowerCardActor.m_portraitMesh.GetComponent<Renderer>());
        Material material1 = materials4[mulliganManager.myHeroPowerCardActor.m_portraitMatIdx];
        if ((UnityEngine.Object) material1 != (UnityEngine.Object) null && material1.HasProperty("_LightingBlend"))
          material1.SetFloat("_LightingBlend", 1f);
        Material material2 = materials4[mulliganManager.myHeroPowerCardActor.m_portraitFrameMatIdx];
        if ((UnityEngine.Object) material2 != (UnityEngine.Object) null && material2.HasProperty("_LightingBlend"))
          material2.SetFloat("_LightingBlend", 1f);
      }
      if ((UnityEngine.Object) mulliganManager.hisHeroPowerCardActor != (UnityEngine.Object) null && (UnityEngine.Object) mulliganManager.hisHeroPowerCardActor.m_portraitMesh != (UnityEngine.Object) null)
      {
        List<Material> materials5 = RendererExtension.GetMaterials(mulliganManager.hisHeroPowerCardActor.m_portraitMesh.GetComponent<Renderer>());
        Material material3 = materials5[mulliganManager.hisHeroPowerCardActor.m_portraitMatIdx];
        if ((UnityEngine.Object) material3 != (UnityEngine.Object) null && material3.HasProperty("_LightingBlend"))
          material3.SetFloat("_LightingBlend", 1f);
        Material material4 = materials5[mulliganManager.hisHeroPowerCardActor.m_portraitFrameMatIdx];
        if ((UnityEngine.Object) material4 != (UnityEngine.Object) null && material4.HasProperty("_LightingBlend"))
          material4.SetFloat("_LightingBlend", 1f);
      }
      mulliganManager.myHeroCardActor.TurnOffCollider();
      mulliganManager.hisHeroCardActor.TurnOffCollider();
      gameEntity.NotifyOfMulliganInitialized();
      if (GameState.Get().GetGameEntity().DoAlternateMulliganIntro())
      {
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
          mulliganManager.myHeroCardActor.Hide();
        mulliganManager.introComplete = true;
      }
      else
      {
        while (mulliganManager.waitingForVersusText || mulliganManager.waitingForVersusVo)
          yield return (object) null;
        Log.LoadingScreen.Print("MulliganManager.WaitForHeroesAndStartAnimations() - NotifySceneLoaded()");
        SceneMgr.Get().NotifySceneLoaded();
        while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
          yield return (object) null;
        GameMgr.Get().UpdatePresence();
        GameObject myHero = mulliganManager.myHeroCardActor.gameObject;
        GameObject hisHero = mulliganManager.hisHeroCardActor.gameObject;
        mulliganManager.myHeroCardActor.GetHealthObject().Hide();
        mulliganManager.hisHeroCardActor.GetHealthObject().Hide();
        if ((UnityEngine.Object) mulliganManager.myHeroCardActor.GetAttackObject() != (UnityEngine.Object) null)
          mulliganManager.myHeroCardActor.GetAttackObject().Hide();
        if ((UnityEngine.Object) mulliganManager.hisHeroCardActor.GetAttackObject() != (UnityEngine.Object) null)
          mulliganManager.hisHeroCardActor.GetAttackObject().Hide();
        if ((bool) (UnityEngine.Object) mulliganManager.versusText)
          mulliganManager.versusText.transform.position = Board.Get().FindBone("VS_Position").position;
        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.heroLabelPrefab);
        mulliganManager.myheroLabel = gameObject1.GetComponent<HeroLabel>();
        mulliganManager.myheroLabel.transform.parent = mulliganManager.myHeroCardActor.GetMeshRenderer().transform;
        mulliganManager.myheroLabel.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        TAG_CLASS tag1 = mulliganManager.myHeroCardActor.GetEntity().GetClass();
        string classText1 = "";
        if (tag1 != TAG_CLASS.NEUTRAL && gameEntity.ShouldShowHeroClassDuringMulligan(Player.Side.FRIENDLY))
          classText1 = GameStrings.GetClassName(tag1).ToUpper();
        mulliganManager.myheroLabel.UpdateText(mulliganManager.myHeroCardActor.GetEntity().GetName(), classText1);
        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.heroLabelPrefab);
        mulliganManager.hisheroLabel = gameObject2.GetComponent<HeroLabel>();
        mulliganManager.hisheroLabel.transform.parent = mulliganManager.hisHeroCardActor.GetMeshRenderer().transform;
        mulliganManager.hisheroLabel.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        TAG_CLASS tag2 = mulliganManager.hisHeroCardActor.GetEntity().GetClass();
        string classText2 = "";
        if (tag2 != TAG_CLASS.NEUTRAL && gameEntity.ShouldShowHeroClassDuringMulligan(Player.Side.OPPOSING))
          classText2 = GameStrings.GetClassName(tag2).ToUpper();
        mulliganManager.hisheroLabel.UpdateText(mulliganManager.hisHeroCardActor.GetEntity().GetName(), classText2);
        if (!GameState.Get().WasConcedeRequested())
        {
          gameEntity.StartMulliganSoundtracks(false);
          Animation cardAnim = myHero.GetComponent<Animation>();
          if ((UnityEngine.Object) cardAnim == (UnityEngine.Object) null)
            cardAnim = myHero.AddComponent<Animation>();
          cardAnim.AddClip(mulliganManager.hisheroAnimatesToPosition, "hisHeroAnimateToPosition");
          mulliganManager.StartCoroutine(mulliganManager.SampleAnimFrame(cardAnim, "hisHeroAnimateToPosition", 0.0f));
          Animation oppCardAnim = hisHero.GetComponent<Animation>();
          if ((UnityEngine.Object) oppCardAnim == (UnityEngine.Object) null)
            oppCardAnim = hisHero.AddComponent<Animation>();
          oppCardAnim.AddClip(mulliganManager.myheroAnimatesToPosition, "myHeroAnimateToPosition");
          mulliganManager.StartCoroutine(mulliganManager.SampleAnimFrame(oppCardAnim, "myHeroAnimateToPosition", 0.0f));
          mulliganManager.m_customIntroCoroutine = mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoCustomIntro(myHeroCard, hisHeroCard, mulliganManager.myheroLabel, mulliganManager.hisheroLabel, mulliganManager.versusText));
          yield return (object) mulliganManager.m_customIntroCoroutine;
          mulliganManager.m_customIntroCoroutine = (Coroutine) null;
          while (LoadingScreen.Get().IsTransitioning())
            yield return (object) null;
          AudioSource myHeroLine = gameEntity.GetAnnouncerLine(myHeroCard, Card.AnnouncerLineType.BEFORE_VERSUS);
          AudioSource hisHeroLine = gameEntity.GetAnnouncerLine(hisHeroCard, Card.AnnouncerLineType.AFTER_VERSUS);
          if ((bool) (UnityEngine.Object) mulliganManager.versusVo && (bool) (UnityEngine.Object) myHeroLine && (bool) (UnityEngine.Object) hisHeroLine)
          {
            if ((UnityEngine.Object) myHeroCard != (UnityEngine.Object) null)
              myHeroCard.ActivateLegendaryHeroAnimEvent("OnFriendlyAnnounceVO");
            SoundManager.Get().Play(myHeroLine);
            while (SoundManager.Get().IsActive(myHeroLine) && !SoundManager.Get().IsPlaybackFinished(myHeroLine))
              yield return (object) null;
            yield return (object) new WaitForSeconds(0.05f);
            SoundManager.Get().PlayPreloaded(mulliganManager.versusVo);
            while (SoundManager.Get().IsActive(mulliganManager.versusVo) && !SoundManager.Get().IsPlaybackFinished(mulliganManager.versusVo))
              yield return (object) null;
            yield return (object) new WaitForSeconds(0.05f);
            if ((UnityEngine.Object) hisHeroCard != (UnityEngine.Object) null)
              hisHeroCard.ActivateLegendaryHeroAnimEvent("OnOpponentAnnounceVO");
            if ((UnityEngine.Object) hisHeroLine != (UnityEngine.Object) null && (UnityEngine.Object) hisHeroLine.clip != (UnityEngine.Object) null)
            {
              SoundManager.Get().Play(hisHeroLine);
              while (SoundManager.Get().IsActive(hisHeroLine) && !SoundManager.Get().IsPlaybackFinished(hisHeroLine))
                yield return (object) null;
            }
          }
          else
            yield return (object) new WaitForSeconds(0.6f);
          yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().PlayMissionIntroLineAndWait());
          mulliganManager.myheroLabel.transform.parent = (Transform) null;
          mulliganManager.hisheroLabel.transform.parent = (Transform) null;
          mulliganManager.myheroLabel.FadeOut();
          mulliganManager.hisheroLabel.FadeOut();
          yield return (object) new WaitForSeconds(0.5f);
          if ((UnityEngine.Object) mulliganManager.m_MyCustomSocketInSpell != (UnityEngine.Object) null)
          {
            mulliganManager.m_MyCustomSocketInSpell.m_Location = SpellLocation.NONE;
            mulliganManager.m_MyCustomSocketInSpell.gameObject.SetActive(true);
            if (mulliganManager.myHeroCardActor.SocketInParentEffectToHero)
            {
              Vector3 localScale = mulliganManager.myHeroCardActor.transform.localScale;
              mulliganManager.myHeroCardActor.transform.localScale = Vector3.one;
              mulliganManager.m_MyCustomSocketInSpell.transform.parent = mulliganManager.myHeroCardActor.transform;
              mulliganManager.m_MyCustomSocketInSpell.transform.localPosition = Vector3.zero;
              mulliganManager.myHeroCardActor.transform.localScale = localScale;
            }
            mulliganManager.m_MyCustomSocketInSpell.SetSource(mulliganManager.myHeroCardActor.GetCard().gameObject);
            mulliganManager.m_MyCustomSocketInSpell.RemoveAllTargets();
            GameObject myHeroSocketBone = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).gameObject;
            mulliganManager.m_MyCustomSocketInSpell.AddTarget(myHeroSocketBone);
            mulliganManager.m_MyCustomSocketInSpell.ActivateState(SpellStateType.BIRTH);
            if ((UnityEngine.Object) myHeroCard != (UnityEngine.Object) null)
              myHeroCard.ActivateLegendaryHeroAnimEvent("OnFriendlySocketIn");
            mulliganManager.m_MyCustomSocketInSpell.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
            {
              this.myHeroCardActor.transform.position = myHeroSocketBone.transform.position;
              this.myHeroCardActor.transform.localScale = Vector3.one;
            }));
            if (!mulliganManager.myHeroCardActor.SocketInOverrideHeroAnimation)
              cardAnim.Play("hisHeroAnimateToPosition");
          }
          else
            cardAnim.Play("hisHeroAnimateToPosition");
          if ((UnityEngine.Object) mulliganManager.m_HisCustomSocketInSpell != (UnityEngine.Object) null)
          {
            if ((bool) (UnityEngine.Object) mulliganManager.m_MyCustomSocketInSpell)
              SoundUtils.SetSourceVolumes((Component) mulliganManager.m_HisCustomSocketInSpell, 0.0f);
            mulliganManager.m_HisCustomSocketInSpell.m_Location = SpellLocation.NONE;
            if (mulliganManager.hisHeroCardActor.SocketInOverrideHeroAnimation)
              yield return (object) new WaitForSeconds(0.25f);
            mulliganManager.m_HisCustomSocketInSpell.gameObject.SetActive(true);
            if (mulliganManager.hisHeroCardActor.SocketInParentEffectToHero)
            {
              Vector3 localScale = mulliganManager.hisHeroCardActor.transform.localScale;
              mulliganManager.hisHeroCardActor.transform.localScale = Vector3.one;
              mulliganManager.m_HisCustomSocketInSpell.transform.parent = mulliganManager.hisHeroCardActor.transform;
              mulliganManager.m_HisCustomSocketInSpell.transform.localPosition = Vector3.zero;
              mulliganManager.hisHeroCardActor.transform.localScale = localScale;
            }
            mulliganManager.m_HisCustomSocketInSpell.SetSource(mulliganManager.hisHeroCardActor.GetCard().gameObject);
            mulliganManager.m_HisCustomSocketInSpell.RemoveAllTargets();
            GameObject hisHeroSocketBone = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).gameObject;
            mulliganManager.m_HisCustomSocketInSpell.AddTarget(hisHeroSocketBone);
            mulliganManager.m_HisCustomSocketInSpell.ActivateState(SpellStateType.BIRTH);
            if ((UnityEngine.Object) hisHeroCard != (UnityEngine.Object) null)
              hisHeroCard.ActivateLegendaryHeroAnimEvent("OnOpponentSocketIn");
            mulliganManager.m_HisCustomSocketInSpell.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
            {
              this.hisHeroCardActor.transform.position = hisHeroSocketBone.transform.position;
              this.hisHeroCardActor.transform.localScale = Vector3.one;
            }));
            if (!mulliganManager.hisHeroCardActor.SocketInOverrideHeroAnimation)
              oppCardAnim.Play("myHeroAnimateToPosition");
          }
          else
            oppCardAnim.Play("myHeroAnimateToPosition");
          SoundManager.Get().LoadAndPlay((AssetReference) "FX_MulliganCoin01_HeroCoinDrop.prefab:c46488739eda9f94eb0160290e35f321", mulliganManager.hisHeroCardActor.GetCard().gameObject);
          if ((bool) (UnityEngine.Object) mulliganManager.versusText)
          {
            yield return (object) new WaitForSeconds(0.1f);
            mulliganManager.versusText.FadeOut();
            yield return (object) new WaitForSeconds(0.32f);
          }
          if ((UnityEngine.Object) mulliganManager.m_MyCustomSocketInSpell == (UnityEngine.Object) null)
          {
            mulliganManager.myWeldEffect = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.weldPrefab);
            mulliganManager.myWeldEffect.transform.position = myHero.transform.position;
            if ((bool) (UnityEngine.Object) mulliganManager.m_HisCustomSocketInSpell)
              SoundUtils.SetSourceVolumes(mulliganManager.myWeldEffect, 0.0f);
            mulliganManager.myWeldEffect.GetComponent<HeroWeld>().DoAnim();
          }
          if ((UnityEngine.Object) mulliganManager.m_HisCustomSocketInSpell == (UnityEngine.Object) null)
          {
            mulliganManager.hisWeldEffect = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.weldPrefab);
            mulliganManager.hisWeldEffect.transform.position = hisHero.transform.position;
            if ((bool) (UnityEngine.Object) mulliganManager.m_MyCustomSocketInSpell)
              SoundUtils.SetSourceVolumes(mulliganManager.hisWeldEffect, 0.0f);
            mulliganManager.hisWeldEffect.GetComponent<HeroWeld>().DoAnim();
          }
          yield return (object) new WaitForSeconds(0.05f);
          iTween.ShakePosition(Camera.main.gameObject, iTween.Hash((object) "time", (object) 0.6f, (object) "amount", (object) new Vector3(0.03f, 0.01f, 0.03f)));
          Action<object> action1 = (Action<object>) (amount =>
          {
            if ((UnityEngine.Object) myHeroMat != (UnityEngine.Object) null)
              myHeroMat.SetFloat("_LightingBlend", (float) amount);
            if (!((UnityEngine.Object) myHeroFrameMat != (UnityEngine.Object) null))
              return;
            myHeroFrameMat.SetFloat("_LightingBlend", (float) amount);
          });
          action1((object) 0.0f);
          Hashtable args1 = iTween.Hash((object) "time", (object) 1f, (object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "delay", (object) 2f, (object) "onupdate", (object) action1, (object) "onupdatetarget", (object) mulliganManager.gameObject, (object) "name", (object) "MyHeroLightBlend");
          iTween.ValueTo(mulliganManager.gameObject, args1);
          Action<object> action2 = (Action<object>) (amount =>
          {
            if ((UnityEngine.Object) hisHeroMat != (UnityEngine.Object) null)
              hisHeroMat.SetFloat("_LightingBlend", (float) amount);
            if (!((UnityEngine.Object) hisHeroFrameMat != (UnityEngine.Object) null))
              return;
            hisHeroFrameMat.SetFloat("_LightingBlend", (float) amount);
          });
          action2((object) 0.0f);
          Hashtable args2 = iTween.Hash((object) "time", (object) 1f, (object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "delay", (object) 2f, (object) "onupdate", (object) action2, (object) "onupdatetarget", (object) mulliganManager.gameObject, (object) "name", (object) "HisHeroLightBlend");
          iTween.ValueTo(mulliganManager.gameObject, args2);
          yield return (object) GameState.Get().GetGameEntity().DoGameSpecificPostIntroActions();
          mulliganManager.introComplete = true;
          GameState.Get().GetGameEntity().NotifyOfHeroesFinishedAnimatingInMulligan();
          ScreenEffectsMgr.Get().SetActive(true);
        }
      }
    }
  }

  public void BeginMulligan()
  {
    bool mulliganActive = this.mulliganActive;
    this.ForceMulliganActive(true);
    if (GameState.Get().WasConcedeRequested())
    {
      this.HandleGameOverDuringMulligan();
    }
    else
    {
      if (mulliganActive && SpectatorManager.Get().IsSpectatingOpposingSide())
        return;
      this.m_ContinueMulliganWhenBoardLoads = this.ContinueMulliganWhenBoardLoads();
      this.StartCoroutine(this.m_ContinueMulliganWhenBoardLoads);
    }
  }

  private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
  {
    GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
    this.HandleGameStart();
  }

  private void HandleGameStart()
  {
    Log.LoadingScreen.Print("MulliganManager.HandleGameStart() - IsPastBeginPhase()={0}", (object) GameState.Get().IsPastBeginPhase());
    bool flag = GameMgr.Get().IsSpectator() && GameState.Get().GetGameEntity().HasTag(GAME_TAG.PUZZLE_MODE);
    if (GameState.Get().IsPastBeginPhase() | flag)
    {
      this.m_SkipMulliganForResume = this.SkipMulliganForResume();
      this.StartCoroutine(this.m_SkipMulliganForResume);
    }
    else
    {
      this.InitZones();
      this.m_DimLightsOnceBoardLoads = this.DimLightsOnceBoardLoads();
      this.StartCoroutine(this.m_DimLightsOnceBoardLoads);
      if (!GameState.Get().GetGameEntity().ShouldDoAlternateMulliganIntro())
      {
        this.m_xLabels = new GameObject[4];
        this.coinObject = UnityEngine.Object.Instantiate<GameObject>(this.coinPrefab);
        this.coinObject.SetActive(false);
        if (!Cheats.Get().ShouldSkipMulligan())
        {
          if (Cheats.Get().IsLaunchingQuickGame())
            TimeScaleMgr.Get().SetTimeScaleMultiplier(SceneDebugger.GetDevTimescaleMultiplier());
          this.waitingForVersusVo = true;
          SoundLoader.LoadSound((AssetReference) "VO_ANNOUNCER_VERSUS_21.prefab:acc34acb15f07ff4ba08025a57a9a458", new PrefabCallback<GameObject>(this.OnVersusVoLoaded));
        }
        this.waitingForVersusText = true;
        AssetLoader.Get().InstantiatePrefab((AssetReference) "GameStart_VS_Letters.prefab:3cb2cbed6d44a694eb23fb8791684003", new PrefabCallback<GameObject>(this.OnVersusTextLoaded));
        if (this.m_WaitForBoardThenLoadButton != null)
          this.StopCoroutine(this.m_WaitForBoardThenLoadButton);
        this.m_WaitForBoardThenLoadButton = this.WaitForBoardThenLoadButton();
        this.StartCoroutine(this.m_WaitForBoardThenLoadButton);
      }
      else
      {
        this.waitingForVersusVo = true;
        SoundLoader.LoadSound((AssetReference) "VO_ANNOUNCER_VERSUS_21.prefab:acc34acb15f07ff4ba08025a57a9a458", new PrefabCallback<GameObject>(this.OnVersusVoLoaded));
        this.waitingForVersusText = true;
        AssetLoader.Get().InstantiatePrefab((AssetReference) "GameStart_VS_Letters.prefab:3cb2cbed6d44a694eb23fb8791684003", new PrefabCallback<GameObject>(this.OnVersusTextLoaded));
      }
      this.m_WaitForHeroesAndStartAnimations = this.WaitForHeroesAndStartAnimations();
      this.StartCoroutine(this.m_WaitForHeroesAndStartAnimations);
      Log.LoadingScreen.Print("MulliganManager.HandleGameStart() - IsMulliganPhase()={0}", (object) GameState.Get().IsMulliganPhase());
      if (!GameState.Get().IsMulliganPhase())
        return;
      this.m_ResumeMulligan = this.ResumeMulligan();
      this.StartCoroutine(this.m_ResumeMulligan);
    }
  }

  private IEnumerator DimLightsOnceBoardLoads()
  {
    while ((UnityEngine.Object) Board.Get() == (UnityEngine.Object) null)
      yield return (object) null;
    Board.Get().SetMulliganLighting();
  }

  private IEnumerator ResumeMulligan()
  {
    this.m_resuming = true;
    foreach (Player player in GameState.Get().GetPlayerMap().Values)
    {
      if (player.GetTag<TAG_MULLIGAN>(GAME_TAG.MULLIGAN_STATE) == TAG_MULLIGAN.DONE)
      {
        if (player.IsFriendlySide())
          this.friendlyPlayerHasReplacementCards = true;
        else
          this.opponentPlayerHasReplacementCards = true;
      }
    }
    if (this.friendlyPlayerHasReplacementCards)
    {
      this.SkipCardChoosing();
    }
    else
    {
      while (GameState.Get().GetResponseMode() != GameState.ResponseMode.CHOICE)
        yield return (object) null;
    }
    this.BeginMulligan();
  }

  private void OnMulliganTimerUpdate(TurnTimerUpdate update, object userData)
  {
    if ((double) update.GetSecondsRemaining() > (double) Mathf.Epsilon)
    {
      if (update.ShouldShow())
        this.BeginMulliganCountdown(update.GetEndTimestamp());
      else
        this.StopMulliganCountdown();
    }
    else
    {
      GameState.Get().UnregisterMulliganTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnMulliganTimerUpdate));
      this.AutomaticContinueMulligan();
    }
  }

  private bool OnEntitiesChosenReceived(Network.EntitiesChosen chosen, object userData)
  {
    if (!GameMgr.Get().IsSpectator() || chosen.PlayerId != GameState.Get().GetFriendlyPlayerId())
      return false;
    this.m_Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen = this.Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen(chosen);
    this.StartCoroutine(this.m_Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen);
    return true;
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData) => this.HandleGameOverDuringMulligan();

  private IEnumerator Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen(
    Network.EntitiesChosen chosen)
  {
    while (!this.m_waitingForUserInput)
    {
      if (GameState.Get().IsGameOver() || this.skipCardChoosing)
        yield break;
      else
        yield return (object) null;
    }
    for (int index = 0; index < this.m_startingCards.Count; ++index)
    {
      bool flag = !chosen.Entities.Contains(this.m_startingCards[index].GetEntity().GetEntityId());
      if (this.m_handCardsMarkedForReplace[index] != flag)
        this.ToggleHoldState(index);
    }
    GameState.Get().OnEntitiesChosenProcessed(chosen);
    this.BeginDealNewCards();
  }

  private IEnumerator ContinueMulliganWhenBoardLoads()
  {
    MulliganManager mulliganManager = this;
    while ((UnityEngine.Object) ZoneMgr.Get() == (UnityEngine.Object) null)
      yield return (object) null;
    Board board = Board.Get();
    mulliganManager.startingHandZone = board.FindBone("StartingHandZone").gameObject;
    mulliganManager.InitZones();
    if (mulliganManager.m_resuming)
    {
      while (mulliganManager.ShouldWaitForMulliganCardsToBeProcessed())
        yield return (object) null;
    }
    mulliganManager.SortHand((Zone) mulliganManager.friendlySideHandZone);
    mulliganManager.SortHand((Zone) mulliganManager.opposingSideHandZone);
    board.CombinedSurface();
    board.FindCollider("DragPlane").enabled = false;
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
    {
      mulliganManager.m_ShowMultiplayerWaitingArea = mulliganManager.ShowMultiplayerWaitingArea();
      mulliganManager.StartCoroutine(mulliganManager.m_ShowMultiplayerWaitingArea);
    }
    else
    {
      mulliganManager.m_DealStartingCards = mulliganManager.DealStartingCards();
      mulliganManager.StartCoroutine(mulliganManager.m_DealStartingCards);
    }
  }

  private void InitZones()
  {
    foreach (Zone zone in ZoneMgr.Get().GetZones())
    {
      if (zone is ZoneHand)
      {
        if (zone.m_Side == Player.Side.FRIENDLY)
          this.friendlySideHandZone = (ZoneHand) zone;
        else
          this.opposingSideHandZone = (ZoneHand) zone;
      }
      if (zone is ZoneDeck)
      {
        if (zone.m_Side == Player.Side.FRIENDLY)
        {
          this.friendlySideDeck = (ZoneDeck) zone;
          this.friendlySideDeck.SetSuppressEmotes(true);
          this.friendlySideDeck.UpdateLayout();
        }
        else
        {
          this.opposingSideDeck = (ZoneDeck) zone;
          this.opposingSideDeck.SetSuppressEmotes(true);
          this.opposingSideDeck.UpdateLayout();
        }
      }
    }
  }

  private bool ShouldWaitForMulliganCardsToBeProcessed()
  {
    PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
    bool receivedEndOfMulligan = false;
    powerProcessor.ForEachTaskList((Action<int, PowerTaskList>) ((index, taskList) =>
    {
      if (!this.IsTaskListPuttingUsPastMulligan(taskList))
        return;
      receivedEndOfMulligan = true;
    }));
    return !receivedEndOfMulligan && powerProcessor.HasTaskLists();
  }

  private bool IsTaskListPuttingUsPastMulligan(PowerTaskList taskList)
  {
    foreach (PowerTask task in taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = power as Network.HistTagChange;
        if (histTagChange.Tag == 198 && GameUtils.IsPastBeginPhase((TAG_STEP) histTagChange.Value))
          return true;
      }
    }
    return false;
  }

  private void GetStartingLists()
  {
    List<Card> cards1 = this.friendlySideHandZone.GetCards();
    List<Card> cards2 = this.opposingSideHandZone.GetCards();
    int num;
    if (this.ShouldHandleCoinCard())
    {
      if (this.friendlyPlayerGoesFirst)
      {
        num = cards1.Count;
        this.m_bonusCardIndex = cards2.Count - 2;
        this.m_coinCardIndex = cards2.Count - 1;
      }
      else
      {
        num = cards1.Count - 1;
        this.m_bonusCardIndex = cards1.Count - 2;
      }
    }
    else
    {
      num = cards1.Count;
      this.m_bonusCardIndex = !this.friendlyPlayerGoesFirst ? cards1.Count - 1 : cards2.Count - 1;
    }
    this.m_startingCards = new List<Card>();
    for (int index = 0; index < num; ++index)
      this.m_startingCards.Add(cards1[index]);
    this.m_startingOppCards = new List<Card>();
    for (int index = 0; index < cards2.Count; ++index)
      this.m_startingOppCards.Add(cards2[index]);
  }

  private IEnumerator PlayStartingTaunts() => EmoteHandler.Get().PlayStartingTaunts(this.gameObject);

  private void SetupCardActor(ref List<Card> cards)
  {
    bool flag = false;
    foreach (Card callbackData in cards)
    {
      if ((UnityEngine.Object) callbackData != (UnityEngine.Object) null && (UnityEngine.Object) callbackData.GetActor() != (UnityEngine.Object) null)
      {
        callbackData.GetActor().SetActorState(ActorStateType.CARD_IDLE);
        callbackData.GetActor().TurnOffCollider();
        callbackData.GetActor().GetMeshRenderer().gameObject.layer = 8;
        if ((UnityEngine.Object) callbackData.GetActor().m_nameTextMesh != (UnityEngine.Object) null)
          callbackData.GetActor().m_nameTextMesh.UpdateNow();
      }
      else if ((UnityEngine.Object) callbackData == (UnityEngine.Object) null)
        flag = true;
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS) && (UnityEngine.Object) callbackData != (UnityEngine.Object) null && (UnityEngine.Object) callbackData.GetActor() != (UnityEngine.Object) null)
      {
        ++this.pendingHeroCount;
        callbackData.GetActor().gameObject.SetActive(false);
        AssetLoader.Get().InstantiatePrefab((AssetReference) GameState.Get().GetStringGameOption(GameEntityOption.ALTERNATE_MULLIGAN_ACTOR_NAME), new PrefabCallback<GameObject>(this.OnHeroActorLoaded), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
      }
    }
    if (!flag)
      return;
    string str = "SetupCardActor - Found a null card within starting hero cards during initialization. Starting Cards: ";
    for (int index = 0; index < this.m_startingCards.Count; ++index)
      str = str + ((UnityEngine.Object) this.m_startingCards[index] == (UnityEngine.Object) null ? "NULL" : this.m_startingCards[index].GetEntity().GetName()) + (index == this.m_startingCards.Count - 1 ? "." : ", ");
    TelemetryManager.Client().SendLiveIssue("Gameplay_MulliganManager", str);
    Log.MulliganManager.PrintWarning(str);
  }

  private void SetupCardCollider(ref List<Card> cards)
  {
    bool flag = false;
    foreach (Card key in cards)
    {
      if ((UnityEngine.Object) key != (UnityEngine.Object) null)
      {
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS) && this.choiceHeroActors.ContainsKey(key))
          this.choiceHeroActors[key].TurnOnCollider();
        else
          key.GetActor().TurnOnCollider();
      }
      else
        flag = true;
    }
    if (!flag)
      return;
    string str = "SetupCardCollider - Found a null card in starting cards while enabling colliders. Starting cards: ";
    for (int index = 0; index < this.m_startingCards.Count; ++index)
      str = str + ((UnityEngine.Object) this.m_startingCards[index] == (UnityEngine.Object) null ? "NULL" : this.m_startingCards[index].GetEntity().GetName()) + (index == this.m_startingCards.Count - 1 ? "." : ", ");
    TelemetryManager.Client().SendLiveIssue("Gameplay_MulliganManager", str);
    Log.MulliganManager.PrintWarning(str);
  }

  private IEnumerator ShowMultiplayerWaitingArea()
  {
    MulliganManager mulliganManager = this;
    yield return (object) new WaitForSeconds(1f);
    while (!mulliganManager.introComplete)
      yield return (object) null;
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsAfterIntroBeforeMulligan());
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.DO_OPENING_TAUNTS) && !Cheats.Get().ShouldSkipMulligan())
    {
      mulliganManager.m_PlayStartingTaunts = mulliganManager.PlayStartingTaunts();
      mulliganManager.StartCoroutine(mulliganManager.m_PlayStartingTaunts);
    }
    while (ZoneMgr.Get().HasPendingServerChange() || ZoneMgr.Get().HasActiveServerChange())
      yield return (object) null;
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    mulliganManager.friendlyPlayerGoesFirst = friendlySidePlayer.HasTag(GAME_TAG.FIRST_PLAYER);
    mulliganManager.GetStartingLists();
    bool isMulliganOver = false;
    bool shouldSendTelemetry = true;
    if (mulliganManager.m_startingCards.Count == 0)
    {
      while ((UnityEngine.Object) GameState.Get().GetFriendlySidePlayer().GetHeroCard() == (UnityEngine.Object) null)
      {
        if (shouldSendTelemetry)
        {
          TelemetryManager.Client().SendLiveIssue("Gameplay_MulliganManager", "No hero card set for friendly side player");
          shouldSendTelemetry = false;
        }
        yield return (object) null;
      }
      mulliganManager.m_startingCards.Add(GameState.Get().GetFriendlySidePlayer().GetHeroCard());
      isMulliganOver = true;
    }
    shouldSendTelemetry = false;
    foreach (Card startingCard in mulliganManager.m_startingCards)
    {
      if ((UnityEngine.Object) startingCard != (UnityEngine.Object) null && startingCard.IsActorLoading())
        yield return (object) null;
    }
    mulliganManager.SetupCardActor(ref mulliganManager.m_startingCards);
    while (mulliganManager.pendingHeroCount > 0)
      yield return (object) null;
    float zoneWidth = mulliganManager.startingHandZone.GetComponent<Collider>().bounds.size.x;
    if ((bool) UniversalInputManager.UsePhoneUI)
      zoneWidth *= 0.55f;
    int numFakeCardsOnLeft = GameState.Get().GetGameEntity().GetNumberOfFakeMulliganCardsToShowOnLeft(mulliganManager.m_startingCards.Count);
    int numFakeCardsOnRight = GameState.Get().GetGameEntity().GetNumberOfFakeMulliganCardsToShowOnRight(mulliganManager.m_startingCards.Count);
    if (!isMulliganOver)
    {
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
      {
        mulliganManager.pendingFakeHeroCount = numFakeCardsOnLeft + numFakeCardsOnRight;
        for (int index = 0; index < numFakeCardsOnLeft; ++index)
          AssetLoader.Get().InstantiatePrefab((AssetReference) GameState.Get().GetStringGameOption(GameEntityOption.ALTERNATE_MULLIGAN_ACTOR_NAME), new PrefabCallback<GameObject>(mulliganManager.OnFakeHeroActorLoaded), (object) mulliganManager.fakeCardsOnLeft, AssetLoadingOptions.IgnorePrefabPosition);
        for (int index = 0; index < numFakeCardsOnRight; ++index)
          AssetLoader.Get().InstantiatePrefab((AssetReference) GameState.Get().GetStringGameOption(GameEntityOption.ALTERNATE_MULLIGAN_ACTOR_NAME), new PrefabCallback<GameObject>(mulliganManager.OnFakeHeroActorLoaded), (object) mulliganManager.fakeCardsOnRight, AssetLoadingOptions.IgnorePrefabPosition);
      }
      while (mulliganManager.pendingFakeHeroCount > 0)
        yield return (object) null;
    }
    else
    {
      numFakeCardsOnLeft = 0;
      numFakeCardsOnRight = 0;
    }
    float spaceForEachCard = zoneWidth / (float) Mathf.Max(mulliganManager.m_startingCards.Count + numFakeCardsOnLeft + numFakeCardsOnRight, 1);
    float spacingToUse = spaceForEachCard;
    float leftSideOfZone = mulliganManager.startingHandZone.transform.position.x - zoneWidth / 2f;
    float rightSideOfZone = mulliganManager.startingHandZone.transform.position.x + zoneWidth / 2f;
    float timingBonus = 0.1f;
    int numCardsToDealExcludingBonusCard = mulliganManager.m_startingCards.Count;
    mulliganManager.opposingSideHandZone.SetDoNotUpdateLayout(false);
    mulliganManager.opposingSideHandZone.UpdateLayout((Card) null, true, 3);
    float cardHeightOffset = 0.0f;
    if ((bool) UniversalInputManager.UsePhoneUI)
      cardHeightOffset = 7f;
    float cardZpos = mulliganManager.startingHandZone.transform.position.z - 0.3f;
    if ((bool) UniversalInputManager.UsePhoneUI)
      cardZpos = mulliganManager.startingHandZone.transform.position.z - 0.2f;
    float xOffset = spacingToUse / 2f;
    GameObject card;
    foreach (Actor actor in mulliganManager.fakeCardsOnLeft)
    {
      if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
      {
        card = actor.gameObject;
        iTween.Stop(card);
        Vector3[] vector3Array = new Vector3[3]
        {
          card.transform.position,
          new Vector3(card.transform.position.x, card.transform.position.y + 3.6f, card.transform.position.z),
          new Vector3(leftSideOfZone + xOffset, mulliganManager.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos)
        };
        iTween.MoveTo(card, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
          iTween.ScaleTo(card, GameState.Get().GetGameEntity().GetAlternateMulliganActorScale(), MulliganManager.ANIMATION_TIME_DEAL_CARD);
        else
          iTween.ScaleTo(card, (Vector3) MulliganManager.FRIENDLY_PLAYER_CARD_SCALE, MulliganManager.ANIMATION_TIME_DEAL_CARD);
        iTween.RotateTo(card, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "delay", (object) (float) ((double) MulliganManager.ANIMATION_TIME_DEAL_CARD / 16.0)));
        yield return (object) new WaitForSeconds(0.04f);
        SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart09_CardsOntoTable.prefab:da502e035813b5742a04d2ef4f588255", card);
        xOffset += spacingToUse;
        yield return (object) new WaitForSeconds(0.05f + timingBonus);
        timingBonus = 0.0f;
        card = (GameObject) null;
      }
    }
    for (int i = 0; i < numCardsToDealExcludingBonusCard; ++i)
    {
      if (!((UnityEngine.Object) mulliganManager.m_startingCards[i] == (UnityEngine.Object) null))
      {
        card = mulliganManager.m_startingCards[i].gameObject;
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS) && mulliganManager.choiceHeroActors.ContainsKey(mulliganManager.m_startingCards[i]))
          card = mulliganManager.choiceHeroActors[mulliganManager.m_startingCards[i]].transform.parent.gameObject;
        iTween.Stop(card);
        Vector3[] vector3Array = new Vector3[3]
        {
          card.transform.position,
          new Vector3(card.transform.position.x, card.transform.position.y + 3.6f, card.transform.position.z),
          new Vector3(leftSideOfZone + xOffset, mulliganManager.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos)
        };
        iTween.MoveTo(card, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
          iTween.ScaleTo(card, GameState.Get().GetGameEntity().GetAlternateMulliganActorScale(), MulliganManager.ANIMATION_TIME_DEAL_CARD);
        else
          iTween.ScaleTo(card, (Vector3) MulliganManager.FRIENDLY_PLAYER_CARD_SCALE, MulliganManager.ANIMATION_TIME_DEAL_CARD);
        iTween.RotateTo(card, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "delay", (object) (float) ((double) MulliganManager.ANIMATION_TIME_DEAL_CARD / 16.0)));
        yield return (object) new WaitForSeconds(0.04f);
        SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart09_CardsOntoTable.prefab:da502e035813b5742a04d2ef4f588255", card);
        xOffset += spacingToUse;
        yield return (object) new WaitForSeconds(0.05f + timingBonus);
        timingBonus = 0.0f;
        card = (GameObject) null;
      }
    }
    foreach (Actor actor in mulliganManager.fakeCardsOnRight)
    {
      if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
      {
        card = actor.gameObject;
        iTween.Stop(card);
        Vector3[] vector3Array = new Vector3[3]
        {
          card.transform.position,
          new Vector3(card.transform.position.x, card.transform.position.y + 3.6f, card.transform.position.z),
          new Vector3(leftSideOfZone + xOffset, mulliganManager.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos)
        };
        iTween.MoveTo(card, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
          iTween.ScaleTo(card, GameState.Get().GetGameEntity().GetAlternateMulliganActorScale(), MulliganManager.ANIMATION_TIME_DEAL_CARD);
        else
          iTween.ScaleTo(card, (Vector3) MulliganManager.FRIENDLY_PLAYER_CARD_SCALE, MulliganManager.ANIMATION_TIME_DEAL_CARD);
        iTween.RotateTo(card, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "delay", (object) (float) ((double) MulliganManager.ANIMATION_TIME_DEAL_CARD / 16.0)));
        yield return (object) new WaitForSeconds(0.04f);
        SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart09_CardsOntoTable.prefab:da502e035813b5742a04d2ef4f588255", card);
        xOffset += spacingToUse;
        yield return (object) new WaitForSeconds(0.05f + timingBonus);
        timingBonus = 0.0f;
        card = (GameObject) null;
      }
    }
    if (mulliganManager.skipCardChoosing)
    {
      mulliganManager.mulliganChooseBanner = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.GetChooseBannerPrefab());
      mulliganManager.SetMulliganBannerText(GameStrings.Get("GAMEPLAY_MULLIGAN_STARTING_HAND"));
      Vector3 position = Board.Get().FindBone("ChoiceBanner").position;
      mulliganManager.mulliganChooseBanner.transform.position = position;
      Vector3 localScale = mulliganManager.mulliganChooseBanner.transform.localScale;
      mulliganManager.mulliganChooseBanner.transform.localScale = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
      iTween.ScaleTo(mulliganManager.mulliganChooseBanner, localScale, 0.5f);
      mulliganManager.m_ShrinkStartingHandBanner = mulliganManager.ShrinkStartingHandBanner(mulliganManager.mulliganChooseBanner);
      mulliganManager.StartCoroutine(mulliganManager.m_ShrinkStartingHandBanner);
      mulliganManager.ShowMulliganDetail();
    }
    yield return (object) new WaitForSeconds(1.1f);
    while (GameState.Get().IsBusy())
      yield return (object) null;
    if (mulliganManager.friendlyPlayerGoesFirst)
    {
      xOffset = 0.0f;
      for (int index = mulliganManager.m_startingCards.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) mulliganManager.m_startingCards[index] != (UnityEngine.Object) null)
        {
          GameObject gameObject = mulliganManager.m_startingCards[index].gameObject;
          if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS) && mulliganManager.choiceHeroActors.ContainsKey(mulliganManager.m_startingCards[index]))
            gameObject = mulliganManager.choiceHeroActors[mulliganManager.m_startingCards[index]].gameObject;
          iTween.Stop(gameObject);
          iTween.MoveTo(gameObject, iTween.Hash((object) "position", (object) new Vector3((float) ((double) rightSideOfZone - (double) spaceForEachCard - (double) xOffset + (double) spaceForEachCard / 2.0), mulliganManager.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos), (object) "time", (object) 0.9333333f, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
          xOffset += spaceForEachCard;
        }
      }
    }
    GameState.Get().GetGameEntity().OnMulliganCardsDealt(mulliganManager.m_startingCards);
    yield return (object) new WaitForSeconds(0.6f);
    if (mulliganManager.skipCardChoosing)
    {
      if (GameState.Get().IsMulliganPhase() || GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
      {
        if (GameState.Get().IsFriendlySidePlayerTurn())
          TurnStartManager.Get().BeginListeningForTurnEvents();
        mulliganManager.m_WaitForOpponentToFinishMulligan = mulliganManager.WaitForOpponentToFinishMulligan();
        mulliganManager.StartCoroutine(mulliganManager.m_WaitForOpponentToFinishMulligan);
      }
      else
      {
        yield return (object) new WaitForSeconds(2f);
        mulliganManager.EndMulligan();
      }
    }
    else
    {
      mulliganManager.SetupCardCollider(ref mulliganManager.m_startingCards);
      string mulliganBannerText = GameState.Get().GetGameEntity().GetMulliganBannerText();
      string bannerSubtitleText = GameState.Get().GetGameEntity().GetMulliganBannerSubtitleText();
      mulliganManager.mulliganChooseBanner = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.GetChooseBannerPrefab(), Board.Get().FindBone("ChoiceBanner").position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
      mulliganManager.SetMulliganBannerText(mulliganBannerText, bannerSubtitleText);
      mulliganManager.ShowMulliganDetail();
      mulliganManager.CreateTagConditionalVFXs(Board.Get().FindBone("ChoiceBanner").position);
      if (GameState.Get().IsInChoiceMode() && GameMgr.Get().IsSpectator())
      {
        mulliganManager.m_replaceLabels = new List<MulliganReplaceLabel>();
        for (int index = 0; index < mulliganManager.m_startingCards.Count; ++index)
        {
          if ((UnityEngine.Object) mulliganManager.m_startingCards[index] != (UnityEngine.Object) null)
            InputManager.Get().DoNetworkResponse(mulliganManager.m_startingCards[index].GetEntity());
          mulliganManager.m_replaceLabels.Add((MulliganReplaceLabel) null);
        }
      }
      while ((UnityEngine.Object) mulliganManager.mulliganButton == (UnityEngine.Object) null && GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
        yield return (object) null;
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
      {
        mulliganManager.mulliganButton.transform.position = new Vector3(mulliganManager.startingHandZone.transform.position.x, mulliganManager.friendlySideHandZone.transform.position.y, mulliganManager.myHeroCardActor.transform.position.z);
        mulliganManager.mulliganButton.transform.localEulerAngles = new Vector3(90f, 90f, 90f);
        mulliganManager.mulliganButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(mulliganManager.OnMulliganButtonReleased));
        mulliganManager.mulliganButtonWidget.transform.position = new Vector3(mulliganManager.startingHandZone.transform.position.x, mulliganManager.friendlySideHandZone.transform.position.y, mulliganManager.myHeroCardActor.transform.position.z);
        mulliganManager.mulliganButtonWidget.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(mulliganManager.OnMulliganButtonReleased));
        mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganButton = mulliganManager.WaitAFrameBeforeSendingEventToMulliganButton(mulliganManager.mulliganButton);
        mulliganManager.StartCoroutine(mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganButton);
        if (!GameMgr.Get().IsSpectator() && !Options.Get().GetBool(Option.HAS_SEEN_MULLIGAN, false) && !GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE) && UserAttentionManager.CanShowAttentionGrabber("MulliganManager.DealStartingCards:" + (object) Option.HAS_SEEN_MULLIGAN))
        {
          mulliganManager.innkeeperMulliganDialog = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_MULLIGAN_13"), "VO_INNKEEPER_MULLIGAN_13.prefab:3ec6b2e741ac16d4ca519bdfd26d10e3");
          Options.Get().SetBool(Option.HAS_SEEN_MULLIGAN, true);
          mulliganManager.mulliganButton.GetComponent<Collider>().enabled = false;
        }
      }
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
      {
        while ((UnityEngine.Object) mulliganManager.m_refreshButton == (UnityEngine.Object) null)
          yield return (object) null;
        mulliganManager.m_refreshButton.transform.position = new Vector3(mulliganManager.mulliganButton.transform.position.x + 2f, mulliganManager.mulliganButton.transform.position.y, mulliganManager.mulliganButton.transform.position.z);
        mulliganManager.m_refreshButton.transform.localEulerAngles = new Vector3(90f, 90f, 90f);
        mulliganManager.m_refreshButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(mulliganManager.OnMulliganRefreshButtonReleased));
        mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton = mulliganManager.WaitAFrameBeforeSendingEventToMulliganButton(mulliganManager.m_refreshButton);
        mulliganManager.StartCoroutine(mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton);
        mulliganManager.m_refreshButton.GetComponent<Collider>().enabled = true;
      }
      GameState.Get().GetGameEntity().StartMulliganSoundtracks(true);
      mulliganManager.m_waitingForUserInput = true;
      while ((UnityEngine.Object) mulliganManager.innkeeperMulliganDialog != (UnityEngine.Object) null)
        yield return (object) null;
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
        mulliganManager.mulliganButton.GetComponent<Collider>().enabled = true;
      if (mulliganManager.skipCardChoosing || Cheats.Get().ShouldSkipMulligan())
        mulliganManager.BeginDealNewCards();
    }
  }

  private IEnumerator DealStartingCards()
  {
    MulliganManager mulliganManager = this;
    yield return (object) new WaitForSeconds(1f);
    while (!mulliganManager.introComplete)
      yield return (object) null;
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsAfterIntroBeforeMulligan());
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.DO_OPENING_TAUNTS) && !Cheats.Get().ShouldSkipMulligan())
    {
      mulliganManager.m_PlayStartingTaunts = mulliganManager.PlayStartingTaunts();
      mulliganManager.StartCoroutine(mulliganManager.m_PlayStartingTaunts);
    }
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    mulliganManager.friendlyPlayerGoesFirst = friendlySidePlayer.HasTag(GAME_TAG.FIRST_PLAYER);
    mulliganManager.GetStartingLists();
    if (mulliganManager.m_startingCards.Count == 0)
      mulliganManager.SkipCardChoosing();
    foreach (Card startingCard in mulliganManager.m_startingCards)
    {
      startingCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
      startingCard.GetActor().TurnOffCollider();
      startingCard.GetActor().GetMeshRenderer().gameObject.layer = 8;
      startingCard.GetActor().m_nameTextMesh.UpdateNow();
    }
    float x = mulliganManager.startingHandZone.GetComponent<Collider>().bounds.size.x;
    if ((bool) UniversalInputManager.UsePhoneUI)
      x *= 0.55f;
    float spaceForEachCard = x / (float) mulliganManager.m_startingCards.Count;
    float spacingToUse = x / (float) (mulliganManager.m_startingCards.Count + 1);
    float leftSideOfZone = mulliganManager.startingHandZone.transform.position.x - x / 2f;
    float rightSideOfZone = mulliganManager.startingHandZone.transform.position.x + x / 2f;
    float timingBonus = 0.1f;
    int numCardsToDealExcludingBonusCard = mulliganManager.m_startingCards.Count;
    if (!mulliganManager.friendlyPlayerGoesFirst)
    {
      numCardsToDealExcludingBonusCard = mulliganManager.m_bonusCardIndex;
      spacingToUse = spaceForEachCard;
    }
    else if (mulliganManager.m_startingOppCards.Count > 0)
    {
      mulliganManager.m_startingOppCards[mulliganManager.m_bonusCardIndex].SetDoNotSort(true);
      if (mulliganManager.m_coinCardIndex >= 0)
        mulliganManager.m_startingOppCards[mulliganManager.m_coinCardIndex].SetDoNotSort(true);
    }
    mulliganManager.opposingSideHandZone.SetDoNotUpdateLayout(false);
    mulliganManager.opposingSideHandZone.UpdateLayout((Card) null, true, 3);
    float cardHeightOffset = 0.0f;
    if ((bool) UniversalInputManager.UsePhoneUI)
      cardHeightOffset = 7f;
    float cardZpos = mulliganManager.startingHandZone.transform.position.z - 0.3f;
    if ((bool) UniversalInputManager.UsePhoneUI)
      cardZpos = mulliganManager.startingHandZone.transform.position.z - 0.2f;
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsBeforeDealingBaseMulliganCards());
    float xOffset = spacingToUse / 2f;
    GameObject topCard;
    for (int i = 0; i < numCardsToDealExcludingBonusCard; ++i)
    {
      topCard = mulliganManager.m_startingCards[i].gameObject;
      iTween.Stop(topCard);
      Vector3[] vector3Array = new Vector3[3]
      {
        topCard.transform.position,
        new Vector3(topCard.transform.position.x, topCard.transform.position.y + 3.6f, topCard.transform.position.z),
        new Vector3(leftSideOfZone + xOffset, mulliganManager.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos)
      };
      iTween.MoveTo(topCard, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
      iTween.ScaleTo(topCard, (Vector3) MulliganManager.FRIENDLY_PLAYER_CARD_SCALE, MulliganManager.ANIMATION_TIME_DEAL_CARD);
      iTween.RotateTo(topCard, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "delay", (object) (float) ((double) MulliganManager.ANIMATION_TIME_DEAL_CARD / 16.0)));
      yield return (object) new WaitForSeconds(0.04f);
      SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart09_CardsOntoTable.prefab:da502e035813b5742a04d2ef4f588255", topCard);
      xOffset += spacingToUse;
      yield return (object) new WaitForSeconds(0.05f + timingBonus);
      timingBonus = 0.0f;
      topCard = (GameObject) null;
    }
    if (mulliganManager.skipCardChoosing)
    {
      mulliganManager.mulliganChooseBanner = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.GetChooseBannerPrefab());
      mulliganManager.SetMulliganBannerText(GameStrings.Get("GAMEPLAY_MULLIGAN_STARTING_HAND"));
      Vector3 position = Board.Get().FindBone("ChoiceBanner").position;
      mulliganManager.mulliganChooseBanner.transform.position = position;
      Vector3 localScale = mulliganManager.mulliganChooseBanner.transform.localScale;
      mulliganManager.mulliganChooseBanner.transform.localScale = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
      iTween.ScaleTo(mulliganManager.mulliganChooseBanner, localScale, 0.5f);
      mulliganManager.m_ShrinkStartingHandBanner = mulliganManager.ShrinkStartingHandBanner(mulliganManager.mulliganChooseBanner);
      mulliganManager.StartCoroutine(mulliganManager.m_ShrinkStartingHandBanner);
    }
    yield return (object) new WaitForSeconds(1.1f);
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsAfterDealingBaseMulliganCards());
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsBeforeCoinFlip());
    if ((UnityEngine.Object) mulliganManager.coinObject != (UnityEngine.Object) null)
    {
      Transform bone = Board.Get().FindBone("MulliganCoinPosition");
      mulliganManager.coinObject.transform.position = bone.position;
      mulliganManager.coinObject.transform.localEulerAngles = bone.localEulerAngles;
      mulliganManager.coinObject.SetActive(true);
      mulliganManager.coinObject.GetComponent<CoinEffect>().DoAnim(mulliganManager.friendlyPlayerGoesFirst);
      SoundManager.Get().LoadAndPlay((AssetReference) "FX_MulliganCoin03_CoinFlip.prefab:07015cb3f02713a45aa03fc3aa798778", mulliganManager.coinObject);
      mulliganManager.coinLocation = bone.position;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "MulliganResultText.prefab:0369b435afd2e344db21e58648f8636c", new PrefabCallback<GameObject>(mulliganManager.CoinTossTextCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
      yield return (object) new WaitForSeconds(2f);
    }
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsAfterCoinFlip());
    if (!mulliganManager.friendlyPlayerGoesFirst)
    {
      topCard = mulliganManager.m_startingCards[mulliganManager.m_bonusCardIndex].gameObject;
      Vector3[] vector3Array = new Vector3[3]
      {
        topCard.transform.position,
        new Vector3(topCard.transform.position.x, topCard.transform.position.y + 3.6f, topCard.transform.position.z),
        new Vector3(leftSideOfZone + xOffset, mulliganManager.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos)
      };
      iTween.MoveTo(topCard, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
      iTween.ScaleTo(topCard, (Vector3) MulliganManager.FRIENDLY_PLAYER_CARD_SCALE, MulliganManager.ANIMATION_TIME_DEAL_CARD);
      iTween.RotateTo(topCard, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "delay", (object) (float) ((double) MulliganManager.ANIMATION_TIME_DEAL_CARD / 8.0)));
      yield return (object) new WaitForSeconds(0.04f);
      SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart20_CardDealSingle.prefab:0da693603ca05d846b9cfe26e9f0e3c7", topCard);
      topCard = (GameObject) null;
    }
    else if (mulliganManager.m_startingOppCards.Count > 0)
    {
      mulliganManager.m_startingOppCards[mulliganManager.m_bonusCardIndex].SetDoNotSort(false);
      mulliganManager.opposingSideHandZone.UpdateLayout((Card) null, true, 4);
    }
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsAfterDealingBonusCard());
    yield return (object) new WaitForSeconds(1.75f);
    while (GameState.Get().IsBusy())
      yield return (object) null;
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsBeforeSpreadingMulliganCards());
    if (mulliganManager.friendlyPlayerGoesFirst)
    {
      xOffset = 0.0f;
      for (int index = mulliganManager.m_startingCards.Count - 1; index >= 0; --index)
      {
        GameObject gameObject = mulliganManager.m_startingCards[index].gameObject;
        iTween.Stop(gameObject);
        iTween.MoveTo(gameObject, iTween.Hash((object) "position", (object) new Vector3((float) ((double) rightSideOfZone - (double) spaceForEachCard - (double) xOffset + (double) spaceForEachCard / 2.0), mulliganManager.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos), (object) "time", (object) 0.9333333f, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
        xOffset += spaceForEachCard;
      }
    }
    GameState.Get().GetGameEntity().OnMulliganCardsDealt(mulliganManager.m_startingCards);
    yield return (object) new WaitForSeconds(0.6f);
    yield return (object) mulliganManager.StartCoroutine(GameState.Get().GetGameEntity().DoActionsAfterSpreadingMulliganCards());
    if (mulliganManager.skipCardChoosing)
    {
      if (GameState.Get().IsMulliganPhase())
      {
        if (GameState.Get().IsFriendlySidePlayerTurn())
          TurnStartManager.Get().BeginListeningForTurnEvents();
        mulliganManager.m_WaitForOpponentToFinishMulligan = mulliganManager.WaitForOpponentToFinishMulligan();
        mulliganManager.StartCoroutine(mulliganManager.m_WaitForOpponentToFinishMulligan);
      }
      else
      {
        yield return (object) new WaitForSeconds(2f);
        mulliganManager.EndMulligan();
      }
    }
    else
    {
      foreach (Card startingCard in mulliganManager.m_startingCards)
        startingCard.GetActor().TurnOnCollider();
      mulliganManager.mulliganChooseBanner = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.GetChooseBannerPrefab(), Board.Get().FindBone("ChoiceBanner").position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
      mulliganManager.SetMulliganBannerText(GameStrings.Get("GAMEPLAY_MULLIGAN_STARTING_HAND"), GameStrings.Get("GAMEPLAY_MULLIGAN_SUBTITLE"));
      mulliganManager.CreateTagConditionalVFXs(Board.Get().FindBone("ChoiceBanner").position);
      if (GameState.Get().IsInChoiceMode())
      {
        mulliganManager.m_replaceLabels = new List<MulliganReplaceLabel>();
        for (int index = 0; index < mulliganManager.m_startingCards.Count; ++index)
        {
          InputManager.Get().DoNetworkResponse(mulliganManager.m_startingCards[index].GetEntity());
          mulliganManager.m_replaceLabels.Add((MulliganReplaceLabel) null);
        }
      }
      while ((UnityEngine.Object) mulliganManager.mulliganButton == (UnityEngine.Object) null && !GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
        yield return (object) null;
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
      {
        mulliganManager.mulliganButton.transform.position = new Vector3(mulliganManager.startingHandZone.transform.position.x, mulliganManager.friendlySideHandZone.transform.position.y, mulliganManager.myHeroCardActor.transform.position.z);
        mulliganManager.mulliganButton.transform.localEulerAngles = new Vector3(90f, 90f, 90f);
        mulliganManager.mulliganButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(mulliganManager.OnMulliganButtonReleased));
        mulliganManager.mulliganButtonWidget.transform.position = new Vector3(mulliganManager.startingHandZone.transform.position.x, mulliganManager.friendlySideHandZone.transform.position.y, mulliganManager.myHeroCardActor.transform.position.z);
        mulliganManager.mulliganButtonWidget.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(mulliganManager.OnMulliganButtonReleased));
        mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganButton = mulliganManager.WaitAFrameBeforeSendingEventToMulliganButton(mulliganManager.mulliganButton);
        mulliganManager.StartCoroutine(mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganButton);
        if (!GameMgr.Get().IsSpectator() && !Options.Get().GetBool(Option.HAS_SEEN_MULLIGAN, false) && !GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE) && UserAttentionManager.CanShowAttentionGrabber("MulliganManager.DealStartingCards:" + (object) Option.HAS_SEEN_MULLIGAN))
        {
          mulliganManager.innkeeperMulliganDialog = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_MULLIGAN_13"), "VO_INNKEEPER_MULLIGAN_13.prefab:3ec6b2e741ac16d4ca519bdfd26d10e3");
          Options.Get().SetBool(Option.HAS_SEEN_MULLIGAN, true);
          mulliganManager.mulliganButton.GetComponent<Collider>().enabled = false;
        }
      }
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
      {
        while ((UnityEngine.Object) mulliganManager.m_refreshButton == (UnityEngine.Object) null && GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
          yield return (object) null;
        mulliganManager.m_refreshButton.transform.position = new Vector3(mulliganManager.mulliganButton.transform.position.x + 2f, mulliganManager.mulliganButton.transform.position.y, mulliganManager.mulliganButton.transform.position.z);
        mulliganManager.m_refreshButton.transform.localEulerAngles = new Vector3(90f, 90f, 90f);
        mulliganManager.m_refreshButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(mulliganManager.OnMulliganRefreshButtonReleased));
        mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton = mulliganManager.WaitAFrameBeforeSendingEventToMulliganButton(mulliganManager.m_refreshButton);
        mulliganManager.StartCoroutine(mulliganManager.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton);
        mulliganManager.m_refreshButton.GetComponent<Collider>().enabled = true;
      }
      GameState.Get().GetGameEntity().StartMulliganSoundtracks(true);
      mulliganManager.m_waitingForUserInput = true;
      while ((UnityEngine.Object) mulliganManager.innkeeperMulliganDialog != (UnityEngine.Object) null)
        yield return (object) null;
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
        mulliganManager.mulliganButton.GetComponent<Collider>().enabled = true;
      if (mulliganManager.skipCardChoosing || Cheats.Get().ShouldSkipMulligan())
        mulliganManager.BeginDealNewCards();
    }
  }

  private IEnumerator WaitAFrameBeforeSendingEventToMulliganButton(NormalButton button)
  {
    yield return (object) null;
    button.m_button.GetComponent<PlayMakerFSM>().SendEvent("Birth");
  }

  public bool IsMulliganTimerActive() => (UnityEngine.Object) this.m_mulliganTimer != (UnityEngine.Object) null;

  private void BeginMulliganCountdown(float endTimeStamp)
  {
    if (!this.m_waitingForUserInput && !GameState.Get().GetBooleanGameOption(GameEntityOption.ALWAYS_SHOW_MULLIGAN_TIMER))
      return;
    if ((UnityEngine.Object) this.m_mulliganTimer == (UnityEngine.Object) null)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.mulliganTimerPrefab);
      this.m_mulliganTimer = gameObject.GetComponent<MulliganTimer>();
      if ((UnityEngine.Object) this.m_mulliganTimer == (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
        return;
      }
    }
    this.m_mulliganTimer.SetEndTime(endTimeStamp);
  }

  private void StopMulliganCountdown() => this.DestroyMulliganTimer();

  public GameObject GetMulliganBanner() => this.mulliganChooseBanner;

  public GameObject GetMulliganButton() => (UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null ? this.mulliganButton.gameObject : (GameObject) null;

  public GameObject GetMulliganRefreshButton() => (UnityEngine.Object) this.m_refreshButton != (UnityEngine.Object) null ? this.m_refreshButton.gameObject : (GameObject) null;

  public Vector3 GetMulliganTimerPosition()
  {
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_TIMER_HAS_ALTERNATE_POSITION))
      return GameState.Get().GetGameEntity().GetMulliganTimerAlternatePosition();
    if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
      return this.mulliganButton.transform.position;
    return !((UnityEngine.Object) this.m_mulliganTimer != (UnityEngine.Object) null) ? new Vector3(0.0f, 0.0f, 0.0f) : this.m_mulliganTimer.transform.position;
  }

  private void CoinTossTextCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.coinTossText = go;
    RenderUtils.SetAlpha(go, 1f);
    go.transform.position = this.coinLocation + new Vector3(0.0f, 0.0f, -1f);
    go.transform.eulerAngles = new Vector3(90f, 0.0f, 0.0f);
    go.transform.GetComponentInChildren<UberText>().Text = !this.friendlyPlayerGoesFirst ? GameStrings.Get("GAMEPLAY_COIN_TOSS_LOST") : GameStrings.Get("GAMEPLAY_COIN_TOSS_WON");
    GameState.Get().GetGameEntity().NotifyOfCoinFlipResult();
    this.m_AnimateCoinTossText = this.AnimateCoinTossText();
    this.StartCoroutine(this.m_AnimateCoinTossText);
  }

  private IEnumerator AnimateCoinTossText()
  {
    yield return (object) new WaitForSeconds(1.8f);
    if (!((UnityEngine.Object) this.coinTossText == (UnityEngine.Object) null))
    {
      iTween.FadeTo(this.coinTossText, 1f, 0.25f);
      iTween.MoveTo(this.coinTossText, this.coinTossText.transform.position + new Vector3(0.0f, 0.5f, 0.0f), 2f);
      yield return (object) new WaitForSeconds(1.9f);
      while (GameState.Get().IsBusy())
        yield return (object) null;
      if (!((UnityEngine.Object) this.coinTossText == (UnityEngine.Object) null))
      {
        iTween.FadeTo(this.coinTossText, 0.0f, 1f);
        yield return (object) new WaitForSeconds(0.1f);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.coinTossText);
      }
    }
  }

  private MulliganReplaceLabel CreateNewUILabelAtCardPosition(
    MulliganReplaceLabel prefab,
    int cardPosition)
  {
    MulliganReplaceLabel labelAtCardPosition = UnityEngine.Object.Instantiate<MulliganReplaceLabel>(prefab);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      labelAtCardPosition.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
      labelAtCardPosition.transform.position = new Vector3(this.m_startingCards[cardPosition].transform.position.x, this.m_startingCards[cardPosition].transform.position.y + 0.3f, this.m_startingCards[cardPosition].transform.position.z - 1.1f);
    }
    else
      labelAtCardPosition.transform.position = new Vector3(this.m_startingCards[cardPosition].transform.position.x, this.m_startingCards[cardPosition].transform.position.y + 0.3f, this.m_startingCards[cardPosition].transform.position.z - this.startingHandZone.GetComponent<Collider>().bounds.size.z / 2.6f);
    return labelAtCardPosition;
  }

  public void SetAllMulliganCardsToHold()
  {
    foreach (Card card in this.friendlySideHandZone.GetCards())
      InputManager.Get().DoNetworkResponse(card.GetEntity());
  }

  private void ToggleHoldState(int startingCardsIndex, bool forceDisable = false)
  {
    if (!GameState.Get().IsInChoiceMode() || startingCardsIndex >= this.m_startingCards.Count || (!forceDisable || forceDisable && this.m_handCardsMarkedForReplace[startingCardsIndex]) && !InputManager.Get().DoNetworkResponse(this.m_startingCards[startingCardsIndex].GetEntity()))
      return;
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
    {
      this.m_handCardsMarkedForReplace[startingCardsIndex] = !forceDisable && !this.m_handCardsMarkedForReplace[startingCardsIndex];
      if (!this.m_handCardsMarkedForReplace[startingCardsIndex])
      {
        SoundManager.Get().LoadAndPlay((AssetReference) "GM_ChatWarning.prefab:41baa28576a71664eabd8712a198b67f");
        if (this.m_xLabels != null && (UnityEngine.Object) this.m_xLabels[startingCardsIndex] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_xLabels[startingCardsIndex]);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_replaceLabels[startingCardsIndex].gameObject);
      }
      else
      {
        SoundManager.Get().LoadAndPlay((AssetReference) "HeroDropItem1.prefab:587232e6704b20942af1205d00cfc0f9");
        if (this.m_xLabels != null && (UnityEngine.Object) this.m_xLabels[startingCardsIndex] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_xLabels[startingCardsIndex]);
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.mulliganXlabelPrefab);
        gameObject.transform.position = this.m_startingCards[startingCardsIndex].transform.position;
        gameObject.transform.rotation = this.m_startingCards[startingCardsIndex].transform.rotation;
        if (this.m_xLabels != null)
          this.m_xLabels[startingCardsIndex] = gameObject;
        if (this.m_replaceLabels != null)
          this.m_replaceLabels[startingCardsIndex] = this.CreateNewUILabelAtCardPosition(this.mulliganReplaceLabelPrefab, startingCardsIndex);
      }
    }
    else
    {
      this.m_handCardsMarkedForReplace[startingCardsIndex] = !forceDisable && !this.m_handCardsMarkedForReplace[startingCardsIndex];
      if (!this.m_handCardsMarkedForReplace[startingCardsIndex])
        SoundManager.Get().LoadAndPlay((AssetReference) "GM_ChatWarning.prefab:41baa28576a71664eabd8712a198b67f");
      else
        SoundManager.Get().LoadAndPlay((AssetReference) "HeroDropItem1.prefab:587232e6704b20942af1205d00cfc0f9");
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
        GameState.Get().GetGameEntity().ToggleAlternateMulliganActorHighlight(this.m_startingCards[startingCardsIndex], this.m_handCardsMarkedForReplace[startingCardsIndex]);
    }
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_REQUIRES_CONFIRMATION))
      return;
    this.BeginDealNewCards();
  }

  private void DestroyXobjects()
  {
    if (this.m_xLabels == null)
      return;
    for (int index = 0; index < this.m_xLabels.Length; ++index)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_xLabels[index]);
    this.m_xLabels = (GameObject[]) null;
  }

  private void DestroyChooseBanner()
  {
    if ((UnityEngine.Object) this.mulliganChooseBanner == (UnityEngine.Object) null)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.mulliganChooseBanner);
  }

  private GameObject GetChooseBannerPrefab()
  {
    if ((UnityEngine.Object) this.m_overrideMulliganChooseBannerPrefab == (UnityEngine.Object) null)
      this.OverrideChooseBannerPrefab();
    return !((UnityEngine.Object) this.m_overrideMulliganChooseBannerPrefab != (UnityEngine.Object) null) ? this.mulliganChooseBannerPrefab : this.m_overrideMulliganChooseBannerPrefab;
  }

  private void OverrideChooseBannerPrefab()
  {
    this.m_overrideMulliganChooseBannerPrefab = (GameObject) null;
    if (GameState.Get() == null)
      return;
    int num = -1;
    for (int index = 0; index < this.tagConditionalVFXPrefabs.Count; ++index)
    {
      MulliganManager.TagConditionalVFX conditionalVfxPrefab = this.tagConditionalVFXPrefabs[index];
      if (GameState.Get().GetGameEntity().GetTag(conditionalVfxPrefab.m_requiredTag) != 0 && conditionalVfxPrefab.m_bannerReplacementPrefabPriority > num)
      {
        this.m_overrideMulliganChooseBannerPrefab = conditionalVfxPrefab.m_bannerReplacementPrefab;
        num = conditionalVfxPrefab.m_bannerReplacementPrefabPriority;
      }
    }
  }

  private void CreateTagConditionalVFXs(Vector3 position)
  {
    if (GameState.Get() == null)
      return;
    this.m_tagConditionalVFXs = new List<GameObject>();
    for (int index = 0; index < this.tagConditionalVFXPrefabs.Count; ++index)
    {
      MulliganManager.TagConditionalVFX conditionalVfxPrefab = this.tagConditionalVFXPrefabs[index];
      if (GameState.Get().GetGameEntity().GetTag(conditionalVfxPrefab.m_requiredTag) != 0)
        this.m_tagConditionalVFXs.Add(UnityEngine.Object.Instantiate<GameObject>(conditionalVfxPrefab.m_VFXPrefab, position, Quaternion.identity));
    }
  }

  private void DestroyTagConditionalVFXs()
  {
    if (this.m_tagConditionalVFXs == null)
      return;
    for (int index = 0; index < this.m_tagConditionalVFXs.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_tagConditionalVFXs[index] != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_tagConditionalVFXs[index]);
    }
    this.m_tagConditionalVFXs.Clear();
  }

  private void DestroyDetailLabel()
  {
    if (!((UnityEngine.Object) this.mulliganDetailLabel != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.mulliganDetailLabel);
    this.mulliganDetailLabel = (GameObject) null;
  }

  private void DestroyMulliganTimer()
  {
    if ((UnityEngine.Object) this.m_mulliganTimer == (UnityEngine.Object) null)
      return;
    this.m_mulliganTimer.SelfDestruct();
    this.m_mulliganTimer = (MulliganTimer) null;
  }

  public void ToggleHoldState(Actor toggleActor)
  {
    bool flag = false;
    GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE);
    List<Actor> actorList = new List<Actor>(this.fakeCardsOnLeft.Count + this.fakeCardsOnRight.Count);
    actorList.AddRange((IEnumerable<Actor>) this.fakeCardsOnLeft);
    actorList.AddRange((IEnumerable<Actor>) this.fakeCardsOnRight);
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
    {
      foreach (Actor actor in actorList)
      {
        if ((UnityEngine.Object) toggleActor == (UnityEngine.Object) actor)
          flag = GameState.Get().GetGameEntity().ToggleAlternateMulliganActorHighlight(actor);
        else
          GameState.Get().GetGameEntity().ToggleAlternateMulliganActorHighlight(actor, new bool?(false));
      }
    }
    if (flag)
    {
      for (int startingCardsIndex = 0; startingCardsIndex < this.m_startingCards.Count; ++startingCardsIndex)
        this.ToggleHoldState(startingCardsIndex, true);
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
      {
        if ((UnityEngine.Object) this.mulliganButtonWidget != (UnityEngine.Object) null)
        {
          this.mulliganButtonWidget.SetEnabled(false);
          this.mulliganButtonWidget.gameObject.SetActive(false);
        }
      }
      else if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
      {
        this.mulliganButton.SetEnabled(false);
        this.mulliganButton.gameObject.SetActive(false);
      }
      if (!((UnityEngine.Object) this.conditionalHelperTextLabel != (UnityEngine.Object) null))
        return;
      this.conditionalHelperTextLabel.gameObject.SetActive(true);
    }
    else
    {
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
      {
        if ((UnityEngine.Object) this.mulliganButtonWidget != (UnityEngine.Object) null)
          this.mulliganButtonWidget.gameObject.SetActive(true);
      }
      else if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
        this.mulliganButton.gameObject.SetActive(true);
      if (!((UnityEngine.Object) this.conditionalHelperTextLabel != (UnityEngine.Object) null))
        return;
      this.conditionalHelperTextLabel.gameObject.SetActive(false);
    }
  }

  public void ToggleHoldState(Card toggleCard)
  {
    bool flag = false;
    bool booleanGameOption = GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE);
    for (int index = 0; index < this.m_startingCards.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_startingCards[index] == (UnityEngine.Object) toggleCard)
        this.ToggleHoldState(index);
      else if (booleanGameOption)
        this.ToggleHoldState(index, true);
      flag |= this.m_handCardsMarkedForReplace[index];
    }
    List<Actor> actorList = new List<Actor>(this.fakeCardsOnLeft.Count + this.fakeCardsOnRight.Count);
    actorList.AddRange((IEnumerable<Actor>) this.fakeCardsOnLeft);
    actorList.AddRange((IEnumerable<Actor>) this.fakeCardsOnRight);
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
    {
      if ((UnityEngine.Object) this.mulliganButtonWidget != (UnityEngine.Object) null)
        this.mulliganButtonWidget.gameObject.SetActive(true);
    }
    else
    {
      if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
        this.mulliganButton.gameObject.SetActive(true);
      if ((UnityEngine.Object) this.m_refreshButton != (UnityEngine.Object) null)
        this.m_refreshButton.gameObject.SetActive(true);
    }
    if ((UnityEngine.Object) this.conditionalHelperTextLabel != (UnityEngine.Object) null)
      this.conditionalHelperTextLabel.gameObject.SetActive(false);
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
    {
      foreach (Actor actor in actorList)
        GameState.Get().GetGameEntity().ToggleAlternateMulliganActorHighlight(actor, new bool?(false));
    }
    if (!booleanGameOption || !((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null))
      return;
    if (!flag)
    {
      this.mulliganButton.SetEnabled(false);
      this.mulliganButtonWidget.SetEnabled(false);
    }
    else
    {
      this.mulliganButton.SetEnabled(true);
      this.mulliganButtonWidget.SetEnabled(true);
    }
  }

  public void ServerHasDealtReplacementCards(bool isFriendlySide)
  {
    if (isFriendlySide)
    {
      this.friendlyPlayerHasReplacementCards = true;
      if (!GameState.Get().IsFriendlySidePlayerTurn())
        return;
      TurnStartManager.Get().BeginListeningForTurnEvents();
    }
    else
      this.opponentPlayerHasReplacementCards = true;
  }

  public void AutomaticContinueMulligan()
  {
    if (this.m_waitingForUserInput)
    {
      if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
        this.mulliganButton.SetEnabled(false);
      if ((UnityEngine.Object) this.mulliganButtonWidget != (UnityEngine.Object) null)
        this.mulliganButtonWidget.SetEnabled(false);
      if ((UnityEngine.Object) this.m_refreshButton != (UnityEngine.Object) null)
        this.m_refreshButton.SetEnabled(false);
      this.DestroyMulliganTimer();
      this.BeginDealNewCards();
    }
    else
      this.SkipCardChoosing();
  }

  private void OnMulliganButtonReleased(UIEvent e)
  {
    if (!InputManager.Get().PermitDecisionMakingInput())
      return;
    if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
      this.mulliganButton.SetEnabled(false);
    if ((UnityEngine.Object) this.mulliganButtonWidget != (UnityEngine.Object) null)
      this.mulliganButtonWidget.SetEnabled(false);
    this.BeginDealNewCards();
  }

  private void BeginDealNewCards(bool isBGRefresing = false)
  {
    if (!isBGRefresing)
      GameState.Get().GetGameEntity().OnMulliganBeginDealNewCards();
    if (this.m_waitingForUserInput)
    {
      this.m_waitingForUserInput = isBGRefresing;
      this.m_RemoveOldCardsAnimation = this.RemoveOldCardsAnimation(isBGRefresing);
      this.StartCoroutine(this.m_RemoveOldCardsAnimation);
    }
    this.EnableDamageCapFX(!isBGRefresing);
  }

  private void OnMulliganRefreshButtonReleased(UIEvent e)
  {
    if (!InputManager.Get().PermitDecisionMakingInput())
      return;
    this.friendlyPlayerHasReplacementCards = false;
    if ((UnityEngine.Object) this.m_refreshButton != (UnityEngine.Object) null)
      this.m_refreshButton.gameObject.SetActive(false);
    this.BeginDealNewCards(true);
  }

  private void RefreshBGHeroes()
  {
    if (!InputManager.Get().PermitDecisionMakingInput())
      return;
    Network.Get().SendPreRefreshBGHeroes();
    GameState.Get().ClearFriendlyChoicesList();
    if (this.m_startingCards.Count <= 0 || !InputManager.Get().DoNetworkResponse(this.m_startingCards[0].GetEntity()))
      return;
    GameState.Get().SendChoices();
    GameState.Get().ClearFriendlyChoicesList();
    this.ClearHandCardsMarkedForReplace();
  }

  private void ClearHandCardsMarkedForReplace()
  {
    for (int index = 0; index < this.m_handCardsMarkedForReplace.Length; ++index)
      this.m_handCardsMarkedForReplace[index] = false;
  }

  private IEnumerator RemoveOldCardsAnimation(bool isBGRefreshing = false)
  {
    MulliganManager mulliganManager = this;
    mulliganManager.m_waitingForUserInput = isBGRefreshing;
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
      mulliganManager.DestroyMulliganTimer();
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
    {
      SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart28_CardDismissWoosh2_v2.prefab:6eb21cb332351ea419772cb5ae32772a");
      mulliganManager.DestroyXobjects();
    }
    else
      SoundManager.Get().LoadAndPlay((AssetReference) "BG_SelectHero.prefab:40cb8c418fca5f44391df4df2e9660cd");
    Vector3 mulliganedCardsPosition = Board.Get().FindBone("MulliganedCardsPosition").position;
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
    {
      mulliganManager.DestroyChooseBanner();
      mulliganManager.DestroyDetailLabel();
      mulliganManager.DestroyTagConditionalVFXs();
    }
    else
    {
      mulliganManager.m_UpdateChooseBanner = mulliganManager.UpdateChooseBanner();
      mulliganManager.StartCoroutine(mulliganManager.m_UpdateChooseBanner);
    }
    if (!(bool) UniversalInputManager.UsePhoneUI || GameState.Get().GetBooleanGameOption(GameEntityOption.SUPPRESS_CLASS_NAMES))
      Gameplay.Get().RemoveClassNames();
    foreach (Card startingCard in mulliganManager.m_startingCards)
    {
      startingCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
      startingCard.GetActor().ToggleForceIdle(true);
      startingCard.GetActor().TurnOffCollider();
    }
    mulliganManager.hisHeroCardActor.SetActorState(ActorStateType.CARD_IDLE);
    mulliganManager.hisHeroCardActor.ToggleForceIdle(true);
    Card heroPowerCard = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard();
    if ((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null && (UnityEngine.Object) heroPowerCard.GetActor() != (UnityEngine.Object) null)
    {
      heroPowerCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
      heroPowerCard.GetActor().ToggleForceIdle(true);
    }
    if (!isBGRefreshing)
    {
      if (mulliganManager.m_RemoveUIButtons != null)
        mulliganManager.StopCoroutine(mulliganManager.m_RemoveUIButtons);
      mulliganManager.m_RemoveUIButtons = mulliganManager.RemoveUIButtons();
      mulliganManager.StartCoroutine(mulliganManager.m_RemoveUIButtons);
    }
    float TO_DECK_ANIMATION_TIME = 1.5f;
    int i;
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE))
    {
      for (i = 0; i < mulliganManager.m_startingCards.Count; ++i)
      {
        if (mulliganManager.m_handCardsMarkedForReplace[i])
        {
          GameObject gameObject = mulliganManager.m_startingCards[i].gameObject;
          Vector3[] vector3Array = new Vector3[4]
          {
            gameObject.transform.position,
            new Vector3(gameObject.transform.position.x + 2f, gameObject.transform.position.y - 1.7f, gameObject.transform.position.z),
            new Vector3(mulliganedCardsPosition.x, mulliganedCardsPosition.y, mulliganedCardsPosition.z),
            mulliganManager.friendlySideDeck.transform.position
          };
          iTween.MoveTo(gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) TO_DECK_ANIMATION_TIME, (object) "easetype", (object) iTween.EaseType.easeOutCubic));
          Animation animation = gameObject.GetComponent<Animation>();
          if ((UnityEngine.Object) animation == (UnityEngine.Object) null)
            animation = gameObject.AddComponent<Animation>();
          animation.AddClip(mulliganManager.cardAnimatesFromBoardToDeck, "putCardBack");
          animation.Play("putCardBack");
          yield return (object) new WaitForSeconds(0.5f);
        }
      }
    }
    else if (isBGRefreshing)
    {
      for (i = 0; i < mulliganManager.m_startingCards.Count; ++i)
      {
        GameObject gameObject = mulliganManager.m_startingCards[i].gameObject;
        Vector3[] vector3Array = new Vector3[4]
        {
          gameObject.transform.position,
          new Vector3(gameObject.transform.position.x + 2f, gameObject.transform.position.y - 1.7f, gameObject.transform.position.z),
          new Vector3(mulliganedCardsPosition.x, mulliganedCardsPosition.y, mulliganedCardsPosition.z),
          mulliganManager.friendlySideDeck.transform.position
        };
        iTween.MoveTo(gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) TO_DECK_ANIMATION_TIME, (object) "easetype", (object) iTween.EaseType.easeOutCubic));
        Animation animation = gameObject.GetComponent<Animation>();
        if ((UnityEngine.Object) animation == (UnityEngine.Object) null)
          animation = gameObject.AddComponent<Animation>();
        animation.AddClip(mulliganManager.cardAnimatesFromBoardToDeck, "putCardBack");
        animation.Play("putCardBack");
        yield return (object) new WaitForSeconds(0.5f);
      }
    }
    if (isBGRefreshing)
      mulliganManager.RefreshBGHeroes();
    else if (!EndTurnButton.Get().IsDisabled)
      InputManager.Get().DoEndTurnButton();
    else
      GameState.Get().SendChoices();
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_IS_CHOOSE_ONE) | isBGRefreshing)
    {
      mulliganManager.friendlySideHandZone.AddInputBlocker();
      while (!mulliganManager.friendlyPlayerHasReplacementCards)
        yield return (object) null;
      mulliganManager.friendlySideHandZone.RemoveInputBlocker();
      mulliganManager.SortHand((Zone) mulliganManager.friendlySideHandZone);
      List<Card> handZoneCards = mulliganManager.friendlySideHandZone.GetCards();
      if (!isBGRefreshing)
      {
        foreach (Card card in handZoneCards)
        {
          if (!mulliganManager.IsCoinCard(card))
          {
            card.GetActor().SetActorState(ActorStateType.CARD_IDLE);
            card.GetActor().ToggleForceIdle(true);
            card.GetActor().TurnOffCollider();
          }
        }
      }
      else
      {
        mulliganManager.GetStartingLists();
        mulliganManager.SetupCardActor(ref mulliganManager.m_startingCards);
        while (mulliganManager.pendingHeroCount > 0)
          yield return (object) null;
        mulliganManager.SetupCardCollider(ref mulliganManager.m_startingCards);
      }
      Bounds bounds = mulliganManager.startingHandZone.GetComponent<Collider>().bounds;
      float x1 = bounds.size.x;
      if ((bool) UniversalInputManager.UsePhoneUI)
        x1 *= 0.55f;
      float spaceForEachCard = x1 / (float) mulliganManager.m_startingCards.Count;
      float leftSideOfZone = mulliganManager.startingHandZone.transform.position.x - x1 / 2f;
      float xOffset = 0.0f;
      if (isBGRefreshing)
      {
        int cardsToShowOnLeft = GameState.Get().GetGameEntity().GetNumberOfFakeMulliganCardsToShowOnLeft(mulliganManager.m_startingCards.Count);
        int cardsToShowOnRight = GameState.Get().GetGameEntity().GetNumberOfFakeMulliganCardsToShowOnRight(mulliganManager.m_startingCards.Count);
        spaceForEachCard = x1 / (float) (mulliganManager.m_startingCards.Count + cardsToShowOnLeft + cardsToShowOnRight);
        xOffset += (float) cardsToShowOnLeft * spaceForEachCard;
      }
      float cardHeightOffset = 0.0f;
      if ((bool) UniversalInputManager.UsePhoneUI)
        cardHeightOffset = 7f;
      float cardZpos = mulliganManager.startingHandZone.transform.position.z - 0.3f;
      if ((bool) UniversalInputManager.UsePhoneUI)
        cardZpos = mulliganManager.startingHandZone.transform.position.z - 0.2f;
      for (i = 0; i < mulliganManager.m_startingCards.Count; ++i)
      {
        if (mulliganManager.m_handCardsMarkedForReplace[i] | isBGRefreshing)
        {
          GameObject topCard = isBGRefreshing ? mulliganManager.m_startingCards[i].gameObject : handZoneCards[i].gameObject;
          iTween.Stop(topCard);
          GameObject target = topCard;
          object[] objArray = new object[4]
          {
            (object) "position",
            null,
            null,
            null
          };
          double x2 = (double) leftSideOfZone + (double) spaceForEachCard + (double) xOffset - (double) spaceForEachCard / 2.0;
          bounds = mulliganManager.friendlySideHandZone.GetComponent<Collider>().bounds;
          double y1 = (double) bounds.center.y;
          double z1 = (double) mulliganManager.startingHandZone.transform.position.z;
          objArray[1] = (object) new Vector3((float) x2, (float) y1, (float) z1);
          objArray[2] = (object) "time";
          objArray[3] = (object) 3f;
          Hashtable args = iTween.Hash(objArray);
          iTween.MoveTo(target, args);
          Vector3[] vector3Array1 = new Vector3[4];
          vector3Array1[0] = topCard.transform.position;
          vector3Array1[1] = new Vector3(mulliganedCardsPosition.x, mulliganedCardsPosition.y, mulliganedCardsPosition.z);
          Vector3[] vector3Array2 = vector3Array1;
          double x3 = (double) leftSideOfZone + (double) spaceForEachCard + (double) xOffset - (double) spaceForEachCard / 2.0;
          bounds = mulliganManager.friendlySideHandZone.GetComponent<Collider>().bounds;
          double y2 = (double) bounds.center.y + (double) cardHeightOffset;
          double z2 = (double) cardZpos;
          Vector3 vector3 = new Vector3((float) x3, (float) y2, (float) z2);
          vector3Array2[3] = vector3;
          vector3Array1[2] = new Vector3(vector3Array1[3].x + 2f, vector3Array1[3].y - 1.7f, vector3Array1[3].z);
          iTween.MoveTo(topCard, iTween.Hash((object) "path", (object) vector3Array1, (object) "time", (object) TO_DECK_ANIMATION_TIME, (object) "easetype", (object) iTween.EaseType.easeInCubic));
          if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
            iTween.ScaleTo(topCard, GameState.Get().GetGameEntity().GetAlternateMulliganActorScale(), MulliganManager.ANIMATION_TIME_DEAL_CARD);
          else
            iTween.ScaleTo(topCard, (Vector3) MulliganManager.FRIENDLY_PLAYER_CARD_SCALE, MulliganManager.ANIMATION_TIME_DEAL_CARD);
          Animation animation = topCard.GetComponent<Animation>();
          if ((UnityEngine.Object) animation == (UnityEngine.Object) null)
            animation = topCard.AddComponent<Animation>();
          string str = "putCardBack";
          animation.AddClip(mulliganManager.cardAnimatesFromBoardToDeck, str);
          animation[str].normalizedTime = 1f;
          animation[str].speed = -1f;
          animation.Play(str);
          yield return (object) new WaitForSeconds(0.5f);
          if ((UnityEngine.Object) topCard.GetComponent<AudioSource>() == (UnityEngine.Object) null)
            topCard.AddComponent<AudioSource>();
          SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart30_CardReplaceSingle.prefab:aa2b215965bf6484da413a795c17e995", topCard);
          topCard = (GameObject) null;
        }
        xOffset += spaceForEachCard;
      }
      yield return (object) new WaitForSeconds(1f);
      mulliganManager.ShuffleDeck();
      yield return (object) new WaitForSeconds(1.5f);
      handZoneCards = (List<Card>) null;
    }
    if (!isBGRefreshing)
    {
      if (mulliganManager.opponentPlayerHasReplacementCards && !GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
      {
        mulliganManager.EndMulligan();
      }
      else
      {
        mulliganManager.m_WaitForOpponentToFinishMulligan = mulliganManager.WaitForOpponentToFinishMulligan();
        mulliganManager.StartCoroutine(mulliganManager.WaitForOpponentToFinishMulligan());
      }
    }
  }

  private IEnumerator UpdateChooseBanner()
  {
    yield break;
  }

  private IEnumerator WaitForOpponentToFinishMulligan()
  {
    MulliganManager mulliganManager = this;
    mulliganManager.DestroyChooseBanner();
    mulliganManager.DestroyDetailLabel();
    mulliganManager.DestroyTagConditionalVFXs();
    Vector3 position1 = Board.Get().FindBone("ChoiceBanner").position;
    Vector3 position2;
    Vector3 scale;
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        position2 = new Vector3(position1.x, mulliganManager.friendlySideHandZone.transform.position.y + 1f, mulliganManager.myHeroCardActor.transform.position.z + 6.8f);
        scale = new Vector3(2.5f, 2.5f, 2.5f);
      }
      else
      {
        position2 = new Vector3(position1.x, mulliganManager.friendlySideHandZone.transform.position.y, mulliganManager.myHeroCardActor.transform.position.z + 0.4f);
        scale = new Vector3(1.4f, 1.4f, 1.4f);
      }
    }
    else
    {
      position2 = position1;
      scale = new Vector3(1.4f, 1.4f, 1.4f);
    }
    mulliganManager.mulliganChooseBanner = UnityEngine.Object.Instantiate<GameObject>(mulliganManager.GetChooseBannerPrefab(), position2, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
    mulliganManager.mulliganChooseBanner.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    iTween.ScaleTo(mulliganManager.mulliganChooseBanner, scale, 0.4f);
    mulliganManager.CreateTagConditionalVFXs(position2);
    Actor yourHeroActor = (Actor) null;
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
    {
      GameState.Get().GetGameEntity().GetMulliganWaitingText();
      GameState.Get().GetGameEntity().GetMulliganWaitingSubtitleText();
      while (GameState.Get().GetPlayerInfoMap()[GameState.Get().GetFriendlyPlayerId()].GetPlayerHero() == null)
      {
        string mulliganWaitingText = GameState.Get().GetGameEntity().GetMulliganWaitingText();
        string waitingSubtitleText = GameState.Get().GetGameEntity().GetMulliganWaitingSubtitleText();
        mulliganManager.SetMulliganBannerText(mulliganWaitingText, waitingSubtitleText);
        yield return (object) new WaitForSeconds(0.5f);
      }
      if (mulliganManager.m_startingCards.Count == 0)
      {
        mulliganManager.m_startingCards.Add(GameState.Get().GetFriendlySidePlayer().GetHeroCard());
        foreach (Card startingCard in mulliganManager.m_startingCards)
        {
          startingCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
          startingCard.GetActor().TurnOffCollider();
          startingCard.GetActor().GetMeshRenderer().gameObject.layer = 8;
          if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
          {
            ++mulliganManager.pendingHeroCount;
            startingCard.GetActor().gameObject.SetActive(false);
            AssetLoader.Get().InstantiatePrefab((AssetReference) GameState.Get().GetStringGameOption(GameEntityOption.ALTERNATE_MULLIGAN_ACTOR_NAME), new PrefabCallback<GameObject>(mulliganManager.OnHeroActorLoaded), (object) startingCard, AssetLoadingOptions.IgnorePrefabPosition);
          }
        }
        while (mulliganManager.pendingHeroCount > 0)
          yield return (object) null;
      }
      foreach (Card key in mulliganManager.choiceHeroActors.Keys)
      {
        if (key.GetEntity().GetCardId() == GameState.Get().GetPlayerInfoMap()[GameState.Get().GetFriendlyPlayerId()].GetPlayerHero().GetCardId())
        {
          float x = mulliganManager.startingHandZone.GetComponent<Collider>().bounds.size.x;
          if ((bool) UniversalInputManager.UsePhoneUI)
            x *= 0.55f;
          double num1 = (double) x;
          float num2 = mulliganManager.startingHandZone.transform.position.x - x / 2f;
          float num3 = 0.0f;
          if ((bool) UniversalInputManager.UsePhoneUI)
            num3 = 7f;
          float z = mulliganManager.startingHandZone.transform.position.z - 0.3f;
          if ((bool) UniversalInputManager.UsePhoneUI)
            z = mulliganManager.startingHandZone.transform.position.z - 0.2f;
          float num4 = (float) (num1 / 2.0);
          GameObject gameObject = key.gameObject;
          if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
          {
            gameObject = mulliganManager.choiceHeroActors[key].gameObject.transform.parent.gameObject;
            yourHeroActor = mulliganManager.choiceHeroActors[key];
            yourHeroActor.GetCard().SetActor(yourHeroActor);
            yourHeroActor.GetCard().GetActor().Show();
            GameState.Get().GetGameEntity().ApplyMulliganActorLobbyStateChanges(yourHeroActor);
            ((PlayerLeaderboardMainCardActor) yourHeroActor).UpdatePlayerNameText(GameState.Get().GetGameEntity().GetBestNameForPlayer(GameState.Get().GetFriendlySidePlayer().GetPlayerId()));
            mulliganManager.myHeroCardActor = yourHeroActor;
          }
          iTween.Stop(gameObject);
          Vector3[] vector3Array = new Vector3[3]
          {
            gameObject.transform.position,
            new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 3.6f, gameObject.transform.position.z),
            new Vector3(num2 + num4, mulliganManager.friendlySideHandZone.transform.position.y + num3, z)
          };
          iTween.MoveTo(gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
          if ((bool) UniversalInputManager.UsePhoneUI)
            iTween.ScaleTo(gameObject, new Vector3(0.9f, 1.1f, 0.9f), MulliganManager.ANIMATION_TIME_DEAL_CARD);
          else
            iTween.ScaleTo(gameObject, new Vector3(1.2f, 1.1f, 1.2f), MulliganManager.ANIMATION_TIME_DEAL_CARD);
          iTween.RotateTo(gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "delay", (object) (float) ((double) MulliganManager.ANIMATION_TIME_DEAL_CARD / 16.0)));
        }
        else if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
        {
          if (!mulliganManager.choiceHeroActors.ContainsKey(key))
          {
            Debug.LogError((object) ("MulliganManager - ChoiceHeroActors doesn't contain card: " + key.name));
          }
          else
          {
            mulliganManager.choiceHeroActors[key].ActivateSpellBirthState(SpellType.DEATH);
            ((PlayerLeaderboardMainCardActor) mulliganManager.choiceHeroActors[key]).m_fullSelectionHighlight.SetActive(false);
          }
        }
        else
          key.FakeDeath();
      }
      mulliganManager.CleanupFakeCards();
      bool heroPowerCreated = false;
      do
      {
        if (!heroPowerCreated)
        {
          Card heroPowerCard = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard();
          if ((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null && (UnityEngine.Object) heroPowerCard.GetActor() != (UnityEngine.Object) null)
          {
            heroPowerCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
            heroPowerCard.GetActor().ToggleForceIdle(true);
            heroPowerCard.GetActor().TurnOffCollider();
            heroPowerCreated = true;
          }
        }
        string mulliganWaitingText = GameState.Get().GetGameEntity().GetMulliganWaitingText();
        string waitingSubtitleText = GameState.Get().GetGameEntity().GetMulliganWaitingSubtitleText();
        mulliganManager.SetMulliganBannerText(mulliganWaitingText, waitingSubtitleText);
        yield return (object) null;
      }
      while (!GameState.Get().GetGameEntity().IsHeroMulliganLobbyFinished());
      foreach (SharedPlayerInfo sph in GameState.Get().GetPlayerInfoMap().Values)
      {
        if (sph.GetPlayerId() != GameState.Get().GetFriendlyPlayerId())
        {
          while (sph.GetPlayerHero() == null)
            yield return (object) null;
          ++mulliganManager.pendingHeroCount;
          AssetLoader.Get().InstantiatePrefab((AssetReference) GameState.Get().GetStringGameOption(GameEntityOption.ALTERNATE_MULLIGAN_LOBBY_ACTOR_NAME), new PrefabCallback<GameObject>(mulliganManager.OnOpponentHeroActorLoaded), (object) sph.GetPlayerHero().GetCard(), AssetLoadingOptions.IgnorePrefabPosition);
        }
      }
      while (mulliganManager.pendingHeroCount > 0)
        yield return (object) null;
      yield return (object) new WaitForSeconds(0.5f);
      mulliganManager.DestroyMulliganTimer();
      mulliganManager.DestroyChooseBanner();
      mulliganManager.DestroyDetailLabel();
      mulliganManager.DestroyTagConditionalVFXs();
      Transform rootTransform = yourHeroActor.gameObject.transform.parent.parent;
      Transform yourHeroRoot = yourHeroActor.gameObject.transform.parent;
      Vector3 vsPosition = Board.Get().FindBone("VS_Position").position;
      yield return (object) new WaitForSeconds(1f);
      iTween.Stop(yourHeroRoot.gameObject);
      int num5 = 1;
      foreach (Actor actor in mulliganManager.opponentHeroActors.Values)
      {
        actor.gameObject.transform.parent = rootTransform;
        actor.gameObject.transform.localScale = new Vector3(1.0506f, 1.0506f, 1.0506f);
        actor.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Vector3 position3 = Board.Get().FindBone("HeroSpawnLineUp0" + num5++.ToString()).position;
        actor.gameObject.transform.position = position3;
        ((PlayerLeaderboardMainCardActor) actor).SetAlternateNameTextActive(false);
        SharedPlayerInfo playerForCard = mulliganManager.GetPlayerForCard(actor.GetCard());
        if (playerForCard != null)
          ((PlayerLeaderboardMainCardActor) actor).UpdatePlayerNameText(GameState.Get().GetGameEntity().GetBestNameForPlayer(playerForCard.GetPlayerId()));
      }
      yourHeroActor.transform.parent = (Transform) null;
      yourHeroRoot.position = new Vector3(-7.7726f, 0.0055918f, -8.054f);
      yourHeroRoot.localScale = new Vector3(1.134f, 1.134f, 1.134f);
      yourHeroActor.transform.parent = yourHeroRoot;
      yourHeroActor.GetComponent<PlayMakerFSM>().SendEvent((bool) UniversalInputManager.UsePhoneUI ? "SlotInHeroAfterFlyIn_Phone" : "SlotInHeroAfterFlyIn");
      yield return (object) new WaitForSeconds(1f);
      if ((bool) (UnityEngine.Object) mulliganManager.versusText)
      {
        mulliganManager.versusText.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        mulliganManager.versusText.transform.position = vsPosition;
      }
      yield return (object) new WaitForSeconds(1.5f);
      int num6 = 1;
      foreach (Component component1 in mulliganManager.opponentHeroActors.Values)
      {
        PlayMakerFSM component2 = component1.GetComponent<PlayMakerFSM>();
        component2.FsmVariables.GetFsmInt("Player").Value = num6++;
        component2.SendEvent((bool) UniversalInputManager.UsePhoneUI ? "Spawn_Phone" : "Spawn");
      }
      yield return (object) new WaitForSeconds(1.5f);
      if ((bool) (UnityEngine.Object) mulliganManager.versusText)
      {
        yield return (object) new WaitForSeconds(0.1f);
        mulliganManager.versusText.FadeOut();
        yield return (object) new WaitForSeconds(0.32f);
      }
      foreach (Component component in mulliganManager.opponentHeroActors.Values)
        component.GetComponent<PlayMakerFSM>().SendEvent((bool) UniversalInputManager.UsePhoneUI ? "FlyIn_Phone" : "FlyIn");
      if ((UnityEngine.Object) PlayerLeaderboardManager.Get() != (UnityEngine.Object) null)
        PlayerLeaderboardManager.Get().UpdateLayout(false);
      yield return (object) new WaitForSeconds(1.5f);
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
        GameState.Get().GetGameEntity().ClearMulliganActorStateChanges(yourHeroActor);
      foreach (Component component in mulliganManager.opponentHeroActors.Values)
        component.gameObject.SetActive(false);
      rootTransform = (Transform) null;
      yourHeroRoot = (Transform) null;
      vsPosition = new Vector3();
    }
    else
    {
      mulliganManager.SetMulliganBannerText(GameStrings.Get("GAMEPLAY_MULLIGAN_WAITING"));
      mulliganManager.mulliganChooseBanner.GetComponent<Banner>().MoveGlowForBottomPlacement();
      while (!mulliganManager.opponentPlayerHasReplacementCards && !GameState.Get().IsGameOver())
        yield return (object) null;
    }
    mulliganManager.EndMulligan();
  }

  private SharedPlayerInfo GetPlayerForCard(Card card)
  {
    foreach (SharedPlayerInfo playerForCard in GameState.Get().GetPlayerInfoMap().Values)
    {
      if (card.GetEntity().GetCardId() == playerForCard.GetPlayerHero().GetCardId())
        return playerForCard;
    }
    return (SharedPlayerInfo) null;
  }

  private void SetMulliganBannerText(string title) => this.SetMulliganBannerText(title, (string) null);

  private void SetMulliganBannerText(string title, string subtitle)
  {
    if ((UnityEngine.Object) this.mulliganChooseBanner == (UnityEngine.Object) null)
      return;
    if (subtitle != null)
      this.mulliganChooseBanner.GetComponent<Banner>().SetText(title, subtitle);
    else
      this.mulliganChooseBanner.GetComponent<Banner>().SetText(title);
  }

  private void SetMulliganDetailLabelText(string title)
  {
    if ((UnityEngine.Object) this.mulliganDetailLabel == (UnityEngine.Object) null)
      return;
    this.mulliganDetailLabel.GetComponent<UberText>().Text = title;
  }

  private void ShowMulliganDetail()
  {
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.DISPLAY_MULLIGAN_DETAIL_LABEL))
      return;
    string mulliganDetailText = GameState.Get().GetGameEntity().GetMulliganDetailText();
    if (mulliganDetailText == null)
      return;
    this.mulliganDetailLabel = UnityEngine.Object.Instantiate<GameObject>(this.mulliganDetailLabelPrefab);
    this.mulliganDetailLabel.transform.position = Board.Get().FindBone("MulliganDetail").position;
    this.SetMulliganDetailLabelText(mulliganDetailText);
  }

  private IEnumerator RemoveUIButtons()
  {
    MulliganManager mulliganManager = this;
    if ((UnityEngine.Object) mulliganManager.mulliganButton != (UnityEngine.Object) null)
      mulliganManager.mulliganButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Death");
    if ((UnityEngine.Object) mulliganManager.mulliganButtonWidget != (UnityEngine.Object) null)
      mulliganManager.mulliganButtonWidget.gameObject.SetActive(false);
    if ((UnityEngine.Object) mulliganManager.m_refreshButton != (UnityEngine.Object) null)
      mulliganManager.m_refreshButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Death");
    if (mulliganManager.m_replaceLabels != null)
    {
      for (int i = 0; i < mulliganManager.m_replaceLabels.Count; ++i)
      {
        if ((UnityEngine.Object) mulliganManager.m_replaceLabels[i] != (UnityEngine.Object) null)
        {
          iTween.RotateTo(mulliganManager.m_replaceLabels[i].gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeInExpo));
          iTween.ScaleTo(mulliganManager.m_replaceLabels[i].gameObject, iTween.Hash((object) "scale", (object) new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeInExpo, (object) "oncomplete", (object) "DestroyButton", (object) "oncompletetarget", (object) mulliganManager.gameObject, (object) "oncompleteparams", (object) mulliganManager.m_replaceLabels[i]));
          yield return (object) new WaitForSeconds(0.05f);
        }
      }
    }
    yield return (object) new WaitForSeconds(3.5f);
    if ((UnityEngine.Object) mulliganManager.mulliganButton != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) mulliganManager.mulliganButton.gameObject);
    if ((UnityEngine.Object) mulliganManager.mulliganButtonWidget != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) mulliganManager.mulliganButtonWidget.gameObject);
    if ((UnityEngine.Object) mulliganManager.m_refreshButton != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) mulliganManager.m_refreshButton.gameObject);
  }

  private void DestroyButton(UnityEngine.Object buttonToDestroy) => UnityEngine.Object.Destroy(buttonToDestroy);

  private void HandleGameOverDuringMulligan()
  {
    if (this.m_WaitForBoardThenLoadButton != null)
      this.StopCoroutine(this.m_WaitForBoardThenLoadButton);
    this.m_WaitForBoardThenLoadButton = (IEnumerator) null;
    if (this.m_WaitForHeroesAndStartAnimations != null)
      this.StopCoroutine(this.m_WaitForHeroesAndStartAnimations);
    this.m_WaitForHeroesAndStartAnimations = (IEnumerator) null;
    if (this.m_ResumeMulligan != null)
      this.StopCoroutine(this.m_ResumeMulligan);
    this.m_ResumeMulligan = (IEnumerator) null;
    if (this.m_DealStartingCards != null)
      this.StopCoroutine(this.m_DealStartingCards);
    this.m_DealStartingCards = (IEnumerator) null;
    if (this.m_ShowMultiplayerWaitingArea != null)
      this.StopCoroutine(this.m_ShowMultiplayerWaitingArea);
    this.m_ShowMultiplayerWaitingArea = (IEnumerator) null;
    if (this.m_RemoveOldCardsAnimation != null)
      this.StopCoroutine(this.m_RemoveOldCardsAnimation);
    this.m_RemoveOldCardsAnimation = (IEnumerator) null;
    if (this.m_PlayStartingTaunts != null)
      this.StopCoroutine(this.m_PlayStartingTaunts);
    this.m_PlayStartingTaunts = (IEnumerator) null;
    if (this.m_Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen != null)
      this.StopCoroutine(this.m_Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen);
    this.m_Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen = (IEnumerator) null;
    if (this.m_ContinueMulliganWhenBoardLoads != null)
      this.StopCoroutine(this.m_ContinueMulliganWhenBoardLoads);
    this.m_ContinueMulliganWhenBoardLoads = (IEnumerator) null;
    if (this.m_WaitAFrameBeforeSendingEventToMulliganButton != null)
      this.StopCoroutine(this.m_WaitAFrameBeforeSendingEventToMulliganButton);
    this.m_WaitAFrameBeforeSendingEventToMulliganButton = (IEnumerator) null;
    if (this.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton != null)
      this.StopCoroutine(this.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton);
    this.m_WaitAFrameBeforeSendingEventToMulliganRefreshButton = (IEnumerator) null;
    if (this.m_ShrinkStartingHandBanner != null)
      this.StopCoroutine(this.m_ShrinkStartingHandBanner);
    this.m_ShrinkStartingHandBanner = (IEnumerator) null;
    if (this.m_AnimateCoinTossText != null)
      this.StopCoroutine(this.m_AnimateCoinTossText);
    this.m_AnimateCoinTossText = (IEnumerator) null;
    if (this.m_WaitForOpponentToFinishMulligan != null)
      this.StopCoroutine(this.m_WaitForOpponentToFinishMulligan);
    this.m_WaitForOpponentToFinishMulligan = (IEnumerator) null;
    if (this.m_EndMulliganWithTiming != null)
      this.StopCoroutine(this.m_EndMulliganWithTiming);
    this.m_EndMulliganWithTiming = (IEnumerator) null;
    if (this.m_HandleCoinCard != null)
      this.StopCoroutine(this.m_HandleCoinCard);
    this.m_HandleCoinCard = (IEnumerator) null;
    if (this.m_EnableHandCollidersAfterCardsAreDealt != null)
      this.StopCoroutine(this.m_EnableHandCollidersAfterCardsAreDealt);
    this.m_EnableHandCollidersAfterCardsAreDealt = (IEnumerator) null;
    if (this.m_SkipMulliganForResume != null)
      this.StopCoroutine(this.m_SkipMulliganForResume);
    this.m_SkipMulliganForResume = (IEnumerator) null;
    if (this.m_SkipMulliganWhenIntroComplete != null)
      this.StopCoroutine(this.m_SkipMulliganWhenIntroComplete);
    this.m_SkipMulliganWhenIntroComplete = (IEnumerator) null;
    if (this.m_WaitForBoardAnimToCompleteThenStartTurn != null)
      this.StopCoroutine(this.m_WaitForBoardAnimToCompleteThenStartTurn);
    this.m_WaitForBoardAnimToCompleteThenStartTurn = (IEnumerator) null;
    if (this.m_customIntroCoroutine != null)
    {
      this.StopCoroutine(this.m_customIntroCoroutine);
      GameState.Get().GetGameEntity().OnCustomIntroCancelled(this.myHeroCardActor.GetCard(), this.hisHeroCardActor.GetCard(), this.myheroLabel, this.hisheroLabel, this.versusText);
      this.m_customIntroCoroutine = (Coroutine) null;
    }
    this.m_waitingForUserInput = false;
    this.DestroyXobjects();
    this.DestroyChooseBanner();
    this.DestroyDetailLabel();
    this.DestroyMulliganTimer();
    this.DestroyTagConditionalVFXs();
    if ((UnityEngine.Object) this.coinObject != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.coinObject);
    if ((UnityEngine.Object) this.versusText != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.versusText.gameObject);
    if ((UnityEngine.Object) this.versusVo != (UnityEngine.Object) null)
      SoundManager.Get().Destroy(this.versusVo);
    if ((UnityEngine.Object) this.coinTossText != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.coinTossText);
    if ((bool) UniversalInputManager.UsePhoneUI)
      Gameplay.Get().RemoveNameBanners();
    else
      Gameplay.Get().RemoveClassNames();
    if (this.m_RemoveUIButtons != null)
      this.StopCoroutine(this.m_RemoveUIButtons);
    this.m_RemoveUIButtons = this.RemoveUIButtons();
    this.StartCoroutine(this.m_RemoveUIButtons);
    if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
      this.mulliganButton.SetEnabled(false);
    if ((UnityEngine.Object) this.mulliganButtonWidget != (UnityEngine.Object) null)
      this.mulliganButtonWidget.SetEnabled(false);
    if ((UnityEngine.Object) this.m_refreshButton != (UnityEngine.Object) null)
      this.m_refreshButton.SetEnabled(false);
    this.DestoryHeroSkinSocketInEffects();
    if ((UnityEngine.Object) this.myheroLabel != (UnityEngine.Object) null && this.myheroLabel.isActiveAndEnabled)
      this.myheroLabel.FadeOut();
    if ((UnityEngine.Object) this.hisheroLabel != (UnityEngine.Object) null && this.hisheroLabel.isActiveAndEnabled)
      this.hisheroLabel.FadeOut();
    if ((UnityEngine.Object) this.friendlySideHandZone != (UnityEngine.Object) null)
    {
      foreach (Card card in this.friendlySideHandZone.GetCards())
      {
        Actor actor = card.GetActor();
        actor.SetActorState(ActorStateType.CARD_IDLE);
        actor.ToggleForceIdle(true);
        if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS) && GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
        {
          actor.ActivateSpellBirthState(SpellType.DEATH);
          ((PlayerLeaderboardMainCardActor) actor).m_fullSelectionHighlight.SetActive(false);
        }
      }
      if ((UnityEngine.Object) this.hisHeroCardActor != (UnityEngine.Object) null)
      {
        this.hisHeroCardActor.SetActorState(ActorStateType.CARD_IDLE);
        this.hisHeroCardActor.ToggleForceIdle(true);
      }
      Card heroPowerCard = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard();
      if ((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null && (UnityEngine.Object) heroPowerCard.GetActor() != (UnityEngine.Object) null)
      {
        heroPowerCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
        heroPowerCard.GetActor().ToggleForceIdle(true);
      }
      if (!this.friendlyPlayerGoesFirst && this.ShouldHandleCoinCard())
      {
        Card fromFriendlyHand = this.GetCoinCardFromFriendlyHand();
        fromFriendlyHand.SetDoNotSort(false);
        fromFriendlyHand.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
        this.PutCoinCardInSpawnPosition(fromFriendlyHand);
        fromFriendlyHand.GetActor().Show();
      }
      this.friendlySideHandZone.ForceStandInUpdate();
      this.friendlySideHandZone.SetDoNotUpdateLayout(false);
      this.friendlySideHandZone.UpdateLayout();
    }
    this.CleanupFakeCards();
    Board board = Board.Get();
    if ((UnityEngine.Object) board != (UnityEngine.Object) null)
      board.RaiseTheLightsQuickly();
    if ((UnityEngine.Object) this.myHeroCardActor != (UnityEngine.Object) null)
    {
      Animation component = this.myHeroCardActor.gameObject.GetComponent<Animation>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.Stop();
      this.myHeroCardActor.transform.localScale = Vector3.one;
      this.myHeroCardActor.transform.rotation = Quaternion.identity;
      this.myHeroCardActor.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).transform.position;
    }
    if (!((UnityEngine.Object) this.hisHeroCardActor != (UnityEngine.Object) null))
      return;
    Animation component1 = this.hisHeroCardActor.gameObject.GetComponent<Animation>();
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      component1.Stop();
    this.hisHeroCardActor.transform.localScale = Vector3.one;
    this.hisHeroCardActor.transform.rotation = Quaternion.identity;
    this.hisHeroCardActor.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).transform.position;
  }

  private void CleanupFakeCards()
  {
    List<Actor> actorList = new List<Actor>(this.fakeCardsOnLeft.Count + this.fakeCardsOnRight.Count);
    actorList.AddRange((IEnumerable<Actor>) this.fakeCardsOnLeft);
    actorList.AddRange((IEnumerable<Actor>) this.fakeCardsOnRight);
    foreach (Actor actor in actorList)
    {
      actor.ActivateSpellBirthState(SpellType.DEATH);
      actor.TurnOffCollider();
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS))
        GameState.Get().GetGameEntity().ConfigureFakeMulliganCardActor(actor, false);
    }
    if (!((UnityEngine.Object) this.conditionalHelperTextLabel != (UnityEngine.Object) null))
      return;
    this.conditionalHelperTextLabel.gameObject.SetActive(false);
  }

  public void EndMulligan()
  {
    this.m_waitingForUserInput = false;
    if (this.m_replaceLabels != null)
    {
      for (int index = 0; index < this.m_replaceLabels.Count; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_replaceLabels[index]);
    }
    if ((UnityEngine.Object) this.mulliganButton != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.mulliganButton.gameObject);
    if ((UnityEngine.Object) this.mulliganButtonWidget != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.mulliganButtonWidget.gameObject);
    if ((UnityEngine.Object) this.m_refreshButton != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_refreshButton.gameObject);
    this.DestroyXobjects();
    this.DestroyChooseBanner();
    this.DestroyDetailLabel();
    this.DestroyTagConditionalVFXs();
    if ((UnityEngine.Object) this.versusText != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.versusText.gameObject);
    if ((UnityEngine.Object) this.versusVo != (UnityEngine.Object) null)
      SoundManager.Get().Destroy(this.versusVo);
    if ((UnityEngine.Object) this.coinTossText != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.coinTossText);
    if ((UnityEngine.Object) this.hisheroLabel != (UnityEngine.Object) null)
      this.hisheroLabel.FadeOut();
    if ((UnityEngine.Object) this.myheroLabel != (UnityEngine.Object) null)
      this.myheroLabel.FadeOut();
    this.DestoryHeroSkinSocketInEffects();
    this.myHeroCardActor.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    this.hisHeroCardActor.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    this.myHeroCardActor.Show();
    if (GameState.Get().IsGameOver())
      return;
    this.myHeroCardActor.GetHealthObject().Show();
    this.hisHeroCardActor.GetHealthObject().Show();
    if ((UnityEngine.Object) this.myHeroCardActor.GetAttackObject() != (UnityEngine.Object) null)
      this.myHeroCardActor.GetAttackObject().Show();
    if ((UnityEngine.Object) this.hisHeroCardActor.GetAttackObject() != (UnityEngine.Object) null)
      this.hisHeroCardActor.GetAttackObject().Show();
    this.friendlySideHandZone.ForceStandInUpdate();
    this.friendlySideHandZone.SetDoNotUpdateLayout(false);
    this.friendlySideHandZone.UpdateLayout();
    if (this.m_startingOppCards != null && this.m_startingOppCards.Count > 0)
      this.m_startingOppCards[this.m_startingOppCards.Count - 1].SetDoNotSort(false);
    this.opposingSideHandZone.SetDoNotUpdateLayout(false);
    this.opposingSideHandZone.UpdateLayout();
    this.friendlySideDeck.SetSuppressEmotes(false);
    this.opposingSideDeck.SetSuppressEmotes(false);
    Board.Get().SplitSurface();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      Gameplay.Get().RemoveNameBanners();
      Gameplay.Get().AddGamePlayNameBannerPhone();
    }
    if ((UnityEngine.Object) this.m_MyCustomSocketInSpell != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_MyCustomSocketInSpell);
    if ((UnityEngine.Object) this.m_HisCustomSocketInSpell != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_HisCustomSocketInSpell);
    this.m_EndMulliganWithTiming = this.EndMulliganWithTiming();
    this.StartCoroutine(this.m_EndMulliganWithTiming);
  }

  private IEnumerator EndMulliganWithTiming()
  {
    MulliganManager mulliganManager = this;
    if (mulliganManager.ShouldHandleCoinCard())
    {
      mulliganManager.m_HandleCoinCard = mulliganManager.HandleCoinCard();
      yield return (object) mulliganManager.StartCoroutine(mulliganManager.m_HandleCoinCard);
    }
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) mulliganManager.coinObject);
    mulliganManager.myHeroCardActor.TurnOnCollider();
    mulliganManager.hisHeroCardActor.TurnOnCollider();
    mulliganManager.FadeOutMulliganMusicAndStartGameplayMusic();
    foreach (Card card in mulliganManager.friendlySideHandZone.GetCards())
    {
      card.GetActor().TurnOnCollider();
      card.GetActor().ToggleForceIdle(false);
    }
    mulliganManager.myHeroCardActor.ToggleForceIdle(false);
    mulliganManager.hisHeroCardActor.ToggleForceIdle(false);
    Card heroPowerCard = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard();
    if ((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null && (UnityEngine.Object) heroPowerCard.GetActor() != (UnityEngine.Object) null)
      heroPowerCard.GetActor().ToggleForceIdle(false);
    if (!mulliganManager.friendlyPlayerHasReplacementCards)
    {
      mulliganManager.m_EnableHandCollidersAfterCardsAreDealt = mulliganManager.EnableHandCollidersAfterCardsAreDealt();
      mulliganManager.StartCoroutine(mulliganManager.m_EnableHandCollidersAfterCardsAreDealt);
    }
    Board.Get().FindCollider("DragPlane").enabled = true;
    mulliganManager.ForceMulliganActive(false);
    Board.Get().RaiseTheLights();
    mulliganManager.FadeHeroPowerIn(GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard());
    mulliganManager.FadeHeroPowerIn(GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard());
    InputManager.Get().OnMulliganEnded();
    EndTurnButton.Get().OnMulliganEnded();
    GameState.Get().GetGameEntity().NotifyOfMulliganEnded();
    mulliganManager.m_WaitForBoardAnimToCompleteThenStartTurn = mulliganManager.WaitForBoardAnimToCompleteThenStartTurn();
    mulliganManager.StartCoroutine(mulliganManager.m_WaitForBoardAnimToCompleteThenStartTurn);
  }

  private IEnumerator HandleCoinCard()
  {
    MulliganManager mulliganManager = this;
    if (!mulliganManager.friendlyPlayerGoesFirst)
    {
      if ((UnityEngine.Object) mulliganManager.coinObject != (UnityEngine.Object) null && mulliganManager.coinObject.activeSelf)
      {
        yield return (object) new WaitForSeconds(0.5f);
        mulliganManager.coinObject.GetComponentInChildren<PlayMakerFSM>().SendEvent("Birth");
        yield return (object) new WaitForSeconds(0.1f);
      }
      if (!GameMgr.Get().IsSpectator() && !Options.Get().GetBool(Option.HAS_SEEN_THE_COIN, false) && UserAttentionManager.CanShowAttentionGrabber("MulliganManager.HandleCoinCard:" + (object) Option.HAS_SEEN_THE_COIN))
      {
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_COIN_INTRO"), "VO_INNKEEPER_COIN_INTRO.prefab:6fb1b3b124d474c4c84e392646caada4");
        Options.Get().SetBool(Option.HAS_SEEN_THE_COIN, true);
      }
      Card fromFriendlyHand = mulliganManager.GetCoinCardFromFriendlyHand();
      mulliganManager.PutCoinCardInSpawnPosition(fromFriendlyHand);
      fromFriendlyHand.ActivateActorSpell(SpellType.SUMMON_IN, new Spell.FinishedCallback(mulliganManager.CoinCardSummonFinishedCallback));
      yield return (object) new WaitForSeconds(1f);
    }
    else
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) mulliganManager.coinObject);
      if (mulliganManager.m_coinCardIndex >= 0)
        mulliganManager.m_startingOppCards[mulliganManager.m_coinCardIndex].SetDoNotSort(false);
      mulliganManager.opposingSideHandZone.UpdateLayout();
    }
  }

  private bool IsCoinCard(Card card) => card.GetEntity().GetCardId() == CoinManager.Get().GetFavoriteCoinCardId();

  private Card GetCoinCardFromFriendlyHand()
  {
    List<Card> cards = this.friendlySideHandZone.GetCards();
    if (cards.Count > 0)
      return cards[cards.Count - 1];
    Debug.LogError((object) "GetCoinCardFromFriendlyHand() failed. friendlySideHandZone is empty.");
    return (Card) null;
  }

  private void PutCoinCardInSpawnPosition(Card coinCard)
  {
    coinCard.transform.position = Board.Get().FindBone("MulliganCoinCardSpawnPosition").position;
    coinCard.transform.localScale = Board.Get().FindBone("MulliganCoinCardSpawnPosition").localScale;
  }

  private bool ShouldHandleCoinCard() => GameState.Get().IsMulliganPhase() && GameState.Get().GetBooleanGameOption(GameEntityOption.HANDLE_COIN);

  private void CoinCardSummonFinishedCallback(Spell spell, object userData)
  {
    Card componentInParents = GameObjectUtils.FindComponentInParents<Card>((Component) spell);
    componentInParents.RefreshActor();
    componentInParents.UpdateActorComponents();
    componentInParents.SetDoNotSort(false);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.coinObject);
    componentInParents.SetTransitionStyle(ZoneTransitionStyle.VERY_SLOW);
    this.friendlySideHandZone.UpdateLayout((Card) null, true);
  }

  private IEnumerator EnableHandCollidersAfterCardsAreDealt()
  {
    while (!this.friendlyPlayerHasReplacementCards)
      yield return (object) null;
    foreach (Card card in this.friendlySideHandZone.GetCards())
      card.GetActor().TurnOnCollider();
  }

  public void SkipCardChoosing()
  {
    this.skipCardChoosing = true;
    this.EnableDamageCapFX(true);
  }

  public void SkipMulliganForDev()
  {
    if (this.m_WaitForBoardThenLoadButton != null)
      this.StopCoroutine(this.m_WaitForBoardThenLoadButton);
    this.m_WaitForBoardThenLoadButton = (IEnumerator) null;
    if (this.m_WaitForHeroesAndStartAnimations != null)
      this.StopCoroutine(this.m_WaitForHeroesAndStartAnimations);
    this.m_WaitForHeroesAndStartAnimations = (IEnumerator) null;
    if (this.m_DealStartingCards != null)
      this.StopCoroutine(this.m_DealStartingCards);
    this.m_DealStartingCards = (IEnumerator) null;
    if (this.m_ShowMultiplayerWaitingArea != null)
      this.StopCoroutine(this.m_ShowMultiplayerWaitingArea);
    this.m_ShowMultiplayerWaitingArea = (IEnumerator) null;
    this.EndMulligan();
  }

  private IEnumerator SkipMulliganForResume()
  {
    MulliganManager mulliganManager = this;
    mulliganManager.introComplete = true;
    mulliganManager.ForceMulliganActive(false);
    SoundDucker ducker = (SoundDucker) null;
    if (!GameMgr.Get().IsSpectator())
    {
      ducker = mulliganManager.gameObject.AddComponent<SoundDucker>();
      ducker.m_DuckedCategoryDefs = new List<SoundDuckedCategoryDef>();
      foreach (Global.SoundCategory soundCategory in Enum.GetValues(typeof (Global.SoundCategory)))
      {
        switch (soundCategory)
        {
          case Global.SoundCategory.MUSIC:
          case Global.SoundCategory.AMBIENCE:
            continue;
          default:
            ducker.m_DuckedCategoryDefs.Add(new SoundDuckedCategoryDef()
            {
              m_Category = soundCategory,
              m_Volume = 0.0f,
              m_RestoreSec = 5f,
              m_BeginSec = 0.0f
            });
            continue;
        }
      }
      ducker.StartDucking();
    }
    while ((UnityEngine.Object) Board.Get() == (UnityEngine.Object) null)
      yield return (object) null;
    Board.Get().RaiseTheLightsQuickly();
    while ((UnityEngine.Object) ZoneMgr.Get() == (UnityEngine.Object) null)
      yield return (object) null;
    mulliganManager.InitZones();
    Collider dragPlane = Board.Get().FindCollider("DragPlane");
    mulliganManager.friendlySideHandZone.SetDoNotUpdateLayout(false);
    mulliganManager.opposingSideHandZone.SetDoNotUpdateLayout(false);
    dragPlane.enabled = false;
    mulliganManager.friendlySideHandZone.AddInputBlocker();
    mulliganManager.opposingSideHandZone.AddInputBlocker();
    while (!GameState.Get().IsGameCreated())
      yield return (object) null;
    while (ZoneMgr.Get().HasActiveServerChange())
      yield return (object) null;
    GameState.Get().GetGameEntity().NotifyOfMulliganInitialized();
    SceneMgr.Get().NotifySceneLoaded();
    while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
      yield return (object) null;
    if ((UnityEngine.Object) ducker != (UnityEngine.Object) null)
    {
      ducker.StopDucking();
      UnityEngine.Object.Destroy((UnityEngine.Object) ducker);
    }
    if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY)
      mulliganManager.FadeOutMulliganMusicAndStartGameplayMusic();
    dragPlane.enabled = true;
    mulliganManager.friendlySideHandZone.RemoveInputBlocker();
    mulliganManager.opposingSideHandZone.RemoveInputBlocker();
    mulliganManager.friendlySideDeck.SetSuppressEmotes(false);
    mulliganManager.opposingSideDeck.SetSuppressEmotes(false);
    if (GameState.Get().GetResponseMode() == GameState.ResponseMode.CHOICE)
      GameState.Get().UpdateChoiceHighlights();
    else if (GameState.Get().GetResponseMode() == GameState.ResponseMode.OPTION)
      GameState.Get().UpdateOptionHighlights();
    GameMgr.Get().UpdatePresence();
    InputManager.Get().OnMulliganEnded();
    EndTurnButton.Get().OnMulliganEnded();
    GameState.Get().GetGameEntity().NotifyOfMulliganEnded();
    UnityEngine.Object.Destroy((UnityEngine.Object) mulliganManager.gameObject);
  }

  public void SkipMulligan()
  {
    Gameplay.Get().RemoveClassNames();
    this.m_SkipMulliganWhenIntroComplete = this.SkipMulliganWhenIntroComplete();
    this.StartCoroutine(this.m_SkipMulliganWhenIntroComplete);
  }

  private IEnumerator SkipMulliganWhenIntroComplete()
  {
    MulliganManager mulliganManager = this;
    mulliganManager.m_waitingForUserInput = false;
    while (!mulliganManager.introComplete)
      yield return (object) null;
    mulliganManager.myHeroCardActor?.TurnOnCollider();
    mulliganManager.hisHeroCardActor?.TurnOnCollider();
    mulliganManager.FadeOutMulliganMusicAndStartGameplayMusic();
    mulliganManager.myHeroCardActor?.GetHealthObject().Show();
    mulliganManager.hisHeroCardActor?.GetHealthObject().Show();
    Board.Get().FindCollider("DragPlane").enabled = true;
    Board.Get().SplitSurface();
    Board.Get().RaiseTheLights();
    mulliganManager.FadeHeroPowerIn(GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard());
    mulliganManager.FadeHeroPowerIn(GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard());
    mulliganManager.ForceMulliganActive(false);
    mulliganManager.InitZones();
    mulliganManager.friendlySideHandZone.SetDoNotUpdateLayout(false);
    mulliganManager.friendlySideHandZone.UpdateLayout();
    mulliganManager.opposingSideHandZone.SetDoNotUpdateLayout(false);
    mulliganManager.opposingSideHandZone.UpdateLayout();
    mulliganManager.friendlySideDeck.SetSuppressEmotes(false);
    mulliganManager.opposingSideDeck.SetSuppressEmotes(false);
    InputManager.Get().OnMulliganEnded();
    EndTurnButton.Get().OnMulliganEnded();
    GameState.Get().GetGameEntity().NotifyOfMulliganEnded();
    mulliganManager.m_WaitForBoardAnimToCompleteThenStartTurn = mulliganManager.WaitForBoardAnimToCompleteThenStartTurn();
    mulliganManager.StartCoroutine(mulliganManager.m_WaitForBoardAnimToCompleteThenStartTurn);
  }

  private void FadeOutMulliganMusicAndStartGameplayMusic() => GameState.Get().GetGameEntity().StartGameplaySoundtracks();

  private IEnumerator WaitForBoardAnimToCompleteThenStartTurn()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MulliganManager mulliganManager = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      GameState.Get().SetMulliganBusy(false);
      UnityEngine.Object.Destroy((UnityEngine.Object) mulliganManager.gameObject);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(1.5f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void ShuffleDeck()
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "FX_MulliganCoin09_DeckShuffle.prefab:e80f93eec961ec24485521285a8addf7", this.friendlySideDeck.gameObject);
    Animation animation1 = this.friendlySideDeck.gameObject.GetComponent<Animation>();
    if ((UnityEngine.Object) animation1 == (UnityEngine.Object) null)
      animation1 = this.friendlySideDeck.gameObject.AddComponent<Animation>();
    animation1.AddClip(this.shuffleDeck, "shuffleDeckAnim");
    animation1.Play("shuffleDeckAnim");
    Animation animation2 = this.opposingSideDeck.gameObject.GetComponent<Animation>();
    if ((UnityEngine.Object) animation2 == (UnityEngine.Object) null)
      animation2 = this.opposingSideDeck.gameObject.AddComponent<Animation>();
    animation2.AddClip(this.shuffleDeck, "shuffleDeckAnim");
    animation2.Play("shuffleDeckAnim");
  }

  private void SlideCard(GameObject topCard) => iTween.MoveTo(topCard, iTween.Hash((object) "position", (object) new Vector3(topCard.transform.position.x - 0.5f, topCard.transform.position.y, topCard.transform.position.z), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.linear));

  private IEnumerator SampleAnimFrame(
    Animation animToUse,
    string animName,
    float startSec)
  {
    AnimationState state = animToUse[animName];
    state.enabled = true;
    state.time = startSec;
    animToUse.Play(animName);
    yield return (object) null;
    state.enabled = false;
  }

  private void SortHand(Zone zone) => zone.GetCards().Sort(new Comparison<Card>(Zone.CardSortComparison));

  private IEnumerator ShrinkStartingHandBanner(GameObject banner)
  {
    yield return (object) new WaitForSeconds(4f);
    if (!((UnityEngine.Object) banner == (UnityEngine.Object) null))
    {
      iTween.ScaleTo(banner, new Vector3(0.0f, 0.0f, 0.0f), 0.5f);
      yield return (object) new WaitForSeconds(0.5f);
      UnityEngine.Object.Destroy((UnityEngine.Object) banner);
    }
  }

  private void FadeHeroPowerIn(Card heroPowerCard)
  {
    if ((UnityEngine.Object) heroPowerCard == (UnityEngine.Object) null)
      return;
    Actor actor = heroPowerCard.GetActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    actor.TurnOnCollider();
  }

  private void LoadMyHeroSkinSocketInEffect(Actor myHero)
  {
    if (string.IsNullOrEmpty(myHero.SocketInEffectFriendly) && !(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty(myHero.SocketInEffectFriendlyPhone) && (bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_isLoadingMyCustomSocketIn = true;
    string assetRef = myHero.SocketInEffectFriendly;
    if ((bool) UniversalInputManager.UsePhoneUI)
      assetRef = myHero.SocketInEffectFriendlyPhone;
    AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, new PrefabCallback<GameObject>(this.OnMyHeroSkinSocketInEffectLoaded));
  }

  private void OnMyHeroSkinSocketInEffectLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Failed to load My custom hero socket in effect!");
      this.m_isLoadingMyCustomSocketIn = false;
    }
    else
    {
      go.transform.position = Board.Get().FindBone("CustomSocketIn_Friendly").position;
      Spell component = go.GetComponent<Spell>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Faild to locate Spell on custom socket in effect!");
        this.m_isLoadingMyCustomSocketIn = false;
      }
      else
      {
        this.m_MyCustomSocketInSpell = component;
        if (this.m_MyCustomSocketInSpell.HasUsableState(SpellStateType.IDLE))
          this.m_MyCustomSocketInSpell.ActivateState(SpellStateType.IDLE);
        else
          this.m_MyCustomSocketInSpell.gameObject.SetActive(false);
        this.m_isLoadingMyCustomSocketIn = false;
      }
    }
  }

  private void LoadHisHeroSkinSocketInEffect(Actor hisHero)
  {
    if (string.IsNullOrEmpty(hisHero.SocketInEffectOpponent) && !(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty(hisHero.SocketInEffectOpponentPhone) && (bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_isLoadingHisCustomSocketIn = true;
    string assetRef = hisHero.SocketInEffectOpponent;
    if ((bool) UniversalInputManager.UsePhoneUI)
      assetRef = hisHero.SocketInEffectOpponentPhone;
    AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, new PrefabCallback<GameObject>(this.OnHisHeroSkinSocketInEffectLoaded));
  }

  private void OnHisHeroSkinSocketInEffectLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Failed to load His custom hero socket in effect!");
      this.m_isLoadingHisCustomSocketIn = false;
    }
    else
    {
      go.transform.position = Board.Get().FindBone("CustomSocketIn_Opposing").position;
      Spell component = go.GetComponent<Spell>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Faild to locate Spell on custom socket in effect!");
        this.m_isLoadingHisCustomSocketIn = false;
      }
      else
      {
        this.m_HisCustomSocketInSpell = component;
        if (this.m_HisCustomSocketInSpell.HasUsableState(SpellStateType.IDLE))
          this.m_HisCustomSocketInSpell.ActivateState(SpellStateType.IDLE);
        else
          this.m_HisCustomSocketInSpell.gameObject.SetActive(false);
        this.m_isLoadingHisCustomSocketIn = false;
      }
    }
  }

  private void DestoryHeroSkinSocketInEffects()
  {
    if ((UnityEngine.Object) this.m_MyCustomSocketInSpell != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_MyCustomSocketInSpell.gameObject);
    if (!((UnityEngine.Object) this.m_HisCustomSocketInSpell != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_HisCustomSocketInSpell.gameObject);
  }

  private void OnFakeHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.MulliganManager.PrintWarning(string.Format("MulliganManager.OnFakeHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      --this.pendingFakeHeroCount;
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Log.MulliganManager.PrintWarning(string.Format("MulliganManager.OnFakeHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        --this.pendingFakeHeroCount;
      }
      else
      {
        ((List<Actor>) callbackData).Add(component);
        component.SetUnlit();
        LayerUtils.SetLayer(component.gameObject, this.gameObject.layer);
        component.GetMeshRenderer().gameObject.layer = 8;
        GameState.Get().GetGameEntity().ConfigureFakeMulliganCardActor(component, true);
        if (this.m_startingCards.Count > 0)
          component.gameObject.transform.position = new Vector3(this.m_startingCards[0].transform.position.x, this.m_startingCards[0].transform.position.y, this.m_startingCards[0].transform.position.z);
        --this.pendingFakeHeroCount;
      }
    }
  }

  private void OnHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.MulliganManager.PrintWarning(string.Format("MulliganManager.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      --this.pendingHeroCount;
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Log.MulliganManager.PrintWarning(string.Format("MulliganManager.OnHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        --this.pendingHeroCount;
      }
      else
      {
        Card card = (Card) callbackData;
        component.SetCard(card);
        component.SetCardDefFromCard(card);
        component.SetPremium(card.GetPremium());
        component.UpdateAllComponents();
        if ((UnityEngine.Object) card.GetActor() != (UnityEngine.Object) null)
          card.GetActor().Destroy();
        card.SetActor(component);
        component.SetEntity(card.GetEntity());
        component.UpdateAllComponents();
        component.SetUnlit();
        LayerUtils.SetLayer(component.gameObject, this.gameObject.layer);
        component.GetMeshRenderer().gameObject.layer = 8;
        component.GetHealthObject().Hide();
        GameState.Get().GetGameEntity().ApplyMulliganActorStateChanges(component);
        this.choiceHeroActors.Add(card, component);
        --this.pendingHeroCount;
      }
    }
  }

  private void OnOpponentHeroActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.MulliganManager.PrintWarning(string.Format("MulliganManager.OnOpponentHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      --this.pendingHeroCount;
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Log.MulliganManager.PrintWarning(string.Format("MulliganManager.OnOpponentHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        --this.pendingHeroCount;
      }
      else
      {
        Card card = (Card) callbackData;
        component.SetCard(card);
        component.SetCardDefFromCard(card);
        component.SetPremium(card.GetPremium());
        component.UpdateAllComponents();
        if ((UnityEngine.Object) card.GetActor() != (UnityEngine.Object) null)
          card.GetActor().Destroy();
        card.SetActor(component);
        component.SetEntity(card.GetEntity());
        component.UpdateAllComponents();
        component.SetUnlit();
        component.transform.localPosition = new Vector3(component.transform.localPosition.x + 1000f, component.transform.localPosition.y, component.transform.localPosition.z);
        LayerUtils.SetLayer(component.gameObject, this.gameObject.layer);
        UnityEngine.Object.Destroy((UnityEngine.Object) component.m_healthObject);
        UnityEngine.Object.Destroy((UnityEngine.Object) component.m_attackObject);
        GameState.Get().GetGameEntity().ApplyMulliganActorLobbyStateChanges(component);
        this.opponentHeroActors.Add(card, component);
        --this.pendingHeroCount;
      }
    }
  }

  [Serializable]
  public class TagConditionalVFX
  {
    [CustomEditField(Label = "Required Game Tag", SortPopupByName = true)]
    public GAME_TAG m_requiredTag;
    [CustomEditField(Label = "VFX Prefab")]
    public GameObject m_VFXPrefab;
    [CustomEditField(Label = "Banner Replacement Prefab")]
    public GameObject m_bannerReplacementPrefab;
    [CustomEditField(Label = "Banner Replacement Prefab Priority (Larger value wins)")]
    public int m_bannerReplacementPrefabPriority;
  }
}
