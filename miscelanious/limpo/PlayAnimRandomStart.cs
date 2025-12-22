using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimRandomStart : MonoBehaviour
{
  public List<GameObject> m_Bubbles;
  public float minWait;
  public float maxWait = 10f;
  public float MinSpeed = 0.2f;
  public float MaxSpeed = 1.1f;
  public string animName = "Bubble1";

  private void Start() => this.StartCoroutine(this.PlayRandomBubbles());

  private IEnumerator PlayRandomBubbles()
  {
    while (true)
    {
      GameObject bubble;
      do
      {
        yield return (object) new WaitForSeconds(Random.Range(this.minWait, this.maxWait));
        bubble = this.m_Bubbles[Random.Range(0, this.m_Bubbles.Count)];
      }
      while ((Object) bubble == (Object) null);
      Animation component = bubble.GetComponent<Animation>();
      component.Play();
      component[this.animName].speed = Random.Range(this.MinSpeed, this.MaxSpeed);
    }
  }
}
