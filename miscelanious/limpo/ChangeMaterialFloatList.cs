using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialFloatList : MonoBehaviour
{
  public List<GameObject> m_TargetList;
  public string m_propertyName;
  public float m_propertyValue;
  private List<Renderer> rend;
  private int m_materialProperty;
  private Material m_mat;

  private void Start()
  {
    this.rend = new List<Renderer>();
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        this.rend.Add(target.GetComponent<Renderer>());
    }
  }

  private void Update()
  {
    foreach (Renderer renderer in this.rend)
      renderer.GetMaterial().SetFloat(this.m_propertyName, this.m_propertyValue);
  }
}
