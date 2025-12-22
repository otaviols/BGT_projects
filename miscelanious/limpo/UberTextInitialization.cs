using Blizzard.T5.Core;

public class UberTextInitialization
{
  public static void InitializeUberText() => UberTextSetup.SetConfig(UberTextInitialization.CreateUberTextConfig());

  private static UberTextConfig CreateUberTextConfig() => new UberTextConfig(28, (IUberTextRenderTextureTracker) new UberTextRenderTextureTracker(), (IUberTextShaderUtil) new UberTextShhadetUtil(), (ILogger) Log.UberText);
}
