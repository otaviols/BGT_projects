using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using PegasusClient;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DraftManager : IService
{
  private CollectionDeck m_draftDeck;
  private bool m_hasReceivedSessionWinsLosses;
  private int m_currentSlot;
  private DraftSlotType m_currentSlotType;
  private List<DraftSlotType> m_uniqueDraftSlotTypesForDeck = new List<DraftSlotType>();
  private int m_validSlot;
  private int m_maxSlot;
  private int m_losses;
  private int m_wins;
  private int m_maxWins = int.MaxValue;
  private int m_numTicketsOwned;
  private bool m_isNewKey;
  private bool m_deckActiveDuringSession;
  private Network.RewardChest m_chest;
  private bool m_inRewards;
  private ArenaSession m_currentSession;
  private List<DraftManager.DraftDeckSet> m_draftDeckSetListeners = new List<DraftManager.DraftDeckSet>();
  private bool m_pendingRequestToDisablePremiums;
  private int m_chosenIndex;
  private ArenaSeasonInfo m_currentSeason;
  private static readonly AssetReference DEFAULT_DRAFT_PAPER_TEXTURE = (AssetReference) "Forge_Main_Paper.psd:64b6646e1c591d545885572fccd74259";
  private static readonly AssetReference DEFAULT_DRAFT_PAPER_TEXTURE_PHONE = (AssetReference) "Forge_Main_Paper_phone.psd:ab59053fdba3ebd40bfd6ced4fd246bc";

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DraftManager draftManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new Action(draftManager.WillReset);
    serviceLocator.Get<GameMgr>().RegisterFindGameEvent(new GameMgr.FindGameCallback(draftManager.OnFindGameEvent));
    Network network = serviceLocator.Get<Network>();
    network.RegisterNetHandler((object) ArenaSessionResponse.PacketID.ID, new Network.NetHandler(draftManager.OnArenaSessionResponse));
    network.RegisterNetHandler((object) DraftRewardsAcked.PacketID.ID, new Network.NetHandler(draftManager.OnAckRewards));
    network.RegisterNetHandler((object) DraftError.PacketID.ID, new Network.NetHandler(draftManager.OnError));
    network.RegisterNetHandler((object) DraftRemovePremiumsResponse.PacketID.ID, new Network.NetHandler(draftManager.OnDraftRemovePremiumsResponse));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (Network),
    typeof (GameMgr)
  };

  private void WillReset() => this.ClearDeckInfo();

  public void Shutdown()
  {
  }

  public static DraftManager Get() => ServiceManager.Get<DraftManager>();

  public void OnLoggedIn() => SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));

  public void RegisterDisplayHandlers()
  {
    Network network = Network.Get();
    network.RegisterNetHandler((object) DraftBeginning.PacketID.ID, new Network.NetHandler(this.OnBegin));
    network.RegisterNetHandler((object) PegasusUtil.DraftRetired.PacketID.ID, new Network.NetHandler(this.OnRetire));
    network.RegisterNetHandler((object) PegasusUtil.DraftChoicesAndContents.PacketID.ID, new Network.NetHandler(this.OnChoicesAndContents));
    network.RegisterNetHandler((object) PegasusUtil.DraftChosen.PacketID.ID, new Network.NetHandler(this.OnChosen));
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnDraftPurchaseAck));
    if (!DemoMgr.Get().ArenaIs1WinMode())
      return;
    StoreManager.Get().RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnDraftPurchaseAck));
  }

  public void UnregisterDisplayHandlers()
  {
    Network network = Network.Get();
    network.RemoveNetHandler((object) DraftBeginning.PacketID.ID, new Network.NetHandler(this.OnBegin));
    network.RemoveNetHandler((object) PegasusUtil.DraftRetired.PacketID.ID, new Network.NetHandler(this.OnRetire));
    network.RemoveNetHandler((object) PegasusUtil.DraftChoicesAndContents.PacketID.ID, new Network.NetHandler(this.OnChoicesAndContents));
    network.RemoveNetHandler((object) PegasusUtil.DraftChosen.PacketID.ID, new Network.NetHandler(this.OnChosen));
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnDraftPurchaseAck));
    if (!DemoMgr.Get().ArenaIs1WinMode())
      return;
    StoreManager.Get().RemoveSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnDraftPurchaseAck));
  }

  public void RegisterDraftDeckSetListener(DraftManager.DraftDeckSet dlg) => this.m_draftDeckSetListeners.Add(dlg);

  public void RemoveDraftDeckSetListener(DraftManager.DraftDeckSet dlg) => this.m_draftDeckSetListeners.Remove(dlg);

  public ulong SecondsUntilEndOfSeason => this.m_currentSeason != null ? this.m_currentSeason.Season.GameContentSeason.EndSecondsFromNow : 0UL;

  public int CurrentSeasonId => this.m_currentSeason != null ? this.m_currentSeason.Season.GameContentSeason.SeasonId : 0;

  public bool HasActiveRun => this.m_currentSession != null && this.m_currentSession.HasIsActive && this.m_currentSession.IsActive;

  public int ChosenIndex => this.m_chosenIndex;

  public void RefreshCurrentSeasonFromServer() => Network.Get().SendArenaSessionRequest();

  public CollectionDeck GetDraftDeck() => this.m_draftDeck;

  public int GetSlot() => this.m_currentSlot;

  public DraftSlotType GetSlotType() => this.m_currentSlotType;

  public bool HasSlotType(DraftSlotType slotType) => this.m_uniqueDraftSlotTypesForDeck.Contains(slotType);

  public bool CanShowWinsLosses => this.m_hasReceivedSessionWinsLosses;

  public int GetLosses() => this.m_losses;

  public int GetWins() => this.m_wins;

  public int GetMaxWins() => this.m_maxWins;

  public int GetNumTicketsOwned() => this.m_numTicketsOwned;

  public bool GetIsNewKey() => this.m_isNewKey;

  public AssetReference GetDraftPaperTexture()
  {
    string assetString = (string) null;
    if (this.m_currentSeason != null)
      assetString = !(bool) UniversalInputManager.UsePhoneUI ? this.m_currentSeason.Season.DraftPaperTexture : this.m_currentSeason.Season.DraftPaperTexturePhone;
    if (!string.IsNullOrEmpty(assetString))
      return new AssetReference(assetString);
    return (bool) UniversalInputManager.UsePhoneUI ? DraftManager.DEFAULT_DRAFT_PAPER_TEXTURE_PHONE : DraftManager.DEFAULT_DRAFT_PAPER_TEXTURE;
  }

  public bool GetDraftPaperTextColorOverride(ref Color overrideColor) => this.m_currentSeason != null && !string.IsNullOrEmpty(this.m_currentSeason.Season.DraftPaperTextColor) && ColorUtility.TryParseHtmlString(this.m_currentSeason.Season.DraftPaperTextColor, out overrideColor);

  public AssetReference GetRewardPaperPrefab()
  {
    string assetString = (string) null;
    if (this.m_currentSeason != null)
      assetString = !(bool) UniversalInputManager.UsePhoneUI ? this.m_currentSeason.Season.RewardPaperPrefab : this.m_currentSeason.Season.RewardPaperPrefabPhone;
    return !string.IsNullOrEmpty(assetString) ? new AssetReference(assetString) : ArenaRewardPaper.GetDefaultRewardPaper();
  }

  public string GetSceneHeadlineText() => this.m_currentSeason != null && this.m_currentSeason.Season.Strings.Count > 0 ? GameStrings.FormatStringWithPlurals(this.m_currentSeason.Season.Strings, "SCENE_HEADLINE") : string.Empty;

  public bool ShouldActivateKey()
  {
    GameContentScenario gameContentScenario = this.m_currentSeason.Season.GameContentSeason.Scenarios.FirstOrDefault<GameContentScenario>();
    int num1 = gameContentScenario == null ? 0 : gameContentScenario.MaxWins;
    int num2 = gameContentScenario == null ? 0 : gameContentScenario.MaxLosses;
    if (this.m_deckActiveDuringSession)
      return true;
    return this.m_inRewards && this.m_wins < num1 && this.m_losses < num2;
  }

  public List<RewardData> GetRewards() => this.m_chest != null ? this.m_chest.Rewards : new List<RewardData>();

  public void MakeChoice(int choiceNum, TAG_PREMIUM choicePremium)
  {
    this.m_chosenIndex = choiceNum;
    if (this.m_draftDeck == null)
    {
      Debug.LogWarning((object) "DraftManager.MakeChoice(): Trying to make a draft choice while the draft deck is null");
    }
    else
    {
      if (this.m_validSlot != this.m_currentSlot)
        return;
      ++this.m_validSlot;
      Network.Get().MakeDraftChoice(this.m_draftDeck.ID, this.m_currentSlot, choiceNum, (int) choicePremium);
    }
  }

  public void NotifyOfFinalGame(bool wonFinalGame)
  {
    if (wonFinalGame)
      ++this.m_wins;
    else
      ++this.m_losses;
  }

  public void FindGame()
  {
    GameContentScenario gameContentScenario = this.m_currentSeason.Season.GameContentSeason.Scenarios.FirstOrDefault<GameContentScenario>();
    int missionId = gameContentScenario == null ? 2 : gameContentScenario.ScenarioId;
    GameMgr.Get().FindGame(GameType.GT_ARENA, FormatType.FT_WILD, missionId, seasonId: new int?(this.CurrentSeasonId));
    if (this.m_draftDeck == null)
      return;
    Log.Decks.PrintInfo("Starting Arena Game With Deck:");
    this.m_draftDeck.LogDeckStringInformation();
  }

  public TAG_PREMIUM GetDraftPremium(string cardId)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager.GetNumCopiesInCollection(cardId, TAG_PREMIUM.DIAMOND) > 0 && this.m_draftDeck.GetCardIdCount(cardId) == 0)
      return TAG_PREMIUM.DIAMOND;
    bool isLegendary = collectionManager.GetCard(cardId, TAG_PREMIUM.NORMAL).Rarity == TAG_RARITY.LEGENDARY;
    if (this.IsBestValidPremium(cardId, TAG_PREMIUM.SIGNATURE, isLegendary))
      return TAG_PREMIUM.SIGNATURE;
    return this.IsBestValidPremium(cardId, TAG_PREMIUM.GOLDEN, isLegendary) ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL;
  }

  private bool IsBestValidPremium(string cardId, TAG_PREMIUM premium, bool isLegendary)
  {
    int copiesInCollection = CollectionManager.Get().GetNumCopiesInCollection(cardId, premium);
    return copiesInCollection > 0 && (isLegendary || copiesInCollection >= 2 || this.m_draftDeck.GetCardCountAllMatchingSlots(cardId, premium) < copiesInCollection);
  }

  public bool ShouldShowFreeArenaWinScreen() => SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_FROST_FESTIVAL_FREE_ARENA_WIN, false) && !Options.Get().GetBool(Option.HAS_SEEN_FREE_ARENA_WIN_DIALOG_THIS_DRAFT) && this.m_wins > 0;

  public void PromptToDisablePremium()
  {
    if (this.m_pendingRequestToDisablePremiums || Options.Get().GetBool(Option.HAS_DISABLED_PREMIUMS_THIS_DRAFT) || this.m_inRewards)
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_DRAFT_REMOVE_PREMIUMS_DIALOG_TITLE"),
      m_text = GameStrings.Get("GLUE_DRAFT_REMOVE_PREMIUMS_DIALOG_BODY"),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
      m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnDisablePremiumsConfirmationResponse)
    });
    this.m_pendingRequestToDisablePremiums = true;
  }

  private void OnDisablePremiumsConfirmationResponse(AlertPopup.Response response, object userData)
  {
    this.m_pendingRequestToDisablePremiums = false;
    if (response != AlertPopup.Response.CONFIRM)
      return;
    Network.Get().DraftRequestDisablePremiums();
  }

  private void OnDraftRemovePremiumsResponse()
  {
    Options.Get().SetBool(Option.HAS_DISABLED_PREMIUMS_THIS_DRAFT, true);
    Network.DraftChoicesAndContents premiumsResponse = Network.Get().GetDraftRemovePremiumsResponse();
    this.m_draftDeck.GetSlots().Clear();
    foreach (Network.CardUserData card in premiumsResponse.DeckInfo.Cards)
    {
      string cardID = card.DbId == 0 ? string.Empty : GameUtils.TranslateDbIdToCardId(card.DbId);
      for (int index = 0; index < card.Count; ++index)
      {
        if (!this.m_draftDeck.AddCard(cardID, card.Premium, false))
          Debug.LogWarning((object) string.Format("DraftManager.OnDraftRemovePremiumsResponse() - Card {0} could not be added to draft deck", (object) cardID));
      }
    }
    DraftPhoneDeckTray.Get().GetCardsContent().UpdateCardList();
    this.InformDraftDisplayOfChoices(premiumsResponse.Choices);
  }

  public bool ShowArenaPopup_SeasonEndingSoon(
    long secondsToCurrentSeasonEnd,
    Action popupClosedCallback)
  {
    if (this.m_currentSeason == null || !this.m_currentSeason.HasSeasonEndingSoonPrefab || string.IsNullOrEmpty(this.m_currentSeason.SeasonEndingSoonPrefab) || !this.m_currentSeason.HasSeason || this.m_currentSeason.Season == null || this.m_currentSeason.Season.Strings.Count == 0)
    {
      Error.AddDevWarning("No Season Data", "Cannot show 'Ending Soon' dialog - the current Arena season={0} does not have the ENDING_SOON_PREFAB data or header/body strings.", (object) this.CurrentSeasonId);
      return false;
    }
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_seconds = (string) null,
      m_minutes = (string) null,
      m_hours = "GLUE_ARENA_POPUP_ENDING_SOON_HEADER_HOURS",
      m_yesterday = (string) null,
      m_days = "GLUE_ARENA_POPUP_ENDING_SOON_HEADER_DAYS",
      m_weeks = "GLUE_ARENA_POPUP_ENDING_SOON_HEADER_WEEKS",
      m_monthAgo = "GLUE_ARENA_POPUP_ENDING_SOON_HEADER_MONTHS"
    };
    return DialogManager.Get().ShowArenaSeasonPopup(UserAttentionBlocker.NONE, new BasicPopup.PopupInfo()
    {
      m_prefabAssetRefs = {
        this.m_currentSeason.SeasonEndingSoonPrefab,
        this.m_currentSeason.SeasonEndingSoonPrefabExtra
      },
      m_headerText = TimeUtils.GetElapsedTimeString(secondsToCurrentSeasonEnd, stringSet, true),
      m_bodyText = GameStrings.FormatStringWithPlurals(this.m_currentSeason.Season.Strings, "ENDING_SOON_BODY"),
      m_responseUserData = (object) this.CurrentSeasonId,
      m_blurWhenShown = true,
      m_responseCallback = (BasicPopup.ResponseCallback) ((response, userData) =>
      {
        if (popupClosedCallback == null)
          return;
        popupClosedCallback();
      })
    });
  }

  public bool ShowArenaPopup_SeasonComingSoon(
    long secondsToNextSeasonStart,
    Action popupClosedCallback)
  {
    if (this.m_currentSeason == null || !this.m_currentSeason.HasNextSeasonComingSoonPrefab || string.IsNullOrEmpty(this.m_currentSeason.NextSeasonComingSoonPrefab) || this.m_currentSeason.NextSeasonStrings == null || this.m_currentSeason.NextSeasonStrings.Count == 0)
    {
      Error.AddDevWarning("No Season Data", "Cannot show 'Coming Soon' dialog - the season after current Arena season={0} does not have the COMING_SOON_PREFAB data or header/body strings.", (object) this.CurrentSeasonId);
      return false;
    }
    TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
    {
      m_seconds = (string) null,
      m_minutes = (string) null,
      m_hours = "GLUE_ARENA_POPUP_COMING_SOON_HEADER_HOURS",
      m_yesterday = (string) null,
      m_days = "GLUE_ARENA_POPUP_COMING_SOON_HEADER_DAYS",
      m_weeks = "GLUE_ARENA_POPUP_COMING_SOON_HEADER_WEEKS",
      m_monthAgo = "GLUE_ARENA_POPUP_COMING_SOON_HEADER_MONTHS"
    };
    return DialogManager.Get().ShowArenaSeasonPopup(UserAttentionBlocker.NONE, new BasicPopup.PopupInfo()
    {
      m_prefabAssetRefs = {
        this.m_currentSeason.NextSeasonComingSoonPrefab,
        this.m_currentSeason.NextSeasonComingSoonPrefabExtra
      },
      m_headerText = TimeUtils.GetElapsedTimeString(secondsToNextSeasonStart, stringSet, true),
      m_bodyText = GameStrings.FormatStringWithPlurals(this.m_currentSeason.NextSeasonStrings, "COMING_SOON_BODY"),
      m_blurWhenShown = true,
      m_responseUserData = (object) this.m_currentSeason.NextSeasonId,
      m_responseCallback = (BasicPopup.ResponseCallback) ((response, userData) =>
      {
        if (popupClosedCallback == null)
          return;
        popupClosedCallback();
      })
    });
  }

  public bool ShowNextArenaPopup(Action popupClosedCallback)
  {
    if (this.m_currentSeason == null || PopupDisplayManager.Get().IsShowing || ReturningPlayerMgr.Get().SuppressOldPopups || SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN || !this.HasActiveRun)
      return false;
    bool flag = this.ShowSeasonEnding(popupClosedCallback);
    return !flag ? this.ShowSeasonStarting(popupClosedCallback) : flag;
  }

  private bool ShowSeasonEnding(Action popupClosedCallback)
  {
    int num = Options.Get().GetInt(Option.LATEST_SEEN_ARENA_SEASON_ENDING);
    long? nullable = !this.m_currentSeason.HasSeason ? new long?() : new long?((long) this.m_currentSeason.Season.GameContentSeason.EndSecondsFromNow);
    if (!nullable.HasValue || !this.m_currentSeason.HasSeasonEndingSoonDays || nullable.Value > (long) (this.m_currentSeason.SeasonEndingSoonDays * 86400) || num >= this.CurrentSeasonId)
      return false;
    int seasonIdEnding = this.CurrentSeasonId;
    Action popupClosedCallback1 = (Action) (() =>
    {
      Options.Get().SetInt(Option.LATEST_SEEN_ARENA_SEASON_ENDING, seasonIdEnding);
      if (popupClosedCallback == null)
        return;
      popupClosedCallback();
    });
    return this.ShowArenaPopup_SeasonEndingSoon(nullable.Value, popupClosedCallback1);
  }

  private bool ShowSeasonStarting(Action popupClosedCallback)
  {
    int num = Options.Get().GetInt(Option.LATEST_SEEN_ARENA_SEASON_STARTING);
    long? nullable = !this.m_currentSeason.HasNextStartSecondsFromNow ? new long?() : new long?((long) this.m_currentSeason.NextStartSecondsFromNow);
    if (!nullable.HasValue || !this.m_currentSeason.HasNextSeasonComingSoonDays || nullable.Value > (long) (this.m_currentSeason.NextSeasonComingSoonDays * 86400) || !this.m_currentSeason.HasNextSeasonId || num >= this.m_currentSeason.NextSeasonId)
      return false;
    int seasonIdStarting = this.m_currentSeason.NextSeasonId;
    Action popupClosedCallback1 = (Action) (() =>
    {
      Options.Get().SetInt(Option.LATEST_SEEN_ARENA_SEASON_STARTING, seasonIdStarting);
      if (popupClosedCallback == null)
        return;
      popupClosedCallback();
    });
    return this.ShowArenaPopup_SeasonComingSoon(nullable.Value, popupClosedCallback1);
  }

  public void ClearAllInnkeeperPopups()
  {
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_HERO_CHOICE);
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_CARD_CHOICE);
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_CARD_CHOICE2);
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_PLAY_MODE);
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_1WIN);
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_2LOSS);
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_RETIRE);
    Options.Get().DeleteOption(Option.HAS_SEEN_FORGE_MAX_WIN);
  }

  public void ClearAllSeenPopups()
  {
    Options.Get().DeleteOption(Option.LATEST_SEEN_SCHEDULED_ENTERED_ARENA_DRAFT);
    Options.Get().DeleteOption(Option.HAS_SEEN_FREE_ARENA_WIN_DIALOG_THIS_DRAFT);
    Options.Get().DeleteOption(Option.LATEST_SEEN_ARENA_SEASON_ENDING);
    Options.Get().DeleteOption(Option.LATEST_SEEN_ARENA_SEASON_STARTING);
  }

  private void ClearDeckInfo()
  {
    this.m_draftDeck = (CollectionDeck) null;
    this.m_hasReceivedSessionWinsLosses = false;
    this.m_losses = 0;
    this.m_wins = 0;
    this.m_maxWins = int.MaxValue;
    this.m_isNewKey = false;
    this.m_chest = (Network.RewardChest) null;
    this.m_deckActiveDuringSession = false;
    Options.Get().SetBool(Option.HAS_SEEN_FREE_ARENA_WIN_DIALOG_THIS_DRAFT, false);
    Options.Get().SetBool(Option.HAS_DISABLED_PREMIUMS_THIS_DRAFT, false);
  }

  private void OnBegin()
  {
    Options.Get().SetBool(Option.HAS_SEEN_FREE_ARENA_WIN_DIALOG_THIS_DRAFT, false);
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.DRAFT || SceneMgr.Get().IsTransitionNowOrPending() && SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.DRAFT)
      return;
    this.m_hasReceivedSessionWinsLosses = true;
    Network.BeginDraft beginDraft = Network.Get().GetBeginDraft();
    this.m_draftDeck = new CollectionDeck()
    {
      ID = beginDraft.DeckID,
      Type = DeckType.DRAFT_DECK,
      FormatType = FormatType.FT_WILD
    };
    this.m_wins = beginDraft.Wins;
    this.m_losses = 0;
    this.m_currentSlot = 0;
    this.m_currentSlotType = beginDraft.SlotType;
    this.m_uniqueDraftSlotTypesForDeck = beginDraft.UniqueSlotTypesForDraft;
    this.m_validSlot = 0;
    this.m_maxSlot = beginDraft.MaxSlot;
    this.m_chest = (Network.RewardChest) null;
    this.m_inRewards = false;
    this.m_currentSession = beginDraft.Session;
    Options.Get().SetBool(Option.HAS_DISABLED_PREMIUMS_THIS_DRAFT, false);
    BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
    {
      Wins = (uint) beginDraft.Wins,
      Losses = 0U,
      RunFinished = false,
      SessionRecordType = SessionRecordType.ARENA
    });
    Log.Arena.Print(string.Format("DraftManager.OnBegin - Got new draft deck with ID: {0}", (object) this.m_draftDeck.ID));
    this.InformDraftDisplayOfChoices(beginDraft.Heroes);
    this.FireDraftDeckSetEvent();
  }

  private void OnRetire()
  {
    Network.DraftRetired retiredDraft = Network.Get().GetRetiredDraft();
    Log.Arena.Print(string.Format("DraftManager.OnRetire deckID={0}", (object) retiredDraft.Deck));
    this.m_chest = retiredDraft.Chest;
    this.m_inRewards = true;
    this.InformDraftDisplayOfChoices(new List<NetCache.CardDefinition>());
  }

  private void OnAckRewards()
  {
    BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
    {
      Wins = (uint) this.m_wins,
      Losses = (uint) this.m_losses,
      RunFinished = true,
      SessionRecordType = SessionRecordType.ARENA
    });
    if (!Options.Get().GetBool(Option.HAS_ACKED_ARENA_REWARDS, false) && UserAttentionManager.CanShowAttentionGrabber("DraftManager.OnAckRewards:" + (object) Option.HAS_ACKED_ARENA_REWARDS))
    {
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_ARENA_1ST_REWARD"), "VO_INNKEEPER_ARENA_1ST_REWARD.prefab:660e915849550ae4085735866647d529");
      Options.Get().SetBool(Option.HAS_ACKED_ARENA_REWARDS, true);
    }
    Network.Get().GetRewardsAckDraftID();
    this.ClearDeckInfo();
  }

  private void OnChoicesAndContents()
  {
    Network.DraftChoicesAndContents choicesAndContents = Network.Get().GetDraftChoicesAndContents();
    this.m_hasReceivedSessionWinsLosses = true;
    this.m_currentSlot = choicesAndContents.Slot;
    this.m_currentSlotType = choicesAndContents.SlotType;
    this.m_uniqueDraftSlotTypesForDeck = choicesAndContents.UniqueSlotTypesForDraft;
    this.m_validSlot = choicesAndContents.Slot;
    this.m_maxSlot = choicesAndContents.MaxSlot;
    this.m_draftDeck = new CollectionDeck()
    {
      ID = choicesAndContents.DeckInfo.Deck,
      Type = DeckType.DRAFT_DECK,
      HeroCardID = choicesAndContents.Hero.Name,
      HeroPowerCardID = choicesAndContents.HeroPower.Name,
      FormatType = FormatType.FT_WILD
    };
    Log.Arena.Print(string.Format("DraftManager.OnChoicesAndContents - Draft Deck ID: {0}, Hero Card = {1}", (object) this.m_draftDeck.ID, (object) this.m_draftDeck.HeroCardID));
    foreach (Network.CardUserData card in choicesAndContents.DeckInfo.Cards)
    {
      string cardID = card.DbId == 0 ? string.Empty : GameUtils.TranslateDbIdToCardId(card.DbId);
      Log.Arena.Print(string.Format("DraftManager.OnChoicesAndContents - Draft deck contains card {0}", (object) cardID));
      for (int index = 0; index < card.Count; ++index)
      {
        if (!this.m_draftDeck.AddCard(cardID, card.Premium, false))
          Debug.LogWarning((object) string.Format("DraftManager.OnChoicesAndContents() - Card {0} could not be added to draft deck", (object) cardID));
      }
    }
    this.m_losses = choicesAndContents.Losses;
    this.m_isNewKey = choicesAndContents.Wins > this.m_wins;
    this.m_wins = choicesAndContents.Wins;
    this.m_maxWins = choicesAndContents.MaxWins;
    this.m_chest = choicesAndContents.Chest;
    this.m_inRewards = this.m_chest != null;
    this.m_currentSession = choicesAndContents.Session;
    if (this.m_losses > 0 && DemoMgr.Get().ArenaIs1WinMode())
    {
      Network.Get().DraftRetire(this.GetDraftDeck().ID, this.GetSlot(), this.CurrentSeasonId);
    }
    else
    {
      if (this.m_wins == 5 && DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2013)
        DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_5_WINS"), false, false);
      else if (this.m_losses == 3 && !Options.Get().GetBool(Option.HAS_LOST_IN_ARENA, false) && UserAttentionManager.CanShowAttentionGrabber("DraftManager.OnChoicesAndContents:" + (object) Option.HAS_LOST_IN_ARENA))
      {
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_ARENA_3RD_LOSS"), "VO_INNKEEPER_ARENA_3RD_LOSS.prefab:6b2af024c9980d344a087295afb5e3df");
        Options.Get().SetBool(Option.HAS_LOST_IN_ARENA, true);
      }
      this.InformDraftDisplayOfChoices(choicesAndContents.Choices);
    }
  }

  private void InformDraftDisplayOfChoices(List<NetCache.CardDefinition> choices)
  {
    DraftDisplay draftDisplay = DraftDisplay.Get();
    if ((UnityEngine.Object) draftDisplay == (UnityEngine.Object) null)
      return;
    if (this.m_inRewards)
      draftDisplay.SetDraftMode(DraftDisplay.DraftMode.IN_REWARDS);
    else if (choices.Count == 0)
    {
      this.m_deckActiveDuringSession = true;
      draftDisplay.SetDraftMode(DraftDisplay.DraftMode.ACTIVE_DRAFT_DECK);
    }
    else
    {
      if (!Options.Get().GetBool(Option.HAS_DISABLED_PREMIUMS_THIS_DRAFT) && this.GetSlotType() != DraftSlotType.DRAFT_SLOT_HERO_POWER)
      {
        foreach (NetCache.CardDefinition choice in choices)
          choice.Premium = this.GetDraftPremium(choice.Name);
      }
      draftDisplay.SetDraftMode(DraftDisplay.DraftMode.DRAFTING);
      draftDisplay.AcceptNewChoices(choices);
    }
  }

  private void InformDraftDisplayOfSelectedChoice(int chosenIndex)
  {
    DraftDisplay draftDisplay = DraftDisplay.Get();
    if ((UnityEngine.Object) draftDisplay == (UnityEngine.Object) null)
      return;
    draftDisplay.OnChoiceSelected(chosenIndex);
  }

  private void OnChosen()
  {
    Network.DraftChosen draftChosen = Network.Get().GetDraftChosen();
    if (this.m_currentSlotType == DraftSlotType.DRAFT_SLOT_HERO)
    {
      Log.Arena.Print("DraftManager.OnChosen(): hero=" + draftChosen.ChosenCard.Name);
      this.m_draftDeck.HeroCardID = draftChosen.ChosenCard.Name;
    }
    else if (this.m_currentSlotType == DraftSlotType.DRAFT_SLOT_CARD)
      this.m_draftDeck.AddCard(draftChosen.ChosenCard.Name, draftChosen.ChosenCard.Premium, false);
    ++this.m_currentSlot;
    this.m_currentSlotType = draftChosen.SlotType;
    if (this.m_currentSlot > this.m_maxSlot && (UnityEngine.Object) DraftDisplay.Get() != (UnityEngine.Object) null)
      DraftDisplay.Get().DoDeckCompleteAnims();
    this.InformDraftDisplayOfSelectedChoice(this.m_chosenIndex);
    this.InformDraftDisplayOfChoices(draftChosen.NextChoices);
  }

  private void OnError()
  {
    if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.DRAFT))
      return;
    DraftError draftError = Network.Get().GetDraftError();
    this.m_numTicketsOwned = draftError.HasNumTicketsOwned ? draftError.NumTicketsOwned : 0;
    DraftDisplay draftDisplay = DraftDisplay.Get();
    switch (draftError.ErrorCode_)
    {
      case DraftError.ErrorCode.DE_UNKNOWN:
        Debug.LogError((object) "DraftManager.OnError - UNKNOWN EXCEPTION - See server logs for more info.");
        break;
      case DraftError.ErrorCode.DE_NO_LICENSE:
        Debug.LogWarning((object) "DraftManager.OnError - No License.  What does this mean???");
        break;
      case DraftError.ErrorCode.DE_RETIRE_FIRST:
        Debug.LogError((object) "DraftManager.OnError - You cannot start a new draft while one is in progress.");
        break;
      case DraftError.ErrorCode.DE_NOT_IN_DRAFT:
        if (!((UnityEngine.Object) draftDisplay != (UnityEngine.Object) null))
          break;
        draftDisplay.SetDraftMode(DraftDisplay.DraftMode.NO_ACTIVE_DRAFT);
        break;
      case DraftError.ErrorCode.DE_NOT_IN_DRAFT_BUT_COULD_BE:
        if (Options.Get().GetBool(Option.HAS_SEEN_FORGE, false))
        {
          if (this.m_numTicketsOwned > 0)
          {
            DraftDisplay.Get().SetDraftMode(DraftDisplay.DraftMode.NO_ACTIVE_DRAFT);
            break;
          }
          this.RequestDraftBegin();
          break;
        }
        DraftDisplay.Get().SetDraftMode(DraftDisplay.DraftMode.NO_ACTIVE_DRAFT);
        break;
      case DraftError.ErrorCode.DE_FEATURE_DISABLED:
        Debug.LogError((object) "DraftManager.OnError - The Arena is currently disabled. Returning to the hub.");
        if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
          break;
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_FORGE");
        break;
      case DraftError.ErrorCode.DE_SEASON_INCREMENTED:
        Error.AddWarningLoc("GLOBAL_ERROR_GENERIC_HEADER", "GLOBAL_ARENA_SEASON_ERROR_NOT_ACTIVE");
        DraftManager.Get().RefreshCurrentSeasonFromServer();
        if (SceneMgr.Get().GetMode() != SceneMgr.Mode.DRAFT)
          break;
        Navigation.GoBack();
        break;
      default:
        Debug.LogErrorFormat("DraftManager.onError - UNHANDLED ERROR - See server logs for more info. ERROR: {0}", (object) draftError.ErrorCode_);
        break;
    }
  }

  private void OnArenaSessionResponse() => this.OnArenaSessionResponsePacket(Network.Get().GetArenaSessionResponse());

  public void OnArenaSessionResponsePacket(ArenaSessionResponse response)
  {
    if (response == null || response.ErrorCode != PegasusShared.ErrorCode.ERROR_OK || !response.HasSession)
      return;
    this.m_hasReceivedSessionWinsLosses = true;
    this.m_wins = response.HasSession ? response.Session.Wins : 0;
    this.m_losses = response.HasSession ? response.Session.Losses : 0;
    this.m_currentSession = response.HasSession ? response.Session : (ArenaSession) null;
    if (response.HasCurrentSeason)
      this.m_currentSeason = response.CurrentSeason;
    if (!GameMgr.Get().IsArena() && !GameMgr.Get().IsNextArena())
      return;
    BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
    {
      Wins = (uint) this.m_wins,
      Losses = (uint) this.m_losses,
      RunFinished = false,
      SessionRecordType = SessionRecordType.ARENA
    });
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        if ((UnityEngine.Object) DraftDisplay.Get() != (UnityEngine.Object) null)
        {
          DraftDisplay.Get().HandleGameStartupFailure();
          break;
        }
        break;
      case FindGameState.SERVER_GAME_CONNECTING:
        if (GameMgr.Get().IsNextArena() && !this.m_hasReceivedSessionWinsLosses)
        {
          this.RefreshCurrentSeasonFromServer();
          break;
        }
        break;
    }
    return false;
  }

  private void OnDraftPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod)
  {
    if (this.m_draftDeck != null)
      StoreManager.Get().HideStore(ShopType.ARENA_STORE);
    else
      this.RequestDraftBegin();
  }

  public void RequestDraftBegin() => Network.Get().DraftBegin();

  private void FireDraftDeckSetEvent()
  {
    foreach (DraftManager.DraftDeckSet draftDeckSet in this.m_draftDeckSetListeners.ToArray())
      draftDeckSet(this.m_draftDeck);
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (!GameMgr.Get().IsArena() || mode != SceneMgr.Mode.GAMEPLAY)
      return;
    GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData)
  {
    switch (playState)
    {
      case TAG_PLAYSTATE.WON:
        NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
        if (netObject == null || this.GetWins() >= netObject.BestForgeWins)
          NetCache.Get().RefreshNetObject<NetCache.NetCacheProfileProgress>();
        if (this.GetWins() != 11)
          break;
        this.NotifyOfFinalGame(true);
        break;
      case TAG_PLAYSTATE.LOST:
      case TAG_PLAYSTATE.TIED:
        if (this.GetLosses() != 2)
          break;
        this.NotifyOfFinalGame(false);
        break;
    }
  }

  public delegate void DraftDeckSet(CollectionDeck deck);
}
