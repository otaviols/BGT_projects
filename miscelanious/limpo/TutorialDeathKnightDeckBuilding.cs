using System;
using UnityEngine;

public class TutorialDeathKnightDeckBuilding : MonoBehaviour
{
  private static TutorialDeathKnightDeckBuilding s_instance;
  private const string EVENT_DATA_SHOW_RUNE_POPUP = "SHOW_RUNE_POPUP";
  private const string EVENT_DATA_SHOW_RUNE_INDICATOR_ARROW = "SHOW_RUNE_INDICATOR_ARROW";

  private void Awake()
  {
    if (!(bool) (UnityEngine.Object) TutorialDeathKnightDeckBuilding.s_instance)
    {
      TutorialDeathKnightDeckBuilding.s_instance = this;
    }
    else
    {
      if (!((UnityEngine.Object) TutorialDeathKnightDeckBuilding.s_instance != (UnityEngine.Object) this))
        return;
      Debug.LogWarning((object) "TutorialDeathKnightDeckBuilding object should only be instantiated by the Initialize function.");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
  }

  private void OnEnable()
  {
    UIVoiceLinesManager.VoiceLineStarted += new Action<UIVoiceLineItem>(TutorialDeathKnightDeckBuilding.OnVoiceLineStarted);
    UIVoiceLinesManager.VoiceLineFinished += new Action<UIVoiceLineItem, bool>(TutorialDeathKnightDeckBuilding.OnVoiceLineFinished);
  }

  private void OnDisable()
  {
    UIVoiceLinesManager.VoiceLineStarted -= new Action<UIVoiceLineItem>(TutorialDeathKnightDeckBuilding.OnVoiceLineStarted);
    UIVoiceLinesManager.VoiceLineFinished -= new Action<UIVoiceLineItem, bool>(TutorialDeathKnightDeckBuilding.OnVoiceLineFinished);
  }

  private static void Initialize()
  {
    if ((bool) (UnityEngine.Object) TutorialDeathKnightDeckBuilding.s_instance)
      return;
    TutorialDeathKnightDeckBuilding.s_instance = new GameObject("DK Deck Building Tutorial").AddComponent<TutorialDeathKnightDeckBuilding>();
  }

  private static void OnVoiceLineStarted(UIVoiceLineItem voiceLine)
  {
    if (voiceLine.m_VOTrigger.m_Category != UIVoiceLinesManager.UIVoiceLineCategory.DK_DECK_BUILDING_TUTORIAL)
      return;
    CollectionPageManager pageManager = TutorialDeathKnightDeckBuilding.GetPageManager();
    if ((UnityEngine.Object) pageManager == (UnityEngine.Object) null)
      return;
    string eventData = voiceLine.m_eventData;
    if (!(eventData == "SHOW_RUNE_POPUP"))
    {
      if (!(eventData == "SHOW_RUNE_INDICATOR_ARROW") || !((UnityEngine.Object) pageManager != (UnityEngine.Object) null))
        return;
      pageManager.ShowRuneIndicatorArrowForTutorial();
    }
    else
    {
      if (!((UnityEngine.Object) pageManager != (UnityEngine.Object) null))
        return;
      pageManager.ShowRuneCardPopupForTutorial();
    }
  }

  private static void OnVoiceLineFinished(UIVoiceLineItem voiceLine, bool isFinalVoiceLine)
  {
    if (voiceLine.m_VOTrigger.m_Category != UIVoiceLinesManager.UIVoiceLineCategory.DK_DECK_BUILDING_TUTORIAL)
      return;
    CollectionPageManager pageManager = TutorialDeathKnightDeckBuilding.GetPageManager();
    if ((UnityEngine.Object) pageManager == (UnityEngine.Object) null)
      return;
    string eventData = voiceLine.m_eventData;
    if (!(eventData == "SHOW_RUNE_POPUP"))
    {
      if (eventData == "SHOW_RUNE_INDICATOR_ARROW" && (UnityEngine.Object) pageManager != (UnityEngine.Object) null)
        pageManager.DismissRuneIndicatorArrowForTutorial();
    }
    else if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
      pageManager.DismissRuneCardPopupForTutorial();
    if (!isFinalVoiceLine)
      return;
    GameSaveKeySubkeyId voiceLineTriggerType = TutorialDeathKnightDeckBuilding.GetGSDKeyForVoiceLineTriggerType(voiceLine.m_VOTrigger.m_TriggerType);
    if (voiceLineTriggerType == GameSaveKeySubkeyId.INVALID)
      return;
    TutorialDeathKnightDeckBuilding.SetTutorialSeen(voiceLineTriggerType);
  }

  private static bool HasSeenTutorial(GameSaveKeySubkeyId tutorialSeenSubKey)
  {
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.FTUE, tutorialSeenSubKey, out num);
    return num > 0L;
  }

  private static void SetTutorialSeen(GameSaveKeySubkeyId tutorialSeenSubKey)
  {
    if (TutorialDeathKnightDeckBuilding.HasSeenTutorial(tutorialSeenSubKey))
      return;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.FTUE, tutorialSeenSubKey, new long[1]
    {
      1L
    }));
  }

  private static CollectionPageManager GetPageManager()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return (CollectionPageManager) null;
    CollectibleDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay();
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      return (CollectionPageManager) null;
    CollectionPageManager pageManager = collectibleDisplay.GetPageManager() as CollectionPageManager;
    return !((UnityEngine.Object) pageManager == (UnityEngine.Object) null) ? pageManager : (CollectionPageManager) null;
  }

  private static GameSaveKeySubkeyId GetGSDKeyForVoiceLineTriggerType(
    UIVoiceLinesManager.TriggerType triggerType)
  {
    switch (triggerType)
    {
      case UIVoiceLinesManager.TriggerType.STARTED_EDITING_DEATH_KNIGHT_DECK:
        return GameSaveKeySubkeyId.HAS_SEEN_DK_DECK_BUILDING_INTRO_TUTORIAL;
      case UIVoiceLinesManager.TriggerType.ADDED_TRIPLE_DEATH_KNIGHT_RUNES:
        return GameSaveKeySubkeyId.HAS_SEEN_DK_DECK_BUILDING_TRIPLE_RUNES_POPUP;
      case UIVoiceLinesManager.TriggerType.REMOVED_THIRD_RUNE:
        return GameSaveKeySubkeyId.HAS_SEEN_DK_DECK_BUILDING_RUNE_SLOT_AVAILABLE_POPUP;
      case UIVoiceLinesManager.TriggerType.CANNOT_ADD_RUNES:
        return GameSaveKeySubkeyId.HAS_SEEN_DK_DECK_BUILDING_CANNOT_ADD_RUNES_POPUP;
      default:
        return GameSaveKeySubkeyId.INVALID;
    }
  }

  public static void ShowTutorial(UIVoiceLinesManager.TriggerType tutorialTrigger)
  {
    GameSaveKeySubkeyId voiceLineTriggerType = TutorialDeathKnightDeckBuilding.GetGSDKeyForVoiceLineTriggerType(tutorialTrigger);
    if (voiceLineTriggerType == GameSaveKeySubkeyId.INVALID || TutorialDeathKnightDeckBuilding.HasSeenTutorial(voiceLineTriggerType))
      return;
    TutorialDeathKnightDeckBuilding.Initialize();
    UIVoiceLinesManager.Get().ExecuteTrigger(UIVoiceLinesManager.UIVoiceLineCategory.DK_DECK_BUILDING_TUTORIAL, tutorialTrigger);
  }
}
