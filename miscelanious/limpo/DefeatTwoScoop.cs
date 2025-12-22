using System.Collections;
using UnityEngine;

public class DefeatTwoScoop : EndGameTwoScoop
{
  public GameObject m_rightTrumpet;
  public GameObject m_rightBanner;
  public GameObject m_rightBannerShred;
  public GameObject m_rightCloud;
  public GameObject m_leftTrumpet;
  public GameObject m_leftBanner;
  public GameObject m_leftBannerFront;
  public GameObject m_leftCloud;
  public GameObject m_crown;
  public GameObject m_defeatBanner;

  protected override void ShowImpl()
  {
    Entity hero = GameState.Get().GetFriendlySidePlayer().GetHero();
    if (hero != null)
    {
      this.m_heroActor.SetFullDefFromEntity(hero);
      this.m_heroActor.UpdateAllComponents();
    }
    this.m_heroActor.TurnOffCollider();
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    string screenBannerText = gameEntity.GetDefeatScreenBannerText();
    if (GameMgr.Get().LastGameData.GameResult == TAG_PLAYSTATE.TIED)
      screenBannerText = gameEntity.GetTieScreenBannerText();
    this.SetBannerLabel(screenBannerText);
    this.GetComponent<PlayMakerFSM>().SendEvent("Action");
    iTween.FadeTo(this.gameObject, 1f, 0.25f);
    this.gameObject.transform.localScale = new Vector3(EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL), (object) "time", (object) 0.5f, (object) "oncomplete", (object) "PunchEndGameTwoScoop", (object) "oncompletetarget", (object) this.gameObject, (object) "easetype", (object) iTween.EaseType.easeOutBounce));
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.gameObject.transform.position + new Vector3(0.005f, 0.005f, 0.005f)), (object) "time", (object) 1.5f, (object) "oncomplete", (object) "TokyoDriftTo", (object) "oncompletetarget", (object) this.gameObject));
    this.AnimateCrownTo();
    this.AnimateLeftTrumpetTo();
    this.AnimateRightTrumpetTo();
    this.StartCoroutine(this.AnimateAll());
    this.m_heroActor.LegendaryHeroPortrait?.RaiseAnimationEvent("OnDefeatTwoScoop");
  }

  protected override void ResetPositions()
  {
    this.gameObject.transform.localPosition = EndGameTwoScoop.START_POSITION;
    this.gameObject.transform.eulerAngles = new Vector3(0.0f, 180f, 0.0f);
    this.m_rightTrumpet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    this.m_rightTrumpet.transform.localEulerAngles = new Vector3(0.0f, -180f, 0.0f);
    this.m_leftTrumpet.transform.localEulerAngles = new Vector3(0.0f, -180f, 0.0f);
    this.m_rightBanner.transform.localScale = new Vector3(1f, 1f, -0.0375f);
    this.m_rightBannerShred.transform.localScale = new Vector3(1f, 1f, 0.05f);
    this.m_rightCloud.transform.localPosition = new Vector3(-0.036f, -0.3f, 0.46f);
    this.m_leftCloud.transform.localPosition = new Vector3(-0.047f, -0.3f, 0.41f);
    this.m_crown.transform.localEulerAngles = new Vector3(-0.026f, 17f, 0.2f);
    this.m_defeatBanner.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
  }

  private IEnumerator AnimateAll()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DefeatTwoScoop defeatTwoScoop = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Hashtable args1 = iTween.Hash((object) "scale", (object) new Vector3(1f, 1f, 1.1f), (object) "time", (object) 0.25f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
      iTween.ScaleTo(defeatTwoScoop.m_rightTrumpet, args1);
      Hashtable args2 = iTween.Hash((object) "z", (object) 1, (object) "delay", (object) 0.5f, (object) "time", (object) 1f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic);
      iTween.ScaleTo(defeatTwoScoop.m_rightBanner, args2);
      Hashtable args3 = iTween.Hash((object) "z", (object) 1, (object) "delay", (object) 0.5f, (object) "time", (object) 1f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic);
      iTween.ScaleTo(defeatTwoScoop.m_rightBannerShred, args3);
      Hashtable args4 = iTween.Hash((object) "x", (object) -0.81f, (object) "time", (object) 5, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "CloudTo", (object) "oncompletetarget", (object) defeatTwoScoop.gameObject);
      iTween.MoveTo(defeatTwoScoop.m_rightCloud, args4);
      Hashtable args5 = iTween.Hash((object) "x", (object) 0.824f, (object) "time", (object) 5, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutCubic);
      iTween.MoveTo(defeatTwoScoop.m_leftCloud, args5);
      Hashtable args6 = iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 183f, 0.0f), (object) "time", (object) 0.5f, (object) "delay", (object) 0.75f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
      iTween.RotateTo(defeatTwoScoop.m_defeatBanner, args6);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(0.25f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void AnimateLeftTrumpetTo() => iTween.RotateTo(this.m_leftTrumpet, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -184f, 0.0f), (object) "time", (object) 5f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutCirc, (object) "oncomplete", (object) "AnimateLeftTrumpetFro", (object) "oncompletetarget", (object) this.gameObject));

  private void AnimateLeftTrumpetFro() => iTween.RotateTo(this.m_leftTrumpet, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -180f, 0.0f), (object) "time", (object) 5f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutCirc, (object) "oncomplete", (object) "AnimateLeftTrumpetTo", (object) "oncompletetarget", (object) this.gameObject));

  private void AnimateRightTrumpetTo() => iTween.RotateTo(this.m_rightTrumpet, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -172f, 0.0f), (object) "time", (object) 8f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutCirc, (object) "oncomplete", (object) "AnimateRightTrumpetFro", (object) "oncompletetarget", (object) this.gameObject));

  private void AnimateRightTrumpetFro() => iTween.RotateTo(this.m_rightTrumpet, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -180f, 0.0f), (object) "time", (object) 8f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutCirc, (object) "oncomplete", (object) "AnimateRightTrumpetTo", (object) "oncompletetarget", (object) this.gameObject));

  private void TokyoDriftTo() => iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (EndGameTwoScoop.START_POSITION + new Vector3(0.2f, 0.2f, 0.2f)), (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "TokyoDriftFro", (object) "oncompletetarget", (object) this.gameObject));

  private void TokyoDriftFro() => iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) EndGameTwoScoop.START_POSITION, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "TokyoDriftTo", (object) "oncompletetarget", (object) this.gameObject));

  private void CloudTo()
  {
    iTween.MoveTo(this.m_rightCloud, iTween.Hash((object) "x", (object) -0.38f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "CloudFro", (object) "oncompletetarget", (object) this.gameObject));
    iTween.MoveTo(this.m_leftCloud, iTween.Hash((object) "x", (object) 0.443f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  private void CloudFro()
  {
    iTween.MoveTo(this.m_rightCloud, iTween.Hash((object) "x", (object) -0.81f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "CloudTo", (object) "oncompletetarget", (object) this.gameObject));
    iTween.MoveTo(this.m_leftCloud, iTween.Hash((object) "x", (object) 0.824f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  private void AnimateCrownTo() => iTween.RotateTo(this.m_crown, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 1.8f, 0.0f), (object) "time", (object) 0.75f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "AnimateCrownFro", (object) "oncompletetarget", (object) this.gameObject));

  private void AnimateCrownFro() => iTween.RotateTo(this.m_crown, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 17f, 0.0f), (object) "time", (object) 0.75f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "AnimateCrownTo", (object) "oncompletetarget", (object) this.gameObject));
}
