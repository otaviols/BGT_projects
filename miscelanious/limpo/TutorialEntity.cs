using Blizzard.T5.Core;
using System.Collections;
using UnityEngine;

public class TutorialEntity : MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TutorialEntity.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TutorialEntity.InitStringOptions();
  private Notification thatsABadPlayPopup;
  private Notification manaReminder;
  private TooltipPanel historyTooltip;
  private static readonly float TUTORIAL_DIALOG_SCALE_PHONE = 1.4f;
  private static readonly Vector3 HELP_POPUP_SCALE = 16f * Vector3.one;
  protected TutorialNotification m_preTutorialNotification;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    },
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public TutorialEntity()
    : base()
  {
    this.m_gameOptions.AddOptions(TutorialEntity.s_booleanOptions, TutorialEntity.s_stringOptions);
  }

  protected override void HandleMulliganTagChange()
  {
  }

  public override bool ShouldShowHeroClassDuringMulligan(Player.Side playerSide) => false;

  public static Vector3 GetTextScale() => (bool) UniversalInputManager.UsePhoneUI ? TutorialEntity.HELP_POPUP_SCALE * TutorialEntity.TUTORIAL_DIALOG_SCALE_PHONE : TutorialEntity.HELP_POPUP_SCALE;

  public override bool NotifyOfPlayError(
    PlayErrors.ErrorType error,
    int? errorParam,
    Entity errorSource)
  {
    if (error == PlayErrors.ErrorType.REQ_ENOUGH_MANA)
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (errorSource.GetCost() > GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.RESOURCES))
      {
        Notification speechBubble = NotificationManager.Get().CreateSpeechBubble(GameStrings.Get("TUTORIAL02_JAINA_05"), Notification.SpeechBubbleDirection.BottomLeft, actor, true);
        SoundManager.Get().LoadAndPlay((AssetReference) "VO_TUTORIAL_02_JAINA_05_20.prefab:700f7c6b778de5d41bf6d45a2e01b13d");
        NotificationManager.Get().DestroyNotification(speechBubble, 3.5f);
        Gameplay.Get().StartCoroutine(this.DisplayManaReminder(GameStrings.Get("TUTORIAL02_HELP_01")));
      }
      else
      {
        Notification speechBubble = NotificationManager.Get().CreateSpeechBubble(GameStrings.Get("TUTORIAL02_JAINA_04"), Notification.SpeechBubbleDirection.BottomLeft, actor, true);
        SoundManager.Get().LoadAndPlay((AssetReference) "VO_TUTORIAL_02_JAINA_04_19.prefab:af04fcf53166d04469dc1b22b4181bf9");
        NotificationManager.Get().DestroyNotification(speechBubble, 3.5f);
        Gameplay.Get().StartCoroutine(this.DisplayManaReminder(GameStrings.Get("TUTORIAL02_HELP_03")));
      }
      return true;
    }
    if (error == PlayErrors.ErrorType.REQ_ATTACK_GREATER_THAN_0 && errorSource.GetCardId() == "TU4a_006")
      return true;
    if (error != PlayErrors.ErrorType.REQ_TARGET_TAUNTER)
      return false;
    SoundManager.Get().LoadAndPlay((AssetReference) "UI_no_can_do.prefab:7b1a22774f818544387c0f2ca4fea02c");
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_TAUNT);
    GameState.Get().ShowEnemyTauntCharacters();
    this.HighlightTaunters();
    return true;
  }

  private IEnumerator DisplayManaReminder(string reminderText)
  {
    yield return (object) new WaitForSeconds(0.5f);
    if ((Object) this.manaReminder != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.manaReminder);
    this.NotifyOfManaError();
    Vector3 crystalSpawnPosition = ManaCrystalMgr.Get().GetManaCrystalSpawnPosition();
    Vector3 position;
    Notification.PopUpArrowDirection direction;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      position = new Vector3(crystalSpawnPosition.x - 0.7f, crystalSpawnPosition.y + 1.14f, crystalSpawnPosition.z + 4.33f);
      direction = Notification.PopUpArrowDirection.RightDown;
    }
    else
    {
      position = new Vector3(crystalSpawnPosition.x - 0.02f, crystalSpawnPosition.y + 0.2f, crystalSpawnPosition.z + 1.93f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    this.manaReminder = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), reminderText);
    this.manaReminder.ShowPopUpArrow(direction);
    NotificationManager.Get().DestroyNotification(this.manaReminder, 4f);
  }

  private void HighlightTaunters()
  {
    foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
    {
      if (card.GetEntity().HasTaunt() && !card.GetEntity().IsStealthed())
      {
        NotificationManager.Get().DestroyAllPopUps();
        Vector3 position = new Vector3(card.transform.position.x - 2f, card.transform.position.y, card.transform.position.z);
        Notification fadeArrow = NotificationManager.Get().CreateFadeArrow(position, new Vector3(0.0f, 270f, 0.0f));
        NotificationManager.Get().DestroyNotification(fadeArrow, 3f);
        break;
      }
    }
  }

  public virtual bool IsCustomIntroFinished() => true;

  public void ClearPreTutorialNotification()
  {
    if ((Object) this.m_preTutorialNotification == (Object) null)
      return;
    this.m_preTutorialNotification.gameObject.SetActive(false);
  }

  public override bool NotifyOfTooltipDisplay(TooltipZone tooltip)
  {
    ZoneDeck component = tooltip.targetObject.GetComponent<ZoneDeck>();
    if ((Object) component == (Object) null)
      return false;
    if (component.m_Side == Player.Side.FRIENDLY)
    {
      string headline = GameStrings.Get("GAMEPLAY_TOOLTIP_DECK_HEADLINE");
      string bodytext = GameStrings.Get("TUTORIAL_TOOLTIP_DECK_DESCRIPTION");
      if ((bool) UniversalInputManager.UsePhoneUI)
        tooltip.ShowGameplayTooltipLarge(headline, bodytext);
      else
        tooltip.ShowGameplayTooltip(headline, bodytext);
      return true;
    }
    if (component.m_Side != Player.Side.OPPOSING)
      return false;
    string headline1 = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYDECK_HEADLINE");
    string bodytext1 = GameStrings.Get("TUTORIAL_TOOLTIP_ENEMYDECK_DESC");
    if ((bool) UniversalInputManager.UsePhoneUI)
      tooltip.ShowGameplayTooltipLarge(headline1, bodytext1);
    else
      tooltip.ShowGameplayTooltip(headline1, bodytext1);
    return true;
  }

  public override void NotifyOfHeroesFinishedAnimatingInMulligan()
  {
    Board.Get().FindCollider("DragPlane").GetComponent<Collider>().enabled = false;
    this.HandleMissionEvent(54);
  }

  public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
  {
    if (!clickedEntity.IsControlledByLocalUser())
      return true;
    Network.Options.Option selectedNetworkOption = GameState.Get().GetSelectedNetworkOption();
    if (selectedNetworkOption == null || selectedNetworkOption.Main == null)
      return true;
    Entity entity = GameState.Get().GetEntity(selectedNetworkOption.Main.ID);
    if (!wasInTargetMode || entity == null || clickedEntity == entity)
      return true;
    string cardId = entity.GetCardId();
    if (!(cardId == "CS2_022") && !(cardId == "CS2_029") && !(cardId == "CS2_034"))
      return true;
    this.ShowDontHurtYourselfPopup(clickedEntity.GetCard().transform.position);
    return false;
  }

  private void ShowDontHurtYourselfPopup(Vector3 origin)
  {
    if ((Object) this.thatsABadPlayPopup != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.thatsABadPlayPopup);
    Vector3 position = new Vector3(origin.x - 3f, origin.y, origin.z);
    this.thatsABadPlayPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_07"));
    NotificationManager.Get().DestroyNotification(this.thatsABadPlayPopup, 2.5f);
  }

  protected void HandleGameStartEvent()
  {
    MulliganManager.Get().ForceMulliganActive(true);
    MulliganManager.Get().SkipCardChoosing();
    TurnStartManager.Get().BeginListeningForTurnEvents();
  }

  protected void UserPressedStartButton(UIEvent e) => this.HandleMissionEvent(55);

  protected TutorialNotification ShowTutorialDialog(
    string headlineGameString,
    string bodyTextGameString,
    string buttonGameString,
    Vector2 materialOffset,
    bool swapMaterial = false)
  {
    return NotificationManager.Get().CreateTutorialDialog(headlineGameString, bodyTextGameString, buttonGameString, new UIEvent.Handler(this.UserPressedStartButton), materialOffset, swapMaterial);
  }

  public override void NotifyOfHistoryTokenMousedOver(GameObject mousedOverTile)
  {
    this.historyTooltip = TooltipPanelManager.Get().CreateKeywordPanel(0);
    this.historyTooltip.Reset();
    this.historyTooltip.Initialize(GameStrings.Get("TUTORIAL_TOOLTIP_HISTORY_HEADLINE"), GameStrings.Get("TUTORIAL_TOOLTIP_HISTORY_DESC"));
    Vector3 vector3 = !UniversalInputManager.Get().IsTouchMode() ? new Vector3(-1.140343f, 0.1916952f, 0.4895353f) : new Vector3(1f, 0.1916952f, 1.2f);
    this.historyTooltip.transform.parent = mousedOverTile.GetComponent<HistoryCard>().m_mainCardActor.transform;
    float num = 0.4792188f;
    this.historyTooltip.transform.localPosition = vector3;
    this.historyTooltip.transform.localScale = new Vector3(num, num, num);
  }

  public override void NotifyOfHistoryTokenMousedOut()
  {
    if (!((Object) this.historyTooltip != (Object) null))
      return;
    Object.Destroy((Object) this.historyTooltip.gameObject);
  }

  protected virtual void NotifyOfManaError()
  {
  }

  protected void SetTutorialLostProgress(TutorialProgress val)
  {
    int val1 = Options.Get().GetInt(Option.TUTORIAL_LOST_PROGRESS) | 1 << (int) (val & (TutorialProgress) 31);
    Options.Get().SetInt(Option.TUTORIAL_LOST_PROGRESS, val1);
    AdTrackingManager.Get().TrackTutorialProgress(val, false);
  }

  protected bool DidLoseTutorial(TutorialProgress val)
  {
    int num1 = Options.Get().GetInt(Option.TUTORIAL_LOST_PROGRESS);
    bool flag = false;
    int num2 = 1 << (int) (val & (TutorialProgress) 31);
    if ((num1 & num2) > 0)
      flag = true;
    return flag;
  }

  protected void ResetTutorialLostProgress() => Options.Get().SetInt(Option.TUTORIAL_LOST_PROGRESS, 0);
}
