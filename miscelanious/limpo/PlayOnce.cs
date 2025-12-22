using UnityEngine;

public class PlayOnce : MonoBehaviour
{
  public string notes;
  public string notes2;
  public GameObject tester;
  public string testerAnim;
  public GameObject tester2;
  public string tester2Anim;
  public GameObject tester3;
  public string tester3Anim;

  private void Start()
  {
    if ((Object) this.tester != (Object) null)
      this.tester.SetActive(false);
    if ((Object) this.tester2 != (Object) null)
      this.tester2.SetActive(false);
    if (!((Object) this.tester3 != (Object) null))
      return;
    this.tester3.SetActive(false);
  }

  private void OnGUI()
  {
    if (!UnityEngine.Event.current.isKey)
      return;
    if ((Object) this.tester != (Object) null)
    {
      this.tester.SetActive(true);
      Animation component = this.tester.GetComponent<Animation>();
      component.Stop(this.testerAnim);
      component.Play(this.testerAnim);
    }
    else
      Debug.Log((object) "NO 'tester' object.");
    if ((Object) this.tester2 != (Object) null)
    {
      this.tester2.SetActive(true);
      Animation component = this.tester2.GetComponent<Animation>();
      component.Stop(this.tester2Anim);
      component.Play(this.tester2Anim);
    }
    else
      Debug.Log((object) "NO 'tester2' object.");
    if ((Object) this.tester3 != (Object) null)
    {
      this.tester3.SetActive(true);
      Animation component = this.tester3.GetComponent<Animation>();
      component.Stop(this.tester3Anim);
      component.Play(this.tester3Anim);
    }
    else
      Debug.Log((object) "NO 'tester3' object.");
  }
}
