using Blizzard.T5.Core;
using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugDisplay : MonoBehaviour
{
  private static GameDebugDisplay s_instance;
  private bool m_showEntities;
  private bool m_hideZeroTags;
  private List<GAME_TAG> m_tagsToDisplay = new List<GAME_TAG>();

  public static GameDebugDisplay Get()
  {
    if ((UnityEngine.Object) GameDebugDisplay.s_instance == (UnityEngine.Object) null)
    {
      GameObject gameObject = new GameObject();
      GameDebugDisplay.s_instance = gameObject.AddComponent<GameDebugDisplay>();
      gameObject.name = "GameDebugDisplay (Dynamically created)";
    }
    return GameDebugDisplay.s_instance;
  }

  public bool ToggleEntityCount(string func, string[] args, string rawArgs)
  {
    this.m_showEntities = !this.m_showEntities;
    return true;
  }

  public bool ToggleHideZeroTags(string func, string[] args, string rawArgs)
  {
    this.m_hideZeroTags = !this.m_hideZeroTags;
    return true;
  }

  public bool AddTagToDisplay(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      return false;
    for (int index = 0; index < args.Length; ++index)
    {
      int result = 0;
      if (!int.TryParse(args[index], out result))
      {
        string str = args[index].Trim();
        if (str.Length > 0)
        {
          foreach (int num in Enum.GetValues(typeof (GAME_TAG)))
          {
            if (((GAME_TAG) num).ToString().ToLower().CompareTo(str.ToLower()) == 0)
            {
              result = num;
              break;
            }
          }
        }
      }
      if (result != 0 && !this.m_tagsToDisplay.Contains((GAME_TAG) result))
        this.m_tagsToDisplay.Add((GAME_TAG) result);
    }
    return true;
  }

  public bool RemoveTagToDisplay(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      return false;
    for (int index = 0; index < args.Length; ++index)
    {
      int num = int.Parse(args[index]);
      if (this.m_tagsToDisplay.Contains((GAME_TAG) num))
        this.m_tagsToDisplay.Remove((GAME_TAG) num);
    }
    return true;
  }

  public bool RemoveAllTags(string func, string[] args, string rawArgs)
  {
    this.m_tagsToDisplay.Clear();
    return true;
  }

  private void Update()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    Map<int, Entity> entityMap = gameState.GetEntityMap();
    string str1 = "";
    string str2 = "";
    foreach (KeyValuePair<int, Entity> keyValuePair in entityMap)
    {
      Entity ent = keyValuePair.Value;
      if (ent != null)
      {
        str1 = this.HandleCornerDisplay(ent, GAME_TAG.DEBUG_DISPLAY_TAG_TOP_RIGHT, str1);
        str2 = this.HandleCornerDisplay(ent, GAME_TAG.DEBUG_DISPLAY_TAG_BOTTOM_RIGHT, str2);
      }
    }
    if (str1 != "")
      DebugTextManager.Get().DrawDebugText(str1, new Vector3((float) Screen.width - 150f, (float) Screen.height - 100f, 0.0f), 0.0f, true);
    if (str2 != "")
      DebugTextManager.Get().DrawDebugText(str2, new Vector3((float) Screen.width - 150f, 100f, 0.0f), 0.0f, true);
    if (this.m_showEntities)
    {
      string text = "Entities: " + (object) entityMap.Count;
      DebugTextManager.Get().DrawDebugText(text, new Vector3(100f, 100f, 0.0f), 0.0f, true);
    }
    if (this.m_tagsToDisplay.Count == 0)
      return;
    Card mousedOverCard = InputManager.Get().GetMousedOverCard();
    Entity ent1 = (Entity) null;
    if ((UnityEngine.Object) mousedOverCard != (UnityEngine.Object) null && mousedOverCard.GetEntity() != null)
    {
      ent1 = mousedOverCard.GetEntity();
    }
    else
    {
      RaycastHit hitInfo;
      if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast, out hitInfo))
      {
        GameObject gameObject = hitInfo.collider.gameObject;
        if ((UnityEngine.Object) gameObject.GetComponent<EndTurnButton>() != (UnityEngine.Object) null || (UnityEngine.Object) gameObject.GetComponent<EndTurnButtonReminder>() != (UnityEngine.Object) null)
          ent1 = (Entity) gameState.GetGameEntity();
      }
    }
    List<Zone> zones = ZoneMgr.Get().GetZones();
    for (int index = 0; index < zones.Count; ++index)
    {
      Zone zone = zones[index];
      if (zone.m_ServerTag == TAG_ZONE.HAND || zone.m_ServerTag == TAG_ZONE.PLAY || zone.m_ServerTag == TAG_ZONE.SECRET || zone.m_ServerTag == TAG_ZONE.LETTUCE_ABILITY)
      {
        foreach (Card card in zone.GetCards())
        {
          Entity entity = card.GetEntity();
          if (ent1 == null || ent1 == entity)
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
            else if (zone.m_ServerTag == TAG_ZONE.LETTUCE_ABILITY)
            {
              if (entity.GetLettuceAbilityOwner() == ZoneMgr.Get().GetLettuceAbilitiesSourceEntity())
              {
                Actor actor = card.GetActor();
                if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
                  position = actor.transform.position;
                else
                  continue;
              }
              else
                continue;
            }
            if (ent1 != null)
              this.DrawDebugTextForHighlightedCard(entity, (Vector3) DebugTextManager.WorldPosToScreenPos(position));
            else
              this.DrawDebugTextForCard(entity, position);
            if (ent1 != null)
            {
              if (!ent1.IsHero())
                return;
              Entity ent2 = !ent1.IsControlledByFriendlySidePlayer() ? (Entity) GameState.Get().GetOpposingSidePlayer() : (Entity) GameState.Get().GetFriendlySidePlayer();
              Vector2 pos = DebugTextManager.WorldPosToScreenPos(ent2.GetHeroCard().transform.position) + new Vector2(-300f, 0.0f);
              this.DrawDebugTextForHighlightedCard(ent2, (Vector3) pos);
              return;
            }
          }
        }
      }
    }
    if (ent1 == gameState.GetGameEntity())
    {
      this.DrawDebugTextForHighlightedCard(ent1, (Vector3) DebugTextManager.WorldPosToScreenPos(EndTurnButton.Get().transform.position));
    }
    else
    {
      this.DrawDebugTextForCard((Entity) gameState.GetGameEntity(), EndTurnButton.Get().transform.position);
      foreach (Player ent3 in GameState.Get().GetPlayerMap().Values)
      {
        if (ent3 != null && !((UnityEngine.Object) ent3.GetHeroCard() == (UnityEngine.Object) null))
        {
          Vector2 pos = DebugTextManager.WorldPosToScreenPos(ent3.GetHeroCard().transform.position) + new Vector2(-300f, 0.0f);
          this.DrawDebugTextForCard((Entity) ent3, (Vector3) pos, true);
        }
      }
    }
  }

  private string HandleCornerDisplay(Entity ent, GAME_TAG tag, string currentString)
  {
    if (ent.HasTag(tag))
    {
      if (currentString != "")
        currentString += "\n";
      GAME_TAG tag1 = (GAME_TAG) ent.GetTag(tag);
      string s = tag1.ToString();
      string str = int.TryParse(s, out int _) ? "" : string.Format("{0}: ", (object) s);
      currentString = string.Format("{0}{1}\n{2}{3}", (object) currentString, (object) ent.GetName(), (object) str, (object) ent.GetTag(tag1));
    }
    return currentString;
  }

  private void DrawDebugTextForHighlightedCard(Entity ent, Vector3 pos)
  {
    string text = this.DrawDebugTextForCard(ent, pos, true, true);
    Vector3 vector3 = new Vector3(0.0f, DebugTextManager.Get().TextSize(text).y + 5f, 0.0f);
    if (ent.IsGame())
    {
      List<Entity> attachments = ent.GetAttachments();
      for (int index = 0; index < attachments.Count; ++index)
      {
        Vector3 pos1 = index % 2 != 0 ? pos - vector3 * (float) (index / 2 + 1) : pos + vector3 * (float) (index / 2 + 1);
        this.DrawDebugTextForCard(attachments[index], pos1, true);
      }
    }
    else
    {
      if (ent.IsControlledByOpposingSidePlayer())
        vector3.y = -vector3.y;
      foreach (Entity attachment in ent.GetAttachments())
      {
        pos += vector3;
        this.DrawDebugTextForCard(attachment, pos, true);
      }
    }
  }

  private string DrawDebugTextForCard(
    Entity ent,
    Vector3 pos,
    bool screenSpace = false,
    bool forceShowZeroTags = false)
  {
    string text = "";
    for (int index = 0; index < this.m_tagsToDisplay.Count; ++index)
    {
      GAME_TAG enumTag = this.m_tagsToDisplay[index];
      int tag = ent.GetTag(enumTag);
      if (forceShowZeroTags || !this.m_hideZeroTags || tag != 0)
        text = string.Format("{0}\n{1}: {2}", (object) text, (object) enumTag.ToString(), (object) tag);
    }
    if (!string.IsNullOrEmpty(text))
    {
      text = ent.GetName() + text;
      DebugTextManager.Get().DrawDebugText(text, pos, 0.0f, screenSpace);
    }
    return text;
  }
}
