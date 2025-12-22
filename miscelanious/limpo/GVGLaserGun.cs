using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GVGLaserGun : MonoBehaviour
{
  [CustomEditField(Sections = "Lever")]
  public GameObject m_GunLever;
  [CustomEditField(Sections = "Lever")]
  public Spell m_PullLeverSpell;
  [CustomEditField(Sections = "Rotation")]
  [Tooltip("The thing that will be rotated.")]
  public GameObject m_GunRotator;
  [CustomEditField(Sections = "Rotation")]
  public GameObject m_RotateLeftButton;
  [CustomEditField(Sections = "Rotation")]
  public GameObject m_RotateRightButton;
  [CustomEditField(Sections = "Rotation")]
  public Spell m_StartRotationSpell;
  [CustomEditField(Sections = "Rotation")]
  public Spell m_StopRotationSpell;
  [CustomEditField(Sections = "Rotation")]
  [Tooltip("How fast the gun rotates in degrees per second.")]
  public float m_RotationSpeed;
  [CustomEditField(ListTable = true, Sections = "Rotation")]
  public List<GVGLaserGun.AngleDef> m_AngleDefs = new List<GVGLaserGun.AngleDef>();
  [CustomEditField(Sections = "Debug")]
  public bool m_DebugShowGunAngle;
  private List<int> m_sortedAngleDefIndexes = new List<int>();
  private int m_rotationDir;
  private int m_requestedRotationDir;
  private float m_angle;
  private int m_angleIndex = -1;
  private int m_minAngleIndex = -1;
  private int m_maxAngleIndex = -1;
  private bool m_leverEffectsActive;

  private void Awake()
  {
    if (this.m_AngleDefs.Count == 0)
      return;
    for (int index = 0; index < this.m_AngleDefs.Count; ++index)
      this.m_sortedAngleDefIndexes.Add(index);
    this.m_sortedAngleDefIndexes.Sort((Comparison<int>) ((index1, index2) => GVGLaserGun.AngleDefSortComparison(this.m_AngleDefs[index1], this.m_AngleDefs[index2])));
    this.m_angleIndex = 0;
    this.m_minAngleIndex = 0;
    this.m_maxAngleIndex = 0;
    float angle = this.m_AngleDefs[0].m_Angle;
    float num = angle;
    for (int index = 0; index < this.m_sortedAngleDefIndexes.Count; ++index)
    {
      GVGLaserGun.AngleDef angleDef = this.m_AngleDefs[this.m_sortedAngleDefIndexes[index]];
      if ((double) angleDef.m_Angle < (double) angle)
      {
        angle = angleDef.m_Angle;
        this.m_minAngleIndex = index;
      }
      if ((double) angleDef.m_Angle > (double) num)
      {
        num = angleDef.m_Angle;
        this.m_maxAngleIndex = index;
      }
      if (angleDef.m_Default)
      {
        this.m_angleIndex = index;
        this.SetAngle(angleDef.m_Angle);
      }
    }
  }

  private void Update()
  {
    this.HandleRotation();
    this.HandleLever();
  }

  private GVGLaserGun.AngleDef GetAngleDef()
  {
    if (this.m_angleIndex < 0)
      return (GVGLaserGun.AngleDef) null;
    return this.m_angleIndex >= this.m_sortedAngleDefIndexes.Count ? (GVGLaserGun.AngleDef) null : this.m_AngleDefs[this.m_sortedAngleDefIndexes[this.m_angleIndex]];
  }

  private static int AngleDefSortComparison(GVGLaserGun.AngleDef def1, GVGLaserGun.AngleDef def2)
  {
    if ((double) def1.m_Angle < (double) def2.m_Angle)
      return -1;
    return (double) def1.m_Angle > (double) def2.m_Angle ? 1 : 0;
  }

  private void HandleRotation()
  {
    if (this.m_leverEffectsActive)
      return;
    this.m_requestedRotationDir = 0;
    if (InputCollection.GetMouseButton(0))
    {
      if (this.IsOver(this.m_RotateLeftButton))
        this.m_requestedRotationDir = -1;
      else if (this.IsOver(this.m_RotateRightButton))
        this.m_requestedRotationDir = 1;
    }
    if (this.ShouldStartRotating())
      this.StartRotating(this.m_requestedRotationDir);
    if (this.m_rotationDir < 0)
    {
      this.RotateLeft();
    }
    else
    {
      if (this.m_rotationDir <= 0)
        return;
      this.RotateRight();
    }
  }

  private bool ShouldStartRotating() => this.m_requestedRotationDir != 0 && this.m_requestedRotationDir != this.m_rotationDir && (this.m_requestedRotationDir >= 0 || this.m_angleIndex != this.m_minAngleIndex) && (this.m_requestedRotationDir <= 0 || this.m_angleIndex != this.m_maxAngleIndex);

  private void RotateLeft()
  {
    GVGLaserGun.AngleDef angleDef = this.GetAngleDef();
    float angle = Mathf.MoveTowards(this.m_angle, angleDef.m_Angle, this.m_RotationSpeed * Time.deltaTime);
    if ((double) angle <= (double) angleDef.m_Angle)
    {
      if (this.m_requestedRotationDir == 0 || this.m_angleIndex == this.m_minAngleIndex)
      {
        this.SetAngle(angle);
        this.StopRotating();
      }
      else
        --this.m_angleIndex;
    }
    else
      this.SetAngle(angle);
  }

  private void RotateRight()
  {
    GVGLaserGun.AngleDef angleDef = this.GetAngleDef();
    float angle = Mathf.MoveTowards(this.m_angle, angleDef.m_Angle, this.m_RotationSpeed * Time.deltaTime);
    if ((double) angle >= (double) angleDef.m_Angle)
    {
      if (this.m_requestedRotationDir == 0 || this.m_angleIndex == this.m_maxAngleIndex)
      {
        this.SetAngle(angle);
        this.StopRotating();
      }
      else
        ++this.m_angleIndex;
    }
    else
      this.SetAngle(angle);
  }

  private void StartRotating(int dir)
  {
    this.m_rotationDir = dir;
    if (dir < 0)
      --this.m_angleIndex;
    else
      ++this.m_angleIndex;
    if (!(bool) (UnityEngine.Object) this.m_StartRotationSpell)
      return;
    this.m_StartRotationSpell.Activate();
  }

  private void StopRotating()
  {
    this.m_rotationDir = 0;
    if (!(bool) (UnityEngine.Object) this.m_StopRotationSpell)
      return;
    this.m_StopRotationSpell.Activate();
  }

  private void SetAngle(float angle)
  {
    this.m_angle = angle;
    TransformUtil.SetLocalEulerAngleY(this.m_GunRotator, this.m_angle);
  }

  private void HandleLever()
  {
    if (this.m_rotationDir != 0 || this.m_leverEffectsActive || !InputCollection.GetMouseButtonUp(0) || !this.IsOver(this.m_GunLever))
      return;
    this.PullLever();
  }

  private void PullLever()
  {
    if (!(bool) (UnityEngine.Object) this.m_PullLeverSpell)
      return;
    this.m_leverEffectsActive = true;
    this.m_PullLeverSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnPullLeverSpellFinished));
    this.m_PullLeverSpell.Activate();
  }

  private void OnPullLeverSpellFinished(Spell spell, object userData)
  {
    Spell impactSpell = this.GetAngleDef().m_ImpactSpell;
    if (!(bool) (UnityEngine.Object) impactSpell)
    {
      this.m_leverEffectsActive = false;
    }
    else
    {
      impactSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnImpactSpellFinished));
      impactSpell.Activate();
    }
  }

  private void OnImpactSpellFinished(Spell spell, object userData) => this.m_leverEffectsActive = false;

  private bool IsOver(GameObject go) => (bool) (UnityEngine.Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);

  [Serializable]
  public class AngleDef
  {
    public bool m_Default;
    [CustomEditField(ListSortable = true)]
    public float m_Angle;
    public Spell m_ImpactSpell;

    public int CustomBehaviorListCompare(GVGLaserGun.AngleDef def1, GVGLaserGun.AngleDef def2) => GVGLaserGun.AngleDefSortComparison(def1, def2);
  }
}
