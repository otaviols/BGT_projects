using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBattlegroundHeroBuddy : Zone
{
  private const float INTERMEDIATE_Y_OFFSET = 1.5f;
  private const float INTERMEDIATE_TRANSITION_SEC = 0.9f;
  private const float DESTROYED_HERO_BUDDY_WAIT_SEC = 1.75f;
  private const float FINAL_TRANSITION_SEC = 0.1f;
  private List<Card> m_destroyedHeroBuddies = new List<Card>();

  public override string ToString() => string.Format("{0} (Battleground hero Buddy)", (object) base.ToString());

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return base.CanAcceptTags(controllerId, zoneTag, cardType, entity) && cardType == TAG_CARDTYPE.BATTLEGROUND_HERO_BUDDY;
  }

  public override int RemoveCard(Card card)
  {
    int num = base.RemoveCard(card);
    if (num < 0 || this.m_destroyedHeroBuddies.Contains(card))
      return num;
    this.m_destroyedHeroBuddies.Add(card);
    return num;
  }

  public override void UpdateLayout()
  {
    ++this.m_updatingLayout;
    if (GameState.Get().IsMulliganManagerActive())
      this.UpdateLayoutFinished();
    else if (this.IsBlockingLayout())
      this.UpdateLayoutFinished();
    else if (this.m_cards.Count == 0)
    {
      this.m_destroyedHeroBuddies.Clear();
      this.UpdateLayoutFinished();
    }
    else
      this.StartCoroutine(this.UpdateLayoutImpl());
  }

  private IEnumerator UpdateLayoutImpl()
  {
    ZoneBattlegroundHeroBuddy battlegroundHeroBuddy = this;
    Card activeBgHeroBuddy = battlegroundHeroBuddy.m_cards[0];
    while (activeBgHeroBuddy.IsDoNotSort())
      yield return (object) null;
    activeBgHeroBuddy.ShowCard();
    activeBgHeroBuddy.EnableTransitioningZones(true);
    string tweenName = ZoneMgr.Get().GetTweenName<ZoneBattlegroundHeroBuddy>();
    if (battlegroundHeroBuddy.m_Side == Player.Side.OPPOSING)
      iTween.StopOthersByName(activeBgHeroBuddy.gameObject, tweenName);
    Vector3 position = battlegroundHeroBuddy.transform.position;
    position.y += 1.5f;
    object[] objArray1 = new object[6]
    {
      (object) "name",
      (object) tweenName,
      (object) "position",
      (object) position,
      (object) "time",
      (object) 0.9f
    };
    iTween.MoveTo(activeBgHeroBuddy.gameObject, iTween.Hash(objArray1));
    object[] objArray2 = new object[6]
    {
      (object) "name",
      (object) tweenName,
      (object) "rotation",
      (object) battlegroundHeroBuddy.transform.localEulerAngles,
      (object) "time",
      (object) 0.9f
    };
    iTween.RotateTo(activeBgHeroBuddy.gameObject, iTween.Hash(objArray2));
    object[] objArray3 = new object[6]
    {
      (object) "name",
      (object) tweenName,
      (object) "scale",
      (object) battlegroundHeroBuddy.transform.localScale,
      (object) "time",
      (object) 0.9f
    };
    iTween.ScaleTo(activeBgHeroBuddy.gameObject, iTween.Hash(objArray3));
    yield return (object) new WaitForSeconds(0.9f);
    if (battlegroundHeroBuddy.m_destroyedHeroBuddies.Count > 0)
      yield return (object) new WaitForSeconds(1.75f);
    battlegroundHeroBuddy.m_destroyedHeroBuddies.Clear();
    object[] objArray4 = new object[8]
    {
      (object) "position",
      (object) battlegroundHeroBuddy.transform.position,
      (object) "time",
      (object) 0.1f,
      (object) "easetype",
      (object) iTween.EaseType.easeOutCubic,
      (object) "name",
      (object) tweenName
    };
    iTween.MoveTo(activeBgHeroBuddy.gameObject, iTween.Hash(objArray4));
    battlegroundHeroBuddy.StartFinishLayoutTimer(0.1f);
  }
}
