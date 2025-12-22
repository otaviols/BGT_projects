using System;
using System.Collections;
using System.Collections.Generic;

public class GenericDungeonMissionEntity : MissionEntity
{
  protected Dictionary<int, GenericDungeonMissionEntity.VOPool> m_VOPools = new Dictionary<int, GenericDungeonMissionEntity.VOPool>();
  private GameSaveKeyId m_gameSaveDataClientKey = GameSaveKeyId.INVALID;

  public virtual AdventureDbId GetAdventureID() => AdventureDbId.INVALID;

  public override void PreloadAssets()
  {
    AdventureDataDbfRecord record = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == this.GetAdventureID()));
    if (record != null)
      this.m_gameSaveDataClientKey = (GameSaveKeyId) record.GameSaveDataClientKey;
    foreach (KeyValuePair<int, GenericDungeonMissionEntity.VOPool> voPool in this.m_VOPools)
    {
      foreach (string soundFile in voPool.Value.m_soundFiles)
        this.PreloadSound(soundFile);
      if (this.m_gameSaveDataClientKey != GameSaveKeyId.INVALID && voPool.Value.m_oncePerAccountGameSaveSubkey != GameSaveKeySubkeyId.INVALID)
        GameSaveDataManager.Get().GetSubkeyValue(this.m_gameSaveDataClientKey, voPool.Value.m_oncePerAccountGameSaveSubkey, out voPool.Value.m_timesOncePerAccountVOSeen);
    }
  }

  protected virtual bool CanPlayVOLines(
    Entity heroEntity,
    GenericDungeonMissionEntity.VOSpeaker speaker)
  {
    return true;
  }

  protected Card ResolveSpeakerCard(GenericDungeonMissionEntity.VOSpeaker speaker)
  {
    switch (speaker)
    {
      case GenericDungeonMissionEntity.VOSpeaker.FRIENDLY_HERO:
        return GameState.Get().GetFriendlySidePlayer()?.GetHeroCard();
      case GenericDungeonMissionEntity.VOSpeaker.OPPONENT_HERO:
        return GameState.Get().GetOpposingSidePlayer()?.GetHeroCard();
      default:
        return (Card) null;
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GenericDungeonMissionEntity dungeonMissionEntity = this;
    if (dungeonMissionEntity.m_VOPools.ContainsKey(missionEvent))
    {
      while (dungeonMissionEntity.m_enemySpeaking)
        yield return (object) null;
      GenericDungeonMissionEntity.VOPool voPool = dungeonMissionEntity.m_VOPools[missionEvent];
      if (voPool != null && (voPool.m_oncePerAccountGameSaveSubkey == GameSaveKeySubkeyId.INVALID || voPool.m_timesOncePerAccountVOSeen <= 0L))
      {
        Actor speaker = (Actor) null;
        if (string.IsNullOrEmpty(voPool.m_quotePrefabPath))
        {
          Card card = dungeonMissionEntity.ResolveSpeakerCard(voPool.m_speaker);
          if ((UnityEngine.Object) card == (UnityEngine.Object) null)
          {
            yield break;
          }
          else
          {
            Entity entity = card.GetEntity();
            if (entity == null)
            {
              yield break;
            }
            else
            {
              speaker = card.GetActor();
              if ((UnityEngine.Object) speaker == (UnityEngine.Object) null || !dungeonMissionEntity.CanPlayVOLines(entity, voPool.m_speaker))
                yield break;
            }
          }
        }
        List<string> stringList = new List<string>((IEnumerable<string>) voPool.m_soundFiles);
        if (stringList != null && stringList.Count != 0 && (double) voPool.m_chanceToPlay >= (double) UnityEngine.Random.Range(0.0f, 1f))
        {
          string soundFile;
          do
          {
            soundFile = stringList[UnityEngine.Random.Range(0, stringList.Count)];
            if (NotificationManager.Get().HasSoundPlayedThisSession(soundFile))
              stringList.Remove(soundFile);
            else
              goto label_25;
          }
          while (stringList.Count != 0);
          if (voPool.m_shouldPlay != MissionEntity.ShouldPlayValue.Always)
          {
            yield break;
          }
          else
          {
            for (int index = 0; index < voPool.m_soundFiles.Count; ++index)
              NotificationManager.Get().ForceRemoveSoundFromPlayedList(voPool.m_soundFiles[index]);
            soundFile = voPool.m_soundFiles[UnityEngine.Random.Range(0, voPool.m_soundFiles.Count)];
          }
label_25:
          if (!string.IsNullOrEmpty(soundFile))
          {
            if (voPool.m_oncePerAccountGameSaveSubkey != GameSaveKeySubkeyId.INVALID)
            {
              ++voPool.m_timesOncePerAccountVOSeen;
              GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(dungeonMissionEntity.m_gameSaveDataClientKey, voPool.m_oncePerAccountGameSaveSubkey, new long[1]
              {
                1L
              }));
            }
            if (string.IsNullOrEmpty(voPool.m_quotePrefabPath))
            {
              yield return (object) dungeonMissionEntity.PlayCriticalLine(speaker, soundFile);
            }
            else
            {
              dungeonMissionEntity.m_enemySpeaking = true;
              yield return (object) dungeonMissionEntity.PlayBossLine(voPool.m_quotePrefabPath, soundFile);
              dungeonMissionEntity.m_enemySpeaking = false;
            }
          }
        }
      }
    }
  }

  protected IEnumerator WaitForEntitySoundToFinish(Entity entity)
  {
    List<CardSoundSpell> playSoundSpells = entity.GetCard().GetPlaySoundSpells(0, false);
    if (playSoundSpells != null && playSoundSpells.Count > 0)
    {
      CardSoundSpell firstSoundSpell = playSoundSpells[0];
      if (!((UnityEngine.Object) firstSoundSpell == (UnityEngine.Object) null))
      {
        while ((UnityEngine.Object) firstSoundSpell.GetActiveAudioSource() != (UnityEngine.Object) null && firstSoundSpell.GetActiveAudioSource().isPlaying)
          yield return (object) null;
        firstSoundSpell = (CardSoundSpell) null;
      }
    }
  }

  public override string GetNameBannerSubtextOverride(Player.Side playerSide) => string.Empty;

  public override bool ShouldShowHeroClassDuringMulligan(Player.Side playerSide) => playerSide == Player.Side.FRIENDLY;

  protected static string GetOpposingHeroCardID(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    int num = 0;
    foreach (Network.HistCreateGame.PlayerData player in createGame.Players)
    {
      if (player.GameAccountId.IsEmpty())
      {
        num = player.Player.Tags.Find((Predicate<Network.Entity.Tag>) (x => x.Name == 27)).Value;
        break;
      }
    }
    for (int index = 0; index < powerList.Count; ++index)
    {
      Network.PowerHistory power = powerList[index];
      if (power.Type == Network.PowerType.FULL_ENTITY)
      {
        Network.Entity entity = ((Network.HistFullEntity) power).Entity;
        if (entity.ID == num)
          return entity.CardID;
      }
    }
    return "";
  }

  protected virtual float ChanceToPlayRandomVOLine() => 0.5f;

  protected string PopRandomLineWithChance(List<string> lines)
  {
    if (lines.Count == 0 || lines == null)
      return (string) null;
    if ((double) this.ChanceToPlayRandomVOLine() < (double) UnityEngine.Random.Range(0.0f, 1f))
      return (string) null;
    string line = lines[UnityEngine.Random.Range(0, lines.Count)];
    lines.Remove(line);
    return line;
  }

  public GenericDungeonMissionEntity()
    : base()
  {
  }

  public enum VOSpeaker
  {
    INVALID,
    FRIENDLY_HERO,
    OPPONENT_HERO,
  }

  protected class VOPool
  {
    public List<string> m_soundFiles;
    public float m_chanceToPlay = 0.2f;
    public MissionEntity.ShouldPlayValue m_shouldPlay = MissionEntity.ShouldPlayValue.Once;
    public GenericDungeonMissionEntity.VOSpeaker m_speaker;
    public string m_quotePrefabPath;
    public GameSaveKeySubkeyId m_oncePerAccountGameSaveSubkey = GameSaveKeySubkeyId.INVALID;
    public long m_timesOncePerAccountVOSeen;

    public VOPool(
      List<string> soundFiles,
      float chanceToPlay,
      MissionEntity.ShouldPlayValue shouldPlay,
      GenericDungeonMissionEntity.VOSpeaker speaker,
      string quotePrefabPath = "",
      GameSaveKeySubkeyId oncePerAccountGameSaveSubkey = GameSaveKeySubkeyId.INVALID)
    {
      this.m_soundFiles = soundFiles;
      this.m_chanceToPlay = chanceToPlay;
      this.m_shouldPlay = shouldPlay;
      this.m_speaker = speaker;
      this.m_quotePrefabPath = quotePrefabPath;
      this.m_oncePerAccountGameSaveSubkey = oncePerAccountGameSaveSubkey;
    }
  }
}
