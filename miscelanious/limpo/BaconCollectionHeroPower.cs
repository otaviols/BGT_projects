using UnityEngine;

public class BaconCollectionHeroPower : MonoBehaviour
{
  public GameObject m_shadow;

  public void HideItemsForGhostView() => this.m_shadow?.SetActive(false);
}
