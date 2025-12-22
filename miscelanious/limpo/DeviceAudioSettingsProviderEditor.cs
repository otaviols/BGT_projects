using UnityEngine;

public class DeviceAudioSettingsProviderEditor : MonoBehaviour, IDeviceAudioSettingsProvider
{
  [SerializeField]
  private float m_volume = 1f;
  [SerializeField]
  private bool m_isMuted;

  public float Volume => this.m_volume;

  public bool IsMuted => this.m_isMuted;

  private void Awake() => this.gameObject.AddComponent<HSDontDestroyOnLoad>();
}
