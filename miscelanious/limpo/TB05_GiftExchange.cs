using System.Collections;
using UnityEngine;

public class TB05_GiftExchange : MissionEntity
{
  private string[] GiftVOList = new string[4]
  {
    "VO_TB_1503_FATHER_WINTER_GIFT1.prefab:4ae447402c1a6044584b4c310ec9ac54",
    "VO_TB_1503_FATHER_WINTER_GIFT2.prefab:03e080e95e177f448b422251a62b3534",
    "VO_TB_1503_FATHER_WINTER_GIFT3.prefab:83f8d60f0a3375845a3d3d8256c77998",
    "VO_TB_1503_FATHER_WINTER_GIFT4.prefab:0696e29df4374fe44998942aa3b98fdd"
  };
  private string[] PissedVOList = new string[5]
  {
    "VO_TB_1503_FATHER_WINTER_LONG2.prefab:3229f3d88d7ef9f45b3b5e5314f59f62",
    "VO_TB_1503_FATHER_WINTER_LONG3.prefab:7e768808510998e48b5df809cbf33c0c",
    "VO_TB_1503_FATHER_WINTER_LONG4.prefab:b10dc2bd2c73ce44e9d5dca4d4bba2c1",
    "VO_TB_1503_FATHER_WINTER_LONG5.prefab:b2bb544f1972fd546bd6fdba7f0b8aab",
    "VO_TB_1503_FATHER_WINTER_LONG6.prefab:ef76d11be6f4e4244905e125d5064438"
  };
  private string FirstGiftVO = "VO_TB_1503_FATHER_WINTER_GIFT1.prefab:4ae447402c1a6044584b4c310ec9ac54";
  private string StartVO = "VO_TB_1503_FATHER_WINTER_LONG6.prefab:ef76d11be6f4e4244905e125d5064438";
  private string FirstStolenVO = "VO_TB_1503_FATHER_WINTER_START.prefab:71392d50932d2bb4f93ad5f687f229dd";
  private string NextStolenVO = "VO_TB_1503_FATHER_WINTER_LONG1.prefab:3f1fe4e70cbaf3a40960431138ef961e";
  private string VOChoice;
  private float delayTime;
  private Notification GiftStolenPopup;
  private Notification GiftSpawnedPopup;
  private Notification GameStartPopup;
  private string textID;
  private Vector3 popUpPos;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT1.prefab:4ae447402c1a6044584b4c310ec9ac54");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT2.prefab:03e080e95e177f448b422251a62b3534");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT3.prefab:83f8d60f0a3375845a3d3d8256c77998");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT4.prefab:0696e29df4374fe44998942aa3b98fdd");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG1.prefab:3f1fe4e70cbaf3a40960431138ef961e");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG2.prefab:3229f3d88d7ef9f45b3b5e5314f59f62");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG3.prefab:7e768808510998e48b5df809cbf33c0c");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG4.prefab:b10dc2bd2c73ce44e9d5dca4d4bba2c1");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG5.prefab:b2bb544f1972fd546bd6fdba7f0b8aab");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG6.prefab:ef76d11be6f4e4244905e125d5064438");
    this.PreloadSound("VO_TB_1503_FATHER_WINTER_START.prefab:71392d50932d2bb4f93ad5f687f229dd");
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB05_GiftExchange tb05GiftExchange = this;
    tb05GiftExchange.VOChoice = "";
    tb05GiftExchange.delayTime = 0.0f;
    while (tb05GiftExchange.m_enemySpeaking)
      yield return (object) null;
    switch (missionEvent)
    {
      case 1:
        if (tb05GiftExchange.FirstGiftVO.Length > 0)
        {
          tb05GiftExchange.VOChoice = tb05GiftExchange.FirstGiftVO;
          tb05GiftExchange.FirstGiftVO = "";
          tb05GiftExchange.delayTime = 4f;
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(0.5f);
          GameState.Get().SetBusy(false);
          tb05GiftExchange.textID = "TB_GIFTEXCHANGE_GIFTSPAWNED";
          tb05GiftExchange.popUpPos = new Vector3(1.27f, 0.0f, -9.32f);
          if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
            tb05GiftExchange.popUpPos.z = 19f;
          float num = 1.25f;
          if ((bool) UniversalInputManager.UsePhoneUI)
            num = 1.75f;
          tb05GiftExchange.GiftSpawnedPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb05GiftExchange.popUpPos, TutorialEntity.GetTextScale() * num, GameStrings.Get(tb05GiftExchange.textID), false);
          NotificationManager.Get().DestroyNotification(tb05GiftExchange.GiftSpawnedPopup, 4f);
          break;
        }
        tb05GiftExchange.VOChoice = tb05GiftExchange.GiftVOList[Random.Range(1, tb05GiftExchange.GiftVOList.Length)];
        tb05GiftExchange.delayTime = 3f;
        break;
      case 2:
        tb05GiftExchange.VOChoice = tb05GiftExchange.PissedVOList[Random.Range(1, tb05GiftExchange.PissedVOList.Length)];
        tb05GiftExchange.delayTime = 2f;
        break;
      case 10:
        tb05GiftExchange.VOChoice = tb05GiftExchange.StartVO;
        tb05GiftExchange.delayTime = 5f;
        tb05GiftExchange.textID = "TB_GIFTEXCHANGE_START";
        tb05GiftExchange.popUpPos = new Vector3(22.2f, 0.0f, -44.6f);
        tb05GiftExchange.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
        tb05GiftExchange.GameStartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb05GiftExchange.popUpPos, TutorialEntity.GetTextScale() * 1.75f, GameStrings.Get(tb05GiftExchange.textID), false);
        NotificationManager.Get().DestroyNotification(tb05GiftExchange.GameStartPopup, 3f);
        break;
      case 11:
        if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
        {
          if (tb05GiftExchange.FirstStolenVO.Length > 0)
          {
            tb05GiftExchange.VOChoice = tb05GiftExchange.FirstStolenVO;
            tb05GiftExchange.FirstStolenVO = "";
            yield return (object) new WaitForSeconds(1.5f);
            tb05GiftExchange.delayTime = 4f;
            tb05GiftExchange.textID = "TB_GIFTEXCHANGE_GIFTSTOLEN";
            tb05GiftExchange.popUpPos = new Vector3(22.2f, 0.0f, -44.6f);
            if ((bool) UniversalInputManager.UsePhoneUI)
            {
              tb05GiftExchange.popUpPos.x = 61f;
              tb05GiftExchange.popUpPos.z = -29f;
            }
            tb05GiftExchange.GiftStolenPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tb05GiftExchange.popUpPos, TutorialEntity.GetTextScale() * 1.25f, GameStrings.Get(tb05GiftExchange.textID), false);
            NotificationManager.Get().DestroyNotification(tb05GiftExchange.GiftStolenPopup, 4f);
            break;
          }
          break;
        }
        if (tb05GiftExchange.NextStolenVO.Length > 0)
        {
          tb05GiftExchange.VOChoice = tb05GiftExchange.NextStolenVO;
          tb05GiftExchange.NextStolenVO = "";
          break;
        }
        break;
    }
    tb05GiftExchange.PlaySound(tb05GiftExchange.VOChoice);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(tb05GiftExchange.delayTime);
    GameState.Get().SetBusy(false);
  }

  public TB05_GiftExchange()
    : base()
  {
  }
}
