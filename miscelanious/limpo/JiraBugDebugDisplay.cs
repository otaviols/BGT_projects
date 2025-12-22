using Blizzard.T5.AssetManager;
using Hearthstone;
using Hearthstone.Core;
using MiniJSON;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class JiraBugDebugDisplay : MonoBehaviour
{
  private const string s_loadingText = "Loading...";
  private const string s_jiraUrl = "https://jira.blizzard.com/";
  private const string s_searchQuery = "summary ~ \"{0}\" and status != closed and issuetype=Bug";
  private const string s_jiraAuth = "";
  private static JiraBugDebugDisplay s_instance = (JiraBugDebugDisplay) null;
  private static readonly AssetReference s_backgroundTexture = new AssetReference("tilable_background_grey_vertical.tif:2069edef921936f4db7eaeb542bcf5f1");
  private ConcurrentDictionary<string, string> m_bugcache = new ConcurrentDictionary<string, string>();
  private int m_remoteRequestCount;
  private string m_currentCard = "";
  private bool m_isEnabled;
  private GUIStyle m_debugTextStyle;
  private AssetHandle<Texture> m_loadedBackgroundTexture;

  public static JiraBugDebugDisplay Get()
  {
    if ((Object) JiraBugDebugDisplay.s_instance == (Object) null)
    {
      GameObject gameObject = new GameObject();
      JiraBugDebugDisplay.s_instance = gameObject.AddComponent<JiraBugDebugDisplay>();
      gameObject.name = "JIRABugDebugDisplay (Dynamically created)";
      AssetLoader.Get().LoadAsset<Texture>(JiraBugDebugDisplay.s_backgroundTexture, new AssetHandleCallback<Texture>(JiraBugDebugDisplay.s_instance.OnTextureLoad));
      JiraBugDebugDisplay.s_instance.m_debugTextStyle = new GUIStyle((GUIStyle) "box");
      JiraBugDebugDisplay.s_instance.m_debugTextStyle.fontSize = 16;
      JiraBugDebugDisplay.s_instance.m_debugTextStyle.normal.textColor = Color.white;
      JiraBugDebugDisplay.s_instance.m_debugTextStyle.alignment = TextAnchor.MiddleLeft;
    }
    return JiraBugDebugDisplay.s_instance;
  }

  private void OnTextureLoad(
    AssetReference assetRef,
    AssetHandle<Texture> loadedTexture,
    object callbackData)
  {
    AssetHandle.Take<Texture>(ref this.m_loadedBackgroundTexture, loadedTexture);
    JiraBugDebugDisplay.s_instance.m_debugTextStyle.normal.background = (Texture2D) this.m_loadedBackgroundTexture.Asset;
  }

  private void OnDestroy() => AssetHandle.SafeDispose<Texture>(ref this.m_loadedBackgroundTexture);

  private void LoadBugsInBrowser() => Application.OpenURL(this.GetSearchURL(this.m_currentCard, false));

  private bool GetBugsForCard(string cardid, out string bugs)
  {
    this.m_bugcache.TryGetValue(cardid, out bugs);
    if (string.IsNullOrWhiteSpace(bugs))
    {
      bugs = "No Issues Found";
      return false;
    }
    return !(bugs == "Loading...");
  }

  private string GetSearchURL(string search, bool useApiEndpoint = true) => (!useApiEndpoint ? "https://jira.blizzard.com/issues/?jql=" : "https://jira.blizzard.com/rest/api/2/search/?jql=") + UnityWebRequest.EscapeURL(string.Format("summary ~ \"{0}\" and status != closed and issuetype=Bug", (object) search));

  private IEnumerator SearchJira(string search)
  {
    if (!this.m_bugcache.ContainsKey(search))
    {
      ++this.m_remoteRequestCount;
      this.m_bugcache.TryAdd(search, "Loading...");
      UnityWebRequest request = new UnityWebRequest(this.GetSearchURL(search), "GET");
      request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
      request.SetRequestHeader("Authorization", "");
      request.useHttpContinue = false;
      yield return (object) request.SendWebRequest();
      this.m_bugcache.TryUpdate(search, this.ParseJiraSearchResults(request), "Loading...");
      request = (UnityWebRequest) null;
    }
    yield return (object) null;
  }

  private string ParseJiraSearchResults(UnityWebRequest request)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (Json.Deserialize(request.downloadHandler.text) is JsonNode jsonNode && jsonNode.Count > 0)
    {
      if (!jsonNode.ContainsKey("total"))
        return string.Empty;
      long num = (long) jsonNode["total"];
      if (num == 0L)
        return string.Empty;
      JsonList jsonList = jsonNode["issues"] as JsonList;
      for (int index = 0; (long) index < num; ++index)
      {
        JsonNode jsonNode1 = jsonList[index] as JsonNode;
        string str = jsonNode1["key"] as string;
        JsonNode jsonNode2 = jsonNode1["fields"] as JsonNode;
        stringBuilder.Append(str.PadRight(11));
        stringBuilder.Append(" - ");
        stringBuilder.AppendLine(jsonNode2["summary"] as string);
      }
      --stringBuilder.Length;
    }
    return stringBuilder.ToString();
  }

  public bool EnableDebugDisplay(string func, string[] args, string rawArgs)
  {
    this.m_isEnabled = true;
    return true;
  }

  public bool DisableDebugDisplay(string func, string[] args, string rawArgs)
  {
    this.m_isEnabled = false;
    this.m_bugcache.Clear();
    return true;
  }

  private void Update()
  {
    if (HearthstoneApplication.IsPublic() || !this.m_isEnabled)
      return;
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    gameState.GetEntityMap();
    Card mousedOverCard = InputManager.Get().GetMousedOverCard();
    Entity entity1 = (Entity) null;
    if ((Object) mousedOverCard != (Object) null && mousedOverCard.GetEntity() != null)
      entity1 = mousedOverCard.GetEntity();
    List<Zone> zones = ZoneMgr.Get().GetZones();
    for (int index = 0; index < zones.Count; ++index)
    {
      Zone zone = zones[index];
      if (zone.m_ServerTag == TAG_ZONE.HAND || zone.m_ServerTag == TAG_ZONE.PLAY || zone.m_ServerTag == TAG_ZONE.SECRET)
      {
        foreach (Card card in zone.GetCards())
        {
          Entity entity2 = card.GetEntity();
          if (entity1 == null || entity1 == entity2)
          {
            Vector3 position = card.transform.position;
            if (zone.m_ServerTag == TAG_ZONE.HAND)
            {
              Vector3 vector3 = card.transform.forward;
              if (card.GetControllerSide() == Player.Side.OPPOSING)
              {
                vector3 *= -1.5f;
                if (card.GetController().IsRevealed())
                  vector3 = -vector3;
              }
              position += vector3;
            }
            if (entity1 != null)
            {
              string cardId = card.GetEntity().GetCardId();
              if (string.IsNullOrEmpty(cardId))
                return;
              Processor.RunCoroutine(this.SearchJira(cardId));
              this.SetCurrentCard(cardId);
              this.DrawDebugTextForHighlightedCard(entity2, (Vector3) DebugTextManager.WorldPosToScreenPos(position));
              return;
            }
          }
        }
      }
    }
  }

  private void SetCurrentCard(string cardid) => JiraBugDebugDisplay.s_instance.m_currentCard = cardid;

  private void DrawDebugTextForHighlightedCard(
    Entity ent,
    Vector3 pos,
    bool screenSpace = false,
    bool forceShowZeroTags = false)
  {
    string bugs;
    if (this.GetBugsForCard(ent.GetCardId(), out bugs))
      bugs = "Press ALT+F2 to view in JIRA\n" + bugs;
    DebugTextManager.Get().DrawDebugText(bugs, pos, 0.0f, screenSpace, textStyle: this.m_debugTextStyle);
  }
}
