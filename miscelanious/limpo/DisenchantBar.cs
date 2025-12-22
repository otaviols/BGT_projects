using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

[Serializable]
public class DisenchantBar
{
  public TAG_PREMIUM m_premiumType;
  public TAG_RARITY m_rarity;
  public UberText m_typeText;
  public UberText m_amountText;
  public UberText m_numCardsText;
  public GameObject m_amountBar;
  public GameObject m_dustJar;
  public GameObject m_rarityGem;
  public GameObject m_glow;
  public UberText m_numGoldText;
  public MeshRenderer m_barFrameMesh;
  private int m_numCards;
  private int m_amount;
  private int m_numGoldCards;
  public const float NUM_CARDS_TEXT_CENTER_X = 2.902672f;

  public void Reset()
  {
    this.m_numCards = 0;
    this.m_amount = 0;
    this.m_numGoldCards = 0;
    this.SetPercent(0.0f);
  }

  public void AddCards(int count, int sellAmount, TAG_PREMIUM premiumType)
  {
    this.m_numCards += count;
    if (this.m_premiumType != premiumType)
      this.m_numGoldCards += count;
    this.m_amount += sellAmount;
  }

  public void Init()
  {
    if (!((UnityEngine.Object) this.m_typeText != (UnityEngine.Object) null))
      return;
    string rarityText = GameStrings.GetRarityText(this.m_rarity);
    UberText typeText = this.m_typeText;
    string str;
    if (TAG_PREMIUM.GOLDEN != this.m_premiumType)
      str = rarityText;
    else
      str = GameStrings.Format("GLUE_MASS_DISENCHANT_PREMIUM_TITLE", (object) rarityText);
    typeText.Text = str;
  }

  public int GetNumCards() => this.m_numCards;

  public int GetAmountDust() => this.m_amount;

  public void UpdateVisuals(int totalNumCards)
  {
    this.m_numCardsText.Text = this.m_numCards.ToString();
    this.m_amountText.Text = this.m_amount.ToString();
    if ((UnityEngine.Object) this.m_numGoldText != (UnityEngine.Object) null)
    {
      MeshFilter component = this.m_barFrameMesh.GetComponent<MeshFilter>();
      if (this.m_numGoldCards > 0)
      {
        this.m_numGoldText.gameObject.SetActive(true);
        this.m_numGoldText.Text = GameStrings.Format("GLUE_MASS_DISENCHANT_NUM_GOLDEN_CARDS", (object) this.m_numGoldCards.ToString());
        TransformUtil.SetLocalPosX((Component) this.m_numCardsText, 2.902672f);
        component.mesh = MassDisenchant.Get().m_rarityBarGoldMesh;
        this.m_barFrameMesh.SetMaterial(MassDisenchant.Get().m_rarityBarGoldMaterial);
      }
      else
      {
        this.m_numGoldText.gameObject.SetActive(false);
        TransformUtil.SetLocalPosX((Component) this.m_numCardsText, 2.902672f);
        component.mesh = MassDisenchant.Get().m_rarityBarNormalMesh;
        this.m_barFrameMesh.SetMaterial(MassDisenchant.Get().m_rarityBarNormalMaterial);
      }
    }
    this.SetPercent((double) totalNumCards > 0.0 ? (float) this.m_numCards / (float) totalNumCards : 0.0f);
  }

  public void SetPercent(float percent) => this.m_amountBar.GetComponent<Renderer>().GetMaterial().SetFloat("_Percent", percent);
}
