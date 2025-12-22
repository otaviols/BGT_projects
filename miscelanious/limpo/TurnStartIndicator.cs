using UnityEngine;

public class TurnStartIndicator : MonoBehaviour
{
  public GameObject m_explosionFX;
  public GameObject m_godRays;
  public UberText m_labelTop;
  public UberText m_labelMiddle;
  public UberText m_labelBottom;
  public UberText m_reminderText;
  public float m_desiredDisplayDuration = 1.5f;
  public float m_desiredDelayDuration = 1f;
  private const float DISAPPEAR_SCALE_VAL = 0.01f;
  private const float START_SCALE_VAL = 1f;
  private const float AFTER_PUNCH_SCALE_VAL = 9.8f;
  private const float END_SCALE_VAL = 10f;

  private void Start()
  {
    iTween.FadeTo(this.gameObject, 0.0f, 0.0f);
    this.gameObject.transform.position = new Vector3(-7.8f, 8.2f, -5f);
    this.gameObject.transform.eulerAngles = new Vector3(90f, 0.0f, 0.0f);
    this.gameObject.SetActive(false);
    this.SetReminderText("");
  }

  public bool IsShown() => this.gameObject.activeSelf;

  public float GetDesiredDelayDuration() => this.m_desiredDelayDuration;

  public virtual void Show()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.gameObject.transform.position = new Vector3(-7.8f, 8.2f, -4.2f);
    else
      this.gameObject.transform.position = new Vector3(-7.8f, 8.2f, -5f);
    this.gameObject.SetActive(true);
    if ((Object) this.m_labelTop != (Object) null)
      this.m_labelTop.Text = GameStrings.Get("GAMEPLAY_YOUR_TURN");
    if ((Object) this.m_labelMiddle != (Object) null)
      this.m_labelMiddle.Text = GameStrings.Get("GAMEPLAY_YOUR_TURN");
    if ((Object) this.m_labelBottom != (Object) null)
      this.m_labelBottom.Text = GameStrings.Get("GAMEPLAY_YOUR_TURN");
    iTween.FadeTo(this.gameObject, 1f, 0.25f);
    this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(10f, 10f, 10f), (object) "time", (object) 0.25f, (object) "oncomplete", (object) "PunchTurnStartInstance", (object) "oncompletetarget", (object) this.gameObject));
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.gameObject.transform.position + new Vector3(0.02f, 0.02f, 0.02f)), (object) "time", (object) this.m_desiredDisplayDuration, (object) "oncomplete", (object) "HideTurnStartInstance", (object) "oncompletetarget", (object) this.gameObject));
    this.m_explosionFX.GetComponent<ParticleSystem>().Play();
  }

  private void PunchTurnStartInstance() => iTween.ScaleTo(this.gameObject, new Vector3(9.8f, 9.8f, 9.8f), 0.15f);

  private void HideTurnStartInstance()
  {
    iTween.FadeTo(this.gameObject, 0.0f, 0.25f);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) 0.25f, (object) "oncomplete", (object) "DeactivateTurnStartInstance", (object) "oncompletetarget", (object) this.gameObject));
  }

  private void DeactivateTurnStartInstance() => this.gameObject.SetActive(false);

  public void SetReminderText(string newText)
  {
    if (!((Object) this.m_reminderText != (Object) null))
      return;
    this.m_reminderText.Text = newText;
  }
}
