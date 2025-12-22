using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using System;
using System.Collections.Generic;

public class RedundantNDERerollPopups : IDisposable
{
  private Action OnPopupShown;
  private Action OnPopupClosed;
  private Action<bool> SetIsShowing;
  private static readonly AssetReference POPUP_PREFAB = new AssetReference("RedundantNDEPopup.prefab:f547c99ed5cef4b419d9ba11c141e89f");
  private Widget m_currentPopup;
  private RedundantNDEPopup m_currentPopupComponent;
  private bool m_isResultDisplaying;
  private Queue<RedundantNDERerollPopups.RerollPopupInfo> m_RerollNotices = new Queue<RedundantNDERerollPopups.RerollPopupInfo>();
  private Queue<NetCache.ProfileNoticeRedundantNDERerollResult> m_rerollResults = new Queue<NetCache.ProfileNoticeRedundantNDERerollResult>();
  private NetCache.ProfileNoticeRedundantNDERerollResult m_currentRerollResults;
  private RedundantNDERerollPopups.RerollPopupInfo m_currentPopupInfo;
  private RewardPresenter m_rewardPresenter = new RewardPresenter();
  private HashSet<int> m_queuedRewardPresenterRewardAssetIds = new HashSet<int>();

  public bool SuppressNDEPopups { get; set; }

  public RedundantNDERerollPopups(
    Action<bool> setIsShowing,
    Action onPopupShown,
    Action onPopupClosed)
  {
    this.SetIsShowing = setIsShowing;
    this.OnPopupShown = onPopupShown;
    this.OnPopupClosed = onPopupClosed;
    this.SuppressNDEPopups = false;
    this.StartupRegistration();
  }

  private void StartupRegistration() => NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));

  public void Dispose() => NetCache.Get().RemoveNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));

  public void OnRewardPresenterScrollQueued(int rewardId) => this.m_queuedRewardPresenterRewardAssetIds.Add(rewardId);

  public bool ShowRerollPopup()
  {
    if (this.m_isResultDisplaying || (UnityEngine.Object) this.m_currentPopup != (UnityEngine.Object) null)
      return true;
    if (this.SuppressNDEPopups)
      return false;
    while (this.m_rerollResults.Count > 0 && !this.m_isResultDisplaying)
    {
      this.m_currentRerollResults = this.m_rerollResults.Dequeue();
      CardDbfRecord record = GameDbf.Card.GetRecord(this.m_currentRerollResults.GrantedCardID);
      if (record != null)
      {
        this.m_rewardPresenter.EnqueueReward(new RewardScrollDataModel()
        {
          DisplayName = GameStrings.Get("GLOBAL_REDUNDANT_NDE_REROLL_RESULT_HEADER"),
          Description = (string) record.Name,
          RewardList = new RewardListDataModel()
          {
            Items = new DataModelList<RewardItemDataModel>()
            {
              new RewardItemDataModel()
              {
                ItemType = RewardItemType.CARD,
                ItemId = this.m_currentRerollResults.GrantedCardID,
                Quantity = 1,
                Card = new CardDataModel()
                {
                  CardId = record.NoteMiniGuid,
                  Premium = this.m_currentRerollResults.Premium
                }
              }
            }
          }
        }, (Action) (() => Network.Get().AckNotice(this.m_currentRerollResults.NoticeID)));
        this.m_isResultDisplaying = true;
        this.m_rewardPresenter.ShowNextReward(new Action(this.DismissResults));
      }
    }
    while (this.m_RerollNotices.Count > 0)
    {
      RedundantNDERerollPopups.RerollPopupInfo rerollPopupInfo = this.m_RerollNotices.Peek();
      if (rerollPopupInfo.m_waitForReward && !this.m_queuedRewardPresenterRewardAssetIds.Contains((int) rerollPopupInfo.m_notices[0].OriginData))
        return false;
      this.m_currentPopupInfo = this.m_RerollNotices.Dequeue();
      if (this.m_currentPopupInfo == null)
        return false;
      int signature = 0;
      int normal;
      int golden;
      CollectionManager.Get().GetOwnedCardCount(this.m_currentPopupInfo.m_notices[0].CardID, out normal, out golden, out signature, out int _);
      CollectibleCard card1 = CollectionManager.Get().GetCard(this.m_currentPopupInfo.m_notices[0].CardID, TAG_PREMIUM.GOLDEN);
      CollectibleCard card2 = CollectionManager.Get().GetCard(this.m_currentPopupInfo.m_notices[0].CardID, TAG_PREMIUM.NORMAL);
      int num = Math.Min((card1.IsCraftable ? golden : 0) + (card2.IsCraftable ? normal : 0), this.m_currentPopupInfo.m_notices.Count);
      if (num == 0)
      {
        foreach (NetCache.ProfileNoticeRedundantNDEReroll notice in this.m_currentPopupInfo.m_notices)
          Network.Get().AckNotice(notice.NoticeID);
      }
      else
      {
        Action onPopupShown = this.OnPopupShown;
        if (onPopupShown != null)
          onPopupShown();
        Action<bool> setIsShowing = this.SetIsShowing;
        if (setIsShowing != null)
          setIsShowing(true);
        this.m_currentPopup = (Widget) WidgetInstance.Create((string) RedundantNDERerollPopups.POPUP_PREFAB);
        this.m_currentPopup.RegisterReadyListener((Action<object>) (_ =>
        {
          this.m_currentPopupComponent = this.m_currentPopup.GetComponentInChildren<RedundantNDEPopup>();
          if (!((UnityEngine.Object) this.m_currentPopupComponent != (UnityEngine.Object) null))
            return;
          this.m_currentPopupComponent.Show();
          this.m_currentPopupComponent.RerollSelected += new Action(this.OnRerollSelected);
          this.m_currentPopupComponent.RefuseSelected += new Action(this.OnRefuseSelected);
        }), (object) null, true);
        TAG_PREMIUM tag1 = golden <= 0 || !card1.IsCraftable ? TAG_PREMIUM.NORMAL : TAG_PREMIUM.GOLDEN;
        TAG_PREMIUM tag2 = golden <= 1 || !card1.IsCraftable ? TAG_PREMIUM.NORMAL : TAG_PREMIUM.GOLDEN;
        CardDataModel cardDataModel1 = new CardDataModel()
        {
          CardId = this.m_currentPopupInfo.m_notices[0].CardID,
          Premium = tag1
        };
        CardDataModel cardDataModel2 = new CardDataModel()
        {
          CardId = this.m_currentPopupInfo.m_notices[0].CardID,
          Premium = tag2
        };
        RandomCardDataModel randomCardDataModel1 = new RandomCardDataModel()
        {
          Rarity = card2.Rarity,
          Premium = tag1
        };
        RandomCardDataModel randomCardDataModel2 = new RandomCardDataModel()
        {
          Rarity = card2.Rarity,
          Premium = tag2
        };
        NDERerollPopupDataModel rerollPopupDataModel = new NDERerollPopupDataModel();
        rerollPopupDataModel.RerollCards.Add(cardDataModel1);
        rerollPopupDataModel.RerollCards.Add(cardDataModel2);
        rerollPopupDataModel.RandomCards.Add(randomCardDataModel1);
        rerollPopupDataModel.RandomCards.Add(randomCardDataModel2);
        rerollPopupDataModel.Quantity = num;
        GameStrings.PluralNumber[] pluralNumbers = GameStrings.MakePlurals(num);
        if (tag1 == tag2 || num == 1)
        {
          rerollPopupDataModel.HeaderText = GameStrings.FormatPlurals("GLOBAL_REDUNDANT_NDE_TITLE", pluralNumbers, (object) GameStrings.GetPremiumText(tag1));
          rerollPopupDataModel.BodyText = GameStrings.Format("GLOBAL_REDUNDANT_NDE_BODY", (object) GameStrings.GetPremiumText(this.m_currentPopupInfo.m_notices[0].Premium), (object) GameStrings.GetPremiumText(tag1), (object) GameStrings.GetCardSetName(card2.Set), (object) GameStrings.GetRarityText(card2.Rarity));
        }
        else
        {
          rerollPopupDataModel.HeaderText = GameStrings.Format("GLOBAL_REDUNDANT_NDE_TITLE_MULTIPLE_PREMIUMS", (object) GameStrings.GetPremiumText(tag1), (object) GameStrings.GetPremiumText(tag2));
          rerollPopupDataModel.BodyText = GameStrings.Format("GLOBAL_REDUNDANT_NDE_BODY_MULTIPLE_PREMIUMS", (object) GameStrings.GetPremiumText(this.m_currentPopupInfo.m_notices[0].Premium), (object) GameStrings.GetPremiumText(tag1), (object) GameStrings.GetPremiumText(tag2), (object) GameStrings.GetCardSetName(card2.Set), (object) GameStrings.GetRarityText(card2.Rarity));
        }
        this.m_currentPopup.BindDataModel((IDataModel) rerollPopupDataModel);
        return true;
      }
    }
    return false;
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList) => newNotices.ForEach((Action<NetCache.ProfileNotice>) (notice =>
  {
    if (notice.Origin == NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_NDE_REDUNDANT_REROLL && notice.Type == NetCache.ProfileNotice.NoticeType.REDUNDANT_NDE_REROLL)
    {
      bool flag = false;
      NetCache.ProfileNoticeRedundantNDEReroll redundantNdeReroll = notice as NetCache.ProfileNoticeRedundantNDEReroll;
      foreach (RedundantNDERerollPopups.RerollPopupInfo rerollNotice in this.m_RerollNotices)
      {
        NetCache.ProfileNoticeRedundantNDEReroll notice1 = rerollNotice.m_notices[0];
        if (notice1 != null && notice1.CardID == redundantNdeReroll.CardID && notice1.Premium == redundantNdeReroll.Premium)
        {
          rerollNotice.m_notices.Add(redundantNdeReroll);
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this.m_RerollNotices.Enqueue(new RedundantNDERerollPopups.RerollPopupInfo()
      {
        m_notices = {
          redundantNdeReroll
        },
        m_waitForReward = !isInitialNoticeList && redundantNdeReroll.OriginData != 0L
      });
    }
    else
    {
      if (notice.Origin != NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_NDE_REDUNDANT_REROLL || notice.Type != NetCache.ProfileNotice.NoticeType.REDUNDANT_NDE_REROLL_RESULT)
        return;
      this.m_rerollResults.Enqueue(notice as NetCache.ProfileNoticeRedundantNDERerollResult);
    }
  }));

  private void OnRerollSelected()
  {
    List<long> longList = new List<long>(this.m_currentPopupInfo.m_notices.Count);
    this.PopulateNoticeIDList(longList);
    Network.Get().RespondToRedundantNDEReroll(longList, true);
    this.DismissPopup();
  }

  private void OnRefuseSelected()
  {
    List<long> longList = new List<long>(this.m_currentPopupInfo.m_notices.Count);
    this.PopulateNoticeIDList(longList);
    Network.Get().RespondToRedundantNDEReroll(longList, false);
    this.DismissPopup();
  }

  private void PopulateNoticeIDList(List<long> noticeIDs)
  {
    foreach (NetCache.ProfileNoticeRedundantNDEReroll notice in this.m_currentPopupInfo.m_notices)
      noticeIDs.Add(notice.NoticeID);
  }

  private void DismissPopup()
  {
    this.m_currentPopupComponent.RerollSelected -= new Action(this.OnRerollSelected);
    this.m_currentPopupComponent.RefuseSelected -= new Action(this.OnRefuseSelected);
    this.m_currentPopupComponent.OnDismissAnimationComplete += new Action(this.OnDismissAnimationComplete);
    this.m_currentPopupComponent.StartCoroutine(this.m_currentPopupComponent.Hide());
  }

  private void OnDismissAnimationComplete()
  {
    this.m_currentPopup.Hide();
    this.m_currentPopup = (Widget) null;
    this.m_currentPopupComponent = (RedundantNDEPopup) null;
    this.m_currentPopupInfo = (RedundantNDERerollPopups.RerollPopupInfo) null;
    Action onPopupClosed = this.OnPopupClosed;
    if (onPopupClosed != null)
      onPopupClosed();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing == null)
      return;
    setIsShowing(false);
  }

  private void DismissResults()
  {
    this.m_currentRerollResults = (NetCache.ProfileNoticeRedundantNDERerollResult) null;
    this.m_isResultDisplaying = false;
  }

  private class RerollPopupInfo
  {
    public List<NetCache.ProfileNoticeRedundantNDEReroll> m_notices = new List<NetCache.ProfileNoticeRedundantNDEReroll>();
    public bool m_waitForReward;
  }
}
