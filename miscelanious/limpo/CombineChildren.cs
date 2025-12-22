using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour
{
  public bool generateTriangleStrips = true;

  private void Start()
  {
    Component[] componentsInChildren = this.GetComponentsInChildren(typeof (MeshFilter));
    Matrix4x4 worldToLocalMatrix = this.transform.worldToLocalMatrix;
    Hashtable hashtable = new Hashtable();
    for (int index1 = 0; index1 < componentsInChildren.Length; ++index1)
    {
      MeshFilter meshFilter = (MeshFilter) componentsInChildren[index1];
      Renderer component = componentsInChildren[index1].GetComponent<Renderer>();
      MeshCombineUtility.MeshInstance meshInstance = new MeshCombineUtility.MeshInstance();
      meshInstance.mesh = meshFilter.sharedMesh;
      if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.enabled && (UnityEngine.Object) meshInstance.mesh != (UnityEngine.Object) null)
      {
        meshInstance.transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
        List<Material> sharedMaterials = component.GetSharedMaterials();
        for (int index2 = 0; index2 < sharedMaterials.Count; ++index2)
        {
          meshInstance.subMeshIndex = Math.Min(index2, meshInstance.mesh.subMeshCount - 1);
          ArrayList arrayList = (ArrayList) hashtable[(object) sharedMaterials[index2]];
          if (arrayList != null)
            arrayList.Add((object) meshInstance);
          else
            hashtable.Add((object) sharedMaterials[index2], (object) new ArrayList()
            {
              (object) meshInstance
            });
        }
        component.enabled = false;
      }
    }
    foreach (DictionaryEntry dictionaryEntry in hashtable)
    {
      MeshCombineUtility.MeshInstance[] array = (MeshCombineUtility.MeshInstance[]) ((ArrayList) dictionaryEntry.Value).ToArray(typeof (MeshCombineUtility.MeshInstance));
      if (hashtable.Count == 1)
      {
        if ((UnityEngine.Object) this.GetComponent(typeof (MeshFilter)) == (UnityEngine.Object) null)
          this.gameObject.AddComponent(typeof (MeshFilter));
        if (!(bool) (UnityEngine.Object) this.GetComponent("MeshRenderer"))
          this.gameObject.AddComponent<MeshRenderer>();
        ((MeshFilter) this.GetComponent(typeof (MeshFilter))).mesh = MeshCombineUtility.Combine(array, this.generateTriangleStrips);
        Renderer component = this.GetComponent<Renderer>();
        component.SetMaterial((Material) dictionaryEntry.Key);
        component.enabled = true;
      }
      else
      {
        GameObject gameObject = new GameObject("Combined mesh");
        gameObject.transform.parent = this.transform;
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.AddComponent(typeof (MeshFilter));
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<Renderer>().SetMaterial((Material) dictionaryEntry.Key);
        ((MeshFilter) gameObject.GetComponent(typeof (MeshFilter))).mesh = MeshCombineUtility.Combine(array, this.generateTriangleStrips);
      }
    }
  }
}
