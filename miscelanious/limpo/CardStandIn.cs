using UnityEngine;

public class CardStandIn : MonoBehaviour
{
  public Card linkedCard;
  public Collider standInCollider;

  public void DisableStandIn() => this.standInCollider.enabled = false;

  public void EnableStandIn() => this.standInCollider.enabled = true;
}
