using Blizzard.T5.Core.Utils;

public class CheatResourceParser
{
  public static bool TryParse(string[] args, out CheatResource resource, out string errMsg)
  {
    resource = (CheatResource) null;
    errMsg = (string) null;
    if (args.Length == 0)
    {
      errMsg = "Missing valid resource. You must specify one of the following valid resources: cards, gold, dust, tutorial, hero, pack, arenaticket, arena.";
      return false;
    }
    string[] strArray = args[0].Split('=');
    switch (strArray[0])
    {
      case "adventureownership":
        resource = (CheatResource) new AllAdventureOwnershipCheatResource();
        return true;
      case "arena":
        int? nullable1 = new int?();
        int? nullable2 = new int?();
        if (args.Length > 1)
        {
          string[] args1 = args.Slice<string>(1);
          MultiAttributeParser multiAttributeParser = new MultiAttributeParser();
          if (!multiAttributeParser.load(args1, out errMsg) || !multiAttributeParser.getIntAttribute("win", out nullable1, out errMsg) || !multiAttributeParser.getIntAttribute("loss", out nullable2, out errMsg))
            return false;
        }
        resource = (CheatResource) new ArenaCheatResource()
        {
          Win = nullable1,
          Loss = nullable2
        };
        return true;
      case "arenaticket":
        int? nullable3 = new int?();
        if (strArray.Length > 1)
        {
          int result;
          if (!int.TryParse(strArray[1], out result))
          {
            errMsg = "Failed to parse ticket count value. The amount must be a valid number.";
            return false;
          }
          nullable3 = new int?(result);
        }
        resource = (CheatResource) new ArenaTicketCheatResource()
        {
          TicketCount = nullable3
        };
        return true;
      case "cards":
        resource = (CheatResource) new FullCardCollectionCheatResource();
        return true;
      case "dust":
        int? nullable4 = new int?();
        if (strArray.Length > 1)
        {
          int result;
          if (!int.TryParse(strArray[1], out result))
          {
            errMsg = "Failed to parse dust amount. The amount must be a valid number.";
            return false;
          }
          nullable4 = new int?(result);
        }
        resource = (CheatResource) new DustCheatResource()
        {
          Amount = nullable4
        };
        return true;
      case "gold":
        int? nullable5 = new int?();
        if (strArray.Length > 1)
        {
          int result;
          if (!int.TryParse(strArray[1], out result))
          {
            errMsg = "Failed to parse gold amount. The amount must be a valid number.";
            return false;
          }
          nullable5 = new int?(result);
        }
        resource = (CheatResource) new GoldCheatResource()
        {
          Amount = nullable5
        };
        return true;
      case "hero":
        string str1 = (string) null;
        string str2 = (string) null;
        bool? nullable6 = new bool?();
        int? nullable7 = new int?();
        int? nullable8 = new int?();
        if (args.Length > 1)
        {
          string[] args2 = args.Slice<string>(1);
          MultiAttributeParser multiAttributeParser = new MultiAttributeParser();
          if (!multiAttributeParser.load(args2, out errMsg))
            return false;
          multiAttributeParser.getStringAttribute("class", out str1);
          multiAttributeParser.getStringAttribute("gametype", out str2);
          if (!multiAttributeParser.getIntAttribute("level", out nullable7, out errMsg) || !multiAttributeParser.getIntAttribute("wins", out nullable8, out errMsg) || !multiAttributeParser.getBoolAttribute("golden", out nullable6, out errMsg))
            return false;
        }
        TAG_PREMIUM tagPremium = (nullable6.HasValue ? (nullable6.Value ? 1 : 0) : 0) != 0 ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL;
        resource = (CheatResource) new HeroCheatResource()
        {
          ClassName = str1,
          Level = nullable7,
          Wins = nullable8,
          Gametype = str2,
          Premium = new TAG_PREMIUM?(tagPremium)
        };
        return true;
      case "pack":
        int? nullable9 = new int?();
        int? nullable10 = new int?();
        if (args.Length > 1)
        {
          string[] args3 = args.Slice<string>(1);
          MultiAttributeParser multiAttributeParser = new MultiAttributeParser();
          if (!multiAttributeParser.load(args3, out errMsg) || !multiAttributeParser.getIntAttribute("count", out nullable9, out errMsg) || !multiAttributeParser.getIntAttribute("typeID", out nullable10, out errMsg))
            return false;
        }
        resource = (CheatResource) new PackCheatResource()
        {
          PackCount = nullable9,
          TypeID = nullable10
        };
        return true;
      case "tutorial":
        int? nullable11 = new int?();
        if (strArray.Length > 1)
        {
          int result;
          if (!int.TryParse(strArray[1], out result))
          {
            errMsg = "Failed to parse progress value. The amount must be a valid number.";
            return false;
          }
          nullable11 = new int?(result);
        }
        resource = (CheatResource) new TutorialCheatResource()
        {
          Progress = nullable11
        };
        return true;
      default:
        errMsg = "Missing valid resource. You must specify one of the following valid resources: cards, gold, dust, tutorial, hero, pack, arenaticket, arena.";
        return false;
    }
  }
}
