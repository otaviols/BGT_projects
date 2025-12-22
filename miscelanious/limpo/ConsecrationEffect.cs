using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsecrationEffect : MonoBehaviour
{
  public float m_StartDelayMin = 2f;
  public float m_StartDelayMax = 3f;
  public float m_LiftTime = 1f;
  public float m_LiftHeightMin = 2f;
  public float m_LiftHeightMax = 3f;
  public float m_LiftRotMin = -15f;
  public float m_LiftRotMax = 15f;
  public float m_HoverTime = 0.8f;
  public float m_SlamTime = 0.2f;
  public float m_Bounceness = 0.2f;
  public GameObject m_StartImpact;
  public GameObject m_EndImpact;
  public float m_TotalTime;
  private SuperSpell m_SuperSpell;
  private List<GameObject> m_ImpactObjects;
  private AudioSource m_ImpactSound;

  private void Awake()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_LiftHeightMin = 3f;
    this.m_LiftHeightMax = 5f;
  }

  private void Start()
  {
    Spell component = this.GetComponent<Spell>();
    if ((Object) component == (Object) null)
      this.enabled = false;
    this.m_SuperSpell = component.GetSuperSpellParent();
    this.m_ImpactSound = this.GetComponent<AudioSource>();
    this.m_ImpactObjects = new List<GameObject>();
  }

  private void OnDestroy()
  {
    if (this.m_ImpactObjects.Count <= 0)
      return;
    foreach (Object impactObject in this.m_ImpactObjects)
      Object.Destroy(impactObject);
  }

  private void StartAnimation()
  {
    if ((Object) this.m_SuperSpell == (Object) null)
      return;
    int num1 = 0;
    foreach (GameObject target in this.m_SuperSpell.GetTargets())
    {
      Vector3 position = target.transform.position;
      Quaternion rotation = target.transform.rotation;
      ++num1;
      float num2 = Random.Range(this.m_StartDelayMin, this.m_StartDelayMax);
      GameObject gameObject1 = Object.Instantiate<GameObject>(this.m_StartImpact, position, rotation);
      this.m_ImpactObjects.Add(gameObject1);
      foreach (ParticleSystem componentsInChild in gameObject1.GetComponentsInChildren<ParticleSystem>())
      {
        componentsInChild.main.startDelay = (ParticleSystem.MinMaxCurve) num2;
        componentsInChild.Play();
      }
      float num3 = num2 + 0.2f;
      float num4 = Random.Range(this.m_LiftHeightMin, this.m_LiftHeightMax);
      Hashtable args1 = iTween.Hash((object) "time", (object) this.m_LiftTime, (object) "delay", (object) num3, (object) "position", (object) new Vector3(position.x, position.y + num4, position.z), (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "name", (object) string.Format("Lift_{0}_{1}", (object) target.name, (object) num1));
      iTween.MoveTo(target, args1);
      Vector3 eulerAngles = rotation.eulerAngles;
      eulerAngles.x += Random.Range(this.m_LiftRotMin, this.m_LiftRotMax);
      eulerAngles.z += Random.Range(this.m_LiftRotMin, this.m_LiftRotMax);
      Hashtable args2 = iTween.Hash((object) "time", (object) (float) ((double) this.m_LiftTime + (double) this.m_HoverTime + (double) this.m_SlamTime * 0.800000011920929), (object) "delay", (object) num3, (object) "rotation", (object) eulerAngles, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "name", (object) string.Format("LiftRot_{0}_{1}", (object) target.name, (object) num1));
      iTween.RotateTo(target, args2);
      float num5 = this.m_StartDelayMax + this.m_LiftTime + this.m_HoverTime;
      Hashtable args3 = iTween.Hash((object) "time", (object) this.m_SlamTime, (object) "delay", (object) num5, (object) "position", (object) position, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "name", (object) string.Format("SlamPos_{0}_{1}", (object) target.name, (object) num1));
      iTween.MoveTo(target, args3);
      Hashtable args4 = iTween.Hash((object) "time", (object) (float) ((double) this.m_SlamTime * 0.800000011920929), (object) "delay", (object) (float) ((double) num5 + (double) this.m_SlamTime * 0.200000002980232), (object) "rotation", (object) Vector3.zero, (object) "easetype", (object) iTween.EaseType.easeInQuad, (object) "oncomplete", (object) "Finished", (object) "oncompletetarget", (object) this.gameObject, (object) "name", (object) string.Format("SlamRot_{0}_{1}", (object) target.name, (object) num1));
      iTween.RotateTo(target, args4);
      this.m_TotalTime = num5 + this.m_SlamTime;
      if ((bool) (Object) target.GetComponentInChildren<MinionShake>())
      {
        MinionShake.ShakeObject(target, ShakeMinionType.RandomDirection, target.transform.position, ShakeMinionIntensity.LargeShake, 1f, 0.1f, num5 + this.m_SlamTime, true, true);
      }
      else
      {
        Bounce bounce = target.GetComponent<Bounce>();
        if ((Object) bounce == (Object) null)
          bounce = target.AddComponent<Bounce>();
        bounce.m_BounceAmount = num4 * this.m_Bounceness;
        bounce.m_BounceSpeed = 3.5f * Random.Range(0.8f, 1.3f);
        bounce.m_BounceCount = 3;
        bounce.m_Bounceness = this.m_Bounceness;
        bounce.m_Delay = num5 + this.m_SlamTime;
        bounce.StartAnimation();
      }
      GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_EndImpact, position, rotation);
      this.m_ImpactObjects.Add(gameObject2);
      foreach (ParticleSystem componentsInChild in gameObject2.GetComponentsInChildren<ParticleSystem>())
      {
        componentsInChild.main.startDelay = (ParticleSystem.MinMaxCurve) (num5 + this.m_SlamTime);
        componentsInChild.Play();
      }
    }
  }

  private void Finished()
  {
    SoundManager.Get().Play(this.m_ImpactSound);
    CameraShakeMgr.Shake(Camera.main, new Vector3(0.15f, 0.15f, 0.15f), 0.9f);
  }
}
