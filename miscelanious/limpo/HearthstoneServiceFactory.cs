using Blizzard.T5.Core.Time;
using Blizzard.T5.Fonts;
using Blizzard.T5.MaterialService;
using Blizzard.T5.Services;
using Hearthstone.UI;
using System;
using System.Collections.Generic;

public static class HearthstoneServiceFactory
{
  public static readonly Dictionary<System.Type, Func<IService>> ServiceConstructors = new Dictionary<System.Type, Func<IService>>()
  {
    {
      typeof (UniversalInputManager),
      (Func<IService>) (() => (IService) new UniversalInputManager())
    },
    {
      typeof (SoundManager),
      (Func<IService>) (() => (IService) new SoundManager())
    },
    {
      typeof (FullScreenFXMgr),
      (Func<IService>) (() => (IService) new FullScreenFXMgr())
    },
    {
      typeof (IAssetLoader),
      (Func<IService>) (() => (IService) new AssetLoader())
    },
    {
      typeof (IAliasedAssetResolver),
      (Func<IService>) (() => (IService) new AliasedAssetResolver())
    },
    {
      typeof (IFontTable),
      (Func<IService>) (() => (IService) new FontTable((IFontLoader) new FontLoader(Log.Font)))
    },
    {
      typeof (IGraphicsManager),
      (Func<IService>) (() => (IService) new GraphicsManager())
    },
    {
      typeof (ShaderTime),
      (Func<IService>) (() => (IService) new ShaderTime())
    },
    {
      typeof (GameDbf),
      (Func<IService>) (() => (IService) new GameDbf())
    },
    {
      typeof (WidgetRunner),
      (Func<IService>) (() => (IService) new WidgetRunner())
    },
    {
      typeof (SpriteAtlasProvider),
      (Func<IService>) (() => (IService) new SpriteAtlasProvider())
    },
    {
      typeof (SpellManager),
      (Func<IService>) (() => (IService) new SpellManager())
    },
    {
      typeof (IMaterialService),
      (Func<IService>) (() => (IService) new Blizzard.T5.MaterialService.MaterialService((ITimeProvider) new UnityTimeProvider()))
    },
    {
      typeof (DiamondRenderToTextureService),
      (Func<IService>) (() => (IService) new DiamondRenderToTextureService())
    },
    {
      typeof (LegendaryHeroRenderToTextureService),
      (Func<IService>) (() => (IService) new LegendaryHeroRenderToTextureService())
    },
    {
      typeof (IGameStringsService),
      (Func<IService>) (() => (IService) new GameStringsService())
    },
    {
      typeof (ITouchScreenService),
      (Func<IService>) (() => (IService) new W8Touch())
    }
  };

  public static IServiceFactory CreateServiceFactory() => (IServiceFactory) new HearthstoneServiceFactory.HearthstoneServiceFactoryPrivate();

  private class HearthstoneServiceFactoryPrivate : IServiceFactory
  {
    public bool TryCreateService(System.Type serviceType, out IService service)
    {
      Func<IService> func;
      if (HearthstoneServiceFactory.ServiceConstructors.TryGetValue(serviceType, out func))
      {
        service = func();
        return true;
      }
      service = (IService) null;
      return false;
    }
  }
}
