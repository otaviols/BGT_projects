using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Commerce;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardUtils
{
  public static readonly Vector3 RewardHiddenScale = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
  public static readonly float RewardHideTime = 0.25f;
  public static readonly float MercRewardEndBlurTime = 0.1f;
  private static readonly AssetReference s_questRewardsTexturePage2 = new AssetReference("QuestRewards2.psd:1de88a86bd486434dab6ab887ca40254");
  private static readonly AssetReference s_arcaneOrbIcon = new AssetReference("Shop_VC2_Arcane_Orb_Icon.tif:b47e50430b8b4554688cc9e385ced3f2");

  public static List<RewardData> GetRewards(List<NetCache.ProfileNotice> notices)
  {
    List<RewardData> rewardDataList = new List<RewardData>();
    foreach (NetCache.ProfileNotice notice in notices)
    {
      RewardData rewardData = (RewardData) null;
      switch (notice.Type)
      {
        case NetCache.ProfileNotice.NoticeType.REWARD_BOOSTER:
          NetCache.ProfileNoticeRewardBooster noticeRewardBooster = notice as NetCache.ProfileNoticeRewardBooster;
          rewardData = (RewardData) new BoosterPackRewardData(noticeRewardBooster.Id, noticeRewardBooster.Count);
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_CARD:
          NetCache.ProfileNoticeRewardCard noticeRewardCard = notice as NetCache.ProfileNoticeRewardCard;
          rewardData = (RewardData) new CardRewardData(noticeRewardCard.CardID, noticeRewardCard.Premium, noticeRewardCard.Quantity);
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_DUST:
          if (notice.Origin != NetCache.ProfileNotice.NoticeOrigin.HOF_COMPENSATION)
          {
            rewardData = (RewardData) new ArcaneDustRewardData((notice as NetCache.ProfileNoticeRewardDust).Amount);
            break;
          }
          continue;
        case NetCache.ProfileNotice.NoticeType.REWARD_MOUNT:
          rewardData = (RewardData) new MountRewardData((MountRewardData.MountType) (notice as NetCache.ProfileNoticeRewardMount).MountID);
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_FORGE:
          rewardData = (RewardData) new ForgeTicketRewardData((notice as NetCache.ProfileNoticeRewardForge).Quantity);
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_CURRENCY:
          NetCache.ProfileNoticeRewardCurrency noticeRewardCurrency = notice as NetCache.ProfileNoticeRewardCurrency;
          switch (noticeRewardCurrency.CurrencyType)
          {
            case PegasusShared.CurrencyType.CURRENCY_TYPE_GOLD:
              rewardData = (RewardData) new GoldRewardData((long) noticeRewardCurrency.Amount, new DateTime?(DateTime.FromFileTimeUtc(noticeRewardCurrency.Date)));
              break;
            case PegasusShared.CurrencyType.CURRENCY_TYPE_CN_ARCANE_ORBS:
              rewardData = (RewardData) RewardUtils.CreateArcaneOrbRewardData(noticeRewardCurrency.Amount);
              break;
          }
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_CARD_BACK:
          rewardData = (RewardData) new CardBackRewardData((notice as NetCache.ProfileNoticeRewardCardBack).CardBackID);
          break;
        case NetCache.ProfileNotice.NoticeType.EVENT:
          rewardData = (RewardData) new EventRewardData((notice as NetCache.ProfileNoticeEvent).EventType);
          break;
        case NetCache.ProfileNotice.NoticeType.GENERIC_REWARD_CHEST:
          RewardUtils.AddRewardDataForGenericRewardChest(notice as NetCache.ProfileNoticeGenericRewardChest, ref rewardDataList);
          rewardData = (RewardData) null;
          break;
        case NetCache.ProfileNotice.NoticeType.MINI_SET_GRANTED:
          NetCache.ProfileNoticeMiniSetGranted noticeMiniSetGranted = notice as NetCache.ProfileNoticeMiniSetGranted;
          MiniSetDbfRecord record1 = GameDbf.MiniSet.GetRecord(noticeMiniSetGranted.MiniSetID);
          if ((record1 != null ? (record1.HideOnClient ? 1 : 0) : 0) != 0)
          {
            Network.Get().AckNotice(notice.NoticeID);
            break;
          }
          rewardData = (RewardData) new MiniSetRewardData(noticeMiniSetGranted.MiniSetID, noticeMiniSetGranted.Premium);
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_GUIDE:
          string toastName1;
          string toastDesc1;
          bool shouldSkipToast1;
          RewardUtils.TryGetToastTextFromFixedRewardMap(notice is NetCache.ProfileNoticeRewardBattlegroundsGuideSkin battlegroundsGuideSkin ? battlegroundsGuideSkin.FixedRewardMapID : 0, out toastName1, out toastDesc1, out shouldSkipToast1);
          if (shouldSkipToast1)
          {
            Network.Get().AckNotice(notice.NoticeID);
            break;
          }
          rewardData = (RewardData) new CardRewardData(battlegroundsGuideSkin.CardID, TAG_PREMIUM.NORMAL, 1);
          rewardData.NameOverride = toastName1;
          rewardData.DescriptionOverride = toastDesc1;
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_HERO:
          string toastName2;
          string toastDesc2;
          bool shouldSkipToast2;
          RewardUtils.TryGetToastTextFromFixedRewardMap(notice is NetCache.ProfileNoticeRewardBattlegroundsHeroSkin battlegroundsHeroSkin ? battlegroundsHeroSkin.FixedRewardMapID : 0, out toastName2, out toastDesc2, out shouldSkipToast2);
          if (shouldSkipToast2)
          {
            Network.Get().AckNotice(notice.NoticeID);
            break;
          }
          rewardData = (RewardData) new CardRewardData(battlegroundsHeroSkin.CardID, TAG_PREMIUM.NORMAL, 1);
          rewardData.NameOverride = toastName2;
          rewardData.DescriptionOverride = toastDesc2;
          break;
        case NetCache.ProfileNotice.NoticeType.MERCENARIES_ABILITY_UNLOCK:
          NetCache.ProfileNoticeMercenariesAbilityUnlock mercenariesAbilityUnlock = notice as NetCache.ProfileNoticeMercenariesAbilityUnlock;
          rewardData = (RewardData) new MercenariesAbilityUnlockRewardData(mercenariesAbilityUnlock.MercenaryId, mercenariesAbilityUnlock.AbilityId);
          break;
        case NetCache.ProfileNotice.NoticeType.MERCENARIES_MERC_LICENSE:
          NetCache.ProfileNoticeMercenariesMercenaryLicense mercenaryLicense = notice as NetCache.ProfileNoticeMercenariesMercenaryLicense;
          rewardData = RewardUtils.CreateMercenaryOrKnockoutRewardData(mercenaryLicense.MercenaryId, mercenaryLicense.ArtVariationId, (TAG_PREMIUM) mercenaryLicense.ArtVariationPremium, (int) mercenaryLicense.CurrencyAmount);
          if (rewardData != null)
            break;
          continue;
        case NetCache.ProfileNotice.NoticeType.MERCENARIES_CURRENCY_LICENSE:
          NetCache.ProfileNoticeMercenariesCurrencyLicense mercenariesCurrencyLicense = notice as NetCache.ProfileNoticeMercenariesCurrencyLicense;
          rewardData = (RewardData) RewardUtils.CreateMercenaryCoinsRewardData(mercenariesCurrencyLicense.MercenaryId, (int) mercenariesCurrencyLicense.CurrencyAmount, true, false);
          string localizedName1;
          string localizedShortName1;
          RewardUtils.GetMercenaryName(LettuceMercenary.GetDefaultArtVariationRecord(mercenariesCurrencyLicense.MercenaryId).CardRecord.NoteMiniGuid, out localizedName1, out localizedShortName1);
          if (localizedName1 != null)
          {
            rewardData.NameOverride = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_COINS_TITLE", (object) localizedShortName1);
            rewardData.DescriptionOverride = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_COINS_DESC", (object) localizedShortName1);
            break;
          }
          continue;
        case NetCache.ProfileNotice.NoticeType.MERCENARIES_BOOSTER_LICENSE:
          rewardData = (RewardData) BoosterPackRewardData.CreateMercenariesBoosterPackRewardData((notice as NetCache.ProfileNoticeMercenariesBoosterLicense).Count);
          break;
        case NetCache.ProfileNotice.NoticeType.MERCENARIES_RANDOM_REWARD_LICENSE:
          NetCache.ProfileNoticeMercenariesRandomRewardLicense randomRewardLicense = notice as NetCache.ProfileNoticeMercenariesRandomRewardLicense;
          RewardItemDataModel rewardItemDataModel = RewardUtils.CreateMercenaryRewardItemDataModel(randomRewardLicense.MercenaryId, randomRewardLicense.ArtVariationId, (TAG_PREMIUM) randomRewardLicense.ArtVariationPremium);
          string localizedName2;
          string localizedShortName2;
          RewardUtils.GetMercenaryName(rewardItemDataModel.Mercenary.Card.CardId, out localizedName2, out localizedShortName2);
          if (localizedName2 != null)
          {
            if (randomRewardLicense.IsConvertedMercenary)
            {
              RewardItemDataModel dataModel = RewardUtils.CreateMercenaryCoinsRewardData(randomRewardLicense.MercenaryId, (int) randomRewardLicense.CurrencyAmount, true, false).DataModel;
              rewardData = (RewardData) new MercenariesKnockoutRewardData(rewardItemDataModel, dataModel);
              rewardData.NameOverride = RewardUtils.GetMercenaryRarityText(rewardItemDataModel.Mercenary.MercenaryRarity);
              rewardData.DescriptionOverride = RewardUtils.GetMercenaryKnockoutCoinsText((TAG_PREMIUM) randomRewardLicense.ArtVariationPremium, localizedName2, localizedShortName2);
              break;
            }
            if (randomRewardLicense.CurrencyAmount > 0L)
            {
              rewardData = (RewardData) RewardUtils.CreateMercenaryCoinsRewardData(randomRewardLicense.MercenaryId, (int) randomRewardLicense.CurrencyAmount, true, false);
              rewardData.NameOverride = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_COINS_TITLE", (object) localizedShortName2);
              rewardData.DescriptionOverride = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_COINS_DESC", (object) localizedShortName2);
              break;
            }
            rewardData = (RewardData) new RewardItemRewardData(rewardItemDataModel, true, Reward.Type.MERCENARY_RANDOM_MERCENARY);
            rewardData.NameOverride = RewardUtils.GetMercenaryRarityText(rewardItemDataModel.Mercenary.MercenaryRarity);
            rewardData.DescriptionOverride = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_DESC", (object) localizedShortName2);
            break;
          }
          continue;
        case NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_BOARD_SKIN:
          string toastName3;
          string toastDesc3;
          bool shouldSkipToast3;
          RewardUtils.TryGetToastTextFromFixedRewardMap(notice is NetCache.ProfileNoticeRewardBattlegroundsBoard battlegroundsBoard1 ? battlegroundsBoard1.FixedRewardMapID : 0, out toastName3, out toastDesc3, out shouldSkipToast3);
          if (shouldSkipToast3)
          {
            Network.Get().AckNotice(notice.NoticeID);
            break;
          }
          CollectibleBattlegroundsBoard battlegroundsBoard2 = new CollectibleBattlegroundsBoard(GameDbf.BattlegroundsBoardSkin.GetRecord((int) battlegroundsBoard1.BoardSkinID));
          rewardData = (RewardData) new BattlegroundsBoardSkinRewardData(battlegroundsBoard1.BoardSkinID, battlegroundsBoard2.CreateBoardDataModel());
          rewardData.NameOverride = toastName3;
          rewardData.DescriptionOverride = toastDesc3;
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_FINISHER:
          string toastName4;
          string toastDesc4;
          bool shouldSkipToast4;
          RewardUtils.TryGetToastTextFromFixedRewardMap(notice is NetCache.ProfileNoticeRewardBattlegroundsFinisher battlegroundsFinisher1 ? battlegroundsFinisher1.FixedRewardMapID : 0, out toastName4, out toastDesc4, out shouldSkipToast4);
          if (shouldSkipToast4)
          {
            Network.Get().AckNotice(notice.NoticeID);
            break;
          }
          CollectibleBattlegroundsFinisher battlegroundsFinisher2 = new CollectibleBattlegroundsFinisher(GameDbf.BattlegroundsFinisher.GetRecord((int) battlegroundsFinisher1.FinisherID));
          rewardData = (RewardData) new BattlegroundsFinisherRewardData(battlegroundsFinisher1.FinisherID, battlegroundsFinisher2.CreateFinisherDataModel());
          rewardData.NameOverride = toastName4;
          rewardData.DescriptionOverride = toastDesc4;
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_BATTLEGROUNDS_EMOTE:
          string toastName5;
          string toastDesc5;
          bool shouldSkipToast5;
          RewardUtils.TryGetToastTextFromFixedRewardMap(notice is NetCache.ProfileNoticeRewardBattlegroundsEmote battlegroundsEmote1 ? battlegroundsEmote1.FixedRewardMapID : 0, out toastName5, out toastDesc5, out shouldSkipToast5);
          if (shouldSkipToast5)
          {
            Network.Get().AckNotice(notice.NoticeID);
            break;
          }
          CollectibleBattlegroundsEmote battlegroundsEmote2 = new CollectibleBattlegroundsEmote(GameDbf.BattlegroundsEmote.GetRecord((int) battlegroundsEmote1.EmoteID));
          rewardData = (RewardData) new BattlegroundsEmoteRewardData(battlegroundsEmote1.EmoteID, battlegroundsEmote2.CreateEmoteDataModel());
          rewardData.NameOverride = toastName5;
          rewardData.DescriptionOverride = toastDesc5;
          break;
        case NetCache.ProfileNotice.NoticeType.REWARD_LUCKY_DRAW:
          NetCache.ProfileNoticeLuckyDrawReward noticeLuckyDrawReward = notice as NetCache.ProfileNoticeLuckyDrawReward;
          LuckyDrawRewardsDbfRecord record2 = GameDbf.LuckyDrawRewards.GetRecord(noticeLuckyDrawReward.LuckyDrawRewardId);
          if (record2 == null)
          {
            Debug.LogErrorFormat("REWARD_LUCKY_DRAW invalid LuckyDrawRewardId: {0}", (object) noticeLuckyDrawReward.LuckyDrawRewardId);
            continue;
          }
          rewardData = (RewardData) null;
          int num = 0;
          using (List<RewardItemDbfRecord>.Enumerator enumerator = record2.RewardListRecord.RewardItems.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              RewardItemDbfRecord current = enumerator.Current;
              RewardData newRewardData = (RewardData) null;
              switch (current.RewardType)
              {
                case RewardItem.RewardType.BATTLEGROUNDS_HERO_SKIN:
                  newRewardData = (RewardData) new CardRewardData(GameUtils.TranslateDbIdToCardId(current.BattlegroundsHeroSkinRecord.SkinCardId), TAG_PREMIUM.NORMAL, 1, true);
                  newRewardData.DescriptionOverride = GameStrings.Format("GLUE_BATTLEBASH_REWARD_NOTICE_BODY_SKIN", (object) current.BattlegroundsHeroSkinRecord.SkinCardRecord.Name.GetString());
                  break;
                case RewardItem.RewardType.BATTLEGROUNDS_GUIDE_SKIN:
                  newRewardData = (RewardData) new CardRewardData(GameUtils.TranslateDbIdToCardId(current.BattlegroundsGuideSkinRecord.SkinCardId), TAG_PREMIUM.NORMAL, 1, true);
                  newRewardData.DescriptionOverride = GameStrings.Format("GLUE_BATTLEBASH_REWARD_NOTICE_BODY_BARTENDER", (object) current.BattlegroundsGuideSkinRecord.SkinCardRecord.Name.GetString());
                  break;
                case RewardItem.RewardType.BATTLEGROUNDS_BOARD_SKIN:
                  BattlegroundsBoardSkinDbfRecord record3 = GameDbf.BattlegroundsBoardSkin.GetRecord(current.BattlegroundsBoardSkinId);
                  CollectibleBattlegroundsBoard battlegroundsBoard3 = new CollectibleBattlegroundsBoard(record3);
                  newRewardData = (RewardData) new BattlegroundsBoardSkinRewardData((long) current.BattlegroundsBoardSkinId, battlegroundsBoard3.CreateBoardDataModel());
                  newRewardData.DescriptionOverride = GameStrings.Format("GLUE_BATTLEBASH_REWARD_NOTICE_BODY_BOARD", (object) record3.CollectionName.GetString());
                  break;
                case RewardItem.RewardType.BATTLEGROUNDS_FINISHER:
                  BattlegroundsFinisherDbfRecord record4 = GameDbf.BattlegroundsFinisher.GetRecord(current.BattlegroundsFinisherId);
                  CollectibleBattlegroundsFinisher battlegroundsFinisher3 = new CollectibleBattlegroundsFinisher(record4);
                  newRewardData = (RewardData) new BattlegroundsFinisherRewardData((long) current.BattlegroundsFinisherId, battlegroundsFinisher3.CreateFinisherDataModel());
                  newRewardData.DescriptionOverride = GameStrings.Format("GLUE_BATTLEBASH_REWARD_NOTICE_BODY_STRIKE", (object) record4.CollectionName.GetString());
                  break;
                case RewardItem.RewardType.BATTLEGROUNDS_EMOTE:
                  BattlegroundsEmoteDbfRecord record5 = GameDbf.BattlegroundsEmote.GetRecord(current.BattlegroundsEmoteId);
                  CollectibleBattlegroundsEmote battlegroundsEmote3 = new CollectibleBattlegroundsEmote(record5);
                  newRewardData = (RewardData) new BattlegroundsEmoteRewardData((long) current.BattlegroundsEmoteId, battlegroundsEmote3.CreateEmoteDataModel());
                  string key1 = current.Quantity > 1 ? "GLUE_BATTLEBASH_REWARD_NOTICE_BODY_EMOTEBUNDLE" : "GLUE_BATTLEBASH_REWARD_NOTICE_BODY_EMOTE";
                  newRewardData.DescriptionOverride = GameStrings.Format(key1, (object) record5.CollectionShortName.GetString());
                  break;
                default:
                  Debug.LogErrorFormat("REWARD_LUCKY_DRAW invalid reward type: {0}", (object) current.RewardType.ToString());
                  break;
              }
              if (newRewardData != null)
              {
                string key2 = noticeLuckyDrawReward.LuckyDrawOrigin == PegasusShared.ProfileNoticeLuckyDrawReward.OriginType.ORIGIN_AUTO_GRANT_FROM_EXPIRED_BOX ? "GLUE_BATTLEBASH_REWARD_NOTICE_TITLE_EXPIRED" : "GLUE_BATTLEBASH_REWARD_NOTICE_TITLE";
                newRewardData.NameOverride = GameStrings.Get(key2);
                newRewardData.SetOrigin(NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_LUCKY_DRAW, (long) num++);
                newRewardData.AddNoticeID(notice.NoticeID);
                RewardUtils.AddRewardDataToList(newRewardData, rewardDataList);
              }
            }
            break;
          }
        default:
          continue;
      }
      if (rewardData != null)
        RewardUtils.SetNoticeAndAddRewardDataToList(notice, ref rewardData, ref rewardDataList);
    }
    return rewardDataList;
  }

  private static void SetNoticeAndAddRewardDataToList(
    NetCache.ProfileNotice notice,
    ref RewardData rewardData,
    ref List<RewardData> rewardDataList)
  {
    rewardData.SetOrigin(notice.Origin, notice.OriginData);
    rewardData.AddNoticeID(notice.NoticeID);
    RewardUtils.AddRewardDataToList(rewardData, rewardDataList);
  }

  private static void AddRewardDataForGenericRewardChest(
    NetCache.ProfileNoticeGenericRewardChest notice,
    ref List<RewardData> rewardDataList)
  {
    PegasusShared.RewardChest rewardChest = notice.RewardChest;
    if (rewardChest == null)
      return;
    RewardUtils.AddRewardDataForGenericRewardChestBag(notice, rewardChest.Bag1, 1, ref rewardDataList);
    RewardUtils.AddRewardDataForGenericRewardChestBag(notice, rewardChest.Bag2, 2, ref rewardDataList);
    RewardUtils.AddRewardDataForGenericRewardChestBag(notice, rewardChest.Bag3, 3, ref rewardDataList);
    RewardUtils.AddRewardDataForGenericRewardChestBag(notice, rewardChest.Bag4, 4, ref rewardDataList);
    RewardUtils.AddRewardDataForGenericRewardChestBag(notice, rewardChest.Bag5, 5, ref rewardDataList);
  }

  private static void AddRewardDataForGenericRewardChestBag(
    NetCache.ProfileNoticeGenericRewardChest notice,
    PegasusShared.RewardBag rewardBag,
    int bagNum,
    ref List<RewardData> rewardDataList)
  {
    if (rewardBag == null)
      return;
    string str1 = string.Empty;
    string str2 = string.Empty;
    if (notice.Origin == NetCache.ProfileNotice.NoticeOrigin.GENERIC_REWARD_CHEST_ACHIEVE)
    {
      AchieveDbfRecord record = GameDbf.Achieve.GetRecord((int) notice.OriginData);
      if (record != null)
      {
        str1 = (string) record.Name;
        str2 = (string) record.Description;
      }
    }
    if ((string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2)) && GameDbf.RewardChest.HasRecord(notice.RewardChestAssetId))
    {
      RewardChestDbfRecord record = GameDbf.RewardChest.GetRecord(notice.RewardChestAssetId);
      if (record.Name != null && string.IsNullOrEmpty(str1))
        str1 = record.Name.GetString();
      if (record.Description != null && string.IsNullOrEmpty(str2))
        str2 = record.Description.GetString();
    }
    RewardData rewardData = Network.ConvertRewardBag(rewardBag);
    if (rewardData == null)
      return;
    rewardData.RewardChestAssetId = new int?(notice.RewardChestAssetId);
    rewardData.RewardChestBagNum = new int?(bagNum);
    rewardData.NameOverride = str1;
    rewardData.DescriptionOverride = str2;
    RewardUtils.SetNoticeAndAddRewardDataToList((NetCache.ProfileNotice) notice, ref rewardData, ref rewardDataList);
  }

  public static void GetViewableRewards(
    List<RewardData> rewardDataList,
    HashSet<Assets.Achieve.RewardTiming> rewardTimings,
    out List<RewardData> rewardsToShow,
    out List<RewardData> genericRewardChestsToShow,
    ref List<RewardData> purchasedCardRewardsToShow,
    ref List<Achievement> completedQuests)
  {
    bool flag1 = GameUtils.IsAnyTutorialComplete();
    rewardsToShow = new List<RewardData>();
    genericRewardChestsToShow = new List<RewardData>();
    if (completedQuests == null)
      completedQuests = new List<Achievement>();
    foreach (RewardData rewardData in rewardDataList)
    {
      Log.Achievements.Print("RewardUtils.GetViewableRewards() - processing reward {0}", (object) rewardData);
      if (NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT == rewardData.Origin)
      {
        if (flag1)
        {
          Achievement completedQuest = AchieveManager.Get().GetAchievement((int) rewardData.OriginData);
          if (completedQuest != null)
          {
            List<long> noticeIds = rewardData.GetNoticeIDs();
            Achievement achievement = completedQuests.Find((Predicate<Achievement>) (obj => completedQuest.ID == obj.ID));
            if (achievement != null)
            {
              foreach (long noticeID in noticeIds)
                achievement.AddRewardNoticeID(noticeID);
            }
            else
            {
              foreach (long noticeID in noticeIds)
                completedQuest.AddRewardNoticeID(noticeID);
              if (rewardTimings.Contains(completedQuest.RewardTiming))
                completedQuests.Add(completedQuest);
            }
          }
        }
      }
      else if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.GENERIC_REWARD_CHEST_ACHIEVE)
      {
        Achievement achievement = AchieveManager.Get().GetAchievement((int) rewardData.OriginData);
        if (achievement == null || rewardTimings.Contains(achievement.RewardTiming))
          genericRewardChestsToShow.Add(rewardData);
      }
      else if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.GENERIC_REWARD_CHEST)
      {
        if (rewardData.Origin != NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_DUELS)
          genericRewardChestsToShow.Add(rewardData);
      }
      else
      {
        bool flag2 = false;
        switch (rewardData.RewardType)
        {
          case Reward.Type.ARCANE_DUST:
          case Reward.Type.BOOSTER_PACK:
          case Reward.Type.GOLD:
          case Reward.Type.MERCENARY_COIN:
          case Reward.Type.MERCENARY_ABILITY_UNLOCK:
          case Reward.Type.MERCENARY_BOOSTER:
          case Reward.Type.MERCENARY_MERCENARY:
          case Reward.Type.MERCENARY_RANDOM_MERCENARY:
          case Reward.Type.MERCENARY_KNOCKOUT:
          case Reward.Type.BATTLEGROUNDS_FINISHER:
          case Reward.Type.BATTLEGROUNDS_BOARD_SKIN:
          case Reward.Type.BATTLEGROUNDS_EMOTE:
          case Reward.Type.MERCENARY_RENOWN:
            flag2 = true;
            break;
          case Reward.Type.CARD:
            CardRewardData cardReward = rewardData as CardRewardData;
            if ((!cardReward.CardID.Equals("HERO_08") ? 0 : (cardReward.Premium == TAG_PREMIUM.NORMAL ? 1 : 0)) != 0)
            {
              flag2 = false;
              rewardData.AcknowledgeNotices();
              CollectionManager.Get().AddCardReward(cardReward, false);
              break;
            }
            if (NetCache.ProfileNotice.NoticeOrigin.FROM_PURCHASE == rewardData.Origin || NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE == rewardData.Origin)
            {
              flag2 = false;
              if (StoreManager.Get() != null && StoreManager.Get().WillStoreDisplayNotice(rewardData.Origin, NetCache.ProfileNotice.NoticeType.REWARD_CARD, rewardData.OriginData))
              {
                rewardData.AcknowledgeNotices();
                break;
              }
              if (purchasedCardRewardsToShow != null)
              {
                purchasedCardRewardsToShow.Add(rewardData);
                break;
              }
              break;
            }
            flag2 = true;
            break;
          case Reward.Type.CARD_BACK:
            flag2 = NetCache.ProfileNotice.NoticeOrigin.SEASON != rewardData.Origin;
            break;
          case Reward.Type.FORGE_TICKET:
            bool flag3 = false;
            if (NetCache.ProfileNotice.NoticeOrigin.BLIZZCON == rewardData.Origin && 2013L == rewardData.OriginData)
              flag3 = true;
            if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE)
            {
              Log.Achievements.Print(string.Format("RewardUtils.GetViewableRewards(): auto-acking notices for out of band license reward {0}", (object) rewardData));
              flag3 = true;
            }
            if (flag3)
              rewardData.AcknowledgeNotices();
            flag2 = false;
            break;
          case Reward.Type.MINI_SET:
            flag2 = false;
            if (purchasedCardRewardsToShow != null)
            {
              purchasedCardRewardsToShow.Add(rewardData);
              break;
            }
            break;
        }
        if (flag2)
          rewardsToShow.Add(rewardData);
      }
    }
  }

  public static void SortRewards(ref List<Reward> rewards)
  {
    if (rewards == null)
      return;
    rewards.Sort((Comparison<Reward>) ((r1, r2) =>
    {
      if (r1.RewardType == r2.RewardType)
      {
        if (r1.RewardType != Reward.Type.CARD)
          return 0;
        CardRewardData data1 = r1.Data as CardRewardData;
        CardRewardData data2 = r2.Data as CardRewardData;
        EntityDef entityDef1 = DefLoader.Get().GetEntityDef(data1.CardID);
        EntityDef entityDef2 = DefLoader.Get().GetEntityDef(data2.CardID);
        bool flag1 = entityDef1.IsHeroSkin();
        bool flag2 = entityDef2.IsHeroSkin();
        if (flag1 == flag2)
          return 0;
        return !flag1 ? 1 : -1;
      }
      if (Reward.Type.CARD_BACK == r1.RewardType)
        return -1;
      if (Reward.Type.CARD_BACK == r2.RewardType)
        return 1;
      if (Reward.Type.CARD == r1.RewardType)
        return -1;
      if (Reward.Type.CARD == r2.RewardType)
        return 1;
      if (Reward.Type.BOOSTER_PACK == r1.RewardType)
        return -1;
      if (Reward.Type.BOOSTER_PACK == r2.RewardType)
        return 1;
      if (Reward.Type.MOUNT == r1.RewardType)
        return -1;
      if (Reward.Type.MOUNT == r2.RewardType)
        return 1;
      if (Reward.Type.MERCENARY_EXP == r1.RewardType)
        return -1;
      if (Reward.Type.MERCENARY_EXP == r2.RewardType)
        return 1;
      if (Reward.Type.MERCENARY_ABILITY_UNLOCK == r1.RewardType)
        return -1;
      return Reward.Type.MERCENARY_ABILITY_UNLOCK == r2.RewardType ? 1 : 0;
    }));
  }

  public static void AddRewardDataToList(
    RewardData newRewardData,
    List<RewardData> existingRewardDataList)
  {
    CardRewardData duplicateCardDataReward = RewardUtils.GetDuplicateCardDataReward(newRewardData, existingRewardDataList);
    if (duplicateCardDataReward == null)
    {
      existingRewardDataList.Add(newRewardData);
    }
    else
    {
      CardRewardData other = newRewardData as CardRewardData;
      duplicateCardDataReward.Merge(other);
    }
  }

  public static bool GetNextHeroLevelRewardText(
    TAG_CLASS heroClass,
    int heroLevel,
    int totalLevel,
    out string nextRewardTitle,
    out string nextRewardDescription)
  {
    int nextRewardLevel1;
    RewardData nextHeroLevelReward = FixedRewardsMgr.Get().GetNextHeroLevelReward(heroClass, heroLevel, out nextRewardLevel1);
    int nextRewardLevel2;
    RewardData totalLevelReward = FixedRewardsMgr.Get().GetNextTotalLevelReward(totalLevel, out nextRewardLevel2);
    nextRewardTitle = string.Empty;
    nextRewardDescription = string.Empty;
    bool flag1 = nextRewardLevel1 > 0;
    bool flag2 = nextRewardLevel2 > 0;
    if (!flag1 && !flag2)
      return false;
    int num1 = 0;
    int num2 = nextRewardLevel1 - heroLevel;
    int num3 = nextRewardLevel2 - totalLevel;
    if (flag1 && (!flag2 || num2 <= num3))
    {
      num1 = nextRewardLevel1;
      nextRewardDescription = RewardUtils.GetRewardText(nextHeroLevelReward);
    }
    if (flag1 & flag2 && num2 == num3)
      nextRewardDescription += "\n";
    if (flag2 && (!flag1 || num3 <= num2))
    {
      num1 = heroLevel + num3;
      nextRewardDescription += RewardUtils.GetRewardText(totalLevelReward);
    }
    if (num1 > 0)
      nextRewardTitle = GameStrings.Format("GLOBAL_HERO_LEVEL_NEXT_REWARD_TITLE", (object) num1);
    return nextRewardTitle != string.Empty;
  }

  public static string GetRewardText(RewardData rewardData)
  {
    if (rewardData == null)
      return string.Empty;
    string rewardText;
    switch (rewardData.RewardType)
    {
      case Reward.Type.ARCANE_DUST:
        rewardText = GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_ARCANE_DUST", (object) (rewardData as ArcaneDustRewardData).Amount);
        break;
      case Reward.Type.BOOSTER_PACK:
        BoosterPackRewardData boosterPackRewardData = rewardData as BoosterPackRewardData;
        rewardText = GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_BOOSTER", (object) (string) GameDbf.Booster.GetRecord(boosterPackRewardData.Id).Name);
        break;
      case Reward.Type.CARD:
        CardRewardData cardRewardData = rewardData as CardRewardData;
        EntityDef entityDef = DefLoader.Get().GetEntityDef(cardRewardData.CardID);
        if (cardRewardData.Premium == TAG_PREMIUM.GOLDEN)
        {
          rewardText = GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_GOLDEN_CARD", (object) GameStrings.Get("GLOBAL_COLLECTION_GOLDEN"), (object) entityDef.GetName());
          break;
        }
        rewardText = entityDef.GetName();
        break;
      case Reward.Type.GOLD:
        rewardText = GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_GOLD", (object) (rewardData as GoldRewardData).Amount);
        break;
      default:
        rewardText = "UNKNOWN";
        break;
    }
    return rewardText;
  }

  public static bool ShowReward(
    UserAttentionBlocker blocker,
    Reward reward,
    bool updateCacheValues,
    Vector3 rewardPunchScale,
    Vector3 rewardScale,
    AnimationUtil.DelOnShownWithPunch callback,
    object callbackData)
  {
    return RewardUtils.ShowReward_Internal(blocker, reward, updateCacheValues, rewardPunchScale, rewardScale, string.Empty, (GameObject) null, callback, callbackData);
  }

  public static bool ShowReward(
    UserAttentionBlocker blocker,
    Reward reward,
    bool updateCacheValues,
    Vector3 rewardPunchScale,
    Vector3 rewardScale,
    string callbackName = "",
    object callbackData = null,
    GameObject callbackGO = null)
  {
    return RewardUtils.ShowReward_Internal(blocker, reward, updateCacheValues, rewardPunchScale, rewardScale, callbackName, callbackGO, (AnimationUtil.DelOnShownWithPunch) null, callbackData);
  }

  public static void SetupRewardIcon(
    RewardData rewardData,
    Renderer rewardRenderer,
    UberText rewardAmountLabel,
    out float amountToScaleReward,
    bool doubleGold = false)
  {
    UnityEngine.Vector2 vector2 = UnityEngine.Vector2.zero;
    amountToScaleReward = 1f;
    rewardAmountLabel.gameObject.SetActive(false);
    Material rewardMaterial = (UnityEngine.Object) rewardRenderer != (UnityEngine.Object) null ? RendererExtension.GetMaterial(rewardRenderer) : (Material) null;
    AssetHandleCallback<Texture> callback = (AssetHandleCallback<Texture>) ((assetRef, texture, loadTextureCbData) =>
    {
      if ((UnityEngine.Object) rewardRenderer != (UnityEngine.Object) null)
      {
        ServiceManager.Get<DisposablesCleaner>()?.Attach((Component) rewardRenderer, (IDisposable) texture);
        if (!((UnityEngine.Object) rewardMaterial != (UnityEngine.Object) null))
          return;
        rewardMaterial.mainTexture = (Texture) texture;
      }
      else
        texture?.Dispose();
    });
    switch (rewardData.RewardType)
    {
      case Reward.Type.ARCANE_DUST:
        AssetLoader.Get().LoadAsset<Texture>(RewardUtils.s_questRewardsTexturePage2, callback);
        vector2 = new UnityEngine.Vector2(0.25f, 0.0f);
        ArcaneDustRewardData arcaneDustRewardData = rewardData as ArcaneDustRewardData;
        rewardAmountLabel.Text = arcaneDustRewardData.Amount.ToString();
        rewardAmountLabel.gameObject.SetActive(true);
        break;
      case Reward.Type.BOOSTER_PACK:
        BoosterPackRewardData boosterPackRewardData = rewardData as BoosterPackRewardData;
        BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterPackRewardData.Id);
        if (!string.IsNullOrEmpty(record.QuestIconPath))
        {
          AssetLoader.Get().LoadAsset<Texture>((AssetReference) record.QuestIconPath, callback);
          vector2 = new UnityEngine.Vector2((float) record.QuestIconOffsetX, (float) record.QuestIconOffsetY);
          break;
        }
        Log.Achievements.PrintWarning("Booster Record ID = {0} does not have proper reward icon data", (object) boosterPackRewardData.Id);
        vector2 = new UnityEngine.Vector2(0.0f, 0.75f);
        if (boosterPackRewardData.Id == 11 && boosterPackRewardData.Count > 1)
        {
          vector2 = new UnityEngine.Vector2(0.0f, 0.5f);
          break;
        }
        break;
      case Reward.Type.CARD:
        CardRewardData cardRewardData = rewardData as CardRewardData;
        vector2 = !(cardRewardData.CardID == "HERO_03a") ? (!(cardRewardData.CardID == "HERO_06a") ? new UnityEngine.Vector2(0.5f, 0.0f) : new UnityEngine.Vector2(0.75f, 0.25f)) : new UnityEngine.Vector2(0.75f, 0.5f);
        break;
      case Reward.Type.FORGE_TICKET:
        vector2 = new UnityEngine.Vector2(0.75f, 0.75f);
        amountToScaleReward = 1.46881f;
        break;
      case Reward.Type.GOLD:
        vector2 = new UnityEngine.Vector2(0.25f, 0.75f);
        long amount = ((GoldRewardData) rewardData).Amount;
        if (doubleGold)
          amount *= 2L;
        rewardAmountLabel.Text = amount.ToString();
        rewardAmountLabel.gameObject.SetActive(true);
        break;
      case Reward.Type.ARCANE_ORBS:
        AssetLoader.Get().LoadAsset<Texture>(RewardUtils.s_arcaneOrbIcon, callback);
        rewardAmountLabel.Text = ((SimpleRewardData) rewardData).Amount.ToString();
        rewardAmountLabel.gameObject.SetActive(true);
        rewardMaterial.mainTextureScale = new UnityEngine.Vector2(4f, 4f);
        break;
    }
    rewardMaterial.mainTextureOffset = vector2;
  }

  public static void LoadAndDisplayRewards(List<RewardData> rewards, Action doneCallback = null) => RewardUtils.LoadAndDisplayRewards_LoadNextReward(new RewardUtils.RewardDisplayCallbackData()
  {
    rewardsToDisplay = rewards,
    rewardIndex = 0,
    doneCallback = doneCallback
  });

  private static void LoadAndDisplayRewards_LoadNextReward(
    RewardUtils.RewardDisplayCallbackData callbackData)
  {
    RewardData rewardData = callbackData.rewardsToDisplay[callbackData.rewardIndex];
    ++callbackData.rewardIndex;
    Reward.DelOnRewardLoaded callback = new Reward.DelOnRewardLoaded(RewardUtils.LoadAndDisplayRewards_OnRewardObjectLoaded);
    RewardUtils.RewardDisplayCallbackData callbackData1 = callbackData;
    rewardData.LoadRewardObject(callback, (object) callbackData1);
  }

  private static void LoadAndDisplayRewards_OnRewardObjectLoaded(Reward reward, object callbackData)
  {
    (callbackData as RewardUtils.RewardDisplayCallbackData).currentReward = reward;
    PopupDisplayManager.Get().RewardPopups.DisplayRewardObject(reward, new AnimationUtil.DelOnShownWithPunch(RewardUtils.LoadAndDisplayRewards_OnRewardShown), callbackData);
    reward.ScreenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
  }

  private static void LoadAndDisplayRewards_OnRewardShown(object callbackData)
  {
    Reward currentReward = (callbackData as RewardUtils.RewardDisplayCallbackData).currentReward;
    if ((UnityEngine.Object) currentReward == (UnityEngine.Object) null)
      return;
    currentReward.RegisterClickListener(new Reward.OnClickedCallback(RewardUtils.LoadAndDisplayRewards_OnRewardDismissed), callbackData);
    currentReward.EnableClickCatcher(true);
  }

  private static void LoadAndDisplayRewards_OnRewardDismissed(Reward reward, object callbackData)
  {
    reward.RemoveClickListener(new Reward.OnClickedCallback(RewardUtils.LoadAndDisplayRewards_OnRewardDismissed));
    RewardUtils.RewardDisplayCallbackData displayCallbackData = callbackData as RewardUtils.RewardDisplayCallbackData;
    if (displayCallbackData.rewardIndex >= displayCallbackData.rewardsToDisplay.Count)
      reward.RegisterHideListener(new Reward.OnHideCallback(RewardUtils.LoadAndDisplayRewards_OnAllRewardsShown), (object) displayCallbackData);
    else
      RewardUtils.LoadAndDisplayRewards_LoadNextReward(displayCallbackData);
    reward.Hide(true);
  }

  private static void LoadAndDisplayRewards_OnAllRewardsShown(object callbackData)
  {
    if (!(callbackData is RewardUtils.RewardDisplayCallbackData userData))
    {
      Log.RewardBox.PrintError("RewardUtils.LoadAndDisplayRewards_OnAllRewardsShown(): callbackData was null or now RewardDisplayCallbackData");
    }
    else
    {
      if ((UnityEngine.Object) userData.currentReward != (UnityEngine.Object) null)
        userData.currentReward.ScreenEffectsHandle.StopEffect();
      userData.currentReward?.RemoveHideListener(new Reward.OnHideCallback(RewardUtils.LoadAndDisplayRewards_OnAllRewardsShown), (object) userData);
      Action doneCallback = userData.doneCallback;
      if (doneCallback == null)
        return;
      doneCallback();
    }
  }

  public static void ShowQuestChestReward(
    string title,
    string desc,
    List<RewardData> rewards,
    Transform rewardBone,
    Action doneCallback,
    bool fromNotice = false,
    int noticeID = -1,
    string prefab = "RewardChest_Lock.prefab:06ffa33e82036694e8cacb96aa7b48e8")
  {
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      ChestRewardDisplay componentInChildren = go.GetComponentInChildren<ChestRewardDisplay>();
      componentInChildren.RegisterDoneCallback(doneCallback);
      GameUtils.SetParent((Component) componentInChildren.m_parent.transform, (Component) rewardBone);
      if (componentInChildren.ShowRewards_Quest(rewards, rewardBone, title, desc, fromNotice, noticeID))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) prefab, callback);
  }

  public static void ShowMercenariesChestReward(
    List<RewardData> rewards,
    List<RewardData> bonusRewards,
    Transform rewardBone,
    Action doneCallback,
    bool autoOpenChest,
    bool fromNotice = false,
    int noticeID = -1)
  {
    if (rewards == null || rewards.Count == 0)
    {
      Debug.LogErrorFormat("ShowMercenariesChestReward: No rewards provided.");
      Action action = doneCallback;
      if (action == null)
        return;
      action();
    }
    else
    {
      List<RewardData> mainChestRewards = new List<RewardData>();
      List<RewardData> equipmentChestRewards = new List<RewardData>();
      List<RewardData> bonusChestRewards = new List<RewardData>();
      foreach (RewardData reward in rewards)
      {
        if (reward.RewardType == Reward.Type.MERCENARY_EQUIPMENT)
          equipmentChestRewards.Add(reward);
        else
          mainChestRewards.Add(reward);
      }
      if (bonusRewards != null)
      {
        foreach (RewardData bonusReward in bonusRewards)
        {
          if (bonusReward.RewardType == Reward.Type.MERCENARY_EQUIPMENT)
            equipmentChestRewards.Add(bonusReward);
          else
            bonusChestRewards.Add(bonusReward);
        }
      }
      Action onMainChestComplete = (Action) (() =>
      {
        if (equipmentChestRewards.Count == 0)
        {
          Action action = doneCallback;
          if (action == null)
            return;
          action();
        }
        else
          RewardUtils.ShowMercenariesEquipmentReward(equipmentChestRewards, doneCallback, fromNotice, noticeID);
      });
      if (mainChestRewards.Count == 0 && bonusChestRewards.Count == 0 && equipmentChestRewards.Count > 0)
      {
        RewardUtils.ShowMercenariesEquipmentReward(equipmentChestRewards, doneCallback, fromNotice, noticeID);
      }
      else
      {
        PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
        {
          ChestRewardDisplay componentInChildren = go.GetComponentInChildren<ChestRewardDisplay>();
          componentInChildren.RegisterDoneCallback(onMainChestComplete);
          if ((UnityEngine.Object) rewardBone != (UnityEngine.Object) null)
            GameUtils.SetParent((Component) componentInChildren.m_parent.transform, (Component) rewardBone);
          if (componentInChildren.ShowRewards_Mercenaries(mainChestRewards, bonusChestRewards, autoOpenChest, fromNotice, noticeID))
            return;
          UnityEngine.Object.Destroy((UnityEngine.Object) go);
        });
        AssetLoader.Get().InstantiatePrefab((AssetReference) "RewardChest_Mercenaries.prefab:7ba36254f98c8914e9b9931bbede3c88", callback);
      }
    }
  }

  public static void ShowMercenariesEquipmentReward(
    List<RewardData> rewards,
    Action doneCallback,
    bool fromNotice = false,
    int noticeID = -1)
  {
    Action doneCallback1 = (Action) (() =>
    {
      if (fromNotice)
        Network.Get().AckNotice((long) noticeID);
      Action action = doneCallback;
      if (action == null)
        return;
      action();
    });
    RewardUtils.LoadAndDisplayRewards(rewards, doneCallback1);
  }

  public static void ShowConsolationMercenariesReward(
    PegasusShared.ProfileNoticeMercenariesRewards.RewardType rewardType,
    RewardListDataModel rewards,
    Transform rewardBone,
    Action doneCallback)
  {
    RewardUtils.ShowMercenariesReward("LettuceConsolationPrize.prefab:8c837b1ecf3fe184eadfca1a3d661f6f", rewards, rewardBone, doneCallback);
  }

  public static void ShowAutoRetireMercenariesReward(
    PegasusShared.ProfileNoticeMercenariesRewards.RewardType rewardType,
    RewardListDataModel rewards,
    Transform rewardBone,
    Action doneCallback)
  {
    RewardUtils.ShowMercenariesReward("LettuceAutorunPrize.prefab:05f50ccdbe9c5994e9dd5b2d19860822", rewards, rewardBone, doneCallback);
  }

  private static void ShowMercenariesReward(
    string prefab,
    RewardListDataModel rewards,
    Transform rewardBone,
    Action doneCallback)
  {
    if (rewards == null || rewards.Items.Count == 0)
    {
      Debug.LogErrorFormat("ShowConsolationMercenariesReward: No rewards provided.");
      Action action = doneCallback;
      if (action == null)
        return;
      action();
    }
    else
    {
      PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
      {
        MercenariesConsolationReward componentInChildren = go.GetComponentInChildren<MercenariesConsolationReward>();
        if ((UnityEngine.Object) rewardBone != (UnityEngine.Object) null)
          GameUtils.SetParent((Component) go.transform, (Component) rewardBone);
        componentInChildren.RegisterDoneCallback(doneCallback);
        componentInChildren.RegisterDoneCallback((Action) (() => UnityEngine.Object.Destroy((UnityEngine.Object) go, 1f)));
        componentInChildren.VisualController.BindDataModel((IDataModel) rewards);
      });
      AssetLoader.Get().InstantiatePrefab((AssetReference) prefab, callback);
    }
  }

  public static void ShowMercenaryFullyUpgraded(
    LettuceMercenaryDataModel mercenary,
    Transform rewardBone,
    Action doneCallback)
  {
    if (mercenary == null)
    {
      Debug.LogErrorFormat("ShowMercenaryFullyUpgraded: No mercenary provided.");
      Action action = doneCallback;
      if (action == null)
        return;
      action();
    }
    else
    {
      PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
      {
        MercenaryFullyUpgraded componentInChildren = go.GetComponentInChildren<MercenaryFullyUpgraded>();
        if ((UnityEngine.Object) rewardBone != (UnityEngine.Object) null)
          GameUtils.SetParent((Component) go.transform, (Component) rewardBone);
        componentInChildren.RegisterDoneCallback(doneCallback);
        componentInChildren.RegisterDoneCallback((Action) (() => UnityEngine.Object.Destroy((UnityEngine.Object) go, 1f)));
        componentInChildren.VisualController.BindDataModel((IDataModel) mercenary);
        componentInChildren.VisualController.SetState("SHOW");
      });
      AssetLoader.Get().InstantiatePrefab((AssetReference) "MercenariesMaxedOutReward.prefab:57fbf1dc798a43547b597a5d63e18271", callback);
    }
  }

  public static void ShowTavernBrawlRewards(
    int wins,
    List<RewardData> rewards,
    Transform rewardBone,
    Action doneCallback,
    bool fromNotice = false,
    NetCache.ProfileNoticeTavernBrawlRewards notice = null)
  {
    int num = fromNotice ? (int) notice.Mode : (int) TavernBrawlManager.Get().CurrentSeasonBrawlMode;
    long noticeID = notice == null ? 0L : notice.NoticeID;
    if (num == 0)
      RewardUtils.ShowSessionTavernBrawlRewards(wins, rewards, rewardBone, doneCallback, fromNotice, noticeID);
    else
      RewardUtils.ShowHeroicSessionTavernBrawlRewards(wins, rewards, rewardBone, doneCallback, fromNotice, noticeID);
  }

  public static void ShowSessionTavernBrawlRewards(
    int wins,
    List<RewardData> rewards,
    Transform rewardBone,
    Action doneCallback,
    bool fromNotice = false,
    long noticeID = -1)
  {
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      ChestRewardDisplay componentInChildren = go.GetComponentInChildren<ChestRewardDisplay>();
      componentInChildren.RegisterDoneCallback(doneCallback);
      GameUtils.SetParent((Component) componentInChildren.m_parent.transform, (Component) rewardBone);
      if (componentInChildren.ShowRewards_TavernBrawl(wins, rewards, rewardBone, fromNotice, noticeID))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) "RewardChest_Lock.prefab:06ffa33e82036694e8cacb96aa7b48e8", callback);
  }

  public static void ShowLeaguePromotionRewards(
    int leagueId,
    List<RewardData> rewards,
    Transform rewardBone,
    Action doneCallback,
    bool fromNotice = false,
    long noticeID = -1)
  {
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      ChestRewardDisplay componentInChildren = go.GetComponentInChildren<ChestRewardDisplay>();
      componentInChildren.RegisterDoneCallback(doneCallback);
      GameUtils.SetParent((Component) componentInChildren.m_parent.transform, (Component) rewardBone);
      if (componentInChildren.ShowRewards_LeaguePromotion(leagueId, rewards, rewardBone, fromNotice, noticeID))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) "RewardChest_Lock.prefab:06ffa33e82036694e8cacb96aa7b48e8", callback);
  }

  public static void ShowHeroicSessionTavernBrawlRewards(
    int wins,
    List<RewardData> rewards,
    Transform rewardBone,
    Action doneCallback,
    bool fromNotice = false,
    long noticeID = -1)
  {
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      HeroicBrawlRewardDisplay component = go.GetComponent<HeroicBrawlRewardDisplay>();
      component.RegisterDoneCallback(doneCallback);
      TransformUtil.AttachAndPreserveLocalTransform(component.transform, rewardBone);
      component.ShowRewards(wins, rewards, fromNotice, noticeID);
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) "HeroicBrawlReward.prefab:8f49f1fcb5ca4485d9b6b22993e1b1ab", callback);
  }

  public static PegasusShared.RewardChest GenerateTavernBrawlRewardChest_CHEAT(
    int wins,
    TavernBrawlMode mode)
  {
    PegasusShared.RewardChest rewardChestCheat = new PegasusShared.RewardChest();
    PegasusShared.RewardBag rewardBag1 = new PegasusShared.RewardBag()
    {
      RewardBooster = new PegasusShared.ProfileNoticeRewardBooster()
    };
    rewardBag1.RewardBooster.BoosterType = 1;
    int num1 = 0;
    int num2 = 0;
    switch (wins)
    {
      case 0:
        num1 = 1;
        break;
      case 1:
        num1 = 2;
        break;
      case 2:
        num1 = 4;
        break;
      case 3:
        num1 = 4;
        num2 = 120;
        break;
      case 4:
        num1 = 5;
        num2 = 230;
        break;
      case 5:
        num1 = 6;
        num2 = 260;
        break;
      case 6:
        num1 = 7;
        num2 = 290;
        break;
      case 7:
        num1 = 8;
        num2 = 320;
        break;
      case 8:
        num1 = 9;
        num2 = 350;
        break;
      case 9:
        num1 = 14;
        num2 = 500;
        break;
      case 10:
        num1 = 15;
        num2 = 550;
        break;
      case 11:
        num1 = 20;
        num2 = 600;
        break;
      case 12:
        num1 = 50;
        num2 = 1000;
        break;
    }
    rewardBag1.RewardBooster.BoosterCount = num1;
    rewardChestCheat.Bag.Add(rewardBag1);
    if (wins > 2)
    {
      PegasusShared.RewardBag rewardBag2 = new PegasusShared.RewardBag()
      {
        RewardDust = new PegasusShared.ProfileNoticeRewardDust()
      };
      rewardBag2.RewardDust.Amount = num2 + UnityEngine.Random.Range(-4, 4) * 5;
      PegasusShared.RewardBag rewardBag3 = new PegasusShared.RewardBag()
      {
        RewardGold = new PegasusShared.ProfileNoticeRewardCurrency()
      };
      rewardBag3.RewardGold.Amount = num2 + UnityEngine.Random.Range(-4, 4) * 5;
      rewardChestCheat.Bag.Add(rewardBag3);
      rewardChestCheat.Bag.Add(rewardBag2);
    }
    if (wins > 9)
    {
      PegasusShared.RewardBag rewardBag4 = new PegasusShared.RewardBag()
      {
        RewardCard = new PegasusShared.ProfileNoticeRewardCard()
      };
      rewardBag4.RewardCard.Card = new PegasusShared.CardDef();
      rewardBag4.RewardCard.Card.Premium = 1;
      rewardBag4.RewardCard.Card.Asset = 834;
      rewardChestCheat.Bag.Add(rewardBag4);
    }
    if (wins > 10)
    {
      PegasusShared.RewardBag rewardBag5 = new PegasusShared.RewardBag()
      {
        RewardCard = new PegasusShared.ProfileNoticeRewardCard()
      };
      rewardBag5.RewardCard.Card = new PegasusShared.CardDef();
      rewardBag5.RewardCard.Card.Premium = 1;
      rewardBag5.RewardCard.Card.Asset = 374;
      rewardChestCheat.Bag.Add(rewardBag5);
    }
    if (wins > 11 && mode == TavernBrawlMode.TB_MODE_HEROIC)
    {
      PegasusShared.RewardBag rewardBag6 = new PegasusShared.RewardBag()
      {
        RewardCard = new PegasusShared.ProfileNoticeRewardCard()
      };
      rewardBag6.RewardCard.Card = new PegasusShared.CardDef();
      rewardBag6.RewardCard.Card.Premium = 1;
      rewardBag6.RewardCard.Card.Asset = 640;
      rewardChestCheat.Bag.Add(rewardBag6);
    }
    return rewardChestCheat;
  }

  public static PegasusShared.RewardChest GenerateMercenariesMapRewardChest_CHEAT()
  {
    PegasusShared.RewardChest rewardChestCheat = new PegasusShared.RewardChest();
    PegasusShared.RewardBag rewardBag1 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag1.RewardMercenariesCurrency.CurrencyDelta = 98L;
    rewardBag1.RewardMercenariesCurrency.MercenaryId = 1;
    rewardChestCheat.Bag.Add(rewardBag1);
    PegasusShared.RewardBag rewardBag2 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag2.RewardMercenariesCurrency.CurrencyDelta = 87L;
    rewardBag2.RewardMercenariesCurrency.MercenaryId = 100;
    rewardChestCheat.Bag.Add(rewardBag2);
    PegasusShared.RewardBag rewardBag3 = new PegasusShared.RewardBag()
    {
      RewardMercenariesExperience = new ProfileNoticeRewardMercenariesExperience()
    };
    rewardBag3.RewardMercenariesExperience.ExpDelta = 76L;
    rewardBag3.RewardMercenariesExperience.PreExp = 1000L;
    rewardBag3.RewardMercenariesExperience.PostExp = 1076L;
    rewardBag3.RewardMercenariesExperience.MercenaryId = 6;
    rewardChestCheat.Bag.Add(rewardBag3);
    PegasusShared.RewardBag rewardBag4 = new PegasusShared.RewardBag()
    {
      RewardMercenariesExperience = new ProfileNoticeRewardMercenariesExperience()
    };
    rewardBag4.RewardMercenariesExperience.ExpDelta = 300L;
    rewardBag4.RewardMercenariesExperience.PreExp = 0L;
    rewardBag4.RewardMercenariesExperience.PostExp = 300L;
    rewardBag4.RewardMercenariesExperience.MercenaryId = 7;
    rewardChestCheat.Bag.Add(rewardBag4);
    PegasusShared.RewardBag rewardBag5 = new PegasusShared.RewardBag()
    {
      RewardMercenariesExperience = new ProfileNoticeRewardMercenariesExperience()
    };
    rewardBag5.RewardMercenariesExperience.ExpDelta = 10001L;
    rewardBag5.RewardMercenariesExperience.PreExp = 0L;
    rewardBag5.RewardMercenariesExperience.PostExp = 10001L;
    rewardBag5.RewardMercenariesExperience.MercenaryId = 8;
    rewardChestCheat.Bag.Add(rewardBag5);
    PegasusShared.RewardBag rewardBag6 = new PegasusShared.RewardBag()
    {
      RewardMercenariesEquipment = new ProfileNoticeRewardMercenariesEquipment()
    };
    rewardBag6.RewardMercenariesEquipment.EquipmentId = 158;
    rewardBag6.RewardMercenariesEquipment.EquipmentTier = 4U;
    rewardBag6.RewardMercenariesEquipment.MercenaryId = 100;
    rewardChestCheat.Bag.Add(rewardBag6);
    return rewardChestCheat;
  }

  public static PegasusShared.RewardChest GenerateMercenariesConsolationReward_CHEAT()
  {
    PegasusShared.RewardChest consolationRewardCheat = new PegasusShared.RewardChest();
    PegasusShared.RewardBag rewardBag1 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag1.RewardMercenariesCurrency.CurrencyDelta = 98L;
    rewardBag1.RewardMercenariesCurrency.MercenaryId = 100;
    consolationRewardCheat.Bag.Add(rewardBag1);
    PegasusShared.RewardBag rewardBag2 = new PegasusShared.RewardBag()
    {
      RewardMercenariesExperience = new ProfileNoticeRewardMercenariesExperience()
    };
    rewardBag2.RewardMercenariesExperience.ExpDelta = 13370L;
    rewardBag2.RewardMercenariesExperience.PreExp = 0L;
    rewardBag2.RewardMercenariesExperience.PostExp = 13370L;
    rewardBag2.RewardMercenariesExperience.MercenaryId = 1;
    consolationRewardCheat.Bag.Add(rewardBag2);
    PegasusShared.RewardBag rewardBag3 = new PegasusShared.RewardBag()
    {
      RewardMercenariesExperience = new ProfileNoticeRewardMercenariesExperience()
    };
    rewardBag3.RewardMercenariesExperience.ExpDelta = 76L;
    rewardBag3.RewardMercenariesExperience.PreExp = 1000L;
    rewardBag3.RewardMercenariesExperience.PostExp = 1076L;
    rewardBag3.RewardMercenariesExperience.MercenaryId = 6;
    consolationRewardCheat.Bag.Add(rewardBag3);
    return consolationRewardCheat;
  }

  public static PegasusShared.RewardChest GenerateMercenariesSeasonReward_CHEAT()
  {
    PegasusShared.RewardChest seasonRewardCheat = new PegasusShared.RewardChest();
    PegasusShared.RewardBag rewardBag1 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag1.RewardMercenariesCurrency.CurrencyDelta = 98L;
    rewardBag1.RewardMercenariesCurrency.MercenaryId = 1;
    seasonRewardCheat.Bag.Add(rewardBag1);
    PegasusShared.RewardBag rewardBag2 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag2.RewardMercenariesCurrency.CurrencyDelta = 87L;
    rewardBag2.RewardMercenariesCurrency.MercenaryId = 100;
    seasonRewardCheat.Bag.Add(rewardBag2);
    PegasusShared.RewardBag rewardBag3 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag3.RewardMercenariesCurrency.CurrencyDelta = 12L;
    rewardBag3.RewardMercenariesCurrency.MercenaryId = 7;
    seasonRewardCheat.Bag.Add(rewardBag3);
    PegasusShared.RewardBag rewardBag4 = new PegasusShared.RewardBag()
    {
      RewardBooster = new PegasusShared.ProfileNoticeRewardBooster()
    };
    rewardBag4.RewardBooster.BoosterType = 629;
    rewardBag4.RewardBooster.BoosterCount = 3;
    seasonRewardCheat.Bag.Add(rewardBag4);
    PegasusShared.RewardBag rewardBag5 = new PegasusShared.RewardBag()
    {
      RewardRandomMercenary = new PegasusShared.ProfileNoticeMercenariesRandomRewardLicense()
    };
    rewardBag5.RewardRandomMercenary.MercenaryId = 2;
    rewardBag5.RewardRandomMercenary.ArtVariationId = 70;
    rewardBag5.RewardRandomMercenary.ArtVariationPremium = 2U;
    seasonRewardCheat.Bag.Add(rewardBag5);
    PegasusShared.RewardBag rewardBag6 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag6.RewardMercenariesCurrency.CurrencyDelta = 50L;
    rewardBag6.RewardMercenariesCurrency.MercenaryId = 102;
    seasonRewardCheat.Bag.Add(rewardBag6);
    PegasusShared.RewardBag rewardBag7 = new PegasusShared.RewardBag()
    {
      RewardMercenariesCurrency = new ProfileNoticeRewardMercenariesCurrency()
    };
    rewardBag7.RewardMercenariesCurrency.CurrencyDelta = 42L;
    rewardBag7.RewardMercenariesCurrency.MercenaryId = 7;
    seasonRewardCheat.Bag.Add(rewardBag7);
    PegasusShared.RewardBag rewardBag8 = new PegasusShared.RewardBag()
    {
      RewardRandomMercenary = new PegasusShared.ProfileNoticeMercenariesRandomRewardLicense()
    };
    rewardBag8.RewardRandomMercenary.MercenaryId = 2;
    rewardBag8.RewardRandomMercenary.ArtVariationId = 70;
    rewardBag8.RewardRandomMercenary.ArtVariationPremium = 1U;
    rewardBag8.RewardRandomMercenary.CurrencyAmount = 123L;
    seasonRewardCheat.Bag.Add(rewardBag8);
    return seasonRewardCheat;
  }

  public static void SetQuestTileNameLinePosition(
    GameObject nameLine,
    UberText questName,
    float padding)
  {
    int num = questName.isHidden() ? 1 : 0;
    if (num != 0)
      questName.Show();
    TransformUtil.SetPoint(nameLine, Anchor.TOP, (Component) questName, Anchor.BOTTOM);
    nameLine.transform.localPosition = new Vector3(nameLine.transform.localPosition.x, nameLine.transform.localPosition.y, nameLine.transform.localPosition.z + padding);
    if (num == 0)
      return;
    questName.Hide();
  }

  public static RewardChestContentsDbfRecord GetRewardChestContents(
    int rewardChestAssetId,
    int rewardLevel)
  {
    return GameDbf.RewardChest.HasRecord(rewardChestAssetId) ? GameDbf.RewardChestContents.GetRecord((Predicate<RewardChestContentsDbfRecord>) (r => r.RewardChestId == rewardChestAssetId && r.RewardLevel == rewardLevel)) : (RewardChestContentsDbfRecord) null;
  }

  public static List<RewardData> GetRewardDataFromRewardChestAsset(
    int rewardChestAssetId,
    int rewardLevel)
  {
    List<RewardData> rewardData = new List<RewardData>();
    RewardChestContentsDbfRecord rewardChestContents = RewardUtils.GetRewardChestContents(rewardChestAssetId, rewardLevel);
    if (rewardChestContents != null)
    {
      int seasonId = 0;
      RewardUtils.AddRewardDataStubForBag(rewardChestContents.Bag1, seasonId, ref rewardData);
      RewardUtils.AddRewardDataStubForBag(rewardChestContents.Bag2, seasonId, ref rewardData);
      RewardUtils.AddRewardDataStubForBag(rewardChestContents.Bag3, seasonId, ref rewardData);
      RewardUtils.AddRewardDataStubForBag(rewardChestContents.Bag4, seasonId, ref rewardData);
      RewardUtils.AddRewardDataStubForBag(rewardChestContents.Bag5, seasonId, ref rewardData);
    }
    return rewardData;
  }

  public static void AddRewardDataStubForBag(
    int bagId,
    int seasonId,
    ref List<RewardData> rewardData)
  {
    RewardBagDbfRecord rewardBagDbfRecord = (RewardBagDbfRecord) null;
    List<RewardBagDbfRecord> records = GameDbf.RewardBag.GetRecords();
    int index = 0;
    for (int count = records.Count; index < count; ++index)
    {
      if (records[index].BagId == bagId)
      {
        rewardBagDbfRecord = records[index];
        break;
      }
    }
    if (rewardBagDbfRecord == null)
      return;
    switch (rewardBagDbfRecord.Reward)
    {
      case Assets.RewardBag.Reward.GOLD:
        rewardData.Add((RewardData) new GoldRewardData((long) rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.DUST:
        rewardData.Add((RewardData) new ArcaneDustRewardData(rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.COM:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.COMMON, TAG_PREMIUM.NORMAL, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.RARE:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.RARE, TAG_PREMIUM.NORMAL, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.EPIC:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.EPIC, TAG_PREMIUM.NORMAL, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.LEG:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.LEGENDARY, TAG_PREMIUM.NORMAL, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.GRARE:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.RARE, TAG_PREMIUM.GOLDEN, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.GCOM:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.COMMON, TAG_PREMIUM.GOLDEN, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.GEPIC:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.EPIC, TAG_PREMIUM.GOLDEN, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.GLEG:
        rewardData.Add((RewardData) new RandomCardRewardData(TAG_RARITY.LEGENDARY, TAG_PREMIUM.GOLDEN, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.LATEST_PACK:
        rewardData.Add((RewardData) new BoosterPackRewardData(rewardBagDbfRecord.RewardData, rewardBagDbfRecord.Base, new int?(rewardBagDbfRecord.BagId)));
        break;
      case Assets.RewardBag.Reward.RANDOM_CARD:
        rewardData.Add((RewardData) new RandomCardRewardData(RewardUtils.GetRarityForRandomCardReward(rewardBagDbfRecord.RewardData), TAG_PREMIUM.NORMAL));
        break;
      case Assets.RewardBag.Reward.SPECIFIC_PACK:
        rewardData.Add((RewardData) new BoosterPackRewardData(rewardBagDbfRecord.RewardData, rewardBagDbfRecord.Base));
        break;
      case Assets.RewardBag.Reward.REWARD_CHEST_CONTENTS:
        RewardChestContentsDbfRecord record = GameDbf.RewardChestContents.GetRecord(rewardBagDbfRecord.RewardData);
        if (record == null)
        {
          Log.All.PrintWarning("No reward chest contents of id {0} found on client for random card reward", (object) rewardBagDbfRecord.RewardData);
          break;
        }
        RewardUtils.ProcessRewardChestContents(record, seasonId, ref rewardData);
        break;
      case Assets.RewardBag.Reward.RANKED_SEASON_REWARD_PACK:
        int boosterIdForSeasonId = RankMgr.Get().GetRankedRewardBoosterIdForSeasonId(seasonId);
        rewardData.Add((RewardData) new BoosterPackRewardData(boosterIdForSeasonId, rewardBagDbfRecord.Base));
        break;
    }
  }

  public static void ProcessRewardChestContents(
    RewardChestContentsDbfRecord rewardChestContents,
    int seasonId,
    ref List<RewardData> rewardData)
  {
    foreach (int num in new List<int>()
    {
      rewardChestContents.Bag1,
      rewardChestContents.Bag2,
      rewardChestContents.Bag3,
      rewardChestContents.Bag4,
      rewardChestContents.Bag5
    })
    {
      int bag = num;
      if (bag != 0)
      {
        RewardBagDbfRecord record = GameDbf.RewardBag.GetRecord((Predicate<RewardBagDbfRecord>) (r => r.BagId == bag));
        if (record != null && !(record.Reward.ToString().ToLower() == "reward_chest_contents"))
          RewardUtils.AddRewardDataStubForBag(bag, seasonId, ref rewardData);
      }
    }
  }

  public static void ShowRewardBoxes(
    List<RewardData> rewards,
    Action doneCallback,
    Transform bone = null,
    bool useLocalPosition = false,
    GameLayer layer = GameLayer.IgnoreFullScreenEffects,
    bool useDarkeningClickCatcher = false)
  {
    GameObjectCallback callback = (GameObjectCallback) ((assetRef, go, callbackData) =>
    {
      if (SoundManager.Get() != null)
        SoundManager.Get().LoadAndPlay((AssetReference) "card_turn_over_legendary.prefab:a8140f686bff601459e954bc23de35e0");
      RewardBoxesDisplay component = go.GetComponent<RewardBoxesDisplay>();
      component.SetRewards(rewards);
      component.m_playBoxFlyoutSound = false;
      component.SetLayer(layer);
      component.UseDarkeningClickCatcher(useDarkeningClickCatcher);
      component.RegisterDoneCallback(doneCallback);
      if ((UnityEngine.Object) bone != (UnityEngine.Object) null)
      {
        if (useLocalPosition)
          component.transform.localPosition = bone.localPosition;
        else
          component.transform.position = bone.position;
        component.transform.localRotation = bone.localRotation;
        component.transform.localScale = bone.localScale;
      }
      component.AnimateRewards();
    });
    AssetLoader.Get().LoadGameObject((AssetReference) RewardBoxesDisplay.GetPrefab(rewards), callback);
  }

  public static TAG_RARITY GetRarityForRandomCardReward(int boosterCardSetId)
  {
    BoosterCardSetDbfRecord record = GameDbf.BoosterCardSet.GetRecord(boosterCardSetId);
    if (record == null)
    {
      Log.All.PrintWarning("No BoosterCardSet of id [{0}] found)", (object) boosterCardSetId);
      return TAG_RARITY.INVALID;
    }
    SubsetDbfRecord subsetRecord = record.SubsetRecord;
    if (subsetRecord == null)
    {
      Log.All.PrintWarning("No subset of id {0} found on client for random card reward on boosterCardSet {1}", (object) record.SubsetId, (object) record.ID);
      return TAG_RARITY.INVALID;
    }
    IEnumerable<SubsetRuleDbfRecord> source = subsetRecord.Rules.Where<SubsetRuleDbfRecord>((Func<SubsetRuleDbfRecord, bool>) (r => r.Tag == 203));
    SubsetRuleDbfRecord subsetRuleDbfRecord = source.FirstOrDefault<SubsetRuleDbfRecord>();
    if (source.Count<SubsetRuleDbfRecord>() == 1 && subsetRuleDbfRecord != null && !subsetRuleDbfRecord.RuleIsNot && subsetRuleDbfRecord.MinValue == subsetRuleDbfRecord.MaxValue)
      return (TAG_RARITY) subsetRuleDbfRecord.MinValue;
    Log.All.PrintWarning("Random card display requires exactly one rarity rule to specify a single rarity (subset id [{0}])", (object) subsetRecord.ID);
    return TAG_RARITY.INVALID;
  }

  public static UserAttentionBlocker GetUserAttentionBlockerForReward(
    NetCache.ProfileNotice.NoticeOrigin origin,
    long originData)
  {
    if (origin != NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT && origin != NetCache.ProfileNotice.NoticeOrigin.GENERIC_REWARD_CHEST_ACHIEVE)
      return UserAttentionBlocker.NONE;
    AchieveDbfRecord record = GameDbf.Achieve.GetRecord((int) originData);
    return record == null ? UserAttentionBlocker.NONE : (UserAttentionBlocker) record.AttentionBlocker;
  }

  public static bool IsMercenaryRewardPortrait(LettuceMercenaryDataModel rewardData)
  {
    MercenaryArtVariationDbfRecord variationDbfRecord = GameDbf.LettuceMercenary.GetRecord(rewardData.MercenaryId).MercenaryArtVariations.First<MercenaryArtVariationDbfRecord>((Func<MercenaryArtVariationDbfRecord, bool>) (e => e.CardRecord.NoteMiniGuid == rewardData.Card.CardId));
    return rewardData.Card.Premium > TAG_PREMIUM.NORMAL || !variationDbfRecord.DefaultVariation;
  }

  public static bool IsRequiredDataLoadedToShowReward(Reward reward)
  {
    switch (reward.RewardType)
    {
      case Reward.Type.MERCENARY_EXP:
      case Reward.Type.MERCENARY_ABILITY_UNLOCK:
      case Reward.Type.MERCENARY_EQUIPMENT:
        if (!CollectionManager.Get().IsLettuceLoaded())
          return false;
        break;
    }
    return true;
  }

  public static bool IsRequiredContextForReward(Reward reward)
  {
    switch (reward.RewardType)
    {
      case Reward.Type.MERCENARY_EXP:
      case Reward.Type.MERCENARY_ABILITY_UNLOCK:
        if (!SceneMgr.Get().IsInLettuceMode())
          return false;
        break;
    }
    return true;
  }

  public static void GetTitleAndDescriptionFromReward(
    Reward reward,
    out string title,
    out string description)
  {
    RewardData data1 = reward.Data;
    title = data1.NameOverride;
    description = data1.DescriptionOverride;
    bool flag1 = string.IsNullOrEmpty(title);
    bool flag2 = string.IsNullOrEmpty(description);
    if (!(flag1 | flag2))
      return;
    switch (data1.RewardType)
    {
      case Reward.Type.CARD:
        CardRewardData card = data1 as CardRewardData;
        EntityDef entityDef = DefLoader.Get().GetEntityDef(card.CardID);
        ProductClientDataDbfRecord record1 = GameDbf.ProductClientData.GetRecord((Predicate<ProductClientDataDbfRecord>) (r => r.PmtProductId == card.OriginData));
        if (record1 != null)
        {
          title = GameStrings.FormatLocalizedString((string) record1.PopupTitle);
          description = GameStrings.FormatLocalizedString((string) record1.PopupBody, (object) entityDef.GetName());
          break;
        }
        RewardBanner rewardBanner = reward.m_rewardBanner;
        title = rewardBanner.HeadlineText;
        description = rewardBanner.DetailsText;
        break;
      case Reward.Type.MINI_SET:
        MiniSetRewardData data2 = reward.Data as MiniSetRewardData;
        MiniSetDbfRecord record2 = GameDbf.MiniSet.GetRecord(data2.MiniSetID);
        int count = record2.DeckRecord.Cards.Count;
        if (data2.Premium == 1)
          title = GameStrings.FormatLocalizedString((string) record2.GoldenName);
        if (string.IsNullOrEmpty(title))
          title = GameStrings.FormatLocalizedString((string) record2.DeckRecord.Name);
        description = GameStrings.FormatLocalizedString((string) record2.DeckRecord.Description, (object) count);
        break;
      case Reward.Type.BATTLEGROUNDS_FINISHER:
        BattlegroundsFinisherDataModel finisherDataModel = data1 is BattlegroundsFinisherRewardData finisherRewardData ? finisherRewardData.DataModel : (BattlegroundsFinisherDataModel) null;
        if (finisherDataModel == null)
          break;
        if (flag1)
          title = finisherDataModel.DetailsDisplayName;
        if (!flag2)
          break;
        description = finisherDataModel.Description;
        break;
      case Reward.Type.BATTLEGROUNDS_BOARD_SKIN:
        BattlegroundsBoardSkinDataModel boardSkinDataModel = data1 is BattlegroundsBoardSkinRewardData boardSkinRewardData ? boardSkinRewardData.DataModel : (BattlegroundsBoardSkinDataModel) null;
        if (boardSkinDataModel == null)
          break;
        if (flag1)
          title = boardSkinDataModel.DetailsDisplayName;
        if (!flag2)
          break;
        description = boardSkinDataModel.Description;
        break;
      case Reward.Type.BATTLEGROUNDS_EMOTE:
        BattlegroundsEmoteDataModel battlegroundsEmoteDataModel = data1 is BattlegroundsEmoteRewardData battlegroundsEmoteRewardData ? battlegroundsEmoteRewardData.DataModel : (BattlegroundsEmoteDataModel) null;
        if (battlegroundsEmoteDataModel == null)
          break;
        if (flag1)
          title = battlegroundsEmoteDataModel.DisplayName;
        if (!flag2)
          break;
        description = battlegroundsEmoteDataModel.Description;
        break;
    }
  }

  private static bool ShowReward_Internal(
    UserAttentionBlocker blocker,
    Reward reward,
    bool updateCacheValues,
    Vector3 rewardPunchScale,
    Vector3 rewardScale,
    string gameObjectCallbackName,
    GameObject callbackGO,
    AnimationUtil.DelOnShownWithPunch onShowPunchCallback,
    object callbackData)
  {
    if ((UnityEngine.Object) reward == (UnityEngine.Object) null)
      return false;
    int attentionCategory = (int) blocker;
    string str;
    if (!((UnityEngine.Object) reward == (UnityEngine.Object) null) && reward.Data != null)
      str = reward.Data.Origin.ToString() + ":" + (object) reward.Data.OriginData + ":" + (object) reward.Data.RewardType;
    else
      str = "null";
    string callerName = "RewardUtils.ShowReward:" + str;
    if (!UserAttentionManager.CanShowAttentionGrabber((UserAttentionBlocker) attentionCategory, callerName))
      return false;
    Log.Achievements.Print("RewardUtils: Showing Reward: reward={0} reward.Data={1}", (object) reward, (object) reward.Data);
    AnimationUtil.ShowWithPunch(reward.gameObject, RewardUtils.RewardHiddenScale, rewardPunchScale, rewardScale, gameObjectCallbackName, true, callbackGO, callbackData, onShowPunchCallback);
    reward.Show(updateCacheValues);
    RewardUtils.ShowInnkeeperQuoteForReward(reward);
    return true;
  }

  private static CardRewardData GetDuplicateCardDataReward(
    RewardData newRewardData,
    List<RewardData> existingRewardData)
  {
    if (!(newRewardData is CardRewardData))
      return (CardRewardData) null;
    CardRewardData newCardRewardData = newRewardData as CardRewardData;
    return existingRewardData.Find((Predicate<RewardData>) (obj =>
    {
      if (!(obj is CardRewardData))
        return false;
      CardRewardData cardRewardData = obj as CardRewardData;
      return cardRewardData.CardID.Equals(newCardRewardData.CardID) && cardRewardData.Premium.Equals((object) newCardRewardData.Premium) && cardRewardData.Origin.Equals((object) newCardRewardData.Origin) && cardRewardData.OriginData.Equals(newCardRewardData.OriginData);
    })) as CardRewardData;
  }

  private static void ShowInnkeeperQuoteForReward(Reward reward)
  {
    if ((UnityEngine.Object) reward == (UnityEngine.Object) null || Reward.Type.CARD != reward.RewardType)
      return;
    switch ((reward.Data as CardRewardData).InnKeeperLine)
    {
      case CardRewardData.InnKeeperTrigger.CORE_CLASS_SET_COMPLETE:
        Notification innkeeperQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_BASIC_DONE1_11"), "VO_INNKEEPER_BASIC_DONE1_11.prefab:9b8f8ab262305c54dbb6c847ac8b1fdb");
        if (Options.Get().GetBool(Option.HAS_SEEN_ALL_BASIC_CLASS_CARDS_COMPLETE, false))
          break;
        Processor.RunCoroutine(RewardUtils.NotifyOfExpertPacksNeeded(innkeeperQuote));
        break;
      case CardRewardData.InnKeeperTrigger.SECOND_REWARD_EVER:
        if (Options.Get().GetBool(Option.HAS_BEEN_NUDGED_TO_CM, false))
          break;
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_NUDGE_CM_X"), "VO_INNKEEPER2_NUDGE_COLLECTION_10.prefab:b20c7d803cf82fb46830cba5d4bda11e");
        Options.Get().SetBool(Option.HAS_BEEN_NUDGED_TO_CM, true);
        break;
    }
  }

  private static IEnumerator NotifyOfExpertPacksNeeded(Notification innkeeperQuote)
  {
    while ((UnityEngine.Object) innkeeperQuote.GetAudio() == (UnityEngine.Object) null)
      yield return (object) null;
    yield return (object) new WaitForSeconds(innkeeperQuote.GetAudio().clip.length);
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_BASIC_DONE2_12"), "VO_INNKEEPER_BASIC_DONE2_12.prefab:b20f6a03438c5b440a2963095330589c");
    Options.Get().SetBool(Option.HAS_SEEN_ALL_BASIC_CLASS_CARDS_COMPLETE, true);
  }

  public static SimpleRewardData CreateArcaneOrbRewardData(int amount) => new SimpleRewardData(Reward.Type.ARCANE_ORBS, amount)
  {
    RewardHeadlineText = GameStrings.Get("GLOBAL_REWARD_ARCANE_ORBS_HEADLINE")
  };

  public static DeckRewardData CreateDeckRewardData(
    int deckId,
    int classId,
    string deckNameOverride)
  {
    return new DeckRewardData(deckId, classId, deckNameOverride);
  }

  public static bool TryGetSellableDeck(int deckId, out SellableDeckDbfRecord sellableDeckDbfRecord)
  {
    sellableDeckDbfRecord = (SellableDeckDbfRecord) null;
    List<SellableDeckDbfRecord> records = GameDbf.SellableDeck.GetRecords((Predicate<SellableDeckDbfRecord>) (r => r.DeckTemplateRecord != null && r.DeckTemplateRecord.DeckId == deckId));
    if (records.Count == 0)
    {
      Log.Store.PrintWarning("[RewardUtils.TryGetSellableDeck] Failed to find DB record for deck reward! (ID {0})", (object) deckId);
      return false;
    }
    if (records.Count > 1)
    {
      Log.Store.PrintWarning("[RewardUtils.TryGetSellableDeck] Found multiple rewardable deck records that grant the same deck! (ID {0})", (object) deckId);
      return false;
    }
    if (records[0].DeckTemplateRecord?.DeckRecord == null)
    {
      Log.Store.PrintWarning("[RewardUtils.TryGetSellableDeck] The DB record {0} for deck reward does NOT have a deck template with a valid deck record!", (object) records[0].ID);
      return false;
    }
    sellableDeckDbfRecord = records[0];
    return true;
  }

  public static RewardItemDataModel RewardDataToRewardItemDataModel(
    RewardData rewardData)
  {
    TAG_RARITY rarity = TAG_RARITY.INVALID;
    TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
    RewardItemDataModel rewardItemDataModel1 = (RewardItemDataModel) null;
    switch (rewardData.RewardType)
    {
      case Reward.Type.ARCANE_DUST:
        ArcaneDustRewardData arcaneDustRewardData = rewardData as ArcaneDustRewardData;
        rewardItemDataModel1 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.DUST,
          Quantity = arcaneDustRewardData.Amount
        };
        break;
      case Reward.Type.BOOSTER_PACK:
        BoosterPackRewardData boosterPackRewardData = rewardData as BoosterPackRewardData;
        rewardItemDataModel1 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.BOOSTER,
          ItemId = boosterPackRewardData.Id,
          Quantity = boosterPackRewardData.Count
        };
        break;
      case Reward.Type.CARD:
        CardRewardData cardRewardData = rewardData as CardRewardData;
        RewardItemDataModel rewardItemDataModel2 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.CARD,
          ItemId = GameUtils.TranslateCardIdToDbId(cardRewardData.CardID),
          Quantity = cardRewardData.Count
        };
        if (GameDbf.Card.GetRecord(rewardItemDataModel2.ItemId) != null)
        {
          premium = cardRewardData.Premium;
          rewardItemDataModel1 = rewardItemDataModel2;
          break;
        }
        break;
      case Reward.Type.CARD_BACK:
        CardBackRewardData cardBackRewardData = rewardData as CardBackRewardData;
        rewardItemDataModel1 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.CARD_BACK,
          ItemId = cardBackRewardData.CardBackID
        };
        break;
      case Reward.Type.RANDOM_CARD:
        RandomCardRewardData randomCardRewardData = rewardData as RandomCardRewardData;
        rewardItemDataModel1 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.RANDOM_CARD,
          Quantity = randomCardRewardData.Count
        };
        rarity = randomCardRewardData.Rarity;
        premium = randomCardRewardData.Premium;
        break;
      case Reward.Type.ARCANE_ORBS:
        SimpleRewardData simpleRewardData = rewardData as SimpleRewardData;
        rewardItemDataModel1 = new RewardItemDataModel()
        {
          ItemType = RewardItemType.CN_ARCANE_ORBS,
          Quantity = simpleRewardData.Amount
        };
        break;
      default:
        Log.All.PrintWarning("RewardDataToRewardItemDataModel() - RewardData of type {0} is not currently supported!", (object) rewardData.RewardType);
        break;
    }
    if (rewardItemDataModel1 != null)
    {
      string.Format("RewardData Error [Type = {0}]: ", (object) rewardData.RewardType);
      string failReason;
      if (!RewardUtils.InitializeRewardItemDataModel(rewardItemDataModel1, rarity, premium, out failReason))
      {
        Log.All.PrintWarning(string.Format("RewardData Error [Type = {0}]: {1}", (object) rewardData.RewardType, (object) (failReason ?? "Unspecified reason")));
        rewardItemDataModel1 = (RewardItemDataModel) null;
      }
    }
    return rewardItemDataModel1;
  }

  public static RewardListDataModel CreateRewardListDataModelFromRewardListId(
    int rewardListId,
    int chooseOneRewardItemId = 0,
    List<RewardItemOutput> rewardItemOutputs = null)
  {
    return RewardUtils.CreateRewardListDataModelFromRewardListRecord(GameDbf.RewardList.GetRecord(rewardListId), chooseOneRewardItemId, rewardItemOutputs);
  }

  public static RewardListDataModel CreateRewardListDataModelFromRewardListRecord(
    RewardListDbfRecord rewardListRecord,
    int chooseOneRewardItemId = 0,
    List<RewardItemOutput> rewardItemOutputs = null)
  {
    if (rewardListRecord == null)
      return (RewardListDataModel) null;
    return new RewardListDataModel()
    {
      ChooseOne = rewardListRecord.ChooseOne,
      Items = rewardListRecord.RewardItems.Where<RewardItemDbfRecord>((Func<RewardItemDbfRecord, bool>) (r => chooseOneRewardItemId <= 0 || r.ID == chooseOneRewardItemId)).SelectMany<RewardItemDbfRecord, RewardItemDataModel>((Func<RewardItemDbfRecord, IEnumerable<RewardItemDataModel>>) (r => (IEnumerable<RewardItemDataModel>) RewardFactory.CreateRewardItemDataModel(r, rewardItemOutputs?.Find((Predicate<RewardItemOutput>) (rio => rio.RewardItemId == r.ID))?.OutputData))).OrderBy<RewardItemDataModel, RewardItemDataModel>((Func<RewardItemDataModel, RewardItemDataModel>) (item => item), (IComparer<RewardItemDataModel>) new RewardUtils.RewardItemComparer()).ToDataModelList<RewardItemDataModel>(),
      Description = (string) rewardListRecord.Description
    };
  }

  public static bool InitializeRewardItemDataModelForShop(
    RewardItemDataModel item,
    Network.BundleItem netBundleItem,
    Network.Bundle netBundle)
  {
    TAG_RARITY rarity = TAG_RARITY.INVALID;
    TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
    switch (item.ItemType)
    {
      case RewardItemType.HERO_SKIN:
        premium = TAG_PREMIUM.GOLDEN;
        break;
      case RewardItemType.RANDOM_CARD:
        premium = RewardUtils.GetPremiumTypeFromNetBundleAttributes(netBundleItem);
        rarity = RewardUtils.GetRarityForRandomCardReward(item.ItemId);
        break;
      case RewardItemType.CARD:
        premium = RewardUtils.GetPremiumTypeFromNetBundleAttributes(netBundleItem);
        break;
    }
    string failReason;
    if (RewardUtils.InitializeRewardItemDataModel(item, rarity, premium, out failReason))
      return true;
    if ((Record) netBundle != (Record) null)
      ProductIssues.LogError(netBundle, "License or VC grant reward invalid. " + (failReason ?? "Unspecified reason"));
    return false;
  }

  private static TAG_PREMIUM GetPremiumTypeFromNetBundleAttributes(
    Network.BundleItem netBundleItem)
  {
    TAG_PREMIUM premiumType = TAG_PREMIUM.NORMAL;
    if ((Record) netBundleItem != (Record) null)
      netBundleItem.Attributes.GetValue("premium").Match((Action<string>) (premium =>
      {
        if (premium.Equals("1"))
          premiumType = TAG_PREMIUM.GOLDEN;
        else if (premium.Equals("2"))
        {
          premiumType = TAG_PREMIUM.DIAMOND;
        }
        else
        {
          if (!premium.Equals("3"))
            return;
          premiumType = TAG_PREMIUM.SIGNATURE;
        }
      }));
    return premiumType;
  }

  public static bool InitializeRewardItemDataModel(
    RewardItemDataModel item,
    TAG_RARITY rarity,
    TAG_PREMIUM premium,
    out string failReason)
  {
    bool flag = false;
    failReason = (string) null;
    switch (item.ItemType)
    {
      case RewardItemType.BOOSTER:
        if (GameDbf.Booster.HasRecord(item.ItemId))
        {
          item.Booster = new PackDataModel()
          {
            Type = (BoosterDbId) item.ItemId,
            Quantity = item.Quantity
          };
          flag = true;
          break;
        }
        failReason = string.Format("Booster reward has unknown ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.DUST:
      case RewardItemType.CN_RUNESTONES:
      case RewardItemType.CN_ARCANE_ORBS:
      case RewardItemType.ROW_RUNESTONES:
        item.Currency = new PriceDataModel()
        {
          Currency = RewardUtils.RewardItemTypeToCurrencyType(item.ItemType),
          Amount = (float) item.Quantity,
          DisplayText = item.Quantity.ToString()
        };
        CurrencyType currency = item.Currency.Currency;
        if (ShopUtils.IsCurrencyVirtual(currency))
        {
          if (ShopUtils.IsVirtualCurrencyEnabled() && ShopUtils.IsVirtualCurrencyTypeEnabled(currency))
          {
            flag = true;
            break;
          }
          failReason = string.Format("Reward currency {0} is a virtual currency and VC is not enabled/active", (object) currency);
          break;
        }
        flag = true;
        break;
      case RewardItemType.HERO_SKIN:
        CardDbfRecord record1 = GameDbf.Card.GetRecord(item.ItemId);
        if (record1 != null)
        {
          if (record1.CardHero != null)
          {
            item.Card = new CardDataModel()
            {
              CardId = record1.NoteMiniGuid,
              Premium = premium
            };
            flag = true;
            break;
          }
          failReason = string.Format("Hero Skin reward has Card ID {0} with no CARD_HERO subtable. NoteMiniGuid={1}", (object) item.ItemId, (object) record1.NoteMiniGuid);
          break;
        }
        failReason = string.Format("Hero Skin reward has unknown Card ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.CARD_BACK:
        if (GameDbf.CardBack.HasRecord(item.ItemId))
        {
          item.CardBack = new CardBackDataModel()
          {
            CardBackId = item.ItemId
          };
          flag = true;
          break;
        }
        failReason = string.Format("Card Back reward has unknown ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.ADVENTURE_WING:
        if (GameDbf.Wing.HasRecord(item.ItemId))
        {
          flag = true;
          break;
        }
        failReason = string.Format("Adventure Wing reward has unknown ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.ARENA_TICKET:
      case RewardItemType.BATTLEGROUNDS_BONUS:
      case RewardItemType.TAVERN_BRAWL_TICKET:
      case RewardItemType.PROGRESSION_BONUS:
      case RewardItemType.REWARD_TRACK_XP_BOOST:
      case RewardItemType.MERCENARY_COIN:
      case RewardItemType.MERCENARY:
      case RewardItemType.MERCENARY_RANDOM_MERCENARY:
      case RewardItemType.MERCENARY_KNOCKOUT_SPECIFIC:
      case RewardItemType.MERCENARY_KNOCKOUT_RANDOM:
      case RewardItemType.LUCKY_DRAW:
        flag = true;
        break;
      case RewardItemType.RANDOM_CARD:
        item.RandomCard = new RandomCardDataModel()
        {
          Premium = premium,
          Rarity = rarity,
          Count = item.Quantity
        };
        if (rarity != TAG_RARITY.INVALID && Enum.IsDefined(typeof (TAG_RARITY), (object) rarity))
        {
          flag = true;
          break;
        }
        failReason = string.Format("Random card reward has invalid rarity {0}", (object) rarity);
        break;
      case RewardItemType.CARD:
        CardDbfRecord record2 = GameDbf.Card.GetRecord(item.ItemId);
        if (record2 != null)
        {
          item.Card = new CardDataModel()
          {
            CardId = record2.NoteMiniGuid,
            Premium = premium
          };
          flag = true;
          break;
        }
        failReason = string.Format("Card reward has unknown ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.CUSTOM_COIN:
        CoinDbfRecord record3 = GameDbf.Coin.GetRecord(item.ItemId);
        if (record3 != null)
        {
          CardDbfRecord record4 = GameDbf.Card.GetRecord(record3.CardId);
          if (record4 != null)
          {
            item.Card = new CardDataModel()
            {
              CardId = record4.NoteMiniGuid,
              Premium = premium
            };
            flag = true;
            break;
          }
          failReason = string.Format("Custom Coin reward {0} has unknown Card ID in COIN table {1}", (object) item.ItemId, (object) record3.CardId);
          break;
        }
        failReason = string.Format("Custom Coin reward has unknown ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.MINI_SET:
        if (GameDbf.MiniSet.HasRecord(item.ItemId))
        {
          flag = true;
          break;
        }
        failReason = string.Format("Mini Set reward has unknown ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.SELLABLE_DECK:
        if (GameDbf.SellableDeck.HasRecord(item.ItemId))
        {
          flag = true;
          break;
        }
        failReason = string.Format("Sellable Deck reward has unknown ID {0}", (object) item.ItemId);
        break;
      case RewardItemType.MERCENARY_BOOSTER:
        item.Booster = new PackDataModel()
        {
          Type = BoosterDbId.MERCENARIES,
          Quantity = item.Quantity
        };
        flag = true;
        break;
      case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
        if (CollectionManager.Get().IsBattlegroundsHeroSkinCard(item.ItemId))
        {
          CardDbfRecord record5 = GameDbf.Card.GetRecord(item.ItemId);
          if (record5 != null)
          {
            if (record5.CardHero != null)
            {
              item.Card = new CardDataModel()
              {
                CardId = record5.NoteMiniGuid,
                Premium = premium
              };
              flag = true;
              break;
            }
            failReason = string.Format("Battlegrounds Hero Skin reward has Card ID {0} with no CARD_HERO subtable. NoteMiniGuid={1}", (object) item.ItemId, (object) record5.NoteMiniGuid);
            break;
          }
          failReason = string.Format("Battlegrounds Hero Skin reward has unknown Card ID {0}", (object) item.ItemId);
          break;
        }
        failReason = string.Format("Battlegrounds Hero Skin reward has Card ID {0} with no corresponding entry in the battlegrounds hero skin table", (object) item.ItemId);
        break;
      case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
        if (CollectionManager.Get().IsBattlegroundsGuideSkinCard(item.ItemId))
        {
          CardDbfRecord record6 = GameDbf.Card.GetRecord(item.ItemId);
          if (record6 != null)
          {
            if (record6.CardHero != null)
            {
              item.Card = new CardDataModel()
              {
                CardId = record6.NoteMiniGuid,
                Premium = premium
              };
              flag = true;
              break;
            }
            failReason = string.Format("Battlegrounds Guide Skin reward has Card ID {0} with no CARD_HERO subtable. NoteMiniGuid={1}", (object) item.ItemId, (object) record6.NoteMiniGuid);
            break;
          }
          failReason = string.Format("Battlegrounds Guide Skin reward has unknown Card ID {0}", (object) item.ItemId);
          break;
        }
        failReason = string.Format("Battlegrounds Guide Skin reward has Card ID {0} with no corresponding entry in the Battlegrounds Guide Skin table", (object) item.ItemId);
        break;
      case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
        if (CollectionManager.Get().IsValidBattlegroundsBoardSkinId(BattlegroundsBoardSkinId.FromTrustedValue(item.ItemId)))
        {
          BattlegroundsBoardSkinDbfRecord record7 = GameDbf.BattlegroundsBoardSkin.GetRecord(item.ItemId);
          if (record7 != null)
          {
            item.BGBoardSkin = new BattlegroundsBoardSkinDataModel()
            {
              DisplayName = (string) record7.CollectionShortName,
              DetailsDisplayName = (string) record7.CollectionName,
              Description = (string) record7.Description,
              BorderType = record7.BorderType,
              BoardDbiId = item.ItemId,
              ShopDetailsTexture = PlatformSettings.Screen == ScreenCategory.Phone ? record7.DetailsTexturePhone : record7.DetailsTexture,
              ShopDetailsMovie = PlatformSettings.Screen == ScreenCategory.Phone ? record7.DetailsMoviePhone : record7.DetailsMovie,
              Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record7.Rarity))
            };
            flag = true;
            break;
          }
          failReason = string.Format("Battlegrounds Board Skin Item has unknown board skin id [{0}]", (object) item.ItemId);
          break;
        }
        failReason = string.Format("Battlegrounds Board Skin Item has invalid card DBIid [{0}] with no corresponding entry in the board skin table.", (object) item.ItemId);
        break;
      case RewardItemType.BATTLEGROUNDS_FINISHER:
        if (CollectionManager.Get().IsValidBattlegroundsFinisherId(BattlegroundsFinisherId.FromTrustedValue(item.ItemId)))
        {
          BattlegroundsFinisherDbfRecord record8 = GameDbf.BattlegroundsFinisher.GetRecord(item.ItemId);
          if (record8 != null)
          {
            item.BGFinisher = new BattlegroundsFinisherDataModel()
            {
              DisplayName = (string) record8.CollectionShortName,
              DetailsDisplayName = (string) record8.CollectionName,
              Description = (string) record8.Description,
              FinisherDbiId = item.ItemId,
              CapsuleType = record8.CapsuleType,
              ShopDetailsMovie = record8.DetailsMovie,
              ShopDetailsTexture = record8.DetailsTexture,
              BodyMaterial = record8.MiniBodyMaterial,
              ArtMaterial = record8.MiniArtMaterial,
              Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record8.Rarity))
            };
            flag = true;
            break;
          }
          failReason = string.Format("Battlegrounds Finisher Item has unknown finisher id [{0}]", (object) item.ItemId);
          break;
        }
        failReason = string.Format("Battlegrounds Finisher Item has invalid ID [{0}] with no corresponding entry in the finisher table.", (object) item.ItemId);
        break;
      case RewardItemType.BATTLEGROUNDS_EMOTE:
        if (CollectionManager.Get().IsValidBattlegroundsEmoteId(BattlegroundsEmoteId.FromTrustedValue(item.ItemId)))
        {
          BattlegroundsEmoteDbfRecord record9 = GameDbf.BattlegroundsEmote.GetRecord(item.ItemId);
          if (record9 != null)
          {
            item.BGEmote = new BattlegroundsEmoteDataModel()
            {
              DisplayName = (string) record9.CollectionShortName,
              Description = (string) record9.Description,
              EmoteDbiId = item.ItemId,
              Animation = record9.AnimationPath,
              IsAnimating = record9.IsAnimating,
              BorderType = record9.BorderType,
              XOffset = (float) record9.XOffset,
              ZOffset = (float) record9.ZOffset,
              Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) record9.Rarity))
            };
            flag = true;
            break;
          }
          failReason = string.Format("Battlegrounds Emote Item has unknown emote id [{0}]", (object) item.ItemId);
          break;
        }
        failReason = string.Format("Battlegrounds Emote Item has invalid ID [{0}] with no corresponding entry in the emote table.", (object) item.ItemId);
        break;
      default:
        failReason = string.Format("Reward has unsupported type {0}", (object) item.ItemType);
        break;
    }
    return flag;
  }

  public static CurrencyType RewardItemTypeToCurrencyType(RewardItemType itemType)
  {
    switch (itemType)
    {
      case RewardItemType.DUST:
        return CurrencyType.DUST;
      case RewardItemType.CN_RUNESTONES:
        return CurrencyType.CN_RUNESTONES;
      case RewardItemType.CN_ARCANE_ORBS:
        return CurrencyType.CN_ARCANE_ORBS;
      case RewardItemType.ROW_RUNESTONES:
        return CurrencyType.ROW_RUNESTONES;
      default:
        return CurrencyType.NONE;
    }
  }

  public static int GetRewardItemTypeSortOrder(RewardItemType itemType)
  {
    switch (itemType)
    {
      case RewardItemType.BOOSTER:
        return 800;
      case RewardItemType.DUST:
        return 1100;
      case RewardItemType.HERO_SKIN:
        return 100;
      case RewardItemType.CARD_BACK:
        return 200;
      case RewardItemType.ADVENTURE_WING:
        return 700;
      case RewardItemType.ARENA_TICKET:
        return 500;
      case RewardItemType.RANDOM_CARD:
        return 400;
      case RewardItemType.CN_RUNESTONES:
        return 900;
      case RewardItemType.CN_ARCANE_ORBS:
        return 1000;
      case RewardItemType.ADVENTURE:
        return 600;
      case RewardItemType.CARD:
        return 300;
      case RewardItemType.BATTLEGROUNDS_BONUS:
        return 450;
      case RewardItemType.TAVERN_BRAWL_TICKET:
        return 550;
      case RewardItemType.PROGRESSION_BONUS:
        return 1200;
      case RewardItemType.REWARD_TRACK_XP_BOOST:
        return 50;
      case RewardItemType.MINI_SET:
        return 1300;
      case RewardItemType.CARD_SUBSET:
        return 425;
      case RewardItemType.SELLABLE_DECK:
        return 1400;
      case RewardItemType.MERCENARY_COIN:
        return 1040;
      case RewardItemType.MERCENARY:
        return 350;
      case RewardItemType.MERCENARY_BOOSTER:
        return 850;
      case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
        return 1600;
      case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
        return 1500;
      case RewardItemType.MERCENARY_RANDOM_MERCENARY:
        return 415;
      case RewardItemType.MERCENARY_KNOCKOUT_SPECIFIC:
        return 360;
      case RewardItemType.MERCENARY_KNOCKOUT_RANDOM:
        return 440;
      case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
        return 1466;
      case RewardItemType.BATTLEGROUNDS_FINISHER:
        return 1433;
      case RewardItemType.BATTLEGROUNDS_EMOTE:
        return 1550;
      case RewardItemType.ROW_RUNESTONES:
        return 950;
      default:
        return int.MaxValue;
    }
  }

  public static int CompareItemsForSort(RewardItemDataModel xItem, RewardItemDataModel yItem)
  {
    if (xItem == null && yItem == null)
      return 0;
    if (xItem == null)
      return 1;
    if (yItem == null)
      return -1;
    int itemTypeSortOrder1 = RewardUtils.GetRewardItemTypeSortOrder(xItem.ItemType);
    int itemTypeSortOrder2 = RewardUtils.GetRewardItemTypeSortOrder(yItem.ItemType);
    if (itemTypeSortOrder1 < itemTypeSortOrder2)
      return -1;
    if (itemTypeSortOrder1 > itemTypeSortOrder2)
      return 1;
    if (xItem.Quantity > yItem.Quantity)
      return -1;
    if (xItem.Quantity < yItem.Quantity)
      return 1;
    if (xItem.Booster != null && yItem.Booster != null)
    {
      BoosterDbfRecord record1 = GameDbf.Booster.GetRecord((int) xItem.Booster.Type);
      BoosterDbfRecord record2 = GameDbf.Booster.GetRecord((int) yItem.Booster.Type);
      if (record1 == null && record2 == null)
        return 0;
      if (record1 == null)
        return 1;
      if (record2 == null)
        return -1;
      int num = GameUtils.PackSortingPredicate(record1, record2);
      if (num != 0)
        return num;
    }
    return 0;
  }

  public static void SetNewRewardedDeck(long collectionDeckId) => Options.Get().SetLong(Option.NEWEST_REWARDED_DECK_ID, collectionDeckId);

  public static bool HasNewRewardedDeck(out long collectionDeckId)
  {
    collectionDeckId = Options.Get().GetLong(Option.NEWEST_REWARDED_DECK_ID);
    return collectionDeckId != 0L;
  }

  public static void MarkNewestRewardedDeckAsSeen() => RewardUtils.SetNewRewardedDeck(0L);

  public static int CompareOwnedItemsForSort(RewardItemDataModel xItem, RewardItemDataModel yItem)
  {
    if (xItem != null && xItem.Card != null && yItem != null && yItem.Card != null)
    {
      if (xItem.Card.Owned && !yItem.Card.Owned)
        return 1;
      if (!xItem.Card.Owned && yItem.Card.Owned)
        return -1;
    }
    return RewardUtils.CompareItemsForSort(xItem, yItem);
  }

  public static int GetSortOrderFromItems(DataModelList<RewardItemDataModel> items)
  {
    foreach (RewardItemDataModel rewardItemDataModel in items)
    {
      int sortOrder;
      if (RewardUtils.AttemptToGetItemSortOrder(rewardItemDataModel, out sortOrder))
        return sortOrder;
    }
    return 0;
  }

  public static bool AttemptToGetItemSortOrder(RewardItemDataModel item, out int sortOrder)
  {
    if (item != null && item.ItemType == RewardItemType.SELLABLE_DECK && RewardUtils.IsValidSellableDeckRecordId(item.ItemId))
    {
      sortOrder = RewardUtils.GetSortOrderForSellableDeck(item.ItemId);
      return true;
    }
    sortOrder = 0;
    return false;
  }

  public static bool IsValidSellableDeckRecordId(int sellableDeckRecordId) => GameDbf.SellableDeck.GetRecord(sellableDeckRecordId)?.DeckTemplateRecord != null;

  public static int GetSortOrderForSellableDeck(int sellableDeckRecordId) => GameDbf.SellableDeck.GetRecord(sellableDeckRecordId).DeckTemplateRecord.SortOrder;

  public static void GetMercenaryName(
    string cardId,
    out string localizedName,
    out string localizedShortName)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    if (entityDef == null)
    {
      localizedName = (string) null;
      localizedShortName = (string) null;
    }
    else
    {
      string name = entityDef.GetName();
      string shortName = entityDef.GetShortName();
      if (string.IsNullOrEmpty(name))
      {
        localizedName = (string) null;
        localizedShortName = (string) null;
      }
      else
      {
        localizedName = GameStrings.FormatLocalizedString(name);
        localizedShortName = !string.IsNullOrWhiteSpace(shortName) ? GameStrings.FormatLocalizedString(shortName) : localizedName;
      }
    }
  }

  public static string GetMercenaryRarityText(TAG_RARITY rarity)
  {
    string mercenaryRarityText;
    switch (rarity)
    {
      case TAG_RARITY.RARE:
        mercenaryRarityText = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_TITLE_RARE");
        break;
      case TAG_RARITY.EPIC:
        mercenaryRarityText = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_TITLE_EPIC");
        break;
      case TAG_RARITY.LEGENDARY:
        mercenaryRarityText = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_TITLE_LEGENDARY");
        break;
      default:
        mercenaryRarityText = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_TITLE_COMMON");
        break;
    }
    return mercenaryRarityText;
  }

  public static string GetMercenaryKnockoutCoinsText(
    TAG_PREMIUM premium,
    string mercName,
    string mercShortName)
  {
    string knockoutCoinsText;
    if (premium == TAG_PREMIUM.NORMAL || !GameStrings.HasPremiumText(premium))
      knockoutCoinsText = GameStrings.Format("GLUE_LETTUCE_REWARD_KNOCKOUT_COINS_DESC", (object) mercName, (object) mercShortName);
    else
      knockoutCoinsText = GameStrings.Format("GLUE_LETTUCE_REWARD_KNOCKOUT_COINS_DESC_PREMIUM", (object) GameStrings.GetPremiumText(premium), (object) mercName, (object) mercShortName);
    return knockoutCoinsText;
  }

  public static RewardItemRewardData CreateMercenaryRewardData(
    int mercId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    return new RewardItemRewardData(RewardUtils.CreateMercenaryRewardItemDataModel(mercId, artVariationId, premium), false);
  }

  public static RewardData CreateMercenaryOrKnockoutRewardData(
    int mercId,
    int artVariationId,
    TAG_PREMIUM premium,
    int currencyAmount)
  {
    RewardItemDataModel rewardItemDataModel = RewardUtils.CreateMercenaryRewardItemDataModel(mercId, artVariationId, premium);
    string localizedName;
    string localizedShortName;
    RewardUtils.GetMercenaryName(rewardItemDataModel.Mercenary.Card.CardId, out localizedName, out localizedShortName);
    if (localizedName == null)
      return (RewardData) null;
    RewardData knockoutRewardData;
    if (currencyAmount > 0)
    {
      RewardItemDataModel dataModel = RewardUtils.CreateMercenaryCoinsRewardData(mercId, currencyAmount, true, false).DataModel;
      knockoutRewardData = (RewardData) new MercenariesKnockoutRewardData(rewardItemDataModel, dataModel);
      knockoutRewardData.NameOverride = RewardUtils.GetMercenaryRarityText(rewardItemDataModel.Mercenary.MercenaryRarity);
      if (localizedName != null && localizedShortName != null)
        knockoutRewardData.DescriptionOverride = RewardUtils.GetMercenaryKnockoutCoinsText(premium, localizedName, localizedShortName);
    }
    else
    {
      knockoutRewardData = (RewardData) new RewardItemRewardData(rewardItemDataModel, true, Reward.Type.MERCENARY_MERCENARY);
      if (!string.IsNullOrEmpty(localizedName))
        knockoutRewardData.NameOverride = localizedName;
      if (!string.IsNullOrEmpty(localizedShortName))
        knockoutRewardData.DescriptionOverride = GameStrings.Format("GLUE_LETTUCE_REWARD_MERCENARY_DESC", (object) localizedShortName);
    }
    return knockoutRewardData;
  }

  public static RewardItemDataModel CreateMercenaryRewardItemDataModel(
    int mercId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(mercId, artVariationId, premium);
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercId, ReportError: false);
    RewardItemDataModel rewardItemDataModel = new RewardItemDataModel()
    {
      ItemType = RewardItemType.MERCENARY,
      Quantity = 1,
      Mercenary = mercenaryDataModel,
      IsMercenaryPortrait = RewardUtils.IsMercenaryRewardPortrait(mercenaryDataModel) && mercenary != null && mercenary.m_owned
    };
    rewardItemDataModel.Mercenary.Owned = true;
    rewardItemDataModel.Mercenary.HideXp = true;
    rewardItemDataModel.Mercenary.HideWatermark = false;
    rewardItemDataModel.Mercenary.Label = string.Empty;
    return rewardItemDataModel;
  }

  private static string GetNameFromCardReward(RewardItemDataModel rewardItemDataModel)
  {
    switch (rewardItemDataModel.Card?.Name)
    {
      case null:
        switch (rewardItemDataModel.Card?.CardId)
        {
          case null:
            Log.Dbf.PrintWarning("GetNameFromCardReward could not find a card name for this RewardItemDataModel.");
            return (string) null;
          default:
            EntityDef entityDef = DefLoader.Get().GetEntityDef(rewardItemDataModel.Card.CardId);
            if (entityDef != null)
              return entityDef.GetName();
            goto case null;
        }
      default:
        return rewardItemDataModel.Card.Name;
    }
  }

  public static string GetName(RewardItemDataModel rewardItemDataModel)
  {
    if (rewardItemDataModel == null)
    {
      Log.Gameplay.PrintWarning("GetNameFromCardReward tried to get name from a null RewardItemDataModel.");
      return (string) null;
    }
    switch (rewardItemDataModel.ItemType)
    {
      case RewardItemType.BOOSTER:
      case RewardItemType.MERCENARY_BOOSTER:
        return rewardItemDataModel.Booster?.BoosterName;
      case RewardItemType.HERO_SKIN:
      case RewardItemType.CARD:
      case RewardItemType.BATTLEGROUNDS_HERO_SKIN:
      case RewardItemType.BATTLEGROUNDS_GUIDE_SKIN:
        return RewardUtils.GetNameFromCardReward(rewardItemDataModel);
      case RewardItemType.MERCENARY_COIN:
        return rewardItemDataModel.MercenaryCoin?.MercenaryName;
      case RewardItemType.MERCENARY:
        return rewardItemDataModel.Mercenary?.MercenaryName;
      case RewardItemType.MERCENARY_EQUIPMENT:
      case RewardItemType.MERCENARY_EQUIPMENT_ICON:
        return rewardItemDataModel.MercenaryEquip?.AbilityName;
      case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
        return rewardItemDataModel.BGBoardSkin?.DetailsDisplayName;
      case RewardItemType.BATTLEGROUNDS_FINISHER:
        return rewardItemDataModel.BGFinisher?.DetailsDisplayName;
      case RewardItemType.BATTLEGROUNDS_EMOTE:
        return rewardItemDataModel.BGEmote?.DisplayName;
      default:
        throw new NotImplementedException();
    }
  }

  public static RewardItemDataModel CreateKnockoutSpecificMercenaryRewardItemDataModel(
    int mercId)
  {
    int artVariationId = 0;
    TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
    LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(mercId, artVariationId, premium);
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercId, ReportError: false);
    RewardItemDataModel rewardItemDataModel = new RewardItemDataModel()
    {
      ItemType = RewardItemType.MERCENARY_KNOCKOUT_SPECIFIC,
      Quantity = 1,
      Mercenary = mercenaryDataModel,
      IsMercenaryPortrait = RewardUtils.IsMercenaryRewardPortrait(mercenaryDataModel) && mercenary != null && mercenary.m_owned
    };
    rewardItemDataModel.Mercenary.Owned = true;
    rewardItemDataModel.Mercenary.HideXp = true;
    rewardItemDataModel.Mercenary.HideWatermark = false;
    rewardItemDataModel.Mercenary.Label = string.Empty;
    return rewardItemDataModel;
  }

  public static RewardItemRewardData CreateMercenaryCoinsRewardData(
    int mercId,
    int quantity,
    bool glowActive,
    bool nameActive)
  {
    if (mercId == 0)
      return new RewardItemRewardData(new RewardItemDataModel()
      {
        ItemType = RewardItemType.MERCENARY_COIN,
        Quantity = 1,
        MercenaryCoin = new LettuceMercenaryCoinDataModel()
        {
          Quantity = quantity,
          GlowActive = glowActive,
          IsRandom = true,
          NameActive = nameActive
        }
      }, true, Reward.Type.MERCENARY_COIN);
    string idFromMercenaryId = GameUtils.GetCardIdFromMercenaryId(mercId);
    EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromMercenaryId);
    string shortName = entityDef.GetShortName();
    string str = string.IsNullOrEmpty(shortName) ? entityDef.GetName() : shortName;
    return new RewardItemRewardData(new RewardItemDataModel()
    {
      ItemType = RewardItemType.MERCENARY_COIN,
      Quantity = 1,
      MercenaryCoin = new LettuceMercenaryCoinDataModel()
      {
        MercenaryId = mercId,
        MercenaryName = str,
        Quantity = quantity,
        GlowActive = glowActive,
        NameActive = nameActive
      }
    }, true, Reward.Type.MERCENARY_COIN);
  }

  public static RewardItemRewardData CreateMercenaryRenownRewardData(int amount) => new RewardItemRewardData(new RewardItemDataModel()
  {
    ItemType = RewardItemType.MERCENARY_RENOWN,
    Quantity = 1,
    Currency = new PriceDataModel()
    {
      Currency = CurrencyType.RENOWN,
      Amount = (float) amount
    }
  }, true, Reward.Type.MERCENARY_RENOWN);

  private static void TryGetToastTextFromFixedRewardMap(
    int rewardMapId,
    out string toastName,
    out string toastDesc,
    out bool shouldSkipToast)
  {
    toastDesc = (string) null;
    toastName = (string) null;
    FixedRewardMapDbfRecord record = GameDbf.FixedRewardMap.GetRecord(rewardMapId);
    if (record != null && !record.UseQuestToast)
    {
      shouldSkipToast = true;
    }
    else
    {
      shouldSkipToast = false;
      if (record == null)
        return;
      toastName = (string) record.ToastName;
      toastDesc = (string) record.ToastDescription;
    }
  }

  private class RewardDisplayCallbackData
  {
    public List<RewardData> rewardsToDisplay;
    public Reward currentReward;
    public int rewardIndex;
    public Action doneCallback;
  }

  public class RewardItemComparer : IComparer<RewardItemDataModel>
  {
    public int Compare(RewardItemDataModel first, RewardItemDataModel second) => RewardUtils.CompareItemsForSort(first, second);
  }

  public class RewardOwnedItemComparer : IComparer<RewardItemDataModel>
  {
    public int Compare(RewardItemDataModel first, RewardItemDataModel second) => RewardUtils.CompareOwnedItemsForSort(first, second);
  }
}
