using Hearthstone;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SmartDiscoverDebugManager : MonoBehaviour
{
  private static SmartDiscoverDebugManager s_instance;
  private Regex m_fileOpenRegex = new Regex("beginsmartdiscoverreport");
  private Regex m_beginRegex = new Regex("beginsmartdiscovertest (?<testName>.+)");
  private Regex m_descriptionRegex = new Regex("smartdiscovertestdescription (?<testString>.+)");
  private Regex m_testExpectsOneResultRegex = new Regex("smartdiscovertestexpectresult (?<cardId1>[^\\s]+)");
  private Regex m_testExpectsTwoResultsRegex = new Regex("smartdiscovertestexpectresult (?<cardId1>[^\\s]+) (?<cardId2>[^\\s]+)");
  private Regex m_testExpectsThreeResultsRegex = new Regex("smartdiscovertestexpectresult (?<cardId1>[^\\s]+) (?<cardId2>[^\\s]+) (?<cardId3>[^\\s]+)");
  private Regex m_endRegex = new Regex("endsmartdiscovertest");
  private List<string> m_expectedResults = new List<string>();
  private string m_currentTestName = "";

  public static SmartDiscoverDebugManager Get()
  {
    if ((Object) SmartDiscoverDebugManager.s_instance == (Object) null)
    {
      GameObject gameObject = new GameObject();
      SmartDiscoverDebugManager.s_instance = gameObject.AddComponent<SmartDiscoverDebugManager>();
      gameObject.name = "SmartDiscoverDebugManager (Dynamically created)";
    }
    return SmartDiscoverDebugManager.s_instance;
  }

  public bool RequiresWaiting(string line) => this.m_endRegex.Match(line).Success;

  public bool PreprocessCommand(string line)
  {
    if (!this.m_endRegex.Match(line).Success)
      return false;
    Network.Get().SendDebugConsoleCommand("spawncard XXX_56633 friendly play 0");
    return true;
  }

  public bool ParseCheatCommand(string line)
  {
    if (this.m_fileOpenRegex.Match(line).Success)
    {
      Log.SmartDiscover.PurgeFile();
      return true;
    }
    Match match1 = this.m_beginRegex.Match(line);
    if (match1.Success)
    {
      Network.Get().SendDebugConsoleCommand("settag 1324 1 0");
      GameState.Get().GetGameEntity().SetTag(GAME_TAG.SMART_DISCOVER_DEBUG_TEST_COMPLETE, 0);
      Log.SmartDiscover.ForceFilePrint(Blizzard.T5.Logging.LogLevel.Info, string.Format("Begin Smart Discover Test: {0}", (object) match1.Groups[1].Value));
      this.m_currentTestName = match1.Groups[1].Value;
      this.m_expectedResults.Clear();
      return true;
    }
    Match match2 = this.m_descriptionRegex.Match(line);
    if (match2.Success)
    {
      Log.SmartDiscover.ForceFilePrint(Blizzard.T5.Logging.LogLevel.Info, match2.Groups[1].Value);
      return true;
    }
    Match match3 = this.m_testExpectsThreeResultsRegex.Match(line);
    if (match3.Success)
    {
      this.ParseExpectedResultsCommand(match3, 3);
      return true;
    }
    Match match4 = this.m_testExpectsTwoResultsRegex.Match(line);
    if (match4.Success)
    {
      this.ParseExpectedResultsCommand(match4, 2);
      return true;
    }
    Match match5 = this.m_testExpectsOneResultRegex.Match(line);
    if (match5.Success)
    {
      this.ParseExpectedResultsCommand(match5, 1);
      return true;
    }
    Match match6 = this.m_endRegex.Match(line);
    if (!match6.Success)
      return false;
    this.ParseEndCommand(match6);
    return true;
  }

  private void ParseExpectedResultsCommand(Match match, int expectedResultsCount)
  {
    this.m_expectedResults.Clear();
    List<string> stringList = new List<string>();
    string message = "Expected results:";
    for (int groupnum = 1; groupnum <= expectedResultsCount; ++groupnum)
    {
      string cardId = match.Groups[groupnum].Value;
      this.m_expectedResults.Add(cardId);
      EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
      if (entityDef != null)
        stringList.Add(entityDef.GetName());
      else
        stringList.Add(string.Format("UNRECOGNIZED CARD ID: {0}", (object) cardId));
      message = string.Format("{0} {1}", (object) message, (object) stringList[stringList.Count - 1]);
    }
    Log.SmartDiscover.ForceFilePrint(Blizzard.T5.Logging.LogLevel.Info, message);
  }

  private void ParseEndCommand(Match match)
  {
    bool flag = true;
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    string str1 = "CHOICE_1_INVALID";
    string str2 = "CHOICE_2_INVALID";
    string str3 = "CHOICE_3_INVALID";
    EntityDef entityDef1 = DefLoader.Get().GetEntityDef(friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_1), false);
    if (entityDef1 != null)
      str1 = entityDef1.GetName();
    EntityDef entityDef2 = DefLoader.Get().GetEntityDef(friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_2), false);
    if (entityDef2 != null)
      str2 = entityDef2.GetName();
    EntityDef entityDef3 = DefLoader.Get().GetEntityDef(friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_3), false);
    if (entityDef3 != null)
      str3 = entityDef3.GetName();
    Log.SmartDiscover.ForceFilePrint(Blizzard.T5.Logging.LogLevel.Info, string.Format("Received results: {0}, {1}, {2}", (object) str1, (object) str2, (object) str3));
    foreach (string expectedResult in this.m_expectedResults)
    {
      int dbId = GameUtils.TranslateCardIdToDbId(expectedResult);
      if (dbId == 0)
      {
        flag = false;
        break;
      }
      if (dbId != friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_1) && dbId != friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_2) && dbId != friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_3))
      {
        flag = false;
        break;
      }
    }
    Log.SmartDiscover.ForceFilePrint(Blizzard.T5.Logging.LogLevel.Info, string.Format("Test {0} {1}\n", (object) this.m_currentTestName, flag ? (object) "passed" : (object) "FAILED"));
  }

  private void Update()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    Player friendlySidePlayer = gameState.GetFriendlySidePlayer();
    if (friendlySidePlayer == null)
      return;
    int tag1 = friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_1);
    if (tag1 != 0)
    {
      EntityDef entityDef1 = DefLoader.Get().GetEntityDef(tag1);
      string str1 = "Unknown";
      if (entityDef1 != null)
        str1 = entityDef1.GetName();
      int tag2 = friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_2);
      EntityDef entityDef2 = DefLoader.Get().GetEntityDef(tag2);
      string str2 = "Unknown";
      if (entityDef2 != null)
        str2 = entityDef2.GetName();
      int tag3 = friendlySidePlayer.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_ENTITY_3);
      EntityDef entityDef3 = DefLoader.Get().GetEntityDef(tag3);
      string str3 = "Unknown";
      if (entityDef3 != null)
        str3 = entityDef3.GetName();
      string text = string.Format("Results:\n1. {0}\n2. {1}\n3. {2}", (object) str1, (object) str2, (object) str3);
      Vector3 position = new Vector3((float) Screen.width, (float) Screen.height, 0.0f);
      DebugTextManager.Get().DrawDebugText(text, position, 0.0f, true);
    }
    string text1 = this.GetStringForPassiveResults(friendlySidePlayer);
    if (text1 == "")
    {
      text1 = this.GetStringForPassiveResults(gameState.GetOpposingSidePlayer());
    }
    else
    {
      string forPassiveResults = this.GetStringForPassiveResults(gameState.GetOpposingSidePlayer());
      if (forPassiveResults != "")
        text1 = string.Format("{0}\n\n{1}", (object) text1, (object) forPassiveResults);
    }
    if (!(text1 != ""))
      return;
    Vector3 position1 = new Vector3((float) Screen.width, 0.0f, 0.0f);
    DebugTextManager.Get().DrawDebugText(text1, position1, 0.0f, true);
  }

  private string GetStringForPassiveResults(Player player)
  {
    if (GameState.Get() == null || player == null)
      return "";
    int tag1 = player.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_PASSIVE_EVAL_RESULT_1);
    if (tag1 == 0)
      return "";
    EntityDef entityDef1 = DefLoader.Get().GetEntityDef(tag1);
    string str1 = "Unknown";
    if (entityDef1 != null)
      str1 = entityDef1.GetName();
    int tag2 = player.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_PASSIVE_EVAL_RESULT_2);
    EntityDef entityDef2 = DefLoader.Get().GetEntityDef(tag2);
    string str2 = "Unknown";
    if (entityDef2 != null)
      str2 = entityDef2.GetName();
    int tag3 = player.GetTag(GAME_TAG.SMART_DISCOVER_DEBUG_PASSIVE_EVAL_RESULT_3);
    EntityDef entityDef3 = DefLoader.Get().GetEntityDef(tag3);
    string str3 = "Unknown";
    if (entityDef3 != null)
      str3 = entityDef3.GetName();
    return string.Format("Passive Results for {0}:\n1. {1}\n2. {2}\n3. {3}", (object) player.GetName(), (object) str1, (object) str2, (object) str3);
  }
}
