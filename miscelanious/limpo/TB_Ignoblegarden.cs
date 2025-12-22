using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_Ignoblegarden : MissionEntity
{
  private HashSet<int> seen = new HashSet<int>();
  private static readonly AssetReference VO_DrBoom_Male_Goblin_Ignoblegarden_01 = new AssetReference("VO_DrBoom_Male_Goblin_Ignoblegarden_01:d5352ba39a24f7e40b490823b44249a2");
  private static readonly AssetReference VO_DrBoom_Male_Goblin_Ignoblegarden_02 = new AssetReference("VO_DrBoom_Male_Goblin_Ignoblegarden_02:1cd6b63958bd93845a4339e493869ef2");
  private static readonly AssetReference VO_DrBoom_Male_Goblin_Ignoblegarden_03 = new AssetReference("VO_DrBoom_Male_Goblin_Ignoblegarden_03:5bd75d7de52abed44a1cbb07a2d5d65b");
  private static readonly AssetReference VO_DrBoom_Male_Goblin_Ignoblegarden_04 = new AssetReference("VO_DrBoom_Male_Goblin_Ignoblegarden_04:86b663815d9d8fc43806cb55c41df1dc");
  private static readonly AssetReference VO_DrBoom_Male_Goblin_Ignoblegarden_05 = new AssetReference("VO_DrBoom_Male_Goblin_Ignoblegarden_05:bf12e1c7bd80d724d8050188a7ea4ec3");
  private static readonly AssetReference VO_DrBoom_Male_Goblin_Ignoblegarden_06 = new AssetReference("VO_DrBoom_Male_Goblin_Ignoblegarden_06:2e155dd569b89f145967acb09a2d59a7");
  private static readonly AssetReference VO_HERO_02b_Male_Troll_Event_11 = new AssetReference("VO_HERO_02b_Male_Troll_Event_11:e360856574d463247960068d89134791");
  private static readonly Vector3 LEFT_OF_FRIENDLY_HERO = new Vector3(-1f, 0.0f, 1f);

  public override void PreloadAssets()
  {
    this.PreloadSound((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_01);
    this.PreloadSound((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_02);
    this.PreloadSound((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_03);
    this.PreloadSound((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_04);
    this.PreloadSound((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_05);
    this.PreloadSound((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_06);
    this.PreloadSound((string) TB_Ignoblegarden.VO_HERO_02b_Male_Troll_Event_11);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_Ignoblegarden tbIgnoblegarden = this;
    while (tbIgnoblegarden.m_enemySpeaking)
      yield return (object) null;
    if (!tbIgnoblegarden.seen.Contains(missionEvent))
    {
      tbIgnoblegarden.seen.Add(missionEvent);
      switch (missionEvent)
      {
        case 1:
          yield return (object) tbIgnoblegarden.PlayBossLine((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_01);
          break;
        case 2:
          yield return (object) tbIgnoblegarden.PlayBossLine((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_02);
          break;
        case 3:
          yield return (object) tbIgnoblegarden.PlayBossLine((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_03);
          break;
        case 4:
          yield return (object) tbIgnoblegarden.PlayBossLine((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_04);
          break;
        case 5:
          yield return (object) tbIgnoblegarden.PlayBossLine((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_05);
          break;
        case 6:
          yield return (object) tbIgnoblegarden.PlayBossLine((string) TB_Ignoblegarden.VO_DrBoom_Male_Goblin_Ignoblegarden_06);
          break;
      }
      yield return (object) new WaitForSeconds(4f);
    }
  }

  private IEnumerator PlayBossLine(string line, bool persistCharacter = false)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_Ignoblegarden tbIgnoblegarden = this;
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
    this.\u003C\u003E2__current = (object) tbIgnoblegarden.PlayMissionFlavorLine("DrBoom_BrassRing_Quote:01c6f6e5b12fb0e4cbb9adde214ac8dc", line, TB_Ignoblegarden.LEFT_OF_FRIENDLY_HERO, direction, persistCharacter: persistCharacter);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public TB_Ignoblegarden()
    : base()
  {
  }
}
