using System.Collections;
using UnityEngine;

[CustomEditClass]
public class ChessAttackAnimation : Spell
{
  public GameObject m_ChessShockwaveRed;
  public GameObject m_ChessShockwaveBlue;
  public GameObject m_ChessTrailRed;
  public GameObject m_ChessTrailBlue;
  public GameObject m_ChessImpactRed;
  public GameObject m_ChessImpactBlue;
  public GameObject m_ChessSettleDust;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_ShowAttackSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_ShowImpactSoundPrefab;
  public float m_ImpactEffectDelay = 0.3f;
  public float m_SpellFinishDelay = 0.15f;

  protected override void OnAction(SpellStateType prevStateType)
  {
    base.OnAction(prevStateType);
    this.StartCoroutine(this.AttackAnimation());
  }

  private void Finish()
  {
    this.OnSpellFinished();
    this.OnStateFinished();
  }

  private IEnumerator AttackAnimation()
  {
    ChessAttackAnimation chessAttackAnimation = this;
    if (chessAttackAnimation.m_targets.Count == 0)
    {
      chessAttackAnimation.Finish();
    }
    else
    {
      string tweenLabel = ZoneMgr.Get().GetTweenName<ZonePlay>();
      while (iTween.CountByName(chessAttackAnimation.GetSourceCard().gameObject, tweenLabel) > 0)
        yield return (object) null;
      GameObject gameObject1 = chessAttackAnimation.GetSourceCard().gameObject;
      Vector3 position1 = gameObject1.transform.position;
      Vector3 eulerAngles = gameObject1.transform.eulerAngles;
      Vector3 localScale = gameObject1.transform.localScale;
      GameObject gameObject2 = chessAttackAnimation.m_targets[0].gameObject;
      Vector3 position2 = gameObject2.transform.position;
      GameObject gameObject3 = Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessSettleDust);
      Vector3 vector3_1 = new Vector3(gameObject1.transform.localScale.x * 1.2f, gameObject1.transform.localScale.y * 1.2f, gameObject1.transform.localScale.z * 1.2f);
      float num1 = (double) gameObject1.transform.position.z > (double) gameObject2.transform.position.z ? Random.Range(0.65f, 0.85f) : Random.Range(-0.65f, -0.85f);
      float num2 = (double) gameObject1.transform.position.z > (double) gameObject2.transform.position.z ? -0.1f : 0.1f;
      Vector3 vector3_2 = (double) position1.z > (double) position2.z ? new Vector3(eulerAngles.x - 15f, eulerAngles.y, eulerAngles.z) : new Vector3(eulerAngles.x + 15f, eulerAngles.y, eulerAngles.z);
      iTween.MoveTo(gameObject1, iTween.Hash((object) "position", (object) new Vector3(position1.x, position1.y + 1f, position1.z + num1), (object) "easetype", (object) iTween.EaseType.easeOutQuart, (object) "time", (object) 0.15f));
      iTween.ScaleTo(gameObject1, iTween.Hash((object) "scale", (object) vector3_1, (object) "time", (object) 0.2f));
      chessAttackAnimation.StartCoroutine(chessAttackAnimation.DoSpellFinished());
      iTween.RotateTo(gameObject1, iTween.Hash((object) "rotation", (object) vector3_2, (object) "easetype", (object) iTween.EaseType.easeOutQuart, (object) "time", (object) 0.1f, (object) "delay", (object) 0.15f));
      iTween.MoveTo(gameObject1, iTween.Hash((object) "position", (object) new Vector3(position1.x, position1.y + 0.1f, position1.z + num2), (object) "easetype", (object) iTween.EaseType.easeOutQuart, (object) "time", (object) 0.05f, (object) "delay", (object) 0.25f));
      iTween.RotateTo(gameObject1, iTween.Hash((object) "rotation", (object) eulerAngles, (object) "easetype", (object) iTween.EaseType.easeOutQuart, (object) "time", (object) 0.05f, (object) "delay", (object) 0.25f));
      iTween.ScaleTo(gameObject1, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.05f, (object) "delay", (object) 0.25f));
      iTween.MoveTo(gameObject1, iTween.Hash((object) "position", (object) position1, (object) "easetype", (object) iTween.EaseType.easeOutQuart, (object) "time", (object) 0.3f, (object) "delay", (object) 0.25));
      gameObject3.transform.parent = chessAttackAnimation.transform;
      gameObject3.transform.position = new Vector3(position1.x, position1.y + 1f, position1.z);
      gameObject3.GetComponent<ParticleSystem>().Play();
      yield return (object) new WaitForSeconds(chessAttackAnimation.m_ImpactEffectDelay);
      if (!string.IsNullOrEmpty(chessAttackAnimation.m_ShowAttackSoundPrefab))
        SoundManager.Get().LoadAndPlay((AssetReference) chessAttackAnimation.m_ShowAttackSoundPrefab);
      yield return (object) new WaitForSeconds(0.3f);
    }
  }

  private IEnumerator PlayImpactEffects()
  {
    ChessAttackAnimation chessAttackAnimation = this;
    if (chessAttackAnimation.m_targets.Count == 0)
    {
      chessAttackAnimation.Finish();
    }
    else
    {
      GameObject gameObject1 = chessAttackAnimation.GetSourceCard().gameObject;
      GameObject gameObject2 = chessAttackAnimation.m_targets[0].gameObject;
      Vector3 position1 = gameObject2.transform.position;
      Vector3 eulerAngles1 = gameObject2.transform.eulerAngles;
      GameObject target1 = (double) gameObject1.transform.position.z > (double) gameObject2.transform.position.z ? Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessShockwaveRed) : Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessShockwaveBlue);
      ParticleSystem component1 = target1.GetComponent<ParticleSystem>();
      ParticleSystem.MainModule main1 = component1.main;
      GameObject gameObject3 = (double) gameObject1.transform.position.z > (double) gameObject2.transform.position.z ? Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessImpactBlue) : Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessImpactRed);
      ParticleSystem component2 = gameObject3.GetComponent<ParticleSystem>();
      ParticleSystem.MainModule main2 = component2.main;
      float num1 = (double) gameObject1.transform.position.z > (double) gameObject2.transform.position.z ? 0.25f : -0.25f;
      float seconds = 0.15f;
      bool flag = chessAttackAnimation.m_targets.Count == 1 && ((double) gameObject2.transform.position.z < -7.0 || (double) gameObject2.transform.position.z > -2.0);
      float num2 = chessAttackAnimation.m_targets.Count != 2 || (double) gameObject1.transform.position.z <= (double) gameObject2.transform.position.z ? (chessAttackAnimation.m_targets.Count != 1 || (double) gameObject1.transform.position.z <= (double) gameObject2.transform.position.z || (double) Mathf.Abs(gameObject1.transform.position.x) - (double) Mathf.Abs(gameObject2.transform.position.x) >= -0.5 ? (chessAttackAnimation.m_targets.Count != 1 || (double) gameObject1.transform.position.z <= (double) gameObject2.transform.position.z || (double) Mathf.Abs(gameObject1.transform.position.x) - (double) Mathf.Abs(gameObject2.transform.position.x) <= 0.5 ? (chessAttackAnimation.m_targets.Count != 1 || (double) gameObject1.transform.position.z >= (double) gameObject2.transform.position.z || (double) Mathf.Abs(gameObject1.transform.position.x) - (double) Mathf.Abs(gameObject2.transform.position.x) >= -0.5 ? (chessAttackAnimation.m_targets.Count != 1 || (double) gameObject1.transform.position.z >= (double) gameObject2.transform.position.z || (double) Mathf.Abs(gameObject1.transform.position.x) - (double) Mathf.Abs(gameObject2.transform.position.x) <= 0.5 ? (chessAttackAnimation.m_targets.Count != 1 || (double) gameObject1.transform.position.z <= (double) gameObject2.transform.position.z ? 3.14159f : 0.0f) : 3.66519f) : 2.61799f) : -0.523599f) : 0.523599f) : 0.0f;
      target1.transform.parent = chessAttackAnimation.transform;
      gameObject3.transform.parent = chessAttackAnimation.transform;
      target1.transform.position = new Vector3(gameObject1.transform.position.x, gameObject1.transform.position.y + 0.5f, gameObject1.transform.position.z - num1);
      main1.startRotation = (ParticleSystem.MinMaxCurve) num2;
      if (chessAttackAnimation.m_targets.Count == 2)
      {
        main1.startSize = (ParticleSystem.MinMaxCurve) 4f;
        iTween.MoveTo(target1, iTween.Hash((object) "position", (object) new Vector3(gameObject1.transform.position.x, gameObject2.transform.position.y + 0.5f, gameObject2.transform.position.z + num1), (object) "time", (object) 0.4f));
        component1.Play();
      }
      else if (flag)
      {
        GameObject target2 = (double) gameObject1.transform.position.z > (double) gameObject2.transform.position.z ? Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessTrailRed) : Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessTrailBlue);
        target2.transform.parent = chessAttackAnimation.transform;
        target2.transform.position = new Vector3(gameObject1.transform.position.x, gameObject1.transform.position.y + 0.1f, gameObject1.transform.position.z);
        seconds = 0.5f;
        float x = (float) (((double) gameObject1.transform.position.x + (double) gameObject2.transform.position.x) * 0.5);
        float z = (double) gameObject2.transform.position.z > -4.0 ? -2.4f : -6.4f;
        if ((double) gameObject1.transform.position.x + (double) gameObject2.transform.position.x < -17.5 || (double) gameObject1.transform.position.x + (double) gameObject2.transform.position.x > -12.5)
        {
          iTween.MoveTo(target2, iTween.Hash((object) "path", (object) new Vector3[2]
          {
            new Vector3(x, gameObject2.transform.position.y + 2f, z),
            new Vector3(gameObject2.transform.position.x, gameObject2.transform.position.y + 0.1f, gameObject2.transform.position.z)
          }, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "time", (object) 0.4f));
        }
        else
        {
          seconds = 0.4f;
          iTween.MoveTo(target2, iTween.Hash((object) "position", (object) new Vector3(gameObject2.transform.position.x, gameObject2.transform.position.y + 0.5f, gameObject2.transform.position.z), (object) "time", (object) 0.3f));
        }
      }
      else
      {
        iTween.MoveTo(target1, iTween.Hash((object) "position", (object) new Vector3(gameObject2.transform.position.x, gameObject2.transform.position.y + 0.5f, gameObject2.transform.position.z + num1), (object) "time", (object) 0.4f));
        component1.Play();
      }
      gameObject3.transform.position = new Vector3(gameObject2.transform.position.x, gameObject2.transform.position.y + 1f, gameObject2.transform.position.z);
      main2.startDelay = (ParticleSystem.MinMaxCurve) seconds;
      component2.Play();
      if (!flag)
        chessAttackAnimation.ShakeMinion(gameObject2, position1, eulerAngles1);
      if (chessAttackAnimation.m_targets.Count == 2)
      {
        GameObject gameObject4 = chessAttackAnimation.m_targets[1].gameObject;
        Vector3 position2 = gameObject4.transform.position;
        Vector3 eulerAngles2 = gameObject4.transform.eulerAngles;
        GameObject gameObject5 = (double) gameObject1.transform.position.z > (double) gameObject4.transform.position.z ? Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessImpactBlue) : Object.Instantiate<GameObject>(chessAttackAnimation.m_ChessImpactRed);
        gameObject5.transform.parent = chessAttackAnimation.transform;
        gameObject5.transform.position = new Vector3(gameObject4.transform.position.x, gameObject4.transform.position.y + 1f, gameObject4.transform.position.z);
        gameObject5.GetComponent<ParticleSystem>().Play();
        chessAttackAnimation.ShakeMinion(gameObject4, position2, eulerAngles2);
      }
      yield return (object) new WaitForSeconds(seconds);
      if (!string.IsNullOrEmpty(chessAttackAnimation.m_ShowImpactSoundPrefab))
        SoundManager.Get().LoadAndPlay((AssetReference) chessAttackAnimation.m_ShowImpactSoundPrefab);
    }
  }

  private void ShakeMinion(GameObject target, Vector3 targetOrgPos, Vector3 targetOrgRot)
  {
    iTween.MoveTo(target, iTween.Hash((object) "position", (object) new Vector3(targetOrgPos.x, targetOrgPos.y + 0.15f, targetOrgPos.z), (object) "time", (object) 0.05f, (object) "islocal", (object) true));
    iTween.RotateTo(target, iTween.Hash((object) "rotation", (object) new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), Random.Range(-15f, 15f)), (object) "time", (object) 0.08f, (object) "islocal", (object) true));
    iTween.RotateTo(target, iTween.Hash((object) "rotation", (object) new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), (object) "time", (object) 0.08f, (object) "islocal", (object) true, (object) "delay", (object) 0.08f));
    iTween.RotateTo(target, iTween.Hash((object) "rotation", (object) new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), (object) "time", (object) 0.08f, (object) "islocal", (object) true, (object) "delay", (object) 0.16f));
    iTween.MoveTo(target, iTween.Hash((object) "position", (object) targetOrgPos, (object) "time", (object) 0.08f, (object) "islocal", (object) true, (object) "delay", (object) 0.24f));
    iTween.RotateTo(target, iTween.Hash((object) "rotation", (object) targetOrgRot, (object) "time", (object) 0.08f, (object) "islocal", (object) true, (object) "delay", (object) 0.24f));
  }

  private IEnumerator DoSpellFinished()
  {
    ChessAttackAnimation chessAttackAnimation = this;
    if (chessAttackAnimation.m_targets.Count == 0)
    {
      chessAttackAnimation.Finish();
    }
    else
    {
      GameObject source = chessAttackAnimation.GetSourceCard().gameObject;
      GameObject gameObject = chessAttackAnimation.m_targets[0].gameObject;
      bool useSpellFinishDelay = false;
      if (chessAttackAnimation.m_targets.Count == 1 && ((double) gameObject.transform.position.z < -7.0 || (double) gameObject.transform.position.z > -2.0) && ((double) source.transform.position.x + (double) gameObject.transform.position.x < -17.5 || (double) source.transform.position.x + (double) gameObject.transform.position.x > -12.5))
        useSpellFinishDelay = true;
      yield return (object) new WaitForSeconds(chessAttackAnimation.m_ImpactEffectDelay);
      chessAttackAnimation.StartCoroutine(chessAttackAnimation.PlayImpactEffects());
      if (useSpellFinishDelay)
        yield return (object) new WaitForSeconds(chessAttackAnimation.m_SpellFinishDelay);
      foreach (GameObject target in chessAttackAnimation.m_targets)
        GameUtils.DoDamageTasks(chessAttackAnimation.m_taskList, chessAttackAnimation.GetSourceCard(), target.GetComponentInChildren<Card>());
      foreach (GameObject target in chessAttackAnimation.m_targets)
      {
        while (iTween.HasTween(target))
          yield return (object) null;
      }
      while (iTween.HasTween(source))
        yield return (object) null;
      chessAttackAnimation.OnSpellFinished();
      yield return (object) new WaitForSeconds(1f);
      chessAttackAnimation.OnStateFinished();
    }
  }
}
