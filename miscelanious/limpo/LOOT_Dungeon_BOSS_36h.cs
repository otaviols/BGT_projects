using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_36h : LOOT_Dungeon
{
  private static readonly AssetReference VO_LOOT_329_Male_Elemental_Attack_01 = new AssetReference("VO_LOOT_329_Male_Elemental_Attack_01.prefab:c263e413027e1c3419330793de1a9b83");
  private static readonly AssetReference VO_LOOT_329_Male_Elemental_Death_01 = new AssetReference("VO_LOOT_329_Male_Elemental_Death_01.prefab:7fda354ba88198d4992562d4c9b51373");
  private static readonly AssetReference VO_LOOT_329_Male_Elemental_Play_01 = new AssetReference("VO_LOOT_329_Male_Elemental_Play_01.prefab:acccd0bdaf7b3964d8c782e6191599c5");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LOOT_Dungeon_BOSS_36h.VO_LOOT_329_Male_Elemental_Attack_01,
      (string) LOOT_Dungeon_BOSS_36h.VO_LOOT_329_Male_Elemental_Death_01,
      (string) LOOT_Dungeon_BOSS_36h.VO_LOOT_329_Male_Elemental_Play_01
    })
      this.PreloadSound(soundPath);
  }

  protected override string GetBossDeathLine() => (string) LOOT_Dungeon_BOSS_36h.VO_LOOT_329_Male_Elemental_Death_01;

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_36h.VO_LOOT_329_Male_Elemental_Play_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_36h.VO_LOOT_329_Male_Elemental_Attack_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_36h lootDungeonBoss36h = this;
    while (lootDungeonBoss36h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss36h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
