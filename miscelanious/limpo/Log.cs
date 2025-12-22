using Blizzard.T5.Logging;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Util;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Log
{
  private static readonly List<string> s_legacyHearthstoneLoggers = new List<string>()
  {
    nameof (All),
    "AchievementManager",
    nameof (Achievements),
    nameof (AdTracking),
    nameof (Adventures),
    nameof (Arena),
    nameof (Asset),
    nameof (AsyncLoading),
    nameof (BattlegroundsAuthoring),
    nameof (BattleNet),
    "BIReport",
    nameof (Box),
    nameof (BreakingNews),
    "BugReporter",
    nameof (CardbackMgr),
    "ChangedCards",
    "ClientRequestManager",
    nameof (CloudStorage),
    nameof (CollectionDeckBox),
    nameof (CollectionManager),
    nameof (CoinManager),
    nameof (ConfigFile),
    nameof (ContentConnect),
    nameof (Crafting),
    nameof (CRM),
    nameof (Dbf),
    "DeckHelper",
    nameof (DeckRuleset),
    nameof (Decks),
    nameof (DeckTray),
    nameof (DeepLink),
    "DelayedReporter",
    nameof (DeviceEmulation),
    nameof (Downloader),
    nameof (EndOfGame),
    nameof (ErrorReporter),
    nameof (EventTable),
    nameof (EventTiming),
    nameof (ExceptionReporter),
    nameof (FaceDownCard),
    nameof (FiresideGatherings),
    nameof (FlowPerformance),
    nameof (Font),
    nameof (FullScreenFX),
    nameof (GameMgr),
    nameof (Gameplay),
    nameof (Graphics),
    nameof (Hand),
    nameof (InGameBrowser),
    nameof (InGameMessage),
    nameof (InnKeepersSpecial),
    nameof (Jobs),
    nameof (Lettuce),
    nameof (LoadingScreen),
    nameof (Login),
    nameof (MinSpecManager),
    nameof (MissingAssets),
    nameof (MobileCallback),
    nameof (MulliganManager),
    nameof (NarrativeManager),
    nameof (Net),
    nameof (Notifications),
    nameof (Offline),
    nameof (Options),
    "Packet",
    nameof (Party),
    nameof (Performance),
    nameof (PlayErrors),
    nameof (PlayMaker),
    nameof (PlayModeInvestigation),
    nameof (Power),
    nameof (Presence),
    nameof (Privacy),
    "PVPDR",
    nameof (RAF),
    nameof (ReturningPlayer),
    "Replay",
    nameof (Reset),
    nameof (RewardBox),
    nameof (Services),
    nameof (SmartDiscover),
    nameof (Spells),
    nameof (Sound),
    "Spectator",
    nameof (Store),
    "Tag",
    nameof (TavernBrawl),
    nameof (Telemetry),
    nameof (TemporaryAccount),
    nameof (UberText),
    nameof (UIFramework),
    nameof (UIStatus),
    nameof (UserAttention),
    nameof (W8Touch),
    nameof (Zone)
  };
  private static readonly LogInfo[] DEFAULT_LOG_INFOS = new LogInfo[5]
  {
    new LogInfo()
    {
      m_name = nameof (Jobs),
      m_consolePrinting = true,
      m_minLevel = Blizzard.T5.Logging.LogLevel.Error
    },
    new LogInfo()
    {
      m_name = nameof (Downloader),
      m_filePrinting = true,
      m_consolePrinting = true,
      m_minLevel = Blizzard.T5.Logging.LogLevel.Info
    },
    new LogInfo()
    {
      m_name = nameof (Login),
      m_filePrinting = true,
      m_consolePrinting = true,
      m_minLevel = Blizzard.T5.Logging.LogLevel.Info
    },
    new LogInfo()
    {
      m_name = nameof (ExceptionReporter),
      m_filePrinting = true,
      m_consolePrinting = true,
      m_minLevel = Blizzard.T5.Logging.LogLevel.Info
    },
    new LogInfo()
    {
      m_name = nameof (Offline),
      m_filePrinting = true,
      m_minLevel = Blizzard.T5.Logging.LogLevel.Info
    }
  };

  public static Logger All => Log.GetLoggerFromSystem(nameof (All));

  public static Logger Achievements => Log.GetLoggerFromSystem(nameof (Achievements));

  public static Logger AdTracking => Log.GetLoggerFromSystem(nameof (AdTracking));

  public static Logger Adventures => Log.GetLoggerFromSystem(nameof (Adventures));

  public static Logger Arena => Log.GetLoggerFromSystem(nameof (Arena));

  public static Logger Asset => Log.GetLoggerFromSystem(nameof (Asset));

  public static Logger AsyncLoading => Log.GetLoggerFromSystem(nameof (AsyncLoading));

  public static Logger BattlegroundsAuthoring => Log.GetLoggerFromSystem(nameof (BattlegroundsAuthoring));

  public static Logger BattleNet => Log.GetLoggerFromSystem(nameof (BattleNet));

  public static Logger Box => Log.GetLoggerFromSystem(nameof (Box));

  public static Logger BreakingNews => Log.GetLoggerFromSystem(nameof (BreakingNews));

  public static Logger CardbackMgr => Log.GetLoggerFromSystem(nameof (CardbackMgr));

  public static Logger CloudStorage => Log.GetLoggerFromSystem(nameof (CloudStorage));

  public static Logger CollectionDeckBox => Log.GetLoggerFromSystem(nameof (CollectionDeckBox));

  public static Logger CollectionManager => Log.GetLoggerFromSystem(nameof (CollectionManager));

  public static Logger CoinManager => Log.GetLoggerFromSystem(nameof (CoinManager));

  public static Logger ConfigFile => Log.GetLoggerFromSystem(nameof (ConfigFile));

  public static Logger ContentConnect => Log.GetLoggerFromSystem(nameof (ContentConnect));

  public static Logger CosmeticPreview => Log.GetLoggerFromSystem(nameof (CosmeticPreview));

  public static Logger Crafting => Log.GetLoggerFromSystem(nameof (Crafting));

  public static Logger CRM => Log.GetLoggerFromSystem(nameof (CRM));

  public static Logger Dbf => Log.GetLoggerFromSystem(nameof (Dbf));

  public static Logger DeckRuleset => Log.GetLoggerFromSystem(nameof (DeckRuleset));

  public static Logger Decks => Log.GetLoggerFromSystem(nameof (Decks));

  public static Logger DeckTray => Log.GetLoggerFromSystem(nameof (DeckTray));

  public static Logger DeepLink => Log.GetLoggerFromSystem(nameof (DeepLink));

  public static Logger DeviceEmulation => Log.GetLoggerFromSystem(nameof (DeviceEmulation));

  public static Logger Downloader => Log.GetLoggerFromSystem(nameof (Downloader));

  public static Logger EndOfGame => Log.GetLoggerFromSystem(nameof (EndOfGame));

  public static Logger ErrorReporter => Log.GetLoggerFromSystem(nameof (ErrorReporter));

  public static Logger EventTable => Log.GetLoggerFromSystem(nameof (EventTable));

  public static Logger EventTiming => Log.GetLoggerFromSystem(nameof (EventTiming));

  public static Logger ExceptionReporter => Log.GetLoggerFromSystem(nameof (ExceptionReporter));

  public static Logger FaceDownCard => Log.GetLoggerFromSystem(nameof (FaceDownCard));

  public static Logger FiresideGatherings => Log.GetLoggerFromSystem(nameof (FiresideGatherings));

  public static Logger FlowPerformance => Log.GetLoggerFromSystem(nameof (FlowPerformance));

  public static Logger Font => Log.GetLoggerFromSystem(nameof (Font));

  public static Logger FullScreenFX => Log.GetLoggerFromSystem(nameof (FullScreenFX));

  public static Logger GameMgr => Log.GetLoggerFromSystem(nameof (GameMgr));

  public static Logger Gameplay => Log.GetLoggerFromSystem(nameof (Gameplay));

  public static Logger Graphics => Log.GetLoggerFromSystem(nameof (Graphics));

  public static Logger Hand => Log.GetLoggerFromSystem(nameof (Hand));

  public static Logger InGameBrowser => Log.GetLoggerFromSystem(nameof (InGameBrowser));

  public static Logger InGameMessage => Log.GetLoggerFromSystem(nameof (InGameMessage));

  public static Logger InnKeepersSpecial => Log.GetLoggerFromSystem(nameof (InnKeepersSpecial));

  public static Logger Jobs => Log.GetLoggerFromSystem(nameof (Jobs));

  public static Logger Lettuce => Log.GetLoggerFromSystem(nameof (Lettuce));

  public static Logger LoadingScreen => Log.GetLoggerFromSystem(nameof (LoadingScreen));

  public static Logger Login => Log.GetLoggerFromSystem(nameof (Login));

  public static Logger MinSpecManager => Log.GetLoggerFromSystem(nameof (MinSpecManager));

  public static Logger MissingAssets => Log.GetLoggerFromSystem(nameof (MissingAssets));

  public static Logger MobileCallback => Log.GetLoggerFromSystem(nameof (MobileCallback));

  public static Logger MulliganManager => Log.GetLoggerFromSystem(nameof (MulliganManager));

  public static Logger NarrativeManager => Log.GetLoggerFromSystem(nameof (NarrativeManager));

  public static Logger Net => Log.GetLoggerFromSystem(nameof (Net));

  public static Logger Notifications => Log.GetLoggerFromSystem(nameof (Notifications));

  public static Logger Offline => Log.GetLoggerFromSystem(nameof (Offline));

  public static Logger Options => Log.GetLoggerFromSystem(nameof (Options));

  public static Logger Party => Log.GetLoggerFromSystem(nameof (Party));

  public static Logger Performance => Log.GetLoggerFromSystem(nameof (Performance));

  public static Logger PlayErrors => Log.GetLoggerFromSystem(nameof (PlayErrors));

  public static Logger PlayMaker => Log.GetLoggerFromSystem(nameof (PlayMaker));

  public static Logger PlayModeInvestigation => Log.GetLoggerFromSystem(nameof (PlayModeInvestigation));

  public static Logger Power => Log.GetLoggerFromSystem(nameof (Power));

  public static Logger Presence => Log.GetLoggerFromSystem(nameof (Presence));

  public static Logger Privacy => Log.GetLoggerFromSystem(nameof (Privacy));

  public static Logger RAF => Log.GetLoggerFromSystem(nameof (RAF));

  public static Logger ReturningPlayer => Log.GetLoggerFromSystem(nameof (ReturningPlayer));

  public static Logger Reset => Log.GetLoggerFromSystem(nameof (Reset));

  public static Logger RewardBox => Log.GetLoggerFromSystem(nameof (RewardBox));

  public static Logger Services => Log.GetLoggerFromSystem(nameof (Services));

  public static Logger SmartDiscover => Log.GetLoggerFromSystem(nameof (SmartDiscover));

  public static Logger Spells => Log.GetLoggerFromSystem(nameof (Spells));

  public static Logger Sound => Log.GetLoggerFromSystem(nameof (Sound));

  public static Logger Store => Log.GetLoggerFromSystem(nameof (Store));

  public static Logger TavernBrawl => Log.GetLoggerFromSystem(nameof (TavernBrawl));

  public static Logger Telemetry => Log.GetLoggerFromSystem(nameof (Telemetry));

  public static Logger TemporaryAccount => Log.GetLoggerFromSystem(nameof (TemporaryAccount));

  public static Logger UberText => Log.GetLoggerFromSystem(nameof (UberText));

  public static Logger UIFramework => Log.GetLoggerFromSystem(nameof (UIFramework));

  public static Logger UIStatus => Log.GetLoggerFromSystem(nameof (UIStatus));

  public static Logger UserAttention => Log.GetLoggerFromSystem(nameof (UserAttention));

  public static Logger W8Touch => Log.GetLoggerFromSystem(nameof (W8Touch));

  public static Logger Zone => Log.GetLoggerFromSystem(nameof (Zone));

  private static Logger GetLoggerFromSystem(string name)
  {
    Log.Initialize();
    return LogSystem.Get().GetFullLogger(name);
  }

  public static string ConfigPath
  {
    get
    {
      string path;
      if (false)
      {
        path = string.Format("{0}/{1}", (object) Application.persistentDataPath, (object) "log.config");
        if (!File.Exists(path))
          path = PlatformFilePaths.GetAssetPath("log.config", false);
      }
      else
      {
        path = string.Format("{0}/{1}", (object) PlatformFilePaths.ExternalDataPath, (object) "log.config");
        if (!File.Exists(path))
        {
          path = string.Format("{0}/{1}", (object) PlatformFilePaths.PersistentDataPath, (object) "log.config");
          if (!File.Exists(path))
            path = PlatformFilePaths.GetAssetPath("log.config", false);
        }
      }
      return path;
    }
  }

  public static IEnumerable<string> GetEnabledLogNames()
  {
    Dictionary<string, Logger> allLoggers = LogSystem.Get().GetAllLoggers();
    List<string> enabledLogNames = new List<string>(allLoggers.Count);
    foreach (Logger logger in allLoggers.Values)
    {
      if (LogSystem.Get().GetLogInfo(logger.GetName()).m_filePrinting)
        enabledLogNames.Add(logger.GetName());
    }
    return (IEnumerable<string>) enabledLogNames;
  }

  public static IEnumerable<string> GetDefaultLogNames()
  {
    List<string> defaultLogNames = new List<string>(Log.DEFAULT_LOG_INFOS.Length);
    foreach (LogInfo logInfo in Log.DEFAULT_LOG_INFOS)
      defaultLogNames.Add(logInfo.m_name);
    return (IEnumerable<string>) defaultLogNames;
  }

  public static void SetStandardLogInfo(string logName)
  {
    LogInfo logInfo = LogSystem.Get().GetLogInfo(logName);
    if (logInfo == null)
      logInfo = new LogInfo() { m_name = logName };
    logInfo.m_filePrinting = true;
    logInfo.m_screenPrinting = true;
    logInfo.m_minLevel = Blizzard.T5.Logging.LogLevel.Info;
    LogSystem.Get().SetLogInfo(logName, logInfo);
  }

  public static string LogsPath
  {
    get
    {
      string logsPath;
      switch (PlatformSettings.RuntimeOS)
      {
        case OSCategory.iOS:
          logsPath = string.Format("{0}/{1}", (object) PlatformFilePaths.PersistentDataPath, (object) "Logs");
          break;
        case OSCategory.Android:
          logsPath = string.Format("{0}/{1}", (object) PlatformFilePaths.ExternalDataPath, (object) "Logs");
          break;
        default:
          logsPath = "Logs";
          break;
      }
      return logsPath;
    }
  }

  public static void Initialize()
  {
    if (LogSystem.Get().IsConfigured())
      return;
    Log.ConfigureLogSystem(Log.BuildRuntimeLogConfig());
  }

  private static LogConfig BuildRuntimeLogConfig() => new LogConfig()
  {
    Printers = new List<ILogPrinter>()
    {
      (ILogPrinter) new StandardFileLogPrinter(Log.LogsPath, (StandardFileLogPrinter.ExecuteOnMainThread) (func => Processor.ScheduleCallback(0.0f, false, (Processor.ScheduledCallback) (_ => func()))), (StandardFileLogPrinter.IsMainThread) (() => HearthstoneApplication.IsMainThread)),
      (ILogPrinter) new UnityConsoleLogPrinter(),
      (ILogPrinter) new ScreenLogPrinter()
    },
    IsMainThreadFunc = (LogConfig.IsMainThread) (() => HearthstoneApplication.IsMainThread),
    DefaultLogInfo = Log.GetDefaultLogInfo(),
    LogInfoConfigDirectory = Log.ConfigPath
  };

  private static void ConfigureLogSystem(LogConfig config)
  {
    LogSystem.Get().SetConfiguration(config);
    foreach (LogInfo logInfo in Log.DEFAULT_LOG_INFOS)
    {
      if (LogSystem.Get().GetLogInfo(logInfo.m_name) == null)
        LogSystem.Get().SetLogInfo(logInfo.m_name, logInfo);
    }
    foreach (string hearthstoneLogger in Log.s_legacyHearthstoneLoggers)
      LogSystem.Get().CreateFullLogger(hearthstoneLogger);
  }

  private static LogInfo GetDefaultLogInfo() => new LogInfo()
  {
    m_alwaysPrintErrors = true,
    m_consolePrinting = false,
    m_defaultLevel = Blizzard.T5.Logging.LogLevel.Debug,
    m_minLevel = Blizzard.T5.Logging.LogLevel.Debug,
    m_filePrinting = false,
    m_screenPrinting = false,
    m_verbose = false
  };
}
