using UnityEngine;

public class Secret : MonoBehaviour
{
  public UberText secretLabelTop;
  public UberText secretLabelMiddle;
  public UberText secretLabelBottom;

  private void Start()
  {
    this.secretLabelTop.SetText(GameStrings.Get("GAMEPLAY_SECRET_BANNER_TITLE"));
    this.secretLabelMiddle.SetText(GameStrings.Get("GAMEPLAY_SECRET_BANNER_TITLE"));
    this.secretLabelBottom.SetText(GameStrings.Get("GAMEPLAY_SECRET_BANNER_TITLE"));
  }
}
