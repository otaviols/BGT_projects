using Hearthstone;
using System;
using System.Diagnostics;
using System.IO;

public class BattleNetClient
{
  public static bool needsToRun => BattleNetClient.usedOnThisPlatform && !BattleNetClient.launchedHearthstone;

  public static void quitHearthstoneAndRun()
  {
    Log.All.PrintWarning("Hearthstone was not run from Battle.net Client");
    if (!BattleNetClient.bootstrapper.Exists)
    {
      Log.All.PrintWarning("Hearthstone could not find Battle.net client");
      Error.AddFatal(FatalErrorReason.NO_BNET_CLIENT, "GLUE_CANNOT_FIND_BATTLENET_CLIENT");
    }
    else
    {
      try
      {
        new Process()
        {
          StartInfo = {
            UseShellExecute = false,
            FileName = BattleNetClient.bootstrapper.FullName,
            Arguments = "-uid hs_beta"
          },
          EnableRaisingEvents = true
        }.Start();
        Log.All.PrintWarning("Hearthstone ran Battle.net Client.  Exiting.");
        HearthstoneApplication.Get().Exit();
      }
      catch (Exception ex)
      {
        Error.AddFatal(FatalErrorReason.FAIL_BNET_CLIENT, "GLUE_CANNOT_RUN_BATTLENET_CLIENT");
        Log.All.PrintWarning("Hearthstone could not launch Battle.net client: {0}", (object) ex.Message);
      }
    }
  }

  private static bool usedOnThisPlatform => true;

  private static bool launchedHearthstone
  {
    get
    {
      foreach (string commandLineArg in Environment.GetCommandLineArgs())
      {
        if (commandLineArg.Equals("-launch", StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }
  }

  private static FileInfo bootstrapper => new FileInfo("Hearthstone Beta Launcher.exe");
}
