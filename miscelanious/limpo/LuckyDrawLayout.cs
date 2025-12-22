using Hearthstone.DataModels;
using Hearthstone.UI;
using System.Collections.Generic;
using UnityEngine;

public class LuckyDrawLayout : MonoBehaviour
{
  private Widget m_widget;
  private PlayMakerFSM m_playmaker;
  public List<WidgetInstance> m_commonRewardsList;
  public List<WidgetInstance> m_legendaryRewardsList;
  private List<WidgetInstance> m_unownedRewardList;
  private int m_rewardTileToAnimate;
  private int m_maxTilesToAnimate;
  private List<WidgetInstance> m_fullRewardList;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("Error", "[LuckyDrawLayout] Awake() no Widget Template found for {0}", (object) this.gameObject.name);
    }
    else
    {
      this.m_playmaker = this.GetComponent<PlayMakerFSM>();
      if ((UnityEngine.Object) this.m_playmaker == (UnityEngine.Object) null)
      {
        Error.AddDevWarning("Error", "[LuckyDrawLayout] Awake() no PlaymakerFSM found for {0}", (object) this.gameObject.name);
      }
      else
      {
        this.m_fullRewardList = new List<WidgetInstance>((IEnumerable<WidgetInstance>) this.m_commonRewardsList);
        this.m_fullRewardList.AddRange((IEnumerable<WidgetInstance>) this.m_legendaryRewardsList);
        this.m_unownedRewardList = new List<WidgetInstance>();
      }
    }
  }

  public void InitializeRewardTileWidgets(DataModelList<LuckyDrawRewardDataModel> rewardList)
  {
    int index1 = 0;
    int index2 = 0;
    foreach (LuckyDrawRewardDataModel reward in rewardList)
    {
      LuckyDrawRewardDataModel rewardItem;
      if ((rewardItem = reward).Style == LuckyDrawStyle.COMMON)
      {
        if (this.m_commonRewardsList.Count <= index1)
        {
          Error.AddDevWarning("Error", "[LuckyDrawLayout] InitializeRewardTileWidget() Reward List has more common items than available slots! was the HE2 data setup properly?");
        }
        else
        {
          WidgetInstance commonRewards = this.m_commonRewardsList[index1];
          this.SetupTileWidget(rewardItem, commonRewards);
          ++index1;
        }
      }
      else if (this.m_legendaryRewardsList.Count <= index2)
      {
        Error.AddDevWarning("Error", "[LuckyDrawLayout] InitializeRewardTileWidget() Reward List has more legendary items than available slots! Was the HE2 data setup properly?");
      }
      else
      {
        WidgetInstance legendaryRewards = this.m_legendaryRewardsList[index2];
        this.SetupTileWidget(rewardItem, legendaryRewards);
        ++index2;
      }
    }
    if (index1 < this.m_commonRewardsList.Count)
      Error.AddDevWarning("Error", "[LuckyDrawLayout] InitializeRewardTileWidget() Common reward list not fully filled. The number of common rewards does not match the reward list length and tiles will be empty! Was the HE2 data setup properly?");
    if (index2 >= this.m_legendaryRewardsList.Count)
      return;
    Error.AddDevWarning("Error", "[LuckyDrawLayout] InitailizeRewardTileWidget() Legendary reward list not fully filled. The number of legendary rewards does not match the reward list length and tiles will be empty! Was the HE2 data setup properly?");
  }

  public Vector3 GetWorldPositionOfTile(int tileNumber) => this.m_fullRewardList[tileNumber].transform.position;

  private void SetupTileWidget(LuckyDrawRewardDataModel rewardItem, WidgetInstance rewardWidget)
  {
    if ((UnityEngine.Object) rewardWidget == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error", "[LuckyDrawLayout] SetupTileWidget() rewardWidget was null! Cant setup rewardTile.");
    else if (rewardItem == null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawLayout] SetupTileWidget() rewardItem was null! No data to setup rewardTile");
    }
    else
    {
      rewardWidget.BindDataModel((IDataModel) rewardItem, false);
      rewardWidget.BindDataModel((IDataModel) rewardItem.RewardList, false);
    }
  }

  public void AnimateTiles()
  {
    if ((UnityEngine.Object) this.m_playmaker == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error", "[LuckyDrawLayout] AnimateTiles() playmaker not found! Cant do animation!");
    }
    else
    {
      this.PopulateUnownedRewardList();
      this.m_maxTilesToAnimate = this.m_playmaker.FsmVariables.GetFsmInt("MaxNumberTilesToAnimate").Value;
      float num1 = this.m_playmaker.FsmVariables.GetFsmFloat("LowThresholdAnimationMultiplier").Value;
      float num2 = this.m_playmaker.FsmVariables.GetFsmFloat("TileUpTime").Value;
      float num3 = this.m_playmaker.FsmVariables.GetFsmFloat("TileDownTime").Value;
      this.m_playmaker.FsmVariables.GetFsmFloat("LowThresholdTileUpTiming").Value = num2 * num1;
      this.m_playmaker.FsmVariables.GetFsmFloat("LowThresholdTileDownTiming").Value = num3 * num1;
      int num4 = this.m_playmaker.FsmVariables.GetFsmInt("LowBoxThreshold").Value;
      this.m_playmaker.FsmVariables.GetFsmBool("LowTileThresholdReached").Value = this.m_unownedRewardList.Count <= num4;
      this.ShuffleUnownedRewardList();
      this.m_rewardTileToAnimate = 0;
      this.AnimateNextTile();
    }
  }

  public void AnimateNextTile()
  {
    if (this.m_rewardTileToAnimate > this.m_maxTilesToAnimate || this.m_rewardTileToAnimate >= this.m_unownedRewardList.Count)
    {
      this.m_playmaker.SendEvent("All_Finished");
    }
    else
    {
      this.m_playmaker.FsmVariables.GetFsmGameObject("TargetTile").Value = this.m_unownedRewardList[this.m_rewardTileToAnimate].gameObject;
      this.m_playmaker.FsmVariables.GetFsmVector3("TileInitialPosition").Value = this.m_unownedRewardList[this.m_rewardTileToAnimate].transform.position;
      this.m_playmaker.SendEvent("Animate_Tile");
      ++this.m_rewardTileToAnimate;
    }
  }

  public void PlayTileSmashAnim(int tileNumber)
  {
    this.m_fullRewardList[tileNumber].GetComponentInChildren<PlayMakerFSM>().SendEvent("Code_Box_Smashed");
    this.SetTileOwned(tileNumber);
  }

  private void PopulateUnownedRewardList()
  {
    this.m_unownedRewardList.Clear();
    foreach (WidgetInstance fullReward in this.m_fullRewardList)
    {
      IDataModel model;
      if (fullReward.GetDataModel(667, out model) && !(model as LuckyDrawRewardDataModel).IsOwned)
        this.m_unownedRewardList.Add(fullReward);
    }
  }

  private void ShuffleUnownedRewardList()
  {
    System.Random random = new System.Random();
    for (int index1 = this.m_unownedRewardList.Count - 1; index1 > 0; --index1)
    {
      int index2 = random.Next(index1 + 1);
      WidgetInstance unownedReward = this.m_unownedRewardList[index2];
      this.m_unownedRewardList[index2] = this.m_unownedRewardList[index1];
      this.m_unownedRewardList[index1] = unownedReward;
    }
  }

  public int GetTileFromRewardID(int rewardID)
  {
    for (int index = 0; index < this.m_fullRewardList.Count; ++index)
    {
      LuckyDrawRewardDataModel boundRewardDataModel = this.m_fullRewardList[index].GetComponentInChildren<LuckyDrawTile>().GetBoundRewardDataModel();
      if (boundRewardDataModel != null && boundRewardDataModel.RewardID == rewardID)
        return index;
    }
    return -1;
  }

  public void SetTileOwned(int tileNumber)
  {
    IDataModel model;
    if (!this.m_fullRewardList[tileNumber].GetDataModel(667, out model))
      return;
    LuckyDrawRewardDataModel targetData = model as LuckyDrawRewardDataModel;
    targetData.IsOwned = true;
    this.RemoveTileFromUnownedTileList(targetData);
    LuckyDrawManager.Get()?.OnLuckyDrawHammerAnimationFinished();
  }

  private void RemoveTileFromUnownedTileList(LuckyDrawRewardDataModel targetData)
  {
    foreach (WidgetInstance unownedReward in this.m_unownedRewardList)
    {
      IDataModel model;
      if (unownedReward.GetDataModel(667, out model) && (model as LuckyDrawRewardDataModel).RewardID == targetData.RewardID)
      {
        this.m_unownedRewardList.Remove(unownedReward);
        break;
      }
    }
  }
}
