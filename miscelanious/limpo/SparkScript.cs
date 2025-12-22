using UnityEngine;

public class SparkScript : MonoBehaviour
{
  public AudioClip clip1;
  public AudioClip clip2;

  private void Awake()
  {
    AudioSource component = this.GetComponent<AudioSource>();
    if ((double) Random.value >= 0.5)
      component.clip = this.clip1;
    else
      component.clip = this.clip2;
  }
}
