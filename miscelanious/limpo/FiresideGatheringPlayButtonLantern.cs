using UnityEngine;

public class FiresideGatheringPlayButtonLantern : MonoBehaviour
{
  public GameObject LitLantern;
  public GameObject UnlitLantern;

  public void SetLanternLit(bool lit)
  {
    this.LitLantern.SetActive(lit);
    this.UnlitLantern.SetActive(!lit);
  }
}
