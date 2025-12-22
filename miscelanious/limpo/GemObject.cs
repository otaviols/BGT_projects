using UnityEngine;

public class GemObject : MonoBehaviour
{
  public Vector3 startingScale;
  public float jiggleAmount;
  public GameObject m_gemMesh;
  public UberText m_gemText;
  private bool initialized;
  private bool m_hiddenFlag;

  private void Awake() => this.startingScale = this.transform.localScale;

  public void Enlarge(float scaleFactor)
  {
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(this.startingScale.x * scaleFactor, this.startingScale.y * scaleFactor, this.startingScale.z * scaleFactor), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
  }

  public void Shrink() => iTween.ScaleTo(this.gameObject, this.startingScale, 0.5f);

  public void ImmediatelyScaleToZero() => this.transform.localScale = Vector3.zero;

  public void ScaleToZero()
  {
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, Vector3.zero, 0.5f);
  }

  public void SetToZeroThenEnlarge()
  {
    this.transform.localScale = Vector3.zero;
    this.Enlarge(1f);
  }

  public void Hide() => this.gameObject.SetActive(false);

  public void Show() => this.gameObject.SetActive(true);

  public void Initialize() => this.initialized = true;

  public void SetHideNumberFlag(bool enable) => this.m_hiddenFlag = enable;

  public bool IsNumberHidden() => this.m_hiddenFlag;

  public void Jiggle()
  {
    if (!this.initialized)
    {
      this.initialized = true;
    }
    else
    {
      iTween.Stop(this.gameObject);
      this.transform.localScale = this.startingScale;
      iTween.PunchScale(this.gameObject, new Vector3(this.jiggleAmount, this.jiggleAmount, this.jiggleAmount), 1f);
    }
  }

  public void SetMeshActive(bool active)
  {
    if ((Object) this.m_gemMesh == (Object) null)
      return;
    this.m_gemMesh.SetActive(active);
  }

  public void SetTextActive(bool active)
  {
    if ((Object) this.m_gemText == (Object) null)
      return;
    this.m_gemText.gameObject.SetActive(active);
  }
}
