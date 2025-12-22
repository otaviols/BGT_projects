using System;
using System.Text;

public class ShareableMercenariesTeam : ShareableDeck
{
  private int VersionNumber
  {
    set => this.\u003CVersionNumber\u003Ek__BackingField = value;
  }

  public LettuceTeam Team { get; set; }

  private ShareableMercenariesTeam() => this.VersionNumber = 0;

  public ShareableMercenariesTeam(LettuceTeam team)
  {
    this.Team = team;
    this.VersionNumber = 0;
  }

  public override string Serialize(bool includeComments = true)
  {
    string encodedDeck = Convert.ToBase64String(ProtobufUtil.ToByteArray((IProtoBuf) LettuceTeam.Convert(this.Team)));
    if (includeComments)
      encodedDeck = this.ModifyWithComments(encodedDeck);
    return encodedDeck;
  }

  public static ShareableMercenariesTeam DeserializeFromClipboard() => ShareableMercenariesTeam.Deserialize(ClipboardUtils.PastedStringFromClipboard);

  public static ShareableMercenariesTeam Deserialize(string pastedString)
  {
    if (string.IsNullOrEmpty(pastedString))
      return (ShareableMercenariesTeam) null;
    ShareableMercenariesTeam shareableMercenariesTeam = new ShareableMercenariesTeam();
    try
    {
      string deckName;
      string dataFromDeckString = ShareableDeck.ParseDataFromDeckString(pastedString, out deckName);
      if (string.IsNullOrEmpty(dataFromDeckString))
        return (ShareableMercenariesTeam) null;
      shareableMercenariesTeam.DeckName = deckName;
      PegasusLettuce.LettuceTeam from = ProtobufUtil.ParseFrom<PegasusLettuce.LettuceTeam>(Convert.FromBase64String(dataFromDeckString));
      shareableMercenariesTeam.Team = LettuceTeam.Convert(from, false, false);
      if (shareableMercenariesTeam.Team.TeamType == PegasusLettuce.LettuceTeam.Type.TYPE_INVALID)
        return (ShareableMercenariesTeam) null;
    }
    catch (Exception ex)
    {
      return (ShareableMercenariesTeam) null;
    }
    return shareableMercenariesTeam;
  }

  public static ShareableMercenariesTeam ParseDeckCode(
    string input,
    out string deckName)
  {
    deckName = string.Empty;
    if (input == null)
      return (ShareableMercenariesTeam) null;
    if (input.Length <= "###".Length)
      return (ShareableMercenariesTeam) null;
    string pastedString = input;
    if (input.StartsWith("###"))
    {
      string str = input.Remove(0, "###".Length + 1).TrimEnd();
      char[] anyOf = new char[2]{ ' ', '\n' };
      int num = str.LastIndexOfAny(anyOf);
      if (num < 0)
        return (ShareableMercenariesTeam) null;
      if (num > 0)
        deckName = str.Substring(0, num);
      pastedString = str.Substring(num, str.Length - num);
    }
    return ShareableMercenariesTeam.Deserialize(pastedString);
  }

  private string ModifyWithComments(string encodedDeck)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("###").Append(" ").AppendLine(this.Team.Name);
    stringBuilder.AppendLine("# ");
    if (this.Team != null)
    {
      foreach (LettuceMercenary merc in this.Team.GetMercs())
      {
        stringBuilder.Append("# ").AppendLine(merc.m_mercName ?? "");
        if (!merc.IsEquipmentSlotUnassigned())
          stringBuilder.Append("# ").Append("\t - ").AppendLine(merc.GetSlottedEquipment().GetCardName() ?? "");
      }
    }
    stringBuilder.AppendLine("# ");
    stringBuilder.AppendLine(encodedDeck);
    stringBuilder.AppendLine("# ");
    stringBuilder.Append("# ").AppendLine(GameStrings.Get("GLUE_COLLECTION_DECK_PASTE_COMMENT_INSTRUCTIONS"));
    return stringBuilder.ToString();
  }

  public override bool Equals(object obj)
  {
    LettuceTeam team = ((ShareableMercenariesTeam) obj).Team;
    if (this.Team == null && team != null || this.Team != null && team == null)
      return false;
    if (this.Team == null && team == null)
      return true;
    if (this.Team.GetMercCount() != team.GetMercCount())
      return false;
    foreach (LettuceMercenary merc in this.Team.GetMercs())
    {
      if (!team.IsMercInTeam(merc.ID) || !team.TryGetMerc(merc.ID, out LettuceMercenary _))
        return false;
      LettuceMercenary.Loadout loadout1 = this.Team.GetLoadout(merc);
      LettuceMercenary.Loadout loadout2 = team.GetLoadout(merc);
      int? id1 = loadout1.m_equipmentRecord?.ID;
      int? id2 = loadout2.m_equipmentRecord?.ID;
      if (!(id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue))
        return false;
      id2 = loadout1.m_artVariationRecord?.ID;
      id1 = loadout2.m_artVariationRecord?.ID;
      if (!(id2.GetValueOrDefault() == id1.GetValueOrDefault() & id2.HasValue == id1.HasValue))
        return false;
    }
    return true;
  }

  public override int GetHashCode() => this.Team != null ? this.Team.GetHashCode() : 0;
}
