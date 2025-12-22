using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class UIBObjectSpacing : MonoBehaviour
{
  public List<UIBObjectSpacing.SpacedObject> m_Objects = new List<UIBObjectSpacing.SpacedObject>();
  [SerializeField]
  private Vector3 m_LocalOffset;
  [SerializeField]
  private Vector3 m_LocalSpacing;
  [SerializeField]
  private Vector3 m_Alignment = new Vector3(0.5f, 0.5f, 0.5f);
  public bool m_reverse;

  public Vector3 LocalOffset
  {
    get => this.m_LocalOffset;
    set
    {
      this.m_LocalOffset = value;
      this.UpdatePositions();
    }
  }

  public Vector3 LocalSpacing
  {
    get => this.m_LocalSpacing;
    set
    {
      this.m_LocalSpacing = value;
      this.UpdatePositions();
    }
  }

  [CustomEditField(Range = "0 - 1")]
  public Vector3 Alignment
  {
    get => this.m_Alignment;
    set
    {
      this.m_Alignment = value;
      this.m_Alignment.x = Mathf.Clamp01(this.m_Alignment.x);
      this.m_Alignment.y = Mathf.Clamp01(this.m_Alignment.y);
      this.m_Alignment.z = Mathf.Clamp01(this.m_Alignment.z);
      this.UpdatePositions();
    }
  }

  public void AddSpace(int index) => this.m_Objects.Insert(index, new UIBObjectSpacing.SpacedObject()
  {
    m_CountIfNull = true
  });

  public void AddSpace(int index, Vector3 offset) => this.m_Objects.Insert(index, new UIBObjectSpacing.SpacedObject()
  {
    m_Offset = offset,
    m_CountIfNull = true
  });

  public void AddObject(GameObject obj, bool countIfNull = true) => this.AddObject(obj, Vector3.zero, countIfNull);

  public void AddObject(Component comp, bool countIfNull = true) => this.AddObject(comp, Vector3.zero, countIfNull);

  public void AddObject(Component comp, Vector3 offset, bool countIfNull = true) => this.AddObject(comp.gameObject, offset, countIfNull);

  public void AddObject(GameObject obj, Vector3 offset, bool countIfNull = true) => this.m_Objects.Add(new UIBObjectSpacing.SpacedObject()
  {
    m_Object = obj,
    m_Offset = offset,
    m_CountIfNull = countIfNull
  });

  public void ClearObjects() => this.m_Objects.Clear();

  public void AnimateUpdatePositions(float animTime, iTween.EaseType tweenType = iTween.EaseType.easeInOutQuad)
  {
    List<UIBObjectSpacing.AnimationPosition> animationPositionList = new List<UIBObjectSpacing.AnimationPosition>();
    List<UIBObjectSpacing.SpacedObject> all = this.m_Objects.FindAll((Predicate<UIBObjectSpacing.SpacedObject>) (o =>
    {
      if (o.m_CountIfNull)
        return true;
      return (UnityEngine.Object) o.m_Object != (UnityEngine.Object) null && o.m_Object.activeInHierarchy;
    }));
    if (this.m_reverse)
      all.Reverse();
    Vector3 localOffset = this.m_LocalOffset;
    Vector3 localSpacing = this.m_LocalSpacing;
    Vector3 zero = Vector3.zero;
    for (int index = 0; index < all.Count; ++index)
    {
      UIBObjectSpacing.SpacedObject spacedObject = all[index];
      GameObject gameObject = spacedObject.m_Object;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        animationPositionList.Add(new UIBObjectSpacing.AnimationPosition()
        {
          m_targetPos = localOffset + spacedObject.m_Offset,
          m_object = gameObject
        });
      Vector3 offset = spacedObject.m_Offset;
      if (index < all.Count - 1)
        offset += localSpacing;
      localOffset += offset;
      zero += offset;
    }
    zero.x *= this.m_Alignment.x;
    zero.y *= this.m_Alignment.y;
    zero.z *= this.m_Alignment.z;
    for (int index = 0; index < animationPositionList.Count; ++index)
    {
      UIBObjectSpacing.AnimationPosition animationPosition = animationPositionList[index];
      iTween.MoveTo(animationPosition.m_object, iTween.Hash((object) "position", (object) (animationPosition.m_targetPos - zero), (object) "islocal", (object) true, (object) "easetype", (object) tweenType, (object) "time", (object) animTime));
    }
  }

  public void UpdatePositions()
  {
    List<UIBObjectSpacing.SpacedObject> all = this.m_Objects.FindAll((Predicate<UIBObjectSpacing.SpacedObject>) (o =>
    {
      if (o.m_CountIfNull)
        return true;
      return (UnityEngine.Object) o.m_Object != (UnityEngine.Object) null && o.m_Object.activeInHierarchy;
    }));
    if (this.m_reverse)
      all.Reverse();
    Vector3 localOffset = this.m_LocalOffset;
    Vector3 localSpacing = this.m_LocalSpacing;
    Vector3 zero = Vector3.zero;
    for (int index = 0; index < all.Count; ++index)
    {
      UIBObjectSpacing.SpacedObject spacedObject = all[index];
      GameObject gameObject = spacedObject.m_Object;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        gameObject.transform.localPosition = localOffset + spacedObject.m_Offset;
      Vector3 offset = spacedObject.m_Offset;
      if (index < all.Count - 1)
        offset += localSpacing;
      localOffset += offset;
      zero += offset;
    }
    zero.x *= this.m_Alignment.x;
    zero.y *= this.m_Alignment.y;
    zero.z *= this.m_Alignment.z;
    for (int index = 0; index < all.Count; ++index)
    {
      GameObject gameObject = all[index].m_Object;
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        gameObject.transform.localPosition -= zero;
    }
  }

  [Serializable]
  public class SpacedObject
  {
    public GameObject m_Object;
    public Vector3 m_Offset;
    public bool m_CountIfNull;
  }

  private class AnimationPosition
  {
    public Vector3 m_targetPos;
    public GameObject m_object;
  }
}
