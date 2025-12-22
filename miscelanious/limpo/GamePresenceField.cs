using Blizzard.GameService.SDK.Client.Integration;
using PegasusClient;
using PegasusFSG;
using SpectatorProto;

public class GamePresenceField
{
  public const uint GAME_ACCOUNT = 2;
  public const uint CAN_BE_INVITED_TO_GAME = 1;
  public const uint DEBUG_STRING = 2;
  public const uint DEPRECATED_ARENA_RECORD = 3;
  public const uint CARDS_OPENED = 4;
  public const uint DRUID_LEVEL = 5;
  public const uint HUNTER_LEVEL = 6;
  public const uint MAGE_LEVEL = 7;
  public const uint PALADIN_LEVEL = 8;
  public const uint PRIEST_LEVEL = 9;
  public const uint ROGUE_LEVEL = 10;
  public const uint SHAMAN_LEVEL = 11;
  public const uint WARLOCK_LEVEL = 12;
  public const uint WARRIOR_LEVEL = 13;
  public const uint GAIN_MEDAL = 14;
  public const uint TRADITIONAL_TUTORIAL_COMPLETED = 15;
  public const uint COLLECTION_EVENT = 16;
  public const uint STATUS = 17;
  public const uint RANK = 18;
  public const uint CLIENT_VERSION = 19;
  public const uint CLIENT_ENV = 20;
  public const uint SPECTATOR_INFO = 21;
  public const uint SESSION_RECORD = 22;
  public const uint SECRET_SPECTATOR_INFO = 23;
  public const uint DECK_VALIDITY = 24;
  public const uint FIRESIDE_GATHERING_INFO = 25;
  public const uint PARTY_ID = 26;
  public const uint ACHIEVEMENT_COMPLETED = 27;
  public const uint BATTLEGROUNDS_TUTORIAL_COMPLETE = 28;
  public const uint MERCENARIES_TUTORIAL_COMPLETE = 29;

  public static uint[] TransientStatusFields => new uint[9]
  {
    17U,
    19U,
    20U,
    21U,
    23U,
    24U,
    25U,
    26U,
    1U
  };

  public static string GetFieldName(uint fieldId)
  {
    switch (fieldId)
    {
      case 1:
        return "CanBeInvitedToGame";
      case 2:
        return "DebugString";
      case 3:
        return "ArenaRecord";
      case 4:
        return "CardsOpened";
      case 5:
        return "DruidLevel";
      case 6:
        return "HunterLevel";
      case 7:
        return "MageLevel";
      case 8:
        return "PaladinLevel";
      case 9:
        return "PriestLevel";
      case 10:
        return "RogueLevel";
      case 11:
        return "ShamanLevel";
      case 12:
        return "WarlockLevel";
      case 13:
        return "WarriorLevel";
      case 14:
        return "GainMedal";
      case 15:
        return "TutorialBeaten";
      case 16:
        return "CollectionEvent";
      case 17:
        return "Status";
      case 18:
        return "Rank";
      case 19:
        return "ClientVersion";
      case 20:
        return "ClientEnv";
      case 21:
        return "SpectatorInfo";
      case 22:
        return "SessionRecord";
      case 23:
        return "SecretJoinInfo";
      case 24:
        return "DeckValidity";
      case 25:
        return "FSGInfo";
      case 26:
        return "PartyId";
      case 27:
        return "AchievementCompleted";
      case 28:
        return "BattlegroundsTutorialComplete";
      case 29:
        return "MercenariesTutorialBountyComplete";
      default:
        return fieldId.ToString();
    }
  }

  public static string GetFieldValue(PresenceUpdate update)
  {
    if (update.valCleared)
      return "null";
    if (update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE && update.groupId == 2U)
    {
      switch (update.fieldId)
      {
        case 1:
          return update.boolVal.ToString();
        case 2:
        case 3:
        case 4:
        case 19:
        case 20:
          return update.stringVal ?? "null";
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
        case 11:
        case 12:
        case 13:
        case 14:
        case 15:
        case 16:
        case 27:
        case 28:
        case 29:
          return update.intVal.ToString();
        case 17:
          return update.blobVal != null ? PresenceMgr.Get().GetStatusText(update.blobVal) : "null";
        case 18:
          return update.blobVal == null ? "null" : ProtobufUtil.ParseFrom<GamePresenceRank>(update.blobVal).ToHumanReadableString();
        case 21:
          return update.blobVal == null ? "null" : ProtobufUtil.ParseFrom<JoinInfo>(update.blobVal).ToHumanReadableString();
        case 22:
          return update.blobVal == null ? "null" : ProtobufUtil.ParseFrom<SessionRecord>(update.blobVal).ToHumanReadableString();
        case 23:
          return update.blobVal == null ? "null" : ProtobufUtil.ParseFrom<SecretJoinInfo>(update.blobVal).ToHumanReadableString();
        case 24:
          return update.blobVal == null ? "null" : ProtobufUtil.ParseFrom<DeckValidity>(update.blobVal).ToHumanReadableString();
        case 25:
          return update.blobVal == null ? "null" : ProtobufUtil.ParseFrom<FiresideGatheringInfo>(update.blobVal).ToHumanReadableString();
        case 26:
          return update.entityIdVal.ToString();
      }
    }
    return BnetPresenceField.GetUnnamedFieldValue(update);
  }
}
