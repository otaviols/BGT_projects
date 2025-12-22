using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_32h : GIL_Dungeon
{
  private List<string> m_PlayerAoE = new List<string>()
  {
    "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysAoEBuff_01.prefab:5f698b1cc65290c4c9f1db1cb30869a4",
    "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysAoEBuff_02.prefab:d20aec1c64396dd47ab5d7dd4318dd68"
  };
  private List<string> m_BigMinion = new List<string>()
  {
    "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysBigMinion_01.prefab:432161c3bcb0f3043bcd0dfbe5ad13e4",
    "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysBigMinion_02.prefab:4bf7fa157adb5e5448034cf92565a9df"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_32h_Female_Sprite_Intro_01.prefab:7da1bbcac53b07947807b53a82b5fb0c",
      "VO_GILA_BOSS_32h_Female_Sprite_EmoteResponse_01.prefab:047de08f89dd6d648ae29bd17fe4a201",
      "VO_GILA_BOSS_32h_Female_Sprite_Death_01.prefab:865008e5ebcb3ad41a9fe76d3e400773",
      "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_01.prefab:7758b3cb7c06072498e6c91d0cfeca79",
      "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_02.prefab:77e3de115374db848bf3f3441123e1c9",
      "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_03.prefab:5c068ac1cfce92043905c4d0ab6c860a",
      "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_04.prefab:d957d877bb1d7154b8573a47031bdbf1",
      "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysAoEBuff_01.prefab:5f698b1cc65290c4c9f1db1cb30869a4",
      "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysAoEBuff_02.prefab:d20aec1c64396dd47ab5d7dd4318dd68",
      "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysBigMinion_01.prefab:432161c3bcb0f3043bcd0dfbe5ad13e4",
      "VO_GILA_BOSS_32h_Female_Sprite_EventPlaysBigMinion_02.prefab:4bf7fa157adb5e5448034cf92565a9df"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_01.prefab:7758b3cb7c06072498e6c91d0cfeca79",
    "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_02.prefab:77e3de115374db848bf3f3441123e1c9",
    "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_03.prefab:5c068ac1cfce92043905c4d0ab6c860a",
    "VO_GILA_BOSS_32h_Female_Sprite_HeroPower_04.prefab:d957d877bb1d7154b8573a47031bdbf1"
  };

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_32h_Female_Sprite_Death_01.prefab:865008e5ebcb3ad41a9fe76d3e400773";

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_32h_Female_Sprite_Intro_01.prefab:7da1bbcac53b07947807b53a82b5fb0c", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_32h_Female_Sprite_EmoteResponse_01.prefab:047de08f89dd6d648ae29bd17fe4a201", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_32h gilDungeonBoss32h = this;
    while (gilDungeonBoss32h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        string line1 = gilDungeonBoss32h.PopRandomLineWithChance(gilDungeonBoss32h.m_PlayerAoE);
        if (line1 == null)
          break;
        yield return (object) gilDungeonBoss32h.PlayLineOnlyOnce(actor, line1);
        break;
      case 102:
        string line2 = gilDungeonBoss32h.PopRandomLineWithChance(gilDungeonBoss32h.m_BigMinion);
        if (line2 == null)
          break;
        yield return (object) gilDungeonBoss32h.PlayLineOnlyOnce(actor, line2);
        break;
    }
  }
}
