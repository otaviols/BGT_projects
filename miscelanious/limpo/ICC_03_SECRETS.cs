using Blizzard.T5.Core;
using System.Collections;
using UnityEngine;

public class ICC_03_SECRETS : ICC_MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = ICC_03_SECRETS.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = ICC_03_SECRETS.InitStringOptions();
  private Notification m_turnCounter;
  private TempleArt m_templeArt;
  private int m_mostRecentMissionEvent;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public ICC_03_SECRETS() => this.m_gameOptions.AddOptions(ICC_03_SECRETS.s_booleanOptions, ICC_03_SECRETS.s_stringOptions);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LOE_03_TURN_5_4.prefab:d35f6f70b2c0eca44920996f1d1b280b");
    this.PreloadSound("VO_LOE_03_TURN_6.prefab:43a3554df2b302c428fdc7325ab913ed");
    this.PreloadSound("VO_LOE_03_TURN_9.prefab:3f4b6112b2fea054693987a5cfbaf29b");
    this.PreloadSound("VO_LOE_03_TURN_5.prefab:cd01ccc585ea5e541bb6d6bf014ab57f");
    this.PreloadSound("VO_LOE_03_TURN_4_GOOD.prefab:04e58ac2fa0d1874caa45fe4bd009c16");
    this.PreloadSound("VO_LOE_03_TURN_4_BAD.prefab:2f27d2a958ffdd44387be7a1fe070234");
    this.PreloadSound("VO_LOE_03_TURN_6_2.prefab:e648bc075f6c30249a9221100bec6c06");
    this.PreloadSound("VO_LOE_03_TURN_4_NEITHER.prefab:ddb8aae686b170648898dc591fc7a554");
    this.PreloadSound("VO_LOE_03_TURN_3_WARNING.prefab:e9e9a86a32fba3842b95b38d71afe678");
    this.PreloadSound("VO_LOE_03_TURN_2.prefab:053b45bd9efaedb4f9178135644347b5");
    this.PreloadSound("VO_LOE_03_TURN_2_2.prefab:da4366528d8d3e84397dde52274585b0");
    this.PreloadSound("VO_LOE_03_TURN_4.prefab:d81cbf6a2f8657740a049208a3cc48e6");
    this.PreloadSound("VO_LOE_03_TURN_7.prefab:3ba8d9e054983934798a9ba027841605");
    this.PreloadSound("VO_LOE_03_TURN_7_2.prefab:79e0b4c1a04228749897dff2ca0d9edd");
    this.PreloadSound("VO_LOE_03_TURN_3_BOULDER.prefab:d633a3b18e9199b4d9f61b7fe8ce6527");
    this.PreloadSound("VO_LOE_03_TURN_1.prefab:90d2bc2b8b80a5444b9c15759d179dd7");
    this.PreloadSound("VO_LOE_03_TURN_8.prefab:7047eaf23edc34b42b0b171b7124d1b5");
    this.PreloadSound("VO_LOE_03_TURN_10.prefab:621285e424dd9e74c85573e03f34fb68");
    this.PreloadSound("VO_LOE_03_WIN.prefab:3b8d8d12b4f129c428c975ccc353a785");
    this.PreloadSound("VO_LOE_WING_1_WIN_2.prefab:db68235e589657e4ba8a94ff1458e299");
    this.PreloadSound("VO_LOE_03_RESPONSE.prefab:e38dfdb8a73972343b81c64ddb44171b");
  }

  public override void NotifyOfMulliganInitialized()
  {
    base.NotifyOfMulliganInitialized();
    this.m_mostRecentMissionEvent = this.GetTag(GAME_TAG.MISSION_EVENT);
    this.InitVisuals();
  }

  public override void NotifyOfMulliganEnded()
  {
    base.NotifyOfMulliganEnded();
    GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().GetHealthObject().Hide();
  }

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    if (change.tag != 48)
      return;
    this.UpdateVisuals(change.newValue);
  }

  public override string CustomChoiceBannerText()
  {
    if (this.GetTag<TAG_STEP>(GAME_TAG.STEP) == TAG_STEP.MAIN_START)
    {
      string key = (string) null;
      switch (this.m_mostRecentMissionEvent)
      {
        case 4:
          key = "MISSION_STATUES_EYE";
          break;
        case 10:
          key = "MISSION_GLOWING_POOL";
          break;
        case 11:
          key = "MISSION_PIT_OF_SPIKES";
          break;
        case 12:
          key = "MISSION_TAKE_THE_SHORTCUT";
          break;
      }
      if (key != null)
        return GameStrings.Get(key);
    }
    return (string) null;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    ICC_03_SECRETS icc03Secrets = this;
    while (icc03Secrets.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 1)
    {
      GameState.Get().SetBusy(true);
      yield return (object) icc03Secrets.PlayMissionFlavorLine("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_Sidekick_Mission03_01");
      GameState.Get().SetBusy(false);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    ICC_03_SECRETS icc03Secrets = this;
    GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().ActivateSpellDeathState(SpellType.IMMUNE);
    if (turn == 1)
    {
      int cost = icc03Secrets.GetCost();
      icc03Secrets.InitTurnCounter(cost);
      yield return (object) Gameplay.Get().StartCoroutine(icc03Secrets.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_03_TURN_1.prefab:90d2bc2b8b80a5444b9c15759d179dd7"));
    }
    if (turn % 2 == 0)
    {
      switch (icc03Secrets.GetCost())
      {
        case 1:
          yield return (object) Gameplay.Get().StartCoroutine(icc03Secrets.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_03_TURN_10.prefab:621285e424dd9e74c85573e03f34fb68"));
          break;
        case 3:
          yield return (object) Gameplay.Get().StartCoroutine(icc03Secrets.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_03_TURN_8.prefab:7047eaf23edc34b42b0b171b7124d1b5"));
          break;
      }
    }
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlayBigCharacterQuoteAndWait("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_03_RESPONSE.prefab:e38dfdb8a73972343b81c64ddb44171b"));
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    ICC_03_SECRETS icc03Secrets = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(icc03Secrets.PlayCharacterQuoteAndWait("Reno_Quote.prefab:0a2e34fa6782a0747b4f5d5574d1331a", "VO_LOE_03_WIN.prefab:3b8d8d12b4f129c428c975ccc353a785", allowRepeatDuringSession: false));
    }
  }

  private void InitVisuals()
  {
    int cost = this.GetCost();
    int tag = this.GetTag(GAME_TAG.TURN);
    this.InitTempleArt(cost);
    if (tag < 1 || !GameState.Get().IsPastBeginPhase())
      return;
    this.InitTurnCounter(cost);
  }

  private void InitTempleArt(int cost)
  {
    this.m_templeArt = AssetLoader.Get().InstantiatePrefab((AssetReference) "TempleArt.prefab:c5d0fc0812982fc4ba576e2b0cdfa548").GetComponent<TempleArt>();
    this.UpdateTempleArt(cost);
  }

  private void InitTurnCounter(int cost)
  {
    this.m_turnCounter = AssetLoader.Get().InstantiatePrefab((AssetReference) "LOE_Turn_Timer.prefab:b05530aa55868554fb8f0c66632b3c22").GetComponent<Notification>();
    PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
    component.FsmVariables.GetFsmBool("RunningMan").Value = true;
    component.FsmVariables.GetFsmBool("MineCart").Value = false;
    component.SendEvent("Birth");
    this.m_turnCounter.transform.parent = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().gameObject.transform;
    this.m_turnCounter.transform.localPosition = new Vector3(-1.4f, 0.187f, -0.11f);
    this.m_turnCounter.transform.localScale = Vector3.one * 0.52f;
    this.UpdateTurnCounterText(cost);
  }

  private void UpdateVisuals(int cost)
  {
    this.UpdateTempleArt(cost);
    if (!(bool) (Object) this.m_turnCounter)
      return;
    this.UpdateTurnCounter(cost);
  }

  private void UpdateTempleArt(int cost) => this.m_templeArt.DoPortraitSwap(GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor(), cost);

  private void UpdateTurnCounter(int cost)
  {
    this.m_turnCounter.GetComponent<PlayMakerFSM>().SendEvent("Action");
    this.UpdateTurnCounterText(cost);
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("MISSION_DEFAULTCOUNTERNAME", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");
}
