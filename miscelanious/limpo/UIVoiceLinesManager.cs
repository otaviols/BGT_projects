using System;
using System.Collections.Generic;
using UnityEngine;

public class UIVoiceLinesManager : MonoBehaviour
{
  [SerializeField]
  private UIVoiceLinesList[] m_UIVoiceLineLists;
  private static UIVoiceLinesManager s_instance;

  public static event Action<UIVoiceLineItem> VoiceLineStarted;

  public static event Action<UIVoiceLineItem, bool> VoiceLineFinished;

  public static UIVoiceLinesManager Get() => UIVoiceLinesManager.s_instance;

  private void Awake()
  {
    if ((UnityEngine.Object) UIVoiceLinesManager.s_instance != (UnityEngine.Object) null)
      Debug.LogWarning((object) "UIVoiceLinesManager is supposed to be a singleton, but a second instance of it is being created!");
    UIVoiceLinesManager.s_instance = this;
  }

  public void ExecuteTrigger(
    UIVoiceLinesManager.UIVoiceLineCategory category,
    UIVoiceLinesManager.TriggerType triggerType,
    int param1 = 0,
    string param2 = "")
  {
    if (this.m_UIVoiceLineLists.Length == 0)
      return;
    List<UIVoiceLineItem> dialogues = new List<UIVoiceLineItem>();
    for (int index1 = 0; index1 < this.m_UIVoiceLineLists.Length; ++index1)
    {
      UIVoiceLinesList uiVoiceLineList = this.m_UIVoiceLineLists[index1];
      if (uiVoiceLineList.m_Category == UIVoiceLinesManager.UIVoiceLineCategory.ALL || uiVoiceLineList.m_Category == category)
      {
        if (uiVoiceLineList.m_DialogueItems.Count == 0)
        {
          Debug.LogError((object) "No Voice Line Items available, List empty!");
        }
        else
        {
          List<UIVoiceLineItem> dialogueItems = uiVoiceLineList.m_DialogueItems;
          for (int index2 = 0; index2 < dialogueItems.Count; ++index2)
          {
            UIVoiceLineItem uiVoiceLineItem = dialogueItems[index2];
            if (triggerType == uiVoiceLineItem.m_VOTrigger.m_TriggerType && (uiVoiceLineItem.m_VOTrigger.m_AdditonalStringParam == param2 || uiVoiceLineItem.m_VOTrigger.m_AdditonalIntParam == param1))
              dialogues.Add(uiVoiceLineItem);
          }
        }
      }
    }
    if (dialogues.Count == 1)
    {
      UIVoiceLineItem item = dialogues[0];
      Action<UIVoiceLineItem> voiceLineStarted = UIVoiceLinesManager.VoiceLineStarted;
      if (voiceLineStarted != null)
        voiceLineStarted(item);
      this.PlayVoiceLine(item.m_SoundReference, item.m_VisualAssetReference, item.m_StringToLocalize, item.m_AllowRepeatDuringSession, item.m_BlockAllOtherInput, item.m_AnchorPoint, item.m_Position, (Action<int>) (groupId =>
      {
        Action<UIVoiceLineItem, bool> voiceLineFinished = UIVoiceLinesManager.VoiceLineFinished;
        if (voiceLineFinished == null)
          return;
        voiceLineFinished(item, true);
      }));
    }
    else
    {
      if (dialogues.Count <= 1)
        return;
      this.PlayMultipleVoiceLines(0, dialogues);
    }
  }

  private void PlayVoiceLine(
    string soundRef,
    string visualReference,
    string stringToLocalize,
    bool allowRepeatDuringSession,
    bool blockAllOtherInput,
    CanvasAnchor anchorPoint,
    Vector3 position,
    Action<int> finishCallback = null)
  {
    NotificationManager.Get().CreateCharacterQuote(visualReference, position, GameStrings.Get(stringToLocalize), soundRef, allowRepeatDuringSession, finishCallback: finishCallback, anchorPoint: anchorPoint, blockAllOtherInput: blockAllOtherInput);
  }

  private void PlayMultipleVoiceLines(int counter, List<UIVoiceLineItem> dialogues)
  {
    UIVoiceLineItem item = dialogues[counter];
    Action<int> finishCallback = counter >= dialogues.Count - 1 ? (Action<int>) (groupId =>
    {
      Action<UIVoiceLineItem, bool> voiceLineFinished = UIVoiceLinesManager.VoiceLineFinished;
      if (voiceLineFinished == null)
        return;
      voiceLineFinished(item, true);
    }) : (Action<int>) (groupId =>
    {
      Action<UIVoiceLineItem, bool> voiceLineFinished = UIVoiceLinesManager.VoiceLineFinished;
      if (voiceLineFinished != null)
        voiceLineFinished(item, false);
      this.PlayMultipleVoiceLines(counter + 1, dialogues);
    });
    Action<UIVoiceLineItem> voiceLineStarted = UIVoiceLinesManager.VoiceLineStarted;
    if (voiceLineStarted != null)
      voiceLineStarted(item);
    this.PlayVoiceLine(item.m_SoundReference, item.m_VisualAssetReference, item.m_StringToLocalize, item.m_AllowRepeatDuringSession, item.m_BlockAllOtherInput, item.m_AnchorPoint, item.m_Position, finishCallback);
  }

  public enum TriggerType
  {
    NONE,
    BUTTON_PRESSED,
    DUNGEON_RUN_BOSS_REVEAL,
    BOSS_COIN_CLICKED,
    STARTED_EDITING_DEATH_KNIGHT_DECK,
    ADDED_TRIPLE_DEATH_KNIGHT_RUNES,
    REMOVED_THIRD_RUNE,
    CANNOT_ADD_RUNES,
  }

  public enum UIVoiceLineCategory
  {
    ALL,
    ADVENTURE,
    EVENTS,
    DK_DECK_BUILDING_TUTORIAL,
  }
}
