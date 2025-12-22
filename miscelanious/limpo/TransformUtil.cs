using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtil
{
  public static Vector3 GetUnitAnchor(Anchor anchor)
  {
    Vector3 unitAnchor = new Vector3();
    switch (anchor)
    {
      case Anchor.TOP_LEFT:
        unitAnchor.x = 0.0f;
        unitAnchor.y = 1f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.TOP:
        unitAnchor.x = 0.5f;
        unitAnchor.y = 1f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.TOP_RIGHT:
        unitAnchor.x = 1f;
        unitAnchor.y = 1f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.LEFT:
        unitAnchor.x = 0.0f;
        unitAnchor.y = 0.5f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.CENTER:
        unitAnchor.x = 0.5f;
        unitAnchor.y = 0.5f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.RIGHT:
        unitAnchor.x = 1f;
        unitAnchor.y = 0.5f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.BOTTOM_LEFT:
        unitAnchor.x = 0.0f;
        unitAnchor.y = 0.0f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.BOTTOM:
        unitAnchor.x = 0.5f;
        unitAnchor.y = 0.0f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.BOTTOM_RIGHT:
        unitAnchor.x = 1f;
        unitAnchor.y = 0.0f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.FRONT:
        unitAnchor.x = 0.5f;
        unitAnchor.y = 0.0f;
        unitAnchor.z = 1f;
        break;
      case Anchor.BACK:
        unitAnchor.x = 0.5f;
        unitAnchor.y = 0.0f;
        unitAnchor.z = 0.0f;
        break;
      case Anchor.TOP_LEFT_XZ:
        unitAnchor.x = 0.0f;
        unitAnchor.z = 1f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.TOP_XZ:
        unitAnchor.x = 0.5f;
        unitAnchor.z = 1f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.TOP_RIGHT_XZ:
        unitAnchor.x = 1f;
        unitAnchor.z = 1f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.LEFT_XZ:
        unitAnchor.x = 0.0f;
        unitAnchor.z = 0.5f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.CENTER_XZ:
        unitAnchor.x = 0.5f;
        unitAnchor.z = 0.5f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.RIGHT_XZ:
        unitAnchor.x = 1f;
        unitAnchor.z = 0.5f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.BOTTOM_LEFT_XZ:
        unitAnchor.x = 0.0f;
        unitAnchor.z = 0.0f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.BOTTOM_XZ:
        unitAnchor.x = 0.5f;
        unitAnchor.z = 0.0f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.BOTTOM_RIGHT_XZ:
        unitAnchor.x = 1f;
        unitAnchor.z = 0.0f;
        unitAnchor.y = 0.0f;
        break;
      case Anchor.FRONT_XZ:
        unitAnchor.x = 0.5f;
        unitAnchor.z = 0.0f;
        unitAnchor.y = 1f;
        break;
      case Anchor.BACK_XZ:
        unitAnchor.x = 0.5f;
        unitAnchor.z = 0.0f;
        unitAnchor.y = 0.0f;
        break;
    }
    return unitAnchor;
  }

  public static Vector3 ComputeWorldPoint(Bounds bounds, Vector3 selfUnitAnchor) => new Vector3()
  {
    x = Mathf.Lerp(bounds.min.x, bounds.max.x, selfUnitAnchor.x),
    y = Mathf.Lerp(bounds.min.y, bounds.max.y, selfUnitAnchor.y),
    z = Mathf.Lerp(bounds.min.z, bounds.max.z, selfUnitAnchor.z)
  };

  public static Bounds ComputeSetPointBounds(Component c) => TransformUtil.ComputeSetPointBounds(c.gameObject, false);

  public static Bounds ComputeSetPointBounds(GameObject go) => TransformUtil.ComputeSetPointBounds(go, false);

  public static Bounds ComputeSetPointBounds(Component c, bool includeInactive) => TransformUtil.ComputeSetPointBounds(c.gameObject, includeInactive);

  public static Bounds ComputeSetPointBounds(GameObject go, bool includeInactive)
  {
    UberText component1 = go.GetComponent<UberText>();
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      return component1.GetTextWorldSpaceBounds();
    Renderer component2 = go.GetComponent<Renderer>();
    if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
      return component2.bounds;
    BoundsOverride component3 = go.GetComponent<BoundsOverride>();
    if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
      return component3.bounds;
    Collider component4 = go.GetComponent<Collider>();
    if (!((UnityEngine.Object) component4 != (UnityEngine.Object) null))
      return TransformUtil.GetBoundsOfChildren(go, includeInactive);
    Bounds bounds;
    if (component4.enabled)
    {
      bounds = component4.bounds;
    }
    else
    {
      component4.enabled = true;
      bounds = component4.bounds;
      component4.enabled = false;
    }
    MobileHitBox component5 = go.GetComponent<MobileHitBox>();
    if ((UnityEngine.Object) component5 != (UnityEngine.Object) null && component5.HasExecuted())
      bounds.size = new Vector3(bounds.size.x / component5.m_scaleX, bounds.size.y / component5.m_scaleY, bounds.size.z / component5.m_scaleY);
    return bounds;
  }

  public static OrientedBounds ComputeOrientedWorldBounds(
    GameObject go,
    bool includeAllChildren = true)
  {
    return TransformUtil.ComputeOrientedWorldBounds(go, true, includeAllChildren);
  }

  public static OrientedBounds ComputeOrientedWorldBounds(
    GameObject go,
    List<GameObject> ignoreMeshes,
    bool includeAllChildren = true)
  {
    return TransformUtil.ComputeOrientedWorldBounds(go, true, ignoreMeshes, includeAllChildren);
  }

  public static OrientedBounds ComputeOrientedWorldBounds(
    GameObject go,
    bool includeUberText,
    bool includeAllChildren = true)
  {
    return TransformUtil.ComputeOrientedWorldBounds(go, includeUberText, Vector3.zero, Vector3.zero, (List<GameObject>) null, includeAllChildren);
  }

  public static OrientedBounds ComputeOrientedWorldBounds(
    GameObject go,
    bool includeUberText,
    List<GameObject> ignoreMeshes,
    bool includeAllChildren = true)
  {
    return TransformUtil.ComputeOrientedWorldBounds(go, includeUberText, Vector3.zero, Vector3.zero, ignoreMeshes, includeAllChildren);
  }

  public static OrientedBounds ComputeOrientedWorldBounds(
    GameObject go,
    bool includeUberText,
    Vector3 minLocalPadding,
    Vector3 maxLocalPadding,
    List<GameObject> ignoreMeshes,
    bool includeAllChildren = true)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null || !go.activeSelf)
      return (OrientedBounds) null;
    List<MeshFilter> componentsWithIgnore = TransformUtil.GetComponentsWithIgnore<MeshFilter>(go, ignoreMeshes, includeAllChildren);
    List<UberText> uberTextList = (List<UberText>) null;
    if (includeUberText)
      uberTextList = TransformUtil.GetComponentsWithIgnore<UberText>(go, ignoreMeshes, includeAllChildren);
    if ((componentsWithIgnore == null || componentsWithIgnore.Count == 0) && (uberTextList == null || uberTextList.Count == 0))
      return (OrientedBounds) null;
    Matrix4x4 worldToLocalMatrix = go.transform.worldToLocalMatrix;
    Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    if (componentsWithIgnore != null)
    {
      foreach (MeshFilter meshFilter in componentsWithIgnore)
      {
        if (meshFilter.gameObject.activeSelf && !((UnityEngine.Object) meshFilter.sharedMesh == (UnityEngine.Object) null))
        {
          Matrix4x4 localToWorldMatrix = meshFilter.transform.localToWorldMatrix;
          Bounds bounds = meshFilter.sharedMesh.bounds;
          Matrix4x4 matrix4x4 = worldToLocalMatrix * localToWorldMatrix;
          Vector3[] vector3Array = new Vector3[3]
          {
            (Vector3) (matrix4x4 * (Vector4) new Vector3(bounds.extents.x, 0.0f, 0.0f)),
            (Vector3) (matrix4x4 * (Vector4) new Vector3(0.0f, bounds.extents.y, 0.0f)),
            (Vector3) (matrix4x4 * (Vector4) new Vector3(0.0f, 0.0f, bounds.extents.z))
          };
          Vector3 vector3 = (Vector3) (localToWorldMatrix * (Vector4) meshFilter.sharedMesh.bounds.center);
          TransformUtil.GetBoundsMinMax((Vector3) (worldToLocalMatrix * (Vector4) (meshFilter.transform.position + vector3)), vector3Array[0], vector3Array[1], vector3Array[2], ref min, ref max);
        }
      }
    }
    if (uberTextList != null)
    {
      foreach (UberText uberText in uberTextList)
      {
        if (uberText.gameObject.activeSelf)
        {
          Matrix4x4 localToWorldMatrix = uberText.transform.localToWorldMatrix;
          Matrix4x4 matrix4x4 = worldToLocalMatrix * localToWorldMatrix;
          Vector3[] vector3Array = new Vector3[3]
          {
            (Vector3) (matrix4x4 * (Vector4) new Vector3(uberText.Width * 0.5f, 0.0f, 0.0f)),
            (Vector3) (matrix4x4 * (Vector4) new Vector3(0.0f, uberText.Height * 0.5f)),
            (Vector3) (matrix4x4 * (Vector4) new Vector3(0.0f, 0.0f, 0.01f))
          };
          TransformUtil.GetBoundsMinMax((Vector3) (worldToLocalMatrix * (Vector4) uberText.transform.position), vector3Array[0], vector3Array[1], vector3Array[2], ref min, ref max);
        }
      }
    }
    if ((double) minLocalPadding.sqrMagnitude > 0.0)
      min -= minLocalPadding;
    if ((double) maxLocalPadding.sqrMagnitude > 0.0)
      max += maxLocalPadding;
    Matrix4x4 localToWorldMatrix1 = go.transform.localToWorldMatrix;
    Matrix4x4 matrix4x4_1 = localToWorldMatrix1;
    matrix4x4_1.SetColumn(3, Vector4.zero);
    Vector3 vector3_1 = (Vector3) ((localToWorldMatrix1 * (Vector4) max + localToWorldMatrix1 * (Vector4) min) * 0.5f);
    Vector3 vector3_2 = (max - min) * 0.5f;
    return new OrientedBounds()
    {
      Extents = new Vector3[3]
      {
        (Vector3) (matrix4x4_1 * (Vector4) new Vector3(vector3_2.x, 0.0f, 0.0f)),
        (Vector3) (matrix4x4_1 * (Vector4) new Vector3(0.0f, vector3_2.y, 0.0f)),
        (Vector3) (matrix4x4_1 * (Vector4) new Vector3(0.0f, 0.0f, vector3_2.z))
      },
      Origin = vector3_1,
      CenterOffset = go.transform.position - vector3_1
    };
  }

  public static bool CanComputeOrientedWorldBounds(
    GameObject go,
    bool includeUberText,
    List<GameObject> ignoreMeshes,
    bool includeAllChildren = true)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null || !go.activeSelf)
      return false;
    List<MeshFilter> componentsWithIgnore1 = TransformUtil.GetComponentsWithIgnore<MeshFilter>(go, ignoreMeshes, includeAllChildren);
    if (componentsWithIgnore1 != null && componentsWithIgnore1.Count > 0)
      return true;
    if (!includeUberText)
      return false;
    List<UberText> componentsWithIgnore2 = TransformUtil.GetComponentsWithIgnore<UberText>(go, ignoreMeshes, includeAllChildren);
    return componentsWithIgnore2 != null && componentsWithIgnore2.Count > 0;
  }

  public static List<T> GetComponentsWithIgnore<T>(
    GameObject obj,
    List<GameObject> ignoreObjects,
    bool includeAllChildren = true)
    where T : Component
  {
    List<T> componentsWithIgnore = new List<T>();
    if (includeAllChildren)
      obj.GetComponentsInChildren<T>(componentsWithIgnore);
    T component = obj.GetComponent<T>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      componentsWithIgnore.Add(component);
    if (ignoreObjects != null && ignoreObjects.Count > 0)
    {
      T[] array = componentsWithIgnore.ToArray();
      componentsWithIgnore.Clear();
      foreach (T obj1 in array)
      {
        bool flag = true;
        foreach (GameObject ignoreObject in ignoreObjects)
        {
          if ((UnityEngine.Object) ignoreObject == (UnityEngine.Object) null || (UnityEngine.Object) obj1.transform == (UnityEngine.Object) ignoreObject.transform || obj1.transform.IsChildOf(ignoreObject.transform))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          componentsWithIgnore.Add(obj1);
      }
    }
    return componentsWithIgnore;
  }

  public static Vector3[] GetBoundCorners(
    Vector3 origin,
    Vector3 xExtent,
    Vector3 yExtent,
    Vector3 zExtent)
  {
    Vector3 vector3_1 = origin + xExtent;
    Vector3 vector3_2 = origin - xExtent;
    Vector3 vector3_3 = yExtent + zExtent;
    Vector3 vector3_4 = yExtent - zExtent;
    Vector3 vector3_5 = -yExtent + zExtent;
    Vector3 vector3_6 = -yExtent - zExtent;
    return new Vector3[8]
    {
      vector3_1 + vector3_3,
      vector3_1 + vector3_4,
      vector3_1 + vector3_5,
      vector3_1 + vector3_6,
      vector3_2 - vector3_3,
      vector3_2 - vector3_4,
      vector3_2 - vector3_5,
      vector3_2 - vector3_6
    };
  }

  public static void GetBoundsMinMax(
    Vector3 origin,
    Vector3 xExtent,
    Vector3 yExtent,
    Vector3 zExtent,
    ref Vector3 min,
    ref Vector3 max)
  {
    Vector3[] boundCorners = TransformUtil.GetBoundCorners(origin, xExtent, yExtent, zExtent);
    for (int index = 0; index < boundCorners.Length; ++index)
    {
      min.x = Mathf.Min(boundCorners[index].x, min.x);
      min.y = Mathf.Min(boundCorners[index].y, min.y);
      min.z = Mathf.Min(boundCorners[index].z, min.z);
      max.x = Mathf.Max(boundCorners[index].x, max.x);
      max.y = Mathf.Max(boundCorners[index].y, max.y);
      max.z = Mathf.Max(boundCorners[index].z, max.z);
    }
  }

  public static void SetLocalScaleToWorldDimension(
    GameObject obj,
    params WorldDimensionIndex[] dimensions)
  {
    TransformUtil.SetLocalScaleToWorldDimension(obj, (List<GameObject>) null, dimensions);
  }

  public static void SetLocalScaleToWorldDimension(
    GameObject obj,
    List<GameObject> ignoreMeshes,
    params WorldDimensionIndex[] dimensions)
  {
    Vector3 localScale = obj.transform.localScale;
    OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds(obj, ignoreMeshes);
    for (int index = 0; index < dimensions.Length; ++index)
    {
      float num = orientedWorldBounds.Extents[dimensions[index].Index].magnitude * 2f;
      localScale[dimensions[index].Index] *= (double) num <= (double) Mathf.Epsilon ? 1f / 1000f : dimensions[index].Dimension / num;
      if ((double) Mathf.Abs(localScale[dimensions[index].Index]) < 1.0 / 1000.0)
        localScale[dimensions[index].Index] = 1f / 1000f;
    }
    obj.transform.localScale = localScale;
  }

  public static void SetPoint(Component src, Anchor srcAnchor, Component dst, Anchor dstAnchor) => TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);

  public static void SetPoint(Component src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor) => TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);

  public static void SetPoint(GameObject src, Anchor srcAnchor, Component dst, Anchor dstAnchor) => TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);

  public static void SetPoint(GameObject src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor) => TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);

  public static void SetPoint(
    Component src,
    Anchor srcAnchor,
    Component dst,
    Anchor dstAnchor,
    Vector3 offset)
  {
    TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
  }

  public static void SetPoint(
    Component src,
    Anchor srcAnchor,
    GameObject dst,
    Anchor dstAnchor,
    Vector3 offset)
  {
    TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
  }

  public static void SetPoint(
    GameObject src,
    Anchor srcAnchor,
    Component dst,
    Anchor dstAnchor,
    Vector3 offset)
  {
    TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
  }

  public static void SetPoint(
    GameObject src,
    Anchor srcAnchor,
    GameObject dst,
    Anchor dstAnchor,
    Vector3 offset)
  {
    TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
  }

  public static void SetPoint(
    GameObject src,
    Anchor srcAnchor,
    GameObject dst,
    Anchor dstAnchor,
    Vector3 offset,
    bool includeInactive)
  {
    TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), offset, includeInactive);
  }

  public static void SetPoint(
    Component self,
    Vector3 selfUnitAnchor,
    GameObject relative,
    Vector3 relativeUnitAnchor)
  {
    TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative, relativeUnitAnchor, Vector3.zero, false);
  }

  public static void SetPoint(
    GameObject self,
    Vector3 selfUnitAnchor,
    GameObject relative,
    Vector3 relativeUnitAnchor)
  {
    TransformUtil.SetPoint(self, selfUnitAnchor, relative, relativeUnitAnchor, Vector3.zero, false);
  }

  public static void SetPoint(
    Component self,
    Vector3 selfUnitAnchor,
    Component relative,
    Vector3 relativeUnitAnchor,
    Vector3 offset)
  {
    TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative.gameObject, relativeUnitAnchor, offset, false);
  }

  public static void SetPoint(
    Component self,
    Vector3 selfUnitAnchor,
    GameObject relative,
    Vector3 relativeUnitAnchor,
    Vector3 offset)
  {
    TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative, relativeUnitAnchor, offset, false);
  }

  public static void SetPoint(
    GameObject self,
    Vector3 selfUnitAnchor,
    GameObject relative,
    Vector3 relativeUnitAnchor,
    Vector3 offset)
  {
    TransformUtil.SetPoint(self, selfUnitAnchor, relative, relativeUnitAnchor, offset, false);
  }

  public static void SetPoint(
    GameObject self,
    Vector3 selfUnitAnchor,
    GameObject relative,
    Vector3 relativeUnitAnchor,
    Vector3 offset,
    bool includeInactive)
  {
    if (!(bool) (UnityEngine.Object) self || !(bool) (UnityEngine.Object) relative)
      return;
    Bounds setPointBounds1 = TransformUtil.ComputeSetPointBounds(self, includeInactive);
    Bounds setPointBounds2 = TransformUtil.ComputeSetPointBounds(relative, includeInactive);
    Vector3 selfUnitAnchor1 = selfUnitAnchor;
    Vector3 worldPoint1 = TransformUtil.ComputeWorldPoint(setPointBounds1, selfUnitAnchor1);
    Vector3 worldPoint2 = TransformUtil.ComputeWorldPoint(setPointBounds2, relativeUnitAnchor);
    Vector3 translation = new Vector3(worldPoint2.x - worldPoint1.x + offset.x, worldPoint2.y - worldPoint1.y + offset.y, worldPoint2.z - worldPoint1.z + offset.z);
    self.transform.Translate(translation, Space.World);
  }

  public static Bounds GetBoundsOfChildren(Component c) => TransformUtil.GetBoundsOfChildren(c.gameObject, false);

  public static Bounds GetBoundsOfChildren(GameObject go) => TransformUtil.GetBoundsOfChildren(go, false);

  public static Bounds GetBoundsOfChildren(Component c, bool includeInactive) => TransformUtil.GetBoundsOfChildren(c.gameObject, includeInactive);

  public static Bounds GetBoundsOfChildren(GameObject go, bool includeInactive)
  {
    Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(includeInactive);
    if (componentsInChildren.Length == 0)
      return new Bounds(go.transform.position, Vector3.zero);
    Bounds bounds1 = componentsInChildren[0].bounds;
    for (int index = 1; index < componentsInChildren.Length; ++index)
    {
      Bounds bounds2 = componentsInChildren[index].bounds;
      Vector3 max = Vector3.Max(bounds2.max, bounds1.max);
      Vector3 min = Vector3.Min(bounds2.min, bounds1.min);
      bounds1.SetMinMax(min, max);
    }
    return bounds1;
  }

  public static void SetLocalPosX(GameObject go, float x)
  {
    Transform transform = go.transform;
    transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
  }

  public static void SetLocalPosX(Component component, float x)
  {
    Transform transform = component.transform;
    transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
  }

  public static void SetLocalPosY(GameObject go, float y)
  {
    Transform transform = go.transform;
    transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
  }

  public static void SetLocalPosY(Component component, float y)
  {
    Transform transform = component.transform;
    transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
  }

  public static void SetLocalPosZ(GameObject go, float z)
  {
    Transform transform = go.transform;
    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
  }

  public static void SetLocalPosZ(Component component, float z)
  {
    Transform transform = component.transform;
    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
  }

  public static void SetPosX(GameObject go, float x)
  {
    Transform transform = go.transform;
    transform.position = new Vector3(x, transform.position.y, transform.position.z);
  }

  public static void SetPosX(Component component, float x)
  {
    Transform transform = component.transform;
    transform.position = new Vector3(x, transform.position.y, transform.position.z);
  }

  public static void SetPosY(GameObject go, float y)
  {
    Transform transform = go.transform;
    transform.position = new Vector3(transform.position.x, y, transform.position.z);
  }

  public static void SetPosY(Component component, float y)
  {
    Transform transform = component.transform;
    transform.position = new Vector3(transform.position.x, y, transform.position.z);
  }

  public static void SetPosZ(GameObject go, float z)
  {
    Transform transform = go.transform;
    transform.position = new Vector3(transform.position.x, transform.position.y, z);
  }

  public static void SetPosZ(Component component, float z)
  {
    Transform transform = component.transform;
    transform.position = new Vector3(transform.position.x, transform.position.y, z);
  }

  public static void SetLocalEulerAngleX(GameObject go, float x)
  {
    Transform transform = go.transform;
    transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
  }

  public static void SetLocalEulerAngleY(GameObject go, float y)
  {
    Transform transform = go.transform;
    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
  }

  public static void SetEulerAngleX(GameObject go, float x)
  {
    Transform transform = go.transform;
    transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
  }

  public static void SetEulerAngleY(GameObject go, float y)
  {
    Transform transform = go.transform;
    transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
  }

  public static void SetEulerAngleZ(GameObject go, float z)
  {
    Transform transform = go.transform;
    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
  }

  public static void SetLocalScaleX(Component component, float x)
  {
    Transform transform = component.transform;
    transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
  }

  public static void SetLocalScaleX(GameObject go, float x)
  {
    Transform transform = go.transform;
    transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
  }

  public static void SetLocalScaleY(GameObject go, float y)
  {
    Transform transform = go.transform;
    transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
  }

  public static void SetLocalScaleZ(Component component, float z)
  {
    Transform transform = component.transform;
    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
  }

  public static void SetLocalScaleZ(GameObject go, float z)
  {
    Transform transform = go.transform;
    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
  }

  public static void SetLocalScaleXY(GameObject go, float x, float y)
  {
    Transform transform = go.transform;
    transform.localScale = new Vector3(x, y, transform.localScale.z);
  }

  public static void SetLocalScaleXY(Component component, Vector2 v)
  {
    Transform transform = component.transform;
    transform.localScale = new Vector3(v.x, v.y, transform.localScale.z);
  }

  public static void SetLocalScaleXZ(GameObject go, Vector2 v)
  {
    Transform transform = go.transform;
    transform.localScale = new Vector3(v.x, transform.localScale.y, v.y);
  }

  public static void Identity(Component c)
  {
    c.transform.localScale = Vector3.one;
    c.transform.localRotation = Quaternion.identity;
    c.transform.localPosition = Vector3.zero;
  }

  public static void Identity(GameObject go)
  {
    go.transform.localScale = Vector3.one;
    go.transform.localRotation = Quaternion.identity;
    go.transform.localPosition = Vector3.zero;
  }

  public static void CopyLocal(Component destination, Component source) => TransformUtil.CopyLocal(destination.gameObject, source.gameObject);

  public static void CopyLocal(GameObject destination, Component source) => TransformUtil.CopyLocal(destination, source.gameObject);

  public static void CopyLocal(GameObject destination, GameObject source)
  {
    destination.transform.localScale = source.transform.localScale;
    destination.transform.localRotation = source.transform.localRotation;
    destination.transform.localPosition = source.transform.localPosition;
  }

  public static void CopyLocal(Component destination, TransformProps source) => TransformUtil.CopyLocal(destination.gameObject, source);

  public static void CopyLocal(GameObject destination, TransformProps source)
  {
    destination.transform.localScale = source.scale;
    destination.transform.localRotation = source.rotation;
    destination.transform.localPosition = source.position;
  }

  public static TransformProps GetLocalTransformProps(Component source) => TransformUtil.GetLocalTransformProps(source.gameObject);

  public static TransformProps GetLocalTransformProps(GameObject source) => new TransformProps()
  {
    scale = source.transform.localScale,
    rotation = source.transform.localRotation,
    position = source.transform.localPosition
  };

  public static void CopyWorld(Component destination, Component source)
  {
    if (!((UnityEngine.Object) destination != (UnityEngine.Object) null))
      return;
    TransformUtil.CopyWorld(destination.gameObject, source);
  }

  public static void CopyWorld(Component destination, GameObject source)
  {
    if (!((UnityEngine.Object) destination != (UnityEngine.Object) null))
      return;
    TransformUtil.CopyWorld(destination.gameObject, source);
  }

  public static void CopyWorld(GameObject destination, Component source)
  {
    if (!((UnityEngine.Object) source != (UnityEngine.Object) null))
      return;
    TransformUtil.CopyWorld(destination, source.gameObject);
  }

  public static void CopyWorld(GameObject destination, GameObject source)
  {
    TransformUtil.CopyWorldScale(destination, source);
    destination.transform.rotation = source.transform.rotation;
    destination.transform.position = source.transform.position;
  }

  public static void CopyWorld(Component destination, TransformProps source) => TransformUtil.CopyWorld(destination.gameObject, source);

  public static void CopyWorld(GameObject destination, TransformProps source)
  {
    TransformUtil.SetWorldScale(destination, source.scale);
    destination.transform.rotation = source.rotation;
    destination.transform.position = source.position;
  }

  public static TransformProps GetWorldTransformProps(Component source) => TransformUtil.GetWorldTransformProps(source.gameObject);

  public static TransformProps GetWorldTransformProps(GameObject source) => new TransformProps()
  {
    scale = TransformUtil.ComputeWorldScale(source),
    rotation = source.transform.rotation,
    position = source.transform.position
  };

  public static void CopyWorldScale(Component destination, Component source) => TransformUtil.CopyWorldScale(destination.gameObject, source.gameObject);

  public static void CopyWorldScale(GameObject destination, GameObject source)
  {
    Vector3 worldScale = TransformUtil.ComputeWorldScale(source);
    TransformUtil.SetWorldScale(destination, worldScale);
  }

  public static void SetWorldScale(Component destination, Vector3 scale) => TransformUtil.SetWorldScale(destination.gameObject, scale);

  public static void SetWorldScale(GameObject destination, Vector3 scale)
  {
    if ((UnityEngine.Object) destination.transform.parent != (UnityEngine.Object) null)
    {
      for (Transform parent = destination.transform.parent; (UnityEngine.Object) parent != (UnityEngine.Object) null; parent = parent.parent)
        scale.Scale(TransformUtil.Vector3Reciprocal(parent.localScale));
    }
    destination.transform.localScale = scale;
  }

  public static Vector3 ComputeWorldScale(Component c) => TransformUtil.ComputeWorldScale(c.gameObject);

  public static Vector3 ComputeWorldScale(GameObject go)
  {
    Vector3 localScale = go.transform.localScale;
    if ((UnityEngine.Object) go.transform.parent != (UnityEngine.Object) null)
    {
      for (Transform parent = go.transform.parent; (UnityEngine.Object) parent != (UnityEngine.Object) null; parent = parent.parent)
        localScale.Scale(parent.localScale);
    }
    return localScale;
  }

  public static Vector3 Vector3Reciprocal(Vector3 source)
  {
    Vector3 vector3 = source;
    if ((double) vector3.x != 0.0)
      vector3.x = 1f / vector3.x;
    if ((double) vector3.y != 0.0)
      vector3.y = 1f / vector3.y;
    if ((double) vector3.z != 0.0)
      vector3.z = 1f / vector3.z;
    return vector3;
  }

  public static Vector3 RandomVector3(Vector3 min, Vector3 max) => new Vector3()
  {
    x = UnityEngine.Random.Range(min.x, max.x),
    y = UnityEngine.Random.Range(min.y, max.y),
    z = UnityEngine.Random.Range(min.z, max.z)
  };

  public static void AttachAndPreserveLocalTransform(Transform child, Transform parent)
  {
    TransformProps localTransformProps = TransformUtil.GetLocalTransformProps((Component) child);
    child.parent = parent;
    TransformUtil.CopyLocal((Component) child, localTransformProps);
  }

  public static float GetAspectRatioValue(TransformUtil.PhoneAspectRatio aspectRatio)
  {
    switch (aspectRatio)
    {
      case TransformUtil.PhoneAspectRatio.Minimum:
        return 1.5f;
      case TransformUtil.PhoneAspectRatio.Wide:
        return 1.777778f;
      case TransformUtil.PhoneAspectRatio.ExtraWide:
        return 2.04f;
      default:
        return 0.0f;
    }
  }

  public static Vector3 GetAspectRatioDependentPosition(
    Vector3 aspectSmall,
    Vector3 aspectWide,
    Vector3 aspectExtraWide)
  {
    return TransformUtil.GetAspectRatioDependentValue<Vector3>(new Func<Vector3, Vector3, float, Vector3>(Vector3.Lerp), aspectSmall, aspectWide, aspectExtraWide);
  }

  public static float GetAspectRatioDependentValue(
    float aspectSmall,
    float aspectWide,
    float aspectExtraWide)
  {
    return TransformUtil.GetAspectRatioDependentValue<float>(new Func<float, float, float, float>(Mathf.Lerp), aspectSmall, aspectWide, aspectExtraWide);
  }

  private static T GetAspectRatioDependentValue<T>(
    Func<T, T, float, T> interpolator,
    T small,
    T wide,
    T extraWide)
  {
    Dictionary<TransformUtil.PhoneAspectRatio, T> dictionary = new Dictionary<TransformUtil.PhoneAspectRatio, T>()
    {
      {
        TransformUtil.PhoneAspectRatio.Minimum,
        small
      },
      {
        TransformUtil.PhoneAspectRatio.Wide,
        wide
      },
      {
        TransformUtil.PhoneAspectRatio.ExtraWide,
        extraWide
      }
    };
    TransformUtil.PhoneAspectRatio lowerRatio;
    TransformUtil.PhoneAspectRatio upperRatio;
    float num = TransformUtil.PhoneAspectRatioScale(out lowerRatio, out upperRatio);
    return interpolator(dictionary[lowerRatio], dictionary[upperRatio], num);
  }

  public static bool IsExtraWideAspectRatio() => (double) TransformUtil.GetAspectRatioDependentValue(0.0f, 1f, 2f) > 1.20000004768372;

  private static float PhoneAspectRatioScale(
    out TransformUtil.PhoneAspectRatio lowerRatio,
    out TransformUtil.PhoneAspectRatio upperRatio)
  {
    float num1 = (float) Screen.width / (float) Screen.height;
    lowerRatio = TransformUtil.PhoneAspectRatio.Minimum;
    upperRatio = TransformUtil.PhoneAspectRatio.ExtraWide;
    int num2 = EnumUtils.Length<TransformUtil.PhoneAspectRatio>();
    for (int index = 0; index < num2; ++index)
    {
      TransformUtil.PhoneAspectRatio aspectRatio = (TransformUtil.PhoneAspectRatio) index;
      if ((double) TransformUtil.GetAspectRatioValue(aspectRatio) > (double) num1)
      {
        lowerRatio = index > 0 ? (TransformUtil.PhoneAspectRatio) (index - 1) : TransformUtil.PhoneAspectRatio.Minimum;
        upperRatio = index == 0 ? (TransformUtil.PhoneAspectRatio) (index + 1) : aspectRatio;
        break;
      }
    }
    float aspectRatioValue1 = TransformUtil.GetAspectRatioValue(lowerRatio);
    float aspectRatioValue2 = TransformUtil.GetAspectRatioValue(upperRatio);
    float num3 = aspectRatioValue2 - aspectRatioValue1;
    return (Mathf.Clamp(num1, aspectRatioValue1, aspectRatioValue2) - aspectRatioValue1) / num3;
  }

  public static void ConstrainToScreen(GameObject go, int layer)
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(layer);
    if ((UnityEngine.Object) firstByLayer == (UnityEngine.Object) null)
    {
      Log.All.PrintError("TransformUtil.ConstrainToScreen - No camera found for indicated layer.");
    }
    else
    {
      Bounds setPointBounds = TransformUtil.ComputeSetPointBounds(go);
      Vector3[] outCorners = new Vector3[4];
      Vector3 vector3_1 = firstByLayer.transform.InverseTransformPoint(setPointBounds.center);
      firstByLayer.CalculateFrustumCorners(new Rect(0.0f, 0.0f, 1f, 1f), vector3_1.z, Camera.MonoOrStereoscopicEye.Mono, outCorners);
      Bounds bounds = new Bounds(firstByLayer.transform.TransformPoint(outCorners[0]), new Vector3());
      for (int index = 1; index < 4; ++index)
      {
        Vector3 point = firstByLayer.transform.TransformPoint(outCorners[index]);
        bounds.Encapsulate(point);
      }
      Vector3 position = go.transform.position;
      bounds.SetMinMax(bounds.min - (setPointBounds.min - position), bounds.max - (setPointBounds.max - position));
      Vector3 rhs = bounds.ClosestPoint(position) - position;
      Vector3 vector3_2 = rhs - firstByLayer.transform.forward * Vector3.Dot(firstByLayer.transform.forward, rhs);
      go.transform.position += vector3_2;
    }
  }

  public enum PhoneAspectRatio
  {
    Minimum = 0,
    Wide = 1,
    ExtraWide = 2,
    Maximum = 2,
  }
}
