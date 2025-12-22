using UnityEngine;

public class PegUIContainer : MonoBehaviour
{
  public bool isActive = true;

  public void SetActive(bool a)
  {
    if (a == this.gameObject.activeSelf)
      return;
    this.gameObject.SetActive(a);
  }
}
