using PegasusShared;
using UnityEngine;

public class CardInfoPane : MonoBehaviour
{
  public UberText m_artistName;
  public UberText m_rarityLabel;
  public UberText m_flavorText;
  public UberText m_setName;
  public GameObject m_standardTheming;
  public RarityGem m_rarityGem;
  public GameObject m_wildTheming;
  public RarityGem m_wildRarityGem;
  public GameObject m_classicTheming;
  public RarityGem m_classicRarityGem;

  public void UpdateContent()
  {
    EntityDef entityDef;
    TAG_PREMIUM premium;
    if (!CraftingManager.Get().GetShownCardInfo(out entityDef, out premium))
      return;
    TAG_RARITY rarity = entityDef.GetRarity();
    TAG_CARD_SET tagCardSet = entityDef.GetCardSet();
    if (GameUtils.IsLegacySet(tagCardSet))
      tagCardSet = TAG_CARD_SET.LEGACY;
    this.m_rarityLabel.Text = rarity != TAG_RARITY.FREE ? GameStrings.GetRarityText(rarity) : "";
    this.AssignRarityColors(rarity, tagCardSet);
    FormatType cardSetFormat = GameUtils.GetCardSetFormat(tagCardSet);
    this.m_wildTheming.SetActive(cardSetFormat == FormatType.FT_WILD);
    this.m_standardTheming.SetActive(cardSetFormat == FormatType.FT_STANDARD);
    this.m_classicTheming.SetActive(cardSetFormat == FormatType.FT_CLASSIC);
    switch (cardSetFormat)
    {
      case FormatType.FT_WILD:
        this.m_wildRarityGem.SetRarityGem(rarity, tagCardSet);
        break;
      case FormatType.FT_STANDARD:
        this.m_rarityGem.SetRarityGem(rarity, tagCardSet);
        break;
      case FormatType.FT_CLASSIC:
        this.m_classicRarityGem.SetRarityGem(rarity, tagCardSet);
        break;
    }
    this.m_setName.Text = GameStrings.GetCardSetName(tagCardSet);
    this.m_artistName.Text = GameStrings.Format("GLUE_COLLECTION_ARTIST", (object) entityDef.GetArtistName(premium));
    this.m_wildTheming.SetActive(cardSetFormat == FormatType.FT_WILD);
    string str = "<color=#000000ff>" + entityDef.GetFlavorText() + "</color>";
    NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(entityDef.GetCardId(), premium);
    if (cardValue != null && cardValue.IsOverrideActive())
    {
      if (!string.IsNullOrEmpty(str))
        str += "\n\n";
      str += GameStrings.Get("GLUE_COLLECTION_RECENTLY_NERFED");
    }
    this.m_flavorText.Text = str;
  }

  private void AssignRarityColors(TAG_RARITY rarity, TAG_CARD_SET cardSet)
  {
    switch (rarity)
    {
      case TAG_RARITY.RARE:
        this.m_rarityLabel.TextColor = new Color(0.11f, 0.33f, 0.8f, 1f);
        break;
      case TAG_RARITY.EPIC:
        this.m_rarityLabel.TextColor = new Color(0.77f, 0.03f, 1f, 1f);
        break;
      case TAG_RARITY.LEGENDARY:
        this.m_rarityLabel.TextColor = new Color(1f, 0.56f, 0.0f, 1f);
        break;
      default:
        this.m_rarityLabel.TextColor = Color.white;
        break;
    }
  }
}
