using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB14_DPromo : MissionEntity
{
  private Notification MyPopup;
  private Vector3 popUpPos;
  private string textID;
  private bool doPopup;
  private bool doLeftArrow;
  private bool doUpArrow;
  private bool doDownArrow;
  private float delayTime;
  private float popupDuration = 2.5f;
  private float popupScale = 2.5f;
  private HashSet<int> seen = new HashSet<int>();
  private static readonly Dictionary<int, string> minionMsgs = new Dictionary<int, string>()
  {
    {
      10,
      "TB_DPROMO_2NDHEROPOPUP"
    },
    {
      11,
      "TB_DPROMO_2NDHEROPOPUP"
    }
  };

  public override void PreloadAssets()
  {
    this.PreloadSound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
    this.PreloadSound("CowKing_TB_SPT_DPromo_Hero2_Play.wav:d4884afd0de894f618c37a00901e0258");
    this.PreloadSound("CowKing_TB_SPT_DPromo_Hero2_Death.wav:96e41f1a7ed1747e0b8ca7feb8312585");
    this.PreloadSound("HellBovine_TB_SPT_DPromoMinion_Attack.wav:e0b94995a3c774aaf86c35c2f6f9968f");
    this.PreloadSound("HellBovine_TB_SPT_DPromoMinion_Death.wav:7c64102817d15435a9319ca137fb4d5a");
    this.PreloadSound("HellBovine_TB_SPT_DPromoMinion_Play.wav:22be52fa77e13486ab76a4266aa1a815");
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB14_DPromo tb14Dpromo = this;
    while (tb14Dpromo.m_enemySpeaking)
      yield return (object) null;
    if (!tb14Dpromo.seen.Contains(missionEvent))
    {
      tb14Dpromo.seen.Add(missionEvent);
      tb14Dpromo.doPopup = false;
      tb14Dpromo.doLeftArrow = false;
      tb14Dpromo.doUpArrow = false;
      tb14Dpromo.doDownArrow = false;
      tb14Dpromo.delayTime = 0.0f;
      tb14Dpromo.popupDuration = 2.5f;
      if (missionEvent == 100)
      {
        NotificationManager.Get().DestroyNotification(tb14Dpromo.MyPopup, 0.0f);
        tb14Dpromo.doPopup = false;
      }
      else
      {
        tb14Dpromo.doPopup = true;
        tb14Dpromo.textID = !TB14_DPromo.minionMsgs.ContainsKey(missionEvent) ? TB14_DPromo.minionMsgs[2] : TB14_DPromo.minionMsgs[missionEvent];
        tb14Dpromo.popUpPos.x = 0.0f;
        tb14Dpromo.popUpPos.z = 4f;
        int num = (bool) UniversalInputManager.UsePhoneUI ? 1 : 0;
        if (missionEvent == 10)
          Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName(GameStrings.Get("TB_DPROMO_2NDHERO"));
        if (missionEvent == 11)
          Gameplay.Get().GetNameBannerForSide(Player.Side.FRIENDLY).SetName(GameStrings.Get("TB_DPROMO_2NDHERO"));
      }
      if (tb14Dpromo.doPopup)
      {
        if (missionEvent == 1)
        {
          tb14Dpromo.delayTime = 5f;
          tb14Dpromo.popupDuration = 5f;
        }
        yield return (object) new WaitForSeconds(tb14Dpromo.delayTime);
        tb14Dpromo.MyPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb14Dpromo.popUpPos, TutorialEntity.GetTextScale() * tb14Dpromo.popupScale, GameStrings.Get(tb14Dpromo.textID), false);
        if (tb14Dpromo.doLeftArrow)
          tb14Dpromo.MyPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        if (tb14Dpromo.doUpArrow)
          tb14Dpromo.MyPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
        if (tb14Dpromo.doDownArrow)
          tb14Dpromo.MyPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
        tb14Dpromo.PlaySound("tutorial_mission_hero_coin_mouse_away.prefab:6266be3ca0b50a645915b9ea0a59d774");
        NotificationManager.Get().DestroyNotification(tb14Dpromo.MyPopup, tb14Dpromo.popupDuration);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public TB14_DPromo()
    : base()
  {
  }
}
