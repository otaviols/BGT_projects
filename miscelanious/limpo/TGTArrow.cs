using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class TGTArrow : MonoBehaviour
{
  public GameObject m_ArrowRoot;
  public GameObject m_ArrowMesh;
  public GameObject m_Trail;
  public ParticleSystem m_BullseyeParticles;

  private void onEnable() => this.m_ArrowRoot.transform.localEulerAngles = new Vector3(0.0f, 170f, 0.0f);

  public void FireArrow(bool randomRotation)
  {
    if (randomRotation)
    {
      Vector3 localEulerAngles = this.m_ArrowMesh.transform.localEulerAngles;
      this.m_ArrowMesh.transform.localEulerAngles = new Vector3(localEulerAngles.x + Random.Range(0.0f, 360f), localEulerAngles.y, localEulerAngles.z);
      this.m_ArrowRoot.transform.localEulerAngles = new Vector3(Random.Range(0.0f, 20f), Random.Range(160f, 180f), 0.0f);
    }
    this.ArrowAnimation();
  }

  public void ArrowAnimation()
  {
    this.m_Trail.SetActive(true);
    this.m_Trail.GetComponent<Renderer>().GetMaterial().SetColor("_Color", new Color(0.15f, 0.15f, 0.15f, 0.15f));
    iTween.ColorTo(this.m_Trail, iTween.Hash((object) "color", (object) Color.clear, (object) "time", (object) 0.1f, (object) "oncomplete", (object) "OnAnimationComplete"));
    Vector3 localPosition = this.m_ArrowRoot.transform.localPosition;
    iTween.MoveFrom(this.m_ArrowRoot, iTween.Hash((object) "position", (object) new Vector3(localPosition.x, localPosition.y, localPosition.z + 0.4f), (object) "islocal", (object) true, (object) "time", (object) 0.05f, (object) "easetype", (object) iTween.EaseType.easeOutQuart));
  }

  public void OnAnimationComplete() => this.m_Trail.SetActive(false);

  public void Bullseye() => this.m_BullseyeParticles.Play();
}
