using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

public class SetFilterItem : PegUIElement
{
  public UberText m_uberText;
  public GameObject m_selectedGlow;
  public MeshRenderer m_icon;
  public GameObject m_mouseOverGlow;
  public GameObject m_pressedShadow;
  public GameObject m_iconFX;
  public TooltipZone m_tooltipZone;
  private bool m_isHeader;
  private FormatType m_formatType;
  private bool m_isAllStandard;
  private List<TAG_CARD_SET> m_cardSets;
  private List<int> m_metaShakeupEvents;
  private float m_height;
  private SetFilterItem.ItemSelectedCallback m_callback;
  private bool m_isSelected;
  private string m_tooltipHeadline;
  private string m_tooltipDescription;
  private bool m_showTooltip;

  public bool IsHeader
  {
    get => this.m_isHeader;
    set => this.m_isHeader = value;
  }

  public string Text
  {
    get => this.m_uberText.Text;
    set => this.m_uberText.Text = value;
  }

  public FormatType FormatType
  {
    get => this.m_formatType;
    set => this.m_formatType = value;
  }

  public bool IsAllStandard
  {
    get => this.m_isAllStandard;
    set => this.m_isAllStandard = value;
  }

  public List<TAG_CARD_SET> CardSets
  {
    get => this.m_cardSets;
    set => this.m_cardSets = value;
  }

  public List<int> SpecificCards
  {
    get => this.m_metaShakeupEvents;
    set => this.m_metaShakeupEvents = value;
  }

  public float Height
  {
    get => this.m_height;
    set => this.m_height = value;
  }

  public SetFilterItem.ItemSelectedCallback Callback
  {
    get => this.m_callback;
    set => this.m_callback = value;
  }

  public string TooltipHeadline
  {
    get => this.m_tooltipHeadline;
    set => this.m_tooltipHeadline = value;
  }

  public string TooltipDescription
  {
    get => this.m_tooltipDescription;
    set => this.m_tooltipDescription = value;
  }

  public bool ShowTooltip
  {
    get => this.m_showTooltip;
    set => this.m_showTooltip = value;
  }

  public Texture IconTexture
  {
    get => this.m_icon.GetMaterial().GetTexture("_MainTex");
    set
    {
      if ((Object) value == (Object) null)
        this.m_icon.gameObject.SetActive(false);
      else
        this.m_icon.gameObject.SetActive(true);
      this.m_icon.GetMaterial().SetTexture("_MainTex", value);
    }
  }

  public UnityEngine.Vector2? IconOffset
  {
    get => new UnityEngine.Vector2?(this.m_icon.GetMaterial().GetTextureOffset("_MainTex"));
    set
    {
      if (!value.HasValue || (Object) this.IconTexture == (Object) null)
      {
        this.m_icon.gameObject.SetActive(false);
      }
      else
      {
        this.m_icon.gameObject.SetActive((Object) this.IconTexture != (Object) null);
        this.m_icon.GetMaterial().SetTextureOffset("_MainTex", value.Value);
      }
    }
  }

  public TooltipZone Tooltip
  {
    get => this.m_tooltipZone;
    private set => this.m_tooltipZone = value;
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if ((Object) this.m_mouseOverGlow != (Object) null && !UniversalInputManager.Get().IsTouchMode())
    {
      this.m_mouseOverGlow.SetActive(true);
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9"), this.gameObject);
    }
    if (!((Object) this.m_tooltipZone != (Object) null) || !this.m_showTooltip)
      return;
    this.m_tooltipZone.ShowCollectionManagerTooltip(this.m_tooltipHeadline, this.m_tooltipDescription);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if ((Object) this.m_mouseOverGlow != (Object) null)
      this.m_mouseOverGlow.SetActive(false);
    if (!this.m_isSelected && (Object) this.m_pressedShadow != (Object) null)
      this.m_pressedShadow.SetActive(false);
    if (!((Object) this.m_tooltipZone != (Object) null))
      return;
    this.m_tooltipZone.HideTooltip();
  }

  public void SetSelected(bool selected)
  {
    this.m_selectedGlow.SetActive(selected);
    if ((Object) this.m_pressedShadow != (Object) null)
      this.m_pressedShadow.SetActive(selected);
    this.m_isSelected = selected;
  }

  protected override void OnPress()
  {
    if (!((Object) this.m_pressedShadow != (Object) null) || UniversalInputManager.Get().IsTouchMode())
      return;
    this.m_pressedShadow.SetActive(true);
  }

  protected override void OnRelease()
  {
    if (this.m_isSelected || !((Object) this.m_pressedShadow != (Object) null))
      return;
    this.m_pressedShadow.SetActive(false);
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681"));
  }

  public void SetIconFxActive(bool active)
  {
    if ((Object) this.m_iconFX == (Object) null)
      return;
    this.m_iconFX.SetActive(active);
  }

  public delegate void ItemSelectedCallback(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    FormatType formatType,
    SetFilterItem selectedItem,
    bool transitionPage);
}
