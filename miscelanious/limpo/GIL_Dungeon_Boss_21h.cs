using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_21h : GIL_Dungeon
{
  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_21h_Rottooth_Intro.prefab:a7bfe727e3a77ef4faa4e1151a670e87",
      "VO_GILA_BOSS_21h_Rottooth_Emote.prefab:01a3a0ce7aa408347910735b9416dfde",
      "VO_GILA_BOSS_21h_Rottooth_Death.prefab:4e73b15125c45344088be9decbe0c07f"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_21h_Rottooth_Death.prefab:4e73b15125c45344088be9decbe0c07f";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_21h_Rottooth_Intro.prefab:a7bfe727e3a77ef4faa4e1151a670e87", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_21h_Rottooth_Emote.prefab:01a3a0ce7aa408347910735b9416dfde", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
