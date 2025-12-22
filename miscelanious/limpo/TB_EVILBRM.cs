using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_EVILBRM : MissionEntity
{
  private static readonly AssetReference VO_Rafaam_Male_Ethereal_BRM_Start_01 = new AssetReference("VO_Rafaam_Male_Ethereal_BRM_Start_01:840b30444d3dcac419d14454b31ef534");
  private static readonly AssetReference VO_DrBoom_Male_Goblin_BRM_T1End_01 = new AssetReference("VO_DrBoom_Male_Goblin_BRM_T1End_01:ba94239568242a04db30dcf8fc6be837");
  private static readonly AssetReference VO_DrBoom_Male_Goblin_BRM_Victory_01 = new AssetReference("VO_DrBoom_Male_Goblin_BRM_Victory_01:0b3187eb9664d9f4ca73ff246baa6463");
  private static readonly AssetReference VO_Hagatha_Female_Orc_BRM_Victory_01 = new AssetReference("VO_Hagatha_Female_Orc_BRM_Victory_01:2832c50d764531d4794545901326adac");
  private static readonly AssetReference VO_MadameLazul_Female_Troll_BRM_Victory_01 = new AssetReference("VO_MadameLazul_Female_Troll_BRM_Victory_01:1d1c9015c5e6cd34892e179df49768e2");
  private static readonly AssetReference VO_Togwaggle_Male_Kobold_BRM_Victory_01 = new AssetReference("VO_Togwaggle_Male_Kobold_BRM_Victory_01:849eb420629bf2a41aaf192489366f8d");
  private static readonly AssetReference VO_Rafaam_Male_Ethereal_HM_Victory_01 = new AssetReference("VO_Rafaam_Male_Ethereal_HM_Victory_01:04141be712eae134b85f869c50056efa");
  private Notification m_popup;
  private float popupScale = 1.4f;
  private static readonly Dictionary<int, TB_EVILBRM.PopupMessage> popupMsgs = new Dictionary<int, TB_EVILBRM.PopupMessage>()
  {
    {
      1000,
      new TB_EVILBRM.PopupMessage()
      {
        Message = "TB_EVILBRM_CURRENT_BEST_SCORE",
        Delay = 5f
      }
    },
    {
      2000,
      new TB_EVILBRM.PopupMessage()
      {
        Message = "TB_EVILBRM_NEW_BEST_SCORE",
        Delay = 5f
      }
    }
  };
  private static readonly Vector3 LEFT_OF_ENEMY_HERO = new Vector3(-1f, 0.0f, -1.8f);
  private static readonly Vector3 RIGHT_OF_ENEMY_HERO = new Vector3(-6f, 0.0f, -1.8f);
  private TB_EVILBRM.VICTOR matchResult;
  private int currentSelectedBoss;
  private int isOnRagnaros;
  private Player enemyPlayer;

  public override void PreloadAssets()
  {
    this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
    this.PreloadSound((string) TB_EVILBRM.VO_Rafaam_Male_Ethereal_BRM_Start_01);
    this.PreloadSound((string) TB_EVILBRM.VO_DrBoom_Male_Goblin_BRM_T1End_01);
    this.PreloadSound((string) TB_EVILBRM.VO_DrBoom_Male_Goblin_BRM_Victory_01);
    this.PreloadSound((string) TB_EVILBRM.VO_Hagatha_Female_Orc_BRM_Victory_01);
    this.PreloadSound((string) TB_EVILBRM.VO_MadameLazul_Female_Troll_BRM_Victory_01);
    this.PreloadSound((string) TB_EVILBRM.VO_Togwaggle_Male_Kobold_BRM_Victory_01);
    this.PreloadSound((string) TB_EVILBRM.VO_Rafaam_Male_Ethereal_HM_Victory_01);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_EVILBRM tbEvilbrm = this;
    while (tbEvilbrm.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 1000)
    {
      int tag = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
      string msgString;
      if (tag == 0)
      {
        msgString = GameStrings.Get(TB_EVILBRM.popupMsgs[2000].Message);
      }
      else
      {
        string str1 = "";
        string str2 = "";
        string str3 = "";
        int num1 = tag / 3600;
        int num2 = tag % 3600 / 60;
        int num3 = tag % 60;
        if (num1 < 10)
          str1 = "0";
        if (num2 < 10)
          str2 = "0";
        if (num3 < 10)
          str3 = "0";
        msgString = GameStrings.Get(TB_EVILBRM.popupMsgs[missionEvent].Message) + "\n" + str1 + (object) num1 + ":" + str2 + (object) num2 + ":" + str3 + (object) num3;
        tbEvilbrm.popupScale = 1.7f;
      }
      Vector3 popUpPos = new Vector3();
      popUpPos.z = GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer() ? ((bool) UniversalInputManager.UsePhoneUI ? -40f : -40f) : ((bool) UniversalInputManager.UsePhoneUI ? 27f : 18f);
      yield return (object) new WaitForSeconds(4f);
      yield return (object) tbEvilbrm.ShowPopup(msgString, TB_EVILBRM.popupMsgs[missionEvent].Delay, popUpPos, tbEvilbrm.popupScale);
      msgString = (string) null;
      popUpPos = new Vector3();
    }
    if (missionEvent == 10)
    {
      yield return (object) new WaitForSeconds(1f);
      yield return (object) tbEvilbrm.PlayBossLineLeft(TB_EVILBRM.BOSS.RAFAAM, (string) TB_EVILBRM.VO_Rafaam_Male_Ethereal_BRM_Start_01);
      yield return (object) new WaitForSeconds(0.5f);
      yield return (object) tbEvilbrm.PlayBossLineRight(TB_EVILBRM.BOSS.BOOM, (string) TB_EVILBRM.VO_DrBoom_Male_Goblin_BRM_T1End_01);
    }
  }

  private IEnumerator PlayBossLineLeft(
    TB_EVILBRM.BOSS boss,
    string line,
    bool persistCharacter = false)
  {
    TB_EVILBRM tbEvilbrm = this;
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.TopLeft;
    switch (boss)
    {
      case TB_EVILBRM.BOSS.BOOM:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Blastermaster_Boom_popup_BrassRing_Quote.prefab:71029fa93b8e9564bb2fa3003158ba08", line, TB_EVILBRM.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.HAGATHA:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Hagatha_Pop-up_BrassRing_Quote.prefab:82d8a1fd3b66a3c4da28e4dc34b42617", line, TB_EVILBRM.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.TOGWAGGLE:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Togwaggle_pop-up_BrassRing_Quote.prefab:99e68bee5c488cb45a212327619b0922", line, TB_EVILBRM.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.LAZUL:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Madam_Lazul_Popup_BrassRing_Quote.prefab:5fd991c28d0cc7842b99ae3ddb65aa0c", line, TB_EVILBRM.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.RAFAAM:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Rafaam_popup_BrassRing_Quote:187724fae6d64cf49acf11aa53ca2087", line, TB_EVILBRM.LEFT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
    }
  }

  private IEnumerator PlayBossLineRight(
    TB_EVILBRM.BOSS boss,
    string line,
    bool persistCharacter = false)
  {
    TB_EVILBRM tbEvilbrm = this;
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.TopRight;
    switch (boss)
    {
      case TB_EVILBRM.BOSS.BOOM:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Blastermaster_Boom_popup_BrassRing_Quote.prefab:71029fa93b8e9564bb2fa3003158ba08", line, TB_EVILBRM.RIGHT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.HAGATHA:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Hagatha_Pop-up_BrassRing_Quote.prefab:82d8a1fd3b66a3c4da28e4dc34b42617", line, TB_EVILBRM.RIGHT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.TOGWAGGLE:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Togwaggle_pop-up_BrassRing_Quote.prefab:99e68bee5c488cb45a212327619b0922", line, TB_EVILBRM.RIGHT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.LAZUL:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Madam_Lazul_Popup_BrassRing_Quote.prefab:5fd991c28d0cc7842b99ae3ddb65aa0c", line, TB_EVILBRM.RIGHT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
      case TB_EVILBRM.BOSS.RAFAAM:
        yield return (object) tbEvilbrm.PlayMissionFlavorLine("Rafaam_popup_BrassRing_Quote:187724fae6d64cf49acf11aa53ca2087", line, TB_EVILBRM.RIGHT_OF_ENEMY_HERO, direction, persistCharacter: persistCharacter);
        break;
    }
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        this.matchResult = TB_EVILBRM.VICTOR.PLAYERWIN;
        break;
      case TAG_PLAYSTATE.LOST:
        this.matchResult = TB_EVILBRM.VICTOR.PLAYERLOST;
        break;
      case TAG_PLAYSTATE.TIED:
        this.matchResult = TB_EVILBRM.VICTOR.ERROR;
        break;
    }
    base.NotifyOfGameOver(gameResult);
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    this.enemyPlayer = GameState.Get().GetOpposingSidePlayer();
    this.currentSelectedBoss = this.enemyPlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2);
    this.isOnRagnaros = this.enemyPlayer.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
    Debug.Log((object) ("isRagnaros returns " + (object) this.isOnRagnaros));
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(5f);
    GameState.Get().SetBusy(false);
    if (this.isOnRagnaros == 1)
    {
      switch (this.matchResult)
      {
        case TB_EVILBRM.VICTOR.PLAYERWIN:
          if (this.currentSelectedBoss == 1)
          {
            yield return (object) new WaitForSeconds(1.5f);
            yield return (object) this.PlayBossLineLeft(TB_EVILBRM.BOSS.BOOM, (string) TB_EVILBRM.VO_DrBoom_Male_Goblin_BRM_Victory_01);
          }
          if (this.currentSelectedBoss == 2)
          {
            yield return (object) new WaitForSeconds(1.5f);
            yield return (object) this.PlayBossLineLeft(TB_EVILBRM.BOSS.HAGATHA, (string) TB_EVILBRM.VO_Hagatha_Female_Orc_BRM_Victory_01);
          }
          if (this.currentSelectedBoss == 3)
          {
            yield return (object) new WaitForSeconds(1.5f);
            yield return (object) this.PlayBossLineLeft(TB_EVILBRM.BOSS.LAZUL, (string) TB_EVILBRM.VO_MadameLazul_Female_Troll_BRM_Victory_01);
          }
          if (this.currentSelectedBoss == 4)
          {
            yield return (object) new WaitForSeconds(1.5f);
            yield return (object) this.PlayBossLineLeft(TB_EVILBRM.BOSS.TOGWAGGLE, (string) TB_EVILBRM.VO_Togwaggle_Male_Kobold_BRM_Victory_01);
          }
          if (this.currentSelectedBoss != 5)
            break;
          yield return (object) new WaitForSeconds(1.5f);
          yield return (object) this.PlayBossLineLeft(TB_EVILBRM.BOSS.RAFAAM, (string) TB_EVILBRM.VO_Rafaam_Male_Ethereal_HM_Victory_01);
          break;
      }
    }
  }

  private IEnumerator ShowPopup(
    string stringID,
    float popupDuration,
    Vector3 popUpPos,
    float popupScale)
  {
    this.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.GetTextScale() * popupScale, GameStrings.Get(stringID), false);
    NotificationManager.Get().DestroyNotification(this.m_popup, popupDuration);
    yield return (object) new WaitForSeconds(0.0f);
  }

  public TB_EVILBRM()
    : base()
  {
  }

  public struct PopupMessage
  {
    public string Message;
    public float Delay;
  }

  private enum BOSS
  {
    BOOM,
    HAGATHA,
    TOGWAGGLE,
    LAZUL,
    RAFAAM,
  }

  private enum VICTOR
  {
    PLAYERLOST,
    PLAYERWIN,
    ERROR,
  }
}
