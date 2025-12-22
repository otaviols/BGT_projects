using UnityEngine;

public class LossMarks : MonoBehaviour
{
  public void Init(int numMarks)
  {
    for (int index = 0; index < this.transform.childCount; ++index)
    {
      this.transform.GetChild(index).gameObject.SetActive(numMarks > 0);
      --numMarks;
    }
  }

  public void SetNumMarked(int numMarked)
  {
    for (int index = 0; index < this.transform.childCount; ++index)
    {
      this.transform.GetChild(index).GetChild(0).gameObject.SetActive(numMarked > 0);
      --numMarked;
    }
  }
}
