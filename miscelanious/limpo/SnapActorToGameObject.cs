using Blizzard.T5.Core.Utils;
using UnityEngine;

public class SnapActorToGameObject : MonoBehaviour
{
  public bool m_SnapPostion = true;
  public bool m_SnapRotation = true;
  public bool m_SnapScale = true;
  public bool m_ResetTransformOnDisable;
  private Transform m_actorTransform;
  private Vector3 m_OrgPosition;
  private Quaternion m_OrgRotation;
  private Vector3 m_OrgScale;

  private void Start()
  {
    Actor actor = GameObjectUtils.FindComponentInThisOrParents<Actor>(this.gameObject);
    if ((Object) actor == (Object) null)
    {
      Spell componentInParents = GameObjectUtils.FindComponentInParents<Spell>(this.gameObject);
      if ((Object) componentInParents != (Object) null)
        actor = componentInParents.GetSourceCard().GetActor();
    }
    if ((Object) actor == (Object) null)
    {
      Debug.LogError((object) string.Format("SnapActorToGameObject on {0} failed to find Actor object!", (object) this.gameObject.name));
      this.enabled = false;
    }
    else
      this.m_actorTransform = actor.transform;
  }

  private void OnEnable()
  {
    if ((Object) this.m_actorTransform == (Object) null)
      return;
    this.m_OrgPosition = this.m_actorTransform.localPosition;
    this.m_OrgRotation = this.m_actorTransform.localRotation;
    this.m_OrgScale = this.m_actorTransform.localScale;
  }

  private void OnDisable()
  {
    if ((Object) this.m_actorTransform == (Object) null || !this.m_ResetTransformOnDisable)
      return;
    this.m_actorTransform.localPosition = this.m_OrgPosition;
    this.m_actorTransform.localRotation = this.m_OrgRotation;
    this.m_actorTransform.localScale = this.m_OrgScale;
  }

  private void LateUpdate()
  {
    if ((Object) this.m_actorTransform == (Object) null)
      return;
    if (this.m_SnapPostion)
      this.m_actorTransform.position = this.transform.position;
    if (this.m_SnapRotation)
      this.m_actorTransform.rotation = this.transform.rotation;
    if (!this.m_SnapScale)
      return;
    TransformUtil.SetWorldScale((Component) this.m_actorTransform, this.transform.lossyScale);
  }
}
