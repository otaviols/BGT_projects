using UnityEngine;

[CustomEditClass]
public class DustJarPanel : MonoBehaviour
{
  [CustomEditField(Sections = "Dust Panel")]
  public GameObject m_dustJar;
  [CustomEditField(Sections = "Dust Panel")]
  public UberText m_dustCount;
  [CustomEditField(Sections = "Dust Panel")]
  public AudioSource m_dustJarEntranceSound;

  public void Show(int dustAmount)
  {
    this.m_dustCount.Text = dustAmount.ToString();
    Vector3 localScale = this.m_dustJar.transform.localScale;
    this.m_dustJar.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    iTween.ScaleTo(this.m_dustJar.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
    if (!((Object) this.m_dustJarEntranceSound != (Object) null))
      return;
    SoundManager.Get().Play(Object.Instantiate<AudioSource>(this.m_dustJarEntranceSound, this.transform));
  }
}
