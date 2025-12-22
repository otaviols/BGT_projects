using UnityEngine;

public class TargetDamageIndicator : MonoBehaviour
{
  public UberText m_indicatorText;
  public GameObject m_targetArrowBang;

  public void SetText(string newText) => this.m_indicatorText.Text = newText ?? string.Empty;

  public void Show(bool active)
  {
    this.m_indicatorText.gameObject.SetActive(active);
    this.m_targetArrowBang.SetActive(active);
  }
}
