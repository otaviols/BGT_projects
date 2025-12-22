using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Renderer))]
public class SetRenderQue : MonoBehaviour
{
  public int queue = 1;
  public bool includeChildren;
  public int[] queues;
  private Renderer m_Renderer;

  private void Awake() => this.m_Renderer = this.GetComponent<Renderer>();

  private void Start()
  {
    if (this.includeChildren)
    {
      foreach (Renderer componentsInChild in this.GetComponentsInChildren<Renderer>())
      {
        if (!((Object) componentsInChild == (Object) null))
          componentsInChild.sortingOrder += this.queue;
      }
    }
    else
    {
      if ((Object) this.m_Renderer == (Object) null)
        return;
      this.m_Renderer.sortingOrder += this.queue;
    }
    this.Run();
  }

  public void Run()
  {
    if (this.queues == null || (Object) this.m_Renderer == (Object) null)
      return;
    List<Material> sharedMaterials = this.m_Renderer.GetSharedMaterials();
    if (sharedMaterials == null)
      return;
    int count = sharedMaterials.Count;
    for (int materialIndex = 0; materialIndex < this.queues.Length && materialIndex < count; ++materialIndex)
    {
      int queue = this.queues[materialIndex];
      if (queue != 0 && queue != this.queue)
      {
        Material sharedMaterial = this.m_Renderer.GetSharedMaterial(materialIndex);
        if (!((Object) sharedMaterial == (Object) null))
        {
          if (queue < 0)
            Debug.LogWarning((object) string.Format("WARNING: Using negative renderQueue for {0}'s {1} (renderQueue = {2})", (object) this.transform.root.name, (object) this.gameObject.name, (object) this.queues[materialIndex]));
          Material material = new Material(sharedMaterial);
          material.renderQueue += queue;
          this.m_Renderer.SetMaterial(materialIndex, material);
        }
      }
    }
  }
}
