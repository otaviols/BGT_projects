using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneWeapon : Zone
{
  private const float INTERMEDIATE_Y_OFFSET = 1.5f;
  private const float INTERMEDIATE_TRANSITION_SEC = 0.9f;
  private const float DESTROYED_WEAPON_WAIT_SEC = 1.75f;
  private const float FINAL_TRANSITION_SEC = 0.1f;
  private List<Card> m_destroyedWeapons = new List<Card>();

  public override string ToString() => string.Format("{0} (Weapon)", (object) base.ToString());

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return base.CanAcceptTags(controllerId, zoneTag, cardType, entity) && cardType == TAG_CARDTYPE.WEAPON;
  }

  public override int RemoveCard(Card card)
  {
    int num = base.RemoveCard(card);
    if (num < 0 || this.m_destroyedWeapons.Contains(card))
      return num;
    this.m_destroyedWeapons.Add(card);
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
      this.m_destroyedWeapons.Clear();
      this.UpdateLayoutFinished();
    }
    else
      this.StartCoroutine(this.UpdateLayoutImpl());
  }

  private IEnumerator UpdateLayoutImpl()
  {
    ZoneWeapon zoneWeapon = this;
    Card equippedWeapon = zoneWeapon.m_cards[0];
    while (equippedWeapon.IsDoNotSort())
      yield return (object) null;
    equippedWeapon.ShowCard();
    equippedWeapon.EnableTransitioningZones(true);
    string tweenName = ZoneMgr.Get().GetTweenName<ZoneWeapon>();
    if (zoneWeapon.m_Side == Player.Side.OPPOSING)
      iTween.StopOthersByName(equippedWeapon.gameObject, tweenName);
    Vector3 position = zoneWeapon.transform.position;
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
    iTween.MoveTo(equippedWeapon.gameObject, iTween.Hash(objArray1));
    object[] objArray2 = new object[6]
    {
      (object) "name",
      (object) tweenName,
      (object) "rotation",
      (object) zoneWeapon.transform.localEulerAngles,
      (object) "time",
      (object) 0.9f
    };
    iTween.RotateTo(equippedWeapon.gameObject, iTween.Hash(objArray2));
    object[] objArray3 = new object[6]
    {
      (object) "name",
      (object) tweenName,
      (object) "scale",
      (object) zoneWeapon.transform.localScale,
      (object) "time",
      (object) 0.9f
    };
    iTween.ScaleTo(equippedWeapon.gameObject, iTween.Hash(objArray3));
    yield return (object) new WaitForSeconds(0.9f);
    if (zoneWeapon.m_destroyedWeapons.Count > 0)
      yield return (object) new WaitForSeconds(1.75f);
    zoneWeapon.m_destroyedWeapons.Clear();
    object[] objArray4 = new object[8]
    {
      (object) "position",
      (object) zoneWeapon.transform.position,
      (object) "time",
      (object) 0.1f,
      (object) "easetype",
      (object) iTween.EaseType.easeOutCubic,
      (object) "name",
      (object) tweenName
    };
    iTween.MoveTo(equippedWeapon.gameObject, iTween.Hash(objArray4));
    zoneWeapon.StartFinishLayoutTimer(0.1f);
  }
}
