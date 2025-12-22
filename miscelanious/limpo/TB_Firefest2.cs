using System.Collections;
using UnityEngine;

public class TB_Firefest2 : MissionEntity
{
  private Actor ragnarosActor;
  private Card ragnarosCard;
  private TB_Firefest2.VICTOR matchResult;
  private static readonly AssetReference VO_AHUNE_Male_Elemental_HSFireFestival_02 = new AssetReference("VO_AHUNE_Male_Elemental_HSFireFestival_02:8dc670a0c96a23d44bc6b87957b223fe");
  private static readonly AssetReference VO_AHUNE_Male_Elemental_HSFireFestival_04 = new AssetReference("VO_AHUNE_Male_Elemental_HSFireFestival_04:63a6f5920298e39418087cbfb837e9af");
  private static readonly AssetReference VO_AHUNE_Male_Elemental_HSFireFestival_05 = new AssetReference("VO_AHUNE_Male_Elemental_HSFireFestival_05:4a15da86b328e8c4ea5a805f9080f8c5");
  private static readonly AssetReference VO_RAGNAROS_Male_Elemental_AhuneResponses_01 = new AssetReference("VO_RAGNAROS_Male_Elemental_AhuneResponses_01:0de84fe30f9c4c04dbc1f996bd2694b3");
  private static readonly AssetReference VO_RAGNAROS_Male_Elemental_AhuneResponses_02 = new AssetReference("VO_RAGNAROS_Male_Elemental_AhuneResponses_02:58a0b7d69171f57409999d3b984c54d9");
  private static readonly AssetReference VO_RAGNAROS_Male_Elemental_AhuneResponses_05 = new AssetReference("VO_RAGNAROS_Male_Elemental_AhuneResponses_05:31f75d15dc1a7f34bafcfbdde1c9f2a1");
  private static readonly AssetReference VO_Ragnaros_Male_Elemental_Brawl_01 = new AssetReference("VO_Ragnaros_Male_Elemental_Brawl_01:da09dbd1ad9ba434fbb549c8bbd2c9ce");
  private static readonly AssetReference VO_Ragnaros_Male_Elemental_Brawl_02 = new AssetReference("VO_Ragnaros_Male_Elemental_Brawl_02:b5630abf5a135384695d1f58fa025fe5");
  private static readonly AssetReference VO_Ragnaros_Male_Elemental_Brawl_05 = new AssetReference("VO_Ragnaros_Male_Elemental_Brawl_05:4455f90db8e99eb45bc158677acb672e");
  private static readonly AssetReference VO_Ragnaros_Male_Elemental_Brawl_07 = new AssetReference("VO_Ragnaros_Male_Elemental_Brawl_07:0934d1efe1db28041be7f03b4295ffdd");
  private static readonly AssetReference VO_Ahune_Male_Elemental_Brawl_18 = new AssetReference("VO_Ahune_Male_Elemental_Brawl_18:49d4e9ef35728e84cb171df7cc56a32b");
  private static readonly AssetReference VO_Ahune_Male_Elemental_Brawl_20 = new AssetReference("VO_Ahune_Male_Elemental_Brawl_20:5fb1142388aecbd4588f2ca08d8f391a");
  private static readonly AssetReference VO_Ahune_Male_Elemental_Brawl_25 = new AssetReference("VO_Ahune_Male_Elemental_Brawl_25:c280663239e28fe419072cc64df39098");
  private static readonly Vector3 LEFT_OF_ENEMY_HERO = new Vector3(-1f, 0.0f, -2.8f);
  private static readonly Vector3 RIGHT_OF_ENEMY_HERO = new Vector3(-6f, 0.0f, -2.8f);

  public override void PreloadAssets()
  {
    this.PreloadSound((string) TB_Firefest2.VO_AHUNE_Male_Elemental_HSFireFestival_02);
    this.PreloadSound((string) TB_Firefest2.VO_AHUNE_Male_Elemental_HSFireFestival_04);
    this.PreloadSound((string) TB_Firefest2.VO_AHUNE_Male_Elemental_HSFireFestival_05);
    this.PreloadSound((string) TB_Firefest2.VO_RAGNAROS_Male_Elemental_AhuneResponses_01);
    this.PreloadSound((string) TB_Firefest2.VO_RAGNAROS_Male_Elemental_AhuneResponses_02);
    this.PreloadSound((string) TB_Firefest2.VO_RAGNAROS_Male_Elemental_AhuneResponses_05);
    this.PreloadSound((string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_01);
    this.PreloadSound((string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_02);
    this.PreloadSound((string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_05);
    this.PreloadSound((string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_07);
    this.PreloadSound((string) TB_Firefest2.VO_Ahune_Male_Elemental_Brawl_18);
    this.PreloadSound((string) TB_Firefest2.VO_Ahune_Male_Elemental_Brawl_20);
    this.PreloadSound((string) TB_Firefest2.VO_Ahune_Male_Elemental_Brawl_25);
  }

  private void GetRagnaros()
  {
    int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_2);
    Entity entity = GameState.Get().GetEntity(tag);
    if (entity != null)
      this.ragnarosCard = entity.GetCard();
    if (!((Object) this.ragnarosCard != (Object) null))
      return;
    this.ragnarosActor = this.ragnarosCard.GetActor();
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_Firefest2 tbFirefest2 = this;
    while (tbFirefest2.m_enemySpeaking)
      yield return (object) null;
    tbFirefest2.GetRagnaros();
    NameBanner banner = Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING);
    switch (missionEvent)
    {
      case 10:
        yield return (object) tbFirefest2.PlayBossLine(TB_Firefest2.BOSS.AHUNE, (string) TB_Firefest2.VO_AHUNE_Male_Elemental_HSFireFestival_02);
        yield return (object) tbFirefest2.PlayBossLineGameOver(TB_Firefest2.BOSS.RAGNAROS, (string) TB_Firefest2.VO_RAGNAROS_Male_Elemental_AhuneResponses_01);
        yield return (object) new WaitForSeconds(4f);
        banner.SetName(GameStrings.Get("TB_FIREFEST2_02"));
        break;
      case 11:
        tbFirefest2.GetRagnaros();
        Debug.Log((object) "Case11");
        yield return (object) tbFirefest2.PlayBossLine(TB_Firefest2.BOSS.AHUNE, (string) TB_Firefest2.VO_AHUNE_Male_Elemental_HSFireFestival_04);
        yield return (object) tbFirefest2.PlayBossLineGameOver(TB_Firefest2.BOSS.RAGNAROS, (string) TB_Firefest2.VO_RAGNAROS_Male_Elemental_AhuneResponses_02);
        yield return (object) tbFirefest2.PlayBossLine(TB_Firefest2.BOSS.AHUNE, (string) TB_Firefest2.VO_AHUNE_Male_Elemental_HSFireFestival_05);
        yield return (object) tbFirefest2.PlayBossLineGameOver(TB_Firefest2.BOSS.RAGNAROS, (string) TB_Firefest2.VO_RAGNAROS_Male_Elemental_AhuneResponses_05);
        banner.SetName(GameStrings.Get("TB_FIREFEST2_02"));
        break;
      case 13:
        yield return (object) tbFirefest2.PlayBossLineGameOver(TB_Firefest2.BOSS.RAGNAROS, (string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_02);
        banner.SetName(GameStrings.Get("TB_FIREFEST2_01"));
        break;
      case 14:
        yield return (object) tbFirefest2.PlayBossLineGameOver(TB_Firefest2.BOSS.RAGNAROS, (string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_01);
        banner.SetName(GameStrings.Get("TB_FIREFEST2_01"));
        break;
      case 16:
        Gameplay.Get().StartCoroutine(tbFirefest2.PlaySoundAndBlockSpeech((string) TB_Firefest2.VO_Ahune_Male_Elemental_Brawl_18, Notification.SpeechBubbleDirection.TopLeft, tbFirefest2.ragnarosActor));
        break;
    }
  }

  private IEnumerator PlayBossLineGameOver(
    TB_Firefest2.BOSS boss,
    string line,
    bool persistCharacter = false)
  {
    TB_Firefest2 tbFirefest2 = this;
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.TopRight;
    switch (boss)
    {
      case TB_Firefest2.BOSS.AHUNE:
        yield return (object) tbFirefest2.PlayMissionFlavorLine("Ahune_BigQuote.prefab:00dd8f83adda33345ac291cc76241482", line, TB_Firefest2.RIGHT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_Firefest2.BOSS.RAGNAROS:
        yield return (object) tbFirefest2.PlayMissionFlavorLine("Ragnaros_BigQuote.prefab:843c4fab946192943a909b026f755505", line, TB_Firefest2.RIGHT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
    }
  }

  private IEnumerator PlayBossLine(
    TB_Firefest2.BOSS boss,
    string line,
    bool persistCharacter = false)
  {
    TB_Firefest2 tbFirefest2 = this;
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.TopLeft;
    switch (boss)
    {
      case TB_Firefest2.BOSS.AHUNE:
        yield return (object) tbFirefest2.PlayMissionFlavorLine("Ahune_BigQuote.prefab:00dd8f83adda33345ac291cc76241482", line, TB_Firefest2.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_Firefest2.BOSS.RAGNAROS:
        yield return (object) tbFirefest2.PlayMissionFlavorLine("Ragnaros_BigQuote.prefab:843c4fab946192943a909b026f755505", line, TB_Firefest2.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
    }
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    Debug.Log((object) ("gameresult is " + (object) gameResult));
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        this.matchResult = TB_Firefest2.VICTOR.PLAYERWIN;
        break;
      case TAG_PLAYSTATE.LOST:
        Debug.Log((object) "Made it to Playstate:Lost");
        this.matchResult = TB_Firefest2.VICTOR.ELEMENTALSWIN;
        break;
      case TAG_PLAYSTATE.TIED:
        this.matchResult = TB_Firefest2.VICTOR.ERROR;
        break;
    }
    base.NotifyOfGameOver(gameResult);
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    yield return (object) new WaitForSeconds(2f);
    switch (this.matchResult)
    {
      case TB_Firefest2.VICTOR.ELEMENTALSWIN:
        Debug.Log((object) "elementals won");
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(3f);
        yield return (object) this.PlayBossLine(TB_Firefest2.BOSS.AHUNE, (string) TB_Firefest2.VO_Ahune_Male_Elemental_Brawl_25);
        yield return (object) this.PlayBossLineGameOver(TB_Firefest2.BOSS.RAGNAROS, (string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_07);
        GameState.Get().SetBusy(false);
        break;
      case TB_Firefest2.VICTOR.PLAYERWIN:
        yield return (object) new WaitForSeconds(3f);
        GameState.Get().SetBusy(true);
        yield return (object) this.PlayBossLine(TB_Firefest2.BOSS.AHUNE, (string) TB_Firefest2.VO_Ahune_Male_Elemental_Brawl_20);
        yield return (object) this.PlayBossLineGameOver(TB_Firefest2.BOSS.RAGNAROS, (string) TB_Firefest2.VO_Ragnaros_Male_Elemental_Brawl_05);
        GameState.Get().SetBusy(false);
        break;
    }
  }

  public TB_Firefest2()
    : base()
  {
  }

  private enum VICTOR
  {
    ELEMENTALSWIN,
    PLAYERWIN,
    ERROR,
  }

  private enum BOSS
  {
    AHUNE,
    RAGNAROS,
  }
}
