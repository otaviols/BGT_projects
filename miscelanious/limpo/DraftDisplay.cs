using Assets;
using Blizzard.T5.Configuration;
using Hearthstone;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DraftDisplay : MonoBehaviour
{
  public Collider m_pickArea;
  public UberText m_instructionText;
  public UberText m_instructionDetailText;
  public UberText m_forgeLabel;
  public DraftManaCurve m_manaCurve;
  public GameObject m_heroLabel;
  public Spell m_DeckCompleteSpell;
  public float m_DeckCardBarFlareUpDelay;
  public Spell m_heroPowerChosenFadeOut;
  public Spell m_heroPowerChosenFadeIn;
  public PegUIElement m_heroClickCatcher;
  public DraftPhoneDeckTray m_draftDeckTray;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_backButton;
  public StandardPegButtonNew m_retireButton;
  public PlayButton m_playButton;
  [CustomEditField(Sections = "Bones")]
  public Transform m_bigHeroBone;
  public Transform m_socketHeroBone;
  public List<Transform> m_heroPowerBones = new List<Transform>();
  public Transform m_socketHeroPowerBone;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_PhonePlayButtonTray;
  public Transform m_PhoneBackButtonBone;
  public Transform m_PhoneDeckTrayHiddenBone;
  public GameObject m_Phone3WayButtonRoot;
  public GameObject m_PhoneChooseHero;
  public GameObject m_PhoneLargeViewDeckButton;
  public ArenaPhoneControl m_PhoneDeckControl;
  private const string ALERTPOPUPID_FIRSTTIME = "arena_first_time";
  private static readonly Vector3 CHOICE_ACTOR_LOCAL_SCALE = new Vector3(7.2f, 7.2f, 7.2f);
  private static readonly Vector3 HERO_ACTOR_LOCAL_SCALE = new Vector3(8.285825f, 8.285825f, 8.285825f);
  private static readonly Vector3 HERO_LABEL_SCALE = new Vector3(8f, 8f, 8f);
  private static readonly Vector3 HERO_POWER_START_POSITION = new Vector3(0.0f, 0.0f, -0.3410472f);
  private static readonly Vector3 HERO_POWER_POSITION = new Vector3(1.40873f, 0.0f, -0.3410472f);
  private static readonly Vector3 HERO_POWER_SCALE = new Vector3(0.3419997f, 0.3419997f, 0.3419997f);
  private static readonly Vector3 DRAFTING_HERO_POWER_POSITION = new Vector3(0.9f, 0.215f, -0.164f);
  private static readonly Vector3 DRAFTING_HERO_POWER_BIG_CARD_SCALE = new Vector3(0.5f, 0.5f, 0.5f);
  private static readonly Vector3 DRAFTING_HERO_POWER_SCALE = new Vector3(5f, 5f, 5f);
  private static readonly Vector3 HERO_POWER_TOOLTIP_POSITION = new Vector3(-16.3f, 0.3f, -12.5f);
  private static readonly Vector3 HERO_POWER_TOOLTIP_SCALE = new Vector3(7f, 7f, 7f);
  private static readonly Vector3 CHOICE_ACTOR_LOCAL_SCALE_PHONE = new Vector3(14.5f, 14.5f, 14.5f) / DraftScene.DRAFT_SCENE_LOCAL_SCALE_INDEX_PHONE;
  private static readonly Vector3 HERO_ACTOR_LOCAL_SCALE_PHONE = new Vector3(15.5f, 15.5f, 15.5f) / DraftScene.DRAFT_SCENE_LOCAL_SCALE_INDEX_PHONE;
  private static readonly Vector3 HERO_LABEL_SCALE_PHONE = new Vector3(15f, 15f, 15f);
  private static readonly Vector3 HERO_POWER_START_POSITION_PHONE = new Vector3(1.6f, 0.3f, -0.15f);
  private static readonly Vector3 HERO_POWER_POSITION_PHONE = new Vector3(1.07f, 0.3f, -0.15f);
  private static readonly Vector3 HERO_POWER_SCALE_PHONE = new Vector3(0.5f, 0.5f, 0.5f) / DraftScene.DRAFT_SCENE_LOCAL_SCALE_INDEX_PHONE;
  private static readonly Vector3 DRAFTING_HERO_POWER_SCALE_PHONE = new Vector3(8f, 8f, 8f) / DraftScene.DRAFT_SCENE_LOCAL_SCALE_INDEX_PHONE;
  private static readonly Vector3 HERO_POWER_TOOLTIP_POSITION_PHONE = new Vector3(-6.7f, 5f, -5f);
  private static readonly Vector3 HERO_POWER_TOOLTIP_SCALE_PHONE = new Vector3(15f, 15f, 15f) / DraftScene.DRAFT_SCENE_LOCAL_SCALE_INDEX_PHONE;
  private static DraftDisplay s_instance;
  private DraftManager m_draftManager;
  private List<DraftDisplay.DraftChoice> m_choices = new List<DraftDisplay.DraftChoice>();
  private Actor[] m_heroPowerCardActors = new Actor[3];
  private DefLoader.DisposableFullDef[] m_heroPowerDefs = new DefLoader.DisposableFullDef[3];
  private DefLoader.DisposableFullDef[] m_subClassHeroPowerDefs = new DefLoader.DisposableFullDef[3];
  private DraftDisplay.DraftMode m_currentMode;
  private NormalButton m_confirmButton;
  private Actor m_heroPower;
  private Actor m_defaultHeroPowerSkin;
  private Actor m_goldenHeroPowerSkin;
  private bool m_netCacheReady;
  private Actor m_chosenHero;
  private Actor m_inPlayHeroPowerActor;
  private bool m_animationsComplete = true;
  private List<HeroLabel> m_currentLabels = new List<HeroLabel>();
  private CardSoundSpell[] m_heroEmotes = new CardSoundSpell[3];
  private bool m_skipHeroEmotes;
  private bool m_isHeroAnimating;
  private DraftCardVisual m_zoomedHero;
  private bool m_wasDrafting;
  private bool m_firstTimeIntroComplete;
  private DialogBase m_firstTimeDialog;
  private bool m_fxActive;
  private bool m_inPositionAndShowChoices;
  private List<Actor> m_subclassHeroClones = new List<Actor>();
  private Actor[] m_subclassHeroPowerActors = new Actor[3];
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    DraftDisplay.s_instance = this;
    this.m_draftManager = DraftManager.Get();
    AssetLoader.Get().InstantiatePrefab((AssetReference) "DraftHeroChooseButton.prefab:7640de5f1d8e50e4caf8dccc55f28c6a", new PrefabCallback<GameObject>(this.OnConfirmButtonLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "History_HeroPower.prefab:e73edf8ccea2b11429093f7a448eef53", new PrefabCallback<GameObject>(this.LoadHeroPowerCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER, TAG_PREMIUM.GOLDEN), new PrefabCallback<GameObject>(this.LoadGoldenHeroPowerCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    if ((bool) UniversalInputManager.UsePhoneUI)
      AssetLoader.Get().InstantiatePrefab((AssetReference) "BackButton_phone.prefab:08de22f2aa1facd42812215422eba8c7", new PrefabCallback<GameObject>(this.OnPhoneBackButtonLoaded));
    this.m_draftManager.RegisterDisplayHandlers();
    SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
    string sceneHeadlineText = this.m_draftManager.GetSceneHeadlineText();
    if (string.IsNullOrEmpty(sceneHeadlineText))
      sceneHeadlineText = GameStrings.Get("GLUE_TOOLTIP_BUTTON_FORGE_HEADLINE");
    this.m_forgeLabel.Text = sceneHeadlineText;
    this.m_instructionText.Text = string.Empty;
    this.m_pickArea.enabled = false;
    if (DemoMgr.Get().ArenaIs1WinMode())
    {
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_PLAY_MODE, false);
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE, false);
      Options.Get().SetBool(Option.HAS_SEEN_FORGE, true);
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_HERO_CHOICE, true);
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE2, false);
    }
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy()
  {
    foreach (DefLoader.DisposableFullDef heroPowerDef in this.m_heroPowerDefs)
      heroPowerDef?.Dispose();
    foreach (DefLoader.DisposableFullDef classHeroPowerDef in this.m_subClassHeroPowerDefs)
      classHeroPowerDef?.Dispose();
    this.FadeEffectsOut();
    DraftDisplay.s_instance = (DraftDisplay) null;
  }

  private void Start()
  {
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    NetCache.Get().RegisterScreenForge(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    this.SetupRetireButton();
    this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButtonPress));
    this.m_manaCurve.GetComponent<PegUIElement>().AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ManaCurveOver));
    this.m_manaCurve.GetComponent<PegUIElement>().AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.ManaCurveOut));
    this.m_playButton.SetText(GameStrings.Get("GLOBAL_PLAY"));
    this.ShowPhonePlayButton(false);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.SetupBackButton();
    Network.Get().RequestDraftChoicesAndContents();
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Arena);
    this.StartCoroutine(this.NotifySceneLoadedWhenReady());
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_draftDeckTray.gameObject.SetActive(true);
  }

  private void Update() => Network.Get().ProcessNetwork();

  public static DraftDisplay Get() => DraftDisplay.s_instance;

  public void OnOpenRewardsComplete() => this.ExitDraftScene();

  public void OnApplicationPause(bool pauseStatus)
  {
    if (!GameMgr.Get().IsFindingGame())
      return;
    this.CancelFindGame();
  }

  public void Unload()
  {
    Box.Get().SetToIgnoreFullScreenEffects(false);
    if ((UnityEngine.Object) this.m_confirmButton != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_confirmButton.gameObject);
    if ((UnityEngine.Object) this.m_heroPower != (UnityEngine.Object) null)
      this.m_heroPower.Destroy();
    if ((UnityEngine.Object) this.m_chosenHero != (UnityEngine.Object) null)
      this.m_chosenHero.Destroy();
    foreach (Actor subclassHeroClone in this.m_subclassHeroClones)
    {
      if ((UnityEngine.Object) subclassHeroClone != (UnityEngine.Object) null)
        subclassHeroClone.Destroy();
    }
    this.m_subclassHeroClones.Clear();
    foreach (Actor subclassHeroPowerActor in this.m_subclassHeroPowerActors)
    {
      if ((UnityEngine.Object) subclassHeroPowerActor != (UnityEngine.Object) null)
        subclassHeroPowerActor.Destroy();
    }
    this.m_currentLabels.Clear();
    this.m_draftManager.UnregisterDisplayHandlers();
    this.m_draftManager = (DraftManager) null;
    DraftInputManager.Get().Unload();
  }

  public void AcceptNewChoices(List<NetCache.CardDefinition> choices)
  {
    this.DestroyOldChoices();
    this.UpdateInstructionText();
    this.StartCoroutine(this.WaitForAnimsToFinishAndThenDisplayNewChoices(choices));
  }

  public void OnChoiceSelected(int chosenIndex)
  {
    DraftDisplay.DraftChoice choice = this.m_choices[chosenIndex - 1];
    Actor actor = choice.m_actor;
    if (actor.GetEntityDef().IsHeroSkin() || actor.GetEntityDef().IsHeroPower())
      return;
    this.AddCardToManaCurve(actor.GetEntityDef());
    this.m_draftDeckTray.GetCardsContent().UpdateCardList(choice.m_cardID, animateFromActor: actor);
  }

  private IEnumerator WaitForAnimsToFinishAndThenDisplayNewChoices(
    List<NetCache.CardDefinition> choices)
  {
    DraftDisplay draftDisplay = this;
    while (!draftDisplay.m_animationsComplete)
      yield return (object) null;
    while (draftDisplay.m_isHeroAnimating)
      yield return (object) null;
    draftDisplay.m_choices.Clear();
    for (int index = 0; index < choices.Count; ++index)
    {
      NetCache.CardDefinition choice = choices[index];
      DraftDisplay.DraftChoice draftChoice = new DraftDisplay.DraftChoice()
      {
        m_cardID = choice.Name,
        m_premium = choice.Premium,
        m_actor = (Actor) null
      };
      draftDisplay.m_choices.Add(draftChoice);
    }
    if (draftDisplay.m_draftManager.GetSlotType() != DraftSlotType.DRAFT_SLOT_HERO)
    {
      while ((UnityEngine.Object) draftDisplay.m_chosenHero == (UnityEngine.Object) null)
        yield return (object) null;
    }
    draftDisplay.m_skipHeroEmotes = false;
    for (int index = 0; index < draftDisplay.m_choices.Count; ++index)
    {
      DraftDisplay.DraftChoice choice = draftDisplay.m_choices[index];
      DefLoader.Get().LoadFullDef(choice.m_cardID, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(draftDisplay.OnFullDefLoaded), (object) new DraftDisplay.ChoiceCallback()
      {
        choiceID = (index + 1),
        slot = draftDisplay.m_draftManager.GetSlot(),
        premium = choice.m_premium
      });
    }
  }

  public void SetDraftMode(DraftDisplay.DraftMode mode)
  {
    int num = this.m_currentMode != mode ? 1 : 0;
    this.m_currentMode = mode;
    if (num == 0)
      return;
    Log.Arena.Print("SetDraftMode - " + (object) this.m_currentMode);
    this.StartCoroutine(this.InitializeDraftScreen());
  }

  public DraftDisplay.DraftMode GetDraftMode() => this.m_currentMode;

  public void CancelFindGame()
  {
    GameMgr.Get().CancelFindGame();
    this.HandleGameStartupFailure();
  }

  public void ZoomHeroCard(Actor hero, bool isDraftingHeroPower)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "tournament_screen_select_hero.prefab:2b9bdf587ac07084b8f7d5c4bce33ecf");
    this.m_isHeroAnimating = true;
    hero.SetUnlit();
    iTween.MoveTo(hero.gameObject, this.m_bigHeroBone.position, 0.25f);
    iTween.ScaleTo(hero.gameObject, this.m_bigHeroBone.localScale, 0.25f);
    SoundManager.Get().LoadAndPlay((AssetReference) "forge_hero_portrait_plate_rises.prefab:bffebffeb579074418432f59870e854e");
    this.FadeEffectsIn();
    LayerUtils.SetLayer(hero.gameObject, GameLayer.IgnoreFullScreenEffects);
    UniversalInputManager.Get().SetGameDialogActive(true);
    this.m_confirmButton.gameObject.SetActive(true);
    this.m_confirmButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Birth");
    this.m_confirmButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnConfirmButtonClicked));
    this.m_heroClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonClicked));
    this.m_heroClickCatcher.gameObject.SetActive(true);
    hero.TurnOffCollider();
    hero.SetActorState(ActorStateType.CARD_IDLE);
    if (isDraftingHeroPower)
    {
      foreach (Actor subclassHeroPowerActor in this.m_subclassHeroPowerActors)
        subclassHeroPowerActor.Hide();
    }
    if (!isDraftingHeroPower && this.m_draftManager.HasSlotType(DraftSlotType.DRAFT_SLOT_HERO_POWER))
      return;
    this.StartCoroutine(this.ShowHeroPowerWhenDefIsLoaded(isDraftingHeroPower));
  }

  public void OnHeroClicked(int heroChoice)
  {
    Actor hero = (Actor) null;
    bool isDraftingHeroPower = false;
    if (this.m_draftManager.GetSlotType() == DraftSlotType.DRAFT_SLOT_HERO)
      hero = this.m_choices[heroChoice - 1].m_actor;
    else if (this.m_draftManager.GetSlotType() == DraftSlotType.DRAFT_SLOT_HERO_POWER)
    {
      isDraftingHeroPower = true;
      hero = this.m_subclassHeroClones[heroChoice - 1];
      this.m_heroPower = this.m_heroPowerCardActors[heroChoice - 1];
      this.m_heroPower.Hide();
    }
    if ((UnityEngine.Object) hero != (UnityEngine.Object) null)
    {
      this.m_zoomedHero = hero.GetCollider().gameObject.GetComponent<DraftCardVisual>();
      this.ZoomHeroCard(hero, isDraftingHeroPower);
    }
    else
      Log.Arena.PrintWarning("DraftDisplay.OnHeroClicked: ChosenHeroActor is null! HeroChoice={0}", (object) heroChoice);
    bool flag = true;
    if (!isDraftingHeroPower)
    {
      flag = this.IsHeroEmoteSpellReady(heroChoice - 1);
      this.StartCoroutine(this.WaitForSpellToLoadAndPlay(heroChoice - 1));
    }
    if (!(this.CanAutoDraft() & flag))
      return;
    this.OnConfirmButtonClicked((UIEvent) null);
  }

  private void MakeHeroPowerGoldenIfPremium(DefLoader.DisposableFullDef heroPowerDef)
  {
    EntityDef entityDef = heroPowerDef.EntityDef;
    TAG_PREMIUM heroPremium = CollectionManager.Get().GetHeroPremium(entityDef.GetClass());
    this.m_heroPower = heroPremium == TAG_PREMIUM.GOLDEN ? this.m_goldenHeroPowerSkin : this.m_defaultHeroPowerSkin;
    this.m_heroPower.SetCardDef(heroPowerDef.DisposableCardDef);
    this.m_heroPower.SetEntityDef(entityDef);
    this.m_heroPower.SetPremium(heroPremium);
    this.m_heroPower.UpdateAllComponents();
  }

  private bool IsHeroEmoteSpellReady(int index) => (UnityEngine.Object) this.m_heroEmotes[index] != (UnityEngine.Object) null || this.m_skipHeroEmotes;

  private IEnumerator WaitForSpellToLoadAndPlay(int index)
  {
    bool wasEmoteAlreadyReady = this.IsHeroEmoteSpellReady(index);
    while (!this.IsHeroEmoteSpellReady(index))
      yield return (object) null;
    if (!this.m_skipHeroEmotes)
      this.m_heroEmotes[index].Reactivate();
    if (this.CanAutoDraft() && !wasEmoteAlreadyReady)
      this.OnConfirmButtonClicked((UIEvent) null);
  }

  public void ClickConfirmButton() => this.OnConfirmButtonClicked((UIEvent) null);

  private void OnConfirmButtonClicked(UIEvent e)
  {
    if (GameUtils.IsAnyTransitionActive())
      return;
    this.EnableBackButton(false);
    this.m_choices.ForEach((Action<DraftDisplay.DraftChoice>) (choice => choice.m_actor.TurnOffCollider()));
    if (this.m_draftManager.GetSlotType() == DraftSlotType.DRAFT_SLOT_HERO_POWER)
      this.m_subclassHeroClones.ForEach((Action<Actor>) (choice => choice.TurnOffCollider()));
    this.DoHeroSelectAnimation();
  }

  private void OnCancelButtonClicked(UIEvent e)
  {
    if (this.IsInHeroSelectMode())
      this.DoHeroCancelAnimation();
    else
      Navigation.GoBack();
  }

  private void RemoveListeners()
  {
    this.m_confirmButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnConfirmButtonClicked));
    this.m_confirmButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Death");
    this.m_confirmButton.gameObject.SetActive(false);
    this.m_heroClickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonClicked));
    this.m_heroClickCatcher.gameObject.SetActive(false);
  }

  private void FadeEffectsIn()
  {
    if (this.m_fxActive)
      return;
    this.m_fxActive = true;
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = 0.4f,
      Blur = new BlurParameters(brightness: 1f),
      Desaturate = new DesaturateParameters(0.0f)
    });
  }

  private void FadeEffectsOut()
  {
    if (!this.m_fxActive)
      return;
    this.m_fxActive = false;
    this.m_screenEffectsHandle.StopEffect(0.0f, new Action(this.OnFadeFinished));
  }

  private void OnFadeFinished()
  {
    if ((UnityEngine.Object) this.m_chosenHero == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(this.m_chosenHero.gameObject, GameLayer.Default);
  }

  public void DoHeroCancelAnimation()
  {
    this.RemoveListeners();
    this.m_heroPower.Hide();
    Actor actor;
    if (this.m_draftManager.GetSlotType() == DraftSlotType.DRAFT_SLOT_HERO)
    {
      actor = this.m_choices[this.m_zoomedHero.GetChoiceNum() - 1].m_actor;
    }
    else
    {
      actor = this.m_subclassHeroClones[this.m_zoomedHero.GetChoiceNum() - 1];
      foreach (Actor subclassHeroPowerActor in this.m_subclassHeroPowerActors)
      {
        subclassHeroPowerActor.Show();
        Spell componentInChildren = subclassHeroPowerActor.GetComponentInChildren<Spell>();
        if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        {
          componentInChildren.Deactivate();
          componentInChildren.Activate();
        }
      }
    }
    LayerUtils.SetLayer(actor.gameObject, GameLayer.Default);
    actor.TurnOnCollider();
    this.FadeEffectsOut();
    UniversalInputManager.Get().SetGameDialogActive(false);
    this.m_isHeroAnimating = false;
    this.m_pickArea.enabled = true;
    iTween.MoveTo(actor.gameObject, this.GetCardPosition(this.m_zoomedHero.GetChoiceNum() - 1, true), 0.25f);
    if ((bool) UniversalInputManager.UsePhoneUI)
      iTween.ScaleTo(actor.gameObject, DraftDisplay.HERO_ACTOR_LOCAL_SCALE_PHONE, 0.25f);
    else
      iTween.ScaleTo(actor.gameObject, DraftDisplay.HERO_ACTOR_LOCAL_SCALE, 0.25f);
    this.m_pickArea.enabled = false;
    this.m_zoomedHero = (DraftCardVisual) null;
  }

  public bool IsInHeroSelectMode() => (UnityEngine.Object) this.m_zoomedHero != (UnityEngine.Object) null;

  private void DoHeroSelectAnimation()
  {
    bool flag = this.m_draftManager.GetSlotType() == DraftSlotType.DRAFT_SLOT_HERO_POWER;
    this.RemoveListeners();
    this.m_heroPower.transform.parent = (Transform) null;
    if (!flag)
      this.m_heroPower.Hide();
    this.FadeEffectsOut();
    UniversalInputManager.Get().SetGameDialogActive(false);
    this.m_chosenHero = flag ? this.m_zoomedHero.GetSubActor() : this.m_zoomedHero.GetActor();
    this.m_zoomedHero.SetChosenFlag(true);
    this.m_draftManager.MakeChoice(this.m_zoomedHero.GetChoiceNum(), this.m_chosenHero.GetPremium());
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      Actor actor1;
      if (!flag)
      {
        actor1 = this.m_zoomedHero.GetActor();
      }
      else
      {
        actor1 = this.m_zoomedHero.GetSubActor();
        this.m_inPlayHeroPowerActor = this.m_subclassHeroPowerActors[this.m_zoomedHero.GetChoiceNum() - 1];
        Actor actor2 = this.m_zoomedHero.GetActor();
        actor2.transform.parent = this.m_socketHeroPowerBone;
        iTween.MoveTo(actor2.gameObject, iTween.Hash((object) "position", (object) Vector3.zero, (object) "time", (object) 0.25f, (object) "isLocal", (object) true, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "PhoneHeroPowerAnimationFinished", (object) "oncompletetarget", (object) this.gameObject));
        iTween.ScaleTo(actor2.gameObject, iTween.Hash((object) "scale", (object) Vector3.one, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCubic));
      }
      actor1.transform.parent = this.m_socketHeroBone;
      iTween.MoveTo(actor1.gameObject, iTween.Hash((object) "position", (object) Vector3.zero, (object) "time", (object) 0.25f, (object) "isLocal", (object) true, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "PhoneHeroAnimationFinished", (object) "oncompletetarget", (object) this.gameObject));
      iTween.ScaleTo(actor1.gameObject, iTween.Hash((object) "scale", (object) Vector3.one, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCubic));
    }
    else
    {
      this.m_zoomedHero.GetActor().ActivateSpellBirthState(SpellType.CONSTRUCT);
      this.m_zoomedHero = (DraftCardVisual) null;
      this.m_isHeroAnimating = false;
    }
    SoundManager.Get().LoadAndPlay((AssetReference) "forge_hero_portrait_plate_descend_and_impact.prefab:371e56744a872fc45a4bb3c043e684aa");
    this.ShowInnkeeperInstructions();
  }

  private void PhoneHeroAnimationFinished()
  {
    Log.Arena.Print("Phone Hero animation complete");
    this.m_zoomedHero = (DraftCardVisual) null;
    this.m_isHeroAnimating = false;
  }

  private void PhoneHeroPowerAnimationFinished()
  {
    Log.Arena.Print("Phone Hero Power animation complete");
    this.m_inPlayHeroPowerActor.transform.parent = this.m_socketHeroPowerBone;
    this.m_inPlayHeroPowerActor.transform.localPosition = Vector3.zero;
    this.m_inPlayHeroPowerActor.transform.localScale = Vector3.one;
    this.m_inPlayHeroPowerActor.Show();
  }

  public void AddCardToManaCurve(EntityDef entityDef)
  {
    if ((UnityEngine.Object) this.m_manaCurve == (UnityEngine.Object) null)
      Debug.LogWarning((object) string.Format("DraftDisplay.AddCardToManaCurve({0}) - m_manaCurve is null", (object) entityDef));
    else
      this.m_manaCurve.AddCardToManaCurve(entityDef);
  }

  public List<DraftCardVisual> GetCardVisuals()
  {
    List<DraftCardVisual> cardVisuals = new List<DraftCardVisual>();
    foreach (DraftDisplay.DraftChoice choice in this.m_choices)
    {
      if ((UnityEngine.Object) choice.m_actor == (UnityEngine.Object) null)
        return (List<DraftCardVisual>) null;
      DraftCardVisual component1 = choice.m_actor.GetCollider().gameObject.GetComponent<DraftCardVisual>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      {
        cardVisuals.Add(component1);
      }
      else
      {
        if ((UnityEngine.Object) choice.m_subActor == (UnityEngine.Object) null)
          return (List<DraftCardVisual>) null;
        DraftCardVisual component2 = choice.m_subActor.GetCollider().gameObject.GetComponent<DraftCardVisual>();
        if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
          return (List<DraftCardVisual>) null;
        cardVisuals.Add(component2);
      }
    }
    return cardVisuals;
  }

  public void HandleGameStartupFailure()
  {
    this.m_playButton.Enable();
    this.ShowPhonePlayButton(true);
    if (PresenceMgr.Get().CurrentStatus != Global.PresenceStatus.ARENA_QUEUE)
      return;
    PresenceMgr.Get().SetPrevStatus();
  }

  public void DoDeckCompleteAnims()
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "forge_commit_deck.prefab:1e3ef554bb2848b48816f336f2f91569");
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_DeckCompleteSpell.Activate();
    if (!((UnityEngine.Object) this.m_draftDeckTray != (UnityEngine.Object) null))
      return;
    this.m_draftDeckTray.GetCardsContent().ShowDeckCompleteEffects();
  }

  public bool DraftAnimationIsComplete() => this.m_animationsComplete;

  private IEnumerator NotifySceneLoadedWhenReady()
  {
    DraftDisplay draftDisplay = this;
    while ((UnityEngine.Object) draftDisplay.m_confirmButton == (UnityEngine.Object) null)
      yield return (object) null;
    while ((UnityEngine.Object) draftDisplay.m_heroPower == (UnityEngine.Object) null)
      yield return (object) null;
    while (draftDisplay.m_currentMode == DraftDisplay.DraftMode.INVALID)
      yield return (object) null;
    while (!draftDisplay.m_netCacheReady)
      yield return (object) null;
    while (!AchieveManager.Get().IsReady())
      yield return (object) null;
    draftDisplay.InitManaCurve();
    draftDisplay.m_draftDeckTray.Initialize();
    PegUIElement component = draftDisplay.m_draftDeckTray.GetTooltipZone().gameObject.GetComponent<PegUIElement>();
    component.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(draftDisplay.DeckHeaderOver));
    component.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(draftDisplay.DeckHeaderOut));
    SceneMgr.Get().NotifySceneLoaded();
  }

  private IEnumerator InitializeDraftScreen()
  {
    DraftDisplay draftDisplay = this;
    while (!ArenaTrayDisplay.Get().IsReady())
      yield return (object) null;
    if (!draftDisplay.m_firstTimeIntroComplete && !Options.Get().GetBool(Option.HAS_SEEN_FORGE, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.InitializeDraftScreen:" + (object) Option.HAS_SEEN_FORGE))
    {
      while (SceneMgr.Get().IsTransitioning())
        yield return (object) null;
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ARENA_PURCHASE);
      draftDisplay.m_firstTimeIntroComplete = true;
      draftDisplay.DoFirstTimeIntro();
    }
    else
    {
      switch (draftDisplay.m_currentMode)
      {
        case DraftDisplay.DraftMode.NO_ACTIVE_DRAFT:
          while (SceneMgr.Get().IsTransitioning())
            yield return (object) null;
          int numTicketsOwned = draftDisplay.m_draftManager.GetNumTicketsOwned();
          if (StoreManager.Get().HasOutstandingPurchaseNotices(ProductType.PRODUCT_TYPE_DRAFT))
          {
            draftDisplay.ShowPurchaseScreen();
            break;
          }
          if (numTicketsOwned > 0)
          {
            draftDisplay.ShowOutstandingTicketScreen(numTicketsOwned);
            break;
          }
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ARENA_PURCHASE);
          draftDisplay.ShowPurchaseScreen();
          break;
        case DraftDisplay.DraftMode.DRAFTING:
          if (StoreManager.Get().HasOutstandingPurchaseNotices(ProductType.PRODUCT_TYPE_DRAFT))
          {
            while (SceneMgr.Get().IsTransitioning())
              yield return (object) null;
            draftDisplay.ShowPurchaseScreen();
          }
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ARENA_FORGE);
          if (draftDisplay.m_draftManager.ShouldShowFreeArenaWinScreen())
          {
            draftDisplay.ShowFreeArenaWinScreen();
            break;
          }
          draftDisplay.ShowCurrentlyDraftingScreen();
          break;
        case DraftDisplay.DraftMode.ACTIVE_DRAFT_DECK:
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ARENA_IDLE);
          draftDisplay.StartCoroutine(draftDisplay.ShowActiveDraftScreen());
          break;
        case DraftDisplay.DraftMode.IN_REWARDS:
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ARENA_REWARD);
          draftDisplay.ShowDraftRewardsScreen();
          break;
        default:
          Debug.LogError((object) string.Format("DraftDisplay.InitializeDraftScreen(): don't know how to handle m_currentMode = {0}", (object) draftDisplay.m_currentMode));
          break;
      }
    }
  }

  private void OnConfirmButtonLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_confirmButton = go.GetComponent<NormalButton>();
    this.m_confirmButton.SetText(GameStrings.Get("GLUE_CHOOSE"));
    this.m_confirmButton.gameObject.SetActive(false);
    LayerUtils.SetLayer(go, GameLayer.IgnoreFullScreenEffects);
  }

  private void OnPhoneBackButtonLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Phone Back Button failed to load!");
    }
    else
    {
      go.transform.SetParent(this.transform, true);
      this.m_backButton = go.GetComponent<UIBButton>();
      this.m_backButton.transform.parent = this.m_PhoneBackButtonBone;
      this.m_backButton.transform.position = this.m_PhoneBackButtonBone.position;
      this.m_backButton.transform.localScale = this.m_PhoneBackButtonBone.localScale;
      this.m_backButton.transform.rotation = Quaternion.identity;
      LayerUtils.SetLayer(go, GameLayer.Default);
      this.SetupBackButton();
    }
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("DeckPickerTrayDisplay.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      go.transform.SetParent(this.transform, true);
      this.m_inPlayHeroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_inPlayHeroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DeckPickerTrayDisplay.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_inPlayHeroPowerActor.SetUnlit();
        this.m_inPlayHeroPowerActor.Hide();
      }
    }
  }

  private void LoadHeroPowerCallback(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "DeckPickerTrayDisplay.LoadHeroPowerCallback() - ERROR actor null.");
    }
    else
    {
      actor.transform.SetParent(this.transform, true);
      actor.TurnOffCollider();
      LayerUtils.SetLayer(actor.gameObject, GameLayer.IgnoreFullScreenEffects);
      this.m_heroPower = actor;
      actor.Hide();
    }
  }

  private void LoadHeroPowerCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("DeckPickerTrayDisplay.LoadHeroPowerCallback() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      go.transform.SetParent(this.transform, true);
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DeckPickerTrayDisplay.LoadHeroPowerCallback() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        component.TurnOffCollider();
        LayerUtils.SetLayer(component.gameObject, GameLayer.IgnoreFullScreenEffects);
        this.m_defaultHeroPowerSkin = component;
        this.m_heroPower = component;
        component.Hide();
      }
    }
  }

  private void LoadGoldenHeroPowerCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    go.transform.SetParent(this.transform, true);
    this.m_goldenHeroPowerSkin = go.GetComponent<Actor>();
  }

  private void ShowHeroPowerBigCard(bool isDraftingHeroPower)
  {
    if ((UnityEngine.Object) this.m_heroPower == (UnityEngine.Object) null)
      return;
    LayerUtils.SetLayer(this.m_heroPower.gameObject, GameLayer.IgnoreFullScreenEffects);
    Actor actor = this.m_zoomedHero.GetSubActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      actor = this.m_zoomedHero.GetActor();
    this.m_heroPower.gameObject.transform.SetParent(actor.transform);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_heroPower.gameObject.transform.localPosition = DraftDisplay.HERO_POWER_START_POSITION_PHONE;
      this.m_heroPower.gameObject.transform.localScale = DraftDisplay.HERO_POWER_SCALE_PHONE;
    }
    else if (!isDraftingHeroPower)
    {
      this.m_heroPower.gameObject.transform.localPosition = DraftDisplay.HERO_POWER_START_POSITION;
      this.m_heroPower.gameObject.transform.localScale = DraftDisplay.HERO_POWER_SCALE;
    }
    else
    {
      this.m_heroPower.gameObject.transform.localPosition = DraftDisplay.HERO_POWER_START_POSITION;
      this.m_heroPower.gameObject.transform.localScale = DraftDisplay.DRAFTING_HERO_POWER_BIG_CARD_SCALE;
    }
  }

  private void ShowHeroPower(Actor actor)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_heroPower.gameObject.transform.localPosition = DraftDisplay.HERO_POWER_TOOLTIP_POSITION_PHONE;
      this.m_heroPower.gameObject.transform.localScale = DraftDisplay.HERO_POWER_TOOLTIP_SCALE_PHONE;
    }
    else
    {
      this.m_heroPower.gameObject.transform.localPosition = DraftDisplay.HERO_POWER_TOOLTIP_POSITION;
      this.m_heroPower.gameObject.transform.localScale = DraftDisplay.HERO_POWER_TOOLTIP_SCALE;
    }
    this.m_heroPower.SetFullDefFromActor(actor);
    this.m_heroPower.UpdateAllComponents();
    this.m_heroPower.Show();
  }

  private IEnumerator ShowHeroPowerWhenDefIsLoaded(bool isDraftingHeroPower = false)
  {
    if (!((UnityEngine.Object) this.m_zoomedHero == (UnityEngine.Object) null))
    {
      if (!isDraftingHeroPower)
      {
        while (this.m_heroPowerDefs[this.m_zoomedHero.GetChoiceNum() - 1] == null)
          yield return (object) null;
        DefLoader.DisposableFullDef heroPowerDef = this.m_heroPowerDefs[this.m_zoomedHero.GetChoiceNum() - 1];
        this.MakeHeroPowerGoldenIfPremium(heroPowerDef);
        if (!GameUtils.IsVanillaHero(this.m_zoomedHero.GetActor().GetEntityDef().GetCardId()))
          heroPowerDef.CardDef.m_AlwaysRenderPremiumPortrait = true;
      }
      this.m_heroPower.Show();
      this.ShowHeroPowerBigCard(isDraftingHeroPower);
      if ((bool) UniversalInputManager.UsePhoneUI)
        iTween.MoveTo(this.m_heroPower.gameObject, iTween.Hash((object) "position", (object) DraftDisplay.HERO_POWER_POSITION_PHONE, (object) "isLocal", (object) true, (object) "time", (object) 0.5f));
      else if (!isDraftingHeroPower)
        iTween.MoveTo(this.m_heroPower.gameObject, iTween.Hash((object) "position", (object) DraftDisplay.HERO_POWER_POSITION, (object) "isLocal", (object) true, (object) "time", (object) 0.5f));
      else
        iTween.MoveTo(this.m_heroPower.gameObject, iTween.Hash((object) "position", (object) DraftDisplay.DRAFTING_HERO_POWER_POSITION, (object) "isLocal", (object) true, (object) "time", (object) 0.5f));
    }
  }

  private IEnumerator WaitAndPositionHeroPower()
  {
    yield return (object) new WaitForSeconds(0.35f);
    this.m_inPlayHeroPowerActor = this.m_subclassHeroPowerActors[this.m_draftManager.ChosenIndex - 1];
    this.m_inPlayHeroPowerActor.transform.localPosition = this.m_socketHeroPowerBone.transform.localPosition;
    this.m_inPlayHeroPowerActor.transform.localScale = this.m_socketHeroPowerBone.transform.localScale;
    this.SetupToDisplayHeroPowerTooltip(this.m_inPlayHeroPowerActor);
    Spell componentInChildren1 = this.m_inPlayHeroPowerActor.GetComponentInChildren<Spell>();
    if ((UnityEngine.Object) componentInChildren1 != (UnityEngine.Object) null)
      componentInChildren1.Activate();
    this.m_inPlayHeroPowerActor.Show();
    DraftCardVisual componentInChildren2 = this.m_inPlayHeroPowerActor.GetComponentInChildren<DraftCardVisual>();
    if ((UnityEngine.Object) componentInChildren2 != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren2);
  }

  private void DestroyOldChoices()
  {
    this.m_animationsComplete = false;
    for (int index = 1; index < this.m_choices.Count + 1; ++index)
    {
      DraftDisplay.DraftChoice choice = this.m_choices[index - 1];
      Actor actor = choice.m_actor;
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
      {
        Actor subActor = choice.m_subActor;
        actor.TurnOffCollider();
        Spell spell1 = actor.GetSpell(DraftDisplay.GetSpellTypeForRarity(actor.GetEntityDef().GetRarity()));
        if (index == this.m_draftManager.ChosenIndex)
        {
          if (actor.GetEntityDef().IsHeroSkin())
          {
            foreach (HeroLabel currentLabel in this.m_currentLabels)
              currentLabel.FadeOut();
          }
          else if (actor.GetEntityDef().IsHeroPower())
          {
            actor.transform.parent = (Transform) null;
            LayerUtils.SetLayer(actor.gameObject, GameLayer.IgnoreFullScreenEffects);
            if (!(bool) UniversalInputManager.UsePhoneUI)
            {
              this.m_heroPower = actor.Clone();
              this.m_heroPower.Hide();
              Spell componentInChildren = actor.GetComponentInChildren<Spell>();
              componentInChildren.AddFinishedCallback(new Spell.FinishedCallback(this.CleanupChoicesOnSpellFinish_HeroPower), (object) actor);
              actor.Show();
              componentInChildren.Activate();
              this.StartCoroutine(this.WaitAndPositionHeroPower());
            }
            else
            {
              foreach (Actor subclassHeroPowerActor in this.m_subclassHeroPowerActors)
                subclassHeroPowerActor.Hide();
              this.SetupToDisplayHeroPowerTooltip(this.m_inPlayHeroPowerActor);
              this.m_heroPower.Hide();
            }
            foreach (HeroLabel currentLabel in this.m_currentLabels)
            {
              if ((UnityEngine.Object) currentLabel != (UnityEngine.Object) null)
                currentLabel.FadeOut();
            }
          }
          else
          {
            Spell spell2 = actor.GetSpell(SpellType.SUMMON_OUT_FORGE);
            if ((UnityEngine.Object) spell2 == (UnityEngine.Object) null)
            {
              Debug.LogError((object) "DraftDisplay.DestroyOldChoices: The SUMMON_OUT_FORGE spell is missing from the spell table for this card.");
            }
            else
            {
              spell2.AddFinishedCallback(new Spell.FinishedCallback(this.DestroyChoiceOnSpellFinish), (object) actor);
              actor.ActivateSpellBirthState(SpellType.SUMMON_OUT_FORGE);
              spell1.ActivateState(SpellStateType.DEATH);
              SoundManager.Get().LoadAndPlay((AssetReference) "forge_select_card_1.prefab:b770cd64bb913f0409902629f975421e");
            }
          }
        }
        else
        {
          SoundManager.Get().LoadAndPlay((AssetReference) "unselected_cards_dissipate.prefab:a68b6959b8e9ed4408bf2475f37fd97d");
          Spell spell3 = actor.GetSpell(SpellType.BURN);
          if ((UnityEngine.Object) spell3 != (UnityEngine.Object) null)
          {
            spell3.AddFinishedCallback(new Spell.FinishedCallback(this.DestroyChoiceOnSpellFinish), (object) actor);
            actor.ActivateSpellBirthState(SpellType.BURN);
          }
          Spell spell4 = (UnityEngine.Object) subActor == (UnityEngine.Object) null ? (Spell) null : subActor.GetSpell(SpellType.BURN);
          if ((UnityEngine.Object) spell4 != (UnityEngine.Object) null)
          {
            spell4.AddFinishedCallback(new Spell.FinishedCallback(this.DestroyChoiceOnSpellFinish), (object) subActor);
            subActor.ActivateSpellBirthState(SpellType.BURN);
          }
          if ((UnityEngine.Object) spell1 != (UnityEngine.Object) null)
            spell1.ActivateState(SpellStateType.DEATH);
        }
      }
    }
    this.StartCoroutine(this.CompleteAnims());
    this.m_inPositionAndShowChoices = false;
  }

  private void SetupToDisplayHeroPowerTooltip(Actor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Log.Arena.PrintWarning("DraftDisplay.SetupToDisplayHeroPowerTooltip: Actor is null!");
    }
    else
    {
      PegUIElement pegUiElement = actor.gameObject.GetComponent<PegUIElement>();
      if ((UnityEngine.Object) pegUiElement == (UnityEngine.Object) null)
      {
        pegUiElement = actor.gameObject.AddComponent<PegUIElement>();
        pegUiElement.gameObject.AddComponent<BoxCollider>();
      }
      pegUiElement.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnMouseOverHeroPower));
      pegUiElement.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnMouseOutHeroPower));
      actor.Show();
    }
  }

  private IEnumerator CompleteAnims()
  {
    yield return (object) new WaitForSeconds(0.5f);
    this.m_animationsComplete = true;
  }

  private void CleanupChoicesOnSpellFinish_HeroPower(Spell spell, object actorObject)
  {
    foreach (Actor subclassHeroClone in this.m_subclassHeroClones)
      subclassHeroClone.Hide();
    foreach (Actor subclassHeroPowerActor in this.m_subclassHeroPowerActors)
    {
      if ((UnityEngine.Object) subclassHeroPowerActor != (UnityEngine.Object) this.m_inPlayHeroPowerActor)
        subclassHeroPowerActor.Hide();
    }
    this.DestroyChoiceOnSpellFinish(spell, actorObject);
  }

  private void DestroyChoiceOnSpellFinish(Spell spell, object actorObject) => this.StartCoroutine(this.DestroyObjectAfterDelay(((Component) actorObject).gameObject));

  private IEnumerator DestroyObjectAfterDelay(GameObject gameObjectToDestroy)
  {
    yield return (object) new WaitForSeconds(5f);
    UnityEngine.Object.Destroy((UnityEngine.Object) gameObjectToDestroy);
  }

  private void OnFullDefLoaded(string cardID, DefLoader.DisposableFullDef def, object userData)
  {
    using (def)
    {
      if (def == null)
      {
        Debug.LogErrorFormat("Unable to load FullDef for cardID={0}", (object) cardID);
      }
      else
      {
        DraftDisplay.ChoiceCallback choiceCallback = (DraftDisplay.ChoiceCallback) userData;
        choiceCallback.fullDef = def;
        if (def.EntityDef.IsHeroSkin())
        {
          AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetZoneActor(def.EntityDef, TAG_ZONE.PLAY), new PrefabCallback<GameObject>(this.OnActorLoaded), (object) choiceCallback.Copy(), AssetLoadingOptions.IgnorePrefabPosition);
          DefLoader.Get().LoadCardDef(def.EntityDef.GetCardId(), new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnCardDefLoaded), (object) choiceCallback.choiceID);
          string powerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(def.EntityDef.GetCardId());
          DefLoader.Get().LoadFullDef(powerCardIdFromHero, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroPowerFullDefLoaded), (object) choiceCallback.choiceID);
        }
        else if (def.EntityDef.IsHeroPower())
        {
          AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(def.EntityDef, choiceCallback.premium), new PrefabCallback<GameObject>(this.OnActorLoaded), (object) choiceCallback.Copy(), AssetLoadingOptions.IgnorePrefabPosition);
          AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetZoneActor(def.EntityDef, TAG_ZONE.PLAY, choiceCallback.premium), new PrefabCallback<GameObject>(this.OnSubClassActorLoaded), (object) choiceCallback.Copy(), AssetLoadingOptions.IgnorePrefabPosition);
        }
        else
          AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(def.EntityDef, choiceCallback.premium), new PrefabCallback<GameObject>(this.OnActorLoaded), (object) choiceCallback.Copy(), AssetLoadingOptions.IgnorePrefabPosition);
      }
    }
  }

  private void OnHeroPowerFullDefLoaded(
    string cardID,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    int num = (int) userData;
    this.m_heroPowerDefs[num - 1]?.Dispose();
    this.m_heroPowerDefs[num - 1] = def;
  }

  public void ShowInnkeeperInstructions()
  {
    if (this.m_draftManager.GetSlotType() == DraftSlotType.DRAFT_SLOT_HERO && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_HERO_CHOICE, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.UpdateInstructionText:" + (object) Option.HAS_SEEN_FORGE_HERO_CHOICE))
    {
      if (this.m_draftManager.HasSlotType(DraftSlotType.DRAFT_SLOT_HERO_POWER))
        return;
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST1_19"), "VO_INNKEEPER_FORGE_INST1_19.prefab:a0e06e90b545b274290dad8e442e83d0", 3f);
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_HERO_CHOICE, true);
    }
    else if (this.m_draftManager.GetSlotType() == DraftSlotType.DRAFT_SLOT_CARD && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.DoHeroSelectAnimation:" + (object) Option.HAS_SEEN_FORGE_CARD_CHOICE))
    {
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_HERO_CHOICE, true);
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST2_20"), "VO_INNKEEPER_FORGE_INST2_20.prefab:242b6a30031534e47b1f8ddd69370eac", 3f);
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE, true);
    }
    else
    {
      if (this.m_draftManager.GetSlotType() != DraftSlotType.DRAFT_SLOT_CARD || Options.Get().GetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE2, false) || !UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.UpdateInstructionText:" + (object) Option.HAS_SEEN_FORGE_CARD_CHOICE2))
        return;
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST3_21"), "VO_INNKEEPER_FORGE_INST3_21.prefab:06182dd3360965d4ea48952a6dd4a720", 3f);
      Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE2, true);
    }
  }

  public void SetInstructionText()
  {
    switch (this.m_draftManager.GetSlotType())
    {
      case DraftSlotType.DRAFT_SLOT_CARD:
        this.m_instructionText.Text = GameStrings.Get("GLUE_DRAFT_INSTRUCTIONS");
        this.m_instructionDetailText.Text = "";
        break;
      case DraftSlotType.DRAFT_SLOT_HERO:
        this.m_instructionText.Text = GameStrings.Get("GLUE_DRAFT_HERO_INSTRUCTIONS");
        this.m_instructionDetailText.Text = "";
        break;
      case DraftSlotType.DRAFT_SLOT_HERO_POWER:
        this.m_instructionText.Text = GameStrings.Get("GLUE_DRAFT_HERO_POWER_INSTRUCTIONS_TITLE");
        this.m_instructionDetailText.Text = GameStrings.Get("GLUE_DRAFT_HERO_POWER_INSTRUCTIONS_DETAIL");
        break;
      default:
        this.m_instructionText.Text = GameStrings.Get("GLUE_DRAFT_INSTRUCTIONS");
        this.m_instructionDetailText.Text = "";
        break;
    }
  }

  private void UpdateInstructionText()
  {
    if (this.GetDraftMode() == DraftDisplay.DraftMode.DRAFTING)
    {
      this.ShowInnkeeperInstructions();
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        switch (this.m_draftManager.GetSlotType())
        {
          case DraftSlotType.DRAFT_SLOT_HERO:
            this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.ChooseHero);
            break;
          case DraftSlotType.DRAFT_SLOT_HERO_POWER:
            this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.ChooseHeroPower);
            break;
          default:
            if (this.m_draftManager.GetDraftDeck().GetTotalCardCount() > 0)
            {
              this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.CardCountViewDeck);
              break;
            }
            this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.ChooseCard);
            break;
        }
      }
      else
        this.SetInstructionText();
    }
    else if (this.GetDraftMode() == DraftDisplay.DraftMode.ACTIVE_DRAFT_DECK)
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.ViewDeck);
      else
        this.m_instructionText.Text = GameStrings.Get("GLUE_DRAFT_MATCH_PROG");
    }
    else
      this.m_instructionText.Text = "";
  }

  private void DoFirstTimeIntro()
  {
    Box.Get().SetToIgnoreFullScreenEffects(true);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    this.m_retireButton.Disable();
    if ((bool) (UnityEngine.Object) this.m_manaCurve)
      this.m_manaCurve.ResetBars();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      StoreManager.Get().StartArenaTransaction(new Store.ExitCallback(this.OnStoreBackButtonPressed), (object) null, true);
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_ARENA_1ST_TIME_HEADER"),
      m_text = GameStrings.Get("GLUE_ARENA_1ST_TIME_DESC"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnFirstTimeIntroOkButtonPressed),
      m_id = "arena_first_time"
    }, (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      this.m_firstTimeDialog = dialog;
      return true;
    }));
    SoundManager.Get().LoadAndPlay((AssetReference) "VO_INNKEEPER_ARENA_INTRO2.prefab:40f8c705d6df66445937a3ded7460725");
  }

  private void OnFirstTimeIntroOkButtonPressed(AlertPopup.Response response, object userData)
  {
    StoreManager.Get().HideStore(ShopType.ARENA_STORE);
    this.m_draftManager.RequestDraftBegin();
    Options.Get().SetBool(Option.HAS_SEEN_FORGE, true);
  }

  private void ShowFreeArenaWinScreen()
  {
    Box.Get().SetToIgnoreFullScreenEffects(true);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    this.m_retireButton.Disable();
    if ((bool) (UnityEngine.Object) this.m_manaCurve)
      this.m_manaCurve.ResetBars();
    StoreManager.Get().HideStore(ShopType.ARENA_STORE);
    DialogManager.Get().ShowFreeArenaWinPopup(UserAttentionBlocker.NONE, new FreeArenaWinDialog.Info()
    {
      m_callbackOnHide = new DialogBase.HideCallback(this.OnFreeArenaWinOkButtonPress),
      m_winCount = this.m_draftManager.GetWins()
    });
  }

  private void ShowOutstandingTicketScreen(int numTicketsOwned)
  {
    Box.Get().SetToIgnoreFullScreenEffects(true);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    this.m_retireButton.Disable();
    if ((bool) (UnityEngine.Object) this.m_manaCurve)
      this.m_manaCurve.ResetBars();
    DialogManager.Get().ShowOutstandingDraftTicketPopup(UserAttentionBlocker.NONE, new OutstandingDraftTicketDialog.Info()
    {
      m_callbackOnEnter = new Action(this.OnOutstandingTicketEnterButtonPress),
      m_callbackOnCancel = new Action(this.OnOutstandingTicketCancelButtonPress),
      m_outstandingTicketCount = numTicketsOwned
    });
  }

  private void ShowPurchaseScreen()
  {
    Box.Get().SetToIgnoreFullScreenEffects(true);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    this.m_retireButton.Disable();
    if ((bool) (UnityEngine.Object) this.m_manaCurve)
      this.m_manaCurve.ResetBars();
    if (DemoMgr.Get().ArenaIs1WinMode())
      Network.Get().PurchaseViaGold(1, ProductType.PRODUCT_TYPE_DRAFT, 0);
    else
      StoreManager.Get().StartArenaTransaction(new Store.ExitCallback(this.OnStoreBackButtonPressed), (object) null, false);
  }

  private void ShowCurrentlyDraftingScreen()
  {
    this.m_wasDrafting = true;
    ArenaTrayDisplay.Get().ShowPlainPaperBackground();
    StoreManager.Get().HideStore(ShopType.ARENA_STORE);
    this.UpdateInstructionText();
    this.m_retireButton.Disable();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    this.LoadAndPositionHeroCard();
    NarrativeManager.Get().OnArenaDraftStarted();
  }

  private IEnumerator ShowActiveDraftScreen()
  {
    StoreManager.Get().HideStore(ShopType.ARENA_STORE);
    int losses = this.m_draftManager.GetLosses();
    this.DestroyOldChoices();
    this.m_retireButton.Enable();
    this.m_playButton.Enable();
    this.ShowPhonePlayButton(true);
    this.UpdateInstructionText();
    this.LoadAndPositionHeroCard();
    if (this.m_wasDrafting)
      yield return (object) new WaitForSeconds(0.3f);
    ArenaTrayDisplay.Get().UpdateTray();
    if (UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.ShowActiveDraftScreen"))
    {
      if (!Options.Get().GetBool(Option.HAS_SEEN_FORGE_PLAY_MODE, false))
      {
        if (this.m_draftManager.GetWins() == 0 && losses == 0)
        {
          NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_COMPLETE_22"), "VO_INNKEEPER_ARENA_COMPLETE.prefab:d0c3736823e5a47479bc204abb7a6e71");
          Options.Get().SetBool(Option.HAS_SEEN_FORGE_PLAY_MODE, true);
        }
      }
      else if (losses == 2 && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_2LOSS, false))
      {
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_2LOSS_25"), "VO_INNKEEPER_FORGE_2LOSS_25.prefab:82e4f0325619e9d4e9a7fb384b6f7e47", 3f);
        Options.Get().SetBool(Option.HAS_SEEN_FORGE_2LOSS, true);
      }
      else if (this.m_draftManager.GetWins() == 1 && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_1WIN, false))
      {
        while (GameToastMgr.Get().AreToastsActive())
          yield return (object) null;
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(133.1f, NotificationManager.DEPTH, 54.2f), GameStrings.Get("VO_INNKEEPER_FORGE_1WIN"), "VO_INNKEEPER_ARENA_1WIN.prefab:31bb13e800c74c0439ee1a7bfc1e3499");
        Options.Get().SetBool(Option.HAS_SEEN_FORGE_1WIN, true);
      }
    }
  }

  private void ShowDraftRewardsScreen()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    this.EnableBackButton(false);
    this.m_retireButton.Disable();
    if (DemoMgr.Get().ArenaIs1WinMode())
    {
      this.StartCoroutine(this.RestartArena());
    }
    else
    {
      if (this.m_draftManager.ShouldActivateKey())
      {
        int maxWins = this.m_draftManager.GetMaxWins();
        if (this.m_draftManager.GetWins() >= maxWins && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_MAX_WIN, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.ShowDraftRewardsScreen:" + (object) Option.HAS_SEEN_FORGE_MAX_WIN))
        {
          NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_MAX_ARENA_WINS_04"), "VO_INNKEEPER_MAX_ARENA_WINS_04.prefab:cdf8e488f2d17604499f2cc358cb35f6");
          Options.Get().SetBool(Option.HAS_SEEN_FORGE_MAX_WIN, true);
        }
        ArenaTrayDisplay.Get().UpdateTray(false);
        ArenaTrayDisplay.Get().ActivateKey();
        if ((UnityEngine.Object) this.m_PhoneDeckControl != (UnityEngine.Object) null)
          this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.Rewards);
      }
      else
        ArenaTrayDisplay.Get().ShowRewardsOpenAtStart();
      this.LoadAndPositionHeroCard();
    }
  }

  private IEnumerator RestartArena()
  {
    DraftDisplay draftDisplay = this;
    Debug.LogWarning((object) "Restarting");
    int wins = draftDisplay.m_draftManager.GetWins();
    if (wins < 5)
      DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_NO_PRIZE"), true);
    else if (wins < 9)
      DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_PRIZE"), true);
    else if (wins == 9)
      DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_GRAND_PRIZE"), true);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "NumberLabel.prefab:597544d5ed24b994f95fe56e28584992", new PrefabCallback<GameObject>(draftDisplay.LastArenaWinsLabelLoaded), (object) wins, AssetLoadingOptions.IgnorePrefabPosition);
    draftDisplay.m_currentLabels = new List<HeroLabel>();
    yield return (object) new WaitForSeconds(6f);
    draftDisplay.SetDraftMode(DraftDisplay.DraftMode.NO_ACTIVE_DRAFT);
    yield return (object) new WaitForSeconds(2f);
    Network.Get().AckDraftRewards(draftDisplay.m_draftManager.GetDraftDeck().ID, draftDisplay.m_draftManager.GetSlot());
    yield return (object) new WaitForSeconds(1f);
    ArenaTrayDisplay.Get().UpdateTray();
    if ((UnityEngine.Object) draftDisplay.m_chosenHero != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) draftDisplay.m_chosenHero.gameObject);
    yield return (object) new WaitForSeconds(1f);
    Network.Get().PurchaseViaGold(1, ProductType.PRODUCT_TYPE_DRAFT, 0);
    yield return (object) new WaitForSeconds(15f);
    if (wins >= 5)
    {
      DemoMgr.Get().MakeDemoTextClickable(true);
      DemoMgr.Get().NextDemoTipIsNewArenaMatch();
    }
    else
    {
      DemoMgr.Get().RemoveDemoTextDialog();
      DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA"), false, true);
    }
  }

  private void LastArenaWinsLabelLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    int num = (int) callbackData;
    go.GetComponent<UberText>().Text = "Last Arena: " + (object) num + " Wins";
    go.transform.position = new Vector3(11.40591f, 1.341853f, 29.28797f);
    go.transform.localScale = new Vector3(15f, 15f, 15f);
  }

  private void LoadAndPositionHeroCard()
  {
    if ((UnityEngine.Object) this.m_chosenHero != (UnityEngine.Object) null)
      return;
    CollectionDeck draftDeck = this.m_draftManager.GetDraftDeck();
    if (draftDeck == null)
    {
      Log.All.Print("bug 8052, null exception");
    }
    else
    {
      TAG_PREMIUM heroPremium = CollectionManager.Get().GetHeroPremium(draftDeck.GetClass());
      GameUtils.LoadAndPositionCardActor("Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d", draftDeck.HeroCardID, heroPremium, new GameUtils.LoadActorCallback(this.OnHeroActorLoaded));
      string actorName;
      if (heroPremium == TAG_PREMIUM.GOLDEN)
      {
        actorName = "Card_Play_HeroPower_Premium.prefab:015ad985f9ec49e4db327d131fd79901";
        GameUtils.LoadAndPositionCardActor("History_HeroPower_Premium.prefab:081da807b95b8495e9f16825c5164787", draftDeck.HeroPowerCardID, heroPremium, new GameUtils.LoadActorCallback(this.LoadHeroPowerCallback));
      }
      else
        actorName = "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af";
      GameUtils.LoadAndPositionCardActor(actorName, draftDeck.HeroPowerCardID, heroPremium, new GameUtils.LoadActorCallback(this.OnHeroPowerActorLoaded));
    }
  }

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.Forge)
    {
      if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        return;
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_FORGE");
    }
    else
      this.m_netCacheReady = true;
  }

  private void PositionAndShowChoices()
  {
    if (this.m_inPositionAndShowChoices)
      return;
    this.m_inPositionAndShowChoices = true;
    this.m_pickArea.enabled = true;
    for (int index1 = 0; index1 < this.m_choices.Count; ++index1)
    {
      DraftDisplay.DraftChoice choice = this.m_choices[index1];
      if ((UnityEngine.Object) choice.m_actor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DraftDisplay.PositionAndShowChoices(): WARNING found choice with null actor (cardID = {0}). Skipping...", (object) choice.m_cardID));
      }
      else
      {
        bool isHeroSkin = choice.m_actor.GetEntityDef().IsHeroSkin();
        bool flag = choice.m_actor.GetEntityDef().IsHeroPower();
        Actor actor = (Actor) null;
        Actor heroPowerActor = (Actor) null;
        TAG_RARITY rarity;
        if (flag)
        {
          LayerUtils.SetLayer(this.m_chosenHero.gameObject, GameLayer.Default);
          actor = this.m_chosenHero.Clone();
          UberShaderController[] componentsInChildren = actor.GetComponentsInChildren<UberShaderController>(true);
          if (componentsInChildren != null)
          {
            for (int index2 = 0; index2 < componentsInChildren.Length; ++index2)
            {
              UberShaderController shaderController = componentsInChildren[index2];
              if ((UnityEngine.Object) shaderController.UberShaderAnimation != (UnityEngine.Object) null)
                shaderController.UberShaderAnimation = UnityEngine.Object.Instantiate<UberShaderAnimation>(shaderController.UberShaderAnimation);
            }
          }
          actor.transform.position = this.GetCardPosition(index1, true);
          actor.Show();
          actor.ActivateSpellBirthState(SpellType.SUMMON_IN_FORGE);
          rarity = actor.GetEntityDef().GetRarity();
          actor.ActivateSpellBirthState(DraftDisplay.GetSpellTypeForRarity(rarity));
          this.m_subclassHeroClones.Add(actor);
          DraftCardVisual draftCardVisual1 = actor.GetCollider().gameObject.GetComponent<DraftCardVisual>();
          if ((UnityEngine.Object) draftCardVisual1 == (UnityEngine.Object) null)
            draftCardVisual1 = actor.GetCollider().gameObject.AddComponent<DraftCardVisual>();
          draftCardVisual1.SetChoiceNum(index1 + 1);
          draftCardVisual1.SetActor(choice.m_actor);
          draftCardVisual1.SetSubActor(actor);
          choice.m_subActor = actor;
          actor.TurnOnCollider();
          heroPowerActor = this.m_subclassHeroPowerActors[index1];
          heroPowerActor.transform.position = this.m_heroPowerBones[index1].position;
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            heroPowerActor.transform.localScale = DraftDisplay.DRAFTING_HERO_POWER_SCALE_PHONE;
          }
          else
          {
            heroPowerActor.transform.localScale = DraftDisplay.DRAFTING_HERO_POWER_SCALE;
            SpellUtils.SetCustomSpellParent(SpellManager.Get().GetSpell(this.m_heroPowerChosenFadeOut), (Component) choice.m_actor);
          }
          DraftCardVisual draftCardVisual2 = heroPowerActor.GetCollider().gameObject.AddComponent<DraftCardVisual>();
          draftCardVisual2.SetChoiceNum(index1 + 1);
          draftCardVisual2.SetActor(choice.m_actor);
          draftCardVisual2.SetSubActor(actor);
          heroPowerActor.TurnOnCollider();
          DefLoader.DisposableFullDef classHeroPowerDef = this.m_subClassHeroPowerDefs[index1];
          heroPowerActor.SetPremium(choice.m_premium);
          heroPowerActor.SetCardDef(classHeroPowerDef.DisposableCardDef);
          heroPowerActor.SetEntityDef(classHeroPowerDef.EntityDef);
          heroPowerActor.UpdateAllComponents();
          heroPowerActor.Hide();
          Spell spell = SpellManager.Get().GetSpell(this.m_heroPowerChosenFadeIn);
          SpellUtils.SetCustomSpellParent(spell, (Component) heroPowerActor);
          spell.transform.localPosition = new Vector3(spell.transform.localPosition.x, spell.transform.localPosition.y + 0.5f, spell.transform.localPosition.z);
        }
        else
        {
          choice.m_actor.transform.position = this.GetCardPosition(index1, isHeroSkin);
          choice.m_actor.Show();
          choice.m_actor.ActivateSpellBirthState(SpellType.SUMMON_IN_FORGE);
          rarity = choice.m_actor.GetEntityDef().GetRarity();
          choice.m_actor.ActivateSpellBirthState(DraftDisplay.GetSpellTypeForRarity(rarity));
        }
        switch (rarity)
        {
          case TAG_RARITY.COMMON:
          case TAG_RARITY.FREE:
            SoundManager.Get().LoadAndPlay((AssetReference) "forge_normal_card_appears.prefab:3e1223a4e6503f2469fb0090db8da67e");
            break;
          case TAG_RARITY.RARE:
          case TAG_RARITY.EPIC:
          case TAG_RARITY.LEGENDARY:
            SoundManager.Get().LoadAndPlay((AssetReference) "forge_rarity_card_appears.prefab:4ecbc5de846e50746986849690c01e6a");
            break;
        }
        if (isHeroSkin)
        {
          if (index1 == 0 && DemoMgr.Get().ArenaIs1WinMode())
            DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA"), false, true);
          choice.m_actor.GetHealthObject().Hide();
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_heroLabel);
          gameObject.transform.position = choice.m_actor.GetMeshRenderer().transform.position;
          HeroLabel component = gameObject.GetComponent<HeroLabel>();
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            choice.m_actor.transform.localScale = DraftDisplay.HERO_ACTOR_LOCAL_SCALE_PHONE;
            gameObject.transform.localScale = DraftDisplay.HERO_LABEL_SCALE_PHONE;
          }
          else
          {
            choice.m_actor.transform.localScale = DraftDisplay.HERO_ACTOR_LOCAL_SCALE;
            gameObject.transform.localScale = DraftDisplay.HERO_LABEL_SCALE;
          }
          Color white = Color.white;
          if (this.m_draftManager.GetDraftPaperTextColorOverride(ref white))
            component.SetColor(white);
          gameObject.transform.SetParent(this.transform, true);
          component.UpdateText(choice.m_actor.GetEntityDef().GetName(), GameStrings.GetClassName(choice.m_actor.GetEntityDef().GetClass()).ToUpper());
          this.m_currentLabels.Add(component);
        }
        else if (flag)
        {
          actor.GetHealthObject().Hide();
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_heroLabel);
          gameObject.transform.position = actor.GetMeshRenderer().transform.position;
          HeroLabel newLabel = gameObject.GetComponent<HeroLabel>();
          newLabel.m_nameText.Hide();
          newLabel.m_classText.Hide();
          actor.GetSpell(SpellType.SUMMON_IN_FORGE).AddSpellEventCallback((Spell.SpellEventCallback) ((eventName, eventData, userData) =>
          {
            if (!(eventName == SummonInForge.ACTOR_VISIBLE_EVENT))
              return;
            heroPowerActor.Show();
            newLabel.m_classText.Show();
          }));
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            actor.transform.localScale = DraftDisplay.HERO_ACTOR_LOCAL_SCALE_PHONE;
            gameObject.transform.localScale = DraftDisplay.HERO_LABEL_SCALE_PHONE;
          }
          else
          {
            actor.transform.localScale = DraftDisplay.HERO_ACTOR_LOCAL_SCALE;
            gameObject.transform.localScale = DraftDisplay.HERO_LABEL_SCALE;
          }
          Color white = Color.white;
          if (this.m_draftManager.GetDraftPaperTextColorOverride(ref white))
            newLabel.SetColor(white);
          string classText = GameStrings.GetClassName(actor.GetEntityDef().GetClass()).ToUpper() + "-" + GameStrings.GetClassName(choice.m_actor.GetEntityDef().GetClass()).ToUpper();
          newLabel.UpdateText(this.m_chosenHero.GetEntityDef().GetName(), classText);
          newLabel.m_classText.CharacterSize = 5f;
          this.m_currentLabels.Add(newLabel);
        }
        else if ((bool) UniversalInputManager.UsePhoneUI)
          choice.m_actor.transform.localScale = DraftDisplay.CHOICE_ACTOR_LOCAL_SCALE_PHONE;
        else
          choice.m_actor.transform.localScale = DraftDisplay.CHOICE_ACTOR_LOCAL_SCALE;
      }
    }
    this.EnableBackButton(true);
    this.StartCoroutine(this.RunAutoDraftCheat());
    this.m_pickArea.enabled = false;
  }

  private bool CanAutoDraft() => HearthstoneApplication.IsInternal() && Vars.Key("Arena.AutoDraft").GetBool(false);

  public IEnumerator RunAutoDraftCheat()
  {
    if (this.CanAutoDraft())
    {
      int frameStart = Time.frameCount;
      while (GameUtils.IsAnyTransitionActive() && Time.frameCount - frameStart < 120)
        yield return (object) null;
      List<DraftCardVisual> draftChoices = this.GetCardVisuals();
      if (draftChoices != null && draftChoices.Count > 0)
      {
        int pickedIndex = UnityEngine.Random.Range(0, draftChoices.Count - 1);
        DraftCardVisual visual = draftChoices[pickedIndex];
        frameStart = Time.frameCount;
        while ((UnityEngine.Object) visual.GetActor() == (UnityEngine.Object) null && Time.frameCount - frameStart < 120)
          yield return (object) null;
        if ((UnityEngine.Object) visual.GetActor() != (UnityEngine.Object) null)
        {
          string message = string.Format("autodraft'ing {0}\nto stop, use cmd 'autodraft off'", (object) visual.GetActor().GetEntityDef().GetName());
          UIStatus.Get().AddInfo(message, 2f);
          draftChoices[pickedIndex].ChooseThisCard();
        }
        visual = (DraftCardVisual) null;
      }
    }
  }

  private Vector3 GetCardPosition(int cardChoice, bool isHeroSkin)
  {
    Bounds bounds = this.m_pickArea.bounds;
    double x1 = (double) bounds.center.x;
    bounds = this.m_pickArea.bounds;
    double x2 = (double) bounds.extents.x;
    double num1 = x1 - x2;
    bounds = this.m_pickArea.bounds;
    float num2 = bounds.size.x / 3f;
    float num3 = this.m_choices.Count == 2 ? 0.0f : (float) (-(double) num2 / 2.0);
    float num4 = 0.0f;
    if (isHeroSkin)
      num4 = 1f;
    double num5 = (double) (cardChoice + 1) * (double) num2;
    return new Vector3((float) (num1 + num5) + num3, this.m_pickArea.transform.position.y, this.m_pickArea.transform.position.z + num4);
  }

  public static SpellType GetSpellTypeForRarity(TAG_RARITY rarity)
  {
    switch (rarity)
    {
      case TAG_RARITY.RARE:
        return SpellType.BURST_RARE;
      case TAG_RARITY.EPIC:
        return SpellType.BURST_EPIC;
      case TAG_RARITY.LEGENDARY:
        return SpellType.BURST_LEGENDARY;
      default:
        return SpellType.BURST_COMMON;
    }
  }

  private void OnHeroActorLoaded(Actor actor)
  {
    actor.transform.SetParent(this.transform, true);
    this.m_chosenHero = actor;
    this.m_chosenHero.transform.parent = this.m_socketHeroBone;
    this.m_chosenHero.transform.localPosition = Vector3.zero;
    this.m_chosenHero.transform.localScale = Vector3.one;
    this.m_chosenHero.transform.localRotation = Quaternion.identity;
  }

  private void OnHeroPowerActorLoaded(Actor actor)
  {
    actor.transform.SetParent(this.transform, true);
    this.m_inPlayHeroPowerActor = actor;
    this.SetupToDisplayHeroPowerTooltip(this.m_inPlayHeroPowerActor);
    this.m_inPlayHeroPowerActor.transform.parent = this.m_socketHeroPowerBone;
    this.m_inPlayHeroPowerActor.transform.localPosition = Vector3.zero;
    this.m_inPlayHeroPowerActor.transform.localScale = Vector3.one;
    this.m_inPlayHeroPowerActor.transform.localRotation = Quaternion.identity;
  }

  private void OnMouseOverHeroPower(UIEvent uiEvent)
  {
    if (!((UnityEngine.Object) this.m_inPlayHeroPowerActor != (UnityEngine.Object) null))
      return;
    this.ShowHeroPower(this.m_inPlayHeroPowerActor);
  }

  private void OnMouseOutHeroPower(UIEvent uiEvent)
  {
    if (!((UnityEngine.Object) this.m_heroPower != (UnityEngine.Object) null))
      return;
    this.m_heroPower.Hide();
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    DraftDisplay.ChoiceCallback choiceCallback = (DraftDisplay.ChoiceCallback) callbackData;
    using (DefLoader.DisposableFullDef fullDef = choiceCallback?.fullDef)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DraftDisplay.OnActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      }
      else
      {
        go.transform.SetParent(this.transform, true);
        Actor component = go.GetComponent<Actor>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("DraftDisplay.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        }
        else
        {
          DraftDisplay.DraftChoice draftChoice = this.m_choices.Find((Predicate<DraftDisplay.DraftChoice>) (obj => obj.m_cardID.Equals(fullDef.EntityDef.GetCardId())));
          if (draftChoice == null)
          {
            Debug.LogWarningFormat("DraftDisplay.OnActorLoaded(): Could not find draft choice {0} (cardID = {1}) in m_choices.", (object) fullDef.EntityDef.GetName(), (object) fullDef.EntityDef.GetCardId());
            UnityEngine.Object.Destroy((UnityEngine.Object) go);
          }
          else
          {
            draftChoice.m_actor = component;
            draftChoice.m_actor.SetPremium(draftChoice.m_premium);
            draftChoice.m_actor.SetEntityDef(fullDef.EntityDef);
            draftChoice.m_actor.SetCardDef(fullDef.DisposableCardDef);
            draftChoice.m_actor.UpdateAllComponents();
            draftChoice.m_actor.gameObject.name = fullDef.CardDef.name + "_actor";
            draftChoice.m_actor.ContactShadow(true);
            if (draftChoice.m_actor.GetEntityDef().IsHeroPower())
            {
              this.m_heroPowerCardActors[choiceCallback.choiceID - 1] = draftChoice.m_actor;
              if (this.HaveActorsForAllChoices() && this.HaveAllSubclassHeroPowerDefs())
                this.PositionAndShowChoices();
              else
                draftChoice.m_actor.Hide();
            }
            else
            {
              DraftCardVisual draftCardVisual = draftChoice.m_actor.GetCollider().gameObject.AddComponent<DraftCardVisual>();
              draftCardVisual.SetActor(draftChoice.m_actor);
              draftCardVisual.SetChoiceNum(choiceCallback.choiceID);
              if (this.HaveActorsForAllChoices())
                this.PositionAndShowChoices();
              else
                draftChoice.m_actor.Hide();
            }
          }
        }
      }
    }
  }

  private void OnSubClassActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    DraftDisplay.ChoiceCallback choiceCallback = (DraftDisplay.ChoiceCallback) callbackData;
    using (DefLoader.DisposableFullDef fullDef = choiceCallback.fullDef)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DraftDisplay.OnDualClassActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      }
      else
      {
        go.transform.SetParent(this.transform, true);
        Actor component = go.GetComponent<Actor>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("DraftDisplay.OnDualClassActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        }
        else
        {
          this.m_subClassHeroPowerDefs[choiceCallback.choiceID - 1]?.Dispose();
          this.m_subClassHeroPowerDefs[choiceCallback.choiceID - 1] = fullDef.Share();
          this.m_subclassHeroPowerActors[choiceCallback.choiceID - 1] = component;
          if (!this.HaveActorsForAllChoices() || !this.HaveAllSubclassHeroPowerDefs())
            return;
          this.PositionAndShowChoices();
        }
      }
    }
  }

  private void OnCardDefLoaded(string cardId, DefLoader.DisposableCardDef def, object callbackData)
  {
    using (def)
    {
      if (def == null)
        return;
      foreach (EmoteEntryDef emoteDef in def?.CardDef.m_EmoteDefs)
      {
        if (emoteDef.m_emoteType == EmoteType.PICKED)
          AssetLoader.Get().InstantiatePrefab((AssetReference) emoteDef.m_emoteSoundSpellPath, new PrefabCallback<GameObject>(this.OnStartEmoteLoaded), callbackData);
      }
    }
  }

  private void OnStartEmoteLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    CardSoundSpell cardSoundSpell = (CardSoundSpell) null;
    if ((UnityEngine.Object) go != (UnityEngine.Object) null)
    {
      cardSoundSpell = go.GetComponent<CardSoundSpell>();
      go.transform.SetParent(this.transform, true);
    }
    this.m_skipHeroEmotes |= (UnityEngine.Object) cardSoundSpell == (UnityEngine.Object) null;
    if (this.m_skipHeroEmotes)
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    else
      this.m_heroEmotes[(int) callbackData - 1] = cardSoundSpell;
  }

  private bool HaveActorsForAllChoices()
  {
    foreach (DraftDisplay.DraftChoice choice in this.m_choices)
    {
      if ((UnityEngine.Object) choice.m_actor == (UnityEngine.Object) null)
        return false;
    }
    return true;
  }

  private bool HaveAllSubclassHeroPowerDefs()
  {
    foreach (DefLoader.DisposableFullDef classHeroPowerDef in this.m_subClassHeroPowerDefs)
    {
      if (classHeroPowerDef == null)
        return false;
    }
    return true;
  }

  private void InitManaCurve()
  {
    CollectionDeck draftDeck = this.m_draftManager.GetDraftDeck();
    if (draftDeck == null)
      return;
    foreach (CollectionDeckSlot slot in draftDeck.GetSlots())
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(slot.CardID);
      for (int index = 0; index < slot.Count; ++index)
        this.AddCardToManaCurve(entityDef);
    }
  }

  private void OnStoreBackButtonPressed(bool authorizationBackButtonPressed, object userData) => this.ExitDraftScene();

  private bool OnNavigateBack()
  {
    if (this.IsInHeroSelectMode())
    {
      this.DoHeroCancelAnimation();
      return false;
    }
    if ((UnityEngine.Object) ArenaTrayDisplay.Get() == (UnityEngine.Object) null)
      return false;
    ArenaTrayDisplay.Get().KeyFXCancel();
    this.ExitDraftScene();
    return true;
  }

  private void BackButtonPress(UIEvent e) => Navigation.GoBack();

  private void ExitDraftScene()
  {
    GameMgr.Get().CancelFindGame();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    StoreManager.Get().HideStore(ShopType.ARENA_STORE);
    if (!SceneMgr.Get().IsInDuelsMode())
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAME_MODE, SceneMgr.TransitionHandlerType.NEXT_SCENE);
    Box.Get().SetToIgnoreFullScreenEffects(false);
  }

  private void PlayButtonPress(UIEvent e)
  {
    if (SetRotationManager.Get().CheckForSetRotationRollover() || PlayerMigrationManager.Get() != null && PlayerMigrationManager.Get().CheckForPlayerMigrationRequired())
      return;
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_playButton.Disable();
    this.ShowPhonePlayButton(false);
    this.m_draftManager.FindGame();
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ARENA_QUEUE);
  }

  private void RetireButtonPress(UIEvent e) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLUE_FORGE_RETIRE_WARNING_HEADER"),
    m_text = GameStrings.Get("GLUE_FORGE_RETIRE_WARNING_DESC"),
    m_showAlertIcon = false,
    m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
    m_responseCallback = new AlertPopup.ResponseCallback(this.OnRetirePopupResponse)
  });

  private void OnFreeArenaWinOkButtonPress(DialogBase dialog, object userData)
  {
    Options.Get().SetBool(Option.HAS_SEEN_FREE_ARENA_WIN_DIALOG_THIS_DRAFT, true);
    this.ShowCurrentlyDraftingScreen();
  }

  private void OnOutstandingTicketEnterButtonPress()
  {
    this.m_draftManager.RequestDraftBegin();
    Options.Get().SetBool(Option.HAS_SEEN_FORGE, true);
  }

  private void OnOutstandingTicketCancelButtonPress() => this.ExitDraftScene();

  private void OnRetirePopupResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_draftDeckTray.gameObject.GetComponent<SlidingTray>().HideTray();
    this.m_retireButton.Disable();
    this.EnableBackButton(false);
    Network.Get().DraftRetire(this.m_draftManager.GetDraftDeck().ID, this.m_draftManager.GetSlot(), this.m_draftManager.CurrentSeasonId);
  }

  private void ManaCurveOver(UIEvent e) => this.m_manaCurve.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get("GLUE_FORGE_MANATIP_HEADER"), GameStrings.Get("GLUE_FORGE_MANATIP_DESC"), (float) TooltipPanel.FORGE_SCALE);

  private void ManaCurveOut(UIEvent e) => this.m_manaCurve.GetComponent<TooltipZone>().HideTooltip();

  private void DeckHeaderOver(UIEvent e) => this.m_draftDeckTray.GetTooltipZone().ShowTooltip(GameStrings.Get("GLUE_ARENA_DECK_TOOLTIP_HEADER"), GameStrings.Get("GLUE_ARENA_DECK_TOOLTIP"), (float) TooltipPanel.FORGE_SCALE);

  private void DeckHeaderOut(UIEvent e) => this.m_draftDeckTray.GetTooltipZone().HideTooltip();

  private void SetupBackButton()
  {
    if (DemoMgr.Get().CantExitArena())
    {
      this.m_backButton.SetText("");
    }
    else
    {
      this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonPress));
    }
  }

  private void EnableBackButton(bool buttonEnabled)
  {
    if (buttonEnabled != this.m_backButton.IsEnabled())
      this.m_backButton.Flip(buttonEnabled);
    this.m_backButton.SetEnabled(buttonEnabled);
    if (!((UnityEngine.Object) this.m_PhoneBackButtonBone != (UnityEngine.Object) null))
      return;
    this.m_PhoneBackButtonBone.gameObject.SetActive(buttonEnabled);
  }

  private void SetupRetireButton()
  {
    if (DemoMgr.Get().CantExitArena())
    {
      this.m_retireButton.SetText("");
    }
    else
    {
      this.m_retireButton.SetText(GameStrings.Get("GLUE_DRAFT_RETIRE_BUTTON"));
      this.m_retireButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.RetireButtonPress));
    }
  }

  private void ShowPhonePlayButton(bool show)
  {
    if ((UnityEngine.Object) this.m_PhonePlayButtonTray == (UnityEngine.Object) null)
      return;
    SlidingTray component = this.m_PhonePlayButtonTray.GetComponent<SlidingTray>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.ToggleTraySlider(show);
  }

  private void OnScenePreUnload(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    if (prevMode != SceneMgr.Mode.DRAFT)
      return;
    StoreManager.Get().HideStore(ShopType.ARENA_STORE);
    DialogManager.Get().RemoveUniquePopupRequestFromQueue("arena_first_time");
    if ((UnityEngine.Object) this.m_firstTimeDialog != (UnityEngine.Object) null)
      this.m_firstTimeDialog.Hide();
    if (!this.IsInHeroSelectMode())
      return;
    this.m_zoomedHero.gameObject.SetActive(false);
    this.m_heroPower.gameObject.SetActive(false);
    this.m_confirmButton.gameObject.SetActive(false);
    UniversalInputManager.Get().SetGameDialogActive(false);
  }

  public enum DraftMode
  {
    INVALID,
    NO_ACTIVE_DRAFT,
    DRAFTING,
    ACTIVE_DRAFT_DECK,
    IN_REWARDS,
  }

  private class ChoiceCallback
  {
    public DefLoader.DisposableFullDef fullDef;
    public int choiceID;
    public int slot;
    public TAG_PREMIUM premium;

    public DraftDisplay.ChoiceCallback Copy() => new DraftDisplay.ChoiceCallback()
    {
      fullDef = this.fullDef?.Share(),
      choiceID = this.choiceID,
      slot = this.slot,
      premium = this.premium
    };
  }

  private class DraftChoice
  {
    public string m_cardID = string.Empty;
    public TAG_PREMIUM m_premium;
    public Actor m_actor;
    public Actor m_subActor;
  }
}
