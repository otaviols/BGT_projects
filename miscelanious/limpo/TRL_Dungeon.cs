using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRL_Dungeon : TRL_MissionEntity
{
  private static readonly AssetReference VO_TRLA_209h_Male_Troll_Shrine_Killed_01 = new AssetReference("VO_TRLA_209h_Male_Troll_Shrine_Killed_01.prefab:6310971ec24c8b04c855b61ab0de2ee7");
  private static readonly AssetReference VO_TRLA_209h_Male_Troll_Shrine_Killed_02 = new AssetReference("VO_TRLA_209h_Male_Troll_Shrine_Killed_02.prefab:69df2bf9c601e064ebe4b0fcc9b7bff0");
  private static readonly AssetReference VO_TRLA_209h_Male_Troll_Shrine_Killed_03 = new AssetReference("VO_TRLA_209h_Male_Troll_Shrine_Killed_03.prefab:80a489f9a60de644d94dd836da122249");
  private static readonly AssetReference VO_TRLA_209h_Male_Troll_Shrine_Killed_04 = new AssetReference("VO_TRLA_209h_Male_Troll_Shrine_Killed_04.prefab:8f1be59482206f046a5bc90d77e5a215");
  public static float f_shrineKillPlayRate = 1f;
  public static string s_deathLine = (string) null;
  public static string s_responseLineGreeting = (string) null;
  public static string s_responseLineWow = (string) null;
  public static string s_responseLineThreaten = (string) null;
  public static string s_responseLineWellPlayed = (string) null;
  public static string s_responseLineThanks = (string) null;
  public static string s_responseLineOops = (string) null;
  public static string s_responseLineSorry = (string) null;
  public static List<string> s_bossShrineDeathLines = new List<string>();
  public static List<string> s_genericShrineDeathLines = new List<string>();
  public static List<string> s_druidShrineDeathLines = new List<string>();
  public static List<string> s_hunterShrineDeathLines = new List<string>();
  public static List<string> s_mageShrineDeathLines = new List<string>();
  public static List<string> s_paladinShrineDeathLines = new List<string>();
  public static List<string> s_priestShrineDeathLines = new List<string>();
  public static List<string> s_rogueShrineDeathLines = new List<string>();
  public static List<string> s_shamanShrineDeathLines = new List<string>();
  public static List<string> s_warlockShrineDeathLines = new List<string>();
  public static List<string> s_warriorShrineDeathLines = new List<string>();
  protected static List<string> m_RikkarRandomLines = new List<string>()
  {
    (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_01,
    (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_02,
    (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_03,
    (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_04
  };

  public static TRL_Dungeon InstantiateTRLDungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    string opposingHeroCardId = GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame);
    switch (opposingHeroCardId)
    {
      case "TRLA_200h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_200h();
      case "TRLA_201h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_201h();
      case "TRLA_202h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_202h();
      case "TRLA_203h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_203h();
      case "TRLA_204h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_204h();
      case "TRLA_205h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_205h();
      case "TRLA_206h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_206h();
      case "TRLA_207h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_207h();
      case "TRLA_208h":
        return (TRL_Dungeon) new TRL_Dungeon_Boss_208h();
      default:
        Log.All.PrintError("TRL_Dungeon.InstantiateTRLDungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) opposingHeroCardId);
        return new TRL_Dungeon();
    }
  }

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_01,
      (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_02,
      (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_03,
      (string) TRL_Dungeon.VO_TRLA_209h_Male_Troll_Shrine_Killed_04
    })
      this.PreloadSound(soundPath);
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (this.m_enemySpeaking || string.IsNullOrEmpty(TRL_Dungeon.s_deathLine) || gameResult != TAG_PLAYSTATE.WON)
      return;
    if (this.GetShouldSupressDeathTextBubble())
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_deathLine, Notification.SpeechBubbleDirection.None, actor));
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_deathLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (emoteType)
    {
      case EmoteType.GREETINGS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_responseLineGreeting, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.WELL_PLAYED:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_responseLineWellPlayed, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.OOPS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_responseLineOops, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.THREATEN:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_responseLineThreaten, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.THANKS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_responseLineThanks, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.SORRY:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_responseLineSorry, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.WOW:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(TRL_Dungeon.s_responseLineWow, Notification.SpeechBubbleDirection.TopRight, actor));
        break;
    }
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    TRL_Dungeon.s_deathLine = (string) null;
    TRL_Dungeon.s_responseLineGreeting = (string) null;
    TRL_Dungeon.s_responseLineWow = (string) null;
    TRL_Dungeon.s_responseLineThreaten = (string) null;
    TRL_Dungeon.s_responseLineWellPlayed = (string) null;
    TRL_Dungeon.s_responseLineThanks = (string) null;
    TRL_Dungeon.s_responseLineOops = (string) null;
    TRL_Dungeon.s_responseLineSorry = (string) null;
    TRL_Dungeon.s_bossShrineDeathLines = new List<string>();
    TRL_Dungeon.s_genericShrineDeathLines = new List<string>();
    TRL_Dungeon.s_druidShrineDeathLines = new List<string>();
    TRL_Dungeon.s_hunterShrineDeathLines = new List<string>();
    TRL_Dungeon.s_mageShrineDeathLines = new List<string>();
    TRL_Dungeon.s_paladinShrineDeathLines = new List<string>();
    TRL_Dungeon.s_priestShrineDeathLines = new List<string>();
    TRL_Dungeon.s_rogueShrineDeathLines = new List<string>();
    TRL_Dungeon.s_shamanShrineDeathLines = new List<string>();
    TRL_Dungeon.s_warlockShrineDeathLines = new List<string>();
    TRL_Dungeon.s_warriorShrineDeathLines = new List<string>();
  }

  protected virtual bool GetShouldSupressDeathTextBubble() => false;

  protected override float ChanceToPlayRandomVOLine() => 1f;

  private IEnumerator PlayRandomRikkarShrineDeathLine()
  {
    TRL_Dungeon trlDungeon = this;
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    if (!GameState.Get().IsFriendlySidePlayerTurn() && trlDungeon.CanPlayVOLines(actor.GetEntity(), GenericDungeonMissionEntity.VOSpeaker.FRIENDLY_HERO))
    {
      string line = trlDungeon.PopRandomLineWithChance(TRL_Dungeon.m_RikkarRandomLines);
      if (line != null)
        yield return (object) trlDungeon.PlayLineOnlyOnce(actor, line);
    }
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnce(Actor actor, List<string> lines)
  {
    TRL_Dungeon trlDungeon = this;
    string line = trlDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) trlDungeon.PlayLineOnlyOnce(actor, line);
  }

  protected string PopRandomLine(List<string> lines)
  {
    if (lines.Count == 0 || lines == null)
      return (string) null;
    string line = lines[Random.Range(0, lines.Count)];
    lines.Remove(line);
    return line;
  }

  private IEnumerator PlayClassOrGenericShrineDeathLine(List<string> classDeathLines)
  {
    TRL_Dungeon trlDungeon = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string line1 = trlDungeon.PopRandomLineWithChance(classDeathLines);
    if (line1 != null)
    {
      yield return (object) trlDungeon.PlayLineOnlyOnce(actor, line1);
    }
    else
    {
      string line2 = trlDungeon.PopRandomLineWithChance(TRL_Dungeon.s_genericShrineDeathLines);
      if (line2 != null)
        yield return (object) trlDungeon.PlayLineOnlyOnce(actor, line2);
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TRL_Dungeon trlDungeon = this;
    while (trlDungeon.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 150:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_genericShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 152:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_druidShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 153:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_hunterShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 154:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_mageShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 155:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_paladinShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 156:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_priestShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 157:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_rogueShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 158:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_shamanShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 159:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_warlockShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 160:
        yield return (object) trlDungeon.PlayClassOrGenericShrineDeathLine(TRL_Dungeon.s_warriorShrineDeathLines);
        yield return (object) trlDungeon.PlayRandomRikkarShrineDeathLine();
        break;
      case 202:
        string line = trlDungeon.PopRandomLineWithChance(TRL_Dungeon.s_bossShrineDeathLines);
        if (line == null)
          break;
        yield return (object) trlDungeon.PlayBossLine(actor, line);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) trlDungeon.\u003C\u003En__0(missionEvent);
        break;
    }
  }
}
