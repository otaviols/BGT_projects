using System.Collections;
using UnityEngine;

public class TB_Firefest3 : MissionEntity
{
  private Actor headActor;
  private Card headCard;
  private static readonly AssetReference VO_Rakanishu_Male_Elemental_FF_Start_02 = new AssetReference("VO_Rakanishu_Male_Elemental_FF_Start_02.prefab:8985db50d3217a349812bd24624db30d");

  public override void PreloadAssets() => this.PreloadSound((string) TB_Firefest3.VO_Rakanishu_Male_Elemental_FF_Start_02);

  private void GetHorsemanHead()
  {
    int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
    if (tag == 0)
      return;
    Entity entity = GameState.Get().GetEntity(tag);
    if (entity != null)
      this.headCard = entity.GetCard();
    if (!((Object) this.headCard != (Object) null))
      return;
    this.headActor = this.headCard.GetActor();
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_Firefest3 tbFirefest3 = this;
    if (missionEvent == 15)
      tbFirefest3.GetHorsemanHead();
    while (tbFirefest3.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent != 15)
      tbFirefest3.GetHorsemanHead();
    if (missionEvent == 10)
    {
      Gameplay.Get().StartCoroutine(tbFirefest3.PlaySoundAndBlockSpeech((string) TB_Firefest3.VO_Rakanishu_Male_Elemental_FF_Start_02, Notification.SpeechBubbleDirection.TopRight, tbFirefest3.headActor));
      GameState.Get().SetBusy(true);
      yield return (object) new WaitForSeconds(5f);
      GameState.Get().SetBusy(false);
    }
  }

  public TB_Firefest3()
    : base()
  {
  }
}
