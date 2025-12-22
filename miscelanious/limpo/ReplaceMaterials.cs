using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[CustomEditClass]
public class ReplaceMaterials : MonoBehaviour
{
  public List<ReplaceMaterials.MaterialData> m_Materials;

  private void Start()
  {
    foreach (ReplaceMaterials.MaterialData material in this.m_Materials)
    {
      if (!((UnityEngine.Object) material.NewMaterial == (UnityEngine.Object) null))
      {
        GameObject gameObject = this.FindGameObject(material.GameObjectName);
        if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null && !material.ReplaceChildMaterials)
          Log.Graphics.Print("ReplaceMaterials failed to locate object: {0}", (object) material.GameObjectName);
        else if (material.ReplaceChildMaterials)
        {
          foreach (Renderer componentsInChild in gameObject.GetComponentsInChildren<Renderer>())
          {
            if (!((UnityEngine.Object) componentsInChild == (UnityEngine.Object) null))
              componentsInChild.SetMaterial(material.MaterialIndex, material.NewMaterial);
          }
        }
        else
        {
          Renderer component = gameObject.GetComponent<Renderer>();
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
            Log.Graphics.Print("ReplaceMaterials failed to get Renderer: {0}", (object) material.GameObjectName);
          else
            component.SetMaterial(material.MaterialIndex, material.NewMaterial);
        }
      }
    }
  }

  private GameObject FindGameObject(string gameObjName)
  {
    if (gameObjName[0] != '/')
      return GameObject.Find(gameObjName);
    string[] strArray = gameObjName.Split('/');
    return GameObject.Find(strArray[strArray.Length - 1]);
  }

  [Serializable]
  public class MaterialData
  {
    [CustomEditField(T = EditType.SCENE_OBJECT)]
    public string GameObjectName;
    public int MaterialIndex;
    public Material NewMaterial;
    public bool ReplaceChildMaterials;
    public GameObject DisplayGameObject;
  }
}
