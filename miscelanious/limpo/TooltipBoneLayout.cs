using UnityEngine;

public class TooltipBoneLayout : MonoBehaviour
{
  public GameObject m_topLeftTooltipBone;
  public GameObject m_bottomLeftTooltipBone;
  public GameObject m_topRightTooltipBone;
  public GameObject m_bottomRightTooltipBone;
  [Range(-1f, 1f)]
  public float m_manualHorizontalAdjustment;
  [Range(-1f, 1f)]
  public float m_manualVerticalAdjustment;

  public bool HasAllBones() => (Object) this.m_topLeftTooltipBone != (Object) null && (Object) this.m_bottomLeftTooltipBone != (Object) null && (Object) this.m_topRightTooltipBone != (Object) null && (Object) this.m_bottomRightTooltipBone != (Object) null;

  public bool HasPrimaryBones() => (Object) this.m_topLeftTooltipBone != (Object) null && (Object) this.m_topRightTooltipBone != (Object) null;
}
