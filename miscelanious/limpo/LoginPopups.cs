using System;
using System.Collections.Generic;

public class LoginPopups : IDisposable
{
  private List<LoginPopupSequenceDbfRecord> m_loginPopupSequenceDbfRecords = new List<LoginPopupSequenceDbfRecord>();
  private List<long> m_saveKeyValues = new List<long>();
  private List<LoginPopupSequencePopupDbfRecord> m_popupRecords = new List<LoginPopupSequencePopupDbfRecord>();

  public void Dispose()
  {
    this.m_loginPopupSequenceDbfRecords = (List<LoginPopupSequenceDbfRecord>) null;
    this.m_saveKeyValues = (List<long>) null;
    this.m_popupRecords = (List<LoginPopupSequencePopupDbfRecord>) null;
  }

  public bool ShowLoginPopupSequence(
    bool suppressRewardPopupsForNewPlayer,
    bool shouldDisableNotificationOnLogin,
    CardPopups m_cardPopups)
  {
    if (suppressRewardPopupsForNewPlayer || !UserAttentionManager.CanShowAttentionGrabber(nameof (ShowLoginPopupSequence)))
      return false;
    this.m_loginPopupSequenceDbfRecords.Clear();
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    List<LoginPopupSequenceDbfRecord> records1 = GameDbf.LoginPopupSequence.GetRecords();
    int index1 = 0;
    for (int count = records1.Count; index1 < count; ++index1)
    {
      LoginPopupSequenceDbfRecord sequenceDbfRecord = records1[index1];
      if (specialEventManager.IsEventActive(sequenceDbfRecord.EventTiming, false))
        this.m_loginPopupSequenceDbfRecords.Add(sequenceDbfRecord);
    }
    if (this.m_loginPopupSequenceDbfRecords.Count == 0)
      return false;
    if (shouldDisableNotificationOnLogin)
    {
      List<long> longList = new List<long>();
      foreach (LoginPopupSequenceDbfRecord sequenceDbfRecord in this.m_loginPopupSequenceDbfRecords)
        longList.Add((long) sequenceDbfRecord.ID);
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LOGIN_POPUP_SEQUENCE_SEEN_POPUPS, longList.ToArray()));
      return false;
    }
    bool flag1 = false;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LOGIN_POPUP_SEQUENCE_SEEN_POPUPS, this.m_saveKeyValues);
    foreach (DbfRecord sequenceDbfRecord in this.m_loginPopupSequenceDbfRecords)
    {
      int id = sequenceDbfRecord.ID;
      if (!this.m_saveKeyValues.Contains((long) id))
      {
        this.m_popupRecords.Clear();
        List<LoginPopupSequencePopupDbfRecord> records2 = GameDbf.LoginPopupSequencePopup.GetRecords();
        int index2 = 0;
        for (int count = records2.Count; index2 < count; ++index2)
        {
          LoginPopupSequencePopupDbfRecord sequencePopupDbfRecord = records2[index2];
          if (sequencePopupDbfRecord.LoginPopupSequenceId == id)
            this.m_popupRecords.Add(sequencePopupDbfRecord);
        }
        for (int index3 = 0; index3 < this.m_popupRecords.Count; ++index3)
        {
          LoginPopupSequencePopupDbfRecord popupRecord = this.m_popupRecords[index3];
          Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType popupType = popupRecord.PopupType;
          bool flag2 = true;
          if (popupRecord.RequiresWildUnlocked && !CollectionManager.Get().ShouldAccountSeeStandardWild())
            flag2 = false;
          else if (popupRecord.SuppressForReturningPlayer && ReturningPlayerMgr.Get().IsInReturningPlayerMode)
            flag2 = false;
          bool flag3 = index3 == this.m_popupRecords.Count - 1;
          DialogBase.HideCallback callbackOnHide = (DialogBase.HideCallback) null;
          if (flag3)
          {
            if (flag2)
            {
              callbackOnHide = this.CreateCallback(id);
            }
            else
            {
              this.OnPopupSequenceDismissed(id);
              break;
            }
          }
          else if (!flag2)
            continue;
          switch (popupType)
          {
            case Assets.LoginPopupSequencePopup.LoginPopupSequencePopupType.FEATURED_CARDS:
              if (m_cardPopups.ShowFeaturedCards(DbfShared.GetEventMap().ConvertStringToSpecialEvent(popupRecord.FeaturedCardsEvent), (string) popupRecord.HeaderText, callbackOnHide))
              {
                flag1 = true;
                continue;
              }
              if (flag3)
              {
                this.OnPopupSequenceDismissed(id);
                continue;
              }
              continue;
            default:
              LoginPopupSequencePopup.Info info = new LoginPopupSequencePopup.Info()
              {
                m_headerText = (string) popupRecord.HeaderText,
                m_bodyText = (string) popupRecord.BodyText,
                m_buttonText = (string) popupRecord.ButtonText,
                m_backgroundMaterialReference = new AssetReference(popupRecord.BackgroundMaterial),
                m_callbackOnHide = callbackOnHide,
                m_prefabAssetReference = popupRecord.PrefabOverride
              };
              if (popupRecord.CardId != 0)
              {
                TAG_PREMIUM cardPremium = (TAG_PREMIUM) popupRecord.CardPremium;
                info.m_card = CollectionManager.Get().GetCard(GameUtils.TranslateDbIdToCardId(popupRecord.CardId), cardPremium);
              }
              DialogManager.Get().ShowLoginPopupSequenceBasicPopup(UserAttentionBlocker.NONE, info);
              flag1 = true;
              continue;
          }
        }
        this.m_popupRecords.Clear();
      }
    }
    return flag1;
  }

  private DialogBase.HideCallback CreateCallback(int recordId) => (DialogBase.HideCallback) ((dialog, userData) => this.OnPopupSequenceDismissed(recordId));

  private void OnPopupSequenceDismissed(int popupSequenceId)
  {
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LOGIN_POPUP_SEQUENCE_SEEN_POPUPS, out values);
    if (values == null)
      values = new List<long>();
    values.Add((long) popupSequenceId);
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LOGIN_POPUP_SEQUENCE_SEEN_POPUPS, values.ToArray()));
  }
}
