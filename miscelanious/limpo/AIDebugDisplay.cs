using Hearthstone;
using PegasusGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIDebugDisplay : MonoBehaviour
{
  private static AIDebugDisplay s_instance;
  private List<List<List<AIDebugInformation>>> m_debugInformation = new List<List<List<AIDebugInformation>>>();
  public bool m_isDisplayed;
  private float m_currentHistoryScrollBarValue = 1f;
  private float m_currentIterationScrollBarValue = 1f;
  private float m_currentDepthScrollBarValue;
  private bool m_showIterationScrollBar;
  private bool m_showDepthScrollBar;

  public static AIDebugDisplay Get()
  {
    if ((UnityEngine.Object) AIDebugDisplay.s_instance == (UnityEngine.Object) null)
    {
      GameObject gameObject = new GameObject();
      AIDebugDisplay.s_instance = gameObject.AddComponent<AIDebugDisplay>();
      gameObject.name = "AIDebugDisplay (Dynamically created)";
    }
    return AIDebugDisplay.s_instance;
  }

  private void Start()
  {
    if (HearthstoneApplication.IsPublic() || GameState.Get() == null)
      return;
    GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent), (object) null);
  }

  private void GameState_CreateGameEvent(GameState.CreateGamePhase createGamePhase, object userData) => this.m_debugInformation.Clear();

  public bool ToggleDebugDisplay(string func, string[] args, string rawArgs)
  {
    this.m_isDisplayed = !this.m_isDisplayed;
    return true;
  }

  private void Update()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    GameState gameState = GameState.Get();
    if (gameState == null || !this.m_isDisplayed)
      return;
    AIDebugInformation debugInfo = (AIDebugInformation) null;
    if (!gameState.IsFriendlySidePlayerTurn())
    {
      int currentTurn = gameState.GetGameEntity().GetTag(GAME_TAG.TURN);
      int moveID = gameState.GetOpposingSidePlayer().GetTag(GAME_TAG.NUM_OPTIONS_PLAYED_THIS_TURN) + 1;
      List<List<AIDebugInformation>> debugInformationListList = this.m_debugInformation.Find((Predicate<List<List<AIDebugInformation>>>) (x => x.Count > 0 && x[0].Count > 0 && x[0][0].MoveID == moveID && x[0][0].TurnID == currentTurn));
      if (debugInformationListList != null)
      {
        List<AIDebugInformation> debugInformationList = debugInformationListList[debugInformationListList.Count - 1];
        if (debugInformationList[0].DebugIteration == 0)
          debugInfo = debugInformationList[0];
      }
      this.m_currentHistoryScrollBarValue = 1f;
      this.m_currentIterationScrollBarValue = 1f;
      this.m_currentDepthScrollBarValue = 0.0f;
      this.m_showIterationScrollBar = this.m_showDepthScrollBar = debugInformationListList != null && debugInformationListList.Count > 1;
    }
    else if (this.m_debugInformation.Count > 0)
    {
      int index1 = (int) ((double) this.m_currentHistoryScrollBarValue * (double) this.m_debugInformation.Count);
      if (index1 >= this.m_debugInformation.Count)
        index1 = this.m_debugInformation.Count - 1;
      List<List<AIDebugInformation>> debugInformationListList = this.m_debugInformation[index1];
      this.m_showIterationScrollBar = this.m_showDepthScrollBar = debugInformationListList != null && debugInformationListList.Count > 1;
      int index2 = (int) ((double) this.m_currentIterationScrollBarValue * (double) debugInformationListList.Count);
      if (index2 >= debugInformationListList.Count)
        index2 = debugInformationListList.Count - 1;
      List<AIDebugInformation> debugInformationList = debugInformationListList[index2];
      int index3 = (int) ((double) this.m_currentDepthScrollBarValue * (double) debugInformationList.Count);
      if (index3 >= debugInformationList.Count)
        index3 = debugInformationList.Count - 1;
      debugInfo = debugInformationList[index3];
    }
    if (debugInfo == null && this.m_debugInformation.Count > 0)
    {
      List<List<AIDebugInformation>> last = this.m_debugInformation.FindLast((Predicate<List<List<AIDebugInformation>>>) (x => x.Count > 0 && x[x.Count - 1].Count > 0 && x[x.Count - 1][0].DebugIteration == 0));
      if (last != null)
      {
        debugInfo = last[last.Count - 1][0];
        this.m_showIterationScrollBar = this.m_showDepthScrollBar = last != null && last.Count > 1;
      }
    }
    this.UpdateDisplay(debugInfo);
  }

  private string AppendLine(string inputString, string stringToAppend) => string.Format("{0}\n{1}", (object) inputString, (object) stringToAppend);

  private string FormatOptionName(AIEvaluation evaluation)
  {
    string str = !evaluation.OptionChosen ? string.Format("{0} (ID{1})", (object) evaluation.OptionName, (object) evaluation.EntityID) : string.Format("AI CHOSE: {0} (ID{1})", (object) evaluation.OptionName, (object) evaluation.EntityID);
    if (evaluation.TargetScores.Count >= 1)
    {
      AITarget aiTarget = evaluation.TargetScores.Find((Predicate<AITarget>) (x => x.TargetChosen));
      if (aiTarget != null)
        str = string.Format("{0} targeting {1} (ID{2})", (object) str, (object) aiTarget.EntityName, (object) aiTarget.EntityID);
    }
    return str;
  }

  private int GetOverallScore(AIEvaluation evaluation) => evaluation.BaseScore + evaluation.BonusScore + evaluation.ContextualScore.Sum<AIContextualValue>((Func<AIContextualValue, int>) (x => x.ContextualScore)) + evaluation.EdgeCount;

  private void UpdateDisplay(AIDebugInformation debugInfo)
  {
    string text = "";
    Vector3 position = new Vector3((float) Screen.width, (float) Screen.height, 0.0f);
    if (GameState.Get() != null && GameState.Get().GetGameEntity() != null)
      text = string.Format("Uuid: {0}\n", (object) GameState.Get().GetGameEntity().Uuid);
    if (debugInfo == null)
    {
      DebugTextManager.Get().DrawDebugText(text, position, 0.0f, true);
    }
    else
    {
      if (debugInfo.ModelVersion != 0L)
        text += string.Format("Model Version: {0}\n", (object) debugInfo.ModelVersion);
      string str1 = text + string.Format("AI Debug Turn {0}, Move {1}", (object) debugInfo.TurnID, (object) debugInfo.MoveID);
      string stringToAppend1 = "";
      if (debugInfo.DebugIteration != 0)
        stringToAppend1 = stringToAppend1 + "Iteration: " + (object) debugInfo.DebugIteration;
      if (debugInfo.DebugDepth != 0)
        stringToAppend1 = stringToAppend1 + " Depth: " + (object) debugInfo.DebugDepth;
      if (!string.IsNullOrEmpty(stringToAppend1))
        str1 = this.AppendLine(str1, stringToAppend1);
      string stringToAppend2 = "";
      if ((double) debugInfo.InferenceValue != 0.0)
        stringToAppend2 = stringToAppend2 + "Inference: " + debugInfo.InferenceValue.ToString(".000");
      if ((double) debugInfo.HeuristicValue != 0.0)
        stringToAppend2 = stringToAppend2 + " Heuristic: " + debugInfo.HeuristicValue.ToString(".000");
      if ((double) debugInfo.SubtreeValue != 0.0)
        stringToAppend2 = stringToAppend2 + " Subtree: " + debugInfo.SubtreeValue.ToString(".000");
      if (!string.IsNullOrEmpty(stringToAppend2))
        str1 = this.AppendLine(str1, stringToAppend2);
      if (debugInfo.TotalVisits > 0)
        str1 = this.AppendLine(str1, "Total Visits: " + (object) debugInfo.TotalVisits);
      if (debugInfo.UniqueNodes > 0)
        str1 = this.AppendLine(str1, "Unique Nodes: " + (object) debugInfo.UniqueNodes);
      if (debugInfo.SubtreeDepth > 0)
        str1 = this.AppendLine(str1, "SubTree Depth: " + (object) debugInfo.SubtreeDepth);
      List<AIEvaluation> aiEvaluationList = new List<AIEvaluation>();
      aiEvaluationList.AddRange((IEnumerable<AIEvaluation>) debugInfo.Evaluations);
      debugInfo.Evaluations = debugInfo.Evaluations.OrderByDescending<AIEvaluation, int>((Func<AIEvaluation, int>) (x => x.OptionChosen ? 9999999 : this.GetOverallScore(x))).ToList<AIEvaluation>();
      foreach (AIEvaluation evaluation in aiEvaluationList)
      {
        str1 = this.AppendLine(str1, "---");
        str1 = this.AppendLine(str1, this.FormatOptionName(evaluation));
        if (evaluation.BaseScore > 0)
          str1 = this.AppendLine(str1, "Total Option Score: " + (object) this.GetOverallScore(evaluation));
        int num = 0;
        foreach (AIContextualValue aiContextualValue in evaluation.ContextualScore)
          num += aiContextualValue.ContextualScore;
        if (evaluation.BonusScore != 0 || num != 0)
          str1 = this.AppendLine(str1, "Base Score: " + (object) evaluation.BaseScore);
        if (evaluation.BonusScore != 0)
          str1 = this.AppendLine(str1, "Bonus Score: " + (object) evaluation.BonusScore);
        if (evaluation.ContextualScore.Count > 0)
        {
          str1 = this.AppendLine(str1, "Contextual Score from: ");
          foreach (AIContextualValue aiContextualValue in evaluation.ContextualScore)
            str1 = this.AppendLine(str1, string.Format("{0} (ID{1}): {2}", (object) aiContextualValue.EntityName, (object) aiContextualValue.EntityID, (object) aiContextualValue.ContextualScore));
        }
        if ((double) evaluation.PriorProbability != 0.0)
          str1 = this.AppendLine(str1, "Prior Probability: " + evaluation.PriorProbability.ToString(".000"));
        if ((double) evaluation.PuctValue != 0.0 && debugInfo.TotalVisits > 1)
          str1 = this.AppendLine(str1, "Puct Value: " + evaluation.PuctValue.ToString(".000"));
        if (evaluation.FinalVisitCount > 0)
          str1 = this.AppendLine(str1, "Visit Count: " + (object) evaluation.FinalVisitCount + " (" + (object) evaluation.EdgeCount + ")");
        if (evaluation.SubtreeDepth > 0)
          str1 = this.AppendLine(str1, "Subtree Depth: " + (object) evaluation.SubtreeDepth);
        if ((double) evaluation.FinalQValue != 0.0)
          str1 = this.AppendLine(str1, "Q Value: " + evaluation.FinalQValue.ToString(".000"));
        string stringToAppend3 = "";
        if ((double) evaluation.InferenceValue != 0.0)
          stringToAppend3 = stringToAppend3 + "Inference: " + evaluation.InferenceValue.ToString(".000");
        if ((double) evaluation.HeuristicValue != 0.0)
          stringToAppend3 = stringToAppend3 + " Heuristic: " + evaluation.HeuristicValue.ToString(".000");
        if ((double) evaluation.SubtreeValue != 0.0)
          stringToAppend3 = stringToAppend3 + " Subtree: " + evaluation.SubtreeValue.ToString(".000");
        if (!string.IsNullOrEmpty(stringToAppend3))
          str1 = this.AppendLine(str1, stringToAppend3);
        if (evaluation.TargetScores.Count >= 1)
        {
          str1 = this.AppendLine(str1, "Target scores: ");
          foreach (AITarget targetScore in evaluation.TargetScores)
          {
            if (targetScore.TargetScore > 0)
              str1 = this.AppendLine(str1, string.Format("{0} (ID{1}): {2}", (object) targetScore.EntityName, (object) targetScore.EntityID, (object) targetScore.TargetScore));
            else if ((double) targetScore.PriorProbability > 0.0)
            {
              string str2 = "";
              if ((double) targetScore.InferenceValue != 0.0)
                str2 = str2 + ", Inf: " + targetScore.InferenceValue.ToString(".000");
              if ((double) targetScore.HeuristicValue != 0.0)
                str2 = str2 + ", Heur: " + targetScore.HeuristicValue.ToString(".000");
              if ((double) targetScore.SubtreeValue != 0.0)
                str2 = str2 + ", Sub: " + targetScore.SubtreeValue.ToString(".000");
              string str3 = "";
              if (debugInfo.TotalVisits > 1)
                str3 = string.Format(", PUCT {0:.000}, Visit {1} ({2}), Value {3:.000}{4}", (object) targetScore.PuctValue, (object) targetScore.FinalVisitCount, (object) targetScore.EdgeCount, (object) targetScore.FinalQValue, (object) str2);
              str1 = this.AppendLine(str1, string.Format("{0} (ID{1}): Prior {2:.000}{3}", (object) targetScore.EntityName, (object) targetScore.EntityID, (object) targetScore.PriorProbability, (object) str3));
            }
          }
        }
        if (evaluation.PositionScores.Count >= 2)
        {
          str1 = this.AppendLine(str1, "Position scores: ");
          foreach (AIPosition positionScore in evaluation.PositionScores)
          {
            if ((double) positionScore.PriorProbability > 0.0)
            {
              string str4 = "";
              if ((double) positionScore.InferenceValue != 0.0)
                str4 = str4 + ", Inf: " + positionScore.InferenceValue.ToString(".000");
              if ((double) positionScore.HeuristicValue != 0.0)
                str4 = str4 + ", Heur: " + positionScore.HeuristicValue.ToString(".000");
              if ((double) positionScore.SubtreeValue != 0.0)
                str4 = str4 + ", Sub: " + positionScore.SubtreeValue.ToString(".000");
              str1 = this.AppendLine(str1, string.Format("Pos {0}: Prior {1:.000}, PUCT {2:.000}, Visit {3} ({4}), Value {5:.000}{6}", (object) (positionScore.Position > 0 ? positionScore.Position : evaluation.PositionScores.Count), (object) positionScore.PriorProbability, (object) positionScore.PuctValue, (object) positionScore.FinalVisitCount, (object) positionScore.EdgeCount, (object) positionScore.FinalQValue, (object) str4));
            }
          }
        }
      }
      DebugTextManager.Get().DrawDebugText(str1, position, 0.0f, true);
    }
  }

  public void OnAIDebugInformation(AIDebugInformation debugInfo)
  {
    int index1 = this.m_debugInformation.FindIndex((Predicate<List<List<AIDebugInformation>>>) (x => x.Count > 0 && x[0].Count > 0 && x[0][0].MoveID == debugInfo.MoveID && x[0][0].TurnID == debugInfo.TurnID));
    if (index1 == -1)
    {
      index1 = this.m_debugInformation.Count;
      this.m_debugInformation.Add(new List<List<AIDebugInformation>>());
    }
    int index2 = this.m_debugInformation[index1].FindIndex((Predicate<List<AIDebugInformation>>) (x => x.Count > 0 && x[0].DebugIteration == debugInfo.DebugIteration));
    if (index2 == -1)
    {
      index2 = this.m_debugInformation[index1].Count;
      this.m_debugInformation[index1].Add(new List<AIDebugInformation>());
    }
    int index3 = this.m_debugInformation[index1][index2].FindIndex((Predicate<AIDebugInformation>) (x => x.DebugDepth == debugInfo.DebugDepth));
    if (index3 == -1)
      this.m_debugInformation[index1][index2].Add(debugInfo);
    else
      this.m_debugInformation[index1][index2][index3] = debugInfo;
  }
}
