using System.Collections;
using UnityEngine;

public class TB_SPT_DALA : MissionEntity
{
  private TB_SPT_DALA.VICTOR matchResult;
  private static readonly AssetReference VO_DRGA_BOSS_05h_Male_Goblin_WinterEVIL_Exchange6_01 = new AssetReference("VO_DRGA_BOSS_05h_Male_Goblin_WinterEVIL_Exchange6_01.prefab:334f3d6a44555ea45badbedcf4f49248");
  private static readonly AssetReference VO_DRGA_BOSS_06h_Female_Orc_WinterEVIL_Exchange10_01 = new AssetReference("VO_DRGA_BOSS_06h_Female_Orc_WinterEVIL_Exchange10_01.prefab:68676ec9aa6516246ada918a62545dfc");
  private static readonly AssetReference VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Exchange1_01 = new AssetReference("VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Exchange1_01.prefab:f1b2b04920a7b7041ba167c3e8f387f5");
  private static readonly AssetReference VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Loss_01 = new AssetReference("VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Loss_01.prefab:dedb812e87f1b4c49b972140a35db48c");
  private static readonly AssetReference VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_01 = new AssetReference("VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_01.prefab:6b860d24c5fc93344971ace72547d60b");
  private static readonly AssetReference VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_02 = new AssetReference("VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_02.prefab:6f65ddd984b97d94a93a5ae316fe0bf5");
  private static readonly AssetReference VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_03 = new AssetReference("VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_03.prefab:2d3ab2f215843064c8b7aea4dd6d0f89");
  private static readonly AssetReference VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Victory_01 = new AssetReference("VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Victory_01.prefab:33e07132a9c1ea741b5fd8b807e799b9");
  private static readonly AssetReference VO_DRGA_BOSS_08h_Male_Kobold_WinterEVIL_Exchange4_01 = new AssetReference("VO_DRGA_BOSS_08h_Male_Kobold_WinterEVIL_Exchange4_01.prefab:1ad7915c429c71645a840c5b39e127db");
  private static readonly AssetReference VO_DRGA_BOSS_09h_Female_Troll_WinterEVIL_Exchange8_01 = new AssetReference("VO_DRGA_BOSS_09h_Female_Troll_WinterEVIL_Exchange8_01.prefab:da1b10361658a1341b1fa8f97d041a26");
  private static readonly Vector3 LEFT_OF_FRIENDLY_HERO = new Vector3(-1f, 0.0f, 1f);
  private static readonly Vector3 LEFT_OF_ENEMY_HERO = new Vector3(-1f, 0.0f, -2.8f);
  private static readonly Vector3 RIGHT_OF_ENEMY_HERO = new Vector3(-6f, 0.0f, -2.8f);

  public override void PreloadAssets()
  {
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_05h_Male_Goblin_WinterEVIL_Exchange6_01);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_06h_Female_Orc_WinterEVIL_Exchange10_01);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Exchange1_01);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Loss_01);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_01);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_02);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_03);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Victory_01);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_08h_Male_Kobold_WinterEVIL_Exchange4_01);
    this.PreloadSound((string) TB_SPT_DALA.VO_DRGA_BOSS_09h_Female_Troll_WinterEVIL_Exchange8_01);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_SPT_DALA tbSptDala = this;
    while (tbSptDala.m_enemySpeaking)
      yield return (object) null;
    switch (missionEvent)
    {
      case 101:
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.RAFAAM, (string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_01);
        break;
      case 102:
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.RAFAAM, (string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_02);
        yield return (object) new WaitForSeconds(1f);
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.RAFAAM, (string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Start_03);
        break;
      case 103:
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.RAFAAM, (string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Exchange1_01);
        break;
      case 104:
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.BOOM, (string) TB_SPT_DALA.VO_DRGA_BOSS_05h_Male_Goblin_WinterEVIL_Exchange6_01);
        break;
      case 105:
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.HAGATHA, (string) TB_SPT_DALA.VO_DRGA_BOSS_06h_Female_Orc_WinterEVIL_Exchange10_01);
        break;
      case 107:
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.LAZUL, (string) TB_SPT_DALA.VO_DRGA_BOSS_09h_Female_Troll_WinterEVIL_Exchange8_01);
        break;
      case 108:
        yield return (object) tbSptDala.PlayBossLineLeft(TB_SPT_DALA.BOSS.TOGWAGGLE, (string) TB_SPT_DALA.VO_DRGA_BOSS_08h_Male_Kobold_WinterEVIL_Exchange4_01);
        break;
    }
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        this.matchResult = TB_SPT_DALA.VICTOR.PLAYERWIN;
        break;
      case TAG_PLAYSTATE.LOST:
        Debug.Log((object) "Made it to Playstate:Lost");
        this.matchResult = TB_SPT_DALA.VICTOR.PLAYERLOST;
        break;
      case TAG_PLAYSTATE.TIED:
        this.matchResult = TB_SPT_DALA.VICTOR.ERROR;
        break;
    }
    base.NotifyOfGameOver(gameResult);
  }

  private IEnumerator PlayBossLineLeft(
    TB_SPT_DALA.BOSS boss,
    string line,
    bool persistCharacter = false)
  {
    TB_SPT_DALA tbSptDala = this;
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.TopLeft;
    switch (boss)
    {
      case TB_SPT_DALA.BOSS.BOOM:
        yield return (object) tbSptDala.PlayMissionFlavorLine("Blastermaster_Boom_popup_BrassRing_Quote.prefab:71029fa93b8e9564bb2fa3003158ba08", line, TB_SPT_DALA.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_SPT_DALA.BOSS.HAGATHA:
        yield return (object) tbSptDala.PlayMissionFlavorLine("Hagatha_Pop-up_BrassRing_Quote.prefab:82d8a1fd3b66a3c4da28e4dc34b42617", line, TB_SPT_DALA.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_SPT_DALA.BOSS.TOGWAGGLE:
        yield return (object) tbSptDala.PlayMissionFlavorLine("Togwaggle_pop-up_BrassRing_Quote.prefab:99e68bee5c488cb45a212327619b0922", line, TB_SPT_DALA.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_SPT_DALA.BOSS.LAZUL:
        yield return (object) tbSptDala.PlayMissionFlavorLine("Madam_Lazul_Popup_BrassRing_Quote.prefab:5fd991c28d0cc7842b99ae3ddb65aa0c", line, TB_SPT_DALA.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_SPT_DALA.BOSS.RAFAAM:
        yield return (object) tbSptDala.PlayMissionFlavorLine("Rafaam_popup_BrassRing_Quote.prefab:187724fae6d64cf49acf11aa53ca2087", line, TB_SPT_DALA.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(5f);
    GameState.Get().SetBusy(false);
    switch (this.matchResult)
    {
      case TB_SPT_DALA.VICTOR.PLAYERLOST:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLineLeft(TB_SPT_DALA.BOSS.RAFAAM, (string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case TB_SPT_DALA.VICTOR.PLAYERWIN:
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLineLeft(TB_SPT_DALA.BOSS.RAFAAM, (string) TB_SPT_DALA.VO_DRGA_BOSS_07h_Male_Ethereal_WinterEVIL_Victory_01);
        GameState.Get().SetBusy(false);
        break;
    }
  }

  public TB_SPT_DALA()
    : base()
  {
  }

  private enum VICTOR
  {
    PLAYERLOST,
    PLAYERWIN,
    ERROR,
  }

  private enum BOSS
  {
    BOOM,
    HAGATHA,
    TOGWAGGLE,
    LAZUL,
    RAFAAM,
  }
}
