using UnityEngine;

public class MarkOfEvilCounter : MonoBehaviour
{
  public SpriteRenderer[] m_MarkOfEvilIcons;
  public Sprite m_FullMarkOfEvilSprite;
  public Sprite m_EmptyMarkOfEvilSprite;

  private void Awake() => this.OnMarksChanged(0);

  public void OnMarksChanged(int numMarks)
  {
    if (numMarks <= 0)
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      if (numMarks > this.m_MarkOfEvilIcons.Length)
        Log.Gameplay.PrintWarning("{0}.OnMarksChanged() : num marks is greater than the number of icons!");
      for (int index = 0; index < this.m_MarkOfEvilIcons.Length; ++index)
        this.m_MarkOfEvilIcons[index].sprite = index < numMarks ? this.m_FullMarkOfEvilSprite : this.m_EmptyMarkOfEvilSprite;
      this.gameObject.SetActive(true);
    }
  }
}
