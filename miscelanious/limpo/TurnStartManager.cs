using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStartManager : MonoBehaviour
{
  public TurnStartIndicator m_turnStartPrefab;
  public List<TurnStartIndicator> m_alternativeTurnStartPrefab;
  public Spell m_OpponentExtraTurnSpell;
  public Spell m_FriendlyExtraTurnSpell;
  private static TurnStartManager s_instance;
  private TurnStartIndicator m_turnStartInstance;
  private Spell m_opponentExtraTurnSpellInstance;
  private Spell m_friendlyExtraTurnSpellInstance;
  private bool m_listeningForTurnEvents;
  private int m_manaCrystalsGained;
  private int m_manaCrystalsFilled;
  private List<Card> m_cardsToDraw = new List<Card>();
  private List<TurnStartManager.CardChange> m_exhaustedChangesToHandle = new List<TurnStartManager.CardChange>();
  private SpellController m_spellController;
  private bool m_blockingInput;
  private bool m_twoScoopsDisplayed;
  private bool m_twoScoopsRequestFromMetadata;

  private void Awake()
  {
    TurnStartManager.s_instance = this;
    if (GameState.Get() == null)
    {
      Debug.LogError((object) string.Format("TurnStartManager.Awake() - GameState already Shutdown before TurnStartManager was loaded."));
    }
    else
    {
      if (GameState.Get().IsGameCreated())
        this.StartCoroutine(this.InstantiateTurnStartIndicator());
      else
        GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
      GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    }
  }

  private void OnDestroy() => TurnStartManager.s_instance = (TurnStartManager) null;

  public static TurnStartManager Get() => TurnStartManager.s_instance;

  public bool IsListeningForTurnEvents() => this.m_listeningForTurnEvents;

  public void BeginListeningForTurnEvents(bool fromMetadata = false)
  {
    this.m_cardsToDraw.Clear();
    this.m_exhaustedChangesToHandle.Clear();
    this.m_manaCrystalsGained = 0;
    this.m_manaCrystalsFilled = 0;
    this.m_twoScoopsDisplayed = false;
    this.m_listeningForTurnEvents = true;
    this.m_blockingInput = true;
    this.m_twoScoopsRequestFromMetadata = fromMetadata;
  }

  public void NotifyOfManaCrystalGained(int amount) => this.m_manaCrystalsGained += amount;

  public void NotifyOfManaCrystalFilled(int amount) => this.m_manaCrystalsFilled += amount;

  public void NotifyOfCardDrawn(Entity drawnEntity) => this.m_cardsToDraw.Add(drawnEntity.GetCard());

  public void NotifyOfExhaustedChange(Card card, TagDelta tagChange) => this.m_exhaustedChangesToHandle.Add(new TurnStartManager.CardChange()
  {
    m_card = card,
    m_tagDelta = tagChange
  });

  private int GetCurrentAlternativeAppearanceIndex()
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return 0;
    GameEntity gameEntity = gameState.GetGameEntity();
    return gameEntity == null ? 0 : gameEntity.GetTag(GAME_TAG.TURN_INDICATOR_ALTERNATIVE_APPEARANCE);
  }

  private bool IsTurnStartIndicatorDisabled()
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return false;
    GameEntity gameEntity = gameState.GetGameEntity();
    return gameEntity != null && gameEntity.GetTag(GAME_TAG.DISABLE_TURN_INDICATORS) > 0;
  }

  public void ApplyAlternativeAppearance() => this.StartCoroutine(TurnStartManager.Get().InstantiateTurnStartIndicator());

  private IEnumerator InstantiateTurnStartIndicator()
  {
    TurnStartManager turnStartManager = this;
    if ((UnityEngine.Object) turnStartManager.m_turnStartInstance != (UnityEngine.Object) null)
    {
      while (turnStartManager.m_turnStartInstance.IsShown())
        yield return (object) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) turnStartManager.m_turnStartInstance);
    }
    if (!turnStartManager.IsTurnStartIndicatorDisabled())
    {
      int alternativeAppearanceIndex = turnStartManager.GetCurrentAlternativeAppearanceIndex();
      if (alternativeAppearanceIndex == 0)
        turnStartManager.m_turnStartInstance = UnityEngine.Object.Instantiate<TurnStartIndicator>(turnStartManager.m_turnStartPrefab);
      else if (turnStartManager.m_alternativeTurnStartPrefab.Count >= alternativeAppearanceIndex)
        turnStartManager.m_turnStartInstance = UnityEngine.Object.Instantiate<TurnStartIndicator>(turnStartManager.m_alternativeTurnStartPrefab[alternativeAppearanceIndex - 1]);
      if ((UnityEngine.Object) turnStartManager.m_turnStartInstance != (UnityEngine.Object) null)
        turnStartManager.m_turnStartInstance.transform.parent = turnStartManager.transform;
      else
        Debug.LogError((object) string.Format("TurnStartManager.InstantiateTurnStartIndicator() - FAILED to instantiate turn start prefab for appearance {0}", (object) alternativeAppearanceIndex));
    }
  }

  public Spell GetExtraTurnSpell(bool isFriendly = true)
  {
    Spell turnSpellInstance = this.m_friendlyExtraTurnSpellInstance;
    if (!isFriendly)
      turnSpellInstance = this.m_opponentExtraTurnSpellInstance;
    return turnSpellInstance;
  }

  public Spell SetExtraTurnSpell(Spell extraTurnSpell, bool isFriendly = true)
  {
    if (isFriendly)
      this.m_friendlyExtraTurnSpellInstance = extraTurnSpell;
    else
      this.m_opponentExtraTurnSpellInstance = extraTurnSpell;
    return extraTurnSpell;
  }

  public void NotifyOfExtraTurn(Spell extraTurnSpell, bool isEnding = false, bool isFriendly = true)
  {
    if (!isEnding)
    {
      if ((UnityEngine.Object) extraTurnSpell == (UnityEngine.Object) null)
      {
        extraTurnSpell = !isFriendly ? SpellManager.Get().GetSpell(this.m_OpponentExtraTurnSpell) : SpellManager.Get().GetSpell(this.m_FriendlyExtraTurnSpell);
        extraTurnSpell.Activate();
      }
    }
    else if ((UnityEngine.Object) extraTurnSpell != (UnityEngine.Object) null)
    {
      extraTurnSpell.ActivateState(SpellStateType.DEATH);
      extraTurnSpell.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
      {
        if (spell.GetActiveState() != SpellStateType.NONE)
          return;
        SpellManager.Get().ReleaseSpell(spell);
      }));
      extraTurnSpell = (Spell) null;
    }
    this.SetExtraTurnSpell(extraTurnSpell, isFriendly);
  }

  public void NotifyOfSpellController(SpellController spellController)
  {
    this.m_spellController = spellController;
    this.BeginPlayingTurnEvents();
  }

  public void NotifyOfStartOfTurnChoice() => this.BeginPlayingTurnEvents();

  public SpellController GetSpellController() => this.m_spellController;

  public int GetNumCardsToDraw() => this.m_cardsToDraw.Count;

  public List<Card> GetCardsToDraw() => this.m_cardsToDraw;

  public bool IsCardDrawHandled(Card card) => !((UnityEngine.Object) card == (UnityEngine.Object) null) && this.m_cardsToDraw.Contains(card);

  public void DrawCardImmediately(Card card)
  {
    int num = this.m_cardsToDraw.IndexOf(card);
    if (num < 0)
      return;
    Card[] array = this.m_cardsToDraw.GetRange(0, num + 1).ToArray();
    this.m_cardsToDraw.RemoveRange(0, num + 1);
    this.StartCoroutine(this.DrawCardsImmediatelyWithTiming(array));
  }

  private IEnumerator DrawCardsImmediatelyWithTiming(Card[] cards)
  {
    Card[] cardArray = cards;
    for (int index = 0; index < cardArray.Length; ++index)
    {
      Card card = cardArray[index];
      while (card.IsActorLoading())
        yield return (object) null;
      card.DrawFriendlyCard();
      card = (Card) null;
    }
    cardArray = (Card[]) null;
  }

  public void BeginPlayingTurnEvents() => this.StartCoroutine(this.RunTurnEventsWithTiming());

  public void NotifyOfTriggerVisual() => this.DisplayTwoScoops();

  public bool IsBlockingInput() => this.m_blockingInput;

  public bool IsTurnStartIndicatorShowing() => !((UnityEngine.Object) this.m_turnStartInstance == (UnityEngine.Object) null) && this.m_turnStartInstance.IsShown();

  private void DisplayTwoScoops()
  {
    if (this.m_twoScoopsDisplayed)
      return;
    this.m_twoScoopsDisplayed = true;
    if ((UnityEngine.Object) this.m_turnStartInstance == (UnityEngine.Object) null)
      return;
    this.m_turnStartInstance.SetReminderText(GameState.Get().GetGameEntity().GetTurnStartReminderText());
    this.m_turnStartInstance.Show();
    SoundManager.Get().LoadAndPlay((AssetReference) "ALERT_YourTurn_0v2.prefab:201bcb34d33384e48ab226f7e797771f");
  }

  private IEnumerator RunTurnEventsWithTiming()
  {
    if (this.IsListeningForTurnEvents())
    {
      this.m_listeningForTurnEvents = false;
      if (GameMgr.Get().IsAI() && !this.m_twoScoopsDisplayed && !this.m_twoScoopsRequestFromMetadata)
        yield return (object) new WaitForSeconds(1f);
      this.DisplayTwoScoops();
      Player friendlyPlayer = GameState.Get().GetFriendlySidePlayer();
      friendlyPlayer.ResetUnresolvedManaToBeReadied();
      friendlyPlayer.ReadyManaCrystal(this.m_manaCrystalsFilled);
      friendlyPlayer.AddManaCrystal(this.m_manaCrystalsGained, true);
      friendlyPlayer.UpdateManaCounter();
      this.HandleExhaustedChanges();
      if ((UnityEngine.Object) this.m_turnStartInstance != (UnityEngine.Object) null && this.m_turnStartInstance.IsShown())
        yield return (object) new WaitForSeconds(this.m_turnStartInstance.GetDesiredDelayDuration());
      if (this.m_cardsToDraw.Count > 0)
      {
        Card[] cardsToDraw = this.m_cardsToDraw.ToArray();
        this.m_cardsToDraw.Clear();
        friendlyPlayer.GetHandZone().UpdateLayout();
        Card[] cardArray = cardsToDraw;
        for (int index = 0; index < cardArray.Length; ++index)
        {
          Card card = cardArray[index];
          while (card.IsActorLoading())
            yield return (object) null;
          card.DrawFriendlyCard();
          card = (Card) null;
        }
        cardArray = (Card[]) null;
        while (!this.AreDrawnCardsReady(cardsToDraw))
          yield return (object) null;
        if (this.HasActionsAfterCardDraw())
          yield return (object) new WaitForSeconds(0.35f);
        cardsToDraw = (Card[]) null;
      }
      if ((bool) (UnityEngine.Object) this.m_spellController)
      {
        this.m_spellController.DoPowerTaskList();
        while (this.m_spellController.IsProcessingTaskList())
          yield return (object) null;
        this.m_spellController = (SpellController) null;
      }
      if (GameState.Get().IsLocalSidePlayerTurn())
      {
        this.m_blockingInput = false;
        EndTurnButton.Get().OnTurnStartManagerFinished();
        GameState.Get().GetGameEntity().OnTurnStartManagerFinished();
        if (GameState.Get().IsInMainOptionMode())
          GameState.Get().EnterMainOptionMode();
        GameState.Get().FireFriendlyTurnStartedEvent();
      }
    }
  }

  private bool AreDrawnCardsReady(Card[] cardsToDraw) => !(bool) (UnityEngine.Object) Array.Find<Card>(cardsToDraw, (Predicate<Card>) (card => !card.IsActorReady()));

  private bool HasActionsAfterCardDraw()
  {
    if ((UnityEngine.Object) this.m_spellController != (UnityEngine.Object) null)
      return true;
    Network.EntityChoices friendlyEntityChoices = GameState.Get().GetFriendlyEntityChoices();
    return friendlyEntityChoices != null && friendlyEntityChoices.ChoiceType == CHOICE_TYPE.GENERAL;
  }

  private void HandleExhaustedChanges()
  {
    foreach (TurnStartManager.CardChange cardChange in this.m_exhaustedChangesToHandle)
    {
      Card card = cardChange.m_card;
      if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
      {
        switch (card.GetEntity().GetZone())
        {
          case TAG_ZONE.PLAY:
          case TAG_ZONE.SECRET:
            card.ShowExhaustedChange(cardChange.m_tagDelta.newValue);
            continue;
          default:
            continue;
        }
      }
    }
    this.m_exhaustedChangesToHandle.Clear();
  }

  private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
  {
    if (phase != GameState.CreateGamePhase.CREATED)
      return;
    GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
    this.StartCoroutine(this.InstantiateTurnStartIndicator());
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData) => this.StopAllCoroutines();

  private class CardChange
  {
    public Card m_card;
    public TagDelta m_tagDelta;
  }
}
