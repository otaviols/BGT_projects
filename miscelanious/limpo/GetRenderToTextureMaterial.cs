using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Pegasus")]
[Tooltip("Get the material instance from an object with RenderToTexture")]
public class GetRenderToTextureMaterial : FsmStateAction
{
  [RequiredField]
  [CheckForComponent(typeof (RenderToTexture))]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmMaterial material;

  [Tooltip("Get the material instance from an object with RenderToTexture. This is used to get the material of the procedurally generated render plane.")]
  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.material = (FsmMaterial) null;
  }

  public override void OnEnter()
  {
    this.DoGetMaterial();
    this.Finish();
  }

  private void DoGetMaterial()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    RenderToTexture component = ownerDefaultTarget.GetComponent<RenderToTexture>();
    if ((Object) component == (Object) null)
      this.LogError("Missing RenderToTexture component!");
    else
      this.material.Value = component.GetRenderMaterial();
  }
}
