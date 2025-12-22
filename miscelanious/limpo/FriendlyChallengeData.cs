using Blizzard.GameService.SDK.Client.Integration;
using PegasusShared;
using System.Collections.Generic;

public class FriendlyChallengeData
{
  public BnetPartyId m_partyId;
  public BnetGameAccountId m_challengerId;
  public bool m_challengerPending;
  public int m_scenarioId = 2;
  public int m_seasonId;
  public int m_brawlLibraryItemId;
  public BrawlType m_challengeBrawlType;
  public FormatType m_challengeFormatType;
  public BnetPlayer m_challenger;
  public long m_challengerDeckId;
  public long m_challengerHeroId;
  public bool m_challengerDeckOrHeroSelected;
  public byte[] m_challengerFsgSharedSecret;
  public BnetPlayer m_challengee;
  public long m_challengeeDeckId;
  public long m_challengeeHeroId;
  public long? m_challengerRandomHeroCardId;
  public long? m_challengeeRandomHeroCardId;
  public byte[] m_challengeeFsgSharedSecret;
  public bool m_challengeeAccepted;
  public bool m_challengeeDeckOrHeroSelected;
  public bool m_challengerInGameState;
  public bool m_challengeeInGameState;
  public string m_challengerDeckShareState;
  public string m_challengeeDeckShareState;
  public List<CollectionDeck> m_sharedDecks;
  public long? m_challengerCardBackId;
  public long? m_challengeeCardBackId;
  public bool m_updatePartyQuestInfoOnGameplaySceneUnload;
  public bool m_findGameErrorOccurred;

  public bool DidReceiveChallenge
  {
    get
    {
      if (this.m_challengerPending)
        return true;
      return this.m_challenger != null && this.m_challengee == BnetPresenceMgr.Get().GetMyPlayer();
    }
  }

  public bool DidSendChallenge => this.m_challengee != null && this.m_challenger == BnetPresenceMgr.Get().GetMyPlayer();

  public bool IsPendingGotoGame => !this.m_challengerInGameState || !this.m_challengeeInGameState;
}
