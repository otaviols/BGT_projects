using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_62h : GIL_Dungeon
{
  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_62h_GustaveTheGutRipper_Intro.prefab:79e90a27856ed7f41965b5180001fddf",
      "VO_GILA_BOSS_62h_GustaveTheGutRipper_Emote.prefab:75823bea3585ef44ab6cae97961d0634",
      "VO_GILA_BOSS_62h_GustaveTheGutRipper_Death.prefab:3440e77119ee33a4d9b018aef09f9411"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_62h_GustaveTheGutRipper_Death.prefab:3440e77119ee33a4d9b018aef09f9411";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_62h_GustaveTheGutRipper_Intro.prefab:79e90a27856ed7f41965b5180001fddf", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_62h_GustaveTheGutRipper_Emote.prefab:75823bea3585ef44ab6cae97961d0634", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
