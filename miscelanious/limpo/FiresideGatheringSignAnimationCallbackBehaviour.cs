using UnityEngine;

public class FiresideGatheringSignAnimationCallbackBehaviour : MonoBehaviour
{
  public FiresideGatheringSign m_ParentSign;

  public void EnableShadowOnSign() => this.m_ParentSign.SetSignShadowEnabled(true);

  public void DisableShadowOnSign() => this.m_ParentSign.SetSignShadowEnabled(false);

  public void OnSignSocketAnimationComplete() => this.m_ParentSign.FireSignSocketAnimationCompleteListener();
}
