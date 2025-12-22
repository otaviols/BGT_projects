using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LettuceMap : MonoBehaviour
{
  public AsyncReference m_FinalBossChestReference;
  public Transform MapBoundsLeftBone;
  public Transform MapBoundsRightBone;
  public bool EnableRandomCoinPositionsInRow = true;
  private const float MinDistanceBetweenCoinsOnRow = 15f;
  private Dictionary<LettuceMapCoinDataModel, LettuceMapCoin> m_lettuceCoinsByDataModel = new Dictionary<LettuceMapCoinDataModel, LettuceMapCoin>();
  private Dictionary<int, LettuceMapCoin> m_lettuceCoinsByNodeId = new Dictionary<int, LettuceMapCoin>();
  private List<LettuceMapCoinDataModel> m_lettuceCoinDataModels = new List<LettuceMapCoinDataModel>();
  private int m_numDataModelsLeftToRegister;
  private bool m_allLinesDrawn;
  private bool m_finalBossChestFinishedLoading;
  private LettuceMapCoinDataModel m_selectedCoin;
  private GameObject m_finalBossChest;
  private bool m_isFinalBossDefeated;
  private int m_seed;
  private List<DefLoader.DisposableCardDef> m_loadedCoinCardDefs = new List<DefLoader.DisposableCardDef>();

  public int NumberOfRows { get; private set; }

  public List<LettuceMapNode> NodeData { get; private set; }

  private void Start() => this.m_FinalBossChestReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnFinalBossChestReady));

  private void OnDestroy()
  {
    foreach (DefLoader.DisposableCardDef loadedCoinCardDef in this.m_loadedCoinCardDefs)
      loadedCoinCardDef.Dispose();
  }

  private LettuceMapDataModel GetLettuceMapDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (LettuceMapDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(198, out model))
    {
      model = (IDataModel) new LettuceMapDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceMapDataModel;
  }

  public void OnFinalBossChestReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "FinalBossChest could not be found!");
    }
    else
    {
      this.m_finalBossChest = visualController.gameObject;
      this.m_finalBossChestFinishedLoading = true;
    }
  }

  public bool IsFinishedLoading() => this.m_allLinesDrawn && this.m_finalBossChestFinishedLoading;

  public void CreateMapFromProto(PegasusLettuce.LettuceMap lettuceMap)
  {
    LettuceMapDataModel lettuceMapDataModel = this.GetLettuceMapDataModel();
    if (lettuceMapDataModel == null)
      Log.Lettuce.PrintError("CreateMapFromProto: No data model for lettuce map.");
    else if (lettuceMap == null)
    {
      Log.Lettuce.PrintError("CreateMapFromProto: No map provided.");
    }
    else
    {
      this.NodeData = lettuceMap.Nodes;
      int num = 0;
      Dictionary<uint, List<LettuceMapNode>> dictionary1 = new Dictionary<uint, List<LettuceMapNode>>();
      Dictionary<uint, DataModelList<int>> dictionary2 = new Dictionary<uint, DataModelList<int>>();
      foreach (LettuceMapNode node in lettuceMap.Nodes)
      {
        if (!dictionary1.ContainsKey(node.Row))
          dictionary1.Add(node.Row, new List<LettuceMapNode>());
        dictionary1[node.Row].Add(node);
        if ((long) node.Row > (long) num)
          num = (int) node.Row;
        if (GameUtils.IsFinalBossNodeType((int) node.NodeTypeId) && node.NodeState_ == LettuceMapNode.NodeState.COMPLETE)
          this.m_isFinalBossDefeated = true;
        foreach (uint childNodeId in node.ChildNodeIds)
        {
          if (!dictionary2.ContainsKey(childNodeId))
            dictionary2.Add(childNodeId, new DataModelList<int>());
          dictionary2[childNodeId].Add((int) node.NodeId);
        }
      }
      if (!dictionary1.ContainsKey(0U))
      {
        Debug.LogError((object) "LettuceMap had no root node (no node with row == 0)!");
      }
      else
      {
        lettuceMapDataModel.Rows = new DataModelList<LettuceMapRowDataModel>();
        for (int key = num; key >= 0; --key)
        {
          DataModelList<LettuceMapCoinDataModel> dataModelList1 = new DataModelList<LettuceMapCoinDataModel>();
          foreach (LettuceMapNode node in dictionary1[(uint) key])
          {
            DataModelList<int> dataModelList2 = new DataModelList<int>();
            foreach (uint childNodeId in node.ChildNodeIds)
              dataModelList2.Add((int) childNodeId);
            DataModelList<int> dataModelList3 = !dictionary2.ContainsKey(node.NodeId) ? new DataModelList<int>() : dictionary2[node.NodeId];
            LettuceMapCoinDataModel coinDataModel = new LettuceMapCoinDataModel()
            {
              Id = (int) node.NodeId,
              NeighborIds = dataModelList2,
              NodeTypeId = (int) node.NodeTypeId,
              MercenaryRole = node.NodeRole,
              CoinState = node.NodeState_,
              CoinData = this.GetCoinDataForNode(node),
              ParentIds = dataModelList3,
              NodeVisualId = node.NodeTypeId > 0U ? GameDbf.LettuceMapNodeType.GetRecord((int) node.NodeTypeId).NodeVisualId : string.Empty
            };
            string headerString;
            string bodyString;
            this.GetTooltipStringsForNodeType((int) node.NodeTypeId, GameUtils.GetMercenaryTagRoleFromProtoRole(node.NodeRole), out headerString, out bodyString);
            coinDataModel.HoverTooltipHeader = headerString;
            coinDataModel.HoverTooltipBody = bodyString;
            this.SetGrantedAnomalyCardFromMapData(lettuceMap, (int) node.NodeId, coinDataModel);
            dataModelList1.Add(coinDataModel);
            this.m_lettuceCoinDataModels.Add(coinDataModel);
            ++this.m_numDataModelsLeftToRegister;
          }
          LettuceMapRowDataModel lettuceMapRowDataModel = new LettuceMapRowDataModel()
          {
            Coins = dataModelList1
          };
          lettuceMapDataModel.Rows.Add(lettuceMapRowDataModel);
        }
        this.NumberOfRows = lettuceMapDataModel.Rows.Count;
        this.m_seed = lettuceMap.Seed;
        this.m_lettuceCoinDataModels = this.m_lettuceCoinDataModels.OrderBy<LettuceMapCoinDataModel, int>((Func<LettuceMapCoinDataModel, int>) (c => c.Id)).ToList<LettuceMapCoinDataModel>();
      }
    }
  }

  public void RegisterCoin(LettuceMapCoin coin, LettuceMapCoinDataModel coinDataModel)
  {
    if (coinDataModel == null)
    {
      Debug.LogError((object) "LettuceMap.RegisterCoin() - Coin had no data model!");
    }
    else
    {
      this.m_lettuceCoinsByDataModel.Add(coinDataModel, coin);
      this.m_lettuceCoinsByNodeId.Add(coinDataModel.Id, coin);
      --this.m_numDataModelsLeftToRegister;
      if (this.m_numDataModelsLeftToRegister != 0)
        return;
      this.OnAllCoinsLoaded();
    }
  }

  public void SelectCoin(LettuceMapCoinDataModel selectedCoin)
  {
    if (!this.m_lettuceCoinsByDataModel.ContainsKey(selectedCoin))
    {
      Debug.LogError((object) ("SelectCoin() - No coin with id=" + (object) selectedCoin.Id));
    }
    else
    {
      if (this.m_selectedCoin != null)
      {
        this.m_selectedCoin.CoinData.Selected = false;
        this.m_lettuceCoinsByDataModel[this.m_selectedCoin].GetComponent<Widget>().BindDataModel((IDataModel) this.m_selectedCoin);
      }
      selectedCoin.CoinData.Selected = true;
      this.m_lettuceCoinsByDataModel[selectedCoin].GetComponent<Widget>().BindDataModel((IDataModel) selectedCoin);
      this.m_selectedCoin = selectedCoin;
    }
  }

  public void RefreshWithNewData(PegasusLettuce.LettuceMap updatedMapData)
  {
    foreach (LettuceMapCoinDataModel key in this.m_lettuceCoinsByDataModel.Keys)
    {
      LettuceMapCoinDataModel coinDataModel = key;
      LettuceMapNode node = updatedMapData.Nodes.FirstOrDefault<LettuceMapNode>((Func<LettuceMapNode, bool>) (n => (long) n.NodeId == (long) coinDataModel.Id));
      if (node != null)
      {
        coinDataModel.CoinData = this.GetCoinDataForNode(node);
        coinDataModel.CoinState = node.NodeState_;
        coinDataModel.MercenaryRole = node.NodeRole;
        coinDataModel.NodeTypeId = (int) node.NodeTypeId;
        if (node.NodeTypeId > 0U)
          coinDataModel.NodeVisualId = GameDbf.LettuceMapNodeType.GetRecord((int) node.NodeTypeId).NodeVisualId;
        string headerString;
        string bodyString;
        this.GetTooltipStringsForNodeType((int) node.NodeTypeId, GameUtils.GetMercenaryTagRoleFromProtoRole(node.NodeRole), out headerString, out bodyString);
        coinDataModel.HoverTooltipHeader = headerString;
        coinDataModel.HoverTooltipBody = bodyString;
        this.SetGrantedAnomalyCardFromMapData(updatedMapData, coinDataModel.Id, coinDataModel);
      }
    }
    this.UpdateCoinGlowLines();
    this.NodeData = updatedMapData.Nodes;
  }

  public List<LettuceMapCoin> GetCompletedCoins()
  {
    List<LettuceMapCoin> source = new List<LettuceMapCoin>();
    foreach (LettuceMapCoinDataModel key in this.m_lettuceCoinsByDataModel.Keys)
    {
      if (key.CoinState == LettuceMapNode.NodeState.COMPLETE)
      {
        LettuceMapCoin lettuceMapCoin = this.m_lettuceCoinsByDataModel[key];
        source.Add(lettuceMapCoin);
      }
    }
    return source.OrderBy<LettuceMapCoin, int>((Func<LettuceMapCoin, int>) (c => c.NodeId)).ToList<LettuceMapCoin>();
  }

  public List<LettuceMapCoinDataModel> GetUnlockedCoinDataModels()
  {
    List<LettuceMapCoinDataModel> unlockedCoinDataModels = new List<LettuceMapCoinDataModel>();
    foreach (LettuceMapCoinDataModel key in this.m_lettuceCoinsByDataModel.Keys)
    {
      if (key.CoinState == LettuceMapNode.NodeState.UNLOCKED)
        unlockedCoinDataModels.Add(key);
    }
    return unlockedCoinDataModels;
  }

  public LettuceMapCoinDataModel GetCoinDataModelById(int id)
  {
    foreach (LettuceMapCoinDataModel key in this.m_lettuceCoinsByDataModel.Keys)
    {
      if (key.Id == id)
        return key;
    }
    return (LettuceMapCoinDataModel) null;
  }

  public LettuceMapCoinDataModel GetFinalBossCoinDataModel() => this.m_lettuceCoinDataModels.Last<LettuceMapCoinDataModel>();

  public LettuceMapCoinDataModel GetDefeatCoinDataModel() => this.m_lettuceCoinDataModels.FirstOrDefault<LettuceMapCoinDataModel>((Func<LettuceMapCoinDataModel, bool>) (dataModel => dataModel.CoinState == LettuceMapNode.NodeState.DEFEAT));

  public LettuceMapCoinDataModel GetLastCompletedCoinDataModel() => this.m_lettuceCoinDataModels.OrderByDescending<LettuceMapCoinDataModel, int>((Func<LettuceMapCoinDataModel, int>) (c => c.Id)).FirstOrDefault<LettuceMapCoinDataModel>((Func<LettuceMapCoinDataModel, bool>) (c => c.CoinState == LettuceMapNode.NodeState.COMPLETE));

  public bool IsFinalBossDefeated() => this.m_isFinalBossDefeated;

  public void FlipUnlockedCoins()
  {
    foreach (KeyValuePair<LettuceMapCoinDataModel, LettuceMapCoin> keyValuePair in this.m_lettuceCoinsByDataModel)
    {
      LettuceMapCoinDataModel key = keyValuePair.Key;
      if (key.CoinState == LettuceMapNode.NodeState.UNLOCKED)
        key.CoinData.MissionState = AdventureMissionState.UNLOCKED;
    }
  }

  public Vector3 GetWorldSpacePositionOfCoin(int coinId)
  {
    LettuceMapCoinDataModel mapCoinFromCoinId = this.GetLettuceMapCoinFromCoinId(coinId);
    return mapCoinFromCoinId == null ? Vector3.zero : this.m_lettuceCoinsByDataModel[mapCoinFromCoinId].transform.position;
  }

  private LettuceMapCoinDataModel GetLettuceMapCoinFromCoinId(int coinId)
  {
    foreach (LettuceMapCoinDataModel key in this.m_lettuceCoinsByDataModel.Keys)
    {
      if (key.Id == coinId)
        return key;
    }
    Debug.LogError((object) ("LettuceMap.GetLettuceMapCoinFromCoinId() - No coin found with id=" + (object) coinId));
    return (LettuceMapCoinDataModel) null;
  }

  private void OnAllCoinsLoaded()
  {
    this.PositionCoinsInRow();
    this.DrawCoinConnectionLines();
    this.UpdateCoinGlowLines();
  }

  private void PositionCoinsInRow()
  {
    LettuceMapDataModel lettuceMapDataModel = this.GetLettuceMapDataModel();
    float x1 = this.MapBoundsLeftBone.position.x;
    float num1 = this.MapBoundsRightBone.position.x - x1;
    UnityEngine.Random.InitState(this.m_seed);
    for (int index1 = 0; index1 < lettuceMapDataModel.Rows.Count; ++index1)
    {
      LettuceMapRowDataModel row = lettuceMapDataModel.Rows[index1];
      if (index1 == 0 || index1 >= lettuceMapDataModel.Rows.Count - 2)
      {
        float x2 = x1 + num1 / 2f;
        LettuceMapCoin lettuceMapCoin = this.m_lettuceCoinsByDataModel[row.Coins.First<LettuceMapCoinDataModel>()];
        lettuceMapCoin.transform.position = new Vector3(x2, lettuceMapCoin.transform.position.y, lettuceMapCoin.transform.position.z);
      }
      else
      {
        int count = row.Coins.Count;
        for (int index2 = 0; index2 < count; ++index2)
        {
          LettuceMapCoin lettuceMapCoin = this.m_lettuceCoinsByDataModel[row.Coins[index2]];
          float num2 = num1 / (float) count;
          float min = (float) ((double) x1 + (double) num2 * (double) index2 + 7.5);
          float max = (float) ((double) x1 + (double) num2 * (double) (index2 + 1) - 7.5);
          float x3 = !this.EnableRandomCoinPositionsInRow ? (float) (((double) min + (double) max) / 2.0) : UnityEngine.Random.Range(min, max);
          lettuceMapCoin.transform.position = new Vector3(x3, lettuceMapCoin.transform.position.y, lettuceMapCoin.transform.position.z);
        }
      }
    }
  }

  private void DrawCoinConnectionLines()
  {
    foreach (LettuceMapCoinDataModel key in this.m_lettuceCoinsByDataModel.Keys)
    {
      LettuceMapCoin lettuceMapCoin1 = this.m_lettuceCoinsByDataModel[key];
      for (int index1 = 0; index1 < key.NeighborIds.Count; ++index1)
      {
        LettuceMapCoinDataModel mapCoinFromCoinId = this.GetLettuceMapCoinFromCoinId(key.NeighborIds[index1]);
        if (mapCoinFromCoinId != null)
        {
          LettuceMapCoin lettuceMapCoin2 = this.m_lettuceCoinsByDataModel[mapCoinFromCoinId];
          int numConnectionsComingFromLeft = 0;
          int numConnectionsComingFromRight = 0;
          int currentConnectionIndex = 0;
          for (int index2 = 0; index2 < mapCoinFromCoinId.ParentIds.Count; ++index2)
          {
            int parentId = mapCoinFromCoinId.ParentIds[index2];
            LettuceMapCoin lettuceMapCoin3 = this.m_lettuceCoinsByNodeId[parentId];
            if (parentId == key.Id)
              currentConnectionIndex = index2;
            if ((double) lettuceMapCoin3.transform.position.x < (double) lettuceMapCoin2.transform.position.x)
              ++numConnectionsComingFromLeft;
            else
              ++numConnectionsComingFromRight;
          }
          lettuceMapCoin1.DrawLineToObjectOnNextRow(lettuceMapCoin2.gameObject, currentConnectionIndex, numConnectionsComingFromLeft, numConnectionsComingFromRight);
        }
      }
      if (GameUtils.IsFinalBossNodeType(key.NodeTypeId))
        lettuceMapCoin1.DrawLineToObjectOnSameRow(this.m_finalBossChest);
    }
    this.m_allLinesDrawn = true;
  }

  private AdventureMissionState GetAdventureMissionStateFromNode(
    LettuceMapNode node)
  {
    switch (node.NodeState_)
    {
      case LettuceMapNode.NodeState.LOCKED:
        return GameUtils.IsFinalBossNodeType((int) node.NodeTypeId) ? AdventureMissionState.UNLOCKED : AdventureMissionState.LOCKED;
      case LettuceMapNode.NodeState.UNLOCKED:
        return AdventureMissionState.LOCKED;
      case LettuceMapNode.NodeState.COMPLETE:
        return AdventureMissionState.COMPLETED;
      case LettuceMapNode.NodeState.BLOCKED:
        return AdventureMissionState.LOCKED;
      case LettuceMapNode.NodeState.DEFEAT:
        return AdventureMissionState.UNLOCKED;
      default:
        Log.Lettuce.PrintError("Unable to get AdventureMissionState for node state: {0}", (object) node.NodeState_);
        return AdventureMissionState.LOCKED;
    }
  }

  private AdventureMissionDataModel GetCoinDataForNode(LettuceMapNode node)
  {
    Material bossCoinMaterial = (Material) null;
    if (node.HasBossCard && node.BossCard.Asset != 0)
    {
      DefLoader.Get().LoadCardDef(GameUtils.TranslateDbIdToCardId(node.BossCard.Asset), (DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>) ((cardId, def, userData) =>
      {
        if (def == null)
          return;
        bossCoinMaterial = def.CardDef.m_MercenaryMapBossCoinPortrait;
        this.m_loadedCoinCardDefs.Add(def);
      }));
      if ((UnityEngine.Object) bossCoinMaterial == (UnityEngine.Object) null)
        bossCoinMaterial = AssetLoader.Get().LoadMaterial((AssetReference) "LOE_08CoinPortrait.mat:b5cdfac2e9672f9479083d73014858c6");
    }
    return new AdventureMissionDataModel()
    {
      CoinPortraitMaterial = bossCoinMaterial,
      MissionState = this.GetAdventureMissionStateFromNode(node)
    };
  }

  private void UpdateCoinGlowLines()
  {
    foreach (LettuceMapCoinDataModel key in this.m_lettuceCoinsByDataModel.Keys)
      key.LineGlowVisible = false;
    List<LettuceMapCoin> completedCoins = this.GetCompletedCoins();
    if (completedCoins == null || completedCoins.Count == 0)
      return;
    LettuceMapCoinDataModel mapCoinDataModel = completedCoins.Last<LettuceMapCoin>()?.GetMapCoinDataModel();
    if (mapCoinDataModel == null)
      return;
    mapCoinDataModel.LineGlowVisible = true;
  }

  private void GetTooltipStringsForNodeType(
    int nodeTypeId,
    TAG_ROLE nodeRole,
    out string headerString,
    out string bodyString)
  {
    LettuceMapNodeTypeDbfRecord record = GameDbf.LettuceMapNodeType.GetRecord(nodeTypeId);
    if (nodeTypeId <= 0 || record == null)
    {
      headerString = GameStrings.Get("GLUE_LETTUCE_MAP_MYSTERY_TOOLTIP_HEADER");
      bodyString = GameStrings.Get("GLUE_LETTUCE_MAP_MYSTERY_TOOLTIP_BODY");
    }
    else
    {
      bodyString = (string) record.HoverTooltipBody;
      if (nodeRole != TAG_ROLE.INVALID)
      {
        if (record.BossType == LettuceMapNodeType.LettuceMapBossType.NORMAL_BOSS || record.BossType == LettuceMapNodeType.LettuceMapBossType.SIMPLE_BOSS)
        {
          headerString = GameStrings.Format("GLUE_LETTUCE_MAP_BOSS_FIGHT_TOOLTIP_HEADER", (object) GameStrings.GetRoleName(nodeRole));
          return;
        }
        if (record.BossType == LettuceMapNodeType.LettuceMapBossType.ELITE_BOSS)
        {
          headerString = GameStrings.Format("GLUE_LETTUCE_MAP_ELITE_BOSS_FIGHT_TOOLTIP_HEADER", (object) GameStrings.GetRoleName(nodeRole));
          return;
        }
      }
      headerString = (string) record.HoverTooltipHeader;
    }
  }

  private LettuceMapNodeType.LettuceMapBossType GetBossTypeForNodeType(
    int nodeTypeId)
  {
    LettuceMapNodeTypeDbfRecord record = GameDbf.LettuceMapNodeType.GetRecord(nodeTypeId);
    return record == null ? LettuceMapNodeType.LettuceMapBossType.NONE : record.BossType;
  }

  private void SetGrantedAnomalyCardFromMapData(
    PegasusLettuce.LettuceMap mapData,
    int nodeId,
    LettuceMapCoinDataModel coinDataModel)
  {
    foreach (LettuceMapAnomalyAssignment anomalyCard in mapData.AnomalyCards)
    {
      if (anomalyCard.SourceNodeId == nodeId)
        coinDataModel.GrantedAnomalyCard = new CardDataModel()
        {
          CardId = GameUtils.TranslateDbIdToCardId(anomalyCard.AnomalyCard),
          Premium = TAG_PREMIUM.NORMAL
        };
    }
  }
}
