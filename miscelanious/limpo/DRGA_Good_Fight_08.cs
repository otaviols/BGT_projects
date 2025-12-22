using System.Collections;
using System.Collections.Generic;

public class DRGA_Good_Fight_08 : DRGA_Dungeon
{
  private static readonly AssetReference VO_DRGA_BOSS_03h_Male_Murloc_Good_Fight_08_Victory_01 = new AssetReference("VO_DRGA_BOSS_03h_Male_Murloc_Good_Fight_08_Victory_01.prefab:4592b8df0e5dc8e4181c0e1cd9a908cb");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round1_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round1_01.prefab:922408d89638efe41b99343b9d945603");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round2_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round2_01.prefab:9aa0f0c617cc3894d80673068d200190");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DragonslayerGreatbow_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DragonslayerGreatbow_01.prefab:40b22d6704363aa44a961b846ef6c468");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DrawCard_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DrawCard_01.prefab:49a7684d76f21994ba5d18c554dca495");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Heartseeker_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Heartseeker_01.prefab:e80eefdf2e7a5f941948d58dfb2c91cd");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_01_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_01_01.prefab:97fc0167fd11ba6448e3077524996078");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_02_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_02_01.prefab:7c1c3ffc262648e459011cf156741014");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_03_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_03_01.prefab:36e7bbf9fc01e9f4b8539a2c85ab7bbc");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossAttack_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossAttack_01.prefab:dacfac7175aaa374691c9226827b6e98");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossStart_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossStart_01.prefab:b6df9f87a8ab3424e916d504b2cf9a70");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_EmoteResponse_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_EmoteResponse_01.prefab:24d104072c731834c90765784aa9ac13");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_01_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_01_01.prefab:b1acd087bba2a1f4884af74f8a3c824f");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_02_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_02_01.prefab:a9ba6eadaf6324641bccd665106cd2a6");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_03_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_03_01.prefab:7fec6c5f112ee884fac6232913ca710c");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_01_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_01_01.prefab:0ae0effc3066b9747b0943922a00fbd1");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_02_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_02_01.prefab:ad216649b44a83044966b6f92ca90897");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_03_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_03_01.prefab:3e5364c9c1c270840afd6313122267bb");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_04_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_04_01.prefab:cba75782ea131674c9442e103d8f4940");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_05_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_05_01.prefab:c1befb2ed5fb9f0479558efebff953d7");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_06_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_06_01.prefab:6c5a3b1050e63154cbed4c2a568b4d3e");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_DragonHero_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_DragonHero_01.prefab:af8e516d6757b0043884fa58cace2d9d");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Gruul_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Gruul_01.prefab:cd4fc6d5b2201454295a889928358ef8");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_NemsyHero_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_NemsyHero_01.prefab:a1ded04d3fec7b643af39c01e78af54c");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_RenoHero_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_RenoHero_01.prefab:71e57e35373546646913400f0485598a");
  private static readonly AssetReference VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Toki_01 = new AssetReference("VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Toki_01.prefab:3695445bfdba2d4438974859bc85b2d8");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_07_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_07_01.prefab:285cea29098d3df49b44a2d76fe878fc");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_08_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_08_01.prefab:73b28feb6dad5eb40a5f955e40f52479");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_09_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_09_01.prefab:416bb4f5279714d49bb09b5ecfbdad21");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_10_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_10_01.prefab:0b823edf48a6ec7498aeccb5b00443e9");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_11_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_11_01.prefab:1ea2737b4b9337f45b1226f40fe27567");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_12_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_12_01.prefab:d85b347383555074c90426cd37056612");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_13_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_13_01.prefab:7d513f8eda27c994a9dc733dac53066e");
  private static readonly AssetReference VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_14_01 = new AssetReference("VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_14_01.prefab:6183ebefc40fd9941997dae74d81310e");
  private List<string> m_VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPowerLines = new List<string>()
  {
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_01_01,
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_02_01,
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_03_01
  };
  private List<string> m_VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_IdleLines = new List<string>()
  {
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_01_01,
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_02_01,
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_03_01
  };
  private List<string> m_ChromieTransform = new List<string>()
  {
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_08_01,
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_09_01,
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_10_01
  };
  private List<string> m_BossEquipDragonslayer = new List<string>()
  {
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_05_01,
    (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DragonslayerGreatbow_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_03h_Male_Murloc_Good_Fight_08_Victory_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round1_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round2_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DragonslayerGreatbow_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DrawCard_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Heartseeker_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_01_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_02_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPower_03_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossAttack_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossStart_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_EmoteResponse_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_01_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_02_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Idle_03_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_01_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_02_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_03_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_04_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_05_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_06_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_DragonHero_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Gruul_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_NemsyHero_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_RenoHero_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Toki_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_07_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_08_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_09_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_10_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_11_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_12_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_13_01,
      (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_14_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round1_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "HERO_01c")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_DragonHero_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "NEW1_038")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Gruul_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "HERO_07a")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_NemsyHero_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "YOD_009")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_RenoHero_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Toki_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossStart_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DRGA_Good_Fight_08 drgaGoodFight08 = this;
    while (drgaGoodFight08.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_01_01);
        break;
      case 101:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_02_01);
        break;
      case 102:
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_03_01);
        break;
      case 103:
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_04_01);
        break;
      case 104:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_05_01);
        break;
      case 105:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Misc_06_01);
        break;
      case 106:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor2, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_07_01);
        drgaGoodFight08.m_deathLine = (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round2_01;
        break;
      case 107:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayAndRemoveRandomLineOnlyOnce(actor2, drgaGoodFight08.m_ChromieTransform);
        break;
      case 108:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineOnlyOnce(actor2, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Heartseeker_01);
        break;
      case 110:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor2, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_11_01);
        break;
      case 111:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor2, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_12_01);
        break;
      case 112:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor2, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_13_01);
        break;
      case 113:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways(actor2, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_36h_Female_Gnome_Good_Fight_08_Misc_14_01);
        break;
      case 114:
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round1_01);
        break;
      case 115:
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_Death_Round2_01);
        break;
      case 116:
        yield return (object) drgaGoodFight08.PlayLineAlways(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Boss_DrawCard_01);
        break;
      case 117:
        yield return (object) drgaGoodFight08.PlayLineOnlyOnce(actor1, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_BossAttack_01);
        break;
      case 118:
        if (drgaGoodFight08.m_Heroic)
          break;
        yield return (object) drgaGoodFight08.PlayLineAlways((string) DRGA_Dungeon.FinleyBrassRing, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_03h_Male_Murloc_Good_Fight_08_Victory_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) drgaGoodFight08.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DRGA_Good_Fight_08 drgaGoodFight08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) drgaGoodFight08.\u003C\u003En__1(entity);
    while (drgaGoodFight08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!drgaGoodFight08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) drgaGoodFight08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      drgaGoodFight08.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "NEW1_038")
        yield return (object) drgaGoodFight08.PlayLineOnlyOnce(actor, (string) DRGA_Good_Fight_08.VO_DRGA_BOSS_22h_Male_Gronn_Good_Fight_08_Player_Gruul_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DRGA_Good_Fight_08 drgaGoodFight08 = this;
    while (drgaGoodFight08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!drgaGoodFight08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) drgaGoodFight08.\u003C\u003En__2(entity);
      yield return (object) drgaGoodFight08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      drgaGoodFight08.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "DRGA_BOSS_22t" || cardId == "DRGA_BOSS_22t4")
        yield return (object) drgaGoodFight08.PlayAndRemoveRandomLineOnlyOnce(actor, drgaGoodFight08.m_BossEquipDragonslayer);
    }
  }
}
