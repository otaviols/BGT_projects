using System;
using System.IO;
using System.Runtime.InteropServices;

public class DLLUtils
{
  [DllImport("kernel32.dll")]
  public static extern IntPtr LoadLibrary(string filename);

  [DllImport("kernel32.dll")]
  public static extern IntPtr GetProcAddress(IntPtr module, string funcName);

  [DllImport("kernel32.dll")]
  public static extern bool FreeLibrary(IntPtr module);

  public static string GetPluginPath(string fileName) => string.Format("Hearthstone_Data/Plugins/{0}", (object) fileName);

  public static IntPtr LoadPlugin(string fileName, bool handleError = true)
  {
    try
    {
      string pluginPath = DLLUtils.GetPluginPath(fileName);
      IntPtr num = DLLUtils.LoadLibrary(pluginPath);
      string str = Directory.GetCurrentDirectory().Replace("\\", "/");
      if (num == IntPtr.Zero & handleError)
      {
        Error.AddDevFatal("Failed to load plugin from '{0}'", (object) string.Format("{0}/{1}", (object) str, (object) pluginPath));
        Error.AddFatal(FatalErrorReason.LOAD_PLUGIN, "GLOBAL_ERROR_ASSET_LOAD_FAILED", (object) fileName);
      }
      return num;
    }
    catch (Exception ex)
    {
      Error.AddDevFatal("FileUtils.LoadPlugin() - Exception occurred. message={0} stackTrace={1}", (object) ex.Message, (object) ex.StackTrace);
      return IntPtr.Zero;
    }
  }
}
