using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB13_LethalPuzzles_Restart : PhasedMissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TB13_LethalPuzzles_Restart.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TB13_LethalPuzzles_Restart.InitStringOptions();
  private Notification m_popup;
  private HashSet<int> m_seen = new HashSet<int>();
  private static readonly Dictionary<int, string> s_minionMsgs = new Dictionary<int, string>()
  {
    {
      1,
      "TB_LETHALPUZZLES_START"
    },
    {
      2,
      "TB_LETHALPUZZLES_SUCCESS"
    },
    {
      3,
      "TB_LETHALPUZZLES_FAILURE"
    }
  };

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public TB13_LethalPuzzles_Restart() => this.m_gameOptions.AddOptions(TB13_LethalPuzzles_Restart.s_booleanOptions, TB13_LethalPuzzles_Restart.s_stringOptions);

  public override void PreloadAssets() => this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");

  public override bool ShouldDoAlternateMulliganIntro() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB13_LethalPuzzles_Restart lethalPuzzlesRestart = this;
    while (lethalPuzzlesRestart.m_enemySpeaking)
      yield return (object) null;
    if (!lethalPuzzlesRestart.m_seen.Contains(missionEvent))
    {
      lethalPuzzlesRestart.m_seen.Add(missionEvent);
      switch (missionEvent)
      {
        case 10:
          NameBanner nameBannerForSide1 = Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING);
          if (!((Object) nameBannerForSide1 != (Object) null))
            break;
          nameBannerForSide1.SetName(GameStrings.Get("TB_LETHAL_NAME"));
          break;
        case 100:
          NotificationManager.Get().DestroyNotification(lethalPuzzlesRestart.m_popup, 0.0f);
          break;
        default:
          if (!TB13_LethalPuzzles_Restart.s_minionMsgs.ContainsKey(missionEvent))
            break;
          string textID = TB13_LethalPuzzles_Restart.s_minionMsgs[missionEvent];
          float seconds = 0.0f;
          float popupDuration = 2.5f;
          float popupScale = 2.5f;
          Vector3 popUpPos = new Vector3(0.0f, 0.0f, 4f);
          if (missionEvent == 1)
          {
            seconds = 5f;
            popupDuration = 5f;
          }
          yield return (object) new WaitForSeconds(seconds);
          lethalPuzzlesRestart.m_popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.GetTextScale() * popupScale, GameStrings.Get(textID), false);
          lethalPuzzlesRestart.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
          NotificationManager.Get().DestroyNotification(lethalPuzzlesRestart.m_popup, popupDuration);
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2f);
          GameState.Get().SetBusy(false);
          NameBanner nameBannerForSide2 = Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING);
          if ((Object) nameBannerForSide2 != (Object) null)
            nameBannerForSide2.SetName(GameStrings.Get("TB_LETHAL_NAME"));
          textID = (string) null;
          popUpPos = new Vector3();
          break;
      }
    }
  }
}
