using UnityEngine;

public class BigCardTooltipDisplayBones : MonoBehaviour
{
  public GameObject_MobileOverride m_BoneRigs;

  public TooltipBoneLayout GetRigForCurrentPlatform()
  {
    if (this.m_BoneRigs == null)
      return (TooltipBoneLayout) null;
    GameObject valueForScreen = this.m_BoneRigs.GetValueForScreen(PlatformSettings.Screen, (object) null);
    if ((Object) valueForScreen == (Object) null)
      return (TooltipBoneLayout) null;
    TooltipBoneLayout component = valueForScreen.GetComponent<TooltipBoneLayout>();
    return (Object) component == (Object) null ? (TooltipBoneLayout) null : component;
  }

  public bool HasBonesForCurrentPlatform(
    BigCardTooltipDisplayBones.BoneVerification bonesToCheck)
  {
    TooltipBoneLayout forCurrentPlatform = this.GetRigForCurrentPlatform();
    if ((Object) forCurrentPlatform == (Object) null)
      return false;
    return bonesToCheck == BigCardTooltipDisplayBones.BoneVerification.PRIMARY_ONLY ? forCurrentPlatform.HasPrimaryBones() : forCurrentPlatform.HasAllBones();
  }

  public enum BoneVerification
  {
    PRIMARY_ONLY,
    ALL_BONES,
  }
}
