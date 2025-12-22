using Assets;
using Hearthstone;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusLettuce;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LettuceVillageDataUtil
{
  private static readonly AssetReference TASK_TOAST_PREFAB = new AssetReference("LettuceTaskItemToast.prefab:4bd9a9a0603657a4d8948dfae543dcd4");
  public static int CurrentTaskContext;
  public static int RecentlyClaimedTaskId = 0;
  public static bool ZoneWasRecentlyUnlocked = false;
  private static DateTime m_LastRefreshTime;
  private static int m_prevPackCount = 0;

  public static bool Initialized
  {
    get
    {
      NetCache.NetCacheMercenariesVillageInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>();
      return netObject != null && netObject.Initialized;
    }
  }

  public static List<MercenariesTaskState> GetTaskStates()
  {
    NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
    List<MercenariesTaskState> taskStates = new List<MercenariesTaskState>();
    if (netObject != null)
    {
      foreach (MercenariesVisitorState visitorState in netObject.VisitorStates)
        taskStates.Add(visitorState.ActiveTaskState);
    }
    return taskStates;
  }

  public static List<MercenariesVisitorState> VisitorStates
  {
    get
    {
      NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
      return netObject != null ? netObject.VisitorStates : new List<MercenariesVisitorState>();
    }
  }

  public static List<MercenariesCompletedVisitorState> CompletedVisitorStates
  {
    get
    {
      NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
      return netObject != null ? netObject.CompletedVisitorStates : new List<MercenariesCompletedVisitorState>();
    }
  }

  public static int[] VisitingMercenaries
  {
    get
    {
      NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
      return netObject != null ? netObject.VisitingMercenaries : new int[0];
    }
  }

  public static List<MercenariesBuildingState> BuildingStates
  {
    get
    {
      NetCache.NetCacheMercenariesVillageInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>();
      return netObject != null ? netObject.BuildingStates : new List<MercenariesBuildingState>();
    }
  }

  public static List<MercenariesRenownOfferData> ActiveRenownStates
  {
    get
    {
      NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
      return netObject != null ? netObject.ActiveRenownOffers : new List<MercenariesRenownOfferData>();
    }
  }

  public static void InitializeData()
  {
    NetCache.NetCacheMercenariesVillageInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>();
    if (netObject != null && netObject.Initialized)
      return;
    Network.Get().MercenariesVillageStatusRequest();
  }

  public static void RefreshDataIfNecessary()
  {
    if (DateTime.UtcNow.CompareTo(LettuceVillageDataUtil.m_LastRefreshTime.AddSeconds(60.0)) < 1)
      return;
    LettuceVillageDataUtil.RefreshData();
  }

  public static void RefreshData()
  {
    Network.Get().MercenariesVisitorRefreshRequest();
    LettuceVillageDataUtil.m_LastRefreshTime = DateTime.UtcNow;
  }

  public static VisitorTaskDbfRecord GetTaskRecordByID(int taskId) => GameDbf.VisitorTask.GetRecord(taskId);

  public static MercenariesTaskState GetTaskStateByID(int taskId)
  {
    foreach (MercenariesTaskState taskState in LettuceVillageDataUtil.GetTaskStates())
    {
      if (taskState.TaskId == taskId)
        return taskState;
    }
    return (MercenariesTaskState) null;
  }

  public static MercenaryVisitorDbfRecord GetVisitorRecordByID(
    int visitorId)
  {
    return GameDbf.MercenaryVisitor.GetRecord(visitorId);
  }

  public static MercenariesVisitorState GetVisitorStateByID(int visitorId)
  {
    foreach (MercenariesVisitorState visitorState in LettuceVillageDataUtil.VisitorStates)
    {
      if (visitorState.VisitorId == visitorId)
        return visitorState;
    }
    return (MercenariesVisitorState) null;
  }

  public static MercenaryVisitorDbfRecord GetVisitorDbfRecordByMercenaryId(
    int mercenaryId)
  {
    return GameDbf.MercenaryVisitor.GetRecord((Predicate<MercenaryVisitorDbfRecord>) (r => r.MercenaryId == mercenaryId));
  }

  public static VisitorTaskChainDbfRecord GetVisitorTaskChainByID(
    int taskChainId)
  {
    return GameDbf.VisitorTaskChain.GetRecord(taskChainId);
  }

  public static VisitorTaskChainDbfRecord GetCurrentTaskChainByVisitorState(
    MercenariesVisitorState visitorState)
  {
    if (visitorState == null)
      return (VisitorTaskChainDbfRecord) null;
    return visitorState.ActiveTaskState == null ? (VisitorTaskChainDbfRecord) null : LettuceVillageDataUtil.GetVisitorTaskChain(LettuceVillageDataUtil.GetVisitorRecordByID(visitorState.VisitorId));
  }

  public static VisitorTaskChainDbfRecord GetVisitorTaskChain(
    MercenaryVisitorDbfRecord visitorRecord)
  {
    return visitorRecord == null || visitorRecord.VisitorTaskChains == null || visitorRecord.VisitorTaskChains.Count == 0 ? (VisitorTaskChainDbfRecord) null : visitorRecord.VisitorTaskChains[visitorRecord.VisitorTaskChains.Count - 1];
  }

  public static bool IsBountyTutorial(LettuceBountyDbfRecord bountyRecord) => bountyRecord != null && bountyRecord.BountySetRecord != null && bountyRecord.BountySetRecord.IsTutorial;

  public static string GetBountyBossName(LettuceBountyDbfRecord bountyRecord)
  {
    int finalBossCardId = bountyRecord.FinalBossCardId;
    return string.IsNullOrWhiteSpace((string) bountyRecord.BossNameOverride) ? DefLoader.Get().GetEntityDef(finalBossCardId).GetName() : bountyRecord.BossNameOverride.GetString();
  }

  public static string GenerateBountyName(
    LettuceBountyDbfRecord bountyRecord,
    bool includeZoneName = true,
    bool includeDifficulty = true)
  {
    if (bountyRecord == null)
      return "";
    string bountyName = (string) null;
    if (bountyRecord.BountyNameOverride != null)
      bountyName = bountyRecord.BountyNameOverride.GetString();
    if (string.IsNullOrEmpty(bountyName) & includeZoneName)
    {
      LettuceBountySetDbfRecord bountySetRecord = bountyRecord.BountySetRecord;
      if (bountySetRecord != null && bountySetRecord.Name != null)
        bountyName = GameStrings.Format("GLUE_LETTUCE_BOUNTY_BOARD_BOUNTY_TITLE", (object) LettuceVillageDataUtil.GetBountyBossName(bountyRecord), (object) bountyRecord.BountySetRecord.Name.GetString());
    }
    if (string.IsNullOrEmpty(bountyName))
      bountyName = LettuceVillageDataUtil.GetBountyBossName(bountyRecord);
    if (includeDifficulty && !string.IsNullOrEmpty(bountyName) && bountyRecord.Heroic)
      bountyName = GameStrings.Format("GLUE_LETTUCE_BOUNTY_BOARD_BOUNTY_TITLE_HEROIC_ADD", (object) bountyName);
    return bountyName;
  }

  public static bool BuildingIsBuilt(MercenariesBuildingState bldg) => NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>().BuildingIsBuilt(bldg);

  public static List<int> GetNextTierListByTierId(int tierId) => NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>().GetNextTierListByTierId(tierId);

  public static BuildingTierDbfRecord GetTierRecordByTierId(int tierId) => GameDbf.BuildingTier.GetRecord(tierId);

  public static BuildingTierDbfRecord GetTierRecordByTierIndex(
    MercenaryBuilding.Mercenarybuildingtype buildingType,
    int tierIndex)
  {
    MercenaryBuildingDbfRecord buildingRecordByType = LettuceVillageDataUtil.GetBuildingRecordByType(buildingType);
    if (buildingRecordByType == null)
      return (BuildingTierDbfRecord) null;
    return tierIndex < buildingRecordByType.MercenaryBuildingTiers.Count ? buildingRecordByType.MercenaryBuildingTiers[tierIndex] : (BuildingTierDbfRecord) null;
  }

  public static MercenaryBuildingDbfRecord GetBuildingRecordByType(
    MercenaryBuilding.Mercenarybuildingtype buildingType)
  {
    return GameDbf.MercenaryBuilding.GetRecord((Predicate<MercenaryBuildingDbfRecord>) (r => r.MercenaryBuildingType == buildingType));
  }

  public static MercenaryBuildingDbfRecord GetBuildingRecordByID(
    int buildingID)
  {
    return GameDbf.MercenaryBuilding.GetRecord((Predicate<MercenaryBuildingDbfRecord>) (r => r.ID == buildingID));
  }

  public static MercenariesBuildingState GetBuildingStateByID(int buildingId)
  {
    foreach (MercenariesBuildingState buildingState in LettuceVillageDataUtil.BuildingStates)
    {
      if (buildingState.BuildingId == buildingId)
        return buildingState;
    }
    return (MercenariesBuildingState) null;
  }

  public static BuildingTierDbfRecord GetCurrentTierRecordFromBuilding(
    MercenaryBuilding.Mercenarybuildingtype buildingType)
  {
    MercenaryBuildingDbfRecord buildingRecordByType = LettuceVillageDataUtil.GetBuildingRecordByType(buildingType);
    if (buildingRecordByType != null)
    {
      MercenariesBuildingState buildingStateById = LettuceVillageDataUtil.GetBuildingStateByID(buildingRecordByType.ID);
      if (buildingStateById != null)
      {
        foreach (BuildingTierDbfRecord mercenaryBuildingTier in buildingRecordByType.MercenaryBuildingTiers)
        {
          if (mercenaryBuildingTier.ID == buildingStateById.CurrentTierId)
            return mercenaryBuildingTier;
        }
      }
    }
    return (BuildingTierDbfRecord) null;
  }

  public static List<BuildingTierDbfRecord> GetTierRecordsThatCanBeBuilt()
  {
    List<BuildingTierDbfRecord> recordsThatCanBeBuilt = new List<BuildingTierDbfRecord>();
    foreach (MercenaryBuilding.Mercenarybuildingtype buildingType in Enum.GetValues(typeof (MercenaryBuilding.Mercenarybuildingtype)))
    {
      MercenaryBuildingDbfRecord buildingRecordByType = LettuceVillageDataUtil.GetBuildingRecordByType(buildingType);
      if (buildingRecordByType != null)
      {
        MercenariesBuildingState buildingStateById = LettuceVillageDataUtil.GetBuildingStateByID(buildingRecordByType.ID);
        if (buildingStateById != null)
        {
          List<BuildingTierDbfRecord> mercenaryBuildingTiers = buildingRecordByType.MercenaryBuildingTiers;
          int num1 = 1000;
          int num2 = 0;
          foreach (BuildingTierDbfRecord buildingTierDbfRecord in mercenaryBuildingTiers)
          {
            if (num2 > num1)
              recordsThatCanBeBuilt.Add(buildingTierDbfRecord);
            if (buildingTierDbfRecord.ID == buildingStateById.CurrentTierId)
              num1 = num2;
            ++num2;
          }
        }
      }
    }
    return recordsThatCanBeBuilt;
  }

  public static bool IsBuildingReadyToUpgrade(
    MercenaryBuilding.Mercenarybuildingtype buildingType,
    int targetTierID = 0)
  {
    MercenaryBuildingDbfRecord buildingRecordByType = LettuceVillageDataUtil.GetBuildingRecordByType(buildingType);
    if (buildingRecordByType == null)
      return false;
    MercenariesBuildingState buildingStateById = LettuceVillageDataUtil.GetBuildingStateByID(buildingRecordByType.ID);
    if (buildingStateById == null)
      return false;
    BuildingTierDbfRecord nextTierRecord = LettuceVillageDataUtil.GetNextTierRecord(LettuceVillageDataUtil.GetTierRecordByTierId(buildingStateById.CurrentTierId));
    return nextTierRecord != null && LettuceVillageDataUtil.IsBuildingTierAchievementComplete(nextTierRecord.ID) && (targetTierID == 0 || nextTierRecord.ID == targetTierID) && NetCache.Get().GetGoldBalance() >= (long) nextTierRecord.UpgradeCost;
  }

  public static int GetCurrentTierPropertyForBuilding(
    MercenaryBuilding.Mercenarybuildingtype buildingType,
    TierProperties.Buildingtierproperty tierProperty,
    BuildingTierDbfRecord buildingTierRecord = null)
  {
    if (buildingTierRecord == null)
      buildingTierRecord = LettuceVillageDataUtil.GetCurrentTierRecordFromBuilding(buildingType);
    if (buildingTierRecord != null)
    {
      foreach (TierPropertiesDbfRecord buildingTierProperty in buildingTierRecord.MercenaryBuildingTierProperties)
      {
        if (buildingTierProperty.TierPropertyType == tierProperty)
          return buildingTierProperty.TierPropertyValue;
      }
    }
    return 0;
  }

  public static BuildingTierDbfRecord GetNextTierRecord(
    BuildingTierDbfRecord currentTierRecord)
  {
    if (currentTierRecord == null)
      return (BuildingTierDbfRecord) null;
    BuildingTierDbfRecord nextTierRecord = (BuildingTierDbfRecord) null;
    List<int> tierListByTierId = LettuceVillageDataUtil.GetNextTierListByTierId(currentTierRecord.ID);
    if (tierListByTierId.Count != 0)
      nextTierRecord = LettuceVillageDataUtil.GetTierRecordByTierId(tierListByTierId[0]);
    return nextTierRecord;
  }

  public static bool TryGetRenownConversionRate(TAG_RARITY rarity, out int conversionRate)
  {
    conversionRate = 0;
    NetCache.NetCacheMercenariesVillageInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>();
    return netObject != null && netObject.TryGetRenownRate(rarity, out conversionRate);
  }

  public static string FormatTaskStringForProceduralData(
    string taskString,
    int mercenaryId,
    int bountyId,
    List<int> additionalMercenaryIds)
  {
    return string.IsNullOrEmpty(taskString) ? string.Empty : LettuceVillageDataUtil.FormatTaskStringForBounty(LettuceVillageDataUtil.FormatTaskStringForMercenaries(taskString, mercenaryId, additionalMercenaryIds), bountyId);
  }

  public static string FormatTaskStringForMercenaries(
    string taskString,
    int mercenaryId,
    List<int> additionalMercenaryIds)
  {
    LettuceMercenaryDbfRecord record1 = GameDbf.LettuceMercenary.GetRecord(mercenaryId);
    if (record1 == null)
      return string.Empty;
    string taskDescription = LettuceVillageDataUtil.SetMercenaryNamesInTaskString("$owner_merc", taskString, new List<LettuceMercenaryDbfRecord>()
    {
      record1
    }, (Func<LettuceMercenaryDbfRecord, CardDbfRecord, string>) ((merc, card) => (string) (string.IsNullOrWhiteSpace((string) card.ShortName) ? card.Name : card.ShortName)));
    List<LettuceMercenaryDbfRecord> mercenaries = new List<LettuceMercenaryDbfRecord>();
    if (additionalMercenaryIds != null)
    {
      foreach (int additionalMercenaryId in additionalMercenaryIds)
      {
        LettuceMercenaryDbfRecord record2 = GameDbf.LettuceMercenary.GetRecord(additionalMercenaryId);
        if (record2 != null)
          mercenaries.Add(record2);
      }
    }
    return LettuceVillageDataUtil.SetMercenaryNamesInTaskString("$additional_mercs", taskDescription, mercenaries, (Func<LettuceMercenaryDbfRecord, CardDbfRecord, string>) ((merc, card) =>
    {
      string str = (string) null;
      if (!string.IsNullOrWhiteSpace((string) card.ShortName))
        str = (string) card.ShortName;
      if (str == null || str.Length == 0)
        str = (string) card.Name;
      return str;
    }));
  }

  public static string FormatTaskStringForBounty(string taskString, int bountyId)
  {
    LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord(bountyId);
    if (record == null)
      return taskString;
    string taskDescription = LettuceVillageDataUtil.SetBountyNameInTaskString("$bounty_n", LettuceVillageDataUtil.SetBountyNameInTaskString("$bounty_nd", LettuceVillageDataUtil.SetBountyNameInTaskString("$bounty_nz", taskString, record, true, false), record, false, true), record, false, false);
    LettuceBountySetDbfRecord bountySetRecord = record.BountySetRecord;
    if (bountySetRecord != null)
      taskDescription = LettuceVillageDataUtil.SetBountySetInTaskString("$bounty_set", taskDescription, bountySetRecord);
    return LettuceVillageDataUtil.SetBountyNameInTaskString("$bounty", LettuceVillageDataUtil.SetBountyDifficultyInTaskString("$bounty_diff", taskDescription, record), record, true, true);
  }

  public static string FormatTaskDescriptionForAbility(
    string taskDescription,
    int quota,
    int mercenaryId)
  {
    return string.IsNullOrEmpty(taskDescription) ? string.Empty : ProgressUtils.FormatDescription(LettuceVillageDataUtil.SetEquipmentNameFromTaskDescription(LettuceVillageDataUtil.SetAbilityNameFromTaskDescription(taskDescription, mercenaryId), mercenaryId), quota);
  }

  public static IEnumerator ShowTaskToast(
    List<MercenariesTaskState> CompletedTasks,
    bool useGeneric = false)
  {
    int maxDisplayed;
    Vector3 toastOffset;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      maxDisplayed = 2;
      toastOffset = LettuceVillageTaskToast.TASK_TOAST_OFFSET_PHONE;
    }
    else
    {
      maxDisplayed = 3;
      toastOffset = LettuceVillageTaskToast.TASK_TOAST_OFFSET;
    }
    for (int i = 0; i < CompletedTasks.Count; ++i)
    {
      MercenaryVillageTaskItemDataModel model = (MercenaryVillageTaskItemDataModel) null;
      model = !useGeneric ? LettuceVillageDataUtil.CreateTaskModelFromTaskState(CompletedTasks[i]) : LettuceVillageDataUtil.Dev_CreateGenericTaskModel(CompletedTasks[i].TaskId, CompletedTasks[i].Progress);
      if (model != null)
      {
        WidgetInstance taskItemWidget = WidgetInstance.Create((string) LettuceVillageDataUtil.TASK_TOAST_PREFAB);
        Vector3 vector3 = (float) (i % maxDisplayed) * toastOffset;
        taskItemWidget.gameObject.transform.position = vector3;
        taskItemWidget.RegisterReadyListener((Action<object>) (_ =>
        {
          LettuceVillageTaskToast componentInChildren = taskItemWidget.GetComponentInChildren<LettuceVillageTaskToast>();
          componentInChildren.Initialize(model);
          componentInChildren.Show();
        }), (object) null, true);
      }
      if ((i + 1) % maxDisplayed == 0 || i == CompletedTasks.Count - 1)
        yield return (object) new WaitForSeconds(5f);
    }
  }

  private static string SetMercenaryNamesInTaskString(
    string command,
    string taskDescription,
    List<LettuceMercenaryDbfRecord> mercenaries,
    Func<LettuceMercenaryDbfRecord, CardDbfRecord, string> getNameFromRecordFn)
  {
    Func<string> detailsFn = (Func<string>) (() =>
    {
      List<string> stringList = new List<string>();
      foreach (LettuceMercenaryDbfRecord mercenary in mercenaries)
      {
        if (mercenary.MercenaryArtVariations.Count != 0)
        {
          string str = getNameFromRecordFn(mercenary, mercenary.MercenaryArtVariations[0].CardRecord);
          stringList.Add(str);
        }
      }
      return GameStrings.Format(string.Format("GLUE_MERCENARIES_TASKBOARD_PROC_TASK_MERCENARY_LIST_{0}", (object) stringList.Count), (object[]) stringList.ToArray());
    });
    return LettuceVillageDataUtil.ReplaceCommandWithDetails(command, taskDescription, detailsFn);
  }

  private static string SetBountyNameInTaskString(
    string command,
    string taskDescription,
    LettuceBountyDbfRecord bountyRecord,
    bool includeZoneName,
    bool includeDifficulty)
  {
    return LettuceVillageDataUtil.ReplaceCommandWithDetails(command, taskDescription, (Func<string>) (() => LettuceVillageDataUtil.GenerateBountyName(bountyRecord, includeZoneName, includeDifficulty)));
  }

  private static string SetBountySetInTaskString(
    string command,
    string taskDescription,
    LettuceBountySetDbfRecord bountySetRecord)
  {
    return LettuceVillageDataUtil.ReplaceCommandWithDetails(command, taskDescription, (Func<string>) (() => (string) bountySetRecord.Name));
  }

  private static string SetBountyDifficultyInTaskString(
    string command,
    string taskDescription,
    LettuceBountyDbfRecord bountyRecord)
  {
    Func<string> detailsFn = (Func<string>) (() =>
    {
      if (bountyRecord.DifficultyMode == LettuceBounty.MercenariesBountyDifficulty.HEROIC || bountyRecord.Heroic)
        return GameStrings.Get("GLUE_MERCENARIES_TASKBOARD_PROC_TASK_HEROIC_DIFFICULTY");
      return bountyRecord.DifficultyMode == LettuceBounty.MercenariesBountyDifficulty.NORMAL ? GameStrings.Get("GLUE_MERCENARIES_TASKBOARD_PROC_TASK_NORMAL_DIFFICULTY") : "";
    });
    return LettuceVillageDataUtil.ReplaceCommandWithDetails(command, taskDescription, detailsFn);
  }

  private static string ReplaceCommandWithDetails(
    string command,
    string taskDescription,
    Func<string> detailsFn)
  {
    return Regex.Replace(taskDescription, "\\" + command, detailsFn());
  }

  private static string SetAbilityNameFromTaskDescription(string taskDescription, int mercenaryId)
  {
    int num1 = taskDescription.IndexOf("$ability(");
    if (num1 == -1)
      return taskDescription;
    int startIndex = num1 + "$ability(".Length;
    int num2 = taskDescription.IndexOf(")", startIndex);
    if (num2 == -1)
    {
      Debug.LogError((object) ("Incorrect format for $ability command for " + taskDescription));
      return taskDescription;
    }
    string[] strArray = taskDescription.Substring(startIndex, num2 - startIndex).Split(',');
    int result1 = 0;
    int result2 = 0;
    if (!int.TryParse(strArray[0], out result1) || !int.TryParse(strArray[1], out result2))
    {
      Debug.LogError((object) ("Error in params for task description for" + taskDescription));
      return taskDescription;
    }
    LettuceMercenaryDbfRecord record1 = GameDbf.LettuceMercenary.GetRecord(mercenaryId);
    if (result1 >= record1.LettuceMercenarySpecializations.Count)
    {
      Debug.LogError((object) ("Error in params,invalid specialization slot in task description, received " + (object) result1));
      return taskDescription;
    }
    LettuceMercenarySpecializationDbfRecord mercenarySpecialization = record1.LettuceMercenarySpecializations[result1];
    if (result2 >= mercenarySpecialization.LettuceMercenaryAbilities.Count)
    {
      Debug.LogError((object) ("Error in params,invalid ability slot in task description, received " + (object) result2));
      return taskDescription;
    }
    int lettuceAbilityId = mercenarySpecialization.LettuceMercenaryAbilities[result2].LettuceAbilityId;
    LettuceAbilityDbfRecord record2 = GameDbf.LettuceAbility.GetRecord(lettuceAbilityId);
    return Regex.Replace(taskDescription, "\\$ability(.*?)\\)", (string) record2.AbilityName, RegexOptions.IgnoreCase);
  }

  private static string SetEquipmentNameFromTaskDescription(string taskDescription, int mercenaryId)
  {
    int num1 = taskDescription.IndexOf("$equipment(");
    if (num1 == -1)
      return taskDescription;
    int startIndex = num1 + "$equipment(".Length;
    int num2 = taskDescription.IndexOf(")", startIndex);
    if (num2 == -1)
    {
      Debug.LogError((object) ("Incorrect format for $equipment command for " + taskDescription));
      return taskDescription;
    }
    string[] strArray = taskDescription.Substring(startIndex, num2 - startIndex).Split(',');
    int result1 = 0;
    int result2 = 0;
    if (!int.TryParse(strArray[0], out result1) || !int.TryParse(strArray[1], out result2))
    {
      Debug.LogError((object) ("Error in params for task description for" + taskDescription));
      return taskDescription;
    }
    LettuceMercenaryDbfRecord record1 = GameDbf.LettuceMercenary.GetRecord(mercenaryId);
    if (result1 >= record1.LettuceMercenaryEquipment.Count)
    {
      Debug.LogError((object) ("Error in params,invalid equipment slot in task description, received " + (object) result1));
      return taskDescription;
    }
    LettuceMercenaryEquipmentDbfRecord equipmentDbfRecord = record1.LettuceMercenaryEquipment[result1];
    if (result2 >= equipmentDbfRecord.LettuceEquipmentRecord.LettuceEquipmentTiers.Count)
    {
      Debug.LogError((object) ("Error in params,invalid equipment tier slot in task description, received " + (object) result2));
      return taskDescription;
    }
    int cardId = equipmentDbfRecord.LettuceEquipmentRecord.LettuceEquipmentTiers[result2].CardId;
    CardDbfRecord record2 = GameDbf.Card.GetRecord(cardId);
    return Regex.Replace(taskDescription, "\\$equipment(.*?)\\)", (string) record2.Name, RegexOptions.IgnoreCase);
  }

  public static List<MercenaryBuilding.Mercenarybuildingtype> GetAvailableBuildingsForCurrentFTUEState(
    bool isUIContext = false)
  {
    List<MercenaryBuilding.Mercenarybuildingtype> currentFtueState;
    if (GameUtils.IsMercenariesVillageTutorialComplete())
    {
      currentFtueState = new List<MercenaryBuilding.Mercenarybuildingtype>()
      {
        MercenaryBuilding.Mercenarybuildingtype.PVEZONES,
        MercenaryBuilding.Mercenarybuildingtype.PVP,
        MercenaryBuilding.Mercenarybuildingtype.COLLECTION,
        MercenaryBuilding.Mercenarybuildingtype.SHOP,
        MercenaryBuilding.Mercenarybuildingtype.MAILBOX,
        MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL,
        MercenaryBuilding.Mercenarybuildingtype.TASKBOARD,
        MercenaryBuilding.Mercenarybuildingtype.BUILDINGMANAGER
      };
    }
    else
    {
      currentFtueState = new List<MercenaryBuilding.Mercenarybuildingtype>();
      currentFtueState.Add(MercenaryBuilding.Mercenarybuildingtype.COLLECTION);
      if (!isUIContext)
        currentFtueState.Add(MercenaryBuilding.Mercenarybuildingtype.BUILDINGMANAGER);
      if (LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_END))
        currentFtueState.Add(MercenaryBuilding.Mercenarybuildingtype.TASKBOARD);
      if (LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_TASK_BOARD_END))
        currentFtueState.Add(MercenaryBuilding.Mercenarybuildingtype.PVEZONES);
      if (LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_SHOP_BUILD_START))
        currentFtueState.Add(MercenaryBuilding.Mercenarybuildingtype.SHOP);
      if (LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_SHOP_BUILD_END))
        currentFtueState.Add(MercenaryBuilding.Mercenarybuildingtype.MAILBOX);
    }
    return currentFtueState;
  }

  public static bool IsBuildingAvailableInTutorial(
    int buildingId,
    List<MercenaryBuilding.Mercenarybuildingtype> availableBuildings)
  {
    MercenaryBuildingDbfRecord buildingRecordById = LettuceVillageDataUtil.GetBuildingRecordByID(buildingId);
    return buildingRecordById != null && availableBuildings.Contains(buildingRecordById.MercenaryBuildingType);
  }

  public static bool IsBuildingTierAchievementComplete(int nextTierId)
  {
    BuildingTierDbfRecord tierRecordByTierId = LettuceVillageDataUtil.GetTierRecordByTierId(nextTierId);
    if (tierRecordByTierId == null)
    {
      Debug.LogError((object) "LettuceVillageDataUtil.IsBuildingAchievementComplete: No record exists for the CurrentTierId.");
      return false;
    }
    if (tierRecordByTierId.UnlockAchievement == 0)
      return true;
    AchievementDataModel achievementDataModel = AchievementManager.Get().GetAchievementDataModel(tierRecordByTierId.UnlockAchievement);
    if (achievementDataModel == null)
    {
      Debug.LogError((object) "LettuceVillageDataUtil.IsBuildingAchievementComplete: No record exists for this achievement.");
      return false;
    }
    switch (achievementDataModel.Status)
    {
      case AchievementManager.AchievementStatus.COMPLETED:
      case AchievementManager.AchievementStatus.REWARD_GRANTED:
      case AchievementManager.AchievementStatus.REWARD_ACKED:
        return true;
      default:
        return false;
    }
  }

  public static List<MercenariesVisitorState> GetVisitorsByType(
    MercenaryVisitor.VillageVisitorType visitorType)
  {
    List<MercenariesVisitorState> visitorsByType = new List<MercenariesVisitorState>();
    foreach (MercenariesVisitorState visitorState in LettuceVillageDataUtil.VisitorStates)
    {
      MercenaryVisitorDbfRecord visitorRecordById = LettuceVillageDataUtil.GetVisitorRecordByID(visitorState.VisitorId);
      if (visitorRecordById != null && visitorRecordById.VisitorType == visitorType)
        visitorsByType.Add(visitorState);
    }
    return visitorsByType;
  }

  public static List<MercenariesCompletedVisitorState> GetCompletedVisitorsByType(
    MercenaryVisitor.VillageVisitorType visitorType)
  {
    List<MercenariesCompletedVisitorState> completedVisitorsByType = new List<MercenariesCompletedVisitorState>();
    foreach (MercenariesCompletedVisitorState completedVisitorState in LettuceVillageDataUtil.CompletedVisitorStates)
    {
      MercenaryVisitorDbfRecord visitorRecordById = LettuceVillageDataUtil.GetVisitorRecordByID(completedVisitorState.VisitorId);
      if (visitorRecordById != null && visitorRecordById.VisitorType == visitorType)
        completedVisitorsByType.Add(completedVisitorState);
    }
    return completedVisitorsByType;
  }

  public static int GetNumberOfMercPacksToOpen()
  {
    int ofMercPacksToOpen = 0;
    if (LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_SHOP_CLAIM_PACK_POPUP))
      ofMercPacksToOpen = BoosterPackUtils.GetBoosterCount(629);
    return ofMercPacksToOpen;
  }

  public static bool DidPackCountChangeFromZero(int newCount) => LettuceVillageDataUtil.m_prevPackCount == 0 && newCount > 0;

  public static void UpdatePrevPackCount(int newCount) => LettuceVillageDataUtil.m_prevPackCount = newCount;

  public static bool IsHeroicDifficultyUnlocked()
  {
    NetCache.NetCacheMercenariesVillageInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>();
    return netObject != null && netObject.UnlockedBountyDifficultyLevel >= 2;
  }

  public static GameSaveKeySubkeyId GetNotificationSubkeyIdForBuilding(
    MercenaryBuilding.Mercenarybuildingtype buildingType)
  {
    GameSaveKeySubkeyId subkeyIdForBuilding = GameSaveKeySubkeyId.INVALID;
    switch (buildingType)
    {
      case MercenaryBuilding.Mercenarybuildingtype.TASKBOARD:
        subkeyIdForBuilding = GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_TASKBOARD_NOTIFICATION;
        break;
      case MercenaryBuilding.Mercenarybuildingtype.BUILDINGMANAGER:
        subkeyIdForBuilding = GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_WORKSHOP_NOTIFICATION;
        break;
      case MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL:
        subkeyIdForBuilding = GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_TRAININGHALL_NOTIFICATION;
        break;
      case MercenaryBuilding.Mercenarybuildingtype.PVEZONES:
        subkeyIdForBuilding = GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_PVEZONES_NOTIFICATION;
        break;
      case MercenaryBuilding.Mercenarybuildingtype.PVP:
        subkeyIdForBuilding = GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_ARENA_NOTIFICATION;
        break;
      case MercenaryBuilding.Mercenarybuildingtype.COLLECTION:
        subkeyIdForBuilding = GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_COLLECTION_NOTIFICATION;
        break;
      case MercenaryBuilding.Mercenarybuildingtype.SHOP:
        subkeyIdForBuilding = GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_SHOP_NOTIFICATION;
        break;
    }
    return subkeyIdForBuilding;
  }

  public static bool GetNotificationStatusForBuilding(
    MercenaryBuilding.Mercenarybuildingtype buildingType)
  {
    if (buildingType == MercenaryBuilding.Mercenarybuildingtype.COLLECTION)
      return LettuceVillageDataUtil.GetNotificationStatusForCollectionBuilding();
    GameSaveKeySubkeyId subkeyIdForBuilding = LettuceVillageDataUtil.GetNotificationSubkeyIdForBuilding(buildingType);
    if (subkeyIdForBuilding == GameSaveKeySubkeyId.INVALID)
      return false;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, subkeyIdForBuilding, out num);
    switch (buildingType)
    {
      case MercenaryBuilding.Mercenarybuildingtype.TASKBOARD:
        return num > 0L || LettuceVillageDataUtil.GetNotificationStatusForTaskBoard();
      case MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL:
        return num > 0L || LettuceVillageDataUtil.GetNotificationStatusForTrainingGrounds();
      case MercenaryBuilding.Mercenarybuildingtype.PVEZONES:
        return num > 0L || LettuceVillageDataUtil.GetNotificationStatusForTravelPoint();
      default:
        return num > 0L;
    }
  }

  public static bool GetNotificationStatusForCollectionBuilding() => CollectionManager.Get().DoesAnyMercenaryNeedToBeAcknowledged();

  public static bool GetNotificationStatusForTrainingGrounds() => LettuceVillageDataUtil.IsAnyMercenaryFinishedTraining();

  public static bool GetNotificationStatusForTravelPoint() => LettuceVillageDataUtil.ZoneWasRecentlyUnlocked;

  public static bool GetNotificationStatusForTaskBoard()
  {
    foreach (MercenariesTaskState taskState in LettuceVillageDataUtil.GetTaskStates())
    {
      if (taskState != null && taskState.Status_ == MercenariesTaskState.Status.COMPLETE)
        return true;
    }
    return false;
  }

  public static bool MarkNotificationAsSeenForBuilding(
    MercenaryBuilding.Mercenarybuildingtype buildingType)
  {
    GameSaveKeySubkeyId subkeyIdForBuilding = LettuceVillageDataUtil.GetNotificationSubkeyIdForBuilding(buildingType);
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, subkeyIdForBuilding, out num);
    if (num == 0L || subkeyIdForBuilding == GameSaveKeySubkeyId.INVALID)
      return false;
    switch (buildingType)
    {
      case MercenaryBuilding.Mercenarybuildingtype.PVEZONES:
        LettuceVillageDataUtil.ZoneWasRecentlyUnlocked = false;
        break;
      case MercenaryBuilding.Mercenarybuildingtype.COLLECTION:
        return GameSaveDataManager.Get().SaveSubkeys(new List<GameSaveDataManager.SubkeySaveRequest>()
        {
          new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, subkeyIdForBuilding, new long[1]),
          new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_SHOULD_SEE_ABILITYUPGRADE_NOTIFICATION, new long[1])
        });
    }
    return GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, subkeyIdForBuilding, new long[1]));
  }

  public static void RemoveCompletedOrDismissedTaskDialogue(int taskId)
  {
    VisitorTaskDbfRecord taskRecordById = LettuceVillageDataUtil.GetTaskRecordByID(taskId);
    if (taskRecordById == null || taskRecordById.OnAssignedDialog == 0)
      return;
    GameSaveDataManager.SubkeySaveRequest subkeyIfItExists = GameSaveDataManager.Get().GenerateSaveRequestToRemoveValueFromSubkeyIfItExists(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_VILLAGE_RECENTLY_PLAYED_TASK_DIALOGS, (long) taskRecordById.OnAssignedDialog);
    if (subkeyIfItExists == null)
      return;
    GameSaveDataManager.Get().SaveSubkey(subkeyIfItExists);
  }

  public static MercenaryVillageTaskItemDataModel CreateTaskModelFromActiveVisitorState(
    MercenariesVisitorState visitorState)
  {
    if (visitorState == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageTaskBoard.CreateTaskItemFromVisitorState: visitor state was null");
      return (MercenaryVillageTaskItemDataModel) null;
    }
    MercenariesTaskState activeTaskState = visitorState.ActiveTaskState;
    if (activeTaskState != null)
      return LettuceVillageDataUtil.CreateTaskModelFromTaskState(activeTaskState, visitorState);
    Debug.LogErrorFormat(string.Format("error in LettuceVillageTaskBoard.CreateTaskItemFromVisitorState: active task state null for Visitor id: {0}", (object) visitorState.VisitorId));
    return (MercenaryVillageTaskItemDataModel) null;
  }

  public static bool IsDataEqual(
    MercenariesVisitorState visitorData,
    MercenaryVillageTaskItemDataModel dataModel)
  {
    return visitorData != null && visitorData.ActiveTaskState != null && dataModel.TaskId == visitorData.ActiveTaskState.TaskId && dataModel.Progress == visitorData.ActiveTaskState.Progress && dataModel.TaskStatus == visitorData.ActiveTaskState.Status_;
  }

  public static bool IsDataEqual(
    MercenariesCompletedVisitorState completedVisitorData,
    MercenaryVillageTaskItemDataModel dataModel)
  {
    return completedVisitorData != null && completedVisitorData.CompletedTaskChainId == dataModel.TaskChainId && dataModel.TaskStatus == MercenariesTaskState.Status.CLAIMED;
  }

  public static MercenaryVillageTaskItemDataModel CreateTaskModelFromTaskState(
    MercenariesTaskState taskState,
    MercenariesVisitorState visitorState = null)
  {
    if (taskState == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModelFromTaskState: task state is null");
      return (MercenaryVillageTaskItemDataModel) null;
    }
    VisitorTaskDbfRecord taskRecordById = LettuceVillageDataUtil.GetTaskRecordByID(taskState.TaskId);
    if (taskRecordById == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModelFromTaskState: task record is null for task id {0}", (object) taskState.TaskId);
      return (MercenaryVillageTaskItemDataModel) null;
    }
    if (LettuceVillageDataUtil.GetVisitorRecordByID(taskRecordById.MercenaryVisitorId) == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModelFromTaskState: visitor record is null for mercenary visitor id: {0}", (object) taskRecordById.MercenaryVisitorId);
      return (MercenaryVillageTaskItemDataModel) null;
    }
    if (visitorState == null)
    {
      foreach (MercenariesVisitorState visitorState1 in LettuceVillageDataUtil.VisitorStates)
      {
        if (visitorState1 != null && visitorState1.ActiveTaskState != null && visitorState1.ActiveTaskState.TaskId == taskState.TaskId)
        {
          visitorState = visitorState1;
          break;
        }
      }
    }
    if (visitorState == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModelFromTaskState: unable to find visitor state for visitor id: {0}", (object) taskRecordById.MercenaryVisitorId);
      return (MercenaryVillageTaskItemDataModel) null;
    }
    LettuceVillageDataUtil.AdditionalTaskModelInfo additionalTaskInfo = new LettuceVillageDataUtil.AdditionalTaskModelInfo();
    if (visitorState.HasProceduralMercenaryId)
      additionalTaskInfo.MercenaryID = visitorState.ProceduralMercenaryId;
    if (taskState.HasProceduralBountyId)
      additionalTaskInfo.BountyID = taskState.ProceduralBountyId;
    additionalTaskInfo.AdditionalMercenaryIDs = taskState.AdditionalMercenaryId;
    return LettuceVillageDataUtil.CreateTaskModel(taskRecordById, taskState.Progress, visitorState.TaskChainProgress, taskState.Status_, additionalTaskInfo, true);
  }

  public static MercenaryVillageTaskItemDataModel CreateTaskModelFromCompletedVisitorState(
    MercenariesCompletedVisitorState completedState)
  {
    if (completedState == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModelFromCompletedVisitorState: completed state is null");
      return (MercenaryVillageTaskItemDataModel) null;
    }
    VisitorTaskChainDbfRecord visitorTaskChainById = LettuceVillageDataUtil.GetVisitorTaskChainByID(completedState.CompletedTaskChainId);
    int count = visitorTaskChainById.TaskList.Count;
    if (count == 0)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModelFromCompletedVisitorState: no tasks in related task chain");
      return (MercenaryVillageTaskItemDataModel) null;
    }
    VisitorTaskDbfRecord taskRecord = visitorTaskChainById.TaskList[count - 1]?.TaskRecord;
    if (taskRecord == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModelFromCompletedVisitorState: task record is null");
      return (MercenaryVillageTaskItemDataModel) null;
    }
    LettuceVillageDataUtil.AdditionalTaskModelInfo additionalTaskInfo = new LettuceVillageDataUtil.AdditionalTaskModelInfo();
    if (completedState.HasProceduralMercenaryId)
      additionalTaskInfo.MercenaryID = completedState.ProceduralMercenaryId;
    return LettuceVillageDataUtil.CreateTaskModel(taskRecord, taskRecord.Quota, count - 1, MercenariesTaskState.Status.CLAIMED, additionalTaskInfo, true);
  }

  public static MercenaryVillageTaskItemDataModel CreateTaskModel(
    VisitorTaskDbfRecord taskRecord,
    int progress,
    int taskChainProgress,
    MercenariesTaskState.Status taskStatus,
    LettuceVillageDataUtil.AdditionalTaskModelInfo additionalTaskInfo = null,
    bool setTaskContext = false,
    bool ShowEquipmentRewardsAsIcon = false)
  {
    MercenaryVisitorDbfRecord visitorRecordById = LettuceVillageDataUtil.GetVisitorRecordByID(taskRecord.MercenaryVisitorId);
    if (visitorRecordById == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModel: visitor record is null for mercenary visitor id: {0}", (object) taskRecord.MercenaryVisitorId);
      return (MercenaryVillageTaskItemDataModel) null;
    }
    LettuceMercenary lettuceMercenary = (LettuceMercenary) null;
    int num = -1;
    if (additionalTaskInfo != null && additionalTaskInfo.ValidMercenaryID)
    {
      num = additionalTaskInfo.MercenaryID;
      lettuceMercenary = CollectionManager.Get().GetMercenary((long) num, true);
    }
    if (lettuceMercenary == null)
    {
      num = LettuceVillageDataUtil.GetMercenaryIdForVisitor(visitorRecordById, taskRecord);
      lettuceMercenary = CollectionManager.Get().GetMercenary((long) num, true);
    }
    if (lettuceMercenary == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModel: merc is null for mercenary id: {0}", (object) num);
      return (MercenaryVillageTaskItemDataModel) null;
    }
    MercenaryVillageTaskItemDataModel taskModel = new MercenaryVillageTaskItemDataModel();
    string taskString = LettuceVillageDataUtil.FormatTaskDescriptionForAbility((string) taskRecord.TaskDescription, taskRecord.Quota, num);
    int bountyId = -1;
    List<int> additionalMercenaryIds = (List<int>) null;
    if (additionalTaskInfo != null)
    {
      bountyId = additionalTaskInfo.BountyID;
      additionalMercenaryIds = additionalTaskInfo.AdditionalMercenaryIDs;
    }
    string str1 = LettuceVillageDataUtil.FormatTaskStringForProceduralData(taskString, num, bountyId, additionalMercenaryIds);
    string str2 = LettuceVillageDataUtil.FormatTaskStringForProceduralData((string) taskRecord.TaskTitle, num, bountyId, additionalMercenaryIds);
    taskModel.TaskId = taskRecord.ID;
    taskModel.Title = str2;
    taskModel.Description = str1;
    taskModel.Progress = progress;
    taskModel.ProgressNeeded = taskRecord.Quota;
    taskModel.ProgressMessage = string.Format(GameStrings.Get("GLOBAL_PROGRESSION_PROGRESS_MESSAGE"), (object) progress, (object) taskRecord.Quota);
    taskModel.MercenaryId = num;
    taskModel.TaskType = visitorRecordById.VisitorType;
    taskModel.TaskChainIndex = taskChainProgress;
    taskModel.MercShoutOut = (string) taskRecord.MercenaryQuote;
    taskModel.IsTimedEvent = false;
    taskModel.TaskStatus = taskStatus;
    VisitorTaskChainDbfRecord visitorTaskChain = LettuceVillageDataUtil.GetVisitorTaskChain(visitorRecordById);
    if (visitorTaskChain != null)
    {
      taskModel.TaskChainId = visitorTaskChain.ID;
      taskModel.TaskChainLength = visitorTaskChain.TaskList.Count;
    }
    if (setTaskContext)
      LettuceVillageDataUtil.CurrentTaskContext = num;
    RewardListDataModel rewardListRecord = RewardUtils.CreateRewardListDataModelFromRewardListRecord(taskRecord.RewardListRecord);
    taskModel.RewardList = rewardListRecord;
    if (rewardListRecord != null && rewardListRecord.Items != null)
    {
      foreach (RewardItemDataModel rewardItemDataModel in rewardListRecord.Items)
      {
        if (taskStatus == MercenariesTaskState.Status.CLAIMED)
          rewardItemDataModel.IsClaimed = true;
        if (rewardItemDataModel.ItemType == RewardItemType.MERCENARY_EQUIPMENT)
          rewardItemDataModel.ItemType = RewardItemType.MERCENARY_EQUIPMENT_ICON;
      }
    }
    switch (visitorRecordById.VisitorType)
    {
      case MercenaryVisitor.VillageVisitorType.STANDARD:
      case MercenaryVisitor.VillageVisitorType.PROCEDURAL:
        taskModel.TaskStyle = LettuceVillageTaskBoard.TaskStyle.NORMAL;
        break;
      case MercenaryVisitor.VillageVisitorType.EVENT:
        taskModel.TaskStyle = LettuceVillageTaskBoard.TaskStyle.NORMAL;
        SpecialEventType eventType = visitorRecordById.Event;
        switch (eventType)
        {
          case SpecialEventType.UNKNOWN:
          case SpecialEventType.IGNORE:
          case SpecialEventType.SPECIAL_EVENT_NEVER:
          case SpecialEventType.SPECIAL_EVENT_ALWAYS:
            break;
          default:
            taskModel.IsTimedEvent = true;
            taskModel.TaskStyle = LettuceVillageTaskBoard.TaskStyle.LEGENDARY;
            TimeSpan timeLeftForEvent = SpecialEventManager.Get().GetTimeLeftForEvent(eventType);
            if (timeLeftForEvent.Days > 0)
            {
              taskModel.RemainingEventTime = GameStrings.Format("GLUE_MERCENARIES_TASKBOARD_EVENT_TIME_REM_DAYS", (object) timeLeftForEvent.Days, (object) timeLeftForEvent.Hours);
              break;
            }
            if (timeLeftForEvent.Hours > 1)
            {
              taskModel.RemainingEventTime = GameStrings.Format("GLUE_MERCENARIES_TASKBOARD_EVENT_TIME_REM_HOURS", (object) timeLeftForEvent.Hours);
              break;
            }
            taskModel.RemainingEventTime = GameStrings.Get("GLUE_MERCENARIES_TASKBOARD_EVENT_TIME_REM_HOUR_OR_LESS");
            break;
        }
        break;
      case MercenaryVisitor.VillageVisitorType.SPECIAL:
        taskModel.TaskStyle = LettuceVillageTaskBoard.TaskStyle.LEGENDARY;
        break;
    }
    taskModel.MercenaryName = lettuceMercenary.m_mercName;
    taskModel.MercenaryShortName = lettuceMercenary.m_mercShortName;
    taskModel.MercenaryRole = lettuceMercenary.m_role;
    taskModel.MercenaryLevel = lettuceMercenary.m_level;
    CardDbfRecord cardRecord = lettuceMercenary.GetCardRecord();
    taskModel.MercenaryCard = new CardDataModel()
    {
      CardId = cardRecord.NoteMiniGuid,
      Premium = TAG_PREMIUM.NORMAL,
      FlavorText = (string) cardRecord?.FlavorText
    };
    return taskModel;
  }

  public static MercenaryVillageTaskItemDataModel CreateTaskModelFromRenownOffer(
    MercenariesRenownOfferData renownOffer)
  {
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) renownOffer.MercenaryId, true);
    if (mercenary == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModel: merc is null for mercenary id: {0}", (object) renownOffer.MercenaryId);
      return (MercenaryVillageTaskItemDataModel) null;
    }
    RewardListDataModel rewardListDataModel = new RewardListDataModel()
    {
      Items = new DataModelList<RewardItemDataModel>()
    };
    if (renownOffer.CoinAmount > 0)
      rewardListDataModel.Items.Add(new RewardItemDataModel()
      {
        Quantity = 1,
        ItemType = RewardItemType.MERCENARY_COIN,
        MercenaryCoin = new LettuceMercenaryCoinDataModel()
        {
          MercenaryId = mercenary.ID,
          MercenaryName = mercenary.m_mercName,
          Quantity = renownOffer.CoinAmount,
          GlowActive = false,
          NameActive = false
        }
      });
    if (renownOffer.PortraitId > 0)
    {
      MercenaryArtVariationPremiumDbfRecord record = GameDbf.MercenaryArtVariationPremium.GetRecord(renownOffer.PortraitId);
      if (record != null)
      {
        LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(renownOffer.MercenaryId, record.MercenaryArtVariationId, (TAG_PREMIUM) record.Premium, mercenary);
        mercenaryDataModel.Owned = true;
        rewardListDataModel.Items.Add(new RewardItemDataModel()
        {
          Quantity = 1,
          ItemType = RewardItemType.MERCENARY,
          Mercenary = mercenaryDataModel,
          IsMercenaryPortrait = RewardUtils.IsMercenaryRewardPortrait(mercenaryDataModel)
        });
      }
      else
        Debug.LogErrorFormat("error in LettuceVillageDataUtil.CreateTaskModel: merc portrait is null for mercenary id: {0}", (object) renownOffer.PortraitId);
    }
    MercenaryVillageTaskItemDataModel modelFromRenownOffer = new MercenaryVillageTaskItemDataModel();
    modelFromRenownOffer.TaskId = (int) renownOffer.RenownOfferId;
    modelFromRenownOffer.Title = string.Format(GameStrings.Get("GLUE_MERCENARIES_TASKBOARD_RENOWN_OFFER_TITLE"), (object) mercenary.m_mercShortName);
    modelFromRenownOffer.Description = string.Empty;
    modelFromRenownOffer.TaskStyle = LettuceVillageTaskBoard.TaskStyle.RENOWN;
    modelFromRenownOffer.TaskStatus = MercenariesTaskState.Status.ACTIVE;
    modelFromRenownOffer.TaskType = MercenaryVisitor.VillageVisitorType.PROCEDURAL;
    modelFromRenownOffer.Progress = 0;
    modelFromRenownOffer.ProgressNeeded = 1;
    modelFromRenownOffer.MercenaryName = mercenary.m_mercName;
    modelFromRenownOffer.MercenaryShortName = mercenary.m_mercShortName;
    modelFromRenownOffer.MercenaryRole = mercenary.m_role;
    modelFromRenownOffer.MercenaryLevel = mercenary.m_level;
    modelFromRenownOffer.RewardList = rewardListDataModel;
    modelFromRenownOffer.TaskChainIndex = 0;
    modelFromRenownOffer.TaskChainId = 0;
    modelFromRenownOffer.TaskChainLength = 0;
    modelFromRenownOffer.MercShoutOut = string.Empty;
    modelFromRenownOffer.IsTimedEvent = false;
    modelFromRenownOffer.IsRenownOffer = true;
    CardDbfRecord cardRecord = mercenary.GetCardRecord();
    modelFromRenownOffer.MercenaryCard = new CardDataModel()
    {
      CardId = cardRecord.NoteMiniGuid,
      Premium = TAG_PREMIUM.NORMAL,
      FlavorText = (string) cardRecord?.FlavorText
    };
    return modelFromRenownOffer;
  }

  public static MercenaryVillageTaskItemDataModel Dev_CreateGenericTaskModel(
    int taskId,
    int progress,
    bool ShowEquipmentRewardsAsIcon = false)
  {
    VisitorTaskDbfRecord taskRecordById = LettuceVillageDataUtil.GetTaskRecordByID(taskId);
    if (taskRecordById != null)
      return LettuceVillageDataUtil.CreateTaskModel(taskRecordById, progress, 1, MercenariesTaskState.Status.ACTIVE, ShowEquipmentRewardsAsIcon: ShowEquipmentRewardsAsIcon);
    Debug.LogErrorFormat("error in LettuceVillageDataUtil.Dev_CreateGenericTaskModel: task record is null for task id {0}", (object) taskId);
    return (MercenaryVillageTaskItemDataModel) null;
  }

  public static int GetCurrentProgressForTaskRecord(
    VisitorTaskDbfRecord taskRecord,
    MercenariesVisitorState visitorState)
  {
    if (taskRecord == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageTaskBoard.CreateTaskItemFromTaskRecord: task record was null");
      return 0;
    }
    if (visitorState == null)
      return 0;
    MercenariesTaskState activeTaskState = visitorState.ActiveTaskState;
    return activeTaskState == null || activeTaskState.TaskId != taskRecord.ID ? 0 : activeTaskState.Progress;
  }

  public static MercenariesTaskState.Status GetCurrentTaskStatusForTaskRecord(
    VisitorTaskDbfRecord taskRecord,
    int taskChainIndex,
    MercenariesVisitorState visitorState)
  {
    if (taskRecord == null)
    {
      Debug.LogErrorFormat("error in LettuceVillageTaskBoard.GetCurrentTaskStatusForTaskRecord: task record was null");
      return MercenariesTaskState.Status.INVALID;
    }
    if (visitorState == null)
      return MercenariesTaskState.Status.CLAIMED;
    MercenariesTaskState activeTaskState = visitorState.ActiveTaskState;
    if (activeTaskState == null)
      return MercenariesTaskState.Status.INVALID;
    if (activeTaskState.TaskId == taskRecord.ID)
      return activeTaskState.Status_;
    if (taskChainIndex < visitorState.TaskChainProgress)
      return MercenariesTaskState.Status.CLAIMED;
    int taskChainProgress = visitorState.TaskChainProgress;
    return MercenariesTaskState.Status.INVALID;
  }

  public static List<string> GetListOfNewMercProducts()
  {
    List<string> productIds = new List<string>();
    StoreManager storeManager = StoreManager.Get();
    if (storeManager == null || storeManager.CatalogNetworkPages == null || storeManager.Catalog == null)
      return (List<string>) null;
    List<ShopType> shopTypeList = new List<ShopType>()
    {
      ShopType.MERCENARIES_STORE
    };
    if (!storeManager.CatalogNetworkPages.Contains((IEnumerable<ShopType>) shopTypeList))
      return (List<string>) null;
    List<Network.ShopSection> sections = storeManager.CatalogNetworkPages.Pages?[ShopType.MERCENARIES_STORE]?.Sections;
    if (sections == null)
      return (List<string>) null;
    sections.ForEach((Action<Network.ShopSection>) (t => t.Products.ForEach((Action<Network.ShopSection.ProductRef>) (product =>
    {
      ProductDataModel productByPmtId = StoreManager.Get().Catalog.GetProductByPmtId(ProductId.CreateFrom(product.PmtId));
      if (productByPmtId == null || productByPmtId.Availability != ProductAvailability.CAN_PURCHASE || !productByPmtId.Tags.Contains("new"))
        return;
      productIds.Add(productByPmtId.PmtId.ToString());
    }))));
    return productIds;
  }

  public static bool HasNewMercShopProducts()
  {
    List<string> ofNewMercProducts = LettuceVillageDataUtil.GetListOfNewMercProducts();
    if (ofNewMercProducts == null)
      return false;
    string str1 = Options.Get().GetString(Option.LATEST_DISPLAYED_SHOP_PRODUCT_LIST);
    List<string> stringList = new List<string>();
    stringList.AddRange((IEnumerable<string>) str1.Split(':'));
    foreach (string str2 in ofNewMercProducts)
    {
      if (!stringList.Contains(str2))
        return true;
    }
    return false;
  }

  public static bool IsMercShopAvailable()
  {
    StoreManager storeManager = StoreManager.Get();
    if (!storeManager.IsOpen() || !storeManager.IsVintageStoreEnabled() && storeManager.Catalog.GetTiers(ShopType.MERCENARIES_STORE).Count == 0)
      return false;
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (netObject == null)
    {
      Debug.LogError((object) "LettuceVillageDataUtil.IsMercShopAvailable - Can't access NetCacheMercenariesPlayerInfo");
      return false;
    }
    bool flag;
    return !netObject.BuildingEnabledMap.TryGetValue(MercenaryBuilding.Mercenarybuildingtype.SHOP, out flag) || flag;
  }

  public static int GetMercenaryIdForVisitor(
    MercenaryVisitorDbfRecord visitorRecord,
    VisitorTaskDbfRecord taskRecord = null)
  {
    if (visitorRecord == null)
    {
      Debug.LogError((object) "LettuceVillageDataUtil.GetMercenaryIdForVisitor - Visitor record not provided");
      return 0;
    }
    if (taskRecord == null)
    {
      MercenariesVisitorState visitorStateById = LettuceVillageDataUtil.GetVisitorStateByID(visitorRecord.ID);
      if (visitorStateById == null)
      {
        Debug.LogError((object) "LettuceVillageDataUtil.GetMercenaryIdForVisitor - Can't find visitor state");
        return visitorRecord.MercenaryId;
      }
      if (visitorStateById.HasProceduralMercenaryId)
        return visitorStateById.ProceduralMercenaryId;
      taskRecord = LettuceVillageDataUtil.GetTaskRecordByID(visitorStateById.ActiveTaskState.TaskId);
      if (taskRecord == null)
      {
        Debug.LogError((object) "LettuceVillageDataUtil.GetMercenaryIdForVisitor - Can't find task record");
        return visitorRecord.MercenaryId;
      }
    }
    return taskRecord.MercenaryOverride != 0 ? taskRecord.MercenaryOverride : visitorRecord.MercenaryId;
  }

  public static List<int> GetDisabledMercenaryList()
  {
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (netObject != null)
      return netObject.DisabledMercenaryList;
    Log.Lettuce.PrintError("GetDisabledMercenaryList - Can't access NetCacheMercenariesPlayerInfo");
    return (List<int>) null;
  }

  public static bool IsMercenaryDisabled(int mercenaryId, List<int> disabledMercList = null)
  {
    if (disabledMercList == null)
      disabledMercList = LettuceVillageDataUtil.GetDisabledMercenaryList();
    return disabledMercList.Contains(mercenaryId);
  }

  public static bool IsAnyMercenaryFinishedTraining()
  {
    BuildingTierDbfRecord recordFromBuilding = LettuceVillageDataUtil.GetCurrentTierRecordFromBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL);
    int propertyForBuilding1 = LettuceVillageDataUtil.GetCurrentTierPropertyForBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL, TierProperties.Buildingtierproperty.TRAININGXPPOOLSIZE, recordFromBuilding);
    int propertyForBuilding2 = LettuceVillageDataUtil.GetCurrentTierPropertyForBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL, TierProperties.Buildingtierproperty.TRAININGXPPERHOUR, recordFromBuilding);
    (LettuceMercenary, LettuceMercenary) mercenariesInTraining = CollectionManager.Get().GetMercenariesInTraining();
    bool flag1 = false;
    bool flag2 = false;
    if (mercenariesInTraining.Item1 != null)
    {
      int gainedFromTimestamp = LettuceVillageDataUtil.CalculateExpGainedFromTimestamp(mercenariesInTraining.Item1.m_trainingStartDate, propertyForBuilding2);
      flag1 = mercenariesInTraining.Item1.IsMaxLevel() || gainedFromTimestamp >= propertyForBuilding1;
    }
    if (mercenariesInTraining.Item2 != null)
    {
      int gainedFromTimestamp = LettuceVillageDataUtil.CalculateExpGainedFromTimestamp(mercenariesInTraining.Item2.m_trainingStartDate, propertyForBuilding2);
      flag2 = mercenariesInTraining.Item2.IsMaxLevel() || gainedFromTimestamp >= propertyForBuilding1;
    }
    return flag1 | flag2;
  }

  public static int GetTimeTrainingInSeconds(Date startDate) => startDate == null ? 0 : (int) (DateTime.UtcNow - new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hours, startDate.Min, startDate.Sec)).TotalSeconds;

  public static int CalculateExpGainedFromTimestamp(Date startDate, int expPerHour)
  {
    if (startDate == null)
      return 0;
    TimeSpan timeSpan = DateTime.UtcNow - new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hours, startDate.Min, startDate.Sec);
    return (int) timeSpan.TotalHours * expPerHour + (int) (timeSpan.TotalSeconds % 3600.0 * (double) expPerHour / 3600.0);
  }

  public class AdditionalTaskModelInfo
  {
    public bool ValidMercenaryID => this.MercenaryID != -1;

    public int MercenaryID { get; set; } = -1;

    public int BountyID { get; set; } = -1;

    public List<int> AdditionalMercenaryIDs { get; set; }
  }
}
