using UnityEngine;

public class BaconHeroMulliganBestPlaceVisual : MonoBehaviour
{
  public UberText BestPlaceText;
  public Color FirstPlaceColorOverride;
  private PlayMakerFSM m_playmaker;
  private const int MaxSupportedPlace = 4;

  public void SetVisualActive(int place, int heroDbId)
  {
    if (place < 0 || place > 4)
    {
      this.BestPlaceText.gameObject.SetActive(false);
    }
    else
    {
      this.BestPlaceText.Text = GameStrings.Format("GAMEPLAY_MULLIGAN_BEST_PLACE", (object) GameStrings.GetOrdinalNumber(place));
      if (place == 1)
        this.BestPlaceText.TextColor = this.FirstPlaceColorOverride;
      PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
      if (!((Object) component != (Object) null))
        return;
      component.SendEvent("Birth");
    }
  }

  public void Hide()
  {
    PlayMakerFSM component = this.GetComponent<PlayMakerFSM>();
    if ((Object) component != (Object) null)
      component.SendEvent("Death");
    Object.Destroy((Object) this.gameObject, 10f);
  }
}
