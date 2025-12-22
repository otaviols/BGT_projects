using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBattlegroundQuestReward : Zone
{
  public bool m_isHeroPower;
  private const float INTERMEDIATE_Y_OFFSET = 1.5f;
  private const float INTERMEDIATE_TRANSITION_SEC = 0.9f;
  private const float DESTROYED_QUEST_REWARD_WAIT_SEC = 1.75f;
  private const float FINAL_TRANSITION_SEC = 0.1f;
  private List<Card> m_destroyedQuestRewards = new List<Card>();

  public override string ToString() => string.Format("{0} (Battleground quest reward)", (object) base.ToString());

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    return base.CanAcceptTags(controllerId, zoneTag, cardType, entity) && cardType == TAG_CARDTYPE.BATTLEGROUND_QUEST_REWARD && this.m_isHeroPower == (entity.GetTag(GAME_TAG.BACON_IS_HEROPOWER_QUESTREWARD) != 0);
  }

  public override int RemoveCard(Card card)
  {
    int num = base.RemoveCard(card);
    if (num < 0 || this.m_destroyedQuestRewards.Contains(card))
      return num;
    this.m_destroyedQuestRewards.Add(card);
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
      this.m_destroyedQuestRewards.Clear();
      this.UpdateLayoutFinished();
    }
    else
      this.StartCoroutine(this.UpdateLayoutImpl());
  }

  private IEnumerator UpdateLayoutImpl()
  {
    ZoneBattlegroundQuestReward battlegroundQuestReward = this;
    Card activeBgQuestReward = battlegroundQuestReward.m_cards[0];
    while (activeBgQuestReward.IsDoNotSort())
      yield return (object) null;
    activeBgQuestReward.ShowCard();
    activeBgQuestReward.EnableTransitioningZones(true);
    string tweenName = ZoneMgr.Get().GetTweenName<ZoneBattlegroundQuestReward>();
    if (battlegroundQuestReward.m_Side == Player.Side.OPPOSING)
      iTween.StopOthersByName(activeBgQuestReward.gameObject, tweenName);
    Vector3 position = battlegroundQuestReward.transform.position;
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
    iTween.MoveTo(activeBgQuestReward.gameObject, iTween.Hash(objArray1));
    object[] objArray2 = new object[6]
    {
      (object) "name",
      (object) tweenName,
      (object) "rotation",
      (object) battlegroundQuestReward.transform.localEulerAngles,
      (object) "time",
      (object) 0.9f
    };
    iTween.RotateTo(activeBgQuestReward.gameObject, iTween.Hash(objArray2));
    object[] objArray3 = new object[6]
    {
      (object) "name",
      (object) tweenName,
      (object) "scale",
      (object) battlegroundQuestReward.transform.localScale,
      (object) "time",
      (object) 0.9f
    };
    iTween.ScaleTo(activeBgQuestReward.gameObject, iTween.Hash(objArray3));
    yield return (object) new WaitForSeconds(0.9f);
    if (battlegroundQuestReward.m_destroyedQuestRewards.Count > 0)
      yield return (object) new WaitForSeconds(1.75f);
    battlegroundQuestReward.m_destroyedQuestRewards.Clear();
    object[] objArray4 = new object[8]
    {
      (object) "position",
      (object) battlegroundQuestReward.transform.position,
      (object) "time",
      (object) 0.1f,
      (object) "easetype",
      (object) iTween.EaseType.easeOutCubic,
      (object) "name",
      (object) tweenName
    };
    iTween.MoveTo(activeBgQuestReward.gameObject, iTween.Hash(objArray4));
    battlegroundQuestReward.StartFinishLayoutTimer(0.1f);
  }
}
