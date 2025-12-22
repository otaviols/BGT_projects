using PegasusShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class ShareableDeck
{
  public const int VersionNumberZero = 0;
  public const int VersionNumberOne = 1;
  public const int VersionNumberCurrent = 1;
  public const string CommentLinePrefix = "# ";
  public const string DeckNameLinePrefix = "###";
  public const char DeckCodeWithNameSeparator = '\n';
  public static readonly string DeckCodeWithNameFormat = "### {0}\n{1}";

  private int VersionNumber
  {
    set => this.\u003CVersionNumber\u003Ek__BackingField = value;
  }

  public string DeckName { get; set; }

  public int HeroCardDbId { get; set; }

  public PegasusUtil.DeckContents DeckContents { get; set; }

  public FormatType FormatType { get; set; }

  public bool IsArenaDeck { get; set; }

  protected ShareableDeck()
  {
    this.VersionNumber = 1;
    this.DeckName = string.Empty;
    this.HeroCardDbId = 0;
    this.DeckContents = new PegasusUtil.DeckContents();
    this.FormatType = FormatType.FT_UNKNOWN;
    this.IsArenaDeck = false;
  }

  public ShareableDeck(
    string deckName,
    int heroCardDbId,
    PegasusUtil.DeckContents deckContents,
    FormatType formatType,
    bool isArenaDeck)
  {
    this.DeckName = deckName;
    this.HeroCardDbId = heroCardDbId;
    this.DeckContents = deckContents;
    this.FormatType = formatType;
    this.IsArenaDeck = isArenaDeck;
    this.VersionNumber = 1;
  }

  public static ShareableDeck DeserializeFromClipboard() => ShareableDeck.Deserialize(ClipboardUtils.PastedStringFromClipboard);

  public static ShareableDeck Deserialize(string pastedString)
  {
    if (string.IsNullOrEmpty(pastedString))
      return (ShareableDeck) null;
    bool deckHasWildCards = false;
    ShareableDeck shareableDeck = new ShareableDeck();
    try
    {
      string deckName;
      string dataFromDeckString = ShareableDeck.ParseDataFromDeckString(pastedString, out deckName);
      if (string.IsNullOrEmpty(dataFromDeckString))
        return (ShareableDeck) null;
      shareableDeck.DeckName = deckName;
      using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(dataFromDeckString)))
      {
        if (!ShareableDeck.IsValidEncodedDeckHeader((Stream) stream))
          return (ShareableDeck) null;
        if (!ShareableDeck.DeserializeFromVersion((int) ProtocolParser.ReadUInt64((Stream) stream), shareableDeck, stream, ref deckHasWildCards))
          return (ShareableDeck) null;
      }
    }
    catch (Exception ex)
    {
      return (ShareableDeck) null;
    }
    if (deckHasWildCards)
      shareableDeck.FormatType = FormatType.FT_WILD;
    else if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
      shareableDeck.FormatType = FormatType.FT_STANDARD;
    return shareableDeck;
  }

  public static ShareableDeck ParseDeckCode(string input, out string deckName)
  {
    deckName = string.Empty;
    if (string.IsNullOrEmpty(input))
      return (ShareableDeck) null;
    if (input.Length <= "###".Length)
      return (ShareableDeck) null;
    string pastedString = input;
    if (input.StartsWith("###"))
    {
      string str = input.Remove(0, "###".Length + 1).TrimEnd();
      if (string.IsNullOrEmpty(str))
        return (ShareableDeck) null;
      char[] anyOf = new char[2]{ ' ', '\n' };
      int num = str.LastIndexOfAny(anyOf);
      if (num < 0)
        return (ShareableDeck) null;
      if (num > 0)
        deckName = str.Substring(0, num);
      pastedString = str.Substring(num, str.Length - num);
    }
    return ShareableDeck.Deserialize(pastedString);
  }

  public static string GenerateDeckCodeMessage(string deckCode, string deckName = null) => string.IsNullOrWhiteSpace(deckName) ? deckCode : string.Format(ShareableDeck.DeckCodeWithNameFormat, (object) deckName, (object) deckCode);

  public static TAG_CLASS ExtractClassFromDeck(ShareableDeck deck)
  {
    string cardId = GameUtils.TranslateDbIdToCardId(deck.HeroCardDbId);
    return string.IsNullOrEmpty(cardId) ? TAG_CLASS.INVALID : DefLoader.Get().GetEntityDef(cardId).GetClass();
  }

  private static bool DeserializeFromVersion(
    int versionNumber,
    ShareableDeck shareableDeck,
    MemoryStream stream,
    ref bool deckHasWildCards)
  {
    if (versionNumber == 0)
      return ShareableDeck.DeserializeFromVersion_0(shareableDeck, stream, ref deckHasWildCards);
    if (versionNumber == 1)
      ;
    return ShareableDeck.DeserializeFromVersion_1(shareableDeck, stream, ref deckHasWildCards);
  }

  private static bool DeserializeFromVersion_0(
    ShareableDeck shareableDeck,
    MemoryStream stream,
    ref bool deckHasWildCards)
  {
    ulong num1 = ProtocolParser.ReadUInt64((Stream) stream);
    for (ulong index = 0; index < num1; ++index)
      shareableDeck.HeroCardDbId = (int) ProtocolParser.ReadUInt64((Stream) stream);
    if (!GameDbf.Card.HasRecord(shareableDeck.HeroCardDbId))
      return false;
    string cardId = GameUtils.TranslateDbIdToCardId(shareableDeck.HeroCardDbId);
    if (!DefLoader.Get().GetEntityDef(cardId).IsHeroSkin())
      return false;
    shareableDeck.FormatType = (FormatType) ProtocolParser.ReadUInt64((Stream) stream);
    if (shareableDeck.FormatType != FormatType.FT_WILD && shareableDeck.FormatType != FormatType.FT_STANDARD || !ShareableDeck.Deserialize_ReadArrayOfCards(1, TAG_PREMIUM.NORMAL, shareableDeck, stream, ref deckHasWildCards) || !ShareableDeck.Deserialize_ReadArrayOfCards(1, TAG_PREMIUM.GOLDEN, shareableDeck, stream, ref deckHasWildCards))
      return false;
    ulong num2 = ProtocolParser.ReadUInt64((Stream) stream);
    for (uint index = 0; (ulong) index < num2; ++index)
    {
      int num3 = (int) ProtocolParser.ReadUInt64((Stream) stream);
      ulong num4 = ProtocolParser.ReadUInt64((Stream) stream);
      if (!GameDbf.Card.HasRecord(num3) || !GameUtils.IsCardCollectible(GameUtils.TranslateDbIdToCardId(num3)))
        return false;
      if (GameUtils.IsWildCard(GameUtils.TranslateDbIdToCardId(num3)))
        deckHasWildCards = true;
      DeckCardData deckCardData = new DeckCardData()
      {
        Def = new PegasusShared.CardDef() { Premium = 0, Asset = num3 },
        Qty = (int) num4
      };
      shareableDeck.DeckContents.Cards.Add(deckCardData);
    }
    ulong num5 = ProtocolParser.ReadUInt64((Stream) stream);
    for (ulong index = 0; index < num5; ++index)
    {
      int num6 = (int) ProtocolParser.ReadUInt64((Stream) stream);
      ulong num7 = ProtocolParser.ReadUInt64((Stream) stream);
      if (!GameDbf.Card.HasRecord(num6) || !GameUtils.IsCardCollectible(GameUtils.TranslateDbIdToCardId(num6)))
        return false;
      if (GameUtils.IsWildCard(GameUtils.TranslateDbIdToCardId(num6)))
        deckHasWildCards = true;
      DeckCardData deckCardData = new DeckCardData()
      {
        Def = new PegasusShared.CardDef() { Premium = 1, Asset = num6 },
        Qty = (int) num7
      };
      shareableDeck.DeckContents.Cards.Add(deckCardData);
    }
    return true;
  }

  private static bool DeserializeFromVersion_1(
    ShareableDeck shareableDeck,
    MemoryStream stream,
    ref bool deckHasWildCards)
  {
    ulong num1 = ProtocolParser.ReadUInt64((Stream) stream);
    if (num1 <= 0UL)
      return false;
    bool flag = false;
    foreach (FormatType formatType in Enum.GetValues(typeof (FormatType)))
    {
      if ((long) formatType == (long) num1)
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return false;
    shareableDeck.FormatType = (FormatType) num1;
    ulong num2 = ProtocolParser.ReadUInt64((Stream) stream);
    for (ulong index = 0; index < num2; ++index)
      shareableDeck.HeroCardDbId = (int) ProtocolParser.ReadUInt64((Stream) stream);
    if (!GameDbf.Card.HasRecord(shareableDeck.HeroCardDbId))
      return false;
    string cardId = GameUtils.TranslateDbIdToCardId(shareableDeck.HeroCardDbId);
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    if (SceneMgr.Get().IsInDuelsMode())
    {
      if (!entityDef.IsHero())
        return false;
    }
    else if (!entityDef.IsHeroSkin())
      return false;
    if (shareableDeck.FormatType == FormatType.FT_CLASSIC && !((IEnumerable<TAG_CLASS>) GameUtils.CLASSIC_ORDERED_HERO_CLASSES).Contains<TAG_CLASS>(entityDef.GetClass()) || !ShareableDeck.Deserialize_ReadArrayOfCards(1, TAG_PREMIUM.NORMAL, shareableDeck, stream, ref deckHasWildCards) || !ShareableDeck.Deserialize_ReadArrayOfCards(2, TAG_PREMIUM.NORMAL, shareableDeck, stream, ref deckHasWildCards))
      return false;
    ulong num3 = ProtocolParser.ReadUInt64((Stream) stream);
    for (uint index = 0; (ulong) index < num3; ++index)
    {
      int cardDbId = (int) ProtocolParser.ReadUInt64((Stream) stream);
      ulong num4 = ProtocolParser.ReadUInt64((Stream) stream);
      if (!GameDbf.Card.HasRecord(cardDbId))
        return false;
      if (GameUtils.IsWildCard(GameUtils.TranslateDbIdToCardId(cardDbId)))
        deckHasWildCards = true;
      DeckCardData deckCardData1 = shareableDeck.DeckContents.Cards.FirstOrDefault<DeckCardData>((Func<DeckCardData, bool>) (deckCardData => deckCardData != null && deckCardData.Def != null && deckCardData.Def.Asset == cardDbId && deckCardData.Def.Premium == 0));
      if (deckCardData1 == null)
        deckCardData1 = new DeckCardData()
        {
          Def = new PegasusShared.CardDef()
          {
            Premium = 0,
            Asset = cardDbId
          },
          Qty = (int) num4
        };
      else
        deckCardData1.Qty += (int) num4;
      shareableDeck.DeckContents.Cards.Add(deckCardData1);
    }
    return true;
  }

  private static bool Deserialize_ReadArrayOfCards(
    int quantityPerCard,
    TAG_PREMIUM premium,
    ShareableDeck shareableDeck,
    MemoryStream stream,
    ref bool deckHasWildCards)
  {
    ulong num = ProtocolParser.ReadUInt64((Stream) stream);
    for (ulong index = 0; index < num; ++index)
    {
      int cardDbId = (int) ProtocolParser.ReadUInt64((Stream) stream);
      if (!GameDbf.Card.HasRecord(cardDbId))
        return false;
      if (GameUtils.IsWildCard(GameUtils.TranslateDbIdToCardId(cardDbId)))
        deckHasWildCards = true;
      DeckCardData deckCardData1 = shareableDeck.DeckContents.Cards.FirstOrDefault<DeckCardData>((Func<DeckCardData, bool>) (deckCardData => deckCardData != null && deckCardData.Def != null && deckCardData.Def.Asset == cardDbId && deckCardData.Def.Premium == 0));
      if (deckCardData1 == null)
        deckCardData1 = new DeckCardData()
        {
          Def = new PegasusShared.CardDef()
          {
            Premium = (int) premium,
            Asset = cardDbId
          },
          Qty = quantityPerCard
        };
      else
        deckCardData1.Qty += quantityPerCard;
      shareableDeck.DeckContents.Cards.Add(deckCardData1);
    }
    return true;
  }

  public virtual string Serialize(bool includeComments = true)
  {
    string version = this.SerializeToVersion(1);
    return includeComments ? this.GetDeckStringWithComments(version) : version;
  }

  private string SerializeToVersion(int versionNumber)
  {
    if (versionNumber == 0)
      return this.SerializeToVersion_0();
    if (versionNumber == 1)
      ;
    return this.SerializeToVersion_1();
  }

  private string SerializeToVersion_0()
  {
    if (this.DeckContents == null)
      return (string) null;
    byte[] inArray = (byte[]) null;
    using (MemoryStream stream = new MemoryStream())
    {
      stream.WriteByte((byte) 0);
      ProtocolParser.WriteUInt64((Stream) stream, 0UL);
      ProtocolParser.WriteUInt64((Stream) stream, 1UL);
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) this.HeroCardDbId);
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) Convert.ToUInt32((object) this.FormatType));
      int[] array1 = this.DeckContents.Cards.Where<DeckCardData>((Func<DeckCardData, bool>) (d => d.Def.Premium == 0 && d.Qty == 1)).Select<DeckCardData, int>((Func<DeckCardData, int>) (d => d.Def.Asset)).ToArray<int>();
      int[] array2 = this.DeckContents.Cards.Where<DeckCardData>((Func<DeckCardData, bool>) (d => d.Def.Premium == 1 && d.Qty == 1)).Select<DeckCardData, int>((Func<DeckCardData, int>) (d => d.Def.Asset)).ToArray<int>();
      DeckCardData[] array3 = this.DeckContents.Cards.Where<DeckCardData>((Func<DeckCardData, bool>) (d => d.Def.Premium == 0 && d.Qty != 1)).ToArray<DeckCardData>();
      DeckCardData[] array4 = this.DeckContents.Cards.Where<DeckCardData>((Func<DeckCardData, bool>) (d => d.Def.Premium == 1 && d.Qty != 1)).ToArray<DeckCardData>();
      this.Serialize_WriteArrayOfCards(array1, stream);
      this.Serialize_WriteArrayOfCards(array2, stream);
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) array3.Length);
      foreach (DeckCardData deckCardData in array3)
      {
        ProtocolParser.WriteUInt64((Stream) stream, (ulong) deckCardData.Def.Asset);
        ProtocolParser.WriteUInt64((Stream) stream, (ulong) Math.Max(0, deckCardData.Qty));
      }
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) ((IEnumerable<DeckCardData>) array4).Count<DeckCardData>());
      foreach (DeckCardData deckCardData in array4)
      {
        ProtocolParser.WriteUInt64((Stream) stream, (ulong) deckCardData.Def.Asset);
        ProtocolParser.WriteUInt64((Stream) stream, (ulong) Math.Max(0, deckCardData.Qty));
      }
      inArray = stream.ToArray();
    }
    return Convert.ToBase64String(inArray);
  }

  private string SerializeToVersion_1()
  {
    if (this.DeckContents == null)
      return (string) null;
    byte[] inArray = (byte[]) null;
    using (MemoryStream stream = new MemoryStream())
    {
      stream.WriteByte((byte) 0);
      ProtocolParser.WriteUInt64((Stream) stream, 1UL);
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) Convert.ToUInt32((object) this.FormatType));
      ProtocolParser.WriteUInt64((Stream) stream, 1UL);
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) this.HeroCardDbId);
      int[] array1 = this.DeckContents.Cards.Where<DeckCardData>((Func<DeckCardData, bool>) (d => d.Qty == 1)).Select<DeckCardData, int>((Func<DeckCardData, int>) (d => d.Def.Asset)).OrderBy<int, int>((Func<int, int>) (d => d)).ToArray<int>();
      int[] array2 = this.DeckContents.Cards.Where<DeckCardData>((Func<DeckCardData, bool>) (d => d.Qty == 2)).Select<DeckCardData, int>((Func<DeckCardData, int>) (d => d.Def.Asset)).OrderBy<int, int>((Func<int, int>) (d => d)).ToArray<int>();
      DeckCardData[] array3 = this.DeckContents.Cards.Where<DeckCardData>((Func<DeckCardData, bool>) (d => d.Qty > 2)).OrderBy<DeckCardData, int>((Func<DeckCardData, int>) (d => d.Def.Asset)).ToArray<DeckCardData>();
      this.Serialize_WriteArrayOfCards(array1, stream);
      this.Serialize_WriteArrayOfCards(array2, stream);
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) array3.Length);
      foreach (DeckCardData deckCardData in array3)
      {
        ProtocolParser.WriteUInt64((Stream) stream, (ulong) deckCardData.Def.Asset);
        ProtocolParser.WriteUInt64((Stream) stream, (ulong) Math.Max(0, deckCardData.Qty));
      }
      inArray = stream.ToArray();
    }
    return Convert.ToBase64String(inArray);
  }

  public void Serialize_WriteArrayOfCards(int[] cardDbIds, MemoryStream stream)
  {
    ProtocolParser.WriteUInt64((Stream) stream, (ulong) cardDbIds.Length);
    foreach (int cardDbId in cardDbIds)
      ProtocolParser.WriteUInt64((Stream) stream, (ulong) cardDbId);
  }

  public override bool Equals(object obj)
  {
    ShareableDeck shareableDeck = (ShareableDeck) obj;
    if (shareableDeck == null || this.FormatType != shareableDeck.FormatType || this.DeckContents == null && shareableDeck.DeckContents != null || this.DeckContents != null && shareableDeck.DeckContents == null)
      return false;
    if (this.DeckContents == null && shareableDeck.DeckContents == null)
      return true;
    Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
    Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
    for (int index = 0; index < this.DeckContents.Cards.Count; ++index)
    {
      if (dictionary1.ContainsKey(this.DeckContents.Cards[index].Def.Asset))
        dictionary1[this.DeckContents.Cards[index].Def.Asset] += this.DeckContents.Cards[index].Qty;
      else
        dictionary1[this.DeckContents.Cards[index].Def.Asset] = this.DeckContents.Cards[index].Qty;
    }
    for (int index = 0; index < shareableDeck.DeckContents.Cards.Count; ++index)
    {
      if (dictionary2.ContainsKey(shareableDeck.DeckContents.Cards[index].Def.Asset))
        dictionary2[shareableDeck.DeckContents.Cards[index].Def.Asset] += shareableDeck.DeckContents.Cards[index].Qty;
      else
        dictionary2[shareableDeck.DeckContents.Cards[index].Def.Asset] = shareableDeck.DeckContents.Cards[index].Qty;
    }
    if (dictionary1.Count != dictionary2.Count)
      return false;
    foreach (KeyValuePair<int, int> keyValuePair in dictionary1)
    {
      if (!dictionary2.ContainsKey(keyValuePair.Key) || dictionary1[keyValuePair.Key] != dictionary2[keyValuePair.Key])
        return false;
    }
    return true;
  }

  public override int GetHashCode() => this.DeckContents != null ? this.DeckContents.GetHashCode() ^ this.HeroCardDbId.GetHashCode() : 0;

  private string GetDeckStringWithComments(string encodedDeck)
  {
    StringBuilder stringBuilder = new StringBuilder();
    string cardId = GameUtils.TranslateDbIdToCardId(this.HeroCardDbId);
    List<TAG_CLASS> tagClassList = new List<TAG_CLASS>();
    DefLoader.Get().GetEntityDef(cardId).GetClasses((IList<TAG_CLASS>) tagClassList);
    string classesName = GameStrings.GetClassesName((IList<TAG_CLASS>) tagClassList);
    string formatName = GameStrings.GetFormatName(this.FormatType);
    if (!this.IsArenaDeck)
    {
      stringBuilder.AppendFormat("{0} {1}\n", (object) "###", (object) this.DeckName);
    }
    else
    {
      stringBuilder.AppendFormat("{0} {1}\n", (object) "###", (object) GameStrings.Get("GLUE_COLLECTION_DECK_COPY_COMMENT_HEADER_DECK_ARENA"));
      stringBuilder.AppendFormat("{0}{1}\n", (object) "# ", (object) GameStrings.Get("GLUE_COLLECTION_DECK_PASTE_COMMENT_ARENA_WARNING"));
    }
    stringBuilder.Append("# ").AppendFormat(GameStrings.Get("GLUE_COLLECTION_DECK_COPY_COMMENT_HEADER_CLASS"), (object) classesName).Append("\n");
    stringBuilder.Append("# ").AppendFormat(GameStrings.Get("GLUE_COLLECTION_DECK_COPY_COMMENT_HEADER_FORMAT"), (object) formatName).Append("\n");
    if (this.FormatType == FormatType.FT_STANDARD)
    {
      string yearLocalizedString = SetRotationManager.Get().GetActiveSetRotationYearLocalizedString();
      stringBuilder.Append("# ").Append(yearLocalizedString).Append("\n");
    }
    stringBuilder.Append("#\n");
    if (this.DeckContents != null)
    {
      foreach (DeckCardData card in this.DeckContents.Cards)
      {
        EntityDef entityDef = DefLoader.Get().GetEntityDef(card.Def.Asset);
        stringBuilder.AppendFormat("# {0}x ({1}) {2}\n", (object) card.Qty, (object) entityDef.GetCost(), (object) entityDef.GetName());
      }
    }
    stringBuilder.Append("# \n");
    stringBuilder.Append(encodedDeck + "\n");
    stringBuilder.Append("# \n");
    stringBuilder.Append("# " + GameStrings.Get("GLUE_COLLECTION_DECK_PASTE_COMMENT_INSTRUCTIONS") + "\n");
    return stringBuilder.ToString();
  }

  private static bool IsValidEncodedDeckHeader(Stream stream)
  {
    byte[] buffer = new byte[1];
    if (stream.Read(buffer, 0, buffer.Length) < buffer.Length)
      return false;
    int num1 = 0;
    byte[] numArray = buffer;
    int index = num1;
    int num2 = index + 1;
    return numArray[index] == (byte) 0;
  }

  protected static string ParseDataFromDeckString(string deckString, out string deckName)
  {
    string[] source = deckString.Split(new string[3]
    {
      Environment.NewLine,
      "\r",
      "\n"
    }, StringSplitOptions.RemoveEmptyEntries);
    string str1 = ((IEnumerable<string>) source).FirstOrDefault<string>((Func<string, bool>) (s => !s.Trim().StartsWith("#")));
    string str2 = ((IEnumerable<string>) source).FirstOrDefault<string>((Func<string, bool>) (s => s.Trim().StartsWith("###")));
    deckName = string.Empty;
    if (!string.IsNullOrEmpty(str2))
    {
      deckName = str2.Replace("###", string.Empty);
      deckName = deckName.Trim();
    }
    int deckNameCharacters = CollectionDeck.DefaultMaxDeckNameCharacters;
    if (deckName.Length > deckNameCharacters)
      deckName = deckName.Substring(0, deckNameCharacters);
    return str1?.Trim();
  }
}
