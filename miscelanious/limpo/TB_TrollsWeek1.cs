using System.Collections;
using UnityEngine;

public class TB_TrollsWeek1 : MissionEntity
{
  private TB_TrollsWeek1.VICTOR matchResult;
  private int shouldShowVictory;
  private int shouldShowIntro;
  private static readonly AssetReference VO_HERO_02b_Male_Troll_Event_11 = new AssetReference("VO_HERO_02b_Male_Troll_Event_11:e360856574d463247960068d89134791");
  private static readonly AssetReference VO_HERO_02b_Male_Troll_Event_03 = new AssetReference("VO_HERO_02b_Male_Troll_Event_03:7a6078498a8e2dc4284fc70f8d37faf4");
  private static readonly AssetReference VO_HERO_02b_Male_Troll_Event_09 = new AssetReference("VO_HERO_02b_Male_Troll_Event_09:2b061472d8f0e4549801ff0c25d8d686");
  private static readonly Vector3 LEFT_OF_FRIENDLY_HERO = new Vector3(-1f, 0.0f, 1f);
  private Player friendlySidePlayer;

  public override void PreloadAssets()
  {
    this.PreloadSound((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_11);
    this.PreloadSound((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_03);
    this.PreloadSound((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_09);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_TrollsWeek1 tbTrollsWeek1 = this;
    while (tbTrollsWeek1.m_enemySpeaking)
      yield return (object) null;
    switch (missionEvent)
    {
      case 10:
        tbTrollsWeek1.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
        tbTrollsWeek1.shouldShowIntro = tbTrollsWeek1.friendlySidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
        if (tbTrollsWeek1.shouldShowIntro != 1)
          break;
        yield return (object) tbTrollsWeek1.PlayBossLine((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_11);
        yield return (object) new WaitForSeconds(4f);
        break;
      case 11:
        yield return (object) tbTrollsWeek1.PlayBossLine((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_03);
        yield return (object) new WaitForSeconds(4f);
        break;
      case 12:
        yield return (object) tbTrollsWeek1.PlayBossLine((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_09);
        yield return (object) new WaitForSeconds(4f);
        break;
    }
  }

  private IEnumerator PlayBossLine(string line, bool persistCharacter = false)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_TrollsWeek1 tbTrollsWeek1 = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.BottomLeft;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) tbTrollsWeek1.PlayMissionFlavorLine("Rastakhan_BrassRing_Quote:179bfad1464576448aeabfe5c3eff601", line, TB_TrollsWeek1.LEFT_OF_FRIENDLY_HERO, direction, persistCharacter: persistCharacter);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        this.matchResult = TB_TrollsWeek1.VICTOR.PLAYERWIN;
        break;
      case TAG_PLAYSTATE.LOST:
        Debug.Log((object) "Made it to Playstate:Lost");
        this.matchResult = TB_TrollsWeek1.VICTOR.PLAYERLOST;
        break;
      case TAG_PLAYSTATE.TIED:
        this.matchResult = TB_TrollsWeek1.VICTOR.ERROR;
        break;
    }
    base.NotifyOfGameOver(gameResult);
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    this.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    this.shouldShowVictory = this.friendlySidePlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2);
    yield return (object) new WaitForSeconds(2f);
    switch (this.matchResult)
    {
      case TB_TrollsWeek1.VICTOR.PLAYERLOST:
        GameState.Get().SetBusy(true);
        GameState.Get().SetBusy(false);
        break;
      case TB_TrollsWeek1.VICTOR.PLAYERWIN:
        if (this.shouldShowVictory != 1)
          break;
        yield return (object) new WaitForSeconds(3f);
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_03);
        yield return (object) this.PlayBossLine((string) TB_TrollsWeek1.VO_HERO_02b_Male_Troll_Event_09);
        GameState.Get().SetBusy(false);
        break;
    }
  }

  public TB_TrollsWeek1()
    : base()
  {
  }

  private enum VICTOR
  {
    PLAYERLOST,
    PLAYERWIN,
    ERROR,
  }
}
