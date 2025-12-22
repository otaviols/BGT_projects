using Blizzard.T5.Core;
using Game.PackOpening;
using Hearthstone.Attribution;
using Hearthstone.Progression;
using PegasusLettuce;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class PackOpeningDirector : MonoBehaviour
{
  private readonly Vector3 PACK_OPENING_FX_POSITION = Vector3.zero;
  public PackOpeningCard m_HiddenCard;
  public GameObject m_CardsInsidePack;
  public GameObject m_ClassName;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_DoneButtonPrefab;
  public Shared.UI.Scripts.Carousel.Carousel m_Carousel;
  private readonly HiddenCards m_hiddenCards = new HiddenCards();
  private NormalButton m_doneButton;
  private bool m_loadingDoneButton;
  private bool m_playing;
  private readonly Map<int, Spell> m_packFxSpells = new Map<int, Spell>();
  private Spell m_activePackFxSpell;
  private int m_cardsPendingReveal;
  private int m_effectsPendingFinish;
  private int m_effectsPendingDestroy;
  private int m_centerCardIndex;
  private bool m_doneButtonShown;
  private ScreenEffectsHandle m_effectHandle;
  private PackOpeningCard m_clickedCard;
  private int m_clickedPosition;
  private PackOpeningCard m_glowingCard;
  private float m_initializePackOpeningAnimationStartTime;

  public event EventHandler OnFinishedEvent;

  public event Action OnDoneOpeningPack;

  public bool IsDoneButtonShown => this.m_doneButtonShown;

  private void Awake()
  {
    this.m_hiddenCards.OnCardRevealedEvent += new EventHandler(this.OnCardRevealed);
    this.m_hiddenCards.OnCardSpellFinishedEvent += new EventHandler(this.OnHiddenCardSpellFinished);
    this.m_hiddenCards.OnCardSpellStateFinishedEvent += new EventHandler<Spell>(this.OnHiddenCardSpellStateFinished);
    this.m_hiddenCards.InitializeCards(this.m_HiddenCard);
    this.m_effectHandle = new ScreenEffectsHandle((object) this);
    this.InitializeUI();
  }

  private void Update()
  {
    if (!(bool) (UnityEngine.Object) this.m_Carousel)
      return;
    this.m_Carousel.UpdateUI(InputCollection.GetMouseButtonDown(0));
  }

  public void Play(int boosterId, float timeToRegisterPackOpening, int packOpeningId)
  {
    if (this.m_playing)
      return;
    this.m_playing = true;
    this.EnableCardInput(false);
    this.m_initializePackOpeningAnimationStartTime = Time.realtimeSinceStartup;
    this.StartCoroutine(this.PlayWhenReady(boosterId, timeToRegisterPackOpening, packOpeningId));
  }

  public bool IsPlaying() => this.m_playing;

  public void OnBoosterOpened(List<NetCache.BoosterCard> cards)
  {
    if (cards.Count > 5)
    {
      Debug.LogError((object) string.Format("PackOpeningDirector.OnBoosterOpened() - Not enough PackOpeningCards! Received {0} cards. There are only {1} hidden cards.", (object) cards.Count, (object) 5));
    }
    else
    {
      this.m_cardsPendingReveal = Mathf.Min(cards.Count, 5);
      this.StartCoroutine(this.m_hiddenCards.AttachBoosterCards(cards));
    }
  }

  public void OnMercenariesBoosterOpened(List<LettucePackComponent> packComponents)
  {
    if (packComponents.Count > 5)
    {
      Debug.LogError((object) string.Format("PackOpeningDirector.OnMercenariesBoosterOpened() - Not enough PackOpeningCards! Received {0} cards. There are only {1} hidden cards.", (object) packComponents.Count, (object) 5));
    }
    else
    {
      this.m_cardsPendingReveal = Mathf.Min(packComponents.Count, 5);
      this.StartCoroutine(this.m_hiddenCards.AttachBoosterMercenaries(packComponents));
    }
  }

  public void HideCardsAndDoneButton()
  {
    this.m_hiddenCards.DeactivateCards();
    if (!this.IsDoneButtonShown)
      return;
    this.HideDoneButton();
  }

  public void FinishPackOpen()
  {
    if (!this.m_doneButtonShown)
      return;
    this.m_activePackFxSpell.ActivateState(SpellStateType.DEATH);
    this.m_effectHandle.StopEffect();
    this.m_effectsPendingFinish = 11;
    this.m_effectsPendingDestroy = this.m_effectsPendingFinish;
    this.HideDoneButton();
    this.m_hiddenCards.Dissipate();
    Action onDoneOpeningPack = this.OnDoneOpeningPack;
    if (onDoneOpeningPack != null)
      onDoneOpeningPack();
    this.HideKeywordTooltips();
  }

  public void ForceRevealRandomCard() => this.m_hiddenCards.ForceRevealRandomCard();

  public static bool QuickPackOpeningAllowed
  {
    get
    {
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      return netObject != null && netObject.QuickOpenEnabled;
    }
  }

  private IEnumerator PlayWhenReady(
    int boosterId,
    float timeToRegisterPackOpening,
    int packOpeningId)
  {
    PackOpeningDirector sender = this;
    while (sender.m_loadingDoneButton)
      yield return (object) null;
    if ((UnityEngine.Object) sender.m_doneButton == (UnityEngine.Object) null)
    {
      EventHandler onFinishedEvent = sender.OnFinishedEvent;
      if (onFinishedEvent != null)
        onFinishedEvent((object) sender, EventArgs.Empty);
    }
    else
    {
      Spell spell;
      if (!sender.m_packFxSpells.TryGetValue(boosterId, out spell))
      {
        BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterId);
        bool loading = true;
        AssetLoader.Get().InstantiatePrefab(new AssetReference(record.PackOpeningFxPrefab), new PrefabCallback<GameObject>(Callback));
        while (loading)
          yield return (object) null;

        void Callback(AssetReference assetRef, GameObject go, object callbackData)
        {
          loading = false;
          this.m_packFxSpells[boosterId] = spell;
          if ((UnityEngine.Object) go == (UnityEngine.Object) null)
          {
            Error.AddDevFatal("PackOpeningDirector.PlayWhenReady() - Error loading {0} for booster id {1}", (object) assetRef, (object) boosterId);
          }
          else
          {
            spell = go.GetComponent<Spell>();
            go.transform.parent = this.transform;
            go.transform.localPosition = this.PACK_OPENING_FX_POSITION;
          }
        }
      }
      if (!(bool) (UnityEngine.Object) spell)
      {
        EventHandler onFinishedEvent = sender.OnFinishedEvent;
        if (onFinishedEvent != null)
          onFinishedEvent((object) sender, EventArgs.Empty);
      }
      else
      {
        sender.m_activePackFxSpell = spell;
        PlayMakerFSM component = spell.GetComponent<PlayMakerFSM>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          component.FsmVariables.GetFsmGameObject("CardsInsidePack").Value = sender.m_CardsInsidePack;
          component.FsmVariables.GetFsmGameObject("ClassName").Value = sender.m_ClassName;
          component.FsmVariables.GetFsmGameObject(nameof (PackOpeningDirector)).Value = sender.gameObject;
        }
        sender.m_activePackFxSpell.AddFinishedCallback(new Spell.FinishedCallback(sender.OnSpellFinished));
        float timeTillAnimationStart = Time.realtimeSinceStartup - sender.m_initializePackOpeningAnimationStartTime;
        TelemetryManager.Client().SendPackOpening(timeToRegisterPackOpening, timeTillAnimationStart, packOpeningId);
        BlizzardAttributionManager.Get().SendEvent_PackOpen(packOpeningId);
        sender.m_activePackFxSpell.ActivateState(SpellStateType.ACTION);
      }
    }
  }

  private void OnSpellFinished(Spell spell, object userData)
  {
    this.m_hiddenCards.SetInputEnabled(true);
    this.m_hiddenCards.EnableReveal();
    this.AttachCardsToCarousel();
  }

  private void CameraBlurOn() => this.m_effectHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective);

  private void AttachCardsToCarousel()
  {
    if ((UnityEngine.Object) this.m_Carousel == (UnityEngine.Object) null)
      return;
    this.m_hiddenCards.EnableCollision();
    if (PackOpeningDirector.QuickPackOpeningAllowed && (bool) UniversalInputManager.UsePhoneUI)
      this.m_hiddenCards.ShowRarityGlow();
    this.m_Carousel.Initialize(this.m_hiddenCards.ToCarouselItems().ToArray<Shared.UI.Scripts.Carousel.Carousel.Item>());
    this.m_Carousel.OnSettled += new Shared.UI.Scripts.Carousel.Carousel.SettledEventHandler(this.CarouselSettled);
    this.m_Carousel.OnStartedScrolling += new Shared.UI.Scripts.Carousel.Carousel.StartedScrollingEventHandler(this.CarouselStartedScrolling);
    this.m_Carousel.OnItemClicked += new Shared.UI.Scripts.Carousel.Carousel.ItemClickedEventHandler(this.CarouselItemClicked);
    this.m_Carousel.OnItemReleased += new Shared.UI.Scripts.Carousel.Carousel.ItemReleasedHandler(this.CarouselItemReleased);
    this.m_Carousel.OnItemCrossedCenterPosition += new Shared.UI.Scripts.Carousel.Carousel.ItemPulledEventHandler(this.CarouselItemCrossedCenterPosition);
    this.CarouselSettled();
    this.CarouselItemCrossedCenterPosition(this.m_Carousel.CurrentItem, 0);
  }

  private void CarouselItemCrossedCenterPosition(Shared.UI.Scripts.Carousel.Carousel.Item item, int index)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI || item == null || !PackOpeningDirector.QuickPackOpeningAllowed)
      return;
    PackOpeningCard component = ((PackOpeningCardCarouselItem) item).GetGameObject().GetComponent<PackOpeningCard>();
    if (component.IsRevealed())
      return;
    component.ForceReveal();
  }

  private void CarouselItemClicked(Shared.UI.Scripts.Carousel.Carousel.Item item, int index)
  {
    this.m_clickedCard = item.GetGameObject().GetComponent<PackOpeningCard>();
    this.m_clickedPosition = index;
  }

  private void CarouselItemReleased()
  {
    if (this.m_Carousel.IsScrolling)
      return;
    bool flag = !(bool) UniversalInputManager.UsePhoneUI || !PackOpeningDirector.QuickPackOpeningAllowed;
    if (this.m_clickedPosition == this.m_Carousel.CurrentIndex)
    {
      if (this.m_clickedCard.IsRevealed())
      {
        if (!flag || this.m_clickedPosition >= 4)
          return;
        this.m_Carousel.SetPosition(this.m_clickedPosition + 1, true);
      }
      else
        this.m_clickedCard.ForceReveal();
    }
    else
    {
      if (!flag)
        return;
      this.m_Carousel.SetPosition(this.m_clickedPosition, true);
    }
  }

  private void CarouselSettled()
  {
    PackOpeningCard component = ((PackOpeningCardCarouselItem) this.m_Carousel.CurrentItem).GetGameObject().GetComponent<PackOpeningCard>();
    this.m_glowingCard = component;
    component.ShowRarityGlow();
  }

  private void CarouselStartedScrolling()
  {
    if (!((UnityEngine.Object) this.m_glowingCard != (UnityEngine.Object) null) || this.m_glowingCard.GetEntityDef() == null || this.m_glowingCard.GetEntityDef().GetRarity() == TAG_RARITY.COMMON)
      return;
    this.m_glowingCard.HideRarityGlow();
  }

  private void InitializeUI()
  {
    this.m_loadingDoneButton = true;
    AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_DoneButtonPrefab, new PrefabCallback<GameObject>(this.OnDoneButtonLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnDoneButtonLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_loadingDoneButton = false;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("PackOpeningDirector.OnDoneButtonLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_doneButton = go.GetComponent<NormalButton>();
      if ((UnityEngine.Object) this.m_doneButton == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("PackOpeningDirector.OnDoneButtonLoaded() - ERROR \"{0}\" has no {1} component", (object) assetRef, (object) typeof (NormalButton)));
      }
      else
      {
        LayerUtils.SetLayer(this.m_doneButton.gameObject, GameLayer.IgnoreFullScreenEffects);
        this.m_doneButton.transform.parent = this.transform;
        TransformUtil.CopyWorld((Component) this.m_doneButton, (Component) global::PackOpening.Get().m_Bones.m_DoneButton);
        RenderUtils.EnableRenderersAndColliders(this.m_doneButton.gameObject, false);
      }
    }
  }

  private void ShowDoneButton()
  {
    this.m_doneButtonShown = true;
    RenderUtils.EnableRenderersAndColliders(this.m_doneButton.gameObject, true);
    Spell component = this.m_doneButton.m_button.GetComponent<Spell>();
    component.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonShown));
    component.ActivateState(SpellStateType.BIRTH);
  }

  private void OnDoneButtonShown(Spell spell, object userData) => this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));

  private void HideDoneButton()
  {
    this.m_doneButtonShown = false;
    RenderUtils.EnableColliders(this.m_doneButton.gameObject, false);
    this.m_doneButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
    Spell component = this.m_doneButton.m_button.GetComponent<Spell>();
    component.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonHidden));
    component.ActivateState(SpellStateType.DEATH);
  }

  private void OnDoneButtonHidden(Spell spell, object userData)
  {
    this.OnEffectFinished();
    this.OnEffectDone();
  }

  private void OnDoneButtonPressed(UIEvent e)
  {
    this.HideKeywordTooltips();
    this.FinishPackOpen();
  }

  private void HideKeywordTooltips()
  {
    this.m_hiddenCards.RemoveOnOverWhileFlippedListeners();
    TooltipPanelManager.Get().HideKeywordHelp();
  }

  private void EnableCardInput(bool enable) => this.m_hiddenCards.SetInputEnabled(enable);

  private void OnCardRevealed(object userData, EventArgs eventArgs)
  {
    PackOpeningCard packOpeningCard = (PackOpeningCard) userData;
    if (packOpeningCard.GetEntityDef().GetRarity() == TAG_RARITY.LEGENDARY && (UnityEngine.Object) packOpeningCard.GetActor() != (UnityEngine.Object) null)
    {
      if (packOpeningCard.GetActor().GetPremium() == TAG_PREMIUM.GOLDEN)
        BnetPresenceMgr.Get().SetGameField(4U, packOpeningCard.GetCardId() + ",1");
      else
        BnetPresenceMgr.Get().SetGameField(4U, packOpeningCard.GetCardId() + ",0");
    }
    --this.m_cardsPendingReveal;
    if (this.m_cardsPendingReveal > 0)
      return;
    AchievementManager.Get().UnpauseToastNotifications();
    this.ShowDoneButton();
  }

  private void OnHiddenCardSpellFinished(object userData, EventArgs eventArgs) => this.OnEffectFinished();

  private void OnHiddenCardSpellStateFinished(object sender, Spell spell)
  {
    if ((UnityEngine.Object) spell != (UnityEngine.Object) null && spell.GetActiveState() != SpellStateType.NONE)
      return;
    this.OnEffectDone();
  }

  private void OnEffectFinished()
  {
    --this.m_effectsPendingFinish;
    if (this.m_effectsPendingFinish > 0)
      return;
    EventHandler onFinishedEvent = this.OnFinishedEvent;
    if (onFinishedEvent == null)
      return;
    onFinishedEvent((object) this, EventArgs.Empty);
  }

  private void OnEffectDone()
  {
    --this.m_effectsPendingDestroy;
    if (this.m_effectsPendingDestroy > 0)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }
}
