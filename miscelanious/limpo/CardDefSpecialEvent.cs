using Assets;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using System;

[Serializable]
public class CardDefSpecialEvent
{
  public Global.RewardTrackType TrackType;
  public CardDefSpecialEvent.EventSceneMode m_SceneMode;
  public ScenarioDbId m_Scenario;
  [CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_PortraitTextureOverride;
  [CustomEditField(Sections = "Portrait", T = EditType.MATERIAL)]
  public string m_PremiumPortraitMaterialOverride;
  [CustomEditField(Sections = "Portrait", T = EditType.UBERANIMATION)]
  public string m_PremiumUberShaderAnimationOverride;
  [CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
  public string m_PremiumPortraitTextureOverride;

  public static CardDefSpecialEvent FindActiveEvent(CardDef cardDef)
  {
    foreach (CardDefSpecialEvent specialEvent in cardDef.m_SpecialEvents)
    {
      EventDetailsDataModel detailsForCurrentEvent = RewardTrackManager.Get().GetEventDetailsForCurrentEvent();
      if (detailsForCurrentEvent != null && detailsForCurrentEvent.RewardTrackType == specialEvent.TrackType)
      {
        SceneMgr.Mode mode = SceneMgr.Mode.INVALID;
        switch (specialEvent.m_SceneMode)
        {
          case CardDefSpecialEvent.EventSceneMode.Arena:
            mode = SceneMgr.Mode.DRAFT;
            break;
          case CardDefSpecialEvent.EventSceneMode.Gameplay:
            mode = SceneMgr.Mode.GAMEPLAY;
            break;
          case CardDefSpecialEvent.EventSceneMode.CollectionManager:
            mode = SceneMgr.Mode.COLLECTIONMANAGER;
            break;
          case CardDefSpecialEvent.EventSceneMode.TavernBrawl:
            mode = SceneMgr.Mode.TAVERN_BRAWL;
            break;
        }
        if (SceneMgr.Get().GetMode() == mode || mode == SceneMgr.Mode.INVALID || (ScenarioDbId) GameMgr.Get().GetMissionId() == specialEvent.m_Scenario)
          return specialEvent;
      }
    }
    return (CardDefSpecialEvent) null;
  }

  public enum EventSceneMode
  {
    All,
    Arena,
    Gameplay,
    CollectionManager,
    TavernBrawl,
  }
}
