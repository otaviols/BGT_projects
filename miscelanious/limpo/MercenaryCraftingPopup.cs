using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using System;
using UnityEngine;

public class MercenaryCraftingPopup : MonoBehaviour
{
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_mercCraftingPopupReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_mercCardReference;
  private Widget m_mercPopupManagerVisualController;
  private VisualController m_mercCraftingPopupVisualController;
  private Hearthstone.UI.Card m_mercCard;
  private MaterialDataModel m_materialData = new MaterialDataModel();

  private void Start()
  {
    this.m_mercPopupManagerVisualController = this.gameObject.GetComponent<Widget>();
    this.m_mercPopupManagerVisualController.RegisterEventListener(new Widget.EventListenerDelegate(this.MercPopupEventListener));
    this.m_mercCraftingPopupReference.RegisterReadyListener<VisualController>((Action<VisualController>) (vc =>
    {
      this.m_mercCraftingPopupVisualController = vc;
      this.m_mercCraftingPopupVisualController.BindDataModel((IDataModel) this.m_materialData);
    }));
    this.m_mercCardReference.RegisterReadyListener<Hearthstone.UI.Card>((Action<Hearthstone.UI.Card>) (card => this.m_mercCard = card));
    Network.Get().RegisterNetHandler((object) CraftMercenaryResponse.PacketID.ID, new Network.NetHandler(this.OnCraftMercenaryNetworkResponse));
  }

  private void OnDestroy() => Network.Get()?.RemoveNetHandler((object) CraftMercenaryResponse.PacketID.ID, new Network.NetHandler(this.OnCraftMercenaryNetworkResponse));

  private void MercPopupEventListener(string eventName)
  {
    if (!(eventName == "MERC_CRAFT_code"))
    {
      if (!(eventName == "MERC_CRAFT_COMPLETE_code"))
        return;
      this.OnCraftMercenaryComplete();
    }
    else
      this.CraftMercenary();
  }

  public void ShowCraftingPopup(LettuceMercenaryDataModel mercData)
  {
    if ((UnityEngine.Object) this.m_mercPopupManagerVisualController == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintWarning("MercenaryCraftingPopup.ShowCraftingPopup - no merc popup manager visual controller found!");
    }
    else
    {
      this.m_mercPopupManagerVisualController.BindDataModel((IDataModel) mercData);
      this.m_mercPopupManagerVisualController.TriggerEvent("MERC_CRAFTING_POPUP_show");
    }
  }

  private void CraftMercenary()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_mercPopupManagerVisualController.GetComponent<VisualController>());
    if (eventDataModel == null)
      Log.Lettuce.PrintError("No event data model attached to the LettuceMercCraftingPopup");
    else if (!(eventDataModel.Payload is LettuceMercenaryDataModel payload))
    {
      Log.Lettuce.PrintError("Event data attached to LettuceMercCraftingPopup not of expected type!");
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) payload.MercenaryId);
      if (mercenary == null)
      {
        Log.Lettuce.PrintWarning("LettuceCollectionDisplay.CraftMercenary - no mercenary found with ID {0}", (object) payload.MercenaryId);
      }
      else
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
        if (mercenary.m_currencyAmount < (long) mercenary.GetCraftingCost())
        {
          info.m_headerText = GameStrings.Get("GLUE_LETTUCE_MERCENARY_CRAFTING_CONFIRMATION_HEADER");
          info.m_text = GameStrings.Get("GLUE_LETTUCE_MERCENARY_CRAFTING_NOT_ENOUGH_COIN_BODY");
          info.m_showAlertIcon = false;
          info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
        }
        else
        {
          info.m_headerText = GameStrings.Get("GLUE_LETTUCE_MERCENARY_CRAFTING_CONFIRMATION_HEADER");
          info.m_text = GameStrings.Get("GLUE_LETTUCE_MERCENARY_CRAFTING_CONFIRMATION_BODY");
          info.m_showAlertIcon = false;
          info.m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle;
          info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
          info.m_confirmText = GameStrings.Get("GLUE_LETTUCE_MERCENARY_CRAFT_TITLE");
          info.m_cancelText = GameStrings.Get("GLOBAL_CANCEL");
          info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnMercenaryCraftingPopupResponse);
          info.m_responseUserData = (object) payload;
        }
        DialogManager.Get().ShowPopup(info);
      }
    }
  }

  private void OnMercenaryCraftingPopupResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL || !(userData is LettuceMercenaryDataModel mercenaryDataModel))
      return;
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryDataModel.MercenaryId);
    if (mercenary == null)
      Log.Lettuce.PrintWarning("LettuceCollectionDisplay.OnMercenaryCraftingPopupResponse - no mercenary found with ID {0}", (object) mercenaryDataModel.MercenaryId);
    else if (mercenary.m_owned)
      Log.Lettuce.PrintWarning("LettuceCollectionDisplay.OnMercenaryCraftingPopupResponse - mercenary ID {0} in craft request already owned!", (object) mercenary.ID);
    else if (mercenary.m_currencyAmount < (long) mercenary.GetCraftingCost())
    {
      Log.Lettuce.PrintWarning("LettuceCollectionDisplay.OnMercenaryCraftingPopupResponse - Mercenary ID {0} requires {1} coins to craft, but only has {2}", (object) mercenary.ID, (object) mercenary.GetCraftingCost(), (object) mercenary.m_currencyAmount);
    }
    else
    {
      if (Network.IsLoggedIn())
        Network.Get().CraftMercenary(mercenary.ID);
      this.m_materialData.Material = this.m_mercCard.CardActor.GetPortraitMaterial();
      if (!((UnityEngine.Object) this.m_mercCraftingPopupVisualController != (UnityEngine.Object) null))
        return;
      this.m_mercCraftingPopupVisualController.OwningWidget.TriggerEvent("PLAY_EFFECTS", new Widget.TriggerEventParameters());
    }
  }

  private void OnCraftMercenaryNetworkResponse()
  {
    CraftMercenaryResponse mercenaryResponse = Network.Get().CraftMercenaryResponse();
    if (mercenaryResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.Lettuce.PrintError("LettuceCollectionDisplay.OnCraftMercenaryNetworkResponse - Error Code {0} crafting mercenary ID {1}", (object) mercenaryResponse.ErrorCode, (object) mercenaryResponse.MercenaryId);
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryResponse.MercenaryId);
      if (mercenary == null)
      {
        Log.Lettuce.PrintWarning("LettuceCollectionDisplay.OnCraftMercenaryNetworkResponse - No mercenary found with ID {0}.", (object) mercenaryResponse.MercenaryId);
      }
      else
      {
        mercenary.m_owned = true;
        mercenary.m_currencyAmount = mercenaryResponse.CurrencyFinal;
      }
    }
  }

  private void OnCraftMercenaryComplete()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_mercPopupManagerVisualController.GetComponent<VisualController>());
    if (eventDataModel == null)
      Log.Lettuce.PrintError("No event data model attached to the LettuceMercCraftingPopup");
    else if (!(eventDataModel.Payload is LettuceMercenaryDataModel payload))
    {
      Log.Lettuce.PrintError("Event data attached to LettuceMercCraftingPopup not of expected type!");
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) payload.MercenaryId);
      if (mercenary == null)
      {
        Log.Lettuce.PrintWarning("LettuceCollectionDisplay.CraftMercenary - no mercenary found with ID {0}", (object) payload.MercenaryId);
      }
      else
      {
        LettuceCollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager;
        if ((UnityEngine.Object) pageManager == (UnityEngine.Object) null)
          Log.Lettuce.PrintWarning("LettuceCollectionDisplay.OnCraftMercenaryNetworkResponse - Unable to retrieve LettuceCollectionPageManager!");
        else
          pageManager.UpdatePageMercenary(MercenaryFactory.CreateMercenaryDataModelWithCoin(mercenary));
      }
    }
  }
}
