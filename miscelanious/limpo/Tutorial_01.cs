using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_01 : TutorialEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = Tutorial_01.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = Tutorial_01.InitStringOptions();
  private Notification endTurnNotifier;
  private Notification handBounceArrow;
  private Notification handFadeArrow;
  private Notification noFireballPopup;
  private Notification attackWithYourMinion;
  private Notification crushThisGnoll;
  private Notification freeCardsPopup;
  private TooltipPanel attackHelpPanel;
  private TooltipPanel healthHelpPanel;
  private Card mousedOverCard;
  private GameObject costLabel;
  private GameObject attackLabel;
  private GameObject healthLabel;
  private Card firstMurlocCard;
  private Card firstRaptorCard;
  private int numTimesTextSwapStarted;
  private string textToShowForAttackTip = GameStrings.Get("TUTORIAL01_HELP_02");
  private GameObject startingPack;
  private bool packOpened;
  private bool announcerIsFinishedYapping;
  private bool firstAttackFinished;
  private bool m_jainaSpeaking;
  private bool m_isShowingAttackHelpPanel;
  private bool m_customIntroFinished;
  private PlatformDependentValue<float> m_gemScale = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = 1.75f,
    Phone = 1.2f
  };
  private PlatformDependentValue<Vector3> m_attackTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(-2.15f, 0.0f, -0.62f),
    Phone = new Vector3(-3.5f, 0.0f, -0.62f)
  };
  private PlatformDependentValue<Vector3> m_healthTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(2.05f, 0.0f, -0.62f),
    Phone = new Vector3(3.25f, 0.0f, -0.62f)
  };
  private PlatformDependentValue<Vector3> m_heroHealthTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(2.4f, 0.3f, -0.8f),
    Phone = new Vector3(3.5f, 0.3f, 0.6f)
  };

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.MOUSEOVER_DELAY_OVERRIDDEN,
      true
    },
    {
      GameEntityOption.SHOW_HERO_TOOLTIPS,
      true
    },
    {
      GameEntityOption.DISABLE_TOOLTIPS,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public Tutorial_01()
  {
    this.m_gameOptions.AddOptions(Tutorial_01.s_booleanOptions, Tutorial_01.s_stringOptions);
    MulliganManager.Get().ForceMulliganActive(true);
  }

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_01.prefab:79419083a1b828341be6d208491a88f8");
    this.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_02.prefab:d6b08fa7e06a51c4abd80eea2ea30a41");
    this.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_03.prefab:f47d0faf9067b3341bb9adb38f90be5b");
    this.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_04.prefab:e6fb72da1414d454f9d96a51c7009a82");
    this.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_05.prefab:635b33010e4704a42a87c7625b5b5ada");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_13_10.prefab:b13670e36c248e141837c4eb0645a000");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_01_01.prefab:883391234efbde84eb99a16abd164d9d");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_02_02.prefab:cccdcb509085a974d922ac1d545d9bb6");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_03_03.prefab:4921407046d90bb44b2bfcf3984ffd47");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_20_16.prefab:7980d02c581e4174991a8066e5785666");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_05_05.prefab:982193e53ab81f04ba562de4b32dd39c");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_06_06.prefab:ffe0ebdca06ca1d4c84cc28e4a1ed7cf");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_07_07.prefab:a8bf811494e94d742a3910fac9da906f");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_21_17.prefab:c1524bd0ef92bb845b5dab48cbd017f9");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_09_08.prefab:b7b739d9e31865a478275394ee57ad89");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_15_11.prefab:a644986d34ab8964582c6221cde54d45");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_16_12.prefab:e6b4ab6fc1f11634e88f013ce5351e46");
    this.PreloadSound("VO_TUTORIAL_JAINA_02_55_ALT2.prefab:d049e67ad6c16db4da2c04be7a02a1ae");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_10_09.prefab:5bf553d532aca174083f48bf407b2b11");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_17_13.prefab:9b257c86e7c7f9045a2b819d35876aca");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_18_14.prefab:fedcdecb3346ec745b6fb4204f7dd4e0");
    this.PreloadSound("VO_TUTORIAL_01_JAINA_19_15.prefab:659652a121ac01941a40c64c1c151f87");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_01_01.prefab:5833f4aeb72110741a2c9bc3a92f9bc8");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_02_02.prefab:7f321b26431a4974a82deefc368adf63");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_03_03.prefab:4ef21f71824b97842b33d8ebccb37ed2");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_04_04.prefab:3e16e42edb324e2469a25363ffd013a6");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_06_06_ALT.prefab:6c9ef3c501462474ab59a37b967cab6f");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_08_08_ALT.prefab:19ddb4ddaa4aee2468b17bae25da9419");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_09_09_ALT.prefab:70c4d2941509856448660f89d6c72b2b");
    this.PreloadSound("VO_TUTORIAL_01_HOGGER_11_11.prefab:1fdb0543bf56c4b4e95148a518bd9a2d");
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    if ((Object) this.attackHelpPanel != (Object) null)
    {
      Object.Destroy((Object) this.attackHelpPanel.gameObject);
      this.attackHelpPanel = (TooltipPanel) null;
    }
    if ((Object) this.healthHelpPanel != (Object) null)
    {
      Object.Destroy((Object) this.healthHelpPanel.gameObject);
      this.healthHelpPanel = (TooltipPanel) null;
    }
    this.EnsureCardGemsAreOnTheCorrectLayer();
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        GameUtils.SetTutorialProgress(TutorialProgress.HOGGER_COMPLETE);
        this.PlaySound("VO_TUTORIAL_01_HOGGER_11_11.prefab:1fdb0543bf56c4b4e95148a518bd9a2d");
        break;
      case TAG_PLAYSTATE.TIED:
        this.PlaySound("VO_TUTORIAL_01_HOGGER_11_11.prefab:1fdb0543bf56c4b4e95148a518bd9a2d");
        break;
    }
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    InputManager.Get().RemovePhoneHandShownListener(new InputManager.PhoneHandShownCallback(this.OnPhoneHandShown));
    InputManager.Get().RemovePhoneHandHiddenListener(new InputManager.PhoneHandHiddenCallback(this.OnPhoneHandHidden));
  }

  private void EnsureCardGemsAreOnTheCorrectLayer()
  {
    List<Card> cardList = new List<Card>();
    cardList.AddRange((IEnumerable<Card>) GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards());
    cardList.AddRange((IEnumerable<Card>) GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards());
    cardList.Add(GameState.Get().GetFriendlySidePlayer().GetHeroCard());
    cardList.Add(GameState.Get().GetOpposingSidePlayer().GetHeroCard());
    foreach (Card card in cardList)
    {
      if (!((Object) card == (Object) null) && !((Object) card.GetActor() == (Object) null))
      {
        if ((Object) card.GetActor().GetAttackObject() != (Object) null)
          LayerUtils.SetLayer(card.GetActor().GetAttackObject().gameObject, GameLayer.Default);
        if ((Object) card.GetActor().GetHealthObject() != (Object) null)
          LayerUtils.SetLayer(card.GetActor().GetHealthObject().gameObject, GameLayer.Default);
      }
    }
  }

  public override void NotifyOfCardGrabbed(Entity entity)
  {
    if (this.GetTag(GAME_TAG.TURN) == 2 || entity.GetCardId() == "TU5_CS2_025")
      BoardTutorial.Get().EnableHighlight(true);
    this.NukeNumberLabels();
  }

  public override void NotifyOfCardDropped(Entity entity)
  {
    if (this.GetTag(GAME_TAG.TURN) != 2 && !(entity.GetCardId() == "TU5_CS2_025"))
      return;
    BoardTutorial.Get().EnableHighlight(false);
  }

  public override bool NotifyOfEndTurnButtonPushed()
  {
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket != null && !optionsPacket.HasValidOption())
    {
      NotificationManager.Get().DestroyAllArrows();
      return true;
    }
    if ((Object) this.endTurnNotifier != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
    string key = "TUTORIAL_NO_ENDTURN_ATK";
    if (!GameState.Get().GetFriendlySidePlayer().HasReadyAttackers())
      key = "TUTORIAL_NO_ENDTURN";
    this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
    return false;
  }

  public override bool NotifyOfPlayError(
    PlayErrors.ErrorType error,
    int? errorParam,
    Entity errorSource)
  {
    return error == PlayErrors.ErrorType.REQ_ATTACK_GREATER_THAN_0 && errorSource.GetCardId() == "TU4a_006";
  }

  public override void NotifyOfTargetModeCancelled()
  {
    if ((Object) this.crushThisGnoll == (Object) null)
      return;
    NotificationManager.Get().DestroyAllPopUps();
    if ((Object) this.firstRaptorCard == (Object) null || !(this.firstRaptorCard.GetZone() is ZonePlay))
      return;
    this.ShowAttackWithYourMinionPopup();
  }

  public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
  {
    if (this.GetTag(GAME_TAG.TURN) == 4)
    {
      if (clickedEntity.GetCardId() == "TU5_CS2_168")
      {
        if (!wasInTargetMode && !this.firstAttackFinished)
        {
          if ((Object) this.crushThisGnoll != (Object) null)
            NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.crushThisGnoll);
          NotificationManager.Get().DestroyAllPopUps();
          Vector3 position1 = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard().transform.position;
          Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
          this.crushThisGnoll = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_03"));
          this.crushThisGnoll.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
          ++this.numTimesTextSwapStarted;
          Gameplay.Get().StartCoroutine(this.WaitAndThenHide(this.numTimesTextSwapStarted));
        }
      }
      else if (clickedEntity.GetCardId() == "TU4a_002" & wasInTargetMode)
      {
        if ((Object) this.crushThisGnoll != (Object) null)
          NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.crushThisGnoll);
        NotificationManager.Get().DestroyAllPopUps();
        this.firstAttackFinished = true;
      }
    }
    else if (this.GetTag(GAME_TAG.TURN) == 6 && clickedEntity.GetCardId() == "TU4a_001" && wasInTargetMode)
      NotificationManager.Get().DestroyAllPopUps();
    if (wasInTargetMode && (Object) InputManager.Get().GetHeldCard() != (Object) null && InputManager.Get().GetHeldCard().GetEntity().GetCardId() == "TU5_CS2_029")
    {
      if (clickedEntity.IsControlledByLocalUser())
      {
        this.ShowDontFireballYourselfPopup(clickedEntity.GetCard().transform.position);
        return false;
      }
      if (clickedEntity.GetCardId() == "TU4a_003" && this.GetTag(GAME_TAG.TURN) >= 8)
      {
        if ((Object) this.noFireballPopup != (Object) null)
          NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.noFireballPopup);
        Vector3 position3 = clickedEntity.GetCard().transform.position;
        Vector3 position4 = new Vector3(position3.x - 3f, position3.y, position3.z);
        this.noFireballPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position4, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_08"));
        NotificationManager.Get().DestroyNotification(this.noFireballPopup, 3f);
        return false;
      }
    }
    return true;
  }

  private IEnumerator WaitAndThenHide(int numTimesStarted)
  {
    yield return (object) new WaitForSeconds(6f);
    if (!((Object) this.crushThisGnoll == (Object) null) && numTimesStarted == this.numTimesTextSwapStarted && !((Object) GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard() == (Object) null))
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.crushThisGnoll);
  }

  public override bool NotifyOfCardTooltipDisplayShow(Card card)
  {
    if (GameState.Get().IsGameOver())
      return false;
    Entity entity = card.GetEntity();
    if (entity.IsMinion())
    {
      if ((Object) this.attackHelpPanel == (Object) null)
      {
        this.m_isShowingAttackHelpPanel = true;
        this.ShowAttackTooltip(card);
        Gameplay.Get().StartCoroutine(this.ShowHealthTooltipAfterWait(card));
      }
      return false;
    }
    if (!entity.IsHero())
      return true;
    if ((Object) this.healthHelpPanel == (Object) null)
      this.ShowHealthTooltip(card);
    return false;
  }

  private void ShowAttackTooltip(Card card)
  {
    LayerUtils.SetLayer(card.GetActor().GetAttackObject().gameObject, GameLayer.Tooltip);
    Vector3 position = card.transform.position;
    Vector3 attackTooltipPosition = (Vector3) this.m_attackTooltipPosition;
    Vector3 vector3 = new Vector3(position.x + attackTooltipPosition.x, position.y + attackTooltipPosition.y, position.z + attackTooltipPosition.z);
    this.attackHelpPanel = TooltipPanelManager.Get().CreateKeywordPanel(0);
    this.attackHelpPanel.Reset();
    this.attackHelpPanel.Initialize(GameStrings.Get("GLOBAL_ATTACK"), GameStrings.Get("TUTORIAL01_HELP_12"));
    this.attackHelpPanel.SetScale((float) TooltipPanel.GAMEPLAY_SCALE);
    this.attackHelpPanel.transform.position = vector3;
    RenderUtils.SetAlpha(this.attackHelpPanel.gameObject, 0.0f);
    iTween.FadeTo(this.attackHelpPanel.gameObject, iTween.Hash((object) "alpha", (object) 1, (object) "time", (object) 0.25f));
    card.GetActor().GetAttackObject().Enlarge((float) this.m_gemScale);
  }

  private IEnumerator ShowHealthTooltipAfterWait(Card card)
  {
    yield return (object) new WaitForSeconds(0.05f);
    if (!((Object) InputManager.Get().GetMousedOverCard() != (Object) card))
      this.ShowHealthTooltip(card);
  }

  private void ShowHealthTooltip(Card card)
  {
    LayerUtils.SetLayer(card.GetActor().GetHealthObject().gameObject, GameLayer.Tooltip);
    Vector3 position = card.transform.position;
    Vector3 healthTooltipPosition = (Vector3) this.m_healthTooltipPosition;
    if (card.GetEntity().IsHero())
    {
      healthTooltipPosition = (Vector3) this.m_heroHealthTooltipPosition;
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        if (!card.GetEntity().IsControlledByLocalUser())
          healthTooltipPosition.z -= 0.75f;
        else if (Localization.GetLocale() == Locale.ruRU)
          ++healthTooltipPosition.z;
      }
    }
    Vector3 vector3 = new Vector3(position.x + healthTooltipPosition.x, position.y + healthTooltipPosition.y, position.z + healthTooltipPosition.z);
    this.healthHelpPanel = TooltipPanelManager.Get().CreateKeywordPanel(0);
    this.healthHelpPanel.Reset();
    this.healthHelpPanel.Initialize(GameStrings.Get("GLOBAL_HEALTH"), GameStrings.Get("TUTORIAL01_HELP_13"));
    this.healthHelpPanel.SetScale((float) TooltipPanel.GAMEPLAY_SCALE);
    this.healthHelpPanel.transform.position = vector3;
    RenderUtils.SetAlpha(this.healthHelpPanel.gameObject, 0.0f);
    iTween.FadeTo(this.healthHelpPanel.gameObject, iTween.Hash((object) "alpha", (object) 1, (object) "time", (object) 0.25f));
    card.GetActor().GetHealthObject().Enlarge((float) this.m_gemScale);
  }

  public override void NotifyOfCardTooltipDisplayHide(Card card)
  {
    if ((Object) this.attackHelpPanel != (Object) null)
    {
      if ((Object) card != (Object) null)
      {
        GemObject attackObject = card.GetActor().GetAttackObject();
        LayerUtils.SetLayer(attackObject.gameObject, GameLayer.Default);
        attackObject.Shrink();
      }
      Object.Destroy((Object) this.attackHelpPanel.gameObject);
      this.m_isShowingAttackHelpPanel = false;
    }
    if (!((Object) this.healthHelpPanel != (Object) null))
      return;
    if ((Object) card != (Object) null)
    {
      GemObject healthObject = card.GetActor().GetHealthObject();
      LayerUtils.SetLayer(healthObject.gameObject, GameLayer.Default);
      healthObject.Shrink();
    }
    Object.Destroy((Object) this.healthHelpPanel.gameObject);
  }

  private void ManaLabelLoadedCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    if (this.m_isShowingAttackHelpPanel)
      return;
    GameObject costTextObject = ((Card) callbackData).GetActor().GetCostTextObject();
    if ((Object) costTextObject == (Object) null)
    {
      Object.Destroy((Object) go);
    }
    else
    {
      this.costLabel = go;
      go.transform.parent = costTextObject.transform;
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.transform.localPosition = new Vector3(-0.017f, 0.3512533f, 0.0f);
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_COST");
    }
  }

  private void AttackLabelLoadedCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if (this.m_isShowingAttackHelpPanel)
      return;
    GameObject attackTextObject = ((Card) callbackData).GetActor().GetAttackTextObject();
    if ((Object) attackTextObject == (Object) null)
    {
      Object.Destroy((Object) go);
    }
    else
    {
      this.attackLabel = go;
      go.transform.parent = attackTextObject.transform;
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.transform.localPosition = new Vector3(-0.2f, -0.3039344f, 0.0f);
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_ATTACK");
    }
  }

  private void HealthLabelLoadedCallback(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if (this.m_isShowingAttackHelpPanel)
      return;
    GameObject healthTextObject = ((Card) callbackData).GetActor().GetHealthTextObject();
    if ((Object) healthTextObject == (Object) null)
    {
      Object.Destroy((Object) go);
    }
    else
    {
      this.healthLabel = go;
      go.transform.parent = healthTextObject.transform;
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.transform.localPosition = new Vector3(0.21f, -0.31f, 0.0f);
      go.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      go.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_HEALTH");
    }
  }

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    if (this.ShouldShowArrowOnCardInHand(mousedOverEntity))
      NotificationManager.Get().DestroyAllArrows();
    if (mousedOverEntity.GetZone() != TAG_ZONE.HAND)
      return;
    this.mousedOverCard = mousedOverEntity.GetCard();
    IAssetLoader assetLoader = AssetLoader.Get();
    assetLoader.InstantiatePrefab((AssetReference) "NumberLabel.prefab:597544d5ed24b994f95fe56e28584992", new PrefabCallback<GameObject>(this.ManaLabelLoadedCallback), (object) this.mousedOverCard, AssetLoadingOptions.IgnorePrefabPosition);
    assetLoader.InstantiatePrefab((AssetReference) "NumberLabel.prefab:597544d5ed24b994f95fe56e28584992", new PrefabCallback<GameObject>(this.AttackLabelLoadedCallback), (object) this.mousedOverCard, AssetLoadingOptions.IgnorePrefabPosition);
    assetLoader.InstantiatePrefab((AssetReference) "NumberLabel.prefab:597544d5ed24b994f95fe56e28584992", new PrefabCallback<GameObject>(this.HealthLabelLoadedCallback), (object) this.mousedOverCard, AssetLoadingOptions.IgnorePrefabPosition);
  }

  public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
  {
    if (this.ShouldShowArrowOnCardInHand(mousedOffEntity))
      Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0.5f));
    this.NukeNumberLabels();
  }

  private void NukeNumberLabels()
  {
    this.mousedOverCard = (Card) null;
    if ((Object) this.costLabel != (Object) null)
      Object.Destroy((Object) this.costLabel);
    if ((Object) this.attackLabel != (Object) null)
      Object.Destroy((Object) this.attackLabel);
    if (!((Object) this.healthLabel != (Object) null))
      return;
    Object.Destroy((Object) this.healthLabel);
  }

  private bool ShouldShowArrowOnCardInHand(Entity entity)
  {
    if (entity.GetZone() != TAG_ZONE.HAND)
      return false;
    switch (this.GetTag(GAME_TAG.TURN))
    {
      case 2:
        return true;
      case 4:
        if (GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count == 0)
          return true;
        break;
    }
    return false;
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
      if (!cardInHand.IsMousedOver() && !((Object) InputManager.Get().GetHeldCard() == (Object) cardInHand))
        this.ShowHandBouncingArrow();
    }
  }

  private void ShowHandBouncingArrow()
  {
    if ((Object) this.handBounceArrow != (Object) null)
      return;
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count == 0)
      return;
    Card card = cards[0];
    Vector3 position1 = card.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z + 2f) : new Vector3(position1.x - 0.08f, position1.y + 0.2f, position1.z + 1.2f);
    this.handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, 0.0f, 0.0f));
    this.handBounceArrow.transform.parent = card.transform;
  }

  private void ShowHandFadeArrow()
  {
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count == 0)
      return;
    this.ShowFadeArrow(cards[0]);
  }

  private void ShowFadeArrow(Card card, Card target = null)
  {
    if ((Object) this.handFadeArrow != (Object) null)
      return;
    Vector3 position1 = card.transform.position;
    Vector3 rotation = new Vector3(0.0f, 180f, 0.0f);
    Vector3 position2;
    if ((Object) target != (Object) null)
    {
      Vector3 vector3_1 = target.transform.position - position1;
      Vector3 vector3_2 = new Vector3(position1.x, position1.y + 0.47f, position1.z + 0.27f);
      float num = Vector3.Angle(target.transform.position - vector3_2, new Vector3(0.0f, 0.0f, -1f));
      rotation = new Vector3(0.0f, -Mathf.Sign(vector3_1.x) * num, 0.0f);
      position2 = vector3_2 + 0.3f * vector3_1;
    }
    else
      position2 = new Vector3(position1.x, position1.y + 0.047f, position1.z + 0.95f);
    this.handFadeArrow = NotificationManager.Get().CreateFadeArrow(position2, rotation);
    if ((Object) target != (Object) null)
      this.handFadeArrow.transform.localScale = 1.25f * Vector3.one;
    this.handFadeArrow.transform.parent = card.transform;
  }

  private void HideFadeArrow()
  {
    if (!((Object) this.handFadeArrow != (Object) null))
      return;
    NotificationManager.Get().DestroyNotification(this.handFadeArrow, 0.0f);
    this.handFadeArrow = (Notification) null;
  }

  private void OnPhoneHandShown(object userData)
  {
    if ((Object) this.handBounceArrow != (Object) null)
    {
      NotificationManager.Get().DestroyNotification(this.handBounceArrow, 0.0f);
      this.handBounceArrow = (Notification) null;
    }
    this.ShowHandFadeArrow();
  }

  private void OnPhoneHandHidden(object userData)
  {
    this.HideFadeArrow();
    this.ShowHandBouncingArrow();
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    Tutorial_01 tutorial01 = this;
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        List<Card> cards1 = GameState.Get().GetFriendlySidePlayer().GetDeckZone().GetCards();
        tutorial01.firstMurlocCard = cards1[cards1.Count - 1];
        tutorial01.firstRaptorCard = cards1[cards1.Count - 2];
        GameState.Get().SetBusy(true);
        Board.Get().FindCollider("DragPlane").enabled = false;
        yield return (object) new WaitForSeconds(1.25f);
        TutorialNotification tutorialNotification = tutorial01.ShowTutorialDialog("TUTORIAL01_HELP_14", "TUTORIAL01_HELP_15", "TUTORIAL01_HELP_16", Vector2.zero);
        tutorialNotification.SetWantedText(GameStrings.Get("MISSION_PRE_TUTORIAL_WANTED"));
        tutorial01.m_preTutorialNotification = tutorialNotification;
        break;
      case 2:
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          InputManager.Get().RegisterPhoneHandShownListener(new InputManager.PhoneHandShownCallback(tutorial01.OnPhoneHandShown));
          InputManager.Get().RegisterPhoneHandHiddenListener(new InputManager.PhoneHandHiddenCallback(tutorial01.OnPhoneHandHidden));
        }
        yield return (object) new WaitForSeconds(1f);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_02_02.prefab:cccdcb509085a974d922ac1d545d9bb6", "TUTORIAL01_JAINA_02", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        List<Card> cards2 = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
        if (tutorial01.GetTag(GAME_TAG.TURN) != 2 || cards2.Count != 1 || !((Object) InputManager.Get().GetHeldCard() == (Object) null) || cards2[0].IsMousedOver())
          break;
        Gameplay.Get().StartCoroutine(tutorial01.ShowArrowInSeconds(0.0f));
        break;
      case 3:
        if (!(bool) UniversalInputManager.UsePhoneUI)
          break;
        InputManager.Get().RemovePhoneHandShownListener(new InputManager.PhoneHandShownCallback(tutorial01.OnPhoneHandShown));
        InputManager.Get().RemovePhoneHandHiddenListener(new InputManager.PhoneHandHiddenCallback(tutorial01.OnPhoneHandHidden));
        break;
      case 4:
        actor.SetActorState(ActorStateType.CARD_IDLE);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_06_06.prefab:ffe0ebdca06ca1d4c84cc28e4a1ed7cf", "TUTORIAL01_JAINA_06", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        if (!((Object) tutorial01.firstMurlocCard != (Object) null))
          break;
        tutorial01.firstMurlocCard.GetActor().ToggleForceIdle(true);
        tutorial01.firstMurlocCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
        break;
      case 6:
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_17_13.prefab:9b257c86e7c7f9045a2b819d35876aca", "TUTORIAL01_JAINA_17", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        break;
      case 8:
        tutorial01.m_jainaSpeaking = true;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_18_14.prefab:fedcdecb3346ec745b6fb4204f7dd4e0", "TUTORIAL01_JAINA_18", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        tutorial01.m_jainaSpeaking = false;
        yield return (object) new WaitForSeconds(1f);
        Gameplay.Get().StartCoroutine(tutorial01.FlashMinionUntilAttackBegins(tutorial01.firstRaptorCard));
        break;
      case 10:
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_19_15.prefab:659652a121ac01941a40c64c1c151f87", "TUTORIAL01_JAINA_19", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        break;
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    Tutorial_01 tutorial01 = this;
    Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor hoggerActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    AudioSource prevLine;
    Vector3 middleSpot;
    Notification innkeeperLine;
    switch (missionEvent)
    {
      case 1:
        GameState.Get().SetBusy(true);
        HistoryManager.Get().DisableHistory();
        goto case 6;
      case 2:
        GameState.Get().SetBusy(true);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_01_01.prefab:883391234efbde84eb99a16abd164d9d", "TUTORIAL01_JAINA_01", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        Gameplay.Get().SetGameStateBusy(false, 2.2f);
        goto case 6;
      case 3:
        int turn = GameState.Get().GetTurn();
        yield return (object) new WaitForSeconds(2f);
        if (turn != GameState.Get().GetTurn())
          break;
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_03_03.prefab:4921407046d90bb44b2bfcf3984ffd47", "TUTORIAL01_JAINA_03", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        if (tutorial01.GetTag(GAME_TAG.TURN) == 2 && !EndTurnButton.Get().IsInWaitingState())
        {
          tutorial01.ShowEndTurnBouncingArrow();
          goto case 6;
        }
        else
          goto case 6;
      case 4:
        GameState.Get().SetBusy(true);
        prevLine = tutorial01.GetPreloadedSound("VO_TUTORIAL_01_JAINA_03_03.prefab:4921407046d90bb44b2bfcf3984ffd47");
        while (SoundManager.Get().IsPlaying(prevLine))
          yield return (object) null;
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_20_16.prefab:7980d02c581e4174991a8066e5785666", "TUTORIAL01_JAINA_20", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_06_06_ALT.prefab:6c9ef3c501462474ab59a37b967cab6f", "TUTORIAL01_HOGGER_07", Notification.SpeechBubbleDirection.TopRight, hoggerActor));
        Vector3 position1 = jainaActor.transform.position;
        Vector3 position2 = new Vector3(position1.x + 3.3f, position1.y + 0.5f, position1.z - 0.85f);
        Notification.PopUpArrowDirection direction1 = Notification.PopUpArrowDirection.Left;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          position2 = new Vector3(position1.x + 3f, position1.y + 0.5f, position1.z + 0.85f);
          direction1 = Notification.PopUpArrowDirection.LeftDown;
        }
        Notification popupText1 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_01"));
        popupText1.ShowPopUpArrow(direction1);
        NotificationManager.Get().DestroyNotification(popupText1, 5f);
        Gameplay.Get().SetGameStateBusy(false, 5.2f);
        goto case 6;
      case 5:
        tutorial01.HideFadeArrow();
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_05_05.prefab:982193e53ab81f04ba562de4b32dd39c", "TUTORIAL01_JAINA_05", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        goto case 6;
      case 6:
        prevLine = (AudioSource) null;
        middleSpot = new Vector3();
        innkeeperLine = (Notification) null;
        break;
      case 7:
        NotificationManager.Get().DestroyAllPopUps();
        yield return (object) new WaitForSeconds(1.7f);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_07_07.prefab:a8bf811494e94d742a3910fac9da906f", "TUTORIAL01_JAINA_07", Notification.SpeechBubbleDirection.BottomRight, jainaActor));
        if ((Object) tutorial01.firstRaptorCard != (Object) null)
        {
          Vector3 position3 = tutorial01.firstRaptorCard.transform.position;
          Notification popupText2;
          if ((Object) tutorial01.firstMurlocCard != (Object) null && tutorial01.firstRaptorCard.GetZonePosition() > tutorial01.firstMurlocCard.GetZonePosition())
          {
            Vector3 position4 = new Vector3(position3.x + 3f, position3.y, position3.z);
            popupText2 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position4, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_04"));
            popupText2.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
          }
          else
          {
            Vector3 position5 = new Vector3(position3.x - 3f, position3.y, position3.z);
            popupText2 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position5, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_04"));
            popupText2.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
          }
          NotificationManager.Get().DestroyNotification(popupText2, 4f);
        }
        yield return (object) new WaitForSeconds(4f);
        if (GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count > 1 && !GameState.Get().IsInTargetMode())
          tutorial01.ShowAttackWithYourMinionPopup();
        if (tutorial01.GetTag(GAME_TAG.TURN) == 4 && EndTurnButton.Get().IsInNMPState())
        {
          yield return (object) new WaitForSeconds(1f);
          tutorial01.ShowEndTurnBouncingArrow();
          goto case 6;
        }
        else
          goto case 6;
      case 8:
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_03_03.prefab:4ef21f71824b97842b33d8ebccb37ed2", "TUTORIAL01_HOGGER_05", Notification.SpeechBubbleDirection.TopRight, hoggerActor));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_21_17.prefab:c1524bd0ef92bb845b5dab48cbd017f9", "TUTORIAL01_JAINA_21", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        GameState.Get().SetBusy(false);
        goto case 6;
      case 12:
        yield return (object) new WaitForSeconds(1f);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_15_11.prefab:a644986d34ab8964582c6221cde54d45", "TUTORIAL01_JAINA_15", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        goto case 6;
      case 13:
        while (tutorial01.m_jainaSpeaking)
          yield return (object) null;
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_16_12.prefab:e6b4ab6fc1f11634e88f013ce5351e46", "TUTORIAL01_JAINA_16", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        goto case 6;
      case 14:
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_08_08_ALT.prefab:19ddb4ddaa4aee2468b17bae25da9419", "TUTORIAL01_HOGGER_08", Notification.SpeechBubbleDirection.TopRight, hoggerActor));
        Vector3 position6 = hoggerActor.transform.position;
        Vector3 position7 = new Vector3(position6.x + 3.3f, position6.y + 0.5f, position6.z - 1f);
        if ((bool) UniversalInputManager.UsePhoneUI)
          position7 = new Vector3(position6.x + 3f, position6.y + 0.5f, position6.z - 0.75f);
        Notification.PopUpArrowDirection direction2 = Notification.PopUpArrowDirection.Left;
        Notification popupText3 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position7, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_09"));
        popupText3.ShowPopUpArrow(direction2);
        NotificationManager.Get().DestroyNotification(popupText3, 5f);
        if (tutorial01.GetTag(GAME_TAG.TURN) == 6 && EndTurnButton.Get().IsInNMPState())
        {
          yield return (object) new WaitForSeconds(9f);
          tutorial01.ShowEndTurnBouncingArrow();
          goto case 6;
        }
        else
          goto case 6;
      case 15:
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_JAINA_02_55_ALT2.prefab:d049e67ad6c16db4da2c04be7a02a1ae", "", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        goto case 6;
      case 20:
        GameState.Get().SetBusy(true);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_10_09.prefab:5bf553d532aca174083f48bf407b2b11", "TUTORIAL01_JAINA_10", Notification.SpeechBubbleDirection.BottomLeft, jainaActor));
        yield return (object) new WaitForSeconds(1.5f);
        GameState.Get().SetBusy(false);
        List<Card> cards = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards();
        cards[cards.Count - 1].GetActor().GetAttackObject().Jiggle();
        goto case 6;
      case 22:
        GameState.Get().SetBusy(true);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_09_09_ALT.prefab:70c4d2941509856448660f89d6c72b2b", "TUTORIAL01_HOGGER_02", Notification.SpeechBubbleDirection.TopRight, hoggerActor));
        Gameplay.Get().SetGameStateBusy(false, 2f);
        goto case 6;
      case 55:
        tutorial01.GetGameOptions().SetBooleanOption(GameEntityOption.DISABLE_TOOLTIPS, false);
        Board.Get().FindCollider("DragPlane").enabled = true;
        while (!tutorial01.announcerIsFinishedYapping)
          yield return (object) null;
        if (!SoundUtils.CanDetectVolume())
        {
          Notification battlebegin = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 84.8f), GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_05"), "", 15f);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_05.prefab:635b33010e4704a42a87c7625b5b5ada", "", Notification.SpeechBubbleDirection.None, (Actor) null));
          NotificationManager.Get().DestroyNotification(battlebegin, 0.0f);
          battlebegin = (Notification) null;
        }
        else
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_05.prefab:635b33010e4704a42a87c7625b5b5ada", "", Notification.SpeechBubbleDirection.None, (Actor) null));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_01_01.prefab:5833f4aeb72110741a2c9bc3a92f9bc8", "TUTORIAL01_HOGGER_01", Notification.SpeechBubbleDirection.TopRight, hoggerActor));
        GameState.Get().SetBusy(false);
        yield return (object) new WaitForSeconds(4f);
        Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_04_04.prefab:3e16e42edb324e2469a25363ffd013a6", "TUTORIAL01_HOGGER_06", Notification.SpeechBubbleDirection.TopRight, hoggerActor));
        goto case 6;
      case 66:
        Vector3 position8 = new Vector3(136f, NotificationManager.DEPTH, 131f);
        middleSpot = new Vector3(136f, NotificationManager.DEPTH, 80f);
        innkeeperLine = (Notification) null;
        if (!SoundUtils.CanDetectVolume())
        {
          innkeeperLine = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, position8, GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_01"), "", 15f);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_01.prefab:79419083a1b828341be6d208491a88f8", "", Notification.SpeechBubbleDirection.None, (Actor) null));
          NotificationManager.Get().DestroyNotification(innkeeperLine, 0.0f);
        }
        else
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_01.prefab:79419083a1b828341be6d208491a88f8", "", Notification.SpeechBubbleDirection.None, (Actor) null));
        yield return (object) new WaitForSeconds(0.5f);
        if (!SoundUtils.CanDetectVolume())
        {
          innkeeperLine = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, middleSpot, GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_02"), "", 15f);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_02.prefab:d6b08fa7e06a51c4abd80eea2ea30a41", "", Notification.SpeechBubbleDirection.None, (Actor) null));
          NotificationManager.Get().DestroyNotification(innkeeperLine, 0.0f);
        }
        else
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_02.prefab:d6b08fa7e06a51c4abd80eea2ea30a41", "", Notification.SpeechBubbleDirection.None, (Actor) null));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_02_02.prefab:7f321b26431a4974a82deefc368adf63", "TUTORIAL01_HOGGER_04", Notification.SpeechBubbleDirection.TopRight, hoggerActor));
        if ((bool) UniversalInputManager.UsePhoneUI)
          Gameplay.Get().AddGamePlayNameBannerPhone();
        if (!SoundUtils.CanDetectVolume())
        {
          innkeeperLine = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_03"), "", 15f);
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_03.prefab:f47d0faf9067b3341bb9adb38f90be5b", "", Notification.SpeechBubbleDirection.None, (Actor) null));
          NotificationManager.Get().DestroyNotification(innkeeperLine, 0.0f);
        }
        else
          yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_03.prefab:f47d0faf9067b3341bb9adb38f90be5b", "", Notification.SpeechBubbleDirection.None, (Actor) null));
        yield return (object) Gameplay.Get().StartCoroutine(tutorial01.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_04.prefab:e6fb72da1414d454f9d96a51c7009a82", "", Notification.SpeechBubbleDirection.None, (Actor) null));
        tutorial01.announcerIsFinishedYapping = true;
        goto case 6;
      default:
        Debug.LogWarning((object) "WARNING - Mission fired an event that we are not listening for.");
        goto case 6;
    }
  }

  private void ShowAttackWithYourMinionPopup()
  {
    if ((Object) this.attackWithYourMinion != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.attackWithYourMinion);
    if (this.firstAttackFinished || (Object) this.firstMurlocCard == (Object) null)
      return;
    this.firstMurlocCard.GetActor().ToggleForceIdle(false);
    this.firstMurlocCard.GetActor().SetActorState(ActorStateType.CARD_PLAYABLE);
    Vector3 position1 = this.firstMurlocCard.transform.position;
    if (this.firstMurlocCard.GetEntity().IsExhausted() || !(this.firstMurlocCard.GetZone() is ZonePlay))
      return;
    if ((Object) this.firstRaptorCard != (Object) null && this.firstMurlocCard.GetZonePosition() < this.firstRaptorCard.GetZonePosition())
    {
      Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
      this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), this.textToShowForAttackTip);
      this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
    }
    else
    {
      Vector3 position3 = new Vector3(position1.x + 3f, position1.y, position1.z);
      this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position3, TutorialEntity.GetTextScale(), this.textToShowForAttackTip);
      this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
    }
    this.ShowFadeArrow(this.firstMurlocCard, GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard());
    Gameplay.Get().StartCoroutine(this.SwapHelpTextAndFlashMinion());
  }

  private IEnumerator SwapHelpTextAndFlashMinion()
  {
    if (!((Object) this.firstMurlocCard == (Object) null))
    {
      Gameplay.Get().StartCoroutine(this.BeginFlashingMinionLoop(this.firstMurlocCard));
      yield return (object) new WaitForSeconds(4f);
      if (!(this.textToShowForAttackTip == GameStrings.Get("TUTORIAL01_HELP_10")) && !this.firstMurlocCard.GetEntity().IsExhausted() && this.firstMurlocCard.GetActor().GetActorStateType() != ActorStateType.CARD_IDLE && this.firstMurlocCard.GetActor().GetActorStateType() != ActorStateType.CARD_MOUSE_OVER && this.firstMurlocCard.GetZone() is ZonePlay && !this.firstAttackFinished)
      {
        Vector3 position1 = this.firstMurlocCard.transform.position;
        this.textToShowForAttackTip = GameStrings.Get("TUTORIAL01_HELP_10");
        if ((Object) this.attackWithYourMinion != (Object) null)
          NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.attackWithYourMinion);
        if ((Object) this.firstRaptorCard != (Object) null && this.firstMurlocCard.GetZonePosition() < this.firstRaptorCard.GetZonePosition())
        {
          Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
          this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), this.textToShowForAttackTip);
          this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
        }
        else
        {
          Vector3 position3 = new Vector3(position1.x + 3f, position1.y, position1.z);
          this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position3, TutorialEntity.GetTextScale(), this.textToShowForAttackTip);
          this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        }
      }
    }
  }

  private IEnumerator FlashMinionUntilAttackBegins(Card minionToFlash)
  {
    yield return (object) new WaitForSeconds(8f);
    Gameplay.Get().StartCoroutine(this.BeginFlashingMinionLoop(minionToFlash));
  }

  private IEnumerator BeginFlashingMinionLoop(Card minionToFlash)
  {
    if (!((Object) minionToFlash == (Object) null) && !minionToFlash.GetEntity().IsExhausted() && minionToFlash.GetActor().GetActorStateType() != ActorStateType.CARD_IDLE && minionToFlash.GetActor().GetActorStateType() != ActorStateType.CARD_MOUSE_OVER)
    {
      minionToFlash.GetActorSpell(SpellType.WIGGLE).Activate();
      yield return (object) new WaitForSeconds(1.5f);
      Gameplay.Get().StartCoroutine(this.BeginFlashingMinionLoop(minionToFlash));
    }
  }

  private void ShowEndTurnBouncingArrow()
  {
    if (EndTurnButton.Get().IsInWaitingState())
      return;
    Vector3 position1 = EndTurnButton.Get().transform.position;
    Vector3 position2 = new Vector3(position1.x - 2f, position1.y, position1.z);
    NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, -90f, 0.0f));
  }

  private void ShowDontFireballYourselfPopup(Vector3 origin)
  {
    if ((Object) this.noFireballPopup != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.noFireballPopup);
    Vector3 position = new Vector3(origin.x - 3f, origin.y, origin.z);
    this.noFireballPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_07"));
    NotificationManager.Get().DestroyNotification(this.noFireballPopup, 2.5f);
  }

  public override bool ShouldDoAlternateMulliganIntro() => true;

  public override bool DoAlternateMulliganIntro()
  {
    AssetLoader.Get().InstantiatePrefab((AssetReference) "GameOpen_Pack.prefab:fca6ae094e9a74644b00fc9029f304c3", new PrefabCallback<GameObject>(this.PackLoadedCallback), options: AssetLoadingOptions.IgnorePrefabPosition);
    return true;
  }

  private void PackLoadedCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    MusicManager.Get().StartPlaylist(MusicPlaylistType.Misc_Tutorial01);
    Card heroCard1 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    Card heroCard2 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    this.startingPack = go;
    heroCard1.transform.parent = GameObjectUtils.FindChildBySubstring(this.startingPack, "Hero_Dummy").transform;
    heroCard1.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    heroCard1.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    LayerUtils.SetLayer(heroCard1.GetActor().GetRootObject(), GameLayer.IgnoreFullScreenEffects);
    Transform transform = GameObjectUtils.FindChildBySubstring(this.startingPack, "HeroEnemy_Dummy").transform;
    heroCard2.transform.parent = transform;
    heroCard2.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    heroCard2.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    heroCard1.SetDoNotSort(true);
    Transform bone = Board.Get().FindBone("Tutorial1HeroStart");
    go.transform.position = bone.position;
    heroCard1.GetActor().GetHealthObject().Hide();
    heroCard2.GetActor().GetHealthObject().Hide();
    heroCard2.GetActor().Hide();
    heroCard1.GetActor().Hide();
    SceneMgr.Get().NotifySceneLoaded();
    Gameplay.Get().StartCoroutine(this.UpdatePresence());
    Gameplay.Get().StartCoroutine(this.ShowPackOpeningArrow(bone.position));
  }

  private IEnumerator UpdatePresence()
  {
    while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
      yield return (object) null;
    GameMgr.Get().UpdatePresence();
  }

  private IEnumerator ShowPackOpeningArrow(Vector3 packSpot)
  {
    yield return (object) new WaitForSeconds(4f);
    if (!this.packOpened)
    {
      Vector3 position = new Vector3(packSpot.x + 4.014574f, packSpot.y, packSpot.z + 0.2307634f);
      this.freeCardsPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), GameStrings.Get("TUTORIAL01_HELP_18"));
      this.freeCardsPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
    }
  }

  public override void NotifyOfGamePackOpened()
  {
    this.packOpened = true;
    if (!((Object) this.freeCardsPopup != (Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.freeCardsPopup);
  }

  public override bool IsCustomIntroFinished() => this.m_customIntroFinished;

  public override void NotifyOfCustomIntroFinished()
  {
    Card heroCard1 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    Card heroCard2 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(Player.Side.FRIENDLY);
    if ((Object) zoneOfType != (Object) null)
    {
      zoneOfType.ShowTradeableGlow();
      zoneOfType.HideTradeableGlow();
    }
    heroCard1.SetDoNotSort(false);
    heroCard2.GetActor().TurnOnCollider();
    heroCard1.GetActor().TurnOnCollider();
    heroCard1.transform.parent = (Transform) null;
    heroCard2.transform.parent = (Transform) null;
    LayerUtils.SetLayer(heroCard1.GetActor().GetRootObject(), GameLayer.CardRaycast);
    Gameplay.Get().StartCoroutine(this.ContinueFinishingCustomIntro());
  }

  private IEnumerator ContinueFinishingCustomIntro()
  {
    yield return (object) new WaitForSeconds(3f);
    Object.Destroy((Object) this.startingPack);
    GameState.Get().SetBusy(false);
    MulliganManager.Get().SkipMulligan();
    this.m_customIntroFinished = true;
  }

  public override bool ShouldShowBigCard() => this.GetTag(GAME_TAG.TURN) > 8;

  public override void NotifyOfDefeatCoinAnimation() => this.PlaySound("VO_TUTORIAL_01_JAINA_13_10.prefab:b13670e36c248e141837c4eb0645a000");

  public override List<RewardData> GetCustomRewards()
  {
    List<RewardData> customRewards = new List<RewardData>();
    CardRewardData cardRewardData = new CardRewardData("CS2_023", TAG_PREMIUM.NORMAL, 2);
    cardRewardData.MarkAsDummyReward();
    customRewards.Add((RewardData) cardRewardData);
    return customRewards;
  }
}
