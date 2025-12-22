using System.Collections;
using UnityEngine;

public class TB01_RagVsNef : MissionEntity
{
  private Card m_ragnarosCard;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    if (missionEvent == 1)
    {
      foreach (Entity entity in GameState.Get().GetPlayerMap().Values)
      {
        Entity hero = entity.GetHero();
        Card card = hero.GetCard();
        if (hero.GetCardId() == "TBA01_1")
          this.m_ragnarosCard = card;
      }
      GameState.Get().SetBusy(true);
      CardSoundSpell cardSoundSpell = this.m_ragnarosCard.PlayEmote(EmoteType.THREATEN);
      if ((Object) cardSoundSpell.m_CardSoundData.m_AudioSource == (Object) null || (Object) cardSoundSpell.m_CardSoundData.m_AudioSource.clip == (Object) null)
      {
        GameState.Get().SetBusy(false);
      }
      else
      {
        yield return (object) new WaitForSeconds(cardSoundSpell.m_CardSoundData.m_AudioSource.clip.length * 0.8f);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public TB01_RagVsNef()
    : base()
  {
  }
}
