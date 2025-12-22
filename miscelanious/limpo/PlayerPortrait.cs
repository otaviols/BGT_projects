using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class PlayerPortrait : MonoBehaviour
{
  private BnetProgramId m_programId;
  private string m_currentTextureName;
  private string m_loadingTextureName;
  private AssetHandle<Texture> m_loadedTexture;
  private static readonly Map<BnetProgramId, string> s_textureNameMap = new Map<BnetProgramId, string>()
  {
    {
      BnetProgramId.HEARTHSTONE,
      "HS.tif:f7eebe7fed3c76b4da1dd53875182b34"
    },
    {
      BnetProgramId.WOW,
      "WOW.tif:c1d7415957aa3497e8ac1e3cc25442b3"
    },
    {
      BnetProgramId.DIABLO3,
      "D3.tif:97e4dfeddc92e4eaf965bf7ad67f85a7"
    },
    {
      BnetProgramId.STARCRAFT,
      "S1.tif:4fed402b1d52dcc4189ac7dce9d17900"
    },
    {
      BnetProgramId.STARCRAFT2,
      "SC2.tif:9b4c5e61999e44d5385b6ec0f732ec49"
    },
    {
      BnetProgramId.PHOENIX,
      "BN.tif:5097f44734476465a9e079abd8b8d576"
    },
    {
      BnetProgramId.PHOENIX_OLD,
      "BN.tif:5097f44734476465a9e079abd8b8d576"
    },
    {
      BnetProgramId.PHOENIX_MOBILE,
      "BN.tif:5097f44734476465a9e079abd8b8d576"
    },
    {
      BnetProgramId.HEROES,
      "Heroes.tif:9ffc5e07959da3e4e850acb6b3054cad"
    },
    {
      BnetProgramId.OVERWATCH,
      "Overwatch2.tif:ae4d98ac2761fb64c8d0099bd3589f84"
    },
    {
      BnetProgramId.BLACKOPS4,
      "VIPR.tif:96eba72539c21b4408d61ae35f571e0b"
    },
    {
      BnetProgramId.WARCRAFT3,
      "W3.tif:4e24f53718a4ff344ba934da823e471d"
    },
    {
      BnetProgramId.MODERNWARFARE,
      "ModernWarfare.tif:c7ea70acfe6235d4baf5c475d6d30b5e"
    },
    {
      BnetProgramId.MODERNWARFARE2_REMASTER,
      "MW2.tif:83b5e9a927db745499fb61379e3c7cd3"
    },
    {
      BnetProgramId.BLACKOPSCOLDWAR,
      "BO.tif:322f8c355eeced846a4d6d1c0940e73e"
    },
    {
      BnetProgramId.CRASHBANDICOOT4,
      "WLBY.tif:a2e632bf5fdd0e947a2bfdaa80520e9c"
    },
    {
      BnetProgramId.DIABLO2RESURRECTED,
      "D2R.tif:5a4b3a345642d074a90591d8415a3f0e"
    },
    {
      BnetProgramId.CALLOFDUTYVANGUARD,
      "CoD_Vanguard.tif:1752c74ff52b59645a7fe94b0db8b3de"
    },
    {
      BnetProgramId.DIABLOIMMORTAL,
      "DI.tif:db62405df0cf27a43b5c4ce59fee53c6"
    },
    {
      BnetProgramId.WARCRAFTARCLIGHTRUMBLE,
      "ArclightRumble.tif:dae01550ff661b14a9871d23b7c63f0a"
    },
    {
      BnetProgramId.ARCADECOLLECTION,
      "Arcade.tif:2c59e80f52a06f04aafd9c738fbfee4d"
    }
  };

  private void OnDestroy() => AssetHandle.SafeDispose<Texture>(ref this.m_loadedTexture);

  public static string GetTextureName(BnetProgramId programId)
  {
    if ((Blizzard.GameService.SDK.Client.Integration.FourCC) programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) null)
      return (string) null;
    if ((Blizzard.GameService.SDK.Client.Integration.FourCC) programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.OVERWATCH && BattleNet.GetCurrentRegion() == BnetRegion.REGION_CN)
      return "Overwatch.tif:a950d4020e3431649992a6da7c716e09";
    string textureName = (string) null;
    PlayerPortrait.s_textureNameMap.TryGetValue(programId, out textureName);
    return textureName;
  }

  public BnetProgramId GetProgramId() => this.m_programId;

  public bool SetProgramId(BnetProgramId programId)
  {
    if ((Blizzard.GameService.SDK.Client.Integration.FourCC) this.m_programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) programId)
      return false;
    this.m_programId = programId;
    this.UpdateIcon();
    return true;
  }

  public bool IsIconReady() => this.m_loadingTextureName == null && this.m_currentTextureName != null;

  public bool IsIconLoading() => this.m_loadingTextureName != null;

  private void UpdateIcon()
  {
    if ((Blizzard.GameService.SDK.Client.Integration.FourCC) this.m_programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) null)
    {
      this.m_currentTextureName = (string) null;
      this.m_loadingTextureName = (string) null;
      RendererExtension.GetMaterial(this.GetComponent<Renderer>()).mainTexture = (Texture) null;
    }
    else
    {
      string textureName = PlayerPortrait.GetTextureName(this.m_programId);
      if (this.m_currentTextureName == textureName || this.m_loadingTextureName == textureName)
        return;
      this.m_loadingTextureName = textureName;
      AssetLoader.Get().LoadAsset<Texture>((AssetReference) this.m_loadingTextureName, new AssetHandleCallback<Texture>(this.OnTextureLoaded));
    }
  }

  private void OnTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> texture,
    object callbackData)
  {
    using (texture)
    {
      if (assetRef.ToString() != this.m_loadingTextureName)
        return;
      if (!(bool) texture)
      {
        Error.AddDevFatal("PlayerPortrait.OnTextureLoaded() - Failed to load {0}. ProgramId={1}", (object) assetRef, (object) this.m_programId);
        this.m_currentTextureName = (string) null;
        this.m_loadingTextureName = (string) null;
      }
      else
      {
        this.m_currentTextureName = this.m_loadingTextureName;
        this.m_loadingTextureName = (string) null;
        AssetHandle.Set<Texture>(ref this.m_loadedTexture, texture);
        RendererExtension.GetMaterial(this.GetComponent<Renderer>()).mainTexture = (Texture) texture;
      }
    }
  }
}
