using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTransitions : MonoBehaviour
{
  public List<GameObject> m_TargetList;
  public float amount;
  private List<Renderer> rend;

  public void StartTransitions()
  {
    foreach (Renderer renderer in this.rend)
      renderer.GetMaterial().SetFloat("_Transistion", this.amount);
  }

  private void Start()
  {
    this.rend = new List<Renderer>();
    foreach (GameObject target in this.m_TargetList)
    {
      if (!((Object) target == (Object) null))
        this.rend.Add(target.GetComponent<Renderer>());
    }
  }

  private void Update() => this.StartTransitions();
}
