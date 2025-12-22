using UnityEngine;

public class VS : MonoBehaviour
{
  public GameObject m_shadow;

  private void Start() => this.SetDefaults();

  private void OnDestroy() => this.SetDefaults();

  private void SetDefaults() => this.ActivateShadow(false);

  public void ActivateShadow(bool active = true) => this.m_shadow.SetActive(active);

  public void ActivateAnimation(bool active = true) => this.gameObject.GetComponentInChildren<Animation>().enabled = active;
}
