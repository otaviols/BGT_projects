using UnityEngine;

public class BattlegroundsFinisherShopWidget : MonoBehaviour
{
  public Transform m_finisherWidgetTransform;

  private void Start()
  {
    if ((double) this.transform.rotation.eulerAngles.y == 0.0)
      return;
    this.m_finisherWidgetTransform.localRotation = Quaternion.identity;
  }
}
