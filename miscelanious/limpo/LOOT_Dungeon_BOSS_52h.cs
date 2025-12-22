using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_52h : LOOT_Dungeon
{
  private static readonly AssetReference LOOTA_BOSS_52h_TreasureVault_Death = new AssetReference("LOOTA_BOSS_52h_TreasureVault_Death.prefab:b6852fd41796e6649b95bbfca14a45e4");
  private static readonly AssetReference LOOTA_BOSS_52h_TreasureVault_Emote = new AssetReference("LOOTA_BOSS_52h_TreasureVault_Emote.prefab:0248c411691a18a4f88409445f837035");
  private static readonly AssetReference LOOTA_BOSS_52h_TreasureVault_Intro = new AssetReference("LOOTA_BOSS_52h_TreasureVault_Intro.prefab:dd6522622d543b742a2766c78d14f3e3");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LOOT_Dungeon_BOSS_52h.LOOTA_BOSS_52h_TreasureVault_Death,
      (string) LOOT_Dungeon_BOSS_52h.LOOTA_BOSS_52h_TreasureVault_Emote,
      (string) LOOT_Dungeon_BOSS_52h.LOOTA_BOSS_52h_TreasureVault_Intro
    })
      this.PreloadSound(soundPath);
  }

  protected override string GetBossDeathLine() => (string) LOOT_Dungeon_BOSS_52h.LOOTA_BOSS_52h_TreasureVault_Death;

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_52h.LOOTA_BOSS_52h_TreasureVault_Intro, Notification.SpeechBubbleDirection.None, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_52h.LOOTA_BOSS_52h_TreasureVault_Emote, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_52h lootDungeonBoss52h = this;
    while (lootDungeonBoss52h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss52h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
