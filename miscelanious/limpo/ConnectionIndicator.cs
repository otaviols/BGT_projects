using UnityEngine;

public class ConnectionIndicator : MonoBehaviour
{
  public GameObject m_indicator;
  private static ConnectionIndicator s_instance;
  private bool m_active;
  private const float LATENCY_TOLERANCE = 3f;

  private void Awake()
  {
    ConnectionIndicator.s_instance = this;
    this.m_active = false;
    this.m_indicator.SetActive(false);
  }

  private void OnDestroy() => ConnectionIndicator.s_instance = (ConnectionIndicator) null;

  public static ConnectionIndicator Get() => ConnectionIndicator.s_instance;

  private void SetIndicator(bool val)
  {
    if (val == this.m_active)
      return;
    this.m_active = val;
    this.m_indicator.SetActive(val);
    BnetBar.Get().UpdateLayout();
  }

  public bool IsVisible() => this.m_active;

  private void Update() => this.SetIndicator(Network.Get().TimeSinceLastPong() > 3.0);
}
