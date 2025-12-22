using UnityEngine;

public class ModularBundleDustJar : MonoBehaviour
{
  public ModularBundleText HeaderText;
  public UberText AmountText;

  public void KeepHeaderTextStraight() => this.HeaderText.transform.localRotation = Quaternion.Euler(90f, 360f - this.transform.parent.localRotation.eulerAngles.y, 0.0f);
}
