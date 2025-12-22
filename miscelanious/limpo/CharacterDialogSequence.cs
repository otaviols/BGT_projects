using System;
using System.Collections;
using System.Collections.Generic;

public class CharacterDialogSequence : IEnumerable<CharacterDialog>, IEnumerable
{
  private List<CharacterDialog> m_dialogItems;
  public CharacterDialogDbfRecord m_characterDialogRecord;
  public int m_onCompleteBannerId;
  public bool m_ignorePopups = true;
  public bool m_deferOnComplete = true;
  public bool m_blockInput = true;
  public Action<CharacterDialogSequence> m_onPreShow;

  public int Count => this.m_dialogItems.Count;

  public CharacterDialogSequence(int dialogSequenceId, CharacterDialogEventType eventType = CharacterDialogEventType.UNSPECIFIED)
  {
    CharacterDialogDbfRecord record = GameDbf.CharacterDialog.GetRecord(dialogSequenceId);
    this.m_characterDialogRecord = record;
    this.m_onCompleteBannerId = record.OnCompleteBannerId;
    this.m_ignorePopups = record.IgnorePopups;
    this.m_deferOnComplete = record.DeferOnComplete;
    this.m_blockInput = record.BlockInput;
    this.m_dialogItems = new List<CharacterDialog>();
    List<CharacterDialogItemsDbfRecord> records = GameDbf.CharacterDialogItems.GetRecords();
    int index = 0;
    for (int count = records.Count; index < count; ++index)
    {
      CharacterDialogItemsDbfRecord dialogItemsDbfRecord = records[index];
      if (dialogItemsDbfRecord.CharacterDialogId == dialogSequenceId)
      {
        if (eventType != CharacterDialogEventType.UNSPECIFIED)
        {
          CharacterDialogEventType result = CharacterDialogEventType.UNSPECIFIED;
          if (dialogItemsDbfRecord.AchieveEventType != null)
            Enum.TryParse<CharacterDialogEventType>(dialogItemsDbfRecord.AchieveEventType, true, out result);
          if (result != eventType)
            continue;
        }
        this.m_dialogItems.Add(new CharacterDialog()
        {
          dbfRecordId = dialogItemsDbfRecord.ID,
          playOrder = dialogItemsDbfRecord.PlayOrder,
          useInnkeeperQuote = dialogItemsDbfRecord.UseInnkeeperQuote,
          prefabName = dialogItemsDbfRecord.PrefabName,
          bannerPrefabName = dialogItemsDbfRecord.BannerPrefabName,
          audioName = dialogItemsDbfRecord.AudioName,
          useAltSpeechBubble = dialogItemsDbfRecord.AltBubblePosition,
          waitBefore = (float) dialogItemsDbfRecord.WaitBefore,
          waitAfter = (float) dialogItemsDbfRecord.WaitAfter,
          persistPrefab = dialogItemsDbfRecord.PersistPrefab,
          useAltPosition = dialogItemsDbfRecord.AltPosition,
          minimumDurationSeconds = (float) dialogItemsDbfRecord.MinimumDurationSeconds,
          localeExtraSeconds = (float) dialogItemsDbfRecord.LocaleExtraSeconds,
          bubbleText = dialogItemsDbfRecord.BubbleText,
          useBannerStyle = dialogItemsDbfRecord.UseBannerStyle,
          canvasAnchor = (CanvasAnchor) dialogItemsDbfRecord.BannerAnchorPosition
        });
      }
    }
    this.m_dialogItems.Sort((Comparison<CharacterDialog>) ((a, b) =>
    {
      if (a.playOrder < b.playOrder)
        return -1;
      return a.playOrder > b.playOrder ? 1 : 0;
    }));
  }

  public static List<string> GetAudioOfCharacterDialogSequence(int dialogSequenceId)
  {
    List<string> characterDialogSequence = new List<string>();
    CharacterDialogDbfRecord record = GameDbf.CharacterDialog.GetRecord(dialogSequenceId);
    foreach (CharacterDialogItemsDbfRecord dialogItemsDbfRecord in GameDbf.CharacterDialogItems.GetRecords().FindAll((Predicate<CharacterDialogItemsDbfRecord>) (obj => obj.CharacterDialogId == record.ID)))
      characterDialogSequence.Add(dialogItemsDbfRecord.AudioName);
    return characterDialogSequence;
  }

  public IEnumerator<CharacterDialog> GetEnumerator()
  {
    foreach (CharacterDialog dialogItem in this.m_dialogItems)
      yield return dialogItem;
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
