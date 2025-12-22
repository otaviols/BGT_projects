using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_68h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_PoisonMinionLines = new List<string>()
  {
    "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_01.prefab:07927380484b26541be4b49e7a8aad33",
    "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_02.prefab:ef7a4e3aea34db34caa72aa721f6ee45",
    "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_03.prefab:dc750561f86b04b48bdc5e3516c6b41a",
    "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_04.prefab:057102807e5fe1943b8b8780ba1c37a3"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_68h_Male_Undead_Intro_01.prefab:d41182657f7477847966469d4eb6fc08",
      "VO_GILA_BOSS_68h_Male_Undead_EmoteResponse_01.prefab:e1dfb4ab0b0331a4abcb536c5896186a",
      "VO_GILA_BOSS_68h_Male_Undead_Death_01.prefab:75f7dabf0922e044aac6a4f8a7315238",
      "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_01.prefab:07927380484b26541be4b49e7a8aad33",
      "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_02.prefab:ef7a4e3aea34db34caa72aa721f6ee45",
      "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_03.prefab:dc750561f86b04b48bdc5e3516c6b41a",
      "VO_GILA_BOSS_68h_Male_Undead_EventPlayPoison_04.prefab:057102807e5fe1943b8b8780ba1c37a3"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_68h_Male_Undead_Death_01.prefab:75f7dabf0922e044aac6a4f8a7315238";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_68h_Male_Undead_Intro_01.prefab:d41182657f7477847966469d4eb6fc08", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_68h_Male_Undead_EmoteResponse_01.prefab:e1dfb4ab0b0331a4abcb536c5896186a", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_68h gilDungeonBoss68h = this;
    while (gilDungeonBoss68h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss68h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss68h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss68h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (entity.HasTag(GAME_TAG.POISONOUS))
      {
        string line = gilDungeonBoss68h.PopRandomLineWithChance(gilDungeonBoss68h.m_PoisonMinionLines);
        if (line != null)
          yield return (object) gilDungeonBoss68h.PlayLineOnlyOnce(actor, line);
      }
    }
  }
}
