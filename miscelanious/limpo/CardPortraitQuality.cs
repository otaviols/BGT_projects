using UnityEngine;

public class CardPortraitQuality
{
  public const int NOT_LOADED = 0;
  public const int LOW = 1;
  public const int MEDIUM = 2;
  public const int HIGH = 3;
  public int TextureQuality;
  public TAG_PREMIUM PremiumType;

  public CardPortraitQuality(int quality, TAG_PREMIUM premiumType)
  {
    this.TextureQuality = quality;
    this.PremiumType = premiumType;
  }

  public static CardPortraitQuality GetUnloaded() => new CardPortraitQuality(0, TAG_PREMIUM.NORMAL);

  public static CardPortraitQuality GetDefault() => new CardPortraitQuality(3, TAG_PREMIUM.SIGNATURE);

  public static CardPortraitQuality GetFromDef(CardDef def) => !((Object) def == (Object) null) ? def.GetPortraitQuality() : CardPortraitQuality.GetDefault();

  public override string ToString() => "(" + (object) this.TextureQuality + ", " + (object) this.PremiumType + ")";
}
