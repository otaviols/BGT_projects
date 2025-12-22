using Blizzard.T5.Core.Utils;
using Blizzard.T5.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShake : MonoBehaviour
{
  private readonly Vector3 OFFSCREEN_POSITION = new Vector3(-400f, -400f, -400f);
  private const float INTENSITY_SMALL = 0.1f;
  private const float INTENSITY_MEDIUM = 0.5f;
  private const float INTENSITY_LARGE = 1f;
  private const float DISABLE_HEIGHT = 0.1f;
  public GameObject m_MinionShakeAnimator;
  private bool m_Animating;
  private Animator m_Animator;
  private Vector2 m_ImpactDirection;
  private Vector3 m_ImpactPosition;
  private float m_Angle;
  private ShakeMinionIntensity m_ShakeIntensityType = ShakeMinionIntensity.MediumShake;
  private float m_IntensityValue = 0.5f;
  private ShakeMinionType m_ShakeType = ShakeMinionType.RandomDirection;
  private GameObject m_MinionShakeInstance;
  private Transform m_CardPlayAllyTransform;
  private Vector3 m_MinionOrgPos;
  private Quaternion m_MinionOrgRot;
  private float m_StartDelay;
  private float m_Radius;
  private bool m_IgnoreAnimationPlaying;
  private bool m_IgnoreHeight;
  private IGraphicsManager m_graphicsManager;
  private static int s_IdleState = Animator.StringToHash("Base.Idle");

  private void Awake() => this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();

  private void LateUpdate()
  {
    if (this.m_graphicsManager != null && this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.Low || !this.m_Animating || (Object) this.m_Animator == (Object) null || (Object) this.m_MinionShakeInstance == (Object) null)
      return;
    if (this.m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == MinionShake.s_IdleState && !this.m_Animator.GetBool("shake"))
    {
      this.transform.localPosition = this.m_MinionOrgPos;
      this.transform.localRotation = this.m_MinionOrgRot;
      this.m_Animating = false;
    }
    else
    {
      this.transform.localPosition = this.m_CardPlayAllyTransform.localPosition + this.m_MinionOrgPos;
      this.transform.localRotation = this.m_MinionOrgRot;
      this.transform.Rotate(this.m_CardPlayAllyTransform.localRotation.eulerAngles);
    }
  }

  private void OnDestroy()
  {
    if (!(bool) (Object) this.m_MinionShakeInstance)
      return;
    Object.Destroy((Object) this.m_MinionShakeInstance);
  }

  public bool isShaking() => this.m_Animating;

  public static void ShakeAllMinions(
    GameObject shakeTrigger,
    ShakeMinionType shakeType,
    Vector3 impactPoint,
    ShakeMinionIntensity intensityType,
    float intensityValue,
    float radius,
    float startDelay)
  {
    foreach (MinionShake allMinionShaker in MinionShake.FindAllMinionShakers(shakeTrigger))
    {
      allMinionShaker.m_StartDelay = startDelay;
      allMinionShaker.m_ShakeType = shakeType;
      allMinionShaker.m_ImpactPosition = impactPoint;
      allMinionShaker.m_ShakeIntensityType = intensityType;
      allMinionShaker.m_IntensityValue = intensityValue;
      allMinionShaker.m_Radius = radius;
      allMinionShaker.ShakeMinion();
      BoardEvents boardEvents = BoardEvents.Get();
      if ((Object) boardEvents != (Object) null)
        boardEvents.MinionShakeEvent(intensityType, intensityValue);
    }
  }

  public static void ShakeTargetMinion(
    GameObject shakeTarget,
    ShakeMinionType shakeType,
    Vector3 impactPoint,
    ShakeMinionIntensity intensityType,
    float intensityValue,
    float radius,
    float startDelay)
  {
    Spell componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<Spell>(shakeTarget);
    if ((Object) componentInThisOrParents == (Object) null)
    {
      Debug.LogWarning((object) "MinionShake: failed to locate Spell component");
    }
    else
    {
      GameObject visualTarget = componentInThisOrParents.GetVisualTarget();
      if ((Object) visualTarget == (Object) null)
      {
        Debug.LogWarning((object) "MinionShake: failed to Spell GetVisualTarget");
      }
      else
      {
        MinionShake componentInChildren = visualTarget.GetComponentInChildren<MinionShake>();
        if ((Object) componentInChildren == (Object) null)
        {
          Debug.LogWarning((object) "MinionShake: failed to locate MinionShake component");
        }
        else
        {
          componentInChildren.m_StartDelay = startDelay;
          componentInChildren.m_ShakeType = shakeType;
          componentInChildren.m_ImpactPosition = impactPoint;
          componentInChildren.m_ShakeIntensityType = intensityType;
          componentInChildren.m_IntensityValue = intensityValue;
          componentInChildren.m_Radius = radius;
          componentInChildren.ShakeMinion();
        }
      }
    }
  }

  public static void ShakeObject(
    GameObject shakeObject,
    ShakeMinionType shakeType,
    Vector3 impactPoint,
    ShakeMinionIntensity intensityType,
    float intensityValue,
    float radius,
    float startDelay)
  {
    MinionShake.ShakeObject(shakeObject, shakeType, impactPoint, intensityType, intensityValue, radius, startDelay, false);
  }

  public static void ShakeObject(
    GameObject shakeObject,
    ShakeMinionType shakeType,
    Vector3 impactPoint,
    ShakeMinionIntensity intensityType,
    float intensityValue,
    float radius,
    float startDelay,
    bool ignoreAnimationPlaying)
  {
    MinionShake.ShakeObject(shakeObject, shakeType, impactPoint, intensityType, intensityValue, radius, startDelay, false, ignoreAnimationPlaying);
  }

  public static void ShakeObject(
    GameObject shakeObject,
    ShakeMinionType shakeType,
    Vector3 impactPoint,
    ShakeMinionIntensity intensityType,
    float intensityValue,
    float radius,
    float startDelay,
    bool ignoreAnimationPlaying,
    bool ignoreHeight)
  {
    MinionShake componentInChildren = shakeObject.GetComponentInChildren<MinionShake>();
    if ((Object) componentInChildren == (Object) null)
    {
      Actor componentInParents = GameObjectUtils.FindComponentInParents<Actor>(shakeObject);
      if ((Object) componentInParents == (Object) null)
        return;
      componentInChildren = componentInParents.gameObject.GetComponentInChildren<MinionShake>();
      if ((Object) componentInChildren == (Object) null)
        return;
    }
    componentInChildren.m_StartDelay = startDelay;
    componentInChildren.m_ShakeType = shakeType;
    componentInChildren.m_ImpactPosition = impactPoint;
    componentInChildren.m_ShakeIntensityType = intensityType;
    componentInChildren.m_IntensityValue = intensityValue;
    componentInChildren.m_Radius = radius;
    componentInChildren.m_IgnoreAnimationPlaying = ignoreAnimationPlaying;
    componentInChildren.ShakeMinion();
  }

  public void ShakeAllMinionsRandomMedium()
  {
    Vector3 impactPoint = Vector3.zero;
    Board board = Board.Get();
    if ((Object) board != (Object) null)
    {
      Transform bone = board.FindBone("CenterPointBone");
      if ((Object) bone != (Object) null)
        impactPoint = bone.position;
    }
    MinionShake.ShakeAllMinions(this.gameObject, ShakeMinionType.RandomDirection, impactPoint, ShakeMinionIntensity.MediumShake, 0.5f, 0.0f, 0.0f);
  }

  public void ShakeAllMinionsRandomLarge()
  {
    Vector3 impactPoint = Vector3.zero;
    Board board = Board.Get();
    if ((Object) board != (Object) null)
    {
      Transform bone = board.FindBone("CenterPointBone");
      if ((Object) bone != (Object) null)
        impactPoint = bone.position;
    }
    MinionShake.ShakeAllMinions(this.gameObject, ShakeMinionType.RandomDirection, impactPoint, ShakeMinionIntensity.LargeShake, 1f, 0.0f, 0.0f);
  }

  public void RandomShake(float impact)
  {
    this.m_ShakeIntensityType = ShakeMinionIntensity.Custom;
    this.m_IntensityValue = impact;
    this.m_ShakeType = ShakeMinionType.Angle;
    this.m_ShakeType = ShakeMinionType.RandomDirection;
    this.ShakeMinion();
  }

  private void ShakeMinion()
  {
    if (this.m_graphicsManager == null || this.m_graphicsManager.RenderQualityLevel == GraphicsQuality.Low)
      return;
    if ((Object) this.m_MinionShakeAnimator == (Object) null)
    {
      Debug.LogWarning((object) "MinionShake: failed to locate MinionShake Animator");
    }
    else
    {
      Animation component = this.GetComponent<Animation>();
      if ((Object) component != (Object) null && component.isPlaying && !this.m_IgnoreAnimationPlaying)
        return;
      Vector3 vector3 = Vector3.zero;
      Board board = Board.Get();
      if ((Object) board != (Object) null)
      {
        Transform bone = board.FindBone("CenterPointBone");
        if ((Object) bone != (Object) null)
          vector3 = bone.position;
      }
      if ((double) vector3.y - (double) this.transform.position.y > 0.100000001490116 && !this.m_IgnoreHeight)
        return;
      if ((Object) this.m_MinionShakeInstance == (Object) null)
      {
        this.m_MinionShakeInstance = Object.Instantiate<GameObject>(this.m_MinionShakeAnimator, this.OFFSCREEN_POSITION, this.transform.rotation);
        this.m_CardPlayAllyTransform = this.m_MinionShakeInstance.transform.Find("Card_Play_Ally").gameObject.transform;
      }
      if ((Object) this.m_Animator == (Object) null)
        this.m_Animator = this.m_MinionShakeInstance.GetComponent<Animator>();
      if (!this.m_Animating)
      {
        this.m_MinionOrgPos = this.transform.localPosition;
        this.m_MinionOrgRot = this.transform.localRotation;
      }
      if (this.m_ShakeType == ShakeMinionType.Angle)
        this.m_ImpactDirection = this.AngleToVector(this.m_Angle);
      else if (this.m_ShakeType == ShakeMinionType.ImpactDirection)
        this.m_ImpactDirection = (Vector2) Vector3.Normalize(this.transform.position - this.m_ImpactPosition);
      else if (this.m_ShakeType == ShakeMinionType.RandomDirection)
        this.m_ImpactDirection = this.AngleToVector(Random.Range(0.0f, 360f));
      float num = this.m_IntensityValue;
      if (this.m_ShakeIntensityType == ShakeMinionIntensity.SmallShake)
        num = 0.1f;
      else if (this.m_ShakeIntensityType == ShakeMinionIntensity.MediumShake)
        num = 0.5f;
      else if (this.m_ShakeIntensityType == ShakeMinionIntensity.LargeShake)
        num = 1f;
      this.m_ImpactDirection *= num;
      this.m_Animator.SetFloat("posx", this.m_ImpactDirection.x);
      this.m_Animator.SetFloat("posy", this.m_ImpactDirection.y);
      if ((double) this.m_Radius > 0.0 && (double) Vector3.Distance(this.transform.position, this.m_ImpactPosition) > (double) this.m_Radius)
        return;
      if ((double) this.m_StartDelay > 0.0)
      {
        this.StartCoroutine(this.StartShakeAnimation());
      }
      else
      {
        this.m_Animating = true;
        this.m_Animator.SetBool("shake", true);
      }
      this.StartCoroutine(this.ResetShakeAnimator());
    }
  }

  private Vector2 AngleToVector(float angle)
  {
    Vector3 vector3 = Quaternion.Euler(0.0f, angle, 0.0f) * new Vector3(0.0f, 0.0f, -1f);
    return new Vector2(vector3.x, vector3.z);
  }

  private IEnumerator StartShakeAnimation()
  {
    yield return (object) new WaitForSeconds(this.m_StartDelay);
    this.m_Animating = true;
    this.m_Animator.SetBool("shake", true);
  }

  private IEnumerator ResetShakeAnimator()
  {
    yield return (object) new WaitForSeconds(this.m_StartDelay);
    yield return (object) new WaitForEndOfFrame();
    yield return (object) new WaitForEndOfFrame();
    this.m_Animator.SetBool("shake", false);
  }

  private static MinionShake[] FindAllMinionShakers(GameObject shakeTrigger)
  {
    Card card1 = (Card) null;
    Spell componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<Spell>(shakeTrigger);
    if ((Object) componentInThisOrParents != (Object) null)
      card1 = componentInThisOrParents.GetSourceCard();
    List<MinionShake> minionShakeList = new List<MinionShake>();
    ZoneMgr zoneMgr = ZoneMgr.Get();
    if ((Object) zoneMgr == (Object) null)
      return minionShakeList.ToArray();
    foreach (Zone zone in zoneMgr.FindZonesForTag(TAG_ZONE.PLAY))
    {
      if (!(((object) zone).GetType() == typeof (ZoneHero)))
      {
        foreach (Card card2 in zone.GetCards())
        {
          if (!((Object) card2 == (Object) card1) && card2.IsActorReady())
          {
            MinionShake componentInChildren = card2.GetComponentInChildren<MinionShake>();
            if (!((Object) componentInChildren == (Object) null))
              minionShakeList.Add(componentInChildren);
          }
        }
      }
    }
    return minionShakeList.ToArray();
  }
}
