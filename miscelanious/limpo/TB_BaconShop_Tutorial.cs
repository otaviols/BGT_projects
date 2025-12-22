using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TB_BaconShop_Tutorial : TB_BaconShop
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TB_BaconShop_Tutorial.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TB_BaconShop_Tutorial.InitStringOptions();
  private static readonly AssetReference Bob_BrassRing_Quote = new AssetReference("Bob_BrassRing_Quote.prefab:89385ff7d67aa1e49bcf25bc15ca61f6");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterFreezing_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterFreezing_01.prefab:4dc4f16c60d79ed40be28f898346df02");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterSelling_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterSelling_01.prefab:d71f34687d09a064bab5d202ea3fb965");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_01.prefab:d3ad51eb14e20324387e5dbbd1e82811");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_03 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_03.prefab:03260a54e677e4247aa19eb29662371e");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_04 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_04.prefab:ecf9f68fe25195a4a93b50d0c8e82a1a");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterTriple_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterTriple_01.prefab:dc92cab5423afa045b4ad528dd25f9d5");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterTriple_02 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterTriple_02.prefab:d845164292fb45a4f85eed478ad5d1c2");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_AfterTriple_03 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_AfterTriple_03.prefab:4704869d519d6e7479602dd15e12b175");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_CombatWin_03 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_CombatWin_03.prefab:8ff8566f08747ad4bb76409e6db1504b");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_FirstBattle_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_FirstBattle_01.prefab:b88923936df527147b6eda2517ce91ef");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_FirstVictory_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_FirstVictory_01.prefab:e40b154f86185d3428ffa48867241f76");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_General_02 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_General_02.prefab:d5908d1fd355b8c4b8344e300dc4fc42");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_HeroSelection_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_HeroSelection_01.prefab:93cd3efc86126de478be0e56c8e275a7");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Hire_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Hire_01.prefab:bfd9513b46b92e84da5f22e01a0387a4");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Hire_02 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Hire_02.prefab:eb20d844bee8bdf4f9cbb514c8ab8580");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Idle_02 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Idle_02.prefab:3808bb035b74ac04f9bb4be91009e2b7");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Idle_03 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Idle_03.prefab:34248aac29c16274c95fb999635368ff");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_ModeSelect_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_ModeSelect_01.prefab:261a9714c4cf3ad4d8944d9127a38ddf");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_RecruitMediumMinion_03 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_RecruitMediumMinion_03.prefab:e3cbf2a35ac2e8245b5bb3de3baa054e");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_RecruitSmallMinion_02 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_RecruitSmallMinion_02.prefab:eb000d8de28cd6d478b9a718ebe1fd9e");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_RecruitWork_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_RecruitWork_01.prefab:a5e1a6db102be6d4495aa1cd7dc7ddfc");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_ShopFirstTime_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_ShopFirstTime_01.prefab:8070938a2c3ba2f4ea92b7f0b5fdf280");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_ShopUpgrade_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_ShopUpgrade_01.prefab:f5019f07757dde341aae503b53a9102e");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Triple_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Triple_01.prefab:26a5500e887280c40a810c01741e2544");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Triple_02 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Triple_02.prefab:1aff064425948044791b8b9e3f8de61b");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Triple_03 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Triple_03.prefab:e14f16322b47b814d8ccb07a60ccf6d1");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_UpgradeShop_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_UpgradeShop_01.prefab:ec1459e08d9b5a04e97c6a3499505cf6");
  protected Notification m_dragBuyTutorialNotification;
  protected Notification m_recruitReminderTutorialNofification;
  protected Notification m_refreshButtonTutorialNotification;
  protected Notification m_minionMoveTutorialNotification;
  protected Notification m_upgradeTavernTutorialNotification;
  protected Notification m_dragSellTutorialNotification;
  protected Notification m_freezeTutorialNotification;
  protected Notification m_popupTutorialNotification;
  protected Notification m_manaNotifier;
  protected Notification m_handBounceArrow;
  protected bool m_shouldPlayMinionMoveTutorial = true;
  protected bool m_shouldShowHandBounceArrow;
  private static readonly AssetReference BaconTutorialPopup = new AssetReference("BaconTutorialPopup.prefab:b68a7306f3300874a833909005fa797d");
  private static readonly AssetReference DRAGBUY_DIALOG_TUTORIAL_PREFAB = TB_BaconShop_Tutorial.BaconTutorialPopup;
  private static readonly AssetReference DRAGSELL_DIALOG_TUTORIAL_PREFAB = TB_BaconShop_Tutorial.BaconTutorialPopup;
  private static readonly AssetReference TRIPLE_DIALOG_TUTORIAL_PREFAB = TB_BaconShop_Tutorial.BaconTutorialPopup;
  private static readonly AssetReference COMBAT_DIALOG_TUTORIAL_PREFAB = TB_BaconShop_Tutorial.BaconTutorialPopup;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.MULLIGAN_HAS_HERO_LOBBY,
      false
    },
    {
      GameEntityOption.WAIT_FOR_RATING_INFO,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  protected override string GetFavoriteBattlegroundsGuideSkinCardId() => "TB_BaconShopBob";

  public TB_BaconShop_Tutorial()
  {
    this.m_gameOptions.AddOptions(TB_BaconShop_Tutorial.s_booleanOptions, TB_BaconShop_Tutorial.s_stringOptions);
    HistoryManager.Get().DisableHistory();
    PlayerLeaderboardManager.Get().SetEnabled(true);
    PlayerLeaderboardManager.Get().SetAllowFakePlayers(true);
    EndTurnButton.Get().SetDisabled(true);
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    this.InitializeTurnTimer();
    this.m_gamePhase = 1;
    GameEntity.Coroutines.StartCoroutine(this.OnShopPhase(false));
  }

  public override void OnDecommissionGame()
  {
    this.HideShopTutorials();
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    base.OnDecommissionGame();
  }

  private void OnGameplaySceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.GAMEPLAY)
      return;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    ManaCrystalMgr.Get().SetEnemyManaCounterActive(false);
    if (GameMgr.Get().IsSpectator())
      this.InitializeFakeHeroLeaderboard();
    this.HideShopTutorials();
    GameEntity.Coroutines.StartCoroutine(this.OnReconnect());
  }

  protected override List<string> SoundFilesForPreload()
  {
    List<string> stringList = base.SoundFilesForPreload();
    stringList.AddRange((IEnumerable<string>) new List<string>()
    {
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterFreezing_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterSelling_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_03,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_04,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterTriple_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterTriple_02,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterTriple_03,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_CombatWin_03,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_FirstBattle_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_FirstVictory_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_General_02,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_HeroSelection_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Hire_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Hire_02,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Idle_02,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Idle_03,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_ModeSelect_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_RecruitMediumMinion_03,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_RecruitSmallMinion_02,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_RecruitWork_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_ShopFirstTime_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_ShopUpgrade_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Triple_01,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Triple_02,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Triple_03,
      (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_UpgradeShop_01
    });
    return stringList;
  }

  public override InputManager.ZoneTooltipSettings GetZoneTooltipSettings() => new InputManager.ZoneTooltipSettings()
  {
    EnemyDeck = new InputManager.TooltipSettings(false),
    EnemyHand = new InputManager.TooltipSettings(false),
    EnemyMana = new InputManager.TooltipSettings(false),
    FriendlyDeck = new InputManager.TooltipSettings(false),
    FriendlyMana = new InputManager.TooltipSettings(true, new InputManager.TooltipContentDelegate(((TB_BaconShop) this).GetFriendlyManaTooltipContent))
  };

  public override bool ShouldDoAlternateMulliganIntro() => true;

  public override bool DoAlternateMulliganIntro()
  {
    if (!this.ShouldDoAlternateMulliganIntro())
      return false;
    GameEntity.Coroutines.StartCoroutine(this.SkipStandardMulliganWithTiming());
    return true;
  }

  protected override IEnumerator OnShopPhase(bool expectStateChangeCallback)
  {
    TB_BaconShop_Tutorial baconShopTutorial = this;
    yield return (object) baconShopTutorial.ShowPopup("Shop", false);
    PlayerLeaderboardManager.Get().UpdateLayout();
    GameState.Get().GetOpposingSidePlayer().UpdateDisplayInfo();
    baconShopTutorial.UpdateNameBanner();
    baconShopTutorial.ShowTechLevelDisplay(true);
    int tag = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.RESOURCES);
    TurnStartManager.Get().NotifyOfManaCrystalFilled(tag);
    yield return (object) new WaitForSeconds(3f);
  }

  protected override IEnumerator OnCombatPhase()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_BaconShop_Tutorial baconShopTutorial = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      baconShopTutorial.ShowTechLevelDisplay(false);
      GameState.Get().GetOpposingSidePlayer().UpdateDisplayInfo();
      baconShopTutorial.UpdateNameBanner();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    baconShopTutorial.HideShopTutorials();
    BaconBoard.Get().OnBoardSkinChosen(1);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) baconShopTutorial.ShowPopup("Combat", false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator OnReconnect()
  {
    TB_BaconShop_Tutorial baconShopTutorial = this;
    if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.TURN) <= 12)
    {
      baconShopTutorial.SetInputEnableForAllButtons(false);
      baconShopTutorial.SetInputEnableForAllCards(false);
    }
    baconShopTutorial.HideShopTutorials();
    yield return (object) new WaitForSeconds(3f);
    int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.MISSION_EVENT);
    if (GameMgr.Get().IsSpectator())
    {
      if (tag == 1 || tag == 2 || tag == 5)
        GameEntity.Coroutines.StartCoroutine(baconShopTutorial.HandleMissionEventWithTiming(tag));
    }
    else
      GameEntity.Coroutines.StartCoroutine(baconShopTutorial.HandleMissionEventWithTiming(tag));
    yield return (object) null;
  }

  protected void InitializeFakeHeroLeaderboard()
  {
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetGraveyardZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetCardType() == TAG_CARDTYPE.HERO)
        PlayerLeaderboardManager.Get().CreatePlayerTile(entity);
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_BaconShop_Tutorial baconShopTutorial = this;
    switch (missionEvent)
    {
      case 1:
        baconShopTutorial.m_gamePhase = 1;
        yield return (object) baconShopTutorial.OnShopPhase(true);
        break;
      case 2:
        baconShopTutorial.m_gamePhase = 2;
        yield return (object) baconShopTutorial.OnCombatPhase();
        break;
      case 5:
        GameState.Get().GetOpposingSidePlayer().UpdateDisplayInfo();
        baconShopTutorial.UpdateNameBanner();
        break;
    }
    if (!GameMgr.Get().IsSpectator())
    {
      switch (missionEvent)
      {
        case 3:
          if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.TURN) == 9)
            break;
          baconShopTutorial.SetInputEnableForFrozenButton(false);
          yield return (object) new WaitForSeconds(0.75f);
          baconShopTutorial.SetInputEnableForFrozenButton(true);
          break;
        case 4:
          baconShopTutorial.SetInputEnableForRefreshButton(false);
          yield return (object) new WaitForSeconds(0.75f);
          baconShopTutorial.SetInputEnableForRefreshButton(true);
          break;
        case 10:
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_ModeSelect_01);
          baconShopTutorial.InitializeFakeHeroLeaderboard();
          yield return (object) new WaitForSeconds(0.5f);
          GameState.Get().SetBusy(false);
          PlayerLeaderboardManager.Get().UpdateLayout();
          break;
        case 11:
          GameState.Get().SetBusy(true);
          baconShopTutorial.CreateTutorialDialog(TB_BaconShop_Tutorial.DRAGBUY_DIALOG_TUTORIAL_PREFAB, "GAMEPLAY_BACON_DRAGBUY_TITLE_TUTORIAL", "GAMEPLAY_BACON_DRAGBUY_BODY_TUTORIAL", "GAMEPLAY_BACON_CONFIRM_BUTTON_TUTORIAL", new UIEvent.Handler(baconShopTutorial.UserPressedDragBuyTutorial), new Vector2(0.5f, 0.5f));
          break;
        case 12:
          yield return (object) new WaitForSeconds(1f);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_ShopFirstTime_01);
          GameState.Get().SetBusy(false);
          break;
        case 13:
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Hire_01);
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForAllCards(true);
          Card cardInOpposingPlay1 = baconShopTutorial.GetCardInOpposingPlay("CS2_065");
          if (!((Object) cardInOpposingPlay1 != (Object) null))
            break;
          baconShopTutorial.ShowDragBuyTutorial(cardInOpposingPlay1, "GAMEPLAY_BACON_DRAGBUY_TUTORIAL");
          GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideDragBuyTutorial("GAMEPLAY_BACON_DRAGBUY_TUTORIAL"));
          break;
        case 14:
          baconShopTutorial.SetInputEnableForAllCards(false);
          baconShopTutorial.HideNotification(baconShopTutorial.m_dragBuyTutorialNotification);
          yield return (object) baconShopTutorial.ShowManaArrowWithText("GAMEPLAY_BACON_COIN_TUTORIAL_1");
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_RecruitWork_01);
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) new WaitForSeconds(3f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 15:
          baconShopTutorial.HideHandBounceArrow();
          GameState.Get().SetBusy(true);
          baconShopTutorial.HideNotification(baconShopTutorial.m_dragBuyTutorialNotification);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_HeroSelection_01);
          GameState.Get().SetBusy(false);
          break;
        case 20:
          baconShopTutorial.SetInputEnableForAllButtons(false);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_General_02);
          yield return (object) new WaitForSeconds(0.25f);
          baconShopTutorial.RecruitReminderTutorial();
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForBuy(true);
          break;
        case 22:
          baconShopTutorial.m_shouldShowHandBounceArrow = false;
          baconShopTutorial.SetInputEnableForAllCards(false);
          baconShopTutorial.HideNotification(baconShopTutorial.m_recruitReminderTutorialNofification);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Idle_02);
          yield return (object) baconShopTutorial.ShowManaArrowWithText("GAMEPLAY_BACON_COIN_TUTORIAL_2");
          yield return (object) new WaitForSeconds(0.5f);
          baconShopTutorial.SetInputEnableForAllCards(true);
          GameState.Get().SetBusy(false);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_RecruitSmallMinion_02);
          yield return (object) new WaitForSeconds(2.5f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 24:
          baconShopTutorial.HideHandBounceArrow();
          baconShopTutorial.SetInputEnableForAllCards(true);
          GameState.Get().SetBusy(true);
          baconShopTutorial.ShowMinionMoveTutorial();
          GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideMoveMinionTutorial());
          GameState.Get().SetBusy(false);
          break;
        case 25:
          baconShopTutorial.m_shouldPlayMinionMoveTutorial = false;
          baconShopTutorial.HideNotification(baconShopTutorial.m_minionMoveTutorialNotification);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_FirstBattle_01);
          GameState.Get().SetBusy(false);
          break;
        case 30:
          baconShopTutorial.SetInputEnableForAllButtons(false);
          baconShopTutorial.SetInputEnableForAllCards(false);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_ShopUpgrade_01);
          baconShopTutorial.SetInputEnableForTavernUpgradeButton(true);
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForAllCards(false);
          yield return (object) new WaitForSeconds(0.5f);
          baconShopTutorial.ShowTavernUpgradeButtonTutorial();
          break;
        case 31:
          baconShopTutorial.SetInputEnableForRefreshButton(false);
          GameState.Get().SetBusy(true);
          baconShopTutorial.HideNotification(baconShopTutorial.m_upgradeTavernTutorialNotification);
          baconShopTutorial.SetInputEnableForBuy(false);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_03);
          GameState.Get().SetBusy(false);
          baconShopTutorial.ShowRefreshButtonTutorial();
          baconShopTutorial.SetInputEnableForRefreshButton(true);
          break;
        case 32:
          baconShopTutorial.SetInputEnableForRefreshButton(false);
          baconShopTutorial.HideNotification(baconShopTutorial.m_refreshButtonTutorialNotification);
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Hire_02);
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForBuy(true);
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) new WaitForSeconds(5f);
          baconShopTutorial.RecruitTutorialWithBoardSize(4);
          GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideRecruitTutorialWithBoardSize(4));
          break;
        case 33:
          baconShopTutorial.SetInputEnableForBuy(true);
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) new WaitForSeconds(5f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 34:
          baconShopTutorial.HideHandBounceArrow();
          baconShopTutorial.HideNotification(baconShopTutorial.m_dragBuyTutorialNotification);
          baconShopTutorial.HideNotification(baconShopTutorial.m_refreshButtonTutorialNotification);
          baconShopTutorial.SetInputEnableForRefreshButton(false);
          break;
        case 40:
          baconShopTutorial.SetInputEnableForAllButtons(false);
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Triple_02);
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) new WaitForSeconds(5f);
          baconShopTutorial.RecruitTutorialWithBoardSize(2);
          GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideRecruitTutorialWithBoardSize(2));
          break;
        case 41:
          baconShopTutorial.SetInputEnableForAllCards(false);
          baconShopTutorial.SetInputEnableForAllButtons(false);
          GameState.Get().SetBusy(true);
          baconShopTutorial.CreateTutorialDialog(TB_BaconShop_Tutorial.TRIPLE_DIALOG_TUTORIAL_PREFAB, "GAMEPLAY_BACON_TRIPLE_TITLE_TUTORIAL", "GAMEPLAY_BACON_TRIPLE_BODY_TUTORIAL", "GAMEPLAY_BACON_CONFIRM_BUTTON_TUTORIAL", new UIEvent.Handler(baconShopTutorial.UserPressedTripleTutorial), new Vector2(0.5f, 0.0f));
          break;
        case 42:
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Triple_01);
          Card cardInOpposingPlay2 = baconShopTutorial.GetCardInOpposingPlay("CS2_065");
          if ((Object) cardInOpposingPlay2 != (Object) null)
          {
            cardInOpposingPlay2.SetInputEnabled(true);
            baconShopTutorial.ShowDragBuyTutorial(cardInOpposingPlay2, "GAMEPLAY_BACON_DRAGBUY_TRIPLE_TUTORIAL");
            GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideDragBuyTutorial("GAMEPLAY_BACON_DRAGBUY_TRIPLE_TUTORIAL"));
          }
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForBuy(true);
          break;
        case 44:
          baconShopTutorial.HideNotification(baconShopTutorial.m_dragBuyTutorialNotification);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterTriple_01);
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 45:
          baconShopTutorial.HideHandBounceArrow();
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Triple_03);
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 46:
          baconShopTutorial.HideHandBounceArrow();
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 47:
          baconShopTutorial.HideHandBounceArrow();
          baconShopTutorial.SetInputEnableForAllButtons(true);
          baconShopTutorial.SetInputEnableForAllCards(true);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_RecruitMediumMinion_03);
          GameState.Get().SetBusy(false);
          break;
        case 51:
          baconShopTutorial.SetInputEnableForAllButtons(false);
          baconShopTutorial.SetInputEnableForAllCards(false);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_UpgradeShop_01);
          yield return (object) new WaitForSeconds(0.5f);
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForAllCards(false);
          baconShopTutorial.SetInputEnableForTavernUpgradeButton(true);
          baconShopTutorial.ShowTavernUpgradeButtonTutorial();
          break;
        case 52:
          GameState.Get().SetBusy(true);
          baconShopTutorial.HideNotification(baconShopTutorial.m_upgradeTavernTutorialNotification);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_04);
          yield return (object) new WaitForSeconds(0.5f);
          GameState.Get().SetBusy(false);
          baconShopTutorial.ShowRefreshButtonTutorial("GAMEPLAY_BACON_REFRESH_UPGRADE_TUTORIAL");
          baconShopTutorial.SetInputEnableForRefreshButton(true);
          break;
        case 53:
          baconShopTutorial.SetInputEnableForRefreshButton(false);
          baconShopTutorial.HideNotification(baconShopTutorial.m_refreshButtonTutorialNotification);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterShopUpgrade_01);
          yield return (object) new WaitForSeconds(0.5f);
          baconShopTutorial.SetInputEnableForFrozenButton(true);
          baconShopTutorial.ShowFreezeTutorial();
          break;
        case 54:
          baconShopTutorial.m_shouldShowHandBounceArrow = false;
          baconShopTutorial.SetInputEnableForFrozenButton(false);
          baconShopTutorial.HideNotification(baconShopTutorial.m_freezeTutorialNotification);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterFreezing_01);
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForAllCards(true);
          break;
        case 60:
          baconShopTutorial.SetInputEnableForAllButtons(false);
          baconShopTutorial.SetInputEnableForBuy(true);
          GameState.Get().SetBusy(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterTriple_02);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Hire_01);
          GameState.Get().SetBusy(false);
          yield return (object) new WaitForSeconds(2f);
          baconShopTutorial.RecruitTutorialWithBoardSize(4);
          GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideRecruitTutorialWithBoardSize(4));
          break;
        case 61:
          baconShopTutorial.SetInputEnableForFriendlyHandCards(false);
          baconShopTutorial.SetInputEnableForBuy(true);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.RecruitTutorialWithBoardSize(3);
          GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideRecruitTutorialWithBoardSize(3));
          break;
        case 62:
          baconShopTutorial.SetInputEnableForAllButtons(false);
          baconShopTutorial.SetInputEnableForFriendlyHandCards(false);
          GameState.Get().SetBusy(true);
          baconShopTutorial.CreateTutorialDialog(TB_BaconShop_Tutorial.DRAGSELL_DIALOG_TUTORIAL_PREFAB, "GAMEPLAY_BACON_DRAGSELL_TITLE_TUTORIAL", "GAMEPLAY_BACON_DRAGSELL_BODY_TUTORIAL", "GAMEPLAY_BACON_CONFIRM_BUTTON_TUTORIAL", new UIEvent.Handler(baconShopTutorial.UserPressedDragSellTutorial), new Vector2(0.0f, 0.5f));
          break;
        case 63:
          GameState.Get().SetBusy(false);
          baconShopTutorial.SetInputEnableForAllCards(true);
          baconShopTutorial.SetInputEnableForFriendlyHandCards(false);
          baconShopTutorial.ShowDragSellTutorial();
          break;
        case 64:
          baconShopTutorial.SetInputEnableForBuy(true);
          GameState.Get().SetBusy(true);
          baconShopTutorial.HideNotification(baconShopTutorial.m_dragSellTutorialNotification);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterSelling_01);
          GameState.Get().SetBusy(false);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.RecruitTutorialWithBoardSize(2);
          GameEntity.Coroutines.StartCoroutine(baconShopTutorial.ShowOrHideRecruitTutorialWithBoardSize(2));
          break;
        case 65:
          baconShopTutorial.HideNotification(baconShopTutorial.m_handBounceArrow);
          baconShopTutorial.SetInputEnableForFriendlyHandCards(true);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 66:
          baconShopTutorial.HideHandBounceArrow();
          baconShopTutorial.SetInputEnableForFriendlyHandCards(true);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 67:
          baconShopTutorial.HideHandBounceArrow();
          baconShopTutorial.SetInputEnableForFriendlyHandCards(true);
          yield return (object) new WaitForSeconds(6f);
          baconShopTutorial.ShowHandBounceArrow();
          break;
        case 68:
          baconShopTutorial.HideHandBounceArrow();
          baconShopTutorial.SetInputEnableForAllButtons(true);
          baconShopTutorial.SetInputEnableForAllCards(true);
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_AfterTriple_03);
          break;
        case 70:
          baconShopTutorial.HideHandBounceArrow();
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_CombatWin_03);
          break;
        case 79:
          baconShopTutorial.HideHandBounceArrow();
          GameState.Get().SetBusy(true);
          baconShopTutorial.CreateTutorialDialog(TB_BaconShop_Tutorial.COMBAT_DIALOG_TUTORIAL_PREFAB, "GAMEPLAY_BACON_COMBAT_TITLE_TUTORIAL", "GAMEPLAY_BACON_COMBAT_BODY_TUTORIAL", "GAMEPLAY_BACON_CONFIRM_BUTTON_TUTORIAL", new UIEvent.Handler(baconShopTutorial.UserPressedCombatTutorial), Vector2.zero);
          break;
        case 80:
          baconShopTutorial.HideHandBounceArrow();
          yield return (object) baconShopTutorial.PlayBobLine((string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_Idle_03);
          break;
      }
    }
  }

  public override void OnPlayThinkEmote()
  {
    if (!this.HasSeenAllTutorial() || this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide())
      return;
    currentPlayer.GetHeroCard().HasActiveEmoteSound();
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    TB_BaconShop_Tutorial baconShopTutorial = this;
    baconShopTutorial.HideShopTutorials();
    PlayerLeaderboardManager.Get().UpdateLayout();
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.WAIT_FOR_RATING_INFO))
        yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(baconShopTutorial.PlayBigCharacterQuoteAndWait((string) TB_BaconShop_Tutorial.Bob_BrassRing_Quote, (string) TB_BaconShop_Tutorial.VO_DALA_BOSS_99h_Male_Human_FirstVictory_01));
    }
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    this.HideShopTutorials();
    GameEntity.Coroutines.StartCoroutine(this.HandleGameOverWithTiming(gameResult));
    if (gameResult == TAG_PLAYSTATE.WON)
      base.NotifyOfGameOver(gameResult);
    if (gameResult != TAG_PLAYSTATE.LOST)
      return;
    PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
    Network.Get().DisconnectFromGameServer();
    SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
    GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
    SceneMgr.Get().SetNextMode(postGameSceneMode);
  }

  protected IEnumerator PlayBobLine(string voLine)
  {
    TB_BaconShop_Tutorial baconShopTutorial = this;
    Actor bobActor = baconShopTutorial.GetBobActor();
    if ((Object) bobActor != (Object) null && bobActor.GetEntity() != null)
    {
      string legacyAssetName = new AssetReference(voLine).GetLegacyAssetName();
      yield return (object) baconShopTutorial.PlaySoundAndWait(voLine, legacyAssetName, Notification.SpeechBubbleDirection.TopRight, bobActor);
    }
  }

  public override string GetNameBannerOverride(Player.Side side)
  {
    if (side != Player.Side.OPPOSING)
      return (string) null;
    if (GameState.Get() == null)
      return (string) null;
    if (GameState.Get().GetOpposingSidePlayer() == null)
      return (string) null;
    return GameState.Get().GetOpposingSidePlayer().GetHero() == null ? (string) null : GameState.Get().GetOpposingSidePlayer().GetHero().GetName();
  }

  protected new void InitializeTurnTimer() => TurnTimer.Get().SetGameModeSettings(new TurnTimerGameModeSettings()
  {
    m_RopeFuseVolume = 0.05f,
    m_EndTurnButtonExplosionVolume = 0.0f,
    m_RopeRolloutVolume = 0.3f,
    m_PlayMusicStinger = false,
    m_PlayTimeoutFx = false,
    m_PlayTickSound = false
  });

  protected void UserPressedDragBuyTutorial(UIEvent e) => this.HandleMissionEvent(12);

  protected void UserPressedTripleTutorial(UIEvent e) => this.HandleMissionEvent(42);

  protected void UserPressedDragSellTutorial(UIEvent e) => this.HandleMissionEvent(63);

  protected void UserPressedCombatTutorial(UIEvent e) => GameState.Get().SetBusy(false);

  protected override void HideShopTutorials()
  {
    this.HideHandBounceArrow();
    NotificationManager.Get().DestroyAllPopUps();
  }

  protected void SetInputEnableForAllButtons(bool isEnabled)
  {
    this.SetInputEnableForBuy(isEnabled);
    this.SetInputEnableForRefreshButton(isEnabled);
    this.SetInputEnableForTavernUpgradeButton(isEnabled);
    this.SetInputEnableForFrozenButton(isEnabled);
  }

  protected void SetInputEnableForAllCards(bool isEnabled)
  {
    List<Card> cards1 = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards();
    List<Card> cards2 = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards();
    List<Card> cards3 = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    List<Card> second = cards2;
    foreach (Card card in cards1.Concat<Card>((IEnumerable<Card>) second).Concat<Card>((IEnumerable<Card>) cards3).ToList<Card>())
      card.SetInputEnabled(isEnabled);
  }

  protected void SetInputEnableForFriendlyHandCards(bool isEnabled)
  {
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards())
      card.SetInputEnabled(isEnabled);
  }

  public TutorialNotification CreateTutorialDialog(
    AssetReference assetPrefab,
    string headlineGameString,
    string bodyTextGameString,
    string buttonGameString,
    UIEvent.Handler buttonHandler,
    Vector2 materialOffset)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab(assetPrefab);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogError((object) "Unable to load tutorial dialog TutorialIntroDialog prefab.");
      return (TutorialNotification) null;
    }
    TutorialNotification notification = gameObject.GetComponent<TutorialNotification>();
    if ((Object) notification == (Object) null)
    {
      Debug.LogError((object) "TutorialNotification component does not exist on TutorialIntroDialog prefab.");
      return (TutorialNotification) null;
    }
    TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, OverlayUI.Get().m_heightScale.m_Center);
    if ((bool) UniversalInputManager.UsePhoneUI)
      gameObject.transform.localScale = 1.5f * gameObject.transform.localScale;
    this.m_popupTutorialNotification = (Notification) notification;
    notification.headlineUberText.Text = GameStrings.Get(headlineGameString);
    notification.speechUberText.Text = GameStrings.Get(bodyTextGameString);
    notification.m_ButtonStart.SetText(GameStrings.Get(buttonGameString));
    RendererExtension.GetMaterial((Renderer) notification.artOverlay).mainTextureOffset = materialOffset;
    notification.m_ButtonStart.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
    {
      if (buttonHandler != null)
        buttonHandler(e);
      notification.m_ButtonStart.ClearEventListeners();
      NotificationManager.Get().DestroyNotification((Notification) notification, 0.0f);
    }));
    this.m_popupTutorialNotification.PlayBirth();
    UniversalInputManager.Get().SetGameDialogActive(true);
    return notification;
  }

  protected void HideNotification(Notification notification, bool hideImmediately = false)
  {
    if (!((Object) notification != (Object) null))
      return;
    if (hideImmediately)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(notification);
    else
      NotificationManager.Get().DestroyNotification(notification, 0.0f);
  }

  protected void ShowDragBuyTutorial(Card card, string textID = "GAMEPLAY_BACON_PLAY_MINION_TUTORIAL", bool hideImmediately = false)
  {
    if ((Object) card == (Object) null)
      return;
    Vector3 position1 = card.transform.position;
    Vector3 position2;
    Notification.PopUpArrowDirection direction;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      position2 = new Vector3(position1.x + 0.05f, position1.y, position1.z + 2.9f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    else
    {
      position2 = new Vector3(position1.x, position1.y, position1.z + 2.5f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    this.m_dragBuyTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(textID));
    this.m_dragBuyTutorialNotification.ShowPopUpArrow(direction);
    this.m_dragBuyTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  private IEnumerator ShowOrHideDragBuyTutorial(string textString)
  {
    while (!(bool) (Object) InputManager.Get().GetHeldCard())
      yield return (object) null;
    this.HideNotification(this.m_dragBuyTutorialNotification);
    while ((bool) (Object) InputManager.Get().GetHeldCard())
      yield return (object) null;
    yield return (object) new WaitForSeconds(2f);
    Card cardInOpposingPlay = this.GetCardInOpposingPlay("CS2_065");
    if ((Object) cardInOpposingPlay != (Object) null || (bool) (Object) InputManager.Get().GetHeldCard())
    {
      this.ShowDragBuyTutorial(cardInOpposingPlay, textString);
      GameEntity.Coroutines.StartCoroutine(this.ShowOrHideDragBuyTutorial(textString));
    }
    this.HideNotification(this.m_dragBuyTutorialNotification);
  }

  protected void RecruitTutorialWithBoardSize(
    int enemyBoardSize,
    string textID = "GAMEPLAY_BACON_RECRUIT_REMINDER_TUTORIAL_2",
    bool hideImmediately = false)
  {
    List<Card> cards = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards();
    if (cards.Count != enemyBoardSize)
      return;
    Vector3 position1 = cards[0].transform.position;
    Vector3 position2;
    Notification.PopUpArrowDirection direction;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      position2 = new Vector3(position1.x + 0.05f, position1.y, position1.z + 2.9f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    else
    {
      position2 = new Vector3(position1.x, position1.y, position1.z + 2.5f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    this.m_dragBuyTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(textID));
    this.m_dragBuyTutorialNotification.ShowPopUpArrow(direction);
    this.m_dragBuyTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  private IEnumerator ShowOrHideRecruitTutorialWithBoardSize(
    int enemyBoardSize,
    string textID = "GAMEPLAY_BACON_RECRUIT_REMINDER_TUTORIAL_2")
  {
    while (!(bool) (Object) InputManager.Get().GetHeldCard())
      yield return (object) null;
    this.HideNotification(this.m_dragBuyTutorialNotification);
    while ((bool) (Object) InputManager.Get().GetHeldCard())
      yield return (object) null;
    yield return (object) new WaitForSeconds(2f);
    List<Card> cards = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards();
    if (cards.Count == enemyBoardSize && ((Object) cards[0] != (Object) null || (bool) (Object) InputManager.Get().GetHeldCard()))
    {
      this.RecruitTutorialWithBoardSize(enemyBoardSize, textID);
      GameEntity.Coroutines.StartCoroutine(this.ShowOrHideRecruitTutorialWithBoardSize(enemyBoardSize, textID));
    }
  }

  protected void RecruitReminderTutorial()
  {
    if (GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards().Count < 3)
      return;
    Vector3 position = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().transform.position + new Vector3(0.0f, 0.0f, 2.25f);
    string key = "GAMEPLAY_BACON_RECRUIT_REMINDER_TUTORIAL";
    this.m_recruitReminderTutorialNofification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    this.m_recruitReminderTutorialNofification.ShowPopUpArrow(Notification.PopUpArrowDirection.BottomThree);
    this.m_recruitReminderTutorialNofification.PulseReminderEveryXSeconds(2f);
  }

  protected void ShowRefreshButtonTutorial(string textID = "GAMEPLAY_BACON_REFRESH_TUTORIAL", bool hideImmediately = false)
  {
    List<Zone> zonesForSide = ZoneMgr.Get().FindZonesForSide(Player.Side.FRIENDLY);
    Zone zone1 = (Zone) null;
    foreach (Zone zone2 in zonesForSide)
    {
      if (zone2 is ZoneGameModeButton && ((ZoneGameModeButton) zone2).m_ButtonSlot == 2)
        zone1 = zone2;
    }
    Vector3 position1 = zone1.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z - 2.25f) : new Vector3(position1.x, position1.y, position1.z - 2.5f);
    this.m_refreshButtonTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(textID));
    this.m_refreshButtonTutorialNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
    this.m_refreshButtonTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  protected void ShowTavernUpgradeButtonTutorial(bool hideImmediately = false)
  {
    List<Zone> zonesForSide = ZoneMgr.Get().FindZonesForSide(Player.Side.FRIENDLY);
    Zone zone1 = (Zone) null;
    foreach (Zone zone2 in zonesForSide)
    {
      if (zone2 is ZoneGameModeButton && ((ZoneGameModeButton) zone2).m_ButtonSlot == 3)
        zone1 = zone2;
    }
    Vector3 position1 = zone1.transform.position;
    Vector3 position2 = new Vector3(position1.x, position1.y, position1.z - 2.25f);
    string key = "GAMEPLAY_BACON_MINION_UPGRADE_TAVERN_TUTORIAL";
    this.m_upgradeTavernTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    this.m_upgradeTavernTutorialNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
    this.m_upgradeTavernTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  protected void ShowMinionMoveTutorial()
  {
    Card minionInFriendlyPlay = this.GetLeftMostMinionInFriendlyPlay();
    if ((Object) minionInFriendlyPlay == (Object) null)
      return;
    Vector3 position1 = minionInFriendlyPlay.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z + 2.5f) : new Vector3(position1.x + 0.05f, position1.y, position1.z + 2.6f);
    string key = "GAMEPLAY_BACON_MINION_MOVE_TUTORIAL";
    this.m_minionMoveTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    this.m_minionMoveTutorialNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
    this.m_minionMoveTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  private IEnumerator ShowOrHideMoveMinionTutorial()
  {
    while (!(bool) (Object) InputManager.Get().GetHeldCard())
      yield return (object) null;
    this.HideNotification(this.m_minionMoveTutorialNotification);
    while ((bool) (Object) InputManager.Get().GetHeldCard())
      yield return (object) null;
    yield return (object) new WaitForSeconds(2f);
    if (((Object) this.GetLeftMostMinionInFriendlyPlay() != (Object) null || (bool) (Object) InputManager.Get().GetHeldCard()) && this.m_shouldPlayMinionMoveTutorial)
    {
      this.ShowMinionMoveTutorial();
      GameEntity.Coroutines.StartCoroutine(this.ShowOrHideMoveMinionTutorial());
    }
  }

  protected void ShowDragSellTutorial(bool hideImmediately = false)
  {
    Card card = this.GetBobActor().GetCard();
    if ((Object) card == (Object) null)
      return;
    card.GetActor().SetActorState(ActorStateType.CARD_SELECTABLE);
    Vector3 position1 = card.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x - 3.2f, position1.y, position1.z) : new Vector3(position1.x - 3.3f, position1.y, position1.z - 0.0f);
    string key = "GAMEPLAY_BACON_DRAGSELL_TUTORIAL";
    this.m_dragSellTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    this.m_dragSellTutorialNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
    this.m_dragSellTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  protected void ShowFreezeTutorial(bool hideImmediately = false)
  {
    List<Zone> zonesForSide = ZoneMgr.Get().FindZonesForSide(Player.Side.FRIENDLY);
    Zone zone1 = (Zone) null;
    foreach (Zone zone2 in zonesForSide)
    {
      if (zone2 is ZoneGameModeButton && ((ZoneGameModeButton) zone2).m_ButtonSlot == 1)
        zone1 = zone2;
    }
    Vector3 position1 = zone1.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z - 2.1f) : new Vector3(position1.x, position1.y, position1.z - 2.5f);
    string key = "GAMEPLAY_BACON_FREEZE_TUTORIAL";
    this.m_freezeTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    this.m_freezeTutorialNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
    this.m_freezeTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  protected IEnumerator ShowManaArrowWithText(string textID)
  {
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
      position = new Vector3(crystalSpawnPosition.x - 0.02f, crystalSpawnPosition.y + 0.2f, crystalSpawnPosition.z + 1.8f);
      direction = Notification.PopUpArrowDirection.Down;
    }
    this.m_manaNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), GameStrings.Get(textID));
    this.m_manaNotifier.ShowPopUpArrow(direction);
    yield return (object) new WaitForSeconds(2.5f);
    if ((Object) this.m_manaNotifier != (Object) null)
    {
      iTween.PunchScale(this.m_manaNotifier.gameObject, iTween.Hash((object) "amount", (object) new Vector3(1f, 1f, 1f), (object) "time", (object) 1f));
      yield return (object) new WaitForSeconds(2f);
    }
    if ((Object) this.m_manaNotifier != (Object) null)
      NotificationManager.Get().DestroyNotification(this.m_manaNotifier, 0.0f);
  }

  protected void ShowHandBounceArrow()
  {
    this.m_shouldShowHandBounceArrow = true;
    this.HideNotification(this.m_handBounceArrow);
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count == 0)
      return;
    Card card = cards[cards.Count - 1];
    Vector3 position1 = card.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z + 2f) : new Vector3(position1.x - 0.08f, position1.y + 0.2f, position1.z + 1.2f);
    this.m_handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, 0.0f, 0.0f));
    this.m_handBounceArrow.transform.parent = card.transform;
  }

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    if (mousedOverEntity.GetZone() != TAG_ZONE.HAND)
      return;
    this.HideNotification(this.m_handBounceArrow);
  }

  public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
  {
    if (mousedOffEntity.GetZone() != TAG_ZONE.HAND || !this.m_shouldShowHandBounceArrow)
      return;
    Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0.5f));
  }

  protected IEnumerator ShowArrowInSeconds(float seconds)
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

  protected void HideHandBounceArrow()
  {
    this.m_shouldShowHandBounceArrow = false;
    this.HideNotification(this.m_handBounceArrow);
  }

  protected Card GetLeftMostMinionInFriendlyPlay()
  {
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards())
    {
      if (card.GetEntity().GetTag(GAME_TAG.ZONE_POSITION) == 1)
        return card;
    }
    return (Card) null;
  }

  protected Card GetCardInOpposingPlay(string cardId)
  {
    foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
    {
      if (card.GetEntity().GetCardId() == cardId)
        return card;
    }
    return (Card) null;
  }
}
