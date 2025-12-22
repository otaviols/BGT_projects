using Assets;
using PegasusShared;
using System;
using System.Collections.Generic;

public static class DbfUtils
{
  public static ScenarioDbfRecord ConvertFromProtobuf(
    ScenarioDbRecord protoScenario,
    out List<ScenarioGuestHeroesDbfRecord> outScenarioGuestHeroRecords,
    out List<ClassExclusionsDbfRecord> outClassExclusionsRecords)
  {
    outScenarioGuestHeroRecords = new List<ScenarioGuestHeroesDbfRecord>();
    outClassExclusionsRecords = new List<ClassExclusionsDbfRecord>();
    if (protoScenario == null)
      return (ScenarioDbfRecord) null;
    ScenarioDbfRecord record = new ScenarioDbfRecord();
    record.SetID(protoScenario.Id);
    record.SetNoteDesc(protoScenario.NoteDesc);
    record.SetPlayers(protoScenario.NumHumanPlayers);
    record.SetPlayer1HeroCardId((int) protoScenario.Player1HeroCardId);
    record.SetPlayer2HeroCardId((int) protoScenario.Player2HeroCardId);
    record.SetIsExpert(protoScenario.IsExpert);
    record.SetIsCoop(protoScenario.HasIsCoop && protoScenario.IsCoop);
    record.SetAdventureId(protoScenario.AdventureId);
    if (protoScenario.HasAdventureModeId)
      record.SetModeId(protoScenario.AdventureModeId);
    record.SetWingId(protoScenario.WingId);
    record.SetSortOrder(protoScenario.SortOrder);
    if (protoScenario.HasClientPlayer2HeroCardId)
      record.SetClientPlayer2HeroCardId((int) protoScenario.ClientPlayer2HeroCardId);
    record.SetTbTexture(protoScenario.TavernBrawlTexture);
    record.SetTbTexturePhone(protoScenario.TavernBrawlTexturePhone);
    if (protoScenario.HasTavernBrawlTexturePhoneOffset)
      record.SetTbTexturePhoneOffsetY((double) protoScenario.TavernBrawlTexturePhoneOffset.Y);
    foreach (ScenarioGuestHeroDbRecord guestHero in protoScenario.GuestHeroes)
    {
      ScenarioGuestHeroesDbfRecord guestHeroesDbfRecord = new ScenarioGuestHeroesDbfRecord();
      guestHeroesDbfRecord.SetScenarioId(guestHero.ScenarioId);
      guestHeroesDbfRecord.SetGuestHeroId(guestHero.GuestHeroId);
      guestHeroesDbfRecord.SetSortOrder(guestHero.SortOrder);
      outScenarioGuestHeroRecords.Add(guestHeroesDbfRecord);
    }
    foreach (ClassExclusionDbRecord classExclusion in protoScenario.ClassExclusions)
    {
      ClassExclusionsDbfRecord exclusionsDbfRecord = new ClassExclusionsDbfRecord();
      exclusionsDbfRecord.SetScenarioId(classExclusion.ScenarioId);
      exclusionsDbfRecord.SetClassId(classExclusion.ClassId);
      outClassExclusionsRecords.Add(exclusionsDbfRecord);
    }
    record.SetScriptObject(protoScenario.ScriptObject);
    DbfUtils.AddLocStrings((DbfRecord) record, protoScenario.Strings);
    if (protoScenario.HasDeckRulesetId)
      record.SetDeckRulesetId(protoScenario.DeckRulesetId);
    if (protoScenario.HasRuleType)
    {
      int ruleType = (int) protoScenario.RuleType;
      record.SetRuleType((Scenario.RuleType) ruleType);
    }
    return record;
  }

  public static DeckRulesetDbfRecord ConvertFromProtobuf(
    DeckRulesetDbRecord proto)
  {
    if (proto == null)
      return (DeckRulesetDbfRecord) null;
    DeckRulesetDbfRecord rulesetDbfRecord = new DeckRulesetDbfRecord();
    rulesetDbfRecord.SetID(proto.Id);
    return rulesetDbfRecord;
  }

  public static DeckRulesetRuleDbfRecord ConvertFromProtobuf(
    DeckRulesetRuleDbRecord proto,
    out List<int> outTargetSubsetIds)
  {
    outTargetSubsetIds = (List<int>) null;
    if (proto == null)
      return (DeckRulesetRuleDbfRecord) null;
    DeckRulesetRuleDbfRecord record = new DeckRulesetRuleDbfRecord();
    record.SetID(proto.Id);
    record.SetDeckRulesetId(proto.DeckRulesetId);
    if (proto.HasAppliesToSubsetId)
      record.SetAppliesToSubsetId(proto.AppliesToSubsetId);
    if (proto.HasAppliesToIsNot)
      record.SetAppliesToIsNot(proto.AppliesToIsNot);
    DeckRulesetRule.RuleType v = (DeckRulesetRule.RuleType) Enum.Parse(typeof (DeckRulesetRule.RuleType), proto.RuleType, true);
    record.SetRuleType(v);
    record.SetRuleIsNot(proto.RuleIsNot);
    if (proto.HasMinValue)
      record.SetMinValue(proto.MinValue);
    if (proto.HasMaxValue)
      record.SetMaxValue(proto.MaxValue);
    if (proto.HasTag)
      record.SetTag(proto.Tag);
    if (proto.HasTagMinValue)
      record.SetTagMinValue(proto.TagMinValue);
    if (proto.HasTagMaxValue)
      record.SetTagMaxValue(proto.TagMaxValue);
    if (proto.HasStringValue)
      record.SetStringValue(proto.StringValue);
    record.SetShowInvalidCards(proto.ShowInvalidCards);
    outTargetSubsetIds = proto.TargetSubsetIds;
    DbfUtils.AddLocStrings((DbfRecord) record, proto.Strings);
    return record;
  }

  public static RewardChestDbfRecord ConvertFromProtobuf(
    RewardChestDbRecord proto)
  {
    if (proto == null)
      return (RewardChestDbfRecord) null;
    RewardChestDbfRecord record = new RewardChestDbfRecord();
    record.SetID(proto.Id);
    record.SetShowToReturningPlayer(proto.HasShowToReturningPlayer && proto.ShowToReturningPlayer);
    DbfUtils.AddLocStrings((DbfRecord) record, proto.Strings);
    return record;
  }

  public static GuestHeroDbfRecord ConvertFromProtobuf(GuestHeroDbRecord proto)
  {
    if (proto == null)
      return (GuestHeroDbfRecord) null;
    GuestHeroDbfRecord record = new GuestHeroDbfRecord();
    record.SetID(proto.Id);
    record.SetCardId(proto.CardId);
    record.SetUnlockEvent(DbfShared.GetEventMap().ConvertStringToSpecialEvent(proto.UnlockEvent));
    DbfUtils.AddLocStrings((DbfRecord) record, proto.Strings);
    return record;
  }

  public static DbfLocValue ConvertFromProtobuf(LocalizedString protoLocString)
  {
    DbfLocValue dbfLocValue = new DbfLocValue();
    foreach (LocalizedStringValue localizedStringValue in protoLocString.Values)
      dbfLocValue.SetString((Locale) localizedStringValue.Locale, TextUtils.DecodeWhitespaces(localizedStringValue.Value));
    return dbfLocValue;
  }

  private static void AddLocStrings(DbfRecord record, List<LocalizedString> protoStrings)
  {
    foreach (LocalizedString protoString in protoStrings)
      record.SetVar(protoString.Key, (object) DbfUtils.ConvertFromProtobuf(protoString));
  }
}
