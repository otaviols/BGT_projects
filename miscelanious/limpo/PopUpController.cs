using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
  public float LifeTime = 4f;
  public GameObject Portrait;
  public GameObject Banner;

  private void Update()
  {
    if ((double) this.LifeTime < 0.0)
      return;
    this.LifeTime -= Time.deltaTime;
    if ((double) this.LifeTime > 0.0)
      return;
    Object.Destroy((Object) this.gameObject);
  }

  public void Populate(int currentValue, int totalValue, int cardID, TAG_PREMIUM premium)
  {
    DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(cardID);
    if (cardDef == null)
      return;
    Texture portraitTexture = cardDef.CardDef.GetPortraitTexture(premium);
    if ((Object) portraitTexture != (Object) null)
      this.Portrait.GetComponent<Renderer>().GetMaterial().SetTexture("_MainTex", portraitTexture);
    this.Banner.GetComponent<RewardBanner>().SetText(currentValue.ToString() + " out of " + (object) totalValue, "Entity: " + DefLoader.Get().GetEntityDef(cardID).GetName(), "");
  }
}
