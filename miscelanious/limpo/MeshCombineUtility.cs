using UnityEngine;

public class MeshCombineUtility
{
  public static Mesh Combine(MeshCombineUtility.MeshInstance[] combines, bool generateStrips)
  {
    int length1 = 0;
    int length2 = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
        length1 += combine.mesh.vertexCount;
    }
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
        length2 += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
    }
    Vector3[] dst1 = new Vector3[length1];
    Vector3[] dst2 = new Vector3[length1];
    Vector4[] dst3 = new Vector4[length1];
    Vector2[] dst4 = new Vector2[length1];
    Vector2[] dst5 = new Vector2[length1];
    Color[] dst6 = new Color[length1];
    int[] numArray = new int[length2];
    int offset = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
        MeshCombineUtility.Copy(combine.mesh.vertexCount, combine.mesh.vertices, dst1, ref offset, combine.transform);
    }
    offset = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
      {
        Matrix4x4 transform = combine.transform;
        transform = transform.inverse.transpose;
        MeshCombineUtility.CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, dst2, ref offset, transform);
      }
    }
    offset = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
      {
        Matrix4x4 transform = combine.transform;
        transform = transform.inverse.transpose;
        MeshCombineUtility.CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, dst3, ref offset, transform);
      }
    }
    offset = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
        MeshCombineUtility.Copy(combine.mesh.vertexCount, combine.mesh.uv, dst4, ref offset);
    }
    offset = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
        MeshCombineUtility.Copy(combine.mesh.vertexCount, combine.mesh.uv2, dst5, ref offset);
    }
    offset = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
        MeshCombineUtility.CopyColors(combine.mesh.vertexCount, combine.mesh.colors, dst6, ref offset);
    }
    int num1 = 0;
    int num2 = 0;
    foreach (MeshCombineUtility.MeshInstance combine in combines)
    {
      if ((bool) (Object) combine.mesh)
      {
        int[] triangles = combine.mesh.GetTriangles(combine.subMeshIndex);
        for (int index = 0; index < triangles.Length; ++index)
          numArray[index + num1] = triangles[index] + num2;
        num1 += triangles.Length;
        num2 += combine.mesh.vertexCount;
      }
    }
    Mesh mesh = new Mesh();
    mesh.name = "Combined Mesh";
    mesh.vertices = dst1;
    mesh.normals = dst2;
    mesh.colors = dst6;
    mesh.uv = dst4;
    mesh.uv2 = dst5;
    mesh.tangents = dst3;
    mesh.triangles = numArray;
    return mesh;
  }

  private static void Copy(
    int vertexcount,
    Vector3[] src,
    Vector3[] dst,
    ref int offset,
    Matrix4x4 transform)
  {
    for (int index = 0; index < src.Length; ++index)
      dst[index + offset] = transform.MultiplyPoint(src[index]);
    offset += vertexcount;
  }

  private static void CopyNormal(
    int vertexcount,
    Vector3[] src,
    Vector3[] dst,
    ref int offset,
    Matrix4x4 transform)
  {
    for (int index = 0; index < src.Length; ++index)
      dst[index + offset] = transform.MultiplyVector(src[index]).normalized;
    offset += vertexcount;
  }

  private static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
  {
    for (int index = 0; index < src.Length; ++index)
      dst[index + offset] = src[index];
    offset += vertexcount;
  }

  private static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
  {
    for (int index = 0; index < src.Length; ++index)
      dst[index + offset] = src[index];
    offset += vertexcount;
  }

  private static void CopyTangents(
    int vertexcount,
    Vector4[] src,
    Vector4[] dst,
    ref int offset,
    Matrix4x4 transform)
  {
    for (int index = 0; index < src.Length; ++index)
    {
      Vector4 vector4 = src[index];
      Vector3 vector = new Vector3(vector4.x, vector4.y, vector4.z);
      vector = transform.MultiplyVector(vector).normalized;
      dst[index + offset] = new Vector4(vector.x, vector.y, vector.z, vector4.w);
    }
    offset += vertexcount;
  }

  public struct MeshInstance
  {
    public Mesh mesh;
    public int subMeshIndex;
    public Matrix4x4 transform;
  }
}
