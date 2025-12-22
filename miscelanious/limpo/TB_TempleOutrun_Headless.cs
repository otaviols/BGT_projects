using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_TempleOutrun_Headless : ULDA_Dungeon
{
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HHIntro_02 = new AssetReference("VO_HeadlessHorseman_Male_Human_HHIntro_02.prefab:0dc446d089c1c6142819ecd89009e9bf");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HHReaction1_01 = new AssetReference("VO_HeadlessHorseman_Male_Human_HHReaction1_01.prefab:8443a7874cc9cbb48a30f57d69e1b431");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_19 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_19.prefab:74a92ec2af554f94fb8c6e205c561bde");
  private List<string> m_HeroPowerLines = new List<string>();
  private List<string> m_IdleLines = new List<string>();
  private Notification m_popup;
  private float popupScale = 1.4f;
  private static readonly Dictionary<int, TB_TempleOutrun_Headless.PopupMessage> popupMsgs = new Dictionary<int, TB_TempleOutrun_Headless.PopupMessage>()
  {
    {
      1000,
      new TB_TempleOutrun_Headless.PopupMessage()
      {
        Message = "TB_EVILBRM_CURRENT_BEST_SCORE",
        Delay = 5f
      }
    },
    {
      2000,
      new TB_TempleOutrun_Headless.PopupMessage()
      {
        Message = "TB_EVILBRM_NEW_BEST_SCORE",
        Delay = 5f
      }
    }
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) TB_TempleOutrun_Headless.VO_HeadlessHorseman_Male_Human_HHIntro_02,
      (string) TB_TempleOutrun_Headless.VO_HeadlessHorseman_Male_Human_HHReaction1_01,
      (string) TB_TempleOutrun_Headless.VO_HeadlessHorseman_Male_Human_HallowsEve_19
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) TB_TempleOutrun_Headless.VO_HeadlessHorseman_Male_Human_HallowsEve_19;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.None, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_TempleOutrun_Headless templeOutrunHeadless = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    while (templeOutrunHeadless.m_enemySpeaking)
      yield return (object) null;
    switch (missionEvent)
    {
      case 100:
        Debug.Log((object) "Got Case 100");
        Gameplay.Get().StartCoroutine(templeOutrunHeadless.PlaySoundAndBlockSpeech((string) TB_TempleOutrun_Headless.VO_HeadlessHorseman_Male_Human_HHIntro_02, Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 101:
        Debug.Log((object) "Got Case 101");
        Gameplay.Get().StartCoroutine(templeOutrunHeadless.PlaySoundAndBlockSpeech((string) TB_TempleOutrun_Headless.VO_HeadlessHorseman_Male_Human_HHReaction1_01, Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 1010:
        Debug.Log((object) "Got Case 1010");
        int tag = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
        string msgString;
        if (tag == 0)
        {
          msgString = GameStrings.Get(TB_TempleOutrun_Headless.popupMsgs[2000].Message);
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
          msgString = GameStrings.Get(TB_TempleOutrun_Headless.popupMsgs[missionEvent].Message) + "\n" + str1 + (object) num1 + ":" + str2 + (object) num2 + ":" + str3 + (object) num3;
          templeOutrunHeadless.popupScale = 1.7f;
        }
        Vector3 popUpPos = new Vector3();
        popUpPos.z = GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer() ? ((bool) UniversalInputManager.UsePhoneUI ? -40f : -40f) : ((bool) UniversalInputManager.UsePhoneUI ? 27f : 18f);
        yield return (object) new WaitForSeconds(4f);
        yield return (object) templeOutrunHeadless.ShowPopup(msgString, TB_TempleOutrun_Headless.popupMsgs[missionEvent].Delay, popUpPos, templeOutrunHeadless.popupScale);
        msgString = (string) null;
        popUpPos = new Vector3();
        break;
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

  public struct PopupMessage
  {
    public string Message;
    public float Delay;
  }
}
