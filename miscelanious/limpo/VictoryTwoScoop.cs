using System.Collections;
using UnityEngine;

public class VictoryTwoScoop : EndGameTwoScoop
{
  public GameObject m_godRays;
  public GameObject m_godRays2;
  public GameObject m_rightTrumpet;
  public GameObject m_rightBanner;
  public GameObject m_rightCloud;
  public GameObject m_rightLaurel;
  public GameObject m_leftTrumpet;
  public GameObject m_leftBanner;
  public GameObject m_leftCloud;
  public GameObject m_leftLaurel;
  public GameObject m_crown;
  public AudioSource m_fireworksAudio;
  private const float GOD_RAY_ANGLE = 20f;
  private const float GOD_RAY_DURATION = 20f;
  private const float LAUREL_ROTATION = 2f;
  protected EntityDef m_overrideHeroEntityDef;
  protected DefLoader.DisposableCardDef m_overrideHeroCardDef;

  public void StopFireworksAudio()
  {
    if (!((Object) this.m_fireworksAudio != (Object) null))
      return;
    SoundManager.Get().Stop(this.m_fireworksAudio);
  }

  public void SetOverrideHero(EntityDef overrideHero)
  {
    if (overrideHero != null)
    {
      if (!overrideHero.IsHero())
      {
        Log.Gameplay.PrintError("VictoryTwoScoop.SetOverrideHero() - passed EntityDef {0} is not a hero!", (object) overrideHero);
      }
      else
      {
        DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(overrideHero.GetCardId());
        if (cardDef == null)
        {
          Log.Gameplay.PrintError("VictoryTwoScoop.SetOverrideHero() - passed EntityDef {0} does not have a CardDef!", (object) overrideHero);
        }
        else
        {
          this.m_overrideHeroEntityDef = overrideHero;
          this.m_overrideHeroCardDef?.Dispose();
          this.m_overrideHeroCardDef = cardDef;
        }
      }
    }
    else
    {
      this.m_overrideHeroEntityDef = (EntityDef) null;
      this.m_overrideHeroCardDef?.Dispose();
      this.m_overrideHeroCardDef = (DefLoader.DisposableCardDef) null;
    }
  }

  public override void OnDestroy()
  {
    this.m_overrideHeroCardDef?.Dispose();
    this.m_overrideHeroCardDef = (DefLoader.DisposableCardDef) null;
    base.OnDestroy();
  }

  protected override void ShowImpl()
  {
    this.SetupHeroActor();
    this.SetupBannerText();
    this.PlayShowAnimations();
  }

  protected override void ResetPositions()
  {
    this.gameObject.transform.localPosition = EndGameTwoScoop.START_POSITION;
    this.gameObject.transform.eulerAngles = new Vector3(0.0f, 180f, 0.0f);
    if ((Object) this.m_rightTrumpet != (Object) null)
    {
      this.m_rightTrumpet.transform.localPosition = new Vector3(0.23f, -0.6f, 0.16f);
      this.m_rightTrumpet.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    if ((Object) this.m_leftTrumpet != (Object) null)
    {
      this.m_leftTrumpet.transform.localPosition = new Vector3(-0.23f, -0.6f, 0.16f);
      this.m_leftTrumpet.transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    if ((Object) this.m_rightBanner != (Object) null)
      this.m_rightBanner.transform.localScale = new Vector3(1f, 1f, 0.08f);
    if ((Object) this.m_leftBanner != (Object) null)
      this.m_leftBanner.transform.localScale = new Vector3(1f, 1f, 0.08f);
    if ((Object) this.m_rightCloud != (Object) null)
      this.m_rightCloud.transform.localPosition = new Vector3(-0.2f, -0.8f, 0.26f);
    if ((Object) this.m_leftCloud != (Object) null)
      this.m_leftCloud.transform.localPosition = new Vector3(0.16f, -0.8f, 0.2f);
    if ((Object) this.m_godRays != (Object) null)
      this.m_godRays.transform.localEulerAngles = new Vector3(0.0f, 29f, 0.0f);
    if ((Object) this.m_godRays2 != (Object) null)
      this.m_godRays2.transform.localEulerAngles = new Vector3(0.0f, -29f, 0.0f);
    if ((Object) this.m_crown != (Object) null)
      this.m_crown.transform.localPosition = new Vector3(-0.041f, -0.04f, -0.834f);
    if ((Object) this.m_rightLaurel != (Object) null)
    {
      this.m_rightLaurel.transform.localEulerAngles = new Vector3(0.0f, -90f, 0.0f);
      this.m_rightLaurel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
    if (!((Object) this.m_leftLaurel != (Object) null))
      return;
    this.m_leftLaurel.transform.localEulerAngles = new Vector3(0.0f, 90f, 0.0f);
    this.m_leftLaurel.transform.localScale = new Vector3(-0.7f, 0.7f, 0.7f);
  }

  public override void StopAnimating()
  {
    this.StopCoroutine("AnimateAll");
    iTween.Stop(this.gameObject, true);
    this.StartCoroutine(this.ResetPositionsForGoldEvent());
  }

  protected void SetupHeroActor()
  {
    if (this.m_overrideHeroEntityDef != null && this.m_overrideHeroCardDef != null)
    {
      this.m_heroActor.SetEntityDef(this.m_overrideHeroEntityDef);
      this.m_heroActor.SetCardDef(this.m_overrideHeroCardDef);
      this.m_heroActor.UpdateAllComponents();
    }
    else
    {
      Entity hero = GameState.Get().GetFriendlySidePlayer().GetHero();
      if (hero != null)
      {
        this.m_heroActor.SetFullDefFromEntity(hero);
        this.m_heroActor.UpdateAllComponents();
      }
    }
    this.m_heroActor.TurnOffCollider();
  }

  protected void SetupBannerText() => this.SetBannerLabel(GameState.Get().GetGameEntity().GetVictoryScreenBannerText());

  protected void PlayShowAnimations()
  {
    this.GetComponent<PlayMakerFSM>().SendEvent("Action");
    iTween.FadeTo(this.gameObject, 1f, 0.25f);
    this.gameObject.transform.localScale = new Vector3(EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL), (object) "time", (object) 0.5f, (object) "oncomplete", (object) "PunchEndGameTwoScoop", (object) "oncompletetarget", (object) this.gameObject));
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.gameObject.transform.position + new Vector3(0.005f, 0.005f, 0.005f)), (object) "time", (object) 1.5f, (object) "oncomplete", (object) "TokyoDriftTo", (object) "oncompletetarget", (object) this.gameObject));
    this.AnimateGodraysTo();
    this.AnimateCrownTo();
    this.StartCoroutine(this.AnimateAll());
    this.m_heroActor.LegendaryHeroPortrait?.RaiseAnimationEvent("OnVictoryTwoScoop");
  }

  private IEnumerator AnimateAll()
  {
    // ISSUE: reference to a compiler-generated field
    int num1 = this.\u003C\u003E1__state;
    VictoryTwoScoop victoryTwoScoop = this;
    if (num1 != 0)
    {
      if (num1 != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      float num2 = 0.4f;
      Hashtable args1 = iTween.Hash((object) "position", (object) new Vector3(-0.52f, -0.6f, -0.23f), (object) "time", (object) num2, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic);
      iTween.MoveTo(victoryTwoScoop.m_rightTrumpet, args1);
      Hashtable args2 = iTween.Hash((object) "position", (object) new Vector3(0.44f, -0.6f, -0.23f), (object) "time", (object) num2, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic);
      iTween.MoveTo(victoryTwoScoop.m_leftTrumpet, args2);
      Hashtable args3 = iTween.Hash((object) "scale", (object) new Vector3(1.1f, 1.1f, 1.1f), (object) "time", (object) 0.25f, (object) "delay", (object) 0.3f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
      iTween.ScaleTo(victoryTwoScoop.m_rightTrumpet, args3);
      Hashtable args4 = iTween.Hash((object) "scale", (object) new Vector3(-1.1f, 1.1f, 1.1f), (object) "time", (object) 0.25f, (object) "delay", (object) 0.3f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
      iTween.ScaleTo(victoryTwoScoop.m_leftTrumpet, args4);
      Hashtable args5 = iTween.Hash((object) "z", (object) 1, (object) "delay", (object) 0.24f, (object) "time", (object) 1f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic);
      iTween.ScaleTo(victoryTwoScoop.m_rightBanner, args5);
      Hashtable args6 = iTween.Hash((object) "z", (object) 1, (object) "delay", (object) 0.24f, (object) "time", (object) 1f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic);
      iTween.ScaleTo(victoryTwoScoop.m_leftBanner, args6);
      Hashtable args7 = iTween.Hash((object) "x", (object) -1.227438, (object) "time", (object) 5, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "CloudTo", (object) "oncompletetarget", (object) victoryTwoScoop.gameObject);
      iTween.MoveTo(victoryTwoScoop.m_rightCloud, args7);
      Hashtable args8 = iTween.Hash((object) "x", (object) 1.053244, (object) "time", (object) 5, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutCubic);
      iTween.MoveTo(victoryTwoScoop.m_leftCloud, args8);
      Hashtable args9 = iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 2f, 0.0f), (object) "time", (object) 0.5f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic, (object) "oncomplete", (object) "LaurelWaveTo", (object) "oncompletetarget", (object) victoryTwoScoop.gameObject);
      iTween.RotateTo(victoryTwoScoop.m_rightLaurel, args9);
      Hashtable args10 = iTween.Hash((object) "scale", (object) new Vector3(1f, 1f, 1f), (object) "time", (object) 0.25f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
      iTween.ScaleTo(victoryTwoScoop.m_rightLaurel, args10);
      Hashtable args11 = iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -2f, 0.0f), (object) "time", (object) 0.5f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic);
      iTween.RotateTo(victoryTwoScoop.m_leftLaurel, args11);
      Hashtable args12 = iTween.Hash((object) "scale", (object) new Vector3(-1f, 1f, 1f), (object) "time", (object) 0.25f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutBounce);
      iTween.ScaleTo(victoryTwoScoop.m_leftLaurel, args12);
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

  protected void TokyoDriftTo() => iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (EndGameTwoScoop.START_POSITION + new Vector3(0.2f, 0.2f, 0.2f)), (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "TokyoDriftFro", (object) "oncompletetarget", (object) this.gameObject));

  private void TokyoDriftFro() => iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) EndGameTwoScoop.START_POSITION, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "TokyoDriftTo", (object) "oncompletetarget", (object) this.gameObject));

  private void CloudTo()
  {
    iTween.MoveTo(this.m_rightCloud, iTween.Hash((object) "x", (object) -0.92f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "CloudFro", (object) "oncompletetarget", (object) this.gameObject));
    iTween.MoveTo(this.m_leftCloud, iTween.Hash((object) "x", (object) 0.82f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  private void CloudFro()
  {
    iTween.MoveTo(this.m_rightCloud, iTween.Hash((object) "x", (object) -1.227438f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "CloudTo", (object) "oncompletetarget", (object) this.gameObject));
    iTween.MoveTo(this.m_leftCloud, iTween.Hash((object) "x", (object) 1.053244f, (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  private void LaurelWaveTo()
  {
    iTween.RotateTo(this.m_rightLaurel, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "LaurelWaveFro", (object) "oncompletetarget", (object) this.gameObject));
    iTween.RotateTo(this.m_leftLaurel, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  private void LaurelWaveFro()
  {
    iTween.RotateTo(this.m_rightLaurel, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 2f, 0.0f), (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "LaurelWaveTo", (object) "oncompletetarget", (object) this.gameObject));
    iTween.RotateTo(this.m_leftLaurel, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -2f, 0.0f), (object) "time", (object) 10, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  protected void AnimateCrownTo() => iTween.MoveTo(this.m_crown, iTween.Hash((object) "z", (object) -0.78f, (object) "time", (object) 5, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutBack, (object) "oncomplete", (object) "AnimateCrownFro", (object) "oncompletetarget", (object) this.gameObject));

  private void AnimateCrownFro() => iTween.MoveTo(this.m_crown, iTween.Hash((object) "z", (object) -0.834f, (object) "time", (object) 5, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutBack, (object) "oncomplete", (object) "AnimateCrownTo", (object) "oncompletetarget", (object) this.gameObject));

  protected void AnimateGodraysTo()
  {
    iTween.RotateTo(this.m_godRays, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -20f, 0.0f), (object) "time", (object) 20f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "AnimateGodraysFro", (object) "oncompletetarget", (object) this.gameObject));
    iTween.RotateTo(this.m_godRays2, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 20f, 0.0f), (object) "time", (object) 20f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  private void AnimateGodraysFro()
  {
    iTween.RotateTo(this.m_godRays, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 20f, 0.0f), (object) "time", (object) 20f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "AnimateGodraysTo", (object) "oncompletetarget", (object) this.gameObject));
    iTween.RotateTo(this.m_godRays2, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -20f, 0.0f), (object) "time", (object) 20f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }

  private IEnumerator ResetPositionsForGoldEvent()
  {
    yield return (object) new WaitForEndOfFrame();
    yield return (object) new WaitForEndOfFrame();
    float num = 0.25f;
    iTween.MoveTo(this.m_rightCloud, iTween.Hash((object) "position", (object) new Vector3(-1.211758f, -0.8f, -0.2575677f), (object) "time", (object) num, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
    iTween.MoveTo(this.m_leftCloud, iTween.Hash((object) "position", (object) new Vector3(1.068925f, -0.8f, -0.197469f), (object) "time", (object) num, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
    this.m_rightLaurel.transform.localRotation = Quaternion.Euler(Vector3.zero);
    iTween.MoveTo(this.m_rightLaurel, iTween.Hash((object) "position", (object) new Vector3(0.1723f, -0.206f, 0.753f), (object) "time", (object) num, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
    this.m_leftLaurel.transform.localRotation = Quaternion.Euler(Vector3.zero);
    iTween.MoveTo(this.m_leftLaurel, iTween.Hash((object) "position", (object) new Vector3(-0.2201783f, -0.318f, 0.753f), (object) "time", (object) num, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
    iTween.MoveTo(this.m_crown, iTween.Hash((object) "z", (object) -0.9677765f, (object) "time", (object) num, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutBack));
    iTween.RotateTo(this.m_godRays, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 20f, 0.0f), (object) "time", (object) num, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
    iTween.RotateTo(this.m_godRays2, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, -20f, 0.0f), (object) "time", (object) num, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear));
  }
}
