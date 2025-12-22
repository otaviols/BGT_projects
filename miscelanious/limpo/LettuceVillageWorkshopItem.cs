using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class LettuceVillageWorkshopItem : MonoBehaviour
{
  [Tooltip("The sprite renderer that draws the building icon")]
  public SpriteRenderer BuildingSprite;
  [Tooltip("Used to configure which icon is shown for each building / tier level")]
  public LettuceVillageWorkshopItem.BuildingIconsDef[] BuildingIcons;
  private Widget m_widget;
  private static readonly MercenaryVillageWorkshopItemDataModel s_prewarmModel = new MercenaryVillageWorkshopItemDataModel()
  {
    Prewarm = true,
    Price = new PriceDataModel()
    {
      Amount = 1f,
      Currency = CurrencyType.GOLD,
      DisplayText = "1"
    }
  };

  private void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.BindDataModel((IDataModel) LettuceVillageWorkshopItem.s_prewarmModel);
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnEvent));
  }

  private void OnEvent(string eventName)
  {
    if (!string.Equals(eventName, "SET_ICON", StringComparison.Ordinal))
      return;
    MercenaryVillageWorkshopItemDataModel dataModel = this.m_widget.GetDataModel<MercenaryVillageWorkshopItemDataModel>();
    this.SetBuildingSprite(dataModel.BuildingType, dataModel.CurrentTierId);
  }

  private void SetBuildingSprite(MercenaryBuilding.Mercenarybuildingtype type, int tierId)
  {
    LettuceVillageWorkshopItem.TierToSpriteReference[] toSpriteReferenceArray = (LettuceVillageWorkshopItem.TierToSpriteReference[]) null;
    foreach (LettuceVillageWorkshopItem.BuildingIconsDef buildingIcon in this.BuildingIcons)
    {
      if (buildingIcon.BuildingType == type)
      {
        toSpriteReferenceArray = buildingIcon.Icons;
        break;
      }
    }
    LettuceVillageWorkshopItem.TierToSpriteReference toSpriteReference1 = (LettuceVillageWorkshopItem.TierToSpriteReference) null;
    if (toSpriteReferenceArray != null && toSpriteReferenceArray.Length != 0)
    {
      foreach (LettuceVillageWorkshopItem.TierToSpriteReference toSpriteReference2 in toSpriteReferenceArray)
      {
        if (toSpriteReference2.TierID == tierId)
        {
          toSpriteReference1 = toSpriteReference2;
          break;
        }
      }
    }
    this.BuildingSprite.sprite = toSpriteReference1?.Icon;
  }

  [Serializable]
  public class TierToSpriteReference
  {
    public int TierID;
    public Sprite Icon;
  }

  [Serializable]
  public class BuildingIconsDef
  {
    public MercenaryBuilding.Mercenarybuildingtype BuildingType;
    public LettuceVillageWorkshopItem.TierToSpriteReference[] Icons;
  }
}
