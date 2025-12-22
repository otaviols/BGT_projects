using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
[ExecuteAlways]
public abstract class NestedPrefabBase : MonoBehaviour
{
  private List<NestedPrefabBase.EditorMesh> m_EditorMeshes = new List<NestedPrefabBase.EditorMesh>();
  private string m_lastPrefab;
  private GameObject m_PrefabGameObject;

  public GameObject PrefabGameObject(bool instantiateIfNeeded = false)
  {
    if ((Object) this.m_PrefabGameObject == (Object) null & instantiateIfNeeded)
      this.UpdateMesh();
    return this.m_PrefabGameObject;
  }

  public bool PrefabIsLoaded() => (Object) this.m_PrefabGameObject != (Object) null;

  private void OnEnable()
  {
    if (!((Object) this.m_PrefabGameObject == (Object) null))
      return;
    this.UpdateMesh();
  }

  private void UpdateMesh()
  {
    this.LoadPrefab();
    this.m_EditorMeshes.Clear();
    if (!this.enabled || !((Object) this.m_PrefabGameObject != (Object) null))
      return;
    this.SetupEditorMesh(this.m_PrefabGameObject, Matrix4x4.identity);
  }

  private void SetupEditorMesh(GameObject go, Matrix4x4 goMtx)
  {
    if (!(bool) (Object) go)
      return;
    Vector3 pos = go.transform.position * -1f;
    Matrix4x4 matrix4x4 = goMtx * Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
    foreach (Renderer componentsInChild in go.GetComponentsInChildren(typeof (Renderer), true))
    {
      MeshFilter component = componentsInChild.GetComponent<MeshFilter>();
      if (!((Object) component == (Object) null))
      {
        List<Material> sharedMaterials = componentsInChild.GetSharedMaterials();
        if (sharedMaterials.Count != 0)
          this.m_EditorMeshes.Add(new NestedPrefabBase.EditorMesh()
          {
            mesh = component.sharedMesh,
            matrix = matrix4x4 * componentsInChild.transform.localToWorldMatrix,
            materials = new List<Material>((IEnumerable<Material>) sharedMaterials)
          });
      }
    }
    foreach (NestedPrefabBase componentsInChild in go.GetComponentsInChildren(typeof (NestedPrefabBase), true))
    {
      if (componentsInChild.enabled && componentsInChild.gameObject.activeSelf)
        this.SetupEditorMesh(componentsInChild.m_PrefabGameObject, matrix4x4 * componentsInChild.transform.localToWorldMatrix);
    }
  }

  protected abstract void LoadPrefab();

  protected void LoadPrefab(string prefabToLoad)
  {
    this.m_PrefabGameObject = this.LoadWithAssetLoader(prefabToLoad);
    Quaternion localRotation = this.m_PrefabGameObject.transform.localRotation;
    Vector3 localScale = this.m_PrefabGameObject.transform.localScale;
    this.m_PrefabGameObject.transform.parent = this.transform;
    this.m_PrefabGameObject.transform.localPosition = Vector3.zero;
    this.m_PrefabGameObject.transform.localRotation = localRotation;
    this.m_PrefabGameObject.transform.localScale = localScale;
    this.m_PrefabGameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
  }

  private GameObject LoadWithAssetLoader(string prefab) => AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit(prefab), (AssetLoadingOptions) 0);

  private struct EditorMesh
  {
    public Mesh mesh;
    public Matrix4x4 matrix;
    public List<Material> materials;
  }
}
