using Blizzard.T5.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Navigation
{
  private static Stack<Navigation.NavigateBackHandler> m_history = new Stack<Navigation.NavigateBackHandler>();

  public static bool NAVIGATION_DEBUG
  {
    get => Vars.Key("Application.Navigation.Debug").GetBool(false);
    set => Vars.Key("Application.Navigation.Debug").Set(value.ToString(), false);
  }

  public static void Clear()
  {
    Navigation.m_history.Clear();
    if (!Navigation.NAVIGATION_DEBUG)
      return;
    Navigation.DumpStack();
  }

  public static bool IsEmpty => Navigation.m_history.Count == 0;

  public static bool CanGoBack => !Navigation.IsEmpty && Navigation.CanNavigate();

  public static bool GoBack()
  {
    if (!Navigation.CanGoBack)
      return false;
    Navigation.NavigateBackHandler entry = Navigation.m_history.Peek();
    if (!entry())
      return false;
    if (Navigation.m_history.Count > 0 && entry == Navigation.m_history.Peek())
      Navigation.m_history.Pop();
    else if (Navigation.m_history.Contains(entry))
      Log.All.PrintWarning("Navigation tried to remove handler and failed, but the handler exists further down the stack! Perhaps something went wrong, like a new scene added itself to the top of the stack in its Awake? Handler to remove: {0}", (object) Navigation.StackEntryToString(entry));
    if (Navigation.NAVIGATION_DEBUG)
      Navigation.DumpStack();
    return true;
  }

  public static void Push(Navigation.NavigateBackHandler handler)
  {
    if (handler == null)
      return;
    Navigation.m_history.Push(handler);
    if (!Navigation.NAVIGATION_DEBUG)
      return;
    Navigation.DumpStack();
  }

  public static void PushUnique(Navigation.NavigateBackHandler handler)
  {
    if (handler == null || Navigation.m_history.Contains(handler))
      return;
    if (!handler.Method.IsStatic)
      Debug.LogWarningFormat("Navigation.PushUnique called for non-static method! - {0}", (object) handler.Method.Name);
    Navigation.m_history.Push(handler);
    if (!Navigation.NAVIGATION_DEBUG)
      return;
    Navigation.DumpStack();
  }

  public static void PushIfNotOnTop(Navigation.NavigateBackHandler handler)
  {
    if (handler == null)
      return;
    if (Navigation.m_history.Count > 0 && Navigation.m_history.Peek() == handler)
    {
      if (!Navigation.NAVIGATION_DEBUG)
        return;
      Debug.LogFormat("Navigation - Did not push {0}, it already exists on the top of the stack!", (object) Navigation.StackEntryToString(handler));
    }
    else
    {
      Navigation.m_history.Push(handler);
      if (!Navigation.NAVIGATION_DEBUG)
        return;
      Navigation.DumpStack();
    }
  }

  public static void Pop()
  {
    if (Navigation.IsEmpty || !Navigation.CanNavigate())
      return;
    Navigation.m_history.Pop();
    if (!Navigation.NAVIGATION_DEBUG)
      return;
    Navigation.DumpStack();
  }

  public static bool RemoveHandler(Navigation.NavigateBackHandler handler)
  {
    if (Navigation.IsEmpty)
      return false;
    int num = Navigation.m_history.Contains(handler) ? 1 : 0;
    if (num != 0)
      Navigation.m_history = new Stack<Navigation.NavigateBackHandler>(Navigation.m_history.Where<Navigation.NavigateBackHandler>((Func<Navigation.NavigateBackHandler, bool>) (h => h != handler)).Reverse<Navigation.NavigateBackHandler>());
    if (!Navigation.NAVIGATION_DEBUG)
      return num != 0;
    Navigation.DumpStack();
    return num != 0;
  }

  public static bool BackStackContainsHandler(Navigation.NavigateBackHandler handler) => Navigation.m_history.Contains(handler);

  public static void PushBlockBackingOut() => Navigation.Push(new Navigation.NavigateBackHandler(Navigation.BlockBackingOut));

  public static void PopBlockBackingOut() => Navigation.RemoveHandler(new Navigation.NavigateBackHandler(Navigation.BlockBackingOut));

  private static bool BlockBackingOut() => false;

  private static bool CanNavigate()
  {
    if (GameUtils.IsAnyTransitionActive())
      return false;
    switch (GameMgr.Get().GetFindGameState())
    {
      case FindGameState.CLIENT_STARTED:
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CONNECTING:
      case FindGameState.SERVER_GAME_STARTED:
      case FindGameState.SERVER_GAME_CANCELED:
        return false;
      default:
        return true;
    }
  }

  public static string StackDumpString
  {
    get
    {
      int count = 0;
      return string.Join("\n", Navigation.m_history.Select<Navigation.NavigateBackHandler, string>((Func<Navigation.NavigateBackHandler, string>) (entry => string.Format("{0}: {1}", (object) count++, (object) Navigation.StackEntryToString(entry)))).ToArray<string>());
    }
  }

  private static string StackEntryToString(Navigation.NavigateBackHandler entry) => string.Format("{0}.{1} Target={2}", (object) entry.Method.DeclaringType, (object) entry.Method.Name, entry == null || entry.Target == null ? (entry.Method.IsStatic ? (object) "<static>" : (object) "null") : (object) entry.Target.ToString());

  public static void DumpStack()
  {
    Debug.Log((object) string.Format("Navigation Stack Dump (count: {0})\n", (object) Navigation.m_history.Count));
    int num = 0;
    foreach (Navigation.NavigateBackHandler entry in Navigation.m_history)
    {
      Debug.Log((object) string.Format("{0}: {1}\n", (object) num, (object) Navigation.StackEntryToString(entry)));
      ++num;
    }
  }

  public static void GoBackUntilOnNavigateBackCalled(Navigation.NavigateBackHandler handler)
  {
    do
      ;
    while (Navigation.BackStackContainsHandler(handler) && Navigation.GoBack());
  }

  public delegate bool NavigateBackHandler();
}
