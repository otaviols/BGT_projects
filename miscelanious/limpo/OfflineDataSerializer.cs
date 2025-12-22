using PegasusShared;
using PegasusUtil;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class OfflineDataSerializer
{
  public static IOfflineDataSerializer GetSerializer(int serializerVersion)
  {
    if (serializerVersion == 0)
      return (IOfflineDataSerializer) new OfflineDataSerializer.OfflineDataSerializer_V0Deserializer();
    return serializerVersion == 1 ? (IOfflineDataSerializer) new OfflineDataSerializer.OfflineDataSerializer_V1Deserializer() : (IOfflineDataSerializer) null;
  }

  private static T ReadProtoFromFile<T>(BinaryReader reader) where T : IProtoBuf, new()
  {
    int num = reader.ReadInt32();
    return num == 0 ? default (T) : ProtobufUtil.ParseFrom<T>(reader.ReadBytes(num), length: num);
  }

  private static void AppendProtoToFile(BinaryWriter writer, IProtoBuf packet)
  {
    if (packet == null)
    {
      writer.Write(0);
    }
    else
    {
      byte[] byteArray = ProtobufUtil.ToByteArray(packet);
      writer.Write(byteArray.Length);
      writer.Write(byteArray);
    }
  }

  private abstract class OfflineDataSerializerBase : IOfflineDataSerializer
  {
    public void Serialize(OfflineDataCache.OfflineData data, BinaryWriter writer)
    {
      if (writer == null)
      {
        Debug.LogError((object) "Could not Serialize OfflineData, writer was null");
      }
      else
      {
        writer.Write(data.UniqueFakeDeckId);
        List<long> fakeDeckIds = OfflineDataCache.GetFakeDeckIds(data);
        writer.Write(fakeDeckIds.Count);
        foreach (long num in fakeDeckIds)
          writer.Write(num);
        List<DeckInfo> deckInfoList1 = data.OriginalDeckList == null ? new List<DeckInfo>() : data.OriginalDeckList;
        List<DeckInfo> deckInfoList2 = data.LocalDeckList == null ? new List<DeckInfo>() : data.LocalDeckList;
        writer.Write(deckInfoList1.Count);
        foreach (DeckInfo packet in deckInfoList1)
          OfflineDataSerializer.AppendProtoToFile(writer, (IProtoBuf) packet);
        writer.Write(deckInfoList2.Count);
        foreach (DeckInfo packet in deckInfoList2)
          OfflineDataSerializer.AppendProtoToFile(writer, (IProtoBuf) packet);
        List<PegasusUtil.DeckContents> deckContentsList1 = data.OriginalDeckContents == null ? new List<PegasusUtil.DeckContents>() : data.OriginalDeckContents;
        List<PegasusUtil.DeckContents> deckContentsList2 = data.LocalDeckContents == null ? new List<PegasusUtil.DeckContents>() : data.LocalDeckContents;
        writer.Write(deckContentsList1.Count);
        foreach (PegasusUtil.DeckContents packet in deckContentsList1)
          OfflineDataSerializer.AppendProtoToFile(writer, (IProtoBuf) packet);
        writer.Write(deckContentsList2.Count);
        foreach (PegasusUtil.DeckContents packet in deckContentsList2)
          OfflineDataSerializer.AppendProtoToFile(writer, (IProtoBuf) packet);
        List<FavoriteHero> favoriteHeroList = data.FavoriteHeroes == null ? new List<FavoriteHero>() : data.FavoriteHeroes;
        writer.Write(favoriteHeroList.Count);
        foreach (FavoriteHero packet in favoriteHeroList)
          OfflineDataSerializer.AppendProtoToFile(writer, (IProtoBuf) packet);
        writer.Write(data.m_hasChangedFavoriteHeroesOffline);
        OfflineDataSerializer.AppendProtoToFile(writer, (IProtoBuf) data.CardBacks);
        writer.Write(data.m_hasChangedCardBacksOffline);
        OfflineDataSerializer.AppendProtoToFile(writer, (IProtoBuf) data.Collection);
      }
    }

    public abstract OfflineDataCache.OfflineData Deserialize(BinaryReader reader);
  }

  private class OfflineDataSerializer_V0Deserializer : 
    OfflineDataSerializer.OfflineDataSerializerBase
  {
    public override OfflineDataCache.OfflineData Deserialize(BinaryReader reader)
    {
      if (reader == null)
      {
        Debug.LogError((object) "Could not Deserialize v0 OfflineData, reader was null");
        return (OfflineDataCache.OfflineData) null;
      }
      OfflineDataCache.OfflineData offlineData = new OfflineDataCache.OfflineData();
      offlineData.UniqueFakeDeckId = reader.ReadInt32();
      int num1 = reader.ReadInt32();
      offlineData.FakeDeckIds = new List<long>();
      for (int index = 0; index < num1; ++index)
        offlineData.FakeDeckIds.Add(reader.ReadInt64());
      int num2 = reader.ReadInt32();
      offlineData.OriginalDeckList = new List<DeckInfo>();
      for (int index = 0; index < num2; ++index)
      {
        DeckInfo deckInfo = OfflineDataSerializer.ReadProtoFromFile<DeckInfo>(reader);
        offlineData.OriginalDeckList.Add(deckInfo);
      }
      int num3 = reader.ReadInt32();
      offlineData.LocalDeckList = new List<DeckInfo>();
      for (int index = 0; index < num3; ++index)
      {
        DeckInfo deckInfo = OfflineDataSerializer.ReadProtoFromFile<DeckInfo>(reader);
        offlineData.LocalDeckList.Add(deckInfo);
      }
      int num4 = reader.ReadInt32();
      offlineData.OriginalDeckContents = new List<PegasusUtil.DeckContents>();
      for (int index = 0; index < num4; ++index)
      {
        PegasusUtil.DeckContents deckContents = OfflineDataSerializer.ReadProtoFromFile<PegasusUtil.DeckContents>(reader);
        offlineData.OriginalDeckContents.Add(deckContents);
      }
      int num5 = reader.ReadInt32();
      offlineData.LocalDeckContents = new List<PegasusUtil.DeckContents>();
      for (int index = 0; index < num5; ++index)
      {
        PegasusUtil.DeckContents deckContents = OfflineDataSerializer.ReadProtoFromFile<PegasusUtil.DeckContents>(reader);
        offlineData.LocalDeckContents.Add(deckContents);
      }
      int num6 = reader.ReadInt32();
      offlineData.FavoriteHeroes = new List<FavoriteHero>();
      for (int index = 0; index < num6; ++index)
      {
        FavoriteHero favoriteHero = OfflineDataSerializer.ReadProtoFromFile<FavoriteHero>(reader);
        offlineData.FavoriteHeroes.Add(favoriteHero);
      }
      offlineData.m_hasChangedFavoriteHeroesOffline = reader.ReadBoolean();
      offlineData.CardBacks = OfflineDataSerializer.ReadProtoFromFile<CardBacks>(reader);
      offlineData.m_hasChangedCardBacksOffline = reader.ReadBoolean();
      return offlineData;
    }
  }

  private class OfflineDataSerializer_V1Deserializer : 
    OfflineDataSerializer.OfflineDataSerializerBase
  {
    public override OfflineDataCache.OfflineData Deserialize(BinaryReader reader)
    {
      if (reader == null)
      {
        Debug.LogError((object) "Could not Deserialize v10 OfflineData, reader was null");
        return (OfflineDataCache.OfflineData) null;
      }
      OfflineDataCache.OfflineData offlineData = new OfflineDataCache.OfflineData();
      offlineData.UniqueFakeDeckId = reader.ReadInt32();
      int num1 = reader.ReadInt32();
      offlineData.FakeDeckIds = new List<long>();
      for (int index = 0; index < num1; ++index)
        offlineData.FakeDeckIds.Add(reader.ReadInt64());
      int num2 = reader.ReadInt32();
      offlineData.OriginalDeckList = new List<DeckInfo>();
      for (int index = 0; index < num2; ++index)
      {
        DeckInfo deckInfo = OfflineDataSerializer.ReadProtoFromFile<DeckInfo>(reader);
        offlineData.OriginalDeckList.Add(deckInfo);
      }
      int num3 = reader.ReadInt32();
      offlineData.LocalDeckList = new List<DeckInfo>();
      for (int index = 0; index < num3; ++index)
      {
        DeckInfo deckInfo = OfflineDataSerializer.ReadProtoFromFile<DeckInfo>(reader);
        offlineData.LocalDeckList.Add(deckInfo);
      }
      int num4 = reader.ReadInt32();
      offlineData.OriginalDeckContents = new List<PegasusUtil.DeckContents>();
      for (int index = 0; index < num4; ++index)
      {
        PegasusUtil.DeckContents deckContents = OfflineDataSerializer.ReadProtoFromFile<PegasusUtil.DeckContents>(reader);
        offlineData.OriginalDeckContents.Add(deckContents);
      }
      int num5 = reader.ReadInt32();
      offlineData.LocalDeckContents = new List<PegasusUtil.DeckContents>();
      for (int index = 0; index < num5; ++index)
      {
        PegasusUtil.DeckContents deckContents = OfflineDataSerializer.ReadProtoFromFile<PegasusUtil.DeckContents>(reader);
        offlineData.LocalDeckContents.Add(deckContents);
      }
      int num6 = reader.ReadInt32();
      offlineData.FavoriteHeroes = new List<FavoriteHero>();
      for (int index = 0; index < num6; ++index)
      {
        FavoriteHero favoriteHero = OfflineDataSerializer.ReadProtoFromFile<FavoriteHero>(reader);
        offlineData.FavoriteHeroes.Add(favoriteHero);
      }
      offlineData.m_hasChangedFavoriteHeroesOffline = reader.ReadBoolean();
      offlineData.CardBacks = OfflineDataSerializer.ReadProtoFromFile<CardBacks>(reader);
      offlineData.m_hasChangedCardBacksOffline = reader.ReadBoolean();
      offlineData.Collection = OfflineDataSerializer.ReadProtoFromFile<Collection>(reader);
      return offlineData;
    }
  }
}
