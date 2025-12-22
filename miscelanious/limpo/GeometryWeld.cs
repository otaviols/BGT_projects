using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeometryWeld
{
  private static readonly bool DEBUG;
  public GameObject weldedGameObject;
  private IEnumerable<MeshRenderer> meshRenderers;

  public GeometryWeld(GameObject root, params GameObject[] objectsToWeld)
  {
    if (!Application.IsPlaying((UnityEngine.Object) root))
      return;
    IEnumerable<GameObject> source1 = ((IEnumerable<GameObject>) new GameObject[1]
    {
      root
    }).Concat<GameObject>(((IEnumerable<GameObject>) objectsToWeld).Where<GameObject>((Func<GameObject, bool>) (x => (UnityEngine.Object) x != (UnityEngine.Object) root))).Where<GameObject>((Func<GameObject, bool>) (x => (UnityEngine.Object) x.GetComponent<MeshRenderer>() != (UnityEngine.Object) null));
    IEnumerable<MeshFilter> source2 = source1.Select<GameObject, MeshFilter>((Func<GameObject, MeshFilter>) (x => x.GetComponent<MeshFilter>()));
    this.meshRenderers = source1.Select<GameObject, MeshRenderer>((Func<GameObject, MeshRenderer>) (x => x.GetComponent<MeshRenderer>()));
    List<Material> rootMaterials = this.meshRenderers.First<MeshRenderer>().GetSharedMaterials();
    Func<Material[], bool> predicate = (Func<Material[], bool>) (materials =>
    {
      if (materials.Length != rootMaterials.Count)
        return false;
      for (int index = 0; index < rootMaterials.Count; ++index)
      {
        if ((UnityEngine.Object) materials[index] != (UnityEngine.Object) rootMaterials[index])
          return false;
      }
      return true;
    });
    if (!this.meshRenderers.Skip<MeshRenderer>(1).Select<MeshRenderer, Material[]>((Func<MeshRenderer, Material[]>) (x => x.GetSharedMaterials().ToArray())).All<Material[]>(predicate))
    {
      Error.AddDevFatal("Unable to weld {0} to {1}.  Materials differ.", (object) root.name, (object) string.Join(", ", ((IEnumerable<GameObject>) objectsToWeld).Select<GameObject, string>((Func<GameObject, string>) (x => x.name)).ToArray<string>()));
    }
    else
    {
      this.weldedGameObject = new GameObject("Welded_" + root.name);
      this.weldedGameObject.AddComponent<MeshFilter>().sharedMesh = GeometryWeld.CombineMeshes((IEnumerable<CombineInstance>) source2.Select<MeshFilter, CombineInstance>((Func<MeshFilter, CombineInstance>) (x => new CombineInstance()
      {
        mesh = x.sharedMesh,
        transform = root.transform.worldToLocalMatrix * x.transform.localToWorldMatrix
      })).ToArray<CombineInstance>());
      this.weldedGameObject.AddComponent<MeshRenderer>().SetSharedMaterials(rootMaterials);
      this.weldedGameObject.transform.SetParent(root.transform.parent);
      this.weldedGameObject.transform.position = root.transform.position;
      this.weldedGameObject.transform.rotation = root.transform.rotation;
      this.weldedGameObject.transform.localScale = root.transform.localScale;
      this.weldedGameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
      if (GeometryWeld.DEBUG)
        return;
      foreach (Renderer meshRenderer in this.meshRenderers)
        meshRenderer.enabled = false;
    }
  }

  private static Mesh CombineMeshes(IEnumerable<CombineInstance> combines)
  {
    Mesh mesh = new Mesh();
    int destinationIndex = 0;
    int num = 0;
    int length1 = combines.Select<CombineInstance, int>((Func<CombineInstance, int>) (x => x.mesh.vertexCount)).Sum();
    Vector3[] verticies = new Vector3[length1];
    int[] numArray = new int[combines.Select<CombineInstance, int>((Func<CombineInstance, int>) (x => x.mesh.triangles.Length)).Sum()];
    Vector2[] destinationArray1 = new Vector2[length1];
    Vector3[] destinationArray2 = new Vector3[length1];
    foreach (CombineInstance combine in combines)
    {
      Vector3[] vertices = combine.mesh.vertices;
      int length2 = vertices.Length;
      Array.Copy((Array) combine.mesh.uv, 0, (Array) destinationArray1, destinationIndex, length2);
      Array.Copy((Array) combine.mesh.normals, 0, (Array) destinationArray2, destinationIndex, length2);
      for (int index = 0; index < length2; ++index)
      {
        Vector3 vector3 = vertices[index];
        Vector4 vector4 = new Vector4(vector3.x, vector3.y, vector3.z, 1f);
        vector4 = combine.transform * vector4;
        verticies[destinationIndex + index] = (Vector3) vector4;
      }
      int[] triangles = combine.mesh.triangles;
      int length3 = triangles.Length;
      for (int index = 0; index < length3; ++index)
        numArray[num++] = triangles[index] + destinationIndex;
      destinationIndex += length2;
    }
    GeometryWeld.ClampMeshes(verticies, combines, 0.03f, 20f);
    GeometryWeld.StretchTriangles(verticies, combines, 0.005f);
    mesh.vertices = verticies;
    mesh.triangles = numArray;
    mesh.uv = destinationArray1;
    mesh.normals = destinationArray2;
    return mesh;
  }

  private static void ClampMeshes(
    Vector3[] verticies,
    IEnumerable<CombineInstance> meshRanges,
    float clampSqrDistance,
    float clampErrorAngle)
  {
    List<GeometryWeld.SuggestedTranslation> source = new List<GeometryWeld.SuggestedTranslation>();
    int num1 = -1;
    foreach (CombineInstance meshRange in meshRanges)
    {
      source.Clear();
      int num2 = num1;
      num1 += meshRange.mesh.vertexCount;
      for (int index1 = 0; index1 <= num2; ++index1)
      {
        Vector3 verticy1 = verticies[index1];
        for (int index2 = num2 + 1; index2 <= num1; ++index2)
        {
          Vector3 verticy2 = verticies[index2];
          Vector3 from = verticy1 - verticy2;
          if ((double) from.sqrMagnitude <= (double) clampSqrDistance)
          {
            Vector4 vector4 = new Vector4(verticy2.x, verticy2.y, verticy2.z, 1f);
            Vector3 to = (Vector3) (meshRange.transform * vector4 - vector4);
            float num3 = Vector3.Angle(from, to);
            if ((double) num3 < (double) clampErrorAngle)
            {
              GeometryWeld.SuggestedTranslation suggestedTranslation = new GeometryWeld.SuggestedTranslation()
              {
                translation = from
              };
              suggestedTranslation.startIndicies.Add(index2);
              suggestedTranslation.endIndicies.Add(index1);
              source.Add(suggestedTranslation);
            }
            else if ((double) num3 + (double) clampErrorAngle > 180.0)
            {
              GeometryWeld.SuggestedTranslation suggestedTranslation = new GeometryWeld.SuggestedTranslation()
              {
                translation = -from
              };
              suggestedTranslation.startIndicies.Add(index2);
              suggestedTranslation.endIndicies.Add(index1);
              source.Add(suggestedTranslation);
            }
          }
        }
      }
      int count = source.Count;
      for (int index3 = 0; index3 < source.Count; ++index3)
      {
        for (int index4 = index3 + 1; index4 < source.Count; ++index4)
        {
          if (source[index3].MergeWith(source[index4], clampErrorAngle))
          {
            source.RemoveAt(index4);
            --index4;
          }
        }
      }
      GeometryWeld.SuggestedTranslation suggestedTranslation1 = source.OrderBy<GeometryWeld.SuggestedTranslation, int>((Func<GeometryWeld.SuggestedTranslation, int>) (x => x.startIndicies.Count)).FirstOrDefault<GeometryWeld.SuggestedTranslation>();
      if (suggestedTranslation1 != null && suggestedTranslation1.startIndicies.Count > count / 2)
      {
        for (int index5 = num2 + 1; index5 <= num1; ++index5)
        {
          int index6 = suggestedTranslation1.startIndicies.IndexOf(index5);
          if (index6 == -1)
            verticies[index5] += suggestedTranslation1.translation;
          else
            verticies[suggestedTranslation1.startIndicies[index6]] = verticies[suggestedTranslation1.endIndicies[index6]];
        }
      }
    }
  }

  private static void StretchTriangles(
    Vector3[] verticies,
    IEnumerable<CombineInstance> meshRanges,
    float strechSqrDistance)
  {
    int num1 = -1;
    int num2 = -1;
    foreach (CombineInstance meshRange in meshRanges)
    {
      for (int index1 = 0; index1 <= num1; ++index1)
      {
        for (int index2 = num1 + 1; index2 <= num2; ++index2)
        {
          if ((double) (verticies[index1] - verticies[index2]).sqrMagnitude <= (double) strechSqrDistance)
            verticies[index2] = verticies[index1];
        }
      }
    }
  }

  public void Unweld()
  {
    if ((UnityEngine.Object) this.weldedGameObject == (UnityEngine.Object) null || !Application.IsPlaying((UnityEngine.Object) this.weldedGameObject))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.weldedGameObject);
    foreach (Renderer meshRenderer in this.meshRenderers)
      meshRenderer.enabled = true;
  }

  protected class SuggestedTranslation
  {
    public Vector3 translation;
    public List<int> startIndicies = new List<int>();
    public List<int> endIndicies = new List<int>();

    public bool MergeWith(GeometryWeld.SuggestedTranslation other, float clampErrorAngle)
    {
      if ((double) Vector3.Angle(this.translation, other.translation) > (double) clampErrorAngle)
        return false;
      this.startIndicies.AddRange((IEnumerable<int>) other.startIndicies);
      this.endIndicies.AddRange((IEnumerable<int>) other.endIndicies);
      return true;
    }
  }
}
