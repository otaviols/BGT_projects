using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneIndicatorVisual : MonoBehaviour
{
  public RuneButton[] runeButtons;
  public RuneButton draggableButton;
  public Transform draggedCardsContainer;
  public int maxDraggedCardsToShow = 5;
  public float tooltipScale = 0.085f;
  public float tooltipDelay = 1f;
  public Transform cardCountContainer;
  public UberText cardCountText;
  public Vector3 draggedTileOffset = new Vector3(0.0f, 0.0f, 0.42f);
  private RuneButton m_currentDraggedButton;
  private CollectionDeck m_currentDeck;
  private CollectionDeckTray m_deckTray;
  private Coroutine m_showTooltipCoroutine;
  private List<string> m_cardsToRemove = new List<string>();
  private readonly List<DeckTrayDeckTileVisual> m_draggedTiles = new List<DeckTrayDeckTileVisual>();
  private const int INITIAL_DECK_TILE_POOL_SIZE = 5;
  private Stack<DeckTrayDeckTileVisual> m_deckTilePool = new Stack<DeckTrayDeckTileVisual>();

  public static event Action<RunePattern> RunePatternChanged;

  private void OnEnable()
  {
    this.runeButtons[0].AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRuneButtonClicked));
    this.runeButtons[1].AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRuneButtonClicked));
    this.runeButtons[2].AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRuneButtonClicked));
    this.runeButtons[0].AddEventListener(UIEventType.DRAG, new UIEvent.Handler(this.OnRuneButtonDragged));
    this.runeButtons[1].AddEventListener(UIEventType.DRAG, new UIEvent.Handler(this.OnRuneButtonDragged));
    this.runeButtons[2].AddEventListener(UIEventType.DRAG, new UIEvent.Handler(this.OnRuneButtonDragged));
    this.runeButtons[0].AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRuneButtonOver));
    this.runeButtons[1].AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRuneButtonOver));
    this.runeButtons[2].AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRuneButtonOver));
    this.runeButtons[0].AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRuneButtonOut));
    this.runeButtons[1].AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRuneButtonOut));
    this.runeButtons[2].AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRuneButtonOut));
    CollectionDeckTray.DeckTrayCardAdded += new Action<CollectionDeck, RunePattern>(this.OnDeckTrayCardAdded);
  }

  private void OnDisable()
  {
    this.runeButtons[0].RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRuneButtonClicked));
    this.runeButtons[1].RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRuneButtonClicked));
    this.runeButtons[2].RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRuneButtonClicked));
    this.runeButtons[0].RemoveEventListener(UIEventType.DRAG, new UIEvent.Handler(this.OnRuneButtonDragged));
    this.runeButtons[1].RemoveEventListener(UIEventType.DRAG, new UIEvent.Handler(this.OnRuneButtonDragged));
    this.runeButtons[2].RemoveEventListener(UIEventType.DRAG, new UIEvent.Handler(this.OnRuneButtonDragged));
    this.runeButtons[0].RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRuneButtonOver));
    this.runeButtons[1].RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRuneButtonOver));
    this.runeButtons[2].RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRuneButtonOver));
    this.runeButtons[0].RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRuneButtonOut));
    this.runeButtons[1].RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRuneButtonOut));
    this.runeButtons[2].RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRuneButtonOut));
    CollectionDeckTray.DeckTrayCardAdded -= new Action<CollectionDeck, RunePattern>(this.OnDeckTrayCardAdded);
  }

  public void Initialize(CollectionDeck deck, CollectionDeckTray deckTray)
  {
    this.m_currentDeck = deck;
    this.m_deckTray = deckTray;
    for (int index = 0; index < this.runeButtons.Length; ++index)
      this.runeButtons[index].Initialize(index, deck.GetRuneAtIndex(index));
  }

  public void InitializeWithTilePool(CollectionDeck deck, CollectionDeckTray deckTray)
  {
    this.Initialize(deck, deckTray);
    this.m_deckTilePool.Clear();
    for (int index = 0; index < 5; ++index)
    {
      DeckTrayDeckTileVisual cardTileVisual = this.m_deckTray.GetCardsContent().CreateCardTileVisual("tileClone", this.draggedCardsContainer);
      cardTileVisual.Hide();
      this.m_deckTilePool.Push(cardTileVisual);
    }
  }

  private void Update()
  {
    if (InputCollection.GetMouseButtonUp(0))
    {
      this.DropButton();
    }
    else
    {
      RaycastHit hitInfo;
      if (!(bool) (UnityEngine.Object) this.m_currentDraggedButton || !UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.DragPlane.LayerBit(), out hitInfo))
        return;
      Vector3 point = hitInfo.point;
      if ((bool) UniversalInputManager.UsePhoneUI)
        point.y += (float) InputMgr.PHONE_HEIGHT_OFFSET;
      this.draggableButton.transform.position = point;
    }
  }

  private void OnDeckTrayCardAdded(CollectionDeck deck, RunePattern cardRunesAdded)
  {
    RunePattern runesFromButtons = this.GetRunesFromButtons();
    foreach (RuneType validRuneType in RunePattern.ValidRuneTypes)
    {
      int cost = cardRunesAdded.GetCost(validRuneType);
      if (cost != 0)
      {
        int num1 = cost - runesFromButtons.GetCost(validRuneType);
        if (num1 > 0)
        {
          int num2 = 0;
          foreach (RuneButton runeButton in this.runeButtons)
          {
            if (runeButton.RuneType == RuneType.RT_NONE && num2 < num1)
            {
              runeButton.SetRune(validRuneType, true);
              ++num2;
            }
          }
        }
      }
    }
    if (!cardRunesAdded.HasMaxAmountOfOneRuneType)
      return;
    TutorialDeathKnightDeckBuilding.ShowTutorial(UIVoiceLinesManager.TriggerType.ADDED_TRIPLE_DEATH_KNIGHT_RUNES);
  }

  private void OnRuneButtonClicked(UIEvent e)
  {
    RuneButton element = e.GetElement() as RuneButton;
    if (!(bool) (UnityEngine.Object) element)
      return;
    element.ShowNextRune();
    this.m_currentDeck.SetRuneAtIndex(element.ButtonIndex, element.RuneType);
    element.SetHighlighted(true);
    Action<RunePattern> runePatternChanged = RuneIndicatorVisual.RunePatternChanged;
    if (runePatternChanged != null)
      runePatternChanged(this.m_currentDeck.Runes);
    this.HideTooltip(element);
  }

  private void OnRuneButtonOver(UIEvent e)
  {
    RuneButton element = e.GetElement() as RuneButton;
    if (!(bool) (UnityEngine.Object) element)
      return;
    this.ShowTooltip(element);
    element.SetHighlighted(true);
  }

  private void OnRuneButtonOut(UIEvent e)
  {
    RuneButton element = e.GetElement() as RuneButton;
    if (!(bool) (UnityEngine.Object) element)
      return;
    element.SetHighlighted(false);
    this.HideTooltip(element);
  }

  private void OnRuneButtonDragged(UIEvent e)
  {
    RuneButton element = e.GetElement() as RuneButton;
    if (!(bool) (UnityEngine.Object) element || element.RuneType == RuneType.RT_NONE || (bool) (UnityEngine.Object) this.m_currentDraggedButton)
      return;
    int cost = this.m_currentDeck.Runes.GetCost(element.RuneType);
    List<EntityDef> remainingCards = new List<EntityDef>();
    this.m_cardsToRemove = this.m_deckTray.GetCardsContent().GetCardIdsMatchingOrAboveRuneCost(element.RuneType, cost, remainingCards);
    bool usePhoneUi = (bool) UniversalInputManager.UsePhoneUI;
    int cardCount = 0;
    if (this.m_cardsToRemove.Count > 0)
    {
      this.m_draggedTiles.Clear();
      for (int index = 0; index < this.m_cardsToRemove.Count; ++index)
      {
        string cardID = this.m_cardsToRemove[index];
        DeckTrayDeckTileVisual cardTileVisual = this.m_deckTray.GetCardsContent().GetCardTileVisual(cardID);
        cardCount += cardTileVisual.GetSlot().Count;
        if (!usePhoneUi || index < this.maxDraggedCardsToShow)
          this.StartCoroutine(this.CreateDraggableDeckTile(this.m_cardsToRemove[index], index, (Action<DeckTrayDeckTileVisual>) (tile =>
          {
            if (!((UnityEngine.Object) tile != (UnityEngine.Object) null))
              return;
            this.m_draggedTiles.Add(tile);
          })));
      }
    }
    this.GrabButton(element);
    this.UpdateDraggedCardCountText(usePhoneUi, cardCount);
    this.HideTooltip(element);
  }

  private void ShowTooltip(RuneButton button)
  {
    if (this.m_showTooltipCoroutine != null)
      this.StopCoroutine(this.m_showTooltipCoroutine);
    TooltipZone component = button.gameObject.GetComponent<TooltipZone>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    this.m_showTooltipCoroutine = this.StartCoroutine(this.ShowRuneTooltip(component));
  }

  private void HideTooltip(RuneButton button)
  {
    if (this.m_showTooltipCoroutine != null)
      this.StopCoroutine(this.m_showTooltipCoroutine);
    TooltipZone component = button.gameObject.GetComponent<TooltipZone>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.HideTooltip();
  }

  private IEnumerator ShowRuneTooltip(TooltipZone tooltip)
  {
    yield return (object) new WaitForSeconds(this.tooltipDelay);
    tooltip.ShowBoxTooltip(GameStrings.Get("GLUE_COLLECTION_RUNES_TOOLTIP_HEADER"), GameStrings.Get("GLUE_COLLECTION_RUNES_TOOLTIP_DESC"));
    tooltip.Scale = this.tooltipScale;
  }

  private IEnumerator CreateDraggableDeckTile(
    string cardId,
    int index,
    Action<DeckTrayDeckTileVisual> callback)
  {
    DeckTrayDeckTileVisual cardTileVisual = this.m_deckTray.GetCardsContent().GetCardTileVisual(cardId);
    cardTileVisual.SetPendingRemoval(true);
    DeckTrayDeckTileVisual tileClone = this.GetDraggableClone(cardTileVisual, index);
    tileClone.Show();
    callback(tileClone);
    yield return (object) new WaitUntil((Func<bool>) (() => tileClone.isActiveAndEnabled));
    tileClone.GetActor().GetSpell(SpellType.SUMMON_IN).ActivateState(SpellStateType.BIRTH);
  }

  private DeckTrayDeckTileVisual GetDraggableClone(
    DeckTrayDeckTileVisual tileVisual,
    int index)
  {
    if (this.m_deckTilePool.Count <= 0)
      return this.CreateDraggableClone(tileVisual, index);
    DeckTrayDeckTileVisual tileClone = this.m_deckTilePool.Pop();
    this.InitializeDraggableClone(tileClone, tileVisual, index);
    return tileClone;
  }

  private void UpdateDraggedCardCountText(bool shouldShowCardCount, int cardCount)
  {
    if (!shouldShowCardCount)
      return;
    if ((bool) (UnityEngine.Object) this.cardCountContainer)
      this.cardCountContainer.gameObject.SetActive(cardCount > 1);
    if (!(bool) (UnityEngine.Object) this.cardCountText)
      return;
    this.cardCountText.Text = cardCount.ToString();
  }

  private void GrabButton(RuneButton runeButton)
  {
    this.m_currentDraggedButton = runeButton;
    this.draggableButton.gameObject.SetActive(true);
    this.draggableButton.SetRune(runeButton.RuneType, false);
    this.draggableButton.PlayDragEffect();
    this.m_currentDeck.SetRuneAtIndex(runeButton.ButtonIndex, RuneType.RT_NONE);
    runeButton.SetRune(RuneType.RT_NONE, false);
    Action<RunePattern> runePatternChanged = RuneIndicatorVisual.RunePatternChanged;
    if (runePatternChanged == null)
      return;
    runePatternChanged(this.m_currentDeck.Runes);
  }

  private void DropButton()
  {
    if (!(bool) (UnityEngine.Object) this.m_currentDraggedButton)
      return;
    bool flag = this.m_deckTray.MouseIsOver(Box.Get().GetCamera());
    if (flag)
    {
      this.m_currentDraggedButton.SetRune(this.draggableButton.RuneType, true);
      this.m_currentDeck.SetRuneAtIndex(this.m_currentDraggedButton.ButtonIndex, this.m_currentDraggedButton.RuneType);
      Action<RunePattern> runePatternChanged = RuneIndicatorVisual.RunePatternChanged;
      if (runePatternChanged != null)
        runePatternChanged(this.m_currentDeck.Runes);
    }
    else
    {
      if (this.m_currentDeck.Runes.CombinedValue == DeckRule_DeathKnightRuneLimit.MaxRuneSlots - 1)
        TutorialDeathKnightDeckBuilding.ShowTutorial(UIVoiceLinesManager.TriggerType.REMOVED_THIRD_RUNE);
      this.m_currentDraggedButton.SetRune(RuneType.RT_NONE, true);
      this.m_currentDeck.SetRuneAtIndex(this.m_currentDraggedButton.ButtonIndex, RuneType.RT_NONE);
      Action<RunePattern> runePatternChanged = RuneIndicatorVisual.RunePatternChanged;
      if (runePatternChanged != null)
        runePatternChanged(this.m_currentDeck.Runes);
    }
    DeckTrayCardListContent cardsContent = this.m_deckTray.GetCardsContent();
    foreach (string cardID in this.m_cardsToRemove)
      cardsContent.GetCardTileVisual(cardID).SetPendingRemoval(false);
    foreach (string cardID in this.m_cardsToRemove)
    {
      DeckTrayDeckTileVisual cardTileVisual = cardsContent.GetCardTileVisual(cardID);
      if (!flag)
        this.m_deckTray.RemoveAllCopiesOfCard(cardTileVisual.GetCardID());
    }
    this.m_cardsToRemove.Clear();
    this.m_currentDraggedButton = (RuneButton) null;
    this.draggableButton.StopDragEffect();
    this.draggableButton.gameObject.SetActive(false);
    if (this.m_draggedTiles.Count > 0)
    {
      for (int index = 0; index < this.m_draggedTiles.Count; ++index)
      {
        DeckTrayDeckTileVisual draggedTile = this.m_draggedTiles[index];
        draggedTile.Hide();
        this.m_deckTilePool.Push(draggedTile);
      }
    }
    this.m_draggedTiles.Clear();
  }

  private RunePattern GetRunesFromButtons()
  {
    RunePattern runesFromButtons = new RunePattern();
    foreach (RuneButton runeButton in this.runeButtons)
      runesFromButtons.AddRunes(runeButton.RuneType, 1);
    return runesFromButtons;
  }

  private DeckTrayDeckTileVisual CreateDraggableClone(
    DeckTrayDeckTileVisual tileVisual,
    int index)
  {
    DeckTrayDeckTileVisual cardTileVisual = this.m_deckTray.GetCardsContent().CreateCardTileVisual(tileVisual.name + " Preview", this.draggedCardsContainer);
    this.InitializeDraggableClone(cardTileVisual, tileVisual, index);
    return cardTileVisual;
  }

  private void InitializeDraggableClone(
    DeckTrayDeckTileVisual tileClone,
    DeckTrayDeckTileVisual tileVisual,
    int index)
  {
    bool offsetCardNameForRunes = tileVisual.HasRuneCost();
    tileClone.SetSlot(this.m_currentDeck, tileVisual.GetSlot(), false, offsetCardNameForRunes);
    tileClone.transform.rotation = Quaternion.identity;
    float x = (float) index * this.draggedTileOffset.x;
    float y = (float) index * this.draggedTileOffset.y;
    float z = (float) index * this.draggedTileOffset.z;
    tileClone.transform.localPosition = new Vector3(x, y, z);
  }

  public void Show() => this.gameObject.SetActive(true);

  public void Hide() => this.gameObject.SetActive(false);

  public void EnableRuneButtons()
  {
    for (int index = 0; index < this.runeButtons.Length; ++index)
      this.runeButtons[index].SetEnabled(true);
  }

  public void DisableRuneButtons()
  {
    for (int index = 0; index < this.runeButtons.Length; ++index)
      this.runeButtons[index].SetEnabled(false);
  }

  public void ResetRuneButtons()
  {
    if (this.m_currentDeck == null)
      return;
    foreach (RuneButton runeButton in this.runeButtons)
    {
      runeButton.SetRune(RuneType.RT_NONE, true);
      Action<RunePattern> runePatternChanged = RuneIndicatorVisual.RunePatternChanged;
      if (runePatternChanged != null)
        runePatternChanged(this.m_currentDeck.Runes);
    }
  }

  public void HighlightAllRunes(bool highlight)
  {
    foreach (RuneButton runeButton in this.runeButtons)
      runeButton.SetHighlighted(highlight);
  }
}
