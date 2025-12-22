using Blizzard.T5.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class ChoiceCardMgr : MonoBehaviour
{
  public ChoiceCardMgr.CommonData m_CommonData = new ChoiceCardMgr.CommonData();
  public ChoiceCardMgr.ChoiceData m_ChoiceData = new ChoiceCardMgr.ChoiceData();
  public ChoiceCardMgr.SubOptionData m_SubOptionData = new ChoiceCardMgr.SubOptionData();
  public List<ChoiceCardMgr.TagSpecificChoiceEffect> m_TagSpecificChoiceEffectData = new List<ChoiceCardMgr.TagSpecificChoiceEffect>();
  public List<ChoiceCardMgr.CardSpecificChoiceEffect> m_CardSpecificChoiceEffectData = new List<ChoiceCardMgr.CardSpecificChoiceEffect>();
  public List<ChoiceCardMgr.TagPostChoiceEffect> m_TagPostChoiceEffectData = new List<ChoiceCardMgr.TagPostChoiceEffect>();
  private ChoiceCardMgr.ChoiceEffectData m_DiscoverChoiceEffectData = new ChoiceCardMgr.ChoiceEffectData();
  private ChoiceCardMgr.ChoiceEffectData m_AdaptChoiceEffectData = new ChoiceCardMgr.ChoiceEffectData();
  private ChoiceCardMgr.ChoiceEffectData m_GearsChoiceEffectData = new ChoiceCardMgr.ChoiceEffectData();
  private ChoiceCardMgr.ChoiceEffectData m_DragonChoiceEffectData = new ChoiceCardMgr.ChoiceEffectData();
  private static readonly Vector3 INVISIBLE_SCALE = new Vector3(0.0001f, 0.0001f, 0.0001f);
  private static ChoiceCardMgr s_instance;
  private ChoiceCardMgr.SubOptionState m_subOptionState;
  private ChoiceCardMgr.SubOptionState m_pendingCancelSubOptionState;
  private Dictionary<int, ChoiceCardMgr.ChoiceState> m_choiceStateMap = new Dictionary<int, ChoiceCardMgr.ChoiceState>();
  private Banner m_choiceBanner;
  private NormalButton m_toggleChoiceButton;
  private NormalButton m_confirmChoiceButton;
  private bool m_friendlyChoicesShown;
  private bool m_restoreEnlargedHand;
  private ChoiceCardMgr.ChoiceState m_lastShownChoiceState;

  private void Awake()
  {
    ChoiceCardMgr.s_instance = this;
    foreach (ChoiceCardMgr.TagSpecificChoiceEffect specificChoiceEffect in this.m_TagSpecificChoiceEffectData)
    {
      switch (specificChoiceEffect.m_Tag)
      {
        case GAME_TAG.ADAPT:
          if (specificChoiceEffect.m_ValueSpellMap.Count > 0)
          {
            this.m_AdaptChoiceEffectData = specificChoiceEffect.m_ValueSpellMap[0].m_ChoiceEffectData;
            continue;
          }
          continue;
        case GAME_TAG.GEARS:
          if (specificChoiceEffect.m_ValueSpellMap.Count > 0)
          {
            this.m_GearsChoiceEffectData = specificChoiceEffect.m_ValueSpellMap[0].m_ChoiceEffectData;
            continue;
          }
          continue;
        case GAME_TAG.USE_DISCOVER_VISUALS:
          if (specificChoiceEffect.m_ValueSpellMap.Count > 0)
          {
            this.m_DiscoverChoiceEffectData = specificChoiceEffect.m_ValueSpellMap[0].m_ChoiceEffectData;
            continue;
          }
          continue;
        case GAME_TAG.GOOD_OL_GENERIC_FRIENDLY_DRAGON_DISCOVER_VISUALS:
          if (specificChoiceEffect.m_ValueSpellMap.Count > 0)
          {
            this.m_DragonChoiceEffectData = specificChoiceEffect.m_ValueSpellMap[0].m_ChoiceEffectData;
            continue;
          }
          continue;
        default:
          continue;
      }
    }
  }

  private void OnDestroy() => ChoiceCardMgr.s_instance = (ChoiceCardMgr) null;

  private void Start()
  {
    if (GameState.Get() == null)
    {
      Debug.LogError((object) string.Format("ChoiceCardMgr.Start() - GameState already Shutdown before ChoiceCardMgr was loaded."));
    }
    else
    {
      GameState.Get().RegisterEntityChoicesReceivedListener(new GameState.EntityChoicesReceivedCallback(this.OnEntityChoicesReceived));
      GameState.Get().RegisterEntitiesChosenReceivedListener(new GameState.EntitiesChosenReceivedCallback(this.OnEntitiesChosenReceived));
      GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    }
  }

  public static ChoiceCardMgr Get() => ChoiceCardMgr.s_instance;

  public bool RestoreEnlargedHandAfterChoice() => this.m_restoreEnlargedHand;

  public Banner GetChoiceBanner() => this.m_choiceBanner;

  public NormalButton GetToggleButton() => this.m_toggleChoiceButton;

  public List<Card> GetFriendlyCards()
  {
    if (this.m_subOptionState != null)
      return this.m_subOptionState.m_cards;
    ChoiceCardMgr.ChoiceState choiceState;
    return this.m_choiceStateMap.TryGetValue(GameState.Get().GetFriendlyPlayerId(), out choiceState) ? choiceState.m_cards : (List<Card>) null;
  }

  public bool IsShown() => this.m_subOptionState != null || this.m_choiceStateMap.Count > 0;

  public bool IsFriendlyShown() => this.m_subOptionState != null || this.m_choiceStateMap.ContainsKey(GameState.Get().GetFriendlyPlayerId());

  public bool HasSubOption() => this.m_subOptionState != null;

  public Card GetSubOptionParentCard() => this.m_subOptionState != null ? this.m_subOptionState.m_parentCard : (Card) null;

  public void ClearSubOptions() => this.m_subOptionState = (ChoiceCardMgr.SubOptionState) null;

  public void ShowSubOptions(Card parentCard)
  {
    this.m_subOptionState = new ChoiceCardMgr.SubOptionState();
    this.m_subOptionState.m_parentCard = parentCard;
    this.StartCoroutine(this.WaitThenShowSubOptions());
  }

  public void QuenePendingCancelSubOptions() => this.m_pendingCancelSubOptionState = this.m_subOptionState;

  public bool HasPendingCancelSubOptions() => this.m_pendingCancelSubOptionState != null && this.m_pendingCancelSubOptionState == this.m_subOptionState;

  public void ClearPendingCancelSubOptions() => this.m_pendingCancelSubOptionState = (ChoiceCardMgr.SubOptionState) null;

  public bool IsWaitingToShowSubOptions()
  {
    if (!this.HasSubOption())
      return false;
    Entity entity = this.m_subOptionState.m_parentCard.GetEntity();
    Player controller = entity.GetController();
    Zone zone = this.m_subOptionState.m_parentCard.GetZone();
    if (entity.IsMinion())
    {
      if (zone.m_ServerTag == TAG_ZONE.SETASIDE)
        return false;
      ZonePlay battlefieldZone = controller.GetBattlefieldZone();
      if ((UnityEngine.Object) zone != (UnityEngine.Object) battlefieldZone || this.m_subOptionState.m_parentCard.GetZonePosition() == 0)
        return true;
    }
    if (entity.IsHero())
    {
      ZoneHero heroZone = controller.GetHeroZone();
      if ((UnityEngine.Object) zone != (UnityEngine.Object) heroZone || !this.m_subOptionState.m_parentCard.IsActorReady())
        return true;
    }
    return !entity.HasSubCards();
  }

  public void CancelSubOptions()
  {
    if (!this.HasSubOption())
      return;
    Entity entity = this.m_subOptionState.m_parentCard.GetEntity();
    Card card = entity.GetCard();
    for (int suboption = 0; suboption < this.m_subOptionState.m_cards.Count; ++suboption)
    {
      Spell subOptionSpell = card.GetSubOptionSpell(suboption, 0, false);
      if ((bool) (UnityEngine.Object) subOptionSpell)
      {
        switch (subOptionSpell.GetActiveState())
        {
          case SpellStateType.NONE:
          case SpellStateType.CANCEL:
            continue;
          default:
            subOptionSpell.ActivateState(SpellStateType.CANCEL);
            continue;
        }
      }
    }
    card.ActivateHandStateSpells();
    if (entity.IsHeroPower() || entity.IsGameModeButton())
      entity.SetTagAndHandleChange<int>(GAME_TAG.EXHAUSTED, 0);
    this.HideSubOptions();
  }

  public void OnSubOptionClicked(Entity chosenEntity)
  {
    if (!this.HasSubOption())
      return;
    this.HideSubOptions(chosenEntity);
  }

  public bool HasChoices() => this.m_choiceStateMap.Count > 0;

  public bool HasChoices(int playerId) => this.m_choiceStateMap.ContainsKey(playerId);

  public ChoiceCardMgr.ChoiceState GetChoiceStateForPlayer(int playerId) => !this.HasChoices(playerId) ? (ChoiceCardMgr.ChoiceState) null : this.m_choiceStateMap[playerId];

  public bool HasFriendlyChoices() => this.HasChoices(GameState.Get().GetFriendlyPlayerId());

  public PowerTaskList GetPreChoiceTaskList(int playerId)
  {
    ChoiceCardMgr.ChoiceState choiceState;
    return this.m_choiceStateMap.TryGetValue(playerId, out choiceState) ? choiceState.m_preTaskList : (PowerTaskList) null;
  }

  public PowerTaskList GetFriendlyPreChoiceTaskList() => this.GetPreChoiceTaskList(GameState.Get().GetFriendlyPlayerId());

  public bool IsWaitingToStartChoices(int playerId)
  {
    ChoiceCardMgr.ChoiceState choiceState;
    return this.m_choiceStateMap.TryGetValue(playerId, out choiceState) && choiceState.m_waitingToStart;
  }

  public bool IsFriendlyWaitingToStartChoices() => this.IsWaitingToStartChoices(GameState.Get().GetFriendlyPlayerId());

  public void OnSendChoices(Network.EntityChoices choicePacket, List<Entity> chosenEntities)
  {
    if (choicePacket.ChoiceType != CHOICE_TYPE.GENERAL)
      return;
    int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
    ChoiceCardMgr.ChoiceState choiceState;
    if (!this.m_choiceStateMap.TryGetValue(friendlyPlayerId, out choiceState))
    {
      Error.AddDevFatal("ChoiceCardMgr.OnSendChoices() - there is no ChoiceState for friendly player {0}", (object) friendlyPlayerId);
    }
    else
    {
      choiceState.m_chosenEntities = new List<Entity>((IEnumerable<Entity>) chosenEntities);
      this.ConcealChoicesFromInput(friendlyPlayerId, choiceState);
    }
  }

  public void OnChosenEntityAdded(Entity entity)
  {
    if (entity == null)
    {
      Log.Gameplay.PrintError("ChoiceCardMgr.OnChosenEntityAdded(): null entity passed!");
    }
    else
    {
      Network.EntityChoices friendlyEntityChoices = GameState.Get().GetFriendlyEntityChoices();
      if (friendlyEntityChoices == null || friendlyEntityChoices.IsSingleChoice() || !this.m_choiceStateMap.ContainsKey(GameState.Get().GetFriendlyPlayerId()))
        return;
      ChoiceCardMgr.ChoiceState choiceState = this.m_choiceStateMap[GameState.Get().GetFriendlyPlayerId()];
      if (choiceState.m_xObjs == null)
      {
        Log.Gameplay.PrintError("ChoiceCardMgr.OnChosenEntityAdded(): ChoiceState does not have an m_xObjs map!");
      }
      else
      {
        if (choiceState.m_xObjs.ContainsKey(entity.GetEntityId()))
          return;
        Card card = entity.GetCard();
        if ((UnityEngine.Object) card == (UnityEngine.Object) null)
        {
          Log.Gameplay.PrintError("ChoiceCardMgr.OnChosenEntityAdded(): Entity does not have a card!");
        }
        else
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_ChoiceData.m_xPrefab);
          TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, card.transform);
          gameObject.transform.localRotation = Quaternion.identity;
          gameObject.transform.localPosition = Vector3.zero;
          choiceState.m_xObjs.Add(entity.GetEntityId(), gameObject);
        }
      }
    }
  }

  public void OnChosenEntityRemoved(Entity entity)
  {
    if (entity == null)
    {
      Log.Gameplay.PrintError("ChoiceCardMgr.OnChosenEntityRemoved(): null entity passed!");
    }
    else
    {
      Network.EntityChoices friendlyEntityChoices = GameState.Get().GetFriendlyEntityChoices();
      if (friendlyEntityChoices == null || friendlyEntityChoices.IsSingleChoice() || !this.m_choiceStateMap.ContainsKey(GameState.Get().GetFriendlyPlayerId()))
        return;
      ChoiceCardMgr.ChoiceState choiceState = this.m_choiceStateMap[GameState.Get().GetFriendlyPlayerId()];
      if (choiceState.m_xObjs == null)
      {
        Log.Gameplay.PrintError("ChoiceCardMgr.OnChosenEntityRemoved(): ChoiceState does not have an m_xObjs map!");
      }
      else
      {
        int entityId = entity.GetEntityId();
        if (!choiceState.m_xObjs.ContainsKey(entityId))
          return;
        GameObject xObj = choiceState.m_xObjs[entityId];
        choiceState.m_xObjs.Remove(entityId);
        UnityEngine.Object.Destroy((UnityEngine.Object) xObj);
      }
    }
  }

  private void OnEntityChoicesReceived(
    Network.EntityChoices choices,
    PowerTaskList preChoiceTaskList,
    object userData)
  {
    if (choices.ChoiceType != CHOICE_TYPE.GENERAL)
      return;
    this.StartCoroutine(this.WaitThenStartChoices(choices, preChoiceTaskList));
  }

  private bool OnEntitiesChosenReceived(Network.EntitiesChosen chosen, object userData)
  {
    if (chosen.ChoiceType != CHOICE_TYPE.GENERAL)
      return false;
    this.StartCoroutine(this.WaitThenConcealChoicesFromPacket(chosen));
    return true;
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData)
  {
    this.StopAllCoroutines();
    this.CancelSubOptions();
    this.CancelChoices();
  }

  private IEnumerator WaitThenStartChoices(
    Network.EntityChoices choices,
    PowerTaskList preChoiceTaskList)
  {
    ChoiceCardMgr choiceCardMgr = this;
    int playerId = choices.PlayerId;
    ChoiceCardMgr.ChoiceState state = new ChoiceCardMgr.ChoiceState();
    choiceCardMgr.m_choiceStateMap.Add(playerId, state);
    state.m_waitingToStart = true;
    state.m_hasBeenConcealed = false;
    state.m_hasBeenRevealed = false;
    state.m_choiceID = choices.ID;
    state.m_hideChosen = choices.HideChosen;
    state.m_sourceEntityId = choices.Source;
    state.m_preTaskList = preChoiceTaskList;
    state.m_xObjs = new Map<int, GameObject>();
    Entity entity1 = GameState.Get().GetEntity(choices.Source);
    if (entity1 != null)
      state.m_showFromDeck = entity1.HasTag(GAME_TAG.SHOW_DISCOVER_FROM_DECK);
    PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
    if (powerProcessor.HasTaskList(state.m_preTaskList))
      Log.Power.Print("ChoiceCardMgr.WaitThenShowChoices() - id={0} WAIT for taskList {1}", (object) choices.ID, (object) preChoiceTaskList.GetId());
    while (powerProcessor.HasTaskList(state.m_preTaskList))
      yield return (object) null;
    HistoryManager historyManager = HistoryManager.Get();
    if (historyManager.HasBigCard() && historyManager.GetCurrentBigCard().GetEntity().GetEntityId() == state.m_sourceEntityId)
      historyManager.HandleClickOnBigCard(historyManager.GetCurrentBigCard());
    Log.Power.Print("ChoiceCardMgr.WaitThenShowChoices() - id={0} BEGIN", (object) choices.ID);
    List<Card> linkedChoiceCards = new List<Card>();
    Entity entity2 = GameState.Get().GetEntity(state.m_sourceEntityId);
    for (int index = 0; index < choices.Entities.Count; ++index)
    {
      int entity3 = choices.Entities[index];
      Entity entity4 = GameState.Get().GetEntity(entity3);
      Card card = entity4.GetCard();
      if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      {
        Error.AddDevFatal("ChoiceCardMgr.WaitThenShowChoices() - Entity {0} (option {1}) has no Card", (object) entity4, (object) index);
      }
      else
      {
        if (entity4.HasTag(GAME_TAG.LINKED_ENTITY))
        {
          int timeLinkedEntityId = entity4.GetRealTimeLinkedEntityId();
          Entity entity5 = GameState.Get().GetEntity(timeLinkedEntityId);
          if (entity5 != null && (UnityEngine.Object) entity5.GetCard() != (UnityEngine.Object) null)
            linkedChoiceCards.Add(entity5.GetCard());
        }
        state.m_cards.Add(card);
        choiceCardMgr.StartCoroutine(choiceCardMgr.LoadChoiceCardActors(entity2, entity4, card));
      }
    }
    int i;
    Card linkedCard;
    for (i = 0; i < linkedChoiceCards.Count; ++i)
    {
      linkedCard = linkedChoiceCards[i];
      while ((UnityEngine.Object) linkedCard != (UnityEngine.Object) null && !linkedCard.IsActorReady())
        yield return (object) null;
      linkedCard = (Card) null;
    }
    for (i = 0; i < state.m_cards.Count; ++i)
    {
      linkedCard = state.m_cards[i];
      while (!choiceCardMgr.IsChoiceCardReady(linkedCard))
        yield return (object) null;
      linkedCard = (Card) null;
    }
    bool friendly = playerId == GameState.Get().GetFriendlyPlayerId();
    if (friendly)
    {
      while (GameState.Get().IsTurnStartManagerBlockingInput())
      {
        if (GameState.Get().IsTurnStartManagerActive())
          TurnStartManager.Get().NotifyOfStartOfTurnChoice();
        yield return (object) null;
      }
    }
    state.m_isFriendly = friendly;
    state.m_waitingToStart = false;
    choiceCardMgr.PopulateTransformDatas(state);
    choiceCardMgr.StartChoices(state);
  }

  private IEnumerator LoadChoiceCardActors(Entity source, Entity entity, Card card)
  {
    while (!this.IsEntityReady(entity))
      yield return (object) null;
    card.HideCard();
    while (!this.IsCardReady(card))
      yield return (object) null;
    CHOICE_ACTOR choiceActor = CHOICE_ACTOR.CARD;
    if (source.HasTag(GAME_TAG.CHOICE_ACTOR_TYPE))
      choiceActor = (CHOICE_ACTOR) source.GetTag(GAME_TAG.CHOICE_ACTOR_TYPE);
    switch (choiceActor)
    {
      case CHOICE_ACTOR.HERO:
        this.LoadHeroChoiceCardActor(source, entity, card);
        card.ActivateHandStateSpells();
        break;
      default:
        card.ForceLoadHandActor();
        card.ActivateHandStateSpells();
        break;
    }
  }

  private void LoadHeroChoiceCardActor(Entity source, Entity entity, Card card)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "Choose_Hero.prefab:1834beb8747ef06439f3a1b86a35ff3d", AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintWarning(string.Format("ChoiceCardManager.LoadHeroChoiceActor() - FAILED to load actor \"{0}\"", (object) "Choose_Hero.prefab:1834beb8747ef06439f3a1b86a35ff3d"));
    }
    else
    {
      Actor component = gameObject.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Log.Gameplay.PrintWarning(string.Format("ChoiceCardManager.LoadHeroChoiceActor() - ERROR actor \"{0}\" has no Actor component", (object) "Choose_Hero.prefab:1834beb8747ef06439f3a1b86a35ff3d"));
      }
      else
      {
        if ((UnityEngine.Object) card.GetActor() != (UnityEngine.Object) null)
          card.GetActor().Destroy();
        card.SetActor(component);
        component.SetCard(card);
        component.SetCardDefFromCard(card);
        component.SetPremium(card.GetPremium());
        component.UpdateAllComponents();
        component.SetEntity(entity);
        component.UpdateAllComponents();
        component.SetUnlit();
        LayerUtils.SetLayer(component.gameObject, this.gameObject.layer);
        component.GetMeshRenderer().gameObject.layer = 8;
        this.ConfigureHeroChoiceActor(source, entity, component as HeroChoiceActor);
      }
    }
  }

  private void ConfigureHeroChoiceActor(Entity source, Entity entity, HeroChoiceActor actor)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    if (entity == null || source == null)
    {
      actor.SetNameTextActive(false);
    }
    else
    {
      CHOICE_NAME_DISPLAY choiceNameDisplay = CHOICE_NAME_DISPLAY.INVALID;
      if (source.HasTag(GAME_TAG.CHOICE_NAME_DISPLAY_TYPE))
        choiceNameDisplay = (CHOICE_NAME_DISPLAY) source.GetTag(GAME_TAG.CHOICE_NAME_DISPLAY_TYPE);
      switch (choiceNameDisplay)
      {
        case CHOICE_NAME_DISPLAY.PLAYER:
          int tag = entity.GetTag(GAME_TAG.PLAYER_ID);
          if (tag == 0)
            tag = entity.GetTag(GAME_TAG.PLAYER_ID_LOOKUP);
          actor.SetNameText(GameState.Get().GetGameEntity().GetBestNameForPlayer(tag));
          actor.SetNameTextActive(true);
          break;
        case CHOICE_NAME_DISPLAY.HERO:
          actor.SetNameText(entity.GetName());
          actor.SetNameTextActive(true);
          break;
        default:
          actor.SetNameTextActive(false);
          break;
      }
    }
  }

  private bool IsChoiceCardReady(Card card) => this.IsEntityReady(card.GetEntity()) && this.IsCardReady(card) && this.IsCardActorReady(card);

  private void PopulateTransformDatas(ChoiceCardMgr.ChoiceState state)
  {
    int num1 = state.m_isFriendly ? 1 : 0;
    state.m_cardTransforms.Clear();
    int count = state.m_cards.Count;
    float num2 = this.m_ChoiceData.m_HorizontalPadding;
    if (num1 != 0 && count > this.m_CommonData.m_MaxCardsBeforeAdjusting)
      num2 = this.GetPaddingForCardCount(count);
    float num3 = num1 != 0 ? this.m_CommonData.m_FriendlyCardWidth : this.m_CommonData.m_OpponentCardWidth;
    float num4 = 1f;
    if (num1 != 0 && count > this.m_CommonData.m_MaxCardsBeforeAdjusting)
    {
      num4 = this.GetScaleForCardCount(count);
      num3 *= num4;
    }
    float num5 = 0.5f * num3;
    float num6 = 0.5f * (float) ((double) num3 * (double) count + (double) num2 * (double) (count - 1));
    string name = num1 != 0 ? this.m_ChoiceData.m_FriendlyBoneName : this.m_ChoiceData.m_OpponentBoneName;
    if ((bool) UniversalInputManager.UsePhoneUI)
      name += "_phone";
    Transform bone = Board.Get().FindBone(name);
    Vector3 position = bone.position;
    Vector3 eulerAngles = bone.rotation.eulerAngles;
    Vector3 localScale = bone.localScale;
    float num7 = position.x - num6 + num5;
    for (int index = 0; index < count; ++index)
    {
      ChoiceCardMgr.TransformData transformData = new ChoiceCardMgr.TransformData();
      transformData.Position = new Vector3()
      {
        x = num7,
        y = position.y,
        z = position.z
      };
      Vector3 vector3 = localScale;
      vector3.x *= num4;
      vector3.y *= num4;
      vector3.z *= num4;
      transformData.LocalScale = vector3;
      transformData.RotationAngles = eulerAngles;
      state.m_cardTransforms.Add(transformData);
      num7 += num3 + num2;
    }
  }

  private float GetScaleForCardCount(int cardCount)
  {
    if (cardCount <= this.m_CommonData.m_MaxCardsBeforeAdjusting)
      return 1f;
    if (cardCount == 4)
      return (float) this.m_CommonData.m_FourCardScale;
    return cardCount == 5 ? (float) this.m_CommonData.m_FiveCardScale : (float) this.m_CommonData.m_SixPlusCardScale;
  }

  private float GetPaddingForCardCount(int cardCount)
  {
    if (cardCount <= this.m_CommonData.m_MaxCardsBeforeAdjusting)
      return this.m_ChoiceData.m_HorizontalPadding;
    if (cardCount == 4)
      return (float) this.m_ChoiceData.m_HorizontalPaddingFourCards;
    return cardCount == 5 ? (float) this.m_ChoiceData.m_HorizontalPaddingFiveCards : (float) this.m_ChoiceData.m_HorizontalPaddingSixPlusCards;
  }

  private void StartChoices(ChoiceCardMgr.ChoiceState state)
  {
    this.m_lastShownChoiceState = state;
    int count = state.m_cards.Count;
    for (int index = 0; index < count; ++index)
    {
      Card card = state.m_cards[index];
      ChoiceCardMgr.TransformData cardTransform = state.m_cardTransforms[index];
      card.transform.position = cardTransform.Position;
      card.transform.rotation = Quaternion.Euler(cardTransform.RotationAngles);
      card.transform.localScale = cardTransform.LocalScale;
    }
    this.RevealChoiceCards(state);
  }

  private void RevealChoiceCards(ChoiceCardMgr.ChoiceState state)
  {
    Spell choiceRevealSpell = this.GetCustomChoiceRevealSpell(state);
    if ((UnityEngine.Object) choiceRevealSpell != (UnityEngine.Object) null)
      this.RevealChoiceCardsUsingCustomSpell(choiceRevealSpell, state);
    else
      this.DefaultRevealChoiceCards(state);
  }

  private void DefaultRevealChoiceCards(ChoiceCardMgr.ChoiceState choiceState)
  {
    bool isFriendly = choiceState.m_isFriendly;
    if (isFriendly)
      this.ShowChoiceUi(choiceState);
    this.ShowChoiceCards(choiceState, isFriendly);
    choiceState.m_hasBeenRevealed = true;
  }

  private void ShowChoiceCards(ChoiceCardMgr.ChoiceState state, bool friendly) => this.StartCoroutine(this.PlayCardAnimation(state, friendly));

  private void GetDeckTransform(
    ZoneDeck deckZone,
    out Vector3 startPos,
    out Vector3 startRot,
    out Vector3 startScale)
  {
    Actor thicknessForLayout = deckZone.GetThicknessForLayout();
    startPos = thicknessForLayout.GetMeshRenderer().bounds.center + Card.IN_DECK_OFFSET;
    startRot = Card.IN_DECK_ANGLES;
    startScale = Card.IN_DECK_SCALE;
  }

  private IEnumerator PlayCardAnimation(ChoiceCardMgr.ChoiceState state, bool friendly)
  {
    if (state.m_showFromDeck)
    {
      state.m_showFromDeck = false;
      Vector3 deckPos;
      Vector3 deckRot;
      Vector3 deckScale;
      this.GetDeckTransform(GameState.Get().GetEntity(state.m_sourceEntityId).GetController().GetDeckZone(), out deckPos, out deckRot, out deckScale);
      float timingBonus = 0.1f;
      int cardCount = state.m_cards.Count;
      for (int i = 0; i < cardCount; ++i)
      {
        Card card = state.m_cards[i];
        card.ShowCard();
        GameObject cardObject = card.gameObject;
        cardObject.transform.position = deckPos;
        cardObject.transform.rotation = Quaternion.Euler(deckRot);
        cardObject.transform.localScale = deckScale;
        ChoiceCardMgr.TransformData cardTransform = state.m_cardTransforms[i];
        iTween.Stop(cardObject);
        Vector3[] vector3Array = new Vector3[3]
        {
          cardObject.transform.position,
          new Vector3(cardObject.transform.position.x, cardObject.transform.position.y + 3.6f, cardObject.transform.position.z),
          cardTransform.Position
        };
        iTween.MoveTo(cardObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
        iTween.ScaleTo(cardObject, (Vector3) MulliganManager.FRIENDLY_PLAYER_CARD_SCALE, MulliganManager.ANIMATION_TIME_DEAL_CARD);
        iTween.RotateTo(cardObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) MulliganManager.ANIMATION_TIME_DEAL_CARD, (object) "delay", (object) (float) ((double) MulliganManager.ANIMATION_TIME_DEAL_CARD / 16.0)));
        yield return (object) new WaitForSeconds(0.04f);
        SoundManager.Get().LoadAndPlay((AssetReference) "FX_GameStart09_CardsOntoTable.prefab:da502e035813b5742a04d2ef4f588255", cardObject);
        yield return (object) new WaitForSeconds(0.05f + timingBonus);
        timingBonus = 0.0f;
        cardObject = (GameObject) null;
      }
      deckPos = new Vector3();
      deckRot = new Vector3();
      deckScale = new Vector3();
    }
    else
    {
      int count = state.m_cards.Count;
      for (int index = 0; index < count; ++index)
      {
        Card card = state.m_cards[index];
        ChoiceCardMgr.TransformData cardTransform = state.m_cardTransforms[index];
        card.ShowCard();
        card.transform.localScale = ChoiceCardMgr.INVISIBLE_SCALE;
        iTween.Stop(card.gameObject);
        iTween.RotateTo(card.gameObject, cardTransform.RotationAngles, this.m_ChoiceData.m_CardShowTime);
        iTween.ScaleTo(card.gameObject, cardTransform.LocalScale, this.m_ChoiceData.m_CardShowTime);
        iTween.MoveTo(card.gameObject, cardTransform.Position, this.m_ChoiceData.m_CardShowTime);
        this.ActivateChoiceCardStateSpells(card);
      }
    }
    this.PlayChoiceEffects(state, friendly);
  }

  private void PlayChoiceEffects(ChoiceCardMgr.ChoiceState state, bool friendly)
  {
    if (!friendly)
      return;
    Entity entity = GameState.Get().GetEntity(state.m_sourceEntityId);
    if (entity == null)
      return;
    ChoiceCardMgr.ChoiceEffectData effectDataForCard = this.GetChoiceEffectDataForCard(entity.GetCard());
    if (effectDataForCard == null || (UnityEngine.Object) effectDataForCard.m_Spell == (UnityEngine.Object) null || state.m_hasBeenRevealed && !effectDataForCard.m_AlwaysPlayEffect)
      return;
    Spell.StateFinishedCallback callback = (Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
    {
      if (spell.GetActiveState() != SpellStateType.NONE)
        return;
      SpellManager.Get().ReleaseSpell(spell);
    });
    if (effectDataForCard.m_PlayOncePerCard)
    {
      foreach (Card card in state.m_cards)
      {
        Spell spell = SpellManager.Get().GetSpell(effectDataForCard.m_Spell);
        TransformUtil.AttachAndPreserveLocalTransform(spell.transform, card.GetActor().transform);
        spell.AddStateFinishedCallback(callback);
        spell.Activate();
        state.m_choiceEffectSpells.Add(spell);
      }
    }
    else
    {
      Spell spell = SpellManager.Get().GetSpell(effectDataForCard.m_Spell);
      spell.AddStateFinishedCallback(callback);
      spell.Activate();
      state.m_choiceEffectSpells.Add(spell);
    }
  }

  private void ActivateChoiceCardStateSpells(Card card)
  {
    Actor actor = card.GetActor();
    if (!((UnityEngine.Object) actor != (UnityEngine.Object) null))
      return;
    if (actor.UseCoinManaGemForChoiceCard())
      actor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
    else
      actor.ReleaseSpell(SpellType.COIN_MANA_GEM);
    if (actor.UseTechLevelManaGem())
    {
      Spell spell = actor.GetSpell(SpellType.TECH_LEVEL_MANA_GEM);
      Entity entity = card.GetEntity();
      if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || entity == null)
        return;
      spell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("TechLevel").Value = entity.GetTechLevel();
      spell.ActivateState(SpellStateType.BIRTH);
    }
    else
      actor.ReleaseSpell(SpellType.TECH_LEVEL_MANA_GEM);
  }

  private void DeactivateChoiceCardStateSpells(Card card)
  {
    Actor actor = card.GetActor();
    if (!((UnityEngine.Object) actor != (UnityEngine.Object) null))
      return;
    if (actor.UseCoinManaGemForChoiceCard())
      actor.ReleaseSpell(SpellType.COIN_MANA_GEM);
    if (!actor.UseTechLevelManaGem())
      return;
    actor.ReleaseSpell(SpellType.TECH_LEVEL_MANA_GEM);
  }

  private void DeactivateChoiceEffects(ChoiceCardMgr.ChoiceState state)
  {
    foreach (Spell choiceEffectSpell in state.m_choiceEffectSpells)
    {
      if (!((UnityEngine.Object) choiceEffectSpell == (UnityEngine.Object) null) && choiceEffectSpell.HasUsableState(SpellStateType.DEATH))
        choiceEffectSpell.ActivateState(SpellStateType.DEATH);
    }
    state.m_choiceEffectSpells.Clear();
  }

  private ChoiceCardMgr.TagPostChoiceEffect GetTagPostChoiceEffect(
    ChoiceCardMgr.ChoiceState choiceState)
  {
    Entity entity = GameState.Get().GetEntity(choiceState.m_sourceEntityId);
    foreach (ChoiceCardMgr.TagPostChoiceEffect postChoiceEffect in this.m_TagPostChoiceEffectData)
    {
      if (entity.HasTag(postChoiceEffect.m_Tag))
        return postChoiceEffect;
    }
    return (ChoiceCardMgr.TagPostChoiceEffect) null;
  }

  private void ApplyPostChoiceEffects(
    ChoiceCardMgr.TagPostChoiceEffect postChoiceEffect,
    ChoiceCardMgr.ChoiceState choiceState,
    Network.EntitiesChosen chosen)
  {
    Spell.StateFinishedCallback callback = (Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
    {
      if (spell.GetActiveState() != SpellStateType.NONE)
        return;
      SpellManager.Get().ReleaseSpell(spell);
    });
    if (postChoiceEffect == null)
      return;
    List<Card> cards = choiceState.m_cards;
    for (int index = 0; index < cards.Count; ++index)
    {
      Card card = cards[index];
      Spell spell1 = this.WasCardChosen(card, chosen.Entities) ? postChoiceEffect.m_SpellSelectedCards : postChoiceEffect.m_SpellUnselectedCards;
      Spell spell2 = SpellManager.Get().GetSpell(spell1);
      TransformUtil.AttachAndPreserveLocalTransform(spell2.transform, card.GetActor().transform);
      spell2.AddStateFinishedCallback(callback);
      spell2.ActivateState(SpellStateType.DEATH);
      choiceState.m_postChoiceSpells.Add(spell2);
    }
  }

  private bool HavePostChoiceEffectsFinished(ChoiceCardMgr.ChoiceState choiceState)
  {
    foreach (Spell postChoiceSpell in choiceState.m_postChoiceSpells)
    {
      if ((UnityEngine.Object) postChoiceSpell != (UnityEngine.Object) null && !postChoiceSpell.IsFinished())
        return false;
    }
    return true;
  }

  private ChoiceCardMgr.ChoiceEffectData GetChoiceEffectDataForCard(Card sourceCard)
  {
    if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
      return (ChoiceCardMgr.ChoiceEffectData) null;
    foreach (ChoiceCardMgr.CardSpecificChoiceEffect specificChoiceEffect in this.m_CardSpecificChoiceEffectData)
    {
      if (specificChoiceEffect.m_CardID == sourceCard.GetEntity().GetCardId())
        return specificChoiceEffect.m_ChoiceEffectData;
    }
    foreach (ChoiceCardMgr.TagSpecificChoiceEffect specificChoiceEffect in this.m_TagSpecificChoiceEffectData)
    {
      if (sourceCard.GetEntity().HasTag(specificChoiceEffect.m_Tag))
      {
        foreach (ChoiceCardMgr.TagValueSpecificChoiceEffect valueSpell in specificChoiceEffect.m_ValueSpellMap)
        {
          if (valueSpell.m_Value == sourceCard.GetEntity().GetTag(specificChoiceEffect.m_Tag))
            return valueSpell.m_ChoiceEffectData;
        }
      }
    }
    if (sourceCard.GetEntity().HasTag(GAME_TAG.USE_DISCOVER_VISUALS))
      return this.m_DiscoverChoiceEffectData;
    if (sourceCard.GetEntity().HasReferencedTag(GAME_TAG.ADAPT))
      return this.m_AdaptChoiceEffectData;
    if (sourceCard.GetEntity().HasTag(GAME_TAG.GEARS))
      return this.m_GearsChoiceEffectData;
    return sourceCard.GetEntity().HasTag(GAME_TAG.GOOD_OL_GENERIC_FRIENDLY_DRAGON_DISCOVER_VISUALS) ? this.m_DragonChoiceEffectData : (ChoiceCardMgr.ChoiceEffectData) null;
  }

  private IEnumerator WaitThenConcealChoicesFromPacket(Network.EntitiesChosen chosen)
  {
    int playerId = chosen.PlayerId;
    ChoiceCardMgr.ChoiceState choiceState;
    if (this.m_choiceStateMap.TryGetValue(playerId, out choiceState))
    {
      if (choiceState.m_waitingToStart || !choiceState.m_hasBeenRevealed)
      {
        Log.Power.Print("ChoiceCardMgr.WaitThenHideChoicesFromPacket() - id={0} BEGIN WAIT for EntityChoice", (object) chosen.ID);
        while (choiceState.m_waitingToStart)
          yield return (object) null;
        while (!choiceState.m_hasBeenRevealed)
          yield return (object) null;
        yield return (object) new WaitForSeconds(this.m_ChoiceData.m_MinShowTime);
      }
    }
    else if (this.m_lastShownChoiceState.m_choiceID == chosen.ID)
      choiceState = this.m_lastShownChoiceState;
    if (choiceState == null)
    {
      Log.Power.Print("ChoiceCardMgr.WaitThenHideChoicesFromPacket(): Unable to find ChoiceState corresponding to EntitiesChosen packet with ID %d.", (object) chosen.ID);
      Log.Power.Print("ChoiceCardMgr.WaitThenHideChoicesFromPacket() - id={0} END WAIT", (object) chosen.ID);
      GameState.Get().OnEntitiesChosenProcessed(chosen);
    }
    else
    {
      this.ResolveConflictBetweenLocalChoiceAndServerPacket(choiceState, chosen);
      if (choiceState.m_isFriendly)
      {
        this.ApplyPostChoiceEffects(this.GetTagPostChoiceEffect(choiceState), choiceState, chosen);
        while (!this.HavePostChoiceEffectsFinished(choiceState))
          yield return (object) null;
      }
      Log.Power.Print("ChoiceCardMgr.WaitThenHideChoicesFromPacket() - id={0} END WAIT", (object) chosen.ID);
      this.ConcealChoicesFromPacket(playerId, choiceState, chosen);
    }
  }

  private void ResolveConflictBetweenLocalChoiceAndServerPacket(
    ChoiceCardMgr.ChoiceState choiceState,
    Network.EntitiesChosen chosen)
  {
    if (this.DoesLocalChoiceMatchPacket(choiceState.m_chosenEntities, chosen.Entities))
      return;
    choiceState.m_chosenEntities = new List<Entity>();
    foreach (int entity1 in chosen.Entities)
    {
      Entity entity2 = GameState.Get().GetEntity(entity1);
      if (entity2 != null)
        choiceState.m_chosenEntities.Add(entity2);
    }
    if (!choiceState.m_hasBeenConcealed)
      return;
    foreach (Card card in choiceState.m_cards)
      card.ShowCard();
    choiceState.m_hasBeenConcealed = false;
  }

  private bool DoesLocalChoiceMatchPacket(List<Entity> localChoices, List<int> packetChoices)
  {
    if (localChoices == null || packetChoices == null)
    {
      Log.Power.Print(string.Format("ChoiceCardMgr.DoesLocalChoiceMatchPacket(): Null list passed in! localChoices={0}, packetChoices={1}.", (object) localChoices, (object) packetChoices));
      return false;
    }
    if (localChoices.Count != packetChoices.Count)
      return false;
    for (int index = 0; index < packetChoices.Count; ++index)
    {
      int packetChoice = packetChoices[index];
      Entity entity = GameState.Get().GetEntity(packetChoice);
      if (!localChoices.Contains(entity))
        return false;
    }
    return true;
  }

  private void ConcealChoicesFromPacket(
    int playerId,
    ChoiceCardMgr.ChoiceState choiceState,
    Network.EntitiesChosen chosen)
  {
    if (choiceState.m_isFriendly)
      this.HideChoiceUI();
    Spell choiceConcealSpell = this.GetCustomChoiceConcealSpell(choiceState);
    if ((UnityEngine.Object) choiceConcealSpell != (UnityEngine.Object) null)
      this.ConcealChoiceCardsUsingCustomSpell(choiceConcealSpell, choiceState, chosen);
    else
      this.DefaultConcealChoicesFromPacket(playerId, choiceState, chosen);
  }

  private void DefaultConcealChoicesFromPacket(
    int playerId,
    ChoiceCardMgr.ChoiceState choiceState,
    Network.EntitiesChosen chosen)
  {
    if (!choiceState.m_hasBeenConcealed)
    {
      List<Card> cards = choiceState.m_cards;
      bool hideChosen = choiceState.m_hideChosen;
      for (int index = 0; index < cards.Count; ++index)
      {
        Card card = cards[index];
        if (hideChosen || !this.WasCardChosen(card, chosen.Entities))
        {
          card.DeactivateHandStateSpells(card.GetActor());
          this.DeactivateChoiceCardStateSpells(card);
          card.HideCard();
        }
      }
      this.DeactivateChoiceEffects(choiceState);
      choiceState.m_hasBeenConcealed = true;
    }
    this.OnFinishedConcealChoices(playerId);
    GameState.Get().OnEntitiesChosenProcessed(chosen);
  }

  private bool WasCardChosen(Card card, List<int> chosenEntityIds)
  {
    int entityId = card.GetEntity().GetEntityId();
    return chosenEntityIds.FindIndex((Predicate<int>) (currEntityId => entityId == currEntityId)) >= 0;
  }

  private void ConcealChoicesFromInput(int playerId, ChoiceCardMgr.ChoiceState choiceState)
  {
    if (choiceState.m_isFriendly)
      this.HideChoiceUI();
    Spell choiceConcealSpell = this.GetCustomChoiceConcealSpell(choiceState);
    ChoiceCardMgr.TagPostChoiceEffect postChoiceEffect = this.GetTagPostChoiceEffect(choiceState);
    if (!((UnityEngine.Object) choiceConcealSpell == (UnityEngine.Object) null) || postChoiceEffect != null)
      return;
    for (int index = 0; index < choiceState.m_cards.Count; ++index)
    {
      Card card = choiceState.m_cards[index];
      Entity entity = card.GetEntity();
      if (choiceState.m_hideChosen || !choiceState.m_chosenEntities.Contains(entity))
      {
        card.HideCard();
        card.DeactivateHandStateSpells(card.GetActor());
        this.DeactivateChoiceCardStateSpells(card);
      }
    }
    this.DeactivateChoiceEffects(choiceState);
    choiceState.m_hasBeenConcealed = true;
    this.OnFinishedConcealChoices(playerId);
  }

  private void OnFinishedConcealChoices(int playerId)
  {
    if (!this.m_choiceStateMap.ContainsKey(playerId))
      return;
    foreach (UnityEngine.Object @object in this.m_choiceStateMap[playerId].m_xObjs.Values)
      UnityEngine.Object.Destroy(@object);
    this.m_choiceStateMap.Remove(playerId);
  }

  private void HideChoiceCards(ChoiceCardMgr.ChoiceState state)
  {
    for (int index = 0; index < state.m_cards.Count; ++index)
      this.HideChoiceCard(state.m_cards[index]);
    this.DeactivateChoiceEffects(state);
  }

  private void HideChoiceCard(Card card)
  {
    Action<object> action = (Action<object>) (userData => ((Card) userData).HideCard());
    iTween.Stop(card.gameObject);
    Hashtable args = iTween.Hash((object) "scale", (object) ChoiceCardMgr.INVISIBLE_SCALE, (object) "time", (object) this.m_ChoiceData.m_CardHideTime, (object) "oncomplete", (object) action, (object) "oncompleteparams", (object) card, (object) "oncompletetarget", (object) this.gameObject);
    iTween.ScaleTo(card.gameObject, args);
  }

  private void ShowChoiceUi(ChoiceCardMgr.ChoiceState choiceState)
  {
    this.ShowChoiceBanner(choiceState.m_cards);
    this.ShowChoiceButtons();
    this.HideEnlargedHand();
  }

  private void HideChoiceUI()
  {
    this.HideChoiceBanner();
    this.HideChoiceButtons();
    this.RestoreEnlargedHand();
  }

  private void ShowChoiceBanner(List<Card> cards)
  {
    this.HideChoiceBanner();
    Network.EntityChoices friendlyEntityChoices = GameState.Get().GetFriendlyEntityChoices();
    Transform bone = Board.Get().FindBone(this.m_ChoiceData.m_BannerBoneName);
    this.m_choiceBanner = UnityEngine.Object.Instantiate<Banner>(this.m_ChoiceData.m_BannerPrefab, bone.position, bone.rotation);
    this.m_choiceBanner.SetupBanner(friendlyEntityChoices, cards);
    Vector3 localScale = this.m_choiceBanner.transform.localScale;
    this.m_choiceBanner.transform.localScale = ChoiceCardMgr.INVISIBLE_SCALE;
    iTween.ScaleTo(this.m_choiceBanner.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) this.m_ChoiceData.m_UiShowTime));
  }

  private void HideChoiceBanner()
  {
    if (!(bool) (UnityEngine.Object) this.m_choiceBanner)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_choiceBanner.gameObject);
  }

  private void ShowChoiceButtons()
  {
    Network.EntityChoices friendlyEntityChoices = GameState.Get().GetFriendlyEntityChoices();
    if (friendlyEntityChoices == null)
      return;
    this.HideChoiceButtons();
    string choiceButtonBoneName1 = this.m_ChoiceData.m_ToggleChoiceButtonBoneName;
    if ((bool) UniversalInputManager.UsePhoneUI)
      choiceButtonBoneName1 += "_phone";
    this.m_toggleChoiceButton = this.CreateChoiceButton(choiceButtonBoneName1, new UIEvent.Handler(this.ChoiceButton_OnPress), new UIEvent.Handler(this.ToggleChoiceButton_OnRelease), GameStrings.Get("GLOBAL_HIDE"));
    if (friendlyEntityChoices.IsSingleChoice())
      return;
    string choiceButtonBoneName2 = this.m_ChoiceData.m_ConfirmChoiceButtonBoneName;
    if ((bool) UniversalInputManager.UsePhoneUI)
      choiceButtonBoneName2 += "_phone";
    this.m_confirmChoiceButton = this.CreateChoiceButton(choiceButtonBoneName2, new UIEvent.Handler(this.ChoiceButton_OnPress), new UIEvent.Handler(this.ConfirmChoiceButton_OnRelease), GameStrings.Get("GLOBAL_CONFIRM"));
  }

  private NormalButton CreateChoiceButton(
    string boneName,
    UIEvent.Handler OnPressHandler,
    UIEvent.Handler OnReleaseHandler,
    string buttonText)
  {
    NormalButton component = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_ChoiceData.m_ButtonPrefab, AssetLoadingOptions.IgnorePrefabPosition).GetComponent<NormalButton>();
    component.GetButtonUberText().TextAlpha = 1f;
    TransformUtil.CopyWorld((Component) component, (Component) Board.Get().FindBone(boneName));
    this.m_friendlyChoicesShown = true;
    component.AddEventListener(UIEventType.PRESS, OnPressHandler);
    component.AddEventListener(UIEventType.RELEASE, OnReleaseHandler);
    component.SetText(buttonText);
    component.m_button.GetComponent<Spell>().ActivateState(SpellStateType.BIRTH);
    return component;
  }

  private void HideChoiceButtons()
  {
    if ((UnityEngine.Object) this.m_toggleChoiceButton != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_toggleChoiceButton.gameObject);
      this.m_toggleChoiceButton = (NormalButton) null;
    }
    if (!((UnityEngine.Object) this.m_confirmChoiceButton != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_confirmChoiceButton.gameObject);
    this.m_confirmChoiceButton = (NormalButton) null;
  }

  private void HideEnlargedHand()
  {
    ZoneHand handZone = GameState.Get().GetFriendlySidePlayer().GetHandZone();
    if (!handZone.HandEnlarged())
      return;
    this.m_restoreEnlargedHand = true;
    handZone.SetHandEnlarged(false);
  }

  private void RestoreEnlargedHand()
  {
    if (!this.m_restoreEnlargedHand)
      return;
    this.m_restoreEnlargedHand = false;
    if (GameState.Get().IsInTargetMode())
      return;
    ZoneHand handZone = GameState.Get().GetFriendlySidePlayer().GetHandZone();
    if (handZone.HandEnlarged())
      return;
    handZone.SetHandEnlarged(true);
  }

  private void ChoiceButton_OnPress(UIEvent e) => SoundManager.Get().LoadAndPlay((AssetReference) "UI_MouseClick_01.prefab:fa537702a0db1c3478c989967458788b");

  private void ToggleChoiceButton_OnRelease(UIEvent e)
  {
    ChoiceCardMgr.ChoiceState choiceState = this.m_choiceStateMap[GameState.Get().GetFriendlyPlayerId()];
    if (this.m_friendlyChoicesShown)
    {
      this.m_toggleChoiceButton.SetText(GameStrings.Get("GLOBAL_SHOW"));
      this.HideChoiceCards(choiceState);
      this.m_friendlyChoicesShown = false;
    }
    else
    {
      this.m_toggleChoiceButton.SetText(GameStrings.Get("GLOBAL_HIDE"));
      this.ShowChoiceCards(choiceState, true);
      this.m_friendlyChoicesShown = true;
    }
    this.ToggleChoiceBannerVisibility(this.m_friendlyChoicesShown);
  }

  private void ToggleChoiceBannerVisibility(bool visible) => this.m_choiceBanner.gameObject.SetActive(visible);

  private void ConfirmChoiceButton_OnRelease(UIEvent e) => GameState.Get().SendChoices();

  private void CancelChoices()
  {
    this.HideChoiceUI();
    foreach (ChoiceCardMgr.ChoiceState choiceState in this.m_choiceStateMap.Values)
    {
      for (int index = 0; index < choiceState.m_cards.Count; ++index)
      {
        Card card = choiceState.m_cards[index];
        card.HideCard();
        card.DeactivateHandStateSpells(card.GetActor());
        this.DeactivateChoiceCardStateSpells(card);
      }
    }
    this.m_choiceStateMap.Clear();
  }

  private IEnumerator WaitThenShowSubOptions()
  {
    while (this.IsWaitingToShowSubOptions())
    {
      yield return (object) null;
      if (this.m_subOptionState == null)
        yield break;
    }
    this.ShowSubOptions();
  }

  private void ShowSubOptions()
  {
    GameState gameState = GameState.Get();
    Card parentCard = this.m_subOptionState.m_parentCard;
    Entity entity1 = this.m_subOptionState.m_parentCard.GetEntity();
    string boneName = this.m_SubOptionData.m_BoneName;
    if ((bool) UniversalInputManager.UsePhoneUI)
      boneName += "_phone";
    Transform bone = Board.Get().FindBone(boneName);
    float friendlyCardWidth = this.m_CommonData.m_FriendlyCardWidth;
    float x1 = bone.position.x;
    ZonePlay battlefieldZone = entity1.GetController().GetBattlefieldZone();
    List<int> subCardIds = entity1.GetSubCardIDs();
    float num1;
    float num2;
    if (entity1.IsMinion() && !(bool) UniversalInputManager.UsePhoneUI && subCardIds.Count <= 2)
    {
      int zonePosition = parentCard.GetZonePosition();
      float x2 = battlefieldZone.GetCardPosition(parentCard).x;
      if (zonePosition > 5)
      {
        num1 = friendlyCardWidth + this.m_SubOptionData.m_AdjacentCardXOffset;
        num2 = x2 - (this.m_CommonData.m_FriendlyCardWidth * 1.5f + this.m_SubOptionData.m_AdjacentCardXOffset + this.m_SubOptionData.m_MinionParentXOffset);
      }
      else if (zonePosition == 1 && battlefieldZone.GetCards().Count > 6)
      {
        num1 = friendlyCardWidth + this.m_SubOptionData.m_AdjacentCardXOffset;
        num2 = x2 + (this.m_CommonData.m_FriendlyCardWidth / 2f + this.m_SubOptionData.m_MinionParentXOffset);
      }
      else
      {
        num1 = friendlyCardWidth + this.m_SubOptionData.m_MinionParentXOffset * 2f;
        num2 = x2 - (this.m_CommonData.m_FriendlyCardWidth / 2f + this.m_SubOptionData.m_MinionParentXOffset);
      }
    }
    else
    {
      int count = subCardIds.Count;
      num1 = friendlyCardWidth + (count > this.m_CommonData.m_MaxCardsBeforeAdjusting ? this.m_SubOptionData.m_PhoneMaxAdjacentCardXOffset : this.m_SubOptionData.m_AdjacentCardXOffset);
      num2 = x1 - num1 / 2f * (float) (count - 1);
    }
    for (int index = 0; index < subCardIds.Count; ++index)
    {
      int id = subCardIds[index];
      Entity entity2 = gameState.GetEntity(id);
      Card card = entity2.GetCard();
      if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
      {
        if (entity2.GetCardType() == TAG_CARDTYPE.LETTUCE_ABILITY)
        {
          foreach (Transform componentsInChild in card.gameObject.GetComponentsInChildren<Transform>())
            componentsInChild.position = new Vector3();
        }
        this.m_subOptionState.m_cards.Add(card);
        card.ForceLoadHandActor();
        card.transform.position = parentCard.transform.position;
        card.transform.localScale = ChoiceCardMgr.INVISIBLE_SCALE;
        iTween.MoveTo(card.gameObject, new Vector3()
        {
          x = num2 + (float) index * num1,
          y = bone.position.y,
          z = bone.position.z
        }, this.m_SubOptionData.m_CardShowTime);
        Vector3 localScale = bone.localScale;
        if (subCardIds.Count > this.m_CommonData.m_MaxCardsBeforeAdjusting)
        {
          float scaleForCardCount = this.GetScaleForCardCount(subCardIds.Count);
          localScale.x *= scaleForCardCount;
          localScale.y *= scaleForCardCount;
          localScale.z *= scaleForCardCount;
        }
        iTween.ScaleTo(card.gameObject, localScale, this.m_SubOptionData.m_CardShowTime);
        card.ActivateHandStateSpells();
      }
    }
    this.HideEnlargedHand();
  }

  private void HideSubOptions(Entity chosenEntity = null)
  {
    for (int index = 0; index < this.m_subOptionState.m_cards.Count; ++index)
    {
      Card card = this.m_subOptionState.m_cards[index];
      card.DeactivateHandStateSpells();
      this.DeactivateChoiceCardStateSpells(card);
      if (card.GetEntity() != chosenEntity)
        card.HideCard();
    }
    this.RestoreEnlargedHand();
  }

  private bool IsEntityReady(Entity entity) => entity.GetZone() != TAG_ZONE.INVALID && !entity.IsBusy();

  private bool IsCardReady(Card card) => card.HasCardDef;

  private bool IsCardActorReady(Card card) => card.IsActorReady();

  private Spell GetCustomChoiceRevealSpell(ChoiceCardMgr.ChoiceState choiceState)
  {
    Entity entity = GameState.Get().GetEntity(choiceState.m_sourceEntityId);
    if (entity == null)
      return (Spell) null;
    Card card = entity.GetCard();
    return (UnityEngine.Object) card == (UnityEngine.Object) null ? (Spell) null : card.GetCustomChoiceRevealSpell();
  }

  private Spell GetCustomChoiceConcealSpell(ChoiceCardMgr.ChoiceState choiceState)
  {
    Entity entity = GameState.Get().GetEntity(choiceState.m_sourceEntityId);
    if (entity == null)
      return (Spell) null;
    Card card = entity.GetCard();
    return (UnityEngine.Object) card == (UnityEngine.Object) null ? (Spell) null : card.GetCustomChoiceConcealSpell();
  }

  private void RevealChoiceCardsUsingCustomSpell(
    Spell customChoiceRevealSpell,
    ChoiceCardMgr.ChoiceState state)
  {
    CustomChoiceSpell customChoiceSpell = customChoiceRevealSpell as CustomChoiceSpell;
    if ((UnityEngine.Object) customChoiceSpell != (UnityEngine.Object) null)
      customChoiceSpell.SetChoiceState(state);
    customChoiceRevealSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnCustomChoiceRevealSpellFinished), (object) state);
    customChoiceRevealSpell.Activate();
  }

  private void OnCustomChoiceRevealSpellFinished(Spell spell, object userData)
  {
    if (!(userData is ChoiceCardMgr.ChoiceState choiceState))
      Log.Power.PrintError("userData passed to ChoiceCardMgr.OnCustomChoiceRevealSpellFinished() is not of type ChoiceState.");
    if (choiceState.m_isFriendly)
      this.ShowChoiceUi(choiceState);
    foreach (Card card in choiceState.m_cards)
    {
      card.ShowCard();
      this.ActivateChoiceCardStateSpells(card);
    }
    this.PlayChoiceEffects(choiceState, choiceState.m_isFriendly);
    choiceState.m_hasBeenRevealed = true;
  }

  private void ConcealChoiceCardsUsingCustomSpell(
    Spell customChoiceConcealSpell,
    ChoiceCardMgr.ChoiceState choiceState,
    Network.EntitiesChosen chosen)
  {
    if (customChoiceConcealSpell.IsActive())
      Log.Power.PrintError("ChoiceCardMgr.HideChoicesFromPacket(): CustomChoiceConcealSpell is already active!");
    CustomChoiceSpell customChoiceSpell = customChoiceConcealSpell as CustomChoiceSpell;
    if ((UnityEngine.Object) customChoiceSpell != (UnityEngine.Object) null)
      customChoiceSpell.SetChoiceState(choiceState);
    this.DeactivateChoiceEffects(choiceState);
    choiceState.m_hasBeenConcealed = true;
    customChoiceConcealSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnCustomChoiceConcealSpellFinished), (object) chosen);
    customChoiceConcealSpell.Activate();
  }

  private void OnCustomChoiceConcealSpellFinished(Spell spell, object userData)
  {
    Network.EntitiesChosen chosen = userData as Network.EntitiesChosen;
    this.OnFinishedConcealChoices(chosen.PlayerId);
    GameState.Get().OnEntitiesChosenProcessed(chosen);
  }

  [Serializable]
  public class CommonData
  {
    public float m_FriendlyCardWidth = 2.85f;
    public float m_OpponentCardWidth = 1.5f;
    public int m_MaxCardsBeforeAdjusting = 3;
    public PlatformDependentValue<float> m_FourCardScale = new PlatformDependentValue<float>(PlatformCategory.Screen)
    {
      PC = 1f,
      Tablet = 1f,
      Phone = 0.8f
    };
    public PlatformDependentValue<float> m_FiveCardScale = new PlatformDependentValue<float>(PlatformCategory.Screen)
    {
      PC = 0.85f,
      Tablet = 0.85f,
      Phone = 0.65f
    };
    public PlatformDependentValue<float> m_SixPlusCardScale = new PlatformDependentValue<float>(PlatformCategory.Screen)
    {
      PC = 0.7f,
      Tablet = 0.7f,
      Phone = 0.55f
    };
  }

  [Serializable]
  public class ChoiceData
  {
    public string m_FriendlyBoneName = "FriendlyChoice";
    public string m_OpponentBoneName = "OpponentChoice";
    public string m_BannerBoneName = "ChoiceBanner";
    public string m_ToggleChoiceButtonBoneName = "ToggleChoiceButton";
    public string m_ConfirmChoiceButtonBoneName = "ConfirmChoiceButton";
    public float m_MinShowTime = 1f;
    public Banner m_BannerPrefab;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string m_ButtonPrefab;
    public GameObject m_xPrefab;
    public float m_CardShowTime = 0.2f;
    public float m_CardHideTime = 0.2f;
    public float m_UiShowTime = 0.5f;
    public float m_HorizontalPadding = 0.75f;
    public PlatformDependentValue<float> m_HorizontalPaddingFourCards = new PlatformDependentValue<float>(PlatformCategory.Screen)
    {
      PC = 0.6f,
      Tablet = 0.5f,
      Phone = 0.4f
    };
    public PlatformDependentValue<float> m_HorizontalPaddingFiveCards = new PlatformDependentValue<float>(PlatformCategory.Screen)
    {
      PC = 0.3f,
      Tablet = 0.3f,
      Phone = 0.3f
    };
    public PlatformDependentValue<float> m_HorizontalPaddingSixPlusCards = new PlatformDependentValue<float>(PlatformCategory.Screen)
    {
      PC = 0.2f,
      Tablet = 0.2f,
      Phone = 0.2f
    };
  }

  [Serializable]
  public class SubOptionData
  {
    public string m_BoneName = "SubOption";
    public float m_AdjacentCardXOffset = 0.75f;
    public float m_PhoneMaxAdjacentCardXOffset = 0.1f;
    public float m_MinionParentXOffset = 0.9f;
    public float m_CardShowTime = 0.2f;
  }

  [Serializable]
  public class ChoiceEffectData
  {
    public bool m_AlwaysPlayEffect;
    public bool m_PlayOncePerCard;
    public Spell m_Spell;
  }

  [Serializable]
  public class TagSpecificChoiceEffect
  {
    public GAME_TAG m_Tag;
    public List<ChoiceCardMgr.TagValueSpecificChoiceEffect> m_ValueSpellMap;
  }

  [Serializable]
  public class TagValueSpecificChoiceEffect
  {
    public int m_Value;
    public ChoiceCardMgr.ChoiceEffectData m_ChoiceEffectData;
  }

  [Serializable]
  public class CardSpecificChoiceEffect
  {
    public string m_CardID;
    public ChoiceCardMgr.ChoiceEffectData m_ChoiceEffectData;
  }

  [Serializable]
  public class TagPostChoiceEffect
  {
    public GAME_TAG m_Tag;
    public Spell m_SpellSelectedCards;
    public Spell m_SpellUnselectedCards;
  }

  private class SubOptionState
  {
    public List<Card> m_cards = new List<Card>();
    public Card m_parentCard;
  }

  public struct TransformData
  {
    public Vector3 Position { get; set; }

    public Vector3 RotationAngles { get; set; }

    public Vector3 LocalScale { get; set; }
  }

  public class ChoiceState
  {
    public int m_choiceID;
    public bool m_isFriendly;
    public List<Card> m_cards = new List<Card>();
    public List<ChoiceCardMgr.TransformData> m_cardTransforms = new List<ChoiceCardMgr.TransformData>();
    public bool m_waitingToStart;
    public bool m_hasBeenRevealed;
    public bool m_hasBeenConcealed;
    public bool m_hideChosen;
    public int m_choiceActor;
    public PowerTaskList m_preTaskList;
    public int m_sourceEntityId;
    public List<Entity> m_chosenEntities;
    public Map<int, GameObject> m_xObjs;
    public List<Spell> m_choiceEffectSpells = new List<Spell>();
    public List<Spell> m_postChoiceSpells = new List<Spell>();
    public bool m_showFromDeck;
  }
}
