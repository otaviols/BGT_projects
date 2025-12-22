using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_100th : MissionEntity
{
  private Notification StartPopup;
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[2]
      {
        "TB_100TH_RAVENIDOL_YOU",
        "TB_100TH_RAVENIDOL_OPP"
      }
    },
    {
      11,
      new string[2]
      {
        "TB_100TH_RANDOMONIUM_YOU",
        "TB_100TH_RANDOMONIUM_OPP"
      }
    },
    {
      12,
      new string[2]
      {
        "TB_100TH_BANANA_YOU",
        "TB_100TH_BANANA_OPP"
      }
    },
    {
      13,
      new string[2]{ "TB_100TH_YOGG_YOU", "TB_100TH_YOGG_OPP" }
    },
    {
      14,
      new string[2]
      {
        "TB_100TH_PARTYPORTAL_YOU",
        "TB_100TH_PARTYPORTAL_OPP"
      }
    },
    {
      15,
      new string[2]
      {
        "TB_100TH_CLONEBALL_YOU",
        "TB_100TH_CLONEBALL_OPP"
      }
    },
    {
      16,
      new string[2]
      {
        "TB_100TH_SPELLBAG_YOU",
        "TB_100TH_SPELLBAG_OPP"
      }
    },
    {
      17,
      new string[2]
      {
        "TB_100TH_PORTALS_YOU",
        "TB_100TH_PORTALS_OPP"
      }
    },
    {
      18,
      new string[2]
      {
        "TB_100TH_SHIFTER_YOU",
        "TB_100TH_SHIFTER_OPP"
      }
    },
    {
      20,
      new string[2]
      {
        "TB_100TH_RAVENIDOL_INFO_YOU",
        "TB_100TH_RAVENIDOL_INFO_OPP"
      }
    },
    {
      21,
      new string[2]
      {
        "TB_100TH_RANDEMONIUM_INFO_YOU",
        "TB_100TH_RANDEMONIUM_INFO_OPP"
      }
    },
    {
      22,
      new string[2]
      {
        "TB_100TH_BANANA_INFO_YOU",
        "TB_100TH_BANANA_INFO_OPP"
      }
    },
    {
      23,
      new string[2]
      {
        "TB_100TH_YOGG_INFO_YOU",
        "TB_100TH_YOGG_INFO_OPP"
      }
    },
    {
      24,
      new string[2]
      {
        "TB_100TH_PARTYPORTAL_INFO_YOU",
        "TB_100TH_PARTYPORTAL_INFO_OPP"
      }
    },
    {
      25,
      new string[2]
      {
        "TB_100TH_CLONEBALL_INFO_YOU",
        "TB_100TH_CLONEBALL_INFO_OPP"
      }
    },
    {
      26,
      new string[2]
      {
        "TB_100TH_SPELLBAG_INFO_YOU",
        "TB_100TH_SPELLBAG_INFO_OPP"
      }
    },
    {
      27,
      new string[2]
      {
        "TB_100TH_PORTALS_INFO_YOU",
        "TB_100TH_PORTALS_INFO_OPP"
      }
    },
    {
      28,
      new string[2]
      {
        "TB_100TH_SHIFTER_INFO_YOU",
        "TB_100TH_SHIFTER_INFO_OPP"
      }
    }
  };
  private Dictionary<int, string[]> m_medivaInfo = new Dictionary<int, string[]>()
  {
    {
      41,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_AranHeroPower_04.prefab:9bca13673c70fbe4e856fa6cadb44d32",
        "VO_TB_MEDIVA3_001"
      }
    },
    {
      42,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_AranSpell_06.prefab:7ee6ed9064bae8b4cae9b616322befa2",
        "VO_TB_MEDIVA3_002"
      }
    },
    {
      43,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_AranSpell_01.prefab:391ba56a5ee5e0e4b9098147ac14d7bc",
        "VO_TB_MEDIVA3_003"
      }
    },
    {
      44,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_ChessEmoteOops_01.prefab:4133c951e3b700242b5747520ab4cf2c",
        "VO_TB_MEDIVA3_004"
      }
    },
    {
      45,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_CuratorBeasts_01.prefab:bd55c69fcafba594b984a9d7792e43e9",
        "VO_TB_MEDIVA3_005"
      }
    },
    {
      46,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_IllhoofWounded_01.prefab:6eac5d210aeee3048957194c8f610ceb",
        "VO_TB_MEDIVA3_006"
      }
    },
    {
      47,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_IllhoofWounded_04.prefab:e774579b014394548914674ee10897e6",
        "VO_TB_MEDIVA3_007"
      }
    },
    {
      48,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_JulianneDeadlyPoison_02.prefab:3facdbce34af76a438defff4a56c6ce2",
        "VO_TB_MEDIVA3_008"
      }
    },
    {
      49,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_PrologueTurn5_03.prefab:ee16361b04c4daa419e0006a5b5949e8",
        "VO_TB_MEDIVA3_009"
      }
    },
    {
      50,
      new string[2]
      {
        "VO_Mediva_03_Male_Night Elf_PrologueTurn5_02.prefab:71fd5f307f54d174e939f2aada7af9bf",
        "VO_TB_MEDIVA3_010"
      }
    }
  };
  private bool[] last_time_emoted;
  private Vector3 popUpPos;
  private Player friendlySidePlayer;

  private void SetPopupPosition()
  {
    if (this.friendlySidePlayer.IsCurrentPlayer())
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.popUpPos.z = -66f;
      else
        this.popUpPos.z = -44f;
    }
    else if ((bool) UniversalInputManager.UsePhoneUI)
      this.popUpPos.z = 66f;
    else
      this.popUpPos.z = 44f;
  }

  private Actor GetMedivaActor()
  {
    int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
    Entity entity = GameState.Get().GetEntity(tag);
    if (entity == null)
      return (Actor) null;
    Card card = entity.GetCard();
    return (Object) card == (Object) null ? (Actor) null : card.GetActor();
  }

  public override void PreloadAssets()
  {
    this.last_time_emoted = new bool[100];
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_AranHeroPower_04.prefab:9bca13673c70fbe4e856fa6cadb44d32");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_AranSpell_06.prefab:7ee6ed9064bae8b4cae9b616322befa2");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_AranSpell_01.prefab:391ba56a5ee5e0e4b9098147ac14d7bc");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_ChessEmoteOops_01.prefab:4133c951e3b700242b5747520ab4cf2c");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_CuratorBeasts_01.prefab:bd55c69fcafba594b984a9d7792e43e9");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_IllhoofWounded_01.prefab:6eac5d210aeee3048957194c8f610ceb");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_IllhoofWounded_04.prefab:e774579b014394548914674ee10897e6");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_JulianneDeadlyPoison_02.prefab:3facdbce34af76a438defff4a56c6ce2");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_PrologueTurn5_03.prefab:ee16361b04c4daa419e0006a5b5949e8");
    this.PreloadSound("VO_Mediva_03_Male_Night Elf_PrologueTurn5_02.prefab:71fd5f307f54d174e939f2aada7af9bf");
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_100th tb100th = this;
    tb100th.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    while (tb100th.m_enemySpeaking)
      yield return (object) null;
    tb100th.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
    tb100th.SetPopupPosition();
    if (tb100th.m_popUpInfo.ContainsKey(missionEvent))
      yield return (object) tb100th.ShowPopup(tb100th.friendlySidePlayer.IsCurrentPlayer() ? tb100th.m_popUpInfo[missionEvent][0] : tb100th.m_popUpInfo[missionEvent][1]);
    else if (tb100th.m_medivaInfo.ContainsKey(missionEvent))
      yield return (object) tb100th.ShowMediva(missionEvent, tb100th.m_medivaInfo[missionEvent][0], tb100th.m_medivaInfo[missionEvent][1]);
  }

  private IEnumerator ShowPopup(string displayString)
  {
    this.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.GetTextScale(), GameStrings.Get(displayString), false);
    NotificationManager.Get().DestroyNotification(this.StartPopup, 7f);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(2f);
    GameState.Get().SetBusy(false);
  }

  private IEnumerator ShowMediva(int missionEvent, string soundPath, string stringName)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_100th tb100th = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Actor medivaActor = tb100th.GetMedivaActor();
    if ((Object) medivaActor == (Object) null)
    {
      Debug.LogWarningFormat("No Mediva actor found for mission: {0} in TB_100th.", (object) missionEvent);
      return false;
    }
    if (!tb100th.last_time_emoted[missionEvent])
    {
      GameState.Get().SetBusy(true);
      Gameplay.Get().StartCoroutine(tb100th.PlaySoundAndBlockSpeechWithCustomGameString(soundPath, stringName, Notification.SpeechBubbleDirection.TopRight, medivaActor));
      GameState.Get().SetBusy(false);
      tb100th.last_time_emoted[missionEvent] = true;
    }
    return false;
  }

  public TB_100th()
    : base()
  {
  }
}
