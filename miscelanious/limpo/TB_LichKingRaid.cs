using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_LichKingRaid : MissionEntity
{
  private Notification StartPopup;
  private Vector3 popUpPos;
  private Card m_LichKingCard;
  private Actor m_LichKingActor;
  private bool once = true;
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      10,
      new string[1]{ "FB_LK_INTRO_01" }
    },
    {
      11,
      new string[1]{ "FB_LK_INTRO_02" }
    },
    {
      12,
      new string[1]{ "FB_LK_HEROSWITCH" }
    },
    {
      13,
      new string[1]{ "FB_LK_BOSSSWITCH" }
    },
    {
      14,
      new string[1]{ "FB_LK_DEAD_01" }
    },
    {
      15,
      new string[1]{ "FB_LK_DEAD_02" }
    }
  };

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LichKing_Male_Human_Brawl_01.prefab:df6d7692c0d3d8c4aab91a2eec0a3d9f");
    this.PreloadSound("VO_LichKing_Male_Human_Brawl_03.prefab:12fd2a3bd4b0945448667db58a95f32b");
    this.PreloadSound("VO_LichKing_Male_Human_Brawl_05.prefab:96e0b55b99289824ebbae0d6201e936c");
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICCLichKing);

  private Card GetLichKing()
  {
    foreach (Entity entity in GameState.Get().GetPlayerMap().Values)
    {
      Entity hero = entity.GetHero();
      Card card = hero.GetCard();
      if (hero.GetCardId() == "FB_LK_Raid_Hero")
        return card;
    }
    return (Card) null;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_LichKingRaid tbLichKingRaid = this;
    while (tbLichKingRaid.m_enemySpeaking)
      yield return (object) null;
    tbLichKingRaid.m_LichKingCard = tbLichKingRaid.GetLichKing();
    if ((Object) tbLichKingRaid.m_LichKingCard != (Object) null)
      tbLichKingRaid.m_LichKingActor = tbLichKingRaid.m_LichKingCard.GetActor();
    NameBanner nameBannerForSide = Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING);
    switch (missionEvent)
    {
      case 1:
        if (!tbLichKingRaid.once || !((Object) tbLichKingRaid.m_LichKingCard != (Object) null))
          break;
        tbLichKingRaid.once = false;
        GameState.Get().SetBusy(true);
        Notification.SpeechBubbleDirection direction1 = tbLichKingRaid.m_LichKingActor.GetEntity().IsControlledByFriendlySidePlayer() ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopRight;
        Gameplay.Get().StartCoroutine(tbLichKingRaid.PlaySoundAndBlockSpeech("VO_LichKing_Male_Human_Brawl_01.prefab:df6d7692c0d3d8c4aab91a2eec0a3d9f", direction1, tbLichKingRaid.m_LichKingActor));
        GameState.Get().SetBusy(false);
        break;
      case 3:
        if (!((Object) tbLichKingRaid.m_LichKingCard != (Object) null))
          break;
        GameState.Get().SetBusy(true);
        Notification.SpeechBubbleDirection direction2 = tbLichKingRaid.m_LichKingActor.GetEntity().IsControlledByFriendlySidePlayer() ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopRight;
        Gameplay.Get().StartCoroutine(tbLichKingRaid.PlaySoundAndBlockSpeech("VO_LichKing_Male_Human_Brawl_03.prefab:12fd2a3bd4b0945448667db58a95f32b", direction2, tbLichKingRaid.m_LichKingActor));
        GameState.Get().SetBusy(false);
        break;
      case 5:
        if (!((Object) tbLichKingRaid.m_LichKingCard != (Object) null))
          break;
        tbLichKingRaid.once = false;
        GameState.Get().SetBusy(true);
        Notification.SpeechBubbleDirection direction3 = tbLichKingRaid.m_LichKingActor.GetEntity().IsControlledByFriendlySidePlayer() ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopRight;
        Gameplay.Get().StartCoroutine(tbLichKingRaid.PlaySoundAndBlockSpeech("VO_LichKing_Male_Human_Brawl_05.prefab:96e0b55b99289824ebbae0d6201e936c", direction3, tbLichKingRaid.m_LichKingActor));
        yield return (object) new WaitForSeconds(2f);
        GameState.Get().SetBusy(false);
        break;
      case 10:
        if (!((Object) tbLichKingRaid.m_LichKingCard != (Object) null) || !tbLichKingRaid.m_popUpInfo.ContainsKey(missionEvent))
          break;
        yield return (object) tbLichKingRaid.ShowPopup(tbLichKingRaid.m_popUpInfo[missionEvent][0]);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(6f);
        GameState.Get().SetBusy(false);
        yield return (object) tbLichKingRaid.ShowPopup(tbLichKingRaid.m_popUpInfo[11][0]);
        break;
      case 11:
        if (!((Object) tbLichKingRaid.m_LichKingCard != (Object) null) || !tbLichKingRaid.m_popUpInfo.ContainsKey(missionEvent))
          break;
        yield return (object) tbLichKingRaid.ShowPopup(tbLichKingRaid.m_popUpInfo[missionEvent][0]);
        break;
      case 12:
        if (!((Object) tbLichKingRaid.m_LichKingCard != (Object) null) || !tbLichKingRaid.m_popUpInfo.ContainsKey(missionEvent))
          break;
        yield return (object) tbLichKingRaid.ShowPopup(tbLichKingRaid.m_popUpInfo[missionEvent][0]);
        break;
      case 13:
        if (!((Object) tbLichKingRaid.m_LichKingCard != (Object) null) || !tbLichKingRaid.m_popUpInfo.ContainsKey(missionEvent))
          break;
        yield return (object) tbLichKingRaid.ShowPopup(tbLichKingRaid.m_popUpInfo[missionEvent][0]);
        break;
      case 14:
        if (!((Object) tbLichKingRaid.m_LichKingCard != (Object) null) || !tbLichKingRaid.m_popUpInfo.ContainsKey(missionEvent))
          break;
        yield return (object) tbLichKingRaid.ShowPopup(tbLichKingRaid.m_popUpInfo[missionEvent][0]);
        GameState.Get().SetBusy(true);
        yield return (object) new WaitForSeconds(6f);
        GameState.Get().SetBusy(false);
        yield return (object) tbLichKingRaid.ShowPopup(tbLichKingRaid.m_popUpInfo[15][0]);
        break;
      case 20:
        if (!((Object) nameBannerForSide != (Object) null))
          break;
        nameBannerForSide.SetName(GameStrings.Get("FB_LK_HERO_02"));
        break;
      case 21:
        if (!((Object) nameBannerForSide != (Object) null))
          break;
        nameBannerForSide.SetName(GameStrings.Get("FB_LK_HERO_03"));
        break;
      case 22:
        if (!((Object) nameBannerForSide != (Object) null))
          break;
        nameBannerForSide.SetName(GameStrings.Get("FB_LK_HERO_04"));
        break;
      case 23:
        if (!((Object) nameBannerForSide != (Object) null))
          break;
        nameBannerForSide.SetName(GameStrings.Get("FB_LK_HERO_05"));
        break;
      case 24:
        if (!((Object) nameBannerForSide != (Object) null))
          break;
        nameBannerForSide.SetName(GameStrings.Get("FB_LK_HERO_01"));
        break;
    }
  }

  private IEnumerator ShowPopup(string displayString)
  {
    this.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.GetTextScale() * 1.5f, GameStrings.Get(displayString), false);
    NotificationManager.Get().DestroyNotification(this.StartPopup, 7f);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(2f);
    GameState.Get().SetBusy(false);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public TB_LichKingRaid()
    : base()
  {
  }
}
