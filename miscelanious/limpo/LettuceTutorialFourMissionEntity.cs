using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LettuceTutorialFourMissionEntity : LettuceMissionEntity
{
  private static readonly Map<GameEntityOption, bool> s_booleanOptions = new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      false
    }
  };
  private static readonly Map<GameEntityOption, string> s_stringOptions = new Map<GameEntityOption, string>();
  private Notification m_handBounceArrow;
  private bool m_shouldShowHandBounceArrow;
  private LettuceTutorialFourMissionEntity.TutorialStep m_currentTutorialStep;
  private static readonly AssetReference VO_NEW1_024_Attack_02 = new AssetReference("VO_NEW1_024_Attack_02.prefab:2e3944f60849f71409744641036cd71e");
  private static readonly AssetReference VO_BGS_053_Male_Orc_Play_01 = new AssetReference("VO_BGS_053_Male_Orc_Play_01.prefab:e44994899574498429f95fa821363676");
  private Notification.SpeechBubbleDirection enemyMinionSpeakingDirection = Notification.SpeechBubbleDirection.BottomLeft;
  private Notification endTurnNotifier;

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LettuceTutorialFourMissionEntity.VO_NEW1_024_Attack_02,
      (string) LettuceTutorialFourMissionEntity.VO_BGS_053_Male_Orc_Play_01
    })
      this.PreloadSound(soundPath);
  }

  public LettuceTutorialFourMissionEntity()
    : base()
  {
    this.m_gameOptions.AddOptions(LettuceTutorialFourMissionEntity.s_booleanOptions, LettuceTutorialFourMissionEntity.s_stringOptions);
    this.m_abilityOrderSpeechBubblesEnabled = true;
    this.m_enemyAbilityOrderSpeechBubblesEnabled = false;
  }

  protected override void OnLettuceMissionEntityReconnect(int currentTurn)
  {
    if (this.GetTag<ACTION_STEP_TYPE>(GAME_TAG.ACTION_STEP_TYPE) != ACTION_STEP_TYPE.LETTUCE_MERCENARY_SELECTION)
      return;
    switch (currentTurn)
    {
      case 1:
        if (GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count != 0)
          break;
        this.SetTutorialStep(LettuceTutorialFourMissionEntity.TutorialStep.BENCH_TUTORIAL);
        break;
      case 2:
        if (GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards().Count <= 0)
          break;
        this.SetTutorialStep(LettuceTutorialFourMissionEntity.TutorialStep.REPLACEMENT);
        break;
    }
  }

  public override void NotifyOfStartOfTurnEventsFinished()
  {
    int tag1 = this.GetTag(GAME_TAG.TURN);
    int tag2 = GameState.Get().GetGameEntity().GetTag(GAME_TAG.ACTION_STEP_TYPE);
    if (tag1 == 1 && tag2 == 1)
      this.SetTutorialStep(LettuceTutorialFourMissionEntity.TutorialStep.BENCH_TUTORIAL);
    if (tag1 == 2 && tag2 == 1)
      this.SetTutorialStep(LettuceTutorialFourMissionEntity.TutorialStep.REPLACEMENT);
    if (tag2 != 0)
      return;
    this.ShowAllMercenaryAbilityOrderBubbles();
  }

  private void SetTutorialStep(LettuceTutorialFourMissionEntity.TutorialStep step)
  {
    this.m_currentTutorialStep = step;
    GameEntity.Coroutines.StartCoroutine(this.TransitionTutorialStepCoroutine(step));
  }

  private IEnumerator TransitionTutorialStepCoroutine(
    LettuceTutorialFourMissionEntity.TutorialStep step)
  {
    LettuceTutorialFourMissionEntity fourMissionEntity = this;
    if (!GameMgr.Get().IsSpectator())
    {
      switch (step)
      {
        case LettuceTutorialFourMissionEntity.TutorialStep.BENCH_TUTORIAL:
          EndTurnButton.Get().AddInputBlocker();
          GameState.Get().GetFriendlySidePlayer().GetHandZone().AddInputBlocker();
          fourMissionEntity.CreateTutorialDialog(LettuceTutorialResources.LettuceTutorialPopupBenchPrefab, "GAMEPLAY_LETTUCE_BENCH_TITLE_TUTORIAL", "GAMEPLAY_LETTUCE_BENCH_BODY_TUTORIAL", "GAMEPLAY_LETTUCE_BENCH_BUTTON_TUTORIAL", new UIEvent.Handler(fourMissionEntity.UserPressedBenchTutorial), Vector2.zero);
          break;
        case LettuceTutorialFourMissionEntity.TutorialStep.BENCH_TUTORIAL_CLICKED:
          GameState.Get().GetFriendlySidePlayer().GetHandZone().RemoveInputBlocker();
          GameState.Get().SetBusy(false);
          yield return (object) new WaitForSeconds(5f);
          fourMissionEntity.ShowHandBounceArrow();
          break;
        case LettuceTutorialFourMissionEntity.TutorialStep.BENCH_DONE_TUTORIAL:
          fourMissionEntity.HideHandBounceArrow();
          break;
        case LettuceTutorialFourMissionEntity.TutorialStep.REPLACEMENT:
          GameState.Get().SetBusy(true);
          fourMissionEntity.CreateTutorialDialog(LettuceTutorialResources.LettuceTutorialPopupDeathReplacePrefab, "GAMEPLAY_LETTUCE_REPLACEMENT_TITLE_TUTORIAL", "GAMEPLAY_LETTUCE_REPLACEMENT_BODY_TUTORIAL", "GAMEPLAY_LETTUCE_REPLACEMENT_BUTTON_TUTORIAL", new UIEvent.Handler(fourMissionEntity.UserPressedReplacementTutorial), Vector2.zero);
          break;
      }
    }
  }

  private void UserPressedBenchTutorial(UIEvent e)
  {
    this.SetTutorialStep(LettuceTutorialFourMissionEntity.TutorialStep.BENCH_TUTORIAL_CLICKED);
    GameEntity.Coroutines.StartCoroutine(this.PlayVOOnUserPressedReplacementTutorial());
  }

  protected IEnumerator PlayVOOnUserPressedReplacementTutorial()
  {
    LettuceTutorialFourMissionEntity fourMissionEntity = this;
    float seconds = 0.5f;
    string m_SpeakerActor = "LETL_813_H1";
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(seconds);
    GameState.Get().SetBusy(false);
    Actor playByDesignCode = fourMissionEntity.FindEnemyActorInPlayByDesignCode(m_SpeakerActor);
    yield return (object) fourMissionEntity.PlayLineAlways(playByDesignCode, (string) LettuceTutorialFourMissionEntity.VO_NEW1_024_Attack_02, fourMissionEntity.enemyMinionSpeakingDirection);
  }

  protected IEnumerator PlayLineAlways(
    Actor speaker,
    string line,
    Notification.SpeechBubbleDirection direction,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LettuceTutorialFourMissionEntity fourMissionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(fourMissionEntity.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void UserPressedReplacementTutorial(UIEvent e) => GameState.Get().SetBusy(false);

  private void HideNotification(Notification notification, bool hideImmediately = false)
  {
    if (!((Object) notification != (Object) null))
      return;
    if (hideImmediately)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(notification);
    else
      NotificationManager.Get().DestroyNotification(notification, 0.0f);
  }

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    base.NotifyOfCardMousedOver(mousedOverEntity);
    if (mousedOverEntity.GetZone() != TAG_ZONE.HAND)
      return;
    this.HideNotification(this.m_handBounceArrow);
  }

  public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
  {
    base.NotifyOfCardMousedOff(mousedOffEntity);
    if (mousedOffEntity.GetZone() != TAG_ZONE.HAND || !this.m_shouldShowHandBounceArrow)
      return;
    Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(10f));
  }

  public override void NotifyOfCardGrabbed(Entity grabbedEntity)
  {
    if (grabbedEntity.GetZone() != TAG_ZONE.HAND || !this.m_shouldShowHandBounceArrow)
      return;
    this.HideNotification(this.m_handBounceArrow);
  }

  public override void NotifyOfCardDropped(Entity entity)
  {
    if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.TURN) > 1)
      return;
    if (GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCardCount() < 2)
    {
      Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(10f));
    }
    else
    {
      this.HideHandBounceArrow();
      EndTurnButton.Get().RemoveInputBlocker();
      EndTurnButton.Get().Reset();
    }
  }

  private Card GetNominateSuggestionCard()
  {
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count == 0)
      return (Card) null;
    foreach (TAG_ROLE role in new List<TAG_ROLE>()
    {
      TAG_ROLE.TANK,
      TAG_ROLE.FIGHTER,
      TAG_ROLE.CASTER
    })
    {
      Card cardByRole = this.GetCardByRole(cards, role);
      if ((Object) cardByRole != (Object) null)
        return cardByRole;
    }
    return (Card) null;
  }

  private Card GetCardByRole(List<Card> cards, TAG_ROLE role)
  {
    foreach (Card card in cards)
    {
      if (card.GetEntity().GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE) == role)
        return card;
    }
    return (Card) null;
  }

  private void ShowHandBounceArrow()
  {
    this.m_shouldShowHandBounceArrow = true;
    this.HideNotification(this.m_handBounceArrow);
    Card nominateSuggestionCard = this.GetNominateSuggestionCard();
    if ((Object) nominateSuggestionCard == (Object) null)
      return;
    Vector3 position1 = nominateSuggestionCard.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z + 2f) : new Vector3(position1.x - 0.08f, position1.y + 0.2f, position1.z + 1.2f);
    this.m_handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, 0.0f, 0.0f));
    this.m_handBounceArrow.transform.parent = nominateSuggestionCard.transform;
  }

  private void HideHandBounceArrow()
  {
    this.m_shouldShowHandBounceArrow = false;
    this.HideNotification(this.m_handBounceArrow);
  }

  private IEnumerator ShowArrowInSeconds(float seconds)
  {
    yield return (object) new WaitForSeconds(seconds);
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count != 0)
    {
      Card cardInHand = cards[0];
      while (iTween.Count(cardInHand.gameObject) > 0)
        yield return (object) null;
      if (!cardInHand.IsMousedOver() && !((Object) InputManager.Get().GetHeldCard() == (Object) cardInHand) && this.m_shouldShowHandBounceArrow)
        this.ShowHandBounceArrow();
    }
  }

  public override bool NotifyOfEndTurnButtonPushed()
  {
    if (this.GetTag(GAME_TAG.TURN) < 1)
      return false;
    if (EndTurnButton.Get().HasNoMorePlays())
      return true;
    if ((Object) this.endTurnNotifier != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
    string key = "GAMEPLAY_LETTUCE_NO_ENDTURN";
    this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
    return false;
  }

  private enum TutorialStep
  {
    Invalid,
    BENCH_TUTORIAL,
    BENCH_TUTORIAL_CLICKED,
    BENCH_DONE_TUTORIAL,
    REPLACEMENT,
  }
}
