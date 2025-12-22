using UnityEngine;

public class Tooltip : MonoBehaviour
{
  public TextMesh headlineText;
  public TextMesh descriptionText;

  public void UpdateText(string headline, string description)
  {
    this.headlineText.text = headline;
    this.descriptionText.text = description;
  }
}
