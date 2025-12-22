using System.Collections;
using UnityEngine;

public class RankChangeStar : MonoBehaviour
{
  public MeshRenderer m_starMeshRenderer;
  public MeshRenderer m_bottomGlowRenderer;
  public MeshRenderer m_topGlowRenderer;

  public void BlackOut() => this.m_starMeshRenderer.enabled = false;

  public void UnBlackOut() => this.m_starMeshRenderer.enabled = true;

  public void FadeIn() => this.GetComponent<PlayMakerFSM>().SendEvent(nameof (FadeIn));

  public void Spawn() => this.GetComponent<PlayMakerFSM>().SendEvent(nameof (Spawn));

  public void Reset() => this.GetComponent<PlayMakerFSM>().SendEvent(nameof (Reset));

  public void Blink(float delay) => this.StartCoroutine(this.DelayedBlink(delay));

  public IEnumerator DelayedBlink(float delay)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    RankChangeStar rankChangeStar = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      rankChangeStar.GetComponent<PlayMakerFSM>().SendEvent("Blink");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void Burst(float delay) => this.StartCoroutine(this.DelayedBurst(delay));

  public IEnumerator DelayedBurst(float delay)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    RankChangeStar rankChangeStar = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      rankChangeStar.UnBlackOut();
      rankChangeStar.GetComponent<PlayMakerFSM>().SendEvent("Burst");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator DelayedDespawn(float delay)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    RankChangeStar rankChangeStar = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      rankChangeStar.GetComponent<PlayMakerFSM>().SendEvent("DeSpawn");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void Despawn() => this.GetComponent<PlayMakerFSM>().SendEvent("DeSpawn");

  public void Wipe(float delay) => this.StartCoroutine(this.DelayedWipe(delay));

  public IEnumerator DelayedWipe(float delay)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    RankChangeStar rankChangeStar = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      rankChangeStar.GetComponent<PlayMakerFSM>().SendEvent("Wipe");
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
