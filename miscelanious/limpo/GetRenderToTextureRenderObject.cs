using HutongGames.PlayMaker;
using UnityEngine;

[Tooltip("Get the object being rendered to from RenderToTexture")]
[ActionCategory("Pegasus")]
public class GetRenderToTextureRenderObject : FsmStateAction
{
  [RequiredField]
  [CheckForComponent(typeof (RenderToTexture))]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmGameObject renderObject;

  [Tooltip("Get the object being rendered to from RenderToTexture. This is used to get the procedurally generated render plane object.")]
  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.renderObject = (FsmGameObject) null;
  }

  public override void OnEnter()
  {
    this.DoGetObject();
    this.Finish();
  }

  private void DoGetObject()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    RenderToTexture component = ownerDefaultTarget.GetComponent<RenderToTexture>();
    if ((Object) component == (Object) null)
      this.LogError("Missing RenderToTexture component!");
    else
      this.renderObject.Value = component.GetRenderToObject();
  }
}
