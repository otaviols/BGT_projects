using System.Collections;
using UnityEngine;

public class GameplayErrorCloud : MonoBehaviour
{
  public UberText m_errorText;
  public float initTime;
  public ParticleSystem m_psystem;
  public Vector3_MobileOverride m_psystemLocalPositionInCollectionManager;
  public Vector3_MobileOverride m_psystemLocalPositionInGame;
  private const float ERROR_MESSAGE_DURATION = 2f;
  private const float ERROR_MESSAGE_FADEIN = 0.15f;
  private const float ERROR_MESSAGE_FADEOUT = 0.5f;
  private readonly string START_COROUTINE_NAME = "StartHideMessageDelay";
  private float m_holdDuration;
  private Coroutine m_coroutine;

  private void Start()
  {
    RenderUtils.SetAlpha(this.gameObject, 0.0f);
    this.Hide();
  }

  public void Show()
  {
    this.m_errorText.gameObject.SetActive(true);
    this.m_psystem.gameObject.SetActive(true);
    this.SetParticleEmitterLocalPosition();
  }

  public void Hide()
  {
    iTween.Stop(this.gameObject);
    this.StopCoroutine(this.START_COROUTINE_NAME);
    this.m_coroutine = (Coroutine) null;
    this.m_errorText.gameObject.SetActive(false);
    this.m_psystem.gameObject.SetActive(false);
  }

  public void ShowMessage(string message, float timeToDisplay)
  {
    if (this.m_coroutine != null)
      this.Hide();
    iTween.Stop(this.gameObject);
    this.m_holdDuration = Mathf.Max(2f, timeToDisplay);
    this.m_psystem.main.startLifetime = (ParticleSystem.MinMaxCurve) (float) (0.150000005960464 + (double) this.m_holdDuration * 1.39999997615814 + 0.5);
    this.Show();
    this.m_errorText.Text = message;
    iTween.FadeTo(this.gameObject, iTween.Hash((object) "alpha", (object) 1f, (object) "time", (object) 0.15f));
    this.m_coroutine = this.StartCoroutine(this.START_COROUTINE_NAME);
  }

  public void HideMessage() => iTween.FadeTo(this.gameObject, iTween.Hash((object) "alpha", (object) 0.0f, (object) "time", (object) 0.5f, (object) "oncomplete", (object) "Hide"));

  public IEnumerator StartHideMessageDelay()
  {
    yield return (object) new WaitForSeconds(0.15f + this.m_holdDuration);
    this.HideMessage();
  }

  private void SetParticleEmitterLocalPosition()
  {
    if (CollectionManager.Get().IsInEditMode())
      this.m_psystem.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_psystemLocalPositionInCollectionManager;
    else
      this.m_psystem.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_psystemLocalPositionInGame;
  }
}
