using Hearthstone.UI.Core;
using UnityEngine;

public class ShopButtonDisplay : MonoBehaviour
{
  [SerializeField]
  protected ShopButtonDisplay.DisplayType displayType;
  [SerializeField]
  protected ShopButtonDisplay.ProductIndex m_index;

  [Overridable]
  public ShopButtonDisplay.ProductIndex Index
  {
    get => this.m_index;
    set => this.m_index = value;
  }

  public enum DisplayType
  {
    BOOSTER,
    HERO,
    CARDBACK,
  }

  public enum ProductIndex
  {
    AUTO,
    FIRST,
    SECOND,
    THIRD,
  }
}
