using Blizzard.T5.Configuration;
using Blizzard.T5.Core.Utils;
using Hearthstone.Util;
using System;
using System.IO;
using UnityEngine;

internal class LogArchive
{
  private ulong m_numLinesWritten;
  private int m_maxFileSizeKB = 5000;
  private bool m_maxFileSizeEnabled = true;
  private bool m_stopLogging;
  private static LogArchive s_instance;

  public string LogPath { get; private set; }

  public static LogArchive Get()
  {
    if (LogArchive.s_instance == null)
    {
      LogArchive.s_instance = new LogArchive();
      LogArchive.s_instance.Initialize();
    }
    return LogArchive.s_instance;
  }

  private void Initialize()
  {
    string logsPath = Log.LogsPath;
    this.MakeLogPath(Log.LogsPath);
    try
    {
      Directory.CreateDirectory(logsPath);
      this.CleanOldLogs(logsPath);
      this.FetchMaxFileSizeFromOptions();
      Application.logMessageReceived += new Application.LogCallback(this.HandleLog);
      Debug.LogFormat("Logging Unity output to: {0}", (object) this.LogPath);
    }
    catch (IOException ex)
    {
      Log.All.PrintWarning("Failed to write archive logs to: \"" + this.LogPath + "\"!");
      Log.All.PrintWarning(ex.ToString());
    }
  }

  private void CleanOldLogs(string logFolderPath)
  {
    int num1 = 5;
    FileInfo[] files = new DirectoryInfo(logFolderPath).GetFiles();
    Array.Sort<FileInfo>(files, (Comparison<FileInfo>) ((a, b) => a.LastWriteTime.CompareTo(b.LastWriteTime)));
    int num2 = files.Length - (num1 - 1);
    for (int index = 0; index < num2; ++index)
    {
      if (index >= files.Length)
        break;
      try
      {
        files[index].Delete();
      }
      catch (Exception ex)
      {
        Log.All.PrintError("Failed to delete the file '{0}': {1}", (object) files[index], (object) ex.Message);
      }
    }
  }

  private void MakeLogPath(string logFolderPath)
  {
    if (!string.IsNullOrEmpty(this.LogPath))
      return;
    string timestamp = LogArchive.GenerateTimestamp();
    string str = "hearthstone_" + timestamp.Replace("-", "_").Replace(" ", "_").Replace(":", "_").Remove(timestamp.Length - 4) + ".log";
    this.LogPath = logFolderPath + "/" + str;
  }

  private void HandleLog(string logString, string stackTrace, LogType type)
  {
    if (this.m_stopLogging)
      return;
    try
    {
      if (this.m_maxFileSizeEnabled && this.m_numLinesWritten % 100UL == 0UL)
      {
        FileInfo fileInfo = new FileInfo(this.LogPath);
        if (fileInfo.Exists && fileInfo.Length > (long) (this.m_maxFileSizeKB * 1024))
        {
          this.m_stopLogging = true;
          using (StreamWriter log = new StreamWriter(this.LogPath, true))
          {
            LogArchive.WriteLogLine(log, "");
            LogArchive.WriteLogLine(log, "");
            LogArchive.WriteLogLine(log, "==================================================================");
            LogArchive.WriteLogLine(log, "Truncating log, which has reached the size limit of {0}KB", (object) this.m_maxFileSizeKB);
            LogArchive.WriteLogLine(log, "==================================================================\n\n");
            return;
          }
        }
      }
      using (StreamWriter log = new StreamWriter(this.LogPath, true))
      {
        if (!string.IsNullOrEmpty(stackTrace))
          LogArchive.WriteLogLine(log, "{0}\n{1}", (object) logString, (object) stackTrace);
        else
          LogArchive.WriteLogLine(log, "{0}", (object) logString);
        ++this.m_numLinesWritten;
      }
    }
    catch (Exception ex)
    {
      Log.All.PrintError("LogArchive.HandleLog() - Failed to write \"{0}\". Exception={1}", (object) logString, (object) ex.Message);
    }
  }

  private void FetchMaxFileSizeFromOptions()
  {
    ConfigFile configFile = new ConfigFile();
    if (!configFile.FullLoad(PlatformFilePaths.GetClientConfigPath()))
      return;
    int num = configFile.Get("LogArchive.FileSizeLimit.Int", this.m_maxFileSizeKB);
    if (num == 0)
      this.m_maxFileSizeEnabled = false;
    this.m_maxFileSizeKB = num;
  }

  private static string GenerateTimestamp() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

  private static void WriteLogLine(StreamWriter log, string format, params object[] args)
  {
    string str1 = GeneralUtils.SafeFormat(format, args);
    string str2 = LogArchive.GenerateTimestamp() + ": " + str1;
    try
    {
      log.WriteLine(str2);
      log.Flush();
    }
    catch (Exception ex)
    {
    }
  }
}
