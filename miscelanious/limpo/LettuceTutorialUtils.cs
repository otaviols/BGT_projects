using Assets;
using Hearthstone.UI;
using PegasusLettuce;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class LettuceTutorialUtils
{
  public static Dictionary<LettuceTutorialVo.LettuceTutorialEvent, int> SpecificEventMap = new Dictionary<LettuceTutorialVo.LettuceTutorialEvent, int>()
  {
    {
      LettuceTutorialVo.LettuceTutorialEvent.INVALID,
      0
    },
    {
      LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_END,
      34
    }
  };

  public static bool IsEventTypeComplete(
    LettuceTutorialVo.LettuceTutorialEvent tutorialEvent)
  {
    return LettuceTutorialUtils.IsEventTypeComplete(tutorialEvent, 0, 0, (LettuceTutorialVoDbfRecord) null, out List<long> _);
  }

  public static bool IsEventTypeComplete(
    LettuceTutorialVo.LettuceTutorialEvent tutorialEvent,
    int nodeTypeId,
    int bountyRecordId,
    LettuceTutorialVoDbfRecord vo,
    out List<long> values)
  {
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TUTORIAL_EVENTS, out values);
    bool flag = false;
    if (vo == null)
    {
      foreach (LettuceTutorialVoDbfRecord record in GameDbf.LettuceTutorialVo.GetRecords((Predicate<LettuceTutorialVoDbfRecord>) (r => r.TutorialEvent == tutorialEvent)))
      {
        if (LettuceTutorialUtils.CanRecordPlayUnderCurrentConditions(record, nodeTypeId, bountyRecordId))
        {
          flag = true;
          List<long> longList = values;
          // ISSUE: explicit non-virtual call
          if ((longList != null ? (__nonvirtual (longList.Contains((long) record.ID)) ? 1 : 0) : 0) != 0)
            return true;
        }
      }
      if (!flag)
        Log.Lettuce.PrintError(string.Format("unable to find playable VO Record for tutorial event {0}", (object) tutorialEvent));
      return false;
    }
    List<long> longList1 = values;
    // ISSUE: explicit non-virtual call
    return (longList1 != null ? (__nonvirtual (longList1.Contains((long) vo.ID)) ? 1 : 0) : 0) != 0;
  }

  public static bool IsSpecificEventComplete(LettuceTutorialVo.LettuceTutorialEvent eventType) => LettuceTutorialUtils.IsSpecificEventComplete(LettuceTutorialUtils.SpecificEventMap[eventType]);

  public static bool IsSpecificEventComplete(int tutorialId)
  {
    if (tutorialId <= 0)
      return false;
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TUTORIAL_EVENTS, out values);
    // ISSUE: explicit non-virtual call
    return values != null && __nonvirtual (values.Contains((long) tutorialId));
  }

  public static bool CanRecordPlayUnderCurrentConditions(
    LettuceTutorialVoDbfRecord vo,
    int nodeTypeId = 0,
    int bountyRecordId = 0)
  {
    if (vo.NodeTypeId != 0 && vo.NodeTypeId != nodeTypeId || vo.RequiredActiveBounty != 0 && vo.RequiredActiveBounty != bountyRecordId)
      return false;
    if (vo.RequiredActiveTask != 0)
    {
      MercenariesTaskState taskStateById = LettuceVillageDataUtil.GetTaskStateByID(vo.RequiredActiveTask);
      if (taskStateById == null || taskStateById.Status_ == MercenariesTaskState.Status.COMPLETE || taskStateById.Status_ == MercenariesTaskState.Status.CLAIMED || taskStateById.Status_ == MercenariesTaskState.Status.INVALID)
        return false;
    }
    return vo.RequiredActiveVisitor == 0 || LettuceVillageDataUtil.GetVisitorStateByID(vo.RequiredActiveVisitor) != null;
  }

  public static bool ForceCompleteEvent(
    LettuceTutorialVo.LettuceTutorialEvent tutorialEvent,
    int nodeTypeId = 0,
    int bountyRecordId = 0)
  {
    foreach (LettuceTutorialVoDbfRecord record in GameDbf.LettuceTutorialVo.GetRecords((Predicate<LettuceTutorialVoDbfRecord>) (r => r.TutorialEvent == tutorialEvent)))
    {
      if (LettuceTutorialUtils.CanRecordPlayUnderCurrentConditions(record, nodeTypeId, bountyRecordId))
      {
        List<long> values;
        if (LettuceTutorialUtils.IsEventTypeComplete(tutorialEvent, nodeTypeId, bountyRecordId, record, out values))
          return true;
        values = values ?? new List<long>();
        values.Add((long) record.ID);
        return GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TUTORIAL_EVENTS, values.ToArray()));
      }
    }
    Log.Lettuce.PrintError(string.Format("VO Record not found for tutorial event {0}", (object) tutorialEvent));
    return false;
  }

  public static bool FireEvent(
    LettuceTutorialVo.LettuceTutorialEvent tutorialEvent,
    GameObject gameObject,
    int nodeTypeId = 0,
    int bountyRecordId = 0,
    Action onComplete = null)
  {
    foreach (LettuceTutorialVoDbfRecord record in GameDbf.LettuceTutorialVo.GetRecords((Predicate<LettuceTutorialVoDbfRecord>) (r => r.TutorialEvent == tutorialEvent)))
    {
      LettuceTutorialVoDbfRecord vo = record;
      List<long> values;
      if (!LettuceTutorialUtils.IsEventTypeComplete(tutorialEvent, nodeTypeId, bountyRecordId, vo, out values) || !vo.OnlyShowOnce)
      {
        values = values ?? new List<long>();
        if (LettuceTutorialUtils.CanRecordPlayUnderCurrentConditions(vo, nodeTypeId, bountyRecordId) && vo.ShowChance != 0 && (vo.ShowChance >= 100 || UnityEngine.Random.Range(0, 100) <= vo.ShowChance))
        {
          if (!string.IsNullOrWhiteSpace(vo.UiEvent))
            SendEventUpwardStateAction.SendEventUpward(gameObject, vo.UiEvent);
          if (!string.IsNullOrWhiteSpace(vo.Popup))
          {
            AssetReference assetRef = new AssetReference(vo.Popup);
            GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab(assetRef);
            gameObject1.transform.SetParent(gameObject.transform, true);
            if (onComplete != null)
            {
              TutorialNotification component = gameObject1.GetComponent<TutorialNotification>();
              if ((UnityEngine.Object) component == (UnityEngine.Object) null || (UnityEngine.Object) component.m_ButtonStart == (UnityEngine.Object) null)
                Log.Lettuce.PrintError(string.Format("Popup prefab for tutorial VO event {0} needs a root TutorialNotification component with valid ButtonStart reference.", (object) vo.ID));
              component.m_ButtonStart.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (_ => LettuceTutorialUtils.CompleteEvent(vo, gameObject, nodeTypeId, bountyRecordId, onComplete)));
            }
          }
          else if (vo.TutorialDialog > 0)
            NarrativeManager.Get().PlayMercenariesTutorialDialogue(vo.TutorialDialog, (Action) (() => LettuceTutorialUtils.CompleteEvent(vo, gameObject, nodeTypeId, bountyRecordId, onComplete)));
          values.Add((long) vo.ID);
          GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TUTORIAL_EVENTS, values.ToArray()));
          return true;
        }
      }
    }
    Action action = onComplete;
    if (action != null)
      action();
    return false;
  }

  private static void CompleteEvent(
    LettuceTutorialVoDbfRecord record,
    GameObject gameObject,
    int mapNodeTypeId,
    int bountyRecordId,
    Action onComplete)
  {
    if (record.TriggerEventOnComplete != LettuceTutorialVo.LettuceTutorialEvent.INVALID)
    {
      LettuceTutorialUtils.FireEvent(record.TriggerEventOnComplete, gameObject, mapNodeTypeId, bountyRecordId, onComplete);
    }
    else
    {
      if (onComplete == null)
        return;
      onComplete();
    }
  }
}
