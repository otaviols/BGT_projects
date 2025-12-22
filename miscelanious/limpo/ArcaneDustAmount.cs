using UnityEngine;

public class ArcaneDustAmount : MonoBehaviour
{
  public UberText m_dustCount;
  public GameObject m_dustJar;
  public GameObject m_dustFX;
  public GameObject m_explodeFX_Common;
  public GameObject m_explodeFX_Rare;
  public GameObject m_explodeFX_Epic;
  public GameObject m_explodeFX_Legendary;
  private static ArcaneDustAmount s_instance;

  private void Awake() => ArcaneDustAmount.s_instance = this;

  private void Start() => this.UpdateCurrentDustAmount();

  public static ArcaneDustAmount Get() => ArcaneDustAmount.s_instance;

  public void UpdateCurrentDustAmount() => this.m_dustCount.Text = NetCache.Get().GetArcaneDustBalance().ToString();
}
