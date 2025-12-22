using UnityEngine;

public class PlayParticlesList : MonoBehaviour
{
  public GameObject[] m_objects;

  public void PlayParticle(int theIndex)
  {
    if (theIndex < 0 || theIndex > this.m_objects.Length)
      Debug.LogWarning((object) "The index is out of range");
    else if ((Object) this.m_objects[theIndex] == (Object) null)
      Debug.LogWarningFormat("{0} PlayParticlesList object is null", (object) this.gameObject.name);
    else
      this.m_objects[theIndex].GetComponent<ParticleSystem>().Play();
  }
}
