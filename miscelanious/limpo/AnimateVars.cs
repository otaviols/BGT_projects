using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class AnimateVars : MonoBehaviour
{
  public List<GameObject> m_objects;
  public float amount;
  public string varName;
  private List<Renderer> m_renderers;

  public void AnimateValue()
  {
    foreach (Renderer renderer in this.m_renderers)
    {
      if ((Object) renderer != (Object) null)
        renderer.GetMaterial().SetFloat(this.varName, this.amount);
    }
  }

  private void Start()
  {
    this.m_renderers = new List<Renderer>();
    foreach (GameObject gameObject in this.m_objects)
    {
      if (!((Object) gameObject == (Object) null))
        this.m_renderers.Add(gameObject.GetComponent<Renderer>());
    }
  }

  private void Update() => this.AnimateValue();
}
