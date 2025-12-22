using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using Hearthstone.Util;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DownloadableDbfCache : IService
{
  private Map<int, KeyValuePair<AssetRecordInfo, DownloadableDbfCache.LoadCachedAssetCallback>> m_assetRequests = new Map<int, KeyValuePair<AssetRecordInfo, DownloadableDbfCache.LoadCachedAssetCallback>>();
  private HashSet<AssetKey> m_requiredClientStaticAssetsStillPending = new HashSet<AssetKey>();
  private int m_nextCallbackToken = -1;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DownloadableDbfCache downloadableDbfCache = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    serviceLocator.Get<Network>().RegisterNetHandler((object) GetAssetResponse.PacketID.ID, new Network.NetHandler(downloadableDbfCache.Network_OnGetAssetResponse));
    serviceLocator.Get<NetCache>().RegisterUpdatedListener(typeof (ClientStaticAssetsResponse), new Action(downloadableDbfCache.NetCache_OnClientStaticAssetsResponse));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (Network),
    typeof (NetCache)
  };

  public void Shutdown()
  {
  }

  public static DownloadableDbfCache Get() => ServiceManager.Get<DownloadableDbfCache>();

  public bool IsAssetRequestInProgress(int assetId, AssetType assetType) => this.m_assetRequests.Any<KeyValuePair<int, KeyValuePair<AssetRecordInfo, DownloadableDbfCache.LoadCachedAssetCallback>>>((Func<KeyValuePair<int, KeyValuePair<AssetRecordInfo, DownloadableDbfCache.LoadCachedAssetCallback>>, bool>) (kv => kv.Value.Key.Asset.AssetId == assetId && kv.Value.Key.Asset.Type == assetType));

  public bool IsRequiredClientStaticAssetsStillPending => NetCache.Get().GetNetObject<ClientStaticAssetsResponse>() == null || this.m_requiredClientStaticAssetsStillPending.Count > 0;

  public bool LoadCachedAssets(
    bool canRequestFromServer,
    DownloadableDbfCache.LoadCachedAssetCallback cb,
    params AssetRecordInfo[] assets)
  {
    if (assets.Length == 0)
      return false;
    List<AssetKey> requestKeys = new List<AssetKey>();
    byte[] assetBytes = (byte[]) null;
    foreach (AssetRecordInfo asset in assets)
    {
      if (asset != null)
      {
        if (asset.RecordHash == null)
        {
          if (asset.RecordByteSize == 0U)
            requestKeys.Add(asset.Asset);
        }
        else
        {
          bool flag = false;
          string cachedAssetFilePath = DownloadableDbfCache.GetCachedAssetFilePath(asset.Asset.Type, asset.Asset.AssetId, asset.RecordHash);
          if (!File.Exists(cachedAssetFilePath))
          {
            flag = asset.RecordByteSize > 0U;
            if (!flag)
              this.m_requiredClientStaticAssetsStillPending.Remove(asset.Asset);
            try
            {
              Directory.CreateDirectory(DownloadableDbfCache.GetCachedAssetFolder(asset.Asset.Type));
            }
            catch (Exception ex)
            {
              Error.AddDevFatal("Error creating cached asset folder {0}:\n{1}", (object) cachedAssetFilePath, (object) ex.ToString());
              return false;
            }
          }
          else
          {
            try
            {
              if (new FileInfo(cachedAssetFilePath).Length != (long) asset.RecordByteSize)
                flag = true;
              else if (asset.RecordByteSize != 0U)
              {
                byte[] numArray = File.ReadAllBytes(cachedAssetFilePath);
                if (GeneralUtils.AreArraysEqual<byte>(System.Security.Cryptography.SHA1.Create().ComputeHash(numArray, 0, numArray.Length), asset.RecordHash))
                {
                  Log.Downloader.Print("LoadCachedAsset: locally available=true {0} id={1} hash={2}", (object) asset.Asset.Type, (object) asset.Asset.AssetId, asset.RecordHash == null ? (object) "<null>" : (object) asset.RecordHash.ToHexString());
                  if (assetBytes == null)
                    assetBytes = numArray;
                  DownloadableDbfCache.SetCachedAssetIntoDbfSystem(asset.Asset.Type, numArray);
                  this.m_requiredClientStaticAssetsStillPending.Remove(asset.Asset);
                }
                else
                  flag = true;
              }
            }
            catch (Exception ex)
            {
              Error.AddDevFatal("Error reading cached asset folder {0}:\n{1}", (object) cachedAssetFilePath, (object) ex.ToString());
              requestKeys.Add(asset.Asset);
            }
          }
          if (flag)
          {
            requestKeys.Add(asset.Asset);
            if (canRequestFromServer)
              Log.Downloader.Print("LoadCachedAsset: locally available=false, requesting from server {0} id={1} hash={2}", (object) asset.Asset.Type, (object) asset.Asset.AssetId, asset.RecordHash == null ? (object) "<null>" : (object) asset.RecordHash.ToHexString());
            else
              Log.Downloader.Print("LoadCachedAsset: locally available=false, not requesting from server yet - {0} id={1} hash={2}", (object) asset.Asset.Type, (object) asset.Asset.AssetId, asset.RecordHash == null ? (object) "<null>" : (object) asset.RecordHash.ToHexString());
          }
        }
      }
    }
    AssetRecordInfo asset1 = assets[0];
    if (requestKeys.Count > 0)
    {
      if (canRequestFromServer)
      {
        int nextCallbackToken = this.NextCallbackToken;
        if (cb != null)
          this.m_assetRequests[nextCallbackToken] = new KeyValuePair<AssetRecordInfo, DownloadableDbfCache.LoadCachedAssetCallback>(asset1, cb);
        Network.Get().SendAssetRequest(nextCallbackToken, requestKeys);
      }
    }
    else if (asset1 != null && cb != null)
    {
      if (assetBytes == null)
        assetBytes = new byte[0];
      cb(asset1.Asset, PegasusShared.ErrorCode.ERROR_OK, assetBytes);
    }
    return requestKeys.Count == 0;
  }

  private static string GetCachedAssetFolder(AssetType assetType)
  {
    string str;
    switch (assetType)
    {
      case AssetType.ASSET_TYPE_SCENARIO:
        str = "Scenario";
        break;
      case AssetType.ASSET_TYPE_SUBSET_CARD:
        str = "Subset";
        break;
      case AssetType.ASSET_TYPE_DECK_RULESET:
        str = "DeckRuleset";
        break;
      default:
        str = "Other";
        break;
    }
    return string.Format("{0}/{1}", (object) PlatformFilePaths.CachePath, (object) str);
  }

  private static string GetCachedAssetFileExtension(AssetType assetType)
  {
    switch (assetType)
    {
      case AssetType.ASSET_TYPE_SCENARIO:
        return "scen";
      case AssetType.ASSET_TYPE_SUBSET_CARD:
        return "subset_card";
      case AssetType.ASSET_TYPE_DECK_RULESET:
        return "deck_ruleset";
      default:
        return assetType.ToString().Replace("ASSET_TYPE_", "").ToLower();
    }
  }

  private static string GetCachedAssetFilePath(AssetType assetType, int assetId, byte[] assetHash)
  {
    string cachedAssetFolder = DownloadableDbfCache.GetCachedAssetFolder(assetType);
    string assetFileExtension = DownloadableDbfCache.GetCachedAssetFileExtension(assetType);
    return string.Format("{0}/{1}_{2}.{3}", (object) cachedAssetFolder, (object) assetId, (object) assetHash.ToHexString(), (object) assetFileExtension);
  }

  private static void StoreReceivedAssetIntoLocalCache(
    AssetType assetType,
    int assetId,
    byte[] assetBytes,
    int assetBytesLength)
  {
    byte[] hash = System.Security.Cryptography.SHA1.Create().ComputeHash(assetBytes, 0, assetBytesLength);
    string cachedAssetFilePath = DownloadableDbfCache.GetCachedAssetFilePath(assetType, assetId, hash);
    try
    {
      if (!File.Exists(cachedAssetFilePath))
        File.Create(cachedAssetFilePath).Dispose();
      using (FileStream fileStream = new FileStream(cachedAssetFilePath, FileMode.Truncate))
        fileStream.Write(assetBytes, 0, assetBytesLength);
    }
    catch (Exception ex)
    {
      Error.AddDevFatal("Error saving cached asset {0}:\n{1}", (object) cachedAssetFilePath, (object) ex.ToString());
    }
  }

  private static void SetCachedAssetIntoDbfSystem(AssetType assetType, byte[] assetBytes)
  {
    switch (assetType)
    {
      case AssetType.ASSET_TYPE_SCENARIO:
        DownloadableDbfCache.SetCachedAssetIntoDbfSystem_Scenario(ProtobufUtil.ParseFrom<ScenarioDbRecord>(assetBytes, length: assetBytes.Length));
        break;
      case AssetType.ASSET_TYPE_SUBSET_CARD:
        DownloadableDbfCache.SetCachedAssetIntoDbfSystem_SubsetCard(ProtobufUtil.ParseFrom<SubsetCardListDbRecord>(assetBytes, length: assetBytes.Length));
        break;
      case AssetType.ASSET_TYPE_DECK_RULESET:
        DownloadableDbfCache.SetCachedAssetIntoDbfSystem_DeckRuleset(ProtobufUtil.ParseFrom<DeckRulesetDbRecord>(assetBytes, length: assetBytes.Length));
        break;
      case AssetType.ASSET_TYPE_REWARD_CHEST:
        DownloadableDbfCache.SetCachedAssetIntoDbfSystem_RewardChest(ProtobufUtil.ParseFrom<RewardChestDbRecord>(assetBytes, length: assetBytes.Length));
        break;
      case AssetType.ASSET_TYPE_GUEST_HEROES:
        DownloadableDbfCache.SetCachedAssetIntoDbfSystem_GuestHero(ProtobufUtil.ParseFrom<GuestHeroDbRecord>(assetBytes, length: assetBytes.Length));
        break;
      default:
        Debug.LogError((object) ("DownloadableDbfCache:SetCachedAssetIntoDbfSystem received an unsupported asset type: " + assetType.ToString()));
        break;
    }
  }

  private static void SetCachedAssetIntoDbfSystem_Scenario(ScenarioDbRecord protoScenario)
  {
    List<ScenarioGuestHeroesDbfRecord> outScenarioGuestHeroRecords;
    List<ClassExclusionsDbfRecord> outClassExclusionsRecords;
    ScenarioDbfRecord record1 = DbfUtils.ConvertFromProtobuf(protoScenario, out outScenarioGuestHeroRecords, out outClassExclusionsRecords);
    if (record1 == null)
    {
      Log.Downloader.Print("DbfUtils.ConvertFromProtobuf(protoScenario) returned null:\n{0}", protoScenario == null ? (object) "(null)" : (object) protoScenario.ToString());
    }
    else
    {
      GameDbf.Scenario.ReplaceRecordByRecordId(record1);
      int dbfId = record1.ID;
      GameDbf.ScenarioGuestHeroes.RemoveRecordsWhere((Predicate<ScenarioGuestHeroesDbfRecord>) (r => r.ScenarioId == dbfId));
      foreach (ScenarioGuestHeroesDbfRecord record2 in outScenarioGuestHeroRecords)
        GameDbf.ScenarioGuestHeroes.AddRecord((DbfRecord) record2);
      GameDbf.ClassExclusions.RemoveRecordsWhere((Predicate<ClassExclusionsDbfRecord>) (r => r.ScenarioId == dbfId));
      foreach (ClassExclusionsDbfRecord record3 in outClassExclusionsRecords)
        GameDbf.ClassExclusions.AddRecord((DbfRecord) record3);
    }
  }

  private static void SetCachedAssetIntoDbfSystem_DeckRuleset(DeckRulesetDbRecord proto)
  {
    DeckRulesetDbfRecord record1 = DbfUtils.ConvertFromProtobuf(proto);
    if (record1 == null)
      Log.Downloader.Print("DbfUtils.ConvertFromProtobuf(proto) returned null:\n{0}", proto == null ? (object) "(null)" : (object) proto.ToString());
    else
      GameDbf.DeckRuleset.ReplaceRecordByRecordId(record1);
    foreach (DeckRulesetRuleDbRecord rule in proto.Rules)
    {
      List<int> intList;
      ref List<int> local = ref intList;
      DeckRulesetRuleDbfRecord record2 = DbfUtils.ConvertFromProtobuf(rule, out local);
      GameDbf.DeckRulesetRule.ReplaceRecordByRecordId(record2);
      int dbfRuleID = record2.ID;
      GameDbf.DeckRulesetRuleSubset.RemoveRecordsWhere((Predicate<DeckRulesetRuleSubsetDbfRecord>) (r => r.DeckRulesetRuleId == dbfRuleID));
      if (intList != null)
      {
        for (int index = 0; index < intList.Count; ++index)
        {
          DeckRulesetRuleSubsetDbfRecord record3 = new DeckRulesetRuleSubsetDbfRecord();
          record3.SetDeckRulesetRuleId(dbfRuleID);
          record3.SetSubsetId(intList[index]);
          GameDbf.DeckRulesetRuleSubset.AddRecord((DbfRecord) record3);
        }
      }
    }
  }

  private static void SetCachedAssetIntoDbfSystem_SubsetCard(SubsetCardListDbRecord proto)
  {
    SubsetDbfRecord record1 = GameDbf.Subset.GetRecord(proto.SubsetId);
    if (record1 == null)
    {
      record1 = new SubsetDbfRecord();
      record1.SetID(proto.SubsetId);
      GameDbf.Subset.AddRecord((DbfRecord) record1);
    }
    int dbfID = record1.ID;
    GameDbf.SubsetCard.RemoveRecordsWhere((Predicate<SubsetCardDbfRecord>) (r => r.SubsetId == dbfID));
    foreach (int cardId in proto.CardIds)
    {
      SubsetCardDbfRecord record2 = new SubsetCardDbfRecord();
      record2.SetSubsetId(dbfID);
      record2.SetCardId(cardId);
      GameDbf.SubsetCard.AddRecord((DbfRecord) record2);
    }
  }

  private static void SetCachedAssetIntoDbfSystem_RewardChest(RewardChestDbRecord proto)
  {
    RewardChestDbfRecord record = DbfUtils.ConvertFromProtobuf(proto);
    if (record == null)
      Log.Downloader.Print("DbfUtils.ConvertFromProtobuf(RewardChestDbRecord) returned null:\n{0}", proto == null ? (object) "(null)" : (object) proto.ToString());
    else
      GameDbf.RewardChest.ReplaceRecordByRecordId(record);
  }

  private static void SetCachedAssetIntoDbfSystem_GuestHero(GuestHeroDbRecord proto)
  {
    GuestHeroDbfRecord record = DbfUtils.ConvertFromProtobuf(proto);
    if (record == null)
      Log.Downloader.Print("DbfUtils.ConvertFromProtobuf(GuestHeroDbfRecord) returned null:\n{0}", proto == null ? (object) "(null)" : (object) proto.ToString());
    else
      GameDbf.GuestHero.ReplaceRecordByRecordId(record);
  }

  private void NetCache_OnClientStaticAssetsResponse()
  {
    ClientStaticAssetsResponse netObject = NetCache.Get().GetNetObject<ClientStaticAssetsResponse>();
    if (netObject == null)
      return;
    foreach (AssetRecordInfo assetRecordInfo in netObject.AssetsToGet)
      this.m_requiredClientStaticAssetsStillPending.Add(assetRecordInfo.Asset);
    this.LoadCachedAssets(true, (DownloadableDbfCache.LoadCachedAssetCallback) null, netObject.AssetsToGet.ToArray());
  }

  private void Network_OnGetAssetResponse()
  {
    GetAssetResponse assetResponse = Network.Get().GetAssetResponse();
    if (assetResponse == null)
      return;
    PegasusShared.ErrorCode code = PegasusShared.ErrorCode.ERROR_OK;
    Map<AssetKey, byte[]> map = new Map<AssetKey, byte[]>();
    for (int index = 0; index < assetResponse.Responses.Count; ++index)
    {
      AssetResponse response = assetResponse.Responses[index];
      if (response.ErrorCode == PegasusShared.ErrorCode.ERROR_OK)
      {
        this.m_requiredClientStaticAssetsStillPending.Remove(response.RequestedKey);
      }
      else
      {
        Log.Downloader.Print("Network_OnGetAssetResponse: error={0}:{1} type={2}:{3} id={4}", (object) (int) response.ErrorCode, (object) response.ErrorCode.ToString(), (object) (int) response.RequestedKey.Type, (object) response.RequestedKey.Type.ToString(), (object) response.RequestedKey.AssetId);
        if (code == PegasusShared.ErrorCode.ERROR_OK)
          code = response.ErrorCode;
        if (this.m_requiredClientStaticAssetsStillPending.Contains(response.RequestedKey))
        {
          Error.AddDevFatal(GameStrings.Get("GLUE_REQUIRED_CLIENT_STATIC_ASSETS_ERROR_MESSAGE"));
          return;
        }
      }
      AssetKey requestedKey = response.RequestedKey;
      byte[] assetBytes = (byte[]) null;
      if (response.HasScenarioAsset)
        assetBytes = ProtobufUtil.ToByteArray((IProtoBuf) response.ScenarioAsset);
      if (response.HasSubsetCardListAsset)
        assetBytes = ProtobufUtil.ToByteArray((IProtoBuf) response.SubsetCardListAsset);
      if (response.HasDeckRulesetAsset)
        assetBytes = ProtobufUtil.ToByteArray((IProtoBuf) response.DeckRulesetAsset);
      if (response.HasRewardChestAsset)
        assetBytes = ProtobufUtil.ToByteArray((IProtoBuf) response.RewardChestAsset);
      if (response.HasGuestHeroAsset)
        assetBytes = ProtobufUtil.ToByteArray((IProtoBuf) response.GuestHeroAsset);
      if (assetBytes != null)
      {
        map[requestedKey] = assetBytes;
        DownloadableDbfCache.StoreReceivedAssetIntoLocalCache(requestedKey.Type, requestedKey.AssetId, assetBytes, assetBytes.Length);
        DownloadableDbfCache.SetCachedAssetIntoDbfSystem(requestedKey.Type, assetBytes);
      }
    }
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(DownloadableDbfCache.PruneCachedAssetFiles));
    Processor.ScheduleCallback(5f, true, new Processor.ScheduledCallback(DownloadableDbfCache.PruneCachedAssetFiles));
    KeyValuePair<AssetRecordInfo, DownloadableDbfCache.LoadCachedAssetCallback> keyValuePair;
    if (!this.m_assetRequests.TryGetValue(assetResponse.ClientToken, out keyValuePair))
      return;
    AssetRecordInfo key = keyValuePair.Key;
    DownloadableDbfCache.LoadCachedAssetCallback cb = keyValuePair.Value;
    this.m_assetRequests.Remove(assetResponse.ClientToken);
    byte[] assetBytes1;
    if (!map.TryGetValue(key.Asset, out assetBytes1))
    {
      if (this.LoadCachedAssets(false, cb, key))
        return;
      assetBytes1 = new byte[0];
    }
    cb(key.Asset, code, assetBytes1);
  }

  private static void PruneCachedAssetFiles(object userData)
  {
    string cachePath = PlatformFilePaths.CachePath;
    string message = (string) null;
    string str = (string) null;
    try
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(cachePath);
      if (!directoryInfo.Exists)
        return;
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
      {
        message = directory.FullName;
        foreach (FileInfo file in directory.GetFiles())
        {
          str = file.Name;
          TimeSpan timeSpan = DateTime.Now - file.LastWriteTime;
          if (file.LastWriteTime < DateTime.Now && timeSpan.TotalDays > 124.0)
            file.Delete();
        }
      }
    }
    catch (Exception ex)
    {
      Error.AddDevWarning("Error pruning dir={0} file={1}:\n{2}", message, (object) str, (object) ex.ToString());
    }
  }

  private int NextCallbackToken => ++this.m_nextCallbackToken;

  public delegate void LoadCachedAssetCallback(
    AssetKey requestedKey,
    PegasusShared.ErrorCode code,
    byte[] assetBytes);
}
