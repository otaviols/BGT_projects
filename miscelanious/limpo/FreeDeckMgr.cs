using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FreeDeckMgr : IService
{
  private Dictionary<int, CollectionDeck> m_collectionLoanerDecks;
  private Dictionary<int, DeckTemplateDbfRecord> m_loanerDeckTemplateMap;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    FreeDeckMgr freeDeckMgr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new Action(freeDeckMgr.WillReset);
    Network network = serviceLocator.Get<Network>();
    network.RegisterNetHandler((object) FreeDeckStateUpdate.PacketID.ID, new Network.NetHandler(freeDeckMgr.OnFreeDeckStateUpdate));
    network.RegisterNetHandler((object) FreeDeckChoiceResponse.PacketID.ID, new Network.NetHandler(freeDeckMgr.OnFreeDeckChoiceResponse));
    freeDeckMgr.m_collectionLoanerDecks = new Dictionary<int, CollectionDeck>();
    freeDeckMgr.m_loanerDeckTemplateMap = new Dictionary<int, DeckTemplateDbfRecord>();
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (CheatMgr),
    typeof (Network)
  };

  public void Shutdown()
  {
  }

  public static FreeDeckMgr Get() => ServiceManager.Get<FreeDeckMgr>();

  private void WillReset()
  {
    this.Status = FreeDeckMgr.FreeDeckStatus.UNKNOWN;
    this.TrialPeriodEndTime = new DateTime?();
    this.m_collectionLoanerDecks = new Dictionary<int, CollectionDeck>();
    this.m_loanerDeckTemplateMap = new Dictionary<int, DeckTemplateDbfRecord>();
  }

  public FreeDeckMgr.FreeDeckStatus Status { get; private set; }

  public DateTime? TrialPeriodEndTime { get; private set; }

  public bool IsLoanerDeckAvailableToClaim()
  {
    if (this.Status == FreeDeckMgr.FreeDeckStatus.AVAILABLE)
      return true;
    if (this.Status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD)
    {
      DateTime? trialPeriodEndTime = this.TrialPeriodEndTime;
      if (trialPeriodEndTime.HasValue)
      {
        DateTime now = DateTime.Now;
        trialPeriodEndTime = this.TrialPeriodEndTime;
        DateTime dateTime = trialPeriodEndTime.Value;
        return now >= dateTime;
      }
    }
    return false;
  }

  public CollectionDeck GetLoanerDeckFromDeckTemplateId(int deckTemplateId)
  {
    if (this.Status == FreeDeckMgr.FreeDeckStatus.CLAIMED || this.Status == FreeDeckMgr.FreeDeckStatus.UNKNOWN)
      return (CollectionDeck) null;
    if (this.m_collectionLoanerDecks == null || this.m_collectionLoanerDecks.Count == 0)
      return (CollectionDeck) null;
    return this.m_collectionLoanerDecks.ContainsKey(deckTemplateId) ? this.m_collectionLoanerDecks[deckTemplateId] : (CollectionDeck) null;
  }

  public Dictionary<int, CollectionDeck> GetLoanerDecksAsMap()
  {
    if (this.Status == FreeDeckMgr.FreeDeckStatus.CLAIMED || this.Status == FreeDeckMgr.FreeDeckStatus.UNKNOWN || this.Status == FreeDeckMgr.FreeDeckStatus.EXPIRED)
      return (Dictionary<int, CollectionDeck>) null;
    if (this.m_collectionLoanerDecks.Count == 0)
      this.InitializeLoanerDeckData();
    return this.m_collectionLoanerDecks;
  }

  public Dictionary<int, DeckTemplateDbfRecord> GetLoanerDeckTemplateMap()
  {
    if (this.m_loanerDeckTemplateMap.Count == 0)
      this.InitializeLoanerDeckData();
    return this.m_loanerDeckTemplateMap;
  }

  public int GetLoanerDecksCount() => this.m_collectionLoanerDecks.Count;

  private void OnFreeDeckStateUpdate()
  {
    FreeDeckStateUpdate freeDeckStateUpdate = Network.Get().GetFreeDeckStateUpdate();
    if (freeDeckStateUpdate == null)
      return;
    this.Status = (FreeDeckMgr.FreeDeckStatus) freeDeckStateUpdate.Status;
    if (freeDeckStateUpdate.HasTrialPeriodSecondsRemaining)
      this.TrialPeriodEndTime = new DateTime?(DateTime.Now.AddSeconds((double) freeDeckStateUpdate.TrialPeriodSecondsRemaining));
    else
      this.TrialPeriodEndTime = new DateTime?();
  }

  private void OnFreeDeckChoiceResponse()
  {
    if (Network.Get().GetFreeDeckChoiceResponse().Success)
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_FREE_DECK_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_FREE_DECK_ERROR_TEXT"),
      m_showAlertIcon = false,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  private CollectionDeck GetCollectionDeckForTemplateRecord(
    DeckTemplateDbfRecord templateRecord)
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    int deckId = templateRecord.DeckId;
    DeckDbfRecord record = GameDbf.Deck.GetRecord(deckId);
    if (record == null)
    {
      Debug.LogError((object) string.Format("Unable to find deck with ID {0}", (object) deckId));
      return (CollectionDeck) null;
    }
    TAG_CLASS classId = (TAG_CLASS) templateRecord.ClassId;
    CollectionDeck forTemplateRecord = new CollectionDeck()
    {
      ID = 0,
      Type = DeckType.NORMAL_DECK,
      Name = (string) record.Name,
      HeroCardID = CollectionManager.GetVanillaHero(classId),
      SortOrder = (long) templateRecord.SortOrder,
      FormatType = (FormatType) templateRecord.FormatType,
      CreateDate = TimeUtils.DateTimeToUnixTimeStamp(DateTime.Now),
      DeckTemplateId = templateRecord.ID,
      IsLoanerDeck = true
    };
    foreach (DeckCardDbfRecord card in record.Cards)
    {
      if (card != null)
      {
        string cardId = GameUtils.TranslateDbIdToCardId(card.CardRecord.ID);
        forTemplateRecord.AddCard(cardId, TAG_PREMIUM.NORMAL, false);
      }
    }
    Log.CollectionManager.Print("_decktemplate: Time spent loading loaner decks: " + (object) (float) ((double) Time.realtimeSinceStartup - (double) realtimeSinceStartup));
    return forTemplateRecord;
  }

  private void InitializeLoanerDeckData()
  {
    foreach (DeckTemplateDbfRecord record in GameDbf.DeckTemplate.GetRecords())
    {
      if (record.IsFreeReward && SpecialEventManager.Get().IsEventActive(record.Event, false))
      {
        this.m_collectionLoanerDecks.Add(record.ID, this.GetCollectionDeckForTemplateRecord(record));
        this.m_loanerDeckTemplateMap.Add(record.ID, record);
      }
    }
  }

  public enum FreeDeckStatus
  {
    UNKNOWN,
    AVAILABLE,
    CLAIMED,
    EXPIRED,
    TRIAL_PERIOD,
  }
}
