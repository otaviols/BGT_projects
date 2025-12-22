using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_Blizzcon_2016 : MissionEntity
{
  private bool[] hasUsedLine;
  private int currentTurnsWOEmote;
  private int emoteTurnsLimit = 7;
  private bool emoteThisTurn;
  private List<int> priorityLines;
  private bool hasPlayedMatchupTriggerGoons;
  private bool hasPlayedMatchupTriggerLotus;
  private bool hasPlayedMatchupTriggerKabal;
  private TB_Blizzcon_2016.MATCHUP currentMatchup = TB_Blizzcon_2016.MATCHUP.ERROR;
  private TB_Blizzcon_2016.VICTOR matchResult;
  private TAG_CLASS lotusHero = TAG_CLASS.DRUID;
  private TAG_CLASS kabalHero = TAG_CLASS.PRIEST;
  private TAG_CLASS goonsHero = TAG_CLASS.PALADIN;
  private string grimyGoonsName = GameStrings.Get("GLOBAL_KEYWORD_GRIMY_GOONS");
  private string jadeLotusName = GameStrings.Get("GLOBAL_KEYWORD_JADE_LOTUS");
  private string kabalName = GameStrings.Get("GLOBAL_KEYWORD_KABAL");
  private TAG_CLASS firstPlayerHero;
  private TAG_CLASS secondPlayerHero;

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    if (this.currentMatchup == TB_Blizzcon_2016.MATCHUP.ERROR)
      this.currentMatchup = this.GetBrawlHeroes();
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        if (this.firstPlayerHero == this.goonsHero && this.secondPlayerHero == this.lotusHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.GOONSBEATLOTUS;
          break;
        }
        if (this.firstPlayerHero == this.goonsHero && this.secondPlayerHero == this.kabalHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.GOONSBEATKABAL;
          break;
        }
        if (this.firstPlayerHero == this.lotusHero && this.secondPlayerHero == this.kabalHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.LOTUSBEATKABAL;
          break;
        }
        if (this.firstPlayerHero == this.lotusHero && this.secondPlayerHero == this.goonsHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.LOTUSBEATGOONS;
          break;
        }
        if (this.firstPlayerHero == this.kabalHero && this.secondPlayerHero == this.lotusHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.KABALBEATLOTUS;
          break;
        }
        if (this.firstPlayerHero == this.kabalHero && this.secondPlayerHero == this.goonsHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.KABALBEATGOONS;
          break;
        }
        break;
      case TAG_PLAYSTATE.LOST:
        if (this.firstPlayerHero == this.goonsHero && this.secondPlayerHero == this.lotusHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.LOTUSBEATGOONS;
          break;
        }
        if (this.firstPlayerHero == this.goonsHero && this.secondPlayerHero == this.kabalHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.KABALBEATGOONS;
          break;
        }
        if (this.firstPlayerHero == this.lotusHero && this.secondPlayerHero == this.kabalHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.KABALBEATLOTUS;
          break;
        }
        if (this.firstPlayerHero == this.lotusHero && this.secondPlayerHero == this.goonsHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.GOONSBEATLOTUS;
          break;
        }
        if (this.firstPlayerHero == this.kabalHero && this.secondPlayerHero == this.lotusHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.LOTUSBEATKABAL;
          break;
        }
        if (this.firstPlayerHero == this.kabalHero && this.secondPlayerHero == this.goonsHero)
        {
          this.matchResult = TB_Blizzcon_2016.VICTOR.GOONSBEATKABAL;
          break;
        }
        break;
      case TAG_PLAYSTATE.TIED:
        this.matchResult = TB_Blizzcon_2016.VICTOR.ERROR;
        break;
    }
    base.NotifyOfGameOver(gameResult);
  }

  private TB_Blizzcon_2016.MATCHUP GetBrawlHeroes()
  {
    this.firstPlayerHero = GameState.Get().GetFriendlySidePlayer().GetHero().GetClass();
    this.secondPlayerHero = GameState.Get().GetOpposingSidePlayer().GetHero().GetClass();
    if (this.firstPlayerHero == this.goonsHero && this.secondPlayerHero == this.lotusHero || this.firstPlayerHero == this.lotusHero && this.secondPlayerHero == this.goonsHero)
      return TB_Blizzcon_2016.MATCHUP.GOONSVLOTUS;
    if (this.firstPlayerHero == this.goonsHero && this.secondPlayerHero == this.kabalHero || this.firstPlayerHero == this.kabalHero && this.secondPlayerHero == this.goonsHero)
      return TB_Blizzcon_2016.MATCHUP.KABALVGOONS;
    if (this.firstPlayerHero == this.kabalHero && this.secondPlayerHero == this.lotusHero || this.firstPlayerHero == this.lotusHero && this.secondPlayerHero == this.kabalHero)
      return TB_Blizzcon_2016.MATCHUP.KABALVLOTUS;
    Debug.LogError((object) "Matchup is not as predicted. Should be only one of each hero as defined in TB_Blizzcon_2016.cs");
    return TB_Blizzcon_2016.MATCHUP.ERROR;
  }

  public override AudioSource GetAnnouncerLine(
    Card heroCard,
    Card.AnnouncerLineType type)
  {
    int num = Random.Range(0, 3);
    if (heroCard.GetEntity().GetClass() == this.lotusHero)
    {
      switch (num)
      {
        case 0:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_JadeLotus_Intro_01.prefab:6bc8a6bd85078984db14131a67029b04");
        case 1:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_JadeLotus_Intro_02.prefab:c5c53500c23c1f744bdca5c5a3cbdc04");
        case 2:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_JadeLotus_Intro_03.prefab:e98c84c7bd6010147a3c13956a6e0ece");
      }
    }
    else if (heroCard.GetEntity().GetClass() == this.goonsHero)
    {
      switch (num)
      {
        case 0:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_GrimyGoons_Intro_01.prefab:24bae69ce396c7947b1018b1976de679");
        case 1:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_GrimyGoons_Intro_02.prefab:ab33516e41188c248ae90f6cb17067fa");
        case 2:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_GrimyGoons_Intro_03.prefab:91ddf3c0c5e8d9a4ea1089fa346e5518");
      }
    }
    else if (heroCard.GetEntity().GetClass() == this.kabalHero)
    {
      switch (num)
      {
        case 0:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_TheKabal_Intro_01.prefab:ea1633a8b77d5fb4888b9214519c12ec");
        case 1:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_TheKabal_Intro_02.prefab:c4ea297c7eca6874c874b35aa7f2a018");
        case 2:
          return this.GetPreloadedSound("VO_INKEEPER_Male_Dwarf_TheKabal_Intro_03.prefab:e43f5a1cc60008940a77161a3c635594");
      }
    }
    return base.GetAnnouncerLine(heroCard, type);
  }

  public override void PreloadAssets()
  {
    this.hasUsedLine = new bool[1000];
    this.SetupPriorityLines();
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_1st_Turn_Start_01.prefab:8be17b0dae8da254eb58a9666a008b63");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_2nd_Turn_Start_01.prefab:e5a44c5a2dfa9434a964bcc7da4b0d75");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_Fill1_01.prefab:4be280fcdd3c5594aaf9125ab43be6b6");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_Goons_Win_01.prefab:190589e48de533a42b90b5e170ce326d");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_Goons_Winning_01.prefab:4ee560db3543f9f43a6c881f28d6e854");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_Kabal_Win_01.prefab:c94334b89fcf4ea4e81d0ec930e1d051");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_Kabal_Winning_01.prefab:7f8845179eedd5c4bac313cddef8bbe9");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_1st_Turn_Start_01.prefab:d686738dd65850e44a48e7c443c25263");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Fill1_01.prefab:c9cf61ce3a401064e97ccf4fafa3cff5");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Fill2_01.prefab:2e806195d67655141a710924c5016991");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Kabal_Winning_01.prefab:8e0079b4e73c2554d802a0ac39ba45d7");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Kabal_Winning_02.prefab:7cbbc6dbc527f994ea54a996ae75d525");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Lotus_Winning_01.prefab:6c63549c3cb661d46aa47ea970217c61");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Lotus_Wins_01.prefab:422826db6426bda4783fc3a36d113ee8");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Kabal_Wins_01.prefab:5d9ad80e62f78404daab599afb8bdc60");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Potion1_01.prefab:1a2ad0f6911ab214db7de81137341307");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_01.prefab:6e0fe3ddd39cd424caf7369da719cd7c");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_02.prefab:4add1f34caa4c6841ae5687bc9f9a5a7");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_04.prefab:ac2104b0a0ece5240a320b4ba0968768");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_05.prefab:5bd45b4a07027974da3ad4a4b083a4b2");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Jade_Golem3_01.prefab:162f176395aab6b4aaf471fd549d5b40");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_3_Jade_Golem2_01.prefab:92610ca6cb9ab2c4fbc8d64a77b7fa5e");
    this.PreloadSound("VO_BOSS_KAZAKUS_Male_Troll_BC_1_Arms_Dealing1_01.prefab:b93994bc7cd20b64caf25f092eb3e765");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_1st_Turn_Start_01.prefab:854d26b8a7627264ba091c0b632258f2");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_2nd_Turn_Start_01.prefab:81a12a2b061dfb149a8f2162dd9dd251");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_Fill1_01.prefab:40a180e56c205c24fb70dd84c728f801");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_Goons_Win_01.prefab:f59689a83886afb4aab053e6aa91ff3c");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_Goons_Winning_02.prefab:1eb57a70b5a6f194c9b772c4c925da39");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_Jade_Win_01.prefab:535dd787d222bc54e8c0372766751823");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_Lotus_Winning_01.prefab:3512d5e4b0171f2499a5ac93d747af49");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_1st_Turn_Start_01.prefab:19367564a9bc4a14da1e9d2d2d06123d");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_1st_Turn_Start_02.prefab:de509aa2f0a49a243b9d23d70fb13168");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_2nd_Turn_Start_01.prefab:2ccf4699aa4d28e45b8c95ff79bf52f9");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Fill1_01.prefab:595b4d84ba3e55848a54388b450d40e1");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Fill2_01.prefab:f2c96720bb2daab4b87fcb8f1c663ae9");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Kabal_Winning_01.prefab:e921d0ea7f239b74d8bbda8a77a7d6f6");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Kabal_Wins_01.prefab:796da1c6fde86d046ba242430f5a7be4");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Lotus_Winning_01.prefab:8309a83a5893c4145bd3d4fec40af94a");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Lotus_Wins_01.prefab:ecdd497a01ea5f04fb369ec6e57efd76");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Potion1_02.prefab:e9693fec0b2bb894d8de8c7a2d63b9aa");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_0_Jade_Golem_02.prefab:da9bc8ef3fcddeb468c96ccf75ffc8e1");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_0_Jade_Golem_03.prefab:24ffb25cc6e6fe340afd95ddfb992823");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_0_Jade_Golem_04.prefab:32983d85d0e8f3e4fb4836444f320c60");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_3_Jade_Golem3_01.prefab:a21d7215fef07cf4a870b6ec136c68f6");
    this.PreloadSound("VO_BOSS_AYA_Female_Pandaren_BC_2_Arms_Dealing1_01.prefab:9388a49095971ee499e925c1385093b9");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_1st_Turn_Start_01.prefab:023f51537ec053c48b06d3003134c0a9");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_2nd_Turn_Start_02.prefab:fc8234cf7c6471a45b3f21e4c162ef2c");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_Fill1_01.prefab:a6d42874b7ada1246883d6e150bcfcd4");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_Fill2_01.prefab:3a38dd655d3d83542a6303ebbf5bd553");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_Goons_Win_01.prefab:88e0a1afa65e143488d60a26451a54fe");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_Goons_Winning_01.prefab:8e5e4d9dc062c0248bf4a31346c17817");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_Kabal_Win_02.prefab:ebc7a428c00acbf48802392c8af54462");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_Kabal_Winning_02.prefab:df4ff489a629a2642a6842494a6c2692");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_1st_Turn_Start_01.prefab:3bd5274c23af23c42ab9a2dc3de1bfc2");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_1st_Turn_Start_04.prefab:afdb7eef29038ce41802afe04f44371f");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_Fill1_01.prefab:6bd403211b76afd448ad6eeb4fb0e9c8");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_Goons_Win_01.prefab:dad027263619ad24ebab9d8ffec68436");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_Goons_Winning_01.prefab:26c3e3ecb0645bf43a882631bc1d69a2");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_Lotus_Win_01.prefab:147a1660b03fa0c4dba706ba274c420e");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_Lotus_Winning_01.prefab:ab969d07a5b38ad45b6adf5d0ebd862e");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_1_Potion2_01.prefab:f2473e5ceda5f5b42b0bcdd80f352ea8");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_2_Jade_Golem2_01.prefab:65b6678983cd69a49b341ec8ace0a72c");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_01.prefab:bdb1e3559c2b97c4bb9cdb78ebf19139");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_02.prefab:bf052509477065a48a760a5233513894");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_03.prefab:e0c61e1383fa32644ba6651b2ed842e1");
    this.PreloadSound("VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_04.prefab:aa45ac942e6197142a76054d72014a59");
    this.PreloadSound("VO_BOSS_CHO_Male_Ogre_BC_1_1st_Turn_Start_01.prefab:cb463e997b8fafe48941c06e749d30bd");
    this.PreloadSound("VO_BOSS_CHO_Male_Ogre_BC_1_Fill2_01.prefab:cfb6da4eb23272f40be8d9b2220ddb11");
    this.PreloadSound("VO_BOSS_CHO_Male_Ogre_BC_2_Fill1_01.prefab:e61f0b71bb9160547a872e8aa866486c");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_GrimyGoons_Intro_01.prefab:24bae69ce396c7947b1018b1976de679");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_GrimyGoons_Intro_02.prefab:ab33516e41188c248ae90f6cb17067fa");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_GrimyGoons_Intro_03.prefab:91ddf3c0c5e8d9a4ea1089fa346e5518");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_JadeLotus_Intro_01.prefab:6bc8a6bd85078984db14131a67029b04");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_JadeLotus_Intro_02.prefab:c5c53500c23c1f744bdca5c5a3cbdc04");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_JadeLotus_Intro_03.prefab:e98c84c7bd6010147a3c13956a6e0ece");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_TheKabal_Intro_01.prefab:ea1633a8b77d5fb4888b9214519c12ec");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_TheKabal_Intro_02.prefab:c4ea297c7eca6874c874b35aa7f2a018");
    this.PreloadSound("VO_INKEEPER_Male_Dwarf_TheKabal_Intro_03.prefab:e43f5a1cc60008940a77161a3c635594");
  }

  private void SetupPriorityLines()
  {
    this.priorityLines = new List<int>();
    this.priorityLines.Add(0);
    this.priorityLines.Add(102);
    this.priorityLines.Add(103);
    this.priorityLines.Add(202);
    this.priorityLines.Add(203);
    this.priorityLines.Add(302);
    this.priorityLines.Add(303);
  }

  private Vector3 GetPositionForBoss(TB_Blizzcon_2016.BOSS boss)
  {
    this.firstPlayerHero = GameState.Get().GetFriendlySidePlayer().GetHero().GetClass();
    switch (boss)
    {
      case TB_Blizzcon_2016.BOSS.HAN:
      case TB_Blizzcon_2016.BOSS.CHO:
        return this.firstPlayerHero == this.goonsHero ? NotificationManager.LEFT_OF_FRIENDLY_HERO : NotificationManager.RIGHT_OF_ENEMY_HERO;
      case TB_Blizzcon_2016.BOSS.AYA:
        return this.firstPlayerHero == this.lotusHero ? NotificationManager.LEFT_OF_FRIENDLY_HERO : NotificationManager.RIGHT_OF_ENEMY_HERO;
      case TB_Blizzcon_2016.BOSS.KAZAKUS:
        return this.firstPlayerHero == this.kabalHero ? NotificationManager.LEFT_OF_FRIENDLY_HERO : NotificationManager.RIGHT_OF_ENEMY_HERO;
      default:
        return NotificationManager.DEFAULT_CHARACTER_POS;
    }
  }

  private Notification.SpeechBubbleDirection GetBubbleDirectionForBoss(
    TB_Blizzcon_2016.BOSS boss)
  {
    this.firstPlayerHero = GameState.Get().GetFriendlySidePlayer().GetHero().GetClass();
    switch (boss)
    {
      case TB_Blizzcon_2016.BOSS.HAN:
        return this.firstPlayerHero == this.goonsHero ? Notification.SpeechBubbleDirection.BottomRight : Notification.SpeechBubbleDirection.TopRight;
      case TB_Blizzcon_2016.BOSS.CHO:
        return this.firstPlayerHero == this.goonsHero ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopLeft;
      case TB_Blizzcon_2016.BOSS.AYA:
        return this.firstPlayerHero == this.lotusHero ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopLeft;
      case TB_Blizzcon_2016.BOSS.KAZAKUS:
        return this.firstPlayerHero == this.kabalHero ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopLeft;
      default:
        return Notification.SpeechBubbleDirection.BottomLeft;
    }
  }

  private IEnumerator PlayBossLine(
    TB_Blizzcon_2016.BOSS boss,
    string line,
    bool persistCharacter = false)
  {
    TB_Blizzcon_2016 tbBlizzcon2016 = this;
    Notification.SpeechBubbleDirection directionForBoss = tbBlizzcon2016.GetBubbleDirectionForBoss(boss);
    Vector3 positionForBoss = tbBlizzcon2016.GetPositionForBoss(boss);
    switch (boss)
    {
      case TB_Blizzcon_2016.BOSS.HAN:
      case TB_Blizzcon_2016.BOSS.CHO:
        yield return (object) tbBlizzcon2016.PlayMissionFlavorLine("HanCho_Temp_BigQuote.prefab:7a7804f8f47064946bdbcfd3b78d0dac", line, positionForBoss, directionForBoss, persistCharacter: persistCharacter);
        break;
      case TB_Blizzcon_2016.BOSS.AYA:
        yield return (object) tbBlizzcon2016.PlayMissionFlavorLine("Aya_Temp_BigQuote.prefab:faa6811234b5e2e40b2447c7878616fe", line, positionForBoss, directionForBoss, persistCharacter: persistCharacter);
        break;
      case TB_Blizzcon_2016.BOSS.KAZAKUS:
        yield return (object) tbBlizzcon2016.PlayMissionFlavorLine("Kazakus_Temp_BigQuote.prefab:9d330c5f45374254181cb923722b973d", line, positionForBoss, directionForBoss, persistCharacter: persistCharacter);
        break;
    }
    tbBlizzcon2016.emoteThisTurn = true;
    tbBlizzcon2016.currentTurnsWOEmote = 0;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_Blizzcon_2016 tbBlizzcon2016 = this;
    while (tbBlizzcon2016.m_enemySpeaking)
      yield return (object) null;
    if (tbBlizzcon2016.currentMatchup == TB_Blizzcon_2016.MATCHUP.ERROR)
      tbBlizzcon2016.currentMatchup = tbBlizzcon2016.GetBrawlHeroes();
    if ((!tbBlizzcon2016.emoteThisTurn || tbBlizzcon2016.priorityLines.Contains(missionEvent)) && missionEvent != 1)
    {
      switch (missionEvent)
      {
        case 0:
          ++tbBlizzcon2016.currentTurnsWOEmote;
          Debug.Log((object) tbBlizzcon2016.currentTurnsWOEmote);
          tbBlizzcon2016.emoteThisTurn = false;
          if (tbBlizzcon2016.currentTurnsWOEmote < tbBlizzcon2016.emoteTurnsLimit)
            break;
          Gameplay.Get().StartCoroutine(tbBlizzcon2016.PlayFillLine());
          break;
        case 100:
          if (tbBlizzcon2016.hasUsedLine[100])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_1st_Turn_Start_01.prefab:8be17b0dae8da254eb58a9666a008b63");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_1st_Turn_Start_01.prefab:023f51537ec053c48b06d3003134c0a9", true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.CHO, "VO_BOSS_CHO_Male_Ogre_BC_1_1st_Turn_Start_01.prefab:cb463e997b8fafe48941c06e749d30bd");
          GameState.Get().SetBusy(false);
          tbBlizzcon2016.hasUsedLine[100] = true;
          break;
        case 101:
          if (tbBlizzcon2016.hasUsedLine[101])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_2nd_Turn_Start_01.prefab:e5a44c5a2dfa9434a964bcc7da4b0d75");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_2nd_Turn_Start_02.prefab:fc8234cf7c6471a45b3f21e4c162ef2c");
          GameState.Get().SetBusy(false);
          tbBlizzcon2016.hasUsedLine[101] = true;
          break;
        case 102:
          if (tbBlizzcon2016.hasUsedLine[102])
            break;
          tbBlizzcon2016.matchResult = TB_Blizzcon_2016.VICTOR.GOONSBEATKABAL;
          break;
        case 103:
          if (tbBlizzcon2016.hasUsedLine[103])
            break;
          tbBlizzcon2016.matchResult = TB_Blizzcon_2016.VICTOR.KABALBEATGOONS;
          break;
        case 104:
          if (tbBlizzcon2016.hasUsedLine[104])
            break;
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Goons_Winning_01.prefab:8e5e4d9dc062c0248bf4a31346c17817");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_Goons_Winning_01.prefab:4ee560db3543f9f43a6c881f28d6e854");
          tbBlizzcon2016.hasUsedLine[104] = true;
          break;
        case 105:
          if (tbBlizzcon2016.hasUsedLine[105])
            break;
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_Kabal_Winning_01.prefab:7f8845179eedd5c4bac313cddef8bbe9");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Kabal_Winning_02.prefab:df4ff489a629a2642a6842494a6c2692");
          tbBlizzcon2016.hasUsedLine[105] = true;
          break;
        case 150:
          int num1 = Random.Range(0, 4);
          if (!tbBlizzcon2016.hasPlayedMatchupTriggerKabal)
            num1 = Random.Range(0, 7);
          Debug.Log((object) ("Potion Trigger. Random value = " + (object) num1));
          GameState.Get().SetBusy(true);
          switch (num1)
          {
            case 0:
              if (!tbBlizzcon2016.hasUsedLine[150])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_01.prefab:6e0fe3ddd39cd424caf7369da719cd7c");
                tbBlizzcon2016.hasUsedLine[150] = true;
                break;
              }
              break;
            case 1:
              if (!tbBlizzcon2016.hasUsedLine[151])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_02.prefab:4add1f34caa4c6841ae5687bc9f9a5a7");
                tbBlizzcon2016.hasUsedLine[151] = true;
                break;
              }
              break;
            case 2:
              if (!tbBlizzcon2016.hasUsedLine[152])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_04.prefab:ac2104b0a0ece5240a320b4ba0968768");
                tbBlizzcon2016.hasUsedLine[152] = true;
                break;
              }
              break;
            case 3:
              if (!tbBlizzcon2016.hasUsedLine[153])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_0_Potion_05.prefab:5bd45b4a07027974da3ad4a4b083a4b2");
                tbBlizzcon2016.hasUsedLine[153] = true;
                break;
              }
              break;
            case 4:
            case 5:
            case 6:
            case 7:
              if (tbBlizzcon2016.currentMatchup == TB_Blizzcon_2016.MATCHUP.KABALVGOONS)
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Potion2_01.prefab:f2473e5ceda5f5b42b0bcdd80f352ea8");
                tbBlizzcon2016.hasPlayedMatchupTriggerKabal = true;
                break;
              }
              if (tbBlizzcon2016.currentMatchup == TB_Blizzcon_2016.MATCHUP.KABALVLOTUS)
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Potion1_01.prefab:1a2ad0f6911ab214db7de81137341307");
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Potion1_02.prefab:e9693fec0b2bb894d8de8c7a2d63b9aa");
                tbBlizzcon2016.hasPlayedMatchupTriggerKabal = true;
                break;
              }
              break;
          }
          GameState.Get().SetBusy(false);
          break;
        case 160:
          int num2 = Random.Range(0, 2);
          GameState.Get().SetBusy(true);
          if (num2 == 1 && !tbBlizzcon2016.hasUsedLine[161])
          {
            yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Potion2_01.prefab:f2473e5ceda5f5b42b0bcdd80f352ea8");
            tbBlizzcon2016.hasUsedLine[161] = true;
          }
          GameState.Get().SetBusy(false);
          break;
        case 170:
          int num3 = Random.Range(0, 2);
          GameState.Get().SetBusy(true);
          if (num3 == 0 && !tbBlizzcon2016.hasUsedLine[170])
          {
            yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Potion1_01.prefab:1a2ad0f6911ab214db7de81137341307");
            yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Potion1_02.prefab:e9693fec0b2bb894d8de8c7a2d63b9aa");
            tbBlizzcon2016.hasUsedLine[170] = true;
          }
          GameState.Get().SetBusy(false);
          break;
        case 200:
          if (tbBlizzcon2016.hasUsedLine[200])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_1st_Turn_Start_01.prefab:19367564a9bc4a14da1e9d2d2d06123d");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_1st_Turn_Start_01.prefab:d686738dd65850e44a48e7c443c25263");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_1st_Turn_Start_02.prefab:de509aa2f0a49a243b9d23d70fb13168");
          GameState.Get().SetBusy(false);
          tbBlizzcon2016.hasUsedLine[200] = true;
          break;
        case 201:
          if (tbBlizzcon2016.hasUsedLine[201])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Jade_Golem2_01.prefab:92610ca6cb9ab2c4fbc8d64a77b7fa5e");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_2nd_Turn_Start_01.prefab:2ccf4699aa4d28e45b8c95ff79bf52f9");
          GameState.Get().SetBusy(false);
          tbBlizzcon2016.hasUsedLine[201] = true;
          break;
        case 202:
          if (tbBlizzcon2016.hasUsedLine[202])
            break;
          tbBlizzcon2016.matchResult = TB_Blizzcon_2016.VICTOR.KABALBEATLOTUS;
          break;
        case 203:
          if (tbBlizzcon2016.hasUsedLine[203])
            break;
          tbBlizzcon2016.matchResult = TB_Blizzcon_2016.VICTOR.LOTUSBEATKABAL;
          break;
        case 204:
          if (tbBlizzcon2016.hasUsedLine[204])
            break;
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Kabal_Winning_01.prefab:8e0079b4e73c2554d802a0ac39ba45d7");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Kabal_Winning_01.prefab:e921d0ea7f239b74d8bbda8a77a7d6f6");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Kabal_Winning_02.prefab:7cbbc6dbc527f994ea54a996ae75d525");
          tbBlizzcon2016.hasUsedLine[204] = true;
          break;
        case 205:
          if (tbBlizzcon2016.hasUsedLine[205])
            break;
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Lotus_Winning_01.prefab:8309a83a5893c4145bd3d4fec40af94a");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Lotus_Winning_01.prefab:6c63549c3cb661d46aa47ea970217c61");
          tbBlizzcon2016.hasUsedLine[205] = true;
          break;
        case 250:
          int num4 = Random.Range(0, 3);
          if (!tbBlizzcon2016.hasPlayedMatchupTriggerLotus)
            num4 = Random.Range(0, 6);
          Debug.Log((object) ("Jade Golem Trigger. Random value = " + (object) num4));
          GameState.Get().SetBusy(true);
          switch (num4)
          {
            case 0:
              if (!tbBlizzcon2016.hasUsedLine[250])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_0_Jade_Golem_02.prefab:da9bc8ef3fcddeb468c96ccf75ffc8e1");
                tbBlizzcon2016.hasUsedLine[250] = true;
                break;
              }
              break;
            case 1:
              if (!tbBlizzcon2016.hasUsedLine[251])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_0_Jade_Golem_03.prefab:24ffb25cc6e6fe340afd95ddfb992823");
                tbBlizzcon2016.hasUsedLine[251] = true;
                break;
              }
              break;
            case 2:
              if (!tbBlizzcon2016.hasUsedLine[252])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_0_Jade_Golem_04.prefab:32983d85d0e8f3e4fb4836444f320c60");
                tbBlizzcon2016.hasUsedLine[252] = true;
                break;
              }
              break;
            case 3:
            case 4:
            case 5:
              if (tbBlizzcon2016.currentMatchup == TB_Blizzcon_2016.MATCHUP.GOONSVLOTUS)
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_Jade_Golem2_01.prefab:65b6678983cd69a49b341ec8ace0a72c");
                tbBlizzcon2016.hasPlayedMatchupTriggerLotus = true;
                break;
              }
              if (tbBlizzcon2016.currentMatchup == TB_Blizzcon_2016.MATCHUP.KABALVLOTUS)
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Jade_Golem3_01.prefab:162f176395aab6b4aaf471fd549d5b40");
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Jade_Golem3_01.prefab:a21d7215fef07cf4a870b6ec136c68f6");
                tbBlizzcon2016.hasPlayedMatchupTriggerLotus = true;
                break;
              }
              break;
          }
          GameState.Get().SetBusy(false);
          break;
        case 300:
          if (tbBlizzcon2016.hasUsedLine[300])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_1st_Turn_Start_01.prefab:3bd5274c23af23c42ab9a2dc3de1bfc2");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_1st_Turn_Start_01.prefab:854d26b8a7627264ba091c0b632258f2");
          GameState.Get().SetBusy(false);
          tbBlizzcon2016.hasUsedLine[300] = true;
          break;
        case 301:
          if (tbBlizzcon2016.hasUsedLine[301])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_2nd_Turn_Start_01.prefab:81a12a2b061dfb149a8f2162dd9dd251");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_1st_Turn_Start_04.prefab:afdb7eef29038ce41802afe04f44371f");
          GameState.Get().SetBusy(false);
          tbBlizzcon2016.hasUsedLine[301] = true;
          break;
        case 302:
          if (tbBlizzcon2016.hasUsedLine[302])
            break;
          tbBlizzcon2016.matchResult = TB_Blizzcon_2016.VICTOR.LOTUSBEATGOONS;
          break;
        case 303:
          if (tbBlizzcon2016.hasUsedLine[303])
            break;
          tbBlizzcon2016.matchResult = TB_Blizzcon_2016.VICTOR.GOONSBEATLOTUS;
          break;
        case 304:
          if (tbBlizzcon2016.hasUsedLine[304])
            break;
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_Lotus_Winning_01.prefab:3512d5e4b0171f2499a5ac93d747af49");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_Lotus_Winning_01.prefab:ab969d07a5b38ad45b6adf5d0ebd862e");
          tbBlizzcon2016.hasUsedLine[304] = true;
          break;
        case 305:
          if (tbBlizzcon2016.hasUsedLine[305])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_Goons_Winning_01.prefab:26c3e3ecb0645bf43a882631bc1d69a2");
          yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_Goons_Winning_02.prefab:1eb57a70b5a6f194c9b772c4c925da39");
          GameState.Get().SetBusy(false);
          tbBlizzcon2016.hasUsedLine[305] = true;
          break;
        case 350:
          int num5 = Random.Range(0, 4);
          if (!tbBlizzcon2016.hasPlayedMatchupTriggerGoons)
            num5 = Random.Range(0, 8);
          Debug.Log((object) ("Arms Dealing Trigger. Random value = " + (object) num5));
          GameState.Get().SetBusy(true);
          switch (num5)
          {
            case 0:
              if (!tbBlizzcon2016.hasUsedLine[350])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_01.prefab:bdb1e3559c2b97c4bb9cdb78ebf19139");
                tbBlizzcon2016.hasUsedLine[350] = true;
                break;
              }
              break;
            case 1:
              if (!tbBlizzcon2016.hasUsedLine[351])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_02.prefab:bf052509477065a48a760a5233513894");
                tbBlizzcon2016.hasUsedLine[351] = true;
                break;
              }
              break;
            case 2:
              if (!tbBlizzcon2016.hasUsedLine[352])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_03.prefab:e0c61e1383fa32644ba6651b2ed842e1");
                tbBlizzcon2016.hasUsedLine[352] = true;
                break;
              }
              break;
            case 3:
              if (!tbBlizzcon2016.hasUsedLine[353])
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_0_Arms_Dealing_04.prefab:aa45ac942e6197142a76054d72014a59");
                tbBlizzcon2016.hasUsedLine[353] = true;
                break;
              }
              break;
            case 4:
            case 5:
            case 6:
            case 7:
              if (tbBlizzcon2016.currentMatchup == TB_Blizzcon_2016.MATCHUP.GOONSVLOTUS)
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_Arms_Dealing1_01.prefab:9388a49095971ee499e925c1385093b9");
                tbBlizzcon2016.hasPlayedMatchupTriggerGoons = true;
                break;
              }
              if (tbBlizzcon2016.currentMatchup == TB_Blizzcon_2016.MATCHUP.KABALVGOONS)
              {
                yield return (object) tbBlizzcon2016.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_Arms_Dealing1_01.prefab:b93994bc7cd20b64caf25f092eb3e765");
                tbBlizzcon2016.hasPlayedMatchupTriggerGoons = true;
                break;
              }
              break;
          }
          GameState.Get().SetBusy(false);
          break;
        case 999:
          tbBlizzcon2016.SetNamePlate();
          break;
      }
    }
  }

  private void SetNamePlate()
  {
    TAG_CLASS tagClass1 = GameState.Get().GetFriendlySidePlayer().GetHero().GetClass();
    TAG_CLASS tagClass2 = GameState.Get().GetOpposingSidePlayer().GetHero().GetClass();
    switch (tagClass1)
    {
      case TAG_CLASS.DRUID:
        Gameplay.Get().GetNameBannerForSide(Player.Side.FRIENDLY).SetName(this.jadeLotusName);
        break;
      case TAG_CLASS.PALADIN:
        Gameplay.Get().GetNameBannerForSide(Player.Side.FRIENDLY).SetName(this.grimyGoonsName);
        break;
      case TAG_CLASS.PRIEST:
        Gameplay.Get().GetNameBannerForSide(Player.Side.FRIENDLY).SetName(this.kabalName);
        break;
      default:
        Debug.Log((object) "Incorrect class found in SetNamePlate()");
        break;
    }
    switch (tagClass2)
    {
      case TAG_CLASS.DRUID:
        Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName(this.jadeLotusName);
        break;
      case TAG_CLASS.PALADIN:
        Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName(this.grimyGoonsName);
        break;
      case TAG_CLASS.PRIEST:
        Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName(this.kabalName);
        break;
      default:
        Debug.Log((object) "Incorrect class found in SetNamePlate()");
        break;
    }
  }

  private IEnumerator PlayFillLine()
  {
    if (this.hasUsedLine[101] || this.hasUsedLine[201] || this.hasUsedLine[301])
    {
      bool flag = (double) Random.value < 0.5;
      switch (this.currentMatchup)
      {
        case TB_Blizzcon_2016.MATCHUP.KABALVLOTUS:
          if (!this.hasUsedLine[80] & flag)
          {
            GameState.Get().SetBusy(true);
            yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Fill2_01.prefab:2e806195d67655141a710924c5016991");
            yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Fill2_01.prefab:f2c96720bb2daab4b87fcb8f1c663ae9");
            GameState.Get().SetBusy(false);
            this.hasUsedLine[80] = true;
            break;
          }
          if (this.hasUsedLine[81])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Fill1_01.prefab:595b4d84ba3e55848a54388b450d40e1");
          yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Fill1_01.prefab:c9cf61ce3a401064e97ccf4fafa3cff5");
          GameState.Get().SetBusy(false);
          this.hasUsedLine[81] = true;
          break;
        case TB_Blizzcon_2016.MATCHUP.KABALVGOONS:
          if (!this.hasUsedLine[70] & flag)
          {
            GameState.Get().SetBusy(true);
            yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_Fill1_01.prefab:4be280fcdd3c5594aaf9125ab43be6b6");
            yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Fill1_01.prefab:a6d42874b7ada1246883d6e150bcfcd4");
            GameState.Get().SetBusy(false);
            this.hasUsedLine[70] = true;
            break;
          }
          if (this.hasUsedLine[71])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Fill2_01.prefab:3a38dd655d3d83542a6303ebbf5bd553", true);
          yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.CHO, "VO_BOSS_CHO_Male_Ogre_BC_1_Fill2_01.prefab:cfb6da4eb23272f40be8d9b2220ddb11");
          GameState.Get().SetBusy(false);
          this.hasUsedLine[71] = true;
          break;
        case TB_Blizzcon_2016.MATCHUP.GOONSVLOTUS:
          if (this.hasUsedLine[90])
            break;
          GameState.Get().SetBusy(true);
          yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_Fill1_01.prefab:40a180e56c205c24fb70dd84c728f801");
          yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_Fill1_01.prefab:6bd403211b76afd448ad6eeb4fb0e9c8", true);
          yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.CHO, "VO_BOSS_CHO_Male_Ogre_BC_2_Fill1_01.prefab:e61f0b71bb9160547a872e8aa866486c");
          GameState.Get().SetBusy(false);
          this.hasUsedLine[90] = true;
          break;
      }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    yield return (object) new WaitForSeconds(2f);
    switch (this.matchResult)
    {
      case TB_Blizzcon_2016.VICTOR.GOONSBEATKABAL:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_Goons_Win_01.prefab:190589e48de533a42b90b5e170ce326d");
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Goons_Win_01.prefab:88e0a1afa65e143488d60a26451a54fe");
        GameState.Get().SetBusy(false);
        this.hasUsedLine[102] = true;
        break;
      case TB_Blizzcon_2016.VICTOR.GOONSBEATLOTUS:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_Goons_Win_01.prefab:f59689a83886afb4aab053e6aa91ff3c");
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_Goons_Win_01.prefab:dad027263619ad24ebab9d8ffec68436");
        GameState.Get().SetBusy(false);
        this.hasUsedLine[303] = true;
        break;
      case TB_Blizzcon_2016.VICTOR.LOTUSBEATKABAL:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Lotus_Wins_01.prefab:ecdd497a01ea5f04fb369ec6e57efd76");
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Lotus_Wins_01.prefab:422826db6426bda4783fc3a36d113ee8");
        GameState.Get().SetBusy(false);
        this.hasUsedLine[203] = true;
        break;
      case TB_Blizzcon_2016.VICTOR.LOTUSBEATGOONS:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_2_Jade_Win_01.prefab:535dd787d222bc54e8c0372766751823");
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_2_Lotus_Win_01.prefab:147a1660b03fa0c4dba706ba274c420e");
        GameState.Get().SetBusy(false);
        this.hasUsedLine[302] = true;
        break;
      case TB_Blizzcon_2016.VICTOR.KABALBEATGOONS:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.HAN, "VO_BOSS_HAN_Male_Ogre_BC_1_Kabal_Win_02.prefab:ebc7a428c00acbf48802392c8af54462");
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_1_Kabal_Win_01.prefab:c94334b89fcf4ea4e81d0ec930e1d051");
        GameState.Get().SetBusy(false);
        this.hasUsedLine[103] = true;
        break;
      case TB_Blizzcon_2016.VICTOR.KABALBEATLOTUS:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.KAZAKUS, "VO_BOSS_KAZAKUS_Male_Troll_BC_3_Kabal_Wins_01.prefab:5d9ad80e62f78404daab599afb8bdc60");
        yield return (object) this.PlayBossLine(TB_Blizzcon_2016.BOSS.AYA, "VO_BOSS_AYA_Female_Pandaren_BC_3_Kabal_Wins_01.prefab:796da1c6fde86d046ba242430f5a7be4");
        GameState.Get().SetBusy(false);
        this.hasUsedLine[202] = true;
        break;
    }
  }

  public TB_Blizzcon_2016()
    : base()
  {
  }

  private enum MATCHUP
  {
    KABALVLOTUS,
    KABALVGOONS,
    GOONSVLOTUS,
    ERROR,
  }

  private enum BOSS
  {
    HAN,
    CHO,
    AYA,
    KAZAKUS,
  }

  private enum VICTOR
  {
    GOONSBEATKABAL,
    GOONSBEATLOTUS,
    LOTUSBEATKABAL,
    LOTUSBEATGOONS,
    KABALBEATGOONS,
    KABALBEATLOTUS,
    ERROR,
  }
}
