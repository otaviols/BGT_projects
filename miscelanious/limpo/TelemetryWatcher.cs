using Blizzard.T5.Core;
using Blizzard.Telemetry.WTCG.Client;
using System;
using System.Collections.Generic;

public static class TelemetryWatcher
{
  private static List<TelemetryWatcherWatchType> s_currentlyWatching = new List<TelemetryWatcherWatchType>();
  private static readonly object s_watchLock = new object();
  private static readonly Map<TelemetryWatcherWatchType, Action> s_watchTypeSetupActions = new Map<TelemetryWatcherWatchType, Action>()
  {
    {
      TelemetryWatcherWatchType.CollectionManagerFromDeckPicker,
      new Action(TelemetryWatcher.WatchForCollectionVisitFromDeckPicker)
    },
    {
      TelemetryWatcherWatchType.StoreFromPackOpening,
      new Action(TelemetryWatcher.WatchForStoreVisitFromPackOpening)
    }
  };
  private static readonly Map<TelemetryWatcherWatchType, Action> s_watchTypeTeardownActions = new Map<TelemetryWatcherWatchType, Action>()
  {
    {
      TelemetryWatcherWatchType.CollectionManagerFromDeckPicker,
      new Action(TelemetryWatcher.StopWatchingForCollectionVisitFromDeckPicker)
    },
    {
      TelemetryWatcherWatchType.StoreFromPackOpening,
      new Action(TelemetryWatcher.StopWatchingForStoreVisitFromPackOpening)
    }
  };

  public static void WatchFor(TelemetryWatcherWatchType watchType)
  {
    Action action;
    if (!TelemetryWatcher.s_watchTypeSetupActions.TryGetValue(watchType, out action))
    {
      Log.Telemetry.Print("Watching for type={0} is not currently supported");
    }
    else
    {
      lock (TelemetryWatcher.s_watchLock)
      {
        if (TelemetryWatcher.s_currentlyWatching.Contains(watchType))
        {
          Log.Telemetry.Print("Already watching for type={0}", (object) watchType);
          return;
        }
        TelemetryWatcher.s_currentlyWatching.Add(watchType);
      }
      action();
      Log.Telemetry.Print("Watching for type={0}", (object) watchType);
    }
  }

  public static void StopWatchingFor(TelemetryWatcherWatchType watchType)
  {
    lock (TelemetryWatcher.s_watchLock)
    {
      if (!TelemetryWatcher.s_currentlyWatching.Remove(watchType))
      {
        Log.Telemetry.Print("Was not watching for type={0}", (object) watchType);
        return;
      }
    }
    Action action;
    if (TelemetryWatcher.s_watchTypeTeardownActions.TryGetValue(watchType, out action))
      action();
    Log.Telemetry.Print("No longer watching for type={0}", (object) watchType);
  }

  private static void OnScenePreloadedExclusiveWatch(
    SceneMgr.Mode nextMode,
    SceneMgr.Mode targetMode,
    TelemetryWatcherWatchType watchType)
  {
    Log.Telemetry.Print("Scene change detected while watching for type={0}.  Next scene={1}, Target={2}", (object) watchType, (object) nextMode, (object) targetMode);
    if (nextMode == targetMode)
      return;
    TelemetryWatcher.StopWatchingFor(watchType);
  }

  private static void OnBoxButtonPressedExclusiveWatch(
    Box.ButtonType buttonPressed,
    Box.ButtonType targetType,
    TelemetryWatcherWatchType watchType,
    Action onTargetPressed)
  {
    Log.Telemetry.Print("Button pressed on Box while watching for type={0}.  Button pressed={1}, target={2}", (object) watchType, (object) buttonPressed, (object) targetType);
    if (buttonPressed == targetType)
      onTargetPressed();
    TelemetryWatcher.StopWatchingFor(watchType);
  }

  private static void WatchForStoreVisitFromPackOpening()
  {
    Box.Get().AddButtonPressListener(new Box.ButtonPressCallback(TelemetryWatcher.OnBoxButtonPressStoreWatcher));
    SceneMgr.Get().RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(TelemetryWatcher.OnScenePreloadedStoreWatcher));
  }

  private static void StopWatchingForStoreVisitFromPackOpening()
  {
    Box box = Box.Get();
    if ((UnityEngine.Object) box != (UnityEngine.Object) null)
      box.RemoveButtonPressListener(new Box.ButtonPressCallback(TelemetryWatcher.OnBoxButtonPressStoreWatcher));
    SceneMgr.Get().UnregisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(TelemetryWatcher.OnScenePreloadedStoreWatcher));
  }

  private static void OnBoxButtonPressStoreWatcher(
    Box.ButtonType type,
    bool isShowingTutorialPreview,
    object userData)
  {
    TelemetryWatcher.OnBoxButtonPressedExclusiveWatch(type, Box.ButtonType.STORE, TelemetryWatcherWatchType.StoreFromPackOpening, (Action) (() => TelemetryManager.Client().SendPackOpenToStore(PackOpenToStore.Path.BACK_TO_BOX)));
  }

  private static void OnScenePreloadedStoreWatcher(
    SceneMgr.Mode prevMode,
    SceneMgr.Mode nextMode,
    object userData)
  {
    TelemetryWatcher.OnScenePreloadedExclusiveWatch(nextMode, SceneMgr.Mode.HUB, TelemetryWatcherWatchType.StoreFromPackOpening);
  }

  private static void WatchForCollectionVisitFromDeckPicker()
  {
    SceneMgr.Get().RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(TelemetryWatcher.OnScenePreLoadedCollectionWatcher));
    Box.Get().AddButtonPressListener(new Box.ButtonPressCallback(TelemetryWatcher.OnBoxButtonPressCollectionWatcher));
  }

  private static void StopWatchingForCollectionVisitFromDeckPicker()
  {
    Box box = Box.Get();
    if ((UnityEngine.Object) box != (UnityEngine.Object) null)
      box.RemoveButtonPressListener(new Box.ButtonPressCallback(TelemetryWatcher.OnBoxButtonPressCollectionWatcher));
    SceneMgr.Get().UnregisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(TelemetryWatcher.OnScenePreLoadedCollectionWatcher));
  }

  private static void OnBoxButtonPressCollectionWatcher(
    Box.ButtonType type,
    bool isShowingTutorialPreview,
    object userData)
  {
    TelemetryWatcher.OnBoxButtonPressedExclusiveWatch(type, Box.ButtonType.COLLECTION, TelemetryWatcherWatchType.CollectionManagerFromDeckPicker, (Action) (() => TelemetryManager.Client().SendDeckPickerToCollection(DeckPickerToCollection.Path.BACK_TO_BOX)));
  }

  private static void OnScenePreLoadedCollectionWatcher(
    SceneMgr.Mode prevMode,
    SceneMgr.Mode nextMode,
    object userData)
  {
    TelemetryWatcher.OnScenePreloadedExclusiveWatch(nextMode, SceneMgr.Mode.HUB, TelemetryWatcherWatchType.CollectionManagerFromDeckPicker);
  }
}
