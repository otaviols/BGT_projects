using System.Collections;

public class BRM17_ZombieNef : BRM_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;
  private bool m_inOnyxiaState;
  private Actor m_nefActor;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA17_1_DEATHWING_88.prefab:525f210af61d16b49b3b20fba2c2cd8c");
    this.PreloadSound("VO_BRMA17_1_HERO_POWER_87.prefab:e0f77b0064ea8164e92e8982694d89a7");
    this.PreloadSound("VO_BRMA17_1_CARD_86.prefab:d433af8d96634ae42877ecfd242f93bb");
    this.PreloadSound("VO_BRMA17_1_RESPONSE_85.prefab:c7bbc928438b13241bde42c6578ad5c8");
    this.PreloadSound("VO_BRMA17_1_TURN1_79.prefab:d9c859c6074049d479898c0582940383");
    this.PreloadSound("VO_BRMA17_1_RESURRECT1_82.prefab:67b1bbccbff5d2249a0f00300daef60a");
    this.PreloadSound("VO_BRMA17_1_RESURRECT3_84.prefab:fc2708abf54774d43872254af96d4a6c");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR1_89.prefab:51e99dbc580c406499d55cf131b94d1e");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR2_90.prefab:fb528aa3456f4164a94f9ad0939bb055");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR3_91.prefab:6f790b300a69c3247b83a3e60042ec52");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR4_92.prefab:b2e088056ab3de043a5481de32fd5e8f");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR5_93.prefab:315cfc6364a60c246a3bec36b3fda2ba");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR6_94.prefab:218b8f33f696b194296f1a8c808e5659");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR7_95.prefab:91e8dbfaaf49fd04e93af907bbb61fd4");
    this.PreloadSound("VO_BRMA17_1_NEF_AIR8_96.prefab:25016d16acfda5e458cf4b18470528a0");
    this.PreloadSound("VO_BRMA17_1_TRANSFORM1_80.prefab:82475f6129d5587448c3aa398a77c580");
    this.PreloadSound("VO_BRMA17_1_TRANSFORM2_81.prefab:d064be3da78c0f5449db24a40f9a609b");
    this.PreloadSound("OnyxiaBoss_Start_1.prefab:572ad57bf5b75434b8243fe0c9b5b262");
    this.PreloadSound("OnyxiaBoss_Death_1.prefab:3b229c4926824824598302037ef1483a");
    this.PreloadSound("OnyxiaBoss_EmoteResponse_1.prefab:69d9315cbeeddd34b889fe59faa4c480");
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (emoteType)
    {
      case EmoteType.GREETINGS:
      case EmoteType.WELL_PLAYED:
      case EmoteType.OOPS:
      case EmoteType.THREATEN:
      case EmoteType.THANKS:
      case EmoteType.SORRY:
        string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
        if (cardId == "BRMA17_2" || cardId == "BRMA17_2H")
        {
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_BRMA17_1_RESPONSE_85.prefab:c7bbc928438b13241bde42c6578ad5c8", Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        }
        if (!(cardId == "BRMA17_3") && !(cardId == "BRMA17_3H"))
          break;
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("OnyxiaBoss_EmoteResponse_1.prefab:69d9315cbeeddd34b889fe59faa4c480", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM17_ZombieNef brM17ZombieNef = this;
    while (brM17ZombieNef.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "BRMA17_4"))
    {
      if ((cardId == "BRMA17_5" || cardId == "BRMA17_5H") && !brM17ZombieNef.m_heroPowerLinePlayed)
      {
        brM17ZombieNef.m_heroPowerLinePlayed = true;
        yield return (object) Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_HERO_POWER_87.prefab:e0f77b0064ea8164e92e8982694d89a7", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!brM17ZombieNef.m_cardLinePlayed && !brM17ZombieNef.m_inOnyxiaState)
    {
      brM17ZombieNef.m_cardLinePlayed = true;
      GameState.Get().SetBusy(true);
      yield return (object) Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_CARD_86.prefab:d433af8d96634ae42877ecfd242f93bb", Notification.SpeechBubbleDirection.TopRight, actor));
      GameState.Get().SetBusy(false);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM17_ZombieNef brM17ZombieNef = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 1)
    {
      brM17ZombieNef.m_nefActor = actor;
      Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_TURN1_79.prefab:d9c859c6074049d479898c0582940383", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    return false;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BRM17_ZombieNef brM17ZombieNef = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        brM17ZombieNef.m_inOnyxiaState = true;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_RESURRECT1_82.prefab:67b1bbccbff5d2249a0f00300daef60a", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        yield return (object) Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_RESURRECT3_84.prefab:fc2708abf54774d43872254af96d4a6c", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
        yield return (object) Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("OnyxiaBoss_Start_1.prefab:572ad57bf5b75434b8243fe0c9b5b262", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 3:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_DEATHWING_88.prefab:525f210af61d16b49b3b20fba2c2cd8c", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 4:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR1_89.prefab:51e99dbc580c406499d55cf131b94d1e", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 5:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR2_90.prefab:fb528aa3456f4164a94f9ad0939bb055", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 6:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR3_91.prefab:6f790b300a69c3247b83a3e60042ec52", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 7:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR4_92.prefab:b2e088056ab3de043a5481de32fd5e8f", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 8:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR5_93.prefab:315cfc6364a60c246a3bec36b3fda2ba", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 9:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR6_94.prefab:218b8f33f696b194296f1a8c808e5659", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 10:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR7_95.prefab:91e8dbfaaf49fd04e93af907bbb61fd4", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 11:
        Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR8_96.prefab:25016d16acfda5e458cf4b18470528a0", Notification.SpeechBubbleDirection.TopRight, brM17ZombieNef.m_nefActor));
        break;
      case 13:
        brM17ZombieNef.m_inOnyxiaState = false;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_TRANSFORM1_80.prefab:82475f6129d5587448c3aa398a77c580", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        break;
      case 14:
        while (brM17ZombieNef.m_enemySpeaking)
          yield return (object) null;
        yield return (object) Gameplay.Get().StartCoroutine(brM17ZombieNef.PlaySoundAndBlockSpeech("VO_BRMA17_1_TRANSFORM2_81.prefab:d064be3da78c0f5449db24a40f9a609b", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
    }
  }
}
