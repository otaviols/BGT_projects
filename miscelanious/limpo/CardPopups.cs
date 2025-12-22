using System;
using System.Collections.Generic;
using System.Linq;

public class CardPopups : IDisposable
{
  private bool m_hasBeenInitialized;
  private Dictionary<(SpecialEventType, bool), List<CardChangeDbfRecord>> m_cardChangesPerEventId = new Dictionary<(SpecialEventType, bool), List<CardChangeDbfRecord>>();
  private List<(SpecialEventType, bool)> m_cardChangeEventsToDisplayQueue;

  public void Dispose()
  {
  }

  public bool ShowChangedCards(
    bool shouldDisableNotificationOnLogin = false,
    UserAttentionBlocker ignoredAttentionBlockers = UserAttentionBlocker.NONE)
  {
    if (!this.m_hasBeenInitialized)
    {
      this.InitializeCardChangeEvents(shouldDisableNotificationOnLogin, ignoredAttentionBlockers);
      this.m_hasBeenInitialized = true;
    }
    return this.m_cardChangeEventsToDisplayQueue != null && this.m_cardChangeEventsToDisplayQueue.Count != 0 && UserAttentionManager.CanShowAttentionGrabber(ignoredAttentionBlockers, nameof (ShowChangedCards)) && this.ShowPopup(shouldDisableNotificationOnLogin, ignoredAttentionBlockers);
  }

  public bool ShowFeaturedCards(
    SpecialEventType featuredCardsEvent,
    string headerText,
    DialogBase.HideCallback callbackOnHide = null,
    UserAttentionBlocker ignoredAttentionBlockers = UserAttentionBlocker.NONE)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(ignoredAttentionBlockers, nameof (ShowFeaturedCards)))
      return false;
    MultiPagePopup.Info info = new MultiPagePopup.Info()
    {
      m_callbackOnHide = callbackOnHide,
      m_blurWhenShown = true
    };
    List<int> list = GameDbf.GetIndex().GetCardsWithFeaturedCardsEvent().Where<CardDbfRecord>((Func<CardDbfRecord, bool>) (r => r.FeaturedCardsEvent == featuredCardsEvent)).Select<CardDbfRecord, int>((Func<CardDbfRecord, int>) (r => r.ID)).ToList<int>();
    MultiPagePopup.PageInfo pageInfo = new MultiPagePopup.PageInfo()
    {
      m_pageType = MultiPagePopup.PageType.CARD_LIST,
      m_cards = list,
      m_headerText = headerText
    };
    info.m_pages.Add(pageInfo);
    DialogManager.Get().ShowMultiPagePopup(UserAttentionBlocker.NONE, info);
    return true;
  }

  public void InitializeCardChangeEvents(
    bool shouldDisableNotificationOnLogin = false,
    UserAttentionBlocker ignoredAttentionBlockers = UserAttentionBlocker.NONE)
  {
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LIST_OF_SEEN_CARD_CHANGES, out values);
    if (values == null)
      values = new List<long>();
    HashSet<long> longSet = new HashSet<long>((IEnumerable<long>) values);
    bool flag = ReturningPlayerMgr.Get().IsInReturningPlayerMode | shouldDisableNotificationOnLogin || !GameUtils.IsTraditionalTutorialComplete();
    foreach (CardChangeDbfRecord record in GameDbf.CardChange.GetRecords())
    {
      long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(record.Event);
      if (!longSet.Contains(eventIdFromEventName) && SpecialEventManager.Get().IsEventActive(record.Event, false))
      {
        if (flag)
        {
          this.MarkCardChangeEventAsSeen(record.Event);
        }
        else
        {
          (SpecialEventType, bool) key = (record.Event, record.ChangeType == Assets.CardChange.ChangeType.ADDITION);
          List<CardChangeDbfRecord> cardChangeDbfRecordList;
          if (this.m_cardChangesPerEventId.TryGetValue(key, out cardChangeDbfRecordList))
            cardChangeDbfRecordList.Add(record);
          else
            this.m_cardChangesPerEventId[key] = new List<CardChangeDbfRecord>()
            {
              record
            };
        }
      }
    }
    if (flag)
      return;
    Dictionary<TAG_CLASS, int> orderedClasses = new Dictionary<TAG_CLASS, int>();
    for (int index = 0; index < GameUtils.ORDERED_HERO_CLASSES.Length; ++index)
      orderedClasses.Add(GameUtils.ORDERED_HERO_CLASSES[index], index);
    orderedClasses.Add(TAG_CLASS.NEUTRAL, GameUtils.ORDERED_HERO_CLASSES.Length + 1);
    foreach ((SpecialEventType, bool) key in this.m_cardChangesPerEventId.Keys.ToList<(SpecialEventType, bool)>())
    {
      List<CardChangeDbfRecord> source;
      if (this.m_cardChangesPerEventId.TryGetValue(key, out source))
        this.m_cardChangesPerEventId[key] = source.OrderBy<CardChangeDbfRecord, int>((Func<CardChangeDbfRecord, int>) (r => r.SortOrder)).ThenBy<CardChangeDbfRecord, int>((Func<CardChangeDbfRecord, int>) (r => orderedClasses[DefLoader.Get().GetEntityDef(r.CardId).GetClass()])).ThenByDescending<CardChangeDbfRecord, TAG_RARITY>((Func<CardChangeDbfRecord, TAG_RARITY>) (r => DefLoader.Get().GetEntityDef(r.CardId).GetRarity())).ToList<CardChangeDbfRecord>();
    }
    this.m_cardChangeEventsToDisplayQueue = this.m_cardChangesPerEventId.Keys.OrderBy<(SpecialEventType, bool), DateTime?>((Func<(SpecialEventType, bool), DateTime?>) (k => SpecialEventManager.Get().GetEventStartTimeUtc(k.Item1))).ToList<(SpecialEventType, bool)>();
  }

  public bool MarkCardChangeEventAsSeen(SpecialEventType specialEventType)
  {
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LIST_OF_SEEN_CARD_CHANGES, out values);
    if (values == null)
      values = new List<long>();
    if (values.Count == 100)
      values.RemoveAt(0);
    long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(specialEventType);
    values.Add(eventIdFromEventName);
    return GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LIST_OF_SEEN_CARD_CHANGES, values.ToArray()));
  }

  private bool ShowPopup(
    bool shouldDisableNotificationOnLogin = false,
    UserAttentionBlocker ignoredAttentionBlockers = UserAttentionBlocker.NONE)
  {
    (SpecialEventType, bool) eventToShow = this.m_cardChangeEventsToDisplayQueue[0];
    this.m_cardChangeEventsToDisplayQueue.RemoveAt(0);
    List<int> list = this.m_cardChangesPerEventId[eventToShow].OrderBy<CardChangeDbfRecord, int>((Func<CardChangeDbfRecord, int>) (r => r.SortOrder)).Select<CardChangeDbfRecord, int>((Func<CardChangeDbfRecord, int>) (r => r.CardId)).Distinct<int>().ToList<int>();
    if (eventToShow.Item2)
    {
      CardListPopup.Info info1 = new CardListPopup.Info();
      CardListPopup.Info info2 = info1;
      string str;
      if (list.Count != 1)
        str = GameStrings.Format("GLUE_CARDS_ADDED", (object) list.Count);
      else
        str = GameStrings.Get("GLUE_SINGLE_CARD_ADDED");
      info2.m_description = str;
      info1.m_cards = list;
      info1.m_callbackOnHide = (DialogBase.HideCallback) ((dialog, userData) => this.MarkCardChangeEventAsSeen(eventToShow.Item1));
      CardListPopup.Info info3 = info1;
      info3.m_useMultiLineDescription = info3.m_description.Contains('\n');
      DialogManager.Get().ShowCardListPopup(ignoredAttentionBlockers, info3);
    }
    else
    {
      CardListPopup.Info info = new CardListPopup.Info()
      {
        m_description = GameStrings.Get(list.Count == 1 ? "GLUE_SINGLE_CARD_UPDATED" : "GLUE_CARDS_UPDATED"),
        m_cards = list,
        m_callbackOnHide = (DialogBase.HideCallback) ((dialog, userData) => this.MarkCardChangeEventAsSeen(eventToShow.Item1))
      };
      info.m_useMultiLineDescription = info.m_description.Contains('\n');
      DialogManager.Get().ShowCardListPopup(ignoredAttentionBlockers, info);
    }
    return true;
  }
}
