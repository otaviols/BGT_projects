using System.Collections;
using UnityEngine;

public class TB_HeadlessHorseman : MissionEntity
{
  private Actor horsemanActor;
  private Card horsemanCard;
  private Actor headActor;
  private Card headCard;
  private bool isHeadInPlay;
  private bool isHorsemanInPlay;
  private bool hasSpoken;
  private static readonly AssetReference VO_CS2_222_Attack_02 = new AssetReference("VO_CS2_222_Attack_02.prefab:c3191e3764f78654899b70a311936b93");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_01 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_01.prefab:95ede70d25607fd47923f829d4e5de42");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_02 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_02.prefab:35efdbdae6db14745bb99a6bf351634a");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_04 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_04.prefab:6324e43e11bdb2448ac1de3c8d07d048");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_06 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_06.prefab:36e1b30c8aceaf04992bc6cf0959d9c6");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_07 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_07.prefab:b88173866204fce4da7cbb3f7dddc915");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_08 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_08.prefab:dc0854d9ad190c84dbbea8f4b0f99dfe");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_10 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_10.prefab:92570580735ed754589290f4df5058bb");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_12 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_12.prefab:5b588aa7f8994a04eaf048ca87d73807");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_13 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_13.prefab:a015bfc61fca6a0489f276e3e2fbb0a3");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_17 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_17.prefab:84d6d0029d5f72542926291be8e40b39");
  private static readonly AssetReference VO_HeadlessHorseman_Male_Human_HallowsEve_19 = new AssetReference("VO_HeadlessHorseman_Male_Human_HallowsEve_19.prefab:74a92ec2af554f94fb8c6e205c561bde");
  private Vector3 popUpPos;
  private Notification StartPopup;
  private int _announcerLinesPlayed;
  private int _fireballDialogDelay;
  private bool _hasPlayed14;
  private bool _hasPlayed16;
  private bool _hasPlayed17;
  private bool _hasPlayed18;

  public override void PreloadAssets()
  {
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_01);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_02);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_04);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_06);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_07);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_08);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_10);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_12);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_13);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_17);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_19);
    this.PreloadSound((string) TB_HeadlessHorseman.VO_CS2_222_Attack_02);
  }

  private void GetHorsemanHead()
  {
    this.isHeadInPlay = false;
    int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
    if (tag == 0)
      return;
    Entity entity = GameState.Get().GetEntity(tag);
    if (entity != null)
      this.isHeadInPlay = entity.GetZone() == TAG_ZONE.PLAY;
    if (entity != null)
      this.headCard = entity.GetCard();
    if (!((Object) this.headCard != (Object) null))
      return;
    this.headActor = this.headCard.GetActor();
  }

  private void GetHorseman()
  {
    int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_2);
    Entity entity = GameState.Get().GetEntity(tag);
    if (tag == 0)
      return;
    if (entity != null)
    {
      this.horsemanCard = entity.GetCard();
      this.isHorsemanInPlay = entity.GetZone() == TAG_ZONE.PLAY;
    }
    if (!((Object) this.horsemanCard != (Object) null))
      return;
    this.horsemanActor = this.horsemanCard.GetActor();
  }

  public override AudioSource GetAnnouncerLine(
    Card heroCard,
    Card.AnnouncerLineType type)
  {
    ++this._announcerLinesPlayed;
    switch (this._announcerLinesPlayed)
    {
      case 1:
        return this.GetPreloadedSound((string) TB_HeadlessHorseman.VO_CS2_222_Attack_02);
      case 2:
        return this.GetPreloadedSound((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_19);
      default:
        return base.GetAnnouncerLine(heroCard, type);
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_HeadlessHorseman headlessHorseman = this;
    if (missionEvent == 15)
    {
      headlessHorseman.GetHorsemanHead();
      headlessHorseman.GetHorseman();
    }
    while (headlessHorseman.m_enemySpeaking)
      yield return (object) null;
    if (!headlessHorseman.hasSpoken || missionEvent == 99)
    {
      if (missionEvent != 15)
      {
        headlessHorseman.GetHorsemanHead();
        headlessHorseman.GetHorseman();
      }
      switch (missionEvent)
      {
        case 10:
          Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_04, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.horsemanActor));
          yield return (object) new WaitForSeconds(2f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 11:
          if (headlessHorseman.isHeadInPlay)
            Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_08, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.headActor));
          else if (headlessHorseman.isHorsemanInPlay)
            Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_08, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.horsemanActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(5f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 12:
          ++headlessHorseman._fireballDialogDelay;
          if (headlessHorseman._fireballDialogDelay <= 1 || headlessHorseman._fireballDialogDelay > 2)
            break;
          Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_02, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.headActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2.5f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 13:
          Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_10, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.horsemanActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(4f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 14:
          if (headlessHorseman._hasPlayed14)
            break;
          headlessHorseman._hasPlayed14 = true;
          if (headlessHorseman.isHeadInPlay)
          {
            Debug.LogWarning((object) headlessHorseman.isHeadInPlay);
            Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_13, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.headActor));
          }
          else if (headlessHorseman.isHorsemanInPlay)
            Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_13, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.horsemanActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(4f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 15:
          Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_17, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.headActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(3f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 16:
          if (!headlessHorseman.isHeadInPlay || !headlessHorseman.isHorsemanInPlay || headlessHorseman._hasPlayed16)
            break;
          headlessHorseman._hasPlayed16 = true;
          Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_01, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.headActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(5f);
          GameState.Get().SetBusy(false);
          Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_10, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.horsemanActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 17:
          if (headlessHorseman._hasPlayed17)
            break;
          headlessHorseman._hasPlayed17 = true;
          if (headlessHorseman.isHeadInPlay)
            Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_12, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.headActor));
          else if (headlessHorseman.isHorsemanInPlay)
            Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_12, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.horsemanActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(3f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 18:
          if (headlessHorseman._hasPlayed18)
            break;
          headlessHorseman._hasPlayed18 = true;
          Gameplay.Get().StartCoroutine(headlessHorseman.PlaySoundAndBlockSpeech((string) TB_HeadlessHorseman.VO_HeadlessHorseman_Male_Human_HallowsEve_07, Notification.SpeechBubbleDirection.TopRight, headlessHorseman.headActor));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(3f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 20:
          headlessHorseman.popUpPos = new Vector3(0.0f, 0.0f, 0.0f);
          headlessHorseman.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, headlessHorseman.popUpPos, TutorialEntity.GetTextScale(), GameStrings.Get("TB_HEADLESS_HORSEMAN_POISON"), false);
          NotificationManager.Get().DestroyNotification(headlessHorseman.StartPopup, 5f);
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2f);
          GameState.Get().SetBusy(false);
          headlessHorseman.hasSpoken = true;
          break;
        case 99:
          headlessHorseman.hasSpoken = false;
          break;
      }
    }
  }

  public TB_HeadlessHorseman()
    : base()
  {
  }
}
