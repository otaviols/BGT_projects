using UnityEngine;

public class BigCardDisplayBones : MonoBehaviour
{
  public GameObject_MobileOverride m_BoneRigs;

  public void GetRigForCurrentPlatform(
    out GameObject rig,
    out BigCardBoneLayout.ScaleSettings scale)
  {
    if (this.m_BoneRigs == null)
    {
      rig = (GameObject) null;
      scale = (BigCardBoneLayout.ScaleSettings) null;
    }
    else
    {
      GameObject valueForScreen = this.m_BoneRigs.GetValueForScreen(PlatformSettings.Screen, (object) null);
      if ((Object) valueForScreen == (Object) null)
      {
        rig = (GameObject) null;
        scale = (BigCardBoneLayout.ScaleSettings) null;
      }
      else
      {
        BigCardBoneLayout component = valueForScreen.GetComponent<BigCardBoneLayout>();
        if ((Object) component == (Object) null || !component.HasAllBones())
        {
          rig = (GameObject) null;
          scale = (BigCardBoneLayout.ScaleSettings) null;
        }
        else
        {
          rig = valueForScreen;
          scale = component.m_scaleSettings;
        }
      }
    }
  }

  public bool HasBonesForCurrentPlatform()
  {
    GameObject rig;
    this.GetRigForCurrentPlatform(out rig, out BigCardBoneLayout.ScaleSettings _);
    if ((Object) rig == (Object) null)
      return false;
    BigCardBoneLayout component = rig.GetComponent<BigCardBoneLayout>();
    return !((Object) component == (Object) null) && component.HasAllBones();
  }
}
