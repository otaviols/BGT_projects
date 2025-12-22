using Hearthstone.DataModels;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

public class UpgradeToGoldenPopup : MonoBehaviour
{
  private Widget m_widget;
  private CraftingDataModel m_craftingDataModel = new CraftingDataModel();
  private CraftingUI m_craftingUI;
  private const string CODE_CREATE_EVENT = "CODE_CREATE";
  private const string CODE_UPGRADE_EVENT = "CODE_UPGRADE";
  private const string CODE_HIDE_EVENT = "CODE_HIDE";
  private const string GROW_EVENT = "GROW";
  private const string SHRINK_EVENT = "SHRINK";

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
    this.m_widget.BindDataModel((IDataModel) this.m_craftingDataModel);
  }

  public void SetInfo(
    CraftingPendingTransaction pendingTransaction,
    CraftingUI craftingUI,
    Transform showBone)
  {
    this.m_craftingUI = craftingUI;
    this.m_widget.transform.position = showBone.position;
    this.m_widget.transform.localScale = showBone.localScale;
    CraftingManager.Get().SetCraftingRelatedActorsActiveForUpgradeToGoldenPopup(false);
    this.m_craftingDataModel.IsGolden = pendingTransaction.Premium == TAG_PREMIUM.GOLDEN;
    int upgradeValue;
    CraftingManager.Get().TryGetCardUpgradeValue(pendingTransaction.CardID, out upgradeValue);
    this.m_craftingDataModel.UpgradeDustCost = upgradeValue;
    int buyValue;
    CraftingManager.Get().TryGetCardBuyValue(pendingTransaction.CardID, pendingTransaction.Premium, out buyValue);
    this.m_craftingDataModel.CreateDustCost = buyValue;
    this.m_craftingDataModel.NumOwnedNormal = CraftingManager.Get().GetNumOwnedIncludePending(pendingTransaction.CardID, new TAG_PREMIUM?(TAG_PREMIUM.NORMAL));
    this.m_craftingDataModel.NumOwnedGolden = CraftingManager.Get().GetNumOwnedIncludePending(pendingTransaction.CardID, new TAG_PREMIUM?(TAG_PREMIUM.GOLDEN));
  }

  public void OnHide()
  {
    this.m_widget.TriggerEvent("SHRINK");
    CraftingManager.Get().SetCraftingRelatedActorsActiveForUpgradeToGoldenPopup(true);
  }

  public IEnumerator ShowWhenReadyRoutine()
  {
    while (this.m_widget.IsChangingStates)
      yield return (object) null;
    this.m_widget.TriggerEvent("GROW");
    this.m_widget.Show();
  }

  private void HandleEvent(string eventName)
  {
    CraftingManager craftingManager = CraftingManager.Get();
    if (!(eventName == "CODE_CREATE"))
    {
      if (!(eventName == "CODE_UPGRADE"))
      {
        if (!(eventName == "CODE_HIDE"))
          return;
        craftingManager.HideUpgradeToGoldenWidget();
      }
      else
      {
        CraftingPendingTransaction.Operation transaction = craftingManager.GetShownActor().GetPremium() == TAG_PREMIUM.NORMAL ? CraftingPendingTransaction.Operation.UpgradeToGoldenFromNormal : CraftingPendingTransaction.Operation.UpgradeToGoldenFromGolden;
        craftingManager.GetPendingClientTransaction().Add(transaction);
        int upgradeValue;
        craftingManager.TryGetCardUpgradeValue(craftingManager.GetShownActor().GetEntityDef().GetCardId(), out upgradeValue);
        craftingManager.AdjustUnCommitedArcaneDustChanges(-upgradeValue);
        craftingManager.SwitchPremiumView(TAG_PREMIUM.GOLDEN);
        craftingManager.HideUpgradeToGoldenWidget();
        this.m_craftingUI.DoUpgradeToGoldenAnimations();
      }
    }
    else
    {
      CraftingPendingTransaction.Operation transaction = craftingManager.GetShownActor().GetPremium() == TAG_PREMIUM.NORMAL ? CraftingPendingTransaction.Operation.NormalCreate : CraftingPendingTransaction.Operation.GoldenCreate;
      int buyValue;
      craftingManager.TryGetCardBuyValue(craftingManager.GetShownActor().GetEntityDef().GetCardId(), craftingManager.GetShownActor().GetPremium(), out buyValue);
      craftingManager.AdjustUnCommitedArcaneDustChanges(-buyValue);
      craftingManager.GetPendingClientTransaction().Add(transaction);
      craftingManager.HideUpgradeToGoldenWidget();
      this.m_craftingUI.DoUpgradeToGoldenAnimations();
    }
  }
}
