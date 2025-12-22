using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using UnityEngine;

public class CollectionCardCount : MonoBehaviour
{
  public GameObject m_normalTab;
  public UberText m_normalCountText;
  public GameObject m_normalBorder;
  public GameObject m_normalWideBorder;
  public GameObject m_normalBone;
  public GameObject m_goldenTab;
  public UberText m_goldenCountText;
  public GameObject m_goldenBorder;
  public GameObject m_goldenWideBorder;
  public GameObject m_goldenBone;
  public GameObject m_centerBone;
  [SerializeField]
  private Transform m_signatureBone;
  public Color m_standardTextColor;
  public Color m_wildTextColor;
  public Color m_classicTextColor;
  public Material m_standardBorderMaterial;
  public Material m_wildBorderMaterial;
  public Material m_classicBorderMaterial;
  public Color m_standardGoldTextColor;
  public Material m_standardGoldBorderMaterial;
  private TAG_PREMIUM m_premium;
  private int m_normalCount;
  private int m_goldenCount;
  private int m_signatureCount;
  private int m_diamondCount;

  public void SetCount(
    int normalCount,
    int goldenCount,
    int signatureCount,
    int diamondCount,
    TAG_PREMIUM premium)
  {
    this.m_normalCount = normalCount;
    this.m_goldenCount = goldenCount;
    this.m_signatureCount = signatureCount;
    this.m_diamondCount = diamondCount;
    this.m_premium = premium;
    this.UpdateVisibility();
  }

  public int GetCount(TAG_PREMIUM premium)
  {
    switch (premium)
    {
      case TAG_PREMIUM.NORMAL:
        return this.m_normalCount;
      case TAG_PREMIUM.GOLDEN:
        return this.m_goldenCount;
      case TAG_PREMIUM.DIAMOND:
        return this.m_diamondCount;
      case TAG_PREMIUM.SIGNATURE:
        return this.m_signatureCount;
      default:
        return 0;
    }
  }

  public void Show() => this.UpdateVisibility();

  public void Hide() => this.gameObject.SetActive(false);

  private void UpdateVisibility()
  {
    int count = this.GetCount(this.m_premium);
    if (count <= 1)
    {
      this.Hide();
    }
    else
    {
      this.gameObject.SetActive(true);
      this.m_normalTab.SetActive(false);
      this.m_normalBorder.SetActive(false);
      this.m_normalWideBorder.SetActive(false);
      this.m_goldenTab.SetActive(false);
      this.m_goldenBorder.SetActive(false);
      this.m_goldenWideBorder.SetActive(false);
      FormatType themeShowing = CollectionManager.Get().GetThemeShowing();
      Color color;
      if (new Map<FormatType, Color>()
      {
        {
          FormatType.FT_STANDARD,
          this.m_standardTextColor
        },
        {
          FormatType.FT_WILD,
          this.m_wildTextColor
        },
        {
          FormatType.FT_CLASSIC,
          this.m_classicTextColor
        }
      }.TryGetValue(themeShowing, out color))
        this.m_normalCountText.TextColor = color;
      else
        Debug.LogWarning((object) ("CollectionCardCount.UpdateVisibility failed to find text color for format" + themeShowing.ToString()));
      Material material;
      if (new Map<FormatType, Material>()
      {
        {
          FormatType.FT_STANDARD,
          this.m_standardBorderMaterial
        },
        {
          FormatType.FT_WILD,
          this.m_wildBorderMaterial
        },
        {
          FormatType.FT_CLASSIC,
          this.m_classicBorderMaterial
        }
      }.TryGetValue(themeShowing, out material))
      {
        this.m_normalWideBorder.GetComponent<Renderer>().SetMaterial(material);
        this.m_normalBorder.GetComponent<Renderer>().SetMaterial(material);
      }
      else
        Debug.LogWarning((object) ("CollectionCardCount.UpdateVisibility failed to find material for format" + themeShowing.ToString()));
      this.m_goldenCountText.TextColor = this.m_standardGoldTextColor;
      this.m_goldenWideBorder.GetComponent<Renderer>().SetMaterial(this.m_standardGoldBorderMaterial);
      this.m_goldenBorder.GetComponent<Renderer>().SetMaterial(this.m_standardGoldBorderMaterial);
      if (count < 10)
      {
        this.m_normalBorder.SetActive(true);
        this.m_normalCountText.Text = GameStrings.Format("GLUE_COLLECTION_CARD_COUNT", (object) count);
      }
      else if (count > 0)
      {
        this.m_normalWideBorder.SetActive(true);
        this.m_normalCountText.Text = GameStrings.Get("GLUE_COLLECTION_CARD_COUNT_LARGE");
      }
      if (count <= 0)
        return;
      this.m_normalTab.SetActive(true);
      if (this.m_premium == TAG_PREMIUM.SIGNATURE)
        this.m_normalTab.transform.position = this.m_signatureBone.position;
      else
        this.m_normalTab.transform.position = this.m_centerBone.transform.position;
    }
  }
}
